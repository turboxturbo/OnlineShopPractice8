using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OnlineShop.Models;

namespace OnlineShop.Requests
{
    public class ChangeUser
    {
        public string UserName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string Login1 { get; set; }
        public string Password { get; set; }

    }
}
