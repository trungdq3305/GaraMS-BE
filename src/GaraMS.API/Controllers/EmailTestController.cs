using GaraMS.Service.Services.Email;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GaraMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailTestController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public EmailTestController(IEmailService emailService, IConfiguration configuration)
        {
            _emailService = emailService;
            _configuration = configuration;
        }

        [HttpGet("test")]
        public async Task<IActionResult> TestEmail(string to)
        {
            if (string.IsNullOrEmpty(to))
            {
                return BadRequest("Email address is required");
            }

            try
            {
                // Generate a random 6-digit confirmation code
                Random random = new Random();
                string confirmationCode = random.Next(100000, 999999).ToString();
                
                string subject = "Test Confirmation Code - GaraMS";
                string body = $@"
                    <h2>Test Confirmation Code</h2>
                    <p>Hello,</p>
                    <p>Your confirmation code is:</p>
                    <h1 style='text-align: center; font-size: 32px; letter-spacing: 5px; padding: 10px; background-color: #f0f0f0; border-radius: 5px;'>{confirmationCode}</h1>
                    <p>Please enter this code in the application to confirm your account.</p>
                    <p>This is a test email. The code will not actually work.</p>
                    <p>Thank you,<br>GaraMS Team</p>
                ";
                
                await _emailService.SendEmailAsync(to, subject, body);
                
                return Ok(new { 
                    message = "Test confirmation email sent successfully",
                    code = confirmationCode // Only included for testing purposes
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to send email: {ex.Message}");
            }
        }
    }
}