using Microsoft.EntityFrameworkCore;
using Team1.Entities;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.UserIdentity;
using Team1.Model.UserIdentity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Team1.Infrastructure.Dtos.Helpers;

namespace Team1.Infrastructure.Services.MobileCarriers
{
    public class MobileCarriersGet : BaseService
    {
        public MobileCarriersGet()
        {
        }

        /// <summary>
        /// Gets all mobile carriers
        /// </summary>
        public async Task<IEnumerable<SelectOptionDto<int>>> GetMobileCarriers()
        {
            return await (from x in db.MobileCarriers
                          orderby x.MobileCarrierName
                          select new SelectOptionDto<int>()
                          {
                              Text = x.MobileCarrierName,
                              Value = x.MobileCarrierId
                          }).ToListAsync();
        }
    }
}
