using FluentValidation;
using Team1.Infrastructure.Dtos;
using Team1.Model;
using Team1.Model.Constants;

namespace Team1.Infrastructure.Dtos
{
  public class EventLocationDto : EventLocationBase
  {
    public bool IsDeleted { get; set; }

    public override bool Equals(object? obj)
    {
      if (obj == null)
        return false;

      if (obj is EventLocationDto item)
      {
        return item.LocationId == LocationId && item.IsDeleted == IsDeleted;
      }

      return false;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}

public class EventLocationDtoValidator : AbstractValidator<EventLocationDto>
{
  public EventLocationDtoValidator()
  {
    RuleFor(x => x.LocationId).NotEmpty().WithMessage(ErrorMessages.FVRequiredField);
  }
}