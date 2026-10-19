using System.ComponentModel.DataAnnotations;

namespace LiftLog.Models;

public class WorkoutPlanFormViewModel
{
    [Required(ErrorMessage = "Enter a plan name.")]
    public string Name { get; set; } = string.Empty;

    public List<int> SelectedExerciseIds { get; set; } = [];
    public IReadOnlyList<Exercise> AvailableExercises { get; set; } = [];
    public IReadOnlyList<WorkoutPlan> SavedPlans { get; set; } = [];
}
