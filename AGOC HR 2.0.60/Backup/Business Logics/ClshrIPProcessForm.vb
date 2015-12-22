Imports System.IO
Public Class ClshrIPProcessForm
    Inherits clsBase
    Private InvForConsumedItems As Integer
    Private Shared strFunction As String
    Private oGrid, oGridDetail As SAPbouiCOM.Grid
    Private strHeaderQry, strDetailQry, strFilepath As String
    Private oDT_DIAPI As DataTable
    Private oCombo As SAPbouiCOM.ComboBox
    Dim oGenService As SAPbobsCOM.GeneralService
    Dim oGenData As SAPbobsCOM.GeneralData
    Dim oGenDataCollection As SAPbobsCOM.GeneralDataCollection
    Dim oCompService As SAPbobsCOM.CompanyService
    Dim oChildData As SAPbobsCOM.GeneralData
    Dim oGeneralDataParams As SAPbobsCOM.GeneralDataParams
    Private ocomboCol As SAPbouiCOM.ComboBoxColumn
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Dim sQuery As String
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub

#Region "LoadForm"
    Public Sub LoadForm(ByVal sFunction As String, ByVal strRQType As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_IPProcessForm) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_IPProcessForm, frm_hr_IPProcessForm)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        strFunction = sFunction
        oGrid = oForm.Items.Item("1").Specific
        oGridDetail = oForm.Items.Item("5").Specific
        'oCombo = oForm.Items.Item("7").Specific
        'oCombo.ValidValues.Add("-", "Pending")
        'oCombo.ValidValues.Add("S", "Selected")
        'oCombo.ValidValues.Add("R", "Rejected")
        oForm.ActiveItem = 10
        oForm.DataSources.DataTables.Add("DT_0")
        oForm.DataSources.UserDataSources.Add("CFLRRR", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.DataTables.Add("DT_1")
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
        oGridDetail.DataTable = oForm.DataSources.DataTables.Item("DT_1")
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        strHeaderQry = ""

        Select Case sFunction
            Case "IPLM", "IPLMU"
                oForm.Freeze(True)
                strHeaderQry = ""
                'strHeaderQry = "select DocEntry,U_Z_HRAppID as 'App/ID',U_Z_HRAppName as 'App/Name',U_Z_DeptName as 'Department Name',U_Z_JobPosi as 'Position Name',U_Z_ReqNo as 'Req No',U_Z_Email as 'EMail',U_Z_Mobile as 'Mobile',U_Z_ApplStatus as 'App Status', U_Z_Skills as 'Skills',U_Z_YrExp as 'YOE',case U_Z_IPLUSta when 'S' then 'Selected' when 'R'then 'Rejected' when 'S' then 'Selected' else '-' end as 'LM Status',case U_Z_IPHODSta when 'S' then 'Selected' when 'R'then 'Rejected' when 'S' then 'Selected' else '-' end as 'HOD Status',case U_Z_IPHRSta when 'S' then 'Selected' when 'R'then 'Rejected' when 'S' then 'Selected' else '-' end as 'HR Status' from [@Z_HR_OHEM1] where U_Z_ApplStatus = 'S' And  U_Z_ReqNo='" & strRQType & "' And U_Z_SMgrStatus = 'A'"

                strHeaderQry = "Select T0.DocEntry,T0.U_Z_HRAppID as 'App/ID',T0.U_Z_HRAppName as 'App/Name',T0.U_Z_DeptName as 'Department Name',T0.U_Z_JobPosi as 'Position Name', "
                strHeaderQry += " T0.U_Z_ReqNo as 'Req No',T1.U_Z_MgrStatus As 'Request Status',T0.U_Z_Email as 'EMail',T0.U_Z_Mobile as 'Mobile',T0.U_Z_ApplStatus as 'App Status', T0.U_Z_Skills as 'Skills',T0.U_Z_YrExp as 'YOE',"
                strHeaderQry += " case T0.U_Z_IPLUSta when 'S' then 'Selected' when 'R' then 'Rejected' when 'S' then 'Selected' else 'Pending' end as 'LM Status',case T0.U_Z_IPHODSta when 'S' "
                strHeaderQry += " then 'Selected' when 'R'then 'Rejected' when 'S' then 'Selected' else 'Pending' end as 'HOD Status',case T0.U_Z_IPHRSta when 'S' then 'Selected' when 'R'then "
                strHeaderQry += " 'Rejected' when 'S' then 'Selected' else 'Pending' end as 'HR Status' "
                strHeaderQry += " from [@Z_HR_OHEM1] T0 Join [@Z_HR_ORMPREQ] T1 On T1.DocEntry = T0.U_Z_ReqNo  "
                'strHeaderQry += " And T1.U_Z_EmpCode In (Select EmpID From OHEM Where Manager = (Select empID From OHEM Where UserId = " & oApplication.Company.UserSignature & "))"
                strHeaderQry += " Where  T0.U_Z_ReqNo = " & strRQType & " And T0.U_Z_AppStatus = 'A'"

                oGrid.DataTable.ExecuteQuery(strHeaderQry)
                oForm.Items.Item("1").Enabled = False
                oGrid.Rows.SelectedRows.Add(0)
                Dim oGHCol As SAPbouiCOM.GridColumn
                oGHCol = oGrid.Columns.Item("App/ID")
                oEditTextColumn = oGrid.Columns.Item("App/ID")
                oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee

                oEditTextColumn = oGrid.Columns.Item("Req No")
                oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
                oGrid.Columns.Item("LM Status").TitleObject.Caption = "Interview Summary Status"
                oGrid.Columns.Item("HOD Status").TitleObject.Caption = "Approval Status"
                oGrid.Columns.Item("HR Status").Visible = False
                ' oGrid.Columns.Item("HR Status").TitleObject.Caption = "Second level Approval Status"
                oGrid.Columns.Item("DocEntry").Visible = False
                oGrid.Columns.Item("Department Name").Visible = False
                oGrid.Columns.Item("Mobile").Visible = False
                oGrid.Columns.Item("Skills").Visible = False
                oGrid.Columns.Item("Position Name").Visible = False
                oGrid.Columns.Item("App Status").Visible = False
                oGrid.AutoResizeColumns()

                Dim oCFLs As SAPbouiCOM.ChooseFromListCollection
                oCFLs = oForm.ChooseFromLists
                Dim oCFL As SAPbouiCOM.ChooseFromList
                Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams
                oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
                oCFLCreationParams.MultiSelection = False
                oCFLCreationParams.ObjectType = "171"
                oCFLCreationParams.UniqueID = "UDCFL1"
                oCFL = oCFLs.Add(oCFLCreationParams)

                Dim oCFLCreationParams1 As SAPbouiCOM.ChooseFromListCreationParams
                oCFLCreationParams1 = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
                oCFLCreationParams1.MultiSelection = False
                oCFLCreationParams1.ObjectType = "Z_HR_IRATE"
                oCFLCreationParams1.UniqueID = "UDCFL2"
                oCFL = oCFLs.Add(oCFLCreationParams1)

                Dim oCFLCreationParams2 As SAPbouiCOM.ChooseFromListCreationParams
                oCFLCreationParams2 = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
                oCFLCreationParams2.MultiSelection = False
                oCFLCreationParams2.ObjectType = "171"
                oCFLCreationParams2.UniqueID = "UDCFL3"
                oCFL = oCFLs.Add(oCFLCreationParams2)

                Dim DocNo As Integer = Convert.ToInt32(oGrid.DataTable.GetValue("DocEntry", 0))
                Dim LMStatus As String = oGrid.DataTable.GetValue("LM Status", 0)
                Dim HODStatus As String = oGrid.DataTable.GetValue("HOD Status", 0)
                Dim HRStatus As String = oGrid.DataTable.GetValue("HR Status", 0)
                If DocNo = 0 Then
                    oApplication.Utilities.Message("No Records Found...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    'oForm.Freeze(False)
                    Return
                End If

                oGrid.Columns.Item("Request Status").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                ocomboCol = oGrid.Columns.Item("Request Status")
                ocomboCol.ValidValues.Add("O", "Open")
                ocomboCol.ValidValues.Add("SA", "HOD Approved")
                ocomboCol.ValidValues.Add("SR", "HOD Rejected")
                ocomboCol.ValidValues.Add("C", "Closed")
                ocomboCol.ValidValues.Add("L", "Canceled")
                ocomboCol.ValidValues.Add("HF", "HR Follow-Up")
                ocomboCol.ValidValues.Add("HA", "HR Approved")
                ocomboCol.ValidValues.Add("HR", "HR Rejected")
                ocomboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both


                If oGrid.DataTable.Rows.Count > 0 Then
                    Dim strStatus As String = oGrid.DataTable.GetValue("Request Status", 0)
                    If strStatus = "C" Or strStatus = "L" Then
                        oForm.Items.Item("3").Enabled = False
                        oForm.Items.Item("11").Enabled = False
                        oForm.Items.Item("12").Enabled = False
                        oForm.Items.Item("13").Enabled = False
                        oForm.Items.Item("14").Enabled = False
                    End If
                End If

                If sFunction = "IPLM" Then
                    ForLineManager(DocNo, 1, LMStatus, HODStatus, HRStatus)
                Else
                    ForLineManager(DocNo, 2, LMStatus, HODStatus, HRStatus)
                End If

                oForm.Items.Item("15").Visible = False
                oForm.Items.Item("16").Visible = False

                oForm.Freeze(False)

            Case "IPHOD"
                oForm.Freeze(True)
                strHeaderQry = ""
                'strHeaderQry = "select DocEntry,U_Z_HRAppID as 'App/ID',U_Z_HRAppName as 'App/Name',U_Z_DeptName as 'Department Name',U_Z_JobPosi as 'Position Name',U_Z_ReqNo as 'Req No',U_Z_Email as 'EMail',U_Z_Mobile as 'Mobile',U_Z_ApplStatus as 'App Status', U_Z_Skills as 'Skills',U_Z_YrExp as 'YOE',case U_Z_IPLUSta   when 'S' then 'Selected' when 'R'then 'Rejected' else '-' end as 'LM Status',case U_Z_IPHODSta when 'S' then 'Selected' when 'R'then 'Rejected' else '-' end as 'HOD Status',case U_Z_IPHRSta when 'S' then 'Selected' when 'R'then 'Rejected'  else '-' end as 'HR Status' from [@Z_HR_OHEM1] where U_Z_ApplStatus = 'S' And  U_Z_ReqNo='" & strRQType & "' And U_Z_SMgrStatus = 'A'"

                strHeaderQry = "Select T0.DocEntry,T0.U_Z_HRAppID as 'App/ID',T0.U_Z_HRAppName as 'App/Name',T0.U_Z_DeptName as 'Department Name',T0.U_Z_JobPosi as 'Position Name', "
                strHeaderQry += " T0.U_Z_ReqNo as 'Req No',T1.U_Z_MgrStatus As 'Request Status',T0.U_Z_Email as 'EMail',T0.U_Z_Mobile as 'Mobile',T0.U_Z_ApplStatus as 'App Status', T0.U_Z_Skills as 'Skills',T0.U_Z_YrExp as 'YOE',"
                strHeaderQry += " case T0.U_Z_IPLUSta when 'S' then 'Selected' when 'R' then 'Rejected' when 'S' then 'Selected' else 'Pending' end as 'LM Status',case T0.U_Z_IPHODSta when 'S' "
                strHeaderQry += " then 'Selected' when 'R'then 'Rejected' when 'S' then 'Selected' else 'Pending' end as 'HOD Status',case T0.U_Z_IPHRSta when 'S' then 'Selected' when 'R'then "
                strHeaderQry += " 'Rejected' when 'S' then 'Selected' else 'Pending' end as 'HR Status' "
                strHeaderQry += " from [@Z_HR_OHEM1] T0 Join [@Z_HR_ORMPREQ] T1 On T1.DocEntry = T0.U_Z_ReqNo  "
                'strHeaderQry += " And T1.U_Z_EmpCode In (Select EmpID From OHEM Where Manager = (Select EmpID From OHEM Where Manager = (Select empID From OHEM Where UserId = " & oApplication.Company.UserSignature & " ))"
                'strHeaderQry += " Union Select EmpID From OHEM Where Manager = (Select empID From OHEM Where UserId = " & oApplication.Company.UserSignature & " )) "
                strHeaderQry += " Where T0.U_Z_AppStatus = 'A' And T0.U_Z_ReqNo = " & strRQType & ""

                oGrid.DataTable.ExecuteQuery(strHeaderQry)
                oForm.Items.Item("1").Enabled = False
                oGrid.Rows.SelectedRows.Add(0)
                Dim oGHCol As SAPbouiCOM.GridColumn
                oEditTextColumn = oGrid.Columns.Item("Req No")
                oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
                oGHCol = oGrid.Columns.Item("App/ID")
                oEditTextColumn = oGrid.Columns.Item("App/ID")
                oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
                oGrid.Columns.Item("LM Status").TitleObject.Caption = "Interview Summary Status"
                oGrid.Columns.Item("HOD Status").TitleObject.Caption = "Approval Status"
                oGrid.Columns.Item("HR Status").Visible = False
                oGrid.Columns.Item("DocEntry").Visible = False
                oGrid.Columns.Item("Department Name").Visible = False
                oGrid.Columns.Item("Mobile").Visible = False
                oGrid.Columns.Item("Skills").Visible = False
                oGrid.Columns.Item("Position Name").Visible = False
                oGrid.Columns.Item("App Status").Visible = False
                oGrid.AutoResizeColumns()
                Dim DocNo As Integer = Convert.ToInt32(oGrid.DataTable.GetValue("DocEntry", 0))
                Dim HODStatus As String = oGrid.DataTable.GetValue("HOD Status", 0)
                Dim HRStatus As String = oGrid.DataTable.GetValue("HR Status", 0)
                If DocNo = 0 Then
                    oApplication.Utilities.Message("No Records Found...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return
                End If
                oForm.Items.Item("15").Visible = True
                oForm.Items.Item("16").Visible = True

                Dim oCFLs1 As SAPbouiCOM.ChooseFromListCollection
                oCFLs1 = oForm.ChooseFromLists
                Dim oCFL1 As SAPbouiCOM.ChooseFromList
                Dim oCFLCreationParams5 As SAPbouiCOM.ChooseFromListCreationParams
                oCFLCreationParams5 = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
                oCFLCreationParams5.MultiSelection = False
                oCFLCreationParams5.UniqueID = "CFLRR"
                oCFLCreationParams5.ObjectType = "Z_HR_OREJC"
                oCFL1 = oCFLs1.Add(oCFLCreationParams5)
                Dim oEdit As SAPbouiCOM.EditText
                oEdit = oForm.Items.Item("16").Specific
                oEdit.DataBind.SetBound(True, "", "CFLRRR")
                oEdit.ChooseFromListUID = "CFLRR"
                oEdit.ChooseFromListAlias = "U_Z_TypeCode"

                oGrid.Columns.Item("Request Status").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                ocomboCol = oGrid.Columns.Item("Request Status")
                ocomboCol.ValidValues.Add("O", "Open")
                ocomboCol.ValidValues.Add("SA", "HOD Approved")
                ocomboCol.ValidValues.Add("SR", "HOD Rejected")
                ocomboCol.ValidValues.Add("C", "Closed")
                ocomboCol.ValidValues.Add("L", "Canceled")
                ocomboCol.ValidValues.Add("HF", "HR Follow-Up")
                ocomboCol.ValidValues.Add("HA", "HR Approved")
                ocomboCol.ValidValues.Add("HR", "HR Rejected")
                ocomboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both


                If oGrid.DataTable.Rows.Count > 0 Then
                    Dim strStatus As String = oGrid.DataTable.GetValue("Request Status", 0)
                    If strStatus = "C" Or strStatus = "L" Then
                        oForm.Items.Item("3").Enabled = False
                        oForm.Items.Item("11").Enabled = False
                        oForm.Items.Item("12").Enabled = False
                        oForm.Items.Item("13").Enabled = False
                        oForm.Items.Item("14").Enabled = False
                    End If
                End If

                ForHOD(DocNo, HODStatus, HRStatus)
                oForm.Freeze(False)
            Case "IPHR"
                oForm.Freeze(True)
                strHeaderQry = ""
                'strHeaderQry = "select DocEntry,U_Z_HRAppID as 'App/ID',U_Z_HRAppName as 'App/Name',U_Z_DeptName as 'Department Name',U_Z_JobPosi as 'Position Name',U_Z_ReqNo as 'Req No',U_Z_Email as 'EMail',U_Z_Mobile as 'Mobile',U_Z_ApplStatus as 'App Status', U_Z_Skills as 'Skills',U_Z_YrExp as 'YOE',U_Z_IPLUSta when 'S' then 'Selected' when 'R'then 'Rejected' when 'S' then 'Selected' else '-' end as 'LM Status',case U_Z_IPHODSta when 'S' then 'Selected' when 'R'then 'Rejected' when 'S' then 'Selected' else '-' end as 'HOD Status',case U_Z_IPHRSta when 'S' then 'Selected' when 'R'then 'Rejected' when 'S' then 'Selected' else '-' end as 'HR Status' from [@Z_HR_OHEM1] where U_Z_ApplStatus = 'S' And  U_Z_ReqNo='" & strRQType & "'  And U_Z_SMgrStatus = 'A'"
                strHeaderQry = "Select T0.DocEntry,T0.U_Z_HRAppID as 'App/ID',T0.U_Z_HRAppName as 'App/Name',T0.U_Z_DeptName as 'Department Name',T0.U_Z_JobPosi as 'Position Name', "
                strHeaderQry += " T0.U_Z_ReqNo as 'Req No',T1.U_Z_MgrStatus As 'Request Status',T0.U_Z_Email as 'EMail',T0.U_Z_Mobile as 'Mobile',T0.U_Z_ApplStatus as 'App Status', T0.U_Z_Skills as 'Skills',T0.U_Z_YrExp as 'YOE',"
                strHeaderQry += " case T0.U_Z_IPLUSta when 'S' then 'Selected' when 'R' then 'Rejected' else 'Pending' end as 'LM Status',case T0.U_Z_IPHODSta when 'S' "
                strHeaderQry += " then 'Selected' when 'R'then 'Rejected' when 'S' then 'Selected' else 'Pending' end as 'HOD Status',case T0.U_Z_IPHRSta when 'S' then 'Selected' when 'R'then "
                strHeaderQry += " 'Rejected' when 'S' then 'Selected' else 'Pending' end as 'HR Status' "
                strHeaderQry += " from [@Z_HR_OHEM1] T0 Join [@Z_HR_ORMPREQ] T1 On T1.DocEntry = T0.U_Z_ReqNo  "
                strHeaderQry += " Where T0.U_Z_AppStatus = 'A' And T0.U_Z_ReqNo = " & strRQType & ""

                oGrid.DataTable.ExecuteQuery(strHeaderQry)
                oForm.Items.Item("1").Enabled = False
                oGrid.Rows.SelectedRows.Add(0)
                Dim oGHCol As SAPbouiCOM.GridColumn
                oEditTextColumn = oGrid.Columns.Item("Req No")
                oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
                oGHCol = oGrid.Columns.Item("App/ID")
                oEditTextColumn = oGrid.Columns.Item("App/ID")
                oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
                oGrid.Columns.Item("LM Status").TitleObject.Caption = "Interview Summary Status"
                oGrid.Columns.Item("HOD Status").TitleObject.Caption = "Approval Status"
                oGrid.Columns.Item("HR Status").Visible = False
                oGrid.Columns.Item("DocEntry").Visible = False
                oGrid.Columns.Item("Department Name").Visible = False
                oGrid.Columns.Item("Mobile").Visible = False
                oGrid.Columns.Item("Skills").Visible = False
                oGrid.Columns.Item("Position Name").Visible = False
                oGrid.Columns.Item("App Status").Visible = False
                oGrid.AutoResizeColumns()
                Dim DocNo As Integer = Convert.ToInt32(oGrid.DataTable.GetValue("DocEntry", 0))
                Dim HRStatus As String = oGrid.DataTable.GetValue("HR Status", 0)
                If DocNo = 0 Then
                    oApplication.Utilities.Message("No Records Found...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return
                End If
                oForm.Items.Item("15").Visible = True
                oForm.Items.Item("16").Visible = True


                Dim oCFLs1 As SAPbouiCOM.ChooseFromListCollection
                oCFLs1 = oForm.ChooseFromLists
                Dim oCFL1 As SAPbouiCOM.ChooseFromList
                Dim oCFLCreationParams5 As SAPbouiCOM.ChooseFromListCreationParams
                oCFLCreationParams5 = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
                oCFLCreationParams5.MultiSelection = False
                oCFLCreationParams5.UniqueID = "CFLRR"
                oCFLCreationParams5.ObjectType = "Z_HR_OREJC"
                oCFL1 = oCFLs1.Add(oCFLCreationParams5)
                Dim oEdit As SAPbouiCOM.EditText
                oEdit = oForm.Items.Item("16").Specific
                oEdit.DataBind.SetBound(True, "", "CFLRRR")
                oEdit.ChooseFromListUID = "CFLRR"
                oEdit.ChooseFromListAlias = "U_Z_TypeCode"

                oGrid.Columns.Item("Request Status").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                ocomboCol = oGrid.Columns.Item("Request Status")
                ocomboCol.ValidValues.Add("O", "Open")
                ocomboCol.ValidValues.Add("SA", "HOD Approved")
                ocomboCol.ValidValues.Add("SR", "HOD Rejected")
                ocomboCol.ValidValues.Add("C", "Closed")
                ocomboCol.ValidValues.Add("L", "Canceled")
                ocomboCol.ValidValues.Add("HF", "HR Follow-Up")
                ocomboCol.ValidValues.Add("HA", "HR Approved")
                ocomboCol.ValidValues.Add("HR", "HR Rejected")
                ocomboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

                If oGrid.DataTable.Rows.Count > 0 Then
                    Dim strStatus As String = oGrid.DataTable.GetValue("Request Status", 0)
                    If strStatus = "C" Or strStatus = "L" Then
                        oForm.Items.Item("3").Enabled = False
                        oForm.Items.Item("11").Enabled = False
                        oForm.Items.Item("12").Enabled = False
                        oForm.Items.Item("13").Enabled = False
                        oForm.Items.Item("14").Enabled = False
                    End If
                End If

                ForHR(DocNo, HRStatus)
                oForm.Freeze(False)
        End Select
        reDrawScreen(oForm)
        oForm.Freeze(False)
    End Sub

#End Region

#Region "For LineManager"
    Private Sub ForLineManager(ByVal DocNo As Integer, ByVal iFun As Integer, ByVal LMStatus As String, ByVal HODStatus As String, ByVal HRStatus As String)
        If iFun = 1 Then
            oForm.Title = "Interview Scheduling"
            oForm.Items.Item("13").Visible = False
            oForm.Items.Item("14").Visible = False
            oForm.Items.Item("8").Visible = False
            oForm.Items.Item("9").Visible = False
            oForm.Items.Item("2").Visible = True
            oForm.Items.Item("11").Visible = True
            oForm.Items.Item("12").Visible = True
            oForm.Items.Item("2").Left = oForm.Items.Item("3").Left + oForm.Items.Item("3").Width + 5
        Else
            oForm.Title = "Interview Summary"
            'oForm.Items.Item("13").Visible = True
            'oForm.Items.Item("14").Visible = True
            oForm.Items.Item("2").Left = oForm.Items.Item("3").Left + oForm.Items.Item("3").Width + 5

            oForm.Items.Item("8").Visible = True
            oForm.Items.Item("9").Visible = True
            oForm.Items.Item("11").Visible = False
            oForm.Items.Item("12").Visible = False
        End If

        oForm.Items.Item("5").Visible = True
        oGridDetail = oForm.Items.Item("5").Specific

        

        strDetailQry = ""
        strDetailQry = "Select ISNULL(U_Z_InType,'-') as 'Interview Type',U_Z_ScheduleDate as 'Schedule Date',U_Z_SchEmpID as 'Scheduler EmpID', T1.FirstName As 'Scheduler Name' ,U_Z_ScTime as 'Schedule Time',U_Z_InterviewDate as 'Interview Date',U_Z_InterviwerID as 'Interviewer EmpID',ISNULL(U_Z_Status,'-') as 'Status',U_Z_InterviewStatus as 'Interview Status',U_Z_Rating as 'Rating',U_Z_RatPer as 'Rating Percentage',U_Z_FileName as 'Attachment',U_Z_Comments as 'Comments',LineId as 'LineNo' from [@Z_HR_OHEM2] T0 Left Outer Join OHEM T1 On T0.U_Z_SchEmpID = T1.EmpID Where DocEntry = " & DocNo & ""
        oGridDetail.DataTable.ExecuteQuery(strDetailQry)
        Dim oGCol0, oGCol1, oGCol2, oGCol3, oGCol4, oGCol6, oGCol7, oGCol8, oGCol9, oGCol11, oGCol12, oGCol13, oGCol21 As SAPbouiCOM.GridColumn
        Dim oGCCol0, oGCCol1, oGCCol2 As SAPbouiCOM.ComboBoxColumn
        Dim oGECol, oGECol1, oGECol2, oGECol14 As SAPbouiCOM.EditTextColumn
        oGCol0 = oGridDetail.Columns.Item("Interview Type")
        If iFun = 1 Then
            oGCol1 = oGridDetail.Columns.Item("Status")
            oGCol1.Visible = True
        Else
            oGCol1 = oGridDetail.Columns.Item("Status")
            oGCol1.Visible = True
        End If
        oGCol2 = oGridDetail.Columns.Item("Interview Status")
        oGCol3 = oGridDetail.Columns.Item("Interview Date")
        '  oGCol2.Visible = False
        oGCol3.Visible = False
        oGCol4 = oGridDetail.Columns.Item("Interviewer EmpID")
        oGCol4.Visible = False
        oGCol6 = oGridDetail.Columns.Item("Schedule Date")
        oGCol7 = oGridDetail.Columns.Item("Rating")
        oGCol8 = oGridDetail.Columns.Item("Attachment")
        oEditTextColumn = oGridDetail.Columns.Item("Attachment")
        oEditTextColumn.LinkedObjectType = "Z_HR_OEXFOM"
        oGCol9 = oGridDetail.Columns.Item("Comments")
        oGCol11 = oGridDetail.Columns.Item("Scheduler EmpID")
        oGCol12 = oGridDetail.Columns.Item("Schedule Time")
        oGCol13 = oGridDetail.Columns.Item("Rating Percentage")
        oGCol21 = oGridDetail.Columns.Item("LineNo")
        oGECol14 = oGridDetail.Columns.Item("Scheduler Name")

        oGCol11.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
        oGCol4.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee

        oGECol = oGridDetail.Columns.Item("Interviewer EmpID")
        oGECol1 = oGridDetail.Columns.Item("Rating")
        oGECol2 = oGridDetail.Columns.Item("Scheduler EmpID")


        oGECol.ChooseFromListUID = "UDCFL1"
        oGECol.ChooseFromListAlias = "empID"
        oGECol1.ChooseFromListUID = "UDCFL2"

        oGECol1.ChooseFromListAlias = "U_Z_RateCode"
        oGECol2.ChooseFromListUID = "UDCFL3"
        oGECol2.ChooseFromListAlias = "empID"


        oGCol0.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGCol1.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGCol2.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGCCol0 = oGridDetail.Columns.Item("Interview Type")
        oGCCol1 = oGridDetail.Columns.Item("Status")
        oGCCol2 = oGridDetail.Columns.Item("Interview Status")

        Dim oRec As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRec.DoQuery("Select U_Z_TypeCode As Code,U_Z_TypeName As Name From [@Z_HR_OITYP]")
        oGCCol0.ValidValues.Add("-", "-")
        For i As Integer = 0 To oRec.RecordCount - 1
            oGCCol0.ValidValues.Add(oRec.Fields.Item("Code").Value.ToString(), oRec.Fields.Item("Name").Value.ToString())
            'oGCCol0.ValidValues.Add(i, "Type-" & i & "")
            oRec.MoveNext()
        Next

        oGCCol0.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

        oGCCol1.ValidValues.Add("-", "Pending")
        oGCCol1.ValidValues.Add("CO", "Conducted")
        oGCCol1.ValidValues.Add("CA", "Cancelled")
        oGCCol1.ValidValues.Add("RS", "Rescheduled")

        oGCCol1.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

        oGCCol2.ValidValues.Add("P", "Pending")
        oGCCol2.ValidValues.Add("S", "Selected")
        oGCCol2.ValidValues.Add("R", "Rejected")
        oGCCol2.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both



        oGridDetail.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single


        If iFun = 1 Then
            oGCol0.Editable = True
            oGCol6.Editable = True

            oGCol1.Editable = False
            oGCol2.Editable = False
            oGCol3.Editable = False
            oGCol4.Editable = False
            oGCol7.Editable = False
            oGCol8.Editable = False
            oGCol9.Editable = False
            oGCol13.Editable = False
            oGECol14.Editable = False
            oGCol21.Visible = False
        Else
            oGCol0.Editable = False
            oGCol6.Editable = False
            oGCol11.Editable = False
            oGCol12.Editable = False
            oGCol1.Editable = True
            oGCol2.Editable = True
            oGCol3.Editable = True
            oGCol4.Editable = True
            oGCol7.Editable = True
            oGCol8.Editable = True
            oGCol9.Editable = True
            oGCol13.Editable = False
            oGCol21.Visible = False
            oGECol14.Editable = False
        End If

        'Dim oCombobox As SAPbouiCOM.ComboBox
        'oCombobox = oForm.Items.Item("7").Specific
        'oCombobox.Select(LMStatus, SAPbouiCOM.BoSearchKey.psk_ByDescription)
        oApplication.Utilities.setEdittextvalue(oForm, "9", "")

        If HODStatus <> "Pending" Then
            oForm.Items.Item("5").Enabled = False
        Else
            oForm.Items.Item("5").Enabled = True
        End If

        If HODStatus <> "Pending" Or HRStatus <> "Pending" Then
            oForm.Items.Item("3").Enabled = False
            oForm.Items.Item("11").Enabled = False
            oForm.Items.Item("12").Enabled = False
        Else
            oForm.Items.Item("3").Enabled = True
            oForm.Items.Item("11").Enabled = True
            oForm.Items.Item("12").Enabled = True
        End If

        oForm.Items.Item("2").Visible = True
    End Sub
#End Region

#Region "For HOD"
    Private Sub ForHOD(ByVal DocNo As Integer, ByVal HODStatus As String, ByVal HRStatus As String)
        oForm.Title = "Final Candidate First Level Approval"
        oForm.Items.Item("3").Visible = False
        oForm.Items.Item("2").Visible = True
        oGridDetail = oForm.Items.Item("5").Specific
        strDetailQry = ""
        strDetailQry = "Select ISNULL(U_Z_InType,'-') as 'Interview Type',U_Z_ScheduleDate as 'Schedule Date',U_Z_SchEmpID as 'Scheduler EmpID', T1.FirstName As 'Scheduler Name' ,U_Z_InterviewDate as 'Interview Date',U_Z_InterviwerID as 'Interviewer EmpID',U_Z_Status as 'Status',U_Z_InterviewStatus as 'Interview Status',U_Z_Rating as 'Rating',U_Z_RatPer as 'Rating Percentage',U_Z_FileName as 'Attachment',U_Z_Comments as 'Comments' from [@Z_HR_OHEM2] T0 Left Outer Join OHEM T1 On T0.U_Z_SchEmpID = T1.EmpID Where DocEntry = " & DocNo & ""
        oGridDetail.DataTable.ExecuteQuery(strDetailQry)

        Dim oGCol0, oGCol1, oGCol2 As SAPbouiCOM.GridColumn
        Dim oGCCol0, oGCCol1, oGCCol2 As SAPbouiCOM.ComboBoxColumn
        Dim oGECol, oGECol1, oGECol14 As SAPbouiCOM.EditTextColumn
        oGCol0 = oGridDetail.Columns.Item("Interview Type")
        oGCol1 = oGridDetail.Columns.Item("Status")
        oGCol2 = oGridDetail.Columns.Item("Interview Status")
        oGECol = oGridDetail.Columns.Item("Interviewer EmpID")
        oGECol1 = oGridDetail.Columns.Item("Rating")

        oGCol0.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGCol1.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGCol2.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGCCol0 = oGridDetail.Columns.Item("Interview Type")
        oGCCol1 = oGridDetail.Columns.Item("Status")
        oGCCol2 = oGridDetail.Columns.Item("Interview Status")

        oGECol14 = oGridDetail.Columns.Item("Scheduler EmpID")

        oGECol14.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
        oGECol.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee

        Dim oRec As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

        oRec.DoQuery("Select U_Z_TypeCode As Code,U_Z_TypeName As Name From [@Z_HR_OITYP]")
        oGCCol0.ValidValues.Add("-", "-")
        For i As Integer = 0 To oRec.RecordCount - 1
            'oGCCol0.ValidValues.Add(i, i)
            oGCCol0.ValidValues.Add(oRec.Fields.Item("Code").Value.ToString(), oRec.Fields.Item("Name").Value.ToString())
            oRec.MoveNext()
        Next
        oGCCol0.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

        oGCCol1.ValidValues.Add("-", "Pending")
        oGCCol1.ValidValues.Add("CO", "Conducted")
        oGCCol1.ValidValues.Add("CA", "Cancelled")
        oGCCol1.ValidValues.Add("RS", "Rescheduled")

        oGCCol1.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

        oGCCol2.ValidValues.Add("P", "Pending")
        oGCCol2.ValidValues.Add("S", "Selected")
        oGCCol2.ValidValues.Add("R", "Rejected")
        oGCCol2.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

        oGridDetail.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single


        oForm.Items.Item("5").Enabled = False
        oForm.Items.Item("13").Visible = True
        oForm.Items.Item("14").Visible = True
        oForm.Items.Item("2").Left = oForm.Items.Item("14").Left + oForm.Items.Item("14").Width + 5
        oForm.Items.Item("8").Visible = True
        oForm.Items.Item("9").Visible = True
        oForm.Items.Item("11").Visible = False
        oForm.Items.Item("12").Visible = False

        'Dim oCombobox As SAPbouiCOM.ComboBox
        'oCombobox = oForm.Items.Item("7").Specific
        'oCombobox.Select(HODStatus, SAPbouiCOM.BoSearchKey.psk_ByDescription)
        'oApplication.Utilities.setEdittextvalue(oForm, "9", "")

        If HRStatus <> "Pending" Then
            oForm.Items.Item("3").Enabled = False
        Else
            oForm.Items.Item("3").Enabled = True
        End If

        oForm.Items.Item("5").Height = 180

    End Sub
#End Region
    Private Sub LoadFiles(ByVal aform As SAPbouiCOM.Form)
        oGrid = aform.Items.Item("5").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.Rows.IsSelected(intRow) Then
                Dim strFilename, strFilePath As String
                strFilename = oGrid.DataTable.GetValue("Attachment", intRow)
                Dim Filename As String = Path.GetFileName(strFilename)
                strFilePath = oGrid.DataTable.GetValue("Attachment", intRow)

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
#Region "For HR"
    Private Sub ForHR(ByVal DocNo As Integer, ByVal HRStatus As String)
        oForm.Title = "Final Candidate HR Approval"
        oForm.Items.Item("3").Visible = False
        oForm.Items.Item("2").Visible = True
        oGridDetail = oForm.Items.Item("5").Specific
        strDetailQry = ""
        strDetailQry = "Select ISNULL(U_Z_InType,'-') as 'Interview Type',U_Z_ScheduleDate as 'Schedule Date',U_Z_SchEmpID as 'Scheduler EmpID', T1.FirstName As 'Scheduler Name',U_Z_InterviewDate as 'Interview Date',U_Z_InterviwerID as 'Interviewer EmpID',U_Z_Status as 'Status',U_Z_InterviewStatus as 'Interview Status',U_Z_Rating as 'Rating',U_Z_RatPer as 'Rating Percentage',U_Z_FileName as 'Attachment',U_Z_Comments as 'Comments' from [@Z_HR_OHEM2] T0 Left Outer Join OHEM T1 On T0.U_Z_SchEmpID = T1.EmpID Where DocEntry=" & DocNo & ""
        oGridDetail.DataTable.ExecuteQuery(strDetailQry)

        Dim oGCol0, oGCol1, oGCol2 As SAPbouiCOM.GridColumn
        Dim oGCCol0, oGCCol1, oGCCol2 As SAPbouiCOM.ComboBoxColumn
        Dim oGECol, oGECol1, oGECol14 As SAPbouiCOM.EditTextColumn
        oGCol0 = oGridDetail.Columns.Item("Interview Type")
        oGCol1 = oGridDetail.Columns.Item("Status")
        oGCol2 = oGridDetail.Columns.Item("Interview Status")
        oGECol = oGridDetail.Columns.Item("Interviewer EmpID")
        oGECol1 = oGridDetail.Columns.Item("Rating")
        oGCol0.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGCol1.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGCol2.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGCCol0 = oGridDetail.Columns.Item("Interview Type")
        oGCCol1 = oGridDetail.Columns.Item("Status")
        oGCCol2 = oGridDetail.Columns.Item("Interview Status")

        oGECol14 = oGridDetail.Columns.Item("Scheduler EmpID")
        oGECol14.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
        oGECol.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee

        Dim oRec As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRec.DoQuery("Select U_Z_TypeCode As Code,U_Z_TypeName As Name From [@Z_HR_OITYP]")
        oGCCol0.ValidValues.Add("-", "-")
        For i As Integer = 0 To oRec.RecordCount - 1
            oGCCol0.ValidValues.Add(oRec.Fields.Item("Code").Value.ToString(), oRec.Fields.Item("Name").Value.ToString())
            oRec.MoveNext()
        Next
        oGCCol0.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

        oGCCol1.ValidValues.Add("-", "Pending")
        oGCCol1.ValidValues.Add("CO", "Conducted")
        oGCCol1.ValidValues.Add("CA", "Cancelled")
        oGCCol1.ValidValues.Add("RS", "Rescheduled")

        oGCCol1.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

        oGCCol2.ValidValues.Add("P", "Pending")
        oGCCol2.ValidValues.Add("S", "Selected")
        oGCCol2.ValidValues.Add("R", "Rejected")
        oGCCol2.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

        oGridDetail.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single

        oForm.Items.Item("5").Enabled = False
        oForm.Items.Item("13").Visible = True
        oForm.Items.Item("14").Visible = True
        oForm.Items.Item("2").Left = oForm.Items.Item("14").Left + oForm.Items.Item("14").Width + 5
        oForm.Items.Item("8").Visible = True
        oForm.Items.Item("9").Visible = True
        oForm.Items.Item("11").Visible = False
        oForm.Items.Item("12").Visible = False

        oApplication.Utilities.setEdittextvalue(oForm, "9", "")
        oForm.Items.Item("5").Height = 180
    End Sub
#End Region

#Region "LineManager DIAPI"
    Private Function LineManager_DIAPI(ByVal DocNo As String, ByVal strApplicantID As String, ByVal oDT As DataTable, ByVal iFunction As Integer, ByVal oHT As Hashtable) As Boolean
        Dim strAPPID As String
        Dim RetVal As Boolean
        Dim SchTime As Integer
        RetVal = False
        Dim oTemp As SAPbobsCOM.Recordset
        Try
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If
            oApplication.Company.StartTransaction()
            oCompService = oApplication.Company.GetCompanyService
            oGenService = oCompService.GetGeneralService("Z_HR_OHEM")
            oGenData = oGenService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData)
            oGeneralDataParams = oGenService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)
            oGeneralDataParams.SetProperty("DocEntry", Convert.ToInt32(DocNo))
            oGenData = oGenService.GetByParams(oGeneralDataParams)
            oGenDataCollection = oGenData.Child("Z_HR_OHEM2")
            oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            If oHT.Count > 0 Then
                oGenData.SetProperty("U_Z_IPLURmks", oHT("RMrk").ToString())
                oGenData.SetProperty("U_Z_IPLUSta", oHT("Status").ToString())
            End If

            For i As Integer = 0 To oDT.Rows.Count - 1
                Dim odr As DataRow = oDT.Rows(i)
                If i > oGenDataCollection.Count - 1 Then
                    oChildData = oGenDataCollection.Add()
                Else
                    oChildData = oGenDataCollection.Item(i)
                End If
                If iFunction = 1 Then
                    oChildData.SetProperty("U_Z_HRAppID", strApplicantID)
                    oChildData.SetProperty("U_Z_InType", odr("InType").ToString())
                    oChildData.SetProperty("U_Z_ScheduleDate", odr("SDate").ToString())
                    oChildData.SetProperty("U_Z_SchEmpID", odr("SID").ToString())
                    oChildData.SetProperty("U_Z_InterviewDate", odr("SDate").ToString())
                    oChildData.SetProperty("U_Z_InterviwerID", odr("SID").ToString())
                    oChildData.SetProperty("U_Z_Status", "-")
                    If odr("STime").ToString <> "" Then
                        Try
                            SchTime = odr("STime")

                            'Dim strqry As String = "Update [@Z_HR_OHEM2] set U_Z_ScTime=" & SchTime & " where DocEntry=" & DocNo & " and U_Z_HRAppID='" & strApplicantID & "' and LineId=" & odr("LineNo").ToString & ""
                            'oTemp.DoQuery(strqry)
                            ''

                            'oChildData.SetProperty("U_Z_ScTime", odr("STime").Value.Substring(0, 1) + ":" + odr("STime").Value.Length.SubString(1, 2))
                            'oChildData.SetProperty("U_Z_ScTime", Convert.ToInt16(odr("STime").ToString.Replace(":", "").ToString))
                        Catch ex As Exception

                        End Try
                    End If
                    Dim st As String = odr("STime").ToString()
                    ' oChildData.SetProperty("U_Z_ScTime", CInt(st))
                ElseIf iFunction = 2 Then
                    oChildData.SetProperty("U_Z_HRAppID", strApplicantID)
                    oChildData.SetProperty("U_Z_InterviewDate", odr("IDate").ToString())
                    oChildData.SetProperty("U_Z_InterviwerID", odr("IEmpID").ToString())
                    oChildData.SetProperty("U_Z_Status", odr("Status").ToString())
                    oChildData.SetProperty("U_Z_InterviewStatus", odr("IStatus").ToString())
                    oChildData.SetProperty("U_Z_Rating", odr("Rating").ToString())
                    oChildData.SetProperty("U_Z_FileName", odr("Attach").ToString())
                    oChildData.SetProperty("U_Z_Comments", odr("Comment").ToString())
                    oChildData.SetProperty("U_Z_RatPer", odr("RatngPer").ToString())
                End If
            Next
            oGenService.Update(oGenData)

            'For i As Integer = 0 To oDT.Rows.Count - 1
            '    Dim odr As DataRow = oDT.Rows(i)
            '    If i > oGenDataCollection.Count - 1 Then
            '        oChildData = oGenDataCollection.Add()
            '    Else
            '        oChildData = oGenDataCollection.Item(i)
            '    End If
            '    If iFunction = 1 Then
            '        If odr("STime").ToString <> "" Then
            '            Try
            '                SchTime = odr("STime")
            '                Dim strqry As String = "Update [@Z_HR_OHEM2] set U_Z_ScTime=" & CInt(SchTime) & " where DocEntry=" & DocNo & " and U_Z_HRAppID='" & strApplicantID & "' and LineId=" & odr("LineNo").ToString & ""
            '                oTemp.DoQuery(strqry)
            '            Catch ex As Exception
            '            End Try
            '        End If
            '    End If
            'Next

           

            'Time Stamp
            Dim oRec As SAPbobsCOM.Recordset
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRec.DoQuery("Select U_Z_HRAppID from [@Z_HR_OHEM1] where DocEntry='" & DocNo & "'")
            If Not oRec.EoF Then
                strAPPID = oRec.Fields.Item(0).Value
                oApplication.Utilities.UpdateApplicantTimeStamp(strAPPID, "LU")
            End If

            'Update Applicatant Status To Interview
            sQuery = "Update [@Z_HR_OCRAPP] Set U_Z_Status = 'I' Where DocEntry = '" & strAPPID & "'"
            oRec.DoQuery(sQuery)

            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                If iFunction = 2 Then
                    For i As Integer = 0 To oDT.Rows.Count - 1
                        Dim odr As DataRow = oDT.Rows(i)
                        'Dim oRec As SAPbobsCOM.Recordset
                        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        Dim strQry = "Select AttachPath From OADP"
                        oRec.DoQuery(strQry)
                        Dim SPath As String = odr("Attach").ToString()
                        If SPath = "" Then
                        Else
                            Dim DPath As String = ""
                            If Not oRec.EoF Then
                                DPath = oRec.Fields.Item("AttachPath").Value.ToString()
                            End If
                            If Not Directory.Exists(DPath) Then
                                Directory.CreateDirectory(DPath)
                            End If
                            Dim file = New FileInfo(SPath)
                            Dim Filename As String = Path.GetFileName(SPath)
                            Dim SavePath As String = Path.Combine(DPath, Filename)
                            If System.IO.File.Exists(SavePath) Then
                            Else
                                file.CopyTo(Path.Combine(DPath, file.Name), True)
                            End If
                        End If
                    Next
                ElseIf iFunction = 1 Then
                    For i As Integer = 0 To oDT.Rows.Count - 1
                        Dim odr As DataRow = oDT.Rows(i)

                        If odr("STime").ToString <> "" Then
                            Try
                                SchTime = odr("STime")
                                Dim strqry As String = "Update [@Z_HR_OHEM2] set U_Z_ScTime=" & CInt(SchTime) & " where DocEntry=" & DocNo & " and U_Z_HRAppID='" & strApplicantID & "' and LineId=" & odr("LineNo").ToString & ""
                                oTemp.DoQuery(strqry)
                            Catch ex As Exception
                            End Try
                        End If
                    Next
                End If
            End If
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            End If
            Return False
        End Try
    End Function
#End Region

#Region "HOD Status & Comments Update"
    Private Sub HOD_SC_Update(ByVal DocNo As String, ByVal Status As String, ByVal Comments As String, ByVal strRej As String)
        Dim strAPPID As String
        Dim oRS As SAPbobsCOM.Recordset
        oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim strQuery As String = "Update [@Z_HR_OHEM1] set U_Z_IPHODSta = '" & Status & "',U_Z_IPHODRmks='" & Comments & "' Where DocEntry=" & DocNo & ""
        oRS.DoQuery(strQuery)

        'Time Stamp
        oRS.DoQuery("Select U_Z_HRAppID from [@Z_HR_OHEM1] where DocEntry='" & DocNo & "'")
        If Not oRS.EoF Then
            strAPPID = oRS.Fields.Item(0).Value
            oApplication.Utilities.UpdateApplicantTimeStamp(strAPPID, "FL")
        End If

        If Status = "S" Then
            'Update Applicatant Status To Interview 1st Approval
            sQuery = "Update [@Z_HR_OCRAPP] Set U_Z_Status = 'D' Where DocEntry = '" & strAPPID & "'"
            oRS.DoQuery(sQuery)
        ElseIf Status = "R" Then
            sQuery = "Update [@Z_HR_OHEM1] set U_Z_RejRsn = '" & strRej & "',U_Z_IPHODRmks='" & Comments & "' Where DocEntry=" & DocNo & ""
            oRS.DoQuery(strQuery)

            sQuery = "Update [@Z_HR_OCRAPP] set U_Z_RejResn = '" & strRej & "',U_Z_RejCom='" & Comments & "' Where DocEntry=" & DocNo & ""
            oRS.DoQuery(strQuery)
        End If

        oApplication.Utilities.Message("Document Update Sucessfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
    End Sub
#End Region

#Region "HR Status & Comments Update"
    Private Sub HR_SC_Update(ByVal DocNo As String, ByVal Status As String, ByVal Comments As String, ByVal strRej As String)
        Dim strAPPID As String
        Dim oRS As SAPbobsCOM.Recordset
        oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim strQuery As String = "Update [@Z_HR_OHEM1] set U_Z_IPHRSta = '" & Status & "',U_Z_IPHRRmks='" & Comments & "' Where DocEntry=" & DocNo & ""
        oRS.DoQuery(strQuery)

        'Candidate Job Offered
        If Status = "S" Then
            strQuery = "Update [@Z_HR_OHEM1] set U_Z_IntervStatus = 'A' where DocEntry = '" & DocNo & "'"
            oRS.DoQuery(strQuery)

            oRS.DoQuery("Select U_Z_HRAppID from [@Z_HR_OHEM1] where DocEntry='" & DocNo & "'")
            strAPPID = oRS.Fields.Item(0).Value

            strQuery = "Update [@Z_HR_OCRAPP] set U_Z_Status = 'M' where DocEntry = '" & strAPPID & "'"
            oRS.DoQuery(strQuery)
        ElseIf (Status = "R") Then
            strQuery = "Update [@Z_HR_OHEM1] set U_Z_APPlStatus='R' , U_Z_IntervStatus = 'R',U_Z_Finished = 'Y' where DocEntry = '" & DocNo & "'"
            oRS.DoQuery(strQuery)

            oRS.DoQuery("Select U_Z_HRAppID from [@Z_HR_OHEM1] where DocEntry='" & DocNo & "'")
            strAPPID = oRS.Fields.Item(0).Value

            sQuery = "Update [@Z_HR_OHEM1] set U_Z_RejRsn = '" & strRej & "',U_Z_IPHODRmks='" & Comments & "' Where DocEntry=" & DocNo & ""
            oRS.DoQuery(strQuery)

            strQuery = "Update [@Z_HR_OCRAPP] set U_Z_Status = 'R',U_Z_RejResn = '" & strRej & "',U_Z_RejCom = '" & Comments & "' where DocEntry = '" & strAPPID & "'"
            oRS.DoQuery(strQuery)
        End If

        'Time Stamp
        oRS.DoQuery("Select U_Z_HRAppID from [@Z_HR_OHEM1] where DocEntry='" & DocNo & "'")
        If Not oRS.EoF Then
            strAPPID = oRS.Fields.Item(0).Value
            oApplication.Utilities.UpdateApplicantTimeStamp(strAPPID, "HR")
        End If

        oApplication.Utilities.Message("Document Updated Sucessfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
    End Sub
#End Region

#Region "FileOpen"
    Private Sub FileOpen()
        Dim mythr As New System.Threading.Thread(AddressOf ShowFileDialog)
        mythr.SetApartmentState(Threading.ApartmentState.STA)
        mythr.Start()
        mythr.Join()
    End Sub

    Private Sub ShowFileDialog()
        Dim oDialogBox As New OpenFileDialog
        Dim strMdbFilePath As String
        Dim oProcesses() As Process
        Try
            oProcesses = Process.GetProcessesByName("SAP Business One")
            If oProcesses.Length <> 0 Then
                For i As Integer = 0 To oProcesses.Length - 1
                    Dim MyWindow As New clsListener.WindowWrapper(oProcesses(i).MainWindowHandle)
                    If oDialogBox.ShowDialog(MyWindow) = DialogResult.OK Then
                        strMdbFilePath = oDialogBox.FileName
                        strFilepath = oDialogBox.FileName
                    Else
                    End If
                Next
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
        End Try
    End Sub
#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_IPProcessForm Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1" And pVal.ColUID = "App/ID" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcode As String = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    Dim ooBj As New clshrCrApplicants
                                    ooBj.ViewCandidate(strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If


                                If pVal.ItemUID = "1" And pVal.ColUID = "Req No" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcode As String = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    If 1 = 1 Then
                                        Dim objct As New clshrMPRequest
                                        objct.LoadForm1(strcode, "Employment Offer", , , )

                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                        End Select
                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                reDrawScreen(oForm)
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "5" And pVal.ColUID = "Attachment" Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    oGrid.Columns.Item("RowsHeader").Click(pVal.Row)
                                    LoadFiles(oForm)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                oGridDetail = oForm.Items.Item("5").Specific
                                If pVal.ItemUID = "5" And pVal.ColUID = "Interviewer EmpID" Then
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
                                            val1 = oDataTable.GetValue("empID", 0)
                                            Try
                                                oGridDetail.DataTable.Columns.Item("Interviewer EmpID").Cells.Item(pVal.Row).Value = val1
                                            Catch ex As Exception
                                            End Try
                                            oForm.Freeze(False)
                                        End If
                                    Catch ex As Exception
                                        oForm.Freeze(False)
                                    End Try
                                ElseIf pVal.ItemUID = "5" And pVal.ColUID = "Rating" Then
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
                                            val1 = oDataTable.GetValue("U_Z_RateCode", 0)
                                            Dim Val2 As String = oDataTable.GetValue("U_Z_RatePerc", 0)
                                            Try
                                                oGridDetail.DataTable.Columns.Item("Rating").Cells.Item(pVal.Row).Value = val1
                                                oGridDetail.DataTable.Columns.Item("Rating Percentage").Cells.Item(pVal.Row).Value = Val2
                                            Catch ex As Exception

                                            End Try
                                            oForm.Freeze(False)
                                        End If
                                    Catch ex As Exception
                                        oForm.Freeze(False)
                                    End Try
                                ElseIf pVal.ItemUID = "5" And pVal.ColUID = "Scheduler EmpID" Then
                                    Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                    Dim oCFL As SAPbouiCOM.ChooseFromList
                                    Dim val1 As String
                                    Dim Name As String
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
                                            val1 = oDataTable.GetValue("empID", 0)
                                            Name = oDataTable.GetValue("firstName", 0)
                                            Try
                                                oGridDetail.DataTable.Columns.Item("Scheduler EmpID").Cells.Item(pVal.Row).Value = val1
                                                oGridDetail.DataTable.Columns.Item("Scheduler Name").Cells.Item(pVal.Row).Value = Name
                                            Catch ex As Exception
                                            End Try
                                            oForm.Freeze(False)
                                        End If
                                    Catch ex As Exception
                                        oForm.Freeze(False)
                                    End Try
                                ElseIf pVal.ItemUID = "16" Then
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
                                            val1 = oDataTable.GetValue("U_Z_TypeCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "16", val1)
                                            Catch ex As Exception
                                            End Try
                                            oForm.Freeze(False)
                                        End If
                                    Catch ex As Exception
                                        oForm.Freeze(False)
                                    End Try
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                Dim oGCCol1, oGCCol2 As SAPbouiCOM.ComboBoxColumn
                                If pVal.ItemUID = "5" And strFunction = "IPLMU" Then
                                    oGridDetail = oForm.Items.Item("5").Specific
                                    oGCCol1 = oGridDetail.Columns.Item("Status")
                                    oGCCol2 = oGridDetail.Columns.Item("Interview Status")
                                    If pVal.ItemUID = "5" And pVal.ColUID = "Status" Then
                                        If oGCCol1.GetSelectedValue(pVal.Row).Value = "CA" Then
                                            oGCCol2.SetSelectedValue(pVal.Row, oGCCol2.ValidValues.Item("P"))
                                        ElseIf oGCCol1.GetSelectedValue(pVal.Row).Value = "RS" Then
                                            oGCCol2.SetSelectedValue(pVal.Row, oGCCol2.ValidValues.Item("P"))
                                        End If
                                    ElseIf pVal.ItemUID = "5" And pVal.ColUID = "Interview Status" Then
                                        If oGCCol2.GetSelectedValue(pVal.Row).Value = "S" Then
                                            oGCCol1.SetSelectedValue(pVal.Row, oGCCol1.ValidValues.Item("CO"))
                                        ElseIf oGCCol2.GetSelectedValue(pVal.Row).Value = "R" Then
                                            oGCCol1.SetSelectedValue(pVal.Row, oGCCol1.ValidValues.Item("CO"))
                                        End If
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" Then
                                    Select Case strFunction
                                        Case "IPLM"
                                            oGrid = oForm.Items.Item("1").Specific
                                            Dim strDocEntry As String = ""
                                            Dim strAppID As String = ""
                                            Dim strHODStatus As String = ""
                                            If oGrid.Rows.Count > 0 Then
                                                For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                                    If oGrid.Rows.IsSelected(intRow) Then
                                                        oGridDetail = oForm.Items.Item("5").Specific
                                                        strDocEntry = oGrid.DataTable.GetValue("DocEntry", intRow)
                                                        strAppID = oGrid.DataTable.GetValue("App/ID", intRow)
                                                    End If
                                                Next
                                                If oGridDetail.Rows.Count > 0 Then
                                                    Dim dr As DataRow
                                                    oDT_DIAPI = New DataTable()
                                                    oDT_DIAPI.Columns.Add("InType")
                                                    oDT_DIAPI.Columns.Add("SDate")
                                                    oDT_DIAPI.Columns.Add("IDate")
                                                    oDT_DIAPI.Columns.Add("IEmpID")
                                                    oDT_DIAPI.Columns.Add("Status")
                                                    oDT_DIAPI.Columns.Add("IStatus")
                                                    oDT_DIAPI.Columns.Add("Rating")
                                                    oDT_DIAPI.Columns.Add("Attach")
                                                    oDT_DIAPI.Columns.Add("Comment")
                                                    oDT_DIAPI.Columns.Add("SID")
                                                    oDT_DIAPI.Columns.Add("STime")
                                                    oDT_DIAPI.Columns.Add("RatngPer")
                                                    oDT_DIAPI.Columns.Add("LineNo")
                                                    For i As Integer = 0 To oGridDetail.Rows.Count - 1
                                                        If 1 = 1 Then
                                                            dr = oDT_DIAPI.NewRow()
                                                            If strFunction = "IPLM" Then
                                                                If oGridDetail.DataTable.Columns.Item("Interview Type").Cells.Item(i).Value.ToString() <> "-" Then
                                                                    dr.Item("InType") = oGridDetail.DataTable.Columns.Item("Interview Type").Cells.Item(i).Value.ToString()
                                                                    dr.Item("SDate") = oGridDetail.DataTable.Columns.Item("Schedule Date").Cells.Item(i).Value
                                                                    dr.Item("SID") = oGridDetail.DataTable.Columns.Item("Scheduler EmpID").Cells.Item(i).Value
                                                                    dr.Item("STime") = oGridDetail.DataTable.Columns.Item("Schedule Time").Cells.Item(i).Value
                                                                    dr.Item("LineNo") = oGridDetail.DataTable.Columns.Item("LineNo").Cells.Item(i).Value.ToString()
                                                                End If
                                                            Else
                                                                dr.Item("IDate") = oGridDetail.DataTable.Columns.Item("Interview Date").Cells.Item(i).Value
                                                                dr.Item("IEmpID") = oGridDetail.DataTable.Columns.Item("Interviewer EmpID").Cells.Item(i).Value.ToString()
                                                                dr.Item("Status") = oGridDetail.DataTable.Columns.Item("Status").Cells.Item(i).Value.ToString()
                                                                dr.Item("IStatus") = oGridDetail.DataTable.Columns.Item("Interview Status").Cells.Item(i).Value.ToString()
                                                                dr.Item("Rating") = oGridDetail.DataTable.Columns.Item("Rating").Cells.Item(i).Value.ToString()
                                                                dr.Item("Attach") = oGridDetail.DataTable.Columns.Item("Attachment").Cells.Item(i).Value.ToString()
                                                                dr.Item("Comment") = oGridDetail.DataTable.Columns.Item("Comments").Cells.Item(i).Value.ToString()
                                                                dr.Item("RatngPer") = oGridDetail.DataTable.Columns.Item("Rating Percentage").Cells.Item(i).Value.ToString()


                                                            End If
                                                            oDT_DIAPI.Rows.Add(dr)
                                                        End If
                                                    Next
                                                End If
                                            End If

                                            If strDocEntry = "" Or oDT_DIAPI.Rows.Count = 0 Then
                                                If strDocEntry = "" Then
                                                    oApplication.Utilities.Message("Not Records Found", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                                    BubbleEvent = False
                                                End If
                                            Else
                                                If oApplication.SBO_Application.MessageBox("Click Yes to Proceed", , "Yes", "No") = 2 Then
                                                    Exit Sub
                                                Else
                                                    Dim oHash As Hashtable
                                                    oHash = New Hashtable()
                                                    If strFunction = "IPLM" Then
                                                        If validate("1") Then
                                                            If LineManager_DIAPI(strDocEntry, strAppID, oDT_DIAPI, 1, oHash) Then
                                                                oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                                                ' oForm.Close()
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        Case "IPLMU"
                                            oGrid = oForm.Items.Item("1").Specific
                                            Dim strDocEntry As String = ""
                                            Dim strHODStatus As String = ""
                                            Dim strAppID As String = ""
                                            If oGrid.Rows.Count > 0 Then
                                                For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                                    If oGrid.Rows.IsSelected(intRow) Then
                                                        oGridDetail = oForm.Items.Item("5").Specific
                                                        strDocEntry = oGrid.DataTable.GetValue("DocEntry", intRow)
                                                        strAppID = oGrid.DataTable.GetValue("App/ID", intRow)
                                                    End If
                                                Next
                                                If oGridDetail.Rows.Count > 0 Then
                                                    Dim dr As DataRow
                                                    oDT_DIAPI = New DataTable()
                                                    oDT_DIAPI.Columns.Add("InType")
                                                    oDT_DIAPI.Columns.Add("SDate")
                                                    oDT_DIAPI.Columns.Add("IDate")
                                                    oDT_DIAPI.Columns.Add("IEmpID")
                                                    oDT_DIAPI.Columns.Add("Status")
                                                    oDT_DIAPI.Columns.Add("IStatus")
                                                    oDT_DIAPI.Columns.Add("Rating")
                                                    oDT_DIAPI.Columns.Add("Attach")
                                                    oDT_DIAPI.Columns.Add("Comment")
                                                    oDT_DIAPI.Columns.Add("SID")
                                                    oDT_DIAPI.Columns.Add("STime")
                                                    oDT_DIAPI.Columns.Add("RatngPer")
                                                    For i As Integer = 0 To oGridDetail.Rows.Count - 1
                                                        If 1 = 1 Then
                                                            dr = oDT_DIAPI.NewRow()
                                                            If strFunction = "IPLM" Then
                                                                If oGridDetail.DataTable.Columns.Item("Interview Type").Cells.Item(i).Value.ToString() <> "-" Then
                                                                    dr.Item("InType") = oGridDetail.DataTable.Columns.Item("Interview Type").Cells.Item(i).Value.ToString()
                                                                    dr.Item("SDate") = oGridDetail.DataTable.Columns.Item("Schedule Date").Cells.Item(i).Value
                                                                    dr.Item("SID") = oGridDetail.DataTable.Columns.Item("Scheduler EmpID").Cells.Item(i).Value
                                                                    dr.Item("STime") = oGridDetail.DataTable.Columns.Item("Schedule Time").Cells.Item(i).Value
                                                                End If
                                                            Else
                                                                dr.Item("IDate") = oGridDetail.DataTable.Columns.Item("Interview Date").Cells.Item(i).Value
                                                                dr.Item("IEmpID") = oGridDetail.DataTable.Columns.Item("Interviewer EmpID").Cells.Item(i).Value.ToString()
                                                                dr.Item("Status") = oGridDetail.DataTable.Columns.Item("Status").Cells.Item(i).Value.ToString()
                                                                dr.Item("IStatus") = oGridDetail.DataTable.Columns.Item("Interview Status").Cells.Item(i).Value.ToString()
                                                                dr.Item("Rating") = oGridDetail.DataTable.Columns.Item("Rating").Cells.Item(i).Value.ToString()
                                                                dr.Item("Attach") = oGridDetail.DataTable.Columns.Item("Attachment").Cells.Item(i).Value.ToString()
                                                                dr.Item("Comment") = oGridDetail.DataTable.Columns.Item("Comments").Cells.Item(i).Value.ToString()
                                                                dr.Item("RatngPer") = oGridDetail.DataTable.Columns.Item("Rating Percentage").Cells.Item(i).Value.ToString()

                                                            End If
                                                            oDT_DIAPI.Rows.Add(dr)
                                                        End If
                                                    Next
                                                End If
                                            End If

                                            If strDocEntry = "" Or oDT_DIAPI.Rows.Count = 0 Then
                                                If strDocEntry = "" Then
                                                    oApplication.Utilities.Message("Not Records Found", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                                    BubbleEvent = False
                                                End If
                                            Else
                                                If oApplication.SBO_Application.MessageBox("Click Yes to Proceed", , "Yes", "No") = 2 Then
                                                    Exit Sub
                                                Else
                                                    Dim oHash As Hashtable
                                                    oHash = New Hashtable()
                                                    If strFunction = "IPLMU" Then
                                                        Dim strStatus As String = "S"
                                                        Dim strComment As String = oApplication.Utilities.getEdittextvalue(oForm, "9")
                                                        oHash.Add("Status", strStatus)
                                                        oHash.Add("RMrk", strComment)
                                                        If validate("2") Then
                                                            If LineManager_DIAPI(strDocEntry, strAppID, oDT_DIAPI, 2, oHash) Then
                                                                oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                                                ' oForm.Close()
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            End If
                                    End Select
                                ElseIf pVal.ItemUID = "13" Then
                                    Select Case strFunction
                                        Case "IPHR"
                                            oGrid = oForm.Items.Item("1").Specific
                                            If oGrid.Rows.Count > 0 Then
                                                If oApplication.SBO_Application.MessageBox("Click Yes to Proceed", , "Yes", "No") = 2 Then
                                                    Exit Sub
                                                Else
                                                    For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                                        If oGrid.Rows.IsSelected(intRow) Then
                                                            Dim strDocEntry As String = oGrid.DataTable.GetValue("DocEntry", intRow)
                                                            Dim strStatus As String = "S"
                                                            Dim strComment As String = oApplication.Utilities.getEdittextvalue(oForm, "9")
                                                            HR_SC_Update(strDocEntry, strStatus, strComment, "")
                                                        End If
                                                    Next
                                                    ' oForm.Close()
                                                End If
                                            End If
                                        Case "IPHOD"
                                            oGrid = oForm.Items.Item("1").Specific
                                            If oGrid.Rows.Count > 0 Then
                                                If oApplication.SBO_Application.MessageBox("Click Yes to Proceed", , "Yes", "No") = 2 Then
                                                    Exit Sub
                                                Else
                                                    For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                                        If oGrid.Rows.IsSelected(intRow) Then
                                                            Dim strDocEntry As String = oGrid.DataTable.GetValue("DocEntry", intRow)
                                                            Dim strStatus As String = "S"
                                                            Dim strComment As String = oApplication.Utilities.getEdittextvalue(oForm, "9")
                                                            HOD_SC_Update(strDocEntry, strStatus, strComment, "")
                                                        End If
                                                    Next
                                                    ' oForm.Close()
                                                End If
                                            End If
                                    End Select
                                ElseIf pVal.ItemUID = "14" Then
                                    Select Case strFunction
                                        Case "IPHR"
                                            oGrid = oForm.Items.Item("1").Specific
                                            If oGrid.Rows.Count > 0 Then
                                                For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                                    If oGrid.Rows.IsSelected(intRow) Then
                                                        Dim strDocEntry As String = oGrid.DataTable.GetValue("DocEntry", intRow)
                                                        '  oCombo = oForm.Items.Item("7").Specific
                                                        Dim strStatus As String = "R"

                                                        Dim strReje As String = oApplication.Utilities.getEdittextvalue(oForm, "16")
                                                        If strReje.Length = 0 Then
                                                            oApplication.Utilities.Message("Please Select a Rejection Reason", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                                            Return
                                                        Else
                                                            Dim strComment As String = oApplication.Utilities.getEdittextvalue(oForm, "9")
                                                            HR_SC_Update(strDocEntry, strStatus, strComment, strReje)
                                                            '  oForm.Close()
                                                        End If
                                                    End If
                                                Next
                                            End If
                                        Case "IPHOD"
                                            oGrid = oForm.Items.Item("1").Specific
                                            If oGrid.Rows.Count > 0 Then
                                                For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                                    If oGrid.Rows.IsSelected(intRow) Then
                                                        Dim strDocEntry As String = oGrid.DataTable.GetValue("DocEntry", intRow)
                                                        Dim strStatus As String = "R"
                                                        Dim strReje As String = oApplication.Utilities.getEdittextvalue(oForm, "16")
                                                        If strReje.Length = 0 Then
                                                            oApplication.Utilities.Message("Please Select a Rejection Reason", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                                            Return
                                                        Else
                                                            Dim strComment As String = oApplication.Utilities.getEdittextvalue(oForm, "9")
                                                            HOD_SC_Update(strDocEntry, strStatus, strComment, strReje)
                                                            '  oForm.Close()
                                                        End If
                                                    End If
                                                Next
                                            End If
                                    End Select
                                ElseIf pVal.ItemUID = "1" And pVal.ColUID = "RowsHeader" And pVal.Row <> -1 Then
                                    oGrid = oForm.Items.Item("1").Specific
                                    If oGrid.Rows.Count > 0 Then
                                        Dim DocNo As Integer = Convert.ToInt32(oGrid.DataTable.GetValue("DocEntry", pVal.Row))
                                        Dim LMStatus As String = oGrid.DataTable.GetValue("HOD Status", pVal.Row)
                                        Dim HODStatus As String = oGrid.DataTable.GetValue("HOD Status", pVal.Row)
                                        Dim HRStatus As String = oGrid.DataTable.GetValue("HR Status", pVal.Row)
                                        Dim strRsta As String = oGrid.DataTable.GetValue("Request Status", pVal.Row)
                                        Select Case strFunction
                                            Case "IPLM"
                                                oForm.Freeze(True)
                                                ForLineManager(DocNo, 1, LMStatus, HODStatus, HRStatus)
                                                oForm.Freeze(False)
                                            Case "IPLMU"
                                                oForm.Freeze(True)
                                                ForLineManager(DocNo, 2, LMStatus, HODStatus, HRStatus)
                                                oForm.Freeze(False)
                                            Case "IPHOD"
                                                oForm.Freeze(True)
                                                ForHOD(DocNo, HODStatus, HRStatus)
                                                oForm.Freeze(False)
                                            Case "IPHR"
                                                oForm.Freeze(True)
                                                ForHR(DocNo, HRStatus)
                                                oForm.Freeze(False)
                                        End Select

                                        If strRsta = "C" Or strRsta = "L" Then
                                            oForm.Items.Item("3").Enabled = False
                                            oForm.Items.Item("11").Enabled = False
                                            oForm.Items.Item("12").Enabled = False
                                            oForm.Items.Item("13").Enabled = False
                                            oForm.Items.Item("14").Enabled = False
                                        End If
                                    End If
                                ElseIf pVal.ItemUID = "11" Then
                                   
                                    ' ocomboCol = oGridDetail.Columns.Item("Interview Type")
                                    Try
                                        If 1 = 1 Then 'ocomboCol.GetSelectedValue(oGridDetail.DataTable.Rows.Count - 1).Description <> "" Then
                                            oGrid = oForm.Items.Item("1").Specific
                                            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                                If oGrid.Rows.IsSelected(intRow) Then
                                                    oGridDetail = oForm.Items.Item("5").Specific
                                                    strDocEntry = oGrid.DataTable.GetValue("DocEntry", intRow)

                                                    Dim oCode As String = oApplication.Utilities.getMaxCode_lineNo("@Z_HR_OHEM2", "LineID", CInt(strDocEntry))
                                                    oGridDetail.DataTable.Rows.Add(1)
                                                    oGridDetail.DataTable.SetValue("LineNo", oGridDetail.DataTable.Rows.Count - 1, CInt(oCode))
                                                    Exit Sub
                                                End If
                                            Next
                                        End If
                                    Catch ex As Exception
                                        oGridDetail.DataTable.Rows.Add(1)
                                    End Try

                                ElseIf pVal.ItemUID = "12" Then
                                    oGridDetail = oForm.Items.Item("5").Specific
                                    oGrid = oForm.Items.Item("1").Specific
                                    Dim strDocEntry As String = ""
                                    Dim strAppID As String = ""
                                    Dim strHODStatus As String = ""
                                    If oGrid.Rows.Count > 0 Then
                                        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                            If oGrid.Rows.IsSelected(intRow) Then
                                                oGridDetail = oForm.Items.Item("5").Specific
                                                strDocEntry = oGrid.DataTable.GetValue("DocEntry", intRow)
                                            End If
                                        Next
                                        If oGridDetail.Rows.Count > 0 Then
                                            If oApplication.SBO_Application.MessageBox("Do you want to delete the interview schedule?", , "Yes", "No") = 2 Then
                                                Exit Sub
                                            End If
                                            For intRow As Integer = 0 To oGridDetail.DataTable.Rows.Count - 1
                                                Dim strLineid As Integer
                                                strLineid = oGridDetail.DataTable.GetValue("LineNo", intRow)
                                                If oGridDetail.Rows.IsSelected(intRow) Then
                                                    ocomboCol = oGridDetail.Columns.Item("Status")
                                                    If ocomboCol.GetSelectedValue(intRow).Description = "Pending" Then
                                                        Dim otest As SAPbobsCOM.Recordset
                                                        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                                        otest.DoQuery("Delete from ""@Z_HR_OHEM2"" where ""DocEntry""=" & strDocEntry & " and ""LineId""=" & strLineid & "")
                                                        oGridDetail.DataTable.Rows.Remove(intRow)
                                                    Else
                                                        oApplication.Utilities.Message("You can only delete the pending interview Schedule only", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                                        Exit Sub
                                                    End If

                                                    Exit Sub
                                                End If
                                            Next
                                        End If
                                    End If
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_DOUBLE_CLICK
                                    If pVal.ItemUID = "5" And pVal.ColUID = "Attachment" And strFunction = "IPLMU" Then
                                        oGridDetail = oForm.Items.Item("5").Specific
                                        Dim strPath As String = oGridDetail.DataTable.Columns.Item("Attachment").Cells.Item(pVal.Row).Value.ToString()
                                        FileOpen()
                                        If strFilepath = "" Then
                                            oApplication.Utilities.Message("Please Select a File", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            BubbleEvent = False
                                        Else
                                            oGridDetail.DataTable.Columns.Item("Attachment").Cells.Item(pVal.Row).Value = strFilepath
                                        End If
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

            End Select
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
#End Region

    Private Function validate(ByVal strType As String) As Boolean
        Dim _retVal As Boolean = True

        If strType = "1" Then
            For Each dr As DataRow In oDT_DIAPI.Rows
                If Not IsDBNull(dr(0)) Then
                    If dr(0) <> "-" And String.IsNullOrEmpty(dr(1)) Then
                        oApplication.Utilities.Message("Enter Schedule Date For Selected Interview Type...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        _retVal = False
                        Exit For
                    ElseIf dr(9) = "" Or String.IsNullOrEmpty(dr(9)) Then
                        oApplication.Utilities.Message("Enter Scheduler EmpID For Selected Interview Type...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        _retVal = False
                        Exit For
                        'ElseIf dr(10) = "" Or String.IsNullOrEmpty(dr(10)) Then
                        ' oApplication.Utilities.Message("Enter Schedule Time For Selected Interview Type...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        ' _retVal = False
                        ' Exit For
                    End If
                End If
            Next
        ElseIf strType = "2" Then
            For Each dr As DataRow In oDT_DIAPI.Rows
                If Not IsDBNull(dr(2)) Then
                    If dr(2) <> "" And (String.IsNullOrEmpty(dr(3))) Then ' Or String.IsNullOrEmpty(dr(6))) Then
                        'oApplication.Utilities.Message("Select Interviewer EmpId and Rating for selected interview date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        '_retVal = False
                        'Exit For
                    ElseIf dr(2) <> "" And (dr(4) = "CO" And dr(5) = "P") Then
                        oApplication.Utilities.Message("Select Interview Status...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        _retVal = False
                        Exit For
                        'ElseIf dr(2) <> "" And (dr(5) <> "P") And dr(6) = "" Then
                        '    oApplication.Utilities.Message("Select Rating ...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        '    _retVal = False
                        '    Exit For
                    End If
                ElseIf IsDBNull(dr(2)) Then
                    'If (dr(4) <> "-" Or dr(5) <> "P") Then
                    '    oApplication.Utilities.Message("Select Interview Date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    '    _retVal = False
                    'End If
                End If
            Next
        End If

        Return _retVal
    End Function

    Private Sub reDrawScreen(ByVal sboForm As SAPbouiCOM.Form)
        Try
            sboForm.Freeze(True)

            Dim intTop As Int16
            sboForm.Items.Item("1").Height = (sboForm.Height / 2) - 80
            sboForm.Items.Item("1").Width = (sboForm.Width) - 20

            intTop = sboForm.Items.Item("1").Top + sboForm.Items.Item("1").Height + 5
            sboForm.Items.Item("4").Top = intTop
            sboForm.Items.Item("4").TextStyle = 7


            intTop = sboForm.Items.Item("4").Top + sboForm.Items.Item("4").Height + 5
            sboForm.Items.Item("5").Top = intTop
            sboForm.Items.Item("5").Width = (sboForm.Width) - 10


            If sboForm.Title = "Interview Scheduling" Then
                sboForm.Items.Item("5").Height = (sboForm.Height / 2) - 25
            ElseIf sboForm.Title = "Interview Summary" Then
                sboForm.Items.Item("5").Height = (sboForm.Height / 2) - 75
            Else
                sboForm.Items.Item("5").Height = (sboForm.Height / 2) - 100
            End If

            oGrid = sboForm.Items.Item("1").Specific
            oGridDetail = oForm.Items.Item("5").Specific

            oGrid.AutoResizeColumns()
            oGridDetail.AutoResizeColumns()

            sboForm.Freeze(False)
        Catch ex As Exception
            sboForm.Freeze(False)
        End Try
    End Sub

End Class
