using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPGVolume.Api.Models.Database
{
    public class IPGContext : DbContext
    {
        public IPGContext(DbContextOptions<IPGContext> options) : base(options) { }
        public DbSet<ScheduledVolumeChange> ScheduledVolumeChange { get; set; }
        public DbSet<RecurringDaysActive> RecurringDaysActive { get; set; }
    }
}
