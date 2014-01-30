/*** Script  #1 ***/
    
$(document).ready(function () {     
    $(function () {

        //"Set default values for future Ajax requests. Its use is not recommended"
        //http://api.jquery.com/jquery.ajaxsetup/
        //cache: false will force requested pages not to be cached by the browser

        $.ajaxSetup({ cache: false });
    })
})

/*** Script #2 ***/
//       $(function () {
//           "use strict";
// Proxy created on the fly
            
//$.connection.hub.start({ transport: activeTransport }, function () {
//   // alert("start hub");
            
//    reportStatus.registerUser(reportViewModel.UserId());
//});
        
//function dothis(x){
//    $('#DatePeriods').val($('#DatePeriods').val().replace(/[^\d]/ig, '')) 
//}
function getQueryVariable(variable) {
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split("=");
        if (pair[0] == variable) {
            return unescape(pair[1]);
        }
    }
}

var activeTransport = getQueryVariable('transport') || 'auto';

$.connection.hub.start({ transport: activeTransport, waitForPageLoad: true })
    .done(function () {reportViewModel.IsConnected(true); reportStatus.registerUser(reportViewModel.UserId())})
    .fail(function () { reportViewModel.IsConnected(false); alert("Could not Connect!"); });
          

var reportStatus = $.connection.reportGenerationHub;

$("#runButton").click(function () {
                
    reportStatus.submitReport2(reportViewModel.UserId(), getURLParam("reportRunGenId"), reportViewModel.UnitId(), reportViewModel.UserId(),1);
});

reportStatus.addMessage = function (message) {
    //log(message);
    setmessage(message);
};

reportStatus.reloadGrid = function () {
    reloadGrid();
};


//      }); 

 
//    // Declare a function on the reportNotificationHub hub so the server can invoke it
//    reportnotification.addMessage = function (message) {
//        alert(message);
//       // $('#messages').append('<li>' + message + '</li>');
//    };

        
//    // Start the connection
//    $.connection.hub.start();
//});
    
$('#tabContainer').tabs();
$(document).ready(function () {
    
    // if (getURLParam("mode") == "live")
    // {
    // SetWizard(); 
    // setmessage("Request Validated");
    //var grid = $("#GridViewResults").data("tGrid");
    //grid.rebind();
     
    // }
    // MasterCheckBox functionality
    //Available grid
    $('#masterAvailablecheckbox').click(function () {
      
        if (this.checked) {
      
            $('.availableCheck').attr('checked', 'checked');
            for (var i in leftchecked) {   
                leftchecked[i]=true;   
            }
        } else {
      
            $('.availableCheck').removeAttr('checked');
            for (var i in leftchecked) {   
                leftchecked[i]=false;   
            }
        }
    });
    //selected grid
    $('#masterSelectedcheckbox').click(function () {
   
        if (this.checked) {
            $('.selectedCheck').attr('checked', 'checked');
            for (var i in rightchecked) {   
                rightchecked[i]=true;   
            }
            //        reportViewModel.SelectedRightValues(reportViewModel.SelectedValues());
 
        } else {
            $('.selectedCheck').removeAttr('checked');
            for (var i in rightchecked) {   
                rightchecked[i]=false;   
            }
        }
    });
   


});
  

//clear the checked object whenever a filter or ajax occurs
       
var DirtyDate = false;
        
var IncompleteReports = 0;
       

var leftchecked = {};
var rightchecked = {};
function onRowDataBoundReports(e) {
            
    if (e.dataItem.ReportStatus != "Completed") 
    {	
                
        $(e.row).addClass("rowerror");
        IncompleteReports = IncompleteReports + 1;
                
    }			
}
function onDataBoundReports(e) {
            
    reportViewModel.IncompleteReportCount(IncompleteReports)
            
}
function onDataBindingReports(e) {
    IncompleteReports = 0;
    //change to select all units
    //var unitId = reportViewModel.UnitId();
    var unitId = "";
    e.data = {
        selectedUnitId : unitId
               
    };
}
function onDataBindingLeft(e) {
    leftchecked = {};
    rightchecked = {};
    var dataDictionaryId = reportViewModel.DataDictionaryID();
    var values = reportViewModel.SelectedValues();
    var dictDate = reportViewModel.DictionaryYear();
    var side = "A";
    e.data = {
        id : dataDictionaryId,
        selectedValues: values,
        activeDate: dictDate,
        availableOrSelected: side
        
    };
}
function onLoadLeft() {
    $(this).delegate(":checkbox","click",function() {
        if (this.value != "on")
        {        
            leftchecked[this.value] = this.checked;
        }
        var comma = "";
        var selectedstring = "";
        reportViewModel.SelectedLeftValues("");
        var count = 0;
        var numChkBoxes = $('#AvailableTable input[type=checkbox]').length -1;
        for (var i in leftchecked) {  
            if (leftchecked[i])
            {
                selectedstring += comma+i;
                comma=","
                count ++;
            }
        }
        reportViewModel.SelectedLeftValues(selectedstring);
          
        if (numChkBoxes == count && numChkBoxes > 0) {
            $('#masterAvailablecheckbox').attr('checked','checked');
        }
        else {
            $('#masterAvailablecheckbox').removeAttr('checked');
        }
    });
    $(this).find(".t-filter").click(function () {
        setTimeout(function () {
            $(".t-filter-operator").each(function () {
                $(this).val("substringof");
            });
        });
    });
}
        
function onRowSelectedSingle(e) {
    var selectedvalue = e.row.cells[0].innerHTML;
    reportViewModel.SaveSinglePicker(selectedvalue);
    // reportViewModel.SelectedSingleValues(selectedstring);
          
}
function onDataBindingSingle(e) {
            
    var dataDictionaryId = reportViewModel.DataDictionaryID();
    var values = "";
    var dictDate = reportViewModel.DictionaryYear();
    var side = "Z";
    e.data = {
        id : dataDictionaryId,
        selectedValues: values,
        activeDate: dictDate,
        availableOrSelected: side
        
    };
}    

function onRowDataBoundLeft(e) {

    var dataItem = e.dataItem;
    
    leftchecked[dataItem.FieldValue] = false;
    


}
function onDataBindingRight(e) {
    leftchecked = {};
    rightchecked = {};
    var dataDictionaryId = reportViewModel.DataDictionaryID();
    var values = reportViewModel.SelectedValues();
    var dictDate = reportViewModel.DictionaryYear();
    var side = "S";
    e.data = {
        id : dataDictionaryId,
        selectedValues: values,
        activeDate: dictDate,
        availableOrSelected: side
        
    };
}
   
function onLoadRight() {
    $(this).delegate(":checkbox","click",function() {
        if (this.value != "on")
        {        
            rightchecked[this.value] = this.checked;
        }
        var comma = "";
        var selectedstring = "";
        reportViewModel.SelectedRightValues("");
        var count = 0;
        var numChkBoxes = $('#SelectedTable input[type=checkbox]').length -1;
        for (var i in rightchecked) {  
            if (rightchecked[i])
            {
                selectedstring += comma+i;
                comma=","
                count ++;
            }
        }
        reportViewModel.SelectedRightValues(selectedstring);
          
        if (numChkBoxes == count && numChkBoxes > 0) {
            $('#masterSelectedcheckbox').attr('checked','checked');
        }
        else {
            $('#masterSelectedcheckbox').removeAttr('checked')
        }
    });

    $(this).find(".t-filter").click(function () {
        setTimeout(function () {
            $(".t-filter-operator").each(function () {
                $(this).val("substringof");
            });
        });
    });
}
function onRowDataBoundRight(e) {
    var dataItem = e.dataItem;
    
    rightchecked[dataItem.FieldValue] = false;
    

}
//reset message on tabclick
function onTabClick() {
    reportViewModel.QueryMessage("");
}
  
//.function calLed by picker sorts
function sortByFieldValue(a, b) {
    var x = a.FieldValue.toLowerCase();
    var y = b.FieldValue.toLowerCase();
    return ((x < y) ? -1 : ((x > y) ? 1 : 0));
}

//function to only allow "("
function openparen(e) {
    var k;
    document.all ? k = e.keyCode : k = e.which;
    return (k == 40 );
}

//function to only allow ")"
function closeparen(e) {
    var k;
    document.all ? k = e.keyCode : k = e.which;
    return (k == 41 );
}

///function to see if a value is found in an array
function isInArray(arr, obj) { 
    for(var i=0; i<arr.length; i++) { 
        if (arr[i] == obj) return true; 
    } 
} 
function FindInArray(arr, obj) { 
    for(var i=0; i<arr.length; i++) { 
        if (arr[i].Id() == obj) return i; 
    } 
} 
    
