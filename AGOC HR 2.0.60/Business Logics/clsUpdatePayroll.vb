Public Class clsUpdatePayroll
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oCheckBoxColumn As SAPbouiCOM.CheckBoxColumn
    Private osta As SAPbouiCOM.StaticText
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
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_HR_UpdatePayroll) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_HR_UpdatePayroll, frm_HR_UpdatePayroll)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        FillDepartment(oForm)
        AddChooseFromList(oForm)
        oForm.DataSources.UserDataSources.Add("frmEmp", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("toEmp", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "9", "frmEmp")
        oApplication.Utilities.setUserDatabind(oForm, "11", "toEmp")
        oEditText = oForm.Items.Item("9").Specific
        oEditText.ChooseFromListUID = "CFL1"
        oEditText.ChooseFromListAlias = "empId"

        oEditText = oForm.Items.Item("11").Specific
        oEditText.ChooseFromListUID = "CFL2"
        oEditText.ChooseFromListAlias = "empId"
        oForm.PaneLevel = 1

        osta = oForm.Items.Item("15").Specific
        osta.Caption = "Step " & oForm.PaneLevel & " of 3"
        oForm.Items.Item("15").TextStyle = SAPbouiCOM.BoTextStyle.ts_BOLD
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
        oCombobox = sform.Items.Item("7").Specific
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
        sform.Items.Item("7").DisplayDesc = True
    End Sub

    Private Sub PopulateDetails(ByVal aForm As SAPbouiCOM.Form)
        aForm.Freeze(True)
        oGrid = aForm.Items.Item("12").Specific
        Dim strFromEmp, strToEmp, strDept, strqry, strqry1, strcondition As String
        strFromEmp = oApplication.Utilities.getEdittextvalue(aForm, "9")
        strToEmp = oApplication.Utilities.getEdittextvalue(aForm, "11")
        oCombobox = aForm.Items.Item("7").Specific
        strDept = oCombobox.Selected.Value
        If strFromEmp = "" Then
            strcondition = " 1=1"
        Else
            strcondition = " T0.empID >=" & CInt(strFromEmp)
        End If

        If strToEmp = "" Then
            strcondition = strcondition & " and 1=1"
        Else
            strcondition = strcondition & " and T0.empID <=" & CInt(strToEmp)
        End If
        If strDept <> "" Then
            strcondition = strcondition & " and T0.dept=" & strDept
        End If
        ' strqry = "select  'Y' as 'Select', T4.Code 'Code', empID,firstName 'First Name',lastName 'Last Name',email 'Email' ,T1.Remarks as 'Department',T2.Name as 'Position' ,T2.posID 'Position Code'  from OHEM T0  inner Join  [@Z_HR_HEM2]  T4 on T4.U_Z_EmpId=T0.empID and T4.Code=T0.U_Z_EmpLifRef  inner join OUDP T1 "
        '   strqry = strqry & " on T1.Code=T0.dept   INNER JOIN OHPS T2 ON T2.PosID = T0.position where isnull(T4.U_Z_UpdatePayroll,'N')='N' and  isnull(T4.U_Z_Posting,'N')='Y' and  T0.U_Z_EmpLicyStatus='P' and  Active='Y' and " & strcondition & " order by T0.empID"



        strqry = "select  'Y' as 'Select', T4.Code 'Code', empID,firstName 'First Name',lastName 'Last Name',email 'Email' ,T1.Remarks as 'Department',T2.Name as 'Position' ,T2.posID 'Position Code',T4.U_Z_IncAmount   from OHEM T0  inner Join  [@Z_HR_HEM2]  T4 on T4.U_Z_EmpId=T0.empID   inner join OUDP T1 "
        strqry = strqry & " on T1.Code=T0.dept   INNER JOIN OHPS T2 ON T2.PosID = T0.position where isnull(T4.U_Z_UpdatePayroll,'N')='N' and  isnull(T4.U_Z_Posting,'N')='Y' and  T0.U_Z_EmpLicyStatus='P' and  Active='Y' and " & strcondition & " order by T0.empID"

        '  strQuery = "select  from [@Z_HR_HEM2] T0 left Join OHEM T1 on T0.U_Z_EmpId=T1.empID where T0.Code=T1.U_Z_EmpLifRef and  T0.U_Z_EmpId='" & empID & "' and isnull(T0.U_Z_Posting,'N')='Y' and T1.U_Z_EmpLiCyStatus='P' order by Code desc"
        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("Code").TitleObject.Caption = "Promotion RefCode"
        oGrid.Columns.Item("Code").Editable = False
        oGrid.Columns.Item("Select").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        oGrid.Columns.Item("Select").Editable = True
        oGrid.Columns.Item("empID").TitleObject.Caption = "Employee ID"
        oGrid.Columns.Item("empID").Editable = False
        oEditTextColumn = oGrid.Columns.Item("empID")
        oEditTextColumn.LinkedObjectType = "171"
        oGrid.Columns.Item("First Name").Editable = False
        oGrid.Columns.Item("Last Name").Editable = False
        oGrid.Columns.Item("Email").Editable = False
        oGrid.Columns.Item("Department").Editable = False
        oGrid.Columns.Item("Position").Editable = False
        oGrid.Columns.Item("Position Code").Visible = False
        oGrid.Columns.Item("U_Z_IncAmount").TitleObject.Caption = "Increment Amount"
        oGrid.Columns.Item("U_Z_IncAmount").Editable = False
        oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
        oGrid.AutoResizeColumns()
        aForm.Freeze(False)
    End Sub

    Private Function AddToUDT(ByVal empID As String, ByVal aRefCode As String) As Boolean
        Dim strTable, strEmpId, strCode, strType, strPosId, strJobId, strSalaryScale As String
        Dim dblValue As Double
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim oValidateRS, oTempRS, oTempRS1 As SAPbobsCOM.Recordset
        Dim oComboColumn As SAPbouiCOM.ComboBoxColumn
        strEmpId = empID
        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTempRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTempRS1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oUserTable = oApplication.Company.UserTables.Item("Z_PAY1")
        Try
            Dim strPOSCode, strJOBCode, strSalCode, strQuery, strQuery1 As String
            strQuery = "select  position ,T1.name,* from OHEM T0  inner join OHPS  T1 on T0.position=T1.posid where T0.empID=" & empID
            oTempRS.DoQuery(strQuery)
            If oTempRS.RecordCount > 0 Then
                strPOSCode = oTempRS.Fields.Item(1).Value
                strQuery = "select U_Z_JobCode,T0. U_Z_Poscode, * from [@Z_HR_OPOSIN]  T0 inner Join  [@Z_HR_OPOSCO] T1 on T1.U_Z_PosCode =T0.U_Z_JobCode  where T0.U_Z_PosCode='" & strPOSCode & "'"
                oTempRS.DoQuery(strQuery)
                If oTempRS.RecordCount > 0 Then
                    strJOBCode = oTempRS.Fields.Item("U_Z_JobCode").Value
                    strQuery = "select T1.DocEntry, T0.U_Z_SalCode , * from [@Z_HR_OPOSCO] T0 inner Join [@Z_HR_OSALST] T1 on T1.U_Z_SalCode =T0.U_Z_SalCode   where U_Z_PosCode='" & strJOBCode & "'"
                    oTempRS.DoQuery(strQuery)
                    If oTempRS.RecordCount > 0 Then
                        strSalaryScale = oTempRS.Fields.Item(0).Value
                        strSalCode = oTempRS.Fields.Item(1).Value
                        oValidateRS.DoQuery("Select * from [@Z_HR_SALST1] where DocEntry=" & strSalaryScale)
                        For intRow As Integer = 0 To oValidateRS.RecordCount - 1
                            strQuery1 = "Select * from [@Z_PAY1] where U_Z_EARN_TYPE='" & oValidateRS.Fields.Item("U_Z_AllCode").Value & "' and  U_Z_SalCode='" & strSalCode & "' and U_Z_EMPID='" & empID & "'"
                            oTempRS.DoQuery(strQuery1)
                            If oTempRS.RecordCount <= 0 Then
                                strCode = oApplication.Utilities.getMaxCode("@Z_PAY1", "Code")
                                oUserTable.Code = strCode
                                oUserTable.Name = strCode + "N"
                                oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = empID
                                oUserTable.UserFields.Fields.Item("U_Z_EARN_TYPE").Value = oValidateRS.Fields.Item("U_Z_AllCode").Value
                                oUserTable.UserFields.Fields.Item("U_Z_EARN_VALUE").Value = oValidateRS.Fields.Item("U_Z_Amount").Value
                                oUserTable.UserFields.Fields.Item("U_Z_Percentage").Value = oValidateRS.Fields.Item("U_Z_BasicPer").Value
                                oUserTable.UserFields.Fields.Item("U_Z_SalCode").Value = strSalCode
                                oTempRS1.DoQuery("Select * from [@Z_PAY_OEAR] where U_Z_CODE='" & oValidateRS.Fields.Item("U_Z_AllCode").Value & "'")
                                oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = oTempRS1.Fields.Item("U_Z_EAR_GLACC").Value
                                If oUserTable.Add <> 0 Then
                                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    Return False
                                End If
                            Else
                                strCode = oTempRS.Fields.Item("Code").Value
                                oUserTable.GetByKey(strCode)
                                oUserTable.Code = strCode
                                oUserTable.Name = strCode + "N"
                                oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = empID
                                oUserTable.UserFields.Fields.Item("U_Z_EARN_TYPE").Value = oValidateRS.Fields.Item("U_Z_AllCode").Value
                                ' oUserTable.UserFields.Fields.Item("U_Z_EARN_VALUE").Value = oValidateRS.Fields.Item("U_Z_Amount").Value
                                ' oUserTable.UserFields.Fields.Item("U_Z_Percentage").Value = oValidateRS.Fields.Item("U_Z_BasicPer").Value
                                oUserTable.UserFields.Fields.Item("U_Z_SalCode").Value = strSalCode
                                oTempRS1.DoQuery("Select * from [@Z_PAY_OEAR] where U_Z_CODE='" & oValidateRS.Fields.Item("U_Z_AllCode").Value & "'")
                                If oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = "" Then
                                    oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = oTempRS1.Fields.Item("U_Z_EAR_GLACC").Value
                                End If

                                If oUserTable.Update <> 0 Then
                                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    Return False
                                End If
                            End If
                            oValidateRS.MoveNext()
                        Next
                    End If

                    'Contribution
                    'oUserTable = oApplication.Company.UserTables.Item("Z_PAY3")
                    'strQuery = "select T1.DocEntry, T0.U_Z_SalCode , * from [@Z_HR_OPOSCO] T0 inner Join [@Z_HR_OSALST] T1 on T1.U_Z_SalCode =T0.U_Z_SalCode   where U_Z_PosCode='" & strJOBCode & "'"
                    'oTempRS.DoQuery(strQuery)
                    'If oTempRS.RecordCount > 0 Then
                    '    strSalaryScale = oTempRS.Fields.Item(0).Value
                    '    strSalCode = oTempRS.Fields.Item(1).Value
                    '    oValidateRS.DoQuery("Select * from [@Z_HR_SALST2] where DocEntry=" & strSalaryScale)
                    '    For intRow As Integer = 0 To oValidateRS.RecordCount - 1
                    '        oTempRS.DoQuery("Select * from [@Z_PAY3] where U_Z_CONTR_TYPE='" & oValidateRS.Fields.Item("U_Z_BeneCode").Value & "' and  U_Z_SalCode='" & strSalCode & "' and U_Z_EmpID='" & empID & "'")
                    '        If oTempRS.RecordCount <= 0 Then
                    '            strCode = oApplication.Utilities.getMaxCode("@Z_PAY3", "Code")
                    '            oUserTable.Code = strCode
                    '            oUserTable.Name = strCode + "N"
                    '            oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = empID
                    '            oUserTable.UserFields.Fields.Item("U_Z_CONTR_TYPE").Value = oValidateRS.Fields.Item("U_Z_BeneCode").Value
                    '            'oUserTable.UserFields.Fields.Item("U_Z_CONTR_VALUE").Value = oValidateRS.Fields.Item("U_Z_Amount").Value
                    '            'oUserTable.UserFields.Fields.Item("U_Z_Percentage").Value = oValidateRS.Fields.Item("U_Z_BasicPer").Value
                    '            oTempRS1.DoQuery("Select * from [@Z_PAY_OCON] where Code='" & oValidateRS.Fields.Item("U_Z_BeneCode").Value & "'")
                    '            oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = oTempRS1.Fields.Item("U_Z_CON_GLACC").Value

                    '            oUserTable.UserFields.Fields.Item("U_Z_SalCode").Value = strSalCode
                    '            If oUserTable.Add <> 0 Then
                    '                oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    '                Return False
                    '            End If
                    '        Else
                    '            strCode = oTempRS.Fields.Item("Code").Value
                    '            oUserTable.GetByKey(strCode)
                    '            oUserTable.Code = strCode
                    '            oUserTable.Name = strCode + "N"
                    '            oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = empID
                    '            oUserTable.UserFields.Fields.Item("U_Z_CONTR_TYPE").Value = oValidateRS.Fields.Item("U_Z_BeneCode").Value
                    '            oTempRS1.DoQuery("Select * from [@Z_PAY_OCON] where Code='" & oValidateRS.Fields.Item("U_Z_BeneCode").Value & "'")
                    '            If oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = "" Then
                    '                oUserTable.UserFields.Fields.Item("U_Z_GLACC").Value = oTempRS1.Fields.Item("U_Z_CON_GLACC").Value
                    '            End If

                    '            If oUserTable.Update <> 0 Then
                    '                oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    '                Return False
                    '            End If
                    '        End If
                    '        oValidateRS.MoveNext()
                    '    Next
                    'End If

                    'Salary Updation
                    oUserTable = oApplication.Company.UserTables.Item("Z_PAY11")
                    Dim dblInc As Double
                    strQuery = "select Top 1 T0.U_Z_EffFromdt as U_Z_EffFromdt,T0.U_Z_IncAmount,T0.U_Z_EffTodt as U_Z_EffTodt from [@Z_HR_HEM2] T0 left Join OHEM T1 on T0.U_Z_EmpId=T1.empID where  T0.U_Z_EmpId='" & empID & "' and isnull(T0.U_Z_Posting,'N')='Y' and T1.U_Z_EmpLiCyStatus='P'  and T0.Code='" & aRefCode & "' order by Code desc"
                    oValidateRS.DoQuery(strQuery)
                    If oValidateRS.RecordCount > 0 Then
                        Dim dblIncrAMount As Double = oValidateRS.Fields.Item("U_Z_IncAmount").Value
                        If dblIncrAMount <= 0 Then
                            oTempRS1.DoQuery("Update [@Z_HR_HEM2] set U_Z_UpdatePayroll='Y' where Code='" & aRefCode & "'")
                        Else
                            Dim dtFromDate, dtToDate As Date
                            dtFromDate = oValidateRS.Fields.Item("U_Z_EffFromdt").Value
                            dtToDate = oValidateRS.Fields.Item("U_Z_EffTodt").Value
                            If dtToDate.Year = 1899 Then
                                dtToDate = dtFromDate
                            End If
                            '  strQuery1 = "Select * from [@Z_PAY11] where U_Z_StartDate='" & oValidateRS.Fields.Item("U_Z_EffFromdt").Value & "' and  U_Z_EndDate='" & oValidateRS.Fields.Item("U_Z_EffTodt").Value & "' and U_Z_EmpID='" & empID & "'"
                            strQuery1 = "Select * from [@Z_PAY11] where (('" & dtFromDate.ToString("yyyy-MM-dd") & "' between  U_Z_StartDate and  isnull(U_Z_EndDate,'" & dtFromDate.ToString("yyyy-MM-dd") & "')) or  ('" & dtToDate.ToString("yyyy-MM-dd") & "'   between  U_Z_StartDate and  isnull(U_Z_EndDate,'" & dtToDate.ToString("yyyy-MM-dd") & "')))  and U_Z_EndDate is not null and U_Z_EmpID='" & empID & "'"
                            oTempRS1.DoQuery(strQuery1)
                            If oTempRS1.RecordCount <= 0 Then
                                strQuery1 = "Select * from [@Z_PAY11] where  U_Z_EmpID='" & empID & "' order by Convert(numeric,Code) Desc"
                                oTempRS.DoQuery(strQuery1)
                                Dim stCode As String
                                If oTempRS.RecordCount > 0 Then
                                    stCode = oTempRS.Fields.Item("Code").Value
                                    dtToDate = oTempRS.Fields.Item("U_Z_EndDate").Value
                                    If dtToDate.Year = 1899 Then
                                        dtToDate = dtFromDate.AddDays(-1)
                                        oTempRS.DoQuery("Update [@Z_PAY11] set U_Z_EndDate='" & dtToDate.ToString("yyyy-MM-dd") & "' where Code='" & stCode & "'")
                                    End If
                                End If
                                dtToDate = oValidateRS.Fields.Item("U_Z_EffTodt").Value
                                strCode = oApplication.Utilities.getMaxCode("@Z_PAY11", "Code")
                                oUserTable.Code = strCode
                                oUserTable.Name = strCode + "N"
                                oUserTable.UserFields.Fields.Item("U_Z_EmpID").Value = empID
                                oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oValidateRS.Fields.Item("U_Z_EffFromdt").Value
                                If dtToDate.Year <> "1899" Then
                                    oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oValidateRS.Fields.Item("U_Z_EffTodt").Value
                                Else
                                    Dim dtD As New DateTime(2199, 12, 31)
                                    '  oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = dtD
                                End If
                                oUserTable.UserFields.Fields.Item("U_Z_Amount").Value = oValidateRS.Fields.Item("U_Z_IncAmount").Value
                                oTempRS.DoQuery("Select U_Z_InrAmt from [@Z_PAY11] where U_Z_EmpID=" & empID & " order by convert(numeric,Code) Desc ")
                                If oTempRS.RecordCount > 0 Then
                                    dblInc = oTempRS.Fields.Item(0).Value + oValidateRS.Fields.Item("U_Z_IncAmount").Value
                                Else
                                    dblInc = 0 + oValidateRS.Fields.Item("U_Z_IncAmount").Value
                                End If
                                oUserTable.UserFields.Fields.Item("U_Z_InrAmt").Value = dblInc ' oTempRS.Fields.Item("salary").Value
                                If oUserTable.Add <> 0 Then
                                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                    Return False
                                Else
                                    oTempRS1.DoQuery("Update [@Z_HR_HEM2] set U_Z_UpdatePayroll='Y' where Code='" & aRefCode & "'")

                                End If
                            Else
                                oTempRS1.DoQuery("Update [@Z_HR_HEM2] set U_Z_UpdatePayroll='Y' where Code='" & aRefCode & "'")
                            End If
                        End If
                    End If
                End If
            End If
            oUserTable = Nothing
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function

    Private Sub SelectAll(ByVal aform As SAPbouiCOM.Form, ByVal aChoice As Boolean)
        aform.Freeze(True)
        oGrid = aform.Items.Item("12").Specific
        For introw As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            oCheckBoxColumn = oGrid.Columns.Item("Select")
            oCheckBoxColumn.Check(introw, aChoice)
        Next
        aform.Freeze(False)
    End Sub

    Private Function UpdatePayroll(ByVal aform As SAPbouiCOM.Form) As Boolean


        Try
            aform.Freeze(True)
            oGrid = aform.Items.Item("12").Specific
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                oCheckBoxColumn = oGrid.Columns.Item("Select")
                If oCheckBoxColumn.IsChecked(intRow) Then
                    osta = aform.Items.Item("stProcess").Specific
                    osta.Caption = "Processing employee id : " & oGrid.DataTable.GetValue("empID", intRow)
                    If AddToUDT(oGrid.DataTable.GetValue("empID", intRow), oGrid.DataTable.GetValue("Code", intRow)) = False Then
                        aform.Freeze(False)
                        Return False
                    End If
                End If
            Next
            oApplication.Utilities.Message("Operation complted successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)

            aform.Freeze(False)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
            Return False
        End Try

    End Function
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_HR_UpdatePayroll Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD

                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "3"
                                        oForm.PaneLevel = oForm.PaneLevel - 1
                                        osta = oForm.Items.Item("15").Specific
                                        osta.Caption = "Step " & oForm.PaneLevel & " of 3"
                                    Case "4"
                                        If oForm.PaneLevel = 2 Then
                                            PopulateDetails(oForm)

                                        End If
                                    
                                        oForm.PaneLevel = oForm.PaneLevel + 1
                                        osta = oForm.Items.Item("15").Specific
                                        osta.Caption = "Step " & oForm.PaneLevel & " of 3"
                                    Case "13"
                                        SelectAll(oForm, True)
                                    Case "14"
                                        SelectAll(oForm, False)
                                    Case "5"
                                        If oApplication.SBO_Application.MessageBox("Do you want to update the payroll details ? ", , "Yes", "No") = 2 Then
                                            Exit Sub
                                        End If

                                        If oApplication.Company.InTransaction() Then
                                            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                                        End If
                                        oApplication.Company.StartTransaction()
                                        If UpdatePayroll(oForm) = True Then
                                            If oApplication.Company.InTransaction() Then
                                                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                                            End If
                                            oForm.Close()
                                        Else
                                            If oApplication.Company.InTransaction() Then
                                                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                                            End If
                                        End If
                                End Select

                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1 As String
                                Dim sCHFL_ID As String
                                Dim intChoice As Integer
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
                                        If pVal.ItemUID = "9" Or pVal.ItemUID = "11" Then
                                            val1 = oDataTable.GetValue("empID", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, pVal.ItemUID, val1)
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
                Case mnu_HR_UpdatePayroll
                    If pVal.BeforeAction = False Then
                        LoadForm()
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
