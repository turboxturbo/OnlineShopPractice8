using Microsoft.AspNetCore.Mvc;
using OnlineShop.Requests;

namespace OnlineShop.Intrefaces
{
    public interface IShopService
    {
        Task<IActionResult> GetItems(GetItemsRequest getitem);
        Task<IActionResult> GetLogs();
        Task<IActionResult> AddNewEmployee(ChangeUser addemployee);
        Task<IActionResult> ChangeEmployee(ChangeUser changeUser);
        Task<IActionResult> DelEmployees(Delemployeerequest delemployee);
        Task<IActionResult> GetEmployees();
        Task<IActionResult> ChangeOrders(ChangeOrderStatus changeOrderStatus);
        Task<IActionResult> GetOrdersStatus();
        Task<IActionResult> ChangeItem(ChangeItem changeItem);
        Task<IActionResult> AddItems(ChangeItem changeItem);
        Task<IActionResult> ChangeUser(ChangeUser changeUser);
        Task<IActionResult> GetUser();
        Task<IActionResult> ChangeUserAndLogin(ChangeUser changeUser);
        Task<IActionResult> GetOrders();
        Task<IActionResult> CreateOrder();
        Task<IActionResult> AddItemInBasket(AddItemInBasketRequest additeminbasket);
        Task<IActionResult> GetAllUsersAsync();
        Task<IActionResult> CreateNewUserAndLoginAsync(CreateNewUser newUser);
        Task<IActionResult> AuthUser(AuthUser logindata);

    }
}
