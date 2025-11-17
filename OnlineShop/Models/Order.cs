using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Order
    {
        [Key]
        public int IdOrder { get; set; }
        [Required]
        [ForeignKey("IdMethod")]
        public int IdMethod { get; set; }
        public PaymnetMethod method { get; set; }
        public OrderStatus status { get; set; }
        [Required]
        [ForeignKey("IdStatus")]
        public int IdStatus { get; set; }
        [Required]
        [ForeignKey("orderbasket")]
        public int IdBasket { get; set; }
        public Basket basket { get; set; }

    }
}
