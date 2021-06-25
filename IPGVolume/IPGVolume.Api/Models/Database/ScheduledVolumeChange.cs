using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPGVolume.Api.Models.Database
{
    public class ScheduledVolumeChange
    {
        public int? Id { get; set; }
        public float Setpoint { get; set; }
        public string CreatorName { get; set; }
        public string ClientKey { get; set; }
        public bool IsRecurring { get; set; }
        public DateTime ActiveOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public DateTime? CompletedOn { get; set; }

        public DbSet<DayOfWeek> DaysOfWeek { get; set; }
    }

    public partial class ScheduledVolumeChangeConfiguration : IEntityTypeConfiguration<ScheduledVolumeChange>
    {
        public void Configure(EntityTypeBuilder<ScheduledVolumeChange> entity)
        {
            entity.HasMany(i => i.DaysOfWeek).WithOne(i => i.ScheduledVolumeChange).HasForeignKey("FK_ScheduledVolumeChange_DayOfWeek");
        }
    }
}
