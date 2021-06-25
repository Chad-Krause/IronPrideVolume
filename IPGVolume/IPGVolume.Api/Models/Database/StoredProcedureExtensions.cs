using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPGVolume.Api.Models.Database
{
    public static class StoredProcedureExtensions
    {

        public static async Task<List<ScheduledVolumeChange>> GetRecurringChanges(this IPGContext context)
        {
            return await context.Set<ScheduledVolumeChange>()
                .FromSqlInterpolated($"CALL `GetRecurringScheduledVolumeChanges` {DateTime.Now}")
                .ToListAsync();
        }
    }
}


//SET @AsOf = '2021-06-24 19:05:00';

//SELECT*
//FROM ScheduledVolumeChange SVC
//	JOIN DayOfWeek DOW
//    	ON DOW.ScheduledVolumeChangeId = SVC.Id
//WHERE 
//	(SVC.ExpiresOn IS NULL OR SVC.ExpiresOn >= @AsOf) AND-- Not Expired
//    IsRecurring = 1 AND									  		-- Is Recurring
//    DOW.DayOfWeek = DAYOFWEEK(@AsOf) AND-- Active Today
//(DATE(CompletedOn) != DATE(@AsOf) OR CompletedOn IS NULL)   --Didn't already run today
