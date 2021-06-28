using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace IPGVolume.Api.Models.Database
{
    public static class StoredProcedureExtensions
    {
        public static async Task<List<ScheduledVolumeChange>> GetRecurringChanges(this IPGContext context, DateTime? AsOf)
        {
            if (!AsOf.HasValue) { AsOf = DateTime.Now; }

            var datetime = new MySqlParameter("AsOf", AsOf.Value.ToString("s"));

            return await context.Set<ScheduledVolumeChange>()
                .FromSqlRaw($"CALL `GetRecurringScheduledVolumeChanges` (@AsOf)", datetime)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}