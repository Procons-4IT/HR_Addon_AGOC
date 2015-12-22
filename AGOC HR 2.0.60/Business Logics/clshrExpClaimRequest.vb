Imports System.IO
Public Class clshrExpClaimRequest
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox, oCombobox1, oCombobox2, oCombobox3 As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oComboColumn, oComboColumn1 As SAPbouiCOM.ComboBoxColumn
    Private ocombo, ocombo1, ocombo2, ocombo3 As SAPbouiCOM.ComboBoxColumn
    Private oGrid, oAttGrid, oGrid1, oGrid2 As SAPbouiCOM.Grid
    Private oStatic As SAPbouiCOM.StaticText
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo, strqry1, strFilepath As String
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Dim oRecSet As SAPbobsCOM.Recordset
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Public Sub LoadForm(ByVal EmpCode As String, ByVal EmpName As String)
        'If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_ExitfrmInit) = False Then
        '    oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        '    Exit Sub
        'End If
        ExpintTempID = oApplication.Utilities.CheckApprovalExists(HeaderDoctype.ExpCli, EmpCode)
        If ExpintTempID = "0" Then
            oApplication.Utilities.Message("Approval template is not defined . You can't create request. please Contact the Administrator...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If

        oForm = oApplication.Utilities.LoadForm(xml_hr_ExpenseClaim, frm_hr_ExpenseClaim)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.EnableMenu(mnu_ADD, False)
        oForm.EnableMenu(mnu_FIND, False)
        AddChooseFromList(oForm)
        oForm.DataSources.UserDataSources.Add("subdt", SAPbouiCOM.BoDataType.dt_DATE)
        oApplication.Utilities.setUserDatabind(oForm, "10", "subdt")
        oForm.DataSources.UserDataSources.Add("empid", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "15", "empid")
        oForm.DataSources.UserDataSources.Add("empname", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "4", "empname")
        oForm.DataSources.UserDataSources.Add("DocNo", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "23", "DocNo")
        oForm.DataSources.UserDataSources.Add("Client", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "19", "Client")
        oForm.DataSources.UserDataSources.Add("Project", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "21", "Project")
        oForm.DataSources.UserDataSources.Add("TANo", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "1000001", "TANo")
        oForm.DataSources.UserDataSources.Add("BussCode", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "37", "BussCode")
        oForm.DataSources.UserDataSources.Add("TrpType", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDSCombobox(oForm, "30", "TrpType")
        oForm.DataSources.UserDataSources.Add("TrpCode", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDSCombobox(oForm, "32", "TrpCode")
        oForm.DataSources.UserDataSources.Add("DocStatus", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDSCombobox(oForm, "34", "DocStatus")

        oCombobox = oForm.Items.Item("34").Specific
        oCombobox.ValidValues.Add("O", "Opened")
        oCombobox.ValidValues.Add("C", "Closed")
        oCombobox.Select("O", SAPbouiCOM.BoSearchKey.psk_ByValue)
        oForm.Items.Item("34").DisplayDesc = True

        oCombobox = oForm.Items.Item("30").Specific
        oCombobox.ValidValues.Add("", "")
        oCombobox.ValidValues.Add("N", "Without Travel")
        oCombobox.ValidValues.Add("E", "With Travel")
        oForm.Items.Item("30").DisplayDesc = True

        FillTravel(oForm, EmpCode)

        oForm.Items.Item("15").Enabled = False
        oForm.Items.Item("4").Enabled = False
        'oForm.Items.Item("10").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        ' oForm.Items.Item("11").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
        oApplication.Utilities.setEdittextvalue(oForm, "15", EmpCode)
        oApplication.Utilities.setEdittextvalue(oForm, "4", EmpName)
        oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRecSet.DoQuery("Select * from OHEM where empID=" & EmpCode)
        Dim aCode As String = oRecSet.Fields.Item("U_Z_EmpID").Value.ToString()
        Dim BussCode As String = oRecSet.Fields.Item("U_Z_CardCode").Value.ToString()
        oApplication.Utilities.setEdittextvalue(oForm, "1000001", aCode)
        oApplication.Utilities.setEdittextvalue(oForm, "37", BussCode)
        AddMode(oForm)
        oForm.Items.Item("19").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oForm.PaneLevel = 1
        Gridbind()
        oForm.Freeze(False)
    End Sub
    Public Sub LoadForm1(ByVal strCode As String)
        'If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_ExitfrmInit) = False Then
        '    oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        '    Exit Sub
        'End If
        Try
            oForm = oApplication.Utilities.LoadForm(xml_hr_ExpenseClaim, frm_hr_ExpenseClaim)
            oForm = oApplication.SBO_Application.Forms.ActiveForm()
            oForm.Freeze(True)
            oForm.EnableMenu(mnu_ADD, False)
            oForm.EnableMenu(mnu_FIND, False)
            AddChooseFromList(oForm)
            oForm.DataSources.UserDataSources.Add("subdt", SAPbouiCOM.BoDataType.dt_DATE)
            oApplication.Utilities.setUserDatabind(oForm, "10", "subdt")
            oForm.DataSources.UserDataSources.Add("empid", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oApplication.Utilities.setUserDatabind(oForm, "15", "empid")
            oForm.DataSources.UserDataSources.Add("empname", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oApplication.Utilities.setUserDatabind(oForm, "4", "empname")
            oForm.DataSources.UserDataSources.Add("DocNo", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oApplication.Utilities.setUserDatabind(oForm, "23", "DocNo")
            oForm.DataSources.UserDataSources.Add("Client", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oApplication.Utilities.setUserDatabind(oForm, "19", "Client")
            oForm.DataSources.UserDataSources.Add("Project", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oApplication.Utilities.setUserDatabind(oForm, "21", "Project")
            oForm.DataSources.UserDataSources.Add("TANo", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oApplication.Utilities.setUserDatabind(oForm, "1000001", "TANo")
            oForm.DataSources.UserDataSources.Add("BussCode", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oApplication.Utilities.setUserDatabind(oForm, "37", "BussCode")
            oForm.DataSources.UserDataSources.Add("TrpType", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oApplication.Utilities.setUserDSCombobox(oForm, "30", "TrpType")
            oForm.DataSources.UserDataSources.Add("TrpCode", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oApplication.Utilities.setUserDSCombobox(oForm, "32", "TrpCode")
            oForm.DataSources.UserDataSources.Add("DocStatus", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oApplication.Utilities.setUserDSCombobox(oForm, "34", "DocStatus")
            oCombobox2 = oForm.Items.Item("34").Specific
            oCombobox2.ValidValues.Add("O", "Opened")
            oCombobox2.ValidValues.Add("C", "Closed")
            oForm.Items.Item("34").DisplayDesc = True

            oCombobox3 = oForm.Items.Item("30").Specific
            oCombobox3.ValidValues.Add("N", "Without Travel")
            oCombobox3.ValidValues.Add("E", "With Travel")
            oForm.Items.Item("30").DisplayDesc = True

            oCombobox1 = oForm.Items.Item("32").Specific

            oForm.Items.Item("15").Enabled = False
            oForm.Items.Item("4").Enabled = False
            oApplication.Utilities.setEdittextvalue(oForm, "23", strCode)
            ' PopulateDetails(strCode)
            oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRecSet.DoQuery("Select U_Z_EmpID,U_Z_EmpName,U_Z_Subdt,U_Z_Client,U_Z_Project,U_Z_TAEmpID,U_Z_TraDesc,U_Z_TraCode,isnull(U_Z_CardCode,'') as U_Z_CardCode,isnull(U_Z_DocStatus,'O') AS 'U_Z_DocStatus',isnull(U_Z_TripType,'N') as 'U_Z_TripType' from ""@Z_HR_OEXPCL"" where Code='" & strCode & "'")
            If oRecSet.RecordCount > 0 Then
                oApplication.Utilities.setEdittextvalue(oForm, "15", oRecSet.Fields.Item("U_Z_EmpID").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "4", oRecSet.Fields.Item("U_Z_EmpName").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "10", oRecSet.Fields.Item("U_Z_Subdt").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "19", oRecSet.Fields.Item("U_Z_Client").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "21", oRecSet.Fields.Item("U_Z_Project").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "1000001", oRecSet.Fields.Item("U_Z_TAEmpID").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "37", oRecSet.Fields.Item("U_Z_CardCode").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "36", oRecSet.Fields.Item("U_Z_TraDesc").Value)
                FillTravel(oForm, oRecSet.Fields.Item("U_Z_EmpID").Value)

                oCombobox1.Select(oRecSet.Fields.Item("U_Z_TraCode").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
                oCombobox2.Select(oRecSet.Fields.Item("U_Z_DocStatus").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
                If oRecSet.Fields.Item("U_Z_DocStatus").Value = "C" Then
                    'oForm.Items.Item("13").Visible = False
                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_VIEW_MODE
                Else
                    '   oForm.Items.Item("13").Visible = True
                End If

                Dim strValue As String = oRecSet.Fields.Item("U_Z_TripType").Value
                If strValue = "N" Then
                    oForm.Items.Item("19").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    oForm.Items.Item("32").Enabled = False
                Else
                    oForm.Items.Item("32").Enabled = True
                End If
                'oCombobox.Select(strValue, SAPbouiCOM.BoSearchKey.psk_ByValue)
                oCombobox3.Select(strValue, SAPbouiCOM.BoSearchKey.psk_ByValue)
                oForm.Items.Item("19").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            End If

            oForm.PaneLevel = 1
            Gridbind()
            oForm.Items.Item("23").Enabled = False
            oForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
    Private Sub FillTravel(ByVal sform As SAPbouiCOM.Form, ByVal EmpId As String)
        oCombobox = sform.Items.Item("32").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            Try
                oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
            Catch ex As Exception
            End Try
        Next
        ' strqry1 = "SELECT distinct(""U_Z_TraCode""),""U_Z_TraName"" from [@Z_HR_OTRAREQ]  where ""U_Z_AppStatus""='A' and ""U_Z_EmpId""='" & EmpId & "'"
        strqry1 = "SELECT (""DocEntry""),""U_Z_TraName"" from [@Z_HR_OTRAREQ]  where ""U_Z_AppStatus""='A' and ""U_Z_EmpId""='" & EmpId & "'"
        oSlpRS.DoQuery(strqry1)
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            Try
                oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            Catch ex As Exception
            End Try
            oSlpRS.MoveNext()
        Next
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        sform.Items.Item("32").DisplayDesc = True
    End Sub
    Private Sub PopulateDetails(ByVal aCode As String)
        Try
            oCombobox = oForm.Items.Item("30").Specific
          

            oCombobox1 = oForm.Items.Item("32").Specific
            oCombobox2 = oForm.Items.Item("34").Specific
            oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRecSet.DoQuery("Select U_Z_EmpID,U_Z_EmpName,U_Z_Subdt,U_Z_Client,U_Z_Project,U_Z_TAEmpID,U_Z_TraDesc,U_Z_TraCode,isnull(U_Z_DocStatus,'O') AS 'U_Z_DocStatus',isnull(U_Z_TripType,'N') as 'U_Z_TripType' from ""@Z_HR_OEXPCL"" where Code='" & aCode & "'")
            If oRecSet.RecordCount > 0 Then
                oApplication.Utilities.setEdittextvalue(oForm, "15", oRecSet.Fields.Item("U_Z_EmpID").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "4", oRecSet.Fields.Item("U_Z_EmpName").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "10", oRecSet.Fields.Item("U_Z_Subdt").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "19", oRecSet.Fields.Item("U_Z_Client").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "21", oRecSet.Fields.Item("U_Z_Project").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "1000001", oRecSet.Fields.Item("U_Z_TAEmpID").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "36", oRecSet.Fields.Item("U_Z_TraDesc").Value)
                FillTravel(oForm, oRecSet.Fields.Item("U_Z_EmpID").Value)
            
                oCombobox1.Select(oRecSet.Fields.Item("U_Z_TraCode").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
                oCombobox2.Select(oRecSet.Fields.Item("U_Z_DocStatus").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
                If oRecSet.Fields.Item("U_Z_DocStatus").Value = "C" Then
                    oForm.Items.Item("13").Visible = False
                Else
                    oForm.Items.Item("13").Visible = True
                End If

                Dim strValue As String = oRecSet.Fields.Item("U_Z_TripType").Value
                If strValue = "N" Then
                    oForm.Items.Item("32").Enabled = False
                Else
                    oForm.Items.Item("32").Enabled = True
                End If
                oCombobox.Select(oRecSet.Fields.Item("U_Z_TripType").Value.ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue)
              
                oForm.Items.Item("19").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
    Private Sub reDrawForm(ByVal oForm As SAPbouiCOM.Form)
        Try
            oForm.Freeze(True)
            oForm.Items.Item("26").Width = oForm.Width - 30
            oForm.Items.Item("26").Height = oForm.Items.Item("26").Height + 95
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
        End Try
    End Sub
#Region "Add Choose From List"
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
            'oCFLCreationParams.MultiSelection = False
            'oCFLCreationParams.ObjectType = "Z_HR_OTRAPLA"
            'oCFLCreationParams.UniqueID = "CFL1"
            'oCFL = oCFLs.Add(oCFLCreationParams)

            'oCons = oCFL.GetConditions()
            'oCon = oCons.Add()
            'oCon.Alias = "U_Z_AppStatus"
            'oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            'oCon.CondVal = "A"
            'oCFL.SetConditions(oCons)
            'oCon = oCons.Add()

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_EXPANCES"
            oCFLCreationParams.UniqueID = "CFL2"
            oCFL = oCFLs.Add(oCFLCreationParams)

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

#End Region
    Private Sub formatGrid(ByVal aForm As SAPbouiCOM.Form, ByVal aChoice As String, ByVal gridid As String)
        Dim strQuery As String
        Dim oGECol As SAPbouiCOM.EditTextColumn
        If aChoice = "Request" Then
            oGrid = aForm.Items.Item("12").Specific
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("Name").Visible = False
            oGrid.Columns.Item("U_Z_OverLap").Visible = False
            oGrid.Columns.Item("U_Z_Subdt").TitleObject.Caption = "Submitted Date"
            oGrid.Columns.Item("U_Z_Subdt").Visible = False
            oGrid.Columns.Item("U_Z_Claimdt").TitleObject.Caption = "Transaction Date (*)"
            oGrid.Columns.Item("U_Z_Client").Visible = False
            oGrid.Columns.Item("U_Z_Project").Visible = False
            oGrid.Columns.Item("U_Z_TripType").TitleObject.Caption = "Trip Type (*)"
            oGrid.Columns.Item("U_Z_TripType").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oComboColumn = oGrid.Columns.Item("U_Z_TripType")
            oComboColumn.ValidValues.Add("", "")
            oComboColumn.ValidValues.Add("E", "Existing")
            oComboColumn.ValidValues.Add("N", "New")
            oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_TripType").Visible = False
            oGrid.Columns.Item("U_Z_TraCode").TitleObject.Caption = "Travel Code"
            oEditTextColumn = oGrid.Columns.Item("U_Z_TraCode")
            'oEditTextColumn.ChooseFromListUID = "CFL1"
            'oEditTextColumn.ChooseFromListAlias = "U_Z_TraCode"
            oEditTextColumn.LinkedObjectType = "Z_HR_OTRAPLA"
            oGrid.Columns.Item("U_Z_TraCode").Visible = False
            oGrid.Columns.Item("U_Z_TraDesc").TitleObject.Caption = "Travel Description (*)"
            oGrid.Columns.Item("U_Z_TraDesc").Visible = False
            oGrid.Columns.Item("U_Z_City").TitleObject.Caption = "City"
            oGrid.Columns.Item("U_Z_AlloCode").TitleObject.Caption = "Allowance Code"
            oGrid.Columns.Item("U_Z_AlloCode").Editable = False
            oGrid.Columns.Item("U_Z_Currency").TitleObject.Caption = "Transaction Currency (*)"
            oGrid.Columns.Item("U_Z_Currency").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo2 = oGrid.Columns.Item("U_Z_Currency")
            oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strQuery = "SELECT ""CurrCode"" As ""Code"", ""CurrName"" As ""Name"" FROM ""OCRN"""
            oRecSet.DoQuery(strQuery)
            ocombo2.ValidValues.Add("", "")
            If Not oRecSet.EoF Then
                For index As Integer = 0 To oRecSet.RecordCount - 1
                    If Not oRecSet.EoF Then
                        ocombo2.ValidValues.Add(oRecSet.Fields.Item("Code").Value, oRecSet.Fields.Item("Name").Value)
                        oRecSet.MoveNext()
                    End If
                Next
            End If
            ocombo2.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
            oGrid.Columns.Item("U_Z_CurAmt").TitleObject.Caption = "Transaction Amount (*)"
            oGrid.Columns.Item("U_Z_ExcRate").TitleObject.Caption = "Exchange Rate"
            oGrid.Columns.Item("U_Z_UsdAmt").TitleObject.Caption = "Local Currency Amount"
            oGrid.Columns.Item("U_Z_UsdAmt").Editable = False
            oGrid.Columns.Item("U_Z_Reimburse").TitleObject.Caption = "To be Reimbursed?"
            oGrid.Columns.Item("U_Z_Reimburse").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_Reimburse")
            ocombo.ValidValues.Add("", "")
            ocombo.ValidValues.Add("N", "No")
            ocombo.ValidValues.Add("Y", "Yes")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_ReimAmt").TitleObject.Caption = "Reimbursement Amount"
            oGrid.Columns.Item("U_Z_ReimAmt").Editable = False
            oGrid.Columns.Item("U_Z_ExpType").TitleObject.Caption = "Expense Type (*)"
            oEditTextColumn = oGrid.Columns.Item("U_Z_ExpType")
            oEditTextColumn.ChooseFromListUID = "CFL2"
            oEditTextColumn.ChooseFromListAlias = "Code"
            oEditTextColumn.LinkedObjectType = "Z_HR_EXPANCES"
            oGrid.Columns.Item("U_Z_ExpCode").TitleObject.Caption = "Expenses Code"
            oGrid.Columns.Item("U_Z_ExpCode").Visible = False

            oGrid.Columns.Item("U_Z_PayMethod").TitleObject.Caption = "Payment Method"
            oGrid.Columns.Item("U_Z_PayMethod").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo1 = oGrid.Columns.Item("U_Z_PayMethod")
            oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strQuery = "SELECT ""Code"" As ""Code"", ""U_Z_PayMethod"" As ""Name"" FROM ""@Z_HR_PAYMD"""
            oRecSet.DoQuery(strQuery)
            ocombo1.ValidValues.Add("", "")
            If Not oRecSet.EoF Then
                For index As Integer = 0 To oRecSet.RecordCount - 1
                    If Not oRecSet.EoF Then
                        ocombo1.ValidValues.Add(oRecSet.Fields.Item("Code").Value, oRecSet.Fields.Item("Name").Value)
                        oRecSet.MoveNext()
                    End If
                Next
            End If
            ocombo1.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
            oGrid.Columns.Item("U_Z_Notes").TitleObject.Caption = "Notes"
            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Status"
            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo3 = oGrid.Columns.Item("U_Z_AppStatus")
            ocombo3.ValidValues.Add("P", "Pending")
            ocombo3.ValidValues.Add("R", "Rejected")
            ocombo3.ValidValues.Add("A", "Approved")
            ocombo3.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_AppStatus").Editable = False

            oGrid.Columns.Item("U_Z_Attachment").TitleObject.Caption = "Attachments(Double click to Select Attachment)"
            oGECol = oGrid.Columns.Item("U_Z_Attachment")
            oGECol.LinkedObjectType = "Z_HR_OEXFOM"
            oGrid.Columns.Item("U_Z_Attachment").Editable = False
            oGrid.Columns.Item("U_Z_CurApprover").TitleObject.Caption = "Current Approver"
            oGrid.Columns.Item("U_Z_CurApprover").Editable = False
            oGrid.Columns.Item("U_Z_NxtApprover").TitleObject.Caption = "Next Approver"
            oGrid.Columns.Item("U_Z_NxtApprover").Editable = False
            oGrid.Columns.Item("U_Z_Dimension").TitleObject.Caption = "Distr.Rule"
            oGrid.Columns.Item("U_Z_PayPosted").TitleObject.Caption = "Posted to Payroll"
            oGrid.Columns.Item("U_Z_PayPosted").Editable = False
            oGrid.Columns.Item("U_Z_DebitCode").Visible = False
            oGrid.Columns.Item("U_Z_CreditCode").Visible = False
            oGrid.Columns.Item("U_Z_Posting").Editable = False
            oGrid.Columns.Item("U_Z_Posting").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oGrid.Columns.Item("U_Z_Posting").TitleObject.Caption = "Posting Type"
            ocombo = oGrid.Columns.Item("U_Z_Posting")
            ocombo.ValidValues.Add("G", "G/L Account")
            ocombo.ValidValues.Add("P", "Payroll")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            ocombo.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
            oGrid.Columns.Item("U_Z_CardCode").Visible = False
            oGrid.AutoResizeColumns()
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                oGrid.RowHeaders.SetText(intRow, intRow + 1)
            Next
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        ElseIf aChoice = "Approved" Or aChoice = "Rejected" Then
            oGrid = aForm.Items.Item(gridid).Specific
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("Name").Visible = False
            oGrid.Columns.Item("U_Z_OverLap").Visible = False
            oGrid.Columns.Item("U_Z_Subdt").TitleObject.Caption = "Submitted Date"
            oGrid.Columns.Item("U_Z_Subdt").Visible = False
            oGrid.Columns.Item("U_Z_Claimdt").TitleObject.Caption = "Date"
            oGrid.Columns.Item("U_Z_Client").Visible = False
            oGrid.Columns.Item("U_Z_Project").Visible = False
            oGrid.Columns.Item("U_Z_TripType").TitleObject.Caption = "Trip Type"
            oGrid.Columns.Item("U_Z_TripType").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oComboColumn = oGrid.Columns.Item("U_Z_TripType")
            oComboColumn.ValidValues.Add("", "")
            oComboColumn.ValidValues.Add("E", "Existing")
            oComboColumn.ValidValues.Add("N", "New")
            oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_TripType").Visible = False
            oGrid.Columns.Item("U_Z_TraCode").TitleObject.Caption = "Travel Code"
            oGrid.Columns.Item("U_Z_TraCode").Visible = False
            oGrid.Columns.Item("U_Z_TraDesc").TitleObject.Caption = "Travel Description"
            oGrid.Columns.Item("U_Z_TraDesc").Visible = False
            oGrid.Columns.Item("U_Z_City").TitleObject.Caption = "City"
            oGrid.Columns.Item("U_Z_AlloCode").TitleObject.Caption = "Allowance Code"
            oGrid.Columns.Item("U_Z_AlloCode").Editable = False
            oGrid.Columns.Item("U_Z_Currency").TitleObject.Caption = "Transaction Currency"
            oGrid.Columns.Item("U_Z_CurAmt").TitleObject.Caption = "Transaction Amount"
            oGrid.Columns.Item("U_Z_ExcRate").TitleObject.Caption = "Exchange Rate"
            oGrid.Columns.Item("U_Z_UsdAmt").TitleObject.Caption = "Local Currency Amount"
            oGrid.Columns.Item("U_Z_UsdAmt").Editable = False
            oGrid.Columns.Item("U_Z_Dimension").TitleObject.Caption = "Distr.Rule"
            oGrid.Columns.Item("U_Z_Reimburse").TitleObject.Caption = "To be Reimbursed?"
            oGrid.Columns.Item("U_Z_Reimburse").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_Reimburse")
            ocombo.ValidValues.Add("", "")
            ocombo.ValidValues.Add("N", "No")
            ocombo.ValidValues.Add("Y", "Yes")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_ReimAmt").TitleObject.Caption = "Reimbursement Amount"
            oGrid.Columns.Item("U_Z_ReimAmt").Editable = False
            oGrid.Columns.Item("U_Z_ExpType").TitleObject.Caption = "Expense Type (*)"
            oGrid.Columns.Item("U_Z_ExpCode").TitleObject.Caption = "Expenses Code"
            oGrid.Columns.Item("U_Z_ExpCode").Visible = False
            oGrid.Columns.Item("U_Z_PayMethod").TitleObject.Caption = "Payment Method"
            oGrid.Columns.Item("U_Z_PayMethod").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo1 = oGrid.Columns.Item("U_Z_PayMethod")
            oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strQuery = "SELECT ""Code"" As ""Code"", ""U_Z_PayMethod"" As ""Name"" FROM ""@Z_HR_PAYMD"""
            oRecSet.DoQuery(strQuery)
            ocombo1.ValidValues.Add("", "")
            If Not oRecSet.EoF Then
                For index As Integer = 0 To oRecSet.RecordCount - 1
                    If Not oRecSet.EoF Then
                        ocombo1.ValidValues.Add(oRecSet.Fields.Item("Code").Value, oRecSet.Fields.Item("Name").Value)
                        oRecSet.MoveNext()
                    End If
                Next
            End If
            ocombo1.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
            oGrid.Columns.Item("U_Z_Notes").TitleObject.Caption = "Notes"
            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Status"
            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo3 = oGrid.Columns.Item("U_Z_AppStatus")
            ocombo3.ValidValues.Add("P", "Pending")
            ocombo3.ValidValues.Add("R", "Rejected")
            ocombo3.ValidValues.Add("A", "Approved")
            ocombo3.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_AppStatus").Editable = False
            oGrid.Columns.Item("U_Z_Attachment").TitleObject.Caption = "Attachments(Double click to Select Attachment)"
            oGECol = oGrid.Columns.Item("U_Z_Attachment")
            oGECol.LinkedObjectType = "Z_HR_OEXFOM"
            oGrid.Columns.Item("U_Z_Attachment").Editable = False
            oGrid.Columns.Item("U_Z_ExpCode").TitleObject.Caption = "Expenses Code"
            oGrid.Columns.Item("U_Z_ExpCode").Visible = False
            oGrid.Columns.Item("U_Z_CurApprover").TitleObject.Caption = "Current Approver"
            oGrid.Columns.Item("U_Z_CurApprover").Editable = False
            oGrid.Columns.Item("U_Z_NxtApprover").TitleObject.Caption = "Next Approver"
            oGrid.Columns.Item("U_Z_NxtApprover").Editable = False
            oGrid.Columns.Item("U_Z_Posting").TitleObject.Caption = "Posting Type"
            oGrid.Columns.Item("U_Z_Posting").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_Posting")
            ocombo.ValidValues.Add("G", "G/L Account")
            ocombo.ValidValues.Add("P", "Payroll")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            ocombo.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
            oGrid.Columns.Item("U_Z_DebitCode").Visible = False
            oGrid.Columns.Item("U_Z_CreditCode").Visible = False
            oGrid.Columns.Item("U_Z_Posting").Editable = False
            oGrid.Columns.Item("U_Z_CardCode").Visible = False
            oGrid.AutoResizeColumns()
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                oGrid.RowHeaders.SetText(intRow, intRow + 1)
            Next
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        End If
    End Sub
    Private Sub Gridbind()
        Try
            Dim strqry, strQuery As String

            Dim aCode As String
            aCode = oApplication.Utilities.getEdittextvalue(oForm, "23")

            oGrid = oForm.Items.Item("12").Specific
            oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
            oGrid1 = oForm.Items.Item("27").Specific
            oGrid1.DataTable = oForm.DataSources.DataTables.Item("DT_1")
            oGrid2 = oForm.Items.Item("28").Specific
            oGrid2.DataTable = oForm.DataSources.DataTables.Item("DT_2")
            strqry = "select T0.""Code"",T0.""Name"",""U_Z_ExpCode"", T0.""U_Z_Subdt"",T0.""U_Z_TripType"",T0.""U_Z_TraCode"",T0.""U_Z_TraDesc"",""U_Z_ExpType"",""U_Z_AlloCode"",T0.""U_Z_Client"",T0.""U_Z_Project"",""U_Z_Claimdt"",""U_Z_City"",""U_Z_Currency"",""U_Z_CurAmt"",""U_Z_ExcRate"","
            strqry += """U_Z_UsdAmt"",""U_Z_Reimburse"",""U_Z_ReimAmt"",""U_Z_Dimension"",""U_Z_PayMethod"",""U_Z_Notes"",""U_Z_Attachment"",isnull((select T13.firstName +' '+ISNULL(T13.middleName,'') +' '+ isnull(T13.lastName,'')  from OHEM T13 JOIN OUSR T14 ON T14.INTERNAL_K =T13.userId where T14.USER_CODE=T0.U_Z_CurApprover ),'') AS 'U_Z_CurApprover',isnull((select T13.firstName +' '+ISNULL(T13.middleName,'') +' '+ isnull(T13.lastName,'')  from OHEM T13 JOIN OUSR T14 ON T14.INTERNAL_K =T13.userId where T14.USER_CODE=T0.U_Z_NxtApprover ),'') AS 'U_Z_NxtApprover',""U_Z_AppStatus"",""U_Z_PayPosted"",""U_Z_DebitCode"",""U_Z_CreditCode"",""U_Z_Posting"",T0.""U_Z_CardCode"",T0.""U_Z_OverLap""  from ""@Z_HR_EXPCL"" T0 Left join ""@Z_HR_OEXPCL"" T1 on T0.U_Z_DocRefNo=T1.Code  where T1.""U_Z_EmpID""='" & oApplication.Utilities.getEdittextvalue(oForm, "15") & "' and T1.Code='" & aCode & "' and ""U_Z_AppStatus""='P'"
            oGrid.DataTable.ExecuteQuery(strqry)
            formatGrid(oForm, "Request", "12")
            strqry = "select T0.""Code"",T0.""Name"",""U_Z_ExpCode"", T0.""U_Z_Subdt"",T0.""U_Z_TripType"",T0.""U_Z_TraCode"",T0.""U_Z_TraDesc"",""U_Z_ExpType"",""U_Z_AlloCode"",T0.""U_Z_Client"",T0.""U_Z_Project"",""U_Z_Claimdt"",""U_Z_City"",""U_Z_Currency"",""U_Z_CurAmt"",""U_Z_ExcRate"","
            strqry += """U_Z_UsdAmt"",""U_Z_Reimburse"",""U_Z_ReimAmt"",""U_Z_Dimension"",""U_Z_PayMethod"",""U_Z_Notes"",""U_Z_Attachment"",""U_Z_AppStatus"",isnull((select T13.firstName +' '+ISNULL(T13.middleName,'') +' '+ isnull(T13.lastName,'')  from OHEM T13 JOIN OUSR T14 ON T14.INTERNAL_K =T13.userId where T14.USER_CODE=T0.U_Z_CurApprover ),'') AS 'U_Z_CurApprover',isnull((select T13.firstName +' '+ISNULL(T13.middleName,'') +' '+ isnull(T13.lastName,'')  from OHEM T13 JOIN OUSR T14 ON T14.INTERNAL_K =T13.userId where T14.USER_CODE=T0.U_Z_NxtApprover ),'') AS 'U_Z_NxtApprover',""U_Z_PayPosted"",""U_Z_DebitCode"",""U_Z_CreditCode"",""U_Z_Posting"",T0.""U_Z_CardCode"",T0.""U_Z_OverLap""  from ""@Z_HR_EXPCL"" T0 Left join ""@Z_HR_OEXPCL"" T1 on T0.U_Z_DocRefNo=T1.Code  where T1.""U_Z_EmpID""='" & oApplication.Utilities.getEdittextvalue(oForm, "15") & "' and T1.Code='" & aCode & "' and ""U_Z_AppStatus""='A'"
            oGrid1.DataTable.ExecuteQuery(strqry)
            formatGrid(oForm, "Approved", "27")
            strqry = "select T0.""Code"",T0.""Name"",""U_Z_ExpCode"", T0.""U_Z_Subdt"",T0.""U_Z_TripType"",T0.""U_Z_TraCode"",T0.""U_Z_TraDesc"",""U_Z_ExpType"",""U_Z_AlloCode"",T0.""U_Z_Client"",T0.""U_Z_Project"",""U_Z_Claimdt"",""U_Z_City"",""U_Z_Currency"",""U_Z_CurAmt"",""U_Z_ExcRate"","
            strqry += """U_Z_UsdAmt"",""U_Z_Reimburse"",""U_Z_ReimAmt"",""U_Z_Dimension"",""U_Z_PayMethod"",""U_Z_Notes"",""U_Z_Attachment"",""U_Z_AppStatus"",isnull((select T13.firstName +' '+ISNULL(T13.middleName,'') +' '+ isnull(T13.lastName,'')  from OHEM T13 JOIN OUSR T14 ON T14.INTERNAL_K =T13.userId where T14.USER_CODE=T0.U_Z_CurApprover ),'') AS 'U_Z_CurApprover',isnull((select T13.firstName +' '+ISNULL(T13.middleName,'') +' '+ isnull(T13.lastName,'')  from OHEM T13 JOIN OUSR T14 ON T14.INTERNAL_K =T13.userId where T14.USER_CODE=T0.U_Z_NxtApprover ),'') AS 'U_Z_NxtApprover',""U_Z_PayPosted"",""U_Z_DebitCode"",""U_Z_CreditCode"",""U_Z_Posting"",T0.""U_Z_CardCode"",T0.""U_Z_OverLap""  from ""@Z_HR_EXPCL"" T0 Left join ""@Z_HR_OEXPCL"" T1 on T0.U_Z_DocRefNo=T1.Code  where T1.""U_Z_EmpID""='" & oApplication.Utilities.getEdittextvalue(oForm, "15") & "' and T1.Code='" & aCode & "' and ""U_Z_AppStatus""='R'"
            oGrid2.DataTable.ExecuteQuery(strqry)
            formatGrid(oForm, "Rejected", "28")
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid)
        Try
            If aGrid.DataTable.Rows.Count - 1 < 0 Then
                aGrid.DataTable.Rows.Add()
            End If
            If aGrid.DataTable.GetValue("U_Z_ExpType", aGrid.DataTable.Rows.Count - 1) <> "" Then
                aGrid.DataTable.Rows.Add()
                aGrid.DataTable.SetValue("U_Z_Reimburse", aGrid.DataTable.Rows.Count - 1, "Y")
                aGrid.Columns.Item("U_Z_TripType").Click(aGrid.DataTable.Rows.Count - 1, False)
            End If
            oApplication.Utilities.assignMatrixLineno(aGrid, oForm)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
#End Region
#Region "Remove Row"
    Private Sub RemoveRow(ByVal intRow As Integer, ByVal agrid As SAPbouiCOM.Grid)
        Dim strCode, strname As String
        Dim blnValue As Boolean
        Dim otemprec As SAPbobsCOM.Recordset
        For intRow = 0 To agrid.DataTable.Rows.Count - 1
            If agrid.Rows.IsSelected(intRow) Then
                strCode = agrid.DataTable.GetValue(0, intRow)
                strname = agrid.DataTable.GetValue(1, intRow)
                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                blnValue = oApplication.Utilities.ApprovalStatus("ExpCli", strCode)
                If blnValue = False Then
                    oApplication.Utilities.ExecuteSQL(otemprec, "update ""@Z_HR_EXPCL"" set  ""Name"" =""Name"" +'D'  where ""Code""='" & strCode & "'")
                    agrid.DataTable.Rows.Remove(intRow)
                    Exit Sub
                Else
                    oApplication.Utilities.Message("Already in approval cycle.You can't delete this expense.", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Exit Sub
                End If
            End If
        Next
        oApplication.Utilities.Message("No row selected", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
    End Sub
#End Region
#Region "CommitTrans"
    Private Sub Committrans(ByVal strChoice As String)
        Dim oTemprec, oItemRec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strChoice = "Cancel" Then
            oTemprec.DoQuery("Update ""@Z_HR_EXPCL"" set ""Name""=""Code"" where ""Name"" Like '%D'")
        Else
            oTemprec.DoQuery("Select * from ""@Z_HR_EXPCL"" where ""Name"" like '%D'")
            For intRow As Integer = 0 To oTemprec.RecordCount - 1
                oItemRec.DoQuery("delete from ""@Z_HR_EXPCL"" where ""Name""='" & oTemprec.Fields.Item("Name").Value & "' and ""Code""='" & oTemprec.Fields.Item("Code").Value & "' and ""U_Z_AppStatus""<>'A'")
                oTemprec.MoveNext()
            Next
            oTemprec.DoQuery("Delete from  ""@Z_HR_EXPCL""  where ""U_Z_AppStatus""<>'A' and ""Name"" Like '%D'")
        End If

    End Sub
#End Region
    Public Sub CopyAttachment(ByVal Sfile As String)
        Try
            Dim oRec As SAPbobsCOM.Recordset
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim strQry = "Select AttachPath From OADP"
            oRec.DoQuery(strQry)
            Dim SPath As String = Sfile
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
                Dim Filename As String = path.GetFileName(SPath)
                Dim SavePath As String = path.Combine(DPath, Filename)
                If System.IO.File.Exists(SavePath) Then
                Else
                    file.CopyTo(path.Combine(DPath, file.Name), True)
                End If
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        aform.Freeze(True)
        Try
            Dim oUserTable, oUserTable1 As SAPbobsCOM.UserTable
            Dim strCode, strType, intTempID, Headrcode As String
            oGrid = aform.Items.Item("12").Specific
            oUserTable = oApplication.Company.UserTables.Item("Z_HR_EXPCL")
            oUserTable1 = oApplication.Company.UserTables.Item("Z_HR_OEXPCL")
            Headrcode = oApplication.Utilities.getEdittextvalue(aform, "23")
            oCombobox = aform.Items.Item("30").Specific
            oCombobox1 = aform.Items.Item("32").Specific
            oCombobox2 = aform.Items.Item("34").Specific
            If oUserTable1.GetByKey(Headrcode) Then
                oUserTable1.Code = Headrcode
                oUserTable1.Name = Headrcode
                oUserTable1.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "15")
                oUserTable1.UserFields.Fields.Item("U_Z_EmpName").Value = oApplication.Utilities.getEdittextvalue(aform, "4")
                oUserTable1.UserFields.Fields.Item("U_Z_Subdt").Value = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aform, "10"))
                oUserTable1.UserFields.Fields.Item("U_Z_TAEmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "1000001")
                oUserTable1.UserFields.Fields.Item("U_Z_Client").Value = oApplication.Utilities.getEdittextvalue(aform, "19")
                oUserTable1.UserFields.Fields.Item("U_Z_Project").Value = oApplication.Utilities.getEdittextvalue(aform, "21")
                oUserTable1.UserFields.Fields.Item("U_Z_CardCode").Value = oApplication.Utilities.getEdittextvalue(aform, "37")
                oUserTable1.UserFields.Fields.Item("U_Z_TraCode").Value = oCombobox1.Selected.Value
                oUserTable1.UserFields.Fields.Item("U_Z_TraDesc").Value = oCombobox1.Selected.Description
                oUserTable1.UserFields.Fields.Item("U_Z_TripType").Value = oCombobox.Selected.Value
                If oUserTable1.Update <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            Else
                Headrcode = oApplication.Utilities.getMaxCode("@Z_HR_OEXPCL", "Code")
                oUserTable1.Code = Headrcode
                oUserTable1.Name = Headrcode
                oUserTable1.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "15")
                oUserTable1.UserFields.Fields.Item("U_Z_EmpName").Value = oApplication.Utilities.getEdittextvalue(aform, "4")
                oUserTable1.UserFields.Fields.Item("U_Z_Subdt").Value = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aform, "10"))
                oUserTable1.UserFields.Fields.Item("U_Z_TAEmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "1000001")
                oUserTable1.UserFields.Fields.Item("U_Z_Client").Value = oApplication.Utilities.getEdittextvalue(aform, "19")
                oUserTable1.UserFields.Fields.Item("U_Z_Project").Value = oApplication.Utilities.getEdittextvalue(aform, "21")
                oUserTable1.UserFields.Fields.Item("U_Z_CardCode").Value = oApplication.Utilities.getEdittextvalue(aform, "37")
                oUserTable1.UserFields.Fields.Item("U_Z_TraCode").Value = oCombobox1.Selected.Value
                oUserTable1.UserFields.Fields.Item("U_Z_TraDesc").Value = oCombobox1.Selected.Description
                oUserTable1.UserFields.Fields.Item("U_Z_TripType").Value = oCombobox.Selected.Value
                oUserTable1.UserFields.Fields.Item("U_Z_DocStatus").Value = "O"
                If oUserTable1.Add <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    oApplication.Utilities.setEdittextvalue(aform, "23", Headrcode)
                End If
            End If
            Dim Reimbused As String = "N"
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                strCode = oGrid.DataTable.GetValue("Code", intRow)
                oEditTextColumn = oGrid.Columns.Item("U_Z_Claimdt")
                Try
                    strType = oEditTextColumn.GetText(oGrid.DataTable.Rows.Count - 1).ToString
                Catch ex As Exception
                    strType = ""
                End Try

                If strType <> "" Then
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode
                        Dim strdate As String = oGrid.DataTable.GetValue("U_Z_Claimdt", intRow)
                        Dim dtDate As Date
                        If strdate <> "" Then
                            oUserTable.UserFields.Fields.Item("U_Z_Claimdt").Value = oGrid.DataTable.GetValue("U_Z_Claimdt", intRow)
                            dtDate = oGrid.DataTable.GetValue("U_Z_Claimdt", intRow)
                        Else
                            dtDate = Now.Date
                        End If

                        oUserTable.UserFields.Fields.Item("U_Z_TripType").Value = oCombobox.Selected.Value
                        ocombo = oGrid.Columns.Item("U_Z_Reimburse")
                        Try
                            If ocombo.GetSelectedValue(intRow).Value <> "" Then
                                oUserTable.UserFields.Fields.Item("U_Z_Reimburse").Value = ocombo.GetSelectedValue(intRow).Value
                                Reimbused = ocombo.GetSelectedValue(intRow).Value
                            Else
                                oUserTable.UserFields.Fields.Item("U_Z_Reimburse").Value = "N"
                                Reimbused = "N"
                            End If
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_Reimburse").Value = "N"
                            Reimbused = "N"
                        End Try

                        ocombo1 = oGrid.Columns.Item("U_Z_PayMethod")
                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_PayMethod").Value = ocombo1.GetSelectedValue(intRow).Value
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_PayMethod").Value = "-"
                        End Try

                        ocombo2 = oGrid.Columns.Item("U_Z_AppStatus")
                        oUserTable.UserFields.Fields.Item("U_Z_CardCode").Value = oApplication.Utilities.getEdittextvalue(aform, "37")
                        oUserTable.UserFields.Fields.Item("U_Z_DocRefNo").Value = oApplication.Utilities.getEdittextvalue(aform, "23")
                        oUserTable.UserFields.Fields.Item("U_Z_AppStatus").Value = ocombo2.GetSelectedValue(intRow).Value
                        oUserTable.UserFields.Fields.Item("U_Z_ExpType").Value = oGrid.DataTable.GetValue("U_Z_ExpType", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_ExpCode").Value = oGrid.DataTable.GetValue("U_Z_ExpCode", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_TraCode").Value = oCombobox1.Selected.Value
                        ocombo3 = oGrid.Columns.Item("U_Z_Currency")
                        oUserTable.UserFields.Fields.Item("U_Z_Currency").Value = ocombo3.GetSelectedValue(intRow).Value
                        oUserTable.UserFields.Fields.Item("U_Z_Client").Value = oApplication.Utilities.getEdittextvalue(aform, "19")
                        oUserTable.UserFields.Fields.Item("U_Z_AlloCode").Value = oGrid.DataTable.GetValue("U_Z_AlloCode", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Project").Value = oApplication.Utilities.getEdittextvalue(aform, "21")
                        oUserTable.UserFields.Fields.Item("U_Z_TraDesc").Value = oCombobox1.Selected.Description
                        oUserTable.UserFields.Fields.Item("U_Z_City").Value = oGrid.DataTable.GetValue("U_Z_City", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_CurAmt").Value = oGrid.DataTable.GetValue("U_Z_CurAmt", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_ExcRate").Value = oGrid.DataTable.GetValue("U_Z_ExcRate", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_UsdAmt").Value = oGrid.DataTable.GetValue("U_Z_UsdAmt", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_ReimAmt").Value = oGrid.DataTable.GetValue("U_Z_ReimAmt", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Notes").Value = oGrid.DataTable.GetValue("U_Z_Notes", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Attachment").Value = oGrid.DataTable.GetValue("U_Z_Attachment", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_DebitCode").Value = oGrid.DataTable.GetValue("U_Z_DebitCode", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_CreditCode").Value = oGrid.DataTable.GetValue("U_Z_CreditCode", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Posting").Value = oGrid.DataTable.GetValue("U_Z_Posting", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Dimension").Value = oGrid.DataTable.GetValue("U_Z_Dimension", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Year").Value = dtDate.Year
                        oUserTable.UserFields.Fields.Item("U_Z_Month").Value = dtDate.Month
                        If oGrid.DataTable.GetValue("U_Z_PayPosted", intRow) <> "Y" Then
                            oUserTable.UserFields.Fields.Item("U_Z_PayPosted").Value = "N"
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_PayPosted").Value = "Y"

                        End If

                        If oUserTable.Update <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Return False
                        Else
                            CopyAttachment(oGrid.DataTable.GetValue("U_Z_Attachment", intRow))

                            oApplication.Utilities.AddtoUDT1_PayrollTrans(strCode)
                            If oGrid.DataTable.GetValue("U_Z_Posting", intRow) = "G" Then
                                oApplication.Utilities.CreateJournelVouchers(strCode, Reimbused)
                            End If

                        End If
                    Else
                        strCode = oApplication.Utilities.getMaxCode("@Z_HR_EXPCL", "Code")
                        oUserTable.Code = strCode
                        oUserTable.Name = strCode + "_N"
                        oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = oApplication.Utilities.getEdittextvalue(aform, "15")
                        oUserTable.UserFields.Fields.Item("U_Z_EmpName").Value = oApplication.Utilities.getEdittextvalue(aform, "4")
                        oUserTable.UserFields.Fields.Item("U_Z_Subdt").Value = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aform, "10"))
                        Dim strdate As String = oGrid.DataTable.GetValue("U_Z_Claimdt", intRow)
                        Dim dtDate As Date
                        If strdate <> "" Then
                            oUserTable.UserFields.Fields.Item("U_Z_Claimdt").Value = oGrid.DataTable.GetValue("U_Z_Claimdt", intRow)
                            dtDate = oGrid.DataTable.GetValue("U_Z_Claimdt", intRow)
                        Else
                            dtDate = Now.Date
                        End If
                        oComboColumn = oGrid.Columns.Item("U_Z_TripType")
                        oUserTable.UserFields.Fields.Item("U_Z_TripType").Value = oCombobox.Selected.Value
                        ocombo = oGrid.Columns.Item("U_Z_Reimburse")
                        Try
                            If ocombo.GetSelectedValue(intRow).Value <> "" Then
                                oUserTable.UserFields.Fields.Item("U_Z_Reimburse").Value = ocombo.GetSelectedValue(intRow).Value
                                Reimbused = ocombo.GetSelectedValue(intRow).Value
                            Else
                                oUserTable.UserFields.Fields.Item("U_Z_Reimburse").Value = "N"
                                Reimbused = "N"
                            End If
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_Reimburse").Value = "N"
                            Reimbused = "N"
                        End Try
                        ocombo1 = oGrid.Columns.Item("U_Z_PayMethod")
                        ocombo3 = oGrid.Columns.Item("U_Z_Currency")
                        oUserTable.UserFields.Fields.Item("U_Z_CardCode").Value = oApplication.Utilities.getEdittextvalue(aform, "37")
                        oUserTable.UserFields.Fields.Item("U_Z_DocRefNo").Value = oApplication.Utilities.getEdittextvalue(aform, "23")
                        oUserTable.UserFields.Fields.Item("U_Z_Currency").Value = ocombo3.GetSelectedValue(intRow).Value
                        oUserTable.UserFields.Fields.Item("U_Z_PayMethod").Value = ocombo1.GetSelectedValue(intRow).Value
                        oUserTable.UserFields.Fields.Item("U_Z_ExpType").Value = oGrid.DataTable.GetValue("U_Z_ExpType", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_ExpCode").Value = oGrid.DataTable.GetValue("U_Z_ExpCode", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_TraCode").Value = oCombobox1.Selected.Value
                        oUserTable.UserFields.Fields.Item("U_Z_Client").Value = oApplication.Utilities.getEdittextvalue(aform, "19")
                        oUserTable.UserFields.Fields.Item("U_Z_AlloCode").Value = oGrid.DataTable.GetValue("U_Z_AlloCode", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Project").Value = oApplication.Utilities.getEdittextvalue(aform, "21")
                        oUserTable.UserFields.Fields.Item("U_Z_TraDesc").Value = oCombobox1.Selected.Description
                        oUserTable.UserFields.Fields.Item("U_Z_City").Value = oGrid.DataTable.GetValue("U_Z_City", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_CurAmt").Value = oGrid.DataTable.GetValue("U_Z_CurAmt", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_ExcRate").Value = oGrid.DataTable.GetValue("U_Z_ExcRate", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_UsdAmt").Value = oGrid.DataTable.GetValue("U_Z_UsdAmt", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_ReimAmt").Value = oGrid.DataTable.GetValue("U_Z_ReimAmt", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Notes").Value = oGrid.DataTable.GetValue("U_Z_Notes", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Attachment").Value = oGrid.DataTable.GetValue("U_Z_Attachment", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_DebitCode").Value = oGrid.DataTable.GetValue("U_Z_DebitCode", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_CreditCode").Value = oGrid.DataTable.GetValue("U_Z_CreditCode", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Posting").Value = oGrid.DataTable.GetValue("U_Z_Posting", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Dimension").Value = oGrid.DataTable.GetValue("U_Z_Dimension", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Year").Value = dtDate.Year
                        oUserTable.UserFields.Fields.Item("U_Z_Month").Value = dtDate.Month
                        oUserTable.UserFields.Fields.Item("U_Z_AppStatus").Value = oApplication.Utilities.DocApproval(aform, HeaderDoctype.ExpCli, oApplication.Utilities.getEdittextvalue(aform, "15")) '"P"
                        If oGrid.DataTable.GetValue("U_Z_PayPosted", intRow) <> "Y" Then
                            oUserTable.UserFields.Fields.Item("U_Z_PayPosted").Value = "N"
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_PayPosted").Value = "Y"

                        End If

                        If oUserTable.Add <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Return False
                        Else
                            CopyAttachment(oGrid.DataTable.GetValue("U_Z_Attachment", intRow))
                            If strMailCode = "" Then
                                strMailCode = strCode
                            Else
                                strMailCode = strMailCode & "," & strCode
                            End If
                            oApplication.Utilities.AddtoUDT1_PayrollTrans(strCode)
                            If oGrid.DataTable.GetValue("U_Z_Posting", intRow) = "G" Then
                                oApplication.Utilities.CreateJournelVouchers(strCode, Reimbused)
                            End If
                            ExpintTempID = oApplication.Utilities.GetTemplateID(aform, HeaderDoctype.ExpCli, oApplication.Utilities.getEdittextvalue(aform, "15"))
                            If ExpintTempID <> "0" Then
                                oApplication.Utilities.UpdateApprovalRequired("@Z_HR_EXPCL", "Code", strCode, "Y", ExpintTempID)
                            Else
                                oApplication.Utilities.UpdateApprovalRequired("@Z_HR_EXPCL", "Code", strCode, "N", ExpintTempID)
                            End If
                        End If
                    End If
                End If
            Next
            If strMailCode <> "" Then
                oApplication.Utilities.InitialMessage("Expense Claim No:", Headrcode, oApplication.Utilities.DocApproval(oForm, HeaderDoctype.ExpCli, oApplication.Utilities.getEdittextvalue(oForm, "15")), ExpintTempID, oApplication.Utilities.getEdittextvalue(oForm, "4"), HistoryDoctype.ExpCli, strMailCode)
                strMailCode = ""
            End If
            'oApplication.Utilities.InitialMessage("Expense Claim", strCode, oApplication.Utilities.DocApproval(aform, HeaderDoctype.ExpCli, oApplication.Utilities.getEdittextvalue(aform, "15")), intTempID, oApplication.Utilities.getEdittextvalue(aform, "4"), HistoryDoctype.ExpCli, strMailCode)
            oAttGrid = aform.Items.Item("12").Specific
            For i As Integer = 0 To oAttGrid.DataTable.Rows.Count - 1
                Dim oRec As SAPbobsCOM.Recordset
                oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                Dim strQry = "Select AttachPath From OADP"
                oRec.DoQuery(strQry)
                Dim SPath As String = oAttGrid.DataTable.GetValue("U_Z_Attachment", i).ToString()
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
            oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            Committrans("Add")
            AddMode(aform)
            Gridbind()
            aform.Freeze(False)
            Return True

        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Function
#End Region

#Region "Validation"
    Private Function Validation(ByVal sForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim strdate, Tracode, Exptype, Cur, Client, Project, Triptype, Tradesc, Ovarlap As String
            Dim currency As Double
            oGrid = sForm.Items.Item("12").Specific
            oCombobox = sForm.Items.Item("30").Specific
            oCombobox1 = sForm.Items.Item("32").Specific
            Triptype = oCombobox.Selected.Value
            Tradesc = oCombobox1.Selected.Value
            If Triptype <> "" Then
                If Triptype = "E" Then
                    If Tradesc = "" Then
                        oApplication.Utilities.Message("Travel Code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        sForm.Items.Item("32").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                        Return False
                    End If
                End If
            Else
                oApplication.Utilities.Message("Trip Type is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                sForm.Items.Item("30").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If
            If oGrid.DataTable.Rows.Count > 0 Then
                For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                    strdate = oGrid.DataTable.GetValue("U_Z_Claimdt", intRow)
                    currency = CDbl(oGrid.DataTable.GetValue("U_Z_CurAmt", intRow))
                    Tracode = oGrid.DataTable.GetValue("U_Z_TraCode", intRow) ' ocombo.GetSelectedValue(intRow).Value
                    ocombo1 = oGrid.Columns.Item("U_Z_Currency")
                    Cur = ocombo1.GetSelectedValue(intRow).Value
                    Exptype = oGrid.DataTable.GetValue("U_Z_ExpType", intRow) ' ocombo2.GetSelectedValue(intRow).Value
                    Client = oGrid.DataTable.GetValue("U_Z_Client", intRow)
                    Project = oGrid.DataTable.GetValue("U_Z_Project", intRow)
                    oComboColumn = oGrid.Columns.Item("U_Z_TripType")
                    Ovarlap = oGrid.DataTable.GetValue("U_Z_OverLap", intRow)
                  
                    Dim strfromdt As Date
                    If strdate <> "" Then
                        strfromdt = oApplication.Utilities.GetDateTimeValue(strdate)
                    End If

                    'If Client = "" Then
                    '    oApplication.Utilities.Message("Client is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    '    Return False
                    'ElseIf Project = "" Then
                    '    oApplication.Utilities.Message("Project is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    '    Return False
                    'Else
                    If Exptype = "" Then
                        oApplication.Utilities.Message("Expenses Type is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        oGrid.Columns.Item("U_Z_ExpType").Click(intRow)
                        Return False
                    ElseIf strdate = "" Then
                        oApplication.Utilities.Message("Transaction date is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        oGrid.Columns.Item("U_Z_Claimdt").Click(intRow)
                        Return False
                    ElseIf Cur = "" Then
                        oApplication.Utilities.Message("Transaction Currency is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        oGrid.Columns.Item("U_Z_Currency").Click(intRow)
                        Return False
                    ElseIf currency = 0.0 Then
                        oApplication.Utilities.Message("Transaction Amount is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        oGrid.Columns.Item("U_Z_CurAmt").Click(intRow)
                        Return False
                    End If
                    If Ovarlap.Trim() = "Y" Then
                        Dim oRec As SAPbobsCOM.Recordset
                        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        Dim StrQuery As String = "select * from [@Z_HR_OTRAREQ] where U_Z_EmpId='" & oApplication.Utilities.getEdittextvalue(sForm, "15") & "' and U_Z_AppStatus='A' and '" & strfromdt.ToString("yyyy-MM-dd") & "' between U_Z_TraStDate and U_Z_TraEndDate"
                        oRec.DoQuery(StrQuery)
                        If oRec.RecordCount > 0 Then
                            oApplication.Utilities.Message("You have an approved BTA for this date, you cannot proceed with another expense claim. Expense Type : " & Exptype & "..", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            oGrid.Columns.Item("U_Z_ExpType").Click(intRow)
                            Return False
                        End If
                    End If
                Next
            End If
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
#End Region
#Region "FileOpen"
    Private Sub FileOpen()
        Dim mythr As New System.Threading.Thread(AddressOf ShowFileDialog)
        mythr.SetApartmentState(Threading.ApartmentState.STA)
        mythr.Start()
        mythr.Join()
    End Sub

    Private Sub ShowFileDialog()
        Dim oDialogBox As New OpenFileDialog
        Dim strMdbFilePath As String
        Dim oProcesses() As Process
        Try
            Dim aform As New System.Windows.Forms.Form
            aform.TopMost = True
            oProcesses = Process.GetProcessesByName("SAP Business One")
            If oProcesses.Length <> 0 Then
                For i As Integer = 0 To oProcesses.Length - 1
                    Dim MyWindow As New clsListener.WindowWrapper(oProcesses(i).MainWindowHandle)
                    If oDialogBox.ShowDialog(MyWindow) = DialogResult.OK Then
                        strMdbFilePath = oDialogBox.FileName
                        strFilepath = oDialogBox.FileName
                        Exit For
                    Else
                        Exit For
                    End If
                Next
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
        End Try
    End Sub
#End Region
    Private Sub LoadFiles(ByVal aform As SAPbouiCOM.Form)
        oGrid = aform.Items.Item("12").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.Rows.IsSelected(intRow) Then
                Dim strFilename, strFilePath As String
                strFilename = oGrid.DataTable.GetValue("U_Z_Attachment", intRow)
                Dim Filename As String = Path.GetFileName(strFilename)
                strFilePath = oGrid.DataTable.GetValue("U_Z_Attachment", intRow)
                If File.Exists(strFilePath) = False Then
                    Dim oRec As SAPbobsCOM.Recordset
                    oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    Dim strQry = "Select ""AttachPath"" From OADP"
                    oRec.DoQuery(strQry)
                    strFilePath = oRec.Fields.Item(0).Value

                    If Filename = "" Then
                        strFilePath = strFilePath
                    Else
                        strFilePath = strFilePath & Filename
                    End If
                    If File.Exists(strFilePath) = False Then
                        oApplication.Utilities.Message("File does not exists ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Exit Sub
                    End If
                    strFilename = strFilePath
                Else
                    strFilename = strFilePath
                End If

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
    Private Function BindDimension(ByVal EmpId As String) As String
        Try
            Dim strDimension As String
            Dim oRec As SAPbobsCOM.Recordset
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim strQry = "Select isnull(U_Z_Cost,'') +';'+isnull(U_Z_Dept,'') +';'+isnull(U_Z_Dim3,'') +';'+ isnull(U_Z_HRCost,'') From OHEM where empID=" & EmpId
            oRec.DoQuery(strQry)
            If oRec.RecordCount > 0 Then
                strDimension = oRec.Fields.Item(0).Value
            Else
                strDimension = ""
            End If
            Return strDimension
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Function
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_ExpenseClaim Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "13" Then
                                    If Validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "12" And pVal.ColUID = "U_Z_TraCode" And pVal.Row <> -1 Then
                                    oGrid = oForm.Items.Item("12").Specific
                                    For intRow As Integer = pVal.Row To pVal.Row
                                        If 1 = 1 Then
                                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                            Dim strcode, strstatus As String
                                            strcode = oGrid.DataTable.GetValue("U_Z_TraCode", intRow)
                                            Dim objct As New clshrTravelAgenda
                                            objct.LoadForm1(strcode)
                                        End If
                                    Next
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "12" And pVal.ColUID = "U_Z_ExpType" And pVal.Row <> -1 Then
                                    oGrid = oForm.Items.Item("12").Specific
                                    For intRow As Integer = pVal.Row To pVal.Row
                                        If 1 = 1 Then
                                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                            Dim objct As New clshrExpenses
                                            objct.LoadForm()
                                        End If
                                    Next
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                oGrid = oForm.Items.Item("12").Specific
                                If pVal.ItemUID = "12" And pVal.ColUID = "U_Z_ExcRate" And pVal.CharPressed <> 9 Then
                                    If oGrid.DataTable.GetValue("U_Z_Currency", pVal.Row) = LocalCurrency Or oGrid.DataTable.GetValue("U_Z_Currency", pVal.Row) = SystemCurrency Then
                                        oGrid.DataTable.SetValue("U_Z_ExcRate", pVal.Row, 1)
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                Dim Status As String
                                If pVal.ItemUID = "12" Then
                                    For intRow As Integer = pVal.Row To pVal.Row
                                        If 1 = 1 Then
                                            Status = oGrid.DataTable.GetValue("U_Z_AppStatus", intRow)
                                            If Status = "A" Or Status = "R" Then
                                                If pVal.CharPressed <> 9 And pVal.ItemUID = "12" And (pVal.ColUID = "U_Z_ExpType" Or pVal.ColUID = "U_Z_TraCode" Or pVal.ColUID = "U_Z_Client" Or pVal.ColUID = "U_Z_Project" Or pVal.ColUID = "U_Z_Claimdt" Or pVal.ColUID = "U_Z_CurAmt" Or pVal.ColUID = "U_Z_ExcRate" Or pVal.ColUID = "U_Z_Notes" Or pVal.ColUID = "U_Z_Attachment" Or pVal.ColUID = "U_Z_TraCode" Or pVal.ColUID = "U_Z_City") Then
                                                    BubbleEvent = False
                                                    Exit Sub
                                                End If
                                            End If
                                        End If
                                    Next
                                End If
                                If pVal.ItemUID = "12" And (pVal.ColUID = "U_Z_TraDesc" Or pVal.ColUID = "U_Z_TraCode") And pVal.Row <> -1 Then
                                    oComboColumn = oGrid.Columns.Item("U_Z_TripType")
                                    Dim type As String = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    If type = "E" Or type = "" Then
                                        If pVal.CharPressed <> 9 And pVal.ItemUID = "12" And (pVal.ColUID = "U_Z_TraDesc") Then
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    ElseIf type = "N" Or type = "" Then
                                        If (pVal.CharPressed = 9 Or pVal.CharPressed <> 9) And pVal.ItemUID = "12" And (pVal.ColUID = "U_Z_TraCode") Then
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                oGrid = oForm.Items.Item("12").Specific
                                Dim Status As String
                                If pVal.ItemUID = "12" Then
                                    For intRow As Integer = pVal.Row To pVal.Row
                                        If 1 = 1 Then
                                            Status = oGrid.DataTable.GetValue("U_Z_AppStatus", intRow)
                                            If Status = "A" Or Status = "R" Then
                                                If pVal.CharPressed <> 9 And pVal.ItemUID = "12" And (pVal.ColUID = "U_Z_Currency" Or pVal.ColUID = "U_Z_TripType" Or pVal.ColUID = "U_Z_Reimburse" Or pVal.ColUID = "U_Z_PayMethod") Then
                                                    BubbleEvent = False
                                                    Exit Sub
                                                End If
                                            End If
                                        End If
                                    Next
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                oGrid = oForm.Items.Item("12").Specific
                                Dim Status As String
                                If pVal.ItemUID = "12" And pVal.Row <> -1 Then
                                    For intRow As Integer = pVal.Row To pVal.Row
                                        If 1 = 1 Then
                                            Status = oGrid.DataTable.GetValue("U_Z_AppStatus", intRow)
                                            If Status = "A" Or Status = "R" Then
                                                If pVal.CharPressed <> 9 And pVal.ItemUID = "12" And (pVal.ColUID = "U_Z_TraCode" Or pVal.ColUID = "U_Z_ExpType" Or pVal.ColUID = "U_Z_Attachment") Then
                                                    BubbleEvent = False
                                                    Exit Sub
                                                End If
                                            End If
                                        End If
                                    Next
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_PICKER_CLICKED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                oGrid = oForm.Items.Item("12").Specific
                                If pVal.ItemUID = "12" And (pVal.ColUID = "U_Z_TraDesc" Or pVal.ColUID = "U_Z_TraCode") And pVal.Row <> -1 Then
                                    oComboColumn = oGrid.Columns.Item("U_Z_TripType")
                                    Dim type As String = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    If type = "E" Then
                                        If pVal.CharPressed <> 9 And pVal.ItemUID = "12" And (pVal.ColUID = "U_Z_TraDesc") Then
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    ElseIf type = "N" Or type = "" Then
                                        If pVal.CharPressed <> 9 And pVal.ItemUID = "12" And (pVal.ColUID = "U_Z_TraCode") Then
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    End If
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                reDrawForm(oForm)
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "12" And pVal.ColUID = "U_Z_Attachment" Then
                                    oGrid = oForm.Items.Item("12").Specific
                                    oGrid.Columns.Item("RowsHeader").Click(pVal.Row)
                                    LoadFiles(oForm)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                oGrid = oForm.Items.Item("12").Specific
                                Dim dblcur, dblexrate, dblusd As Double
                                If pVal.ItemUID = "12" And pVal.ColUID = "U_Z_ExcRate" And pVal.CharPressed = 9 Then
                                    dblcur = oGrid.DataTable.GetValue("U_Z_CurAmt", pVal.Row)
                                    dblexrate = oGrid.DataTable.GetValue("U_Z_ExcRate", pVal.Row)
                                    dblusd = dblcur * dblexrate
                                    oGrid.DataTable.SetValue("U_Z_UsdAmt", pVal.Row, LocalCurrency & Math.Round(dblusd, 2))
                                    ocombo = oGrid.Columns.Item("U_Z_Reimburse")
                                    Dim strst As String = ocombo.GetSelectedValue(pVal.Row).Value
                                    If ocombo.GetSelectedValue(pVal.Row).Value = "Y" Then
                                        oGrid.DataTable.SetValue("U_Z_ReimAmt", pVal.Row, LocalCurrency & Math.Round(dblusd, 2))
                                    End If
                                End If

                             

                                If pVal.ItemUID = "12" And pVal.CharPressed = 9 And pVal.ColUID = "U_Z_Dimension" Then
                                    'oMatrix = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim oObj As New clsDisRule
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    oObj.SourceFormUID = FormUID
                                    oObj.ItemUID = pVal.ItemUID
                                    oObj.sourceColumID = pVal.ColUID
                                    oObj.sourcerowId = pVal.Row
                                    oObj.strStaticValue = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    oApplication.Utilities.LoadForm(xml_DisRule, frm_DisRule)
                                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                                    oObj.databound(oForm)

                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "12" And pVal.ColUID = "U_Z_CurAmt" And pVal.CharPressed = 9 Then
                                    dblcur = oGrid.DataTable.GetValue("U_Z_CurAmt", pVal.Row)
                                    dblexrate = oGrid.DataTable.GetValue("U_Z_ExcRate", pVal.Row)
                                    If dblexrate = 0 Then
                                        oGrid.DataTable.SetValue("U_Z_ExcRate", pVal.Row, 1)
                                        dblexrate = oGrid.DataTable.GetValue("U_Z_ExcRate", pVal.Row)
                                    End If
                                    dblusd = dblcur * dblexrate
                                    oGrid.DataTable.SetValue("U_Z_UsdAmt", pVal.Row, LocalCurrency & Math.Round(dblusd, 2))
                                    ocombo = oGrid.Columns.Item("U_Z_Reimburse")
                                    Dim strst As String = ocombo.GetSelectedValue(pVal.Row).Value
                                    If ocombo.GetSelectedValue(pVal.Row).Value = "Y" Then
                                        oGrid.DataTable.SetValue("U_Z_ReimAmt", pVal.Row, LocalCurrency & Math.Round(dblusd, 2))
                                    End If
                                End If
                               
                                If pVal.ItemUID = "12" And pVal.ColUID = "U_Z_TraCode" And pVal.CharPressed = 9 Then
                                    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                    Dim strIns As String
                                    oGrid = oForm.Items.Item("12").Specific
                                    Dim objChooseForm As SAPbouiCOM.Form
                                    Dim objChoose As New clsChooseFromList
                                    clsChooseFromList.ItemUID = pVal.ItemUID
                                    clsChooseFromList.SourceFormUID = FormUID
                                    clsChooseFromList.SourceLabel = 0
                                    clsChooseFromList.CFLChoice = "TravelCode" 'oCombo.Selected.Value
                                    clsChooseFromList.choice = "Travel"
                                    clsChooseFromList.Documentchoice = "TravelCode"
                                    'clsChooseFromList.ItemCode = oApplication.Utilities.getEdittextvalue(oForm, "13")
                                    'oComboColumn = oGrid.Columns.Item("U_Z_Dept")
                                    Try
                                        clsChooseFromList.BinDescrUID = oApplication.Utilities.getEdittextvalue(oForm, "15")
                                    Catch ex As Exception
                                        clsChooseFromList.BinDescrUID = "x"
                                    End Try

                                    clsChooseFromList.sourceColumID = pVal.ColUID
                                    clsChooseFromList.SourceLabel = pVal.Row
                                    oApplication.Utilities.LoadForm("\CFL.xml", frm_ChoosefromList)
                                    objChooseForm = oApplication.SBO_Application.Forms.ActiveForm()
                                    objChoose.databound(objChooseForm)
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "30" Then
                                    oCombobox = oForm.Items.Item("30").Specific
                                    oCombobox2 = oForm.Items.Item("32").Specific
                                    If oCombobox.Selected.Value = "N" Then

                                        oCombobox2.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
                                        oForm.Items.Item("19").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                        oForm.Items.Item("32").Enabled = False
                                    Else

                                        oForm.Items.Item("32").Enabled = True
                                    End If
                                End If
                                If pVal.ItemUID = "12" And pVal.ColUID = "U_Z_Reimburse" Then
                                    ocombo = oGrid.Columns.Item("U_Z_Reimburse")
                                    If ocombo.GetSelectedValue(pVal.Row).Value = "Y" Then
                                        oGrid.DataTable.SetValue("U_Z_ReimAmt", pVal.Row, oGrid.DataTable.GetValue("U_Z_UsdAmt", pVal.Row))
                                    Else
                                        '  oGrid.DataTable.SetValue("U_Z_ReimAmt", pVal.Row, LocalCurrency & "0.0")
                                        oGrid.DataTable.SetValue("U_Z_ReimAmt", pVal.Row, oGrid.DataTable.GetValue("U_Z_UsdAmt", pVal.Row))
                                    End If
                                End If
                                If pVal.ItemUID = "12" And pVal.ColUID = "U_Z_Currency" Then
                                    oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    ocombo = oGrid.Columns.Item("U_Z_Currency")
                                    Dim CurCode As String = ocombo.GetSelectedValue(pVal.Row).Value
                                    Dim dtsub As String = oGrid.DataTable.GetValue("U_Z_Claimdt", pVal.Row) ' oApplication.Utilities.getEdittextvalue(oForm, "10")
                                    Dim dt As Date = oGrid.DataTable.GetValue("U_Z_Claimdt", pVal.Row) 'oApplication.Utilities.GetDateTimeValue(dtsub)
                                    If dtsub <> "" Then
                                        strqry1 = "Select ""Rate"" from ORTT where ""RateDate""='" & dt.ToString("yyyy-MM-dd") & "' and ""Currency""='" & CurCode & "'"
                                        oRecSet.DoQuery(strqry1)
                                        If oRecSet.RecordCount > 0 Then
                                            oGrid.DataTable.SetValue("U_Z_ExcRate", pVal.Row, oRecSet.Fields.Item("Rate").Value)
                                        Else
                                            oGrid.DataTable.SetValue("U_Z_ExcRate", pVal.Row, 1.0)
                                        End If
                                    End If
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "20" Then
                                    If oForm.PaneLevel = 3 Then
                                        oGrid = oForm.Items.Item("28").Specific
                                    ElseIf oForm.PaneLevel = 2 Then
                                        oGrid = oForm.Items.Item("27").Specific
                                    Else

                                        oGrid = oForm.Items.Item("12").Specific
                                    End If

                                    For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                        If oGrid.Rows.IsSelected(intRow) Then
                                            Dim objHistory As New clshrAppHisDetails
                                            If oGrid.DataTable.GetValue("Code", intRow) <> "" Then
                                                objHistory.LoadForm(oForm, HistoryDoctype.ExpCli, oGrid.DataTable.GetValue("Code", intRow))
                                            End If
                                            Exit Sub
                                        End If
                                    Next

                                End If
                                If pVal.ItemUID = "1000005" Then
                                    oForm.PaneLevel = 1
                                ElseIf pVal.ItemUID = "24" Then
                                    oForm.PaneLevel = 2
                                ElseIf pVal.ItemUID = "25" Then
                                    oForm.PaneLevel = 3
                                End If
                                If pVal.ItemUID = "13" Then
                                    AddtoUDT1(oForm)
                                End If
                                If pVal.ItemUID = "16" Then
                                    oGrid = oForm.Items.Item("12").Specific
                                    AddEmptyRow(oGrid)
                                End If
                                If pVal.ItemUID = "17" Then
                                    oGrid = oForm.Items.Item("12").Specific
                                    RemoveRow(pVal.Row, oGrid)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_DOUBLE_CLICK
                                If pVal.ItemUID = "12" And pVal.ColUID = "U_Z_Attachment" Then
                                    oGrid = oForm.Items.Item("12").Specific
                                    Dim strPath As String = oGrid.DataTable.Columns.Item("U_Z_Attachment").Cells.Item(pVal.Row).Value.ToString()
                                    FileOpen()
                                    If strFilepath = "" Then
                                        oApplication.Utilities.Message("Please Select a File", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                        BubbleEvent = False
                                    Else
                                        oGrid.DataTable.Columns.Item("U_Z_Attachment").Cells.Item(pVal.Row).Value = strFilepath

                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1 As String
                                Dim sCHFL_ID, val, val2, strRefCode, val3, val4, val5, Val6, Val7 As String
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
                                        'If pVal.ItemUID = "12" And pVal.ColUID = "U_Z_TraCode" Then
                                        '    val1 = oDataTable.GetValue("U_Z_TraCode", 0)
                                        '    val = oDataTable.GetValue("U_Z_TraName", 0)
                                        '    oGrid = oForm.Items.Item("12").Specific
                                        '    oGrid.DataTable.SetValue("U_Z_TraDesc", pVal.Row, val)
                                        '    oGrid.DataTable.SetValue("U_Z_TraCode", pVal.Row, val1)
                                        'End If
                                        If pVal.ItemUID = "12" And pVal.ColUID = "U_Z_ExpType" Then
                                            val1 = oDataTable.GetValue("U_Z_ExpName", 0)
                                            val2 = oDataTable.GetValue("U_Z_AlloCode", 0)
                                            val3 = oDataTable.GetValue("U_Z_DebitCode", 0)
                                            val4 = oDataTable.GetValue("U_Z_ActCode", 0)
                                            val5 = oDataTable.GetValue("U_Z_Posting", 0)
                                            Val6 = oDataTable.GetValue("U_Z_Amount", 0)
                                            Val7 = oDataTable.GetValue("U_Z_OverLap", 0)
                                            Dim strCurrency As String = LocalCurrency
                                            Try
                                                oGrid.DataTable.SetValue("U_Z_Currency", pVal.Row, strCurrency)
                                            Catch ex As Exception

                                            End Try
                                            Try
                                                '  ocombo = oDataTable.Columns.Item("U_Z_Reimburse")
                                                oGrid.DataTable.SetValue("U_Z_Reimburse", pVal.Row, "Y")
                                            Catch ex As Exception

                                            End Try
                                            If val5 = "" Then
                                                val5 = "G"
                                            End If
                                            val = oDataTable.GetValue("Code", 0)
                                            oGrid = oForm.Items.Item("12").Specific
                                            oGrid.DataTable.SetValue("U_Z_CurAmt", pVal.Row, oApplication.Utilities.getDocumentQuantity(Val6))
                                            oGrid.DataTable.SetValue("U_Z_DebitCode", pVal.Row, val3)
                                            oGrid.DataTable.SetValue("U_Z_OverLap", pVal.Row, Val7)
                                            oGrid.DataTable.SetValue("U_Z_CreditCode", pVal.Row, val4)
                                            oGrid.DataTable.SetValue("U_Z_Posting", pVal.Row, val5)
                                            oGrid.DataTable.SetValue("U_Z_AlloCode", pVal.Row, val2)
                                            oGrid.DataTable.SetValue("U_Z_ExpType", pVal.Row, val1)
                                            oGrid.DataTable.SetValue("U_Z_Dimension", pVal.Row, BindDimension(oApplication.Utilities.getEdittextvalue(oForm, "15")))
                                            oGrid.DataTable.SetValue("U_Z_ExpCode", pVal.Row, val)
                                        End If
                                        oForm.Freeze(False)
                                    End If
                                Catch ex As Exception
                                    oForm.Freeze(False)
                                End Try
                                oForm.Freeze(False)
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
                Case mnu_InvSO
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
                Case mnu_ADD, mnu_FIND
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If oForm.TypeEx = frm_hr_ExpenseClaim And pVal.BeforeAction = True Then
                        oApplication.Utilities.Message("This functionality not applicable for this module. To add new claim click on Add Row", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                        BubbleEvent = False
                        Exit Sub
                    End If
                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("12").Specific
                    If pVal.BeforeAction = False Then
                        AddEmptyRow(oGrid)
                    End If

                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("12").Specific
                    If pVal.BeforeAction = True Then
                        RemoveRow(1, oGrid)
                        BubbleEvent = False
                        Exit Sub
                    End If
               
                Case mnu_FIND
                    If pVal.BeforeAction = False Then
                        oForm = oApplication.SBO_Application.Forms.ActiveForm()
                        oForm.Items.Item("1000003").Enabled = True
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
                oForm = oApplication.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID)
                If oForm.TypeEx = frm_hr_ExpenseClaim And BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True Then
                    Gridbind()
                End If
            ElseIf BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD Or BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE) Then
                Dim oobj As SAPbobsCOM.EmployeesInfo
                Dim strcode As String
                oApplication.Company.GetNewObjectCode(strcode)
                oobj = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oEmployeesInfo)
                If 1 = 1 Then ' oobj.Browser.GetByKeys(BusinessObjectInfo.ObjectKey) Then
                  
                    Gridbind()
                End If
                'CommitTransaction("Add")
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub AddMode(ByVal aform As SAPbouiCOM.Form)
        Dim strCode As String
        strCode = "New" 'oApplication.Utilities.getMaxCode("@Z_HR_OEXPCL", "Code")
        aform.Items.Item("10").Enabled = True
        oApplication.Utilities.setEdittextvalue(aform, "23", strCode)
        aform.Items.Item("10").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oApplication.Utilities.setEdittextvalue(aform, "10", "t")
        oApplication.SBO_Application.SendKeys("{TAB}")
        aform.Items.Item("19").Enabled = True
        aform.Items.Item("19").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        aform.Items.Item("10").Enabled = False
        aform.Items.Item("23").Enabled = False
        oApplication.Utilities.setEdittextvalue(aform, "19", "")
        oApplication.Utilities.setEdittextvalue(aform, "21", "")
    End Sub
End Class
