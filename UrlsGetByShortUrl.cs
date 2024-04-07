using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UrlShortener.Function.Data;
using UrlShortener.Function.Models;
using UrlShortener.Function.SD;

namespace UrlShortener.Function
{
    public class UrlsGetByShortUrl
    {
        private readonly AppDbContext _context;

        public UrlsGetByShortUrl(AppDbContext context)
        {
            _context = context;
        }

        [Function("UrlsGetByShortUrl")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,
             "get", Route = "urlsByShortUrl/{shortUrl}")] HttpRequest req
            ,string shortUrl)
        {
            if(req.Method == HttpMethods.Get){
                var url = await _context.Url.FirstOrDefaultAsync(u => u.ShortUrl == shortUrl);

                if(url == null){
                    return new NotFoundResult();
                }else if(url.Status == Status.Inactive){
                    return new BadRequestResult();
                }

                url.Clicks++;
                _context.Update(url);
                await _context.SaveChangesAsync();

                return new OkObjectResult(url.OriginalUrl);
            }
           
            return new BadRequestResult();
        }

    }
}
