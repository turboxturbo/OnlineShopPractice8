using System.Runtime.InteropServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ShopService(ContextDb contextDb, JWTGenerator generator, IHttpContextAccessor httpContextAccessor)
        {
            _contextDb = contextDb;
            _jWTGenerator = generator;
            _httpContextAccessor = httpContextAccessor;
        }
        
        public async Task<IActionResult> AuthUser(AuthUser logindata)
        {
            var user = await _contextDb.Logins.Include(l => l.user).FirstOrDefaultAsync(l => l.Password == logindata.Password && l.Login1 == logindata.Login);
            if (user == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Login not found" });
            }

            string token = _jWTGenerator.GenerateToken(new LoginPassword()
            {
                IdRole = user.user.IdRole,
                IdUser = user.IdUser
            });
            _contextDb.Sessions.Add(new Session { Token = token, IdUser = user.IdUser });
            _contextDb.SaveChanges();
            return new OkObjectResult(new { token, status = true });
        }
        public async Task<IActionResult> CreateNewUserAndLoginAsync(CreateNewUser newUser)
        {
            var login = new Login()
            {
                user = new User()
                {
                    Description = newUser.Description,
                    UserName = newUser.Name,
                    IdRole = 2
                },
                Password = newUser.Password,
                Login1 = newUser.Login
            };

            await _contextDb.AddAsync(login);
            await _contextDb.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true
            });
        }

        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = _contextDb.Users.ToList();
            return new OkObjectResult(new
            {
                data = new { users = users },
                status = true
            });
        }

        public async Task<IActionResult> GetItems(string NameItem, string NameCategory) //получить товары
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
        public async Task<IActionResult> AddItemInBAsket(int idtem, string quantity)
        {
            var item = _contextDb.Items.Include(i => i.IdItem ==  idtem).FirstOrDefault();
            var userid = _httpContextAccessor.HttpContext?.User;
            var userIdClaim = int.Parse(userid?.FindFirst("UserId").Value);

            
            if (item == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Такого товара нет" });
            }
            var basket = _contextDb.Baskets.Include(b => b.IdUser == userIdClaim).FirstOrDefault();
            
            if (basket == null)
            {
                var additem = new BasketItem()
                {
                    basket = new Basket()
                    {
                        IdUser = userIdClaim,
                    },
                    IdBasket = userIdClaim,
                    IdItem = idtem,
                    Quantity = quantity
                };
                _contextDb.BasketItems.Add(additem);
            }
            else
            {
                var additem = new BasketItem()
                {
                    IdBasket = basket.IdBasket,
                    IdItem = idtem,
                    Quantity = quantity
                };
                _contextDb.BasketItems.Add(additem);
            }
            await _contextDb.SaveChangesAsync();
            return new ObjectResult(new { status = true});
        }
        public async Task<IActionResult> CreateOrder()
        {
            
            var userid = _httpContextAccessor.HttpContext?.User;
            var userIdClaim = int.Parse(userid?.FindFirst("UserId").Value);
            var basket = _contextDb.Baskets.Include(b => b.IdUser == userIdClaim).FirstOrDefault();
            var neworder = new Order() 
            { 
                IdBasket = basket.IdBasket,
                IdStatus = 1
            };
            _contextDb.Orders.AddAsync(neworder);
            await _contextDb.SaveChangesAsync();
            return new ObjectResult(new {data = neworder, status = true });
        }
        public async Task<IActionResult> GetOrders()
        {
            var userid = _httpContextAccessor.HttpContext?.User;
            var userIdClaim = int.Parse(userid?.FindFirst("UserId").Value);
            var orders = _contextDb.Orders.Include(o => o.basket.IdUser == userIdClaim).Include(o => o.IdStatus).FirstOrDefault();
            return new OkObjectResult(new {data = orders, status = true});
        }
        public async Task<IActionResult> ChangeUserAndLogin(ChangeUser changeUser)
        {
            var userid = _httpContextAccessor.HttpContext?.User;
            var userIdClaim = int.Parse(userid?.FindFirst("UserId").Value);
            var user = await _contextDb.Logins.Include(l => l.user).FirstOrDefaultAsync();

            user.user.PhoneNumber = changeUser.PhoneNumber;
            user.Login1 = changeUser.Login1;
            user.user.Description = changeUser.Description;
            user.user.Email = changeUser.Email;
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = user, status = true });
        }
        public async Task<IActionResult> GetUser() //получить всех юзеров (менеджер)
        {
            var users = await _contextDb.Users.FirstOrDefaultAsync();
            return new OkObjectResult(new {data = users, status = true});
        }
        public async Task<IActionResult> ChangeUser(ChangeUser changeUser) //изменить юзера (менеджер)
        {
            var users = await _contextDb.Logins.Include(l => l.user).FirstOrDefaultAsync();
            users.user.PhoneNumber = changeUser.PhoneNumber;
            users.user.UserName = changeUser.UserName;
            users.user.Description = changeUser.Description;
            users.Login1 = changeUser.Login1;
            users.user.Email = changeUser.Email;
            users.user.Address = changeUser.Address;
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = users, status = true });
        }
        public async Task<IActionResult> DelUser(int iduser) // удалить юзера (менеджер)
        {
            var users = await _contextDb.Users.Include(u => u.IdUser == iduser).FirstOrDefaultAsync();
            _contextDb.Remove(users);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new {data = users, status = true });
        }
        public async Task<IActionResult> AddItems (ChangeItem changeItem) //добавить товар (менеджер)
        {
            var items = _contextDb.Items.FirstOrDefaultAsync();
            var newitem = new Item
            {
                IdCategory = changeItem.IdCategory,
                NameItem = changeItem.NameItem,
                DescriptionItem = changeItem.DescriptionItem,
                Price = changeItem.Price,
                Stock = changeItem.Stock,
                isActive = changeItem.isActive,
                createdat = DateTime.Now,
            };
            _contextDb.Add(newitem);
            await _contextDb.SaveChangesAsync();
        }
        public async Task<IActionResult> ChangeItem (ChangeItem changeItem, int iditem) // изменить товар (менеджер)
        {
            var items = _contextDb.Items.Include(i => i.IdItem == iditem).FirstOrDefaultAsync();
            items.IdCategory = changeItem.IdCategory;
                NameItem = changeItem.NameItem;
            DescriptionItem = changeItem.DescriptionItem;
            Price = changeItem.Price;
            Stock = changeItem.Stock;
            isActive = changeItem.isActive;
            createdat = DateTime.Now;
            
        }

    }
}
