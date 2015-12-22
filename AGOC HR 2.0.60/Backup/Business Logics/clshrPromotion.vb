Public Class clshrPromotion
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
    Private Sub LoadForm(ByVal oForm As SAPbouiCOM.Form)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_Promotion) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_Promotion1, frm_hr_Promotion)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.DataSources.UserDataSources.Add("dtDate1", SAPbouiCOM.BoDataType.dt_DATE)
        oForm.DataSources.UserDataSources.Add("stCode1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "63", "dtDate1")
        oApplication.Utilities.setUserDatabind(oForm, "65", "stCode1")
      
        oForm.Freeze(True)
        'oForm.DataSources.UserDataSources.Add("stDate1", SAPbouiCOM.BoDataType.dt_DATE)
        'oApplication.Utilities.setUserDatabind(oForm, "15", "stDate1")
        'oApplication.Utilities.setEdittextvalue(oForm, "15", Now.Date)
        AddChooseFromList(oForm)
        oForm.DataSources.UserDataSources.Add("Reqno", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "1000004", "Reqno")
        oEditText = oForm.Items.Item("1000004").Specific
        oEditText.ChooseFromListUID = "CFL1"
        oEditText.ChooseFromListAlias = "empID"
        oForm.DataSources.UserDataSources.Add("PosCode", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "46", "PosCode")
        oEditText = oForm.Items.Item("46").Specific
        oEditText.ChooseFromListUID = "CFL2"
        oEditText.ChooseFromListAlias = "U_Z_PosCode"
        oForm.DataSources.UserDataSources.Add("SalCode", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "58", "SalCode")
        oEditText = oForm.Items.Item("58").Specific
        oEditText.ChooseFromListUID = "CFL3"
        oEditText.ChooseFromListAlias = "U_Z_SalCode"
        oForm.DataSources.UserDataSources.Add("dtincamt", SAPbouiCOM.BoDataType.dt_SUM)
        oApplication.Utilities.setUserDatabind(oForm, "69", "dtincamt")
        oForm.DataSources.UserDataSources.Add("dtFrom", SAPbouiCOM.BoDataType.dt_DATE)
        oApplication.Utilities.setUserDatabind(oForm, "61", "dtFrom")
        oApplication.Utilities.setEdittextvalue(oForm, "61", Now.Date)
        oForm.DataSources.UserDataSources.Add("dteffFrom", SAPbouiCOM.BoDataType.dt_DATE)
        oApplication.Utilities.setUserDatabind(oForm, "73", "dteffFrom")
        oForm.DataSources.UserDataSources.Add("dteffto", SAPbouiCOM.BoDataType.dt_DATE)
        oApplication.Utilities.setUserDatabind(oForm, "72", "dteffto")
        FillDepartment(oForm)
        oForm.PaneLevel = 1
        Dim osta As SAPbouiCOM.StaticText
        osta = oForm.Items.Item("19").Specific
        osta.Caption = "Step " & oForm.PaneLevel & " of 4"
        oForm.Items.Item("19").TextStyle = SAPbouiCOM.BoTextStyle.ts_BOLD
        oForm.Items.Item("24").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
        oForm.Items.Item("1000010").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
        oForm.Items.Item("1000009").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
        reDrawForm(oForm)
        oForm.Freeze(False)
    End Sub
    Public Sub LoadForm1(ByVal oForm As SAPbouiCOM.Form, ByVal Empid As String)
        oForm = oApplication.Utilities.LoadForm(xml_hr_Promotion1, frm_hr_Promotion)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.DataSources.UserDataSources.Add("dtDate1", SAPbouiCOM.BoDataType.dt_DATE)
        oForm.DataSources.UserDataSources.Add("stCode1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "63", "dtDate1")
        oApplication.Utilities.setUserDatabind(oForm, "65", "stCode1")
        AddChooseFromList(oForm)
        oForm.DataSources.UserDataSources.Add("Reqno", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "1000004", "Reqno")
        oEditText = oForm.Items.Item("1000004").Specific
        oEditText.ChooseFromListUID = "CFL1"
        oEditText.ChooseFromListAlias = "empID"
        oForm.DataSources.UserDataSources.Add("PosCode", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "46", "PosCode")
        oEditText = oForm.Items.Item("46").Specific
        oEditText.ChooseFromListUID = "CFL2"
        oEditText.ChooseFromListAlias = "U_Z_PosCode"
        oForm.DataSources.UserDataSources.Add("SalCode", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "58", "SalCode")
        oEditText = oForm.Items.Item("58").Specific
        oEditText.ChooseFromListUID = "CFL3"
        oEditText.ChooseFromListAlias = "U_Z_SalCode"
        oForm.DataSources.UserDataSources.Add("dtincamt", SAPbouiCOM.BoDataType.dt_SUM)
        oApplication.Utilities.setUserDatabind(oForm, "69", "dtincamt")
        oForm.DataSources.UserDataSources.Add("dtFrom", SAPbouiCOM.BoDataType.dt_DATE)
        oApplication.Utilities.setUserDatabind(oForm, "61", "dtFrom")
        oApplication.Utilities.setEdittextvalue(oForm, "61", Now.Date)
        oForm.DataSources.UserDataSources.Add("dteffFrom", SAPbouiCOM.BoDataType.dt_DATE)
        oApplication.Utilities.setUserDatabind(oForm, "73", "dteffFrom")
        oForm.DataSources.UserDataSources.Add("dteffto", SAPbouiCOM.BoDataType.dt_DATE)
        oApplication.Utilities.setUserDatabind(oForm, "72", "dteffto")
        FillDepartment(oForm)
        Dim osta As SAPbouiCOM.StaticText
        osta = oForm.Items.Item("19").Specific
        osta.Caption = "Step " & oForm.PaneLevel & " of 4"
        oForm.Items.Item("19").TextStyle = SAPbouiCOM.BoTextStyle.ts_BOLD
        oForm.Items.Item("24").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
        oForm.Items.Item("1000010").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
        oForm.Items.Item("1000009").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
        oForm.PaneLevel = 2
        oApplication.Utilities.setEdittextvalue(oForm, "1000004", Empid)
        oApplication.SBO_Application.SendKeys("{TAB}")
        oForm.Items.Item("3").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oForm.Freeze(False)
    End Sub

    Private Sub AddChooseFromList(ByVal objForm As SAPbouiCOM.Form)
        Try
            Dim oCFLs As SAPbouiCOM.ChooseFromListCollection

            oCFLs = objForm.ChooseFromLists
            Dim oCFL As SAPbouiCOM.ChooseFromList
            Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)

            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "171"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_OPOSIN"
            oCFLCreationParams.UniqueID = "CFL2"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_OSALST"
            oCFLCreationParams.UniqueID = "CFL3"
            oCFL = oCFLs.Add(oCFLCreationParams)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub FillDepartment(ByVal sform As SAPbouiCOM.Form)
        oCombobox = sform.Items.Item("1000011").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select Code,Remarks from OUDP")
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        sform.Items.Item("1000011").DisplayDesc = False
    End Sub
    Private Sub Gridbind(ByVal strempid As String, ByVal aForm As SAPbouiCOM.Form)
        Dim strqry, strQry1 As String
        oGrid = aForm.Items.Item("10").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
        ' strQry1 = "Select U_Z_HRAPPID from [@Z_HR_HEM1] where U_Z_Dept='" & oCombobox.Selected.Value & "' and U_Z_ReqNo='" & oApplication.Utilities.getEdittextvalue(aForm, "20") & "' and Name =Code"
        strqry = "	select ""Code"", ""U_Z_EmpId"",""U_Z_DeptName"",""U_Z_PosCode"",""U_Z_PosName"",""U_Z_JobCode"",""U_Z_JobName"",""U_Z_OrgCode"",""U_Z_OrgName"",""U_Z_SalCode"","
        strqry = strqry & """U_Z_JoinDate"",""U_Z_ProJoinDate"",""U_Z_IncAmount"",""U_Z_EffFromdt"",""U_Z_EffTodt"","
        strqry += " case ""U_Z_AppStatus"" when 'P' then 'Pending' when 'A' then 'Approved' when 'R' then 'Rejected' end as ""U_Z_AppStatus""  from ""@Z_HR_HEM2"" where ""U_Z_EmpId""='" & strempid & "' "
        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("Code").Visible = False
        oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee Id"
        oGrid.Columns.Item("U_Z_EmpId").Editable = False
        oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
        oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
        oGrid.Columns.Item("U_Z_PosCode").TitleObject.Caption = "Position Code"
        oGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position Name "
        oGrid.Columns.Item("U_Z_JobCode").TitleObject.Caption = "Job Code"
        oGrid.Columns.Item("U_Z_JobName").TitleObject.Caption = "Job Name"
        oGrid.Columns.Item("U_Z_OrgCode").TitleObject.Caption = "Organization Code"
        oGrid.Columns.Item("U_Z_OrgCode").Visible = False
        oGrid.Columns.Item("U_Z_OrgName").TitleObject.Caption = "Organization Name"
        oGrid.Columns.Item("U_Z_OrgName").Visible = False
        oGrid.Columns.Item("U_Z_SalCode").TitleObject.Caption = "Salary Code"
        oGrid.Columns.Item("U_Z_SalCode").Visible = False
        oGrid.Columns.Item("U_Z_ProJoinDate").TitleObject.Caption = "Promotion Date"
        oGrid.Columns.Item("U_Z_IncAmount").TitleObject.Caption = "Increment Amount"
        oGrid.Columns.Item("U_Z_EffFromdt").TitleObject.Caption = "Effective From"
        oGrid.Columns.Item("U_Z_EffTodt").TitleObject.Caption = "Effective To"
        oGrid.Columns.Item("U_Z_JoinDate").TitleObject.Caption = "Effective From Date"
        oGrid.Columns.Item("U_Z_JoinDate").Visible = False
        oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oApplication.Utilities.AssignRowNo(oGrid, aForm)
    End Sub
#Region "AddToUDT"
    Private Function AddToUDTPromotion(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim strTable, strCode, strdate, strefffrom, streffto, empid, strSQL As String
        Dim prodt, Efffrmdt, Efftodt As Date
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim oValidateRS, otemp2, oTemp As SAPbobsCOM.Recordset
        strSQL = "SELECT T0.U_Z_PosName,T0.U_Z_JobCode,T0.U_Z_JobName,T0.U_Z_DeptCode,T0.U_Z_DeptName,T1.U_Z_OrgCode,T1.U_Z_OrgDesc  FROM [@Z_HR_OPOSIN]  T0 Left Join [dbo].[@Z_HR_ORGST]  T1 on T0.U_Z_PosCode=T1.U_Z_PosCode where T0.U_Z_PosCode='" & oApplication.Utilities.getEdittextvalue(oForm, "46") & "'"
        oCombobox = aForm.Items.Item("1000011").Specific
        If oApplication.Company.InTransaction() Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        empid = oApplication.Utilities.getEdittextvalue(oForm, "13")
        Dim strDeptCode As String
        otemp2.DoQuery("Select * from OHEM where empID=" & empid)
        strDeptCode = otemp2.Fields.Item("dept").Value
        otemp2.DoQuery(strSQL)
        oApplication.Company.StartTransaction()
        Try
            oUserTable = oApplication.Company.UserTables.Item("Z_HR_HEM2")
            oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strTable = "@Z_HR_HEM2"
            strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
            oUserTable.Code = strCode
            oUserTable.Name = strCode

            oUserTable.UserFields.Fields.Item("U_Z_EmpId").Value = empid

            oUserTable.UserFields.Fields.Item("U_Z_FirstName").Value = oApplication.Utilities.getEdittextvalue(oForm, "17")
            oUserTable.UserFields.Fields.Item("U_Z_LastName").Value = oApplication.Utilities.getEdittextvalue(oForm, "1000005")
            
            oUserTable.UserFields.Fields.Item("U_Z_Dept").Value = otemp2.Fields.Item("U_Z_DeptCode").Value ' oCombobox.Selected.Value
            oUserTable.UserFields.Fields.Item("U_Z_DeptName").Value = otemp2.Fields.Item("U_Z_DeptName").Value ' oApplication.Utilities.getEdittextvalue(oForm, "44")
            oUserTable.UserFields.Fields.Item("U_Z_PosCode").Value = oApplication.Utilities.getEdittextvalue(oForm, "46")
            oUserTable.UserFields.Fields.Item("U_Z_PosName").Value = otemp2.Fields.Item("U_Z_PosName").Value ' oApplication.Utilities.getEdittextvalue(oForm, "48")
            oUserTable.UserFields.Fields.Item("U_Z_JobCode").Value = otemp2.Fields.Item("U_Z_JobCode").Value ' oApplication.Utilities.getEdittextvalue(oForm, "50")
            oUserTable.UserFields.Fields.Item("U_Z_JobName").Value = otemp2.Fields.Item("U_Z_JobName").Value 'oApplication.Utilities.getEdittextvalue(oForm, "52")
            oUserTable.UserFields.Fields.Item("U_Z_OrgCode").Value = oApplication.Utilities.getEdittextvalue(oForm, "54")
            oUserTable.UserFields.Fields.Item("U_Z_OrgName").Value = oApplication.Utilities.getEdittextvalue(oForm, "56")
            oUserTable.UserFields.Fields.Item("U_Z_JoinDate").Value = oApplication.Utilities.getEdittextvalue(oForm, "63")
            strdate = oApplication.Utilities.getEdittextvalue(oForm, "61")
            prodt = oApplication.Utilities.GetDateTimeValue(strdate)
            oUserTable.UserFields.Fields.Item("U_Z_ProJoinDate").Value = prodt
            'prodt = prodt.AddDays(-1)
            'oUserTable.UserFields.Fields.Item("U_Z_ProJoinDate").Value = oApplication.Utilities.getEdittextvalue(oForm, "61")
            oUserTable.UserFields.Fields.Item("U_Z_SalCode").Value = oApplication.Utilities.getEdittextvalue(oForm, "58")
            oUserTable.UserFields.Fields.Item("U_Z_IncAmount").Value = CDbl(oApplication.Utilities.getEdittextvalue(oForm, "69"))
            strefffrom = oApplication.Utilities.getEdittextvalue(oForm, "73")
            Efffrmdt = oApplication.Utilities.GetDateTimeValue(strefffrom)
            oUserTable.UserFields.Fields.Item("U_Z_EffFromdt").Value = Efffrmdt
            streffto = oApplication.Utilities.getEdittextvalue(oForm, "72")
            Efftodt = oApplication.Utilities.GetDateTimeValue(streffto)
            oUserTable.UserFields.Fields.Item("U_Z_EffTodt").Value = Efftodt
            oUserTable.UserFields.Fields.Item("U_Z_Status").Value = "P"
            oUserTable.UserFields.Fields.Item("U_Z_Posting").Value = "N"
            oUserTable.UserFields.Fields.Item("U_Z_AppStatus").Value = oApplication.Utilities.DocApproval(oForm, HeaderDoctype.EmpLife, strDeptCode) ' oApplication.Utilities.DocApproval(oForm, HeaderDoctype.EmpLife, otemp2.Fields.Item("U_Z_DeptCode").Value)

            Dim strUserName As String
            strUserName = oApplication.Company.UserName
            oUserTable.UserFields.Fields.Item("U_Z_CreatedBy").Value = strUserName
            oUserTable.UserFields.Fields.Item("U_Z_Credt").Value = Now.Date

            If oUserTable.Add <> 0 Then
                oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                If oApplication.Company.InTransaction() Then
                    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                End If
                Return False
            Else
                Dim strdocnum, strSql1 As String
                otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oApplication.Company.GetNewObjectCode(strdocnum)
                '  strSql1 = "Update OHEM set ""U_Z_HR_PromoCode""='" & strdocnum & "' where ""empID""='" & oApplication.Utilities.getEdittextvalue(oForm, "13") & "'"
                ' otemp2.DoQuery(strSql1)
              

                '    Dim oEmployee As SAPbobsCOM.EmployeesInfo
                '    oEmployee = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oEmployeesInfo)
                '    If oEmployee.GetByKey(oApplication.Utilities.getEdittextvalue(oForm, "13")) Then
                '        oEmployee.FirstName = oApplication.Utilities.getEdittextvalue(oForm, "17")
                '        oEmployee.LastName = oApplication.Utilities.getEdittextvalue(oForm, "1000005")
                '        oEmployee.Department = oCombobox.Selected.Value
                '        oEmployee.UserFields.Fields.Item("U_Z_HR_PosiCode").Value = oApplication.Utilities.getEdittextvalue(oForm, "46")
                '        oEmployee.UserFields.Fields.Item("U_Z_HR_PosiName").Value = oApplication.Utilities.getEdittextvalue(oForm, "48")
                '        oEmployee.UserFields.Fields.Item("U_Z_HR_JobstCode").Value = oApplication.Utilities.getEdittextvalue(oForm, "50")
                '        oEmployee.UserFields.Fields.Item("U_Z_HR_JobstName").Value = oApplication.Utilities.getEdittextvalue(oForm, "52")
                '        oEmployee.UserFields.Fields.Item("U_Z_HR_OrgstCode").Value = oApplication.Utilities.getEdittextvalue(oForm, "54")
                '        oEmployee.UserFields.Fields.Item("U_Z_HR_OrgstName").Value = oApplication.Utilities.getEdittextvalue(oForm, "56")
                '        oEmployee.UserFields.Fields.Item("U_Z_HR_SalaryCode").Value = oApplication.Utilities.getEdittextvalue(oForm, "58")
                '        prodt = oApplication.Utilities.GetDateTimeValue(strdate)
                '        oEmployee.UserFields.Fields.Item("U_Z_HR_JoinDate").Value = prodt ' oApplication.Utilities.getEdittextvalue(oForm, "61")
                '        oEmployee.UserFields.Fields.Item("U_Z_EmpLiCyStatus").Value = "P"
                '        If oEmployee.Update <> 0 Then
                '            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '            If oApplication.Company.InTransaction() Then
                '                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                '            End If
                '            Return False
                '        Else
                '            oApplication.Utilities.Message("Operation Completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                '        End If
                '    End If

                If oApplication.Company.InTransaction() Then
                    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                End If
                Return True

                'If oApplication.Utilities.UpdateEmployeeProfile(oForm, oApplication.Utilities.getEdittextvalue(oForm, "13"), oApplication.Utilities.getEdittextvalue(oForm, "46"), prodt, "P", strCode) = True Then
                '    If oApplication.Company.InTransaction() Then
                '        oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                '    End If
                '    Return True
                'Else
                '    If oApplication.Company.InTransaction() Then
                '        oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                '    End If
                '    Return False
                'End If
                End If

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

    Private Sub reDrawForm(ByVal oForm As SAPbouiCOM.Form)
        Try
            oForm.Freeze(True)
            oForm.Items.Item("66").Width = oForm.Width - 30
            'oForm.Items.Item("69").Height = oForm.Height - 160

            oForm.Items.Item("67").Width = oForm.Width - 30
            oForm.Items.Item("67").Height = oForm.Items.Item("10").Height + 20
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
        End Try
    End Sub

#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim strDept, Reqno As String
            Reqno = oApplication.Utilities.getEdittextvalue(aForm, "1000004")
            If Reqno = "" Then
                oApplication.Utilities.Message("Enter Employee Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If

            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function

    Private Function Validation1(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim strDept, Reqno, stSQL1 As String
            Dim EffeFromdate, EffeTodate, NewEffFrom, NewEffTo As Date
            'oCombobox = oForm.Items.Item("1000011").Specific
            'Try
            '    strDept = oCombobox.Selected.Value
            'Catch ex As Exception

            'End Try

            If oApplication.Utilities.getEdittextvalue(aForm, "61") = "" Then
                oApplication.Utilities.Message("Enter Promotion Date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "69") = "" Then
                oApplication.Utilities.Message("Enter Increment Amount...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "73") = "" Then
                oApplication.Utilities.Message("Enter Effective From Date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "72") = "" Then
                ' oApplication.Utilities.Message("Enter Effective To Date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '  Return False
            End If
            'If oApplication.Utilities.getEdittextvalue(aForm, "46") = "" Then
            '    oApplication.Utilities.Message("Enter Position Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '    Return False
            'End If
            'If strDept = "" Then
            '    oApplication.Utilities.Message("Enter Department...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '    Return False
            'End If

            EffeFromdate = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "61"))
            EffeTodate = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "63"))

            NewEffFrom = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "73"))
            NewEffTo = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "72"))
            If oApplication.Utilities.getEdittextvalue(aForm, "72") <> "" Then
                If NewEffFrom > NewEffTo Then
                    oApplication.Utilities.Message("New Promotion Effect To Date must be greater than or equal to New Promotion Effect From Date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            End If

            'If EffeFromdate < EffeTodate Then
            '    oApplication.Utilities.Message("Effect To Date must be greater than or equal to Promotion date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '    Return False

            'End If

            If oApplication.Utilities.getEdittextvalue(aForm, "46") = oApplication.Utilities.getEdittextvalue(aForm, "30") Then
                oApplication.Utilities.Message("New Position should be different then Existing position ...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
#End Region
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_Promotion Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim strcode As String
                                If pVal.ItemUID = "3" And oForm.PaneLevel = 2 Then
                                    If Validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                If pVal.ItemUID = "5" And oForm.PaneLevel = 4 Then
                                    If Validation1(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                If pVal.ItemUID = "74" Then
                                    strcode = oApplication.Utilities.getEdittextvalue(oForm, "30")
                                    oApplication.Utilities.OpenMasterinLink(oForm, "Position", strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                ElseIf pVal.ItemUID = "80" Then
                                    strcode = oApplication.Utilities.getEdittextvalue(oForm, "46")
                                    oApplication.Utilities.OpenMasterinLink(oForm, "Position", strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                ElseIf pVal.ItemUID = "75" Then
                                    strcode = oApplication.Utilities.getEdittextvalue(oForm, "34")
                                    oApplication.Utilities.OpenMasterinLink(oForm, "JobScreen", strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                ElseIf pVal.ItemUID = "81" Then
                                    strcode = oApplication.Utilities.getEdittextvalue(oForm, "50")
                                    oApplication.Utilities.OpenMasterinLink(oForm, "JobScreen", strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                ElseIf pVal.ItemUID = "76" Or pVal.ItemUID = "82" Then
                                    oApplication.Utilities.OpenMasterinLink(oForm, "OrgStructure")
                                    BubbleEvent = False
                                    Exit Sub
                                ElseIf pVal.ItemUID = "77" Then
                                    strcode = oApplication.Utilities.getEdittextvalue(oForm, "1000008")
                                    oApplication.Utilities.OpenMasterinLink(oForm, "Salary", strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                ElseIf pVal.ItemUID = "83" Then
                                    strcode = oApplication.Utilities.getEdittextvalue(oForm, "79")
                                    oApplication.Utilities.OpenMasterinLink(oForm, "Salary", strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                reDrawForm(oForm)
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1000011" Then
                                    Dim strdep As String
                                    oCombobox = oForm.Items.Item("1000011").Specific
                                    strdep = oCombobox.Selected.Description
                                    oApplication.Utilities.setEdittextvalue(oForm, "44", strdep)
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "85"
                                        Dim objHistory As New clshrAppHisDetails
                                        objHistory.LoadForm(oForm, HistoryDoctype.EmpPro, oApplication.Utilities.getEdittextvalue(oForm, "13"))
                                    Case "4"
                                        oForm.Freeze(True)
                                        Dim osta As SAPbouiCOM.StaticText
                                        osta = oForm.Items.Item("19").Specific
                                        osta.Caption = "Step " & oForm.PaneLevel & " of 4"
                                        oForm.PaneLevel = oForm.PaneLevel - 1
                                        If oForm.PaneLevel = 2 Then
                                            osta.Caption = "Step " & oForm.PaneLevel & " of 4"
                                        End If
                                        If oForm.PaneLevel = 3 Then
                                            osta.Caption = "Step " & oForm.PaneLevel & " of 4"
                                        End If
                                        If oForm.PaneLevel = 1 Then
                                            osta.Caption = "Step " & oForm.PaneLevel & " of 4"
                                        End If
                                        oForm.Freeze(False)
                                    Case "3"
                                        oForm.Freeze(True)
                                        Dim osta As SAPbouiCOM.StaticText
                                        osta = oForm.Items.Item("19").Specific
                                        oForm.PaneLevel = oForm.PaneLevel + 1
                                        If oForm.PaneLevel = 2 Then
                                            osta.Caption = "Step " & oForm.PaneLevel & " of 4"
                                        End If
                                        If oForm.PaneLevel = 3 Then
                                            osta.Caption = "Step " & oForm.PaneLevel & " of 4"
                                            Dim strempid As Integer
                                            strempid = oApplication.Utilities.getEdittextvalue(oForm, "1000004")
                                            Dim strqry As String
                                            Dim otemp, otemp1 As SAPbobsCOM.Recordset
                                            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                            otemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                            strqry = "select empID,firstName,lastName,U_Z_HR_JoinDate,dept,U_Z_HR_PosiCode,U_Z_HR_PosiName,startDate,U_Z_HR_PromoCode,"
                                            strqry = strqry & "	U_Z_HR_JobstCode,U_Z_HR_JobstName,U_Z_HR_OrgstCode,U_Z_HR_OrgstName,U_Z_HR_SalaryCode  from OHEM where empID='" & strempid & "'"
                                            otemp.DoQuery(strqry)
                                            If otemp.RecordCount > 0 Then
                                                oApplication.Utilities.setEdittextvalue(oForm, "13", otemp.Fields.Item("empID").Value)
                                                oApplication.Utilities.setEdittextvalue(oForm, "17", otemp.Fields.Item("firstName").Value)
                                                oApplication.Utilities.setEdittextvalue(oForm, "1000005", otemp.Fields.Item("lastName").Value)
                                                oApplication.Utilities.setEdittextvalue(oForm, "63", otemp.Fields.Item("U_Z_HR_JoinDate").Value)
                                                oApplication.Utilities.setEdittextvalue(oForm, "26", otemp.Fields.Item("dept").Value)
                                                oApplication.Utilities.setEdittextvalue(oForm, "30", otemp.Fields.Item("U_Z_HR_PosiCode").Value)
                                                oApplication.Utilities.setEdittextvalue(oForm, "32", otemp.Fields.Item("U_Z_HR_PosiName").Value)
                                                oApplication.Utilities.setEdittextvalue(oForm, "34", otemp.Fields.Item("U_Z_HR_JobstCode").Value)
                                                oApplication.Utilities.setEdittextvalue(oForm, "36", otemp.Fields.Item("U_Z_HR_JobstName").Value)
                                                oApplication.Utilities.setEdittextvalue(oForm, "38", otemp.Fields.Item("U_Z_HR_OrgstCode").Value)
                                                oApplication.Utilities.setEdittextvalue(oForm, "40", otemp.Fields.Item("U_Z_HR_OrgstName").Value)
                                                oApplication.Utilities.setEdittextvalue(oForm, "1000008", otemp.Fields.Item("U_Z_HR_SalaryCode").Value)
                                                oApplication.Utilities.setEdittextvalue(oForm, "15", otemp.Fields.Item("startDate").Value)
                                                oApplication.Utilities.setEdittextvalue(oForm, "65", otemp.Fields.Item("U_Z_HR_PromoCode").Value)
                                                otemp1.DoQuery("Select Remarks from OUDP where Code='" & otemp.Fields.Item("dept").Value & "'")
                                                oApplication.Utilities.setEdittextvalue(oForm, "28", otemp1.Fields.Item("Remarks").Value)
                                            End If
                                            Gridbind(strempid, oForm)
                                        End If
                                        If oForm.PaneLevel = 4 Then
                                            osta.Caption = "Step " & oForm.PaneLevel & " of 4"
                                        End If
                                        oForm.Freeze(False)
                                    Case "5"
                                        If oApplication.SBO_Application.MessageBox("Do you want confirm the Employee Promotion?", , "Yes", "No") = 2 Then
                                            Exit Sub
                                        Else
                                            oApplication.Utilities.PopulatePositionDetails(oForm, oApplication.Utilities.getEdittextvalue(oForm, "46"), "P")
                                            If AddToUDTPromotion(oForm) Then
                                                oApplication.Utilities.Message("Employee Promotion Completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                                oForm.PaneLevel = 1
                                                ' oForm.Close()
                                            End If
                                        End If
                                End Select
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1 As String
                                Dim sCHFL_ID, val, val2, val3, val4, val5, val6, val7 As String
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

                                        If pVal.ItemUID = "1000004" Then
                                            val = oDataTable.GetValue("firstName", 0)
                                            val2 = oDataTable.GetValue("lastName", 0)
                                            val1 = oDataTable.GetValue("empID", 0)

                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "60", val2)
                                                oApplication.Utilities.setEdittextvalue(oForm, "20", val)
                                                oApplication.Utilities.setEdittextvalue(oForm, "1000004", val1)
                                            Catch ex As Exception
                                            End Try
                                        End If

                                        If pVal.ItemUID = "58" Then
                                            val = oDataTable.GetValue("U_Z_SalCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "58", val)
                                            Catch ex As Exception
                                            End Try
                                        End If

                                        If pVal.ItemUID = "46" Then
                                            val1 = oDataTable.GetValue("U_Z_PosCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "46", val1)
                                            Catch ex As Exception
                                            End Try
                                            oApplication.Utilities.PopulatePositionDetails(oForm, val1, "P")
                                            oApplication.Utilities.setEdittextvalue(oForm, "79", oDataTable.GetValue("U_Z_SalCode", 0))
                                        End If
                                        oForm.Freeze(False)
                                    End If
                                Catch ex As Exception
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
                Case mnu_hr_Promotion1
                    LoadForm(oForm)
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
