using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Role
    {
        [Key]
        public int IdRole {  get; set; }
        public string RoleName { get; set; }

    }
}
