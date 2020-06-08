using Microsoft.EntityFrameworkCore;
using Team1.Entities;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.UserIdentity;
using Team1.Model.UserIdentity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Team1.Infrastructure.Services.GoverningDistricts
{
    public class GoverningDistrictsGet:BaseService
    {
        public GoverningDistrictsGet()
        {
        }

        /// <summary>
        /// Gets all GoverningDistricts
        /// </summary>
        public async Task<IEnumerable<GoverningDistrictDto>> GetGoverningDistricts()
        {
            return await (from x in db.GoverningDistricts
                          orderby x.Name
                          select new GoverningDistrictDto()
                          {
                              Name = x.Name,
                              Code = x.Code,
                              GoverningDistrictId = x.GoverningDistrictId,
                              CountryId = x.CountryId,
                          }).ToListAsync();
        }
    }
}
