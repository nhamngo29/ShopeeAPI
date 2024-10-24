using Microsoft.AspNetCore.Mvc;
using Shopee.API.Models;

namespace Shopee.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private static readonly List<Post> Posts = new List<Post>
{
    new Post
    {
        Id = "2022-10-12T04:32:48.650Z",
        Title = "Mọi công việc thành đạt đều nhờ sự kiên trì và lòng say mê.",
        Description = "Nghịch cảnh là một phần của cuộc sống. Nó không thể bị kiểm soát. Cái có thể kiểm soát chính là cách chúng ta phản ứng trước nghịch cảnh.123",
        FeaturedImage = "https://images.unsplash.com/photo-1665412019489-1928d5afa5cc?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=765&q=80",
        PublishDate = new DateTime(2022, 09, 05, 11, 32, 0),
        Published = true
    },
    new Post
    {
        Id = "2022-10-12T04:33:07.847Z",
        Title = "Muốn thành công thì khao khát thành công phải lớn hơn nỗi sợ bị thất bại.",
        Description = "Bạn chớ nên bỏ cuộc khi bạn vẫn còn điều gì đó để cho đi. Không có gì là hoàn toàn bế tắc, sự việc chỉ thật sự trở nên bế tắc khi bạn thôi không cố gắng nữa.",
        FeaturedImage = "https://images.unsplash.com/photo-1656333422687-6830f720c38f?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=764&q=80",
        PublishDate = new DateTime(2022, 10, 14, 11, 33, 0),
        Published = true
    },
    new Post
    {
        Id = "2022-10-12T04:33:26.301Z",
        Title = "Mất niềm tin vào bản thân, cũng như bạn đánh mất thành công đang đợi mình",
        Description = "Ai cũng nói tương lai chúng ta luôn rộng mở, nhưng nếu không nắm bắt được hiện tại thì tương lai sẽ chẳng có gì.",
        FeaturedImage = "https://images.unsplash.com/photo-1656105544318-1cca8c4d9878?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=687&q=80",
        PublishDate = new DateTime(2022, 10, 15, 11, 33, 0),
        Published = true
    },
    new Post
    {
        Id = "2022-10-12T04:35:12.040Z",
        Title = "Nơi nào có ý chí, nơi đó có con đường.123",
        Description = "Tôi có thể chấp nhận thất bại, mọi người đều thất bại ở một việc gì đó. Nhưng tôi không chấp nhận việc không cố gắng.",
        FeaturedImage = "https://images.unsplash.com/photo-1648184217069-34f0a57791bc?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1471&q=80",
        PublishDate = new DateTime(2022, 10, 16, 11, 35, 0),
        Published = false
    },
    new Post
    {
        Id = "2024-08-23T15:18:45.477Z",
        Title = "123",
        Description = "123",
        FeaturedImage = "https://images.pexels.com/photos/3844788/pexels-photo-3844788.jpeg?auto=compress&cs=tinysrgb&dpr=1&w=500",
        PublishDate = new DateTime(2024, 08, 23, 13, 21, 0),
        Published = true
    }
};

        [HttpGet]
        public ActionResult<IEnumerable<Post>> GetPosts()
        {
            return Ok(Posts);
        }

        [HttpGet("{id}")]
        public ActionResult<Post> GetPost(string id)
        {
            var post = Posts.FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }

        [HttpPost]
        public ActionResult<Post> CreatePost(Post post)
        {
            if (post.PublishDate < DateTime.Now)
            {
                return StatusCode(422, new { publishDate = "Không được publish vào thời điểm trong quá khứ" });
            }
            post.Id = System.Guid.NewGuid().ToString();
            Posts.Add(post);
            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePost(string id, Post post)
        {
            var existingPost = Posts.FirstOrDefault(p => p.Id == id);
            if (existingPost == null)
            {
                return NotFound();
            }

            existingPost.Title = post.Title;
            existingPost.Description = post.Description;
            existingPost.FeaturedImage = post.FeaturedImage;
            existingPost.PublishDate = post.PublishDate;
            existingPost.Published = post.Published;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePost(string id)
        {
            var post = Posts.FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            Posts.Remove(post);
            return NoContent();
        }
    }
}