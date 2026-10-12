using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_Part_2_Restful_API_Development.Data;
using RaceDay_Part_2_Restful_API_Development.Models; 

namespace RaceDay_Part_2_Restful_API_Development.Controllers
{//start of namespace 
    [Route("api/[controller]")]
    [ApiController]
    public class weatherController : ControllerBase
    {//start of class

        private readonly RaceDayDatabase _context;

        public weatherController(RaceDayDatabase context)
        {//start of constructor, this initializes the database context context through a dependency injection
            _context = context;
        }//end of constructor 
        
        [HttpGet]
        public async Task<ActionResult> GetWeather(int eventId)
        {//start of GET method 
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {//start of if statement 
                return NotFound(new { message = $"Event with ID {eventId} not found." });
            }//end of if statement 

            var weatherRecords = await _context.Weather
                .Where(w => w.event_id == eventId)
                .OrderBy(w => w.forecast_date)
                .ToListAsync();

            if (weatherRecords == null || weatherRecords.Count == 0)
            {//start of if 
                return NotFound(new { message = $"No weather forecast set for event {eventId}." });
            }//end of if 

            // Return the most recent forecast (latest forecast_date)
            var latest = weatherRecords.Last();

            return Ok(new
            {//start of return 
                forecastDate = latest.forecast_date.ToString("yyyy-MM-dd"),
                temperature = latest.temperature,
                rain_chance = latest.rain_chance,
                wind_speed = latest.wind_speed
            });//end of return 
        }//end of GET method 

        [HttpPost]
        public async Task<ActionResult> UpsertWeather(int eventId, [FromBody] Weather weather)
        {//start of POST method 
            
            int currentUserId = 1;

            // 1. Verify event exists
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {//start of if 
                return NotFound(new { message = $"Event with ID {eventId} not found." });
            }//end of if 

            // 2. Ownership check
            if (eventItem.user_id != currentUserId)
            {//start of if 
                return Forbid();
            }//end of if 

            // 3. Force event_id and updated_at
            weather.event_id = eventId;
            weather.updated_at = DateTime.Now;

            // Default the forecast date to the event's date if not supplied
            if (weather.forecast_date == default(DateTime))
            {//start of if 
                weather.forecast_date = eventItem.event_date;
            }//end of if 

            _context.Weather.Add(weather);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {//start of return 
                message = "Weather updated",
                weather_id = weather.weather_id
            });//end of return 
        }//end of POST method 
    }//end of class
}//end of namespace 
