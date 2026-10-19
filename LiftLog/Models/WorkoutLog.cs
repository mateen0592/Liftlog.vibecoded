namespace LiftLog.Models;

public class WorkoutLog
{
    public DateTime Date { get; set; }
    public IReadOnlyList<LoggedExercise> Exercises { get; set; } = [];
}

public class LoggedExercise
{
    public Exercise Exercise { get; set; } = new();
    public IReadOnlyList<WorkoutSet> Sets { get; set; } = [];
}

public class WorkoutSet
{
    public int SetNumber { get; set; }
    public decimal WeightKg { get; set; }
    public int Repetitions { get; set; }
}
