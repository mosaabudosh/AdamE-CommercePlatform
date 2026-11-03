using ArabazonPlatform.Domain.Domains.Authorisation;

namespace ArabazonPlatform.Domain.Domains.DashBoard
{
    public class Language : SharedBase
    {
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Code { get; set; }
    }
}
