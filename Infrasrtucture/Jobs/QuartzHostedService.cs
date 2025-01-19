using Quartz;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

public class QuartzHostedService : IHostedService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private IScheduler _scheduler;

    public QuartzHostedService(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _scheduler = await _schedulerFactory.GetScheduler();
        await _scheduler.Start(); // Запуск планировщика
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _scheduler?.Shutdown(); // Остановка планировщика
    }
}
