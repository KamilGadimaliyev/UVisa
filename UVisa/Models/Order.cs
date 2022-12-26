using System;
using System.Collections.Generic;

#nullable disable

namespace UVisa.Models
{
    public partial class Order
    {
        public int OrderId { get; set; }
        public int? OrderUserInfoId { get; set; }
        public decimal? OrderMoney { get; set; }
        public DateTime? OrderDate { get; set; }
        public bool? OrderStatus { get; set; }

        public virtual UserInfo OrderUserInfo { get; set; }
    }
}
