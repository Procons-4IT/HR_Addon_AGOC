Imports System.IO
Imports System.Globalization
Public Class clshrClaimApproval
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox, oCombobox1 As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oGrid As SAPbouiCOM.Grid
    Private dtDocumentList As SAPbouiCOM.DataTable
    Private dtHistoryList As SAPbouiCOM.DataTable
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub

    Public Sub LoadForm(ByVal oForm As SAPbouiCOM.Form)
        Try

            If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_ClaimApproval) = False Then
                oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Exit Sub
            End If
            oForm = oApplication.Utilities.LoadForm(xml_hr_TranApproval, frm_hr_ClaimApproval)
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
            oCombobox = oForm.Items.Item("8").Specific
            oApplication.Utilities.InitializationApproval(oForm, HeaderDoctype.ExpCli, HistoryDoctype.ExpCli)
            oApplication.Utilities.ApprovalSummary(oForm, HeaderDoctype.ExpCli, HistoryDoctype.ExpCli)
            oGrid = oForm.Items.Item("1").Specific
            oGrid.Columns.Item("RowsHeader").Click(0, False, False)
            oGrid = oForm.Items.Item("19").Specific
            oGrid.Columns.Item("RowsHeader").Click(0, False, False)
            oForm.Items.Item("4").TextStyle = 7
            oForm.Items.Item("5").TextStyle = 7
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
            Throw ex
        End Try
    End Sub
    Private Sub LoadFiles(ByVal aform As SAPbouiCOM.Form)
        oGrid = aform.Items.Item("1").Specific
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
            If pVal.FormTypeEx = frm_hr_ClaimApproval Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD

                        End Select
                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If (pVal.ItemUID = "1" Or pVal.ItemUID = "19") And pVal.ColUID = "U_Z_Attachment" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    oGrid.Columns.Item("RowsHeader").Click(pVal.Row)
                                    LoadFiles(oForm)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If (pVal.ItemUID = "1" Or pVal.ItemUID = "19") And pVal.ColUID = "U_Z_ExpType" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
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
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                If oForm.State = SAPbouiCOM.BoFormStateEnum.fs_Restore Or oForm.State = SAPbouiCOM.BoFormStateEnum.fs_Maximized Then
                                    oApplication.Utilities.Resize(oForm)
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
                                    oApplication.Utilities.setEdittextvalue(oForm, "6", strDocEntry)
                                    oApplication.Utilities.LoadHistory(oForm, HistoryDoctype.ExpCli, strDocEntry)
                                ElseIf (pVal.ItemUID = "3" And pVal.ColUID = "RowsHeader" And pVal.Row > -1) Then
                                    'oApplication.Utilities.LoadStatusRemarks(oForm, pVal.Row)
                                    oApplication.Utilities.LoadLeaveRemarks(oForm, pVal.Row)
                                ElseIf pVal.ItemUID = "_1" Then
                                    Dim intRet As Integer = oApplication.SBO_Application.MessageBox("Are you sure want to submit the document?", 2, "Yes", "No", "")
                                    If intRet = 1 Then
                                        oApplication.Utilities.addUpdateDocument(oForm, HistoryDoctype.ExpCli, HeaderDoctype.ExpCli)
                                    End If
                                End If
                                If pVal.ItemUID = "19" And pVal.ColUID = "RowsHeader" And pVal.Row > -1 Then
                                    oGrid = oForm.Items.Item("19").Specific
                                    Dim strDocEntry As String = oGrid.DataTable.GetValue("Code", pVal.Row)
                                    oApplication.Utilities.SummaryHistory(oForm, HistoryDoctype.ExpCli, strDocEntry)
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
                Case mnu_hr_ExpApproval
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
