Public Class clshrFinalApproval
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
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
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_FinalApproval) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_FinalApproval, frm_hr_FinalApproval)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.PaneLevel = 1
        oForm.DataSources.DataTables.Add("dtDocumentList")
        oForm.DataSources.DataTables.Add("dtHistoryList")
        oApplication.Utilities.InitializationApproval(oForm, HeaderDoctype.Rec, HistoryDoctype.Final)
        oApplication.Utilities.ApprovalSummary(oForm, HeaderDoctype.Rec, HistoryDoctype.Final)
        oGrid = oForm.Items.Item("1").Specific
        oGrid.Columns.Item("RowsHeader").Click(0, False, False)
        oGrid = oForm.Items.Item("19").Specific
        oGrid.Columns.Item("RowsHeader").Click(0, False, False)
        oForm.Items.Item("4").TextStyle = 7
        oForm.Items.Item("5").TextStyle = 7
        oForm.Freeze(False)
    End Sub
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_FinalApproval Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD

                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If (pVal.ItemUID = "1" Or pVal.ItemUID = "19") And pVal.ColUID = "U_Z_HRAppID" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcode As String = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    Dim ooBj As New clshrCrApplicants
                                    ooBj.ViewCandidate(strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If


                                If (pVal.ItemUID = "1" Or pVal.ItemUID = "19") And pVal.ColUID = "U_Z_ReqNo" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcode As String = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    If 1 = 1 Then
                                        Dim objct As New clshrMPRequest
                                        objct.LoadForm1(strcode, "Employment Offer", , , )

                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                        End Select
                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "12" Then
                                    oForm.PaneLevel = 1
                                    oGrid = oForm.Items.Item("1").Specific
                                    oGrid.Columns.Item("RowsHeader").Click(0)
                                ElseIf pVal.ItemUID = "13" Then
                                    oForm.PaneLevel = 2
                                    oGrid = oForm.Items.Item("19").Specific
                                    oGrid.Columns.Item("RowsHeader").Click(0)
                                End If
                                If pVal.ItemUID = "14" And oForm.PaneLevel = 1 Then
                                    oGrid = oForm.Items.Item("1").Specific
                                    For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                        If oGrid.Rows.IsSelected(intRow) Then
                                            Dim objHistory As New clshrAppHisDetails
                                            If oGrid.DataTable.GetValue("DocEntry", intRow) <> 0 Then
                                                objHistory.LoadForm1(oForm, oGrid.DataTable.GetValue("DocEntry", intRow))
                                            End If
                                            Exit Sub
                                        End If
                                    Next
                                End If
                                If pVal.ItemUID = "14" And oForm.PaneLevel = 2 Then
                                    oGrid = oForm.Items.Item("19").Specific
                                    For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                        If oGrid.Rows.IsSelected(intRow) Then
                                            Dim objHistory As New clshrAppHisDetails
                                            If oGrid.DataTable.GetValue("DocEntry", intRow) <> 0 Then
                                                objHistory.LoadForm1(oForm, oGrid.DataTable.GetValue("DocEntry", intRow))
                                            End If
                                            Exit Sub
                                        End If
                                    Next
                                End If
                                If pVal.ItemUID = "1" And pVal.ColUID = "RowsHeader" And pVal.Row > -1 Then
                                    oGrid = oForm.Items.Item("1").Specific
                                    Dim strDocEntry As String = oGrid.DataTable.GetValue("DocEntry", pVal.Row)
                                    oApplication.Utilities.LoadHistory(oForm, HistoryDoctype.Final, strDocEntry)
                                ElseIf (pVal.ItemUID = "3" And pVal.ColUID = "RowsHeader" And pVal.Row > -1) Then
                                    oApplication.Utilities.LoadStatusRemarks(oForm, pVal.Row)
                                ElseIf pVal.ItemUID = "_1" Then
                                    Dim intRet As Integer = oApplication.SBO_Application.MessageBox("Are you sure want to submit the document?", 2, "Yes", "No", "")
                                    If intRet = 1 Then
                                        oApplication.Utilities.addUpdateDocument(oForm, HistoryDoctype.Final, HeaderDoctype.Rec)
                                    End If
                                End If
                                If pVal.ItemUID = "19" And pVal.ColUID = "RowsHeader" And pVal.Row > -1 Then
                                    oGrid = oForm.Items.Item("19").Specific
                                    Dim strDocEntry As String = oGrid.DataTable.GetValue("DocEntry", pVal.Row)
                                    oApplication.Utilities.SummaryHistory(oForm, HistoryDoctype.Final, strDocEntry)
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
                Case mnu_hr_IPHOD
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
