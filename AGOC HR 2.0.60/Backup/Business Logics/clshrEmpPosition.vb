Public Class clshrEmpPosition
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
    Public intNewnumber As Double
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_empPosition) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_empPosition, frm_hr_empPosition)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.DataBrowser.BrowseBy = "6"
        oForm.EnableMenu("1283", True)
        FillDivision(oForm)
        FillDepartment(oForm)
        FillPosition(oForm)
        'AddChooseFromList(oForm)
        'databind(oForm)
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
            oForm.Items.Item("4").Enabled = False
            oForm.Items.Item("6").Enabled = False
        Else
            oForm.Items.Item("4").Enabled = True
            oForm.Items.Item("6").Enabled = True
        End If
        oForm.Items.Item("4").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        'AddMode(oForm)
        oForm.Freeze(False)
    End Sub
    Public Sub LoadForm1(ByVal PosCode As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_empPosition) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_empPosition, frm_hr_empPosition)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.EnableMenu("1283", True)
        FillDivision(oForm)
        FillDepartment(oForm)
        FillPosition(oForm)
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oForm.Items.Item("8").Enabled = True
        oApplication.Utilities.setEdittextvalue(oForm, "8", PosCode)
        oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE Then
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        End If
        oForm.Freeze(False)
    End Sub
#Region "Methods"
    Private Sub FillDivision(ByVal sform As SAPbouiCOM.Form)
        oCombobox = sform.Items.Item("36").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            Try
                oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
            Catch ex As Exception

            End Try
        Next
        oSlpRS.DoQuery("Select Name,Remarks from OUBR order by Code")
        oSlpRS.DoQuery("SELECT T0.[U_Z_FuncCode], T0.[U_Z_FuncName] FROM [dbo].[@Z_HR_OFCA]  T0 order by T0.DocEntry")
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        sform.Items.Item("36").DisplayDesc = True
    End Sub

    Private Sub FillDepartment(ByVal sform As SAPbouiCOM.Form)
        oCombobox = sform.Items.Item("20").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            Try
                oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
            Catch ex As Exception

            End Try
        Next


        oSlpRS.DoQuery("Select Code,Name from OUDP order by Code")
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        sform.Items.Item("20").DisplayDesc = True
    End Sub
    Private Sub FillPosition(ByVal sform As SAPbouiCOM.Form)
        oCombobox = sform.Items.Item("16").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        For intRow As Integer = oCombobox.ValidValues.Count - 1 To 0 Step -1
            Try
                oCombobox.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
            Catch ex As Exception

            End Try
        Next
        oSlpRS.DoQuery("Select ""U_Z_PosCode"",""U_Z_PosName"" From ""@Z_HR_OPOSIN""")
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            Try
                oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            Catch ex As Exception

            End Try

            oSlpRS.MoveNext()
        Next
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        sform.Items.Item("16").DisplayDesc = True
    End Sub
    Private Sub BindCompany(ByVal aform As SAPbouiCOM.Form, ByVal strOrgCode As String)
        Dim strqry As String
        Dim oTemp As SAPbobsCOM.Recordset
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Try
            strqry = "Select U_Z_CompCode,U_Z_CompName from [@Z_HR_ORGST] where U_Z_OrgCode='" & strOrgCode & "' "
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.setEdittextvalue(aform, "34", oTemp.Fields.Item("U_Z_CompName").Value)
                Try
                    oApplication.Utilities.setEdittextvalue(aform, "32", oTemp.Fields.Item("U_Z_CompCode").Value)
                Catch ex As Exception
                End Try
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try

    End Sub

    Private Sub AddMode(ByVal aform As SAPbouiCOM.Form)
        Dim strCode As String
        strCode = oApplication.Utilities.getMaxCode("@Z_HR_OPOSIN", "DocEntry")
        aform.Items.Item("6").Enabled = True
        aform.Items.Item("4").Enabled = True
        oApplication.Utilities.setEdittextvalue(aform, "6", strCode)
        aform.Items.Item("4").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oApplication.Utilities.setEdittextvalue(aform, "4", "t")
        oApplication.SBO_Application.SendKeys("{TAB}")
        aform.Items.Item("8").Enabled = True
        aform.Items.Item("8").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        aform.Items.Item("6").Enabled = False
        aform.Items.Item("4").Enabled = False
        ' oApplication.Utilities.setEdittextvalue(aform, "8", "0")
    End Sub
