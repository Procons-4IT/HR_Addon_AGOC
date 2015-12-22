Public Class clshrLeaveRequest
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
    Dim frdate, todate As String
    Dim frdt, todt, Rejoindt As Date
    Dim strQuery As String
    Dim oRecSet As SAPbobsCOM.Recordset
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Public Sub LoadForm(ByVal EmpId As String, ByVal EmpName As String)
        Try
            If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_LveRequest) = False Then
                oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Exit Sub
            End If
            oForm = oApplication.Utilities.LoadForm(xml_hr_LveRequest, frm_hr_LveRequest)
            oForm = oApplication.SBO_Application.Forms.ActiveForm()
            oForm.Freeze(True)
            oApplication.Utilities.setEdittextvalue(oForm, "4", EmpId)
            oApplication.Utilities.setEdittextvalue(oForm, "6", EmpName)
            oForm.DataSources.UserDataSources.Add("Frdt", SAPbouiCOM.BoDataType.dt_DATE)
            oApplication.Utilities.setUserDatabind(oForm, "10", "Frdt")
            oForm.DataSources.UserDataSources.Add("Todt", SAPbouiCOM.BoDataType.dt_DATE)
            oApplication.Utilities.setUserDatabind(oForm, "12", "Todt")
            oForm.DataSources.UserDataSources.Add("Rjdt", SAPbouiCOM.BoDataType.dt_DATE)
            oApplication.Utilities.setUserDatabind(oForm, "16", "Rjdt")
            oForm.DataSources.UserDataSources.Add("Bal", SAPbouiCOM.BoDataType.dt_QUANTITY)
            oApplication.Utilities.setUserDatabind(oForm, "23", "Bal")
            FillLeaveType(oForm)
            oForm.PaneLevel = 1
            oForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
    Public Sub ViewLoadForm(ByVal EmpId As String, ByVal EmpName As String)
        Try
            If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_LveRequest) = False Then
                oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Exit Sub
            End If
            oForm = oApplication.Utilities.LoadForm(xml_hr_LveRequest, frm_hr_LveRequest)
            oForm = oApplication.SBO_Application.Forms.ActiveForm()
            oForm.Freeze(True)
            oApplication.Utilities.setEdittextvalue(oForm, "4", EmpId)
            oApplication.Utilities.setEdittextvalue(oForm, "6", EmpName)
            oForm.DataSources.UserDataSources.Add("Frdt", SAPbouiCOM.BoDataType.dt_DATE)
            oApplication.Utilities.setUserDatabind(oForm, "10", "Frdt")
            oForm.DataSources.UserDataSources.Add("Todt", SAPbouiCOM.BoDataType.dt_DATE)
            oApplication.Utilities.setUserDatabind(oForm, "12", "Todt")
            oForm.DataSources.UserDataSources.Add("Rjdt", SAPbouiCOM.BoDataType.dt_DATE)
            oApplication.Utilities.setUserDatabind(oForm, "16", "Rjdt")
            FillLeaveType(oForm)
            oForm.PaneLevel = 2
            DataBind(oForm, EmpId)
            oForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
  
