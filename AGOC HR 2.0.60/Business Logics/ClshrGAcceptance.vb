Public Class ClshrGAcceptance
    Inherits clsBase
    Private InvForConsumedItems As Integer
    Private oGrid As SAPbouiCOM.Grid
    Private oGrid_P1 As SAPbouiCOM.Grid
    Private oGrid_P2 As SAPbouiCOM.Grid
    Private oGrid_P3 As SAPbouiCOM.Grid
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private strQuery As String

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub

    Public Sub LoadForm(ByVal strFEmp As String, ByVal strTEmp As String, ByVal strDept As String, ByVal strPeriod As String)
        oForm = oApplication.Utilities.LoadForm(xml_hr_GAcceptance, frm_hr_GAcceptance)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.DataSources.DataTables.Add("DT_0")
        oForm.DataSources.DataTables.Add("DT_1_P1")
        oForm.DataSources.DataTables.Add("DT_2_P2")
        oForm.DataSources.DataTables.Add("DT_3_P3")
        oForm.Freeze(True)
        oForm.Title = "HR Grievance Acceptance"
        DataBind(strFEmp, strTEmp, strDept, strPeriod)
        oForm.Freeze(False)
    End Sub

    Private Sub DataBind(ByVal strFEmp As String, ByVal strTEmp As String, ByVal strDept As String, ByVal strPeriod As String)
        Dim strqry As String = ""
        oGrid = oForm.Items.Item("8").Specific
        oGrid_P1 = oForm.Items.Item("12").Specific
        oGrid_P2 = oForm.Items.Item("11").Specific
        oGrid_P3 = oForm.Items.Item("10").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
        oGrid_P1.DataTable = oForm.DataSources.DataTables.Item("DT_1_P1")
        oGrid_P2.DataTable = oForm.DataSources.DataTables.Item("DT_2_P2")
        oGrid_P3.DataTable = oForm.DataSources.DataTables.Item("DT_3_P3")

        strqry = "select DocEntry,U_Z_EmpId as 'Employee ID',(select OUDP.Remarks as 'Department' from OHEM JOIN OUDP on OHEM.dept=OUDP.Code where OHEM.empID=T0.U_Z_EmpId) as 'Department',U_Z_EmpName as 'Employee Name',U_Z_Date as 'Document Date',U_Z_Period as 'Period',T1.""U_Z_PerFrom"" as 'Period From',T1.""U_Z_PerTo"" as 'Period To',U_Z_FDate as 'FromDate',U_Z_TDate as 'ToDate',case U_Z_WStatus when 'DR' then 'Draft' when 'HR' then 'HR Approved' when 'SM'then 'Sr.Manager Approved' when 'LM' then 'LineManager Approved'when 'SE' then 'SelfApproved'  end as 'Status' ,U_Z_GHRSts as 'Grevence Acceptance','' as 'Initailize Aproval'  from [@Z_HR_OSEAPP] T0 Left Outer Join ""@Z_HR_PERAPP"" T1 on T0.U_Z_Period=T1.U_Z_PerCode Where U_Z_GStatus='G' and U_Z_Period='" & strPeriod & "' And ISNULL(U_Z_GRef,0) = 0"
        If strDept.Length > 0 Then
            strqry = strqry & "and U_Z_EmpID in (Select empId from OHEM where Dept='" & strDept & "')"
        End If
        If strFEmp.Length > 0 And strTEmp.Length > 0 Then
            strqry = strqry & "  and ( U_Z_EmpId Between " & strFEmp & " and " & strTEmp & ")"
        End If
        strQuery = strqry
        oGrid.DataTable.ExecuteQuery(strqry)
        oEditTextColumn = oGrid.Columns.Item("Employee ID")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
        Dim oGCol1, oGCol2, oGCol3, oGCol4, oGCol5, oGCol6, oGCol7, oGCol8, oGCol9, oGCol10, oGCol11 As SAPbouiCOM.GridColumn
        oGCol1 = oGrid.Columns.Item("DocEntry")
        oGCol2 = oGrid.Columns.Item("Employee ID")
        oGCol3 = oGrid.Columns.Item("Employee Name")
        oGCol11 = oGrid.Columns.Item("Department")
        oGCol4 = oGrid.Columns.Item("Document Date")
        oGCol5 = oGrid.Columns.Item("Period")
        oGCol5.Visible = False
        oGCol6 = oGrid.Columns.Item("FromDate")
        oGCol7 = oGrid.Columns.Item("ToDate")
        oGCol8 = oGrid.Columns.Item("Status")
        oGCol9 = oGrid.Columns.Item("Grevence Acceptance")
        oGCol10 = oGrid.Columns.Item("Initailize Aproval")
        oGCol9.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGCol10.Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        Dim oCombo As SAPbouiCOM.ComboBoxColumn
        oCombo = oGrid.Columns.Item("Grevence Acceptance")
        oCombo.ValidValues.Add("-", "-")
        oCombo.ValidValues.Add("A", "Accepted")
        oCombo.ValidValues.Add("R", "Rejected")
        oCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oCombo.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        oGCol1.Editable = False
        oGCol2.Editable = False
        oGCol3.Editable = False
        oGCol4.Editable = False
        oGCol5.Editable = False
        oGCol6.Editable = False
        oGCol7.Editable = False
        oGCol8.Editable = False
        oGCol11.Editable = False
        oGrid.Columns.Item("Period From").Editable = False
        oGrid.Columns.Item("Period To").Editable = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        If oGrid.Rows.Count > 0 Then
            oGrid.Rows.SelectedRows.Add(0)
            Dim DocNo As Integer = Convert.ToInt32(oGrid.DataTable.GetValue("DocEntry", 0))
            If DocNo = 0 Then
                oApplication.Utilities.Message("No Records Found...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return
            End If
            Dim StrQP0, StrQP1, StrQP2, StrQP3, StrGre As String
            StrQP0 = ""
            StrQP1 = ""
            StrQP2 = ""
            StrQP3 = ""
            StrGre = ""
            StrQP0 = "Select U_Z_WStatus,U_Z_BSelfRemark,U_Z_BMgrRemark,U_Z_BSMrRemark,U_Z_BHrRemark, U_Z_PSelfRemark,U_Z_PMgrRemark,U_Z_PSMrRemark,U_Z_PHrRemark, U_Z_CSelfRemark,U_Z_CMgrRemark,U_Z_CSMrRemark,U_Z_CHrRemark from [@Z_HR_OSEAPP] Where DocEntry=" & DocNo & ""
            StrQP1 = "Select U_Z_BussCode as 'Code',U_Z_BussDesc as 'Business Objectives',U_Z_BussWeight as 'Weight (%)' ,U_Z_BussSelfRate as 'Self Rating (1-5)',U_Z_BussMgrRate as 'Line Manager Rating (1-5)',U_Z_BussSMRate as 'Sr.Manager Rating (1-5)' from [@Z_HR_SEAPP1] Where DocEntry=" & DocNo & ""
            StrQP2 = "Select U_Z_PeopleCode as 'Code',U_Z_PeopleDesc as 'People Objectives',U_Z_PeopleCat as 'Category',U_Z_PeoWeight as 'Weight (%)',U_Z_PeoSelfRate as 'Self Rating (1-5)',U_Z_PeoMgrRate as 'Line Manager Rating (1-5)',U_Z_PeoSMRate as 'Sr.Manager Rating (1-5)' from [@Z_HR_SEAPP2] Where DocEntry=" & DocNo & ""
            StrQP3 = "Select U_Z_CompCode as 'Code',U_Z_CompDesc as 'Competence Objectives',U_Z_CompWeight as 'Weight (%)',U_Z_CompLevel as 'Levels',U_Z_CompSelfRate as 'Self Rating (1-5)',U_Z_CompMgrRate as 'Line Manager Rating (1-5)',U_Z_CompSMRate as 'Sr.Manager Rating (1-5)' from [@Z_HR_SEAPP3] Where DocEntry=" & DocNo & ""
            StrGre = "Select U_Z_GStatus,U_Z_GDate,U_Z_GNo,U_Z_GRemarks from [@Z_HR_OSEAPP] Where DocEntry=" & DocNo & ""
            oGrid_P1.DataTable.ExecuteQuery(StrQP1)
            oGrid_P2.DataTable.ExecuteQuery(StrQP2)
            oGrid_P3.DataTable.ExecuteQuery(StrQP3)
            Dim oRS As SAPbobsCOM.Recordset
            oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRS.DoQuery(StrGre)
            'oForm.Items.Item("8").Enabled = False
            oForm.Items.Item("12").Enabled = False
            oForm.Items.Item("11").Enabled = False
            oForm.Items.Item("10").Enabled = False
            oApplication.Utilities.setEdittextvalue(oForm, "16", oRS.Fields.Item("U_Z_GDate").Value.ToString())
            oApplication.Utilities.setEdittextvalue(oForm, "17", oRS.Fields.Item("U_Z_GNo").Value.ToString())
            oApplication.Utilities.setEdittextvalue(oForm, "18", oRS.Fields.Item("U_Z_GRemarks").Value.ToString())
            oForm.Items.Item("16").Enabled = False
            oForm.Items.Item("17").Enabled = False
            oForm.Items.Item("18").Enabled = False
        End If
    End Sub

    Private Sub ReDataBind()
        Dim strqry As String = ""
        oGrid = oForm.Items.Item("8").Specific
        oGrid_P1 = oForm.Items.Item("12").Specific
        oGrid_P2 = oForm.Items.Item("11").Specific
        oGrid_P3 = oForm.Items.Item("10").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
        oGrid_P1.DataTable = oForm.DataSources.DataTables.Item("DT_1_P1")
        oGrid_P2.DataTable = oForm.DataSources.DataTables.Item("DT_2_P2")
        oGrid_P3.DataTable = oForm.DataSources.DataTables.Item("DT_3_P3")
        oGrid.DataTable.ExecuteQuery(strQuery)
        oEditTextColumn = oGrid.Columns.Item("Employee ID")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
        Dim oGCol1, oGCol2, oGCol3, oGCol4, oGCol5, oGCol6, oGCol7, oGCol8, oGCol9, oGCol10, oGCol11 As SAPbouiCOM.GridColumn
        oGCol1 = oGrid.Columns.Item("DocEntry")
        oGCol2 = oGrid.Columns.Item("Employee ID")
        oGCol3 = oGrid.Columns.Item("Employee Name")
        oGCol11 = oGrid.Columns.Item("Department")
        oGCol4 = oGrid.Columns.Item("Document Date")
        oGCol5 = oGrid.Columns.Item("Period")
        oGCol5.Visible = False
        oGCol6 = oGrid.Columns.Item("FromDate")
        oGCol7 = oGrid.Columns.Item("ToDate")
        oGCol8 = oGrid.Columns.Item("Status")
        oGCol9 = oGrid.Columns.Item("Grevence Acceptance")
        oGCol10 = oGrid.Columns.Item("Initailize Aproval")
        oGCol9.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGCol10.Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox
        Dim oCombo As SAPbouiCOM.ComboBoxColumn
        oCombo = oGrid.Columns.Item("Grevence Acceptance")
        oCombo.ValidValues.Add("-", "-")
        oCombo.ValidValues.Add("A", "Accepted")
        oCombo.ValidValues.Add("R", "Rejected")
        oCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oCombo.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        oGCol1.Editable = False
        oGCol2.Editable = False
        oGCol3.Editable = False
        oGCol4.Editable = False
        oGCol5.Editable = False
        oGCol6.Editable = False
        oGCol7.Editable = False
        oGCol8.Editable = False
        oGCol11.Editable = False
        oGrid.Columns.Item("Period From").Editable = False
        oGrid.Columns.Item("Period To").Editable = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        If oGrid.Rows.Count > 0 Then
            oGrid.Rows.SelectedRows.Add(0)
            Dim DocNo As Integer = Convert.ToInt32(oGrid.DataTable.GetValue("DocEntry", 0))
            If DocNo = 0 Then
                oApplication.Utilities.Message("No Records Found...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return
            End If
            Dim StrQP0, StrQP1, StrQP2, StrQP3, StrGre As String
            StrQP0 = ""
            StrQP1 = ""
            StrQP2 = ""
            StrQP3 = ""
            StrGre = ""
            StrQP0 = "Select U_Z_WStatus,U_Z_BSelfRemark,U_Z_BMgrRemark,U_Z_BSMrRemark,U_Z_BHrRemark, U_Z_PSelfRemark,U_Z_PMgrRemark,U_Z_PSMrRemark,U_Z_PHrRemark, U_Z_CSelfRemark,U_Z_CMgrRemark,U_Z_CSMrRemark,U_Z_CHrRemark from [@Z_HR_OSEAPP] Where DocEntry=" & DocNo & ""
            StrQP1 = "Select U_Z_BussCode as 'Code',U_Z_BussDesc as 'Business Objectives',U_Z_BussWeight as 'Weight (%)' ,U_Z_BussSelfRate as 'Self Rating (1-5)',U_Z_BussMgrRate as 'Line Manager Rating (1-5)',U_Z_BussSMRate as 'Sr.Manager Rating (1-5)' from [@Z_HR_SEAPP1] Where DocEntry=" & DocNo & ""
            StrQP2 = "Select U_Z_PeopleCode as 'Code',U_Z_PeopleDesc as 'People Objectives',U_Z_PeopleCat as 'Category',U_Z_PeoWeight as 'Weight (%)',U_Z_PeoSelfRate as 'Self Rating (1-5)',U_Z_PeoMgrRate as 'Line Manager Rating (1-5)',U_Z_PeoSMRate as 'Sr.Manager Rating (1-5)' from [@Z_HR_SEAPP2] Where DocEntry=" & DocNo & ""
            StrQP3 = "Select U_Z_CompCode as 'Code',U_Z_CompDesc as 'Competence Objectives',U_Z_CompWeight as 'Weight (%)',U_Z_CompLevel as 'Levels',U_Z_CompSelfRate as 'Self Rating (1-5)',U_Z_CompMgrRate as 'Line Manager Rating (1-5)',U_Z_CompSMRate as 'Sr.Manager Rating (1-5)' from [@Z_HR_SEAPP3] Where DocEntry=" & DocNo & ""
            StrGre = "Select U_Z_GStatus,U_Z_GDate,U_Z_GNo,U_Z_GRemarks from [@Z_HR_OSEAPP] Where DocEntry=" & DocNo & ""
            oGrid_P1.DataTable.ExecuteQuery(StrQP1)
            oGrid_P2.DataTable.ExecuteQuery(StrQP2)
            oGrid_P3.DataTable.ExecuteQuery(StrQP3)
            Dim oRS As SAPbobsCOM.Recordset
            oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRS.DoQuery(StrGre)
            'oForm.Items.Item("8").Enabled = False
            oForm.Items.Item("12").Enabled = False
            oForm.Items.Item("11").Enabled = False
            oForm.Items.Item("10").Enabled = False
            oApplication.Utilities.setEdittextvalue(oForm, "16", oRS.Fields.Item("U_Z_GDate").Value.ToString())
            oApplication.Utilities.setEdittextvalue(oForm, "17", oRS.Fields.Item("U_Z_GNo").Value.ToString())
            oApplication.Utilities.setEdittextvalue(oForm, "18", oRS.Fields.Item("U_Z_GRemarks").Value.ToString())
            oForm.Items.Item("16").Enabled = False
            oForm.Items.Item("17").Enabled = False
            oForm.Items.Item("18").Enabled = False
        End If
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_GAcceptance Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "8" And pVal.ColUID = "Initailize Aproval" Then
                                    Dim oComboColumn As SAPbouiCOM.ComboBoxColumn
                                    oGrid = oForm.Items.Item("8").Specific
                                    oComboColumn = oGrid.Columns.Item("Grevence Acceptance")
                                    Dim stvalue As String
                                    Try
                                        stvalue = oComboColumn.GetSelectedValue(pVal.Row).Value

                                    Catch ex As Exception
                                        stvalue = "-"
                                    End Try
                                 
                                    If stvalue <> "A" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If

                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "8" And pVal.ColUID = "Grevence Acceptance" Then
                                    Dim oComboColumn As SAPbouiCOM.ComboBoxColumn
                                    oGrid = oForm.Items.Item("8").Specific
                                    oComboColumn = oGrid.Columns.Item("Grevence Acceptance")
                                    Dim stvalue As String
                                    Try
                                        stvalue = oComboColumn.GetSelectedValue(pVal.Row).Value

                                    Catch ex As Exception
                                        stvalue = "-"
                                    End Try

                                    If stvalue <> "A" Then
                                        Dim ochk As SAPbouiCOM.CheckBoxColumn
                                        ochk = oGrid.Columns.Item("Initailize Aproval")
                                        ochk.Check(pVal.Row, False)
                                        oGrid.DataTable.SetValue("Initailize Aproval", pVal.Row, "N")
                                    End If

                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" Then
                                    oForm.Freeze(True)
                                    oForm.Items.Item("12").Visible = True
                                    oForm.PaneLevel = 0
                                    oForm.Freeze(False)
                                ElseIf pVal.ItemUID = "4" Then
                                    oForm.Freeze(True)
                                    oForm.Items.Item("12").Visible = False
                                    oForm.PaneLevel = 1
                                    oForm.Freeze(False)
                                ElseIf pVal.ItemUID = "5" Then
                                    oForm.Freeze(True)
                                    oForm.Items.Item("12").Visible = False
                                    oForm.PaneLevel = 2
                                    oForm.Freeze(False)
                                ElseIf pVal.ItemUID = "6" Then
                                    oForm.Freeze(True)
                                    oForm.Items.Item("12").Visible = False
                                    oForm.PaneLevel = 4
                                    oForm.Freeze(False)
                                ElseIf pVal.ItemUID = "8" And pVal.ColUID = "RoesHeader" Then
                                    oForm.Freeze(True)
                                    If oGrid.Rows.Count > 0 Then
                                        oGrid = oForm.Items.Item("8").Specific
                                        oGrid_P1 = oForm.Items.Item("12").Specific
                                        oGrid_P2 = oForm.Items.Item("11").Specific
                                        oGrid_P3 = oForm.Items.Item("10").Specific
                                        oGrid_P1.DataTable = oForm.DataSources.DataTables.Item("DT_1_P1")
                                        oGrid_P2.DataTable = oForm.DataSources.DataTables.Item("DT_2_P2")
                                        oGrid_P3.DataTable = oForm.DataSources.DataTables.Item("DT_3_P3")
                                        Dim DocNo As Integer = Convert.ToInt32(oGrid.DataTable.GetValue("DocEntry", pVal.Row))
                                        Dim StrQP0, StrQP1, StrQP2, StrQP3, StrGre As String
                                        StrQP0 = ""
                                        StrQP1 = ""
                                        StrQP2 = ""
                                        StrQP3 = ""
                                        StrGre = ""
                                        StrQP0 = "Select U_Z_WStatus,U_Z_BSelfRemark,U_Z_BMgrRemark,U_Z_BSMrRemark,U_Z_BHrRemark, U_Z_PSelfRemark,U_Z_PMgrRemark,U_Z_PSMrRemark,U_Z_PHrRemark, U_Z_CSelfRemark,U_Z_CMgrRemark,U_Z_CSMrRemark,U_Z_CHrRemark from [@Z_HR_OSEAPP] Where DocEntry=" & DocNo & ""
                                        StrQP1 = "Select U_Z_BussCode as 'Code',U_Z_BussDesc as 'Business Objectives',U_Z_BussWeight as 'Weight (%)' ,U_Z_BussSelfRate as 'Self Rating (1-5)',U_Z_BussMgrRate as 'Line Manager Rating (1-5)',U_Z_BussSMRate as 'Sr.Manager Rating (1-5)' from [@Z_HR_SEAPP1] Where DocEntry=" & DocNo & ""
                                        StrQP2 = "Select U_Z_PeopleCode as 'Code',U_Z_PeopleDesc as 'People Objectives',U_Z_PeopleCat as 'Category',U_Z_PeoWeight as 'Weight (%)',U_Z_PeoSelfRate as 'Self Rating (1-5)',U_Z_PeoMgrRate as 'Line Manager Rating (1-5)',U_Z_PeoSMRate as 'Sr.Manager Rating (1-5)' from [@Z_HR_SEAPP2] Where DocEntry=" & DocNo & ""
                                        StrQP3 = "Select U_Z_CompCode as 'Code',U_Z_CompDesc as 'Competence Objectives',U_Z_CompWeight as 'Weight (%)',U_Z_CompLevel as 'Levels',U_Z_CompSelfRate as 'Self Rating (1-5)',U_Z_CompMgrRate as 'Line Manager Rating (1-5)',U_Z_CompSMRate as 'Sr.Manager Rating (1-5)' from [@Z_HR_SEAPP3] Where DocEntry=" & DocNo & ""
                                        StrGre = "Select U_Z_GStatus,U_Z_GDate,U_Z_GNo,U_Z_GRemarks from [@Z_HR_OSEAPP] Where DocEntry=" & DocNo & ""
                                        oGrid_P1.DataTable.ExecuteQuery(StrQP1)
                                        oGrid_P2.DataTable.ExecuteQuery(StrQP2)
                                        oGrid_P3.DataTable.ExecuteQuery(StrQP3)
                                        Dim oRS As SAPbobsCOM.Recordset
                                        oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                        oRS.DoQuery(StrGre)
                                        'oForm.Items.Item("8").Enabled = False
                                        oForm.Items.Item("12").Enabled = False
                                        oForm.Items.Item("11").Enabled = False
                                        oForm.Items.Item("10").Enabled = False
                                        oApplication.Utilities.setEdittextvalue(oForm, "16", oRS.Fields.Item("U_Z_GDate").Value.ToString())
                                        oApplication.Utilities.setEdittextvalue(oForm, "17", oRS.Fields.Item("U_Z_GNo").Value.ToString())
                                        oApplication.Utilities.setEdittextvalue(oForm, "18", oRS.Fields.Item("U_Z_GRemarks").Value.ToString())
                                        oForm.Items.Item("16").Enabled = False
                                        oForm.Items.Item("17").Enabled = False
                                        oForm.Items.Item("18").Enabled = False
                                    End If
                                    oForm.Freeze(False)
                                ElseIf pVal.ItemUID = "9" Then
                                    oForm.Freeze(True)
                                    oGrid = oForm.Items.Item("8").Specific
                                    Dim oCombo As SAPbouiCOM.ComboBoxColumn
                                    Dim oChk As SAPbouiCOM.CheckBoxColumn
                                    oCombo = oGrid.Columns.Item("Grevence Acceptance")
                                    oChk = oGrid.Columns.Item("Initailize Aproval")
                                    For i As Integer = 0 To oGrid.Rows.Count - 1

                                        If oCombo.GetSelectedValue(i).Value = "A" And oChk.IsChecked(i) = False Then
                                            Dim DocNo As Integer = Convert.ToInt32(oGrid.DataTable.GetValue("DocEntry", i))
                                            Dim oRecSet As SAPbobsCOM.Recordset
                                            Dim strGSUpdate As String = ""
                                            strGSUpdate = "Update [@Z_HR_OSEAPP] set U_Z_GHRSts='A' Where DocEntry=" & DocNo & ""
                                            oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                            oRecSet.DoQuery(strGSUpdate)
                                            oApplication.Utilities.Message("Operation Completed Successfully...", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                        ElseIf oCombo.GetSelectedValue(i).Value = "R" Then
                                            Dim DocNo As Integer = Convert.ToInt32(oGrid.DataTable.GetValue("DocEntry", i))
                                            Dim oRecSet As SAPbobsCOM.Recordset
                                            Dim strGSUpdate As String = ""
                                            strGSUpdate = "Update [@Z_HR_OSEAPP] set U_Z_GHRSts='R' Where DocEntry = " & DocNo & ""
                                            oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                            oRecSet.DoQuery(strGSUpdate)
                                            oApplication.Utilities.Message("Operation Completed Successfully...", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                        ElseIf oCombo.GetSelectedValue(i).Value = "A" And oChk.IsChecked(i) = True Then
                                            Dim DocNo As Integer = Convert.ToInt32(oGrid.DataTable.GetValue("DocEntry", i))
                                            Dim oRecSet As SAPbobsCOM.Recordset
                                            Dim strGSUpdate As String = ""
                                            strGSUpdate = "Update [@Z_HR_OSEAPP] set U_Z_GHRSts = 'A' Where DocEntry=" & DocNo & ""
                                            oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                            oRecSet.DoQuery(strGSUpdate)
                                            Dim strEmpID As String = oGrid.DataTable.GetValue("Employee ID", i).ToString()
                                            Dim strEmpName As String = oGrid.DataTable.GetValue("Employee Name", i).ToString()
                                            Dim strEmpDept As String = oGrid.DataTable.GetValue("Department", i).ToString()
                                            Dim strDocDate As String = oGrid.DataTable.GetValue("Document Date", i).ToString()
                                            Dim strPeriod As String = oGrid.DataTable.GetValue("Period", i).ToString()
                                            Dim strFromDate As String = oGrid.DataTable.GetValue("FromDate", i)
                                            Dim strToDate As String = oGrid.DataTable.GetValue("ToDate", i)
                                            If ReInitializeDocument(DocNo, strEmpID, strEmpName, strEmpDept, strDocDate, strPeriod, strFromDate, strToDate) = True Then
                                                oApplication.Utilities.Message("Operation Completed Successfully...", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                                oForm.Close()
                                            End If
                                        End If
                                    Next
                                    oForm.Freeze(False)
                                End If
                        End Select


                End Select
            End If


        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
            oForm.Freeze(False)
        End Try
    End Sub
#End Region

#Region "Reinitialize Document"
    Private Function ReInitializeDocument(ByVal DocNo As Integer, ByVal EmpID As String, ByVal EmpName As String, ByVal EmpDept As String, ByVal DocDate As String, ByVal Period As String, ByVal FromDate As String, ByVal ToDate As String) As Boolean
        Try

            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If
            oApplication.Company.StartTransaction()
            Dim oUserTable As SAPbobsCOM.UserTable
            Dim oRetVal As Integer

            Dim oGeneralService, oGeneralService1 As SAPbobsCOM.GeneralService
            Dim oGeneralData, oGeneralData1 As SAPbobsCOM.GeneralData
            Dim oGeneralParams, oGeneralParams1 As SAPbobsCOM.GeneralDataParams
            Dim oCompanyService As SAPbobsCOM.CompanyService
            Dim oChildren, oChildren1, oChildren2 As SAPbobsCOM.GeneralDataCollection
            oCompanyService = oApplication.Company.GetCompanyService()
            Dim otestRs, oRec As SAPbobsCOM.Recordset
            Dim oChild, oChild1, oChild2 As SAPbobsCOM.GeneralData
            Dim blnRecordExists As Boolean = False

            oGeneralService = oCompanyService.GetGeneralService("Z_HR_OSELAPP")
            otestRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)

            Dim blnDownpayment As Boolean = False
            Dim blnDocumentItem As Boolean
            blnDocumentItem = False
            oGeneralData1 = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData)
            oGeneralData1.SetProperty("U_Z_Status", "D")
            oGeneralData1.SetProperty("U_Z_EmpId", EmpID)
            oGeneralData1.SetProperty("U_Z_EmpName", EmpName)
            oGeneralData1.SetProperty("U_Z_Period", Period)
            oGeneralData1.SetProperty("U_Z_Date", Now.Date)
            oGeneralData1.SetProperty("U_Z_Initialize", "N")
            Try
                oGeneralData1.SetProperty("U_Z_FDate", FromDate)
                oGeneralData1.SetProperty("U_Z_TDate", ToDate)
            Catch ex As Exception

            End Try
        
            oGeneralData1.SetProperty("U_Z_WStatus", "DR")
            oChildren1 = oGeneralData1.Child("Z_HR_SEAPP1")
            otestRs.DoQuery("SELECT T1.[U_Z_BussCode], T1.[U_Z_BussName], T1.[U_Z_Weight] FROM [dbo].[@Z_HR_ODEMA]  T0  inner Join  [dbo].[@Z_HR_DEMA1]  T1 on T1.DocEntry=T0.DocEntry and T0.U_Z_DeptName='" & EmpDept & "'")
            For inlloop As Integer = 0 To otestRs.RecordCount - 1
                oChild = oChildren1.Add()
                oChild.SetProperty("U_Z_BussCode", otestRs.Fields.Item("U_Z_BussCode").Value)
                oChild.SetProperty("U_Z_BussDesc", otestRs.Fields.Item("U_Z_BussName").Value)
                oChild.SetProperty("U_Z_BussWeight", otestRs.Fields.Item("U_Z_Weight").Value)
                otestRs.MoveNext()
            Next
            oChildren2 = oGeneralData1.Child("Z_HR_SEAPP2")
            otestRs.DoQuery("SELECT T0.[U_Z_HREmpID], T0.[U_Z_HRPeoobjCode], T0.[U_Z_HRPeoobjName], T0.[U_Z_HRPeoCategory], T0.[U_Z_HRWeight] FROM [dbo].[@Z_HR_PEOBJ1]  T0 where T0.U_Z_HREmpID=" & EmpID & "")
            For inlloop As Integer = 0 To otestRs.RecordCount - 1
                oChild1 = oChildren2.Add()
                oChild1.SetProperty("U_Z_PeopleCode", otestRs.Fields.Item("U_Z_HRPeoobjCode").Value)
                oChild1.SetProperty("U_Z_PeopleDesc", otestRs.Fields.Item("U_Z_HRPeoobjName").Value)
                oChild1.SetProperty("U_Z_PeopleCat", otestRs.Fields.Item("U_Z_HRPeoCategory").Value)
                oChild1.SetProperty("U_Z_PeoWeight", otestRs.Fields.Item("U_Z_HRWeight").Value)
                otestRs.MoveNext()
            Next
            Dim intJobCode, strqry As String
            oRec.DoQuery("select U_Z_HR_JobstCode  from  OHEM  where empid=" & CInt(EmpID))
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
            oGeneralService.Add(oGeneralData1)
            oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                strqry = " Update [@Z_HR_OSEAPP] Set U_Z_GRef = (Select Max(DocEntry) From [@Z_HR_OSEAPP]) Where DocEntry = " & DocNo & ""
                otestRs.DoQuery(strqry)
            End If
            '   ReDataBind()
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            End If
            Return False
        End Try
    End Function
#End Region

#Region "Menu Event"
    Public Overrides Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            Select Case pVal.MenuUID
                
            End Select
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
#End Region
End Class
