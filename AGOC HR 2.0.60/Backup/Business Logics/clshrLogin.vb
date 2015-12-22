Public Class clshrLogin
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox, oCombobox1, oCombobox2, oCombobox3 As SAPbouiCOM.ComboBox
    Private oColumn As SAPbouiCOM.Column
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oRecset As SAPbobsCOM.Recordset
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo, sPath As String
    Private InvForConsumedItems, count As Integer
    Private blnFlag As Boolean = False
    Private MatrixId1 As String
    Dim oDataSrc_Line As SAPbouiCOM.DBDataSource
    Dim oDataSrc_Line1 As SAPbouiCOM.DBDataSource
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Public Sub LoadForm(ByVal aChoice As String)
        If aChoice = "Self" Then
            EntryChoice = "Appraisals"
        ElseIf aChoice = "MgrApp" Then
            EntryChoice = "Manager Approval"
        ElseIf aChoice = "HR" Then
            EntryChoice = "HR Approval"
        ElseIf aChoice = "COREV" Then
            EntryChoice = "Course Review"
        ElseIf aChoice = "MPR" Then
            EntryChoice = "Recruitment"
        ElseIf aChoice = "RHR" Then
            EntryChoice = "Recruitment First Level Approval"
        ElseIf aChoice = "RGM" Then
            EntryChoice = "Recruitment HR Approval"
        ElseIf aChoice = "TraReq" Then
            EntryChoice = "Employee Travel Request"
        ElseIf aChoice = "HRTraReqApp" Then
            EntryChoice = "Travel Request Approval"
        ElseIf aChoice = "EmpExpClaim" Then
            EntryChoice = "Employee Expenses Claim"
        ElseIf aChoice = "HRExpApproval" Then
            EntryChoice = "Expenses Approval"
        ElseIf aChoice = "TrainEva" Then
            EntryChoice = "Training Evaluation"
        ElseIf aChoice = "SCHTRA" Then
            EntryChoice = "Apply Schedule Training"
        ElseIf aChoice = "LVEREQ" Then
            EntryChoice = "Leave Request"
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_Login, frm_hr_Login)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        FillPeriod(oForm)
        FillCourse(oForm)
        databind(oForm)
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE
        If EntryChoice = "Appraisals" Then
            oCombobox = oForm.Items.Item("8").Specific
            oCombobox.Select("Self", SAPbouiCOM.BoSearchKey.psk_ByValue)
            oForm.Items.Item("3").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Items.Item("8").Enabled = False
            oForm.Items.Item("9").Visible = False
            oForm.Items.Item("10").Visible = False
        ElseIf EntryChoice = "Manager Approval" Then
            oCombobox = oForm.Items.Item("8").Specific
            oCombobox.Select("MgrApp", SAPbouiCOM.BoSearchKey.psk_ByValue)
            oForm.Items.Item("3").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Items.Item("8").Enabled = False
        ElseIf EntryChoice = "HR Approval" Then
            oCombobox = oForm.Items.Item("8").Specific
            oCombobox.Select("HR", SAPbouiCOM.BoSearchKey.psk_ByValue)
            oForm.Items.Item("3").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Items.Item("8").Enabled = False
        ElseIf EntryChoice = "Course Review" Then
            oCombobox = oForm.Items.Item("8").Specific
            oCombobox.Select("COREV", SAPbouiCOM.BoSearchKey.psk_ByValue)
            oForm.Items.Item("3").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Items.Item("8").Enabled = False
            oForm.Items.Item("13").Visible = True
            oForm.Items.Item("14").Visible = True
        ElseIf EntryChoice = "Recruitment" Then
            oCombobox = oForm.Items.Item("8").Specific
            oCombobox.Select("MPR", SAPbouiCOM.BoSearchKey.psk_ByValue)
            oForm.Items.Item("3").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Items.Item("8").Enabled = False
            oForm.Items.Item("9").Visible = True
            oForm.Items.Item("10").Visible = True
        ElseIf EntryChoice = "Recruitment First Level Approval" Then
            oCombobox = oForm.Items.Item("8").Specific
            oCombobox.Select("RHR", SAPbouiCOM.BoSearchKey.psk_ByValue)
            oForm.Items.Item("3").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Items.Item("8").Enabled = False
        ElseIf EntryChoice = "Recruitment HR Approval" Then
            oCombobox = oForm.Items.Item("8").Specific
            oCombobox.Select("RGM", SAPbouiCOM.BoSearchKey.psk_ByValue)
            oForm.Items.Item("3").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Items.Item("8").Enabled = False
        ElseIf EntryChoice = "Employee Travel Request" Then
            oCombobox = oForm.Items.Item("8").Specific
            oCombobox.Select("TraReq", SAPbouiCOM.BoSearchKey.psk_ByValue)
            oForm.Items.Item("3").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Items.Item("8").Enabled = False
            oForm.Items.Item("9").Visible = True
            oForm.Items.Item("10").Visible = True
        ElseIf EntryChoice = "Travel Request Approval" Then
            oCombobox = oForm.Items.Item("8").Specific
            oCombobox.Select("HRTraReqApp", SAPbouiCOM.BoSearchKey.psk_ByValue)
            oForm.Items.Item("3").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Items.Item("8").Enabled = False
        ElseIf EntryChoice = "Employee Expenses Claim" Then
            oCombobox = oForm.Items.Item("8").Specific
            oCombobox.Select("EmpExpClaim", SAPbouiCOM.BoSearchKey.psk_ByValue)
            oForm.Items.Item("3").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Items.Item("8").Enabled = False
        ElseIf EntryChoice = "Training Evaluation" Then
            oCombobox = oForm.Items.Item("8").Specific
            oCombobox.Select("TrainEva", SAPbouiCOM.BoSearchKey.psk_ByValue)
            oForm.Items.Item("3").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Items.Item("8").Enabled = False
        ElseIf EntryChoice = "Expenses Approval" Then
            oCombobox = oForm.Items.Item("8").Specific
            oCombobox.Select("HRExpApproval", SAPbouiCOM.BoSearchKey.psk_ByValue)
            oForm.Items.Item("3").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Items.Item("8").Enabled = False
        ElseIf EntryChoice = "Apply Schedule Training" Then
            oCombobox = oForm.Items.Item("8").Specific
            oCombobox.Select("SCHTRA", SAPbouiCOM.BoSearchKey.psk_ByValue)
            oForm.Items.Item("3").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Items.Item("8").Enabled = False
        ElseIf EntryChoice = "Leave Request" Then
            oCombobox = oForm.Items.Item("8").Specific
            oCombobox.Select("LVEREQ", SAPbouiCOM.BoSearchKey.psk_ByValue)
            oForm.Items.Item("3").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Items.Item("8").Enabled = False
            oForm.Items.Item("9").Visible = True
            oForm.Items.Item("10").Visible = True
        Else
            oForm.Items.Item("8").Enabled = True
        End If
        If blnSourceForm = True Then
            BinLoginDetails(oForm)
            blnSourceForm = False
        End If
        oForm.Freeze(False)
    End Sub
    Private Sub FillCourse(ByVal aForm As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = aForm.Items.Item("14").Specific
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oTempRec.DoQuery("Select U_Z_CourseCode,U_Z_CourseName from [@Z_HR_OCOUR] order by DocEntry desc")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item("U_Z_CourseCode").Value, oTempRec.Fields.Item("U_Z_CourseName").Value)
            oTempRec.MoveNext()
        Next
    End Sub

    Private Sub FillPeriod(ByVal aForm As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = aForm.Items.Item("12").Specific
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oTempRec.DoQuery("Select Code,Name from OFPR order by Code desc")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item("Code").Value, oTempRec.Fields.Item("Name").Value)
            oTempRec.MoveNext()
        Next
    End Sub

