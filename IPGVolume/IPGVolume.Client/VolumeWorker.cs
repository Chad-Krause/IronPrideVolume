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
        private readonly HubConnection m_volumeHub;

        public VolumeWorker(ILogger<VolumeWorker> logger, IOptionsMonitor<SignalrClientOptions> optionsDelegate)
        {
            m_logger = logger;
            m_optionClientKey = optionsDelegate;
            m_optionClientKey.OnChange(ClientKeyChanged);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                m_logger.LogInformation("VolumeWorker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void ClientKeyChanged(SignalrClientOptions newOptions)
        {
        }

        private float GetVolume()
        {
            MMDevice device = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            return device.AudioEndpointVolume.MasterVolumeLevelScalar;
        }

        private void SetVolume(float level)
        {
            if(level < 0 || level > 1)
            {
                throw new ArgumentException($"Volume out of range - {level}");
            }

            MMDevice device = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            device.AudioEndpointVolume.MasterVolumeLevelScalar = level;
        }
    }
}