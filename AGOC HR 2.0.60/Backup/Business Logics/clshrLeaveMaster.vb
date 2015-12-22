Public Class clshrLeaveMaster
    Inherits clsBase
    Private WithEvents SBO_Application As SAPbouiCOM.Application
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBoxColumn
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
        oForm = oApplication.Utilities.LoadForm(xml_hr_LeaveMaster, frm_hr_LeaveMaster)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        Databind(oForm)
        AddChooseFromList(oForm)
    End Sub
#Region "Databind"
    Private Sub Databind(ByVal aform As SAPbouiCOM.Form)
        Try
            aform.Freeze(True)
            oGrid = aform.Items.Item("5").Specific
            dtTemp = oGrid.DataTable
            dtTemp.ExecuteQuery("SELECT T0.[Code], T0.[Name], T0.[U_Z_FrgnName],T0.[U_Z_DedRate], T0.[U_Z_PaidLeave], T0.[U_Z_DaysYear], T0.[U_Z_NoofDays], T0.[U_Z_Accured], T0.[U_Z_Cutoff], T0.[U_Z_EOS], T0.[U_Z_EntAft], T0.[U_Z_TimesTaken], T0.[U_Z_MaxDays], T0.[U_Z_DailyRate], T0.[U_Z_LifeTime],T0.[U_Z_StopProces], T0.[U_Z_GLACC], T0.[U_Z_GLACC1], T0.[U_Z_OffCycle], T0.[U_Z_OB], T0.[U_Z_SickLeave] FROM [dbo].[@Z_PAY_LEAVE]  T0 order by Code")
            oGrid.DataTable = dtTemp
            Formatgrid(oGrid)
            oApplication.Utilities.assignMatrixLineno(oGrid, aform)
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
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


            oCFL = oCFLs.Item("CFL2")
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
#End Region

