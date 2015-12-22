Public Class clshrTrainner
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
    Private oColumn As SAPbouiCOM.Column
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private MatrixId As String
    Private InvForConsumedItems, count As Integer
    Private RowtoDelete As Integer
    Private sPath, strSelectedFilepath, strSelectedFolderPath As String
    Private blnFlag As Boolean = False
    Dim oDataSrc_Line, oDataSrc_Line3 As SAPbouiCOM.DBDataSource
    Dim oDataSrc_Line1, oDataSrc_Line2, oDataSrc_Line4, oDataSrc_Line5 As SAPbouiCOM.DBDataSource
    Dim dt As Date
    Dim oCheckBox As SAPbouiCOM.CheckBox
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_HR_Trainner) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_Trainner, frm_HR_Trainner)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        AddChooseFromList(oForm)
        oForm.EnableMenu("1283", True)
        'oEditText = oForm.Items.Item("73").Specific
        'oEditText.ChooseFromListUID = "CFL1"
        'oEditText.ChooseFromListAlias = "DocEntry"
        '  FillDepartment(oForm)
        ' FillPosition(oForm)
        FillCountry(oForm)
        '  FillEducationType(oForm)
        oForm.DataBrowser.BrowseBy = "4"
        oForm.PaneLevel = 1
        oForm.Items.Item("38").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
        oForm.Items.Item("39").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
        oForm.Items.Item("1000005").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oForm.Items.Item("4").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oForm.Freeze(False)
    End Sub
    Public Sub ViewCandidate(ByVal aCode As String)
        oForm = oApplication.Utilities.LoadForm(xml_hr_Trainner, frm_HR_Trainner)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        AddChooseFromList(oForm)
        oForm.Items.Item("38").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
        oForm.Items.Item("39").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
        FillCountry(oForm)
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oForm.Items.Item("4").Enabled = True
        oApplication.Utilities.setEdittextvalue(oForm, "4", aCode)
        oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE Then
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        End If
        oForm.Items.Item("1000005").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        'oForm.PaneLevel = 1
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

            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "U_Z_MgrStatus"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "HA"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub AddMode(ByVal aform As SAPbouiCOM.Form)
        Dim strCode As String
        Try
            aform.Freeze(True)

            strCode = oApplication.Utilities.getMaxCode("@Z_HR_TRRAPP", "DocEntry")
            aform.Items.Item("4").Enabled = True
            aform.Items.Item("6").Enabled = True
            oApplication.Utilities.setEdittextvalue(aform, "4", strCode)
            aform.Items.Item("6").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oApplication.Utilities.setEdittextvalue(aform, "6", "t")
            oApplication.SBO_Application.SendKeys("{TAB}")
            aform.Items.Item("8").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            aform.Items.Item("4").Enabled = False
            aform.Items.Item("6").Enabled = False
            oCheckBox = aform.Items.Item("1000010").Specific
            oCheckBox.Checked = True

            aform.Freeze(False)
            oForm.Update()
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)

        End Try
    End Sub
    Private Sub fillopen()
        Dim mythr As New System.Threading.Thread(AddressOf ShowFileDialog)
        mythr.SetApartmentState(Threading.ApartmentState.STA)
        mythr.Start()
        mythr.Join()

    End Sub

    Private Sub ShowFileDialog()
        Dim oDialogBox As New OpenFileDialog
        Dim strFileName, strMdbFilePath As String
        Dim oProcesses() As Process
        Try
            oProcesses = Process.GetProcessesByName("SAP Business One")
            If oProcesses.Length <> 0 Then
                For i As Integer = 0 To oProcesses.Length - 1
                    Dim MyWindow As New clsListener.WindowWrapper(oProcesses(i).MainWindowHandle)
                    If oDialogBox.ShowDialog(MyWindow) = DialogResult.OK Then
                        strMdbFilePath = oDialogBox.FileName
                        strSelectedFilepath = oDialogBox.FileName
                        strFileName = strSelectedFilepath
                        strSelectedFolderPath = strFileName
                    Else
                    End If
                Next
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
        End Try
    End Sub
