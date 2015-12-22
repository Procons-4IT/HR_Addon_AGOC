Imports System.IO
Public Class clshrAppHisDetails
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oCombo As SAPbouiCOM.ComboBoxColumn
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

    Public Sub LoadForm(ByVal oForm As SAPbouiCOM.Form, ByVal DocType As modVariables.HistoryDoctype, ByVal DocNo As String)
        Try
            oForm = oApplication.Utilities.LoadForm(xml_hr_AppHisDetails, frm_hr_AppHisDetails)
            oForm = oApplication.SBO_Application.Forms.ActiveForm()
            oForm.Items.Item("3").Visible = True
            oForm.Items.Item("4").Visible = True
            enDocType = DocType
            oApplication.Utilities.ViewHistory(oForm, DocType, DocNo)
            oApplication.Utilities.LoadViewHistory(oForm, DocType, DocNo)
            oForm.Items.Item("4").TextStyle = 7
        Catch ex As Exception
            oForm.Freeze(False)
            Throw ex
        End Try
    End Sub
    Public Sub LoadForm1(ByVal oForm As SAPbouiCOM.Form, ByVal DocNo As String)
        Try
            oForm = oApplication.Utilities.LoadForm(xml_hr_AppHisDetails, frm_hr_AppHisDetails)
            oForm = oApplication.SBO_Application.Forms.ActiveForm()
            oForm.Items.Item("1").Height = 300
            oForm.Title = "Interview Summary"
            DataBind(oForm, DocNo)
            oForm.Items.Item("3").Visible = False
            oForm.Items.Item("4").Visible = False
        Catch ex As Exception
            oForm.Freeze(False)
            Throw ex
        End Try
    End Sub
    Public Sub LoadForm2(ByVal oForm As SAPbouiCOM.Form, ByVal DocNo As String)
        Try
            oForm = oApplication.Utilities.LoadForm(xml_hr_AppHisDetails, frm_hr_AppHisDetails)
            oForm = oApplication.SBO_Application.Forms.ActiveForm()
            oForm.Title = "Applicant Interview Details"
            DataBind(oForm, DocNo)
            LoadViewHistory(oForm, HistoryDoctype.Final, DocNo)
            oForm.Items.Item("3").Visible = True
            oForm.Items.Item("4").Description = "Approval History"
        Catch ex As Exception
            oForm.Freeze(False)
            Throw ex
        End Try
    End Sub
    Public Sub LoadViewHistory(ByVal aForm As SAPbouiCOM.Form, ByVal enDocType As modVariables.HistoryDoctype, ByVal strDocEntry As String)
        Try
            aForm.Freeze(True)
            Dim sQuery As String
            oGrid = aForm.Items.Item("3").Specific
            Select Case enDocType
                Case HistoryDoctype.ExpCli, HistoryDoctype.TraReq, HistoryDoctype.RegTra, HistoryDoctype.NewTra, HistoryDoctype.Rec, HistoryDoctype.EmpPro, HistoryDoctype.EmpPos, HistoryDoctype.Final
                    sQuery = " Select DocEntry,U_Z_DocEntry,U_Z_DocType,U_Z_EmpId,U_Z_EmpName,U_Z_ApproveBy,CreateDate ,CreateTime,UpdateDate,UpdateTime,U_Z_AppStatus,U_Z_Remarks From [@Z_HR_APHIS] "
                    sQuery += " Where U_Z_DocType = '" + enDocType.ToString() + "'"
                    sQuery += " And U_Z_DocEntry = '" + strDocEntry + "'"
                    oGrid.DataTable.ExecuteQuery(sQuery)
                    formatHistory(aForm, enDocType)
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
            End Select
            aForm.Freeze(False)
        Catch ex As Exception
            aForm.Freeze(False)
            Throw ex
        End Try
    End Sub
    Private Sub formatHistory(ByVal aForm As SAPbouiCOM.Form, ByVal enDocType As modVariables.HistoryDoctype)
        Try
            aForm.Freeze(True)
            Dim oGrid As SAPbouiCOM.Grid
            Dim oComboBox As SAPbouiCOM.ComboBox
            Dim oGridCombo As SAPbouiCOM.ComboBoxColumn
            Dim oEditTextColumn As SAPbouiCOM.EditTextColumn
            Select Case enDocType
                Case HistoryDoctype.ExpCli, HistoryDoctype.TraReq, HistoryDoctype.RegTra, HistoryDoctype.NewTra, HistoryDoctype.Rec, HistoryDoctype.AppShort, HistoryDoctype.EmpPro, HistoryDoctype.EmpPos, HistoryDoctype.Final
                    oGrid = aForm.Items.Item("3").Specific
                    oGrid.Columns.Item("DocEntry").Visible = False
                    oGrid.Columns.Item("U_Z_DocEntry").TitleObject.Caption = "Reference No."
                    oGrid.Columns.Item("U_Z_DocEntry").Visible = False
                    oGrid.Columns.Item("U_Z_DocType").Visible = False
                    oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee ID"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
                    oEditTextColumn.LinkedObjectType = "171"
                    oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
                    oGrid.Columns.Item("U_Z_ApproveBy").TitleObject.Caption = "Approved By"
                    oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approved Status"
                    oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                    oGridCombo.ValidValues.Add("A", "Approved")
                    oGridCombo.ValidValues.Add("R", "Rejected")
                    oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                    oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oGrid.AutoResizeColumns()
            End Select
            aForm.Freeze(False)
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                If oGrid.DataTable.GetValue("U_Z_ApproveBy", intRow) = oApplication.Company.UserName Then
                    oGrid.Columns.Item("RowsHeader").Click(intRow, False, False)
                    aForm.Freeze(False)
                    Exit Sub
                End If
            Next
            aForm.Items.Item("8").Enabled = True
            aForm.Items.Item("10").Enabled = True
        Catch ex As Exception
            aForm.Freeze(False)
            Throw ex
        End Try
    End Sub
    Private Sub DataBind(ByVal aForm As SAPbouiCOM.Form, ByVal DocNo As String)
        Try
            aForm.Freeze(True)
            Dim oRecSet As SAPbobsCOM.Recordset
            Dim strDetailQry As String
            oGrid = aForm.Items.Item("1").Specific
            strDetailQry = "Select ISNULL(U_Z_InType,'-') as 'Interview Type',U_Z_ScheduleDate as 'Schedule Date',U_Z_SchEmpID as 'Scheduler EmpID', T1.FirstName As 'Scheduler Name' ,U_Z_ScTime as 'Schedule Time',U_Z_InterviewDate as 'Interview Date',U_Z_InterviwerID as 'Interviewer EmpID',ISNULL(U_Z_Status,'-') as 'Status',U_Z_InterviewStatus as 'Interview Status',U_Z_Rating as 'Rating',U_Z_RatPer as 'Rating Percentage',U_Z_FileName as 'Attachment',U_Z_Comments as 'Comments' from [@Z_HR_OHEM2] T0 Left Outer Join OHEM T1 On T0.U_Z_SchEmpID = T1.EmpID Where DocEntry = " & DocNo & ""
            oGrid.DataTable.ExecuteQuery(strDetailQry)
            oGrid.Columns.Item("Interview Type").TitleObject.Caption = "Interview Type"
            oGrid.Columns.Item("Interview Type").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oCombo = oGrid.Columns.Item("Interview Type")
            oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strDetailQry = "Select U_Z_TypeCode As Code,U_Z_TypeName As Name From [@Z_HR_OITYP]"
            oRecSet.DoQuery(strDetailQry)
            oCombo.ValidValues.Add("", "")
            If Not oRecSet.EoF Then
                For index As Integer = 0 To oRecSet.RecordCount - 1
                    If Not oRecSet.EoF Then
                        oCombo.ValidValues.Add(oRecSet.Fields.Item("Code").Value, oRecSet.Fields.Item("Name").Value)
                        oRecSet.MoveNext()
                    End If
                Next
            End If
            oCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
            oGrid.Columns.Item("Schedule Date").TitleObject.Caption = "Schedule Date"
            oGrid.Columns.Item("Scheduler EmpID").TitleObject.Caption = "Scheduler EmpID"
            oEditTextColumn = oGrid.Columns.Item("Scheduler EmpID")
            oEditTextColumn.LinkedObjectType = "171"

            oGrid.Columns.Item("Scheduler Name").TitleObject.Caption = "Scheduler Name"
            oGrid.Columns.Item("Schedule Time").TitleObject.Caption = "Schedule Time"
            oGrid.Columns.Item("Interview Date").TitleObject.Caption = "Interview Date"
            oGrid.Columns.Item("Interviewer EmpID").TitleObject.Caption = "Interviewer EmpID"
            oEditTextColumn = oGrid.Columns.Item("Interviewer EmpID")
            oEditTextColumn.LinkedObjectType = "171"
            oGrid.Columns.Item("Status").TitleObject.Caption = "Status"
            oGrid.Columns.Item("Status").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oCombo = oGrid.Columns.Item("Status")
            oCombo.ValidValues.Add("-", "Pending")
            oCombo.ValidValues.Add("CO", "Conducted")
            oCombo.ValidValues.Add("CA", "Cancelled")
            oCombo.ValidValues.Add("RS", "Rescheduled")
            oCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
            oGrid.Columns.Item("Interview Status").TitleObject.Caption = "Interview Status"
            oGrid.Columns.Item("Interview Status").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oCombo = oGrid.Columns.Item("Interview Status")
            oCombo.ValidValues.Add("P", "Pending")
            oCombo.ValidValues.Add("S", "Selected")
            oCombo.ValidValues.Add("R", "Rejected")
            oCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
            oGrid.Columns.Item("Rating").TitleObject.Caption = "Rating"
            oGrid.Columns.Item("Rating Percentage").TitleObject.Caption = "Rating Percentage"
            oGrid.Columns.Item("Attachment").TitleObject.Caption = "Attachment"
            oEditTextColumn = oGrid.Columns.Item("Attachment")
            oEditTextColumn.LinkedObjectType = "Z_HR_OEXFOM"
            oGrid.Columns.Item("Comments").TitleObject.Caption = "Comments"
            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
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

    Private Sub LoadFiles1(ByVal aform As SAPbouiCOM.Form, ByVal intRow As Integer)
        oGrid = aform.Items.Item("1").Specific
        ' For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
        'If oGrid.Rows.IsSelected(intRow) Then
        Dim strFilename, strFilePath As String
        strFilename = oGrid.DataTable.GetValue("Attachment", intRow)
        Dim Filename As String = Path.GetFileName(strFilename)
        strFilePath = oGrid.DataTable.GetValue("Attachment", intRow)

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
        'End If

        ' Next
        oApplication.Utilities.Message("No file has been selected...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_AppHisDetails Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD

                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                'If pVal.ItemUID = "1" And pVal.ColUID = "RowsHeader" And pVal.Row > -1 Then
                                '    oGrid = oForm.Items.Item("1").Specific
                                '    Select Case ManageDocType
                                '        Case "ExpCli"
                                '            Dim strDocEntry As String = oGrid.DataTable.GetValue("Code", pVal.Row)
                                '            oApplication.Utilities.LoadViewHistory(oForm, Doctype.ExpCli, strDocEntry)
                                '        Case "TraReq"
                                '            Dim strDocEntry As String = oGrid.DataTable.GetValue("DocEntry", pVal.Row)
                                '            oApplication.Utilities.LoadViewHistory(oForm, Doctype.TraReq, strDocEntry)
                                '    End Select

                                'End If
                        End Select
                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1" And pVal.ColUID = "U_Z_Attachment" Then
                                    oGrid = oForm.Items.Item("1").Specific
                                    oGrid.Columns.Item("RowsHeader").Click(pVal.Row)
                                    LoadFiles(oForm)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "1" And pVal.ColUID = "Attachment" Then
                                    oGrid = oForm.Items.Item("1").Specific
                                    oGrid.Columns.Item("RowsHeader").Click(pVal.Row)
                                    LoadFiles1(oForm, pVal.Row)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                Select Case enDocType
                                    Case "5", "6"
                                        If pVal.ItemUID = "1" And pVal.ColUID = "RowsHeader" And pVal.Row > -1 Then
                                            oGrid = oForm.Items.Item("1").Specific
                                            Dim strDocEntry As String = oGrid.DataTable.GetValue("Code", pVal.Row)
                                            Select Case enDocType
                                                Case "5"
                                                    oApplication.Utilities.LoadViewHistory(oForm, HistoryDoctype.EmpPro, strDocEntry)
                                                Case "6"
                                                    oApplication.Utilities.LoadViewHistory(oForm, HistoryDoctype.EmpPos, strDocEntry)
                                            End Select
                                        End If
                                End Select
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
