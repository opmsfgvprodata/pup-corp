namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SAPPDPUP
    {
        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(50)]
        public string fld_ProjectDefinition { get; set; }

        [StringLength(50)]
        public string fld_ProjectDesc { get; set; }

        [StringLength(50)]
        public string fld_WBSElement { get; set; }

        [StringLength(50)]
        public string fld_WBSDesc { get; set; }

        [StringLength(50)]
        public string fld_WBSCode { get; set; }

        [StringLength(50)]
        public string fld_WorkActivity { get; set; }

        [StringLength(50)]
        public string fld_ActivityType { get; set; }

        [StringLength(50)]
        public string fld_NetworkNo { get; set; }

        [StringLength(50)]
        public string fld_ActivityCode { get; set; }

        [StringLength(200)]
        public string fld_ActivityDesc { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_IsSelected { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(50)]
        public string fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        public DateTime? fld_ModifiedDt { get; set; }

        [StringLength(50)]
        public string fld_ModifiedBy { get; set; }
    }
}
