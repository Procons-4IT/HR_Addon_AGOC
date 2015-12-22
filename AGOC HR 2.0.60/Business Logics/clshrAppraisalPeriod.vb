Public Class clshrAppraisalPeriod
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private ocombo, ocombo1, ocombo2, ocombo3 As SAPbouiCOM.ComboBoxColumn
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo, strQuery As String
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Dim oRecSet As SAPbobsCOM.Recordset
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_AppPeriod) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_AppPeriod, frm_AppPeriod)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        Gridbind()
        oForm.Freeze(False)
    End Sub
    Private Sub Gridbind()
        Try
            Dim strqry As String
            oGrid = oForm.Items.Item("1").Specific
            oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
            strqry = "select ""Code"",""U_Z_PerCode"",""U_Z_PerDesc"",""U_Z_PerFrom"",""U_Z_PerTo""  from ""@Z_HR_PERAPP"""
            oGrid.DataTable.ExecuteQuery(strqry)
            formatGrid(oForm)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
    Private Sub formatGrid(ByVal aForm As SAPbouiCOM.Form)
        oGrid = aForm.Items.Item("1").Specific
        oGrid.Columns.Item("Code").Visible = False
        oGrid.Columns.Item("U_Z_PerCode").TitleObject.Caption = "Appraisal Period Code"
        oGrid.Columns.Item("U_Z_PerDesc").TitleObject.Caption = "Appraisal Period Description"
        oGrid.Columns.Item("U_Z_PerFrom").TitleObject.Caption = "Appraisal Period From"

        oGrid.Columns.Item("U_Z_PerFrom").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        ocombo1 = oGrid.Columns.Item("U_Z_PerFrom")
        oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strQuery = "SELECT ""Code"" As ""Code"", ""Name"" As ""Name"" FROM OFPR order by Code desc"
        oRecSet.DoQuery(strQuery)
        ocombo1.ValidValues.Add("", "")
        If Not oRecSet.EoF Then
            For index As Integer = 0 To oRecSet.RecordCount - 1
                If Not oRecSet.EoF Then
                    ocombo1.ValidValues.Add(oRecSet.Fields.Item("Code").Value, oRecSet.Fields.Item("Name").Value)
                    oRecSet.MoveNext()
                End If
            Next
        End If
        ocombo1.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

        oGrid.Columns.Item("U_Z_PerTo").TitleObject.Caption = "Appraisal Period To"

        oGrid.Columns.Item("U_Z_PerTo").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        ocombo1 = oGrid.Columns.Item("U_Z_PerTo")
        oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strQuery = "SELECT ""Code"" As ""Code"", ""Name"" As ""Name"" FROM OFPR order by Code desc"
        oRecSet.DoQuery(strQuery)
        ocombo1.ValidValues.Add("", "")
        If Not oRecSet.EoF Then
            For index As Integer = 0 To oRecSet.RecordCount - 1
                If Not oRecSet.EoF Then
                    ocombo1.ValidValues.Add(oRecSet.Fields.Item("Code").Value, oRecSet.Fields.Item("Name").Value)
                    oRecSet.MoveNext()
                End If
            Next
        End If
        ocombo1.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

        oGrid.AutoResizeColumns()
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            oGrid.RowHeaders.SetText(intRow, intRow + 1)
        Next
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid)
        Try
            If aGrid.DataTable.Rows.Count - 1 < 0 Then
                aGrid.DataTable.Rows.Add()
            End If
            If aGrid.DataTable.GetValue("U_Z_PerCode", aGrid.DataTable.Rows.Count - 1) <> "" Then
                aGrid.DataTable.Rows.Add()
                aGrid.Columns.Item("U_Z_PerCode").Click(aGrid.DataTable.Rows.Count - 1, False)
            End If
            oApplication.Utilities.assignMatrixLineno(aGrid, oForm)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
