using IPGVolume.Api.Models;
using IPGVolume.Api.Models.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPGVolume.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScheduledVolumeChangesController : ControllerBase
    {
        private readonly ILogger<ScheduledVolumeChangesController> m_logger;
        private readonly IPGContext m_db;


        public ScheduledVolumeChangesController(ILogger<ScheduledVolumeChangesController> logger, IPGContext db)
        {
            m_logger = logger;
            m_db = db;
        }

        [HttpGet("GetScheduledVolumeChanges")]
        public async Task<ActionResult<List<ScheduledVolumeChange>>> GetScheduledVolumeChanges(string clientKey)
        {
            m_logger.LogInformation($"GetScheduledVolumeChanges ClientKey: {clientKey} ");
            return await m_db.ScheduledVolumeChange.Include(i => i.RecurringDaysActive)
                                                    .Where(i => i.ClientKey == clientKey
                                                            && (i.IsRecurring || i.CompletedOn == null)
                                                            && (i.ExpiresOn == null || i.ExpiresOn > DateTime.Now)
                                                            && i.ActiveOn < DateTime.Now)

                                                    .ToListAsync();
        }

        [HttpPost("CreateOrUpdateSVC")]
        public async Task CreateOrUpdateSVC(CreateOrUpdateSVCModel scheduledVolumeChange)
        {
            if(scheduledVolumeChange.Id.HasValue)
            {
                throw new NotImplementedException("Updating not supported yet");
            } else
            {
                await CreateSVC(scheduledVolumeChange);
            }
        }

        private async Task CreateSVC(CreateOrUpdateSVCModel scheduledVolumeChange)
        {
            m_logger.LogInformation($"Creating new SVC");

            ScheduledVolumeChange svc = new ScheduledVolumeChange
            {
                Setpoint = scheduledVolumeChange.Setpoint,
                ClientKey = scheduledVolumeChange.ClientKey,
                CreatorName = scheduledVolumeChange.CreatorName,
                ActiveOn = scheduledVolumeChange.ActiveOn,
                IsRecurring = scheduledVolumeChange.IsRecurring,
                ExpiresOn = scheduledVolumeChange.ExpiresOn
            };
            
            foreach(int day in scheduledVolumeChange.RecurringDaysActive)
            {
                svc.RecurringDaysActive.Add(new RecurringDaysActive {
                    DayNumber = day
                });
            }

            m_db.ScheduledVolumeChange.Add(svc);
        }
    }
}
