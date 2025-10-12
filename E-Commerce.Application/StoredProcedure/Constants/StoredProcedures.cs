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
        public const string BrandStoreFilter = "SP_BrandStoreFilter";
        public const string CategoryStoreFilter = "SP_CategoryStoreFilter";
        public const string ShopStoreFilter = "SP_ShopStoreFilter";
        public const string ColorsStoreFilter = "SP_ColorsStoreFilter";
        public const string DiscountsStoreFilter = "SP_DiscountsStoreFilter";
        public const string SizesStoreFilter = "SP_SizesStoreFilter";
        public const string TopBrandsDashboardFooter = "SP_TopBrandsDashboardFooter";
        public const string WomenFashionsDashboardFooter = "SP_WomenFashionsDashboardFooter";
        public const string MenFashionsDashboardFooter = "SP_MenFashionsDashboardFooter";
        public const string KidsDashboardFooter = "SP_KidsDashboardFooter";
        public const string BeautyDashboardFooter = "SP_BeautyDashboardFooter";
    }

    public class ReportParameter
    {
        public const string PageNumber = "PageNumber";
        public const string PageSize = "PageSize";
        public const string SearchText = "SearchText";
        public const string Language = "Language";
    }
}