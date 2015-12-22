Public Class clshrApproveTemp
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oMatrix, oMatrix1, oMatrix2 As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oComboBox, oComboBox1 As SAPbouiCOM.ComboBox
    Private oCheckBox, oCheckBox1 As SAPbouiCOM.CheckBox
    Private InvForConsumedItems, count As Integer
    Dim oDBDataSource As SAPbouiCOM.DBDataSource
    Dim oDBDataSourceLines_1 As SAPbouiCOM.DBDataSource
    Dim oDBDataSourceLines_2, oDataSrc_Line As SAPbouiCOM.DBDataSource
    Public MatrixId As String
    Public intSelectedMatrixrow As Integer = 0
    Private RowtoDelete As Integer
    Private oRecordSet As SAPbobsCOM.Recordset
    Private dtValidFrom, dtValidTo As Date
    Private strQuery As String

#Region "Initialization"

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
#End Region

#Region "Load Form"

    Public Sub LoadForm()
        Try

            If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_ApproveTemp) = False Then
                oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Exit Sub
            End If
            oForm = oApplication.Utilities.LoadForm(xml_hr_ApproveTemp, frm_hr_ApproveTemp)
            oForm = oApplication.SBO_Application.Forms.ActiveForm()
            oForm.Freeze(True)
            'initialize(oForm)
            enableControls(oForm, True)
            FillDocType(oForm)
            FillLeaveType(oForm)
            oMatrix = oForm.Items.Item("9").Specific
            oMatrix.AutoResizeColumns()
            oMatrix = oForm.Items.Item("10").Specific
            oMatrix.AutoResizeColumns()
            oForm.EnableMenu(mnu_ADD_ROW, True)
            oForm.EnableMenu(mnu_DELETE_ROW, False)
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
            oForm.Items.Item("7").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Items.Item("22").Visible = False
            oForm.Items.Item("23").Visible = False
            oForm.Freeze(False)
        Catch ex As Exception
            Throw ex
        Finally
            oForm.Freeze(False)
        End Try
    End Sub

    Private Sub FillDocType(ByVal aForm As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        Dim oDBDataSrc As SAPbouiCOM.DBDataSource
        oComboBox = aForm.Items.Item("17").Specific
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        For intRow As Integer = oComboBox.ValidValues.Count - 1 To 0 Step -1
            oComboBox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oComboBox.ValidValues.Add("", "")
        oComboBox.ValidValues.Add("Train", "Training")
        oComboBox.ValidValues.Add("Rec", "Recruitment")
        oComboBox.ValidValues.Add("EmpLife", "Employee Life Cycle")
        oComboBox.ValidValues.Add("TraReq", "Travel Request")
        oComboBox.ValidValues.Add("ExpCli", "Expenses Claim")
        oComboBox.ValidValues.Add("LveReq", "Leave Request")
        oComboBox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        aForm.Items.Item("17").DisplayDesc = True
    End Sub
    Private Sub FillLeaveType(ByVal sform As SAPbouiCOM.Form)
        Dim oSlpRS, oRecS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oComboBox = sform.Items.Item("23").Specific
        oSlpRS.DoQuery("Select ""Code"",""Name"" from ""@Z_PAY_LEAVE"" order by ""Code""")
        For intRow As Integer = oComboBox.ValidValues.Count - 1 To 0 Step -1
            oComboBox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oComboBox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oComboBox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        sform.Items.Item("23").DisplayDesc = True
        oComboBox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
    End Sub
#End Region

#Region "Item Event"

    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_ApproveTemp Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1" And (oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Or oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE) Then
                                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Or oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                        If oApplication.SBO_Application.MessageBox("Do you want to confirm the information?", , "Yes", "No") = 2 Then
                                            BubbleEvent = False
                                            Exit Sub
                                        Else
                                            If validation(oForm) = False Then
                                                BubbleEvent = False
                                                Exit Sub
                                            End If
                                        End If
                                    End If
                                End If
                                Dim strDocType As String
                                strDocType = oComboBox.Selected.Value
                                Select Case pVal.ItemUID
                                    Case "7"
                                        If strDocType = "Train" Then
                                            oForm.PaneLevel = 1
                                        ElseIf strDocType = "TraReq" Then
                                            oForm.PaneLevel = 1
                                        ElseIf strDocType = "ExpCli" Then
                                            oForm.PaneLevel = 1
                                        ElseIf strDocType = "LveReq" Then
                                            oForm.PaneLevel = 1
                                        ElseIf pVal.CharPressed <> 9 And pVal.ItemUID = "7" And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                            oApplication.Utilities.Message("Employees not applicable for this document type.", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    Case "8"
                                        oForm.PaneLevel = 3
                                    Case "20"
                                        If strDocType = "Rec" Then
                                            oForm.PaneLevel = 2
                                        ElseIf strDocType = "EmpLife" Then
                                            oForm.PaneLevel = 2
                                        ElseIf pVal.CharPressed <> 9 And pVal.ItemUID = "20" And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                            oApplication.Utilities.Message("Department not applicable for this document type.", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                End Select

                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                oComboBox = oForm.Items.Item("17").Specific
                                If pVal.ItemUID = "9" Or pVal.ItemUID = "10" Or pVal.ItemUID = "21" Then
                                    If oApplication.Utilities.getEdittextvalue(oForm, "4") = "" Then
                                        BubbleEvent = False
                                        oApplication.SBO_Application.SetStatusBarMessage("Select Code to Proceed...", SAPbouiCOM.BoMessageTime.bmt_Medium, True)
                                        oForm.Items.Item("4").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                    ElseIf oApplication.Utilities.getEdittextvalue(oForm, "6") = "" Then
                                        BubbleEvent = False
                                        oApplication.SBO_Application.SetStatusBarMessage("Select Name to Proceed...", SAPbouiCOM.BoMessageTime.bmt_Medium, True)
                                        oForm.Items.Item("6").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                    ElseIf oComboBox.Selected.Value = "" Then
                                        BubbleEvent = False
                                        oApplication.SBO_Application.SetStatusBarMessage("Select Document Type to Proceed...", SAPbouiCOM.BoMessageTime.bmt_Medium, True)
                                        oForm.Items.Item("17").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                    End If
                                End If
                                If pVal.ItemUID = "9" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("9").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "9"
                                    frmSourceMatrix = oMatrix
                                End If
                                If pVal.ItemUID = "10" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("10").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "10"
                                    frmSourceMatrix = oMatrix
                                End If
                                If pVal.ItemUID = "21" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("21").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "21"
                                    frmSourceMatrix = oMatrix
                                End If
                        End Select
                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                If pVal.ItemUID = "21" And pVal.ColUID = "V_0" And pVal.CharPressed = 9 Then
                                    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                    Dim strIns As String
                                    oMatrix = oForm.Items.Item("21").Specific
                                    Dim objChooseForm As SAPbouiCOM.Form
                                    Dim objChoose As New clsChooseFromList
                                    clsChooseFromList.ItemUID = pVal.ItemUID
                                    clsChooseFromList.SourceFormUID = FormUID
                                    clsChooseFromList.SourceLabel = 0
                                    clsChooseFromList.CFLChoice = "Department" 'oCombo.Selected.Value
                                    clsChooseFromList.choice = "Dept"
                                    clsChooseFromList.Documentchoice = "Department"
                                    Try
                                        clsChooseFromList.BinDescrUID = ""
                                    Catch ex As Exception
                                        clsChooseFromList.BinDescrUID = "x"
                                    End Try

                                    clsChooseFromList.sourceColumID = pVal.ColUID
                                    clsChooseFromList.SourceLabel = pVal.Row
                                    oApplication.Utilities.LoadForm("\CFL.xml", frm_ChoosefromList)
                                    objChooseForm = oApplication.SBO_Application.Forms.ActiveForm()
                                    objChoose.databound(objChooseForm)
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "17" Then
                                    oMatrix = oForm.Items.Item("9").Specific
                                    oMatrix1 = oForm.Items.Item("10").Specific
                                    oMatrix2 = oForm.Items.Item("21").Specific
                                    oMatrix.Clear()
                                    oMatrix1.Clear()
                                    oMatrix2.Clear()
                                    oComboBox = oForm.Items.Item("17").Specific
                                    oApplication.Utilities.setEdittextvalue(oForm, "19", oComboBox.Selected.Description)
                                    Select Case oComboBox.Selected.Value
                                        Case "Rec", "EmpLife"
                                            oForm.Items.Item("20").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                            oForm.Items.Item("22").Visible = False
                                            oForm.Items.Item("23").Visible = False
                                        Case "Train", "TraReq", "ExpCli"
                                            oForm.Items.Item("7").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                            oForm.Items.Item("22").Visible = False
                                            oForm.Items.Item("23").Visible = False
                                        Case "LveReq"
                                            oForm.Items.Item("7").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                            oForm.Items.Item("22").Visible = True
                                            oForm.Items.Item("23").Visible = True
                                    End Select
                                End If
                                If pVal.ItemUID = "23" Then
                                    oComboBox = oForm.Items.Item("23").Specific
                                    oApplication.Utilities.setEdittextvalue(oForm, "25", oComboBox.Selected.Description)
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                              
                                Select Case pVal.ItemUID
                                    Case "13"
                                        'oApplication.SBO_Application.ActivateMenuItem(mnu_ADD_ROW)
                                        AddRow(oForm)
                                    Case "14"
                                        'oApplication.SBO_Application.ActivateMenuItem(mnu_DELETE_ROW)
                                        RefereshDeleteRow(oForm)
                                End Select
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oDataTable As SAPbouiCOM.DataTable
                                Dim val1, val, Val2 As String
                                Try
                                    oCFLEvento = pVal
                                    oDataTable = oCFLEvento.SelectedObjects
                                    If Not oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                        If pVal.ItemUID = "9" And (pVal.ColUID = "V_0" Or pVal.ColUID = "V_2") Then
                                            oMatrix = oForm.Items.Item("9").Specific
                                            For introw1 As Integer = 0 To oDataTable.Rows.Count - 1
                                                oMatrix = oForm.Items.Item("9").Specific
                                                If introw1 = 0 Then
                                                    val = oDataTable.GetValue("empID", 0)
                                                    Val2 = oDataTable.GetValue("U_Z_EmpID", 0)
                                                    val1 = oDataTable.GetValue("firstName", 0) & " " & oDataTable.GetValue("middleName", 0) & " " & oDataTable.GetValue("lastName", 0)
                                                    Try
                                                        oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val1)
                                                        Try
                                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", pVal.Row, Val2)
                                                        Catch ex As Exception
                                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", pVal.Row, Val2)
                                                        End Try
                                                        Try
                                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val)
                                                        Catch ex As Exception
                                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val)
                                                        End Try
                                                        If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                                            oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                        End If
                                                    Catch ex As Exception
                                                        If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                                            oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                        End If
                                                    End Try
                                                Else
                                                    oMatrix.AddRow()
                                                    oMatrix.ClearRowData(oMatrix.RowCount)
                                                    val = oDataTable.GetValue("empID", introw1)
                                                    Val2 = oDataTable.GetValue("U_Z_EmpID", introw1)
                                                    val1 = oDataTable.GetValue("firstName", introw1) & " " & oDataTable.GetValue("middleName", introw1) & " " & oDataTable.GetValue("lastName", introw1)
                                                    Try
                                                        oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, val1)
                                                        Try
                                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, Val2)
                                                        Catch ex As Exception
                                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, Val2)
                                                        End Try
                                                        Try
                                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, val)
                                                        Catch ex As Exception
                                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, val)
                                                        End Try

                                                        If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                                            oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                        End If
                                                    Catch ex As Exception
                                                        If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                                            oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                        End If
                                                    End Try
                                                End If
                                            Next
                                            AssignLineNo(oForm)
                                        ElseIf pVal.ItemUID = "10" And pVal.ColUID = "V_0" Then
                                            val1 = oDataTable.GetValue("USER_CODE", 0)
                                            val = oDataTable.GetValue("U_NAME", 0)
                                            oMatrix = oForm.Items.Item("10").Specific
                                            Try
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val1)
                                            Catch ex As Exception
                                                If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                End If
                                            End Try
                                        End If
                                    End If
                                Catch ex As Exception
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
                Case mnu_hr_ApproveTemp
                    LoadForm()
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then

                    End If
                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        AddRow(oForm)
                    End If
                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        RefereshDeleteRow(oForm)
                    End If
                Case mnu_ADD
                    If pVal.BeforeAction = False Then
                        oForm = oApplication.SBO_Application.Forms.ActiveForm()
                        ' initialize(oForm)
                        enableControls(oForm, True)
                    End If
                Case mnu_FIND
                    If pVal.BeforeAction = False Then
                        enableControls(oForm, True)
                    End If
                Case "1283"
                    If pVal.BeforeAction = True Then
                        If oApplication.SBO_Application.MessageBox("Do you want to remove approval template?", , "Yes", "No") = 2 Then
                            BubbleEvent = False
                            Exit Sub
                        End If
                    End If
            End Select
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub

