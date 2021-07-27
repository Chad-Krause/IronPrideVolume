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
using System.Net.Http;
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
                try
                {
                    // If the hub is disconnected, reconnect, then call reconnect, which subscribes to volume events
                    if (m_volumeHub.State == HubConnectionState.Disconnected)
                    {
                        m_logger.LogInformation($"State: {m_volumeHub.State}");
                        await m_volumeHub.StartAsync();
                        await Reconnected();
                    }

                    // Report the volume if the hub is connected
                    if (m_volumeHub.State == HubConnectionState.Connected)
                    {
                        await m_volumeHub.SendAsync("ReportVolume", m_clientKey, GetVolume());
                    }
                } catch (HttpRequestException e)
                {
                    m_logger.LogError("Error connecting to the server. Retrying");
                } catch (Exception e)
                {
                    m_logger.LogError(e, "Unexpected error caught in ExecuteAsync in the VolumeWorker!");
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

        // Tell the server to remove you from the subscriber group, add you to the new one
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
                m_logger.LogInformation($"Volume out of range - {level * 100:F0}%");
                throw new ArgumentException($"Volume out of range - {level}");
            }

            m_logger.LogInformation($"Setting volume to {level * 100:F0}%");

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
            m_logger.LogInformation($"Connected to server");
            await Reconnected();
        }

        private async Task Reconnected()
        {
            m_connected = true;
            await m_volumeHub.SendAsync("SubscribeToVolumeAdjustments", m_clientKey);
        }

    }
}