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
            var updatedPlans = _plans.Append(plan).ToList();
            SaveData(updatedPlans, _workouts);
            _plans = updatedPlans;
        }
    }

    public void AddWorkout(WorkoutLog workout)
    {
        lock (_lock)
        {
            var updatedWorkouts = _workouts.Append(workout).ToList();
            SaveData(_plans, updatedWorkouts);
            _workouts = updatedWorkouts;
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
            if (data is null)
            {
                throw new JsonException("The workout data file did not contain a data object.");
            }

            return (data?.Plans ?? [], data?.Workouts ?? []);
        }
        catch (JsonException exception)
        {
            var backupPath = PreserveInvalidFile();
            _logger.LogWarning(exception, "Invalid workout data at {FilePath}; starting with empty data. Backup: {BackupPath}", _filePath, backupPath ?? "not moved");
            return ([], []);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            _logger.LogWarning(exception, "Could not load workout data from {FilePath}; starting with empty data.", _filePath);
            return ([], []);
        }
    }

    private string? PreserveInvalidFile()
    {
        try
        {
            var backupPath = $"{_filePath}.corrupt-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}.json";
            File.Move(_filePath, backupPath);
            return backupPath;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            _logger.LogWarning(exception, "Could not move invalid workout data file {FilePath} to a backup.", _filePath);
            return null;
        }
    }

    private void SaveData(List<WorkoutPlan> plans, List<WorkoutLog> workouts)
    {
        var directory = Path.GetDirectoryName(_filePath)!;
        Directory.CreateDirectory(directory);
        var temporaryPath = _filePath + ".tmp";
        var json = JsonSerializer.Serialize(new StoredWorkoutData { Plans = plans, Workouts = workouts }, JsonOptions);

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
