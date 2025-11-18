using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Item
    {
        [Key]
        public int IdItem { get; set; }
        public string NameItem { get; set; } = string.Empty;
        public string DescriptionItem { get; set; } = string.Empty;
        public string Price {  get; set; }
        public string Stock { get; set; }
        public bool isActive { get; set; }
        public DateTime createdat {  get; set; }
        [Required]
        [ForeignKey("category")]
        public int IdCategory { get; set; }
        public Category category { get; set; }
        

    }
}
