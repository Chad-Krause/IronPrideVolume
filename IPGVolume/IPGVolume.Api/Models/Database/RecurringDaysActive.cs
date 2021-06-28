using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPGVolume.Api.Models.Database
{
    public class RecurringDaysActive
    {
        public int Id { get; set; }
        public int ScheduledVolumeChangeId { get; set; }
        public int DayNumber { get; set; }

        public ScheduledVolumeChange ScheduledVolumeChange { get; set; }
    }
}