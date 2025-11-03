namespace E_Commerce.Application.Model.Authorisation
{
    public class CustomerCardInfoModel
    {
        public int CustomerCardInfoId { get; set; }
        public string CardNumber { get; set; }
        public string Expiry { get; set; }
        public bool IsRememberCard { get; set; }
        public Guid CustomerId { get; set; }
    }
}
