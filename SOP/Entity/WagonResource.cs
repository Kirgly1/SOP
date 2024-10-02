using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SOP.Controllers;
using SOP.HyperMedia;

namespace SOP.Entity
{
    public class WagonResource
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Cargo")]
        public string Cargo { get; set; }

        [BsonElement("Capacity")]
        public int Capacity { get; set; }

        [BsonElement("Loaded")]
        public int Loaded { get; set; }

        [BsonElement("IsLoaded")]
        public bool IsLoaded { get; set; }

        public Links Links { get; set; }

        public string ObjectId => Id.ToString();
    }
}
