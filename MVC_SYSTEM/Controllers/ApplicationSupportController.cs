using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.ModelsCorporate;

namespace MVC_SYSTEM.Controllers
{
    public class ApplicationSupportController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private GetIdentity getidentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        private GetWilayah getwilyah = new GetWilayah();
        private DatabaseAction DatabaseAction = new DatabaseAction();
        private GetIdentity GetIdentity = new GetIdentity();
        private SendEmailNotification SendEmailNotification = new SendEmailNotification();
        private errorlog geterror = new errorlog();
        // GET: ApplicationSupport
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult Index()
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.ApplicationSupport = "class = active";

            ViewBag.ApplicationSupportList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "applicationsupportlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc), "fldOptConfValue", "fldOptConfDesc");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult Index(string ApplicationSupportList)
        {
            return RedirectToAction(ApplicationSupportList, "ApplicationSupport");
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult ApplicationSupportRegion()
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int year = timezone.gettimezone().Year;

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }

            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.GetView = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult ApplicationSupportRegion(int WilayahIDList, int LadangIDList)
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            DateTime getdate = timezone.gettimezone().AddMonths(-1);
            //DateTime getdate = timezone.gettimezone();

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            }

            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            if(WilayahIDList == 0)
            {
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, getdate.Month, getdate.Year);
            }
            else
            {
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID,SyarikatID,WilayahIDList, getdate.Month, getdate.Year);
            }
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.LadangID = LadangIDList;
            ViewBag.Month = getdate.Month;
            ViewBag.Year = getdate.Year;
            ViewBag.GetView = 0;
            return View();
        }
        
        public ActionResult ApplicationSupportRegionDetail(List<long> eachid)
        {
            var getdata = db.vw_PermohonanKewangan.Where(x=> eachid.Contains(x.fld_ID) && x.fld_StsTtpUrsNiaga == true).ToList();
            ViewBag.getgmstatus = getdata.Where(x => x.fld_SokongWilGM_Status == 1).Count();
            return View(getdata);
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2")]
        public ActionResult ApplicationSupportRegionGm()
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int year = timezone.gettimezone().Year;

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }

            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.GetView = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2")]
        public ActionResult ApplicationSupportRegionGm(int WilayahIDList)
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            DateTime getdate = timezone.gettimezone().AddMonths(-1);
            //DateTime getdate = timezone.gettimezone();
            int LadangIDList = 0;

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            }

            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            if (WilayahIDList == 0)
            {
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, getdate.Month, getdate.Year);
            }
            else
            {
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, WilayahIDList, getdate.Month, getdate.Year);
            }
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.LadangID = LadangIDList;
            ViewBag.Month = getdate.Month;
            ViewBag.Year = getdate.Year;
            ViewBag.GetView = 0;
            return View();
        }

        public ActionResult ApplicationSupportRegionGMDetail(List<long> eachid, int NegaraID, int SyarikatID, int WilayahID, int Month, int Year)
        {
            bool matchtotal = false;
            var getdata = db.vw_PermohonanKewangan.Where(x => eachid.Contains(x.fld_ID) && x.fld_StsTtpUrsNiaga == true).ToList();
            var getcoundata = db.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Month == Month && x.fld_Year == Year && x.fld_SemakWil_Status == 1).Count();
            if (getdata.Count() == getcoundata)
            {
                matchtotal = true;
            }

            ViewBag.matchtotal = matchtotal;
            return View(getdata);
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult ApplicationSupportRegionHQ()
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int year = timezone.gettimezone().Year;

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }

            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.GetView = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult ApplicationSupportRegionHQ(int WilayahIDList)
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            DateTime getdate = timezone.gettimezone().AddMonths(-1);
            //DateTime getdate = timezone.gettimezone();
            int LadangIDList = 0;

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            }

            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            if (WilayahIDList == 0)
            {
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, getdate.Month, getdate.Year);
            }
            else
            {
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, WilayahIDList, getdate.Month, getdate.Year);
            }
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.LadangID = LadangIDList;
            ViewBag.Month = getdate.Month;
            ViewBag.Year = getdate.Year;
            ViewBag.GetView = 0;
            return View();
        }

        public ActionResult ApplicationSupportRegionHQDetail(List<long> eachid, int NegaraID, int SyarikatID, int WilayahID, int Month, int Year)
        {
            bool matchtotal = false;
            var getdata = db.vw_PermohonanKewangan.Where(x => eachid.Contains(x.fld_ID) && x.fld_StsTtpUrsNiaga == true).ToList();
            var getcoundata = db.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Month == Month && x.fld_Year == Year && x.fld_SemakWil_Status == 1 && x.fld_SokongWilGM_Status == 1).Count();
            if (getdata.Count() == getcoundata)
            {
                matchtotal = true;
            }

            ViewBag.matchtotal = matchtotal;
            return View(getdata);
        }

        public string ApplicationSupportHistoryDetail(long SPWID)
        {
            string returndetail = "";
            string fontcolor = "";
            var getdata = db.tblSokPermhnWangHisActions.Where(x => x.fldHisSPWID == SPWID).OrderBy(o=>o.fldHisDT).ToArray();
            if (getdata != null)
            {
                foreach (var data in getdata)
                {
                    if (data.fldHisDesc == "Telah Ditolak" || data.fldHisDesc == "Urus Niaga Dibuka Semula")
                    {
                        fontcolor = "red";
                    }
                    else
                    {
                        fontcolor = "green";
                    }
                    returndetail = returndetail + "<font color=\""+ fontcolor + "\"><p class=\"specialClass\">" + data.fldHisDesc + " oleh " + getidentity.MyNameFullName(data.fldHisUserID) + " pada " + data.fldHisDT + "</p>";
                }
            }
            return returndetail;
        }

        public JsonResult UpdateData(long DataID, string UpdateFlag, decimal PDP, decimal CIT, int NegaraId, int SyarikatId, int WilayahId, decimal JumlahWang, int Month, int Year, string NoAcc, string NoGL, string NoCIT, decimal Manual)
        {
            string DescStatus = "";
            int getuserid = getidentity.ID(User.Identity.Name);
            string ActionBy = GetIdentity.MyNameFullName(getuserid);
            string NamaWilayah = getwilyah.GetWilayahName(WilayahId);
            string subject = "";
            string msg = "";
            string[] cc = new string[] { };
            List<string> cclist = new List<string>();
            string[] bcc = new string[] { };
            List<string> bcclist = new List<string>();
            DateTime getdatetime = timezone.gettimezone();
            bool matchtotal = false;
            switch (UpdateFlag)
            {
                case "SemakWil":
                    DescStatus = "Telah Disemak";
                    DatabaseAction.UpdateDataTotblSokPermhnWang(DataID, 1, 0, 0, 0, 0, 0, "SemakWil", getuserid, getdatetime, PDP, CIT, NoAcc, NoGL, NoCIT, Manual);
                    DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, DataID, 1);

                    var getdata = db.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId && x.fld_Month == Month && x.fld_Year == Year).Count();
                    var getcoundata = db.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId && x.fld_Month == Month && x.fld_Year == Year && x.fld_SemakWil_Status == 1).Count();
                    var getJumlahKeseluruhanPermohonan = db.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId && x.fld_Month == Month && x.fld_Year == Year).Select(s=>s.fld_JumlahPermohonan).Sum();
                    if (getdata == getcoundata)
                    {
                        matchtotal = true;
                    }

                    if (matchtotal == true)
                    {
                        //subject = "Sokongan Permohonan Wang";
                        
                        //var GetSentToGM = db2.tblUsers.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == WilayahId && x.fldRoleID == 4).FirstOrDefault();

                        //msg = "<html>";
                        //msg += "<body>";
                        //msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                        //msg += "<p>Tn/Pn " + GetSentToGM.fldUserShortName + ",</p>";
                        //msg += "<p>Sokongan permohonan kewangan (Gaji Pekerja Buruh) untuk kelulusan diperlukan dari pihak Tn/Pn. Keterangan seperti dibawah:-</p>";
                        //msg += "<table border=\"1\">";
                        //msg += "<thead>";
                        //msg += "<tr>";
                        //msg += "<th>Nama Wilayah</th><th>Jumlah Permohonan</th><th>Pautan</th>";
                        //msg += "</tr>";
                        //msg += "</thead>";
                        //msg += "<tbody>";
                        //msg += "<tr>";
                        //msg += "<td>" + NamaWilayah + "</td><td>" + getJumlahKeseluruhanPermohonan + "</td><td><a href=\"" + Url.Action("ApplicationSupportRegionGm", "ApplicationSupport", null, "http") + "\">Klik ke pautan sokongan</a></td>";
                        //msg += "</tr>";
                        //msg += "</tbody>";
                        //msg += "</table>";
                        //msg += "<p>Terima Kasih.</p>";
                        //msg += "</body>";
                        //msg += "</html>";

                        //var emailbcclist1 = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                        //if (emailbcclist1 != null)
                        //{
                        //    foreach (var bccemail in emailbcclist1)
                        //    {
                        //        bcclist.Add(bccemail.fldEmail);
                        //    }
                        //    bcc = bcclist.ToArray();
                        //}

                        //SendEmailNotification.SendEmail(subject, msg, GetSentToGM.fldUserEmail, cc, bcc);
                    }
                    
                    break;
                case "TolakWil":
                    DescStatus = "Telah Ditolak";
                    DatabaseAction.UpdateDataTotblSokPermhnWang(DataID, 0, 1, 0, 0, 0, 0, "TolakWil", getuserid, getdatetime, 0, 0, "", "", "", 0);
                    DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, DataID, 1);

                    subject = "Penolakkan Permohonan Wang";

                    var GetLdgID = db.tbl_SokPermhnWang.Where(x => x.fld_ID == DataID &&  x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId).FirstOrDefault();

                    var GetLdgDetail = db2.tbl_Ladang.Where(x => x.fld_ID == GetLdgID.fld_LadangID && x.fld_WlyhID == WilayahId).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>Kepada Ladang " + GetLdgDetail.fld_LdgName + ",</p>";
                    msg += "<p>Dukacita dimaklumkan, permohonan kewangan (Gaji Pekerja Buruh) telah ditolak pihak wilayah sila hubungi pihak wilayah dan buat semakkan kembali. Keterangan seperti dibawah :-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Nama Ladang</th><th>Kod Ladang</th><th>Jumlah Permohonan(RM)</th><th>Ditolak Oleh</th><th>Waktu Ditolak</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + GetLdgDetail.fld_LdgName + "</td><td align=\"center\">" + GetLdgDetail.fld_LdgCode + "</td><td align=\"center\">" + JumlahWang + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var emailbcclist2 = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailbcclist2 != null)
                    {
                        foreach (var bccemail in emailbcclist2)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    SendEmailNotification.SendEmail(subject, msg, GetLdgDetail.fld_LdgEmail, cc, bcc);

                    break;
            }
            return Json(new { DescStatus = DescStatus, ActionBy = ActionBy, getdatetime = getdatetime });
        }

        public JsonResult AcknowlagdeEmail(int NegaraId, int SyarikatId, int WilayahId, int Month, int Year)
        {
            string msg2 = "";
            string statusmsg = "";
            bool status = false;
            int getuserid = getidentity.ID(User.Identity.Name);
            string ActionBy = GetIdentity.MyNameFullName(getuserid);
            DateTime getdatetime = timezone.gettimezone();
            var getcoundata = db.vw_PermohonanWang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId && x.fld_Month == Month && x.fld_Year == Year && x.fld_SemakWil_Status == 1).Count();
            if (getcoundata > 0)
            {
                try
                {
                    string[] cc = new string[] { };
                    List<string> cclist = new List<string>();
                    string[] bcc = new string[] { };
                    List<string> bcclist = new List<string>();
                    string subject = "";
                    string msg = "";
                    string NamaWilayah = getwilyah.GetWilayahName(WilayahId);
                    var getJumlahKeseluruhanPermohonan = db.vw_PermohonanWang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId && x.fld_Month == Month && x.fld_Year == Year).Select(s => s.fld_JumlahPermohonan).Sum();

                    subject = "Sokongan Permohonan Wang";

                    var GetSentToGM = db2.tblUsers.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == WilayahId && x.fldRoleID == 4).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>Tn/Pn " + GetSentToGM.fldUserShortName + ",</p>";
                    msg += "<p>Sokongan permohonan kewangan (Gaji Pekerja Buruh) untuk kelulusan diperlukan dari pihak Tn/Pn. Keterangan seperti dibawah:-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Nama Wilayah</th><th>Jumlah Permohonan(RM)</th><th>Disemak Oleh</th><th>Waktu Disemak</th><th>Pautan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + NamaWilayah + "</td><td align=\"center\">" + getJumlahKeseluruhanPermohonan + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td><td><a href=\"" + Url.Action("ApplicationSupportRegionGm", "ApplicationSupport", null, "http") + "\">Klik ke pautan sokongan</a></td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var emailbcclist1 = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailbcclist1 != null)
                    {
                        foreach (var bccemail in emailbcclist1)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    if (SendEmailNotification.SendEmail(subject, msg, GetSentToGM.fldUserEmail, cc, bcc))
                    {
                        msg2 = "Email telah dihantar kepada GM Wilayah " + NamaWilayah;
                        statusmsg = "success";
                        status = true;
                    }
                    else
                    {
                        msg2 = "Email gagal dihantar kepada GM Wilayah " + NamaWilayah;
                        statusmsg = "warning";
                        status = true;
                    }
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    msg2 = "Email gagal untuk dihantar";
                    statusmsg = "warning";
                    status = true;
                }
            }
            else
            {
                msg2 = "Sila buat semakan terlebih dahulu sebelum email dihantar kepada GM Wilayah";
                statusmsg = "warning";
                status = true;
            }
            return Json(new { msg = msg2, statusmsg = statusmsg, status = status });
        }

        public JsonResult UpdateDataGM(int DataID, string UpdateFlag, int Month, int Year, int NegaraId, int SyarikatId, int WilayahId)
        {
            string DescStatus = "";
            int getuserid = getidentity.ID(User.Identity.Name);
            string ActionBy = GetIdentity.MyNameFullName(getuserid);
            string NamaWilayah = getwilyah.GetWilayahName(WilayahId);
            string subject = "";
            string msg = "";
            string[] cc = new string[] { };
            List<string> cclist = new List<string>();
            string[] bcc = new string[] { };
            List<string> bcclist = new List<string>();
            DateTime getdatetime = timezone.gettimezone();
            var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_WilayahID == DataID && x.fld_Month == Month && x.fld_Year == Year).Select(s=>s.fld_ID).ToList();
            var getJumlahKeseluruhanPermohonan = db.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId && x.fld_Month == Month && x.fld_Year == Year).Select(s => s.fld_JumlahPermohonan).Sum();
            switch (UpdateFlag)
            {
                case "SokongGMWil":
                    DescStatus = "Telah Disokong";
                    DatabaseAction.UpdateDataTotblSokPermhnWangGM(DataID,UpdateFlag,Month,Year, getuserid, getdatetime);
                    foreach (var getdataid in getdata)
                    {
                        DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, getdataid, 2);
                    }

                    subject = "Kelulusan Permohonan Wang";

                    var GetSentToWil1 = db2.tbl_Wilayah.Where(x => x.fld_ID == DataID && x.fld_SyarikatID == SyarikatId).FirstOrDefault();
                    var GetSentToHQ = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldCategory == GetSentToWil1.fld_ApprovalZone && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();
                    var GetSentCC = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldCategory == "CCFINANCE" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>Pihak HQ,</p>";
                    msg += "<p>Mohon kelulusan permohonan kewangan (Gaji Pekerja Buruh) dari pihak HQ. Keterangan seperti dibawah:-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Nama Wilayah</th><th>Jumlah Permohonan(RM)</th><th>Disokong Oleh</th><th>Waktu Sokongan</th><th>Pautan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + NamaWilayah + "</td><td align=\"center\">" + getJumlahKeseluruhanPermohonan + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td><td align=\"center\"><a href=\"" + Url.Action("ApplicationSupportRegionHQ", "ApplicationSupport", null, "http") + "\">Klik ke pautan sokongan</a></td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var emailbcclist1 = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailbcclist1 != null)
                    {
                        foreach (var bccemail in emailbcclist1)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    cclist.Add(GetSentToWil1.fld_WlyhEmail);
                    cclist.Add(GetSentCC.fldEmail);

                    cc = cclist.ToArray();

                    SendEmailNotification.SendEmail(subject, msg, GetSentToHQ.fldEmail, cc, bcc);

                    break;
                case "TolakGMWil":
                    DescStatus = "Telah Ditolak";
                    DatabaseAction.UpdateDataTotblSokPermhnWangGM(DataID, UpdateFlag, Month, Year, getuserid, getdatetime);
                    foreach (var getdataid in getdata)
                    {
                        DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, getdataid, 2);
                    }

                    subject = "Penolakkan Permohonan Wang";

                    var GetSentToWil = db2.tbl_Wilayah.Where(x => x.fld_ID == WilayahId && x.fld_SyarikatID == SyarikatId).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    //msg += "<p>Pihak HQ,</p>";
                    msg += "<p>Dukacita dimaklumkan, permohonan kewangan (Gaji Pekerja Buruh) telah ditolak oleh GM wilayah sila hubungi GM wilayah untuk keterangan dan buat semakkan kembali. Keterangan seperti dibawah :-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Nama Wilayah</th><th>Jumlah Permohonan(RM)</th><th>Ditolak Oleh</th><th>Waktu Ditolak</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + NamaWilayah + "</td><td align=\"center\">" + getJumlahKeseluruhanPermohonan + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var emailbcclist2 = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailbcclist2 != null)
                    {
                        foreach (var bccemail in emailbcclist2)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    SendEmailNotification.SendEmail(subject, msg, GetSentToWil.fld_WlyhEmail, cc, bcc);
                    break;
            }
            return Json(new { DescStatus = DescStatus, ActionBy = ActionBy, getdatetime = getdatetime });
        }

        public JsonResult UpdateDataHQ(int DataID, string UpdateFlag, int Month, int Year, int NegaraId, int SyarikatId)
        {
            string DescStatus = "";
            int getuserid = getidentity.ID(User.Identity.Name);
            string ActionBy = GetIdentity.MyNameFullName(getuserid);
            string subject = "";
            string msg = "";
            string[] cc = new string[] { };
            List<string> cclist = new List<string>();
            string[] bcc = new string[] { };
            List<string> bcclist = new List<string>();
            DateTime getdatetime = timezone.gettimezone();
            string NamaWilayah = getwilyah.GetWilayahName(DataID);
            var getdata = db.tbl_SokPermhnWang.Where(x => x.fld_WilayahID == DataID && x.fld_Month == Month && x.fld_Year == Year).Select(s => s.fld_ID).ToList();
            var getJumlahKeseluruhanPermohonan = db.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == DataID && x.fld_Month == Month && x.fld_Year == Year).Select(s => s.fld_JumlahPermohonan).Sum();
            switch (UpdateFlag)
            {
                case "TerimaHQ":
                    DescStatus = "Telah Diterima";
                    DatabaseAction.UpdateDataTotblSokPermhnWangHQ(DataID, UpdateFlag, Month, Year, getuserid, getdatetime);
                    foreach (var getdataid in getdata)
                    {
                        DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, getdataid, 2);
                    }

                    subject = "Kelulusan Permohonan Wang";

                    var GetSentToWil = db2.tbl_Wilayah.Where(x => x.fld_ID == DataID && x.fld_SyarikatID == SyarikatId).FirstOrDefault();
                    var GetSentToGM = db2.tblUsers.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == DataID && x.fldRoleID == 4).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>Sukacita dimaklumkan, permohonan kewangan (Gaji Pekerja Buruh) telah diluluskan oleh pihak HQ. Keterangan seperti dibawah:-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Nama Wilayah</th><th>Jumlah Permohonan(RM)</th><th>Diluluskan Oleh</th><th>Waktu Diluluskan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + NamaWilayah + "</td><td align=\"center\">" + getJumlahKeseluruhanPermohonan + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var emailbcclist1 = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailbcclist1 != null)
                    {
                        foreach (var bccemail in emailbcclist1)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    cclist.Add(GetSentToGM.fldUserEmail);

                    cc = cclist.ToArray();

                    SendEmailNotification.SendEmail(subject, msg, GetSentToWil.fld_WlyhEmail, cc, bcc);

                    break;
                case "TolakHQ":
                    DescStatus = "Telah Ditolak";
                    DatabaseAction.UpdateDataTotblSokPermhnWangHQ(DataID, UpdateFlag, Month, Year, getuserid, getdatetime);
                    foreach (var getdataid in getdata)
                    {
                        DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, getdataid, 2);
                    }
                    subject = "Penolakkan Permohonan Wang";

                    var GetSentToWil2 = db2.tbl_Wilayah.Where(x => x.fld_ID == DataID && x.fld_SyarikatID == SyarikatId).FirstOrDefault();
                    var GetSentToGM2 = db2.tblUsers.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == DataID && x.fldRoleID == 4).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>>Dukacita dimaklumkan, permohonan kewangan (Gaji Pekerja Buruh) telah ditolak oleh pihak HQ, sila hubungi pihak HQ untuk keterangan dan buat semakkan kembali. Keterangan seperti dibawah:-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Nama Wilayah</th><th>Jumlah Permohonan(RM)</th><th>Tindakan Oleh</th><th>Waktu Tindakan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + NamaWilayah + "</td><td align=\"center\">" + getJumlahKeseluruhanPermohonan + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var emailbcclist2 = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailbcclist2 != null)
                    {
                        foreach (var bccemail in emailbcclist2)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    cclist.Add(GetSentToGM2.fldUserEmail);

                    cc = cclist.ToArray();

                    SendEmailNotification.SendEmail(subject, msg, GetSentToWil2.fld_WlyhEmail, cc, bcc);

                    break;
            }
            return Json(new { DescStatus = DescStatus, ActionBy = ActionBy, getdatetime = getdatetime });
        }

        public JsonResult GetLadang(int WilayahID)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID, getuserid, User.Identity.Name);

            if (getwilyah.GetAvailableWilayah(SyarikatID))
            {
                if (WilayahID == 0)
                {
                    ladanglist = new SelectList(db2.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L  == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db2.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                db2.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}