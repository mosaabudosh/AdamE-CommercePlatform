using ArabazonPlatform.Domain.Domains.Advertisements;
using ArabazonPlatform.Domain.Domains.Authentication;
using ArabazonPlatform.Domain.Domains.Authorisation;
using ArabazonPlatform.Domain.Domains.CMS;
using ArabazonPlatform.Domain.Domains.CRM;
using ArabazonPlatform.Domain.Domains.DashBoard;
using ArabazonPlatform.Domain.Domains.DMS;
using ArabazonPlatform.Domain.Domains.Notification;
using ArabazonPlatform.Domain.Domains.Settings;
using ArabazonPlatform.Infrastructure.Data.Configuration;
using ArabazonPlatform.Infrastructure.Data.Configuration.Advertisements;
using ArabazonPlatform.Infrastructure.Data.Configuration.Authentication;
using ArabazonPlatform.Infrastructure.Data.Configuration.Authorisation;
using ArabazonPlatform.Infrastructure.Data.Configuration.CMS;
using ArabazonPlatform.Infrastructure.Data.Configuration.CRM;
using ArabazonPlatform.Infrastructure.Data.Configuration.DashBoard;
using ArabazonPlatform.Infrastructure.Data.Configuration.DMS;
using ArabazonPlatform.Infrastructure.Data.Configuration.Notification;
using ArabazonPlatform.Infrastructure.Data.Configuration.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Action = ArabazonPlatform.Domain.Domains.Authorisation.Action;
using File = ArabazonPlatform.Domain.Domains.DMS.File;

namespace ArabazonPlatform.Infrastructure.Data.Context
{
    public class ArabazonPlatformContext : DbContext
	{
        public IConfiguration Configuration { get; }

        #region Constructor
        public ArabazonPlatformContext(DbContextOptions<ArabazonPlatformContext> options, IConfiguration configuration) : base(options)
		{
            Configuration = configuration;
        }
        #endregion