//View Model data 
var Mha = Mha || {};
Mha.Report = function (model){
    return {
        //model fields
        IsConnected: ko.observable(false),
        Id: ko.observable(model.Id),
        ReportDefinitionsId: ko.observable(model.ReportDefinitionsId),           
        UserId: ko.observable(model.UserId),
        UnitId: ko.observable(model.UnitId),
        ReportTypesId: ko.observable(model.ReportTypesId),
        ReportGroupsId: ko.observable(model.ReportGroupsId),
        ModuleId: ko.observable(model.ModuleId),
        IsBaseReport: ko.observable(model.IsBaseReport),
        IsGlobalShare: ko.observable(model.IsGlobalShare),
        IsTemplateReport: ko.observable(model.IsTemplateReport),
        Description: ko.observable(model.Description),
        NewFilterName: ko.observable(model.NewFilterName),
        NewGeographicFilterName: ko.observable(model.NewGeographicFilterName),
        Summary: ko.observable(model.Summary),
        BaseYear: ko.observable(model.BaseYear),
        YearType: ko.observable(model.YearType),
        Trending: ko.observable(model.Trending),
        DatePeriodType: ko.observable(model.DatePeriodType),
        DatePeriods: ko.observable(model.DatePeriods),
        DateCalculationType: ko.observable(model.DateCalculationType),
        OutputType: ko.observable(model.OutputType),
        StartYearMonth: ko.observable(model.StartYearMonth),
        EndYearMonth: ko.observable(model.EndYearMonth),
        InitialEndYearMonth: ko.observable(model.EndYearMonth),
        DataType1: ko.observable(model.DataType1),
        DataType2: ko.observable(model.DataType2),
        DataType3: ko.observable(model.DataType3),
        MarketLevel: ko.observable(model.MarketLevel),
        Field1: ko.observable(model.Field1),
        Field1SortBy: ko.observable(model.Field1SortBy),
        Field1AscDesc: ko.observable(model.Field1AscDesc),
        Field1Group: ko.observable(model.Field1Group),
        Field1Records: ko.observable(model.Field1Records),
        Field1TopN: ko.observable(model.Field1TopN),
        Field1TopNPercent: ko.observable(model.Field1TopNPercent),
        Field2: ko.observable(model.Field2),
        Field2SortBy: ko.observable(model.Field2SortBy),
        Field2AscDesc: ko.observable(model.Field2AscDesc),
        Field2Group: ko.observable(model.Field2Group),
        Field2Records: ko.observable(model.Field2Records),
        Field2TopN: ko.observable(model.Field2TopN),
        Field2TopNPercent: ko.observable(model.Field2TopNPercent),
        Field3: ko.observable(model.Field3),
        Field3SortBy: ko.observable(model.Field3SortBy),
        Field3AscDesc: ko.observable(model.Field3AscDesc),           
        Field3Group: ko.observable(model.Field3Group),
        Field3Records: ko.observable(model.Field3Records),
        Field3TopN: ko.observable(model.Field3TopN),
        Field3TopNPercent: ko.observable(model.Field3TopNPercent),
        ShowDischargesHospital: ko.observable(model.ShowDischargesHospital),
        ShowDischargesState: ko.observable(model.ShowDischargesState),
        ShowDischargesPercent: ko.observable(model.ShowDischargesPercent),
        ShowLOSHospital: ko.observable(model.ShowLOSHospital),
        ShowLOSState: ko.observable(model.ShowLOSState),
        ShowLOSPercent: ko.observable(model.ShowLOSPercent),
        ShowHospitalPercent: ko.observable(model.ShowHospitalPercent),
        ShowLOSHospitalPercent: ko.observable(model.ShowLOSHospitalPercent),
        ShowField1: ko.observable(model.ShowField1),
        ShowField1Description: ko.observable(model.ShowField1Description),
        ShowField2: ko.observable(model.ShowField2),
        ShowField2Description: ko.observable(model.ShowField2Description),
        ShowField3: ko.observable(model.ShowField3),
        ShowField3Description: ko.observable(model.ShowField3Description),
        Column1Text: ko.observable("Hospital Discharges"),
        Column2Text: ko.observable("Total Discharges"),
        Column3Text: ko.observable("Market Share of Discharges"),
        Column4Text: ko.observable("Discharge Market Dependence"),
        Column5Text: ko.observable("Hospital LOS"),
        Column6Text: ko.observable("Total LOS"),
        Column7Text: ko.observable("Market Share of LOS"),
        Column8Text: ko.observable("LOS Market Dependence"),
        ReportDriver: ko.observable("MKTSHARE"),
        ShowSummary: ko.observable(model.ShowSummary),
        ShowFilter: ko.observable(model.ShowFilter),
        FilterIdGeographic: ko.observable(model.FilterIdGeographic),
        FilterIdCase: ko.observable(model.FilterIdCase),
        ActionMethod: ko.observable(model.ActionMethod),
        ShowAdvancedFilter: ko.observable(model.ShowAdvancedFilter),
        // ForceShowAdvancedFilter: ko.observable(true),
        // CaseAdvanced: ko.observable(false),
        // GeographicAdvanced: ko.observable(false),
        IsReportRun: ko.observable(false),
        ReportRunGenId: ko.observable(getURLParam("reportRunGenId")),
        RequestMessage: ko.observable(''),
        DateErrorMessage: ko.observable(''),
        InvalidDate: ko.observable(''),
        TrendOn: ko.observable('Off'),
        TrendOnPeriod: ko.observable('M'),
        TrendOnSort: ko.observable('ASC'),
        IncompleteReportCount:ko.observable(0),
        WizardTab:ko.observable('@ViewBag.TabIndex'),
        QueryMessage: ko.observable('@ViewBag.QueryMessage'),
        SummaryMessage: ko.observable('@ViewBag.SummaryMessage'),
        InvalidReport: ko.observable('@ViewBag.InvalidReport'),
        GeographicFilterMessage: ko.observable('@ViewBag.GeographicFilterMessage'),
        CaseFilterMessage: ko.observable('@ViewBag.CaseFilterMessage'),
        IsHost:ko.observable('@ViewBag.IsHost' == 'True'),
        //Save method
        Save: function($form) {
             
            reportViewModel.GetFieldSorts();
            ko.utils.postJson($form.attr('action'),{model: ko.toJS(this)});
        },
        
        //Build Lists 
        SortList: ko.observableArray([{"Name":"Field Description","Value":"DESCRIPTION"},{"Name":"Field Value","Value":"FIELDVALUE"}
        ,{"Name":"Hospital Discharges","Value":"FACILDISCH"},{"Name":"Comparison Discharges","Value":"STATEDISCH"}
        ,{"Name":"Percent Discharges","Value":"DISCHMKTSHARE"},{"Name":"Hospital LOS","Value":"FACILLOS"},
        {"Name":"Comparison LOS","Value":"STATELOS"},{"Name":"Percent LOS","Value":"LOSMKTSHARE"}]),       
        AscDescList: ko.observableArray([{"Name":"Desc.","Value":"DESC"},{"Name":"Asc.","Value":"ASC"}]),  
        RecordsList: ko.observableArray([{"Name":"All Records","Value":"All"},{"Name":"Top 5","Value":"Top 5"},{"Name":"Top 10","Value":"Top 10"}
        ,{"Name":"Top 20","Value":"Top 20"},{"Name":"Top 30","Value":"Top 30"}
         ,{"Name":"Top 40","Value":"Top 40"},{"Name":"Top 50","Value":"Top 50"}
          ,{"Name":"Top 60","Value":"Top 60"},{"Name":"Top 70","Value":"Top 70"}
           ,{"Name":"Top 80","Value":"Top 80"},{"Name":"Top 90","Value":"Top 90"} ,{"Name":"Top 100","Value":"Top 100"}
        ]),  
        DictionaryList: ko.observableArray(ko.utils.arrayMap(model.DictionaryList,function (dictline){return new Mha.DictionaryLine(dictline);})|| []),
        DictionaryNonGeographicList: ko.observableArray(ko.utils.arrayMap(model.DictionaryNonGeographicList,function (dictline3){return new Mha.DictionaryLine(dictline3);})|| []),
        DictionaryGeographicList: ko.observableArray(ko.utils.arrayMap(model.DictionaryGeographicList,function (dictline2){return new Mha.DictionaryLine(dictline2);})|| []),
        DateList: ko.observableArray(ko.utils.arrayMap(model.DateList,function (dline){return new Mha.DateLine(dline);})|| []),
        GeographicFilterList: ko.observableArray(ko.utils.arrayMap(model.GeograhicFilterList,function (line){return new Mha.FilterListLine(line);})|| []),
        CaseFilterList: ko.observableArray(ko.utils.arrayMap(model.CaseFilterList,function (line){return new Mha.FilterListLine(line);})|| []),
        LevelList: ko.observableArray([{"Name":"One Field","Value":"1"},{"Name":"Two Fields","Value":"2"}]),
        TrendingList: ko.observableArray([{"Name":"No Trending","Value":"0"},{"Name":"Annual Trending - from newest to oldest dates","Value":"YDESC"}
        ,{"Name":"Annual Trending - from oldest to newest dates","Value":"YASC"}
        ,{"Name":"Quartly Trending - from newest to oldest dates","Value":"QDESC"}
        ,{"Name":"Quarterly Trending - from oldest to newest dates","Value":"QASC"}
        ,{"Name":"Monthly Trending - from newest to oldest dates","Value":"MDESC"}
        ,{"Name":"Monthly Trending - from oldest to newest dates","Value":"MASC"}]),
        Trend: ko.observableArray([{"Name":"No Trending","Value":"Off"},{"Name":"Trending","Value":"On"}]),
        TrendPeriod: ko.observableArray([{"Name":"Monthly","Value":"M"},{"Name":"Calendar Quarter","Value":"Q"},{"Name":"Calendar Year","Value":"Y"}]),
        TrendSort: ko.observableArray([{"Name":"Ascending - from oldest to newest dates","Value":"ASC"},{"Name":"Descending - from newest to oldest dates","Value":"DESC"}]),
        PeriodTypeList: ko.observableArray([{"Name":"Month(s)","Value":"M"},{"Name":"Quarter(s)","Value":"Q"},{"Name":"Year(s)","Value":"A"},{"Name":"Year(s) + Year to Date","Value":"Y"}]),  
        PatientTypeList: ko.observableArray([{"Name":"Inpatient","Value":"I"},{"Name":"Outpatient","Value":"O"},{"Name":"Expanded Outpatient","Value":"EO"}]),  
        DateTypeList: ko.observableArray([{"Name":"Enter Dates","Value":"E"},{"Name":"Last x periods","Value":"C"}]), 
        OutputTypeList: ko.observableArray([{"Name":"Excel","Value":"Excel"},{"Name":"Excel - Unformatted","Value":"ExcelNoFormat"},{"Name":"Excel - with Graph","Value":"ExcelGraph"},{"Name":"Excel 2003 - with Graph","Value":"ExcelGraph2003"}]),  
        //filteringExcel 2007 plus with Graph 
        MarketShareLiveFilterGeographicDetailsList: ko.observableArray(ko.utils.arrayMap(model.MarketShareLiveFilterGeographicDetailsList,function (filterline){return new Mha.FilterLine(filterline);})|| []), 
        addLineGeographic: function() { this.MarketShareLiveFilterGeographicDetailsList.push(new  Mha.newfilterLine(this.Id,this.Id,'AND','(',null,null,null,')','0',99,'','')) },
        removeLineGeographic:function(line) { this.MarketShareLiveFilterGeographicDetailsList.remove(line) },
        moveLineUpGeographic: function (line) { var position = ko.utils.arrayIndexOf(this.MarketShareLiveFilterGeographicDetailsList, line); if (position > 0) { this.MarketShareLiveFilterGeographicDetailsList.remove(line); this.MarketShareLiveFilterGeographicDetailsList.splice(position - 1, 0, line); } },
        moveLineDownGeographic:function(line) { var position = ko.utils.arrayIndexOf(this.MarketShareLiveFilterGeographicDetailsList,line);this.MarketShareLiveFilterGeographicDetailsList.remove(line);this.MarketShareLiveFilterGeographicDetailsList.splice(position+1, 0, line); },
        MarketShareLiveFilterDetailsList: ko.observableArray(ko.utils.arrayMap(model.MarketShareLiveFilterDetailsList,function (filterline){return new Mha.FilterLine(filterline);})|| []), 
        AndOrList: ko.observableArray([{"Name":"AND"},{"Name":"OR"}]),
        OperatorList: ko.observableArray([{"Name":"Equal to","Value":"="},{"Name":"Not Equal to","Value":"!="},{"Name":"Greater than","Value":">"},{"Name":"Greater than or Equal to","Value":">="},{"Name":"Less than","Value":"<"},{"Name":"Less than or Equal to","Value":"<="}]),
        addLine: function() { this.MarketShareLiveFilterDetailsList.push(new  Mha.newfilterLine(this.Id,this.Id,'AND','(',null,null,null,')','0',99,'','')) },
        removeLine:function(line) { this.MarketShareLiveFilterDetailsList.remove(line) },
        moveLineUp: function (line) { var position = ko.utils.arrayIndexOf(this.MarketShareLiveFilterDetailsList, line); if (position > 0) { this.MarketShareLiveFilterDetailsList.remove(line); this.MarketShareLiveFilterDetailsList.splice(position - 1, 0, line); }},
        moveLineDown:function(line) { var position = ko.utils.arrayIndexOf(this.MarketShareLiveFilterDetailsList,line);this.MarketShareLiveFilterDetailsList.remove(line);this.MarketShareLiveFilterDetailsList.splice(position+1, 0, line); },
        MarketShareLiveFilterCensusDetailsList: ko.observableArray(ko.utils.arrayMap(model.MarketShareLiveFilterCensusDetailsList,function (filterline){return new Mha.FilterLine(filterline);})|| []), 
        addLineCensus: function() { this.MarketShareLiveFilterCensusDetailsList.push(new  Mha.newfilterLine(this.Id,this.Id,'AND','(',null,null,null,')','0',99,'','')) },
        removeLineCensus:function(line) { this.MarketShareLiveFilterCensusDetailsList.remove(line) },
        moveLineUpCensus: function (line) { var position = ko.utils.arrayIndexOf(this.MarketShareLiveFilterCensusDetailsList, line); if (position > 0) { this.MarketShareLiveFilterCensusDetailsList.remove(line); this.MarketShareLiveFilterCensusDetailsList.splice(position - 1, 0, line); } },
        moveLineDownCensus:function(line) { var position = ko.utils.arrayIndexOf(this.MarketShareLiveFilterCensusDetailsList,line);this.MarketShareLiveFilterCensusDetailsList.remove(line);this.MarketShareLiveFilterCensusDetailsList.splice(position+1, 0, line); },
        MarketShareLiveFilterCompareDetailsList: ko.observableArray(ko.utils.arrayMap(model.MarketShareLiveFilterCompareDetailsList,function (filterline){return new Mha.FilterLine(filterline);})|| []), 
        addLineCompare: function() { this.MarketShareLiveFilterCompareDetailsList.push(new  Mha.newfilterLine(this.Id,this.Id,'AND','(',null,null,null,')','0',99,'','')) },
        removeLineCompare:function(line) { this.MarketShareLiveFilterCompareDetailsList.remove(line) },
        moveLineUpCompare: function (line) { var position = ko.utils.arrayIndexOf(this.MarketShareLiveFilterCompareDetailsList, line); if (position > 0) { this.MarketShareLiveFilterCompareDetailsList.remove(line); this.MarketShareLiveFilterCompareDetailsList.splice(position - 1, 0, line); } },
        moveLineDownCompare:function(line) { var position = ko.utils.arrayIndexOf(this.MarketShareLiveFilterCompareDetailsList,line);this.MarketShareLiveFilterCompareDetailsList.remove(line);this.MarketShareLiveFilterCompareDetailsList.splice(position+1, 0, line); },
        FieldLabel: ko.observable(""),
        DictionaryYear: ko.observable("12/31/2011"),
        SelectColumnsVisible: ko.observable(false),
        SelectColumns: function() {
            if (this.SelectColumnsVisible() == true) {                  
                this.SelectColumnsVisible(false);
            }
            else {
           
                this.SelectColumnsVisible(true);
             
                $("#divSelectColumns").dialog({ height: 200,width: 1000,show: 'fadein',hide:'fadeout',modal: true,title: 'Select Visible Columns' ,position:['center']});
  

            }
        },
        PickerVisible: ko.observable(false),
        PickerSingleVisible: ko.observable(false),
        SaveReportVisible: ko.observable(false),
        DataDictionaryID: ko.observable("1cb79aad-1409-45c6-b3d8-2c46c5aa1d8f"),
        SelectedSingleValues: ko.observable(""),
        SelectedValues: ko.observable(""),
        SelectedLeftValues: ko.observable(""),
        SelectedRightValues: ko.observable(""),

        SQLStatement: ko.observable(""),
        ShowOn: ko.observable(false),

        PickerData: ko.observableArray([]),

        addRequested: function(){
            var SelectedString = this.SelectedValues();
                
            var comma = ",";
            if (SelectedString.length < 1)
            {
                comma = "";
            }
            for (var i in leftchecked) {  
                if (leftchecked[i])
                {
             
                    SelectedString += comma + i;
                    comma = ",";
                }
            }

            this.SelectedValues(SelectedString); 
           
            rightchecked = {};
            leftchecked = {};
            reportViewModel.SelectedRightValues("");
            reportViewModel.SelectedLeftValues("");
            $('#masterAvailablecheckbox').removeAttr('checked');
            $('#masterSelectedcheckbox').removeAttr('checked');
            var grid = $("#AvailableGrid").data("tGrid");
            grid.rebind();
            grid = $("#SelectedGrid").data("tGrid");
            grid.rebind();
                           
        }, 
                       
        removeRequested: function(){
            var SelectedString = this.SelectedValues();
            for (var i in rightchecked) {  
                if (rightchecked[i])
                {
             
                    SelectedString =  SelectedString.replace(i+",","");
                    SelectedString =  SelectedString.replace(","+i,"");
                    SelectedString =  SelectedString.replace(i,"");
                }
            }

            this.SelectedValues(SelectedString); 
            rightchecked = {};
            leftchecked = {};
            reportViewModel.SelectedRightValues("");
            reportViewModel.SelectedLeftValues("");
            $('#masterAvailablecheckbox').removeAttr('checked');
            $('#masterSelectedcheckbox').removeAttr('checked');
            var grid = $("#AvailableGrid").data("tGrid");
            grid.rebind();
            grid = $("#SelectedGrid").data("tGrid");
            grid.rebind();
                            

        },

        showSQL: function() {
            if (this.ShowOn() == true)
            {this.ShowOn(false);}
            else{
                //BuildSQL();
                this.ShowOn(true);
            }
        },
        CloseSaveReport: function() {          
            this.SaveReportVisible(false);
            $("#divSaveReport").dialog("close");              
        },
        OpenSaveReport: function() {
            if (this.SaveReportVisible() == true) {                  
                this.SaveReportVisible(false);
            }
            else {
             
                this.SaveReportVisible(true);
             
                $("#divSaveReport").dialog({ height: 140,width: 450,show: 'fadein',hide:'fadeout',modal: true,title: 'Save Report' ,position:['center']});
  

            }
        },
           
        ClosePicker: function() {          
            this.PickerVisible(false);
            $("#divPicker").dialog("close");              
        },
           
        SavePicker: function() {
            var dictionaryid = this.PickerData.DataDictionaryId();         
                    
            Mha.ReFormatLookups(dictionaryid,this.SelectedValues(), this.DictionaryYear());
            this.PickerVisible(false);
            $("#divPicker").dialog("close");        
        },
        CloseSinglePicker: function() {          
            this.PickerSingleVisible(false);
            $("#divSinglePicker").dialog("close");              
        },
           
        SaveSinglePicker: function(selval) {
            //var dictionaryid = this.PickerData.DataDictionaryId();         
            reportViewModel.PickerData.FieldValue(selval);
            // Mha.SetSinglePickerValue(this.SelectedValues());
            this.PickerSingleVisible(false);
            $("#divSinglePicker").dialog("close");        
        },
        ViewReport: function(){
           
            Mha.ViewReport(this.ReportRunGenId());
        },
           
        SetPicker: function(line) {
            if (this.PickerVisible() == true || this.PickerSingleVisible() == true) {                  
                this.PickerVisible(false);
                this.PickerSingleVisible(false);
            }
            else {
                var dictionaryid = line.DataDictionaryId(); 
                //this should work for both filter types since it has all recs in it        
                var index = FindInArray(reportViewModel.DictionaryList(),dictionaryid);
                this.FieldLabel(reportViewModel.DictionaryList()[index].FieldLabel());      
                //this.PickerRequested([]);        
                var enteredValue = line.FieldValue();
                var operation = line.Operation();
                reportViewModel.DataDictionaryID(dictionaryid);
                        

                            
                if (operation == "=" || operation == "!=")
                {
                    //pausecomp(1250);
                    Mha.DeFormatLookups(dictionaryid,enteredValue, this.DictionaryYear(),operation);

             

                    this.PickerData = line;
                    this.PickerVisible(true);
             
                    $("#divPicker").dialog({ height: 640,width: 1150,show: 'fadein',hide:'fadeout',modal: true,title: 'Select Filter' ,position:['center']});
                }
                else
                {
                    Mha.SingleLookups(dictionaryid,enteredValue, this.DictionaryYear(),operation);
                    reportViewModel.SelectedSingleValues(enteredValue);
                    this.PickerData = line;
                    this.PickerSingleVisible(true);
             
                    $("#divSinglePicker").dialog({ height: 640,width: 1150,show: 'fadein',hide:'fadeout',modal: true,title: 'Select Filter' ,position:['center']});
                }

            }
        },
                    
        //FUNCTIONS
                
        GetFieldSorts: function(){
            var field1Sorts = getColumnSorts("Field1");
            if (field1Sorts.Index != -1)
            {
                if (field1Sorts.Index == 2){
                    reportViewModel.Field1SortBy("FIELD1DESCRIPTION");}
                if (field1Sorts.Index == 1){
                    reportViewModel.Field1SortBy("FIELD1");}
                if (field1Sorts.Index == 3){
                    reportViewModel.Field1SortBy("FACILDISCH");}
                if (field1Sorts.Index == 4){
                    reportViewModel.Field1SortBy("STATEDISCH");}
                if (field1Sorts.Index == 5){
                    reportViewModel.Field1SortBy("DISCHMKTSHARE");
                }
                if (field1Sorts.Index == 6) {
                    reportViewModel.Field1SortBy("DISCHHOSPSHARE");
                }
                if (field1Sorts.Index == 7){
                    reportViewModel.Field1SortBy("FACILLOS");}
                if (field1Sorts.Index == 8){
                    reportViewModel.Field1SortBy("STATELOS");}
                if (field1Sorts.Index == 9){
                    reportViewModel.Field1SortBy("LOSMKTSHARE");
                }
                if (field1Sorts.Index == 10) {
                    reportViewModel.Field1SortBy("LOSHOSPSHARE");
                }
                reportViewModel.Field1AscDesc(field1Sorts.AscDesc)                
            }
            else{
                //set a default sort
                reportViewModel.Field1SortBy("FIELD1");
                reportViewModel.Field1AscDesc("DESC");
            }
             
            var field2Sorts = getColumnSorts("Field2");
            if (field2Sorts.Index != -1)
            {
                if (field2Sorts.Index == 2){
                    reportViewModel.Field2SortBy("FIELD2DESCRIPTION");}
                if (field2Sorts.Index == 1){
                    reportViewModel.Field2SortBy("FIELD2");}
                if (field2Sorts.Index == 3){
                    reportViewModel.Field2SortBy("FACILDISCH");}
                if (field2Sorts.Index == 4){
                    reportViewModel.Field2SortBy("STATEDISCH");}
                if (field2Sorts.Index == 5){
                    reportViewModel.Field2SortBy("DISCHMKTSHARE");
                }
                if (field2Sorts.Index == 6) {
                    reportViewModel.Field2SortBy("DISCHHOSPSHARE");
                }
                if (field2Sorts.Index == 7){
                    reportViewModel.Field2SortBy("FACILLOS");}
                if (field2Sorts.Index == 8){
                    reportViewModel.Field2SortBy("STATELOS");}
                if (field2Sorts.Index == 9){
                    reportViewModel.Field2SortBy("LOSMKTSHARE");
                }
                if (field2Sorts.Index == 10) {
                    reportViewModel.Field2SortBy("LOSHOSPSHARE");
                }
                reportViewModel.Field2AscDesc(field2Sorts.AscDesc)                
            }
            else{
                //set a default sort
                reportViewModel.Field2SortBy("FIELD2");
                reportViewModel.Field2AscDesc("DESC");
            }
              
            var field3Sorts = getColumnSorts("Field3");
            if (field3Sorts.Index != -1)
            {
                if (field3Sorts.Index == 2){
                    reportViewModel.Field3SortBy("FIELD3DESCRIPTION");}
                if (field3Sorts.Index == 1){
                    reportViewModel.Field3SortBy("FIELD3");}
                if (field3Sorts.Index == 3){
                    reportViewModel.Field3SortBy("FACILDISCH");}
                if (field3Sorts.Index == 4){
                    reportViewModel.Field3SortBy("STATEDISCH");}
                if (field3Sorts.Index == 5){
                    reportViewModel.Field3SortBy("DISCHMKTSHARE");
                }
                if (field3Sorts.Index == 6) {
                    reportViewModel.Field3SortBy("DISCHHOSPSHARE");
                }
                if (field3Sorts.Index == 7){
                    reportViewModel.Field3SortBy("FACILLOS");}
                if (field3Sorts.Index == 8){
                    reportViewModel.Field3SortBy("STATELOS");}
                if (field3Sorts.Index == 9){
                    reportViewModel.Field3SortBy("LOSMKTSHARE");
                }
                if (field3Sorts.Index == 10) {
                    reportViewModel.Field3SortBy("LOSHOSPSHARE");
                }
                reportViewModel.Field3AscDesc(field3Sorts.AscDesc)                
            }
            else{
                //set a default sort
                reportViewModel.Field3SortBy("FIELD3");
                reportViewModel.Field3AscDesc("DESC");
            }
             
        }
    }
};
           
