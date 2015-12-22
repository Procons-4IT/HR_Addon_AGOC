Public Class clshrMPRequest
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox, oCombobox1 As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private oColumn As SAPbouiCOM.Column
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private MatrixId As String
    Private InvForConsumedItems, count As Integer
    Private RowtoDelete As Integer
    Private blnFlag As Boolean = False
    Dim oDataSrc_Line, oDataSrc_Line3, oDataSrc_Line4 As SAPbouiCOM.DBDataSource
    Dim oDataSrc_Line1, oDataSrc_Line2 As SAPbouiCOM.DBDataSource
    Dim dt As Date
    Dim sQuery As String

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Public Sub LoadForm(ByVal eid As String, ByVal ename As String, ByVal actiontype As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_Hr_MPRequest) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_MPRequest, frm_Hr_MPRequest)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        ManagerName = ename
        ManagerId = eid
        FillPosition1(oForm, eid)
        FillRequestReason(oForm)
        'oForm.DataBrowser.BrowseBy = "1000003"
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        AddChooseFromList(oForm)
        databind(oForm)
        FillLevels(oForm)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line1 = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ2")
        For count = 1 To oDataSrc_Line1.Size - 1
            oDataSrc_Line1.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line2 = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ3")
        For count = 1 To oDataSrc_Line2.Size - 1
            oDataSrc_Line2.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line3 = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ4")
        For count = 1 To oDataSrc_Line3.Size - 1
            oDataSrc_Line3.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line4 = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ5")
        For count = 1 To oDataSrc_Line3.Size - 1
            oDataSrc_Line4.SetValue("LineId", count - 1, count)
        Next
        If actiontype = "A" Then
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE
            oApplication.Utilities.setEdittextvalue(oForm, "4", eid)
            oApplication.Utilities.setEdittextvalue(oForm, "6", ename)
            BindExtEmpNo(oForm, eid)
            oForm.Items.Item("21").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            'oApplication.Utilities.PopulateMPRPeopleObjectives(eid, oForm)
            'FillDepartment(oForm, eid)
            AddMode(oForm)

        Else
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        End If
        oForm.PaneLevel = 1
        EnableDisable(oForm, oForm.Title, eid, ename, "Pending")
        oForm.DataSources.UserDataSources.Add("CRTime", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("FLTime", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("SLTime", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("CLTime", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)

        oApplication.Utilities.setUserDatabind(oForm, "101", "CRTime")
        oApplication.Utilities.setUserDatabind(oForm, "106", "FLTime")
        oApplication.Utilities.setUserDatabind(oForm, "111", "SLTime")
        oApplication.Utilities.setUserDatabind(oForm, "116", "CLTime")
        reDrawForm(oForm)
        oForm.Freeze(False)
        oForm.Items.Item("1000003").Enabled = False
        'oForm.Items.Item("29").Enabled = False
        oForm.Items.Item("130").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
    End Sub
    Private Sub BindExtEmpNo(ByVal aForm As SAPbouiCOM.Form, ByVal EmpNo As String)
        Dim oRec As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRec.DoQuery("Select ""U_Z_EmpID"" from OHEM where ""empID""='" & EmpNo & "'")
        If oRec.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(aForm, "141", oRec.Fields.Item(0).Value)
        End If
    End Sub
    Public Sub LoadForm1(ByVal strdoc As String, Optional ByVal strtitle As String = "", Optional ByVal empcode As String = "", Optional ByVal empname As String = "", Optional ByVal strstatus As String = "")
        oForm = oApplication.Utilities.LoadForm(xml_hr_MPRequest, frm_Hr_MPRequest)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        'oForm.Title = strtitle
        If empcode <> "" Then
            FillPosition1(oForm, empcode)

        Else
            FillPosition(oForm)
        End If

        FillRequestReason(oForm)
        'oForm.DataBrowser.BrowseBy = "1000003"
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        AddChooseFromList(oForm)
        databind(oForm)
        FillLevels(oForm)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line1 = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ2")
        For count = 1 To oDataSrc_Line1.Size - 1
            oDataSrc_Line1.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line2 = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ3")
        For count = 1 To oDataSrc_Line2.Size - 1
            oDataSrc_Line2.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line3 = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ4")
        For count = 1 To oDataSrc_Line3.Size - 1
            oDataSrc_Line3.SetValue("LineId", count - 1, count)
        Next
        oDataSrc_Line4 = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ5")
        For count = 1 To oDataSrc_Line3.Size - 1
            oDataSrc_Line4.SetValue("LineId", count - 1, count)
        Next
        oForm.PaneLevel = 5
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oForm.Items.Item("1000003").Enabled = True
        oApplication.Utilities.setEdittextvalue(oForm, "1000003", strdoc)
        oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        EnableDisable(oForm, strtitle, empcode, empname, strstatus)
        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE Then
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        End If
        oForm.Items.Item("29").Enabled = False
        oForm.Items.Item("130").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
        Try
            oCombobox = oForm.Items.Item("14").Specific
            FillSalCode(oForm, oCombobox.Selected.Value)
        Catch ex As Exception
        End Try
       
        oForm.Freeze(False)

    End Sub
    Private Sub FillLevels(ByVal aForm As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        Dim oDBDataSrc As SAPbouiCOM.DBDataSource
        Dim oMatrix As SAPbouiCOM.Matrix
        Dim oColum As SAPbouiCOM.Column
        oMatrix = aForm.Items.Item("37").Specific
        ' oDBDataSrc = objForm.DataSources.DBDataSources.Add("@Z_HR_ORGST")
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oColum = oMatrix.Columns.Item("V_4")
        For intRow As Integer = oColum.ValidValues.Count - 1 To 0 Step -1
            oColum.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oColum.ValidValues.Add("", "")
        oTempRec.DoQuery("select ""U_Z_LvelCode"",""U_Z_LvelName""  from ""@Z_HR_COLVL"" order by ""U_Z_LvelCode""")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oColum.ValidValues.Add(oTempRec.Fields.Item("U_Z_LvelCode").Value, oTempRec.Fields.Item("U_Z_LvelName").Value)
            oTempRec.MoveNext()
        Next
        oColum.DisplayDesc = True
        oMatrix.AutoResizeColumns()

    End Sub
    Private Sub EnableDisable(ByVal aForm As SAPbouiCOM.Form, ByVal strtitle As String, ByVal empcode As String, ByVal empname As String, Optional ByVal strstatus As String = "")
        aForm.Items.Item("1000").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        If strtitle = "Recruitment Requisition First Level Approval" Then
            dt = Now.Date
            aForm.Items.Item("54").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oApplication.Utilities.setEdittextvalue(aForm, "54", "t")
            oApplication.SBO_Application.SendKeys("{TAB}")
            aForm.Items.Item("12").Enabled = False
            aForm.Items.Item("14").Enabled = False
            aForm.Items.Item("16").Enabled = False
            aForm.Items.Item("18").Enabled = False
            aForm.Items.Item("22").Visible = True
            aForm.Items.Item("23").Visible = True
            aForm.Items.Item("49").Visible = True
            aForm.Items.Item("50").Visible = True
            aForm.Items.Item("51").Visible = False
            aForm.Items.Item("52").Visible = False
            aForm.Items.Item("1000003").Enabled = False
            aForm.Items.Item("29").Enabled = False
            aForm.Items.Item("34").Enabled = False
            aForm.Items.Item("36").Enabled = False
            aForm.Items.Item("40").Enabled = False
            aForm.Items.Item("48").Enabled = False
            aForm.Items.Item("46").Enabled = False
            aForm.Items.Item("53").Visible = True
            aForm.Items.Item("54").Visible = True
            aForm.Items.Item("55").Visible = False
            aForm.Items.Item("28").Visible = False
            aForm.Items.Item("29").Visible = False
            aForm.Items.Item("1000005").Enabled = False
            aForm.Items.Item("1000004").Enabled = False
            aForm.Items.Item("64").Enabled = False
            oApplication.Utilities.setEdittextvalue(aForm, "56", empcode)
            oApplication.Utilities.setEdittextvalue(aForm, "58", empname)
            oApplication.Utilities.setEdittextvalue(aForm, "57", "")
            oApplication.Utilities.setEdittextvalue(aForm, "59", "")
            If strstatus <> "C" Or strstatus <> "L" Then
                aForm.Items.Item("22").Visible = True
                aForm.Items.Item("23").Visible = True
                aForm.Items.Item("49").Enabled = False
                aForm.Items.Item("50").Enabled = False
                aForm.Items.Item("22").Enabled = True
                aForm.Items.Item("23").Enabled = False
                aForm.Items.Item("1").Visible = True
            Else
                aForm.Items.Item("22").Visible = True
                aForm.Items.Item("23").Visible = True
                aForm.Items.Item("49").Enabled = True
                aForm.Items.Item("50").Enabled = True
                aForm.Items.Item("1").Visible = False
            End If
            oMatrix = aForm.Items.Item("31").Specific
            oMatrix.Columns.Item("V_0").Editable = False
            oMatrix.Columns.Item("V_1").Editable = False
            oMatrix.Columns.Item("V_2").Editable = False
            oMatrix = aForm.Items.Item("32").Specific
            oMatrix.Columns.Item("V_0").Editable = False
            oMatrix.Columns.Item("V_1").Editable = False
            oMatrix.Columns.Item("V_2").Editable = False
            oMatrix.Columns.Item("V_3").Editable = False
            oMatrix = aForm.Items.Item("38").Specific
            oMatrix.Columns.Item("V_0").Editable = False
            oMatrix = aForm.Items.Item("131").Specific
            oMatrix.Columns.Item("V_0").Editable = False
            aForm.Items.Item("43").Visible = False
            aForm.Items.Item("44").Visible = False

            aForm.Items.Item("134").Enabled = False
            aForm.Items.Item("139").Enabled = False
            aForm.Items.Item("137").Enabled = False
            aForm.Items.Item("117").Enabled = False
            aForm.Items.Item("120").Enabled = False
            aForm.Items.Item("123").Enabled = False
            aForm.Items.Item("125").Enabled = False
            aForm.Items.Item("127").Enabled = False
            aForm.Items.Item("129").Enabled = False
            aForm.Items.Item("145").Enabled = False
            aForm.Items.Item("147").Enabled = False
            aForm.Items.Item("145").Enabled = False
            aForm.Items.Item("147").Enabled = False
        ElseIf strtitle = "Recruitment Requisition HR Approval" Then

            '  dt = Now.Date
            '  oApplication.Utilities.setEdittextvalue(aForm, "55", dt)
            aForm.Items.Item("54").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oApplication.Utilities.setEdittextvalue(aForm, "54", "t")
            oApplication.SBO_Application.SendKeys("{TAB}")
            aForm.Items.Item("12").Enabled = False
            aForm.Items.Item("14").Enabled = False
            aForm.Items.Item("16").Enabled = False
            aForm.Items.Item("18").Enabled = False
            aForm.Items.Item("22").Visible = True
            aForm.Items.Item("23").Visible = False
            aForm.Items.Item("49").Visible = False
            aForm.Items.Item("50").Visible = False
            aForm.Items.Item("51").Visible = True
            aForm.Items.Item("52").Visible = True
            aForm.Items.Item("1000003").Enabled = False
            aForm.Items.Item("29").Enabled = False
            aForm.Items.Item("34").Enabled = False
            aForm.Items.Item("36").Enabled = False
            aForm.Items.Item("40").Enabled = False
            aForm.Items.Item("48").Enabled = False
            aForm.Items.Item("46").Enabled = False
            aForm.Items.Item("53").Visible = True
            'aForm.Items.Item("54").Visible = False
            aForm.Items.Item("55").Visible = True
            aForm.Items.Item("28").Visible = False
            aForm.Items.Item("29").Visible = False
            aForm.Items.Item("1000005").Enabled = False
            aForm.Items.Item("1000004").Enabled = False
            aForm.Items.Item("64").Enabled = False
            If (strstatus <> "C" Or strstatus <> "L") Then
                aForm.Items.Item("22").Visible = True
                aForm.Items.Item("23").Visible = True
                aForm.Items.Item("49").Enabled = False
                aForm.Items.Item("50").Enabled = False
                aForm.Items.Item("22").Enabled = True
                aForm.Items.Item("23").Enabled = False
                aForm.Items.Item("1").Visible = True
            Else
                aForm.Items.Item("22").Visible = True
                aForm.Items.Item("23").Visible = True
                aForm.Items.Item("49").Enabled = True
                aForm.Items.Item("50").Enabled = True
                aForm.Items.Item("1").Visible = False
            End If
            oApplication.Utilities.setEdittextvalue(aForm, "56", "")
            oApplication.Utilities.setEdittextvalue(aForm, "58", "")
            oApplication.Utilities.setEdittextvalue(aForm, "57", empcode)
            oApplication.Utilities.setEdittextvalue(aForm, "59", empname)
            oMatrix = aForm.Items.Item("31").Specific
            oMatrix.Columns.Item("V_0").Editable = False
            oMatrix.Columns.Item("V_1").Editable = False
            oMatrix.Columns.Item("V_2").Editable = False
            oMatrix = aForm.Items.Item("32").Specific
            oMatrix.Columns.Item("V_0").Editable = False
            oMatrix.Columns.Item("V_1").Editable = False
            oMatrix.Columns.Item("V_2").Editable = False
            oMatrix.Columns.Item("V_3").Editable = False
            oMatrix = aForm.Items.Item("38").Specific
            oMatrix.Columns.Item("V_0").Editable = False
            oMatrix = aForm.Items.Item("131").Specific
            oMatrix.Columns.Item("V_0").Editable = False
            'oMatrix.Columns.Item("V_1").Editable = False
            aForm.Items.Item("43").Visible = False
            aForm.Items.Item("44").Visible = False

            aForm.Items.Item("134").Enabled = False
            aForm.Items.Item("139").Enabled = False
            aForm.Items.Item("137").Enabled = False
            aForm.Items.Item("117").Enabled = False
            aForm.Items.Item("120").Enabled = False
            aForm.Items.Item("123").Enabled = False
            aForm.Items.Item("125").Enabled = False
            aForm.Items.Item("127").Enabled = False
            aForm.Items.Item("129").Enabled = False
            aForm.Items.Item("145").Enabled = False
            aForm.Items.Item("147").Enabled = False
        Else
            If strstatus <> "Pending" Then
                aForm.Items.Item("54").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                oApplication.Utilities.setEdittextvalue(aForm, "54", "t")
                oApplication.SBO_Application.SendKeys("{TAB}")
                aForm.Items.Item("12").Enabled = False
                aForm.Items.Item("14").Enabled = False
                aForm.Items.Item("16").Enabled = False
                aForm.Items.Item("18").Enabled = False
                aForm.Items.Item("1000003").Enabled = False
                aForm.Items.Item("29").Visible = True
                aForm.Items.Item("34").Enabled = False
                aForm.Items.Item("36").Enabled = False
                aForm.Items.Item("40").Enabled = False
                aForm.Items.Item("48").Enabled = False
                aForm.Items.Item("46").Enabled = False
                aForm.Items.Item("1000005").Enabled = False
                aForm.Items.Item("1000004").Enabled = False
                aForm.Items.Item("64").Enabled = False
                oMatrix = aForm.Items.Item("31").Specific
                oMatrix.Columns.Item("V_0").Editable = False
                oMatrix.Columns.Item("V_2").Editable = False
                oMatrix = aForm.Items.Item("32").Specific
                oMatrix.Columns.Item("V_0").Editable = False
                oMatrix.Columns.Item("V_1").Editable = False
                oMatrix.Columns.Item("V_2").Editable = False
                oMatrix.Columns.Item("V_3").Editable = False
                oMatrix = aForm.Items.Item("38").Specific
                oMatrix.Columns.Item("V_0").Editable = False
                oMatrix = aForm.Items.Item("131").Specific
                oMatrix.Columns.Item("V_0").Editable = False
                aForm.Items.Item("43").Visible = False
                aForm.Items.Item("44").Visible = False

                aForm.Items.Item("134").Enabled = False
                aForm.Items.Item("139").Enabled = False
                aForm.Items.Item("137").Enabled = False
                aForm.Items.Item("117").Enabled = False
                aForm.Items.Item("120").Enabled = False
                aForm.Items.Item("123").Enabled = False
                aForm.Items.Item("125").Enabled = False
                aForm.Items.Item("127").Enabled = False
                aForm.Items.Item("129").Enabled = False
                aForm.Items.Item("145").Enabled = False
                aForm.Items.Item("147").Enabled = False

                oMatrix = aForm.Items.Item("37").Specific
                oMatrix.Columns.Item("V_0").Editable = False
                oMatrix.Columns.Item("V_2").Editable = False
                oMatrix.Columns.Item("V_4").Editable = False
            Else
                aForm.Items.Item("54").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                oApplication.Utilities.setEdittextvalue(aForm, "54", "t")
                oApplication.SBO_Application.SendKeys("{TAB}")
                aForm.Items.Item("12").Enabled = True
                aForm.Items.Item("14").Enabled = True
                aForm.Items.Item("16").Enabled = True
                aForm.Items.Item("23").Enabled = False
                aForm.Items.Item("18").Enabled = True
                aForm.Items.Item("1000003").Enabled = False
                aForm.Items.Item("29").Enabled = True
                aForm.Items.Item("34").Enabled = True
                aForm.Items.Item("36").Enabled = True
                aForm.Items.Item("40").Enabled = True
                aForm.Items.Item("48").Enabled = False
                aForm.Items.Item("46").Enabled = False
                oMatrix = aForm.Items.Item("31").Specific
                oMatrix.Columns.Item("V_0").Editable = True
                oMatrix.Columns.Item("V_1").Editable = False
                oMatrix.Columns.Item("V_2").Editable = True
                oMatrix = aForm.Items.Item("32").Specific
                oMatrix.Columns.Item("V_0").Editable = True
                oMatrix.Columns.Item("V_1").Editable = False
                oMatrix.Columns.Item("V_2").Editable = False
                oMatrix.Columns.Item("V_3").Editable = True
                oMatrix = aForm.Items.Item("38").Specific
                oMatrix.Columns.Item("V_0").Editable = True
                oMatrix = aForm.Items.Item("131").Specific
                oMatrix.Columns.Item("V_0").Editable = True
                aForm.Items.Item("43").Visible = True
                aForm.Items.Item("44").Visible = True

                aForm.Items.Item("134").Enabled = True
                aForm.Items.Item("139").Enabled = True
                aForm.Items.Item("137").Enabled = True
                aForm.Items.Item("117").Enabled = True
                aForm.Items.Item("120").Enabled = True
                aForm.Items.Item("123").Enabled = True
                aForm.Items.Item("125").Enabled = True
                aForm.Items.Item("127").Enabled = True
                aForm.Items.Item("129").Enabled = True
                aForm.Items.Item("145").Enabled = True
                aForm.Items.Item("147").Enabled = True

                oMatrix = aForm.Items.Item("37").Specific
                oMatrix.Columns.Item("V_0").Editable = True
                oMatrix.Columns.Item("V_2").Editable = True
                oMatrix.Columns.Item("V_4").Editable = True
            End If

            aForm.Items.Item("22").Visible = True
            aForm.Items.Item("23").Visible = True
            aForm.Items.Item("49").Visible = False
            aForm.Items.Item("50").Visible = False
            aForm.Items.Item("51").Visible = False
            aForm.Items.Item("52").Visible = False
            aForm.Items.Item("53").Visible = False
            aForm.Items.Item("55").Visible = False
            aForm.Items.Item("28").Visible = True
            aForm.Items.Item("29").Visible = True
        End If
        aForm.Items.Item("200").Visible = True
        aForm.Items.Item("201").Visible = True
        aForm.Items.Item("22").Visible = False
        aForm.Items.Item("23").Visible = False
    End Sub
  
    Private Sub AddMode(ByVal aform As SAPbouiCOM.Form)
        Try
            aform.Freeze(True)
            Dim strCode As String
            strCode = oApplication.Utilities.getMaxCode("@Z_HR_ORMPREQ", "DocEntry")
            aform.Items.Item("1000003").Enabled = True
            aform.Items.Item("29").Enabled = True
            oApplication.Utilities.setEdittextvalue(aform, "1000003", CInt(strCode))
            aform.Items.Item("29").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oApplication.Utilities.setEdittextvalue(aform, "29", "t")
            oApplication.SBO_Application.SendKeys("{TAB}")
            aform.Items.Item("16").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            aform.Items.Item("1000001").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            oMatrix = aform.Items.Item("37").Specific
            oColumn = oMatrix.Columns.Item("V_2")
            oColumn.Editable = True
            oColumn = oMatrix.Columns.Item("V_4")
            oColumn.Editable = True
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub
    Private Sub FillDepartment(ByVal sform As SAPbouiCOM.Form, ByVal poscode As String)
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        ' oSlpRS.DoQuery("select T0.""U_Z_DeptCode"",T0.""U_Z_DeptName""  from ""@Z_HR_OPOSCO"" T0 inner join ""@Z_HR_OPOSIN"" T1 on T0.""U_Z_PosCode""=T1.""U_Z_JobCode"" where T1.""U_Z_PosCode"" = '" & poscode & "'")
        oSlpRS.DoQuery("select T0.""U_Z_DeptCode"",T0.""U_Z_DeptName""  from ""@Z_HR_OPOSIN"" T0  where T0.""U_Z_PosCode"" = '" & poscode & "'")
        If oSlpRS.RecordCount > 0 Then
            oApplication.Utilities.setEdittextvalue(oForm, "8", oSlpRS.Fields.Item(0).Value)
            oApplication.Utilities.setEdittextvalue(oForm, "10", oSlpRS.Fields.Item(1).Value)
        End If
    End Sub
    Private Sub FillPosition(ByVal sform As SAPbouiCOM.Form)
        oCombobox = oForm.Items.Item("14").Specific
        Dim oSlpRS, oslpRec As SAPbobsCOM.Recordset
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select ""U_Z_PosCode"",""U_Z_PosName""  from ""@Z_HR_OPOSIN"" order by ""DocEntry""")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            Try
                oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            Catch ex As Exception
            End Try
            oSlpRS.MoveNext()
        Next
        oForm.Items.Item("14").DisplayDesc = True
    End Sub
    Private Sub FillRequestReason(ByVal sform As SAPbouiCOM.Form)
        oCombobox = oForm.Items.Item("139").Specific
        Dim oSlpRS, oslpRec As SAPbobsCOM.Recordset
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select ""U_Z_ReasonCode"",""U_Z_ReasonName""  from ""@Z_HR_ORRRE"" order by ""DocEntry""")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            Try
                oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            Catch ex As Exception
            End Try
            oSlpRS.MoveNext()
        Next
        oForm.Items.Item("139").DisplayDesc = True
    End Sub
    'Private Sub FillPosition1(ByVal sform As SAPbouiCOM.Form, ByVal empid As String)
    '    oCombobox = oForm.Items.Item("14").Specific
    '    Dim oSlpRS, oslpRec As SAPbobsCOM.Recordset
    '    oslpRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
    '    oslpRec.DoQuery("select dept from OHEM where empID=" & empid & "")
    '    If oslpRec.RecordCount > 0 Then
    '        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
    '            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
    '        Next
    '        oCombobox.ValidValues.Add("", "")
    '        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
    '        oSlpRS.DoQuery("Select ""U_Z_PosCode"",""U_Z_PosName""  from ""@Z_HR_OPOSIN"" where ""U_Z_DeptCode""='" & oslpRec.Fields.Item(0).Value & "' order by ""DocEntry""")
    '        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
    '            Try
    '                oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
    '            Catch ex As Exception
    '            End Try
    '            oSlpRS.MoveNext()
    '        Next
    '    End If
    '    oForm.Items.Item("14").DisplayDesc = True
    'End Sub

    Private Sub FillPosition1(ByVal sform As SAPbouiCOM.Form, ByVal empid As String)
        oCombobox = oForm.Items.Item("14").Specific
        Dim oSlpRS, oslpRec As SAPbobsCOM.Recordset
        oslpRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oslpRec.DoQuery("select ""Code"" from OUDP where ""U_Z_HOD""='" & empid & "'")
        If oslpRec.RecordCount > 0 Then
            For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
                oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
            Next
            oCombobox.ValidValues.Add("", "")
            oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            oSlpRS.DoQuery("Select ""U_Z_PosCode"",""U_Z_PosName""  from ""@Z_HR_OPOSIN"" where ""U_Z_DeptCode"" in (Select ""Code"" from OUDP where ""U_Z_HOD""='" & empid & "')  order by ""DocEntry""")

            For intRow As Integer = 0 To oSlpRS.RecordCount - 1
                Try
                    oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
                Catch ex As Exception
                End Try
                oSlpRS.MoveNext()
            Next
        Else
            oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oSlpRS.DoQuery("Select ""U_Z_PosCode"",""U_Z_PosName""  from ""@Z_HR_OPOSIN""   order by ""DocEntry""")
            For intRow As Integer = 0 To oSlpRS.RecordCount - 1
                Try
                    oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
                Catch ex As Exception
                End Try
                oSlpRS.MoveNext()
            Next
        End If
        oForm.Items.Item("14").DisplayDesc = True
    End Sub
#Region "Add Choose From List"
    Private Sub databind(ByVal aForm As SAPbouiCOM.Form)
        oMatrix = aForm.Items.Item("31").Specific
        oColumn = oMatrix.Columns.Item("V_0")
        oColumn.ChooseFromListUID = "CFL1"
        oColumn.ChooseFromListAlias = "U_Z_BussCode"

        oMatrix = aForm.Items.Item("32").Specific
        oColumn = oMatrix.Columns.Item("V_0")
        oColumn.ChooseFromListUID = "CFL2"
        oColumn.ChooseFromListAlias = "U_Z_PeoobjCode"

        oMatrix = aForm.Items.Item("37").Specific
        oColumn = oMatrix.Columns.Item("V_0")
        oColumn.ChooseFromListUID = "CFL3"
        oColumn.ChooseFromListAlias = "U_Z_CompCode"

        oMatrix = aForm.Items.Item("131").Specific
        oColumn = oMatrix.Columns.Item("V_0")
        oColumn.ChooseFromListUID = "CFL5"
        oColumn.ChooseFromListAlias = "U_Z_LanCode"

        oEditText = aForm.Items.Item("117").Specific
        oEditText.ChooseFromListUID = "CFL4"
        oEditText.ChooseFromListAlias = "U_Z_LocCode"



        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oMatrix = aForm.Items.Item("32").Specific
        Dim oColum As SAPbouiCOM.Column
        oColum = oMatrix.Columns.Item("V_2")
        For intRow As Integer = oColum.ValidValues.Count - 1 To 0 Step -1
            oColum.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oColum.ValidValues.Add("", "")
        oTempRec.DoQuery("Select ""U_Z_CatCode"",""U_Z_CatName"" from ""@Z_HR_PECAT"" order by ""Code""")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oColum.ValidValues.Add(oTempRec.Fields.Item("U_Z_CatCode").Value, oTempRec.Fields.Item("U_Z_CatName").Value)
            oTempRec.MoveNext()
        Next
        oColum.DisplayDesc = True
        oMatrix.LoadFromDataSource()

        'oEditText = oForm.Items.Item("117").Specific
        'oEditText.ChooseFromListUID = "CFL4"
        'oEditText.ChooseFromListAlias = "U_Z_LocCode"

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
            oCFLCreationParams.UniqueID = "CFL2"
            oCFL = oCFLs.Add(oCFLCreationParams)


            oCFLCreationParams.ObjectType = "Z_HR_OCOMP"
            oCFLCreationParams.UniqueID = "CFL3"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams.ObjectType = "Z_HR_OLOC"
            oCFLCreationParams.UniqueID = "CFL4"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams.ObjectType = "Z_HR_OLNG"
            oCFLCreationParams.UniqueID = "CFL5"
            oCFL = oCFLs.Add(oCFLCreationParams)


        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

#End Region
#Region "Methods"
    Private Sub AssignLineNo4(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("131").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ5")
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
    Private Sub AssignLineNo(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("31").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ1")
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
            oMatrix = aForm.Items.Item("32").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ3")
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
            oMatrix = aForm.Items.Item("37").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ2")
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
            oMatrix = aForm.Items.Item("38").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ4")
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
                    oMatrix = aForm.Items.Item("31").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ1")
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
                                Case "2"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
                                Case "3"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
                                Case "4"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
                                Case "8"
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
                Case "3"
                    oMatrix = aForm.Items.Item("32").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ2")

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
                                Case "2"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
                                Case "3"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
                                Case "4"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
                                Case "8"
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
                    AssignLineNo1(aForm)
                Case "2"
                    oMatrix = aForm.Items.Item("37").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ3")
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
                                Case "2"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
                                Case "3"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
                                Case "4"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
                                Case "8"
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
                    AssignLineNo2(aForm)
                Case "4"
                    oMatrix = aForm.Items.Item("38").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ4")
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
                                Case "2"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
                                Case "3"
                                   oMatrix.ClearRowData(oMatrix.RowCount)
                                Case "4"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
                                Case "8"
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
                    AssignLineNo3(aForm)

                Case "8"
                    oMatrix = aForm.Items.Item("131").Specific
                    oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ5")
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
                                Case "2"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
                                Case "3"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
                                Case "4"
                                    oMatrix.ClearRowData(oMatrix.RowCount)
                                Case "8"
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
            Case "1"
                oMatrix = aForm.Items.Item("31").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ1")
            Case "2"
                oMatrix = aForm.Items.Item("32").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ2")
            Case "3"
                oMatrix = aForm.Items.Item("37").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ3")
            Case "4"
                oMatrix = aForm.Items.Item("38").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ4")
            Case "8"
                oMatrix = aForm.Items.Item("131").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ5")
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
                        oMatrix = aForm.Items.Item("31").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ1")
                        AssignLineNo(aForm)
                    Case "2"
                        oMatrix = aForm.Items.Item("32").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ2")
                        AssignLineNo1(aForm)
                    Case "3"
                        oMatrix = aForm.Items.Item("37").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ3")
                        AssignLineNo2(aForm)
                    Case "4"
                        oMatrix = aForm.Items.Item("38").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ4")
                        AssignLineNo3(aForm)
                    Case "8"
                        oMatrix = aForm.Items.Item("131").Specific
                        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ5")
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

#Region "Delete Row"
    Private Sub RefereshDeleteRow(ByVal aForm As SAPbouiCOM.Form)
        If Me.MatrixId = "31" Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ1")
        ElseIf Me.MatrixId = "32" Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ2")
        ElseIf Me.MatrixId = "37" Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ3")
        ElseIf Me.MatrixId = "131" Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ5")
        Else
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ4")
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
                oApplication.Utilities.Message("Enter Requester Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "6") = "" Then
                oApplication.Utilities.Message("Enter Requester Name...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "18") < 0 Then
                oApplication.Utilities.Message("Enter Minimum Experience...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "34") < 0 Then
                oApplication.Utilities.Message("Enter Maximum Experience...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "36") <= 0 Then
                oApplication.Utilities.Message("Enter Vacant Positions...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            oCombobox = aForm.Items.Item("14").Specific
            Dim strposcode As String
            strposcode = oCombobox.Selected.Value
            If strposcode = "" Then
                oApplication.Utilities.Message("Enter Employee Position...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            'Dim oTemp1 As SAPbobsCOM.Recordset
            'Dim stSQL1 As Stringer
            'oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            'If aForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
            '    stSQL1 = "Select * from [@Z_HR_OCOUR] where U_Z_CourseCode='" & oApplication.Utilities.getEdittextvalue(aForm, "4") & "'"
            '    oTemp1.DoQuery(stSQL1)
            '    If oTemp1.RecordCount > 0 Then
            '        oApplication.Utilities.Message("Course Code already exists", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '        Return False
            '    End If
            'End If
            oMatrix = aForm.Items.Item("31").Specific
            Dim strcode2, strcode1 As String
            If oMatrix.RowCount > 1 Then
                strcode2 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount)
                strcode1 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount - 1)
                If strcode2.ToUpper = strcode1.ToUpper Then
                    oApplication.Utilities.Message("This Entry Already Exists...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    Return False
                End If
            End If
            oMatrix = oForm.Items.Item("32").Specific
            If oMatrix.RowCount > 0 Then
                'strcode2 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount)
                'strcode1 = oApplication.Utilities.getMatrixValues(oMtrix, "V_0", oMatrix.RowCount - 1)
                'If strcode2.ToUpper = strcode1.ToUpper Then
                '    oApplication.Utilities.Message("This entry already exists", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                '    Return False
                'End If
                Dim dbweight, TotWeight, dbweight1 As Double
                For introw As Integer = 1 To oMatrix.RowCount
                    strcode2 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", introw)
                    dbweight = oApplication.Utilities.getMatrixValues(oMatrix, "V_3", introw)
                    dbweight1 = dbweight1 + dbweight
                    TotWeight = 100
                Next
                If TotWeight <> dbweight1 Then
                    'oApplication.Utilities.Message("Sum of People Objective Weight Should be Equal To 100%...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    ' Return False
                End If
            Else
                ' oApplication.Utilities.Message("Enter People Objective...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                ' Return False
            End If
            oMatrix = aForm.Items.Item("37").Specific
            Dim strcode5, strcode6, strminLevel As String
            If oMatrix.RowCount > 0 Then
                For introw As Integer = 1 To oMatrix.RowCount
                    strcode5 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", introw)
                    strcode6 = oApplication.Utilities.getMatrixValues(oMatrix, "V_2", introw)
                    strminLevel = oApplication.Utilities.getMatrixValues(oMatrix, "V_4", introw)
                    If strcode5 <> "" Then
                        If strcode6 = "0.0" Then
                            oApplication.Utilities.Message("Competencies Weight missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            oMatrix.Columns.Item("V_2").Cells.Item(introw).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                            Return False
                        ElseIf strminLevel = "" Then
                            oApplication.Utilities.Message("Competencies Min Expected Levels missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            oMatrix.Columns.Item("V_4").Cells.Item(introw).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                            Return False
                        End If
                    End If
                Next


                If strcode5.ToUpper = strcode6.ToUpper Then
                    oApplication.Utilities.Message("This Entry Already Exists...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    Return False
                End If
            End If
            If oForm.Title = "Head of Department Recruitment Approval" Then
                oCombobox = aForm.Items.Item("50").Specific
                Dim oTemp1 As SAPbobsCOM.Recordset
                Dim stSQL1, strstatus As String
                oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                'If aForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                strstatus = oCombobox.Selected.Value
                If strstatus <> "O" Then
                    If strstatus = "A" Then
                        stSQL1 = "Update ""@Z_HR_ORMPREQ"" set ""U_Z_MgrStatus""='SA' where ""DocEntry""='" & oApplication.Utilities.getEdittextvalue(aForm, "1000003") & "'"
                    Else
                        stSQL1 = "Update ""@Z_HR_ORMPREQ"" set ""U_Z_MgrStatus""='SR' where ""DocEntry""='" & oApplication.Utilities.getEdittextvalue(aForm, "1000003") & "'"
                    End If
                    oTemp1.DoQuery(stSQL1)
                End If

                'End If
            End If
            If oForm.Title = "HR Recruitment Approval" Then
                oCombobox = aForm.Items.Item("52").Specific
                Dim oTemp1 As SAPbobsCOM.Recordset
                Dim stSQL1, strstatus As String
                oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                'If aForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                strstatus = oCombobox.Selected.Value
                If strstatus <> "O" Then
                    If strstatus = "A" Then
                        stSQL1 = "Update ""@Z_HR_ORMPREQ"" set ""U_Z_MgrStatus""='" & oCombobox.Selected.Value & "' where ""DocEntry""='" & oApplication.Utilities.getEdittextvalue(aForm, "1000003") & "'"
                    ElseIf strstatus = "L" Then
                        stSQL1 = "Update ""@Z_HR_ORMPREQ"" set ""U_Z_MgrStatus""='C' where ""DocEntry""='" & oApplication.Utilities.getEdittextvalue(aForm, "1000003") & "'"
                    End If
                End If
                'End If
            End If
            oCombobox = aForm.Items.Item("201").Specific
            Dim Approval As String = oApplication.Utilities.DocApproval(aForm, HeaderDoctype.Rec, oApplication.Utilities.getEdittextvalue(aForm, "8"))
            oCombobox.Select(Approval, SAPbouiCOM.BoSearchKey.psk_ByValue)

            AssignLineNo(oForm)
            AssignLineNo1(oForm)
            AssignLineNo2(oForm)
            AssignLineNo3(oForm)
            AssignLineNo4(oForm)
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
            oForm.Items.Item("30").Width = oForm.Width - 30
            oForm.Items.Item("30").Height = oForm.Height - 245
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
        End Try
    End Sub
    Private Sub FillSalCode(ByVal aForm As SAPbouiCOM.Form, ByVal PosCode As String)
        Try
            Dim oRec As SAPbobsCOM.Recordset
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim strQuery As String
            strQuery = "SELECT ""U_Z_SalCode"" FROM ""@Z_HR_OPOSIN"" where ""U_Z_PosCode""='" & PosCode.Trim() & "'"
            oRec.DoQuery(strQuery)
            If oRec.RecordCount > 0 Then
                oApplication.Utilities.setEdittextvalue(aForm, "157", oRec.Fields.Item(0).Value)
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_Hr_MPRequest Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1" And (oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Or oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE) Then
                                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
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
                                If pVal.ItemUID = "143" Then
                                    oApplication.Utilities.OpenMasterinLink(oForm, "Location")
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "158" Then
                                    Dim strcode As String = oApplication.Utilities.getEdittextvalue(oForm, "157")
                                    oApplication.Utilities.OpenMasterinLink(oForm, "Salary", strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.GetForm(pVal.FormTypeEx, pVal.FormTypeCount)
                                If pVal.ItemUID = "31" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("31").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "31"
                                    frmSourceMatrix = oMatrix
                                End If
                                If pVal.ItemUID = "32" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("32").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "32"
                                    frmSourceMatrix = oMatrix
                                End If
                                If pVal.ItemUID = "37" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("37").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "37"
                                    frmSourceMatrix = oMatrix
                                End If
                                If pVal.ItemUID = "38" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("38").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "38"
                                    frmSourceMatrix = oMatrix
                                End If
                                If pVal.ItemUID = "131" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("131").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "131"
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
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                '    If pVal.ItemUID = "12" Then
                                '        oCombobox = oForm.Items.Item("12").Specific
                                '        oCombobox1 = oForm.Items.Item("14").Specific
                                '        If oCombobox.Selected.Value = "E" Then
                                '            oForm.Items.Item("14").Enabled = True
                                '            oForm.Items.Item("16").Enabled = False
                                '        Else
                                '            oForm.Items.Item("14").Enabled = False
                                '            oForm.Items.Item("16").Enabled = True
                                '            oCombobox1.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
                                '        End If
                                '    End If
                                If pVal.ItemUID = "14" Then
                                    oCombobox = oForm.Items.Item(pVal.ItemUID).Specific
                                    oApplication.Utilities.setEdittextvalue(oForm, "611", oCombobox.Selected.Description)
                                    Dim strposcode As String
                                    strposcode = oCombobox.Selected.Value
                                    oApplication.Utilities.PopulateMPRBusinessObjectives(strposcode, oForm)
                                    oApplication.Utilities.PopulateMPRCompetenceObjectives(strposcode, oForm)
                                    FillDepartment(oForm, strposcode)
                                    FillSalCode(oForm, strposcode)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                Select Case pVal.ItemUID


                                    Case "162"
                                        Dim objHistory As New clshrAppHisDetails
                                        objHistory.LoadForm(oForm, HistoryDoctype.Rec, oApplication.Utilities.getEdittextvalue(oForm, "1000003"))
                                    Case "1000001"
                                        oForm.PaneLevel = 1
                                    Case "21"
                                        oForm.PaneLevel = 3
                                    Case "24"
                                        oForm.PaneLevel = 2
                                    Case "27"
                                        oForm.PaneLevel = 4
                                    Case "26"
                                        oForm.PaneLevel = 5
                                    Case "67"
                                        oForm.PaneLevel = 6
                                    Case "96"
                                        oForm.PaneLevel = 7
                                        fillWorkFlowTimeStamp(oForm, "", oApplication.Utilities.getEdittextvalue(oForm, "1000003"))
                                    Case "121"
                                        oForm.PaneLevel = 8
                                    Case "43"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_ADD_ROW)
                                    Case "44"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_DELETE_ROW)
                                    Case "132"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_ADD_ROW)
                                    Case "133"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_DELETE_ROW)
                                    Case "1"
                                        If oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Or oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                            'oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                            If oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                                                If pVal.Action_Success Then
                                                    Dim oRec As SAPbobsCOM.Recordset
                                                    oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                                    sQuery = "Select Max(""DocEntry"") From ""@Z_HR_ORMPREQ"" Where ""UserSign"" = '" & oApplication.Company.UserSignature & "'"
                                                    oRec.DoQuery(sQuery)
                                                    If Not oRec.EoF Then
                                                        oApplication.Utilities.UpdateRecruitmentTimeStamp(oRec.Fields.Item(0).Value, "CR")
                                                    End If
                                                End If
                                            End If
                                            oForm.Close()
                                        End If
                                End Select
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1 As String
                                Dim sCHFL_ID, val, val2 As String
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
                                        If pVal.ItemUID = "31" And pVal.ColUID = "V_0" Then
                                            Try
                                                val = oDataTable.GetValue("U_Z_BussCode", 0)
                                                val1 = oDataTable.GetValue("U_Z_BussName", 0)
                                                oMatrix = oForm.Items.Item("31").Specific

                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val1)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "32" And pVal.ColUID = "V_0" Then
                                            Try
                                                val = oDataTable.GetValue("U_Z_PeoobjCode", 0)
                                                val1 = oDataTable.GetValue("U_Z_PeoobjName", 0)
                                                val2 = oDataTable.GetValue("U_Z_PeoCategory", 0)
                                                oMatrix = oForm.Items.Item("32").Specific
                                                oCombobox = oMatrix.Columns.Item("V_2").Cells.Item(pVal.Row).Specific
                                                oCombobox.Select(val2, SAPbouiCOM.BoSearchKey.psk_ByValue)

                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_3", pVal.Row, oDataTable.GetValue("U_Z_Weight", 0))
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val1)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "37" And pVal.ColUID = "V_0" Then
                                            Try
                                                val = oDataTable.GetValue("U_Z_CompCode", 0)
                                                val1 = oDataTable.GetValue("U_Z_CompName", 0)
                                                oMatrix = oForm.Items.Item("37").Specific

                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val1)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val)
                                            Catch ex As Exception
                                            End Try
                                        End If

                                        If pVal.ItemUID = "131" And pVal.ColUID = "V_0" Then
                                            Try
                                                val = oDataTable.GetValue("U_Z_LanCode", 0)
                                                val1 = oDataTable.GetValue("U_Z_LanName", 0)
                                                oMatrix = oForm.Items.Item("131").Specific
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, val1)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_0", pVal.Row, val)
                                            Catch ex As Exception
                                            End Try
                                        End If

                                        If pVal.ItemUID = "117" Then
                                            Try
                                                val1 = oDataTable.GetValue("U_Z_LocCode", 0)
                                                oApplication.Utilities.setEdittextvalue(oForm, "117", val1)

                                            Catch ex As Exception

                                            End Try
                                            If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                                oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                            End If
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

                Case "CanList"
                    Dim oObj As New clshrCandidates
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oObj.LoadForm(oForm, oApplication.Utilities.getEdittextvalue(oForm, "1000003"), "Job", oForm.Title)
                Case mnu_hr_MPRequest
                    ' LoadForm()
                    Dim oTe As New clshrLogin
                    oTe.LoadForm("MPR")
                Case mnu_hr_RecGMApproval
                    Dim oTe As New clshrLogin
                    oTe.LoadForm("RGM")
                Case mnu_hr_RecHRApproval
                    Dim oTe As New clshrLogin
                    oTe.LoadForm("RHR")
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
                Case mnu_ADD
                    AddMode(oForm)
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


    Public Sub RightClickEvent(ByRef eventInfo As SAPbouiCOM.ContextMenuInfo, ByRef BubbleEvent As Boolean)
        oForm = oApplication.SBO_Application.Forms.Item(eventInfo.FormUID)
        'If eventInfo.FormUID = "RightClk" Then
        If oForm.TypeEx = frm_Hr_MPRequest Then
            If (eventInfo.BeforeAction = True) Then
                Dim oMenuItem As SAPbouiCOM.MenuItem
                Dim oMenus As SAPbouiCOM.Menus
                Try
                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                        Dim oCreationPackage As SAPbouiCOM.MenuCreationParams
                        oCreationPackage = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)
                        oCreationPackage = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)
                        oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING
                        oCreationPackage.UniqueID = "CanList"
                        oCreationPackage.String = "Candidate List"
                        oCreationPackage.Enabled = True
                        oMenuItem = oApplication.SBO_Application.Menus.Item("1280") 'Data'
                        oMenus = oMenuItem.SubMenus
                        oMenus.AddEx(oCreationPackage)
                    End If
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try
            Else
                Dim oMenuItem As SAPbouiCOM.MenuItem
                Dim oMenus As SAPbouiCOM.Menus
                Try
                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                        oApplication.SBO_Application.Menus.RemoveEx("CanList")
                    End If
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try
            End If
        End If
    End Sub

    Public Sub FormDataEvent(ByRef BusinessObjectInfo As SAPbouiCOM.BusinessObjectInfo, ByRef BubbleEvent As Boolean)
        Try
            If BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
                ' oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
                '   AddMode(oForm)
                ' oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub fillWorkFlowTimeStamp(ByVal oForm As SAPbouiCOM.Form, ByVal strForm As String, ByVal strDE As String)
        Dim oRec As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

        'sQuery = "Select LTRIM(RIGHT(CONVERT(VARCHAR(20),""U_Z_CRDate"", 100), 7)) As ""U_Z_CRTime"",LTRIM(RIGHT(CONVERT(VARCHAR(20), ""U_Z_FLDate"", 100), 7)) As ""U_Z_FLTime"",LTRIM(RIGHT(CONVERT(VARCHAR(20),""U_Z_FLDate"", 100), 7)) As ""U_Z_SLTime"",LTRIM(RIGHT(CONVERT(VARCHAR(20),""U_Z_CLDate"", 100), 7)) As ""U_Z_CLTime"" From ""@Z_HR_ORMPREQ"" Where ""DocEntry"" = '" & strDE & "'"
        sQuery = "Select LTRIM(RIGHT(CAST(""U_Z_CRDate"" AS varchar(20)), 7)) AS ""U_Z_CRTime"",LTRIM(RIGHT(CAST(""U_Z_FLDate"" AS varchar(20)), 7)) AS ""U_Z_FLTime"",LTRIM(RIGHT(CAST(""U_Z_HRDate"" AS varchar(20)), 7)) AS ""U_Z_SLTime"",LTRIM(RIGHT(CAST(""U_Z_CLDate"" AS varchar(20)), 7)) AS ""U_Z_CLTime"" From ""@Z_HR_ORMPREQ"" Where ""DocEntry"" = '" & strDE & "'"
        oRec.DoQuery(sQuery)
        If Not oRec.EoF Then
            oApplication.Utilities.setEdittextvalue(oForm, "101", oRec.Fields.Item("U_Z_CRTime").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "106", oRec.Fields.Item("U_Z_FLTime").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "111", oRec.Fields.Item("U_Z_SLTime").Value)
            oApplication.Utilities.setEdittextvalue(oForm, "116", oRec.Fields.Item("U_Z_CLTime").Value)
        End If
    End Sub
End Class