        #region Override
        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new ActionEntityConfiguration());
			modelBuilder.ApplyConfiguration(new RoleEntityConfiguration());
			modelBuilder.ApplyConfiguration(new ModuleEntityConfiguration());
			modelBuilder.ApplyConfiguration(new RoleScreenEntityConfiguration());
			modelBuilder.ApplyConfiguration(new RoleScreenActionEntityConfiguration());
			modelBuilder.ApplyConfiguration(new ScreenEntityConfiguration());
			modelBuilder.ApplyConfiguration(new CustomerEntityConfiguration());
			modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
			modelBuilder.ApplyConfiguration(new UserRoleEntityConfiguration());
			modelBuilder.ApplyConfiguration(new UserTypeEntityConfiguration());
			modelBuilder.ApplyConfiguration(new CartEntityConfiguration());
			modelBuilder.ApplyConfiguration(new CartDetailEntityConfiguration());
			modelBuilder.ApplyConfiguration(new CartStatusEntityConfiguration());
			modelBuilder.ApplyConfiguration(new CategoriesEntityConfiguration());
			modelBuilder.ApplyConfiguration(new CitiesEntityConfiguration());
			modelBuilder.ApplyConfiguration(new CountriesEntityConfiguration());
			modelBuilder.ApplyConfiguration(new GenderEntityConfiguration());
            modelBuilder.ApplyConfiguration(new LanguageEntityConfiguration());
			modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
			modelBuilder.ApplyConfiguration(new OrderDetailEntityConfiguration());
			modelBuilder.ApplyConfiguration(new OrderStatusEntityConfiguration());
			modelBuilder.ApplyConfiguration(new ShopEntityConfiguration());
			modelBuilder.ApplyConfiguration(new ShopItemEntityConfiguration());
			modelBuilder.ApplyConfiguration(new TaxEntityConfiguration());
            modelBuilder.ApplyConfiguration(new FolderEntityConfiguration());
            modelBuilder.ApplyConfiguration(new FileTypeEntityConfiguration());
            modelBuilder.ApplyConfiguration(new FileEntityConfiguration());
            modelBuilder.ApplyConfiguration(new DataTypeEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PlatformSettingEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UserSettingEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ScreenActionEntityConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationEntityConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationStatusEntityConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationTypeEntityConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationTypeRoleEntityConfiguration());
            modelBuilder.ApplyConfiguration(new DiscountEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentMethodEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UnitGroupEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UnitEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UserLoginEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerLoginEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ShopItemStatusEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerFavoriteEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ColorGroupEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ColorEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ShopRatingEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ShopItemRatingEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ShopCategoryEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ShopMenuEntityConfiguration());
            modelBuilder.ApplyConfiguration(new BrandEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CouponEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerDeliveryAddressEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerCardInfoEntityConfiguration());
            modelBuilder.ApplyConfiguration(new StaticPageEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ComplaintEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SuggestionEntityConfiguration());
            modelBuilder.ApplyConfiguration(new AdTypeEntityConfiguration());
            modelBuilder.ApplyConfiguration(new AdvertisementStatusEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ShopAdPackageStatusEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ShopAdPackageEntityConfiguration());
            modelBuilder.ApplyConfiguration(new AdvertisementEntityConfiguration());
            modelBuilder.ApplyConfiguration(new AdPackageEntityConfiguration());
            modelBuilder.ApplyConfiguration(new AdImpressionEntityConfiguration());
            modelBuilder.ApplyConfiguration(new AdClickEntityConfiguration());
        }
        #endregion

        #region DbSet
        public DbSet<Action> Actions { get; set; }
		public DbSet<Module> Modules { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<RoleScreen> RoleScreens { get; set; }
		public DbSet<RoleScreenAction> RoleScreenActions { get; set; }
		public DbSet<Screen> Screens { get; set; }
		public DbSet<Customer> Customers { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }
		public DbSet<UserType> UserTypes { get; set; }
		public DbSet<Cart> Carts { get; set; }
		public DbSet<CartDetail> CartDetails { get; set; }
		public DbSet<CartStatus> CartStatuses { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<City> Cities { get; set; }
		public DbSet<Country> Countries { get; set; }
		public DbSet<Gender> Genders { get; set; }
		public DbSet<Language> Languages { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set; }
		public DbSet<OrderStatus> OrderStatuses { get; set; }
		public DbSet<Shop> Shops { get; set; }
		public DbSet<ShopItem> ShopItems { get; set; }
		public DbSet<Tax> Taxs { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<FileType> FileTypes { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<DataType> DataTypes { get; set; }
        public DbSet<PlatformSetting> PlatformSettings { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }
        public DbSet<ScreenAction> ScreenActions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationStatus> NotificationStatuses { get; set; }
        public DbSet<NotificationType> NotificationTypes { get; set; }
        public DbSet<NotificationTypeRole> NotificationTypeRoles { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<UnitGroup> UnitGroups { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<CustomerLogin> CustomerLogins { get; set; }
        public DbSet<ShopItemStatus> ShopItemStatuses { get; set; }
        public DbSet<CustomerFavorite> CustomerFavorites { get; set; }
        public DbSet<ColorGroup> ColorGroups { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ShopRating> ShopRatings { get; set; }
        public DbSet<ShopItemRating> ShopItemRatings { get; set; }
        public DbSet<ShopCategory> ShopCategories { get; set; }
        public DbSet<ShopMenu> ShopMenus { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CustomerDeliveryAddress> CustomerDeliveryAddresses { get; set; }
        public DbSet<CustomerCardInfo> CustomerCardInfos { get; set; }
        public DbSet<StaticPage> StaticPages { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Suggestion> Suggestions { get; set; }
        public DbSet<AdType> AdTypes { get; set; }
        public DbSet<AdvertisementStatus> AdvertisementStatuses { get; set; }
        public DbSet<ShopAdPackageStatus> ShopAdPackageStatuses { get; set; }
        public DbSet<ShopAdPackage> ShopAdPackages { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<AdPackage> AdPackages { get; set; }
        public DbSet<AdImpression> AdImpressions { get; set; }
        public DbSet<AdClick> AdClicks { get; set; }
        #endregion
    }
}
