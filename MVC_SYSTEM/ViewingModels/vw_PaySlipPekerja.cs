﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.Models;

namespace MVC_SYSTEM.ViewingModels
{
    public class vw_PaySlipPekerja
    {
        public vw_GajiPekerja Pkjmast { get; set; }
        public ViewingModels.tbl_GajiBulanan GajiBulanan { get; set; }
        public List<vw_MaklumatInsentif> InsentifPekerja { get; set; }
        public List<KerjaPekerjaCustomModel> KerjaPekerja { get; set; }
        public List<OTPekerjaCustomModel> OTPekerja { get; set; }
        public List<BonusPekerjaCustomModel> BonusPekerja { get; set; }
        public List<CutiPekerjaCustomModel> CutiPekerja { get; set; }
        public List<FootNoteCustomModel> FootNote { get; set; }
        public List<CarumanTambahanCustomModel> CarumanTambahan { get; set; }
    }
}