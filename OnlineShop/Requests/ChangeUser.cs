using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OnlineShop.Models;

namespace OnlineShop.Requests
{
    public class ChangeUser
    {
        public int Idemployee {  get; set; }
        public string UserName { get; set; } 
        public string Description { get; set; } 
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string Login1 { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public int idrole { get; set; }

    }
}
