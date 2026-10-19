using LiftLog.Models;
using LiftLog.Services;
using Microsoft.AspNetCore.Mvc;

namespace LiftLog.Controllers;

public class WorkoutLogsController : Controller
{
    private const int SetRowsPerExercise = 3;
    private readonly IExerciseService _exerciseService;
    private readonly IWorkoutDataStore _workoutDataStore;

    public WorkoutLogsController(IExerciseService exerciseService, IWorkoutDataStore workoutDataStore)
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
    public IActionResult Index(WorkoutLogFormViewModel form)
    {
        var availableExercises = _exerciseService.GetAll();
        var loggedExercises = new List<LoggedExercise>();
        var completedSetCount = 0;

        for (var exerciseIndex = 0; exerciseIndex < form.ExerciseEntries.Count; exerciseIndex++)
        {
            var entry = form.ExerciseEntries[exerciseIndex];
            var exercise = availableExercises.FirstOrDefault(item => item.Id == entry.ExerciseId);
            if (exercise is null)
            {
                ModelState.AddModelError($"ExerciseEntries[{exerciseIndex}].ExerciseId", "Choose an available exercise.");
                continue;
            }

            var completedSets = new List<WorkoutSet>();
            for (var setIndex = 0; setIndex < entry.Sets.Count; setIndex++)
            {
                var set = entry.Sets[setIndex];
                var hasWeight = set.WeightKg.HasValue;
                var hasRepetitions = set.Repetitions.HasValue;

                if (!hasWeight && !hasRepetitions)
                {
                    continue;
                }

                if (hasWeight != hasRepetitions)
                {
                    ModelState.AddModelError($"ExerciseEntries[{exerciseIndex}].Sets[{setIndex}]", "Enter both weight and repetitions for each set.");
                    continue;
                }

                if (set.SetNumber < 1 || set.SetNumber > SetRowsPerExercise)
                {
                    ModelState.AddModelError($"ExerciseEntries[{exerciseIndex}].Sets[{setIndex}].SetNumber", "Invalid set number.");
                    continue;
                }

                completedSets.Add(new WorkoutSet
                {
                    SetNumber = set.SetNumber,
                    WeightKg = set.WeightKg!.Value,
                    Repetitions = set.Repetitions!.Value
                });
                completedSetCount++;
            }

            if (completedSets.Count > 0)
            {
                loggedExercises.Add(new LoggedExercise { Exercise = exercise, Sets = completedSets });
            }
        }

        if (completedSetCount == 0)
        {
            ModelState.AddModelError(string.Empty, "Enter at least one complete set.");
        }

        if (!ModelState.IsValid)
        {
            form.AvailableExercises = availableExercises;
            form.SavedWorkouts = _workoutDataStore.GetWorkouts();
            return View(form);
        }

        _workoutDataStore.AddWorkout(new WorkoutLog { Date = form.Date.Date, Exercises = loggedExercises });
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult History()
    {
        var savedWorkouts = _workoutDataStore.GetWorkouts();
        var sessions = savedWorkouts
            .Select((workout, index) => new { Workout = workout, Index = index })
            .OrderByDescending(item => item.Workout.Date)
            .ThenByDescending(item => item.Index)
            .Select(item => item.Workout)
            .ToList();

        var comparisons = savedWorkouts
            .SelectMany((workout, index) => workout.Exercises.Select(loggedExercise => new
            {
                Workout = workout,
                LoggedExercise = loggedExercise,
                Index = index
            }))
            .GroupBy(item => item.LoggedExercise.Exercise.Id)
            .Select(group =>
            {
                var performances = group
                    .OrderByDescending(item => item.Workout.Date)
                    .ThenByDescending(item => item.Index)
                    .Take(2)
                    .ToList();

                return new ExercisePerformanceComparison
                {
                    Exercise = performances[0].LoggedExercise.Exercise,
                    LatestWorkout = performances[0].Workout,
                    PreviousWorkout = performances.Count > 1 ? performances[1].Workout : null
                };
            })
            .OrderBy(item => item.Exercise.Name)
            .ToList();

        return View(new WorkoutHistoryViewModel
        {
            Sessions = sessions,
            Comparisons = comparisons
        });
    }

    private WorkoutLogFormViewModel CreateViewModel()
    {
        var exercises = _exerciseService.GetAll();
        return new WorkoutLogFormViewModel
        {
            AvailableExercises = exercises,
            SavedWorkouts = _workoutDataStore.GetWorkouts(),
            ExerciseEntries = exercises.Select(exercise => new ExerciseLogInput
            {
                ExerciseId = exercise.Id,
                Sets = Enumerable.Range(1, SetRowsPerExercise)
                    .Select(number => new WorkoutSetInput { SetNumber = number })
                    .ToList()
            }).ToList()
        };
    }
}
