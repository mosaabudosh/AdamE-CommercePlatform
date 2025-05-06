

namespace E_Commerce.Domain.Domains.Authorisation
{
	public class Screen : SharedBase
	{
		public int ScreenId { get; set; }
		public string Name { get; set; }
		public string NameAr { get; set; }
		public int Order { get; set; }
		public bool IsInMenue { get; set; }
		public string Url { get; set; }
		public string Icon { get; set; }
		public int ModuleId { get; set; }


		public virtual Module Module { get; set; }
		public virtual List<RoleScreen> RoleScreens { get; set; }
	}
}
