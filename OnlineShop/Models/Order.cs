using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Order
    {
        [Key]
        public int IdOrder { get; set; }
        [Required]
        [ForeignKey("method")]
        public int IdMethod { get; set; }
        public PaymnetMethod method { get; set; }

        [Required]
        [ForeignKey("status")]
        public int IdStatus { get; set; }
        public OrderStatus status { get; set; }

        [Required]
        [ForeignKey("basket")]
        public int IdBasket { get; set; }
        public Basket basket { get; set; }

    }
}
