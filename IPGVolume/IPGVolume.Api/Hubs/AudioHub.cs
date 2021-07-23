using IPGVolume.Api.Models;
using IPGVolume.Api.Models.Database;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPGVolume.Api.Hubs
{
    public class AudioHub: Hub<IAudioHubClient>
    {
        private readonly ILogger<AudioHub> m_logger;
        private readonly IPGContext m_db;
        public AudioHub(ILogger<AudioHub> logger, IPGContext db)
        {
            m_logger = logger;
            m_db = db;
        }

        public override async Task<Task> OnConnectedAsync()
        {
            try
            {
                await m_db.Client.AddAsync(new Client
                {
                    Id = Context.ConnectionId,
                    ConnectedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
                await m_db.SaveChangesAsync();

            } catch(Exception e)
            {
                m_logger.LogError(e, $"Error saving client {Context.ConnectionId} to DB!");
            }
            return base.OnConnectedAsync();
        }

        public override async Task<Task> OnDisconnectedAsync(Exception exception)
        {
            try
            {
                Client client = await m_db.Client.SingleAsync(i => i.Id == Context.ConnectionId);
                client.DisconnectedAt = DateTime.Now;
                client.UpdatedAt = DateTime.Now;
                await m_db.SaveChangesAsync();

            } catch (Exception e)
            {
                m_logger.LogError(e, $"Error saving client {Context.ConnectionId} to DB!");
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SubscribeToVolumeReports(string remoteName, string clientKey)
        {
            m_logger.LogInformation($"Adding {remoteName} to {clientKey} reports group");

            try
            {
                Client client = await m_db.Client.SingleAsync(i => i.Id == Context.ConnectionId);
                client.IsHost = false;
                client.ClientKey = clientKey;
                client.Name = remoteName;
                client.UpdatedAt = DateTime.Now;
                await m_db.SaveChangesAsync();
            } catch (Exception e)
            {
                m_logger.LogError($"SubscribeToVolumeReports: Error updating details for {Context.ConnectionId} in the database");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, clientKey + "_report");
        }

        public async Task UnsubscribeFromVolumeReports(string remoteName, string clientKey)
        {
            m_logger.LogInformation($"Removing {remoteName} from {clientKey} reports group");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, clientKey + "_report");
        }

        public async Task SubscribeToVolumeAdjustments(string clientKey)
        {
            m_logger.LogInformation($"Adding {clientKey} as a client");
            
            try
            {
                // Update database's client information
                Client client = await m_db.Client.SingleAsync(i => i.Id == Context.ConnectionId);
                client.IsHost = true;
                client.ClientKey = clientKey;
                client.UpdatedAt = DateTime.Now;
                await m_db.SaveChangesAsync();
            } catch (Exception e)
            {
                m_logger.LogError($"SubscribeToVolumeAdjustments: Error updating details for {Context.ConnectionId} in the database");
            }


            await Groups.AddToGroupAsync(Context.ConnectionId, clientKey);
        }

        public async Task UnsubscribeToVolumeAdjustments(string clientKey)
        {
            m_logger.LogInformation($"Removing {clientKey} from clients");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, clientKey);
        }

        public async Task ReportVolume(string clientKey, float currentVolume)
        {
            m_logger.LogInformation($"Client {clientKey} reported a volume of: {currentVolume}");
            
            try
            {
                Client client = await m_db.Client.SingleAsync(i => i.Id == Context.ConnectionId);
                client.LastReportedVolume = currentVolume;
                client.UpdatedAt = DateTime.Now;
                await m_db.SaveChangesAsync();
            } catch (Exception e)
            {
                m_logger.LogError($"ReportVolume: Error updating details for {Context.ConnectionId} in the database");
            }

            await Clients.Group(clientKey + "_report").ReportVolumeLevel(currentVolume);
        }

        public async Task SetVolume(string clientKey, float currentVolume)
        {
            await Clients.Group(clientKey).SetVolume(currentVolume);
        }

        public async Task ChangeKey(string oldKey, string newKey)
        {
            m_logger.LogInformation($"Changing key from {oldKey} to {newKey}");

            try
            {
                Client client = await m_db.Client.SingleAsync(i => i.Id == Context.ConnectionId);
                client.ClientKey = newKey;
                client.UpdatedAt = DateTime.Now;
                await m_db.SaveChangesAsync();
            } catch(Exception e)
            {
                m_logger.LogError($"ChangeKey: Error updating details for {Context.ConnectionId} in the database");
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, oldKey);
            await Groups.AddToGroupAsync(Context.ConnectionId, newKey);
        }
    }
}
