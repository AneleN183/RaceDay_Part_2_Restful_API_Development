using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_Part_2_Restful_API_Development.Data;
using RaceDay_Part_2_Restful_API_Development.Models;

namespace RaceDay_Part_2_Restful_API_Development.Controllers
{//start of namespace 
    [Route("api/[controller]")]
    [ApiController]
    public class routesController : ControllerBase
    {//start of class

        private readonly RaceDayDatabase _context;

        public routesController(RaceDayDatabase context)
        {
            _context = context;
        }
        

        [HttpGet]
        public async Task<ActionResult> GetRoute(int eventId)
        {
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {
                return NotFound(new { message = $"Event with ID {eventId} not found." });
            }

            var route = await _context.Routes
                .FirstOrDefaultAsync(r => r.event_id == eventId);

            if (route == null)
            {
                return NotFound(new { message = $"No route has been set for event {eventId}." });
            }

            return Ok(new
            {
                id = route.route_id,
                event_description = route.route_description,
                distance_m = route.distance_km * 1000,   // convert km → m to match your expected response
                distance_km = route.distance_km,         // keep km too, in case you want it
                elevation_m = route.elevation_m
            });
        }

        [HttpPost]
        public async Task<ActionResult> UpsertRoute(int eventId, [FromBody] Routes route)
        {
            // TODO: Replace with session user ID once auth is set up
            int currentUserId = 1;

            // 1. Verify the event exists
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {
                return NotFound(new { message = $"Event with ID {eventId} not found." });
            }

            // 2. Ownership check — only the organiser who created the event
            if (eventItem.user_id != currentUserId)
            {
                return Forbid();
            }

            // 3. Upsert
            var existingRoute = await _context.Routes
                .FirstOrDefaultAsync(r => r.event_id == eventId);

            if (existingRoute != null)
            {
                // Update
                existingRoute.route_description = route.route_description;
                existingRoute.distance_km = route.distance_km;
                existingRoute.elevation_m = route.elevation_m;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Route updated successfully.",
                    route_id = existingRoute.route_id
                });
            }
            else
            {
                // Create
                route.event_id = eventId;

                _context.Routes.Add(route);
                await _context.SaveChangesAsync();

                return StatusCode(201, new
                {
                    message = "Route updated successfully.",
                    route_id = route.route_id
                });
            }
        }

    }//end of class


}//end of namespace 
