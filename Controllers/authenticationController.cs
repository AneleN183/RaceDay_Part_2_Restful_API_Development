using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_Part_2_Restful_API_Development.Data;
using RaceDay_Part_2_Restful_API_Development.Models;

namespace RaceDay_Part_2_Restful_API_Development.Controllers
{//start of namespace 
    [Route("api/[controller]")]
    [ApiController]
    public class authenticationController : ControllerBase
    {//start class
        private readonly RaceDayDatabase _context;

        public authenticationController(RaceDayDatabase context)
        {//start of constructor, this initializes the database context context through a dependency injection
            _context = context;
        }//end of constructor 

        
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {//Here we have the register method - this handles new user registration

            // 1. Here is where we validate input
            if (string.IsNullOrWhiteSpace(request.email) ||
                string.IsNullOrWhiteSpace(request.password) ||
                string.IsNullOrWhiteSpace(request.first_name) ||
                string.IsNullOrWhiteSpace(request.last_name) ||
                string.IsNullOrWhiteSpace(request.user_role))
            {
                return BadRequest(new { message = "All fields are required." });
            }

            // 2. Validate role
            if (request.user_role != "organiser" && request.user_role != "participant")
            {//start of if, you have to choose whether you are an organiser or participant, this is very important 
                return BadRequest(new { message = "Role must be 'organiser' or 'participant'." });
            }//end of if 

            // 3. Check for duplicate email
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.email == request.email);

            if (existingUser != null)
            {//start of if, remember we said email must be unique? So, an email is only used for one account
                return Conflict(new { message = "A user with this email already exists." });
            }//end of if

            // 4. Hash the password so I won't be able to see what user's password is 
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.password);

            // 5. Create the user
            var newUser = new Users
            {//this is what will be asked of the user...
                first_name = request.first_name,
                last_name = request.last_name,
                email = request.email,
                user_password = hashedPassword,
                user_role = request.user_role,
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };//...it will also be recorded when they will fill this out

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                message = "User registered successfully.",
                user_id = newUser.user_id
            });
        }

        
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {//start of POST method for user to login 
            if (string.IsNullOrWhiteSpace(request.email) ||
                string.IsNullOrWhiteSpace(request.password))
            {
                return BadRequest(new { message = "Email and password are required." });
            }

            // 1. Find the user
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.email == request.email);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // 2. Verify password against stored hash
            bool passwordValid = BCrypt.Net.BCrypt.Verify(request.password, user.user_password);

            if (!passwordValid)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // 3. Create session
            HttpContext.Session.SetInt32("UserId", user.user_id);
            HttpContext.Session.SetString("UserRole", user.user_role);
            HttpContext.Session.SetString("UserName", $"{user.first_name} {user.last_name}");

            return Ok(new
            {
                message = "Login successful.",
                user = new
                {
                    user_id = user.user_id,
                    first_name = user.first_name,
                    last_name = user.last_name,
                    email = user.email,
                    user_role = user.user_role
                }
            });
        }//end of POST methodf for user to login 

        [HttpPost("logout")]
        public IActionResult Logout()
        {//start of POST method, for user to be able to logout
            HttpContext.Session.Clear();
            return Ok(new { message = "Logged out successfully." });
        }//end of POST method for user to be able to logout

        
        [HttpGet("me")]
        public async Task<ActionResult> GetCurrentUser()
        {//start of GET method
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized(new { message = "Not logged in." });
            }

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return Unauthorized(new { message = "Session user no longer exists." });
            }

            return Ok(new
            {
                user_id = user.user_id,
                first_name = user.first_name,
                last_name = user.last_name,
                email = user.email,
                user_role = user.user_role
            });
        }//end of GET method 
    }//end of class

    
    public class RegisterRequest
    {//start of register request class 
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string user_role { get; set; } = string.Empty;
    }//end of register request class

    public class LoginRequest
    {//start of class
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
    }//end of class
}//end of namespace 