Public Class clsViewEmpDetails
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
    Public Sub LoadForm(ByVal oForm As SAPbouiCOM.Form, ByVal canid As String, ByVal aChoice As String, Optional ByVal strTitle As String = "")
        oForm = oApplication.Utilities.LoadForm(xml_hr_ViewEmpDetails, frm_hr_ViewEmpDetails)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)

        If aChoice = "Promotion" Then
            oForm.Title = "Employee Promotion Details"
            Databind(oForm, canid, aChoice)
        ElseIf aChoice = "Transfer" Then
            oForm.Title = "Employee Transfer Details"
            Databind(oForm, canid, aChoice)
        Else
            oForm.Title = "Employee Position Changes Details"
            Databind(oForm, canid, aChoice)
        End If
        oForm.Freeze(False)
    End Sub

    Private Sub Databind(ByVal aForm As SAPbouiCOM.Form, ByVal strEmpid As String, ByVal strchoice As String)
        Dim strqry As String
        Dim oTemp As SAPbobsCOM.Recordset
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oGrid = aForm.Items.Item("3").Specific
        oGrid.DataTable = aForm.DataSources.DataTables.Item("DT_0")
        If strchoice = "Promotion" Then
            strqry = "select U_Z_EmpId,U_Z_FirstName,U_Z_LastName,U_Z_DeptName,U_Z_JobName,U_Z_OrgName,U_Z_PosName,U_Z_SalCode,"
            strqry = strqry & " U_Z_JoinDate,U_Z_ProJoinDate  from [@Z_HR_HEM2]  where U_Z_EmpId=" & strEmpid & ""
            oGrid.DataTable.ExecuteQuery(strqry)
            oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee Id"
            oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
            oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
            oGrid.Columns.Item("U_Z_FirstName").TitleObject.Caption = "First Name"
            oGrid.Columns.Item("U_Z_LastName").TitleObject.Caption = "Last Name"
            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department"
            oGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position Name"
            oGrid.Columns.Item("U_Z_JobName").TitleObject.Caption = "Job Name"
            oGrid.Columns.Item("U_Z_OrgName").TitleObject.Caption = "Organization Name"
            oGrid.Columns.Item("U_Z_SalCode").TitleObject.Caption = "Salary Code"
            oGrid.Columns.Item("U_Z_ProJoinDate").TitleObject.Caption = "Effective To Date"
            oGrid.Columns.Item("U_Z_JoinDate").TitleObject.Caption = "Effective From Date"
        ElseIf strchoice = "Transfer" Then
            strqry = "select U_Z_EmpId,U_Z_FirstName,U_Z_LastName,U_Z_DeptName,U_Z_JobName,U_Z_OrgName,U_Z_PosName,U_Z_SalCode,"
            strqry = strqry & " U_Z_JoinDate,U_Z_TraJoinDate  from [@Z_HR_HEM3]  where U_Z_EmpId=" & strEmpid & ""
            oGrid.DataTable.ExecuteQuery(strqry)
            oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee Id"
            oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
            oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
            oGrid.Columns.Item("U_Z_FirstName").TitleObject.Caption = "First Name"
            oGrid.Columns.Item("U_Z_LastName").TitleObject.Caption = "Last Name"
            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department"
            oGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position Name"
            oGrid.Columns.Item("U_Z_JobName").TitleObject.Caption = "Job Name"
            oGrid.Columns.Item("U_Z_OrgName").TitleObject.Caption = "Organization Name"
            oGrid.Columns.Item("U_Z_SalCode").TitleObject.Caption = "Salary Code"
            oGrid.Columns.Item("U_Z_TraJoinDate").TitleObject.Caption = "Effective To Date"
            oGrid.Columns.Item("U_Z_JoinDate").TitleObject.Caption = "Effective From Date"
        Else
            strqry = "select U_Z_EmpId,U_Z_FirstName,U_Z_LastName,U_Z_DeptName,U_Z_JobName,U_Z_OrgName,U_Z_PosName,U_Z_SalCode,"
            strqry = strqry & " U_Z_JoinDate,U_Z_NewPosDate  from [@Z_HR_HEM4]  where U_Z_EmpId=" & strEmpid & ""
            oGrid.DataTable.ExecuteQuery(strqry)
            oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee Id"
            oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
            oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
            oGrid.Columns.Item("U_Z_FirstName").TitleObject.Caption = "First Name"
            oGrid.Columns.Item("U_Z_LastName").TitleObject.Caption = "Last Name"
            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department"
            oGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position Name"
            oGrid.Columns.Item("U_Z_JobName").TitleObject.Caption = "Job Name"
            oGrid.Columns.Item("U_Z_OrgName").TitleObject.Caption = "Organization Name"
            oGrid.Columns.Item("U_Z_SalCode").TitleObject.Caption = "Salary Code"
            oGrid.Columns.Item("U_Z_NewPosDate").TitleObject.Caption = "Effective To Date"
            oGrid.Columns.Item("U_Z_JoinDate").TitleObject.Caption = "Effective From Date"
        End If
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_ViewEmpDetails Then
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
