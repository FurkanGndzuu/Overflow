using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestionService.Contexts;
using QuestionService.DTOs;
using QuestionService.Entities;
using System.Security.Claims;

namespace QuestionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController(QuestionDbContext db) : ControllerBase
    {

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Question>> CreateQuestion(CreateQuestionDto dto)
        {
            var validTags = await db.Tags
                .Where(t => dto.Tags.Contains(t.Slug))
                .Select(t => t.Slug)
                .ToListAsync();
            if (validTags.Count != dto.Tags.Count)
                return BadRequest("One or more tags are invalid");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var name = User.FindFirstValue("name");

            if (userId is null || name is null) return BadRequest("Invalid user information");
            Question question = new()
            {
                Title = dto.Title,
                Context = dto.Content,
                TagSlugs = dto.Tags,
                AskerId = userId,
                AskerName = name,
                CreatedAt = DateTime.UtcNow,
            };

            try
            {
                await db.Questions.AddAsync(question);
                await db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return Created($"questions/{question.Id}", question);
        }

        [HttpGet]
        public async Task<ActionResult<List<Question>>> GetQuestions(string? tag)
        {
            var query = db.Questions.AsQueryable();

            query = query.OrderByDescending(x => x.CreatedAt);

            if (string.IsNullOrEmpty(tag) is false)
            {
                query = query.Where(q => q.TagSlugs.Contains(tag));
            }

            var result = await query.ToListAsync();

            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Question>> GetQuestion(string id)
        {
            var question = await db.Questions
                .FirstOrDefaultAsync(x => x.Id == id);

            if (question is null) return NotFound();

            await db.Questions
                .Where(q => q.Id == id)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(q => q.Views, q => q.Views + 1));

            return question;
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateQuestion(string id, CreateQuestionDto dto)
        {
            var validTags = await db.Tags
               .Where(t => dto.Tags.Contains(t.Slug))
               .Select(t => t.Slug)
               .ToListAsync();
            if (validTags.Count != dto.Tags.Count)
                return BadRequest("One or more tags are invalid");

            var question = await db.Questions
                .FirstOrDefaultAsync(x => x.Id == id);
            if (question is null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (question.AskerId != userId) return Forbid();
            question.Title = dto.Title;
            question.Context = dto.Content;
            question.TagSlugs = dto.Tags;
            question.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuestion(string id)
        {
            var question = await db.Questions
                .FirstOrDefaultAsync(x => x.Id == id);
            if (question is null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (question.AskerId != userId) return Forbid();
            db.Questions.Remove(question);
            await db.SaveChangesAsync();
            return NoContent();
        }
    }
}
