Public Class clshrSearch
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox, oCombobox1, oCombobox2 As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oCheckColumn As SAPbouiCOM.CheckBoxColumn
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
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_HR_Search1) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_Search, frm_HR_Search1)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        FillDepartment(oForm)
        FillCountry(oForm)
        FillEducationType(oForm)
        AddChooseFromList(oForm)
        oForm.DataSources.UserDataSources.Add("Reqno", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "20", "Reqno")
        oEditText = oForm.Items.Item("20").Specific
        oEditText.ChooseFromListUID = "CFL2"
        oEditText.ChooseFromListAlias = "DocEntry"
        oForm.DataSources.UserDataSources.Add("major", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "40", "major")
        oForm.DataSources.UserDataSources.Add("Position", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "42", "Position")
        oForm.DataSources.UserDataSources.Add("Posno", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "1000005", "Posno")
        oForm.DataSources.UserDataSources.Add("min", SAPbouiCOM.BoDataType.dt_PRICE)
        oApplication.Utilities.setUserDatabind(oForm, "1000007", "min")
        oForm.DataSources.UserDataSources.Add("max", SAPbouiCOM.BoDataType.dt_PRICE)
        oApplication.Utilities.setUserDatabind(oForm, "32", "max")
        oForm.DataSources.UserDataSources.Add("sex", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDSCombobox(oForm, "36", "sex")
        oForm.DataSources.UserDataSources.Add("MainSkill", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "52", "MainSkill")
        oCombobox = oForm.Items.Item("36").Specific
        oCombobox.ValidValues.Add("", "")
        oCombobox.ValidValues.Add("M", "Male")
        oCombobox.ValidValues.Add("F", "Female")
        oForm.Items.Item("36").DisplayDesc = True
        oForm.PaneLevel = 1
        Dim osta As SAPbouiCOM.StaticText
        osta = oForm.Items.Item("19").Specific
        osta.Caption = "Step " & oForm.PaneLevel & " of 3"
        oForm.Items.Item("19").TextStyle = SAPbouiCOM.BoTextStyle.ts_BOLD
        reDrawForm(oForm)
        oForm.Freeze(False)
    End Sub
    Private Sub FillDepartment(ByVal sform As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = sform.Items.Item("7").Specific
        oCombobox1 = sform.Items.Item("16").Specific
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        For intRow As Integer = oCombobox1.ValidValues.Count - 1 To 0 Step -1
            oCombobox1.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oCombobox1.ValidValues.Add("", "")
        oTempRec.DoQuery("Select Code,Remarks from OUDP")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item("Code").Value, oTempRec.Fields.Item("Remarks").Value)
            oCombobox1.ValidValues.Add(oTempRec.Fields.Item("Code").Value, oTempRec.Fields.Item("Remarks").Value)
            oTempRec.MoveNext()
        Next
        sform.Items.Item("7").DisplayDesc = True
        sform.Items.Item("16").DisplayDesc = True
    End Sub
    Private Sub FillCountry(ByVal sform As SAPbouiCOM.Form)
        oCombobox = sform.Items.Item("34").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select Code,Name from OCRY order by Code")
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        sform.Items.Item("34").DisplayDesc = True
    End Sub
    Private Sub FillEducationType(ByVal sform As SAPbouiCOM.Form)
        oCombobox = sform.Items.Item("38").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("select edType,name  from OHED  order by edType")
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        sform.Items.Item("38").DisplayDesc = True
    End Sub

    Private Sub Databind(ByVal aform As SAPbouiCOM.Form)
        aform.DataSources.UserDataSources.Add("20", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        aform.Items.Item("20").Specific.Databind.SetBOund(True, "", )
        oEditText = aform.Items.Item("20").Specific
        oEditText.ChooseFromListUID = "CFL2"
        oEditText.ChooseFromListAlias = "DocEntry"
    End Sub
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
            oCFLCreationParams.ObjectType = "Z_HR_ORREQS"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "U_Z_AppStatus"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "A"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()


            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_ORREQS"
            oCFLCreationParams.UniqueID = "CFL2"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "U_Z_AppStatus"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "A"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()
        
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub Gridbind(ByVal aForm As SAPbouiCOM.Form)
        Dim strqry, strQry1, strminexp, strmaxexp, strnational, strsex, strReqno, strEducation, strmajor, strposition, strMainSkill As String
        Dim strReqCondition, strExpcondition, strNaticondition, strsexcondition, strCondition, streducondition, strmjrCondition, strposcondition, strMainSkillCondition As String

        strminexp = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(aForm, "1000007"))
        strmaxexp = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(aForm, "32"))
        strReqno = oApplication.Utilities.getEdittextvalue(aForm, "18")
        strmajor = oApplication.Utilities.getEdittextvalue(aForm, "40")
        strposition = oApplication.Utilities.getEdittextvalue(aForm, "42")
        strMainSkill = oApplication.Utilities.getEdittextvalue(aForm, "52")

        oCombobox = aForm.Items.Item("34").Specific
        strnational = oCombobox.Selected.Value

        oCombobox1 = aForm.Items.Item("36").Specific
        strsex = oCombobox1.Selected.Value

        oCombobox2 = aForm.Items.Item("38").Specific
        strEducation = oCombobox2.Selected.Value

        If strReqno <> "" Then
            strReqCondition = "T0.U_Z_RequestCode='" & strReqno & "'"
            ' strReqCondition = "U_Z_RequestCode not in ( Select * from [@Z_HR_ORMPREQ] where DocEntry='" & strReqno & "' and U_Z_MgrStatus='HA')"

        Else
            strReqCondition = "1=1"
        End If
        If strminexp <> 0 And strmaxexp <> 0 Then
            strExpcondition = "T0.U_Z_YrExp between '" & strminexp & "' and '" & strmaxexp & "'"
        ElseIf strminexp <> 0 And strmaxexp = 0 Then
            strExpcondition = "T0.U_Z_YrExp >= '" & strminexp & "'"
        ElseIf strminexp = 0 And strmaxexp <> 0 Then
            strExpcondition = "T0.U_Z_YrExp <= '" & strmaxexp & "'"
        Else
            strExpcondition = " 1=1"
        End If
        If strnational <> "" Then
            strNaticondition = "T0.U_Z_Nationality='" & strnational & "'"
        Else
            strNaticondition = "1=1"
        End If
        If strsex <> "" Then
            strsexcondition = "T0.U_Z_Sex='" & strsex & "'"
        Else
            strsexcondition = "1=1"
        End If
        If strEducation <> "" Then
            streducondition = "T2.U_Z_Level='" & strEducation & "'"
        Else
            streducondition = "1=1"
        End If

        If strmajor <> "" Then
            strmjrCondition = "(T2.U_Z_Major like '%" & strmajor & "%' or T2.U_Z_Major like '" & strmajor & "%')"
        Else
            strmjrCondition = "1=1"
        End If

        If strposition <> "" Then
            strposcondition = "T3.U_Z_PrPosition like '%" & strposition & "%'"
        Else
            strposcondition = "1=1"
        End If

        If strMainSkill <> "" Then
            strMainSkillCondition = "T0.U_Z_Skills like '%" & strMainSkill & "%'"
        Else
            strMainSkillCondition = "1=1"
        End If

        '  strCondition = strReqCondition & " and " & strExpcondition & " and " & strNaticondition & " and " & strsexcondition
        strCondition = strReqCondition & " and " & strExpcondition & " and " & strNaticondition & " and " & strsexcondition & " and " & streducondition & " And " & strmjrCondition & " And " & strposcondition & " and " & strMainSkillCondition
        oGrid = aForm.Items.Item("10").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
        oCombobox = aForm.Items.Item("7").Specific
        strQry1 = "Select U_Z_HRAPPID from [@Z_HR_OHEM1] where U_Z_Dept='" & oCombobox.Selected.Value & "' and U_Z_ReqNo='" & oApplication.Utilities.getEdittextvalue(aForm, "20") & "'"
        strqry = "select distinct(''),T0.U_Z_RequestCode,T0.DocEntry,U_Z_FirstName,T0.U_Z_Dob,T0.U_Z_Mobile ,T0.U_Z_EmailId,T0.U_Z_YrExp,T0.U_Z_AppDate,"
        strqry = strqry & " U_Z_Skills from  [@Z_HR_OCRAPP] T0 inner join [@Z_HR_CRAPP6] T1 on T0.DocEntry=T1.DocEntry inner join [@Z_HR_CRAPP3] T2 on T0.DocEntry=T2.DocEntry inner join [@Z_HR_CRAPP4] T3 on T0.DocEntry=T3.DocEntry "
        ' strqry = strqry & " where U_Z_Status='R' and  T1.U_Z_PosCode='" & oApplication.Utilities.getEdittextvalue(aForm, "30") & "' and  T0.DocEntry not in (" & strQry1 & ") and " & strCondition
        strqry = strqry & " where U_Z_Status='R' and " & strCondition ' T0.DocEntry not in (" & strQry1 & ") and " & strCondition

        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item(0).TitleObject.Caption = "Select"
        oGrid.Columns.Item(0).Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        oGrid.Columns.Item(0).Editable = True
        oGrid.Columns.Item("U_Z_RequestCode").TitleObject.Caption = "Request Code"
        oGrid.Columns.Item("U_Z_RequestCode").Editable = True
        oEditTextColumn = oGrid.Columns.Item("U_Z_RequestCode")
        oEditTextColumn.ChooseFromListUID = "CFL1"
        oEditTextColumn.ChooseFromListAlias = "DocEntry"
        oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Applicant Id"
        oGrid.Columns.Item("DocEntry").Editable = False
        oEditTextColumn = oGrid.Columns.Item("DocEntry")
        'oEditTextColumn = oGrid.Columns.Item("DocEntry")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
        oGrid.Columns.Item("U_Z_FirstName").TitleObject.Caption = "Applicant Name"
        oGrid.Columns.Item("U_Z_FirstName").Editable = False
        oGrid.Columns.Item("U_Z_EmailId").TitleObject.Caption = "Email Id"
        oGrid.Columns.Item("U_Z_EmailId").Editable = False
        oGrid.Columns.Item("U_Z_Mobile").TitleObject.Caption = "Mobile No "
        oGrid.Columns.Item("U_Z_Mobile").Editable = False
        oGrid.Columns.Item("U_Z_Dob").TitleObject.Caption = "Date of Birth"
        oGrid.Columns.Item("U_Z_Dob").Editable = False
        oGrid.Columns.Item("U_Z_YrExp").TitleObject.Caption = "Year of Experience"
        oGrid.Columns.Item("U_Z_YrExp").Editable = False
        oGrid.Columns.Item("U_Z_AppDate").TitleObject.Caption = "Applicant Date"
        oGrid.Columns.Item("U_Z_AppDate").Editable = False
        oGrid.Columns.Item("U_Z_Skills").TitleObject.Caption = "Skill Sets"
        oGrid.Columns.Item("U_Z_Skills").Editable = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        Gridbind1()
    End Sub
    Private Sub Gridbind1()
        Dim strqry As String
        oGrid = oForm.Items.Item("25").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_1")
        strqry = "select U_Z_HRAppID,U_Z_HRAppName,U_Z_DeptName,U_Z_ReqNo,U_Z_Dob,U_Z_Email,U_Z_Mobile,U_Z_YrExp,U_Z_Skills,"
        strqry = strqry & " case U_Z_ApplStatus when 'O' then 'Open' when 'S' then 'Shortlisted' "
        strqry = strqry & " when 'R' then 'Rejected' when 'A' then 'Approved' end U_Z_ApplStatus,case U_Z_IntervStatus when 'O' then 'Open' when 'A' then 'Accepted'"
        strqry = strqry & " when 'R' then 'Rejected' when 'F' then 'Job Offering' when 'P' then 'Placement' end as U_Z_IntervStatus from [@Z_HR_OHEM1]"  ' where U_Z_JobPosiCode='" & oApplication.Utilities.getEdittextvalue(oForm, "30") & "' and U_Z_Dept='" & dept & "'"
        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("U_Z_HRAppID").TitleObject.Caption = "Applicant Id"
        oGrid.Columns.Item("U_Z_HRAppID").Editable = False
        oEditTextColumn = oGrid.Columns.Item("U_Z_HRAppID")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
        oGrid.Columns.Item("U_Z_HRAppName").TitleObject.Caption = "Applicant Name"
        oGrid.Columns.Item("U_Z_HRAppName").Editable = False
        oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department"
        oGrid.Columns.Item("U_Z_DeptName").Editable = False
        oGrid.Columns.Item("U_Z_ReqNo").TitleObject.Caption = "Job Request No"
        oGrid.Columns.Item("U_Z_ReqNo").Editable = False
        oEditTextColumn = oGrid.Columns.Item("U_Z_ReqNo")
        oEditTextColumn.LinkedObjectType = "Z_HR_ORREQS"
        oGrid.Columns.Item("U_Z_Email").TitleObject.Caption = "Email Id"
        oGrid.Columns.Item("U_Z_Email").Editable = False
        oGrid.Columns.Item("U_Z_Mobile").TitleObject.Caption = "Mobile No "
        oGrid.Columns.Item("U_Z_Mobile").Editable = False
        oGrid.Columns.Item("U_Z_Dob").TitleObject.Caption = "Date of Birth"
        oGrid.Columns.Item("U_Z_Dob").Editable = False
        oGrid.Columns.Item("U_Z_YrExp").TitleObject.Caption = "Year of Experience"
        oGrid.Columns.Item("U_Z_YrExp").Editable = False
        oGrid.Columns.Item("U_Z_Skills").TitleObject.Caption = "Skill Sets"
        oGrid.Columns.Item("U_Z_Skills").Editable = False
        oGrid.Columns.Item("U_Z_ApplStatus").TitleObject.Caption = "Interview Status"
        oGrid.Columns.Item("U_Z_ApplStatus").Editable = False
        oGrid.Columns.Item("U_Z_IntervStatus").TitleObject.Caption = "Applicant Status"
        oGrid.Columns.Item("U_Z_IntervStatus").Editable = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
    End Sub
    Private Sub Selectall(ByVal aForm As SAPbouiCOM.Form, ByVal blnValue As Boolean)
        Dim ocheckboxcolumn As SAPbouiCOM.CheckBoxColumn
        Dim ovalue As SAPbouiCOM.ValidValue
        oGrid = aForm.Items.Item("10").Specific
        aForm.Freeze(True)
        For introw As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            ocheckboxcolumn = oGrid.Columns.Item(0)
            ocheckboxcolumn.Check(introw, blnValue)
        Next
        aForm.Freeze(False)
    End Sub
    
    Private Sub LoadFiles(ByVal aform As SAPbouiCOM.Form)
        oGrid = aform.Items.Item("10").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.Rows.IsSelected(intRow) Then
                Dim strFilename As String
                strFilename = oGrid.DataTable.GetValue("U_Z_FileName", intRow)
                Dim x As System.Diagnostics.ProcessStartInfo
                x = New System.Diagnostics.ProcessStartInfo
                x.UseShellExecute = True
                x.FileName = strFilename
                System.Diagnostics.Process.Start(x)
                x = Nothing
                Exit Sub
            End If
        Next
    End Sub

