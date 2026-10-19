using System.Text.Json;
using LiftLog.Models;

namespace LiftLog.Services;

public class JsonWorkoutDataStore : IWorkoutDataStore
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly object _lock = new();
    private readonly string _filePath;
    private readonly ILogger<JsonWorkoutDataStore> _logger;
    private List<WorkoutPlan> _plans;
    private List<WorkoutLog> _workouts;

    public JsonWorkoutDataStore(IWebHostEnvironment environment, ILogger<JsonWorkoutDataStore> logger)
    {
        _logger = logger;
        var localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var defaultDirectory = string.IsNullOrWhiteSpace(localApplicationData)
            ? Path.Combine(environment.ContentRootPath, "App_Data")
            : Path.Combine(localApplicationData, "LiftLog");
        _filePath = Environment.GetEnvironmentVariable("LIFTLOG_DATA_PATH") is { Length: > 0 } overridePath
            ? Path.GetFullPath(overridePath)
            : Path.Combine(defaultDirectory, "workout-data.json");

        (_plans, _workouts) = LoadData();
    }

    public IReadOnlyList<WorkoutPlan> GetPlans()
    {
        lock (_lock)
        {
            return _plans.ToList();
        }
    }

    public IReadOnlyList<WorkoutLog> GetWorkouts()
    {
        lock (_lock)
        {
            return _workouts.ToList();
        }
    }

    public void AddPlan(WorkoutPlan plan)
    {
        lock (_lock)
        {
            _plans.Add(plan);
            SaveData();
        }
    }

    public void AddWorkout(WorkoutLog workout)
    {
        lock (_lock)
        {
            _workouts.Add(workout);
            SaveData();
        }
    }

    private (List<WorkoutPlan> Plans, List<WorkoutLog> Workouts) LoadData()
    {
        if (!File.Exists(_filePath))
        {
            return ([], []);
        }

        try
        {
            var json = File.ReadAllText(_filePath);
            var data = JsonSerializer.Deserialize<StoredWorkoutData>(json, JsonOptions);
            return (data?.Plans ?? [], data?.Workouts ?? []);
        }
        catch (Exception exception) when (exception is JsonException or IOException or UnauthorizedAccessException)
        {
            _logger.LogWarning(exception, "Could not load workout data from {FilePath}; starting with empty data.", _filePath);
            return ([], []);
        }
    }

    private void SaveData()
    {
        var directory = Path.GetDirectoryName(_filePath)!;
        Directory.CreateDirectory(directory);
        var temporaryPath = _filePath + ".tmp";
        var json = JsonSerializer.Serialize(new StoredWorkoutData { Plans = _plans, Workouts = _workouts }, JsonOptions);

        try
        {
            File.WriteAllText(temporaryPath, json);
            File.Move(temporaryPath, _filePath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    private sealed class StoredWorkoutData
    {
        public List<WorkoutPlan> Plans { get; set; } = [];
        public List<WorkoutLog> Workouts { get; set; } = [];
    }
}
