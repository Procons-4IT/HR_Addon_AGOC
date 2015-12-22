Public Class clsBranchesMaster
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private objMatrix As SAPbouiCOM.Matrix
    Private objForm As SAPbouiCOM.Form
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
    Private MatrixId As String
    Private RowtoDelete As Integer
    Private blnFlag As Boolean = False
    Private oColumn As SAPbouiCOM.Column
    Dim oDataSrc_Line As SAPbouiCOM.DBDataSource
    Dim oDBDataSrc As SAPbouiCOM.DBDataSource
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_Branches) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_BranchMaster, frm_Branches)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)

        oForm.DataSources.UserDataSources.Add("LineID", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oMatrix = oForm.Items.Item("3").Specific
        oColumn = oMatrix.Columns.Item("V_-1")
        oColumn.DataBind.SetBound(True, "", "LineID")
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        BindData(oForm)
        'oDBDataSrc = objForm.DataSources.DBDataSources.Item("OUDP")
        For count As Integer = 1 To oMatrix.RowCount
            oApplication.Utilities.SetMatrixValues(oMatrix, "V_-1", count, count.ToString)
        Next
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        oForm.Freeze(False)
    End Sub
#Region "DataBind"
    Public Sub BindData(ByVal objform As SAPbouiCOM.Form)
        Dim oMatrix As SAPbouiCOM.Matrix

        Try
            oMatrix = objform.Items.Item("3").Specific
            oDBDataSrc = objform.DataSources.DBDataSources.Add("OUBR")
            Try
                oDBDataSrc.Query()
            Catch ex As Exception
            End Try
            oMatrix.LoadFromDataSource()
            If oMatrix.RowCount >= 1 Then
                If oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Specific.Value <> "" Then
                    oDBDataSrc.Clear()
                    oMatrix.AddRow()
                    oMatrix.Columns.Item(0).Cells.Item(oMatrix.RowCount).Specific.Value = oMatrix.RowCount
                    oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                End If
            ElseIf oMatrix.RowCount = 0 Then
                oMatrix.AddRow()
                oMatrix.Columns.Item(0).Cells.Item(oMatrix.RowCount).Specific.Value = oMatrix.RowCount
                oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Public Sub ReBindData(ByVal objform As SAPbouiCOM.Form)
        Dim oMatrix As SAPbouiCOM.Matrix

        Try
            oMatrix = objform.Items.Item("3").Specific
            oDBDataSrc = objform.DataSources.DBDataSources.Add("OUBR")
            Try
                oDBDataSrc.Query()
            Catch ex As Exception
            End Try

            oMatrix.LoadFromDataSource()
            For count As Integer = 1 To oMatrix.RowCount
                oApplication.Utilities.SetMatrixValues(oMatrix, "V_-1", count, count.ToString)
            Next
            oMatrix.FlushToDataSource()

            If oMatrix.RowCount >= 1 Then
                If oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Specific.Value <> "" Then
                    oDBDataSrc.Clear()
                    oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
#Region "Enable Matrix After Update"
    '***************************************************************************
    'Type               : Procedure
    'Name               : EnblMatrixAfterUpdate
    'Parameter          : Application,Company,Form
    'Return Value       : 
    'Author             : DEV-2
    'Created Date       : 
    'Last Modified By   : 
    'Modified Date      : 
    'Purpose            : Enable the Matrix after update button is pressed.
    '***************************************************************************
    Private Sub EnblMatrixAfterUpdate(ByVal objApplication As SAPbouiCOM.Application, ByVal ocompany As SAPbobsCOM.Company, ByVal oForm As SAPbouiCOM.Form)
        Dim oMatrix As SAPbouiCOM.Matrix
        Dim oDBDSource As SAPbouiCOM.DBDataSource
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim lnErrCode As Long
        Dim strErrMsg As String
        Dim i As Integer

        Try
            oMatrix = oForm.Items.Item("3").Specific
            oForm.Freeze(True)
            If 1 = 1 Then
                oDBDSource = oForm.DataSources.DBDataSources.Item("OUBR")
                If oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Specific.value = "" Then
                    oMatrix.DeleteRow(oMatrix.RowCount)
                End If

                oMatrix.FlushToDataSource()
                For i = 0 To oDBDSource.Size - 1
                    If (oDBDSource.GetValue("Code", i) <> "") Then
                        DepartmentFunction(oDBDSource.GetValue("Code", i), oDBDSource.GetValue("Name", i), oDBDSource.GetValue("Remarks", i), "Update", oDBDSource.GetValue("U_Z_FrgnName", i))
                    Else
                        DepartmentFunction(oDBDSource.GetValue("Code", i), oDBDSource.GetValue("Name", i), oDBDSource.GetValue("Remarks", i), "Add", oDBDSource.GetValue("U_Z_FrgnName", i))

                    End If
                Next
                oDBDSource.Query()
                oMatrix.Columns.Item(1).Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            End If
            oForm.Freeze(False)
            
            Exit Sub
        Catch ex As Exception
            ocompany.GetLastError(lnErrCode, strErrMsg)
            If strErrMsg <> "" Then
                objApplication.MessageBox(strErrMsg)
            Else
                objApplication.MessageBox(ex.Message)
            End If
        End Try
    End Sub
#End Region

#Region "Add/Update/Remove Branches"
    Private Sub DepartmentFunction(ByVal aCode As String, ByVal aName As String, ByVal aRemarks As String, ByVal aChoice As String, Optional ByVal Frgn As String = "")
        Dim oDeptSrv As SAPbobsCOM.BranchesService
        oDeptSrv = oApplication.Company.GetCompanyService.GetBusinessService(SAPbobsCOM.ServiceTypes.BranchesService)
        Dim addline As SAPbobsCOM.Branch
        Dim oTest As SAPbobsCOM.Recordset
        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

        Select Case aChoice
            Case "Add"
                addline = oDeptSrv.GetDataInterface(SAPbobsCOM.BranchesServiceDataInterfaces.bsBranch)
                addline.Name = aName.Trim
                addline.Description = aRemarks.Trim
                oDeptSrv.AddBranch(addline)

                oTest.DoQuery("SElect max(""CODE"") from OUBR")
                aCode = oTest.Fields.Item(0).Value
                oTest.DoQuery("Update OUBR set U_Z_FrgnName=N'" & Frgn & "' where Code='" & aCode & "'")

            Case "Delete"
                Dim Getline As SAPbobsCOM.BranchParams
                Getline = oDeptSrv.GetDataInterface(SAPbobsCOM.BranchesServiceDataInterfaces.bsBranchParams)
                Getline.Code = aCode.Trim
                oDeptSrv.DeleteBranch(Getline)
            Case "Update"
                Dim Getline As SAPbobsCOM.BranchParams
                Getline = oDeptSrv.GetDataInterface(SAPbobsCOM.BranchesServiceDataInterfaces.bsBranchParams)
                Getline.Code = aCode.Trim
                addline = oDeptSrv.GetBranch(Getline)
                addline.Name = aName.Trim
                addline.Description = aRemarks.Trim
                oDeptSrv.UpdateBranch(addline)
                oTest.DoQuery("Update OUBR set U_Z_FrgnName=N'" & Frgn & "' where Code='" & aCode & "'")
        End Select

    End Sub

    Private Function DeleteBranch(ByVal aCode As String, ByVal aName As String, ByVal aRemarks As String, ByVal aChoice As String, Optional ByVal Frgn As String = "") As Boolean
        Dim oDeptSrv As SAPbobsCOM.BranchesService
        oDeptSrv = oApplication.Company.GetCompanyService.GetBusinessService(SAPbobsCOM.ServiceTypes.BranchesService)
        Dim addline As SAPbobsCOM.Branch
        Dim oTest As SAPbobsCOM.Recordset
        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

        Select Case aChoice
            Case "Add"
                addline = oDeptSrv.GetDataInterface(SAPbobsCOM.BranchesServiceDataInterfaces.bsBranch)
                addline.Name = aName.Trim
                addline.Description = aRemarks.Trim
                oDeptSrv.AddBranch(addline)

                oTest.DoQuery("SElect max(""CODE"") from OUBR")
                aCode = oTest.Fields.Item(0).Value
                oTest.DoQuery("Update OUBR set U_Z_FrgnName=N'" & Frgn & "' where Code='" & aCode & "'")

            Case "Delete"
                Dim Getline As SAPbobsCOM.BranchParams
                Getline = oDeptSrv.GetDataInterface(SAPbobsCOM.BranchesServiceDataInterfaces.bsBranchParams)
                Getline.Code = aCode.Trim
                Try
                    oDeptSrv.DeleteBranch(Getline)
                Catch ex As Exception
                    oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End Try

            Case "Update"
                Dim Getline As SAPbobsCOM.BranchParams
                Getline = oDeptSrv.GetDataInterface(SAPbobsCOM.BranchesServiceDataInterfaces.bsBranchParams)
                Getline.Code = aCode.Trim
                addline = oDeptSrv.GetBranch(Getline)
                addline.Name = aName.Trim
                addline.Description = aRemarks.Trim
                oDeptSrv.UpdateBranch(addline)
                oTest.DoQuery("Update OUBR set U_Z_FrgnName=N'" & Frgn & "' where Code='" & aCode & "'")
        End Select
        Return True

    End Function
#End Region

#Region "Insert Code and Doc Entry"
    '******************************************************************
    'Type               : Procedure
    'Name               : InsertCodeAndDocEntry
    'Parameter          : 
    'Return Value       : 
    'Author             : DEV-2
    'Created Date       : 
    'Last Modified By   : 
    'Modified Date      : 
    'Purpose            : Inserting code and docEntry values.
    '******************************************************************
    Public Sub InsertCodeAndDocEntry(ByVal aForm As SAPbouiCOM.Form)
        Dim oDBDSource As SAPbouiCOM.DBDataSource
        Dim strValue As String = "1"
        Try
            objForm = aForm
            aForm.Freeze(True)
            oDBDSource = objForm.DataSources.DBDataSources.Item("OUBR")
            objMatrix = objForm.Items.Item("3").Specific
            objMatrix.FlushToDataSource()
            If objMatrix.RowCount = 1 Then
                oDBDSource.SetValue("Code", 0, strValue)
            Else
                oDBDSource.SetValue("Code", objMatrix.RowCount - 1, oDBDSource.GetValue("Code", objMatrix.RowCount - 1))
            End If
            objMatrix.LoadFromDataSource()
            aForm.Freeze(False)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
#End Region


    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        oMatrix = oForm.Items.Item("3").Specific
        Dim strcode, strcode1 As String
        strcode = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount)
        strcode1 = oApplication.Utilities.getMatrixValues(oMatrix, "V_1", oMatrix.RowCount)
        strcode = strcode.Trim()
        strcode1 = strcode1.Trim()
        If strcode.Length > 20 Or strcode1.Length > 40 Then
            oApplication.Utilities.Message("Maximum Characters not allowed", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            Return False
        End If
        If oMatrix.RowCount > 1 Then
            strcode = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount)
            strcode1 = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", oMatrix.RowCount - 1)
            If strcode.ToUpper = strcode1.ToUpper Then
                oApplication.Utilities.Message("This entry already Exist", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If
        End If

        For intRow As Integer = 1 To oMatrix.RowCount
            strcode = oApplication.Utilities.getMatrixValues(oMatrix, "V_0", intRow)
            strcode1 = oApplication.Utilities.getMatrixValues(oMatrix, "V_1", intRow)
            If strcode <> "" And strcode1 = "" Then
                oApplication.Utilities.Message("Description can not be empty", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_1").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If

            If strcode = "" And strcode1 <> "" Then
                oApplication.Utilities.Message("Branch Name  can not be empty", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                Return False
            End If
        Next
        Return True
    End Function


#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_Branches Then
                Select Case pVal.BeforeAction
                    Case True
                        If pVal.EventType = SAPbouiCOM.BoEventTypes.et_KEY_DOWN And pVal.CharPressed <> "9" And pVal.ItemUID = "3" And pVal.ColUID = "V_0" Then
                            Dim strVal As String
                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            objMatrix = oForm.Items.Item("3").Specific
                            strVal = oApplication.Utilities.getMatrixValues(objMatrix, "V_0", pVal.Row)
                            'If oApplication.Utilities.ValidateCode(strVal, "DEPT") = True Then
                            '    oApplication.Utilities.Message("Department Name already Exists. ", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                            '    BubbleEvent = False
                            '    Exit Sub
                            'End If
                        End If
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" And pVal.ColUID = "V_0" And pVal.CharPressed <> 9 Then
                                    objMatrix = oForm.Items.Item("3").Specific
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1" And oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                    oForm.Freeze(True)
                                    If Validation(oForm) = False Then
                                        oForm.Freeze(False)
                                        BubbleEvent = False
                                        Exit Sub
                                    End If


                                    InsertCodeAndDocEntry(oForm)
                                    EnblMatrixAfterUpdate(oApplication.SBO_Application, oApplication.Company, oForm)
                                    ReBindData(oForm)
                                    oForm.Freeze(False)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.GetForm(pVal.FormTypeEx, pVal.FormTypeCount)
                                If pVal.ItemUID = "3" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("3").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "3"
                                    frmSourceMatrix = oMatrix
                                End If

                        End Select
                    Case False
                        If ((pVal.EventType = SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED) And (pVal.ItemUID = "1")) Then
                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            oForm.Freeze(True)
                            objForm = oForm
                            objMatrix = objForm.Items.Item("3").Specific
                            objMatrix.AddRow()
                            objMatrix.Columns.Item(0).Cells.Item(objMatrix.RowCount).Specific.value = objMatrix.RowCount
                            objMatrix.Columns.Item("V_0").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item("V_1").Cells.Item(objMatrix.RowCount).Specific.value = ""
                            objMatrix.Columns.Item(1).Cells.Item(objMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                            objForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
                            oForm.Freeze(False)
                        End If
                End Select
            End If


        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
            oForm.Freeze(False)
        End Try
    End Sub
#End Region
    Private Sub RefereshDeleteRow(ByVal aForm As SAPbouiCOM.Form)
      
        If intSelectedMatrixrow <= 0 Then
            Exit Sub
        End If
        Me.RowtoDelete = intSelectedMatrixrow

        oMatrix = aForm.Items.Item("3").Specific
        If DeleteBranch(oApplication.Utilities.getMatrixValues(oMatrix, "V_2", intSelectedMatrixrow), oApplication.Utilities.getMatrixValues(oMatrix, "V_2", intSelectedMatrixrow), "", "Delete", "") = False Then
            Exit Sub
        Else
            aForm.Close()
            LoadForm()
            aForm = oApplication.SBO_Application.Forms.ActiveForm()
            Exit Sub
        End If

        oMatrix = frmSourceMatrix
        oMatrix.FlushToDataSource()
        For count As Integer = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oMatrix.LoadFromDataSource()
        If oMatrix.RowCount > 0 Then
            oMatrix.DeleteRow(oMatrix.RowCount)
        End If
    End Sub

#Region "Menu Event"
    Public Overrides Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            Select Case pVal.MenuUID
                Case mnu_BranchMaster
                    LoadForm()
                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = True Then

                        If oApplication.SBO_Application.MessageBox("Do you want to delete the details?", , "Yes", "No") = 2 Then
                            BubbleEvent = False
                            Exit Sub
                        End If
                        Dim strCode As String
                        oMatrix = oForm.Items.Item("3").Specific
                        strCode = oApplication.Utilities.getMatrixValues(oMatrix, "V_2", intSelectedMatrixrow)

                        If oApplication.Utilities.ValidateCode(strCode, "Branch") = True Then
                            BubbleEvent = False
                            Exit Sub
                        Else
                            RefereshDeleteRow(oForm)
                            BubbleEvent = False
                            Exit Sub
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
            If BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
End Class
