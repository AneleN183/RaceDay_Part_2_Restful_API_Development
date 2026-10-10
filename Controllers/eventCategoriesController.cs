using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay_Part_2_Restful_API_Development.Data;
using RaceDay_Part_2_Restful_API_Development.Models;

namespace RaceDay_Part_2_Restful_API_Development.Controllers
{//start of namespace 
    [Route("api/[controller]")]
    [ApiController]
    public class eventCategoriesController : ControllerBase
    {//start of class

        private readonly RaceDayDatabase _context;

        public eventCategoriesController(RaceDayDatabase context)
        {
            _context = context;
        }

       

    }//end of class
}//end of namespace 
