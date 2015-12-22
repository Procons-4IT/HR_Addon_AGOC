Public Class clshrAssignTraPlan
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox, oCombobox1 As SAPbouiCOM.ComboBox
    Private oComboColumn, oComboColumn1 As SAPbouiCOM.ComboBoxColumn
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oGrid As SAPbouiCOM.Grid
    Private oColumn As SAPbouiCOM.Column
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo, strcode As String
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Public Sub LoadForm(ByVal oForm As SAPbouiCOM.Form, ByVal Empid As String)
        oForm = oApplication.Utilities.LoadForm(xml_hr_AssignTraPlan, frm_hr_AssignTraPlan)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oApplication.Utilities.setEdittextvalue(oForm, "4", Empid)
        FillDepartment(oForm)
        AddChooseFromList(oForm)
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        ' databind(oForm)
        Dim oSlpRS As SAPbobsCOM.Recordset
        oCombobox = oForm.Items.Item("1000002").Specific
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select firstName,U_Z_HR_PosiCode,U_Z_HR_PosiName,dept from OHEM where EmpId='" & Empid & "'")
        If oSlpRS.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(oForm, "6", oSlpRS.Fields.Item(0).Value)
            oApplication.Utilities.setEdittextvalue(oForm, "13", oSlpRS.Fields.Item(1).Value)
            oApplication.Utilities.setEdittextvalue(oForm, "15", oSlpRS.Fields.Item(2).Value)
            strcode = oSlpRS.Fields.Item(3).Value
            oCombobox.Select(strcode, SAPbouiCOM.BoSearchKey.psk_ByValue)
            Department(oForm, strcode)
        End If
        oGrid = oForm.Items.Item("1000004").Specific
        Databind2(oForm, Empid)
        '  RestoreSelections(oForm)
        'Databind(Empid, oForm)
        oForm.PaneLevel = 1
        oForm.Freeze(False)
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

            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_OTRAPLA"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)



        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

#End Region


    Private Sub FillDepartment(ByVal sform As SAPbouiCOM.Form)
        oCombobox = sform.Items.Item("1000002").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oSlpRS.DoQuery("Select Code,Remarks from OUDP order by Code")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
    End Sub
    Private Sub Department(ByVal aForm As SAPbouiCOM.Form, ByVal Deptcode As String)
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("select Remarks from OUDP  where Code=" & Deptcode & "")
        If oSlpRS.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(aForm, "19", oSlpRS.Fields.Item(0).Value)
        End If
    End Sub
    Private Sub Databind2(ByVal aForm As SAPbouiCOM.Form, ByVal strempid As String)
        Dim strqry As String
        oForm = aForm
        Try
            oForm.Freeze(True)
            Dim otemp As SAPbobsCOM.Recordset
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oGrid = oForm.Items.Item("1000004").Specific
            oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_1")
            strqry = "select Code,U_Z_TraCode,U_Z_TraName,U_Z_EffeFromDt,U_Z_EffeToDt"
            strqry = strqry & " from [@Z_HR_OASSTP] where U_Z_EmpId='" & strempid & "' "
            oGrid.DataTable.ExecuteQuery(strqry)
            FormatGrid(oGrid)
            oForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
#Region "Format Grid"
    Private Sub FormatGrid(ByVal aGrid As SAPbouiCOM.Grid)
        Dim otemp As SAPbobsCOM.Recordset
        otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        aGrid.Columns.Item("Code").TitleObject.Caption = "Code"
        aGrid.Columns.Item("Code").Visible = False
        aGrid.Columns.Item("U_Z_TraCode").TitleObject.Caption = "Travel Code"
        oEditTextColumn = aGrid.Columns.Item("U_Z_TraCode")
        oEditTextColumn.ChooseFromListUID = "CFL1"
        oEditTextColumn.ChooseFromListAlias = "U_Z_TraCode"
        aGrid.Columns.Item("U_Z_TraName").TitleObject.Caption = "Travel Description"
        aGrid.Columns.Item("U_Z_TraName").Editable = False
        aGrid.Columns.Item("U_Z_EffeFromDt").TitleObject.Caption = "Effect From Date"
        aGrid.Columns.Item("U_Z_EffeToDt").TitleObject.Caption = "Effect To Date"
        aGrid.AutoResizeColumns()
        aGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
#End Region