//outside viewmodel
         
Mha.FilterLine = function(model){
    // this.MarketShareLiveReportDataId(model.MarketShareReportDataId);
    this.AndOr= ko.observable(model.AndOr);
    this.CloseParenthesis= ko.observable(model.CloseParenthesis);
    this.DataDictionaryId= ko.observable(model.DataDictionaryId);
    this.DateField= ko.observable(model.DateField);
    this.FieldValue= ko.observable(model.FieldValue);
    this.FilterId= ko.observable(model.FilterId);
    this.Id= ko.observable(model.Id);
    this.OpenParenthesis= ko.observable(model.OpenParenthesis);
    this.Operation= ko.observable(model.Operation);
    this.FieldType= ko.observable(model.FieldType);
    this.Sequence= ko.observable(model.Sequence);
    this.SQLText= ko.observable(model.SQLText);
    this.DataDictionaryId.subscribe(function () {
        this.FieldValue("");
                
        var dictionaryid = this.DataDictionaryId();
        //this should work for both filter types since it has all recs in it        
        var index = FindInArray(reportViewModel.DictionaryList(), dictionaryid);
                
        this.FieldType(reportViewModel.DictionaryList()[index].FieldType());
                
    }.bind(this));
          
};

//Define the structure of the new filter line with abiltity to send in defaults
Mha.newfilterLine = function(pFilterId,pId,pAndOr,pOpenParenthesis,pDataDictionaryId,pOperation,pFieldValue,pCloseParenthesis,pDateField,pSequence,pSQLText,pFieldType) {
                
    this.AndOr = ko.observable(pAndOr);
    this.CloseParenthesis = ko.observable(pCloseParenthesis);
    this.DataDictionaryId = ko.observable(pDataDictionaryId);
    this.DateField = ko.observable(pDateField);
    this.FieldValue = ko.observable(pFieldValue);
    this.FilterId = ko.observable(pFilterId);
    this.Id = ko.observable(pId);
    this.OpenParenthesis = ko.observable(pOpenParenthesis);
    this.Operation = ko.observable(pOperation);
    this.FieldType = ko.observable(pFieldType);
    this.Sequence = ko.observable(pSequence);
    this.SQLText = ko.observable(pSQLText);
    this.DataDictionaryId.subscribe(function () {
        this.FieldValue("");
                
        var dictionaryid = this.DataDictionaryId();
        //this should work for both filter types since it has all recs in it        
        var index = FindInArray(reportViewModel.DictionaryList(), dictionaryid);
                
        this.FieldType(reportViewModel.DictionaryList()[index].FieldType());

    }.bind(this));
}
       

