using Microsoft.EntityFrameworkCore;
using Team1.Infrastructure.Dtos;
using Team1.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Team1.Infrastructure.Services.Addresses
{
    public class AddressesGet:BaseService
    {
        public AddressesGet()
        {
        }

        /// <summary>
        /// Gets all addresses for user or company
        /// </summary>
        public async Task<IEnumerable<AddressDto>> GetUserAddresses(int userId)
        {
            var query = db.AddressesByFilter(UserPermissionService, true).Where(w=>w.UserId == userId);
            
            return await ReturnAddressDto(query).ToListAsync();
        }

        private IQueryable<AddressDto> ReturnAddressDto(IQueryable<Address> query)
        {
            return (from x in query
                    select new AddressDto()
                    {
                        AddressId = x.AddressId,
                        AddressObj = new AddressObjDto()
                        {
                            Address1 = x.AddressObj.Address1,
                            Address2 = x.AddressObj.Address2,
                            Address3 = x.AddressObj.Address3,
                            City = x.AddressObj.City,
                            CountryId = x.AddressObj.CountryId,
                            Country = new CountryDto()
                            {
                                Alpha2 = x.AddressObj.Country.Alpha2,
                                Alpha3 = x.AddressObj.Country.Alpha3,
                                CountryId = x.AddressObj.Country.CountryId,
                                GoverningDistrictName = x.AddressObj.Country.GoverningDistrictName,
                                Name = x.AddressObj.Country.Name,
                                PostalCodeMask = x.AddressObj.Country.PostalCodeMask,
                            },
                            GoverningDistrictId = x.AddressObj.GoverningDistrictId,
                            PostalCode = x.AddressObj.PostalCode,
                        },
                        UserId = x.UserId,
                        IsActive = !x.AuditFields.InactiveDateTime.HasValue,
                    });
        }
    }
}
