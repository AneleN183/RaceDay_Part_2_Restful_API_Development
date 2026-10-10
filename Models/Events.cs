using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_Part_2_Restful_API_Development.Models
{//start of namespace 

    [Table("Events")]
    public class Events
    {//start of class 

        [Key]
        [Column("event_id")]
        public int event_id { get; set; }

        [Required]
        [Column("user_id")]
        public int user_id { get; set; }             // Here lies the foreign key to Users 

        [Required]
        [Column("event_title")]
        [MaxLength(100)]
        public string event_title { get; set; }

        [Column("event_description")]
        public string? event_description { get; set; }

        [Required]
        [Column("event_date")]
        public DateTime event_date { get; set; }


        [Required]
        [Column("event_location")]
        [MaxLength(200)]
        public string event_location { get; set; }

        [Column("created_at")]
        public DateTime? created_at { get; set; }

        [Column("updated_at")]
        public DateTime? updated_at { get; set; }

    }//end of class
}//end of namespace 
