

namespace E_Commerce.Domain.Domains
{
	public class SharedBase
	{
		public Guid? CreationUserId { get; set; }
		public DateTime CreationDate { get; set; }
        public Guid? LastUpdateUserId { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public Guid? DeletedUserId { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsActive { get; set; }
	}
}
