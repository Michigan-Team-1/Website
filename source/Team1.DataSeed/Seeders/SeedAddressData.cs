using Microsoft.EntityFrameworkCore;
using Team1.Entities;
using Team1.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Team1.DataSeed.Seeders
{
    public class SeedAddressData : SeedBase
    {
        public SeedAddressData(DataContext context) : base(context) { }

        public override async System.Threading.Tasks.Task Seed()
        {
            await CountriesAndGoverningDistricts();
        }

        private async System.Threading.Tasks.Task CountriesAndGoverningDistricts()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Team1.DataSeed.SeedData.Countries.csv";

            var countries = new List<Country>();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                var line = reader.ReadLine(); // skip header row
                line = reader.ReadLine();
                while (line != null)
                {
                    var lineArray = new List<string>();
                    var index = 0;
                    var nextIndex = line.IndexOf(',');
                    while (index != -1)
                    {
                        if (nextIndex > -1 && line.Substring(index, nextIndex - index).Count(w => w == '"') == 1)
                            nextIndex = line.IndexOf(',', nextIndex + 1);
                        else
                        {
                            if (nextIndex == -1)
                            {
                                lineArray.Add(line.Substring(index));
                                index = nextIndex;
                            }
                            else
                            {
                                lineArray.Add(line.Substring(index, nextIndex - index));
                                index = nextIndex + 1;
                                nextIndex = line.IndexOf(',', index);
                            }
                        }
                    }

                    var dbo = new Country();
                    dbo.Name = lineArray[0].Trim('"').Trim();
                    dbo.Alpha2 = lineArray[1].Trim('"').Trim();
                    dbo.Alpha3 = lineArray[2].Trim('"').Trim();
                    dbo.CountryId = int.Parse(lineArray[3]);
                    dbo.PostalCodeMask = string.IsNullOrWhiteSpace(lineArray[4]) ? null : lineArray[4].Trim('"').Trim();
                    dbo.GoverningDistrictName = string.IsNullOrWhiteSpace(lineArray[5]) ? null : lineArray[5].Trim('"').Trim();

                    countries.Add(dbo);
                    line = reader.ReadLine();
                }

                foreach (var item in countries)
                {
                    var dbObj = await _context.Countries.FirstOrDefaultAsync(w => w.CountryId == item.CountryId);
                    if (dbObj == null)
                    {
                        dbObj = new Country()
                        {
                            CountryId = item.CountryId,
                        };
                        _context.Countries.Add(dbObj);
                    }

                    dbObj.Alpha2 = item.Alpha2;
                    dbObj.Alpha3 = item.Alpha3;
                    dbObj.GoverningDistrictName = item.GoverningDistrictName;
                    dbObj.Name = item.Name;
                    dbObj.PostalCodeMask = item.PostalCodeMask;
                }
            }

            await _context.SaveChangesAsync();

            resourceName = "Team1.DataSeed.SeedData.GoverningDistricts.csv";
            var governingDistricts = new List<GoverningDistrict>();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                var line = reader.ReadLine(); // skip header row
                line = reader.ReadLine();
                while (line != null)
                {
                    var dbo = new GoverningDistrict();
                    var lineArray = line.Split(',');
                    dbo.GoverningDistrictId = int.Parse(lineArray[0]);
                    dbo.CountryId = int.Parse(lineArray[1]);
                    dbo.Name = lineArray[2].Trim('"').Trim();
                    dbo.Code = string.IsNullOrWhiteSpace(lineArray[3]) ? null : lineArray[3].Trim('"').Trim();

                    governingDistricts.Add(dbo);
                    line = reader.ReadLine();
                }

                foreach (var item in governingDistricts)
                {
                    var dbObj = await _context.GoverningDistricts.FirstOrDefaultAsync(w => w.GoverningDistrictId == item.GoverningDistrictId);
                    if (dbObj == null)
                    {
                        dbObj = new GoverningDistrict()
                        {
                            GoverningDistrictId = item.GoverningDistrictId,
                        };
                        _context.GoverningDistricts.Add(dbObj);
                    }

                    dbObj.Code = item.Code;
                    dbObj.CountryId = item.CountryId;
                    dbObj.Name = item.Name;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