Mha.PickerItem = function(model){
    return {
        FieldName: ko.observable(model.FieldName),
        FieldValue: ko.observable(model.FieldValue)
    }
}
//refresh the filters
// Mha.RunReport = function () {
//     setmessage(reportViewModel.Description()+" - Report was Submitted");
//     reportViewModel.QueryMessage("");
//     $.ajax({


//         url: '@Url.Action("Run", "Report")',
//         data: { id: getURLParam("reportRunGenId"), unitId: reportViewModel.UnitId, mode: getURLParam("mode") },
//         type: 'POST',
//         success: function (data) {
//             alert(data);
        
//              reportViewModel.IsReportRun(true);
        
                      
//        },
//        error: function () { setmessage("Request Failed"); }
//     });
        

// };
        
//takes the simple string and calls the server to return a full comma delimted list
Mha.DeFormatLookups = function(dictionaryId,fieldvalue,dictionaryYear,operation)
{
        
    $.ajax({
                    
        url: '@Url.Action("DeformatLookup", "Filter")',
        data: { id: dictionaryId, selectedValues: fieldvalue, activeDate: dictionaryYear, operation: operation },
        type: 'GET',
        success: function (data) {
							
                            
            fieldvalue = data;
            rightchecked = {};
            leftchecked = {};
            reportViewModel.SelectedValues(fieldvalue);

            var grid = $("#AvailableGrid").data("tGrid");
            grid.rebind();
            grid = $("#SelectedGrid").data("tGrid");
            grid.rebind();
            // in the error function.. temp removed alert('failure');
                            
        },   error: function(){        }
    });
                     
};
//takes the simple string and calls the server to return a full comma delimted list
Mha.SingleLookups = function(dictionaryId,fieldvalue,dictionaryYear,operation)
{
        
    $.ajax({
                    
        url: '@Url.Action("DeformatLookup", "Filter")',
        data: { id: dictionaryId, selectedValues: fieldvalue, activeDate: dictionaryYear, operation: operation },
        type: 'GET',
        success: function (data) {
							
                            
            fieldvalue = data;
                    
            reportViewModel.SelectedSingleValues(fieldvalue);

            var grid = $("#SinglePickerGrid").data("tGrid");
            grid.rebind();
                    
            // in the error function.. temp removed alert('failure');
                            
        },   error: function(){        }
    });
                     
};
Mha.PostEdit = function()
{
         
         
    var url = '@Url.Action("Edit", "Report", new { reporttoRunId = "Id1" })';
    $.post(url);
               
           
					
};
Mha.ViewReport = function(Id1)
{
         
    if (Id1 != null)
    {
          
        var url = '@Url.Action("View", "Report", new { reporttoRunId = "Id1" })';
        var newurl = url.replace("Id1",Id1);
        //  alert(newurl);
        window,open(newurl);
    }
					
};
//takes the comma delimted list and calls the server to reformat it back to simple
Mha.ReFormatLookups = function(dictionaryId,fieldvalue,dictionaryYear)
{
      
    $.ajax({
        url: '@Url.Action("ReformatLookup", "Filter")',
        data: {id: dictionaryId, selectedValues: fieldvalue, activeDate: dictionaryYear },
                        
        type: 'POST',
        success: function (data) {
            if (data == "All")
            {
                data = "";
                alert('Selecting all possible values in a filter is invalid.');
            }		
            reportViewModel.PickerData.FieldValue(data);
            // in the error function.. temp removed alert('failure');
        },   error: function(){       }

    });
};
Mha.SetSinglePickerValue = function(fieldvalue)
{
  						
    reportViewModel.PickerData.FieldValue(fieldvalue);
           
};

          
//object class
Mha.DictionaryLine = function(model){
    return {
        CodedField: ko.observable(model.CodedField),
        CodedJoinField: ko.observable(model.CodedJoinField),
        CodedTable: ko.observable(model.CodedTable),
        EndDate: ko.observable(model.EndDate),
        FieldLabel: ko.observable(model.FieldLabel),
        FieldName: ko.observable(model.FieldName),
        FieldType: ko.observable(model.FieldType),
        Id: ko.observable(model.Id),
        IsCoded: ko.observable(model.IsCoded),
        IsGeographic: ko.observable(model.IsGeographic),
        IsLookup: ko.observable(model.IsLookup),
        IsUdf: ko.observable(model.IsUdf),
        LookupName: ko.observable(model.LookupName),
        PickListSort: ko.observable(model.PickListSort),
        StartDate: ko.observable(model.StartDate),
        UdfId: ko.observable(model.UdfId),
        UdfTable: ko.observable(model.UdfTable)
    }}
