Public Class clshrPosition
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
    Dim dt As Date
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_Position) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If

        oForm = oApplication.Utilities.LoadForm(xml_hr_Position, frm_hr_Position)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.DataBrowser.BrowseBy = "121"
        FillDivision(oForm)
        FillDepartment(oForm)
        FillPosition(oForm)
        AddChooseFromList(oForm)
        databind(oForm)
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        'AddMode(oForm)
        oForm.Freeze(False)
    End Sub
    Private Sub FillDivision(ByVal sform As SAPbouiCOM.Form)
        oCombobox = sform.Items.Item("38").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select Name,Remarks from OUBR order by Code")
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        sform.Items.Item("38").DisplayDesc = True
    End Sub

    Private Sub FillDepartment(ByVal sform As SAPbouiCOM.Form)
        oCombobox = sform.Items.Item("45").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select Code,Remarks from OUDP order by Code")
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        sform.Items.Item("45").DisplayDesc = True
    End Sub
    Private Sub FillPosition(ByVal sform As SAPbouiCOM.Form)
        oCombobox = sform.Items.Item("46").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select U_Z_PosCode,U_Z_PosName From [@Z_HR_OPOSIN]")
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
        oCombobox.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        sform.Items.Item("46").DisplayDesc = True
    End Sub

    Private Sub BindCompany(ByVal aform As SAPbouiCOM.Form, ByVal strOrgCode As String)
        Dim strqry As String
        Dim oTemp As SAPbobsCOM.Recordset
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Try
            strqry = "Select U_Z_CompCode,U_Z_CompName from [@Z_HR_ORGST] where U_Z_OrgCode='" & strOrgCode & "' "
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.setEdittextvalue(aform, "36", oTemp.Fields.Item("U_Z_CompName").Value)
                Try
                    oApplication.Utilities.setEdittextvalue(aform, "34", oTemp.Fields.Item("U_Z_CompCode").Value)
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
        aform.Items.Item("121").Enabled = True
        aform.Items.Item("22").Enabled = True
        oApplication.Utilities.setEdittextvalue(aform, "121", strCode)
        aform.Items.Item("22").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oApplication.Utilities.setEdittextvalue(aform, "22", "t")
        oApplication.SBO_Application.SendKeys("{TAB}")
        oForm.Items.Item("4").Enabled = True
        aform.Items.Item("4").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        aform.Items.Item("121").Enabled = False
        aform.Items.Item("22").Enabled = False
        oApplication.Utilities.setEdittextvalue(aform, "30", "")
    End Sub
#Region "Add Choose From List"
    Private Sub databind(ByVal aForm As SAPbouiCOM.Form)

        oEditText = aForm.Items.Item("12").Specific
        oEditText.ChooseFromListUID = "CFL1"
        oEditText.ChooseFromListAlias = "U_Z_PosCode"

        oEditText = aForm.Items.Item("16").Specific
        oEditText.ChooseFromListUID = "CFL2"
        oEditText.ChooseFromListAlias = "U_Z_OrgCode"

        'oEditText = aForm.Items.Item("8").Specific
        'oEditText.ChooseFromListUID = "CFL3"
        'oEditText.ChooseFromListAlias = "empID"

        oEditText = aForm.Items.Item("34").Specific
        oEditText.ChooseFromListUID = "CFL4"
        oEditText.ChooseFromListAlias = "U_Z_CompCode"

        'oEditText = aForm.Items.Item("8").Specific
        'oEditText.ChooseFromListUID = "CFL5"
        'oEditText.ChooseFromListAlias = "U_Z_PosCode"

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
            oCFLCreationParams.ObjectType = "Z_HR_EPOCOM"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)


            oCFLCreationParams.ObjectType = "Z_HR_ORGST"
            oCFLCreationParams.UniqueID = "CFL2"
            oCFL = oCFLs.Add(oCFLCreationParams)


            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "171"
            oCFLCreationParams.UniqueID = "CFL3"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_OADM"
            oCFLCreationParams.UniqueID = "CFL4"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_OPOSIN"
            oCFLCreationParams.UniqueID = "CFL5"
            oCFL = oCFLs.Add(oCFLCreationParams)

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

