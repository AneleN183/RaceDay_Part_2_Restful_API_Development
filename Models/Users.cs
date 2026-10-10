using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_Part_2_Restful_API_Development.Models
{//start of namespace 

    [Table("Users")]//table users in the database 
    public class Users
    {//start of class 

        [Key]
        [Column("user_id")]
        public int user_id { get; set; }

        [Column("first_name")]
        [Required]
        [MaxLength(50)]
        public string first_name { get; set; }

        [Column("last_name")]
        [Required]
        [MaxLength(50)]
        public string last_name { get; set; }

        [Column("email")]
        [Required]
        [MaxLength(100)]
        public string email { get; set; }

        [Column("user_password")]
        [Required]
        [MaxLength(255)]
        public string user_password { get; set; }

        [Column("user_role")]
        [Required]
        [MaxLength(20)]
        public string user_role { get; set; }

        [Column("created_at")]
        public DateTime? created_at { get; set; }

        [Column("updated_at")]
        public DateTime? updated_at { get; set; }

    }//end of class 
}//end of namespace 
