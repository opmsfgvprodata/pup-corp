namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_BlckKmskknDataKerjaHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //fatin added - 20/05/2024
        public Guid fld_ID { get; set; }

        public bool? fld_BlokStatus { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_Year { get; set; }

        [StringLength(300)]
        public string fld_Reason { get; set; }

        public short? fld_BilHariXKyIn { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_UnBlockAppBy { get; set; }

        public DateTime? fld_UnBlockAppDT { get; set; }

        [StringLength(300)]
        public string fld_Remark { get; set; }
        public int? fld_DivisionID { get; set; } //add by wani 2.7.2021

        [StringLength(50)]
        public string fld_Purpose { get; set; } //fatin added - 29/05/2024

    }
}
