namespace ArabazonPlatform.Domain.Domains.DashBoard
{
    public class OrderStatus : SharedBase
    {
        public int OrderStatusId { get; set; }
        public string? Name { get; set; }
        public string? NameAr { get; set; }
        public string? Color { get; set; }

        public virtual List<Order> Orders { get; set; }

    }
}
