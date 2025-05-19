namespace KIOS.Integration.Web.Model
{
    public class DTCreateOrderModel
    {
        public string time { get; set; }
        public int storeNo { get; set; }
        public bool fullLoad { get; set; }
        public string AltOrderId { get; set; }
        public List<DTOrder> orders { get; set; }
        public List<DTOrderItem> orderItems { get; set; }
    }

    public class DTOrder
    {
        public int orderId { get; set; }
        public int storeNo { get; set; }
        public int clientId { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string addressNo { get; set; }
        public string postCode { get; set; }
        public string secondaryAddress { get; set; }
        public decimal lat { get; set; }
        public decimal lng { get; set; }
        public string phone { get; set; }
        public decimal orderTotal { get; set; }
        public int paymentMethod { get; set; }
        //public string paymentMethod { get; set; }
        public decimal cash { get; set; }
        public string orderTime { get; set; }
        public int saleType { get; set; }
        public int dailyNo { get; set; }
        public int priority { get; set; }
        public string carrierInstructions { get; set; }
        public string vipId { get; set; }
    }

    public class DTOrderItem
    {
        public int orderId { get; set; }
        public int storeNo { get; set; }
        public string kdsList { get; set; }
        public string CutTableDineIn_KDS { get; set; }
        public string CutTableDineIn_Printer { get; set; }
        public string position { get; set; }
        public string itemNo { get; set; }
        public int quantity { get; set; }
        public string description { get; set; }
        public int side { get; set; }
        public string rightSideIcon { get; set; }
        public string style { get; set; }
    }
}





