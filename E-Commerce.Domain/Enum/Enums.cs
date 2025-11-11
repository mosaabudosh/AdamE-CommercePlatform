
namespace E_Commerce.Domain.Enum
{
	public class Enums
	{
		public enum Actions
		{
			Add = 1,
			Edit = 2,
			Delete = 3 ,
			View = 4 ,
			Print = 5,
            Active = 6,
            InActive = 7,
            Restor = 8,
            Download = 9,
            Upload = 10
        }

		public enum Roles
		{
			SuperAdmin = 1,
			Admin = 2,
			VendorAdmin = 3,
			Vendor = 4,
			User = 5
		}

		public enum Modules
		{
			AccssControl = 1,
			Lookup = 2,
			Notification = 3,
			Report = 4,
            CustomerManagement = 5,
            CRM = 6,
            CMS = 7,
            Advertisements = 8
        }

		public enum UserTypes
		{
			OuerTeam = 1,
			Vendor = 2 ,
			Customer = 3
		}

		public enum CartStatuses
		{
			Enterd = 1,
			Ordered = 2,
			InProgrees = 3
		}

		public enum Genders
		{
			Man = 1,
			Woman = 2
		}

		public enum Languages
		{
			English = 1,
			Arabic = 2
		}

		public enum OrderStatuses
		{
			Entered = 1,
			Approved = 2,
			InProgrees = 3,
			InDelivery = 4,
			Delivered = 5,
			Canceled = 6,
			Rejected = 7,
            Refund = 8
        }

        public enum FileTypes
        {
            PDF = 1,
            XML = 2,
            ZIP = 3,
            PNG = 4,
            JPEG = 5,
            MP4 = 6,
            Word = 7,
            PowerPoint = 8,
            Xls = 9,
            Xlsx = 10,
            Doc = 11,
            Docx = 12,
            Text = 13,
            MP3 = 14,
            JPG = 15,
            Worddocx = 16,
            SVG = 17,
            WEBP = 18,
            JFIF = 19,
            GIF = 20
        }

		public enum PlatformSettingsDataTypes
		{
            Integer = 1,
            Decimal = 2,
            String = 3,
            Boolean = 4,
            File = 5,
        }

        public enum PlatformSettings
        {
            DefaultTheme = 1,
            DefaultLanguage = 2,
            Logo = 3,
            LogoAr = 4,
			WaterMark = 5,
            Name = 6,
            NameAr = 7,
			DateFormat = 8,
            In24Format = 9,
            DMSUserPath = 10,
            DMSRootFolderPath = 11,
            DMSCategoryPath = 12,
            DMSCountryPath = 13,
            DefaultPage = 14,
            DefaultMenuType = 15,
            LicenceNumber = 16,
            TaxNumber = 17,
            CRNumber = 18,
            Address = 19,
            AddressAr = 20,
            MobileNumber = 21,
            Website = 22,
            Email = 23,
            DMSCustomerPath = 24,
            DMSItemPath = 25,
            DMSPaymentMethodPath = 26,
            DMSShopItemPath = 27,
            DMSShopPath = 28,
            NumberOfFeaturedItemsInDashBoared = 29,
            NumberOfCategoriesInDashBoared = 30,
            GooglePlayUrl = 31,
            AppStoreUrl = 32,
            FacebookUrl = 33,
            InstagramUrl = 34,
            SnapchatUrl = 35,
            DMSBrandPath = 36
        }

        public enum PlatformSettingsCode
        {
            DefaultTheme,
            DefaultLanguage,
            Logo,
            LogoAr,
            WaterMark,
            Name,
            NameAr,
            DateFormat,
            In24Format,
            DMSUserPath,
            DMSRootFolderPath,
            DMSCategoryPath,
            DMSCountryPath,
            DefaultPage,
            DefaultMenuType,
            LicenceNumber,
            TaxNumber,
            CRNumber,
            Address,
            AddressAr,
            MobileNumber,
            Website,
            Email,
            DMSCustomerPath,
            DMSItemPath,
            DMSPaymentMethodPath,
            DMSShopItemPath,
            DMSShopPath,
            NumberOfFeaturedItemsInDashBoared,
            NumberOfCategoriesInDashBoared,
            GooglePlayUrl,
            AppStoreUrl,
            FacebookUrl,
            InstagramUrl,
            SnapchatUrl,
            DMSBrandPath
        }

        public enum Screens
        {
            Users = 1,
            Roles = 2,
            Action = 3,
            Screens = 4,
            Welcome = 5,
            Chats = 6,
            Settings = 7,
            Cities = 8,
            Countries = 9,
            Brands = 10,
            Shops = 11,
            Taxs = 12,
            Categories = 13,
            Carts = 14,
            CartStatuses = 15,
            Orders = 16,
            Reports = 17,
            Notifications = 18,
            Discounts = 19,
            NotificationStatuses = 20,
            NotificationTypes = 21,
            ShopItems = 22,
            OrderStatuses = 23,
            PaymentMethod = 24,
            SizeGroup = 25,
            ShopItemStatus = 26,
            ColorsGroup = 27,
            Customers = 28,
            StaticPages = 29,
            Complaint = 30,
            Suggestion = 31
        }

        public enum MenuType
        {
            Open = 1,
            Close = 2,
            Short = 3
        }

        public enum NotificationStatuses
        {
            Read = 1,
            UnRead = 2
        }
        public enum NotificationTypes
        {
            CreateOrder = 1,
            ChangeOrderStatus = 2,
            ChangeShopItemStatuses = 3
        }

        public enum ShopItemStatuses
        {
            Entered = 1,
            Approved = 2,
            Rejected = 3
        }

        public enum StaticPages
        {
            AboutUs = 1,
            PrivacyPolicy = 2,
            ConsumerRights = 3
        }
    }
}
