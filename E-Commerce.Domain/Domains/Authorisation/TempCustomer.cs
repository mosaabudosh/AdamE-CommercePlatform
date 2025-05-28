

namespace E_Commerce.Domain.Domains.Authorisation
{
	public class TempCustomer : SharedBase
	{
		public Guid TempCustomerId { get; set; }
        public string FirstName { get; set; }
        public string? SecondName { get; set; }
        public string LastName { get; set; }
        public string FirstNameAr { get; set; }
        public string? SecondNameAr { get; set; }
        public string LastNameAr { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }

        public virtual List<Cart> Carts { get; set; }

	}
}
