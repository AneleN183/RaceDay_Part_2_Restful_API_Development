using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_Part_2_Restful_API_Development.Data;
using RaceDay_Part_2_Restful_API_Development.Models; 

namespace RaceDay_Part_2_Restful_API_Development.Controllers
{//start of namespace 
    [Route("api/[controller]")]
    [ApiController]
    public class categoriesController : ControllerBase
    {//start of class

        private readonly RaceDayDatabase _context;

        public categoriesController(RaceDayDatabase context)
        {
            _context = context;
        }

       

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categories>>> GetAllCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Categories>> GetCategoryById(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(new { message = $"Category with ID {id} not found." });
            }

            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<Categories>> CreateCategory([FromBody] Categories newCategory)
        {
            // TODO: Once auth exists, verify user is an organiser

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategoryById),
                new { id = newCategory.category_id },
                newCategory
            );
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Categories updatedCategory)
        {
            // TODO: Once auth exists, verify user is an organiser

            var existing = await _context.Categories.FindAsync(id);

            if (existing == null)
            {
                return NotFound(new { message = $"Category with ID {id} not found." });
            }

            existing.category_name = updatedCategory.category_name;
            existing.category_description = updatedCategory.category_description;
            existing.category_type = updatedCategory.category_type;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Category updated successfully.", existing });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            // TODO: Once auth exists, verify user is an organiser

            var existing = await _context.Categories.FindAsync(id);

            if (existing == null)
            {
                return NotFound(new { message = $"Category with ID {id} not found." });
            }

            _context.Categories.Remove(existing);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Category {id} deleted successfully." });
        }


    }//end of class
}//end of namespace 
