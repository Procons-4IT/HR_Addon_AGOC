Public Class clshrCourseReview
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private ocombo As SAPbouiCOM.ComboBoxColumn
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
    Public Sub LoadForm(ByVal CourseCode As String, ByVal CourseName As String, Optional ByVal achoice As String = "")
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_CourseRev) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_CourseRev, frm_hr_CourseRev)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oApplication.Utilities.setEdittextvalue(oForm, "4", CourseCode)
        oApplication.Utilities.setEdittextvalue(oForm, "6", CourseName)
        Databind(CourseCode, achoice)
        Databind1(CourseCode)
        Databind2(CourseCode)
        oForm.PaneLevel = 1
        oForm.Freeze(False)
    End Sub
    Public Sub LoadForm1(ByVal CourseCode As String, ByVal achoice As String)
        oForm = oApplication.Utilities.LoadForm(xml_hr_CourseRev, frm_hr_CourseRev)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oApplication.Utilities.setEdittextvalue(oForm, "4", CourseCode)
        Databind(CourseCode, achoice)
        Databind1(CourseCode)
        Databind2(CourseCode)
        oForm.PaneLevel = 1
        oForm.Freeze(False)
    End Sub
    Private Sub Databind(ByVal couCode As String, ByVal ochoice As String)
        oForm.Freeze(True)
        Dim strqry As String
        oGrid = oForm.Items.Item("7").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
        strqry = "select U_Z_HREmpID,U_Z_HREmpName,U_Z_PosiName,U_Z_DeptName,U_Z_Venue,U_Z_Fromdt,U_Z_Todt,U_Z_Instruct,U_Z_ApplyDate,"
        strqry = strqry & " U_Z_ApplStatus ,U_Z_Remarks,Code from [@Z_HR_TRIN1] where U_Z_CourseCode='" & couCode & "' "
        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("U_Z_HREmpID").TitleObject.Caption = "Employee Id"
        oGrid.Columns.Item("U_Z_HREmpID").Editable = False
        oEditTextColumn = oGrid.Columns.Item("U_Z_HREmpID")
        oEditTextColumn.LinkedObjectType = 171
        oGrid.Columns.Item("U_Z_HREmpName").TitleObject.Caption = "Employee Name"
        oGrid.Columns.Item("U_Z_HREmpName").Editable = False
        oGrid.Columns.Item("U_Z_PosiName").TitleObject.Caption = "Position Name"
        oGrid.Columns.Item("U_Z_PosiName").Editable = False
        oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
        oGrid.Columns.Item("U_Z_DeptName").Editable = False
        oGrid.Columns.Item("U_Z_Venue").TitleObject.Caption = "Venue"
        oGrid.Columns.Item("U_Z_Venue").Editable = False
        oGrid.Columns.Item("U_Z_Fromdt").TitleObject.Caption = "Course Start Date"
        oGrid.Columns.Item("U_Z_Fromdt").Editable = False
        oGrid.Columns.Item("U_Z_Todt").TitleObject.Caption = "Course End Date"
        oGrid.Columns.Item("U_Z_Todt").Editable = False
        oGrid.Columns.Item("U_Z_Instruct").TitleObject.Caption = "Instruction"
        oGrid.Columns.Item("U_Z_Instruct").Editable = False
        oGrid.Columns.Item("U_Z_ApplyDate").TitleObject.Caption = "Apply Date"
        oGrid.Columns.Item("U_Z_ApplyDate").Editable = False
        If ochoice = "" Then
            oGrid.Columns.Item("U_Z_ApplStatus").TitleObject.Caption = "Status"
            oGrid.Columns.Item("U_Z_ApplStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_ApplStatus")
            ocombo.ValidValues.Add("P", "Pending")
            ocombo.ValidValues.Add("A", "Accepted")
            ocombo.ValidValues.Add("R", "Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"
        Else
            oGrid.Columns.Item("U_Z_ApplStatus").TitleObject.Caption = "Status"
            oGrid.Columns.Item("U_Z_ApplStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_ApplStatus")
            ocombo.ValidValues.Add("P", "Pending")
            ocombo.ValidValues.Add("A", "Accepted")
            ocombo.ValidValues.Add("R", "Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.Columns.Item("U_Z_ApplStatus").Editable = False
            oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"
            oGrid.Columns.Item("U_Z_Remarks").Editable = False
        End If
        oGrid.Columns.Item("Code").TitleObject.Caption = "Code"
        oGrid.Columns.Item("Code").Visible = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oForm.Freeze(False)
    End Sub
    Private Sub Databind1(ByVal couCode As String)
        oForm.Freeze(True)
        Dim strqry As String
        oGrid = oForm.Items.Item("24").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_1")
        strqry = "select DocEntry,U_Z_Venue, U_Z_Fromdt,U_Z_Todt,U_Z_AppStdt,U_Z_AppEnddt,U_Z_Instruct,U_Z_MaxAttendees,"
        strqry = strqry & "case U_Z_Status when 'O' then 'Open' when 'I' then 'InProcess' when 'L' then 'Closed' when 'C' then 'Canceled' when 'R' then 'ApplicationReceived' else 'ApplicationClosed' end as U_Z_Status,"
        strqry = strqry & " U_Z_MinAttendees, U_Z_ReqAtten,U_Z_AccAtten,U_Z_RejAtten,U_Z_TrainCode,U_Z_CourseName  from [@Z_HR_OTRIN] where U_Z_CourseCode='" & couCode & "' "
        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("DocEntry").TitleObject.Caption = "DocEntry"
        oGrid.Columns.Item("DocEntry").Editable = False
        oGrid.Columns.Item("DocEntry").Visible = False
        oGrid.Columns.Item("U_Z_Venue").TitleObject.Caption = "Course Venue"
        oGrid.Columns.Item("U_Z_Venue").Editable = False
        oGrid.Columns.Item("U_Z_Fromdt").TitleObject.Caption = "Course Start Date"
        oGrid.Columns.Item("U_Z_Fromdt").Editable = False
        oGrid.Columns.Item("U_Z_Todt").TitleObject.Caption = "Course End Date"
        oGrid.Columns.Item("U_Z_Todt").Editable = False
        oGrid.Columns.Item("U_Z_AppStdt").TitleObject.Caption = "Application Start Date"
        oGrid.Columns.Item("U_Z_AppStdt").Editable = False
        oGrid.Columns.Item("U_Z_AppEnddt").TitleObject.Caption = "Application End Date"
        oGrid.Columns.Item("U_Z_AppEnddt").Editable = False
        oGrid.Columns.Item("U_Z_Instruct").TitleObject.Caption = "Instruction"
        oGrid.Columns.Item("U_Z_Instruct").Editable = False
        oGrid.Columns.Item("U_Z_Status").Visible = False
        oGrid.Columns.Item("U_Z_MinAttendees").Visible = False
        oGrid.Columns.Item("U_Z_MaxAttendees").Visible = False
        oGrid.Columns.Item("U_Z_ReqAtten").Visible = False
        oGrid.Columns.Item("U_Z_AccAtten").Visible = False
        oGrid.Columns.Item("U_Z_RejAtten").Visible = False
        oGrid.Columns.Item("U_Z_TrainCode").Visible = False
        oGrid.Columns.Item("U_Z_CourseName").Visible = False
        oApplication.Utilities.setEdittextvalue(oForm, "19", oGrid.DataTable.GetValue("U_Z_Status", 0))
        oApplication.Utilities.setEdittextvalue(oForm, "9", oGrid.DataTable.GetValue("U_Z_MinAttendees", 0))
        oApplication.Utilities.setEdittextvalue(oForm, "27", oGrid.DataTable.GetValue("U_Z_MaxAttendees", 0))
        oApplication.Utilities.setEdittextvalue(oForm, "11", oGrid.DataTable.GetValue("U_Z_ReqAtten", 0))
        oApplication.Utilities.setEdittextvalue(oForm, "13", oGrid.DataTable.GetValue("U_Z_AccAtten", 0))
        oApplication.Utilities.setEdittextvalue(oForm, "15", oGrid.DataTable.GetValue("U_Z_RejAtten", 0))
        oApplication.Utilities.setEdittextvalue(oForm, "17", oGrid.DataTable.GetValue("U_Z_TrainCode", 0))
        oApplication.Utilities.setEdittextvalue(oForm, "6", oGrid.DataTable.GetValue("U_Z_CourseName", 0))
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oForm.Freeze(False)
    End Sub
    Private Sub Databind2(ByVal couCode As String)
        oForm.Freeze(True)
        Dim strqry As String
        oGrid = oForm.Items.Item("25").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_2")
        strqry = "select U_Z_ApplStatus, U_Z_HREmpID,U_Z_HREmpName,U_Z_PosiName,U_Z_DeptName,U_Z_Venue,U_Z_Fromdt,U_Z_Todt,U_Z_Instruct,U_Z_ApplyDate,"
        strqry = strqry & " U_Z_Remarks,Code from [@Z_HR_TRIN1] where U_Z_CourseCode='" & couCode & "' "
        oGrid.DataTable.ExecuteQuery(strqry)
        oGrid.Columns.Item("U_Z_ApplStatus").TitleObject.Caption = "Status"
        oGrid.Columns.Item("U_Z_ApplStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        ocombo = oGrid.Columns.Item("U_Z_ApplStatus")
        ocombo.ValidValues.Add("P", "Pending")
        ocombo.ValidValues.Add("A", "Accepted")
        ocombo.ValidValues.Add("R", "Rejected")
        ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oGrid.Columns.Item("U_Z_HREmpID").TitleObject.Caption = "Employee Id"
        oGrid.Columns.Item("U_Z_HREmpID").Editable = False
        oEditTextColumn = oGrid.Columns.Item("U_Z_HREmpID")
        oEditTextColumn.LinkedObjectType = 171
        oGrid.Columns.Item("U_Z_HREmpName").TitleObject.Caption = "Employee Name"
        oGrid.Columns.Item("U_Z_HREmpName").Editable = False
        oGrid.Columns.Item("U_Z_PosiName").TitleObject.Caption = "Position Name"
        oGrid.Columns.Item("U_Z_PosiName").Editable = False
        oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
        oGrid.Columns.Item("U_Z_DeptName").Editable = False
        oGrid.Columns.Item("U_Z_Venue").TitleObject.Caption = "Venue"
        oGrid.Columns.Item("U_Z_Venue").Editable = False
        oGrid.Columns.Item("U_Z_Fromdt").TitleObject.Caption = "Course Start Date"
        oGrid.Columns.Item("U_Z_Fromdt").Editable = False
        oGrid.Columns.Item("U_Z_Todt").TitleObject.Caption = "Course End Date"
        oGrid.Columns.Item("U_Z_Todt").Editable = False
        oGrid.Columns.Item("U_Z_Instruct").TitleObject.Caption = "Instruction"
        oGrid.Columns.Item("U_Z_Instruct").Editable = False
        oGrid.Columns.Item("U_Z_ApplyDate").TitleObject.Caption = "Apply Date"
        oGrid.Columns.Item("U_Z_ApplyDate").Editable = False
        oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"
        oGrid.Columns.Item("U_Z_Remarks").Editable = False
        oGrid.Columns.Item("Code").TitleObject.Caption = "Code"
        oGrid.Columns.Item("Code").Visible = False
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oGrid.CollapseLevel = 1
        oForm.Freeze(False)
    End Sub


#Region "Add to UDT"
    Private Function AddtoUDT(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim strCode, strqry As String
        Dim strcount, strcount1, strMaxAtte As Integer
        Dim strCoustdt, stApplydt As Date
        Dim otemprs, otemp1, otemp2, otemp3 As SAPbobsCOM.Recordset
        aForm.Freeze(True)
        otemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp3 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oGrid = aForm.Items.Item("7").Specific
        Dim strChoice, strremarks As String
        If oApplication.Company.InTransaction() Then
            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
        End If
        oApplication.Company.StartTransaction()
        Try
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                strCoustdt = oApplication.Utilities.GetDateTimeValue(oGrid.DataTable.GetValue("U_Z_Fromdt", intRow))
                stApplydt = oApplication.Utilities.GetDateTimeValue(oGrid.DataTable.GetValue("U_Z_ApplyDate", intRow))
                If strCoustdt < stApplydt Then
                    oApplication.Utilities.Message("Apply date must be Less than or equal to Course Start date...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    If oApplication.Company.InTransaction() Then
                        oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                    End If
                    Return False
                End If

                ocombo = oGrid.Columns.Item("U_Z_ApplStatus")
                strCode = oGrid.DataTable.GetValue("Code", intRow)
                If strCode <> "" Then
                    strChoice = ocombo.GetSelectedValue(intRow).Value
                    strremarks = oGrid.DataTable.GetValue("U_Z_Remarks", intRow)
                    strSQL = "Update [@Z_HR_TRIN1] set U_Z_Remarks='" & strremarks & "', U_Z_ApplStatus='" & strChoice & "' where Code='" & strCode & "'"
                    otemprs.DoQuery(strSQL)
                End If
                strqry = "Select count(*) as Total from [@Z_HR_TRIN1] where U_Z_ApplStatus='A' and U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(oForm, "17") & "' group by U_Z_TrainCode"
                otemp1.DoQuery(strqry)
                If 1 = 1 Then
                    strcount = otemp1.Fields.Item("Total").Value
                    strMaxAtte = oApplication.Utilities.getEdittextvalue(aForm, "27")
                    If strcount > strMaxAtte Then
                        oApplication.Utilities.Message("Accepted Attendees must be Less than or equal to No of Maximum Attendees...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        If oApplication.Company.InTransaction() Then
                            oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                        End If
                        Return False
                    End If
                End If
            Next
       
            strqry = "Select count(*) as Total from [@Z_HR_TRIN1] where U_Z_ApplStatus='A' and U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(oForm, "17") & "' group by U_Z_TrainCode"
            otemp1.DoQuery(strqry)
            If 1 = 1 Then
                strcount = otemp1.Fields.Item("Total").Value
                strqry = "Update [@Z_HR_OTRIN] set U_Z_AccAtten='" & strcount & "' where U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(oForm, "17") & "' "
                otemprs.DoQuery(strqry)
            End If
            strqry = "Select count(*) as Total from [@Z_HR_TRIN1] where U_Z_ApplStatus='R' and U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(oForm, "17") & "' group by U_Z_TrainCode"
            otemp2.DoQuery(strqry)
            If 1 = 1 Then
                strcount = otemp2.Fields.Item("Total").Value
                strqry = "Update [@Z_HR_OTRIN] set U_Z_RejAtten='" & strcount & "' where U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(oForm, "17") & "' "
                otemprs.DoQuery(strqry)
            End If
            strqry = "Select count(*) as Total from [@Z_HR_TRIN1] where U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(oForm, "17") & "' group by U_Z_TrainCode"
            otemp3.DoQuery(strqry)
            If 1 = 1 Then
                strcount = otemp3.Fields.Item("Total").Value
                strqry = "Update [@Z_HR_OTRIN] set U_Z_ReqAtten='" & strcount & "' where U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(oForm, "17") & "' "
                otemprs.DoQuery(strqry)
            End If
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If
            aForm.Freeze(False)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            End If
            Return False
        End Try
        Return True
    End Function
#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_CourseRev Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD

                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "8" Then
                                    If AddtoUDT(oForm) = True Then
                                        oApplication.Utilities.Message("Operation Completed Successfully...", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                        'oForm.Close()
                                        Databind1(oApplication.Utilities.getEdittextvalue(oForm, "4"))
                                        Databind2(oApplication.Utilities.getEdittextvalue(oForm, "4"))
                                    End If
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                Select Case pVal.ItemUID
                                    Case "20"
                                        oForm.PaneLevel = 1
                                    Case "21"
                                        oForm.PaneLevel = 2
                                    Case "22"
                                        oForm.PaneLevel = 3
                                End Select

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
                Case mnu_hr_CourseRev
                    Dim oTe As New clshrLogin
                    oTe.LoadForm("COREV")
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
