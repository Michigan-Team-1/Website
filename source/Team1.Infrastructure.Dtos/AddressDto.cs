using Team1.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Team1.Infrastructure.Dtos
{
    public class AddressDto : AddressBase
    {
        public AddressObjDto AddressObj { get; set; }
        public bool IsActive { get; set; } 
        public bool IsUpdated { get; set; }
        public bool IsDeleted { get; set; }
    }
}
