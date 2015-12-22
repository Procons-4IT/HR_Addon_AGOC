Public Class clshrSalStructure
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private RowtoDelete As Integer
    Private InvBaseDocNo As String
    Private MatrixId As String
    Private oColumn As SAPbouiCOM.Column
    Private InvForConsumedItems, count As Integer
    Private blnFlag As Boolean = False
    Dim oDataSrc_Line As SAPbouiCOM.DBDataSource
    Dim oDataSrc_Line1 As SAPbouiCOM.DBDataSource
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_SalStru) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_SalStru, frm_hr_SalStru)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.DataBrowser.BrowseBy = "5"
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.EnableMenu("1283", True)
        AddChooseFromList(oForm)
        databind(oForm)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_SALST1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line1 = oForm.DataSources.DBDataSources.Item("@Z_HR_SALST2")
        For count = 1 To oDataSrc_Line1.Size - 1
            oDataSrc_Line1.SetValue("LineId", count - 1, count)
        Next
        oForm.PaneLevel = 3
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        reDrawForm(oForm)
        oForm.Freeze(False)
    End Sub
    Public Sub LoadForm1(ByVal salcode As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_SalStru) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_SalStru, frm_hr_SalStru)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        ' oForm.DataBrowser.BrowseBy = "5"
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.EnableMenu("1283", True)
        AddChooseFromList(oForm)
        databind(oForm)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_SALST1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line1 = oForm.DataSources.DBDataSources.Item("@Z_HR_SALST2")
        For count = 1 To oDataSrc_Line1.Size - 1
            oDataSrc_Line1.SetValue("LineId", count - 1, count)
        Next
        oForm.PaneLevel = 3
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oForm.Items.Item("5").Enabled = True
        oApplication.Utilities.setEdittextvalue(oForm, "5", salcode)
        oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE Then
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        End If
        reDrawForm(oForm)
        oForm.Freeze(False)
    End Sub

#Region "Add Choose From List"
    Private Sub databind(ByVal aForm As SAPbouiCOM.Form)
        oMatrix = aForm.Items.Item("11").Specific
        Dim otest As SAPbobsCOM.Recordset
        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oColumn = oMatrix.Columns.Item("V_0")
        otest.DoQuery("Select ""U_Z_CODE"",""U_Z_NAME"" from ""@Z_PAY_OEAR""")
        For intRow As Integer = oColumn.ValidValues.Count - 1 To 0 Step -1
            Try
                oColumn.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
            Catch ex As Exception

            End Try

        Next
        oColumn.ValidValues.Add("", "")
        For intRow As Integer = 0 To otest.RecordCount - 1
            oColumn.ValidValues.Add(otest.Fields.Item(0).Value, otest.Fields.Item(1).Value)
            otest.MoveNext()
        Next
        oColumn.DisplayDesc = False


        'oColumn.ChooseFromListUID = "CFL1"
        'oColumn.ChooseFromListAlias = "U_Z_AlloCode"

        oMatrix = aForm.Items.Item("10").Specific
        oColumn = oMatrix.Columns.Item("V_0")
        otest.DoQuery("Select ""Code"",""Name"" from ""@Z_PAY_OCON""")
        For intRow As Integer = oColumn.ValidValues.Count - 1 To 0 Step -1
            Try
                oColumn.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
            Catch ex As Exception

            End Try

        Next
        oColumn.ValidValues.Add("", "")
        For intRow As Integer = 0 To otest.RecordCount - 1
            oColumn.ValidValues.Add(otest.Fields.Item(0).Value, otest.Fields.Item(1).Value)
            otest.MoveNext()
        Next
        oColumn.DisplayDesc = False

        'oColumn.ChooseFromListUID = "CFL2"
        'oColumn.ChooseFromListAlias = "U_Z_BenefCode"

        oEditText = aForm.Items.Item("7").Specific
        oEditText.ChooseFromListUID = "CFL3"
        oEditText.ChooseFromListAlias = "U_Z_LvelCode"

        oEditText = aForm.Items.Item("16").Specific
        oEditText.ChooseFromListUID = "CFL4"
        oEditText.ChooseFromListAlias = "U_Z_GrdeCode"
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
            oCFLCreationParams.ObjectType = "Z_HR_OALLO"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "U_Z_Status"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.ObjectType = "Z_HR_OBEFI"
            oCFLCreationParams.UniqueID = "CFL2"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "U_Z_Status"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()


            oCFLCreationParams.ObjectType = "Z_HR_OLVL"
            oCFLCreationParams.UniqueID = "CFL3"
            oCFL = oCFLs.Add(oCFLCreationParams)

            '' Adding Conditions to CFL2
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "U_Z_Status"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.ObjectType = "Z_HR_OGRD"
            oCFLCreationParams.UniqueID = "CFL4"
            oCFL = oCFLs.Add(oCFLCreationParams)

            '' Adding Conditions to CFL2
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "U_Z_Status"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
         
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

