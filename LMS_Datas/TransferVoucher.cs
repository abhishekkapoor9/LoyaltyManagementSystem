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
    
    public partial class TransferVoucher
    {
        public int TransferVoucherID { get; set; }
        public int VoucherID { get; set; }
        public Nullable<System.DateTime> TranferDate { get; set; }
        public Nullable<int> TransferFromCustomerID { get; set; }
        public Nullable<int> TransferToCustomerID { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Customer Customer1 { get; set; }
    }
}
