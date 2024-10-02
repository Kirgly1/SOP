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

        public async Task<WagonResource> CreateWagon(CreateWagonInput input)
        {
            var newWagon = new WagonResource
            {
                Id = ObjectId.GenerateNewId(),
                Cargo = input.Cargo,
                Capacity = input.Capacity,
                Loaded = input.Loaded,
                IsLoaded = input.IsLoaded
            };

            await _wagonService.CreateWagonAsync(newWagon);
            return newWagon;
        }
    }

    public class CreateWagonInput
    {
        public string Cargo { get; set; }
        public int Capacity { get; set; }
        public int Loaded { get; set; }
        public bool IsLoaded { get; set; }
    }

}
