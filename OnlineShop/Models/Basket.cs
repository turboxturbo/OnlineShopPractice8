using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Basket
    {
        [Key]
        public int IdBasket {  get; set; }
        [Required]
        [ForeignKey("userbasket")]
        public int IdUser { get; set; }
        //public User user { get; set; }
        
        //public Order order { get; set; }

        
        
    }
}
