Public Class clshrRecApproval
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
    Public Sub LoadForm(ByVal strtitle As String, ByVal empid As String, ByVal empname As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_RecApproval) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_RecApproval, frm_hr_RecApproval)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        ManagerName = empname
        ManagerId = empid
        If strtitle = "RHR" Then
            oForm.Title = "Recruitment Requisition First Level Approval"
            oForm.Items.Item("4").Visible = False
            oForm.Items.Item("12").Visible = True
        ElseIf strtitle = "RGM" Then
            oForm.Title = "Recruitment Requisition HR Approval"
            oForm.Items.Item("4").Visible = False
            oForm.Items.Item("12").Visible = True
        Else
            oForm.Title = "Recruitment Requisition"
            oForm.Items.Item("4").Visible = True
            oForm.Items.Item("12").Visible = False
        End If
        oForm.Freeze(True)
        oApplication.Utilities.setEdittextvalue(oForm, "5", empid)
        oApplication.Utilities.setEdittextvalue(oForm, "7", empname)
        Databind(empid, strtitle)
        Databind2(empid, strtitle)
        oForm.PaneLevel = 1
        reDrawForm(oForm)
        oForm.Freeze(False)
    End Sub
    Private Sub Databind(ByVal strempid As String, ByVal strtitle As String)
        Dim strqry As String
        oGrid = oForm.Items.Item("3").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
        If strempid <> "" And strtitle = "MPR" Then
            strqry = "SELECT ""DocEntry"", ""U_Z_ReqDate"",""U_Z_ExEmpID"", ""U_Z_EmpCode"", ""U_Z_EmpName"",ISNULL(""U_Z_DeptName"",'') as ""U_Z_DeptName"",ISNULL(""U_Z_PosName"", '')  AS ""Position"", ""U_Z_ExpMin"", ""U_Z_ExpMax"", ""U_Z_Vacancy"","
            strqry += " ""U_Z_MgrRemarks"", ""U_Z_EmpstDate"", ""U_Z_IntAppDead"", ""U_Z_ExtAppDead"",case ""U_Z_AppStatus"" when 'P' then 'Pending' when 'A' then 'Approved' when 'R' then 'Rejected' when 'C' then 'Closed' when 'L' then 'Canceled' end as ""U_Z_AppStatus"",isnull((select T13.firstName +' '+ISNULL(T13.middleName,'') +' '+ isnull(T13.lastName,'')  from OHEM T13 JOIN OUSR T14 ON T14.INTERNAL_K =T13.userId where T14.USER_CODE=T0.U_Z_CurApprover ),'') AS 'Current Approver',isnull((select T13.firstName +' '+ISNULL(T13.middleName,'') +' '+ isnull(T13.lastName,'')  from OHEM T13 JOIN OUSR T14 ON T14.INTERNAL_K =T13.userId where T14.USER_CODE=T0.U_Z_NxtApprover ),'') AS 'Next Approver' FROM ""@Z_HR_ORMPREQ"" T0 WHERE ""U_Z_EmpCode"" ='" & strempid & "'"
        End If
        oGrid.DataTable.ExecuteQuery(strqry)
        oEditTextColumn = oGrid.Columns.Item("DocEntry")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Invoice
        If strtitle = "MPR" Then
            oGrid.Columns.Item("DocEntry").TitleObject.Caption = " Recruitment requistion  Number"
            oGrid.Columns.Item("DocEntry").Editable = False
            oGrid.Columns.Item("U_Z_ReqDate").TitleObject.Caption = "Requested Date"
            oGrid.Columns.Item("U_Z_ReqDate").Editable = False
            oGrid.Columns.Item("U_Z_ExEmpID").TitleObject.Caption = "Ext.Requester Code"
            oGrid.Columns.Item("U_Z_ExEmpID").Visible = False
            oGrid.Columns.Item("U_Z_EmpCode").TitleObject.Caption = "Requester Code"
            oGrid.Columns.Item("U_Z_EmpCode").Editable = False
            oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Requester Name"
            oGrid.Columns.Item("U_Z_EmpName").Editable = False
            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
            oGrid.Columns.Item("U_Z_DeptName").Editable = False
            oGrid.Columns.Item("Position").TitleObject.Caption = "Position Name"
            oGrid.Columns.Item("Position").Editable = False
            oGrid.Columns.Item("U_Z_ExpMin").TitleObject.Caption = "Minimum Experience"
            oGrid.Columns.Item("U_Z_ExpMin").Editable = False
            oGrid.Columns.Item("U_Z_ExpMax").TitleObject.Caption = "Maximum Experience"
            oGrid.Columns.Item("U_Z_ExpMax").Editable = False
            oGrid.Columns.Item("U_Z_Vacancy").TitleObject.Caption = "Vacant positons"
            oGrid.Columns.Item("U_Z_Vacancy").Editable = False
            oGrid.Columns.Item("U_Z_MgrRemarks").TitleObject.Caption = "Requester Remarks"
            oGrid.Columns.Item("U_Z_MgrRemarks").Editable = False
            oGrid.Columns.Item("U_Z_EmpstDate").TitleObject.Caption = "Tentative DOJ"
            oGrid.Columns.Item("U_Z_EmpstDate").Editable = False
            oGrid.Columns.Item("U_Z_IntAppDead").TitleObject.Caption = "Internal Application Deadline"
            oGrid.Columns.Item("U_Z_IntAppDead").Editable = False
            oGrid.Columns.Item("U_Z_ExtAppDead").TitleObject.Caption = "External Application Deadline"
            oGrid.Columns.Item("U_Z_ExtAppDead").Editable = False
            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Status"
            oGrid.Columns.Item("U_Z_AppStatus").Editable = False
        End If
       
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
    Private Sub Databind2(ByVal strempid As String, ByVal strtitle As String)
        Dim strqry As String
        oGrid = oForm.Items.Item("11").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_1")
        If strempid <> "" And strtitle = "MPR" Then
            strqry = "SELECT case ""U_Z_AppStatus"" when 'P' then 'Pending' when 'A' then 'Approved' when 'R' then 'Rejected' when 'C' then 'Closed' when 'L' then 'Canceled' end as ""U_Z_AppStatus"", ""DocEntry"", ""U_Z_ReqDate"",""U_Z_ExEmpID"", ""U_Z_EmpCode"", ""U_Z_EmpName"",ISNULL(""U_Z_DeptName"",'') as ""U_Z_DeptName"",ISNULL(""U_Z_PosName"", '')  AS ""Position"", ""U_Z_ExpMin"", ""U_Z_ExpMax"", ""U_Z_Vacancy"","
            strqry += " ""U_Z_MgrRemarks"", ""U_Z_EmpstDate"", ""U_Z_IntAppDead"", ""U_Z_ExtAppDead"",isnull((select T13.firstName +' '+ISNULL(T13.middleName,'') +' '+ isnull(T13.lastName,'')  from OHEM T13 JOIN OUSR T14 ON T14.INTERNAL_K =T13.userId where T14.USER_CODE=T0.U_Z_CurApprover ),'') AS 'Current Approver',isnull((select T13.firstName +' '+ISNULL(T13.middleName,'') +' '+ isnull(T13.lastName,'')  from OHEM T13 JOIN OUSR T14 ON T14.INTERNAL_K =T13.userId where T14.USER_CODE=T0.U_Z_NxtApprover ),'') AS 'Next Approver' FROM ""@Z_HR_ORMPREQ"" T0 WHERE ""U_Z_EmpCode"" ='" & strempid & "'"
        End If
        oGrid.DataTable.ExecuteQuery(strqry)
        oEditTextColumn = oGrid.Columns.Item("DocEntry")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Invoice
        If strtitle = "MPR" Then
            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Status"
            oGrid.Columns.Item("U_Z_AppStatus").Editable = False
            oGrid.Columns.Item("DocEntry").TitleObject.Caption = " Recruitment requistion  Number"
            oGrid.Columns.Item("DocEntry").Editable = False
            oGrid.Columns.Item("U_Z_ReqDate").TitleObject.Caption = "Requested Date"
            oGrid.Columns.Item("U_Z_ReqDate").Editable = False
            oGrid.Columns.Item("U_Z_ExEmpID").TitleObject.Caption = "Ext.Requester Code"
            oGrid.Columns.Item("U_Z_ExEmpID").Visible = False
            oGrid.Columns.Item("U_Z_EmpCode").TitleObject.Caption = "Requester Code"
            oGrid.Columns.Item("U_Z_EmpCode").Editable = False
            oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Requester Name"
            oGrid.Columns.Item("U_Z_EmpName").Editable = False
            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
            oGrid.Columns.Item("U_Z_DeptName").Editable = False
            oGrid.Columns.Item("Position").TitleObject.Caption = "Position Name"
            oGrid.Columns.Item("Position").Editable = False
            oGrid.Columns.Item("U_Z_ExpMin").TitleObject.Caption = "Minimum Experience"
            oGrid.Columns.Item("U_Z_ExpMin").Editable = False
            oGrid.Columns.Item("U_Z_ExpMax").TitleObject.Caption = "Maximum Experience"
            oGrid.Columns.Item("U_Z_ExpMax").Editable = False
            oGrid.Columns.Item("U_Z_Vacancy").TitleObject.Caption = "Vacant positons"
            oGrid.Columns.Item("U_Z_Vacancy").Editable = False
            oGrid.Columns.Item("U_Z_MgrRemarks").TitleObject.Caption = "Requester Remarks"
            oGrid.Columns.Item("U_Z_MgrRemarks").Editable = False
            oGrid.Columns.Item("U_Z_EmpstDate").TitleObject.Caption = "Tentative DOJ"
            oGrid.Columns.Item("U_Z_EmpstDate").Editable = False
            oGrid.Columns.Item("U_Z_IntAppDead").TitleObject.Caption = "Internal Application Deadline"
            oGrid.Columns.Item("U_Z_IntAppDead").Editable = False
            oGrid.Columns.Item("U_Z_ExtAppDead").TitleObject.Caption = "External Application Deadline"
            oGrid.Columns.Item("U_Z_ExtAppDead").Editable = False
            oGrid.Columns.Item("Next Approver").Editable = False
            oGrid.Columns.Item("Current Approver").Editable = False
        End If
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oGrid.CollapseLevel = 1
    End Sub
