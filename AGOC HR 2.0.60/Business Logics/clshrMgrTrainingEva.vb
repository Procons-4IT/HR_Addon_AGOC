Public Class clshrMgrTrainingEva
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oComboColumn, ocombo1 As SAPbouiCOM.ComboBoxColumn
    Private oCheckbox, oCheckbox1, oCheckbox2, oCheckbox3, oCheckbox4, oCheckbox5, oCheckbox6 As SAPbouiCOM.CheckBox
    Private oGrid, oGrid1 As SAPbouiCOM.Grid
    Private oFolder As SAPbouiCOM.Folder
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private ocombo As SAPbouiCOM.ComboBoxColumn
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Public Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_MgrEva) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_MgrEvaluation, frm_hr_MgrEva)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        AddChooseFromList(oForm)
        oForm.DataSources.UserDataSources.Add("Agendano", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oApplication.Utilities.setUserDatabind(oForm, "7", "Agendano")
        oEditText = oForm.Items.Item("7").Specific
        oEditText.ChooseFromListUID = "CFL1"
        oEditText.ChooseFromListAlias = "U_Z_TrainCode"
        oForm.Items.Item("22").TextStyle = 7
        oForm.Items.Item("23").TextStyle = 7
        oForm.Items.Item("1").TextStyle = 7
        oForm.PaneLevel = 1
        oForm.Freeze(False)
    End Sub

    Private Sub Databind(ByVal aform As SAPbouiCOM.Form)
        Try
            oForm.Freeze(True)
            Dim strqry, strstring As String
            oGrid = aform.Items.Item("21").Specific
            oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
            oGrid1 = aform.Items.Item("20").Specific
            oGrid1.DataTable = oForm.DataSources.DataTables.Item("DT_1")
            Dim oUserID As String = oApplication.Company.UserName
            Dim stremp As String = oApplication.Utilities.getEmpIDforMangers(oUserID)
            If stremp = "" Then
                stremp = "9999"
            End If
            strqry = "select [U_Z_HREmpID],[U_Z_HREmpName],[U_Z_TrainCode],[U_Z_CourseName],[U_Z_Startdt],[U_Z_Enddt],[U_Z_InsName],U_Z_AttendeesStatus"
            strqry = strqry & ",Code from [@Z_HR_TRIN1] where U_Z_TrainCode='" & oApplication.Utilities.getEdittextvalue(aform, "7") & "' and U_Z_AttendeesStatus='C' and  U_Z_HREmpID in ( " & stremp & ") "
            oGrid.DataTable.ExecuteQuery(strqry)
            FormatGrid(aform, "Header")

            strstring = "select Code,U_Z_EmpCode,U_Z_EmpName,U_Z_AgendaCode,U_Z_QusCatCode,U_Z_QusCatName,U_Z_QusItemCode,U_Z_QusItemName,"
            strstring += " U_Z_QusRatCode,U_Z_QusRatName,U_Z_Comments,U_Z_MgrStatus  from [@Z_HR_TRAEVA]  where 1=2 " ' U_Z_AgendaCode='" & DocNo1 & "' and U_Z_EmpCode='" & oApplication.Utilities.getEdittextvalue(aform, "25") & "'"
            oGrid1.DataTable.ExecuteQuery(strstring)
            FormatGrid(aform, "Lines")
            oForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
    Private Sub DatabindEvaluation(ByVal aform As SAPbouiCOM.Form)
        Dim strqry, strstring As String
        oGrid = aform.Items.Item("21").Specific
        oGrid1 = aform.Items.Item("20").Specific
        For introw As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.Rows.IsSelected(introw) Then
                strstring = "select Code,U_Z_EmpCode,U_Z_EmpName,U_Z_AgendaCode,U_Z_QusCatCode,U_Z_QusCatName,U_Z_QusItemCode,U_Z_QusItemName,"
                strstring += " U_Z_QusRatCode,U_Z_QusRatName,U_Z_Comments,U_Z_MgrStatus  from [@Z_HR_TRAEVA]  where U_Z_AgendaCode='" & oGrid.DataTable.GetValue("U_Z_TrainCode", introw) & "' and U_Z_EmpCode='" & oGrid.DataTable.GetValue("U_Z_HREmpID", introw) & "'"
                oGrid1.DataTable.ExecuteQuery(strstring)
                FormatGrid(aform, "Lines")
                Exit Sub
            End If
        Next

    End Sub
    Private Sub FormatGrid(ByVal aform As SAPbouiCOM.Form, ByVal aChoice As String)
        aform.Freeze(True)
        If aChoice = "Header" Then
            oGrid.Columns.Item("U_Z_HREmpID").TitleObject.Caption = "Employee Id"
            oGrid.Columns.Item("U_Z_HREmpID").Visible = False
            oEditTextColumn = oGrid.Columns.Item("U_Z_HREmpID")
            oEditTextColumn.LinkedObjectType = 171
            oGrid.Columns.Item("U_Z_HREmpName").TitleObject.Caption = "Employee Name"
            oGrid.Columns.Item("U_Z_HREmpName").Visible = False
            oGrid.Columns.Item("U_Z_TrainCode").TitleObject.Caption = "Agenda Code"
            oGrid.Columns.Item("U_Z_TrainCode").Editable = False
            oEditTextColumn = oGrid.Columns.Item("U_Z_TrainCode")
            oEditTextColumn.LinkedObjectType = "Z_HR_OTRIN"
            oGrid.Columns.Item("U_Z_CourseName").TitleObject.Caption = "Course Name"
            oGrid.Columns.Item("U_Z_CourseName").Editable = False
            oGrid.Columns.Item("U_Z_Startdt").TitleObject.Caption = "Start Date"
            oGrid.Columns.Item("U_Z_Startdt").Editable = False
            oGrid.Columns.Item("U_Z_Enddt").TitleObject.Caption = "End Date"
            oGrid.Columns.Item("U_Z_Enddt").Editable = False
            oGrid.Columns.Item("U_Z_InsName").TitleObject.Caption = "Trainer Code"
            oGrid.Columns.Item("U_Z_InsName").Editable = False
            oGrid.Columns.Item("Code").TitleObject.Caption = "Code"
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("U_Z_AttendeesStatus").TitleObject.Caption = "Training Status"
            oGrid.Columns.Item("U_Z_AttendeesStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo = oGrid.Columns.Item("U_Z_AttendeesStatus")
            ocombo.ValidValues.Add("D", "Dropped")
            ocombo.ValidValues.Add("C", "Completed")
            ocombo.ValidValues.Add("F", "Failed")
            ocombo.ValidValues.Add("L", "Cancel")
            ocombo.ValidValues.Add("W", "WithDraw")
            ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid.AutoResizeColumns()
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
            oApplication.Utilities.AssignRowNo(oGrid, aform)
        ElseIf aChoice = "Lines" Then
            oGrid1.Columns.Item("Code").TitleObject.Caption = "Code"
            oGrid1.Columns.Item("Code").Visible = False
            oGrid1.Columns.Item("U_Z_EmpCode").TitleObject.Caption = "Employee Id"
            oGrid1.Columns.Item("U_Z_EmpCode").Visible = False
            oGrid1.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
            oGrid1.Columns.Item("U_Z_EmpName").Visible = False
            oGrid1.Columns.Item("U_Z_AgendaCode").TitleObject.Caption = "Agenda Code"
            oGrid1.Columns.Item("U_Z_AgendaCode").Visible = False
            oGrid1.Columns.Item("U_Z_QusCatCode").TitleObject.Caption = "Category Code"
            oGrid1.Columns.Item("U_Z_QusCatCode").Visible = False
            oGrid1.Columns.Item("U_Z_QusCatName").TitleObject.Caption = "Category Name"
            oGrid1.Columns.Item("U_Z_QusCatName").Editable = False
            oGrid1.Columns.Item("U_Z_QusItemCode").TitleObject.Caption = "Items Code"
            oGrid1.Columns.Item("U_Z_QusItemCode").Visible = False
            oGrid1.Columns.Item("U_Z_QusItemName").TitleObject.Caption = "Items Name"
            oGrid1.Columns.Item("U_Z_QusItemName").Editable = False
            oGrid1.Columns.Item("U_Z_QusRatCode").TitleObject.Caption = "Rating Code"
            oGrid1.Columns.Item("U_Z_QusRatCode").Visible = False
            oGrid1.Columns.Item("U_Z_QusRatName").TitleObject.Caption = "Rating Name"
            oGrid1.Columns.Item("U_Z_QusRatName").Editable = False
            oGrid1.Columns.Item("U_Z_Comments").TitleObject.Caption = "Comments"
            oGrid1.Columns.Item("U_Z_MgrStatus").TitleObject.Caption = "Manager Status"
            oGrid1.Columns.Item("U_Z_MgrStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            ocombo1 = oGrid1.Columns.Item("U_Z_MgrStatus")
            ocombo1.ValidValues.Add("P", "Pending")
            ocombo1.ValidValues.Add("A", "Approved")
            ocombo1.ValidValues.Add("R", "Rejected")
            ocombo1.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
            oGrid1.Columns.Item("U_Z_MgrStatus").Visible = True
            oGrid1.AutoResizeColumns()
            oGrid1.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
            oApplication.Utilities.AssignRowNo(oGrid1, aform)
        End If
        aform.Freeze(False)
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
            oCFLCreationParams.ObjectType = "Z_HR_OTRIN"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)


        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

 
    Private Function AddToUDT(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim strTable, strEmpId, strCode, strType, strAgendaCode, strqry, strEmpName, strStatus As String
        Dim strcount As Integer
        Dim dblValue As Double
        Dim dtFromDate, dtTodate, dt, AppEnddt As Date
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim oValidateRS, otemp2 As SAPbobsCOM.Recordset
        Dim otemp, otemp1, otemprs As SAPbobsCOM.Recordset
        oUserTable = oApplication.Company.UserTables.Item("Z_HR_TRAEVA")
        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        dt = Now.Date
        oGrid = aForm.Items.Item("20").Specific
        oGrid1 = aForm.Items.Item("21").Specific
        strTable = "@Z_HR_TRAEVA"
        strEmpId = oApplication.Utilities.getEdittextvalue(aForm, "25")
        strEmpName = oApplication.Utilities.getEdittextvalue(aForm, "27")
        For intRow As Integer = 0 To oGrid1.DataTable.Rows.Count - 1
            If oGrid1.Rows.IsSelected(intRow) Then
                strAgendaCode = oGrid1.DataTable.GetValue("U_Z_TrainCode", intRow)
                Exit For
            End If
        Next
        If strAgendaCode <> "" Then
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                If oUserTable.GetByKey(oGrid.DataTable.GetValue("Code", intRow)) Then
                    oUserTable.Code = oGrid.DataTable.GetValue("Code", intRow)
                    oUserTable.Name = oGrid.DataTable.GetValue("Code", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Comments").Value = oGrid.DataTable.GetValue("U_Z_Comments", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_MgrStatus").Value = oGrid.DataTable.GetValue("U_Z_MgrStatus", intRow)
                    If oUserTable.Update() <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
            Next
        End If
        Return True
    End Function

    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            oGrid = aForm.Items.Item("20").Specific
            If oGrid.Rows.Count > 0 Then
                For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                    If oGrid.DataTable.GetValue("U_Z_QusCatCode", intRow) <> "" Then
                        If oGrid.DataTable.GetValue("U_Z_QusItemCode", intRow) = "" Then
                            oApplication.Utilities.Message("questionnaire Items Code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Return False
                        ElseIf oGrid.DataTable.GetValue("U_Z_QusRatCode", intRow) = "" Then
                            oApplication.Utilities.Message("questionnaire Rate Code is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Return False
                        End If
                    End If
                Next
            End If
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
    Private Sub reDrawForm(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)


            Dim intTop As Int16
            aForm.Items.Item("21").Top = aForm.Items.Item("22").Top + aForm.Items.Item("22").Height + 3
            aForm.Items.Item("21").Height = (aForm.Height / 2) - 60
            aForm.Items.Item("21").Width = (aForm.Width) - 20
            intTop = aForm.Items.Item("21").Top + aForm.Items.Item("21").Height
            aForm.Items.Item("23").Top = intTop + 5

            aForm.Items.Item("22").TextStyle = 7
            aForm.Items.Item("23").TextStyle = 7

            intTop = aForm.Items.Item("23").Top + 15
            aForm.Items.Item("20").Top = intTop
            '  aForm.Items.Item("45").Top = intTop

            aForm.Items.Item("20").Height = aForm.Items.Item("21").Height + 10
            aForm.Items.Item("20").Width = (aForm.Width) - 20
            oGrid = aForm.Items.Item("21").Specific
            oGrid.AutoResizeColumns()
            oGrid = aForm.Items.Item("20").Specific
            oGrid.AutoResizeColumns()
          
            aForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
        End Try
    End Sub


#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_MgrEva Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim strCode As String
                                If pVal.ItemUID = "5" And (oForm.PaneLevel = 2 Or oForm.PaneLevel = 3) Then
                                    If Validation(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                                Select Case pVal.ItemUID
                                    Case "28"
                                        strCode = oApplication.Utilities.getEdittextvalue(oForm, "7")
                                         oApplication.Utilities.OpenMasterinLink(oForm, "AgendaCode", strCode)
                                        BubbleEvent = False
                                        Exit Sub
                                    Case "29"
                                        strCode = oApplication.Utilities.getEdittextvalue(oForm, "9")
                                        oApplication.Utilities.OpenMasterinLink(oForm, "Course", strCode)
                                        BubbleEvent = False
                                        Exit Sub
                                    Case "30"
                                        strCode = oApplication.Utilities.getEdittextvalue(oForm, "17")
                                        Dim ooBj As New clshrTrainner
                                        ooBj.ViewCandidate(strCode)
                                        BubbleEvent = False
                                        Exit Sub
                                End Select
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "21" And pVal.ColUID = "U_Z_TrainCode" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcode As String = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    oApplication.Utilities.OpenMasterinLink(oForm, "AgendaCode", strcode)
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
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)


                                If pVal.ItemUID = "3" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = oForm.PaneLevel - 1
                                    oForm.Freeze(False)
                                End If
                                If pVal.ItemUID = "4" Then
                                    oForm.Freeze(True)
                                    oForm.PaneLevel = oForm.PaneLevel + 1
                                    If oForm.PaneLevel = 3 Then
                                        Databind(oForm)
                                    End If
                                    oForm.Freeze(False)
                                End If

                                If pVal.ItemUID = "21" And pVal.ColUID = "RowsHeader" And pVal.Row <> -1 Then
                                    oForm.Freeze(True)
                                    DatabindEvaluation(oForm)
                                    oForm.Freeze(False)
                                End If

                                If pVal.ItemUID = "5" Then
                                    If oApplication.SBO_Application.MessageBox("Do you want to save the Training Evaluation ? ", , "Yes", "No") = 2 Then
                                        Exit Sub
                                    End If
                                    If AddToUDT(oForm) = True Then
                                        oApplication.Utilities.Message("Operation Completed Successfully...", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
                                    Else
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1 As String
                                Dim sCHFL_ID, val, val2, val3 As String
                                Dim dtdate1, dtdate2 As String
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

                                        If pVal.ItemUID = "7" Then
                                            val1 = oDataTable.GetValue("U_Z_TrainCode", 0)
                                            val = oDataTable.GetValue("U_Z_CourseCode", 0)
                                            val2 = oDataTable.GetValue("U_Z_CourseName", 0)
                                            val3 = oDataTable.GetValue("U_Z_InsName", 0)
                                            Try
                                                dtdate2 = oDataTable.GetValue("U_Z_Enddt", 0)
                                                oApplication.Utilities.setEdittextvalue(oForm, "15", dtdate2)
                                            Catch ex As Exception
                                                oApplication.Utilities.setEdittextvalue(oForm, "15", "")
                                            End Try
                                            Try
                                                dtdate1 = oDataTable.GetValue("U_Z_Startdt", 0)
                                                oApplication.Utilities.setEdittextvalue(oForm, "13", dtdate1)
                                            Catch ex As Exception
                                                oApplication.Utilities.setEdittextvalue(oForm, "13", "")
                                            End Try

                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "17", val3)
                                                oApplication.Utilities.setEdittextvalue(oForm, "11", val2)
                                                oApplication.Utilities.setEdittextvalue(oForm, "9", val)
                                                oApplication.Utilities.setEdittextvalue(oForm, "7", val1)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        oForm.Freeze(False)
                                    End If
                                Catch ex As Exception
                                    ' oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
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
                Case mnu_hr_MgrEvaluation
                    LoadForm()
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
