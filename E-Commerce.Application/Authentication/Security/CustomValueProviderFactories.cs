
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace E_Commerce.Application.Authentication.Security
{
	public class CustomValueProviderFactories : IValueProviderFactory
	{
		public CustomValueProviderFactories()
		{
		}

		public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
		{
			context.ValueProviders.Insert(0, new CustomQueryStringValueProvider(context.ActionContext.HttpContext.Request));
			return Task.CompletedTask;
		}
	}
}