#End Region

#Region "Methods"
    Private Sub AssignLineNo(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("11").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_SALST1")
            oMatrix.FlushToDataSource()
            For count = 1 To oDataSrc_Line.Size
                oDataSrc_Line.SetValue("LineId", count - 1, count)
            Next
            oMatrix.LoadFromDataSource()
            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
        End Try
    End Sub
    Private Sub AssignLineNo1(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("10").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_SALST2")
            oMatrix.FlushToDataSource()
            For count = 1 To oDataSrc_Line.Size
                oDataSrc_Line.SetValue("LineId", count - 1, count)
            Next
            oMatrix.LoadFromDataSource()

            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
        End Try

    End Sub
#End Region

    Private Sub AddRow(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)

            Select Case aForm.PaneLevel
                Case "1"
                    oMatrix = aForm.Items.Item("11").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_SALST1")
                    If oMatrix.RowCount <= 0 Then
                        oMatrix.AddRow()
                    End If
                    oEditText = oMatrix.Columns.Item("V_1").Cells.Item(oMatrix.RowCount).Specific
                    Try
                        If oEditText.Value <> "" Then
                            oMatrix.AddRow()
                            Select Case aForm.PaneLevel
                                Case "1"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
                            End Select
                        End If

                    Catch ex As Exception
                        aForm.Freeze(False)
                    End Try
                    oMatrix.FlushToDataSource()
                    For count = 1 To oDataSrc_Line.Size
                        oDataSrc_Line.SetValue("LineId", count - 1, count)
                    Next
                    oMatrix.LoadFromDataSource()
                    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    AssignLineNo(aForm)
                Case "2"
                    oMatrix = aForm.Items.Item("10").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_SALST2")

                    If oMatrix.RowCount <= 0 Then
                        oMatrix.AddRow()
                    End If
                    oEditText = oMatrix.Columns.Item("V_1").Cells.Item(oMatrix.RowCount).Specific
                    Try
                        If oEditText.Value <> "" Then
                            oMatrix.AddRow()
                            Select Case aForm.PaneLevel
                                Case "2"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
                            End Select
                        End If

                    Catch ex As Exception
                        aForm.Freeze(False)
                    End Try
                    oMatrix.FlushToDataSource()
                    For count = 1 To oDataSrc_Line.Size
                        oDataSrc_Line.SetValue("LineId", count - 1, count)
                    Next
                    oMatrix.LoadFromDataSource()
                    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    AssignLineNo1(aForm)
            End Select


            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)

        End Try
    End Sub
    Private Sub deleterow(ByVal aForm As SAPbouiCOM.Form)
        Select Case aForm.PaneLevel
            Case "1"
                oMatrix = aForm.Items.Item("11").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_SALST1")
            Case "2"
                oMatrix = aForm.Items.Item("10").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_SALST2")
        End Select

        '  oMatrix = aForm.Items.Item("16").Specific
        oMatrix.FlushToDataSource()
        For introw As Integer = 1 To oMatrix.RowCount
            If oMatrix.IsRowSelected(introw) Then
                oMatrix.DeleteRow(introw)
                oDataSrc_Line.RemoveRecord(introw - 1)
                'oMatrix = frmSourceMatrix
                For count As Integer = 1 To oDataSrc_Line.Size
                    oDataSrc_Line.SetValue("LineId", count - 1, count)
                Next
                Select Case aForm.PaneLevel
                    Case "1"
                        oMatrix = aForm.Items.Item("11").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_SALST1")
                        AssignLineNo(aForm)
                    Case "2"
                        oMatrix = aForm.Items.Item("10").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_SALST2")
                        AssignLineNo1(aForm)
                End Select
                oMatrix.LoadFromDataSource()
                If aForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                    aForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                End If
                Exit Sub
            End If
        Next

    End Sub

#Region "Delete Row"
    Private Sub RefereshDeleteRow(ByVal aForm As SAPbouiCOM.Form)
        If Me.MatrixId = "11" Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_SALST1")
        Else
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_SALST2")
        End If
        'oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_PRJ1")
        If intSelectedMatrixrow <= 0 Then
            Exit Sub

        End If
        Me.RowtoDelete = intSelectedMatrixrow
        oDataSrc_Line.RemoveRecord(Me.RowtoDelete - 1)
        oMatrix = frmSourceMatrix
        oMatrix.FlushToDataSource()
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oMatrix.LoadFromDataSource()
        If oMatrix.RowCount > 0 Then
            oMatrix.DeleteRow(oMatrix.RowCount)
        End If
    End Sub
#End Region

    Private Sub reDrawForm(ByVal oForm As SAPbouiCOM.Form)
        Try
            oForm.Freeze(True)
            oForm.Items.Item("34").Width = oForm.Width - 30
            oForm.Items.Item("34").Height = oForm.Height - 170
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
        End Try
    End Sub

#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            If oApplication.Utilities.getEdittextvalue(aForm, "5") = "" Then
                oApplication.Utilities.Message("Enter Salary Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            oMatrix = oForm.Items.Item("11").Specific
            Dim strcode, strcode1 As Double
            If oMatrix.RowCount > 1 Then
                For intRow As Integer = 1 To oMatrix.RowCount
                    strcode = oApplication.Utilities.getMatrixValues(oMatrix, "V_2", intRow)
                    strcode1 = oApplication.Utilities.getMatrixValues(oMatrix, "V_3", intRow)
                    If strcode <> 0.0 And strcode1 <> 0.0 Then
                        oApplication.Utilities.Message("Enter either amount or  % of basic salary... ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        oMatrix.Columns.Item("V_1").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                        Return False
                    End If
                Next
            End If

            'If oApplication.Utilities.getEdittextvalue(aForm, "7") = "" Then
            '    oApplication.Utilities.Message("Name is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '    Return False
            'End If
            Dim oTemp1 As SAPbobsCOM.Recordset
            Dim stSQL1 As String
            oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            If aForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                stSQL1 = "Select * from [@Z_HR_OSALST] where U_Z_SalCode='" & oApplication.Utilities.getEdittextvalue(aForm, "5") & "'"
                oTemp1.DoQuery(stSQL1)
                If oTemp1.RecordCount > 0 Then
                    oApplication.Utilities.Message("Salary Code AlReady Exists...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                Dim oTemp As SAPbobsCOM.Recordset
                Dim stSQL As String
                oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                stSQL = "Select * from [@Z_HR_OSALST] where U_Z_GrdeCode='" & oApplication.Utilities.getEdittextvalue(oForm, "16") & "' And U_Z_LevlCode='" & oApplication.Utilities.getEdittextvalue(aForm, "7") & "'"
                oTemp.DoQuery(stSQL)
                If oTemp.RecordCount > 0 Then
                    oApplication.Utilities.Message("Salary Structure AlReady Mapped For Grade And Level...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            End If
            AssignLineNo(oForm)
            AssignLineNo1(oForm)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
#End Region
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_SalStru Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1" And (oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Or oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE) Then
                                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                                        If oApplication.SBO_Application.MessageBox("Do you want to confirm the information?", , "Yes", "No") = 2 Then
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    End If
                                    If Validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                If pVal.ItemUID = "37" Then
                                    oApplication.Utilities.OpenMasterinLink(oForm, "Level")
                                    BubbleEvent = False
                                    Exit Sub
                                ElseIf pVal.ItemUID = "38" Then
                                    oApplication.Utilities.OpenMasterinLink(oForm, "Grade")
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                If pVal.EventType = SAPbouiCOM.BoEventTypes.et_KEY_DOWN And pVal.CharPressed <> "9" And pVal.ItemUID = "5" Then
                                    Dim strVal As String
                                    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                    strVal = oApplication.Utilities.getEdittextvalue(oForm, "5")
                                    If strVal <> "" Then
                                        If oApplication.Utilities.ValidateCode(strVal, "SALARY") = True Then
                                            oApplication.Utilities.Message("Salary Code Already Mapped...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            oForm.Items.Item("5").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.GetForm(pVal.FormTypeEx, pVal.FormTypeCount)
                                If pVal.ItemUID = "11" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("11").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "11"
                                    frmSourceMatrix = oMatrix
                                End If
                                If pVal.ItemUID = "10" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("10").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "10"
                                    frmSourceMatrix = oMatrix
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                reDrawForm(oForm)
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                '' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "10" And pVal.ColUID = "V_0" Then

                                    oMatrix = oForm.Items.Item(pVal.ItemUID).Specific
                                    oCombobox = oMatrix.Columns.Item("V_0").Cells.Item(pVal.Row).Specific
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, oCombobox.Selected.Description)
                                End If
                                If pVal.ItemUID = "11" And pVal.ColUID = "V_0" Then
                                    oMatrix = oForm.Items.Item(pVal.ItemUID).Specific
                                    oCombobox = oMatrix.Columns.Item("V_0").Cells.Item(pVal.Row).Specific
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, oCombobox.Selected.Description)
                                End If


                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "8"
                                        oForm.PaneLevel = 1
                                    Case "9"
                                        oForm.PaneLevel = 2
                                    Case "33"
                                        oForm.PaneLevel = 3
                                    Case "12"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_ADD_ROW)
                                    Case "13"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_DELETE_ROW)
                                End Select
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1 As String
                                Dim sCHFL_ID, val As String
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
                                        If pVal.ItemUID = "7" Then
                                            val = oDataTable.GetValue("U_Z_LvelCode", 0)
                                            val1 = oDataTable.GetValue("U_Z_LvelName", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "7", val)
                                                oApplication.Utilities.setEdittextvalue(oForm, "14", val1)
                                            Catch ex As Exception
                                                oApplication.Utilities.setEdittextvalue(oForm, "14", val1)
                                            End Try
                                        End If
                                        If pVal.ItemUID = "16" Then
                                            val = oDataTable.GetValue("U_Z_GrdeCode", 0)
                                            val1 = oDataTable.GetValue("U_Z_GrdeName", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "16", val)
                                                oApplication.Utilities.setEdittextvalue(oForm, "18", val1)
                                            Catch ex As Exception
                                                oApplication.Utilities.setEdittextvalue(oForm, "18", val1)
                                            End Try
                                        End If
                                        'If pVal.ItemUID = "10" And pVal.ColUID = "V_0" Then
                                        '    val1 = oDataTable.GetValue("U_Z_BenefCode", 0)
                                        '    val = oDataTable.GetValue("U_Z_BenefName", 0)
                                        '    oMatrix = oForm.Items.Item("10").Specific
                                        '    Try
                                        '        oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val1)
                                        '        oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val)
                                        '    Catch ex As Exception
                                        '        oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val1)
                                        '        oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val)
                                        '    End Try

                                        'End If
                                        'If pVal.ItemUID = "11" And pVal.ColUID = "V_0" Then
                                        '    val = oDataTable.GetValue("U_Z_AlloName", 0)
                                        '    val1 = oDataTable.GetValue("U_Z_AlloCode", 0)
                                        '    oMatrix = oForm.Items.Item("11").Specific
                                        '    Try
                                        '        oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val1)
                                        '        oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val)
                                        '    Catch ex As Exception
                                        '        oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val1)
                                        '        oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val)
                                        '    End Try
                                        'End If
                                        oForm.Freeze(False)
                                    End If
                                Catch ex As Exception
                                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                        oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                    End If
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
                Case mnu_hr_SalStru
                    LoadForm()
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        oForm.Items.Item("5").Enabled = False
                        'oForm.Items.Item("7").Enabled = False
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
                    Else
                        'If ValidateDeletion(oForm) = False Then
                        '    BubbleEvent = False
                        '    Exit Sub
                        'End If
                    End If
                Case mnu_ADD
                    If pVal.BeforeAction = False Then
                        oForm = oApplication.SBO_Application.Forms.ActiveForm()
                        oForm.Items.Item("5").Enabled = True
                        oForm.Items.Item("7").Enabled = True
                        oForm.Items.Item("16").Enabled = True
                    End If
                Case mnu_FIND
                    If pVal.BeforeAction = False Then
                        oForm = oApplication.SBO_Application.Forms.ActiveForm()
                        oForm.Items.Item("5").Enabled = True
                        oForm.Items.Item("7").Enabled = True
                        oForm.Items.Item("16").Enabled = True
                    End If
                Case "1283"
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = True Then
                        Dim strValue As String
                        If oApplication.SBO_Application.MessageBox("Do you want to delete the details?", , "Yes", "No") = 2 Then
                            BubbleEvent = False
                            Exit Sub
                        End If
                        strValue = oApplication.Utilities.getEdittextvalue(oForm, "5")
                        If oApplication.Utilities.ValidateCode(strValue, "SALARY") = True Then
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

    Public Sub FormDataEvent(ByRef BusinessObjectInfo As SAPbouiCOM.BusinessObjectInfo, ByRef BubbleEvent As Boolean)
        Try
            If BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
                If oForm.TypeEx = frm_hr_SalStru Then
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oForm.Items.Item("5").Enabled = False
                    oForm.Items.Item("7").Enabled = False
                    oForm.Items.Item("16").Enabled = False
                End If
            End If
            If BusinessObjectInfo.BeforeAction = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD Or BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
                ' strDocEntry = oApplication.Utilities.getEdittextvalue(oForm, "4")
            End If
            If BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD Or BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
                Dim intDoc As Integer
                'intDoc = CInt(strDocEntry)
                ' UpdateMaster(intDoc)
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

End Class
