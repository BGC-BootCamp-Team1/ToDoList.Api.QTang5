using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ToDoList.Api.Models;

namespace ToDoList.Api.ApiTests
{
    public class ToDoListV2ControllerTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private IMongoCollection<MongoDBToDoItem> _mongoCollection;

        public ToDoListV2ControllerTests(WebApplicationFactory<Program> factory)
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
                Description = "Buy groceries V2",
                Done = false,
                Favorite = true
            };

            var content = new StringContent(JsonSerializer.Serialize(todoItem), System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v2/todolist", content);

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
    }
}
