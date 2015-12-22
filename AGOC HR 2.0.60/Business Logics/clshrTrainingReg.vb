Public Class clshrTrainingReg
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oComboColumn As SAPbouiCOM.ComboBoxColumn
    Private oCheckbox, oCheckbox1, oCheckbox2, oCheckbox3, oCheckbox4, oCheckbox5, oCheckbox6 As SAPbouiCOM.CheckBox
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private ocombo As SAPbouiCOM.ComboBoxColumn
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Public Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_TrainReg) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_TrainReg, frm_hr_TrainReg)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.Title = "Training Registration and Approval"
        AddChooseFromList(oForm)

        oForm.DataSources.UserDataSources.Add("Chk1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1)
        oApplication.Utilities.setUserDSCheckBox(oForm, "46", "Chk1")
        oForm.DataSources.UserDataSources.Add("Chk2", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1)
        oApplication.Utilities.setUserDSCheckBox(oForm, "47", "Chk2")
        oForm.DataSources.UserDataSources.Add("Chk3", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1)
        oApplication.Utilities.setUserDSCheckBox(oForm, "48", "Chk3")
        oForm.DataSources.UserDataSources.Add("Chk4", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1)
        oApplication.Utilities.setUserDSCheckBox(oForm, "49", "Chk4")
        oForm.DataSources.UserDataSources.Add("Chk5", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1)
        oApplication.Utilities.setUserDSCheckBox(oForm, "50", "Chk5")
        oForm.DataSources.UserDataSources.Add("Chk6", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1)
        oApplication.Utilities.setUserDSCheckBox(oForm, "51", "Chk6")
        oForm.DataSources.UserDataSources.Add("Chk7", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1)
        oApplication.Utilities.setUserDSCheckBox(oForm, "52", "Chk7")
        oForm.DataSources.UserDataSources.Add("Agendano", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "16", "Agendano")
        oEditText = oForm.Items.Item("16").Specific
        oEditText.ChooseFromListUID = "CFL1"
        oEditText.ChooseFromListAlias = "U_Z_TrainCode"

        oForm.DataSources.UserDataSources.Add("TraCode1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "64", "TraCode1")
        oForm.DataSources.UserDataSources.Add("TraCode2", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "66", "TraCode2")

        oForm.DataSources.UserDataSources.Add("date1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "31", "date1")
        oForm.DataSources.UserDataSources.Add("date2", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "33", "date2")
        oForm.DataSources.UserDataSources.Add("date3", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "35", "date3")
        oForm.DataSources.UserDataSources.Add("date4", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "37", "date4")

        'oEditText = oForm.Items.Item("6").Specific
        'oEditText.ChooseFromListUID = "CFL4"
        'oEditText.ChooseFromListAlias = "DocEntry"

        oForm.PaneLevel = 1
        Dim osta As SAPbouiCOM.StaticText
        osta = oForm.Items.Item("19").Specific
        osta.Caption = "Step " & oForm.PaneLevel & " of 4"
        oForm.Items.Item("19").TextStyle = SAPbouiCOM.BoTextStyle.ts_BOLD
        reDrawForm(oForm)
        oForm.Freeze(False)
    End Sub
    Private Sub AddChooseFromList(ByVal objForm As SAPbouiCOM.Form)
        Try
            Dim oCFLs As SAPbouiCOM.ChooseFromListCollection
            Dim oCons As SAPbouiCOM.Conditions
            Dim oCon As SAPbouiCOM.Condition
            oCFLs = objForm.ChooseFromLists
            Dim oCFL As SAPbouiCOM.ChooseFromList
            Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)

            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_OTRIN"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "U_Z_Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "171"
            oCFLCreationParams.UniqueID = "CFL2"
            oCFL = oCFLs.Add(oCFLCreationParams)

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim Reqno As String
            Reqno = oApplication.Utilities.getEdittextvalue(aForm, "16")
            If Reqno = "" Then
                oApplication.Utilities.Message("Agenda Code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
#End Region

    Private Sub PopulateAgenda(ByVal aform As SAPbouiCOM.Form, ByVal Agendacode As String)
        Dim strqry As String
        Dim oTemp1 As SAPbobsCOM.Recordset
        oCheckbox = aform.Items.Item("46").Specific
        oCheckbox1 = aform.Items.Item("47").Specific
        oCheckbox2 = aform.Items.Item("48").Specific
        oCheckbox3 = aform.Items.Item("49").Specific
        oCheckbox4 = aform.Items.Item("50").Specific
        oCheckbox5 = aform.Items.Item("51").Specific
        oCheckbox6 = aform.Items.Item("52").Specific
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strqry = "  select distinct( U_Z_TrainCode),U_Z_DocDate ,T0.U_Z_CourseCode as 'CourseCode',T0.U_Z_CourseName as 'CourseName',U_Z_CourseTypeDesc,U_Z_Startdt,U_Z_Enddt,U_Z_MinAttendees,U_Z_MaxAttendees,U_Z_AppStdt,U_Z_AppEnddt ,"
        strqry = strqry & " U_Z_InsName,U_Z_NoOfHours,U_Z_StartTime,U_Z_EndTime,U_Z_Sunday,U_Z_Monday,U_Z_Tuesday,U_Z_Wednesday,U_Z_Thursday,U_Z_Friday,U_Z_Saturday,U_Z_AttCost,U_Z_Active  from [@Z_HR_OTRIN] T0 inner join [@Z_HR_OCOUR] T1 on T0.U_Z_CourseCode=T1.U_Z_CourseCode "
        strqry = strqry & "  where  U_Z_TrainCode='" & Agendacode & "' and U_Z_Active='Y' "
        oTemp1.DoQuery(strqry)
        If oTemp1.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(aform, "13", oTemp1.Fields.Item("U_Z_TrainCode").Value)
            oApplication.Utilities.setEdittextvalue(aform, "1000001", oTemp1.Fields.Item("U_Z_DocDate").Value)
            oApplication.Utilities.setEdittextvalue(aform, "17", oTemp1.Fields.Item("CourseCode").Value)
            oApplication.Utilities.setEdittextvalue(aform, "1000003", oTemp1.Fields.Item("CourseName").Value)
            oApplication.Utilities.setEdittextvalue(aform, "27", oTemp1.Fields.Item("U_Z_CourseTypeDesc").Value)
            oApplication.Utilities.setEdittextvalue(aform, "31", oTemp1.Fields.Item("U_Z_Startdt").Value)
            oApplication.Utilities.setEdittextvalue(aform, "29", oTemp1.Fields.Item("U_Z_MaxAttendees").Value)
            oApplication.Utilities.setEdittextvalue(aform, "33", oTemp1.Fields.Item("U_Z_Enddt").Value)
            oApplication.Utilities.setEdittextvalue(aform, "56", oTemp1.Fields.Item("U_Z_MinAttendees").Value)
            oApplication.Utilities.setEdittextvalue(aform, "35", oTemp1.Fields.Item("U_Z_AppStdt").Value)
            oApplication.Utilities.setEdittextvalue(aform, "37", oTemp1.Fields.Item("U_Z_AppEnddt").Value)
            oApplication.Utilities.setEdittextvalue(aform, "39", oTemp1.Fields.Item("U_Z_InsName").Value)
            oApplication.Utilities.setEdittextvalue(aform, "41", oTemp1.Fields.Item("U_Z_NoOfHours").Value)
            oApplication.Utilities.setEdittextvalue(aform, "43", oTemp1.Fields.Item("U_Z_StartTime").Value)
            oApplication.Utilities.setEdittextvalue(aform, "45", oTemp1.Fields.Item("U_Z_EndTime").Value)
            oApplication.Utilities.setEdittextvalue(aform, "54", oTemp1.Fields.Item("U_Z_AttCost").Value)
            If oTemp1.Fields.Item("U_Z_Sunday").Value = "Y" Then
                oCheckbox.Checked = True
            Else
                oCheckbox.Checked = False
            End If
            If oTemp1.Fields.Item("U_Z_Monday").Value = "Y" Then
                oCheckbox1.Checked = True
            Else
                oCheckbox1.Checked = False
            End If
            If oTemp1.Fields.Item("U_Z_Tuesday").Value = "Y" Then
                oCheckbox2.Checked = True
            Else
                oCheckbox2.Checked = False
            End If
            If oTemp1.Fields.Item("U_Z_Wednesday").Value = "Y" Then
                oCheckbox3.Checked = True
            Else
                oCheckbox3.Checked = False
            End If
            If oTemp1.Fields.Item("U_Z_Thursday").Value = "Y" Then
                oCheckbox4.Checked = True
            Else
                oCheckbox4.Checked = False
            End If
            If oTemp1.Fields.Item("U_Z_Friday").Value = "Y" Then
                oCheckbox5.Checked = True
            Else
                oCheckbox5.Checked = False
            End If
            If oTemp1.Fields.Item("U_Z_Saturday").Value = "Y" Then
                oCheckbox6.Checked = True
            Else
                oCheckbox6.Checked = False
            End If
        End If
    End Sub
    Private Sub ReceivedApplicants(ByVal aform As SAPbouiCOM.Form)
        oForm.Freeze(True)
        Dim strqry As String
        oGrid = oForm.Items.Item("57").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_1")
        strqry = "select U_Z_HREmpID,U_Z_HREmpName,U_Z_DeptName,"
        strqry = strqry & " U_Z_Status ,U_Z_Remarks, U_Z_AppStatus,U_Z_AppRequired,Code from [@Z_HR_TRIN1] where U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(aform, "13") & "' "
        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("U_Z_HREmpID").TitleObject.Caption = "Employee Id"
        oGrid.Columns.Item("U_Z_HREmpID").Editable = True
        oEditTextColumn = oGrid.Columns.Item("U_Z_HREmpID")
        oEditTextColumn.ChooseFromListUID = "CFL2"
        oEditTextColumn.ChooseFromListAlias = "empId"
        oEditTextColumn.LinkedObjectType = 171
        oGrid.Columns.Item("U_Z_HREmpName").TitleObject.Caption = "Employee Name"
        oGrid.Columns.Item("U_Z_HREmpName").Editable = False
        oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
        oGrid.Columns.Item("U_Z_DeptName").Editable = False
        oGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Status"
        oGrid.Columns.Item("U_Z_Status").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        ocombo = oGrid.Columns.Item("U_Z_Status")
        ocombo.ValidValues.Add("P", "Pending")
        ocombo.ValidValues.Add("A", "Approved")
        ocombo.ValidValues.Add("R", "Rejected")
        oGrid.Columns.Item("U_Z_Status").Visible = False

        oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Status"
        oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        ocombo = oGrid.Columns.Item("U_Z_AppStatus")
        ocombo.ValidValues.Add("P", "Pending")
        ocombo.ValidValues.Add("A", "Approved")
        ocombo.ValidValues.Add("R", "Rejected")
        ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        ocombo.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        oGrid.Columns.Item("U_Z_AppRequired").TitleObject.Caption = "Approval Required"
        oGrid.Columns.Item("U_Z_AppRequired").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboColumn = oGrid.Columns.Item("U_Z_AppRequired")
        oComboColumn.ValidValues.Add("Y", "Yes")
        oComboColumn.ValidValues.Add("N", "No")
        oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        oGrid.Columns.Item("U_Z_AppRequired").Editable = False

        oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"
        oGrid.Columns.Item("Code").TitleObject.Caption = "Code"
        oGrid.Columns.Item("Code").Visible = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oApplication.Utilities.assignMatrixLineno(oGrid, aform)
        oForm.Freeze(False)
    End Sub
    Private Sub AddRow(ByVal aForm As SAPbouiCOM.Form)
        Select Case aForm.PaneLevel
            Case 4
                oGrid = aForm.Items.Item("57").Specific
        End Select
        Dim strCode As String
        If oGrid.DataTable.Rows.Count - 1 <= 0 Then
            oGrid.DataTable.Rows.Add()
        End If
        If oGrid.DataTable.GetValue("U_Z_HREmpID", oGrid.DataTable.Rows.Count - 1) <> "" Then
            oGrid.DataTable.Rows.Add()
            If aForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And aForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                aForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
            End If
        End If
        oGrid.Columns.Item("U_Z_HREmpID").Click(oGrid.DataTable.Rows.Count - 1)
        oGrid.DataTable.SetValue("U_Z_Status", oGrid.DataTable.Rows.Count - 1, "P")
        oGrid.DataTable.SetValue("U_Z_AppStatus", oGrid.DataTable.Rows.Count - 1, "P")
        oGrid.DataTable.SetValue("U_Z_AppRequired", oGrid.DataTable.Rows.Count - 1, "N")
        oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
    End Sub

    Private Sub DeleteRow(ByVal aForm As SAPbouiCOM.Form)
        Select Case aForm.PaneLevel
            Case 4
                oGrid = aForm.Items.Item("57").Specific
        End Select
        If oApplication.SBO_Application.MessageBox("Do you want to delete the selected Request?", , "Continue", "Cancel") = 2 Then
            Exit Sub
        End If

        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.Rows.IsSelected(intRow) Then
                Dim otest As SAPbobsCOM.Recordset
                If oGrid.DataTable.GetValue("U_Z_AppRequired", intRow) = "Y" Then
                    If oApplication.SBO_Application.MessageBox("Selected Request is part of approval preocess. Do you want to delete the request  ?", , "Continue", "Cancel") = 2 Then
                        Exit Sub
                    End If
                End If

                otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otest.DoQuery("Update [@Z_HR_TRIN1] set Name=Name +'_XD' where Code='" & oGrid.DataTable.GetValue("Code", intRow) & "'")
                'oGrid.DataTable.Rows.Remove(intRow)
                oGrid.DataTable.Rows.Remove(intRow)
                oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
                Exit For
            End If
        Next
    End Sub

    Private Function FillDepartment(ByVal sform As SAPbouiCOM.Form, ByVal dept As String) As String
        Dim oSlpRS As SAPbobsCOM.Recordset
        Dim strdeptname As String
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select Code,Remarks from OUDP where Code='" & dept & "'")
        If oSlpRS.RecordCount > 0 Then
            strdeptname = oSlpRS.Fields.Item(1).Value
        End If
        Return strdeptname
    End Function
    Private Function ApprovedEmp(ByVal sform As SAPbouiCOM.Form, ByVal Empid As String) As Boolean
        Dim strFromEmpid As String
        Dim bGrid As SAPbouiCOM.Grid
        bGrid = sform.Items.Item("57").Specific
        For intloop As Integer = 0 To bGrid.DataTable.Rows.Count - 1
            strFromEmpid = bGrid.DataTable.GetValue("U_Z_HREmpID", intloop)
            If strFromEmpid = Empid Then
                Return True
            End If
        Next
    End Function
#Region "AddToUDT"
    Private Function AddToUDT(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim strTable, strEmpId, strCode, strType, strAccountCode, strqry, strDeptcode, strStatus, aMessage As String
        Dim strcount As Integer
        Dim dblValue As Double
        Dim dtFromDate, dtTodate, dt, AppEnddt As Date
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim oValidateRS, otemp2 As SAPbobsCOM.Recordset
        Dim otemp, otemp1, otemprs As SAPbobsCOM.Recordset
        oUserTable = oApplication.Company.UserTables.Item("Z_HR_TRIN1")
        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        dt = Now.Date
        oGrid = aForm.Items.Item("57").Specific
        strTable = "@Z_HR_TRIN1"
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strEmpId = oGrid.DataTable.GetValue("U_Z_HREmpID", intRow)
            If oUserTable.GetByKey(oGrid.DataTable.GetValue("Code", intRow)) Then
                oUserTable.Code = oGrid.DataTable.GetValue("Code", intRow)
                oUserTable.Name = oGrid.DataTable.GetValue("Code", intRow)
                oUserTable.UserFields.Fields.Item("U_Z_HREmpID").Value = strEmpId
                oUserTable.UserFields.Fields.Item("U_Z_Status").Value = oGrid.DataTable.GetValue("U_Z_AppStatus", intRow)
                oUserTable.UserFields.Fields.Item("U_Z_AppStatus").Value = oGrid.DataTable.GetValue("U_Z_AppStatus", intRow)
                oUserTable.UserFields.Fields.Item("U_Z_Remarks").Value = oGrid.DataTable.GetValue("U_Z_Remarks", intRow)
                If oUserTable.Update() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    strqry = "Update [@Z_HR_TRIN1] set U_Z_HRRegStatus='" & oGrid.DataTable.GetValue("U_Z_Status", intRow) & "' where Code='" & oGrid.DataTable.GetValue("Code", intRow) & "' and U_Z_HREmpID='" & strEmpId & "'"
                    otemprs.DoQuery(strqry)
                    If oGrid.DataTable.GetValue("U_Z_AppRequired", intRow) = "N" And oGrid.DataTable.GetValue("U_Z_AppStatus", intRow) = "A" Then
                        aMessage = "You have been Registered in training : " & oApplication.Utilities.getEdittextvalue(aForm, "1000003") & " . From Date :" & oApplication.Utilities.getEdittextvalue(aForm, "31")
                        aMessage += " Till Date : " & oApplication.Utilities.getEdittextvalue(aForm, "33")
                        oApplication.Utilities.SendMail_ApprovalRegTraining(aMessage, strEmpId)
                    End If
                End If
            Else
                strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                oUserTable.Code = strCode
                oUserTable.Name = strCode
                oUserTable.UserFields.Fields.Item("U_Z_HREmpID").Value = strEmpId
                strqry = "Select firstName,U_Z_HR_PosiCode,U_Z_HR_PosiName,dept from OHEM where empID='" & strEmpId & "'"
                oValidateRS.DoQuery(strqry)
                If oValidateRS.RecordCount > 0 Then
                    oUserTable.UserFields.Fields.Item("U_Z_HREmpName").Value = oValidateRS.Fields.Item("firstName").Value
                    oUserTable.UserFields.Fields.Item("U_Z_PosiCode").Value = oValidateRS.Fields.Item("U_Z_HR_PosiCode").Value
                    oUserTable.UserFields.Fields.Item("U_Z_PosiName").Value = oValidateRS.Fields.Item("U_Z_HR_PosiName").Value
                    oUserTable.UserFields.Fields.Item("U_Z_DeptCode").Value = oValidateRS.Fields.Item("dept").Value.ToString()
                End If
                oUserTable.UserFields.Fields.Item("U_Z_DeptName").Value = oGrid.DataTable.GetValue("U_Z_DeptName", intRow)
                oUserTable.UserFields.Fields.Item("U_Z_TrainCode").Value = oApplication.Utilities.getEdittextvalue(aForm, "13")
                oUserTable.UserFields.Fields.Item("U_Z_CourseCode").Value = oApplication.Utilities.getEdittextvalue(aForm, "17")
                oUserTable.UserFields.Fields.Item("U_Z_CourseName").Value = oApplication.Utilities.getEdittextvalue(aForm, "1000003")
                strqry = "Select U_Z_CourseTypeDesc,U_Z_Startdt,U_Z_Enddt,U_Z_MinAttendees,U_Z_MaxAttendees,U_Z_AppStdt,U_Z_AppEnddt,U_Z_InsName,U_Z_NoOfHours,"
                strqry = strqry & "U_Z_StartTime,U_Z_EndTime,U_Z_Sunday,U_Z_Monday,U_Z_Tuesday,U_Z_Wednesday,U_Z_Thursday,U_Z_Friday,U_Z_Saturday,U_Z_AttCost,"
                strqry = strqry & "U_Z_Active from [@Z_HR_OTRIN]  where U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(aForm, "13") & "'"
                otemprs.DoQuery(strqry)
                If otemprs.RecordCount > 0 Then
                    oUserTable.UserFields.Fields.Item("U_Z_CourseTypeDesc").Value = otemprs.Fields.Item("U_Z_CourseTypeDesc").Value
                    oUserTable.UserFields.Fields.Item("U_Z_Startdt").Value = otemprs.Fields.Item("U_Z_Startdt").Value
                    oUserTable.UserFields.Fields.Item("U_Z_Enddt").Value = otemprs.Fields.Item("U_Z_Enddt").Value
                    oUserTable.UserFields.Fields.Item("U_Z_MinAttendees").Value = otemprs.Fields.Item("U_Z_MinAttendees").Value
                    oUserTable.UserFields.Fields.Item("U_Z_MaxAttendees").Value = otemprs.Fields.Item("U_Z_MaxAttendees").Value
                    oUserTable.UserFields.Fields.Item("U_Z_AppStdt").Value = otemprs.Fields.Item("U_Z_AppStdt").Value
                    oUserTable.UserFields.Fields.Item("U_Z_AppEnddt").Value = otemprs.Fields.Item("U_Z_AppEnddt").Value
                    oUserTable.UserFields.Fields.Item("U_Z_InsName").Value = otemprs.Fields.Item("U_Z_InsName").Value
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_NoOfHours").Value = otemprs.Fields.Item("U_Z_NoOfHours").Value
                        oUserTable.UserFields.Fields.Item("U_Z_StartTime").Value = otemprs.Fields.Item("U_Z_StartTime").Value
                        oUserTable.UserFields.Fields.Item("U_Z_EndTime").Value = otemprs.Fields.Item("U_Z_EndTime").Value
                    Catch ex As Exception

                    End Try

                    oUserTable.UserFields.Fields.Item("U_Z_Sunday").Value = otemprs.Fields.Item("U_Z_Sunday").Value
                    oUserTable.UserFields.Fields.Item("U_Z_Monday").Value = otemprs.Fields.Item("U_Z_Monday").Value
                    oUserTable.UserFields.Fields.Item("U_Z_Tuesday").Value = otemprs.Fields.Item("U_Z_Tuesday").Value
                    oUserTable.UserFields.Fields.Item("U_Z_Wednesday").Value = otemprs.Fields.Item("U_Z_Wednesday").Value
                    oUserTable.UserFields.Fields.Item("U_Z_Thursday").Value = otemprs.Fields.Item("U_Z_Thursday").Value
                    oUserTable.UserFields.Fields.Item("U_Z_Friday").Value = otemprs.Fields.Item("U_Z_Friday").Value
                    oUserTable.UserFields.Fields.Item("U_Z_Saturday").Value = otemprs.Fields.Item("U_Z_Saturday").Value
                    oUserTable.UserFields.Fields.Item("U_Z_AttCost").Value = otemprs.Fields.Item("U_Z_AttCost").Value
                    oUserTable.UserFields.Fields.Item("U_Z_Active").Value = otemprs.Fields.Item("U_Z_Active").Value

                End If
                oUserTable.UserFields.Fields.Item("U_Z_Status").Value = oGrid.DataTable.GetValue("U_Z_AppStatus", intRow)
                oUserTable.UserFields.Fields.Item("U_Z_AppStatus").Value = oGrid.DataTable.GetValue("U_Z_AppStatus", intRow)
                oUserTable.UserFields.Fields.Item("U_Z_AppRequired").Value = "N"
                oUserTable.UserFields.Fields.Item("U_Z_Remarks").Value = oGrid.DataTable.GetValue("U_Z_Remarks", intRow)
                oUserTable.UserFields.Fields.Item("U_Z_ApplyDate").Value = dt
                If oUserTable.Add <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    strqry = "Update [@Z_HR_TRIN1] set U_Z_HRRegStatus='" & oGrid.DataTable.GetValue("U_Z_Status", intRow) & "' where Code='" & oGrid.DataTable.GetValue("Code", intRow) & "' and U_Z_HREmpID='" & strEmpId & "'"
                    otemprs.DoQuery(strqry)
                    If oGrid.DataTable.GetValue("U_Z_AppRequired", intRow) = "N" And oGrid.DataTable.GetValue("U_Z_AppStatus", intRow) = "A" Then
                        aMessage = "You have been Registered in training : " & oApplication.Utilities.getEdittextvalue(aForm, "1000003") & " From Date :" & oApplication.Utilities.getEdittextvalue(aForm, "31")
                        aMessage += " Till Date : " & oApplication.Utilities.getEdittextvalue(aForm, "33")
                        oApplication.Utilities.SendMail_ApprovalRegTraining(aMessage, strEmpId)
                    End If
                End If
            End If
        Next
        oUserTable = Nothing

        Dim ote As SAPbobsCOM.Recordset
        ote = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        ote.DoQuery("Delete from [@Z_HR_TRIN1] where Name like '%_XD'")
        Return True
    End Function
#End Region

    Private Sub reDrawForm(ByVal oForm As SAPbouiCOM.Form)
        Try
            oForm.Freeze(True)
            oForm.Items.Item("25").Width = oForm.Width - 30
            oForm.Items.Item("25").Height = oForm.Height - 160
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
        End Try
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_TrainReg Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)


                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "57" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    If pVal.Row >= 0 Then
                                        If oGrid.DataTable.GetValue("U_Z_AppRequired", pVal.Row) = "Y" Then
                                            oApplication.Utilities.Message("This request is part of approval process. You can not do any changes", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "57" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    If pVal.Row >= 0 Then
                                        If oGrid.DataTable.GetValue("U_Z_AppRequired", pVal.Row) = "Y" Then
                                            oApplication.Utilities.Message("This request is part of approval process. You can not do any changes", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    End If
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "57" And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    If pVal.Row >= 0 Then
                                        If oGrid.DataTable.GetValue("U_Z_AppRequired", pVal.Row) = "Y" Then
                                            oApplication.Utilities.Message("This request is part of approval process. You can not do any changes", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim strCode As String
                                If pVal.ItemUID = "3" And (oForm.PaneLevel = 2 Or oForm.PaneLevel = 3) Then
                                    If Validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                Select Case pVal.ItemUID
                                    Case "67"
                                        strCode = oApplication.Utilities.getEdittextvalue(oForm, "16")
                                        oApplication.Utilities.OpenMasterinLink(oForm, "AgendaCode", strCode)
                                        BubbleEvent = False
                                        Exit Sub
                                    Case "69"
                                        strCode = oApplication.Utilities.getEdittextvalue(oForm, "13")
                                        oApplication.Utilities.OpenMasterinLink(oForm, "AgendaCode", strCode)
                                        BubbleEvent = False
                                        Exit Sub
                                    Case "68"
                                        strCode = oApplication.Utilities.getEdittextvalue(oForm, "20")
                                        oApplication.Utilities.OpenMasterinLink(oForm, "Course", strCode)
                                        BubbleEvent = False
                                        Exit Sub
                                    Case "70"
                                        strCode = oApplication.Utilities.getEdittextvalue(oForm, "17")
                                        oApplication.Utilities.OpenMasterinLink(oForm, "Course", strCode)
                                        BubbleEvent = False
                                        Exit Sub
                                    Case "72"
                                        strCode = oApplication.Utilities.getEdittextvalue(oForm, "39")
                                        Dim ooBj As New clshrTrainner
                                        ooBj.ViewCandidate(strCode)
                                        BubbleEvent = False
                                        Exit Sub
                                    Case "71"
                                        oApplication.Utilities.OpenMasterinLink(oForm, "CourseType")
                                        BubbleEvent = False
                                        Exit Sub
                                End Select
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                reDrawForm(oForm)
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1000005" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 4
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "1000004" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 3
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "58" Then
                                    oForm.Freeze(True)
                                    AddRow(oForm)
                                    oForm.Freeze(False)
                                End If

                                If pVal.ItemUID = "60" Then
                                    oForm.Freeze(True)
                                    DeleteRow(oForm)
                                    oForm.Freeze(False)
                                End If

                                If pVal.ItemUID = "59" Then
                                    If oApplication.SBO_Application.MessageBox("Do you want to save the changes ? ", , "Yes", "No") = 2 Then
                                        Exit Sub
                                    End If
                                    If AddToUDT(oForm) = True Then
                                        oApplication.Utilities.Message("Operation Completed Successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                        oForm.Close()
                                        Exit Sub
                                    Else
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                Select Case pVal.ItemUID
                                    Case "3"
                                        oForm.Freeze(True)
                                        Dim AgendaCode As String
                                        oForm.PaneLevel = oForm.PaneLevel + 1
                                        Dim osta As SAPbouiCOM.StaticText
                                        osta = oForm.Items.Item("19").Specific
                                        osta.Caption = "Step " & oForm.PaneLevel & " of 3"
                                        If oForm.PaneLevel = 3 Then
                                            AgendaCode = oApplication.Utilities.getEdittextvalue(oForm, "16")
                                            PopulateAgenda(oForm, AgendaCode)
                                            ReceivedApplicants(oForm)
                                        End If
                                        oForm.Freeze(False)
                                    Case "4"
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = oForm.PaneLevel - 1
                                        oForm.Freeze(False)

                                End Select

                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1 As String
                                Dim sCHFL_ID, val, val2, val3 As String
                                Dim dtdate1, dtdate2 As String
                                Dim intChoice As Integer
                                Dim codebar As String
                                Try
                                    oCFLEvento = pVal
                                    sCHFL_ID = oCFLEvento.ChooseFromListUID
                                    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                    oCFL = oForm.ChooseFromLists.Item(sCHFL_ID)
                                    If (oCFLEvento.BeforeAction = False) Then
                                        Dim oDataTable As SAPbouiCOM.DataTable
                                        oDataTable = oCFLEvento.SelectedObjects
                                        intChoice = 0
                                        oForm.Freeze(True)

                                        If pVal.ItemUID = "16" Then
                                            val1 = oDataTable.GetValue("U_Z_TrainCode", 0)
                                            val = oDataTable.GetValue("U_Z_CourseCode", 0)
                                            val2 = oDataTable.GetValue("U_Z_CourseName", 0)
                                            Try
                                                dtdate2 = oDataTable.GetValue("U_Z_Enddt", 0)
                                                oApplication.Utilities.setEdittextvalue(oForm, "66", dtdate2)
                                            Catch ex As Exception
                                                oApplication.Utilities.setEdittextvalue(oForm, "66", "")
                                            End Try
                                            Try
                                                dtdate1 = oDataTable.GetValue("U_Z_Startdt", 0)
                                                oApplication.Utilities.setEdittextvalue(oForm, "64", dtdate1)
                                            Catch ex As Exception
                                                oApplication.Utilities.setEdittextvalue(oForm, "64", "")
                                            End Try
                                         
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "22", val2)
                                                oApplication.Utilities.setEdittextvalue(oForm, "20", val)
                                                oApplication.Utilities.setEdittextvalue(oForm, "16", val1)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "6" Then
                                            val = oDataTable.GetValue("DocEntry", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, pVal.ItemUID, val)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "57" And pVal.ColUID = "U_Z_HREmpID" Then
                                            Dim strdep, strqry As String
                                            Dim oTemp As SAPbobsCOM.Recordset
                                            oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                            val = oDataTable.GetValue("empID", 0)
                                            val1 = oDataTable.GetValue("firstName", 0)
                                            val2 = oDataTable.GetValue("dept", 0)
                                            strdep = FillDepartment(oForm, val2)
                                            oGrid = oForm.Items.Item("57").Specific
                                            If ApprovedEmp(oForm, val) = True Then
                                                oApplication.Utilities.Message("Employee already Apply for this training.Employee Id is :" & val, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                                BubbleEvent = False
                                                oForm.Freeze(False)
                                                Exit Sub
                                            End If
                                            oGrid.DataTable.SetValue("U_Z_HREmpName", pVal.Row, val1)
                                            oGrid.DataTable.SetValue("U_Z_DeptName", pVal.Row, strdep)
                                            oGrid.DataTable.SetValue(pVal.ColUID, pVal.Row, val)
                                        End If
                                        oForm.Freeze(False)
                                    End If
                                Catch ex As Exception
                                    ' oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    oForm.Freeze(False)
                                End Try

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
                Case mnu_hr_TrainReg
                    LoadForm()
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
