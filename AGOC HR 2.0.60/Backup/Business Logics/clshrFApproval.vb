Public Class clshrFApproval
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oGrid As SAPbouiCOM.Grid
    Private oGrid_P1 As SAPbouiCOM.Grid
    Private oGrid_P2 As SAPbouiCOM.Grid
    Private oGrid_P3 As SAPbouiCOM.Grid
    Private oGrid_P4 As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Private oDtAppraisal As SAPbouiCOM.DataTable
    Private oRecordSet As SAPbobsCOM.Recordset
    Private sQuery As String
    Private oHTUpdateCol As Hashtable

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub

#Region "LoadForm"

    Public Sub LoadForm(ByVal strtitle As String, Optional ByVal empid As String = "", Optional ByVal FEmp As String = "", Optional ByVal TEmp As String = "", Optional ByVal Dept As String = "", Optional ByVal Period As String = "")
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_FApproval) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_FApproval, frm_hr_FApproval)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Items.Item("30").Visible = False
        oForm.Items.Item("31").Visible = False
        oForm.Items.Item("32").Visible = False
        oForm.Items.Item("_3").Visible = False


        oForm.Items.Item("45").Visible = False
        oForm.Items.Item("46").Visible = False

        Dim ostatic As SAPbouiCOM.StaticText
        ostatic = oForm.Items.Item("351").Specific
        ostatic.Caption = ""
        If strtitle = "HR" Then
            oForm.Title = "HR Acceptance"
        ElseIf strtitle = "Self" Then
            oForm.Title = "Self Appraisals"
        ElseIf strtitle = "SMgrApp" Then
            oForm.Title = "Second Level Approval"
        Else
            oForm.Title = "First Level Approval"
        End If

        oForm.Freeze(True)
        FillStatusCombo()
        FillAcceptanceCombo()
        oForm.DataSources.UserDataSources.Add("SChkStatus", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
        oForm.DataSources.UserDataSources.Add("LChkStatus", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
        oForm.DataSources.UserDataSources.Add("SChkSts", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
        oForm.DataSources.UserDataSources.Add("HChkStatus", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
        Dim oChk As SAPbouiCOM.CheckBox
        oChk = oForm.Items.Item("39").Specific
        oChk.DataBind.SetBound(True, "", "SChkStatus")
        oChk = oForm.Items.Item("40").Specific
        oChk.DataBind.SetBound(True, "", "LChkStatus")
        oChk = oForm.Items.Item("41").Specific
        oChk.DataBind.SetBound(True, "", "SChkSts")
        oChk = oForm.Items.Item("42").Specific
        oChk.DataBind.SetBound(True, "", "HChkStatus")

        oForm.DataSources.UserDataSources.Add("SP1", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
        oForm.DataSources.UserDataSources.Add("SP2", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
        oForm.DataSources.UserDataSources.Add("SP3", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
        oForm.DataSources.UserDataSources.Add("LP1", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
        oForm.DataSources.UserDataSources.Add("LP2", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
        oForm.DataSources.UserDataSources.Add("LP3", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
        oForm.DataSources.UserDataSources.Add("SMP1", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
        oForm.DataSources.UserDataSources.Add("SMP2", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
        oForm.DataSources.UserDataSources.Add("SMP3", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
        oForm.DataSources.UserDataSources.Add("HP1", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
        oForm.DataSources.UserDataSources.Add("HP2", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
        oForm.DataSources.UserDataSources.Add("HP3", SAPbouiCOM.BoDataType.dt_LONG_TEXT)

        oForm.DataSources.UserDataSources.Add("GStatus", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("GDate", SAPbouiCOM.BoDataType.dt_DATE)
        oForm.DataSources.UserDataSources.Add("GNo", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
        oForm.DataSources.UserDataSources.Add("GRemarks", SAPbouiCOM.BoDataType.dt_LONG_TEXT)

        oForm.DataSources.UserDataSources.Add("INUSER", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("INDate", SAPbouiCOM.BoDataType.dt_DATE)
        oForm.DataSources.UserDataSources.Add("INTime", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)

        oForm.DataSources.UserDataSources.Add("SFUSER", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("SFDate", SAPbouiCOM.BoDataType.dt_DATE)
        oForm.DataSources.UserDataSources.Add("SFTime", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("SFAUSER", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("SFADate", SAPbouiCOM.BoDataType.dt_DATE)
        oForm.DataSources.UserDataSources.Add("SFATime", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("FLUSER", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("FLDate", SAPbouiCOM.BoDataType.dt_DATE)
        oForm.DataSources.UserDataSources.Add("FLTime", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("SLUSER", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("SLDate", SAPbouiCOM.BoDataType.dt_DATE)
        oForm.DataSources.UserDataSources.Add("SLTime", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("HRUSER", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("HRDate", SAPbouiCOM.BoDataType.dt_DATE)
        oForm.DataSources.UserDataSources.Add("HRTime", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)

        oApplication.Utilities.setUserDatabind(oForm, "15", "SP1")
        oApplication.Utilities.setUserDatabind(oForm, "16", "SP2")
        oApplication.Utilities.setUserDatabind(oForm, "17", "SP3")
        oApplication.Utilities.setUserDatabind(oForm, "18", "LP1")
        oApplication.Utilities.setUserDatabind(oForm, "19", "LP2")
        oApplication.Utilities.setUserDatabind(oForm, "20", "LP3")
        oApplication.Utilities.setUserDatabind(oForm, "21", "SMP1")
        oApplication.Utilities.setUserDatabind(oForm, "22", "SMP2")
        oApplication.Utilities.setUserDatabind(oForm, "23", "SMP3")
        oApplication.Utilities.setUserDatabind(oForm, "24", "HP1")
        oApplication.Utilities.setUserDatabind(oForm, "25", "HP2")
        oApplication.Utilities.setUserDatabind(oForm, "26", "HP3")
        oApplication.Utilities.setUserDatabind(oForm, "36", "GDate")
        oApplication.Utilities.setUserDatabind(oForm, "37", "GNo")
        oApplication.Utilities.setUserDatabind(oForm, "38", "GRemarks")

        oApplication.Utilities.setUserDatabind(oForm, "50", "SFUSER")
        oApplication.Utilities.setUserDatabind(oForm, "60", "SFDate")
        oApplication.Utilities.setUserDatabind(oForm, "70", "SFTime")
        oApplication.Utilities.setUserDatabind(oForm, "52", "SFAUSER")
        oApplication.Utilities.setUserDatabind(oForm, "62", "SFADate")
        oApplication.Utilities.setUserDatabind(oForm, "72", "SFATime")
        oApplication.Utilities.setUserDatabind(oForm, "54", "FLUSER")
        oApplication.Utilities.setUserDatabind(oForm, "64", "FLDate")
        oApplication.Utilities.setUserDatabind(oForm, "74", "FLTime")
        oApplication.Utilities.setUserDatabind(oForm, "56", "SLUSER")
        oApplication.Utilities.setUserDatabind(oForm, "66", "SLDate")
        oApplication.Utilities.setUserDatabind(oForm, "76", "SLTime")
        oApplication.Utilities.setUserDatabind(oForm, "58", "HRUSER")
        oApplication.Utilities.setUserDatabind(oForm, "68", "HRDate")
        oApplication.Utilities.setUserDatabind(oForm, "78", "HRTime")
        oApplication.Utilities.setUserDatabind(oForm, "81", "INUSER")
        oApplication.Utilities.setUserDatabind(oForm, "83", "INDate")
        oApplication.Utilities.setUserDatabind(oForm, "84", "INTime")

        Databind(oForm, empid, oApplication.Company.UserSignature.ToString(), strtitle, FEmp, TEmp, Dept, Period)

        oForm.Items.Item("1000001").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        If strtitle = "HR" Then
            oForm.ActiveItem = "18"
            oForm.Items.Item("28").Enabled = False
            oForm.Items.Item("_3").Visible = True
        ElseIf strtitle = "Self" Then
            oForm.ActiveItem = "15"
            oForm.Items.Item("28").Enabled = False
        ElseIf strtitle = "SMgrApp" Then
            oForm.ActiveItem = "17"
            oForm.Items.Item("28").Enabled = False
        Else
            oForm.ActiveItem = "16"
            oForm.Items.Item("28").Enabled = False
        End If
        InitializeAppTable()
        reDrawForm(oForm)
        oForm.Freeze(False)
    End Sub

#End Region

#Region "Fill Combo"
    Private Sub FillStatusCombo()
        Dim oComboStatus As SAPbouiCOM.ComboBox
        Try
            oComboStatus = oForm.Items.Item("28").Specific
            For i As Integer = oComboStatus.ValidValues.Count - 1 To 0 Step -1
                oComboStatus.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index)
            Next
            oComboStatus.ValidValues.Add("SE", "SelfApproved")
            oComboStatus.ValidValues.Add("LM", "LineManager Approved")
            oComboStatus.ValidValues.Add("SM", "Sr.Manager Approved")
            oComboStatus.ValidValues.Add("HR", "HR Approved")
            oComboStatus.ValidValues.Add("DR", "Draft")
        Catch ex As Exception

        End Try
    End Sub

#End Region

#Region "FillAcceptanceCombo"
    Private Sub FillAcceptanceCombo()
        Dim oComboStatus As SAPbouiCOM.ComboBox
        Try
            oComboStatus = oForm.Items.Item("32").Specific
            For i As Integer = oComboStatus.ValidValues.Count - 1 To 0 Step -1
                oComboStatus.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index)
            Next
            oComboStatus.ValidValues.Add("-", "-")
            oComboStatus.ValidValues.Add("A", "Accepted")
            oComboStatus.ValidValues.Add("G", "Grevence")
            oComboStatus.Select("-", SAPbouiCOM.BoSearchKey.psk_ByValue)
        Catch ex As Exception

        End Try
    End Sub
#End Region

#Region "DataBind"

    Private Sub Databind(ByVal aform As SAPbouiCOM.Form, ByVal strempid As String, ByVal strUser As String, ByVal strtitle As String, Optional ByVal FEmp As String = "", Optional ByVal TEmp As String = "", Optional ByVal Dept As String = "", Optional ByVal Period As String = "")
        Dim strqry As String
        Dim isLevelStartFromLM As Boolean = False
        oForm.Items.Item("btnGra").Visible = False
        oForm = aform
        oGrid = oForm.Items.Item("3").Specific
        oGrid_P1 = oForm.Items.Item("8").Specific
        oGrid_P2 = oForm.Items.Item("9").Specific
        oGrid_P3 = oForm.Items.Item("10").Specific
        oGrid_P4 = oForm.Items.Item("46").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
        oGrid_P1.DataTable = oForm.DataSources.DataTables.Item("DT_1_P1")
        oGrid_P2.DataTable = oForm.DataSources.DataTables.Item("DT_2_P2")
        oGrid_P3.DataTable = oForm.DataSources.DataTables.Item("DT_3_P3")
        oGrid_P4.DataTable = oForm.DataSources.DataTables.Item("DT_4_P4")
        Dim oUserID As String = oApplication.Company.UserName
        Dim stremp As String = oApplication.Utilities.getEmpIDforMangers(oUserID)


        If strtitle = "MgrApp" Then
            stremp = oApplication.Utilities.getManagerEmPID(oUserID)
            strqry = " select DocEntry,U_Z_EmpId,U_Z_EmpName,U_Z_Date,U_Z_Period,case U_Z_Status when 'D' then 'Draft' when 'F' then 'Approved' when 'S'then '2nd Level Approval' when 'L' then 'Closed' else 'Canceled' end as U_Z_Status,case U_Z_WStatus when 'DR' then 'Draft' when 'HR' then 'HR Approved' when 'SM'then 'Sr.Manager Approved' when 'LM' then 'LineManager Approved'when 'SE' then 'SelfApproved'  end as 'U_Z_WStatus'   from [@Z_HR_OSEAPP] Where U_Z_EmpId in ( select empID from OHEM where manager in( " & stremp & "))"
        ElseIf strtitle = "SMgrApp" Then
            strqry = " select DocEntry,T0.U_Z_EmpId,U_Z_EmpName,U_Z_Date,T0.U_Z_Period,case U_Z_Status when 'D' then 'Draft' when 'F' then 'Approved' when 'S'then '2nd Level Approval' when 'L' then 'Closed' else 'Canceled' end as U_Z_Status,case U_Z_WStatus when 'DR' then 'Draft' when 'HR' then 'HR Approved' when 'SM'then 'Sr.Manager Approved' when 'LM' then 'LineManager Approved'when 'SE' then 'SelfApproved'  end as 'U_Z_WStatus'   from [@Z_HR_OSEAPP] T0 JOIN OHEM T1 On T0.U_Z_EmpID = T1.empID AND T1.Manager IN  (SELECT EmpId From OHEM Where UserID = " & strUser & "  Union (Select EmpId From OHEM Where Manager In (SELECT EmpId From OHEM Where UserID = " & strUser & ")))"
        ElseIf strtitle = "HR" Then
            strqry = "select DocEntry,U_Z_EmpId ,U_Z_EmpName,U_Z_Date,U_Z_Period,case U_Z_Status when 'D' then 'Draft' when 'F' then 'Approved' when 'S'then '2nd Level Approval' when 'L' then 'Closed' else 'Canceled' end as U_Z_Status,case U_Z_WStatus when 'DR' then 'Draft' when 'HR' then 'HR Approved' when 'SM'then 'Sr.Manager Approved' when 'LM' then 'LineManager Approved'when 'SE' then 'SelfApproved'  end as 'U_Z_WStatus' from [@Z_HR_OSEAPP] Where U_Z_Period='" & Period & "' "
            If Dept.Length > 0 Then
                strqry = strqry & "and U_Z_EmpID in (Select empId from OHEM where Dept='" & Dept & "')"
            End If
            If FEmp.Length > 0 And TEmp.Length > 0 Then
                strqry = strqry & " and ( U_Z_EmpId Between " & FEmp & " and " & TEmp & ")"
            End If
        Else
            If strempid <> "" Then
                strqry = "select DocEntry,U_Z_EmpId,U_Z_EmpName,U_Z_Date,U_Z_Period,case U_Z_Status when 'D' then 'Draft' when 'F' then 'Approved'"
                strqry = strqry & " when 'S'then '2nd Level Approval' when 'L' then 'Closed' else 'Canceled' end as U_Z_Status,case U_Z_WStatus when 'DR' then 'Draft' when 'HR' then 'HR Approved' when 'SM'then 'Sr.Manager Approved' when 'LM' then 'LineManager Approved'when 'SE' then 'SelfApproved'  end as 'U_Z_WStatus' from [@Z_HR_OSEAPP] where U_Z_EmpId='" & strempid & "'"
            Else
                strqry = "select DocEntry,U_Z_EmpId,U_Z_EmpName,U_Z_Date,U_Z_Period,case U_Z_Status when 'D' then 'Draft' when 'F' then 'Approved'"
                strqry = strqry & " when 'S'then '2nd Level Approval' when 'L' then 'Closed' else 'Canceled' end as U_Z_Status,case U_Z_WStatus when 'DR' then 'Draft' when 'HR' then 'HR Approved' when 'SM'then 'Sr.Manager Approved' when 'LM' then 'LineManager Approved'when 'SE' then 'SelfApproved'  end as 'U_Z_WStatus' from [@Z_HR_OSEAPP]"
            End If
        End If
        strqry = strqry & " Order by DocEntry desc"
        oGrid.DataTable.ExecuteQuery(strqry)

        Dim ostatic As SAPbouiCOM.StaticText
        ostatic = oForm.Items.Item("351").Specific
        ostatic.Caption = strqry
        oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Apprisal Number"
        oGrid.Columns.Item("DocEntry").Editable = False
        oEditTextColumn = oGrid.Columns.Item("DocEntry")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Invoice
        oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee Id"
        oGrid.Columns.Item("U_Z_EmpId").Editable = False
        oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
        oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
        oGrid.Columns.Item("U_Z_EmpName").Editable = False
        oGrid.Columns.Item("U_Z_Date").TitleObject.Caption = "Date"
        oGrid.Columns.Item("U_Z_Date").Editable = False
        oGrid.Columns.Item("U_Z_Period").TitleObject.Caption = "Period"
        oGrid.Columns.Item("U_Z_Period").Editable = False
        oGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Active"
        oGrid.Columns.Item("U_Z_Status").Visible = False
        oGrid.Columns.Item("U_Z_WStatus").TitleObject.Caption = "Status"
        oGrid.Columns.Item("U_Z_WStatus").Editable = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        If oGrid.Rows.Count > 0 Then
            'oGrid.Columns.Item("RowsHeader").Click(0)
            oGrid.Rows.SelectedRows.Add(0)
            Dim DocNo As Integer = Convert.ToInt32(oGrid.DataTable.GetValue("DocEntry", 0))
            If DocNo = 0 Then
                oApplication.Utilities.Message("No Records Found...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                'Return
                DocNo = 99999
            End If
            Dim StrQP0, StrQP1, StrQP2, StrQP3 As String
            StrQP0 = ""
            StrQP1 = ""
            StrQP2 = ""
            StrQP3 = ""
            StrQP0 = "Select U_Z_WStatus,U_Z_BSelfRemark,U_Z_BMgrRemark,U_Z_BSMrRemark,U_Z_BHrRemark, U_Z_PSelfRemark,U_Z_PMgrRemark,U_Z_PSMrRemark,U_Z_PHrRemark, U_Z_CSelfRemark,U_Z_CMgrRemark,U_Z_CSMrRemark,U_Z_CHrRemark from [@Z_HR_OSEAPP] Where DocEntry=" & DocNo & ""
            StrQP1 = "Select U_Z_BussCode as 'Code',U_Z_BussDesc as 'Business Objectives',U_Z_BussWeight as 'Weight (%)' ,T0.""U_Z_SelfRaCode"",U_Z_BussSelfRate as 'Self Rating',T0.""U_Z_MgrRaCode"",U_Z_BussMgrRate as 'Line Manager Rating',T0.""U_Z_SMRaCode"",U_Z_BussSMRate as 'Sr.Manager Rating' from [@Z_HR_SEAPP1] T0 Where DocEntry=" & DocNo & ""
            StrQP2 = "Select T0.U_Z_PeopleCode as 'Code',T0.U_Z_PeopleDesc as 'People Objectives',T2.U_Z_Remarks 'Emp Remarks',T0.U_Z_PeopleCat as 'Category',T0.U_Z_PeoWeight as 'Weight (%)',""U_Z_SelfRaCode"",T0.U_Z_PeoSelfRate as 'Self Rating',T0.""U_Z_MgrRaCode"",T0.U_Z_PeoMgrRate as 'Line Manager Rating',T0.""U_Z_SMRaCode"",T0.U_Z_PeoSMRate as 'Sr.Manager Rating' from [@Z_HR_SEAPP2] T0 Join [@Z_HR_OSEAPP] T1 ON T1.DocEntry = T0.DocEntry Left Outer Join [@Z_HR_PEOBJ1] T2 On T1.U_Z_EmpId = T2.U_Z_HREmpID  and T2.U_Z_HRPeoobjCode=T0.U_Z_PeopleCode Where T0.DocEntry = " & DocNo & ""
            StrQP3 = "Select T0.U_Z_CompCode as 'Code',T0.U_Z_CompDesc as 'Competence Objectives',T0.U_Z_CompWeight as 'Weight (%)',T0.U_Z_CompLevel as 'Levels',T2.U_Z_CompLevel As 'Current Level',""U_Z_SelfRaCode"",T0.U_Z_CompSelfRate as 'Self Rating',T0.""U_Z_MgrRaCode"",T0.U_Z_CompMgrRate as 'Line Manager Rating',T0.""U_Z_SMRaCode"",T0.U_Z_CompSMRate as 'Sr.Manager Rating' from [@Z_HR_SEAPP3] T0 Join [@Z_HR_OSEAPP] T1 ON T1.DocEntry = T0.DocEntry Left Outer Join [@Z_HR_ECOLVL] T2 On T1.U_Z_EmpId = T2.U_Z_HREmpID and T2.U_Z_CompCode =T0.U_Z_CompCode  Where T0.DocEntry = " & DocNo & ""

            oGrid_P1.DataTable.ExecuteQuery(StrQP1)
            oGrid_P2.DataTable.ExecuteQuery(StrQP2)
            oGrid_P3.DataTable.ExecuteQuery(StrQP3)
            Dim oRS As SAPbobsCOM.Recordset
            oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRS.DoQuery(StrQP0)

            Dim oRec As SAPbobsCOM.Recordset
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim strCStatus As String = ""
            Dim strChkS, strChkL, strChkSr, strChkH, strGHRAcct, strGSta, strWFStatus, strLStart, strStatus As String
            strCStatus = "Select U_Z_SCkApp,U_Z_LCkApp,U_Z_SrCkApp,U_Z_HrCkApp,U_Z_GHRSts,U_Z_GStatus,U_Z_WStatus,U_Z_LStrt,U_Z_Status from [@Z_HR_OSEAPP] where DocEntry=" & DocNo & ""
            oRec.DoQuery(strCStatus)
            If Not oRec.EoF Then
                strChkS = oRec.Fields.Item("U_Z_SCkApp").Value.ToString()
                strChkL = oRec.Fields.Item("U_Z_LCkApp").Value.ToString()
                strChkSr = oRec.Fields.Item("U_Z_SrCkApp").Value.ToString()
                strChkH = oRec.Fields.Item("U_Z_HrCkApp").Value.ToString()
                strGSta = oRec.Fields.Item("U_Z_GStatus").Value.ToString()
                strGHRAcct = oRec.Fields.Item("U_Z_GHRSts").Value.ToString()
                strWFStatus = oRec.Fields.Item("U_Z_WStatus").Value.ToString()
                strLStart = oRec.Fields.Item("U_Z_LStrt").Value.ToString()
                strStatus = oRec.Fields.Item("U_Z_Status").Value.ToString()
                If strLStart = "LM" Then
                    isLevelStartFromLM = True
                End If



            End If

            If oForm.Title = "Self Appraisals" Then
                If strChkS = "Y" Then
                    Dim oChk As SAPbouiCOM.CheckBox
                    oChk = oForm.Items.Item("39").Specific
                    oChk.Checked = True
                    oForm.Items.Item("39").Enabled = False
                    oForm.Items.Item("44").Enabled = False
                    oForm.Items.Item("29").Enabled = False
                ElseIf strGSta = "A" Then
                    oForm.Items.Item("31").Visible = True
                    oForm.Items.Item("32").Visible = True
                    oForm.Items.Item("32").Enabled = False
                    oForm.Items.Item("29").Enabled = False
                    oForm.Items.Item("44").Enabled = False
                End If

                If isLevelStartFromLM = True Then
                    oGrid_P1.Columns.Item("Self Rating").Editable = False
                    oGrid_P2.Columns.Item("Self Rating").Editable = False
                    oGrid_P3.Columns.Item("Self Rating").Editable = False

                    oGrid_P1.Columns.Item("U_Z_SMRaCode").Editable = False
                    oGrid_P2.Columns.Item("U_Z_SMRaCode").Editable = False
                    oGrid_P3.Columns.Item("U_Z_SMRaCode").Editable = False

                    oGrid_P1.Columns.Item("U_Z_MgrRaCode").Editable = False
                    oGrid_P2.Columns.Item("U_Z_MgrRaCode").Editable = False
                    oGrid_P3.Columns.Item("U_Z_MgrRaCode").Editable = False
                    oForm.Items.Item("29").Enabled = False
                    oForm.Items.Item("44").Enabled = False
                End If

                If strWFStatus <> "SE" And strWFStatus <> "DR" Then
                    oGrid_P1.Columns.Item("Self Rating").Editable = False
                    oGrid_P2.Columns.Item("Self Rating").Editable = False
                    oGrid_P3.Columns.Item("Self Rating").Editable = False
                    oGrid_P1.Columns.Item("U_Z_SelfRaCode").Editable = False
                    oGrid_P2.Columns.Item("U_Z_SelfRaCode").Editable = False
                    oGrid_P3.Columns.Item("U_Z_SelfRaCode").Editable = False

                    If strChkH = "Y" And strGSta = "-" Then
                        If isLevelStartFromLM Then
                            oForm.Items.Item("29").Enabled = False
                            oForm.Items.Item("44").Enabled = False
                        Else
                            oForm.Items.Item("29").Enabled = True
                            oForm.Items.Item("44").Enabled = True
                        End If

                    ElseIf strChkH = "N" And strGSta = "-" Then
                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("44").Enabled = False
                    ElseIf strChkH = "Y" And strGSta = "G" Then
                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("32").Enabled = False
                        oForm.Items.Item("44").Enabled = False
                    ElseIf strGSta = "-" Then
                        oForm.Items.Item("btnGra").Visible = True
                        '    'oForm.Items.Item("29").Enabled = True
                        '    'oForm.Items.Item("44").Enabled = True
                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("44").Enabled = False
                    Else

                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("44").Enabled = False
                    End If
                ElseIf strWFStatus = "SE" Or strWFStatus = "DR" Then
                    If isLevelStartFromLM Then
                        oGrid_P1.Columns.Item("Self Rating").Editable = False
                        oGrid_P2.Columns.Item("Self Rating").Editable = False
                        oGrid_P3.Columns.Item("Self Rating").Editable = False

                        oGrid_P1.Columns.Item("U_Z_SelfRaCode").Editable = False
                        oGrid_P2.Columns.Item("U_Z_SelfRaCode").Editable = False
                        oGrid_P3.Columns.Item("U_Z_SelfRaCode").Editable = False
                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("44").Enabled = False
                    Else
                        oGrid_P1.Columns.Item("Self Rating").Editable = True
                        oGrid_P2.Columns.Item("Self Rating").Editable = True
                        oGrid_P3.Columns.Item("Self Rating").Editable = True

                        oGrid_P1.Columns.Item("U_Z_SelfRaCode").Editable = True
                        oGrid_P2.Columns.Item("U_Z_SelfRaCode").Editable = True
                        oGrid_P3.Columns.Item("U_Z_SelfRaCode").Editable = True
                        oForm.Items.Item("29").Enabled = True
                        oForm.Items.Item("44").Enabled = True
                    End If


                ElseIf strWFStatus = "HR" And strChkH = "Y" And strGSta = "-" Then
                    oForm.Items.Item("32").Enabled = True
                    If isLevelStartFromLM Then
                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("44").Enabled = False
                    Else
                        oForm.Items.Item("29").Enabled = True
                        oForm.Items.Item("44").Enabled = True
                    End If
                End If

                ' oCombobox = oForm.Items.Item("28").Specific
                ' MsgBox(oCombobox.Selected.Value)
            ElseIf oForm.Title = "First Level Approval" Then
                If strChkL = "Y" Then
                    Dim oChk As SAPbouiCOM.CheckBox
                    oChk = oForm.Items.Item("40").Specific
                    oChk.Checked = True
                    oForm.Items.Item("40").Enabled = False
                    oForm.Items.Item("29").Enabled = False
                    oForm.Items.Item("44").Enabled = False
                Else
                    If strChkH = "Y" Then
                        Dim oChk As SAPbouiCOM.CheckBox
                        oChk = oForm.Items.Item("41").Specific
                        oChk.Checked = True
                        oForm.Items.Item("41").Enabled = False
                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("44").Enabled = False
                    End If
                End If
            ElseIf oForm.Title = "Second Level Approval" Then
                If strChkSr = "Y" Then
                    Dim oChk As SAPbouiCOM.CheckBox
                    oChk = oForm.Items.Item("41").Specific
                    oChk.Checked = True
                    oForm.Items.Item("41").Enabled = False
                    oForm.Items.Item("29").Enabled = False
                    oForm.Items.Item("44").Enabled = False
                Else
                    If strChkH = "Y" Then
                        Dim oChk As SAPbouiCOM.CheckBox
                        oChk = oForm.Items.Item("41").Specific
                        oChk.Checked = True
                        oForm.Items.Item("41").Enabled = False
                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("44").Enabled = False
                    End If

                End If
            ElseIf oForm.Title = "HR Acceptance" Then
                If strChkH = "Y" Then
                    Dim oChk As SAPbouiCOM.CheckBox
                    oChk = oForm.Items.Item("42").Specific
                    oChk.Checked = True
                    oForm.Items.Item("42").Enabled = False
                    oForm.Items.Item("29").Enabled = False
                    oForm.Items.Item("44").Enabled = False
                End If
            End If

            Dim strBSelfRMark, strBMgrRMark, strBSMrRMark, strBHrRMark, strPSelfRMark, strPMgrRMark, strPSMrRMark, strPHrRMark, strCSelfRMark, strCMgrRMark, strCSMrRMark, strCHrRMark, strWStatus As String
            strBSelfRMark = oRS.Fields.Item("U_Z_BSelfRemark").Value.ToString()
            strBMgrRMark = oRS.Fields.Item("U_Z_BMgrRemark").Value.ToString()
            strBSMrRMark = oRS.Fields.Item("U_Z_BSMrRemark").Value.ToString()
            strBHrRMark = oRS.Fields.Item("U_Z_BHrRemark").Value.ToString()
            strPSelfRMark = oRS.Fields.Item("U_Z_PSelfRemark").Value.ToString()
            strPMgrRMark = oRS.Fields.Item("U_Z_PMgrRemark").Value.ToString()
            strPSMrRMark = oRS.Fields.Item("U_Z_PSMrRemark").Value.ToString()
            strPHrRMark = oRS.Fields.Item("U_Z_PHrRemark").Value.ToString()
            strCSelfRMark = oRS.Fields.Item("U_Z_CSelfRemark").Value.ToString()
            strCMgrRMark = oRS.Fields.Item("U_Z_CMgrRemark").Value.ToString()
            strCSMrRMark = oRS.Fields.Item("U_Z_CSMrRemark").Value.ToString()
            strCHrRMark = oRS.Fields.Item("U_Z_CHrRemark").Value.ToString()
            strWStatus = oRS.Fields.Item("U_Z_WStatus").Value.ToString()
            oApplication.Utilities.setEdittextvalue(oForm, "15", strBSelfRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "16", strBMgrRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "17", strBSMrRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "18", strBHrRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "19", strPSelfRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "20", strPMgrRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "21", strPSMrRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "22", strPHrRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "23", strCSelfRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "24", strCMgrRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "25", strCSMrRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "26", strCHrRMark)

            oForm.ActiveItem = 28
            Dim oComboStatus As SAPbouiCOM.ComboBox
            oComboStatus = oForm.Items.Item("28").Specific
            Try
                oComboStatus.Select(strWStatus, SAPbouiCOM.BoSearchKey.psk_ByValue)

            Catch ex As Exception

            End Try

            Dim oComboGSta As SAPbouiCOM.ComboBox
            oComboGSta = oForm.Items.Item("32").Specific
            Try
                oComboGSta.Select(strGSta, SAPbouiCOM.BoSearchKey.psk_ByValue)

            Catch ex As Exception

            End Try

            colSum()

            oGrid_P2.Columns.Item("Emp Remarks").TitleObject.Caption = "Remarks"
            oGrid_P2.Columns.Item("Emp Remarks").Editable = False

            Dim oComboCol As SAPbouiCOM.ComboBoxColumn


            oGrid_P1.Columns.Item("U_Z_SelfRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oGrid_P1.Columns.Item("U_Z_MgrRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oGrid_P1.Columns.Item("U_Z_SMRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox


            If 1 = 1 Then

                sQuery = "Select U_Z_RateCode As Code,U_Z_RateName As Name From [@Z_HR_ORATE]"
                oRec.DoQuery(sQuery)
                oComboCol = oGrid_P1.Columns.Item("U_Z_SelfRaCode")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                oComboCol = oGrid_P1.Columns.Item("U_Z_MgrRaCode")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                oComboCol = oGrid_P1.Columns.Item("U_Z_SMRaCode")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
            End If


            oGrid_P2.Columns.Item("U_Z_SelfRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oGrid_P2.Columns.Item("U_Z_MgrRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oGrid_P2.Columns.Item("U_Z_SMRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox

            If 1 = 1 Then



                sQuery = "Select U_Z_RateCode As Code,U_Z_RateName As Name From [@Z_HR_ORATE]"
                oRec.DoQuery(sQuery)
                oComboCol = oGrid_P2.Columns.Item("U_Z_SelfRaCode")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                oComboCol = oGrid_P2.Columns.Item("U_Z_MgrRaCode")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                oComboCol = oGrid_P2.Columns.Item("U_Z_SMRaCode")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
            End If


            oGrid_P3.Columns.Item("Levels").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oGrid_P3.Columns.Item("Current Level").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            'oGrid_P3.Columns.Item("Self Rating").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            'oGrid_P3.Columns.Item("Line Manager Rating").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            'oGrid_P3.Columns.Item("Sr.Manager Rating").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox

            oGrid_P3.Columns.Item("U_Z_SelfRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oGrid_P3.Columns.Item("U_Z_MgrRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oGrid_P3.Columns.Item("U_Z_SMRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox


            oGrid_P1.Columns.Item("U_Z_SelfRaCode").TitleObject.Caption = "Self Rating"
            oGrid_P1.Columns.Item("U_Z_MgrRaCode").TitleObject.Caption = "First Level Manager Rating"
            oGrid_P1.Columns.Item("U_Z_SMRaCode").TitleObject.Caption = "Second Level Manager Rating"

            oGrid_P2.Columns.Item("U_Z_SelfRaCode").TitleObject.Caption = "Self Rating"
            oGrid_P2.Columns.Item("U_Z_MgrRaCode").TitleObject.Caption = "First Level Manager Rating"
            oGrid_P2.Columns.Item("U_Z_SMRaCode").TitleObject.Caption = "Second Level Manager Rating"


            oGrid_P3.Columns.Item("U_Z_SelfRaCode").TitleObject.Caption = "Self Rating"
            oGrid_P3.Columns.Item("U_Z_MgrRaCode").TitleObject.Caption = "First Level Manager Rating"
            oGrid_P3.Columns.Item("U_Z_SMRaCode").TitleObject.Caption = "Second Level Manager Rating"


            sQuery = "Select U_Z_LvelCode As Code,U_Z_LvelName As Name From [@Z_HR_COLVL]"
            oRec.DoQuery(sQuery)
            If Not oRec.EoF Then

                oComboCol = oGrid_P3.Columns.Item("Levels")
                oComboCol.ValidValues.Add("", "")
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                oComboCol = oGrid_P3.Columns.Item("Current Level")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                sQuery = "Select U_Z_RateCode As Code,U_Z_RateName As Name From [@Z_HR_ORATE]"
                oRec.DoQuery(sQuery)


                sQuery = "Select U_Z_RateCode As Code,U_Z_RateName As Name From [@Z_HR_ORATE]"
                oRec.DoQuery(sQuery)
                oComboCol = oGrid_P3.Columns.Item("U_Z_SelfRaCode")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                oComboCol = oGrid_P3.Columns.Item("U_Z_MgrRaCode")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                oComboCol = oGrid_P3.Columns.Item("U_Z_SMRaCode")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
            End If
            oGrid_P3.Columns.Item("Current Level").Editable = False
            Disable(strStatus)

            If oForm.Title = "HR Acceptance" Then
                oForm.Items.Item("8").Enabled = False
                oForm.Items.Item("9").Enabled = False
                oForm.Items.Item("10").Enabled = False
                oForm.Items.Item("16").Enabled = False
                oForm.Items.Item("20").Enabled = False
                oForm.Items.Item("24").Enabled = False
                oForm.Items.Item("17").Enabled = False
                oForm.Items.Item("21").Enabled = False
                oForm.Items.Item("25").Enabled = False
                oForm.Items.Item("15").Enabled = False
                oForm.Items.Item("19").Enabled = False
                oForm.Items.Item("23").Enabled = False
                oForm.Items.Item("39").Visible = False
                oForm.Items.Item("40").Visible = False
                oForm.Items.Item("41").Visible = False
                oForm.Items.Item("45").Visible = True
            ElseIf oForm.Title = "Self Appraisals" Then
                oGrid_P1.Columns.Item("Code").Editable = False
                oGrid_P2.Columns.Item("Code").Editable = False
                oGrid_P3.Columns.Item("Code").Editable = False
                oGrid_P1.Columns.Item("Business Objectives").Editable = False
                oGrid_P2.Columns.Item("People Objectives").Editable = False
                oGrid_P3.Columns.Item("Competence Objectives").Editable = False
                oGrid_P3.Columns.Item("Competence Objectives").TitleObject.Caption = "Competencies"
                oGrid_P1.Columns.Item("Weight (%)").Editable = False
                oGrid_P2.Columns.Item("Weight (%)").Editable = False
                oGrid_P3.Columns.Item("Weight (%)").Editable = False

                If isLevelStartFromLM Then
                    oGrid_P1.Columns.Item("Self Rating").Editable = False
                    oGrid_P2.Columns.Item("Self Rating").Editable = False
                    oGrid_P3.Columns.Item("Self Rating").Editable = False
                Else
                    oGrid_P1.Columns.Item("Self Rating").Editable = False
                    oGrid_P2.Columns.Item("Self Rating").Editable = False
                    oGrid_P3.Columns.Item("Self Rating").Editable = False
                    If strWFStatus <> "DR" Then
                        oGrid_P1.Columns.Item("U_Z_SelfRaCode").Editable = False
                        oGrid_P2.Columns.Item("U_Z_SelfRaCode").Editable = False
                        oGrid_P3.Columns.Item("U_Z_SelfRaCode").Editable = False
                    Else
                        oGrid_P1.Columns.Item("U_Z_SelfRaCode").Editable = True
                        oGrid_P2.Columns.Item("U_Z_SelfRaCode").Editable = True
                        oGrid_P3.Columns.Item("U_Z_SelfRaCode").Editable = True

                    End If

                End If



                oGrid_P1.Columns.Item("Line Manager Rating").Editable = False
                oGrid_P2.Columns.Item("Line Manager Rating").Editable = False
                oGrid_P3.Columns.Item("Line Manager Rating").Editable = False

                oGrid_P1.Columns.Item("Sr.Manager Rating").Editable = False
                oGrid_P2.Columns.Item("Sr.Manager Rating").Editable = False
                oGrid_P3.Columns.Item("Sr.Manager Rating").Editable = False

                oGrid_P1.Columns.Item("U_Z_SMRaCode").Editable = False
                oGrid_P2.Columns.Item("U_Z_SMRaCode").Editable = False
                oGrid_P3.Columns.Item("U_Z_SMRaCode").Editable = False

                oGrid_P1.Columns.Item("U_Z_MgrRaCode").Editable = False
                oGrid_P2.Columns.Item("U_Z_MgrRaCode").Editable = False
                oGrid_P3.Columns.Item("U_Z_MgrRaCode").Editable = False

                oGrid_P2.Columns.Item("Category").Editable = False
                oGrid_P3.Columns.Item("Levels").Editable = False
                oGrid_P3.Columns.Item("Levels").TitleObject.Caption = "Min expected level"
                oForm.Items.Item("16").Enabled = False
                oForm.Items.Item("20").Enabled = False
                oForm.Items.Item("24").Enabled = False
                oForm.Items.Item("17").Enabled = False
                oForm.Items.Item("21").Enabled = False
                oForm.Items.Item("25").Enabled = False
                oForm.Items.Item("18").Enabled = False
                oForm.Items.Item("22").Enabled = False
                oForm.Items.Item("26").Enabled = False

                oForm.Items.Item("40").Visible = False
                oForm.Items.Item("41").Visible = False
                oForm.Items.Item("42").Visible = False

                If (oComboStatus.Selected.Value = "LM" Or oComboStatus.Selected.Value = "SM" Or oComboStatus.Selected.Value = "HR") And strGHRAcct <> "R" And strGSta <> "A" Then

                    oForm.Items.Item("30").Visible = True
                    oForm.Items.Item("31").Visible = True
                    oForm.Items.Item("32").Visible = True
                    oForm.Items.Item("45").Visible = False


                    oApplication.Utilities.setEdittextvalue(oForm, "36", System.DateTime.Today.ToString("yyyyMMdd"))
                    Dim oRSGNo As SAPbobsCOM.Recordset
                    Dim strGNo As String
                    oRSGNo = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    Dim strQueryGNo As String = "Select max(isnull(U_Z_GNo,'0'))+1 as 'GNo' from [@Z_HR_OSEAPP]"
                    oRSGNo.DoQuery(strQueryGNo)
                    If Not oRSGNo.EoF Then
                        strGNo = oRSGNo.Fields.Item("GNo").Value.ToString()
                    End If

                    oApplication.Utilities.setEdittextvalue(oForm, "37", strGNo)
                    oForm.Items.Item("36").Enabled = False
                    oForm.Items.Item("37").Enabled = False
                    If isLevelStartFromLM Then
                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("44").Enabled = False
                    Else
                        oForm.Items.Item("29").Enabled = True
                        oForm.Items.Item("44").Enabled = True
                    End If
                    oForm.Items.Item("32").Enabled = True
                    oForm.Items.Item("39").Enabled = True

                Else
                    If (oComboStatus.Selected.Value = "LM" Or oComboStatus.Selected.Value = "SM" Or oComboStatus.Selected.Value = "HR") And strGHRAcct = "R" Then
                        'FillAcceptanceCombo()
                        oForm.Items.Item("30").Visible = True
                        oForm.Items.Item("31").Visible = True
                        oForm.Items.Item("32").Visible = True
                        oForm.Items.Item("45").Visible = False

                        Dim oRSGNo As SAPbobsCOM.Recordset
                        Dim strGNo, strGDate, strGRemarks As String
                        oRSGNo = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        Dim strQueryGNo As String = "Select U_Z_GRemarks,U_Z_GNo, Convert(VarChar(8),U_Z_GDate,112) As U_Z_GDate  from [@Z_HR_OSEAPP] Where DocEntry = " & DocNo & ""
                        oRSGNo.DoQuery(strQueryGNo)
                        If Not oRSGNo.EoF Then
                            strGNo = oRSGNo.Fields.Item("U_Z_GNo").Value.ToString()
                            strGDate = oRSGNo.Fields.Item("U_Z_GDate").Value.ToString()
                            strGRemarks = oRSGNo.Fields.Item("U_Z_GRemarks").Value.ToString()
                        End If

                        oApplication.Utilities.setEdittextvalue(oForm, "36", strGDate)
                        oApplication.Utilities.setEdittextvalue(oForm, "37", strGNo)
                        oApplication.Utilities.setEdittextvalue(oForm, "38", strGRemarks)

                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("44").Enabled = False
                        oForm.Items.Item("32").Enabled = False
                        oForm.Items.Item("39").Enabled = False
                        oForm.Items.Item("36").Enabled = False
                        oForm.Items.Item("37").Enabled = False
                    End If
                End If


                If strWFStatus <> "SE" And strWFStatus <> "DR" Then
                    oGrid_P1.Columns.Item("Self Rating").Editable = False
                    oGrid_P2.Columns.Item("Self Rating").Editable = False
                    oGrid_P3.Columns.Item("Self Rating").Editable = False
                    If strChkH = "Y" Then 'And strGSta = "-" Then
                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("44").Enabled = False
                        oForm.Items.Item("32").Enabled = False
                    ElseIf strChkH = "N" And strGSta = "-" Then
                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("44").Enabled = False
                    ElseIf strChkH = "Y" And strGSta = "G" Then
                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("44").Enabled = False
                        oForm.Items.Item("32").Enabled = False
                    ElseIf strGSta = "-" Then
                        oForm.Items.Item("btnGra").Visible = True
                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("44").Enabled = False
                    Else

                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("44").Enabled = False
                    End If
                ElseIf strWFStatus = "SE" Or strWFStatus = "DR" Then
                    If isLevelStartFromLM Then
                        oGrid_P1.Columns.Item("Self Rating").Editable = False
                        oGrid_P2.Columns.Item("Self Rating").Editable = False
                        oGrid_P3.Columns.Item("Self Rating").Editable = False

                        oGrid_P1.Columns.Item("U_Z_SelfRaCode").Editable = False
                        oGrid_P2.Columns.Item("U_Z_SelfRaCode").Editable = False
                        oGrid_P3.Columns.Item("U_Z_SelfRaCode").Editable = False
                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("44").Enabled = False
                    Else
                        oGrid_P1.Columns.Item("Self Rating").Editable = False
                        oGrid_P2.Columns.Item("Self Rating").Editable = False
                        oGrid_P3.Columns.Item("Self Rating").Editable = False

                        oGrid_P1.Columns.Item("U_Z_SelfRaCode").Editable = True
                        oGrid_P2.Columns.Item("U_Z_SelfRaCode").Editable = True
                        oGrid_P3.Columns.Item("U_Z_SelfRaCode").Editable = True


                        oForm.Items.Item("29").Enabled = True
                        oForm.Items.Item("44").Enabled = True
                    End If

                ElseIf strWFStatus = "HR" And strChkH = "Y" And strGSta = "-" Then
                    oForm.Items.Item("32").Enabled = True
                    If isLevelStartFromLM Then
                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("44").Enabled = False
                    Else
                        oForm.Items.Item("29").Enabled = True
                        oForm.Items.Item("44").Enabled = True
                    End If

                End If

            ElseIf oForm.Title = "First Level Approval" Then
                oGrid_P1.Columns.Item("Code").Editable = False
                oGrid_P2.Columns.Item("Code").Editable = False
                oGrid_P3.Columns.Item("Code").Editable = False
                oGrid_P1.Columns.Item("Business Objectives").Editable = False
                oGrid_P2.Columns.Item("People Objectives").Editable = False
                oGrid_P3.Columns.Item("Competence Objectives").Editable = False
                oGrid_P3.Columns.Item("Competence Objectives").TitleObject.Caption = "Competencies"
                oGrid_P1.Columns.Item("Weight (%)").Editable = False
                oGrid_P2.Columns.Item("Weight (%)").Editable = False
                oGrid_P3.Columns.Item("Weight (%)").Editable = False
                oGrid_P1.Columns.Item("Self Rating").Editable = False
                oGrid_P2.Columns.Item("Self Rating").Editable = False
                oGrid_P3.Columns.Item("Self Rating").Editable = False

                oGrid_P1.Columns.Item("Sr.Manager Rating").Editable = False
                oGrid_P2.Columns.Item("Sr.Manager Rating").Editable = False
                oGrid_P3.Columns.Item("Sr.Manager Rating").Editable = False

                oGrid_P1.Columns.Item("Line Manager Rating").Editable = False
                oGrid_P2.Columns.Item("Line Manager Rating").Editable = False
                oGrid_P3.Columns.Item("Line Manager Rating").Editable = False

                oGrid_P1.Columns.Item("U_Z_SelfRaCode").Editable = False
                oGrid_P2.Columns.Item("U_Z_SelfRaCode").Editable = False
                oGrid_P3.Columns.Item("U_Z_SelfRaCode").Editable = False

                oGrid_P1.Columns.Item("U_Z_SMRaCode").Editable = False
                oGrid_P2.Columns.Item("U_Z_SMRaCode").Editable = False
                oGrid_P3.Columns.Item("U_Z_SMRaCode").Editable = False

                oGrid_P1.Columns.Item("U_Z_MgrRaCode").Editable = True
                oGrid_P2.Columns.Item("U_Z_MgrRaCode").Editable = True
                oGrid_P3.Columns.Item("U_Z_MgrRaCode").Editable = True


                oGrid_P2.Columns.Item("Category").Editable = False
                oGrid_P3.Columns.Item("Levels").Editable = False
                oGrid_P3.Columns.Item("Levels").TitleObject.Caption = "Min expected level"
                oForm.Items.Item("15").Enabled = False
                oForm.Items.Item("19").Enabled = False
                oForm.Items.Item("23").Enabled = False
                oForm.Items.Item("17").Enabled = False
                oForm.Items.Item("21").Enabled = False
                oForm.Items.Item("25").Enabled = False
                oForm.Items.Item("18").Enabled = False
                oForm.Items.Item("22").Enabled = False
                oForm.Items.Item("26").Enabled = False
                oForm.Items.Item("39").Visible = False

                oForm.Items.Item("41").Visible = False
                oForm.Items.Item("42").Visible = False
            ElseIf oForm.Title = "Second Level Approval" Then
                oGrid_P1.Columns.Item("Code").Editable = False
                oGrid_P2.Columns.Item("Code").Editable = False
                oGrid_P3.Columns.Item("Code").Editable = False
                oGrid_P1.Columns.Item("Business Objectives").Editable = False
                oGrid_P2.Columns.Item("People Objectives").Editable = False
                oGrid_P3.Columns.Item("Competence Objectives").Editable = False
                oGrid_P3.Columns.Item("Competence Objectives").TitleObject.Caption = "Competencies"
                oGrid_P1.Columns.Item("Weight (%)").Editable = False
                oGrid_P2.Columns.Item("Weight (%)").Editable = False
                oGrid_P3.Columns.Item("Weight (%)").Editable = False
                oGrid_P1.Columns.Item("Line Manager Rating").Editable = False
                oGrid_P2.Columns.Item("Line Manager Rating").Editable = False
                oGrid_P3.Columns.Item("Line Manager Rating").Editable = False

                oGrid_P1.Columns.Item("Sr.Manager Rating").Editable = False
                oGrid_P2.Columns.Item("Sr.Manager Rating").Editable = False
                oGrid_P3.Columns.Item("Sr.Manager Rating").Editable = False

                oGrid_P1.Columns.Item("Self Rating").Editable = False
                oGrid_P2.Columns.Item("Self Rating").Editable = False
                oGrid_P3.Columns.Item("Self Rating").Editable = False

                oGrid_P1.Columns.Item("U_Z_SelfRaCode").Editable = False
                oGrid_P2.Columns.Item("U_Z_SelfRaCode").Editable = False
                oGrid_P3.Columns.Item("U_Z_SelfRaCode").Editable = False

                oGrid_P1.Columns.Item("U_Z_SMRaCode").Editable = True
                oGrid_P2.Columns.Item("U_Z_SMRaCode").Editable = True
                oGrid_P3.Columns.Item("U_Z_SMRaCode").Editable = True

                oGrid_P1.Columns.Item("U_Z_MgrRaCode").Editable = False
                oGrid_P2.Columns.Item("U_Z_MgrRaCode").Editable = False
                oGrid_P3.Columns.Item("U_Z_MgrRaCode").Editable = False

                oGrid_P2.Columns.Item("Category").Editable = False
                oGrid_P3.Columns.Item("Levels").Editable = False
                oGrid_P3.Columns.Item("Levels").TitleObject.Caption = "Min expected level"
                oForm.Items.Item("16").Enabled = False
                oForm.Items.Item("20").Enabled = False
                oForm.Items.Item("24").Enabled = False
                oForm.Items.Item("15").Enabled = False
                oForm.Items.Item("19").Enabled = False
                oForm.Items.Item("23").Enabled = False
                oForm.Items.Item("18").Enabled = False
                oForm.Items.Item("22").Enabled = False
                oForm.Items.Item("26").Enabled = False
                oForm.Items.Item("39").Visible = False
                oForm.Items.Item("40").Visible = False

                oForm.Items.Item("42").Visible = False
            End If
            If strStatus = "C" Then
                oForm.Items.Item("29").Enabled = False
                oForm.Items.Item("44").Enabled = False
            End If
        End If
    End Sub

    Private Sub ReDatabind(ByVal aForm As SAPbouiCOM.Form)
        oForm = aForm
        Dim strqry As String
        oGrid = oForm.Items.Item("3").Specific
        oGrid_P1 = oForm.Items.Item("8").Specific
        oGrid_P2 = oForm.Items.Item("9").Specific
        oGrid_P3 = oForm.Items.Item("10").Specific
        oGrid_P4 = oForm.Items.Item("46").Specific
        aForm.Items.Item("btnGra").Visible = False
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
        oGrid_P1.DataTable = oForm.DataSources.DataTables.Item("DT_1_P1")
        oGrid_P2.DataTable = oForm.DataSources.DataTables.Item("DT_2_P2")
        oGrid_P3.DataTable = oForm.DataSources.DataTables.Item("DT_3_P3")
        oGrid_P4.DataTable = oForm.DataSources.DataTables.Item("DT_4_P4")
        ' oGrid.DataTable.ExecuteQuery(strqry)
        Dim ostatic As SAPbouiCOM.StaticText
        ostatic = oForm.Items.Item("351").Specific
        If ostatic.Caption <> "" Then
            strqry = ostatic.Caption
        Else
            Exit Sub
        End If
        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Apprisal Number"
        oGrid.Columns.Item("DocEntry").Editable = False
        oEditTextColumn = oGrid.Columns.Item("DocEntry")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Invoice
        oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee Id"
        oGrid.Columns.Item("U_Z_EmpId").Editable = False
        oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
        oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
        oGrid.Columns.Item("U_Z_EmpName").Editable = False
        oGrid.Columns.Item("U_Z_Date").TitleObject.Caption = "Date"
        oGrid.Columns.Item("U_Z_Date").Editable = False
        oGrid.Columns.Item("U_Z_Period").TitleObject.Caption = "Period"
        oGrid.Columns.Item("U_Z_Period").Editable = False
        oGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Active"
        oGrid.Columns.Item("U_Z_Status").Editable = False
        oGrid.Columns.Item("U_Z_WStatus").TitleObject.Caption = "Status"
        oGrid.Columns.Item("U_Z_WStatus").Editable = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        If oGrid.Rows.Count > 0 Then
            oGrid.Rows.SelectedRows.Add(0)
            Dim DocNo As Integer = Convert.ToInt32(oGrid.DataTable.GetValue("DocEntry", 0))
            If DocNo = 0 Then
                oApplication.Utilities.Message("No Records Found...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                DocNo = 99999
            End If
            Dim StrQP0, StrQP1, StrQP2, StrQP3 As String
            StrQP0 = ""
            StrQP1 = ""
            StrQP2 = ""
            StrQP3 = ""
            StrQP0 = "Select U_Z_WStatus,U_Z_BSelfRemark,U_Z_BMgrRemark,U_Z_BSMrRemark,U_Z_BHrRemark, U_Z_PSelfRemark,U_Z_PMgrRemark,U_Z_PSMrRemark,U_Z_PHrRemark, U_Z_CSelfRemark,U_Z_CMgrRemark,U_Z_CSMrRemark,U_Z_CHrRemark from [@Z_HR_OSEAPP] Where DocEntry=" & DocNo & ""
            'StrQP1 = "Select U_Z_BussCode as 'Code',U_Z_BussDesc as 'Business Objectives',U_Z_BussWeight as 'Weight (%)' ,U_Z_BussSelfRate as 'Self Rating',U_Z_BussMgrRate as 'Line Manager Rating',U_Z_BussSMRate as 'Sr.Manager Rating' from [@Z_HR_SEAPP1] Where DocEntry=" & DocNo & ""
            'StrQP2 = "Select T0.U_Z_PeopleCode as 'Code',T0.U_Z_PeopleDesc as 'People Objectives',T2.U_Z_Remarks 'Emp Remarks',T0.U_Z_PeopleCat as 'Category',T0.U_Z_PeoWeight as 'Weight (%)',T0.U_Z_PeoSelfRate as 'Self Rating',T0.U_Z_PeoMgrRate as 'Line Manager Rating',T0.U_Z_PeoSMRate as 'Sr.Manager Rating' from [@Z_HR_SEAPP2] T0 Join [@Z_HR_OSEAPP] T1 ON T1.DocEntry = T0.DocEntry Left Outer Join [@Z_HR_PEOBJ1] T2 On T1.U_Z_EmpId = T2.U_Z_HREmpID  and T2.U_Z_HRPeoobjCode=T0.U_Z_PeopleCode Where T0.DocEntry =" & DocNo & ""
            'StrQP3 = "Select T0.U_Z_CompCode as 'Code',T0.U_Z_CompDesc as 'Competence Objectives',T0.U_Z_CompWeight as 'Weight (%)',T0.U_Z_CompLevel as 'Levels',T2.U_Z_CompLevel As 'Current Level',T0.U_Z_CompSelf as 'Self Rating',T0.U_Z_CompMgr as 'Line Manager Rating',T0.U_Z_CompSM as 'Sr.Manager Rating' from [@Z_HR_SEAPP3] T0 Join [@Z_HR_OSEAPP] T1 ON T1.DocEntry = T0.DocEntry Left Outer Join [@Z_HR_ECOLVL] T2 On T1.U_Z_EmpId = T2.U_Z_HREmpID Where T0.DocEntry=" & DocNo & ""
            StrQP1 = "Select U_Z_BussCode as 'Code',U_Z_BussDesc as 'Business Objectives',U_Z_BussWeight as 'Weight (%)' ,T0.""U_Z_SelfRaCode"",U_Z_BussSelfRate as 'Self Rating',T0.""U_Z_MgrRaCode"",U_Z_BussMgrRate as 'Line Manager Rating',T0.""U_Z_SMRaCode"",U_Z_BussSMRate as 'Sr.Manager Rating' from [@Z_HR_SEAPP1] T0 Where DocEntry=" & DocNo & ""
            StrQP2 = "Select T0.U_Z_PeopleCode as 'Code',T0.U_Z_PeopleDesc as 'People Objectives',T2.U_Z_Remarks 'Emp Remarks',T0.U_Z_PeopleCat as 'Category',T0.U_Z_PeoWeight as 'Weight (%)',""U_Z_SelfRaCode"",T0.U_Z_PeoSelfRate as 'Self Rating',T0.""U_Z_MgrRaCode"",T0.U_Z_PeoMgrRate as 'Line Manager Rating',T0.""U_Z_SMRaCode"",T0.U_Z_PeoSMRate as 'Sr.Manager Rating' from [@Z_HR_SEAPP2] T0 Join [@Z_HR_OSEAPP] T1 ON T1.DocEntry = T0.DocEntry Left Outer Join [@Z_HR_PEOBJ1] T2 On T1.U_Z_EmpId = T2.U_Z_HREmpID  and T2.U_Z_HRPeoobjCode=T0.U_Z_PeopleCode Where T0.DocEntry = " & DocNo & ""
            StrQP3 = "Select T0.U_Z_CompCode as 'Code',T0.U_Z_CompDesc as 'Competence Objectives',T0.U_Z_CompWeight as 'Weight (%)',T0.U_Z_CompLevel as 'Levels',T2.U_Z_CompLevel As 'Current Level',""U_Z_SelfRaCode"",T0.U_Z_CompSelfRate as 'Self Rating',T0.""U_Z_MgrRaCode"",T0.U_Z_CompMgrRate as 'Line Manager Rating',T0.""U_Z_SMRaCode"",T0.U_Z_CompSMRate as 'Sr.Manager Rating' from [@Z_HR_SEAPP3] T0 Join [@Z_HR_OSEAPP] T1 ON T1.DocEntry = T0.DocEntry Left Outer Join [@Z_HR_ECOLVL] T2 On T1.U_Z_EmpId = T2.U_Z_HREmpID and T2.U_Z_CompCode =T0.U_Z_CompCode Where T0.DocEntry = " & DocNo & ""

            oGrid_P1.DataTable.ExecuteQuery(StrQP1)
            oGrid_P2.DataTable.ExecuteQuery(StrQP2)
            oGrid_P3.DataTable.ExecuteQuery(StrQP3)
            Dim oRS As SAPbobsCOM.Recordset
            oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRS.DoQuery(StrQP0)

            Dim oRec As SAPbobsCOM.Recordset
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim strCStatus As String = ""
            Dim strChkS, strChkL, strChkSr, strChkH, strGHRAcct, strGSta, strStatus As String
            strCStatus = "Select U_Z_SCkApp,U_Z_LCkApp,U_Z_SrCkApp,U_Z_HrCkApp,U_Z_GHRSts,U_Z_GStatus,U_Z_Status from [@Z_HR_OSEAPP] where DocEntry=" & DocNo & ""
            oRec.DoQuery(strCStatus)
            If Not oRec.EoF Then
                strChkS = oRec.Fields.Item("U_Z_SCkApp").Value.ToString()
                strChkL = oRec.Fields.Item("U_Z_LCkApp").Value.ToString()
                strChkSr = oRec.Fields.Item("U_Z_SrCkApp").Value.ToString()
                strChkH = oRec.Fields.Item("U_Z_HrCkApp").Value.ToString()
                strGSta = oRec.Fields.Item("U_Z_GStatus").Value.ToString()
                strGHRAcct = oRec.Fields.Item("U_Z_GHRSts").Value.ToString()
                strStatus = oRec.Fields.Item("U_Z_Status").Value.ToString()
            End If

            colSum()

            oGrid_P2.Columns.Item("Emp Remarks").TitleObject.Caption = "Remarks"
            oGrid_P2.Columns.Item("Emp Remarks").Editable = False

            oGrid_P1.Columns.Item("U_Z_SelfRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oGrid_P1.Columns.Item("U_Z_MgrRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oGrid_P1.Columns.Item("U_Z_SMRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox

            Dim oComboCol As SAPbouiCOM.ComboBoxColumn
            If 1 = 1 Then

                sQuery = "Select U_Z_RateCode As Code,U_Z_RateName As Name From [@Z_HR_ORATE]"
                oRec.DoQuery(sQuery)
                oComboCol = oGrid_P1.Columns.Item("U_Z_SelfRaCode")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                oComboCol = oGrid_P1.Columns.Item("U_Z_MgrRaCode")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                oComboCol = oGrid_P1.Columns.Item("U_Z_SMRaCode")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
            End If


            oGrid_P2.Columns.Item("U_Z_SelfRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oGrid_P2.Columns.Item("U_Z_MgrRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oGrid_P2.Columns.Item("U_Z_SMRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox


            If 1 = 1 Then


                sQuery = "Select U_Z_RateCode As Code,U_Z_RateName As Name From [@Z_HR_ORATE]"
                oRec.DoQuery(sQuery)
                oComboCol = oGrid_P2.Columns.Item("U_Z_SelfRaCode")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                oComboCol = oGrid_P2.Columns.Item("U_Z_MgrRaCode")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                oComboCol = oGrid_P2.Columns.Item("U_Z_SMRaCode")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
            End If



            oGrid_P3.Columns.Item("Levels").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oGrid_P3.Columns.Item("Current Level").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            'oGrid_P3.Columns.Item("Self Rating").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            'oGrid_P3.Columns.Item("Line Manager Rating").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            'oGrid_P3.Columns.Item("Sr.Manager Rating").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox

            oGrid_P3.Columns.Item("U_Z_SelfRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oGrid_P3.Columns.Item("U_Z_MgrRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oGrid_P3.Columns.Item("U_Z_SMRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox

            oGrid_P1.Columns.Item("U_Z_SelfRaCode").TitleObject.Caption = "Self Rating"
            oGrid_P1.Columns.Item("U_Z_MgrRaCode").TitleObject.Caption = "First Level Manager Rating"
            oGrid_P1.Columns.Item("U_Z_SMRaCode").TitleObject.Caption = "Second Level Manager Rating"

            oGrid_P2.Columns.Item("U_Z_SelfRaCode").TitleObject.Caption = "Self Rating"
            oGrid_P2.Columns.Item("U_Z_MgrRaCode").TitleObject.Caption = "First Level Manager Rating"
            oGrid_P2.Columns.Item("U_Z_SMRaCode").TitleObject.Caption = "Second Level Manager Rating"


            oGrid_P3.Columns.Item("U_Z_SelfRaCode").TitleObject.Caption = "Self Rating"
            oGrid_P3.Columns.Item("U_Z_MgrRaCode").TitleObject.Caption = "First Level Manager Rating"
            oGrid_P3.Columns.Item("U_Z_SMRaCode").TitleObject.Caption = "Second Level Manager Rating"

            sQuery = "Select U_Z_LvelCode As Code,U_Z_LvelName As Name From [@Z_HR_COLVL]"
            oRec.DoQuery(sQuery)
            If Not oRec.EoF Then

                oComboCol = oGrid_P3.Columns.Item("Levels")
                oComboCol.ValidValues.Add("", "")
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                oComboCol = oGrid_P3.Columns.Item("Current Level")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                sQuery = "Select U_Z_RateCode As Code,U_Z_RateName As Name From [@Z_HR_ORATE]"
                oRec.DoQuery(sQuery)
                oComboCol = oGrid_P3.Columns.Item("U_Z_SelfRaCode")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                oComboCol = oGrid_P3.Columns.Item("U_Z_MgrRaCode")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                oComboCol = oGrid_P3.Columns.Item("U_Z_SMRaCode")
                oComboCol.ValidValues.Add("", "")
                oRec.MoveFirst()
                For index As Integer = 0 To oRec.RecordCount - 1
                    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                    oRec.MoveNext()
                Next
                oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
            End If
            oGrid_P3.Columns.Item("Current Level").Editable = False
            Disable(strStatus)

            If oForm.Title = "Self Appraisals" Then
                If strChkS = "Y" Then
                    Dim oChk As SAPbouiCOM.CheckBox
                    oChk = oForm.Items.Item("39").Specific
                    oChk.Checked = True
                    oForm.Items.Item("39").Enabled = False
                    oForm.Items.Item("29").Enabled = False
                    oForm.Items.Item("44").Enabled = False
                ElseIf strGSta = "A" Then
                    oForm.Items.Item("31").Visible = True
                    oForm.Items.Item("32").Visible = True
                    oForm.Items.Item("32").Enabled = False
                    oForm.Items.Item("29").Enabled = False
                    oForm.Items.Item("44").Enabled = False
                End If
            ElseIf oForm.Title = "First Level Approval" Then
                If strChkL = "Y" Then
                    Dim oChk As SAPbouiCOM.CheckBox
                    oChk = oForm.Items.Item("40").Specific
                    oChk.Checked = True
                    oForm.Items.Item("40").Enabled = False
                    oForm.Items.Item("29").Enabled = False
                    oForm.Items.Item("44").Enabled = False
                Else
                    If strChkH = "Y" Then
                        Dim oChk As SAPbouiCOM.CheckBox
                        oChk = oForm.Items.Item("41").Specific
                        oChk.Checked = True
                        oForm.Items.Item("41").Enabled = False
                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("44").Enabled = False
                    End If
                End If
            ElseIf oForm.Title = "Second Level Approval" Then
                If strChkSr = "Y" Then
                    Dim oChk As SAPbouiCOM.CheckBox
                    oChk = oForm.Items.Item("41").Specific
                    oChk.Checked = True
                    oForm.Items.Item("41").Enabled = False
                    oForm.Items.Item("29").Enabled = False
                    oForm.Items.Item("44").Enabled = False
                Else
                    If strChkH = "Y" Then
                        Dim oChk As SAPbouiCOM.CheckBox
                        oChk = oForm.Items.Item("41").Specific
                        oChk.Checked = True
                        oForm.Items.Item("41").Enabled = False
                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("44").Enabled = False
                    End If
                End If
            ElseIf oForm.Title = "HR Acceptance" Then
                If strChkH = "Y" Then
                    Dim oChk As SAPbouiCOM.CheckBox
                    oChk = oForm.Items.Item("42").Specific
                    oChk.Checked = True
                    oForm.Items.Item("42").Enabled = False
                    oForm.Items.Item("29").Enabled = False
                    oForm.Items.Item("44").Enabled = False
                End If
            End If

            Dim strBSelfRMark, strBMgrRMark, strBSMrRMark, strBHrRMark, strPSelfRMark, strPMgrRMark, strPSMrRMark, strPHrRMark, strCSelfRMark, strCMgrRMark, strCSMrRMark, strCHrRMark, strWStatus As String
            strBSelfRMark = oRS.Fields.Item("U_Z_BSelfRemark").Value.ToString()
            strBMgrRMark = oRS.Fields.Item("U_Z_BMgrRemark").Value.ToString()
            strBSMrRMark = oRS.Fields.Item("U_Z_BSMrRemark").Value.ToString()
            strBHrRMark = oRS.Fields.Item("U_Z_BHrRemark").Value.ToString()
            strPSelfRMark = oRS.Fields.Item("U_Z_PSelfRemark").Value.ToString()
            strPMgrRMark = oRS.Fields.Item("U_Z_PMgrRemark").Value.ToString()
            strPSMrRMark = oRS.Fields.Item("U_Z_PSMrRemark").Value.ToString()
            strPHrRMark = oRS.Fields.Item("U_Z_PHrRemark").Value.ToString()
            strCSelfRMark = oRS.Fields.Item("U_Z_CSelfRemark").Value.ToString()
            strCMgrRMark = oRS.Fields.Item("U_Z_CMgrRemark").Value.ToString()
            strCSMrRMark = oRS.Fields.Item("U_Z_CSMrRemark").Value.ToString()
            strCHrRMark = oRS.Fields.Item("U_Z_CHrRemark").Value.ToString()
            strWStatus = oRS.Fields.Item("U_Z_WStatus").Value.ToString()
            oApplication.Utilities.setEdittextvalue(oForm, "15", strBSelfRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "16", strBMgrRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "17", strBSMrRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "18", strBHrRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "19", strPSelfRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "20", strPMgrRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "21", strPSMrRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "22", strPHrRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "23", strCSelfRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "24", strCMgrRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "25", strCSMrRMark)
            oApplication.Utilities.setEdittextvalue(oForm, "26", strCHrRMark)
            oForm.ActiveItem = 28
            Dim oComboStatus As SAPbouiCOM.ComboBox
            oComboStatus = oForm.Items.Item("28").Specific
            Try
                oComboStatus.Select(strWStatus, SAPbouiCOM.BoSearchKey.psk_ByValue)

            Catch ex As Exception

            End Try

            Dim oComboGSta As SAPbouiCOM.ComboBox
            oComboGSta = oForm.Items.Item("32").Specific
            Try
                oComboGSta.Select(strGSta, SAPbouiCOM.BoSearchKey.psk_ByValue)
            Catch ex As Exception

            End Try



            If oForm.Title = "HR Acceptance" Then
                oForm.Items.Item("8").Enabled = False
                oForm.Items.Item("9").Enabled = False
                oForm.Items.Item("10").Enabled = False
                oForm.Items.Item("16").Enabled = False
                oForm.Items.Item("20").Enabled = False
                oForm.Items.Item("24").Enabled = False
                oForm.Items.Item("17").Enabled = False
                oForm.Items.Item("21").Enabled = False
                oForm.Items.Item("25").Enabled = False
                oForm.Items.Item("15").Enabled = False
                oForm.Items.Item("19").Enabled = False
                oForm.Items.Item("23").Enabled = False
                oForm.Items.Item("39").Visible = False
                oForm.Items.Item("40").Visible = False
                oForm.Items.Item("41").Visible = False

            ElseIf oForm.Title = "Self Appraisals" Then
                oGrid_P1.Columns.Item("Code").Editable = False
                oGrid_P2.Columns.Item("Code").Editable = False
                oGrid_P3.Columns.Item("Code").Editable = False
                oGrid_P1.Columns.Item("Business Objectives").Editable = False
                oGrid_P2.Columns.Item("People Objectives").Editable = False
                oGrid_P3.Columns.Item("Competence Objectives").Editable = False
                oGrid_P3.Columns.Item("Competence Objectives").TitleObject.Caption = "Competencies"
                oGrid_P1.Columns.Item("Weight (%)").Editable = False
                oGrid_P2.Columns.Item("Weight (%)").Editable = False
                oGrid_P3.Columns.Item("Weight (%)").Editable = False
                oGrid_P1.Columns.Item("Line Manager Rating").Editable = False
                oGrid_P2.Columns.Item("Line Manager Rating").Editable = False
                oGrid_P3.Columns.Item("Line Manager Rating").Editable = False
                oGrid_P1.Columns.Item("Sr.Manager Rating").Editable = False
                oGrid_P2.Columns.Item("Sr.Manager Rating").Editable = False
                oGrid_P3.Columns.Item("Sr.Manager Rating").Editable = False
                oGrid_P2.Columns.Item("Category").Editable = False
                oGrid_P3.Columns.Item("Levels").Editable = False
                oGrid_P3.Columns.Item("Levels").TitleObject.Caption = "Min expected level"
                oForm.Items.Item("16").Enabled = False
                oForm.Items.Item("20").Enabled = False
                oForm.Items.Item("24").Enabled = False
                oForm.Items.Item("17").Enabled = False
                oForm.Items.Item("21").Enabled = False
                oForm.Items.Item("25").Enabled = False
                oForm.Items.Item("18").Enabled = False
                oForm.Items.Item("22").Enabled = False
                oForm.Items.Item("26").Enabled = False

                oForm.Items.Item("40").Visible = False
                oForm.Items.Item("41").Visible = False
                oForm.Items.Item("42").Visible = False
                If (oComboStatus.Selected.Value = "LM" Or oComboStatus.Selected.Value = "SM" Or oComboStatus.Selected.Value = "HR") And strGHRAcct <> "R" And strGSta <> "A" Then

                    oForm.Items.Item("30").Visible = True
                    oForm.Items.Item("31").Visible = True
                    oForm.Items.Item("32").Visible = True
                    oForm.Items.Item("45").Visible = False

                    oApplication.Utilities.setEdittextvalue(oForm, "36", System.DateTime.Today.ToString("yyyyMMdd"))
                    Dim oRSGNo As SAPbobsCOM.Recordset
                    Dim strGNo As String
                    oRSGNo = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    Dim strQueryGNo As String = "Select max(isnull(U_Z_GNo,'0'))+1 as 'GNo' from [@Z_HR_OSEAPP]"
                    oRSGNo.DoQuery(strQueryGNo)
                    If Not oRSGNo.EoF Then
                        strGNo = oRSGNo.Fields.Item("GNo").Value.ToString()
                    End If

                    oApplication.Utilities.setEdittextvalue(oForm, "37", strGNo)
                    oForm.Items.Item("36").Enabled = False
                    oForm.Items.Item("37").Enabled = False

                    oForm.Items.Item("29").Enabled = True
                    oForm.Items.Item("44").Enabled = True
                    oForm.Items.Item("32").Enabled = True
                    oForm.Items.Item("39").Enabled = True
                Else
                    If (oComboStatus.Selected.Value = "LM" Or oComboStatus.Selected.Value = "SM" Or oComboStatus.Selected.Value = "HR") And strGHRAcct = "R" Then
                        'FillAcceptanceCombo()
                        oForm.Items.Item("30").Visible = True
                        oForm.Items.Item("31").Visible = True
                        oForm.Items.Item("32").Visible = True
                        oForm.Items.Item("45").Visible = False

                        Dim oRSGNo As SAPbobsCOM.Recordset
                        Dim strGNo, strGDate, strGRemarks As String
                        oRSGNo = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        Dim strQueryGNo As String = "Select U_Z_GRemarks,U_Z_GNo, Convert(VarChar(8),U_Z_GDate,112) As U_Z_GDate  from [@Z_HR_OSEAPP] Where DocEntry = " & DocNo & ""
                        oRSGNo.DoQuery(strQueryGNo)
                        If Not oRSGNo.EoF Then
                            strGNo = oRSGNo.Fields.Item("U_Z_GNo").Value.ToString()
                            strGDate = oRSGNo.Fields.Item("U_Z_GDate").Value.ToString()
                            strGRemarks = oRSGNo.Fields.Item("U_Z_GRemarks").Value.ToString()
                        End If

                        oApplication.Utilities.setEdittextvalue(oForm, "36", strGDate)
                        oApplication.Utilities.setEdittextvalue(oForm, "37", strGNo)
                        oApplication.Utilities.setEdittextvalue(oForm, "38", strGRemarks)

                        oForm.Items.Item("29").Enabled = False
                        oForm.Items.Item("44").Enabled = False

                        oForm.Items.Item("32").Enabled = False
                        oForm.Items.Item("39").Enabled = False
                        oForm.Items.Item("36").Enabled = False
                        oForm.Items.Item("37").Enabled = False
                    End If
                End If
            ElseIf oForm.Title = "First Level Approval" Then
                oGrid_P1.Columns.Item("Code").Editable = False
                oGrid_P2.Columns.Item("Code").Editable = False
                oGrid_P3.Columns.Item("Code").Editable = False
                oGrid_P1.Columns.Item("Business Objectives").Editable = False
                oGrid_P2.Columns.Item("People Objectives").Editable = False
                oGrid_P3.Columns.Item("Competence Objectives").Editable = False
                oGrid_P3.Columns.Item("Competence Objectives").TitleObject.Caption = "Competencies"
                oGrid_P1.Columns.Item("Weight (%)").Editable = False
                oGrid_P2.Columns.Item("Weight (%)").Editable = False
                oGrid_P3.Columns.Item("Weight (%)").Editable = False
                oGrid_P1.Columns.Item("Self Rating").Editable = False


                oGrid_P2.Columns.Item("Self Rating").Editable = False
                oGrid_P3.Columns.Item("Self Rating").Editable = False
                oGrid_P1.Columns.Item("Sr.Manager Rating").Editable = False
                oGrid_P2.Columns.Item("Sr.Manager Rating").Editable = False
                oGrid_P3.Columns.Item("Sr.Manager Rating").Editable = False
                oGrid_P2.Columns.Item("Category").Editable = False
                oGrid_P3.Columns.Item("Levels").Editable = False
                oGrid_P3.Columns.Item("Levels").TitleObject.Caption = "Min expected level"
                oForm.Items.Item("15").Enabled = False
                oForm.Items.Item("19").Enabled = False
                oForm.Items.Item("23").Enabled = False
                oForm.Items.Item("17").Enabled = False
                oForm.Items.Item("21").Enabled = False
                oForm.Items.Item("25").Enabled = False
                oForm.Items.Item("18").Enabled = False
                oForm.Items.Item("22").Enabled = False
                oForm.Items.Item("26").Enabled = False
                oForm.Items.Item("39").Visible = False
                oForm.Items.Item("41").Visible = False
                oForm.Items.Item("42").Visible = False
            ElseIf oForm.Title = "Second Level Approval" Then
                oGrid_P1.Columns.Item("Code").Editable = False
                oGrid_P2.Columns.Item("Code").Editable = False
                oGrid_P3.Columns.Item("Code").Editable = False
                oGrid_P1.Columns.Item("Business Objectives").Editable = False
                oGrid_P2.Columns.Item("People Objectives").Editable = False
                oGrid_P3.Columns.Item("Competence Objectives").Editable = False
                oGrid_P3.Columns.Item("Competence Objectives").TitleObject.Caption = "Competencies"
                oGrid_P1.Columns.Item("Weight (%)").Editable = False
                oGrid_P2.Columns.Item("Weight (%)").Editable = False
                oGrid_P3.Columns.Item("Weight (%)").Editable = False
                oGrid_P1.Columns.Item("Line Manager Rating").Editable = False
                oGrid_P2.Columns.Item("Line Manager Rating").Editable = False
                oGrid_P3.Columns.Item("Line Manager Rating").Editable = False
                oGrid_P1.Columns.Item("Self Rating").Editable = False
                oGrid_P2.Columns.Item("Self Rating").Editable = False
                oGrid_P3.Columns.Item("Self Rating").Editable = False
                oGrid_P2.Columns.Item("Category").Editable = False
                oGrid_P3.Columns.Item("Levels").Editable = False
                oGrid_P3.Columns.Item("Levels").TitleObject.Caption = "Min expected level"
                oForm.Items.Item("16").Enabled = False
                oForm.Items.Item("20").Enabled = False
                oForm.Items.Item("24").Enabled = False
                oForm.Items.Item("15").Enabled = False
                oForm.Items.Item("19").Enabled = False
                oForm.Items.Item("23").Enabled = False
                oForm.Items.Item("18").Enabled = False
                oForm.Items.Item("22").Enabled = False
                oForm.Items.Item("26").Enabled = False
                oForm.Items.Item("39").Visible = False
                oForm.Items.Item("40").Visible = False
                oForm.Items.Item("42").Visible = False
            End If
            If strStatus = "C" Then
                oForm.Items.Item("29").Enabled = False
                oForm.Items.Item("44").Enabled = False
            End If
        End If
    End Sub

#End Region

#Region "Update HR Doc"
    Private Sub UpdateHRStatus(ByVal DocNo As Integer, ByVal Status As String, ByVal BRemark As String, ByVal PRemark As String, ByVal CRemark As String, ByVal StrChkStatus As String)
        Dim oRec As SAPbobsCOM.Recordset
        Dim strQry As String = ""
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strQry = "Update [@Z_HR_OSEAPP] set U_Z_HrCkApp='" & StrChkStatus & "',U_Z_WStatus='" & Status & "' , U_Z_BHrRemark='" & BRemark & "' ,  U_Z_PHrRemark='" & PRemark & "' ,  U_Z_CHrRemark='" & CRemark & "' Where DocEntry=" & DocNo & ""
        'oRec.DoQuery(strQry)

        oHTUpdateCol = New Hashtable()
        oHTUpdateCol.Add("U_Z_HrCkApp", StrChkStatus)
        oHTUpdateCol.Add("U_Z_WStatus", Status)
        oHTUpdateCol.Add("U_Z_BHrRemark", BRemark)
        oHTUpdateCol.Add("U_Z_PHrRemark", PRemark)
        oHTUpdateCol.Add("U_Z_CHrRemark", CRemark)
        oApplication.Utilities.UpdateUsing_DIAPI("Z_HR_OSELAPP", DocNo, oHTUpdateCol)

        If StrChkStatus = "Y" Then
            strQry = "Update [@Z_HR_OSEAPP] set U_Z_HRNotify='Y' Where DocEntry = " & DocNo & ""
            'oRec.DoQuery(strQry)

            oHTUpdateCol = New Hashtable()
            oHTUpdateCol.Add("U_Z_HRNotify", "Y")
            oApplication.Utilities.UpdateUsing_DIAPI("Z_HR_OSELAPP", DocNo, oHTUpdateCol)
        End If
    End Sub
#End Region

#Region "Update HR Doc"
    Private Sub UpdateHRRating()
        Dim oRec As SAPbobsCOM.Recordset
        Dim strQry As String = ""
        oGrid_P4 = oForm.Items.Item("46").Specific
        For index As Integer = 0 To oGrid_P4.Rows.Count - 1
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strQry = "Update [@Z_HR_SEAPP4] Set U_Z_AvgComp = '" & oGrid_P4.DataTable.GetValue("U_Z_AvgComp", index) & "',U_Z_HRComp = '" & oGrid_P4.DataTable.GetValue("U_Z_HRComp", index) & "' Where DocEntry = '" & oGrid_P4.DataTable.GetValue("DocEntry", index) & "' And LineId = '" & oGrid_P4.DataTable.GetValue("LineId", index) & "'"
            oRec.DoQuery(strQry)
        Next
        'UpdateFInallRate(oForm)
    End Sub
#End Region

#Region "Update Appraisal Doc"
    Private Sub UpdateAppraisal()
        Dim oRec As SAPbobsCOM.Recordset
        Dim strDocEntry As String
        Dim strQry As String = ""
        oGrid = oForm.Items.Item("3").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.Rows.IsSelected(intRow) Then
                strDocEntry = oGrid.DataTable.GetValue("DocEntry", intRow)
            End If
        Next
        If strDocEntry <> "" Then
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strQry = "Update [@Z_HR_OSEAPP] Set U_Z_Status = 'C' Where DocEntry = '" & strDocEntry & "'"
            oRec.DoQuery(strQry)
        End If
    End Sub
#End Region

#Region "Update Document"
    Private Sub UpdateDocument(ByVal oHashB As Hashtable, ByVal oHashP As Hashtable, ByVal oHashC As Hashtable, ByVal strBRmark As String, ByVal strPRmark As String, ByVal strCRmark As String, ByVal strWStatus As String, ByVal DocNo As Integer, ByVal intFlag As Integer, ByVal oHGrevence As Hashtable, ByVal strChkStatus As String)
        Dim oRec As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oHTUpdateCol = New Hashtable()

        If intFlag = 1 Then
            Dim strQuery As String = ""
            Dim strQuery1 As String = ""
            Dim strQuery2 As String = ""
            Dim strQuery3 As String = ""
            Dim i, j, k As Integer

            If oHGrevence.Count > 0 Then
                If oHGrevence(1).ToString() = "Y" Then
                    If oHGrevence(2).ToString() = "A" Then
                        strQuery = "Update [@Z_HR_OSEAPP] set U_Z_SCkApp='" & strChkStatus & "',U_Z_WStatus='" & strWStatus & "' , U_Z_BSelfRemark='" & strBRmark & "' ,  U_Z_PSelfRemark='" & strPRmark & "' ,  U_Z_CSelfRemark='" & strCRmark & "',U_Z_GStatus='" & oHGrevence(2).ToString() & "' Where DocEntry=" & DocNo & ""
                        oHTUpdateCol.Add("U_Z_SCkApp", strChkStatus)
                        oHTUpdateCol.Add("U_Z_WStatus", strWStatus)
                        oHTUpdateCol.Add("U_Z_BSelfRemark", strBRmark)
                        oHTUpdateCol.Add("U_Z_PSelfRemark", strPRmark)
                        oHTUpdateCol.Add("U_Z_CSelfRemark", strCRmark)
                        oHTUpdateCol.Add("U_Z_GStatus", oHGrevence(2).ToString())
                    Else
                        If oHGrevence(2).ToString = "-" Then
                            strQuery = "Update [@Z_HR_OSEAPP] set U_Z_SCkApp='" & strChkStatus & "',U_Z_WStatus='" & strWStatus & "' , U_Z_BSelfRemark='" & strBRmark & "' ,  U_Z_PSelfRemark='" & strPRmark & "' ,  U_Z_CSelfRemark='" & strCRmark & "',U_Z_GStatus='" & oHGrevence(2).ToString() & "' Where DocEntry=" & DocNo & ""
                            oHTUpdateCol.Add("U_Z_SCkApp", strChkStatus)
                            oHTUpdateCol.Add("U_Z_WStatus", strWStatus)
                            oHTUpdateCol.Add("U_Z_BSelfRemark", strBRmark)
                            oHTUpdateCol.Add("U_Z_PSelfRemark", strPRmark)
                            oHTUpdateCol.Add("U_Z_CSelfRemark", strCRmark)
                            oHTUpdateCol.Add("U_Z_GStatus", oHGrevence(2).ToString())
                        Else
                            strQuery = "Update [@Z_HR_OSEAPP] set U_Z_SCkApp='" & strChkStatus & "',U_Z_WStatus='" & strWStatus & "' , U_Z_BSelfRemark='" & strBRmark & "' ,  U_Z_PSelfRemark='" & strPRmark & "' ,  U_Z_CSelfRemark='" & strCRmark & "',U_Z_GStatus='" & oHGrevence(2).ToString() & "',U_Z_GDate='" & oHGrevence(3).ToString() & "',U_Z_GNo='" & oHGrevence(4).ToString() & "',U_Z_GRemarks='" & oHGrevence(5).ToString() & "' Where DocEntry=" & DocNo & ""
                            oHTUpdateCol.Add("U_Z_SCkApp", strChkStatus)
                            oHTUpdateCol.Add("U_Z_WStatus", strWStatus)
                            oHTUpdateCol.Add("U_Z_BSelfRemark", strBRmark)
                            oHTUpdateCol.Add("U_Z_PSelfRemark", strPRmark)
                            oHTUpdateCol.Add("U_Z_CSelfRemark", strCRmark)
                            oHTUpdateCol.Add("U_Z_GStatus", oHGrevence(2).ToString())
                            oHTUpdateCol.Add("U_Z_GDate", oHGrevence(3).ToString())
                            oHTUpdateCol.Add("U_Z_GNo", oHGrevence(4).ToString())
                            oHTUpdateCol.Add("U_Z_GRemarks", oHGrevence(5).ToString())
                        End If
                    End If
                    'oRec.DoQuery(strQuery)
                    oApplication.Utilities.UpdateUsing_DIAPI("Z_HR_OSELAPP", DocNo, oHTUpdateCol)
                End If
            End If

            strQuery = "Update [@Z_HR_OSEAPP] set U_Z_SCkApp='" & strChkStatus & "', U_Z_WStatus='" & strWStatus & "' , U_Z_BSelfRemark='" & strBRmark & "' ,  U_Z_PSelfRemark='" & strPRmark & "' ,  U_Z_CSelfRemark='" & strCRmark & "' Where DocEntry=" & DocNo & ""
            oHTUpdateCol.Clear()
            oHTUpdateCol.Add("U_Z_SCkApp", strChkStatus)
            oHTUpdateCol.Add("U_Z_WStatus", strWStatus)
            oHTUpdateCol.Add("U_Z_BSelfRemark", strBRmark)
            oHTUpdateCol.Add("U_Z_PSelfRemark", strPRmark)
            oHTUpdateCol.Add("U_Z_CSelfRemark", strCRmark)
            oApplication.Utilities.UpdateUsing_DIAPI("Z_HR_OSELAPP", DocNo, oHTUpdateCol)
            'oRec.DoQuery(strQuery)
            For i = 1 To oHashB.Count
                strQuery1 = "Update [@Z_HR_SEAPP1] set U_Z_BussSelfRate='" & oHashB(i).ToString() & "' Where LineId=" & i & " and DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery1)
            Next
            For i = 1 To oHashP.Count
                strQuery2 = "Update [@Z_HR_SEAPP2] set U_Z_PeoSelfRate='" & oHashP(i).ToString() & "' Where LineId=" & i & " and DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery2)
            Next
            For i = 1 To oHashC.Count
                strQuery3 = "Update [@Z_HR_SEAPP3] set  U_Z_CompSelfRate = '" & oHashC(i).ToString() & "',U_Z_CompSelf = '" & oHashC(i).ToString() & "' Where LineId=" & i & " and DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery3)
            Next

        ElseIf intFlag = 2 Then
            Dim strQuery As String = ""
            Dim strQuery1 As String = ""
            Dim strQuery2 As String = ""
            Dim strQuery3 As String = ""
            Dim i, j, k As Integer
            strQuery = "Update [@Z_HR_OSEAPP] set U_Z_LCkApp='" & strChkStatus & "',U_Z_WStatus='" & strWStatus & "' , U_Z_BMgrRemark='" & strBRmark & "' ,  U_Z_PMgrRemark='" & strPRmark & "' ,  U_Z_CMgrRemark='" & strCRmark & "' Where DocEntry=" & DocNo & ""
            oHTUpdateCol.Clear()
            oHTUpdateCol.Add("U_Z_LCkApp", strChkStatus)
            oHTUpdateCol.Add("U_Z_WStatus", strWStatus)
            oHTUpdateCol.Add("U_Z_BMgrRemark", strBRmark)
            oHTUpdateCol.Add("U_Z_PMgrRemark", strPRmark)
            oHTUpdateCol.Add("U_Z_CMgrRemark", strCRmark)
            oApplication.Utilities.UpdateUsing_DIAPI("Z_HR_OSELAPP", DocNo, oHTUpdateCol)
            'oRec.DoQuery(strQuery)
            For i = 1 To oHashB.Count
                strQuery1 = "Update [@Z_HR_SEAPP1] set U_Z_BussMgrRate='" & oHashB(i).ToString() & "' Where LineId=" & i & " and DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery1)
            Next
            For i = 1 To oHashP.Count
                strQuery2 = "Update [@Z_HR_SEAPP2] set U_Z_PeoMgrRate='" & oHashP(i).ToString() & "' Where LineId=" & i & " and DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery2)
            Next
            For i = 1 To oHashC.Count
                strQuery3 = "Update [@Z_HR_SEAPP3] set U_Z_CompMgrRate = '" & oHashC(i).ToString() & "' Where LineId=" & i & " and DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery3)
            Next

            If strChkStatus = "Y" Then
                strQuery = "Update [@Z_HR_OSEAPP] set U_Z_LMNotify='Y' Where DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery)
            End If

        ElseIf intFlag = 3 Then
            Dim strQuery As String = ""
            Dim strQuery1 As String = ""
            Dim strQuery2 As String = ""
            Dim strQuery3 As String = ""
            Dim i, j, k As Integer
            strQuery = "Update [@Z_HR_OSEAPP] set U_Z_SrCkApp='" & strChkStatus & "',U_Z_WStatus='" & strWStatus & "' , U_Z_BSMrRemark='" & strBRmark & "' ,  U_Z_PSMrRemark='" & strPRmark & "' ,  U_Z_CSMrRemark='" & strCRmark & "' Where DocEntry=" & DocNo & ""
            'oRec.DoQuery(strQuery)
            oHTUpdateCol.Clear()
            oHTUpdateCol.Add("U_Z_SrCkApp", strChkStatus)
            oHTUpdateCol.Add("U_Z_WStatus", strWStatus)
            oHTUpdateCol.Add("U_Z_BSMrRemark", strBRmark)
            oHTUpdateCol.Add("U_Z_PSMrRemark", strPRmark)
            oHTUpdateCol.Add("U_Z_CSMrRemark", strCRmark)
            oApplication.Utilities.UpdateUsing_DIAPI("Z_HR_OSELAPP", DocNo, oHTUpdateCol)
            For i = 1 To oHashB.Count
                strQuery1 = "Update [@Z_HR_SEAPP1] set U_Z_BussSMRate='" & oHashB(i).ToString() & "' Where LineId=" & i & " and DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery1)
            Next
            For i = 1 To oHashP.Count
                strQuery2 = "Update [@Z_HR_SEAPP2] set U_Z_PeoSMRate='" & oHashP(i).ToString() & "' Where LineId=" & i & " and DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery2)
            Next
            For i = 1 To oHashC.Count
                strQuery3 = "Update [@Z_HR_SEAPP3] set U_Z_CompSMRate = '" & oHashC(i).ToString() & "' Where LineId=" & i & " and DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery3)
            Next
        End If
        Dim stRe1, stRe2, stRe3, stRe4 As String
        stRe1 = oApplication.Utilities.getEdittextvalue(oForm, "15")
        stRe2 = oApplication.Utilities.getEdittextvalue(oForm, "16")
        stRe3 = oApplication.Utilities.getEdittextvalue(oForm, "17")
        stRe4 = oApplication.Utilities.getEdittextvalue(oForm, "18")
        Dim otest As SAPbobsCOM.Recordset
        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        'otest.DoQuery("Update [@Z_HR_OSEAPP] set U_Z_BSelfRemark='" & stRe1 & "',U_Z_BMgrRemark='" & stRe2 & "' ,U_Z_BSMrRemark='" & stRe3 & "',U_Z_BHrRemark ='" & stRe4 & "' where Docentry=" & DocNo)
        oHTUpdateCol.Clear()
        oHTUpdateCol.Add("U_Z_BSelfRemark", stRe1)
        oHTUpdateCol.Add("U_Z_BMgrRemark", stRe2)
        oHTUpdateCol.Add("U_Z_BSMrRemark", stRe3)
        oHTUpdateCol.Add("U_Z_BHrRemark", stRe4)
        oApplication.Utilities.UpdateUsing_DIAPI("Z_HR_OSELAPP", DocNo, oHTUpdateCol)
        oApplication.Utilities.Message("Updated Sucessfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
    End Sub

    Private Sub UpdateDocument1(ByVal oHashB As Hashtable, ByVal oHashP As Hashtable, ByVal oHashC As Hashtable, ByVal strBRmark As String, ByVal strPRmark As String, ByVal strCRmark As String, ByVal strWStatus As String, ByVal DocNo As Integer, ByVal intFlag As Integer, ByVal oHGrevence As Hashtable, ByVal strChkStatus As String)
        Dim oRec As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oHTUpdateCol = New Hashtable()

        If intFlag = 1 Then
            Dim strQuery As String = ""
            Dim strQuery1 As String = ""
            Dim strQuery2 As String = ""
            Dim strQuery3 As String = ""
            Dim i, j, k As Integer

            If oHGrevence.Count > 0 Then
                If oHGrevence(1).ToString() = "Y" Then
                    If oHGrevence(2).ToString() = "A" Then
                        strQuery = "Update [@Z_HR_OSEAPP] set U_Z_SCkApp='" & strChkStatus & "',U_Z_WStatus='" & strWStatus & "' , U_Z_BSelfRemark='" & strBRmark & "' ,  U_Z_PSelfRemark='" & strPRmark & "' ,  U_Z_CSelfRemark='" & strCRmark & "',U_Z_GStatus='" & oHGrevence(2).ToString() & "' Where DocEntry=" & DocNo & ""
                        oHTUpdateCol.Add("U_Z_SCkApp", strChkStatus)
                        oHTUpdateCol.Add("U_Z_WStatus", strWStatus)
                        oHTUpdateCol.Add("U_Z_BSelfRemark", strBRmark)
                        oHTUpdateCol.Add("U_Z_PSelfRemark", strPRmark)
                        oHTUpdateCol.Add("U_Z_CSelfRemark", strCRmark)
                        oHTUpdateCol.Add("U_Z_GStatus", oHGrevence(2).ToString())
                    Else
                        If oHGrevence(2).ToString = "-" Then
                            strQuery = "Update [@Z_HR_OSEAPP] set U_Z_SCkApp='" & strChkStatus & "',U_Z_WStatus='" & strWStatus & "' , U_Z_BSelfRemark='" & strBRmark & "' ,  U_Z_PSelfRemark='" & strPRmark & "' ,  U_Z_CSelfRemark='" & strCRmark & "',U_Z_GStatus='" & oHGrevence(2).ToString() & "' Where DocEntry=" & DocNo & ""
                            oHTUpdateCol.Add("U_Z_SCkApp", strChkStatus)
                            oHTUpdateCol.Add("U_Z_WStatus", strWStatus)
                            oHTUpdateCol.Add("U_Z_BSelfRemark", strBRmark)
                            oHTUpdateCol.Add("U_Z_PSelfRemark", strPRmark)
                            oHTUpdateCol.Add("U_Z_CSelfRemark", strCRmark)
                            oHTUpdateCol.Add("U_Z_GStatus", oHGrevence(2).ToString())
                        Else
                            strQuery = "Update [@Z_HR_OSEAPP] set U_Z_SCkApp='" & strChkStatus & "',U_Z_WStatus='" & strWStatus & "' , U_Z_BSelfRemark='" & strBRmark & "' ,  U_Z_PSelfRemark='" & strPRmark & "' ,  U_Z_CSelfRemark='" & strCRmark & "',U_Z_GStatus='" & oHGrevence(2).ToString() & "',U_Z_GDate='" & oHGrevence(3).ToString() & "',U_Z_GNo='" & oHGrevence(4).ToString() & "',U_Z_GRemarks='" & oHGrevence(5).ToString() & "' Where DocEntry=" & DocNo & ""
                            oHTUpdateCol.Add("U_Z_SCkApp", strChkStatus)
                            oHTUpdateCol.Add("U_Z_WStatus", strWStatus)
                            oHTUpdateCol.Add("U_Z_BSelfRemark", strBRmark)
                            oHTUpdateCol.Add("U_Z_PSelfRemark", strPRmark)
                            oHTUpdateCol.Add("U_Z_CSelfRemark", strCRmark)
                            oHTUpdateCol.Add("U_Z_GStatus", oHGrevence(2).ToString())
                            oHTUpdateCol.Add("U_Z_GDate", oHGrevence(3).ToString())
                            oHTUpdateCol.Add("U_Z_GNo", oHGrevence(4).ToString())
                            oHTUpdateCol.Add("U_Z_GRemarks", oHGrevence(5).ToString())
                        End If
                    End If
                    'oRec.DoQuery(strQuery)
                    oApplication.Utilities.UpdateUsing_DIAPI("Z_HR_OSELAPP", DocNo, oHTUpdateCol)
                End If
            End If

            strQuery = "Update [@Z_HR_OSEAPP] set U_Z_SCkApp='" & strChkStatus & "', U_Z_WStatus='" & strWStatus & "' , U_Z_BSelfRemark='" & strBRmark & "' ,  U_Z_PSelfRemark='" & strPRmark & "' ,  U_Z_CSelfRemark='" & strCRmark & "' Where DocEntry=" & DocNo & ""
            oHTUpdateCol.Clear()
            oHTUpdateCol.Add("U_Z_SCkApp", strChkStatus)
            oHTUpdateCol.Add("U_Z_WStatus", strWStatus)
            oHTUpdateCol.Add("U_Z_BSelfRemark", strBRmark)
            oHTUpdateCol.Add("U_Z_PSelfRemark", strPRmark)
            oHTUpdateCol.Add("U_Z_CSelfRemark", strCRmark)
            oApplication.Utilities.UpdateUsing_DIAPI("Z_HR_OSELAPP", DocNo, oHTUpdateCol)
            'oRec.DoQuery(strQuery)
            For i = 1 To oHashB.Count
                strQuery1 = "Update [@Z_HR_SEAPP1] set U_Z_SelfRaCode='" & oHashB(i).ToString() & "' Where LineId=" & i & " and DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery1)
            Next
            For i = 1 To oHashP.Count
                strQuery2 = "Update [@Z_HR_SEAPP2] set U_Z_SelfRaCode='" & oHashP(i).ToString() & "' Where LineId=" & i & " and DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery2)
            Next
            For i = 1 To oHashC.Count
                strQuery3 = "Update [@Z_HR_SEAPP3] set U_Z_SelfRaCode = '" & oHashC(i).ToString() & "' Where LineId=" & i & " and DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery3)
            Next

        ElseIf intFlag = 2 Then
            Dim strQuery As String = ""
            Dim strQuery1 As String = ""
            Dim strQuery2 As String = ""
            Dim strQuery3 As String = ""
            Dim i, j, k As Integer
            strQuery = "Update [@Z_HR_OSEAPP] set U_Z_LCkApp='" & strChkStatus & "',U_Z_WStatus='" & strWStatus & "' , U_Z_BMgrRemark='" & strBRmark & "' ,  U_Z_PMgrRemark='" & strPRmark & "' ,  U_Z_CMgrRemark='" & strCRmark & "' Where DocEntry=" & DocNo & ""
            oHTUpdateCol.Clear()
            oHTUpdateCol.Add("U_Z_LCkApp", strChkStatus)
            oHTUpdateCol.Add("U_Z_WStatus", strWStatus)
            oHTUpdateCol.Add("U_Z_BMgrRemark", strBRmark)
            oHTUpdateCol.Add("U_Z_PMgrRemark", strPRmark)
            oHTUpdateCol.Add("U_Z_CMgrRemark", strCRmark)
            oApplication.Utilities.UpdateUsing_DIAPI("Z_HR_OSELAPP", DocNo, oHTUpdateCol)
            'oRec.DoQuery(strQuery)
            For i = 1 To oHashB.Count
                strQuery1 = "Update [@Z_HR_SEAPP1] set U_Z_MgrRaCode='" & oHashB(i).ToString() & "' Where LineId=" & i & " and DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery1)
            Next
            For i = 1 To oHashP.Count
                strQuery2 = "Update [@Z_HR_SEAPP2] set U_Z_MgrRaCode='" & oHashP(i).ToString() & "' Where LineId=" & i & " and DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery2)
            Next
            For i = 1 To oHashC.Count
                strQuery3 = "Update [@Z_HR_SEAPP3] set U_Z_MgrRaCode = '" & oHashC(i).ToString() & "' Where LineId=" & i & " and DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery3)
            Next

            If strChkStatus = "Y" Then
                strQuery = "Update [@Z_HR_OSEAPP] set U_Z_LMNotify='Y' Where DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery)
            End If

        ElseIf intFlag = 3 Then
            Dim strQuery As String = ""
            Dim strQuery1 As String = ""
            Dim strQuery2 As String = ""
            Dim strQuery3 As String = ""
            Dim i, j, k As Integer
            strQuery = "Update [@Z_HR_OSEAPP] set U_Z_SrCkApp='" & strChkStatus & "',U_Z_WStatus='" & strWStatus & "' , U_Z_BSMrRemark='" & strBRmark & "' ,  U_Z_PSMrRemark='" & strPRmark & "' ,  U_Z_CSMrRemark='" & strCRmark & "' Where DocEntry=" & DocNo & ""
            'oRec.DoQuery(strQuery)
            oHTUpdateCol.Clear()
            oHTUpdateCol.Add("U_Z_SrCkApp", strChkStatus)
            oHTUpdateCol.Add("U_Z_WStatus", strWStatus)
            oHTUpdateCol.Add("U_Z_BSMrRemark", strBRmark)
            oHTUpdateCol.Add("U_Z_PSMrRemark", strPRmark)
            oHTUpdateCol.Add("U_Z_CSMrRemark", strCRmark)
            oApplication.Utilities.UpdateUsing_DIAPI("Z_HR_OSELAPP", DocNo, oHTUpdateCol)
            For i = 1 To oHashB.Count
                strQuery1 = "Update [@Z_HR_SEAPP1] set U_Z_SMRaCode='" & oHashB(i).ToString() & "' Where LineId=" & i & " and DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery1)
            Next
            For i = 1 To oHashP.Count
                strQuery2 = "Update [@Z_HR_SEAPP2] set U_Z_SMRaCode='" & oHashP(i).ToString() & "' Where LineId=" & i & " and DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery2)
            Next
            For i = 1 To oHashC.Count
                strQuery3 = "Update [@Z_HR_SEAPP3] set U_Z_SMRaCode = '" & oHashC(i).ToString() & "' Where LineId=" & i & " and DocEntry=" & DocNo & ""
                oRec.DoQuery(strQuery3)
            Next
        End If
        Dim stRe1, stRe2, stRe3, stRe4 As String
        stRe1 = oApplication.Utilities.getEdittextvalue(oForm, "15")
        stRe2 = oApplication.Utilities.getEdittextvalue(oForm, "16")
        stRe3 = oApplication.Utilities.getEdittextvalue(oForm, "17")
        stRe4 = oApplication.Utilities.getEdittextvalue(oForm, "18")
        Dim otest As SAPbobsCOM.Recordset
        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        'otest.DoQuery("Update [@Z_HR_OSEAPP] set U_Z_BSelfRemark='" & stRe1 & "',U_Z_BMgrRemark='" & stRe2 & "' ,U_Z_BSMrRemark='" & stRe3 & "',U_Z_BHrRemark ='" & stRe4 & "' where Docentry=" & DocNo)
        oHTUpdateCol.Clear()
        oHTUpdateCol.Add("U_Z_BSelfRemark", stRe1)
        oHTUpdateCol.Add("U_Z_BMgrRemark", stRe2)
        oHTUpdateCol.Add("U_Z_BSMrRemark", stRe3)
        oHTUpdateCol.Add("U_Z_BHrRemark", stRe4)
        oApplication.Utilities.UpdateUsing_DIAPI("Z_HR_OSELAPP", DocNo, oHTUpdateCol)
        oApplication.Utilities.Message("Updated Sucessfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
    End Sub
#End Region

    Private Sub InitializeAppTable()
        oForm.DataSources.DataTables.Add("dtAppraisal")
        oDtAppraisal = oForm.DataSources.DataTables.Item("dtAppraisal")
        oDtAppraisal.Columns.Add("DocEntry", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
        oDtAppraisal.Columns.Add("Name", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
        oDtAppraisal.Columns.Add("toID", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
        oDtAppraisal.Columns.Add("ccID", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
        oDtAppraisal.Columns.Add("Type", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
        oDtAppraisal.Columns.Add("Path", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
    End Sub


#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_FApproval Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If (pVal.ItemUID = "36" Or pVal.ItemUID = "37") And pVal.CharPressed <> 9 Then
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                If pVal.ItemUID = "3" And pVal.ColUID = "DocEntry" Then
                                    oGrid = oForm.Items.Item("3").Specific
                                    For intRow As Integer = pVal.Row To pVal.Row
                                        If 1 = 1 Then
                                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                            Dim strcode, strstatus As String
                                            strcode = oGrid.DataTable.GetValue("DocEntry", intRow)
                                            strstatus = oGrid.DataTable.GetValue("U_Z_Status", intRow)
                                            Dim objct As New clshrSelfAppraisal
                                            objct.LoadForm(strcode, oForm.Title, strstatus)
                                        End If
                                    Next
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                If pVal.ItemUID = "29" Or pVal.ItemUID = "44" Then
                                    Dim strCol As String
                                    If oForm.Title = "Self Appraisals" Then
                                        strCol = "Self Rating"
                                    ElseIf oForm.Title = "First Level Approval" Then
                                        strCol = "Line Manager Rating"
                                    ElseIf oForm.Title = "Second Level Approval" Then
                                        strCol = "Sr.Manager Rating"
                                    End If

                                    If oApplication.SBO_Application.MessageBox("Do you want to confirm the changes?", , "Yes", "NO") = 2 Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If

                                    If oForm.Title <> "HR Acceptance" Then
                                        If validate(strCol) = False Then
                                            oApplication.Utilities.Message("Rating Should be Less Than or Equal to 100...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    End If
                                ElseIf pVal.ItemUID = "_3" Then
                                    If oApplication.SBO_Application.MessageBox("Do you want to Cancel Selected Appraisal?", , "Yes", "NO") = 2 Then
                                        BubbleEvent = False
                                        Exit Sub
                                    Else
                                        UpdateAppraisal()
                                        oApplication.SBO_Application.StatusBar.SetText("Selected Appraisal Cancelled Sucessfully....", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                    End If
                                ElseIf pVal.ItemUID = "_2" Then
                                    oForm.Close()
                                End If
                        End Select
                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                reDrawForm(oForm)
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                'oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT

                                If pVal.ItemUID = "10" And pVal.ColUID = "U_Z_SelfRaCode" Then
                                    Dim oGrid1 As SAPbouiCOM.Grid
                                    oGrid1 = oForm.Items.Item("10").Specific
                                    Dim oComboColumn As SAPbouiCOM.ComboBoxColumn
                                    oComboColumn = oGrid1.Columns.Item("U_Z_SelfRaCode")
                                    Dim strRate, strWeight, strQuery, strRateValue, strFinalRate, strSelfRate As String
                                    Dim dblRateValue, dblExpectedLevel, dblweight, dblSelfRate, dblFinalRate As Double
                                    Dim oRateRs As SAPbobsCOM.Recordset
                                    strSelfRate = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    strWeight = oGrid1.DataTable.GetValue("Weight (%)", pVal.Row)

                                    dblExpectedLevel = oApplication.Utilities.getDocumentQuantity(oGrid1.DataTable.GetValue("Levels", pVal.Row))

                                    dblSelfRate = oApplication.Utilities.getDocumentQuantity(strSelfRate)

                                    dblweight = oApplication.Utilities.getDocumentQuantity(strWeight)

                                    If dblSelfRate > dblExpectedLevel Then
                                        dblFinalRate = dblweight
                                    Else
                                        dblFinalRate = dblweight / dblExpectedLevel * dblSelfRate
                                    End If
                                    oGrid1.DataTable.SetValue("Self Rating", pVal.Row, dblFinalRate)

                                End If

                                If pVal.ItemUID = "10" And pVal.ColUID = "U_Z_SelfRaCode" Then
                                    Dim oGrid1 As SAPbouiCOM.Grid
                                    oGrid1 = oForm.Items.Item("10").Specific
                                    Dim oComboColumn As SAPbouiCOM.ComboBoxColumn
                                    oComboColumn = oGrid1.Columns.Item("U_Z_SelfRaCode")
                                    Dim strRate, strWeight, strQuery, strRateValue, strFinalRate, strSelfRate As String
                                    Dim dblRateValue, dblExpectedLevel, dblweight, dblSelfRate, dblFinalRate As Double
                                    Dim oRateRs As SAPbobsCOM.Recordset
                                    strSelfRate = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    strWeight = oGrid1.DataTable.GetValue("Weight (%)", pVal.Row)

                                    dblExpectedLevel = oApplication.Utilities.getDocumentQuantity(oGrid1.DataTable.GetValue("Levels", pVal.Row))

                                    dblSelfRate = oApplication.Utilities.getDocumentQuantity(strSelfRate)

                                    dblweight = oApplication.Utilities.getDocumentQuantity(strWeight)

                                    If dblSelfRate > dblExpectedLevel Then
                                        dblFinalRate = dblweight
                                    Else
                                        dblFinalRate = dblweight / dblExpectedLevel * dblSelfRate
                                    End If
                                    oGrid1.DataTable.SetValue("Self Rating", pVal.Row, dblFinalRate)

                                End If

                                If pVal.ItemUID = "10" And pVal.ColUID = "U_Z_MgrRaCode" Then
                                    Dim oGrid1 As SAPbouiCOM.Grid
                                    oGrid1 = oForm.Items.Item("10").Specific
                                    Dim oComboColumn As SAPbouiCOM.ComboBoxColumn
                                    oComboColumn = oGrid1.Columns.Item("U_Z_MgrRaCode")
                                    Dim strRate, strWeight, strQuery, strRateValue, strFinalRate, strSelfRate As String
                                    Dim dblRateValue, dblExpectedLevel, dblweight, dblSelfRate, dblFinalRate As Double
                                    Dim oRateRs As SAPbobsCOM.Recordset
                                    strSelfRate = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    strWeight = oGrid1.DataTable.GetValue("Weight (%)", pVal.Row)

                                    dblExpectedLevel = oApplication.Utilities.getDocumentQuantity(oGrid1.DataTable.GetValue("Levels", pVal.Row))

                                    dblSelfRate = oApplication.Utilities.getDocumentQuantity(strSelfRate)

                                    dblweight = oApplication.Utilities.getDocumentQuantity(strWeight)

                                    If dblSelfRate > dblExpectedLevel Then
                                        dblFinalRate = dblweight
                                    Else
                                        dblFinalRate = dblweight / dblExpectedLevel * dblSelfRate
                                    End If
                                    oGrid1.DataTable.SetValue("Line Manager Rating", pVal.Row, dblFinalRate)

                                End If

                                If pVal.ItemUID = "10" And pVal.ColUID = "U_Z_SMRaCode" Then
                                    Dim oGrid1 As SAPbouiCOM.Grid
                                    oGrid1 = oForm.Items.Item("10").Specific
                                    Dim oComboColumn As SAPbouiCOM.ComboBoxColumn
                                    oComboColumn = oGrid1.Columns.Item("U_Z_SMRaCode")
                                    Dim strRate, strWeight, strQuery, strRateValue, strFinalRate, strSelfRate As String
                                    Dim dblRateValue, dblExpectedLevel, dblweight, dblSelfRate, dblFinalRate As Double
                                    Dim oRateRs As SAPbobsCOM.Recordset
                                    strSelfRate = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    strWeight = oGrid1.DataTable.GetValue("Weight (%)", pVal.Row)

                                    dblExpectedLevel = oApplication.Utilities.getDocumentQuantity(oGrid1.DataTable.GetValue("Levels", pVal.Row))

                                    dblSelfRate = oApplication.Utilities.getDocumentQuantity(strSelfRate)

                                    dblweight = oApplication.Utilities.getDocumentQuantity(strWeight)

                                    If dblSelfRate > dblExpectedLevel Then
                                        dblFinalRate = dblweight
                                    Else
                                        dblFinalRate = dblweight / dblExpectedLevel * dblSelfRate
                                    End If
                                    oGrid1.DataTable.SetValue("Sr.Manager Rating", pVal.Row, dblFinalRate)

                                End If


                                If pVal.ItemUID = "9" And pVal.ColUID = "U_Z_SelfRaCode" Then
                                    Dim oGrid1 As SAPbouiCOM.Grid
                                    oGrid1 = oForm.Items.Item("9").Specific
                                    Dim oComboColumn As SAPbouiCOM.ComboBoxColumn
                                    oComboColumn = oGrid1.Columns.Item("U_Z_SelfRaCode")
                                    Dim strRate, strWeight, strQuery, strRateValue, strFinalRate As String
                                    Dim dblRateValue, dblVate, dblweight As Double
                                    Dim oRateRs As SAPbobsCOM.Recordset
                                    strRate = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    strWeight = oGrid1.DataTable.GetValue("Weight (%)", pVal.Row)

                                    strQuery = "Select * from [@Z_HR_ORATE] where U_Z_RateCode='" & strRate & "'"
                                    oRateRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    oRateRs.DoQuery(strQuery)
                                    dblVate = oRateRs.Fields.Item("U_Z_Total").Value
                                    dblRateValue = oApplication.Utilities.getDocumentQuantity(strRate)
                                    dblweight = oApplication.Utilities.getDocumentQuantity(strWeight)
                                    dblVate = dblVate * dblweight
                                    oGrid1.DataTable.SetValue("Self Rating", pVal.Row, dblVate / 100)


                                End If

                                If pVal.ItemUID = "9" And pVal.ColUID = "U_Z_MgrRaCode" Then
                                    Dim oGrid1 As SAPbouiCOM.Grid
                                    oGrid1 = oForm.Items.Item("9").Specific
                                    Dim oComboColumn As SAPbouiCOM.ComboBoxColumn
                                    oComboColumn = oGrid1.Columns.Item("U_Z_MgrRaCode")
                                    Dim strRate, strWeight, strQuery, strRateValue, strFinalRate As String
                                    Dim dblRateValue, dblVate, dblweight As Double
                                    Dim oRateRs As SAPbobsCOM.Recordset
                                    strRate = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    strWeight = oGrid1.DataTable.GetValue("Weight (%)", pVal.Row)

                                    strQuery = "Select * from [@Z_HR_ORATE] where U_Z_RateCode='" & strRate & "'"
                                    oRateRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    oRateRs.DoQuery(strQuery)
                                    dblVate = oRateRs.Fields.Item("U_Z_Total").Value
                                    dblRateValue = oApplication.Utilities.getDocumentQuantity(strRate)
                                    dblweight = oApplication.Utilities.getDocumentQuantity(strWeight)
                                    dblVate = dblVate * dblweight
                                    oGrid1.DataTable.SetValue("Line Manager Rating", pVal.Row, dblVate / 100)


                                End If

                                If pVal.ItemUID = "9" And pVal.ColUID = "U_Z_SMRaCode" Then
                                    Dim oGrid1 As SAPbouiCOM.Grid
                                    oGrid1 = oForm.Items.Item("9").Specific
                                    Dim oComboColumn As SAPbouiCOM.ComboBoxColumn
                                    oComboColumn = oGrid1.Columns.Item("U_Z_SMRaCode")
                                    Dim strRate, strWeight, strQuery, strRateValue, strFinalRate As String
                                    Dim dblRateValue, dblVate, dblweight As Double
                                    Dim oRateRs As SAPbobsCOM.Recordset
                                    strRate = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    strWeight = oGrid1.DataTable.GetValue("Weight (%)", pVal.Row)

                                    strQuery = "Select * from [@Z_HR_ORATE] where U_Z_RateCode='" & strRate & "'"
                                    oRateRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    oRateRs.DoQuery(strQuery)
                                    dblVate = oRateRs.Fields.Item("U_Z_Total").Value
                                    dblRateValue = oApplication.Utilities.getDocumentQuantity(strRate)
                                    dblweight = oApplication.Utilities.getDocumentQuantity(strWeight)
                                    dblVate = dblVate * dblweight
                                    oGrid1.DataTable.SetValue("Sr.Manager Rating", pVal.Row, dblVate / 100)

                                End If



                                If pVal.ItemUID = "8" And pVal.ColUID = "U_Z_SelfRaCode" Then
                                    Dim oGrid1 As SAPbouiCOM.Grid
                                    oGrid1 = oForm.Items.Item("8").Specific
                                    Dim oComboColumn As SAPbouiCOM.ComboBoxColumn
                                    oComboColumn = oGrid1.Columns.Item("U_Z_SelfRaCode")
                                    Dim strRate, strWeight, strQuery, strRateValue, strFinalRate As String
                                    Dim dblRateValue, dblVate, dblweight As Double
                                    Dim oRateRs As SAPbobsCOM.Recordset
                                    strRate = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    strWeight = oGrid1.DataTable.GetValue("Weight (%)", pVal.Row)

                                    strQuery = "Select * from [@Z_HR_ORATE] where U_Z_RateCode='" & strRate & "'"
                                    oRateRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    oRateRs.DoQuery(strQuery)
                                    dblVate = oRateRs.Fields.Item("U_Z_Total").Value
                                    dblRateValue = oApplication.Utilities.getDocumentQuantity(strRate)
                                    dblweight = oApplication.Utilities.getDocumentQuantity(strWeight)
                                    dblVate = dblVate * dblweight
                                    oGrid1.DataTable.SetValue("Self Rating", pVal.Row, dblVate / 100)


                                End If

                                If pVal.ItemUID = "8" And pVal.ColUID = "U_Z_MgrRaCode" Then
                                    Dim oGrid1 As SAPbouiCOM.Grid
                                    oGrid1 = oForm.Items.Item("8").Specific
                                    Dim oComboColumn As SAPbouiCOM.ComboBoxColumn
                                    oComboColumn = oGrid1.Columns.Item("U_Z_MgrRaCode")
                                    Dim strRate, strWeight, strQuery, strRateValue, strFinalRate As String
                                    Dim dblRateValue, dblVate, dblweight As Double
                                    Dim oRateRs As SAPbobsCOM.Recordset
                                    strRate = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    strWeight = oGrid1.DataTable.GetValue("Weight (%)", pVal.Row)

                                    strQuery = "Select * from [@Z_HR_ORATE] where U_Z_RateCode='" & strRate & "'"
                                    oRateRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    oRateRs.DoQuery(strQuery)
                                    dblVate = oRateRs.Fields.Item("U_Z_Total").Value
                                    dblRateValue = oApplication.Utilities.getDocumentQuantity(strRate)
                                    dblweight = oApplication.Utilities.getDocumentQuantity(strWeight)
                                    dblVate = dblVate * dblweight
                                    oGrid1.DataTable.SetValue("Line Manager Rating", pVal.Row, dblVate / 100)


                                End If

                                If pVal.ItemUID = "8" And pVal.ColUID = "U_Z_SMRaCode" Then
                                    Dim oGrid1 As SAPbouiCOM.Grid
                                    oGrid1 = oForm.Items.Item("8").Specific
                                    Dim oComboColumn As SAPbouiCOM.ComboBoxColumn
                                    oComboColumn = oGrid1.Columns.Item("U_Z_SMRaCode")
                                    Dim strRate, strWeight, strQuery, strRateValue, strFinalRate As String
                                    Dim dblRateValue, dblVate, dblweight As Double
                                    Dim oRateRs As SAPbobsCOM.Recordset
                                    strRate = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    strWeight = oGrid1.DataTable.GetValue("Weight (%)", pVal.Row)

                                    strQuery = "Select * from [@Z_HR_ORATE] where U_Z_RateCode='" & strRate & "'"
                                    oRateRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    oRateRs.DoQuery(strQuery)
                                    dblVate = oRateRs.Fields.Item("U_Z_Total").Value
                                    dblRateValue = oApplication.Utilities.getDocumentQuantity(strRate)
                                    dblweight = oApplication.Utilities.getDocumentQuantity(strWeight)
                                    dblVate = dblVate * dblweight
                                    oGrid1.DataTable.SetValue("Sr.Manager Rating", pVal.Row, dblVate / 100)

                                End If


                                If pVal.ItemUID = "32" Then
                                    Dim oComboStatus As SAPbouiCOM.ComboBox
                                    oComboStatus = oForm.Items.Item("32").Specific
                                    If oComboStatus.Selected.Value = "G" Then
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = 3
                                        oForm.Items.Item("30").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                        oForm.Items.Item("36").Enabled = True
                                        oForm.Items.Item("37").Enabled = True
                                        oApplication.Utilities.setEdittextvalue(oForm, "36", System.DateTime.Today.ToString("yyyyMMdd"))
                                        Dim oRSGNo As SAPbobsCOM.Recordset
                                        Dim strGNo As String
                                        oRSGNo = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                        Dim strQueryGNo As String = "Select max(isnull(U_Z_GNo,'0'))+1 as 'GNo' from [@Z_HR_OSEAPP]"
                                        oRSGNo.DoQuery(strQueryGNo)
                                        If Not oRSGNo.EoF Then
                                            strGNo = oRSGNo.Fields.Item("GNo").Value.ToString()
                                        End If
                                        oApplication.Utilities.setEdittextvalue(oForm, "37", strGNo)
                                        oForm.Items.Item("38").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                        Try

                                            oForm.Items.Item("36").Enabled = False
                                            oForm.Items.Item("37").Enabled = False

                                        Catch ex As Exception

                                        End Try
                                        oForm.Freeze(False)
                                        oForm.Update()
                                    ElseIf oComboStatus.Selected.Value = "A" Then
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = oForm.PaneLevel
                                        oApplication.Utilities.setEdittextvalue(oForm, "36", "")
                                        oApplication.Utilities.setEdittextvalue(oForm, "37", "")
                                        oApplication.Utilities.setEdittextvalue(oForm, "38", "")
                                        oForm.Items.Item("1000001").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                        oForm.Freeze(False)
                                    End If
                                End If


                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim strCode, strstatus As String
                                If pVal.ItemUID = "4" Then
                                    oGrid = oForm.Items.Item("3").Specific
                                    For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                        If oGrid.Rows.IsSelected(intRow) Then
                                            strCode = oGrid.DataTable.GetValue("DocEntry", intRow)
                                            strstatus = oGrid.DataTable.GetValue("U_Z_Status", intRow)
                                            Dim objct As New clshrSelfAppraisal
                                            'If strstatus = "Draft" And oForm.Title = "Manager Appraisal Approval" Then
                                            objct.LoadForm(strCode, oForm.Title, strstatus)
                                            ''ElseIf strstatus = "2nd Level Approval" And oForm.Title = "HR Appraisal Approval" Then
                                            ' objct.LoadForm(strCode, oForm.Title, strstatus)
                                            'Else
                                            ''  oApplication.Utilities.Message("You are not perform this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            'End If
                                        End If
                                    Next
                                ElseIf pVal.ItemUID = "79" Then
                                    oGrid = oForm.Items.Item("3").Specific
                                    For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                        If oGrid.Rows.IsSelected(intRow) Then
                                            strCode = oGrid.DataTable.GetValue("DocEntry", intRow)
                                            oApplication.Utilities.PrintReport(strCode)
                                            Exit Sub
                                        End If
                                    Next
                                ElseIf pVal.ItemUID = "29" Or pVal.ItemUID = "44" Or pVal.ItemUID = "btnGra" Then
                                    oGrid = oForm.Items.Item("3").Specific
                                    oGrid_P1 = oForm.Items.Item("8").Specific
                                    oGrid_P2 = oForm.Items.Item("9").Specific
                                    oGrid_P3 = oForm.Items.Item("10").Specific
                                    Dim oComboStatus As SAPbouiCOM.ComboBox
                                    oComboStatus = oForm.Items.Item("28").Specific
                                    Dim oHashBusRating As Hashtable
                                    Dim oHashPeoRating As Hashtable
                                    Dim oHashComRating As Hashtable

                                    Dim oHashBusRating1 As Hashtable
                                    Dim oHashPeoRating1 As Hashtable
                                    Dim oHashComRating1 As Hashtable
                                    Dim strBStatus As String
                                    Dim strPStatus As String
                                    Dim strCStatus As String
                                    Dim DocNo As Integer = 0

                                    If oGrid.Rows.Count > 0 Then
                                        Dim i As Integer
                                        For i = 0 To oGrid.Rows.Count - 1
                                            If oGrid.Rows.IsSelected(i) = True Then
                                                DocNo = Convert.ToInt32(oGrid.DataTable.GetValue("DocEntry", i))
                                            End If
                                        Next
                                    End If

                                    Dim strCol As String
                                    If oForm.Title = "Self Appraisals" Then
                                        strCol = "Self Rating"
                                    ElseIf oForm.Title = "First Level Approval" Then
                                        strCol = "Line Manager Rating"
                                    ElseIf oForm.Title = "Second Level Approval" Then
                                        strCol = "Sr.Manager Rating"
                                    End If


                                    'If Not validate(strCol) Then
                                    '    oApplication.Utilities.Message("Rating Should be Less than or Equal to 5...", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                    'End If

                                    If oForm.Title = "Self Appraisals" Then
                                        If oGrid_P1.Rows.Count > 0 Then
                                            oHashBusRating = New Hashtable()
                                            oHashBusRating1 = New Hashtable()
                                            Dim i As Integer
                                            For i = 0 To oGrid_P1.Rows.Count - 1
                                                oHashBusRating.Add(i + 1, oGrid_P1.DataTable.GetValue("Self Rating", i))
                                                oHashBusRating1.Add(i + 1, oGrid_P1.DataTable.GetValue("U_Z_SelfRaCode", i))
                                            Next
                                        End If

                                        If oGrid_P2.Rows.Count > 0 Then
                                            oHashPeoRating = New Hashtable()
                                            oHashPeoRating1 = New Hashtable()
                                            Dim i As Integer
                                            For i = 0 To oGrid_P2.Rows.Count - 1
                                                oHashPeoRating.Add(i + 1, oGrid_P2.DataTable.GetValue("Self Rating", i))
                                                oHashPeoRating1.Add(i + 1, oGrid_P2.DataTable.GetValue("U_Z_SelfRaCode", i))
                                            Next
                                        End If

                                        If oGrid_P3.Rows.Count > 0 Then
                                            oHashComRating = New Hashtable()
                                            oHashComRating1 = New Hashtable()
                                            Dim i As Integer
                                            For i = 0 To oGrid_P3.Rows.Count - 1
                                                oHashComRating.Add(i + 1, oGrid_P3.DataTable.GetValue("Self Rating", i))
                                                oHashComRating1.Add(i + 1, oGrid_P3.DataTable.GetValue("U_Z_SelfRaCode", i))
                                            Next
                                        End If
                                        Dim strworkflowstatus As String
                                        strstatus = oComboStatus.Selected.Value.ToString()
                                        strworkflowstatus = strstatus
                                        Dim oHashGrevence As Hashtable
                                        oHashGrevence = New Hashtable()
                                        If strstatus = "HR" Or strstatus = "LM" Or strstatus = "SM" Then
                                            Dim strGDate As String
                                            Dim oComboGStatus As SAPbouiCOM.ComboBox
                                            oComboGStatus = oForm.Items.Item("32").Specific
                                            If oComboGStatus.Selected.Value = "G" Then
                                                oHashGrevence.Add(1, "Y")
                                                oHashGrevence.Add(2, oComboGStatus.Selected.Value)
                                                If oApplication.Utilities.getEdittextvalue(oForm, "36").Length > 0 Then
                                                    strGDate = Convert.ToDateTime(oApplication.Utilities.getEdittextvalue(oForm, "36"))
                                                    strGDate = strGDate.Substring(6, 4) + strGDate.Substring(3, 2) + strGDate.Substring(0, 2)
                                                End If
                                                oHashGrevence.Add(3, strGDate)
                                                oHashGrevence.Add(4, oApplication.Utilities.getEdittextvalue(oForm, "37"))
                                                oHashGrevence.Add(5, oApplication.Utilities.getEdittextvalue(oForm, "38"))
                                            Else
                                                oHashGrevence.Add(1, "Y")
                                                oHashGrevence.Add(2, oComboGStatus.Selected.Value)
                                            End If
                                        End If
                                        strBStatus = oApplication.Utilities.getEdittextvalue(oForm, "15")
                                        strPStatus = oApplication.Utilities.getEdittextvalue(oForm, "19")
                                        strCStatus = oApplication.Utilities.getEdittextvalue(oForm, "23")
                                        Dim strChkAppStatus As String = ""
                                        Dim strWAppStatus As String = ""
                                        Dim oChek As SAPbouiCOM.CheckBox
                                        oChek = oForm.Items.Item("39").Specific

                                        If pVal.ItemUID = "44" Then
                                            oForm.Items.Item("39").Enabled = True
                                            oForm.DataSources.UserDataSources.Item("SChkStatus").Value = "Y"
                                            oForm.Update()
                                        End If

                                        If oForm.Items.Item("39").Enabled = True Then
                                            If oChek.Checked = True Then
                                                If oHashGrevence.Count > 0 Then
                                                    If oHashGrevence(1).ToString() = "Y" Then
                                                        strChkAppStatus = "Y"
                                                        strWAppStatus = strstatus
                                                    End If
                                                Else
                                                    strChkAppStatus = "Y"
                                                    strWAppStatus = "SE"
                                                End If
                                            Else
                                                strWAppStatus = strstatus
                                                strChkAppStatus = ""
                                            End If
                                        End If
                                        If pVal.ItemUID = "btnGra" Then
                                            strWAppStatus = strworkflowstatus
                                        End If
                                        If oChek.Checked Then
                                            Dim iret As Int16 = oApplication.SBO_Application.MessageBox("Sure Want to Approve Document", 2, "Yes", "No", "")
                                            If iret = 1 Then
                                                UpdateDocument(oHashBusRating, oHashPeoRating, oHashComRating, strBStatus, strPStatus, strCStatus, strWAppStatus, DocNo, 1, oHashGrevence, strChkAppStatus)
                                                UpdateDocument1(oHashBusRating1, oHashPeoRating1, oHashComRating1, strBStatus, strPStatus, strCStatus, strWAppStatus, DocNo, 1, oHashGrevence, strChkAppStatus)

                                                If Not oHashGrevence(2) = "A" Then
                                                    oApplication.Utilities.UpdateTimeStamp(DocNo, "SF")
                                                End If

                                            End If
                                        Else
                                            UpdateDocument(oHashBusRating, oHashPeoRating, oHashComRating, strBStatus, strPStatus, strCStatus, strWAppStatus, DocNo, 1, oHashGrevence, strChkAppStatus)
                                            UpdateDocument1(oHashBusRating1, oHashPeoRating1, oHashComRating1, strBStatus, strPStatus, strCStatus, strWAppStatus, DocNo, 1, oHashGrevence, strChkAppStatus)

                                            'TimeStamp for Acceptance
                                            If pVal.ItemUID = "btnGra" Then
                                                oApplication.Utilities.UpdateTimeStamp(DocNo, "SFA")
                                            End If
                                        End If
                                    ElseIf oForm.Title = "First Level Approval" Then
                                        If oGrid_P1.Rows.Count > 0 Then
                                            oHashBusRating = New Hashtable()
                                            oHashBusRating1 = New Hashtable()
                                            Dim i As Integer
                                            For i = 0 To oGrid_P1.Rows.Count - 1
                                                oHashBusRating.Add(i + 1, oGrid_P1.DataTable.GetValue("Line Manager Rating", i))
                                                oHashBusRating1.Add(i + 1, oGrid_P1.DataTable.GetValue("U_Z_MgrRaCode", i))
                                            Next
                                        End If
                                        If oGrid_P2.Rows.Count > 0 Then
                                            oHashPeoRating = New Hashtable()
                                            oHashPeoRating1 = New Hashtable()
                                            Dim i As Integer
                                            For i = 0 To oGrid_P2.Rows.Count - 1
                                                oHashPeoRating.Add(i + 1, oGrid_P2.DataTable.GetValue("Line Manager Rating", i))
                                                oHashPeoRating1.Add(i + 1, oGrid_P2.DataTable.GetValue("U_Z_MgrRaCode", i))
                                            Next
                                        End If
                                        If oGrid_P3.Rows.Count > 0 Then
                                            oHashComRating = New Hashtable()
                                            oHashComRating1 = New Hashtable()
                                            Dim i As Integer
                                            For i = 0 To oGrid_P3.Rows.Count - 1
                                                oHashComRating.Add(i + 1, oGrid_P3.DataTable.GetValue("Line Manager Rating", i))
                                                oHashComRating1.Add(i + 1, oGrid_P3.DataTable.GetValue("U_Z_MgrRaCode", i))
                                            Next
                                        End If
                                        strstatus = oComboStatus.Selected.Value.ToString()
                                        strBStatus = oApplication.Utilities.getEdittextvalue(oForm, "16")
                                        strPStatus = oApplication.Utilities.getEdittextvalue(oForm, "20")
                                        strCStatus = oApplication.Utilities.getEdittextvalue(oForm, "24")
                                        Dim oHashGrevence As Hashtable
                                        oHashGrevence = New Hashtable()
                                        Dim strChkAppStatus As String = ""
                                        Dim strWAppStatus As String = ""
                                        Dim oChek As SAPbouiCOM.CheckBox
                                        oChek = oForm.Items.Item("40").Specific

                                        If pVal.ItemUID = "44" Then
                                            oForm.Items.Item("40").Enabled = True
                                            oForm.DataSources.UserDataSources.Item("LChkStatus").Value = "Y"
                                            oForm.Update()

                                            oDtAppraisal = oForm.DataSources.DataTables.Item("dtAppraisal")
                                            oDtAppraisal.Rows.Clear()
                                            oDtAppraisal.Rows.Add(1)
                                        End If

                                        If oForm.Items.Item("40").Enabled = True Then
                                            If oChek.Checked = True Then
                                                strChkAppStatus = "Y"
                                                strWAppStatus = "LM"
                                            Else
                                                strWAppStatus = strstatus
                                                strChkAppStatus = ""
                                            End If
                                        End If
                                        Dim blnSendMail As Boolean = False
                                        If oChek.Checked Then
                                            Dim iret As Int16 = oApplication.SBO_Application.MessageBox("Sure Want to Approve Document", 2, "Yes", "No", "")
                                            If iret = 1 Then
                                                UpdateDocument(oHashBusRating, oHashPeoRating, oHashComRating, strBStatus, strPStatus, strCStatus, strWAppStatus, DocNo, 2, oHashGrevence, strChkAppStatus)
                                                UpdateDocument1(oHashBusRating1, oHashPeoRating1, oHashComRating1, strBStatus, strPStatus, strCStatus, strWAppStatus, DocNo, 2, oHashGrevence, strChkAppStatus)

                                                oApplication.Utilities.UpdateTimeStamp(DocNo, "FL")
                                                blnSendMail = True
                                            End If
                                        Else
                                            UpdateDocument(oHashBusRating, oHashPeoRating, oHashComRating, strBStatus, strPStatus, strCStatus, strWAppStatus, DocNo, 2, oHashGrevence, strChkAppStatus)
                                            UpdateDocument1(oHashBusRating1, oHashPeoRating1, oHashComRating1, strBStatus, strPStatus, strCStatus, strWAppStatus, DocNo, 2, oHashGrevence, strChkAppStatus)

                                        End If

                                        If pVal.ItemUID = "44" And blnSendMail Then
                                            oDtAppraisal.SetValue("DocEntry", 0, DocNo)

                                            For index As Integer = 0 To oDtAppraisal.Rows.Count - 1
                                                oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                                sQuery = "Select T0.Email,T1.Email,T0.FirstName +' ' + T0.lastName As Name From OHEM T0 JOIN OHEM T1  ON T0.Manager = T1.EmpID JOIN [@Z_HR_OSEAPP] T2 ON T0.EmpID = T2.U_Z_EmpId Where T2.DocEntry = '" & oDtAppraisal.GetValue("DocEntry", index) & "'"
                                                oRecordSet.DoQuery(sQuery)
                                                If Not oRecordSet.EoF Then
                                                    oDtAppraisal.SetValue("ccID", index, oRecordSet.Fields.Item(0).Value)
                                                    oDtAppraisal.SetValue("toID", index, oRecordSet.Fields.Item(1).Value)
                                                    oDtAppraisal.SetValue("Name", index, oRecordSet.Fields.Item(2).Value)
                                                    oDtAppraisal.SetValue("Type", index, "LA")
                                                End If
                                            Next

                                            If oApplication.Utilities.checkmailconfiguration() = False Then
                                                oApplication.Utilities.Message("Email configuration not availble...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            Else
                                                If Not IsNothing(oDtAppraisal) And oDtAppraisal.Rows.Count > 0 Then
                                                    oApplication.SBO_Application.StatusBar.SetText("Generating Report Started....", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                                    oApplication.Utilities.generateReport(oDtAppraisal)
                                                    oApplication.SBO_Application.StatusBar.SetText("Process Sending Mail....", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                                    oApplication.Utilities.SendMail(oDtAppraisal, "Appraisal")
                                                    oApplication.SBO_Application.StatusBar.SetText("Mail Process Completed Sucessfully....", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                                End If
                                            End If
                                        End If

                                    ElseIf oForm.Title = "Second Level Approval" Then
                                        If oGrid_P1.Rows.Count > 0 Then
                                            oHashBusRating = New Hashtable()
                                            oHashBusRating1 = New Hashtable()
                                            Dim i As Integer
                                            For i = 0 To oGrid_P1.Rows.Count - 1
                                                oHashBusRating.Add(i + 1, oGrid_P1.DataTable.GetValue("Sr.Manager Rating", i))
                                                oHashBusRating1.Add(i + 1, oGrid_P1.DataTable.GetValue("U_Z_SMRaCode", i))
                                            Next
                                        End If
                                        If oGrid_P2.Rows.Count > 0 Then
                                            oHashPeoRating = New Hashtable()
                                            oHashPeoRating1 = New Hashtable()
                                            Dim i As Integer
                                            For i = 0 To oGrid_P2.Rows.Count - 1
                                                oHashPeoRating.Add(i + 1, oGrid_P2.DataTable.GetValue("Sr.Manager Rating", i))
                                                oHashPeoRating1.Add(i + 1, oGrid_P2.DataTable.GetValue("U_Z_SMRaCode", i))
                                            Next
                                        End If
                                        If oGrid_P3.Rows.Count > 0 Then
                                            oHashComRating = New Hashtable()
                                            oHashComRating1 = New Hashtable()
                                            Dim i As Integer
                                            For i = 0 To oGrid_P3.Rows.Count - 1
                                                oHashComRating.Add(i + 1, oGrid_P3.DataTable.GetValue("Sr.Manager Rating", i))
                                                oHashComRating1.Add(i + 1, oGrid_P3.DataTable.GetValue("U_Z_SMRaCode", i))
                                            Next
                                        End If
                                        strstatus = oComboStatus.Selected.Value.ToString()
                                        strBStatus = oApplication.Utilities.getEdittextvalue(oForm, "17")
                                        strPStatus = oApplication.Utilities.getEdittextvalue(oForm, "21")
                                        strCStatus = oApplication.Utilities.getEdittextvalue(oForm, "25")
                                        Dim oHashGrevence As Hashtable
                                        oHashGrevence = New Hashtable()
                                        Dim strChkAppStatus As String = ""
                                        Dim strWAppStatus As String = ""
                                        Dim oChek As SAPbouiCOM.CheckBox
                                        oChek = oForm.Items.Item("41").Specific

                                        If pVal.ItemUID = "44" Then
                                            oForm.Items.Item("41").Enabled = True
                                            oForm.DataSources.UserDataSources.Item("SChkSts").Value = "Y"
                                            oForm.Update()
                                        End If

                                        If oForm.Items.Item("41").Enabled = True Then
                                            If oChek.Checked = True Then
                                                strChkAppStatus = "Y"
                                                strWAppStatus = "SM"
                                            Else
                                                strWAppStatus = strstatus
                                                strChkAppStatus = ""
                                            End If
                                        End If
                                        If oChek.Checked Then
                                            Dim iret As Int16 = oApplication.SBO_Application.MessageBox("Sure Want to Approve Document", 2, "Yes", "No", "")
                                            If iret = 1 Then
                                                UpdateDocument(oHashBusRating, oHashPeoRating, oHashComRating, strBStatus, strPStatus, strCStatus, strWAppStatus, DocNo, 3, oHashGrevence, strChkAppStatus)
                                                UpdateDocument1(oHashBusRating1, oHashPeoRating1, oHashComRating1, strBStatus, strPStatus, strCStatus, strWAppStatus, DocNo, 3, oHashGrevence, strChkAppStatus)

                                                oApplication.Utilities.UpdateTimeStamp(DocNo, "SL")
                                            End If
                                        Else
                                            UpdateDocument(oHashBusRating, oHashPeoRating, oHashComRating, strBStatus, strPStatus, strCStatus, strWAppStatus, DocNo, 3, oHashGrevence, strChkAppStatus)
                                            UpdateDocument1(oHashBusRating1, oHashPeoRating1, oHashComRating1, strBStatus, strPStatus, strCStatus, strWAppStatus, DocNo, 3, oHashGrevence, strChkAppStatus)

                                        End If
                                    ElseIf oForm.Title = "HR Acceptance" Then
                                        strstatus = oComboStatus.Selected.Value.ToString()
                                        strBStatus = oApplication.Utilities.getEdittextvalue(oForm, "18")
                                        strPStatus = oApplication.Utilities.getEdittextvalue(oForm, "22")
                                        strCStatus = oApplication.Utilities.getEdittextvalue(oForm, "26")
                                        Dim strChkAppStatus As String = ""
                                        Dim strWAppStatus As String = ""
                                        Dim oChek As SAPbouiCOM.CheckBox
                                        oChek = oForm.Items.Item("42").Specific

                                        If pVal.ItemUID = "44" Then
                                            oForm.Items.Item("42").Enabled = True
                                            oForm.DataSources.UserDataSources.Item("HChkStatus").Value = "Y"
                                            oForm.Update()

                                            oDtAppraisal = oForm.DataSources.DataTables.Item("dtAppraisal")
                                            oDtAppraisal.Rows.Clear()
                                            oDtAppraisal.Rows.Add(1)
                                        End If

                                        If oForm.Items.Item("42").Enabled = True Then
                                            If oChek.Checked = True Then
                                                strChkAppStatus = "Y"
                                                strWAppStatus = "HR"
                                            Else
                                                strWAppStatus = strstatus
                                                strChkAppStatus = ""
                                            End If
                                        End If
                                        Dim blnSendMail As Boolean = False
                                        If oChek.Checked Then
                                            Dim iret As Int16 = oApplication.SBO_Application.MessageBox("Sure Want to Approve Document", 2, "Yes", "No", "")
                                            If iret = 1 Then
                                                UpdateHRRating()
                                                UpdateHRStatus(DocNo, strWAppStatus, strBStatus, strPStatus, strCStatus, strChkAppStatus)
                                                oApplication.Utilities.UpdateTimeStamp(DocNo, "HR")
                                                blnSendMail = True
                                            End If
                                        Else
                                            UpdateHRRating()
                                            UpdateHRStatus(DocNo, strWAppStatus, strBStatus, strPStatus, strCStatus, strChkAppStatus)
                                        End If

                                        If pVal.ItemUID = "44" And blnSendMail Then
                                            oDtAppraisal.SetValue("DocEntry", 0, DocNo)
                                            For index As Integer = 0 To oDtAppraisal.Rows.Count - 1
                                                oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                                sQuery = "Select T0.Email,T1.Email,T0.FirstName +' ' + T0.lastName As Name From OHEM T0 JOIN OHEM T1  ON T0.Manager = T1.EmpID JOIN [@Z_HR_OSEAPP] T2 ON T0.EmpID = T2.U_Z_EmpId Where T2.DocEntry = '" & oDtAppraisal.GetValue("DocEntry", index) & "'"
                                                oRecordSet.DoQuery(sQuery)
                                                If Not oRecordSet.EoF Then
                                                    oDtAppraisal.SetValue("ccID", index, oRecordSet.Fields.Item(0).Value)
                                                    oDtAppraisal.SetValue("toID", index, oRecordSet.Fields.Item(1).Value)
                                                    oDtAppraisal.SetValue("Name", index, oRecordSet.Fields.Item(2).Value)
                                                    oDtAppraisal.SetValue("Type", index, "HA")
                                                End If
                                            Next

                                            If oApplication.Utilities.checkmailconfiguration() = False Then
                                                oApplication.Utilities.Message("Email configuration not availble...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            Else
                                                If Not IsNothing(oDtAppraisal) And oDtAppraisal.Rows.Count > 0 Then
                                                    oApplication.SBO_Application.StatusBar.SetText("Generating Report Started....", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                                    oApplication.Utilities.generateReport(oDtAppraisal)
                                                    oApplication.SBO_Application.StatusBar.SetText("Process Sending Mail....", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                                    oApplication.Utilities.SendMail(oDtAppraisal, "Appraisal")
                                                    oApplication.SBO_Application.StatusBar.SetText("Mail Process Completed Sucessfully....", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                                End If
                                            End If
                                        End If
                                    End If
                                    ReDatabind(oForm)
                                ElseIf pVal.ItemUID = "3" And pVal.ColUID = "RowsHeader" And pVal.Row <> -1 Then
                                    oForm.Freeze(True)
                                    Dim isLevelStartFromLMLine As Boolean = False
                                    oGrid = oForm.Items.Item("3").Specific
                                    oGrid_P1 = oForm.Items.Item("8").Specific
                                    oGrid_P2 = oForm.Items.Item("9").Specific
                                    oGrid_P3 = oForm.Items.Item("10").Specific
                                    oForm.Items.Item("30").Visible = False
                                    oForm.Items.Item("31").Visible = False
                                    oForm.Items.Item("32").Visible = False
                                    If oGrid.Rows.Count > 0 Then
                                        Dim DocNo As Integer = Convert.ToInt32(oGrid.DataTable.GetValue("DocEntry", pVal.Row))
                                        Dim StrQP0, StrQP1, StrQP2, StrQP3 As String
                                        StrQP0 = ""
                                        StrQP1 = ""
                                        StrQP2 = ""
                                        StrQP3 = ""
                                        StrQP0 = "Select U_Z_GStatus,U_Z_WStatus,U_Z_BSelfRemark,U_Z_BMgrRemark,U_Z_BSMrRemark,U_Z_BHrRemark, U_Z_PSelfRemark,U_Z_PMgrRemark,U_Z_PSMrRemark,U_Z_PHrRemark, U_Z_CSelfRemark,U_Z_CMgrRemark,U_Z_CSMrRemark,U_Z_CHrRemark,U_Z_LStrt from [@Z_HR_OSEAPP] Where DocEntry=" & DocNo & ""
                                        'StrQP1 = "Select U_Z_BussCode as 'Code',U_Z_BussDesc as 'Business Objectives',U_Z_BussWeight as 'Weight (%)' ,U_Z_BussSelfRate as 'Self Rating',U_Z_BussMgrRate as 'Line Manager Rating',U_Z_BussSMRate as 'Sr.Manager Rating' from [@Z_HR_SEAPP1] Where DocEntry=" & DocNo & ""
                                        'StrQP2 = "Select T0.U_Z_PeopleCode as 'Code',T0.U_Z_PeopleDesc as 'People Objectives',T2.U_Z_Remarks 'Emp Remarks',T0.U_Z_PeopleCat as 'Category',T0.U_Z_PeoWeight as 'Weight (%)',T0.U_Z_PeoSelfRate as 'Self Rating',T0.U_Z_PeoMgrRate as 'Line Manager Rating',T0.U_Z_PeoSMRate as 'Sr.Manager Rating' from [@Z_HR_SEAPP2] T0 Join [@Z_HR_OSEAPP] T1 ON T1.DocEntry = T0.DocEntry Left Outer Join [@Z_HR_PEOBJ1] T2 On T1.U_Z_EmpId = T2.U_Z_HREmpID and T2.U_Z_HRPeoobjCode=T0.U_Z_PeopleCode Where T0.DocEntry = " & DocNo & ""
                                        'StrQP3 = "Select T0.U_Z_CompCode as 'Code',T0.U_Z_CompDesc as 'Competence Objectives',T0.U_Z_CompWeight as 'Weight (%)',T0.U_Z_CompLevel as 'Levels',T2.U_Z_CompLevel As 'Current Level',T0.U_Z_CompSelf as 'Self Rating',T0.U_Z_CompMgr as 'Line Manager Rating',T0.U_Z_CompSM as 'Sr.Manager Rating' from [@Z_HR_SEAPP3] T0 Join [@Z_HR_OSEAPP] T1 ON T1.DocEntry = T0.DocEntry Left Outer Join [@Z_HR_ECOLVL] T2 On T1.U_Z_EmpId = T2.U_Z_HREmpID Where T0.DocEntry=" & DocNo & ""

                                        StrQP1 = "Select U_Z_BussCode as 'Code',U_Z_BussDesc as 'Business Objectives',U_Z_BussWeight as 'Weight (%)' ,T0.""U_Z_SelfRaCode"",U_Z_BussSelfRate as 'Self Rating',T0.""U_Z_MgrRaCode"",U_Z_BussMgrRate as 'Line Manager Rating',T0.""U_Z_SMRaCode"",U_Z_BussSMRate as 'Sr.Manager Rating' from [@Z_HR_SEAPP1] T0 Where DocEntry=" & DocNo & ""
                                        StrQP2 = "Select T0.U_Z_PeopleCode as 'Code',T0.U_Z_PeopleDesc as 'People Objectives',T2.U_Z_Remarks 'Emp Remarks',T0.U_Z_PeopleCat as 'Category',T0.U_Z_PeoWeight as 'Weight (%)',""U_Z_SelfRaCode"",T0.U_Z_PeoSelfRate as 'Self Rating',T0.""U_Z_MgrRaCode"",T0.U_Z_PeoMgrRate as 'Line Manager Rating',T0.""U_Z_SMRaCode"",T0.U_Z_PeoSMRate as 'Sr.Manager Rating' from [@Z_HR_SEAPP2] T0 Join [@Z_HR_OSEAPP] T1 ON T1.DocEntry = T0.DocEntry Left Outer Join [@Z_HR_PEOBJ1] T2 On T1.U_Z_EmpId = T2.U_Z_HREmpID  and T2.U_Z_HRPeoobjCode=T0.U_Z_PeopleCode Where T0.DocEntry = " & DocNo & ""
                                        StrQP3 = "Select T0.U_Z_CompCode as 'Code',T0.U_Z_CompDesc as 'Competence Objectives',T0.U_Z_CompWeight as 'Weight (%)',T0.U_Z_CompLevel as 'Levels',T2.U_Z_CompLevel As 'Current Level',""U_Z_SelfRaCode"",T0.U_Z_CompSelfRate as 'Self Rating',T0.""U_Z_MgrRaCode"",T0.U_Z_CompMgrRate as 'Line Manager Rating',T0.""U_Z_SMRaCode"",T0.U_Z_CompSMRate as 'Sr.Manager Rating' from [@Z_HR_SEAPP3] T0 Join [@Z_HR_OSEAPP] T1 ON T1.DocEntry = T0.DocEntry Left Outer Join [@Z_HR_ECOLVL] T2 On T1.U_Z_EmpId = T2.U_Z_HREmpID and T2.U_Z_CompCode =T0.U_Z_CompCode and T2.U_Z_CompCode =T0.U_Z_CompCode  Where T0.DocEntry = " & DocNo & ""

                                        oGrid_P1.DataTable.ExecuteQuery(StrQP1)
                                        oGrid_P2.DataTable.ExecuteQuery(StrQP2)
                                        oGrid_P3.DataTable.ExecuteQuery(StrQP3)
                                        Dim oRS As SAPbobsCOM.Recordset
                                        oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                        oRS.DoQuery(StrQP0)
                                        Dim strBSelfRMark, strBMgrRMark, strBSMrRMark, strBHrRMark, strPSelfRMark, strPMgrRMark, strPSMrRMark, strPHrRMark, strCSelfRMark, strCMgrRMark, strCSMrRMark, strCHrRMark, strWStatus, strGStatus, strLStart As String
                                        strBSelfRMark = oRS.Fields.Item("U_Z_BSelfRemark").Value.ToString()
                                        strBMgrRMark = oRS.Fields.Item("U_Z_BMgrRemark").Value.ToString()
                                        strBSMrRMark = oRS.Fields.Item("U_Z_BSMrRemark").Value.ToString()
                                        strBHrRMark = oRS.Fields.Item("U_Z_BHrRemark").Value.ToString()
                                        strPSelfRMark = oRS.Fields.Item("U_Z_PSelfRemark").Value.ToString()
                                        strPMgrRMark = oRS.Fields.Item("U_Z_PMgrRemark").Value.ToString()
                                        strPSMrRMark = oRS.Fields.Item("U_Z_PSMrRemark").Value.ToString()
                                        strPHrRMark = oRS.Fields.Item("U_Z_PHrRemark").Value.ToString()
                                        strCSelfRMark = oRS.Fields.Item("U_Z_CSelfRemark").Value.ToString()
                                        strCMgrRMark = oRS.Fields.Item("U_Z_CMgrRemark").Value.ToString()
                                        strCSMrRMark = oRS.Fields.Item("U_Z_CSMrRemark").Value.ToString()
                                        strCHrRMark = oRS.Fields.Item("U_Z_CHrRemark").Value.ToString()
                                        strWStatus = oRS.Fields.Item("U_Z_WStatus").Value.ToString()
                                        strGStatus = oRS.Fields.Item("U_Z_GStatus").Value.ToString()
                                        strLStart = oRS.Fields.Item("U_Z_LStrt").Value.ToString()
                                        If strLStart = "LM" Then
                                            isLevelStartFromLMLine = True
                                        End If
                                        oApplication.Utilities.setEdittextvalue(oForm, "15", strBSelfRMark)
                                        oApplication.Utilities.setEdittextvalue(oForm, "16", strBMgrRMark)
                                        oApplication.Utilities.setEdittextvalue(oForm, "17", strBSMrRMark)
                                        oApplication.Utilities.setEdittextvalue(oForm, "18", strBHrRMark)
                                        oApplication.Utilities.setEdittextvalue(oForm, "19", strPSelfRMark)
                                        oApplication.Utilities.setEdittextvalue(oForm, "20", strPMgrRMark)
                                        oApplication.Utilities.setEdittextvalue(oForm, "21", strPSMrRMark)
                                        oApplication.Utilities.setEdittextvalue(oForm, "22", strPHrRMark)
                                        oApplication.Utilities.setEdittextvalue(oForm, "23", strCSelfRMark)
                                        oApplication.Utilities.setEdittextvalue(oForm, "24", strCMgrRMark)
                                        oApplication.Utilities.setEdittextvalue(oForm, "25", strCSMrRMark)
                                        oApplication.Utilities.setEdittextvalue(oForm, "26", strCHrRMark)
                                        oForm.ActiveItem = 28
                                        Dim oComboStatus As SAPbouiCOM.ComboBox
                                        oComboStatus = oForm.Items.Item("28").Specific
                                        Try
                                            oComboStatus.Select(strWStatus, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                        Catch ex As Exception

                                        End Try

                                        oForm.Items.Item("28").Enabled = False

                                        Dim oComboGStatus As SAPbouiCOM.ComboBox
                                        oComboGStatus = oForm.Items.Item("32").Specific
                                        Try
                                            oComboGStatus.Select(strGStatus, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                        Catch ex As Exception
                                        End Try

                                        Dim oRec As SAPbobsCOM.Recordset
                                        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                        Dim strCStatus As String = ""
                                        Dim strChkS, strChkL, strChkSr, strChkH, strGHRAcct, strDStatus As String
                                        strCStatus = "select U_Z_SCkApp,U_Z_LCkApp,U_Z_SrCkApp,U_Z_HrCkApp,U_Z_GHRSts,U_Z_Status from [@Z_HR_OSEAPP] where DocEntry = " & DocNo & ""
                                        oRec.DoQuery(strCStatus)
                                        If Not oRec.EoF Then
                                            strChkS = oRec.Fields.Item("U_Z_SCkApp").Value.ToString()
                                            strChkL = oRec.Fields.Item("U_Z_LCkApp").Value.ToString()
                                            strChkSr = oRec.Fields.Item("U_Z_SrCkApp").Value.ToString()
                                            strChkH = oRec.Fields.Item("U_Z_HrCkApp").Value.ToString()
                                            strGHRAcct = oRec.Fields.Item("U_Z_GHRSts").Value.ToString()
                                            strDStatus = oRec.Fields.Item("U_Z_Status").Value.ToString()
                                        End If

                                        oGrid_P2.Columns.Item("Emp Remarks").TitleObject.Caption = "Remarks"
                                        oGrid_P2.Columns.Item("Emp Remarks").Editable = False

                                        'oGrid_P1.Columns.Item("Self Rating").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                                        'oGrid_P1.Columns.Item("Line Manager Rating").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                                        'oGrid_P1.Columns.Item("Sr.Manager Rating").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox

                                        oGrid_P1.Columns.Item("U_Z_SelfRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                                        oGrid_P1.Columns.Item("U_Z_MgrRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                                        oGrid_P1.Columns.Item("U_Z_SMRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox


                                        Dim oComboCol As SAPbouiCOM.ComboBoxColumn
                                        If 1 = 1 Then


                                            sQuery = "Select U_Z_RateCode As Code,U_Z_RateName As Name From [@Z_HR_ORATE]"
                                            oRec.DoQuery(sQuery)
                                            oComboCol = oGrid_P1.Columns.Item("U_Z_SelfRaCode")
                                            oComboCol.ValidValues.Add("", "")
                                            oRec.MoveFirst()
                                            For index As Integer = 0 To oRec.RecordCount - 1
                                                oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                                                oRec.MoveNext()
                                            Next
                                            oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                                            oComboCol = oGrid_P1.Columns.Item("U_Z_MgrRaCode")
                                            oComboCol.ValidValues.Add("", "")
                                            oRec.MoveFirst()
                                            For index As Integer = 0 To oRec.RecordCount - 1
                                                oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                                                oRec.MoveNext()
                                            Next
                                            oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                                            oComboCol = oGrid_P1.Columns.Item("U_Z_SMRaCode")
                                            oComboCol.ValidValues.Add("", "")
                                            oRec.MoveFirst()
                                            For index As Integer = 0 To oRec.RecordCount - 1
                                                oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                                                oRec.MoveNext()
                                            Next
                                            oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                                        End If


                                        'oGrid_P2.Columns.Item("Self Rating").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                                        'oGrid_P2.Columns.Item("Line Manager Rating").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                                        'oGrid_P2.Columns.Item("Sr.Manager Rating").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox


                                        oGrid_P2.Columns.Item("U_Z_SelfRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                                        oGrid_P2.Columns.Item("U_Z_MgrRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                                        oGrid_P2.Columns.Item("U_Z_SMRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox


                                        If 1 = 1 Then



                                            sQuery = "Select U_Z_RateCode As Code,U_Z_RateName As Name From [@Z_HR_ORATE]"
                                            oRec.DoQuery(sQuery)
                                            oComboCol = oGrid_P2.Columns.Item("U_Z_SelfRaCode")
                                            oComboCol.ValidValues.Add("", "")
                                            oRec.MoveFirst()
                                            For index As Integer = 0 To oRec.RecordCount - 1
                                                oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                                                oRec.MoveNext()
                                            Next
                                            oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                                            oComboCol = oGrid_P2.Columns.Item("U_Z_MgrRaCode")
                                            oComboCol.ValidValues.Add("", "")
                                            oRec.MoveFirst()
                                            For index As Integer = 0 To oRec.RecordCount - 1
                                                oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                                                oRec.MoveNext()
                                            Next
                                            oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                                            oComboCol = oGrid_P2.Columns.Item("U_Z_SMRaCode")
                                            oComboCol.ValidValues.Add("", "")
                                            oRec.MoveFirst()
                                            For index As Integer = 0 To oRec.RecordCount - 1
                                                oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                                                oRec.MoveNext()
                                            Next
                                            oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                                        End If





                                        oGrid_P3.Columns.Item("Levels").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                                        oGrid_P3.Columns.Item("Current Level").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                                        'oGrid_P3.Columns.Item("Self Rating").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                                        'oGrid_P3.Columns.Item("Line Manager Rating").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                                        'oGrid_P3.Columns.Item("Sr.Manager Rating").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox

                                        oGrid_P3.Columns.Item("U_Z_SelfRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                                        oGrid_P3.Columns.Item("U_Z_MgrRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                                        oGrid_P3.Columns.Item("U_Z_SMRaCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox


                                        oGrid_P1.Columns.Item("U_Z_SelfRaCode").TitleObject.Caption = "Self Rating"
                                        oGrid_P1.Columns.Item("U_Z_MgrRaCode").TitleObject.Caption = "First Level Manager Rating"
                                        oGrid_P1.Columns.Item("U_Z_SMRaCode").TitleObject.Caption = "Second Level Manager Rating"

                                        oGrid_P2.Columns.Item("U_Z_SelfRaCode").TitleObject.Caption = "Self Rating"
                                        oGrid_P2.Columns.Item("U_Z_MgrRaCode").TitleObject.Caption = "First Level Manager Rating"
                                        oGrid_P2.Columns.Item("U_Z_SMRaCode").TitleObject.Caption = "Second Level Manager Rating"


                                        oGrid_P3.Columns.Item("U_Z_SelfRaCode").TitleObject.Caption = "Self Rating"
                                        oGrid_P3.Columns.Item("U_Z_MgrRaCode").TitleObject.Caption = "First Level Manager Rating"
                                        oGrid_P3.Columns.Item("U_Z_SMRaCode").TitleObject.Caption = "Second Level Manager Rating"


                                        sQuery = "Select U_Z_LvelCode As Code,U_Z_LvelName As Name From [@Z_HR_COLVL]"
                                        oRec.DoQuery(sQuery)
                                        If Not oRec.EoF Then

                                            'oComboCol = oGrid_P3.Columns.Item("Levels")
                                            'oComboCol.ValidValues.Add("", "")
                                            'For index As Integer = 0 To oRec.RecordCount - 1
                                            '    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                                            '    oRec.MoveNext()
                                            'Next
                                            'oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                                            'oComboCol = oGrid_P3.Columns.Item("Current Level")
                                            'oComboCol.ValidValues.Add("", "")
                                            'oRec.MoveFirst()
                                            'For index As Integer = 0 To oRec.RecordCount - 1
                                            '    oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                                            '    oRec.MoveNext()
                                            'Next
                                            'oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description


                                            sQuery = "Select U_Z_RateCode As Code,U_Z_RateName As Name From [@Z_HR_ORATE]"
                                            oRec.DoQuery(sQuery)
                                            oComboCol = oGrid_P3.Columns.Item("U_Z_SelfRaCode")
                                            oComboCol.ValidValues.Add("", "")
                                            oRec.MoveFirst()
                                            For index As Integer = 0 To oRec.RecordCount - 1
                                                oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                                                oRec.MoveNext()
                                            Next
                                            oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                                            oComboCol = oGrid_P3.Columns.Item("U_Z_MgrRaCode")
                                            oComboCol.ValidValues.Add("", "")
                                            oRec.MoveFirst()
                                            For index As Integer = 0 To oRec.RecordCount - 1
                                                oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                                                oRec.MoveNext()
                                            Next
                                            oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                                            oComboCol = oGrid_P3.Columns.Item("U_Z_SMRaCode")
                                            oComboCol.ValidValues.Add("", "")
                                            oRec.MoveFirst()
                                            For index As Integer = 0 To oRec.RecordCount - 1
                                                oComboCol.ValidValues.Add(oRec.Fields.Item("Code").Value, oRec.Fields.Item("Name").Value)
                                                oRec.MoveNext()
                                            Next
                                            oComboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                                        End If
                                        oGrid_P3.Columns.Item("Current Level").Editable = False
                                        Disable(strDStatus)

                                        colSum()

                                        If oForm.Title = "HR Acceptance" Then
                                            oForm.Items.Item("16").Enabled = False
                                            oForm.Items.Item("20").Enabled = False
                                            oForm.Items.Item("24").Enabled = False
                                            oForm.Items.Item("17").Enabled = False
                                            oForm.Items.Item("21").Enabled = False
                                            oForm.Items.Item("25").Enabled = False
                                            oForm.Items.Item("15").Enabled = False
                                            oForm.Items.Item("19").Enabled = False
                                            oForm.Items.Item("23").Enabled = False
                                            oForm.Items.Item("39").Enabled = False
                                            oForm.Items.Item("40").Enabled = False
                                            oForm.Items.Item("41").Enabled = False
                                            Dim oChk As SAPbouiCOM.CheckBox
                                            oChk = oForm.Items.Item("42").Specific
                                            Dim blnHRApproved As Boolean = False
                                            If strChkH = "Y" Then
                                                oChk.Checked = True
                                                oForm.Items.Item("42").Enabled = False
                                                '  oForm.Items.Item("29").Enabled = False
                                                blnHRApproved = True
                                            Else
                                                oChk.Checked = False
                                                oForm.Items.Item("42").Enabled = True
                                                'oForm.Items.Item("29").Enabled = True
                                            End If
                                            oCombobox = oForm.Items.Item("28").Specific
                                            If oCombobox.Selected.Value <> "SE" And blnHRApproved = False Then
                                                If oCombobox.Selected.Value = "DR" Then
                                                    oForm.Items.Item("29").Enabled = False
                                                    oForm.Items.Item("44").Enabled = False
                                                Else

                                                    oForm.Items.Item("29").Enabled = True
                                                    oForm.Items.Item("44").Enabled = True
                                                End If

                                            Else
                                                oForm.Items.Item("29").Enabled = False
                                            End If


                                        ElseIf oForm.Title = "Self Appraisals" Then
                                            oForm.Items.Item("btnGra").Visible = False
                                            If strGStatus = "A" Then
                                                oForm.Items.Item("31").Visible = True
                                                oForm.Items.Item("32").Visible = True
                                                oForm.Items.Item("32").Enabled = False
                                                oForm.Items.Item("29").Enabled = False
                                                oForm.Items.Item("44").Enabled = False
                                                oForm.Items.Item("btnGra").Visible = False
                                            ElseIf strGStatus = "-" Then
                                                oForm.Items.Item("32").Enabled = True
                                                If isLevelStartFromLMLine Then
                                                    oForm.Items.Item("29").Enabled = False
                                                    oForm.Items.Item("44").Enabled = False
                                                Else
                                                    oForm.Items.Item("29").Enabled = True
                                                    oForm.Items.Item("44").Enabled = True
                                                End If
                                                oForm.Items.Item("btnGra").Visible = True
                                            End If
                                            If (oComboStatus.Selected.Value = "LM" Or oComboStatus.Selected.Value = "SM" Or oComboStatus.Selected.Value = "HR") And strGHRAcct <> "R" And strGStatus <> "A" Then
                                                Try
                                                    'FillAcceptanceCombo()
                                                    oForm.Items.Item("30").Visible = True
                                                    oForm.Items.Item("31").Visible = True
                                                    oForm.Items.Item("32").Visible = True
                                                    oForm.Items.Item("45").Visible = False

                                                    oApplication.Utilities.setEdittextvalue(oForm, "36", System.DateTime.Today.ToString("yyyyMMdd"))
                                                    oForm.Items.Item("36").Enabled = False
                                                    Dim oRSGNo As SAPbobsCOM.Recordset
                                                    Dim strGNo As String
                                                    oRSGNo = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                                    Dim strQueryGNo As String = "Select max(isnull(U_Z_GNo,'0'))+1 as 'GNo' from [@Z_HR_OSEAPP]"
                                                    oRSGNo.DoQuery(strQueryGNo)
                                                    strGNo = oRSGNo.Fields.Item("GNo").Value.ToString()
                                                    oApplication.Utilities.setEdittextvalue(oForm, "37", strGNo)
                                                    oForm.Items.Item("37").Enabled = False
                                                    If isLevelStartFromLMLine Then
                                                        oForm.Items.Item("29").Enabled = False
                                                        oForm.Items.Item("44").Enabled = False
                                                    Else
                                                        oForm.Items.Item("29").Enabled = True
                                                        oForm.Items.Item("44").Enabled = True
                                                    End If

                                                Catch ex As Exception

                                                End Try
                                            Else
                                                If (oComboStatus.Selected.Value = "LM" Or oComboStatus.Selected.Value = "SM" Or oComboStatus.Selected.Value = "HR") And strGHRAcct = "R" Then
                                                    Try
                                                        'FillAcceptanceCombo()
                                                        oForm.Items.Item("30").Visible = True
                                                        oForm.Items.Item("31").Visible = True
                                                        oForm.Items.Item("32").Visible = True
                                                        oForm.Items.Item("45").Visible = False

                                                        Dim oRSGNo As SAPbobsCOM.Recordset
                                                        Dim strGNo, strGDate, strGRemarks As String
                                                        oRSGNo = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                                        Dim strQueryGNo As String = "Select U_Z_GRemarks,U_Z_GNo, Convert(VarChar(8),U_Z_GDate,112) As U_Z_GDate  from [@Z_HR_OSEAPP] Where DocEntry = " & DocNo & ""
                                                        oRSGNo.DoQuery(strQueryGNo)
                                                        If Not oRSGNo.EoF Then
                                                            strGNo = oRSGNo.Fields.Item("U_Z_GNo").Value.ToString()
                                                            strGDate = oRSGNo.Fields.Item("U_Z_GDate").Value.ToString()
                                                            strGRemarks = oRSGNo.Fields.Item("U_Z_GRemarks").Value.ToString()
                                                        End If

                                                        oApplication.Utilities.setEdittextvalue(oForm, "36", strGDate)
                                                        oApplication.Utilities.setEdittextvalue(oForm, "37", strGNo)
                                                        oApplication.Utilities.setEdittextvalue(oForm, "38", strGRemarks)

                                                        oForm.Items.Item("36").Enabled = False
                                                        oForm.Items.Item("37").Enabled = False
                                                        oForm.Items.Item("29").Enabled = False
                                                        oForm.Items.Item("44").Enabled = False
                                                        oForm.Items.Item("btnGra").Visible = False
                                                    Catch ex As Exception

                                                    End Try
                                                End If
                                            End If
                                            If isLevelStartFromLMLine Then
                                                oGrid_P1.Columns.Item("Self Rating").Editable = False
                                                oGrid_P2.Columns.Item("Self Rating").Editable = False
                                                oGrid_P3.Columns.Item("Self Rating").Editable = False

                                                oGrid_P1.Columns.Item("U_Z_SelfRaCode").Editable = False
                                                oGrid_P2.Columns.Item("U_Z_SelfRaCode").Editable = False
                                                oGrid_P3.Columns.Item("U_Z_SelfRaCode").Editable = False
                                                oGrid_P1.Columns.Item("U_Z_SMRaCode").Editable = False
                                                oGrid_P2.Columns.Item("U_Z_SMRaCode").Editable = False
                                                oGrid_P3.Columns.Item("U_Z_SMRaCode").Editable = False

                                                oGrid_P1.Columns.Item("U_Z_MgrRaCode").Editable = False
                                                oGrid_P2.Columns.Item("U_Z_MgrRaCode").Editable = False
                                                oGrid_P3.Columns.Item("U_Z_MgrRaCode").Editable = False
                                            Else
                                                oGrid_P1.Columns.Item("Self Rating").Editable = False
                                                oGrid_P2.Columns.Item("Self Rating").Editable = False
                                                oGrid_P3.Columns.Item("Self Rating").Editable = False

                                                oGrid_P1.Columns.Item("U_Z_SelfRaCode").Editable = False
                                                oGrid_P2.Columns.Item("U_Z_SelfRaCode").Editable = False
                                                oGrid_P3.Columns.Item("U_Z_SelfRaCode").Editable = False

                                                oGrid_P1.Columns.Item("U_Z_SMRaCode").Editable = False
                                                oGrid_P2.Columns.Item("U_Z_SMRaCode").Editable = False
                                                oGrid_P3.Columns.Item("U_Z_SMRaCode").Editable = False

                                                oGrid_P1.Columns.Item("U_Z_MgrRaCode").Editable = False
                                                oGrid_P2.Columns.Item("U_Z_MgrRaCode").Editable = False
                                                oGrid_P3.Columns.Item("U_Z_MgrRaCode").Editable = False
                                            End If

                                            oGrid_P1.Columns.Item("Code").Editable = False
                                            oGrid_P2.Columns.Item("Code").Editable = False
                                            oGrid_P3.Columns.Item("Code").Editable = False
                                            oGrid_P1.Columns.Item("Business Objectives").Editable = False
                                            oGrid_P2.Columns.Item("People Objectives").Editable = False
                                            oGrid_P3.Columns.Item("Competence Objectives").Editable = False
                                            oGrid_P1.Columns.Item("Weight (%)").Editable = False
                                            oGrid_P2.Columns.Item("Weight (%)").Editable = False
                                            oGrid_P3.Columns.Item("Weight (%)").Editable = False
                                            oGrid_P1.Columns.Item("Line Manager Rating").Editable = False
                                            oGrid_P2.Columns.Item("Line Manager Rating").Editable = False
                                            oGrid_P3.Columns.Item("Line Manager Rating").Editable = False
                                            oGrid_P1.Columns.Item("Sr.Manager Rating").Editable = False
                                            oGrid_P2.Columns.Item("Sr.Manager Rating").Editable = False
                                            oGrid_P3.Columns.Item("Sr.Manager Rating").Editable = False
                                            oGrid_P2.Columns.Item("Category").Editable = False
                                            oGrid_P3.Columns.Item("Levels").Editable = False

                                            Dim oChk As SAPbouiCOM.CheckBox
                                            oChk = oForm.Items.Item("39").Specific
                                            If strChkS = "Y" Then
                                                oChk.Checked = True
                                                oForm.Items.Item("39").Enabled = False
                                            Else
                                                oChk.Checked = False
                                                oForm.Items.Item("39").Enabled = True
                                            End If


                                            If strGStatus <> "-" Then
                                                oChk.Checked = True
                                                oForm.Items.Item("39").Enabled = False
                                                oForm.Items.Item("29").Enabled = False
                                                oForm.Items.Item("44").Enabled = False
                                                oForm.Items.Item("btnGra").Visible = False
                                            ElseIf strGStatus = "-" Then
                                                oForm.Items.Item("39").Enabled = True
                                                If isLevelStartFromLMLine Then
                                                    oForm.Items.Item("29").Enabled = False
                                                    oForm.Items.Item("44").Enabled = False
                                                Else
                                                    oForm.Items.Item("29").Enabled = True
                                                    oForm.Items.Item("44").Enabled = True
                                                End If
                                                oForm.Items.Item("btnGra").Visible = True
                                            End If

                                            oForm.Items.Item("16").Enabled = False
                                            oForm.Items.Item("20").Enabled = False
                                            oForm.Items.Item("24").Enabled = False
                                            oForm.Items.Item("17").Enabled = False
                                            oForm.Items.Item("21").Enabled = False
                                            oForm.Items.Item("25").Enabled = False
                                            oForm.Items.Item("18").Enabled = False
                                            oForm.Items.Item("22").Enabled = False
                                            oForm.Items.Item("26").Enabled = False
                                            oForm.Items.Item("40").Enabled = False
                                            oForm.Items.Item("41").Enabled = False
                                            oForm.Items.Item("42").Enabled = False

                                            oCombobox = oForm.Items.Item("28").Specific
                                            If oCombobox.Selected.Value <> "SE" And oCombobox.Selected.Value <> "DR" Then
                                                oGrid_P1.Columns.Item("Self Rating").Editable = False
                                                oGrid_P2.Columns.Item("Self Rating").Editable = False
                                                oGrid_P3.Columns.Item("Self Rating").Editable = False
                                                If strChkH = "Y" And strGStatus = "-" Then
                                                    If isLevelStartFromLMLine Then
                                                        oForm.Items.Item("29").Enabled = False
                                                        oForm.Items.Item("44").Enabled = False
                                                    Else
                                                        oForm.Items.Item("29").Enabled = True
                                                        oForm.Items.Item("44").Enabled = True
                                                    End If
                                                ElseIf strChkH = "N" And strGStatus = "-" Then
                                                    oForm.Items.Item("29").Enabled = False
                                                    oForm.Items.Item("44").Enabled = False
                                                ElseIf strChkH = "Y" And strGStatus = "G" Then
                                                    oForm.Items.Item("29").Enabled = False
                                                    oForm.Items.Item("44").Enabled = False
                                                    oForm.Items.Item("32").Enabled = False
                                                Else
                                                    oForm.Items.Item("29").Enabled = False
                                                    oForm.Items.Item("44").Enabled = False
                                                End If
                                            ElseIf oCombobox.Selected.Value = "SE" Or oCombobox.Selected.Value = "DR" Then
                                                If isLevelStartFromLMLine Then
                                                    oGrid_P1.Columns.Item("Self Rating").Editable = False
                                                    oGrid_P2.Columns.Item("Self Rating").Editable = False
                                                    oGrid_P3.Columns.Item("Self Rating").Editable = False

                                                    oGrid_P1.Columns.Item("U_Z_SelfRaCode").Editable = False
                                                    oGrid_P2.Columns.Item("U_Z_SelfRaCode").Editable = False
                                                    oGrid_P3.Columns.Item("U_Z_SelfRaCode").Editable = False
                                                Else
                                                    oGrid_P1.Columns.Item("Self Rating").Editable = False
                                                    oGrid_P2.Columns.Item("Self Rating").Editable = False
                                                    oGrid_P3.Columns.Item("Self Rating").Editable = False

                                                    oGrid_P1.Columns.Item("U_Z_SelfRaCode").Editable = True
                                                    oGrid_P2.Columns.Item("U_Z_SelfRaCode").Editable = True
                                                    oGrid_P3.Columns.Item("U_Z_SelfRaCode").Editable = True
                                                End If
                                                If isLevelStartFromLMLine Then
                                                    oForm.Items.Item("29").Enabled = False
                                                    oForm.Items.Item("44").Enabled = False
                                                Else
                                                    oForm.Items.Item("29").Enabled = True
                                                    oForm.Items.Item("44").Enabled = True
                                                End If
                                            ElseIf oCombobox.Selected.Value = "HR" And strChkH = "Y" And strGStatus = "-" Then
                                                oForm.Items.Item("32").Enabled = True
                                                If isLevelStartFromLMLine Then
                                                    oForm.Items.Item("29").Enabled = False
                                                    oForm.Items.Item("44").Enabled = False
                                                Else
                                                    oForm.Items.Item("29").Enabled = True
                                                    oForm.Items.Item("44").Enabled = True
                                                End If
                                            End If
                                        ElseIf oForm.Title = "First Level Approval" Then
                                            oGrid_P1.Columns.Item("Code").Editable = False
                                            oGrid_P2.Columns.Item("Code").Editable = False
                                            oGrid_P3.Columns.Item("Code").Editable = False
                                            oGrid_P1.Columns.Item("Business Objectives").Editable = False
                                            oGrid_P2.Columns.Item("People Objectives").Editable = False
                                            oGrid_P3.Columns.Item("Competence Objectives").Editable = False
                                            oGrid_P1.Columns.Item("Weight (%)").Editable = False
                                            oGrid_P2.Columns.Item("Weight (%)").Editable = False
                                            oGrid_P3.Columns.Item("Weight (%)").Editable = False
                                            oGrid_P1.Columns.Item("Self Rating").Editable = False
                                            oGrid_P2.Columns.Item("Self Rating").Editable = False
                                            oGrid_P3.Columns.Item("Self Rating").Editable = False

                                            oGrid_P1.Columns.Item("Line Manager Rating").Editable = False
                                            oGrid_P2.Columns.Item("Line Manager Rating").Editable = False
                                            oGrid_P3.Columns.Item("Line Manager Rating").Editable = False


                                            oGrid_P1.Columns.Item("U_Z_SelfRaCode").Editable = False
                                            oGrid_P2.Columns.Item("U_Z_SelfRaCode").Editable = False
                                            oGrid_P3.Columns.Item("U_Z_SelfRaCode").Editable = False


                                            oGrid_P1.Columns.Item("Sr.Manager Rating").Editable = False
                                            oGrid_P2.Columns.Item("Sr.Manager Rating").Editable = False
                                            oGrid_P3.Columns.Item("Sr.Manager Rating").Editable = False

                                            oGrid_P1.Columns.Item("U_Z_SMRaCode").Editable = False
                                            oGrid_P2.Columns.Item("U_Z_SMRaCode").Editable = False
                                            oGrid_P3.Columns.Item("U_Z_SMRaCode").Editable = False


                                            oGrid_P1.Columns.Item("U_Z_MgrRaCode").Editable = True
                                            oGrid_P2.Columns.Item("U_Z_MgrRaCode").Editable = True
                                            oGrid_P3.Columns.Item("U_Z_MgrRaCode").Editable = True


                                            oGrid_P2.Columns.Item("Category").Editable = False
                                            oGrid_P3.Columns.Item("Levels").Editable = False
                                            Dim oChk As SAPbouiCOM.CheckBox
                                            oChk = oForm.Items.Item("40").Specific
                                            If strChkL = "Y" Then

                                                oChk.Checked = True
                                                oForm.Items.Item("40").Enabled = False
                                                oForm.Items.Item("29").Enabled = False
                                                oForm.Items.Item("44").Enabled = False
                                            Else
                                                oChk.Checked = False
                                                oForm.Items.Item("40").Enabled = True
                                                oForm.Items.Item("29").Enabled = True
                                                oForm.Items.Item("44").Enabled = True
                                            End If

                                            oForm.Items.Item("15").Enabled = False
                                            oForm.Items.Item("19").Enabled = False
                                            oForm.Items.Item("23").Enabled = False
                                            oForm.Items.Item("17").Enabled = False
                                            oForm.Items.Item("21").Enabled = False
                                            oForm.Items.Item("25").Enabled = False
                                            oForm.Items.Item("18").Enabled = False
                                            oForm.Items.Item("22").Enabled = False
                                            oForm.Items.Item("26").Enabled = False
                                            oForm.Items.Item("39").Enabled = False
                                            oForm.Items.Item("41").Enabled = False
                                            oForm.Items.Item("42").Enabled = False
                                        ElseIf oForm.Title = "Second Level Approval" Then
                                            oGrid_P1.Columns.Item("Code").Editable = False
                                            oGrid_P2.Columns.Item("Code").Editable = False
                                            oGrid_P3.Columns.Item("Code").Editable = False
                                            oGrid_P1.Columns.Item("Business Objectives").Editable = False
                                            oGrid_P2.Columns.Item("People Objectives").Editable = False
                                            oGrid_P3.Columns.Item("Competence Objectives").Editable = False
                                            oGrid_P1.Columns.Item("Weight (%)").Editable = False
                                            oGrid_P2.Columns.Item("Weight (%)").Editable = False
                                            oGrid_P3.Columns.Item("Weight (%)").Editable = False

                                            oGrid_P1.Columns.Item("Line Manager Rating").Editable = False
                                            oGrid_P2.Columns.Item("Line Manager Rating").Editable = False
                                            oGrid_P3.Columns.Item("Line Manager Rating").Editable = False

                                            oGrid_P1.Columns.Item("U_Z_SelfRaCode").Editable = False
                                            oGrid_P2.Columns.Item("U_Z_SelfRaCode").Editable = False
                                            oGrid_P3.Columns.Item("U_Z_SelfRaCode").Editable = False


                                            oGrid_P1.Columns.Item("Self Rating").Editable = False
                                            oGrid_P2.Columns.Item("Self Rating").Editable = False
                                            oGrid_P3.Columns.Item("Self Rating").Editable = False

                                            oGrid_P1.Columns.Item("U_Z_SelfRaCode").Editable = False
                                            oGrid_P2.Columns.Item("U_Z_SelfRaCode").Editable = False
                                            oGrid_P3.Columns.Item("U_Z_SelfRaCode").Editable = False


                                            oGrid_P1.Columns.Item("Sr.Manager Rating").Editable = False
                                            oGrid_P2.Columns.Item("Sr.Manager Rating").Editable = False
                                            oGrid_P3.Columns.Item("Sr.Manager Rating").Editable = False

                                            oGrid_P1.Columns.Item("U_Z_SMRaCode").Editable = True
                                            oGrid_P2.Columns.Item("U_Z_SMRaCode").Editable = True
                                            oGrid_P3.Columns.Item("U_Z_SMRaCode").Editable = True


                                            oGrid_P1.Columns.Item("U_Z_MgrRaCode").Editable = False
                                            oGrid_P2.Columns.Item("U_Z_MgrRaCode").Editable = False
                                            oGrid_P3.Columns.Item("U_Z_MgrRaCode").Editable = False


                                            oGrid_P2.Columns.Item("Category").Editable = False
                                            oGrid_P3.Columns.Item("Levels").Editable = False
                                            Dim oChk As SAPbouiCOM.CheckBox
                                            oChk = oForm.Items.Item("41").Specific
                                            If strChkSr = "Y" Then

                                                oChk.Checked = True
                                                oForm.Items.Item("41").Enabled = False
                                                oForm.Items.Item("29").Enabled = False
                                                oForm.Items.Item("44").Enabled = False
                                            Else
                                                If strChkH = "Y" Then
                                                    oForm.Items.Item("41").Enabled = False
                                                    oForm.Items.Item("29").Enabled = False
                                                    oForm.Items.Item("44").Enabled = False
                                                Else
                                                    oChk.Checked = False
                                                    oForm.Items.Item("41").Enabled = True
                                                    oForm.Items.Item("29").Enabled = True
                                                    oForm.Items.Item("44").Enabled = True
                                                End If
                                            End If
                                            oForm.Items.Item("16").Enabled = False
                                            oForm.Items.Item("20").Enabled = False
                                            oForm.Items.Item("24").Enabled = False
                                            oForm.Items.Item("15").Enabled = False
                                            oForm.Items.Item("19").Enabled = False
                                            oForm.Items.Item("23").Enabled = False
                                            oForm.Items.Item("18").Enabled = False
                                            oForm.Items.Item("22").Enabled = False
                                            oForm.Items.Item("26").Enabled = False
                                            oForm.Items.Item("39").Enabled = False
                                            oForm.Items.Item("40").Enabled = False

                                            oForm.Items.Item("42").Enabled = False
                                            'If strGStatus = "-" Then
                                            '    oForm.Items.Item("btnGra").Visible = True
                                            'Else
                                            '    oForm.Items.Item("btnGra").Visible = False
                                            'End If
                                        End If

                                        oForm.Items.Item("1000001").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                        If oForm.Title = "HR Acceptance" Then
                                            oForm.ActiveItem = "18"
                                            oForm.Items.Item("28").Enabled = False
                                        ElseIf oForm.Title = "Self Appraisals" Then
                                            oForm.ActiveItem = "15"
                                            oForm.Items.Item("28").Enabled = False
                                        ElseIf oForm.Title = "Second Level Approval" Then
                                            oForm.ActiveItem = "17"
                                            oForm.Items.Item("28").Enabled = False
                                        Else
                                            oForm.ActiveItem = "16"
                                            oForm.Items.Item("28").Enabled = False
                                        End If

                                        If strDStatus = "C" Then
                                            oForm.Items.Item("29").Enabled = False
                                            oForm.Items.Item("44").Enabled = False
                                        End If
                                    End If
                                    oForm.Freeze(False)
                                ElseIf pVal.ItemUID = "1000001" Then
                                    oForm.Freeze(True)
                                    oForm.Items.Item("8").Visible = True
                                    oForm.PaneLevel = 0
                                    If oForm.Title = "HR Acceptance" Then
                                        oForm.ActiveItem = "18"
                                    ElseIf oForm.Title = "Self Appraisals" Then
                                        oForm.ActiveItem = "15"
                                    ElseIf oForm.Title = "Second Level Approval" Then
                                        oForm.ActiveItem = "17"
                                    Else
                                        oForm.ActiveItem = "16"
                                    End If
                                    oForm.Freeze(False)
                                ElseIf pVal.ItemUID = "5" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 1
                                    If oForm.Title = "HR Acceptance" Then
                                        oForm.ActiveItem = "18"
                                    ElseIf oForm.Title = "Self Appraisals" Then
                                        oForm.ActiveItem = "15"
                                    ElseIf oForm.Title = "Second Level Approval" Then
                                        oForm.ActiveItem = "17"
                                    Else
                                        oForm.ActiveItem = "16"
                                    End If
                                    oForm.Freeze(False)
                                ElseIf pVal.ItemUID = "6" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 2
                                    If oForm.Title = "HR Acceptance" Then
                                        oForm.ActiveItem = "18"
                                    ElseIf oForm.Title = "Self Appraisals" Then
                                        oForm.ActiveItem = "15"
                                    ElseIf oForm.Title = "Second Level Approval" Then
                                        oForm.ActiveItem = "17"
                                    Else
                                        oForm.ActiveItem = "16"
                                    End If
                                    oForm.Freeze(False)
                                ElseIf pVal.ItemUID = "30" Then
                                    oForm.Freeze(True)
                                    oForm.Items.Item("8").Visible = False
                                    oForm.PaneLevel = 3
                                    oForm.Freeze(False)
                                ElseIf pVal.ItemUID = "45" Then
                                    oForm.Freeze(True)
                                    oGrid = oForm.Items.Item("3").Specific
                                    For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                        If oGrid.Rows.IsSelected(intRow) Then
                                            strCode = oGrid.DataTable.GetValue("DocEntry", intRow)
                                            fillHRRating(oForm, oForm.Title, strCode)
                                            oForm.PaneLevel = 4
                                        End If
                                    Next
                                    oForm.Freeze(False)
                                ElseIf pVal.ItemUID = "48" Then
                                    oForm.Freeze(True)
                                    oForm.Items.Item("8").Visible = False
                                    oGrid = oForm.Items.Item("3").Specific
                                    For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                        If oGrid.Rows.IsSelected(intRow) Then
                                            strCode = oGrid.DataTable.GetValue("DocEntry", intRow)
                                            fillWorkFlowTimeStamp(oForm, oForm.Title, strCode)
                                            oForm.PaneLevel = 5
                                        End If
                                    Next
                                    oForm.Freeze(False)
                                End If
                        End Select
                End Select
            End If


        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub

#End Region

#Region "Menu Event"
    Public Overrides Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            Select Case pVal.MenuUID
                Case mnu_hr_MgrAppr
                    LoadForm("MgrApp")
                Case mnu_hr_SMgrAppr
                    LoadForm("SMgrApp")
                Case mnu_InvSO
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
            End Select
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
#End Region

#Region "FormDataEvent"

    Public Sub FormDataEvent(ByRef BusinessObjectInfo As SAPbouiCOM.BusinessObjectInfo, ByRef BubbleEvent As Boolean)
        Try
            If BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
#End Region

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Function validate(ByVal strCol As String) As Boolean
        Dim _retVal As Boolean = True

        oGrid_P1 = oForm.Items.Item("8").Specific
        oGrid_P2 = oForm.Items.Item("9").Specific
        oGrid_P3 = oForm.Items.Item("10").Specific

        If oGrid_P1.Rows.Count > 0 Then
            Dim i As Integer
            For i = 0 To oGrid_P1.Rows.Count - 1
                If oGrid_P1.DataTable.GetValue("Code", i) <> "" Then
                    If (oGrid_P1.DataTable.GetValue(strCol, i)) > 100 Then
                        _retVal = False
                        Return False
                    End If
                End If
            Next
        End If
        If oGrid_P2.Rows.Count > 0 Then
            Dim i As Integer
            For i = 0 To oGrid_P2.Rows.Count - 1
                If oGrid_P2.DataTable.GetValue("Code", i) <> "" Then
                    If (oGrid_P2.DataTable.GetValue(strCol, i)) > 100 Then
                        _retVal = False
                        Return False
                    End If
                End If
            Next
        End If

        'If oGrid_P3.Rows.Count > 0 Then
        '    Dim i As Integer
        '    For i = 0 To oGrid_P3.Rows.Count - 1
        '        If oGrid_P3.DataTable.GetValue("Code", i) <> "" Then

        '            If (oGrid_P3.DataTable.GetValue(strCol, i)) > 100 Then
        '                _retVal = False
        '                Return False
        '            End If
        '        End If
        '    Next
        'End If

        Return _retVal
    End Function

    Private Sub colSum()

        oGrid_P1 = oForm.Items.Item("8").Specific
        oGrid_P2 = oForm.Items.Item("9").Specific
        oGrid_P3 = oForm.Items.Item("10").Specific
        oGrid_P4 = oForm.Items.Item("46").Specific

        oEditTextColumn = oGrid_P1.Columns.Item("Weight (%)")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oEditTextColumn = oGrid_P2.Columns.Item("Weight (%)")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oEditTextColumn = oGrid_P3.Columns.Item("Weight (%)")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto


        oEditTextColumn = oGrid_P1.Columns.Item("Self Rating")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oEditTextColumn = oGrid_P2.Columns.Item("Self Rating")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oEditTextColumn = oGrid_P3.Columns.Item("Self Rating")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto

        oEditTextColumn = oGrid_P1.Columns.Item("Line Manager Rating")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oEditTextColumn = oGrid_P2.Columns.Item("Line Manager Rating")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oEditTextColumn = oGrid_P3.Columns.Item("Line Manager Rating")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto

        oEditTextColumn = oGrid_P1.Columns.Item("Sr.Manager Rating")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oEditTextColumn = oGrid_P2.Columns.Item("Sr.Manager Rating")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oEditTextColumn = oGrid_P3.Columns.Item("Sr.Manager Rating")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto

    End Sub

    Private Sub fillHRRating(ByVal oForm As SAPbouiCOM.Form, ByVal strForm As String, ByVal strDE As String)
        oGrid_P4 = oForm.Items.Item("46").Specific
        Dim strQuery As String
        strQuery = "Select  DocEntry,LineId, U_Z_CompType,(Select Case When SUM(U_Z_BussSMRate) > 0 Then (SUM(U_Z_BussMgrRate) +  SUM(U_Z_BussSMRate))/2 Else SUM(U_Z_BussMgrRate) End "
        strQuery += " From [@Z_HR_SEAPP1] Where DocEntry = '" & strDE & "') As U_Z_AvgComp ,U_Z_HRComp From [@Z_HR_SEAPP4] Where DocEntry = '" & strDE & "' And LineId = 1"
        strQuery += " Union All "
        strQuery += "Select  DocEntry,LineId, U_Z_CompType,(Select Case When SUM(U_Z_PeoSMRate) > 0 Then (SUM(U_Z_PeoMgrRate) +  SUM(U_Z_PeoSMRate))/2 Else SUM(U_Z_PeoMgrRate) End "
        strQuery += " From [@Z_HR_SEAPP2] Where DocEntry = '" & strDE & "' ) As U_Z_AvgComp ,U_Z_HRComp From [@Z_HR_SEAPP4] Where DocEntry = '" & strDE & "' And LineId = 2"
        strQuery += " Union All "
        strQuery += "Select  DocEntry,LineId, U_Z_CompType,(Select Case When SUM(U_Z_CompSMRate) > 0 Then (SUM(U_Z_CompMgrRate) +  SUM(U_Z_CompSMRate))/2 Else SUM(U_Z_CompMgrRate) End "
        strQuery += " From [@Z_HR_SEAPP3] Where DocEntry = '" & strDE & "' ) As U_Z_AvgComp ,U_Z_HRComp From [@Z_HR_SEAPP4] Where DocEntry = '" & strDE & "' And LineId = 3 "


        oGrid_P4.DataTable.ExecuteQuery(strQuery)
        oGrid_P4.Columns.Item("DocEntry").Visible = False
        oGrid_P4.Columns.Item("LineId").Visible = False
        oGrid_P4.Columns.Item("U_Z_CompType").TitleObject.Caption = "Objective Type"
        oGrid_P4.Columns.Item("U_Z_AvgComp").TitleObject.Caption = "Average Rating"
        oGrid_P4.Columns.Item("U_Z_HRComp").TitleObject.Caption = "HR Rating"
        If strForm = "Self Appraisals" Then
            oForm.Items.Item("46").Enabled = False
        ElseIf strForm = "HR Acceptance" Then
            oGrid_P4.Columns.Item("U_Z_CompType").Editable = False
            oGrid_P4.Columns.Item("U_Z_AvgComp").Editable = False
            oGrid_P4.Columns.Item("U_Z_HRComp").Editable = True
        End If

        oEditTextColumn = oGrid_P4.Columns.Item("U_Z_AvgComp")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oEditTextColumn = oGrid_P4.Columns.Item("U_Z_HRComp")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        UpdateFInallRate(oForm)
    End Sub

    Private Sub UpdateHRRating(ByVal oForm As SAPbouiCOM.Form, ByVal strForm As String, ByVal strDE As String)
        oGrid_P4 = oForm.Items.Item("46").Specific
        Dim strQuery As String
        strQuery = "Select  DocEntry,LineId, U_Z_CompType,(Select Case When SUM(U_Z_BussSMRate) > 0 Then (SUM(U_Z_BussMgrRate) +  SUM(U_Z_BussSMRate))/2 Else SUM(U_Z_BussMgrRate) End "
        strQuery += " From [@Z_HR_SEAPP1] Where DocEntry = '" & strDE & "') As U_Z_AvgComp ,U_Z_HRComp From [@Z_HR_SEAPP4] Where DocEntry = '" & strDE & "' And LineId = 1"
        strQuery += " Union All "
        strQuery += "Select  DocEntry,LineId, U_Z_CompType,(Select Case When SUM(U_Z_PeoSMRate) > 0 Then (SUM(U_Z_PeoMgrRate) +  SUM(U_Z_PeoSMRate))/2 Else SUM(U_Z_PeoMgrRate) End "
        strQuery += " From [@Z_HR_SEAPP2] Where DocEntry = '" & strDE & "' ) As U_Z_AvgComp ,U_Z_HRComp From [@Z_HR_SEAPP4] Where DocEntry = '" & strDE & "' And LineId = 2"
        strQuery += " Union All "
        strQuery += "Select  DocEntry,LineId, U_Z_CompType,(Select Case When SUM(U_Z_CompSMRate) > 0 Then (SUM(U_Z_CompMgrRate) +  SUM(U_Z_CompSMRate))/2 Else SUM(U_Z_CompMgrRate) End "
        strQuery += " From [@Z_HR_SEAPP3] Where DocEntry = '" & strDE & "' ) As U_Z_AvgComp ,U_Z_HRComp From [@Z_HR_SEAPP4] Where DocEntry = '" & strDE & "' And LineId = 3 "


        oGrid_P4.DataTable.ExecuteQuery(strQuery)
        oGrid_P4.Columns.Item("DocEntry").Visible = False
        oGrid_P4.Columns.Item("LineId").Visible = False
        oGrid_P4.Columns.Item("U_Z_CompType").TitleObject.Caption = "Objective Type"
        oGrid_P4.Columns.Item("U_Z_AvgComp").TitleObject.Caption = "Average Rating"
        oGrid_P4.Columns.Item("U_Z_HRComp").TitleObject.Caption = "HR Rating"
        If strForm = "Self Appraisals" Then
            oForm.Items.Item("46").Enabled = False
        ElseIf strForm = "HR Acceptance" Then
            oGrid_P4.Columns.Item("U_Z_CompType").Editable = False
            oGrid_P4.Columns.Item("U_Z_AvgComp").Editable = False
            oGrid_P4.Columns.Item("U_Z_HRComp").Editable = True
        End If

        oEditTextColumn = oGrid_P4.Columns.Item("U_Z_AvgComp")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oEditTextColumn = oGrid_P4.Columns.Item("U_Z_HRComp")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        UpdateFInallRate(oForm)
    End Sub


    Private Sub UpdateFinalRating(ByVal oForm As SAPbouiCOM.Form, ByVal strForm As String, ByVal strDE As String)
        oGrid_P4 = oForm.Items.Item("46").Specific

        Dim oTest1, oTest As SAPbobsCOM.Recordset
        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

        Dim strQuery, strQuery1 As String
        strQuery = "Select  DocEntry,LineId, U_Z_CompType,(Select Case When SUM(U_Z_BussSMRate) > 0 Then (SUM(U_Z_BussMgrRate) +  SUM(U_Z_BussSMRate))/2 Else SUM(U_Z_BussMgrRate) End "
        strQuery += " From [@Z_HR_SEAPP1] Where DocEntry = '" & strDE & "') As U_Z_AvgComp ,U_Z_HRComp From [@Z_HR_SEAPP4] Where DocEntry = '" & strDE & "' And LineId = 1"
        strQuery += " Union All "
        strQuery += "Select  DocEntry,LineId, U_Z_CompType,(Select Case When SUM(U_Z_PeoSMRate) > 0 Then (SUM(U_Z_PeoMgrRate) +  SUM(U_Z_PeoSMRate))/2 Else SUM(U_Z_PeoMgrRate) End "
        strQuery += " From [@Z_HR_SEAPP2] Where DocEntry = '" & strDE & "' ) As U_Z_AvgComp ,U_Z_HRComp From [@Z_HR_SEAPP4] Where DocEntry = '" & strDE & "' And LineId = 2"
        strQuery += " Union All "
        strQuery += "Select  DocEntry,LineId, U_Z_CompType,(Select Case When SUM(U_Z_CompSMRate) > 0 Then (SUM(U_Z_CompMgrRate) +  SUM(U_Z_CompSMRate))/2 Else SUM(U_Z_CompMgrRate) End "
        strQuery += " From [@Z_HR_SEAPP3] Where DocEntry = '" & strDE & "' ) As U_Z_AvgComp ,U_Z_HRComp From [@Z_HR_SEAPP4] Where DocEntry = '" & strDE & "' And LineId = 3 "

        oGrid_P4.DataTable.ExecuteQuery(strQuery)
        oGrid_P4.Columns.Item("DocEntry").Visible = False
        oGrid_P4.Columns.Item("LineId").Visible = False
        oGrid_P4.Columns.Item("U_Z_CompType").TitleObject.Caption = "Objective Type"
        oGrid_P4.Columns.Item("U_Z_AvgComp").TitleObject.Caption = "Average Rating"
        oGrid_P4.Columns.Item("U_Z_HRComp").TitleObject.Caption = "HR Rating"
        If strForm = "Self Appraisals" Then
            oForm.Items.Item("46").Enabled = False
        ElseIf strForm = "HR Acceptance" Then
            oGrid_P4.Columns.Item("U_Z_CompType").Editable = False
            oGrid_P4.Columns.Item("U_Z_AvgComp").Editable = False
            oGrid_P4.Columns.Item("U_Z_HRComp").Editable = True
        End If

        oEditTextColumn = oGrid_P4.Columns.Item("U_Z_AvgComp")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        oEditTextColumn = oGrid_P4.Columns.Item("U_Z_HRComp")
        oEditTextColumn.ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto
        UpdateFInallRate(oForm)
    End Sub

    Private Sub UpdateFInallRate(ByVal aform As SAPbouiCOM.Form)
        oGrid_P4 = oForm.Items.Item("46").Specific

        Dim oTest1, oTest As SAPbobsCOM.Recordset
        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim strQuery, strQuery1 As String
        For intRow As Integer = 0 To oGrid_P4.DataTable.Rows.Count - 1
            If oGrid_P4.DataTable.GetValue("U_Z_CompType", intRow) = "Business" Then
                oTest1.DoQuery("Select * from ""@Z_HR_OARE"" where ""U_Z_Obj""='BUSINESS OBJECTIVE'")
                Dim dblWeight, dblValue As Double
                dblValue = oGrid_P4.DataTable.GetValue("U_Z_AvgComp", intRow)
                dblWeight = oTest1.Fields.Item("U_Z_Weight").Value
                If dblWeight > 0 Then
                    dblValue = dblValue * dblWeight / 100
                Else
                    dblValue = 0
                End If
                oGrid_P4.DataTable.SetValue("U_Z_AvgComp", intRow, dblValue)
                oTest.DoQuery("Update ""@Z_HR_SEAPP4"" set U_Z_AvgComp='" & dblValue & "' where ""DocEntry""=" & oGrid_P4.DataTable.GetValue("DocEntry", intRow) & " and ""LineID""=1")
            End If

            If oGrid_P4.DataTable.GetValue("U_Z_CompType", intRow) = "People" Then
                oTest1.DoQuery("Select * from ""@Z_HR_OARE"" where ""U_Z_Obj""='PEOPLE OBJECTIVE'")
                Dim dblWeight, dblValue As Double
                dblValue = oGrid_P4.DataTable.GetValue("U_Z_AvgComp", intRow)
                dblWeight = oTest1.Fields.Item("U_Z_Weight").Value
                If dblWeight > 0 Then
                    dblValue = dblValue * dblWeight / 100
                Else
                    dblValue = 0
                End If
                oGrid_P4.DataTable.SetValue("U_Z_AvgComp", intRow, dblValue)
                oTest.DoQuery("Update ""@Z_HR_SEAPP4"" set U_Z_AvgComp='" & dblValue & "' where ""DocEntry""=" & oGrid_P4.DataTable.GetValue("DocEntry", intRow) & " and ""LineID""=2")

            End If

            If oGrid_P4.DataTable.GetValue("U_Z_CompType", intRow) = "Competency" Then
                oTest1.DoQuery("Select * from ""@Z_HR_OARE"" where ""U_Z_Obj""='COMPTENCIES'")
                Dim dblWeight, dblValue As Double
                dblValue = oGrid_P4.DataTable.GetValue("U_Z_AvgComp", intRow)
                dblWeight = oTest1.Fields.Item("U_Z_Weight").Value
                If dblWeight > 0 Then
                    dblValue = dblValue * dblWeight / 100
                Else
                    dblValue = 0
                End If
                oGrid_P4.DataTable.SetValue("U_Z_AvgComp", intRow, dblValue)
                oTest.DoQuery("Update ""@Z_HR_SEAPP4"" set U_Z_AvgComp='" & dblValue & "' where ""DocEntry""=" & oGrid_P4.DataTable.GetValue("DocEntry", intRow) & " and ""LineID""=3")

            End If
        Next
    End Sub

    Private Sub fillWorkFlowTimeStamp(ByVal oForm As SAPbouiCOM.Form, ByVal strForm As String, ByVal strDE As String)
        Dim oRec As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        sQuery = "Select U_Z_AIUserID,U_Z_AIUDate,LTRIM(RIGHT(CONVERT(VARCHAR(20), U_Z_AIUDate, 100), 7)) As 'U_Z_AIUTime',U_Z_SFUserID,U_Z_SFUDate,LTRIM(RIGHT(CONVERT(VARCHAR(20), U_Z_SFUDate, 100), 7)) As 'U_Z_SFUTime',U_Z_SFAUserID,U_Z_SFAUDate,LTRIM(RIGHT(CONVERT(VARCHAR(20), U_Z_SFAUDate, 100), 7)) As 'U_Z_SFAUTime',U_Z_FUserID,U_Z_FUDate,LTRIM(RIGHT(CONVERT(VARCHAR(20), U_Z_FUDate, 100), 7)) As 'U_Z_FUTime',U_Z_SCUserID,U_Z_SCUDate,LTRIM(RIGHT(CONVERT(VARCHAR(20), U_Z_SCUDate, 100), 7)) As 'U_Z_SCUTime',U_Z_HRUserID,U_Z_HRDate,LTRIM(RIGHT(CONVERT(VARCHAR(20), U_Z_HRDate, 100), 7)) As 'U_Z_HRTime' From [@Z_HR_OSEAPP] Where DocEntry = '" & strDE & "'"
        oRec.DoQuery(sQuery)
        If Not oRec.EoF Then
            oApplication.Utilities.setEdittextvalue(oForm, "81", oRec.Fields.Item("U_Z_AIUserID").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "83", oRec.Fields.Item("U_Z_AIUDate").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "84", oRec.Fields.Item("U_Z_AIUTime").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "50", oRec.Fields.Item("U_Z_SFUserID").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "60", oRec.Fields.Item("U_Z_SFUDate").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "70", oRec.Fields.Item("U_Z_SFUTime").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "52", oRec.Fields.Item("U_Z_SFAUserID").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "62", oRec.Fields.Item("U_Z_SFAUDate").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "72", oRec.Fields.Item("U_Z_SFAUTime").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "54", oRec.Fields.Item("U_Z_FUserID").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "64", oRec.Fields.Item("U_Z_FUDate").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "74", oRec.Fields.Item("U_Z_FUTime").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "56", oRec.Fields.Item("U_Z_SCUserID").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "66", oRec.Fields.Item("U_Z_SCUDate").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "76", oRec.Fields.Item("U_Z_SCUTime").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "58", oRec.Fields.Item("U_Z_HRUserID").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "68", oRec.Fields.Item("U_Z_HRDate").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "78", oRec.Fields.Item("U_Z_HRTime").Value)
        End If
    End Sub

    Private Sub reDrawForm(ByVal oForm As SAPbouiCOM.Form)
        Try
            oForm.Freeze(True)
            oForm.Items.Item("7").Width = oForm.Width - 25
            oForm.Items.Item("7").Height = oForm.Items.Item("10").Height + 10
            Dim intHeight As Int32 = oForm.Items.Item("3").Top + oForm.Items.Item("3").Height
            oForm.Items.Item("27").Top = intHeight
            oForm.Items.Item("27").Left = oForm.Width - 300
            oForm.Items.Item("28").Left = oForm.Items.Item("27").Left + oForm.Items.Item("27").Width
            oForm.Items.Item("28").Top = oForm.Items.Item("27").Top
            oForm.Items.Item("31").Left = oForm.Items.Item("27").Left
            oForm.Items.Item("31").Top = oForm.Items.Item("27").Top + oForm.Items.Item("27").Height + 1
            oForm.Items.Item("32").Left = oForm.Items.Item("28").Left
            oForm.Items.Item("32").Top = oForm.Items.Item("31").Top
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
        End Try
    End Sub

    Private Sub Disable(ByVal strStatus As String)

        Try


            If strStatus = "C" Then
                'oForm.Items.Item("8").Enabled = False
                'oForm.Items.Item("9").Enabled = False
                'oForm.Items.Item("10").Enabled = False
                'oForm.Items.Item("46").Enabled = False
                oForm.Items.Item("79").Enabled = False
                oForm.Items.Item("_3").Enabled = False
                oForm.Items.Item("btnGra").Enabled = False
            Else
                'oForm.Items.Item("8").Enabled = True
                'oForm.Items.Item("9").Enabled = True
                'oForm.Items.Item("10").Enabled = True
                'oForm.Items.Item("46").Enabled = True
                oForm.Items.Item("79").Enabled = True
                oForm.Items.Item("_3").Enabled = True
                oForm.Items.Item("btnGra").Enabled = True
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub

End Class
