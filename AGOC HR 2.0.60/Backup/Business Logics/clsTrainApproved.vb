Public Class clsTrainApproved
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
    Private oFolder As SAPbouiCOM.Folder
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
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_AppAttendees) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_AppAttendees, frm_hr_AppAttendees)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.Title = "Training Process Overview"
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
        oApplication.Utilities.setUserDatabind(oForm, "67", "TraCode1")
        oForm.DataSources.UserDataSources.Add("TraCode2", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "68", "TraCode2")

        oForm.DataSources.UserDataSources.Add("date1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "31", "date1")
        oForm.DataSources.UserDataSources.Add("date2", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "33", "date2")
        oForm.DataSources.UserDataSources.Add("date3", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "35", "date3")
        oForm.DataSources.UserDataSources.Add("date4", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "37", "date4")


        oForm.PaneLevel = 1
        Dim osta As SAPbouiCOM.StaticText
        osta = oForm.Items.Item("19").Specific
        osta.Caption = "Step " & oForm.PaneLevel & " of 4"
        oForm.Items.Item("19").TextStyle = SAPbouiCOM.BoTextStyle.ts_BOLD
        oCombobox = oForm.Items.Item("73").Specific
        oCombobox.ValidValues.Add("O", "Open")
        oCombobox.ValidValues.Add("L", "Cancel")
        oCombobox.ValidValues.Add("C", "Close")
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
                oApplication.Utilities.Message("Enter Agenda Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
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
            oApplication.Utilities.setEdittextvalue(aform, "33", oTemp1.Fields.Item("U_Z_Enddt").Value)
            oApplication.Utilities.setEdittextvalue(aform, "29", oTemp1.Fields.Item("U_Z_MaxAttendees").Value)
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
        strqry = strqry & " U_Z_Status ,U_Z_Remarks,Code from [@Z_HR_TRIN1] where U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(aform, "13") & "' "
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
        ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"
        oGrid.Columns.Item("Code").TitleObject.Caption = "Code"
        oGrid.Columns.Item("Code").Visible = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oForm.Freeze(False)
    End Sub
    Private Sub ApprovedApplicants(ByVal aform As SAPbouiCOM.Form)
        oForm.Freeze(True)
        Dim strqry As String
        oGrid = oForm.Items.Item("60").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_2")
        strqry = "select U_Z_HREmpID,U_Z_HREmpName,U_Z_DeptName,"
        strqry = strqry & " case U_Z_Status when 'A' then 'Approved' when 'R' then 'Rejected' else 'Pending' end as U_Z_Status ,U_Z_AttCost,U_Z_AddionalCost,U_Z_ApproveRemarks,Code,U_Z_AttendeesStatus from [@Z_HR_TRIN1] where U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(aform, "13") & "' and U_Z_Status='A' "
        Dim orec As SAPbobsCOM.Recordset
        orec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        orec.DoQuery(strqry)
        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("U_Z_HREmpID").TitleObject.Caption = "Employee Id"
        oGrid.Columns.Item("U_Z_HREmpID").Editable = False
        oEditTextColumn = oGrid.Columns.Item("U_Z_HREmpID")
        ' oEditTextColumn.ChooseFromListUID = "CFL2"
        ' oEditTextColumn.ChooseFromListAlias = "empId"
        oEditTextColumn.LinkedObjectType = 171
        oGrid.Columns.Item("U_Z_HREmpName").TitleObject.Caption = "Employee Name"
        oGrid.Columns.Item("U_Z_HREmpName").Editable = False
        oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
        oGrid.Columns.Item("U_Z_DeptName").Editable = False
        oGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Approved Status"
        oGrid.Columns.Item("U_Z_Status").Editable = False
        oGrid.Columns.Item("U_Z_AttendeesStatus").TitleObject.Caption = "Attendees Training Status"
        oGrid.Columns.Item("U_Z_AttendeesStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        ocombo = oGrid.Columns.Item("U_Z_AttendeesStatus")
        ocombo.ValidValues.Add("D", "Dropped")
        ocombo.ValidValues.Add("C", "completed")
        ocombo.ValidValues.Add("F", "Failed")
        ocombo.ValidValues.Add("L", "Failed")

        ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oGrid.Columns.Item("U_Z_AttendeesStatus").Visible = False
        oGrid.Columns.Item("U_Z_AttCost").TitleObject.Caption = "Training Cost"
        oGrid.Columns.Item("U_Z_AddionalCost").TitleObject.Caption = "Additional Cost"
        oGrid.Columns.Item("U_Z_ApproveRemarks").TitleObject.Caption = "Remarks"
        oGrid.Columns.Item("Code").TitleObject.Caption = "Code"
        oGrid.Columns.Item("Code").Visible = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oForm.Freeze(False)
    End Sub
    Private Sub AbsenceApplicants(ByVal aform As SAPbouiCOM.Form)
        oForm.Freeze(True)
        Dim strqry As String
        oGrid = oForm.Items.Item("61").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_3")
        'strqry = "select U_Z_HREmpID,U_Z_HREmpName,U_Z_DeptName,"
        'strqry = strqry & "case U_Z_Status when 'A' then 'Approved' when 'R' then 'Rejected' else 'Pending' end as U_Z_Status ,U_Z_AbsenceDate,U_Z_TrainHours,Code,U_Z_AttendeesStatus,U_Z_AbsenceRemarks from [@Z_HR_TRIN1] where U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(aform, "13") & "' and U_Z_Status='A' "

        strqry = "select U_Z_HREmpID,U_Z_HREmpName,U_Z_DeptName,"
        strqry = strqry & "case U_Z_Status when 'A' then 'Approved' when 'R' then 'Rejected' else 'Pending' end as U_Z_Status ,U_Z_Date,U_Z_Hours,Code,U_Z_AttendeesStatus,U_Z_Remarks,U_Z_RefCode,Name from [@Z_HR_TRIN2] where U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(aform, "13") & "' "
        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("U_Z_HREmpID").TitleObject.Caption = "Employee Id"
        oGrid.Columns.Item("U_Z_HREmpID").Editable = True
        oEditTextColumn = oGrid.Columns.Item("U_Z_HREmpID")
        oEditTextColumn.LinkedObjectType = 171
        oGrid.Columns.Item("U_Z_HREmpName").TitleObject.Caption = "Employee Name"
        oGrid.Columns.Item("U_Z_HREmpName").Editable = False
        oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
        oGrid.Columns.Item("U_Z_DeptName").Editable = False
        oGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Approved Status"
        oGrid.Columns.Item("U_Z_Status").Editable = False
        oGrid.Columns.Item("U_Z_Status").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        ocombo = oGrid.Columns.Item("U_Z_Status")
        ocombo.ValidValues.Add("P", "Pending")
        ocombo.ValidValues.Add("A", "Approved")
        ocombo.ValidValues.Add("R", "Rejected")
        ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oGrid.Columns.Item("U_Z_AttendeesStatus").TitleObject.Caption = "Attendees Training Status"
        oGrid.Columns.Item("U_Z_AttendeesStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        ocombo = oGrid.Columns.Item("U_Z_AttendeesStatus")
        ocombo.ValidValues.Add("D", "Dropped")
        ocombo.ValidValues.Add("C", "Completed")
        ocombo.ValidValues.Add("F", "Failed")

        ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oGrid.Columns.Item("U_Z_AttendeesStatus").Visible = False
        oGrid.Columns.Item("U_Z_Date").TitleObject.Caption = "Date"
        oGrid.Columns.Item("U_Z_Hours").TitleObject.Caption = "No of Hours"
        oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Absence Remarks"
        oGrid.Columns.Item("Code").TitleObject.Caption = "Code"
        oGrid.Columns.Item("Code").Visible = False
        oGrid.Columns.Item("U_Z_RefCode").Visible = False
        oGrid.Columns.Item("Name").Visible = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oForm.Freeze(False)
    End Sub

    Private Function CostPosting(ByVal aform As SAPbouiCOM.Form) As Boolean
        Try
            Dim strqry, strTranCode, strCreditAc, strDebitAc As String
            Dim oRec, oRec1 As SAPbobsCOM.Recordset
            Dim oJE As SAPbobsCOM.JournalEntries
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oGrid = oForm.Items.Item("62").Specific
            strTranCode = oApplication.Utilities.getEdittextvalue(aform, "13")
            oRec.DoQuery("Select isnull(U_Z_CGLACC,'') 'Credit',isnull(U_Z_DGLACC,'') 'Debit' from [@Z_HR_OTRIN] where U_Z_TrainCode ='" & strTranCode & "'")
            If oRec.RecordCount > 0 Then
                If oRec.Fields.Item("Credit").Value = "" Then
                    oApplication.Utilities.Message("Credit account missing for selected Training Agenda...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    strCreditAc = oRec.Fields.Item(0).Value
                    strCreditAc = oApplication.Utilities.getSAPAccount(strCreditAc)
                End If
                If oRec.Fields.Item("Debit").Value = "" Then
                    oApplication.Utilities.Message("Debit account missing for selected Training Agenda...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    strDebitAc = oRec.Fields.Item(1).Value
                    strDebitAc = oApplication.Utilities.getSAPAccount(strDebitAc)
                End If
            End If

            strqry = "select Sum(U_Z_TotalCost) from [@Z_HR_TRIN1] where U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(aform, "13") & "' and U_Z_Status='A' and isnull(U_Z_JENO,'')='' "
            oRec1.DoQuery(strqry)
            Dim dblTotalCost As Double
            If oRec1.RecordCount > 0 Then
                dblTotalCost = oRec1.Fields.Item(0).Value
                If dblTotalCost > 0 Then
                    oJE = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries)
                    oJE.TaxDate = Now.Date
                    '  oJE.DueDate = Now.Date
                    oJE.Memo = "Cost posting for training agenda code -" & strTranCode
                    oJE.Lines.AccountCode = strCreditAc
                    oJE.Lines.Credit = dblTotalCost
                    oJE.Lines.Debit = 0
                    oJE.Lines.Add()
                    oJE.Lines.SetCurrentLine(1)
                    oJE.Lines.AccountCode = strDebitAc
                    oJE.Lines.Debit = dblTotalCost
                    oJE.Lines.Credit = 0
                    If oJE.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    Else
                        Dim strDoc As String
                        oApplication.Company.GetNewObjectCode(strDoc)
                        If oJE.GetByKey(CInt(strDoc)) Then
                            strqry = "Update [@Z_HR_TRIN1] set U_Z_JENO=" & oJE.JdtNum & " where U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(aform, "13") & "' and U_Z_Status='A' and isnull(U_Z_JENO,'')='' "
                            oRec.DoQuery(strqry)
                        End If

                    End If
                End If
            End If
            Return True

        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        End Try
    End Function

    Private Function ValidateClosing(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim strQry As String
        Dim oRec1 As SAPbobsCOM.Recordset
        oRec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

        strQry = "select Sum(""U_Z_TotalCost"") from ""@Z_HR_TRIN1"" where ""U_Z_TrainCode""='" & oApplication.Utilities.getEdittextvalue(aForm, "13") & "' and ""U_Z_Status""='A'  "
        oRec1.DoQuery(strqry)

        If oRec1.Fields.Item(0).Value > 0 Then
            strQry = "Select * from ""@Z_HR_TRIN1""  where ""U_Z_TrainCode""='" & oApplication.Utilities.getEdittextvalue(aForm, "13") & "' and ""U_Z_Status""='A' and isnull(""U_Z_JENO"",'')='' "
            oRec1.DoQuery(strQry)
            If oRec1.RecordCount > 0 Then
                Return True
            Else
                Return False
            End If
        End If
        Return False
    End Function
    Private Sub CloseApplicants(ByVal aform As SAPbouiCOM.Form)
        oForm.Freeze(True)
        Dim strqry As String
        oGrid = oForm.Items.Item("62").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_4")
        strqry = "select U_Z_HREmpID,U_Z_HREmpName,U_Z_DeptName,U_Z_TotalCost,U_Z_AttendeesStatus,U_Z_CloseRemarks"
        strqry = strqry & ",Code,U_Z_UpEmpTrain,U_Z_UpEmpComp,U_Z_JENO from [@Z_HR_TRIN1] where U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(aform, "13") & "' and U_Z_Status='A' "
        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("U_Z_HREmpID").TitleObject.Caption = "Employee Id"
        oGrid.Columns.Item("U_Z_HREmpID").Editable = False
        oEditTextColumn = oGrid.Columns.Item("U_Z_HREmpID")
        oEditTextColumn.LinkedObjectType = 171
        oGrid.Columns.Item("U_Z_HREmpName").TitleObject.Caption = "Employee Name"
        oGrid.Columns.Item("U_Z_HREmpName").Editable = False
        oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
        oGrid.Columns.Item("U_Z_DeptName").Editable = False
        oGrid.Columns.Item("U_Z_TotalCost").TitleObject.Caption = "Training Cost"
        oGrid.Columns.Item("U_Z_TotalCost").Editable = False
        oGrid.Columns.Item("U_Z_CloseRemarks").TitleObject.Caption = "Remarks"
        oGrid.Columns.Item("U_Z_UpEmpTrain").TitleObject.Caption = "Update Employee Training Profile"
        oGrid.Columns.Item("U_Z_UpEmpTrain").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        oGrid.Columns.Item("U_Z_UpEmpComp").TitleObject.Caption = "Update Employee Competence"
        oGrid.Columns.Item("U_Z_UpEmpComp").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        oGrid.Columns.Item("Code").TitleObject.Caption = "Code"
        oGrid.Columns.Item("Code").Visible = False

        oGrid.Columns.Item("U_Z_AttendeesStatus").TitleObject.Caption = "Attendees Training Status"
        oGrid.Columns.Item("U_Z_AttendeesStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        ocombo = oGrid.Columns.Item("U_Z_AttendeesStatus")
        ocombo.ValidValues.Add("D", "Dropped")
        ocombo.ValidValues.Add("C", "Completed")
        ocombo.ValidValues.Add("F", "Failed")
        ocombo.ValidValues.Add("L", "Cancel")
        ocombo.ValidValues.Add("W", "WithDraw")
        ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oGrid.Columns.Item("U_Z_JENO").TitleObject.Caption = "Journal Entry Number"
        oGrid.Columns.Item("U_Z_JENO").Editable = False
        oEditTextColumn = oGrid.Columns.Item("U_Z_JENO")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_JournalPosting

        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oForm.Freeze(False)
    End Sub
  
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
  
#Region "AddToUDT"
    Private Function AddToUDT(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim strTable, strEmpId, strCode, strType, strAccountCode, strqry, strDeptcode, strStatus As String
        Dim strcount As Integer
        Dim dblValue As Double
        Dim dtFromDate, dtTodate, dt, AppEnddt As Date
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim oValidateRS, otemp2 As SAPbobsCOM.Recordset
        Dim otemp, otemp1, otemprs As SAPbobsCOM.Recordset

        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        dt = Now.Date
        strTable = "@Z_HR_TRIN1"
        oUserTable = oApplication.Company.UserTables.Item("Z_HR_TRIN2")
        oGrid = aForm.Items.Item("61").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strEmpId = oGrid.DataTable.GetValue("U_Z_HREmpID", intRow)
            If strEmpId <> "" Then
                If oUserTable.GetByKey(oGrid.DataTable.GetValue("Code", intRow)) Then
                    oUserTable.Code = oGrid.DataTable.GetValue("Code", intRow)
                    oUserTable.Name = oGrid.DataTable.GetValue("Name", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_HREmpID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_HREmpName").Value = oGrid.DataTable.GetValue("U_Z_HREmpName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_TrainCode").Value = oApplication.Utilities.getEdittextvalue(aForm, "13")
                    oUserTable.UserFields.Fields.Item("U_Z_DeptName").Value = oGrid.DataTable.GetValue("U_Z_DeptName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_RefCode").Value = oGrid.DataTable.GetValue("U_Z_RefCode", intRow)
                    ' oUserTable.UserFields.Fields.Item("U_Z_AttendeesStatus").Value = oGrid.DataTable.GetValue("U_Z_AttendeesStatus", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Date").Value = oGrid.DataTable.GetValue("U_Z_Date", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Hours").Value = oGrid.DataTable.GetValue("U_Z_Hours", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Remarks").Value = oGrid.DataTable.GetValue("U_Z_Remarks", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Status").Value = "A"
                    If oUserTable.Update() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                Else
                    strCode = oApplication.Utilities.getMaxCode("@Z_HR_TRIN2", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_HREmpID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_HREmpName").Value = oGrid.DataTable.GetValue("U_Z_HREmpName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_DeptName").Value = oGrid.DataTable.GetValue("U_Z_DeptName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_RefCode").Value = oGrid.DataTable.GetValue("U_Z_RefCode", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_TrainCode").Value = oApplication.Utilities.getEdittextvalue(aForm, "13")
                    'oUserTable.UserFields.Fields.Item("U_Z_AttendeesStatus").Value = oGrid.DataTable.GetValue("U_Z_AttendeesStatus", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Date").Value = oGrid.DataTable.GetValue("U_Z_Date", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Hours").Value = oGrid.DataTable.GetValue("U_Z_Hours", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Remarks").Value = oGrid.DataTable.GetValue("U_Z_Remarks", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Status").Value = "A"
                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
            End If
        Next

        oUserTable = oApplication.Company.UserTables.Item("Z_HR_TRIN1")

        oGrid = aForm.Items.Item("60").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strEmpId = oGrid.DataTable.GetValue("U_Z_HREmpID", intRow)
            If oUserTable.GetByKey(oGrid.DataTable.GetValue("Code", intRow)) Then
                oUserTable.Code = oGrid.DataTable.GetValue("Code", intRow)
                oUserTable.Name = oGrid.DataTable.GetValue("Code", intRow)
                oUserTable.UserFields.Fields.Item("U_Z_HREmpID").Value = strEmpId
                oUserTable.UserFields.Fields.Item("U_Z_AttCost").Value = oGrid.DataTable.GetValue("U_Z_AttCost", intRow)
                oUserTable.UserFields.Fields.Item("U_Z_AddionalCost").Value = oGrid.DataTable.GetValue("U_Z_AddionalCost", intRow)
                oUserTable.UserFields.Fields.Item("U_Z_ApproveRemarks").Value = oGrid.DataTable.GetValue("U_Z_ApproveRemarks", intRow)
                oUserTable.UserFields.Fields.Item("U_Z_AttendeesStatus").Value = oGrid.DataTable.GetValue("U_Z_AttendeesStatus", intRow)
                If oUserTable.Update() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            End If
        Next

       

        oGrid = aForm.Items.Item("62").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strEmpId = oGrid.DataTable.GetValue("U_Z_HREmpID", intRow)
            If oUserTable.GetByKey(oGrid.DataTable.GetValue("Code", intRow)) Then
                oUserTable.Code = oGrid.DataTable.GetValue("Code", intRow)
                oUserTable.Name = oGrid.DataTable.GetValue("Code", intRow)
                oUserTable.UserFields.Fields.Item("U_Z_HREmpID").Value = strEmpId
                oUserTable.UserFields.Fields.Item("U_Z_TotalCost").Value = oGrid.DataTable.GetValue("U_Z_TotalCost", intRow)
                If oGrid.DataTable.GetValue("U_Z_UpEmpTrain", intRow) = "" Then
                    oUserTable.UserFields.Fields.Item("U_Z_UpEmpTrain").Value = "N"
                Else
                    oUserTable.UserFields.Fields.Item("U_Z_UpEmpTrain").Value = "Y"
                End If
                If oGrid.DataTable.GetValue("U_Z_UpEmpComp", intRow) = "" Then
                    oUserTable.UserFields.Fields.Item("U_Z_UpEmpComp").Value = "N"
                Else
                    oUserTable.UserFields.Fields.Item("U_Z_UpEmpComp").Value = "Y"
                End If

                oUserTable.UserFields.Fields.Item("U_Z_CloseRemarks").Value = oGrid.DataTable.GetValue("U_Z_CloseRemarks", intRow)
                If oUserTable.Update() <> 0 Then
                    MessageBox.Show(oApplication.Company.GetLastErrorDescription)
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            End If
        Next

        oUserTable = Nothing
        Dim ote As SAPbobsCOM.Recordset
        ote = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        ote.DoQuery("Delete from [@Z_HR_TRIN2] where Name like '%_XD'")
        Return True
    End Function
#End Region

    Private Sub updateadditionalcost(ByVal aGrid As SAPbouiCOM.Grid, ByVal bGrid As SAPbouiCOM.Grid, ByVal aStartRow As Integer, ByVal aEndrow As Integer)
        Dim strSourceCode, strDesgCode As String
        For intRow As Integer = aStartRow To aEndrow
            strSourceCode = aGrid.DataTable.GetValue("Code", intRow)
            For intloop As Integer = 0 To bGrid.DataTable.Rows.Count - 1
                strDesgCode = bGrid.DataTable.GetValue("Code", intloop)
                If strSourceCode = strDesgCode Then
                    bGrid.DataTable.SetValue("U_Z_TotalCost", intloop, aGrid.DataTable.GetValue("U_Z_AttCost", intRow) + aGrid.DataTable.GetValue("U_Z_AddionalCost", intRow))
                    Exit For
                End If
            Next
        Next
    End Sub

    Private Function UpdateEmployeeProfile(ByVal aform As SAPbouiCOM.Form) As Boolean
        Try
            Dim strqry, strqry1, strqry2, strTranCode, strCourseCode As String
            Dim oRec, oRec1, oRec2, oTemp As SAPbobsCOM.Recordset
            Dim oJE As SAPbobsCOM.JournalEntries
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRec2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oGrid = oForm.Items.Item("62").Specific
            Dim strTable, strCode As String
            Dim oUserTable, oCompTable, ObjLoanTable As SAPbobsCOM.UserTable
            oCompTable = oApplication.Company.UserTables.Item("Z_HR_ECOLVL")
            strTable = "@Z_HR_ECOLVL"
            strTranCode = oApplication.Utilities.getEdittextvalue(aform, "13")
            strCourseCode = oApplication.Utilities.getEdittextvalue(aform, "17")
            If oGrid.DataTable.Rows.Count > 0 Then
                For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                    If oGrid.DataTable.GetValue("U_Z_AttendeesStatus", intRow) = "C" And oGrid.DataTable.GetValue("U_Z_UpEmpComp", intRow) = "Y" Then
                        strqry1 = "SELECT * FROM ""@Z_HR_COUR3""  T0  left Join ""@Z_HR_OCOUR""  T1 on T0.""DocEntry""=T1.""DocEntry"" where T1.""U_Z_CourseCode"" ='" & strCourseCode & "'"
                        oRec1.DoQuery(strqry1)
                        If oRec1.RecordCount > 0 Then
                            For intLoop As Integer = 0 To oRec1.RecordCount - 1
                                strqry2 = "SELECT * FROM ""@Z_HR_ECOLVL"" where ""U_Z_HREmpID"" ='" & oGrid.DataTable.GetValue("U_Z_HREmpID", intRow) & "' and ""U_Z_CompCode""='" & oRec1.Fields.Item("U_Z_CompCode").Value & "'"
                                oRec2.DoQuery(strqry2)
                                If oRec2.RecordCount > 0 Then
                                    strqry = "Update ""@Z_HR_ECOLVL"" set ""U_Z_CompLevel""='" & oRec1.Fields.Item("U_Z_CompLevel").Value & "' where ""Code""='" & oRec2.Fields.Item("Code").Value & "'"
                                    oTemp.DoQuery(strqry)
                                Else
                                    oTemp.DoQuery("Select * from ""@Z_HR_OCOMP"" where ""U_Z_CompCode""='" & oRec1.Fields.Item("U_Z_CompCode").Value & "'")
                                    If oTemp.RecordCount > 0 Then


                                        strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                                        oCompTable.Code = strCode
                                        oCompTable.Name = strCode
                                        oCompTable.UserFields.Fields.Item("U_Z_HREmpID").Value = oGrid.DataTable.GetValue("U_Z_HREmpID", intRow)
                                        oCompTable.UserFields.Fields.Item("U_Z_CompCode").Value = oRec1.Fields.Item("U_Z_CompCode").Value
                                        oCompTable.UserFields.Fields.Item("U_Z_CompName").Value = oRec1.Fields.Item("U_Z_CompDesc").Value
                                        oCompTable.UserFields.Fields.Item("U_Z_Weight").Value = oTemp.Fields.Item("U_Z_Weight").Value
                                        oCompTable.UserFields.Fields.Item("U_Z_CompLevel").Value = oRec1.Fields.Item("U_Z_CompLevel").Value
                                        If oCompTable.Add <> 0 Then
                                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            Return False
                                        End If
                                    End If
                                End If
                                oRec1.MoveNext()
                            Next
                        End If
                    Else
                        Return False
                    End If
                Next
            End If


            Return True

        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        End Try
    End Function

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_AppAttendees Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
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
                                    Case "74"
                                        strCode = oApplication.Utilities.getEdittextvalue(oForm, "16")
                                        oApplication.Utilities.OpenMasterinLink(oForm, "AgendaCode", strCode)
                                        BubbleEvent = False
                                        Exit Sub
                                    Case "76"
                                        strCode = oApplication.Utilities.getEdittextvalue(oForm, "13")
                                       oApplication.Utilities.OpenMasterinLink(oForm, "AgendaCode", strCode)
                                        BubbleEvent = False
                                        Exit Sub
                                    Case "75"
                                        strCode = oApplication.Utilities.getEdittextvalue(oForm, "20")
                                        oApplication.Utilities.OpenMasterinLink(oForm, "Course", strCode)
                                        BubbleEvent = False
                                        Exit Sub
                                    Case "77"
                                        strCode = oApplication.Utilities.getEdittextvalue(oForm, "17")
                                        oApplication.Utilities.OpenMasterinLink(oForm, "Course", strCode)
                                        BubbleEvent = False
                                        Exit Sub
                                    Case "79"
                                        strCode = oApplication.Utilities.getEdittextvalue(oForm, "39")
                                        Dim ooBj As New clshrTrainner
                                        ooBj.ViewCandidate(strCode)
                                        BubbleEvent = False
                                        Exit Sub
                                    Case "78"
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
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                                If pVal.ItemUID = "61" And pVal.ColUID = "U_Z_HREmpID" And pVal.CharPressed = 9 Then
                                    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                    Dim strIns As String
                                    oGrid = oForm.Items.Item("61").Specific

                                    strIns = oGrid.DataTable.GetValue("U_Z_HREmpID", pVal.Row)
                                    Dim otest As SAPbobsCOM.Recordset
                                    otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    otest.DoQuery("Select * from [@Z_HR_TRIN1] where U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(oForm, "13") & "' and  U_Z_HREmpID='" & strIns & "' and U_Z_Status='A'")
                                    If otest.RecordCount <= 0 Then
                                        oGrid.DataTable.SetValue("U_Z_HREmpID", pVal.Row, "")
                                        strIns = ""
                                    Else
                                        oForm.Freeze(True)
                                        oGrid.DataTable.SetValue("U_Z_HREmpName", pVal.Row, otest.Fields.Item("U_Z_HREmpName").Value)
                                        oGrid.DataTable.SetValue("U_Z_DeptName", pVal.Row, otest.Fields.Item("U_Z_DeptName").Value)
                                        oGrid.DataTable.SetValue("U_Z_Status", pVal.Row, otest.Fields.Item("U_Z_Status").Value)
                                        oForm.Freeze(False)
                                        Exit Sub
                                    End If
                                    Dim objChooseForm As SAPbouiCOM.Form
                                    Dim objChoose As New clsChooseFromList
                                    clsChooseFromList.ItemUID = pVal.ItemUID
                                    clsChooseFromList.SourceFormUID = FormUID
                                    clsChooseFromList.SourceLabel = 0
                                    clsChooseFromList.CFLChoice = "Training" 'oCombo.Selected.Value
                                    clsChooseFromList.choice = "Bin"
                                    clsChooseFromList.Documentchoice = "Training"
                                    clsChooseFromList.ItemCode = oApplication.Utilities.getEdittextvalue(oForm, "13")
                                    ' clsChooseFromList.BinDescrUID = "BinToBinHeader"
                                    clsChooseFromList.sourceColumID = pVal.ColUID
                                    clsChooseFromList.SourceLabel = pVal.Row
                                    oApplication.Utilities.LoadForm("\CFL.xml", frm_ChoosefromList)
                                    objChooseForm = oApplication.SBO_Application.Forms.ActiveForm()
                                    objChoose.databound(objChooseForm)
                                End If

                                If pVal.ItemUID = "60" And (pVal.ColUID = "U_Z_AttCost" Or pVal.ColUID = "U_Z_AdditionalCost") And pVal.CharPressed = 9 Then
                                    Dim aGrid, bgrid As SAPbouiCOM.Grid
                                    aGrid = oForm.Items.Item("60").Specific
                                    bgrid = oForm.Items.Item("62").Specific
                                    updateadditionalcost(aGrid, bgrid, pVal.Row, pVal.Row)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "69" Then
                                    If oApplication.SBO_Application.MessageBox("Do you want to continue the Cost Posting...?", , "Contine", "Cancel") = 2 Then
                                    Else
                                        If CostPosting(oForm) = True Then
                                            oApplication.Utilities.Message("Cost posting completed successfully...", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                            If UpdateEmployeeProfile(oForm) = True Then

                                            End If
                                            CloseApplicants(oForm)
                                        End If
                                    End If
                                End If

                                If pVal.ItemUID = "70" Then
                                    If oApplication.SBO_Application.MessageBox("Do you want to continue the to update employee profile...?", , "Contine", "Cancel") = 2 Then
                                    Else
                                        If UpdateEmployeeProfile(oForm) = True Then
                                            oApplication.Utilities.Message("Employee Profile updating completed successfully...", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                            ' CloseApplicants(oForm)
                                        End If
                                    End If
                                End If
                                If pVal.ItemUID = "71" Then
                                    If oApplication.SBO_Application.MessageBox("Do you want to Close the Traing Agenda...?", , "Yes", "NO") = 2 Then
                                        Exit Sub
                                    Else
                                        If ValidateClosing(oForm) = True Then
                                            oApplication.Utilities.Message("Cost posting not done. Please do the Cost posting and close the training agenda", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            Exit Sub
                                        End If
                                        Dim strTranCode As String
                                        Dim oClosRs As SAPbobsCOM.Recordset
                                        oClosRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                        strTranCode = oApplication.Utilities.getEdittextvalue(oForm, "13")
                                        oClosRs.DoQuery("Select * from [@Z_HR_TRIN1] where U_Z_TrainCode='" & strTranCode & "' and U_Z_Status='A' and isnull(U_Z_JENO,'')=''")
                                        If oClosRs.RecordCount > 0 Then
                                            If oApplication.SBO_Application.MessageBox("Cost posting not completed. Do you want to close the Training Agenda?.", , "Yes", "NO") = 2 Then
                                                Exit Sub
                                            End If
                                        End If
                                        If strTranCode <> "" Then
                                            oClosRs.DoQuery("Update [@Z_HR_OTRIN] set U_Z_Status='C' where U_Z_TrainCode='" & strTranCode & "'")
                                            oClosRs.DoQuery("Update [@Z_HR_TRIN1] set U_Z_ClosingDate=getdate() ,U_Z_Closeby='" & oApplication.Company.UserName & "' where U_Z_TrainCode='" & strTranCode & "'")
                                            oForm.Close()
                                        End If
                                    End If
                                End If
                                If pVal.ItemUID = "63" Then
                                    oGrid = oForm.Items.Item("61").Specific
                                    If oGrid.DataTable.GetValue("U_Z_HREmpID", oGrid.DataTable.Rows.Count - 1) <> "" Then
                                        oGrid.DataTable.Rows.Add()
                                    End If
                                End If

                                If pVal.ItemUID = "64" Then
                                    oGrid = oForm.Items.Item("61").Specific
                                    For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                        If oGrid.Rows.IsSelected(intRow) Then
                                            Dim otest As SAPbobsCOM.Recordset
                                            otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                            otest.DoQuery("Update [@Z_HR_TRIN2] set Name=Name +'_XD' where Code='" & oGrid.DataTable.GetValue("Code", intRow) & "'")
                                            oGrid.DataTable.Rows.Remove(intRow)
                                            Exit For
                                        End If
                                    Next
                                End If


                                If pVal.ItemUID = "63" Then
                                    oGrid = oForm.Items.Item("61").Specific
                                    If oGrid.DataTable.GetValue("U_Z_HREmpID", oGrid.DataTable.Rows.Count - 1) <> "" Then
                                        oGrid.DataTable.Rows.Add()
                                    End If
                                End If
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
                                If pVal.ItemUID = "1000006" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 5
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "23" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 6
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "24" Then
                                    oForm.Freeze(True)
                                    Dim aGrid, bgrid As SAPbouiCOM.Grid
                                    aGrid = oForm.Items.Item("60").Specific
                                    bgrid = oForm.Items.Item("62").Specific
                                    updateadditionalcost(aGrid, bgrid, 0, aGrid.DataTable.Rows.Count - 1)
                                    oCombobox = oForm.Items.Item("73").Specific
                                    If oCombobox.Selected.Value <> "O" Then
                                        oForm.Items.Item("69").Enabled = False
                                        oForm.Items.Item("70").Enabled = False
                                        oForm.Items.Item("71").Enabled = False
                                    Else
                                        oForm.Items.Item("69").Enabled = True
                                        oForm.Items.Item("70").Enabled = True
                                        oForm.Items.Item("71").Enabled = True
                                    End If
                                    oForm.PaneLevel = 7
                                    oForm.Freeze(False)
                                End If

                                If pVal.ItemUID = "59" Then
                                    If oApplication.SBO_Application.MessageBox("Do you want to save the changes ? ", , "Yes", "No") = 2 Then
                                        Exit Sub
                                    End If
                                    oGrid = oForm.Items.Item("61").Specific
                                    Dim strdate As String
                                    For introw As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                        If oGrid.DataTable.GetValue("U_Z_HREmpID", introw) <> "" Then
                                            strdate = oGrid.DataTable.GetValue("U_Z_Date", introw)
                                            If strdate = "" Then
                                                oApplication.Utilities.Message("Enter Absense Date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                                Exit Sub
                                            End If
                                        End If
                                    Next
                                    If AddToUDT(oForm) = True Then
                                        oApplication.Utilities.Message("Operation Completed Successfully...", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
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
                                            ApprovedApplicants(oForm)
                                            AbsenceApplicants(oForm)
                                            CloseApplicants(oForm)
                                            Dim otest As SAPbobsCOM.Recordset
                                            otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                            otest.DoQuery("Select isnull(U_Z_Status,'O') from [@Z_HR_OTRIN] where U_Z_TrainCode='" & AgendaCode & "'")
                                            If otest.Fields.Item(0).Value <> "O" Then
                                                oForm.Items.Item("59").Enabled = False
                                                oForm.Items.Item("63").Enabled = False
                                                oForm.Items.Item("64").Enabled = False
                                            Else
                                                oForm.Items.Item("59").Enabled = True
                                                oForm.Items.Item("63").Enabled = True
                                                oForm.Items.Item("64").Enabled = True
                                            End If
                                            oForm.Items.Item("1000004").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                        End If
                                        'If oForm.PaneLevel = 7 Then
                                        '    oFolder = oForm.Items.Item("24").Specific
                                        '    oFolder.AutoPaneSelection = True
                                        '    CloseApplicants(oForm)
                                        'End If
                                        oForm.Freeze(False)
                                    Case "4"
                                        oForm.Freeze(True)
                                        If oForm.PaneLevel <> 2 Then
                                            oForm.PaneLevel = 2
                                        Else
                                            oForm.PaneLevel = oForm.PaneLevel - 1
                                        End If
                                        oForm.Freeze(False)


                                End Select

                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1 As String
                                Dim sCHFL_ID, val, val2, val3, val4 As String
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
                                        Dim dtdate1, dtdate2 As String
                                        If pVal.ItemUID = "16" Then
                                            val1 = oDataTable.GetValue("U_Z_TrainCode", 0)
                                            val = oDataTable.GetValue("U_Z_CourseCode", 0)
                                            val2 = oDataTable.GetValue("U_Z_CourseName", 0)
                                            val4 = oDataTable.GetValue("U_Z_Status", 0)
                                            Try
                                                dtdate1 = oDataTable.GetValue("U_Z_Startdt", 0)
                                                oApplication.Utilities.setEdittextvalue(oForm, "67", dtdate1)
                                            Catch ex As Exception
                                                oApplication.Utilities.setEdittextvalue(oForm, "67", "")
                                            End Try
                                            Try
                                                dtdate2 = oDataTable.GetValue("U_Z_Enddt", 0)
                                                oApplication.Utilities.setEdittextvalue(oForm, "68", dtdate2)
                                            Catch ex As Exception
                                                oApplication.Utilities.setEdittextvalue(oForm, "68", "")
                                            End Try
                                            Try
                                                oCombobox = oForm.Items.Item("73").Specific
                                                oCombobox.Select(val4, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                                oApplication.Utilities.setEdittextvalue(oForm, "22", val2)
                                                oApplication.Utilities.setEdittextvalue(oForm, "20", val)
                                                oApplication.Utilities.setEdittextvalue(oForm, "16", val1)
                                            Catch ex As Exception
                                            End Try
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
                Case mnu_hr_AppAttendees
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
