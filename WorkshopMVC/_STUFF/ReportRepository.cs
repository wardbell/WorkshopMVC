using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using Mha.Ids.Data;
using System.Data.Entity;
using Mha.Ids.Data.Model;


namespace Mha.Ids.Data.Repository
{
    public class ReportRepository
    {
        private IdsDbEntities context = new IdsDbEntities();

        /// <summary>
        /// Three tables(Module, ModuleGroupLookup and Unit) are joined together 
        /// to returns a collection of custom object "UnitModules"
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ReportDefinition> GetAllReports(Guid userID)
        {
            var reportdefinitions = context.ReportDefinitions.Include(r => r.ReportGroupsLookup).Include(r => r.ReportTypeLookup).Where(f => f.IsBaseReport == true || f.IsGlobalShare == true || f.UserId == userID);

            return reportdefinitions.ToList();
        }
                
        
        public IEnumerable<ReportDefinition> GetAllReportsByType(Guid userID,string reportTypeId)
        {
            var reportdefinitions = context.ReportDefinitions.Include(r => r.ReportGroupsLookup).Include(r => r.ReportTypeLookup).Where(f => f.ReportTypesId == reportTypeId && (f.IsBaseReport == true || f.IsGlobalShare == true || f.UserId == userID) && f.IsTemplateReport == false);

            return reportdefinitions.ToList();
        }
        public IEnumerable<ReportDefinition> GetAllReportsByGroup(Guid userID, string reportGroupId)
        {
            var reportdefinitions = context.ReportDefinitions.Include(r => r.ReportGroupsLookup).Include(r => r.ReportTypeLookup).Where(f => f.ReportGroupsId == reportGroupId && (f.IsBaseReport == true || f.IsGlobalShare == true || f.UserId == userID));

            return reportdefinitions.ToList();
        }
        public IEnumerable<ReportDefinition> GetAllReportsIncludeNoneTemplate()
        {
            var reportdefinitions = context.ReportDefinitions.Include(r => r.ReportGroupsLookup).Include(r => r.ReportTypeLookup);

            return reportdefinitions.ToList();
        }
        //calls a stored procedure to validate the report - ti
        public IList<MarketShareValidateReport_Result> ValidateReport(Guid reportId, int reportType)
        {
            //reportType will be 0-regular, 1-live, 2-reportrun
            var newlist = context.MarketShareValidateReport(reportId,reportType).ToList();
            return newlist;
        }
        //calls a stored procedure to return all reports or just templated for a user - ti
        public IList<ReportsSubmittedTodayList_Result> GetReportsTodayListByUser(Guid userId,string unitId, string filtertype)
        {

            var newlist = context.ReportsSubmittedTodayList(userId,unitId, filtertype).ToList();
            return newlist;
        }
        //calls a stored procedure to return all reports or just templated for a group and user - ti
        //calls a stored procedure to return all reports or just templated for a user - ti
        public IList<ReportListbyUserIdTemplateOrReport_Result> GetReportListByUser(Guid userId, string templateOrReport)
        {

            var newlist = context.ReportListbyUserIdTemplateOrReport(userId, templateOrReport).ToList();
            return newlist;
        }
        //calls a stored procedure to return all reports or just templated for a group and user - ti
        public IList<ReportListbyReportGroupIdUserIdTemplateOrReport_Result> GetReportListbyGroupUser(string groupsid, Guid userId, string templateOrReport)
        {

            var newlist = context.ReportListbyReportGroupIdUserIdTemplateOrReport(groupsid, userId, templateOrReport).ToList();
            return newlist;
        }
               

        public ReportDefinition GetReportdefinition(Guid id)
        {
            return context.ReportDefinitions.Find(id);
        }

        public void DeleteReportDefinition(ReportDefinition reportdefinition)
        {
            context.ReportDefinitions.Remove(reportdefinition);
            context.SaveChanges();
        }

        public bool DeleteReport(Guid reportDefinitionId)
        {
            
            try
            {
                context.DeleteMarketShareReportRecords(reportDefinitionId);
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                throw;
                return false;
            }
             
            return false;
        }

