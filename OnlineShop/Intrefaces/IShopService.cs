using Microsoft.AspNetCore.Mvc;
using OnlineShop.Requests;

namespace OnlineShop.Intrefaces
{
    public interface IShopService
    {
        Task<IActionResult> GetItems(GetItemsRequest getitem);
        Task<IActionResult> GetLogs();
        Task<IActionResult> AddNewEmployee(AddNewEmployee addemployee);
        Task<IActionResult> ChangeEmployee(ChangeEmployeesAndUsers changeUser);
        Task<IActionResult> DelEmployees(Delemployeerequest delemployee);
        Task<IActionResult> GetEmployees();
        Task<IActionResult> ChangeOrders(ChangeOrderStatus changeOrderStatus);
        Task<IActionResult> GetOrdersStatus();
        Task<IActionResult> ChangeItem(ChangeItem changeItem);
        Task<IActionResult> AddItems(AddItem addItem);
        Task<IActionResult> ChangeUser(ChangeUserForMan changeUser);
        Task<IActionResult> GetUser();
        Task<IActionResult> ChangeUserAndLogin(ChangeUser changeUser);
        Task<IActionResult> GetOrders();
        Task<IActionResult> CreateOrder(MethodsRequests methods);
        Task<IActionResult> AddItemInBasket(AddItemInBasketRequest additeminbasket);
        Task<IActionResult> GetAllUsersAsync();
        Task<IActionResult> CreateNewUserAndLoginAsync(CreateNewUser newUser);
        Task<IActionResult> AuthUser(AuthUser logindata);
        Task<IActionResult> DelUser(Delemployeerequest delemployeerequest);

    }
}
