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
        private async Task<int?> GetUserIdFromToken()
        {
            string? token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (token == null)
            {
                return null;
            }

            var session = await _contextDb.Sessions
                .FirstOrDefaultAsync(u => u.Token == token);

            return session?.IdUser;
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

            var newlog = new Log
            {
                IdAction = 1,
                IdUser = user.IdUser,
            };

            await _contextDb.Sessions.AddAsync(new Session { Token = token, IdUser = user.IdUser });
            await _contextDb.Logs.AddAsync(newlog);
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
                    IdRole = 2,
                    Email = newUser.Email,
                    Address = newUser.Address,
                    PhoneNumber = newUser.PhoneNumber,
                    createdat = DateTime.Now,
                    updatedat = DateTime.Now
                },
                
                Password = newUser.Password,
                Login1 = newUser.Login
            };

            await _contextDb.AddAsync(login);
            await _contextDb.SaveChangesAsync();

            var newlog = new Log
            {
                IdAction = 2,
                IdUser = login.user.IdUser,
            };

            
            await _contextDb.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();

            

            return new OkObjectResult(new
            {
                status = true
            });
        }

        public async Task<IActionResult> GetAllUsersAsync()
        {

            

            var userid = await GetUserIdFromToken();
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }

            var users = await _contextDb.Users.FirstOrDefaultAsync(u => u.IdUser == userid);


            if (users == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователей нет" });
            }

            var newlog = new Log
            {
                IdAction = 3,
                IdUser = userid.Value,
            };


            await _contextDb.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();

            return new OkObjectResult(new
            {
                data = new { users = users },
                status = true
            });

        }

        public async Task<IActionResult> GetItems(GetItemsRequest getitem) //получить товары sort
        {
            var userid = await GetUserIdFromToken();
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }

            var query = _contextDb.Items.Include(i => i.category).AsQueryable();

            // Применяем фильтры
            if (!string.IsNullOrEmpty(getitem.NameItem))
            {
                query = query.Where(i => i.NameItem.Contains(getitem.NameItem));
            }
             
            if (!string.IsNullOrEmpty(getitem.NameCategory))
            {
                query = query.Where(i => i.category.NameCategory.Contains(getitem.NameCategory));
            }

            if (getitem.OrderByCategory)
            {
                query = query.OrderBy(o => o.category.NameCategory);
            }

            if (getitem.OrderByItem)
            {
                query = query.OrderBy(o => o.NameItem);
            }

            var items = await query.ToListAsync();
            

            if (items == null || items.Count == 0)
            {
                return new NotFoundObjectResult(new { status = false, message = "Таких товаров нет" });
            }

            var newlog = new Log
            {
                IdAction = 4,
                IdUser = userid.Value,
            };
            await _contextDb.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();

            return new OkObjectResult(new
            {
                data = new { items = items },
                status = true
            });

        }
        public async Task<IActionResult> AddItemInBasket(AddItemInBasketRequest additeminbasket)
        {
            var item = await _contextDb.Items.FirstOrDefaultAsync(i => i.IdItem == additeminbasket.idtem);
            string? token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (token == null)
            {
                return new BadRequestObjectResult(new { status = false, message = "Неправильный jwt" });
            }
            var userid = await _contextDb.Sessions.FirstOrDefaultAsync(u => u.Token == token);
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Ошибка" });
            }

            if (item == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Такого товара нет" });
            }
            var basket = await _contextDb.Baskets.FirstOrDefaultAsync(b => b.IdUser == userid.IdUser);
            
            if (basket == null)
            {

                basket = new Basket()
                {
                    IdUser = userid.IdUser
                };

                _contextDb.Baskets.Add(basket);
                await _contextDb.SaveChangesAsync();
            }

            var basketitem = new BasketItem()
            {
                IdBasket = basket.IdBasket,
                IdItem = additeminbasket.idtem,
                Quantity = additeminbasket.quantity
            };
            var newlog = new Log
            {
                IdAction = 5,
                IdUser = userid.IdUser,
            };
            await _contextDb.AddAsync(newlog);
            await _contextDb.BasketItems.AddAsync(basketitem);
            await _contextDb.SaveChangesAsync();
            return new ObjectResult(new { status = true});
        }
        public async Task<IActionResult> CreateOrder()
        {

            string? token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (token == null)
            {
                return new BadRequestObjectResult(new { status = false, message = "Неправильный jwt" });
            }
            var userid = await _contextDb.Sessions.FirstOrDefaultAsync(u => u.Token == token);
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Ошибка" });
            }
            var basket = _contextDb.Baskets.Include(b => b.IdUser == userid.IdUser).FirstOrDefault();
            var neworder = new Order() 
            { 
                IdBasket = basket.IdBasket,
                IdStatus = 1,
                IdMethod = 1,
            };
            var newlog = new Log
            {
                IdAction = 6, // сделал заказ
                IdUser = userid.IdUser,
            };
            await _contextDb.AddAsync(newlog);
            await _contextDb.Orders.AddAsync(neworder);
            await _contextDb.SaveChangesAsync();
            return new ObjectResult(new {data = neworder, status = true });
        }
        public async Task<IActionResult> GetOrders()
        {
            string? token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (token == null)
            {
                return new BadRequestObjectResult(new { status = false, message = "Неправильный jwt" });
            }
            var userid = await _contextDb.Sessions.FirstOrDefaultAsync(u => u.Token == token);
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Ошибка" });
            }
            var orders = await _contextDb.Orders.Include(o => o.basket).Where(o => o.basket.IdUser == userid.IdUser).FirstOrDefaultAsync();
            var newlog = new Log
            {
                IdAction = 7, // сделал заказ
                IdUser = userid.IdUser,
            };
            await _contextDb.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new {data = orders, status = true});
        }
        public async Task<IActionResult> ChangeUserAndLogin(ChangeUser changeUser)
        {
            string? token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (token == null)
            {
                return new BadRequestObjectResult(new { status = false, message = "Неправильный jwt" });
            }
            var userid = await _contextDb.Sessions.FirstOrDefaultAsync(u => u.Token == token);
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Ошибка" });
            }
            var user = await _contextDb.Logins.Include(l => l.user).FirstOrDefaultAsync(l => l.IdUser == userid.IdUser);

            user.user.PhoneNumber = changeUser.PhoneNumber;
            user.Login1 = changeUser.Login1;
            user.user.Description = changeUser.Description;
            user.user.Email = changeUser.Email;

            var newlog = new Log
            {
                IdAction = 7, // изменил профиль
                IdUser = userid.IdUser,
            };
            await _contextDb.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();

            return new OkObjectResult(new { data = user, status = true });
        }
        public async Task<IActionResult> GetUser() //получить всех юзеров (менеджер и админ)
        {
            string? token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (token == null)
            {
                return new BadRequestObjectResult(new { status = false, message = "Неправильный jwt" });
            }
            var userid = await _contextDb.Sessions.FirstOrDefaultAsync(u => u.Token == token);
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Ошибка" });
            }
            var users = await _contextDb.Users.FirstOrDefaultAsync();

            var newlog = new Log
            {
                IdAction = 8, // изменил профиль
                IdUser = userid.IdUser,
            };
            await _contextDb.AddAsync(newlog);
            return new OkObjectResult(new {data = users, status = true});
        }
        public async Task<IActionResult> ChangeUser(ChangeUser changeUser) //изменить юзера (менеджер)
        {
            string? token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (token == null)
            {
                return new BadRequestObjectResult(new { status = false, message = "Неправильный jwt" });
            }
            var userid = await _contextDb.Sessions.FirstOrDefaultAsync(u => u.Token == token);
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Ошибка" });
            }

            var users = await _contextDb.Logins.Include(l => l.user).FirstOrDefaultAsync();
            users.user.PhoneNumber = changeUser.PhoneNumber;
            users.user.UserName = changeUser.UserName;
            users.user.Description = changeUser.Description;
            users.Login1 = changeUser.Login1;
            users.user.Email = changeUser.Email;
            users.user.Address = changeUser.Address;

            var newlog = new Log
            {
                IdAction = 9, // изменил юзера
                IdUser = userid.IdUser,
            };
            await _contextDb.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = users, status = true });
        }
        public async Task<IActionResult> DelUser(int iduser) // удалить юзера (менеджер)
        {
            string? token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (token == null)
            {
                return new BadRequestObjectResult(new { status = false, message = "Неправильный jwt" });
            }
            var userid = await _contextDb.Sessions.FirstOrDefaultAsync(u => u.Token == token);
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Ошибка" });
            }

            var users = await _contextDb.Users.Include(u => u.IdUser == iduser).FirstOrDefaultAsync();

            _contextDb.Remove(users);
            var newlog = new Log
            {
                IdAction = 10, // удалил юзера
                IdUser = userid.IdUser,
            };
            await _contextDb.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new {data = users, status = true });
        }
        public async Task<IActionResult> AddItems (ChangeItem changeItem) //добавить товар (менеджер)
        {
            string? token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (token == null)
            {
                return new BadRequestObjectResult(new { status = false, message = "Неправильный jwt" });
            }
            var userid = await _contextDb.Sessions.FirstOrDefaultAsync(u => u.Token == token);
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Ошибка" });
            }

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
            var newlog = new Log
            {
                IdAction = 11, // добавил товар
                IdUser = userid.IdUser,
            };
            await _contextDb.AddAsync(newlog);
            await _contextDb.AddAsync(newitem);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = items, status = true });
        }
        public async Task<IActionResult> ChangeItem (ChangeItem changeItem) // изменить товар (менеджер)
        {
            string? token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (token == null)
            {
                return new BadRequestObjectResult(new { status = false, message = "Неправильный jwt" });
            }
            var userid = await _contextDb.Sessions.FirstOrDefaultAsync(u => u.Token == token);
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Ошибка" });
            }

            var items = await _contextDb.Items.Where(i => i.IdItem == changeItem.IdItem).FirstOrDefaultAsync();
            items.IdCategory = changeItem.IdCategory;
            items.NameItem = changeItem.NameItem;
            items.DescriptionItem = changeItem.DescriptionItem;
            items.Price = changeItem.Price;
            items.Stock = changeItem.Stock;
            items.isActive = changeItem.isActive;
            items.createdat = DateTime.Now;

            var newlog = new Log
            {
                IdAction = 12, // изменил товар
                IdUser = userid.IdUser,
            };
            await _contextDb.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = items, status = true });
        }
        public async Task<IActionResult> GetOrdersStatus() // получить заказы и их статусы
        {
            string? token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (token == null)
            {
                return new BadRequestObjectResult(new { status = false, message = "Неправильный jwt" });
            }
            var userid = await _contextDb.Sessions.FirstOrDefaultAsync(u => u.Token == token);
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Ошибка" });
            }

            var orders = _contextDb.Orders.Include(o => o.status).FirstOrDefaultAsync();

            var newlog = new Log
            {
                IdAction = 13, // получить заказы и статусы
                IdUser = userid.IdUser,
            };
            await _contextDb.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = orders, status = true });
        }
        public async Task<IActionResult> ChangeOrders(ChangeOrderStatus changeOrderStatus) // изменить статус заказа 
        {
            var orders = await _contextDb.Orders.Include(o => o.status).Where(o => o.IdOrder == changeOrderStatus.idorder).FirstOrDefaultAsync();
            orders.IdStatus = changeOrderStatus.IdStatus;
            return new OkObjectResult(new { data = orders, status = true });
        }
        public async Task<IActionResult> ChangeUserMen(ChangeUser changeUser) // изменить самого себя (менеджер)
        {
            string? token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (token == null)
            {
                return new BadRequestObjectResult(new { status = false, message = "Неправильный jwt" });
            }
            var userid = await _contextDb.Sessions.FirstOrDefaultAsync(u => u.Token == token);
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Ошибка" });
            }
            var user = await _contextDb.Logins.Include(u=>u.user).Where(u => u.IdUser == userid.IdUser).FirstOrDefaultAsync();
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
        public async Task<IActionResult> DelEmployees(Delemployeerequest delemployee) // удалить сотрудника (админ)
        {
            var employees = _contextDb.Users.Include(u => u.IdRole == 1 || u.IdRole == 3).Where(u =>u.IdUser== delemployee.iduser).FirstOrDefaultAsync();
            _contextDb.Remove(employees);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = employees, status = true });
        }
        public async Task<IActionResult> ChangeEmployee(ChangeUser changeUser)
        {
            var user = await _contextDb.Logins.Include(u => u.user).Where(u => u.IdUser == changeUser.Idemployee).FirstOrDefaultAsync();
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
        public async Task<IActionResult> AddNewEmployee(ChangeUser addemployee) // добавить нового юзера/сотрудника(admin)
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
                    IdRole = addemployee.idrole,
                }

            };
            _contextDb.Add(newuser);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = newuser, status = true });
        }
        public async Task<IActionResult> GetLogs() // получить все логи пользователей
        {
            var logs = await _contextDb.Logs.FirstOrDefaultAsync();
            return new OkObjectResult(new { data = logs, status = true });
        }
        

    }
}
