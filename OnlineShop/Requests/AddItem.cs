namespace OnlineShop.Requests
{
    public class AddItem
    {
        public string NameItem { get; set; }
        public string DescriptionItem { get; set; }
        public string Price { get; set; }
        public string Stock { get; set; }
        public bool isActive { get; set; }
        public DateTime createdat { get; set; }
        public int IdCategory { get; set; }
    }
}
