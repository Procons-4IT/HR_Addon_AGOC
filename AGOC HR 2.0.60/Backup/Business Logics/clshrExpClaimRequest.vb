Imports System.IO
Public Class clshrExpClaimRequest
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oComboColumn, oComboColumn1 As SAPbouiCOM.ComboBoxColumn
    Private ocombo, ocombo1, ocombo2, ocombo3 As SAPbouiCOM.ComboBoxColumn
    Private oGrid, oAttGrid As SAPbouiCOM.Grid
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
        oForm = oApplication.Utilities.LoadForm(xml_hr_ExpenseClaim, frm_hr_ExpenseClaim)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        AddChooseFromList(oForm)
        oForm.DataSources.UserDataSources.Add("empid", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "15", "empid")
        oForm.DataSources.UserDataSources.Add("empname", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "4", "empname")
        oForm.DataSources.UserDataSources.Add("dt", SAPbouiCOM.BoDataType.dt_DATE)
        oApplication.Utilities.setUserDatabind(oForm, "10", "dt")
        oApplication.Utilities.setEdittextvalue(oForm, "15", EmpCode)
        oApplication.Utilities.setEdittextvalue(oForm, "4", EmpName)
        oForm.Items.Item("10").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oApplication.Utilities.setEdittextvalue(oForm, "10", "t")
        oApplication.SBO_Application.SendKeys("{TAB}")
        oForm.Items.Item("15").Enabled = False
        oForm.Items.Item("4").Enabled = False
        oForm.Items.Item("11").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
        Gridbind(EmpCode)
        oForm.Freeze(False)
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
    Private Sub Gridbind(ByVal Empid As String)
        Try
            Dim strqry, strQuery As String
            Dim oGECol As SAPbouiCOM.EditTextColumn
            oGrid = oForm.Items.Item("12").Specific
            oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
            strqry = "select ""Code"",""Name"",""U_Z_ExpCode"", ""U_Z_Subdt"",""U_Z_TripType"",""U_Z_TraCode"",""U_Z_TraDesc"",""U_Z_ExpType"",""U_Z_AlloCode"",""U_Z_Client"",""U_Z_Project"",""U_Z_Claimdt"",""U_Z_City"",""U_Z_Currency"",""U_Z_CurAmt"",""U_Z_ExcRate"","
            strqry += """U_Z_UsdAmt"",""U_Z_Reimburse"",""U_Z_ReimAmt"",""U_Z_PayMethod"",""U_Z_Notes"",""U_Z_Attachment"",""U_Z_AppStatus"",""U_Z_PayPosted""  from ""@Z_HR_EXPCL""   where ""U_Z_EmpID""='" & oApplication.Utilities.getEdittextvalue(oForm, "15") & "'"
            oGrid.DataTable.ExecuteQuery(strqry)
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("Name").Visible = False
            oGrid.Columns.Item("U_Z_Subdt").TitleObject.Caption = "Submitted Date"
            oGrid.Columns.Item("U_Z_Subdt").Visible = False
            oGrid.Columns.Item("U_Z_Claimdt").TitleObject.Caption = "Date (*)"
            oGrid.Columns.Item("U_Z_Client").TitleObject.Caption = "Client"
            oGrid.Columns.Item("U_Z_Project").TitleObject.Caption = "Project"
            oGrid.Columns.Item("U_Z_TripType").TitleObject.Caption = "Trip Type (*)"
            oGrid.Columns.Item("U_Z_TripType").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oComboColumn = oGrid.Columns.Item("U_Z_TripType")
            oComboColumn.ValidValues.Add("", "")
            oComboColumn.ValidValues.Add("E", "Existing")
            oComboColumn.ValidValues.Add("N", "New")
            oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_TraCode").TitleObject.Caption = "Travel Code"
            oEditTextColumn = oGrid.Columns.Item("U_Z_TraCode")
            'oEditTextColumn.ChooseFromListUID = "CFL1"
            'oEditTextColumn.ChooseFromListAlias = "U_Z_TraCode"
            oEditTextColumn.LinkedObjectType = "Z_HR_OTRAPLA"
            oGrid.Columns.Item("U_Z_TraDesc").TitleObject.Caption = "Travel Description (*)"
            oGrid.Columns.Item("U_Z_TraDesc").Editable = True
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
            oGrid.Columns.Item("U_Z_PayPosted").TitleObject.Caption = "Posted to Payroll"
            oGrid.Columns.Item("U_Z_PayPosted").Editable = False
            oGrid.AutoResizeColumns()
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                oGrid.RowHeaders.SetText(intRow, intRow + 1)
            Next

            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid)
        Try
            If aGrid.DataTable.GetValue("U_Z_ExpType", aGrid.DataTable.Rows.Count - 1) <> "" Then
                aGrid.DataTable.Rows.Add()
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
        Dim otemprec As SAPbobsCOM.Recordset
        For intRow = 0 To agrid.DataTable.Rows.Count - 1
            If agrid.Rows.IsSelected(intRow) Then
                strCode = agrid.DataTable.GetValue(0, intRow)
                strname = agrid.DataTable.GetValue(1, intRow)
                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oApplication.Utilities.ExecuteSQL(otemprec, "update ""@Z_HR_EXPCL"" set  ""Name"" =""Name"" +'D'  where ""Code""='" & strCode & "'")
                agrid.DataTable.Rows.Remove(intRow)
                Exit Sub
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
#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        aform.Freeze(True)
        Try
            Dim oUserTable As SAPbobsCOM.UserTable
            Dim strCode, strType As String
            oGrid = aform.Items.Item("12").Specific
            oUserTable = oApplication.Company.UserTables.Item("Z_HR_EXPCL")
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
                      
                        oComboColumn = oGrid.Columns.Item("U_Z_TripType")
                        oUserTable.UserFields.Fields.Item("U_Z_TripType").Value = oComboColumn.GetSelectedValue(intRow).Value
                        ocombo = oGrid.Columns.Item("U_Z_Reimburse")
                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_Reimburse").Value = ocombo.GetSelectedValue(intRow).Value
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_Reimburse").Value = "N"
                        End Try
                        ocombo1 = oGrid.Columns.Item("U_Z_PayMethod")
                        oUserTable.UserFields.Fields.Item("U_Z_PayMethod").Value = ocombo1.GetSelectedValue(intRow).Value
                        ocombo2 = oGrid.Columns.Item("U_Z_AppStatus")
                        oUserTable.UserFields.Fields.Item("U_Z_AppStatus").Value = ocombo2.GetSelectedValue(intRow).Value
                        oUserTable.UserFields.Fields.Item("U_Z_ExpType").Value = oGrid.DataTable.GetValue("U_Z_ExpType", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_ExpCode").Value = oGrid.DataTable.GetValue("U_Z_ExpCode", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_TraCode").Value = oGrid.DataTable.GetValue("U_Z_TraCode", intRow)
                        ocombo3 = oGrid.Columns.Item("U_Z_Currency")
                        oUserTable.UserFields.Fields.Item("U_Z_Currency").Value = ocombo3.GetSelectedValue(intRow).Value
                        oUserTable.UserFields.Fields.Item("U_Z_Client").Value = oGrid.DataTable.GetValue("U_Z_Client", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_AlloCode").Value = oGrid.DataTable.GetValue("U_Z_AlloCode", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Project").Value = oGrid.DataTable.GetValue("U_Z_Project", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_TraDesc").Value = oGrid.DataTable.GetValue("U_Z_TraDesc", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_City").Value = oGrid.DataTable.GetValue("U_Z_City", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_CurAmt").Value = oGrid.DataTable.GetValue("U_Z_CurAmt", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_ExcRate").Value = oGrid.DataTable.GetValue("U_Z_ExcRate", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_UsdAmt").Value = oGrid.DataTable.GetValue("U_Z_UsdAmt", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_ReimAmt").Value = oGrid.DataTable.GetValue("U_Z_ReimAmt", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Notes").Value = oGrid.DataTable.GetValue("U_Z_Notes", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Attachment").Value = oGrid.DataTable.GetValue("U_Z_Attachment", intRow)
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
                            oApplication.Utilities.AddtoUDT1_PayrollTrans(strCode)
                           
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
                        oUserTable.UserFields.Fields.Item("U_Z_TripType").Value = oComboColumn.GetSelectedValue(intRow).Value
                        ocombo = oGrid.Columns.Item("U_Z_Reimburse")
                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_Reimburse").Value = ocombo.GetSelectedValue(intRow).Value
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_Reimburse").Value = "N"
                        End Try

                        ocombo1 = oGrid.Columns.Item("U_Z_PayMethod")
                        ocombo3 = oGrid.Columns.Item("U_Z_Currency")
                        oUserTable.UserFields.Fields.Item("U_Z_Currency").Value = ocombo3.GetSelectedValue(intRow).Value
                        oUserTable.UserFields.Fields.Item("U_Z_PayMethod").Value = ocombo1.GetSelectedValue(intRow).Value
                        oUserTable.UserFields.Fields.Item("U_Z_ExpType").Value = oGrid.DataTable.GetValue("U_Z_ExpType", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_ExpCode").Value = oGrid.DataTable.GetValue("U_Z_ExpCode", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_TraCode").Value = oGrid.DataTable.GetValue("U_Z_TraCode", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Client").Value = oGrid.DataTable.GetValue("U_Z_Client", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_AlloCode").Value = oGrid.DataTable.GetValue("U_Z_AlloCode", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Project").Value = oGrid.DataTable.GetValue("U_Z_Project", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_TraDesc").Value = oGrid.DataTable.GetValue("U_Z_TraDesc", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_City").Value = oGrid.DataTable.GetValue("U_Z_City", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_CurAmt").Value = oGrid.DataTable.GetValue("U_Z_CurAmt", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_ExcRate").Value = oGrid.DataTable.GetValue("U_Z_ExcRate", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_UsdAmt").Value = oGrid.DataTable.GetValue("U_Z_UsdAmt", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_ReimAmt").Value = oGrid.DataTable.GetValue("U_Z_ReimAmt", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Notes").Value = oGrid.DataTable.GetValue("U_Z_Notes", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_Attachment").Value = oGrid.DataTable.GetValue("U_Z_Attachment", intRow)
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
                            oApplication.Utilities.AddtoUDT1_PayrollTrans(strCode)
                            Dim intTempID As String = oApplication.Utilities.GetTemplateID(aform, HeaderDoctype.ExpCli, oApplication.Utilities.getEdittextvalue(aform, "15"))
                            If intTempID <> "0" Then
                                oApplication.Utilities.InitialMessage("Expense Claim", strCode, oApplication.Utilities.DocApproval(aform, HeaderDoctype.ExpCli, oApplication.Utilities.getEdittextvalue(aform, "15")), intTempID, oApplication.Utilities.getEdittextvalue(aform, "4"), HistoryDoctype.ExpCli)
                            End If
                        End If
                    End If
                End If
            Next
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
            Gridbind(oApplication.Utilities.getEdittextvalue(aform, "15"))
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
            Dim strdate, Tracode, Exptype, Cur, Client, Project, Triptype, Tradesc As String
            Dim currency As Double
            oGrid = sForm.Items.Item("12").Specific
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
                    Triptype = oComboColumn.GetSelectedValue(intRow).Value
                    Tradesc = oGrid.DataTable.GetValue("U_Z_TraDesc", intRow)
                    If Triptype <> "" Then
                        If Triptype = "E" Then
                            If Tracode = "" Then
                                oApplication.Utilities.Message("Travel Code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                Return False
                            End If
                        ElseIf Triptype = "N" Then
                            If Tradesc = "" Then
                                oApplication.Utilities.Message("Travel Description is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                Return False
                            End If
                        End If
                    Else
                        oApplication.Utilities.Message("Trip Type is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
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
                        Return False
                    ElseIf strdate = "" Then
                        oApplication.Utilities.Message("Transaction date is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    ElseIf Cur = "" Then
                        oApplication.Utilities.Message("Transaction Currency is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    ElseIf currency = 0.0 Then
                        oApplication.Utilities.Message("Transaction Amount is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
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
            oProcesses = Process.GetProcessesByName("SAP Business One")
            If oProcesses.Length <> 0 Then
                For i As Integer = 0 To oProcesses.Length - 1
                    Dim MyWindow As New clsListener.WindowWrapper(oProcesses(i).MainWindowHandle)
                    If oDialogBox.ShowDialog(MyWindow) = DialogResult.OK Then
                        strMdbFilePath = oDialogBox.FileName
                        strFilepath = oDialogBox.FileName
                    Else
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
                                If pVal.ItemUID = "12" And pVal.ColUID = "U_Z_TraCode" Then
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
                                If pVal.ItemUID = "12" And pVal.ColUID = "U_Z_ExpType" Then
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
                                If pVal.ItemUID = "12" And (pVal.ColUID = "U_Z_TraDesc" Or pVal.ColUID = "U_Z_TraCode") Then
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
                                If pVal.ItemUID = "12" Then
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
                                If pVal.ItemUID = "12" And (pVal.ColUID = "U_Z_TraDesc" Or pVal.ColUID = "U_Z_TraCode") Then
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
                                    dblusd = dblcur / dblexrate
                                    oGrid.DataTable.SetValue("U_Z_UsdAmt", pVal.Row, LocalCurrency & Math.Round(dblusd, 2))
                                    ocombo = oGrid.Columns.Item("U_Z_Reimburse")
                                    Dim strst As String = ocombo.GetSelectedValue(pVal.Row).Value
                                    If ocombo.GetSelectedValue(pVal.Row).Value = "Y" Then
                                        oGrid.DataTable.SetValue("U_Z_ReimAmt", pVal.Row, LocalCurrency & Math.Round(dblusd, 2))
                                    End If
                                End If
                                If pVal.ItemUID = "12" And pVal.ColUID = "U_Z_CurAmt" And pVal.CharPressed = 9 Then
                                    dblcur = oGrid.DataTable.GetValue("U_Z_CurAmt", pVal.Row)
                                    dblexrate = oGrid.DataTable.GetValue("U_Z_ExcRate", pVal.Row)
                                    dblusd = dblcur / dblexrate
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

                                If pVal.ItemUID = "12" And pVal.ColUID = "U_Z_Reimburse" Then
                                    ocombo = oGrid.Columns.Item("U_Z_Reimburse")
                                    If ocombo.GetSelectedValue(pVal.Row).Value = "Y" Then
                                        oGrid.DataTable.SetValue("U_Z_ReimAmt", pVal.Row, oGrid.DataTable.GetValue("U_Z_UsdAmt", pVal.Row))
                                    Else
                                        oGrid.DataTable.SetValue("U_Z_ReimAmt", pVal.Row, LocalCurrency & "0.0")
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
                                    oGrid = oForm.Items.Item("12").Specific
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
                                Dim sCHFL_ID, val, val2, strRefCode As String
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
                                            val = oDataTable.GetValue("Code", 0)
                                            oGrid = oForm.Items.Item("12").Specific
                                            oGrid.DataTable.SetValue("U_Z_ExpCode", pVal.Row, val)
                                            oGrid.DataTable.SetValue("U_Z_AlloCode", pVal.Row, val2)
                                            oGrid.DataTable.SetValue("U_Z_ExpType", pVal.Row, val1)

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
