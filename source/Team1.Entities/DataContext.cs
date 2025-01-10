using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Team1.Entities.TableConfigs;
using Team1.Entities.TableConfigs.UserIdentity;
using Team1.Infrastructure.UserIdentity;
using Team1.Model;
using Team1.Model.UserIdentity;

namespace Team1.Entities;

public class DataContext : DbContext, ILoggingContext
{
  public DataContext(UserPermissionService ups, DbContextOptions<DataContext> options, ILogger<DataContext> logger) : base(options)
  {
    _userPermissionService = ups;
    _logger = logger;
  }

  private ILogger<DataContext> _logger;

  public UserPermissionService _userPermissionService;

  public Guid LogGroupKey { get; set; } = Guid.NewGuid();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // configure all decimal types to have the same precision.  Override after this line if necessary
    var decimalTypes = new[] { typeof(decimal), typeof(decimal?) };
    foreach (var property in modelBuilder.Model.GetEntityTypes()
        .SelectMany(t => t.GetProperties().Where(p => decimalTypes.Contains(p.ClrType.UnderlyingSystemType))))
    {
      property.SetPrecision(18);
      property.SetScale(6);
    }

    // used to setup properties based on the dataType attribute
    foreach (var property in modelBuilder.Model.GetEntityTypes()
              .SelectMany(s => s.GetProperties().Where(w => w.PropertyInfo != null && w.PropertyInfo.CustomAttributes.Where(caw => caw.AttributeType == typeof(DataTypeAttribute)).Any()))
              )
    {
      var dataTypeAttribute = property.PropertyInfo.GetCustomAttributes(typeof(DataTypeAttribute), true).Cast<DataTypeAttribute>().First();
      switch (dataTypeAttribute.DataType)
      {
        // add other DataType values if needed
        case DataType.Date:
          property.SetColumnType("Date");
          break;
      }
    }

    // for special configurations create a table config like below
    modelBuilder.ApplyConfiguration(new AddressConfig());
    modelBuilder.ApplyConfiguration(new EventLocationConfig());
    modelBuilder.ApplyConfiguration(new LocationConfig());
    modelBuilder.ApplyConfiguration(new UserLoginConfig());
    modelBuilder.ApplyConfiguration(new UserMemberTypeConfig());
    modelBuilder.ApplyConfiguration(new UserRoleConfig());
    modelBuilder.ApplyConfiguration(new UserTokenConfig());

