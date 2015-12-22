Public Class clshrTrainPlan
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private objMatrix As SAPbouiCOM.Matrix
    Private objForm As SAPbouiCOM.Form
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox, oCombobox1 As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oComboboxcolumn As SAPbouiCOM.ComboBoxColumn
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private oColumn As SAPbouiCOM.Column
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private MatrixId As String
    Private InvBaseDocNo As String
    Private RowtoDelete As Integer
    Private sPath, strSelectedFilepath, strSelectedFolderPath As String
    Private InvForConsumedItems, count As Integer
    Private blnFlag As Boolean = False
    Dim oDataSrc_Line, oDataSrc_Line3 As SAPbouiCOM.DBDataSource
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_TrainPlan1) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_TrainPlan, frm_hr_TrainPlan1)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.DataBrowser.BrowseBy = "1000001"
        AddChooseFromList(oForm)
        databind(oForm)
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.EnableMenu("1283", True)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_OTRIN1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oForm.Items.Item("9").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oForm.PaneLevel = 1
        reDrawForm(oForm)
        oForm.Freeze(False)
    End Sub
    Public Sub LoadForm1(ByVal AgendaCode As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_TrainPlan1) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_TrainPlan, frm_hr_TrainPlan1)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        ' oForm.DataBrowser.BrowseBy = "1000001"
        AddChooseFromList(oForm)
        databind(oForm)
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.EnableMenu("1283", True)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_OTRIN1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oForm.Items.Item("1000001").Enabled = True
        oApplication.Utilities.setEdittextvalue(oForm, "1000001", AgendaCode)
        oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE Then
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        End If
        oForm.Items.Item("1000001").Enabled = False
        oForm.PaneLevel = 1
        reDrawForm(oForm)
        oForm.Freeze(False)
    End Sub
    Private Sub databind(ByVal aForm As SAPbouiCOM.Form)

        oEditText = aForm.Items.Item("13").Specific
        oEditText.ChooseFromListUID = "CFL1"
        oEditText.ChooseFromListAlias = "U_Z_CouTypeCode"

        oEditText = aForm.Items.Item("9").Specific
        oEditText.ChooseFromListUID = "CFL2"
        oEditText.ChooseFromListAlias = "U_Z_CourseCode"

        oEditText = aForm.Items.Item("51").Specific
        oEditText.ChooseFromListUID = "CFL5"
        oEditText.ChooseFromListAlias = "DocEntry"

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
            oCFLCreationParams.ObjectType = "Z_HR_OCOTY"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "U_Z_Status"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_TRRAPP"
            oCFLCreationParams.UniqueID = "CFL5"
            oCFL = oCFLs.Add(oCFLCreationParams)

            'oCons = oCFL.GetConditions()
            'oCon = oCons.Add()
            'oCon.Alias = "U_Z_Active"
            'oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            'oCon.CondVal = "Y"
            'oCFL.SetConditions(oCons)
            'oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_OCOURS"
            oCFLCreationParams.UniqueID = "CFL2"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFL = oCFLs.Item("CFL_2")

            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_3")

            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()




        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub AddMode(ByVal aform As SAPbouiCOM.Form)
        Dim strCode As String
        strCode = oApplication.Utilities.getMaxCode("@Z_HR_OTRIN", "DocEntry")
        aform.Items.Item("1000001").Enabled = True
        aform.Items.Item("7").Enabled = True
        oApplication.Utilities.setEdittextvalue(aform, "1000001", strCode)
        oApplication.Utilities.setEdittextvalue(aform, "1000002", CInt(strCode))
        aform.Items.Item("7").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oApplication.Utilities.setEdittextvalue(aform, "7", "t")
        oApplication.SBO_Application.SendKeys("{TAB}")
        aform.Items.Item("9").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        aform.Items.Item("1000001").Enabled = False
        aform.Items.Item("7").Enabled = False
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
        oMatrix = aform.Items.Item("46").Specific
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
    Private Sub AssignLineNo(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("46").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_OTRIN1")
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
                Case "2"
                    oMatrix = aForm.Items.Item("46").Specific
                    oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_OTRIN1")
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
                    AssignLineNo(aForm)
                    aForm.Freeze(False)
            End Select
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
        End Try
    End Sub
    Private Sub deleterow(ByVal aForm As SAPbouiCOM.Form)
        Select Case aForm.PaneLevel
            Case "2"
                oMatrix = aForm.Items.Item("46").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_OTRIN1")
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
                    Case "2"
                        oMatrix = aForm.Items.Item("46").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_OTRIN1")
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
        If Me.MatrixId = "46" Then
            oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_OTRIN1")
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
#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try

            Dim oTest As SAPbobsCOM.Recordset
            Dim strfromdt, sttodt, stAppstdt, stAppEnddt As Date
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            If oApplication.Utilities.getEdittextvalue(aForm, "9") = "" Then
                oApplication.Utilities.Message("Course Code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "13") = "" Then
                oApplication.Utilities.Message("Course Type is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "17") = "" Then
                oApplication.Utilities.Message("Start Date is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "19") = "" Then
                oApplication.Utilities.Message("End Date is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "21") = "" Then
                oApplication.Utilities.Message("Application Issue date is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "23") = "" Then
                oApplication.Utilities.Message("Application Deadline is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "27") = "" Then
                oApplication.Utilities.Message("Maximum No of Attendees is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            strfromdt = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "17"))
            sttodt = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "19"))
            stAppstdt = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "21"))
            stAppEnddt = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "23"))
            If stAppstdt > stAppEnddt Then
                oApplication.Utilities.Message("Application End Date must be greater than or equal to Application Issue date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If stAppEnddt > strfromdt Then
                oApplication.Utilities.Message(" Application End date must be Less than or equal to Course Start date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If strfromdt > sttodt Then
                oApplication.Utilities.Message("Course End Date must be greater than or equal to Course Start date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If

            'Dim oTemp1 As SAPbobsCOM.Recordset
            'Dim stSQL1 As String
            'oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            'If aForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Or aForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
            '    stSQL1 = "Select * from [@Z_HR_OCRAPP] where docentry<>" & oApplication.Utilities.getEdittextvalue(aForm, "4") & " and  U_Z_EmailId='" & oApplication.Utilities.getEdittextvalue(aForm, "1000002") & "'"
            '    oTemp1.DoQuery(stSQL1)
            '    If oTemp1.RecordCount > 0 Then
            '        oApplication.Utilities.Message("EmailId already exists", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '        Return False
            '    End If
            'End If

            oMatrix = aForm.Items.Item("46").Specific
            If oMatrix.RowCount <= 0 Then
                'oApplication.Utilities.Message("Attachment is missing", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                'Return False
            ElseIf oApplication.Utilities.getMatrixValues(oMatrix, "V_0", 1) = "" Then
                'oApplication.Utilities.Message("Attachment is missing", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                'Return False
            End If

            AssignLineNo(aForm)
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
            oForm.Items.Item("30").Width = oForm.Width - 30
            oForm.Items.Item("30").Height = oForm.Height - 290
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
        End Try
    End Sub


#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_TrainPlan1 Then
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
                                If pVal.ItemUID = "64" Then
                                    Dim strcode As String = oApplication.Utilities.getEdittextvalue(oForm, "9")
                                    Dim ooBj As New clshrCourse
                                    ooBj.LoadForm1(strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                ElseIf pVal.ItemUID = "65" Then
                                    oApplication.Utilities.OpenMasterinLink(oForm, "CourseType")
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                reDrawForm(oForm)
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT

                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "53" Then
                                    oCombobox = oForm.Items.Item("53").Specific
                                    If oCombobox.Selected.Value = "O" Then
                                        oForm.Items.Item("1").Enabled = True

                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID

                                    Case "1"
                                        If oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                                            AddMode(oForm)
                                        End If

                                    Case "1000003"
                                        oForm.PaneLevel = 1
                                    Case "29"
                                        oForm.PaneLevel = 2
                                    Case "58"
                                        If oApplication.Utilities.getEdittextvalue(oForm, "51") <> "" Then
                                            Dim oobj As New clshrTrainner
                                            oobj.ViewCandidate(oApplication.Utilities.getEdittextvalue(oForm, "51"))
                                        End If

                                    Case "49"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            Exit Sub
                                        End If
                                        deleterow(oForm)
                                        RefereshDeleteRow(oForm)
                                    Case "48"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            Exit Sub
                                        End If
                                        LoadFiles(oForm)
                                    Case "47"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            Exit Sub

                                        End If
                                        fillopen()
                                        If strSelectedFilepath <> "" Then


                                            oMatrix = oForm.Items.Item("46").Specific
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
                                                oEditText.String = Now.Date
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
                                        If pVal.ItemUID = "9" Then
                                            val1 = oDataTable.GetValue("U_Z_CourseCode", 0)
                                            val = oDataTable.GetValue("U_Z_CourseName", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "11", val)
                                                oApplication.Utilities.setEdittextvalue(oForm, "9", val1)
                                            Catch ex As Exception
                                            End Try
                                        End If

                                        If pVal.ItemUID = "51" Then
                                            val = oDataTable.GetValue("DocEntry", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, pVal.ItemUID, val)
                                            Catch ex As Exception
                                            End Try
                                        End If


                                        If pVal.ItemUID = "55" Then
                                            val1 = oDataTable.GetValue("FormatCode", 0)
                                            val = oDataTable.GetValue("FormatCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "55", val)

                                            Catch ex As Exception
                                            End Try
                                        End If


                                        If pVal.ItemUID = "57" Then
                                            val1 = oDataTable.GetValue("FormatCode", 0)
                                            val = oDataTable.GetValue("FormatCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "57", val)

                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "13" Then
                                            val1 = oDataTable.GetValue("U_Z_CouTypeCode", 0)
                                            val = oDataTable.GetValue("U_Z_CouTypeDesc", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "15", val)
                                                oApplication.Utilities.setEdittextvalue(oForm, "13", val1)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
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
                Case mnu_hr_TrainPlan
                    LoadForm()
                Case mnu_ADD
                    AddMode(oForm)
                Case "1283"
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = True Then
                        Dim strValue As String
                        If oApplication.SBO_Application.MessageBox("Do you want to delete the details?", , "Yes", "No") = 2 Then
                            BubbleEvent = False
                            Exit Sub
                        End If
                        strValue = oApplication.Utilities.getEdittextvalue(oForm, "1000001")
                        If oApplication.Utilities.ValidateCode(strValue, "TRAINAGENDA") = True Then
                            BubbleEvent = False
                            Exit Sub
                        End If
                    End If
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
            If BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
                oCombobox = oForm.Items.Item("53").Specific
                If oCombobox.Selected.Value = "O" Then
                    oForm.Items.Item("1").Enabled = True
                Else
                    oForm.Items.Item("1").Enabled = False
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

End Class
