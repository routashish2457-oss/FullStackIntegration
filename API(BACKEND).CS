using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

var tasks = new List<TaskItem>
{
    new TaskItem { Id = 1, Title = "Setup project", Completed = true },
    new TaskItem { Id = 2, Title = "Build API", Completed = false }
};

app.MapGet("/api/tasks", () => tasks);
app.MapPost("/api/tasks", (TaskItem newTask) =>
{
    newTask.Id = tasks.Count == 0 ? 1 : tasks.Max(x => x.Id) + 1;
    tasks.Add(newTask);
    return Results.Created($"/api/tasks/{newTask.Id}", newTask);
});
app.MapPut("/api/tasks/{id:int}", (int id, TaskItem update) =>
{
    var t = tasks.FirstOrDefault(x => x.Id == id);
    if (t is null) return Results.NotFound();
    t.Title = update.Title;
    t.Completed = update.Completed;
    return Results.Ok(t);
});
app.MapDelete("/api/tasks/{id:int}", (int id) =>
{
    var t = tasks.FirstOrDefault(x => x.Id == id);
    if (t is null) return Results.NotFound();
    tasks.Remove(t);
    return Results.NoContent();
});

app.Run();

record TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public bool Completed { get; set; }
}



