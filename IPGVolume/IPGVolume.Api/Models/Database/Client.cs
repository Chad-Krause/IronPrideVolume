using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPGVolume.Api.Models.Database
{
    public class Client
    {
        public string Id { get; set; }
        public bool IsHost { get; set; }
        public string ClientKey { get; set; }
        public string Name { get; set; }
        public float? LastReportedVolume { get; set; }
        public DateTime ConnectedAt { get; set; }
        public DateTime? DisconnectedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }

    public partial class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> entity)
        {
            entity.HasKey(i => i.Id);
        }
    }
}