#Region "AddToUDT"
    Private Function AddToUDT(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim strTable, strCode, strType, strAccountCode, strqry, strDeptcode, strStatus As String
        Dim strempname, strposcode, strposname, strdeptname As String
        Dim strcount, strEmpId As Integer
        Dim dblValue As Double
        Dim dt As Date
        Dim oUserTable, oUserTable1 As SAPbobsCOM.UserTable
        Dim oValidateRS, otemp2, oTest As SAPbobsCOM.Recordset
        Dim otemp, otemp1, otemprs As SAPbobsCOM.Recordset
        If oApplication.Company.InTransaction() Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        oApplication.Company.StartTransaction()
        Try

            strEmpId = oApplication.Utilities.getEdittextvalue(aForm, "4")
            strempname = oApplication.Utilities.getEdittextvalue(aForm, "6")
            strposcode = oApplication.Utilities.getEdittextvalue(aForm, "13")
            strposname = oApplication.Utilities.getEdittextvalue(aForm, "15")
            strdeptname = oApplication.Utilities.getEdittextvalue(aForm, "19")
            oCombobox = oForm.Items.Item("1000002").Specific
            Try
                strDeptcode = oCombobox.Selected.Value
            Catch ex As Exception
                strDeptcode = ""
            End Try
            oUserTable = oApplication.Company.UserTables.Item("Z_HR_OASSTP")
            oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            dt = Now.Date
            oGrid = aForm.Items.Item("1000004").Specific
            strTable = "@Z_HR_OASSTP"
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                If oUserTable.GetByKey(oGrid.DataTable.GetValue("Code", intRow)) Then
                    oUserTable.UserFields.Fields.Item("U_Z_TraCode").Value = oGrid.DataTable.GetValue("U_Z_TraCode", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_TraName").Value = oGrid.DataTable.GetValue("U_Z_TraName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EffeFromDt").Value = oApplication.Utilities.GetDateTimeValue(oGrid.DataTable.GetValue("U_Z_EffeFromDt", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_EffeToDt").Value = oApplication.Utilities.GetDateTimeValue(oGrid.DataTable.GetValue("U_Z_EffeToDt", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_EmpId").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_EmpName").Value = strempname
                    oUserTable.UserFields.Fields.Item("U_Z_PosCode").Value = strposcode
                    oUserTable.UserFields.Fields.Item("U_Z_PosName").Value = strposname
                    oUserTable.UserFields.Fields.Item("U_Z_Dept").Value = strDeptcode
                    oUserTable.UserFields.Fields.Item("U_Z_DeptName").Value = strdeptname
                    If oUserTable.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        If oApplication.Company.InTransaction() Then
                            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                        End If
                        Return False
                    Else
                        oApplication.Utilities.Message("Operation Completed Successfully...", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                    End If
                Else
                    ' strStatus = oGrid.DataTable.GetValue("U_Z_Status", intRow)
                    oUserTable.Code = oGrid.DataTable.GetValue("Code", intRow)
                    oUserTable.Name = oGrid.DataTable.GetValue("Code", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_TraCode").Value = oGrid.DataTable.GetValue("U_Z_TraCode", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_TraName").Value = oGrid.DataTable.GetValue("U_Z_TraName", intRow)
                    'oUserTable.UserFields.Fields.Item("U_Z_EffeFromDt").Value = oGrid.DataTable.GetValue("U_Z_EffeFromDt", intRow)
                    ' oUserTable.UserFields.Fields.Item("U_Z_EffeToDt").Value = oGrid.DataTable.GetValue("U_Z_EffeToDt", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_EffeFromDt").Value = oApplication.Utilities.GetDateTimeValue(oGrid.DataTable.GetValue("U_Z_EffeFromDt", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_EffeToDt").Value = oApplication.Utilities.GetDateTimeValue(oGrid.DataTable.GetValue("U_Z_EffeToDt", intRow))
                    ' oUserTable.UserFields.Fields.Item("U_Z_Status").Value = strStatus
                    oUserTable.UserFields.Fields.Item("U_Z_EmpId").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_EmpName").Value = strempname
                    oUserTable.UserFields.Fields.Item("U_Z_PosCode").Value = strposcode
                    oUserTable.UserFields.Fields.Item("U_Z_PosName").Value = strposname
                    oUserTable.UserFields.Fields.Item("U_Z_Dept").Value = strDeptcode
                    oUserTable.UserFields.Fields.Item("U_Z_DeptName").Value = strdeptname
                    If oUserTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        If oApplication.Company.InTransaction() Then
                            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                        End If
                    End If
                End If

                'If oUserTable.GetByKey(oGrid.DataTable.GetValue("Code", intRow)) Then
                '    oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                '    strSQL = "Update  [@Z_HR_ASSTP1] set Name=Code where U_Z_RefCode=" & oGrid.DataTable.GetValue("Code", intRow) & ""
                '    oTest.DoQuery(strSQL)
                'End If
            Next
            RestoreSelections(aForm, True)
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            End If
        End Try
        oUserTable = Nothing
        Return True
    End Function

    Private Function AddtoHeader(ByVal aTravelCode As String, ByVal aform As SAPbouiCOM.Form, ByVal aRow As Integer) As String
        Dim strTable, strCode, strType, strAccountCode, strqry, strDeptcode, strStatus As String
        Dim strempname, strposcode, strposname, strdeptname As String
        Dim strcount, strEmpId As Integer
        Dim dblValue As Double
        Dim dt As Date
        Dim oUserTable, oUserTable1 As SAPbobsCOM.UserTable
        Dim oValidateRS, otemp2, oTest As SAPbobsCOM.Recordset
        Dim otemp, otemp1, otemprs As SAPbobsCOM.Recordset
        If oApplication.Company.InTransaction() Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        oApplication.Company.StartTransaction()
        Try
            strEmpId = oApplication.Utilities.getEdittextvalue(aform, "4")
            strempname = oApplication.Utilities.getEdittextvalue(aform, "6")
            strposcode = oApplication.Utilities.getEdittextvalue(aform, "13")
            strposname = oApplication.Utilities.getEdittextvalue(aform, "15")
            strdeptname = oApplication.Utilities.getEdittextvalue(aform, "19")
            oCombobox = oForm.Items.Item("1000002").Specific
            Try
                strDeptcode = oCombobox.Selected.Value
            Catch ex As Exception
                strDeptcode = ""
            End Try
            oUserTable = oApplication.Company.UserTables.Item("Z_HR_OASSTP")
            oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            dt = Now.Date
            oGrid = aform.Items.Item("1000004").Specific
            strTable = "@Z_HR_OASSTP"
            strTable = oApplication.Utilities.getMaxCode(strTable, "Code")
            'For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1

            '  strStatus = oGrid.DataTable.GetValue("U_Z_Status", aRow)
            oUserTable.Code = strTable
            oUserTable.Name = strTable & "NX"
            oUserTable.UserFields.Fields.Item("U_Z_TraCode").Value = aTravelCode
            oUserTable.UserFields.Fields.Item("U_Z_EmpId").Value = strEmpId
            oUserTable.UserFields.Fields.Item("U_Z_EmpName").Value = strempname
            oUserTable.UserFields.Fields.Item("U_Z_PosCode").Value = strposcode
            oUserTable.UserFields.Fields.Item("U_Z_PosName").Value = strposname
            oUserTable.UserFields.Fields.Item("U_Z_Dept").Value = strDeptcode
            oUserTable.UserFields.Fields.Item("U_Z_DeptName").Value = strdeptname
            If oUserTable.Add <> 0 Then
                oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                If oApplication.Company.InTransaction() Then
                    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                End If
                Return ""
            Else
                oGrid.DataTable.SetValue("Code", aRow, strTable)
                If oApplication.Company.InTransaction() Then
                    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                End If
                Return strTable
            End If
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            End If
        End Try
        oUserTable = Nothing
        Return True
    End Function
#End Region
    Private Function RestoreSelections(ByVal aForm As SAPbouiCOM.Form, ByVal flag As Boolean) As Boolean
        Try
            Dim oTest, oTemp As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim strDept, Reqno, strsql, Tracode, strsql1 As String
            If flag = False Then
                strsql1 = "SElect Code from [@Z_HR_OASSTP] where name like '%NX' and  U_Z_EmpId='" & oApplication.Utilities.getEdittextvalue(aForm, "4") & "'"
                oTemp.DoQuery("Delete from [@Z_HR_ASSTP1] where U_Z_RefCode in (" & strsql1 & ")")
                oTemp.DoQuery("Delete from [@Z_HR_OASSTP] where name like '%NX' and  U_Z_EmpId='" & oApplication.Utilities.getEdittextvalue(aForm, "4") & "'")
                oTemp.DoQuery("Update  [@Z_HR_ASSTP1] set Name=Code where U_Z_EmpID='" & oApplication.Utilities.getEdittextvalue(aForm, "4") & "'")
                oTemp.DoQuery("Update  [@Z_HR_OASSTP] set Name=Code where name like '%ND' and  U_Z_EmpId='" & oApplication.Utilities.getEdittextvalue(aForm, "4") & "'")
            Else
                strsql1 = "SElect Code from [@Z_HR_OASSTP] where name like '%ND' and  U_Z_EmpId='" & oApplication.Utilities.getEdittextvalue(aForm, "4") & "'"
                oTemp.DoQuery("Delete from [@Z_HR_ASSTP1] where U_Z_RefCode in (" & strsql1 & ")")
                oTemp.DoQuery("Delete from [@Z_HR_OASSTP] where name like '%ND' and  U_Z_EmpId='" & oApplication.Utilities.getEdittextvalue(aForm, "4") & "'")
                oTemp.DoQuery("Update  [@Z_HR_ASSTP1] set Name=Code where U_Z_EmpID='" & oApplication.Utilities.getEdittextvalue(aForm, "4") & "'")
                oTemp.DoQuery("Update  [@Z_HR_OASSTP] set Name=Code where name like '%NX' and  U_Z_EmpId='" & oApplication.Utilities.getEdittextvalue(aForm, "4") & "'")
            End If




            '   strsql = "Delete from [@Z_HR_ASSTP1] where  Name Like '%NX'"
            ' oTest.DoQuery(strsql)

            'Next
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
    Private Function validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        oGrid = aForm.Items.Item("1000004").Specific
        Dim dtFrmdate, dtEndDate As Date
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.DataTable.GetValue("U_Z_TraCode", intRow) <> "" Then
                Dim strdate As String = oGrid.DataTable.GetValue("U_Z_EffeFromDt", intRow)
                If strdate = "" Then
                    oApplication.Utilities.Message("Enter Effective From Date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    dtFrmdate = oGrid.DataTable.GetValue("U_Z_EffeFromDt", intRow)
                End If
                strdate = oGrid.DataTable.GetValue("U_Z_EffeToDt", intRow)
                If strdate = "" Then
                    oApplication.Utilities.Message("Enter Effective To Date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    dtEndDate = oGrid.DataTable.GetValue("U_Z_EffeToDt", intRow)
                End If
                If dtFrmdate > dtEndDate Then
                    oApplication.Utilities.Message("Effective From Date Should be Less Than Or Equal To Effective To Date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If

            End If
            Return True

        Next
    End Function



#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_AssignTraPlan Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "2" Then
                                    RestoreSelections(oForm, False)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1000004" And pVal.ColUID = "U_Z_TraCode" And pVal.CharPressed <> 9 Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    If oGrid.DataTable.GetValue("Code", pVal.Row) <> "" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1000004" And pVal.ColUID = "U_Z_TraCode" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    If oGrid.DataTable.GetValue("Code", pVal.Row) <> "" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_DOUBLE_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1000004" Then
                                    oGrid = oForm.Items.Item("1000004").Specific
                                    oGrid.Columns.Item("RowsHeader").Click(pVal.Row)
                                    oForm.Items.Item("20").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                'Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                '    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                '    oGrid = oForm.Items.Item("1000004").Specific
                                '    If pVal.ItemUID = "1000004" And pVal.ColUID = "U_Z_TraCode" Then
                                '        Dim stCode, stCode1 As String
                                '        strcode = oApplication.Utilities.getMaxCode("@Z_HR_OASSTP", "Code")
                                '        oComboColumn = oGrid.Columns.Item("U_Z_TraCode")
                                '        stCode1 = oComboColumn.GetSelectedValue(pVal.Row).Value
                                '        stCode = oComboColumn.GetSelectedValue(pVal.Row).Description
                                '        oGrid.DataTable.SetValue("TravelName", pVal.Row, stCode)
                                '        oGrid.DataTable.SetValue("Code", pVal.Row, strcode)
                                '    End If


                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                  
                                    Case "8"
                                        If oApplication.SBO_Application.MessageBox("Do you want Assign the Employee Travel Plan?", , "Yes", "No") = 2 Then
                                            ' RestoreSelections(oForm)
                                            Exit Sub
                                        Else
                                            If validation(oForm) = False Then
                                                Exit Sub
                                            End If
                                            If AddToUDT(oForm) = True Then
                                                oApplication.Utilities.Message("Employee Travel Plan Completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                                oForm.Close()
                                            Else
                                                '  RestoreSelections(oForm)
                                            End If
                                        End If
                                    Case "20"
                                        Dim empid, Tracode, TraName, empName, RefCode As String
                                        oGrid = oForm.Items.Item("1000004").Specific
                                        empid = oApplication.Utilities.getEdittextvalue(oForm, "4")
                                        empName = oApplication.Utilities.getEdittextvalue(oForm, "6")
                                        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                            If oGrid.Rows.IsSelected(intRow) Then
                                                Tracode = oGrid.DataTable.GetValue("U_Z_TraCode", intRow)
                                                TraName = oGrid.DataTable.GetValue("U_Z_TraName", intRow)
                                                RefCode = oGrid.DataTable.GetValue("Code", intRow)
                                                Dim objct As New clshrAssExpenses
                                                objct.LoadForm(empid, Tracode, empName, TraName, RefCode)
                                                'Else
                                                '    oApplication.Utilities.Message("No rows selected", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            End If
                                        Next
                                End Select
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1 As String
                                Dim sCHFL_ID, val, val2, strRefCode As String
                                Dim intChoice As Integer
                                Dim codebar As String
                                Try
                                    oCFLEvento = pVal
                                    sCHFL_ID = oCFLEvento.ChooseFromListUID
                                    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                    oCFL = oForm.ChooseFromLists.Item(sCHFL_ID)
                                    If (oCFLEvento.BeforeAction = False) Then
                                        Dim oDataTable As SAPbouiCOM.DataTable
                                        oDataTable = oCFLEvento.SelectedObjects
                                        intChoice = 0
                                        oForm.Freeze(True)
                                        If pVal.ItemUID = "1000004" And pVal.ColUID = "U_Z_TraCode" Then
                                            val1 = oDataTable.GetValue("U_Z_TraCode", 0)
                                            val = oDataTable.GetValue("U_Z_TraName", 0)
                                            oGrid = oForm.Items.Item("1000004").Specific
                                            oGrid.DataTable.SetValue("U_Z_TraName", pVal.Row, val)

                                            val2 = oGrid.DataTable.GetValue("Code", pVal.Row)
                                            If val2 <> "" Then
                                                val2 = oGrid.DataTable.GetValue("Code", pVal.Row)
                                            Else
                                                val2 = AddtoHeader(val1, oForm, pVal.Row)
                                            End If
                                            If val2 <> "" Then
                                                strRefCode = oApplication.Utilities.ReturnRefCode(val1, oApplication.Utilities.getEdittextvalue(oForm, "4"), val2)
                                            Else
                                                strRefCode = oApplication.Utilities.ReturnRefCode(val1, oApplication.Utilities.getEdittextvalue(oForm, "4"))
                                            End If
                                            oGrid.DataTable.SetValue("Code", pVal.Row, strRefCode)
                                            oGrid.DataTable.SetValue("U_Z_TraCode", pVal.Row, val1)
                                        End If
                                        oForm.Freeze(False)
                                    End If
                                Catch ex As Exception
                                    oForm.Freeze(False)
                                End Try
                                oForm.Freeze(False)
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
                ' Case mnu_hr_ExpApproval
                'LoadForm()
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        oGrid = oForm.Items.Item("1000004").Specific
                        oGrid.DataTable.Rows.Add()
                    End If
                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        oGrid = oForm.Items.Item("1000004").Specific
                        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                            If oGrid.Rows.IsSelected(intRow) Then
                                Dim strTravelCode, strEmpid As String
                                Dim strCode As String = oGrid.DataTable.GetValue("Code", intRow)
                                Dim otest As SAPbobsCOM.Recordset
                                otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                otest.DoQuery("Update  [@Z_HR_OASSTP] set Name=Name + '_ND' where code='" & strCode & "'")
                                oGrid.DataTable.Rows.Remove(intRow)
                            End If
                        Next
                    Else
                        oGrid = oForm.Items.Item("1000004").Specific
                        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                            If oGrid.Rows.IsSelected(intRow) Then
                                Dim strTravelCode, strEmpid As String
                                Dim strCode As String = oGrid.DataTable.GetValue("Code", intRow)
                                Dim otest As SAPbobsCOM.Recordset
                                otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                otest.DoQuery(" select * from [@Z_HR_OTRAREQ]  where U_Z_TraDocCode='" & strCode & "'")
                                If otest.RecordCount > 0 Then
                                    oApplication.Utilities.Message("Travel request already exists. You can not delete the travel Plan.", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    BubbleEvent = False
                                    Exit Sub
                                Else
                                    otest.DoQuery("Update  [@Z_HR_OASSTP] set Name=Name + '_ND' where code='" & strCode & "'")
                                    ' oGrid.DataTable.Rows.Remove(intRow)
                                    Exit Sub
                                    ' oGrid.DataTable.Rows.Remove(intRow)
                                End If
                            End If
                        Next

                    End If
                 
                  

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
