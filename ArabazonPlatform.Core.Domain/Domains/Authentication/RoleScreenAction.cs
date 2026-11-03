
namespace ArabazonPlatform.Domain.Domains.Authorisation
{
	public class RoleScreenAction : SharedBase
	{
		public int RoleScreenActionId { get; set; }
		public int RoleScreenId { get; set; }
		public int ActionId { get; set; }

		public virtual RoleScreen RoleScreen { get; set; }
		public virtual Action Action { get; set; }

	}
}
