using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sentry;
public class SentryController : Controller
{
    private readonly IHub _sentryHub;

    public SentryController(IHub sentryHub) => _sentryHub = sentryHub;

    [HttpGet("/person/{id}")]
    public IActionResult Person(string id)
    {
        var childSpan = _sentryHub.GetSpan()?.StartChild("additional-work");
        try
        {
            // Mock data for demonstration purposes
            var mockPerson = new { Id = id, Name = "John Doe", Age = 30 };

            // Check if the person exists
            if (id == "123") // Replace this condition with your logic
            {
                childSpan?.Finish(SpanStatus.Ok);
                return Ok(mockPerson);
            }
            else
            {
                childSpan?.Finish(SpanStatus.NotFound);
                return NotFound(new { Message = $"Person with ID {id} not found." });
            }
        }
        catch (Exception e)
        {
            childSpan?.Finish(e);
            _sentryHub.CaptureException(e);
            return StatusCode(500, new { Message = "An error occurred while processing your request." });
        }
    }
}
