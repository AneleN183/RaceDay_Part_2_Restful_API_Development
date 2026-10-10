using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_Part_2_Restful_API_Development.Models
{//start of namespace 

    [Table("EventCategories")]
    public class EventCategories
    {//start of class 
        [Key]
        [Column("event_category_id")]

        public int event_category_id { get; set; }

        [Column("event_id")]
        [Required]
        public int event_id { get; set; }            // Here lies the foreign key to Events

        [Column("category_id")]
        [Required]
        public int category_id { get; set; }         // Here lies the foreign key  to Categories
        
        [Column("start_time")]
        [Required]
        public TimeSpan start_time { get; set; }

        [Column("entry_fee")]
        [Required]
        public decimal entry_fee { get; set; }

        [Column("created_at")]
        [Required]
        public DateTime? created_at { get; set; }
    }//end of class
}//end of namespace 
