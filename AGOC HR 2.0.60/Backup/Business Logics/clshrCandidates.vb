Public Class clshrCandidates
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private ocombo1, ocombo2, ocombo3, ocombo4 As SAPbouiCOM.ComboBoxColumn
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
    Public Sub LoadForm(ByVal oForm As SAPbouiCOM.Form, ByVal canid As String, ByVal aChoice As String, Optional ByVal strTitle As String = "")
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_Candidate) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_Candidate, frm_hr_Candidate)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        Dim strcode As String
        If aChoice = "Candidate" Then
            oForm.PaneLevel = 1
            oForm.Title = "Interview Lists"
            oApplication.Utilities.CandidateLists(oForm, canid, aChoice)
        Else
            oForm.PaneLevel = 2
            oForm.Title = "Candidate Lists"
            oApplication.Utilities.CandidateLists(oForm, canid, aChoice, strTitle)
        End If
        oForm.Freeze(False)
    End Sub

#Region "Add to UDT"
    Private Function AddtoUDT(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim ousertable As SAPbobsCOM.UserTable
        Dim strCode, strremarks1, strremarks2, strremarks3, strChoice1, strChoice2, strChoice3, strFstdt, strSnddt, strTrddt, strChoice4 As String
        Dim Fstdt, Snddt, Trddt As Date
        Dim oCheckboxcolumn As SAPbouiCOM.CheckBoxColumn
        Dim otemprs, oTemp As SAPbobsCOM.Recordset
        aForm.Freeze(True)
        otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        ousertable = oApplication.Company.UserTables.Item("Z_HR_HEM1")
        oGrid = aForm.Items.Item("26").Specific
        Dim strChoice, strremarks, strAppid As String
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strCode = oGrid.DataTable.GetValue("Code", intRow)
            strAppid = oGrid.DataTable.GetValue("U_Z_HRAppID", intRow)

            ocombo1 = oGrid.Columns.Item("U_Z_1stRoundStatus")
            ocombo2 = oGrid.Columns.Item("U_Z_2ndRoundStatus")
            ocombo3 = oGrid.Columns.Item("U_Z_3rdRoundStatus")
            ocombo4 = oGrid.Columns.Item("U_Z_ApplStatus")
            strChoice1 = ocombo1.GetSelectedValue(intRow).Value
            strChoice2 = ocombo2.GetSelectedValue(intRow).Value
            strChoice3 = ocombo3.GetSelectedValue(intRow).Value
            Try
                strChoice4 = ocombo4.GetSelectedValue(intRow).Value
            Catch ex As Exception

            End Try

            strremarks1 = oGrid.DataTable.GetValue("U_Z_1stRoundRem", intRow)
            strremarks2 = oGrid.DataTable.GetValue("U_Z_2ndRoundRem", intRow)
            strremarks3 = oGrid.DataTable.GetValue("U_Z_3rdRoundRem", intRow)
            strFstdt = oGrid.DataTable.GetValue("U_Z_1stRounddt", intRow)
            strSnddt = oGrid.DataTable.GetValue("U_Z_2ndRounddt", intRow)
            strTrddt = oGrid.DataTable.GetValue("U_Z_3rdRounddt", intRow)
            Fstdt = oApplication.Utilities.GetDateTimeValue(strFstdt)
            Snddt = oApplication.Utilities.GetDateTimeValue(strSnddt)
            Trddt = oApplication.Utilities.GetDateTimeValue(strTrddt)
            If ousertable.GetByKey(strCode) Then
                ousertable.Code = strCode
                ousertable.Name = strCode
                If strFstdt <> "" Then
                    ousertable.UserFields.Fields.Item("U_Z_1stRounddt").Value = Fstdt
                Else
                    ousertable.UserFields.Fields.Item("U_Z_1stRounddt").Value = ""
                End If
                ousertable.UserFields.Fields.Item("U_Z_1stRoundRem").Value = strremarks1
                ousertable.UserFields.Fields.Item("U_Z_1stRoundStatus").Value = strChoice1
                If strSnddt <> "" Then
                    ousertable.UserFields.Fields.Item("U_Z_2ndRounddt").Value = Snddt
                Else
                    ousertable.UserFields.Fields.Item("U_Z_2ndRounddt").Value = ""
                End If
                ousertable.UserFields.Fields.Item("U_Z_2ndRoundRem").Value = strremarks2
                ousertable.UserFields.Fields.Item("U_Z_2ndRoundStatus").Value = strChoice2
                If strTrddt <> "" Then
                    ousertable.UserFields.Fields.Item("U_Z_3rdRounddt").Value = Trddt
                Else
                    ousertable.UserFields.Fields.Item("U_Z_3rdRounddt").Value = ""
                End If
                ousertable.UserFields.Fields.Item("U_Z_3rdRoundRem").Value = strremarks3
                ousertable.UserFields.Fields.Item("U_Z_3rdRoundStatus").Value = strChoice3
                If strChoice4 <> "P" Then
                    ousertable.UserFields.Fields.Item("U_Z_ApplStatus").Value = strChoice4
                End If

                If ousertable.Update <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                Else
                    oApplication.Utilities.Message("Interview Allocated successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)

                    'If strChoice4 = "S" Then
                    '    strSQL = "Update [@Z_HR_OCRAPP] set U_Z_Status='S' where DocEntry='" & strCode & "' "
                    'ElseIf strChoice4 = "R" Then
                    '    strSQL = "Update [@Z_HR_OCRAPP] set U_Z_Status='R' where DocEntry='" & strCode & "' "
                    If strChoice1 = "S" And strChoice2 = "S" And strChoice3 = "S" And strChoice4 <> "P" Then
                        strSQL = "Update [@Z_HR_OCRAPP] set U_Z_Status='A' where DocEntry='" & strAppid & "' and U_Z_Status<>'O'"
                        otemprs.DoQuery(strSQL)
                        strSQL = "Update [@Z_HR_HEM1] set U_Z_ApplStatus='S' where U_Z_HRAppID='" & strAppid & "' and U_Z_ApplStatus<>'P' "
                        oTemp.DoQuery(strSQL)
                    ElseIf strChoice1 = "S" And strChoice2 = "O" And strChoice3 = "O" And strChoice4 <> "P" Then
                        strSQL = "Update [@Z_HR_OCRAPP] set U_Z_Status='I' where DocEntry='" & strAppid & "' and U_Z_Status<>'O'"
                        otemprs.DoQuery(strSQL)
                    ElseIf strChoice1 = "S" And strChoice2 = "S" And strChoice3 = "O" And strChoice4 <> "P" Then
                        strSQL = "Update [@Z_HR_OCRAPP] set U_Z_Status='I' where DocEntry='" & strAppid & "' and U_Z_Status<>'O'"
                        otemprs.DoQuery(strSQL)
                    ElseIf strChoice1 = "R" And strChoice2 = "O" And strChoice3 = "O" And strChoice4 <> "P" Then
                        strSQL = "Update [@Z_HR_OCRAPP] set U_Z_Status='J' where DocEntry='" & strAppid & "' and U_Z_Status<>'O'"
                        otemprs.DoQuery(strSQL)
                        strSQL = "Update [@Z_HR_HEM1] set U_Z_ApplStatus='R' where U_Z_HRAppID='" & strAppid & "' and U_Z_ApplStatus<>'P'"
                        oTemp.DoQuery(strSQL)
                    ElseIf strChoice1 = "S" And strChoice2 = "R" And strChoice3 = "O" And strChoice4 <> "P" Then
                        strSQL = "Update [@Z_HR_OCRAPP] set U_Z_Status='J' where DocEntry='" & strAppid & "' and U_Z_Status<>'O'"
                        otemprs.DoQuery(strSQL)
                        strSQL = "Update [@Z_HR_HEM1] set U_Z_ApplStatus='R' where U_Z_HRAppID='" & strAppid & "' and U_Z_ApplStatus<>'P' "
                        oTemp.DoQuery(strSQL)
                    ElseIf strChoice1 = "S" And strChoice2 = "S" And strChoice3 = "R" And strChoice4 <> "P" Then
                        strSQL = "Update [@Z_HR_OCRAPP] set U_Z_Status='J' where DocEntry='" & strAppid & "' and U_Z_Status<>'O'"
                        otemprs.DoQuery(strSQL)
                        strSQL = "Update [@Z_HR_HEM1] set U_Z_ApplStatus='R' where U_Z_HRAppID='" & strAppid & "' and U_Z_ApplStatus<>'P' "
                        oTemp.DoQuery(strSQL)
                    ElseIf (strChoice1 = "C" Or strChoice2 = "C" Or strChoice3 = "C") And strChoice4 <> "P" Then
                        strSQL = "Update [@Z_HR_OCRAPP] set U_Z_Status='C' where DocEntry='" & strAppid & "' and U_Z_Status<>'O'"
                        otemprs.DoQuery(strSQL)
                        strSQL = "Update [@Z_HR_HEM1] set U_Z_ApplStatus='R' where U_Z_HRAppID='" & strAppid & "' and U_Z_ApplStatus<>'P' "
                        oTemp.DoQuery(strSQL)
                    Else
                        strSQL = "Update [@Z_HR_OCRAPP] set U_Z_Status='S' where DocEntry='" & strAppid & "' and U_Z_Status<>'O'"
                        otemprs.DoQuery(strSQL)
                        strSQL = strSQL & "Update [@Z_HR_HEM1] set U_Z_ApplStatus='O' where U_Z_HRAppID='" & strAppid & "' and U_Z_ApplStatus<>'P' "
                        oTemp.DoQuery(strSQL)
                    End If

                    'strSQL = "Update [@Z_HR_HEM1] set U_Z_1stRounddt='" & Fstdt & "', U_Z_2ndRounddt='" & Snddt & "', U_Z_3rdRounddt='" & Trddt & "', "
                    'strSQL = strSQL & " U_Z_1stRoundRem='" & strremarks1 & "',U_Z_2ndRoundRem='" & strremarks2 & "',U_Z_3rdRoundRem='" & strremarks3 & "', "
                    'strSQL = strSQL & " U_Z_1stRoundStatus='" & strChoice1 & "',U_Z_2ndRoundStatus='" & strChoice2 & "', U_Z_3rdRoundStatus='" & strChoice3 & "' where U_Z_HRAppID='" & strCode & "'"

                    '
                    aForm.Freeze(False)
                End If

            End If
        Next
        Return True
        aForm.Freeze(False)
    End Function
#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_Candidate Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1000001" Then
                                    If oApplication.SBO_Application.MessageBox("Do you want to confirm the information?", , "Yes", "No") = 2 Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "26" And (pVal.ColUID = "DocEntry" Or pVal.ColUID = "U_Z_HRAppID") Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcode As String = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    Dim ooBj As New clshrCrApplicants
                                    ooBj.ViewCandidate(strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If

                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1000001" Then
                                    If AddtoUDT(oForm) = True Then
                                        oApplication.Utilities.Message("Operation Completed Successfully...", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                        oForm.Freeze(False)
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
                Case mnu_InvSO
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
