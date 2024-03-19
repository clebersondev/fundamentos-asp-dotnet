using BlogEF3.Data;
using BlogEF3.Models;
using BlogEF3.ViewModels;
using BlogEF3.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogEF3.Controllers;

[ApiController]
public class PostController : ControllerBase
{
    [HttpGet("v1/posts")]
    public async Task<IActionResult> GetAsync(
        [FromServices] BlogDataContext context,
        [FromQuery] int page = 0,
        [FromQuery] int pagesize = 25)
    {
        try
        {
            var count = await context.Posts.AsNoTracking().CountAsync();
            var posts = await context.Posts
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Author)
                .Select(x => new ListPostsViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Slug = x.Slug,
                    LastUpdateDate = x.LastUpdateDate,
                    Category = x.Category.Name,
                    Author = $"{x.Author.Name} ({x.Author.Email})"
                })
                .Skip(page * pagesize)
                .Take(pagesize)
                .OrderByDescending(x => x.LastUpdateDate)
                .ToListAsync();

            return Ok(new ResultViewModel<dynamic>(new
            {
                total = count,
                pagesize,
                posts
            }));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<List<Post>>("Falha interna no servidor."));
        }
    }

    [HttpGet("v1/posts/{id:int}")]
    public async Task<IActionResult> DetailsAsync(
    [FromServices] BlogDataContext context,
    [FromRoute] int id)
    {
        try
        {
            var post = await context.Posts
                .AsNoTracking()
                .Include(x => x.Author)
                .ThenInclude(x => x.Roles) //Esse ThenInclude foi só pra testar (é um subselect)
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (post == null)
                return NotFound(new ResultViewModel<Post>("Conteúdo não encontrado."));

            return Ok(new ResultViewModel<Post>(post));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResultViewModel<Post>("Falha interna no servidor."));
        }
    }

    [HttpGet("v1/posts/category/{category}")]
    public async Task<IActionResult> GetByCategoryAsync(
        [FromServices] BlogDataContext context,
        [FromRoute] string category,
        [FromQuery] int page = 0,
        [FromQuery] int pagesize = 25)
    {
        try
        {
            var count = await context.Posts.AsNoTracking().CountAsync();
            var posts = await context.Posts
                .AsNoTracking()
                .Include(x => x.Author)
                .Include(x => x.Category)
                .Where(x => x.Category.Slug == category)
                .Select(x => new ListPostsViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Slug = x.Slug,
                    LastUpdateDate = x.LastUpdateDate,
                    Category = x.Category.Name,
                    Author = $"{x.Author.Name} ({x.Author.Email})"
                })
                .Skip(page * pagesize)
                .Take(pagesize)
                .OrderByDescending(x => x.LastUpdateDate)
                .ToListAsync();

            return Ok(new ResultViewModel<dynamic>(new
            {
                total = count,
                pagesize,
                posts
            }));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<List<Post>>("Falha interna no servidor."));
        }
    }
}