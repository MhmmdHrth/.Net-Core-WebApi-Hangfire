using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hangfire_webApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HangfireController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello world");
        }

        //fire & forget job
        [HttpPost]
        [Route("[action]")]
        public IActionResult Welcome()
        {
            var jobId = BackgroundJob.Enqueue(() => this.sendWelcome("Welcome to the background job"));

            return Ok($"Job Id: {jobId}. Welcome email sent to the user");
        }

        //schedule job
        [HttpPost]
        [Route("[action]/{time}")]
        public IActionResult Discount(int time)
        {
            var jobId = BackgroundJob.Schedule(() => this.sendWelcome("Welcome to the background job"), TimeSpan.FromSeconds(time));

            return Ok($"Job Id: {jobId}. Discount email will be sent in {time}");
        }

        //recurring job
        [HttpPost]
        [Route("[action]")]
        public IActionResult DatabaseUpdate()
        {
            RecurringJob.AddOrUpdate(() => Console.WriteLine("Database Update"), Cron.Minutely);

            return Ok("Database check job initiated");
        }

        //continuous job
        [HttpPost]
        [Route("[action]/{time}")]
        public IActionResult Confirm(int time)
        {
            var parentId = BackgroundJob.Schedule(() => Console.WriteLine("You asked to be unsubscribed"), TimeSpan.FromSeconds(time));
            BackgroundJob.ContinueJobWith(parentId, () => Console.WriteLine("You were unsubscribed"));

            return Ok("Confirmation job created");
        }

        [HttpGet("GetStorage/{jobId}")]
        public IActionResult GetStorage(string jobId)
        {
            IMonitoringApi jobMonitoringApi = JobStorage.Current.GetMonitoringApi();
            //var jobMonitoringApi = JobStorage.Current.GetConnection();
            var job = jobMonitoringApi.FailedJobs(0, int.MaxValue);
            return Ok();
        }

        public void sendWelcome(string message)
        {
            Console.WriteLine(message);
        }
    }
}
