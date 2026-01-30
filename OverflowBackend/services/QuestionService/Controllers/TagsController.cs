using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestionService.Contexts;
using QuestionService.Entities;

namespace QuestionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController(QuestionDbContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Tag>>> GetTags() => await db.Tags
            .OrderByDescending(t => t.Name).ToListAsync();
    }
}
