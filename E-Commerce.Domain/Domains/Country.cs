

namespace E_Commerce.Domain.Domains
{
	public class Country : SharedBase
	{
		public int CountryId { get; set; }
		public string Name { get; set; }
		public string NameAr { get; set; }
		public string? Banner { get; set; }

		public virtual List<City> Cities { get; set; }
	}
}
