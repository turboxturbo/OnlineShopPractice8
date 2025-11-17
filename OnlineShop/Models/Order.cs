using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Order
    {
        [Key]
        public int IdOrder { get; set; }
        [Required]
        public int IdMethod { get; set; }
        [ForeignKey("IdMethod")]
        public PaymnetMethod method { get; set; }
        [Required]
        public int IdStatus { get; set; }
        [ForeignKey("IdStatus")]
        public OrderStatus status { get; set; }
        [Required]
        public int IdBasket { get; set; }
        [ForeignKey("IdBasketOrder")]
        public Basket basket { get; set; }

    }
}
