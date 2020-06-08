using Microsoft.EntityFrameworkCore;
using Team1.Entities;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.UserIdentity;
using Team1.Model.UserIdentity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Team1.Infrastructure.Services.Countries
{
    public class CountriesGet:BaseService
    {
        public CountriesGet()
        {
        }

        /// <summary>
        /// Gets all countries
        /// </summary>
        public async Task<IEnumerable<CountryDto>> GetCountries()
        {
            return await (from x in db.Countries
                          orderby x.Name
                          select new CountryDto()
                          {
                              Name = x.Name,
                              Alpha2 = x.Alpha2,
                              Alpha3 = x.Alpha3,
                              CountryId = x.CountryId,
                              GoverningDistrictName = x.GoverningDistrictName,
                              PostalCodeMask = x.PostalCodeMask,
                          }).ToListAsync();
        }
    }
}
