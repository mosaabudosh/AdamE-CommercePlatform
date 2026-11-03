
namespace ArabazonPlatform.Domain.Domains.Authorisation
{
	public class UserType : SharedBase
	{
		public int UserTypeId { get; set; }
		public string Name { get; set; }
		public string NameAr { get; set; }


		public virtual List<User> Users { get; set; }

	}
}
