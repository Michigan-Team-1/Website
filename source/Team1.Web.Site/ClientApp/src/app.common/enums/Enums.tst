${
    // Enable extension methods by adding using Typewriter.Extensions.*
    using Typewriter.Extensions.Types;
    
    string EnumTypesClassName(Enum thisEnum)
    {   
        return $"{thisEnum.Name}_class";
    }

    string DisplayName(EnumValue enumValue){
        var attributeList = enumValue.Attributes.ToList();

        string displayString = $"\"{enumValue.Name}\"";
        var displayNameAttribute = attributeList.FirstOrDefault(x=> x.FullName.Contains("Display"));
        if(displayNameAttribute != null)
        {
            displayString = ParsedValue(displayNameAttribute);
        }
        else
        {
            var descriptionNameAttribute = attributeList.FirstOrDefault(x=> x.FullName.Contains("Description"));
            if(descriptionNameAttribute != null)
            {            
                displayString = ParsedValue(descriptionNameAttribute);
            }
        }

        return displayString;
    }

    string ParsedValue(Attribute a) 
    {        
        if(int.TryParse(a.Value, out var value))
            return a.Value;

        string returnValue = $"\"{a?.Value?.Replace("\"", "\\\"")}\"" ?? "";
        if(returnValue.Contains("Name = "))
        {
            returnValue = returnValue.Replace("Name = ","").Replace("\\","").Replace("\"\"","\"");
        }
        return returnValue;        
    }
}

$Enums(c=>c.Name.EndsWith("Enum") && !c.Attributes.Any(cc=>cc.Name == "TypeWriterIgnore"))[
import {ISelectOption} from 'app.common/dtos/SelectOptionDto';
enum $Name {
        $Values[$Name = $Value,
        ] 
}

export class $EnumTypesClassName {

public static enum = $Name;

  public static readonly enumByText : { [key : string ]: number} = {
    $Values[['$Name']: $Value,
    ]
  };
  public static readonly enumByNumber : { [key : number ]: string} = {
    $Values[[$Value]: $DisplayName,
    ]
  };

  public static readonly enumAsSelectOptions:Array<ISelectOption<number>> = [
  $Values[{value: $Value, text: $DisplayName},
  ]]
}]