        public bool IsReportShared(Guid reportDefinitionId)
        {
            var result = false;
            var x = context.ReportDefinitionUserShares.Where(r => r.ReportDefinitionsId.Equals(reportDefinitionId));
            if (x.Count() > 0)
            {
                result = true;
            }
            return result;
        }

        public bool IsReportScheduled(Guid reportDefinitionId)
        {
            var result = false;
            var x = context.ReportSchedulers.Where(r => r.ReportDefinitionsId.Equals(reportDefinitionId));
            if (x.Count() > 0)
            {
                result = true;
            }
            return result;
        }
        public void EditReportDefinition(ReportDefinition reportdefinition)
        {
            context.Entry(reportdefinition).State = EntityState.Modified;
            context.SaveChanges();
        }
        public IEnumerable<ReportGroupsLookup> GetReportGroups()
        {
            return context.ReportGroupsLookups.ToList();
        }
        //public IEnumerable<YearMonthLookup> GetDatesByModuleUnit(string moduleId,string unitId)
        //{
        //    //return context.YearMonthLookups.ToList();
        //}
        public IEnumerable<YearMonthLookup> GetDates()
        {
            return context.YearMonthLookups.ToList();
        }
        public IEnumerable<GetModuleDatesbyModuleUnit_Result> GetModuleDates(string unitId,string moduleId)
        {
            return context.GetModuleDatesbyModuleUnit(moduleId,unitId).ToList();
        }
        public IEnumerable<ReportTypeLookup> GetReportTypes()
        {
            return context.ReportTypeLookups.ToList();
        }
        public ReportTypeLookup GetReportTypeById(string reportTypeId)
        {
            return context.ReportTypeLookups.Find(reportTypeId); 
        }
        public void CreateReportDefinition(ReportDefinition reportdefinition)
        {
            context.ReportDefinitions.Add(reportdefinition);
            context.SaveChanges();
        }

        public IEnumerable<ReportDefinition> GetReportsByUserId(Guid userId)
        {
            return context.ReportDefinitions.Where(r=> r.UserId == userId ).ToList();
        }

