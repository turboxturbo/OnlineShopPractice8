using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Order
    {
        [Key]
        public int IdOrder { get; set; }
        public OrderStatus status { get; set; }
        public Basket basket { get; set; }
        [Required]
        [ForeignKey("IdBasket")]
        public int IdBasket { get; set; }
        [Required]
        [ForeignKey("IdStatus")]
        public int IdStatus { get; set; }
    }
}
