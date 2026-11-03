

using ArabazonPlatform.Domain.Domains.CRM;
using ArabazonPlatform.Domain.Domains.DashBoard;

namespace ArabazonPlatform.Domain.Domains.Authorisation
{
    public class User : BaseUser 
	{
		public Guid UserId { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string? ProfileImage { get; set; }
		public string Adresse { get; set; }
		public bool IsBlock { get; set; }
		public int UserTypeId { get; set; }
		public int GenderId { get; set; }
		public int CityId { get; set; }
		public int? ShopId { get; set; }

   
        public virtual List<UserRole> UserRoles { get; set; }
		public virtual UserType UserType { get; set; }
		public virtual Shop Shop { get; set; }
        public virtual List<UserLogin> UserLogins { get; set; }
        public virtual List<User> CreationUsers { get; set; }
        public virtual List<User> LastUpdateUsers { get; set; }
        public virtual List<UserSetting> UserSettings { get; set; }
        public virtual List<Domains.Notification.Notification> Notifications { set; get; }
        public virtual List<Order> CreationUserOrders { get; set; }
        public virtual List<Order> LastUpdateUsersOrders { get; set; }
        public virtual List<Order> DeletedUsersOrders { get; set; }
        public virtual List<Complaint> Complaints { get; set; }
        public virtual List<Suggestion> Suggestions { get; set; }
    }
}
