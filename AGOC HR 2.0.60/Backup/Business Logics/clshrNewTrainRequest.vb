Public Class clshrNewTrainRequest
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox, oCombobox1 As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oOption, oOption1 As SAPbouiCOM.OptionBtn
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
    Public Sub LoadForm(ByVal Empid As String, ByVal EmpName As String, ByVal poscode As String, ByVal posiname As String, ByVal aDeptCode As String, ByVal aDeptName As String)
        oForm = oApplication.Utilities.LoadForm(xml_hr_NewTrainReq, frm_hr_NewTrainReq)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.EnableMenu(mnu_ADD, False)
        oForm.EnableMenu(mnu_FIND, False)
        FillDepartment(oForm)
        oApplication.Utilities.setEdittextvalue(oForm, "8", Empid)
        oApplication.Utilities.setEdittextvalue(oForm, "10", EmpName)
        oApplication.Utilities.setEdittextvalue(oForm, "22", poscode)
        oApplication.Utilities.setEdittextvalue(oForm, "14", posiname)
        oCombobox1 = oForm.Items.Item("24").Specific
        oCombobox1.Select(aDeptCode, SAPbouiCOM.BoSearchKey.psk_ByValue)
        oApplication.Utilities.setEdittextvalue(oForm, "12", aDeptName)
        oOption = oForm.Items.Item("42").Specific
        oOption.GroupWith("43")
        oOption.Selected = True
        oOption1 = oForm.Items.Item("45").Specific
        oOption1.GroupWith("46")
        oOption1.Selected = True
        AddMode(oForm)
        'oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE
        oForm.Freeze(False)
    End Sub
    Public Sub LoadForm1(ByVal RequestCode As String, ByVal strStatus As String)
        oForm = oApplication.Utilities.LoadForm(xml_hr_NewTrainReq, frm_hr_NewTrainReq)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        FillDepartment(oForm)
        oOption = oForm.Items.Item("42").Specific
        oOption.GroupWith("43")
        oOption1 = oForm.Items.Item("45").Specific
        oOption1.GroupWith("46")
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oForm.Items.Item("4").Enabled = True
        oApplication.Utilities.setEdittextvalue(oForm, "4", RequestCode)
        oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE Then
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        End If
        oForm.Items.Item("4").Enabled = False
        If strStatus <> "Pending" Then
            oForm.Items.Item("1").Visible = False
        Else
            oForm.Items.Item("1").Visible = True
        End If
        oForm.Freeze(False)
    End Sub
    Private Sub AddMode(ByVal aform As SAPbouiCOM.Form)
        Dim strCode As String
        strCode = oApplication.Utilities.getMaxCode("@Z_HR_ONTREQ", "DocEntry")
        aform.Items.Item("4").Enabled = True
        aform.Items.Item("6").Enabled = True
        oApplication.Utilities.setEdittextvalue(aform, "4", strCode)
        aform.Items.Item("6").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        oApplication.Utilities.setEdittextvalue(aform, "6", "t")
        oApplication.SBO_Application.SendKeys("{TAB}")
        aform.Items.Item("16").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        aform.Items.Item("4").Enabled = False
        aform.Items.Item("6").Enabled = False
    End Sub
    Private Sub FillDepartment(ByVal sform As SAPbouiCOM.Form)
        oCombobox = oForm.Items.Item("24").Specific
        Dim oSlpRS As SAPbobsCOM.Recordset
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oSlpRS.DoQuery("Select Code,Remarks from OUDP order by Code")
        oCombobox.ValidValues.Add("", "")
        For intRow As Integer = 0 To oSlpRS.RecordCount - 1
            oCombobox.ValidValues.Add(oSlpRS.Fields.Item(0).Value, oSlpRS.Fields.Item(1).Value)
            oSlpRS.MoveNext()
        Next
    End Sub
    Private Sub Department(ByVal poscode As String)
        Dim oSlpRS As SAPbobsCOM.Recordset
        Dim strcode, strqry As String
        oSlpRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strqry = "Select U_Z_DeptCode,U_Z_DeptName from [@Z_HR_OPOSCO] T0 inner join [@Z_HR_OPOSIN] T1 on"
        strqry = strqry & " t0.U_Z_PosCode=t1.U_Z_JobCode where T1.U_Z_PosCode='" & poscode & "'"
        oSlpRS.DoQuery(strqry)
        If oSlpRS.RecordCount > 0 Then
            oCombobox1 = oForm.Items.Item("24").Specific
            strcode = oSlpRS.Fields.Item(0).Value
            oCombobox1.Select(strcode, SAPbouiCOM.BoSearchKey.psk_ByValue)
            oApplication.Utilities.setEdittextvalue(oForm, "12", oSlpRS.Fields.Item(1).Value)
        End If
        'oForm.Items.Item("1000002").DisplayDesc = True
    End Sub
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim strfromdt, Reqno, strTodt, Leaveduty, Travelon, returnon, ResumeOn As String
            Dim fromdt, todt As Date
            Dim Blflag As Boolean
            Reqno = oApplication.Utilities.getEdittextvalue(aForm, "16")
            strfromdt = oApplication.Utilities.getEdittextvalue(aForm, "30")
            strTodt = oApplication.Utilities.getEdittextvalue(aForm, "32")
            fromdt = oApplication.Utilities.GetDateTimeValue(strfromdt)
            todt = oApplication.Utilities.GetDateTimeValue(strTodt)
            Leaveduty = oApplication.Utilities.getEdittextvalue(aForm, "48")
            Travelon = oApplication.Utilities.getEdittextvalue(aForm, "50")
            returnon = oApplication.Utilities.getEdittextvalue(aForm, "52")
            ResumeOn = oApplication.Utilities.getEdittextvalue(aForm, "54")
            If Reqno = "" Then
                oApplication.Utilities.Message("Enter Training Title...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            ElseIf strfromdt = "" Then
                oApplication.Utilities.Message("Enter Training from date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            ElseIf strTodt = "" Then
                oApplication.Utilities.Message("Enter Training to date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            ElseIf Now.Date > fromdt Then
                oApplication.Utilities.Message("Training From date must be greater than or equal to Current date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            ElseIf fromdt > todt Then
                oApplication.Utilities.Message("Training to date must be greater than or equal to Training from date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            ElseIf Leaveduty <> "" Then
                Blflag = DateValidation(aForm, Leaveduty, fromdt, todt)
                If Blflag = False Then
                    oApplication.Utilities.Message("Trainee leaves duty on must be between training from date and training to date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            End If
            If Travelon <> "" Then
                Blflag = DateValidation(aForm, Travelon, fromdt, todt)
                If Blflag = False Then
                    oApplication.Utilities.Message("Trainee Travels on on must be between training from date and training to date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            End If
            If returnon <> "" Then
                Blflag = DateValidation(aForm, returnon, fromdt, todt)
                If Blflag = False Then
                    oApplication.Utilities.Message("Trainee Returns on must be between training from date and training to date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            End If
            If ResumeOn <> "" Then
                Blflag = DateValidation(aForm, ResumeOn, fromdt, todt)
                If Blflag = False Then
                    oApplication.Utilities.Message("Trainee Resumes Duty on must be between training from date and training to date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
            End If
            oCombobox = aForm.Items.Item("57").Specific
            Dim Approval As String = oApplication.Utilities.DocApproval(aForm, HeaderDoctype.Train, oApplication.Utilities.getEdittextvalue(aForm, "8"))
            oCombobox.Select(Approval, SAPbouiCOM.BoSearchKey.psk_ByValue)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
    Private Function DateValidation(ByVal sForm As SAPbouiCOM.Form, ByVal stDate As String, ByVal fromdt As Date, ByVal todate As Date) As Boolean
        Try
            Dim strquery As String
            Dim dtPickDate As Date
            Dim oRec As SAPbobsCOM.Recordset
            dtPickDate = oApplication.Utilities.GetDateTimeValue(stDate)
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strquery = "Select * from OITM where '" & dtPickDate.ToString("yyyy-MM-dd") & "' between '" & fromdt.ToString("yyyy-MM-dd") & "' and '" & todate.ToString("yyyy-MM-dd") & "' "
            oRec.DoQuery(strquery)
            If oRec.RecordCount > 0 Then
                Return True
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
        Return False
    End Function

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_NewTrainReq Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1" And (oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Or oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE) Then
                                    If Validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    ElseIf oApplication.SBO_Application.MessageBox("Do you want confirm the New Training Request", , "Yes", "No") = 2 Then
                                        BubbleEvent = False
                                        Exit Sub
                                    Else
                                        oApplication.Utilities.Message("New Training Request Added Successfully...", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                        oForm.Close()
                                    End If
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "58"
                                        Dim objHistory As New clshrAppHisDetails
                                        objHistory.LoadForm(oForm, HistoryDoctype.NewTra, oApplication.Utilities.getEdittextvalue(oForm, "4"))
                                End Select

                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN


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
                Dim stXML As String = BusinessObjectInfo.ObjectKey
                stXML = stXML.Replace("<?xml version=""1.0"" encoding=""UTF-16"" ?><New Training RequestParams><DocEntry>", "")
                stXML = stXML.Replace("</DocEntry></New Training RequestParams>", "")
                Dim otest As SAPbobsCOM.Recordset
                otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                If stXML <> "" Then

                    otest.DoQuery("select * from [@Z_HR_ONTREQ]  where DocEntry=" & stXML)
                    If otest.RecordCount > 0 Then
                        Dim intTempID As String = oApplication.Utilities.GetTemplateID(oForm, HeaderDoctype.Train, otest.Fields.Item("U_Z_HREmpID").Value)

                        If intTempID <> "0" Then
                            oApplication.Utilities.InitialMessage("New Training Request", otest.Fields.Item("DocEntry").Value, oApplication.Utilities.DocApproval(oForm, HeaderDoctype.Train, otest.Fields.Item("U_Z_HREmpID").Value), intTempID, otest.Fields.Item("U_Z_HREmpName").Value, HistoryDoctype.NewTra)
                        End If
                    End If

                End If

            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
End Class
