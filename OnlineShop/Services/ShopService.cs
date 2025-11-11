using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.DataBaseContext;
using OnlineShop.Intrefaces;
using OnlineShop.Models;
using OnlineShop.Requests;
using OnlineShop.UniversalMethods;
using static System.Reflection.Metadata.BlobBuilder;

namespace OnlineShop.Services
{
    public class ShopService : IShopService
    {
        private readonly ContextDb _contextDb;
        private readonly JWTGenerator _jWTGenerator;
        public ShopService(ContextDb contextDb, JWTGenerator generator)
        {
            _contextDb = contextDb;
            _jWTGenerator = generator;
        }
        public async Task<IActionResult> GetItems(string NameItem, string NameCategory)
        {
            var items = _contextDb.Items.Include(i => i.category).AsQueryable();
            if (!string.IsNullOrEmpty(NameItem) && !string.IsNullOrEmpty(NameCategory))
            {
                items = items.Where(i => i.category.NameCategory == NameCategory && i.NameItem == NameItem);
            }
            if (items == null)
            {
                return new NotFoundObjectResult(new {status = false, message = "Таких товаров нет"});
            }
            return new ObjectResult(new
            {
                data = new { items = items },
                status = true
            });
        }
        //public async Task<IActionResult> ChangeUser(ChangeUser changeUser, )
        //{
        //    var user = 
        //}
    }
}
