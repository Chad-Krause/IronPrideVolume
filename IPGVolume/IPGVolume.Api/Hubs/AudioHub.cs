using IPGVolume.Api.Models;
using Microsoft.AspNetCore.SignalR;
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
        public AudioHub(ILogger<AudioHub> logger)
        {
            m_logger = logger;
        }

        public async Task SubscribeToVolumeReports(string remoteName, string clientKey)
        {
            m_logger.LogInformation($"Adding {remoteName} to {clientKey} reports group");
            await Groups.AddToGroupAsync(Context.ConnectionId, clientKey + "_report");
        }

        public async Task UnsubscribeToVolumeReports(string remoteName, string clientKey)
        {
            m_logger.LogInformation($"Removing {remoteName} from {clientKey} reports group");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, clientKey + "_report");
        }

        public async Task SubscribeToVolumeAdjustments(string clientKey)
        {
            m_logger.LogInformation($"Adding {clientKey} as a client");
            await Groups.AddToGroupAsync(Context.ConnectionId, clientKey);
        }

        public async Task UnsubscribeToVolumeAdjustments(string clientKey)
        {
            m_logger.LogInformation($"Removing {clientKey} from clients");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, clientKey);
        }

        public async Task ReportVolume(string clientKey, float currentVolume)
        {
            await Clients.Group(clientKey + "_reports").ReportVolumeLevel(currentVolume);
        }

        public async Task SetVolume(string clientKey, float currentVolume)
        {
            await Clients.Group(clientKey).SetVolume(currentVolume);
        }
    }
}
