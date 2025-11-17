using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class BasketItem
    {
        [Key]
        public int IdBasketItem { get; set; } // Уникальный идентификатор товара в корзине
        [Required]
        public int IdBasket { get; set; } // Идентификатор корзины
        [ForeignKey("IdBasketBasketItems")]
        public Basket basket { get; set; } // Связь с корзиной

        [Required]
        public int IdItem { get; set; } // Идентификатор товара
        [ForeignKey("IdItemBasketItems")]
        public Item item { get; set; } // Связь с товаром

        public string Quantity { get; set; } // Количество товара в корзине
    }
}