        public MarketShareReportData GetMarketShareReportData(Guid reportDefinitionId)
        {
            return context.MarketShareReportDatas.FirstOrDefault(r => r.ReportDefinitionsId == reportDefinitionId);

        }
        public ReportRun GetReportRunFromReportDataId(Guid reportRunId)
        {
            return context.ReportRuns.FirstOrDefault(r => r.Id == reportRunId);

        }
        public MarketShareLiveReportData GetMarketShareLiveReportData(Guid id)
        {
            return context.MarketShareLiveReportDatas.FirstOrDefault(r => r.Id == id);

        }
        public IList<DataDictionaryGetFieldList_Result> GetFieldList(Guid userId,string pattype)
        {
            var moduleId = "MIDB";
            if (pattype == "O")
            {
                moduleId = "MODB";
            }
           
            var maxdate = context.ReportDatesMaxDateAvailableGetbyModuleId(moduleId);
            var newdate = DateTime.Parse(maxdate.First());
            var newlist = context.DataDictionaryGetFieldList(userId, newdate, false, "A", pattype, "MKTSHARE", "R").ToList();
           
           // var newlist = context.DataDictionaries.OrderBy(x => x.FieldLabel).ToList();
           
            return newlist;
        }
        public IList<DataDictionaryGetFieldList_Result> GetFieldList(Guid userId, string pattype, DateTime newdate)
        {
            var moduleId = "MIDB";
            if (pattype == "O")
            {
                moduleId = "MODB";
            }

            //var maxdate = context.ReportDatesMaxDateAvailableGetbyModuleId(moduleId);
           // var newdate = DateTime.Parse(maxdate.First());
            var newlist = context.DataDictionaryGetFieldList(userId, newdate, false, "A", pattype, "MKTSHARE", "R").ToList();

            // var newlist = context.DataDictionaries.OrderBy(x => x.FieldLabel).ToList();

            return newlist;
        }
        public IList<DataDictionaryGetFieldList_Result> GetNonGeographicFieldList(Guid userId, string pattype)
        {
            var moduleId = "MIDB";
            if (pattype == "O")
            {
                moduleId = "MODB";
            }
            var maxdate = context.ReportDatesMaxDateAvailableGetbyModuleId(moduleId);
            var newdate = DateTime.Parse(maxdate.First());
            var newlist = context.DataDictionaryGetFieldList(userId, newdate, false, "C", pattype, "MKTSHARE", "R").ToList();
           
            
            //var newlist = context.DataDictionaries.Where(x => x.IsGeographic == false).OrderBy(x => x.FieldLabel).ToList();
            
            return newlist;
        }
        public IList<DataDictionaryGetFieldList_Result> GetNonGeographicFieldList(Guid userId, string pattype, DateTime newdate)
        {
            var moduleId = "MIDB";
            if (pattype == "O")
            {
                moduleId = "MODB";
            }
            //var maxdate = context.ReportDatesMaxDateAvailableGetbyModuleId(moduleId);
            //var newdate = DateTime.Parse(maxdate.First());
            var newlist = context.DataDictionaryGetFieldList(userId, newdate, false, "C", pattype, "MKTSHARE", "R").ToList();


            //var newlist = context.DataDictionaries.Where(x => x.IsGeographic == false).OrderBy(x => x.FieldLabel).ToList();

            return newlist;
        }
        public IList<DataDictionaryGetFieldList_Result> GetGeographicFieldList(Guid userId, string pattype)
        {
            var moduleId = "MIDB";
            if (pattype == "O")
            {
                moduleId = "MODB";
            }
            var maxdate = context.ReportDatesMaxDateAvailableGetbyModuleId(moduleId);
            var newdate = DateTime.Parse(maxdate.First());
            var newlist = context.DataDictionaryGetFieldList(userId, newdate, false, "G", pattype, "MKTSHARE", "R").ToList();
           
            //var newlist = context.DataDictionaries.Where(x=> x.IsGeographic == true).OrderBy(x => x.FieldLabel).ToList();
            
            return newlist;
        }
        public IList<DataDictionaryGetFieldList_Result> GetGeographicFieldList(Guid userId, string pattype, DateTime newdate)
        {
            var moduleId = "MIDB";
            if (pattype == "O")
            {
                moduleId = "MODB";
            }
           
            var newlist = context.DataDictionaryGetFieldList(userId, newdate, false, "G", pattype, "MKTSHARE", "R").ToList();

            //var newlist = context.DataDictionaries.Where(x=> x.IsGeographic == true).OrderBy(x => x.FieldLabel).ToList();

            return newlist;
        }
        //Thom starting to support modules
        
        public IList<DataDictionaryGetFieldList_Result> GetFieldList(Guid userId, string pattype, DateTime newdate, string moduleId)
        {
            var modulerecord = GetModuleById(moduleId);
            var datatype = modulerecord.MHADataTypes;
            var newlist = context.DataDictionaryGetFieldList(userId, newdate, false, "A", datatype, "MKTSHARE", "R").ToList();

            // var newlist = context.DataDictionaries.OrderBy(x => x.FieldLabel).ToList();

            return newlist;
        }
       
        public IList<DataDictionaryGetFieldList_Result> GetNonGeographicFieldList(Guid userId, string pattype, DateTime newdate, string moduleId)
        {
            var modulerecord = GetModuleById(moduleId);
            var datatype = modulerecord.MHADataTypes;
            var newlist = context.DataDictionaryGetFieldList(userId, newdate, false, "C", datatype, "MKTSHARE", "R").ToList();


            return newlist;
        }
        
