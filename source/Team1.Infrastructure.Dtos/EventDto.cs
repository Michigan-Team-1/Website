using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Team1.Model;
using Team1.Model.Constants;
using Team1.Model.OwnedTypes;

namespace Team1.Infrastructure.Dtos;

public class EventDto : EventBase
  {
      [Display(Name = "Is Active")]
      public bool IsActive { get; set; }
      public bool IsUpdated { get; set; }

  [Display(Name = "Event Date")]
    [JsonIgnore]
    public DateTime? EventDateWrapper { 
      get
    {
      if (EventDate.Year < 2000)
        return null;

      return EventDate;
    }
    set
    {
      if (value.HasValue)
        EventDate = value.Value;
      else
        EventDate = DateTime.MinValue;
    }
  }

    [Display(Name="Locations")]
      public List<EventLocationDto>? EventLocations { get; set; }

  /// <summary>
  /// Used on the landing page
  /// </summary>
      public List<LocationDto>? Locations { get; set; }

    public AuditFieldsDto AuditFieldsDto { get; set; } = default!;
  public override bool Equals(object? obj)
  {
    if (obj == null)
      return false;

    if (obj is EventDto item)
    {
      var areEventLocationsEqual = AreEventLocationsEqual(item.EventLocations);
      return item.Name.IfNullThenEmptyString() == Name.IfNullThenEmptyString() && item.IsActive == IsActive
        && item.EventDate == EventDate && item.EventAlternateDate == EventAlternateDate && areEventLocationsEqual;
    }

    return false;
  }

  public bool AreEventLocationsEqual(List<EventLocationDto>? eventLocations)
  {
    if (eventLocations == null && EventLocations == null)
      return true;
    else if (eventLocations == null && EventLocations != null)
      return false;
    else if (eventLocations != null && EventLocations == null)
      return false;
    else if (eventLocations!.Count != EventLocations!.Count)
      return false;

    foreach (var item in from el in eventLocations
                         join eel in EventLocations on el.LocationId equals eel.LocationId into ljEEL
                         from eel in ljEEL.DefaultIfEmpty()
                         select new { el, eel })
    {
      if (item.eel == null)
        return false;
      if (!item.eel.Equals(item.el))
        return false;
    }

    return true;
  }

  public override int GetHashCode()
  {
    return base.GetHashCode();
  }
}

public class EventDtoValidator : AbstractValidator<EventDto>
{
  public EventDtoValidator()
  {
    RuleFor(x => x.Name).NotEmpty().WithMessage(ErrorMessages.FVRequiredField)
      .MaximumLength(FieldSizes.NameLength).WithMessage(ErrorMessages.FVStringLengthMax);
    RuleFor(x => x.EventDateWrapper).NotEmpty().WithMessage(ErrorMessages.FVRequiredField);
    RuleFor(x => x.EventLocations).Must(x=>x.Count(w=>w.LocationId > 0 && !w.IsDeleted) > 0).WithMessage("You must select at least one location.");
  }
}