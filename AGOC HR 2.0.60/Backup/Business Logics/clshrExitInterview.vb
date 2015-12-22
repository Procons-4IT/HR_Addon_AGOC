Imports System.IO
Public Class clshrExitInterview
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox, oCombobox1, oCombobox2 As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oCheckColumn As SAPbouiCOM.CheckBoxColumn
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private ocombo As SAPbouiCOM.ComboBoxColumn
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Dim oDataSrc_Line, oDataSrc_Line3 As SAPbouiCOM.DBDataSource
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private sPath, strSelectedFilepath, strSelectedFolderPath As String
    Private MatrixId As String
    Private InvForConsumedItems, count As Integer
    Private RowtoDelete As Integer
    Private blnFlag As Boolean = False
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Public Sub LoadForm(ByVal oForm As SAPbouiCOM.Form)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_ExitInvForm1) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_ExitInvForm, frm_hr_ExitInvForm1)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        FillDepartment(oForm)
        FillPosition(oForm)
        FillResignReason(oForm)
        AddChooseFromList(oForm)
        FillSubSidiary(oForm)
        FillTerReason(oForm)
        oForm.EnableMenu(mnu_ADD, False)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM3")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oForm.DataSources.UserDataSources.Add("Reqno", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "6", "Reqno")
        oEditText = oForm.Items.Item("6").Specific
        oEditText.ChooseFromListUID = "CFL1"
        oEditText.ChooseFromListAlias = "DocEntry"

        oEditText = oForm.Items.Item("43").Specific
        oEditText.ChooseFromListUID = "CFL2"
        oEditText.ChooseFromListAlias = "empID"

        oForm.DataSources.UserDataSources.Add("dt", SAPbouiCOM.BoDataType.dt_DATE)
        oApplication.Utilities.setUserDatabind(oForm, "11", "dt")
        oApplication.Utilities.setEdittextvalue(oForm, "11", Now.Date)
        'oForm.DataSources.UserDataSources.Add("Invno", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        'oApplication.Utilities.setUserDatabind(oForm, "43", "Invno")
        'oEditText = oForm.Items.Item("43").Specific
        'oEditText.ChooseFromListUID = "CFL2"
        'oEditText.ChooseFromListAlias = "empID"
        oForm.PaneLevel = 1
        'Dim osta As SAPbouiCOM.StaticText
        'osta = oForm.Items.Item("19").Specific
        'osta.Caption = "Step " & oForm.PaneLevel & " of 3"
        'oForm.Items.Item("19").TextStyle = SAPbouiCOM.BoTextStyle.ts_BOLD
        oMatrix = oForm.Items.Item("31").Specific
        oMatrix.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
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
            oCFLCreationParams.ObjectType = "Z_HR_OEXFOM"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "171"
            oCFLCreationParams.UniqueID = "CFL2"
            oCFL = oCFLs.Add(oCFLCreationParams)

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub FillResignReason(ByVal sform As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = sform.Items.Item("41").Specific
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oTempRec.DoQuery("select reasonID,name  from OHTR")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item("reasonID").Value, oTempRec.Fields.Item("name").Value)
            oTempRec.MoveNext()
        Next
        sform.Items.Item("41").DisplayDesc = True

    End Sub
    'Private Sub FillDepartment(ByVal sform As SAPbouiCOM.Form)
    '    Dim oTempRec As SAPbobsCOM.Recordset
    '    oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
    '    oCombobox = sform.Items.Item("21").Specific
    '    For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
    '        oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
    '    Next
    '    oCombobox.ValidValues.Add("", "")
    '    oTempRec.DoQuery("Select Code,Remarks from OUDP")
    '    For intRow As Integer = 0 To oTempRec.RecordCount - 1
    '        oCombobox.ValidValues.Add(oTempRec.Fields.Item("Code").Value, oTempRec.Fields.Item("Remarks").Value)
    '        oTempRec.MoveNext()
    '    Next
    '    sform.Items.Item("21").DisplayDesc = True

    'End Sub

    Private Sub FillDepartment(ByVal sform As SAPbouiCOM.Form)
        Dim oMatrix As SAPbouiCOM.Matrix
        oMatrix = sform.Items.Item("61").Specific
        Dim oColum As SAPbouiCOM.Column
        oColum = oMatrix.Columns.Item("V_3")
        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = sform.Items.Item("21").Specific
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
        sform.Items.Item("21").DisplayDesc = True
        oColum.DisplayDesc = True
    End Sub
    Private Sub FillSubSidiary(ByVal sform As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = sform.Items.Item("65").Specific
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oTempRec.DoQuery("Select ""Code"",""Name"" From OUBR")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item(0).Value, oTempRec.Fields.Item(1).Value)
            oTempRec.MoveNext()
        Next
        sform.Items.Item("65").DisplayDesc = True

    End Sub
    Private Sub FillTerReason(ByVal sform As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = sform.Items.Item("69").Specific
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oTempRec.DoQuery("Select ""reasonID"",""name"" From OHTR")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item(0).Value, oTempRec.Fields.Item(1).Value)
            oTempRec.MoveNext()
        Next
        sform.Items.Item("69").DisplayDesc = True

    End Sub
    Private Sub FillPosition(ByVal sform As SAPbouiCOM.Form)
        oCombobox = sform.Items.Item("25").Specific
        oCombobox1 = sform.Items.Item("45").Specific
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
            oCombobox1.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTempRec.DoQuery("Select posID,descriptio From OHPS")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item("posID").Value, oTempRec.Fields.Item("descriptio").Value)
            oCombobox1.ValidValues.Add(oTempRec.Fields.Item("posID").Value, oTempRec.Fields.Item("descriptio").Value)
            oTempRec.MoveNext()
        Next
        sform.Items.Item("25").DisplayDesc = True
        sform.Items.Item("45").DisplayDesc = True
    End Sub
    Private Sub reDrawForm(ByVal oForm As SAPbouiCOM.Form)
        Try
            oForm.Freeze(True)
            oForm.Items.Item("35").Width = oForm.Width - 30
            oForm.Items.Item("35").Height = oForm.Height - 156
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
        End Try
    End Sub

    Private Sub PopulateEmpdetails(ByVal aform As SAPbouiCOM.Form, ByVal ExitNo As String)
        Dim strqry As String
        Dim oTemp As SAPbobsCOM.Recordset
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strqry = "Select * from [@Z_HR_OEXFOM] where U_Z_empID='" & ExitNo & "'"
        oTemp.DoQuery(strqry)
        If oTemp.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(aform, "53", oTemp.Fields.Item("DocEntry").Value)
            oApplication.Utilities.setEdittextvalue(aform, "11", oTemp.Fields.Item("CreateDate").Value)
            oApplication.Utilities.setEdittextvalue(aform, "13", oTemp.Fields.Item("U_Z_empID").Value)
            oApplication.Utilities.setEdittextvalue(aform, "15", oTemp.Fields.Item("U_Z_FirstName").Value)
            oApplication.Utilities.setEdittextvalue(aform, "17", oTemp.Fields.Item("U_Z_MiddleName").Value)
            oApplication.Utilities.setEdittextvalue(aform, "19", oTemp.Fields.Item("U_Z_LastName").Value)

            oCombobox = aform.Items.Item("21").Specific
            oCombobox.Select(oTemp.Fields.Item("U_Z_DeptCode").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
            oCombobox = aform.Items.Item("25").Specific
            oCombobox.Select(oTemp.Fields.Item("U_Z_PosCode").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
            oCombobox = aform.Items.Item("27").Specific
            oCombobox.Select(oTemp.Fields.Item("U_Z_Status").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
        End If
    End Sub
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim strDept, Reqno, posname As String
            Reqno = oApplication.Utilities.getEdittextvalue(aForm, "6")
            If Reqno = "" Then
                oApplication.Utilities.Message("Employee Number is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
    Private Sub UpdateAttachment(ByVal aForm As SAPbouiCOM.Form)
        Try
            oMatrix = aForm.Items.Item("48").Specific
            For i As Integer = 1 To oMatrix.RowCount
                Dim oRec As SAPbobsCOM.Recordset
                oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                Dim strQry = "Select AttachPath From OADP"
                oRec.DoQuery(strQry)
                Dim SPath As String = oApplication.Utilities.getMatrixValues(oMatrix, "V_3", i) ' oOfferGrid.DataTable.GetValue("U_Z_Attachment", i).ToString()
                If SPath = "" Then
                Else
                    Dim DPath As String = ""
                    If Not oRec.EoF Then
                        DPath = oRec.Fields.Item("AttachPath").Value.ToString()
                    End If
                    If Not Directory.Exists(DPath) Then
                        Directory.CreateDirectory(DPath)
                    End If
                    Dim file = New FileInfo(SPath)
                    Dim Filename As String = Path.GetFileName(SPath)
                    Dim SavePath As String = Path.Combine(DPath, Filename)
                    If System.IO.File.Exists(SavePath) Then
                    Else
                        file.CopyTo(Path.Combine(DPath, file.Name), True)
                    End If
                End If
            Next

           
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
    Private Sub LoadFiles(ByVal aform As SAPbouiCOM.Form)
        oMatrix = aform.Items.Item("31").Specific
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
    Private Sub AssignLineNo(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("31").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM3")
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
#Region "Add Row/ Delete Row"
    Private Sub AddRow(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)

            Select Case aForm.PaneLevel
                Case "4"
                    oMatrix = aForm.Items.Item("31").Specific
                    oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM3")
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
                    AssignLineNo(aForm)
            End Select


            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)

        End Try
    End Sub
    Private Sub deleterow(ByVal aForm As SAPbouiCOM.Form)
        Select Case aForm.PaneLevel
            Case "4"
                oMatrix = aForm.Items.Item("31").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM3")
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
                    Case "4"
                        oMatrix = aForm.Items.Item("31").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM3")
                        AssignLineNo(aForm)
                End Select
                oMatrix.LoadFromDataSource()
                If aForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                    aForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                End If
                Exit Sub
            End If
        Next

    End Sub
    Private Sub RefereshDeleteRow(ByVal aForm As SAPbouiCOM.Form)
        If Me.MatrixId = "31" Then
            oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_EXFORM3")
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

    Private Sub BinJoindate(ByVal exitno As String)
        Dim oRec As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRec.DoQuery("Select startDate from OHEM where empID='" & exitno & "'")
        If oRec.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(oForm, "1000002", oRec.Fields.Item(0).Value)
        End If
    End Sub
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_ExitInvForm1 Then
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
                                If pVal.ItemUID = "_2" Then
                                    oForm.Close()
                                End If
                                If pVal.ItemUID = "57" Then
                                    Dim strCode As String
                                    strCode = oApplication.Utilities.getEdittextvalue(oForm, "53")
                                    Dim ooBj As New clshrExitfrmInitialization
                                    ooBj.LoadForm1(strCode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                reDrawForm(oForm)
                            Case SAPbouiCOM.BoEventTypes.et_DOUBLE_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "48" And pVal.ColUID = "V_3" Then
                                    oMatrix = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strPath As String = oApplication.Utilities.getMatrixValues(oMatrix, pVal.ColUID, pVal.Row) 'oOfferGrid.DataTable.Columns.Item("U_Z_Attachment").Cells.Item(pVal.Row).Value.ToString()
                                    fillopen()
                                    If strSelectedFolderPath = "" Then
                                        oApplication.Utilities.Message("Please Select a File", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                        BubbleEvent = False
                                    Else
                                        oApplication.Utilities.SetMatrixValues(oMatrix, pVal.ColUID, pVal.Row, strSelectedFolderPath)
                                        ' oOfferGrid.DataTable.Columns.Item("U_Z_Attachment").Cells.Item(pVal.Row).Value = strPath
                                    End If
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "28" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 3
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "29" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 4
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "59" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 5
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "60" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 6
                                    oForm.Freeze(False)
                                End If
                                Select Case pVal.ItemUID
                                    Case "3"
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = oForm.PaneLevel + 1
                                        If oForm.PaneLevel = 3 Then
                                            Dim ExitNo As String
                                            ExitNo = oApplication.Utilities.getEdittextvalue(oForm, "6")
                                            ' PopulateEmpdetails(oForm, ExitNo)
                                            oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
                                            oForm.Items.Item("53").Enabled = True
                                            oApplication.Utilities.setEdittextvalue(oForm, "53", ExitNo)
                                            oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                            BinJoindate(oApplication.Utilities.getEdittextvalue(oForm, "13"))
                                            If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                                oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
                                            End If

                                            Dim strstatus As String
                                            oCombobox = oForm.Items.Item("27").Specific
                                            strstatus = oCombobox.Selected.Value
                                            If strstatus = "E" Then
                                                oForm.Items.Item("1").Visible = False
                                                ' oForm.Items.Item("1000002").Enabled = False
                                                oForm.Items.Item("37").Enabled = False
                                                oForm.Items.Item("39").Enabled = False
                                                oForm.Items.Item("41").Enabled = False
                                                oForm.Items.Item("43").Enabled = False
                                                oForm.Items.Item("45").Enabled = False
                                                oForm.Items.Item("47").Enabled = False
                                                oForm.Items.Item("27").Enabled = False
                                                oForm.Items.Item("48").Enabled = False
                                            Else
                                                oForm.Items.Item("1").Visible = True
                                                ' oForm.Items.Item("1000002").Enabled = True
                                                oForm.Items.Item("37").Enabled = True
                                                oForm.Items.Item("39").Enabled = True
                                                oForm.Items.Item("41").Enabled = True
                                                oForm.Items.Item("43").Enabled = True
                                                oForm.Items.Item("45").Enabled = False
                                                oForm.Items.Item("47").Enabled = True
                                                oForm.Items.Item("27").Enabled = True
                                                oForm.Items.Item("48").Enabled = True
                                            End If
                                            oForm.Items.Item("53").Enabled = False
                                            oForm.PaneLevel = 3
                                        End If
                                        oForm.Freeze(False)
                                    Case "4"
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = oForm.PaneLevel - 1
                                        oForm.Freeze(False)
                                    Case "1"
                                        If pVal.ItemUID = "1" Then
                                            UpdateAttachment(oForm)
                                            Dim oTemp As SAPbobsCOM.Recordset
                                            oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                            oCombobox = oForm.Items.Item("27").Specific
                                            Dim Status As String = oCombobox.Selected.Value
                                            If Status = "E" Then
                                                Dim strquery As String = " Update [@Z_HR_EXFORM2] Set U_Z_ApprovedBy = '" & oApplication.Company.UserName.ToString & "',U_Z_Appdt = GetDate() Where DocEntry = '" & oApplication.Utilities.getEdittextvalue(oForm, "53") & "'"
                                                oTemp.DoQuery(strquery)
                                            End If
                                            'If oApplication.SBO_Application.MessageBox("Do you want confirm Employee Exit Interview form ?", , "Yes", "No") = 2 Then
                                            '    Exit Sub
                                            '    'Else
                                            '    '    If CommitSelections(oForm) = True Then
                                            '    '        oApplication.Utilities.Message("Operation Completed Successfully...", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                            '    '        oForm.Close()
                                            '    '    Else
                                            '    '        BubbleEvent = False
                                            '    '        Exit Sub
                                            '    '    End If
                                            'End If
                                        End If
                                    Case "34"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            Exit Sub
                                        End If
                                        deleterow(oForm)
                                        RefereshDeleteRow(oForm)
                                    Case "33"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            Exit Sub
                                        End If
                                        LoadFiles(oForm)
                                    Case "32"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            Exit Sub

                                        End If
                                        fillopen()
                                        If strSelectedFilepath <> "" Then


                                            oMatrix = oForm.Items.Item("31").Specific
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

                                        If pVal.ItemUID = "6" Then
                                            val = oDataTable.GetValue("DocEntry", 0)
                                            oApplication.Utilities.setEdittextvalue(oForm, "6", val)
                                        End If
                                        If pVal.ItemUID = "43" Then
                                            val1 = oDataTable.GetValue("firstName", 0)
                                            val2 = oDataTable.GetValue("position", 0)
                                            val = oDataTable.GetValue("empID", 0)
                                            oCombobox = oForm.Items.Item("45").Specific
                                            oCombobox.Select(val2, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                            oApplication.Utilities.setEdittextvalue(oForm, "49", val1)
                                            oApplication.Utilities.setEdittextvalue(oForm, "43", val)
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
                Case mnu_hr_ExitInvForm
                    LoadForm(oForm)
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
