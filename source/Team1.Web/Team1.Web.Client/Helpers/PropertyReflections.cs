using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Team1.Infrastructure.Dtos.Users;

namespace Team1.Web.Client.Helpers;

public static class PropertyReflections
{
  private static Dictionary<string, List<PropertyInfo>> dtoProperties = new Dictionary<string, List<PropertyInfo>>();
  public static List<PropertyInfo> UserDtoProperties
  {
    get
    {
      if (dtoProperties.ContainsKey(nameof(UserDto)))
        dtoProperties[nameof(UserDto)] = typeof(UserDto).GetProperties().ToList();

      return dtoProperties[nameof(UserDto)];
    }
  }

  public static PropertyInfo GetRocketDtoProperty(string name)
  {
    return UserDtoProperties.Single(w => w.Name == name);
  }

  public static PropertyInfo GetPropertyByName(List<PropertyInfo> properties, string name)
  {
    return properties.Single(w => w.Name == name);
  }


  /// <summary>
  /// Returns proper label name for a property based on attributes
  /// </summary>
  /// <param name="propertyInfo">property's reflection info</param>
  /// <returns>string</returns>
  public static string GetLabelName(PropertyInfo propertyInfo)
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

    if (propertyInfo.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute), true).Length > 0)
      return true;

    return propertyInfo.PropertyType.IsValueType && Nullable.GetUnderlyingType(propertyInfo.PropertyType) == null;
  }
}
