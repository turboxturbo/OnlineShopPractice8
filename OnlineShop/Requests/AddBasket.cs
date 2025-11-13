using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OnlineShop.Models;

namespace OnlineShop.Requests
{
    public class AddBasket
    {
        public int IdBasket { get; set; }
        [Required]
        [ForeignKey("userbasket")]
        public int IdUser { get; set; }
        public User user { get; set; }
    }
}
