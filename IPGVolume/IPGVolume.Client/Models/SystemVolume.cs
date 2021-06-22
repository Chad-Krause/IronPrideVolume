using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace IPGVolume.Client.Models
{
    public class SystemVolume
    {
        //The Unit to use when getting and setting the volume
        public enum VolumeUnit
        {
            //Perform volume action in decibels</param>
            Decibel,
            //Perform volume action in scalar
            Scalar
        }

        /// <summary>
        /// Gets the current volume
        /// </summary>
        /// <param name="vUnit">The unit to report the current volume in</param>
        [DllImport("IPGVolume.WindowsVolAPI")]
        public static extern float GetSystemVolume(VolumeUnit vUnit);
        /// <summary>
        /// sets the current volume
        /// </summary>
        /// <param name="newVolume">The new volume to set</param>
        /// <param name="vUnit">The unit to set the current volume in</param>
        [DllImport("IPGVolume.WindowsVolAPI")]
        public static extern void SetSystemVolume(double newVolume, VolumeUnit vUnit);
    }
}
