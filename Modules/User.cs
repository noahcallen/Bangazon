namespace Bangazon.Modules
{
    public class User
    {
        public int Id { get; set; }  // Only for SQL visibility
        public string UId { get; set; } // Firebase UID
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int Zip { get; set; }
    }
}
