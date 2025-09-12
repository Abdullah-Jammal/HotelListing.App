using HotelListing.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {

        private static List<Hotel> HotelsList = new List<Hotel>
        {
            new Hotel
            {
                Id = 1,
                Name = "Hotel 1",
                Address = "Address 1",
                Rating = 4.5
            },
            new Hotel
            {
                Id = 2,
                Name = "Hotel 2",
                Address = "Address 2",
                Rating = 3.8
            }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Hotel>> Get()
        {
            return Ok(HotelsList);
        }

        [HttpGet("{id}")]
        public ActionResult<IEnumerable<Hotel>> Get(int id)
        {
            var hotel = HotelsList.FirstOrDefault(h => h.Id == id);
            if (hotel == null)
            {
                return NotFound();
            }
            return Ok(hotel);
        }

        [HttpPost]
        public ActionResult<Hotel> Post([FromBody] Hotel newHotel)
        {
            if (HotelsList.Any(h => h.Id == newHotel.Id))
            {
                return Conflict("A hotel with the same ID already exists.");
            }
            HotelsList.Add(newHotel);
            return CreatedAtAction(nameof(Get), new { id = newHotel.Id }, newHotel);
        }

        [HttpPut("{id}")]
        public ActionResult<Hotel> Put(int id, [FromBody] Hotel updatedHoel)
        {
            var existingHotel = HotelsList.FirstOrDefault(h => h.Id == id);
            if (existingHotel == null)
            {
                return NotFound();
            }

            existingHotel.Name = updatedHoel.Name;
            existingHotel.Address = updatedHoel.Address;
            existingHotel.Rating = updatedHoel.Rating;
            return Ok(existingHotel);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var existingHotel = HotelsList.FirstOrDefault(h => h.Id == id);
            if (existingHotel == null)
            {
                return NotFound(new {message = "Hotel not found!"});
            }
            HotelsList.Remove(existingHotel);
            return NoContent();
        }
    }
}
