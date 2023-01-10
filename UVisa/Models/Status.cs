using System;
using System.Collections.Generic;

#nullable disable

namespace UVisa.Models
{
    public partial class Status
    {
        public Status()
        {
            UserInfos = new HashSet<UserInfo>();
        }

        public int StatusId { get; set; }
        public string StatusName { get; set; }

        public virtual ICollection<UserInfo> UserInfos { get; set; }
    }
}
