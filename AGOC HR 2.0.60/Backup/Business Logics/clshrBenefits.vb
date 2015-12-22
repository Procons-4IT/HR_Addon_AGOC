Public Class clshrBenefits
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
    Private oTemp As SAPbobsCOM.Recordset
    Private InvBaseDocNo, strname As String
    Private oMenuobject As Object
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_Benefit) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_Benefit, frm_hr_Benefit)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        AddChooseFromList(oForm)
        Databind(oForm)
    End Sub

#Region "Add Choose From List"
    Private Sub AddChooseFromList(ByVal objForm As SAPbouiCOM.Form)
        Try

            Dim oCFLs As SAPbouiCOM.ChooseFromListCollection
            Dim oCons As SAPbouiCOM.Conditions
            Dim oCon As SAPbouiCOM.Condition
            oCFLs = objForm.ChooseFromLists
            Dim oCFL As SAPbouiCOM.ChooseFromList
            Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
            oCFL = oCFLs.Item("CFL1")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()


        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

#End Region
#Region "Databind"
    Private Sub Databind(ByVal aform As SAPbouiCOM.Form)
        Try
            aform.Freeze(True)
            oGrid = aform.Items.Item("5").Specific
            dtTemp = oGrid.DataTable
            dtTemp.ExecuteQuery("Select * from [@Z_PAY_OCON] order by Code")
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

        agrid.Columns.Item(0).TitleObject.Caption = "Contribution type"
        agrid.Columns.Item(1).TitleObject.Caption = "Contribution Name"
        agrid.Columns.Item("U_Z_FrgnName").TitleObject.Caption = "Second Lanugage Name"
        agrid.Columns.Item("U_Z_CON_GLACC").TitleObject.Caption = "G/L Account"

        oEditTextColumn = agrid.Columns.Item("U_Z_CON_GLACC")
        oEditTextColumn.ChooseFromListUID = "CFL1"
        oEditTextColumn.ChooseFromListAlias = "FormatCode"
        oEditTextColumn.LinkedObjectType = "1"
        'agrid.Columns.Item("U_Z_EOS").TitleObject.Caption = "Affect EOS "
        'agrid.Columns.Item("U_Z_EOS").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        'agrid.Columns.Item("U_Z_EOS").Editable = True
        agrid.Columns.Item("U_Z_PostType").TitleObject.Caption = "Posting Type"
        agrid.Columns.Item("U_Z_PostType").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oComboColumn = agrid.Columns.Item("U_Z_PostType")
        oComboColumn.ValidValues.Add("B", "Business Partner")
        oComboColumn.ValidValues.Add("A", "G/L Account")
        oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both

        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
#End Region

#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid)
        If aGrid.DataTable.GetValue("Code", aGrid.DataTable.Rows.Count - 1) <> "" Then
            aGrid.DataTable.Rows.Add()
            aGrid.Columns.Item(0).Click(aGrid.DataTable.Rows.Count - 1, False)
        End If
    End Sub
#End Region

#Region "CommitTrans"
    Private Sub Committrans(ByVal strChoice As String)
        Dim oTemprec, oItemRec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strChoice = "Cancel" Then
            oTemprec.DoQuery("Update [@Z_PAY_OCON] set Name=Code where Name Like '%D'")
        Else
            oTemprec.DoQuery("Select * from [@Z_PAY_OCON] where Name like '%D'")
            For intRow As Integer = 0 To oTemprec.RecordCount - 1
                oItemRec.DoQuery("delete from [@Z_PAY_OCON] where Name='" & oTemprec.Fields.Item("Name").Value & "' and Code='" & oTemprec.Fields.Item("Code").Value & "'")
                oTemprec.MoveNext()
            Next
            oTemprec.DoQuery("Delete from  [@Z_PAY_OCON]  where Name Like '%D'")
        End If

    End Sub
#End Region

