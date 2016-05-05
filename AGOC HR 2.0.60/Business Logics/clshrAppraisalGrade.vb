Public Class clshrAppraisalGrade
    Inherits clsBase
    Private WithEvents SBO_Application As SAPbouiCOM.Application
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oComboColumn As SAPbouiCOM.ComboBoxColumn
    Private oCheckbox As SAPbouiCOM.CheckBox
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private oTemp As SAPbobsCOM.Recordset
    Private InvBaseDocNo, strname As String
    Private InvForConsumedItems As Integer
    Private oMenuobject As Object
    Private blnFlag As Boolean = False
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_AppGrade) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_AppGrade, frm_hr_AppGrade)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        Databind(oForm)
    End Sub
#Region "Databind"
    Private Sub Databind(ByVal aform As SAPbouiCOM.Form)
        Try
            aform.Freeze(True)
            oGrid = aform.Items.Item("5").Specific
            dtTemp = oGrid.DataTable
            dtTemp.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_CODE], T0.[U_Z_Desc],T0.[U_Z_Ratefrom], T0.[U_Z_RateTo], T0.[U_Z_Grade] FROM [@Z_HR_APPGRE]  T0 order by Code")
            oGrid.DataTable = dtTemp
            Formatgrid(oGrid)
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub
#End Region


#Region "FormatGrid"
    Private Sub Formatgrid(ByVal agrid As SAPbouiCOM.Grid)
        agrid.Columns.Item("Code").Visible = False
        agrid.Columns.Item("Name").Visible = False
        agrid.Columns.Item("U_Z_CODE").TitleObject.Caption = "Appraisal Grade Code"
        agrid.Columns.Item("U_Z_Desc").TitleObject.Caption = "Appraisal Grade Description"
        agrid.Columns.Item("U_Z_Ratefrom").TitleObject.Caption = "Rating From"
        agrid.Columns.Item("U_Z_RateTo").TitleObject.Caption = "Rating To"
        agrid.Columns.Item("U_Z_Grade").TitleObject.Caption = "Grade"
        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
#End Region

#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid)
        If aGrid.DataTable.GetValue("U_Z_CODE", aGrid.DataTable.Rows.Count - 1) <> "" Then
            aGrid.DataTable.Rows.Add()
            aGrid.Columns.Item(2).Click(aGrid.DataTable.Rows.Count - 1, False)
        End If
    End Sub
#End Region

#Region "CommitTrans"
    Private Sub Committrans(ByVal strChoice As String)
        Dim oTemprec, oItemRec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strChoice = "Cancel" Then
            oTemprec.DoQuery("Update [@Z_HR_APPGRE] set NAME=CODE where Name Like '%D'")
        Else
            oTemprec.DoQuery("Delete from  [@Z_HR_APPGRE]  where NAME Like '%D'")
        End If

    End Sub
#End Region

