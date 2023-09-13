namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Bank
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(5)]
        public string fld_KodBank { get; set; }

        [StringLength(50)]
        public string fld_NamaBank { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_NegaraID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
