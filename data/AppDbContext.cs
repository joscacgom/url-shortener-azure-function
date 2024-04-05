using Microsoft.EntityFrameworkCore;
using UrlShortener.Function.Models;

namespace UrlShortener.Function.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Url> Url { get; set; }


    }
}