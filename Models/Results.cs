using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_Part_2_Restful_API_Development.Models
{//start of namespace 

    [Table("Results")]
    public class Results
    {//start of class
        
        [Key]
        [Column("result_id")]
        public int result_id { get; set; }

        [Column("registration_id")]
        public int registration_id { get; set; }     // Here lies the foreign key to Registration with a one-to-one relationship

        [Column("finish_time")]
        [Required]
        public TimeSpan finish_time { get; set; }

        [Column("overall_rank")]
        [Required]
        public int? overall_rank { get; set; }

        [Column("category_rank")]
        [Required]
        public int? category_rank { get; set; }

        [Column("status")]
        [Required]
        public string status { get; set; }           // You choose between the following OK / DNF / DQ / DNS

        [Column("created_at")]
        [Required]
        public DateTime? created_at { get; set; }

        [Column("updated_at")]
        [Required]
        public DateTime? updated_at { get; set; }

    }//end of class
}//end of namespace 
