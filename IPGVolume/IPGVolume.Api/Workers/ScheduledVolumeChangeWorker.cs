using IPGVolume.Api.Hubs;
using IPGVolume.Api.Models;
using IPGVolume.Api.Models.Database;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IPGVolume.Api.Workers
{
    public class ScheduledVolumeChangeWorker : BackgroundService
    {
        private IPGContext m_db;
        private readonly IHubContext<AudioHub, IAudioHubClient> m_audioHub;
        private readonly IServiceProvider m_serviceProvider;
        private IServiceScope m_scope;
        private readonly ILogger<ScheduledVolumeChangeWorker> m_logger;

        public ScheduledVolumeChangeWorker(ILogger<ScheduledVolumeChangeWorker> logger, IServiceProvider serviceProvider, IHubContext<AudioHub, IAudioHubClient> audioHub)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
            m_audioHub = audioHub;
            m_scope = m_serviceProvider.CreateScope();
            m_db = m_scope.ServiceProvider.GetRequiredService<IPGContext>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var oneOffChanges = await GetOneOffChanges();

                var recurringChanges = await GetRecurringChanges();

                foreach (var change in oneOffChanges)
                {
                    m_logger.LogInformation($"One-off Change: {change.ClientKey}");
                    await m_audioHub.Clients.Groups(change.ClientKey).SetVolume(change.Setpoint);
                    change.CompletedOn = DateTime.Now;
                    m_db.Update(change);
                }

                foreach (var change in recurringChanges)
                {
                    m_logger.LogInformation($"Recurring Change: {change.ClientKey}");
                    await m_audioHub.Clients.Groups(change.ClientKey).SetVolume(change.Setpoint);

                    ScheduledVolumeChange current_instance = await m_db.ScheduledVolumeChange.SingleAsync(i => i.Id == change.Id);
                    current_instance.CompletedOn = DateTime.Now;
                    m_db.Update(current_instance);
                }

                await m_db.SaveChangesAsync();
                await Task.Delay(5000, stoppingToken);
            }
        }

        private async Task<List<ScheduledVolumeChange>> GetRecurringChanges()
        {
            return await m_db.GetRecurringChanges(DateTime.Now);
        }

        /// <summary>
        /// Get any one-off scheduled volume changes
        /// 
        /// Conditions:
        /// 1. Not recurring
        /// 2. ActiveOn <= Now
        /// 3. CompletedOn == null
        /// </summary>
        /// <returns></returns>
        private async Task<List<ScheduledVolumeChange>> GetOneOffChanges()
        {
            return await m_db.ScheduledVolumeChange
                .AsNoTracking()
                .Where(i => i.IsRecurring == false && DateTime.Now >= i.ActiveOn && i.CompletedOn == null)
                .ToListAsync();
        }
    }
}
