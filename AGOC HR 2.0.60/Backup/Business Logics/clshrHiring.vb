Public Class clshrHiring
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox, oCombobox1, oCombobox2, oCombobox3 As SAPbouiCOM.ComboBox
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
    Private count As Integer
    Private MatrixId As String
    Private RowtoDelete As Integer
    Private sPath, strSelectedFilepath, strSelectedFolderPath As String
    Private blnFlag As Boolean = False
    Dim oDataSrc_Line, oDataSrc_Line3 As SAPbouiCOM.DBDataSource
    Dim oDataSrc_Line1, oDataSrc_Line2, oDataSrc_Line4 As SAPbouiCOM.DBDataSource
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Public Sub LoadForm(ByVal strdoc As String, ByVal Deptid As String, ByVal Deptname As String, ByVal position As String, ByVal InvBaseno As String, ByVal Reqno As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_HR_Hiring) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_Hiring, frm_HR_Hiring)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        AddChooseFromList(oForm)
        databind(oForm)
        FillCountry(oForm)
        FillState(oForm)
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line1 = oForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP2")
        For count = 1 To oDataSrc_Line1.Size - 1
            oDataSrc_Line1.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line2 = oForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP3")
        For count = 1 To oDataSrc_Line2.Size - 1
            oDataSrc_Line2.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line3 = oForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP4")
        For count = 1 To oDataSrc_Line3.Size - 1
            oDataSrc_Line3.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line4 = oForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP5")
        For count = 1 To oDataSrc_Line4.Size - 1
            oDataSrc_Line4.SetValue("LineId", count - 1, count)
        Next
        'AddMode(oForm)
        oMatrix = oForm.Items.Item("62").Specific
        oMatrix.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oForm.PaneLevel = 7

        'oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        'oForm.Items.Item("4").Enabled = True
        'oApplication.Utilities.setEdittextvalue(oForm, "4", strdoc)
        'oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        'oApplication.Utilities.setEdittextvalue(oForm, "73", Deptid)
        'oApplication.Utilities.setEdittextvalue(oForm, "75", Deptname)
        'BindPosition(oForm, position)
        'If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE Then
        '    oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        'End If
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oForm.Items.Item("1000017").Enabled = True
        oApplication.Utilities.setEdittextvalue(oForm, "4", strdoc)
        oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)

        oApplication.Utilities.setEdittextvalue(oForm, "95", InvBaseno)
        BindPosition(oForm, Reqno)
        BindSalary(oForm, InvBaseno)
        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE Then
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        End If
        FillEducationType(oForm)
        oForm.Items.Item("4").Enabled = False
        If oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(oForm, "92")) > 0 Then
            oForm.Items.Item("71").Visible = False
            oForm.Items.Item("1").Visible = False
            EnableDisable(oForm, "H")
        Else
            oForm.Items.Item("71").Visible = True
            oForm.Items.Item("1").Visible = True
            EnableDisable(oForm, "I")
        End If
        oForm.Items.Item("1000018").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
        oForm.Items.Item("1000019").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
        ' oForm.Items.Item("56").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
        oForm.Items.Item("1000010").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oForm.Freeze(False)
    End Sub
    Private Sub FillEducationType(ByVal sform As SAPbouiCOM.Form)
        oMatrix = sform.Items.Item("60").Specific
        Dim oColum As SAPbouiCOM.Column
        oColum = oMatrix.Columns.Item("V_2")
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("select edType,name from OHED ")
        For intRow As Integer = oColum.ValidValues.Count - 1 To 0 Step -1
            oColum.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oColum.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oColum.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        oColum.DisplayDesc = True
    End Sub
    Private Sub FillCountry(ByVal sform As SAPbouiCOM.Form)
        'oMatrix = sform.Items.Item("60").Specific
        Dim oColum As SAPbouiCOM.Column
        'oColum = oMatrix.Columns.Item("V_2")
        oCombobox = sform.Items.Item("1000017").Specific
        oCombobox1 = sform.Items.Item("1000033").Specific
        oCombobox2 = sform.Items.Item("1000035").Specific
        oCombobox3 = sform.Items.Item("105").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select Code,Name from OCRY order by Code")
     
        oCombobox.ValidValues.Add("", "")
        oCombobox1.ValidValues.Add("", "")
        oCombobox2.ValidValues.Add("", "")
        oCombobox3.ValidValues.Add("", "")

        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            ' oColum.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oCombobox1.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oCombobox2.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oCombobox3.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        sform.Items.Item("1000017").DisplayDesc = True
        sform.Items.Item("1000033").DisplayDesc = True
        sform.Items.Item("1000035").DisplayDesc = True
        sform.Items.Item("105").DisplayDesc = True
        ' oColum.DisplayDesc = True
    End Sub
    Private Sub FillState(ByVal sform As SAPbouiCOM.Form)
      
        oCombobox = sform.Items.Item("122").Specific
        oCombobox1 = sform.Items.Item("123").Specific

        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("select Code,Name  from OCST order by Code")
        oCombobox.ValidValues.Add("", "")
        oCombobox1.ValidValues.Add("", "")


        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oCombobox1.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        sform.Items.Item("122").DisplayDesc = True
        sform.Items.Item("123").DisplayDesc = True

    End Sub

    Private Sub FillState1(ByVal sform As SAPbouiCOM.Form, ByVal CountryCode As String)
        Dim oColum As SAPbouiCOM.Column
        oCombobox = sform.Items.Item("122").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("select Code,Name  from OCST where Country='" & CountryCode & "' order by Code")
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        sform.Items.Item("122").DisplayDesc = True
    End Sub
    Private Sub FillState2(ByVal sform As SAPbouiCOM.Form, ByVal CountryCode As String)
        Dim oColum As SAPbouiCOM.Column
        oCombobox = sform.Items.Item("123").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("select Code,Name  from OCST where Country='" & CountryCode & "' order by Code")
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        sform.Items.Item("123").DisplayDesc = True
    End Sub

#Region "Add Choose From List"
    Private Sub databind(ByVal aForm As SAPbouiCOM.Form)
        oEditText = aForm.Items.Item("90").Specific
        oEditText.ChooseFromListUID = "CFL3"
        oEditText.ChooseFromListAlias = "U_Z_SalCode"
    End Sub

    Private Sub AddChooseFromList(ByVal aform As SAPbouiCOM.Form)
        Try
            Dim oCFLs As SAPbouiCOM.ChooseFromListCollection
            Dim oCons As SAPbouiCOM.Conditions
            Dim oCon As SAPbouiCOM.Condition

            oCFLs = aform.ChooseFromLists

            Dim oCFL As SAPbouiCOM.ChooseFromList
            Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams

            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)

            ' Adding 3 CFL, one for the button and one for the edit text.
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_OSALST"
            oCFLCreationParams.UniqueID = "CFL3"
            oCFL = oCFLs.Add(oCFLCreationParams)

        Catch ex As Exception

        End Try
    End Sub


