Public Class clshrCourse
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oCheckbox As SAPbouiCOM.CheckBox
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
    Private sPath, strSelectedFilepath, strSelectedFolderPath As String
    Private blnFlag As Boolean = False
    Dim oDataSrc_Line, oDataSrc_Line3 As SAPbouiCOM.DBDataSource
    Dim oDataSrc_Line1, oDataSrc_Line2 As SAPbouiCOM.DBDataSource

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_Course) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_Course, frm_hr_Course)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.DataBrowser.BrowseBy = "4"
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.EnableMenu("1283", True)
        AddChooseFromList(oForm)
        databind(oForm)
        FillLevels(oForm)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line1 = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR2")
        For count = 1 To oDataSrc_Line1.Size - 1
            oDataSrc_Line1.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line2 = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR3")
        For count = 1 To oDataSrc_Line2.Size - 1
            oDataSrc_Line2.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line3 = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR4")
        For count = 1 To oDataSrc_Line3.Size - 1
            oDataSrc_Line3.SetValue("LineId", count - 1, count)
        Next
        oForm.PaneLevel = 4
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        reDrawForm(oForm)
        oForm.Items.Item("4").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oForm.Freeze(False)
    End Sub

    Public Sub LoadForm1(ByVal CourseCode As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_Course) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_Course, frm_hr_Course)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        ' oForm.DataBrowser.BrowseBy = "4"
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.EnableMenu("1283", True)
        AddChooseFromList(oForm)
        databind(oForm)
        FillLevels(oForm)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line1 = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR2")
        For count = 1 To oDataSrc_Line1.Size - 1
            oDataSrc_Line1.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line2 = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR3")
        For count = 1 To oDataSrc_Line2.Size - 1
            oDataSrc_Line2.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line3 = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR4")
        For count = 1 To oDataSrc_Line3.Size - 1
            oDataSrc_Line3.SetValue("LineId", count - 1, count)
        Next
        oForm.PaneLevel = 4
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oForm.Items.Item("4").Enabled = True
        oApplication.Utilities.setEdittextvalue(oForm, "4", CourseCode)
        oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE Then
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        End If
        reDrawForm(oForm)
        oForm.Items.Item("4").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oForm.Freeze(False)
    End Sub
    Private Sub FillLevels(ByVal aForm As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        Dim oDBDataSrc As SAPbouiCOM.DBDataSource
        Dim oMatrix As SAPbouiCOM.Matrix
        Dim oColum As SAPbouiCOM.Column
        oMatrix = aForm.Items.Item("12").Specific
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
        oColum.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        oMatrix.AutoResizeColumns()

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
    Private Sub LoadFiles(ByVal aform As SAPbouiCOM.Form)
        oMatrix = aform.Items.Item("24").Specific
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

#Region "Add Choose From List"
    Private Sub databind(ByVal aForm As SAPbouiCOM.Form)
        oMatrix = aForm.Items.Item("10").Specific
        oColumn = oMatrix.Columns.Item("V_0")
        oColumn.ChooseFromListUID = "CFL1"
        oColumn.ChooseFromListAlias = "U_Z_BussCode"

        oMatrix = aForm.Items.Item("11").Specific
        oColumn = oMatrix.Columns.Item("V_0")
        oColumn.ChooseFromListUID = "CFL2"
        oColumn.ChooseFromListAlias = "U_Z_PeoobjCode"

        oMatrix = aForm.Items.Item("12").Specific
        oColumn = oMatrix.Columns.Item("V_0")
        oColumn.ChooseFromListUID = "CFL3"
        oColumn.ChooseFromListAlias = "U_Z_CompCode"

        oMatrix = aForm.Items.Item("17").Specific
        oColumn = oMatrix.Columns.Item("V_0")
        oColumn.ChooseFromListUID = "CFL4"
        oColumn.ChooseFromListAlias = "U_Z_PosCode"

        oEditText = aForm.Items.Item("20").Specific
        oEditText.ChooseFromListUID = "CFL5"
        oEditText.ChooseFromListAlias = "U_Z_CouCatCode"


        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oMatrix = aForm.Items.Item("11").Specific
        Dim oColum As SAPbouiCOM.Column
        oColum = oMatrix.Columns.Item("V_2")
        For intRow As Integer = oColum.ValidValues.Count - 1 To 0 Step -1
            oColum.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oColum.ValidValues.Add("", "")
        oTempRec.DoQuery("Select U_Z_CatCode,U_Z_CatName from [@Z_HR_PECAT] order by Code")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oColum.ValidValues.Add(oTempRec.Fields.Item("U_Z_CatCode").Value, oTempRec.Fields.Item("U_Z_CatName").Value)
            oTempRec.MoveNext()
        Next
        oColum.DisplayDesc = True
        oMatrix.LoadFromDataSource()

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
            oCFLCreationParams.ObjectType = "Z_HR_OBUOB"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)

          
            oCFLCreationParams.ObjectType = "Z_HR_OPEOB"
            oCFLCreationParams.UniqueID = "CFL2"
            oCFL = oCFLs.Add(oCFLCreationParams)
            

            oCFLCreationParams.ObjectType = "Z_HR_OCOMP"
            oCFLCreationParams.UniqueID = "CFL3"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams.ObjectType = "Z_HR_OPOSIN"
            oCFLCreationParams.UniqueID = "CFL4"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams.ObjectType = "Z_HR_OCOCA"
            oCFLCreationParams.UniqueID = "CFL5"
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
            oMatrix = aForm.Items.Item("10").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR1")
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
            oMatrix = aForm.Items.Item("11").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR2")
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
            oMatrix = aForm.Items.Item("11").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR3")
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
            oMatrix = aForm.Items.Item("17").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR4")
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
            oMatrix = aForm.Items.Item("24").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR5")
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
                    oMatrix = aForm.Items.Item("10").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR1")
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
                    oMatrix = aForm.Items.Item("11").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR2")

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
                    oMatrix = aForm.Items.Item("12").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR3")
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
                    oMatrix = aForm.Items.Item("17").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR4")
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
                    oMatrix = aForm.Items.Item("24").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR5")
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
                oMatrix = aForm.Items.Item("10").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_COUR1")
            Case "2"
                oMatrix = aForm.Items.Item("11").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_COUR2")
            Case "3"
                oMatrix = aForm.Items.Item("12").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_COUR3")
            Case "4"
                oMatrix = aForm.Items.Item("17").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_COUR4")
            Case "5"
                oMatrix = aForm.Items.Item("24").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_COUR5")
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
                        oMatrix = aForm.Items.Item("10").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_COUR1")
                        AssignLineNo(aForm)
                    Case "2"
                        oMatrix = aForm.Items.Item("11").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_COUR2")
                        AssignLineNo1(aForm)
                    Case "3"
                        oMatrix = aForm.Items.Item("12").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_COUR3")
                        AssignLineNo2(aForm)
                    Case "4"
                        oMatrix = aForm.Items.Item("17").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_COUR4")
                        AssignLineNo3(aForm)
                    Case "5"
                        oMatrix = aForm.Items.Item("24").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_COUR5")
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
        If Me.MatrixId = "10" Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR1")
        ElseIf Me.MatrixId = "11" Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR2")
        ElseIf Me.MatrixId = "12" Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR3")
        ElseIf Me.MatrixId = "24" Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR5")
        Else
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_COUR4")
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

        If aForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
            aForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
        End If
    End Sub