#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim strDept, Reqno, posname As String
            oCombobox = oForm.Items.Item("7").Specific
            strDept = oCombobox.Selected.Description
            Reqno = oApplication.Utilities.getEdittextvalue(aForm, "20")
            posname = oApplication.Utilities.getEdittextvalue(aForm, "1000003")
         
            'If Reqno = "" Then
            '    oApplication.Utilities.Message("Job Request No is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '    Return False
            'End If
          
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function

    Private Function RestoreSelections(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim strDept, Reqno, strsql As String
            oCombobox = oForm.Items.Item("7").Specific
            strDept = oCombobox.Selected.Value
            Reqno = oApplication.Utilities.getEdittextvalue(aForm, "20")
            strsql = "Delete from [@Z_HR_OHEM1] where U_Z_Dept='" & strDept & "'  and U_Z_ReqNo='" & Reqno & "' and Name<>Code"
            oTest.DoQuery(strsql)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
    Private Function CommitSelections(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim strDept, Reqno, strsql As String
            oCombobox = oForm.Items.Item("7").Specific
            strDept = oCombobox.Selected.Value
            Reqno = oApplication.Utilities.getEdittextvalue(aForm, "20")
            oGrid = aForm.Items.Item("10").Specific
            '    strTable = "@Z_HR_HEM1"
            Dim strTable, strReqno, strCode, strType, strAppcode, strqry, strDeptcode, strStatus As String
            Dim strcount As Integer
            Dim dblValue As Double
            Dim dt As Date
            Dim oUserTable As SAPbobsCOM.UserTable
            Dim oValidateRS, otemp2 As SAPbobsCOM.Recordset
            Dim otemp, otemp1, otemprs As SAPbobsCOM.Recordset
            otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oUserTable = oApplication.Company.UserTables.Item("Z_HR_OHEM1")
            otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                '  oCheckColumn = oGrid.Columns.Item(0)
                If 1 = 1 Then
                    '  strAppcode = oGrid.DataTable.GetValue("DocEntry", intRow)
                    strCode = oGrid.DataTable.GetValue("Code", intRow)
                    If oUserTable.GetByKey(strCode) Then
                        strsql = "Update  [@Z_HR_OHEM1] set Name=Code where Code='" & strCode & "'"
                        oTest.DoQuery(strsql)
                        strAppcode = oUserTable.UserFields.Fields.Item("U_Z_HRAppID").Value
                        strsql = "Update [@Z_HR_OCRAPP] set U_Z_Status='S' where DocEntry='" & strAppcode & "' and (U_Z_Status='R') "
                        otemprs.DoQuery(strsql)
                        'strqry = "Update [@Z_HR_ORMPREQ] set U_Z_MgrStatus='N' where DocEntry='" & Reqno & "' "
                        'otemp.DoQuery(strqry)
                    End If
                End If
            Next

            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
#End Region

#Region "AddToUDT"
    Private Function ApplicantsShortlisted(ByVal aform As SAPbouiCOM.Form) As Boolean
        Try
            aform.Freeze(True)
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If
            oApplication.Company.StartTransaction()
            Dim oUserTable As SAPbobsCOM.UserTable
            oGrid = aform.Items.Item("10").Specific
            Dim oGeneralService, oGeneralService1 As SAPbobsCOM.GeneralService
            Dim oGeneralData, oGeneralData1 As SAPbobsCOM.GeneralData
            Dim oGeneralParams, oGeneralParams1 As SAPbobsCOM.GeneralDataParams
            Dim oCompanyService As SAPbobsCOM.CompanyService
            Dim oChildren, oChildren1, oChildren2 As SAPbobsCOM.GeneralDataCollection
            oCompanyService = oApplication.Company.GetCompanyService()
            Dim otestRs, oRec, oTemp1 As SAPbobsCOM.Recordset
            Dim oChild, oChild1, oChild2 As SAPbobsCOM.GeneralData
            Dim blnRecordExists As Boolean = False
            Dim strTable, strReqno, strType, strAppcode, strqry, strDeptcode, strStatus, strDept, strDeptName, strPosition As String
            'Get GeneralService (oCmpSrv is the CompanyService)
            oGeneralService = oCompanyService.GetGeneralService("Z_HR_OHEM")

            otestRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)
            ' oGeneralParams1 = oGeneralService1.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)

            Dim oCheckbox, ocheckbox1 As SAPbouiCOM.CheckBoxColumn
            Dim blnDownpayment As Boolean = False
            Dim blnDocumentItem As Boolean
            Dim ReStdate, reEndDate As Date
            Dim status As String
            Dim strempid, strempname, strposCode, strRequestno As String
            oCombobox1 = oForm.Items.Item("16").Specific
            strReqno = oApplication.Utilities.getEdittextvalue(aform, "18")
            strPosition = oApplication.Utilities.getEdittextvalue(aform, "1000003")
            strposCode = oApplication.Utilities.getEdittextvalue(aform, "30")
            strDept = oCombobox1.Selected.Value
            strDeptName = oCombobox1.Selected.Description
            intNumofCount = 0
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                blnDocumentItem = False
                oCheckbox = oGrid.Columns.Item(0)
                oGeneralData1 = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData)
                If oCheckbox.IsChecked(intRow) Then
                    strRequestno = oGrid.DataTable.GetValue("U_Z_RequestCode", intRow)
                    If strRequestno = "" Then
                        oApplication.Utilities.Message("Request Number is missing", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        aform.Freeze(False)
                        Return False
                    Else
                        Dim stSQL1 As String
                        stSQL1 = "Select * from [@Z_HR_ORMPREQ] where DocEntry='" & strRequestno & "' and (U_Z_AppStatus='C' or U_Z_AppStatus='L')"
                        oRec.DoQuery(stSQL1)
                        If oRec.RecordCount > 0 Then
                            oApplication.Utilities.Message("Request Number already closed :" & strRequestno, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            aform.Freeze(False)
                            Return False
                        Else
                            stSQL1 = "Select U_Z_DeptCode from [@Z_HR_ORMPREQ] where DocEntry='" & strRequestno & "'"
                            oRec.DoQuery(stSQL1)
                            status = oApplication.Utilities.DocApproval(aform, HeaderDoctype.Rec, oRec.Fields.Item(0).Value)
                        End If
                    End If

                    strempid = oGrid.DataTable.GetValue("DocEntry", intRow)
                    strempname = oGrid.DataTable.GetValue("U_Z_FirstName", intRow)
                    oGeneralData1.SetProperty("U_Z_ApplStatus", "S")
                    oGeneralData1.SetProperty("U_Z_HRAppID", strempid)
                    oGeneralData1.SetProperty("U_Z_HRAppName", strempname)
                    oGeneralData1.SetProperty("U_Z_Dob", oGrid.DataTable.GetValue("U_Z_Dob", intRow))
                    oGeneralData1.SetProperty("U_Z_Email", oGrid.DataTable.GetValue("U_Z_EmailId", intRow))
                    oGeneralData1.SetProperty("U_Z_YrExp", oGrid.DataTable.GetValue("U_Z_YrExp", intRow))
                    oGeneralData1.SetProperty("U_Z_AppDate", oGrid.DataTable.GetValue("U_Z_AppDate", intRow))
                    oGeneralData1.SetProperty("U_Z_Skills", oGrid.DataTable.GetValue("U_Z_Skills", intRow))
                    oGeneralData1.SetProperty("U_Z_Dept", strDept)
                    oGeneralData1.SetProperty("U_Z_DeptName", strDeptName)
                    oGeneralData1.SetProperty("U_Z_ReqNo", strRequestno)
                    oGeneralData1.SetProperty("U_Z_JobPosi", strPosition)
                    oGeneralData1.SetProperty("U_Z_Mobile", oGrid.DataTable.GetValue("U_Z_Mobile", intRow))
                    oGeneralData1.SetProperty("U_Z_ApplyDate", Now.Date)
                    oGeneralData1.SetProperty("U_Z_JobPosiCode", strposCode)
                    oGeneralData1.SetProperty("U_Z_AppStatus", status)

                    oChildren1 = oGeneralData1.Child("Z_HR_OHEM2")
                    'otestRs.DoQuery("SELECT T1.[U_Z_HRAppID], T1.[U_Z_ScheduleDate], T1.[U_Z_InterviwerName],T1.[U_Z_InterviewDate],T1.[U_Z_Comments],T1.[U_Z_Rating],T1.[U_Z_InterviewStatus],T1.[U_Z_FileName] FROM [dbo].[@Z_HR_OHEM1]  T0  inner Join  [dbo].[@Z_HR_OHEM2]  T1 on T1.DocEntry=T0.DocEntry and T0.U_Z_HRAppID=" & strempid & "")
                    'If otestRs.RecordCount > 0 Then
                    '    For inlloop As Integer = 0 To otestRs.RecordCount - 1
                    '        oChild = oChildren1.Add()
                    '        oChild.SetProperty("U_Z_HRAppID", otestRs.Fields.Item("U_Z_HRAppID").Value)
                    '        oChild.SetProperty("U_Z_ScheduleDate", otestRs.Fields.Item("U_Z_ScheduleDate").Value)
                    '        oChild.SetProperty("U_Z_InterviwerName", otestRs.Fields.Item("U_Z_InterviwerName").Value)
                    '        oChild.SetProperty("U_Z_InterviewDate", otestRs.Fields.Item("U_Z_InterviewDate").Value)
                    '        oChild.SetProperty("U_Z_Comments", otestRs.Fields.Item("U_Z_Comments").Value)
                    '        oChild.SetProperty("U_Z_Rating", otestRs.Fields.Item("U_Z_Rating").Value)
                    '        oChild.SetProperty("U_Z_InterviewStatus", otestRs.Fields.Item("U_Z_InterviewStatus").Value)
                    '        oChild.SetProperty("U_Z_FileName", otestRs.Fields.Item("U_Z_FileName").Value)
                    '        otestRs.MoveNext()
                    '    Next
                    'Else
                    '     oChild = oChildren1.Add()
                    '    oChild.SetProperty("U_Z_HRAppID", strempid)
                    '   'oChild.SetProperty("U_Z_ScheduleDate", Now.Date)
                    'oChild.SetProperty("U_Z_InterviwerName", "")
                    'oChild.SetProperty("U_Z_InterviewDate", "")
                    'oChild.SetProperty("U_Z_Comments", "")
                    'oChild.SetProperty("U_Z_Rating", "")
                    'oChild.SetProperty("U_Z_InterviewStatus", "P")
                    'oChild.SetProperty("U_Z_FileName", "")
                    'End If

                    oGeneralService.Add(oGeneralData1)
                    strSQL = "Update [@Z_HR_OCRAPP] set U_Z_Status='S',U_Z_RequestCode='" & oGrid.DataTable.GetValue("U_Z_RequestCode", intRow) & "' where DocEntry='" & strempid & "' and (U_Z_Status='R') "
                    otestRs.DoQuery(strSQL)
                End If
                oApplication.Utilities.CandidateUpdation("Search", oGrid.DataTable.GetValue("U_Z_RequestCode", intRow))
            Next

            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
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
#End Region

    Private Sub reDrawForm(ByVal oForm As SAPbouiCOM.Form)
        Try
            oForm.Freeze(True)
            oForm.Items.Item("24").Width = oForm.Width - 30
            oForm.Items.Item("24").Height = oForm.Height - 156
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
        End Try
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_HR_Search1 Then
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
                                If pVal.ItemUID = "_2" Then
                                    oForm.Close()
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If (pVal.ItemUID = "10" And pVal.ColUID = "DocEntry") Or (pVal.ItemUID = "25" And pVal.ColUID = "U_Z_HRAppID") Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcode As String = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    Dim ooBj As New clshrCrApplicants
                                    ooBj.ViewCandidate(strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If (pVal.ItemUID = "10" And pVal.ColUID = "U_Z_RequestCode") Or (pVal.ItemUID = "25" And pVal.ColUID = "U_Z_ReqNo") Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcode As String = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    Dim ooBj As New clshrMPRequest
                                    ooBj.LoadForm1(strcode)
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
                                '  ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "22" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 3
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "23" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = 4
                                    oForm.Freeze(False)
                                End If
                                Select Case pVal.ItemUID
                                    Case "22"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            Exit Sub
                                        End If
                                        LoadFiles(oForm)
                                    Case "4"
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = oForm.PaneLevel - 1
                                        Dim osta As SAPbouiCOM.StaticText
                                        osta = oForm.Items.Item("19").Specific
                                        osta.Caption = "Step " & oForm.PaneLevel & " of 3"
                                        If oForm.PaneLevel = 3 Then
                                            Dim strDept, stSQL1, strskilles, Reqno As String
                                            'Dim oTemp1 As SAPbobsCOM.Recordset
                                            'oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                            'oCombobox = oForm.Items.Item("16").Specific
                                            'strDept = oCombobox.Selected.Description
                                            'oApplication.Utilities.getEdittextvalue(oForm, "18")
                                            'stSQL1 = "select t1.U_Z_Skillsets,isnull(T0.U_Z_PosName,'') as Position from [@Z_HR_ORMPREQ] T0 inner join [@Z_HR_RMPREQ4] T1 on t0.DocEntry=t1.DocEntry "
                                            'stSQL1 = stSQL1 & " where t0.U_Z_DeptCode='" & oCombobox.Selected.Value & "' and T0.DocEntry='" & oApplication.Utilities.getEdittextvalue(oForm, "20") & "'"
                                            'oTemp1.DoQuery(stSQL1)
                                            'For intRow As Integer = 0 To oTemp1.RecordCount - 1
                                            '    strskilles = oTemp1.Fields.Item(0).Value
                                            '    oApplication.Utilities.setEdittextvalue(oForm, "1000003", oTemp1.Fields.Item("Position").Value)
                                            '    oTemp1.MoveNext()
                                            'Next
                                            Reqno = oApplication.Utilities.getEdittextvalue(oForm, "18")
                                            Gridbind(oForm)
                                        End If
                                        oForm.Freeze(False)
                                    Case "3"
                                        oForm.Freeze(True)
                                        Dim dept, Reqno As String
                                        Dim strDept, stSQL1, strskilles As String
                                        oForm.PaneLevel = oForm.PaneLevel + 1
                                        Dim osta As SAPbouiCOM.StaticText
                                        osta = oForm.Items.Item("19").Specific
                                        osta.Caption = "Step " & oForm.PaneLevel & " of 3"
                                        If oForm.PaneLevel = 3 Then
                                            Dim oTemp1 As SAPbobsCOM.Recordset
                                            oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                            oCombobox = oForm.Items.Item("7").Specific
                                            strDept = oCombobox.Selected.Value
                                            oCombobox1 = oForm.Items.Item("16").Specific
                                            oCombobox1.Select(strDept, SAPbouiCOM.BoSearchKey.psk_ByDescription)
                                            oApplication.Utilities.setEdittextvalue(oForm, "18", oApplication.Utilities.getEdittextvalue(oForm, "20"))
                                            If oApplication.Utilities.getEdittextvalue(oForm, "20") <> "" Then
                                                stSQL1 = "select t1.U_Z_Skillsets,isnull(T0.U_Z_PosName,'') as Position from [@Z_HR_ORMPREQ] T0 inner join [@Z_HR_RMPREQ4] T1 on t0.DocEntry=t1.DocEntry "
                                                stSQL1 = stSQL1 & " where  T0.DocEntry='" & oApplication.Utilities.getEdittextvalue(oForm, "20") & "'"
                                                oTemp1.DoQuery(stSQL1)
                                                If oTemp1.RecordCount > 0 Then
                                                    oApplication.Utilities.setEdittextvalue(oForm, "1000003", oTemp1.Fields.Item("Position").Value)
                                                End If
                                            End If
                                        Reqno = oApplication.Utilities.getEdittextvalue(oForm, "18")
                                        Gridbind(oForm)
                                        oForm.Items.Item("22").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                        End If
                                        oForm.Freeze(False)
                                        'If oForm.PaneLevel = 4 Then
                                        '    oForm.Freeze(True)
                                        '    If ApplicantsShortlisted(oForm) = False Then
                                        '        BubbleEvent = False
                                        '        Exit Sub
                                        '    Else
                                        '        oCombobox = oForm.Items.Item("16").Specific
                                        '        strDept = oCombobox.Selected.Value
                                        '        Reqno = oApplication.Utilities.getEdittextvalue(oForm, "18")
                                        '        Gridbind1(strDept, Reqno)
                                        '    End If
                                        '    oForm.Freeze(False)
                                        'End If
                                    Case "5"
                                        If oApplication.SBO_Application.MessageBox("Do you want confirm the candidate allocation", , "Yes", "No") = 2 Then
                                            Exit Sub
                                        ElseIf ApplicantsShortlisted(oForm) = True Then
                                            oApplication.Utilities.Message("Applicants Allocated successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                            oForm.Close()
                                        Else
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    Case "26"
                                        Selectall(oForm, True)
                                    Case "27"
                                        Selectall(oForm, False)
                                End Select
                                If pVal.ItemUID = "43" Then
                                    oGrid = oForm.Items.Item("10").Specific
                                    If oGrid.Rows.Count > 0 Then
                                        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                            If oGrid.Rows.IsSelected(intRow) Then
                                                Dim strcode As String = oGrid.DataTable.GetValue("DocEntry", intRow)
                                                If strcode <> "0" Then
                                                    Dim ooBj As New clshrCrApplicants
                                                    ooBj.ViewCandidate(strcode)
                                                Else
                                                    BubbleEvent = False
                                                    Exit Sub
                                                End If
                                                ' Exit Sub
                                            End If
                                        Next
                                    End If
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1 As String
                                Dim sCHFL_ID, val, val2, val3, val4 As String
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
                                            val1 = oDataTable.GetValue("DocEntry", 0)
                                            val = oDataTable.GetValue("U_Z_PosName", 0)
                                            val4 = oDataTable.GetValue("U_Z_EmpPosi", 0)
                                            val2 = oDataTable.GetValue("U_Z_DeptCode", 0)
                                            val3 = oDataTable.GetValue("U_Z_DeptName", 0)
                                            Try
                                                oCombobox1 = oForm.Items.Item("7").Specific
                                                oCombobox1.Select(val2, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                                oApplication.Utilities.setEdittextvalue(oForm, "21", val3)
                                                oApplication.Utilities.setEdittextvalue(oForm, "1000005", val)
                                                oApplication.Utilities.setEdittextvalue(oForm, "30", val4)
                                                oApplication.Utilities.setEdittextvalue(oForm, "20", val1)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "10" And pVal.ColUID = "U_Z_RequestCode" Then
                                            oGrid = oForm.Items.Item("10").Specific
                                            val = oDataTable.GetValue("DocEntry", 0)
                                            oGrid.DataTable.Columns.Item("U_Z_RequestCode").Cells.Item(pVal.Row).Value = val
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
                Case mnu_hr_Search
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
