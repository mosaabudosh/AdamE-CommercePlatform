
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
			Print = 5
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
			Report = 4
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
			Male = 1,
			Female = 2
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
			Rejected = 7
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
    }
}
