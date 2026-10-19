using LiftLog.Models;

namespace LiftLog.Services;

public class ExerciseService : IExerciseService
{
    private readonly IReadOnlyList<Exercise> _exercises =
    [
        new Exercise { Id = 1, Name = "Bench Press", MuscleGroup = "Chest" },
        new Exercise { Id = 2, Name = "Squat", MuscleGroup = "Legs" },
        new Exercise { Id = 3, Name = "Deadlift", MuscleGroup = "Back" },
        new Exercise { Id = 4, Name = "Shoulder Press", MuscleGroup = "Shoulders" },
        new Exercise { Id = 5, Name = "Lat Pulldown", MuscleGroup = "Back" }
    ];

    public IReadOnlyList<Exercise> GetAll() => _exercises;
}
