using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Log
    {
        [Key]
        public int IdLog { get; set; }
        [Required]
        [ForeignKey("actionuser")]
        public int IdAction { get; set; }
        public ActionUser actionuser { get; set; }
        [Required]
        [ForeignKey("user")]
        public int IdUser { get; set; }
        public User user { get; set; }
    }
}