//object class
Mha.DateLine = function(model){
    return {
               
        Id: ko.observable(model.Id),
        YearMonth: ko.observable(model.YearMonth),              
        Description: ko.observable(model.Description)
    }}
Mha.FilterListLine = function(model){
          
    return {
        Id: ko.observable(model.Id),
        Description: ko.observable(model.Description)
    }
}
          

// 
//addLevel:function() {this.MarketLevel += 1;} 
          
var initialData = @Html.Raw(new JavaScriptSerializer().Serialize(Model));

var reportViewModel = new Mha.Report(initialData);
//set trending value
//           reportViewModel.TrendSetter = ko.dependentObservable(function() {

//           var TrendingValue = "0";
//           if (reportViewModel.TrendOn = "On")
//           {
//            TrendingValue = reportViewModel.TrendOnPeriod()+reportViewModel.TrendOnSort();
//           }
//           reportViewModel.Trending(TrendingValue);
//           
//           }
//           );

//defaults when type changes
function SetReportTypeDefaults() {

    reportViewModel.ShowDischargesHospital(true);
    reportViewModel.ShowDischargesState(true);
    reportViewModel.ShowDischargesPercent(true);
    reportViewModel.ShowLOSHospital(true);
    reportViewModel.ShowLOSState(true);
    reportViewModel.ShowLOSPercent(true);
    reportViewModel.ShowLOSHospitalPercent(true);
    reportViewModel.ShowHospitalPercent(true);
    reportViewModel.ShowField1(true);
    reportViewModel.ShowField1Description(true);
    reportViewModel.ShowField2(true);
    reportViewModel.ShowField2Description(true);
    reportViewModel.ShowField3(true);
    reportViewModel.ShowField3Description(true);

};
//set the report designer descriptions

