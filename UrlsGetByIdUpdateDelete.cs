using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UrlShortener.Function.Data;
using UrlShortener.Function.Models;

namespace UrlShortener.Function
{
    public class UrlsGetByIdUpdateDelete
    {
        private readonly AppDbContext _context;

        public UrlsGetByIdUpdateDelete(AppDbContext context)
        {
            _context = context;
        }

        [Function("UrlsGetByIdUpdateDelete")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,
             "get","put","delete", Route = "urls/{id}")] HttpRequest req
            ,int id)
        {
            if(req.Method == HttpMethods.Get){
                var url = await _context.Url.FirstOrDefaultAsync(u => u.Id == id);

                if(url == null){
                    return new NotFoundResult();
                }

                return new OkObjectResult(url);
            } else if(req.Method == HttpMethods.Put){
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var url = JsonConvert.DeserializeObject<Url>(requestBody);
                url.Id = id;
                var existingUrl = await _context.Url.FirstOrDefaultAsync(u => u.Id == id);

                if(existingUrl == null){
                    return new NotFoundResult();
                }

                existingUrl.OriginalUrl = url.OriginalUrl;
                existingUrl.ShortUrl = GetShortUrl(url.OriginalUrl);
                _context.Update(existingUrl);

                await _context.SaveChangesAsync();
                return new OkObjectResult(existingUrl);
            } else if(req.Method == HttpMethods.Delete){
                var url = await _context.Url.FirstOrDefaultAsync(u => u.Id == id);

                if(url == null){
                    return new NotFoundResult();
                }

                _context.Url.Remove(url);
                await _context.SaveChangesAsync();
                return new OkResult();
            }
           
            return new BadRequestResult();
        }

         private string GetShortUrl(string longUrl)
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