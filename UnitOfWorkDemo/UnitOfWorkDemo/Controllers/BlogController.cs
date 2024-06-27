using DataAccessWithEF;
using DataAccessWithEF.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnitOfWorkDemo.Dtos;

namespace UnitOfWorkDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly UnitOfWorkDemoDbContext _unitOfWorkDemoDbContext;
        private readonly ILogger<BlogController> _logger;

        public BlogController(ILogger<BlogController> logger,
            UnitOfWorkDemoDbContext unitOfWorkDemoDbContext)
        {
            _logger = logger;
            _unitOfWorkDemoDbContext = unitOfWorkDemoDbContext;
        }

        [HttpGet("GetBlogs")]
        public async Task<IEnumerable<BlogResponseDto>> GetBlogs()
        {
            List<BlogResponseDto> blogs = await _unitOfWorkDemoDbContext
                .Blogs
                .Include(b => b.CreatedByUser)
                .Include(b => b.Comments)
                .Where(b => b.IsPublished && !string.IsNullOrWhiteSpace(b.Content))
                .OrderByDescending(b => b.PublishedDate)
                .Select(b => new BlogResponseDto()
                {
                    BlogId = b.Id,
                    CreatedByName = b.CreatedByUser.Name,
                    CreatedByEmail = b.CreatedByUser.Email,
                    PublishedOn = b.PublishedDate ?? DateTimeOffset.Now,
                    BlogContent = string.IsNullOrWhiteSpace(b.Content) ? string.Empty : b.Content,
                    BlogComments = b.Comments != null ? 
                        b.Comments
                        .Where(c => !string.IsNullOrWhiteSpace(c.CommentContent))
                        .Select(c => new BlogCommentResponseDto()
                        {
                            CommentId = c.Id,
                            ParentBlogId = b.Id,
                            CommentedByEmail = c.CommentedByUser.Name,
                            CommentedByName = c.CommentedByUser.Name,
                            CommentedOn = c.CommentedOn,
                            CommentContent = string.IsNullOrWhiteSpace(c.CommentContent) ? string.Empty : c.CommentContent,
                        })
                        .ToList() : null,
                })
                .ToListAsync();

            return blogs;
        }

        [HttpPost("CreateBlog")]
        public async Task<BlogResponseDto?> CreateBlog(BlogCreateDto blog)
        {
            DataAccessWithEF.Models.User? CreatedByUser = await _unitOfWorkDemoDbContext
                .Users
                .Where(u => u.Id == blog.CreatedById)
                .FirstOrDefaultAsync();

            BlogResponseDto? blogResponse = null;

            if (CreatedByUser is not null)
            {
                Blog createdBlog = new Blog()
                {
                    IsPublished = blog.IsPublished,
                    PublishedDate = blog.IsPublished ? DateTimeOffset.Now : null,
                    CreatedByUser = CreatedByUser,
                    CreatedTime = DateTimeOffset.Now,
                    LastEditedTime = DateTimeOffset.Now,
                    Content = blog.Content
                };

                _unitOfWorkDemoDbContext.Blogs.Add(createdBlog);

                await _unitOfWorkDemoDbContext.SaveChangesAsync();

                blogResponse = new BlogResponseDto()
                {
                    BlogId = createdBlog.Id,
                    CreatedByName = createdBlog.CreatedByUser.Name,
                    CreatedByEmail = createdBlog.CreatedByUser.Email,
                    BlogContent = createdBlog.Content,
                    PublishedOn = createdBlog.PublishedDate ?? DateTimeOffset.Now,
                };
            }

            return blogResponse;
        }
    }
}
