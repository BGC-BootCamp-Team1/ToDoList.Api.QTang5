using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Driver;
using System.Net;
using System.Text.Json;
using ToDoList.Api.Models;

namespace ToDoList.Api.ApiTests;

public class GetOneTodoItemTest : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{


    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private IMongoCollection<MongoDBToDoItem> _mongoCollection;

    public GetOneTodoItemTest(WebApplicationFactory<Program> factory) {
        _factory = factory;
        _client = _factory.CreateClient();

        var mongoClient = new MongoClient("mongodb://localhost:27017");
        var mongoDatabase = mongoClient.GetDatabase("ToDoItems");
        _mongoCollection = mongoDatabase.GetCollection<MongoDBToDoItem>("ToDoItems");
    }

    public async Task InitializeAsync()
    {
        // 清空集合
        await _mongoCollection.DeleteManyAsync(FilterDefinition<MongoDBToDoItem>.Empty);
    }

    // DisposeAsync 在测试完成后运行（如果有需要的话）
    public Task DisposeAsync() => Task.CompletedTask;


    [Fact]
    public async void should_get_todo_by_given_id()
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

        // Act
        var response = await _client.GetAsync("/api/v1/todoList/5f9a7d8e2d3b4a1eb8a7d8e2");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();

        var returnedTodos = JsonSerializer.Deserialize<ToDoItemDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(returnedTodos);
        Assert.Equal("Buy groceries", returnedTodos.Description);
        Assert.True(returnedTodos.Favorite);
        Assert.False(returnedTodos.Done);
    }

    [Fact]
    public async void should_get_NOTFOUND_by_invalid_id()
    {
        // Arrange
        
        // Act
        var response = await _client.GetAsync("/api/v1/todoitems/not_exist_id");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

}