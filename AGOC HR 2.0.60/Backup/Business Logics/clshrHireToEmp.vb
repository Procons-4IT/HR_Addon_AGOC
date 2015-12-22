Public Class clshrHireToEmp
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
    Private Sub LoadForm(ByVal oForm As SAPbouiCOM.Form)
        oForm = oApplication.Utilities.LoadForm(xml_hr_HireToEmp, frm_hr_HireToEmp)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        BindData(oForm)
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        oForm.Freeze(False)
    End Sub
    Private Sub BindData(ByVal aform As SAPbouiCOM.Form)
        Dim strqry As String
        oGrid = aform.Items.Item("3").Specific
        oGrid.DataTable = aform.DataSources.DataTables.Item("DT_0")
        strqry = " select U_Z_ReqNo,U_Z_JobPosi ,T0.DocEntry, U_Z_HRAppID,U_Z_HRAppName,T0.U_Z_Dept,T0.U_Z_DeptName,T0.U_Z_Dob,U_Z_Email,T0.U_Z_Mobile,T0.U_Z_YrExp,T0.U_Z_Skills,"
        strqry = strqry & " case T1.U_Z_Status when 'R' then 'Received' when 'S' then 'Shortlisted' when 'I' then 'Interviewed'"
        strqry = strqry & " when 'O' then 'Job Offering' when 'J' then 'Rejected' when 'A' then 'Offer Accepted' when 'H' then 'Hired' else 'Canceled' end as U_Z_Status  ,T1.U_Z_JoinDate 'Joining Date' from [@Z_HR_OHEM1] T0 inner join [@Z_HR_OCRAPP] T1"
        strqry = strqry & "     on T0.U_Z_HRAppID=T1.DocEntry  where T1.U_Z_Status ='A' and T0.U_Z_AppStatus='A' " 'and T0.U_Z_ApplStatus='S'"
        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("DocEntry").TitleObject.Caption = "DocEntry"
        oGrid.Columns.Item("DocEntry").Visible = False
        oGrid.Columns.Item("U_Z_HRAppID").TitleObject.Caption = "Applicant Id"
        oGrid.Columns.Item("U_Z_HRAppID").Editable = False
        oEditTextColumn = oGrid.Columns.Item("U_Z_HRAppID")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
        oGrid.Columns.Item("U_Z_HRAppName").TitleObject.Caption = "Applicant Name"
        oGrid.Columns.Item("U_Z_HRAppName").Editable = False
        oGrid.Columns.Item("U_Z_Dept").TitleObject.Caption = "Department"
        oGrid.Columns.Item("U_Z_Dept").Visible = False
        oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department"
        oGrid.Columns.Item("U_Z_DeptName").Editable = False
        oGrid.Columns.Item("U_Z_ReqNo").TitleObject.Caption = "Recruitmet Requesition No"
        oGrid.Columns.Item("U_Z_ReqNo").Editable = False
        oEditTextColumn = oGrid.Columns.Item("U_Z_ReqNo")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
        oEditTextColumn = oGrid.Columns.Item("U_Z_ReqNo")
        oEditTextColumn.LinkedObjectType = "Z_HR_ORREQS"
        oGrid.Columns.Item("U_Z_Email").TitleObject.Caption = "Email Id"
        oGrid.Columns.Item("U_Z_Email").Editable = False
        oGrid.Columns.Item("U_Z_Mobile").TitleObject.Caption = "Mobile No "
        oGrid.Columns.Item("U_Z_Mobile").Editable = False
        oGrid.Columns.Item("U_Z_JobPosi").TitleObject.Caption = "Position "
        oGrid.Columns.Item("U_Z_JobPosi").Editable = False
        oGrid.Columns.Item("U_Z_Dob").TitleObject.Caption = "Date of Birth"
        oGrid.Columns.Item("U_Z_Dob").Editable = False
        oGrid.Columns.Item("U_Z_YrExp").TitleObject.Caption = "Year of Experience"
        oGrid.Columns.Item("U_Z_YrExp").Editable = False
        oGrid.Columns.Item("U_Z_Skills").TitleObject.Caption = "Skill Sets"
        oGrid.Columns.Item("U_Z_Skills").Editable = False
        oGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Applicant Status"
        oGrid.Columns.Item("U_Z_Status").Editable = False
        oGrid.Columns.Item("Joining Date").TitleObject.Caption = "Joining Date"
        oGrid.Columns.Item("Joining Date").Editable = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_HireToEmp Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" And pVal.ColUID = "U_Z_HRAppID" Then
                                    'oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    'Dim strcode As String = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    'Dim strdeptname As String = oGrid.DataTable.GetValue("U_Z_DeptName", pVal.Row)
                                    'Dim strdptid As String = oGrid.DataTable.GetValue("U_Z_Dept", pVal.Row)
                                    'Dim strPosi As String = oGrid.DataTable.GetValue("U_Z_JobPosi", pVal.Row)
                                    'Dim Invbase As String = oGrid.DataTable.GetValue("DocEntry", pVal.Row)
                                    'Dim Reqno As String = oGrid.DataTable.GetValue("U_Z_ReqNo", pVal.Row)
                                    'Dim ooBj As New clshrHiring
                                    'ooBj.LoadForm(strcode, strdptid, strdeptname, strPosi, Invbase, Reqno)
                                    'BubbleEvent = False
                                    'Exit Sub
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcode As String = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    Dim ooBj As New clshrCrApplicants
                                    ooBj.ViewCandidate(strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "3" And pVal.ColUID = "U_Z_ReqNo" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcode As String = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    Dim ooBj As New clshrMPRequest
                                    ooBj.LoadForm1(strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If

                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim strCode, strstatus, strdptid, strdeptname, strPosi As String
                                If pVal.ItemUID = "4" Then
                                    oGrid = oForm.Items.Item("3").Specific
                                    For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                        If oGrid.Rows.IsSelected(intRow) Then
                                            strCode = oGrid.DataTable.GetValue("U_Z_HRAppID", intRow)
                                            strdeptname = oGrid.DataTable.GetValue("U_Z_DeptName", intRow)
                                            strdptid = oGrid.DataTable.GetValue("U_Z_Dept", intRow)
                                            strPosi = oGrid.DataTable.GetValue("U_Z_JobPosi", intRow)
                                            Dim Invbase As String = oGrid.DataTable.GetValue("DocEntry", intRow)
                                            Dim Reqno As String = oGrid.DataTable.GetValue("U_Z_ReqNo", intRow)
                                            Dim objct As New clshrHiring
                                            objct.LoadForm(strCode, strdptid, strdeptname, strPosi, Invbase, Reqno)
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
                Case mnu_hr_Hiring
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
