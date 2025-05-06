

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using System.Globalization;

namespace E_Commerce.Application.Authentication.Security
{
	public class CustomQueryStringValueProvider : QueryStringValueProvider
	{
		int timeZoneOffset = 0;

		public CustomQueryStringValueProvider(HttpRequest request) : base(BindingSource.Query, request.Query, CultureInfo.InvariantCulture)
		{
			timeZoneOffset = int.Parse(request.Headers["TimeZone-Offset"] != StringValues.Empty ? request.Headers["TimeZone-Offset"][0] : "0");
		}

		public override ValueProviderResult GetValue(string key)
		{
			var value = base.GetValue(key);

			if (!string.IsNullOrEmpty(value.ToString()))
			{
				DateTime date;
				if (DateTime.TryParse(value.ToString(), out date))
				{
					return new ValueProviderResult(new StringValues(date.AddMinutes(timeZoneOffset).ToString()), value.Culture);
				}
			}
			return value;
		}
	}
}
