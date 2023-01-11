using Hangfire.API.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.ScheduleJob;

namespace Hangfire.API.Controllers;

[ApiController]
[Route("api/schedule-job")]
public class ScheduleJobController : ControllerBase
{
    private readonly IBackgroundJobService _backgroundJobService;
    
    public ScheduleJobController(IBackgroundJobService backgroundJobService)
    {
        _backgroundJobService = backgroundJobService;
    }

    [HttpPost("send-email")]
    public IActionResult SendEmail([FromBody] ReminderCheckoutOrderDto dto)
    {
        var jobId = _backgroundJobService.SendEmailContent(dto.email, dto.subject, dto.content, dto.enqueue);
        return Ok(jobId);
    }
}