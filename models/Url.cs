using UrlShortener.Function.SD;

namespace UrlShortener.Function.Models
{
    public class Url
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; } = "";
        public string ShortUrl { get; set; } = "";

        public int Clicks { get; set; } = 0;

        public string UserId { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Status Status { get; set; } = Status.Active;
    }
}