#End Region



    Private Sub BindPosition(ByVal aForm As SAPbouiCOM.Form, ByVal Reqno As String)
        Dim strqry As String
        Dim oTemp, oTemp1 As SAPbobsCOM.Recordset
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strqry = "Select * from [@Z_HR_ORMPREQ] where DocEntry='" & Reqno & "'"
        oTemp1.DoQuery(strqry)
        If oTemp1.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(oForm, "73", oTemp1.Fields.Item("U_Z_DeptCode").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "75", oTemp1.Fields.Item("U_Z_DeptName").Value)
            strqry = "select U_Z_PosCode,U_Z_PosName,U_Z_JobCode,U_Z_JobName,U_Z_OrgCode,U_Z_OrgName,U_Z_SalCode from [@Z_HR_OPOSIN] where U_Z_PosName='" & oTemp1.Fields.Item("U_Z_PosName").Value & "' and U_Z_PosActive='Y'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.setEdittextvalue(aForm, "77", oTemp.Fields.Item(0).Value)
                oApplication.Utilities.setEdittextvalue(aForm, "79", oTemp.Fields.Item(1).Value)
                oApplication.Utilities.setEdittextvalue(aForm, "81", oTemp.Fields.Item(2).Value)
                oApplication.Utilities.setEdittextvalue(aForm, "83", oTemp.Fields.Item(3).Value)
                oApplication.Utilities.setEdittextvalue(aForm, "85", oTemp.Fields.Item(4).Value)
                oApplication.Utilities.setEdittextvalue(aForm, "87", oTemp.Fields.Item(5).Value)
                oApplication.Utilities.setEdittextvalue(aForm, "90", oTemp.Fields.Item(6).Value)
            End If
        End If
    End Sub
    Private Sub BindSalary(ByVal aForm As SAPbouiCOM.Form, ByVal DocEntry As String)
        Dim strqry As String
        Dim oTemp As SAPbobsCOM.Recordset
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strqry = "select Max(T1.U_Z_Basic) from [@Z_HR_OHEM1] T0 inner join [@Z_HR_OHEM3] T1 on T0.DocEntry=T1.DocEntry where T0.DocEntry='" & DocEntry & "' group by T1.DocEntry"
        oTemp.DoQuery(strqry)
        If oTemp.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(aForm, "97", oTemp.Fields.Item(0).Value)
        End If
    End Sub
#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            If oApplication.Utilities.getEdittextvalue(aForm, "89") = "" Then
                oApplication.Utilities.Message("Enter Joining Date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            'If oApplication.Utilities.getEdittextvalue(aForm, "90") = "" Then
            '    oApplication.Utilities.Message("Salary Code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '    Return False
            'End If
            Dim oTemp1 As SAPbobsCOM.Recordset
            Dim stSQL1 As String
            oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            ' If aForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
            'stSQL1 = "Select isnull(U_Z_PosCode,''),isnull(U_Z_PosName,'') from [@Z_HR_OCRAPP] where DocEntry='" & oApplication.Utilities.getEdittextvalue(aForm, "4") & "'"
            'oTemp1.DoQuery(stSQL1)
            'If oTemp1.RecordCount > 0 Then
            '    If oTemp1.Fields.Item(0).Value = "" Then
            '        oApplication.Utilities.Message("you must Update Hiring Details", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '        Return False
            '    End If
            'End If
            ' End If

            Dim probmonth As Double
            Dim Probdate, joindt As Date
            If oApplication.Utilities.getEdittextvalue(oForm, "89") <> "" Then
                joindt = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(oForm, "89"))
                probmonth = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(oForm, "126"))
                Probdate = DateAdd(DateInterval.Month, probmonth, joindt)
                oApplication.Utilities.setEdittextvalue(oForm, "128", Probdate)
            End If

            AssignLineNo1(aForm)
            AssignLineNo2(aForm)
            AssignLineNo3(aForm)
            AssignLineNo4(aForm)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
#End Region

