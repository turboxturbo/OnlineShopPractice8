namespace OnlineShop.Requests
{
    public class AddNewEmployee
    {
        public string UserName { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Login1 { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public int IdRole { get; set; }
    }
}
