

namespace ArabazonPlatform.Domain.Domains.Authorisation
{
	public class Action : SharedBase
	{
		public int ActionId { get; set; }
		public string Name { get; set; }
		public string NameAr { get; set; }

		public virtual List<RoleScreenAction> RoleScreenActions { get; set; }
        public virtual List<ScreenAction> ScreenActions { get; set; }

    }
}
