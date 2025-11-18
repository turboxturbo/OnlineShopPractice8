namespace OnlineShop.Requests
{
    public class GetItemsRequest
    {
        public string NameItem { get; set; }
        public string NameCategory { get; set; }
        public bool OrderByItem { get; set; }
        public bool OrderByCategory { get; set; }
    }
}