#End Region
#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            If oApplication.Utilities.getEdittextvalue(aForm, "8") = "" Then
                oApplication.Utilities.Message("Enter Position Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oForm.Items.Item("8").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "10") = "" Then
                oApplication.Utilities.Message("Enter Position Description...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oForm.Items.Item("10").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "12") = "" Then
                oApplication.Utilities.Message("Enter Job Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oForm.Items.Item("12").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If

           
            Dim str As String
            oCombobox = aForm.Items.Item("20").Specific
            Try
                str = oCombobox.Selected.Value
            Catch ex As Exception
                str = ""
            End Try
            If str = "" Then
                oApplication.Utilities.Message("Department Details is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oForm.Items.Item("20").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If

            If oApplication.Utilities.getEdittextvalue(aForm, "32") = "" Then
                oApplication.Utilities.Message("Company Code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oForm.Items.Item("32").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If
            oCombobox = aForm.Items.Item("36").Specific
            Try
                str = oCombobox.Selected.Value
            Catch ex As Exception
                str = ""
            End Try
            If str = "" Then
                oApplication.Utilities.Message("Division Details is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oForm.Items.Item("36").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If



            Dim oTemp As SAPbobsCOM.Recordset
            Dim stSQL As String
            If aForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                AddMode(oForm)
                oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                stSQL = "Select * from [@Z_HR_OPOSIN] where U_Z_PosCode='" & oApplication.Utilities.getEdittextvalue(aForm, "8") & "'"
                oTemp.DoQuery(stSQL)
                If oTemp.RecordCount > 0 Then
                    oApplication.Utilities.Message("Position Code Already Exists...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            End If

            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
    Private Function VacantPosition(ByVal ExpEmp As Integer, ByVal CurrEmp As Integer) As Integer
        Dim VacEmp As Integer
        VacEmp = ExpEmp - CurrEmp
        Return VacEmp
    End Function

#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_EmpPosition Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE And (pVal.ItemUID = "4" Or pVal.ItemUID = "6") And pVal.CharPressed <> 9 Then
                                    BubbleEvent = False
                                    Exit Sub
                                End If

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
                                If pVal.ItemUID = "41" Then
                                    Dim strcode As String = oApplication.Utilities.getEdittextvalue(oForm, "24")
                                   oApplication.Utilities.OpenMasterinLink(oForm, "Salary", strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                ElseIf pVal.ItemUID = "42" Then
                                    Dim strcode As String = oApplication.Utilities.getEdittextvalue(oForm, "12")
                                     oApplication.Utilities.OpenMasterinLink(oForm, "JobScreen", strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                ElseIf pVal.ItemUID = "43" Then
                                    oApplication.Utilities.OpenMasterinLink(oForm, "Department")
                                    BubbleEvent = False
                                    Exit Sub
                                ElseIf pVal.ItemUID = "44" Then
                                    oApplication.Utilities.OpenMasterinLink(oForm, "Company")
                                    BubbleEvent = False
                                    Exit Sub
                                ElseIf pVal.ItemUID = "45" Then
                                    oApplication.Utilities.OpenMasterinLink(oForm, "Function")
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "36" Then
                                    Dim stCode2, stCode3 As String
                                    oCombobox = oForm.Items.Item("36").Specific
                                    stCode3 = oCombobox.Selected.Value
                                    stCode2 = oCombobox.Selected.Description
                                    oApplication.Utilities.setEdittextvalue(oForm, "38", stCode2)
                                ElseIf pVal.ItemUID = "20" Then
                                    Dim stCode2, stCode3 As String
                                    oCombobox = oForm.Items.Item("20").Specific
                                    stCode3 = oCombobox.Selected.Value
                                    Dim oTest As SAPbobsCOM.Recordset
                                    oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    oTest.DoQuery("Select Remarks from OUDP where Code=" & CDbl(stCode3))

                                    stCode2 = oTest.Fields.Item(0).Value ' oCombobox.Selected.Description
                                    oApplication.Utilities.setEdittextvalue(oForm, "22", stCode2)
                                ElseIf pVal.ItemUID = "16" Then
                                    Dim stCode2, stCode3 As String
                                    oCombobox = oForm.Items.Item("16").Specific
                                    stCode3 = oCombobox.Selected.Value
                                    stCode2 = oCombobox.Selected.Description
                                    oApplication.Utilities.setEdittextvalue(oForm, "18", stCode2)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1282" Then
                                    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                    If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                        oForm.Items.Item("4").Enabled = False
                                        oForm.Items.Item("6").Enabled = False
                                    Else
                                        oForm.Items.Item("4").Enabled = True
                                        oForm.Items.Item("6").Enabled = True
                                    End If
                                End If
                                If pVal.ItemUID = "26" And pVal.CharPressed = 9 Then
                                    Dim Expemp, CurEmp, vacPos As Integer
                                    Expemp = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(oForm, "28"))
                                    CurEmp = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(oForm, "26"))
                                    vacPos = VacantPosition(Expemp, CurEmp)
                                    oApplication.Utilities.setEdittextvalue(oForm, "30", vacPos)
                                End If
                                If pVal.ItemUID = "28" And pVal.CharPressed = 9 Then
                                    Dim Expemp, CurEmp, vacPos As Integer
                                    Expemp = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(oForm, "28"))
                                    CurEmp = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(oForm, "26"))
                                    vacPos = VacantPosition(Expemp, CurEmp)
                                    oApplication.Utilities.setEdittextvalue(oForm, "30", vacPos)
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "1"
                                        If oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                                            AddMode(oForm)
                                        End If
                                End Select
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1, Val7 As String
                                Dim sCHFL_ID, val, val2, val3, val4, val5, val6 As String
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
                                        If pVal.ItemUID = "12" Then
                                            Try
                                                val = oDataTable.GetValue("U_Z_PosCode", 0)
                                                val1 = oDataTable.GetValue("U_Z_PosName", 0)
                                                val6 = oDataTable.GetValue("U_Z_SalCode", 0)
                                                Val7 = oDataTable.GetValue("U_Z_DivCode", 0)
                                                oCombobox = oForm.Items.Item("36").Specific
                                                BindCompany(oForm, val)
                                                ' oCombobox.Select(Val7, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                                oApplication.Utilities.setEdittextvalue(oForm, "24", val6)
                                                oApplication.Utilities.setEdittextvalue(oForm, "14", val1)
                                                oApplication.Utilities.setEdittextvalue(oForm, "12", val)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "32" Then
                                            Try
                                                val = oDataTable.GetValue("U_Z_CompCode", 0)
                                                val1 = oDataTable.GetValue("U_Z_CompName", 0)
                                                oApplication.Utilities.setEdittextvalue(oForm, "34", val1)
                                                oApplication.Utilities.setEdittextvalue(oForm, "32", val)
                                            Catch ex As Exception

                                            End Try
                                        End If
                                        oForm.Freeze(False)
                                    End If
                                Catch ex As Exception
                                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                        oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                    End If
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
                Case mnu_hr_EmpPosition
                    LoadForm()
                Case mnu_ADD
                  
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                        oForm.Items.Item("4").Enabled = False
                        oForm.Items.Item("6").Enabled = False
                    End If
                    If pVal.BeforeAction = False Then
                        AddMode(oForm)
                    End If
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
                    If Form.TypeEx = frm_hr_EmpPosition Then
                        Dim oRec As SAPbobsCOM.Recordset
                        Dim strPosCode = oApplication.Utilities.getEdittextvalue(oForm, "8")
                        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oRec.DoQuery("select * from OHEM where U_Z_HR_Posicode='" & strPosCode.trim() & "'")
                        'If oRec.EoF Then
                        '    oApplication.Utilities.setEdittextvalue(oForm, "26", "0")
                        'Else
                        '    oApplication.Utilities.setEdittextvalue(oForm, "26", "" & oRec.RecordCount() & "")
                        'End If
                        Dim Expemp, CurEmp, vacPos As Integer
                        Expemp = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(oForm, "28"))
                        CurEmp = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(oForm, "26"))
                        vacPos = VacantPosition(Expemp, CurEmp)
                        oApplication.Utilities.setEdittextvalue(oForm, "30", vacPos)
                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
                        End If
                    End If
                Case mnu_FIND
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If Form.TypeEx = frm_hr_empPosition Then
                        oForm.Items.Item("8").Enabled = True
                    End If
                Case "1283"
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = True Then
                        Dim strValue As String
                        If oApplication.SBO_Application.MessageBox("Do you want to delete the details?", , "Yes", "No") = 2 Then
                            BubbleEvent = False
                            Exit Sub
                        End If
                        strValue = oApplication.Utilities.getEdittextvalue(oForm, "8")
                        If oApplication.Utilities.ValidateCode(strValue, "POSITION") = True Then
                            BubbleEvent = False
                            Exit Sub
                        Else
                            Dim st As SAPbobsCOM.Recordset
                            st = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            st.DoQuery("Delete from OHPS where ""Name""='" & strValue & "'")

                        End If
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
            If BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD Or BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
                If oForm.TypeEx = frm_hr_EmpPosition Then
                    Dim oTest, otest1 As SAPbobsCOM.Recordset
                    Dim intNumber, intPosID As Integer
                    Dim strpositionname, posCode, strstring, strFrgnName As String
                    oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    otest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oTest.DoQuery("Select isnull(max(posid),0)+1 from OHPS")
                    intNumber = oTest.Fields.Item(0).Value
                    otest1.DoQuery("select * from [@Z_HR_OPOSIN]  where docentry=" & intNewnumber)
                    If otest1.RecordCount > 0 Then
                        strpositionname = otest1.Fields.Item("U_Z_PosName").Value
                        posCode = otest1.Fields.Item("U_Z_PosCode").Value
                        strFrgnName = otest1.Fields.Item("U_Z_FrgnName").Value
                        '  strstring = "Select * from ohps where U_Z_POSRef='" & CInt(intNewnumber) & "'"
                        strstring = "Select * from ohps where Name='" & posCode & "'"
                        oTest.DoQuery(strstring)
                        If oTest.RecordCount <= 0 Then
                            Try
                                strstring = "Insert into OHPS (posID,name,descriptio,LocFields,U_Z_POSRef,U_Z_FrgnName) values (" & intNumber & ",'" & posCode & "','" & strpositionname & "','N','" & CInt(intNewnumber) & "','" & strFrgnName & "')"
                                otest1.DoQuery(strstring)
                            Catch ex As Exception
                            End Try
                        Else
                            intPosID = oTest.Fields.Item("posID").Value
                            Try
                                otest1.DoQuery("Update ohps set Name='" & posCode & "',Descriptio='" & strpositionname & "',U_Z_POSRef='" & CInt(intNewnumber) & "' ,U_Z_FrgnName='" & strFrgnName & "' where Name='" & posCode & "'")
                            Catch ex As Exception

                            End Try
                        End If
                    End If
                End If
            End If

            If BusinessObjectInfo.BeforeAction = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD Or BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
                If oForm.TypeEx = frm_hr_empPosition Then
                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                        intNewnumber = CInt(oApplication.Utilities.getMaxCode("@Z_HR_OPOSIN", "DocEntry"))
                    Else
                        intNewnumber = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(oForm, "6"))
                    End If
                End If
            End If

            If BusinessObjectInfo.BeforeAction = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
                If oForm.TypeEx = frm_hr_empPosition Then
                    oForm.Items.Item("10").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    oForm.Items.Item("8").Enabled = False
                    oForm.Items.Item("4").Enabled = False
                    oForm.Items.Item("6").Enabled = False
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

End Class
