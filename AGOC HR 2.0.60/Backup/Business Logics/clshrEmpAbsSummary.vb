Public Class clshrEmpAbsSummary
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
    Public Sub LoadForm(ByVal Empid As String, ByVal Agendacode As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_EmpAbsSummary) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_EmpAbsSummary, frm_hr_EmpAbsSummary)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        Databind(Empid, Agendacode, oForm)
        oForm.Freeze(False)
    End Sub
    Private Sub Databind(ByVal strempid As String, ByVal Traincode As String, ByVal aForm As SAPbouiCOM.Form)
        Dim strqry As String
        oGrid = aForm.Items.Item("1").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
        strqry = " select U_Z_TrainCode,U_Z_HREmpID,U_Z_HREmpName,U_Z_DeptName,U_Z_Date,U_Z_Hours,U_Z_Remarks  from [@Z_HR_TRIN2] where U_Z_HREmpID='" & strempid & "' and U_Z_TrainCode='" & Traincode & "' "
        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("U_Z_TrainCode").TitleObject.Caption = "Agenda Code"
        oEditTextColumn = oGrid.Columns.Item("U_Z_TrainCode")
        oEditTextColumn.LinkedObjectType = "Z_HR_OTRIN"
        oGrid.Columns.Item("U_Z_HREmpID").TitleObject.Caption = "Employee Id"
        oEditTextColumn = oGrid.Columns.Item("U_Z_HREmpID")
        oEditTextColumn.LinkedObjectType = 171
        oGrid.Columns.Item("U_Z_HREmpName").TitleObject.Caption = "Employee Name"
        oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department"
        oGrid.Columns.Item("U_Z_Date").TitleObject.Caption = "Date"
        oGrid.Columns.Item("U_Z_Hours").TitleObject.Caption = "Hours"
        oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_EmpAbsSummary Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1" And pVal.ColUID = "U_Z_TrainCode" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcode As String = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    oApplication.Utilities.OpenMasterinLink(oForm, "AgendaCode", strcode)
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
