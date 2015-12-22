Public Class clshrExitResponse
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private objMatrix As SAPbouiCOM.Matrix
    Private objForm As SAPbouiCOM.Form
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
    Private InvBaseDocNo As String
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_ExitResponse) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_ExitResponse, frm_hr_ExitResponse)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.DataSources.UserDataSources.Add("SlNo", SAPbouiCOM.BoDataType.dt_SHORT_NUMBER)
        oForm.Freeze(True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.Settings.Enabled = True
        FillDepartment(oForm)
        FillPosition(oForm)
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        oForm.Freeze(False)
    End Sub
#Region "Fill Department and Position Code"
    Private Sub FillDepartment(ByVal sform As SAPbouiCOM.Form)
        Dim oMatrix As SAPbouiCOM.Matrix
        Dim oDBDataSrc As SAPbouiCOM.DBDataSource
        Dim otestrs As SAPbobsCOM.Recordset
        otestrs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otestrs.DoQuery("Update ""@Z_HR_ORES"" set ""Name""=' '  where ""Name"" Like '%_XD'")
        oMatrix = sform.Items.Item("3").Specific
        oDBDataSrc = sform.DataSources.DBDataSources.Add("@Z_HR_ORES")
        Try
            oDBDataSrc.Query()
        Catch ex As Exception

        End Try
        Dim oColum As SAPbouiCOM.Column
        oColum = oMatrix.Columns.Item("V_4")
        For intRow As Integer = oColum.ValidValues.Count - 1 To 0 Step -1
            oColum.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oColum.ValidValues.Add("", "")
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select Code,Name from OUDP order by Code")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oColum.ValidValues.Add(oSlpRS.Fields.Item("Code").Value, oSlpRS.Fields.Item("Name").Value)
            oSlpRS.MoveNext()
        Next
        oColum.DisplayDesc = True
        oColum = oMatrix.Columns.Item("SlNo")
        oColum.DataBind.SetBound(True, "", "SlNo")
        oMatrix.LoadFromDataSource()

        If oMatrix.RowCount >= 1 Then
            If oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Specific.Value <> "" Then
                oDBDataSrc.Clear()
                oMatrix.AddRow()
                oMatrix.Columns.Item(0).Cells.Item(oMatrix.RowCount).Specific.Value = oMatrix.RowCount
                'oCombobox = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                'oCombobox.Select("", SAPbouiCOM.BoSearchKey.psk_ByValue)
                oMatrix.Columns.Item(0).Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            End If
        ElseIf oMatrix.RowCount = 0 Then
            oMatrix.AddRow()
            oMatrix.Columns.Item(0).Cells.Item(oMatrix.RowCount).Specific.Value = oMatrix.RowCount
            'oCombobox = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
            ' oCombobox.Select("", SAPbouiCOM.BoSearchKey.psk_ByValue)
            oMatrix.Columns.Item(0).Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        End If
        oApplication.Utilities.AssignSerialNo(oMatrix, sform)
        oMatrix.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
    Private Sub FillPosition(ByVal sform As SAPbouiCOM.Form)
        Dim oMatrix As SAPbouiCOM.Matrix
        oMatrix = sform.Items.Item("3").Specific
        Dim oColum As SAPbouiCOM.Column
        oColum = oMatrix.Columns.Item("V_6")
        For intRow As Integer = oColum.ValidValues.Count - 1 To 0 Step -1
            oColum.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oColum.ValidValues.Add("", "")
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select name,descriptio From OHPS")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oColum.ValidValues.Add(oSlpRS.Fields.Item("name").Value, oSlpRS.Fields.Item("descriptio").Value)
            oSlpRS.MoveNext()
        Next
        oColum.DisplayDesc = True
    End Sub

#End Region
#Region "DataBind"

#Region "Enable Matrix After Update"
    '***************************************************************************
    'Type               : Procedure
    'Name               : EnblMatrixAfterUpdate
    'Parameter          : Application,Company,Form
    'Return Value       : 
    'Author             : DEV-2
    'Created Date       : 
    'Last Modified By   : 
    'Modified Date      : 
    'Purpose            : Enable the Matrix after update button is pressed.
    '***************************************************************************
    Private Sub EnblMatrixAfterUpdate(ByVal objApplication As SAPbouiCOM.Application, ByVal ocompany As SAPbobsCOM.Company, ByVal oForm As SAPbouiCOM.Form)
        Dim oMatrix As SAPbouiCOM.Matrix
        Dim oDBDSource As SAPbouiCOM.DBDataSource
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim lnErrCode As Long
        Dim strErrMsg As String
        Dim i As Integer

        Dim oGeneralService As SAPbobsCOM.GeneralService
        Dim oGeneralData As SAPbobsCOM.GeneralData
        Dim oGeneralParams As SAPbobsCOM.GeneralDataParams
        Dim oCompanyService As SAPbobsCOM.CompanyService
        Dim oChildren As SAPbobsCOM.GeneralDataCollection
        oCompanyService = oApplication.Company.GetCompanyService()
        Dim otestRs As SAPbobsCOM.Recordset
        Dim oChild As SAPbobsCOM.GeneralData
        Dim strCode As String
        Dim blnRecordExists As Boolean = False
        Try
            oMatrix = oForm.Items.Item("3").Specific
            oForm.Freeze(True)
            '  If oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Specific.value = "" Then
            If oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Specific.value = "" Then
                oMatrix.DeleteRow(oMatrix.RowCount)
            End If
            oUserTable = ocompany.UserTables.Item("Z_HR_ORES")
            oDBDSource = oForm.DataSources.DBDataSources.Item("@Z_HR_ORES")
            '  oMatrix.DeleteRow(oMatrix.RowCount)
            oMatrix.FlushToDataSource()
            For i = 0 To oDBDSource.Size - 1
                oGeneralService = oCompanyService.GetGeneralService("Z_HR_ORES")
                oGeneralData = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData)
                strCode = oDBDSource.GetValue("Code", i).Trim
                otestRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)
                otestRs.DoQuery("SElect * from [@Z_HR_ORES] where Code='" & strCode & "'")
                If otestRs.RecordCount > 0 Then
                    oGeneralParams.SetProperty("Code", strCode)
                    oGeneralData = oGeneralService.GetByParams(oGeneralParams)
                    oGeneralData.SetProperty("U_Z_DeptCode", oDBDSource.GetValue("U_Z_DeptCode", i))
                    oGeneralData.SetProperty("U_Z_DeptName", oDBDSource.GetValue("U_Z_DeptName", i))

                    Dim strstatus As String = oDBDSource.GetValue("U_Z_PosCode", i)
                    If strstatus = "" Then
                        strstatus = "Y"
                    End If
                    oGeneralData.SetProperty("U_Z_PosCode", strstatus.Trim)
                    '  oGeneralData.SetProperty("U_Z_PosCode", oDBDSource.GetValue("U_Z_PosCode", i))
                    oGeneralData.SetProperty("U_Z_PosName", oDBDSource.GetValue("U_Z_PosName", i))
                    oGeneralData.SetProperty("U_Z_ResCode", oDBDSource.GetValue("U_Z_ResCode", i))
                    oGeneralData.SetProperty("U_Z_ResDesc", oDBDSource.GetValue("U_Z_ResDesc", i))
                    oGeneralData.SetProperty("U_Z_ResID", oDBDSource.GetValue("U_Z_ResID", i))
                    oGeneralData.SetProperty("U_Z_ResName", oDBDSource.GetValue("U_Z_ResName", i))

                    blnRecordExists = True
                Else
                    oGeneralData.SetProperty("Code", strCode)
                    blnRecordExists = False
                End If

                If blnRecordExists = True Then
                    oGeneralService.Update(oGeneralData)
                Else
                    '  oGeneralService.Add(oGeneralData)
                End If
            Next
            oDBDSource.Query()
            ' oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            ' End If
            oForm.Freeze(False)
            Exit Sub
        Catch ex As Exception
            ocompany.GetLastError(lnErrCode, strErrMsg)
            If strErrMsg <> "" Then
                objApplication.MessageBox(strErrMsg)
            Else
                objApplication.MessageBox(ex.Message)
            End If
        End Try
    End Sub
