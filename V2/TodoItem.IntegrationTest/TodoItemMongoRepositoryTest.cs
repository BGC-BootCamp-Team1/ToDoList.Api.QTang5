using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using TodoItem.Infrastructure;
using TodoItems.Core;
using FluentAssertions;

namespace TodoItem.IntegrationTest;

public class TodoItemMongoRepositoryTest: IAsyncLifetime
{
    private readonly TodoItemMongoRepository _mongoRepository;
    private IMongoCollection<TodoItemPo> _mongoCollection;


    public TodoItemMongoRepositoryTest()
    {
        var mockSettings = new Mock<IOptions<TodoStoreDatabaseSettings>>();
        
        mockSettings.Setup(s => s.Value).Returns(new TodoStoreDatabaseSettings
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "TodoTestStore",
            TodoItemsCollectionName = "Todos"
        });

        // 初始化 TodoService
        _mongoRepository = new TodoItemMongoRepository(mockSettings.Object);
        
        var mongoClient = new MongoClient("mongodb://localhost:27017");
        var mongoDatabase = mongoClient.GetDatabase("TodoTestStore");
        _mongoCollection = mongoDatabase.GetCollection<TodoItemPo>("Todos");
    }
    
    // IAsyncLifetime 中的 InitializeAsync 方法在每个测试前运行
    public async Task InitializeAsync()
    {
        // 清空集合
        await _mongoCollection.DeleteManyAsync(FilterDefinition<TodoItemPo>.Empty);
    }

    // DisposeAsync 在测试完成后运行（如果有需要的话）
    public Task DisposeAsync() => Task.CompletedTask;


    [Fact]
    public async void should_return_item_by_id_1()
    {
        //await InitializeAsync();
        var todoItemPo = new TodoItemPo{
            Id = "5f9a7d8e2d3b4a1eb8a7d8e1", 
            Description = "Buy groceries",
            IsComplete = false
        };
        await _mongoCollection.InsertOneAsync(todoItemPo);
        var todoItem = await _mongoRepository.FindByIdAsync("5f9a7d8e2d3b4a1eb8a7d8e1");
        
        Assert.NotNull(todoItem);
        Assert.Equal("5f9a7d8e2d3b4a1eb8a7d8e1", todoItem.Id);
        Assert.Equal("Buy groceries", todoItem.Description);
    }

    [Fact]
    public async Task ShouldReturnTodosCount_ByDueDateAsync()
    {
        DateOnly dueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var todoItemPo_1 = new TodoItemPo
        {
            Id = "5f9a7d8e2d3b4a1eb8a7d8e2",
            Description = "Des_1",
            DueDate = dueDate,
        };
        var todoItemPo_2 = new TodoItemPo
        {
            Id = "5f9a7d8e2d3b4a1eb8a7d8e3",
            Description = "Des_2",
            DueDate = dueDate,
        };
        await _mongoCollection.InsertOneAsync(todoItemPo_1);
        await _mongoCollection.InsertOneAsync(todoItemPo_2);

        var count = _mongoRepository.CountTodoItemsByDueDate(dueDate);

        Assert.Equal(2, count);
    }
    [Fact]
    public async Task ShouldSave_WhenNewTodoAsync()
    {
        var todoItemPo = new TodoItemPo
        {
            Id = "5f9a7d8e2d3b4a1eb8a7d8e4",
            Description = "Buy groceries",
            IsComplete = false
        };
        var todoItem = await _mongoRepository.FindByIdAsync("5f9a7d8e2d3b4a1eb8a7d8e4");

        Assert.Null(todoItem);

        var expectTodoItem = todoItemPo.ConvertToTodoItem();
        await _mongoRepository.SaveAsync(expectTodoItem);       
        var actualTodoItem = await _mongoRepository.FindByIdAsync("5f9a7d8e2d3b4a1eb8a7d8e4");

        actualTodoItem.Should().BeEquivalentTo(expectTodoItem);
    }
    [Fact]
    public async Task ShouldUpdate_WhenExistTodoAsync()
    {
        var oldTodoItemPo = new TodoItemPo
        {
            Id = "5f9a7d8e2d3b4a1eb8a7d8e5",
            Description = "Buy groceries",
            IsComplete = false
        };
        var oldTodoItem = oldTodoItemPo.ConvertToTodoItem();
        await _mongoRepository.SaveAsync(oldTodoItem);

        var newTodoItem = oldTodoItem;
        newTodoItem.Description = "New Des";
        await _mongoRepository.SaveAsync(newTodoItem);


        var actualTodoItem = await _mongoRepository.FindByIdAsync("5f9a7d8e2d3b4a1eb8a7d8e5");

        actualTodoItem.Should().BeEquivalentTo(newTodoItem);
    }

    [Fact]
    public async Task ShouldSave_WhenNewTodo_FromService_Async()
    {
        var todoItemService = new TodoItemService(_mongoRepository);
        var description = "Test Todo Item";
        var dueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var newTodoItem = await todoItemService.CreateAsync(description, dueDate, TodoItemService.CreateOptionEnum.NextAvailableInFiveDaysOption);

        // Assert
        var filter = Builders<TodoItemPo>.Filter.Eq(x => x.Id, newTodoItem.Id);
        var savedTodoItemPo = await _mongoCollection.Find(filter).FirstOrDefaultAsync();
        Assert.NotNull(savedTodoItemPo);
        Assert.Equal(description, savedTodoItemPo.Description);
        Assert.Equal(dueDate, savedTodoItemPo.DueDate);
    }

    [Fact]
    public async Task CountTodoItemsInFiveDays_ShouldReturnCorrectCountsAsync()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.Today);
        var todos = new List<TodoItemPo>
        {
            new TodoItemPo { DueDate = today },
            new TodoItemPo { DueDate = today.AddDays(1) },
            new TodoItemPo { DueDate = today.AddDays(1) },
            new TodoItemPo { DueDate = today.AddDays(2) },
            new TodoItemPo { DueDate = today.AddDays(4) }
        };

        await _mongoCollection.InsertManyAsync(todos);

        // Act
        var result = _mongoRepository.CountTodoItemsInFiveDays();

        // Assert
        Assert.Equal(new List<int> { 1, 2, 1, 0, 1 }, result);
    }

}