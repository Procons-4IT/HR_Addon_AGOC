Public Class clshrAssExpenses
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
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
    Public Sub LoadForm(ByVal Empid As String, ByVal TraCode As String, ByVal EmpName As String, ByVal TraName As String, ByVal RefCode As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_AssExpenses) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_AssExpenses, frm_hr_AssExpenses)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oApplication.Utilities.setEdittextvalue(oForm, "4", Empid)
        oApplication.Utilities.setEdittextvalue(oForm, "6", EmpName)
        oApplication.Utilities.setEdittextvalue(oForm, "8", TraCode)
        oApplication.Utilities.setEdittextvalue(oForm, "10", TraName)
        oApplication.Utilities.setEdittextvalue(oForm, "12", RefCode)
        Databind(oForm, Empid, TraCode)
        oForm.Freeze(False)
    End Sub
    Private Sub Databind(ByVal aForm As SAPbouiCOM.Form, ByVal empid As String, ByVal TraCode As String)
        Dim strqry As String
        oForm = aForm
        Try
            oForm.Freeze(True)
            oGrid = oForm.Items.Item("13").Specific
            oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
            strqry = "select Code,Name,U_Z_EmpId,U_Z_TraCode,U_Z_ExpName,U_Z_ActCode,U_Z_LocCurrency,U_Z_Amount,U_Z_UtilAmt,U_Z_BalAmount,U_Z_RefCode from [@Z_HR_ASSTP1]"
            strqry = strqry & " where U_Z_EmpId=" & empid & " and U_Z_TraCode='" & TraCode & "' "
            oGrid.DataTable.ExecuteQuery(strqry)
            oGrid.Columns.Item("Code").TitleObject.Caption = "Code"
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("Name").TitleObject.Caption = "Name"
            oGrid.Columns.Item("Name").Visible = False
            oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee Id"
            oGrid.Columns.Item("U_Z_EmpId").Visible = False
            oGrid.Columns.Item("U_Z_TraCode").TitleObject.Caption = "Travel Plan Code"
            oGrid.Columns.Item("U_Z_TraCode").Visible = False
            oGrid.Columns.Item("U_Z_ExpName").TitleObject.Caption = "Expenses Name"
            oGrid.Columns.Item("U_Z_ExpName").Editable = False
            oGrid.Columns.Item("U_Z_ActCode").TitleObject.Caption = "Account Code"
            oGrid.Columns.Item("U_Z_ActCode").Visible = False
            oGrid.Columns.Item("U_Z_LocCurrency").TitleObject.Caption = "Local Currency"
            oGrid.Columns.Item("U_Z_LocCurrency").Editable = False
            oGrid.Columns.Item("U_Z_Amount").TitleObject.Caption = "Budget Amount"
            oGrid.Columns.Item("U_Z_Amount").Editable = True
            oGrid.Columns.Item("U_Z_UtilAmt").TitleObject.Caption = "Utilize Amount"
            oGrid.Columns.Item("U_Z_UtilAmt").Editable = False
            oGrid.Columns.Item("U_Z_BalAmount").TitleObject.Caption = "Balance Amount"
            oGrid.Columns.Item("U_Z_BalAmount").Editable = False
            oGrid.Columns.Item("U_Z_RefCode").TitleObject.Caption = "Reference Code"
            oGrid.Columns.Item("U_Z_RefCode").Visible = False
            oGrid.AutoResizeColumns()
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
            oForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
#Region "AddToUDT"
    Private Function AddToUDT(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim strTable, strCode, strqry As String
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim oValidateRS, otemp2 As SAPbobsCOM.Recordset
        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oGrid = aForm.Items.Item("13").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strCode = oGrid.DataTable.GetValue("Code", intRow)
            strqry = "Update [@Z_HR_ASSTP1] set U_Z_Amount='" & oGrid.DataTable.GetValue("U_Z_Amount", intRow) & "' where Code='" & strCode & "'"
            oValidateRS.DoQuery(strqry)
        Next
        Return True
    End Function
#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_AssExpenses Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD

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
                                    If AddToUDT(oForm) = True Then
                                        oApplication.Utilities.Message("Operation Completed Successfully...", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
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
                Case mnu_InvSO
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
