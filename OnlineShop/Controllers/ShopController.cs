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
        [RoleAuthAtribute([3])]
        public async Task<IActionResult> GetLogs()
        {
            return await _shopService.GetLogs();
        }
        [HttpPost]
        [Route("adduser/admin")]
        [RoleAuthAtribute([3])]
        public async Task<IActionResult> AddNewEmployee([FromBody] ChangeUser addemployee)
        {
            return await _shopService.AddNewEmployee(addemployee);
        }
        [HttpPut]
        [Route("changeemployee")]
        [RoleAuthAtribute([1,3])]
        public async Task<IActionResult> ChangeEmployee([FromBody] ChangeUser changeUser)
        {
            return await _shopService.ChangeEmployee(changeUser);
        }
        [HttpDelete]
        [Route("delemployee")]
        [RoleAuthAtribute([1,3])]
        public async Task<IActionResult> DelEmployees([FromBody]Delemployeerequest delemployee)
        {
            return await _shopService.DelEmployees(delemployee);
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
        public async Task<IActionResult> ChangeOrders([FromBody] ChangeOrderStatus changeOrderStatus)
        {
            return await _shopService.ChangeOrders(changeOrderStatus);
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
        public async Task<IActionResult> ChangeItem([FromBody] ChangeItem changeItem)
        {
            return await _shopService.ChangeItem(changeItem);
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
        public async Task<IActionResult> AddItemInBasket([FromBody] AddItemInBasketRequest additeminbasket)
        {
            return await _shopService.AddItemInBasket(additeminbasket);
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
        public async Task<IActionResult> CreateNewUserAndLoginAsync([FromBody] CreateNewUser newUser)
        {
            return await _shopService.CreateNewUserAndLoginAsync(newUser);
        }
        [HttpPost]
        [Route("auth")]
        //[RoleAuthAtribute([1, 2, 3])]
        public async Task<IActionResult> AuthUser([FromBody] AuthUser logindata)
        {
            return await _shopService.AuthUser(logindata);
        }


    }
}
