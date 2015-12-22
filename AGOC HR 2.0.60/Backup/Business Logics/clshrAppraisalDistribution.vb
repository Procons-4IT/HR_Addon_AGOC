Public Class clshrAppraisalDistribution
    Inherits clsBase
    Private WithEvents SBO_Application As SAPbouiCOM.Application
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
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
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_AppDisMaster) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_AppDisMaster, frm_hr_AppDisMaster)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        Databind(oForm)
        AddPFRecord(oForm)
        ' Databind(oForm)
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
            oCFL = oCFLs.Item("CFL_2")
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Postable"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFL = oCFLs.Item("CFL_3")
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
            oGrid = aform.Items.Item("3").Specific
            oGrid.DataTable.ExecuteQuery("SELECT * from ""@Z_HR_OARE"" order by ""Code""")
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
        agrid.Columns.Item(0).Visible = False
        agrid.Columns.Item(1).Visible = False
        agrid.Columns.Item(2).TitleObject.Caption = "Objective"
        agrid.Columns.Item(2).Editable = False
        agrid.Columns.Item(3).TitleObject.Caption = "Weight %"
      
        'oCheckbox = agrid.Columns.Item(5)
        'oCheckbox.Checked = True
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
        'If strChoice = "Cancel" Then
        '    oTemprec.DoQuery("Update [@Z_HR_OARE] set ""Name""=""Code"" where Name Like '%D'")
        'Else
        '    'oTemprec.DoQuery("Select * from [@Z_PAY_OSBM] where NAME like '%D'")
        '    'For intRow As Integer = 0 To oTemprec.RecordCount - 1
        '    '    oItemRec.DoQuery("delete from [@Z_PAY_OSBM] where U_Z_NAME='" & oTemprec.Fields.Item("U_Z_NAME").Value & "' and U_Z_CODE='" & oTemprec.Fields.Item("U_Z_CODE").Value & "'")
        '    '    oTemprec.MoveNext()
        '    'Next
        '    oTemprec.DoQuery("Update [@Z_PAY_OSBM] set Code=Name where Name like '%N'")
        '    oTemprec.DoQuery("Delete from  [@Z_PAY_OSBM]  where NAME Like '%D'")

        'End If

    End Sub
#End Region

