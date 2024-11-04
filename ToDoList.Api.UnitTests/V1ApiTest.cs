using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Driver;
using System.Net;
using System.Text.Json;
using ToDoList.Api.Models;

namespace ToDoList.Api.ApiTests;

public class ToDoListControllerTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private IMongoCollection<MongoDBToDoItem> _mongoCollection;

    public ToDoListControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        var mongoClient = new MongoClient("mongodb://localhost:27017");
        var mongoDatabase = mongoClient.GetDatabase("ToDoItems");
        _mongoCollection = mongoDatabase.GetCollection<MongoDBToDoItem>("ToDoItems");
    }

    public async Task InitializeAsync()
    {
        await _mongoCollection.DeleteManyAsync(FilterDefinition<MongoDBToDoItem>.Empty);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async void Should_Create_ToDoItem()
    {
        // Arrange
        var todoItem = new ToDoItemCreateRequest
        {
            Description = "Buy groceries",
            Done = false,
            Favorite = true
        };

        var content = new StringContent(JsonSerializer.Serialize(todoItem), System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/todolist", content);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        var createdItem = JsonSerializer.Deserialize<ToDoItemDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(createdItem);
        Assert.Equal(todoItem.Description, createdItem.Description);
        Assert.Equal(todoItem.Done, createdItem.Done);
        Assert.Equal(todoItem.Favorite, createdItem.Favorite);
    }

    [Fact]
    public async void Should_Update_ToDoItem()
    {
        // Arrange
        var todoItem = new MongoDBToDoItem
        {
            Id = "5f9a7d8e2d3b4a1eb8a7d8e2",
            Description = "Buy groceries",
            Done = false,
            Favorite = true,
            CreatedTime = DateTime.Now,
        };

        await _mongoCollection.InsertOneAsync(todoItem);

        var updatedItem = new ToDoItemDto
        {
            Id = todoItem.Id,
            Description = "Buy groceries and cook dinner",
            Done = true,
            Favorite = false
        };

        var content = new StringContent(JsonSerializer.Serialize(updatedItem), System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/v1/todolist/{todoItem.Id}", content);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        var returnedItem = JsonSerializer.Deserialize<ToDoItemDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(returnedItem);
        Assert.Equal(updatedItem.Description, returnedItem.Description);
        Assert.Equal(updatedItem.Done, returnedItem.Done);
        Assert.Equal(updatedItem.Favorite, returnedItem.Favorite);
    }
}
