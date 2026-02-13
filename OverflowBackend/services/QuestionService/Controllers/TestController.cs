using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace QuestionService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [Authorize]
        [HttpGet("auth")]

        public ActionResult<string> GetAuth()
        {
            var user = User.FindFirstValue("name");
            return Ok($"{user} is authenticated");
        }

        [HttpGet("errors")]
        public ActionResult TriggerError(int code)
        {
            return code switch
            {
                400 => BadRequest("This is a bad request example."),
                401 => Unauthorized("This is an unauthorized example."),
                403 => Forbid("This is a forbidden example."),
                404 => NotFound("This is a not found example."),
                500 => StatusCode(500, "This is an internal server error example."),
                _ => Ok("No error triggered.")
            };
        }
    }
}