#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode, strECode, strESocial, strEname, strETax, strGLAcc As String
        Dim OCHECKBOXCOLUMN As SAPbouiCOM.CheckBoxColumn
        oGrid = aform.Items.Item("3").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            '
            If oGrid.DataTable.GetValue(2, intRow) <> "" Then
                strCode = oGrid.DataTable.GetValue(0, intRow)
                strECode = oGrid.DataTable.GetValue(2, intRow)
                strEname = oGrid.DataTable.GetValue(3, intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_HR_OARE")
                If 1 = 2 Then 'oGrid.DataTable.GetValue(0, intRow) = "" Then
                Else
                    strCode = oGrid.DataTable.GetValue(0, intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Name = strCode
                        oUserTable.UserFields.Fields.Item("U_Z_Obj").Value = oGrid.DataTable.GetValue(2, intRow).ToString.ToUpper()
                        oUserTable.UserFields.Fields.Item("U_Z_Weight").Value = (oGrid.DataTable.GetValue(3, intRow))
                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Committrans("Cancel")
                            Return False
                        End If
                    End If
                End If
            End If
        Next
        oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
        Committrans("Add")
        Databind(aform)
    End Function

    Private Function AddPFRecord(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim oTemp As SAPbobsCOM.Recordset
        Dim strCode, strECode, strESocial, strEname, strETax, strGLAcc As String
        Dim OCHECKBOXCOLUMN As SAPbouiCOM.CheckBoxColumn
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp.DoQuery("Select * from ""@Z_HR_OARE"" where U_Z_Obj='Business Objective'")
        If oTemp.RecordCount > 0 Then
        Else
            oUserTable = oApplication.Company.UserTables.Item("Z_HR_OARE")
            strCode = oApplication.Utilities.getMaxCode("@Z_HR_OARE", "Code")
            oUserTable.Code = strCode
            oUserTable.Name = strCode
            oUserTable.UserFields.Fields.Item("U_Z_Obj").Value = "Business Objective"
            oUserTable.UserFields.Fields.Item("U_Z_Weight").Value = 0
            If oUserTable.Add <> 0 Then
                oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Committrans("Cancel")
                ' Databind(aform)
            Else
                Committrans("Add")
                'Databind(aform)
            End If
        End If

        oTemp.DoQuery("Select * from ""@Z_HR_OARE"" where U_Z_Obj='Comptencies'")
        If oTemp.RecordCount > 0 Then
        Else
            oUserTable = oApplication.Company.UserTables.Item("Z_HR_OARE")
            strCode = oApplication.Utilities.getMaxCode("@Z_HR_OARE", "Code")
            oUserTable.Code = strCode
            oUserTable.Name = strCode
            oUserTable.UserFields.Fields.Item("U_Z_Obj").Value = "Comptencies"
            oUserTable.UserFields.Fields.Item("U_Z_Weight").Value = 0
            If oUserTable.Add <> 0 Then
                oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Committrans("Cancel")
                ' Databind(aform)
            Else
                Committrans("Add")
                'Databind(aform)
            End If
        End If

        oTemp.DoQuery("Select * from ""@Z_HR_OARE"" where U_Z_Obj='People Objective'")
        If oTemp.RecordCount > 0 Then
        Else
            oUserTable = oApplication.Company.UserTables.Item("Z_HR_OARE")
            strCode = oApplication.Utilities.getMaxCode("@Z_HR_OARE", "Code")
            oUserTable.Code = strCode
            oUserTable.Name = strCode
            oUserTable.UserFields.Fields.Item("U_Z_Obj").Value = "People Objective"
            oUserTable.UserFields.Fields.Item("U_Z_Weight").Value = 0
            If oUserTable.Add <> 0 Then
                oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Committrans("Cancel")
                ' Databind(aform)
            Else
                Committrans("Add")
                'Databind(aform)
            End If
        End If

    End Function
#End Region

#Region "Remove Row"
    Private Sub RemoveRow(ByVal intRow As Integer, ByVal agrid As SAPbouiCOM.Grid)
        Dim strCode, strname As String
        Dim otemprec As SAPbobsCOM.Recordset
        For intRow = 0 To agrid.DataTable.Rows.Count - 1
            If agrid.Rows.IsSelected(intRow) Then
                strCode = agrid.DataTable.GetValue(0, intRow)
                strname = agrid.DataTable.GetValue(3, intRow)
                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

                oApplication.Utilities.ExecuteSQL(oTemp, "update [@Z_PAY_OSBM] set NAME =NAME +'D'  where CODE='" & strCode & "'")
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
            strECode = aGrid.DataTable.GetValue(2, intRow)
            strEname = aGrid.DataTable.GetValue(3, intRow)
            For intInnerLoop As Integer = intRow To aGrid.DataTable.Rows.Count - 1
                strECode1 = aGrid.DataTable.GetValue(2, intInnerLoop)
                strEname1 = aGrid.DataTable.GetValue(3, intInnerLoop)
                If strECode1 <> "" And strEname1 = "" Then
                    oApplication.Utilities.Message("Name can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                If strECode1 = "" And strEname1 <> "" Then
                    oApplication.Utilities.Message("Code can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                If strECode = strECode1 And intRow <> intInnerLoop Then
                    oApplication.Utilities.Message("This Social benifit Code  already exists. Code no : " & strECode, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
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
            If pVal.FormTypeEx = frm_hr_AppDisMaster Then
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
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "11" Then
                                    oGrid = oForm.Items.Item("3").Specific
                                    If 1 = 1 Then
                                        AddtoUDT1(oForm)
                                    End If
                                End If
                                'If pVal.ItemUID = "3" Then
                                '    oGrid = oForm.Items.Item("5").Specific
                                '    AddEmptyRow(oGrid)
                                'End If
                                'If pVal.ItemUID = "4" Then
                                '    oGrid = oForm.Items.Item("5").Specific
                                '    RemoveRow(pVal.Row, oGrid)
                                'End If

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
                                        'If pVal.ItemUID = "5" And (pVal.ColUID = "U_Z_CRACCOUNT" Or pVal.ColUID = "U_Z_DRACCOUNT") Then
                                        '    oGrid = oForm.Items.Item("5").Specific
                                        '    val = oDataTable.GetValue("FormatCode", 0)
                                        '    Try
                                        '        oGrid.DataTable.SetValue(pVal.ColUID, pVal.Row, val)
                                        '    Catch ex As Exception
                                        '    End Try
                                        'End If
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
                Case mnu_hr_AppDisMaster
                    If pVal.BeforeAction = False Then
                        LoadForm()
                    End If
                    'Case mnu_ADD_ROW
                    '    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    '    oGrid = oForm.Items.Item("5").Specific
                    '    If pVal.BeforeAction = False Then
                    '        AddEmptyRow(oGrid)
                    '    End If

                    'Case mnu_DELETE_ROW
                    '    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    '    oGrid = oForm.Items.Item("5").Specific
                    '    If pVal.BeforeAction = True Then
                    '        RemoveRow(1, oGrid)
                    '        BubbleEvent = False
                    '        Exit Sub
                    '    End If

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
                    Case mnu_hr_AppDisMaster
                        LoadForm()
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
