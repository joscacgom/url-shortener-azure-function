using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web.Resource;
using UrlShortener.Function.Data;

namespace UrlShortener.Function
{
    [Authorize]
    [RequiredScope("tasks.read", "tasks.write")]
    public class UrlsGetAllByUserId
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;


        public UrlsGetAllByUserId(AppDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        [Function("UrlsGetAllByUserId")]
        public async Task<IActionResult> Run(
                    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "urlsByUserId/{userId}")] HttpRequest req, string userId)
        {
            var user = _contextAccessor?.HttpContext?.User;
            if (req.Method == HttpMethods.Get)
            {
                var urls = await _context.Url.Where(u => u.UserId == userId).ToListAsync();
                return new OkObjectResult(urls);
            }
            return new BadRequestResult();
          
        }
    }
}
