Public Class clshrEmpTraining
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oButton As SAPbouiCOM.Button
    Private oCombobox, oCombobox1 As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
   
    Private Sub PopulateDetails(ByVal Empid As String)
        Try
            Dim Strqry As String
            Dim oRect As SAPbobsCOM.Recordset
            oCombobox = oForm.Items.Item("1000002").Specific
            oCombobox1 = oForm.Items.Item("13").Specific
            oRect = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Strqry = "Select firstName +' ' +isnull(middleName,'')+' '+lastName as EmpName,isnull(dept,0) as dept,isnull(position,0) as position from OHEM where empID=" & Empid
            oRect.DoQuery(Strqry)
            If oRect.RecordCount > 0 Then
                oApplication.Utilities.setEdittextvalue(oForm, "6", oRect.Fields.Item(0).Value)
                Dim strdept As String = oRect.Fields.Item(1).Value
                If strdept <> "" Then
                    oCombobox.Select(strdept, SAPbouiCOM.BoSearchKey.psk_ByValue)
                    Department(oRect.Fields.Item(1).Value)
                End If
                Dim strpos As String = oRect.Fields.Item(2).Value
                If strpos <> 0 Then
                    oCombobox1.Select(strpos, SAPbouiCOM.BoSearchKey.psk_ByValue)
                    Position(oRect.Fields.Item(2).Value)
                    Databind2(strpos)
                End If

            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub

    Public Sub LoadForm(ByVal Empid As String, ByVal EmpName As String, Optional ByVal poscode As String = "", Optional ByVal posiname As String = "")
        oForm = oApplication.Utilities.LoadForm(xml_hr_EmpTraining, frm_hr_EmpTraining)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        'oApplication.Utilities.setEdittextvalue(oForm, "4", Empid)
        'oApplication.Utilities.setEdittextvalue(oForm, "6", EmpName)
        'oApplication.Utilities.setEdittextvalue(oForm, "13", poscode)
        'oApplication.Utilities.setEdittextvalue(oForm, "15", posiname)
        'FillDepartment(oForm)
        'oCombobox1 = oForm.Items.Item("1000002").Specific
        'oCombobox1.Select(aDeptCode, SAPbouiCOM.BoSearchKey.psk_ByValue)
        'Department(aDeptCode)
        ''oApplication.Utilities.setEdittextvalue(oForm, "19", aDeptName)
        'Databind2(poscode)
        oApplication.Utilities.setEdittextvalue(oForm, "4", Empid)
        FillDepartment(oForm)
        FillPosition(oForm)
        PopulateDetails(Empid)
        Databind(Empid, oForm)
        oCombobox1 = oForm.Items.Item("13").Specific
        Dim posid As String = oApplication.Utilities.getEdittextvalue(oForm, "15")
        If posid <> "" Then
            Databind2(oCombobox1.Selected.Description)
        Else
            Databind2("0")
        End If

        NewTrainingSummary(Empid)
        RequestSummary(oForm, Empid)
        oForm.Items.Item("1000001").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oForm.PaneLevel = 1
        reDrawForm(oForm)
        oForm.Freeze(False)
    End Sub

    Private Sub LoadData(ByVal aform As SAPbouiCOM.Form)
        aform.Freeze(True)
        Databind2(oApplication.Utilities.getEdittextvalue(aform, "15"))
        Databind(oApplication.Utilities.getEdittextvalue(aform, "4"), aform)
        oForm.PaneLevel = 1
        aform.Freeze(False)
    End Sub
    Private Sub FillDepartment(ByVal sform As SAPbouiCOM.Form)
        oCombobox = oForm.Items.Item("1000002").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select Code,Remarks from OUDP order by Code")
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        'oForm.Items.Item("1000002").DisplayDesc = True
    End Sub
    Private Sub FillPosition(ByVal sform As SAPbouiCOM.Form)
        oCombobox = oForm.Items.Item("13").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select posID,name from OHPS order by posID")
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        'oForm.Items.Item("1000002").DisplayDesc = True
    End Sub
    Private Sub Department(ByVal deptCode As String)
        Dim oSlpRS As SAPbobsCOM.Recordset
        Dim strcode, strqry As String
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strqry = "Select Remarks from OUDP where Code='" & deptCode & "'"
        oSlpRS.DoQuery(strqry)
        If oSlpRS.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(oForm, "19", oSlpRS.Fields.Item(0).Value)
        End If
        'oForm.Items.Item("1000002").DisplayDesc = True
    End Sub
    Private Sub Position(ByVal posCode As String)
        Dim oSlpRS As SAPbobsCOM.Recordset
        Dim strcode, strqry As String
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strqry = "Select descriptio from OHPS where posID='" & posCode & "'"
        oSlpRS.DoQuery(strqry)
        If oSlpRS.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(oForm, "15", oSlpRS.Fields.Item(0).Value)
        End If
        'oForm.Items.Item("1000002").DisplayDesc = True
    End Sub
    Private Sub RequestSummary(ByVal aform As SAPbouiCOM.Form, ByVal Empid As String)
        Dim strqry As String
        oForm = aform
        Try
            oForm.Freeze(True)
            oGrid = oForm.Items.Item("25").Specific
            oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_3")
            strqry = "  select U_Z_HREmpID,U_Z_TrainCode,U_Z_CourseCode,U_Z_CourseName,U_Z_CourseTypeDesc,U_Z_Startdt,U_Z_Enddt,U_Z_MinAttendees,U_Z_MaxAttendees,U_Z_AppStdt,U_Z_AppEnddt ,"
            strqry = strqry & " U_Z_InsName,U_Z_NoOfHours,U_Z_AttCost,U_Z_AddionalCost,U_Z_TotalCost,case U_Z_AttendeesStatus when 'D' then 'Dropped' when 'C' then 'completed' when 'F' then 'Failed' end as U_Z_AttendeesStatus,"
            strqry = strqry & " case U_Z_Status when 'P' then 'Pending' when 'A' then 'Accepted' else 'Rejected' end as U_Z_Status,U_Z_JENO 'JENO',U_Z_Remarks,Code from [@Z_HR_TRIN1] where U_Z_HREmpID='" & Empid & "' and U_Z_UpEmpTrain='Y' "
            oGrid.DataTable.ExecuteQuery(strqry)
            oGrid.Columns.Item("U_Z_HREmpID").TitleObject.Caption = "Employee Id"
            oGrid.Columns.Item("U_Z_HREmpID").Visible = False
            oEditTextColumn = oGrid.Columns.Item("U_Z_HREmpID")
            oEditTextColumn.LinkedObjectType = 171
            oGrid.Columns.Item("U_Z_TrainCode").TitleObject.Caption = "Agenda Code"
            oGrid.Columns.Item("U_Z_TrainCode").Editable = False
            oEditTextColumn = oGrid.Columns.Item("U_Z_TrainCode")
            oEditTextColumn.LinkedObjectType = "Z_HR_OTRIN"
            oGrid.Columns.Item("U_Z_CourseCode").TitleObject.Caption = "Course Code"
            oGrid.Columns.Item("U_Z_CourseCode").Editable = False
            oGrid.Columns.Item("U_Z_CourseName").TitleObject.Caption = "Course Name"
            oGrid.Columns.Item("U_Z_CourseName").Editable = False
            oGrid.Columns.Item("U_Z_CourseTypeDesc").TitleObject.Caption = "Course Type"
            oGrid.Columns.Item("U_Z_CourseTypeDesc").Visible = False
            oGrid.Columns.Item("U_Z_Startdt").TitleObject.Caption = "Course Start Date"
            oGrid.Columns.Item("U_Z_Startdt").Editable = False
            oGrid.Columns.Item("U_Z_Enddt").TitleObject.Caption = "Course End Date"
            oGrid.Columns.Item("U_Z_Enddt").Editable = False
            oGrid.Columns.Item("U_Z_MinAttendees").TitleObject.Caption = "No of Min.Attentees"
            oGrid.Columns.Item("U_Z_MinAttendees").Visible = False
            oGrid.Columns.Item("U_Z_MaxAttendees").TitleObject.Caption = "No of Max.Attentees"
            oGrid.Columns.Item("U_Z_MaxAttendees").Visible = False
            oGrid.Columns.Item("U_Z_AppStdt").TitleObject.Caption = "Application start Date"
            oGrid.Columns.Item("U_Z_AppStdt").Editable = False
            oGrid.Columns.Item("U_Z_AppEnddt").TitleObject.Caption = "Application End Date"
            oGrid.Columns.Item("U_Z_AppEnddt").Editable = False
            oGrid.Columns.Item("U_Z_InsName").TitleObject.Caption = "Instructor Name"
            oGrid.Columns.Item("U_Z_InsName").Editable = False
            oGrid.Columns.Item("U_Z_NoOfHours").TitleObject.Caption = "No of Hours"
            oGrid.Columns.Item("U_Z_NoOfHours").Editable = False
            oGrid.Columns.Item("U_Z_AttCost").TitleObject.Caption = "Attendee Cost"
            oGrid.Columns.Item("U_Z_AttCost").Editable = False
            oGrid.Columns.Item("U_Z_AddionalCost").TitleObject.Caption = "Additional Cost"
            oGrid.Columns.Item("U_Z_AddionalCost").Editable = False
            oGrid.Columns.Item("U_Z_TotalCost").TitleObject.Caption = "Total Cost"
            oGrid.Columns.Item("U_Z_TotalCost").Editable = False
            oGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Applicant Status"
            oGrid.Columns.Item("U_Z_Status").Visible = False
            oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Closing Remarks"
            oGrid.Columns.Item("U_Z_Remarks").Editable = False
            oGrid.Columns.Item("U_Z_AttendeesStatus").TitleObject.Caption = "Attendees Status"
            oGrid.Columns.Item("U_Z_AttendeesStatus").Editable = False
            oGrid.Columns.Item("JENO").TitleObject.Caption = "Cost Posting Reference"
            oGrid.Columns.Item("JENO").Editable = False
            oEditTextColumn = oGrid.Columns.Item("JENO")
            oEditTextColumn.LinkedObjectType = "30"
            oGrid.Columns.Item("Code").Visible = False
            oGrid.AutoResizeColumns()
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
            oForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
    Private Sub NewTrainingSummary(ByVal strempid As String)
        Dim strqry As String
        oGrid = oForm.Items.Item("23").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_2")
        strqry = " select DocEntry,U_Z_ReqDate,U_Z_HREmpID,U_Z_HREmpName,U_Z_DeptName,U_Z_PosiName,U_Z_CourseName,U_Z_CourseDetails,convert(varchar(10),U_Z_TrainFrdt,103) as U_Z_TrainFrdt,convert(varchar(10),U_Z_TrainTodt,103) as U_Z_TrainTodt,U_Z_TrainCost,U_Z_Notes,"
        strqry += " case U_Z_AppStatus when 'P' then 'Pending' when 'A' then 'Approved' when 'R' then 'Rejected' end as U_Z_AppStatus,CASE U_Z_ReqStatus when 'P' then 'Pending' when 'MA' then 'Manager Approved' when 'MR' then 'Manager Rejected'"
        strqry = strqry & " when 'HA' then 'HR Approved' else 'HR Rejected' end as U_Z_ReqStatus,case U_Z_mgrstatus when 'P' then 'Pending' when 'MA' then 'Approved' else 'Rejected' end as U_Z_MgrStatus,"
        strqry = strqry & " U_Z_MgrRemarks,CASE U_Z_HRStatus when 'P' then 'Pending' when 'HA' then 'HR Approved' when 'HR' then 'HR Rejected'"
        strqry = strqry & " end as U_Z_HRStatus,U_Z_HRRemarks  from [@Z_HR_ONTREQ] where U_Z_HREmpID='" & strempid & "'"
        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Request Code"
        oEditTextColumn = oGrid.Columns.Item("DocEntry")
        oEditTextColumn.LinkedObjectType = "Z_HR_ONTREQ"
        oGrid.Columns.Item("U_Z_ReqDate").TitleObject.Caption = "Request Date"
        oGrid.Columns.Item("U_Z_HREmpID").TitleObject.Caption = "Employee Id"
        oGrid.Columns.Item("U_Z_HREmpName").TitleObject.Caption = "Employee Name"
        oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department"
        oGrid.Columns.Item("U_Z_PosiName").TitleObject.Caption = "Position"
        oGrid.Columns.Item("U_Z_HREmpID").Visible = False
        oGrid.Columns.Item("U_Z_HREmpName").Visible = False
        oGrid.Columns.Item("U_Z_DeptName").Visible = False
        oGrid.Columns.Item("U_Z_PosiName").Visible = False
        oGrid.Columns.Item("U_Z_CourseName").TitleObject.Caption = "Training Title"
        oGrid.Columns.Item("U_Z_CourseDetails").TitleObject.Caption = "Justification"
        oGrid.Columns.Item("U_Z_TrainFrdt").TitleObject.Caption = "Training From Date"
        oGrid.Columns.Item("U_Z_TrainTodt").TitleObject.Caption = "Training To Date"
        oGrid.Columns.Item("U_Z_TrainCost").TitleObject.Caption = "Training Course Cost"
        oGrid.Columns.Item("U_Z_Notes").TitleObject.Caption = "Comments"
        oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
        oGrid.Columns.Item("U_Z_ReqStatus").Visible = False
        oGrid.Columns.Item("U_Z_MgrStatus").Visible = False
        oGrid.Columns.Item("U_Z_MgrRemarks").Visible = False
        oGrid.Columns.Item("U_Z_HRStatus").Visible = False
        oGrid.Columns.Item("U_Z_HRRemarks").Visible = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
    Private Sub Databind2(ByVal strposcode As String)
        Dim strqry As String
        oGrid = oForm.Items.Item("7").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
        strqry = "  select distinct( U_Z_TrainCode),U_Z_DocDate ,T0.U_Z_CourseCode as 'CourseCode',T0.U_Z_CourseName as 'CourseName',U_Z_CourseTypeDesc,U_Z_Startdt,U_Z_Enddt,U_Z_MinAttendees,U_Z_MaxAttendees,U_Z_AppStdt,U_Z_AppEnddt ,"
        strqry = strqry & " U_Z_InsName,U_Z_NoOfHours,U_Z_StartTime,U_Z_EndTime,isnull(U_Z_Sunday,'N') 'U_Z_Sunday',isnull(U_Z_Monday,'N') 'U_Z_Monday',isnull(U_Z_Tuesday,'N') 'U_Z_Tuesday',isnull(U_Z_Wednesday,'N') 'U_Z_Wednesday',isnull(U_Z_Thursday,'N') 'U_Z_Thursday',isnull(U_Z_Friday,'N') 'U_Z_Friday',isnull(U_Z_Saturday,'N') 'U_Z_Saturday',U_Z_AttCost,U_Z_Active  from [@Z_HR_OTRIN] T0 inner join [@Z_HR_OCOUR] T1 on T0.U_Z_CourseCode=T1.U_Z_CourseCode inner join "
        strqry = strqry & "  [@Z_HR_COUR4] T2 on T1.DocEntry=t2.DocEntry where  (isnull(T1.U_Z_Allpos,'N')='Y' or  T2.U_Z_PosCode='" & strposcode & "') and T0.U_Z_Active='Y' and isnull(T0.U_Z_Status,'O')='O'"

        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("U_Z_TrainCode").TitleObject.Caption = "Agenda Code"
        oGrid.Columns.Item("U_Z_TrainCode").Editable = False
        oEditTextColumn = oGrid.Columns.Item("U_Z_TrainCode")
        oEditTextColumn.LinkedObjectType = "Z_HR_OTRIN"
        oGrid.Columns.Item("U_Z_DocDate").TitleObject.Caption = "Agenda Date"
        oGrid.Columns.Item("U_Z_DocDate").Editable = False
        oGrid.Columns.Item("CourseCode").TitleObject.Caption = "Course Code"
        oGrid.Columns.Item("CourseCode").Editable = False
        oGrid.Columns.Item("CourseName").TitleObject.Caption = "Course Name"
        oGrid.Columns.Item("CourseName").Editable = False
        oGrid.Columns.Item("U_Z_CourseTypeDesc").TitleObject.Caption = "Course Type"
        oGrid.Columns.Item("U_Z_CourseTypeDesc").Editable = False
        oGrid.Columns.Item("U_Z_Startdt").TitleObject.Caption = "Course Start Date"
        oGrid.Columns.Item("U_Z_Startdt").Editable = False
        oGrid.Columns.Item("U_Z_Enddt").TitleObject.Caption = "Course End Date"
        oGrid.Columns.Item("U_Z_Enddt").Editable = False
        oGrid.Columns.Item("U_Z_MinAttendees").TitleObject.Caption = "No of Min.Attentees"
        oGrid.Columns.Item("U_Z_MinAttendees").Editable = False
        oGrid.Columns.Item("U_Z_MaxAttendees").TitleObject.Caption = "No of Max.Attentees"
        oGrid.Columns.Item("U_Z_MaxAttendees").Editable = False
        oGrid.Columns.Item("U_Z_AppStdt").TitleObject.Caption = "Application start Date"
        oGrid.Columns.Item("U_Z_AppStdt").Editable = False
        oGrid.Columns.Item("U_Z_AppEnddt").TitleObject.Caption = "Application End Date"
        oGrid.Columns.Item("U_Z_AppEnddt").Editable = False
        oGrid.Columns.Item("U_Z_InsName").TitleObject.Caption = "Instructor Name"
        oGrid.Columns.Item("U_Z_InsName").Editable = False
        oGrid.Columns.Item("U_Z_NoOfHours").TitleObject.Caption = "No of Hours"
        oGrid.Columns.Item("U_Z_NoOfHours").Editable = False
        oGrid.Columns.Item("U_Z_StartTime").TitleObject.Caption = "Start Time"
        oGrid.Columns.Item("U_Z_StartTime").Editable = False
        oGrid.Columns.Item("U_Z_EndTime").TitleObject.Caption = "End Time"
        oGrid.Columns.Item("U_Z_EndTime").Editable = False
        oGrid.Columns.Item("U_Z_Sunday").TitleObject.Caption = "Sunday"
        oGrid.Columns.Item("U_Z_Sunday").Editable = False
        oGrid.Columns.Item("U_Z_Monday").TitleObject.Caption = "Monday"
        oGrid.Columns.Item("U_Z_Monday").Editable = False
        oGrid.Columns.Item("U_Z_Tuesday").TitleObject.Caption = "Tuesday"
        oGrid.Columns.Item("U_Z_Tuesday").Editable = False
        oGrid.Columns.Item("U_Z_Wednesday").TitleObject.Caption = "Wednesday"
        oGrid.Columns.Item("U_Z_Wednesday").Editable = False
        oGrid.Columns.Item("U_Z_Thursday").TitleObject.Caption = "Thursday"
        oGrid.Columns.Item("U_Z_Thursday").Editable = False
        oGrid.Columns.Item("U_Z_Friday").TitleObject.Caption = "Friday"
        oGrid.Columns.Item("U_Z_Friday").Editable = False
        oGrid.Columns.Item("U_Z_Saturday").TitleObject.Caption = "Saturday"
        oGrid.Columns.Item("U_Z_Saturday").Editable = False
        oGrid.Columns.Item("U_Z_AttCost").TitleObject.Caption = "Attendee Cost"
        oGrid.Columns.Item("U_Z_AttCost").Editable = False
        oGrid.Columns.Item("U_Z_Active").TitleObject.Caption = "Active"
        oGrid.Columns.Item("U_Z_Active").Editable = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
    Private Sub Databind(ByVal empid As String, ByVal aForm As SAPbouiCOM.Form)
        Dim strqry As String
        oForm = aForm
        Try
            oForm.Freeze(True)
            oGrid = oForm.Items.Item("11").Specific
            oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_1")
            strqry = "  select U_Z_HREmpID,U_Z_TrainCode,U_Z_CourseCode,U_Z_CourseName,U_Z_CourseTypeDesc,U_Z_Startdt,U_Z_Enddt,U_Z_MinAttendees,U_Z_MaxAttendees,U_Z_AppStdt,U_Z_AppEnddt ,"
            strqry = strqry & " U_Z_InsName,U_Z_NoOfHours,U_Z_StartTime,U_Z_EndTime,isnull(U_Z_Sunday,'N') 'U_Z_Sunday',isnull(U_Z_Monday,'N') 'U_Z_Monday',isnull(U_Z_Tuesday,'N') 'U_Z_Tuesday',isnull(U_Z_Wednesday,'N') 'U_Z_Wednesday',isnull(U_Z_Thursday,'N') 'U_Z_Thursday',isnull(U_Z_Friday,'N') 'U_Z_Friday',isnull(U_Z_Saturday,'N') 'U_Z_Saturday',U_Z_AttCost,U_Z_Active,"
            strqry = strqry & " case U_Z_AppStatus when 'P' then 'Pending' when 'A' then 'Approved' else 'Rejected' end as U_Z_AppStatus, case U_Z_Status when 'P' then 'Pending' when 'A' then 'Accepted' else 'Rejected' end as U_Z_Status,U_Z_Remarks,Code from [@Z_HR_TRIN1] where U_Z_HREmpID='" & empid & "' "
            oGrid.DataTable.ExecuteQuery(strqry)
            oGrid.Columns.Item("U_Z_HREmpID").TitleObject.Caption = "Employee Id"
            oGrid.Columns.Item("U_Z_HREmpID").Visible = False
            oEditTextColumn = oGrid.Columns.Item("U_Z_HREmpID")
            oEditTextColumn.LinkedObjectType = 171
            oGrid.Columns.Item("U_Z_TrainCode").TitleObject.Caption = "Agenda Code"
            oGrid.Columns.Item("U_Z_TrainCode").Editable = False
            oEditTextColumn = oGrid.Columns.Item("U_Z_TrainCode")
            oEditTextColumn.LinkedObjectType = "Z_HR_OTRIN"
            oGrid.Columns.Item("U_Z_CourseCode").TitleObject.Caption = "Course Code"
            oGrid.Columns.Item("U_Z_CourseCode").Editable = False
            oGrid.Columns.Item("U_Z_CourseName").TitleObject.Caption = "Course Name"
            oGrid.Columns.Item("U_Z_CourseName").Editable = False
            oGrid.Columns.Item("U_Z_CourseTypeDesc").TitleObject.Caption = "Course Type"
            oGrid.Columns.Item("U_Z_CourseTypeDesc").Editable = False
            oGrid.Columns.Item("U_Z_Startdt").TitleObject.Caption = "Course Start Date"
            oGrid.Columns.Item("U_Z_Startdt").Editable = False
            oGrid.Columns.Item("U_Z_Enddt").TitleObject.Caption = "Course End Date"
            oGrid.Columns.Item("U_Z_Enddt").Editable = False
            oGrid.Columns.Item("U_Z_MinAttendees").TitleObject.Caption = "No of Min.Attentees"
            oGrid.Columns.Item("U_Z_MinAttendees").Editable = False
            oGrid.Columns.Item("U_Z_MaxAttendees").TitleObject.Caption = "No of Max.Attentees"
            oGrid.Columns.Item("U_Z_MaxAttendees").Editable = False
            oGrid.Columns.Item("U_Z_AppStdt").TitleObject.Caption = "Application start Date"
            oGrid.Columns.Item("U_Z_AppStdt").Editable = False
            oGrid.Columns.Item("U_Z_AppEnddt").TitleObject.Caption = "Application End Date"
            oGrid.Columns.Item("U_Z_AppEnddt").Editable = False
            oGrid.Columns.Item("U_Z_InsName").TitleObject.Caption = "Instructor Name"
            oGrid.Columns.Item("U_Z_InsName").Editable = False
            oGrid.Columns.Item("U_Z_NoOfHours").TitleObject.Caption = "No of Hours"
            oGrid.Columns.Item("U_Z_NoOfHours").Editable = False
            oGrid.Columns.Item("U_Z_StartTime").TitleObject.Caption = "Start Time"
            oGrid.Columns.Item("U_Z_StartTime").Editable = False
            oGrid.Columns.Item("U_Z_EndTime").TitleObject.Caption = "End Time"
            oGrid.Columns.Item("U_Z_EndTime").Editable = False
            oGrid.Columns.Item("U_Z_Sunday").TitleObject.Caption = "Sunday"
            oGrid.Columns.Item("U_Z_Sunday").Editable = False
            oGrid.Columns.Item("U_Z_Monday").TitleObject.Caption = "Monday"
            oGrid.Columns.Item("U_Z_Monday").Editable = False
            oGrid.Columns.Item("U_Z_Tuesday").TitleObject.Caption = "Tuesday"
            oGrid.Columns.Item("U_Z_Tuesday").Editable = False
            oGrid.Columns.Item("U_Z_Wednesday").TitleObject.Caption = "Wednesday"
            oGrid.Columns.Item("U_Z_Wednesday").Editable = False
            oGrid.Columns.Item("U_Z_Thursday").TitleObject.Caption = "Thursday"
            oGrid.Columns.Item("U_Z_Thursday").Editable = False
            oGrid.Columns.Item("U_Z_Friday").TitleObject.Caption = "Friday"
            oGrid.Columns.Item("U_Z_Friday").Editable = False
            oGrid.Columns.Item("U_Z_Saturday").TitleObject.Caption = "Saturday"
            oGrid.Columns.Item("U_Z_Saturday").Editable = False
            oGrid.Columns.Item("U_Z_AttCost").TitleObject.Caption = "Attendee Cost"
            oGrid.Columns.Item("U_Z_AttCost").Editable = False
            oGrid.Columns.Item("U_Z_Active").TitleObject.Caption = "Active"
            oGrid.Columns.Item("U_Z_Active").Editable = False
            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Status"
            oGrid.Columns.Item("U_Z_AppStatus").Editable = False
            oGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Status"
            oGrid.Columns.Item("U_Z_Status").Visible = False
            oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"
            oGrid.Columns.Item("U_Z_Remarks").Editable = False
            oGrid.Columns.Item("Code").Visible = False
            oGrid.AutoResizeColumns()
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
            oForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
    Private Sub reDrawForm(ByVal oForm As SAPbouiCOM.Form)
        Try
            oForm.Freeze(True)
            oForm.Items.Item("10").Width = oForm.Width - 25
            oForm.Items.Item("10").Height = oForm.Items.Item("23").Height + 10
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
        End Try
    End Sub
