/*using Entities.Interfaces;
using Entities.Models;
using Infrasrtucture.Jobs;
using Quartz;
using System.Threading.Tasks;


public class JobScheduler : IJobScheduler
{
   private readonly IScheduler _scheduler;

   public JobScheduler(ISchedulerFactory schedulerFactory)
   {
       _scheduler = schedulerFactory.GetScheduler().Result; 
   }

   public async Task ScheduleUnlockJobAsync()
   {
       var job = JobBuilder.Create<UnlockUserJob>()
           .WithIdentity("unlockAllUsersJob")  
           .Build();

       var trigger = TriggerBuilder.Create()
           .WithIdentity("dailyTrigger") 
           .WithCronSchedule("* * * * * ?")  
           .Build();

       await _scheduler.ScheduleJob(job, trigger);
   }
}




*/