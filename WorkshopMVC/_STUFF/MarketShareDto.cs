using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Mha.Ids.Data.Model 
{


    public class MarketShareDto
    {
        public string Id { get; set; }
        public string ReportDefinitionsId { get; set; }
        public string UserId { get; set; }
        public string UnitId { get; set; }
        public string ReportTypesId { get; set; }
        public string ReportGroupsId { get; set; }
        public string ModuleId { get; set; }
        public bool IsGlobalShare { get; set; }
        public bool IsBaseReport { get; set; }
        public bool IsTemplateReport { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public string BaseYear { get; set; }
        public string YearType { get; set; }
        public string Trending { get; set; }
        public string DatePeriodType { get; set; }
        public string DatePeriods { get; set; }
        public string DateCalculationType { get; set; }
        public string StartYearMonth { get; set; }
        public string EndYearMonth { get; set; }
        public string DataType1 { get; set; }
        public string DataType2 { get; set; }
        public string DataType3 { get; set; }
        public string MarketLevel { get; set; }
        public string Field1 { get; set; }
        public string Field1SortBy { get; set; }
        public string Field1AscDesc { get; set; }
        public string Field1Group { get; set; }
        public string Field1Records { get; set; }
        public string Field1TopN { get; set; }
        public string Field1TopNPercent { get; set; }
        public string Field2 { get; set; }
        public string Field2SortBy { get; set; }
        public string Field2AscDesc { get; set; }
        public string Field2Group { get; set; }
        public string Field2Records { get; set; }
        public string Field2TopN { get; set; }
        public string Field2TopNPercent { get; set; }
        public string Field3 { get; set; }
        public string Field3SortBy { get; set; }
        public string Field3AscDesc { get; set; }
        public string Field3Group { get; set; }
        public string Field3Records { get; set; }
        public string Field3TopN { get; set; }
        public string Field3TopNPercent { get; set; }
        public bool? ShowDischargesHospital { get; set; }
        public bool? ShowDischargesState { get; set; }
        public bool? ShowDischargesPercent { get; set; }
        public bool? ShowLOSHospital { get; set; }
        public bool? ShowLOSState { get; set; }
        public bool? ShowLOSPercent { get; set; }
        public bool? ShowHospitalPercent { get; set; }
        public bool? ShowLOSHospitalPercent { get; set; }
        public bool? ShowField1 { get; set; }
        public bool? ShowField1Description { get; set; }
        public bool? ShowField2 { get; set; }
        public bool? ShowField2Description { get; set; }
        public bool? ShowField3 { get; set; }
        public bool? ShowField3Description { get; set; }
        public bool? ShowSummary { get; set; }
        public bool? ShowFilter { get; set; }
        public bool? ShowAdvancedFilter { get; set; }
        public string OutputType { get; set; }
        public string FilterIdGeographic { get; set; }
        public string FilterIdCase { get; set; }
        public string ActionMethod { get; set; }
        public string NewFilterName { get; set; }
        public string NewGeographicFilterName { get; set; }

        public IList<LiveFilterDetailDto> MarketShareLiveFilterDetailsList { get; set; }
        public IList<LiveFilterDetailDto> MarketShareLiveFilterGeographicDetailsList { get; set; }
        public IList<LiveFilterDetailDto> MarketShareLiveFilterCensusDetailsList { get; set; }
        public IList<LiveFilterDetailDto> MarketShareLiveFilterCompareDetailsList { get; set; }

        public IList<DataDictionaryDto> DictionaryList { get; set; }
        public IList<DataDictionaryDto> DictionaryNonGeographicList { get; set; }
        public IList<DataDictionaryDto> DictionaryGeographicList { get; set; }
        
        public IList<LiveDatesDto> DateList { get; set; }
        public IList<LiveFiltersDto> GeograhicFilterList { get; set; }
        public IList<LiveFiltersDto> CaseFilterList { get; set; }

        public static MarketShareDto Create(
            MarketShareLiveReportData liveReportData, 
            MarketShareReportData reportdata, 
            IEnumerable<GetModuleDatesbyModuleUnit_Result> reportdates, 
            Guid userid, 
            IEnumerable<DataDictionaryGetFieldList_Result> dataDictionary, 
            IEnumerable<DataDictionaryGetFieldList_Result> dataDictionaryNonGeographic, 
            IEnumerable<DataDictionaryGetFieldList_Result> dataDictionaryGeographic, 
            IEnumerable<FilterListbyUserIdModuleId_Result> geographicFilters, 
            IEnumerable<FilterListbyUserIdModuleId_Result> caseFilters, 
            string mode)
        {
            var newId = System.Guid.NewGuid();
            var dto = new MarketShareDto();
            dto.Id = newId.ToString();
            
            dto.UserId = userid.ToString();

            if (mode == "live" || mode == "liveedit" || mode == "liveupdate" || mode == "liveupdateDate" || mode == "SaveFilter" || mode == "SaveGeographicFilter" || mode == "SaveCancel" || mode == "SaveCancelDuplicateReportName")
            {
                dto.UnitId = liveReportData.UnitId;
                dto.ReportDefinitionsId = liveReportData.ReportDefinitionsId.ToString();
                dto.ReportTypesId = liveReportData.ReportTypesId;
                dto.ReportGroupsId = liveReportData.ReportGroupsId;
                dto.ModuleId = liveReportData.ModuleId;
                dto.IsGlobalShare = liveReportData.IsGlobalShare;
                dto.IsBaseReport = liveReportData.IsBaseReport;
                dto.IsTemplateReport = liveReportData.IsTemplateReport;

                dto.Description = liveReportData.Description;
                dto.Summary = liveReportData.Summary;
                dto.BaseYear = liveReportData.BaseYear;
                dto.YearType = liveReportData.YearType;
                dto.Trending = liveReportData.Trending;
                dto.DatePeriodType = liveReportData.DatePeriodType;
                dto.DatePeriods = liveReportData.DatePeriods.ToString();
                dto.DateCalculationType = liveReportData.DateCalculationType;
                dto.OutputType = liveReportData.OutputType;
                dto.StartYearMonth = liveReportData.StartYearMonth.ToString();
                dto.EndYearMonth = liveReportData.EndYearMonth.ToString();
                dto.DataType1 = liveReportData.DataType1;
                dto.DataType2 = liveReportData.DataType2;
                dto.DataType3 = liveReportData.DataType3;
                dto.MarketLevel = liveReportData.MarketLevel.ToString();
                dto.Field1 = liveReportData.Field1.ToString();
                dto.Field1SortBy = liveReportData.Field1SortBy;
                dto.Field1AscDesc = liveReportData.Field1AscDesc;
                dto.Field1Group = liveReportData.Field1Group.ToString();
                dto.Field1Records = liveReportData.Field1Records;
                dto.Field1TopN = liveReportData.Field1TopN.ToString();
                dto.Field1TopNPercent = liveReportData.Field1TopNPercent.ToString();
                dto.Field2 = liveReportData.Field2.ToString();
                dto.Field2SortBy = liveReportData.Field2SortBy;
                dto.Field2AscDesc = liveReportData.Field2AscDesc;
                dto.Field2Group = liveReportData.Field2Group.ToString();
                dto.Field2Records = liveReportData.Field2Records;
                dto.Field2TopN = liveReportData.Field2TopN.ToString();
                dto.Field2TopNPercent = liveReportData.Field2TopNPercent.ToString();
                dto.Field3 = liveReportData.Field3.ToString();
                dto.Field3SortBy = liveReportData.Field3SortBy;
                dto.Field3AscDesc = liveReportData.Field3AscDesc;
                dto.Field3Group = liveReportData.Field3Group.ToString();
                dto.Field3Records = liveReportData.Field3Records;
                dto.Field3TopN = liveReportData.Field3TopN.ToString();
                dto.Field3TopNPercent = liveReportData.Field3TopNPercent.ToString();
                dto.ShowDischargesHospital = liveReportData.ShowDischargesHospital;
                dto.ShowDischargesState = liveReportData.ShowDischargesState;
                dto.ShowDischargesPercent = liveReportData.ShowDischargesPercent;
                dto.ShowLOSHospital = liveReportData.ShowLOSHospital;
                dto.ShowLOSState = liveReportData.ShowLOSState;
                dto.ShowLOSPercent = liveReportData.ShowLOSPercent;
                dto.ShowHospitalPercent = liveReportData.ShowHospitalPercent;
                dto.ShowLOSHospitalPercent = liveReportData.ShowLOSHospitalPercent;
                dto.ShowField1 = liveReportData.ShowField1;
                dto.ShowField1Description = liveReportData.ShowField1Description;
                dto.ShowField2 = liveReportData.ShowField2;
                dto.ShowField2Description = liveReportData.ShowField2Description;
                dto.ShowField3 = liveReportData.ShowField3;
                dto.ShowField3Description = liveReportData.ShowField3Description;
                dto.ShowSummary = liveReportData.ShowSummary;
                dto.ShowFilter = liveReportData.ShowFilter;
                dto.ShowAdvancedFilter = liveReportData.ShowAdvancedFilter;
                dto.FilterIdGeographic = liveReportData.FilterIdGeographic.ToString();
                dto.FilterIdCase = liveReportData.FilterIdCase.ToString();
                dto.ActionMethod = "NotSet";
                //get normal case filter
                dto.MarketShareLiveFilterDetailsList = new List<LiveFilterDetailDto>();
                var ordererdtable = from o in liveReportData.MarketShareLiveFilterDetails
                                    orderby o.Sequence
                                    select o;
                foreach (MarketShareLiveFilterDetail line in ordererdtable)
                {
                    var details = new LiveFilterDetailDto();
                    details.MarketShareLiveReportDataId = line.MarketShareLiveReportDataId;
                    details.AndOr = line.AndOr;
                    details.CloseParenthesis = line.CloseParenthesis;
                    details.DataDictionaryId = line.DataDictionaryId;
                  

                    details.FieldValue = line.FieldValue;
                    details.FilterId = line.FilterId;
                    details.Id = line.Id;
                    details.OpenParenthesis = line.OpenParenthesis;
                    details.Operation = line.Operation;
                    details.Sequence = line.Sequence;
                    details.SqlText = line.SqlText;
                    details.FieldType = line.FieldType;
                    dto.MarketShareLiveFilterDetailsList.Add(details);
                }
                //get geographic filter
                dto.MarketShareLiveFilterGeographicDetailsList = new List<LiveFilterDetailDto>();
                var ordererdtableG = from o in liveReportData.MarketShareLiveFilterGeographicDetails
                                    orderby o.Sequence
                                    select o;
                foreach (MarketShareLiveFilterGeographicDetail line in ordererdtableG)
                {
                    var details = new LiveFilterDetailDto();
                    details.MarketShareLiveReportDataId = line.MarketShareLiveReportDataId;
                    details.AndOr = line.AndOr;
                    details.CloseParenthesis = line.CloseParenthesis;
                    details.DataDictionaryId = line.DataDictionaryId;
                    

                    details.FieldValue = line.FieldValue;
                    details.FilterId = line.FilterId;
                    details.Id = line.Id;
                    details.OpenParenthesis = line.OpenParenthesis;
                    details.Operation = line.Operation;
                    details.Sequence = line.Sequence;
                    details.SqlText = line.SqlText;
                    details.FieldType = line.FieldType;
                    dto.MarketShareLiveFilterGeographicDetailsList.Add(details);
                }
                //get census filter
                dto.MarketShareLiveFilterCensusDetailsList = new List<LiveFilterDetailDto>();
                var ordererdtableCen = from o in liveReportData.MarketShareLiveFilterCensusDetails
                                     orderby o.Sequence
                                     select o;
                foreach (MarketShareLiveFilterCensusDetail line in ordererdtableCen)
                {
                    var details = new LiveFilterDetailDto();
                    details.MarketShareLiveReportDataId = line.MarketShareLiveReportDataId;
                    details.AndOr = line.AndOr;
                    details.CloseParenthesis = line.CloseParenthesis;
                    details.DataDictionaryId = line.DataDictionaryId;
                    

                    details.FieldValue = line.FieldValue;
                    details.FilterId = line.FilterId;
                    details.Id = line.Id;
                    details.OpenParenthesis = line.OpenParenthesis;
                    details.Operation = line.Operation;
                    details.Sequence = line.Sequence;
                    details.SqlText = line.SqlText;
                    details.FieldType = line.FieldType;
                    dto.MarketShareLiveFilterGeographicDetailsList.Add(details);
                }
                //get compare filter
                dto.MarketShareLiveFilterCompareDetailsList = new List<LiveFilterDetailDto>();
                var ordererdtableComp = from o in liveReportData.MarketShareLiveFilterCompareDetails
                                       orderby o.Sequence
                                       select o;
                foreach (MarketShareLiveFilterCompareDetail line in ordererdtableComp)
                {
                    var details = new LiveFilterDetailDto();
                    details.MarketShareLiveReportDataId = line.MarketShareLiveReportDataId;
                    details.AndOr = line.AndOr;
                    details.CloseParenthesis = line.CloseParenthesis;
                    details.DataDictionaryId = line.DataDictionaryId;
                    

                    details.FieldValue = line.FieldValue;
                    details.FilterId = line.FilterId;
                    details.Id = line.Id;
                    details.OpenParenthesis = line.OpenParenthesis;
                    details.Operation = line.Operation;
                    details.Sequence = line.Sequence;
                    details.SqlText = line.SqlText;
                    details.FieldType = line.FieldType;
                    dto.MarketShareLiveFilterCompareDetailsList.Add(details);
                }
            }
            else
            {
                dto.UnitId = reportdata.UnitId;
                dto.ReportDefinitionsId = reportdata.ReportDefinitionsId.ToString();
                dto.ReportTypesId = reportdata.ReportDefinition.ReportTypesId;
                dto.ReportGroupsId = reportdata.ReportDefinition.ReportGroupsId;
                dto.ModuleId = reportdata.ReportDefinition.ModuleId;
                dto.IsGlobalShare = reportdata.ReportDefinition.IsGlobalShare;
                dto.IsBaseReport = reportdata.ReportDefinition.IsBaseReport;
                dto.IsTemplateReport = reportdata.ReportDefinition.IsTemplateReport;

                dto.Description = reportdata.ReportDefinition.Description;
                dto.Summary = reportdata.ReportDefinition.Summary;
                dto.BaseYear = reportdata.BaseYear;
                dto.YearType = reportdata.YearType;
                dto.Trending = reportdata.Trending;
                dto.DatePeriodType = reportdata.DatePeriodType;
                dto.DatePeriods = reportdata.DatePeriods.ToString();
                dto.DateCalculationType = reportdata.DateCalculationType;
                dto.OutputType = reportdata.OutputType;
                dto.StartYearMonth = reportdata.StartYearMonth.ToString();
                dto.EndYearMonth = reportdata.EndYearMonth.ToString();
                dto.DataType1 = reportdata.DataType1;
                dto.DataType2 = reportdata.DataType2;
                dto.DataType3 = reportdata.DataType3;
                dto.MarketLevel = reportdata.MarketLevel.ToString();
                dto.Field1 = reportdata.Field1.ToString();
                dto.Field1SortBy = reportdata.Field1SortBy;
                dto.Field1AscDesc = reportdata.Field1AscDesc;
                dto.Field1Group = reportdata.Field1Group.ToString();
                dto.Field1Records = reportdata.Field1Records;
                dto.Field1TopN = reportdata.Field1TopN.ToString();
                dto.Field1TopNPercent = reportdata.Field1TopNPercent.ToString();
                dto.Field2 = reportdata.Field2.ToString();
                dto.Field2SortBy = reportdata.Field2SortBy;
                dto.Field2AscDesc = reportdata.Field2AscDesc;
                dto.Field2Group = reportdata.Field2Group.ToString();
                dto.Field2Records = reportdata.Field2Records;
                dto.Field2TopN = reportdata.Field2TopN.ToString();
                dto.Field2TopNPercent = reportdata.Field2TopNPercent.ToString();
                dto.Field3 = reportdata.Field3.ToString();
                dto.Field3SortBy = reportdata.Field3SortBy;
                dto.Field3AscDesc = reportdata.Field3AscDesc;
                dto.Field3Group = reportdata.Field3Group.ToString();
                dto.Field3Records = reportdata.Field3Records;
                dto.Field3TopN = reportdata.Field3TopN.ToString();
                dto.Field3TopNPercent = reportdata.Field3TopNPercent.ToString();
                dto.ShowDischargesHospital = reportdata.ShowDischargesHospital;
                dto.ShowDischargesState = reportdata.ShowDischargesState;
                dto.ShowDischargesPercent = reportdata.ShowDischargesPercent;
                dto.ShowLOSHospital = reportdata.ShowLOSHospital;
                dto.ShowLOSState = reportdata.ShowLOSState;
                dto.ShowLOSPercent = reportdata.ShowLOSPercent;
                dto.ShowHospitalPercent = reportdata.ShowHospitalPercent;
                dto.ShowLOSHospitalPercent = reportdata.ShowLOSHospitalPercent;
                dto.ShowField1 = reportdata.ShowField1;
                dto.ShowField1Description = reportdata.ShowField1Description;
                dto.ShowField2 = reportdata.ShowField2;
                dto.ShowField2Description = reportdata.ShowField2Description;
                dto.ShowField3 = reportdata.ShowField3;
                dto.ShowField3Description = reportdata.ShowField3Description;
                dto.ShowSummary = reportdata.ShowSummary;
                dto.ShowFilter = reportdata.ShowFilter;
                dto.ShowAdvancedFilter = reportdata.ShowAdvancedFilter;
                dto.FilterIdGeographic = reportdata.FilterIdGeographic.ToString();
                dto.FilterIdCase = reportdata.FilterIdCase.ToString();
                dto.ActionMethod = "Run";
                dto.MarketShareLiveFilterDetailsList = new List<LiveFilterDetailDto>();
                var ordererdtable = from o in reportdata.MarketShareFilterDetails
                                    orderby o.Sequence
                                    select o;
                foreach (MarketShareFilterDetail line in ordererdtable)
                {
                    var details = new LiveFilterDetailDto();
                    details.MarketShareLiveReportDataId = line.MarketShareReportDataId;
                    details.AndOr = line.AndOr;
                    details.CloseParenthesis = line.CloseParenthesis;
                    details.DataDictionaryId = line.DataDictionaryId;
                    

                    details.FieldValue = line.FieldValue;
                    details.FilterId = line.FilterId;
                    details.Id = line.Id;
                    details.OpenParenthesis = line.OpenParenthesis;
                    details.Operation = line.Operation;
                    details.Sequence = line.Sequence;
                    details.SqlText = line.SqlText;
                    details.FieldType = line.FieldType;
                    dto.MarketShareLiveFilterDetailsList.Add(details);
                }
                //get the geographic filters
                dto.MarketShareLiveFilterGeographicDetailsList = new List<LiveFilterDetailDto>();
                var ordererdtableG = from o in reportdata.MarketShareFilterGeographicDetails
                                    orderby o.Sequence
                                    select o;
                foreach (MarketShareFilterGeographicDetail line in ordererdtableG)
                {
                    var details = new LiveFilterDetailDto();
                    details.MarketShareLiveReportDataId = line.MarketShareReportDataId;
                    details.AndOr = line.AndOr;
                    details.CloseParenthesis = line.CloseParenthesis;
                    details.DataDictionaryId = line.DataDictionaryId;
                   

                    details.FieldValue = line.FieldValue;
                    details.FilterId = line.FilterId;
                    details.Id = line.Id;
                    details.OpenParenthesis = line.OpenParenthesis;
                    details.Operation = line.Operation;
                    details.Sequence = line.Sequence;
                    details.SqlText = line.SqlText;
                    details.FieldType = line.FieldType;
                    dto.MarketShareLiveFilterGeographicDetailsList.Add(details);
                }
                //get the census filters
                dto.MarketShareLiveFilterCensusDetailsList = new List<LiveFilterDetailDto>();
                var ordererdtableCen = from o in reportdata.MarketShareFilterCensusDetails
                                     orderby o.Sequence
                                     select o;
                foreach (MarketShareFilterCensusDetail line in ordererdtableCen)
                {
                    var details = new LiveFilterDetailDto();
                    details.MarketShareLiveReportDataId = line.MarketShareReportDataId;
                    details.AndOr = line.AndOr;
                    details.CloseParenthesis = line.CloseParenthesis;
                    details.DataDictionaryId = line.DataDictionaryId;
                   

                    details.FieldValue = line.FieldValue;
                    details.FilterId = line.FilterId;
                    details.Id = line.Id;
                    details.OpenParenthesis = line.OpenParenthesis;
                    details.Operation = line.Operation;
                    details.Sequence = line.Sequence;
                    details.SqlText = line.SqlText;
                    details.FieldType = line.FieldType;
                    dto.MarketShareLiveFilterCensusDetailsList.Add(details);
                }
                //get the compare filters
                dto.MarketShareLiveFilterCompareDetailsList = new List<LiveFilterDetailDto>();
                var ordererdtableComp = from o in reportdata.MarketShareFilterCompareDetails
                                       orderby o.Sequence
                                       select o;
                foreach (MarketShareFilterCompareDetail line in ordererdtableComp)
                {
                    var details = new LiveFilterDetailDto();
                    details.MarketShareLiveReportDataId = line.MarketShareReportDataId;
                    details.AndOr = line.AndOr;
                    details.CloseParenthesis = line.CloseParenthesis;
                    details.DataDictionaryId = line.DataDictionaryId;
                  

                    details.FieldValue = line.FieldValue;
                    details.FilterId = line.FilterId;
                    details.Id = line.Id;
                    details.OpenParenthesis = line.OpenParenthesis;
                    details.Operation = line.Operation;
                    details.Sequence = line.Sequence;
                    details.SqlText = line.SqlText;
                    details.FieldType = line.FieldType;
                    dto.MarketShareLiveFilterCompareDetailsList.Add(details);
                }
            }
            dto.DictionaryList = new List<DataDictionaryDto>();
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

                dto.DictionaryList.Add(details);
            }
            dto.DictionaryGeographicList = new List<DataDictionaryDto>();
            foreach (DataDictionaryGetFieldList_Result line in dataDictionaryGeographic)
            {
                var details = new DataDictionaryDto();
                details.CodedField = line.CodedField;
                details.CodedJoinField = line.CodedJoinField;
                details.CodedTable = line.CodedTable;
                details.EndDate = line.EndDate.ToString(); //.ToShortDateString();
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

                dto.DictionaryGeographicList.Add(details);
            }
            dto.DictionaryNonGeographicList = new List<DataDictionaryDto>();
            foreach (DataDictionaryGetFieldList_Result line in dataDictionaryNonGeographic)
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

                dto.DictionaryNonGeographicList.Add(details);
            }
            dto.DateList = new List<LiveDatesDto>();
            foreach (GetModuleDatesbyModuleUnit_Result line in reportdates)
            {
                var details = new LiveDatesDto();
                details.Id = (int)line.Id;
                details.YearMonth = line.YearMonth;
                details.Description = line.Description;
                dto.DateList.Add(details);
            }
            dto.GeograhicFilterList = new List<LiveFiltersDto>();
            foreach (FilterListbyUserIdModuleId_Result line in geographicFilters)
            {
                var details = new LiveFiltersDto();
                details.Id = line.id.ToString();                
                details.Description = line.description;
                dto.GeograhicFilterList.Add(details);
            }
            dto.CaseFilterList = new List<LiveFiltersDto>();
            foreach (FilterListbyUserIdModuleId_Result line in caseFilters)
            {
                var details = new LiveFiltersDto();
                details.Id = line.id.ToString();
                details.Description = line.description;
                dto.CaseFilterList.Add(details);
            }
            return dto;
        }
    }

    public class LiveFilterDetailDto
    {
        public System.Guid Id { get; set; }
        public System.Guid MarketShareLiveReportDataId { get; set; }
        public int FilterTypesId { get; set; }
        public System.Guid? FilterId { get; set; }
        public int? Sequence { get; set; }
        public string OpenParenthesis { get; set; }
        public string AndOr { get; set; }
        public System.Guid DataDictionaryId { get; set; }
        public string Operation { get; set; }
        public string FieldValue { get; set; }
        public string CloseParenthesis { get; set; }
        
        public string SqlText { get; set; }
        public string FieldType { get; set; }

    }
    public class LiveDatesDto
    {
        public int Id { get; set; }      
        public int YearMonth { get; set; }      
        public string Description { get; set; }
    }

    public class LiveFiltersDto
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }



}
