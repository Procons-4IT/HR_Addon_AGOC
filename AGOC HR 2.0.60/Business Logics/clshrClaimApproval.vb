Imports System.IO
Imports System.Globalization
Public Class clshrClaimApproval
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox, oCombobox1, oCombobox2 As SAPbouiCOM.ComboBox
    Private oEditTextColumn, oGECol As SAPbouiCOM.EditTextColumn
    Private oGridCombo, oGridCombo1, oGridCombo2 As SAPbouiCOM.ComboBoxColumn
    Private oGrid, oGrid1 As SAPbouiCOM.Grid
    Private dtDocumentList As SAPbouiCOM.DataTable
    Private dtHistoryList As SAPbouiCOM.DataTable
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Dim oRec As SAPbobsCOM.Recordset
    Dim sQuery As String

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub

    Public Sub LoadForm(ByVal oForm As SAPbouiCOM.Form)
        Try

            If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_ClaimApproval) = False Then
                oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Exit Sub
            End If
            oForm = oApplication.Utilities.LoadForm(xml_hr_ClaimApproval, frm_hr_ClaimApproval)
            oForm = oApplication.SBO_Application.Forms.ActiveForm()
            oForm.Freeze(True)
            oForm.PaneLevel = 1
            oCombobox1 = oForm.Items.Item("13").Specific
            oCombobox = oForm.Items.Item("15").Specific
            oCombobox.ValidValues.Add("0", "")
            For j As Integer = 2010 To 2050
                Dim year As String = j
                oCombobox.ValidValues.Add(year, year)
            Next
            oCombobox1.ValidValues.Add("0", "")
            For i As Integer = 1 To 12
                Dim info As DateTimeFormatInfo = DateTimeFormatInfo.GetInstance(Nothing)
                oCombobox1.ValidValues.Add(i, info.GetMonthName(i))
            Next
            oForm.DataSources.DataTables.Add("dtDocumentList")
            oForm.DataSources.DataTables.Add("dtHistoryList")
            oCombobox = oForm.Items.Item("8").Specific
            'oApplication.Utilities.InitializationApproval(oForm, HeaderDoctype.ExpCli, HistoryDoctype.ExpCli)
            ' oApplication.Utilities.ApprovalSummary(oForm, HeaderDoctype.ExpCli, HistoryDoctype.ExpCli)
            'InitializationApproval(oForm)
            LoadSummary(oForm)
            LoadBind(oForm)
            oGrid = oForm.Items.Item("1").Specific
            oGrid.Columns.Item("RowsHeader").Click(0, False, False)
            oGrid = oForm.Items.Item("19").Specific
            oGrid.Columns.Item("RowsHeader").Click(0, False, False)
            oForm.Items.Item("4").TextStyle = 7
            oForm.Items.Item("5").TextStyle = 7
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
            Throw ex
        End Try
    End Sub
    Private Sub LoadBind(ByVal aForm As SAPbouiCOM.Form)
        Try
            oGrid = aForm.Items.Item("1").Specific
            sQuery = " select distinct(T0.Code),T0.U_Z_TAEmpID,T0.U_Z_EmpID,T0.U_Z_EmpName,T0.U_Z_Subdt,T0.U_Z_Client,T0.U_Z_Project,Case T0.U_Z_DocStatus when 'C' then 'Closed' else 'Open' end as 'Document Status'  from [@Z_HR_OEXPCL] T0"
            sQuery += " Left outer Join [@Z_HR_EXPCL] T1 on T0.Code=T1.U_Z_DocRefNo "
            sQuery += " JOIN [@Z_HR_APPT1] T4 ON T0.U_Z_EmpID = T4.U_Z_OUser" '  and (T1.""U_Z_AppStatus""='P' or T1.""U_Z_AppStatus""='-') "
            sQuery += " JOIN [@Z_HR_APPT2] T2 ON T4.DocEntry = T2.DocEntry "
            sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
            sQuery += " And (T1.U_Z_CurApprover = '" + oApplication.Company.UserName + "' OR T1.U_Z_NxtApprover = '" + oApplication.Company.UserName + "')"
            sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and  isnull(T1.U_Z_AppRequired,'N')='Y' and  T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = 'ExpCli'  Order by T0.Code Desc"
            oGrid.DataTable.ExecuteQuery(sQuery)
            LoadDocument(aForm)
            oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
            oGrid.Columns.Item("RowsHeader").Click(0, False, False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
    Private Sub LoadDocument(ByVal aForm As SAPbouiCOM.Form)
        oGrid = aForm.Items.Item("1").Specific
        oGrid.Columns.Item("Code").TitleObject.Caption = "Expense Claim No."
        oGrid.Columns.Item("U_Z_TAEmpID").TitleObject.Caption = "T&A Employee No"
        oGrid.Columns.Item("U_Z_TAEmpID").Visible = False
        oGrid.Columns.Item("U_Z_EmpID").TitleObject.Caption = "Employee"
        oEditTextColumn = oGrid.Columns.Item("U_Z_EmpID")
        oEditTextColumn.LinkedObjectType = "171"
        oGrid.Columns.Item("U_Z_EmpID").Visible = False
        oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
        oGrid.Columns.Item("U_Z_Subdt").TitleObject.Caption = "Submitted Date"
        oGrid.Columns.Item("U_Z_Client").TitleObject.Caption = "Client"
        oGrid.Columns.Item("U_Z_Project").TitleObject.Caption = "Project"
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oGrid.AutoResizeColumns()
    End Sub
    Private Sub LoadSummary(ByVal aForm As SAPbouiCOM.Form)
        Try
            oGrid = aForm.Items.Item("19").Specific
            sQuery = " select distinct(T0.Code),T0.U_Z_TAEmpID,T0.U_Z_EmpID,T0.U_Z_EmpName,T0.U_Z_Subdt,T0.U_Z_Client,T0.U_Z_Project,Case T0.U_Z_DocStatus when 'C' then 'Closed' else 'Open' end as 'Document Status'  from [@Z_HR_OEXPCL] T0"
            sQuery += " Left outer Join [@Z_HR_EXPCL] T1 on T0.Code=T1.U_Z_DocRefNo "
            sQuery += " JOIN [@Z_HR_APPT1] T4 ON T0.U_Z_EmpID = T4.U_Z_OUser "
            sQuery += " JOIN [@Z_HR_APPT2] T2 ON T4.DocEntry = T2.DocEntry "
            sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
            'sQuery += " And (T1.U_Z_CurApprover = '" + oApplication.Company.UserName + "' OR T1.U_Z_NxtApprover = '" + oApplication.Company.UserName + "')"
            sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and  isnull(T1.U_Z_AppRequired,'N')='Y' and  T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = 'ExpCli' Order by T0.Code Desc"
            oGrid.DataTable.ExecuteQuery(sQuery)
            SummaryHeadDocument(aForm)
            oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
            oGrid.Columns.Item("RowsHeader").Click(0, False, False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
    Private Sub SummaryHeadDocument(ByVal aForm As SAPbouiCOM.Form)
        oGrid = aForm.Items.Item("19").Specific
        oGrid.Columns.Item("Code").TitleObject.Caption = "Expense Claim No."
        oGrid.Columns.Item("U_Z_TAEmpID").TitleObject.Caption = "T&A Employee No"
        oGrid.Columns.Item("U_Z_TAEmpID").Visible = False
        oGrid.Columns.Item("U_Z_EmpID").TitleObject.Caption = "Employee"
        oEditTextColumn = oGrid.Columns.Item("U_Z_EmpID")
        oEditTextColumn.LinkedObjectType = "171"
        oGrid.Columns.Item("U_Z_EmpID").Visible = False
        oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
        oGrid.Columns.Item("U_Z_Subdt").TitleObject.Caption = "Submitted Date"
        oGrid.Columns.Item("U_Z_Client").TitleObject.Caption = "Client"
        oGrid.Columns.Item("U_Z_Project").TitleObject.Caption = "Project"
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oGrid.AutoResizeColumns()
    End Sub
    Private Sub InitializationApproval(ByVal aForm As SAPbouiCOM.Form, ByVal strCode As String)
        Try
            oGrid = aForm.Items.Item("1").Specific
            Dim stremp As String = ""
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                If oGrid.Rows.IsSelected(intRow) Then
                    stremp = oGrid.DataTable.GetValue("U_Z_EmpID", intRow)
                    Exit For
                End If
            Next
            oGrid = aForm.Items.Item("3").Specific
            sQuery = " Select T0.U_Z_DocRefNo,T0.Code,T0.U_Z_EmpID,T0.U_Z_EmpName,T0.U_Z_SubDt,T0.U_Z_Client,T0.U_Z_Project,U_Z_Claimdt,U_Z_ExpType,U_Z_Currency,U_Z_CurAmt,U_Z_UsdAmt,U_Z_ReimAmt,case U_Z_Posting when 'G' then 'G/L Account' else 'Payroll' end as U_Z_Posting,Isnull(T5.U_Z_AppStatus,'P') AS U_Z_AppStatus,Convert(Varchar(10),isnull(T5.""U_Z_Month"",MONTH(U_Z_Claimdt))) AS ""U_Z_Month"",Convert(Varchar(10),isnull(T5.""U_Z_Year"",YEAR(U_Z_Claimdt))) AS ""U_Z_Year"","
            sQuery += "T5.U_Z_Remarks,U_Z_Attachment,isnull((select T13.firstName +' '+ISNULL(T13.middleName,'') +' '+ isnull(T13.lastName,'')  from OHEM T13 JOIN OUSR T14 ON T14.INTERNAL_K =T13.userId where T14.USER_CODE=T0.U_Z_CurApprover ),'') AS 'Current Approver',isnull((select T13.firstName +' '+ISNULL(T13.middleName,'') +' '+ isnull(T13.lastName,'')  from OHEM T13 JOIN OUSR T14 ON T14.INTERNAL_K =T13.userId where T14.USER_CODE=T0.U_Z_NxtApprover ),'') AS 'Next Approver', "
            sQuery += " Case U_Z_AppRequired when 'Y' then 'Yes' else 'No' End as  'Approval Required',U_Z_AppReqDate 'Requested Date',CONVERT(VARCHAR(8),U_Z_ReqTime,108) AS 'Requested Time'"
            sQuery += ",T5.DocEntry  From [@Z_HR_EXPCL] T0 Left Outer Join [@Z_HR_APHIS] T5 on T0.Code=T5.U_Z_DocEntry And T5.U_Z_DocType= 'ExpCli' and T5.U_Z_ApproveBy='" + oApplication.Company.UserName + "'"
            ' sQuery += " JOIN [@Z_HR_OEXPCL] T6 ON T0.U_Z_DocRefNo = T6.Code "
            sQuery += " JOIN [@Z_HR_APPT1] T1 ON T0.U_Z_EmpID = T1.U_Z_OUser  and (T0.""U_Z_AppStatus""='P' or T0.""U_Z_AppStatus""='-') "
            sQuery += " JOIN [@Z_HR_APPT2] T2 ON T1.DocEntry = T2.DocEntry "
            sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
            sQuery += " And (T0.U_Z_CurApprover = '" + oApplication.Company.UserName + "' OR T0.U_Z_NxtApprover = '" + oApplication.Company.UserName + "')"
            sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and  isnull(T0.U_Z_AppRequired,'N')='Y' and  T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = 'ExpCli' where T0.U_Z_DocRefNo='" & strCode & "' and T0.U_Z_EmpID='" & stremp & "' Order by Convert(Numeric,T0.Code) Desc"
            oGrid.DataTable.ExecuteQuery(sQuery)
            formatDocument(aForm)
            oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
            'oGrid.Columns.Item("RowsHeader").Click(0, False, False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
    Private Sub formatDocument(ByVal aForm As SAPbouiCOM.Form)
        oGrid = aForm.Items.Item("3").Specific
        oGrid.Columns.Item("U_Z_DocRefNo").TitleObject.Caption = "Expense Claim No."
        oGrid.Columns.Item("U_Z_DocRefNo").Editable = False
        oGrid.Columns.Item("Code").TitleObject.Caption = "Serial No."
        oGrid.Columns.Item("Code").Editable = False
        oEditTextColumn = oGrid.Columns.Item("Code")
        oEditTextColumn.LinkedObjectType = "Z_HR_OEXFOM"
        oGrid.Columns.Item("U_Z_EmpID").TitleObject.Caption = "Employee"
        oEditTextColumn = oGrid.Columns.Item("U_Z_EmpID")
        oEditTextColumn.LinkedObjectType = "171"
        oGrid.Columns.Item("U_Z_EmpID").Visible = False
        oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
        oGrid.Columns.Item("U_Z_EmpName").Visible = False
        oGrid.Columns.Item("U_Z_SubDt").TitleObject.Caption = "Submitted Date"
        oGrid.Columns.Item("U_Z_SubDt").Visible = False
        oGrid.Columns.Item("U_Z_Claimdt").TitleObject.Caption = "Transaction Date"
        oGrid.Columns.Item("U_Z_Claimdt").Editable = False
        oGrid.Columns.Item("U_Z_ExpType").TitleObject.Caption = "Expense Type"
        oEditTextColumn = oGrid.Columns.Item("U_Z_ExpType")
        oEditTextColumn.LinkedObjectType = "Z_HR_EXPANCES"
        oGrid.Columns.Item("U_Z_ExpType").Editable = False
        oGrid.Columns.Item("U_Z_Currency").TitleObject.Caption = "Transaction Currency"
        oGrid.Columns.Item("U_Z_Currency").Editable = False
        oGrid.Columns.Item("U_Z_Client").TitleObject.Caption = "Client"
        oGrid.Columns.Item("U_Z_Client").Visible = False
        oGrid.Columns.Item("U_Z_Project").TitleObject.Caption = "Project"
        oGrid.Columns.Item("U_Z_Project").Visible = False
        oGrid.Columns.Item("U_Z_CurAmt").TitleObject.Caption = "Transaction Amount"
        oGrid.Columns.Item("U_Z_CurAmt").Editable = False
        oGrid.Columns.Item("U_Z_UsdAmt").TitleObject.Caption = "Local Currency Amount"
        oGrid.Columns.Item("U_Z_UsdAmt").Editable = False
        oGrid.Columns.Item("U_Z_ReimAmt").TitleObject.Caption = "Redim Amount"
        oGrid.Columns.Item("U_Z_ReimAmt").Editable = False
        oGrid.Columns.Item("U_Z_Posting").TitleObject.Caption = "Posting To (Payroll/GL)"
        oGrid.Columns.Item("U_Z_Posting").Editable = False
        oGrid.Columns.Item("U_Z_Attachment").TitleObject.Caption = "Attachments"
        oGECol = oGrid.Columns.Item("U_Z_Attachment")
        oGECol.LinkedObjectType = "Z_HR_OEXFOM"
        oGrid.Columns.Item("U_Z_Attachment").Editable = False
       
        oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
        oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
        oGridCombo.ValidValues.Add("P", "Pending")
        oGridCombo.ValidValues.Add("A", "Approved")
        oGridCombo.ValidValues.Add("R", "Rejected")
        oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
        oGrid.Columns.Item("U_Z_AppStatus").Editable = True
        oGrid.Columns.Item("U_Z_Month").TitleObject.Caption = "Payroll Month"
        oGrid.Columns.Item("U_Z_Month").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGridCombo1 = oGrid.Columns.Item("U_Z_Month")
        oGridCombo1.ValidValues.Add("0", "")
        For i As Integer = 1 To 12
            Dim info As DateTimeFormatInfo = DateTimeFormatInfo.GetInstance(Nothing)
            oGridCombo1.ValidValues.Add(i, info.GetMonthName(i))
        Next
        oGridCombo1.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
        oGrid.Columns.Item("U_Z_Month").Editable = True
        oGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Payroll Year"
        oGrid.Columns.Item("U_Z_Year").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGridCombo2 = oGrid.Columns.Item("U_Z_Year")
        oGridCombo2.ValidValues.Add("0", "")
        For j As Integer = 2010 To 2050
            Dim year As String = j
            oGridCombo2.ValidValues.Add(year, year)
        Next
        oGridCombo2.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
        oGrid.Columns.Item("U_Z_Year").Editable = True
        oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"
        oGrid.Columns.Item("U_Z_Remarks").Editable = True
        oGrid.Columns.Item("DocEntry").Visible = False
        oGrid.Columns.Item("Current Approver").Editable = False
        oGrid.Columns.Item("Next Approver").Editable = False
        oGrid.Columns.Item("Approval Required").Editable = False
        oGrid.Columns.Item("Requested Date").Editable = False
        oGrid.Columns.Item("Requested Time").Editable = False
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oGrid.AutoResizeColumns()
    End Sub
    Private Sub ApprovalSummary(ByVal aForm As SAPbouiCOM.Form, ByVal strcode As String)
        oGrid = aForm.Items.Item("19").Specific
        Dim stremp As String = ""
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.Rows.IsSelected(intRow) Then
                stremp = oGrid.DataTable.GetValue("U_Z_EmpID", intRow)
                Exit For
            End If
        Next
        oGrid = aForm.Items.Item("20").Specific
        sQuery = " Select Code,T0.U_Z_EmpID,U_Z_EmpName,U_Z_SubDt,U_Z_Claimdt,U_Z_ExpType,U_Z_Currency,U_Z_CurAmt,U_Z_UsdAmt,U_Z_ReimAmt,case U_Z_Posting when 'G' then 'G/L Account' else 'Payroll' end as U_Z_Posting,U_Z_AppStatus,U_Z_Client,""U_Z_Month"",""U_Z_Year"",U_Z_Project, "
        sQuery += "U_Z_Attachment, Case U_Z_AppRequired when 'Y' then 'Yes' else 'No' End as  'Approval Required',U_Z_AppReqDate 'Requested Date',CONVERT(VARCHAR(8),U_Z_ReqTime,108) AS 'Requested Time'"
        sQuery += " , isnull((select T13.firstName +' '+ISNULL(T13.middleName,'') +' '+ isnull(T13.lastName,'')  from OHEM T13 JOIN OUSR T14 ON T14.INTERNAL_K =T13.userId where T14.USER_CODE=T0.U_Z_CurApprover ),'') AS 'Current Approver',isnull((select T13.firstName +' '+ISNULL(T13.middleName,'') +' '+ isnull(T13.lastName,'')  from OHEM T13 JOIN OUSR T14 ON T14.INTERNAL_K =T13.userId where T14.USER_CODE=T0.U_Z_NxtApprover ),'') AS 'Next Approver' From [@Z_HR_EXPCL] T0 JOIN [@Z_HR_APPT1] T1 ON T0.U_Z_EmpID = T1.U_Z_OUser "
        sQuery += " JOIN [@Z_HR_APPT2] T2 ON T1.DocEntry = T2.DocEntry "
        sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
        sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and  T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = 'ExpCli' where T0.U_Z_DocRefNo='" & strcode & "' and T0.U_Z_EmpID='" & stremp & "' Order by Convert(Numeric,Code) Desc"
        oGrid.DataTable.ExecuteQuery(sQuery)
        SummaryDocument(aForm)
        oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
        oGrid.Columns.Item("RowsHeader").Click(0, False, False)
    End Sub
    Private Sub SummaryDocument(ByVal aForm As SAPbouiCOM.Form)
        Dim oGridCombo As SAPbouiCOM.ComboBoxColumn
        Dim oEditTextColumn As SAPbouiCOM.EditTextColumn
        Dim oGECol As SAPbouiCOM.EditTextColumn
        oGrid = aForm.Items.Item("20").Specific
        oGrid.Columns.Item("Code").TitleObject.Caption = "Request No."
        oEditTextColumn = oGrid.Columns.Item("Code")
        oEditTextColumn.LinkedObjectType = "Z_HR_OEXFOM"
        oGrid.Columns.Item("U_Z_EmpID").TitleObject.Caption = "Employee"
        oEditTextColumn = oGrid.Columns.Item("U_Z_EmpID")
        oEditTextColumn.LinkedObjectType = "171"
        oGrid.Columns.Item("U_Z_EmpID").Visible = False
        oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
        oGrid.Columns.Item("U_Z_EmpName").Visible = False
        oGrid.Columns.Item("U_Z_SubDt").TitleObject.Caption = "Submitted Date"
        oGrid.Columns.Item("U_Z_SubDt").Visible = False
        oGrid.Columns.Item("U_Z_Claimdt").TitleObject.Caption = "Transaction Date"
        oGrid.Columns.Item("U_Z_ExpType").TitleObject.Caption = "Expense Type"
        oEditTextColumn = oGrid.Columns.Item("U_Z_ExpType")
        oEditTextColumn.LinkedObjectType = "Z_HR_EXPANCES"
        oGrid.Columns.Item("U_Z_Currency").TitleObject.Caption = "Transaction Currency"
        oGrid.Columns.Item("U_Z_Client").TitleObject.Caption = "Client"
        oGrid.Columns.Item("U_Z_Client").Visible = False
        oGrid.Columns.Item("U_Z_Project").TitleObject.Caption = "Project"
        oGrid.Columns.Item("U_Z_Project").Visible = False
        oGrid.Columns.Item("U_Z_CurAmt").TitleObject.Caption = "Transaction Amount"
        oGrid.Columns.Item("U_Z_UsdAmt").TitleObject.Caption = "Local Currency Amount"
        oGrid.Columns.Item("U_Z_ReimAmt").TitleObject.Caption = "Redim Amount"
        oGrid.Columns.Item("U_Z_Posting").TitleObject.Caption = "Posting To (Payroll/GL)"
        oGrid.Columns.Item("U_Z_Month").TitleObject.Caption = " Payroll Month"
        oGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Payroll Year"
        oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
        oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
        oGridCombo.ValidValues.Add("P", "Pending")
        oGridCombo.ValidValues.Add("A", "Approved")
        oGridCombo.ValidValues.Add("R", "Rejected")
        oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
        oGrid.Columns.Item("U_Z_Attachment").TitleObject.Caption = "Attachments"
        oGECol = oGrid.Columns.Item("U_Z_Attachment")
        oGECol.LinkedObjectType = "Z_HR_OEXFOM"
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oGrid.AutoResizeColumns()
    End Sub

      
    Private Sub LoadFiles(ByVal aform As SAPbouiCOM.Form, ByVal ItemUID As String)
        oGrid = aform.Items.Item(ItemUID).Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.Rows.IsSelected(intRow) Then
                Dim strFilename, strFilePath As String
                strFilename = oGrid.DataTable.GetValue("U_Z_Attachment", intRow)
                Dim Filename As String = Path.GetFileName(strFilename)
                strFilePath = oGrid.DataTable.GetValue("U_Z_Attachment", intRow)

                If File.Exists(strFilePath) = False Then
                    Dim oRec As SAPbobsCOM.Recordset
                    oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    Dim strQry = "Select ""AttachPath"" From OADP"
                    oRec.DoQuery(strQry)
                    strFilePath = oRec.Fields.Item(0).Value

                    If Filename = "" Then
                        strFilePath = strFilePath
                    Else
                        strFilePath = strFilePath & Filename
                    End If
                    If File.Exists(strFilePath) = False Then
                        oApplication.Utilities.Message("File does not exists ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Exit Sub
                    End If
                    strFilename = strFilePath
                Else
                    strFilename = strFilePath
                End If

                Dim x As System.Diagnostics.ProcessStartInfo
                x = New System.Diagnostics.ProcessStartInfo
                x.UseShellExecute = True
                x.FileName = strFilename
                System.Diagnostics.Process.Start(x)
                x = Nothing
                Exit Sub
            End If
        Next
        oApplication.Utilities.Message("No file has been selected...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
    End Sub
    Private Sub ComboSelect(ByVal sForm As SAPbouiCOM.Form, ByVal Status As String)
        Try
            oCombobox1 = oForm.Items.Item("13").Specific
            ' oCombobox1.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
            'oCombobox2.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
        Catch ex As Exception

        End Try
    End Sub

    Public Sub addUpdateDocument(ByVal aForm As SAPbouiCOM.Form)
        Dim oGeneralService As SAPbobsCOM.GeneralService
        Dim oGeneralData As SAPbobsCOM.GeneralData
        Dim oGeneralParams As SAPbobsCOM.GeneralDataParams
        Dim oCompanyService As SAPbobsCOM.CompanyService
        Dim oChildren As SAPbobsCOM.GeneralDataCollection
        oCompanyService = oApplication.Company.GetCompanyService()
        Dim otestRs As SAPbobsCOM.Recordset
        Dim oChild As SAPbobsCOM.GeneralData
        Dim strCode, strQuery As String
        Dim strEmpName As String = ""
        Dim blnRecordExists As Boolean = False
        Dim HeadDocEntry, UserLineId As Integer
        Dim oRecordSet As SAPbobsCOM.Recordset
        Dim oComboBox1, oCombobox2 As SAPbouiCOM.ComboBox
        Try
            If oApplication.SBO_Application.MessageBox("Documents once approved can not be changed. Do you want Continue?", , "Contine", "Cancel") = 2 Then
                Exit Sub
            End If
            oGeneralService = oCompanyService.GetGeneralService("Z_HR_APHIS")
            oGeneralData = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData)
            oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)
            oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otestRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oGrid = aForm.Items.Item("3").Specific
            Dim strDocEntry As String = ""
            Dim strDocType1, HeaderCode, NextApprover As String
            Dim strHeader As String = enDocType
            Dim strEmpID As String = ""
            Dim strLeaveType As String = ""
            MailDocEntry = ""
            If oGrid.DataTable.Rows.Count > 0 Then
                For index As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                    If oGrid.DataTable.GetValue("U_Z_CurAmt", index) > 0.0 Then
                        HeaderCode = oGrid.DataTable.GetValue("U_Z_DocRefNo", index)
                        strDocEntry = oGrid.DataTable.GetValue("Code", index)

                        strEmpID = oGrid.DataTable.GetValue("U_Z_EmpID", index)
                        strQuery = "select T0.DocEntry,T1.LineId from [@Z_HR_OAPPT] T0 JOIN [@Z_HR_APPT2] T1 on T0.DocEntry=T1.DocEntry"
                        strQuery += " JOIN [@Z_HR_APPT1] T2 on T1.DocEntry=T2.DocEntry"
                        strQuery += " where T0.U_Z_DocType='ExpCli' AND T2.U_Z_OUser='" & strEmpID & "' AND T1.U_Z_AUser='" & oApplication.Company.UserName & "'"
                        otestRs.DoQuery(strQuery)
                        If otestRs.RecordCount > 0 Then
                            HeadDocEntry = otestRs.Fields.Item(0).Value
                            UserLineId = otestRs.Fields.Item(1).Value
                        End If
                        strQuery = "Select Top 1 U_Z_AUser From [@Z_HR_APPT2] Where  DocEntry = '" & HeadDocEntry & "' And LineId > '" & UserLineId.ToString() & "' and isnull(U_Z_AMan,'')='Y'  Order By LineId Asc "
                        oRecordSet.DoQuery(strQuery)
                        If Not oRecordSet.EoF Then
                            NextApprover = oRecordSet.Fields.Item(0).Value
                        End If
                        strQuery = "Select * from [@Z_HR_APHIS] where U_Z_DocEntry='" & strDocEntry & "' and U_Z_DocType='ExpCli' and U_Z_ApproveBy='" & oApplication.Company.UserName & "'"
                        oRecordSet.DoQuery(strQuery)
                        If oRecordSet.RecordCount > 0 Then
                            oGeneralParams.SetProperty("DocEntry", oRecordSet.Fields.Item("DocEntry").Value)
                            oGeneralData = oGeneralService.GetByParams(oGeneralParams)
                            oGeneralData.SetProperty("U_Z_AppStatus", oGrid.DataTable.GetValue("U_Z_AppStatus", index))
                            oGeneralData.SetProperty("U_Z_Remarks", oGrid.DataTable.GetValue("U_Z_Remarks", index))
                            oGeneralData.SetProperty("U_Z_Month", oGrid.DataTable.GetValue("U_Z_Month", index))
                            oGeneralData.SetProperty("U_Z_Year", oGrid.DataTable.GetValue("U_Z_Year", index))
                            oGeneralData.SetProperty("U_Z_ADocEntry", HeadDocEntry)
                            oGeneralData.SetProperty("U_Z_ALineId", UserLineId)
                            oGeneralData.SetProperty("U_Z_NextApprover", NextApprover)
                            Dim oTemp As SAPbobsCOM.Recordset
                            oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            oTemp.DoQuery("Select * ,isnull(""firstName"",'') +  ' ' + isnull(""middleName"",'') +  ' ' + isnull(""lastName"",'') 'EmpName' from OHEM where ""userid""=" & oApplication.Company.UserSignature)
                            If oTemp.RecordCount > 0 Then
                                oGeneralData.SetProperty("U_Z_EmpId", oTemp.Fields.Item("empID").Value.ToString())
                                oGeneralData.SetProperty("U_Z_EmpName", oTemp.Fields.Item("EmpName").Value)
                                strEmpName = oTemp.Fields.Item("EmpName").Value
                            Else
                                oGeneralData.SetProperty("U_Z_EmpId", "")
                                oGeneralData.SetProperty("U_Z_EmpName", "")
                            End If
                            oGeneralService.Update(oGeneralData)
                        ElseIf (strDocEntry <> "" And strDocEntry <> "0") Then
                            Dim oTemp As SAPbobsCOM.Recordset
                            oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            oTemp.DoQuery("Select * ,isnull(""firstName"",'') + ' ' + isnull(""middleName"",'') +  ' ' + isnull(""lastName"",'') 'EmpName' from OHEM where ""userid""=" & oApplication.Company.UserSignature)
                            If oTemp.RecordCount > 0 Then
                                oGeneralData.SetProperty("U_Z_EmpId", oTemp.Fields.Item("empID").Value.ToString())
                                oGeneralData.SetProperty("U_Z_EmpName", oTemp.Fields.Item("EmpName").Value)
                                strEmpName = oTemp.Fields.Item("EmpName").Value
                            Else
                                oGeneralData.SetProperty("U_Z_EmpId", "")
                                oGeneralData.SetProperty("U_Z_EmpName", "")
                            End If
                            oGeneralData.SetProperty("U_Z_DocEntry", strDocEntry.ToString())
                            oGeneralData.SetProperty("U_Z_DocType", "ExpCli")
                            oGeneralData.SetProperty("U_Z_AppStatus", oGrid.DataTable.GetValue("U_Z_AppStatus", index))
                            oGeneralData.SetProperty("U_Z_Remarks", oGrid.DataTable.GetValue("U_Z_Remarks", index))
                            oGeneralData.SetProperty("U_Z_Month", oGrid.DataTable.GetValue("U_Z_Month", index))
                            oGeneralData.SetProperty("U_Z_Year", oGrid.DataTable.GetValue("U_Z_Year", index))
                            oGeneralData.SetProperty("U_Z_ApproveBy", oApplication.Company.UserName)
                            oGeneralData.SetProperty("U_Z_Approvedt", System.DateTime.Now)
                            oGeneralData.SetProperty("U_Z_ADocEntry", HeadDocEntry)
                            oGeneralData.SetProperty("U_Z_ALineId", UserLineId)
                            oGeneralData.SetProperty("U_Z_NextApprover", NextApprover)
                            oGeneralService.Add(oGeneralData)
                        End If
                        updateFinalStatus(aForm, HeadDocEntry, strDocEntry, strEmpID, oGrid.DataTable.GetValue("U_Z_AppStatus", index), oGrid.DataTable.GetValue("U_Z_Year", index), oGrid.DataTable.GetValue("U_Z_Month", index), oGrid.DataTable.GetValue("U_Z_Remarks", index))
                        If oGrid.DataTable.GetValue("U_Z_AppStatus", index) = "A" Then
                            If MailDocEntry = "" Then
                                MailDocEntry = strDocEntry
                            Else
                                MailDocEntry = MailDocEntry & "," & strDocEntry
                            End If
                        End If
                    End If
                Next
            End If
           
            If MailDocEntry <> "" Then
                SendMessage(HeaderCode, HeadDocEntry, strEmpName, oApplication.Company.UserName, MailDocEntry)
            End If

            oGrid1 = aForm.Items.Item("1").Specific
            For index1 As Integer = 0 To oGrid1.DataTable.Rows.Count - 1
                If oGrid1.Rows.IsSelected(index1) Then
                    strEmpID = oGrid1.DataTable.GetValue("U_Z_EmpID", index1)
                    strDocEntry = oGrid1.DataTable.GetValue("Code", index1)
                    sQuery = " Select T2.DocEntry "
                    sQuery += " From [@Z_HR_APPT2] T2 "
                    sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
                    sQuery += " JOIN [@Z_HR_APPT1] T4 ON T4.DocEntry = T3.DocEntry  "
                    sQuery += " Where T4.U_Z_Ouser='" & strEmpID & "' and  U_Z_AFinal = 'Y'"
                    sQuery += " And T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = 'ExpCli'"
                    oRecordSet.DoQuery(sQuery)
                    If Not oRecordSet.EoF Then
                        oCombobox = aForm.Items.Item("24").Specific
                        sQuery = "Update [@Z_HR_OEXPCL] set U_Z_DocStatus='" & oCombobox.Selected.Value & "' where Code='" & strDocEntry & "'"
                        oRecordSet.DoQuery(sQuery)
                    End If
                End If
            Next
            LoadBind(aForm)
            LoadSummary(aForm)
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
    Private Sub updateFinalStatus(ByVal aForm As SAPbouiCOM.Form, ByVal strTemplateNo As String, ByVal strDocEntry As String, ByVal aEmpID As String, ByVal strStatus As String, ByVal strYear As Integer, ByVal IntMonth As Integer, ByVal Remarks As String)
        Try
            Dim intLineID As Integer
            Dim strMessageUser, StrMailMessage, strClaimDocNo As String
            Dim oRecordSet, oTemp As SAPbobsCOM.Recordset
            oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTemp.DoQuery("Select * from [@Z_HR_EXPCL] where Code='" & strDocEntry & "'")
            If oTemp.RecordCount > 0 Then
                strClaimDocNo = oTemp.Fields.Item("U_Z_DocRefNo").Value
            Else
                strClaimDocNo = strDocEntry
            End If

            If strStatus = "A" Then
                sQuery = "Select LineId From [@Z_HR_APPT2] Where DocEntry = '" & strTemplateNo & "' And U_Z_AUser = '" & oApplication.Company.UserName & "'"
                oRecordSet.DoQuery(sQuery)
                If Not oRecordSet.EoF Then
                    intLineID = CInt(oRecordSet.Fields.Item(0).Value)
                    sQuery = "Select Top 1 U_Z_AUser From [@Z_HR_APPT2] Where  DocEntry = '" & strTemplateNo & "' And LineId > '" & intLineID.ToString() & "' and isnull(U_Z_AMan,'')='Y'  Order By LineId Asc "
                    oRecordSet.DoQuery(sQuery)
                    If Not oRecordSet.EoF Then
                        strMessageUser = oRecordSet.Fields.Item(0).Value
                        sQuery = "Update [@Z_HR_EXPCL] set U_Z_CurApprover='" & oApplication.Company.UserName & "',U_Z_NxtApprover='" & strMessageUser & "' where Code='" & strDocEntry & "'"
                        oTemp.DoQuery(sQuery)
                    End If
                End If

                sQuery = " Select T2.DocEntry "
                sQuery += " From [@Z_HR_APPT2] T2 "
                sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
                sQuery += " JOIN [@Z_HR_APPT1] T4 ON T4.DocEntry = T3.DocEntry  "
                sQuery += " Where T4.U_Z_Ouser='" & aEmpID & "' and  U_Z_AFinal = 'Y'"
                sQuery += " And T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = 'ExpCli'"
                oRecordSet.DoQuery(sQuery)
                If Not oRecordSet.EoF Then
                    'sQuery = "Update [@Z_HR_EXPCL] Set U_Z_Year=" & strYear & ",U_Z_Month=" & IntMonth & ", U_Z_AppStatus = 'A' Where Code = '" + strDocEntry + "'"
                    'oRecordSet.DoQuery(sQuery)
                    'oApplication.Utilities.AddtoUDT1_PayrollTrans(strDocEntry)

                    Try
                        'If oApplication.Company.InTransaction() Then
                        '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                        'End If
                        'oApplication.Company.StartTransaction()
                        Dim blnvalue As Boolean
                        sQuery = "Update [@Z_HR_EXPCL] Set U_Z_Year=" & strYear & ",U_Z_Month=" & IntMonth & ", U_Z_AppStatus = 'A' Where Code = '" + strDocEntry + "'"
                        oRecordSet.DoQuery(sQuery)
                        sQuery = "Select isnull(U_Z_Posting,'P') as U_Z_Posting,U_Z_Reimburse from [@Z_HR_EXPCL] where Code='" & strDocEntry & "'"
                        oTemp.DoQuery(sQuery)
                        If oTemp.RecordCount > 0 Then
                            Dim Posting, Reimbused As String

                            Posting = oTemp.Fields.Item("U_Z_Posting").Value
                            Reimbused = oTemp.Fields.Item("U_Z_Reimburse").Value
                            If Posting = "P" And Reimbused = "Y" Then
                                oApplication.Utilities.AddtoUDT1_PayrollTrans(strDocEntry)
                            ElseIf Posting = "G" Then
                                oApplication.Utilities.CreateJournelVouchers(strDocEntry, Reimbused)
                            End If
                        End If
                      
                        'StrMailMessage = "Expense claim request has been approved for the request number :" & CInt(strDocEntry)
                        StrMailMessage = "-	Expenses Claim request approved for the request code : " & CInt(strDocEntry) & " in the expense claim number: " & CInt(strClaimDocNo)
                        oApplication.Utilities.SendMail_RequestApproval(StrMailMessage, aEmpID)
                    Catch ex As Exception
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        'If oApplication.Company.InTransaction() Then
                        '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                        'End If
                    End Try

                End If
            ElseIf strStatus = "R" Then
                sQuery = " Select T2.DocEntry "
                sQuery += " From [@Z_HR_APPT2] T2 "
                sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
                sQuery += " JOIN [@Z_HR_APPT1] T4 ON T4.DocEntry = T3.DocEntry  "
                sQuery += " Where T4.U_Z_Ouser='" & aEmpID & "'"
                sQuery += " And T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = 'ExpCli'"
                oRecordSet.DoQuery(sQuery)
                If Not oRecordSet.EoF Then
                    sQuery = "Update [@Z_HR_EXPCL] Set U_Z_RejRemark='" & Remarks & "', U_Z_AppStatus = 'R' Where Code = '" + strDocEntry + "'"
                    oRecordSet.DoQuery(sQuery)
                    ' StrMailMessage = "Expense claim request has been Rejected for the request number :" & CInt(strDocEntry)
                    StrMailMessage = "Expenses Claim request rejected for the request code : " & CInt(strDocEntry) & " in the expense claim number: " & CInt(strClaimDocNo)
                    oApplication.Utilities.SendMail_RequestApproval(StrMailMessage, aEmpID)
                End If
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
    Private Sub reDrawForm(ByVal oForm As SAPbouiCOM.Form)
        'Try
        'oForm.Freeze(True)
        'oForm.Items.Item("18").Width = oForm.Width - 25
        'oForm.Items.Item("18").Height = oForm.Height - 10
        '    Dim intHeight As Int32 = oForm.Items.Item("3").Top + oForm.Items.Item("3").Height
        '    oForm.Items.Item("27").Top = intHeight
        '    oForm.Items.Item("27").Left = oForm.Width - 300
        '    oForm.Items.Item("28").Left = oForm.Items.Item("27").Left + oForm.Items.Item("27").Width
        '    oForm.Items.Item("28").Top = oForm.Items.Item("27").Top
        '    oForm.Items.Item("31").Left = oForm.Items.Item("27").Left
        '    oForm.Items.Item("31").Top = oForm.Items.Item("27").Top + oForm.Items.Item("27").Height + 1
        '    oForm.Items.Item("32").Left = oForm.Items.Item("28").Left
        '    oForm.Items.Item("32").Top = oForm.Items.Item("31").Top
        '    oForm.Freeze(False)
        'Catch ex As Exception
        oForm.Freeze(False)
        'End Try
    End Sub
    Public Function ApprovalValidation(ByVal aform As SAPbouiCOM.Form) As Boolean
        Try
            oGrid = aform.Items.Item("3").Specific
            For index As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                If oGrid.DataTable.GetValue("U_Z_AppStatus", index) = "R" Then
                    If oGrid.DataTable.GetValue("U_Z_Remarks", index) = "" Then
                        oApplication.Utilities.Message("Remarks is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                ElseIf oGrid.DataTable.GetValue("U_Z_AppStatus", index) = "A" Then
                    Dim strMonth, strYear As String
                    Try
                        strMonth = oGrid.DataTable.GetValue("U_Z_Month", index)
                    Catch ex As Exception
                        strMonth = ""
                    End Try
                    Try
                        strYear = oGrid.DataTable.GetValue("U_Z_Year", index)
                    Catch ex As Exception
                        strYear = ""
                    End Try

                    If strMonth = "" Or strMonth = "0" Then
                        oApplication.Utilities.Message("Month is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    ElseIf strYear = "" Or strYear = "0" Then
                        oApplication.Utilities.Message("Year is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                    Dim strEMPID As String
                    oGrid = aform.Items.Item("3").Specific
                    Dim Posting As String = oGrid.DataTable.GetValue("U_Z_Posting", index)
                    If oGrid.DataTable.GetValue("U_Z_Posting", index) = "Payroll" Then
                        Dim orec As SAPbobsCOM.Recordset
                        orec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        Try
                            sQuery = "Select * from [@Z_PAYROLL1] where U_Z_empID='" & oGrid.DataTable.GetValue("U_Z_EmpID", index) & "' and U_Z_Month='" & strMonth & "' and U_Z_Year='" & strYear & "' and U_Z_Posted='Y'"
                            orec.DoQuery(sQuery)
                            If orec.RecordCount > 0 Then
                                oApplication.Utilities.Message("Payroll already posted for this month and year.", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                Return False
                            End If
                        Catch ex As Exception
                            Return True
                        End Try
                    End If
                End If
            Next
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function

    Public Sub SendMessage(ByVal strReqNo As String, ByVal strTemplateNo As String, ByVal strOrginator As String, ByVal strAuthorizer As String, ByVal MailDocEntry As String)
        Try
            Dim strQuery As String
            Dim strMessageUser As String
            Dim strEmailMessage As String = ""
            Dim intLineID As Integer
            Dim oRecordSet, oTemp As SAPbobsCOM.Recordset
            Dim oCmpSrv As SAPbobsCOM.CompanyService
            Dim oMessageService As SAPbobsCOM.MessagesService
            Dim oMessage As SAPbobsCOM.Message
            Dim pMessageDataColumns As SAPbobsCOM.MessageDataColumns
            Dim pMessageDataColumn As SAPbobsCOM.MessageDataColumn
            Dim oLines As SAPbobsCOM.MessageDataLines
            Dim oLine As SAPbobsCOM.MessageDataLine
            Dim oRecipientCollection As SAPbobsCOM.RecipientCollection
            oCmpSrv = oApplication.Company.GetCompanyService()
            oMessageService = oCmpSrv.GetBusinessService(SAPbobsCOM.ServiceTypes.MessagesService)
            oMessage = oMessageService.GetDataInterface(SAPbobsCOM.MessagesServiceDataInterfaces.msdiMessage)
            oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strQuery = "Select LineId From [@Z_HR_APPT2] Where DocEntry = '" & strTemplateNo & "' And U_Z_AUser = '" & strAuthorizer & "'"
            oRecordSet.DoQuery(strQuery)
            If Not oRecordSet.EoF Then
                intLineID = CInt(oRecordSet.Fields.Item(0).Value)
                strQuery = "Select Top 1 U_Z_AUser From [@Z_HR_APPT2] Where  DocEntry = '" & strTemplateNo & "' And LineId > '" & intLineID.ToString() & "' and isnull(U_Z_AMan,'')='Y'  Order By LineId Asc "
                oRecordSet.DoQuery(strQuery)

                If Not oRecordSet.EoF Then
                    strMessageUser = oRecordSet.Fields.Item(0).Value
                    oMessage.Subject = "Expense Claim" & ":" & " Need Your Approval "
                    Dim strMessage As String = ""
                    strQuery = "Select * from  [@Z_HR_OEXPCL]  where Code ='" & strReqNo & "'"
                    oTemp.DoQuery(strQuery)
                    strMessage = " Requested by  :" & oTemp.Fields.Item("U_Z_EmpName").Value
                    strOrginator = strMessage
                    oMessage.Text = "Expense Claim No:" & " " & CInt(strReqNo).ToString() & " with Expenses : " & CInt(MailDocEntry).ToString() & strOrginator & " Needs Your Approval "
                    oRecipientCollection = oMessage.RecipientCollection
                    oRecipientCollection.Add()
                    oRecipientCollection.Item(0).SendInternal = SAPbobsCOM.BoYesNoEnum.tYES
                    oRecipientCollection.Item(0).UserCode = strMessageUser
                    pMessageDataColumns = oMessage.MessageDataColumns
                    pMessageDataColumn = pMessageDataColumns.Add()
                    pMessageDataColumn.ColumnName = "Request No"
                    oLines = pMessageDataColumn.MessageDataLines()
                    oLine = oLines.Add()
                    oLine.Value = MailDocEntry
                    oMessageService.SendMessage(oMessage)
                    strEmailMessage = "Expense Claim No:" & " " & CInt(strReqNo).ToString() & " with Expenses : " & CInt(MailDocEntry).ToString() & strOrginator & " Needs Your Approval "
                    oApplication.Utilities.SendMail_Approval(strEmailMessage, strMessageUser, strMessageUser)
                End If
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub ValidateDocStatus(ByVal aForm As SAPbouiCOM.Form, ByVal aEmpID As String)
        Dim oRecordSet, oTemp As SAPbobsCOM.Recordset
        oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        sQuery = " Select T2.DocEntry "
        sQuery += " From [@Z_HR_APPT2] T2 "
        sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
        sQuery += " JOIN [@Z_HR_APPT1] T4 ON T4.DocEntry = T3.DocEntry  "
        sQuery += " Where T4.U_Z_Ouser='" & aEmpID & "' and  U_Z_AFinal = 'Y'"
        sQuery += " And T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = 'ExpCli'"
        oRecordSet.DoQuery(sQuery)
        If oRecordSet.RecordCount > 0 Then
            aForm.Items.Item("24").Enabled = True
        Else
            aForm.Items.Item("10").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            Try
                aForm.Items.Item("24").Enabled = False
            Catch ex As Exception
            End Try
        End If
    End Sub
    Private Sub EnableDisable(ByVal aForm As SAPbouiCOM.Form, ByVal Status As String)
        oGrid = aForm.Items.Item("3").Specific
        If Status = "C" Then
            aForm.Items.Item("22").Enabled = False
            oGrid.Columns.Item("U_Z_Month").Editable = False
            oGrid.Columns.Item("U_Z_Year").Editable = False
            oGrid.Columns.Item("U_Z_Remarks").Editable = False
            oGrid.Columns.Item("U_Z_AppStatus").Editable = False
        Else
            aForm.Items.Item("22").Enabled = True
            oGrid.Columns.Item("U_Z_Month").Editable = True
            oGrid.Columns.Item("U_Z_Year").Editable = True
            oGrid.Columns.Item("U_Z_Remarks").Editable = True
            oGrid.Columns.Item("U_Z_AppStatus").Editable = True
        End If
    End Sub
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_ClaimApproval Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "_1" Then
                                    If ApprovalValidation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If (pVal.ItemUID = "3" Or pVal.ItemUID = "20") And pVal.ColUID = "Code" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    oGrid.Columns.Item("RowsHeader").Click(pVal.Row)
                                    Dim strDocEntry As String = oGrid.DataTable.GetValue("Code", pVal.Row)
                                    Dim objHistory As New clshrAppHisDetails
                                    objHistory.LoadForm(oForm, HistoryDoctype.ExpCli, strDocEntry)
                                End If
                        End Select
                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                reDrawForm(oForm)
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "8" Then
                                    oCombobox = oForm.Items.Item("8").Specific
                                    If oCombobox.Selected.Value = "A" Then
                                        oForm.Items.Item("13").Enabled = True
                                        oForm.Items.Item("15").Enabled = True
                                        ComboSelect(oForm, oCombobox.Selected.Value)
                                    Else

                                        oForm.Items.Item("13").Enabled = False
                                        oForm.Items.Item("15").Enabled = False
                                        ComboSelect(oForm, oCombobox.Selected.Value)
                                    End If
                                End If
                                If pVal.ItemUID = "22" Then
                                    oForm.Freeze(True)
                                    oCombobox = oForm.Items.Item("22").Specific
                                    oGrid = oForm.Items.Item("3").Specific
                                    If oCombobox.Selected.Value <> "" Then
                                        For intRow As Integer = 0 To oGrid.Rows.Count - 1
                                            oGrid.DataTable.SetValue("U_Z_AppStatus", intRow, oCombobox.Selected.Value)
                                        Next
                                    End If
                                    oForm.Freeze(False)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If (pVal.ItemUID = "3" Or pVal.ItemUID = "20") And pVal.ColUID = "U_Z_Attachment" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    oGrid.Columns.Item("RowsHeader").Click(pVal.Row)
                                    LoadFiles(oForm, pVal.ItemUID)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If (pVal.ItemUID = "3" Or pVal.ItemUID = "20") And pVal.ColUID = "U_Z_ExpType" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    For intRow As Integer = pVal.Row To pVal.Row
                                        If 1 = 1 Then
                                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                            Dim objct As New clshrExpenses
                                            objct.LoadForm()
                                        End If
                                    Next
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                If oForm.State = SAPbouiCOM.BoFormStateEnum.fs_Restore Or oForm.State = SAPbouiCOM.BoFormStateEnum.fs_Maximized Then
                                    oApplication.Utilities.Resize(oForm)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "16" Then
                                    oForm.PaneLevel = 1
                                    oGrid = oForm.Items.Item("1").Specific
                                    oGrid.Columns.Item("RowsHeader").Click(0)
                                ElseIf pVal.ItemUID = "17" Then
                                    oForm.PaneLevel = 2
                                    oGrid = oForm.Items.Item("19").Specific
                                    oGrid.Columns.Item("RowsHeader").Click(0)
                                End If
                                If (pVal.ItemUID = "1" And pVal.ColUID = "RowsHeader" And pVal.Row > -1) Then
                                    oGrid = oForm.Items.Item("1").Specific
                                    Dim strDocEntry As String = oGrid.DataTable.GetValue("Code", pVal.Row)
                                    oApplication.Utilities.setEdittextvalue(oForm, "6", strDocEntry)
                                    oForm.Freeze(True)
                                    oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    InitializationApproval(oForm, strDocEntry)
                                    sQuery = " select isnull(T0.U_Z_DocStatus,'O'),U_Z_EmpID  from [@Z_HR_OEXPCL] T0 where T0.Code ='" & strDocEntry & "'"
                                    oRec.DoQuery(sQuery)
                                    If oRec.RecordCount > 0 Then
                                        EnableDisable(oForm, oRec.Fields.Item(0).Value)

                                        ValidateDocStatus(oForm, oRec.Fields.Item(1).Value)
                                        oCombobox = oForm.Items.Item("24").Specific
                                        oCombobox.Select(oRec.Fields.Item(0).Value, SAPbouiCOM.BoSearchKey.psk_ByValue)

                                    End If

                                   
                                    oForm.Freeze(False)
                                    'oApplication.Utilities.LoadHistory(oForm, HistoryDoctype.ExpCli, strDocEntry)
                                ElseIf (pVal.ItemUID = "3" And pVal.ColUID = "RowsHeader" And pVal.Row > -1) Then
                                    'oApplication.Utilities.LoadStatusRemarks(oForm, pVal.Row)
                                    'oApplication.Utilities.LoadLeaveRemarks(oForm, pVal.Row)
                                ElseIf pVal.ItemUID = "_1" Then
                                    Dim intRet As Integer = oApplication.SBO_Application.MessageBox("Are you sure want to submit the document?", 2, "Yes", "No", "")
                                    If intRet = 1 Then
                                        'oApplication.Utilities.addUpdateDocument(oForm, HistoryDoctype.ExpCli, HeaderDoctype.ExpCli)
                                        addUpdateDocument(oForm)
                                    End If
                                End If
                                If pVal.ItemUID = "19" And pVal.ColUID = "RowsHeader" And pVal.Row > -1 Then
                                    oGrid = oForm.Items.Item("19").Specific
                                    Dim strDocEntry As String = oGrid.DataTable.GetValue("Code", pVal.Row)
                                    oForm.Freeze(True)
                                    ApprovalSummary(oForm, strDocEntry)
                                    oForm.Freeze(False)
                                End If
                        End Select
                End Select
            End If
        Catch ex As Exception
            oForm.Freeze(False)
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
#End Region

#Region "Menu Event"
    Public Overrides Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            Select Case pVal.MenuUID
                Case mnu_hr_ExpApproval
                    LoadForm(oForm)
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
