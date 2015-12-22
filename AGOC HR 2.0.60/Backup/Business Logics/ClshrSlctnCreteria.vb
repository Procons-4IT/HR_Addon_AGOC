Public Class ClshrSlctnCreteria
    Inherits clsBase
    Private InvForConsumedItems As Integer
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText, oEditFDate, oEditTDate As SAPbouiCOM.EditText
    Private oCombobox, oCombobox1, oCombobox2, oCombobox3 As SAPbouiCOM.ComboBox
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
    Private blnFlag As Boolean = False



    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub

    Public Sub LoadForm(ByVal DocType As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_SlctnCreteria) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_SlctnCreteria, frm_hr_SlctnCreteria)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        FillDepartment(oForm)
        FillPeriod(oForm)
        AddChooseFromList(oForm)
        oForm.DataSources.UserDataSources.Add("EmpNo1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "3", "EmpNo1")
        oForm.DataSources.UserDataSources.Add("EmpNo2", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "5", "EmpNo2")
        oForm.DataSources.UserDataSources.Add("DocType", SAPbouiCOM.BoDataType.dt_LONG_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "12", "DocType")
        oEditText = oForm.Items.Item("3").Specific
        oEditText.ChooseFromListUID = "CFL1"
        oEditText.ChooseFromListAlias = "empId"
        oEditText = oForm.Items.Item("5").Specific
        oEditText.ChooseFromListUID = "CFL2"
        oEditText.ChooseFromListAlias = "empId"
        If DocType = "HRA" Then
            oApplication.Utilities.setEdittextvalue(oForm, "12", "HR Acceptance")
        Else
            oApplication.Utilities.setEdittextvalue(oForm, "12", "HR Grevence Acceptance")
        End If
        oForm.PaneLevel = 1
        oForm.Freeze(False)
    End Sub

#Region "Form Initialize"


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
            oCFLCreationParams.ObjectType = "171"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "171"
            oCFLCreationParams.UniqueID = "CFL2"
            oCFL = oCFLs.Add(oCFLCreationParams)


        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub FillDepartment(ByVal sform As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = sform.Items.Item("8").Specific
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oTempRec.DoQuery("Select Code,Remarks from OUDP")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item("Code").Value, oTempRec.Fields.Item("Remarks").Value)
            oTempRec.MoveNext()
        Next

        sform.Items.Item("8").DisplayDesc = True
    End Sub
    Private Sub FillPeriod(ByVal aForm As SAPbouiCOM.Form)
        Dim oTempRec As SAPbobsCOM.Recordset
        oTempRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oCombobox = aForm.Items.Item("10").Specific
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
        Next
        oCombobox.ValidValues.Add("", "")
        oTempRec.DoQuery("Select Code,Name from OFPR order by Code desc")
        For intRow As Integer = 0 To oTempRec.RecordCount - 1
            oCombobox.ValidValues.Add(oTempRec.Fields.Item("Code").Value, oTempRec.Fields.Item("Name").Value)
            oTempRec.MoveNext()
        Next
        aForm.Items.Item("10").DisplayDesc = True
    End Sub
#End Region

#Region "Validation"
    Private Function Validate(ByVal oForm As SAPbouiCOM.Form) As Boolean
        Dim RetValue As Boolean = True
        'If oApplication.Utilities.getEdittextvalue(oForm, "3") = "" Then
        '    oApplication.Utilities.Message("Please Enter From Employee", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        '    Return RetValue = False
        'ElseIf oApplication.Utilities.getEdittextvalue(oForm, "5") = "" Then
        '    oApplication.Utilities.Message("Please Enter To Employee", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        '    Return RetValue = False
        'End If
        Dim oCombo2 As SAPbouiCOM.ComboBox
        oCombo2 = oForm.Items.Item("10").Specific
        Dim strPeriod As String = oCombo2.Selected.Description
        If strPeriod.Length = 0 Then
            oApplication.Utilities.Message("Select Financial Year...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            RetValue = False
        End If

        Return RetValue
    End Function
#End Region
    

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_SlctnCreteria Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                        End Select

                    Case False
                        Select Case pVal.EventType

                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1 As String
                                Dim sCHFL_ID As String
                                Dim intChoice As Integer


                                oCFLEvento = pVal
                                sCHFL_ID = oCFLEvento.ChooseFromListUID
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                oCFL = oForm.ChooseFromLists.Item(sCHFL_ID)
                                If (oCFLEvento.BeforeAction = False) Then
                                    Dim oDataTable As SAPbouiCOM.DataTable
                                    oDataTable = oCFLEvento.SelectedObjects
                                    intChoice = 0
                                    oForm.Freeze(True)

                                    If pVal.ItemUID = "3" Then
                                        val1 = oDataTable.GetValue("empID", 0)
                                        Try
                                            oApplication.Utilities.setEdittextvalue(oForm, "3", val1)
                                        Catch ex As Exception
                                        End Try
                                    End If
                                    If pVal.ItemUID = "5" Then
                                        val1 = oDataTable.GetValue("empID", 0)
                                        Try
                                            oApplication.Utilities.setEdittextvalue(oForm, "5", val1)
                                        Catch ex As Exception
                                        End Try
                                    End If
                                    oForm.Freeze(False)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                If pVal.ItemUID = "6" Then
                                    If Validate(oForm) Then
                                        If (oApplication.Utilities.getEdittextvalue(oForm, "12") = "HR Acceptance") Then
                                            Dim oCombo1, oCombo2 As SAPbouiCOM.ComboBox
                                            oCombo1 = oForm.Items.Item("8").Specific
                                            oCombo2 = oForm.Items.Item("10").Specific
                                            Dim strFromEmployee As String = oApplication.Utilities.getEdittextvalue(oForm, "3")
                                            Dim strToEmployee As String = oApplication.Utilities.getEdittextvalue(oForm, "5")
                                            Dim strDepartment As String = oCombo1.Selected.Value
                                            Dim strPeriod As String = oCombo2.Selected.Value
                                            Dim objct As New clshrApproval
                                            Dim strqry As String
                                            strqry = "select DocEntry,U_Z_EmpId ,U_Z_EmpName,U_Z_Date,U_Z_Period,case U_Z_Status when 'D' then 'Draft' when 'F' then 'Approved' when 'S'then '2nd Level Approval' when 'L' then 'Closed' else 'Canceled' end as U_Z_Status,case U_Z_WStatus when 'DR' then 'Draft' when 'HR' then 'HR Approved' when 'SM'then 'Sr.Manager Approved' when 'LM' then 'LineManager Approved'when 'SE' then 'SelfApproved'  end as 'U_Z_WStatus' from [@Z_HR_OSEAPP] Where U_Z_Period='" & strPeriod & "' "
                                            If strDepartment.Length > 0 Then
                                                strqry = strqry & "and U_Z_EmpID in (Select empId from OHEM where Dept='" & strDepartment & "')"
                                            End If
                                            If strFromEmployee.Length > 0 And strToEmployee.Length > 0 Then
                                                strqry = strqry & " and ( U_Z_EmpId Between " & strFromEmployee & " and " & strToEmployee & ")"
                                            End If
                                            Dim oRS As SAPbobsCOM.Recordset
                                            oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                            oRS.DoQuery(strqry)
                                            If Not oRS.EoF Then
                                                objct.LoadForm("HR", "", strFromEmployee, strToEmployee, strDepartment, strPeriod)
                                            Else
                                                oApplication.Utilities.Message("No Records Found", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            End If
                                        Else
                                            Dim oCombo1, oCombo2 As SAPbouiCOM.ComboBox
                                            oCombo1 = oForm.Items.Item("8").Specific
                                            oCombo2 = oForm.Items.Item("10").Specific
                                            Dim strFromEmployee As String = oApplication.Utilities.getEdittextvalue(oForm, "3")
                                            Dim strToEmployee As String = oApplication.Utilities.getEdittextvalue(oForm, "5")
                                            Dim strDepartment As String = oCombo1.Selected.Value
                                            Dim strPeriod As String = oCombo2.Selected.Value
                                            Dim objct As New ClshrGAcceptance
                                            Dim strqry As String
                                            strqry = "select DocEntry,U_Z_EmpId as 'Employee ID',(select OUDP.Name as 'Department' from OHEM JOIN OUDP on OHEM.dept=OUDP.Code where OHEM.empID=[@Z_HR_OSEAPP].U_Z_EmpId) as 'Department',U_Z_EmpName as 'Employee Name',U_Z_Date as 'Document Date',U_Z_Period as 'Period',U_Z_FDate as 'FromDate',U_Z_TDate as 'ToDate',case U_Z_WStatus when 'DR' then 'Draft' when 'HR' then 'HR Approved' when 'SM'then 'Sr.Manager Approved' when 'LM' then 'LineManager Approved'when 'SE' then 'SelfApproved'  end as 'Status' ,'' as 'Grevence Acceptance','' as 'Initailize Aproval'  from [@Z_HR_OSEAPP] Where U_Z_GStatus='G' and U_Z_Period='" & strPeriod & "' And ISNULL(U_Z_GRef,0) = 0"
                                            If strDepartment.Length > 0 Then
                                                strqry = strqry & "and U_Z_EmpID in (Select empId from OHEM where Dept='" & strDepartment & "')"
                                            End If
                                            If strFromEmployee.Length > 0 And strToEmployee.Length > 0 Then
                                                strqry = strqry & "  and ( U_Z_EmpId Between " & strFromEmployee & " and " & strToEmployee & ")"
                                            End If
                                            Dim oRS As SAPbobsCOM.Recordset
                                            oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                            oRS.DoQuery(strqry)
                                            If Not oRS.EoF Then
                                                objct.LoadForm(strFromEmployee, strToEmployee, strDepartment, strPeriod)
                                            Else
                                                oApplication.Utilities.Message("No Records Found", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                            End If
                                        End If
                                    End If
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

#Region "Menu Event"
    Public Overrides Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            Select Case pVal.MenuUID
                Case mnu_hr_HRAppr
                    LoadForm("HRA")
                Case mnu_hr_GAcceptance
                    LoadForm("HRGA")
            End Select
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
#End Region


End Class
