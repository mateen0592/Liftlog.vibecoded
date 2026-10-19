using LiftLog.Models;
using LiftLog.Services;
using Microsoft.AspNetCore.Mvc;

namespace LiftLog.Controllers;

public class WorkoutPlansController : Controller
{
    private readonly IExerciseService _exerciseService;
    private readonly IWorkoutDataStore _workoutDataStore;

    public WorkoutPlansController(IExerciseService exerciseService, IWorkoutDataStore workoutDataStore)
    {
        _exerciseService = exerciseService;
        _workoutDataStore = workoutDataStore;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(CreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(WorkoutPlanFormViewModel form)
    {
        var availableExercises = _exerciseService.GetAll();
        var selectedExercises = availableExercises
            .Where(exercise => form.SelectedExerciseIds.Contains(exercise.Id))
            .ToList();

        if (string.IsNullOrWhiteSpace(form.Name))
        {
            ModelState.AddModelError(nameof(form.Name), "Enter a plan name.");
        }

        if (selectedExercises.Count == 0)
        {
            ModelState.AddModelError(nameof(form.SelectedExerciseIds), "Select at least one exercise.");
        }

        if (selectedExercises.Count != form.SelectedExerciseIds.Distinct().Count())
        {
            ModelState.AddModelError(nameof(form.SelectedExerciseIds), "Select exercises from the available list.");
        }

        if (!ModelState.IsValid)
        {
            form.AvailableExercises = availableExercises;
            form.SavedPlans = _workoutDataStore.GetPlans();
            return View(form);
        }

        _workoutDataStore.AddPlan(new WorkoutPlan { Name = form.Name.Trim(), Exercises = selectedExercises });
        return RedirectToAction(nameof(Index));
    }

    private WorkoutPlanFormViewModel CreateViewModel() => new()
    {
        AvailableExercises = _exerciseService.GetAll(),
        SavedPlans = _workoutDataStore.GetPlans()
    };
}
