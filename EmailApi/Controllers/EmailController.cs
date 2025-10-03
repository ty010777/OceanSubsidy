using EmailApi.Biz;
using EmailApi.DTO;
using Microsoft.AspNetCore.Mvc;

namespace EmailApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly EmailBiz _biz;

        public EmailController(EmailBiz biz)
        {
            _biz = biz;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailDTO request)
        {
            try
            {
                await _biz.SendEmailAsync(request.To, request.Cc, request.Bcc, request.Subject, request.Body);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.ToString() });
            }
        }
    }
}