#Region "Methods"
    Private Sub FillDepartment(ByVal sform As SAPbouiCOM.Form)
        oCombobox = sform.Items.Item("12").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select ""Code"",""Name"" from OUDP order by ""Code""")
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        sform.Items.Item("12").DisplayDesc = True
    End Sub
    Private Sub FillPosition(ByVal sform As SAPbouiCOM.Form)
        oCombobox = sform.Items.Item("14").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select ""U_Z_PosCode"",""U_Z_PosName"" from ""@Z_HR_OPOSIN"" order by ""DocEntry""")
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        sform.Items.Item("14").DisplayDesc = True
    End Sub
    Private Sub FillCountry(ByVal sform As SAPbouiCOM.Form)
        'oMatrix = sform.Items.Item("60").Specific
        Dim oColum As SAPbouiCOM.Column
        'oColum = oMatrix.Columns.Item("V_2")
        oCombobox = sform.Items.Item("36").Specific
        oCombobox1 = sform.Items.Item("53").Specific
        oCombobox2 = sform.Items.Item("55").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select ""Code"",""Name"" from OCRY order by ""Code""")
        'For intRow As Integer = oColum.ValidValues.Count - 1 To 0 Step -1
        '    '    'oColum.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        '    oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        '    oCombobox1.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        '    oCombobox2.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        'Next
        ' oColum.ValidValues.Add("", "")
        oCombobox.ValidValues.Add("", "")
        oCombobox1.ValidValues.Add("", "")
        oCombobox2.ValidValues.Add("", "")

        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            ' oColum.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oCombobox1.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oCombobox2.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        sform.Items.Item("36").DisplayDesc = True
        sform.Items.Item("53").DisplayDesc = True
        sform.Items.Item("55").DisplayDesc = True
        ' oColum.DisplayDesc = True
    End Sub
    Private Sub FillEducationType(ByVal sform As SAPbouiCOM.Form)
        oMatrix = sform.Items.Item("60").Specific
        Dim oColum As SAPbouiCOM.Column
        oColum = oMatrix.Columns.Item("V_2")
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery(" select ""edType"",""name"" from OHED ")
        For intRow As Integer = oColum.ValidValues.Count - 1 To 0 Step -1
            oColum.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oColum.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oColum.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        oColum.DisplayDesc = True
    End Sub
#End Region

#Region "Methods"
    Private Sub AssignLineNo(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("57").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP1")
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
            oMatrix = aForm.Items.Item("58").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP2")
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
            oMatrix = aForm.Items.Item("60").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP3")
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
    Private Sub AssignLineNo3(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("61").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP4")
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

    Private Sub AssignLineNo4(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("62").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP5")
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
    Private Sub AssignLineNo5(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("76").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP6")
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

#Region "Add Row/ Delete Row"
    Private Sub AddRow(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)

            Select Case aForm.PaneLevel
                Case "1"
                    oMatrix = aForm.Items.Item("57").Specific
                    oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_TRRAPP1")
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
                    oMatrix = aForm.Items.Item("58").Specific
                    oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP2")

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
                Case "3"
                    oMatrix = aForm.Items.Item("60").Specific
                    oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP3")
                    If oMatrix.RowCount <= 0 Then
                        oMatrix.AddRow()
                    End If
                    oEditText = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                    Try
                        If oEditText.Value <> "" Then
                            oMatrix.AddRow()
                            Select Case aForm.PaneLevel
                                Case "3"
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
                    AssignLineNo2(aForm)
                Case "4"
                    oMatrix = aForm.Items.Item("61").Specific
                    oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP4")
                    If oMatrix.RowCount <= 0 Then
                        oMatrix.AddRow()
                    End If
                    oEditText = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                    Try
                        If oEditText.Value <> "" Then
                            oMatrix.AddRow()
                            Select Case aForm.PaneLevel
                                Case "4"
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
                    AssignLineNo3(aForm)
                Case "5"
                    oMatrix = aForm.Items.Item("76").Specific
                    oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP6")
                    If oMatrix.RowCount <= 0 Then
                        oMatrix.AddRow()
                    End If
                    oEditText = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                    Try
                        If oEditText.Value <> "" Then
                            oMatrix.AddRow()
                            Select Case aForm.PaneLevel
                                Case "5"
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
                    AssignLineNo5(aForm)
                Case "6"
                    oMatrix = aForm.Items.Item("62").Specific
                    oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP5")
                    If oMatrix.RowCount <= 0 Then
                        oMatrix.AddRow()
                    End If
                    oEditText = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                    Try
                        If oEditText.Value <> "" Then
                            oMatrix.AddRow()
                            Select Case aForm.PaneLevel
                                Case "6"
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
                    AssignLineNo4(aForm)
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
                oMatrix = aForm.Items.Item("57").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP1")
            Case "2"
                oMatrix = aForm.Items.Item("58").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP2")
            Case "3"
                oMatrix = aForm.Items.Item("60").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP3")
            Case "4"
                oMatrix = aForm.Items.Item("61").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP4")
            Case "5"
                oMatrix = aForm.Items.Item("76").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP6")

            Case "6"
                oMatrix = aForm.Items.Item("62").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP5")

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
                        oMatrix = aForm.Items.Item("57").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP1")
                        AssignLineNo(aForm)
                    Case "2"
                        oMatrix = aForm.Items.Item("58").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP2")
                        AssignLineNo1(aForm)
                    Case "3"
                        oMatrix = aForm.Items.Item("60").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP3")
                        AssignLineNo2(aForm)
                    Case "4"
                        oMatrix = aForm.Items.Item("61").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP4")
                        AssignLineNo3(aForm)
                    Case "5"
                        oMatrix = aForm.Items.Item("76").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP6")
                        AssignLineNo4(aForm)
                    Case "6"
                        oMatrix = aForm.Items.Item("62").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP5")
                        AssignLineNo4(aForm)

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
        If Me.MatrixId = "57" Then
            oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP1")
        ElseIf Me.MatrixId = "58" Then
            oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP2")
        ElseIf Me.MatrixId = "60" Then
            oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP3")
        ElseIf Me.MatrixId = "76" Then
            oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP6")
        Else
            oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP4")
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

    Private Sub LoadFiles(ByVal aform As SAPbouiCOM.Form)
        oMatrix = aform.Items.Item("62").Specific
        For intRow As Integer = 1 To oMatrix.RowCount
            If oMatrix.IsRowSelected(intRow) Then
                Dim strFilename As String
                strFilename = oMatrix.Columns.Item("V_0").Cells.Item(intRow).Specific.value
                Dim x As System.Diagnostics.ProcessStartInfo
                x = New System.Diagnostics.ProcessStartInfo
                x.UseShellExecute = True
                x.FileName = strFilename
                System.Diagnostics.Process.Start(x)
                x = Nothing
                Exit Sub
            End If
        Next
        oApplication.Utilities.Message("No file has been selected...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
    End Sub
#End Region
#End Region

#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oCombobox = aForm.Items.Item("1000008").Specific
            Dim strType As String = oCombobox.Selected.Value
            If strType = "I" Then
                If oApplication.Utilities.getEdittextvalue(aForm, "1000012") = "" Then
                    oApplication.Utilities.Message("Employee Id missing", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aForm.Items.Item("1000012").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    Return False
                End If
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "8") = "" Then
                oApplication.Utilities.Message("Trainner first Name is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Items.Item("8").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "10") = "" Then
                oApplication.Utilities.Message("Trainner Last Name is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Items.Item("10").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "1000002") = "" Then
                oApplication.Utilities.Message("Email Id is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Items.Item("1000002").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "32") = "" Then
                oApplication.Utilities.Message("Date of Birth is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                aForm.Items.Item("32").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If
            'If oApplication.Utilities.getEdittextvalue(aForm, "1000009") = "" Then
            '    oApplication.Utilities.Message("Main Skill set is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '    Return False
            'End If





            Dim oTemp1 As SAPbobsCOM.Recordset
            Dim stSQL1 As String
            oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            If aForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                stSQL1 = "Select * from ""@Z_HR_TRRAPP"" where  ""U_Z_EmailId""='" & oApplication.Utilities.getEdittextvalue(aForm, "1000002") & "'"
                oTemp1.DoQuery(stSQL1)
                If oTemp1.RecordCount > 0 Then
                    oApplication.Utilities.Message("EmailId already exists", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aForm.Items.Item("1000002").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    Return False
                End If
            End If

            If aForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                stSQL1 = "Select * from ""@Z_HR_TRRAPP"" where ""DocEntry""<>" & oApplication.Utilities.getEdittextvalue(aForm, "4") & " and  ""U_Z_EmailId""='" & oApplication.Utilities.getEdittextvalue(aForm, "1000002") & "'"
                oTemp1.DoQuery(stSQL1)
                If oTemp1.RecordCount > 0 Then
                    oApplication.Utilities.Message("EmailId already exists", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aForm.Items.Item("1000002").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    Return False
                End If
            End If

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
            If pVal.FormTypeEx = frm_HR_Trainner Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                'If pVal.ItemUID = "62" And pVal.ColUID = "V_0" And pVal.CharPressed <> 9 Then
                                '    oMatrix = oForm.Items.Item("62").Specific
                                'End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1" And (oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Or oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE) Then
                                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                                        If oApplication.SBO_Application.MessageBox("Do you want to confirm the information?", , "Yes", "No") = 2 Then
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                        AddMode(oForm)
                                    End If

                                    If Validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.GetForm(pVal.FormTypeEx, pVal.FormTypeCount)
                                'If pVal.ItemUID = "57" And pVal.Row > 0 Then
                                '    oMatrix = oForm.Items.Item("57").Specific
                                '    Me.RowtoDelete = pVal.Row
                                '    intSelectedMatrixrow = pVal.Row
                                '    Me.MatrixId = "57"
                                '    frmSourceMatrix = oMatrix
                                'End If
                                'If pVal.ItemUID = "58" And pVal.Row > 0 Then
                                '    oMatrix = oForm.Items.Item("58").Specific
                                '    Me.RowtoDelete = pVal.Row
                                '    intSelectedMatrixrow = pVal.Row
                                '    Me.MatrixId = "58"
                                '    frmSourceMatrix = oMatrix
                                'End If
                                'If pVal.ItemUID = "60" And pVal.Row > 0 Then
                                '    oMatrix = oForm.Items.Item("60").Specific
                                '    Me.RowtoDelete = pVal.Row
                                '    intSelectedMatrixrow = pVal.Row
                                '    Me.MatrixId = "60"
                                '    frmSourceMatrix = oMatrix
                                'End If
                                'If pVal.ItemUID = "61" And pVal.Row > 0 Then
                                '    oMatrix = oForm.Items.Item("61").Specific
                                '    Me.RowtoDelete = pVal.Row
                                '    intSelectedMatrixrow = pVal.Row
                                '    Me.MatrixId = "61"
                                '    frmSourceMatrix = oMatrix
                                'End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1000008" Then
                                    Dim stCode2, stCode3 As String
                                    oCombobox = oForm.Items.Item("1000008").Specific
                                    stCode3 = oCombobox.Selected.Value
                                    If stCode3 = "E" Then
                                        oForm.Items.Item("1000012").Enabled = False
                                        oApplication.Utilities.setEdittextvalue(oForm, "1000012", "")
                                        ClearAll()
                                    Else
                                        oForm.Items.Item("1000012").Enabled = True
                                    End If
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "1"
                                        If oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                                            AddMode(oForm)
                                        End If
                                    Case "1000005"
                                        oForm.PaneLevel = 1
                                    Case "26"
                                        oForm.PaneLevel = 2
                                    Case "1000006"
                                        oForm.PaneLevel = 3
                                    Case "28"
                                        oForm.PaneLevel = 4
                                    Case "29"
                                        oForm.PaneLevel = 6
                                    Case "30"
                                        oForm.PaneLevel = 7
                                    Case "75"
                                        oForm.PaneLevel = 5
                                    Case "67"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_ADD_ROW)
                                    Case "68"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_DELETE_ROW)
                                    Case "65"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            Exit Sub
                                        End If
                                        deleterow(oForm)
                                        RefereshDeleteRow(oForm)
                                    Case "64"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            Exit Sub
                                        End If
                                        LoadFiles(oForm)
                                    Case "63"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            Exit Sub

                                        End If
                                        fillopen()
                                        If strSelectedFilepath <> "" Then


                                            oMatrix = oForm.Items.Item("62").Specific
                                            AddRow(oForm)
                                            Try
                                                oForm.Freeze(True)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, strSelectedFilepath)
                                                Dim strDate As String
                                                Dim dtdate As Date
                                                dtdate = Now.Date
                                                strDate = Date.Today().ToString
                                                ''  strdate=
                                                Dim oColumn As SAPbouiCOM.Column
                                                oColumn = oMatrix.Columns.Item("V_1")
                                                ' oColumn.Editable = True
                                                oColumn.Editable = True
                                                oEditText = oMatrix.Columns.Item("V_1").Cells.Item(oMatrix.RowCount).Specific
                                                oEditText.String = "t"
                                                oApplication.SBO_Application.SendKeys("{TAB}")
                                                oForm.Items.Item("10").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                                oColumn.Editable = False
                                                'oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, dtdate)
                                                If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                End If
                                                oForm.Freeze(False)
                                            Catch ex As Exception
                                                oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                                oForm.Freeze(False)

                                            End Try
                                        End If
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
                                        If pVal.ItemUID = "73" Then
                                            val1 = oDataTable.GetValue("DocEntry", 0)
                                            oApplication.Utilities.setEdittextvalue(oForm, "73", val1)
                                            oForm.Freeze(False)
                                        End If
                                        If pVal.ItemUID = "76" And pVal.ColUID = "V_0" Then
                                            val = oDataTable.GetValue("U_Z_PosCode", 0)
                                            val1 = oDataTable.GetValue("U_Z_PosName", 0)
                                            oMatrix = oForm.Items.Item("76").Specific
                                            Try
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val1)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val)
                                                oForm.Freeze(False)
                                            Catch ex As Exception
                                                oForm.Freeze(False)
                                            End Try
                                        End If
                                        If pVal.ItemUID = "1000012" Then
                                            Try
                                                val = oDataTable.GetValue("empID", 0)
                                                PopulateEmployee(oForm, val)
                                                oApplication.Utilities.setEdittextvalue(oForm, "1000012", val)
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
    Private Sub ClearAll()
        oApplication.Utilities.setEdittextvalue(oForm, "8", "")
        oApplication.Utilities.setEdittextvalue(oForm, "80", "")
        oApplication.Utilities.setEdittextvalue(oForm, "78", "")
        oApplication.Utilities.setEdittextvalue(oForm, "10", "")
        oApplication.Utilities.setEdittextvalue(oForm, "1000002", "")
        oApplication.Utilities.setEdittextvalue(oForm, "1000004", "")
        oApplication.Utilities.setEdittextvalue(oForm, "32", "")
        oCombobox = oForm.Items.Item("36").Specific
        oCombobox.Select("", SAPbouiCOM.BoSearchKey.psk_ByValue)
        ' oApplication.Utilities.setEdittextvalue(oForm, "8", oRec.Fields.Item("citizenshp").Value)
        ' oApplication.Utilities.setEdittextvalue(oForm, "8", oRec.Fields.Item("sex").Value)
        oApplication.Utilities.setEdittextvalue(oForm, "41", "")
        oApplication.Utilities.setEdittextvalue(oForm, "45", "")
        oApplication.Utilities.setEdittextvalue(oForm, "49", "")
        oCombobox1 = oForm.Items.Item("53").Specific
        oCombobox1.Select("", SAPbouiCOM.BoSearchKey.psk_ByValue)
        'oApplication.Utilities.setEdittextvalue(oForm, "8", oRec.Fields.Item("homeCountr").Value)
        oApplication.Utilities.setEdittextvalue(oForm, "43", "")
        oApplication.Utilities.setEdittextvalue(oForm, "47", "")
        oApplication.Utilities.setEdittextvalue(oForm, "51", "")
        'oApplication.Utilities.setEdittextvalue(oForm, "8", oRec.Fields.Item("workCountr").Value)
        oCombobox1 = oForm.Items.Item("55").Specific
        oCombobox1.Select("", SAPbouiCOM.BoSearchKey.psk_ByValue)
    End Sub
    Private Sub PopulateEmployee(ByVal aform As SAPbouiCOM.Form, ByVal EmpId As String)
        Dim oRec As SAPbobsCOM.Recordset
        Dim strQuery As String
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strQuery = "Select ""empID"", ""firstName"",""middleName"",""U_Z_HR_ThirdName"",""lastName"",""email"",""mobile"",""birthDate"",""citizenshp"",""sex"",""homeStreet"","
        strQuery += """homeCity"",""homeZip"",""homeCountr"",""workStreet"",""workCity"",""workZip"",""workCountr"" from OHEM where ""empID""=" & EmpId
        oRec.DoQuery(strQuery)
        If oRec.RecordCount > 0 Then
            ' oApplication.Utilities.setEdittextvalue(aform, "1000012", oRec.Fields.Item("empID").Value)
            oApplication.Utilities.setEdittextvalue(aform, "8", oRec.Fields.Item("firstName").Value)
            oApplication.Utilities.setEdittextvalue(aform, "80", oRec.Fields.Item("middleName").Value)
            oApplication.Utilities.setEdittextvalue(aform, "78", oRec.Fields.Item("U_Z_HR_ThirdName").Value)
            oApplication.Utilities.setEdittextvalue(aform, "10", oRec.Fields.Item("lastName").Value)
            oApplication.Utilities.setEdittextvalue(aform, "1000002", oRec.Fields.Item("email").Value)
            oApplication.Utilities.setEdittextvalue(aform, "1000004", oRec.Fields.Item("mobile").Value)
            oApplication.Utilities.setEdittextvalue(aform, "32", oRec.Fields.Item("birthDate").Value)
            oCombobox = aform.Items.Item("36").Specific
            oCombobox.Select(oRec.Fields.Item("citizenshp").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
            ' oApplication.Utilities.setEdittextvalue(aform, "8", oRec.Fields.Item("citizenshp").Value)
            ' oApplication.Utilities.setEdittextvalue(aform, "8", oRec.Fields.Item("sex").Value)
            oApplication.Utilities.setEdittextvalue(aform, "41", oRec.Fields.Item("homeStreet").Value)
            oApplication.Utilities.setEdittextvalue(aform, "45", oRec.Fields.Item("homeCity").Value)
            oApplication.Utilities.setEdittextvalue(aform, "49", oRec.Fields.Item("homeZip").Value)
            oCombobox1 = aform.Items.Item("53").Specific
            oCombobox1.Select(oRec.Fields.Item("homeCountr").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
            'oApplication.Utilities.setEdittextvalue(aform, "8", oRec.Fields.Item("homeCountr").Value)
            oApplication.Utilities.setEdittextvalue(aform, "43", oRec.Fields.Item("workStreet").Value)
            oApplication.Utilities.setEdittextvalue(aform, "47", oRec.Fields.Item("workCity").Value)
            oApplication.Utilities.setEdittextvalue(aform, "51", oRec.Fields.Item("workZip").Value)
            'oApplication.Utilities.setEdittextvalue(aform, "8", oRec.Fields.Item("workCountr").Value)
            oCombobox1 = aform.Items.Item("55").Specific
            oCombobox1.Select(oRec.Fields.Item("workCountr").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)

        End If
    End Sub
    Private Sub EnableControls(ByVal aform As SAPbouiCOM.Form)
        Try
            aform.Freeze(True)
            Select Case aform.Mode
                Case SAPbouiCOM.BoFormMode.fm_ADD_MODE
                    aform.Items.Item("4").Enabled = False
                    aform.Items.Item("6").Enabled = False
                Case SAPbouiCOM.BoFormMode.fm_FIND_MODE
                    aform.Items.Item("4").Enabled = True
                    aform.Items.Item("6").Enabled = True
            End Select
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub

#Region "Menu Event"
    Public Overrides Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            Select Case pVal.MenuUID
                'Case "IntList"
                '    Dim oObj As New clshrCandidates
                '    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                '    oObj.LoadForm(oForm, oApplication.Utilities.getEdittextvalue(oForm, "4"), "Candidate")
                Case mnu_hr_Trainner
                    LoadForm()
                Case mnu_ADD
                    AddMode(oForm)
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

                    End If
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
                    If pVal.BeforeAction = False Then
                        oForm = oApplication.SBO_Application.Forms.ActiveForm()
                        'Dim stremp As String
                        'stremp = oApplication.Utilities.getEdittextvalue(oForm, "71")
                        'If stremp <> "" Then
                        '    '  oForm.Items.Item("70").Enabled = False
                        '    'Else
                        '    '    oForm.Items.Item("70").Enabled = True
                        'End If
                    End If
                Case mnu_FIND
                    If pVal.BeforeAction = False Then
                        oForm = oApplication.SBO_Application.Forms.ActiveForm()
                        EnableControls(oForm)
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
                        If oApplication.Utilities.ValidateCode(strValue, "TRAINER") = True Then
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

    Public Sub RightClickEvent(ByRef eventInfo As SAPbouiCOM.ContextMenuInfo, ByRef BubbleEvent As Boolean)
        oForm = oApplication.SBO_Application.Forms.Item(eventInfo.FormUID)
        'If eventInfo.FormUID = "RightClk" Then
        If oForm.TypeEx = frm_HR_Trainner Then
            If (eventInfo.BeforeAction = True) Then
                Dim oMenuItem As SAPbouiCOM.MenuItem
                Dim oMenus As SAPbouiCOM.Menus
                Try
                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                        'Dim oCreationPackage As SAPbouiCOM.MenuCreationParams
                        'oCreationPackage = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)
                        'oCreationPackage = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)
                        'oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING
                        'oCreationPackage.UniqueID = "TraDetails"
                        'oCreationPackage.String = "Trainning Details"
                        'oCreationPackage.Enabled = True
                        'oMenuItem = oApplication.SBO_Application.Menus.Item("1280") 'Data'
                        'oMenus = oMenuItem.SubMenus
                        'oMenus.AddEx(oCreationPackage)
                    End If
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try
            Else
                Dim oMenuItem As SAPbouiCOM.MenuItem
                Dim oMenus As SAPbouiCOM.Menus
                Try
                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                        '  oApplication.SBO_Application.Menus.RemoveEx("TraDetails")
                    End If
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try
            End If
        End If
    End Sub
#End Region

    Public Sub FormDataEvent(ByRef BusinessObjectInfo As SAPbouiCOM.BusinessObjectInfo, ByRef BubbleEvent As Boolean)
        Try
            If BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
                oForm.Items.Item("8").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                oForm.Items.Item("4").Enabled = False
                oForm.Items.Item("6").Enabled = False
                ' oForm.Items.Item("70").Enabled = False
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
End Class
