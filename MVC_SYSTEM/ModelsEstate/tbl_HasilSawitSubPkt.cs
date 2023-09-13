namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_HasilSawitSubPkt
    {
        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(10)]
        public string fld_KodSubPeringkat { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_HasilTan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasHektar { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Bulan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Tahun { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(10)]
        public string fld_YieldType { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }
    }
}
