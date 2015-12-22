Imports System.IO
Public Class clshrInitializeAppraisal
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText, oEditFDate, oEditTDate As SAPbouiCOM.EditText
    Private oCombobox, oCombobox1, oCombobox2, oCombobox3, oComboLevel As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oComboBoxcolumn As SAPbouiCOM.ComboBoxColumn
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo, sPath As String
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Private oDtAppraisal, oDtAppraisal1 As SAPbouiCOM.DataTable
    Private oRecordSet As SAPbobsCOM.Recordset
    Private sQuery As String

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub

    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_IniAppraisal) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_IniAppraisal, frm_hr_IniAppraisal)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        FillDepartment(oForm)
        FillPosition(oForm)
        FillPeriod(oForm)
        AddChooseFromList(oForm)
        oForm.DataSources.UserDataSources.Add("empno", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "20", "empno")
        oEditText = oForm.Items.Item("20").Specific
        oEditFDate = oForm.Items.Item("34").Specific
        oEditTDate = oForm.Items.Item("36").Specific
        oForm.DataSources.UserDataSources.Add("fdate", SAPbouiCOM.BoDataType.dt_DATE)
        oApplication.Utilities.setUserDatabind(oForm, "34", "fdate")
        oForm.DataSources.UserDataSources.Add("tdate", SAPbouiCOM.BoDataType.dt_DATE)
        oApplication.Utilities.setUserDatabind(oForm, "36", "tdate")
        oEditText.ChooseFromListUID = "CFL1"
        oEditText.ChooseFromListAlias = "empId"
        oForm.DataSources.UserDataSources.Add("empno1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "23", "empno1")
        oEditText = oForm.Items.Item("23").Specific
        oEditText.ChooseFromListUID = "CFL2"
        oEditText.ChooseFromListAlias = "empId"
        oForm.PaneLevel = 1
        Dim osta As SAPbouiCOM.StaticText
        oComboLevel = oForm.Items.Item("38").Specific
        oComboLevel.ValidValues.Add("SA", "Self Appraisal")
        oComboLevel.ValidValues.Add("LM", "Line Manager")
        oComboLevel.Select("SA", SAPbouiCOM.BoSearchKey.psk_ByValue)
        oComboLevel.ExpandType = SAPbouiCOM.BoExpandType.et_ValueDescription
        osta = oForm.Items.Item("19").Specific
        osta.Caption = "Step " & oForm.PaneLevel & " of 4"
        oForm.Items.Item("19").TextStyle = SAPbouiCOM.BoTextStyle.ts_BOLD
        InitializeAppTable()
        InitializeMailTable()
        oForm.Freeze(False)
    End Sub

    Private Sub AddChooseFromList(ByVal objForm As SAPbouiCOM.Form)
        Try
            Dim oCFLs As SAPbouiCOM.ChooseFromListCollection
            Dim oCons As SAPbouiCOM.Conditions
            Dim oCon As SAPbouiCOM.Condition
            oCombobox = objForm.Items.Item("7").Specific
            oCFLs = objForm.ChooseFromLists
            Dim oCFL As SAPbouiCOM.ChooseFromList
            Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)

            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "171"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)

            ' oCFL = oCFLs.Add(oCFLCreationParams)
            ' Adding Conditions to CFL1
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()


            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "171"
            oCFLCreationParams.UniqueID = "CFL2"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()


        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub FillDepartment(ByVal sform As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = sform.Items.Item("1000007").Specific
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oTempRec.DoQuery("Select Code,Remarks from OUDP")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item("Code").Value, oTempRec.Fields.Item("Remarks").Value)
            oTempRec.MoveNext()
        Next
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        sform.Items.Item("1000007").DisplayDesc = True
    End Sub

    Private Sub FillPeriod(ByVal aForm As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = aForm.Items.Item("25").Specific
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oTempRec.DoQuery("Select Code,Name from OFPR order by Code desc")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item("Code").Value, oTempRec.Fields.Item("Name").Value)
            oTempRec.MoveNext()
        Next
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        aForm.Items.Item("25").DisplayDesc = True
    End Sub

    Private Sub FillPosition(ByVal sform As SAPbouiCOM.Form)
        oCombobox = sform.Items.Item("1000008").Specific
        oCombobox1 = sform.Items.Item("29").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        For intRow As Integer = oCombobox1.ValidValues.Count - 1 To 0 Step -1
            oCombobox1.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oCombobox1.ValidValues.Add("", "")
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select U_Z_PosCode,U_Z_PosName from [@Z_HR_OPOSIN] order by DocEntry")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            Try
                oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            Catch ex As Exception
            End Try

            oSlpRS.MoveNext()
        Next
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        oCombobox1.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        sform.Items.Item("1000008").DisplayDesc = True
        sform.Items.Item("29").DisplayDesc = True
    End Sub

    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim strDept, strPeriod, posname, strFDate, strTDate, strSLevel As String
            oCombobox = oForm.Items.Item("1000007").Specific
            strDept = oCombobox.Selected.Description
            oCombobox1 = aForm.Items.Item("25").Specific
            oComboLevel = aForm.Items.Item("38").Specific
            strSLevel = oComboLevel.Selected.Value
            strPeriod = oCombobox1.Selected.Value
            strFDate = oForm.Items.Item("34").Specific.Value.ToString()
            strTDate = oForm.Items.Item("36").Specific.Value.ToString()
            If strPeriod = "" Then
                oApplication.Utilities.Message("Enter Period...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Exit Function
            End If
            If Not String.IsNullOrEmpty(strFDate) And Not String.IsNullOrEmpty(strTDate) Then
                Dim intFDate As Integer = Convert.ToInt32(strFDate)
                Dim intTDate As Integer = Convert.ToInt32(strTDate)
                If intFDate > intTDate Then
                    oApplication.Utilities.Message("From Date Should be less than To Date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Exit Function
                End If
            ElseIf Not String.IsNullOrEmpty(strFDate) And String.IsNullOrEmpty(strTDate) Then
                oApplication.Utilities.Message("Enter To Date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Exit Function
            ElseIf String.IsNullOrEmpty(strFDate) And Not String.IsNullOrEmpty(strTDate) Then
                oApplication.Utilities.Message("Enter From Date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Exit Function
            ElseIf String.IsNullOrEmpty(strSLevel) Then
                oApplication.Utilities.Message("Select Appraisal Starting Level...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Exit Function
            End If
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function

    Private Sub Databind(ByVal aform As SAPbouiCOM.Form)
        Dim strFromEMP, strToEMP, strDept, strFromPos, strToPos, strPeriod, strqry As String
        Dim strEMPCondition As String = ""
        Dim strPositionCondition As String = ""
        Dim strdeptcondition As String = ""
        Dim strPeriodcondition As String = ""
        Dim strDatecondition As String = ""
        Dim strFDate As String = ""
        Dim strTDate As String = ""
        Dim strLevelStartFrom As String = ""

        oCombobox = aform.Items.Item("1000007").Specific
        oCombobox1 = aform.Items.Item("1000008").Specific
        oCombobox2 = aform.Items.Item("29").Specific
        oCombobox3 = aform.Items.Item("25").Specific
        strFromEMP = oApplication.Utilities.getEdittextvalue(aform, "20")
        strToEMP = oApplication.Utilities.getEdittextvalue(aform, "23")
        strDept = oCombobox.Selected.Value
        strFromPos = oCombobox1.Selected.Value
        strToPos = oCombobox2.Selected.Value
        strPeriod = oCombobox3.Selected.Value
        strFDate = oForm.Items.Item("34").Specific.Value.ToString()
        strTDate = oForm.Items.Item("36").Specific.Value.ToString()
        Dim intFDate As Integer
        Dim intTDate As Integer
        If strFDate <> "" And strTDate <> "" Then
            intFDate = Convert.ToInt32(strFDate)
            intTDate = Convert.ToInt32(strTDate)
        End If
        Dim oComboLevel As SAPbouiCOM.ComboBox
        oComboLevel = oForm.Items.Item("38").Specific
        strLevelStartFrom = oComboLevel.Selected.Value
        Dim strLevelStartGrid As String
        If strLevelStartFrom = "SA" Then
            strLevelStartGrid = "Self Appraisal"
        Else
            strLevelStartGrid = "Line Manager"
        End If

        If strFromEMP <> "" And strToEMP <> "" Then
            strEMPCondition = " Convert(Decimal,empID) between " & CDbl(strFromEMP) & " and " & CDbl(strToEMP)
        ElseIf strFromEMP <> "" And strToEMP = "" Then
            strEMPCondition = " Convert(Decimal,empID) >= " & CDbl(strFromEMP)
        ElseIf strFromEMP = "" And strToEMP <> "" Then
            strEMPCondition = " Convert(Decimal,empID) <= " & CDbl(strToEMP)
        Else
            strEMPCondition = " 1=1"
        End If

        If strDept <> "" Then
            strdeptcondition = " Convert(Decimal,dept) = " & CDbl(strDept)
        Else
            strdeptcondition = " 1=1"
        End If

        If strPeriod <> "" Then
            strPeriodcondition = " U_Z_Period = '" & strPeriod & "'"
        Else
            strPeriodcondition = " 1=1"
        End If
        If strFDate <> "" And strFDate <> "" Then
            strDatecondition = "Z_Date Between '" & intFDate & "' and '" & intTDate & "'"
        Else
            strDatecondition = "1=1"
        End If


        If strFromPos <> "" And strToPos <> "" Then
            strPositionCondition = " U_Z_HR_PosiCode between '" & strFromPos & "' and '" & strToPos & "'"
        ElseIf strFromPos <> "" And strToPos = "" Then
            strPositionCondition = " U_Z_HR_PosiCode >= '" & strFromPos & "'"
        ElseIf strFromPos = "" And strToPos <> "" Then
            strPositionCondition = " U_Z_HR_PosiCode <= '" & strToPos & "'"
        Else
            strPositionCondition = " 1=1"
        End If


        Dim strcondition, strqry1 As String

        strcondition = strEMPCondition & " and " & strdeptcondition & " and " & strPositionCondition & "  Order by empID"
        Dim strPeriod1 As String
        oCombobox = aform.Items.Item("25").Specific
        strPeriod1 = oCombobox.Selected.Description

        strqry1 = "select  U_Z_EmpId  from [@Z_HR_OSEAPP] where " & strPeriodcondition & " And U_Z_Status <> 'C'"
        strqry = "select 'Y' as 'Select', empID,firstName,lastName,email ,T1.Remarks as 'Department',U_Z_HR_PosiCode, '" & strPeriod1 & "' 'Period',  U_Z_HR_PosiName,'" & strLevelStartGrid & "' as 'Level Start From'  from OHEM T0 Left join OUDP T1 "
        strqry = strqry & " on T0.dept=T1.Code where Active='Y' and  empID not in (" & strqry1 & ") and  " & strcondition

        oGrid = aform.Items.Item("10").Specific
        oGrid.DataTable.ExecuteQuery(strqry)
        Try
            aform.Freeze(True)
            FormatGrid(aform)
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub

    Private Sub FormatGrid(ByVal aForm As SAPbouiCOM.Form)
        oGrid = aForm.Items.Item("10").Specific
        oGrid.Columns.Item("Select").TitleObject.Caption = "Select"
        oGrid.Columns.Item("Select").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        oGrid.Columns.Item("Select").Editable = True
        oGrid.Columns.Item("empID").TitleObject.Caption = "Employee Id"
        oEditTextColumn = oGrid.Columns.Item("empID")
        oEditTextColumn.LinkedObjectType = "171"
        oGrid.Columns.Item("empID").Editable = False
        oGrid.Columns.Item("firstName").TitleObject.Caption = "First Name"
        oGrid.Columns.Item("firstName").Editable = False
        oGrid.Columns.Item("lastName").TitleObject.Caption = "Last Name"
        oGrid.Columns.Item("lastName").Editable = False
        oGrid.Columns.Item("email").TitleObject.Caption = "Email Id"
        oGrid.Columns.Item("email").Editable = False
        oGrid.Columns.Item("Department").TitleObject.Caption = "Department"
        oGrid.Columns.Item("Department").Editable = False
        oGrid.Columns.Item("U_Z_HR_PosiCode").TitleObject.Caption = "Position Code"
        oGrid.Columns.Item("U_Z_HR_PosiCode").Editable = False
        oGrid.Columns.Item("U_Z_HR_PosiName").TitleObject.Caption = "Position Name"
        oGrid.Columns.Item("U_Z_HR_PosiName").Editable = False
        oGrid.Columns.Item("Level Start From").Editable = False
        oGrid.Columns.Item("Period").TitleObject.Caption = "Period"
        oGrid.Columns.Item("Period").Editable = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub

    Private Function InitializeApprisal(ByVal aform As SAPbouiCOM.Form, ByVal strPeriod As String, ByVal strdepart As String, ByVal FDate As String, ByVal TDate As String, ByVal LStart As String) As Boolean
        Try
            aform.Freeze(True)
            sPath = System.Windows.Forms.Application.StartupPath & "\ApprisalLog.txt"
            If File.Exists(sPath) Then
                File.Delete(sPath)
            End If

            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If
            oApplication.Company.StartTransaction()
            Dim oUserTable As SAPbobsCOM.UserTable
            Dim strCode, strECode, strESocial, strEname, strETax, strGLAcc, strDept As String
            oGrid = aform.Items.Item("10").Specific
            Dim oGeneralService, oGeneralService1 As SAPbobsCOM.GeneralService
            Dim oGeneralData, oGeneralData1 As SAPbobsCOM.GeneralData
            Dim oGeneralParams, oGeneralParams1 As SAPbobsCOM.GeneralDataParams
            Dim oCompanyService As SAPbobsCOM.CompanyService
            Dim oChildren, oChildren1, oChildren2 As SAPbobsCOM.GeneralDataCollection
            oCompanyService = oApplication.Company.GetCompanyService()
            Dim otestRs, oRec As SAPbobsCOM.Recordset
            Dim oChild, oChild1, oChild2, oChild3 As SAPbobsCOM.GeneralData
            Dim blnRecordExists As Boolean = False
            'Get GeneralService (oCmpSrv is the CompanyService)
            oGeneralService = oCompanyService.GetGeneralService("Z_HR_OSELAPP")
            ' oGeneralService1 = oCompanyService.GetGeneralService("Z_CONTRACT")
            'oChildren = oGeneralData.Child("DAILY_FACTS_DETAILS")
            'Create data for new row in main UDO
            'strCode = aCode ' oGrid.DataTable.GetValue("U_Z_Code", 0)
            otestRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)
            ' oGeneralParams1 = oGeneralService1.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)

            Dim oCheckbox, ocheckbox1 As SAPbouiCOM.CheckBoxColumn
            Dim blnDownpayment As Boolean = False
            Dim blnDocumentItem As Boolean
            Dim ReStdate, reEndDate As Date
            oApplication.Utilities.WriteErrorlog("Appraisal Process Started : ", sPath)
            intNumofCount = 0
            oDtAppraisal = oForm.DataSources.DataTables.Item("dtAppraisal")
            oDtAppraisal.Rows.Clear()
            '  oDtAppraisal.Rows.Add(oGrid.DataTable.Rows.Count)
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                blnDocumentItem = False
                oCheckbox = oGrid.Columns.Item("Select")
                oGeneralData1 = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData)
                If oCheckbox.IsChecked(intRow) Then
                    Dim strempid, strempname As String
                    strempid = oGrid.DataTable.GetValue("empID", intRow)
                    strempname = oGrid.DataTable.GetValue("firstName", intRow)
                    strDept = oApplication.Utilities.getDeptID(oGrid.DataTable.GetValue("Department", intRow))
                    oGeneralData1.SetProperty("U_Z_Status", "D")
                    oGeneralData1.SetProperty("U_Z_EmpId", strempid)
                    oGeneralData1.SetProperty("U_Z_EmpName", strempname)
                    oGeneralData1.SetProperty("U_Z_Period", strPeriod)
                    oGeneralData1.SetProperty("U_Z_Date", Now.Date)
                    If FDate <> "" And TDate <> "" Then
                        Dim dtFromDt As DateTime = Convert.ToDateTime(FDate.Substring(0, 4) & "-" & FDate.Substring(4, 2) & "-" & FDate.Substring(6, 2))
                        Dim dtToDt As DateTime = Convert.ToDateTime(TDate.Substring(0, 4) & "-" & TDate.Substring(4, 2) & "-" & TDate.Substring(6, 2))
                        oGeneralData1.SetProperty("U_Z_FDate", dtFromDt)
                        oGeneralData1.SetProperty("U_Z_TDate", dtToDt)
                    End If
                    oGeneralData1.SetProperty("U_Z_LStrt", LStart)
                    oGeneralData1.SetProperty("U_Z_WStatus", "DR")
                    oGeneralData1.SetProperty("U_Z_Initialize", "N")
                    oChildren1 = oGeneralData1.Child("Z_HR_SEAPP1")
                    If strDept <> "" Then
                        otestRs.DoQuery("SELECT T1.[U_Z_BussCode], T1.[U_Z_BussName], T1.[U_Z_Weight] FROM [dbo].[@Z_HR_ODEMA]  T0  inner Join  [dbo].[@Z_HR_DEMA1]  T1 on T1.DocEntry=T0.DocEntry and T0.U_Z_DeptCode=" & strDept & "")
                        For inlloop As Integer = 0 To otestRs.RecordCount - 1
                            oChild = oChildren1.Add()
                            oChild.SetProperty("U_Z_BussCode", otestRs.Fields.Item("U_Z_BussCode").Value)
                            oChild.SetProperty("U_Z_BussDesc", otestRs.Fields.Item("U_Z_BussName").Value)
                            oChild.SetProperty("U_Z_BussWeight", otestRs.Fields.Item("U_Z_Weight").Value)
                            otestRs.MoveNext()
                        Next
                    End If
                    oChildren2 = oGeneralData1.Child("Z_HR_SEAPP2")
                    otestRs.DoQuery("SELECT T0.[U_Z_HREmpID], T0.[U_Z_HRPeoobjCode], T0.[U_Z_HRPeoobjName], T0.[U_Z_HRPeoCategory], T0.[U_Z_HRWeight] FROM [dbo].[@Z_HR_PEOBJ1]  T0 where T0.U_Z_HREmpID=" & strempid & "")
                    For inlloop As Integer = 0 To otestRs.RecordCount - 1
                        oChild1 = oChildren2.Add()
                        oChild1.SetProperty("U_Z_PeopleCode", otestRs.Fields.Item("U_Z_HRPeoobjCode").Value)
                        oChild1.SetProperty("U_Z_PeopleDesc", otestRs.Fields.Item("U_Z_HRPeoobjName").Value)
                        oChild1.SetProperty("U_Z_PeopleCat", otestRs.Fields.Item("U_Z_HRPeoCategory").Value)
                        oChild1.SetProperty("U_Z_PeoWeight", otestRs.Fields.Item("U_Z_HRWeight").Value)
                        otestRs.MoveNext()
                    Next
                    Dim intJobCode, strqry As String
                    oRec.DoQuery("select U_Z_HR_JobstCode  from  OHEM  where empid=" & CInt(strempid))
                    If oRec.RecordCount > 0 Then
                        intJobCode = oRec.Fields.Item("U_Z_HR_JobstCode").Value
                        oChildren = oGeneralData1.Child("Z_HR_SEAPP3")
                        strqry = "SELECT T1.[U_Z_CompCode], T1.[U_Z_CompDesc], T1.[U_Z_Weight],T1.[U_Z_CompLevel] FROM [dbo].[@Z_HR_OPOSCO]  T0  inner Join  [dbo].[@Z_HR_POSCO1]  T1 on T1.DocEntry=T0.DocEntry and T0.U_Z_PosCode='" & intJobCode & "'"
                        otestRs.DoQuery(strqry)
                        For inlloop As Integer = 0 To otestRs.RecordCount - 1
                            oChild2 = oChildren.Add()
                            oChild2.SetProperty("U_Z_CompCode", otestRs.Fields.Item("U_Z_CompCode").Value)
                            oChild2.SetProperty("U_Z_CompDesc", otestRs.Fields.Item("U_Z_CompDesc").Value)
                            oChild2.SetProperty("U_Z_CompWeight", otestRs.Fields.Item("U_Z_Weight").Value)
                            oChild2.SetProperty("U_Z_CompLevel", otestRs.Fields.Item("U_Z_CompLevel").Value)
                            otestRs.MoveNext()
                        Next
                    End If

                    oChildren = oGeneralData1.Child("Z_HR_SEAPP4")
                    oChild3 = oChildren.Add()
                    oChild3.SetProperty("U_Z_CompType", "Business")
                    oChild3 = oChildren.Add()
                    oChild3.SetProperty("U_Z_CompType", "People")
                    oChild3 = oChildren.Add()
                    oChild3.SetProperty("U_Z_CompType", "Competency")

                    oGeneralParams = oGeneralService.Add(oGeneralData1)

                    Dim strDocEntry As String = oGeneralParams.GetProperty("DocEntry")
                    oDtAppraisal.Rows.Add()
                    oDtAppraisal.SetValue("Select", oDtAppraisal.Rows.Count - 1, "Y")
                    oDtAppraisal.SetValue("DocEntry", oDtAppraisal.Rows.Count - 1, strDocEntry)
                    oDtAppraisal.SetValue("Name", oDtAppraisal.Rows.Count - 1, strempname)
                    oDtAppraisal.SetValue("EmpID", oDtAppraisal.Rows.Count - 1, strempid)
                    intNumofCount = intNumofCount + 1
                End If
            Next

            oApplication.Utilities.WriteErrorlog("Initialize Appraisal Process Completed...", sPath)
            oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                For index As Integer = 0 To oDtAppraisal.Rows.Count - 1
                    oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    sQuery = "Select T1.EmpID,T0.Email,T1.Email From OHEM T0 JOIN OHEM T1  ON T0.Manager = T1.EmpID JOIN [@Z_HR_OSEAPP] T2 ON T0.EmpID = T2.U_Z_EmpId Where T2.DocEntry = '" & oDtAppraisal.GetValue("DocEntry", index) & "'"
                    oRecordSet.DoQuery(sQuery)
                    If Not oRecordSet.EoF Then
                        ' oDtAppraisal.SetValue("EmpID", index, oRecordSet.Fields.Item(0).Value)
                        oDtAppraisal.SetValue("ccID", index, oRecordSet.Fields.Item(1).Value)
                        oDtAppraisal.SetValue("toID", index, oRecordSet.Fields.Item(2).Value)
                        oDtAppraisal.SetValue("Type", index, "AI")
                    End If
                Next
                BindAppraisalData(oForm)
            End If
            aform.Freeze(False)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            End If

            aform.Freeze(False)
            Return False
        End Try
    End Function

    Private Sub InitializeAppTable()
        oForm.DataSources.DataTables.Add("dtAppraisal")
        oDtAppraisal = oForm.DataSources.DataTables.Item("dtAppraisal")
        oDtAppraisal.Columns.Add("Select", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
        oDtAppraisal.Columns.Add("DocEntry", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
        oDtAppraisal.Columns.Add("EmpID", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
        oDtAppraisal.Columns.Add("Name", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
        oDtAppraisal.Columns.Add("toID", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
        oDtAppraisal.Columns.Add("ccID", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
        oDtAppraisal.Columns.Add("Type", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
        oDtAppraisal.Columns.Add("Path", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
    End Sub

    Private Sub InitializeMailTable()
        oForm.DataSources.DataTables.Add("dtAppraisal1")
        oDtAppraisal1 = oForm.DataSources.DataTables.Item("dtAppraisal1")
        oDtAppraisal1.Columns.Add("Select", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
        oDtAppraisal1.Columns.Add("DocEntry", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
        oDtAppraisal1.Columns.Add("EmpID", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
        oDtAppraisal1.Columns.Add("Name", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
        oDtAppraisal1.Columns.Add("toID", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
        oDtAppraisal1.Columns.Add("ccID", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
        oDtAppraisal1.Columns.Add("Type", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
        oDtAppraisal1.Columns.Add("Path", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric)
    End Sub

    Private Sub BindAppraisalData(ByVal aform As SAPbouiCOM.Form)
        oGrid = aform.Items.Item("39").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("dtAppraisal")
        oGrid.Columns.Item("Select").TitleObject.Caption = "Select"
        oGrid.Columns.Item("Select").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        oGrid.Columns.Item("Select").Editable = True
        oGrid.Columns.Item("EmpID").TitleObject.Caption = "Employee ID"
        oEditTextColumn = oGrid.Columns.Item("EmpID")
        oEditTextColumn.LinkedObjectType = "171"
        oGrid.Columns.Item("EmpID").Editable = False
        oGrid.Columns.Item("Name").TitleObject.Caption = "Employee Name"
        oEditTextColumn = oGrid.Columns.Item("Name")
        oGrid.Columns.Item("Name").Editable = False
        oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Appraisal ID"
        oGrid.Columns.Item("DocEntry").Editable = False
        oGrid.Columns.Item("toID").TitleObject.Caption = "Email ID"
        oGrid.Columns.Item("toID").Editable = False
        oGrid.Columns.Item("ccID").TitleObject.Caption = "Manager Mail ID"
        oGrid.Columns.Item("ccID").Editable = False
        oGrid.Columns.Item("Type").Visible = False
        oGrid.Columns.Item("Path").Visible = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub

    Private Sub updateTimeStamp(ByVal aform As SAPbouiCOM.Form)
        oGrid = aform.Items.Item("39").Specific
        For index As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            oApplication.Utilities.UpdateTimeStamp(oGrid.DataTable.GetValue("DocEntry", index), "IN")
        Next
    End Sub

    Private Sub SendMail(ByVal aform As SAPbouiCOM.Form)
        oGrid = aform.Items.Item("39").Specific
        oDtAppraisal1 = oForm.DataSources.DataTables.Item("dtAppraisal1") 'oDtAppraisal1.Rows.Clear()
        For index As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.DataTable.GetValue("Select", index) = "Y" Then
                oDtAppraisal1.Rows.Add(1)
                oDtAppraisal1.SetValue("DocEntry", oDtAppraisal1.Rows.Count - 1, oGrid.DataTable.GetValue("DocEntry", index))
                oDtAppraisal1.SetValue("EmpID", oDtAppraisal1.Rows.Count - 1, oGrid.DataTable.GetValue("EmpID", index))
                oDtAppraisal1.SetValue("Name", oDtAppraisal1.Rows.Count - 1, oGrid.DataTable.GetValue("Name", index))
                oDtAppraisal1.SetValue("toID", oDtAppraisal1.Rows.Count - 1, oGrid.DataTable.GetValue("toID", index))
                oDtAppraisal1.SetValue("ccID", oDtAppraisal1.Rows.Count - 1, oGrid.DataTable.GetValue("ccID", index))
                oDtAppraisal1.SetValue("Type", oDtAppraisal1.Rows.Count - 1, "AI")
            End If
        Next
        If Not IsNothing(oDtAppraisal1) And oDtAppraisal1.Rows.Count > 0 Then
            oApplication.SBO_Application.StatusBar.SetText("Generating Report Started....", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            oApplication.Utilities.generateReport(oDtAppraisal1)
            oApplication.SBO_Application.StatusBar.SetText("Process Sending Mail....", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            oApplication.Utilities.SendMail(oDtAppraisal1, "Appraisal")
            oApplication.SBO_Application.StatusBar.SetText("Mail Process Completed Sucessfully....", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success)
        End If
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_IniAppraisal Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" And (oForm.PaneLevel = 2 Or oForm.PaneLevel = 3) Then
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
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "3"
                                        oForm.PaneLevel = oForm.PaneLevel + 1
                                        If oForm.PaneLevel = 3 Then
                                            Databind(oForm)
                                        ElseIf oForm.PaneLevel = 4 Then
                                            BindAppraisalData(oForm)
                                        End If
                                        Dim osta As SAPbouiCOM.StaticText
                                        osta = oForm.Items.Item("19").Specific
                                        osta.Caption = "Step " & oForm.PaneLevel & " of 4"
                                        oForm.Items.Item("19").TextStyle = SAPbouiCOM.BoTextStyle.ts_BOLD
                                    Case "4"
                                        oForm.PaneLevel = oForm.PaneLevel - 1
                                        Dim osta As SAPbouiCOM.StaticText
                                        osta = oForm.Items.Item("19").Specific
                                        osta.Caption = "Step " & oForm.PaneLevel & " of 4"
                                        oForm.Items.Item("19").TextStyle = SAPbouiCOM.BoTextStyle.ts_BOLD
                                    Case "5"
                                        If oApplication.SBO_Application.MessageBox("Do you want confirm the Employee Appraisal Initialization", , "Yes", "No") = 2 Then
                                            Exit Sub
                                        Else
                                            Dim blnStatus As Boolean = False
                                            Dim strPeriod As String
                                            Dim strDept As String
                                            Dim strFDate, strTDate, strLStart As String
                                            oCombobox1 = oForm.Items.Item("1000007").Specific
                                            strDept = oCombobox.Selected.Value
                                            oCombobox = oForm.Items.Item("25").Specific
                                            strPeriod = oCombobox.Selected.Value
                                            strFDate = oForm.Items.Item("34").Specific.Value.ToString()
                                            strTDate = oForm.Items.Item("36").Specific.Value.ToString()
                                            Dim oComboLevel As SAPbouiCOM.ComboBox
                                            oComboLevel = oForm.Items.Item("38").Specific
                                            strLStart = oComboLevel.Selected.Value
                                            If InitializeApprisal(oForm, strPeriod, strDept, strFDate, strTDate, strLStart) = True Then ' createDocuments(oForm, intChoice) = True Then
                                                oApplication.Utilities.Message("Operation Completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                                oApplication.SBO_Application.MessageBox("Operation Completed successfully")
                                                Dim ostatic As SAPbouiCOM.StaticText
                                                ostatic = oForm.Items.Item("30").Specific
                                                ostatic.Caption = "The appraisal was successfully inisialized..."
                                                oForm.Items.Item("30").TextStyle = SAPbouiCOM.BoTextStyle.ts_BOLD
                                                ostatic = oForm.Items.Item("31").Specific
                                                ostatic.Caption = "Perid : " & strPeriod
                                                oForm.Items.Item("31").TextStyle = SAPbouiCOM.BoTextStyle.ts_BOLD
                                                ostatic = oForm.Items.Item("32").Specific
                                                ostatic.Caption = "Number of Employee : " & intNumofCount
                                                oForm.Items.Item("32").TextStyle = SAPbouiCOM.BoTextStyle.ts_BOLD
                                                Dim osta As SAPbouiCOM.StaticText
                                                osta = oForm.Items.Item("19").Specific
                                                oForm.Items.Item("19").TextStyle = SAPbouiCOM.BoTextStyle.ts_BOLD
                                                oForm.PaneLevel = 4
                                                osta.Caption = "Step " & oForm.PaneLevel & " of 4"
                                                'If oApplication.Company.InTransaction() Then
                                                '    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                                                'End If
                                                'Dim x As System.Diagnostics.ProcessStartInfo
                                                'x = New System.Diagnostics.ProcessStartInfo
                                                'x.UseShellExecute = True
                                                'sPath = System.Windows.Forms.Application.StartupPath & "\ApprisalLog.txt"
                                                'x.FileName = sPath
                                                'System.Diagnostics.Process.Start(x)
                                                'x = Nothing
                                                ' oForm.Close()
                                                updateTimeStamp(oForm)
                                            Else
                                                If oApplication.Company.InTransaction() Then
                                                    oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                                                End If
                                                oApplication.SBO_Application.MessageBox("Renewal process encounterd with some errors,")
                                                Dim x As System.Diagnostics.ProcessStartInfo
                                                x = New System.Diagnostics.ProcessStartInfo
                                                x.UseShellExecute = True
                                                sPath = System.Windows.Forms.Application.StartupPath & "\ApprisalLog.txt"
                                                x.FileName = sPath
                                                System.Diagnostics.Process.Start(x)
                                                x = Nothing
                                            End If
                                            oApplication.Utilities.Message("Employee Appraisal Initialize successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                        End If
                                    Case "40"
                                        If oApplication.Utilities.checkmailconfiguration() = False Then
                                            oApplication.Utilities.Message("Email configuration not availble...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            Exit Sub
                                        End If
                                        SendMail(oForm)
                                        oApplication.Utilities.Message("Email sent successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                        oForm.Close()
                                        oForm.PaneLevel = 5
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

                                        If pVal.ItemUID = "20" Then
                                            val1 = oDataTable.GetValue("empID", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "20", val1)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "23" Then
                                            val1 = oDataTable.GetValue("empID", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "23", val1)
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
                Case mnu_hr_IniAppraisal
                    LoadForm()
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
