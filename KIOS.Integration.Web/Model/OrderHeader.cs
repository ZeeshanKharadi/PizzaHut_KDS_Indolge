namespace KIOS.Integration.Web.Model
{
    public class OrderHeader
    {
        public string thirdPartyOrderId { get; set; }
        public string storeId { get; set; }
        public string? comment { get; set; }
        public List<Orders> salesLines { get; set; }
        public OrderHeader()
        {
            salesLines = new List<Orders>();
        }
    }
}
