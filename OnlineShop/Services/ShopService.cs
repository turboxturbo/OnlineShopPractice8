using System.Runtime.InteropServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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
            var users = _contextDb.Users.FirstOrDefaultAsync();
            return new OkObjectResult(new
            {
                data = new { users = users },
                status = true
            });
        }

        public async Task<IActionResult> GetItems(string NameItem, string NameCategory) //получить товары sort
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
        public async Task<IActionResult> AddItemInBasket(int idtem, string quantity)
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
            return new OkObjectResult(new { data = items, status = true });
        }
        public async Task<IActionResult> ChangeItem (ChangeItem changeItem, int iditem) // изменить товар (менеджер)
        {
            var items = await _contextDb.Items.Where(i => i.IdItem == iditem).FirstOrDefaultAsync();
            items.IdCategory = changeItem.IdCategory;
            items.NameItem = changeItem.NameItem;
            items.DescriptionItem = changeItem.DescriptionItem;
            items.Price = changeItem.Price;
            items.Stock = changeItem.Stock;
            items.isActive = changeItem.isActive;
            items.createdat = DateTime.Now;
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = items, status = true });
        }
        public async Task<IActionResult> GetOrdersStatus() // получить заказы и их статусы
        {
            var orders = _contextDb.Orders.Include(o => o.status).FirstOrDefaultAsync();
            return new OkObjectResult(new { data = orders, status = true });
        }
        public async Task<IActionResult> ChangeOrders(int idorder, ChangeOrderStatus changeOrderStatus) // изменить статус заказа 
        {
            var orders = await _contextDb.Orders.Include(o => o.status).Where(o => o.IdOrder == idorder).FirstOrDefaultAsync();
            orders.IdStatus = changeOrderStatus.IdStatus;
            return new OkObjectResult(new { data = orders, status = true });
        }
        public async Task<IActionResult> ChangeUserMen(ChangeUser changeUser) // изменить самого себя (менеджер)
        {
            var userid = _httpContextAccessor.HttpContext?.User;
            var userIdClaim = int.Parse(userid?.FindFirst("UserId").Value);
            var user = await _contextDb.Logins.Include(u=>u.user).Where(u => u.IdUser == userIdClaim).FirstOrDefaultAsync();
            user.Login1 = changeUser.Login1;
            user.Password = changeUser.Password;
            user.user.Address = changeUser.Address;
            user.user.PhoneNumber = changeUser.PhoneNumber;
            user.user.updatedat = DateTime.Now;
            user.user.UserName = changeUser.UserName;
            user.user.Description = changeUser.Description;
            user.user.Email = changeUser.Email;
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = user, status = true });
        }
        public async Task<IActionResult> GetEmployees() // получить всех сотрудником (админ)
        {
            var employees = _contextDb.Users.Include(u => u.IdRole == 1 || u.IdRole == 3).FirstOrDefaultAsync();
            return new OkObjectResult(new { data = employees, status = true });
        }
        public async Task<IActionResult> DelEmployees(int iduser) // удалить сотрудника (админ)
        {
            var employees = _contextDb.Users.Include(u => u.IdRole == 1 || u.IdRole == 3).Where(u =>u.IdUser==iduser).FirstOrDefaultAsync();
            _contextDb.Remove(employees);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = employees, status = true });
        }
        public async Task<IActionResult> ChangeEmployee(int idemployee, ChangeUser changeUser)
        {
            var user = await _contextDb.Logins.Include(u => u.user).Where(u => u.IdUser == idemployee).FirstOrDefaultAsync();
            user.Login1 = changeUser.Login1;
            user.Password = changeUser.Password;
            user.user.Address = changeUser.Address;
            user.user.PhoneNumber = changeUser.PhoneNumber;
            user.user.updatedat = DateTime.Now;
            user.user.UserName = changeUser.UserName;
            user.user.Description = changeUser.Description;
            user.user.Email = changeUser.Email;
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = user, status = true });
        }
        public async Task<IActionResult> AddNewEmployee(ChangeUser addemployee, int idrole) // добавить нового юзера/сотрудника(admin)
        {
            var users = _contextDb.Logins.Include(u => u.user).FirstOrDefaultAsync();
            var newuser = new Login
            {
                Login1 = addemployee.Login1,
                Password = addemployee.Password,
                user = new User
                {
                    Address = addemployee.Address,
                    PhoneNumber = addemployee.PhoneNumber,
                    updatedat = DateTime.Now,
                    UserName = addemployee.UserName,
                    Description = addemployee.Description,
                    Email = addemployee.Email,
                    IdRole = idrole
                }

            };
            _contextDb.Add(newuser);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = newuser, status = true });
        }
        public async Task<IActionResult> GetLogs() // получить все логи пользователей
        {
            var logs = await _contextDb.Sessions.FirstOrDefaultAsync();
            return new OkObjectResult(new { data = logs, status = true });
        }
        

    }
}
