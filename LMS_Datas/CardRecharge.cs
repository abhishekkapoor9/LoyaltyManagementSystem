//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LMS_Datas
{
    using System;
    using System.Collections.Generic;
    
    public partial class CardRecharge
    {
        public int CardRechargeId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> Amount { get; set; }
        public Nullable<System.DateTime> TodayDate { get; set; }
        public Nullable<bool> Status { get; set; }
        public string CardNo { get; set; }
        public Nullable<int> PayModeId { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual PayMode PayMode { get; set; }
    }
}
