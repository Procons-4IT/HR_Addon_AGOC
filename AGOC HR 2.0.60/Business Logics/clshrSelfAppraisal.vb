Public Class clshrSelfAppraisal
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
    Private RowtoDelete As Integer
    Private InvBaseDocNo As String
    Private MatrixId As String
    Private oColumn As SAPbouiCOM.Column
    Private InvForConsumedItems, count As Integer
    Private blnFlag As Boolean = False
    Dim oDataSrc_Line As SAPbouiCOM.DBDataSource
    Dim oDataSrc_Line1, oDataSrc_Line2 As SAPbouiCOM.DBDataSource
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Public Sub LoadForm1(ByVal empid As String, ByVal empname As String, ByVal period As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_SelfAppraisal) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_SelfAppr, frm_hr_SelfAppraisal)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        EnableDisable("Appraisals")
        'oForm.EnableMenu(mnu_ADD_ROW, True)
        'oForm.EnableMenu(mnu_DELETE_ROW, True)
        AddChooseFromList(oForm)
        databind(oForm)
        ' FillPeriod(oForm)
        Dim oRecs As SAPbobsCOM.Recordset
        oRecs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim strQ As String = "select U_Z_PerFrom,U_Z_PerTo from [@Z_HR_PERAPP] where U_Z_PerCode=" & period & ""
        oRecs.DoQuery(strQ)
        oApplication.Utilities.setEdittextvalue(oForm, "12", oRecs.Fields.Item("U_Z_PerFrom").Value)
        oApplication.Utilities.setEdittextvalue(oForm, "59", oRecs.Fields.Item("U_Z_PerTo").Value)
        FillLevels(oForm)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line1 = oForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP2")
        For count = 1 To oDataSrc_Line1.Size - 1
            oDataSrc_Line1.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line2 = oForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP3")
        For count = 1 To oDataSrc_Line1.Size - 1
            oDataSrc_Line2.SetValue("LineId", count - 1, count)
        Next
        oForm.PaneLevel = 1
        'oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        If PeriodValidation(empid, period) = False Then
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE
            oApplication.Utilities.setEdittextvalue(oForm, "4", empid)
            oApplication.Utilities.setEdittextvalue(oForm, "6", empname)
            'oForm.Items.Item("8").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            'oApplication.Utilities.setEdittextvalue(oForm, "8", "T")
            'oApplication.SBO_Application.SendKeys("{TAB}")

            ' oCombobox = oForm.Items.Item("12").Specific
            'oCombobox.Select(period, SAPbouiCOM.BoSearchKey.psk_ByDescription)
            oApplication.Utilities.PopulateBusinessObjectives(empid, oForm)
            oForm.Items.Item("14").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oApplication.Utilities.PopulatePeopleObjectives(empid, oForm)
            oForm.Items.Item("15").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oApplication.Utilities.PopulateCompetenceObjectives(empid, oForm)
            AddMode(oForm)

        Else
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
            oForm.Items.Item("4").Enabled = True
            ' oForm.Items.Item("12").Enabled = True
            oApplication.Utilities.setEdittextvalue(oForm, "4", empid)
            ' oCombobox = oForm.Items.Item("12").Specific
            ' oCombobox.Select(period, SAPbouiCOM.BoSearchKey.psk_ByDescription)
            oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        End If
        reDrawForm(oForm)
        oForm.Freeze(False)
        oForm.Items.Item("4").Enabled = False
        oForm.Items.Item("12").Enabled = False
    End Sub

    Private Sub AddMode(ByVal aform As SAPbouiCOM.Form)
        Dim strCode As String
        strCode = oApplication.Utilities.getMaxCode("@Z_HR_OSEAPP", "DocEntry")
        aform.Items.Item("40").Enabled = True
        aform.Items.Item("8").Enabled = True
        oApplication.Utilities.setEdittextvalue(aform, "40", strCode)
        aform.Items.Item("8").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oApplication.Utilities.setEdittextvalue(aform, "8", "t")
        oApplication.SBO_Application.SendKeys("{TAB}")
        aform.Items.Item("26").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        aform.Items.Item("40").Enabled = True
        aform.Items.Item("8").Enabled = True
    End Sub
    Public Sub LoadForm(ByVal strdoc As String, ByVal strtitle As String, ByVal strstatus As String)
        oForm = oApplication.Utilities.LoadForm(xml_hr_SelfAppr, frm_hr_SelfAppraisal)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.Title = strtitle
        'oForm.EnableMenu(mnu_ADD_ROW, True)
        'oForm.EnableMenu(mnu_DELETE_ROW, True)
        AddChooseFromList(oForm)
        databind(oForm)
        'FillPeriod(oForm)
        FillLevels(oForm)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line1 = oForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP2")
        For count = 1 To oDataSrc_Line1.Size - 1
            oDataSrc_Line1.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line2 = oForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP3")
        For count = 1 To oDataSrc_Line1.Size - 1
            oDataSrc_Line2.SetValue("LineId", count - 1, count)
        Next
        oForm.PaneLevel = 1
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oForm.Items.Item("40").Enabled = True
        oForm.Items.Item("51").Visible = False
        oApplication.Utilities.setEdittextvalue(oForm, "40", strdoc)
        oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        Dim oRecs As SAPbobsCOM.Recordset
        oRecs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim strQ As String = "select U_Z_LStrt,U_Z_Period from [@Z_HR_OSEAPP] where DocEntry=" & strdoc & ""
        oRecs.DoQuery(strQ)
        Dim strLStart As String = oRecs.Fields.Item("U_Z_LStrt").Value.ToString()
        strQ = "select U_Z_PerFrom,U_Z_PerTo from [@Z_HR_PERAPP] where U_Z_PerCode='" & oRecs.Fields.Item("U_Z_Period").Value & "'"
        oRecs.DoQuery(strQ)
        oApplication.Utilities.setEdittextvalue(oForm, "12", oRecs.Fields.Item("U_Z_PerFrom").Value)
        oApplication.Utilities.setEdittextvalue(oForm, "59", oRecs.Fields.Item("U_Z_PerTo").Value)
        EnableDisable(strtitle, strstatus, strLStart)
        Dim oRec As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim strCStatus As String = ""
        Dim strChkS, strChkL, strChkSr, strChkH As String
        strCStatus = "select U_Z_SCkApp,U_Z_LCkApp,U_Z_SrCkApp,U_Z_HrCkApp from [@Z_HR_OSEAPP] where DocEntry=" & strdoc & ""
        oRec.DoQuery(strCStatus)
        strChkS = oRec.Fields.Item("U_Z_SCkApp").Value.ToString()
        strChkL = oRec.Fields.Item("U_Z_LCkApp").Value.ToString()
        strChkSr = oRec.Fields.Item("U_Z_SrCkApp").Value.ToString()
        strChkH = oRec.Fields.Item("U_Z_HrCkApp").Value.ToString()

        If strtitle = "Appraisals" Then
            If strChkS = "Y" Then
                Dim oChk As SAPbouiCOM.CheckBox
                oChk = oForm.Items.Item("47").Specific
                oChk.Checked = True
                oForm.Items.Item("47").Enabled = False
            ElseIf strChkH = "" Then
                oForm.Items.Item("51").Visible = True
            End If
        ElseIf strtitle = "Line Manager Appraisal Approval" Then
            If strChkL = "Y" Then
                Dim oChk As SAPbouiCOM.CheckBox
                oChk = oForm.Items.Item("48").Specific
                oChk.Checked = True
                oForm.Items.Item("48").Enabled = False
            End If

        ElseIf strtitle = "Sr.Manager Appraisal Approval" Then
            If strChkSr = "Y" Then
                Dim oChk As SAPbouiCOM.CheckBox
                oChk = oForm.Items.Item("49").Specific
                oChk.Checked = True
                oForm.Items.Item("49").Enabled = False
            End If

        ElseIf strtitle = "HR Acceptance" Then
            If strChkH = "Y" Then
                Dim oChk As SAPbouiCOM.CheckBox
                oChk = oForm.Items.Item("50").Specific
                oChk.Checked = True
                oForm.Items.Item("50").Enabled = False
            End If
        End If
        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE Then
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        End If
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_VIEW_MODE
        oForm.Freeze(False)
    End Sub
    Private Sub EnableDisable(ByVal strtitle As String, Optional ByVal strstatus As String = "", Optional ByVal strLStart As String = "")
        Dim isLevelFromLineManager As Boolean = False
        If strLStart = "LM" Then
            isLevelFromLineManager = True
        End If
        If strtitle = "Line Manager Appraisal Approval" Then
            oForm.Items.Item("40").Enabled = False
            oForm.Items.Item("4").Enabled = False
            oForm.Items.Item("19").Enabled = False
            oForm.Items.Item("23").Enabled = False
            oMatrix = oForm.Items.Item("17").Specific
            oMatrix.Columns.Item("V_0").Editable = False
            oMatrix.Columns.Item("V_3").Editable = False
            oMatrix.Columns.Item("V_4").Editable = True
            oMatrix.Columns.Item("V_5").Editable = False
            oForm.Items.Item("26").Enabled = False
            oForm.Items.Item("30").Enabled = False
            oForm.Items.Item("44").Enabled = False
            oForm.Items.Item("45").Enabled = False
            oForm.Items.Item("46").Enabled = False

            oForm.Items.Item("47").Visible = False

            oForm.Items.Item("49").Visible = False
            oForm.Items.Item("50").Visible = False

            oForm.Items.Item("10").Enabled = False
            oForm.Items.Item("42").Enabled = False

            oMatrix = oForm.Items.Item("24").Specific
            oMatrix.Columns.Item("V_0").Editable = False
            oMatrix.Columns.Item("V_4").Editable = False
            oMatrix.Columns.Item("V_5").Editable = True
            oMatrix.Columns.Item("V_6").Editable = False
            oForm.Items.Item("33").Enabled = False
            oForm.Items.Item("37").Enabled = False

            oMatrix = oForm.Items.Item("31").Specific
            oMatrix.Columns.Item("V_0").Editable = False
            oMatrix.Columns.Item("V_3").Editable = False
            oMatrix.Columns.Item("V_4").Editable = True
            oMatrix.Columns.Item("V_5").Editable = False
            If strstatus = "Draft" Then
                oCombobox = oForm.Items.Item("10").Specific
                oCombobox.Select("S", SAPbouiCOM.BoSearchKey.psk_ByDescription)
            End If
            If strstatus = "Approved" Or strstatus = "Closed" Or strstatus = "Canceled" Then
                oForm.Items.Item("8").Enabled = False
                oForm.Items.Item("10").Enabled = False
                oForm.Items.Item("12").Enabled = False
                oMatrix = oForm.Items.Item("17").Specific
                oMatrix.Columns.Item("V_4").Editable = False
                oMatrix = oForm.Items.Item("24").Specific
                oMatrix.Columns.Item("V_5").Editable = False
                oMatrix = oForm.Items.Item("31").Specific
                oMatrix.Columns.Item("V_4").Editable = False
                oForm.Items.Item("21").Enabled = False
                oForm.Items.Item("28").Enabled = False
                oForm.Items.Item("35").Enabled = False
            End If
        ElseIf strtitle = "Sr.Manager Appraisal Approval" Then
            oForm.Items.Item("40").Enabled = False
            oForm.Items.Item("4").Enabled = False
            oForm.Items.Item("19").Enabled = False
            oForm.Items.Item("21").Enabled = False
            oForm.Items.Item("33").Enabled = False
            oMatrix = oForm.Items.Item("17").Specific
            oMatrix.Columns.Item("V_0").Editable = False
            oMatrix.Columns.Item("V_3").Editable = False
            oMatrix.Columns.Item("V_4").Editable = False
            oMatrix.Columns.Item("V_5").Editable = True
            oForm.Items.Item("47").Visible = False
            oForm.Items.Item("48").Visible = False

            oForm.Items.Item("50").Visible = False


            oForm.Items.Item("10").Enabled = False
            oForm.Items.Item("42").Enabled = False

            oForm.Items.Item("23").Enabled = False
            oForm.Items.Item("26").Enabled = False
            oForm.Items.Item("28").Enabled = False
            oForm.Items.Item("30").Enabled = False
            oForm.Items.Item("44").Enabled = True
            oForm.Items.Item("45").Enabled = True
            oForm.Items.Item("46").Enabled = True
            oMatrix = oForm.Items.Item("24").Specific
            oMatrix.Columns.Item("V_0").Editable = False
            oMatrix.Columns.Item("V_4").Editable = False
            oMatrix.Columns.Item("V_5").Editable = False
            oMatrix.Columns.Item("V_6").Editable = True
            oForm.Items.Item("33").Enabled = False
            oForm.Items.Item("35").Enabled = False
            oForm.Items.Item("37").Enabled = False
            oMatrix = oForm.Items.Item("31").Specific
            oMatrix.Columns.Item("V_0").Editable = False
            oMatrix.Columns.Item("V_3").Editable = False
            oMatrix.Columns.Item("V_4").Editable = False
            oMatrix.Columns.Item("V_5").Editable = True
            If strstatus = "2nd Level Approval" Then
                oCombobox = oForm.Items.Item("10").Specific
                oCombobox.Select("F", SAPbouiCOM.BoSearchKey.psk_ByDescription)
            End If
            If strstatus = "Approved" Or strstatus = "Closed" Or strstatus = "Canceled" Then
                oForm.Items.Item("8").Enabled = False
                oForm.Items.Item("10").Enabled = False
                oForm.Items.Item("12").Enabled = False
                oMatrix = oForm.Items.Item("17").Specific
                oMatrix.Columns.Item("V_5").Editable = False
                oMatrix = oForm.Items.Item("24").Specific
                oMatrix.Columns.Item("V_6").Editable = False
                oMatrix = oForm.Items.Item("31").Specific
                oMatrix.Columns.Item("V_5").Editable = False
                oForm.Items.Item("37").Enabled = False
                oForm.Items.Item("30").Enabled = False
                oForm.Items.Item("23").Enabled = False
            End If
        ElseIf strtitle = "Appraisals" Then
            
            oForm.Items.Item("40").Enabled = False
            oForm.Items.Item("23").Enabled = False
            oForm.Items.Item("21").Enabled = False
            oMatrix = oForm.Items.Item("17").Specific
            If isLevelFromLineManager Then
                oMatrix.Columns.Item("V_3").Editable = False
            Else
                oMatrix.Columns.Item("V_3").Editable = True
            End If
            oMatrix.Columns.Item("V_4").Editable = False
            oMatrix.Columns.Item("V_5").Editable = False
            oForm.Items.Item("28").Enabled = False
            oForm.Items.Item("30").Enabled = False
            oMatrix = oForm.Items.Item("24").Specific
            If isLevelFromLineManager Then
                oMatrix.Columns.Item("V_4").Editable = False
            Else
                oMatrix.Columns.Item("V_4").Editable = True
            End If

            oMatrix.Columns.Item("V_5").Editable = False
            oMatrix.Columns.Item("V_6").Editable = False
            oForm.Items.Item("35").Enabled = False
            oForm.Items.Item("37").Enabled = False
            oForm.Items.Item("44").Enabled = False
            oForm.Items.Item("45").Enabled = False
            oForm.Items.Item("46").Enabled = False


            oForm.Items.Item("10").Enabled = False
            oForm.Items.Item("42").Enabled = False

            oForm.Items.Item("48").Visible = False
            oForm.Items.Item("49").Visible = False
            oForm.Items.Item("50").Visible = False


            oMatrix = oForm.Items.Item("31").Specific
            If isLevelFromLineManager Then
                oMatrix.Columns.Item("V_3").Editable = False
            Else
                oMatrix.Columns.Item("V_3").Editable = True
            End If

            oMatrix.Columns.Item("V_4").Editable = False
            oMatrix.Columns.Item("V_5").Editable = False
            If strstatus = "Approved" Or strstatus = "Closed" Or strstatus = "Canceled" Then
                oForm.Items.Item("8").Enabled = False
                oForm.Items.Item("10").Enabled = False
                oForm.Items.Item("12").Enabled = False
                oMatrix = oForm.Items.Item("17").Specific
                oMatrix.Columns.Item("V_0").Editable = False
                oMatrix.Columns.Item("V_3").Editable = False
                oMatrix = oForm.Items.Item("24").Specific
                oMatrix.Columns.Item("V_0").Editable = False
                oMatrix.Columns.Item("V_4").Editable = False
                oMatrix = oForm.Items.Item("31").Specific
                oMatrix.Columns.Item("V_0").Editable = False
                oMatrix.Columns.Item("V_3").Editable = False
                oForm.Items.Item("19").Enabled = False
                oForm.Items.Item("26").Enabled = False
                oForm.Items.Item("33").Enabled = False
            End If
        Else
            oForm.Items.Item("47").Visible = False
            oForm.Items.Item("48").Visible = False
            oForm.Items.Item("49").Visible = False
            oForm.Items.Item("19").Enabled = False
            oForm.Items.Item("26").Enabled = False
            oForm.Items.Item("33").Enabled = False
            oForm.Items.Item("21").Enabled = False
            oForm.Items.Item("28").Enabled = False
            oForm.Items.Item("35").Enabled = False
            oForm.Items.Item("44").Enabled = False
            oForm.Items.Item("45").Enabled = False
            oForm.Items.Item("46").Enabled = False

            oForm.Items.Item("10").Enabled = False
            oForm.Items.Item("42").Enabled = False


        End If
        'oForm.Items.Item("10").Enabled = False
    End Sub
    Private Sub FillLevels(ByVal aForm As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        Dim oDBDataSrc As SAPbouiCOM.DBDataSource
        Dim oMatrix As SAPbouiCOM.Matrix
        Dim oColum As SAPbouiCOM.Column
        oMatrix = aForm.Items.Item("31").Specific
        ' oDBDataSrc = objForm.DataSources.DBDataSources.Add("@Z_HR_ORGST")
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oColum = oMatrix.Columns.Item("V_6")
        For intRow As Integer = oColum.ValidValues.Count - 1 To 0 Step -1
            oColum.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oColum.ValidValues.Add("", "")
        oTempRec.DoQuery("select U_Z_LvelCode,U_Z_LvelName  from [@Z_HR_COLVL] order by U_Z_LvelCode")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oColum.ValidValues.Add(oTempRec.Fields.Item("U_Z_LvelCode").Value, oTempRec.Fields.Item("U_Z_LvelName").Value)
            oTempRec.MoveNext()
        Next
        oColum.DisplayDesc = True
        oMatrix.AutoResizeColumns()

    End Sub
    Private Function PeriodValidation(ByVal empid As String, ByVal period As String) As Boolean
        Dim otemp As SAPbobsCOM.Recordset
        otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp.DoQuery("Select * from [@Z_HR_OSEAPP] where U_Z_EmpId='" & empid & "' and U_Z_Period='" & period & "'")
        If otemp.RecordCount > 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub FillPeriod(ByVal aForm As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = aform.Items.Item("12").Specific
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oTempRec.DoQuery("Select Code,Name from OFPR order by Code desc")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item("Code").Value, oTempRec.Fields.Item("Name").Value)
            oTempRec.MoveNext()
        Next
    End Sub

#Region "Add Choose From List"
    Private Sub databind(ByVal aForm As SAPbouiCOM.Form)
        oMatrix = aForm.Items.Item("17").Specific
        oColumn = oMatrix.Columns.Item("V_0")
        oColumn.ChooseFromListUID = "CFL1"
        oColumn.ChooseFromListAlias = "U_Z_BussCode"

        oMatrix = aForm.Items.Item("24").Specific
        oColumn = oMatrix.Columns.Item("V_0")
        oColumn.ChooseFromListUID = "CFL5"
        oColumn.ChooseFromListAlias = "U_Z_PeoobjCode"

        oMatrix = aForm.Items.Item("31").Specific
        oColumn = oMatrix.Columns.Item("V_0")
        oColumn.ChooseFromListUID = "CFL3"
        oColumn.ChooseFromListAlias = "U_Z_CompCode"

        oEditText = aForm.Items.Item("4").Specific
        oEditText.ChooseFromListUID = "CFL4"
        oEditText.ChooseFromListAlias = "empID"


        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oMatrix = aForm.Items.Item("24").Specific
        Dim oColum As SAPbouiCOM.Column
        oColum = oMatrix.Columns.Item("V_2")
        For intRow As Integer = oColum.ValidValues.Count - 1 To 0 Step -1
            oColum.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oColum.ValidValues.Add("", "")
        oTempRec.DoQuery("Select U_Z_CatCode,U_Z_CatName from [@Z_HR_PECAT] order by Code")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oColum.ValidValues.Add(oTempRec.Fields.Item("U_Z_CatCode").Value, oTempRec.Fields.Item("U_Z_CatName").Value)
            oTempRec.MoveNext()
        Next
        oColum.DisplayDesc = True
        oMatrix.LoadFromDataSource()

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
            oCFLCreationParams.ObjectType = "Z_HR_OBUOB"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)


            oCFLCreationParams.ObjectType = "Z_HR_OPEOB"
            oCFLCreationParams.UniqueID = "CFL5"
            oCFL = oCFLs.Add(oCFLCreationParams)


            oCFLCreationParams.ObjectType = "Z_HR_OCOBJ"
            oCFLCreationParams.UniqueID = "CFL3"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams.ObjectType = "171"
            oCFLCreationParams.UniqueID = "CFL4"
            oCFL = oCFLs.Add(oCFLCreationParams)


        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

