using Buoi3.Helper;

namespace Buoi3.Models
{
	public class CartItemCountMiddleware
	{
		private readonly RequestDelegate _next;

		public CartItemCountMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var cart = context.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
			int totalItems = cart.Items.Sum(item => item.Quantity);
			context.Items["CartItemCount"] = totalItems;

			await _next(context);
		}
	}
	public static class MiddlewareExtensions
	{
		public static IApplicationBuilder UseCartItemCount(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<CartItemCountMiddleware>();
		}
	}

}
