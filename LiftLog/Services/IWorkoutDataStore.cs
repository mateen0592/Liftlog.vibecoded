using LiftLog.Models;

namespace LiftLog.Services;

public interface IWorkoutDataStore
{
    IReadOnlyList<WorkoutPlan> GetPlans();
    IReadOnlyList<WorkoutLog> GetWorkouts();
    void AddPlan(WorkoutPlan plan);
    void AddWorkout(WorkoutLog workout);
}
