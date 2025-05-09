﻿using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsCorporate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class GetNSWL
    {
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        GetIdentity getidentity = new GetIdentity();
        GetWilayah getwilyah = new GetWilayah();

        //new Model
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();

        public void GetData(out int ? NegaraID, out int ? SyarikatID, out int?  WilayahID, out int ? LadangID, int ? userid, string username)
        {
            NegaraID = 0;
            SyarikatID = 0;
            WilayahID = 0;
            LadangID = 0;

            if (getidentity.SuperPowerAdmin(username) || getidentity.SuperAdmin(username))
            {
                var getcountycompany = db2.tbl_SuperAdminSelection.Where(x => x.fld_SuperAdminID == userid).FirstOrDefault();
                NegaraID = getcountycompany.fld_NegaraID;
                SyarikatID = getcountycompany.fld_SyarikatID;
                WilayahID = 0;
                LadangID = 0;
            }
            else if (getidentity.Admin1(username))
            {
                var getcountycompany = db2.tblUsers.Where(x => x.fldUserID == userid).FirstOrDefault();
                NegaraID = getcountycompany.fldNegaraID;
                SyarikatID = getcountycompany.fldSyarikatID;
                WilayahID = 0;
                LadangID = 0;
            }
            else if (getidentity.Admin2(username))
            {
                var getcountycompany = db2.tblUsers.Where(x => x.fldUserID == userid).FirstOrDefault();
                NegaraID = getcountycompany.fldNegaraID;
                SyarikatID = getcountycompany.fldSyarikatID;
                WilayahID = getcountycompany.fldWilayahID;
                LadangID = 0;
            }
            else if (getidentity.SuperPowerUser(username))
            {
                var getcountycompany = db2.tblUsers.Where(x => x.fldUserID == userid).FirstOrDefault();
                NegaraID = getcountycompany.fldNegaraID;
                SyarikatID = getcountycompany.fldSyarikatID;
                WilayahID = getcountycompany.fldWilayahID;
                LadangID = getcountycompany.fldLadangID;
            }
            else if (getidentity.SuperUser(username) || getidentity.NormalUser(username))
            {
                var getcountycompany = db2.tblUsers.Where(x => x.fldUserID == userid).FirstOrDefault();
                NegaraID = getcountycompany.fldNegaraID;
                SyarikatID = getcountycompany.fldSyarikatID;
                WilayahID = getcountycompany.fldWilayahID;
                LadangID = getcountycompany.fldLadangID;
            }
        }

        public ModelsCorporate.vw_NSWL GetLadangDetail(int LadangID)
        {
            ModelsCorporate.vw_NSWL NSWL = new ModelsCorporate.vw_NSWL();

            NSWL = db.vw_NSWL.Where(x => x.fld_LadangID == LadangID).FirstOrDefault();

            db.Dispose();

            return NSWL;
        }

        public ModelsCorporate.vw_NSWL GetLadangDetail(string kdprmhnan, string kdldg)
        {
            ModelsCorporate.vw_NSWL NSWL = new ModelsCorporate.vw_NSWL();

            NSWL = db.vw_NSWL.Where(x => x.fld_LdgCode == kdldg && x.fld_RequestCode == kdprmhnan).FirstOrDefault();

            db.Dispose();

            return NSWL;
        }

        public ModelsCorporate.vw_NSWL GetLadangDetailByKodLadang(string KodLadang)
        {
            ModelsCorporate.vw_NSWL NSWL = new ModelsCorporate.vw_NSWL();

            NSWL = db.vw_NSWL.Where(x => x.fld_LdgCode == KodLadang).FirstOrDefault();

            db.Dispose();

            return NSWL;
        }

        public List<vw_NSWL_2> GetLadangDetailByRegion(int? regionID)
        {
            List<vw_NSWL_2> NSWL = new List<vw_NSWL_2>();

            NSWL = db.vw_NSWL_2.Where(x => x.fld_WilayahID == regionID && x.fld_Deleted_L == false).ToList();

            db.Dispose();

            return NSWL;
        }

        public void GetSyarikatRCMSDetail(int? Region, int? Estate, out string CorpID, out string ClientID, out string AccNo, out string InitialName)
        {
            var getsyarikat = db.tbl_Ladang.Where(x => x.fld_WlyhID == Region && x.fld_ID == Estate).FirstOrDefault();
            CorpID = getsyarikat.fld_CorporateID;
            ClientID = getsyarikat.fld_ClientBatchID;
            AccNo = getsyarikat.fld_NoAcc;
            InitialName = getsyarikat.fld_LdgCode;
        }

    }
}