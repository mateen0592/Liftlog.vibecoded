namespace LiftLog.Models;

public class WorkoutPlan
{
    public string Name { get; set; } = string.Empty;
    public IReadOnlyList<Exercise> Exercises { get; set; } = [];
}
