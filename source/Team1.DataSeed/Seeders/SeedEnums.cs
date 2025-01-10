using Microsoft.EntityFrameworkCore;
using Team1.Entities;
using Team1.Model;
using Team1.Model.Enums;

namespace Team1.DataSeed.Seeders
{
  public class SeedEnums : SeedBase
    {
        public SeedEnums(DataContext context) : base(context) { }

        public override async System.Threading.Tasks.Task Seed()
        {
            await MemberTypes();
            await LogTypes();
        }

        private async System.Threading.Tasks.Task MemberTypes()
        {
            var enumValues = MemberTypeEnum.Prefect.GetList();
            var values = await _context.MemberTypes.ToListAsync();
            foreach (var item in enumValues)
            {
                var dbObj = values.FirstOrDefault(w => w.MemberTypeId == item);
                if (dbObj == null)
                {
                    dbObj = new MemberType()
                    {
                        MemberTypeId = item
                    };
                    _context.MemberTypes.Add(dbObj);
                }

                var displayName = item.GetDisplayName();
                if (dbObj.Name != displayName)
                    dbObj.Name = displayName;
            }

            await _context.SaveChangesAsync();
        }

        private async System.Threading.Tasks.Task LogTypes()
        {
            var enumValues = LogTypeEnum.Address.GetList();
            var values = await _context.LogTypes.ToListAsync();
            foreach (var item in enumValues)
            {
                var dbObj = values.FirstOrDefault(w => w.LogTypeId == item);
                if (dbObj == null)
                {
                    dbObj = new LogType()
                    {
                        LogTypeId = item
                    };
                    _context.LogTypes.Add(dbObj);
                }

                var displayName = item.GetDisplayName();
                if (dbObj.Name != displayName)
                    dbObj.Name = displayName;
            }

            await _context.SaveChangesAsync();
        }
    }
}
