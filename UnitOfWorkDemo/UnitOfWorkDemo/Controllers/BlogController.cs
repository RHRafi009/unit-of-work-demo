using DataAccess.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;

        public BlogController(
            ILogger<BlogController> logger,
            UnitOfWorkDemoDbContext unitOfWorkDemoDbContext,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWorkDemoDbContext = unitOfWorkDemoDbContext;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetBlogs")]
        public async Task<IEnumerable<BlogResponseDto>> GetBlogs()
        {
            IEnumerable<Blog> blogs = await _unitOfWork
                .BlogRepo
                .GetBlogsWithUserAndComments(
                    b => b.IsPublished && !string.IsNullOrWhiteSpace(b.Content),
                    blog => blog.OrderByDescending(b => b.PublishedDate));
                
            IEnumerable<BlogResponseDto> blogResponses = blogs
                .Select(b => new BlogResponseDto()
                {
                    BlogId = b.Id,
                    CreatedByName = b.CreatedByUser.Name,
                    CreatedByEmail = b.CreatedByUser.Email,
                    PublishedOn = b.PublishedDate ?? DateTimeOffset.Now,
                    BlogContent = string.IsNullOrWhiteSpace(b.Content) ? string.Empty : b.Content,
                    BlogComments = b.Comments?.Where(c => !string.IsNullOrWhiteSpace(c.CommentContent))
                        .Select(c => new BlogCommentResponseDto()
                        {
                            CommentId = c.Id,
                            ParentBlogId = b.Id,
                            CommentedByEmail = c.CommentedByUser.Email,
                            CommentedByName = c.CommentedByUser.Name,
                            CommentedOn = c.CommentedOn,
                            CommentContent = string.IsNullOrWhiteSpace(c.CommentContent) ? string.Empty : c.CommentContent,
                        })
                        .ToList(),
                });

            return blogResponses;
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

        [HttpPut("UpdateBlog")]
        public async Task<BlogResponseDto?> UpdateBlog(UpdateBlogDto updateBlog)
        {
            Blog? blog = await _unitOfWorkDemoDbContext
                .Blogs
                .Include(b => b.CreatedByUser)
                .FirstOrDefaultAsync(b => b.Id == updateBlog.BlogId);

            BlogResponseDto? blogResponse = null;

            if (blog is not null)
            {
                blog.LastEditedTime = DateTimeOffset.Now;
                blog.IsPublished = updateBlog.IsPublished;
                blog.PublishedDate = DateTimeOffset.Now;
                blog.Content = updateBlog.Content;

                _unitOfWorkDemoDbContext.Blogs.Update(blog);

                await _unitOfWorkDemoDbContext.SaveChangesAsync();

                blogResponse = new BlogResponseDto()
                {
                    BlogId = blog.Id,
                    CreatedByName = blog.CreatedByUser.Name,
                    CreatedByEmail = blog.CreatedByUser.Email,
                    PublishedOn = blog.PublishedDate ?? DateTimeOffset.Now,
                    BlogContent = blog.Content,
                    BlogComments = []
                };
            }

            return blogResponse;
        }

        [HttpDelete("DeleteBlog")]
        public async Task<bool> DeleteBlog(int blogId)
        {
            Blog? blog = await _unitOfWorkDemoDbContext
                .Blogs
                .Include(b => b.CreatedByUser)
                .FirstOrDefaultAsync(b => b.Id == blogId);

            bool result = false;

            if (blog is not null)
            {
                _unitOfWorkDemoDbContext.Blogs.Remove(blog);

                await _unitOfWorkDemoDbContext.SaveChangesAsync();

                result = true;
            }

            return result;
        }

        [HttpPost("CreateBlogComment")]
        public async Task<BlogCommentResponseDto?> CreateBlogComment(BlogCommentCreateDto blogComment)
        {
            DataAccessWithEF.Models.User? CreatedByUser = await _unitOfWorkDemoDbContext
                .Users
                .Where(u => u.Id == blogComment.CommentedById)
                .FirstOrDefaultAsync();
            Blog? blog = await _unitOfWorkDemoDbContext
                .Blogs
                .Where(b => b.Id == blogComment.BlogId)
                .FirstOrDefaultAsync();

            BlogCommentResponseDto? responseDto = null;

            if (CreatedByUser is not null &&
                blog is not null)
            {
                BlogComment createdComment = new BlogComment()
                {
                    ParentBlog = blog,
                    CreatedTime = DateTimeOffset.Now,
                    CommentedByUser = CreatedByUser,
                    LastEditedTime = DateTimeOffset.Now,
                    CommentedOn = DateTimeOffset.Now,
                    CommentContent = blogComment.Comment
                };

                _unitOfWorkDemoDbContext.BlogComments.Add(createdComment);

                await _unitOfWorkDemoDbContext.SaveChangesAsync();

                responseDto = new BlogCommentResponseDto()
                {
                    CommentId = createdComment.Id,
                    ParentBlogId = blog.Id,
                    CommentedOn = createdComment.CommentedOn,
                    CommentContent = createdComment.CommentContent,
                    CommentedByEmail = createdComment.CommentedByUser.Email,
                    CommentedByName = createdComment.CommentedByUser.Name
                };
            }

            return responseDto;
        }
    }
}
