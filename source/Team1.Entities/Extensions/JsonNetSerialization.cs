using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System;
using System.Reflection;
using System.Runtime.Serialization;

public static class JSONNetSerialization
{
    public static string SerializeJsonNet<T>(this T toSerialize)
    {
        var jSettings = DefaultJsonSerializerSettings.Clone();
        jSettings.Error = JsonErrorHandler;

        return JsonConvert.SerializeObject(toSerialize, Formatting.None, jSettings);
    }

    public static T DeserializeJsonNet<T>(this string json)
    {
        if (System.Object.ReferenceEquals(json, null))
            return default;

        return JsonConvert.DeserializeObject<T>(json, DefaultJsonSerializerSettings);
    }

    public static T Clone<T>(this T obj)
    {
        if (System.Object.ReferenceEquals(obj, null))
            return default;

        return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj, Formatting.None, DefaultJsonSerializerSettings), DefaultJsonSerializerSettings);
    }

    public static object DeserializeJsonNetAsType(this string json, Type type)
    {
        if (System.Object.ReferenceEquals(json, null))
            return default;

        return JsonConvert.DeserializeObject(json, type, DefaultJsonSerializerSettings);
    }

    /// <summary>
    /// handles the error and moves on.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="errorContext"></param>
    private static void JsonErrorHandler(object sender, ErrorEventArgs args)
    {
        args.ErrorContext.Handled = true;
    }

    public static JsonSerializerSettings DefaultJsonSerializerSettings => new JsonSerializerSettings()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Ignore,
    };
}

// ignore virtual properties
public class IgnoreVirtualPropertiesContractResolver : DefaultContractResolver
{
    public static readonly IgnoreVirtualPropertiesContractResolver Instance = new IgnoreVirtualPropertiesContractResolver();

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        var properties = member.ReflectedType.GetProperties().Where(m => m.Name == member.Name);
        var propertyInfo = properties.FirstOrDefault(x => x.DeclaringType == member.DeclaringType) ?? properties.First();

        property.Ignored = propertyInfo.GetGetMethod().IsVirtual;

        return property;
    }
}
