Public Class clshrPosCompetence
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox, oCombobox1 As SAPbouiCOM.ComboBox
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
    Dim oDataSrc_Line, oDataSrc_Line1, oDataSrc_Line2, oDataSrc_Line3 As SAPbouiCOM.DBDataSource

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_PosComp) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_PosComp, frm_hr_PosComp)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.DataBrowser.BrowseBy = "26"
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.EnableMenu("1283", True)
        AddChooseFromList(oForm)
        databind(oForm)
        FillDepartment(oForm)
        FillDivision(oForm)
        FillLevels(oForm)
        FillJobCode(oForm)

        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line1 = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO2")
        For count = 1 To oDataSrc_Line1.Size - 1
            oDataSrc_Line1.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line2 = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO3")
        For count = 1 To oDataSrc_Line2.Size - 1
            oDataSrc_Line2.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line3 = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO4")
        For count = 1 To oDataSrc_Line3.Size - 1
            oDataSrc_Line3.SetValue("LineId", count - 1, count)
        Next
        oForm.PaneLevel = 1
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        reDrawForm(oForm)
        oForm.Freeze(False)
    End Sub
    Public Sub LoadForm1(ByVal PosCode As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_PosComp) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_PosComp, frm_hr_PosComp)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.EnableMenu("1283", True)
        AddChooseFromList(oForm)
        databind(oForm)
        FillDepartment(oForm)
        FillDivision(oForm)
        FillLevels(oForm)
        FillJobCode(oForm)

        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line1 = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO2")
        For count = 1 To oDataSrc_Line1.Size - 1
            oDataSrc_Line1.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line2 = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO3")
        For count = 1 To oDataSrc_Line2.Size - 1
            oDataSrc_Line2.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line3 = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO4")
        For count = 1 To oDataSrc_Line3.Size - 1
            oDataSrc_Line3.SetValue("LineId", count - 1, count)
        Next
        oForm.PaneLevel = 1
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oForm.Items.Item("26").Enabled = True
        oApplication.Utilities.setEdittextvalue(oForm, "26", PosCode)
        oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE Then
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        End If
        reDrawForm(oForm)
        oForm.Freeze(False)
    End Sub
    Private Sub FillLevels(ByVal aForm As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        Dim oDBDataSrc As SAPbouiCOM.DBDataSource
        Dim oMatrix As SAPbouiCOM.Matrix
        Dim oColum As SAPbouiCOM.Column
        oMatrix = aForm.Items.Item("3").Specific
        ' oDBDataSrc = objForm.DataSources.DBDataSources.Add("@Z_HR_ORGST")
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oColum = oMatrix.Columns.Item("V_3")
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
    Private Sub FillDepartment(ByVal sform As SAPbouiCOM.Form)
        oCombobox = oForm.Items.Item("9").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select Code,Remarks from OUDP order by Code")
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        oForm.Items.Item("9").DisplayDesc = True
    End Sub
    Private Sub FillDivision(ByVal sform As SAPbouiCOM.Form)
        oCombobox = oForm.Items.Item("35").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select Name,Remarks from OUBR order by Code")
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        oForm.Items.Item("35").DisplayDesc = True
    End Sub
    Private Sub FillJobCode(ByVal sform As SAPbouiCOM.Form)
        oCombobox = oForm.Items.Item("56").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select U_Z_PosCode,U_Z_PosName from [@Z_HR_OPOSCO] order by DocEntry")
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_ValueDescription
        oForm.Items.Item("56").DisplayDesc = True
    End Sub
#Region "Add Choose From List"
    Private Sub databind(ByVal aForm As SAPbouiCOM.Form)
        oMatrix = aForm.Items.Item("3").Specific
        oColumn = oMatrix.Columns.Item("V_0")
        oColumn.ChooseFromListUID = "CFL1"
        oColumn.ChooseFromListAlias = "U_Z_CompCode"

        oMatrix = aForm.Items.Item("28").Specific
        oColumn = oMatrix.Columns.Item("V_0")
        oColumn.ChooseFromListUID = "CFL4"
        oColumn.ChooseFromListAlias = "U_Z_CourseCode"

        oEditText = aForm.Items.Item("19").Specific
        oEditText.ChooseFromListUID = "CFL2"
        oEditText.ChooseFromListAlias = "U_Z_LvelCode"

        oEditText = aForm.Items.Item("1000001").Specific
        oEditText.ChooseFromListUID = "CFL3"
        oEditText.ChooseFromListAlias = "U_Z_GrdeCode"

        oEditText = aForm.Items.Item("31").Specific
        oEditText.ChooseFromListUID = "CFL5"
        oEditText.ChooseFromListAlias = "U_Z_OrgCode"

        oEditText = aForm.Items.Item("39").Specific
        oEditText.ChooseFromListUID = "CFL6"
        oEditText.ChooseFromListAlias = "empID"

        oEditText = aForm.Items.Item("58").Specific
        oEditText.ChooseFromListUID = "CFL_7"
        oEditText.ChooseFromListAlias = "U_Z_CompCode"

     
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
            oCFLCreationParams.ObjectType = "Z_HR_OCOMP"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)

          
            oCFLCreationParams.ObjectType = "Z_HR_OCOURS"
            oCFLCreationParams.UniqueID = "CFL4"
            oCFL = oCFLs.Add(oCFLCreationParams)


            oCFLCreationParams.ObjectType = "Z_HR_OLVL"
            oCFLCreationParams.UniqueID = "CFL2"
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

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_ORGST"
            oCFLCreationParams.UniqueID = "CFL5"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "171"
            oCFLCreationParams.UniqueID = "CFL6"
            oCFL = oCFLs.Add(oCFLCreationParams)

          


        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

