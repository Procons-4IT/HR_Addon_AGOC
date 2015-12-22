Public Class clshrExitfrmInitialization
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox, oCombobox1, oCombobox2 As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private oColumn As SAPbouiCOM.Column
    Private InvBaseDocNo As String
    Private MatrixId As String
    Private RowtoDelete As Integer
    Private InvForConsumedItems, count As Integer
    Private blnFlag As Boolean = False
    Dim oDataSrc_Line1, oDataSrc_Line2, oDataSrc_Line As SAPbouiCOM.DBDataSource
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Public Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_ExitfrmInit) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_ExitfrmInit, frm_hr_ExitfrmInit)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.DataBrowser.BrowseBy = "4"
        AddChooseFromList(oForm)
        databind(oForm)
        FillDepartment(oForm)
        FillPosition(oForm)
        FillSubSidiary(oForm)
        FillTerReason(oForm)
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line1 = oForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM2")
        For count = 1 To oDataSrc_Line1.Size - 1
            oDataSrc_Line1.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line2 = oForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM4")
        For count = 1 To oDataSrc_Line2.Size - 1
            oDataSrc_Line2.SetValue("LineId", count - 1, count)
        Next
        'AddMode(oForm)
        oForm.PaneLevel = 4
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
            oForm.Items.Item("4").Enabled = False
            oForm.Items.Item("6").Enabled = False
        Else
            oForm.Items.Item("4").Enabled = True
            oForm.Items.Item("6").Enabled = True
        End If
        oForm.Items.Item("4").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oForm.Freeze(False)
    End Sub

    Public Sub LoadForm1(ByVal ExitCode As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_ExitfrmInit) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_ExitfrmInit, frm_hr_ExitfrmInit)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        ' oForm.DataBrowser.BrowseBy = "4"
        AddChooseFromList(oForm)
        databind(oForm)
        FillDepartment(oForm)
        FillPosition(oForm)
        FillSubSidiary(oForm)
        FillTerReason(oForm)
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line1 = oForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM2")
        For count = 1 To oDataSrc_Line1.Size - 1
            oDataSrc_Line1.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line2 = oForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM4")
        For count = 1 To oDataSrc_Line2.Size - 1
            oDataSrc_Line2.SetValue("LineId", count - 1, count)
        Next
        'AddMode(oForm)
        oForm.PaneLevel = 4
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oForm.Items.Item("4").Enabled = True
        oApplication.Utilities.setEdittextvalue(oForm, "4", ExitCode)
        oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE Then
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        End If
        oCombobox = oForm.Items.Item("29").Specific
        Dim status As String = oCombobox.Selected.Value
        If status = "E" Then
            oForm.Items.Item("1").Visible = False
        Else
            oForm.Items.Item("1").Visible = True
        End If
        oForm.Freeze(False)
    End Sub
    Private Sub AddMode(ByVal aform As SAPbouiCOM.Form)
        Dim strCode As String
        strCode = oApplication.Utilities.getMaxCode("@Z_HR_OEXFOM", "DocEntry")
        aform.Items.Item("4").Enabled = True
        aform.Items.Item("6").Enabled = True
        oApplication.Utilities.setEdittextvalue(aform, "4", strCode)
        aform.Items.Item("6").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oApplication.Utilities.setEdittextvalue(aform, "6", "t")
        oApplication.SBO_Application.SendKeys("{TAB}")
        oForm.Items.Item("4").Enabled = True
        aform.Items.Item("8").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        aform.Items.Item("4").Enabled = False
        aform.Items.Item("6").Enabled = False
        ' oApplication.Utilities.setEdittextvalue(aform, "8", "")
        oForm.Items.Item("1").Visible = True
    End Sub
    Private Sub FillDepartment(ByVal sform As SAPbouiCOM.Form)
        Dim oMatrix As SAPbouiCOM.Matrix
        oMatrix = sform.Items.Item("32").Specific
        Dim oColum As SAPbouiCOM.Column
        oColum = oMatrix.Columns.Item("V_3")
        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = sform.Items.Item("16").Specific
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
            oColum.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oColum.ValidValues.Add("", "")
        oTempRec.DoQuery("Select ""Code"",""Remarks"" from OUDP")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item("Code").Value, oTempRec.Fields.Item("Remarks").Value)
            oColum.ValidValues.Add(oTempRec.Fields.Item("Code").Value, oTempRec.Fields.Item("Remarks").Value)
            oTempRec.MoveNext()
        Next
        sform.Items.Item("16").DisplayDesc = True
        oColum.DisplayDesc = True
    End Sub
    Private Sub FillObjectLoan(ByVal aform As SAPbouiCOM.Form, ByVal Empid As String)
        Try
            Dim strQuery As String
            Dim oMatrix As SAPbouiCOM.Matrix
            oMatrix = aform.Items.Item("32").Specific
            Dim oRec As SAPbobsCOM.Recordset

            oMatrix.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strQuery = "Select * from ""@Z_HR_OBJLOAN"" where ""U_Z_HREmpID""='" & Empid & "'"
            oRec.DoQuery(strQuery)
            If oRec.RecordCount > 0 Then
                oMatrix.Clear()
                oMatrix.FlushToDataSource()
                oMatrix.LoadFromDataSource()
                oMatrix.AddRow()
                oMatrix.ClearRowData(oMatrix.RowCount)
                For introw As Integer = 0 To oRec.RecordCount - 1
                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_-1", oMatrix.RowCount, introw + 1)
                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, oRec.Fields.Item("U_Z_ObjCode").Value)
                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, oRec.Fields.Item("U_Z_ObjName").Value)
                    oCombobox = oMatrix.Columns.Item("V_3").Cells.Item(oMatrix.RowCount).Specific
                    Try
                        oCombobox.Select(oRec.Fields.Item("U_Z_Dept").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
                    Catch ex As Exception
                        oCombobox.Select("", SAPbouiCOM.BoSearchKey.psk_ByValue)
                    End Try
                    oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_ValueDescription
                    Try
                        oApplication.Utilities.SetMatrixValues(oMatrix, "V_4", oMatrix.RowCount, oRec.Fields.Item("U_Z_ResID").Value)
                    Catch ex As Exception
                        oApplication.Utilities.SetMatrixValues(oMatrix, "V_4", oMatrix.RowCount, oRec.Fields.Item("U_Z_ResID").Value)
                    End Try
                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_5", oMatrix.RowCount, oRec.Fields.Item("U_Z_ResName").Value)
                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_6", oMatrix.RowCount, oRec.Fields.Item("U_Z_Remarks").Value)
                    'oApplication.Utilities.SetMatrixValues(oMatrix, "V_8", oMatrix.RowCount,  ""_'oRec.Fields.Item("U_Z_ApprovedBy").Value)
                    'oApplication.Utilities.SetMatrixValues(oMatrix, "V_9", oMatrix.RowCount, oRec.Fields.Item("U_Z_Appdt").Value)
                    oCombobox1 = oMatrix.Columns.Item("V_7").Cells.Item(oMatrix.RowCount).Specific
                    Try
                        oCombobox1.Select(oRec.Fields.Item("U_Z_CompStatus").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
                    Catch ex As Exception
                        oCombobox1.Select("", SAPbouiCOM.BoSearchKey.psk_ByValue)
                    End Try
                    oCombobox1.ExpandType = SAPbouiCOM.BoExpandType.et_ValueDescription
                    oMatrix.AddRow()
                    oRec.MoveNext()
                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_-1", oMatrix.RowCount, oMatrix.RowCount)
                Next
            Else
                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, "")
                oCombobox = oMatrix.Columns.Item("V_3").Cells.Item(oMatrix.RowCount).Specific
                Try
                    oCombobox.Select("", SAPbouiCOM.BoSearchKey.psk_ByValue)
                Catch ex As Exception
                    oCombobox.Select("", SAPbouiCOM.BoSearchKey.psk_ByValue)
                End Try

                Try
                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_4", oMatrix.RowCount, "")
                Catch ex As Exception
                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_4", oMatrix.RowCount, "")
                End Try
                oApplication.Utilities.SetMatrixValues(oMatrix, "V_5", oMatrix.RowCount, "")
                oApplication.Utilities.SetMatrixValues(oMatrix, "V_6", oMatrix.RowCount, "")
                oApplication.Utilities.SetMatrixValues(oMatrix, "V_8", oMatrix.RowCount, "")
                oApplication.Utilities.SetMatrixValues(oMatrix, "V_9", oMatrix.RowCount, "")
                oCombobox1 = oMatrix.Columns.Item("V_7").Cells.Item(oMatrix.RowCount).Specific
                Try
                    oCombobox1.Select("", SAPbouiCOM.BoSearchKey.psk_ByValue)
                Catch ex As Exception
                    oCombobox1.Select("", SAPbouiCOM.BoSearchKey.psk_ByValue)
                End Try
                oCombobox1.ExpandType = SAPbouiCOM.BoExpandType.et_ValueDescription
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Sub FillPosition(ByVal sform As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = sform.Items.Item("20").Specific
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oTempRec.DoQuery("Select posID,descriptio From OHPS")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item("posID").Value, oTempRec.Fields.Item("descriptio").Value)
            oTempRec.MoveNext()
        Next
        sform.Items.Item("20").DisplayDesc = True

    End Sub
    Private Sub FillSubSidiary(ByVal sform As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = sform.Items.Item("37").Specific
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oTempRec.DoQuery("Select ""Code"",""Name"" From OUBR")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item(0).Value, oTempRec.Fields.Item(1).Value)
            oTempRec.MoveNext()
        Next
        sform.Items.Item("37").DisplayDesc = True

    End Sub
    Private Sub FillTerReason(ByVal sform As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = sform.Items.Item("41").Specific
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oTempRec.DoQuery("Select ""reasonID"",""name"" From OHTR")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item(0).Value, oTempRec.Fields.Item(1).Value)
            oTempRec.MoveNext()
        Next
        sform.Items.Item("41").DisplayDesc = True

    End Sub

