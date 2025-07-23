using E_Commerce.Domain.Domains.Authentication;
using E_Commerce.Domain.Domains.Authorisation;
using E_Commerce.Domain.Domains.DashBoard;
using E_Commerce.Domain.Domains.DMS;
using E_Commerce.Domain.Domains.Notification;
using E_Commerce.Domain.Domains.Settings;
using E_Commerce.Infrastructure.Data.Configuration;
using E_Commerce.Infrastructure.Data.Configuration.Authentication;
using E_Commerce.Infrastructure.Data.Configuration.Authorisation;
using E_Commerce.Infrastructure.Data.Configuration.DashBoard;
using E_Commerce.Infrastructure.Data.Configuration.DMS;
using E_Commerce.Infrastructure.Data.Configuration.Notification;
using E_Commerce.Infrastructure.Data.Configuration.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Action = E_Commerce.Domain.Domains.Authorisation.Action;
using File = E_Commerce.Domain.Domains.DMS.File;

namespace E_Commerce.Infrastructure.Data.Context
{
    public class E_CommerceContext : DbContext
	{
        public IConfiguration Configuration { get; }

        #region Constructor
        public E_CommerceContext(DbContextOptions<E_CommerceContext> options, IConfiguration configuration) : base(options)
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
			modelBuilder.ApplyConfiguration(new ItemEntityConfiguration());
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
		public DbSet<Item> Items { get; set; }
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
        #endregion
    }
}