#Region "FormatGrid"
    Private Sub Formatgrid(ByVal agrid As SAPbouiCOM.Grid)

        'SELECT T0.[Code], T0.[Name], T0.[U_Z_DedRate], T0.[U_Z_PaidLeave], T0.[U_Z_DaysYear], T0.[U_Z_NoofDays], T0.[U_Z_Accured], T0.[U_Z_Cutoff],
        ' T0.[U_Z_EOS], T0.[U_Z_EntAft], T0.[U_Z_TimesTaken], T0.[U_Z_MaxDays], T0.[U_Z_DailyRate], T0.[U_Z_LifeTime], T0.[U_Z_GLACC], T0.[U_Z_GLACC1], T0.[U_Z_OffCycle], T0.[U_Z_OB], T0.[U_Z_SickLeave] FROM [dbo].[@Z_PAY_LEAVE]  T0
        agrid.Columns.Item("Code").TitleObject.Caption = "Leave Code"
        agrid.Columns.Item("Name").TitleObject.Caption = "Leave Name"
        agrid.Columns.Item("U_Z_FrgnName").TitleObject.Caption = "Second Language Name"
        agrid.Columns.Item("U_Z_DedRate").TitleObject.Caption = "Paid / Deduction Rate"
        agrid.Columns.Item("U_Z_PaidLeave").TitleObject.Caption = "Leave Category"
        agrid.Columns.Item("U_Z_PaidLeave").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oCombobox = agrid.Columns.Item("U_Z_PaidLeave")
        '  oCombobox.ValidValues.Add("", "")
        oCombobox.ValidValues.Add("P", "Paid Leave")
        oCombobox.ValidValues.Add("U", "UnPaid Leave")
        oCombobox.ValidValues.Add("A", "Annual Leave")
        oCombobox.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        agrid.Columns.Item("U_Z_DaysYear").TitleObject.Caption = "Entitled Days"
        agrid.Columns.Item("U_Z_NoofDays").TitleObject.Caption = "Accured Monthly Days"
        agrid.Columns.Item("U_Z_NoofDays").Visible = False
        agrid.Columns.Item("U_Z_Accured").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        agrid.Columns.Item("U_Z_Accured").TitleObject.Caption = "Accured"
        agrid.Columns.Item("U_Z_Cutoff").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        agrid.Columns.Item("U_Z_Cutoff").TitleObject.Caption = "Cutoff Days"
        oCombobox = agrid.Columns.Item("U_Z_Cutoff")
        oCombobox.ValidValues.Add("H", "Holidays")
        oCombobox.ValidValues.Add("W", "Week Ends")
        oCombobox.ValidValues.Add("B", "Both")
        oCombobox.ValidValues.Add("N", "None")
        oCombobox.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        ' T0.[U_Z_EOS], T0.[U_Z_EntAft], T0.[U_Z_TimesTaken], T0.[U_Z_MaxDays], T0.[U_Z_DailyRate], T0.[U_Z_LifeTime], T0.[U_Z_GLACC], T0.[U_Z_GLACC1], T0.[U_Z_OffCycle], T0.[U_Z_OB], T0.[U_Z_SickLeave] FROM [dbo].[@Z_PAY_LEAVE]  T0
        agrid.Columns.Item("U_Z_EOS").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        agrid.Columns.Item("U_Z_EOS").TitleObject.Caption = "Affect EOS"
        agrid.Columns.Item("U_Z_EntAft").TitleObject.Caption = "Entitle After"
        agrid.Columns.Item("U_Z_TimesTaken").TitleObject.Caption = "Times Taken/Year"
        agrid.Columns.Item("U_Z_MaxDays").TitleObject.Caption = "Max days taken / transaction"
        agrid.Columns.Item("U_Z_DailyRate").TitleObject.Caption = "Daily rate days"
        agrid.Columns.Item("U_Z_LifeTime").TitleObject.Caption = "Taken per lifetime"


        agrid.Columns.Item("U_Z_GLACC").TitleObject.Caption = "Debit G/L Account"
        oEditTextColumn = agrid.Columns.Item("U_Z_GLACC")
        oEditTextColumn.ChooseFromListUID = "CFL1"
        oEditTextColumn.ChooseFromListAlias = "Formatcode"
        oEditTextColumn.LinkedObjectType = "1"




        agrid.Columns.Item("U_Z_GLACC1").TitleObject.Caption = "Credit G/L Account"
        oEditTextColumn = agrid.Columns.Item("U_Z_GLACC1")
        oEditTextColumn.ChooseFromListUID = "CFL2"
        oEditTextColumn.ChooseFromListAlias = "Formatcode"
        oEditTextColumn.LinkedObjectType = "1"

        agrid.Columns.Item("U_Z_OB").TitleObject.Caption = "Default Opening Balance"
        agrid.Columns.Item("U_Z_OB").Visible = False

        agrid.Columns.Item("U_Z_OffCycle").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        agrid.Columns.Item("U_Z_OffCycle").TitleObject.Caption = "Affect Off Cycle Payroll"
        agrid.Columns.Item("U_Z_OffCycle").Visible = False

        agrid.Columns.Item("U_Z_StopProces").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        agrid.Columns.Item("U_Z_StopProces").TitleObject.Caption = "Stop Transactions"
        agrid.Columns.Item("U_Z_StopProces").Visible = True


        agrid.Columns.Item("U_Z_SickLeave").TitleObject.Caption = "Sick Leave Type"
        agrid.Columns.Item("U_Z_SickLeave").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oCombobox = agrid.Columns.Item("U_Z_SickLeave")
        oCombobox.ValidValues.Add("", "")
        oCombobox.ValidValues.Add("F", "Sick Leave Full")
        oCombobox.ValidValues.Add("T", "Sick Leave 75%")
        oCombobox.ValidValues.Add("H", "Sick Leave 50%")
        oCombobox.ValidValues.Add("Q", "Sick Leave 25%")
        oCombobox.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        agrid.Columns.Item("U_Z_SickLeave").Visible = False
        agrid.AutoResizeColumns()
        agrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
#End Region

#Region "AddRow"
    Private Sub AddEmptyRow(ByVal aGrid As SAPbouiCOM.Grid)
        If aGrid.DataTable.GetValue("Code", aGrid.DataTable.Rows.Count - 1) <> "" Then
            aGrid.DataTable.Rows.Add()
            aGrid.Columns.Item("Code").Click(aGrid.DataTable.Rows.Count - 1, False)
        End If
    End Sub
#End Region

#Region "CommitTrans"
    Private Sub Committrans(ByVal strChoice As String)
        Dim oTemprec, oItemRec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oItemRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strChoice = "Cancel" Then
            oTemprec.DoQuery("Update [@Z_PAY_LEAVE] set NAME=CODE where Name Like '%XX'")
        Else
            oTemprec.DoQuery("Delete from  [@Z_PAY_LEAVE]  where NAME Like '%XX'")
        End If
    End Sub
