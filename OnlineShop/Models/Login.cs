using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Login
    {
        [Key]
        public int IdLogin { get; set; }
        public string Login1 { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        [Required]
        public int IdUser { get; set; }
        [ForeignKey("IdUser")]
        public User user { get; set; }
    }
}
