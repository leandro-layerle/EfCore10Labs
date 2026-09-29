using ExecuteUpdateJson.Api.Data;
using ExecuteUpdateJson.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExecuteUpdateJson.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BlogsController : ControllerBase
{
    private readonly AppDbContext _context;

    public BlogsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        if (await _context.Blogs.AnyAsync())
        {
            return Ok(new
            {
                message = "Los datos de prueba ya existen."
            });
        }

        _context.Blogs.AddRange(
            CreateSeedBlogs());

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Blogs creados."
        });
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        var blogs =
            await _context.Blogs
                .ToListAsync();

        _context.Blogs.RemoveRange(blogs);

        await _context.SaveChangesAsync();

        _context.Blogs.AddRange(
            CreateSeedBlogs());

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Datos reiniciados."
        });
    }

    [HttpGet]
    public async Task<ActionResult<List<Blog>>> Get()
    {
        var blogs =
            await _context.Blogs
                .AsNoTracking()
                .OrderBy(blog => blog.Id)
                .ToListAsync();

        return Ok(blogs);
    }

    [HttpPost("savechanges/increment-views")]
    public async Task<IActionResult> IncrementViewsWithSaveChanges(
        [FromQuery] string category = ".NET")
    {
        var blogs =
            await _context.Blogs
                .Where(blog =>
                    blog.Details.Category == category)
                .OrderBy(blog => blog.Id)
                .ToListAsync();

        foreach (var blog in blogs)
        {
            blog.Details.Views++;
        }

        var trackedEntries =
            _context.ChangeTracker
                .Entries<Blog>()
                .Count();

        var affectedRows =
            await _context.SaveChangesAsync();

        return Ok(new
        {
            category,
            loadedBlogs = blogs.Count,
            trackedEntries,
            affectedRows
        });
    }

    [HttpPost("executeupdate/set-views")]
    public async Task<IActionResult> SetViewsWithExecuteUpdate(
        [FromQuery] string category = ".NET",
        [FromQuery] int views = 100)
    {
        var affectedRows =
            await _context.Blogs
                .Where(blog =>
                    blog.Details.Category == category)
                .ExecuteUpdateAsync(
                    setters =>
                        setters.SetProperty(
                            blog => blog.Details.Views,
                            views));

        return Ok(new
        {
            category,
            views,
            affectedRows
        });
    }

    [HttpPost("executeupdate/increment-views")]
    public async Task<IActionResult> IncrementViewsWithExecuteUpdate(
        [FromQuery] string category = ".NET")
    {
        var affectedRows =
            await _context.Blogs
                .Where(blog =>
                    blog.Details.Category == category)
                .ExecuteUpdateAsync(
                    setters =>
                        setters.SetProperty(
                            blog => blog.Details.Views,
                            blog =>
                                blog.Details.Views + 1));

        return Ok(new
        {
            category,
            affectedRows
        });
    }

    [HttpPost("executeupdate/feature-category")]
    public async Task<IActionResult> FeatureCategory(
        [FromQuery] string category = ".NET")
    {
        var affectedRows =
            await _context.Blogs
                .Where(blog =>
                    blog.Details.Category == category)
                .ExecuteUpdateAsync(
                    setters =>
                    {
                        setters.SetProperty(
                            blog => blog.Details.Featured,
                            true);

                        setters.SetProperty(
                            blog => blog.Details.Views,
                            500);
                    });

        return Ok(new
        {
            category,
            affectedRows
        });
    }

    [HttpPost("change-tracker-demo/{id:int}")]
    public async Task<IActionResult> ChangeTrackerDemo(
        int id)
    {
        var blog =
            await _context.Blogs
                .SingleAsync(
                    currentBlog =>
                        currentBlog.Id == id);

        var before =
            blog.Details.Views;

        var newValue =
            before + 100;

        var affectedRows =
            await _context.Blogs
                .Where(currentBlog =>
                    currentBlog.Id == id)
                .ExecuteUpdateAsync(
                    setters =>
                        setters.SetProperty(
                            currentBlog =>
                                currentBlog.Details.Views,
                            newValue));

        var inMemoryAfterExecuteUpdate =
            blog.Details.Views;

        await _context.Entry(blog)
            .ReloadAsync();

        var afterReload =
            blog.Details.Views;

        return Ok(new
        {
            affectedRows,
            before,
            inMemoryAfterExecuteUpdate,
            afterReload
        });
    }

    private static Blog[] CreateSeedBlogs()
    {
        return
        [
            new Blog
            {
                Name = "Architecture Notes",

                Details = new BlogDetails
                {
                    Title = "EF Core Performance",
                    Category = ".NET",
                    Views = 10,
                    Featured = false
                }
            },

            new Blog
            {
                Name = "Cloud Journal",

                Details = new BlogDetails
                {
                    Title = "Azure Architecture",
                    Category = "Cloud",
                    Views = 25,
                    Featured = true
                }
            },

            new Blog
            {
                Name = "Legacy Notes",

                Details = new BlogDetails
                {
                    Title = "EF Core Migration Notes",
                    Category = ".NET",
                    Views = 3,
                    Featured = false
                }
            }
        ];
    }
}