#Region "AddToUDT"
    Private Function AddToUDT(ByVal aForm As SAPbouiCOM.Form, ByVal strtitle As String) As Boolean
        oForm.Freeze(True)
        Dim strTable, strEmpId, strcode, strqry As String
        Dim strHRStatus, strHRRemarks, strHREmpStDate, strHRInAppRead, strHRExtAppRead As String
        Dim dt As Date
        Dim oValidateRS, otemprs As SAPbobsCOM.Recordset
        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        dt = Now.Date
        If oApplication.Company.InTransaction() Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        oApplication.Company.StartTransaction()
        oGrid = aForm.Items.Item("3").Specific
        If strtitle = "Recruitment Requisition First Level Approval" Then
            Try
                For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                    strEmpId = oGrid.DataTable.GetValue("U_Z_EmpCode", intRow)
                    strcode = oGrid.DataTable.GetValue("DocEntry", intRow)
                    strqry = "Update ""@Z_HR_ORMPREQ"" set  ""U_Z_HODStatus""='" & oGrid.DataTable.GetValue("U_Z_HODStatus", intRow) & "',""U_Z_HODRemarks""='" & oGrid.DataTable.GetValue("U_Z_HODRemarks", intRow) & "',""U_Z_MgrStatus""='" & oGrid.DataTable.GetValue("U_Z_HODStatus", intRow) & "' where ""DocEntry""='" & strcode & "'"
                    oValidateRS.DoQuery(strqry)
                    oApplication.Utilities.UpdateRecruitmentTimeStamp(strcode, "FL")
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
        ElseIf strtitle = "Recruitment Requisition HR Approval" Then
            Try
                For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                    strEmpId = oGrid.DataTable.GetValue("U_Z_EmpCode", intRow)
                    strcode = oGrid.DataTable.GetValue("DocEntry", intRow)
                    strHRStatus = oGrid.DataTable.GetValue("U_Z_HRStatus", intRow)
                    strHRRemarks = oGrid.DataTable.GetValue("U_Z_HRRemarks", intRow)
                    strHREmpStDate = oGrid.DataTable.GetValue("U_Z_EmpstDate", intRow)
                    strHRInAppRead = oGrid.DataTable.GetValue("U_Z_IntAppDead", intRow)
                    strHRExtAppRead = oGrid.DataTable.GetValue("U_Z_ExtAppDead", intRow)
                    If HRValidation(strHRStatus, strHRRemarks, strHREmpStDate, strHRInAppRead, strHRExtAppRead, intRow) = True Then
                        strqry = "Update ""@Z_HR_ORMPREQ"" set  ""U_Z_HRStatus""='" & oGrid.DataTable.GetValue("U_Z_HRStatus", intRow) & "',""U_Z_HRRemarks""='" & oGrid.DataTable.GetValue("U_Z_HRRemarks", intRow) & "',"
                        strqry = strqry & " ""U_Z_MgrStatus""='" & oGrid.DataTable.GetValue("U_Z_HRStatus", intRow) & "',""U_Z_EmpstDate""='" & Convert.ToDateTime(oGrid.DataTable.GetValue("U_Z_EmpstDate", intRow)).ToString("yyyyMMdd") & "',""U_Z_IntAppDead""='" & Convert.ToDateTime(oGrid.DataTable.GetValue("U_Z_IntAppDead", intRow)).ToString("yyyyMMdd") & "',"
                        strqry = strqry & " ""U_Z_ExtAppDead""='" & Convert.ToDateTime(oGrid.DataTable.GetValue("U_Z_ExtAppDead", intRow)).ToString("yyyyMMdd") & "' where ""DocEntry""='" & strcode & "'"
                        oValidateRS.DoQuery(strqry)
                        oApplication.Utilities.UpdateRecruitmentTimeStamp(strcode, "HR")
                    Else
                        aForm.Freeze(False)
                        Return False
                    End If
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
        End If
        Return True
    End Function