    // must be after the table configurations.
    foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
      relationship.DeleteBehavior = DeleteBehavior.Restrict;
  }

  public IDbContextTransaction Transaction { get; set; }

  public bool EnableTriggers { get; set; } = true;

  #region Tables

  public DbSet<Address> Addresses { get; set; }
  public DbSet<Announcement> Announcements { get; set; }
  public DbSet<APILog> APILogs { get; set; }
  public DbSet<Country> Countries { get; set; }
  public DbSet<Event> Events { get; set; }
  public DbSet<EventLocation> EventLocations { get; set; }
  public DbSet<GoverningDistrict> GoverningDistricts { get; set; }
  public DbSet<Location> Locations { get; set; }
  public DbSet<LogType> LogTypes { get; set; }
  public DbSet<MemberType> MemberTypes { get; set; }
  public DbSet<MobileCarrier> MobileCarriers { get; set; }
  public DbSet<Picture> Pictures { get; set; }
  public DbSet<RoleClaim> RoleClaims { get; set; }
  public DbSet<Role> Roles { get; set; }
  public DbSet<SystemLog> SystemLogs { get; set; }
  public DbSet<UserClaim> UserClaims { get; set; }
  public DbSet<UserLogin> UserLogins { get; set; }
  public DbSet<UserMemberType> UserMemberTypes { get; set; }
  public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
  public DbSet<UserRole> UserRoles { get; set; }
  public DbSet<User> Users { get; set; }
  public DbSet<UserToken> UserTokens { get; set; }

  #endregion

  #region Triggers

  private void Triggers()
  {
    if (EnableTriggers && ChangeTracker.HasChanges())
    {
      CreateSystemLog();
      // must come after audit logs
      AssignLogGroupKeyToLogs();
    }
  }

  /// <summary>
  /// Creates a system log for any database changes that occur for auditing purposes.
  /// </summary>
  private void CreateSystemLog()
  {
    // get all DbSet Properties that have a class that has the AuditLog attribute
    // only log tables with "AuditLog" attribute
    var objectLogType = this.GetType().GetProperties().Where(w => w.PropertyType.IsGenericType && w.PropertyType.GenericTypeArguments[0].GetCustomAttributes(typeof(Team1.Model.Attributes.AuditLogAttribute), false).Any())
        .ToDictionary(k => k.PropertyType.GenericTypeArguments[0], v => v.PropertyType.GenericTypeArguments[0].GetCustomAttributes(typeof(Team1.Model.Attributes.AuditLogAttribute), false).OfType<Team1.Model.Attributes.AuditLogAttribute>().First());

    //Populate the IP Address for system logs
    foreach (var item in ChangeTracker.Entries().Where(obj => obj.Entity.GetType() == typeof(SystemLog)))
    {
      ((SystemLog)item.Entity).IpAddress = _userPermissionService.IpAddress;
    }

    // only log tables with "AuditLog" attribute
    foreach (var item in ChangeTracker.Entries().Where(w => w.Entity.GetType() != typeof(SystemLog) && objectLogType.ContainsKey(w.Entity.GetType())).ToList())
    {
      // don't log unchanged
      if (item.State == EntityState.Unchanged)
        continue;

      DateTimeOffset sysEventDate;
      int? sysUserId = null;
      int? itemId = null;

      //Check if object has audit fields
      var auditFieldsProperty = item.Entity.GetType().GetProperty("AuditFields");
      if (auditFieldsProperty != null)
      {
        Model.OwnedTypes.AuditFields auditFields = (Model.OwnedTypes.AuditFields)auditFieldsProperty.GetValue(item.Entity);
        sysUserId = auditFields.UpdatedById;
        sysEventDate = auditFields.UpdatedDateTime!.Value;
      }
      else
      {
        sysEventDate = DateTime.UtcNow;
      }

      //Check if object has an id
      var idProperty = item.Entity.GetType().GetProperty($"{item.Entity.GetType().Name}Id");
      if (idProperty != null)
      {
        itemId = (int?)idProperty.GetValue(item.Entity);
        if (item.Entity.GetType().BaseType == typeof(User))
        {
          sysUserId = itemId; //Set the userId == object id if the 
        }
      }

      var itemIdAddedOrUnknown = item.State == EntityState.Added ? "check matching timestamp in corresponding table to obtain added id" : "unknown";

      SystemLog sysLog = new SystemLog()
      {
        UserId = sysUserId <= 0 ? null : sysUserId,
        IpAddress = _userPermissionService.IpAddress,
        EventDateTime = sysEventDate,
        LogTypeId = objectLogType.ContainsKey(item.Entity.GetType()) ? objectLogType[item.Entity.GetType()].LogType : Team1.Model.Enums.LogTypeEnum.Unknown,
        EventDescription = String.Format("{0} with an id of ({1}) was {2}.",
              item.Entity.GetType().Name.ToString(), itemId.HasValue && itemId > 0 ? itemId.ToString() : itemIdAddedOrUnknown, item.State.ToString())
      };

      SystemLogs.Add(sysLog);
    }
  }

  private void AssignLogGroupKeyToLogs()
  {
    var allowedTypes = new List<Type>() { typeof(SystemLog), };

    foreach (var item in ChangeTracker.Entries().Where(w => allowedTypes.Contains(w.Entity.GetType())).ToList())
    {
      switch (item.Entity)
      {
        case SystemLog systemLog:
          if (systemLog.LogGroupKey.HasValue)
            systemLog.LogGroupKey = LogGroupKey;
          break;
      }
    }
  }

  #endregion

  #region Custom SQL

  public async Task<int> SaveIpAddress(int userId, string ipAddress)
  {
    return await Database.ExecuteSqlRawAsync(@"Update Users Set IpAddress = {0} Where UserId = {1}", ipAddress, userId);
  }

  public async Task<int> SaveLastSignInDateTime(int userId)
  {
    return await Database.ExecuteSqlRawAsync(@"Update Users Set LastSignInDateTime = GetUtcDate() Where UserId = {0}", userId);
  }

  public async Task<int> SaveLastPasswordChangeDateTime(int userId)
  {
    return await Database.ExecuteSqlRawAsync(@"Update Users Set LastPasswordChangeDateTime = GetUtcDate() Where UserId = {0}", userId);
  }

  public async Task<int> ClearUserRoles(int userId)
  {
    return await Database.ExecuteSqlRawAsync(@"Delete from UserRoles Where UserId = {0}", userId);
  }

  #endregion

  public override int SaveChanges(bool acceptAllChangesOnSuccess)
  {
    Triggers();
    return base.SaveChanges(acceptAllChangesOnSuccess);
  }

  public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
  {
    Triggers();
    return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
  }

  public async Task<IDbContextTransaction> BeginTransactionAsync()
  {
    Transaction = await Database.BeginTransactionAsync();
    return Transaction;
  }
}
