using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web.Resource;
using Newtonsoft.Json;
using UrlShortener.Function.Data;
using UrlShortener.Function.Models;

namespace UrlShortener.Function
{
    [Authorize]
    [RequiredScope("tasks.read", "tasks.write")]
    public class UrlsGetByIdUpdateDelete
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public UrlsGetByIdUpdateDelete(AppDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        [Function("UrlsGetByIdUpdateDelete")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,
             "get","put","delete", Route = "urls/{id}")] HttpRequest req
            , int id)
        {
            var user = _contextAccessor?.HttpContext?.User;
            if (req.Method == HttpMethods.Get)
            {
                var url = await _context.Url.FirstOrDefaultAsync(u => u.Id == id);

                if (url == null)
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(url);
            }
            else if (req.Method == HttpMethods.Put)
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var url = JsonConvert.DeserializeObject<Url>(requestBody);
                url.Id = id;
                var existingUrl = await _context.Url.FirstOrDefaultAsync(u => u.Id == id);

                if (existingUrl == null)
                {
                    return new NotFoundResult();
                }

                existingUrl.Status = url.Status;

                _context.Update(existingUrl);

                await _context.SaveChangesAsync();
                return new OkObjectResult(existingUrl);
            }
            else if (req.Method == HttpMethods.Delete)
            {
                var url = await _context.Url.FirstOrDefaultAsync(u => u.Id == id);

                if (url == null)
                {
                    return new NotFoundResult();
                }

                _context.Url.Remove(url);
                await _context.SaveChangesAsync();
                return new OkResult();
            }

            return new BadRequestResult();
        }

    }
}
