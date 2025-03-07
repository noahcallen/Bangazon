namespace Bangazon.Modules
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerId { get; set; } // Firebase UID
        public bool IsComplete { get; set; }
        public int CartId { get; set; }
    }
}
