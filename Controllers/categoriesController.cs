using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_Part_2_Restful_API_Development.Data;
using RaceDay_Part_2_Restful_API_Development.Models; 
//imports 

namespace RaceDay_Part_2_Restful_API_Development.Controllers
{//start of namespace 
    [Route("api/[controller]")]
    [ApiController]
    public class categoriesController : ControllerBase
    {//start of class

        private readonly RaceDayDatabase _context;

        public categoriesController(RaceDayDatabase context)
        {//start of constructor, this initializes the database context context through a dependency injection
            _context = context;
        }//end of constructor 

       

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categories>>> GetAllCategories()
        {//start of GET method
         //this emthod retrieves all categories from the database 
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }//end of GET method
         //it returns a 200 status code with the list. 

        [HttpGet("{id}")]
        public async Task<ActionResult<Categories>> GetCategoryById(int id)
        {//start of GET method
            //it retrives a single category by its ID 
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {//start of if statement
                //it returns a 404 status code if the category does not match the ID
                return NotFound(new { message = $"Category with ID {id} not found." });
            }//end of if statement 

            return Ok(category);
        }//end of GET method 

        [HttpPost]
        public async Task<ActionResult<Categories>> CreateCategory([FromBody] Categories newCategory)
        {//start of POST method
            // TODO: Once auth exists, verify user is an organiser

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategoryById),
                new { id = newCategory.category_id },
                newCategory
            );
        }//end of POST method 


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Categories updatedCategory)
        {//end of PUT method 
            //once auth exists, verify user is an organiser

            var existing = await _context.Categories.FindAsync(id);

            if (existing == null)
            {//start of if statement
                return NotFound(new { message = $"Category with ID {id} not found." });
            }//end of if statement

            existing.category_name = updatedCategory.category_name;
            existing.category_description = updatedCategory.category_description;
            existing.category_type = updatedCategory.category_type;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Category updated successfully.", existing });
        }//end of PUT method 

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {//start of DELETE method 
            // TODO: Once auth exists, verify user is an organiser

            var existing = await _context.Categories.FindAsync(id);

            if (existing == null)
            {//start of if statement 
                return NotFound(new { message = $"Category with ID {id} not found." });
            }//end of if statement 

            _context.Categories.Remove(existing);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Category {id} deleted successfully." });
        }//end of DELETE statement 


    }//end of class
}//end of namespace 
