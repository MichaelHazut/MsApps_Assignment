using Quartz.Spi;
using Quartz;

namespace ServerApp.Services.JobSchedule
{
    public class QuartzHostedService : IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly IEnumerable<JobSchedule> _jobSchedules;
        private IScheduler? _scheduler;
        private readonly ILogger<QuartzHostedService> _logger;

        public QuartzHostedService(
            ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory,
            IEnumerable<JobSchedule> jobSchedules,
            ILogger<QuartzHostedService> logger)
        {
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
            _jobSchedules = jobSchedules;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
                _scheduler.JobFactory = _jobFactory;

                foreach (var jobSchedule in _jobSchedules)
                {
                    var job = CreateJob(jobSchedule);
                    var trigger = CreateTrigger(jobSchedule);

                    await _scheduler.ScheduleJob(job, trigger, cancellationToken);
                    _logger.LogInformation($"Scheduled job {jobSchedule.JobType.Name} with cron expression {jobSchedule.CronExpression}");
                }

                await _scheduler.Start(cancellationToken);
                _logger.LogInformation("Quartz Hosted Service started.");
            }
            catch (SchedulerException ex)
            {
                _logger.LogError(ex, "Error starting Quartz Hosted Service");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error starting Quartz Hosted Service");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_scheduler != null)
            {
                await _scheduler.Shutdown(cancellationToken);
                _logger.LogInformation("Quartz Hosted Service stopped.");
            }
        }

        private static IJobDetail CreateJob(JobSchedule schedule)
        {
            var jobType = schedule.JobType;
            return JobBuilder
                .Create(jobType)
                .WithIdentity(jobType.FullName!)
                .WithDescription(jobType.Name)
                .Build();
        }

        private static ITrigger CreateTrigger(JobSchedule schedule)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity($"{schedule.JobType.FullName}.trigger")
                .WithCronSchedule(schedule.CronExpression)
                .WithDescription(schedule.CronExpression)
                .Build();
        }
    }


}
