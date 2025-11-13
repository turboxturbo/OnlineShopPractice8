using Microsoft.AspNetCore.Mvc;
using OnlineShop.Intrefaces;
using OnlineShop.CustomAtributes;

namespace OnlineShop.Controllers
{
    public class ShopController
    {
        public readonly IShopService _shopService;
        public ShopController(IShopService shopService)
        {
            _shopService = shopService;
        }
        [HttpGet]
        [Route("get/items")]
        [RoleAuthAtribute([1, 2])]
        public async Task<IActionResult> GetItems(string NameItem, string NameCategory)
        {
            return await _shopService.GetItems(NameItem, NameCategory);
        }
    }
}