#End Region

#Region "Insert Code and Doc Entry"
    '******************************************************************
    'Type               : Procedure
    'Name               : InsertCodeAndDocEntry
    'Parameter          : 
    'Return Value       : 
    'Author             : DEV-2
    'Created Date       : 
    'Last Modified By   : 
    'Modified Date      : 
    'Purpose            : Inserting code and docEntry values.
    '******************************************************************
    Public Sub InsertCodeAndDocEntry(ByVal aForm As SAPbouiCOM.Form)
        Dim oDBDSource As SAPbouiCOM.DBDataSource
        Dim strValue As String = "1"
        Try
            objForm = aForm
            aForm.Freeze(True)
            oDBDSource = objForm.DataSources.DBDataSources.Item("@Z_HR_ORES")
            objMatrix = objForm.Items.Item("3").Specific
            objMatrix.FlushToDataSource()
            Dim strCode, strDocEntry As String
            strCode = oApplication.Utilities.getMaxCode("@Z_HR_ORES", "Code")
            strDocEntry = oApplication.Utilities.getMaxCode("@Z_HR_ORES", "DocEntry")
            If objMatrix.RowCount = 1 Then
                oDBDSource.SetValue("Code", 0, strValue.PadLeft(8, "0"))
                oDBDSource.SetValue("DocEntry", 0, strValue.PadLeft(8, "0"))
            Else
                'oDBDSource.SetValue("Code", objMatrix.RowCount - 1, oDBDSource.GetValue("DocEntry", objMatrix.RowCount - 1).PadLeft(8, "0"))
                'oDBDSource.SetValue("DocEntry", objMatrix.RowCount - 1, oDBDSource.GetValue("DocEntry", objMatrix.RowCount - 1).PadLeft(8, "0"))
                oDBDSource.SetValue("Code", objMatrix.RowCount - 1, strCode)
                oDBDSource.SetValue("DocEntry", objMatrix.RowCount - 1, CInt(strDocEntry))
            End If
            objMatrix.LoadFromDataSource()
            aForm.Freeze(False)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
