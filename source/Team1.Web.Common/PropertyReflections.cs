using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Dtos.Users;

namespace Team1.Web.Common;

public static class PropertyReflections
{
  #region PropertyInfos

  private static Dictionary<string, List<PropertyInfo>> properties = new Dictionary<string, List<PropertyInfo>>();

  public static PropertyInfo GetAnnouncementDtoProperty(string name)
  {
    return AnnouncementDtoProperties.Single(w => w.Name == name);
  }

  public static List<PropertyInfo> AnnouncementDtoProperties
  {
    get
    {
      if (!properties.ContainsKey(nameof(AnnouncementDto)))
        properties[nameof(AnnouncementDto)] = typeof(AnnouncementDto).GetProperties().ToList();

      return properties[nameof(AnnouncementDto)];
    }
  }

  public static PropertyInfo GetEventDtoProperty(string name)
  {
    return EventDtoProperties.Single(w => w.Name == name);
  }

  public static List<PropertyInfo> EventDtoProperties
  {
    get
    {
      if (!properties.ContainsKey(nameof(EventDto)))
        properties[nameof(EventDto)] = typeof(EventDto).GetProperties().ToList();

      return properties[nameof(EventDto)];
    }
  }

  public static PropertyInfo GetLocationDtoProperty(string name)
  {
    return LocationDtoProperties.Single(w => w.Name == name);
  }

  public static List<PropertyInfo> LocationDtoProperties
  {
    get
    {
      if (!properties.ContainsKey(nameof(LocationDto)))
        properties[nameof(LocationDto)] = typeof(LocationDto).GetProperties().ToList();
      
      return properties[nameof(LocationDto)];
    }
  }

  public static PropertyInfo GetAuditFieldsDtoProperty(string name)
  {
    return AuditFieldsDtoProperties.Single(w => w.Name == name);
  }

  public static List<PropertyInfo> AuditFieldsDtoProperties
  {
    get
    {
      if (!properties.ContainsKey(nameof(AuditFieldsDto)))
        properties[nameof(AuditFieldsDto)] = typeof(AuditFieldsDto).GetProperties().ToList();
      
      return properties[nameof(AuditFieldsDto)];
    }
  }

  public static PropertyInfo GetAddressObjDtoProperty(string name)
  {
    return AddressObjDtoProperties.Single(w => w.Name == name);
  }

  public static List<PropertyInfo> AddressObjDtoProperties
  {
    get
    {
      if (!properties.ContainsKey(nameof(AddressObjDto)))
        properties[nameof(AddressObjDto)] = typeof(AddressObjDto).GetProperties().ToList();

      return properties[nameof(AddressObjDto)];
    }
  }

  public static PropertyInfo GetPictureDtoProperty(string name)
  {
    return PictureDtoProperties.Single(w => w.Name == name);
  }

  public static List<PropertyInfo> PictureDtoProperties
  {
    get
    {
      if (!properties.ContainsKey(nameof(PictureDto)))
        properties[nameof(PictureDto)] = typeof(PictureDto).GetProperties().ToList();

      return properties[nameof(PictureDto)];
    }
  }

  public static PropertyInfo GetUserDtoProperty(string name)
  {
    return UserDtoProperties.Single(w => w.Name == name);
  }

  public static List<PropertyInfo> UserDtoProperties
  {
    get
    {
      if (!properties.ContainsKey(nameof(UserDto)))
        properties[nameof(UserDto)] = typeof(UserDto).GetProperties().ToList();

      return properties[nameof(UserDto)];
    }
  }

  #endregion

  #region helpers

  public static PropertyInfo GetPropertyByName(List<PropertyInfo> properties, string name)
  {
    return properties.Single(w => w.Name == name);
  }

  /// <summary>
  /// Returns proper label name for a property based on attributes
  /// </summary>
  /// <param name="propertyInfo">property's reflection info</param>
  /// <returns>string</returns>
  public static string GetLabelName(this PropertyInfo propertyInfo)
  {
    var result = propertyInfo.Name;
    {
      var attribute = propertyInfo.GetCustomAttribute<DisplayAttribute>();
      if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Name))
        result = attribute.Name;
    }

    {
      var attribute = propertyInfo.GetCustomAttribute<DescriptionAttribute>();
      if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Description))
        result = attribute.Description;
    }
    return result;
  }

  /// <summary>
  /// Returns label name for a property based on a list
  /// </summary>
  /// <param name="properties">list of property infos</param>
  /// <param name="propertyToGet">property string name. hint use: nameof(property)</param>
  /// <returns>string</returns>
  public static string GetLabelName(List<PropertyInfo> properties, string propertyToGet, bool isShortName = false)
  {
    var propInfo = properties.Find((p) => p.Name == propertyToGet);
    if (propInfo == null)
      throw new ArgumentException(nameof(propertyToGet));

    if (isShortName)
      return GetLabelShortName(propInfo);

    return GetLabelName(propInfo);
  }

  /// <summary>
  /// Gets the display short name and returns it.
  /// </summary>
  /// <param name="propertyInfo">property's reflection info</param>
  /// <returns>string</returns>
  public static string GetLabelShortName(PropertyInfo propertyInfo)
  {
    var result = propertyInfo.Name;
    {
      var attribute = propertyInfo.GetCustomAttribute<DisplayAttribute>();
      if (attribute != null && !string.IsNullOrWhiteSpace(attribute.ShortName))
        result = attribute.ShortName;
      else
        result = GetLabelName(propertyInfo);
    }
    return result;
  }

  /// <summary>
  /// returns true if the field is required
  /// </summary>
  /// <param name="propertyInfo">property info of the property</param>
  /// <returns>bool</returns>
  public static bool IsRequired(PropertyInfo propertyInfo)
  {
    if (propertyInfo == null)
      return false;

    if (propertyInfo.GetCustomAttributes(typeof(RequiredAttribute), true).Length > 0)
      return true;

    return propertyInfo.PropertyType.IsValueType && Nullable.GetUnderlyingType(propertyInfo.PropertyType) == null;
  }

  #endregion
}
