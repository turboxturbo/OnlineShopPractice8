using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class ActionUser
    {
        [Key]
        public int IdAction { get; set; }
        public string NameAction { get; set; }
    }
}
