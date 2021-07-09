using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPGVolume.Api.Models
{
    public class CreateOrUpdateSVCModel
    {
        public int? Id { get; set; }
        public float Setpoint { get; set; }
        public string CreatorName { get; set; }
        public string ClientKey { get; set; }
        public bool IsRecurring { get; set; }
        public DateTime ActiveOn { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public List<int> RecurringDaysActive { get; set; }
    }
}
