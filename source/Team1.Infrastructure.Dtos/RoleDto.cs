using Team1.Model.UserIdentity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Team1.Infrastructure.Dtos
{
    public class RoleDto : RoleBase
    {
        public RoleDataDto DataObj { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsAdded { get; set; }
    }
}
