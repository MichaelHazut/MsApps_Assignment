using Quartz.Spi;
using Quartz;

namespace ServerApp.Services.JobSchedule
{
    public class SingletonJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SingletonJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var scope = _serviceProvider.CreateScope();
            var job = scope.ServiceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;

            if (job == null)
            {
                throw new InvalidOperationException($"The job of type {bundle.JobDetail.JobType} could not be resolved.");
            }

            return job;
        }

        public void ReturnJob(IJob job) { }
    }

}
