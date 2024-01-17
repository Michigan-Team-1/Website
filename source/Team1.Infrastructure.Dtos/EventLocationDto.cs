using Team1.Model;

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