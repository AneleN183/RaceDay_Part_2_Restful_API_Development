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
        {//start of constructor, this initializes the database context context through a dependency injection
            _context = context;
        }//end of constructor 

        

        [HttpPost]
        public async Task<ActionResult> CreateRegistration([FromBody] Registration newRegistration)
        {//start of POST method 
            // TODO: Replace with session user ID once auth is set up
            int currentUserId = 1;

            // 1. Verify the event-category exists
            var eventCategory = await _context.EventCategories
                .FirstOrDefaultAsync(ec => ec.event_category_id == newRegistration.event_category_id);

            if (eventCategory == null)
            {//start of if statement 
                return NotFound(new
                {//return statement 
                    message = $"Event category with ID {newRegistration.event_category_id} not found."
                });//this is what will be returned if event with category is not found 
            }//end of if statement 

            // 2. Prevent duplicate registrations by the same user for the same event-category
            var alreadyRegistered = await _context.Registration
                .AnyAsync(r =>
                    r.user_id == currentUserId &&
                    r.event_category_id == newRegistration.event_category_id &&
                    r.registration_status != "cancelled" &&
                    r.registration_status != "withdrawn");

            if (alreadyRegistered)
            {//start of if statement 
                return Conflict(new { message = "You are already registered for this event category." });
            }//end of if statment
            //if for example you forgot that you registered an event category and you enter for it again, this is the message it will send you 

            // 3. Force user_id to be the current user (never trust the body)
            newRegistration.user_id = currentUserId;
            newRegistration.registration_date = DateTime.Now;
            newRegistration.registration_status = "pending"; // or "confirmed" if you want instant confirmation
            newRegistration.created_at = DateTime.Now;
            newRegistration.updated_at = DateTime.Now;

            _context.Registration.Add(newRegistration);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {//this is what will be returned if your registration was successful 
                message = "Registration successful.",
                registration_id = newRegistration.registration_id,
                registration_status = newRegistration.registration_status
            });//end of return method 
        }//end of POST method 

        [HttpGet("{id}")]
        public async Task<ActionResult> GetRegistrationById(int id)
        {//start of GET method 
            //Replace with session user ID once auth is set up
            int currentUserId = 1;

            var registration = await _context.Registration.FindAsync(id);

            if (registration == null)
            {//start of if statement
                return NotFound(new { message = $"Registration with ID {id} not found." });
            }//end of if statement

            // Ownership check: only the participant or the event's organiser can view
            var eventCategory = await _context.EventCategories
                .FirstOrDefaultAsync(ec => ec.event_category_id == registration.event_category_id);

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.event_id == eventCategory!.event_id);

            bool isOwner = registration.user_id == currentUserId;
            bool isOrganiser = eventItem != null && eventItem.user_id == currentUserId;

            if (!isOwner && !isOrganiser)
            {//start of if 
                return Forbid();
            }//end of if 

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
        }//end of GET method 

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRegistrationStatus(int id, [FromBody] Registration updatedRegistration)
        {//start of PUT method 
            // TODO: Replace with session user ID once auth is set up
            int currentUserId = 1;

            var existing = await _context.Registration.FindAsync(id);

            if (existing == null)
            {//start of if statement 
                return NotFound(new { message = $"Registration with ID {id} not found." });
            }//end of if statement 

            // Only the owner can change their own registration
            if (existing.user_id != currentUserId)
            {//start of if statement 
                return Forbid();
            }//end of if statement

            // Validate the new status against allowed values
            var allowedStatuses = new[] { "pending", "confirmed", "cancelled", "withdrawn" };
            if (!allowedStatuses.Contains(updatedRegistration.registration_status))
            {//start of if statement 
                return BadRequest(new
                {//start of return method 
                    message = $"Invalid status. Allowed values: {string.Join(", ", allowedStatuses)}"
                });//end of return method 
            }//end of if statement 

            existing.registration_status = updatedRegistration.registration_status;
            existing.updated_at = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new
            {//start of return method 
                message = "Registration updated.",
                registration = new
                {
                    registration_id = existing.registration_id,
                    registration_status = existing.registration_status,
                    updated_at = existing.updated_at
                }
            });//end of return method 
        }//end of PUT method 
        [HttpDelete("{id}")]
        public async Task<IActionResult> WithdrawRegistration(int id)
        {//start of PUT method 
            //Replace with session user ID once auth is set up
            int currentUserId = 1;

            var existing = await _context.Registration.FindAsync(id);

            if (existing == null)
            {//start of if statement 
                return NotFound(new { message = $"Registration with ID {id} not found." });
            }//end of if statement 

            // Only the owner can withdraw
            if (existing.user_id != currentUserId)
            {//start of if statement
                return Forbid();
            }//end of if statement 

            _context.Registration.Remove(existing);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registration withdrawn successfully." });
        }//end of DELETE method 

    }//end of class
}//end of namespace 
