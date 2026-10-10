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
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult> GetWeather(int eventId)
        {
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {
                return NotFound(new { message = $"Event with ID {eventId} not found." });
            }

            var weatherRecords = await _context.Weather
                .Where(w => w.event_id == eventId)
                .OrderBy(w => w.forecast_date)
                .ToListAsync();

            if (weatherRecords == null || weatherRecords.Count == 0)
            {
                return NotFound(new { message = $"No weather forecast set for event {eventId}." });
            }

            // Return the most recent forecast (latest forecast_date)
            var latest = weatherRecords.Last();

            return Ok(new
            {
                forecastDate = latest.forecast_date.ToString("yyyy-MM-dd"),
                temperature = latest.temperature,
                rain_chance = latest.rain_chance,
                wind_speed = latest.wind_speed
            });
        }

        [HttpPost]
        public async Task<ActionResult> UpsertWeather(int eventId, [FromBody] Weather weather)
        {
            // TODO: Replace with session user ID once auth is set up
            int currentUserId = 1;

            // 1. Verify event exists
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {
                return NotFound(new { message = $"Event with ID {eventId} not found." });
            }

            // 2. Ownership check
            if (eventItem.user_id != currentUserId)
            {
                return Forbid();
            }

            // 3. Force event_id and updated_at
            weather.event_id = eventId;
            weather.updated_at = DateTime.Now;

            // Default the forecast date to the event's date if not supplied
            if (weather.forecast_date == default(DateTime))
            {
                weather.forecast_date = eventItem.event_date;
            }

            _context.Weather.Add(weather);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                message = "Weather updated",
                weather_id = weather.weather_id
            });
        }
    }//end of class
}//end of namespace 