#Region "AddToUDT"
    Private Function AddToUDT(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim strTable, strEmpId, strCode, strType, strAccountCode, strqry, strDeptcode, strStatus As String
        Dim strcount As Integer
        Dim dblValue As Double
        Dim dtFromDate, dtTodate, dt, AppEnddt As Date
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim oValidateRS, otemp2 As SAPbobsCOM.Recordset
        Dim otemp, otemp1, otemprs As SAPbobsCOM.Recordset
        strEmpId = oApplication.Utilities.getEdittextvalue(aForm, "4")
        oCombobox = aForm.Items.Item("1000002").Specific
        oCombobox1 = aForm.Items.Item("13").Specific
        strDeptcode = oCombobox.Selected.Value
        oUserTable = oApplication.Company.UserTables.Item("Z_HR_TRIN1")
        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        dt = Now.Date
        oGrid = aForm.Items.Item("7").Specific
        strTable = "@Z_HR_TRIN1"
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.Rows.IsSelected(intRow) Then
                strStatus = oGrid.DataTable.GetValue("U_Z_Active", intRow)
                otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemp.DoQuery("Select * from [@Z_HR_TRIN1] where U_Z_TrainCode='" & oGrid.DataTable.GetValue("U_Z_TrainCode", intRow) & "' and U_Z_HREmpID='" & strEmpId & "'")
                If otemp.RecordCount > 0 Then
                    oApplication.Utilities.Message("You already applied for the selected Training", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                'AppEnddt = oGrid.DataTable.GetValue("EndDate", intRow)
                'If AppEnddt < dt Then
                '    oApplication.Utilities.Message("Application End date must be Less than or equal to Today date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '    Return False
                'End If

                otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                dtFromDate = oGrid.DataTable.GetValue("U_Z_AppStdt", intRow)
                dtTodate = oGrid.DataTable.GetValue("U_Z_AppEnddt", intRow)
                strqry = "Select * from [@Z_HR_OTRIN] where '" & dt.ToString("yyyy-MM-dd") & "' between '" & dtFromDate.ToString("yyyy-MM-dd") & "' and '" & dtTodate.ToString("yyyy-MM-dd") & "'"
                otemp2.DoQuery(strqry)
                If otemp2.RecordCount <= 0 Then
                    oApplication.Utilities.Message("Application request date are not available..", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                If strStatus = "Y" Then
                    strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_HREmpID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_HREmpName").Value = oApplication.Utilities.getEdittextvalue(oForm, "6")
                    oUserTable.UserFields.Fields.Item("U_Z_PosiCode").Value = oCombobox1.Selected.Value
                    oUserTable.UserFields.Fields.Item("U_Z_PosiName").Value = oApplication.Utilities.getEdittextvalue(oForm, "15")
                    oUserTable.UserFields.Fields.Item("U_Z_DeptCode").Value = strDeptcode
                    oUserTable.UserFields.Fields.Item("U_Z_DeptName").Value = oApplication.Utilities.getEdittextvalue(oForm, "19")
                    oUserTable.UserFields.Fields.Item("U_Z_TrainCode").Value = oGrid.DataTable.GetValue("U_Z_TrainCode", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_CourseCode").Value = oGrid.DataTable.GetValue("CourseCode", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_CourseName").Value = oGrid.DataTable.GetValue("CourseName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_CourseTypeDesc").Value = oGrid.DataTable.GetValue("U_Z_CourseTypeDesc", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Startdt").Value = oGrid.DataTable.GetValue("U_Z_Startdt", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Enddt").Value = oGrid.DataTable.GetValue("U_Z_Enddt", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_MinAttendees").Value = oGrid.DataTable.GetValue("U_Z_MinAttendees", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_MaxAttendees").Value = oGrid.DataTable.GetValue("U_Z_MaxAttendees", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_AppStdt").Value = oGrid.DataTable.GetValue("U_Z_AppStdt", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_AppEnddt").Value = oGrid.DataTable.GetValue("U_Z_AppEnddt", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_InsName").Value = oGrid.DataTable.GetValue("U_Z_InsName", intRow)
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_NoOfHours").Value = oGrid.DataTable.GetValue("U_Z_NoOfHours", intRow)
                    Catch ex As Exception

                    End Try

                    oUserTable.UserFields.Fields.Item("U_Z_StartTime").Value = oGrid.DataTable.GetValue("U_Z_StartTime", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EndTime").Value = oGrid.DataTable.GetValue("U_Z_EndTime", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Sunday").Value = oGrid.DataTable.GetValue("U_Z_Sunday", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Monday").Value = oGrid.DataTable.GetValue("U_Z_Monday", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Tuesday").Value = oGrid.DataTable.GetValue("U_Z_Tuesday", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Wednesday").Value = oGrid.DataTable.GetValue("U_Z_Wednesday", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Thursday").Value = oGrid.DataTable.GetValue("U_Z_Thursday", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Friday").Value = oGrid.DataTable.GetValue("U_Z_Friday", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Saturday").Value = oGrid.DataTable.GetValue("U_Z_Saturday", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_AttCost").Value = oGrid.DataTable.GetValue("U_Z_AttCost", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Active").Value = oGrid.DataTable.GetValue("U_Z_Active", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_AppStatus").Value = oApplication.Utilities.DocApproval(aForm, HeaderDoctype.Train, strEmpId)
                    oUserTable.UserFields.Fields.Item("U_Z_ApplyDate").Value = dt

                    If oUserTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    Else

                        Dim intTempID As String = oApplication.Utilities.GetTemplateID(aForm, HeaderDoctype.Train, strEmpId)
                        If intTempID <> "0" Then
                            oApplication.Utilities.InitialMessage("Reg.Training Request", strCode, oApplication.Utilities.DocApproval(aForm, HeaderDoctype.Train, strEmpId), intTempID, oApplication.Utilities.getEdittextvalue(aForm, "6"), HistoryDoctype.LveReq)
                        End If
                        oApplication.Utilities.Message("Application Request submitted successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                    End If
                Else
                    oApplication.Utilities.Message("Training schedule already " & strStatus & " ..", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                'otemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                'strqry = "Select count(*) as Code from [@Z_HR_TRIN1] where U_Z_TrainCode='" & oGrid.DataTable.GetValue("U_Z_TrainCode", intRow) & "' group by U_Z_TrainCode"
                'otemp1.DoQuery(strqry)
                'If 1 = 1 Then ' otemp1.RecordCount > 0 Then
                '    strcount = otemp1.Fields.Item("Code").Value
                '    otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                '    otemprs.DoQuery("Update [@Z_HR_OTRIN] set U_Z_ReqAtten='" & strcount & "' where U_Z_TrainCode='" & oGrid.DataTable.GetValue("U_Z_TrainCode", intRow) & "' ")
                'End If
            End If
        Next

        oUserTable = Nothing
        Return True
    End Function
#End Region

#Region "Withdraw application"
    Private Sub WithdrawApplication(ByVal aform As SAPbouiCOM.Form)
        Dim oTemp, otemp1 As SAPbobsCOM.Recordset
        oGrid = aform.Items.Item("11").Specific
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.Rows.IsSelected(intRow) Then
                If oApplication.SBO_Application.MessageBox("Do you want to withdraw the selected trainning?", , "Yes", "No") = 2 Then
                    Exit Sub
                End If
                Dim strcode, strqry, strTrainningCode As String
                strcode = oGrid.DataTable.GetValue("U_Z_AppStatus", intRow)
                If strcode.ToUpper() <> "PENDING" Then
                    oApplication.Utilities.Message("Training Request already approved. You can not withdraw the request", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Exit Sub
                End If
                strcode = oGrid.DataTable.GetValue("Code", intRow)
                strTrainningCode = oGrid.DataTable.GetValue("U_Z_TrainCode", intRow)
                oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oTemp.DoQuery("Delete from [@Z_HR_TRIN1] where code='" & strcode & "'")
                ' strqry = "Select count(*) as Code from [@Z_HR_TRIN1] where U_Z_TrainCode='" & oGrid.DataTable.GetValue("U_Z_TrainCode", intRow) & "' group by U_Z_TrainCode"
                'oTemp.DoQuery(strqry)
                'If 1 = 1 Then 'oTemp.f Then
                '    otemp1.DoQuery("Update [@Z_HR_OTRIN] set U_Z_ReqAtten='" & oTemp.Fields.Item(0).Value & "' where U_Z_TrainCode='" & strTrainningCode & "' ")
                'End If
            End If
        Next
        LoadData(oForm)

    End Sub
#End Region


#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_EmpTraining Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If (pVal.ItemUID = "7" And pVal.ColUID = "U_Z_TrainCode") Or (pVal.ItemUID = "11" And pVal.ColUID = "U_Z_TrainCode") Or (pVal.ItemUID = "25" And pVal.ColUID = "U_Z_TrainCode") Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcode As String = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    oApplication.Utilities.OpenMasterinLink(oForm, "AgendaCode", strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "23" And pVal.ColUID = "DocEntry" Then
                                    oGrid = oForm.Items.Item("23").Specific
                                    For intRow As Integer = pVal.Row To pVal.Row
                                        If 1 = 1 Then
                                            Dim strcode, strStatus As String
                                            strcode = oGrid.DataTable.GetValue("DocEntry", intRow)
                                            strStatus = oGrid.DataTable.GetValue("U_Z_ReqStatus", intRow)
                                            Dim objct As New clshrNewTrainRequest
                                            objct.LoadForm1(strcode, strStatus)
                                        End If
                                    Next
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                '  ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                reDrawForm(oForm)

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "28" Then
                                    Select Case oForm.PaneLevel
                                        Case "2"
                                            oGrid = oForm.Items.Item("11").Specific
                                            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                                If oGrid.Rows.IsSelected(intRow) Then
                                                    Dim objHistory As New clshrAppHisDetails
                                                    If oGrid.DataTable.GetValue("Code", intRow) <> "" Then
                                                        objHistory.LoadForm(oForm, HistoryDoctype.RegTra, oGrid.DataTable.GetValue("Code", intRow))
                                                    End If
                                                    Exit Sub
                                                End If
                                            Next
                                        Case "3"
                                            oGrid = oForm.Items.Item("23").Specific
                                            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                                If oGrid.Rows.IsSelected(intRow) Then
                                                    Dim objHistory As New clshrAppHisDetails
                                                    If oGrid.DataTable.GetValue("DocEntry", intRow) > 0 Then
                                                        objHistory.LoadForm(oForm, HistoryDoctype.NewTra, oGrid.DataTable.GetValue("DocEntry", intRow))
                                                    End If
                                                    Exit Sub
                                                End If
                                            Next
                                    End Select
                                End If

                                If pVal.ItemUID = "8" Then
                                    If AddToUDT(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    Else
                                        blnFlag = False
                                        ' oForm.Close()
                                        Databind(oApplication.Utilities.getEdittextvalue(oForm, "4"), oForm)

                                    End If
                                End If
                                Select Case pVal.ItemUID
                                    Case "20"
                                        WithdrawApplication(oForm)
                                    Case "1000001"
                                        oForm.PaneLevel = 1
                                        oButton = oForm.Items.Item("8").Specific
                                        oButton.Caption = "Apply"
                                        oForm.Items.Item("8").Visible = True

                                    Case "9"
                                        oForm.PaneLevel = 2
                                        Databind(oApplication.Utilities.getEdittextvalue(oForm, "4"), oForm)
                                        oForm.Items.Item("8").Visible = False
                                    Case "22"
                                        oForm.PaneLevel = 3
                                    Case "21"
                                        Dim empid, empname, poscode, posName As String
                                        oCombobox1 = oForm.Items.Item("1000002").Specific
                                        oCombobox = oForm.Items.Item("13").Specific
                                        empid = oApplication.Utilities.getEdittextvalue(oForm, "4")
                                        empname = oApplication.Utilities.getEdittextvalue(oForm, "6")
                                        poscode = oCombobox.Selected.Value  'oApplication.Utilities.getEdittextvalue(oForm, "13")
                                        posName = oApplication.Utilities.getEdittextvalue(oForm, "15")
                                        Dim objct As New clshrNewTrainRequest
                                        objct.LoadForm(empid, empname, poscode, posName, oCombobox1.Selected.Value, oCombobox1.Selected.Description)
                                    Case "24"
                                        oForm.PaneLevel = 4
                                    Case "26"
                                        oGrid = oForm.Items.Item("25").Specific
                                        Dim strtraincode As String = ""
                                        Dim strempid As String = ""
                                        If oGrid.Rows.Count > 0 Then
                                            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                                If oGrid.Rows.IsSelected(intRow) Then
                                                    strtraincode = oGrid.DataTable.GetValue("U_Z_TrainCode", intRow)
                                                    strempid = oGrid.DataTable.GetValue("U_Z_HREmpID", intRow)
                                                    Dim objEmp As New clshrEmpAbsSummary
                                                    objEmp.LoadForm(strempid, strtraincode)
                                                End If
                                            Next
                                        End If
                                End Select

                        End Select
                End Select
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
#End Region

#Region "Menu Event"
    Public Overrides Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            Select Case pVal.MenuUID
                Case mnu_hr_SheduleTrain
                    Dim oTe As New clshrLogin
                    oTe.LoadForm("SCHTRA")
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
            End Select
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
#End Region

    Public Sub FormDataEvent(ByRef BusinessObjectInfo As SAPbouiCOM.BusinessObjectInfo, ByRef BubbleEvent As Boolean)
        Try
            If BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
