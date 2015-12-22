Public Class clshrRecClosing
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private ocombo, ocombo1 As SAPbouiCOM.ComboBoxColumn
    Private oGrid, oGrid_P1, oGrid_P2, oGrid_P3 As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Private strQuery As String

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub

    Public Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_RecClosing) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_RecClosing, frm_hr_RecClosing)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        AddChooseFromList(oForm)
        oForm.DataSources.UserDataSources.Add("Reqno", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "5", "Reqno")
        oEditText = oForm.Items.Item("5").Specific
        oEditText.ChooseFromListUID = "CFL3"
        oEditText.ChooseFromListAlias = "DocEntry"
        oForm.DataSources.UserDataSources.Add("toReqno", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "46", "toReqno")
        oEditText = oForm.Items.Item("46").Specific
        oEditText.ChooseFromListUID = "CFL1"
        oEditText.ChooseFromListAlias = "DocEntry"
        oForm.DataSources.UserDataSources.Add("poscode", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "41", "poscode")
        oEditText = oForm.Items.Item("41").Specific
        oEditText.ChooseFromListUID = "CFL2"
        oEditText.ChooseFromListAlias = "U_Z_PosCode"
        oForm.DataSources.UserDataSources.Add("HP3", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "42", "HP3")
        ' Gridbind(oForm)
        oForm.PaneLevel = 1
        reDrawScreen(oForm)
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
            oCFLCreationParams.ObjectType = "Z_HR_ORREQS"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)

            'oCons = oCFL.GetConditions()
            'oCon = oCons.Add()
            'oCon.Alias = "U_Z_MgrStatus"
            'oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            'oCon.CondVal = "HA"
            'oCFL.SetConditions(oCons)
            'oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_OPOSIN"
            oCFLCreationParams.UniqueID = "CFL2"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_ORREQS"
            oCFLCreationParams.UniqueID = "CFL3"
            oCFL = oCFLs.Add(oCFLCreationParams)

            'oCons = oCFL.GetConditions()
            'oCon = oCons.Add()
            'oCon.Alias = "U_Z_MgrStatus"
            'oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            'oCon.CondVal = "HA"
            'oCFL.SetConditions(oCons)
            'oCon = oCons.Add()


        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim strDept, Reqno, posname As String
            Reqno = oApplication.Utilities.getEdittextvalue(aForm, "5")
            If Reqno = "" Then
                'oApplication.Utilities.Message(" Request No is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                ' Return False
            End If
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function

    Private Sub Gridbind(ByVal aForm As SAPbouiCOM.Form, ByVal Reqno As String)
        aForm.Freeze(True)
        Dim strqry As String
        oGrid_P1 = oForm.Items.Item("44").Specific
        oGrid_P2 = oForm.Items.Item("45").Specific
        oGrid_P1.DataTable = oForm.DataSources.DataTables.Item("DT_1")
        oGrid_P2.DataTable = oForm.DataSources.DataTables.Item("DT_2")
        Dim strstring As String
        strstring = "select DocEntry, U_Z_HRAppID,U_Z_HRAppName,U_Z_DeptName,U_Z_ApplStatus from [@Z_HR_OHEM1] where U_Z_ReqNo='" & Reqno & "'"
        oGrid_P1.DataTable.ExecuteQuery(strstring)
        oGrid_P1.Columns.Item("DocEntry").TitleObject.Caption = "Document No"
        oGrid_P1.Columns.Item("U_Z_HRAppID").TitleObject.Caption = "Applicant Id"
        oEditTextColumn = oGrid_P1.Columns.Item("U_Z_HRAppID")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
        oGrid_P1.Columns.Item("U_Z_HRAppName").TitleObject.Caption = "Applicant Name"
        oGrid_P1.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department"
        oGrid_P1.Columns.Item("U_Z_ApplStatus").TitleObject.Caption = "Applicant Status"
        oGrid_P1.Columns.Item("U_Z_ApplStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        ocombo = oGrid_P1.Columns.Item("U_Z_ApplStatus")
        ocombo.ValidValues.Add("O", "Open")
        ocombo.ValidValues.Add("S", "Shortlisted")
        ocombo.ValidValues.Add("A", "Approved")
        ocombo.ValidValues.Add("R", "Rejected")
        ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oGrid_P1.Columns.Item("DocEntry").Visible = False
        oGrid_P1.AutoResizeColumns()
        oGrid_P1.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        Dim DocNo1 As Integer = 0
        If oGrid_P1.Rows.Count > 0 Then
            DocNo1 = Convert.ToInt32(oGrid_P1.DataTable.GetValue("DocEntry", 0))
        End If

        strstring = "select U_Z_HRAppID,U_Z_ScheduleDate,U_Z_Comments,U_Z_Rating,U_Z_InterviewStatus  from [@Z_HR_OHEM2]  where DocEntry='" & DocNo1 & "'"
        oGrid_P2.DataTable.ExecuteQuery(strstring)
        oGrid_P2.Columns.Item("U_Z_HRAppID").TitleObject.Caption = "Applicant Id"
        oEditTextColumn = oGrid_P2.Columns.Item("U_Z_HRAppID")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
        oGrid_P2.Columns.Item("U_Z_ScheduleDate").TitleObject.Caption = "Interview Date"
        oGrid_P2.Columns.Item("U_Z_Comments").TitleObject.Caption = "Comments"
        oGrid_P2.Columns.Item("U_Z_Rating").TitleObject.Caption = "Interview Rating"
        oGrid_P2.Columns.Item("U_Z_InterviewStatus").TitleObject.Caption = "Interview Status"
        oGrid_P2.Columns.Item("U_Z_InterviewStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        ocombo1 = oGrid_P2.Columns.Item("U_Z_InterviewStatus")
        ocombo1.ValidValues.Add("P", "Pending")
        ocombo1.ValidValues.Add("S", "Selected")
        ocombo1.ValidValues.Add("R", "Rejected")
        ocombo1.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oGrid_P2.AutoResizeColumns()
        oGrid_P2.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single

        If DocNo1 = 0 Then
            oApplication.Utilities.Message("No Records Found...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
            Return
        End If
        aForm.Freeze(False)
    End Sub

    Private Sub LoadData()

        Dim oRecordSet As SAPbobsCOM.Recordset
        oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim strReqNo As String = oForm.Items.Item("5").Specific.value
        strQuery = "Select * From [@Z_HR_ORMPREQ] Where DocEntry = '" & strReqNo & "'"
        oRecordSet.DoQuery(strQuery)

        If Not oRecordSet.EoF Then
            oApplication.Utilities.setEdittextvalue(oForm, "43", oRecordSet.Fields.Item("DocEntry").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "48", oRecordSet.Fields.Item("U_Z_ReqDate").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "16", oRecordSet.Fields.Item("U_Z_EmpCode").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "18", oRecordSet.Fields.Item("U_Z_EmpName").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "25", oRecordSet.Fields.Item("U_Z_DeptName").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "33", oRecordSet.Fields.Item("U_Z_ExpMin").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "34", oRecordSet.Fields.Item("U_Z_ExpMax").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "35", oRecordSet.Fields.Item("U_Z_Vacancy").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "51", oRecordSet.Fields.Item("U_Z_EmpstDate").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "54", oRecordSet.Fields.Item("U_Z_IntAppDead").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "56", oRecordSet.Fields.Item("U_Z_ExtAppDead").Value)

            oCombobox = oForm.Items.Item("29").Specific
            oCombobox.Select(oRecordSet.Fields.Item("U_Z_EmpPosi").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
            oCombobox = oForm.Items.Item("39").Specific
            oCombobox.Select(oRecordSet.Fields.Item("U_Z_HRStatus").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
            '  oApplication.Utilities.setUserDSCombobox(oForm, "41", oRecordSet.Fields.Item("U_Z_HRStatus").Value)
        End If


    End Sub

    Private Sub Gridbind1(ByVal aform As SAPbouiCOM.Form)
        aform.Freeze(True)
        Dim strqry, strFromReq, strToReq, strposcode, strReqcondition, strPoscondition, strCondition As String
        strFromReq = oApplication.Utilities.getEdittextvalue(aform, "5")
        strToReq = oApplication.Utilities.getEdittextvalue(aform, "46")
        strposcode = oApplication.Utilities.getEdittextvalue(aform, "41")

        If strFromReq <> "" And strToReq <> "" Then
            strReqcondition = " DocEntry between '" & strFromReq & "' and '" & strToReq & "'"
        ElseIf strFromReq <> "" And strToReq = "" Then
            strReqcondition = " DocEntry >= '" & strFromReq & "'"
        ElseIf strFromReq = "" And strToReq <> "" Then
            strReqcondition = " DocEntry <= '" & strToReq & "'"
        Else
            strReqcondition = " 1=1"
        End If
        If strposcode <> "" Then
            strPoscondition = "U_Z_EmpPosi='" & strposcode & "'"
        Else
            strPoscondition = "1=1"
        End If
        oGrid = oForm.Items.Item("43").Specific
        oGrid_P1 = oForm.Items.Item("44").Specific
        oGrid_P2 = oForm.Items.Item("45").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
        oGrid_P1.DataTable = oForm.DataSources.DataTables.Item("DT_1")
        oGrid_P2.DataTable = oForm.DataSources.DataTables.Item("DT_2")
        strCondition = strReqcondition & " and " & strPoscondition & " Order by DocEntry Desc"
        strqry = "select DocEntry,U_Z_EmpCode,U_Z_EmpName,U_Z_DeptName,U_Z_PosName,U_Z_ReqDate,U_Z_Vacancy,"
        strqry = strqry & " case U_Z_AppStatus when 'P' then 'Pending' when 'A' then 'Approved' when 'R' then 'Rejected'"
        strqry = strqry & " when 'C' then 'Closed' when 'L' then 'Canceled' end as U_Z_AppStatus from [@Z_HR_ORMPREQ] where " & strCondition
        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Request Recruitment Code"
        oEditTextColumn = oGrid.Columns.Item("DocEntry")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Invoice
        oGrid.Columns.Item("U_Z_EmpCode").TitleObject.Caption = "Requester Code"
        oEditTextColumn = oGrid.Columns.Item("U_Z_EmpCode")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
        oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Requester Name"
        oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department"
        oGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position"
        oGrid.Columns.Item("U_Z_ReqDate").TitleObject.Caption = "Request Date"
        oGrid.Columns.Item("U_Z_Vacancy").TitleObject.Caption = "Vacant Positions"
        oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Recruitment Status"
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        If oGrid.Rows.Count > 0 Then
            oGrid.Rows.SelectedRows.Add(0)
            Dim DocNo As Integer = Convert.ToInt32(oGrid.DataTable.GetValue("DocEntry", 0))

            Dim strstring As String
            strstring = "select DocEntry, U_Z_HRAppID,U_Z_HRAppName,U_Z_DeptName,U_Z_ApplStatus from [@Z_HR_OHEM1] where U_Z_ReqNo='" & DocNo & "'"
            oGrid_P1.DataTable.ExecuteQuery(strstring)
            oGrid_P1.Columns.Item("DocEntry").TitleObject.Caption = "Document No"
            oGrid_P1.Columns.Item("U_Z_HRAppID").TitleObject.Caption = "Applicant Id"
            oEditTextColumn = oGrid_P1.Columns.Item("U_Z_HRAppID")
            oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
            oGrid_P1.Columns.Item("U_Z_HRAppName").TitleObject.Caption = "Applicant Name"
            oGrid_P1.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department"
            oGrid_P1.Columns.Item("U_Z_ApplStatus").TitleObject.Caption = "Applicant Status"
            oGrid_P1.Columns.Item("U_Z_ApplStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid_P1.Columns.Item("U_Z_ApplStatus")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("S", "Shortlisted")
            ocombo.ValidValues.Add("A", "Approved")
            ocombo.ValidValues.Add("R", "Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid_P1.Columns.Item("DocEntry").Visible = False
            oGrid_P1.AutoResizeColumns()
            oGrid_P1.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
            Dim DocNo1 As Integer = 0
            If oGrid_P1.Rows.Count > 0 Then
                DocNo1 = Convert.ToInt32(oGrid_P1.DataTable.GetValue("DocEntry", 0))
            End If

            strstring = "select U_Z_HRAppID,U_Z_ScheduleDate,U_Z_Comments,U_Z_Rating,U_Z_InterviewStatus  from [@Z_HR_OHEM2]  where DocEntry='" & DocNo1 & "'"
            oGrid_P2.DataTable.ExecuteQuery(strstring)
            oGrid_P2.Columns.Item("U_Z_HRAppID").TitleObject.Caption = "Applicant Id"
            oEditTextColumn = oGrid_P2.Columns.Item("U_Z_HRAppID")
            oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
            oGrid_P2.Columns.Item("U_Z_ScheduleDate").TitleObject.Caption = "Interview Date"
            oGrid_P2.Columns.Item("U_Z_Comments").TitleObject.Caption = "Comments"
            oGrid_P2.Columns.Item("U_Z_Rating").TitleObject.Caption = "Interview Rating"
            oGrid_P2.Columns.Item("U_Z_InterviewStatus").TitleObject.Caption = "Interview Status"
            oGrid_P2.Columns.Item("U_Z_InterviewStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo1 = oGrid_P2.Columns.Item("U_Z_InterviewStatus")
            ocombo1.ValidValues.Add("P", "Pending")
            ocombo1.ValidValues.Add("S", "Selected")
            ocombo1.ValidValues.Add("R", "Rejected")
            ocombo1.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid_P2.AutoResizeColumns()
            oGrid_P2.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single

            If DocNo = 0 Then
                oApplication.Utilities.Message("No Records Found...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aform.Freeze(False)
                Return
            End If

            If DocNo1 = 0 Then
                oApplication.Utilities.Message("No Records Found...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aform.Freeze(False)
                Return
            End If
        End If
        aform.Freeze(False)
    End Sub

    Private Sub reDrawScreen(ByVal sboForm As SAPbouiCOM.Form)
        Try
            sboForm.Freeze(True)

            sboForm.Items.Item("1").TextStyle = 3
            Dim intTop As Int16
            sboForm.Items.Item("43").Height = (sboForm.Height / 2) - 50
            sboForm.Items.Item("43").Width = (sboForm.Width) - 20
            intTop = sboForm.Items.Item("43").Top + sboForm.Items.Item("43").Height
            sboForm.Items.Item("28").Top = intTop
            sboForm.Items.Item("29").Top = sboForm.Items.Item("28").Top

            sboForm.Items.Item("28").TextStyle = 7
            sboForm.Items.Item("29").TextStyle = 7

            intTop = sboForm.Items.Item("28").Top + sboForm.Items.Item("28").Height
            sboForm.Items.Item("44").Top = intTop
            sboForm.Items.Item("45").Top = intTop

            sboForm.Items.Item("44").Height = sboForm.Items.Item("43").Height + 10
            sboForm.Items.Item("45").Height = sboForm.Items.Item("43").Height + 10
            sboForm.Items.Item("44").Width = (oForm.Width / 2) - 20
            sboForm.Items.Item("45").Width = sboForm.Items.Item("44").Width
            sboForm.Items.Item("45").Left = sboForm.Items.Item("44").Left + sboForm.Items.Item("44").Width + 5
            sboForm.Items.Item("29").Left = sboForm.Items.Item("45").Left

            oGrid = sboForm.Items.Item("43").Specific
            oGrid_P1 = oForm.Items.Item("44").Specific
            oGrid_P2 = oForm.Items.Item("45").Specific
            oGrid.AutoResizeColumns()
            oGrid_P1.AutoResizeColumns()
            oGrid_P2.AutoResizeColumns()

            sboForm.Freeze(False)
        Catch ex As Exception
            sboForm.Freeze(False)
        End Try
    End Sub

#Region "AddToUDT"
    Private Function RecruitmentClosed(ByVal aform As SAPbouiCOM.Form, ByVal Reqno As String) As Boolean
        Try
            Dim otestRs As SAPbobsCOM.Recordset
            otestRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            aform.Freeze(True)
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If
            oApplication.Company.StartTransaction()
            strSQL = "Update [@Z_HR_ORMPREQ] set U_Z_AppStatus='C' where DocEntry='" & Reqno & "'"
            otestRs.DoQuery(strSQL)
            'Time Stamp
            oApplication.Utilities.UpdateRecruitmentTimeStamp(Reqno, "CL")
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If
            aform.Freeze(False)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            End If
            aform.Freeze(False)
            Return False
        End Try
    End Function

    Private Function RecruitmentCanceled(ByVal aform As SAPbouiCOM.Form) As Boolean
        Try
            Dim otestRs, oRec As SAPbobsCOM.Recordset
            Dim strRequestno As String
            otestRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            aform.Freeze(True)
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If
            oApplication.Company.StartTransaction()
            Dim stSQL1 As String
            strRequestno = oApplication.Utilities.getEdittextvalue(aform, "5")
            stSQL1 = "Select * from [@Z_HR_OHEM1] where U_Z_ReqNo='" & strRequestno & "'"
            oRec.DoQuery(stSQL1)
            If oRec.RecordCount > 0 Then
                oApplication.Utilities.Message("Interview already started do not canceled the Request :" & strRequestno, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aform.Freeze(False)
                Return False
            Else
                strSQL = "Update [@Z_HR_ORMPREQ] set U_Z_AppStatus='L' where DocEntry='" & oApplication.Utilities.getEdittextvalue(aform, "5") & "'"
                otestRs.DoQuery(strSQL)
            End If
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If
            aform.Freeze(False)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            End If
            aform.Freeze(False)
            Return False
        End Try
    End Function

#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_RecClosing Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim strcode As String
                                If pVal.ItemUID = "3" And (oForm.PaneLevel = 2 Or oForm.PaneLevel = 3) Then
                                    If Validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                If pVal.ItemUID = "32" Then
                                    strcode = oApplication.Utilities.getEdittextvalue(oForm, "5")
                                    Dim objct As New clshrMPRequest
                                    objct.LoadForm1(strcode, oForm.Title, , , )
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "33" Then
                                    strcode = oApplication.Utilities.getEdittextvalue(oForm, "46")
                                    Dim objct As New clshrMPRequest
                                    objct.LoadForm1(strcode, oForm.Title, , , )
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim strCode, strHRstatus, strGMstatus, empcode, empname As String
                                If pVal.ItemUID = "43" And pVal.ColUID = "DocEntry" Then
                                    oGrid = oForm.Items.Item("8").Specific
                                    For intRow As Integer = pVal.Row To pVal.Row
                                        If 1 = 1 Then
                                            strCode = oGrid.DataTable.GetValue("DocEntry", intRow)
                                            'strHRstatus = oGrid.DataTable.GetValue("U_Z_HODStatus", intRow)
                                            'empcode = oApplication.Utilities.getEdittextvalue(oForm, "5")
                                            'empname = oApplication.Utilities.getEdittextvalue(oForm, "7")
                                            If oForm.Title = "Recruitment Closing" Then
                                                Dim objct As New clshrMPRequest
                                                objct.LoadForm1(strCode, oForm.Title, , , )
                                            Else
                                                oApplication.Utilities.Message("Your Request is Processing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            End If
                                        End If
                                    Next
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "44" And pVal.ColUID = "U_Z_HRAppID" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    strCode = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    Dim ooBj As New clshrCrApplicants
                                    ooBj.ViewCandidate(strCode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "45" And pVal.ColUID = "U_Z_HRAppID" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    strCode = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    Dim ooBj As New clshrCrApplicants
                                    ooBj.ViewCandidate(strCode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                reDrawScreen(oForm)
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "3"
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = oForm.PaneLevel + 1
                                        If oForm.PaneLevel = 3 Then
                                            Dim strDept, stSQL1, strskilles, Reqno As String
                                            Reqno = oApplication.Utilities.getEdittextvalue(oForm, "5")
                                            ' LoadData()
                                            Gridbind1(oForm)
                                        End If
                                        oForm.Freeze(False)
                                    Case "6"
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = oForm.PaneLevel - 1
                                        oForm.Freeze(False)
                                    Case "7"
                                        oGrid = oForm.Items.Item("43").Specific
                                        If oApplication.SBO_Application.MessageBox("Do you want confirm the Requisition Closing", , "Yes", "No") = 2 Then
                                            Exit Sub
                                        ElseIf oGrid.Rows.Count > 0 Then
                                            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                                If oGrid.Rows.IsSelected(intRow) Then
                                                    Dim DocNo As Integer = Convert.ToInt32(oGrid.DataTable.GetValue("DocEntry", intRow))
                                                    If RecruitmentClosed(oForm, DocNo) = True Then
                                                        oApplication.Utilities.Message("Recruitment Closed successfully...", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                                        oForm.Close()
                                                        Exit Sub
                                                    Else
                                                        BubbleEvent = False
                                                        Exit Sub
                                                    End If
                                                End If
                                            Next
                                        End If
                                End Select
                                If pVal.ItemUID = "43" And pVal.ColUID = "RowsHeader" And pVal.Row <> -1 Then
                                    oGrid = oForm.Items.Item("43").Specific
                                    If oGrid.Rows.Count > 0 Then
                                        Dim DocNo As Integer = Convert.ToInt32(oGrid.DataTable.GetValue("DocEntry", pVal.Row))
                                        Gridbind(oForm, DocNo)
                                    End If
                                End If
                                If pVal.ItemUID = "44" And pVal.ColUID = "RowsHeader" And pVal.Row <> -1 Then
                                    oForm.Freeze(True)
                                    Dim strstring As String
                                    oGrid = oForm.Items.Item("44").Specific
                                    oGrid_P1 = oForm.Items.Item("45").Specific
                                    If oGrid.Rows.Count > 0 Then
                                        Dim DocNo As Integer = Convert.ToInt32(oGrid.DataTable.GetValue("DocEntry", pVal.Row))
                                        strstring = "select DocEntry ,U_Z_HRAppID,U_Z_InterviewDate,U_Z_Comments,U_Z_Rating,U_Z_InterviewStatus  from [@Z_HR_OHEM2]  where DocEntry='" & DocNo & "'"
                                        oGrid_P1.DataTable.ExecuteQuery(strstring)
                                        oGrid_P1.Columns.Item("DocEntry").TitleObject.Caption = "Document No"
                                        oGrid_P1.Columns.Item("U_Z_HRAppID").TitleObject.Caption = "Applicant Id"
                                        oEditTextColumn = oGrid_P1.Columns.Item("U_Z_HRAppID")
                                        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
                                        oGrid_P1.Columns.Item("U_Z_InterviewDate").TitleObject.Caption = "Interview Date"
                                        oGrid_P1.Columns.Item("U_Z_Comments").TitleObject.Caption = "Comments"
                                        oGrid_P2.Columns.Item("U_Z_Rating").TitleObject.Caption = "Interview Rating"
                                        oGrid_P1.Columns.Item("U_Z_InterviewStatus").TitleObject.Caption = "Interview Status"
                                        oGrid_P1.Columns.Item("U_Z_InterviewStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                                        ocombo1 = oGrid_P1.Columns.Item("U_Z_InterviewStatus")
                                        ocombo1.ValidValues.Add("P", "Pending")
                                        ocombo1.ValidValues.Add("S", "Selected")
                                        ocombo1.ValidValues.Add("R", "Rejected")
                                        ocombo1.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                                        oGrid_P1.Columns.Item("DocEntry").Visible = False
                                        oGrid_P1.AutoResizeColumns()
                                        oGrid_P1.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                                    End If
                                    oForm.Freeze(False)
                                End If
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

                                        If pVal.ItemUID = "5" Then
                                            val = oDataTable.GetValue("DocEntry", 0)
                                            oApplication.Utilities.setEdittextvalue(oForm, "5", val)
                                        End If
                                        If pVal.ItemUID = "46" Then
                                            val = oDataTable.GetValue("DocEntry", 0)
                                            oApplication.Utilities.setEdittextvalue(oForm, "46", val)
                                        End If
                                        If pVal.ItemUID = "41" Then
                                            val = oDataTable.GetValue("U_Z_PosCode", 0)
                                            val1 = oDataTable.GetValue("U_Z_PosName", 0)
                                            oApplication.Utilities.setEdittextvalue(oForm, "42", val1)
                                            oApplication.Utilities.setEdittextvalue(oForm, "41", val)
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
                Case mnu_hr_RecClosing
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
