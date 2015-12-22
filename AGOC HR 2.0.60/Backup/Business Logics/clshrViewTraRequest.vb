Public Class clshrViewTraRequest
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oFolder, oFolder1 As SAPbouiCOM.Folder
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
    Public Sub LoadForm(ByVal strtitle As String, Optional ByVal empid As String = "")
        'If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_TravelApproval) = False Then
        '    oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        '    Exit Sub
        'End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_ViewTraApp, frm_hr_ViewTraApp)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oFolder = oForm.Items.Item("4").Specific
        oFolder1 = oForm.Items.Item("5").Specific
        If strtitle = "EmpReq" Then
            oForm.Title = "Employee Travel Request"
        End If
        oForm.Freeze(True)
        oForm.PaneLevel = 1
        Databind(empid)
        Databind2(empid)
        reDrawForm(oForm)
        oForm.Freeze(False)
    End Sub
    Private Sub Databind2(ByVal strempid As String)
        Dim strqry As String = ""
        oGrid = oForm.Items.Item("3").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
        If oForm.Title = "Employee Travel Request" Then
            If strempid = "" Then
                Dim oUserID As String = oApplication.Company.UserName
                Dim stremp As String = oApplication.Utilities.getEmpIDforMangers(oUserID)
                strqry = "select DocEntry,U_Z_DocDate,U_Z_EmpId,U_Z_EmpName,U_Z_DeptName,U_Z_PosName,U_Z_TraName,U_Z_TraStLoc,U_Z_TraEdLoc,U_Z_TraStDate,U_Z_TraEndDate ,U_Z_NewReq,"
                strqry = strqry & "U_Z_Status,U_Z_AppStatus from [@Z_HR_OTRAREQ] where U_Z_EmpId in (" & stremp & ") and (U_Z_Status='RA' or U_Z_Status='RR' or U_Z_Status='C' or U_Z_Status='O')"
                oGrid.DataTable.ExecuteQuery(strqry)
                FormatGrid(oGrid)
            Else
                strqry = "select DocEntry,U_Z_DocDate,U_Z_EmpId,U_Z_EmpName,U_Z_DeptName,U_Z_PosName,U_Z_TraName,U_Z_TraStLoc,U_Z_TraEdLoc,U_Z_TraStDate,U_Z_TraEndDate ,U_Z_NewReq,"
                strqry = strqry & "U_Z_Status,U_Z_AppStatus from [@Z_HR_OTRAREQ] where U_Z_EmpId in (" & strempid & ") and (U_Z_Status='RA' or U_Z_Status='RR' or U_Z_Status='C' or U_Z_Status='O')"
                oGrid.DataTable.ExecuteQuery(strqry)
                FormatGrid(oGrid)
            End If
        End If


    End Sub
    Private Sub Databind(ByVal strempid As String)
        Dim strqry As String = ""
        oGrid = oForm.Items.Item("7").Specific
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_1")
        If oForm.Title = "Employee Travel Request" Or oForm.Title = "Travel Request Approval" Then
            If strempid = "" And oForm.Title = "Travel Request Approval" Then
                Dim oUserID As String = oApplication.Company.UserName
                Dim stremp As String = oApplication.Utilities.getEmpIDforMangers(oUserID)
                strqry = "select case U_Z_AppStatus when 'P' then 'Pending' when 'A' then 'Approved' when 'R' then 'Rejected' end as U_Z_AppStatus, case U_Z_Status when 'O' then 'Open' when 'RA' then 'Request Approved' when 'RR' then 'Request Rejected' when 'CR' then 'Claim Received'"
                strqry = strqry & " when 'CA' then 'Claim Approved' when 'CJ' then 'Claim Rejected' else 'Closed' end as U_Z_Status, DocEntry,U_Z_DocDate,U_Z_EmpId,U_Z_EmpName,U_Z_DeptName,U_Z_PosName,U_Z_TraName,U_Z_TraStLoc,U_Z_TraEdLoc,U_Z_TraStDate,U_Z_TraEndDate "
                strqry = strqry & " from [@Z_HR_OTRAREQ] where U_Z_EmpId in (" & stremp & ") "
                oGrid.DataTable.ExecuteQuery(strqry)
                FormatGrid1(oGrid, "Travel")
            Else
                strqry = "select case U_Z_AppStatus when 'P' then 'Pending' when 'A' then 'Approved' when 'R' then 'Rejected' end as U_Z_AppStatus,  case U_Z_Status when 'O' then 'Open' when 'RA' then 'Request Approved' when 'RR' then 'Request Rejected' when 'CR' then 'Claim Received'"
                strqry = strqry & " when 'CA' then 'Claim Approved' when 'CJ' then 'Claim Rejected' else 'Closed' end as U_Z_Status, DocEntry,U_Z_DocDate,U_Z_EmpId,U_Z_EmpName,U_Z_DeptName,U_Z_PosName,U_Z_TraName,U_Z_TraStLoc,U_Z_TraEdLoc,U_Z_TraStDate,U_Z_TraEndDate "
                strqry = strqry & " from [@Z_HR_OTRAREQ] where U_Z_EmpId in (" & strempid & ") "
                oGrid.DataTable.ExecuteQuery(strqry)
                FormatGrid1(oGrid, "Travel")
            End If
        End If

    End Sub
    Private Sub FormatGrid(ByVal aGrid As SAPbouiCOM.Grid)
        aGrid.Columns.Item("DocEntry").TitleObject.Caption = "Request Number"
        aGrid.Columns.Item("DocEntry").Editable = False
        oEditTextColumn = aGrid.Columns.Item("DocEntry")
        oEditTextColumn.LinkedObjectType = "Z_HR_OTRAREQ"
        aGrid.Columns.Item("U_Z_DocDate").TitleObject.Caption = "Request Date"
        aGrid.Columns.Item("U_Z_DocDate").Editable = False
        aGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee Id"
        oEditTextColumn = aGrid.Columns.Item("U_Z_EmpId")
        oEditTextColumn.LinkedObjectType = "171"
        aGrid.Columns.Item("U_Z_EmpId").Editable = False
        aGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
        aGrid.Columns.Item("U_Z_EmpName").Editable = False
        aGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department"
        aGrid.Columns.Item("U_Z_DeptName").Editable = False
        aGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position Name"
        aGrid.Columns.Item("U_Z_PosName").Editable = False
        aGrid.Columns.Item("U_Z_TraName").TitleObject.Caption = "Travel Description"
        aGrid.Columns.Item("U_Z_TraName").Editable = False
        aGrid.Columns.Item("U_Z_TraStLoc").TitleObject.Caption = "Travel Start Location"
        aGrid.Columns.Item("U_Z_TraStLoc").Editable = False
        aGrid.Columns.Item("U_Z_TraEdLoc").TitleObject.Caption = "Travel End Location"
        aGrid.Columns.Item("U_Z_TraEdLoc").Editable = False
        aGrid.Columns.Item("U_Z_TraStDate").TitleObject.Caption = "Travel Start Date"
        aGrid.Columns.Item("U_Z_TraStDate").Editable = False
        aGrid.Columns.Item("U_Z_TraEndDate").TitleObject.Caption = "Travel End Date"
        aGrid.Columns.Item("U_Z_TraEndDate").Editable = False
        aGrid.Columns.Item("U_Z_NewReq").TitleObject.Caption = "New Travel Request"
        aGrid.Columns.Item("U_Z_NewReq").Visible = False
        If oForm.Title = "Travel Request Approval" Then
            aGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Status"
            aGrid.Columns.Item("U_Z_Status").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = aGrid.Columns.Item("U_Z_Status")
            ocombo.ValidValues.Add("O", "Open")
            ocombo.ValidValues.Add("RA", "Request Approved")
            ocombo.ValidValues.Add("RR", "Request Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            aGrid.Columns.Item("U_Z_Status").Editable = True
            oForm.Items.Item("10").Visible = True
            'ElseIf oForm.Title = "Employee Expenses Claim Approval" Then
            '    aGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Status"
            '    aGrid.Columns.Item("U_Z_Status").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            '    ocombo = aGrid.Columns.Item("U_Z_Status")
            '    ocombo.ValidValues.Add("CR", "Claim Received")
            '    ocombo.ValidValues.Add("CA", "Claim Approved")
            '    ocombo.ValidValues.Add("CJ", "Claim Rejected")
            '    ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            '    aGrid.Columns.Item("U_Z_Status").Editable = True
            '    oForm.Items.Item("10").Visible = True
        Else
            aGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Status"
            aGrid.Columns.Item("U_Z_Status").Visible = False
            aGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Status"
            aGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = aGrid.Columns.Item("U_Z_AppStatus")
            ocombo.ValidValues.Add("P", "Pending")
            ocombo.ValidValues.Add("A", "Approved")
            ocombo.ValidValues.Add("R", "Rejected")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            aGrid.Columns.Item("U_Z_AppStatus").Editable = False
            oForm.Items.Item("10").Visible = False
        End If
    End Sub
    Private Sub FormatGrid1(ByVal aGrid As SAPbouiCOM.Grid, ByVal aChoice As String)
        If aChoice = "Travel" Then
            aGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Status"
            aGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Status"
            aGrid.Columns.Item("U_Z_Status").Visible = False
            aGrid.Columns.Item("DocEntry").TitleObject.Caption = "Request Number"
            oEditTextColumn = aGrid.Columns.Item("DocEntry")
            oEditTextColumn.LinkedObjectType = "Z_HR_OTRAREQ"
            aGrid.Columns.Item("U_Z_DocDate").TitleObject.Caption = "Request Date"
            aGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee Id"
            oEditTextColumn = aGrid.Columns.Item("U_Z_EmpId")
            oEditTextColumn.LinkedObjectType = "171"
            aGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
            aGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department"
            aGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position Name"
            aGrid.Columns.Item("U_Z_TraName").TitleObject.Caption = "Travel Description"
            aGrid.Columns.Item("U_Z_TraStLoc").TitleObject.Caption = "Travel Start Location"
            aGrid.Columns.Item("U_Z_TraEdLoc").TitleObject.Caption = "Travel End Location"
            aGrid.Columns.Item("U_Z_TraStDate").TitleObject.Caption = "Travel Start Date"
            aGrid.Columns.Item("U_Z_TraEndDate").TitleObject.Caption = "Travel End Date"
            aGrid.AutoResizeColumns()
            For intRow As Integer = 0 To aGrid.DataTable.Rows.Count - 1
                aGrid.RowHeaders.SetText(intRow, intRow + 1)
            Next
        End If
        aGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oGrid.CollapseLevel = 1
    End Sub

    Private Sub reDrawForm(ByVal oForm As SAPbouiCOM.Form)
        Try
            oForm.Freeze(True)
            oForm.Items.Item("6").Width = oForm.Width - 40
            oForm.Items.Item("6").Height = oForm.Height - 90
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
        End Try
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_ViewTraApp Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                If pVal.ItemUID = "3" And pVal.ColUID = "DocEntry" Then
                                    oGrid = oForm.Items.Item("3").Specific
                                    For intRow As Integer = pVal.Row To pVal.Row
                                        If 1 = 1 Then
                                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                            Dim strcode, strstatus, strNewReq, strempid As String
                                            strcode = oGrid.DataTable.GetValue("DocEntry", intRow)
                                            strstatus = oGrid.DataTable.GetValue("U_Z_AppStatus", intRow)
                                            strNewReq = oGrid.DataTable.GetValue("U_Z_NewReq", intRow)
                                            strempid = oGrid.DataTable.GetValue("U_Z_EmpId", intRow)
                                            Dim objct As New clshrTravelRequest
                                            objct.LoadForm1(oForm, strcode, oForm.Title, strstatus, strempid, strNewReq)
                                            BubbleEvent = False
                                            Exit Sub
                                        End If
                                    Next
                                    BubbleEvent = False
                                    Exit Sub
                                End If

                                If pVal.ItemUID = "7" And pVal.ColUID = "DocEntry" Then
                                    oGrid = oForm.Items.Item("7").Specific
                                    Dim intPartentRow As Integer = oGrid.Rows.GetParent(pVal.Row)

                                    If 1 = 1 Then
                                        'intRow = oGrid.GetDataTableRowIndex(pVal.Row)
                                        oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                        Dim strcode, strstatus, strNewReq, strempid As String
                                        strcode = oGrid.DataTable.GetValue("DocEntry", oGrid.GetDataTableRowIndex(pVal.Row))
                                        strstatus = oGrid.DataTable.GetValue("U_Z_AppStatus", oGrid.GetDataTableRowIndex(pVal.Row))
                                        ' strNewReq = oGrid.DataTable.GetValue("U_Z_NewReq", intRow)
                                        strempid = oGrid.DataTable.GetValue("U_Z_EmpId", oGrid.GetDataTableRowIndex(pVal.Row))
                                        Dim objct As New clshrTravelRequest
                                        objct.LoadForm1(oForm, strcode, oForm.Title, strstatus, strempid)
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                    BubbleEvent = False
                                    Exit Sub

                                End If

                                If (pVal.ItemUID = "3" Or pVal.ItemUID = "7") And pVal.ColUID = "U_Z_TraCode" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    For intRow As Integer = pVal.Row To pVal.Row
                                        If 1 = 1 Then
                                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                            Dim strcode As String
                                            strcode = oGrid.DataTable.GetValue("U_Z_TraCode", intRow)
                                            Dim objct As New clshrTravelAgenda
                                            objct.LoadForm1(strcode)
                                        End If
                                    Next
                                    BubbleEvent = False
                                    Exit Sub
                                End If


                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                reDrawForm(oForm)
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                'oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim strCode, strstatus As String

                                If pVal.ItemUID = "4" Then
                                    oForm.PaneLevel = 1
                                ElseIf pVal.ItemUID = "5" Then
                                    oForm.PaneLevel = 2
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
