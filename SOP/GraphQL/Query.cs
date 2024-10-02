using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using SOP.Controllers;
using SOP.Services;
using SOP.Entity;
using SOP.Mongo;

namespace SOP.GraphQL
{
    public class Query
    {
        private readonly WagonService _wagonService;

        public Query(WagonService wagonService)
        {
            _wagonService = wagonService;
        }

        public async Task<IEnumerable<WagonResource>> Wagons() =>
            await _wagonService.GetAllWagonsAsync();
    }

}
