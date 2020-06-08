${
    // Enable extension methods by adding using Typewriter.Extensions.*
    using Typewriter.Extensions.Types;

    // Uncomment the constructor to change template settings.
    //Template(Settings settings)
    //{
    //    settings.IncludeProject("Project.Name");
    //    settings.OutputExtension = ".tsx";
    //}
    
    string BuildAttributeProperties(Property p)
    {
      var result = "";
      var requiredFound = false;
      var displayFound = false;
      if (p.Attributes == null)
        return result; 

      foreach (var item in p.Attributes)
      {
        switch(item.name.ToLower())
        {
          case "display":
          case "description": 
            result += $"{CleanUpAttributeText(item.Value)}, ";
            displayFound = true;
            break;
          case "emailaddress":
            result += $"emailAddress: {{ {CleanUpAttributeText(item.Value)} }}, ";
            break; 
          case "stringlength":
            result += "stringLength: { ";
            var attributeValues = item.Value.Split(',').Select(s=> CleanUpAttributeText(s)).ToList();
            foreach(var attributeValue in attributeValues)
            {
              if (int.TryParse(attributeValue, out int maxLength))
                result += $"maxLength: {maxLength}, ";
              else
                result += $"{attributeValue}, ";
            }
            result += "}, ";
            break;
          case "compare": 
            result += $"compare: {{ to: {CleanUpAttributeText(item.Value)} }}, ";
            break;
          case "required":
            requiredFound = true;
            result += $"required: {{ value: true, {CleanUpAttributeText(item.Value)} }}, ";
            break;
          case "column":
          case "datatype":
            // do nothing
            break;
          default:
            result += item.name + ": true, ";
            break;
        }
      }

      if (!requiredFound && !p.Type.IsNullable && !p.Type.IsEnumerable && p.Type.name != "string")
        result += "required: { value: true }, ";
      else if(!requiredFound && (p.Type.IsNullable || p.Type.IsGeneric || p.Type.name == "string"))
        result += "required: { value: false }, ";

      if (!displayFound)
        result += $"name: \"{p.Name}\", ";

      return result;
    }

    string CleanUpAttributeText(string value)
    {
      if (value == null)
        return null; 

      if (value.StartsWith("Name = "))
        value = value.Replace("Name = ", "name: ");
      return value.Replace("ErrorMessage = ", "errorMessage: ").Replace("MinimumLength = ", "minimumLength: ").Trim();
    }
    
    List<Property> ContructProperties(Class c)
    {
        if (c == null)
            return new List<Property>();

        var properties = c.Properties.Where(w=>w.Type.Name != typeof(System.IO.Stream).Name 
                    && !w.Attributes.Any(aw=>aw.Name == "NotMapped") && !w.Attributes.Any(cc=>cc.Name == "TypeWriterIgnore")).ToList();
        var propertyNames = properties.Select(x=> x.Name).ToList();
        var baseProperties = ContructProperties(c.BaseClass).Where(x => !propertyNames.Contains(x.Name) && !x.Attributes.Any(cc=>cc.Name == "TypeWriterIgnore")).ToList();

        return properties.Union(baseProperties).ToList();
    }

    List<Property> ContructPropertiesWithAttributes(Class c)
    {
      if (c == null)
            return new List<Property>();

        var properties = c.Properties.Where(w=>w.Type.Name != typeof(System.IO.Stream).Name
                    && !w.Attributes.Any(aw=>aw.Name == "NotMapped") && !w.Attributes.Any(cc=>cc.Name == "TypeWriterIgnore")).ToList();
        var propertyNames = properties.Select(x=> x.Name).ToList();
        var baseProperties = ContructProperties(c.BaseClass).Where(x => !propertyNames.Contains(x.Name) && !x.Attributes.Any(cc=>cc.Name == "TypeWriterIgnore")).ToList();

        return properties.Union(baseProperties).ToList();
    }

    List<Property> DateProperties(Class c)
    {
        if (c == null)
            return new List<Property>();

        return ContructProperties(c).Where(w => CleanType(w) == "Date").ToList();
    }

    string InterfaceNameForClass(Class c) 
    {
        var name = MakeInterfaceName(c.Name);
        if (c.IsGeneric)
        {
            name += $"<{string.Join(",",c.TypeParameters.Select(s=>s.Name))}>";
        }
        return name;
    }

    string InterfaceNameForPropertyAttributes(Class c) 
    {
        var name = MakeInterfaceName(c.Name);
        return name;
    }

    string MakeInterfaceName(string name)
    {
        return $"I{name.Replace("Dto","")}";
    }

    string CleanType(Property p)
    {
        var typeName = "";
        if (p.Type.Name == "IFormFile")
          typeName = "any"; 
        if (p.Type.Unwrap().IsEnum)
            typeName = "number";
        var parentClass = p.Parent as Class;
        var isGenericParamenter = parentClass.TypeParameters.Any(w=>w.Name == p.Type.Unwrap().Name);
        if (isGenericParamenter) 
            typeName = p.Type.Name;
        if (string.IsNullOrEmpty(typeName))
            typeName = p.Type.ClassName(); 
        var nameChangeList = p.Type.TypeArguments.Where(w=>w.Name.EndsWith("Dto") || w.Unwrap().Name.EndsWith("Dto")).ToDictionary(k=>k.Name, v=>MakeInterfaceName(v.Name));
        if (!p.Type.Unwrap().IsEnum && !p.Type.Unwrap().IsPrimitive && typeName != "any" && !isGenericParamenter)
            typeName = MakeInterfaceName(typeName);
        if (p.Type.IsEnumerable && !p.Type.FullName.Contains("Dictionary") && !typeName.Contains("[]")) 
            typeName += "[]";
             
            foreach (var item in nameChangeList)
            {
              typeName = typeName.Replace(item.Key, item.Value);
            }  
            return typeName;   
    }

    string GetAllModuleImports(Class c) {
      var properties = ContructProperties(c);
      var importsNeeded = new List<Type>();
      foreach (var property in properties)
      {
         importsNeeded.AddRange( GetModuleImports(property));
      }

      return string.Join("", importsNeeded.Select(s=>$"import {{{MakeInterfaceName(s.Unwrap().Name)}}} from './{s.Unwrap().Name}';\n").Distinct());
    }
      
    List<Type> GetModuleImports(Property p) {
      var typeName = "";
      if (p.Type.Name == "IFormFile")
          typeName = "any";  
      if (p.Type.Unwrap().IsEnum)
          typeName = "number";
      var parentClass = p.Parent as Class;
      var isGenericParamenter = parentClass.TypeParameters.Any(w=>w.Name == p.Type.Unwrap().Name);
      if (isGenericParamenter)
      {
          typeName = p.Type.Name;
      } 
      if (string.IsNullOrEmpty(typeName))
          typeName = p.Type.ClassName();

      var result = p.Type.TypeArguments.Where(w=>w.Name.EndsWith("Dto") && !w.IsEnumerable).ToList();
      result.AddRange(p.Type.TypeArguments.Where(w=>w.Unwrap().Name.EndsWith("Dto") && !w.Unwrap().IsEnumerable).Select(s=>s.Unwrap()).ToList());
      if (!p.Type.Unwrap().IsEnum && !p.Type.Unwrap().IsPrimitive && typeName != "any" && !isGenericParamenter)
      {
        result.Add(p.Type);
      }
      return result.ToList();
    }
} 
/* Autogenerated by Typewriter do NOT edit
   Dtos.tst is the generating template */
$Classes(c=>c.Name.EndsWith("Dto") && !c.Attributes.Any(cc=>cc.Name == "TypeWriterIgnore"))[
$GetAllModuleImports
export interface $InterfaceNameForClass  { 
    $ContructProperties[
    $name?: $CleanType;] 
} 

export class $InterfaceNameForPropertyAttributes_PropertyAttributes {
   $ContructPropertiesWithAttributes[ static readonly $name_Attributes = {
      $BuildAttributeProperties
      };
      ]
}]
$Classes(c=>c.Name.EndsWith("Dto") && !c.IsGeneric && !c.Attributes.Any(cc=>cc.Name == "TypeWriterIgnore"))[export var $InterfaceNameForClass_PrepareDto = function(dto: $InterfaceNameForClass) {		

    $DateProperties[if(dto.$name != null) {
        dto.$name = new Date(<any>dto.$name);
    }
    ]
    return dto;
}
]
