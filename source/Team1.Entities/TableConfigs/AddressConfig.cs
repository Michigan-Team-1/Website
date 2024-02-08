using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Team1.Model;

namespace Team1.Entities.TableConfigs;

public class AddressConfig : IEntityTypeConfiguration<Address>
{
  public void Configure(EntityTypeBuilder<Address> builder)
  {
    builder.OwnsOne(o => o.AddressObj);
  }
}
