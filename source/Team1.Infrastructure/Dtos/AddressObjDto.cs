using Team1.Model.OwnedTypes;

namespace Team1.Infrastructure.Dtos
{
    public class AddressObjDto : AddressObjBase
    {
        /// <summary>
        /// Used when you only want to display an address
        /// </summary>
        public string GoverningDistrictName { get; set; }
        /// <summary>
        /// Used when you only want to display an address
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Used when editing an address
        /// </summary>
        public CountryDto Country { get; set; }
    }
}