#End Region
#Region "Remove Row"
    Private Sub RemoveRow(ByVal intRow As Integer, ByVal agrid As SAPbouiCOM.Grid)
        Dim strCode, strname As String
        Dim blnValue As Boolean
        Dim otemprec As SAPbobsCOM.Recordset
        For intRow = 0 To agrid.DataTable.Rows.Count - 1
            If agrid.Rows.IsSelected(intRow) Then
                strCode = agrid.DataTable.GetValue(0, intRow)
                strname = agrid.DataTable.GetValue(1, intRow)
                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                blnValue = RemoveValidate(oForm, strname)
                If blnValue = False Then
                    oApplication.Utilities.ExecuteSQL(otemprec, "update ""@Z_HR_PERAPP"" set  ""Name"" =""Name"" +'D'  where ""Code""='" & strCode & "'")
                    agrid.DataTable.Rows.Remove(intRow)
                    Exit Sub
                Else
                    oApplication.Utilities.Message("Already in Initialize the Appraisal.You can't delete this Period.", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Exit Sub
                End If
            End If
        Next
        oApplication.Utilities.Message("No row selected", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
    End Sub
#End Region
    Private Function RemoveValidate(ByVal aForm As SAPbouiCOM.Form, ByVal PeriodCode As String) As Boolean
        Try
            oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strQuery = "Select * from [@Z_HR_OSEAPP] where U_Z_Period='" & PeriodCode & "'"
            oRecSet.DoQuery(strQuery)
            If oRecSet.RecordCount > 0 Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Throw ex
            Return False
        End Try
    End Function
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form)
        Try
            Dim strCode, strName, strCode1, strName1, strfrom, strTo As String
            oGrid = aForm.Items.Item("1").Specific
            If oGrid.DataTable.Rows.Count > 0 Then
                For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                    strCode = oGrid.DataTable.GetValue("U_Z_PerCode", intRow)
                    strName = oGrid.DataTable.GetValue("U_Z_PerDesc", intRow)
                    ocombo1 = oGrid.Columns.Item("U_Z_PerFrom")
                    strfrom = ocombo1.GetSelectedValue(intRow).Value
                    ocombo1 = oGrid.Columns.Item("U_Z_PerTo")
                    strTo = ocombo1.GetSelectedValue(intRow).Value
                    If strCode = "" Then
                        oApplication.Utilities.Message("Appraisal Period Code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    ElseIf strName = "" Then
                        oApplication.Utilities.Message("Appraisal Period Name is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    ElseIf strfrom = "" Then
                        oApplication.Utilities.Message("Appraisal Period From is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    ElseIf strTo = "" Then
                        oApplication.Utilities.Message("Appraisal Period To is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If

                    If oGrid.DataTable.GetValue("U_Z_PerCode", intRow) <> "" Then
                        strCode = oGrid.DataTable.GetValue("U_Z_PerCode", intRow)
                        strName = oGrid.DataTable.GetValue("U_Z_PerDesc", intRow)
                        For intLoop As Integer = intRow + 1 To oGrid.DataTable.Rows.Count - 1
                            strCode1 = oGrid.DataTable.GetValue("U_Z_PerCode", intLoop)
                            strName1 = oGrid.DataTable.GetValue("U_Z_PerDesc", intLoop)
                            If strCode1 <> "" Then
                                If strCode.ToUpper = strCode1.ToUpper Then
                                    oApplication.Utilities.Message("Appraisal Period Code : This entry already exists : " & strCode1, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    oGrid.Columns.Item("U_Z_PerCode").Click(intLoop)
                                    Return False
                                ElseIf strName.ToUpper = strName1.ToUpper Then
                                    oApplication.Utilities.Message("Appraisal Period Description : This entry already exists : " & strCode, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    oGrid.Columns.Item("U_Z_PerDesc").Click(intLoop)
                                    Return False
                                End If
                            End If
                        Next
                    End If
                Next
            End If
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
        Return True
    End Function
#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        aform.Freeze(True)
        Try
            Dim oUserTable1 As SAPbobsCOM.UserTable
            Dim Headrcode As String
            oGrid = aform.Items.Item("1").Specific
            oUserTable1 = oApplication.Company.UserTables.Item("Z_HR_PERAPP")
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                Headrcode = oGrid.DataTable.GetValue("Code", intRow)
                ocombo1 = oGrid.Columns.Item("U_Z_PerFrom")
                ocombo2 = oGrid.Columns.Item("U_Z_PerTo")
                If oUserTable1.GetByKey(Headrcode) Then
                    oUserTable1.Code = Headrcode
                    oUserTable1.Name = Headrcode
                    oUserTable1.UserFields.Fields.Item("U_Z_PerCode").Value = oGrid.DataTable.GetValue("U_Z_PerCode", intRow)
                    oUserTable1.UserFields.Fields.Item("U_Z_PerDesc").Value = oGrid.DataTable.GetValue("U_Z_PerDesc", intRow)
                    oUserTable1.UserFields.Fields.Item("U_Z_PerFrom").Value = ocombo1.GetSelectedValue(intRow).Value
                    oUserTable1.UserFields.Fields.Item("U_Z_PerTo").Value = ocombo2.GetSelectedValue(intRow).Value
                    If oUserTable1.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                Else
                    Headrcode = oApplication.Utilities.getMaxCode("@Z_HR_PERAPP", "Code")
                    oUserTable1.Code = Headrcode
                    oUserTable1.Name = Headrcode
                    oUserTable1.UserFields.Fields.Item("U_Z_PerCode").Value = oGrid.DataTable.GetValue("U_Z_PerCode", intRow)
                    oUserTable1.UserFields.Fields.Item("U_Z_PerDesc").Value = oGrid.DataTable.GetValue("U_Z_PerDesc", intRow)
                    oUserTable1.UserFields.Fields.Item("U_Z_PerFrom").Value = ocombo1.GetSelectedValue(intRow).Value
                    oUserTable1.UserFields.Fields.Item("U_Z_PerTo").Value = ocombo2.GetSelectedValue(intRow).Value
                    If oUserTable1.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
            Next
            oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            Committrans("Add")
            Gridbind()
            aform.Freeze(False)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Function
#End Region
#Region "CommitTrans"
    Private Sub Committrans(ByVal strChoice As String)
        Dim oTemprec, oItemRec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strChoice = "Cancel" Then
            oTemprec.DoQuery("Update ""@Z_HR_PERAPP"" set ""Name""=""Code"" where ""Name"" Like '%D'")
        Else
            oTemprec.DoQuery("Select * from ""@Z_HR_PERAPP"" where ""Name"" like '%D'")
            For intRow As Integer = 0 To oTemprec.RecordCount - 1
                oItemRec.DoQuery("delete from ""@Z_HR_PERAPP"" where ""Name""='" & oTemprec.Fields.Item("Name").Value & "' and ""Code""='" & oTemprec.Fields.Item("Code").Value & "'")
                oTemprec.MoveNext()
            Next
            oTemprec.DoQuery("Delete from  ""@Z_HR_PERAPP""  where ""Name"" Like '%D'")
        End If

    End Sub
#End Region
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_AppPeriod Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" Then
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
                                If pVal.ItemUID = "3" Then
                                    AddtoUDT1(oForm)
                                End If
                                If pVal.ItemUID = "4" Then
                                    oGrid = oForm.Items.Item("1").Specific
                                    AddEmptyRow(oGrid)
                                End If
                                If pVal.ItemUID = "5" Then
                                    oGrid = oForm.Items.Item("1").Specific
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
                Case mnu_AppPeriod
                    LoadForm()
                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("1").Specific
                    If pVal.BeforeAction = False Then
                        AddEmptyRow(oGrid)
                    End If

                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("1").Specific
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