reportViewModel.SetDesignerDescriptions = ko.dependentObservable(function () {
            
    var hospspec = false;
            
    //check for fields using hospital

    if (reportViewModel.MarketLevel() > 0) {
        if (reportViewModel.Field1() == "da13301b-3299-465b-908a-95604b0f9e65") {
            hospspec = true;
        }
        var index = FindInArray(reportViewModel.DictionaryList(),reportViewModel.Field1());
        if (reportViewModel.DictionaryList()[index].FieldType() == "H") {
            hospspec = true;
        }
    }
            
    if (reportViewModel.MarketLevel() > 1) {
        if (reportViewModel.Field2() == "da13301b-3299-465b-908a-95604b0f9e65") {
            hospspec = true;
        }
        var index = FindInArray(reportViewModel.DictionaryList(), reportViewModel.Field2());
        if (reportViewModel.DictionaryList()[index].FieldType() == "H") {
            hospspec = true;
        }
    }
    if (reportViewModel.MarketLevel() > 2) {
        if (reportViewModel.Field3() == "da13301b-3299-465b-908a-95604b0f9e65") {
            hospspec = true;
        }
        var index = FindInArray(reportViewModel.DictionaryList(), reportViewModel.Field3());
        if (reportViewModel.DictionaryList()[index].FieldType() == "H") {
            hospspec = true;
        }
    }
    //override the hospital specific code for all trending and any > two level 
    if (reportViewModel.Trending() != '0' || reportViewModel.MarketLevel() > 2)
    {
        hospspec = false;
    }

    //look at the report type
           
            
    switch (reportViewModel.ReportTypesId()) {
        case 'MKTSHARE':
            reportViewModel.Column1Text("Hospital Discharges");
            reportViewModel.Column2Text("Total Discharges");
            reportViewModel.Column3Text("Market Share of Discharges");
            reportViewModel.Column4Text("Discharge Market Dependence");
            reportViewModel.Column5Text("Hospital LOS");
            reportViewModel.Column6Text("Total LOS");
            reportViewModel.Column7Text("Market Share of LOS");
            reportViewModel.Column8Text("LOS Market Dependence");
            if (hospspec == true) {
                reportViewModel.ReportDriver("HOSPMKTSHARE");
                       
            }
            else {
                reportViewModel.ReportDriver("MKTSHARE");
                        
            }
            break;
        case 'MKTTREND':
            reportViewModel.Column1Text("Hospital Discharges");
            reportViewModel.Column2Text("Total Discharges");
            reportViewModel.Column3Text("Market Share of Discharges");
            reportViewModel.Column4Text("Discharge Market Dependence");
            reportViewModel.Column5Text("Hospital LOS");
            reportViewModel.Column6Text("Total LOS");
            reportViewModel.Column7Text("Market Share of LOS");
            reportViewModel.Column8Text("LOS Market Dependence");
            if (hospspec == true) {
                reportViewModel.ReportDriver("HOSPMKTSHARETREND");
            }
            else {
                reportViewModel.ReportDriver("MKTSHARETREND");
            }
            break;
        case 'MKTREPLICATION':
            reportViewModel.Column1Text("Replicated Total");
            reportViewModel.Column2Text("Non-Replicated Total");
            reportViewModel.Column3Text("");
            reportViewModel.Column4Text("");
            reportViewModel.Column5Text("");
            reportViewModel.Column6Text("Total");
            reportViewModel.Column7Text("Percent Replicated");
            reportViewModel.Column8Text("");
            reportViewModel.ReportDriver("MKTREPLICATION");
            break;
        case 'MKTREPLICATIONTREND':
            reportViewModel.Column1Text("Replicated Total");
            reportViewModel.Column2Text("Non-Replicated Total");
            reportViewModel.Column3Text("");
            reportViewModel.Column4Text("");
            reportViewModel.Column5Text("");
            reportViewModel.Column6Text("Total");
            reportViewModel.Column7Text("Percent Replicated");
            reportViewModel.Column8Text("");
                    
            reportViewModel.ReportDriver("MKTREPLICATIONTREND");
            break;
                

        default:
                   
            reportViewModel.ReportDriver("MKTSHARE");
            reportViewModel.Column1Text("Hospital Discharges");
            reportViewModel.Column2Text("Total Discharges");
            reportViewModel.Column3Text("Market Share of Discharges");
            reportViewModel.Column4Text("Discharge Market Dependence");
            reportViewModel.Column5Text("Hospital LOS");
            reportViewModel.Column6Text("Total LOS");
            reportViewModel.Column7Text("Market Share of LOS");
            reportViewModel.Column8Text("LOS Market Dependence");
            if (hospspec == true) {
                reportViewModel.ReportDriver("HOSPMKTSHARE");

            }
            else {
                reportViewModel.ReportDriver("MKTSHARE");

            }
                    
    }
            
            
}, reportViewModel);

        

//get trend descripition
reportViewModel.TrendDescription = ko.dependentObservable(function() {
           
    var TrendDesc;
    var yearType = reportViewModel.YearType() == 'F' ? ' by Fiscal Year' : ' by Calendar Year';

    switch (reportViewModel.Trending())
    {
        case 'YASC': TrendDesc = 'Annual Trending from oldest to newest dates' +yearType+' (Sorted by Oldest Period)';
            break;
        case 'YDESC': TrendDesc = 'Annual Trending from newest to oldest dates' +yearType+' (Sorted by Newest Period)';
            break;
        case 'QASC': TrendDesc = 'Quarterly Trending from oldest to newest dates (Sorted by Oldest Period)';
            break;
        case 'QDESC': TrendDesc = 'Quarterly Trending from newest to oldest dates (Sorted by Newest Period)';
            break;
        case 'MASC': TrendDesc = 'Monthly Trending from oldest to newest dates (Sorted by Oldest Period)';
            break;
        case 'MDESC': TrendDesc = 'Monthly Trending from newest to oldest dates (Sorted by Newest Period)';
            break;
            
        default: TrendDesc = '';
    }
    return TrendDesc;
});
//try to see if I can trigger that the end date changed without the infinite loop
reportViewModel.EndChanged = ko.dependentObservable(function() {
    if (reportViewModel.EndYearMonth() != reportViewModel.InitialEndYearMonth())
    {
        //trigger the change
        reportViewModel.InitialEndYearMonth(reportViewModel.EndYearMonth());
    }
});
//calculate start date from choices
reportViewModel.TriggerDates = ko.dependentObservable(function () {
    var p = reportViewModel.DatePeriods();
    var endym = reportViewModel.DateList()[0].YearMonth().toString();
    var endmonth = parseInt(endym.substr(-2));
    if (endmonth == 12)
    {
        endmonth = 0;
    }
    //alert(endmonth);
    if (typeof p == "number") {
        p = p.toString();
    }
    var p2 = '';
    //p = p.replace(/[^\d]*/gi, "");
    var len = p.length;
    for (gg = 0; gg < len; gg++) {

        if (isNumeric(p.charAt(gg))) {
            p2 += p.charAt(gg);

        }
    }
    // alert("this is p2 = " + p2 + " and this is DatePeriods " + reportViewModel.DatePeriods());
    // alert("this is reportViewModel.DateList().length " + reportViewModel.DateList().length);
    if (p2 == "") {
        p2 = 1;
    }
    //
            
    //do not allow values < 0
    if (DirtyDate == false)
    {
        reportViewModel.DateErrorMessage("");
    }
    //reportViewModel.DateErrorMessage("");
    if (p2 != reportViewModel.DatePeriods()) {
        // alert("This is where it's gettting 1 and p2 ! = DatePeriods; p2 = " + p2 + " and reportViewModel.DatePeriods() = " + reportViewModel.DatePeriods());
        //  reportViewModel.DateErrorMessage("Invalid Characters were entered. Changed to 1.");
        DirtyDate = true;
        //  reportViewModel.DatePeriods(1);

    }
    else {
        if (p2 < 1) {
            reportViewModel.DateErrorMessage("Period must be greater than 0. Changed to 1.");
            reportViewModel.DatePeriods(1);

        }
        //do not allow values that are bigger than the index
        if (reportViewModel.DatePeriodType() == "M" && p2 > reportViewModel.DateList().length) {
                  
            reportViewModel.DateErrorMessage("Period calculation was changed to maximum monthly value.");
            reportViewModel.DatePeriods(reportViewModel.DateList().length);

        }
        if (reportViewModel.DatePeriodType() == "Q" && p2 > (reportViewModel.DateList().length / 3)) {
                  
            reportViewModel.DateErrorMessage("Period calculation was changed to maximum quarterly value.");
            reportViewModel.DatePeriods(Math.floor(reportViewModel.DateList().length / 3));

        }
        if (reportViewModel.DatePeriodType() == "A" && p2 > (reportViewModel.DateList().length / 12)) {
            // alert("This is for big year");
            reportViewModel.DateErrorMessage("Period calculation was changed to maximum yearly value.");
            reportViewModel.DatePeriods(Math.floor(reportViewModel.DateList().length / 12));

        }
        if (reportViewModel.DatePeriodType() == "Y" && p2 > (reportViewModel.DateList().length / 12)) {
            // alert("This is for big year");
            reportViewModel.DateErrorMessage("Period calculation was changed to maximum yearly value.");
            reportViewModel.DatePeriods(Math.floor(reportViewModel.DateList().length / 12));

        }
    }
    if (reportViewModel.DateCalculationType() == 'C') {
        var newindex = 0;
        if (reportViewModel.DatePeriodType() == "A") {
            newindex += (12 * parseInt(p2)) - 1;
        }
        if (reportViewModel.DatePeriodType() == "Y") {
            newindex += (12 * parseInt(p2)) - 1 + (endmonth);
        }
        if (reportViewModel.DatePeriodType() == "Q") {
            newindex += (3 * parseInt(p2)) - 1;
        }
        if (reportViewModel.DatePeriodType() == "M") {
            newindex += parseInt(p2) - 1;
        }
        if (newindex + 1 >= reportViewModel.DateList().length) {
            newindex = reportViewModel.DateList().length - 1;
        }

        reportViewModel.StartYearMonth(reportViewModel.DateList()[newindex].YearMonth());
        reportViewModel.EndYearMonth(reportViewModel.DateList()[0].YearMonth());
    }
            
}, reportViewModel);

