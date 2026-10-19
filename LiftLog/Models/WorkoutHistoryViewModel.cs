namespace LiftLog.Models;

public class WorkoutHistoryViewModel
{
    public IReadOnlyList<WorkoutLog> Sessions { get; set; } = [];
    public IReadOnlyList<ExercisePerformanceComparison> Comparisons { get; set; } = [];
}

public class ExercisePerformanceComparison
{
    public Exercise Exercise { get; set; } = new();
    public WorkoutLog LatestWorkout { get; set; } = new();
    public WorkoutLog? PreviousWorkout { get; set; }
}
