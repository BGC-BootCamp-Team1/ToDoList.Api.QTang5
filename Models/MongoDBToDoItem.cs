using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ToDoList.Api.Models
{
        [BsonIgnoreExtraElements]
        public class MongoDBToDoItem
        {
            [BsonId]
            public required string Id { get; init; }
            public required string Description { get; set; }
            public required bool Done { get; set; }
            public required bool Favorite { get; set; }

            [BsonRepresentation(BsonType.String)]
            public required DateTimeOffset CreatedTime { get; init; }
            public ToDoItemDto ConvertToDto()
            {
                return new ToDoItemDto()
                {
                    Id = this.Id,
                    CreatedTime = this.CreatedTime,
                    Favorite = this.Favorite,
                    Description = this.Description,
                    Done = this.Done
                };
            }
        
        }
    
}
