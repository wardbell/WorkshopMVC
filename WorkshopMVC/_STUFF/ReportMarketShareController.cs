using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mha.Ids.Data;
using Mha.Ids.Data.Model;
using Mha.Ids.Data.Security;
using Mha.Ids.Data.Repository;
using Mha.Ids.Web.Models;
using Mha.Ids.Common;
using Mha.Ids.Web.ReportDeliveryService;
using System.Threading;
using SignalR.Infrastructure;
using SignalR.Hubs;
using SignalR;

namespace Mha.Ids.Web.Controllers
{
    [NoCache]
    [NoAsyncTimeout]
    public class ReportMarketShareController : BaseAsynchController
    {
       
        private ReportRepository reportRepository = new ReportRepository();
        private FilterRepository filterRepository = new FilterRepository();
        private UserRepository userRepository = new UserRepository();
        private UnitRepository unitRepository = new UnitRepository();
        //private UtilityFunctions utility = new SecurityUtils(); // UtilityFunctions();
        private ModuleRepository moduleRepository = new ModuleRepository();
        private IReportStorageRepository reportStorageRepository;
     //   private IdsDbEntities db = new IdsDbEntities();

        //
        // GET: /Report/
        private Guid reportRunId;
        public ViewResult Index()
        {
            //try to use the emulate user function
            Profile = IdsProfile.GetUserProfile(User.Identity.Name);
            var sessionSink = Profile.SessionObject;
            var emulatename = string.Empty;
            if (sessionSink.UserId != sessionSink.LoginUserId)
            {
                var userId = sessionSink.UserId;
                var username = userRepository.GetUserNameById(userId);
                Profile = IdsProfile.GetUserProfile(username);
                sessionSink = Profile.SessionObject;
                emulatename = username;
            }
            ViewBag.EmulateName = emulatename;

            var userLoginId = sessionSink.LoginUserId;
            var isUserHost = sessionSink.IsHost;
            var selectedUnitId = sessionSink.UnitId ?? userRepository.GetFirstUnitIdByUserId(userLoginId);

            var moduleFilter = sessionSink.ModuleFilter ?? "";
            var groupFilter = sessionSink.GroupFilter ?? "";
            var typeFilter = sessionSink.TypeFilter ?? "";

            var filterParam = new FilterSelectionViewModel()
            {
                SelectedUnitId = selectedUnitId,
                SelectedModuleFilter = moduleFilter,
                SelectedGroupFilter = groupFilter,
                SelectedTypeFilter = typeFilter
            };
            Profile.SessionObject.ControllerName = "Report";
            Profile.SessionObject.MethodName = "Index";
            Profile.Save();

            // Extracted code into a method SetFilters
            SetFilters(userLoginId, isUserHost, filterParam);

            var viewModel = reportRepository.GetReportFilteredList("R",moduleFilter, groupFilter, typeFilter, selectedUnitId, userLoginId,"");
            ViewBag.UserId = userLoginId;
            return View(viewModel.ToList());
        }

        

        [HttpPost]
        public ViewResult Index(FormCollection form) //   (Guid hiddenUserId, FormCollection form)
        {
            //try to use the emulate user function
            Profile = IdsProfile.GetUserProfile(User.Identity.Name);
            var sessionSink = Profile.SessionObject;
            var emulatename = string.Empty;
            if (sessionSink.UserId != sessionSink.LoginUserId)
            {
                var userId = sessionSink.UserId;
                var username = userRepository.GetUserNameById(userId);
                Profile = IdsProfile.GetUserProfile(username);
                sessionSink = Profile.SessionObject;
                emulatename = username;
            }
            ViewBag.EmulateName = emulatename;
            var userLoginId = Profile.SessionObject.LoginUserId;
            var isUserHost = Profile.SessionObject.IsHost;

            var selectedUnitId = form["DropDownSelectUnit"].ToString();
            var moduleFilter = form["DropDownSelectModule"] != null ? form["DropDownSelectModule"].ToString() : sessionSink.ModuleFilter;
            var groupFilter = form["DropDownSelectGroup"] != null ? form["DropDownSelectGroup"].ToString() : sessionSink.GroupFilter;
            var typeFilter = form["DropDownSelectType"] != null ? form["DropDownSelectType"].ToString() : sessionSink.TypeFilter;

            var filterParam = new FilterSelectionViewModel()
            {
                SelectedUnitId = selectedUnitId,
                SelectedModuleFilter = moduleFilter,
                SelectedGroupFilter = groupFilter,
                SelectedTypeFilter = typeFilter
            };

            SetFilters(userLoginId, isUserHost, filterParam);
            Profile.SessionObject.UnitId = selectedUnitId;

            if ((string)ViewData["IsServicedAreaDefined"] == "Defined")
            {
                Profile.SessionObject.ModuleFilter = moduleFilter;
                Profile.SessionObject.GroupFilter = groupFilter;
                Profile.SessionObject.TypeFilter = typeFilter;
            }
            Profile.SessionObject.ControllerName = "Report";
            Profile.SessionObject.MethodName = "Index";
            Profile.Save();

            var viewModel = reportRepository.GetReportFilteredList("R", moduleFilter, groupFilter, typeFilter, selectedUnitId, userLoginId,"");
            ViewBag.UserId = userLoginId;
            return View(viewModel.ToList());
        }

        /// <summary>
        /// Sets the filters, extracted from the index method.
        /// </summary>
        /// <param name="userLoginId">The user login id.</param>
        /// <param name="isUserHost">if set to <c>true</c> [is user host].</param>
        /// <param name="filterViewModel">The filter view model.</param>
        private void SetFilters(Guid userLoginId, bool isUserHost, FilterSelectionViewModel filterViewModel)
        {
            if (isUserHost)
            {
                // for host, list all the units in the system
                ViewData["UnitList"] = new SelectList(unitRepository.GeAllUnitsToList(), "ObjectId", "ObjectDescription", filterViewModel.SelectedUnitId);
                ViewData["ModuleList"] = new SelectList(moduleRepository.GetModulesByUnitId(filterViewModel.SelectedUnitId), "ObjectId", "ObjectDescription", filterViewModel.SelectedModuleFilter);
            }
            else
            {
                // for non-host, get the login user's units and modules
                ViewData["UnitList"] = new SelectList(userRepository.GetUserUnitsByUserId(userLoginId), "ObjectId", "ObjectDescription", filterViewModel.SelectedUnitId);
                ViewData["ModuleList"] = new SelectList(userRepository.GetModulesByUserIdUnitId(userLoginId, filterViewModel.SelectedUnitId), "ObjectId", "ObjectDescription", filterViewModel.SelectedModuleFilter);
            }

            ViewData["GroupList"] = new SelectList(moduleRepository.GetReportGroupsByModuleId(filterViewModel.SelectedModuleFilter), "ObjectId", "ObjectDescription", filterViewModel.SelectedGroupFilter);
            ViewData["TypeList"] = new SelectList(moduleRepository.GetReportTypesByGroupId(filterViewModel.SelectedGroupFilter), "ObjectId", "ObjectDescription", filterViewModel.SelectedTypeFilter);

            //var selectedUserId = (isUserHost) ? new System.Guid(form["DropDownSelectUser"].ToString()) : loginUserId; //hiddenUserId; // new System.Guid(form["DropDownSelectUser"].ToString());
            ViewData["UnitId"] = filterViewModel.SelectedUnitId;
            ViewData["Host"] = (isUserHost) ? "Yes" : "No";
            ViewData["UserId"] = userLoginId;
            ViewData["IsServicedAreaDefined"] = (this.filterRepository.IsServiceAreaDefined(filterViewModel.SelectedUnitId)) ? "Defined" : "NotDefined";
        }


        //
        // GET: /Report/Details/5

        public ViewResult Details(Guid id)
        {
            ReportDefinition reportdefinition =reportRepository.GetReportdefinition(id);
            return View(reportdefinition);
        }

        //
        // GET: /Report/Add --common add that redirects to edit copy

        public ActionResult AddReport(string reportTypeId,string moduleId)
        {
            Profile = IdsProfile.GetUserProfile(User.Identity.Name);
            var unitId = Profile.SessionObject.UnitId;
            var reportRepository = new ReportRepository();
            var reportTypeRecord = reportRepository.GetReportTypeById(reportTypeId);
            
            var basereport = reportTypeRecord.DefaultId;
            return RedirectToAction("Edit", new { id = basereport, unitID = unitId, mode = "Copy", reportRunGenId = new Guid(),moduleId = moduleId });
        }

        // GET: /Report/Edit --common edit that redirects to edit 

        public ActionResult EditReport(Guid reportId,string moduleId)
        {
            Profile = IdsProfile.GetUserProfile(User.Identity.Name);
            var unitId = Profile.SessionObject.UnitId;
           
            return RedirectToAction("Edit", new { id = reportId, unitID = unitId, mode = "Edit", reportRunGenId = new Guid(), moduleId = moduleId });
        }

        //
        // GET: /Report/Create

        public ActionResult Create()
        {
            FillLookups();
            return View();
        } 

        //
        // POST: /Report/Create

        [HttpPost]
        public ActionResult Create(ReportDefinition reportdefinition)
        {
            if (ModelState.IsValid)
            {
                reportdefinition.Id = Guid.NewGuid();
                reportRepository.CreateReportDefinition(reportdefinition);
                return RedirectToAction("Index");  
            }

            FillLookups(reportdefinition);
            return View(reportdefinition);
        }

        //
        // GET: /Report/Edit/5

