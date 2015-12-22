Public Class clshrLoginSetup
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix, objMatrix As SAPbouiCOM.Matrix
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
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_LogSetup) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_Logsetup, frm_hr_LogSetup)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.DataSources.UserDataSources.Add("SlNo", SAPbouiCOM.BoDataType.dt_SHORT_NUMBER)
        oForm.Freeze(True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.Settings.Enabled = True
      
        BindData(oForm)
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
            otestrs.DoQuery("Update ""@Z_HR_LOGIN"" set ""Name""=' '  where ""Name"" Like '%_XD'")
            oMatrix = objform.Items.Item("3").Specific
            oDBDataSrc = objform.DataSources.DBDataSources.Add("@Z_HR_LOGIN")
            Try
                oDBDataSrc.Query()
            Catch ex As Exception

            End Try
            'Dim oColumn As SAPbouiCOM.Column
            'oColumn = oMatrix.Columns.Item("V_4")
            'oColumn.IsPassword = True
            'oColum.ValidValues.Add("Y", "Yes")
            'oColum.ValidValues.Add("N", "No")
            'oColum.DisplayDesc = True
            Dim oColum As SAPbouiCOM.Column
            oColum = oMatrix.Columns.Item("SlNo")
            oColum.DataBind.SetBound(True, "", "SlNo")
            oMatrix.LoadFromDataSource()
          
            If oMatrix.RowCount >= 1 Then
                If oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Specific.Value <> "" Then
                    oDBDataSrc.Clear()
                    oMatrix.AddRow()
                    oMatrix.Columns.Item(0).Cells.Item(oMatrix.RowCount).Specific.Value = oMatrix.RowCount
                    oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                End If
            ElseIf oMatrix.RowCount = 0 Then
                oMatrix.AddRow()
                oMatrix.Columns.Item(0).Cells.Item(oMatrix.RowCount).Specific.Value = oMatrix.RowCount
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
            oUserTable = ocompany.UserTables.Item("Z_HR_LOGIN")
            oDBDSource = oForm.DataSources.DBDataSources.Item("@Z_HR_LOGIN")
            ' oMatrix.DeleteRow(oMatrix.RowCount)
            oMatrix.FlushToDataSource()
            For i = 0 To oDBDSource.Size - 1
                oGeneralService = oCompanyService.GetGeneralService("Z_HR_LOGIN")
                oGeneralData = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData)
                strCode = oDBDSource.GetValue("Code", i).Trim
                otestRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)
                otestRs.DoQuery("SElect * from [@Z_HR_LOGIN] where Code='" & strCode & "'")
                If otestRs.RecordCount > 0 Then
                    oGeneralParams.SetProperty("Code", strCode)
                    oGeneralData = oGeneralService.GetByParams(oGeneralParams)
                    oGeneralData.SetProperty("U_Z_UID", oDBDSource.GetValue("U_Z_UID", i))
                    oGeneralData.SetProperty("U_Z_PWD", oDBDSource.GetValue("U_Z_PWD", i))
                    oGeneralData.SetProperty("U_Z_EMPID", oDBDSource.GetValue("U_Z_EMPID", i))
                    oGeneralData.SetProperty("U_Z_EMPNAME", oDBDSource.GetValue("U_Z_EMPNAME", i))

                    If oDBDSource.GetValue("U_Z_ESSAPPROVER", i) = "" Then
                        oGeneralData.SetProperty("U_Z_ESSAPPROVER", "E")
                    Else
                        oGeneralData.SetProperty("U_Z_ESSAPPROVER", oDBDSource.GetValue("U_Z_ESSAPPROVER", i).Trim)
                    End If
                    If oDBDSource.GetValue("U_Z_SUPERUSER", i) = "" Then
                        oGeneralData.SetProperty("U_Z_SUPERUSER", "N")
                    Else
                        oGeneralData.SetProperty("U_Z_SUPERUSER", oDBDSource.GetValue("U_Z_SUPERUSER", i).Trim)
                    End If
                    If oDBDSource.GetValue("U_Z_APPROVER", i) = "" Then
                        oGeneralData.SetProperty("U_Z_APPROVER", "N")
                    Else
                        oGeneralData.SetProperty("U_Z_APPROVER", oDBDSource.GetValue("U_Z_APPROVER", i).Trim)
                    End If
                    If oDBDSource.GetValue("U_Z_MGRAPPROVER", i) = "" Then
                        oGeneralData.SetProperty("U_Z_APPROVER", "N")
                    Else
                        oGeneralData.SetProperty("U_Z_MGRAPPROVER", oDBDSource.GetValue("U_Z_MGRAPPROVER", i).Trim)
                    End If
                    If oDBDSource.GetValue("U_Z_HRAPPROVER", i) = "" Then
                        oGeneralData.SetProperty("U_Z_HRAPPROVER", "N")
                    Else
                        oGeneralData.SetProperty("U_Z_HRAPPROVER", oDBDSource.GetValue("U_Z_HRAPPROVER", i).Trim)
                    End If



                    If oDBDSource.GetValue("U_Z_MGRREQUEST", i) = "" Then
                        oGeneralData.SetProperty("U_Z_MGRREQUEST", "N")
                    Else
                        oGeneralData.SetProperty("U_Z_MGRREQUEST", oDBDSource.GetValue("U_Z_MGRREQUEST", i).Trim)
                    End If
                    If oDBDSource.GetValue("U_Z_HRRECAPPROVER", i) = "" Then
                        oGeneralData.SetProperty("U_Z_HRRECAPPROVER", "N")
                    Else
                        oGeneralData.SetProperty("U_Z_HRRECAPPROVER", oDBDSource.GetValue("U_Z_HRRECAPPROVER", i).Trim)
                    End If
                    If oDBDSource.GetValue("U_Z_GMRECAPPROVER", i) = "" Then
                        oGeneralData.SetProperty("U_Z_GMRECAPPROVER", "N")
                    Else
                        oGeneralData.SetProperty("U_Z_GMRECAPPROVER", oDBDSource.GetValue("U_Z_GMRECAPPROVER", i).Trim)
                    End If

                    blnRecordExists = True
                Else
                    oGeneralData.SetProperty("Code", strCode)
                    blnRecordExists = False
                End If

                If blnRecordExists = True Then
                    oGeneralService.Update(oGeneralData)
                Else
                    ' oGeneralService.Add(oGeneralData)
                End If
            Next
            oDBDSource.Query()
            oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            '  End If
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
            aForm.Freeze(True)
            oDBDSource = aForm.DataSources.DBDataSources.Item("@Z_HR_LOGIN")
            objMatrix = aForm.Items.Item("3").Specific
            objMatrix.FlushToDataSource()
            Dim strCode, strDocEntry As String
            strCode = oApplication.Utilities.getMaxCode("@Z_HR_LOGIN", "Code")
            strDocEntry = oApplication.Utilities.getMaxCode("@Z_HR_LOGIN", "DocEntry")

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
                    strCode = oMatrix.Columns.Item("V_6").Cells.Item(intRow).Specific.value
                    otestRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oGeneralService = oCompanyService.GetGeneralService("Z_HR_LOGIN")
                    oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)
                    otestRs.DoQuery("Update ""@Z_HR_LOGIN"" set ""Name""=isnull(""Name"",'') + '_XD' where ""Code""='" & strCode & "'")
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

