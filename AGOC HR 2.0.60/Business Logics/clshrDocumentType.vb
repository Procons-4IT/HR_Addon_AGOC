Public Class clshrDocumentType
    Inherits clsBase
    Private WithEvents SBO_Application As SAPbouiCOM.Application
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oComboColumn As SAPbouiCOM.ComboBoxColumn
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo, strname As String
    Private oTemp As SAPbobsCOM.Recordset
    Private oMenuobject As Object
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        oForm = oApplication.Utilities.LoadForm(xml_hr_DocType, frm_hr_DocType)
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
            dtTemp.ExecuteQuery("SELECT ""Code"", ""Name"", ""U_Z_DocType"",""U_Z_DocDesc"" FROM ""@Z_HR_DOCTY""  order by ""Code""")
            oGrid.DataTable = dtTemp
            Formatgrid(oGrid)
            oApplication.Utilities.assignMatrixLineno(oGrid, aform)
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub
#End Region

#Region "FormatGrid"
    Private Sub Formatgrid(ByVal agrid As SAPbouiCOM.Grid)
        agrid.Columns.Item("Code").TitleObject.Caption = "Code"
        agrid.Columns.Item("Code").Visible = False
        agrid.Columns.Item("Name").TitleObject.Caption = "Name"
        agrid.Columns.Item("Name").Visible = False
        agrid.Columns.Item("U_Z_DocType").TitleObject.Caption = "Document Code"
        agrid.Columns.Item("U_Z_DocDesc").TitleObject.Caption = "Document Description"
        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
#End Region

#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid)
        If aGrid.DataTable.GetValue("U_Z_DocType", aGrid.DataTable.Rows.Count - 1) <> "" Then
            aGrid.DataTable.Rows.Add()
            aGrid.Columns.Item("U_Z_DocType").Click(aGrid.DataTable.Rows.Count - 1, False)
        End If
        oApplication.Utilities.assignMatrixLineno(aGrid, oForm)
    End Sub
#End Region

#Region "CommitTrans"
    Private Sub Committrans(ByVal strChoice As String)
        Dim oTemprec, oItemRec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strChoice = "Cancel" Then
            oTemprec.DoQuery("Update ""@Z_HR_DOCTY"" set ""Name""=""Code"" where ""Name"" Like '%D'")
        Else
            oTemprec.DoQuery("Select * from ""@Z_HR_DOCTY"" where ""Name"" like '%D'")
            For intRow As Integer = 0 To oTemprec.RecordCount - 1
                oItemRec.DoQuery("delete from ""@Z_HR_DOCTY"" where ""Name""='" & oTemprec.Fields.Item("Name").Value & "' and ""Code""='" & oTemprec.Fields.Item("Code").Value & "'")
                oTemprec.MoveNext()
            Next
            oTemprec.DoQuery("Delete from  ""@Z_HR_DOCTY""  where ""Name"" Like '%D'")
        End If

    End Sub
#End Region

#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode, strType As String
        oGrid = aform.Items.Item("5").Specific
        oUserTable = oApplication.Company.UserTables.Item("Z_HR_DOCTY")
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strCode = oGrid.DataTable.GetValue("Code", intRow)
            Try
                strType = oGrid.DataTable.GetValue("U_Z_DocType", intRow)
            Catch ex As Exception
                strType = ""
            End Try

            If strType <> "" Then
                If oUserTable.GetByKey(strCode) Then
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_DocType").Value = oGrid.DataTable.GetValue("U_Z_DocType", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_DocDesc").Value = oGrid.DataTable.GetValue("U_Z_DocDesc", intRow)
                    If oUserTable.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                ElseIf strType <> "" Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_HR_DOCTY", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode + "_N"
                    oUserTable.UserFields.Fields.Item("U_Z_DocType").Value = oGrid.DataTable.GetValue("U_Z_DocType", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_DocDesc").Value = oGrid.DataTable.GetValue("U_Z_DocDesc", intRow)
                    If oUserTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
            End If
        Next
        oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
        Committrans("Add")
        Databind(aform)
        Return True
    End Function
#End Region




#Region "Remove Row"
    Private Sub RemoveRow(ByVal intRow As Integer, ByVal agrid As SAPbouiCOM.Grid)
        Dim strCode, strname As String
        Dim otemprec As SAPbobsCOM.Recordset
        For intRow = 0 To agrid.DataTable.Rows.Count - 1
            If agrid.Rows.IsSelected(intRow) Then
                strCode = agrid.DataTable.GetValue(0, intRow)
                strname = agrid.DataTable.GetValue(1, intRow)
                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oApplication.Utilities.ExecuteSQL(oTemp, "update ""@Z_HR_DOCTY"" set  ""Name"" =""Name"" +'D'  where ""Code""='" & strCode & "'")
                agrid.DataTable.Rows.Remove(intRow)
                oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
                Exit Sub
            End If
        Next
        oApplication.Utilities.Message("No row selected", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
    End Sub
#End Region
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        oGrid = aForm.Items.Item("5").Specific
        Dim strECode, strECode1, strEname, strEname1 As String
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strECode = oGrid.DataTable.GetValue("U_Z_DocType", intRow)
            strEname = oGrid.DataTable.GetValue("U_Z_DocDesc", intRow)
            For intInnerLoop As Integer = intRow To oGrid.DataTable.Rows.Count - 1
                strECode1 = oGrid.DataTable.GetValue("U_Z_DocType", intInnerLoop)
                strEname1 = oGrid.DataTable.GetValue("U_Z_DocDesc", intInnerLoop)
                If strECode1 <> "" And strEname1 = "" Then
                    oApplication.Utilities.Message("Document Description can not be empty", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    oGrid.Columns.Item("U_Z_DocDesc").Click(intInnerLoop, , 1)
                    Return False
                End If
                If strECode1 = "" And strEname1 <> "" Then
                    oApplication.Utilities.Message("Document Code can not be empty", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    oGrid.Columns.Item("U_Z_DocType").Click(intInnerLoop, , 1)
                    Return False
                End If
                If strECode = strECode1 And intRow <> intInnerLoop Then
                    oApplication.Utilities.Message("This Document Code already exists. Code no : " & strECode, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    oGrid.Columns.Item("U_Z_DocType").Click(intInnerLoop, , 1)
                    Return False
                End If
            Next
        Next
        Return True
    End Function

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_DocType Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "2" Then
                                    Committrans("Cancel")
                                End If
                                If pVal.ItemUID = "13" Then
                                    If Validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
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
                                    AddtoUDT1(oForm)
                                End If
                                If pVal.ItemUID = "3" Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    AddEmptyRow(oGrid)
                                End If
                                If pVal.ItemUID = "4" Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    RemoveRow(pVal.Row, oGrid)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim oItm As SAPbobsCOM.Items
                                Dim sCHFL_ID, val As String
                                Dim intChoice As Integer
                                Try
                                    oCFLEvento = pVal
                                    sCHFL_ID = oCFLEvento.ChooseFromListUID
                                    oCFL = oForm.ChooseFromLists.Item(sCHFL_ID)
                                    If (oCFLEvento.BeforeAction = False) Then
                                        Dim oDataTable As SAPbouiCOM.DataTable
                                        oDataTable = oCFLEvento.SelectedObjects
                                        oForm.Freeze(True)
                                        oForm.Update()

                                        oForm.Freeze(False)
                                    End If
                                Catch ex As Exception
                                    oForm.Freeze(False)
                                    'MsgBox(ex.Message)
                                End Try
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
                Case mnu_hr_DocType
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
End Class
