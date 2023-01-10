using System;
using System.Collections.Generic;

#nullable disable

namespace UVisa.Models
{
    public partial class UserInfo
    {
        public UserInfo()
        {
            Orders = new HashSet<Order>();
        }

        public int UserInfoId { get; set; }
        public string UserInfoName { get; set; }
        public string UserInfoSurname { get; set; }
        public string UserInfoPhone { get; set; }
        public string UserInfoEmail { get; set; }
        public string UserInfoPassportId { get; set; }
        public string UserInfoTypeVisa { get; set; }
        public string UserInfoFile { get; set; }
        public string UserInfoAge { get; set; }
        public int? UserInfoStatusId { get; set; }

        public virtual Status UserInfoStatus { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