#End Region

#Region "Data Events"

    Public Sub FormDataEvent(ByRef BusinessObjectInfo As SAPbouiCOM.BusinessObjectInfo, ByRef BubbleEvent As Boolean)
        Try
            oForm = oApplication.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID)
            If oForm.TypeEx = frm_hr_ApproveTemp And BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True Then
                oComboBox = oForm.Items.Item("17").Specific
                Dim strtype As String = oComboBox.Selected.Value
                Select Case strtype
                    Case "Rec", "EmpLife"
                        oForm.Items.Item("20").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                        oForm.Items.Item("22").Visible = False
                        oForm.Items.Item("23").Visible = False
                    Case "Train", "TraReq", "ExpCli"
                        oForm.Items.Item("7").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                        oForm.Items.Item("22").Visible = False
                        oForm.Items.Item("23").Visible = False
                    Case "LveReq"
                        oForm.Items.Item("7").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                        oForm.Items.Item("22").Visible = True
                        oForm.Items.Item("23").Visible = True
                End Select
            End If
            If oForm.TypeEx = frm_hr_ApproveTemp Then
                Select Case BusinessObjectInfo.BeforeAction
                    Case True
                        
                    Case False
                        Select Case BusinessObjectInfo.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD
                                oDBDataSource = oForm.DataSources.DBDataSources.Item("@Z_HR_OAPPT")
                                enableControls(oForm, False)
                        End Select
                End Select
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub

