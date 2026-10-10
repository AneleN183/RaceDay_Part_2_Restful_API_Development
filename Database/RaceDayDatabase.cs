using Microsoft.EntityFrameworkCore;
using RaceDay_Part_2_Restful_API_Development.Models;

namespace RaceDay_Part_2_Restful_API_Development.Data
{//start of namespace 
    public class RaceDayDatabase :DbContext
    {//start of class
        public RaceDayDatabase(DbContextOptions<RaceDayDatabase> options)
            : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<Routes> Routes { get; set; }
        public DbSet<Weather> Weather { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<EventCategories> EventCategories { get; set; }
        public DbSet<Registration> Registration { get; set; }
        public DbSet<Models.Results> Results { get; set; }


    }//end of class
}//end of namespace 
