using System.ComponentModel.DataAnnotations;

namespace LiftLog.Models;

public class WorkoutLogFormViewModel
{
    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.Today;

    public List<ExerciseLogInput> ExerciseEntries { get; set; } = [];
    public IReadOnlyList<Exercise> AvailableExercises { get; set; } = [];
    public IReadOnlyList<WorkoutLog> SavedWorkouts { get; set; } = [];
}

public class ExerciseLogInput
{
    public int ExerciseId { get; set; }
    public List<WorkoutSetInput> Sets { get; set; } = [];
}

public class WorkoutSetInput
{
    public int SetNumber { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Weight must be zero or greater.")]
    public decimal? WeightKg { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Repetitions must be greater than zero.")]
    public int? Repetitions { get; set; }
}
