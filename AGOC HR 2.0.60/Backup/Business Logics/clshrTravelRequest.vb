Imports System.Xml
Public Class clshrTravelRequest
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix, objMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oGrid As SAPbouiCOM.Grid
    Private oCheckbox As SAPbouiCOM.CheckBox
    Private oColumn As SAPbouiCOM.Column
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private MatrixId As String
    Private RowtoDelete As Integer
    Private sPath, strSelectedFilepath, strSelectedFolderPath As String
    Private InvForConsumedItems, count As Integer
    Private blnFlag As Boolean = False
    Dim oDataSrc_Line As SAPbouiCOM.DBDataSource

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Public Sub LoadForm(ByVal oForm As SAPbouiCOM.Form, ByVal strDocType As String, ByVal Empid As String)
        oForm = oApplication.Utilities.LoadForm(xml_hr_TraRequest, frm_hr_TraRequest)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_TRAREQ1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        FillDepartment(oForm)
        PopulateEmployee(oForm, Empid)
        FillTravelCode(oForm, Empid)
        ' LocalCurrency(oForm)
        AddMode(oForm)
        oForm.Items.Item("51").Visible = False
        oForm.Items.Item("52").Visible = False
        oForm.Items.Item("53").Visible = False
        oForm.Items.Item("54").Visible = False
        oForm.Items.Item("55").Visible = False
        oForm.Items.Item("56").Visible = False
        oForm.PaneLevel = 1
        reDrawForm(oForm)
        oForm.Freeze(False)
    End Sub
    Public Sub LoadForm1(ByVal oForm As SAPbouiCOM.Form, ByVal strdoc As String, ByVal strtitle As String, ByVal strstatus As String, ByVal Empid As String, Optional ByVal strNeqReq As String = "")
        oForm = oApplication.Utilities.LoadForm(xml_hr_TraRequest, frm_hr_TraRequest)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)

        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_TRAREQ1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        FillDepartment(oForm)
        'PopulateEmployee(oForm, Empid)
        ' LocalCurrency(oForm)
        FillTravelCode(oForm, Empid)
        oCombobox = oForm.Items.Item("33").Specific
        oMatrix = oForm.Items.Item("45").Specific
        oMatrix.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oForm.PaneLevel = 1
        'oApplication.Utilities.EnableDisable(oForm, strtitle, strNeqReq, strstatus)
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oForm.Items.Item("4").Enabled = True
        oApplication.Utilities.setEdittextvalue(oForm, "4", strdoc)
        oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        If strtitle = "Employee Expenses Approval" Or strtitle = "Employee Expenses Claim Request" Then
            oForm.Items.Item("18").Visible = True
        Else
            oForm.Items.Item("18").Visible = False
        End If
        oForm.Items.Item("4").Enabled = False
        If strtitle = "Employee Expenses Claim Request" Then
            oCombobox.Select("CR", SAPbouiCOM.BoSearchKey.psk_ByValue)
            objMatrix = oForm.Items.Item("38").Specific
            objMatrix.Columns.Item("V_4").Editable = False
        End If
        If strtitle = "Employee Expenses Approval" Then
            oCombobox.Select("CA", SAPbouiCOM.BoSearchKey.psk_ByValue)
        End If
        If strstatus <> "P" Then
            oForm.Items.Item("1").Visible = False
        Else
            oForm.Items.Item("1").Visible = True
        End If
        'oForm.Freeze(False)
        'If strstatus = "Request Approved" Then
        '    oCombobox.Select("CR", SAPbouiCOM.BoSearchKey.psk_ByValue)
        'End If

        'If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE Then
        '    oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        'End If
        oForm.EnableMenu(mnu_FIND, False)
        oForm.EnableMenu(mnu_ADD, False)
        oForm.Freeze(False)
    End Sub
    Private Sub LocalCurrency(ByVal aForm As SAPbouiCOM.Form)
        Try
            Dim oRec As SAPbobsCOM.Recordset
            Dim strQuery As String
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strQuery = "Select T1.""CurrName"" from OADM T0 inner join OCRN T1 on T0.""MainCurncy""=T1.""CurrCode"""
            oRec.DoQuery(strQuery)
            If oRec.RecordCount > 0 Then
                oApplication.Utilities.setEdittextvalue(aForm, "68", oRec.Fields.Item(0).Value)
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
    Private Sub FillDepartment(ByVal sform As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = sform.Items.Item("11").Specific
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oTempRec.DoQuery("Select Code,Remarks from OUDP")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item("Code").Value, oTempRec.Fields.Item("Remarks").Value)
            oTempRec.MoveNext()
        Next
        sform.Items.Item("11").DisplayDesc = True
    End Sub
    Private Sub FillTravelCode(ByVal sform As SAPbouiCOM.Form, ByVal Empid As String)
        Dim oTempRec As SAPbobsCOM.Recordset
        Dim strqry As String
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = sform.Items.Item("21").Specific
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        strqry = "select Code,U_Z_TraCode  from [@Z_HR_OASSTP] where U_Z_EmpId =" & Empid & ""
        oTempRec.DoQuery(strqry)
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item("U_Z_TraCode").Value, oTempRec.Fields.Item("Code").Value)
            oTempRec.MoveNext()
        Next
        'sform.Items.Item("21").DisplayDesc = True
    End Sub

    Private Sub PopulateEmployee(ByVal aForm As SAPbouiCOM.Form, ByVal Empid As String)
        aForm.Freeze(True)
        Dim strqry, strcode As String
        Dim oRect As SAPbobsCOM.Recordset
        oRect = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = aForm.Items.Item("11").Specific
        strqry = "Select * from OHEM where empID=" & Empid & ""
        oRect.DoQuery(strqry)
        If oRect.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(aForm, "8", oRect.Fields.Item("empID").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "12", oRect.Fields.Item("firstName").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "14", oRect.Fields.Item("U_Z_HR_PosiCode").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "15", oRect.Fields.Item("U_Z_HR_PosiName").Value)
            strcode = oRect.Fields.Item("dept").Value
            oCombobox.Select(strcode, SAPbouiCOM.BoSearchKey.psk_ByValue)
            Department(aForm, strcode)

        End If
        aForm.Freeze(False)
    End Sub
    Private Sub PopulateTraPlan(ByVal aForm As SAPbouiCOM.Form, ByVal stCode As String)
        aForm.Freeze(True)
        Dim strqry, strcode As String
        Dim oRect As SAPbobsCOM.Recordset
        oRect = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strqry = "Select * from [@Z_HR_OASSTP] where Code=" & stCode & ""
        oRect.DoQuery(strqry)
        If oRect.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(aForm, "23", oRect.Fields.Item("U_Z_TraName").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "41", oRect.Fields.Item("U_Z_EffeFromDt").Value)
            oApplication.Utilities.setEdittextvalue(aForm, "43", oRect.Fields.Item("U_Z_EffeToDt").Value)
        End If
        aForm.Freeze(False)
    End Sub
    Private Sub Department(ByVal aForm As SAPbouiCOM.Form, ByVal Deptcode As String)
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("select Remarks from OUDP  where Code=" & Deptcode & "")
        If oSlpRS.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(aForm, "10", oSlpRS.Fields.Item(0).Value)
        End If
    End Sub
    Private Sub AddMode(ByVal aform As SAPbouiCOM.Form)
        Try
            aform.Freeze(True)
            Dim strCode As String
            Dim dt As Date
            dt = Now.Date
            strCode = oApplication.Utilities.getMaxCode("@Z_HR_OTRAREQ", "DocEntry")
            oApplication.Utilities.setEdittextvalue(aform, "4", CInt(strCode))
            oApplication.Utilities.setEdittextvalue(aform, "6", dt)
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub
    Private Sub PopulateExpenses(ByVal sForm As SAPbouiCOM.Form, ByVal Tracode As String, ByVal stcode As String)
        Dim strqry As String
        Dim oRect As SAPbobsCOM.Recordset
        oMatrix = sForm.Items.Item("38").Specific
        oMatrix.Clear()
        oRect = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strqry = "select U_Z_ExpName,U_Z_Amount,U_Z_UtilAmt,U_Z_BalAmount  from [@Z_HR_ASSTP1] where U_Z_TraCode='" & Tracode & "' and U_Z_RefCode='" & stcode & "' "
        oRect.DoQuery(strqry)
        If oRect.RecordCount > 0 Then
            For introw As Integer = 0 To oRect.RecordCount - 1
                If oMatrix.RowCount <= 0 Then
                    oMatrix.AddRow()
                End If
                oEditText = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                If oEditText.Value <> "" Then
                    oMatrix.AddRow()
                End If
                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, oRect.Fields.Item("U_Z_ExpName").Value)
                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, oRect.Fields.Item("U_Z_Amount").Value)
                oApplication.Utilities.SetMatrixValues(oMatrix, "V_3", oMatrix.RowCount, oRect.Fields.Item("U_Z_UtilAmt").Value)
                oApplication.Utilities.SetMatrixValues(oMatrix, "V_5", oMatrix.RowCount, oRect.Fields.Item("U_Z_BalAmount").Value)
                AssignLineNo1(sForm)
                oRect.MoveNext()
            Next
            oMatrix.Columns.Item("V_0").Editable = False
            oMatrix.Columns.Item("V_1").Editable = False
            oMatrix.AutoResizeColumns()
        End If
    End Sub