#End Region
#Region "Methods"
    Private Sub AssignLineNo(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("17").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP1")
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
    Private Sub AssignLineNo1(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("24").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP2")
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
            oMatrix = aForm.Items.Item("31").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP3")
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

#End Region

#Region "Add Row/ Delete Row"
    Private Sub AddRow(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)

            Select Case aForm.PaneLevel
                Case "1"
                    oMatrix = aForm.Items.Item("17").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP1")
                    If oMatrix.RowCount <= 0 Then
                        oMatrix.AddRow()
                    End If
                    oEditText = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                    Try
                        If oEditText.Value <> "" Then
                            oMatrix.AddRow()
                            Select Case aForm.PaneLevel
                                Case "1"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
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
                    AssignLineNo(aForm)
                Case "2"
                    oMatrix = aForm.Items.Item("24").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP2")

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
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_3", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_4", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_5", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_6", oMatrix.RowCount, "")
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
                    oMatrix = aForm.Items.Item("31").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP3")
                    If oMatrix.RowCount <= 0 Then
                        oMatrix.AddRow()
                    End If
                    oEditText = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
                    Try
                        If oEditText.Value <> "" Then
                            oMatrix.AddRow()
                            Select Case aForm.PaneLevel
                                Case "3"
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_3", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_4", oMatrix.RowCount, "")
                                    oApplication.Utilities.SetMatrixValues(oMatrix, "V_5", oMatrix.RowCount, "")
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
            End Select


            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)

        End Try
    End Sub
    Private Sub deleterow(ByVal aForm As SAPbouiCOM.Form)
        Select Case aForm.PaneLevel
            Case "1"
                oMatrix = aForm.Items.Item("17").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP1")
            Case "2"
                oMatrix = aForm.Items.Item("24").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP2")
            Case "3"
                oMatrix = aForm.Items.Item("31").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP3")
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
                    Case "1"
                        oMatrix = aForm.Items.Item("17").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP1")
                        AssignLineNo(aForm)
                    Case "2"
                        oMatrix = aForm.Items.Item("24").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP2")
                        AssignLineNo1(aForm)
                    Case "3"
                        oMatrix = aForm.Items.Item("31").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP3")
                        AssignLineNo2(aForm)
                End Select
                oMatrix.LoadFromDataSource()
                If aForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                    aForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                End If
                Exit Sub
            End If
        Next

    End Sub

