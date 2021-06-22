using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPGVolume.Api.Models
{
    public interface IAudioHubClient
    {
        public Task SetVolume(float volumeLevel);
        public Task ReportVolumeLevel(float volumeLevel);
    }
}
