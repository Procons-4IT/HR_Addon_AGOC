Public Class clsAppShortListedApproval
    Inherits clsBase

    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private ocombo As SAPbouiCOM.ComboBoxColumn

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub LoadForm(ByVal strtitle As String, ByVal empid As String, ByVal empname As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_RecApproval) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_RecApproval, frm_hr_RecApproval)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        If strtitle = "RHR" Then
            oForm.Title = "Recruitment First Level Approval"
            oForm.Items.Item("4").Visible = False
            oForm.Items.Item("12").Visible = True
        ElseIf strtitle = "RGM" Then
            oForm.Title = "Recruitment HR Approval"
            oForm.Items.Item("4").Visible = False
            oForm.Items.Item("12").Visible = True
        Else
            oForm.Title = "Manager Review Status"
            oForm.Items.Item("4").Visible = True
            oForm.Items.Item("12").Visible = False
        End If
        oForm.Freeze(True)
        oApplication.Utilities.setEdittextvalue(oForm, "5", empid)
        oApplication.Utilities.setEdittextvalue(oForm, "7", empname)
        Databind(empid, strtitle)
        Databind2(empid, strtitle)
        oForm.PaneLevel = 1
        oForm.Freeze(False)
    End Sub
    Private Sub Databind(ByVal strempid As String, ByVal strtitle As String)
        Dim strqry As String
        oGrid = oForm.Items.Item("3").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
        If strempid <> "" And strtitle = "MPR" Then
            strqry = "select DocEntry,U_Z_ReqDate,U_Z_EmpCode,U_Z_EmpName,U_Z_DeptCode,U_Z_DeptName,"
            strqry = strqry & " case U_Z_ReqClss when 'E' then 'Existing' else 'New Position' end as 'U_Z_ReqClss',isnull(U_Z_PosName,'') +''+ isnull(U_Z_NewPosi,'') as Position,U_Z_ExpMin,U_Z_ExpMax,"
            strqry = strqry & " U_Z_Vacancy,U_Z_MgrStatus ,U_Z_MgrRemarks,U_Z_HODStatus,U_Z_HODRemarks,U_Z_HRStatus"
            strqry = strqry & "  ,U_Z_HRRemarks,U_Z_EmpstDate,U_Z_IntAppDead,U_Z_ExtAppDead  from [@Z_HR_ORMPREQ] where U_Z_EmpCode='" & strempid & "'"
        ElseIf strtitle = "RHR" Then
            strqry = "select DocEntry,U_Z_ReqDate,U_Z_EmpCode,U_Z_EmpName,U_Z_DeptCode,U_Z_DeptName,"
            strqry = strqry & " case U_Z_ReqClss when 'E' then 'Existing' else 'New Position' end as 'U_Z_ReqClss',isnull(U_Z_PosName,'') +''+ isnull(U_Z_NewPosi,'') as Position,U_Z_ExpMin,U_Z_ExpMax,"
            strqry = strqry & " U_Z_Vacancy,U_Z_MgrStatus,U_Z_MgrRemarks,U_Z_HRStatus,U_Z_HRRemarks,U_Z_HODStatus"
            strqry = strqry & " ,U_Z_HODRemarks,U_Z_EmpstDate,U_Z_IntAppDead,U_Z_ExtAppDead from [@Z_HR_ORMPREQ] where (U_Z_MgrStatus='O' or U_Z_MgrStatus='SA' or U_Z_MgrStatus='SR') "
        ElseIf strtitle = "RGM" Then
            strqry = "select DocEntry,U_Z_ReqDate,U_Z_EmpCode,U_Z_EmpName,U_Z_DeptCode,U_Z_DeptName,"
            strqry = strqry & " case U_Z_ReqClss when 'E' then 'Existing' else 'New Position' end as 'U_Z_ReqClss',isnull(U_Z_PosName,'') +''+ isnull(U_Z_NewPosi,'') as Position,U_Z_ExpMin,U_Z_ExpMax,"
            strqry = strqry & " U_Z_Vacancy,U_Z_MgrStatus,U_Z_MgrRemarks,U_Z_HODStatus"
            strqry = strqry & " ,U_Z_HODRemarks,U_Z_HRStatus,U_Z_HRRemarks,U_Z_EmpstDate,U_Z_IntAppDead,U_Z_ExtAppDead    from [@Z_HR_ORMPREQ] where U_Z_HODStatus='SA'"
        End If
        oGrid.DataTable.ExecuteQuery(strqry)
        oEditTextColumn = oGrid.Columns.Item("DocEntry")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Invoice
        If strtitle = "MPR" Then
            oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Request Number"
            oGrid.Columns.Item("DocEntry").Editable = False
            oGrid.Columns.Item("U_Z_ReqDate").TitleObject.Caption = "Requested Date"
            oGrid.Columns.Item("U_Z_ReqDate").Editable = False
            oGrid.Columns.Item("U_Z_EmpCode").TitleObject.Caption = "Requester Code"
            oGrid.Columns.Item("U_Z_EmpCode").Editable = False
            oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Requester Name"
            oGrid.Columns.Item("U_Z_EmpName").Editable = False
            oGrid.Columns.Item("U_Z_DeptCode").TitleObject.Caption = "Department Code"
            oGrid.Columns.Item("U_Z_DeptCode").Editable = False
            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
            oGrid.Columns.Item("U_Z_DeptName").Editable = False
            oGrid.Columns.Item("U_Z_ReqClss").TitleObject.Caption = "Req.Classification"
            oGrid.Columns.Item("U_Z_ReqClss").Visible = False
            oGrid.Columns.Item("Position").TitleObject.Caption = "Position Name"
            oGrid.Columns.Item("Position").Editable = False
            oGrid.Columns.Item("U_Z_ExpMin").TitleObject.Caption = "Minimum Experience"
            oGrid.Columns.Item("U_Z_ExpMin").Editable = False
            oGrid.Columns.Item("U_Z_ExpMax").TitleObject.Caption = "Maximum Experience"
            oGrid.Columns.Item("U_Z_ExpMax").Editable = False
            oGrid.Columns.Item("U_Z_Vacancy").TitleObject.Caption = "Vacant positons"
            oGrid.Columns.Item("U_Z_Vacancy").Editable = False
            oGrid.Columns.Item("U_Z_MgrRemarks").TitleObject.Caption = "Manager Remarks"
            oGrid.Columns.Item("U_Z_MgrRemarks").Editable = False
            oGrid.Columns.Item("U_Z_MgrStatus").TitleObject.Caption = "Manager Status"
            oGrid.Columns.Item("U_Z_MgrStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_MgrStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("SA", "HOD Approved")
            ocombo.ValidValues.Add("SR", "HOD Rejected")
            ocombo.ValidValues.Add("HF", "HR Follow-UP")
            ocombo.ValidValues.Add("HA", "HR Approved")
            ocombo.ValidValues.Add("HR", "HR Rejected")
            ocombo.ValidValues.Add("C", "Closed")
            ocombo.ValidValues.Add("L", "Canceled")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_MgrStatus").Editable = False
            oGrid.Columns.Item("U_Z_HODStatus").TitleObject.Caption = "HOD Status"
            oGrid.Columns.Item("U_Z_HODStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_HODStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("SA", "HOD Approved")
            ocombo.ValidValues.Add("SR", "HOD Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_HODStatus").Editable = False
            oGrid.Columns.Item("U_Z_HODRemarks").TitleObject.Caption = "HOD Remarks"
            oGrid.Columns.Item("U_Z_HODRemarks").Editable = False
            oGrid.Columns.Item("U_Z_HRStatus").TitleObject.Caption = "HR Status"
            oGrid.Columns.Item("U_Z_HRStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_HRStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("HF", "HR Follow-UP")
            ocombo.ValidValues.Add("HA", "HR Approved")
            ocombo.ValidValues.Add("HR", "HR Rejected")
            ocombo.ValidValues.Add("C", "Closed")
            ocombo.ValidValues.Add("L", "Canceled")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_HRStatus").Editable = False
            oGrid.Columns.Item("U_Z_HRRemarks").TitleObject.Caption = "HR Remarks"
            oGrid.Columns.Item("U_Z_HRRemarks").Editable = False
            oGrid.Columns.Item("U_Z_EmpstDate").TitleObject.Caption = "Employment Start Date"
            oGrid.Columns.Item("U_Z_EmpstDate").Editable = False
            oGrid.Columns.Item("U_Z_IntAppDead").TitleObject.Caption = "Internal Application Deadline"
            oGrid.Columns.Item("U_Z_IntAppDead").Editable = False
            oGrid.Columns.Item("U_Z_ExtAppDead").TitleObject.Caption = "External Application Deadline"
            oGrid.Columns.Item("U_Z_ExtAppDead").Editable = False
        ElseIf strtitle = "RGM" Then
            oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Request Number"
            oGrid.Columns.Item("DocEntry").Editable = False
            oGrid.Columns.Item("U_Z_ReqDate").TitleObject.Caption = "Requested Date"
            oGrid.Columns.Item("U_Z_ReqDate").Editable = False
            oGrid.Columns.Item("U_Z_EmpCode").TitleObject.Caption = "Requester Code"
            oGrid.Columns.Item("U_Z_EmpCode").Editable = False
            oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Requester Name"
            oGrid.Columns.Item("U_Z_EmpName").Editable = False
            oGrid.Columns.Item("U_Z_DeptCode").TitleObject.Caption = "Department Code"
            oGrid.Columns.Item("U_Z_DeptCode").Editable = False
            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
            oGrid.Columns.Item("U_Z_DeptName").Editable = False
            oGrid.Columns.Item("U_Z_ReqClss").TitleObject.Caption = "Req.Classification"
            oGrid.Columns.Item("U_Z_ReqClss").Visible = False
            oGrid.Columns.Item("Position").TitleObject.Caption = "Position Name"
            oGrid.Columns.Item("Position").Editable = False
            oGrid.Columns.Item("U_Z_ExpMin").TitleObject.Caption = "Minimum Experience"
            oGrid.Columns.Item("U_Z_ExpMin").Editable = False
            oGrid.Columns.Item("U_Z_ExpMax").TitleObject.Caption = "Maximum Experience"
            oGrid.Columns.Item("U_Z_ExpMax").Editable = False
            oGrid.Columns.Item("U_Z_Vacancy").TitleObject.Caption = "Vacant positons"
            oGrid.Columns.Item("U_Z_Vacancy").Editable = False
            oGrid.Columns.Item("U_Z_MgrRemarks").TitleObject.Caption = "Manager Remarks"
            oGrid.Columns.Item("U_Z_MgrRemarks").Editable = False
            oGrid.Columns.Item("U_Z_MgrStatus").TitleObject.Caption = "Manager Status"
            oGrid.Columns.Item("U_Z_MgrStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_MgrStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("SA", "HOD Approved")
            ocombo.ValidValues.Add("SR", "HOD Rejected")
            ocombo.ValidValues.Add("HF", "HR Follow-UP")
            ocombo.ValidValues.Add("HA", "HR Approved")
            ocombo.ValidValues.Add("HR", "HR Rejected")
            ocombo.ValidValues.Add("C", "Closed")
            ocombo.ValidValues.Add("L", "Canceled")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_MgrStatus").Editable = False
            oGrid.Columns.Item("U_Z_HODStatus").TitleObject.Caption = "HOD Status"
            oGrid.Columns.Item("U_Z_HODStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_HODStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("SA", "HOD Approved")
            ocombo.ValidValues.Add("SR", "HOD Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_HODStatus").Editable = False
            oGrid.Columns.Item("U_Z_HODRemarks").TitleObject.Caption = "HOD Remarks"
            oGrid.Columns.Item("U_Z_HODRemarks").Editable = False
            oGrid.Columns.Item("U_Z_HRStatus").TitleObject.Caption = "HR Status"
            oGrid.Columns.Item("U_Z_HRStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_HRStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("HF", "HR Follow-UP")
            ocombo.ValidValues.Add("HA", "HR Approved")
            ocombo.ValidValues.Add("HR", "HR Rejected")
            ocombo.ValidValues.Add("C", "Closed")
            ocombo.ValidValues.Add("L", "Canceled")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_HRStatus").Editable = True
            oGrid.Columns.Item("U_Z_HRRemarks").TitleObject.Caption = "HR Remarks"
            oGrid.Columns.Item("U_Z_HRRemarks").Editable = True
            oGrid.Columns.Item("U_Z_EmpstDate").TitleObject.Caption = "Employment Start Date"
            oGrid.Columns.Item("U_Z_EmpstDate").Editable = True
            oGrid.Columns.Item("U_Z_IntAppDead").TitleObject.Caption = "Internal Application Deadline"
            oGrid.Columns.Item("U_Z_IntAppDead").Editable = True
            oGrid.Columns.Item("U_Z_ExtAppDead").TitleObject.Caption = "External Application Deadline"
            oGrid.Columns.Item("U_Z_ExtAppDead").Editable = True
        Else
            oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Request Number"
            oGrid.Columns.Item("DocEntry").Editable = False
            oGrid.Columns.Item("U_Z_ReqDate").TitleObject.Caption = "Requested Date"
            oGrid.Columns.Item("U_Z_ReqDate").Editable = False
            oGrid.Columns.Item("U_Z_EmpCode").TitleObject.Caption = "Requester Code"
            oGrid.Columns.Item("U_Z_EmpCode").Editable = False
            oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Requester Name"
            oGrid.Columns.Item("U_Z_EmpName").Editable = False
            oGrid.Columns.Item("U_Z_DeptCode").TitleObject.Caption = "Department Code"
            oGrid.Columns.Item("U_Z_DeptCode").Editable = False
            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
            oGrid.Columns.Item("U_Z_DeptName").Editable = False
            oGrid.Columns.Item("U_Z_ReqClss").TitleObject.Caption = "Req.Classification"
            oGrid.Columns.Item("U_Z_ReqClss").Visible = False
            oGrid.Columns.Item("Position").TitleObject.Caption = "Position Name"
            oGrid.Columns.Item("Position").Editable = False
            oGrid.Columns.Item("U_Z_ExpMin").TitleObject.Caption = "Minimum Experience"
            oGrid.Columns.Item("U_Z_ExpMin").Editable = False
            oGrid.Columns.Item("U_Z_ExpMax").TitleObject.Caption = "Maximum Experience"
            oGrid.Columns.Item("U_Z_ExpMax").Editable = False
            oGrid.Columns.Item("U_Z_Vacancy").TitleObject.Caption = "Vacant positons"
            oGrid.Columns.Item("U_Z_Vacancy").Editable = False
            oGrid.Columns.Item("U_Z_MgrRemarks").TitleObject.Caption = "Manager Remarks"
            oGrid.Columns.Item("U_Z_MgrRemarks").Editable = False
            oGrid.Columns.Item("U_Z_MgrStatus").TitleObject.Caption = "Manager Status"
            oGrid.Columns.Item("U_Z_MgrStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_MgrStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("SA", "HOD Approved")
            ocombo.ValidValues.Add("SR", "HOD Rejected")
            ocombo.ValidValues.Add("HF", "HR Follow-UP")
            ocombo.ValidValues.Add("HA", "HR Approved")
            ocombo.ValidValues.Add("HR", "HR Rejected")
            ocombo.ValidValues.Add("C", "Closed")
            ocombo.ValidValues.Add("L", "Canceled")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_MgrStatus").Editable = False
            oGrid.Columns.Item("U_Z_HODStatus").TitleObject.Caption = "HOD Status"
            oGrid.Columns.Item("U_Z_HODStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_HODStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("SA", "HOD Approved")
            ocombo.ValidValues.Add("SR", "HOD Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_HODStatus").Editable = True
            oGrid.Columns.Item("U_Z_HODRemarks").TitleObject.Caption = "HOD Remarks"
            oGrid.Columns.Item("U_Z_HODRemarks").Editable = True
            oGrid.Columns.Item("U_Z_HRStatus").TitleObject.Caption = "HR Status"
            oGrid.Columns.Item("U_Z_HRStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_HRStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("HF", "HR Follow-UP")
            ocombo.ValidValues.Add("HA", "HR Approved")
            ocombo.ValidValues.Add("HR", "HR Rejected")
            ocombo.ValidValues.Add("C", "Closed")
            ocombo.ValidValues.Add("L", "Canceled")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_HRStatus").Editable = False
            oGrid.Columns.Item("U_Z_HRRemarks").TitleObject.Caption = "HR Remarks"
            oGrid.Columns.Item("U_Z_HRRemarks").Editable = False
            oGrid.Columns.Item("U_Z_EmpstDate").TitleObject.Caption = "Employment Start Date"
            oGrid.Columns.Item("U_Z_EmpstDate").Editable = False
            oGrid.Columns.Item("U_Z_IntAppDead").TitleObject.Caption = "Internal Application Deadline"
            oGrid.Columns.Item("U_Z_IntAppDead").Editable = False
            oGrid.Columns.Item("U_Z_ExtAppDead").TitleObject.Caption = "External Application Deadline"
            oGrid.Columns.Item("U_Z_ExtAppDead").Editable = False
        End If

        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub

    Private Sub Databind2(ByVal strempid As String, ByVal strtitle As String)
        Dim strqry As String
        oGrid = oForm.Items.Item("11").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_1")
        If strempid <> "" And strtitle = "MPR" Then
            strqry = "select U_Z_MgrStatus,DocEntry,U_Z_ReqDate,U_Z_EmpCode,U_Z_EmpName,U_Z_DeptCode,U_Z_DeptName,"
            strqry = strqry & " case U_Z_ReqClss when 'E' then 'Existing' else 'New Position' end as 'U_Z_ReqClss',isnull(U_Z_PosName,'') +''+ isnull(U_Z_NewPosi,'') as Position,U_Z_ExpMin,U_Z_ExpMax,"
            strqry = strqry & " U_Z_Vacancy,U_Z_MgrRemarks,U_Z_HODStatus,U_Z_HODRemarks,U_Z_HRStatus"
            strqry = strqry & "  ,U_Z_HRRemarks,U_Z_EmpstDate,U_Z_IntAppDead,U_Z_ExtAppDead from [@Z_HR_ORMPREQ] where U_Z_EmpCode='" & strempid & "'"
        ElseIf strtitle = "RHR" Then
            strqry = "select U_Z_HODStatus, DocEntry,U_Z_ReqDate,U_Z_EmpCode,U_Z_EmpName,U_Z_DeptCode,U_Z_DeptName,"
            strqry = strqry & " case U_Z_ReqClss when 'E' then 'Existing' else 'New Position' end as 'U_Z_ReqClss',isnull(U_Z_PosName,'') +''+ isnull(U_Z_NewPosi,'') as Position,U_Z_ExpMin,U_Z_ExpMax,"
            strqry = strqry & " U_Z_Vacancy,U_Z_MgrStatus,U_Z_MgrRemarks,U_Z_HRStatus,U_Z_HRRemarks,"
            strqry = strqry & " U_Z_HODRemarks,U_Z_EmpstDate,U_Z_IntAppDead,U_Z_ExtAppDead  from [@Z_HR_ORMPREQ] "
        ElseIf strtitle = "RGM" Then
            strqry = "select U_Z_HRStatus, DocEntry,U_Z_ReqDate,U_Z_EmpCode,U_Z_EmpName,U_Z_DeptCode,U_Z_DeptName,"
            strqry = strqry & " case U_Z_ReqClss when 'E' then 'Existing' else 'New Position' end as 'U_Z_ReqClss',isnull(U_Z_PosName,'') +''+ isnull(U_Z_NewPosi,'') as Position,U_Z_ExpMin,U_Z_ExpMax,"
            strqry = strqry & " U_Z_Vacancy,U_Z_MgrStatus,U_Z_MgrRemarks,U_Z_HODStatus"
            strqry = strqry & " ,U_Z_HODRemarks,U_Z_HRRemarks,U_Z_EmpstDate,U_Z_IntAppDead,U_Z_ExtAppDead  from [@Z_HR_ORMPREQ] "
        End If
        oGrid.DataTable.ExecuteQuery(strqry)
        oEditTextColumn = oGrid.Columns.Item("DocEntry")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Invoice
        If strtitle = "MPR" Then
            oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Request Number"
            oGrid.Columns.Item("DocEntry").Editable = False
            oGrid.Columns.Item("U_Z_ReqDate").TitleObject.Caption = "Requested Date"
            oGrid.Columns.Item("U_Z_ReqDate").Editable = False
            oGrid.Columns.Item("U_Z_EmpCode").TitleObject.Caption = "Requester Code"
            oGrid.Columns.Item("U_Z_EmpCode").Editable = False
            oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Requester Name"
            oGrid.Columns.Item("U_Z_EmpName").Editable = False
            oGrid.Columns.Item("U_Z_DeptCode").TitleObject.Caption = "Department Code"
            oGrid.Columns.Item("U_Z_DeptCode").Editable = False
            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
            oGrid.Columns.Item("U_Z_DeptName").Editable = False
            oGrid.Columns.Item("U_Z_ReqClss").TitleObject.Caption = "Req.Classification"
            oGrid.Columns.Item("U_Z_ReqClss").Visible = False
            oGrid.Columns.Item("Position").TitleObject.Caption = "Position Name"
            oGrid.Columns.Item("Position").Editable = False
            oGrid.Columns.Item("U_Z_ExpMin").TitleObject.Caption = "Minimum Experience"
            oGrid.Columns.Item("U_Z_ExpMin").Editable = False
            oGrid.Columns.Item("U_Z_ExpMax").TitleObject.Caption = "Maximum Experience"
            oGrid.Columns.Item("U_Z_ExpMax").Editable = False
            oGrid.Columns.Item("U_Z_Vacancy").TitleObject.Caption = "Vacant positons"
            oGrid.Columns.Item("U_Z_Vacancy").Editable = False
            oGrid.Columns.Item("U_Z_MgrRemarks").TitleObject.Caption = "Manager Remarks"
            oGrid.Columns.Item("U_Z_MgrRemarks").Editable = False
            oGrid.Columns.Item("U_Z_MgrStatus").TitleObject.Caption = "Manager Status"
            oGrid.Columns.Item("U_Z_MgrStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_MgrStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("SA", "HOD Approved")
            ocombo.ValidValues.Add("SR", "HOD Rejected")
            ocombo.ValidValues.Add("HF", "HR Follow-UP")
            ocombo.ValidValues.Add("HA", "HR Approved")
            ocombo.ValidValues.Add("HR", "HR Rejected")
            ocombo.ValidValues.Add("C", "Closed")
            ocombo.ValidValues.Add("L", "Canceled")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_MgrStatus").Editable = False
            oGrid.Columns.Item("U_Z_HODStatus").TitleObject.Caption = "HOD Status"
            oGrid.Columns.Item("U_Z_HODStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_HODStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("SA", "HOD Approved")
            ocombo.ValidValues.Add("SR", "HOD Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_HODStatus").Editable = False
            oGrid.Columns.Item("U_Z_HODRemarks").TitleObject.Caption = "HOD Remarks"
            oGrid.Columns.Item("U_Z_HODRemarks").Editable = False
            oGrid.Columns.Item("U_Z_HRStatus").TitleObject.Caption = "HR Status"
            oGrid.Columns.Item("U_Z_HRStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_HRStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("HF", "HR Follow-UP")
            ocombo.ValidValues.Add("HA", "HR Approved")
            ocombo.ValidValues.Add("HR", "HR Rejected")
            ocombo.ValidValues.Add("C", "Closed")
            ocombo.ValidValues.Add("L", "Canceled")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_HRStatus").Editable = False
            oGrid.Columns.Item("U_Z_HRRemarks").TitleObject.Caption = "HR Remarks"
            oGrid.Columns.Item("U_Z_HRRemarks").Editable = False
            oGrid.Columns.Item("U_Z_EmpstDate").TitleObject.Caption = "Employment Start Date"
            oGrid.Columns.Item("U_Z_EmpstDate").Editable = False
            oGrid.Columns.Item("U_Z_IntAppDead").TitleObject.Caption = "Internal Application Deadline"
            oGrid.Columns.Item("U_Z_IntAppDead").Editable = False
            oGrid.Columns.Item("U_Z_ExtAppDead").TitleObject.Caption = "External Application Deadline"
            oGrid.Columns.Item("U_Z_ExtAppDead").Editable = False
        ElseIf strtitle = "RGM" Then
            oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Request Number"
            oGrid.Columns.Item("DocEntry").Editable = False
            oGrid.Columns.Item("U_Z_ReqDate").TitleObject.Caption = "Requested Date"
            oGrid.Columns.Item("U_Z_ReqDate").Editable = False
            oGrid.Columns.Item("U_Z_EmpCode").TitleObject.Caption = "Requester Code"
            oGrid.Columns.Item("U_Z_EmpCode").Editable = False
            oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Requester Name"
            oGrid.Columns.Item("U_Z_EmpName").Editable = False
            oGrid.Columns.Item("U_Z_DeptCode").TitleObject.Caption = "Department Code"
            oGrid.Columns.Item("U_Z_DeptCode").Editable = False
            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
            oGrid.Columns.Item("U_Z_DeptName").Editable = False
            oGrid.Columns.Item("U_Z_ReqClss").TitleObject.Caption = "Req.Classification"
            oGrid.Columns.Item("U_Z_ReqClss").Visible = False
            oGrid.Columns.Item("Position").TitleObject.Caption = "Position Name"
            oGrid.Columns.Item("Position").Editable = False
            oGrid.Columns.Item("U_Z_ExpMin").TitleObject.Caption = "Minimum Experience"
            oGrid.Columns.Item("U_Z_ExpMin").Editable = False
            oGrid.Columns.Item("U_Z_ExpMax").TitleObject.Caption = "Maximum Experience"
            oGrid.Columns.Item("U_Z_ExpMax").Editable = False
            oGrid.Columns.Item("U_Z_Vacancy").TitleObject.Caption = "Vacant positons"
            oGrid.Columns.Item("U_Z_Vacancy").Editable = False
            oGrid.Columns.Item("U_Z_MgrRemarks").TitleObject.Caption = "Manager Remarks"
            oGrid.Columns.Item("U_Z_MgrRemarks").Editable = False
            oGrid.Columns.Item("U_Z_MgrStatus").TitleObject.Caption = "Manager Status"
            oGrid.Columns.Item("U_Z_MgrStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_MgrStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("SA", "HOD Approved")
            ocombo.ValidValues.Add("SR", "HOD Rejected")
            ocombo.ValidValues.Add("HF", "HR Follow-UP")
            ocombo.ValidValues.Add("HA", "HR Approved")
            ocombo.ValidValues.Add("HR", "HR Rejected")
            ocombo.ValidValues.Add("C", "Closed")
            ocombo.ValidValues.Add("L", "Canceled")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_MgrStatus").Editable = False
            oGrid.Columns.Item("U_Z_HODStatus").TitleObject.Caption = "HOD Status"
            oGrid.Columns.Item("U_Z_HODStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_HODStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("SA", "HOD Approved")
            ocombo.ValidValues.Add("SR", "HOD Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_HODStatus").Editable = False
            oGrid.Columns.Item("U_Z_HODRemarks").TitleObject.Caption = "HOD Remarks"
            oGrid.Columns.Item("U_Z_HODRemarks").Editable = False
            oGrid.Columns.Item("U_Z_HRStatus").TitleObject.Caption = "HR Status"
            oGrid.Columns.Item("U_Z_HRStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_HRStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("HF", "HR Follow-UP")
            ocombo.ValidValues.Add("HA", "HR Approved")
            ocombo.ValidValues.Add("HR", "HR Rejected")
            ocombo.ValidValues.Add("C", "Closed")
            ocombo.ValidValues.Add("L", "Canceled")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_HRStatus").Editable = False
            oGrid.Columns.Item("U_Z_HRRemarks").TitleObject.Caption = "HR Remarks"
            oGrid.Columns.Item("U_Z_HRRemarks").Editable = False
            oGrid.Columns.Item("U_Z_EmpstDate").TitleObject.Caption = "Employment Start Date"
            oGrid.Columns.Item("U_Z_EmpstDate").Editable = False
            oGrid.Columns.Item("U_Z_IntAppDead").TitleObject.Caption = "Internal Application Deadline"
            oGrid.Columns.Item("U_Z_IntAppDead").Editable = False
            oGrid.Columns.Item("U_Z_ExtAppDead").TitleObject.Caption = "External Application Deadline"
            oGrid.Columns.Item("U_Z_ExtAppDead").Editable = False
        Else
            oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Request Number"
            oGrid.Columns.Item("DocEntry").Editable = False
            oGrid.Columns.Item("U_Z_ReqDate").TitleObject.Caption = "Requested Date"
            oGrid.Columns.Item("U_Z_ReqDate").Editable = False
            oGrid.Columns.Item("U_Z_EmpCode").TitleObject.Caption = "Requester Code"
            oGrid.Columns.Item("U_Z_EmpCode").Editable = False
            oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Requester Name"
            oGrid.Columns.Item("U_Z_EmpName").Editable = False
            oGrid.Columns.Item("U_Z_DeptCode").TitleObject.Caption = "Department Code"
            oGrid.Columns.Item("U_Z_DeptCode").Editable = False
            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
            oGrid.Columns.Item("U_Z_DeptName").Editable = False
            oGrid.Columns.Item("U_Z_ReqClss").TitleObject.Caption = "Req.Classification"
            oGrid.Columns.Item("U_Z_ReqClss").Visible = False
            oGrid.Columns.Item("Position").TitleObject.Caption = "Position Name"
            oGrid.Columns.Item("Position").Editable = False
            oGrid.Columns.Item("U_Z_ExpMin").TitleObject.Caption = "Minimum Experience"
            oGrid.Columns.Item("U_Z_ExpMin").Editable = False
            oGrid.Columns.Item("U_Z_ExpMax").TitleObject.Caption = "Maximum Experience"
            oGrid.Columns.Item("U_Z_ExpMax").Editable = False
            oGrid.Columns.Item("U_Z_Vacancy").TitleObject.Caption = "Vacant positons"
            oGrid.Columns.Item("U_Z_Vacancy").Editable = False
            oGrid.Columns.Item("U_Z_MgrRemarks").TitleObject.Caption = "Manager Remarks"
            oGrid.Columns.Item("U_Z_MgrRemarks").Editable = False
            oGrid.Columns.Item("U_Z_MgrStatus").TitleObject.Caption = "Manager Status"
            oGrid.Columns.Item("U_Z_MgrStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_MgrStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("SA", "HOD Approved")
            ocombo.ValidValues.Add("SR", "HOD Rejected")
            ocombo.ValidValues.Add("HF", "HR Follow-UP")
            ocombo.ValidValues.Add("HA", "HR Approved")
            ocombo.ValidValues.Add("HR", "HR Rejected")
            ocombo.ValidValues.Add("C", "Closed")
            ocombo.ValidValues.Add("L", "Canceled")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_MgrStatus").Editable = False
            oGrid.Columns.Item("U_Z_HODStatus").TitleObject.Caption = "HOD Status"
            oGrid.Columns.Item("U_Z_HODStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_HODStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("SA", "HOD Approved")
            ocombo.ValidValues.Add("SR", "HOD Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_HODStatus").Editable = False
            oGrid.Columns.Item("U_Z_HODRemarks").TitleObject.Caption = "HOD Remarks"
            oGrid.Columns.Item("U_Z_HODRemarks").Editable = False
            oGrid.Columns.Item("U_Z_HRStatus").TitleObject.Caption = "HR Status"
            oGrid.Columns.Item("U_Z_HRStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_HRStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("HF", "HR Follow-UP")
            ocombo.ValidValues.Add("HA", "HR Approved")
            ocombo.ValidValues.Add("HR", "HR Rejected")
            ocombo.ValidValues.Add("C", "Closed")
            ocombo.ValidValues.Add("L", "Canceled")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_HRStatus").Editable = False
            oGrid.Columns.Item("U_Z_HRRemarks").TitleObject.Caption = "HR Remarks"
            oGrid.Columns.Item("U_Z_HRRemarks").Editable = False
            oGrid.Columns.Item("U_Z_EmpstDate").TitleObject.Caption = "Employment Start Date"
            oGrid.Columns.Item("U_Z_EmpstDate").Editable = False
            oGrid.Columns.Item("U_Z_IntAppDead").TitleObject.Caption = "Internal Application Deadline"
            oGrid.Columns.Item("U_Z_IntAppDead").Editable = False
            oGrid.Columns.Item("U_Z_ExtAppDead").TitleObject.Caption = "External Application Deadline"
            oGrid.Columns.Item("U_Z_ExtAppDead").Editable = False
        End If
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oGrid.CollapseLevel = 1
    End Sub

#Region "AddToUDT"
    Private Function AddToUDT(ByVal aForm As SAPbouiCOM.Form, ByVal strtitle As String) As Boolean
        oForm.Freeze(True)
        Dim strTable, strEmpId, strcode, strqry As String
        Dim dt As Date
        Dim oValidateRS, otemprs As SAPbobsCOM.Recordset
        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        dt = Now.Date
        If oApplication.Company.InTransaction() Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        oApplication.Company.StartTransaction()
        oGrid = aForm.Items.Item("3").Specific
        If strtitle = "Head of Department Recruitment Approval" Then
            Try
                For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                    strEmpId = oGrid.DataTable.GetValue("U_Z_EmpCode", intRow)
                    strcode = oGrid.DataTable.GetValue("DocEntry", intRow)
                    strqry = "Update [@Z_HR_ORMPREQ] set  U_Z_HODStatus='" & oGrid.DataTable.GetValue("U_Z_HODStatus", intRow) & "',U_Z_HODRemarks='" & oGrid.DataTable.GetValue("U_Z_HODRemarks", intRow) & "',U_Z_MgrStatus='" & oGrid.DataTable.GetValue("U_Z_HODStatus", intRow) & "' where DocEntry='" & strcode & "'"
                    oValidateRS.DoQuery(strqry)
                Next
                If oApplication.Company.InTransaction() Then
                    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                End If
                oForm.Freeze(False)
                Return True
            Catch ex As Exception
                oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                If oApplication.Company.InTransaction() Then
                    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                End If
                Return False
            End Try
        ElseIf strtitle = "HR Recruitment Approval" Then
            Try
                For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                    strEmpId = oGrid.DataTable.GetValue("U_Z_EmpCode", intRow)
                    strcode = oGrid.DataTable.GetValue("DocEntry", intRow)
                    strqry = "Update [@Z_HR_ORMPREQ] set  U_Z_HRStatus='" & oGrid.DataTable.GetValue("U_Z_HRStatus", intRow) & "',U_Z_HRRemarks='" & oGrid.DataTable.GetValue("U_Z_HRRemarks", intRow) & "',"
                    strqry = strqry & " U_Z_MgrStatus='" & oGrid.DataTable.GetValue("U_Z_HRStatus", intRow) & "',U_Z_EmpstDate='" & oGrid.DataTable.GetValue("U_Z_EmpstDate", intRow) & "',U_Z_IntAppDead='" & oGrid.DataTable.GetValue("U_Z_IntAppDead", intRow) & "',"
                    strqry = strqry & " U_Z_ExtAppDead='" & oGrid.DataTable.GetValue("U_Z_ExtAppDead", intRow) & "' where DocEntry='" & strcode & "'"
                    oValidateRS.DoQuery(strqry)
                Next
                If oApplication.Company.InTransaction() Then
                    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                End If
                oForm.Freeze(False)
                Return True
            Catch ex As Exception
                oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                If oApplication.Company.InTransaction() Then
                    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                End If
                Return False
            End Try
        End If
        Return True
    End Function
#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_RecApproval Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim strCode, strHRstatus, strGMstatus, empcode, empname As String
                                If (pVal.ItemUID = "3" Or pVal.ItemUID = "11") And pVal.ColUID = "DocEntry" Then
                                    If pVal.ItemUID = "3" Then
                                        oGrid = oForm.Items.Item("3").Specific
                                    ElseIf pVal.ItemUID = "11" Then
                                        oGrid = oForm.Items.Item("11").Specific
                                    End If
                                    For intRow As Integer = pVal.Row To pVal.Row
                                        If 1 = 1 Then
                                            strCode = oGrid.DataTable.GetValue("DocEntry", intRow)
                                            strHRstatus = oGrid.DataTable.GetValue("U_Z_HODStatus", intRow)
                                            empcode = oApplication.Utilities.getEdittextvalue(oForm, "5")
                                            empname = oApplication.Utilities.getEdittextvalue(oForm, "7")
                                            If oForm.Title = "Manager Review Status" And strHRstatus = "O" Then
                                                Dim objct As New clshrMPRequest
                                                objct.LoadForm1(strCode, oForm.Title, empcode, empname, strHRstatus)
                                            ElseIf oForm.Title = "HR Recruitment Approval" Then
                                                Dim objct As New clshrMPRequest
                                                objct.LoadForm1(strCode, oForm.Title, empcode, empname, strHRstatus)
                                            ElseIf oForm.Title = "Head of Department Recruitment Approval" Then ' And strHRstatus <> "Closed" Then
                                                strGMstatus = oGrid.DataTable.GetValue("U_Z_HRStatus", intRow)
                                                Dim objct As New clshrMPRequest
                                                objct.LoadForm1(strCode, oForm.Title, empcode, empname, strGMstatus)
                                            ElseIf oForm.Title = "Manager Review Status" Then ' And strHRstatus <> "Open" Then
                                                Dim objct As New clshrMPRequest
                                                objct.LoadForm1(strCode, oForm.Title, empcode, empname, strHRstatus)
                                            Else
                                                oApplication.Utilities.Message("Your request is processing", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            End If
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
                                '  oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems))
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim strCode, strHRstatus, strGMstatus, empcode, empname As String
                                If pVal.ItemUID = "8" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 1
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "9" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 2
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "12" Then
                                    If oApplication.SBO_Application.MessageBox("Do you want confirm the Recruitment Approval", , "Yes", "No") = 2 Then
                                        Exit Sub
                                    Else
                                        If AddToUDT(oForm, oForm.Title) = True Then
                                            oApplication.Utilities.Message(oForm.Title & "successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                            oForm.Close()
                                        Else
                                            BubbleEvent = False
                                            Exit Sub
                                        End If

                                    End If
                                End If
                                If pVal.ItemUID = "4" Then
                                    oGrid = oForm.Items.Item("3").Specific
                                    For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                        If oGrid.Rows.IsSelected(intRow) Then
                                            strCode = oGrid.DataTable.GetValue("DocEntry", intRow)
                                            strHRstatus = oGrid.DataTable.GetValue("U_Z_HODStatus", intRow)
                                            empcode = oApplication.Utilities.getEdittextvalue(oForm, "5")
                                            empname = oApplication.Utilities.getEdittextvalue(oForm, "7")
                                            If oForm.Title = "Manager Review Status" And strHRstatus = "Open" Then
                                                Dim objct As New clshrMPRequest
                                                objct.LoadForm1(strCode, oForm.Title, empcode, empname, strHRstatus)
                                            ElseIf oForm.Title = "HR Recruitment Approval" And strHRstatus <> "Open" Then
                                                Dim objct As New clshrMPRequest
                                                objct.LoadForm1(strCode, oForm.Title, empcode, empname, strHRstatus)
                                            ElseIf oForm.Title = "Head of Department Recruitment Approval" Then ' And strHRstatus <> "Closed" Then
                                                strGMstatus = oGrid.DataTable.GetValue("U_Z_HRStatus", intRow)
                                                Dim objct As New clshrMPRequest
                                                objct.LoadForm1(strCode, oForm.Title, empcode, empname, strGMstatus)
                                            ElseIf oForm.Title = "Manager Review Status" Then ' And strHRstatus <> "Open" Then
                                                Dim objct As New clshrMPRequest
                                                objct.LoadForm1(strCode, oForm.Title, empcode, empname, strHRstatus)
                                            Else
                                                oApplication.Utilities.Message("Your request is processing", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                                Exit Sub
                                            End If
                                            'Else
                                            '    oApplication.Utilities.Message("No rows selected", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            '    Exit Sub
                                        End If
                                    Next
                                End If
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
                Case mnu_InvSO
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

End Class
