using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Team1.Infrastructure.UserIdentity;
using Team1.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Team1.Entities.TableConfigs
{
    public class AddressConfig : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.OwnsOne(o => o.AddressObj);
        }
    }
}
