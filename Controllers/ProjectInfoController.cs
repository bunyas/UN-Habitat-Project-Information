using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.EJ.Export;
using Syncfusion.JavaScript.Models;
using RecordKeeping.Models;
using Syncfusion.XlsIO;
using System.Web.Script.Serialization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Security.Claims;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

namespace RecordKeeping.Controllers
{
    public class ProjectInfoController : Controller
    {
        private UNHabitat_ProjectsEntities db = new UNHabitat_ProjectsEntities();
        // GET: ProjectInfo
        public ActionResult ProjectInfo()
        {
            db.Configuration.ProxyCreationEnabled = false;
            ViewBag.ApprovalStatus = db.A_ApprovalStatus.AsNoTracking().ToList();
            ViewBag.Country = db.A_Country.AsNoTracking().ToList();
            ViewBag.Donor = db.A_Donor.AsNoTracking().ToList();
            ViewBag.Fund = db.A_Fund.AsNoTracking().ToList();
            ViewBag.LeadOrgUnit = db.A_LeadOrgUnit.AsNoTracking().ToList();
            ViewBag.Theme = db.A_Theme.AsNoTracking().ToList();
            return View();
        }

        public ActionResult DataSourceGridEdit(DataManager dm)
        {
            ((IObjectContextAdapter)this.db).ObjectContext.CommandTimeout = 3000;
            IEnumerable data = db.UNHabitatProjects.ToList();
            int count = db.UNHabitatProjects.ToList().Count;

            DataOperations operation = new DataOperations();
            //Performing filtering operation
            if (dm.Where != null)
            {
                data = operation.PerformWhereFilter(data, dm.Where, "and");
                var filtered = (IEnumerable<object>)data;
                count = filtered.Count();
            }
            //Performing search operation
            if (dm.Search != null)
            {
                data = operation.PerformSearching(data, dm.Search);
                var searched = (IEnumerable<object>)data;
                count = searched.Count();
            }
            //Performing sorting operation
            if (dm.Sorted != null)
                data = operation.PerformSorting(data, dm.Sorted);

            //Performing paging operations
            if (dm.Skip > 0)
                data = operation.PerformSkip(data, dm.Skip);
            if (dm.Take > 0)
                data = operation.PerformTake(data, dm.Take);

            return Json(new { result = data, count = count }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult UpdateGridEdit(string action, List<UNHabitatProject> added, List<UNHabitatProject> changed, List<UNHabitatProject> deleted, int? key)
        {
            ((IObjectContextAdapter)this.db).ObjectContext.CommandTimeout = 3000;
            if (changed != null && changed.Count > 0)
            {
                foreach (var value in changed)
                {
                    var filterData = db.UNHabitatProjects.FirstOrDefault(c => c.ProjectID == value.ProjectID);
                    UNHabitatProject appoint = db.UNHabitatProjects.Single(A => A.ProjectID == value.ProjectID);
                    if (filterData != null)
                    {
                        DateTime startTime = Convert.ToDateTime(value.StartDate);
                        DateTime endTime = Convert.ToDateTime(value.EndDate);
                        appoint.StartDate = startTime;
                        appoint.EndDate = endTime;
                        appoint.ProjectTitle = value.ProjectTitle;
                        appoint.PAASCode = value.PAASCode;
                        appoint.ApprovalStatusID = value.ApprovalStatusID;
                        appoint.FundID = value.FundID;
                        appoint.PAGValue = value.PAGValue;
                        appoint.Country = value.Country;
                        appoint.LeadOrgUnitID = value.LeadOrgUnitID;
                        appoint.Theme = value.Theme;
                        appoint.Donor = value.Donor;
                        appoint.TotalExpenditure = value.TotalExpenditure;
                        appoint.TotalContribution = value.TotalContribution;
                        appoint.TotalContribution_TotalExpenditure = value.TotalContribution_TotalExpenditure;
                        appoint.TotalPSC = value.TotalPSC;
                        appoint.AddedBy = "User Name";
                        appoint.EditedBy = "User Name";
                        appoint.DateAdded = System.DateTime.Now;
                        appoint.DateEdited = System.DateTime.Now;
                    }
                    db.SaveChanges();
                }
            }
            if (deleted != null && deleted.Count > 0)
            {
                foreach (var value in deleted)
                {
                    value.DeletedRecord = 1;
                    value.DeletedBy = "User Name";
                    value.DateDeleted = System.DateTime.Now;
                    db.Entry(value).State = EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception dbEx)
                    {
                        throw (dbEx);
                    }
                }
            }
            var data = "";// new CalendarOfActivitiesModels().getAll(null, null, null, null, null, null, false);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


    }
}