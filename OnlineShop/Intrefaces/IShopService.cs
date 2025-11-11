using Microsoft.AspNetCore.Mvc;

namespace OnlineShop.Intrefaces
{
    public interface IShopService
    {
        Task<IActionResult> GetItems(string NameItem, string NameCategory);
    }
}