        public ActionResult Edit(Guid id, string unitId, string mode, Guid reportRunGenId, string moduleId)
        {

            MarketShareLiveReportData marketsharelivereportdata = null;
            MarketShareReportData marketsharereportdata = null;
            //see if the viewbag has any data
            if (TempData["GeographicFilterMessage"] == null)
            {
                ViewBag.GeographicFilterMessage = "";
            }
            else
            {
                ViewBag.GeographicFilterMessage = TempData["GeographicFilterMessage"];
            }
            if (TempData["CaseFilterMessage"] == null)
            {
                ViewBag.CaseFilterMessage = "";
            }
            else
            {
                ViewBag.CaseFilterMessage = TempData["CaseFilterMessage"];
            }
            //try to use the emulate user function
            Profile = IdsProfile.GetUserProfile(User.Identity.Name);
            var sessionSink = Profile.SessionObject;
            var emulatename = string.Empty;
            if (sessionSink.UserId != sessionSink.LoginUserId)
            {
                var userId1 = sessionSink.UserId;
                var username = userRepository.GetUserNameById(userId1);
                Profile = IdsProfile.GetUserProfile(username);
                sessionSink = Profile.SessionObject;
                emulatename = username;
            }
            ViewBag.SaveMessage = "";
            ViewBag.EmulateName = emulatename;
            ViewBag.QueryMessage = "";
            ViewBag.SummaryMessage = "";
            ViewBag.InvalidReport = "";
            var userId = sessionSink.LoginUserId;
            var isUserHost = sessionSink.IsHost;
            var patientType = "I";
            string enddate;
            if (mode == "live" || mode == "liveedit" || mode == "liveupdate" || mode == "liveupdateDate" || mode == "SaveCancel" || mode == "SaveCancelDuplicateReportName")
            {

                if (mode == "live" || mode == "SaveCancel")
                {
                    var validationmsg = reportRepository.ValidateReport(id, 1);
                    var QueryMessage = validationmsg.AsQueryable().FirstOrDefault().ValidateMessage;
                    var EstimatedTime = validationmsg.AsQueryable().FirstOrDefault().EstimatedTime;
                    var ValidQuery = validationmsg.AsQueryable().FirstOrDefault().ValidRequest;
                    if (ValidQuery != true)
                    {
                        ViewBag.InvalidReport = "Invalid Report";
                    }
                    //if (QueryMessage == "" && ValidQuery == true)
                    //{
                    //    QueryMessage = "<div>Valid Request</div><div>Estimated Execution Time: "+EstimatedTime.ToString()+" seconds</div>";
                    //}
                    ViewBag.QueryMessage = QueryMessage;
                }
                
                
               marketsharelivereportdata = reportRepository.GetMarketShareLiveReportData(id);
                patientType = marketsharelivereportdata.DataType1;
                enddate = marketsharelivereportdata.EndYearMonth.ToString();
                //set to run report tab
               ViewBag.TabIndex = "4";
               if (mode == "liveedit")
               {
                   //set to filter tab
                   ViewBag.TabIndex = "2";
               }
               if (mode == "liveupdate")
               {
                   //set to filter tab
                   ViewBag.TabIndex = "0";
               }
               if (mode == "liveupdateDate")
               {
                   //set to filter tab
                   ViewBag.TabIndex = "1";
               }
               if (mode == "SaveCancel")
               {
                   //set to filter tab
                   ViewBag.TabIndex = "5";
                   ViewBag.SaveMessage = "Report was not saved because of errors";
               }
               if (mode == "SaveCancelDuplicateReportName")
               {
                   //set to filter tab
                   ViewBag.TabIndex = "5";
                   ViewBag.SaveMessage = "Report was not saved because the report name already exists for the user.";
               }
               
            }
            else
            {
               marketsharereportdata = reportRepository.GetMarketShareReportData(id);
                //not in edit mode
               patientType = marketsharereportdata.DataType1;
               enddate = marketsharereportdata.EndYearMonth.ToString();
                marketsharereportdata.UnitId = unitId;
                if (mode == "Saved")
                {
                    //set to filter tab
                    ViewBag.TabIndex = "5";
                    ViewBag.SaveMessage = "Report was saved";
                }
                else if (mode == "SavedAdd")
                {
                    //set to filter tab
                    ViewBag.TabIndex = "5";
                    ViewBag.SaveMessage = "Report was saved as a new copy";
                }
                else if (mode == "Copy")
                {
                    
                    //set to filter tab
                    ViewBag.TabIndex = "1";
                    //var moduleRepository = new ModuleRepository();
                    var module = moduleRepository.GetModuleById(moduleId);
                    patientType = module.DataTypeID.ToString().Trim();
                    marketsharereportdata.DataType1 = patientType;
                   
                    marketsharereportdata.ReportDefinition.ModuleId = moduleId;
                    //get the module to override some settings

                }
                else
                {
                    ViewBag.TabIndex = "1";
                    
                }
                //var validationmsg = reportRepository.ValidateReport(id, 0);
                //ViewBag.QueryMessage = validationmsg.AsQueryable().FirstOrDefault().ValidateMessage;
                //ViewBag.EstimatedTime = validationmsg.AsQueryable().FirstOrDefault().EstimatedTime;
                //ViewBag.ValidQuery = validationmsg.AsQueryable().FirstOrDefault().ValidRequest;

            }
            var unitname = unitRepository.GetUnitNameByUnitId(unitId);
            ViewBag.UnitName = unitname;
           // var geographicFilters = filterRepository.GetFilterByTypeAndIds(1, unitId, userId);
           // var caseFilters = filterRepository.GetFilterByTypeAndIds(2, unitId, userId);

            var geographicFilters = filterRepository.GetFilterListbyUserIdModuleId(userId,moduleId, 1);
            var caseFilters = filterRepository.GetFilterListbyUserIdModuleId(userId, moduleId, 2);
            
            
            //Guid userID = reportdefdata.UserId;
            //ti added to check for units date range
            var reportdates = reportRepository.GetModuleDates(unitId,moduleId);
            //IEnumerable<DataDictionary> dataDictionary;
            //add support for dates
            
            var calcdate = new DateTime(Convert.ToInt16(enddate.Substring(0, 4)), Convert.ToInt32(enddate.Substring(4, 2)), 1).AddMonths(1).AddDays(-1);
            var dataDict = reportRepository.GetFieldList(userId, patientType,calcdate,moduleId);
            var dataDictGeographic = reportRepository.GetGeographicFieldList(userId, patientType, calcdate, moduleId);
            var dataDictNonGeographic = reportRepository.GetNonGeographicFieldList(userId, patientType, calcdate, moduleId);
            
            var dto = MarketShareDto.Create(marketsharelivereportdata,marketsharereportdata, reportdates, userId, dataDict,dataDictNonGeographic,dataDictGeographic, geographicFilters, caseFilters,mode);
            
            ViewBag.Title = "Market Share Report";
            
            ViewBag.IsHost = isUserHost;
            SelectList unitList;
            if (isUserHost)
            {
                // for host, list all the units in the system
                unitList = new SelectList(unitRepository.GeAllUnitsToList(), "ObjectId", "ObjectDescription");
            }
            else
            {
                // for non-host, get the login user's units 
                unitList = new SelectList(userRepository.GetUserUnitsByUserId(userId), "ObjectId", "ObjectDescription");
            }
            ViewBag.UnitList = unitList;
           
            ViewBag.TypeList = new SelectList(moduleRepository.GetReportTypesByGroupId(""), "ObjectId", "ObjectDescription");

            // get user browser infor
            var browser = Request.Browser;
            string browserInfor = "Browser Capabilities\n"
                + "Type = " + browser.Type + "\n"
                + "Name = " + browser.Browser + "\n"
                + "Version = " + browser.Version + "\n"
                + "Major Version = " + browser.MajorVersion + "\n"
                + "Minor Version = " + browser.MinorVersion + "\n"
                + "Platform = " + browser.Platform + "\n"
                + "Is Beta = " + browser.Beta + "\n"
                + "Is Crawler = " + browser.Crawler + "\n"
                + "Is AOL = " + browser.AOL + "\n"
                + "Is Win16 = " + browser.Win16 + "\n"
                + "Is Win32 = " + browser.Win32 + "\n"
                + "Supports Frames = " + browser.Frames + "\n"
                + "Supports Tables = " + browser.Tables + "\n"
                + "Supports Cookies = " + browser.Cookies + "\n"
                + "Supports VBScript = " + browser.VBScript + "\n"
                + "Supports JavaScript = " +
                    browser.EcmaScriptVersion.ToString() + "\n"
                + "Supports Java Applets = " + browser.JavaApplets + "\n"
                + "Supports ActiveX Controls = " + browser.ActiveXControls
                      + "\n"
                + "Supports JavaScript Version = " +
                    browser["JavaScriptVersion"] + "\n";
            var browserType = "IE8NO";
            if (browserInfor.ToUpper().Contains("IE8"))
            {
                browserType = "IE8";
            }

            ViewBag.BrowserType = browserType;
            return View(dto);
        }

   
        //
        // POST: /Report/Edit/5

