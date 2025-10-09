using EmailApi.Biz;
using EmailApi.DTO;
using Microsoft.AspNetCore.Mvc;
using OperationLibrary.Entity;
using OperationLibrary.Operation.DbContext;

namespace EmailApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly EmailBiz _biz;
        private readonly EmailApiDbContext _context;

        public EmailController(EmailBiz biz, EmailApiDbContext context)
        {
            _biz = biz;
            _context = context;
        }

        [HttpGet("schedule")]
        public async Task<IActionResult> Schedule()
        {
            try
            {
                var logs = _context.EmailLogs
                    .Where(d => d.SendTime == null)
                    .OrderBy(d => d.CreateTime)
                    .Take(5)
                    .ToList();

                foreach (var log in logs)
                {
                    await _biz.SendEmailAsync(log.To, log.Cc, log.Bcc, log.Subject, log.Body);

                    log.SendTime = DateTime.Now;

                    _context.EmailLogs.Update(log);
                    _context.SaveChanges();
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.ToString() });
            }
        }

        [HttpPost("send")]
        public IActionResult Send([FromBody] EmailDTO request)
        {
            try
            {
                _context.EmailLogs.Add(new EmailLog
                {
                    To = request.To,
                    Cc = request.Cc,
                    Bcc = request.Bcc,
                    Subject = request.Subject,
                    Body = request.Body,
                    CreateTime = DateTime.Now
                });

                _context.SaveChanges();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.ToString() });
            }
        }
    }
}
