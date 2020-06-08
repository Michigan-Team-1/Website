using System;
using System.Collections.Generic;
using Team1.Entities;
using Team1.Model;
using Team1.Model.OwnedTypes;
using Task = System.Threading.Tasks.Task;

namespace Team1.DataSeed.Seeders
{
    public class SeedAdminData : SeedBase
    {
        public List<Location> _locations;

        public SeedAdminData(DataContext context) : base(context) { }

        public override async Task Seed()
        {
            await SeedLocations();
            await SeedEvents();
        }

        public async Task SeedLocations()
        {
            _locations = new List<Location>();

            _locations.Add(new Location()
            {
                AddressObj = new AddressObj()
                {
                    Address1 = "9419 N Webster Rd",
                    City = "Clio",
                    GoverningDistrictId = 27,
                    CountryId = 840,
                    PostalCode = "48420"
                },
                LocationDescription = "Launch Location",
                LocationName = "Alkay Airport",
                FAAWaiver = "6,000 ft",
                AuditFields = new AuditFields(0, _timestamp),
            });
            _locations.Add(new Location()
            {
                AddressObj = new AddressObj()
                {
                    Address1 = "W Fry Rd & Bishop Rd",
                    City = "Albee Township",
                    GoverningDistrictId = 27,
                    CountryId = 840,
                    PostalCode = "48655"
                },
                FAAWaiver = "17,999 ft",
                LocationDescription = "Launch Location",
                LocationName = "Birch Run Field",
                AuditFields = new AuditFields(0, _timestamp),
            });

            _context.Locations.AddRange(_locations);

            await _context.SaveChangesAsync();
        }

        public async Task SeedEvents()
        {
            //_context.Events.Add(new Event()
            //{
            //    AuditFields = new AuditFields(0, _timestamp),
            //    EventDate = new DateTime(2019, 1, 20),
            //    Name = "Annual Meeting",
            //    EventLocations = new List<EventLocation>() { new EventLocation() { LocationId = _locations[0].LocationId } }
            //});

            await _context.SaveChangesAsync();
        }
    }
}
