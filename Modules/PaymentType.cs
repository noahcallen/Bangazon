namespace Bangazon.Modules
{
    public class PaymentType
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string CustomerId { get; set; } // Firebase UID
    }
}
