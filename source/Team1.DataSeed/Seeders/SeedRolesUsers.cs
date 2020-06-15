using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Team1.Entities;
using Team1.Model;
using Team1.Model.OwnedTypes;
using Team1.Model.SerializedObjects;
using Team1.Model.UserIdentity;

namespace Team1.DataSeed.Seeders
{
    public class SeedRolesUsers : SeedBase
    {
        private List<Role> _roles = new List<Role>();

        public SeedRolesUsers(DataContext context) : base(context) { }

        public override async System.Threading.Tasks.Task Seed()
        {
            await Roles();
            await Users();
        }

        private async System.Threading.Tasks.Task Roles()
        {
            _roles.Add(new Role() { RoleId = 1, Name = Role.Admin, NormalizedName = Role.Admin.ToLower(), Type = Role.RoleTypeGeneral, Level = 1, });
            _roles.Add(new Role() { RoleId = 2, Name = Role.Registrar, NormalizedName = Role.Registrar.ToLower(), Type = Role.RoleTypeGeneral, Level = 10 });
            _roles.Add(new Role() { RoleId = 3, Name = Role.LCO, NormalizedName = Role.LCO.ToLower(), Type = Role.RoleTypeGeneral, Level = 20 });

            _roles[0].Data = new RoleData() { GrantableRoleIds = _roles.Select(s => s.RoleId).ToList() }.SerializeJsonNet();
            _roles[1].Data = new RoleData() { GrantableRoleIds = _roles.Where(w => false).Select(s => s.RoleId).ToList() }.SerializeJsonNet();
            _roles[2].Data = new RoleData() { GrantableRoleIds = _roles.Where(w => false).Select(s => s.RoleId).ToList() }.SerializeJsonNet();

            foreach (var item in _roles)
            {
                var dbObj = await _context.Roles.FirstOrDefaultAsync(w => w.RoleId == item.RoleId);
                if (dbObj == null)
                {
                    dbObj = new Role()
                    {
                        RoleId = item.RoleId
                    };
                    _context.Roles.Add(dbObj);
                }

                dbObj.Level = item.Level;
                dbObj.Name = item.Name;
                dbObj.NormalizedName = item.NormalizedName;
                dbObj.Type = item.Type;
                dbObj.Data = item.Data;
            }

            await _context.SaveChangesAsync();
        }

