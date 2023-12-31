﻿using MVC_SYSTEM.Class;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.log;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.AuthModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using MVC_SYSTEM.ModelsCorporate;

namespace MVC_SYSTEM.Controllers
{
    public class GeneralConfigsController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        GetConfig GetConfig = new GetConfig();
        GetIdentity GetIdentity = new GetIdentity();
        GetNSWL GetNSWL = new GetNSWL();
        GetWilayah GetWilayah = new GetWilayah();
        GetLadang GetLadang = new GetLadang();
        errorlog geterror = new errorlog();
        // GET: GeneralConfigs
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult Index(string filter = "", int fldUserID = 0, int page = 1, string sort = "fldUserName", string sortdir = "DESC")
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? getroleid = GetIdentity.getRoleID(getuserid);
            int?[] reportid = new int?[] { };
            ViewBag.GeneralConfig = "class = active";
            ViewBag.MaintenanceList = new SelectList(db.tblMaintenanceLists.Where(x => x.fldDeleted == false).OrderBy(o => o.fldID).Select(s => new SelectListItem { Value = s.fldID.ToString(), Text = s.fldName }), "Value", "Text").ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult Index(int MaintenanceList)
        {
            string action = "", controller = "";
            var getentry = db.tblMaintenanceLists.Find(MaintenanceList);

            action = getentry.fldAction;
            controller = getentry.fldController;

            return RedirectToAction(action, controller, new { id = MaintenanceList });
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult GeneralConfig(string filter = "", int fldUserID = 0, int page = 1, string sort = "fldOptConfValue", string sortdir = "ASC", int id = 0)
        {

            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;

            ViewBag.DataEntry = "class = active";
            ViewBag.Dropdown2 = "dropdown open active";
            ViewBag.filter = filter;
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tblOptionConfigsWeb>();
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            int flag = id;
            string type = "";
            switch (flag)
            {
                case 1:
                    type = "monthlist";
                    break;
                case 3:
                    type = "cuti";
                    break;
                case 4:
                    type = "jantina";
                    break;
                case 5:
                    type = "kdhPengiraan";
                    break;
                case 6:
                    type = "krytnlist";
                    break;
            }

            if (filter == "")
            {
                records.Content = dbview.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == type && x.fldDeleted == false)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                records.TotalRecords = dbview.tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == type && x.fldDeleted == false).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            else
            {
                records.Content = dbview.tblOptionConfigsWeb.Where(x => (x.fldOptConfValue.Contains(filter) || x.fldOptConfDesc.Contains(filter)) && (x.fldOptConfFlag1 == type && x.fldDeleted == false))
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                records.TotalRecords = dbview.tblOptionConfigsWeb.Where(x => (x.fldOptConfValue.Contains(filter) || x.fldOptConfDesc.Contains(filter)) && (x.fldOptConfFlag1 == type && x.fldDeleted == false)).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            ViewBag.Flag1 = type;
            return View(records);
        }

        public ActionResult ConfigInsert(string flag1)
        {
            ViewData["Flag1"] = flag1;
            return PartialView("ConfigInsert");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfigInsert(string flag1, ModelsCorporate.tblOptionConfigsWeb tblOptionConfigsWeb)
        {

            //if (ModelState.IsValid)
            //{
            try
            {
                var checkdata = db.tblOptionConfigsWebs.Where(x => x.fldOptConfValue == tblOptionConfigsWeb.fldOptConfValue && x.fldOptConfFlag1 == flag1).FirstOrDefault();
                if (checkdata == null)
                {
                    tblOptionConfigsWeb.fldOptConfFlag1 = flag1;
                    tblOptionConfigsWeb.fldDeleted = false;
                    db.tblOptionConfigsWebs.Add(tblOptionConfigsWeb);
                    db.SaveChanges();
                    var getid = db.tblOptionConfigsWebs.Where(w => w.fldOptConfID == tblOptionConfigsWeb.fldOptConfID).FirstOrDefault();
                    return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = tblOptionConfigsWeb.fldOptConfID, data2 = flag1, data3 = "" });
                }
                else
                {
                    return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }
            //}
            //else
            //{
            //    return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            //}
        }

        public ActionResult ConfigUpdate(int? id)
        {

            if (id == null)
            {
                return RedirectToAction("GeneralConfig");
            }
            ModelsCorporate.tblOptionConfigsWeb tblOptionConfigsWeb = db.tblOptionConfigsWebs.Where(w => w.fldOptConfID == id && w.fldDeleted == false).FirstOrDefault();
            if (tblOptionConfigsWeb == null)
            {
                return RedirectToAction("GeneralConfig");
            }
            return PartialView("ConfigUpdate", tblOptionConfigsWeb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfigUpdate(int id, Models.tblOptionConfigsWeb tblOptionConfigsWeb)
        {
                try
                {
                    var getdata = db.tblOptionConfigsWebs.Where(w => w.fldOptConfID == id && w.fldDeleted == false).FirstOrDefault();
                    getdata.fldOptConfDesc = tblOptionConfigsWeb.fldOptConfDesc;

                    db.Entry(getdata).State = EntityState.Modified;
                    db.SaveChanges();
                    var getid = id;
                    return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
        }

        public ActionResult ConfigDelete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("GeneralConfig");
            }
            ModelsCorporate.tblOptionConfigsWeb tblOptionConfigsWeb = db.tblOptionConfigsWebs.Where(w => w.fldOptConfID == id && w.fldDeleted == false).FirstOrDefault();
            if (tblOptionConfigsWeb == null)
            {
                return RedirectToAction("GeneralConfig");
            }
            return PartialView("ConfigDelete", tblOptionConfigsWeb);
        }

        [HttpPost, ActionName("ConfigDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfigDeleteConfirmed(int id)
        {
            try
            {
                ModelsCorporate.tblOptionConfigsWeb tblOptionConfigsWeb = db.tblOptionConfigsWebs.Find(id);
                if (tblOptionConfigsWeb == null)
                {
                    return Json(new { success = true, msg = "Data already deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                else
                {
                    db.tblOptionConfigsWebs.Remove(tblOptionConfigsWeb);
                    db.SaveChanges();
                    return Json(new { success = true, msg = "Data successfully deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult EstateDetail(int WilayahIDList = 0, int fldUserID = 0, int page = 1, string sort = "fld_WlyhID", string sortdir = "ASC")
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            //int? wilayahselection = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            //List<SelectListItem> LadangIDList = new List<SelectListItem>();
            if (WilayahID == 0)
            {
                wlyhid = GetWilayah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList2 = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }
            else
            {
                //WilayahIDList = WilayahID;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList2 = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
                //WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            }




            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            ViewBag.GeneralConfig = "class = active";
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_Ladang>();

            if (WilayahIDList == 0)
            {
                records.Content = dbview.tbl_Ladang
                       .OrderBy(sort + " " + sortdir)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();

                records.TotalRecords = dbview.tbl_Ladang.Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            else
            {
                records.Content = dbview.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList)
                       .OrderBy(sort + " " + sortdir)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();

                records.TotalRecords = dbview.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            ViewBag.WilayahIDList = WilayahIDList2;
            return View(records);
        }


        public ActionResult EstateDetailUpdate(string id, int? wlyh)
        {

            if (id == null)
            {
                return RedirectToAction("EstateDetail");
            }
            ModelsCorporate.tbl_Ladang tbl_Ladang = db.tbl_Ladang.Where(w => w.fld_WlyhID == wlyh && w.fld_LdgCode == id).FirstOrDefault();
            if (tbl_Ladang == null)
            {
                return RedirectToAction("EstateDetail");
            }

            return PartialView("EstateDetailUpdate", tbl_Ladang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EstateDetailUpdate(string id, int wlyh, Models.tbl_Ladang tbl_Ladang)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var getdata = db.tbl_Ladang.Where(w => w.fld_LdgCode == id && w.fld_WlyhID == wlyh).FirstOrDefault();

                    getdata.fld_LdgEmail = tbl_Ladang.fld_LdgEmail;
                    getdata.fld_NoAcc = tbl_Ladang.fld_NoAcc;
                    getdata.fld_NoGL = tbl_Ladang.fld_NoGL;
                    getdata.fld_NoCIT = tbl_Ladang.fld_NoCIT;

                    db.Entry(getdata).State = EntityState.Modified;
                    db.SaveChanges();
                    var getid = id;
                    return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult EstateDetailInsert()
        {
            //int drpyear = 0;
            //int drprangeyear = 0;
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int[] wlyhid = new int[] { };
            //string mywlyid = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = GetWilayah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            }

            ViewBag.fld_WlyhID = WilayahIDList;
            return PartialView("EstateDetailInsert");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EstateDetailInsert(ModelsCorporate.tbl_Ladang tbl_Ladang)
        {
            //GetLadang GetLadang = new GetLadang();
            if (ModelState.IsValid)
            {
                try
                {
                    var checkdata = db.tbl_Ladang.Where(x => x.fld_WlyhID == tbl_Ladang.fld_WlyhID && x.fld_LdgCode == tbl_Ladang.fld_LdgCode).FirstOrDefault();
                    if (checkdata == null)
                    {
                        tbl_Ladang.fld_Deleted = false;
                        db.tbl_Ladang.Add(tbl_Ladang);
                        db.SaveChanges();
                        var getid = db.tbl_Ladang.Where(w => w.fld_ID == tbl_Ladang.fld_ID).FirstOrDefault();
                        return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = tbl_Ladang.fld_ID, data2 = tbl_Ladang.fld_ID, data3 = "" });
                    }
                    else
                    {
                        return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
                    }

                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult EstateDetailDelete(string id, int? wlyh)
        {
            if (id == null)
            {
                return RedirectToAction("EstateDetail");
            }
            ModelsCorporate.tbl_Ladang tbl_Ladang = db.tbl_Ladang.Where(w => w.fld_WlyhID == wlyh && w.fld_LdgCode == id).FirstOrDefault();
            if (tbl_Ladang == null)
            {
                return RedirectToAction("EstateDetail");
            }
            return PartialView("EstateDetailDelete", tbl_Ladang);
        }

        [HttpPost, ActionName("EstateDetailDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult EstateDetailDeleteConfirmed(string id, int? wlyh)
        {
            try
            {
                ModelsCorporate.tbl_Ladang tbl_Ladang = db.tbl_Ladang.Where(w => w.fld_WlyhID == wlyh && w.fld_LdgCode == id).FirstOrDefault();
                if (tbl_Ladang == null)
                {
                    return Json(new { success = true, msg = "Data already deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                else
                {
                    db.tbl_Ladang.Remove(tbl_Ladang);
                    db.SaveChanges();
                    return Json(new { success = true, msg = "Data successfully deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }

        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult CompanyDetail()
        {
            var record = db.tbl_Syarikat.Where(x => x.fld_NegaraID == 1).FirstOrDefault();
            return View(record);
        }

        public ActionResult CompanyDetailUpdate()
        {
            ModelsCorporate.tbl_Syarikat tbl_Syarikat = db.tbl_Syarikat.Where(x => x.fld_NegaraID == 1).FirstOrDefault();
            return PartialView("CompanyDetailUpdate", tbl_Syarikat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult CompanyDetailUpdate(Models.tbl_Syarikat tbl_Syarikat)  //comment by wani 22.3.2021
        public ActionResult CompanyDetailUpdate(ModelsCorporate.tbl_Syarikat tbl_Syarikat)  //add by wani 22.3.2021
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var getdata = db.tbl_Syarikat.Where(w => w.fld_NegaraID == 1).FirstOrDefault();

                    getdata.fld_NamaSyarikat = tbl_Syarikat.fld_NamaSyarikat;
                    getdata.fld_NamaPndkSyarikat = tbl_Syarikat.fld_NamaPndkSyarikat;
                    getdata.fld_NoSyarikat = tbl_Syarikat.fld_NoSyarikat;
                    getdata.fld_SyarikatEmail = tbl_Syarikat.fld_SyarikatEmail;

                    db.Entry(getdata).State = EntityState.Modified;
                    db.SaveChanges();
                    var getid = 0;
                    return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            }
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult AdditionalEmail(string filter = "", int page = 1, string sort = "fldName", string sortdir = "ASC")
        {
            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tblEmailList>();

            if (filter == "")
            {
                records.Content = dbview.tblEmailList.Where(x => x.fldDeleted == false)
                       .OrderBy(sort + " " + sortdir)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();

                records.TotalRecords = dbview.tblEmailList.Where(x => x.fldDeleted == false).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            else
            {
                int wlyhid = int.Parse(filter);
                records.Content = dbview.tblEmailList.Where(x => (x.fldName.Contains(filter) || x.fldEmail.Contains(filter)) && (x.fldDeleted == false))
                       .OrderBy(sort + " " + sortdir)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();

                records.TotalRecords = dbview.tblEmailList.Where(x => (x.fldName.Contains(filter) || x.fldEmail.Contains(filter)) && (x.fldDeleted == false)).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            return View(records);
        }

        public ActionResult AdditionalEmailInsert()
        {

            List<SelectListItem> Category = new List<SelectListItem>();
            Category.Insert(0, (new SelectListItem { Text = "BCC", Value = "BCC" }));
            Category.Insert(1, (new SelectListItem { Text = "CC", Value = "CC" }));
            //Fitri add 23-12-2020
            ViewBag.fldDepartment = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldDeleted == false && x.fldOptConfFlag1 == "department"), "fldOptConfValue", "fldOptConfValue");

            int negaraid = 0;

            ViewBag.fldNegaraID = new SelectList(db2.tbl_Negara.Where(x => x.fld_Deleted == false), "fld_NegaraID", "fld_NamaNegara");
            negaraid = db2.tbl_Negara.Where(x => x.fld_Deleted == false).Select(s => s.fld_NegaraID).Take(1).FirstOrDefault();
            ViewBag.fldSyarikatID = new SelectList(db.tbl_Syarikat.Where(x => x.fld_NegaraID == negaraid && x.fld_Deleted == false), "fld_SyarikatID", "fld_NamaSyarikat");
            ViewBag.fldCategory = Category;
            //ViewBag.fld_WlyhID = WilayahIDList;
            return PartialView("AdditionalEmailInsert");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdditionalEmailInsert(ModelsCorporate.tblEmailList tblEmailList)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var checkdata = db.tblEmailLists.Where(x => x.fldName == tblEmailList.fldName).FirstOrDefault();
                    if (checkdata == null)
                    {
                        //string category = "";
                        //if (CategoryList == 0)
                        //{
                        //    category = "BCC";
                        //}
                        //else
                        //{
                        //    category = "CC";
                        //}
                        //tblEmailList.fldCategory = category;
                        tblEmailList.fldDeleted = false;
                        db.tblEmailLists.Add(tblEmailList);
                        db.SaveChanges();
                        var getid = db.tblEmailLists.Where(w => w.fldName == tblEmailList.fldName).FirstOrDefault();
                        return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = tblEmailList.fldName, data2 = tblEmailList.fldName, data3 = "" });
                    }
                    else
                    {
                        return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
                    }

                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult AdditionalEmailUpdate(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("AdditionalEmail");
            }
            ModelsCorporate.tblEmailList tblEmailList = db.tblEmailLists.Find(id);
            if (tblEmailList == null)
            {
                return RedirectToAction("AdditionalEmail");
            }

            //Fitri add 23-12-2020
            ViewBag.fldDepartment = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldDeleted == false && x.fldOptConfFlag1 == "department"), "fldOptConfValue", "fldOptConfValue", tblEmailList.fldDepartment);
            return PartialView("AdditionalEmailUpdate", tblEmailList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdditionalEmailUpdate(int id, Models.tblEmailList tblEmailList)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var getdata = db.tblEmailLists.Find(id);
                    getdata.fldEmail = tblEmailList.fldEmail;
                    getdata.fldDepartment = tblEmailList.fldDepartment; //Fitri add 23-12-2020
                    db.Entry(getdata).State = EntityState.Modified;
                    db.SaveChanges();
                    var getid = id;
                    return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult AdditionalEmailDelete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("AdditionalEmail");
            }
            ModelsCorporate.tblEmailList tblEmailList = db.tblEmailLists.Find(id);
            if (tblEmailList == null)
            {
                return RedirectToAction("AdditionalEmail");
            }
            return PartialView("AdditionalEmailDelete", tblEmailList);
        }

        [HttpPost, ActionName("AdditionalEmailDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult AdditionalEmailDeleteConfirmed(int id)
        {
            try
            {
                ModelsCorporate.tblEmailList tblEmailList = db.tblEmailLists.Find(id);
                if (tblEmailList == null)
                {
                    return Json(new { success = true, msg = "Data already deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                else
                {
                    db.tblEmailLists.Remove(tblEmailList);
                    db.SaveChanges();
                    return Json(new { success = true, msg = "Data successfully deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult Supplier(int page = 1, string sort = "fld_KodPbkl", string sortdir = "ASC", int id = 0)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int[] wlyhid = new int[] { };
            //string mywlyid = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            wlyhid = GetWilayah.GetWilayahID(SyarikatID);
            //mywlyid = String.Join("", wlyhid); ;
            WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
            WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            var wlyhID = db.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
            LadangIDList = new SelectList(db.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));

            //if (WilayahID == 0 && LadangID == 0)
            //{
            //    wlyhid = GetWilayah.GetWilayahID(SyarikatID);
            //    //mywlyid = String.Join("", wlyhid); ;
            //    WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => mywlyid.Contains(x.fld_ID.ToString())).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
            //    WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    var wlyhID = db.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).Take(1).FirstOrDefault();
            //    LadangIDList = new SelectList(db.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //    LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    //selectionrpt = 1;
            //    //wilayahselection = WilayahID;
            //    //ladangselection = LadangID;
            //    //incldg = 1;
            //    //getflag = 1;
            //}
            //else if (WilayahID != 0 && LadangID == 0)
            //{
            //    //mywlyid = String.Join("", WilayahID); ;
            //    WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => mywlyid.Contains(x.fld_ID.ToString())), "fld_ID", "fld_WlyhName").ToList();
            //    LadangIDList = new SelectList(db.tbl_Ladang.Where(x => mywlyid.Contains(x.fld_WlyhID.ToString()) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResReport.sltAll, Value = "0" }));
            //    //selectionrpt = 2;
            //    //wilayahselection = WilayahID;
            //    //ladangselection = LadangID;
            //    //incldg = 2;
            //    //getflag = 1;

            //}
            //else if (WilayahID != 0 && LadangID != 0)
            //{
            //    //mywlyid = String.Join("", WilayahID); ;
            //    WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => mywlyid.Contains(x.fld_ID.ToString())), "fld_ID", "fld_WlyhName").ToList();
            //    LadangIDList = new SelectList(db.tbl_Ladang.Where(x => mywlyid.Contains(x.fld_WlyhID.ToString()) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    //selectionrpt = 3;
            //    //wilayahselection = WilayahID;
            //    //ladangselection = LadangID;
            //    //incldg = 3;
            //    //getflag = 1;
            //}

            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            ViewBag.GeneralConfig = "class = active";
            ViewBag.Dropdown2 = "dropdown open active";
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_Pembekal>();
            if (GetIdentity.MySuperAdmin(User.Identity.Name))
            {
                records.Content = dbview.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = dbview.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }

            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            return View(records);
        }

        public ActionResult SupplierDetail(string filter = "", int fldUserID = 0, int page = 1, string sort = "fld_KodPbkl", string sortdir = "ASC", int id = 0)
        {

            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            ViewBag.User = "class = active";
            ViewBag.Dropdown2 = "dropdown open active";
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_Pembekal>();
            ViewBag.filter = filter;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (filter == "")
            {
                if (GetIdentity.MySuperAdmin(User.Identity.Name))
                {
                    records.Content = dbview.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbview.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else
                {
                    records.Content = dbview.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbview.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
            }
            else
            {
                if (GetIdentity.MySuperAdmin(User.Identity.Name))
                {
                    records.Content = dbview.tbl_Pembekal.Where(x => (x.fld_KodPbkl.Contains(filter) || x.fld_NamaPbkl.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                    records.TotalRecords = dbview.tbl_Pembekal.Where(x => (x.fld_KodPbkl.Contains(filter) || x.fld_NamaPbkl.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else
                {
                    records.Content = dbview.tbl_Pembekal.Where(x => (x.fld_KodPbkl.Contains(filter) || x.fld_NamaPbkl.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                    records.TotalRecords = dbview.tbl_Pembekal.Where(x => (x.fld_KodPbkl.Contains(filter) || x.fld_NamaPbkl.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
            }
            return View(records);
        }

        public ActionResult SupplierDetailInsert()
        {
            int negaraid = 0;

            ViewBag.fld_NegaraID = new SelectList(db2.tbl_Negara.Where(x => x.fld_Deleted == false), "fld_NegaraID", "fld_NamaNegara");
            negaraid = db2.tbl_Negara.Where(x => x.fld_Deleted == false).Select(s => s.fld_NegaraID).Take(1).FirstOrDefault();
            ViewBag.fld_SyarikatID = new SelectList(db.tbl_Syarikat.Where(x => x.fld_NegaraID == negaraid && x.fld_Deleted == false), "fld_SyarikatID", "fld_NamaSyarikat");

            return PartialView("SupplierDetailInsert");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SupplierDetailInsert(ModelsCorporate.tbl_Pembekal tbl_Pembekal)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var checkdata = db.tbl_Pembekal.Where(x => x.fld_KodPbkl == tbl_Pembekal.fld_KodPbkl).FirstOrDefault();
                    if (checkdata == null)
                    {
                        tbl_Pembekal.fld_NamaPbkl = tbl_Pembekal.fld_NamaPbkl.ToUpper();
                        tbl_Pembekal.fld_NegaraID = tbl_Pembekal.fld_NegaraID;
                        tbl_Pembekal.fld_SyarikatID = tbl_Pembekal.fld_SyarikatID;
                        tbl_Pembekal.fld_Deleted = false;
                        db.tbl_Pembekal.Add(tbl_Pembekal);
                        db.SaveChanges();
                        var getid = db.tbl_Pembekal.Where(w => w.fld_KodPbkl == tbl_Pembekal.fld_KodPbkl).FirstOrDefault();
                        return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = tbl_Pembekal.fld_KodPbkl, data2 = tbl_Pembekal.fld_KodPbkl, data3 = "" });
                    }
                    else
                    {
                        return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
                    }

                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult SupplierDetailUpdate(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("SupplierDetail");
            }
            ModelsCorporate.tbl_Pembekal tbl_Pembekal = db.tbl_Pembekal.Find(id);
            if (tbl_Pembekal == null)
            {
                return RedirectToAction("SupplierDetail");
            }

            ViewBag.NegaraList = new SelectList(db2.tbl_Negara.Where(x => x.fld_Deleted == false && x.fld_NegaraID == tbl_Pembekal.fld_NegaraID), "fld_NegaraID", "fld_NamaNegara");
            ViewBag.SyarikatList = new SelectList(db.tbl_Syarikat.Where(x => x.fld_NegaraID == tbl_Pembekal.fld_NegaraID && x.fld_Deleted == false), "fld_SyarikatID", "fld_NamaSyarikat");
            return PartialView("SupplierDetailUpdate", tbl_Pembekal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SupplierDetailUpdate(int id, Models.tbl_Pembekal tbl_Pembekal)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var getdata = db.tbl_Pembekal.Find(id);
                    //getdata.fldUserPassword = crypto.Encrypt(tblUser.fldUserPassword);
                    getdata.fld_NamaPbkl = tbl_Pembekal.fld_NamaPbkl;

                    db.Entry(getdata).State = EntityState.Modified;
                    db.SaveChanges();
                    var getid = id;
                    return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult SupplierDetailDelete(int? id)
        {
            string negara = "";
            string syarikat = "";

            if (id == null)
            {
                return RedirectToAction("SupplierDetail");
            }
            ModelsCorporate.tbl_Pembekal tbl_Pembekal = db.tbl_Pembekal.Find(id);
            if (tbl_Pembekal == null)
            {
                return RedirectToAction("SupplierDetail");
            }
            else
            {
                negara = db2.tbl_Negara.Where(x => x.fld_NegaraID == tbl_Pembekal.fld_NegaraID).Select(s => s.fld_NamaNegara).FirstOrDefault();
                syarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == tbl_Pembekal.fld_SyarikatID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            }
            ViewBag.CountryName = negara;
            ViewBag.CompanyName = syarikat;
            return PartialView("SupplierDetailDelete", tbl_Pembekal);
        }

        [HttpPost, ActionName("SupplierDetailDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult SupplierDetailDeleteConfirmed(int id)
        {
            try
            {
                ModelsCorporate.tbl_Pembekal tbl_Pembekal = db.tbl_Pembekal.Find(id);
                if (tbl_Pembekal == null)
                {
                    return Json(new { success = true, msg = "Data already deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                else
                {
                    db.tbl_Pembekal.Remove(tbl_Pembekal);
                    db.SaveChanges();
                    return Json(new { success = true, msg = "Data successfully deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1")]
        public ActionResult Region(string filter = "", int fldUserID = 0, int page = 1, string sort = "fld_WlyhName", string sortdir = "DESC")
        {
            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            ViewBag.GeneralConfig = "class = active";
            ViewBag.Dropdown2 = "dropdown open active";
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_Wilayah>();
            ViewBag.filter = filter;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            if (filter == "")
            {
                records.Content = dbview.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = dbview.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            else
            {
                records.Content = dbview.tbl_Wilayah.Where(x => (x.fld_WlyhName.Contains(filter) || x.fld_WlyhEmail.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .OrderBy(sort + " " + sortdir)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

                records.TotalRecords = dbview.tbl_Wilayah.Where(x => (x.fld_WlyhName.Contains(filter) || x.fld_WlyhEmail.Contains(filter)) && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            return View(records);
        }

        public ActionResult RegionInsert()
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            ViewBag.fld_SyarikatID = new SelectList(db.tbl_Syarikat.Where(x => x.fld_NegaraID == NegaraID && x.fld_Deleted == false), "fld_SyarikatID", "fld_NamaSyarikat").ToList();
            return PartialView("RegionInsert");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegionInsert(ModelsCorporate.tbl_Wilayah tbl_Wilayah)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var checkdata = db.tbl_Wilayah.Where(x => x.fld_WlyhName == tbl_Wilayah.fld_WlyhName).FirstOrDefault();
                    if (checkdata == null)
                    {
                        tbl_Wilayah.fld_WlyhName = tbl_Wilayah.fld_WlyhName.ToUpper();
                        //tbl_Wilayah.fld_SyarikatID = 0;
                        //tbl_Wilayah.fldUserPassword = crypto.Encrypt(tbl_Wilayah.fldUserPassword);
                        tbl_Wilayah.fld_Deleted = false;
                        db.tbl_Wilayah.Add(tbl_Wilayah);
                        db.SaveChanges();
                        var getid = db.tbl_Wilayah.Where(w => w.fld_WlyhName == tbl_Wilayah.fld_WlyhName).FirstOrDefault();
                        return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = tbl_Wilayah.fld_WlyhName, data2 = tbl_Wilayah.fld_WlyhName, data3 = "" });
                    }
                    else
                    {
                        return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
                    }

                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult RegionUpdate(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Region");
            }
            ModelsCorporate.tbl_Wilayah tbl_Wilayah = db.tbl_Wilayah.Find(id);
            if (tbl_Wilayah == null)
            {
                return RedirectToAction("Region");
            }
            return PartialView("RegionUpdate", tbl_Wilayah);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegionUpdate(int id, Models.tbl_Wilayah tbl_Wilayah)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var getdata = db.tbl_Wilayah.Find(id);
                    getdata.fld_WlyhEmail = tbl_Wilayah.fld_WlyhEmail;
                    db.Entry(getdata).State = EntityState.Modified;
                    db.SaveChanges();
                    var getid = id;
                    return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult RegionDelete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Region");
            }
            ModelsCorporate.tbl_Wilayah tbl_Wilayah = db.tbl_Wilayah.Find(id);
            if (tbl_Wilayah == null)
            {
                return RedirectToAction("Region");
            }
            return PartialView("RegionDelete", tbl_Wilayah);
        }

        [HttpPost, ActionName("RegionDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult RegionDeleteConfirmed(int id)
        {
            try
            {
                ModelsCorporate.tbl_Wilayah tbl_Wilayah = db.tbl_Wilayah.Find(id);
                if (tbl_Wilayah == null)
                {
                    return Json(new { success = true, msg = "Data already deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                else
                {
                    db.tbl_Wilayah.Remove(tbl_Wilayah);
                    db.SaveChanges();
                    return Json(new { success = true, msg = "Data successfully deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }
        }
    }
}