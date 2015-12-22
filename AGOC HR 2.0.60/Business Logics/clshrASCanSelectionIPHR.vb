Imports System.IO
Public Class clshrASCanSelectionIPHR
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oItem As SAPbouiCOM.Items

    Public Sub New()
        MyBase.New()
    End Sub

    Private Sub LoadForm(ByVal intType As Int16, ByVal Title As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_CReqSelIPHR) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_CReqSelIPHR, frm_hr_CReqSelIPHR)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Title = Title
        oForm.Freeze(True)
        AddChooseFromList(oForm)

        fillDocumentType(oForm)
        oForm.DataSources.UserDataSources.Add("ReqNo", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        'oForm.DataSources.UserDataSources.Add("DocType", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "6", "ReqNo")
        'oApplication.Utilities.setUserDatabind(oForm, "4", "DocType")

        oCombobox = oForm.Items.Item("4").Specific
        oCombobox.Select(intType, SAPbouiCOM.BoSearchKey.psk_Index)
        oForm.Items.Item("4").DisplayDesc = True
        oForm.Items.Item("6").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oForm.Items.Item("4").Enabled = False

        'Assign CF
        oEditText = oForm.Items.Item("6").Specific
        oEditText.ChooseFromListUID = "CFL1"
        oEditText.ChooseFromListAlias = "DocEntry"
        oForm.Freeze(False)
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

            ' Adding 1 CFL, one for the button and one for the edit text.
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

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_CReqSelIPHR Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "_1" Then
                                    If validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "6" Then

                                End If
                        End Select
                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "_1" Then
                                    Dim objASA As New clsAppShortListed
                                    Dim objIPPRocess As New ClshrIPProcessForm
                                    Dim objIPOA As New ClshrIPOfferAcceptance
                                    oCombobox = oForm.Items.Item("4").Specific
                                    Select Case oCombobox.Selected.Value
                                        Case "LM"
                                            objASA.LoadForm("LM", oForm.Items.Item("6").Specific.value.ToString)
                                        Case "SM"
                                            objASA.LoadForm("SM", oForm.Items.Item("6").Specific.value.ToString)
                                        Case "IPLM"
                                            objIPPRocess.LoadForm("IPLM", oForm.Items.Item("6").Specific.value.ToString)
                                        Case "IPHOD"
                                            objIPPRocess.LoadForm("IPHOD", oForm.Items.Item("6").Specific.value.ToString)
                                        Case "IPHR"
                                            objIPPRocess.LoadForm("IPHR", oForm.Items.Item("6").Specific.value.ToString)
                                        Case "IPOA"
                                            objIPOA.LoadForm("IPOA", oForm.Items.Item("6").Specific.value.ToString)
                                        Case "IPLMU"
                                            objIPPRocess.LoadForm("IPLMU", oForm.Items.Item("6").Specific.value.ToString)
                                    End Select
                                    oForm.Close()
                                ElseIf pVal.ItemUID = "6" Then
                                    oForm.Close()
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1 As String
                                Dim sCHFL_ID As String
                                Try
                                    oCFLEvento = pVal
                                    sCHFL_ID = oCFLEvento.ChooseFromListUID
                                    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                    oCFL = oForm.ChooseFromLists.Item(sCHFL_ID)
                                    If (oCFLEvento.BeforeAction = False) Then
                                        Dim oDataTable As SAPbouiCOM.DataTable
                                        oDataTable = oCFLEvento.SelectedObjects
                                        oForm.Freeze(True)
                                        If pVal.ItemUID = "6" Then
                                            val1 = oDataTable.GetValue("DocEntry", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "6", val1)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        oForm.Freeze(False)
                                    End If
                                Catch ex As Exception
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
                Case mnu_hr_ASLMApproval
                    LoadForm(0, "Line Manager - Approval Selection")
                Case mnu_hr_ASSMApproval
                    LoadForm(1, "Senior Manager - Approval Selection")
                Case mnu_hr_IPLM
                    LoadForm(2, "Interview Scheduling - Approval Selection")
                Case mnu_hr_IPHOD
                    LoadForm(3, "Final Candidate Selection (HOD) - Approval Selection")
                Case mnu_hr_IPHR
                    LoadForm(4, "Final Candidate Selection (HR) - Approval Selection")
                Case mnu_hr_OAcceptance
                    LoadForm(5, "Offer Acceptance - Approval Selection ")
                Case mnu_hr_IPLMU
                    LoadForm(6, "Update Interview Status - Approval Selection")
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

            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Function validation(ByVal oForm As SAPbouiCOM.Form) As Boolean
        Dim _retVal As Boolean = True
        oEditText = oForm.Items.Item("6").Specific
        If oEditText.Value.Length = 0 Then
            oApplication.Utilities.Message("Select Request No To Proceed...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            _retVal = False
        End If
        Return _retVal
    End Function

    Private Sub fillDocumentType(ByVal oForm As SAPbouiCOM.Form)
        oCombobox = oForm.Items.Item("4").Specific
        oCombobox.ValidValues.Add("LM", "Line Mananger Approval")
        oCombobox.ValidValues.Add("SM", "Senior Manager Approval")
        oCombobox.ValidValues.Add("IPLM", "Line Mananger Approval")
        oCombobox.ValidValues.Add("IPHOD", "Senior Manager Approval")
        oCombobox.ValidValues.Add("IPHR", "HR Approval")
        oCombobox.ValidValues.Add("IPOA", "Offer Acceptance")
        oCombobox.ValidValues.Add("IPLMU", "Line Manager Approval - Update")
    End Sub

End Class