        private async System.Threading.Tasks.Task Users()
        {
            // role must be cloned for each use
            var adminRole = _roles.Where(w => w.Name == Role.Admin).Select(s => new UserRole() { RoleId = s.RoleId }).First();
            var registrarRole = _roles.Where(w => w.Name == Role.Registrar).Select(s => new UserRole() { RoleId = s.RoleId }).First();
            var users = new List<User>();
            users.Add(new User()
            {
                Email = "jdsmith39@gmail.com",
                FirstName = "Jeremy",
                LastName = "Smith",
                PasswordHash = "ACMVM5I16Hmp1MN1VVvPoi3qDFSgQnx2ptICSzBMVkeElBJ6DB09lV4DFDKhu/nZQQ==",
                SecurityStamp = "09e6a71b-5a4f-4c54-b9b3-2fc54f114bb8",
                PhoneNumber = "8102523799",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                EmailConfirmed = true,
                IsLoginEnabled = true,
                TripoliNumber = "12939",
                CertificationLevel = 2,
                BirthDate = new System.DateTime(1981, 7, 27),
                MobileCarrierId = 41,
                Addresses = new List<Address>()
                {
                    new Address()
                    {
                        AuditFields = new AuditFields(0, _timestamp),
                        AddressObj = new AddressObj()
                        {
                            Address1 = "2700 Steeplechase",
                            City = "Highland",
                            CountryId = 840,
                            GoverningDistrictId = 27,
                            PostalCode = "48357"
                        }
                    }
                },
                AuditFields = new AuditFields(0, _timestamp),
                UserRoles = new List<UserRole>() { adminRole.SerializeJsonNet().DeserializeJsonNet<UserRole>() },
                UserMemberTypes = new List<UserMemberType>() { new UserMemberType() { MemberTypeId = Model.Enums.MemberTypeEnum.VicePrefect } }
            });

            users.Add(new User()
            {
                Email = "bschultz32@aol.com",
                FirstName = "Bob",
                LastName = "Schultz",
                PasswordHash = "",
                SecurityStamp = "09e6a71b-5a4f-4c54-b9b3-2fc54f114bb8",
                PhoneNumber = "7346454124",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                EmailConfirmed = true,
                IsLoginEnabled = true,
                TripoliNumber = null,
                CertificationLevel = 2,
                BirthDate = new System.DateTime(1970, 1, 1),
                MobileCarrierId = 50,
                Addresses = new List<Address>()
                {
                    new Address()
                    {
                        AuditFields = new AuditFields(0, _timestamp),
                        AddressObj = new AddressObj()
                        {
                            Address1 = "asdf",
                            City = "Brighton",
                            CountryId = 840,
                            GoverningDistrictId = 27,
                            PostalCode = "48357"
                        }
                    }
                },
                AuditFields = new AuditFields(0, _timestamp),
                UserRoles = new List<UserRole>() { adminRole.SerializeJsonNet().DeserializeJsonNet<UserRole>() },
                UserMemberTypes = new List<UserMemberType>() { new UserMemberType() { MemberTypeId = Model.Enums.MemberTypeEnum.Secretary }, new UserMemberType() { MemberTypeId = Model.Enums.MemberTypeEnum.Treasurer } }
            });

            users.Add(new User()
            {
                Email = "NJnAZAROFF@FASCOMINC.COM".ToLower(),
                FirstName = "Norm",
                LastName = "Nazaroff",
                PasswordHash = "",
                SecurityStamp = "09e6a71b-5a4f-4c54-b9b3-2fc54f114bb8",
                PhoneNumber = "2482124962",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                EmailConfirmed = true,
                IsLoginEnabled = true,
                TripoliNumber = null,
                CertificationLevel = 2,
                BirthDate = new System.DateTime(1970, 1, 1),
                MobileCarrierId = 37,
                Addresses = new List<Address>()
                {
                    new Address()
                    {
                        AuditFields = new AuditFields(0, _timestamp),
                        AddressObj = new AddressObj()
                        {
                            Address1 = "blah road",
                            City = "Novi",
                            CountryId = 840,
                            GoverningDistrictId = 27,
                            PostalCode = "48357"
                        }
                    }
                },
                AuditFields = new AuditFields(0, _timestamp),
                UserRoles = new List<UserRole>() { adminRole.SerializeJsonNet().DeserializeJsonNet<UserRole>() },
                UserMemberTypes = new List<UserMemberType>() { new UserMemberType() { MemberTypeId = Model.Enums.MemberTypeEnum.Prefect } }
            });

            //users.Add(new User()
            //{
            //    Email = "test@test.com",
            //    FirstName = "test",
            //    LastName = "test",
            //    PasswordHash = "ACMVM5I16Hmp1MN1VVvPoi3qDFSgQnx2ptICSzBMVkeElBJ6DB09lV4DFDKhu/nZQQ==",
            //    SecurityStamp = "09e6a71b-5a4f-4c54-b9b3-2fc54f114bb8",
            //    PhoneNumber = "1235554321",
            //    PhoneNumberConfirmed = true,
            //    TwoFactorEnabled = false,
            //    LockoutEnabled = true,
            //    EmailConfirmed = true,
            //    IsLoginEnabled = true,
            //    OptOutOfGeneralEmails = true,
            //    OptOutOfMemberEmails = true,
            //    AuditFields = new AuditFields(0, _timestamp),
            //    UserRoles = new List<UserRole>() { registrarRole.SerializeJsonNet().DeserializeJsonNet<UserRole>() },
            //});

            for (int i = 0; i < users.Count; i++)
            {
                var item = users[i];
                item.NormalizedEmail = item.Email.ToUpper();

                var dbObj = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(w => w.Email == item.Email);
                // re-adds if missing
                if (dbObj == null)
                    _context.Users.Add(item);
                else
                    users[i] = dbObj;
            }
            await _context.SaveChangesAsync();
        }
    }
}
