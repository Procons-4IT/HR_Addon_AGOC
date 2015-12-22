Imports System.Globalization
Public Class clshrLeaveApproval
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox, oCombobox1, oCombobox2 As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oFolder, oFolder1 As SAPbouiCOM.Folder
    Private ocombo As SAPbouiCOM.ComboBoxColumn
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
    Public Sub LoadForm(ByVal oForm As SAPbouiCOM.Form)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_LeaveApproval) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_LeaveApproval, frm_hr_LeaveApproval)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.PaneLevel = 1
        oCombobox1 = oForm.Items.Item("13").Specific
        oCombobox = oForm.Items.Item("15").Specific
        oCombobox.ValidValues.Add("0", "")
        For j As Integer = 2010 To 2050
            Dim year As String = j
            oCombobox.ValidValues.Add(year, year)
        Next
        oCombobox1.ValidValues.Add("0", "")
        For i As Integer = 1 To 12
            Dim info As DateTimeFormatInfo = DateTimeFormatInfo.GetInstance(Nothing)
            oCombobox1.ValidValues.Add(i, info.GetMonthName(i))
        Next
        oForm.DataSources.DataTables.Add("dtDocumentList")
        oForm.DataSources.DataTables.Add("dtHistoryList")
        oApplication.Utilities.InitializationApproval(oForm, HeaderDoctype.LveReq, HistoryDoctype.LveReq)
        oApplication.Utilities.ApprovalSummary(oForm, HeaderDoctype.LveReq, HistoryDoctype.LveReq)
        oGrid = oForm.Items.Item("1").Specific
        oGrid.Columns.Item("RowsHeader").Click(0, False, False)
        oGrid = oForm.Items.Item("19").Specific
        oGrid.Columns.Item("RowsHeader").Click(0, False, False)
        oForm.Items.Item("4").TextStyle = 7
        oForm.Items.Item("5").TextStyle = 7
        oForm.Freeze(False)
    End Sub
    Private Sub ComboSelect(ByVal sForm As SAPbouiCOM.Form, ByVal Status As String)
        Try
            oCombobox1 = oForm.Items.Item("13").Specific
            oCombobox2 = oForm.Items.Item("15").Specific
            ' oCombobox1.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
            ' oCombobox2.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
        Catch ex As Exception

        End Try
    End Sub
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_LeaveApproval Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "_1" Then
                                    If oApplication.Utilities.ApprovalValidation(oForm, HistoryDoctype.LveReq) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If (pVal.ItemUID = "1" Or pVal.ItemUID = "19") And pVal.ColUID = "Code" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcode As String = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    Dim objHistory As New clshrLeaveRequest
                                    objHistory.ViewPopulateDetails(oForm, strcode, "A")
                                End If
                        End Select
                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "8" Then
                                    oCombobox = oForm.Items.Item("8").Specific
                                    If oCombobox.Selected.Value = "A" Then
                                        oForm.Items.Item("13").Enabled = True
                                        oForm.Items.Item("15").Enabled = True
                                        ComboSelect(oForm, oCombobox.Selected.Value)
                                    Else

                                        oForm.Items.Item("13").Enabled = False
                                        oForm.Items.Item("15").Enabled = False
                                        ComboSelect(oForm, oCombobox.Selected.Value)
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "16" Then
                                    oForm.PaneLevel = 1
                                    oGrid = oForm.Items.Item("1").Specific
                                    oGrid.Columns.Item("RowsHeader").Click(0)
                                ElseIf pVal.ItemUID = "17" Then
                                    oForm.PaneLevel = 2
                                    oGrid = oForm.Items.Item("19").Specific
                                    oGrid.Columns.Item("RowsHeader").Click(0)
                                End If
                                If pVal.ItemUID = "1" And pVal.ColUID = "RowsHeader" And pVal.Row > -1 Then
                                    oGrid = oForm.Items.Item("1").Specific
                                    Dim strDocEntry As String = oGrid.DataTable.GetValue("Code", pVal.Row)
                                    oApplication.Utilities.LoadHistory(oForm, HistoryDoctype.LveReq, strDocEntry)
                                    '  oApplication.Utilities.LoadLeaveRemarks(oForm, pVal.Row)
                                ElseIf (pVal.ItemUID = "3" And pVal.ColUID = "RowsHeader" And pVal.Row > -1) Then
                                    oApplication.Utilities.LoadLeaveRemarks(oForm, pVal.Row)
                                ElseIf pVal.ItemUID = "_1" Then
                                    Dim intRet As Integer = oApplication.SBO_Application.MessageBox("Are you sure want to submit the document?", 2, "Yes", "No", "")
                                    If intRet = 1 Then
                                        oApplication.Utilities.addUpdateDocument(oForm, HistoryDoctype.LveReq, HeaderDoctype.LveReq)
                                    End If
                                End If
                                If pVal.ItemUID = "19" And pVal.ColUID = "RowsHeader" And pVal.Row > -1 Then
                                    oGrid = oForm.Items.Item("19").Specific
                                    Dim strDocEntry As String = oGrid.DataTable.GetValue("Code", pVal.Row)
                                    oApplication.Utilities.SummaryHistory(oForm, HistoryDoctype.LveReq, strDocEntry)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                If oForm.State = SAPbouiCOM.BoFormStateEnum.fs_Restore Or oForm.State = SAPbouiCOM.BoFormStateEnum.fs_Maximized Then
                                    oApplication.Utilities.Resize(oForm)
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
                Case mnu_HR_LveApproval
                    LoadForm(oForm)
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