#Region "Delete Row"
    Private Sub RefereshDeleteRow(ByVal aForm As SAPbouiCOM.Form)
        If Me.MatrixId = "17" Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP1")
        ElseIf Me.MatrixId = "24" Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP2")
        Else
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_SEAPP3")
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
#End Region

#End Region

#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            If oApplication.Utilities.getEdittextvalue(aForm, "4") = "" Then
                oApplication.Utilities.Message("Enter Employee Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "6") = "" Then
                oApplication.Utilities.Message("Enter Employee Name...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "8") = "" Then
                oApplication.Utilities.Message("Enter Date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            'Dim oTemp1 As SAPbobsCOM.Recordset
            'Dim stSQL1 As String
            'oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            'If aForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
            '    stSQL1 = "Select * from [@Z_HR_OCOUR] where U_Z_CourseCode='" & oApplication.Utilities.getEdittextvalue(aForm, "4") & "'"
            '    oTemp1.DoQuery(stSQL1)
            '    If oTemp1.RecordCount > 0 Then
            '        oApplication.Utilities.Message("Course Code already exists", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '        Return False
            '    End If
            'End If
            oMatrix = oForm.Items.Item("17").Specific
            Dim strcode2, strcode1 As String
            If oMatrix.RowCount = 0 Then
                'oApplication.Utilities.Message("Business objectives details missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                'Return False
            Else
                If oMatrix.RowCount > 1 Then
                    For intRow As Integer = 1 To oMatrix.RowCount

                        If oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getMatrixValues(oMatrix, "V_3", intRow)) > 5 Then
                            oApplication.Utilities.Message("Rating will be less than 5", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            oMatrix.Columns.Item("V_3").Cells.Item(intRow).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                            Return False
                        End If

                        If oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getMatrixValues(oMatrix, "V_4", intRow)) > 5 Then
                            oApplication.Utilities.Message("Rating will be less than 5", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            oMatrix.Columns.Item("V_4").Cells.Item(intRow).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                            Return False
                        End If

                        If oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getMatrixValues(oMatrix, "V_5", intRow)) > 5 Then
                            oApplication.Utilities.Message("Rating will be less than 5", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            oMatrix.Columns.Item("V_5").Cells.Item(intRow).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                            Return False
                        End If
                        'strcode1 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount - 1)
                        'If strcode2.ToUpper = strcode1.ToUpper Then
                        '    oApplication.Utilities.Message("This entry already exists", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        '    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                        '    Return False
                        'End If
                    Next
                End If
            End If
            
            oMatrix = oForm.Items.Item("24").Specific
            Dim strcode3, strcode4 As String
            If oMatrix.RowCount = 0 Then
                'oApplication.Utilities.Message("People objectives details missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                'Return False
            Else
                If oMatrix.RowCount > 1 Then
                    For intRow As Integer = 1 To oMatrix.RowCount


                        If oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getMatrixValues(oMatrix, "V_4", intRow)) > 5 Then
                            oApplication.Utilities.Message("Rating will be less than 5", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            oMatrix.Columns.Item("V_4").Cells.Item(intRow).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                            Return False
                        End If

                        If oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getMatrixValues(oMatrix, "V_5", intRow)) > 5 Then
                            oApplication.Utilities.Message("Rating will be less than 5", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            oMatrix.Columns.Item("V_5").Cells.Item(intRow).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                            Return False
                        End If

                        If oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getMatrixValues(oMatrix, "V_6", intRow)) > 5 Then
                            oApplication.Utilities.Message("Rating will be less than 5", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            oMatrix.Columns.Item("V_6").Cells.Item(intRow).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                            Return False
                        End If
                    Next
                    'strcode1 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount - 1)
                    'If strcode2.ToUpper = strcode1.ToUpper Then
                    '    oApplication.Utilities.Message("This entry already exists", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    '    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    '    Return False
                    'End If
                End If
            End If

            oMatrix = oForm.Items.Item("31").Specific
            Dim strcode5, strcode6 As String
            If oMatrix.RowCount = 0 Then
                'oApplication.Utilities.Message("Competence objectives details missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                'Return False
            Else
                If oMatrix.RowCount > 1 Then
                    For introw As Integer = 1 To oMatrix.RowCount
                        If oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getMatrixValues(oMatrix, "V_3", introw)) > 5 Then
                            oApplication.Utilities.Message("Rating will be less than 5", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            oMatrix.Columns.Item("V_3").Cells.Item(introw).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                            Return False
                        End If

                        If oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getMatrixValues(oMatrix, "V_4", introw)) > 5 Then
                            oApplication.Utilities.Message("Rating will be less than 5", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            oMatrix.Columns.Item("V_4").Cells.Item(introw).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                            Return False
                        End If

                        If oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getMatrixValues(oMatrix, "V_5", introw)) > 5 Then
                            oApplication.Utilities.Message("Rating will be less than 5", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            oMatrix.Columns.Item("V_5").Cells.Item(introw).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                            Return False
                        End If
                    Next
                End If
            End If
            AssignLineNo(oForm)
            AssignLineNo1(oForm)
            AssignLineNo2(oForm)

            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
#End Region

    Private Sub reDrawForm(ByVal oForm As SAPbouiCOM.Form)
        Try
            oForm.Freeze(True)
            oForm.Items.Item("16").Width = oForm.Width - 30
            oForm.Items.Item("16").Height = oForm.Height - 300
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
        End Try
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_SelfAppraisal Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1" And (oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Or oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE) Then
                                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                                        EnableDisable("Self Appraisal")
                                        If oApplication.SBO_Application.MessageBox("Do you want to confirm the information?", , "Yes", "No") = 2 Then
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    End If
                                    If Validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.GetForm(pVal.FormTypeEx, pVal.FormTypeCount)
                                If pVal.ItemUID = "17" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("17").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "17"
                                    frmSourceMatrix = oMatrix
                                End If
                                If pVal.ItemUID = "24" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("24").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "24"
                                    frmSourceMatrix = oMatrix
                                End If
                                If pVal.ItemUID = "31" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("31").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "31"
                                    frmSourceMatrix = oMatrix
                                End If

                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                reDrawForm(oForm)
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "13"

                                        oForm.PaneLevel = 1
                                    Case "14"
                                        oForm.PaneLevel = 2
                                    Case "15"
                                        oForm.PaneLevel = 3
                                    Case "51"
                                        oForm.PaneLevel = 4
                                    Case "38"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_ADD_ROW)
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
                                        If pVal.ItemUID = "17" And pVal.ColUID = "V_0" Then
                                            val = oDataTable.GetValue("U_Z_BussCode", 0)
                                            val1 = oDataTable.GetValue("U_Z_BussName", 0)
                                            val2 = oDataTable.GetValue("U_Z_Weight", 0)
                                            oMatrix = oForm.Items.Item("17").Specific
                                            Try
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", pVal.Row, val2)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val1)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "24" And pVal.ColUID = "V_0" Then
                                            val = oDataTable.GetValue("U_Z_PeoobjCode", 0)
                                            val1 = oDataTable.GetValue("U_Z_PeoobjName", 0)
                                            val2 = oDataTable.GetValue("U_Z_PeoCategory", 0)
                                            val3 = oDataTable.GetValue("U_Z_Weight", 0)
                                            oMatrix = oForm.Items.Item("24").Specific
                                            oCombobox = oMatrix.Columns.Item("V_2").Cells.Item(pVal.Row).Specific
                                            oCombobox.Select(val2, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                            Try
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_3", pVal.Row, val3)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val1)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "31" And pVal.ColUID = "V_0" Then
                                            val = oDataTable.GetValue("U_Z_CompCode", 0)
                                            val1 = oDataTable.GetValue("U_Z_CompName", 0)
                                            oMatrix = oForm.Items.Item("31").Specific
                                            Try
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val1)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "4" Then
                                            val1 = oDataTable.GetValue("empID", 0)
                                            val = oDataTable.GetValue("firstName", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "6", val)
                                                oApplication.Utilities.setEdittextvalue(oForm, "4", val1)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        oForm.Freeze(False)
                                    End If
                                Catch ex As Exception
                                    oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
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
                Case mnu_hr_SelfAppr
                    'LoadForm()
                    Dim oTe As New clshrLogin
                    oTe.LoadForm("Self")
                Case mnu_hr_MgrAppr
                    Dim oTe As New clshrLogin
                    oTe.LoadForm("MgrApp")
                Case mnu_hr_SMgrAppr
                    Dim oTe As New clshrLogin
                    oTe.LoadForm("SMgrApp")
                Case mnu_hr_HRAppr
                    Dim oTe As New clshrLogin
                    oTe.LoadForm("HR")
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        oForm.Items.Item("4").Enabled = False
                        oForm.Items.Item("6").Enabled = False
                    End If
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
                        'If ValidateDeletion(oForm) = False Then
                        '    BubbleEvent = False
                        '    Exit Sub
                        'End If
                    End If
                Case mnu_ADD
                    If pVal.BeforeAction = False Then
                        oForm = oApplication.SBO_Application.Forms.ActiveForm()
                        oForm.Items.Item("4").Enabled = True
                        oForm.Items.Item("6").Enabled = True
                    End If
                Case mnu_FIND
                    If pVal.BeforeAction = False Then
                        oForm = oApplication.SBO_Application.Forms.ActiveForm()
                        oForm.Items.Item("4").Enabled = True
                        oForm.Items.Item("6").Enabled = True
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
