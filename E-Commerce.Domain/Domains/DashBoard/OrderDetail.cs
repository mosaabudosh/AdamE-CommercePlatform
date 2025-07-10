namespace E_Commerce.Domain.Domains.DashBoard
{
    public class OrderDetail : SharedBase
    {
        public Guid OrderDetailId { get; set; }
        public double Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Price { get; set; }
        public Guid OrderId { get; set; }
        public Guid ShopItemId { get; set; }
        public int? ColorId { get; set; }


        public virtual Order Order { get; set; }
        public virtual ShopItem ShopItem { get; set; }
        public virtual Color Color { get; set; }

    }
}
