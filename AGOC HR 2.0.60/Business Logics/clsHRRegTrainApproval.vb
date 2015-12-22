Public Class clsHRRegTrainApproval
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
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_HRRegTrainApproval) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_HRRegTrainApproval, frm_hr_HRRegTrainApproval)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        '  oForm.Title = "Training Registration and Approval"
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
        oForm.DataSources.UserDataSources.Add("Status", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDSCombobox(oForm, "63", "Status")
        oCombobox = oForm.Items.Item("63").Specific
        oCombobox.ValidValues.Add("O", "Open")
        oCombobox.ValidValues.Add("C", "Closed")
        oCombobox.ValidValues.Add("L", "Canceled")
        oForm.Items.Item("63").DisplayDesc = True
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
        Dim strqry, strConiditon As String
        oGrid = oForm.Items.Item("57").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_1")
        'Dim strmanager As String = oApplication.Utilities.getloggedonuser()
        'Dim strEmpList As String = oApplication.Utilities.getEmpIDforMangers(strmanager)
        'If strEmpList <> "" Then
        '    strConiditon = " U_Z_HREmpID in (" & strEmpList & ")"
        'Else
        '    strConiditon = " 1=1 "
        'End If
        strqry = "select U_Z_HREmpID,U_Z_HREmpName,U_Z_DeptName,"
        strqry = strqry & " U_Z_Status ,U_Z_Remarks,U_Z_MgrRegStatus,U_Z_MgrRegRemarks,U_Z_HRRegStatus,U_Z_HrRegRemarks,Code from [@Z_HR_TRIN1] where U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(aform, "13") & "'"
        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("U_Z_HREmpID").TitleObject.Caption = "Employee Id"
        oGrid.Columns.Item("U_Z_HREmpID").Editable = False
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
        oGrid.Columns.Item("U_Z_Status").Editable = False
        oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"
        oGrid.Columns.Item("U_Z_Remarks").Editable = False
        oGrid.Columns.Item("U_Z_MgrRegStatus").TitleObject.Caption = "First Level Approval Status"
        oGrid.Columns.Item("U_Z_MgrRegStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        ocombo = oGrid.Columns.Item("U_Z_MgrRegStatus")
        ocombo.ValidValues.Add("P", "Pending")
        ocombo.ValidValues.Add("A", "Approved")
        ocombo.ValidValues.Add("R", "Rejected")
        ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oGrid.Columns.Item("U_Z_MgrRegStatus").Editable = False
        oGrid.Columns.Item("U_Z_MgrRegRemarks").TitleObject.Caption = "First Level Remarks"
        oGrid.Columns.Item("U_Z_MgrRegRemarks").Editable = False
        oCombobox = aform.Items.Item("63").Specific
        If oCombobox.Selected.Value = "O" Then
            oGrid.Columns.Item("U_Z_HRRegStatus").TitleObject.Caption = "HR Status"
            oGrid.Columns.Item("U_Z_HRRegStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_HRRegStatus")
            ocombo.ValidValues.Add("P", "Pending")
            ocombo.ValidValues.Add("A", "Approved")
            ocombo.ValidValues.Add("R", "Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_HRRegStatus").Editable = True
            oGrid.Columns.Item("U_Z_HrRegRemarks").TitleObject.Caption = "HR Remarks"
            oGrid.Columns.Item("U_Z_HrRegRemarks").Editable = True
        Else
            oGrid.Columns.Item("U_Z_HRRegStatus").TitleObject.Caption = "HR Status"
            oGrid.Columns.Item("U_Z_HRRegStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_HRRegStatus")
            ocombo.ValidValues.Add("P", "Pending")
            ocombo.ValidValues.Add("A", "Approved")
            ocombo.ValidValues.Add("R", "Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_HRRegStatus").Editable = False
            oGrid.Columns.Item("U_Z_HrRegRemarks").TitleObject.Caption = "HR Remarks"
            oGrid.Columns.Item("U_Z_HrRegRemarks").Editable = False
        End If
      
        oGrid.Columns.Item("Code").TitleObject.Caption = "Code"
        oGrid.Columns.Item("Code").Visible = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oForm.Freeze(False)
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
        Dim strTable, strEmpId, strCode, strType, strAccountCode, strqry, strDeptcode, strStatus As String
        Dim dtFromDate, dtTodate, dt, AppEnddt As Date

        Dim otemp, otemp1, otemprs As SAPbobsCOM.Recordset
        otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        dt = Now.Date
        oGrid = aForm.Items.Item("57").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strEmpId = oGrid.DataTable.GetValue("U_Z_HREmpID", intRow)
            ' strCode = oGrid.DataTable.GetValue("DocEntry", intRow)
            strqry = "Update [@Z_HR_TRIN1] set U_Z_Status='" & oGrid.DataTable.GetValue("U_Z_HRRegStatus", intRow) & "' , U_Z_HRRegStatus='" & oGrid.DataTable.GetValue("U_Z_HRRegStatus", intRow) & "',U_Z_HrRegRemarks='" & oGrid.DataTable.GetValue("U_Z_HrRegRemarks", intRow) & "' where U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(aForm, "13") & "' and U_Z_HREmpID='" & strEmpId & "'"
            otemprs.DoQuery(strqry)
        Next
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
            If pVal.FormTypeEx = frm_hr_HRRegTrainApproval Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" And (oForm.PaneLevel = 2 Or oForm.PaneLevel = 3) Then
                                    If Validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                Dim strCode As String
                                Select Case pVal.ItemUID
                                    Case "1000007"
                                        strCode = oApplication.Utilities.getEdittextvalue(oForm, "16")
                                        Dim ooBj As New clshrTrainPlan
                                        ooBj.LoadForm1(strCode)
                                        BubbleEvent = False
                                        Exit Sub
                                    Case "65"
                                        strCode = oApplication.Utilities.getEdittextvalue(oForm, "13")
                                        Dim ooBj As New clshrTrainPlan
                                        ooBj.LoadForm1(strCode)
                                        BubbleEvent = False
                                        Exit Sub
                                    Case "64"
                                        strCode = oApplication.Utilities.getEdittextvalue(oForm, "20")
                                        Dim ooBj As New clshrCourse
                                        ooBj.LoadForm1(strCode)
                                        BubbleEvent = False
                                        Exit Sub
                                    Case "66"
                                        strCode = oApplication.Utilities.getEdittextvalue(oForm, "17")
                                        Dim ooBj As New clshrCourse
                                        ooBj.LoadForm1(strCode)
                                        BubbleEvent = False
                                        Exit Sub
                                    Case "68"
                                       
                                        oApplication.Utilities.OpenMasterinLink(oForm, "CourseType")
                                    Case "67"
                                        strCode = oApplication.Utilities.getEdittextvalue(oForm, "39")
                                        Dim ooBj As New clshrTrainner
                                        ooBj.ViewCandidate(strCode)
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
                                If pVal.ItemUID = "59" Then
                                    If oApplication.SBO_Application.MessageBox("Do you want to save the changes ? ", , "Yes", "No") = 2 Then
                                        Exit Sub
                                    End If
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
                                            oForm.Items.Item("1000004").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                            oCombobox = oForm.Items.Item("63").Specific
                                            If oCombobox.Selected.Value <> "O" Then
                                                oForm.Items.Item("59").Enabled = False
                                            Else
                                                oForm.Items.Item("59").Enabled = True
                                            End If
                                        End If
                                        oForm.Freeze(False)
                                    Case "4"
                                        oForm.Freeze(True)
                                        If oForm.PaneLevel >= 3 Then
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
                                Dim sCHFL_ID, val, val2, val3 As String
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
                                            val3 = oDataTable.GetValue("U_Z_Status", 0)
                                            Try
                                                oCombobox = oForm.Items.Item("63").Specific
                                                oCombobox.Select(val3, SAPbouiCOM.BoSearchKey.psk_ByDescription)
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
                Case mnu_hr_HRRegTrainApproval
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
