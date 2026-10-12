using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_Part_2_Restful_API_Development.Data;
using RaceDay_Part_2_Restful_API_Development.Models;

namespace RaceDay_Part_2_Restful_API_Development.Controllers
{//start of namespace 
    [Route("api/[controller]")]
    [ApiController]
    public class resultsController : ControllerBase
    {//start of class 

        private readonly RaceDayDatabase _context;

        public resultsController(RaceDayDatabase context)
        {//start of constructor, this initializes the database context context through a dependency injection
            _context = context;
        }//end of constructor 
        

        [HttpPost("api/events/{eventId}/results")]
        public async Task<ActionResult> UploadResults(int eventId, [FromBody] BulkResultsRequest request)
        {//start of POST method 
            // TODO: Replace with session user ID once auth is set up
            int currentUserId = 1;

            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {//start of if statement 
                return NotFound(new { message = $"Event with ID {eventId} not found." });
            }//end of if statement 

            // Ownership check
            if (eventItem.user_id != currentUserId)
            {//start of if statement 
                return Forbid();
            }//end of if statement 

            if (request?.Results == null || request.Results.Count == 0)
            {//start of if statement 
                return BadRequest(new { message = "No results provided." });
            }//end of if statement

            int savedCount = 0;

            foreach (var incoming in request.Results)
            {//start of foreach statement 
                // 1. Make sure the registration exists and belongs to this event
                var registration = await _context.Registration
                    .FirstOrDefaultAsync(r => r.registration_id == incoming.registration_id);

                if (registration == null)
                {//start of if statment 
                    continue; // skip invalid entries
                }//end of if statement 

                // Verify the registration is for this event
                var eventCategory = await _context.EventCategories
                    .FirstOrDefaultAsync(ec => ec.event_category_id == registration.event_category_id);

                if (eventCategory == null || eventCategory.event_id != eventId)
                {//start of if statment 
                    continue; // registration doesn't belong to this event
                }//end of if statement 

                // 2. Upsert: if a result already exists, update it; otherwise insert
                var existingResult = await _context.Results
                    .FirstOrDefaultAsync(res => res.registration_id == incoming.registration_id);

                if (existingResult != null)
                {//start of if statement 
                  
                    existingResult.status = incoming.status ?? "OK";
                    existingResult.updated_at = DateTime.Now;
                }//end of if statement 
                else
                {//start of else statement 
                    var newResult = new Models.Results
                    {
                        registration_id = incoming.registration_id,
                       
                        overall_rank = incoming.overall_rank,
                        category_rank = incoming.category_rank,
                        status = incoming.status ?? "OK",
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    };

                    _context.Results.Add(newResult);
                }//end of else statement 

                savedCount++;
            }//end of foreach 

            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {//start of return 
                message = "Results uploaded successfully.",
                count = savedCount
            });//end of return 
        }//end of POST method 
        [HttpGet("api/events/{eventId}/results")]
        public async Task<ActionResult> GetEventResults(
            int eventId,
            [FromQuery] int? category,
            [FromQuery] int? top)
        {//start of GET method 
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {//start of if 
                return NotFound(new { message = $"Event with ID {eventId} not found." });
            }//end of if 

            // Build the base query: Results → Registration → EventCategories → filter by event
            var query =
                from res in _context.Results
                join reg in _context.Registration on res.registration_id equals reg.registration_id
                join ec in _context.EventCategories on reg.event_category_id equals ec.event_category_id
                join u in _context.Users on reg.user_id equals u.user_id
                join c in _context.Categories on ec.category_id equals c.category_id
                where ec.event_id == eventId
                select new
                {
                    res.result_id,
                    name = u.first_name + " " + u.last_name,
                    category_id = c.category_id,
                    category = c.category_name,
                    finish_time = res.finish_time,
                    overall_rank = res.overall_rank,
                    category_rank = res.category_rank,
                    status = res.status
                };

            // Optional filter by category
            if (category.HasValue)
            {//start of if statement 
                query = query.Where(x => x.category_id == category.Value);
            }//end of if 

            // Order by overall rank (nulls last)
            var results = await query
                .OrderBy(x => x.overall_rank == null)
                .ThenBy(x => x.overall_rank)
                .ToListAsync();

            // Optional top-N
            if (top.HasValue)
            {//start of if 
                results = results.Take(top.Value).ToList();
            }//end of if  

            return Ok(results);
        }//end of GET method 
        [HttpGet("api/registrations/{registrationId}/results")]
        public async Task<ActionResult> GetResultByRegistration(int registrationId)
        {//start of GET method 
            //Replace with session user ID once auth is set up
            int currentUserId = 1;

            var registration = await _context.Registration.FindAsync(registrationId);
            if (registration == null)
            {//start of if 
                return NotFound(new { message = $"Registration with ID {registrationId} not found." });
            }//end of if 

            // Ownership check
            var eventCategory = await _context.EventCategories
                .FirstOrDefaultAsync(ec => ec.event_category_id == registration.event_category_id);
            var eventItem = eventCategory != null
                ? await _context.Events.FirstOrDefaultAsync(e => e.event_id == eventCategory.event_id)
                : null;

            bool isOwner = registration.user_id == currentUserId;
            bool isOrganiser = eventItem != null && eventItem.user_id == currentUserId;

            if (!isOwner && !isOrganiser)
            {//start of if 
                return Forbid();
            }//end of if 

            var result = await _context.Results
                .FirstOrDefaultAsync(res => res.registration_id == registrationId);

            if (result == null)
            {//start of if 
                return NotFound(new { message = "No result found for this registration." });
            }//end of if 

            return Ok(new
            {//start of return 
                result_id = result.result_id,
                finish_time = result.finish_time,
                overall_rank = result.overall_rank,
                category_rank = result.category_rank,
                status = result.status
            });//end of return
        }//end of GET method 
        [HttpPut("api/results/{resultId}")]
        public async Task<IActionResult> UpdateResult(int resultId, [FromBody] Models.Results updatedResult)
        {//start of PUT method 
            //Replace with session user ID once auth is set up
            int currentUserId = 1;

            var existingResult = await _context.Results.FindAsync(resultId);
            if (existingResult == null)
            {//start of if 
                return NotFound(new { message = $"Result with ID {resultId} not found." });
            }//end of if 

            // Ownership check via registration → event
            var registration = await _context.Registration
                .FirstOrDefaultAsync(r => r.registration_id == existingResult.registration_id);
            var eventCategory = registration != null
                ? await _context.EventCategories.FirstOrDefaultAsync(ec => ec.event_category_id == registration.event_category_id)
                : null;
            var eventItem = eventCategory != null
                ? await _context.Events.FirstOrDefaultAsync(e => e.event_id == eventCategory.event_id)
                : null;

            if (eventItem == null || eventItem.user_id != currentUserId)
            {//start of if 
                return Forbid();
            }//end of if 

            // Only update allowed fields
            existingResult.finish_time = updatedResult.finish_time;
            existingResult.status = updatedResult.status ?? existingResult.status;
            existingResult.updated_at = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Result updated successfully." });
        }//end of PUT 
    }//end of class 
    public class BulkResultsRequest
    {//start of class BulkResultsRequest 
        public List<BulkResultItem> Results { get; set; } = new();
    }//end of class BulkResultsRequest 

    public class BulkResultItem
    {//start of class BulkResultItem
        public int registration_id { get; set; }
        public TimeSpan? finish_time { get; set; }
        public int? overall_rank { get; set; }
        public int? category_rank { get; set; }
        public string? status { get; set; }
    }//end of class BulkResultItem
}//end of class 
