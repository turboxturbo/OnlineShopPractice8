using Microsoft.AspNetCore.Mvc;
using OnlineShop.CustomAtributes;
using OnlineShop.Intrefaces;
using OnlineShop.Requests;

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
        [RoleAuthAtribute([1, 2, 3])]
        public async Task<IActionResult> GetItems([FromBody] GetItemsRequest getitem)
        {
            return await _shopService.GetItems(getitem);
        }
        [HttpGet]
        [Route("get/logs")]
        [RoleAuthAtribute([1])]
        public async Task<IActionResult> GetLogs()
        {
            return await _shopService.GetLogs();
        }
        [HttpPost]
        [Route("adduser/admin")]
        [RoleAuthAtribute([1])]
        public async Task<IActionResult> AddNewEmployee([FromBody] AddNewEmployee addemployee)
        {
            return await _shopService.AddNewEmployee(addemployee);
        }
        [HttpPut]
        [Route("change/employee")]
        [RoleAuthAtribute([1])]
        public async Task<IActionResult> ChangeEmployee([FromBody] ChangeEmployeesAndUsers changeUser)
        {
            return await _shopService.ChangeEmployee(changeUser);
        }
        [HttpDelete]
        [Route("del/employee")]
        [RoleAuthAtribute([1])]
        public async Task<IActionResult> DelEmployees([FromBody]Delemployeerequest delemployee)
        {
            return await _shopService.DelEmployees(delemployee);
        }
        [HttpGet]
        [Route("get/employee/admin")]
        [RoleAuthAtribute([1])]
        public async Task<IActionResult> GetEmployees()
        {
            return await _shopService.GetEmployees();
        }
        [HttpPut]
        [Route("change/orders")]
        [RoleAuthAtribute([1, 3])]
        public async Task<IActionResult> ChangeOrders([FromBody] ChangeOrderStatus changeOrderStatus)
        {
            return await _shopService.ChangeOrders(changeOrderStatus);
        }
        [HttpGet]
        [Route("get/orders&statuses")]
        [RoleAuthAtribute([1, 3])]
        public async Task<IActionResult> GetOrdersStatus()
        {
            return await _shopService.GetOrdersStatus();
        }
        [HttpPut]
        [Route("change/item")]
        [RoleAuthAtribute([1, 3])]
        public async Task<IActionResult> ChangeItem([FromBody] ChangeItem changeItem)
        {
            return await _shopService.ChangeItem(changeItem);
        }
        [HttpPost]
        [Route("add/items")]
        [RoleAuthAtribute([1, 3])]
        public async Task<IActionResult> AddItems([FromBody] AddItem addItem)
        {
            return await _shopService.AddItems(addItem);
        }
        [HttpPut]
        [Route("change/user/men")]
        [RoleAuthAtribute([3])]
        public async Task<IActionResult> ChangeUser([FromBody] ChangeUserForMan changeUser)
        {
            return await _shopService.ChangeUser(changeUser);
        }
        [HttpGet]
        [Route("get/users/man/admin")]
        [RoleAuthAtribute([1, 3])]
        public async Task<IActionResult> GetUser()
        {
            return await _shopService.GetUser();
        }
        [HttpPut]
        [Route("change/myprofile")]
        [RoleAuthAtribute([1,2,3])]
        public async Task<IActionResult> ChangeUserAndLogin([FromBody] ChangeUser changeUser)
        {
            return await _shopService.ChangeUserAndLogin(changeUser);
        }
        [HttpGet]
        [Route("get/myorders")]
        [RoleAuthAtribute([1,2,3])]
        public async Task<IActionResult> GetOrders()
        {
            return await _shopService.GetOrders();
        }
        [HttpPost]
        [Route("create/order")]
        [RoleAuthAtribute([1,2,3])]
        public async Task<IActionResult> CreateOrder([FromBody]MethodsRequests methods)
        {
            return await _shopService.CreateOrder(methods);
        }
        [HttpPost]
        [Route("add/iteminbasket")]
        [RoleAuthAtribute([1, 2, 3])]
        public async Task<IActionResult> AddItemInBasket([FromBody] AddItemInBasketRequest additeminbasket)
        {
            return await _shopService.AddItemInBasket(additeminbasket);
        }
        [HttpGet]
        [Route("get/profile")]
        [RoleAuthAtribute([1,2, 3])]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            return await _shopService.GetAllUsersAsync();
        }
        [HttpPost]
        [Route("registration")]
        public async Task<IActionResult> CreateNewUserAndLoginAsync([FromBody] CreateNewUser newUser)
        {
            return await _shopService.CreateNewUserAndLoginAsync(newUser);
        }
        [HttpPost]
        [Route("auth")]
        public async Task<IActionResult> AuthUser([FromBody] AuthUser logindata)
        {
            return await _shopService.AuthUser(logindata);
        }
        [HttpDelete]
        [Route("del/user/man")]
        [RoleAuthAtribute([1, 3])]
        public async Task<IActionResult> DelUser([FromBody] Delemployeerequest delemployeerequest)
        {
            return await _shopService.DelUser(delemployeerequest);
        }


    }
}
