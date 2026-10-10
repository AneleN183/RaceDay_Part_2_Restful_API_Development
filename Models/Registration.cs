using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_Part_2_Restful_API_Development.Models
{//start of namespace 

    [Table("Registration")]
    public class Registration
    {//start of class 
        [Key]
        [Column("registration_id")]
        public int registration_id { get; set; }

        [Required]
        [Column("user_id")]
        public int user_id { get; set; }             // Here lies the foreign key to  to Users

        [Required]
        [Column("event_category_id")]
        public int event_category_id { get; set; }   // Here lies the foreign key to EventCategories

        [Column("registration_date")]
        public DateTime? registration_date { get; set; }

        [Required]
        [Column("registration_status")]
        [MaxLength(20)]
        public string registration_status { get; set; }

        [Column("created_at")]
        public DateTime? created_at { get; set; }

        [Column("updated_at")]
        public DateTime? updated_at { get; set; }

    }//end of class 
}//end of namespace 
