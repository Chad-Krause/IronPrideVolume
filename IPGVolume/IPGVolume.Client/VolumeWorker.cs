using IPGVolume.Client.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IPGVolume.Client
{
    public class VolumeWorker : BackgroundService
    {
        private readonly ILogger<VolumeWorker> m_logger;
        private readonly IOptionsMonitor<SignalrClientOptions> m_optionClientKey;
        private HubConnection m_volumeHub;
        private string m_clientKey;
        private bool m_connected;

        public VolumeWorker(ILogger<VolumeWorker> logger, IOptionsMonitor<SignalrClientOptions> optionsDelegate)
        {
            m_logger = logger;
            m_optionClientKey = optionsDelegate;
            m_optionClientKey.OnChange(ClientKeyChanged);

            m_clientKey = m_optionClientKey.CurrentValue.ClientKey;

            m_volumeHub = new HubConnectionBuilder()
                .WithUrl(optionsDelegate.CurrentValue.Url)
                .WithAutomaticReconnect()
                .Build();

            m_volumeHub.Closed += Disconnected;
            m_volumeHub.Reconnecting += Disconnected;
            m_volumeHub.Reconnected += Reconnected;

            m_volumeHub.On<float>("SetVolume", SetVolume);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // This look essentially just keeps the connection and reports the volume
            while (!stoppingToken.IsCancellationRequested)
            {
                m_logger.LogInformation("VolumeWorker running at: {time}", DateTimeOffset.Now);

                // If the hub is disconnected, reconnect, then call reconnect, which subscribes to volume events
                if(m_volumeHub.State == HubConnectionState.Disconnected)
                {
                    m_logger.LogInformation($"State: {m_volumeHub.State}");
                    await m_volumeHub.StartAsync();
                    await Reconnected();
                }

                // Report the volume if the hub is connected
                if(m_volumeHub.State == HubConnectionState.Connected)
                {
                    await m_volumeHub.SendAsync("ReportVolume", m_clientKey, GetVolume());
                }

                // Loop forever every 5 seconds
                await Task.Delay(5000, stoppingToken);
            }
        }

        // Unsubscribe and stop the connection
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await m_volumeHub.SendAsync("UnsubscribeToVolumeAdjustments", m_clientKey);
            await m_volumeHub.StopAsync();
            await base.StopAsync(stoppingToken);
        }

        // Tell the server to remove you from the subscriber group
        private void ClientKeyChanged(SignalrClientOptions newOptions)
        {
            if(newOptions.ClientKey == m_clientKey) { return; } // If the client key doesn't change, don't do anything

            m_logger.LogInformation($"Changing key from {m_clientKey} to {newOptions.ClientKey}");
            _ = m_volumeHub.InvokeAsync("ChangeKey", m_clientKey, newOptions.ClientKey);
            m_clientKey = newOptions.ClientKey;
        }

        // Return a float of the current volume (0.0 - 1.0)
        private float GetVolume()
        {
            MMDevice device = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            return device.AudioEndpointVolume.MasterVolumeLevelScalar;
        }

        // Sets the volume
        private void SetVolume(float level)
        {
            if(level < 0 || level > 1)
            {
                throw new ArgumentException($"Volume out of range - {level}");
            }

            MMDevice device = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            device.AudioEndpointVolume.MasterVolumeLevelScalar = level;
        }
        private void SetVolume(double level)
        {
            SetVolume((float)level);
        }

        private async Task Disconnected(Exception e)
        {
            m_connected = false;
        }
        
        private async Task Reconnected(string connectionId)
        {
            await Reconnected();
        }

        private async Task Reconnected()
        {
            m_connected = true;
            await m_volumeHub.SendAsync("SubscribeToVolumeAdjustments", m_clientKey);
        }

    }
}