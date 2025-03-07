namespace Bangazon.Modules
{
    public class Cart
    {
        public int Id { get; set; }
        public string CustomerId { get; set; } // Firebase UID (One-to-One Relationship)
        public int PaymentType { get; set; }
    }
}
