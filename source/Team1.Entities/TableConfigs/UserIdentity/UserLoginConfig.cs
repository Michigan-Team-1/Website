using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Team1.Infrastructure.UserIdentity;
using Team1.Model.UserIdentity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Team1.Entities.TableConfigs.UserIdentity
{
    public class UserLoginConfig : IEntityTypeConfiguration<UserLogin>
    {
        public void Configure(EntityTypeBuilder<UserLogin> builder)
        {
            builder.HasKey(r => new { r.LoginProvider, r.ProviderKey });
        }
    }
}
