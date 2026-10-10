using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_Part_2_Restful_API_Development.Models
{//start of namespace 

    [Table("Categories")]
    public class Categories
    {//start of class 

        [Key]
        [Column("category_id")]
        public int category_id { get; set; }

        [Column("category_name")]
        [Required]
        [MaxLength(50)]
        public string category_name { get; set; }

        [Column("category_description")]
        [Required]
        [MaxLength(255)]
        public string? category_description { get; set; }

        [Column("category_type")]
        [Required]
        [MaxLength(20)] 
        public string category_type { get; set; }    // Here you choose between run / walk / cycle

    }//end of class
}//end of namespace 