#End Region

#Region "AddtoUDT"
    Private Function AddtoUDT1(ByVal aform As SAPbouiCOM.Form) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode, strECode, strESocial, strEname, strETax, strGLAcc, sickLeave As String
        Dim OCHECKBOXCOLUMN As SAPbouiCOM.CheckBoxColumn
        'SELECT T0.[Code], T0.[Name], T0.[U_Z_DedRate], T0.[U_Z_PaidLeave], T0.[U_Z_DaysYear], T0.[U_Z_NoofDays], T0.[U_Z_Accured], T0.[U_Z_Cutoff],
        ' T0.[U_Z_EOS], T0.[U_Z_EntAft], T0.[U_Z_TimesTaken], T0.[U_Z_MaxDays], T0.[U_Z_DailyRate], T0.[U_Z_LifeTime], T0.[U_Z_GLACC], T0.[U_Z_GLACC1], T0.[U_Z_OffCycle], T0.[U_Z_OB], T0.[U_Z_SickLeave] FROM [dbo].[@Z_PAY_LEAVE]  T0
        Dim strLeaveCode, strLeaveName, strPaidLeave, strAccured, strCutoff, strEOS

        oGrid = aform.Items.Item("5").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            oApplication.Utilities.Message("Processing....", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            If oGrid.DataTable.GetValue(0, intRow) <> "" Or oGrid.DataTable.GetValue(1, intRow) <> "" Then
                strCode = oGrid.DataTable.GetValue("Code", intRow)
                strECode = oGrid.DataTable.GetValue("Name", intRow)
                strGLAcc = oGrid.DataTable.GetValue("U_Z_GLACC", intRow)
                oUserTable = oApplication.Company.UserTables.Item("Z_PAY_LEAVE")
                If oUserTable.GetByKey(strCode) = False Then
                    oUserTable.Code = strCode.Trim()
                    oUserTable.Name = strECode.Trim()
                    oUserTable.UserFields.Fields.Item("U_Z_FrgnName").Value = oGrid.DataTable.GetValue("U_Z_FrgnName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_DedRate").Value = (oGrid.DataTable.GetValue("U_Z_DedRate", intRow))
                    oCombobox = oGrid.Columns.Item("U_Z_PaidLeave")
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_PaidLeave").Value = oCombobox.GetSelectedValue(intRow).Value
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_PaidLeave").Value = "U"
                    End Try

                    oUserTable.UserFields.Fields.Item("U_Z_DaysYear").Value = oGrid.DataTable.GetValue("U_Z_DaysYear", intRow)

                    OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_Accured")
                    If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                        oUserTable.UserFields.Fields.Item("U_Z_Accured").Value = "Y"
                        oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = (oGrid.DataTable.GetValue("U_Z_DaysYear", intRow)) / 12
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_Accured").Value = "N"
                        oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = 0
                    End If
                    oCombobox = oGrid.Columns.Item("U_Z_Cutoff")
                    Try
                        oUserTable.UserFields.Fields.Item("U_Z_Cutoff").Value = oCombobox.GetSelectedValue(intRow).Value
                    Catch ex As Exception
                        oUserTable.UserFields.Fields.Item("U_Z_Cutoff").Value = "N"
                    End Try

                    OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_EOS")
                    If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                        oUserTable.UserFields.Fields.Item("U_Z_EOS").Value = "Y"
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_EOS").Value = "N"
                    End If

                    OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_StopProces")
                    If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                        oUserTable.UserFields.Fields.Item("U_Z_StopProces").Value = "Y"
                    Else
                        oUserTable.UserFields.Fields.Item("U_Z_StopProces").Value = "N"
                    End If

                    oUserTable.UserFields.Fields.Item("U_Z_EntAft").Value = oGrid.DataTable.GetValue("U_Z_EntAft", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_TimesTaken").Value = (oGrid.DataTable.GetValue("U_Z_TimesTaken", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_MaxDays").Value = oGrid.DataTable.GetValue("U_Z_MaxDays", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_DailyRate").Value = (oGrid.DataTable.GetValue("U_Z_DailyRate", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_LifeTime").Value = oGrid.DataTable.GetValue("U_Z_LifeTime", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = (oGrid.DataTable.GetValue("U_Z_GLACC", intRow))
                    oUserTable.UserFields.Fields.Item("U_Z_GLACC1").Value = (oGrid.DataTable.GetValue("U_Z_GLACC1", intRow))


                    If oUserTable.Add() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Committrans("Cancel")
                        Return False
                    Else
                        'If AddToUDT_Employee(strCode, strECode, (oGrid.DataTable.GetValue(2, intRow)), (oGrid.DataTable.GetValue(2, intRow)) / 12, (oGrid.DataTable.GetValue(4, intRow)), (oGrid.DataTable.GetValue(5, intRow)), (oGrid.DataTable.GetValue(6, intRow)), oGrid.DataTable.GetValue(7, intRow), sickLeave) = False Then
                        '    Return False
                        'End If
                    End If
                Else
                    strCode = oGrid.DataTable.GetValue("Code", intRow)
                    If oUserTable.GetByKey(strCode) Then
                        oUserTable.Code = strCode.Trim()
                        oUserTable.Name = strECode.Trim()
                        oUserTable.UserFields.Fields.Item("U_Z_FrgnName").Value = oGrid.DataTable.GetValue("U_Z_FrgnName", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_DedRate").Value = (oGrid.DataTable.GetValue("U_Z_DedRate", intRow))
                        oCombobox = oGrid.Columns.Item("U_Z_PaidLeave")
                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_PaidLeave").Value = oCombobox.GetSelectedValue(intRow).Value
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_PaidLeave").Value = "U"
                        End Try
                        oUserTable.UserFields.Fields.Item("U_Z_DaysYear").Value = oGrid.DataTable.GetValue("U_Z_DaysYear", intRow)
                        '  oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = (oGrid.DataTable.GetValue("U_Z_DaysYear", intRow)) / 12
                        OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_Accured")
                        If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                            oUserTable.UserFields.Fields.Item("U_Z_Accured").Value = "Y"
                            oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = (oGrid.DataTable.GetValue("U_Z_DaysYear", intRow)) / 12
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_Accured").Value = "N"
                            oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = 0
                        End If
                        oCombobox = oGrid.Columns.Item("U_Z_Cutoff")
                        Try
                            oUserTable.UserFields.Fields.Item("U_Z_Cutoff").Value = oCombobox.GetSelectedValue(intRow).Value
                        Catch ex As Exception
                            oUserTable.UserFields.Fields.Item("U_Z_Cutoff").Value = "N"
                        End Try
                        OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_EOS")
                        If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                            oUserTable.UserFields.Fields.Item("U_Z_EOS").Value = "Y"
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_EOS").Value = "N"
                        End If

                        OCHECKBOXCOLUMN = oGrid.Columns.Item("U_Z_StopProces")
                        If OCHECKBOXCOLUMN.IsChecked(intRow) Then
                            oUserTable.UserFields.Fields.Item("U_Z_StopProces").Value = "Y"
                        Else
                            oUserTable.UserFields.Fields.Item("U_Z_StopProces").Value = "N"
                        End If

                        oUserTable.UserFields.Fields.Item("U_Z_EntAft").Value = oGrid.DataTable.GetValue("U_Z_EntAft", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_TimesTaken").Value = (oGrid.DataTable.GetValue("U_Z_TimesTaken", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_MaxDays").Value = oGrid.DataTable.GetValue("U_Z_MaxDays", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_DailyRate").Value = (oGrid.DataTable.GetValue("U_Z_DailyRate", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_LifeTime").Value = oGrid.DataTable.GetValue("U_Z_LifeTime", intRow)
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = (oGrid.DataTable.GetValue("U_Z_GLACC", intRow))
                        oUserTable.UserFields.Fields.Item("U_Z_GLACC1").Value = (oGrid.DataTable.GetValue("U_Z_GLACC1", intRow))
                        If oUserTable.Update() <> 0 Then
                            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Committrans("Cancel")
                            Return False
                        Else
                            'If AddToUDT_Employee(strCode, strECode, (oGrid.DataTable.GetValue(2, intRow)), (oGrid.DataTable.GetValue(2, intRow)) / 12, (oGrid.DataTable.GetValue(4, intRow)), (oGrid.DataTable.GetValue(5, intRow)), oGrid.DataTable.GetValue(6, intRow), oGrid.DataTable.GetValue(7, intRow), sickLeave) = False Then
                            '    Return False
                            'End If
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



    Private Function AddToUDT_Employee(ByVal leavecode As String, ByVal leavename As String, ByVal daysinyear As Double, ByVal noofdays As Double, ByVal paid As String, ByVal GLAcct As String, ByVal OB As Double, ByVal GLAcct1 As String, ByVal sickLeave As String) As Boolean
        Dim strTable, strEmpId, strCode, strType As String
        Dim dblValue As Double
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim oValidateRS, oTemp As SAPbobsCOM.Recordset
        oUserTable = oApplication.Company.UserTables.Item("Z_PAY4")
        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp.DoQuery("Select * from [OHEM] order by EmpID")
        strTable = "@Z_PAY4"

        strType = leavecode
        Dim strQuery As String
        If strType <> "" Then
            strQuery = "Update [@Z_PAY4] set U_Z_GLACC='" & GLAcct & "' ,U_Z_GLACC1='" & GLAcct1 & "' where U_Z_LeaveCode='" & leavecode & "'"
            oValidateRS.DoQuery(strQuery)
        End If


        For intRow As Integer = 0 To oTemp.RecordCount - 1
            If leavecode <> "" Then
                strEmpId = oTemp.Fields.Item("empID").Value
                oValidateRS.DoQuery("Select * from [@Z_PAY4] where U_Z_LeaveCode='" & leavecode & "' and U_Z_EMPID='" & strEmpId & "'")
                If oValidateRS.RecordCount > 0 Then
                    strCode = oValidateRS.Fields.Item("Code").Value
                    OB = oValidateRS.Fields.Item("U_Z_OB").Value
                Else
                    strCode = ""
                    OB = OB
                End If
                If strCode <> "" Then ' oUserTable.GetByKey(strCode) Then
                    'oUserTable.Code = strCode
                    'oUserTable.Name = strCode
                    'oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    'oUserTable.UserFields.Fields.Item("U_Z_LeaveCode").Value = leavecode
                    'oUserTable.UserFields.Fields.Item("U_Z_LeaveName").Value = leavename
                    'oUserTable.UserFields.Fields.Item("U_Z_DaysYear").Value = daysinyear
                    'oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = noofdays
                    'oUserTable.UserFields.Fields.Item("U_Z_PaidLeave").Value = paid
                    'oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLAcct
                    'oUserTable.UserFields.Fields.Item("U_Z_GLACC1").Value = GLAcct1
                    'oUserTable.UserFields.Fields.Item("U_Z_OB").Value = OB
                    'If oUserTable.Update <> 0 Then
                    '    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    '    Return False
                    'End If
                Else
                    strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode + "N"
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveCode").Value = leavecode
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveName").Value = leavename
                    oUserTable.UserFields.Fields.Item("U_Z_DaysYear").Value = daysinyear
                    oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = noofdays
                    oUserTable.UserFields.Fields.Item("U_Z_PaidLeave").Value = paid
                    oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = GLAcct
                    oUserTable.UserFields.Fields.Item("U_Z_GLACC1").Value = GLAcct1
                    oUserTable.UserFields.Fields.Item("U_Z_OB").Value = OB
                    oUserTable.UserFields.Fields.Item("U_Z_SickLeave").Value = sickLeave
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
                strCode = agrid.DataTable.GetValue("Code", intRow)
                otemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                If oApplication.Utilities.ValidateDeletionMaster(strCode, "Leave") = False Then
                    Exit Sub
                End If
                oApplication.Utilities.ExecuteSQL(oTemp, "update [@Z_PAY_LEAVE] set  NAME =NAME +'XX'  where ""Code""='" & strCode & "'")
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
            strECode = aGrid.DataTable.GetValue("Code", intRow)
            strEname = aGrid.DataTable.GetValue("Name", intRow)
            If strECode <> "" Then
                For intInnerLoop As Integer = intRow To aGrid.DataTable.Rows.Count - 1
                    strECode1 = aGrid.DataTable.GetValue("Code", intInnerLoop)
                    strEname1 = aGrid.DataTable.GetValue("Name", intInnerLoop)
                    If strECode1 <> "" And strEname1 = "" Then
                        oApplication.Utilities.Message("Name can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                    If strECode1 = "" And strEname1 <> "" Then
                        oApplication.Utilities.Message("Code can not be empty ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                    If strECode = strECode1 And intRow <> intInnerLoop Then
                        oApplication.Utilities.Message("This entry  already exists. Code no : " & strECode, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aGrid.Columns.Item(0).Click(intInnerLoop, , 1)
                        Return False
                    End If
                Next
                Try
                    If CDbl(aGrid.DataTable.GetValue("U_Z_DaysYear", intRow) <= 0) Then
                        ' oApplication.Utilities.Message("Yearly upperlimit should be greater than zero", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        ' aGrid.Columns.Item("U_Z_DaysYear").Click(intRow)
                        '  Return False
                    End If
                Catch ex As Exception
                    '  oApplication.Utilities.Message("Yearly upperlimit should be greater than zero", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    '  aGrid.Columns.Item("U_Z_DaysYear").Click(intRow)
                    '  Return False
                End Try

                Try
                    If CDbl(aGrid.DataTable.GetValue("U_Z_DailyRate", intRow) <= 0) Then
                        oApplication.Utilities.Message("Daily Rate Days  should be greater than zero", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aGrid.Columns.Item("U_Z_DailyRate").Click(intRow)
                        Return False
                    End If
                Catch ex As Exception
                    oApplication.Utilities.Message("Daily Rate Days  should be greater than zero", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    aGrid.Columns.Item("U_Z_DailyRate").Click(intRow)
                    Return False
                End Try

                If aGrid.DataTable.GetValue("U_Z_GLACC", intRow) = "" Then
                    oApplication.Utilities.Message("Debit G/L Account is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                'SELECT T0.[Code], T0.[Name], T0.[U_Z_DedRate], T0.[U_Z_PaidLeave], T0.[U_Z_DaysYear], T0.[U_Z_NoofDays], T0.[U_Z_Accured], T0.[U_Z_Cutoff],
                ' T0.[U_Z_EOS], T0.[U_Z_EntAft], T0.[U_Z_TimesTaken], T0.[U_Z_MaxDays], T0.[U_Z_DailyRate], T0.[U_Z_LifeTime], T0.[U_Z_GLACC], T0.[U_Z_GLACC1], T0.[U_Z_OffCycle], T0.[U_Z_OB], T0.[U_Z_SickLeave] FROM [dbo].[@Z_PAY_LEAVE]  T0

                oCombobox = aGrid.Columns.Item("U_Z_PaidLeave")
                If oCombobox.GetSelectedValue(intRow).Value = "A" Then
                    If aGrid.DataTable.GetValue("U_Z_GLACC1", intRow) = "" Then
                        oApplication.Utilities.Message("Credit G/L Account is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
            End If
        Next
        Return True
    End Function

#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_LeaveMaster Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                If pVal.ItemUID = "2" Then
                                    Committrans("Cancel")
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                oGrid = oForm.Items.Item("5").Specific
                                If pVal.ItemUID = "5" And pVal.ColUID = "Code" And pVal.CharPressed <> 9 Then
                                    If oApplication.Utilities.ValidateDeletionMaster(oGrid.DataTable.GetValue("Code", pVal.Row), "Leave") = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                oGrid = oForm.Items.Item("5").Specific
                                If pVal.ItemUID = "5" And pVal.ColUID = "Code" Then
                                    If oApplication.Utilities.ValidateDeletionMaster(oGrid.DataTable.GetValue("Code", pVal.Row), "Leave") = False Then
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
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                oGrid = oForm.Items.Item("5").Specific
                                If pVal.ItemUID = "5" And pVal.ColUID = "U_Z_DaysYear" And pVal.CharPressed = 9 Then
                                    Dim st As String
                                    st = oGrid.DataTable.GetValue("U_Z_DaysYear", pVal.Row)
                                    If CDbl(st) > 0 Then
                                        oGrid.DataTable.SetValue("U_Z_NoofDays", pVal.Row, oGrid.DataTable.GetValue("U_Z_DaysYear", pVal.Row) / 12)
                                    End If
                                End If

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
                                        If pVal.ItemUID = "5" And (pVal.ColUID = "U_Z_GLACC" Or pVal.ColUID = "U_Z_GLACC1") Then
                                            oGrid = oForm.Items.Item("5").Specific
                                            val = oDataTable.GetValue("FormatCode", 0)
                                            Try
                                                oGrid.DataTable.SetValue(pVal.ColUID, pVal.Row, val)
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
                Case mnu_hr_LeaveMaster
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
                Select Case pVal.MenuUID
                    'Case mnu_hr_LeaveMaster
                    '    oMenuobject = New clsEarning
                    '    oMenuobject.MenuEvent(pVal, BubbleEvent)
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