#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_LogSetup Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" And pVal.ColUID = "V_0" And pVal.CharPressed <> 9 Then
                                    objMatrix = oForm.Items.Item("3").Specific
                                    Dim strValue As String
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1" And oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                    oForm.Freeze(True)
                                    InsertCodeAndDocEntry(oForm)
                                    EnblMatrixAfterUpdate(oApplication.SBO_Application, oApplication.Company, oForm)
                                    oForm.Freeze(False)
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1" Then
                                    oForm.Freeze(True)
                                    Dim otestrs As SAPbobsCOM.Recordset
                                    otestrs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    otestrs.DoQuery("Delete ""@Z_HR_LOGIN"" where ""Name"" Like '%_XD'")

                                    objMatrix = oForm.Items.Item("3").Specific
                                    objMatrix.AddRow()
                                    objMatrix.Columns.Item(0).Cells.Item(objMatrix.RowCount).Specific.value = objMatrix.RowCount
                                    objMatrix.Columns.Item(1).Cells.Item(objMatrix.RowCount).Specific.value = ""
                                    objMatrix.Columns.Item(2).Cells.Item(objMatrix.RowCount).Specific.value = ""
                                    objMatrix.Columns.Item(3).Cells.Item(objMatrix.RowCount).Specific.value = ""
                                    objMatrix.Columns.Item(4).Cells.Item(objMatrix.RowCount).Specific.value = ""
                                    oApplication.Utilities.AssignSerialNo(objMatrix, oForm)
                                    objMatrix.Columns.Item(1).Cells.Item(objMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
                                    oForm.Freeze(False)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1 As String
                                Dim sCHFL_ID, val As String
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
                                        If pVal.ItemUID = "3" And pVal.ColUID = "V_3" Then
                                            val = oDataTable.GetValue("empID", 0)
                                            val1 = oDataTable.GetValue("firstName", 0)
                                            oMatrix = oForm.Items.Item("3").Specific
                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_8", pVal.Row, val1)
                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_3", pVal.Row, val)
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
                Case mnu_hr_Logsetup
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
                                strValue = oApplication.Utilities.getMatrixValues(objMatrix, "V_5", intRow)
                                If strValue <> "" Then
                                    DeleteRow(oForm, intRow)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                'If oApplication.Utilities.ValidateCode(strValue, "LOGIN") = True Then
                                '    BubbleEvent = False
                                '    Exit Sub
                                'Else
                                '    DeleteRow(oForm, intRow)
                                '    BubbleEvent = False
                                '    Exit Sub
                                'End If
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
