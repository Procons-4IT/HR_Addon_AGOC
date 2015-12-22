Public Class clshrTravelAgenda
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
    Private oColumn As SAPbouiCOM.Column
    Private InvBase As DocumentType
    Private MatrixId As String
    Private RowtoDelete As Integer
    Private InvBaseDocNo As String
    Private InvForConsumedItems, count As Integer
    Private blnFlag As Boolean = False
    Dim oDataSrc_Line As SAPbouiCOM.DBDataSource

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_TraAgenda) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_TraAgenda, frm_hr_TraAgenda)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.DataBrowser.BrowseBy = "4"
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.EnableMenu("1283", True)
        AddChooseFromList(oForm)
        databind(oForm)
        FillCurrency(oForm)
        'FillCostCenter(oForm)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_TRAPL1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oForm.Freeze(False)
    End Sub
    Public Sub LoadForm1(ByVal AgendaCode As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_TraAgenda) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_TraAgenda, frm_hr_TraAgenda)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        ' oForm.DataBrowser.BrowseBy = "4"
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.EnableMenu("1283", True)
        AddChooseFromList(oForm)
        databind(oForm)
        FillCurrency(oForm)
        ' FillCostCenter(oForm)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_TRAPL1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oForm.Items.Item("4").Enabled = True
        oApplication.Utilities.setEdittextvalue(oForm, "4", AgendaCode)
        oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE Then
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        End If
        oForm.Freeze(False)
    End Sub