        public IList<DataDictionaryGetFieldList_Result> GetGeographicFieldList(Guid userId, string pattype, DateTime newdate, string moduleId)
        {
            var modulerecord = GetModuleById(moduleId);
            var datatype = modulerecord.MHADataTypes;

            var newlist = context.DataDictionaryGetFieldList(userId, newdate, false, "G", datatype, "MKTSHARE", "R").ToList();

            //var newlist = context.DataDictionaries.Where(x=> x.IsGeographic == true).OrderBy(x => x.FieldLabel).ToList();

            return newlist;
        }
        //end thom supporting modules
        public DateTime GetMaxPublishedDate()
        {
             var maxdate = context.ReportDatesMaxDateAvailableGetbyModuleId("MIDB");
             return DateTime.Parse(maxdate.First());
        }
        public void DeleteLines(List<MarketShareFilterDetail> oldLines)
        {
            foreach (var x in oldLines)
            {
                context.MarketShareFilterDetails.Remove(x);
            }
        }
        public void DeleteGeographicLines(List<MarketShareFilterGeographicDetail> oldLines)
        {
            foreach (var x in oldLines)
            {
                context.MarketShareFilterGeographicDetails.Remove(x);
            }
        }
        public void DeleteCensusLines(List<MarketShareFilterCensusDetail> oldLines)
        {
            foreach (var x in oldLines)
            {
                context.MarketShareFilterCensusDetails.Remove(x);
            }
        }
        public void DeleteCompareLines(List<MarketShareFilterCompareDetail> oldLines)
        {
            foreach (var x in oldLines)
            {
                context.MarketShareFilterCompareDetails.Remove(x);
            }
        }
        public void Save(MarketShareReportData report)
        {
            context.Entry(report).State = EntityState.Modified;
            context.SaveChanges();
            //thom added validation after a save
            context.MarketShareValidateSavedReport(report.ReportDefinitionsId);
        }
        public void Add(MarketShareReportData report)
        {
            context.Entry(report).State = EntityState.Added;
            context.SaveChanges();
            //thom added validation after a save
            context.MarketShareValidateSavedReport(report.ReportDefinitionsId);
        }
        public void AddLive(MarketShareLiveReportData report)
        {
            context.Entry(report).State = EntityState.Added;
            context.SaveChanges();
        }
        public void Dispose()
        {
            context.Dispose();
        }
        public IEnumerable<ReportDefinition> GetTemplateReportsByUserId(Guid userId)
        {
            return context.ReportDefinitions.Where(r =>  r.IsTemplateReport == true && ((r.UserId == userId && r.IsTemplateReport == true) || r.IsGlobalShare == true)).OrderBy(o => o.Description).ToList();
        }

        public IEnumerable<ReportDefinition> GetReportsForScheduler(Guid userID)
        {
            var reportdefinitions = context.ReportDefinitions.Include(r => r.ReportGroupsLookup).Include(r => r.ReportTypeLookup).Where(f => f.IsGlobalShare == true || f.UserId == userID);

            return reportdefinitions.ToList();
        }
        public Module GetModuleById(string id)
        {
            return context.Modules.FirstOrDefault(m => m.Id == id);
        }
        // mw: this function may move to a diff repository
         public IEnumerable<MarketShareReportData> GetMarketShareReportsForScheduler(Guid userId, string unitId)
         {
             var reports =
                 context.MarketShareReportDatas.Include(r => r.ReportDefinition).Where(
                     m => m.UserId == userId && m.UnitId == unitId);
             return reports.ToList();
         }

         public IEnumerable<ReportStorageDisplay> GetGeneratedReportFromStorageByUserId(Guid userId)
         {
             var q = from rs in context.ReportStorages
                     join rd in context.ReportDefinitions on rs.ReportDefinitionsId equals rd.Id
                     join rr in context.ReportRuns on rs.ReportRunId equals rr.Id
                     where rr.UserId == userId
                     select new ReportStorageDisplay
                                {
                                    StorageId =  rs.Id, 
                                    ModuleId = rd.ModuleId,
                                    GroupId = rd.ReportGroupsId,
                                    Description = rd.Description,
                                    RecordDateTime = rs.ReportGeneratedTime
                                };
                         
                         //.Include(r=>r.ReportRun).Where(r=>r.ReportRun.UserId.Equals(userId));
             return   q.ToList() ;
         }

        public byte[] GetReportContentByReportStorageId(Guid reportStorageId)
        {
            var content = context.ReportStorages.SingleOrDefault(r => r.Id == reportStorageId);
            return content.ReportContent;
        }

        /// <summary>
        ///  this function provide the full list of tempalte report list same as in tempalte screen
        /// </summary>
        /// <param name="userLoginId"></param>
        /// <param name="unitId"></param>
        /// <param name="type"></param>
        /// <param name="moduleId"></param>
        /// <param name="reportGroupId"></param>
        /// <param name="reportTypeId"></param>
        /// <returns></returns>
        public IEnumerable<ReportFilteredList_Result> GetReportListForSchedulerByUserIdUnitIdModuleIdGroupId_old(Guid userLoginId, string unitId, string type, string moduleId, string reportGroupId, string reportTypeId)
        {
            var result = context.ReportFilteredList(userLoginId, unitId, type, moduleId, reportGroupId, reportTypeId,null);
            return result;
        }

