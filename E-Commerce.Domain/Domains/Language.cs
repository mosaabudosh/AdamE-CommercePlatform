
using E_Commerce.Domain.Domains.Authorisation;

namespace E_Commerce.Domain.Domains
{
	public class Language : SharedBase
	{
		public int LanguageId { get; set; }
		public string Name { get; set; }
		public string NameAr { get; set; }
		public string Code { get; set; }
	}
}
