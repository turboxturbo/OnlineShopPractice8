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
        public string Token { get; set; }
        [Required]
        public int IdUser { get; set; }
        [ForeignKey("IdUser")]
        public User user { get; set; }
    }
}
