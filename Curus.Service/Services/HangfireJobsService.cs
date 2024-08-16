using Curus.Service.Interfaces;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Curus.Service.Services;

public class HangfireJobsService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public HangfireJobsService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        RecurringJob.AddOrUpdate("DeleteInactiveUsers", () => DeleteInactiveUsers(), Cron.Daily);
        RecurringJob.AddOrUpdate("SendReminders", () => SendReminders(), Cron.Daily);
        return Task.CompletedTask;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task SendReminders() 
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            await userService.SendRemindersAsync();
        }
        Console.WriteLine("Reminder emails sent");
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task DeleteInactiveUsers()
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            await userService.DeleteInactiveUsersAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}