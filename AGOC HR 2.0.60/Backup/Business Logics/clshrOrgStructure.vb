Public Class clshrOrgStructure
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
    Private InvBaseDocNo As String
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Public Sub LoadForm()

        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_OrgSt) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If


        oForm = oApplication.Utilities.LoadForm(xml_hr_OrgSt, frm_hr_OrgSt)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.DataSources.UserDataSources.Add("SlNo", SAPbouiCOM.BoDataType.dt_SHORT_NUMBER)
        oForm.Freeze(True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.Settings.Enabled = True
        FillCountry(oForm)
        FillDepartment(oForm)
        FillBranch(oForm)
        AddChooseFromList(oForm)
        databind(oForm)
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        oForm.Freeze(False)
    End Sub
    Private Sub FillDepartment(ByVal aForm As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        Dim oDBDataSrc As SAPbouiCOM.DBDataSource
        Dim oMatrix As SAPbouiCOM.Matrix
        oMatrix = aForm.Items.Item("3").Specific
        ' oDBDataSrc = objForm.DataSources.DBDataSources.Add("@Z_HR_ORGST")
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim oColum As SAPbouiCOM.Column
        oColum = oMatrix.Columns.Item("V_5")
        For intRow As Integer = oColum.ValidValues.Count - 1 To 0 Step -1
            oColum.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oColum.ValidValues.Add("", "")
        oTempRec.DoQuery("Select ""Code"",""Name"" from OUDP order by Code")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oColum.ValidValues.Add(oTempRec.Fields.Item("Code").Value, oTempRec.Fields.Item("Name").Value)
            oTempRec.MoveNext()
        Next
        oColum.DisplayDesc = True

    End Sub

    Private Sub FillBranch(ByVal aForm As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        Dim oDBDataSrc As SAPbouiCOM.DBDataSource
        Dim oMatrix As SAPbouiCOM.Matrix
        oMatrix = aForm.Items.Item("3").Specific
        ' oDBDataSrc = objForm.DataSources.DBDataSources.Add("@Z_HR_ORGST")
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim oColum As SAPbouiCOM.Column
        oColum = oMatrix.Columns.Item("V_23")
        For intRow As Integer = oColum.ValidValues.Count - 1 To 0 Step -1
            oColum.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oColum.ValidValues.Add("", "")
        oTempRec.DoQuery("Select Code,Name From OUBR order by Code")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oColum.ValidValues.Add(oTempRec.Fields.Item("Code").Value, oTempRec.Fields.Item("Name").Value)
            oTempRec.MoveNext()
        Next
        oColum.DisplayDesc = True

    End Sub

    Private Sub FillCountry(ByVal aForm As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        Dim oDBDataSrc As SAPbouiCOM.DBDataSource
        Dim oMatrix As SAPbouiCOM.Matrix
        Dim otestrs As SAPbobsCOM.Recordset
        otestrs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otestrs.DoQuery("Update ""@Z_HR_ORGST"" set ""Name""=' '  where ""Name"" Like '%_XD'")
        oMatrix = aForm.Items.Item("3").Specific
        oDBDataSrc = aForm.DataSources.DBDataSources.Add("@Z_HR_ORGST")
        Try
            oDBDataSrc.Query()
        Catch ex As Exception

        End Try
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim oColum As SAPbouiCOM.Column
        oColum = oMatrix.Columns.Item("V_9")
        For intRow As Integer = oColum.ValidValues.Count - 1 To 0 Step -1
            oColum.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oColum.ValidValues.Add("", "")
        oTempRec.DoQuery("Select Code,Name from OCRY order by Code")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oColum.ValidValues.Add(oTempRec.Fields.Item("Code").Value, oTempRec.Fields.Item("Name").Value)
            oTempRec.MoveNext()
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
                oCombobox = oMatrix.Columns.Item("V_9").Cells.Item(oMatrix.RowCount).Specific
                oCombobox.Select("", SAPbouiCOM.BoSearchKey.psk_ByValue)
                oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            End If
        ElseIf oMatrix.RowCount = 0 Then
            oMatrix.AddRow()
            oMatrix.Columns.Item(0).Cells.Item(oMatrix.RowCount).Specific.Value = oMatrix.RowCount
            oCombobox = oMatrix.Columns.Item("V_9").Cells.Item(oMatrix.RowCount).Specific
            oCombobox.Select("", SAPbouiCOM.BoSearchKey.psk_ByValue)
            oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        End If
        oApplication.Utilities.AssignSerialNo(oMatrix, aForm)
        oMatrix.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub

#Region "Add Choose From List"
    Private Sub databind(ByVal aForm As SAPbouiCOM.Form)
        oMatrix = aForm.Items.Item("3").Specific
        oColumn = oMatrix.Columns.Item("V_1")
        oColumn.ChooseFromListUID = "CFL1"
        oColumn.ChooseFromListAlias = "U_Z_CompCode"
        '  oColumn.ChooseFromListAlias = "U_Z_CompName"

        oMatrix = aForm.Items.Item("3").Specific
        oColumn = oMatrix.Columns.Item("V_3")
        oColumn.ChooseFromListUID = "CFL2"
        oColumn.ChooseFromListAlias = "U_Z_FuncCode"

        oMatrix = aForm.Items.Item("3").Specific
        oColumn = oMatrix.Columns.Item("V_7")
        oColumn.ChooseFromListUID = "CFL3"
        oColumn.ChooseFromListAlias = "U_Z_UnitCode"

        oMatrix = aForm.Items.Item("3").Specific
        oColumn = oMatrix.Columns.Item("V_17")
        oColumn.ChooseFromListUID = "CFL_HR_4"
        oColumn.ChooseFromListAlias = "U_Z_PosCode"

        oMatrix = aForm.Items.Item("3").Specific
        oColumn = oMatrix.Columns.Item("V_10")
        oColumn.ChooseFromListUID = "CFL4"
        oColumn.ChooseFromListAlias = "U_Z_LocCode"

        oMatrix = aForm.Items.Item("3").Specific
        oColumn = oMatrix.Columns.Item("V_21")
        oColumn.ChooseFromListUID = "CFL5"
        oColumn.ChooseFromListAlias = "U_Z_SecCode"

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
            oCFLCreationParams.ObjectType = "Z_HR_ORGST"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)

            'oCons = oCFL.GetConditions()
            'oCon = oCons.Add()
            'oCon.Alias = "U_Z_Status"
            'oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            'oCon.CondVal = "Y"
            'oCFL.SetConditions(oCons)
            'oCon = oCons.Add()
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)

            oCFLCreationParams.ObjectType = "Z_HR_OFCA"
            oCFLCreationParams.UniqueID = "CFL2"
            oCFL = oCFLs.Add(oCFLCreationParams)
            'oCons = oCFL.GetConditions()
            'oCon = oCons.Add()
            'oCon.Alias = "U_Z_Status"
            'oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            'oCon.CondVal = "Y"
            'oCFL.SetConditions(oCons)
            'oCon = oCons.Add()

            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
            oCFLCreationParams.ObjectType = "Z_HR_OUNT"
            oCFLCreationParams.UniqueID = "CFL3"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
            oCFLCreationParams.ObjectType = "Z_HR_OLOC"
            oCFLCreationParams.UniqueID = "CFL4"
            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding 4 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_OPOSIN"
            oCFLCreationParams.UniqueID = "CFL_HR_4"

            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding Conditions to CFL2
            'oCons = oCFL.GetConditions()
            'oCon = oCons.Add()
            'oCon.Alias = "U_Z_CouName"
            'oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            'oCon.CondVal = stCouName
            'oCFL.SetConditions(oCons)
            'oCon = oCons.Add()

            'oCFLCreationParams.ObjectType = "Z_HR_OLOC"
            'oCFLCreationParams.UniqueID = "CFL4"
            'oCFL = oCFLs.Add(oCFLCreationParams)

            ' '' Adding Conditions to CFL2
            'oCons = oCFL.GetConditions()
            'oCon = oCons.Add()
            'oCon.Alias = "U_Z_Status"
            'oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            'oCon.CondVal = "Y"
            'oCFL.SetConditions(oCons)
            'oCon = oCons.Add()



            'oCFLCreationParams.MultiSelection = False
            'oCFLCreationParams.ObjectType = "17"
            'oCFLCreationParams.UniqueID = "CFL4"
            'oCFL = oCFLs.Add(oCFLCreationParams)

            'oCons = oCFL.GetConditions()
            'oCon = oCons.Add()
            'oCon.Alias = "DocStatus"
            'oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            'oCon.CondVal = "O"
            'oCFL.SetConditions(oCons)
            'oCon = oCons.Add()

            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
            oCFLCreationParams.ObjectType = "Z_HR_OSEC"
            oCFLCreationParams.UniqueID = "CFL5"
            oCFL = oCFLs.Add(oCFLCreationParams)

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
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
            If oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Specific.value = "" Then
                oMatrix.DeleteRow(oMatrix.RowCount)
            End If
            If 1 = 1 Then
                oUserTable = ocompany.UserTables.Item("Z_HR_ORGST")
                oDBDSource = oForm.DataSources.DBDataSources.Item("@Z_HR_ORGST")
                ' oMatrix.DeleteRow(oMatrix.RowCount)
                oMatrix.FlushToDataSource()
                For i = 0 To oDBDSource.Size - 1
                    oGeneralService = oCompanyService.GetGeneralService("Z_HR_ORGST")
                    oGeneralData = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData)
                    strCode = oDBDSource.GetValue("Code", i).Trim
                    otestRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)
                    otestRs.DoQuery("SElect * from [@Z_HR_ORGST] where Code='" & strCode & "'")
                    If otestRs.RecordCount > 0 Then
                        oGeneralParams.SetProperty("Code", strCode)
                        oGeneralData = oGeneralService.GetByParams(oGeneralParams)
                        oGeneralData.SetProperty("U_Z_OrgCode", oDBDSource.GetValue("U_Z_OrgCode", i))
                        oGeneralData.SetProperty("U_Z_OrgDesc", oDBDSource.GetValue("U_Z_OrgDesc", i))
                        oGeneralData.SetProperty("U_Z_CompCode", oDBDSource.GetValue("U_Z_CompCode", i))
                        oGeneralData.SetProperty("U_Z_CompName", oDBDSource.GetValue("U_Z_CompName", i))
                        oGeneralData.SetProperty("U_Z_FuncCode", oDBDSource.GetValue("U_Z_FuncCode", i))
                        oGeneralData.SetProperty("U_Z_FuncName", oDBDSource.GetValue("U_Z_FuncName", i))
                        oGeneralData.SetProperty("U_Z_UnitCode", oDBDSource.GetValue("U_Z_UnitCode", i))
                        oGeneralData.SetProperty("U_Z_UnitName", oDBDSource.GetValue("U_Z_UnitName", i))
                        oGeneralData.SetProperty("U_Z_LocCode", oDBDSource.GetValue("U_Z_LocCode", i))
                        oGeneralData.SetProperty("U_Z_LocName", oDBDSource.GetValue("U_Z_LocName", i))
                        oGeneralData.SetProperty("U_Z_DeptCode", oDBDSource.GetValue("U_Z_DeptCode", i))
                        oGeneralData.SetProperty("U_Z_DeptName", oDBDSource.GetValue("U_Z_DeptName", i))
                        oGeneralData.SetProperty("U_Z_PosCode", oDBDSource.GetValue("U_Z_PosCode", i))
                        oGeneralData.SetProperty("U_Z_PosName", oDBDSource.GetValue("U_Z_PosName", i))
                        oGeneralData.SetProperty("U_Z_SecCode", oDBDSource.GetValue("U_Z_SecCode", i))
                        oGeneralData.SetProperty("U_Z_SecName", oDBDSource.GetValue("U_Z_SecName", i))
                        oGeneralData.SetProperty("U_Z_BranCode", oDBDSource.GetValue("U_Z_BranCode", i))
                        oGeneralData.SetProperty("U_Z_BranName", oDBDSource.GetValue("U_Z_BranName", i))

                        'Dim strstatus As String = oDBDSource.GetValue("U_Z_Status", i)
                        'If strstatus = "" Then
                        '    strstatus = "Y"
                        'End If
                        'oGeneralData.SetProperty("U_Z_Status", strstatus.Trim)
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
            oDBDSource = objForm.DataSources.DBDataSources.Item("@Z_HR_ORGST")
            objMatrix = objForm.Items.Item("3").Specific
            objMatrix.FlushToDataSource()
            Dim strCode, strDocEntry As String
            strCode = oApplication.Utilities.getMaxCode("@Z_HR_ORGST", "Code")
            strDocEntry = oApplication.Utilities.getMaxCode("@Z_HR_ORGST", "DocEntry")
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


    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        oMatrix = oForm.Items.Item("3").Specific
        Dim strcode, strcode1, strcode2, strcode3, strcode4, strcode5, strcode6, strcode7 As String
        If oMatrix.RowCount > 1 Then

            strcode = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount)
            strcode1 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount - 1)
            If strcode.ToUpper = strcode1.ToUpper Then
                oApplication.Utilities.Message("This entry already exists", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If
        End If

        For intRow As Integer = 1 To oMatrix.RowCount
            strcode = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", intRow)
            strcode1 = oApplication.Utilities.getMatrixValues(oMatrix, "V_2", intRow)
            strcode2 = oApplication.Utilities.getMatrixValues(oMatrix, "V_3", intRow)
            strcode3 = oApplication.Utilities.getMatrixValues(oMatrix, "V_5", intRow)
            strcode4 = oApplication.Utilities.getMatrixValues(oMatrix, "V_7", intRow)
            strcode5 = oApplication.Utilities.getMatrixValues(oMatrix, "V_9", intRow)
            strcode6 = oApplication.Utilities.getMatrixValues(oMatrix, "V_10", intRow)
            strcode7 = oApplication.Utilities.getMatrixValues(oMatrix, "V_13", intRow)
            If strcode <> "" And strcode7 = "" Then
                oApplication.Utilities.Message("Enter Organization Description...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_13").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If
            If strcode <> "" And strcode1 = "" And strcode2 <> "" And strcode3 <> "" And strcode4 <> "" And strcode5 <> "" And strcode6 <> "" Then
                oApplication.Utilities.Message("Enter Company Name...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_1").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If

            If strcode = "" And strcode1 <> "" And strcode2 <> "" And strcode3 <> "" And strcode4 <> "" And strcode5 <> "" And strcode6 <> "" Then
                oApplication.Utilities.Message("Enter Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If
            If strcode <> "" And strcode1 <> "" And strcode2 = "" And strcode3 <> "" And strcode4 <> "" And strcode5 <> "" And strcode6 <> "" Then
                oApplication.Utilities.Message("Enter Division...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_3").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If
            If strcode <> "" And strcode1 <> "" And strcode2 <> "" And strcode3 = "" And strcode4 <> "" And strcode5 <> "" And strcode6 <> "" Then
                oApplication.Utilities.Message("Enter Department...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_5").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If
            If strcode <> "" And strcode1 <> "" And strcode2 <> "" And strcode3 <> "" And strcode4 = "" And strcode5 <> "" And strcode6 <> "" Then
                oApplication.Utilities.Message("Enter Unit...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_7").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If
            If strcode <> "" And strcode1 <> "" And strcode2 <> "" And strcode3 <> "" And strcode4 <> "" And strcode5 = "" And strcode6 <> "" Then
                ' oApplication.Utilities.Message("Enter Country...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                ' oMatrix.Columns.Item("V_9").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                ' Return False
            End If
            If strcode <> "" And strcode1 <> "" And strcode2 <> "" And strcode3 <> "" And strcode4 <> "" And strcode5 <> "" And strcode6 = "" Then
                oApplication.Utilities.Message("Enter Location Name...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_11").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If
        Next
        Return True
    End Function

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
                    strCode = oMatrix.Columns.Item("V_12").Cells.Item(intRow).Specific.value
                    otestRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oGeneralService = oCompanyService.GetGeneralService("Z_HR_ORGST")
                    oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)
                    otestRs.DoQuery("Update ""@Z_HR_ORGST"" set ""Name""=isnull(""Name"",'') + '_XD' where ""Code""='" & strCode & "'")
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

    Private Sub FillCOmpanyInfo(ByVal aCode As String, ByVal aMatrix As SAPbouiCOM.Matrix, ByVal aRow As Integer, ByVal aform As SAPbouiCOM.Form)
        Dim oTst, otest1 As SAPbobsCOM.Recordset
        Try


            aform.Freeze(True)
            oTst = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTst.DoQuery("Select * from [@Z_HR_OPOSIN] where DocEntry=" & aCode)
            oApplication.Utilities.SetMatrixValues(aMatrix, "V_1", aRow, oTst.Fields.Item("U_Z_CompCode").Value.ToString)
            oApplication.Utilities.SetMatrixValues(aMatrix, "V_2", aRow, oTst.Fields.Item("U_Z_CompName").Value)
            oCombobox = oMatrix.Columns.Item("V_5").Cells.Item(aRow).Specific
            oCombobox.Select(oTst.Fields.Item("U_Z_DeptCode").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
            otest1.DoQuery("SElect  * from OUDP where Code='" & oTst.Fields.Item("U_Z_DeptCode").Value & "'")
            oApplication.Utilities.SetMatrixValues(aMatrix, "V_6", aRow, otest1.Fields.Item("Remarks").Value)

            'oApplication.Utilities.SetMatrixValues(aMatrix, "V_5", aRow, oTst.Fields.Item("U_Z_DeptCode").Value)
            oApplication.Utilities.SetMatrixValues(aMatrix, "V_3", aRow, oTst.Fields.Item("U_Z_DivCode").Value)

            otest1.DoQuery("SElect  * from [@Z_HR_OFCA] where U_Z_FuncCode='" & oTst.Fields.Item("U_Z_DivCode").Value & "'")
            oApplication.Utilities.SetMatrixValues(aMatrix, "V_4", aRow, otest1.Fields.Item("U_Z_FuncName").Value)
            aform.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_OrgSt Then
                Select Case pVal.BeforeAction
                    Case True
                        If pVal.EventType = SAPbouiCOM.BoEventTypes.et_KEY_DOWN And pVal.CharPressed <> "9" And pVal.ItemUID = "3" And pVal.ColUID = "V_0" Then
                            Dim strVal As String
                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            objMatrix = oForm.Items.Item("3").Specific
                            strVal = oApplication.Utilities.getMatrixValues(objMatrix, "V_0", pVal.Row)
                            If strVal <> "" Then
                                If oApplication.Utilities.ValidateCode(strVal, "ORG") = True Then
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
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                If pVal.ItemUID = "3" And pVal.ColUID = "V_0" Then
                                    If oApplication.Utilities.ValidateCode(oApplication.Utilities.getMatrixValues(oMatrix, "V_0", 0), "ORG") = True Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                        End Select

                    Case False
                        If ((pVal.EventType = SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED) And (pVal.ItemUID = "1")) Then
                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            oForm.Freeze(True)
                            Dim otestrs As SAPbobsCOM.Recordset
                            otestrs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            otestrs.DoQuery("Delete ""@Z_HR_ORGST"" where ""Name"" Like '%_XD'")

                            objForm = oForm
                            objMatrix = objForm.Items.Item("3").Specific
                            objMatrix.AddRow()
                            objMatrix.Columns.Item(0).Cells.Item(objMatrix.RowCount).Specific.value = objMatrix.RowCount
                            objMatrix.Columns.Item("V_0").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_1").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_2").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_3").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_4").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            'objMatrix.Columns.Item("V_5").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_6").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_7").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_8").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_10").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_11").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_12").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_13").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_17").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_18").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_21").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_22").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            'objMatrix.Columns.Item("V_18").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_24").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            oCombobox = objMatrix.Columns.Item("V_9").Cells.Item(objMatrix.RowCount).Specific
                            oCombobox.Select("", SAPbouiCOM.BoSearchKey.psk_ByValue)
                            oCombobox = objMatrix.Columns.Item("V_5").Cells.Item(objMatrix.RowCount).Specific
                            oCombobox.Select("", SAPbouiCOM.BoSearchKey.psk_ByValue)
                            oApplication.Utilities.AssignSerialNo(objMatrix, oForm)
                            objMatrix.Columns.Item(1).Cells.Item(objMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                            objForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
                            oForm.Freeze(False)
                        End If

                        If pVal.EventType = SAPbouiCOM.BoEventTypes.et_COMBO_SELECT Then
                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            oMatrix = oForm.Items.Item("3").Specific
                            If pVal.ItemUID = "3" And pVal.ColUID = "V_5" Then
                                Dim stCode, stCode1 As String
                                oCombobox = oMatrix.Columns.Item("V_5").Cells.Item(pVal.Row).Specific
                                stCode1 = oCombobox.Selected.Value
                                stCode = oCombobox.Selected.Description
                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_6", pVal.Row, stCode)
                            ElseIf pVal.ItemUID = "3" And pVal.ColUID = "V_23" Then
                                Dim stCode, stCode1 As String
                                oCombobox = oMatrix.Columns.Item("V_23").Cells.Item(pVal.Row).Specific
                                stCode1 = oCombobox.Selected.Value
                                stCode = oCombobox.Selected.Description
                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_24", pVal.Row, stCode)
                            End If
                        End If
                        If pVal.EventType = SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST Then
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
                                    If pVal.ItemUID = "3" And pVal.ColUID = "V_1" Then
                                        val1 = oDataTable.GetValue("U_Z_CompCode", 0)
                                        val = oDataTable.GetValue("U_Z_CompName", 0)
                                        oMatrix = oForm.Items.Item("3").Specific
                                        Try
                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", pVal.Row, val)
                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val1)
                                        Catch ex As Exception
                                            ' oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val1)

                                        End Try

                                    End If

                                    If pVal.ItemUID = "3" And pVal.ColUID = "V_17" Then
                                        val1 = oDataTable.GetValue("U_Z_PosCode", 0)
                                        val = oDataTable.GetValue("U_Z_PosName", 0)
                                        oMatrix = oForm.Items.Item("3").Specific
                                        Try
                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_18", pVal.Row, val)
                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_17", pVal.Row, val1)
                                        Catch ex As Exception
                                            ' oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val1)
                                        End Try
                                        FillCOmpanyInfo(oDataTable.GetValue("DocEntry", 0).ToString, oMatrix, pVal.Row, oForm)


                                    End If
                                    If pVal.ItemUID = "3" And pVal.ColUID = "V_3" Then
                                        val = oDataTable.GetValue("U_Z_FuncName", 0)
                                        val1 = oDataTable.GetValue("U_Z_FuncCode", 0)
                                        oMatrix = oForm.Items.Item("3").Specific
                                        Try

                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_4", pVal.Row, val)
                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_3", pVal.Row, val1)
                                        Catch ex As Exception
                                            ' oApplication.Utilities.SetMatrixValues(oMatrix, "V_3", pVal.Row, val1)

                                        End Try
                                    End If
                                    If pVal.ItemUID = "3" And pVal.ColUID = "V_7" Then
                                        val1 = oDataTable.GetValue("U_Z_UnitCode", 0)
                                        val = oDataTable.GetValue("U_Z_UnitName", 0)
                                        oMatrix = oForm.Items.Item("3").Specific
                                        Try
                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_8", pVal.Row, val)
                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_7", pVal.Row, val1)

                                        Catch ex As Exception
                                            'oApplication.Utilities.SetMatrixValues(oMatrix, "V_7", pVal.Row, val1)
                                        End Try
                                    End If
                                    If pVal.ItemUID = "3" And pVal.ColUID = "V_10" Then
                                        Dim strval As String
                                        val1 = oDataTable.GetValue("U_Z_LocCode", 0)
                                        val = oDataTable.GetValue("U_Z_LocName", 0)
                                        val2 = oDataTable.GetValue("U_Z_CouName", 0)
                                        oMatrix = oForm.Items.Item("3").Specific
                                        oCombobox = oMatrix.Columns.Item("V_9").Cells.Item(pVal.Row).Specific
                                        oCombobox.Select(val2, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                        Try

                                            ' oApplication.Utilities.SetMatrixValues(oMatrix, "V_9", pVal.Row, strval)
                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_11", pVal.Row, val)
                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_10", pVal.Row, val1)
                                        Catch ex As Exception
                                            'oApplication.Utilities.SetMatrixValues(oMatrix, "V_7", pVal.Row, val1)
                                        End Try
                                    End If

                                    'If pVal.ItemUID = "3" And pVal.ColUID = "V_6" Then
                                    '    val = oDataTable.GetValue("U_Z_LocName", 0)
                                    '    oMatrix = oForm.Items.Item("3").Specific
                                    '    oApplication.Utilities.SetMatrixValues(oMatrix, "V_6", pVal.Row, val)
                                    'End If
                                    If pVal.ItemUID = "3" And pVal.ColUID = "V_21" Then
                                        Dim strval As String
                                        val = oDataTable.GetValue("U_Z_SecCode", 0)
                                        val1 = oDataTable.GetValue("U_Z_SecName", 0)
                                        oMatrix = oForm.Items.Item("3").Specific
                                        'oCombobox = oMatrix.Columns.Item("V_9").Cells.Item(pVal.Row).Specific
                                        'oCombobox.Select(val2, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                        Try
                                            ' oApplication.Utilities.SetMatrixValues(oMatrix, "V_9", pVal.Row, strval)
                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_22", pVal.Row, val1)
                                            oApplication.Utilities.SetMatrixValues(oMatrix, "V_21", pVal.Row, val)
                                        Catch ex As Exception
                                            'oApplication.Utilities.SetMatrixValues(oMatrix, "V_7", pVal.Row, val1)
                                        End Try
                                    End If
                                    oForm.Freeze(False)
                                End If
                            Catch ex As Exception
                                If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                End If
                                oForm.Freeze(False)
                            End Try
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
                Case mnu_hr_OrgSt
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
                                If oApplication.Utilities.ValidateCode(strValue, "ORG") = True Then
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
