using System.Collections.Generic;

namespace Team1.Infrastructure
{
    public class IpWhiteListSettings
    {
        public List<IpWhiteListItem> IpList { get; set; }

        public class IpWhiteListItem
        {
            public string Name { get; set; }
            public List<string> Values { get; set; }
        }
    }
}
