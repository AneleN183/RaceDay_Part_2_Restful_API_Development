using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_Part_2_Restful_API_Development.Data;
using RaceDay_Part_2_Restful_API_Development.Models;

namespace RaceDay_Part_2_Restful_API_Development.Controllers
{//start of namespace 
    [Route("api/[controller]")]
    [ApiController]
    public class registrationsController : ControllerBase
    {//start of class

        private readonly RaceDayDatabase _context;

        public registrationsController(RaceDayDatabase context)
        {
            _context = context;
        }

        

        [HttpPost]
        public async Task<ActionResult> CreateRegistration([FromBody] Registration newRegistration)
        {
            // TODO: Replace with session user ID once auth is set up
            int currentUserId = 1;

            // 1. Verify the event-category exists
            var eventCategory = await _context.EventCategories
                .FirstOrDefaultAsync(ec => ec.event_category_id == newRegistration.event_category_id);

            if (eventCategory == null)
            {
                return NotFound(new
                {
                    message = $"Event category with ID {newRegistration.event_category_id} not found."
                });
            }

            // 2. Prevent duplicate registrations by the same user for the same event-category
            var alreadyRegistered = await _context.Registration
                .AnyAsync(r =>
                    r.user_id == currentUserId &&
                    r.event_category_id == newRegistration.event_category_id &&
                    r.registration_status != "cancelled" &&
                    r.registration_status != "withdrawn");

            if (alreadyRegistered)
            {
                return Conflict(new { message = "You are already registered for this event category." });
            }

            // 3. Force user_id to be the current user (never trust the body)
            newRegistration.user_id = currentUserId;
            newRegistration.registration_date = DateTime.Now;
            newRegistration.registration_status = "pending"; // or "confirmed" if you want instant confirmation
            newRegistration.created_at = DateTime.Now;
            newRegistration.updated_at = DateTime.Now;

            _context.Registration.Add(newRegistration);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                message = "Registration successful.",
                registration_id = newRegistration.registration_id,
                registration_status = newRegistration.registration_status
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetRegistrationById(int id)
        {
            // TODO: Replace with session user ID once auth is set up
            int currentUserId = 1;

            var registration = await _context.Registration.FindAsync(id);

            if (registration == null)
            {
                return NotFound(new { message = $"Registration with ID {id} not found." });
            }

            // Ownership check: only the participant or the event's organiser can view
            var eventCategory = await _context.EventCategories
                .FirstOrDefaultAsync(ec => ec.event_category_id == registration.event_category_id);

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.event_id == eventCategory!.event_id);

            bool isOwner = registration.user_id == currentUserId;
            bool isOrganiser = eventItem != null && eventItem.user_id == currentUserId;

            if (!isOwner && !isOrganiser)
            {
                return Forbid();
            }

            // Build a detailed response (matches your expected response in the plan)
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.category_id == eventCategory!.category_id);

            var response = new
            {
                registration_id = registration.registration_id,
                event_title = eventItem?.event_title,
                category = category?.category_name,
                status = registration.registration_status,
                registration_date = registration.registration_date
            };

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRegistrationStatus(int id, [FromBody] Registration updatedRegistration)
        {
            // TODO: Replace with session user ID once auth is set up
            int currentUserId = 1;

            var existing = await _context.Registration.FindAsync(id);

            if (existing == null)
            {
                return NotFound(new { message = $"Registration with ID {id} not found." });
            }

            // Only the owner can change their own registration
            if (existing.user_id != currentUserId)
            {
                return Forbid();
            }

            // Validate the new status against allowed values
            var allowedStatuses = new[] { "pending", "confirmed", "cancelled", "withdrawn" };
            if (!allowedStatuses.Contains(updatedRegistration.registration_status))
            {
                return BadRequest(new
                {
                    message = $"Invalid status. Allowed values: {string.Join(", ", allowedStatuses)}"
                });
            }

            existing.registration_status = updatedRegistration.registration_status;
            existing.updated_at = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Registration updated.",
                registration = new
                {
                    registration_id = existing.registration_id,
                    registration_status = existing.registration_status,
                    updated_at = existing.updated_at
                }
            });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> WithdrawRegistration(int id)
        {
            // TODO: Replace with session user ID once auth is set up
            int currentUserId = 1;

            var existing = await _context.Registration.FindAsync(id);

            if (existing == null)
            {
                return NotFound(new { message = $"Registration with ID {id} not found." });
            }

            // Only the owner can withdraw
            if (existing.user_id != currentUserId)
            {
                return Forbid();
            }

            _context.Registration.Remove(existing);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registration withdrawn successfully." });
        }

    }//end of class
}//end of namespace 