#Region "Add Choose From List"
    Private Sub databind(ByVal aForm As SAPbouiCOM.Form)
        oMatrix = aForm.Items.Item("7").Specific
        oColumn = oMatrix.Columns.Item("V_0")
        oColumn.ChooseFromListUID = "CFL1"
        oColumn.ChooseFromListAlias = "U_Z_ExpName"
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
            oCFLCreationParams.ObjectType = "Z_HR_EXPANCES"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)

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
            oMatrix = aForm.Items.Item("7").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_TRAPL1")
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

    Private Sub FillCurrency(ByVal aForm As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        Dim oDBDataSrc As SAPbouiCOM.DBDataSource
        Dim oMatrix As SAPbouiCOM.Matrix
        oMatrix = aForm.Items.Item("7").Specific
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim oColum As SAPbouiCOM.Column
        oColum = oMatrix.Columns.Item("V_3")
        For intRow As Integer = oColum.ValidValues.Count - 1 To 0 Step -1
            oColum.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oColum.ValidValues.Add("", "")
        oTempRec.DoQuery("SELECT ""CurrCode"" As ""Code"", ""CurrName"" As ""Name"" FROM OCRN")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oColum.ValidValues.Add(oTempRec.Fields.Item("Code").Value, oTempRec.Fields.Item("Name").Value)
            oTempRec.MoveNext()
        Next
        oColum.DisplayDesc = True
    End Sub

    Private Sub FillCostCenter(ByVal aForm As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        Dim oDBDataSrc As SAPbouiCOM.DBDataSource
        oCombobox = aForm.Items.Item("11").Specific
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oTempRec.DoQuery("SELECT ""PrcCode"" As ""Code"", ""PrcName"" As ""Name"" FROM OPRC where ""DimCode""=1")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item("Code").Value, oTempRec.Fields.Item("Name").Value)
            oTempRec.MoveNext()
        Next
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_ValueDescription
    End Sub
#End Region
#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim strcode, stCode1, stCode As String
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
         
            If oApplication.Utilities.getEdittextvalue(aForm, "4") = "" Then
                oApplication.Utilities.Message("Enter Travel Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "6") = "" Then
                oApplication.Utilities.Message("Enter Travel Description..", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If

            If aForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                Dim oTemp As SAPbobsCOM.Recordset
                oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oTemp.DoQuery("Select * from [@Z_HR_OTRAPL]  where U_Z_TraCode='" & oApplication.Utilities.getEdittextvalue(aForm, "4") & "' ")
                If oTemp.RecordCount > 0 Then
                    oApplication.Utilities.Message("Travel Code is Already Exists...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            End If
            oMatrix = aForm.Items.Item("7").Specific
            If oMatrix.RowCount = 0 Then
                oApplication.Utilities.Message("Enter Line Details...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            Dim strcode2, strcode1 As String
            If oMatrix.RowCount > 1 Then
                strcode2 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount)
                strcode1 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount - 1)
                If strcode2.ToUpper = strcode1.ToUpper Then
                    oApplication.Utilities.Message("This Entry Already Exists...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    Return False
                End If
            End If
            AssignLineNo(oForm)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
#End Region
    Private Sub AddRow(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)

            oMatrix = aForm.Items.Item("7").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_TRAPL1")
            If oMatrix.RowCount <= 0 Then
                oMatrix.AddRow()
            End If
            oEditText = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
            Try
                If oEditText.Value <> "" Then
                    oMatrix.AddRow()
                    oMatrix.ClearRowData(oMatrix.RowCount)
                    'oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                    'oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                    'oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, "0")
                End If

            Catch ex As Exception
                aForm.Freeze(False)
                'oMatrix.AddRow()
            End Try
            oMatrix.FlushToDataSource()
            For count = 1 To oDataSrc_Line.Size
                oDataSrc_Line.SetValue("LineId", count - 1, count)
            Next
            oMatrix.LoadFromDataSource()
            oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            AssignLineNo(aForm)
            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)

        End Try
    End Sub
#Region "Delete Row"
    Private Sub RefereshDeleteRow(ByVal aForm As SAPbouiCOM.Form)
        If Me.MatrixId = "7" Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_TRAPL1")
        End If
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




#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_TraAgenda Then
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
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.GetForm(pVal.FormTypeEx, pVal.FormTypeCount)
                                If pVal.ItemUID = "7" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("7").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "7"
                                    frmSourceMatrix = oMatrix
                                End If

                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "11" And pVal.CharPressed = 9 Then
                                    'oMatrix = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim oObj As New clsDisRule
                                    oObj.SourceFormUID = FormUID
                                    oObj.ItemUID = pVal.ItemUID
                                    oObj.sourceColumID = ""
                                    oObj.sourcerowId = 0
                                    oObj.strStaticValue = oApplication.Utilities.getEdittextvalue(oForm, pVal.ItemUID)
                                    'If oApplication.Utilities.getMatrixValues(oMatrix, "V_7", pVal.Row) = "A" Then
                                    '    oObj.strformid = "Approved"
                                    'End If
                                    oApplication.Utilities.LoadForm(xml_DisRule, frm_DisRule)
                                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                                    oObj.databound(oForm)
                                   
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "8"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_ADD_ROW)
                                    Case "9"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_DELETE_ROW)
                                End Select
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1 As String
                                Dim sCHFL_ID, val As String
                                Dim val2 As Integer
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
                                        If pVal.ItemUID = "7" And pVal.ColUID = "V_0" Then
                                            val1 = oDataTable.GetValue("U_Z_ExpName", 0)
                                            val = oDataTable.GetValue("U_Z_ActCode", 0)
                                            oMatrix = oForm.Items.Item("7").Specific
                                            Try
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val1)
                                            Catch ex As Exception

                                            End Try

                                        End If
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
                Case mnu_hr_TraAgenda
                    LoadForm()
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        oForm.Items.Item("6").Enabled = False
                        oForm.Items.Item("4").Enabled = False
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
                        oForm.Items.Item("6").Enabled = True
                        oForm.Items.Item("4").Enabled = True
                        'oForm.Items.Item("8").Enabled = True
                    End If
                Case mnu_FIND
                    If pVal.BeforeAction = False Then
                        oForm = oApplication.SBO_Application.Forms.ActiveForm()
                        oForm.Items.Item("4").Enabled = True
                        oForm.Items.Item("6").Enabled = True
                    End If
                Case "1283"
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = True Then
                        Dim strValue As String
                        If oApplication.SBO_Application.MessageBox("Do you want to delete the details?", , "Yes", "No") = 2 Then
                            BubbleEvent = False
                            Exit Sub
                        End If
                        strValue = oApplication.Utilities.getEdittextvalue(oForm, "4")
                        If oApplication.Utilities.ValidateCode(strValue, "TRAPLAN") = True Then
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
                If oForm.TypeEx = frm_hr_TraAgenda Then
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oForm.Items.Item("4").Enabled = False
                    oForm.Items.Item("6").Enabled = True
                    '  oForm.Items.Item("8").Enabled = False
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
