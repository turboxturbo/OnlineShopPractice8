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
            await _contextDb.SaveChangesAsync();
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

            await _contextDb.Logins.AddAsync(login);
            await _contextDb.SaveChangesAsync();

            var newlog = new Log
            {
                IdAction = 2,
                IdUser = login.user.IdUser,
            };

            await _contextDb.Logs.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true
            });
        }

        public async Task<IActionResult> GetAllUsersAsync() // получить профиль
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


            await _contextDb.Logs.AddAsync(newlog);
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
            await _contextDb.Logs.AddAsync(newlog);
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
            var userid = await GetUserIdFromToken();
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }

            if (item == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Такого товара нет" });
            }
            var basket = await _contextDb.Baskets.FirstOrDefaultAsync(b => b.IdUser == userid);

            if (basket == null)
            {
                basket = new Basket()
                {
                    IdUser = userid.Value
                };

                await _contextDb.Baskets.AddAsync(basket);
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
                IdUser = userid.Value,
            };
            await _contextDb.Logs.AddAsync(newlog);
            await _contextDb.BasketItems.AddAsync(basketitem);
            await _contextDb.SaveChangesAsync();
            return new ObjectResult(new { status = true, newitems = basketitem });
        }
        public async Task<IActionResult> CreateOrder(MethodsRequests methods)
        {

            var userid = await GetUserIdFromToken();
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }
            var basket = _contextDb.Baskets.FirstOrDefault(b => b.IdUser == userid);
            var neworder = new Order()
            {
                IdBasket = basket.IdBasket,
                IdStatus = 1,
                IdMethod = methods.IdPay,
                IdMethodDelivery = methods.IdDelivery,
            };
            var newlog = new Log
            {
                IdAction = 6, // сделал заказ
                IdUser = userid.Value,
            };
            await _contextDb.Logs.AddAsync(newlog);
            await _contextDb.Orders.AddAsync(neworder);
            await _contextDb.SaveChangesAsync();
            return new ObjectResult(new { data = neworder, status = true });
        }
        public async Task<IActionResult> GetOrders()
        {
            var userid = await GetUserIdFromToken();
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }
            var orders = await _contextDb.Orders.Include(o => o.basket).Where(o => o.basket.IdUser == userid).ToListAsync();
            if (orders.Count == 0 || orders == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Нет товаров" });
            }
            var newlog = new Log
            {
                IdAction = 7, // получил заказы
                IdUser = userid.Value,
            };
            await _contextDb.Logs.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = orders, status = true });
        }
        public async Task<IActionResult> ChangeUserAndLogin(ChangeUser changeUser) // для юзера изменить профиль
        {
            var userid = await GetUserIdFromToken();
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }
            var user = await _contextDb.Logins.Include(l => l.user).FirstOrDefaultAsync(l => l.IdUser == userid);

            user.user.PhoneNumber = changeUser.PhoneNumber;
            user.Login1 = changeUser.Login1;
            user.user.Description = changeUser.Description;
            user.user.Email = changeUser.Email;
            user.user.Address = changeUser.Address;
            user.user.updatedat = DateTime.Now;
            user.user.UserName = changeUser.UserName;
            user.Password = changeUser.Password;

            var newlog = new Log
            {
                IdAction = 3, // изменил профиль
                IdUser = userid.Value,
            };
            await _contextDb.Logs.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();

            return new OkObjectResult(new { data = user, status = true });
        }
        public async Task<IActionResult> GetUser() //получить всех юзеров (менеджер и админ)
        {
            var userid = await GetUserIdFromToken();
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }

            var users = await _contextDb.Users.ToListAsync();
            if (users == null || users.Count == 0)
            {
                return new NotFoundObjectResult(new { status = false, message = "Нет пользователей и сотрудников" });
            }

            var newlog = new Log
            {
                IdAction = 8, // изменил профиль
                IdUser = userid.Value,
            };
            await _contextDb.Logs.AddAsync(newlog);
            return new OkObjectResult(new { data = users, status = true });
        }
        public async Task<IActionResult> ChangeUser(ChangeUserForMan changeUser) //изменить юзера (менеджер)
        {
            var userid = await GetUserIdFromToken();
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }

            var users = await _contextDb.Logins.Include(l => l.user).FirstOrDefaultAsync(l => l.IdUser == changeUser.IdUser);
            if (users == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }
            if (users.user.IdRole == 3 || users.user.IdRole == 1)
            {
                return new NotFoundObjectResult(new { status = false, message = "Недостаточно прав" });
            }
            users.user.PhoneNumber = changeUser.PhoneNumber;
            users.user.UserName = changeUser.UserName;
            users.user.Description = changeUser.Description;
            users.Login1 = changeUser.Login1;
            users.user.Email = changeUser.Email;
            users.user.Address = changeUser.Address;
            users.Password = changeUser.Password;
            users.user.updatedat = DateTime.Now;

            var newlog = new Log
            {
                IdAction = 9, // изменил юзера
                IdUser = userid.Value,
            };
            await _contextDb.Logs.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = users, status = true });
        }
        public async Task<IActionResult> DelUser(Delemployeerequest delemployeerequest) // удалить юзера (менеджер)
        {
            var userid = await GetUserIdFromToken();
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }

            var users = await _contextDb.Users.FirstOrDefaultAsync(u => u.IdUser == delemployeerequest.iduser);

            if (users == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Такого пользователя нет" });
            }
            if (userid == 3 && (delemployeerequest.iduser == 1 || delemployeerequest.iduser == 3))
            {
                return new NotFoundObjectResult(new { status = false, message = "Не достаточно прав" });
            }

            _contextDb.Users.Remove(users);
            var newlog = new Log
            {
                IdAction = 10, // удалил юзера
                IdUser = userid.Value,
            };
            await _contextDb.Logs.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = users, status = true });
        }
        public async Task<IActionResult> AddItems(AddItem addItem) //добавить товар (менеджер)
        {
            var userid = await GetUserIdFromToken();
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }

            var newitem = new Item
            {
                IdCategory = addItem.IdCategory,
                NameItem = addItem.NameItem,
                DescriptionItem = addItem.DescriptionItem,
                Price = addItem.Price,
                Stock = addItem.Stock,
                isActive = addItem.isActive,
                createdat = DateTime.Now,
            };
            var newlog = new Log
            {
                IdAction = 11, // добавил товар
                IdUser = userid.Value,
            };
            await _contextDb.Logs.AddAsync(newlog);
            await _contextDb.Items.AddAsync(newitem);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = newitem, status = true });
        }
        public async Task<IActionResult> ChangeItem(ChangeItem changeItem) // изменить товар (менеджер)
        {
            var userid = await GetUserIdFromToken();
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }

            try
            {
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
                    IdUser = userid.Value,
                };
                await _contextDb.Logs.AddAsync(newlog);
                await _contextDb.SaveChangesAsync();
                return new OkObjectResult(new { data = items, status = true });
            }
            catch
            {
                return new NotFoundObjectResult(new { status = false, message = "Ошибка" });
            }
        }
        public async Task<IActionResult> GetOrdersStatus() // получить заказы и их статусы
        {
            var userid = await GetUserIdFromToken();
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }

            var orders = await _contextDb.Orders.Include(o => o.status).ToListAsync();

            if (orders == null || orders.Count == 0)
            {
                return new NotFoundObjectResult(new { status = false, message = "Нет заказов и статусов" });
            }

            var newlog = new Log
            {
                IdAction = 13, // получить заказы и статусы
                IdUser = userid.Value,
            };
            await _contextDb.Logs.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = orders, status = true });
        }
        public async Task<IActionResult> ChangeOrders(ChangeOrderStatus changeOrderStatus) // изменить статус заказа 
        {
            var userid = await GetUserIdFromToken();
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }

            var orders = await _contextDb.Orders.Include(o => o.status).Where(o => o.IdOrder == changeOrderStatus.idorder).FirstOrDefaultAsync();
            if (orders == null)
            {
                return new NotFoundObjectResult(new { message = "Нет заказа с таким id", status = true });
            }

            orders.IdStatus = changeOrderStatus.IdStatus;

            var newlog = new Log
            {
                IdAction = 15, // изменить заказы
                IdUser = userid.Value,
            };
            await _contextDb.Logs.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = orders, status = true });
        }
        //public async Task<IActionResult> ChangeUserMen(ChangeUser changeUser) // изменить самого себя (менеджер)
        //{
        //    var userid = await GetUserIdFromToken();
        //    if (userid == null)
        //    {
        //        return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
        //    }

        //    var user = await _contextDb.Logins.Include(u=>u.user).Where(u => u.IdUser == userid).FirstOrDefaultAsync();
        //    user.Login1 = changeUser.Login1;
        //    user.Password = changeUser.Password;
        //    user.user.Address = changeUser.Address;
        //    user.user.PhoneNumber = changeUser.PhoneNumber;
        //    user.user.updatedat = DateTime.Now;
        //    user.user.UserName = changeUser.UserName;
        //    user.user.Description = changeUser.Description;
        //    user.user.Email = changeUser.Email;

        //    var newlog = new Log
        //    {
        //        IdAction = 16, // изменить заказы
        //        IdUser = userid.Value,
        //    };
        //    await _contextDb.Logs.AddAsync(newlog);
        //    await _contextDb.SaveChangesAsync();
        //    return new OkObjectResult(new { data = user, status = true });
        //}
        public async Task<IActionResult> GetEmployees() // получить всех сотрудником (админ)
        {
            var userid = await GetUserIdFromToken();
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }
            var employees = await _contextDb.Users.Where(u => u.IdRole == 1 || u.IdRole == 3).ToListAsync();
            if (employees == null || employees.Count == 0)
            {
                return new NotFoundObjectResult(new { message = "Нет сотрудников(", status = true });
            }
            var newlog = new Log
            {
                IdAction = 16, // получить всех сотрудников
                IdUser = userid.Value,
            };
            await _contextDb.Logs.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = employees, status = true });
        }
        public async Task<IActionResult> DelEmployees(Delemployeerequest delemployee) // удалить сотрудника (админ)
        {
            var userid = await GetUserIdFromToken();
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }

            var employees = await _contextDb.Users.FirstOrDefaultAsync(u => u.IdUser == delemployee.iduser);
            if (employees == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }
            if (employees.IdRole == 1)
            {
                return new BadRequestObjectResult(new { status = false, message = "недостаточно прав, чтобы удалить админа" });
            }
            _contextDb.Remove(employees);
            var newlog = new Log
            {
                IdAction = 17, 
                IdUser = userid.Value,
            };
            await _contextDb.Logs.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = employees, status = true });
        }
        public async Task<IActionResult> ChangeEmployee(ChangeEmployeesAndUsers changeUser)
        {
            var userid = await GetUserIdFromToken();
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }

            var user = await _contextDb.Logins.Include(u => u.user).Where(u => u.IdUser == changeUser.IdUser).FirstOrDefaultAsync();
            if (user == null)
            {
                return new NotFoundObjectResult(new { message = "Нет заказа с таким id", status = false });
            }

            try
            {
                user.Login1 = changeUser.Login1;
                user.Password = changeUser.Password;
                user.user.Address = changeUser.Address;
                user.user.PhoneNumber = changeUser.PhoneNumber;
                user.user.updatedat = DateTime.Now;
                user.user.UserName = changeUser.UserName;
                user.user.Description = changeUser.Description;
                user.user.Email = changeUser.Email;
                user.user.IdRole = changeUser.IdRole;
            }
            catch
            {
                return new BadRequestObjectResult(new { message = "ошибка", status = false });
            }
            

            var newlog = new Log
            {
                IdAction = 18, 
                IdUser = userid.Value,
            };
            await _contextDb.Logs.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = user, status = true });
        }
        public async Task<IActionResult> AddNewEmployee(AddNewEmployee addemployee) // добавить нового юзера/сотрудника(admin)
        {
            var userid = await GetUserIdFromToken();
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }

            try
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
                        IdRole = addemployee.IdRole,
                    }

                };
                await _contextDb.Logins.AddAsync(newuser);
                await _contextDb.SaveChangesAsync();

                var newlog = new Log
                {
                    IdAction = 19,
                    IdUser = userid.Value,
                };
                await _contextDb.Logs.AddAsync(newlog);
                await _contextDb.SaveChangesAsync();
                return new OkObjectResult(new { data = newuser, status = true });
            }
            catch
            {
                return new BadRequestObjectResult(new { message = "Ошибка ", status = false });
            }
        }
        public async Task<IActionResult> GetLogs() // получить все логи пользователей
        {
            var userid = await GetUserIdFromToken();
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Пользователь с таким id не найден" });
            }

            var logs = await _contextDb.Logs.ToListAsync();
            if (logs.Count == 0 || logs == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Нет логов"});
            }

            var newlog = new Log
            {
                IdAction = 14, // получить заказы и статусы
                IdUser = userid.Value,
            };
            await _contextDb.Logs.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new { data = logs, status = true });
        }
        public async Task<IActionResult> DelItem(DelItemReq delItem)
        {
            var userid = await GetUserIdFromToken();
            if (userid == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Таких пользователей нет" });
            }
            var items = await _contextDb.Items.FirstOrDefaultAsync(i => i.IdItem == delItem.iditem);
            if (items == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Нет предмета с таким id" });
            }

            _contextDb.Remove(items);
            var newlog = new Log
            {
                IdAction = 20,
                IdUser = userid.Value,
            };
            await _contextDb.Logs.AddAsync(newlog);
            await _contextDb.SaveChangesAsync();
            return new OkObjectResult(new {data = items, status = true });
        }

    }
}
