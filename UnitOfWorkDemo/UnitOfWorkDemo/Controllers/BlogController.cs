using DataAccess.Interfaces;
using DataAccessWithEF;
using DataAccessWithEF.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using UnitOfWorkDemo.Dtos;

namespace UnitOfWorkDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly ILogger<BlogController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public BlogController(
            ILogger<BlogController> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetBlogs")]
        [ProducesResponseType(typeof(IEnumerable<BlogResponseDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBlogs()
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

            return Ok(blogResponses);
        }

        [HttpPost("CreateBlog")]
        [ProducesResponseType(typeof(BlogResponseDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateBlog(BlogCreateDto blog)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                User? User = await _unitOfWork
                    .UserRepo
                    .GetByIdAsync(blog.CreatedById);

                BlogResponseDto? blogResponse = null;

                if (User is not null)
                {
                    Blog createdBlog = new Blog()
                    {
                        IsPublished = blog.IsPublished,
                        PublishedDate = blog.IsPublished ? DateTimeOffset.Now : null,
                        CreatedByUser = User,
                        CreatedTime = DateTimeOffset.Now,
                        LastEditedTime = DateTimeOffset.Now,
                        Content = blog.Content
                    };

                    await _unitOfWork.BlogRepo.AddAsync(createdBlog);

                    await _unitOfWork.SaveChnagesAsync();
                    await _unitOfWork.CompleteTransactionAsync();

                    blogResponse = new BlogResponseDto()
                    {
                        BlogId = createdBlog.Id,
                        CreatedByName = createdBlog.CreatedByUser.Name,
                        CreatedByEmail = createdBlog.CreatedByUser.Email,
                        BlogContent = createdBlog.Content,
                        PublishedOn = createdBlog.PublishedDate ?? DateTimeOffset.Now,
                    };
                }

                return Ok(blogResponse);
            }
            catch (Exception ex) 
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("UpdateBlog")]
        [ProducesResponseType(typeof(BlogResponseDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateBlog(UpdateBlogDto updateBlog)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                Blog? blog = await _unitOfWork
                .BlogRepo
                .GetBlogByIdWithUser(updateBlog.BlogId);

                BlogResponseDto? blogResponse = null;

                if (blog is not null)
                {
                    blog.LastEditedTime = DateTimeOffset.Now;
                    blog.IsPublished = updateBlog.IsPublished;
                    blog.PublishedDate = DateTimeOffset.Now;
                    blog.Content = updateBlog.Content;

                    _unitOfWork.BlogRepo.Update(blog);
                    await _unitOfWork.SaveChnagesAsync();
                    await _unitOfWork.CompleteTransactionAsync();

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

                return Ok(blogResponse);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("DeleteBlog")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBlog(int blogId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                Blog? blog = await _unitOfWork
                    .BlogRepo
                    .GetBlogByIdWithUser(blogId);

                bool result = false;

                if (blog is not null)
                {
                    _unitOfWork.BlogRepo.Delete(blog);
                    
                    await _unitOfWork.SaveChnagesAsync();
                    await _unitOfWork.CompleteTransactionAsync();

                    result = true;
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("CreateBlogComment")]
        [ProducesResponseType(typeof(BlogCommentResponseDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateBlogComment(BlogCommentCreateDto blogComment)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                User? CreatedByUser = await _unitOfWork
                    .UserRepo
                    .GetByIdAsync(blogComment.CommentedById);

                Blog? blog = await _unitOfWork
                    .BlogRepo
                    .GetByIdAsync(blogComment.BlogId);

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

                    await _unitOfWork.BlogCommentRepo.AddAsync(createdComment);

                    await _unitOfWork.SaveChnagesAsync();
                    await _unitOfWork.CompleteTransactionAsync();

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

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