#End Region
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim strcode, strDivision As String
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
           
            If oApplication.Utilities.getEdittextvalue(aForm, "4") = "" Then
                oApplication.Utilities.Message("Enter Position Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "6") = "" Then
                oApplication.Utilities.Message("Enter Position Description...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "16") = "" Then
                '  oApplication.Utilities.Message("Organization Code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '  Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "8") = "" Then
                ' oApplication.Utilities.Message("Reporting To Code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '  Return False
            End If
            If oApplication.Utilities.getEdittextvalue(aForm, "12") = "" Then
                oApplication.Utilities.Message("Enter Job Code...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            Dim oTemp As SAPbobsCOM.Recordset
            Dim stSQL As String
            If aForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                stSQL = "Select * from [@Z_HR_OPOSIN] where U_Z_PosCode='" & oApplication.Utilities.getEdittextvalue(aForm, "4") & "'"
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

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_Position Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim strVal, strVal1, strVal2 As String
                                If pVal.CharPressed <> "9" And pVal.ItemUID = "4" Then
                                    strVal = oApplication.Utilities.getEdittextvalue(oForm, "4")
                                    If oApplication.Utilities.ValidateCode(strVal, "POSITION") = True Then
                                        oApplication.Utilities.Message("Position Code Already Exists...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                               
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.CharPressed <> 9 And (pVal.ItemUID = "36") Then
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
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)\\

                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "30" And pVal.CharPressed = 9 Then
                                    Dim Expemp, CurEmp, vacPos As Integer
                                    Expemp = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(oForm, "28"))
                                    CurEmp = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(oForm, "30"))
                                    vacPos = VacantPosition(Expemp, CurEmp)
                                    oApplication.Utilities.setEdittextvalue(oForm, "32", vacPos)
                                End If
                                If pVal.ItemUID = "28" And pVal.CharPressed = 9 Then
                                    Dim Expemp, CurEmp, vacPos As Integer
                                    Expemp = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(oForm, "28"))
                                    CurEmp = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(oForm, "30"))
                                    vacPos = VacantPosition(Expemp, CurEmp)
                                    oApplication.Utilities.setEdittextvalue(oForm, "32", vacPos)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "38" Then
                                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                                        Dim stCode2, stCode3 As String
                                        oCombobox = oForm.Items.Item("38").Specific
                                        stCode3 = oCombobox.Selected.Value
                                        stCode2 = oCombobox.Selected.Description
                                        oApplication.Utilities.setEdittextvalue(oForm, "40", stCode2)
                                    End If
                                ElseIf pVal.ItemUID = "45" Then
                                    Dim stCode2, stCode3 As String
                                    oCombobox = oForm.Items.Item("45").Specific
                                    stCode3 = oCombobox.Selected.Value
                                    stCode2 = oCombobox.Selected.Description
                                    oApplication.Utilities.setEdittextvalue(oForm, "44", stCode2)
                                ElseIf pVal.ItemUID = "46" Then
                                    Dim stCode2, stCode3 As String
                                    oCombobox = oForm.Items.Item("46").Specific
                                    stCode3 = oCombobox.Selected.Value
                                    stCode2 = oCombobox.Selected.Description
                                    oApplication.Utilities.setEdittextvalue(oForm, "10", stCode2)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1, Val7 As String
                                Dim sCHFL_ID, val, val2, val3, val4, val5, val6 As String
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
                                        If pVal.ItemUID = "12" Then
                                            val = oDataTable.GetValue("U_Z_PosCode", 0)
                                            val1 = oDataTable.GetValue("U_Z_PosName", 0)
                                            val2 = oDataTable.GetValue("U_Z_OrgCode", 0)
                                            val3 = oDataTable.GetValue("U_Z_OrgDesc", 0)
                                            val4 = oDataTable.GetValue("U_Z_ReportTo", 0)
                                            val5 = oDataTable.GetValue("U_Z_RptName", 0)
                                            val6 = oDataTable.GetValue("U_Z_SalCode", 0)
                                            Val7 = oDataTable.GetValue("U_Z_DivCode", 0)
                                            oCombobox = oForm.Items.Item("38").Specific
                                            Try
                                                BindCompany(oForm, val2)
                                                oCombobox.Select(Val7, SAPbouiCOM.BoSearchKey.psk_ByValue)
                                                oApplication.Utilities.setEdittextvalue(oForm, "26", val6)
                                                oApplication.Utilities.setEdittextvalue(oForm, "10", val5)
                                                oApplication.Utilities.setEdittextvalue(oForm, "8", val4)
                                                oApplication.Utilities.setEdittextvalue(oForm, "14", val1)
                                                oApplication.Utilities.setEdittextvalue(oForm, "12", val)

                                            Catch ex As Exception
                                                'If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                                '    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                'End If
                                            End Try
                                            oApplication.Utilities.setEdittextvalue(oForm, "18", val3)
                                            oApplication.Utilities.setEdittextvalue(oForm, "16", val2)
                                        End If
                                        If pVal.ItemUID = "34" Then
                                            val = oDataTable.GetValue("U_Z_CompCode", 0)
                                            val1 = oDataTable.GetValue("U_Z_CompName", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "36", val1)
                                                oApplication.Utilities.setEdittextvalue(oForm, "34", val)
                                            Catch ex As Exception
                                                'If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                                '    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                'End If
                                            End Try
                                        End If
                                        If pVal.ItemUID = "16" Then
                                            val = oDataTable.GetValue("U_Z_OrgCode", 0)
                                            val1 = oDataTable.GetValue("U_Z_OrgDesc", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "18", val1)
                                                oApplication.Utilities.setEdittextvalue(oForm, "16", val)
                                            Catch ex As Exception
                                                'If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                                '    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                'End If
                                            End Try
                                        End If
                                        If pVal.ItemUID = "8" Then
                                            val = oDataTable.GetValue("U_Z_PosCode", 0)
                                            val1 = oDataTable.GetValue("U_Z_PosName", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "8", val1)
                                                oApplication.Utilities.setEdittextvalue(oForm, "10", val)
                                            Catch ex As Exception
                                                'If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                                                '    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                'End If
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
                Case mnu_hr_Position
                    LoadForm()
                Case mnu_ADD
                    If pVal.BeforeAction = False Then
                        AddMode(oForm)
                    End If

                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
                    If Form.TypeEx = frm_hr_Position Then
                        Dim oRec As SAPbobsCOM.Recordset
                        Dim strPosCode = oApplication.Utilities.getEdittextvalue(oForm, "4")
                        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oRec.DoQuery("select * from OHEM where U_Z_HR_Posicode='" & strPosCode.trim() & "'")
                        If oRec.EoF Then
                            oApplication.Utilities.setEdittextvalue(oForm, "30", "0")
                        Else
                            oApplication.Utilities.setEdittextvalue(oForm, "30", "" & oRec.RecordCount() & "")
                        End If
                        Dim Expemp, CurEmp, vacPos As Integer
                        Expemp = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(oForm, "28"))
                        CurEmp = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(oForm, "30"))
                        vacPos = VacantPosition(Expemp, CurEmp)
                        oApplication.Utilities.setEdittextvalue(oForm, "32", vacPos)
                    End If
                Case mnu_FIND
                        oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If Form.TypeEx = frm_hr_Position And pVal.BeforeAction = False Then
                        oForm.Items.Item("4").Enabled = True
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
                If oForm.TypeEx = frm_hr_Position Then
                    Dim oTest, otest1 As SAPbobsCOM.Recordset
                    Dim intNumber, intPosID As Integer
                    Dim strpositionname, posCode, strstring As String
                    oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    otest1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    Dim st As String
                    oTest.DoQuery("Select isnull(max(posid),0)+1 from OHPS")
                    intNumber = oTest.Fields.Item(0).Value
                    otest1.DoQuery("select * from [@Z_HR_OPOSIN]  where docentry=" & intNewnumber)
                    If otest1.RecordCount > 0 Then
                        strpositionname = otest1.Fields.Item("U_Z_PosName").Value
                        posCode = otest1.Fields.Item("U_Z_PosCode").Value
                        strstring = "Select * from ohps where U_Z_POSRef='" & CInt(intNewnumber) & "'"
                        oTest.DoQuery(strstring)
                        If oTest.RecordCount <= 0 Then
                            strstring = "Insert into OHPS (posID,name,descriptio,LocFields,U_Z_POSRef) values (" & intNumber & ",'" & posCode & "','" & strpositionname & "','N','" & CInt(intNewnumber) & "')"
                            otest1.DoQuery(strstring)
                        Else
                            intPosID = oTest.Fields.Item("posID").Value
                            otest1.DoQuery("Update ohps set Name='" & posCode & "',Descriptio='" & strpositionname & "',U_Z_POSRef='" & CInt(intNewnumber) & "'  where U_Z_POSRef='" & CInt(intNewnumber) & "'")
                        End If
                    End If
                End If
            End If
            If BusinessObjectInfo.BeforeAction = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD Or BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
                If oForm.TypeEx = frm_hr_Position Then
                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Then
                        intNewnumber = CInt(oApplication.Utilities.getMaxCode("@Z_HR_OPOSIN", "DocEntry"))
                    Else
                        intNewnumber = oApplication.Utilities.getDocumentQuantity(oApplication.Utilities.getEdittextvalue(oForm, "121"))
                    End If
                End If
            End If

            If BusinessObjectInfo.BeforeAction = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
                If oForm.TypeEx = frm_hr_Position Then
                    oForm.Items.Item("6").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    oForm.Items.Item("4").Enabled = False
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

End Class
