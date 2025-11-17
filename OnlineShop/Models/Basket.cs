using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Basket
    {
        [Key]
        public int IdBasket {  get; set; }
        [Required]
        public int IdUser { get; set; }
        [ForeignKey("IdUserBasket")]
        public User user { get; set; }
        
        
        

        
        
    }
}