        public IEnumerable<ScheduledReport_GetAvailableReportsForScheduling_Result> GetReportListForSchedulerByUserIdUnitIdModuleIdGroupId(Guid userLoginId, string unitId, string type, string moduleId, string reportGroupId, string reportTypeId)
        {
            var result = context.ScheduledReport_GetAvailableReportsForScheduling(userLoginId, unitId, type, moduleId, reportGroupId, reportTypeId, null);
            return result;
        }

        public IEnumerable<ReportListForScheduler> GetReportListsForAjax(Guid userLoginId, string unitId, string type, string moduleId, string reportGroupId, string reportTypeId)
        {
            // this list provide a list of active scheduled report by user and user is a contact of
            var q = context.ReportFilteredList(userLoginId, unitId, type, moduleId, reportGroupId, reportTypeId, null);
            var x = from y in q
                    select new ReportListForScheduler
                    {
                        ReportId = y.ReportId,
                        ReportDescription = y.ReportDescription,
                        ModuleDescription = y.ModuleDescription
                    };
            return x.ToList();
        }

        public IEnumerable<UserObjectList> GetUserContactListForScheduler(string selectedUnitId, string moduleFilter, string groupFilter)
         {
             var q = from u in context.UnitUserModuleReportGroupsLookupXrefs
                                 join user in context.Users on u.UserId equals user.Id
                                 where
                                     u.UnitId == selectedUnitId && u.ModuleId == moduleFilter &&
                                     u.ReportGroupsLookupId == groupFilter && user.IsApproved == true 
                                 select new UserObjectList 
                                 {
                                     Id = user.Id,
                                     Description = user.FirstName + " " + user.LastName 

                                 };
             return q;

         }

        public IEnumerable<GenericObjectList> GetSchedulableReportsByUserId(Guid userId)
         {
             var q = from u in context.ScheduledReport_GetSchedulableReportsByUserId(userId)
                                 select new GenericObjectList 
                                 {
                                     ObjectId  = u.Id.ToString(),
                                     ObjectDescription  = u.Description 
                                 };
             return q.OrderBy(r=>r.ObjectDescription);

         }

        public string GetFirstReportIdByUserId(Guid userId)
        {
            string q;
            try
            {
                q =
                   (from u in
                        context.ScheduledReport_GetSchedulableReportsByUserId(userId).OrderBy(
                            u => u.Description)
                    select u).FirstOrDefault().Id.ToString();
            }
            catch
            {
                q = "";
            }
            return q;
        }

        public IEnumerable<ReportMasterList_Result> GetReportMasterList(string moduleFilter, string groupFilter, string typeFilter, string selectedUnitId, Guid userLoginId)
        {
            return context.ReportMasterList(userLoginId, selectedUnitId, moduleFilter, groupFilter, typeFilter).ToList();
        }

        public IEnumerable<ReportFilteredListObject> GetReportFilteredList(string reportortemplate, string moduleFilter, string groupFilter, string typeFilter, string selectedUnitId, Guid userLoginId, string showall)
        {
             var q = context.ReportFilteredList(userLoginId, selectedUnitId, reportortemplate, moduleFilter, groupFilter, typeFilter, showall);
             var x = from y in q
                 select new ReportFilteredListObject
                 {
                    ReportRunId = y.ReportRunId,
                    ProductDescription = y.ProductDescription,
                    ModuleDescription = y.ModuleDescription,
                    GroupDescription = y.GroupDescription,
                    TypeDescription = y.TypeDescription,
                    ReportDescription = y.ReportDescription,
                    ReportId = y.ReportId,
                    IsBaseReport = y.IsBaseReport,
                    IsGlobalShare = y.IsGlobalShare,
                    IsTemplateReport = y.IsTemplateReport,
                    IsReadyToRun = y.IsReadyToRun,
                    SampleReportLink = y.SampleReportLink ,
                    Summary = y.Summary,
                    UserId = y.UserId,
                    LastEndDate = y.LastEndDate,
                    ReportRunDataId = y.ReportRunDataId,
                    ReportStatus = y.ReportStatus,
                    ReportDriverId = y.ReportDriverId,
                    OutputType = y.OutputType ,
                    ReportOwnerFullName = y.ReportOwnerFullName,
                   // ValidSummary = y.ValidSummary,
                    ValidSummary = (string.IsNullOrEmpty(y.ValidSummary))?y.ValidSummary:y.ValidSummary.Replace(System.Environment.NewLine, "<br />"),
                   
                   // ValidSummary = y.ValidSummary.Replace("\r\n","<br />"),
                    ValidRequest = y.ValidRequest,
                    ValidEstimatedTime = y.ValidEstimatedTime,
                    ValidMessage = y.ValidMessage,
                    IsHipaaMining = y.IsHipaaMining
                 };
             return x.ToList();              

        }

