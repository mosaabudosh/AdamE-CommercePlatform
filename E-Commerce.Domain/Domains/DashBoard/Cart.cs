using E_Commerce.Domain.Domains.Authorisation;

namespace E_Commerce.Domain.Domains.DashBoard
{
    public class Cart : SharedBase
    {
        public Guid CartId { get; set; }
        public Guid CustomerId { get; set; }
        public int CartStatusId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual CartStatus CartStatus { get; set; }
        public virtual List<CartDetail> CartDetails { get; set; }
    }
}
