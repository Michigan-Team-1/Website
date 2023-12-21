using System.ComponentModel.DataAnnotations;
using Team1.Model;

namespace Team1.Infrastructure.Dtos;

public class EventDto : EventBase
  {
      [Display(Name = "Is Active")]
      public bool IsActive { get; set; }
      public bool IsUpdated { get; set; }

      [Display(Name="Locations")]
      public IEnumerable<EventLocationDto> EventLocations { get; set; }
      public IEnumerable<LocationDto> Locations { get; set; }
  }
