using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvcCoreUploadAndDisplayImage_Demo.Data;
using MvcCoreUploadAndDisplayImage_Demo.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

public class PaymentStatusUpdaterService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public PaymentStatusUpdaterService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var now = DateTime.Now;

                // Update records where DueDate is in the past
                var overdueRecords = context.Histories
                    .Where(h => h.DueDate < now && h.PaymentStatus == PaymentStatus.Paid)
                    .ToList();

                foreach (var record in overdueRecords)
                {
                    record.PaymentStatus = PaymentStatus.Overdue;
                }

                await context.SaveChangesAsync(stoppingToken);
            }

            // Run every minute for testing purposes
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

}
