using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class User
    {
        [Key]
        public int IdUser { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Address { get; set; } 
        public string PhoneNumber { get; set; }
        public DateTime createdat {  get; set; }
        public DateTime updatedat { get; set; }
        [Required]
        [ForeignKey("IdRole")]
        public int IdRole { get; set; }
        public Role role { get; set; }
        public Basket basket { get; set; } // для связи 1 на 1
    }
}
