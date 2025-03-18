namespace KIOS.Integration.Web.Model
{
    public class Orders
    {
        public string itemId { get; set; }
        public string itemName { get; set; }
        public decimal quantity { get; set; }
        public string size { get; set; }
        public string? lineComment { get; set; }
        public string? stationId { get; set; } // 1,2,3
        public string? stationName { get; set; } // PizzaStation, Pasta, Sandwich
        public string posId { get; set; }
    }
}
