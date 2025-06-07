

namespace E_Commerce.Domain.Domains.Authorisation
{
	public class RoleScreen : SharedBase
	{
		public int RoleScreenId { get; set; }
		public int RoleId { get; set; }
		public int ScreenId { get; set; }

		public virtual Role Role { get; set; }
		public virtual Screen Screen { get; set; }
		public virtual List<RoleScreenAction> RoleScreenActions { get; set; }

	}
}
