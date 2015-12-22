Public Class clshrEmpLifePosting
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox, oCombobox1 As SAPbouiCOM.ComboBox
    Private ocombo As SAPbouiCOM.ComboBoxColumn
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
    Private Sub LoadForm(ByVal oForm As SAPbouiCOM.Form)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_EmpLifePost) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_HR_EmpLifePost, frm_hr_EmpLifePost)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.PaneLevel = 1
        FillDepartment(oForm)
        oCombobox = oForm.Items.Item("36").Specific
        oCombobox.ValidValues.Add("", "")
        oCombobox.ValidValues.Add("P", "Promotion")
        oCombobox.ValidValues.Add("C", "Position Change")
        oForm.Items.Item("36").DisplayDesc = True
        oForm.Freeze(False)
    End Sub
    Private Sub FillDepartment(ByVal sform As SAPbouiCOM.Form)
        Dim strqry As String
        oCombobox = sform.Items.Item("34").Specific
        Dim oSlpRS, oslpRec As SAPbobsCOM.Recordset
        oslpRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim oUserID As String = oApplication.Company.UserSignature

        Dim stremp As String = oApplication.Utilities.getEmpIDforMangersApp(oUserID)
        If stremp = "" Then
            stremp = "'999999'"
        End If
        strqry = "select ""Code"" from OUDP where ""U_Z_HOD"" in (" & stremp & ")"
        oslpRec.DoQuery(strqry)
        If oslpRec.RecordCount > 0 Then
            For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
                oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
            Next
            oCombobox.ValidValues.Add("", "")
            oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strqry = "Select ""Code"",""Remarks""  from OUDP where ""Code"" in (Select ""Code"" from OUDP where ""U_Z_HOD"" in (" & stremp & "))  order by ""Code"""
            oSlpRS.DoQuery(strqry)
            For intRow As Integer = 0 To oSlpRS.RecordCount - 1
                Try
                    oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
                Catch ex As Exception
                End Try
                oSlpRS.MoveNext()
            Next
        Else
            oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strqry = "Select ""Code"",""Remarks""  from OUDP   order by ""Code"""
            oSlpRS.DoQuery(strqry)
            For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
                oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
            Next
            oCombobox.ValidValues.Add("", "")
            For intRow As Integer = 0 To oSlpRS.RecordCount - 1
                Try
                    oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
                Catch ex As Exception
                End Try
                oSlpRS.MoveNext()
            Next
        End If
        sform.Items.Item("34").DisplayDesc = True
    End Sub
    Public Function getdepartment(ByVal aCode As String) As String
        Dim oSlpRS As SAPbobsCOM.Recordset
        Dim strdept As String
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strSQL = "Select ""Code"",""Remarks""  from OUDP where ""Code"" in (Select ""Code"" from OUDP where ""U_Z_HOD"" in (" & aCode & "))  order by ""Code"""
        oSlpRS.DoQuery(strSQL)
        If oSlpRS.RecordCount > 0 Then
            For intRow As Integer = 0 To oSlpRS.RecordCount - 1
                If strdept = "" Then
                    strdept = "'" & oSlpRS.Fields.Item(0).Value & "'"
                Else
                    strdept = strdept & " ,'" & oSlpRS.Fields.Item(0).Value & "'"
                End If
                oSlpRS.MoveNext()
            Next
            Return strdept
        Else
            Return ""
        End If
    End Function
    Private Sub Selectall(ByVal aForm As SAPbouiCOM.Form, ByVal blnValue As Boolean)
        Dim ocheckboxcolumn As SAPbouiCOM.CheckBoxColumn
        Dim ovalue As SAPbouiCOM.ValidValue
        oGrid = aForm.Items.Item("10").Specific
        aForm.Freeze(True)
        For introw As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            ocheckboxcolumn = oGrid.Columns.Item(0)
            ocheckboxcolumn.Check(introw, blnValue)
        Next
        aForm.Freeze(False)
    End Sub
    Private Sub NewGridbind(ByVal aForm As SAPbouiCOM.Form, ByVal strDept As String, ByVal strType As String)
        Dim strSql As String
        oGrid = aForm.Items.Item("10").Specific
        Dim oUserID As String = oApplication.Company.UserSignature
        Dim stremp As String = oApplication.Utilities.getEmpIDforMangersApp(oUserID)
        strDept = getdepartment(stremp)
        If strType = "C" Then
            If strType = "C" And strDept = "" Then
                strSql = "	select '',""Code"",""U_Z_EmpId"",""U_Z_FirstName"",""U_Z_DeptName"",""U_Z_PosCode"",""U_Z_PosName"",""U_Z_JobName"",""U_Z_OrgCode"",""U_Z_OrgName"","
                strSql = strSql & """U_Z_NewPosDate"",""U_Z_EffFromdt"",""U_Z_EffTodt"",""U_Z_AppStatus"" from ""@Z_HR_HEM4"" where ""U_Z_Posting""='N' and ""U_Z_AppStatus""='A'" ' and  ""U_Z_EmpId"" in (" & stremp & ")"
            ElseIf strType = "C" And strDept <> "" Then
                strSql = "	select '',""Code"",""U_Z_EmpId"",""U_Z_FirstName"",""U_Z_DeptName"",""U_Z_PosCode"",""U_Z_PosName"",""U_Z_JobName"",""U_Z_OrgCode"",""U_Z_OrgName"","
                strSql = strSql & """U_Z_NewPosDate"",""U_Z_EffFromdt"",""U_Z_EffTodt"",""U_Z_AppStatus"" from ""@Z_HR_HEM4"" where ""U_Z_Posting""='N' and ""U_Z_AppStatus""='A' and  ""U_Z_Dept"" in (" & strDept & ")" ' and  ""U_Z_EmpId"" in (" & stremp & ")"
            End If
            oGrid.DataTable.ExecuteQuery(strSql)
            oGrid.Columns.Item(0).TitleObject.Caption = "Select"
            oGrid.Columns.Item(0).Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
            oGrid.Columns.Item(0).Editable = True
            oGrid.Columns.Item("Code").TitleObject.Caption = "Code"
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee ID"
            oGrid.Columns.Item("U_Z_EmpId").Editable = False
            oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
            oEditTextColumn.LinkedObjectType = 171
            oGrid.Columns.Item("U_Z_FirstName").TitleObject.Caption = "Employee Name"
            oGrid.Columns.Item("U_Z_FirstName").Editable = False
            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
            oGrid.Columns.Item("U_Z_DeptName").Editable = False
            oGrid.Columns.Item("U_Z_PosCode").TitleObject.Caption = "Position Code"
            oGrid.Columns.Item("U_Z_PosCode").Visible = False
            oGrid.Columns.Item("U_Z_OrgCode").TitleObject.Caption = "Organization Code"
            oGrid.Columns.Item("U_Z_OrgCode").Visible = False
            oGrid.Columns.Item("U_Z_OrgName").TitleObject.Caption = "Organization Name"
            oGrid.Columns.Item("U_Z_OrgName").Editable = False
            oGrid.Columns.Item("U_Z_JobName").TitleObject.Caption = "Job Name"
            oGrid.Columns.Item("U_Z_JobName").Editable = False
            oGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position Name"
            oGrid.Columns.Item("U_Z_PosName").Editable = False
            oGrid.Columns.Item("U_Z_NewPosDate").TitleObject.Caption = "Position Change Date"
            oGrid.Columns.Item("U_Z_NewPosDate").Visible = False
            oGrid.Columns.Item("U_Z_EffFromdt").TitleObject.Caption = "Effective From Date"
            oGrid.Columns.Item("U_Z_EffFromdt").Editable = False
            oGrid.Columns.Item("U_Z_EffTodt").TitleObject.Caption = "Effective To Date"
            oGrid.Columns.Item("U_Z_EffTodt").Editable = False
            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_AppStatus")
            ocombo.ValidValues.Add("P", "Pending")
            ocombo.ValidValues.Add("A", "Approved")
            ocombo.ValidValues.Add("R", "Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_AppStatus").Editable = False
        ElseIf strType = "P" Then
            If strDept = "" Then
                ''strDept = getdepartment(stremp)
                strSql = "	select '',""Code"",""U_Z_EmpId"",""U_Z_FirstName"",""U_Z_DeptName"",""U_Z_PosCode"",""U_Z_PosName"",""U_Z_JobName"",""U_Z_OrgName"","
                strSql = strSql & """U_Z_ProJoinDate"",""U_Z_IncAmount"",""U_Z_EffFromdt"",""U_Z_EffTodt"",""U_Z_AppStatus"" from ""@Z_HR_HEM2"" where ""U_Z_Posting""='N' and ""U_Z_AppStatus""='A'" ' and  ""U_Z_EmpId"" in (" & stremp & ")"
            ElseIf strDept <> "" Then
                strSql = "	select '',""Code"",""U_Z_EmpId"",""U_Z_FirstName"",""U_Z_DeptName"",""U_Z_PosCode"",""U_Z_PosName"",""U_Z_JobName"",""U_Z_OrgName"","
                strSql = strSql & """U_Z_ProJoinDate"",""U_Z_IncAmount"",""U_Z_EffFromdt"",""U_Z_EffTodt"",""U_Z_AppStatus"" from ""@Z_HR_HEM2"" where ""U_Z_Posting""='N' and ""U_Z_AppStatus""='A' and  ""U_Z_Dept"" in (" & strDept & ")" ' and  ""U_Z_EmpId"" in (" & stremp & ")"
            End If
            oGrid.DataTable.ExecuteQuery(strSql)
            oGrid.Columns.Item(0).TitleObject.Caption = "Select"
            oGrid.Columns.Item(0).Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
            oGrid.Columns.Item(0).Editable = True
            oGrid.Columns.Item("Code").TitleObject.Caption = "Code"
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee ID"
            oGrid.Columns.Item("U_Z_EmpId").Editable = False
            oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
            oEditTextColumn.LinkedObjectType = 171
            oGrid.Columns.Item("U_Z_FirstName").TitleObject.Caption = "Employee Name"
            oGrid.Columns.Item("U_Z_FirstName").Editable = False
            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
            oGrid.Columns.Item("U_Z_DeptName").Editable = False
            oGrid.Columns.Item("U_Z_OrgName").TitleObject.Caption = "Organization Name"
            oGrid.Columns.Item("U_Z_OrgName").Editable = False
            oGrid.Columns.Item("U_Z_PosCode").TitleObject.Caption = "Position Code"
            oGrid.Columns.Item("U_Z_PosCode").Visible = False
            oGrid.Columns.Item("U_Z_JobName").TitleObject.Caption = "Job Name"
            oGrid.Columns.Item("U_Z_JobName").Editable = False
            oGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position Name"
            oGrid.Columns.Item("U_Z_PosName").Editable = False
            oGrid.Columns.Item("U_Z_ProJoinDate").TitleObject.Caption = "Promotion Date"
            oGrid.Columns.Item("U_Z_ProJoinDate").Editable = False
            oGrid.Columns.Item("U_Z_IncAmount").TitleObject.Caption = "Increment Amount"
            oGrid.Columns.Item("U_Z_IncAmount").Editable = False
            oGrid.Columns.Item("U_Z_EffFromdt").TitleObject.Caption = "Effective From Date"
            oGrid.Columns.Item("U_Z_EffFromdt").Editable = False
            oGrid.Columns.Item("U_Z_EffTodt").TitleObject.Caption = "Effective To Date"
            oGrid.Columns.Item("U_Z_EffTodt").Editable = False
            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_AppStatus")
            ocombo.ValidValues.Add("P", "Pending")
            ocombo.ValidValues.Add("A", "Approved")
            ocombo.ValidValues.Add("R", "Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_AppStatus").Editable = False
        End If
       
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oApplication.Utilities.AssignRowNo(oGrid, aForm)
    End Sub
    Private Sub SummaryGridbind(ByVal aForm As SAPbouiCOM.Form, ByVal strDept As String, ByVal strType As String)
        Dim strSql As String
        oGrid = aForm.Items.Item("25").Specific
        Dim oUserID As String = oApplication.Company.UserSignature
        Dim stremp As String = oApplication.Utilities.getEmpIDforMangersApp(oUserID)
        strDept = getdepartment(stremp)
        If strType = "C" Then
            If strDept = "" Then
                strSql = "	select ""Code"",""U_Z_EmpId"",""U_Z_FirstName"",""U_Z_DeptName"",""U_Z_PosName"",""U_Z_JobName"",""U_Z_OrgCode"",""U_Z_OrgName"","
                strSql = strSql & """U_Z_NewPosDate"",""U_Z_EffFromdt"",""U_Z_EffTodt"",""U_Z_AppStatus"",""U_Z_Posting"" from ""@Z_HR_HEM4"" where  ""U_Z_AppStatus""='A'" ' and  ""U_Z_EmpId"" in (" & stremp & ")"
            ElseIf strDept <> "" Then
                strSql = "	select ""Code"",""U_Z_EmpId"",""U_Z_FirstName"",""U_Z_DeptName"",""U_Z_PosName"",""U_Z_JobName"",""U_Z_OrgCode"",""U_Z_OrgName"","
                strSql = strSql & """U_Z_NewPosDate"",""U_Z_EffFromdt"",""U_Z_EffTodt"",""U_Z_AppStatus"",""U_Z_Posting"" from ""@Z_HR_HEM4"" where ""U_Z_AppStatus""='A' and  ""U_Z_Dept"" in (" & strDept & ")" ' and  ""U_Z_EmpId"" in (" & stremp & ")"
            End If
            oGrid.DataTable.ExecuteQuery(strSql)
            oGrid.Columns.Item("Code").TitleObject.Caption = "Code"
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee ID"
            oGrid.Columns.Item("U_Z_EmpId").Editable = False
            oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
            oEditTextColumn.LinkedObjectType = 171
            oGrid.Columns.Item("U_Z_FirstName").TitleObject.Caption = "Employee Name"
            oGrid.Columns.Item("U_Z_FirstName").Editable = False
            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
            oGrid.Columns.Item("U_Z_DeptName").Editable = False
            oGrid.Columns.Item("U_Z_OrgCode").TitleObject.Caption = "Organization Code"
            oGrid.Columns.Item("U_Z_OrgCode").Visible = False
            oGrid.Columns.Item("U_Z_OrgName").TitleObject.Caption = "Organization Name"
            oGrid.Columns.Item("U_Z_OrgName").Editable = False
            oGrid.Columns.Item("U_Z_JobName").TitleObject.Caption = "Job Name"
            oGrid.Columns.Item("U_Z_JobName").Editable = False
            oGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position Name"
            oGrid.Columns.Item("U_Z_PosName").Editable = False
            oGrid.Columns.Item("U_Z_NewPosDate").TitleObject.Caption = "Position Change Date"
            oGrid.Columns.Item("U_Z_NewPosDate").Visible = False
            oGrid.Columns.Item("U_Z_EffFromdt").TitleObject.Caption = "Effective From Date"
            oGrid.Columns.Item("U_Z_EffFromdt").Editable = False
            oGrid.Columns.Item("U_Z_EffTodt").TitleObject.Caption = "Effective To Date"
            oGrid.Columns.Item("U_Z_EffTodt").Editable = False
            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_AppStatus")
            ocombo.ValidValues.Add("P", "Pending")
            ocombo.ValidValues.Add("A", "Approved")
            ocombo.ValidValues.Add("R", "Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_AppStatus").Editable = False
            oGrid.Columns.Item("U_Z_Posting").TitleObject.Caption = "Posting"
            oGrid.Columns.Item("U_Z_Posting").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_Posting")
            ocombo.ValidValues.Add("Y", "Yes")
            ocombo.ValidValues.Add("N", "No")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_Posting").Editable = False
        ElseIf strType = "P" Then
            If strDept = "" Then
                'strDept = getdepartment(stremp)
                strSql = "	select ""Code"",""U_Z_EmpId"",""U_Z_FirstName"",""U_Z_DeptName"",""U_Z_PosName"",""U_Z_JobName"",""U_Z_OrgName"","
                strSql = strSql & """U_Z_ProJoinDate"",""U_Z_IncAmount"",""U_Z_EffFromdt"",""U_Z_EffTodt"",""U_Z_AppStatus"",""U_Z_Posting"" from ""@Z_HR_HEM2"" where  ""U_Z_AppStatus""='A'" ' and  ""U_Z_EmpId"" in (" & stremp & ")"
            ElseIf strDept <> "" Then
                strSql = "	select ""Code"",""U_Z_EmpId"",""U_Z_FirstName"",""U_Z_DeptName"",""U_Z_PosName"",""U_Z_JobName"",""U_Z_OrgName"","
                strSql = strSql & """U_Z_ProJoinDate"",""U_Z_IncAmount"",""U_Z_EffFromdt"",""U_Z_EffTodt"",""U_Z_AppStatus"",""U_Z_Posting"" from ""@Z_HR_HEM2"" where  ""U_Z_AppStatus""='A' and  ""U_Z_Dept"" in (" & strDept & ")" '  and  ""U_Z_EmpId"" in (" & stremp & ")"
            End If
            oGrid.DataTable.ExecuteQuery(strSql)
            oGrid.Columns.Item("Code").TitleObject.Caption = "Code"
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee ID"
            oGrid.Columns.Item("U_Z_EmpId").Editable = False
            oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
            oEditTextColumn.LinkedObjectType = 171
            oGrid.Columns.Item("U_Z_FirstName").TitleObject.Caption = "Employee Name"
            oGrid.Columns.Item("U_Z_FirstName").Editable = False
            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
            oGrid.Columns.Item("U_Z_DeptName").Editable = False
            oGrid.Columns.Item("U_Z_OrgName").TitleObject.Caption = "Organization Name"
            oGrid.Columns.Item("U_Z_OrgName").Editable = False
            oGrid.Columns.Item("U_Z_JobName").TitleObject.Caption = "Job Name"
            oGrid.Columns.Item("U_Z_JobName").Editable = False
            oGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position Name"
            oGrid.Columns.Item("U_Z_PosName").Editable = False
            oGrid.Columns.Item("U_Z_ProJoinDate").TitleObject.Caption = "Promotion Date"
            oGrid.Columns.Item("U_Z_ProJoinDate").Editable = False
            oGrid.Columns.Item("U_Z_IncAmount").TitleObject.Caption = "Increment Amount"
            oGrid.Columns.Item("U_Z_IncAmount").Editable = False
            oGrid.Columns.Item("U_Z_EffFromdt").TitleObject.Caption = "Effective From Date"
            oGrid.Columns.Item("U_Z_EffFromdt").Editable = False
            oGrid.Columns.Item("U_Z_EffTodt").TitleObject.Caption = "Effective To Date"
            oGrid.Columns.Item("U_Z_EffTodt").Editable = False
            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_AppStatus")
            ocombo.ValidValues.Add("P", "Pending")
            ocombo.ValidValues.Add("A", "Approved")
            ocombo.ValidValues.Add("R", "Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_AppStatus").Editable = False
            oGrid.Columns.Item("U_Z_Posting").TitleObject.Caption = "Posting"
            oGrid.Columns.Item("U_Z_Posting").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_Posting")
            ocombo.ValidValues.Add("Y", "Yes")
            ocombo.ValidValues.Add("N", "No")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_Posting").Editable = False
        End If
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oApplication.Utilities.AssignRowNo(oGrid, aForm)
    End Sub
    Public Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        oCombobox = aForm.Items.Item("36").Specific
        Dim strType As String = oCombobox.Selected.Value
        If strType = "" Then
            oApplication.Utilities.Message("Select Posting Type...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End If
        Return True
    End Function
    Private Function EmployeePosting(ByVal aform As SAPbouiCOM.Form, ByVal strtype As String) As Boolean
        Try
            aform.Freeze(True)
            Dim strempid, strposid, strCode As String
            Dim dt As Date
            'If oApplication.Company.InTransaction() Then
            '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            'End If
            '  oApplication.Company.StartTransaction()
            Dim oUserTable As SAPbobsCOM.UserTable
            oGrid = aform.Items.Item("10").Specific
            Dim oCheckbox, ocheckbox1 As SAPbouiCOM.CheckBoxColumn
            If strtype = "P" Then
                For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                    oCheckbox = oGrid.Columns.Item(0)
                    If oCheckbox.IsChecked(intRow) Then
                        strempid = oGrid.DataTable.GetValue("U_Z_EmpId", intRow)
                        strposid = oGrid.DataTable.GetValue("U_Z_PosCode", intRow)
                        strCode = oGrid.DataTable.GetValue("Code", intRow)
                        If oApplication.Utilities.UpdateEmployeeProfile(oForm, strempid, strposid, oGrid.DataTable.GetValue("U_Z_EffFromdt", intRow), "P", strCode) = False Then
                            'If oApplication.Company.InTransaction() Then
                            '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                            'End If
                        End If
                    End If
                Next
            ElseIf strtype = "C" Then
                For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                    oCheckbox = oGrid.Columns.Item(0)
                    If oCheckbox.IsChecked(intRow) Then
                        strempid = oGrid.DataTable.GetValue("U_Z_EmpId", intRow)
                        strposid = oGrid.DataTable.GetValue("U_Z_PosCode", intRow)
                        strCode = oGrid.DataTable.GetValue("Code", intRow)
                        If oApplication.Utilities.UpdateEmployeeProfile(oForm, strempid, strposid, oGrid.DataTable.GetValue("U_Z_EffFromdt", intRow), "C", strCode) = False Then
                            'If oApplication.Company.InTransaction() Then
                            '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                            'End If
                        End If
                    End If
                Next
            End If
            'If oApplication.Company.InTransaction() Then
            '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            'End If
            aform.Freeze(False)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            'If oApplication.Company.InTransaction() Then
            '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            'End If

            aform.Freeze(False)
            Return False
        End Try
    End Function
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_EmpLifePost Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" And oForm.PaneLevel = 2 Then
                                    If Validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                If pVal.ItemUID = "_2" Then
                                    oForm.Close()
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "22"
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = 3
                                        oForm.Freeze(False)
                                    Case "23"
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = 4
                                        oForm.Freeze(False)
                                    Case "3"
                                        oForm.Freeze(True)
                                        Dim strDept, strType As String
                                        oForm.PaneLevel = oForm.PaneLevel + 1
                                        If oForm.PaneLevel = 3 Then
                                            oCombobox = oForm.Items.Item("34").Specific
                                            strDept = oCombobox.Selected.Value
                                            oCombobox1 = oForm.Items.Item("36").Specific
                                            strType = oCombobox1.Selected.Value
                                            NewGridbind(oForm, strDept, strType)
                                            SummaryGridbind(oForm, strDept, strType)
                                        End If
                                        oForm.Freeze(False)
                                    Case "4"
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = oForm.PaneLevel - 1
                                        oForm.Freeze(False)
                                    Case "5"
                                        Dim strType As String
                                        oCombobox1 = oForm.Items.Item("36").Specific
                                        strType = oCombobox1.Selected.Value
                                        If oApplication.SBO_Application.MessageBox("Do you want confirm the Posting", , "Yes", "No") = 2 Then
                                            Exit Sub
                                        ElseIf EmployeePosting(oForm, strType) = True Then
                                            oApplication.Utilities.Message("Operation Completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                            oForm.Close()
                                        Else
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    Case "26"
                                        Selectall(oForm, True)
                                    Case "27"
                                        Selectall(oForm, False)
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
                Case mnu_HR_EmpLifePost
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
