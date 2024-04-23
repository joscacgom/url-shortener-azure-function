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
    public class UrlsGetAllCreate
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;


        public UrlsGetAllCreate(AppDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        [Function("UrlsGetAllCreate")]
        public async Task<IActionResult> Run(
                    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "urls")] HttpRequest req)
        {
            if (req.Method == HttpMethods.Post)
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var url = JsonConvert.DeserializeObject<Url>(requestBody);

                url.ShortUrl = GetShortUrl();

                _context.Url.Add(url);
                await _context.SaveChangesAsync();
                return new CreatedResult("/urls", url);
            }
            else if (req.Method == HttpMethods.Get)
            {
                var urls = await _context.Url.ToListAsync();
                return new OkObjectResult(urls);
            }
            return new BadRequestResult();
        }


        private string GetShortUrl()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            var random = new Random();
            var result = new char[6];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }

            return new string(result);
        }

    }
}
