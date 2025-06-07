using E_Commerce.Domain.Domains.Authorisation;

namespace E_Commerce.Domain.Domains.DashBoard
{
    public class Gender : SharedBase
    {
        public int GenderId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }

        public virtual List<User> Users { get; set; }
        public virtual List<Customer> Customers { get; set; }
    }
}