#End Region
#End Region
#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            If oApplication.Utilities.getEdittextvalue(aForm, "4") = "" Then
                oApplication.Utilities.Message("Enter Course Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "6") = "" Then
                oApplication.Utilities.Message("Enter Course Name...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "20") = "" Then
                oApplication.Utilities.Message("Enter Course Category...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            Dim oTemp1 As SAPbobsCOM.Recordset
            Dim stSQL1 As String
            oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            If aForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                stSQL1 = "Select * from [@Z_HR_OCOUR] where U_Z_CourseCode='" & oApplication.Utilities.getEdittextvalue(aForm, "4") & "'"
                oTemp1.DoQuery(stSQL1)
                If oTemp1.RecordCount > 0 Then
                    oApplication.Utilities.Message("Course Code Already Exists...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            End If
            oMatrix = oForm.Items.Item("10").Specific
            Dim strcode2, strcode1 As String
            If oMatrix.RowCount > 1 Then
                strcode2 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount)
                strcode1 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount - 1)
                If strcode2.ToUpper = strcode1.ToUpper Then
                    oApplication.Utilities.Message("This Enter Already Exists...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    Return False
                End If
            End If
            oMatrix = oForm.Items.Item("11").Specific
            Dim strcode3, strcode4 As String
            If oMatrix.RowCount > 1 Then
                strcode3 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount)
                strcode4 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount - 1)
                If strcode3.ToUpper = strcode4.ToUpper Then
                    oApplication.Utilities.Message("This Entry Already Exists...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    Return False
                End If
            End If
            oMatrix = oForm.Items.Item("12").Specific
            Dim strcode5, strcode6 As String
            If oMatrix.RowCount > 1 Then
                strcode5 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount)
                strcode6 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount - 1)
                If strcode5.ToUpper = strcode6.ToUpper Then
                    oApplication.Utilities.Message("This Entry Already Exists...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    Return False
                End If
            End If
            oMatrix = oForm.Items.Item("17").Specific
            Dim strcode7, strcode8 As String
            If oMatrix.RowCount > 1 Then
                strcode7 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount)
                strcode8 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount - 1)
                If strcode7.ToUpper = strcode8.ToUpper Then
                    oApplication.Utilities.Message("This Entry Already Exists...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    Return False
                End If
            End If
            oMatrix = oForm.Items.Item("17").Specific
            oCheckbox = oForm.Items.Item("18").Specific
            If oCheckbox.Checked = False Then
                If oMatrix.RowCount = 0 Then
                    oApplication.Utilities.Message("Enter Line Details...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            ElseIf oCheckbox.Checked = True And oMatrix.RowCount > 0 Then
                If oApplication.Utilities.getMatrixValues(oMatrix, "V_0", 1) <> "" Then
                    oApplication.Utilities.Message("Either you can select All Position or Postion Details...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
               
            ElseIf oCheckbox.Checked = True Then
                If oMatrix.RowCount > 0 Then


                    If oApplication.Utilities.getMatrixValues(oMatrix, "V_0", 1) <> "" Then

                        oApplication.Utilities.Message("Either you can select All Position or Postion Details...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
                End If

                AssignLineNo(oForm)
                AssignLineNo1(oForm)
                AssignLineNo2(oForm)
                AssignLineNo3(oForm)
                AssignLineNo4(oForm)
                Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
#End Region

    Private Sub reDrawForm(ByVal oForm As SAPbouiCOM.Form)
        Try
            oForm.Freeze(True)
            oForm.Items.Item("13").Width = oForm.Width - 30
            oForm.Items.Item("13").Height = oForm.Height - 200
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
        End Try
    End Sub


#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_Course Then
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
                                If pVal.ItemUID = "30" Then
                                    oApplication.Utilities.OpenMasterinLink(oForm, "CourseType")
                                    BubbleEvent = False
                                    Exit Sub
                                ElseIf pVal.ItemUID = "31" Then
                                    oApplication.Utilities.OpenMasterinLink(oForm, "CourseCategory")
                                    BubbleEvent = False
                                    Exit Sub
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
                                If pVal.ItemUID = "12" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("12").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "12"
                                    frmSourceMatrix = oMatrix
                                End If
                                If pVal.ItemUID = "17" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("17").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "17"
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
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                reDrawForm(oForm)
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "7"
                                        oForm.PaneLevel = 1
                                    Case "8"
                                        oForm.PaneLevel = 2
                                    Case "9"
                                        oForm.PaneLevel = 3
                                    Case "16"
                                        oForm.PaneLevel = 4
                                    Case "23"
                                        oForm.PaneLevel = 5
                                    Case "14"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_ADD_ROW)
                                    Case "15"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_DELETE_ROW)
                                    Case "27"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            Exit Sub
                                        End If
                                        '   deleterow(oForm)
                                        RefereshDeleteRow(oForm)
                                    Case "26"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            Exit Sub
                                        End If
                                        LoadFiles(oForm)
                                    Case "25"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            Exit Sub

                                        End If
                                        fillopen()
                                        If strSelectedFilepath <> "" Then


                                            oMatrix = oForm.Items.Item("24").Specific
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
                                                oColumn.Editable = True
                                                oMatrix.Columns.Item("V_1").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                                oEditText = oMatrix.Columns.Item("V_1").Cells.Item(oMatrix.RowCount).Specific
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, dtdate)
                                                ' oEditText.Value = Now.Date
                                                ' oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "t")
                                                oApplication.SBO_Application.SendKeys("{TAB}")
                                                oForm.Items.Item("4").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
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
                                Dim sCHFL_ID, val, val2 As String
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
                                        If pVal.ItemUID = "10" And pVal.ColUID = "V_0" Then
                                            Try
                                                val = oDataTable.GetValue("U_Z_BussCode", 0)
                                                val1 = oDataTable.GetValue("U_Z_BussName", 0)
                                                oMatrix = oForm.Items.Item("10").Specific

                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val1)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "11" And pVal.ColUID = "V_0" Then
                                            Try
                                                val = oDataTable.GetValue("U_Z_PeoobjCode", 0)
                                                val1 = oDataTable.GetValue("U_Z_PeoobjName", 0)
                                                val2 = oDataTable.GetValue("U_Z_PeoCategory", 0)
                                                oMatrix = oForm.Items.Item("11").Specific
                                                oCombobox = oMatrix.Columns.Item("V_2").Cells.Item(pVal.Row).Specific
                                                oCombobox.Select(val2, SAPbouiCOM.BoSearchKey.psk_ByValue)

                                                ' oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", pVal.Row, val2)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val1)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "12" And pVal.ColUID = "V_0" Then

                                            Try
                                                val = oDataTable.GetValue("U_Z_CompCode", 0)
                                                val1 = oDataTable.GetValue("U_Z_CompName", 0)
                                                val2 = oDataTable.GetValue("U_Z_CompLevel", 0)
                                                oMatrix = oForm.Items.Item("12").Specific
                                                oCombobox = oMatrix.Columns.Item("V_2").Cells.Item(pVal.Row).Specific
                                                '  oCombobox.Select(val2, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val1)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "17" And pVal.ColUID = "V_0" Then
                                            Try
                                                val = oDataTable.GetValue("U_Z_PosCode", 0)
                                                val1 = oDataTable.GetValue("U_Z_PosName", 0)
                                                oMatrix = oForm.Items.Item("17").Specific

                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val1)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "20" Then

                                            Try
                                                val = oDataTable.GetValue("U_Z_CouCatCode", 0)

                                                val1 = oDataTable.GetValue("U_Z_CouCatDesc", 0)

                                                oApplication.Utilities.setEdittextvalue(oForm, "29", val1)
                                                oApplication.Utilities.setEdittextvalue(oForm, "20", val)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        oForm.Freeze(False)
                                    End If
                                Catch ex As Exception
                                    oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
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
                Case mnu_hr_Course
                    LoadForm()
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        oForm.Items.Item("4").Enabled = False
                        oForm.Items.Item("6").Enabled = False
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
                        oForm.Items.Item("4").Enabled = True
                        oForm.Items.Item("6").Enabled = True
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
                        If oApplication.Utilities.ValidateCode(strValue, "COURSE") = True Then
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
            If BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

End Class
