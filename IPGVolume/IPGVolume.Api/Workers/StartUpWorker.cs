using IPGVolume.Api.Models.Database;
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
    public class StartUpWorker : BackgroundService
    {
        // This class is made to run things once at startup
        private IPGContext m_db;
        private readonly IServiceProvider m_serviceProvider;
        private IServiceScope m_scope;
        private readonly ILogger<StartUpWorker> m_logger;

        public StartUpWorker(ILogger<StartUpWorker> logger, IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
            m_scope = m_serviceProvider.CreateScope();
            m_db = m_scope.ServiceProvider.GetRequiredService<IPGContext>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            m_logger.LogInformation($"Setting all clients to disconnected...");

            // Any client that has a null DisconnectedAt should have it's DisconnectedAt set to provide accurate records
            List<Client> orphanedClients = await m_db.Client.Where(i => i.DisconnectedAt == null).ToListAsync(stoppingToken);
            foreach (Client orphan in orphanedClients)
            {
                orphan.DisconnectedAt = DateTime.Now;
                orphan.UpdatedAt = DateTime.Now;
            }
            await m_db.SaveChangesAsync(stoppingToken);

            m_logger.LogInformation($"Updated {orphanedClients.Count} disconnected clients.");
        }
    }
}