        public List<ReportsSubmittedFilteredList_Result> GetReportsSubmittedFilteredList(string moduleFilter, string groupFilter, string typeFilter, string selectedUnitId, Guid userLoginId)
        {
            return context.ReportsSubmittedFilteredList(userLoginId, selectedUnitId, moduleFilter, groupFilter, typeFilter).ToList();
        }
         
        // mwu - this function provide a list of submitted report scheduled by other user other than the log in user.
        //       The log in user is in contact list to receive these report
        public List<ScheduledReportsSubmittedFilteredList_Result> GetScheduledReportsSubmittedFilteredList(string moduleFilter, string groupFilter, string typeFilter, string selectedUnitId, Guid userLoginId)
        {
            return context.ScheduledReportsSubmittedFilteredList(userLoginId, selectedUnitId, moduleFilter, groupFilter, typeFilter).ToList();
        }
        /// <summary>
        /// mw: 8/23/12 this function will combine user ran reports from the screen and scheduled reports together
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SchedulerRunStatusObject> GetUserReportList(string moduleFilter, string groupFilter, string typeFilter, string selectedUnitId, Guid userLoginId)
        {
            var q1 = context.ReportsSubmittedFilteredList(userLoginId, selectedUnitId, moduleFilter, groupFilter, typeFilter);
            var q2 = context.ScheduledReportsSubmittedFilteredList(userLoginId, selectedUnitId, moduleFilter, groupFilter, typeFilter).Where(r=>r.ReportStatus!="Failed");
            var q3 = context.ReportResultHiddens.Where(r => r.UserId.Equals(userLoginId));
        
            var x1 = from y in q1
                     where !q3.AsEnumerable().Any(r2 => y.ReportRunId == r2.ReportRunId && y.UserId == r2.UserId)  
                    select new SchedulerRunStatusObject
                    {
                        ReportRunId  =  y.ReportRunId,
                        ReportDescription = y.ReportDescription,
                        ReportRunTimeStarted = y.TimeStarted.Value.Date,
                        ReportRunStatus = y.ReportStatus,
                        CreatedBy = null,
                        ReportDriverId = y.ReportDriverId,
                        ReportOupputType = y.OutputType,
                        ReportOwnerFirstName = y.ReportOwnerFirstName,
                        ReportOwnerLastName = y.ReportOwnerLastName,
                        ReportOwnerFullName = y.ReportOwnerLastName + ", " + y.ReportOwnerFirstName ,
                        IsHipaaMining = y.IsHipaaMining
                    };
            var x2 = from y1 in q2
                     where !q3.AsEnumerable().Any(r2 => y1.Id == r2.ReportRunId && y1.ReportSchedulerId == r2.ReportSchedulerId && y1.UserId == r2.ReportScheduledById)  
                     select new SchedulerRunStatusObject
                     {
                         ReportRunId = y1.Id,
                         ReportDescription = y1.Description,
                         ReportRunTimeStarted = y1.TimeStarted.Value.Date,
                         ReportRunStatus = y1.ReportStatus,
                         CreatedBy = y1.ScheduledBy,
                         ReportDriverId = y1.ReportDriverId,
                         ReportOupputType = y1.OutputType,
                         ReportOwnerFirstName = y1.ReportOwnerFirstName,
                         ReportOwnerLastName = y1.ReportOwnerLastName ,
                         ReportOwnerFullName = y1.ReportOwnerLastName + ", " + y1.ReportOwnerFirstName ,
                         IsHipaaMining = y1.IsHIPAAMining
                     };
            var resultUnion = x1.Union(x2);
             
            return resultUnion.ToList();
        }

