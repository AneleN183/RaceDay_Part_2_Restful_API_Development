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
        {
            _context = context;
        }
        

        [HttpPost("api/events/{eventId}/results")]
        public async Task<ActionResult> UploadResults(int eventId, [FromBody] BulkResultsRequest request)
        {
            // TODO: Replace with session user ID once auth is set up
            int currentUserId = 1;

            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {
                return NotFound(new { message = $"Event with ID {eventId} not found." });
            }

            // Ownership check
            if (eventItem.user_id != currentUserId)
            {
                return Forbid();
            }

            if (request?.Results == null || request.Results.Count == 0)
            {
                return BadRequest(new { message = "No results provided." });
            }

            int savedCount = 0;

            foreach (var incoming in request.Results)
            {
                // 1. Make sure the registration exists and belongs to this event
                var registration = await _context.Registration
                    .FirstOrDefaultAsync(r => r.registration_id == incoming.registration_id);

                if (registration == null)
                {
                    continue; // skip invalid entries
                }

                // Verify the registration is for this event
                var eventCategory = await _context.EventCategories
                    .FirstOrDefaultAsync(ec => ec.event_category_id == registration.event_category_id);

                if (eventCategory == null || eventCategory.event_id != eventId)
                {
                    continue; // registration doesn't belong to this event
                }

                // 2. Upsert: if a result already exists, update it; otherwise insert
                var existingResult = await _context.Results
                    .FirstOrDefaultAsync(res => res.registration_id == incoming.registration_id);

                if (existingResult != null)
                {
                  
                    existingResult.status = incoming.status ?? "OK";
                    existingResult.updated_at = DateTime.Now;
                }
                else
                {
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
                }

                savedCount++;
            }

            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                message = "Results uploaded successfully.",
                count = savedCount
            });
        }
        [HttpGet("api/events/{eventId}/results")]
        public async Task<ActionResult> GetEventResults(
            int eventId,
            [FromQuery] int? category,
            [FromQuery] int? top)
        {
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {
                return NotFound(new { message = $"Event with ID {eventId} not found." });
            }

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
            {
                query = query.Where(x => x.category_id == category.Value);
            }

            // Order by overall rank (nulls last)
            var results = await query
                .OrderBy(x => x.overall_rank == null)
                .ThenBy(x => x.overall_rank)
                .ToListAsync();

            // Optional top-N
            if (top.HasValue)
            {
                results = results.Take(top.Value).ToList();
            }

            return Ok(results);
        }
        [HttpGet("api/registrations/{registrationId}/results")]
        public async Task<ActionResult> GetResultByRegistration(int registrationId)
        {
            // TODO: Replace with session user ID once auth is set up
            int currentUserId = 1;

            var registration = await _context.Registration.FindAsync(registrationId);
            if (registration == null)
            {
                return NotFound(new { message = $"Registration with ID {registrationId} not found." });
            }

            // Ownership check
            var eventCategory = await _context.EventCategories
                .FirstOrDefaultAsync(ec => ec.event_category_id == registration.event_category_id);
            var eventItem = eventCategory != null
                ? await _context.Events.FirstOrDefaultAsync(e => e.event_id == eventCategory.event_id)
                : null;

            bool isOwner = registration.user_id == currentUserId;
            bool isOrganiser = eventItem != null && eventItem.user_id == currentUserId;

            if (!isOwner && !isOrganiser)
            {
                return Forbid();
            }

            var result = await _context.Results
                .FirstOrDefaultAsync(res => res.registration_id == registrationId);

            if (result == null)
            {
                return NotFound(new { message = "No result found for this registration." });
            }

            return Ok(new
            {
                result_id = result.result_id,
                finish_time = result.finish_time,
                overall_rank = result.overall_rank,
                category_rank = result.category_rank,
                status = result.status
            });
        }
        [HttpPut("api/results/{resultId}")]
        public async Task<IActionResult> UpdateResult(int resultId, [FromBody] Models.Results updatedResult)
        {
            // TODO: Replace with session user ID once auth is set up
            int currentUserId = 1;

            var existingResult = await _context.Results.FindAsync(resultId);
            if (existingResult == null)
            {
                return NotFound(new { message = $"Result with ID {resultId} not found." });
            }

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
            {
                return Forbid();
            }

            // Only update allowed fields
            existingResult.finish_time = updatedResult.finish_time;
            existingResult.status = updatedResult.status ?? existingResult.status;
            existingResult.updated_at = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Result updated successfully." });
        }
    }
    public class BulkResultsRequest
    {
        public List<BulkResultItem> Results { get; set; } = new();
    }

    public class BulkResultItem
    {
        public int registration_id { get; set; }
        public TimeSpan? finish_time { get; set; }
        public int? overall_rank { get; set; }
        public int? category_rank { get; set; }
        public string? status { get; set; }
    }
}//end of class 
