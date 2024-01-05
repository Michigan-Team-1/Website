using System.Reflection;

public static class SmartishTableHelpers
{
    public static Dictionary<string, SmartishTable.Filters.StringOperators> GetStringOperatorsForTypeProperties(this List<PropertyInfo> properties, SmartishTable.Filters.StringOperators defaultOperator = SmartishTable.Filters.StringOperators.Contains)
    {
        return properties.Where(w => w.PropertyType == typeof(string)).ToDictionary(k => k.Name, v => defaultOperator);
    }

    public static Dictionary<string, SmartishTable.Filters.DateTimeOperators> GetDateTimeOperatorsForTypeProperties(this List<PropertyInfo> properties, SmartishTable.Filters.DateTimeOperators defaultOperator = SmartishTable.Filters.DateTimeOperators.GreaterThanOrEqual)
    {
        return properties.Where(w=>w.PropertyType.IsDateTimeType()).ToDictionary(k => k.Name, v => defaultOperator);
    }

    public static Dictionary<string, SmartishTable.Filters.NumericOperators> GetNumericOperatorsForTypeProperties(this List<PropertyInfo> properties, SmartishTable.Filters.NumericOperators defaultOperator = SmartishTable.Filters.NumericOperators.GreaterThanOrEqual)
    {
        return properties.Where(w => w.PropertyType.IsNumericType()).ToDictionary(k => k.Name, v => defaultOperator);
    }

    public static Dictionary<string, SmartishTable.Filters.BooleanOperators> GetBooleanOperatorsForTypeProperties(this List<PropertyInfo> properties, SmartishTable.Filters.BooleanOperators defaultOperator = SmartishTable.Filters.BooleanOperators.IsTrue)
    {
        return properties.Where(w => w.PropertyType.IsBooleanType()).ToDictionary(k => k.Name, v => defaultOperator);
    }
}
