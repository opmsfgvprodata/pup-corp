﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ViewingModels
{
    public class CarumanTambahanCustomModel
    {
        [Key]
        public Guid? fld_ID { get; set; }

        public string fld_KodCarumanTambahan { get; set; }

        public decimal? fld_CarumanPekerja { get; set; }

        public decimal? fld_CarumanMajikan { get; set; }
    }
}