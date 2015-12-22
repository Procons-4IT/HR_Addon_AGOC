Public Class clshrCompObjective
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix, oMatrix1 As SAPbouiCOM.Matrix
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
    Private InvForConsumedItems, count As Integer
    Private blnFlag As Boolean = False
    Dim oDataSrc_Line As SAPbouiCOM.DBDataSource
    Dim oDataSrc_Line1 As SAPbouiCOM.DBDataSource
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_CompObj) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_CompObj, frm_hr_CompObj)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.DataBrowser.BrowseBy = "5"
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        FillLevels(oForm)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COB1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line1 = oForm.DataSources.DBDataSources.Item("@Z_HR_COB2")
        For count = 1 To oDataSrc_Line1.Size - 1
            oDataSrc_Line1.SetValue("LineId", count - 1, count)
        Next
        oForm.PaneLevel = 1
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oForm.Freeze(False)
    End Sub


    Private Sub FillLevels(ByVal aForm As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        Dim oDBDataSrc As SAPbouiCOM.DBDataSource
        Dim oMatrix As SAPbouiCOM.Matrix
        oMatrix = aForm.Items.Item("11").Specific
        ' oDBDataSrc = objForm.DataSources.DBDataSources.Add("@Z_HR_ORGST")
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim oColum As SAPbouiCOM.Column
        oColum = oMatrix.Columns.Item("V_0")
        For intRow As Integer = oColum.ValidValues.Count - 1 To 0 Step -1
            oColum.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oColum.ValidValues.Add("", "")
        oTempRec.DoQuery("select U_Z_LvelCode,U_Z_LvelName  from [@Z_HR_COLVL] order by U_Z_LvelCode")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oColum.ValidValues.Add(oTempRec.Fields.Item("U_Z_LvelCode").Value, oTempRec.Fields.Item("U_Z_LvelName").Value)
            oTempRec.MoveNext()
        Next
        oColum.DisplayDesc = True
        oMatrix.AutoResizeColumns()

        oMatrix = aForm.Items.Item("10").Specific
        ' oDBDataSrc = objForm.DataSources.DBDataSources.Add("@Z_HR_ORGST")
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oColum = oMatrix.Columns.Item("V_2")
        For intRow As Integer = oColum.ValidValues.Count - 1 To 0 Step -1
            oColum.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oColum.ValidValues.Add("", "")
        oTempRec.DoQuery("select U_Z_LvelCode,U_Z_LvelName  from [@Z_HR_COLVL] order by U_Z_LvelCode")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oColum.ValidValues.Add(oTempRec.Fields.Item("U_Z_LvelCode").Value, oTempRec.Fields.Item("U_Z_LvelName").Value)
            oTempRec.MoveNext()
        Next
        oColum.DisplayDesc = True
        oMatrix.AutoResizeColumns()

    End Sub
#Region "Methods"
    Private Sub AssignLineNo(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("11").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COB1")
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
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COB2")
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
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COB1")
                    If oMatrix.RowCount <= 0 Then
                        oMatrix.AddRow()
                    End If
                    oCombobox = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                    Try
                        If oCombobox.Value <> "" Then
                            oMatrix.AddRow()
                            Select Case aForm.PaneLevel
                                Case "1"
                                    ' oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oCombobox = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                                    oCombobox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)

                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "0")
                                Case "2"
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                            End Select
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
                Case "2"
                    oMatrix = aForm.Items.Item("10").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COB2")

                    If oMatrix.RowCount <= 0 Then
                        oMatrix.AddRow()
                    End If
                    oEditText = oMatrix.Columns.Item("V_10").Cells.Item(oMatrix.RowCount).Specific
                    Try
                        If oEditText.Value <> "" Then
                            oMatrix.AddRow()
                            Select Case aForm.PaneLevel
                                Case "1"
                                    ' oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "0")
                                Case "2"
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_10", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                                    oCombobox = oMatrix.Columns.Item("V_2").Cells.Item(oMatrix.RowCount).Specific
                                    oCombobox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
                            End Select
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
                    oMatrix.Columns.Item("V_10").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
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
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_COB1")
            Case "2"
                oMatrix = aForm.Items.Item("10").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_COB2")
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
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_COB1")
                        AssignLineNo(aForm)
                    Case "2"
                        oMatrix = aForm.Items.Item("10").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_COB2")
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
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COB1")
            oMatrix = aForm.Items.Item("11").Specific
            Me.RowtoDelete = intSelectedMatrixrow
            oDataSrc_Line.RemoveRecord(Me.RowtoDelete - 1)
            oMatrix.LoadFromDataSource()
            oMatrix.FlushToDataSource()
            For count = 1 To oDataSrc_Line.Size
                oDataSrc_Line.SetValue("LineId", count - 1, count)
            Next
        Else
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COB2")
            oMatrix = aForm.Items.Item("10").Specific
            Me.RowtoDelete = intSelectedMatrixrow
            oDataSrc_Line.RemoveRecord(Me.RowtoDelete - 1)
            oMatrix.LoadFromDataSource()
            oMatrix.FlushToDataSource()
            For count = 1 To oDataSrc_Line.Size
                oDataSrc_Line.SetValue("LineId", count - 1, count)
            Next
        End If
        oMatrix.LoadFromDataSource()
        'If intSelectedMatrixrow <= 0 Then
        '    Exit Sub

        'End If
        'Me.RowtoDelete = intSelectedMatrixrow
        'oDataSrc_Line.RemoveRecord(Me.RowtoDelete - 1)
        'oMatrix = frmSourceMatrix
        'oMatrix.FlushToDataSource()
        'For count = 1 To oDataSrc_Line.Size - 1
        '    oDataSrc_Line.SetValue("LineId", count - 1, count)
        'Next
        'oMatrix.LoadFromDataSource()
        'If oMatrix.RowCount > 0 Then
        '    oMatrix.DeleteRow(oMatrix.RowCount)
        'End If
    End Sub
#End Region


#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            If oApplication.Utilities.getEdittextvalue(aForm, "5") = "" Then
                oApplication.Utilities.Message("Enter Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "7") = "" Then
                oApplication.Utilities.Message("Enter Name...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            oMatrix = oForm.Items.Item("11").Specific
            oMatrix1 = oForm.Items.Item("10").Specific

            If oMatrix.RowCount = 0 Or oMatrix1.RowCount = 0 Then
                oApplication.Utilities.Message("Enter Line Details...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
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
            If pVal.FormTypeEx = frm_hr_CompObj Then
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
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                '' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "8"
                                        oForm.PaneLevel = 1
                                    Case "9"
                                        oForm.PaneLevel = 2
                                    Case "12"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_ADD_ROW)
                                    Case "13"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_DELETE_ROW)
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
                Case mnu_hr_CompObj
                    LoadForm()
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        oForm.Items.Item("5").Enabled = False
                        oForm.Items.Item("7").Enabled = False
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
                        'oForm.Items.Item("8").Enabled = True
                    End If
                Case mnu_FIND
                    If pVal.BeforeAction = False Then
                        oForm = oApplication.SBO_Application.Forms.ActiveForm()
                        oForm.Items.Item("5").Enabled = True
                        oForm.Items.Item("7").Enabled = True
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
                If oForm.TypeEx = frm_hr_CompObj Then
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oForm.Items.Item("5").Enabled = False
                    oForm.Items.Item("7").Enabled = True
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
