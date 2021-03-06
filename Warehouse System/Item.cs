//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Warehouse_System
{
    using System;
    using System.Collections.Generic;
    
    public partial class Item
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Item()
        {
            this.Permission_Item = new HashSet<Permission_Item>();
            this.Store_Item = new HashSet<Store_Item>();
        }
    
        public int Code { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public System.DateTime ProductionDate { get; set; }
        public System.DateTime ExpDate { get; set; }
        public int SupplierId { get; set; }
    
        public virtual Stakeholder Stakeholder { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Permission_Item> Permission_Item { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Store_Item> Store_Item { get; set; }
    }
}
