namespace KIOS.Integration.Web.Model
{
    public class Orders
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public string Description { get; set; }
        public string StoreId { get; set; }
        public string PosId { get; set; }
    }
}
