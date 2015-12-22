Public Class clshrMgrTrainApp
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox, oCombobox1 As SAPbouiCOM.ComboBox
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
    Public Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_MgrTrainApp) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_MgrTrainApp, frm_hr_MgrTrainApp)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        FillDepartment(oForm)
        oForm.PaneLevel = 1
        Dim osta As SAPbouiCOM.StaticText
        osta = oForm.Items.Item("19").Specific
        osta.Caption = "Step " & oForm.PaneLevel & " of 3"
        oForm.Items.Item("19").TextStyle = SAPbouiCOM.BoTextStyle.ts_BOLD
        reDrawForm(oForm)
        oForm.Freeze(False)
    End Sub
    Private Sub FillDepartment(ByVal sform As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = sform.Items.Item("7").Specific
        oCombobox1 = sform.Items.Item("16").Specific

        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        For intRow As Integer = oCombobox1.ValidValues.Count - 1 To 0 Step -1
            oCombobox1.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oCombobox1.ValidValues.Add("", "")
        oTempRec.DoQuery("Select Code,Remarks from OUDP")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item("Code").Value, oTempRec.Fields.Item("Remarks").Value)
            oCombobox1.ValidValues.Add(oTempRec.Fields.Item("Code").Value, oTempRec.Fields.Item("Remarks").Value)
            oTempRec.MoveNext()
        Next
        sform.Items.Item("7").DisplayDesc = True
        sform.Items.Item("16").DisplayDesc = True

    End Sub
    Private Sub NewRequestBind(ByVal aform As SAPbouiCOM.Form, ByVal deptno As String)
        aform.Freeze(True)
        Dim strqry As String

        Dim strConiditon As String
        Dim strmanager As String = oApplication.Utilities.getloggedonuser()
        Dim strEmpList As String = oApplication.Utilities.getEmpIDforMangers(strmanager)
        If strEmpList <> "" Then
            strConiditon = " U_Z_HREmpID in (" & strEmpList & ")"
        Else
            strConiditon = " 1=1 "
        End If
        oGrid = aform.Items.Item("10").Specific
        oGrid.DataTable = aform.DataSources.DataTables.Item("DT_0")
        If deptno <> "" Then
            strqry = "  select DocEntry,U_Z_ReqDate,U_Z_HREmpID,U_Z_HREmpName,U_Z_CourseName,U_Z_CourseDetails,convert(varchar(10),U_Z_TrainFrdt,103) as U_Z_TrainFrdt,convert(varchar(10),U_Z_TrainTodt,103) as U_Z_TrainTodt,U_Z_TrainCost,U_Z_Notes,"
            strqry = strqry & " U_Z_DeptName,U_Z_PosiName,CASE U_Z_ReqStatus when 'P' then 'Pending' when 'MA' then 'Manager Approved' when 'MR' then 'Manager Rejected'"
            strqry = strqry & " when 'HA' then 'HR Approved' else 'HR Rejected' end as U_Z_ReqStatus,U_Z_MgrStatus,"
            strqry = strqry & " U_Z_MgrRemarks  from [@Z_HR_ONTREQ] where U_Z_DeptCode='" & deptno & "' and  (U_Z_ReqStatus<>'HA' and U_Z_Reqstatus <>'HR') and " & strConiditon

        Else
            strqry = "  select DocEntry,U_Z_ReqDate,U_Z_HREmpID,U_Z_HREmpName,U_Z_CourseName,U_Z_CourseDetails,convert(varchar(10),U_Z_TrainFrdt,103) as U_Z_TrainFrdt,convert(varchar(10),U_Z_TrainTodt,103) as U_Z_TrainTodt,U_Z_TrainCost,U_Z_Notes,"
            strqry = strqry & " U_Z_DeptName,U_Z_PosiName,CASE U_Z_ReqStatus when 'P' then 'Pending' when 'MA' then 'Manager Approved' when 'MR' then 'Manager Rejected'"
            strqry = strqry & " when 'HA' then 'HR Approved' else 'HR Rejected' end as U_Z_ReqStatus,U_Z_MgrStatus,"
            strqry = strqry & " U_Z_MgrRemarks  from [@Z_HR_ONTREQ] where  (U_Z_ReqStatus<>'HA' and U_Z_Reqstatus <>'HR') and " & strConiditon

        End If
        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Request No"
        oGrid.Columns.Item("DocEntry").Editable = False
        oEditTextColumn = oGrid.Columns.Item("DocEntry")
        oEditTextColumn.LinkedObjectType = "Z_HR_ONTREQ"
        oGrid.Columns.Item("U_Z_ReqDate").TitleObject.Caption = "Request Date"
        oGrid.Columns.Item("U_Z_ReqDate").Editable = False
        oGrid.Columns.Item("U_Z_HREmpID").TitleObject.Caption = "Employee Id"
        oGrid.Columns.Item("U_Z_HREmpID").Editable = False
        oEditTextColumn = oGrid.Columns.Item("U_Z_HREmpID")
        oEditTextColumn.LinkedObjectType = 171
        oGrid.Columns.Item("U_Z_HREmpName").TitleObject.Caption = "Employee Name"
        oGrid.Columns.Item("U_Z_HREmpName").Editable = False
        oGrid.Columns.Item("U_Z_PosiName").TitleObject.Caption = "Position Name"
        oGrid.Columns.Item("U_Z_PosiName").Editable = False
        oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
        oGrid.Columns.Item("U_Z_DeptName").Editable = False
        oGrid.Columns.Item("U_Z_CourseName").TitleObject.Caption = "Training Title"
        oGrid.Columns.Item("U_Z_CourseName").Editable = False
        oGrid.Columns.Item("U_Z_CourseDetails").TitleObject.Caption = "Justification"
        oGrid.Columns.Item("U_Z_CourseDetails").Editable = False
        oGrid.Columns.Item("U_Z_TrainFrdt").TitleObject.Caption = "Training From Date"
        oGrid.Columns.Item("U_Z_TrainFrdt").Editable = False
        oGrid.Columns.Item("U_Z_TrainTodt").TitleObject.Caption = "Training To Date"
        oGrid.Columns.Item("U_Z_TrainTodt").Editable = False
        oGrid.Columns.Item("U_Z_TrainCost").TitleObject.Caption = "Training Course Cost"
        oGrid.Columns.Item("U_Z_TrainCost").Editable = False
        oGrid.Columns.Item("U_Z_Notes").TitleObject.Caption = "Comments"
        oGrid.Columns.Item("U_Z_Notes").Editable = False
        oGrid.Columns.Item("U_Z_ReqStatus").TitleObject.Caption = "Request Status"
        oGrid.Columns.Item("U_Z_ReqStatus").Editable = False
        oGrid.Columns.Item("U_Z_MgrStatus").TitleObject.Caption = "Manager Status"
        oGrid.Columns.Item("U_Z_MgrStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        ocombo = oGrid.Columns.Item("U_Z_MgrStatus")
        ocombo.ValidValues.Add("P", "Pending")
        ocombo.ValidValues.Add("MA", "Manager Approved")
        ocombo.ValidValues.Add("MR", "Manager Rejected")
        ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oGrid.Columns.Item("U_Z_MgrRemarks").TitleObject.Caption = "Manager Remarks"

        'oGrid.Columns.Item("Code").TitleObject.Caption = "Code"
        'oGrid.Columns.Item("Code").Visible = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        aform.Freeze(False)
    End Sub

    Private Sub ExistingRequestBind(ByVal aform As SAPbouiCOM.Form, ByVal deptno As String)
        aform.Freeze(True)
        Dim strqry, strConiditon As String
        Dim strmanager As String = oApplication.Utilities.getloggedonuser()
        Dim strEmpList As String = oApplication.Utilities.getEmpIDforMangers(strmanager)
        If strEmpList <> "" Then
            strConiditon = " U_Z_HREmpID in (" & strEmpList & ")"
        Else
            strConiditon = " 1=1 "
        End If

        oGrid = aform.Items.Item("25").Specific
        oGrid.DataTable = aform.DataSources.DataTables.Item("DT_1")
        strqry = "  select DocEntry,U_Z_ReqDate,U_Z_HREmpID,U_Z_HREmpName,U_Z_CourseName,U_Z_CourseDetails,convert(varchar(10),U_Z_TrainFrdt,103) as U_Z_TrainFrdt,convert(varchar(10),U_Z_TrainTodt,103) as U_Z_TrainTodt,U_Z_TrainCost,U_Z_Notes,"
        strqry = strqry & " U_Z_DeptName,U_Z_PosiName,CASE U_Z_ReqStatus when 'P' then 'Pending' when 'MA' then 'Manager Approved' when 'MR' then 'Manager Rejected'"
        strqry = strqry & " when 'HA' then 'HR Approved' else 'HR Rejected' end as U_Z_ReqStatus,CASE U_Z_MgrStatus when 'P' then 'Pending' when 'MA' then 'Manager Approved' when 'MR' then 'Manager Rejected'"
        strqry = strqry & " end as U_Z_MgrStatus,U_Z_MgrRemarks ,case  U_Z_HrStatus when 'P' then 'Pending' when 'HA' then 'Approved' else 'Rejected' end 'U_Z_HrStatus',U_Z_HrRemarks  from [@Z_HR_ONTREQ] where U_Z_DeptCode='" & deptno & "' and " & strConiditon
        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Request No"
        oEditTextColumn = oGrid.Columns.Item("DocEntry")
        oEditTextColumn.LinkedObjectType = "Z_HR_ONTREQ"
        oGrid.Columns.Item("U_Z_ReqDate").TitleObject.Caption = "Request Date"
        oGrid.Columns.Item("U_Z_HREmpID").TitleObject.Caption = "Employee Id"
        oGrid.Columns.Item("U_Z_HREmpID").Editable = False
        oEditTextColumn = oGrid.Columns.Item("U_Z_HREmpID")
        oEditTextColumn.LinkedObjectType = 171
        oGrid.Columns.Item("U_Z_HREmpName").TitleObject.Caption = "Employee Name"
        oGrid.Columns.Item("U_Z_CourseName").TitleObject.Caption = "Training Title"
        oGrid.Columns.Item("U_Z_CourseDetails").TitleObject.Caption = "Justification"
        oGrid.Columns.Item("U_Z_TrainFrdt").TitleObject.Caption = "Training From Date"
        oGrid.Columns.Item("U_Z_TrainTodt").TitleObject.Caption = "Training To Date"
        oGrid.Columns.Item("U_Z_TrainCost").TitleObject.Caption = "Training Course Cost"
        oGrid.Columns.Item("U_Z_Notes").TitleObject.Caption = "Comments"
        oGrid.Columns.Item("U_Z_PosiName").TitleObject.Caption = "Position Name"
        oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
        oGrid.Columns.Item("U_Z_ReqStatus").TitleObject.Caption = "Request Status"
        oGrid.Columns.Item("U_Z_MgrStatus").TitleObject.Caption = "Manager Status"
        oGrid.Columns.Item("U_Z_MgrRemarks").TitleObject.Caption = "Manager Remarks"
        oGrid.Columns.Item("U_Z_HrStatus").TitleObject.Caption = "HR Status"
        oGrid.Columns.Item("U_Z_HrRemarks").TitleObject.Caption = "HR Remarks"
        'oGrid.Columns.Item("Code").TitleObject.Caption = "Code"
        'oGrid.Columns.Item("Code").Visible = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        aform.Freeze(False)
    End Sub
#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim Reqno As String
            oCombobox = aForm.Items.Item("7").Specific
            Reqno = oCombobox.Selected.Value
            If Reqno = "" Then
                '   oApplication.Utilities.Message("Department is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '   Return False
            End If
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
#End Region
#Region "AddToUDT"
    Private Function AddToUDT(ByVal aForm As SAPbouiCOM.Form) As Boolean
        oForm.Freeze(True)
        Dim strTable, strEmpId, strcode, strqry As String
        Dim dt As Date
        Dim oValidateRS, otemprs As SAPbobsCOM.Recordset
        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        dt = Now.Date
        oGrid = aForm.Items.Item("10").Specific
        If oApplication.Company.InTransaction() Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        oApplication.Company.StartTransaction()
        Try
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                strEmpId = oGrid.DataTable.GetValue("U_Z_HREmpID", intRow)
                strcode = oGrid.DataTable.GetValue("DocEntry", intRow)
                strqry = "Update [@Z_HR_ONTREQ] set  U_Z_MgrStatus='" & oGrid.DataTable.GetValue("U_Z_MgrStatus", intRow) & "',U_Z_MgrRemarks='" & oGrid.DataTable.GetValue("U_Z_MgrRemarks", intRow) & "',U_Z_ReqStatus='" & oGrid.DataTable.GetValue("U_Z_MgrStatus", intRow) & "' where DocEntry='" & strcode & "'"
                oValidateRS.DoQuery(strqry)
            Next
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If
            oForm.Freeze(False)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            End If
            Return False
        End Try
        Return True
    End Function
#End Region

    Private Sub reDrawForm(ByVal oForm As SAPbouiCOM.Form)
        Try
            oForm.Freeze(True)
            oForm.Items.Item("24").Width = oForm.Width - 30
            oForm.Items.Item("24").Height = oForm.Height - 130
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
        End Try
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_MgrTrainApp Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" And (oForm.PaneLevel = 2 Or oForm.PaneLevel = 3) Then
                                    If Validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If (pVal.ItemUID = "10" Or pVal.ItemUID = "25") And pVal.ColUID = "DocEntry" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    For intRow As Integer = pVal.Row To pVal.Row
                                        If 1 = 1 Then
                                            Dim strcode, strStatus As String
                                            strcode = oGrid.DataTable.GetValue("DocEntry", intRow)
                                            strStatus = oGrid.DataTable.GetValue("U_Z_ReqStatus", intRow)
                                            Dim objct As New clshrNewTrainRequest
                                            objct.LoadForm1(strcode, strStatus)
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
                                reDrawForm(oForm)
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "22" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 3
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "23" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 4
                                    oForm.Freeze(False)
                                End If
                                Select Case pVal.ItemUID
                                    Case "3"
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = oForm.PaneLevel + 1
                                        If oForm.PaneLevel = 3 Then
                                            Dim strDept, stSQL1, strskilles, Reqno As String
                                            oCombobox = oForm.Items.Item("7").Specific
                                            oCombobox1 = oForm.Items.Item("16").Specific
                                            strDept = oCombobox.Selected.Value
                                            oCombobox1.Select(strDept, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                            NewRequestBind(oForm, strDept)
                                            ExistingRequestBind(oForm, strDept)
                                        End If
                                        oForm.Freeze(False)
                                    Case "4"
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = oForm.PaneLevel - 1
                                        oForm.Freeze(False)
                                    Case "5"
                                        If oApplication.SBO_Application.MessageBox("Do you want confirm the Training Approval", , "Yes", "No") = 2 Then
                                            Exit Sub
                                        Else
                                            If AddToUDT(oForm) = True Then
                                                oApplication.Utilities.Message("Manager Approved Successfully...", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                                oForm.Close()
                                            Else
                                                BubbleEvent = False
                                                Exit Sub
                                            End If

                                        End If
                                End Select

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
                Case mnu_MgrTrainApp
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
