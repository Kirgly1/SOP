using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace SOP.Controllers
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string WagonStation { get; set; }
    }

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
    }


    public class Links
    {
        public string Self { get; set; }
        public string Update { get; set; }
        public string Delete { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class TrainController : ControllerBase
    {
        private readonly IMongoCollection<WagonResource> _wagons;

        public TrainController(IMongoClient client, IOptions<MongoDBSettings> mongoSettings)
        {
            var database = client.GetDatabase(mongoSettings.Value.WagonStation);
            _wagons = database.GetCollection<WagonResource>("wagons");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWagon(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Неверный формат Id.");
            }

            var wagon = await _wagons.Find(w => w.Id == objectId).FirstOrDefaultAsync();
            if (wagon == null)
            {
                return NotFound();
            }

            var wagonResource = new WagonResource
            {
                Id = wagon.Id,
                Cargo = wagon.Cargo,
                Capacity = wagon.Capacity,
                Loaded = wagon.Loaded,
                IsLoaded = wagon.IsLoaded,
                Links = new Links
                {
                    Self = Url.Link("GetWagon", new { id = wagon.Id.ToString() }),
                    Update = Url.Link("UpdateWagon", new { id = wagon.Id.ToString() }),
                    Delete = Url.Link("DeleteWagon", new { id = wagon.Id.ToString() })
                }
            };

            return Ok(wagonResource);
        }


        [HttpGet]
        public async Task<IActionResult> GetWagons()
        {
            var wagons = await _wagons.Find(w => true).ToListAsync();
            var wagonsWithLinks = wagons.Select(w => new
            {
                w.Id,
                w.Cargo,
                w.Capacity,
                w.Loaded,
                w.IsLoaded,
                Links = new List<object>
                {
                    new { rel = "self", href = Url.Action("GetWagon", new { id = w.Id.ToString() }) },
                    new { rel = "load", href = Url.Action("LoadWagon", new { id = w.Id.ToString() }) }
                }
            });

            return Ok(wagonsWithLinks);
        }

        [HttpPut("{id}/load")]
        public async Task<IActionResult> LoadWagon(string id, [FromBody] int loadAmount)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return BadRequest("Invalid ID format.");
            }

            var wagon = await _wagons.Find(w => w.Id == objectId).FirstOrDefaultAsync();
            if (wagon == null)
            {
                return NotFound();
            }

            if (wagon.Loaded + loadAmount > wagon.Capacity)
            {
                return BadRequest("Loading exceeds capacity.");
            }

            wagon.Loaded += loadAmount;
            if (wagon.Loaded >= wagon.Capacity)
            {
                wagon.IsLoaded = true;
            }

            await _wagons.ReplaceOneAsync(w => w.Id == objectId, wagon);

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWagon(string id, [FromBody] WagonResource updateWagon)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return BadRequest("Invalid ID format.");
            }

            var wagon = await _wagons.Find(w => w.Id == objectId).FirstOrDefaultAsync();
            if (wagon == null)
            {
                return NotFound();
            }

            wagon.Cargo = updateWagon.Cargo;
            wagon.Capacity = updateWagon.Capacity;
            wagon.Loaded = updateWagon.Loaded;
            wagon.IsLoaded = updateWagon.IsLoaded;

            await _wagons.ReplaceOneAsync(w => w.Id == objectId, wagon);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWagon(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return BadRequest("Invalid ID format.");
            }

            var wagon = await _wagons.Find(w => w.Id == objectId).FirstOrDefaultAsync();
            if (wagon == null)
            {
                return NotFound();
            }

            await _wagons.DeleteOneAsync(w => w.Id == objectId);

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateWagon([FromBody] WagonResource newWagon)
        {
            newWagon.Id = ObjectId.GenerateNewId();

            await _wagons.InsertOneAsync(newWagon);
            return CreatedAtAction(nameof(GetWagon), new { id = newWagon.Id.ToString() }, newWagon);
        }
    }
}
