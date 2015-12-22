Public Class clshrOfferAllowance
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oColumn As SAPbouiCOM.ComboBoxColumn
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
    Dim oGenService As SAPbobsCOM.GeneralService
    Dim oGenData As SAPbobsCOM.GeneralData
    Dim oGenDataCollection As SAPbobsCOM.GeneralDataCollection
    Dim oCompService As SAPbobsCOM.CompanyService
    Dim oChildData As SAPbobsCOM.GeneralData
    Dim oGeneralDataParams As SAPbobsCOM.GeneralDataParams
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Public Sub LoadForm(ByVal Appid As String, ByVal AppName As String, ByVal DocEntry As String, Optional ByVal Refcode As String = "", Optional ByVal Basic As String = "", Optional ByVal LineNo As String = "")
        Try
            oForm = oApplication.Utilities.LoadForm(xml_EmpAllOffer, frm_hr_EmpAllOffer)
            oForm = oApplication.SBO_Application.Forms.ActiveForm()
            oForm.Freeze(True)
            oForm.EnableMenu(mnu_ADD_ROW, True)
            oForm.EnableMenu(mnu_DELETE_ROW, True)
            oForm.DataSources.UserDataSources.Add("Appid", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oApplication.Utilities.setUserDatabind(oForm, "4", "Appid")
            oForm.DataSources.UserDataSources.Add("Appname", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oApplication.Utilities.setUserDatabind(oForm, "6", "Appname")
            oForm.DataSources.UserDataSources.Add("Basic", SAPbouiCOM.BoDataType.dt_SUM)
            oApplication.Utilities.setUserDatabind(oForm, "8", "Basic")
            oForm.DataSources.UserDataSources.Add("Refno", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oApplication.Utilities.setUserDatabind(oForm, "12", "Refno")
            '  oForm.DataSources.UserDataSources.Add("DocNo", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)

            oForm.DataSources.UserDataSources.Add("DocNo", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            oApplication.Utilities.setUserDatabind(oForm, "13", "DocNo")
            oApplication.Utilities.setEdittextvalue(oForm, "4", Appid)
            oApplication.Utilities.setEdittextvalue(oForm, "6", AppName)
            oApplication.Utilities.setEdittextvalue(oForm, "8", Basic)
            oApplication.Utilities.setEdittextvalue(oForm, "12", Refcode)
            oApplication.Utilities.setEdittextvalue(oForm, "13", DocEntry)
            'If LineNo = 0 Then
            '    LineNumber = LineNo
            'Else
            '    LineNumber = LineNo
            'End If
            oForm.Items.Item("10").TextStyle = SAPbouiCOM.BoFontStyle.fs_Italic
            ' AddtoUDT(oForm, Refcode, DocEntry, LineNumber)
            Databind(oForm, Refcode)
            oForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
    Private Sub Databind(ByVal aForm As SAPbouiCOM.Form, ByVal LineRefNo As String)
        Try
            oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oGrid = aForm.Items.Item("11").Specific
            strQuery = "Select * from [@Z_HR_EALL1] where U_Z_RefNo='" & LineRefNo & "'"
            oGrid.DataTable.ExecuteQuery(strQuery)
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("Name").Visible = False
            oGrid.Columns.Item("U_Z_LineNo").Visible = False
            oGrid.Columns.Item("U_Z_AllCode").TitleObject.Caption = "Allowance Code"
            oGrid.Columns.Item("U_Z_AllCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oColumn = oGrid.Columns.Item("U_Z_AllCode")
            Dim oRec As SAPbobsCOM.Recordset
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRec.DoQuery("Select ""U_Z_CODE"" As ""Code"",""U_Z_NAME"" As ""Name"" From ""@Z_PAY_OEAR""")
            oColumn.ValidValues.Add("-", "-")
            For i As Integer = 0 To oRec.RecordCount - 1
                oColumn.ValidValues.Add(oRec.Fields.Item("Code").Value.ToString(), oRec.Fields.Item("Name").Value.ToString())
                oRec.MoveNext()
            Next
            oColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
            oGrid.Columns.Item("U_Z_AllName").TitleObject.Caption = "Allowance Name"
            oGrid.Columns.Item("U_Z_AllName").Visible = False
            oGrid.Columns.Item("U_Z_Amount").TitleObject.Caption = "Amount"
            oGrid.Columns.Item("U_Z_RefNo").Visible = False
            oGrid.AutoResizeColumns()
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
   
    Private Function ADDUDTLines(ByVal aForm As SAPbouiCOM.Form, ByVal DocEntry As String) As Boolean
        Dim dblAmount As Double = 0
        Dim dblAmount1, dblTotal As Double
        Dim strCode As String = String.Empty
        Dim RefNo1 As String = String.Empty
        Dim strLineNum As Integer
        Dim RefNo As String
        Dim oUserTable, ouserTableLines As SAPbobsCOM.UserTable
        oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oUserTable = oApplication.Company.UserTables.Item("Z_HR_EALL1")
        Try
            DocEntry = oApplication.Utilities.getEdittextvalue(aForm, "12")
            strQuery = "select U_Z_RefNo,Code,U_Z_LineNo from [@Z_HR_OEALL] where Code='" & DocEntry & "'" ' and U_Z_LineNo='" & LineNumber & "'"
            oRecSet.DoQuery(strQuery)
            If oRecSet.RecordCount > 0 Then
                RefNo = oRecSet.Fields.Item("Code").Value
                strLineNum = oRecSet.Fields.Item("U_Z_LineNo").Value
            End If
            oGrid = aForm.Items.Item("11").Specific
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                oColumn = oGrid.Columns.Item("U_Z_AllCode")
                RefNo1 = oGrid.DataTable.GetValue("Code", intRow)
                If RefNo1 = "" Then
                    strCode = oApplication.Utilities.getMaxCode("@Z_HR_EALL1", "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_AllCode").Value = oColumn.GetSelectedValue(intRow).Value
                    oUserTable.UserFields.Fields.Item("U_Z_AllName").Value = oColumn.GetSelectedValue(intRow).Description
                    oUserTable.UserFields.Fields.Item("U_Z_Amount").Value = oGrid.DataTable.GetValue("U_Z_Amount", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_RefNo").Value = RefNo
                    ' oUserTable.UserFields.Fields.Item("U_Z_LineNo").Value = LineNumber
                    If oUserTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                ElseIf oUserTable.GetByKey(RefNo1) Then
                    oUserTable.Code = RefNo1
                    oUserTable.Name = RefNo1
                    oUserTable.UserFields.Fields.Item("U_Z_AllCode").Value = oColumn.GetSelectedValue(intRow).Value
                    oUserTable.UserFields.Fields.Item("U_Z_AllName").Value = oColumn.GetSelectedValue(intRow).Description
                    oUserTable.UserFields.Fields.Item("U_Z_Amount").Value = oGrid.DataTable.GetValue("U_Z_Amount", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_RefNo").Value = RefNo
                    ' oUserTable.UserFields.Fields.Item("U_Z_LineNo").Value = LineNumber
                    If oUserTable.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
                If dblAmount = 0 Then
                    dblAmount = oGrid.DataTable.GetValue("U_Z_Amount", intRow)
                Else
                    dblAmount = dblAmount + oGrid.DataTable.GetValue("U_Z_Amount", intRow)
                End If
            Next
            dblTotal = CDbl(oApplication.Utilities.getEdittextvalue(aForm, "8")) + dblAmount

            oGrid = frmOfferAcceptance.Items.Item("6").Specific
            strQuery = "Update  [@Z_HR_OEALL] set U_Z_Basic='" & CDbl(oApplication.Utilities.getEdittextvalue(aForm, "8")) & "'  where U_Z_RefNo='" & DocEntry & "'" ' and U_Z_LineNo='" & LineNumber & "'"
            oRecSet.DoQuery(strQuery)

            oGrid.DataTable.SetValue("U_Z_Basic", strLineNum, CDbl(oApplication.Utilities.getEdittextvalue(aForm, "8")))
            oGrid.DataTable.SetValue("U_Z_TotalSalary", strLineNum, dblTotal)
            oGrid.DataTable.SetValue("U_Z_Benifit", strLineNum, dblAmount)

            'strQuery = "Update [@Z_HR_OHEM3] set U_Z_Basic='" & CDbl(oApplication.Utilities.getEdittextvalue(aForm, "8")) & "',U_Z_Benifit='" & dblAmount & "',U_Z_TotalSalary='" & dblTotal & "' where DocEntry='" & DocEntry & "' and LineID='" & LineNumber & "'"
            'oRecSet.DoQuery(strQuery)

            'strQuery = "Update [@Z_HR_OEALL] set U_Z_Basic='" & CDbl(oApplication.Utilities.getEdittextvalue(aForm, "8")) & "',U_Z_LineNo='" & LineNumber & "' where Code='" & RefNo & "' "
            'oRecSet.DoQuery(strQuery)

            ' Databind(aForm, oApplication.Utilities.getEdittextvalue(aForm, "12"))
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_EmpAllOffer Then
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
                                    If ADDUDTLines(oForm, oApplication.Utilities.getEdittextvalue(oForm, "13")) = True Then
                                        oApplication.Utilities.Message("Operation completed successfully...", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                        'Dim orec As SAPbobsCOM.Recordset
                                        'orec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                        'orec.DoQuery("Select U_Z_ReqNo from ""@Z_HR_OHEM1"" where DocEntry='" & oApplication.Utilities.getEdittextvalue(oForm, "13") & "'")
                                        'Dim oObj As New ClshrIPOfferAcceptance()
                                        'oObj.LoadForm(0, orec.Fields.Item(0).Value)
                                        oForm.Close()
                                    End If
                                End If

                        End Select
                End Select
            End If


        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
#End Region
#Region "Validation"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            If CInt(oApplication.Utilities.getEdittextvalue(aForm, "8")) = 0 Then
                oApplication.Utilities.Message("Basic Salary is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            oGrid = aForm.Items.Item("11").Specific
            For index As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                Dim strCode As String = oGrid.DataTable.GetValue("U_Z_AllCode", index) 'oApplication.Utilities.getMatrixValues(oMatrix, "V_0", index)
                For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                    If index <> intRow Then
                        Dim strCode1 As String = oGrid.DataTable.GetValue("U_Z_AllCode", intRow)
                        If strCode = strCode1 Then
                            oApplication.Utilities.Message("Allowance Code Already Exist...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Return False
                        End If
                    End If
                Next
            Next
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
#End Region
#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid)
        If aGrid.DataTable.GetValue("U_Z_AllCode", aGrid.DataTable.Rows.Count - 1) <> "" And aGrid.DataTable.GetValue("U_Z_Amount", aGrid.DataTable.Rows.Count - 1) <> 0 Then
            aGrid.DataTable.Rows.Add()
            aGrid.Columns.Item(2).Click(aGrid.DataTable.Rows.Count - 1, False)
        Else
            oApplication.Utilities.Message("Amount is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
        End If
    End Sub
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
                otemprec.DoQuery("update [@Z_HR_EALL1] set  NAME =NAME +'D'  where U_Z_AllCode='" & strCode & "'")
                agrid.DataTable.Rows.Remove(intRow)
                Exit Sub
            End If
        Next
        oApplication.Utilities.Message("No row selected", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
    End Sub
#End Region
#Region "Menu Event"
    Public Overrides Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            Select Case pVal.MenuUID
                Case mnu_InvSO
                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("11").Specific
                    If pVal.BeforeAction = False Then
                        AddEmptyRow(oGrid)
                    End If

                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oGrid = oForm.Items.Item("11").Specific
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
