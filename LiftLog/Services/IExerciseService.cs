using LiftLog.Models;

namespace LiftLog.Services;

public interface IExerciseService
{
    IReadOnlyList<Exercise> GetAll();
}
