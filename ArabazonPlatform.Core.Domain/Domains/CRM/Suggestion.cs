using ArabazonPlatform.Domain.Domains.Authorisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Domain.Domains.CRM
{
    public class Suggestion : SharedBase
    {
        public int SuggestionId { get; set; }
        public string Message { get; set; }
        public Guid? UserId { get; set; }
        public Guid? CustomerId { get; set; }

        public virtual User User { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
