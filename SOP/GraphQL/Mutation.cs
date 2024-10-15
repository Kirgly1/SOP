namespace SOP.GraphQL
{
    using HotChocolate;
    using HotChocolate.Types;
    using MongoDB.Bson;
    using SOP.Controllers;
    using SOP.Services;
    using System.Threading.Tasks;
    using SOP.Entity;

    public class Mutation
    {
        private readonly WagonService _wagonService;

        public Mutation(WagonService wagonService)
        {
            _wagonService = wagonService;
        }

        public async Task<WagonResource> CreateWagonAsync(WagonInput input)
        {
            var wagon = new WagonResource
            {
                Id = ObjectId.GenerateNewId(),
                Cargo = input.Cargo,
                Capacity = input.Capacity,
                Loaded = input.Loaded,
                IsLoaded = input.IsLoaded
            };

            await _wagonService.CreateWagonAsync(wagon);
            return wagon;
        }

        public async Task<WagonResource> UpdateWagonAsync(string id, WagonInput input)
        {
            // Преобразуем строковый ID в ObjectId
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                throw new ArgumentException("Invalid ObjectId format.", nameof(id));
            }

            // Выполнение запроса с ObjectId
            var wagon = await _wagonService.GetWagonByIdAsync(objectId); // Здесь objectId
            if (wagon == null)
            {
                throw new Exception("Wagon not found.");
            }

            // Обновляем поля вагона
            wagon.Cargo = input.Cargo;
            wagon.Capacity = input.Capacity;
            wagon.Loaded = input.Loaded;
            wagon.IsLoaded = input.IsLoaded;

            // Обновляем в базе данных
            await _wagonService.UpdateWagonAsync(objectId, wagon); // Преобразовывать обратно не нужно

            return wagon;
        }

        public class CreateWagonInput
        {
            public string Cargo { get; set; }
            public int Capacity { get; set; }
            public int Loaded { get; set; }
            public bool IsLoaded { get; set; }
        }

    }
}
