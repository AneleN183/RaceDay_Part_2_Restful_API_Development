using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_Part_2_Restful_API_Development.Models;
using RaceDay_Part_2_Restful_API_Development.Data; 

namespace RaceDay_Part_2_Restful_API_Development.Controllers
{//start of namespace 
    [Route("api/[controller]")]
    [ApiController]
    public class usersController : ControllerBase
    {//start of class 

        private readonly RaceDayDatabase _context;

        public usersController(RaceDayDatabase context)
        {
            _context = context;
        }
        
        [HttpGet("me")]
        public async Task<ActionResult<Users>> GetMyProfile()
        {
            // TODO: Replace with actual session user ID once auth is set up
            int currentUserId = 1;

            var user = await _context.Users.FindAsync(currentUserId);

            if (user == null)
            {
                return NotFound(new { message = "Current user not found." });
            }

            user.user_password = "";
            return Ok(user);
        }
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] Users updatedUser)
        {
            // TODO: Replace with actual session user ID once auth is set up
            int currentUserId = 1;

            var user = await _context.Users.FindAsync(currentUserId);

            if (user == null)
            {
                return NotFound(new { message = "Current user not found." });
            }

            user.first_name = updatedUser.first_name;
            user.last_name = updatedUser.last_name;
            user.email = updatedUser.email;

            if (!string.IsNullOrEmpty(updatedUser.user_password))
            {
                // NOTE: Hash this once auth is implemented
                user.user_password = updatedUser.user_password;
            }

            user.updated_at = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile updated successfully.", user });
        }
        [HttpGet("me/registrations")]
        public async Task<ActionResult> GetMyRegistrations()
        {
            // TODO: Replace with actual session user ID once auth is set up
            int currentUserId = 1;

            var registrations = await _context.Registration
                .Where(r => r.user_id == currentUserId)
                .Select(r => new
                {
                    registration_id = r.registration_id,
                    registration_date = r.registration_date,
                    registration_status = r.registration_status,
                    event_category_id = r.event_category_id
                })
                .ToListAsync();

            return Ok(registrations);
        }
        [HttpGet("me/results")]
        public async Task<ActionResult> GetMyResults()
        {
            // TODO: Replace with actual session user ID once auth is set up
            int currentUserId = 1;

            var results = await _context.Results
                .Join(_context.Registration,
                      res => res.registration_id,
                      reg => reg.registration_id,
                      (res, reg) => new { res, reg })
                .Where(joined => joined.reg.user_id == currentUserId)
                .Select(joined => new
                {
                    result_id = joined.res.result_id,
                    finish_time = joined.res.finish_time,
                    overall_rank = joined.res.overall_rank,
                    category_rank = joined.res.category_rank,
                    status = joined.res.status
                })
                .ToListAsync();

            return Ok(results);
        }


    }//end of class
}//end of namespace 