#Region "FileOpen/LoadFiles"
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
        oMatrix = aform.Items.Item("45").Specific
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
#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            oCheckbox = aForm.Items.Item("57").Specific

            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            If oApplication.Utilities.getEdittextvalue(aForm, "8") = "" Then
                oApplication.Utilities.Message("Select Employee Id...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "25") = "" Then
                oApplication.Utilities.Message("Select Travel From Location...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "27") = "" Then
                oApplication.Utilities.Message("Select Travel To Location...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "23") = "" And oCheckbox.Checked = False Then
                oApplication.Utilities.Message("Select Trip Code or New travel request...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "29") = "" Then
                oApplication.Utilities.Message("Enter Travel Start Date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "31") = "" Then
                oApplication.Utilities.Message("Enter Travel End Date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            Dim strfromdt, sttodt As Date
            strfromdt = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "29"))
            sttodt = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "31"))
            If strfromdt > sttodt Then
                oApplication.Utilities.Message("Travel End Date must be greater than or equal to Travel Start date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If oForm.Title = "Employee Travel Request" Then
                If oApplication.Utilities.getEdittextvalue(aForm, "23") <> "" And oCheckbox.Checked = True Then
                    oApplication.Utilities.Message("Select either Trip Code or New Trip Request ...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            End If
            If oForm.Title = "Travel Request Approval" Then
                If oApplication.Utilities.getEdittextvalue(aForm, "56") = "" Then
                    oApplication.Utilities.Message("Enter Requested Approved Date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                If oCheckbox.Checked = True Then
                    If oApplication.Utilities.getEdittextvalue(aForm, "23") = "" Then
                        oApplication.Utilities.Message("Select Trip Code ...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If

                'Dim strstatus As String
                'oCombobox = aForm.Items.Item("33").Specific
                'strstatus = oCombobox.Selected.Value
                'If strstatus = "CR" Or strstatus = "CA" Or strstatus = "CJ" Then
                '    oApplication.Utilities.Message("This status is not applicable for this approval...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '    Return False
                'End If
            End If
            If oForm.Title = "Employee Expenses Claim Request" Then
                If oApplication.Utilities.getEdittextvalue(aForm, "52") = "" Then
                    oApplication.Utilities.Message("Enter Requested Claim Date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                Dim strstatus As String
                oCombobox = aForm.Items.Item("33").Specific
                strstatus = oCombobox.Selected.Value
                If strstatus = "RA" Then
                    oCombobox.Select("CR", SAPbouiCOM.BoSearchKey.psk_ByValue)
                End If
            End If
            If oForm.Title = "Employee Expenses Approval" Then
                If oApplication.Utilities.getEdittextvalue(aForm, "54") = "" Then
                    oApplication.Utilities.Message("Enter Approved Claim Date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                'Dim strstatus As String
                'oCombobox = aForm.Items.Item("33").Specific
                'strstatus = oCombobox.Selected.Value
                'If strstatus = "O" Or strstatus = "RA" Or strstatus = "RJ" Then
                '    oApplication.Utilities.Message("This status is not applicable for this approval...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '    Return False
                'End If
            End If
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
#End Region

    Private Sub AssignLineNo(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("45").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_TRAREQ2")
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
            oMatrix = aForm.Items.Item("38").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_TRAREQ1")
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
    Private Sub deleterow(ByVal aForm As SAPbouiCOM.Form)
        Select Case aForm.PaneLevel
            Case "4"
                oMatrix = aForm.Items.Item("45").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_TRAREQ2")
            Case "3"
                oMatrix = aForm.Items.Item("38").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_TRAREQ1")
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
                        oMatrix = aForm.Items.Item("45").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_TRAREQ2")
                        AssignLineNo(aForm)
                    Case "3"
                        oMatrix = aForm.Items.Item("38").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_TRAREQ1")
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
    Private Sub RefereshDeleteRow(ByVal aForm As SAPbouiCOM.Form)
        If Me.MatrixId = "45" Then
            oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_TRAREQ2")
        ElseIf Me.MatrixId = "38" Then
            oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_TRAREQ1")
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
    Private Sub AddRow(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)

            Select Case aForm.PaneLevel
                Case "4"
                    oMatrix = aForm.Items.Item("45").Specific
                    oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_TRAREQ2")
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
                Case "3"
                    oMatrix = aForm.Items.Item("38").Specific
                    oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_TRAREQ1")
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
                    AssignLineNo(aForm)

            End Select


            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)

        End Try
    End Sub

    Private Sub reDrawForm(ByVal oForm As SAPbouiCOM.Form)
        'Try
        '    oForm.Freeze(True)
        '    oForm.Items.Item("19").Width = oForm.Width - 30
        '    oForm.Items.Item("19").Height = oForm.Height - 160
        '    oForm.Freeze(False)
        'Catch ex As Exception
        '    oForm.Freeze(False)
        'End Try
    End Sub
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_TraRequest Then
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
                                    Else
                                        Dim strValue As String
                                        oCombobox = oForm.Items.Item("68").Specific
                                        strValue = oApplication.Utilities.DocApproval(oForm, HeaderDoctype.TraReq, oApplication.Utilities.getEdittextvalue(oForm, "8"))
                                        oCombobox.Select(strValue, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                    End If
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.GetForm(pVal.FormTypeEx, pVal.FormTypeCount)
                                If pVal.ItemUID = "45" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("45").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "45"
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
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "21" Then
                                    Dim stCode, stCode1 As String
                                    oCombobox = oForm.Items.Item("21").Specific
                                    stCode = oCombobox.Selected.Value
                                    If stCode <> "" Then
                                        stCode1 = oCombobox.Selected.Description
                                        oApplication.Utilities.setEdittextvalue(oForm, "39", stCode1)
                                        PopulateTraPlan(oForm, stCode1)
                                        PopulateExpenses(oForm, stCode, stCode1)
                                    Else
                                        oApplication.Utilities.setEdittextvalue(oForm, "39", "")
                                        oApplication.Utilities.setEdittextvalue(oForm, "23", "")
                                        oApplication.Utilities.setEdittextvalue(oForm, "41", "")
                                        oApplication.Utilities.setEdittextvalue(oForm, "43", "")
                                    End If

                                End If

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "70"
                                        Dim objHistory As New clshrAppHisDetails
                                        objHistory.LoadForm(oForm, HistoryDoctype.TraReq, oApplication.Utilities.getEdittextvalue(oForm, "4"))
                                    Case "16"
                                        oForm.PaneLevel = 1
                                    Case "17"
                                        oForm.PaneLevel = 2
                                    Case "18"
                                        oForm.PaneLevel = 3
                                    Case "44"
                                        oForm.PaneLevel = 4
                                    Case "49"
                                        AddRow(oForm)
                                    Case "50"
                                        deleterow(oForm)
                                        RefereshDeleteRow(oForm)
                                    Case "1"
                                        If oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                                            oApplication.Utilities.Message("Operation Completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                            oForm.Close()
                                        End If

                                    Case "48"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            Exit Sub
                                        End If
                                        deleterow(oForm)
                                        RefereshDeleteRow(oForm)
                                    Case "47"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            Exit Sub
                                        End If
                                        LoadFiles(oForm)
                                    Case "46"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            Exit Sub

                                        End If
                                        fillopen()
                                        If strSelectedFilepath <> "" Then
                                            oMatrix = oForm.Items.Item("45").Specific
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
                                                ' oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "t")
                                                oEditText = oMatrix.Columns.Item("V_1").Cells.Item(oMatrix.RowCount).Specific
                                                oEditText.String = "t"
                                                oApplication.SBO_Application.SendKeys("{TAB}")
                                                oForm.Items.Item("31").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                                Try
                                                    oMatrix.Columns.Item("V_1").Editable = False
                                                Catch ex As Exception
                                                End Try

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
                Case mnu_hr_TraRequest
                    'LoadForm()
                    Dim oTe As New clshrLogin
                    oTe.LoadForm("TraReq")
                Case mnu_hr_TraApproval
                    Dim oTe As New clshrLogin
                    oTe.LoadForm("HRTraReqApp")
                Case mnu_hr_ExpenseClaim
                    Dim oTe As New clshrLogin
                    oTe.LoadForm("EmpExpClaim")
                Case mnu_hr_ExpApproval
                    Dim oTe As New clshrLogin
                    oTe.LoadForm("HRExpApproval")
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
                Case mnu_ADD
                    AddMode(oForm)
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
                Dim strdocnum As String
                Dim stXML As String = BusinessObjectInfo.ObjectKey
                stXML = stXML.Replace("<?xml version=""1.0"" encoding=""UTF-16"" ?><Employee Travel RequestParams><DocEntry>", "")
                stXML = stXML.Replace("</DocEntry></Employee Travel RequestParams>", "")

                Dim otest As SAPbobsCOM.Recordset
                otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                If stXML <> "" Then

                    otest.DoQuery("select * from [@Z_HR_OTRAREQ]  where DocEntry=" & stXML)
                    If otest.RecordCount > 0 Then
                        Dim intTempID As String = oApplication.Utilities.GetTemplateID(oForm, HeaderDoctype.TraReq, otest.Fields.Item("U_Z_EmpID").Value)

                        If intTempID <> "0" Then
                            oApplication.Utilities.InitialMessage("Travel Request", otest.Fields.Item("DocEntry").Value, oApplication.Utilities.DocApproval(oForm, HeaderDoctype.TraReq, otest.Fields.Item("U_Z_EmpID").Value), intTempID, otest.Fields.Item("U_Z_EmpName").Value, HistoryDoctype.TraReq)
                        End If
                    End If

                End If


                ' oApplication.Company.GetNewObjectCode(strdocnum)
                'Dim intTempID As String = oApplication.Utilities.GetTemplateID(aform, HeaderDoctype.ExpCli, oApplication.Utilities.getEdittextvalue(aform, "15"))
                'If intTempID <> "0" Then
                '    oApplication.Utilities.InitialMessage("Expense Claim", strCode, oApplication.Utilities.DocApproval(aform, HeaderDoctype.ExpCli, oApplication.Utilities.getEdittextvalue(aform, "15")), intTempID, oApplication.Utilities.getEdittextvalue(aform, "4"))
                'End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
End Class
