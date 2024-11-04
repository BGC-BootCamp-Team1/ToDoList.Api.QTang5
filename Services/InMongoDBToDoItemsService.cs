using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ToDoList.Api.Models;
using static ToDoList.Api.Models.MongoDBToDoItem;

namespace ToDoList.Api.Services
{
    public class InMongoDBToDoItemsService : IToDoItemsService
    {
        private readonly IMongoCollection<MongoDBToDoItem> _MongoDBToDoItemsCollection;

        public InMongoDBToDoItemsService(
            IOptions<ToDoItemDatabaseSettings> ToDoItemStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                ToDoItemStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                ToDoItemStoreDatabaseSettings.Value.DatabaseName);

            _MongoDBToDoItemsCollection = mongoDatabase.GetCollection<MongoDBToDoItem>(
                ToDoItemStoreDatabaseSettings.Value.CollectionName);
        }
        public async Task CreateAsync(ToDoItemDto toDoItem)
        {
            await _MongoDBToDoItemsCollection.InsertOneAsync(toDoItem.ConvertToDB());
        }

        public async Task<List<ToDoItemDto>> GetAllAsync()
        {
            var mongoDBToDoItems = await _MongoDBToDoItemsCollection.Find(_ => true).ToListAsync();
            var ToDoItems = new List<ToDoItemDto>();

            foreach (var mongoDBToDoItem in mongoDBToDoItems)
            {
                ToDoItems.Add(mongoDBToDoItem.ConvertToDto());
            }
            return ToDoItems;
        }

        public async Task<ToDoItemDto> GetAsync(string id)
        {
            var toDoItem = await _MongoDBToDoItemsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            return toDoItem.ConvertToDto();
        }

        public async Task<bool> RemoveAsync(string id)
        {
            var result = await _MongoDBToDoItemsCollection.DeleteOneAsync(x => x.Id == id);
            if(result.DeletedCount == 0)
                return false;
            else
                return true;
        }

        public async Task ReplaceAsync(string id, ToDoItemDto updateToDoItem)
        {
            var result = await _MongoDBToDoItemsCollection.Find<MongoDBToDoItem>(x=>x.Id == id).FirstAsync();
            if(result != null)
            {
                updateToDoItem.CreatedTime = result.CreatedTime;
            }
            await _MongoDBToDoItemsCollection.ReplaceOneAsync(x => x.Id == id, updateToDoItem.ConvertToDB());
        }
    }
}
