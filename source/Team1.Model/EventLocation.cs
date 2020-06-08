using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Team1.Model
{
    public class EventLocationBase
    {
        public int EventId { get; set; }
        public int LocationId { get; set; }
    }

    public class EventLocation : EventLocationBase
    {
        #region Navigation Links

        public virtual Event Event { get; set; }

        public virtual Location Location { get; set; }

        #endregion
    }
}