#Region "Methods"
    Private Sub DataBind(ByVal aform As SAPbouiCOM.Form, ByVal EmpId As String)
        Try
            oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oGrid = aform.Items.Item("20").Specific
            strQuery = "Select T0.""Code"" as ""Code"",""U_Z_TrnsCode"",T1.""Name"" as ""Name"",convert(varchar(10),""U_Z_StartDate"",103) AS ""U_Z_StartDate"","
            strQuery += " convert(varchar(10),""U_Z_EndDate"",103) AS ""U_Z_EndDate"" ,T0.""U_Z_NoofDays"",""U_Z_Notes"",convert(varchar(10),"
            strQuery += " ""U_Z_ReJoiNDate"",103) AS ""U_Z_ReJoiNDate"",case ""U_Z_Status"" when 'P' then 'Pending' when 'R' then 'Rejected' "
            strQuery += " when 'A' then 'Approved' end as ""U_Z_Status"",""U_Z_AppRemarks"" from ""@Z_PAY_OLETRANS1"" T0 inner join ""@Z_PAY_LEAVE"" T1 on T0.""U_Z_TrnsCode""=T1.""Code"" where ""U_Z_EMPID""='" & EmpId & "' and ""U_Z_TransType""='L' order by T0.""Code"" Desc"
            oGrid.DataTable.ExecuteQuery(strQuery)
            FormatGrid(oGrid)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub
    Private Sub FormatGrid(ByVal aGrid As SAPbouiCOM.Grid)
        aGrid.Columns.Item("Code").TitleObject.Caption = "Request Code"
        oEditTextColumn = aGrid.Columns.Item("Code")
        oEditTextColumn.LinkedObjectType = "Z_HR_OEXFOM"
        aGrid.Columns.Item("U_Z_TrnsCode").Visible = False
        aGrid.Columns.Item("Name").TitleObject.Caption = "Leave Type"
        aGrid.Columns.Item("U_Z_StartDate").TitleObject.Caption = "From Date"
        aGrid.Columns.Item("U_Z_EndDate").TitleObject.Caption = "To Date"
        aGrid.Columns.Item("U_Z_NoofDays").TitleObject.Caption = "No.of Days"
        aGrid.Columns.Item("U_Z_Notes").TitleObject.Caption = "Remarks"
        aGrid.Columns.Item("U_Z_ReJoiNDate").TitleObject.Caption = "Rejoin Date"
        aGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Status"
        aGrid.Columns.Item("U_Z_AppRemarks").TitleObject.Caption = "Approver Remarks"
        aGrid.Columns.Item("U_Z_AppRemarks").Visible = False
        aGrid.AutoResizeColumns()
        aGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
    Private Sub FillLeaveType(ByVal sform As SAPbouiCOM.Form)
        Dim strEmpID As String = oApplication.Utilities.getEdittextvalue(sform, "4")
        Dim oSlpRS, oRecS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRecS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRecS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Try
            oRecS.DoQuery("Select isnull(U_Z_Terms,'') from OHEM where empID=" & strEmpID)
            If oRecS.Fields.Item(0).Value = "" Then
                strSQL = "Select ""Code"",""Name"" from ""@Z_PAY_LEAVE"" order by ""Code"""
            Else
                strSQL = " Select ""U_Z_LeaveCode"" 'Leave Code',""Name""  from  ""@Z_PAY_OALMP"" T1 inner join ""@Z_PAY_LEAVE"" T0 on T0.""Code""=T1.""U_Z_LeaveCode""  where ""U_Z_Terms""='" & oRecS.Fields.Item(0).Value & "'"
            End If
        Catch ex As Exception
            strSQL = "Select ""Code"",""Name"" from ""@Z_PAY_LEAVE"" order by ""Code"""
        End Try
       

        oCombobox = sform.Items.Item("8").Specific
        Try
            oSlpRS.DoQuery(strSQL)
        Catch ex As Exception
            oSlpRS.DoQuery("Select ""Code"",""Name"" from ""@Z_PAY_LEAVE"" order by ""Code""")
        End Try
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        sform.Items.Item("8").DisplayDesc = True
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
    End Sub
    Public Function getNodays(ByVal frdate As Date, ByVal todate As Date) As String
        Try
            Dim strQuery, NoDays As String
            Dim oRec As SAPbobsCOM.Recordset
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strQuery = "select datediff(D,'" & frdate.ToString("yyyy/MM/dd") & "','" & todate.ToString("yyyy/MM/dd") & "')"
            oRec.DoQuery(strQuery)
            If oRec.RecordCount > 0 Then
                NoDays = oRec.Fields.Item(0).Value.ToString() + 1
            End If
            Return NoDays
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    Public Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim lvetype, Remark, Rejoin As String
            Dim Nodays As Integer
            frdate = oApplication.Utilities.getEdittextvalue(aForm, "10")
            todate = oApplication.Utilities.getEdittextvalue(aForm, "12")
            oCombobox = aForm.Items.Item("8").Specific
            lvetype = oCombobox.Selected.Value
            Remark = oApplication.Utilities.getEdittextvalue(aForm, "18")
            If oApplication.Utilities.getEdittextvalue(aForm, "14") <> "" Then
                Nodays = CInt(oApplication.Utilities.getEdittextvalue(aForm, "14"))
            End If
            Rejoin = oApplication.Utilities.getEdittextvalue(aForm, "16")
            Rejoindt = oApplication.Utilities.GetDateTimeValue(Rejoin)

            If lvetype = "" Then
                oApplication.Utilities.Message("Select Leave Type...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            ElseIf frdate = "" Then
                oApplication.Utilities.Message("Enter From date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            ElseIf todate = "" Then
                oApplication.Utilities.Message("Enter To date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            ElseIf Rejoin = "" Then
                oApplication.Utilities.Message("Enter Rejoin date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            ElseIf Nodays <= 0 Then
                oApplication.Utilities.Message("To date must be greater than or equal to from date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False

            ElseIf Remark = "" Then
                '  oApplication.Utilities.Message("Enter Remarks...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                ' Return False
            End If
            todt = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "12"))
            If todt >= Rejoindt Then
                oApplication.Utilities.Message("To date must be Less than or equal to Rejoin date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function

    Private Function AddToUDT(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim strTable, strCode As String
        Dim oUserTable, oUserTable1 As SAPbobsCOM.UserTable
        Try
            oCombobox = aForm.Items.Item("8").Specific
            oUserTable = oApplication.Company.UserTables.Item("Z_PAY_OLETRANS1")
            strTable = "@Z_PAY_OLETRANS1"

            If oUserTable.GetByKey(oApplication.Utilities.getEdittextvalue(aForm, "19")) Then
                strCode = oApplication.Utilities.getEdittextvalue(aForm, "19")
                oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = oApplication.Utilities.getEdittextvalue(aForm, "4")
                oUserTable.UserFields.Fields.Item("U_Z_EMPNAME").Value = oApplication.Utilities.getEdittextvalue(aForm, "6")
                oUserTable.UserFields.Fields.Item("U_Z_TransType").Value = "L"
                oUserTable.UserFields.Fields.Item("U_Z_TrnsCode").Value = oCombobox.Selected.Value
                oUserTable.UserFields.Fields.Item("U_Z_LeaveName").Value = oCombobox.Selected.Description
                oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "10"))
                oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "12"))
                oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(aForm, "14"))
                oUserTable.UserFields.Fields.Item("U_Z_Notes").Value = oApplication.Utilities.getEdittextvalue(aForm, "18")
                oUserTable.UserFields.Fields.Item("U_Z_ReJoiNDate").Value = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "16"))
                oUserTable.UserFields.Fields.Item("U_Z_Status").Value = oApplication.Utilities.DocApproval(aForm, HeaderDoctype.LveReq, oApplication.Utilities.getEdittextvalue(aForm, "4"), oCombobox.Selected.Value)
                Dim dtdate As Date = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "10"))
                oUserTable.UserFields.Fields.Item("U_Z_Year").Value = dtdate.Year
                oUserTable.UserFields.Fields.Item("U_Z_Month").Value = dtdate.Month
                oUserTable.UserFields.Fields.Item("U_Z_LevBal").Value = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(aForm, "23"))
                If oUserTable.Update <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    oApplication.Utilities.AddUDTPayroll(aForm, strCode)
                    Dim intTempID As String = oApplication.Utilities.GetTemplateID(aForm, HeaderDoctype.LveReq, oApplication.Utilities.getEdittextvalue(aForm, "4"))
                    If intTempID <> "0" Then
                        oApplication.Utilities.InitialMessage("Leave Request", strCode, oApplication.Utilities.DocApproval(aForm, HeaderDoctype.LveReq, oApplication.Utilities.getEdittextvalue(aForm, "4"), oCombobox.Selected.Value), intTempID, oApplication.Utilities.getEdittextvalue(aForm, "4"), HistoryDoctype.LveReq)
                    End If

                    End If
            Else
                    strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = oApplication.Utilities.getEdittextvalue(aForm, "4")
                    oUserTable.UserFields.Fields.Item("U_Z_EMPNAME").Value = oApplication.Utilities.getEdittextvalue(aForm, "6")
                    oUserTable.UserFields.Fields.Item("U_Z_TransType").Value = "L"
                    oUserTable.UserFields.Fields.Item("U_Z_TrnsCode").Value = oCombobox.Selected.Value
                    oUserTable.UserFields.Fields.Item("U_Z_LeaveName").Value = oCombobox.Selected.Description
                    oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "10"))
                    oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "12"))
                    oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(aForm, "14"))
                    oUserTable.UserFields.Fields.Item("U_Z_Notes").Value = oApplication.Utilities.getEdittextvalue(aForm, "18")
                    oUserTable.UserFields.Fields.Item("U_Z_ReJoiNDate").Value = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "16"))
                    oUserTable.UserFields.Fields.Item("U_Z_Status").Value = oApplication.Utilities.DocApproval(aForm, HeaderDoctype.LveReq, oApplication.Utilities.getEdittextvalue(aForm, "4"), oCombobox.Selected.Value)
                    Dim dtdate As Date = oApplication.Utilities.GetDateTimeValue(oApplication.Utilities.getEdittextvalue(aForm, "10"))
                    oUserTable.UserFields.Fields.Item("U_Z_Year").Value = dtdate.Year
                    oUserTable.UserFields.Fields.Item("U_Z_Month").Value = dtdate.Month
                    oUserTable.UserFields.Fields.Item("U_Z_LevBal").Value = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(aForm, "23"))
                    If oUserTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    Else
                    oApplication.Utilities.AddUDTPayroll(aForm, strCode)
                    Dim intTempID As String = oApplication.Utilities.GetTemplateID(aForm, HeaderDoctype.LveReq, oApplication.Utilities.getEdittextvalue(aForm, "4"))
                    If intTempID <> "0" Then
                        oApplication.Utilities.InitialMessage("Leave Request", strCode, oApplication.Utilities.DocApproval(aForm, HeaderDoctype.LveReq, oApplication.Utilities.getEdittextvalue(aForm, "4"), oCombobox.Selected.Value), intTempID, oApplication.Utilities.getEdittextvalue(aForm, "4"), HistoryDoctype.LveReq)
                    End If
                    End If
                End If
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
        oUserTable = Nothing
        Return True
    End Function
    Private Sub PopulateDetails(ByVal oForm As SAPbouiCOM.Form, ByVal strCode As String)
        Try
            oForm.Freeze(True)
            oCombobox = oForm.Items.Item("8").Specific
            oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strQuery = "Select T0.""Code"" as ""Code"",""U_Z_EMPID"",""U_Z_EMPNAME"",""U_Z_TrnsCode"",convert(varchar(10),"
            strQuery += " ""U_Z_StartDate"",103) AS ""U_Z_StartDate"",convert(varchar(10),""U_Z_EndDate"",103) AS ""U_Z_EndDate"" ,"
            strQuery += " T0.""U_Z_NoofDays"",""U_Z_Notes"",convert(varchar(10),""U_Z_ReJoiNDate"",103) AS ""U_Z_ReJoiNDate"",""U_Z_Status"",""U_Z_LevBal"" "
            strQuery += " from ""@Z_PAY_OLETRANS1"" T0 where  T0.""Code""='" & strCode.Trim() & "'"
            oRecSet.DoQuery(strQuery)
            If oRecSet.RecordCount > 0 Then
                oApplication.Utilities.setEdittextvalue(oForm, "19", oRecSet.Fields.Item("Code").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "4", oRecSet.Fields.Item("U_Z_EMPID").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "6", oRecSet.Fields.Item("U_Z_EMPNAME").Value)
                If oRecSet.Fields.Item("U_Z_TrnsCode").Value <> "" Then
                    oCombobox.Select(oRecSet.Fields.Item("U_Z_TrnsCode").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
                End If
                oApplication.Utilities.setEdittextvalue(oForm, "10", oRecSet.Fields.Item("U_Z_StartDate").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "12", oRecSet.Fields.Item("U_Z_EndDate").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "14", oRecSet.Fields.Item("U_Z_NoofDays").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "16", oRecSet.Fields.Item("U_Z_ReJoiNDate").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "18", oRecSet.Fields.Item("U_Z_Notes").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "23", oRecSet.Fields.Item("U_Z_LevBal").Value)
            End If
            oForm.PaneLevel = 1
            If oRecSet.Fields.Item("U_Z_Status").Value <> "P" Then
                oForm.Items.Item("3").Visible = False
            Else
                oForm.Items.Item("3").Visible = True
            End If
            oForm.Items.Item("20").Visible = False
            oForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub

    Public Sub ViewPopulateDetails(ByVal oForm As SAPbouiCOM.Form, ByVal strCode As String)
        Try
            oForm = oApplication.Utilities.LoadForm(xml_hr_LveRequest, frm_hr_LveRequest)
            oForm = oApplication.SBO_Application.Forms.ActiveForm()
            oForm.Freeze(True)
            oForm.DataSources.UserDataSources.Add("Frdt", SAPbouiCOM.BoDataType.dt_DATE)
            oApplication.Utilities.setUserDatabind(oForm, "10", "Frdt")
            oForm.DataSources.UserDataSources.Add("Todt", SAPbouiCOM.BoDataType.dt_DATE)
            oApplication.Utilities.setUserDatabind(oForm, "12", "Todt")
            oForm.DataSources.UserDataSources.Add("Rjdt", SAPbouiCOM.BoDataType.dt_DATE)
            oApplication.Utilities.setUserDatabind(oForm, "16", "Rjdt")
            FillLeaveType(oForm)
            oCombobox = oForm.Items.Item("8").Specific
            oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strQuery = "Select T0.""Code"" as ""Code"",""U_Z_EMPID"",""U_Z_EMPNAME"",""U_Z_TrnsCode"",convert(varchar(10),"
            strQuery += " ""U_Z_StartDate"",103) AS ""U_Z_StartDate"",convert(varchar(10),""U_Z_EndDate"",103) AS ""U_Z_EndDate"" ,"
            strQuery += " T0.""U_Z_NoofDays"",""U_Z_Notes"",convert(varchar(10),""U_Z_ReJoiNDate"",103) AS ""U_Z_ReJoiNDate"",""U_Z_Status"" "
            strQuery += " from ""@Z_PAY_OLETRANS1"" T0 where  T0.""Code""='" & strCode.Trim() & "'"
            oRecSet.DoQuery(strQuery)
            If oRecSet.RecordCount > 0 Then
                oApplication.Utilities.setEdittextvalue(oForm, "19", oRecSet.Fields.Item("Code").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "4", oRecSet.Fields.Item("U_Z_EMPID").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "6", oRecSet.Fields.Item("U_Z_EMPNAME").Value)
                If oRecSet.Fields.Item("U_Z_TrnsCode").Value <> "" Then
                    oCombobox.Select(oRecSet.Fields.Item("U_Z_TrnsCode").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
                End If
                oApplication.Utilities.setEdittextvalue(oForm, "10", oRecSet.Fields.Item("U_Z_StartDate").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "12", oRecSet.Fields.Item("U_Z_EndDate").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "14", oRecSet.Fields.Item("U_Z_NoofDays").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "16", oRecSet.Fields.Item("U_Z_ReJoiNDate").Value)
                oApplication.Utilities.setEdittextvalue(oForm, "18", oRecSet.Fields.Item("U_Z_Notes").Value)
            End If
            oForm.PaneLevel = 1
            If oRecSet.Fields.Item("U_Z_Status").Value <> "P" Then
                oForm.Items.Item("3").Visible = False
            Else
                oForm.Items.Item("3").Visible = True
            End If
            oForm.Items.Item("20").Visible = False
            oForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub

#End Region

    Private Sub GetLeaveBalance(ByVal aform As SAPbouiCOM.Form)
        Dim oRateRS As SAPbobsCOM.Recordset
        Dim dblbasic As Double
        Dim ayear As Integer
        Dim dtDate As Date
        Dim strEMpID, strStartDate, strLeaveCode As String
        strEMpID = oApplication.Utilities.getEdittextvalue(aform, "4")
        strStartDate = oApplication.Utilities.getEdittextvalue(aform, "10")
        oCombobox = aform.Items.Item("8").Specific
        Try
            strLeaveCode = oCombobox.Selected.Value
        Catch ex As Exception
            strLeaveCode = "XXX"
        End Try
        If strStartDate = "" Then
            ayear = 1920 'Now.Year
        Else
            dtDate = oApplication.Utilities.GetDateTimeValue(strStartDate)
            ayear = dtDate.Year
        End If
        oRateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Try
            oRateRS.DoQuery("select isnull(U_Z_Balance,0) from [@Z_EMP_LEAVE_BALANCE] where U_Z_Year=" & ayear & " and U_Z_EmpID='" & strEMpID & "' and U_Z_LeaveCode='" & strLeaveCode & "'")
            dblbasic = oRateRS.Fields.Item(0).Value
        Catch ex As Exception
            dblbasic = 0
        End Try
        oApplication.Utilities.setEdittextvalue(aform, "23", dblbasic)
    End Sub
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_LveRequest Then
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
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "20" And pVal.ColUID = "Code" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcode As String = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    PopulateDetails(oForm, strcode)
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If (pVal.ItemUID = "10" Or pVal.ItemUID = "12") And pVal.CharPressed = 9 Then
                                    frdate = oApplication.Utilities.getEdittextvalue(oForm, "10")
                                    todate = oApplication.Utilities.getEdittextvalue(oForm, "12")
                                    If frdate <> "" And todate <> "" Then
                                        frdt = oApplication.Utilities.GetDateTimeValue(frdate)
                                        todt = oApplication.Utilities.GetDateTimeValue(todate)
                                        oApplication.Utilities.setEdittextvalue(oForm, "14", getNodays(frdt, todt))

                                    End If
                                    GetLeaveBalance(oForm)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_LOST_FOCUS
                                'If pVal.ItemUID = "12" Then
                                '    frdate = oApplication.Utilities.getEdittextvalue(oForm, "10")
                                '    todate = oApplication.Utilities.getEdittextvalue(oForm, "12")
                                '    If frdate <> "" And todate <> "" Then
                                '        frdt = oApplication.Utilities.GetDateTimeValue(frdate)
                                '        todt = oApplication.Utilities.GetDateTimeValue(todate)
                                '        oApplication.Utilities.setEdittextvalue(oForm, "14", getNodays(frdt, todt))
                                '        oApplication.Utilities.setEdittextvalue(oForm, "16", todt.AddDays(1))
                                '    End If
                                'End If
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "8" Then
                                    GetLeaveBalance(oForm)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "21" Then
                                    Dim objHistory As New clshrAppHisDetails
                                    objHistory.LoadForm(oForm, HistoryDoctype.LveReq, oApplication.Utilities.getEdittextvalue(oForm, "19"))
                                End If
                                If pVal.ItemUID = "3" Then
                                    If AddToUDT(oForm) = True Then
                                        oApplication.Utilities.Message("Operation Completed Successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                        oForm.Close()
                                    Else
                                        BubbleEvent = False
                                        Exit Sub
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

#Region "Menu Event"
    Public Overrides Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            Select Case pVal.MenuUID
                Case mnu_HR_LveRequest
                    Dim oTe As New clshrLogin
                    oTe.LoadForm("LVEREQ")
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
