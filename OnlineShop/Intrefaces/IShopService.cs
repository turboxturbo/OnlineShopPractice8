using Microsoft.AspNetCore.Mvc;
using OnlineShop.Requests;

namespace OnlineShop.Intrefaces
{
    public interface IShopService
    {
        Task<IActionResult> GetItems(GetItemsRequest getitem);
        Task<IActionResult> GetLogs();
        Task<IActionResult> AddNewEmployee(ChangeUser addemployee, int idrole);
        Task<IActionResult> ChangeEmployee(int idemployee, ChangeUser changeUser);
        Task<IActionResult> DelEmployees(int iduser);
        Task<IActionResult> GetEmployees();
        Task<IActionResult> ChangeOrders(int idorder, ChangeOrderStatus changeOrderStatus);
        Task<IActionResult> GetOrdersStatus();
        Task<IActionResult> ChangeItem(ChangeItem changeItem, int iditem);
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