        [HttpPost]
        public ActionResult Edit(MarketShareDto model, string mode)
        {

            if (ModelState.IsValid)
            {
                var actionMethod = model.ActionMethod;
                var newMode = "liveedit";
                //check if it is just a refresh update
                if (actionMethod == "Update")
                {
                    newMode = "liveupdate";
                }
                if (actionMethod == "UpdateDate")
                {
                    newMode = "liveupdateDate";
                }

                //ViewBag.QueryMessage = QueryMessage;

                //actionMethod == "Run"

                



                bool AddRecord = false;
                //process the data    .
                Guid id = new Guid(model.ReportDefinitionsId);
                //try to use the emulate user function
                Profile = IdsProfile.GetUserProfile(User.Identity.Name);
                var sessionSink = Profile.SessionObject;
                var emulatename = string.Empty;
                if (sessionSink.UserId != sessionSink.LoginUserId)
                {
                    var userId1 = sessionSink.UserId;
                    var username = userRepository.GetUserNameById(userId1);
                    Profile = IdsProfile.GetUserProfile(username);
                    sessionSink = Profile.SessionObject;
                    emulatename = username;
                }
                ViewBag.EmulateName = emulatename;

                var userId = sessionSink.LoginUserId;
                var isUserHost = sessionSink.IsHost;



                id = System.Guid.NewGuid();

                MarketShareLiveReportData marketsharereportdata = new Mha.Ids.Data.MarketShareLiveReportData();



                marketsharereportdata.Id = id;
                marketsharereportdata.ReportDefinitionsId = new Guid(model.ReportDefinitionsId);
                marketsharereportdata.UserId = userId;

                marketsharereportdata.UnitId = model.UnitId;

                marketsharereportdata.Description = model.Description;
                marketsharereportdata.ReportTypesId = model.ReportTypesId;
                marketsharereportdata.ReportGroupsId = model.ReportGroupsId;
                marketsharereportdata.ModuleId = model.ModuleId;
                //marketsharereportdata.ReportDefinition.IsBaseReport = model.IsBaseReport;
                marketsharereportdata.IsGlobalShare = model.IsGlobalShare;
                marketsharereportdata.IsTemplateReport = model.IsTemplateReport;

                marketsharereportdata.Summary = model.Summary;
                marketsharereportdata.BaseYear = model.BaseYear;
                marketsharereportdata.YearType = model.YearType;
                marketsharereportdata.Trending = model.Trending;
                marketsharereportdata.ShowAdvancedFilter = model.ShowAdvancedFilter;
                marketsharereportdata.DatePeriodType = model.DatePeriodType;
                marketsharereportdata.DatePeriods = Convert.ToInt16(model.DatePeriods);
                marketsharereportdata.DateCalculationType = model.DateCalculationType;
                marketsharereportdata.OutputType = model.OutputType;
                marketsharereportdata.StartYearMonth = Convert.ToInt32(model.StartYearMonth);
                marketsharereportdata.EndYearMonth = Convert.ToInt32(model.EndYearMonth);
                marketsharereportdata.DataType1 = model.DataType1;
                marketsharereportdata.DataType2 = model.DataType2;
                marketsharereportdata.DataType3 = model.DataType3;
                marketsharereportdata.MarketLevel = Convert.ToInt16(model.MarketLevel);
                marketsharereportdata.Field1 = string.IsNullOrEmpty(model.Field1) ? new Guid() : new Guid(model.Field1);
                marketsharereportdata.Field1SortBy = model.Field1SortBy;
                marketsharereportdata.Field1AscDesc = model.Field1AscDesc;
                // marketsharereportdata.Field1Group = model.Field1Group.ToString();
                marketsharereportdata.Field1Records = model.Field1Records;
                //marketsharereportdata.Field1TopN = model.Field1TopN.ToString();
                //marketsharereportdata.Field1TopNPercent = Convert.ToInt16(model.Field1TopNPercent);
                marketsharereportdata.Field2 = string.IsNullOrEmpty(model.Field2) ? new Guid() : new Guid(model.Field2);
                marketsharereportdata.Field2SortBy = model.Field2SortBy;
                marketsharereportdata.Field2AscDesc = model.Field2AscDesc;
                //marketsharereportdata.Field2Group = model.Field2Group.ToString();
                marketsharereportdata.Field2Records = model.Field2Records;
                // marketsharereportdata.Field2TopN = model.Field2TopN.ToString();
                // marketsharereportdata.Field2TopNPercent = model.Field2TopNPercent.ToString();
                marketsharereportdata.Field3 = string.IsNullOrEmpty(model.Field3) ? new Guid() : new Guid(model.Field3);
                marketsharereportdata.Field3SortBy = model.Field3SortBy;
                marketsharereportdata.Field3AscDesc = model.Field3AscDesc;
                // marketsharereportdata.Field3Group = model.Field3Group.ToString();
                marketsharereportdata.Field3Records = model.Field3Records;
                //marketsharereportdata.Field3TopN = model.Field3TopN.ToString();
                //marketsharereportdata.Field3TopNPercent = model.Field3TopNPercent.ToString();
                marketsharereportdata.ShowDischargesHospital = model.ShowDischargesHospital;
                marketsharereportdata.ShowDischargesState = model.ShowDischargesState;
                marketsharereportdata.ShowDischargesPercent = model.ShowDischargesPercent;
                marketsharereportdata.ShowLOSHospital = model.ShowLOSHospital;
                marketsharereportdata.ShowLOSState = model.ShowLOSState;
                marketsharereportdata.ShowLOSPercent = model.ShowLOSPercent;
                marketsharereportdata.ShowHospitalPercent = model.ShowHospitalPercent;
                marketsharereportdata.ShowLOSHospitalPercent = model.ShowLOSHospitalPercent;
                marketsharereportdata.ShowField1 = model.ShowField1;
                marketsharereportdata.ShowField1Description = model.ShowField1Description;
                marketsharereportdata.ShowField2 = model.ShowField2;
                marketsharereportdata.ShowField2Description = model.ShowField2Description;
                marketsharereportdata.ShowField3 = model.ShowField3;
                marketsharereportdata.ShowField3Description = model.ShowField3Description;
                marketsharereportdata.ShowSummary = model.ShowSummary;
                marketsharereportdata.ShowFilter = model.ShowFilter;
                //this variable needs to come from the dates used
                // var datadictdate = "12/31/2011";
                var enddate = model.EndYearMonth;
                var calcdate = new DateTime(Convert.ToInt16(enddate.Substring(0, 4)), Convert.ToInt32(enddate.Substring(4, 2)), 1).AddMonths(1).AddDays(-1);
                //var datadictdate = calcdate.ToString();
                var datadictdate = String.Format("{0:MM/dd/yyyy}", calcdate);
                if (!string.IsNullOrEmpty(model.FilterIdCase))
                {
                    marketsharereportdata.FilterIdCase = new Guid(model.FilterIdCase);
                }
                else
                {
                    marketsharereportdata.FilterIdCase = null;
                }
                if (!string.IsNullOrEmpty(model.FilterIdGeographic))
                {
                    marketsharereportdata.FilterIdGeographic = new Guid(model.FilterIdGeographic);
                }
                else
                {
                    marketsharereportdata.FilterIdGeographic = null;
                }
                if (actionMethod == "SaveGeographicFilter" || actionMethod == "SaveFilter")
                {
                    var newFilterId = System.Guid.NewGuid();
                    var defaultFilter = new Mha.Ids.Data.Filter();
                    defaultFilter.IsPrimaryFilter = false;
                    defaultFilter.IsGlobalShare = false;
                    defaultFilter.Id = newFilterId;
                    defaultFilter.UnitId = model.UnitId;
                    defaultFilter.UserId = userId;
                    defaultFilter.Description = actionMethod == "SaveGeographicFilter" ? model.NewGeographicFilterName : model.NewFilterName;
                    defaultFilter.FilterTypesId = actionMethod == "SaveGeographicFilter" ? 1 : 2;

                    //special handling for mapping from dto to filter details table

                    FilterDetail newdetail;

                    int sequencenumber = 0;
                    foreach (var x in
                        actionMethod == "SaveGeographicFilter" ? model.MarketShareLiveFilterGeographicDetailsList : model.MarketShareLiveFilterDetailsList)
                    {
                        var selectedValues = filterRepository.DeformatString(x.DataDictionaryId, x.FieldValue,
                                                                             datadictdate, x.Operation);
                        selectedValues = filterRepository.ReformatString(x.DataDictionaryId, selectedValues,
                                                                         datadictdate);
                        newdetail = new FilterDetail();
                        newdetail.AndOr = x.AndOr;
                        newdetail.CloseParenthesis = x.CloseParenthesis;
                        newdetail.DataDictionaryId = x.DataDictionaryId;

                        newdetail.FieldValue = selectedValues;
                        newdetail.OpenParenthesis = x.OpenParenthesis;
                        newdetail.Operation = x.Operation;
                        newdetail.FieldType = x.FieldType;
                        newdetail.Sequence = sequencenumber;
                        //at this point we would calculate the sql text for this line
                        newdetail.SQLText = filterRepository.BuildSql(x.DataDictionaryId, selectedValues,
                                                                      x.Operation); //x.SQLText;
                        newdetail.FilterId = newFilterId;
                        newdetail.Id = System.Guid.NewGuid();
                        sequencenumber++;
                        defaultFilter.FilterDetails.Add(newdetail);
                    }

                    //do not set the id when saving
                    if (sequencenumber > 0)
                    {
                        filterRepository.OuterFilterSave(defaultFilter);

                        //    if (actionMethod == "SaveFilter")
                        //    {
                        //        marketsharereportdata.FilterIdCase = newFilterId;
                        //    }
                        //    else
                        //    {
                        //        marketsharereportdata.FilterIdGeographic = newFilterId;
                        //    }
                    }
                }
                //the filter statement will no longer be created since it is linked
                //}
                //else
                //{
                //special handling for mapping from dto to filter details table

                //add filter validation code
                string totalopenparen = "";
                string totalcloseparen = "";
                TempData["CaseFilterMessage"] = null;
                TempData["GeographicFilterMessage"] = null;
                MarketShareLiveFilterDetail newdetailCase;

                int sequencenumberCase = 0;
                foreach (var x in model.MarketShareLiveFilterDetailsList)
                {
                    totalopenparen += x.OpenParenthesis;
                    totalcloseparen += x.CloseParenthesis;
                    if (totalcloseparen.Length > totalopenparen.Length)
                    {
                        TempData["CaseFilterMessage"] = "Parentheses are not formatted correctly";
                    }
                    var selectedValues = filterRepository.DeformatString(x.DataDictionaryId, x.FieldValue,
                                                                         datadictdate, x.Operation);
                    selectedValues = filterRepository.ReformatString(x.DataDictionaryId, selectedValues,
                                                                     datadictdate);
                    
                    newdetailCase = new MarketShareLiveFilterDetail();
                    newdetailCase.AndOr = x.AndOr;
                    newdetailCase.CloseParenthesis = x.CloseParenthesis;
                    newdetailCase.DataDictionaryId = x.DataDictionaryId;

                    newdetailCase.FieldValue = selectedValues;
                    newdetailCase.OpenParenthesis = x.OpenParenthesis;
                    newdetailCase.Operation = x.Operation;
                    newdetailCase.FieldType = x.FieldType;
                    newdetailCase.Sequence = sequencenumberCase;
                    //at this point we would calculate the sql text for this line
                    newdetailCase.SqlText = filterRepository.BuildSql(x.DataDictionaryId, selectedValues,
                                                                  x.Operation);
                    //x.SQLText;
                    newdetailCase.FilterId = x.FilterId;
                    newdetailCase.Id = System.Guid.NewGuid();
                    newdetailCase.MarketShareLiveReportDataId = id;
                    sequencenumberCase++;
                    
                    if (x.FieldValue != selectedValues)
                    {
                        TempData["CaseFilterMessage"] = "Value was reformatted during validation";
                    }
                    if (selectedValues == "")
                    {
                        TempData["CaseFilterMessage"] = "Error in Field Value field";
                    }
                    if (selectedValues == "All")
                    {
                        newdetailCase.FieldValue = "";
                        selectedValues = "";
                        newdetailCase.SqlText = filterRepository.BuildSql(x.DataDictionaryId, selectedValues,
                                                                  x.Operation);
                        TempData["CaseFilterMessage"] = "Selecting all possible values is invalid in a filter.";
                    }
                    marketsharereportdata.MarketShareLiveFilterDetails.Add(newdetailCase);
                }
                if (totalcloseparen.Length != totalopenparen.Length)
                {
                    TempData["CaseFilterMessage"] = "Parentheses are not balanced";
                }

                if (model.ActionMethod == "ValidateCaseFilter" && TempData["CaseFilterMessage"] == null)
                   {
                       TempData["CaseFilterMessage"] = "No errors found";
                   }
                // see if they asked to apply a saved case filter
                if (!string.IsNullOrEmpty(model.FilterIdCase) && model.ActionMethod == "ApplyCaseFilter")
                {
                    var filter = filterRepository.GetFilter(new Guid(model.FilterIdCase));


                    var ordererdtable = from o in filter.FilterDetails

                                        // AsEnumerable will prevent the roundtrip to the store (applying a different extension method)

                                        orderby o.Sequence

                                        select o;


                    foreach (FilterDetail line in ordererdtable)
                    {
                        newdetailCase = new MarketShareLiveFilterDetail();
                        newdetailCase.AndOr = line.AndOr;
                        newdetailCase.CloseParenthesis = line.CloseParenthesis;
                        newdetailCase.DataDictionaryId = line.DataDictionaryId;


                        newdetailCase.FieldValue = line.FieldValue;
                        newdetailCase.FilterId = line.FilterId;
                        newdetailCase.Id = line.Id;
                        newdetailCase.OpenParenthesis = line.OpenParenthesis;
                        newdetailCase.Operation = line.Operation;
                        newdetailCase.FieldType = line.FieldType;
                        newdetailCase.Sequence = sequencenumberCase;
                        newdetailCase.SqlText = line.SQLText;
                        newdetailCase.FilterId = line.FilterId;
                        newdetailCase.Id = System.Guid.NewGuid();
                        newdetailCase.MarketShareLiveReportDataId = id;
                        sequencenumberCase++;
                        marketsharereportdata.MarketShareLiveFilterDetails.Add(newdetailCase);
                    }
                    //clear the value
                    marketsharereportdata.FilterIdCase = null;
                }


                 totalopenparen = "";
                 totalcloseparen = "";

                //special handling for mapping from dto to filter details table
                MarketShareLiveFilterGeographicDetail newdetailGeographic;

                int sequencenumberGeographic = 0;
                foreach (var x in model.MarketShareLiveFilterGeographicDetailsList)
                {
                    totalopenparen += x.OpenParenthesis;
                    totalcloseparen += x.CloseParenthesis;
                    if (totalcloseparen.Length > totalopenparen.Length)
                    {
                        TempData["GeographicFilterMessage"] = "Parentheses are not formatted correctly";
                    }
                    var selectedValues = filterRepository.DeformatString(x.DataDictionaryId, x.FieldValue,
                                                                         datadictdate, x.Operation);
                    selectedValues = filterRepository.ReformatString(x.DataDictionaryId, selectedValues, datadictdate);
                    newdetailGeographic = new MarketShareLiveFilterGeographicDetail();
                    newdetailGeographic.AndOr = x.AndOr;
                    newdetailGeographic.CloseParenthesis = x.CloseParenthesis;
                    newdetailGeographic.DataDictionaryId = x.DataDictionaryId;

                    newdetailGeographic.FieldValue = selectedValues;
                    newdetailGeographic.OpenParenthesis = x.OpenParenthesis;
                    newdetailGeographic.Operation = x.Operation;

                    newdetailGeographic.FieldType = x.FieldType;
                    newdetailGeographic.Sequence = sequencenumberGeographic;
                    //at this point we would calculate the sql text for this line
                    newdetailGeographic.SqlText = filterRepository.BuildSql(x.DataDictionaryId, selectedValues, x.Operation);
                    //x.SQLText;
                    newdetailGeographic.FilterId = x.FilterId;
                    newdetailGeographic.Id = System.Guid.NewGuid();
                    newdetailGeographic.MarketShareLiveReportDataId = id;
                    sequencenumberGeographic++;
                    
                    if (x.FieldValue != selectedValues)
                    {
                        TempData["GeographicFilterMessage"] = "Value was reformatted during validation";
                    }
                    if (selectedValues == "")
                    {
                        TempData["GeographicFilterMessage"] = "Error in Field Value field";
                    }
                    if (selectedValues == "All")
                    {
                        newdetailGeographic.FieldValue = "";
                        selectedValues = "";
                        newdetailGeographic.SqlText = filterRepository.BuildSql(x.DataDictionaryId, selectedValues, x.Operation);
                        TempData["CaseFilterMessage"] = "Selecting all possible values is invalid in a filter.";
                    }
                    marketsharereportdata.MarketShareLiveFilterGeographicDetails.Add(newdetailGeographic);
                }
                if (totalcloseparen.Length != totalopenparen.Length)
                {
                    TempData["GeographicFilterMessage"] = "Parentheses are not balanced";
                }

                if (model.ActionMethod == "ValidateGeographicFilter" && TempData["GeographicFilterMessage"] == null)
                {
                    TempData["GeographicFilterMessage"] = "No errors found";
                }
                // see if they asked to apply a saved case filter
                if (!string.IsNullOrEmpty(model.FilterIdGeographic) && model.ActionMethod == "ApplyGeographicFilter")
                {
                    var filter = filterRepository.GetFilter(new Guid(model.FilterIdGeographic));


                    var ordererdtable = from o in filter.FilterDetails

                                        // AsEnumerable will prevent the roundtrip to the store (applying a different extension method)

                                        orderby o.Sequence

                                        select o;


                    foreach (FilterDetail line in ordererdtable)
                    {
                        newdetailGeographic = new MarketShareLiveFilterGeographicDetail();
                        newdetailGeographic.AndOr = line.AndOr;
                        newdetailGeographic.CloseParenthesis = line.CloseParenthesis;
                        newdetailGeographic.DataDictionaryId = line.DataDictionaryId;


                        newdetailGeographic.FieldValue = line.FieldValue;
                        newdetailGeographic.FilterId = line.FilterId;
                        newdetailGeographic.Id = line.Id;
                        newdetailGeographic.OpenParenthesis = line.OpenParenthesis;
                        newdetailGeographic.Operation = line.Operation;
                        newdetailGeographic.FieldType = line.FieldType;
                        newdetailGeographic.Sequence = sequencenumberGeographic;
                        newdetailGeographic.SqlText = line.SQLText;
                        newdetailGeographic.FilterId = line.FilterId;
                        newdetailGeographic.Id = System.Guid.NewGuid();
                        newdetailGeographic.MarketShareLiveReportDataId = id;
                        sequencenumberGeographic++;
                        marketsharereportdata.MarketShareLiveFilterGeographicDetails.Add(newdetailGeographic);
                    }
                    //clear the value
                    marketsharereportdata.FilterIdGeographic = null;
                }
                //special handling for mapping from dto to filter details table
                MarketShareLiveFilterCensusDetail newdetailCensus;

                int sequencenumberCensus = 0;
                foreach (var x in model.MarketShareLiveFilterCensusDetailsList)
                {
                    var selectedValues = filterRepository.DeformatString(x.DataDictionaryId, x.FieldValue,
                                                                         datadictdate, x.Operation);
                    selectedValues = filterRepository.ReformatString(x.DataDictionaryId, selectedValues, datadictdate);
                    newdetailCensus = new MarketShareLiveFilterCensusDetail();
                    newdetailCensus.AndOr = x.AndOr;
                    newdetailCensus.CloseParenthesis = x.CloseParenthesis;
                    newdetailCensus.DataDictionaryId = x.DataDictionaryId;

                    newdetailCensus.FieldValue = selectedValues;
                    newdetailCensus.OpenParenthesis = x.OpenParenthesis;
                    newdetailCensus.Operation = x.Operation;
                    newdetailCensus.FieldType = x.FieldType;
                    newdetailCensus.Sequence = sequencenumberCensus;
                    //at this point we would calculate the sql text for this line
                    newdetailCensus.SqlText = filterRepository.BuildSql(x.DataDictionaryId, selectedValues, x.Operation);
                    //x.SQLText;
                    newdetailCensus.FilterId = x.FilterId;
                    newdetailCensus.Id = System.Guid.NewGuid();
                    newdetailCensus.MarketShareLiveReportDataId = id;
                    sequencenumberCensus++;
                    marketsharereportdata.MarketShareLiveFilterCensusDetails.Add(newdetailCensus);
                }

                // TODO see if they asked to apply a saved CENSUS filter

                //special handling for mapping from dto to filter details table
                MarketShareLiveFilterCompareDetail newdetailCompare;

                int sequencenumberCompare = 0;
                foreach (var x in model.MarketShareLiveFilterCompareDetailsList)
                {
                    var selectedValues = filterRepository.DeformatString(x.DataDictionaryId, x.FieldValue,
                                                                         datadictdate, x.Operation);
                    selectedValues = filterRepository.ReformatString(x.DataDictionaryId, selectedValues, datadictdate);
                    newdetailCompare = new MarketShareLiveFilterCompareDetail();
                    newdetailCompare.AndOr = x.AndOr;
                    newdetailCompare.CloseParenthesis = x.CloseParenthesis;
                    newdetailCompare.DataDictionaryId = x.DataDictionaryId;

                    newdetailCompare.FieldValue = selectedValues;
                    newdetailCompare.OpenParenthesis = x.OpenParenthesis;
                    newdetailCompare.Operation = x.Operation;
                    newdetailCompare.FieldType = x.FieldType;
                    newdetailCompare.Sequence = sequencenumberCompare;
                    //at this point we would calculate the sql text for this line
                    newdetailCompare.SqlText = filterRepository.BuildSql(x.DataDictionaryId, selectedValues, x.Operation);
                    //x.SQLText;
                    newdetailCompare.FilterId = x.FilterId;
                    newdetailCompare.Id = System.Guid.NewGuid();
                    newdetailCompare.MarketShareLiveReportDataId = id;
                    sequencenumberCompare++;
                    marketsharereportdata.MarketShareLiveFilterCompareDetails.Add(newdetailCompare);
                }
                // }
                // TODO see if they asked to apply a saved COMPARE filter
                //if (AddRecord)
                //{
                reportRepository.AddLive(marketsharereportdata);
                if (actionMethod == "Run")
                {
                    // var reportRunIdResult = reportRepository.MarketShareLiveRunAdd(id, model.UnitId, userId, "", 0);

                    // reportRunId = reportRunIdResult.AsQueryable().FirstOrDefault().GetValueOrDefault();
                    return RedirectToAction("Edit",
                                            new
                                                {
                                                    id = id,
                                                    unitId = model.UnitId,
                                                    mode = "live",
                                                    reportRunGenId = id,
                                                    moduleId = model.ModuleId
                                                    // reportRunGenId = reportRunId
                                                });
                }
                //check for valid record if saving
                var validationId = id;
                var validationmsg = reportRepository.ValidateReport(validationId, 1);

                var ValidQuery = validationmsg.AsQueryable().FirstOrDefault().ValidRequest;

                 
                if (actionMethod == "Save" || actionMethod == "SaveNew")
                {

                    var reportDesc = model.Description; // ReportDefinition.Description;
                    var reportId = new System.Guid(model.ReportDefinitionsId);

                    bool isReportDescriptionUnique = reportRepository.IsReportDescriptionUnique(userId, reportId, reportDesc, actionMethod); // this function can be used for check report description also
                   
                    if (ValidQuery != true) 
                    {
                        newMode = "SaveCancel";
                    }
                    else if (isReportDescriptionUnique != true)
                    {
                        newMode = "SaveCancelDuplicateReportName";
                    }
                    else
                    {
                        //get the prior state of the data
                        id = new Guid(model.ReportDefinitionsId);
                        MarketShareReportData Savemarketsharereportdata = reportRepository.GetMarketShareReportData(id);
                        Guid msId = Savemarketsharereportdata.Id;
                        if (Savemarketsharereportdata.ReportDefinition.IsBaseReport ||
                            (Savemarketsharereportdata.ReportDefinition.IsGlobalShare && !isUserHost) ||
                            (Savemarketsharereportdata.ReportDefinition.UserId != userId && !isUserHost) || actionMethod == "SaveNew")
                        {
                            //the record should be ADDED as a new one for this user

                            id = System.Guid.NewGuid();
                            msId = System.Guid.NewGuid();
                            Savemarketsharereportdata = new Mha.Ids.Data.MarketShareReportData();
                            Savemarketsharereportdata.ReportDefinition = new Mha.Ids.Data.ReportDefinition();

                            Savemarketsharereportdata.UserId = userId;
                            Savemarketsharereportdata.ReportDefinition.IsBaseReport = false;
                            Savemarketsharereportdata.ReportDefinition.UserId = userId;
                            Savemarketsharereportdata.ReportDefinition.ModuleId = model.ModuleId;
                            newMode = "SavedAdd";
                            AddRecord = true;
                            if (!isUserHost)
                            {
                                Savemarketsharereportdata.ReportDefinition.IsGlobalShare = false;
                            }
                        }
                        else
                        {
                            newMode = "Saved";
                            //delete the previous saved filter details from the prior state model
                            var oldLines = Savemarketsharereportdata.MarketShareFilterDetails.ToList();
                            //modify this to delete all of the related filters
                            reportRepository.DeleteLines(oldLines);
                            //delete the previous saved geographic filter details from the prior state model
                            var oldLinesGeo = Savemarketsharereportdata.MarketShareFilterGeographicDetails.ToList();
                            //modify this to delete all of the related filters
                            reportRepository.DeleteGeographicLines(oldLinesGeo);
                            //delete the previous saved census filter details from the prior state model
                            var oldLinesCen = Savemarketsharereportdata.MarketShareFilterCensusDetails.ToList();
                            //modify this to delete all of the related filters
                            reportRepository.DeleteCensusLines(oldLinesCen);
                            //delete the previous saved census filter details from the prior state model
                            var oldLinesComp = Savemarketsharereportdata.MarketShareFilterCompareDetails.ToList();
                            //modify this to delete all of the related filters
                            reportRepository.DeleteCompareLines(oldLinesComp);
                        }



                        //special handling for mapping from dto to main filter table

                        Savemarketsharereportdata.Id = msId;
                        Savemarketsharereportdata.ReportDefinitionsId = id;


                        Savemarketsharereportdata.UnitId = model.UnitId;
                        Savemarketsharereportdata.ReportDefinition.Id = id;
                        Savemarketsharereportdata.ReportDefinition.Description = model.Description;
                        Savemarketsharereportdata.ReportDefinition.ReportTypesId = model.ReportTypesId;
                        Savemarketsharereportdata.ReportDefinition.ReportGroupsId = model.ReportGroupsId;
                        Savemarketsharereportdata.ReportDefinition.ModuleId = model.ModuleId;
                        //marketsharereportdata.ReportDefinition.IsBaseReport = model.IsBaseReport;
                        Savemarketsharereportdata.ReportDefinition.IsGlobalShare = model.IsGlobalShare;
                        Savemarketsharereportdata.ReportDefinition.IsTemplateReport = model.IsTemplateReport;

                        Savemarketsharereportdata.ReportDefinition.Summary = model.Summary;
                        Savemarketsharereportdata.BaseYear = model.BaseYear;
                        Savemarketsharereportdata.YearType = model.YearType;
                        Savemarketsharereportdata.Trending = model.Trending;
                        Savemarketsharereportdata.ShowAdvancedFilter = model.ShowAdvancedFilter;
                        Savemarketsharereportdata.DatePeriodType = model.DatePeriodType;
                        Savemarketsharereportdata.DatePeriods = Convert.ToInt16(model.DatePeriods);
                        Savemarketsharereportdata.DateCalculationType = model.DateCalculationType;
                        Savemarketsharereportdata.OutputType = model.OutputType;
                        Savemarketsharereportdata.StartYearMonth = Convert.ToInt32(model.StartYearMonth);
                        Savemarketsharereportdata.EndYearMonth = Convert.ToInt32(model.EndYearMonth);
                        Savemarketsharereportdata.DataType1 = model.DataType1;
                        Savemarketsharereportdata.DataType2 = model.DataType2;
                        Savemarketsharereportdata.DataType3 = model.DataType3;
                        Savemarketsharereportdata.MarketLevel = Convert.ToInt16(model.MarketLevel);
                        Savemarketsharereportdata.Field1 = string.IsNullOrEmpty(model.Field1) ? new Guid() : new Guid(model.Field1);
                        Savemarketsharereportdata.Field1SortBy = model.Field1SortBy;
                        Savemarketsharereportdata.Field1AscDesc = model.Field1AscDesc;
                        // Savemarketsharereportdata.Field1Group = model.Field1Group.ToString();
                        Savemarketsharereportdata.Field1Records = model.Field1Records;
                        //Savemarketsharereportdata.Field1TopN = model.Field1TopN.ToString();
                        //Savemarketsharereportdata.Field1TopNPercent = Convert.ToInt16(model.Field1TopNPercent);
                        Savemarketsharereportdata.Field2 = string.IsNullOrEmpty(model.Field2) ? new Guid() : new Guid(model.Field2);
                        Savemarketsharereportdata.Field2SortBy = model.Field2SortBy;
                        Savemarketsharereportdata.Field2AscDesc = model.Field2AscDesc;
                        //Savemarketsharereportdata.Field2Group = model.Field2Group.ToString();
                        Savemarketsharereportdata.Field2Records = model.Field2Records;
                        // Savemarketsharereportdata.Field2TopN = model.Field2TopN.ToString();
                        // Savemarketsharereportdata.Field2TopNPercent = model.Field2TopNPercent.ToString();
                        Savemarketsharereportdata.Field3 = string.IsNullOrEmpty(model.Field3) ? new Guid() : new Guid(model.Field3);
                        Savemarketsharereportdata.Field3SortBy = model.Field3SortBy;
                        Savemarketsharereportdata.Field3AscDesc = model.Field3AscDesc;
                        // Savemarketsharereportdata.Field3Group = model.Field3Group.ToString();
                        Savemarketsharereportdata.Field3Records = model.Field3Records;
                        //Savemarketsharereportdata.Field3TopN = model.Field3TopN.ToString();
                        //Savemarketsharereportdata.Field3TopNPercent = model.Field3TopNPercent.ToString();
                        Savemarketsharereportdata.ShowDischargesHospital = model.ShowDischargesHospital;
                        Savemarketsharereportdata.ShowDischargesState = model.ShowDischargesState;
                        Savemarketsharereportdata.ShowDischargesPercent = model.ShowDischargesPercent;
                        Savemarketsharereportdata.ShowLOSHospital = model.ShowLOSHospital;
                        Savemarketsharereportdata.ShowLOSState = model.ShowLOSState;
                        Savemarketsharereportdata.ShowLOSPercent = model.ShowLOSPercent;
                        Savemarketsharereportdata.ShowHospitalPercent = model.ShowHospitalPercent;
                        Savemarketsharereportdata.ShowLOSHospitalPercent = model.ShowLOSHospitalPercent;
                        Savemarketsharereportdata.ShowField1 = model.ShowField1;
                        Savemarketsharereportdata.ShowField1Description = model.ShowField1Description;
                        Savemarketsharereportdata.ShowField2 = model.ShowField2;
                        Savemarketsharereportdata.ShowField2Description = model.ShowField2Description;
                        Savemarketsharereportdata.ShowField3 = model.ShowField3;
                        Savemarketsharereportdata.ShowField3Description = model.ShowField3Description;
                        Savemarketsharereportdata.ShowSummary = model.ShowSummary;
                        Savemarketsharereportdata.ShowFilter = model.ShowFilter;
                        if (model.FilterIdCase != "" && model.FilterIdCase != null)
                        {
                            Savemarketsharereportdata.FilterIdCase = new Guid(model.FilterIdCase);
                        }
                        else
                        {
                            Savemarketsharereportdata.FilterIdCase = null;
                        }
                        if (model.FilterIdGeographic != "" && model.FilterIdGeographic != null)
                        {
                            Savemarketsharereportdata.FilterIdGeographic = new Guid(model.FilterIdGeographic);
                        }
                        else
                        {
                            Savemarketsharereportdata.FilterIdGeographic = null;
                        }
                        //special handling for mapping from dto to filter details table
                        //this variable needs to come from the dates used
                        var SaveEndDate = model.EndYearMonth;
                        var Savecalcdate = new DateTime(Convert.ToInt16(SaveEndDate.Substring(0, 4)), Convert.ToInt32(SaveEndDate.Substring(4, 2)), 1).AddMonths(1).AddDays(-1);
                        var Savedatadictdate = String.Format("{0:MM/dd/yyyy}", Savecalcdate);
                        //var Savedatadictdate = Savecalcdate.ToString();
                        MarketShareFilterDetail Savenewdetail;

                        int sequencenumber = 0;
                        foreach (var x in model.MarketShareLiveFilterDetailsList)
                        {
                            var selectedValues = filterRepository.DeformatString(x.DataDictionaryId, x.FieldValue,
                                                                                 Savedatadictdate, x.Operation);
                            selectedValues = filterRepository.ReformatString(x.DataDictionaryId, selectedValues, Savedatadictdate);
                            Savenewdetail = new MarketShareFilterDetail();
                            Savenewdetail.AndOr = x.AndOr;
                            Savenewdetail.CloseParenthesis = x.CloseParenthesis;
                            Savenewdetail.DataDictionaryId = x.DataDictionaryId;

                            Savenewdetail.FieldValue = selectedValues;
                            Savenewdetail.OpenParenthesis = x.OpenParenthesis;
                            Savenewdetail.Operation = x.Operation;
                            Savenewdetail.FieldType = x.FieldType;
                            Savenewdetail.Sequence = sequencenumber;
                            //at this point we would calculate the sql text for this line
                            Savenewdetail.SqlText = filterRepository.BuildSql(x.DataDictionaryId, selectedValues, x.Operation);
                            //x.SQLText;
                            Savenewdetail.FilterId = x.FilterId;
                            Savenewdetail.Id = System.Guid.NewGuid();
                            Savenewdetail.MarketShareReportDataId = msId;
                            sequencenumber++;
                            Savemarketsharereportdata.MarketShareFilterDetails.Add(Savenewdetail);
                        }
                        //special handling for mapping from dto to filter details table
                        MarketShareFilterGeographicDetail SaveNewdetailGeographic;

                        sequencenumberGeographic = 0;
                        foreach (var x in model.MarketShareLiveFilterGeographicDetailsList)
                        {
                            var selectedValues = filterRepository.DeformatString(x.DataDictionaryId, x.FieldValue,
                                                                                 Savedatadictdate, x.Operation);
                            selectedValues = filterRepository.ReformatString(x.DataDictionaryId, selectedValues, Savedatadictdate);
                            SaveNewdetailGeographic = new MarketShareFilterGeographicDetail();
                            SaveNewdetailGeographic.AndOr = x.AndOr;
                            SaveNewdetailGeographic.CloseParenthesis = x.CloseParenthesis;
                            SaveNewdetailGeographic.DataDictionaryId = x.DataDictionaryId;

                            SaveNewdetailGeographic.FieldValue = selectedValues;
                            SaveNewdetailGeographic.OpenParenthesis = x.OpenParenthesis;
                            SaveNewdetailGeographic.Operation = x.Operation;
                            SaveNewdetailGeographic.FieldType = x.FieldType;
                            SaveNewdetailGeographic.Sequence = sequencenumberGeographic;
                            //at this point we would calculate the sql text for this line
                            SaveNewdetailGeographic.SqlText = filterRepository.BuildSql(x.DataDictionaryId, selectedValues, x.Operation);
                            //x.SQLText;
                            SaveNewdetailGeographic.FilterId = x.FilterId;
                            SaveNewdetailGeographic.Id = System.Guid.NewGuid();
                            SaveNewdetailGeographic.MarketShareReportDataId = msId;
                            sequencenumberGeographic++;
                            Savemarketsharereportdata.MarketShareFilterGeographicDetails.Add(SaveNewdetailGeographic);
                        }
                        //special handling for mapping from dto to filter details table
                        MarketShareFilterCensusDetail SavenewdetailCensus;

                        sequencenumberCensus = 0;
                        foreach (var x in model.MarketShareLiveFilterCensusDetailsList)
                        {
                            var selectedValues = filterRepository.DeformatString(x.DataDictionaryId, x.FieldValue,
                                                                                 Savedatadictdate, x.Operation);
                            selectedValues = filterRepository.ReformatString(x.DataDictionaryId, selectedValues, Savedatadictdate);
                            SavenewdetailCensus = new MarketShareFilterCensusDetail();
                            SavenewdetailCensus.AndOr = x.AndOr;
                            SavenewdetailCensus.CloseParenthesis = x.CloseParenthesis;
                            SavenewdetailCensus.DataDictionaryId = x.DataDictionaryId;

                            SavenewdetailCensus.FieldValue = selectedValues;
                            SavenewdetailCensus.OpenParenthesis = x.OpenParenthesis;
                            SavenewdetailCensus.Operation = x.Operation;
                            SavenewdetailCensus.FieldType = x.FieldType;
                            SavenewdetailCensus.Sequence = sequencenumberCensus;
                            //at this point we would calculate the sql text for this line
                            SavenewdetailCensus.SqlText = filterRepository.BuildSql(x.DataDictionaryId, selectedValues, x.Operation);
                            //x.SQLText;
                            SavenewdetailCensus.FilterId = x.FilterId;
                            SavenewdetailCensus.Id = System.Guid.NewGuid();
                            SavenewdetailCensus.MarketShareReportDataId = msId;
                            sequencenumberCensus++;
                            Savemarketsharereportdata.MarketShareFilterCensusDetails.Add(SavenewdetailCensus);
                        }
                        //special handling for mapping from dto to filter details table
                        MarketShareFilterCompareDetail SavenewdetailCompare;

                        sequencenumberCompare = 0;
                        foreach (var x in model.MarketShareLiveFilterCompareDetailsList)
                        {
                            var selectedValues = filterRepository.DeformatString(x.DataDictionaryId, x.FieldValue,
                                                                                 Savedatadictdate, x.Operation);
                            selectedValues = filterRepository.ReformatString(x.DataDictionaryId, selectedValues, Savedatadictdate);
                            SavenewdetailCompare = new MarketShareFilterCompareDetail();
                            SavenewdetailCompare.AndOr = x.AndOr;
                            SavenewdetailCompare.CloseParenthesis = x.CloseParenthesis;
                            SavenewdetailCompare.DataDictionaryId = x.DataDictionaryId;

                            SavenewdetailCompare.FieldValue = selectedValues;
                            SavenewdetailCompare.OpenParenthesis = x.OpenParenthesis;
                            SavenewdetailCompare.Operation = x.Operation;
                            SavenewdetailCompare.FieldType = x.FieldType;
                            SavenewdetailCompare.Sequence = sequencenumberCompare;
                            //at this point we would calculate the sql text for this line
                            SavenewdetailCompare.SqlText = filterRepository.BuildSql(x.DataDictionaryId, selectedValues, x.Operation);
                            //x.SQLText;
                            SavenewdetailCompare.FilterId = x.FilterId;
                            SavenewdetailCompare.Id = System.Guid.NewGuid();
                            SavenewdetailCompare.MarketShareReportDataId = msId;
                            sequencenumberCompare++;
                            Savemarketsharereportdata.MarketShareFilterCompareDetails.Add(SavenewdetailCompare);
                        }
                        //timmy
                        if (AddRecord)
                        {
                            reportRepository.Add(Savemarketsharereportdata);
                        }
                        else
                        {
                                reportRepository.Save(Savemarketsharereportdata);
                        }


                        //return RedirectToAction("Index");
                        return RedirectToAction("Edit",
                                                new
                                                {
                                                    id = id,
                                                    unitId = model.UnitId,
                                                    mode = newMode,
                                                    reportRunGenId = new Guid(),
                                                    moduleId = model.ModuleId
                                                });
                    }
                }



                return RedirectToAction("Edit",
                                        new
                                            {
                                                id = id,
                                                unitId = model.UnitId,
                                                mode = newMode,
                                                reportRunGenId = new Guid(),
                                                moduleId = model.ModuleId
                                            });

            }






            return RedirectToAction("Index");
        }

