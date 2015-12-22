Public Class clshrCompLevel
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
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_CompLevel) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_CompLevel, frm_hr_CompLevel)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.DataSources.UserDataSources.Add("SlNo", SAPbouiCOM.BoDataType.dt_SHORT_NUMBER)
        oForm.Freeze(True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.Settings.Enabled = True
        BindData(oForm)
        oForm.Settings.Enabled = True
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        oForm.Freeze(False)
    End Sub

#Region "DataBind"

    Public Sub BindData(ByVal objform As SAPbouiCOM.Form)
        Dim oMatrix As SAPbouiCOM.Matrix
        Dim oDBDataSrc As SAPbouiCOM.DBDataSource
        Try
            Dim otestrs As SAPbobsCOM.Recordset
            otestrs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otestrs.DoQuery("Update ""@Z_HR_COLVL"" set ""Name""=' '  where ""Name"" Like '%_XD'")
            oMatrix = objform.Items.Item("3").Specific
            oDBDataSrc = objform.DataSources.DBDataSources.Add("@Z_HR_COLVL")
            Try
                oDBDataSrc.Query()
            Catch ex As Exception

            End Try

            Dim oColum As SAPbouiCOM.Column
            oColum = oMatrix.Columns.Item("V_2")
            Try
                oColum.ValidValues.Add("Y", "Yes")
                oColum.ValidValues.Add("N", "No")
            Catch ex As Exception

            End Try
            oColum.DisplayDesc = True
            oColum = oMatrix.Columns.Item("SlNo")
            oColum.DataBind.SetBound(True, "", "SlNo")
            oMatrix.LoadFromDataSource()
            If oMatrix.RowCount >= 1 Then
                If oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Specific.Value <> "" Then
                    oDBDataSrc.Clear()
                    oMatrix.AddRow()
                    oMatrix.Columns.Item(0).Cells.Item(oMatrix.RowCount).Specific.Value = oMatrix.RowCount
                    oCombobox = oMatrix.Columns.Item("V_2").Cells.Item(oMatrix.RowCount).Specific
                    oCombobox.Select("Y", SAPbouiCOM.BoSearchKey.psk_ByValue)
                    oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                End If
            ElseIf oMatrix.RowCount = 0 Then
                oMatrix.AddRow()
                oMatrix.Columns.Item(0).Cells.Item(oMatrix.RowCount).Specific.Value = oMatrix.RowCount
                oCombobox = oMatrix.Columns.Item("V_2").Cells.Item(oMatrix.RowCount).Specific
                oCombobox.Select("Y", SAPbouiCOM.BoSearchKey.psk_ByValue)
                oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            End If
            oApplication.Utilities.AssignSerialNo(oMatrix, objform)
            oMatrix.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
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
            If oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Specific.value = "" Then
                oMatrix.DeleteRow(oMatrix.RowCount)
            End If
            If 1 = 1 Then
                oUserTable = ocompany.UserTables.Item("Z_HR_COLVL")
                oDBDSource = oForm.DataSources.DBDataSources.Item("@Z_HR_COLVL")
                'oMatrix.DeleteRow(oMatrix.RowCount)
                oMatrix.FlushToDataSource()
                For i = 0 To oDBDSource.Size - 1
                    oGeneralService = oCompanyService.GetGeneralService("Z_HR_COLVL")
                    oGeneralData = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData)
                    strCode = oDBDSource.GetValue("Code", i).Trim
                    otestRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)
                    otestRs.DoQuery("SElect * from [@Z_HR_COLVL] where Code='" & strCode & "'")
                    If otestRs.RecordCount > 0 Then
                        oGeneralParams.SetProperty("Code", strCode)
                        oGeneralData = oGeneralService.GetByParams(oGeneralParams)
                        oGeneralData.SetProperty("U_Z_LvelCode", oDBDSource.GetValue("U_Z_LvelCode", i))
                        oGeneralData.SetProperty("U_Z_LvelName", oDBDSource.GetValue("U_Z_LvelName", i))
                        Dim strstatus As String = oDBDSource.GetValue("U_Z_Status", i)
                        If strstatus = "" Then
                            strstatus = "Y"
                        End If
                        oGeneralData.SetProperty("U_Z_Status", strstatus.Trim)
                        ' oGeneralData.SetProperty("U_Z_FrgnName", oDBDSource.GetValue("U_Z_FrgnName", i))
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
                oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            End If
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
            oDBDSource = objForm.DataSources.DBDataSources.Item("@Z_HR_COLVL")
            objMatrix = objForm.Items.Item("3").Specific
            objMatrix.FlushToDataSource()
            Dim strCode, strDocEntry As String
            strCode = oApplication.Utilities.getMaxCode("@Z_HR_COLVL", "Code")
            strDocEntry = oApplication.Utilities.getMaxCode("@Z_HR_COLVL", "DocEntry")
            If objMatrix.RowCount = 1 Then
                oDBDSource.SetValue("Code", 0, strValue.PadLeft(8, "0"))
                oDBDSource.SetValue("DocEntry", 0, strValue.PadLeft(8, "0"))
            Else
                'oDBDSource.SetValue("Code", objMatrix.RowCount - 1, oDBDSource.GetValue("DocEntry", objMatrix.RowCount - 1).PadLeft(8, "0"))
                ' oDBDSource.SetValue("DocEntry", objMatrix.RowCount - 1, oDBDSource.GetValue("DocEntry", objMatrix.RowCount - 1).PadLeft(8, "0"))
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


    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        oMatrix = oForm.Items.Item("3").Specific
        Dim strcode, strcode1 As String
        If oMatrix.RowCount > 1 Then

            strcode = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount)
            strcode1 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount - 1)
            If strcode.ToUpper = strcode1.ToUpper Then
                oApplication.Utilities.Message("This Entry Already Exists...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If
        End If

        For intRow As Integer = 1 To oMatrix.RowCount
            strcode = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", intRow)
            strcode1 = oApplication.Utilities.getMatrixValues(oMatrix, "V_1", intRow)
            If strcode <> "" And strcode1 = "" Then
                oApplication.Utilities.Message("Level Name can not be empty...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_1").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If

            If strcode = "" And strcode1 <> "" Then
                oApplication.Utilities.Message("Level Code can not be empty...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If
        Next
        Return True
    End Function


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
                    strCode = oMatrix.Columns.Item("V_3").Cells.Item(intRow).Specific.value
                    otestRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oGeneralService = oCompanyService.GetGeneralService("Z_HR_COLVL")
                    oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)
                    otestRs.DoQuery("Update ""@Z_HR_COLVL"" set ""Name""=isnull(""Name"",'') + '_XD' where ""Code""='" & strCode & "'")
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

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_CompLevel Then
                Select Case pVal.BeforeAction
                    Case True
                        If pVal.EventType = SAPbouiCOM.BoEventTypes.et_KEY_DOWN And pVal.CharPressed <> "9" And pVal.ItemUID = "3" And pVal.ColUID = "V_0" Then
                            Dim strVal As String
                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            objMatrix = oForm.Items.Item("3").Specific
                            strVal = oApplication.Utilities.getMatrixValues(objMatrix, "V_0", pVal.Row)
                            If strVal <> "" Then
                                If oApplication.Utilities.ValidateCode(strVal, "COMPLEVEL") = True Then
                                    BubbleEvent = False
                                    Exit Sub
                                End If
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
                        If ((pVal.EventType = SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED) And (pVal.ItemUID = "1")) Then
                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            oForm.Freeze(True)
                            Dim otestrs As SAPbobsCOM.Recordset
                            otestrs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            otestrs.DoQuery("Delete ""@Z_HR_COLVL"" where ""Name"" Like '%_XD'")

                            objForm = oForm
                            objMatrix = objForm.Items.Item("3").Specific
                            objMatrix.AddRow()
                            objMatrix.Columns.Item(0).Cells.Item(objMatrix.RowCount).Specific.value = objMatrix.RowCount
                            objMatrix.Columns.Item("V_0").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_1").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            oCombobox = objMatrix.Columns.Item("V_2").Cells.Item(objMatrix.RowCount).Specific
                            oCombobox.Select("Y", SAPbouiCOM.BoSearchKey.psk_ByValue)
                            oApplication.Utilities.AssignSerialNo(objMatrix, oForm)
                            objMatrix.Columns.Item(1).Cells.Item(objMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                            objForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
                            oForm.Freeze(False)
                        End If
                        If pVal.EventType = SAPbouiCOM.BoEventTypes.et_KEY_DOWN And pVal.CharPressed = "9" And pVal.ItemUID = "3" And pVal.ColUID = "V_0" Then
                            Dim strVal As String
                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            objForm = oForm
                            objMatrix = objForm.Items.Item("3").Specific
                            'For i As Integer = 1 To objMatrix.RowCount
                            '    'If i <> pVal.Row Then
                            '    '    If objMatrix.Columns.Item("V_2").Cells.Item(i).Specific.Value = objMatrix.Columns.Item("V_2").Cells.Item(pVal.Row).Specific.Value Then
                            '    '        oApplication.Utilities.Message("Company Code already exists", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            '    '        objMatrix.Columns.Item("V_2").Cells.Item(pVal.Row).Click(SAPbouiCOM.BoCellClickType.ct_Regular, 1)
                            '    '        BubbleEvent = False
                            '    '        Exit Sub
                            '    '    End If
                            '    'End If
                            'Next
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
                Case mnu_hr_CompLevel
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
                                If oApplication.Utilities.ValidateCode(strValue, "COMPLEVEL") = True Then
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
