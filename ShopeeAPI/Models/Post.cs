namespace Shopee.API.Models
{
    public class Post
    {
        public string? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FeaturedImage { get; set; }
        public DateTime PublishDate { get; set; }
        public bool Published { get; set; }
    }
}