#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode, strECode, strEname, strGLacc As String

        oGrid = aform.Items.Item("5").Specific
        If validation(oGrid) = False Then
            Return False
        End If
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            'strCode = oApplication.Utilities.getMaxCode("@Z_PAY_OCON", "Code")
            oApplication.Utilities.Message("Processing....", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            If oGrid.DataTable.GetValue(0, intRow) <> "" Or oGrid.DataTable.GetValue(1, intRow) <> "" Then
                strECode = oGrid.DataTable.GetValue(0, intRow)
                strEname = oGrid.DataTable.GetValue(1, intRow)
                strGLacc = oGrid.DataTable.GetValue(2, intRow)
                Dim stPosttype As String
                oComboColumn = oGrid.Columns.Item("U_Z_PostType")
                Try
                    stPosttype = oComboColumn.GetSelectedValue(intRow).Value
                Catch ex As Exception
                    stPosttype = "A"
                End Try
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY_OCON")
                If oUserTable.GetByKey(strECode) Then
                    oUserTable.Code = strECode
                    oUserTable.Name = strEname
                    oUserTable.UserFields.Fields.Item("U_Z_FrgnName").Value = oGrid.DataTable.GetValue("U_Z_FrgnName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_CON_GLACC").Value = strGLacc
                    oUserTable.UserFields.Fields.Item("U_Z_PostType").Value = stPosttype
                    If oUserTable.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    Else
                        'If AddToUDT_Employee(strECode, strGLacc) = False Then
                        '    Return False
                        'End If
                    End If
                Else
                    oUserTable.Code = strECode
                    oUserTable.Name = strEname
                    oUserTable.UserFields.Fields.Item("U_Z_FrgnName").Value = oGrid.DataTable.GetValue("U_Z_FrgnName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_CON_GLACC").Value = strGLacc
                    oUserTable.UserFields.Fields.Item("U_Z_PostType").Value = stPosttype
                    If oUserTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    Else
                        'If AddToUDT_Employee(strECode, strGLacc) = False Then
                        '    Return False
                        'End If
                    End If
                End If
            End If
        Next
        oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
        Committrans("Add")
        Databind(aform)
    End Function
#End Region


    Private Function AddToUDT_Employee(ByVal aType As String, ByVal GLAccount As String) As Boolean
        Dim strTable, strEmpId, strCode, strType As String
        Dim dblValue As Double
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim oValidateRS, oTemp As SAPbobsCOM.Recordset
        oUserTable = oApplication.Company.UserTables.Item("Z_PAY3")
        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp.DoQuery("Select * from [OHEM] order by EmpID")
        strTable = "@Z_PAY3"
        strType = aType
        Dim strQuery As String
        If strType <> "" Then


            strQuery = "Update [@Z_PAY3] set U_Z_GLACC='" & GLAccount & "' where U_Z_CONTR_TYPE='" & strType & "'"
            oValidateRS.DoQuery(strQuery)
        End If


        For intRow As Integer = 0 To oTemp.RecordCount - 1
            If strType <> "" Then
                strEmpId = oTemp.Fields.Item("empID").Value
                oValidateRS.DoQuery("Select * from [@Z_PAY3] where U_Z_CONTR_TYPE='" & strType & "' and U_Z_EMPID='" & strEmpId & "'")
                If oValidateRS.RecordCount > 0 Then
                    strCode = oValidateRS.Fields.Item("Code").Value
                Else
                    strCode = ""
                End If

                If strCode <> "" Then ' oUserTable.GetByKey(strCode) Then
                    'oUserTable.Code = strCode
                    'oUserTable.Name = strCode
                    'oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    'oUserTable.UserFields.Fields.Item("U_Z_CONTR_TYPE").Value = strType
                    '' oUserTable.UserFields.Fields.Item("U_Z_CONTR_VALUE").Value = 0
                    'oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLAccount
                    'If oUserTable.Update <> 0 Then
                    '    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    '    Return False
                    'End If
                Else
                    strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode + "N"
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_CONTR_TYPE").Value = strType
                    oUserTable.UserFields.Fields.Item("U_Z_CONTR_VALUE").Value = 0
                    oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLAccount
                    If oUserTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
            End If
            oTemp.MoveNext()
        Next
        oUserTable = Nothing
        Return True
    End Function

#Region "Remove Row"
    Private Sub RemoveRow(ByVal intRow As Integer, ByVal agrid As SAPbouiCOM.Grid)
        Dim strCode, strname As String
        Dim otemprec As SAPbobsCOM.Recordset
        For intRow = 0 To agrid.DataTable.Rows.Count - 1
            If agrid.Rows.IsSelected(intRow) Then
                strCode = agrid.DataTable.GetValue(0, intRow)
                strname = agrid.DataTable.GetValue(1, intRow)
                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                'otemprec.DoQuery("Select * from [@Z_PAY_OCON] where Code='" & strCode & "' and Name='" & strname & "'")
                'If otemprec.RecordCount > 0 And strCode <> "" Then
                '    oApplication.Utilities.Message("Transaction already exists. Can not delete the Bin Details.", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '    Exit Sub
                'End If
                'If oApplication.Utilities.ValidateDeletionMaster(strCode, "Social Benefit") = False Then
                '    Exit Sub
                'End If
                oApplication.Utilities.ExecuteSQL(oTemp, "update [@Z_PAY_OCON] set  Name =Name +'D'  where Code='" & strCode & "'")
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
            strECode = aGrid.DataTable.GetValue(0, intRow)
            strEname = aGrid.DataTable.GetValue(1, intRow)
            If strECode = "" And strEname <> "" Then
                oApplication.Utilities.Message("Code is missing . Code : " & intRow, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If strECode <> "" And strEname = "" Then
                oApplication.Utilities.Message("Name is missing . Code : " & intRow, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            For intInnerLoop As Integer = intRow To aGrid.DataTable.Rows.Count - 1
                strECode1 = aGrid.DataTable.GetValue(0, intInnerLoop)
                strEname1 = aGrid.DataTable.GetValue(1, intInnerLoop)
                If strECode = strECode1 And intRow <> intInnerLoop Then
                    oApplication.Utilities.Message("Contribution already exists. Code no : " & strECode1, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aGrid.Columns.Item(0).Click(intInnerLoop, , 1)
                    Return False
                End If
            Next
        Next
        Return True
    End Function

#End Region

    Private Function AddtoUDT(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim oTemp As SAPbobsCOM.Recordset
        Dim strDocEntry As String
        oGrid = aForm.Items.Item("5").Specific
        If validation(oGrid) = False Then
            Return False
        End If
        ' AssignLineNo(aForm)
        Return True
    End Function
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_Benefit Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "5" And pVal.ColUID = "Code" And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    'If oApplication.Utilities.ValidateDeletionMaster(oGrid.DataTable.GetValue("Code", pVal.Row), "Social Benefit") = False Then
                                    '    BubbleEvent = False
                                    '    Exit Sub
                                    'End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "5" And pVal.ColUID = "Code" And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item("5").Specific
                                    'If oApplication.Utilities.ValidateDeletionMaster(oGrid.DataTable.GetValue("Code", pVal.Row), "Social Benefit") = False Then
                                    '    BubbleEvent = False
                                    '    Exit Sub
                                    'End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                If pVal.ItemUID = "2" Then
                                    Committrans("Cancel")
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
                                Dim intChoice, introw As Integer
                                Try
                                    oCFLEvento = pVal
                                    sCHFL_ID = oCFLEvento.ChooseFromListUID
                                    oCFL = oForm.ChooseFromLists.Item(sCHFL_ID)
                                    If (oCFLEvento.BeforeAction = False) Then
                                        Dim oDataTable As SAPbouiCOM.DataTable
                                        oDataTable = oCFLEvento.SelectedObjects
                                        oForm.Freeze(True)
                                        oForm.Update()
                                        If pVal.ItemUID = "5" Then
                                            oGrid = oForm.Items.Item("5").Specific
                                            val = oDataTable.GetValue("FormatCode", 0)
                                            Try

                                                oGrid.DataTable.SetValue(pval.ColUID, pVal.Row, val)
                                            Catch ex As Exception
                                            End Try
                                        End If
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
                Case mnu_hr_Benefit
                    LoadForm()
                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("5").Specific
                    If pVal.BeforeAction = False Then
                        AddEmptyRow(oGrid)
                        oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
                    End If

                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("5").Specific
                    If pVal.BeforeAction = True Then
                        RemoveRow(1, oGrid)
                        oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
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
                'Select Case pVal.MenuUID
                '    Case mnu_Contribution
                '        oMenuobject = New clsContribution
                '        oMenuobject.MenuEvent(pVal, BubbleEvent)

                '
                '   End Select
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
