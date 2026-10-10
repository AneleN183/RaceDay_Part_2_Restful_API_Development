using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_Part_2_Restful_API_Development.Models
{//start of namespace 

    [Table("Weather")]
    public class Weather
    {//start of class

        [Key]
        [Column("weather_id")]
        public int weather_id { get; set; }

        [Required]
        [Column("event_id")]
        public int event_id { get; set; }            // Here is the foreign key to Events 

        [Required]
        [Column("forecast_date")]
        public DateTime forecast_date { get; set; }

        [Column("temperature")]
        public decimal? temperature { get; set; }

        [Column("rain_chance")]
        public decimal? rain_chance { get; set; }

        [Column("wind_speed")]
        public decimal? wind_speed { get; set; }

        [Column("updated_at")]
        public DateTime? updated_at { get; set; }

    }//end of class
}//end of namespace 
