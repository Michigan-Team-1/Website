using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Team1.Infrastructure.UserIdentity;
using Team1.Model.UserIdentity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Team1.Entities.TableConfigs.UserIdentity
{
    public class UserTokenConfig : IEntityTypeConfiguration<UserToken>
    {
        public void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.HasKey(x => new { x.UserId, x.LoginProvider, x.Name });
        }
    }
}
