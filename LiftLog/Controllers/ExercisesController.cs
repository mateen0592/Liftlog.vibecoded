using LiftLog.Services;
using Microsoft.AspNetCore.Mvc;

namespace LiftLog.Controllers;

public class ExercisesController : Controller
{
    private readonly IExerciseService _exerciseService;

    public ExercisesController(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    public IActionResult Index()
    {
        return View(_exerciseService.GetAll());
    }
}
