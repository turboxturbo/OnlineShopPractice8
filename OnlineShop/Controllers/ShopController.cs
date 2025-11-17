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
        [RoleAuthAtribute([1, 2])]
        public async Task<IActionResult> GetItems([FromBody] string NameItem, string NameCategory)
        {
            return await _shopService.GetItems(NameItem, NameCategory);
        }
        [HttpGet]
        [Route("get/logs")]
        [RoleAuthAtribute([3])]
        public async Task<IActionResult> GetLogs()
        {
            return await _shopService.GetLogs();
        }
        [HttpPost]
        [Route("adduser/admin")]
        [RoleAuthAtribute([3])]
        public async Task<IActionResult> AddNewEmployee([FromBody] ChangeUser addemployee, int idrole)
        {
            return await _shopService.AddNewEmployee(addemployee, idrole);
        }
        [HttpPut]
        [Route("changeemployee")]
        [RoleAuthAtribute([1,3])]
        public async Task<IActionResult> ChangeEmployee([FromBody] int idemployee, ChangeUser changeUser)
        {
            return await _shopService.ChangeEmployee(idemployee, changeUser);
        }
        [HttpDelete]
        [Route("delemployee")]
        [RoleAuthAtribute([1,3])]
        public async Task<IActionResult> DelEmployees([FromBody] int iduser)
        {
            return await _shopService.DelEmployees(iduser);
        }
        [HttpGet]
        [Route("getemployee")]
        [RoleAuthAtribute([1, 3])]
        public async Task<IActionResult> GetEmployees()
        {
            return await _shopService.GetEmployees();
        }
        [HttpPost]
        [Route("changeorders")]
        [RoleAuthAtribute([1, 3])]
        public async Task<IActionResult> ChangeOrders([FromBody] int idorder, ChangeOrderStatus changeOrderStatus)
        {
            return await _shopService.ChangeOrders(idorder, changeOrderStatus);
        }
        [HttpGet]
        [Route("getorders")]
        [RoleAuthAtribute([1, 3])]
        public async Task<IActionResult> GetOrdersStatus()
        {
            return await _shopService.GetOrdersStatus();
        }
        [HttpPut]
        [Route("changeitems")]
        [RoleAuthAtribute([1, 3])]
        public async Task<IActionResult> ChangeItem([FromBody] ChangeItem changeItem, int iditem)
        {
            return await _shopService.ChangeItem(changeItem, iditem);
        }
        [HttpPost]
        [Route("additems")]
        [RoleAuthAtribute([1, 3])]
        public async Task<IActionResult> AddItems([FromBody] ChangeItem changeItem)
        {
            return await _shopService.AddItems(changeItem);
        }
        [HttpPut]
        [Route("changeusermen")]
        [RoleAuthAtribute([1])]
        public async Task<IActionResult> ChangeUser([FromBody] ChangeUser changeUser)
        {
            return await _shopService.ChangeUser(changeUser);
        }
        [HttpGet]
        [Route("get/users")]
        [RoleAuthAtribute([1, 3])]
        public async Task<IActionResult> GetUser()
        {
            return await _shopService.GetUser();
        }
        [HttpPut]
        [Route("changeprofile")]
        [RoleAuthAtribute([2])]
        public async Task<IActionResult> ChangeUserAndLogin([FromBody] ChangeUser changeUser)
        {
            return await _shopService.ChangeUserAndLogin(changeUser);
        }
        [HttpGet]
        [Route("getorder")]
        [RoleAuthAtribute([2])]
        public async Task<IActionResult> GetOrders()
        {
            return await _shopService.GetOrders();
        }
        [HttpPost]
        [Route("createorder")]
        [RoleAuthAtribute([1,2,3])]
        public async Task<IActionResult> CreateOrder()
        {
            return await _shopService.CreateOrder();
        }
        [HttpPost]
        [Route("additeminbasket")]
        [RoleAuthAtribute([1, 2, 3])]
        public async Task<IActionResult> AddItemInBasket([FromBody] int idtem, string quantity)
        {
            return await _shopService.AddItemInBasket(idtem, quantity);
        }
        [HttpGet]
        [Route("getusers")]
        [RoleAuthAtribute([1, 2, 3])]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            return await _shopService.GetAllUsersAsync();
        }
        [HttpPost]
        [Route("adduserorregistration")]
        [RoleAuthAtribute([1, 2, 3])]
        public async Task<IActionResult> CreateNewUserAndLoginAsync([FromBody] CreateNewUser newUser)
        {
            return await _shopService.CreateNewUserAndLoginAsync(newUser);
        }
        [HttpPost]
        [Route("auth")]
        [RoleAuthAtribute([1, 2, 3])]
        public async Task<IActionResult> AuthUser([FromBody] AuthUser logindata)
        {
            return await _shopService.AuthUser(logindata);
        }


    }
}
