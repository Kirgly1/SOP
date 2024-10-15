using MongoDB.Bson;
using SOP.Entity;
using SOP.Services;

public class Query
{
    private readonly WagonService _wagonService;

    public Query(WagonService wagonService)
    {
        _wagonService = wagonService;
    }

    public async Task<List<WagonResource>> GetWagonsAsync()
    {
        return await _wagonService.GetAllWagonsAsync();
    }

    public async Task<WagonResource> GetWagonByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out ObjectId objectId))
        {
            throw new ArgumentException("Invalid ObjectId format.", nameof(id));
        }

        return await _wagonService.GetWagonByIdAsync(objectId);
    }

    public async Task<WagonResource> Wagon(string id)
    {
        if (!ObjectId.TryParse(id, out ObjectId objectId))
        {
            throw new ArgumentException("Invalid ObjectId format.", nameof(id));
        }

        var wagon = await _wagonService.GetWagonByIdAsync(objectId);
        if (wagon == null)
        {
            throw new Exception("Wagon not found.");
        }

        return wagon;
    }

}