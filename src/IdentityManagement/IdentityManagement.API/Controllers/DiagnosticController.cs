using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosticsTestController : ControllerBase
    {
        private static readonly List<byte[]> LeakyMemoryBuffer = new();

        [HttpPost("leak-memory")]
        public IActionResult LeakMemory([FromQuery] int megabytes = 50)
        {
            var chunk = new byte[megabytes * 1024 * 1024];
            new Random().NextBytes(chunk); 
            LeakyMemoryBuffer.Add(chunk);

            return Ok(new
            {
                AllocatedMB = megabytes,
                TotalChunks = LeakyMemoryBuffer.Count
            });
        }

        [HttpPost("high-cpu")]
        public IActionResult HighCpu([FromQuery] int seconds = 10)
        {
            var endTime = DateTime.UtcNow.AddSeconds(seconds);

            Parallel.For(0, Environment.ProcessorCount, _ =>
            {
                while (DateTime.UtcNow < endTime)
                {
                    _ = (int)(Math.Sqrt(new Random().NextDouble()) * Math.Sin(new Random().NextDouble()));
                }
            });

            return Ok($"High CPU load executed for {seconds} seconds across all cores.");
        }
    }
}