//entered date checker
reportViewModel.InvalidDates = ko.dependentObservable(function () {
    var p = reportViewModel.StartYearMonth();
    var q = reportViewModel.EndYearMonth();
    if (p > q) {
        reportViewModel.InvalidDate("Start Date is greater than End Date");

    }
    else {
        reportViewModel.InvalidDate("");
    }
            
}, reportViewModel);
///count and report unbalanced paranthesis
reportViewModel.ParenthesisMessage = ko.dependentObservable( function() {
    var returnmessage = '';
    if (reportViewModel.ShowAdvancedFilter() == true)
    {
        var totalopened = 0;
        var totalclosed = 0;
            
        var closederror = '';
        var invalidmessage = "";
        var shouldforce = false;
        for (var i = 0; i < reportViewModel.MarketShareLiveFilterDetailsList().length; i++)
        {
            var newandor = reportViewModel.MarketShareLiveFilterDetailsList()[i].AndOr();
            if (newandor == 'OR')
            {
                shouldforce = true;
            }
            var newopened = reportViewModel.MarketShareLiveFilterDetailsList()[i].OpenParenthesis().match(/\(/gi);
                
            if (newopened != null)
            {   
                if (newopened.length > 1)
                {
                    shouldforce = true;
                }
                totalopened += newopened.length;  
            } 
            var newclosed = reportViewModel.MarketShareLiveFilterDetailsList()[i].CloseParenthesis().match(/\)/gi);
            if (newclosed != null)
            {   
                if (newclosed.length > 1)
                {
                    shouldforce = true;
                }
                totalclosed += newclosed.length;
                if (totalclosed > totalopened)
                {
                    closederror = "     ***  Parenthesis are not formatted correctly  ***";
                }
            }
        }
        if (totalopened != totalclosed)
        {
            returnmessage = "     *** Parenthesis are NOT balanced. Open: " + totalopened +"  Closed: "+totalclosed+ " ***";
        }
        else if (closederror != '')
        {
            returnmessage = closederror;
        }
                
               
    }
    else {

        for (var i = 0; i < reportViewModel.MarketShareLiveFilterDetailsList().length; i++)
        {
            var newandor = reportViewModel.MarketShareLiveFilterDetailsList()[i].AndOr();
            if (newandor == 'Or')
            {
                returnmessage = 'Filter was reformatted';
            }
            var newopened = reportViewModel.MarketShareLiveFilterDetailsList()[i].OpenParenthesis().match(/\(/gi);
            if (newopened != null)
            {  
                if (newopened.length > 1)
                {
                    returnmessage = 'Filter was reformatted';
                }
                        
            }
            var newclosed = reportViewModel.MarketShareLiveFilterDetailsList()[i].CloseParenthesis().match(/\)/gi);
            if (newclosed != null)
            {   
                if (newclosed.length > 1)
                {
                    returnmessage = 'Filter was reformatted';
                }
                        
            }
            reportViewModel.MarketShareLiveFilterDetailsList()[i].AndOr('AND');
            reportViewModel.MarketShareLiveFilterDetailsList()[i].OpenParenthesis('(');
            reportViewModel.MarketShareLiveFilterDetailsList()[i].CloseParenthesis(')');
                    
        }
                
    }
    return returnmessage ;
},reportViewModel);

reportViewModel.ParenthesisMessageGeographic = ko.dependentObservable( function() {
    var returnmessage = '';
    if (reportViewModel.ShowAdvancedFilter() == true)
    {
        var totalopened = 0;
        var totalclosed = 0;
            
        var closederror = '';
        var invalidmessage = "";
        var shouldforce = false;
        for (var i = 0; i < reportViewModel.MarketShareLiveFilterGeographicDetailsList().length; i++)
        {
            var newandor = reportViewModel.MarketShareLiveFilterGeographicDetailsList()[i].AndOr();
            if (newandor == 'Or')
            {
                shouldforce = true;
            }
            var newopened = reportViewModel.MarketShareLiveFilterGeographicDetailsList()[i].OpenParenthesis().match(/\(/gi);
            if (newopened != null)
            {  
                if (newopened.length > 1)
                {
                    shouldforce = true;
                }
                totalopened += newopened.length;  
            } 
            var newclosed = reportViewModel.MarketShareLiveFilterGeographicDetailsList()[i].CloseParenthesis().match(/\)/gi);
            if (newclosed != null)
            {   
                if (newclosed.length > 1)
                {
                    shouldforce = true;
                }        
                totalclosed += newclosed.length;  
                if (totalclosed > totalopened)
                {
                    closederror = "     ***  Parenthesis are not formatted correctly  ***";
                }
            }
        }
        if (totalopened != totalclosed)
        {
            returnmessage = "     *** Parenthesis are NOT balanced. Open: " + totalopened +"  Closed: "+totalclosed+ " ***";
        }
        else if (closederror != '')
        {
            returnmessage = closederror;
        }
                
    }
    else {

        for (var i = 0; i < reportViewModel.MarketShareLiveFilterGeographicDetailsList().length; i++)
        {
            var newandor = reportViewModel.MarketShareLiveFilterGeographicDetailsList()[i].AndOr();
            if (newandor == 'Or')
            {
                returnmessage = 'Filter was reformatted';
            }
            var newopened = reportViewModel.MarketShareLiveFilterGeographicDetailsList()[i].OpenParenthesis().match(/\(/gi);
            if (newopened != null)
            {  
                if (newopened.length > 1)
                {
                    returnmessage = 'Filter was reformatted';
                }
                        
            }
            var newclosed = reportViewModel.MarketShareLiveFilterGeographicDetailsList()[i].CloseParenthesis().match(/\)/gi);
            if (newclosed != null)
            {   
                if (newclosed.length > 1)
                {
                    returnmessage = 'Filter was reformatted';
                }
                        
            }
            reportViewModel.MarketShareLiveFilterGeographicDetailsList()[i].AndOr('AND');
            reportViewModel.MarketShareLiveFilterGeographicDetailsList()[i].OpenParenthesis('(');
            reportViewModel.MarketShareLiveFilterGeographicDetailsList()[i].CloseParenthesis(')');
                    
        }
                
    }
    return returnmessage ;
}, reportViewModel);

//validate current report state
reportViewModel.ValidateReportMessage = ko.dependentObservable(function () {
               
               
    var invalidmessage = "";
    //check for fields

    if (reportViewModel.MarketLevel() > 0) {
        if (!reportViewModel.Field1()) {
            invalidmessage = invalidmessage + "Error:Field 1 is invalid   ";
        }
    }
    if (reportViewModel.MarketLevel() > 1) {
        if (!reportViewModel.Field2()) {
            invalidmessage = invalidmessage + "Error:Field 2 is invalid   ";
        }
    }
    if (reportViewModel.MarketLevel() > 2) {
        if (!reportViewModel.Field3()) {
            invalidmessage = invalidmessage + "Error:Field 3 is invalid   ";
        }
    }
    if (reportViewModel.EndYearMonth() < reportViewModel.StartYearMonth()) {
                
        invalidmessage = invalidmessage + "Error:Start Date is greater than End Date   ";
                
    }
    //for (var i = 0; i < reportViewModel.MarketShareLiveFilterDetailsList().length; i++) {
    //    var newopened = reportViewModel.MarketShareLiveFilterDetailsList()[i].FieldName();;
    //    if (newopened != null) {
    //        totalopened += newopened.length;
    //    }
    //    var newclosed = reportViewModel.MarketShareLiveFilterDetailsList()[i].CloseParenthesis().match(/\)/gi);
    //    if (newclosed != null) {
    //        totalclosed += newclosed.length;
    //    }
    //}
    //if (totalopened != totalclosed) {
    //    var returnmessage = "     *** Parenthesis are NOT balanced. Open: " + totalopened + "  Closed: " + totalclosed + " ***";
    //}

    return invalidmessage;
}, reportViewModel);


///build the full SQL
function BuildSQL() {
    reportViewModel.SQLStatement("");
    var SQL = "";
    var tagstart = "<div>";
    var tagend = "</div>";
    var currentline = ""

    for (var i = 0; i < reportViewModel.MarketShareLiveFilterDetailsList().length; i++)
    {
        if (reportViewModel.MarketShareLiveFilterDetailsList()[i].SQLText() != null)
        { 
            if (i == 0)
            {
                currentline = reportViewModel.MarketShareLiveFilterDetailsList()[i].OpenParenthesis()+" "+reportViewModel.MarketShareLiveFilterDetailsList()[i].SQLText()+reportViewModel.MarketShareLiveFilterDetailsList()[i].CloseParenthesis();
            }
            else
            {
                currentline = reportViewModel.MarketShareLiveFilterDetailsList()[i].AndOr()+" "+reportViewModel.MarketShareLiveFilterDetailsList()[i].OpenParenthesis()+" "+reportViewModel.MarketShareLiveFilterDetailsList()[i].SQLText()+reportViewModel.MarketShareLiveFilterDetailsList()[i].CloseParenthesis();
            }
            SQL += tagstart+currentline+tagend; 
            //sqlMessage += " "+ currentline;
        }
    }
    reportViewModel.SQLStatement(SQL);
};

ko.applyBindings(reportViewModel); 
           
//reportViewModel.DataType1.subscribe(function(){
//    reportViewModel.ActionMethod("Update");
           
//    $("form#TheForm").submit();

//});
//reportViewModel.UnitId.subscribe(function(){
//    reportViewModel.ActionMethod("Update");
            
//    $("form#TheForm").submit();

//});
reportViewModel.InitialEndYearMonth.subscribe(function(){
           
            
    reportViewModel.ActionMethod("UpdateDate");
            
    $("form#TheForm").submit();
            

});
        
// attach the jquery unobtrusive validator	
$.validator.unobtrusive.parse("form");	
// bind the submit handler to unobtrusive validation.	
$("form").data("validator").settings.submitHandler = function() {reportViewModel.Save( $("form"));};
function setmessage(e) {
    reportViewModel.RequestMessage(e);
};
function setsuccessmessage() {
    reportViewModel.RequestMessage("Your Report Completed.  Status: COMPLETED");
};
function WizardSelect(e) {
    var selectedTab = $(e.item);   
    reportViewModel.WizardTab(selectedTab.index());    
};

