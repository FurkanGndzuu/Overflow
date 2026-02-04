using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestionService.Contexts;
using QuestionService.DTOs;
using QuestionService.Entities;
using QuestionService.Services;
using System.Security.Claims;
using Wolverine;

namespace QuestionService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class QuestionsController(QuestionDbContext db, IMessageBus bus, TagService tagService) : ControllerBase
    {

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Question>> CreateQuestion(CreateQuestionDto dto)
        {
            if (!await tagService.AreTagsValidAsync(dto.Tags))
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

                await bus.PublishAsync(new QuestionCreated
 (
     QuestionId: question.Id,
     Title: question.Title,
     Content: question.Context,
     Created: question.CreatedAt,
     Tags: question.TagSlugs
 ));


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
            var question = await db.Questions.Include(q => q.Answers)
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
            if (!await tagService.AreTagsValidAsync(dto.Tags))
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

            await bus.PublishAsync(new QuestionUpdated
            (
                QuestionId: question.Id,
                Title: question.Title,
                Content: question.Context,
                Tags: question.TagSlugs.ToArray()
            ));

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

            await bus.PublishAsync(new QuestionDeleted(QuestionId: question.Id));

            return NoContent();
        }

        [HttpPost("{questionId}/answers")]
        [Authorize]
        public async Task<ActionResult<Answer>> CreateAnswer(string questionId , CreateAnswerDto dto)
        {
            Question existingQuestion = await db.Questions
                 .FirstOrDefaultAsync(q => q.Id == questionId);
            if (existingQuestion is null)
            {
                return NotFound("Question not found");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var name = User.FindFirstValue("name");
            if (userId is null || name is null) return BadRequest("Invalid user information");
            Answer answer = new()
            {
                Content = dto.Content,
                UserId = userId,
                UserDisplayName = name,
                CreatedAt = DateTime.UtcNow,
                QuestionId = questionId,
                Accepted = false
            };
            await db.Answers.AddAsync(answer);
            existingQuestion.AnswerCount += 1;
            await db.SaveChangesAsync();

            await bus.PublishAsync(new UpdatedAnswerCount(questionId, existingQuestion.AnswerCount));

            return Created($"questions/{questionId}/answers/{answer.Id}", answer);
        }

        [HttpDelete("{questionId}/answers/{answerId}")]
        [Authorize]

        public async Task<ActionResult> DeleteAnswer(string questionId, string answerId)
        {
            var answer = await db.Answers
                .FirstOrDefaultAsync(a => a.Id == answerId && a.QuestionId == questionId);
            if (answer is null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (answer.UserId != userId) return Forbid();
            db.Answers.Remove(answer);
            var question = await db.Questions.FirstOrDefaultAsync(q => q.Id == questionId);
            if (question is not null)
            {
                question.AnswerCount -= 1;
            }
            await db.SaveChangesAsync();

            await bus.PublishAsync(new UpdatedAnswerCount(questionId, question!.AnswerCount));
            return NoContent();
        }

        [HttpPost("{questionId}/answers/{answerId}/accept")]
        [Authorize]
        public async Task<ActionResult> AcceptAnswer(string questionId, string answerId)
        {
            var question = await db.Questions
                .FirstOrDefaultAsync(q => q.Id == questionId);
            if (question is null) return NotFound("Question not found");
            var answer = await db.Answers
                .FirstOrDefaultAsync(a => a.Id == answerId && a.QuestionId == questionId);
            if (answer is null) return NotFound("Answer not found");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (question.AskerId != userId) return Forbid();
            answer.Accepted = true;
            question.HasAcceptedAnswer = true;
            await db.SaveChangesAsync();

            await bus.PublishAsync(new AnswerAccepted
            (
                QuestionId: questionId
               
            ));

            return NoContent();
        }
        [HttpPost("{questionId}/answers/{answerId}")]
        [Authorize]

        public async Task<ActionResult> UpdateAnswer(string questionId, string answerId, CreateAnswerDto dto)
        {
            var answer = await db.Answers
                .FirstOrDefaultAsync(a => a.Id == answerId && a.QuestionId == questionId);
            if (answer is null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (answer.UserId != userId) return Forbid();
            answer.Content = dto.Content;
            answer.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return NoContent();
        }
    }
}