#Region "AddToUDT"
    Private Function AddToUDTPromotion(ByVal aForm As SAPbouiCOM.Form, ByVal strempid As String) As Boolean
        Dim strTable, strReqno, strCode, strType, strAppcode, strqry, strDeptcode, strStatus, strDept, strDeptName, strPosition As String
        Dim strcount As Integer
        Dim dblValue As Double
        Dim dt As Date
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim oValidateRS, otemp2 As SAPbobsCOM.Recordset
        oUserTable = oApplication.Company.UserTables.Item("Z_HR_HEM2")
        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strTable = "@Z_HR_HEM2"
        Dim stQuery As String
        strqry = "select U_Z_EmpId,U_Z_FirstName,U_Z_LastName,U_Z_Dept,U_Z_DeptName,U_Z_PosCode,U_Z_PosName,"
        strqry = strqry & "	U_Z_JobCode,U_Z_JobName,U_Z_JoinDate,U_Z_OrgCode,U_Z_OrgName,U_Z_SalCode  from [@Z_HR_OCRAPP] where U_Z_EmpId='" & strempid & "'"
        oValidateRS.DoQuery(strqry)
        If oValidateRS.RecordCount > 0 Then
            If 1 = 2 Then 'oUserTable.GetByKey(oValidateRS.Fields.Item("U_Z_EmpId").Value) Then
                oUserTable.UserFields.Fields.Item("U_Z_EmpId").Value = oValidateRS.Fields.Item("U_Z_EmpId").Value
                oUserTable.UserFields.Fields.Item("U_Z_FirstName").Value = oValidateRS.Fields.Item("U_Z_FirstName").Value
                oUserTable.UserFields.Fields.Item("U_Z_LastName").Value = oValidateRS.Fields.Item("U_Z_LastName").Value
                oUserTable.UserFields.Fields.Item("U_Z_Dept").Value = oValidateRS.Fields.Item("U_Z_Dept").Value
                oUserTable.UserFields.Fields.Item("U_Z_DeptName").Value = oValidateRS.Fields.Item("U_Z_DeptName").Value
                oUserTable.UserFields.Fields.Item("U_Z_PosCode").Value = oValidateRS.Fields.Item("U_Z_PosCode").Value
                oUserTable.UserFields.Fields.Item("U_Z_PosName").Value = oValidateRS.Fields.Item("U_Z_PosName").Value
                oUserTable.UserFields.Fields.Item("U_Z_JobCode").Value = oValidateRS.Fields.Item("U_Z_JobCode").Value
                oUserTable.UserFields.Fields.Item("U_Z_JobName").Value = oValidateRS.Fields.Item("U_Z_JobName").Value
                oUserTable.UserFields.Fields.Item("U_Z_OrgCode").Value = oValidateRS.Fields.Item("U_Z_OrgCode").Value
                oUserTable.UserFields.Fields.Item("U_Z_OrgName").Value = oValidateRS.Fields.Item("U_Z_OrgName").Value
                oUserTable.UserFields.Fields.Item("U_Z_JoinDate").Value = oValidateRS.Fields.Item("U_Z_JoinDate").Value
                oUserTable.UserFields.Fields.Item("U_Z_SalCode").Value = oValidateRS.Fields.Item("U_Z_SalCode").Value
                If oUserTable.Update <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    oApplication.Utilities.Message("Operation Completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                End If
            Else
                strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                oUserTable.Code = strCode
                oUserTable.Name = strCode
                oUserTable.UserFields.Fields.Item("U_Z_EmpId").Value = oValidateRS.Fields.Item("U_Z_EmpId").Value
                oUserTable.UserFields.Fields.Item("U_Z_FirstName").Value = oValidateRS.Fields.Item("U_Z_FirstName").Value
                oUserTable.UserFields.Fields.Item("U_Z_LastName").Value = oValidateRS.Fields.Item("U_Z_LastName").Value
                oUserTable.UserFields.Fields.Item("U_Z_Dept").Value = oValidateRS.Fields.Item("U_Z_Dept").Value
                oUserTable.UserFields.Fields.Item("U_Z_DeptName").Value = oValidateRS.Fields.Item("U_Z_DeptName").Value
                oUserTable.UserFields.Fields.Item("U_Z_PosCode").Value = oValidateRS.Fields.Item("U_Z_PosCode").Value
                oUserTable.UserFields.Fields.Item("U_Z_PosName").Value = oValidateRS.Fields.Item("U_Z_PosName").Value
                oUserTable.UserFields.Fields.Item("U_Z_JobCode").Value = oValidateRS.Fields.Item("U_Z_JobCode").Value
                oUserTable.UserFields.Fields.Item("U_Z_JobName").Value = oValidateRS.Fields.Item("U_Z_JobName").Value
                oUserTable.UserFields.Fields.Item("U_Z_OrgCode").Value = oValidateRS.Fields.Item("U_Z_OrgCode").Value
                oUserTable.UserFields.Fields.Item("U_Z_OrgName").Value = oValidateRS.Fields.Item("U_Z_OrgName").Value
                oUserTable.UserFields.Fields.Item("U_Z_JoinDate").Value = oValidateRS.Fields.Item("U_Z_JoinDate").Value
                oUserTable.UserFields.Fields.Item("U_Z_SalCode").Value = oValidateRS.Fields.Item("U_Z_SalCode").Value
                If oUserTable.Add <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    Dim strdocnum, strSql As String
                    otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oApplication.Company.GetNewObjectCode(strdocnum)
                    strSql = "Update OHEM set U_Z_HR_PromoCode='" & strdocnum & "' where empID='" & strempid & "'"
                    otemp2.DoQuery(strSql)
                    oApplication.Utilities.Message("Operation Completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                End If

            End If
          
        End If
        oUserTable = Nothing
        Return True
    End Function
    Private Function AddToUDTTransfer(ByVal aForm As SAPbouiCOM.Form, ByVal strempid As String) As Boolean
        Dim strTable, strReqno, strCode, strType, strAppcode, strqry, strDeptcode, strStatus, strDept, strDeptName, strPosition As String
        Dim strcount As Integer
        Dim dblValue As Double
        Dim dt As Date
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim oValidateRS, otemp2 As SAPbobsCOM.Recordset
        oUserTable = oApplication.Company.UserTables.Item("Z_HR_HEM3")
        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strTable = "@Z_HR_HEM3"
        Dim stQuery As String
        strqry = "select U_Z_EmpId,U_Z_FirstName,U_Z_LastName,U_Z_Dept,U_Z_DeptName,U_Z_PosCode,U_Z_PosName,"
        strqry = strqry & "	U_Z_JobCode,U_Z_JobName,U_Z_JoinDate,U_Z_OrgCode,U_Z_OrgName,U_Z_SalCode  from [@Z_HR_OCRAPP] where U_Z_EmpId='" & strempid & "'"
        oValidateRS.DoQuery(strqry)
        If oValidateRS.RecordCount > 0 Then
            If 1 = 2 Then ' oUserTable.GetByKey(oValidateRS.Fields.Item("U_Z_EmpId").Value) Then
                oUserTable.UserFields.Fields.Item("U_Z_EmpId").Value = oValidateRS.Fields.Item("U_Z_EmpId").Value
                oUserTable.UserFields.Fields.Item("U_Z_FirstName").Value = oValidateRS.Fields.Item("U_Z_FirstName").Value
                oUserTable.UserFields.Fields.Item("U_Z_LastName").Value = oValidateRS.Fields.Item("U_Z_LastName").Value
                oUserTable.UserFields.Fields.Item("U_Z_Dept").Value = oValidateRS.Fields.Item("U_Z_Dept").Value
                oUserTable.UserFields.Fields.Item("U_Z_DeptName").Value = oValidateRS.Fields.Item("U_Z_DeptName").Value
                oUserTable.UserFields.Fields.Item("U_Z_PosCode").Value = oValidateRS.Fields.Item("U_Z_PosCode").Value
                oUserTable.UserFields.Fields.Item("U_Z_PosName").Value = oValidateRS.Fields.Item("U_Z_PosName").Value
                oUserTable.UserFields.Fields.Item("U_Z_JobCode").Value = oValidateRS.Fields.Item("U_Z_JobCode").Value
                oUserTable.UserFields.Fields.Item("U_Z_JobName").Value = oValidateRS.Fields.Item("U_Z_JobName").Value
                oUserTable.UserFields.Fields.Item("U_Z_OrgCode").Value = oValidateRS.Fields.Item("U_Z_OrgCode").Value
                oUserTable.UserFields.Fields.Item("U_Z_OrgName").Value = oValidateRS.Fields.Item("U_Z_OrgName").Value
                oUserTable.UserFields.Fields.Item("U_Z_JoinDate").Value = oValidateRS.Fields.Item("U_Z_JoinDate").Value
                oUserTable.UserFields.Fields.Item("U_Z_SalCode").Value = oValidateRS.Fields.Item("U_Z_SalCode").Value
                If oUserTable.Update <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    oApplication.Utilities.Message("Operation Completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                End If
            Else
                strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                oUserTable.Code = strCode
                oUserTable.Name = strCode
                oUserTable.UserFields.Fields.Item("U_Z_EmpId").Value = oValidateRS.Fields.Item("U_Z_EmpId").Value
                oUserTable.UserFields.Fields.Item("U_Z_FirstName").Value = oValidateRS.Fields.Item("U_Z_FirstName").Value
                oUserTable.UserFields.Fields.Item("U_Z_LastName").Value = oValidateRS.Fields.Item("U_Z_LastName").Value
                oUserTable.UserFields.Fields.Item("U_Z_Dept").Value = oValidateRS.Fields.Item("U_Z_Dept").Value
                oUserTable.UserFields.Fields.Item("U_Z_DeptName").Value = oValidateRS.Fields.Item("U_Z_DeptName").Value
                oUserTable.UserFields.Fields.Item("U_Z_PosCode").Value = oValidateRS.Fields.Item("U_Z_PosCode").Value
                oUserTable.UserFields.Fields.Item("U_Z_PosName").Value = oValidateRS.Fields.Item("U_Z_PosName").Value
                oUserTable.UserFields.Fields.Item("U_Z_JobCode").Value = oValidateRS.Fields.Item("U_Z_JobCode").Value
                oUserTable.UserFields.Fields.Item("U_Z_JobName").Value = oValidateRS.Fields.Item("U_Z_JobName").Value
                oUserTable.UserFields.Fields.Item("U_Z_OrgCode").Value = oValidateRS.Fields.Item("U_Z_OrgCode").Value
                oUserTable.UserFields.Fields.Item("U_Z_OrgName").Value = oValidateRS.Fields.Item("U_Z_OrgName").Value
                oUserTable.UserFields.Fields.Item("U_Z_JoinDate").Value = oValidateRS.Fields.Item("U_Z_JoinDate").Value
                oUserTable.UserFields.Fields.Item("U_Z_SalCode").Value = oValidateRS.Fields.Item("U_Z_SalCode").Value
                If oUserTable.Add <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    Dim strdocnum, strSql As String
                    otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oApplication.Company.GetNewObjectCode(strdocnum)
                    strSql = "Update OHEM set U_Z_HR_TransferCode='" & strdocnum & "' where empID='" & strempid & "'"
                    otemp2.DoQuery(strSql)
                    oApplication.Utilities.Message("Operation Completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                End If
            End If
         
        End If
        oUserTable = Nothing
        Return True
    End Function
    Private Function AddToUDTPosition(ByVal aForm As SAPbouiCOM.Form, ByVal strempid As String) As Boolean
        Dim strTable, strReqno, strCode, strType, strAppcode, strqry, strDeptcode, strStatus, strDept, strDeptName, strPosition As String
        Dim strcount As Integer
        Dim dblValue As Double
        Dim dt As Date
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim oValidateRS, otemp2 As SAPbobsCOM.Recordset
        oUserTable = oApplication.Company.UserTables.Item("Z_HR_HEM4")
        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strTable = "@Z_HR_HEM4"
        Dim stQuery As String
        strqry = "select U_Z_EmpId,U_Z_FirstName,U_Z_LastName,U_Z_Dept,U_Z_DeptName,U_Z_PosCode,U_Z_PosName,"
        strqry = strqry & "	U_Z_JobCode,U_Z_JobName,U_Z_JoinDate,U_Z_OrgCode,U_Z_OrgName,U_Z_SalCode  from [@Z_HR_OCRAPP] where U_Z_EmpId='" & strempid & "'"
        oValidateRS.DoQuery(strqry)
        If oValidateRS.RecordCount > 0 Then
            If 1 = 2 Then ' oUserTable.GetByKey(oValidateRS.Fields.Item("U_Z_EmpId").Value) Then
                oUserTable.UserFields.Fields.Item("U_Z_EmpId").Value = oValidateRS.Fields.Item("U_Z_EmpId").Value
                oUserTable.UserFields.Fields.Item("U_Z_FirstName").Value = oValidateRS.Fields.Item("U_Z_FirstName").Value
                oUserTable.UserFields.Fields.Item("U_Z_LastName").Value = oValidateRS.Fields.Item("U_Z_LastName").Value
                oUserTable.UserFields.Fields.Item("U_Z_Dept").Value = oValidateRS.Fields.Item("U_Z_Dept").Value
                oUserTable.UserFields.Fields.Item("U_Z_DeptName").Value = oValidateRS.Fields.Item("U_Z_DeptName").Value
                oUserTable.UserFields.Fields.Item("U_Z_PosCode").Value = oValidateRS.Fields.Item("U_Z_PosCode").Value
                oUserTable.UserFields.Fields.Item("U_Z_PosName").Value = oValidateRS.Fields.Item("U_Z_PosName").Value
                oUserTable.UserFields.Fields.Item("U_Z_JobCode").Value = oValidateRS.Fields.Item("U_Z_JobCode").Value
                oUserTable.UserFields.Fields.Item("U_Z_JobName").Value = oValidateRS.Fields.Item("U_Z_JobName").Value
                oUserTable.UserFields.Fields.Item("U_Z_OrgCode").Value = oValidateRS.Fields.Item("U_Z_OrgCode").Value
                oUserTable.UserFields.Fields.Item("U_Z_OrgName").Value = oValidateRS.Fields.Item("U_Z_OrgName").Value
                oUserTable.UserFields.Fields.Item("U_Z_JoinDate").Value = oValidateRS.Fields.Item("U_Z_JoinDate").Value
                oUserTable.UserFields.Fields.Item("U_Z_SalCode").Value = oValidateRS.Fields.Item("U_Z_SalCode").Value
                If oUserTable.Update <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    oApplication.Utilities.Message("Operation Completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                End If
            Else
                strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                oUserTable.Code = strCode
                oUserTable.Name = strCode
                oUserTable.UserFields.Fields.Item("U_Z_EmpId").Value = oValidateRS.Fields.Item("U_Z_EmpId").Value
                oUserTable.UserFields.Fields.Item("U_Z_FirstName").Value = oValidateRS.Fields.Item("U_Z_FirstName").Value
                oUserTable.UserFields.Fields.Item("U_Z_LastName").Value = oValidateRS.Fields.Item("U_Z_LastName").Value
                oUserTable.UserFields.Fields.Item("U_Z_Dept").Value = oValidateRS.Fields.Item("U_Z_Dept").Value
                oUserTable.UserFields.Fields.Item("U_Z_DeptName").Value = oValidateRS.Fields.Item("U_Z_DeptName").Value
                oUserTable.UserFields.Fields.Item("U_Z_PosCode").Value = oValidateRS.Fields.Item("U_Z_PosCode").Value
                oUserTable.UserFields.Fields.Item("U_Z_PosName").Value = oValidateRS.Fields.Item("U_Z_PosName").Value
                oUserTable.UserFields.Fields.Item("U_Z_JobCode").Value = oValidateRS.Fields.Item("U_Z_JobCode").Value
                oUserTable.UserFields.Fields.Item("U_Z_JobName").Value = oValidateRS.Fields.Item("U_Z_JobName").Value
                oUserTable.UserFields.Fields.Item("U_Z_OrgCode").Value = oValidateRS.Fields.Item("U_Z_OrgCode").Value
                oUserTable.UserFields.Fields.Item("U_Z_OrgName").Value = oValidateRS.Fields.Item("U_Z_OrgName").Value
                oUserTable.UserFields.Fields.Item("U_Z_JoinDate").Value = oValidateRS.Fields.Item("U_Z_JoinDate").Value
                oUserTable.UserFields.Fields.Item("U_Z_SalCode").Value = oValidateRS.Fields.Item("U_Z_SalCode").Value
                If oUserTable.Add <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    Dim strdocnum, strSql As String
                    otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oApplication.Company.GetNewObjectCode(strdocnum)
                    strSql = "Update OHEM set U_Z_HR_PosChangeCode='" & strdocnum & "' where empID='" & strempid & "'"
                    otemp2.DoQuery(strSql)
                    oApplication.Utilities.Message("Operation Completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                End If
            End If

        End If
        oUserTable = Nothing
        Return True
    End Function


#End Region

    'Private Sub AssignLineNo(ByVal aForm As SAPbouiCOM.Form)
    '    Try
    '        aForm.Freeze(True)
    '        oMatrix = aForm.Items.Item("57").Specific
    '        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP1")
    '        oMatrix.FlushToDataSource()
    '        For count = 1 To oDataSrc_Line.Size
    '            oDataSrc_Line.SetValue("LineId", count - 1, count)
    '        Next
    '        oMatrix.LoadFromDataSource()
    '        aForm.Freeze(False)
    '    Catch ex As Exception
    '        oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
    '        aForm.Freeze(False)
    '    End Try
    'End Sub
    Private Sub AssignLineNo1(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("58").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP2")
            oMatrix.FlushToDataSource()
            For count = 1 To oDataSrc_Line.Size
                oDataSrc_Line.SetValue("LineId", count - 1, count)
            Next
            oMatrix.LoadFromDataSource()

            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
        End Try

    End Sub
    Private Sub AssignLineNo2(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("60").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP3")
            oMatrix.FlushToDataSource()
            For count = 1 To oDataSrc_Line.Size
                oDataSrc_Line.SetValue("LineId", count - 1, count)
            Next
            oMatrix.LoadFromDataSource()

            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
        End Try

    End Sub
    Private Sub AssignLineNo3(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("61").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP4")
            oMatrix.FlushToDataSource()
            For count = 1 To oDataSrc_Line.Size
                oDataSrc_Line.SetValue("LineId", count - 1, count)
            Next
            oMatrix.LoadFromDataSource()

            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
        End Try
    End Sub

    Private Sub AssignLineNo4(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("62").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP5")
            oMatrix.FlushToDataSource()
            For count = 1 To oDataSrc_Line.Size
                oDataSrc_Line.SetValue("LineId", count - 1, count)
            Next
            oMatrix.LoadFromDataSource()

            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
        End Try
    End Sub


#Region "Add Row/ Delete Row"

    Private Sub AddRow(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)

            Select Case aForm.PaneLevel
                Case "2"
                    oMatrix = aForm.Items.Item("58").Specific
                    oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP2")

                    If oMatrix.RowCount <= 0 Then
                        oMatrix.AddRow()
                    End If
                    oEditText = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                    Try
                        If oEditText.Value <> "" Then
                            oMatrix.AddRow()
                            Select Case aForm.PaneLevel
                                Case "2"
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, "")
                                Case "3"
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_3", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_4", oMatrix.RowCount, "")
                                Case "4"
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_3", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_4", oMatrix.RowCount, "")
                                Case "5"
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                            End Select
                        End If

                    Catch ex As Exception
                        aForm.Freeze(False)
                        'oMatrix.AddRow()
                    End Try
                    oMatrix.FlushToDataSource()
                    For count = 1 To oDataSrc_Line.Size
                        oDataSrc_Line.SetValue("LineId", count - 1, count)
                    Next
                    oMatrix.LoadFromDataSource()
                    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    AssignLineNo1(aForm)
                Case "3"
                    oMatrix = aForm.Items.Item("60").Specific
                    oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP3")
                    If oMatrix.RowCount <= 0 Then
                        oMatrix.AddRow()
                    End If
                    oEditText = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                    Try
                        If oEditText.Value <> "" Then
                            oMatrix.AddRow()
                            Select Case aForm.PaneLevel
                                Case "2"
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, "")
                                Case "3"
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_3", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_4", oMatrix.RowCount, "")
                                Case "4"
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_3", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_4", oMatrix.RowCount, "")
                                Case "5"
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                            End Select
                        End If

                    Catch ex As Exception
                        aForm.Freeze(False)
                        'oMatrix.AddRow()
                    End Try
                    oMatrix.FlushToDataSource()
                    For count = 1 To oDataSrc_Line.Size
                        oDataSrc_Line.SetValue("LineId", count - 1, count)
                    Next
                    oMatrix.LoadFromDataSource()
                    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    AssignLineNo2(aForm)
                Case "4"
                    oMatrix = aForm.Items.Item("61").Specific
                    oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP4")
                    If oMatrix.RowCount <= 0 Then
                        oMatrix.AddRow()
                    End If
                    oEditText = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                    Try
                        If oEditText.Value <> "" Then
                            oMatrix.AddRow()
                            Select Case aForm.PaneLevel
                                Case "2"
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, "")
                                Case "3"
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_3", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_4", oMatrix.RowCount, "")
                                Case "4"
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_3", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_4", oMatrix.RowCount, "")
                                Case "5"
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                            End Select
                        End If

                    Catch ex As Exception
                        aForm.Freeze(False)
                        'oMatrix.AddRow()
                    End Try
                    oMatrix.FlushToDataSource()
                    For count = 1 To oDataSrc_Line.Size
                        oDataSrc_Line.SetValue("LineId", count - 1, count)
                    Next
                    oMatrix.LoadFromDataSource()
                    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    AssignLineNo3(aForm)
                Case "5"
                    oMatrix = aForm.Items.Item("62").Specific
                    oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP5")
                    If oMatrix.RowCount <= 0 Then
                        oMatrix.AddRow()
                    End If
                    oEditText = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                    Try
                        If oEditText.Value <> "" Then
                            oMatrix.AddRow()
                            Select Case aForm.PaneLevel
                           
                                Case "2"
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, "")
                                Case "3"
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_3", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_4", oMatrix.RowCount, "")
                                Case "4"
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_3", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_4", oMatrix.RowCount, "")
                                Case "5"
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                            End Select
                        End If

                    Catch ex As Exception
                        aForm.Freeze(False)
                        'oMatrix.AddRow()
                    End Try
                    oMatrix.FlushToDataSource()
                    For count = 1 To oDataSrc_Line.Size
                        oDataSrc_Line.SetValue("LineId", count - 1, count)
                    Next
                    oMatrix.LoadFromDataSource()
                    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    AssignLineNo4(aForm)
            End Select


            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)

        End Try
    End Sub
    Private Sub deleterow(ByVal aForm As SAPbouiCOM.Form)
        Select Case aForm.PaneLevel
            Case "2"
                oMatrix = aForm.Items.Item("58").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP2")
            Case "3"
                oMatrix = aForm.Items.Item("60").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP3")
            Case "4"
                oMatrix = aForm.Items.Item("61").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP4")
            Case "5"
                oMatrix = aForm.Items.Item("62").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP5")

        End Select

        '  oMatrix = aForm.Items.Item("16").Specific
        oMatrix.FlushToDataSource()
        For introw As Integer = 1 To oMatrix.RowCount
            If oMatrix.IsRowSelected(introw) Then
                oMatrix.DeleteRow(introw)
                oDataSrc_Line.RemoveRecord(introw - 1)
                'oMatrix = frmSourceMatrix
                For count As Integer = 1 To oDataSrc_Line.Size
                    oDataSrc_Line.SetValue("LineId", count - 1, count)
                Next
                Select Case aForm.PaneLevel
                    Case "2"
                        oMatrix = aForm.Items.Item("58").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP2")
                        AssignLineNo1(aForm)
                    Case "3"
                        oMatrix = aForm.Items.Item("60").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP3")
                        AssignLineNo2(aForm)
                    Case "4"
                        oMatrix = aForm.Items.Item("61").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP4")
                        AssignLineNo3(aForm)
                    Case "5"
                        oMatrix = aForm.Items.Item("62").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP5")
                        AssignLineNo4(aForm)
                End Select
                oMatrix.LoadFromDataSource()
                If aForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                    aForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                End If
                Exit Sub
            End If
        Next

    End Sub

    Private Sub fillopen()
        Dim mythr As New System.Threading.Thread(AddressOf ShowFileDialog)
        mythr.SetApartmentState(Threading.ApartmentState.STA)
        mythr.Start()
        mythr.Join()

    End Sub

    Private Sub ShowFileDialog()
        Dim oDialogBox As New OpenFileDialog
        Dim strFileName, strMdbFilePath As String
        Dim oProcesses() As Process
        Try
            oProcesses = Process.GetProcessesByName("SAP Business One")
            If oProcesses.Length <> 0 Then
                For i As Integer = 0 To oProcesses.Length - 1
                    Dim MyWindow As New clsListener.WindowWrapper(oProcesses(i).MainWindowHandle)
                    If oDialogBox.ShowDialog(MyWindow) = DialogResult.OK Then
                        strMdbFilePath = oDialogBox.FileName
                        strSelectedFilepath = oDialogBox.FileName
                        strFileName = strSelectedFilepath
                        strSelectedFolderPath = strFileName
                    Else
                    End If
                Next
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
        End Try
    End Sub