#End Region

#Region "HRValidation"
    Private Function HRValidation(ByVal HRStatus As String, ByVal HRRemarks As String, ByVal HREmpDate As String, ByVal HRIniApp As String, ByVal HRExtApp As String, ByVal RowNo As Integer) As Boolean
        Dim RetVal As Boolean = False
        If HREmpDate = "" Then
            oApplication.Utilities.Message("Enter Tentative DOJ at Line : " & RowNo + 1 & "", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return RetVal
        ElseIf HRIniApp = "" Then
            oApplication.Utilities.Message("Enter Internal Application DeadLine Date at Line : " & RowNo + 1 & "", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return RetVal
        ElseIf HRExtApp = "" Then
            oApplication.Utilities.Message("Enter External Application DeadLine Date at Line : " & RowNo + 1 & "", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return RetVal
        End If
        RetVal = True
        'return retval
        Return RetVal = True
    End Function


#End Region

    Private Sub reDrawForm(ByVal oForm As SAPbouiCOM.Form)
        Try
            oForm.Freeze(True)
            oForm.Items.Item("10").Width = oForm.Width - 30
            oForm.Items.Item("10").Height = oForm.Height - 92
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
        End Try
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_RecApproval Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If oForm.Title = "Recruitment Requisition HR Approval" Then
                                    oForm.Freeze(True)
                                    Dim oGrid As SAPbouiCOM.Grid
                                    Dim strEmpId, strcode As String
                                    Dim strHRStatus, strHRRemarks, strHREmpStDate, strHRInAppRead, strHRExtAppRead As String
                                    oGrid = oForm.Items.Item("3").Specific
                                    For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                        strEmpId = oGrid.DataTable.GetValue("U_Z_EmpCode", intRow)
                                        strcode = oGrid.DataTable.GetValue("DocEntry", intRow)
                                        strHRStatus = oGrid.DataTable.GetValue("U_Z_HRStatus", intRow)
                                        strHRRemarks = oGrid.DataTable.GetValue("U_Z_HRRemarks", intRow)
                                        strHREmpStDate = oGrid.DataTable.GetValue("U_Z_EmpstDate", intRow)
                                        strHRInAppRead = oGrid.DataTable.GetValue("U_Z_IntAppDead", intRow)
                                        strHRExtAppRead = oGrid.DataTable.GetValue("U_Z_ExtAppDead", intRow)
                                        'If Not HRValidation(strHRStatus, strHRRemarks, strHREmpStDate, strHRInAppRead, strHRExtAppRead, intRow + 1) Then
                                        '    BubbleEvent = False
                                        'End If
                                    Next
                                    oForm.Freeze(False)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                If oForm.Title = "Recruitment Requisition First Level Approval" Then
                                    If pVal.ItemUID = "3" And pVal.ColUID = "U_Z_HODStatus" Then
                                        oGrid = oForm.Items.Item("3").Specific
                                        Dim strHRStatus As String = oGrid.DataTable.Columns.Item("U_Z_HRStatus").Cells.Item(pVal.Row).Value
                                        If strHRStatus <> "O" Then
                                            BubbleEvent = False
                                            oApplication.Utilities.Message("Can't change status when HR Approved...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                        End If
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim strCode, strHRstatus, strGMstatus, empcode, empname As String
                                If (pVal.ItemUID = "3" Or pVal.ItemUID = "11") And pVal.ColUID = "DocEntry" Then
                                    If pVal.ItemUID = "3" Then
                                        oGrid = oForm.Items.Item("3").Specific
                                    ElseIf pVal.ItemUID = "11" Then
                                        oGrid = oForm.Items.Item("11").Specific
                                    End If
                                    For intRow As Integer = pVal.Row To pVal.Row
                                        If 1 = 1 Then
                                            strCode = oGrid.DataTable.GetValue("DocEntry", oGrid.GetDataTableRowIndex(pVal.Row))
                                            strHRstatus = oGrid.DataTable.GetValue("U_Z_AppStatus", oGrid.GetDataTableRowIndex(pVal.Row))
                                            empcode = oApplication.Utilities.getEdittextvalue(oForm, "5")
                                            empname = oApplication.Utilities.getEdittextvalue(oForm, "7")
                                            Dim oTest As SAPbobsCOM.Recordset
                                            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                            oTest.DoQuery("Select * from ""@Z_HR_ORMPREQ"" where ""DocEntry""=" & strCode)
                                            If oTest.RecordCount <= 0 Then
                                                BubbleEvent = False
                                                Exit Sub
                                            End If
                                           If oForm.Title = "Recruitment Requisition First Level Approval" Then ' And strHRstatus <> "Closed" Then
                                                strGMstatus = oGrid.DataTable.GetValue("U_Z_AppStatus", oGrid.GetDataTableRowIndex(pVal.Row))
                                                Dim objct As New clshrMPRequest
                                                objct.LoadForm1(strCode, oForm.Title, empcode, empname, strGMstatus)
                                            ElseIf oForm.Title = "Recruitment Requisition" Then ' And strHRstatus <> "Open" Then
                                                Dim objct As New clshrMPRequest
                                                objct.LoadForm1(strCode, oForm.Title, empcode, empname, strHRstatus)
                                            Else
                                                oApplication.Utilities.Message("Your request is Processing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            End If
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
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim strCode, strHRstatus, strGMstatus, empcode, empname As String
                                If pVal.ItemUID = "13" Then
                                    oGrid = oForm.Items.Item("3").Specific
                                    For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                        If oGrid.Rows.IsSelected(intRow) Then
                                            Dim objHistory As New clshrAppHisDetails
                                            If oGrid.DataTable.GetValue("DocEntry", intRow) > 0 Then
                                                objHistory.LoadForm(oForm, HistoryDoctype.Rec, oGrid.DataTable.GetValue("DocEntry", intRow))
                                            End If
                                            Exit Sub
                                        End If
                                    Next
                                End If
                                If pVal.ItemUID = "8" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 1
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "9" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 2
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "12" Then
                                    If oApplication.SBO_Application.MessageBox("Do you want confirm the Recruitment Approval", , "Yes", "No") = 2 Then
                                        Exit Sub
                                    Else
                                        If AddToUDT(oForm, oForm.Title) = True Then
                                            oApplication.Utilities.Message(oForm.Title & " Successfully...", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                            oForm.Close()
                                        Else
                                            BubbleEvent = False
                                            Exit Sub
                                        End If

                                    End If
                                End If
                                If pVal.ItemUID = "4" Then
                                    oGrid = oForm.Items.Item("3").Specific
                                    For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                        If oGrid.Rows.IsSelected(intRow) Then
                                            strCode = oGrid.DataTable.GetValue("DocEntry", intRow)
                                            strHRstatus = oGrid.DataTable.GetValue("U_Z_AppStatus", intRow)
                                            empcode = oApplication.Utilities.getEdittextvalue(oForm, "5")
                                            empname = oApplication.Utilities.getEdittextvalue(oForm, "7")
                                          If oForm.Title = "Recruitment Requisition" Then
                                                Dim objct As New clshrMPRequest
                                                objct.LoadForm1(strCode, oForm.Title, empcode, empname, strHRstatus)
                                            Else
                                                oApplication.Utilities.Message("Your request is Processing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                                Exit Sub
                                            End If
                                           
                                        End If
                                    Next
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