#Region "Add Choose From List"
    Private Sub databind(ByVal aForm As SAPbouiCOM.Form)
        oMatrix = aForm.Items.Item("23").Specific
        oColumn = oMatrix.Columns.Item("V_4")
        oColumn.ChooseFromListUID = "CFL1"
        oColumn.ChooseFromListAlias = "U_Z_ResCode"

        oMatrix = aForm.Items.Item("24").Specific
        oColumn = oMatrix.Columns.Item("V_0")
        oColumn.ChooseFromListUID = "CFL3"
        oColumn.ChooseFromListAlias = "U_Z_QusCode"

        oEditText = aForm.Items.Item("8").Specific
        oEditText.ChooseFromListUID = "CFL2"
        oEditText.ChooseFromListAlias = "empID"

        oMatrix = aForm.Items.Item("32").Specific
        oColumn = oMatrix.Columns.Item("V_4")
        oColumn.ChooseFromListUID = "CFL4"
        oColumn.ChooseFromListAlias = "empID"
    End Sub

    Private Sub AddChooseFromList(ByVal aform As SAPbouiCOM.Form)
        Try
            Dim oCFLs As SAPbouiCOM.ChooseFromListCollection
            Dim oCons As SAPbouiCOM.Conditions
            Dim oCon As SAPbouiCOM.Condition

            oCFLs = aform.ChooseFromLists

            Dim oCFL As SAPbouiCOM.ChooseFromList
            Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams

            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)

            ' Adding 3 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_ORES"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "171"
            oCFLCreationParams.UniqueID = "CFL2"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_OQUS"
            oCFLCreationParams.UniqueID = "CFL3"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "171"
            oCFLCreationParams.UniqueID = "CFL4"
            oCFL = oCFLs.Add(oCFLCreationParams)



        Catch ex As Exception

        End Try
    End Sub


