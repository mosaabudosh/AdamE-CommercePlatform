namespace ArabazonPlatform.Domain.Domains.DashBoard
{
    public class CartStatus : SharedBase
    {
        public int CartStatusId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string? Color { get; set; }

        public virtual List<Cart> Carts { get; set; }
    }
}
