

using E_Commerce.Domain.Domains.Authorisation;

namespace E_Commerce.Domain.Domains
{
	public class City : SharedBase
	{
		public int CityId { get; set; }
		public string Name { get; set; }
		public string NameAr { get; set; }
		public int CountryId { get; set; }

		public virtual Country Country { get; set; }
		public virtual List<User> Users { get; set; }
		public virtual List<Shop> Shops { get; set; }
	}
}