#End Region

    Private Sub DeleteRow(ByVal aform As SAPbouiCOM.Form, ByVal aRow As Integer)
        Dim oMatrix As SAPbouiCOM.Matrix
        Dim oDBDSource As SAPbouiCOM.DBDataSource
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim lnErrCode As Long
        Dim strErrMsg As String
        Dim i As Integer
        Dim oGeneralService As SAPbobsCOM.GeneralService
        Dim oGeneralData As SAPbobsCOM.GeneralData
        Dim oGeneralParams As SAPbobsCOM.GeneralDataParams
        Dim oCompanyService As SAPbobsCOM.CompanyService
        Dim oChildren As SAPbobsCOM.GeneralDataCollection
        oCompanyService = oApplication.Company.GetCompanyService()
        Dim otestRs As SAPbobsCOM.Recordset
        Dim oChild As SAPbobsCOM.GeneralData
        Dim strCode As String
        Dim blnRecordExists As Boolean = False
        Try
            oMatrix = oForm.Items.Item("3").Specific
            oForm.Freeze(True)
            For intRow As Integer = aRow To aRow
                If oMatrix.IsRowSelected(intRow) Then
                    strCode = oMatrix.Columns.Item("V_8").Cells.Item(intRow).Specific.value
                    otestRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oGeneralService = oCompanyService.GetGeneralService("Z_HR_ORES")
                    oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)
                    otestRs.DoQuery("Update ""@Z_HR_ORES"" set ""Name""=isnull(""Name"",'') + '_XD' where ""Code""='" & strCode & "'")
                    oMatrix.DeleteRow(intRow)
                    oApplication.Utilities.AssignSerialNo(oMatrix, aform)
                    If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                        oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                    End If
                    oForm.Freeze(False)
                End If
            Next
            oForm.Freeze(False)
            Exit Sub
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub


    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        oMatrix = oForm.Items.Item("3").Specific
        Dim strcode, strcode1, strcode2, strcode3, strcode4 As String

        For intRow As Integer = 1 To oMatrix.RowCount
            strcode = oApplication.Utilities.getMatrixValues(oMatrix, "V_5", intRow)
            strcode1 = oApplication.Utilities.getMatrixValues(oMatrix, "V_7", intRow)
            strcode2 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", intRow)
            strcode3 = oApplication.Utilities.getMatrixValues(oMatrix, "V_1", intRow)
            strcode4 = oApplication.Utilities.getMatrixValues(oMatrix, "V_3", intRow)
            If strcode = "" And strcode1 = "" And strcode2 = "" And strcode3 = "" Then
                '  oApplication.Utilities.Message("Department can not be empty", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                ' oMatrix.Columns.Item("V_2").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                'Return False
            End If
            If strcode <> "" And strcode1 <> "" And strcode2 = "" And strcode3 <> "" And strcode4 <> "" Then
                oApplication.Utilities.Message("Responsibilities Code can not be empty", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If
            If strcode <> "" And strcode1 <> "" And strcode2 <> "" And strcode3 = "" And strcode4 <> "" Then
                oApplication.Utilities.Message("Responsibilities Description can not be empty", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_1").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If

            If strcode <> "" And strcode1 = "" And strcode2 <> "" And strcode3 <> "" And strcode4 <> "" Then
                oApplication.Utilities.Message("Position can not be empty", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_6").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If

            If strcode = "" And strcode1 <> "" And strcode2 <> "" And strcode3 <> "" And strcode4 <> "" Then
                oApplication.Utilities.Message("Department can not be empty", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_4").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If
            If strcode <> "" And strcode1 <> "" And strcode2 <> "" And strcode3 <> "" And strcode4 = "" Then
                oApplication.Utilities.Message("Employee can not be empty", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_2").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If

        Next
        Return True
    End Function


#End Region
  
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_ExitResponse Then
                Select Case pVal.BeforeAction
                    Case True
                        If pVal.EventType = SAPbouiCOM.BoEventTypes.et_KEY_DOWN And pVal.CharPressed <> "9" And pVal.ItemUID = "3" And pVal.ColUID = "V_0" Then
                            Dim strVal As String
                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            objMatrix = oForm.Items.Item("3").Specific
                            strVal = oApplication.Utilities.getMatrixValues(objMatrix, "V_0", pVal.Row)
                            If oApplication.Utilities.ValidateCode(strVal, "RESPONSE") = True Then
                                BubbleEvent = False
                                Exit Sub
                            End If
                        End If
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" And pVal.ColUID = "V_0" And pVal.CharPressed <> 9 Then
                                    objMatrix = oForm.Items.Item("3").Specific
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1" And oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                    oForm.Freeze(True)
                                    If Validation(oForm) = False Then
                                        oForm.Freeze(False)
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                    InsertCodeAndDocEntry(oForm)
                                    EnblMatrixAfterUpdate(oApplication.SBO_Application, oApplication.Company, oForm)
                                    oForm.Freeze(False)
                                End If
                        End Select

                    Case False
                        If SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST Then
                            Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                            Dim oCFL As SAPbouiCOM.ChooseFromList
                            Dim val1 As String
                            Dim sCHFL_ID, val, val2, val3, val4, val5, val6 As String
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
                                    If pVal.ItemUID = "3" And pVal.ColUID = "V_2" Then
                                        val = oDataTable.GetValue("empID", 0)
                                        val1 = oDataTable.GetValue("firstName", 0) & " " & oDataTable.GetValue("middleName", 0) & " " & oDataTable.GetValue("lastName", 0)
                                        Try
                                            oMatrix = oForm.Items.Item(pVal.ItemUID).Specific
                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_3", pVal.Row, val1)
                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", pVal.Row, val)
                                        Catch ex As Exception
                                            oForm.Freeze(False)
                                        End Try
                                    End If
                                    oForm.Freeze(False)
                                End If
                            Catch ex As Exception
                                oForm.Freeze(False)
                            End Try
                        End If
                        If pVal.EventType = SAPbouiCOM.BoEventTypes.et_COMBO_SELECT Then
                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            oMatrix = oForm.Items.Item("3").Specific
                            If pVal.ItemUID = "3" And pVal.ColUID = "V_4" Then
                                Dim stCode, stCode1 As String
                                oCombobox = oMatrix.Columns.Item("V_4").Cells.Item(pVal.Row).Specific
                                stCode1 = oCombobox.Selected.Value
                                stCode = oCombobox.Selected.Description
                                Dim orec As SAPbobsCOM.Recordset
                                orec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                Dim strQuery As String = "Select ""Remarks"" from OUDP where ""Code""='" & stCode1 & "'"
                                orec.DoQuery(strQuery)
                                If orec.RecordCount > 0 Then
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_5", pVal.Row, orec.Fields.Item(0).Value)
                                End If
                            ElseIf pVal.ItemUID = "3" And pVal.ColUID = "V_6" Then
                                Dim stCode, stCode1 As String
                                oCombobox = oMatrix.Columns.Item("V_6").Cells.Item(pVal.Row).Specific
                                stCode1 = oCombobox.Selected.Value
                                stCode = oCombobox.Selected.Description
                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_7", pVal.Row, stCode)
                            End If
                        End If
                        If ((pVal.EventType = SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED) And (pVal.ItemUID = "1")) Then
                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            oForm.Freeze(True)
                            Dim otestrs As SAPbobsCOM.Recordset
                            otestrs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            otestrs.DoQuery("Delete from ""@Z_HR_ORES"" where ""Name"" Like '%_XD'")
                            objForm = oForm
                            objMatrix = objForm.Items.Item("3").Specific
                            objMatrix.AddRow()
                            objMatrix.Columns.Item(0).Cells.Item(objMatrix.RowCount).Specific.value = objMatrix.RowCount
                            oCombobox = objMatrix.Columns.Item("V_4").Cells.Item(objMatrix.RowCount).Specific
                            oCombobox.Select("", SAPbouiCOM.BoSearchKey.psk_ByValue)
                            oCombobox = objMatrix.Columns.Item("V_6").Cells.Item(objMatrix.RowCount).Specific
                            oCombobox.Select("", SAPbouiCOM.BoSearchKey.psk_ByValue)
                            objMatrix.Columns.Item("V_0").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_1").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_2").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_3").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_5").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_7").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_8").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            oApplication.Utilities.AssignSerialNo(objMatrix, oForm)
                            objMatrix.Columns.Item(1).Cells.Item(objMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                            objForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
                            oForm.Freeze(False)
                        End If
                End Select
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
            oForm.Freeze(False)
        End Try
    End Sub
#End Region

#Region "Menu Event"
    Public Overrides Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            Select Case pVal.MenuUID
                Case mnu_hr_ExitResponse
                    LoadForm()
                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = True Then
                        Dim strValue As String
                        objMatrix = oForm.Items.Item("3").Specific
                        If oApplication.SBO_Application.MessageBox("Do you want to delete the details?", , "Yes", "No") = 2 Then
                            BubbleEvent = False
                            Exit Sub
                        End If
                        For intRow As Integer = 1 To objMatrix.RowCount
                            If objMatrix.IsRowSelected(intRow) Then
                                strValue = oApplication.Utilities.getMatrixValues(objMatrix, "V_0", intRow)
                                If oApplication.Utilities.ValidateCode(strValue, "RESPONSE") = True Then
                                    BubbleEvent = False
                                    Exit Sub
                                Else
                                    DeleteRow(oForm, intRow)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                            End If
                        Next
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
            If BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
End Class