#End Region

#Region "Function"

    'Private Sub initialize(ByVal oForm As SAPbouiCOM.Form)
    '    Try
    '        oDBDataSource = oForm.DataSources.DBDataSources.Item("@Z_HR_OAPPT")
    '        oDBDataSourceLines_1 = oForm.DataSources.DBDataSources.Item("@Z_HR_APPT1")
    '        oDBDataSourceLines_2 = oForm.DataSources.DBDataSources.Item("@Z_HR_APPT2")

    '        oMatrix = oForm.Items.Item("9").Specific
    '        oMatrix.LoadFromDataSource()
    '        oMatrix.AddRow(1, -1)
    '        oMatrix.FlushToDataSource()

    '        oMatrix = oForm.Items.Item("10").Specific
    '        oMatrix.LoadFromDataSource()
    '        oMatrix.AddRow(1, -1)
    '        oMatrix.FlushToDataSource()

    '        oForm.Update()
    '        MatrixId = "9"
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub

#Region "Methods"
    Private Sub AssignLineNo(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)

            Select Case aForm.PaneLevel
                Case "1"
                    oMatrix = aForm.Items.Item("9").Specific
                    oDBDataSourceLines_1 = oForm.DataSources.DBDataSources.Item("@Z_HR_APPT1")
                    oMatrix.FlushToDataSource()
                    For count = 1 To oDBDataSourceLines_1.Size
                        oDBDataSourceLines_1.SetValue("LineId", count - 1, count)
                    Next
                    oMatrix.LoadFromDataSource()
                Case "3"
                    oMatrix = aForm.Items.Item("10").Specific
                    oDBDataSourceLines_2 = oForm.DataSources.DBDataSources.Item("@Z_HR_APPT2")
                    oMatrix.FlushToDataSource()
                    For count = 1 To oDBDataSourceLines_2.Size
                        oDBDataSourceLines_2.SetValue("LineId", count - 1, count)
                    Next
                    oMatrix.LoadFromDataSource()
                Case "2"
                    oMatrix = aForm.Items.Item("21").Specific
                    oDBDataSourceLines_2 = oForm.DataSources.DBDataSources.Item("@Z_HR_APPT3")
                    oMatrix.FlushToDataSource()
                    For count = 1 To oDBDataSourceLines_2.Size
                        oDBDataSourceLines_2.SetValue("LineId", count - 1, count)
                    Next
                    oMatrix.LoadFromDataSource()
            End Select
            aForm.Freeze(False)
        Catch ex As Exception
            aForm.Freeze(False)
            Throw ex
        End Try
    End Sub

  