#Region "Bind Login Details"
    Private Sub BinLoginDetails(ByVal aForm As SAPbouiCOM.Form)
        Dim otemp As SAPbobsCOM.Recordset
        otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp.DoQuery("Select U_Z_UID,U_Z_Pwd, U_Z_EmpiD,isnull(U_Z_Approver,'N') from [@Z_HR_LOGIN] where U_Z_EmpID='" & strSourceformEmpID & "'")
        If otemp.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(aForm, "3", otemp.Fields.Item(0).Value)
            oApplication.Utilities.setEdittextvalue(aForm, "5", otemp.Fields.Item(1).Value)
        End If
    End Sub
#End Region
#Region "DataBind"
    Private Sub databind(ByVal aForm As SAPbouiCOM.Form)
        Try
            Dim oCode As String
            aForm.Freeze(True)
            aForm.DataSources.UserDataSources.Add("Empid", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            aForm.DataSources.UserDataSources.Add("Pwd", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            aForm.DataSources.UserDataSources.Add("DocType", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            aForm.DataSources.UserDataSources.Add("ViewType", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            '           aForm.DataSources.UserDataSources.Add("dtDate", SAPbouiCOM.BoDataType.dt_DATE)
            oApplication.Utilities.setUserDatabind(aForm, "3", "Empid")
            oApplication.Utilities.setUserDatabind(aForm, "5", "Pwd")
            ' oApplication.Utilities.setUserDatabind(aForm, "12", "dtDate")
            oCombobox = aForm.Items.Item("8").Specific
            oCombobox.ValidValues.Add("", "")
            oCombobox.ValidValues.Add("Self", "Appraisals")
            oCombobox.ValidValues.Add("MgrApp", "Manager Approval")
            oCombobox.ValidValues.Add("HR", "HR Approval")
            oCombobox.ValidValues.Add("COREV", "Course Review")
            oCombobox.ValidValues.Add("MPR", "Recruitment")
            oCombobox.ValidValues.Add("RHR", "Recruitment First Level Approval")
            oCombobox.ValidValues.Add("RGM", "Recruitment HR Approval")
            oCombobox.ValidValues.Add("TraReq", "Employee Travel Request")
            oCombobox.ValidValues.Add("HRTraReqApp", "Travel Request Approval")
            oCombobox.ValidValues.Add("EmpExpClaim", "Employee Expenses Claim")
            oCombobox.ValidValues.Add("HRExpApproval", "Expenses Approval")
            oCombobox.ValidValues.Add("TrainEva", "Training Evaluation")
            oCombobox.ValidValues.Add("SCHTRA", "Apply Schedule Training")
            oCombobox.ValidValues.Add("LVEREQ", "Leave Request")
            oCombobox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
            aForm.Items.Item("8").DisplayDesc = True
            oCombobox = aForm.Items.Item("10").Specific
            oCombobox.ValidValues.Add("", "")
            oCombobox.ValidValues.Add("A", "Add")
            oCombobox.ValidValues.Add("V", "View/Edit")
            oCombobox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
            aForm.Items.Item("3").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            aForm.Items.Item("10").DisplayDesc = True
            aForm.Items.Item("9").Visible = False
            aForm.Items.Item("10").Visible = False
            aForm.Items.Item("11").Visible = False
            aForm.Items.Item("12").Visible = False
            aForm.Items.Item("13").Visible = False
            aForm.Items.Item("14").Visible = False
            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
        End Try
    End Sub
#End Region
#Region "validations"
    Private Function validation(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim UID, pwd, ActionType, Period, courseCode, courseName As String
        Dim otemp As SAPbobsCOM.Recordset
        UID = oApplication.Utilities.getEdittextvalue(aform, "3")
        pwd = oApplication.Utilities.getEdittextvalue(aform, "5")
        oCombobox = aform.Items.Item("8").Specific
        EntryChoice = oCombobox.Selected.Value
        oCombobox1 = aform.Items.Item("10").Specific
        ActionType = oCombobox1.Selected.Value
        oCombobox2 = aform.Items.Item("12").Specific
        Period = oCombobox2.Selected.Value
        oCombobox3 = aform.Items.Item("14").Specific
        courseCode = oCombobox3.Selected.Value
        courseName = oCombobox3.Selected.Description
        If EntryChoice = "" Then
            oApplication.Utilities.Message("Select the Document type", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End If
        If EntryChoice = "Self" Then
            'If ActionType = "" Then
            '    oApplication.Utilities.Message("Select the Action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '    Return False
            'ElseIf ActionType = "A" Then
            '    If Period = "" Then
            '        oApplication.Utilities.Message("Select the Period", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '        Return False
            '    End If
            'End If
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemp.DoQuery("Select U_Z_EmpiD,U_Z_EmpName,isnull(U_Z_Approver,'N'),isnull(U_Z_SUPERUSER,'N') from [@Z_HR_LOGIN] where U_Z_UID='" & UID & "' and U_Z_PWD='" & pwd & "'")
            If otemp.RecordCount <= 0 Then
                oApplication.Utilities.Message("Invalid login details....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            Else
                If otemp.Fields.Item(2).Value <> "Y" And otemp.Fields.Item(3).Value <> "Y" Then
                    oApplication.Utilities.Message("You are not Authorized to Perform this Action...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                ElseIf ActionType = "V" Then
                    Dim objct As New clshrSelfAppraisal
                    strApprovalType = "Self"
                    objct.LoadForm1(otemp.Fields.Item(0).Value, otemp.Fields.Item(1).Value, Period)
                    Return True
                Else
                    Dim objct As New clshrApproval
                    strApprovalType = "Self"
                    Dim strqry As String
                    Dim strEmp As String = otemp.Fields.Item(0).Value
                    strqry = "select DocEntry,U_Z_EmpId,U_Z_EmpName,U_Z_Date,U_Z_Period,case U_Z_Status when 'D' then 'Draft' when 'F' then 'Approved'"
                    strqry = strqry & " when 'S'then '2nd Level Approval' when 'L' then 'Closed' else 'Canceled' end as U_Z_Status,case U_Z_WStatus when 'DR' then 'Draft' when 'HR' then 'HR Approved' when 'SM'then 'Sr.Manager Approved' when 'LM' then 'LineManager Approved'when 'SE' then 'SelfApproved'  end as 'U_Z_WStatus' from [@Z_HR_OSEAPP] where U_Z_EmpId='" & otemp.Fields.Item(0).Value & "'"
                    otemp.DoQuery(strqry)
                    If Not otemp.EoF Then
                        objct.LoadForm(strApprovalType, strEmp)
                        Return True
                    Else
                        oApplication.Utilities.Message("No Records Found....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
            End If
        End If
        If EntryChoice = "MgrApp" Then
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemp.DoQuery("Select U_Z_EmpiD,isnull(U_Z_MGRAPPROVER,'N'),isnull(U_Z_SUPERUSER,'N') from [@Z_HR_LOGIN] where U_Z_UID='" & UID & "' and U_Z_PWD='" & pwd & "'")
            If otemp.RecordCount <= 0 Then
                oApplication.Utilities.Message("Invalid login details....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            Else
                If otemp.Fields.Item(1).Value <> "Y" And otemp.Fields.Item(2).Value <> "Y" Then
                    oApplication.Utilities.Message("You are not Authorized to Perform this Action...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    Dim objct As New clshrApproval
                    strApprovalType = "MgrApp"
                    objct.LoadForm(strApprovalType)
                    Return True
                End If
            End If
        End If
        If EntryChoice = "HR" Then
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemp.DoQuery("Select U_Z_EmpiD,isnull(U_Z_HRAPPROVER,'N'),isnull(U_Z_SUPERUSER,'N') from [@Z_HR_LOGIN] where U_Z_UID='" & UID & "' and U_Z_PWD='" & pwd & "'")
            If otemp.RecordCount <= 0 Then
                oApplication.Utilities.Message("Invalid login details....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            Else
                If otemp.Fields.Item(1).Value <> "Y" And otemp.Fields.Item(2).Value <> "Y" Then
                    oApplication.Utilities.Message("You are not Authorized to Perform this Action...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    Dim objct As New clshrApproval
                    strApprovalType = "HR"
                    objct.LoadForm(strApprovalType)
                    Return True
                End If
            End If
        End If
        If EntryChoice = "COREV" Then
            'If courseCode = "" Then
            '    oApplication.Utilities.Message("Select Course Code/Name....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '    Return False
            'End If
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemp.DoQuery("Select U_Z_EmpiD,isnull(U_Z_SUPERUSER,'N') from [@Z_HR_LOGIN] where U_Z_UID='" & UID & "' and U_Z_PWD='" & pwd & "'")
            If otemp.RecordCount <= 0 Then
                oApplication.Utilities.Message("Invalid login details....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            Else
                If otemp.Fields.Item(1).Value <> "Y" Then
                    oApplication.Utilities.Message("You are not Authorized to Perform this Action...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    Dim objct As New clshrCourseReview
                    strApprovalType = "COREV"
                    objct.LoadForm(courseCode, courseName)
                    Return True
                End If
            End If
        End If
        If EntryChoice = "MPR" Then
            If ActionType = "" Then
                oApplication.Utilities.Message("Select the Action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemp.DoQuery("Select U_Z_EmpiD,U_Z_EmpName,isnull(U_Z_MGRREQUEST,'N'),isnull(U_Z_SUPERUSER,'N') from [@Z_HR_LOGIN] where U_Z_UID='" & UID & "' and U_Z_PWD='" & pwd & "'")
            If otemp.RecordCount <= 0 Then
                oApplication.Utilities.Message("Invalid login details....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            Else
                If otemp.Fields.Item(2).Value <> "Y" And otemp.Fields.Item(3).Value <> "Y" Then
                    oApplication.Utilities.Message("You are not Authorized to Perform this Action...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                ElseIf ActionType = "V" Then
                    Dim objct As New clshrRecApproval
                    strApprovalType = "MPR"
                    objct.LoadForm(strApprovalType, otemp.Fields.Item(0).Value, otemp.Fields.Item(1).Value)
                    Return True
                Else
                    Dim objct As New clshrMPRequest
                    strApprovalType = "MPR"
                    objct.LoadForm(otemp.Fields.Item(0).Value, otemp.Fields.Item(1).Value, ActionType)
                    Return True
                End If
            End If
        End If
        If EntryChoice = "RHR" Then
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemp.DoQuery("Select U_Z_EmpiD,isnull(U_Z_HRRECAPPROVER,'N'),isnull(U_Z_SUPERUSER,'N'),U_Z_EmpName from [@Z_HR_LOGIN] where U_Z_UID='" & UID & "' and U_Z_PWD='" & pwd & "'")
            If otemp.RecordCount <= 0 Then
                oApplication.Utilities.Message("Invalid login details....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            Else
                If otemp.Fields.Item(1).Value <> "Y" And otemp.Fields.Item(2).Value <> "Y" Then
                    oApplication.Utilities.Message("You are not Authorized to Perform this Action...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    Dim objct As New clshrRecApproval
                    strApprovalType = "RHR"
                    objct.LoadForm(strApprovalType, otemp.Fields.Item(0).Value, otemp.Fields.Item(3).Value)
                    Return True
                End If
            End If
        End If
        If EntryChoice = "RGM" Then
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemp.DoQuery("Select U_Z_EmpiD,isnull(U_Z_GMRECAPPROVER,'N'),U_Z_EmpName,isnull(U_Z_SUPERUSER,'N') from [@Z_HR_LOGIN] where U_Z_UID='" & UID & "' and U_Z_PWD='" & pwd & "'")
            If otemp.RecordCount <= 0 Then
                oApplication.Utilities.Message("Invalid login details....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            Else
                If otemp.Fields.Item(1).Value <> "Y" And otemp.Fields.Item(3).Value <> "Y" Then
                    oApplication.Utilities.Message("You are not Authorized to Perform this Action...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    Dim objct As New clshrHRecApproval
                    strApprovalType = "RGM"
                    objct.LoadForm(strApprovalType, otemp.Fields.Item(0).Value, otemp.Fields.Item(2).Value)
                    Return True
                End If
            End If
        End If
        If EntryChoice = "TraReq" Then
            If ActionType = "" Then
                oApplication.Utilities.Message("Select the Action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemp.DoQuery("Select U_Z_EmpiD from [@Z_HR_LOGIN] where U_Z_UID='" & UID & "' and U_Z_PWD='" & pwd & "'")
            If otemp.RecordCount <= 0 Then
                oApplication.Utilities.Message("Invalid login details....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            Else
                If ActionType = "V" Then
                    Dim objct As New clshrViewTraRequest
                    strApprovalType = "EmpReq"
                    objct.LoadForm(strApprovalType, otemp.Fields.Item(0).Value)
                Else
                    Dim objct As New clshrTravelRequest
                    strApprovalType = "TraReq"
                    objct.LoadForm(oForm, strApprovalType, otemp.Fields.Item(0).Value)
                End If
                Return True
            End If
        End If
        If EntryChoice = "HRTraReqApp" Then
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemp.DoQuery("Select U_Z_EmpiD,isnull(U_Z_SUPERUSER,'N') from [@Z_HR_LOGIN] where U_Z_UID='" & UID & "' and U_Z_PWD='" & pwd & "'")
            If otemp.RecordCount <= 0 Then
                oApplication.Utilities.Message("Invalid login details....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            Else
                If otemp.Fields.Item(1).Value <> "Y" Then
                    oApplication.Utilities.Message("You are not Authorized to Perform this Action...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    Dim objct As New clshrTravelApproval
                    strApprovalType = "HRTraReqApp"
                    objct.LoadForm(strApprovalType)
                    Return True
                End If
            End If
        End If
        If EntryChoice = "EmpExpClaim" Then
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemp.DoQuery("Select U_Z_EmpiD,U_Z_EMPNAME from [@Z_HR_LOGIN] where U_Z_UID='" & UID & "' and U_Z_PWD='" & pwd & "'")
            If otemp.RecordCount <= 0 Then
                oApplication.Utilities.Message("Invalid login details....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            Else
                Dim objct As New clshrExpClaimRequest
                strApprovalType = "EmpExpClaim"
                objct.LoadForm(otemp.Fields.Item(0).Value, otemp.Fields.Item(1).Value)
                Return True
            End If
        End If
        If EntryChoice = "TrainEva" Then
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemp.DoQuery("Select U_Z_EmpiD,U_Z_EMPNAME from [@Z_HR_LOGIN] where U_Z_UID='" & UID & "' and U_Z_PWD='" & pwd & "'")
            If otemp.RecordCount <= 0 Then
                oApplication.Utilities.Message("Invalid login details....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            Else
                Dim objct As New clshrTrainEvaluation
                strApprovalType = "EmpExpClaim"
                objct.LoadForm(otemp.Fields.Item(0).Value, otemp.Fields.Item(1).Value)
                Return True
            End If
        End If

        If EntryChoice = "HRExpApproval" Then
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemp.DoQuery("Select U_Z_EmpiD,isnull(U_Z_SUPERUSER,'N') from [@Z_HR_LOGIN] where U_Z_UID='" & UID & "' and U_Z_PWD='" & pwd & "'")
            If otemp.RecordCount <= 0 Then
                oApplication.Utilities.Message("Invalid login details....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            Else
                If otemp.Fields.Item(1).Value <> "Y" Then
                    oApplication.Utilities.Message("You are not Authorized to Perform this Action...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    Dim objct As New clshrTravelApproval
                    strApprovalType = "HRExpApproval"
                    objct.LoadForm(strApprovalType)
                    Return True
                End If
            End If
        End If

        If EntryChoice = "SCHTRA" Then
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemp.DoQuery("Select U_Z_EmpiD,U_Z_EMPNAME,isnull(U_Z_SUPERUSER,'N') from [@Z_HR_LOGIN] where U_Z_UID='" & UID & "' and U_Z_PWD='" & pwd & "'")
            If otemp.RecordCount <= 0 Then
                oApplication.Utilities.Message("Invalid login details....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
                'Else
                '    If otemp.Fields.Item(2).Value <> "Y" Then
                '        oApplication.Utilities.Message("You are not Authorized to Perform this Action...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '        Return False
            Else
                Dim objct As New clshrEmpTraining
                strApprovalType = "SCHTRA"
                objct.LoadForm(otemp.Fields.Item(0).Value, otemp.Fields.Item(1).Value)
                Return True
            End If
        End If

        If EntryChoice = "LVEREQ" Then
            If ActionType = "" Then
                oApplication.Utilities.Message("Select the Action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemp.DoQuery("Select U_Z_EmpiD,U_Z_EMPNAME,isnull(U_Z_SUPERUSER,'N') from [@Z_HR_LOGIN] where U_Z_UID='" & UID & "' and U_Z_PWD='" & pwd & "'")
            If otemp.RecordCount <= 0 Then
                oApplication.Utilities.Message("Invalid login details....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            ElseIf ActionType = "V" Then
                Dim objct As New clshrLeaveRequest
                strApprovalType = "LVEREQ"
                objct.ViewLoadForm(otemp.Fields.Item(0).Value, otemp.Fields.Item(1).Value)
                Return True
            Else
                Dim objct As New clshrLeaveRequest
                strApprovalType = "LVEREQ"
                objct.LoadForm(otemp.Fields.Item(0).Value, otemp.Fields.Item(1).Value)
                Return True
            End If
        End If


    End Function

#End Region
#Region "enable Controls"
    Private Sub enableControls(ByVal aform As SAPbouiCOM.Form)
        If EntryChoice = "Self" Then
            aform.Items.Item("9").Visible = True
            aform.Items.Item("10").Visible = True
        Else
            aform.Items.Item("9").Visible = False
            aform.Items.Item("10").Visible = False
        End If
    End Sub
#End Region


#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_Login Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD

                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "10" Then
                                    'oCombobox = oForm.Items.Item("8").Specific
                                    oCombobox1 = oForm.Items.Item("8").Specific
                                    ' If oCombobox1.Selected.Value = "Self" Or oCombobox1.Selected.Value = "MPR" Then
                                    oCombobox = oForm.Items.Item("10").Specific
                                    If oCombobox.Selected.Value = "V" Or oCombobox1.Selected.Value = "MPR" Or oCombobox1.Selected.Value = "TraReq" Then
                                        oForm.Items.Item("11").Visible = False
                                        oForm.Items.Item("12").Visible = False
                                    Else
                                        oForm.Items.Item("11").Visible = True
                                        oForm.Items.Item("12").Visible = True
                                    End If
                                    If oCombobox1.Selected.Value = "LVEREQ" Then
                                        oForm.Items.Item("11").Visible = False
                                        oForm.Items.Item("12").Visible = False
                                    End If
                                    'End If
                                ElseIf pVal.ItemUID = "8" Then
                                enableControls(oForm)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "6" Then
                                    If validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    Else
                                        oForm.Close()
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
