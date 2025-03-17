namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class vw_PaySheetPekerjaCustomModel
    {
        public vw_PaySheetPekerja PaySheetPekerja { get; set; }
        public List<CarumanTambahanCustomModel> CarumanTambahan { get; set; }
    }


    public class vw_PaySheetPekerjaCustomModelWithCaruman
    {

        public string ladang { get; set; }
        public List<vw_PaySheetPekerjaCustomModel> PaySheetPekerjaCustom { get; set; }
    }
}
