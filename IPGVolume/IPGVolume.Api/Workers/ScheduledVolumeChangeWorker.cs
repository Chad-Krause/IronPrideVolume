using IPGVolume.Api.Hubs;
using IPGVolume.Api.Models;
using IPGVolume.Api.Models.Database;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IPGVolume.Api.Workers
{
    public class ScheduledVolumeChangeWorker : BackgroundService
    {
        private readonly IPGContext m_db;
        private readonly IHubContext<AudioHub, IAudioHubClient> m_audioHub;

        public ScheduledVolumeChangeWorker(IPGContext db, IHubContext<AudioHub, IAudioHubClient> audioHub)
        {
            m_db = db;
            m_audioHub = audioHub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                List<ScheduledVolumeChange> oneOffChanges = await GetOneOffChanges();
                List<ScheduledVolumeChange> recurringChanges = await GetRecurringChanges();

                foreach(ScheduledVolumeChange change in oneOffChanges)
                {
                    await m_audioHub.Clients.Groups(change.ClientKey).SetVolume(change.Setpoint);
                    change.CompletedOn = DateTime.Now;
                }

                foreach(ScheduledVolumeChange change in recurringChanges)
                {
                    Console.WriteLine($"Recurring Change: {change.ClientKey}");
                }

                await m_db.SaveChangesAsync();

                await Task.Delay(5000, stoppingToken);
            }
        }

        private async Task<List<ScheduledVolumeChange>> GetRecurringChanges()
        {
            return await m_db.GetRecurringChanges();
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