function SetWizard() {
    var tabIndex = reportViewModel.WizardTab();
              
    var tabstrip = $("#TabStrip").data("tTabStrip");
    var item = $("li", tabstrip.element)[tabIndex];        
    tabstrip.select(item);
              
              
};

function WizardNext() {
    onTabClick();
    var tabIndex = parseInt(reportViewModel.WizardTab());
              
    if (tabIndex <= 5)
    {
        tabIndex = tabIndex + 1;
    }
            
    reportViewModel.WizardTab(tabIndex);
    var tabstrip = $("#TabStrip").data("tTabStrip");
    var item = $("li", tabstrip.element)[tabIndex];        
    tabstrip.select(item);
              
              
}; 
function WizardPrevious() {
    onTabClick();
    var tabIndex = reportViewModel.WizardTab();
    if (tabIndex >= 1)
    {
        tabIndex = tabIndex - 1;
    }
             
    reportViewModel.WizardTab(tabIndex);
    var tabstrip = $("#TabStrip").data("tTabStrip");
    var item = $("li", tabstrip.element)[tabIndex];        
    tabstrip.select(item);
};
function setColumnSorts(tableName,index,ascDesc) {

    forEach(document.getElementsByTagName('table'), function (table) {

        if (table.className.search(/\bsortable\b/) != -1 && table.id == tableName) {
            headrow = table.tHead.rows[0].cells;
            if (ascDesc == 'ASC') {
                headrow[index].className += ' sorttable_ascending';
                sortfwdind = document.createElement('span');
                sortfwdind.id = tableName + "sorttable_ascendingind";
                sortfwdind.innerHTML = stIsIE ? '&nbsp<font face="webdings">5</font>' : '&nbsp;&#x25B4;';
                headrow[index].appendChild(sortfwdind);
            }
            else {
                headrow[index].className += ' sorttable_descending';
                sortfwdind = document.createElement('span');
                sortfwdind.id = tableName + "sorttable_descendingind";
                sortfwdind.innerHTML = stIsIE ? '&nbsp<font face="webdings">6</font>' : '&nbsp;&#x25BE;';
                headrow[index].appendChild(sortfwdind);
            }
        }
    });
};
function getURLParam(strParamName){
    var strReturn = "";
    var strHref = window.location.href;
    if ( strHref.indexOf("?") > -1 ){
        var strQueryString = strHref.substr(strHref.indexOf("?")).toLowerCase();
        var aQueryString = strQueryString.split("&");
        for ( var iParam = 0; iParam < aQueryString.length; iParam++ ){
            if ( 
      aQueryString[iParam].indexOf(strParamName.toLowerCase() + "=") > -1 ){
                var aParam = aQueryString[iParam].split("=");
                strReturn = aParam[1];
                break;
            }
        }
    }
    return unescape(strReturn);
};
//used only during initial loading of data
function SetTrendSorts(){
            
    var trendreport = reportViewModel.ReportTypesId();
            
    if (trendreport != "MKTTREND" && trendreport != "MKTREPLICATIONTREND") {
        reportViewModel.Trending("0");
    }
    var trendvalue = reportViewModel.Trending();
    switch (trendvalue)
    {
        case '0': 
            reportViewModel.TrendOn("Off");
            reportViewModel.TrendOnPeriod("M");
            reportViewModel.TrendOnSort("DESC");
            break;
        case 'YASC': 
            reportViewModel.TrendOn("On");
            reportViewModel.TrendOnPeriod("Y");
            reportViewModel.TrendOnSort("ASC");
            break;
        case 'YDESC': 
            reportViewModel.TrendOn("On");
            reportViewModel.TrendOnPeriod("Y");
            reportViewModel.TrendOnSort("DESC");
            break;
        case 'QASC': 
            reportViewModel.TrendOn("On");
            reportViewModel.TrendOnPeriod("Q");
            reportViewModel.TrendOnSort("ASC");
            break;
        case 'QDESC': 
            reportViewModel.TrendOn("On");
            reportViewModel.TrendOnPeriod("Q");
            reportViewModel.TrendOnSort("DESC");
            break;
        case 'MASC': 
            reportViewModel.TrendOn("On");
            reportViewModel.TrendOnPeriod("M");
            reportViewModel.TrendOnSort("ASC");
            break;
        case 'MDESC': 
            reportViewModel.TrendOn("On");
            reportViewModel.TrendOnPeriod("M");
            reportViewModel.TrendOnSort("DESC");
            break;
        default: 
            reportViewModel.TrendOn("Off");
            reportViewModel.TrendOnPeriod("M");
            reportViewModel.TrendOnSort("DESC");
    }

};
function reloadGrid() {
    var grid2 = $("#GridViewResults").data("tGrid");
    grid2.rebind();
    //Timer = setTimeout("reloadGrid()", 10000);
    //TimerisOn = true;
    //TimerCounter = TimerCounter + 1;
    //if (TimerCounter > 20) {
    //    stopTimer();
    //}
};
//function stopTimer() {
//    clearTimeout(Timer);
            
//    TimerisOn = false;
       
// }
        
//set the trending value based on the combos
function SetTrendingValue(){
    //start
    var trendreport = reportViewModel.ReportTypesId();
    if (trendreport != "MKTTREND" && trendreport != "MKTREPLICATIONTREND") {
        // reportViewModel.Trending("0");
        reportViewModel.TrendOn("Off");
    }
    else {
        //reportViewModel.Trending("0");
        reportViewModel.TrendOn("On");
    }
    var TrendingValue = "0";
    if (reportViewModel.TrendOn() == "On")
    {
        TrendingValue = reportViewModel.TrendOnPeriod()+reportViewModel.TrendOnSort();
    }
    reportViewModel.Trending(TrendingValue);
    //stop 
};
//used only during initial loading of data
function SetFieldSorts(){
    var fieldsort;
    var fieldAscDesc;
    var columnIndex;
             
    fieldsort = reportViewModel.Field1SortBy();
    fieldAscDesc = reportViewModel.Field1AscDesc();
    columnIndex = GetSortIndex(fieldsort);
    setColumnSorts("Field1",columnIndex,fieldAscDesc);

    fieldsort = reportViewModel.Field2SortBy();
    fieldAscDesc = reportViewModel.Field2AscDesc();
    columnIndex = GetSortIndex(fieldsort);
    setColumnSorts("Field2",columnIndex,fieldAscDesc);

    fieldsort = reportViewModel.Field3SortBy();
    fieldAscDesc = reportViewModel.Field3AscDesc();
    columnIndex = GetSortIndex(fieldsort);
    setColumnSorts("Field3",columnIndex,fieldAscDesc);

};
function GetSortIndex(Field){
    var newindex = 1;
    switch (Field)
    {
        case 'FIELD1DESCRIPTION': newindex = 2;
            break;
        case 'FIELD2DESCRIPTION': newindex = 2;
            break;
        case 'FIELD3DESCRIPTION': newindex = 2;
            break;
        case 'FIELD1': newindex = 1;
            break;
        case 'FIELD2': newindex = 1;
            break;
        case 'FIELD3': newindex = 1;
            break;
  
        case 'FACILDISCH': newindex = 3;
            break;
        case 'STATEDISCH': newindex = 4;
            break;
        case 'DISCHMKTSHARE': newindex = 5;
            break;
        case 'DISCHHOSPSHARE': newindex = 6;
            break;
        case 'FACILLOS': newindex = 7;
            break;
        case 'STATELOS': newindex = 8;
            break;
        case 'LOSMKTSHARE': newindex = 9;
            break;
        case 'LOSHOSPSHARE': newindex = 10;
            break;
        default: newindex = 3;
    }
    return newindex;
};
  
function msieversion()
{
    var ua = window.navigator.userAgent
    var msie = ua.indexOf ( "MSIE " )

    if ( msie > 0 )      // If Internet Explorer, return version number
        return parseInt (ua.substring (msie+5, ua.indexOf (".", msie )))
    else                 // If another browser, return 0
        return 0

}
 
// alert( msieversion());              
window.onload=function(){SetTrendSorts();SetFieldSorts();SetWizard()}; // this work in IE 8

// window.onload=SetFieldSorts(); // this works in IE 9
    

/*** Script #3 ***/
   
function OnLoad() {
        $(this).find(".t-filter").click(function () {
            setTimeout(function () {
                $(".t-filter-operator").each(function () {
                    $(this).val("substringof");
                });
            });
        });
    }

function viewHipaa() {
              
    $("#contentholder2").html("To comply with HIPAA regulations, the results have been suppressed because there is a reasonable basis to believe that the information could be used to identify an individual.  Please contact support at  <a href='mailto: datakoala@mha.org'> datakoala@mha.org</a> if you need further assistance.")
    //$('div.contentholder').html(summarycontent);
    $('#ViewHIPAADialog').dialog('open');

}

$(function () {

        

    $('#ViewHIPAADialog').dialog({
        autoOpen: false,
        width: 650,
        resizable: false,
        title: 'HIPAA Warning',
        position: 'center top',
        modal: true,


        buttons: {
            "Close": function () {
                $(this).dialog("close");
            }
        }
    });
});  // end of Jquery functions  
