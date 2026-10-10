using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_Part_2_Restful_API_Development.Data;
using RaceDay_Part_2_Restful_API_Development.Models;

namespace RaceDay_Part_2_Restful_API_Development.Controllers
{//start of namespace 
    [Route("api/[controller]")]
    [ApiController]
    public class eventsController : ControllerBase
    {//start of class

        private readonly RaceDayDatabase _context;

        public eventsController(RaceDayDatabase context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Events>>> GetAllEvents(
           [FromQuery] string? location)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(e => e.event_location.Contains(location));
            }

            var events = await query.ToListAsync();
            return Ok(events);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetEventById(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
            {
                return NotFound(new { message = $"Event with ID {id} not found." });
            }

            // Fetch related data
            var categories = await _context.EventCategories
                .Where(ec => ec.event_id == id)
                .Join(_context.Categories,
                      ec => ec.category_id,
                      c => c.category_id,
                      (ec, c) => new
                      {
                          event_category_id = ec.event_category_id,
                          category_id = c.category_id,
                          name = c.category_name,
                          category_type = c.category_type,
                          start_time = ec.start_time,
                          entry_fee = ec.entry_fee
                      })
                .ToListAsync();

            var route = await _context.Routes
                .FirstOrDefaultAsync(r => r.event_id == id);

            var weather = await _context.Weather
                .Where(w => w.event_id == id)
                .ToListAsync();

            // Build a combined response
            var result = new
            {
                event_id = eventItem.event_id,
                user_id = eventItem.user_id,
                event_title = eventItem.event_title,
                description = eventItem.event_description,
                event_date = eventItem.event_date,
                event_location = eventItem.event_location,
                categories,
                route,
                weather
            };

            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult<Events>> CreateEvent([FromBody] Events newEvent)
        {
            // TODO: Replace with session user ID once auth is set up
            int currentUserId = 1;

            // TODO: Once auth exists, verify current user is an organiser

            newEvent.user_id = currentUserId;
            newEvent.created_at = DateTime.Now;
            newEvent.updated_at = DateTime.Now;


            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEventById),
                new { id = newEvent.event_id },
                newEvent
            );
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] Events updatedEvent)
        {
            // TODO: Replace with session user ID once auth is set up
            int currentUserId = 1;

            var existingEvent = await _context.Events.FindAsync(id);

            if (existingEvent == null)
            {
                return NotFound(new { message = $"Event with ID {id} not found." });
            }

            // Ownership check
            if (existingEvent.user_id != currentUserId)
            {
                return Forbid(); // 403 - you don't own this event
            }

            // Update only allowed fields
            existingEvent.event_title = updatedEvent.event_title;
            existingEvent.event_description = updatedEvent.event_description;
            existingEvent.event_date = updatedEvent.event_date;
            existingEvent.event_location = updatedEvent.event_location;
            existingEvent.updated_at = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Event updated successfully.", existingEvent });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            // TODO: Replace with session user ID once auth is set up
            int currentUserId = 1;

            var existingEvent = await _context.Events.FindAsync(id);

            if (existingEvent == null)
            {
                return NotFound(new { message = $"Event with ID {id} not found." });
            }

            // Ownership check
            if (existingEvent.user_id != currentUserId)
            {
                return Forbid();
            }

            _context.Events.Remove(existingEvent);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Event {id} deleted successfully." });
        }
        [HttpGet("{id}/registrations")]
        public async Task<ActionResult> GetEventRegistrations(int id)
        {
            // TODO: Replace with session user ID once auth is set up
            int currentUserId = 1;

            var existingEvent = await _context.Events.FindAsync(id);

            if (existingEvent == null)
            {
                return NotFound(new { message = $"Event with ID {id} not found." });
            }

            // Ownership check
            if (existingEvent.user_id != currentUserId)
            {
                return Forbid();
            }

            // Get all registrations for any category under this event
            var registrations = await _context.Registration
                .Join(_context.EventCategories,
                      r => r.event_category_id,
                      ec => ec.event_category_id,
                      (r, ec) => new { r, ec })
                .Where(joined => joined.ec.event_id == id)
                .Join(_context.Users,
                      joined => joined.r.user_id,
                      u => u.user_id,
                      (joined, u) => new
                      {
                          registration_id = joined.r.registration_id,
                          user_id = u.user_id,
                          participant_name = u.first_name + " " + u.last_name,
                          email = u.email,
                          registration_date = joined.r.registration_date,
                          registration_status = joined.r.registration_status,
                          event_category_id = joined.r.event_category_id
                      })
                .ToListAsync();

            return Ok(registrations);
        }

    }//end of class
}//end of namespace 