#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode, strECode, strESocial, strEname, strETax, strGLAcc, strType As String

        oGrid = aform.Items.Item("5").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            oApplication.Utilities.Message("Processing....", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            If oGrid.DataTable.GetValue(2, intRow) <> "" Or oGrid.DataTable.GetValue(3, intRow) <> "" Then
                strCode = oGrid.DataTable.GetValue(0, intRow)
                strECode = oGrid.DataTable.GetValue(2, intRow)
                strEname = oGrid.DataTable.GetValue(3, intRow)
                strGLAcc = oGrid.DataTable.GetValue(4, intRow)

                oUserTable = oApplication.Company.UserTables.Item("Z_HR_APPGRE")
                If oGrid.DataTable.GetValue(0, intRow) = "" Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_HR_APPGRE", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_CODE").Value = oGrid.DataTable.GetValue("U_Z_CODE", intRow).ToString.ToUpper()
                    oUserTable.UserFields.Fields.Item("U_Z_Desc").Value = (oGrid.DataTable.GetValue("U_Z_Desc", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_Ratefrom").Value = (oGrid.DataTable.GetValue("U_Z_Ratefrom", intRow))
                     oUserTable.UserFields.Fields.Item("U_Z_RateTo").Value = (oGrid.DataTable.GetValue("U_Z_RateTo", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_Grade").Value = oGrid.DataTable.GetValue("U_Z_Grade", intRow)
                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    Else
                    End If
                Else
                    strCode = oGrid.DataTable.GetValue(0, intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.UserFields.Fields.Item("U_Z_CODE").Value = oGrid.DataTable.GetValue("U_Z_CODE", intRow).ToString.ToUpper()
                        oUserTable.UserFields.Fields.Item("U_Z_Desc").Value = (oGrid.DataTable.GetValue("U_Z_Desc", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_Ratefrom").Value = oGrid.DataTable.GetValue("U_Z_Ratefrom", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_RateTo").Value = (oGrid.DataTable.GetValue("U_Z_RateTo", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_Grade").Value = oGrid.DataTable.GetValue("U_Z_Grade", intRow)
                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Committrans("Cancel")
                            Return False
                        Else
                        End If
                    End If
                End If
            End If
        Next
        oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
        Committrans("Add")
        Databind(aform)
    End Function
#End Region

   
#Region "Remove Row"
    Private Sub RemoveRow(ByVal intRow As Integer, ByVal agrid As SAPbouiCOM.Grid)
        Dim strCode, strname As String
        Dim otemprec As SAPbobsCOM.Recordset
        For intRow = 0 To agrid.DataTable.Rows.Count - 1
            If agrid.Rows.IsSelected(intRow) Then
                strCode = agrid.DataTable.GetValue(2, intRow)
                strname = agrid.DataTable.GetValue(3, intRow)
                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                'otemprec.DoQuery("Select * from ""@Z_HR_SALST1"" where ""U_Z_AllCode""='" & strCode & "'")
                'If otemprec.RecordCount > 0 Then
                '    oApplication.Utilities.Message("Allowance already mapped in Salary scale", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '    Exit Sub
                'End If
                oApplication.Utilities.ExecuteSQL(oTemp, "update [@Z_HR_APPGRE] set  NAME =NAME +'D'  where U_Z_CODE='" & strCode & "'")
                agrid.DataTable.Rows.Remove(intRow)
                Exit Sub
            End If
        Next
        oApplication.Utilities.Message("No row selected", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
    End Sub
#End Region


#Region "Validate Grid details"
    Private Function validation(ByVal aGrid As SAPbouiCOM.Grid) As Boolean
        Dim strECode, strECode1, strEname, strEname1 As String
        For intRow As Integer = 0 To aGrid.DataTable.Rows.Count - 1
            strECode = aGrid.DataTable.GetValue("U_Z_CODE", intRow)
            strEname = aGrid.DataTable.GetValue("U_Z_Desc", intRow)
            For intInnerLoop As Integer = intRow To aGrid.DataTable.Rows.Count - 1
                strECode1 = aGrid.DataTable.GetValue("U_Z_CODE", intInnerLoop)
                strEname1 = aGrid.DataTable.GetValue("U_Z_Desc", intInnerLoop)
                If strECode1 <> "" And strEname1 = "" Then
                    oApplication.Utilities.Message("Name can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                If strECode1 = "" And strEname1 <> "" Then
                    oApplication.Utilities.Message("Code can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                If strECode = strECode1 And intRow <> intInnerLoop Then
                    oApplication.Utilities.Message("This strEcode already exists. Code no : " & strECode, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aGrid.Columns.Item(2).Click(intInnerLoop, , 1)
                    Return False
                End If
            Next
        Next
        Return True
    End Function

#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_AppGrade Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                If pVal.ItemUID = "2" Then
                                    Committrans("Cancel")
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "13" Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    If validation(oGrid) = True Then
                                        AddtoUDT1(oForm)
                                    End If
                                End If
                                If pVal.ItemUID = "3" Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    AddEmptyRow(oGrid)
                                End If
                                If pVal.ItemUID = "4" Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    RemoveRow(pVal.Row, oGrid)
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
                Case mnu_hr_AppGrade
                    LoadForm()
                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("5").Specific
                    If pVal.BeforeAction = False Then
                        AddEmptyRow(oGrid)
                    End If

                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("5").Specific
                    If pVal.BeforeAction = True Then
                        RemoveRow(1, oGrid)
                        BubbleEvent = False
                        Exit Sub
                    End If

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
    Private Sub SBO_Application_MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean) Handles SBO_Application.MenuEvent
        Try
            If pVal.BeforeAction = False Then
                Select Case pVal.MenuUID
                    Case mnu_hr_AppGrade
                        oMenuobject = New clshrAppraisalGrade
                        oMenuobject.MenuEvent(pVal, BubbleEvent)
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub

End Class
