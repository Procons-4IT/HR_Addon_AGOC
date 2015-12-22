Imports System.Globalization

Public Class clshrExpClaimPosting

    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oGridCombo, oGridCombo1, oGridCombo2 As SAPbouiCOM.ComboBoxColumn
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
    Dim StrCode, Posting, Reimbuse As String
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Public Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_ExpClaimPost) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_ExpClaimPost, frm_hr_ExpClaimPost)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.DataSources.UserDataSources.Add("From", SAPbouiCOM.BoDataType.dt_DATE)
        oApplication.Utilities.setUserDatabind(oForm, "5", "From")
        oForm.DataSources.UserDataSources.Add("To", SAPbouiCOM.BoDataType.dt_DATE)
        oApplication.Utilities.setUserDatabind(oForm, "7", "To")
        oForm.DataSources.UserDataSources.Add("PGL", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDSCombobox(oForm, "9", "PGL")
        oCombobox = oForm.Items.Item("9").Specific
        oCombobox.ValidValues.Add("", "")
        oCombobox.ValidValues.Add("P", "Payroll")
        oCombobox.ValidValues.Add("G", "G/L Account")
        oForm.Items.Item("9").DisplayDesc = True
        oForm.PaneLevel = 1
        oForm.Freeze(False)
    End Sub
    Private Sub Gridbind(ByVal aForm As SAPbouiCOM.Form)
        Dim strqry, strFrom, strTo, strPosting As String
        Dim strDateCondition, strPostcondition, strCondition As String
        Dim dtFrom, dtTo As Date

        strFrom = oApplication.Utilities.getEdittextvalue(aForm, "5")
        strTo = oApplication.Utilities.getEdittextvalue(aForm, "7")
        If strFrom <> "" Then
            dtFrom = oApplication.Utilities.GetDateTimeValue(strFrom)
        End If
        If strTo <> "" Then
            dtTo = oApplication.Utilities.GetDateTimeValue(strTo)
        End If
        oCombobox = aForm.Items.Item("9").Specific
        strPosting = oCombobox.Selected.Value


        If strFrom <> "" And strTo <> "" Then
            strDateCondition = "U_Z_SubDt between '" & dtFrom.ToString("yyyy-MM-dd") & "' and '" & dtTo.ToString("yyyy-MM-dd") & "'"
        ElseIf strFrom <> "" And strTo = "" Then
            strDateCondition = "U_Z_SubDt >= '" & dtFrom.ToString("yyyy-MM-dd") & "'"
        ElseIf strFrom = "" And strTo <> "" Then
            strDateCondition = "U_Z_SubDt <= '" & dtTo.ToString("yyyy-MM-dd") & "'"
        Else
            strDateCondition = " 1=1"
        End If
        If strPosting <> "" Then
            strPostcondition = "isnull(U_Z_Posting,'P')='" & strPosting & "'"
        Else
            strPostcondition = "1=1"
        End If

        strCondition = strDateCondition & " and " & strPostcondition & " Order by U_Z_DocRefNo,Code,U_Z_EmpID Desc"
        oGrid = aForm.Items.Item("11").Specific

        strqry = " Select '',U_Z_DocRefNo,Code,U_Z_EmpID,U_Z_EmpName,U_Z_SubDt,U_Z_Client,U_Z_Project,U_Z_TraDesc,U_Z_Claimdt,U_Z_ExpType,U_Z_Reimburse,U_Z_Currency,U_Z_CurAmt,U_Z_UsdAmt,U_Z_ReimAmt,"
        strqry += " isnull(U_Z_Posting,'P') as U_Z_Posting,Isnull(U_Z_AppStatus,'P') AS U_Z_AppStatus,Convert(Varchar(10),isnull(""U_Z_Month"",MONTH(U_Z_Claimdt))) AS ""U_Z_Month"",Convert(Varchar(10),isnull(""U_Z_Year"",YEAR(U_Z_Claimdt))) AS ""U_Z_Year"""
        strqry += " From [@Z_HR_EXPCL]  where U_Z_AppStatus='A' and isnull(U_Z_PayPosted,'N')='N' and " & strCondition

        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item(0).TitleObject.Caption = "Select"
        oGrid.Columns.Item(0).Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        oGrid.Columns.Item(0).Editable = True
        oGrid.Columns.Item("U_Z_DocRefNo").TitleObject.Caption = "Expense Claim No."
        oGrid.Columns.Item("U_Z_DocRefNo").Editable = False
        oEditTextColumn = oGrid.Columns.Item("U_Z_DocRefNo")
        oEditTextColumn.LinkedObjectType = "Z_HR_OEXFOM"
        oGrid.Columns.Item("Code").TitleObject.Caption = "Serial No."
        oGrid.Columns.Item("Code").Editable = False
        oGrid.Columns.Item("U_Z_EmpID").TitleObject.Caption = "Employee"
        oEditTextColumn = oGrid.Columns.Item("U_Z_EmpID")
        oEditTextColumn.LinkedObjectType = "171"
        oGrid.Columns.Item("U_Z_EmpID").Editable = False
        oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
        oGrid.Columns.Item("U_Z_EmpName").Editable = False
        oGrid.Columns.Item("U_Z_SubDt").TitleObject.Caption = "Submitted Date"
        oGrid.Columns.Item("U_Z_SubDt").Editable = False
        oGrid.Columns.Item("U_Z_TraDesc").TitleObject.Caption = "Travel Description"
        oGrid.Columns.Item("U_Z_TraDesc").Editable = False
        oGrid.Columns.Item("U_Z_Claimdt").TitleObject.Caption = "Transaction Date"
        oGrid.Columns.Item("U_Z_Claimdt").Editable = False
        oGrid.Columns.Item("U_Z_ExpType").TitleObject.Caption = "Expense Type"
        oEditTextColumn = oGrid.Columns.Item("U_Z_ExpType")
        oEditTextColumn.LinkedObjectType = "Z_HR_EXPANCES"
        oGrid.Columns.Item("U_Z_ExpType").Editable = False
        oGrid.Columns.Item("U_Z_Currency").TitleObject.Caption = "Transaction Currency"
        oGrid.Columns.Item("U_Z_Currency").Editable = False
        oGrid.Columns.Item("U_Z_Client").TitleObject.Caption = "Client"
        oGrid.Columns.Item("U_Z_Client").Editable = False
        oGrid.Columns.Item("U_Z_Project").TitleObject.Caption = "Project"
        oGrid.Columns.Item("U_Z_Project").Editable = False
        oGrid.Columns.Item("U_Z_CurAmt").TitleObject.Caption = "Transaction Amount"
        oGrid.Columns.Item("U_Z_CurAmt").Editable = False
        oGrid.Columns.Item("U_Z_UsdAmt").TitleObject.Caption = "Local Currency Amount"
        oGrid.Columns.Item("U_Z_UsdAmt").Editable = False
        oGrid.Columns.Item("U_Z_ReimAmt").TitleObject.Caption = "Redim Amount"
        oGrid.Columns.Item("U_Z_ReimAmt").Editable = False
        oGrid.Columns.Item("U_Z_Posting").TitleObject.Caption = "Posting To (Payroll/GL)"
        oGrid.Columns.Item("U_Z_Posting").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGridCombo = oGrid.Columns.Item("U_Z_Posting")
        oGridCombo.ValidValues.Add("P", "Payroll")
        oGridCombo.ValidValues.Add("G", "G/L Account")
        oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
        oGrid.Columns.Item("U_Z_Posting").Editable = False
        oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
        oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
        oGridCombo.ValidValues.Add("P", "Pending")
        oGridCombo.ValidValues.Add("A", "Approved")
        oGridCombo.ValidValues.Add("R", "Rejected")
        oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
        oGrid.Columns.Item("U_Z_AppStatus").Editable = False
        oGrid.Columns.Item("U_Z_Month").TitleObject.Caption = "Payroll Month"
        oGrid.Columns.Item("U_Z_Month").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGridCombo1 = oGrid.Columns.Item("U_Z_Month")
        oGridCombo1.ValidValues.Add("0", "")
        For i As Integer = 1 To 12
            Dim info As DateTimeFormatInfo = DateTimeFormatInfo.GetInstance(Nothing)
            oGridCombo1.ValidValues.Add(i, info.GetMonthName(i))
        Next
        oGridCombo1.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
        oGrid.Columns.Item("U_Z_Month").Editable = False
        oGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Payroll Year"
        oGrid.Columns.Item("U_Z_Year").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGridCombo2 = oGrid.Columns.Item("U_Z_Year")
        oGridCombo2.ValidValues.Add("0", "")
        For j As Integer = 2010 To 2050
            Dim year As String = j
            oGridCombo2.ValidValues.Add(year, year)
        Next
        oGridCombo2.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
        oGrid.Columns.Item("U_Z_Year").Editable = False
        oGrid.Columns.Item("U_Z_Reimburse").Visible = False
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oGrid.AutoResizeColumns()
    End Sub
    Private Function ExpensesPosting(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim oCheckbox As SAPbouiCOM.CheckBoxColumn
        Try
            oGrid = aForm.Items.Item("11").Specific
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                oCheckbox = oGrid.Columns.Item(0)
                If oCheckbox.IsChecked(intRow) Then
                    StrCode = oGrid.DataTable.GetValue("Code", intRow)
                    Posting = oGrid.DataTable.GetValue("U_Z_Posting", intRow)
                    Reimbuse = oGrid.DataTable.GetValue("U_Z_Reimburse", intRow)
                    If Posting = "P" Then
                        oApplication.Utilities.AddtoUDT1_PayrollTrans(StrCode)
                    ElseIf Posting = "G" Then
                        oApplication.Utilities.CreateJournelVouchers(StrCode, Reimbuse)
                    End If
                End If
            Next
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
    Private Sub Selectall(ByVal aForm As SAPbouiCOM.Form, ByVal blnValue As Boolean)
        Dim ocheckboxcolumn As SAPbouiCOM.CheckBoxColumn
        Dim ovalue As SAPbouiCOM.ValidValue
        oGrid = aForm.Items.Item("11").Specific
        aForm.Freeze(True)
        For introw As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            ocheckboxcolumn = oGrid.Columns.Item(0)
            ocheckboxcolumn.Check(introw, blnValue)
        Next
        aForm.Freeze(False)
    End Sub
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_ExpClaimPost Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "11" And pVal.ColUID = "U_Z_ExpType" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim ooBj As New clshrExpenses
                                    ooBj.LoadForm()
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "11" And pVal.ColUID = "U_Z_DocRefNo" Then
                                    oGrid = oForm.Items.Item("11").Specific
                                    For intRow As Integer = pVal.Row To pVal.Row
                                        If 1 = 1 Then
                                            StrCode = oGrid.DataTable.GetValue("U_Z_DocRefNo", oGrid.GetDataTableRowIndex(pVal.Row))
                                            Dim objct As New clshrExpClaimRequest
                                            objct.LoadForm1(StrCode)
                                        End If
                                    Next
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = oForm.PaneLevel + 1
                                    If oForm.PaneLevel = 3 Then
                                        Gridbind(oForm)
                                    End If
                                    oForm.Freeze(False)
                                ElseIf pVal.ItemUID = "10" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = oForm.PaneLevel - 1
                                    oForm.Freeze(False)
                                ElseIf pVal.ItemUID = "13" Then
                                    Selectall(oForm, True)
                                ElseIf pVal.ItemUID = "14" Then
                                    Selectall(oForm, False)
                                ElseIf pVal.ItemUID = "12" Then
                                    If oApplication.SBO_Application.MessageBox("Do you want confirm the Expense posting", , "Yes", "No") = 2 Then
                                        Exit Sub
                                    ElseIf ExpensesPosting(oForm) = True Then
                                        oApplication.Utilities.Message("Expense posted successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                        oForm.Close()
                                    Else
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
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
                Case mnu_ExpClaimPost
                    LoadForm()
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
