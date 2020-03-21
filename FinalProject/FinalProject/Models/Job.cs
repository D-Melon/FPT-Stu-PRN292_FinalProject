//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FinalProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Job
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Job()
        {
            this.User_Job = new HashSet<User_Job>();
        }
    
        public int ID { get; set; }
        public int SkillID { get; set; }
        public int EmpID { get; set; }
        public int PositionID { get; set; }
        public string Title { get; set; }
        public string Short_Des { get; set; }
        public string Detail { get; set; }
        public double Salary { get; set; }
        public double Time { get; set; }
        public int LocationID { get; set; }
    
        public virtual Location Location { get; set; }
        public virtual Position Position { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User_Job> User_Job { get; set; }
    }
}