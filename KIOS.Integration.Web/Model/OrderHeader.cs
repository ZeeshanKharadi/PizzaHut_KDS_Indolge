namespace KIOS.Integration.Web.Model
{
    public class OrderHeader
    {
        public string ThirdPartyOrderId { get; set; }
        public List<Orders> OrderLines { get; set; }
        public OrderHeader()
        {
            OrderLines = new List<Orders>();
        }
    }
}
