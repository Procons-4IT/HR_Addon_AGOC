Imports System.IO
Public Class clshrTraExpOverView
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
    Public Sub LoadForm(ByVal oForm As SAPbouiCOM.Form)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_TraExpOverView) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_TraExpOverView, frm_hr_TraExpOverView)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.Title = "Travel OverView"

        AddChooseFromList(oForm)

        oForm.DataSources.UserDataSources.Add("ExpName", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "24", "ExpName")
        oEditText = oForm.Items.Item("24").Specific
        oEditText.ChooseFromListUID = "CFL1"
        oEditText.ChooseFromListAlias = "U_Z_ExpName"
        oForm.DataSources.UserDataSources.Add("ExpName1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "26", "ExpName1")
        oEditText = oForm.Items.Item("26").Specific
        oEditText.ChooseFromListUID = "CFL5"
        oEditText.ChooseFromListAlias = "U_Z_ExpName"

        oForm.DataSources.UserDataSources.Add("TraCode", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "20", "TraCode")
        oEditText = oForm.Items.Item("20").Specific
        oEditText.ChooseFromListUID = "CFL2"
        oEditText.ChooseFromListAlias = "U_Z_TraCode"
        oForm.DataSources.UserDataSources.Add("TraCode1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "22", "TraCode1")
        oEditText = oForm.Items.Item("22").Specific
        oEditText.ChooseFromListUID = "CFL6"
        oEditText.ChooseFromListAlias = "U_Z_TraCode"

        oForm.DataSources.UserDataSources.Add("empId", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "16", "empId")
        oEditText = oForm.Items.Item("16").Specific
        oEditText.ChooseFromListUID = "CFL3"
        oEditText.ChooseFromListAlias = "empID"
        oForm.DataSources.UserDataSources.Add("empId1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "18", "empId1")
        oEditText = oForm.Items.Item("18").Specific
        oEditText.ChooseFromListUID = "CFL4"
        oEditText.ChooseFromListAlias = "empID"

        oForm.PaneLevel = 1
        Dim osta As SAPbouiCOM.StaticText
        osta = oForm.Items.Item("19").Specific
        osta.Caption = "Step " & oForm.PaneLevel & " of 3"
        oForm.Items.Item("19").TextStyle = SAPbouiCOM.BoTextStyle.ts_BOLD
        ' EnableDisable(oForm, oForm.Title)
        oForm.Items.Item("1000006").Visible = False
        oForm.Items.Item("1").Visible = True
        oForm.Freeze(False)
    End Sub
    Public Sub LoadForm1(ByVal oForm As SAPbouiCOM.Form)
        oForm = oApplication.Utilities.LoadForm(xml_hr_TraExpOverView, frm_hr_TraExpOverView)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.Title = "Expenses OverView"

        AddChooseFromList(oForm)
        oForm.DataSources.UserDataSources.Add("ExpName", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "24", "ExpName")
        oEditText = oForm.Items.Item("24").Specific
        oEditText.ChooseFromListUID = "CFL1"
        oEditText.ChooseFromListAlias = "U_Z_ExpName"
        oForm.DataSources.UserDataSources.Add("ExpName1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "26", "ExpName1")
        oEditText = oForm.Items.Item("26").Specific
        oEditText.ChooseFromListUID = "CFL5"
        oEditText.ChooseFromListAlias = "U_Z_ExpName"

        oForm.DataSources.UserDataSources.Add("TraCode", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "20", "TraCode")
        oEditText = oForm.Items.Item("20").Specific
        oEditText.ChooseFromListUID = "CFL2"
        oEditText.ChooseFromListAlias = "U_Z_TraCode"
        oForm.DataSources.UserDataSources.Add("TraCode1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "22", "TraCode1")
        oEditText = oForm.Items.Item("22").Specific
        oEditText.ChooseFromListUID = "CFL6"
        oEditText.ChooseFromListAlias = "U_Z_TraCode"

        oForm.DataSources.UserDataSources.Add("empId", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "16", "empId")
        oEditText = oForm.Items.Item("16").Specific
        oEditText.ChooseFromListUID = "CFL3"
        oEditText.ChooseFromListAlias = "empID"
        oForm.DataSources.UserDataSources.Add("empId1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "18", "empId1")
        oEditText = oForm.Items.Item("18").Specific
        oEditText.ChooseFromListUID = "CFL4"
        oEditText.ChooseFromListAlias = "empID"

        oForm.PaneLevel = 1
        Dim osta As SAPbouiCOM.StaticText
        osta = oForm.Items.Item("19").Specific
        osta.Caption = "Step " & oForm.PaneLevel & " of 3"
        oForm.Items.Item("19").TextStyle = SAPbouiCOM.BoTextStyle.ts_BOLD
        ' EnableDisable(oForm, oForm.Title)
        oForm.Items.Item("1000006").Visible = True
        oForm.Items.Item("1").Visible = False
        oForm.Freeze(False)
    End Sub
    Private Sub EnableDisable(ByVal aForm As SAPbouiCOM.Form, ByVal strTitile As String)
        If aForm.Title = "Travel OverView" Then
            aForm.Items.Item("1000006").Visible = False
            If aForm.PaneLevel = 1 Then
                aForm.Items.Item("1").Visible = True
                aForm.Items.Item("15").Visible = False
                aForm.Items.Item("16").Visible = False
                aForm.Items.Item("17").Visible = False
                aForm.Items.Item("18").Visible = False
            ElseIf oForm.PaneLevel = 3 Then
                aForm.Items.Item("1").Visible = False
                aForm.Items.Item("15").Visible = False
                aForm.Items.Item("16").Visible = False
                aForm.Items.Item("17").Visible = False
                aForm.Items.Item("18").Visible = False
            Else
                aForm.Items.Item("1").Visible = False
                aForm.Items.Item("15").Visible = True
                aForm.Items.Item("16").Visible = True
                aForm.Items.Item("17").Visible = True
                aForm.Items.Item("18").Visible = True
            End If

            aForm.Items.Item("23").Visible = False
            aForm.Items.Item("24").Visible = False
            aForm.Items.Item("25").Visible = False
            aForm.Items.Item("26").Visible = False
        Else
            aForm.Items.Item("15").Visible = False
            aForm.Items.Item("16").Visible = False
            aForm.Items.Item("17").Visible = False
            aForm.Items.Item("18").Visible = False
            aForm.Items.Item("1").Visible = False
            If aForm.PaneLevel = 1 Then
                aForm.Items.Item("1000006").Visible = True
                aForm.Items.Item("23").Visible = False
                aForm.Items.Item("24").Visible = False
                aForm.Items.Item("25").Visible = False
                aForm.Items.Item("26").Visible = False
            ElseIf oForm.PaneLevel = 3 Then
                aForm.Items.Item("1000006").Visible = False
                aForm.Items.Item("23").Visible = False
                aForm.Items.Item("24").Visible = False
                aForm.Items.Item("25").Visible = False
                aForm.Items.Item("26").Visible = False
            Else
                aForm.Items.Item("1000006").Visible = False
                aForm.Items.Item("23").Visible = True
                aForm.Items.Item("24").Visible = True
                aForm.Items.Item("25").Visible = True
                aForm.Items.Item("26").Visible = True
            End If

        End If
    End Sub
    Private Sub Databind(ByVal aform As SAPbouiCOM.Form, ByVal strTitle As String)
        Dim strFromEMP, strToEMP, strFromTraCode, strToTraCode, strFromExp, strToExp, strqry As String
        Dim strEMPCondition As String = ""
        Dim strTraCodeCondition As String = ""
        Dim strExpenCondition As String = ""
        strFromEMP = oApplication.Utilities.getEdittextvalue(aform, "16")
        strToEMP = oApplication.Utilities.getEdittextvalue(aform, "18")
        strFromTraCode = oApplication.Utilities.getEdittextvalue(aform, "20")
        strToTraCode = oApplication.Utilities.getEdittextvalue(aform, "22")
        strFromExp = oApplication.Utilities.getEdittextvalue(aform, "24")
        strToExp = oApplication.Utilities.getEdittextvalue(aform, "26")
        If strFromEMP <> "" And strToEMP <> "" Then
            strEMPCondition = " Convert(Decimal,U_Z_EmpId) between " & CDbl(strFromEMP) & " and " & CDbl(strToEMP)
        ElseIf strFromEMP <> "" And strToEMP = "" Then
            strEMPCondition = " Convert(Decimal,U_Z_EmpId) >= " & CDbl(strFromEMP)
        ElseIf strFromEMP = "" And strToEMP <> "" Then
            strEMPCondition = " Convert(Decimal,U_Z_EmpId) <= " & CDbl(strToEMP)
        Else
            strEMPCondition = " 1=1"
        End If

        If strFromTraCode <> "" And strToTraCode <> "" Then
            strTraCodeCondition = " U_Z_TraCode between '" & strFromTraCode & "' and '" & strToTraCode & "'"
        ElseIf strFromTraCode <> "" And strToTraCode = "" Then
            strTraCodeCondition = " U_Z_TraCode >= '" & strFromTraCode & "'"
        ElseIf strFromTraCode = "" And strToTraCode <> "" Then
            strTraCodeCondition = " U_Z_TraCode <= '" & strToTraCode & "'"
        Else
            strTraCodeCondition = " 1=1"
        End If

        If strFromExp <> "" And strToExp <> "" Then
            strExpenCondition = " U_Z_ExpType IN ( '" & strFromExp & "' " & "," & " '" & strToExp & "')"
        ElseIf strFromExp <> "" And strToExp = "" Then
            strExpenCondition = " U_Z_ExpType = '" & strFromExp & "'"
        ElseIf strFromExp = "" And strToExp <> "" Then
            strExpenCondition = " U_Z_ExpType = '" & strToExp & "'"
        Else
            strExpenCondition = " 1=1"
        End If
        If strTitle = "Travel OverView" Then
            Dim strcondition As String
            strcondition = strEMPCondition & " and " & strTraCodeCondition & " Order by DocEntry, U_Z_EmpId,U_Z_TraCode"
            strqry = "Select DocEntry,U_Z_DocDate, U_Z_EmpId,U_Z_EmpName,U_Z_TraCode,U_Z_TraName,U_Z_ReqAppDate,U_Z_ReqClaimDate,"
            strqry = strqry & " U_Z_AppClaimDate,case U_Z_Status when 'O' then 'Open' when 'RA' then 'Request Approved' when 'RR' then 'Request Rejected' when 'CR' then 'Claim Received'"
            strqry = strqry & "  when 'CA' then 'Claim Approved' when 'CJ' then 'Claim Rejected' else 'Closed' end as U_Z_Status from [@Z_HR_OTRAREQ] where (U_Z_Status<>'CR' and U_Z_Status<>'CA' and U_Z_Status<>'CJ') and " & strcondition
        Else
            Dim strcondition As String
            strcondition = strExpenCondition & " and " & strTraCodeCondition & " Order by U_Z_EmpId,U_Z_TraCode,U_Z_ExpType"
            'strqry = "select T1.DocEntry,U_Z_ExpName,U_Z_EmpId,U_Z_EmpName,U_Z_TraCode,U_Z_TraName ,U_Z_Amount,U_Z_UtilAmt,U_Z_BalAmount,U_Z_ReqClaimAmt,U_Z_ApprClaimAmt,case T0.U_Z_Status when 'A' then 'Applicable' "
            'strqry = strqry & " when 'NA' then 'Not Applicable' when 'AP' then 'Approved' else 'Paid' end as U_Z_Status,case T1.U_Z_Status when 'O' then 'Open' when 'CR' then 'Claim Received'"
            'strqry = strqry & "  when 'CA' then 'Claim Approved' when 'CJ' then 'Claim Rejected' else 'Closed' end as Status  from [@Z_HR_TRAREQ1] T0 inner join [@Z_HR_OTRAREQ] T1 on T0.DocEntry=T1.DocEntry   where (T1.U_Z_Status='CR' or T1.U_Z_Status='CA' or T1.U_Z_Status='CJ')  and " & strcondition

            strqry = "select  T0.""Code"",""U_Z_EmpID"",""U_Z_EmpName"",convert(varchar(10),""U_Z_Subdt"",103) as ""U_Z_Subdt"",Convert(varchar(10),""U_Z_Claimdt"",103) as ""U_Z_Claimdt"",""U_Z_Client"",""U_Z_Project"",""U_Z_TraCode"",""U_Z_TraDesc"",""U_Z_ExpType"",""U_Z_City"",""U_Z_Currency"",""U_Z_CurAmt"",""U_Z_ExcRate"","
            strqry += """U_Z_UsdAmt"",case ""U_Z_Reimburse"" when 'Y' then 'Yes' else 'No' end as ""U_Z_Reimburse"",""U_Z_ReimAmt"",T2.""U_Z_PayMethod"",""U_Z_Notes"","
            strqry += """U_Z_Attachment"",case ""U_Z_AppStatus"" when 'P' then 'Pending' when 'A' then 'Approved' when 'R' then 'Rejected' end as ""U_Z_AppStatus"" from ""@Z_HR_EXPCL"" T0"
            strqry += " left join ""@Z_HR_PAYMD"" T2 on T0.""U_Z_PayMethod""=T2.""Code"" where " & strcondition
        End If
        oGrid = aform.Items.Item("10").Specific
        oGrid.DataTable.ExecuteQuery(strqry)
        FormatGrid(aform, strTitle, strqry)
        oApplication.Utilities.assignMatrixLineno(oGrid, aform)
    End Sub
    Private Sub FormatGrid(ByVal aForm As SAPbouiCOM.Form, ByVal aChoice As String, ByVal strQuery As String)
        oGrid = aForm.Items.Item("10").Specific
        Dim oGECol As SAPbouiCOM.EditTextColumn
        If aChoice = "Travel OverView" Then
            oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Request Number"
            oEditTextColumn = oGrid.Columns.Item("DocEntry")
            oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
            oGrid.Columns.Item("U_Z_DocDate").TitleObject.Caption = "Request Date"
            oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee Id"
            oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
            oEditTextColumn.LinkedObjectType = "171"
            oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
            oGrid.Columns.Item("U_Z_TraCode").TitleObject.Caption = "Travel Code"
            oEditTextColumn = oGrid.Columns.Item("U_Z_TraCode")
            oEditTextColumn.LinkedObjectType = "Z_HR_OTRAPLA"
            oGrid.Columns.Item("U_Z_TraName").TitleObject.Caption = "Travel Description"
            oGrid.Columns.Item("U_Z_ReqAppDate").TitleObject.Caption = "Request Approved Date"
            oGrid.Columns.Item("U_Z_ReqClaimDate").TitleObject.Caption = "Request Claim Date"
            oGrid.Columns.Item("U_Z_AppClaimDate").TitleObject.Caption = "Claim Approved Date"
            oGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Status"
        Else
            oGrid.Columns.Item("Code").TitleObject.Caption = "Request Number"
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("U_Z_Subdt").TitleObject.Caption = "Submitted Date"
            oGrid.Columns.Item("U_Z_Subdt").Visible = False
            oGrid.Columns.Item("U_Z_EmpID").TitleObject.Caption = "Employee Id"
            oEditTextColumn = oGrid.Columns.Item("U_Z_EmpID")
            oEditTextColumn.LinkedObjectType = "171"
            oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
            oGrid.Columns.Item("U_Z_Claimdt").TitleObject.Caption = "Transaction Date"
            oGrid.Columns.Item("U_Z_Client").TitleObject.Caption = "Client"
            oGrid.Columns.Item("U_Z_Project").TitleObject.Caption = "Project"
            oGrid.Columns.Item("U_Z_TraCode").TitleObject.Caption = "Travel Code"
            oEditTextColumn = oGrid.Columns.Item("U_Z_TraCode")
            oEditTextColumn.LinkedObjectType = "Z_HR_OTRAPLA"
            oGrid.Columns.Item("U_Z_TraDesc").TitleObject.Caption = "Travel Description"
            oGrid.Columns.Item("U_Z_City").TitleObject.Caption = "City"
            oGrid.Columns.Item("U_Z_Currency").TitleObject.Caption = "Transaction Currency"
            oGrid.Columns.Item("U_Z_CurAmt").TitleObject.Caption = "Transaction Amount"
            oGrid.Columns.Item("U_Z_ExcRate").TitleObject.Caption = "Exchange Rate"
            oGrid.Columns.Item("U_Z_UsdAmt").TitleObject.Caption = "Local Currency Amount"
            oGrid.Columns.Item("U_Z_Reimburse").TitleObject.Caption = "To be Reimbursed?"
            oGrid.Columns.Item("U_Z_ReimAmt").TitleObject.Caption = "Reimbursement Amount"
            oGrid.Columns.Item("U_Z_ExpType").TitleObject.Caption = "Expense Type"
            oEditTextColumn = oGrid.Columns.Item("U_Z_ExpType")
            oEditTextColumn.LinkedObjectType = "Z_HR_EXPANCES"
            oGrid.Columns.Item("U_Z_PayMethod").TitleObject.Caption = "Payment Method"
            oGrid.Columns.Item("U_Z_Notes").TitleObject.Caption = "Notes"
            oGrid.Columns.Item("U_Z_Attachment").TitleObject.Caption = "Attachments"
            oGECol = oGrid.Columns.Item("U_Z_Attachment")
            oGECol.LinkedObjectType = "Z_HR_OEXFOM"
            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Status"
        End If
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub

    Private Sub AddChooseFromList(ByVal objForm As SAPbouiCOM.Form)
        Try
            Dim oCFLs As SAPbouiCOM.ChooseFromListCollection
            Dim oCons As SAPbouiCOM.Conditions
            Dim oCon As SAPbouiCOM.Condition
            ' oCombobox = objForm.Items.Item("7").Specific
            oCFLs = objForm.ChooseFromLists
            Dim oCFL As SAPbouiCOM.ChooseFromList
            Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)

            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_EXPANCES"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_EXPANCES"
            oCFLCreationParams.UniqueID = "CFL5"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_OTRAPLA"
            oCFLCreationParams.UniqueID = "CFL2"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_OTRAPLA"
            oCFLCreationParams.UniqueID = "CFL6"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "171"
            oCFLCreationParams.UniqueID = "CFL3"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "171"
            oCFLCreationParams.UniqueID = "CFL4"
            oCFL = oCFLs.Add(oCFLCreationParams)



        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub LoadFiles(ByVal aform As SAPbouiCOM.Form)
        oGrid = aform.Items.Item("10").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.Rows.IsSelected(intRow) Then
                Dim strFilename, strFilePath As String
                strFilename = oGrid.DataTable.GetValue("U_Z_Attachment", intRow)
                Dim Filename As String = Path.GetFileName(strFilename)
                strFilePath = oGrid.DataTable.GetValue("U_Z_Attachment", intRow)

                If File.Exists(strFilePath) = False Then
                    Dim oRec As SAPbobsCOM.Recordset
                    oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    Dim strQry = "Select ""AttachPath"" From OADP"
                    oRec.DoQuery(strQry)
                    strFilePath = oRec.Fields.Item(0).Value

                    If Filename = "" Then
                        strFilePath = strFilePath
                    Else
                        strFilePath = strFilePath & Filename
                    End If
                    If File.Exists(strFilePath) = False Then
                        oApplication.Utilities.Message("File does not exists ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Exit Sub
                    End If
                    strFilename = strFilePath
                Else
                    strFilename = strFilePath
                End If

                Dim x As System.Diagnostics.ProcessStartInfo
                x = New System.Diagnostics.ProcessStartInfo
                x.UseShellExecute = True
                x.FileName = strFilename
                System.Diagnostics.Process.Start(x)
                x = Nothing
                Exit Sub
            End If
        Next
        oApplication.Utilities.Message("No file has been selected...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_TraExpOverView Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim strcode, strstatus, strempid As String
                                If pVal.ItemUID = "10" And pVal.ColUID = "U_Z_TraCode" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    strcode = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    oApplication.Utilities.OpenMasterinLink(oForm, "TravelAgenda", strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "10" And pVal.ColUID = "U_Z_ExpType" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    For intRow As Integer = pVal.Row To pVal.Row
                                        If 1 = 1 Then
                                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                            Dim objct As New clshrExpenses
                                            objct.LoadForm()
                                        End If
                                    Next
                                    BubbleEvent = False
                                    Exit Sub
                                End If

                                If pVal.ItemUID = "10" And pVal.ColUID = "DocEntry" Then
                                    oGrid = oForm.Items.Item("10").Specific
                                    For intRow As Integer = pVal.Row To pVal.Row
                                        If 1 = 1 Then
                                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                            strcode = oGrid.DataTable.GetValue("DocEntry", intRow)
                                            strstatus = oGrid.DataTable.GetValue("U_Z_Status", intRow)
                                            strempid = oGrid.DataTable.GetValue("U_Z_EmpId", intRow)
                                            Dim objct As New clshrTravelRequest
                                            objct.LoadForm1(oForm, strcode, oForm.Title, strstatus, strempid)
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    Next
                                    BubbleEvent = False
                                    Exit Sub
                                End If


                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim strTraCode As String
                                Select Case pVal.ItemUID
                                    Case "1000002"
                                        strTraCode = oApplication.Utilities.getEdittextvalue(oForm, "20")
                                        oApplication.Utilities.OpenMasterinLink(oForm, "TravelAgenda", strTraCode)
                                        BubbleEvent = False
                                        Exit Sub
                                    Case "1000005"
                                        strTraCode = oApplication.Utilities.getEdittextvalue(oForm, "22")
                                        oApplication.Utilities.OpenMasterinLink(oForm, "TravelAgenda", strTraCode)
                                        BubbleEvent = False
                                        Exit Sub
                                    Case "1000003"
                                        oApplication.Utilities.OpenMasterinLink(oForm, "Expenses")
                                        BubbleEvent = False
                                        Exit Sub
                                    Case "1000007"
                                        oApplication.Utilities.OpenMasterinLink(oForm, "Expenses")
                                        BubbleEvent = False
                                        Exit Sub
                                End Select
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "10" And pVal.ColUID = "U_Z_Attachment" Then
                                    oGrid = oForm.Items.Item("10").Specific
                                    oGrid.Columns.Item("RowsHeader").Click(pVal.Row)
                                    LoadFiles(oForm)
                                    BubbleEvent = False
                                    Exit Sub
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "3"
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = oForm.PaneLevel + 1
                                        Dim osta As SAPbouiCOM.StaticText
                                        osta = oForm.Items.Item("19").Specific
                                        osta.Caption = "Step " & oForm.PaneLevel & " of 3"
                                        EnableDisable(oForm, oForm.Title)
                                        If oForm.PaneLevel = 3 Then
                                            Databind(oForm, oForm.Title)
                                        End If
                                        oForm.Freeze(False)
                                    Case "4"
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = oForm.PaneLevel - 1
                                        Dim osta As SAPbouiCOM.StaticText
                                        osta = oForm.Items.Item("19").Specific
                                        osta.Caption = "Step " & oForm.PaneLevel & " of 3"
                                        EnableDisable(oForm, oForm.Title)
                                        oForm.Freeze(False)
                                End Select
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1 As String
                                Dim sCHFL_ID, val, val2, val3 As String
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

                                        If pVal.ItemUID = "16" Then
                                            val1 = oDataTable.GetValue("empID", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "16", val1)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "18" Then
                                            val1 = oDataTable.GetValue("empID", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "18", val1)
                                            Catch ex As Exception
                                            End Try
                                        End If

                                        If pVal.ItemUID = "20" Then
                                            val1 = oDataTable.GetValue("U_Z_TraCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "20", val1)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "22" Then
                                            val1 = oDataTable.GetValue("U_Z_TraCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "22", val1)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "24" Then
                                            val1 = oDataTable.GetValue("U_Z_ExpName", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "24", val1)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "26" Then
                                            val1 = oDataTable.GetValue("U_Z_ExpName", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "26", val1)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        oForm.Freeze(False)
                                    End If
                                Catch ex As Exception
                                    ' oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    oForm.Freeze(False)
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
                Case mnu_InvSO
                Case mnu_hr_TraOverview
                    LoadForm(oForm)
                Case mnu_hr_ExpOverview
                    LoadForm1(oForm)
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
