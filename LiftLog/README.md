# LiftLog

LiftLog is a small ASP.NET Core MVC workout tracker. It lets a user browse a fixed exercise library, save workout plans, record workouts and review workout history.

## Features

- Browse five built-in exercises: Bench Press, Squat, Deadlift, Shoulder Press and Lat Pulldown.
- Create and view named workout plans with one or more selected exercises.
- Record a workout date, exercises and multiple sets per exercise. Each set stores its number, weight in kilograms and repetition count.
- Review saved sessions and compare the latest dated performance for an exercise with its previous session.
- Keep plans and workout records in a JSON file between application restarts.

## Technology

- C# and ASP.NET Core MVC, targeting .NET 10.
- Razor views for the pages, with HTML forms posting to MVC controllers.
- JSON-file storage for plans and workout records; there is no database.
- Bootstrap and the project's CSS and JavaScript assets support the responsive interface and adding set rows.

## Run locally

Install the .NET 10 SDK, then from the repository directory run:

```powershell
cd LiftLog
dotnet run
```

Open the local URL printed by `dotnet run` in a browser. The main pages are `/`, `/Exercises`, `/WorkoutPlans`, `/WorkoutLogs` and `/WorkoutLogs/History`. Use the navigation links in the application to move between them.

By default, saved data is stored under the operating system's local application data directory in `LiftLog/workout-data.json`. Set `LIFTLOG_DATA_PATH` before starting the app to choose another JSON file location. Keep that runtime data file out of source control.

## Use the main features

1. Choose **Exercises** to browse the fixed exercise library.
2. Choose **Plans**, enter a plan name, select one or more exercises and submit the form.
3. Choose **Log workout**, select a date and exercises, and enter the weight and repetitions for each set. Use **Add another set** when an exercise needs more rows. Submit to save the session.
4. Choose **History** to review saved sessions and compare the latest and previous dated records for exercises they share.

Workout weights must be zero or greater and repetitions must be positive. Blank unused set rows are ignored. The interface supports up to 50 sets for each exercise in a workout.

## Known limitations

- The exercise list is fixed; exercises cannot be created, edited or deleted.
- Plans are saved and displayed but are not used to prefill workout logging.
- Data is local JSON with no accounts, database, cloud sync or multi-user support.
- If the JSON file is invalid, the application preserves it as a corrupt-file backup and starts with empty in-memory data.
- There is no automated test project in the repository.

## Testing performed

- `dotnet build` succeeded on a temporary copy of the project with 0 warnings and 0 errors.
- `node --check` succeeded for `wwwroot/js/workout-log.js`.
- HTTP smoke checks exercised saving a workout with multiple sets, history display, and rejection of duplicate set numbers, negative weights, zero repetitions and duplicate exercises. The checks also confirmed rejected submissions did not change saved records.
- Smoke checks confirmed plan and workout save errors are shown without adding failed records, and malformed JSON is backed up while the app serves an empty history.

These were build, syntax and manual HTTP smoke checks; no automated test suite was run.

## Canvas submission description

LiftLog is an ASP.NET Core MVC workout tracker for browsing a small exercise library, creating exercise-based plans, recording workouts with multiple sets and reviewing performance history. It stores plans and workout sessions locally in JSON between application restarts.