#End Region

#Region "AddRow/Delete Row"
    Private Sub AssignLineNo(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("23").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM1")
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
            oMatrix = aForm.Items.Item("24").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM2")
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
    Private Sub AssignLineNo2(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("32").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM4")
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
    Private Sub AddRow(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)

            Select Case aForm.PaneLevel
                Case "1"
                    oMatrix = aForm.Items.Item("23").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM1")
                    If oMatrix.RowCount <= 0 Then
                        oMatrix.AddRow()
                    End If
                    oEditText = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
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
                    oMatrix = aForm.Items.Item("24").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM2")

                    If oMatrix.RowCount <= 0 Then
                        oMatrix.AddRow()
                    End If
                    oEditText = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
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
                        'oMatrix.AddRow()
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
                oMatrix = aForm.Items.Item("23").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM1")
            Case "2"
                oMatrix = aForm.Items.Item("24").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM2")
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
                        oMatrix = aForm.Items.Item("23").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM1")
                        AssignLineNo(aForm)
                    Case "2"
                        oMatrix = aForm.Items.Item("24").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM2")
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
        If Me.MatrixId = "23" Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM1")
        ElseIf Me.MatrixId = "24" Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM2")
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
#End Region
#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim strcode, strDivision As String
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            If oApplication.Utilities.getEdittextvalue(aForm, "8") = "" Then
                oApplication.Utilities.Message("Enter Employee Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            Dim oTemp As SAPbobsCOM.Recordset
            Dim stSQL As String
            If aForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                AddMode(aForm)
                oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                stSQL = "Select * from [@Z_HR_OEXFOM] where U_Z_empID='" & oApplication.Utilities.getEdittextvalue(aForm, "8") & "'"
                oTemp.DoQuery(stSQL)
                If oTemp.RecordCount > 0 Then
                    oApplication.Utilities.Message("Employee Code Already Exists", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            End If
        Catch ex As Exception

        End Try
        Return True
    End Function
#End Region
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_ExitfrmInit Then
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
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE And (pVal.ItemUID = "4" Or pVal.ItemUID = "6") And pVal.CharPressed <> 9 Then
                                    BubbleEvent = False
                                    Exit Sub
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.GetForm(pVal.FormTypeEx, pVal.FormTypeCount)
                                If pVal.ItemUID = "23" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("23").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "23"
                                    frmSourceMatrix = oMatrix
                                End If
                                If pVal.ItemUID = "24" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("24").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "24"
                                    frmSourceMatrix = oMatrix
                                End If

                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)



                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                Select Case pVal.ItemUID
                                    Case "1"
                                        If oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                                            AddMode(oForm)
                                        End If
                                    Case "21"
                                        oForm.PaneLevel = 1
                                    Case "22"
                                        oForm.PaneLevel = 2
                                    Case "31"
                                        oForm.PaneLevel = 3
                                    Case "33"
                                        oForm.PaneLevel = 4
                                    Case "25"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_ADD_ROW)
                                    Case "26"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_DELETE_ROW)
                                End Select

                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1 As String
                                Dim sCHFL_ID, val, val2, val3, val4, val5, val6, val7, val8, val9 As String
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
                                        If pVal.ItemUID = "8" Then
                                            val1 = oDataTable.GetValue("firstName", 0)
                                            val2 = oDataTable.GetValue("middleName", 0)
                                            val3 = oDataTable.GetValue("lastName", 0)
                                            val4 = oDataTable.GetValue("jobTitle", 0)
                                            val5 = oDataTable.GetValue("position", 0)
                                            val6 = oDataTable.GetValue("dept", 0)
                                            val7 = oDataTable.GetValue("branch", 0)
                                            val8 = oDataTable.GetValue("U_Z_LvlName", 0)
                                            val9 = oDataTable.GetValue("startDate", 0)
                                            val = oDataTable.GetValue("empID", 0)
                                            Try
                                                FillObjectLoan(oForm, val)
                                                oCombobox = oForm.Items.Item("16").Specific
                                                oCombobox1 = oForm.Items.Item("20").Specific
                                                oCombobox2 = oForm.Items.Item("37").Specific
                                                oCombobox2.Select(val7, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                                oCombobox.Select(val6, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                                oCombobox1.Select(val5, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                                If val9 <> Nothing Then
                                                    oApplication.Utilities.setEdittextvalue(oForm, "39", oDataTable.GetValue("startDate", 0))
                                                Else
                                                    oApplication.Utilities.setEdittextvalue(oForm, "39", "")
                                                End If

                                                oApplication.Utilities.setEdittextvalue(oForm, "18", val4)
                                                oApplication.Utilities.setEdittextvalue(oForm, "35", val8)
                                                oApplication.Utilities.setEdittextvalue(oForm, "10", val1)
                                                oApplication.Utilities.setEdittextvalue(oForm, "12", val2)
                                                oApplication.Utilities.setEdittextvalue(oForm, "14", val3)
                                                Try
                                                    oApplication.Utilities.setEdittextvalue(oForm, "8", val)
                                                Catch ex As Exception

                                                End Try

                                                Dim dtdate1 As Date = oDataTable.GetValue("startDate", 0)
                                                oForm.Items.Item("39").Enabled = True
                                                oEditText = oForm.Items.Item("39").Specific
                                                Dim s As String = dtdate1.ToString("yyyyMMdd")
                                                Dim oDBDataSource As SAPbouiCOM.DBDataSource
                                                oDBDataSource = oForm.DataSources.DBDataSources.Item("@Z_HR_OEXFOM")
                                                'oEditText.Value = s ' dtdate1.ToString("dd.MM.yy")
                                                oDBDataSource.SetValue("U_Z_JoinDate", 0, s)

                                                oForm.Items.Item("45").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                                oForm.Items.Item("39").Enabled = False

                                            Catch ex As Exception
                                                oForm.Freeze(False)
                                            End Try
                                        End If
                                        If pVal.ItemUID = "23" And pVal.ColUID = "V_4" Then
                                            val1 = oDataTable.GetValue("U_Z_DeptName", 0)
                                            val2 = oDataTable.GetValue("U_Z_PosCode", 0)
                                            val3 = oDataTable.GetValue("U_Z_PosName", 0)
                                            val4 = oDataTable.GetValue("U_Z_ResCode", 0)
                                            val5 = oDataTable.GetValue("U_Z_ResDesc", 0)
                                            val6 = oDataTable.GetValue("U_Z_ResID", 0)
                                            val7 = oDataTable.GetValue("U_Z_ResName", 0)
                                            val = oDataTable.GetValue("U_Z_DeptCode", 0)
                                            oMatrix = oForm.Items.Item(pVal.ItemUID).Specific
                                            Try
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_5", pVal.Row, val5)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_3", pVal.Row, val3)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", pVal.Row, val2)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val1)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_11", pVal.Row, val7)
                                                Try
                                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_12", pVal.Row, val6)
                                                Catch ex As Exception
                                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_12", pVal.Row, val6)
                                                End Try
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_4", pVal.Row, val4)
                                            Catch ex As Exception
                                                oForm.Freeze(False)
                                            End Try
                                        End If
                                        If pVal.ItemUID = "24" And pVal.ColUID = "V_0" Then
                                            val = oDataTable.GetValue("U_Z_QusCode", 0)
                                            val1 = oDataTable.GetValue("U_Z_QusName", 0)
                                            Try
                                                oMatrix = oForm.Items.Item(pVal.ItemUID).Specific
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val1)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val)
                                            Catch ex As Exception
                                                oForm.Freeze(False)
                                            End Try
                                        End If
                                        If pVal.ItemUID = "32" And pVal.ColUID = "V_4" Then
                                            val = oDataTable.GetValue("empID", 0)
                                            val1 = oDataTable.GetValue("firstName", 0) & " " & oDataTable.GetValue("middleName", 0) & " " & oDataTable.GetValue("lastName", 0)
                                            Try
                                                oMatrix = oForm.Items.Item(pVal.ItemUID).Specific
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_5", pVal.Row, val1)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_4", pVal.Row, val)
                                            Catch ex As Exception
                                                oForm.Freeze(False)
                                            End Try
                                        End If
                                        If pVal.ItemUID = "23" And pVal.ColUID = "V_12" Then
                                            val = oDataTable.GetValue("empID", 0)
                                            val1 = oDataTable.GetValue("firstName", 0) & " " & oDataTable.GetValue("middleName", 0) & " " & oDataTable.GetValue("lastName", 0)
                                            Try
                                                oMatrix = oForm.Items.Item(pVal.ItemUID).Specific
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_11", pVal.Row, val1)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_12", pVal.Row, val)
                                            Catch ex As Exception
                                                oForm.Freeze(False)
                                            End Try
                                        End If
                                        oForm.Freeze(False)
                                    End If
                                Catch ex As Exception
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
                Case mnu_hr_ExitfrmInit
                    LoadForm()
                Case mnu_ADD
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                        oForm.Items.Item("4").Enabled = False
                        oForm.Items.Item("6").Enabled = False
                    End If
                    If pVal.BeforeAction = False Then
                        AddMode(oForm)
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
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If Form.TypeEx = frm_hr_ExitfrmInit Then
                        oCombobox = oForm.Items.Item("29").Specific
                        Dim strstatus As String = oCombobox.Selected.Value
                        If strstatus = "E" Then
                            oForm.Items.Item("1").Visible = False
                        Else
                            oForm.Items.Item("1").Visible = True
                        End If
                    End If
                    If pVal.BeforeAction = False Then
                        oForm.Items.Item("4").Enabled = False
                        oForm.Items.Item("6").Enabled = False
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
                Try
                    If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                        oForm.Items.Item("4").Enabled = False
                        oForm.Items.Item("6").Enabled = False
                    End If
                Catch ex As Exception
                End Try
            End If
            If BusinessObjectInfo.BeforeAction = False And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD Or BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
                If oForm.TypeEx = frm_hr_ExitfrmInit Then
                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                        AddMode(oForm)
                    End If
                End If
            End If
            If BusinessObjectInfo.BeforeAction = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
                If oForm.TypeEx = frm_hr_ExitfrmInit Then
                    oForm.Items.Item("8").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    oForm.Items.Item("4").Enabled = False
                    oForm.Items.Item("6").Enabled = False
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
End Class
