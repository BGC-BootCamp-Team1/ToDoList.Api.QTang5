using Microsoft.Extensions.Options;
using TodoItem.Infrastructure;
using TodoItems.Core;
using ToDoList.Api;
using ToDoList.Api.Services;

var builder = WebApplication.CreateBuilder(args);

//configure DB settings
builder.Services.Configure<ToDoItemDatabaseSettings>(builder.Configuration.GetSection("ToDoItemDatabase"));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IToDoItemsService, InMongoDBToDoItemsService>();
builder.Services.AddSingleton<ITodoItemsServiceV2, TodoItemService>();
builder.Services.AddSingleton<ITodosRepository, TodoItemMongoRepository>();

/*builder.Services.AddSingleton<ITodosRepository>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<TodoStoreDatabaseSettings>>().Value;
    return new TodoItemMongoRepository(Options.Create(settings));
});*/
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.Run();
public partial class Program { }