        public IEnumerable<SchedulerRunStatusObject> GetUserReportList_ori(string moduleFilter, string groupFilter, string typeFilter, string selectedUnitId, Guid userLoginId)
        {
            
            var q1 = context.ReportsSubmittedFilteredList(userLoginId, selectedUnitId, moduleFilter, groupFilter, typeFilter);
            var q2 = context.ScheduledReportsSubmittedFilteredList(userLoginId, selectedUnitId, moduleFilter, groupFilter, typeFilter);
            var x1 = from y in q1
                    select new SchedulerRunStatusObject
                    {
                        ReportRunId  =  y.ReportRunId,
                        ReportDescription = y.ReportDescription,
                        ReportRunTimeStarted = y.TimeStarted,
                        ReportRunStatus = y.ReportStatus,
                        CreatedBy = null,
                        ReportDriverId = y.ReportDriverId,
                        ReportOupputType = y.OutputType
                    };
            var x2 = from y1 in q2
                     select new SchedulerRunStatusObject
                     {
                         ReportRunId = y1.Id,
                         ReportDescription = y1.Description,
                         ReportRunTimeStarted = y1.TimeStarted,
                         ReportRunStatus = y1.ReportStatus,
                         CreatedBy = y1.ScheduledBy,
                         ReportDriverId = y1.ReportDriverId,
                         ReportOupputType = y1.OutputType 
                     };
            var resultUnion = x1.Union(x2);
           
            return resultUnion.ToList();
        }

        public ObjectResult<System.Nullable<Guid>> MarketShareRunAdd(Guid id, string unitId, Guid userLoginId, string schedulerId, int batchId)
        {
            return context.MarketShareRunAddReport(id, unitId, userLoginId, 0, schedulerId, batchId);
        }
        public ObjectResult<System.Nullable<Guid>> MarketShareLiveRunAdd(Guid id, string unitId, Guid userLoginId, string schedulerId, int batchId)
        {
            return context.MarketShareRunAddReport(id, unitId, userLoginId, 1, schedulerId, batchId);
        }

        public bool IsReportDescriptionUnique(Guid userId, Guid excludeId, string description, string saveMode  )
        {
            var descriptionList = context.ReportDescriptionUniqueTest(userId, excludeId, description, "R", saveMode );
            if (descriptionList.Count() > 0)
            {
                return false;
            }
            return true;
        }

       // public IEnumerable<GetAllReportsRunByUser_Result> GetAllReportsRun(DateTime startDate, DateTime endDate) 
        public IEnumerable<GetAllReportsRunByUser_Result> GetAllReportsRun() 
        {
            // this list provide a list of all the run reports  
            var q = context.GetAllReportsRunByUser();      
            return q.ToList();
        }
        public IEnumerable<GetAllMarketShareReportsRunByUserWithFilters_Result> GetAllMarketShareReportsRunByUserWithFilters(DateTime startDate, DateTime endDate)
        //public IEnumerable<GetAllMarketShareReportsRunByUserWithFilters_Result> GetAllMarketShareReportsRunByUserWithFilters()
        {
            // this list provide a list of all the run reports  
            var q = context.GetAllMarketShareReportsRunByUserWithFilters(startDate,endDate);
            return q.ToList();
        }

        public IEnumerable<ReportsHipaaMiningList_Result> GetAllUserRanDataMiningReports()
        {
            return context.ReportsHipaaMiningList().ToList();
        }

        public IEnumerable<GetDataMiningTrackingLogRecords_Result> GetDataMiningTrackingLogRecords()
        {
            return context.GetDataMiningTrackingLogRecords().ToList();
        }

        public void ResetHippaFlagByReportRunId(Guid loginUserId, Guid  reportRunId, string reason)
        {
            context.ReportsHipaaMiningReset(loginUserId, reportRunId, reason);
        }
        

    }
}
