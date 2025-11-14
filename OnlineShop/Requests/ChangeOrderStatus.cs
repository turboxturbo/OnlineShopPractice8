using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OnlineShop.Models;

namespace OnlineShop.Requests
{
    public class ChangeOrderStatus
    {
        
        public int IdStatus { get; set; }
    }
}
