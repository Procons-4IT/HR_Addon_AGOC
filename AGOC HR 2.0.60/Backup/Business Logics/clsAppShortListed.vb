Public Class clsAppShortListed
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
    Private Shared strFunction As String

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub LoadForm(ByVal strtitle As String, ByVal strReqNo As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_AppShortListed) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_AppShortListed, frm_hr_AppShortListed)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.DataSources.DataTables.Add("DT_0")

        strFunction = strtitle
        If strtitle = "LM" Then
            oForm.Title = "Shortlisting First Level Approval"
        ElseIf strtitle = "SM" Then
            oForm.Title = "Shortlisting Second Level Approval"
        ElseIf strtitle = "IPOA" Then
            oForm.Title = "HR Offer Acceptance"
        End If

        oForm.Freeze(True)
        Dim oCFLs As SAPbouiCOM.ChooseFromListCollection
        oCFLs = oForm.ChooseFromLists
        Dim oCFL As SAPbouiCOM.ChooseFromList
        Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams
        oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
        oCFLCreationParams.MultiSelection = False
        oCFLCreationParams.UniqueID = "UDCFL4"
        oCFLCreationParams.ObjectType = "Z_HR_OOREJ"

        oCFL = oCFLs.Add(oCFLCreationParams)
        Databind(strtitle, strReqNo)


        oForm.Freeze(False)
    End Sub

    Private Sub Databind(ByVal strtitle As String, ByVal strReqNo As String)
        Dim strqry As String
        oGrid = oForm.Items.Item("3").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
        Dim oUserID As String = oApplication.Company.UserName
        Dim stremp As String = oApplication.Utilities.getEmpIDforMangers(oUserID)

        If strtitle = "LM" Then
            strqry = "Select T0.U_Z_ReqNo, T2.U_Z_MgrStatus As 'ReqStatus',T0.DocEntry,T0.U_Z_HRAppID as 'App/ID',U_Z_HRAppName,T0.U_Z_DeptName,T1.U_Z_PosName,T3.U_Z_Status"
            'strqry = strqry & "(Case T2.U_Z_MgrStatus When 'O' Then 'OPEN' When 'SA' Then 'HOD Approved' When  'SR' Then 'HOD Rejected' When 'C' Then 'Closed' When 'L' Then 'Canceled' When 'HF' Then 'HR Follow-Up' When 'HA' Then 'HR Approved' When 'HR' Then 'HR Rejected' END) As 'Req Status'"
            strqry = strqry & ",t0.U_Z_Dob, T0.U_Z_Mobile ,"
            strqry = strqry & " U_Z_Email,T0.U_Z_YrExp,T0.U_Z_AppDate, T0.U_Z_Skills,(Case ISNULL(T0.U_Z_SMgrStatus,'-') When 'A' Then 'Approved' When 'R' Then 'Rejected' Else 'Pending' End) As U_Z_SMgrStatus,ISNULL(T0.U_Z_MgrStatus,'-') As U_Z_MgrStatus, T0.U_Z_MgrRemarks,U_Z_Finished from "
            strqry = strqry & " [@Z_HR_OHEM1] T0 Left Outer Join [@Z_HR_CRAPP6] T1 on T0.DocEntry = T1.DocEntry "
            strqry = strqry & " Join [@Z_HR_ORMPREQ] T2 On T2.DocEntry = T0.U_Z_ReqNo JOIN [@Z_HR_OCRAPP] T3 On T3.DocEntry = T0.U_Z_HRAppID"
            'strqry = strqry & " And T2.U_Z_EmpCode In (Select EmpID From OHEM Where Manager = (Select empID From OHEM Where UserId = " & oApplication.Company.UserSignature & ")) "
            strqry = strqry & " Where (U_Z_Status = 'I' or U_Z_Status = 'S') And U_Z_ReqNo = '" & strReqNo & "'"
        ElseIf strtitle = "SM" Then
            strqry = "Select  T0.U_Z_ReqNo, T2.U_Z_MgrStatus As 'ReqStatus',T0.DocEntry,T0.U_Z_HRAppID as 'App/ID',U_Z_HRAppName,T0.U_Z_DeptName,T1.U_Z_PosName,T3.U_Z_Status "
            'strqry = strqry & "(Case T2.U_Z_MgrStatus When 'O' Then 'OPEN' When 'SA' Then 'HOD Approved' When  'SR' Then 'HOD Rejected' When 'C' Then 'Closed' When 'L' Then 'Canceled' When 'HF' Then 'HR Follow-Up' When 'HA' Then 'HR Approved' When 'HR' Then 'HR Rejected' END) As 'Req Status'"
            strqry = strqry & ",t0.U_Z_Dob, T0.U_Z_Mobile ,"
            strqry = strqry & " U_Z_Email,T0.U_Z_YrExp,T0.U_Z_AppDate,T0.U_Z_Skills,(Case ISNULL(T0.U_Z_MgrStatus,'-') When 'A' Then 'Approved' When 'R' Then 'Rejected' Else 'Pending' End) As U_Z_MgrStatus,ISNULL(T0.U_Z_SMgrStatus,'-') As U_Z_SMgrStatus, T0.U_Z_SMgrRemarks,U_Z_Finished from "
            strqry = strqry & " [@Z_HR_OHEM1] T0 Left Outer Join [@Z_HR_CRAPP6] T1 on T0.DocEntry = T1.DocEntry "
            strqry = strqry & " Join [@Z_HR_ORMPREQ] T2 On T2.DocEntry = T0.U_Z_ReqNo  JOIN [@Z_HR_OCRAPP] T3 On T3.DocEntry = T0.U_Z_HRAppID"
            'strqry = strqry & " And  T2.U_Z_EmpCode In (Select EmpID From OHEM Where Manager = (Select EmpID From OHEM Where Manager = (Select empID From OHEM Where UserId = " & oApplication.Company.UserSignature & "))"
            'strqry = strqry & " Union Select EmpID From OHEM Where Manager = (Select empID From OHEM Where UserId = " & oApplication.Company.UserSignature & ")) "
            strqry = strqry & " Where (U_Z_Status = 'S' OR U_Z_Status = 'F') And U_Z_ReqNo = '" & strReqNo & "'"
        ElseIf strtitle = "IPOA" Then
            strqry = "Select T0.DocEntry,T0.U_Z_HRAppID as 'App/ID',U_Z_HRAppName,T0.U_Z_DeptName,T1.U_Z_PosName,T0.U_Z_Status,T0.U_Z_Mobile ,"
            strqry = strqry & " U_Z_YrExp,U_Z_Skills,ISNULL(U_Z_OAStatus,'-') As U_Z_OAStatus,U_Z_OARemarks,U_Z_RejRsn as 'Reject Reason' from  "
            strqry = strqry & "  [@Z_HR_OHEM1] T0 inner join [@Z_HR_CRAPP6] T1 on T0.DocEntry = T1.DocEntry where U_Z_ApplStatus = 'S' "
            strqry = strqry & " And  U_Z_ReqNo = '" & strReqNo & "'"
        End If

        oGrid.DataTable.ExecuteQuery(strqry)
        oEditTextColumn = oGrid.Columns.Item("DocEntry")
        'oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Invoice

        If strtitle = "LM" Then
            oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Applicant ID"
            oGrid.Columns.Item("DocEntry").Visible = False
            oGrid.Columns.Item("App/ID").TitleObject.Caption = "Applicant ID"
            oEditTextColumn = oGrid.Columns.Item("App/ID")
            oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
            oGrid.Columns.Item("U_Z_HRAppName").TitleObject.Caption = "First Name"
            oGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Applicant Status"
            oGrid.Columns.Item("U_Z_Status").Editable = False
            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
            oGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position Name"
            oGrid.Columns.Item("U_Z_ReqNo").TitleObject.Caption = "Recruitment No"
            oEditTextColumn = oGrid.Columns.Item("U_Z_ReqNo")
            oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
            oGrid.Columns.Item("ReqStatus").TitleObject.Caption = "Recruitment Status"
            oGrid.Columns.Item("U_Z_Dob").TitleObject.Caption = "Date of Birth"
            oGrid.Columns.Item("U_Z_Mobile").TitleObject.Caption = "Mobile"
            oGrid.Columns.Item("U_Z_Email").TitleObject.Caption = "Email ID"
            oGrid.Columns.Item("U_Z_YrExp").TitleObject.Caption = "Year Of Experience"
            oGrid.Columns.Item("U_Z_AppDate").TitleObject.Caption = "Applicant Date"
            oGrid.Columns.Item("U_Z_Skills").TitleObject.Caption = "Skills"
            oGrid.Columns.Item("U_Z_MgrStatus").TitleObject.Caption = "First Level Approval Status"
            oGrid.Columns.Item("U_Z_SMgrStatus").TitleObject.Caption = "Second Level Approval Status"
            oGrid.Columns.Item("U_Z_Finished").TitleObject.Caption = "Work Flow Status"
            oGrid.Columns.Item("U_Z_Finished").Visible = False

            oGrid.Columns.Item("U_Z_Status").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_Status")
            ocombo.ValidValues.Add("R", "Received")
            ocombo.ValidValues.Add("S", "Shortlisted")
            ocombo.ValidValues.Add("F", "Shortlisted 1st Level")
            ocombo.ValidValues.Add("N", "Shortlisted 2st Level")
            ocombo.ValidValues.Add("L", "Canceled")
            ocombo.ValidValues.Add("I", "Interview")
            ocombo.ValidValues.Add("D", "Interview 1st Approval")
            ocombo.ValidValues.Add("M", "Interview HR Approval")
            ocombo.ValidValues.Add("O", "Job Offered")
            ocombo.ValidValues.Add("J", "Offer Rejected")
            ocombo.ValidValues.Add("C", "Cancelled")
            ocombo.ValidValues.Add("A", "Offer Accepted")
            ocombo.ValidValues.Add("H", "Hired")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

            oGrid.Columns.Item("ReqStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("ReqStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("SA", "HOD Approved")
            ocombo.ValidValues.Add("SR", "HOD Rejected")
            ocombo.ValidValues.Add("C", "Closed")
            ocombo.ValidValues.Add("L", "Canceled")
            ocombo.ValidValues.Add("HF", "HR Follow-Up")
            ocombo.ValidValues.Add("HA", "HR Approved")
            ocombo.ValidValues.Add("HR", "HR Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

            oGrid.Columns.Item("U_Z_MgrStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_MgrStatus")
            ocombo.ValidValues.Add("-", "Pending")
            ocombo.ValidValues.Add("A", "Approved")
            ocombo.ValidValues.Add("R", "Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

            oGrid.Columns.Item("U_Z_MgrRemarks").TitleObject.Caption = "First Level Approval Remarks"

            oGrid.Columns.Item("DocEntry").Editable = False
            oGrid.Columns.Item("App/ID").Editable = False
            oGrid.Columns.Item("U_Z_HRAppName").Editable = False
            oGrid.Columns.Item("U_Z_DeptName").Editable = False
            oGrid.Columns.Item("U_Z_PosName").Editable = False
            oGrid.Columns.Item("U_Z_ReqNo").Editable = False
            oGrid.Columns.Item("ReqStatus").Editable = False
            oGrid.Columns.Item("U_Z_Dob").Editable = False
            oGrid.Columns.Item("U_Z_Mobile").Visible = False
            oGrid.Columns.Item("U_Z_Email").Editable = False
            oGrid.Columns.Item("U_Z_YrExp").Editable = False
            oGrid.Columns.Item("U_Z_AppDate").Editable = False
            oGrid.Columns.Item("U_Z_Skills").Editable = False
            oGrid.Columns.Item("U_Z_SMgrStatus").Editable = False
            oGrid.Columns.Item("U_Z_MgrStatus").Editable = True
            oGrid.Columns.Item("U_Z_MgrRemarks").Editable = True
            oGrid.Columns.Item("U_Z_Finished").Editable = False


            If oGrid.DataTable.Rows.Count > 0 Then
                Dim strStatus As String = oGrid.DataTable.GetValue("ReqStatus", 0)
                If strStatus = "C" Or strStatus = "L" Then
                    oForm.Items.Item("_1").Enabled = False
                End If
            End If


        ElseIf strtitle = "SM" Then
            oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Applicant ID"
            oGrid.Columns.Item("DocEntry").Visible = False
            oGrid.Columns.Item("App/ID").TitleObject.Caption = "Applicant ID"
            oEditTextColumn = oGrid.Columns.Item("App/ID")
            oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
            oGrid.Columns.Item("U_Z_HRAppName").TitleObject.Caption = "First Name"
            oGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Applicant Status"
            oGrid.Columns.Item("U_Z_Status").Editable = False
            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
            oGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position Name"
            oGrid.Columns.Item("U_Z_ReqNo").TitleObject.Caption = "Recruitment No"
            oEditTextColumn = oGrid.Columns.Item("U_Z_ReqNo")
            oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
            oGrid.Columns.Item("ReqStatus").TitleObject.Caption = "Recruitment Status"
            oGrid.Columns.Item("U_Z_MgrStatus").TitleObject.Caption = "Manager Recruitment Status"
            oGrid.Columns.Item("U_Z_Dob").TitleObject.Caption = "Date of Birth"
            oGrid.Columns.Item("U_Z_Mobile").TitleObject.Caption = "Mobile"
            oGrid.Columns.Item("U_Z_Email").TitleObject.Caption = "Email ID"
            oGrid.Columns.Item("U_Z_YrExp").TitleObject.Caption = "Year Of Experience"
            oGrid.Columns.Item("U_Z_AppDate").TitleObject.Caption = "Applicant Date"
            oGrid.Columns.Item("U_Z_Skills").TitleObject.Caption = "Skills"
            oGrid.Columns.Item("U_Z_MgrStatus").TitleObject.Caption = "First Level Approval Status"
            oGrid.Columns.Item("U_Z_SMgrStatus").TitleObject.Caption = "Second Level Approval Status"
            oGrid.Columns.Item("U_Z_Finished").TitleObject.Caption = "Work Flow Status"
            oGrid.Columns.Item("U_Z_Finished").Visible = False

            oGrid.Columns.Item("U_Z_Status").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_Status")
            ocombo.ValidValues.Add("R", "Received")
            ocombo.ValidValues.Add("S", "Shortlisted")
            ocombo.ValidValues.Add("F", "Shortlisted 1st Level")
            ocombo.ValidValues.Add("N", "Shortlisted 2st Level")
            ocombo.ValidValues.Add("L", "Canceled")
            ocombo.ValidValues.Add("I", "Interview")
            ocombo.ValidValues.Add("D", "Interview 1st Approval")
            ocombo.ValidValues.Add("M", "Interview HR Approval")
            ocombo.ValidValues.Add("O", "Job Offered")
            ocombo.ValidValues.Add("J", "Offer Rejected")
            ocombo.ValidValues.Add("C", "Cancelled")
            ocombo.ValidValues.Add("A", "Offer Accepted")
            ocombo.ValidValues.Add("H", "Hired")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

            oGrid.Columns.Item("ReqStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("ReqStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("SA", "HOD Approved")
            ocombo.ValidValues.Add("SR", "HOD Rejected")
            ocombo.ValidValues.Add("C", "Closed")
            ocombo.ValidValues.Add("L", "Canceled")
            ocombo.ValidValues.Add("HF", "HR Follow-Up")
            ocombo.ValidValues.Add("HA", "HR Approved")
            ocombo.ValidValues.Add("HR", "HR Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

            oGrid.Columns.Item("U_Z_SMgrStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_SMgrStatus")
            ocombo.ValidValues.Add("-", "Pending")
            ocombo.ValidValues.Add("A", "Approved")
            ocombo.ValidValues.Add("R", "Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

            oGrid.Columns.Item("U_Z_SMgrRemarks").TitleObject.Caption = "Second Level Approval Remarks"

            oGrid.Columns.Item("DocEntry").Editable = False
            oGrid.Columns.Item("App/ID").Editable = False
            oGrid.Columns.Item("U_Z_HRAppName").Editable = False
            oGrid.Columns.Item("U_Z_DeptName").Editable = False
            oGrid.Columns.Item("U_Z_PosName").Editable = False
            oGrid.Columns.Item("U_Z_ReqNo").Editable = False
            oGrid.Columns.Item("ReqStatus").Editable = False
            oGrid.Columns.Item("U_Z_Dob").Editable = False
            oGrid.Columns.Item("U_Z_Mobile").Visible = False
            oGrid.Columns.Item("U_Z_Email").Editable = False
            oGrid.Columns.Item("U_Z_YrExp").Editable = False
            oGrid.Columns.Item("U_Z_AppDate").Editable = False
            oGrid.Columns.Item("U_Z_Skills").Editable = False
            oGrid.Columns.Item("U_Z_MgrStatus").Editable = False
            oGrid.Columns.Item("U_Z_SMgrStatus").Editable = True
            oGrid.Columns.Item("U_Z_SMgrRemarks").Editable = True
            oGrid.Columns.Item("U_Z_Finished").Editable = False


            If oGrid.DataTable.Rows.Count > 0 Then
                Dim strStatus As String = oGrid.DataTable.GetValue("ReqStatus", 0)
                If strStatus = "C" Or strStatus = "L" Then
                    oForm.Items.Item("_1").Enabled = False
                End If
            End If

        Else
            oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Applicant ID"
            oGrid.Columns.Item("DocEntry").Visible = False
            oGrid.Columns.Item("App/ID").TitleObject.Caption = "Applicant ID"
            oEditTextColumn = oGrid.Columns.Item("App/ID")
            oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
            oGrid.Columns.Item("U_Z_HRAppName").TitleObject.Caption = "First Name"
            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
            oGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position Name"
           
            oGrid.Columns.Item("U_Z_Mobile").TitleObject.Caption = "Mobile"

            oGrid.Columns.Item("U_Z_YrExp").TitleObject.Caption = "Year Of Experience"

            oGrid.Columns.Item("U_Z_Skills").TitleObject.Caption = "Skills"
            oGrid.Columns.Item("U_Z_OAStatus").TitleObject.Caption = "Offer Acceptance Status"

            oGrid.Columns.Item("U_Z_OAStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_OAStatus")
            ocombo.ValidValues.Add("-", "Pending")
            ocombo.ValidValues.Add("A", "Acceptance")
            ocombo.ValidValues.Add("R", "Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

            oGrid.Columns.Item("U_Z_OARemarks").TitleObject.Caption = "Remarks"

            oGrid.Columns.Item("DocEntry").Editable = False
            oGrid.Columns.Item("App/ID").Editable = False
            oGrid.Columns.Item("U_Z_HRAppName").Editable = False
            oGrid.Columns.Item("U_Z_DeptName").Editable = False
            oGrid.Columns.Item("U_Z_PosName").Editable = False
           
            oGrid.Columns.Item("U_Z_Mobile").Visible = False

            oGrid.Columns.Item("U_Z_YrExp").Editable = False

            oGrid.Columns.Item("U_Z_Skills").Editable = False
            oGrid.Columns.Item("U_Z_OAStatus").Editable = True
            oGrid.Columns.Item("U_Z_OARemarks").Editable = True

            Dim oGCol As SAPbouiCOM.EditTextColumn
            oGCol = oGrid.Columns.Item("Reject Reason")
            oGCol.ChooseFromListUID = "UDCFL4"
            oGCol.ChooseFromListAlias = "U_Z_TypeName"
            oGrid.Columns.Item("U_Z_Status").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_Status")
            ocombo.ValidValues.Add("R", "Received")
            ocombo.ValidValues.Add("S", "Shortlisted")
            ocombo.ValidValues.Add("F", "Shortlisted 1st Level")
            ocombo.ValidValues.Add("N", "Shortlisted 2st Level")
            ocombo.ValidValues.Add("L", "Canceled")
            ocombo.ValidValues.Add("I", "Interview")
            ocombo.ValidValues.Add("D", "Interview 1st Approval")
            ocombo.ValidValues.Add("M", "Interview HR Approval")
            ocombo.ValidValues.Add("O", "Job Offered")
            ocombo.ValidValues.Add("J", "Offer Rejected")
            ocombo.ValidValues.Add("C", "Cancelled")
            ocombo.ValidValues.Add("A", "Offer Accepted")
            ocombo.ValidValues.Add("H", "Hired")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

        End If

        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single


    End Sub

#Region "Update Approval ShortListed"
    Private Function updateApprovedShortListed(ByVal aForm As SAPbouiCOM.Form, ByVal strtitle As String) As Boolean
        oForm.Freeze(True)
        Dim strEmpId, strcode, strqry As String
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
        If strtitle = "Shortlisting First Level Approval" Then
            Try
                For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                    strcode = oGrid.DataTable.GetValue("DocEntry", intRow)

                    'Get
                    strqry = "Select U_Z_HRAppID From [@Z_HR_OHEM1] Where DocEntry = '" & strcode & "'"
                    oValidateRS.DoQuery(strqry)
                    Dim strAppID As String
                    If Not oValidateRS.EoF Then
                        strAppID = oValidateRS.Fields.Item(0).Value
                    End If

                    If oGrid.DataTable.GetValue("U_Z_MgrStatus", intRow) = "A" Then
                        'Update Applicatant Status To Shortlisted First Level Approved
                        strqry = "Update [@Z_HR_OCRAPP] Set U_Z_Status = 'F' Where DocEntry = '" & strAppID & "'"
                        oValidateRS.DoQuery(strqry)
                    ElseIf oGrid.DataTable.GetValue("U_Z_MgrStatus", intRow) = "R" Then
                        strqry = "Update [@Z_HR_OCRAPP] Set U_Z_Status = 'R' Where DocEntry = '" & strAppID & "'"
                        oValidateRS.DoQuery(strqry)
                    End If

                    strqry = "Update [@Z_HR_OHEM1] set U_Z_MgrStatus = '" & oGrid.DataTable.GetValue("U_Z_MgrStatus", intRow) & "',U_Z_MgrRemarks='" & oGrid.DataTable.GetValue("U_Z_MgrRemarks", intRow) & "' Where DocEntry = '" & strcode & "'"
                    oValidateRS.DoQuery(strqry)

                    'Time Stamp
                    Dim sAppID As String
                    oValidateRS.DoQuery("Select U_Z_HRAPPID from [@Z_HR_OHEM1] where DocEntry=" & strcode)

                    If Not oValidateRS.EoF Then
                        sAppID = oValidateRS.Fields.Item("U_Z_HRAPPID").Value
                        oApplication.Utilities.UpdateApplicantTimeStamp(sAppID, "SFL")
                    End If
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
        ElseIf strtitle = "Shortlisting Second Level Approval" Then
            Try
                For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                    strcode = oGrid.DataTable.GetValue("DocEntry", intRow)
                    '  strqry = "Update [@Z_HR_OHEM1] set U_Z_SMgrStatus = '" & oGrid.DataTable.GetValue("U_Z_SMgrStatus", intRow) & "',U_Z_SMgrRemarks='" & oGrid.DataTable.GetValue("U_Z_SMgrRemarks", intRow) & "' where U_Z_HRAppID ='" & strcode & "'"

                    strqry = "Update [@Z_HR_OHEM1] set U_Z_SMgrStatus = '" & oGrid.DataTable.GetValue("U_Z_SMgrStatus", intRow) & "',U_Z_SMgrRemarks='" & oGrid.DataTable.GetValue("U_Z_SMgrRemarks", intRow) & "' where DocEntry = '" & strcode & "'"
                    oValidateRS.DoQuery(strqry)
                    strqry = "Select U_Z_HRAppID From [@Z_HR_OHEM1] Where DocEntry = '" & strcode & "'"
                    oValidateRS.DoQuery(strqry)
                    Dim strAppID As String
                    If Not oValidateRS.EoF Then
                        strAppID = oValidateRS.Fields.Item(0).Value
                    End If

                    If oGrid.DataTable.GetValue("U_Z_SMgrStatus", intRow) = "A" Then
                        'Update Applicatant Status To ShortListed Second Level Approved
                        strqry = "Update [@Z_HR_OCRAPP] Set U_Z_Status = 'N' Where DocEntry = '" & strAppID & "'"
                        oValidateRS.DoQuery(strqry)

                        strqry = "Update [@Z_HR_OHEM1] Set U_Z_ApplStatus = 'A' Where DocEntry = '" & strcode & "'"
                        oValidateRS.DoQuery(strqry)
                    ElseIf oGrid.DataTable.GetValue("U_Z_SMgrStatus", intRow) = "R" Then

                        strqry = "Update [@Z_HR_OCRAPP] Set U_Z_Status = 'R' Where DocEntry = '" & strAppID & "'"
                        oValidateRS.DoQuery(strqry)
                        strqry = "Update [@Z_HR_OHEM1] Set U_Z_ApplStatus = 'R',U_Z_Finished = 'Y' Where DocEntry = '" & strcode & "'"
                        oValidateRS.DoQuery(strqry)
                    End If

                    'Time Stamp
                    Dim sAppID As String
                    oValidateRS.DoQuery("Select U_Z_HRAPPID from [@Z_HR_OHEM1] where DocEntry=" & strcode)
                    If Not oValidateRS.EoF Then
                        sAppID = oValidateRS.Fields.Item("U_Z_HRAPPID").Value
                        oApplication.Utilities.UpdateApplicantTimeStamp(sAppID, "SSL")
                    End If
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
        ElseIf strtitle = "HR Offer Acceptance" Then
            Try
                For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                    strcode = oGrid.DataTable.GetValue("DocEntry", intRow)
                    '    strqry = "Update [@Z_HR_OHEM1] set U_Z_OAStatus = '" & oGrid.DataTable.GetValue("U_Z_OAStatus", intRow) & "',U_Z_OARemarks='" & oGrid.DataTable.GetValue("U_Z_OARemarks", intRow) & "' where U_Z_HRAppID ='" & strcode & "'"
                    strqry = "Update [@Z_HR_OHEM1] set U_Z_OAStatus = '" & oGrid.DataTable.GetValue("U_Z_OAStatus", intRow) & "',U_Z_OARemarks='" & oGrid.DataTable.GetValue("U_Z_OARemarks", intRow) & "' where DocEntry ='" & strcode & "'"
                    oValidateRS.DoQuery(strqry)

                    'Candidate Accepted Offer
                    If oGrid.DataTable.GetValue("U_Z_OAStatus", intRow) = "A" Then
                        Dim otest As SAPbobsCOM.Recordset
                        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        otest.DoQuery("SElect * from [@Z_HR_OHEM1] where DocEntry=" & strcode)

                        strcode = otest.Fields.Item("U_Z_HRAPPID").Value
                        strqry = "Update [@Z_HR_OCRAPP] set U_Z_Status = 'O' where DocEntry = '" & strcode & "'"
                        oValidateRS.DoQuery(strqry)
                    End If
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
            If pVal.FormTypeEx = frm_hr_AppShortListed Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If strFunction = "LM" Then
                                    If pVal.ItemUID = "3" And pVal.ColUID = "U_Z_MgrStatus" Then
                                        Dim oGrid_3 As SAPbouiCOM.Grid = oForm.Items.Item("3").Specific
                                        Dim strSMgrValue = oGrid_3.DataTable.Columns.Item("U_Z_SMgrStatus").Cells.Item(pVal.Row).Value
                                        If strSMgrValue = "Pending" Then
                                        Else
                                            BubbleEvent = False
                                            oApplication.Utilities.Message("Second Level  Manager already Approved...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                        End If
                                    End If
                                ElseIf pVal.ItemUID = "3" And pVal.ColUID = "U_Z_MgrStatus" And pVal.Row <> -1 Then
                                    Dim oGrid_3 As SAPbouiCOM.Grid = oForm.Items.Item("3").Specific
                                    Dim strFinalStatus = oGrid_3.DataTable.Columns.Item("U_Z_Finished").Cells.Item(pVal.Row).Value
                                    If strFinalStatus = "Y" Then
                                        BubbleEvent = False
                                        oApplication.Utilities.Message("Applicant Work Flow Already Finished...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    End If
                                ElseIf pVal.ItemUID = "3" And pVal.ColUID = "U_Z_SMgrStatus" And pVal.Row <> -1 Then
                                    Dim oGrid_3 As SAPbouiCOM.Grid = oForm.Items.Item("3").Specific
                                    Dim strFinalStatus = oGrid_3.DataTable.Columns.Item("U_Z_Finished").Cells.Item(pVal.Row).Value
                                    If strFinalStatus = "Y" Then
                                        BubbleEvent = False
                                        oApplication.Utilities.Message("Applicant Work Flow Already Finished...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" And pVal.ColUID = "App/ID" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcode As String = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    Dim ooBj As New clshrCrApplicants
                                    ooBj.ViewCandidate(strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "3" And pVal.ColUID = "U_Z_ReqNo" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcode As String = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    Dim objct As New clshrMPRequest
                                    objct.LoadForm1(strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                If pVal.ItemUID = "_1" Then
                                    If Not validate(oForm) Then
                                        oApplication.Utilities.Message("Select Status to Update...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                        BubbleEvent = False
                                    Else
                                        If Not ValidateStatus(oForm) Then
                                            BubbleEvent = False
                                        End If
                                    End If
                                End If
                        End Select
                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "_1" Then
                                    If oApplication.SBO_Application.MessageBox("Do you want confirm the Approval", , "Yes", "No") = 2 Then
                                        Exit Sub
                                    Else
                                        If updateApprovedShortListed(oForm, oForm.Title) = True Then
                                            oApplication.Utilities.Message(oForm.Title & " successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                            oForm.Close()
                                        End If
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oGridDetail As SAPbouiCOM.Grid
                                oGridDetail = oForm.Items.Item("3").Specific
                                If pVal.ItemUID = "3" And pVal.ColUID = "Reject Reason" Then
                                    Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                    Dim oCFL As SAPbouiCOM.ChooseFromList
                                    Dim val1 As String
                                    Dim sCHFL_ID As String
                                    Try
                                        oCFLEvento = pVal
                                        sCHFL_ID = oCFLEvento.ChooseFromListUID
                                        oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                        oCFL = oForm.ChooseFromLists.Item(sCHFL_ID)
                                        If (oCFLEvento.BeforeAction = False) Then
                                            Dim oDataTable As SAPbouiCOM.DataTable
                                            oDataTable = oCFLEvento.SelectedObjects
                                            oForm.Freeze(True)
                                            val1 = oDataTable.GetValue("U_Z_TypeName", 0)
                                            Try
                                                oGridDetail.DataTable.Columns.Item("Reject Reason").Cells.Item(pVal.Row).Value = val1
                                            Catch ex As Exception
                                            End Try
                                            oForm.Freeze(False)
                                        End If
                                    Catch ex As Exception
                                        oForm.Freeze(False)
                                    End Try
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

    Public Function validate(ByVal oForm As SAPbouiCOM.Form) As Boolean
        Dim _retVal As Boolean = False
        oGrid = oForm.Items.Item("3").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oForm.Title = "Shortlisting First Level Approval" Then
                If oGrid.DataTable.GetValue("U_Z_MgrStatus", intRow) <> "-" Then
                    _retVal = True
                    Exit For
                End If
            ElseIf oForm.Title = "Shortlisting Second Level Approval" Then
                If oGrid.DataTable.GetValue("U_Z_SMgrStatus", intRow) <> "-" Then
                    _retVal = True
                    Exit For
                End If
            ElseIf oForm.Title = "HR Offer Acceptance" Then
                If oGrid.DataTable.GetValue("U_Z_OAStatus", intRow) <> "-" Then
                    _retVal = True
                    Exit For
                End If
            End If

        Next
        Return True
        'Return _retVal
    End Function

    Public Function ValidateStatus(ByVal oForm As SAPbouiCOM.Form) As Boolean
        Dim _retVal As Boolean = False
        oGrid = oForm.Items.Item("3").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            Try
                If oGrid.DataTable.GetValue("U_Z_OAStatus", intRow) = "R" Then
                    If oGrid.DataTable.GetValue("Reject Reason", intRow) = "" Then
                        oApplication.Utilities.Message("Enter Rejection Reason at Line : " & intRow + 1 & "", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return _retVal
                    End If
                End If
            Catch ex As Exception

            End Try
        Next
        _retVal = True
        Return _retVal

    End Function

End Class