#End Region

#Region "Delete Row"
    Private Sub RefereshDeleteRow(ByVal aForm As SAPbouiCOM.Form)
        If Me.MatrixId = "57" Then
            oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP1")
        ElseIf Me.MatrixId = "58" Then
            oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP2")
        ElseIf Me.MatrixId = "60" Then
            oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP3")
        Else
            oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_CRAPP4")
        End If
        'oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_PRJ1")
        If intSelectedMatrixrow <= 0 Then
            Exit Sub

        End If
        Me.RowtoDelete = intSelectedMatrixrow
        oDataSrc_Line.RemoveRecord(Me.RowtoDelete - 1)
        oMatrix = frmSourceMatrix
        oMatrix.FlushToDataSource()
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oMatrix.LoadFromDataSource()
        If oMatrix.RowCount > 0 Then
            oMatrix.DeleteRow(oMatrix.RowCount)
        End If
    End Sub

    Private Sub LoadFiles(ByVal aform As SAPbouiCOM.Form)
        oMatrix = aform.Items.Item("62").Specific
        For intRow As Integer = 1 To oMatrix.RowCount
            If oMatrix.IsRowSelected(intRow) Then
                Dim strFilename As String
                strFilename = oMatrix.Columns.Item("V_0").Cells.Item(intRow).Specific.value
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
#End Region


