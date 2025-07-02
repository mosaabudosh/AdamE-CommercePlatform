using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.StoredProcedure.Constants
{
    public class StoredProcedures
    {
        public const string FeaturedProducts = "SP_FeaturedProducts";
        public const string StoreProducts = "SP_StoreProducts";
    }

    public class ReportParameter
    {
        public const string PageNumber = "PageNumber";
        public const string PageSize = "PageSize";
        public const string SearchText = "SearchText";
    }
}