#End Region

    Private Sub AddRow(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            Select Case aForm.PaneLevel
                Case "1"
                    oMatrix = aForm.Items.Item("9").Specific
                    oDBDataSourceLines_1 = oForm.DataSources.DBDataSources.Item("@Z_HR_APPT1")
                    If oMatrix.RowCount <= 0 Then
                        oMatrix.AddRow()
                    End If
                    oEditText = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                    Try
                        If oEditText.Value <> "" Then
                            oMatrix.AddRow()
                            oMatrix.ClearRowData(oMatrix.RowCount)
                        End If
                    Catch ex As Exception
                        aForm.Freeze(False)
                    End Try
                    oMatrix.FlushToDataSource()
                    For count = 1 To oDBDataSourceLines_1.Size
                        oDBDataSourceLines_1.SetValue("LineId", count - 1, count)
                    Next
                    oMatrix.LoadFromDataSource()
                    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    AssignLineNo(aForm)
                Case "3"
                    oMatrix = aForm.Items.Item("10").Specific
                    oDBDataSourceLines_2 = oForm.DataSources.DBDataSources.Item("@Z_HR_APPT2")
                    If oMatrix.RowCount <= 0 Then
                        oMatrix.AddRow()
                    End If
                    oEditText = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                    Try
                        If oEditText.Value <> "" Then 'And oCheckBox.Checked = False Then
                            oMatrix.AddRow()
                            oMatrix.ClearRowData(oMatrix.RowCount)
                        End If
                    Catch ex As Exception
                        aForm.Freeze(False)
                    End Try
                    oMatrix.FlushToDataSource()
                    For count = 1 To oDBDataSourceLines_2.Size
                        oDBDataSourceLines_2.SetValue("LineId", count - 1, count)
                    Next
                    oMatrix.LoadFromDataSource()
                    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    AssignLineNo(aForm)
                Case "2"
                    oMatrix = aForm.Items.Item("21").Specific
                    oDBDataSourceLines_2 = oForm.DataSources.DBDataSources.Item("@Z_HR_APPT3")
                    If oMatrix.RowCount <= 0 Then
                        oMatrix.AddRow()
                    End If
                    oEditText = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                    Try
                        If oEditText.Value <> "" Then 'And oCheckBox.Checked = False Then
                            oMatrix.AddRow()
                            oMatrix.ClearRowData(oMatrix.RowCount)
                        End If
                    Catch ex As Exception
                        aForm.Freeze(False)
                    End Try
                    oMatrix.FlushToDataSource()
                    For count = 1 To oDBDataSourceLines_2.Size
                        oDBDataSourceLines_2.SetValue("LineId", count - 1, count)
                    Next
                    oMatrix.LoadFromDataSource()
                    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    AssignLineNo(aForm)
            End Select
            aForm.Freeze(False)
        Catch ex As Exception
            aForm.Freeze(False)
            Throw ex
        End Try
    End Sub

#Region "Delete Row"
    'Private Sub RefereshDeleteRow(ByVal aForm As SAPbouiCOM.Form)
    '    If Me.MatrixId = "9" Then
    '        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_APPT1")
    '    ElseIf Me.MatrixId = "10" Then
    '        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_APPT2")
    '    Else
    '        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_APPT3")
    '    End If
    '    If intSelectedMatrixrow <= 0 Then
    '        Exit Sub
    '    End If
    '    Me.RowtoDelete = intSelectedMatrixrow
    '    oDataSrc_Line.RemoveRecord(Me.RowtoDelete - 1)
    '    oMatrix = frmSourceMatrix
    '    oMatrix.FlushToDataSource()
    '    For count = 1 To oDataSrc_Line.Size - 1
    '        oDataSrc_Line.SetValue("LineId", count - 1, count)
    '    Next
    '    oMatrix.LoadFromDataSource()
    '    If oMatrix.RowCount > 0 Then
    '        oMatrix.DeleteRow(RowtoDelete)
    '    End If
    'End Sub

    Private Sub RefereshDeleteRow(ByVal aForm As SAPbouiCOM.Form)
        Try
            oDBDataSourceLines_1 = oForm.DataSources.DBDataSources.Item("@Z_HR_APPT1")
            oDBDataSourceLines_2 = oForm.DataSources.DBDataSources.Item("@Z_HR_APPT2")
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_APPT3")
            If Me.MatrixId = "9" Then
                oMatrix = aForm.Items.Item("9").Specific
                Me.RowtoDelete = intSelectedMatrixrow
                oDBDataSourceLines_1.RemoveRecord(Me.RowtoDelete - 1)
                oMatrix.LoadFromDataSource()
                oMatrix.FlushToDataSource()
                For count = 1 To oDBDataSourceLines_1.Size
                    oDBDataSourceLines_1.SetValue("LineId", count - 1, count)
                Next
            ElseIf (Me.MatrixId = "10") Then
                oMatrix = aForm.Items.Item("10").Specific
                Me.RowtoDelete = intSelectedMatrixrow
                oDBDataSourceLines_2.RemoveRecord(Me.RowtoDelete - 1)
                oMatrix.LoadFromDataSource()
                oMatrix.FlushToDataSource()
                For count = 1 To oDBDataSourceLines_2.Size
                    oDBDataSourceLines_2.SetValue("LineId", count - 1, count)
                Next
            ElseIf (Me.MatrixId = "21") Then
                oMatrix = aForm.Items.Item("21").Specific
                Me.RowtoDelete = intSelectedMatrixrow
                oDataSrc_Line.RemoveRecord(Me.RowtoDelete - 1)
                oMatrix.LoadFromDataSource()
                oMatrix.FlushToDataSource()
                For count = 1 To oDBDataSourceLines_2.Size
                    oDataSrc_Line.SetValue("LineId", count - 1, count)
                Next
            End If
            oMatrix.LoadFromDataSource()
        Catch ex As Exception
            aForm.Freeze(False)
            Throw ex
        End Try
    End Sub
#End Region

#Region "Validations"
    Private Function validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            aForm.Freeze(True)
            oComboBox = aForm.Items.Item("17").Specific
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            If oApplication.Utilities.getEdittextvalue(aForm, "4") = "" Then
                oApplication.Utilities.Message("Enter Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            ElseIf oApplication.Utilities.getEdittextvalue(aForm, "6") = "" Then
                oApplication.Utilities.Message("Enter Name...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            ElseIf oComboBox.Selected.Value = "" Then
                oApplication.Utilities.Message("Select Document Type...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            End If
            Select Case oComboBox.Selected.Value
                Case "Train", "TraReq", "ExpCli"
                    oMatrix = aForm.Items.Item("9").Specific
                    If oMatrix.RowCount = 0 Then
                        oApplication.Utilities.Message("Employee Row Cannot be Empty...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aForm.Freeze(False)
                        Return False
                    End If
                    oMatrix = aForm.Items.Item("9").Specific
                    For i As Integer = 1 To oMatrix.RowCount
                        oEditText = oMatrix.Columns.Item("V_0").Cells.Item(i).Specific
                        If oEditText.Value <> "" Then ' CType(oMatrix.Columns.Item("V_0").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value <> "" Then
                            oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            strQuery = "Select 1 As 'Return' From [@Z_HR_APPT1] T0 inner join [@Z_HR_OAPPT] T1 on T0.DocEntry=T1.DocEntry"
                            strQuery += " Where "
                            strQuery += " T1.U_Z_Code <> '" & oApplication.Utilities.getEdittextvalue(aForm, "4") & "' and T1.U_Z_DocType ='" & oComboBox.Selected.Value & "'"
                            strQuery += " And T0.U_Z_OUser = '" + CType(oMatrix.Columns.Item("V_0").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value + "'"
                            oRecordSet.DoQuery(strQuery)
                            If oRecordSet.RecordCount > 0 Then
                                oApplication.Utilities.Message("Employee Code : " + CType(oMatrix.Columns.Item("V_0").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value + " Already Defined in another Template...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                aForm.Freeze(False)
                                Return False
                            End If
                        End If
                    Next
                Case "Rec", "EmpLife"
                    oMatrix = aForm.Items.Item("21").Specific
                    If oMatrix.RowCount = 0 Then
                        oApplication.Utilities.Message("Department Row Cannot be Empty...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aForm.Freeze(False)
                        Return False
                    End If
                Case "LveReq"
                    oMatrix = aForm.Items.Item("9").Specific
                    If oMatrix.RowCount = 0 Then
                        oApplication.Utilities.Message("Employee Row Cannot be Empty...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aForm.Freeze(False)
                        Return False
                    End If
                    oComboBox1 = aForm.Items.Item("23").Specific
                    Dim LveType As String = oComboBox1.Selected.Value
                    If LveType = "" Then
                        oApplication.Utilities.Message("Leave Type Cannot be Empty...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aForm.Freeze(False)
                        Return False
                    End If
                    oMatrix = aForm.Items.Item("9").Specific
                    For i As Integer = 1 To oMatrix.RowCount
                        oEditText = oMatrix.Columns.Item("V_0").Cells.Item(i).Specific
                        If oEditText.Value <> "" Then ' CType(oMatrix.Columns.Item("V_0").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value <> "" Then
                            oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            strQuery = "Select 1 As 'Return' From [@Z_HR_APPT1] T0 inner join [@Z_HR_OAPPT] T1 on T0.DocEntry=T1.DocEntry"
                            strQuery += " Where "
                            strQuery += " T1.U_Z_Code <> '" & oApplication.Utilities.getEdittextvalue(aForm, "4") & "' and T1.U_Z_DocType ='" & oComboBox.Selected.Value & "' and T1.U_Z_LveType ='" & oComboBox1.Selected.Value & "'"
                            strQuery += " And T0.U_Z_OUser = '" + CType(oMatrix.Columns.Item("V_0").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value + "'"
                            oRecordSet.DoQuery(strQuery)
                            If oRecordSet.RecordCount > 0 Then
                                oApplication.Utilities.Message("Employee Code : " + CType(oMatrix.Columns.Item("V_0").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value + " Already Defined in another Template...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                aForm.Freeze(False)
                                Return False
                            End If
                        End If
                    Next
            End Select
            oMatrix = aForm.Items.Item("10").Specific
            If oMatrix.RowCount = 0 Then
                oApplication.Utilities.Message("Authorizer Row Cannot be Empty...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            End If
            oMatrix = aForm.Items.Item("10").Specific
            Dim blnflag As Boolean = False
            Dim blnActive As Boolean = False
            Dim oCheck1 As SAPbouiCOM.CheckBox
            For intRow As Integer = 1 To oMatrix.RowCount
                oCheckBox = oMatrix.Columns.Item("V_3").Cells.Item(intRow).Specific
                oCheck1 = oMatrix.Columns.Item("V_4").Cells.Item(intRow).Specific
                If oCheck1.Checked = True Then
                    blnActive = True
                End If
                If oCheckBox.Checked = True Then
                    If oCheck1.Checked = False Then
                        oApplication.Utilities.Message("Only Active Authorizer will be set as final authorizer...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aForm.Freeze(False)
                        Return False
                    End If
                    blnflag = True
                End If
            Next

            If blnActive = False Then
                oApplication.Utilities.Message("Atlease one  Authorizer should be active...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            End If
            If blnflag = False Then
                oApplication.Utilities.Message("Select Final Authorizer...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            End If

            Dim strECode, strECode1, strEname, strEname1 As String
            oMatrix = aForm.Items.Item("9").Specific
            For intRow As Integer = 1 To oMatrix.RowCount
                strECode = CType(oMatrix.Columns.Item("V_0").Cells.Item(intRow).Specific, SAPbouiCOM.EditText).Value
                For intInnerLoop As Integer = intRow To oMatrix.RowCount
                    strECode1 = CType(oMatrix.Columns.Item("V_0").Cells.Item(intInnerLoop).Specific, SAPbouiCOM.EditText).Value
                    If strECode = strECode1 And intRow <> intInnerLoop Then
                        oApplication.Utilities.Message("Employee Duplicated in Row : " + intInnerLoop.ToString() + "...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        oMatrix.Columns.Item("V_0").Cells.Item(intInnerLoop).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                        aForm.Freeze(False)
                        Return False
                    End If
                Next
            Next

            oMatrix = aForm.Items.Item("21").Specific
            For intRow As Integer = 1 To oMatrix.RowCount
                strECode = CType(oMatrix.Columns.Item("V_0").Cells.Item(intRow).Specific, SAPbouiCOM.EditText).Value
                For intInnerLoop As Integer = intRow To oMatrix.RowCount
                    strECode1 = CType(oMatrix.Columns.Item("V_0").Cells.Item(intInnerLoop).Specific, SAPbouiCOM.EditText).Value
                    If strECode = strECode1 And intRow <> intInnerLoop Then
                        oApplication.Utilities.Message("Department Duplicated in Row : " + intInnerLoop.ToString() + "...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        oMatrix.Columns.Item("V_0").Cells.Item(intInnerLoop).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                        aForm.Freeze(False)
                        Return False
                    End If
                Next
            Next


            oMatrix = aForm.Items.Item("10").Specific
            For intRow As Integer = 1 To oMatrix.RowCount
                strECode = CType(oMatrix.Columns.Item("V_0").Cells.Item(intRow).Specific, SAPbouiCOM.EditText).Value
                oCheckBox = oMatrix.Columns.Item("V_3").Cells.Item(intRow).Specific
                For intInnerLoop As Integer = intRow To oMatrix.RowCount
                    strECode1 = CType(oMatrix.Columns.Item("V_0").Cells.Item(intInnerLoop).Specific, SAPbouiCOM.EditText).Value
                    oCheckBox1 = oMatrix.Columns.Item("V_3").Cells.Item(intInnerLoop).Specific
                    If strECode = strECode1 And intRow <> intInnerLoop Then
                        oApplication.Utilities.Message("Authorizer Duplicated in Row : " + intInnerLoop.ToString() + "...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        oMatrix.Columns.Item("V_0").Cells.Item(intInnerLoop).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                        aForm.Freeze(False)
                        Return False
                    ElseIf oCheckBox.Checked = True And oCheckBox1.Checked = True And intRow <> intInnerLoop Then
                        oApplication.Utilities.Message("Select Only one final Authorizer. ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        oMatrix.Columns.Item("V_0").Cells.Item(intInnerLoop).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                        aForm.Freeze(False)
                        Return False
                    End If
                Next
            Next






            'oMatrix = aForm.Items.Item("10").Specific
            'oMatrix.LoadFromDataSource()
            'Dim intApprover As Integer
            'For i As Integer = 1 To oMatrix.RowCount
            '    If CType(oMatrix.Columns.Item("V_0").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value <> "" Then
            '        intApprover += 1
            '    End If
            'Next
            'If intApprover > 4 Then
            '    oApplication.Utilities.Message("Can Have Maximum of 4 Authorizer...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '    aForm.Freeze(False)
            '    Return False
            'End If
            oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strQuery = "Select 1 As 'Return',DocEntry From [@Z_HR_OAPPT]"
            strQuery += " Where "
            strQuery += " U_Z_Code = '" & oApplication.Utilities.getEdittextvalue(aForm, "4") & "' And DocEntry <> '" & oApplication.Utilities.getEdittextvalue(aForm, "12") & "'"
            oRecordSet.DoQuery(strQuery)
            If Not oRecordSet.EoF Then
                oApplication.Utilities.Message("Code Already Exist...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Freeze(False)
                Return False
            End If

            'Select Case oComboBox.Selected.Value
            '    Case "Train", "TraReq", "ExpCli"
            '        strQuery = "Select 1 As 'Return',DocEntry From  [@Z_HR_OAPPT] T0 "
            '        strQuery += " Where "
            '        strQuery += " U_Z_DocType = '" & oComboBox.Selected.Value & "' And DocEntry <> '" & oApplication.Utilities.getEdittextvalue(aForm, "12") & "'"
            '        oRecordSet.DoQuery(strQuery)
            '        If Not oRecordSet.EoF Then
            '            oApplication.Utilities.Message("Document Type Already Exist...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '            aForm.Freeze(False)
            '            Return False
            '        End If

            'End Select

           

            oMatrix = aForm.Items.Item("21").Specific
            For i As Integer = 1 To oMatrix.RowCount
                oEditText = oMatrix.Columns.Item("V_0").Cells.Item(i).Specific
                If oEditText.Value <> "" Then ' CType(oMatrix.Columns.Item("V_0").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value <> "" Then
                    oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    strQuery = "Select 1 As 'Return' From [@Z_HR_APPT3] T0 inner join [@Z_HR_OAPPT] T1 on T0.DocEntry=T1.DocEntry"
                    strQuery += " Where "
                    strQuery += " T1.U_Z_Code <> '" & oApplication.Utilities.getEdittextvalue(aForm, "4") & "' and T1.U_Z_DocType ='" & oComboBox.Selected.Value & "'"
                    strQuery += " And T0.U_Z_DeptCode = '" + CType(oMatrix.Columns.Item("V_0").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value + "'"
                    oRecordSet.DoQuery(strQuery)
                    If oRecordSet.RecordCount > 0 Then
                        oApplication.Utilities.Message("Department  : " + CType(oMatrix.Columns.Item("V_1").Cells.Item(i).Specific, SAPbouiCOM.EditText).Value + " Already mapped in another Template...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aForm.Freeze(False)
                        Return False
                    End If
                End If
            Next
            AssignLineNo(aForm)
            aForm.Freeze(False)
            Return True
        Catch ex As Exception
            aForm.Freeze(False)
            Throw ex
        End Try
    End Function
#End Region

#Region "Disable Controls"

    Private Sub enableControls(ByVal oForm As SAPbouiCOM.Form, ByVal blnEnable As Boolean)
        Try
            'oForm.Items.Item("12").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oForm.Items.Item("4").Enabled = blnEnable
            oForm.Items.Item("6").Enabled = blnEnable
            oForm.Items.Item("17").Enabled = blnEnable
            oForm.Items.Item("23").Enabled = blnEnable
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

#End Region

#End Region
End Class