#End Region

#Region "Methods"
    Private Sub AssignLineNo(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("3").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO1")
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
            oMatrix = aForm.Items.Item("28").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO2")
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
            oMatrix = aForm.Items.Item("47").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO3")
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
            oMatrix = aForm.Items.Item("48").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO4")
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

#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim strcode, strDivision As String
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            'oCombobox = aForm.Items.Item("9").Specific
            'strcode = oCombobox.Selected.Value
            'oCombobox1 = aForm.Items.Item("35").Specific
            'strDivision = oCombobox1.Selected.Value
            If strcode = "" Then
                'oApplication.Utilities.Message("Enter Department Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                'Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "26") = "" Then
                oApplication.Utilities.Message("Enter Job Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "7") = "" Then
                oApplication.Utilities.Message("Enter Job Description...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "31") = "" Then
                '  oApplication.Utilities.Message("Organization Code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                ' Return False
            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "58") = "" Then
                'oApplication.Utilities.Message("Enter Company Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                'Return False
            End If
            If strDivision = "" Then
                'oApplication.Utilities.Message("Enter Branch Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                'Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "39") = "" Then
                '  oApplication.Utilities.Message("Reporting To is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '  Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "54") = "" Then
                oApplication.Utilities.Message("Enter Salary Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            Dim strJobCode, strRptjob As String
            oCombobox = aForm.Items.Item("56").Specific
            strJobCode = oApplication.Utilities.getEdittextvalue(aForm, "26")
            strRptjob = oCombobox.Selected.Value
            If strJobCode = strRptjob Then
                oApplication.Utilities.Message("Job Code and Reporting to job is Same...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            Dim oTemp As SAPbobsCOM.Recordset
            Dim stSQL As String
            If aForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                stSQL = "Select * from [@Z_HR_OPOSCO] where U_Z_PosCode='" & oApplication.Utilities.getEdittextvalue(aForm, "26") & "'"
                oTemp.DoQuery(stSQL)
                If oTemp.RecordCount > 0 Then
                    oApplication.Utilities.Message("Job Code Already Exists", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            End If

            oMatrix = oForm.Items.Item("3").Specific
            Dim strcode2, strcode1 As String
            If oMatrix.RowCount = 0 Then
                'oApplication.Utilities.Message("Line Details missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                'Return False
            End If
            If oMatrix.RowCount > 0 Then
                Dim dbweight, TotWeight, dbweight1 As Double
                If oMatrix.RowCount > 1 Then
                    strcode2 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount)
                    strcode1 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount - 1)
                    If strcode1.ToUpper = strcode2.ToUpper Then
                        oApplication.Utilities.Message("This Entry Already Exists...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                        Return False
                    End If
                End If
                For introw As Integer = 1 To oMatrix.RowCount
                    strcode2 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", introw)
                    ' strcode1 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", introw - 1)
                    dbweight = oApplication.Utilities.getMatrixValues(oMatrix, "V_2", introw)
                    dbweight1 = dbweight1 + dbweight
                    TotWeight = 100
                Next
                If TotWeight <> dbweight1 Then
                    '  oApplication.Utilities.Message("Sum of Competency Weight% Should be Equal 100...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    '   Return False
                End If
            Else
                '  oApplication.Utilities.Message("Enter Competence Objective Details...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '  Return False
            End If
            oMatrix = oForm.Items.Item("28").Specific
            Dim strcode3, strcode4 As String
            If oMatrix.RowCount > 1 Then
                strcode3 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount)
                strcode4 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount - 1)
                If strcode3.ToUpper = strcode4.ToUpper Then
                    oApplication.Utilities.Message("This Entry Already Exists", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    Return False
                End If
            End If
            AssignLineNo(oForm)
            AssignLineNo1(oForm)
            AssignLineNo2(oForm)
            AssignLineNo3(oForm)
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

            Select Case aForm.PaneLevel
                Case "1"
                    oMatrix = aForm.Items.Item("3").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO1")
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
                                Case "2"
                                   oMatrix.ClearRowData(oMatrix.RowCount)
                                Case "4"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
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
                    AssignLineNo(aForm)
                Case "2"
                    oMatrix = aForm.Items.Item("28").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO1")

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
                                Case "2"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
                                Case "4"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
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
                    AssignLineNo1(aForm)
                Case "4"
                    oMatrix = aForm.Items.Item("47").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO3")
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
                                Case "2"
                                  oMatrix.ClearRowData(oMatrix.RowCount)
                                Case "4"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
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
                    AssignLineNo2(aForm)
                Case "5"
                    oMatrix = aForm.Items.Item("48").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO4")
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
                                Case "2"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
                                Case "4"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
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
                    AssignLineNo3(aForm)
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
                oMatrix = aForm.Items.Item("3").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_POSCO1")
            Case "2"
                oMatrix = aForm.Items.Item("28").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_POSCO2")
            Case "4"
                oMatrix = aForm.Items.Item("47").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_POSCO3")
            Case "5"
                oMatrix = aForm.Items.Item("48").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_POSCO4")

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
                        oMatrix = aForm.Items.Item("3").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_POSCO1")
                        AssignLineNo(aForm)
                    Case "2"
                        oMatrix = aForm.Items.Item("28").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_POSCO2")
                        AssignLineNo1(aForm)
                    Case "4"
                        oMatrix = aForm.Items.Item("47").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_POSCO3")
                        AssignLineNo1(aForm)
                    Case "5"
                        oMatrix = aForm.Items.Item("48").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_POSCO4")
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
        If Me.MatrixId = "3" Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO1")
        ElseIf Me.MatrixId = "28" Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO2")
        ElseIf Me.MatrixId = "47" Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO3")
        ElseIf Me.MatrixId = "48" Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_POSCO4")
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

    Private Sub reDrawForm(ByVal oForm As SAPbouiCOM.Form)
        Try
            oForm.Freeze(True)
            oForm.Items.Item("29").Width = oForm.Width - 30
            oForm.Items.Item("29").Height = oForm.Height - 200
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
        End Try
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_PosComp Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
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
                                If pVal.ItemUID = "80" Then
                                    Dim strcode As String = oApplication.Utilities.getEdittextvalue(oForm, "54")
                                    Dim ooBj As New clshrSalStructure
                                    ooBj.LoadForm1(strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.GetForm(pVal.FormTypeEx, pVal.FormTypeCount)
                                If pVal.ItemUID = "3" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("3").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "3"
                                    frmSourceMatrix = oMatrix
                                End If
                                If pVal.ItemUID = "28" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("28").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "28"
                                    frmSourceMatrix = oMatrix
                                End If
                                If pVal.ItemUID = "47" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("47").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "47"
                                    frmSourceMatrix = oMatrix
                                End If
                                If pVal.ItemUID = "48" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("48").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "48"
                                    frmSourceMatrix = oMatrix
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim strcode, strcode1 As String
                                If pVal.ItemUID = "1000001" Or pVal.ItemUID = "19" Then
                                    strcode = oApplication.Utilities.getEdittextvalue(oForm, "15")
                                    strcode1 = oApplication.Utilities.getEdittextvalue(oForm, "18")
                                    oApplication.Utilities.setEdittextvalue(oForm, "23", strcode & " " & strcode1)
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
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim strVal, strVal1, strVal2 As String
                                If pVal.CharPressed = "9" And pVal.ItemUID = "26" Then
                                    strVal1 = oApplication.Utilities.getEdittextvalue(oForm, "26")
                                    If oApplication.Utilities.ValidateCode(strVal1, "JOB") = True Then
                                        oApplication.Utilities.Message("Job Code already mapped. ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                        oForm.Items.Item("26").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "9" Then
                                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                                        Dim stCode, stCode1 As String
                                        oCombobox = oForm.Items.Item("9").Specific
                                        stCode1 = oCombobox.Selected.Value
                                        stCode = oCombobox.Selected.Description
                                        oApplication.Utilities.setEdittextvalue(oForm, "11", stCode)
                                    End If
                                End If
                                If pVal.ItemUID = "35" Then
                                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                                        Dim stCode2, stCode3 As String
                                        oCombobox = oForm.Items.Item("35").Specific
                                        stCode3 = oCombobox.Selected.Value
                                        stCode2 = oCombobox.Selected.Description
                                        oApplication.Utilities.setEdittextvalue(oForm, "37", stCode2)
                                    End If
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "1000002"
                                        oForm.PaneLevel = 1
                                    Case "27"
                                        oForm.PaneLevel = 2
                                    Case "40"
                                        oForm.PaneLevel = 3
                                    Case "41"
                                        oForm.PaneLevel = 4
                                    Case "42"
                                        oForm.PaneLevel = 5
                                    Case "24"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_ADD_ROW)
                                    Case "25"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_DELETE_ROW)
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
                                        If pVal.ItemUID = "19" Then
                                            val = oDataTable.GetValue("U_Z_LvelCode", 0)
                                            val1 = oDataTable.GetValue("U_Z_LvelName", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "18", val1)
                                                oApplication.Utilities.setEdittextvalue(oForm, "19", val)
                                            Catch ex As Exception
                                                If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                End If
                                            End Try
                                        End If
                                        If pVal.ItemUID = "1000001" Then
                                            val = oDataTable.GetValue("U_Z_GrdeCode", 0)
                                            val1 = oDataTable.GetValue("U_Z_GrdeName", 0)
                                            Try

                                                oApplication.Utilities.setEdittextvalue(oForm, "15", val1)
                                                oApplication.Utilities.setEdittextvalue(oForm, "1000001", val)
                                            Catch ex As Exception
                                                If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                End If
                                            End Try
                                        End If
                                        If pVal.ItemUID = "31" Then
                                            val = oDataTable.GetValue("U_Z_OrgCode", 0)
                                            val1 = oDataTable.GetValue("U_Z_OrgDesc", 0)
                                            Try

                                                oApplication.Utilities.setEdittextvalue(oForm, "60", oDataTable.GetValue("U_Z_CompName", 0))
                                                oApplication.Utilities.setEdittextvalue(oForm, "33", val1)
                                                oApplication.Utilities.setEdittextvalue(oForm, "31", val)
                                            Catch ex As Exception
                                                'oApplication.Utilities.setEdittextvalue(oForm, "15", val1)
                                            End Try
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "58", oDataTable.GetValue("U_Z_CompCode", 0))
                                            Catch ex As Exception
                                                If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                End If
                                            End Try

                                        End If

                                        If pVal.ItemUID = "58" Then
                                            val = oDataTable.GetValue("U_Z_CompCode", 0)
                                            val1 = oDataTable.GetValue("U_Z_CompName", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "60", val1)
                                                oApplication.Utilities.setEdittextvalue(oForm, "58", val)
                                            Catch ex As Exception
                                                If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                End If
                                            End Try
                                        End If
                                        If pVal.ItemUID = "3" And pVal.ColUID = "V_0" Then
                                            val1 = oDataTable.GetValue("U_Z_CompCode", 0)
                                            val = oDataTable.GetValue("U_Z_CompName", 0)
                                            val2 = oDataTable.GetValue("U_Z_CompLevel", 0)
                                            oMatrix = oForm.Items.Item("3").Specific
                                            Try
                                                oCombobox = oMatrix.Columns.Item("V_3").Cells.Item(pVal.Row).Specific
                                                ' oCombobox.Select(val2, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", pVal.Row, oDataTable.GetValue("U_Z_Weight", 0))
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val1)
                                            Catch ex As Exception
                                                If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                End If
                                            End Try
                                        End If
                                        If pVal.ItemUID = "28" And pVal.ColUID = "V_0" Then
                                            val1 = oDataTable.GetValue("U_Z_CourseCode", 0)
                                            val = oDataTable.GetValue("U_Z_CourseName", 0)
                                            oMatrix = oForm.Items.Item("28").Specific
                                            Try
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val1)
                                            Catch ex As Exception
                                                If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                End If
                                            End Try

                                        End If
                                        If pVal.ItemUID = "39" Then
                                            val = oDataTable.GetValue("firstName", 0)
                                            val1 = oDataTable.GetValue("empID", 0)
                                            oApplication.Utilities.setEdittextvalue(oForm, "50", val)
                                            oApplication.Utilities.setEdittextvalue(oForm, "39", val1)
                                        End If
                                        If pVal.ItemUID = "54" Then
                                            val = oDataTable.GetValue("U_Z_SalCode", 0)
                                            oApplication.Utilities.setEdittextvalue(oForm, pVal.ItemUID, val)
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
                Case mnu_hr_PosComp
                    LoadForm()
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        oForm.Items.Item("26").Enabled = False
                        oForm.Items.Item("7").Enabled = True
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
                        oForm.Items.Item("26").Enabled = True
                        oForm.Items.Item("7").Enabled = True
                        'oForm.Items.Item("8").Enabled = True
                    End If
                Case mnu_FIND
                    If pVal.BeforeAction = False Then
                        oForm = oApplication.SBO_Application.Forms.ActiveForm()
                        oForm.Items.Item("26").Enabled = True
                        oForm.Items.Item("7").Enabled = True
                    End If
                Case "1283"
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = True Then
                        Dim strValue As String
                        If oApplication.SBO_Application.MessageBox("Do you want to delete the details?", , "Yes", "No") = 2 Then
                            BubbleEvent = False
                            Exit Sub
                        End If
                        strValue = oApplication.Utilities.getEdittextvalue(oForm, "26")
                        If oApplication.Utilities.ValidateCode(strValue, "JOBSCREEN") = True Then
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
                If oForm.TypeEx = frm_hr_PosComp Then
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oForm.Items.Item("26").Enabled = False
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
