using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_Part_2_Restful_API_Development.Models
{//start of namespace 

    [Table("Routes")]
    public class Routes
    {//start of class 

        [Key]
        [Column("route_id")]
        public int route_id { get; set; }

        [Required]
        [Column("event_id")]
        public int event_id { get; set; }            // Here lies the foreign key to Events, with a one-to-one relationship 

        [Column("route_description")]
        public string? route_description { get; set; }

        [Required]
        [Column("distance_km")]
        public decimal distance_km { get; set; }

        [Column("elevation_m")]
        public int? elevation_m { get; set; }

    }//end of class
}//end of namespace 
