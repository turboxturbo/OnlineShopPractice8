using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class OrderStatus
    {
        [Key]
        public int IdStatus { get; set; }
        public string Status { get; set; }
    }
}
