﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsEstate;

namespace MVC_SYSTEM.ModelsCustom
{

    public class CustMod_PaidLeaveGenerateResult
    {
        public int wilayahID { get; set; }

        public int ladangID { get; set; }

        public string divisionName { get; set; }

        public List<CustMod_WorkerList> pkjList { get; set; }
    }

    public class CustMod_WorkerList
    {
        public string noPkj { get; set; }

        public string namaPkj { get; set; }

        public DateTime? tarikhMulaKerja { get; set; }

        public List<CustMod_CutiList> cuti { get; set; }
    }

    public class CustMod_CutiList
    {
        public string leaveName { get; set; }

        public int leaveAmount { get; set; }
    }

    public partial class PaidLeaveGenerate_ModalCreate
    {
        public int? fld_Year { get; set; }


        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_WilayahID { get; set; }


        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public int? fld_LadangID { get; set; }


        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public IEnumerable<int> fld_DivisionID { get; set; }


        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public IEnumerable<string> fld_CutiKategoriID { get; set; }
    }
}