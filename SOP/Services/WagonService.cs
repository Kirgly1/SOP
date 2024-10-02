using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;
using SOP.Controllers;
using Microsoft.Extensions.Options;
using SOP.Entity;
using SOP.Mongo;

namespace SOP.Services
{
    public class WagonService
    {
        private readonly IMongoCollection<WagonResource> _wagons;

        public WagonService(IOptions<MongoDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.WagonStation);
            _wagons = database.GetCollection<WagonResource>("wagons");
        }

        public async Task<WagonResource> GetWagonByIdAsync(string id)
        {
            return await _wagons.Find(w => w.Id == ObjectId.Parse(id)).FirstOrDefaultAsync();
        }

        public async Task<List<WagonResource>> GetAllWagonsAsync()
        {
            return await _wagons.Find(w => true).ToListAsync();
        }

        public async Task CreateWagonAsync(WagonResource newWagon)
        {
            if (newWagon.Id == ObjectId.Empty)
            {
                newWagon.Id = ObjectId.GenerateNewId();
            }

            await _wagons.InsertOneAsync(newWagon);
        }

        public async Task UpdateWagonAsync(string id, WagonResource updateWagon)
        {
            await _wagons.ReplaceOneAsync(w => w.Id == ObjectId.Parse(id), updateWagon);
        }

        public async Task DeleteWagonAsync(string id)
        {
            await _wagons.DeleteOneAsync(w => w.Id == ObjectId.Parse(id));
        }

        public async Task LoadWagonAsync(string id, int loadAmount)
        {
            var wagon = await GetWagonByIdAsync(id);
            if (wagon == null)
            {
                throw new System.Exception("Вагон не найден");
            }

            if (wagon.Loaded + loadAmount > wagon.Capacity)
            {
                throw new Exception("Погрузка невозможна");
            }

            wagon.Loaded += loadAmount;
            if (wagon.Loaded >= wagon.Capacity)
            {
                wagon.IsLoaded = true;
            }

            await UpdateWagonAsync(id, wagon);
        }
    }
}
