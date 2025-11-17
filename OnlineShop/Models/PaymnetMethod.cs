using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class PaymnetMethod
    {
        [Key]
        public int IdMethod { get; set; }
        public string NameMethod { get; set; }    
    }
}
