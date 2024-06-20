using Quartz;

namespace ServerApp.Services.JobSchedule
{
    public class UserBatchJob : IJob
    {
        private readonly UserBatchService _batchService;
        private readonly ILogger<UserBatchJob> _logger;

        public UserBatchJob(UserBatchService batchService, ILogger<UserBatchJob> logger)
        {
            _batchService = batchService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("UserBatchJob started.");
            await _batchService.ProcessBatch();
            _logger.LogInformation("UserBatchJob completed.");
        }
    }


}
