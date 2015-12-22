Public Class clshrExpClaimView
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
    Private ocombo As SAPbouiCOM.ComboBoxColumn
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Public Sub LoadForm(ByVal empid As String, ByVal empname As String)
        oForm = oApplication.Utilities.LoadForm(xml_hr_ExpClaimView, frm_hr_ExpClaimView)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oApplication.Utilities.setEdittextvalue(oForm, "5", empid)
        oApplication.Utilities.setEdittextvalue(oForm, "7", empname)
        Databind(empid)
        oForm.Freeze(False)
    End Sub
    Private Sub Databind(ByVal strempid As String)
        Dim strqry As String
        oGrid = oForm.Items.Item("3").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
        strqry = "SELECT T0.""Code"",T0.""U_Z_EmpID"",T0.""U_Z_EmpName"" as 'Employee Name',T0.""U_Z_Subdt"", T0.""U_Z_Client"", T0.""U_Z_Project"","
        strqry += " Case T0.U_Z_DocStatus when 'C' then 'Closed' else 'Opened' end as 'Document Status' FROM ""@Z_HR_OEXPCL"" T0 WHERE T0.""U_Z_EmpID"" ='" & strempid & "'"
        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("Code").TitleObject.Caption = "Expenses Claim  Number"
        oEditTextColumn = oGrid.Columns.Item("Code")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Invoice
        oGrid.Columns.Item("U_Z_EmpID").TitleObject.Caption = "Employee Code"
        oEditTextColumn = oGrid.Columns.Item("U_Z_EmpID")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
        oGrid.Columns.Item("U_Z_EmpID").Visible = False
        oGrid.Columns.Item("U_Z_Subdt").TitleObject.Caption = "Submitted Date"
        oGrid.Columns.Item("U_Z_Client").TitleObject.Caption = "Client"
        oGrid.Columns.Item("U_Z_Project").TitleObject.Caption = "Project"
        oGrid.AutoResizeColumns()
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            oGrid.RowHeaders.SetText(intRow, intRow + 1)
        Next
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_ExpClaimView Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim strCode As String
                                If pVal.ItemUID = "3" And pVal.ColUID = "Code" Then
                                    oGrid = oForm.Items.Item("3").Specific
                                    For intRow As Integer = pVal.Row To pVal.Row
                                        If 1 = 1 Then
                                            strCode = oGrid.DataTable.GetValue("Code", oGrid.GetDataTableRowIndex(pVal.Row))
                                            Dim objct As New clshrExpClaimRequest
                                            objct.LoadForm1(strCode)
                                        End If
                                    Next
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                        End Select
                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
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
