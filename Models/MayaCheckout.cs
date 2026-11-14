namespace Lucas_The_Tavern_Order_Management_System.Models
{
    public class MayaCheckout
    {
        public TotalAmount TotalAmount { get; set; } = new();
        public RedirectUrl RedirectUrl { get; set; } = new();
        public string RequestReferenceNumber { get; set; } = "";
    }

    public class TotalAmount
    {
        public decimal Value { get; set; }
        public string Currency { get; set; } = "PHP";
    }

    public class RedirectUrl
    {
        public string Success { get; set; } = "";
        public string Failure { get; set; } = "";
        public string Cancel { get; set; } = "";
    }
}
