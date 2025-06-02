

namespace E_Commerce.Domain.Domains.Authorisation
{
	public class User : SharedBase
	{
		public Guid UserId { get; set; }
		public string FirstName { get; set; }
		public string SecondName { get; set; }
		public string LastName { get; set; }
		public string FirstNameAr { get; set; }
		public string SecondNameAr { get; set; }
		public string LastNameAr { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string MobileNumber { get; set; }
		public byte[] PasswordHash { get; set; }
		public byte[] PasswordSalt { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string? ProfileImage { get; set; }
		public string Adresse { get; set; }
		public bool IsBlock { get; set; }
		public int UserTypeId { get; set; }
		public int GenderId { get; set; }
		public int CityId { get; set; }
		public int? ShopId { get; set; }


        public virtual User CreationUser { get; set; }
        public virtual User LastUpdateUser { get; set; }
        public virtual List<UserRole> UserRoles { get; set; }
		public virtual UserType UserType { get; set; }
		public virtual Gender Gender { get; set; }
		public virtual City City { get; set; }
		public virtual Shop Shop { get; set; }
		public virtual List<Cart> Carts { get; set; }
		public virtual List<Order> Orders { get; set; }
        public virtual List<UserLogin> UserLogins { get; set; }
        public virtual List<User> CreationUsers { get; set; }
        public virtual List<User> LastUpdateUsers { get; set; }
        public virtual List<UserSetting> UserSettings { get; set; }
    }
}