#Region "Convert To Employee"
    Private Function EmployeeCreation(ByVal aForm As SAPbouiCOM.Form, ByVal strcode As String) As Boolean
        Try
            Dim strqry As String
            Dim count As Integer = 0
            Dim oEmployee As SAPbobsCOM.EmployeesInfo
            Dim otemp1, otemp, otemp2, otemp3, otemp4 As SAPbobsCOM.Recordset
            oEmployee = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oEmployeesInfo)
            otemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemp4 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If
            oApplication.Company.StartTransaction()
            strqry = "select isnull(U_Z_EmpId,'9999') 'U_Z_EmpId',DocEntry ,U_Z_FirstName,U_Z_LastName,U_Z_EmailId,U_Z_Mobile,U_Z_Dept,U_Z_DeptName ,U_Z_PosCode,U_Z_PosName,"
            strqry = strqry & "U_Z_JobCode,U_Z_JobName,U_Z_OrgCode,U_Z_OrgName,U_Z_JoinDate,U_Z_SalCode,U_Z_offBasic,"
            strqry = strqry & "U_Z_Children,U_Z_Dob,U_Z_Citizen,U_Z_Marital,U_Z_Nationality,U_Z_PBlock,U_Z_PBuilding,U_Z_PCity,U_Z_PCity,U_Z_PCountry,"
            strqry = strqry & " U_Z_PState,U_Z_PStreet,U_Z_PZipCode,U_Z_Passexpdate,U_Z_Passport,U_Z_Sex,U_Z_TBlock,U_Z_TBuilding,U_Z_TCity,"
            strqry = strqry & " U_Z_TCountry,U_Z_TState,U_Z_TStreet,U_Z_TZipCode  from [@Z_HR_OCRAPP] where DocEntry='" & strcode & "'"
            otemp1.DoQuery(strqry)
            If otemp1.RecordCount > 0 Then
                Dim intCode As Integer = CInt(otemp1.Fields.Item("U_Z_EmpId").Value)
                If intCode <> 9999 And intCode > 0 Then ' oEmployee.GetByKey(intCode) Then

                    'End If
                    'If oEmployee.GetByKey(CInt(otemp1.Fields.Item("U_Z_EmpId").Value)) And otemp1.Fields.Item("U_Z_EmpId").Value <> "9999" Then
                    oApplication.Utilities.Message("Applicant already converted to Employee", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    If oApplication.Company.InTransaction() Then
                        oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                    End If
                    Return False
                Else
                    'Time Stamp
                    oApplication.Utilities.UpdateApplicantTimeStamp(otemp1.Fields.Item("DocEntry").Value, "HI")

                    oEmployee.FirstName = otemp1.Fields.Item("U_Z_FirstName").Value
                    oEmployee.LastName = otemp1.Fields.Item("U_Z_LastName").Value
                    oEmployee.eMail = otemp1.Fields.Item("U_Z_EmailId").Value
                    oEmployee.MobilePhone = otemp1.Fields.Item("U_Z_Mobile").Value
                    oEmployee.Salary = otemp1.Fields.Item("U_Z_OffBasic").Value
                    oEmployee.DateOfBirth = otemp1.Fields.Item("U_Z_Dob").Value
                    'oEmployee.Gender = otemp1.Fields.Item("U_Z_Sex").Value
                    oEmployee.CountryOfBirth = otemp1.Fields.Item("U_Z_Nationality").Value
                    '  oEmployee.MartialStatus = otemp1.Fields.Item("U_Z_Marital").Value
                    oEmployee.NumOfChildren = otemp1.Fields.Item("U_Z_Children").Value
                    oEmployee.CitizenshipCountryCode = otemp1.Fields.Item("U_Z_Citizen").Value
                    oEmployee.PassportNumber = otemp1.Fields.Item("U_Z_Passport").Value
                    oEmployee.PassportExpirationDate = otemp1.Fields.Item("U_Z_Passexpdate").Value
                    If otemp1.Fields.Item("U_Z_Dept").Value <> "" Then
                        oEmployee.Department = Convert.ToInt16(otemp1.Fields.Item("U_Z_Dept").Value)
                    End If

                    If otemp1.Fields.Item("U_Z_PosCode").Value <> "" Then
                        Dim oT, oRec As SAPbobsCOM.Recordset
                        oT = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oRec.DoQuery("select * from ""@Z_HR_OPOSIN"" where ""U_Z_PosCode""='" & otemp1.Fields.Item("U_Z_PosCode").Value & "'")
                        If oRec.RecordCount > 0 Then
                            oT.DoQuery("Select ""posID"" from OHPS where ""U_Z_POSRef"" ='" & oRec.Fields.Item("DocEntry").Value & "'")
                            If oT.RecordCount > 0 Then
                                oEmployee.Position = oT.Fields.Item("posID").Value
                            End If
                        End If
                        'oEmployee.Position = Convert.ToInt16(otemp1.Fields.Item("U_Z_PosCode").Value)
                    End If
                    oEmployee.StartDate = otemp1.Fields.Item("U_Z_JoinDate").Value
                    oEmployee.WorkStreet = otemp1.Fields.Item("U_Z_PStreet").Value
                    oEmployee.WorkBlock = otemp1.Fields.Item("U_Z_PBlock").Value
                    oEmployee.WorkBuildingFloorRoom = otemp1.Fields.Item("U_Z_PBuilding").Value
                    oEmployee.WorkCity = otemp1.Fields.Item("U_Z_PCity").Value
                    oEmployee.WorkCountryCode = otemp1.Fields.Item("U_Z_PCountry").Value
                    oEmployee.WorkStateCode = otemp1.Fields.Item("U_Z_PState").Value
                    oEmployee.WorkZipCode = otemp1.Fields.Item("U_Z_PZipCode").Value
                    oEmployee.HomeStreet = otemp1.Fields.Item("U_Z_TStreet").Value
                    oEmployee.HomeBlock = otemp1.Fields.Item("U_Z_TBlock").Value
                    oEmployee.HomeBuildingFloorRoom = otemp1.Fields.Item("U_Z_TBuilding").Value
                    oEmployee.HomeCity = otemp1.Fields.Item("U_Z_TCity").Value
                    oEmployee.HomeCountry = otemp1.Fields.Item("U_Z_TCountry").Value
                    oEmployee.HomeState = otemp1.Fields.Item("U_Z_TState").Value
                    oEmployee.HomeZipCode = otemp1.Fields.Item("U_Z_TZipCode").Value

                    oEmployee.UserFields.Fields.Item("U_Z_HR_PosiCode").Value = otemp1.Fields.Item("U_Z_PosCode").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_PosiName").Value = otemp1.Fields.Item("U_Z_PosName").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_JobstCode").Value = otemp1.Fields.Item("U_Z_JobCode").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_JobstName").Value = otemp1.Fields.Item("U_Z_JobName").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_OrgstCode").Value = otemp1.Fields.Item("U_Z_OrgCode").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_OrgstName").Value = otemp1.Fields.Item("U_Z_OrgName").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_SalaryCode").Value = otemp1.Fields.Item("U_Z_SalCode").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_JoinDate").Value = otemp1.Fields.Item("U_Z_JoinDate").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_ApplId").Value = otemp1.Fields.Item("DocEntry").Value
                    otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    otemp.DoQuery("select isnull(U_Z_Level,0) as U_Z_Level,U_Z_School,U_Z_GrFromDate,U_Z_GrT0Date,U_Z_Major,U_Z_Diploma  from [@Z_HR_CRAPP3] where isnull(U_Z_Level,'')<>'' and DocEntry='" & strcode & "'")
                    For introw As Integer = 0 To otemp.RecordCount - 1
                        oEmployee.EducationInfo.Add()
                        oEmployee.EducationInfo.SetCurrentLine(count)
                        oEmployee.EducationInfo.EmployeeNo = otemp.Fields.Item(0).Value
                        oEmployee.EducationInfo.FromDate = otemp.Fields.Item("U_Z_GrFromDate").Value
                        oEmployee.EducationInfo.ToDate = otemp.Fields.Item("U_Z_GrT0Date").Value
                        If otemp.Fields.Item("U_Z_Level").Value <> 0 Then
                            oEmployee.EducationInfo.EducationType = otemp.Fields.Item("U_Z_Level").Value
                        Else
                            oEmployee.EducationInfo.EducationType = 0
                        End If
                        oEmployee.EducationInfo.Institute = otemp.Fields.Item("U_Z_School").Value
                        oEmployee.EducationInfo.Major = otemp.Fields.Item("U_Z_Major").Value
                        oEmployee.EducationInfo.Diploma = otemp.Fields.Item("U_Z_Diploma").Value
                        otemp.MoveNext()
                        count = count + 1
                    Next
                    otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    otemp2.DoQuery("select U_Z_FromDate,U_Z_ToDate,isnull(U_Z_PrEmployer,'') as U_Z_PrEmployer,U_Z_PrPosition,U_Z_Remarks   from [@Z_HR_CRAPP4] where isnull(U_Z_PrEmployer,'')<>'' and DocEntry='" & strcode & "'")
                    count = 0
                    For introw As Integer = 0 To otemp2.RecordCount - 1
                        oEmployee.PreviousEmpoymentInfo.Add()
                        oEmployee.PreviousEmpoymentInfo.SetCurrentLine(count)
                        oEmployee.PreviousEmpoymentInfo.FromDtae = otemp2.Fields.Item("U_Z_FromDate").Value
                        oEmployee.PreviousEmpoymentInfo.ToDate = otemp2.Fields.Item("U_Z_ToDate").Value
                        If otemp2.Fields.Item("U_Z_PrEmployer").Value <> "" Then
                            oEmployee.PreviousEmpoymentInfo.Employer = otemp2.Fields.Item("U_Z_PrEmployer").Value
                        Else
                            oEmployee.PreviousEmpoymentInfo.Employer = ""
                        End If
                        oEmployee.PreviousEmpoymentInfo.Position = otemp2.Fields.Item("U_Z_PrPosition").Value
                        oEmployee.PreviousEmpoymentInfo.Remarks = otemp2.Fields.Item("U_Z_Remarks").Value
                        otemp2.MoveNext()
                        count = count + 1
                    Next
                    If oEmployee.Add <> 0 Then
                        If oApplication.Company.InTransaction() Then
                            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                            Return False
                        End If
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Else
                        Dim strdocnum, strqry1 As String
                        otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oApplication.Company.GetNewObjectCode(strdocnum)
                        otemp.DoQuery("select empID  from OHEM  where empID=" & strdocnum)
                        strqry1 = "Update [@Z_HR_OCRAPP] set U_Z_EmpId=" & otemp.Fields.Item(0).Value & ",U_Z_Status='H' where DocEntry=" & strcode
                        otemp1.DoQuery(strqry1)
                        otemp1.DoQuery("Update [@Z_HR_OHEM1] set U_Z_IntervStatus='P',U_Z_EmpId='" & otemp.Fields.Item(0).Value & "' where U_Z_IPHODSta='S' and U_Z_HRAppID=" & strcode)
                        Dim dtpro As Date = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "128"))
                        otemp4.DoQuery("Update OHEM set U_Z_Prodate='" & dtpro.ToString("yyyy/MM/dd") & "',U_Z_Promonth=" & oApplication.Utilities.getEdittextvalue(aForm, "126") & " where empID=" & strdocnum)
                        AddToUDTPromotion(oForm, otemp.Fields.Item(0).Value)
                        AddToUDTPosition(oForm, otemp.Fields.Item(0).Value)
                        AddToUDTTransfer(oForm, otemp.Fields.Item(0).Value)
                        If oApplication.Company.InTransaction() Then
                            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                        End If
                        oApplication.Utilities.UpdateEmployeeHRDetails(aForm, strdocnum)
                        Return True
                        'If oEmployee.GetByKey(otemp.Fields.Item(0).Value) Then
                        '    otemp.DoQuery("select isnull(U_Z_Level,0) as U_Z_Level,U_Z_School,U_Z_GrFromDate,U_Z_GrT0Date,U_Z_Major,U_Z_Diploma  from [@Z_HR_CRAPP3] where DocEntry='" & strcode & "'")
                        '    For introw As Integer = 0 To otemp.RecordCount - 1
                        '        oEmployee.EducationInfo.Add()
                        '        oEmployee.EducationInfo.SetCurrentLine(count)
                        '        oEmployee.EducationInfo.EmployeeNo = otemp.Fields.Item(0).Value
                        '        oEmployee.EducationInfo.FromDate = otemp.Fields.Item("U_Z_GrFromDate").Value
                        '        oEmployee.EducationInfo.ToDate = otemp.Fields.Item("U_Z_GrT0Date").Value
                        '        If otemp.Fields.Item("U_Z_Level").Value <> 0 Then
                        '            oEmployee.EducationInfo.EducationType = otemp.Fields.Item("U_Z_Level").Value
                        '        Else
                        '            oEmployee.EducationInfo.EducationType = 0
                        '        End If
                        '        oEmployee.EducationInfo.Institute = otemp.Fields.Item("U_Z_School").Value
                        '        oEmployee.EducationInfo.Major = otemp.Fields.Item("U_Z_Major").Value
                        '        oEmployee.EducationInfo.Diploma = otemp.Fields.Item("U_Z_Diploma").Value
                        '        otemp.MoveNext()
                        '        count = count + 1
                        '    Next
                        '    otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        '    otemp2.DoQuery("select U_Z_FromDate,U_Z_ToDate,isnull(U_Z_PrEmployer,'') as U_Z_PrEmployer,U_Z_PrPosition,U_Z_Remarks   from [@Z_HR_CRAPP4] where DocEntry='" & strcode & "'")
                        '    count = 0
                        '    For introw As Integer = 0 To otemp2.RecordCount - 1
                        '        oEmployee.PreviousEmpoymentInfo.Add()
                        '        oEmployee.PreviousEmpoymentInfo.SetCurrentLine(count)
                        '        oEmployee.PreviousEmpoymentInfo.EmployeeNo = strdocnum
                        '        oEmployee.PreviousEmpoymentInfo.FromDtae = otemp2.Fields.Item("U_Z_FromDate").Value
                        '        oEmployee.PreviousEmpoymentInfo.ToDate = otemp2.Fields.Item("U_Z_ToDate").Value
                        '        If otemp2.Fields.Item("U_Z_PrEmployer").Value <> "" Then
                        '            oEmployee.PreviousEmpoymentInfo.Employer = otemp2.Fields.Item("U_Z_PrEmployer").Value
                        '        Else
                        '            oEmployee.PreviousEmpoymentInfo.Employer = ""
                        '        End If
                        '        oEmployee.PreviousEmpoymentInfo.Position = otemp2.Fields.Item("U_Z_PrPosition").Value
                        '        oEmployee.PreviousEmpoymentInfo.Remarks = otemp2.Fields.Item("U_Z_Remarks").Value
                        '        otemp2.MoveNext()
                        '        count = count + 1
                        '    Next
                        '    If oEmployee.Update <> 0 Then
                        '        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        '        Return False
                        '    End If
                        'Return True
                        'End If
                    End If
                End If
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            End If
            Return False
        End Try
        Return True
    End Function
