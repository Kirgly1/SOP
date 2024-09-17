using Microsoft.AspNetCore.Mvc;

namespace SOP.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class Wagon
    {
        public int Id { get; set; }
        public string Cargo { get; set; }
        public int Capacity { get; set; }
        public int Loaded { get; set; }
        public bool IsLoaded { get; set; }
    }

    public class LoadingStationController : ControllerBase
    {
        private static List<Wagon> _wagons = new List<Wagon>()
        {
            new Wagon() { Id =1, Cargo = "Coal", Capacity = 73, Loaded = 0, IsLoaded = false },
            new Wagon() { Id =2, Cargo = "Gravel", Capacity = 77, Loaded=0, IsLoaded = false },
        };

        [HttpGet("{id}")]
        public IActionResult GetWagon(int id)
        {
            var wagon = _wagons.FirstOrDefault(w => w.Id == id);
            if (wagon == null)
                return NotFound();

            var wagonWithLinks = new
            {
                wagon.Id,
                wagon.Cargo,
                wagon.Capacity,
                wagon.Loaded,
                wagon.IsLoaded,
                Links = new List<object>
                {
                    new { rel = "self", href = Url.Action("GetWagon", new { id = wagon.Id }) },
                    new { rel = "load", href = Url.Action("LoadWagon", new { id = wagon.Id }) },
                    new { rel = "update", href = Url.Action("UpdateWagon", new[] { wagon.Id }) },
                    new { rel = "delete", href = Url.Action("DeleteWagon", new[] { wagon.Id }) },
                    new { rel = "allWagons", href = Url.Action("GetWagons")}
                }
            };

            return Ok(wagonWithLinks);

        }
        [HttpGet]
        public IActionResult GetWagons()
        {
            var wagonsWithLinks = _wagons.Select(w => new
            {
                w.Id,
                w.Cargo,
                w.Capacity,
                w.Loaded,
                w.IsLoaded,
                Links = new List<object>
                {
                    new { rel = "self", href = Url.Action("GetWagon", new {id = w.Id}) },
                    new { rel = "load", href = Url.Action("LoadWagon", new {id =w.Id}) }
                }
            });

            return Ok(wagonsWithLinks);
        }

        [HttpPut("load/{id}")]
        public IActionResult LoadWagon(int id, [FromBody] int loadAmount)
        {
            var wagon = _wagons.FirstOrDefault(w => w.Id == id);
            if (wagon == null)
                return NotFound();

            if (wagon.Loaded + loadAmount > wagon.Capacity)
                return BadRequest("Погрузка невозможна");

            wagon.Loaded += loadAmount;
            if (wagon.Loaded > wagon.Capacity)
                wagon.IsLoaded = true;

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateWagon(int id, [FromBody] Wagon updateWagon)
        {
            var wagon = _wagons.FirstOrDefault(w => w.Id == id);
            if(wagon == null)
                return NotFound();

            wagon.Cargo = updateWagon.Cargo;
            wagon.Capacity = updateWagon.Capacity;
            wagon.Loaded = updateWagon.Capacity; 
            wagon.IsLoaded = updateWagon.IsLoaded;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteWagon(int id)
        {
            var wagon = _wagons.FirstOrDefault(w =>w.Id == id);
            if ( wagon == null)
                return NotFound();

            _wagons.Remove(wagon);
            return NoContent();
        }
    } 
}
   