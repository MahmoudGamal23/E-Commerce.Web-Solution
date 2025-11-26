using E_Commerce.Services_Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Presentation.Attributes
{
    public class RedisCacheAttribute : ActionFilterAttribute
    {

        private readonly int durationInMin;

        public RedisCacheAttribute(int DurationInMin = 5)
        {
            durationInMin = DurationInMin;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get Cache Service From DI Container
            var CacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            var CacheKey = CreateCacheKey(context.HttpContext.Request);

            // Check If Cache Data Exist => Return Cached Data
            var CacheValue = await CacheService.GetAsync(CacheKey);

            if (CacheValue is not null)
            {
                context.Result = new ContentResult()
                {
                    Content = CacheValue,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };
                return;
            }

            // If Not Exist Continue Execution And Cache Data After Execution

            var ExecutedContext = await next.Invoke();

            if (ExecutedContext.Result is OkObjectResult result)
            {
                await CacheService.SetAsync(CacheKey, result.Value, TimeSpan.FromMinutes(durationInMin));
            }



        }

        private string CreateCacheKey(HttpRequest request)
        {
            StringBuilder key = new StringBuilder();
            key.Append(request.Path);

            foreach (var item in request.Query.OrderBy(x => x.Key))
                key.Append($"|{item.Key}-{item.Value}");

            return key.ToString();
        }
    }
}