        /// <summary>
        /// Call Run method async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="unitId">The unit id.</param>
        /// <returns></returns>
        public async Task<ActionResult> Run(Guid id, string unitId, string mode)
        {
            //template running
            if (mode == "live")
            {
                //try to use the emulate user function
                Profile = IdsProfile.GetUserProfile(User.Identity.Name);
                var sessionSink = Profile.SessionObject;
                var emulatename = string.Empty;
                if (sessionSink.UserId != sessionSink.LoginUserId)
                {
                    var userId = sessionSink.UserId;
                    var username = userRepository.GetUserNameById(userId);
                    Profile = IdsProfile.GetUserProfile(username);
                    sessionSink = Profile.SessionObject;
                    emulatename = username;
                }
                ViewBag.EmulateName = emulatename;

                var userLoginId = sessionSink.LoginUserId;
                // TODO wrap all the code into a transaction

                //var options = new TransactionOptions();
                //options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted; 
                //options.Timeout = new TimeSpan(0, 2, 0);

                //using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
                //{
                //Validate the report

                // First insert data into the ReportRunData table
                var reportRunIdResult = reportRepository.MarketShareLiveRunAdd(id, unitId, userLoginId, "", 0);

                reportRunId = reportRunIdResult.AsQueryable().FirstOrDefault().GetValueOrDefault();

                // reportRunId = id;
                var reportDeliveryProxy = new ReportDeliveryClient();
                var generateReportRequest = new GenerateReportRequest(reportRunId);

                var viewBagMessage = "Report was Submitted";
                // SignalR stuff for reload the grid
                var context = GlobalHost.ConnectionManager.GetHubContext<ReportNotificationHub>();
                context.Clients.reloadGrid("#GridViewResults");

                // wait for report tp finish and add some SignalR stuff to notify user once it is done

                // start a new thread for report delivery system to generate report
                var task = Task.Run(async () =>  // async delegate 
                {
                    await reportDeliveryProxy.GenerateReportAsync(generateReportRequest);
                    viewBagMessage = "Report was Submitted";
                    return Json(viewBagMessage, JsonRequestBehavior.AllowGet);
                });

                task.ContinueWith(t =>
                {
                    //    // TODO 
                    //    // wait for report tp finish and 
                    //    // add callback using SignalR stuff to notify user once it is done
                    //    //return RedirectToAction("Index", "ReportViewer");
                    viewBagMessage = "Report is Complete";
                    context.Clients.addMessage(viewBagMessage);
                    context.Clients.reloadGrid("#GridViewResults");
                    //return Json(viewBagMessage, JsonRequestBehavior.AllowGet);
                });
                // wait for report to finish and add some SignalR stuff to notify user once it is done
                //viewBagMessage = "Report is Complete";
                //ViewBag.Message = viewBagMessage;
                //return RedirectToAction("Index");




                // wait for report tp finish and add some SignalR stuff to notify user once it is done

                //get the time it took


                //good code commented out for testing
                //var viewBagMessage = " is Complete: Execution took ";
                //await reportDeliveryProxy.GenerateReportAsync(generateReportRequest);
                //var reportRecord = reportRepository.GetReportRunFromReportDataId(reportRunId);
                //TimeSpan span = (DateTime)reportRecord.TimeEnded - (DateTime)reportRecord.TimeStarted;
                //viewBagMessage = "Report "+reportRecord.Description + viewBagMessage + span.TotalSeconds.ToString() + " seconds.";
                //return Json(viewBagMessage, JsonRequestBehavior.AllowGet);
                //end of good code

            }
            return Json("Report was Submitted", JsonRequestBehavior.AllowGet);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Call Run method async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="unitId">The unit id.</param>
        /// <returns></returns>
        public async Task<ActionResult> RunOld(Guid id, string unitId, string mode)
        {
            //template running
            if (mode == "live")
            {
                //try to use the emulate user function
                Profile = IdsProfile.GetUserProfile(User.Identity.Name);
                var sessionSink = Profile.SessionObject;
                var emulatename = string.Empty;
                if (sessionSink.UserId != sessionSink.LoginUserId)
                {
                    var userId = sessionSink.UserId;
                    var username = userRepository.GetUserNameById(userId);
                    Profile = IdsProfile.GetUserProfile(username);
                    sessionSink = Profile.SessionObject;
                    emulatename = username;
                }
                ViewBag.EmulateName = emulatename;

                var userLoginId = sessionSink.LoginUserId;
                // TODO wrap all the code into a transaction

                //var options = new TransactionOptions();
                //options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted; 
                //options.Timeout = new TimeSpan(0, 2, 0);

                //using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
                //{
                //Validate the report

                // First insert data into the ReportRunData table
                var reportRunIdResult = reportRepository.MarketShareLiveRunAdd(id, unitId, userLoginId, "", 0);

                reportRunId = reportRunIdResult.AsQueryable().FirstOrDefault().GetValueOrDefault();

                // reportRunId = id;
                var reportDeliveryProxy = new ReportDeliveryClient();
                var generateReportRequest = new GenerateReportRequest(reportRunId);

                //var viewBagMessage = "Report was Submitted";


                // wait for report tp finish and add some SignalR stuff to notify user once it is done

                // start a new thread for report delivery system to generate report
                //var task = Task.Run(async () =>  // async delegate 
                //{
                //    await reportDeliveryProxy.GenerateReportAsync(generateReportRequest);
                //    //viewBagMessage = "Report was Submitted";
                //    //return Json(viewBagMessage, JsonRequestBehavior.AllowGet);
                //});

                //task.ContinueWith(t =>
                //{
                //    // TODO 
                //    // wait for report tp finish and 
                //    // add callback using SignalR stuff to notify user once it is done
                //    //return RedirectToAction("Index", "ReportViewer");
                //    viewBagMessage = "Report is Complete";

                //    return Json(viewBagMessage, JsonRequestBehavior.AllowGet);
                //});
                // wait for report to finish and add some SignalR stuff to notify user once it is done
                //viewBagMessage = "Report is Complete";
                //ViewBag.Message = viewBagMessage;
                //return RedirectToAction("Index");


                var viewBagMessage = " is Complete: Execution took ";

                // wait for report tp finish and add some SignalR stuff to notify user once it is done
                await reportDeliveryProxy.GenerateReportAsync(generateReportRequest);
                //get the time it took


                //good code commented out for testing
                var reportRecord = reportRepository.GetReportRunFromReportDataId(reportRunId);
                TimeSpan span = (DateTime)reportRecord.TimeEnded - (DateTime)reportRecord.TimeStarted;
                viewBagMessage = "Report " + reportRecord.Description + viewBagMessage + span.TotalSeconds.ToString() + " seconds.";
                return Json(viewBagMessage, JsonRequestBehavior.AllowGet);
                //end of good code

            }
            return Json("Report was Submitted", JsonRequestBehavior.AllowGet);
            return RedirectToAction("Index");
        }

        public void View(Guid reporttoRunId)
        {
            // first get storeage id from ReportStorage table
            reportStorageRepository = new ReportStorageRepository();
            var storageId = this.reportStorageRepository.GetStorageIdFromReportRunId(reporttoRunId);
            var reportContentInBytes = reportStorageRepository.GetReportContentByReportStorageId(storageId);
            HttpContext.Response.ContentType = "Application/vnd.ms-excel";
            HttpContext.Response.OutputStream.Write(reportContentInBytes, 0, reportContentInBytes.Length);
            
           // utility.DisplayReportInBrowser(reportContentInBytes);

        }

       // public ActionResult Copy(Guid id, string unitId)
       // {
            //Profile = IdsProfile.GetUserProfile(User.Identity.Name);
            //var sessionSink = Profile.SessionObject;
            //var userID = sessionSink.LoginUserId;
        //    MarketShareReportData marketsharereportdata = reportRepository.GetMarketShareReportData(id);
        //    var geographicFilters = filterRepository.GetFilterByTypeAndIds(1, unitId, userID);
        //    var caseFilters = filterRepository.GetFilterByTypeAndIds(2, unitId, userID);
        //    //Guid userID = reportdefdata.UserId;
        //    var reportdates = reportRepository.GetDates();
        //    //IEnumerable<DataDictionary> dataDictionary;

        //    var dataDict = reportRepository.GetFieldList();
        //    var dto = MarketShareDto.Create(marketsharereportdata, reportdates, userID, dataDict, geographicFilters, caseFilters);
        //    ViewBag.Title = "Market Share Report";
        //    var isUserHost = sessionSink.IsHost;
        //    ViewBag.IsHost = isUserHost;
        //    SelectList unitList;
        //    if (isUserHost)
        //    {
        //        // for host, list all the units in the system
        //        unitList = new SelectList(unitRepository.GeAllUnitsToList(), "ObjectId", "ObjectDescription");
        //    }
        //    else
        //    {
        //        // for non-host, get the login user's units 
        //        unitList = new SelectList(userRepository.GetUserUnitsByUserId(userID), "ObjectId", "ObjectDescription");
        //    }
        //    ViewBag.UnitList = unitList;
        //    return View(dto);
        //}


        ////
        //// POST: /Report/Edit/5

        //[HttpPost]
        //public ActionResult Copy(MarketShareDto model, string submitButton)
        //{
        //    if (ModelState.IsValid)
        //    {

                
        //        //process the data    .
        //        Guid id = new Guid(model.ReportDefinitionsId);
            //Profile = IdsProfile.GetUserProfile(User.Identity.Name);
            //var sessionSink = Profile.SessionObject;
            //var userID = sessionSink.LoginUserId;
        //        var isUserHost = sessionSink.IsHost;
        //        //get the prior state of the data
        //        MarketShareReportData marketsharereportdata = reportRepository.GetMarketShareReportData(id);
        //        Guid msId = marketsharereportdata.Id;
                
        //            //the record should be ADDED as a new one for this user

        //            id = System.Guid.NewGuid();
        //            msId = System.Guid.NewGuid();
        //            marketsharereportdata = new Mha.Ids.Data.MarketShareReportData();
        //            marketsharereportdata.ReportDefinition = new Mha.Ids.Data.ReportDefinition();

        //            marketsharereportdata.UserId = userID;
        //            marketsharereportdata.ReportDefinition.IsBaseReport = false;
        //            marketsharereportdata.ReportDefinition.UserId = userID;

                
        //            if (!isUserHost)
        //            {
        //                marketsharereportdata.ReportDefinition.IsGlobalShare = false;
        //            }
                



        //        //special handling for mapping from dto to main filter table

        //        marketsharereportdata.Id = msId;
        //        marketsharereportdata.ReportDefinitionsId = id;


        //        marketsharereportdata.UnitId = model.UnitId;
        //        marketsharereportdata.ReportDefinition.Id = id;
        //        marketsharereportdata.ReportDefinition.Description = model.Description;
        //        marketsharereportdata.ReportDefinition.ReportTypesId = model.ReportTypesId;
        //        marketsharereportdata.ReportDefinition.ReportGroupsId = model.ReportGroupsId;
        //        //marketsharereportdata.ReportDefinition.IsBaseReport = model.IsBaseReport;
        //        marketsharereportdata.ReportDefinition.IsGlobalShare = model.IsGlobalShare;
        //        marketsharereportdata.ReportDefinition.IsTemplateReport = model.IsTemplateReport;

        //        marketsharereportdata.ReportDefinition.Summary = model.Summary;
        //        marketsharereportdata.BaseYear = model.BaseYear;
        //        marketsharereportdata.YearType = model.YearType;
        //        marketsharereportdata.Trending = model.Trending;
        //        marketsharereportdata.ShowAdvancedFilter = model.ShowAdvancedFilter;
        //        marketsharereportdata.DatePeriodType = model.DatePeriodType;
        //        marketsharereportdata.DatePeriods = Convert.ToInt16(model.DatePeriods);
        //        marketsharereportdata.DateCalculationType = model.DateCalculationType;
        //        marketsharereportdata.OutputType = model.OutputType;
        //        marketsharereportdata.StartYearMonth = Convert.ToInt32(model.StartYearMonth);
        //        marketsharereportdata.EndYearMonth = Convert.ToInt32(model.EndYearMonth);
        //        marketsharereportdata.DataType1 = model.DataType1;
        //        marketsharereportdata.DataType2 = model.DataType2;
        //        marketsharereportdata.DataType3 = model.DataType3;
        //        marketsharereportdata.MarketLevel = Convert.ToInt16(model.MarketLevel);
        //        marketsharereportdata.Field1 = new Guid(model.Field1);
        //        marketsharereportdata.Field1SortBy = model.Field1SortBy;
        //        marketsharereportdata.Field1AscDesc = model.Field1AscDesc;
        //        // marketsharereportdata.Field1Group = model.Field1Group.ToString();
        //        marketsharereportdata.Field1Records = model.Field1Records;
        //        //marketsharereportdata.Field1TopN = model.Field1TopN.ToString();
        //        //marketsharereportdata.Field1TopNPercent = Convert.ToInt16(model.Field1TopNPercent);
        //        marketsharereportdata.Field2 = new Guid(model.Field2);
        //        marketsharereportdata.Field2SortBy = model.Field2SortBy;
        //        marketsharereportdata.Field2AscDesc = model.Field2AscDesc;
        //        //marketsharereportdata.Field2Group = model.Field2Group.ToString();
        //        marketsharereportdata.Field2Records = model.Field2Records;
        //        // marketsharereportdata.Field2TopN = model.Field2TopN.ToString();
        //        // marketsharereportdata.Field2TopNPercent = model.Field2TopNPercent.ToString();
        //        marketsharereportdata.Field3 = new Guid(model.Field3);
        //        marketsharereportdata.Field3SortBy = model.Field3SortBy;
        //        marketsharereportdata.Field3AscDesc = model.Field3AscDesc;
        //        // marketsharereportdata.Field3Group = model.Field3Group.ToString();
        //        marketsharereportdata.Field3Records = model.Field3Records;
        //        //marketsharereportdata.Field3TopN = model.Field3TopN.ToString();
        //        //marketsharereportdata.Field3TopNPercent = model.Field3TopNPercent.ToString();
        //        marketsharereportdata.ShowDischargesHospital = model.ShowDischargesHospital;
        //        marketsharereportdata.ShowDischargesState = model.ShowDischargesState;
        //        marketsharereportdata.ShowDischargesPercent = model.ShowDischargesPercent;
        //        marketsharereportdata.ShowLOSHospital = model.ShowLOSHospital;
        //        marketsharereportdata.ShowLOSState = model.ShowLOSState;
        //        marketsharereportdata.ShowLOSPercent = model.ShowLOSPercent;
        //        marketsharereportdata.ShowSummary = model.ShowSummary;
        //        marketsharereportdata.ShowFilter = model.ShowFilter;
        //        if (model.FilterIdCase != "" && model.FilterIdCase != null)
        //        {
        //            marketsharereportdata.FilterIdCase = new Guid(model.FilterIdCase);
        //        }
        //        else
        //        {
        //            marketsharereportdata.FilterIdCase = null;
        //        }
        //        if (model.FilterIdGeographic != "" && model.FilterIdGeographic != null)
        //        {
        //            marketsharereportdata.FilterIdGeographic = new Guid(model.FilterIdGeographic);
        //        }
        //        else
        //        {
        //            marketsharereportdata.FilterIdGeographic = null;
        //        }
        //        //special handling for mapping from dto to filter details table
        //        MarketShareFilterDetail newdetail;

        //        int sequencenumber = 0;
        //        foreach (var x in model.MarketShareLiveFilterDetailsList)
        //        {
        //            var selectedValues = filterRepository.DeformatString(x.DataDictionaryId, x.FieldValue, "");
        //            selectedValues = filterRepository.ReformatString(x.DataDictionaryId, selectedValues, "");
        //            newdetail = new MarketShareFilterDetail();
        //            newdetail.AndOr = x.AndOr;
        //            newdetail.CloseParenthesis = x.CloseParenthesis;
        //            newdetail.DataDictionaryId = x.DataDictionaryId;
        
        //            newdetail.FieldValue = selectedValues;
        //            newdetail.OpenParenthesis = x.OpenParenthesis;
        //            newdetail.Operation = x.Operation;
        //            newdetail.Sequence = sequencenumber;
        //            //at this point we would calculate the sql text for this line
        //            newdetail.SqlText = filterRepository.BuildSql(x.DataDictionaryId, selectedValues, x.Operation); //x.SQLText;
        //            newdetail.FilterId = x.FilterId;
        //            newdetail.Id = System.Guid.NewGuid();
        //            newdetail.MarketShareReportDataId = msId;
        //            sequencenumber++;
        //            marketsharereportdata.MarketShareFilterDetails.Add(newdetail);
        //        }
                
        //            reportRepository.Add(marketsharereportdata);

                
        //        // return View(model);
        //        return RedirectToAction("Index");
        //    }
        //    return RedirectToAction("Index");

        //
        //}

        //Ajax request handler for getting dictionary on change of pattype
        [HttpPost]
        public virtual ActionResult GetDictionaryList(String patType)
        {
            //try to use the emulate user function
            //I put the stop here to see if this ever gets called from the designer
            Profile = IdsProfile.GetUserProfile(User.Identity.Name);
            var sessionSink = Profile.SessionObject;
            var emulatename = string.Empty;
            if (sessionSink.UserId != sessionSink.LoginUserId)
            {
                var userId1 = sessionSink.UserId;
                var username = userRepository.GetUserNameById(userId1);
                Profile = IdsProfile.GetUserProfile(username);
                sessionSink = Profile.SessionObject;
                emulatename = username;
            }
            ViewBag.EmulateName = emulatename;

            var userId = sessionSink.UserId;
            var dataDictionary = reportRepository.GetFieldList(userId, patType);
            var DictionaryList = new List<DataDictionaryDto>();
            foreach (DataDictionaryGetFieldList_Result line in dataDictionary)
            {
                var details = new DataDictionaryDto();
                details.CodedField = line.CodedField;
                details.CodedJoinField = line.CodedJoinField;
                details.CodedTable = line.CodedTable;
                details.EndDate = line.EndDate.ToString();  //.ToShortDateString();
                details.FieldLabel = line.FieldLabel;
                details.FieldName = line.FieldName;
                details.FieldType = line.FieldType;
                details.Id = line.Id;
                details.IsCoded = line.IsCoded;
                details.IsGeographic = line.IsGeographic;
                details.IsLookup = line.IsLookup;
                details.IsUdf = line.IsUdf;
                details.LookupName = line.LookupName;
                details.PickListSort = line.PickListSort;
                details.StartDate = line.StartDate.ToShortDateString();
                details.UdfId = line.UdfId;
                details.UdfTable = line.UdfTable;

                DictionaryList.Add(details);
            }
            return Json(DictionaryList, JsonRequestBehavior.AllowGet);

        }
        //Ajax request handler for getting dictionary on change of pattype
        [HttpPost]
        public virtual ActionResult GetNonGeographicList(String patType)
        {
            //try to use the emulate user function
            Profile = IdsProfile.GetUserProfile(User.Identity.Name);
            var sessionSink = Profile.SessionObject;
            var emulatename = string.Empty;
            if (sessionSink.UserId != sessionSink.LoginUserId)
            {
                var userId1 = sessionSink.UserId;
                var username = userRepository.GetUserNameById(userId1);
                Profile = IdsProfile.GetUserProfile(username);
                sessionSink = Profile.SessionObject;
                emulatename = username;
            }
            ViewBag.EmulateName = emulatename;

            var userId = sessionSink.UserId;
            var dataDictNonGeographic = reportRepository.GetNonGeographicFieldList(userId, patType);

            var DictionaryGeographicList = new List<DataDictionaryDto>();
            foreach (DataDictionaryGetFieldList_Result line in dataDictNonGeographic)
            {
                var details = new DataDictionaryDto();
                details.CodedField = line.CodedField;
                details.CodedJoinField = line.CodedJoinField;
                details.CodedTable = line.CodedTable;
                details.EndDate = line.EndDate.ToString();  //.ToShortDateString();
                details.FieldLabel = line.FieldLabel;
                details.FieldName = line.FieldName;
                details.FieldType = line.FieldType;
                details.Id = line.Id;
                details.IsCoded = line.IsCoded;
                details.IsGeographic = line.IsGeographic;
                details.IsLookup = line.IsLookup;
                details.IsUdf = line.IsUdf;
                details.LookupName = line.LookupName;
                details.PickListSort = line.PickListSort;
                details.StartDate = line.StartDate.ToShortDateString();
                details.UdfId = line.UdfId;
                details.UdfTable = line.UdfTable;

                DictionaryGeographicList.Add(details);
            }
            return Json(DictionaryGeographicList, JsonRequestBehavior.AllowGet);

        }





        //Ajax request handler for getting filters on change of unit
        //no longer valid -- needs module
        [HttpPost]
        public virtual ActionResult GetGeographicFilters(String unitId)
        {
            //try to use the emulate user function
            Profile = IdsProfile.GetUserProfile(User.Identity.Name);
            var sessionSink = Profile.SessionObject;
            var emulatename = string.Empty;
            if (sessionSink.UserId != sessionSink.LoginUserId)
            {
                var userId1 = sessionSink.UserId;
                var username = userRepository.GetUserNameById(userId1);
                Profile = IdsProfile.GetUserProfile(username);
                sessionSink = Profile.SessionObject;
                emulatename = username;
            }
            ViewBag.EmulateName = emulatename;

            var userId = sessionSink.UserId;

            var geographicFilters = filterRepository.GetFilterByTypeAndIds(1, unitId, userId);
            var GeograhicFilterList = new List<LiveFiltersDto>();
            foreach (Mha.Ids.Data.Filter line in geographicFilters)
            {
                var details = new LiveFiltersDto();
                details.Id = line.Id.ToString();
                details.Description = line.Description;
                GeograhicFilterList.Add(details);
            }
            
            return Json(GeograhicFilterList, JsonRequestBehavior.AllowGet);

        }

        //Ajax request handler for getting filters on change of unit
        [HttpPost]
        public virtual ActionResult GetCaseFilters(String unitId)
        {
            //try to use the emulate user function
            Profile = IdsProfile.GetUserProfile(User.Identity.Name);
            var sessionSink = Profile.SessionObject;
            var emulatename = string.Empty;
            if (sessionSink.UserId != sessionSink.LoginUserId)
            {
                var userId1 = sessionSink.UserId;
                var username = userRepository.GetUserNameById(userId1);
                Profile = IdsProfile.GetUserProfile(username);
                sessionSink = Profile.SessionObject;
                emulatename = username;
            }
            ViewBag.EmulateName = emulatename;

            var userId = sessionSink.UserId;

            var caseFilters = filterRepository.GetFilterByTypeAndIds(2, unitId, userId);
            var CaseFilterList = new List<LiveFiltersDto>();
            foreach (Mha.Ids.Data.Filter line in caseFilters)
            {
                var details = new LiveFiltersDto();
                details.Id = line.Id.ToString();
                details.Description = line.Description;
                CaseFilterList.Add(details);
            }

            return Json(CaseFilterList, JsonRequestBehavior.AllowGet);
        }

        private void FillLookups(ReportDefinition reportdefinition)
        {
            ViewBag.ReportGroupsId = new SelectList(reportRepository.GetReportGroups(), "Id", "Description", reportdefinition.ReportGroupsId);
            ViewBag.ReportTypesId = new SelectList(reportRepository.GetReportTypes(), "Id", "Description", reportdefinition.ReportTypesId);
        }

        private void FillLookups()
        {
            ViewBag.ReportGroupsId = new SelectList(reportRepository.GetReportGroups(), "Id", "Description");
            ViewBag.ReportTypesId = new SelectList(reportRepository.GetReportTypes(), "Id", "Description");
        }

        //
        // GET: /Report/Delete/5
 
        public ActionResult Delete(Guid id)
        {
            ReportDefinition reportdefinition = reportRepository.GetReportdefinition(id);
            return View(reportdefinition);
        }
       
        //
        // POST: /Report/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ReportDefinition reportdefinition = reportRepository.GetReportdefinition(id);
            reportRepository.DeleteReportDefinition(reportdefinition);
            return RedirectToAction("Index");
        }
        public ActionResult HideReport(Guid id)
        {
            //try to use the emulate user function
            Profile = IdsProfile.GetUserProfile(User.Identity.Name);
            var sessionSink = Profile.SessionObject;
            var emulatename = string.Empty;
            if (sessionSink.UserId != sessionSink.LoginUserId)
            {
                var userId1 = sessionSink.UserId;
                var username = userRepository.GetUserNameById(userId1);
                Profile = IdsProfile.GetUserProfile(username);
                sessionSink = Profile.SessionObject;
                emulatename = username;
            }
            ViewBag.EmulateName = emulatename;

            var userId = sessionSink.LoginUserId;
            ReportDefinition reportdefinition = reportRepository.GetReportdefinition(id);
            var newdetail = new ReportDefinitionHidden();
            newdetail.ReportDefinitionsId = id;
            newdetail.UserId = userId;
            newdetail.Id = System.Guid.NewGuid();
            userRepository.AddHiddenReportLine(newdetail);
            userRepository.Save();
            return RedirectToAction("Index");
        }
        static void UpdateReportStatus(string reportRunId)
        {
            // to-do - update report status in various tables 
        }

        protected override void Dispose(bool disposing)
        {
            reportRepository.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult CancelEdit()
        {
            Profile = IdsProfile.GetUserProfile(User.Identity.Name);
            var sessionSink = Profile.SessionObject;

            //return RedirectToAction(Profile.SessionObject.MethodName, Profile.SessionObject.ControllerName);
            return RedirectToAction(Profile.SessionObject.MethodName, Profile.SessionObject.ControllerName, new {tabIndex = sessionSink.ReportMasterTab });
        }
    }
}
