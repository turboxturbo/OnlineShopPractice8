using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using Microsoft.AspNetCore.Mvc;

namespace OnlineShop.Models
{
    public class Session
    {
        [Key]
        public int IdSession { get; set; }
        public string Token { get; set; } = string.Empty;
        [Required]
        [ForeignKey("IdUser")]
        public int IdUser { get; set; }
        public User user { get; set; }
    }
}