#End Region
    Private Sub EnableDisable(ByVal aForm As SAPbouiCOM.Form, ByVal strStatus As String)
        If strStatus = "H" Then
            aForm.Items.Item("126").Enabled = False
            aForm.Items.Item("4").Enabled = False
            aForm.Items.Item("124").Enabled = False
            aForm.Items.Item("89").Enabled = False
            aForm.Items.Item("1000015").Enabled = False
            aForm.Items.Item("1000013").Enabled = False
            aForm.Items.Item("1000017").Enabled = False
            aForm.Items.Item("101").Enabled = False
            aForm.Items.Item("1000037").Enabled = False
            aForm.Items.Item("105").Enabled = False
            aForm.Items.Item("103").Enabled = False
            aForm.Items.Item("107").Enabled = False
            aForm.Items.Item("1000021").Enabled = False
            aForm.Items.Item("109").Enabled = False
            aForm.Items.Item("113").Enabled = False
            aForm.Items.Item("117").Enabled = False
            aForm.Items.Item("1000029").Enabled = False
            aForm.Items.Item("1000025").Enabled = False
            aForm.Items.Item("122").Enabled = False
            aForm.Items.Item("1000033").Enabled = False
            aForm.Items.Item("1000023").Enabled = False
            aForm.Items.Item("111").Enabled = False
            aForm.Items.Item("115").Enabled = False
            aForm.Items.Item("119").Enabled = False
            aForm.Items.Item("1000031").Enabled = False
            aForm.Items.Item("1000027").Enabled = False
            aForm.Items.Item("123").Enabled = False
            aForm.Items.Item("1000035").Enabled = False
            aForm.Items.Item("58").Enabled = False
            aForm.Items.Item("60").Enabled = False
            aForm.Items.Item("61").Enabled = False
            aForm.Items.Item("66").Enabled = False
        Else
            aForm.Items.Item("126").Enabled = True
            aForm.Items.Item("4").Enabled = False
            aForm.Items.Item("124").Enabled = True
            aForm.Items.Item("89").Enabled = True
            aForm.Items.Item("1000015").Enabled = True
            aForm.Items.Item("1000013").Enabled = True
            aForm.Items.Item("1000017").Enabled = True
            aForm.Items.Item("101").Enabled = True
            aForm.Items.Item("1000037").Enabled = True
            aForm.Items.Item("105").Enabled = True
            aForm.Items.Item("103").Enabled = True
            aForm.Items.Item("107").Enabled = True
            aForm.Items.Item("1000021").Enabled = True
            aForm.Items.Item("109").Enabled = True
            aForm.Items.Item("113").Enabled = True
            aForm.Items.Item("117").Enabled = True
            aForm.Items.Item("1000029").Enabled = True
            aForm.Items.Item("1000025").Enabled = True
            aForm.Items.Item("122").Enabled = True
            aForm.Items.Item("1000033").Enabled = True
            aForm.Items.Item("1000023").Enabled = True
            aForm.Items.Item("111").Enabled = True
            aForm.Items.Item("115").Enabled = True
            aForm.Items.Item("119").Enabled = True
            aForm.Items.Item("1000031").Enabled = True
            aForm.Items.Item("1000027").Enabled = True
            aForm.Items.Item("123").Enabled = True
            aForm.Items.Item("1000035").Enabled = True
            aForm.Items.Item("58").Enabled = True
            aForm.Items.Item("60").Enabled = True
            aForm.Items.Item("61").Enabled = True
            aForm.Items.Item("66").Enabled = True
        End If
    End Sub
  
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_HR_Hiring Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1" And (oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Or oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE) Then
                                    If Validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.GetForm(pVal.FormTypeEx, pVal.FormTypeCount)
                                'If pVal.ItemUID = "57" And pVal.Row > 0 Then
                                '    oMatrix = oForm.Items.Item("57").Specific
                                '    Me.RowtoDelete = pVal.Row
                                '    intSelectedMatrixrow = pVal.Row
                                '    Me.MatrixId = "57"
                                '    frmSourceMatrix = oMatrix
                                'End If
                                If pVal.ItemUID = "58" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("58").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "58"
                                    frmSourceMatrix = oMatrix
                                End If
                                If pVal.ItemUID = "60" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("60").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "60"
                                    frmSourceMatrix = oMatrix
                                End If
                                If pVal.ItemUID = "61" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("61").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "61"
                                    frmSourceMatrix = oMatrix
                                End If
                                If pVal.ItemUID = "62" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("62").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "62"
                                    frmSourceMatrix = oMatrix
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "126" And pVal.CharPressed = 9 Then
                                    Dim probmonth As Double
                                    Dim Probdate, joindt As Date
                                    If oApplication.Utilities.getEdittextvalue(oForm, "89") <> "" Then
                                        joindt = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(oForm, "89"))
                                        probmonth = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(oForm, "126"))
                                        Probdate = DateAdd(DateInterval.Month, probmonth, joindt)
                                        oApplication.Utilities.setEdittextvalue(oForm, "128", Probdate)
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1000033" Then
                                    oCombobox = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcountry As String
                                    strcountry = oCombobox.Selected.Value
                                    FillState1(oForm, strcountry)
                                End If
                                If pVal.ItemUID = "1000035" Then
                                    oCombobox1 = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcountry As String
                                    strcountry = oCombobox1.Selected.Value
                                    FillState2(oForm, strcountry)
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                Select Case pVal.ItemUID
                                    Case "1000005"
                                        oForm.PaneLevel = 1
                                    Case "26"
                                        oForm.PaneLevel = 2
                                    Case "1000006"
                                        oForm.PaneLevel = 3
                                    Case "28"
                                        oForm.PaneLevel = 4
                                    Case "29"
                                        oForm.PaneLevel = 5
                                    Case "30"
                                        oForm.PaneLevel = 6
                                    Case "1000010"
                                        oForm.PaneLevel = 7
                                    Case "71"
                                        Dim Strcode As String
                                        Strcode = oApplication.Utilities.getEdittextvalue(oForm, "4")
                                        If oApplication.SBO_Application.MessageBox("Do you want to Convert the Applicant to Employee ?", , "Yes", "No") = 2 Then
                                            Exit Sub
                                        End If
                                        If Validation(oForm) = True Then
                                            If EmployeeCreation(oForm, Strcode) = True Then
                                                oApplication.Utilities.Message("Operation Completed Successfully...", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                                oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
                                                oForm.Items.Item("4").Enabled = True
                                                oApplication.Utilities.setEdittextvalue(oForm, "4", Strcode)
                                                oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                                EnableDisable(oForm, "H")
                                                oForm.Items.Item("93").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                            Else
                                                BubbleEvent = False
                                                Exit Sub
                                            End If
                                        Else
                                            BubbleEvent = False
                                            Exit Sub
                                        End If

                                    Case "67"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_ADD_ROW)
                                    Case "68"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_DELETE_ROW)
                                    Case "65"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            Exit Sub
                                        End If
                                        deleterow(oForm)
                                        RefereshDeleteRow(oForm)
                                    Case "64"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            Exit Sub
                                        End If
                                        LoadFiles(oForm)
                                    Case "63"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            Exit Sub

                                        End If
                                        fillopen()
                                        If strSelectedFilepath <> "" Then


                                            oMatrix = oForm.Items.Item("62").Specific
                                            AddRow(oForm)
                                            Try
                                                oForm.Freeze(True)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, strSelectedFilepath)
                                                Dim strDate As String
                                                Dim dtdate As Date
                                                dtdate = Now.Date
                                                strDate = Date.Today().ToString
                                                ''  strdate=
                                                Dim oColumn As SAPbouiCOM.Column
                                                oColumn = oMatrix.Columns.Item("V_1")
                                                ' oColumn.Editable = True
                                                oColumn.Editable = True
                                                oEditText = oMatrix.Columns.Item("V_1").Cells.Item(oMatrix.RowCount).Specific
                                                oEditText.String = "t"
                                                oApplication.SBO_Application.SendKeys("{TAB}")
                                                oForm.Items.Item("10").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                                Try
                                                    oColumn.Editable = False
                                                Catch ex As Exception
                                                End Try

                                                'oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, dtdate)
                                                If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                End If
                                                oForm.Freeze(False)
                                            Catch ex As Exception
                                                oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                                oForm.Freeze(False)

                                            End Try
                                        End If
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

                                        If pVal.ItemUID = "90" Then
                                            val = oDataTable.GetValue("U_Z_SalCode", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "90", val)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        oForm.Freeze(False)
                                    End If
                                Catch ex As Exception
                                    'oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
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
                Case mnu_hr_Hiring
                    'LoadForm(oForm)
                Case mnu_ADD_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        AddRow(oForm)
                    End If
                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        RefereshDeleteRow(oForm)
                    Else

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
