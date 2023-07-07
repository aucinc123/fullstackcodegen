using DapperCommonScenarios.API.Models;
using DapperCommonScenarios.API.Services.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DapperCommonScenarios.API.Controller
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/blog")]
    public class BlogController : BaseController
    {
        private readonly BlogService blogService;

        public BlogController(BlogService blogService)
        {
            this.blogService = blogService;
        }

        [HttpGet("healthCheck")]
        public IActionResult GetHealth()
        {
            return Ok("Ah, Ha, Ha, Ha, Stayin' Alive! Stayin' Alive!");
        }

        [HttpGet("BlogList")]
        public async Task<IActionResult> GetBlogList()
        {
            var blogList = await blogService.GetBlogListAsync();
            return Ok(blogList);
        }

        [HttpGet("Blog/{blogId}")]
        public async Task<IActionResult> GetBlog(int blogId)
        {
            var blog = await this.blogService.GetBlogAsync(blogId);

            if (blog != null)
                return Ok(blog);
            else
                return NotFound();
        }

        [HttpPut("Blog/{blogId}")]
        public async Task<IActionResult> UpdateBlog(int blogId, Blog blog)
        {
            var doesExist = await blogService.UpdateBlogAsync(blogId, blog);

            if (doesExist)
                return Ok();
            else
                return NotFound();
        }

        [HttpGet("BlogWithComments/{blogId}")]
        public async Task<IActionResult> GetBlogWithComments(int blogId)
        {
            var blogWithComments = await blogService.GetBlogWithComments(blogId);

            if (blogWithComments != null)
                return Ok(blogWithComments);
            else
                return NotFound();
        }

        [HttpPut("BlogCommentList/{blogId}")]
        public async Task<IActionResult> SaveBlogCommentList(int blogId, List<BlogComment> blogCommentList)
        {
            var returnList = await blogService.SaveBlogCommentsListAsync(blogId, blogCommentList);
            return Ok(returnList);
        }
    }
}