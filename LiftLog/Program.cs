var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<LiftLog.Services.IExerciseService, LiftLog.Services.ExerciseService>();
builder.Services.AddSingleton<LiftLog.Services.IWorkoutDataStore, LiftLog.Services.JsonWorkoutDataStore>();

var app = builder.Build();

// Load saved plans and workout history before serving requests.
_ = app.Services.GetRequiredService<LiftLog.Services.IWorkoutDataStore>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
