using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TodoItem.Infrastructure;
using ToDoList.Api.Models;

namespace ToDoList.Api.ApiTests
{
    public class ToDoListV2ControllerTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private IMongoCollection<TodoItemPo> _mongoCollection;

        public ToDoListV2ControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();

            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var mongoDatabase = mongoClient.GetDatabase("ToDoItems");
            _mongoCollection = mongoDatabase.GetCollection<TodoItemPo>("ToDoItems");
        }

        public async Task InitializeAsync()
        {
            await _mongoCollection.DeleteManyAsync(FilterDefinition<TodoItemPo>.Empty);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async void Should_Create_ToDoItem()
        {
            // Arrange
            var todoItem = new ToDoItemCreateRequest
            {
                Description = "Buy groceries V2",
                Done = false,
            };

            var content = new StringContent(JsonSerializer.Serialize(todoItem), System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v2/todolistv2", content);

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
           
        }

        [Fact]
        public async Task Should_Update_ToDoItem()
        {
            // Arrange
            var todoItem = new TodoItemPo
            {
                Id = "5f9a7d8e2d3b4a1eb8a7d8e2",
                Description = "Buy groceries",
                ModificationHistory = []
            };

            await _mongoCollection.InsertOneAsync(todoItem);

            var updatedItem = new ToDoItemDto
            {
                Id = todoItem.Id,
                Description = "Buy groceries and cook dinner",
                Done = true,
                Favorite = false
            };

            var content = new StringContent(JsonSerializer.Serialize(updatedItem), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/api/v2/todolistv2/{todoItem.Id}", content);

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
        }
    }
}
