

namespace ArabazonPlatform.Domain.Domains.Authorisation
{
	public class Module : SharedBase
	{
		public int ModuleId { get; set; }
		public string Name { get; set; }
		public string NameAr { get; set; }
		public int Order { get; set; }
		public bool IsInMenue { get; set; }
        public string Icon { get; set; }

        public virtual List<Screen> Screens { get; set; }
	}
}
