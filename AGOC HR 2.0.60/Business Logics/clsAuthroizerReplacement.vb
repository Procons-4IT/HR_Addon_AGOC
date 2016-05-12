Public Class clsAuthroizerReplacement
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
    Private RowtoDelete As Integer
    Private InvBaseDocNo As String
    Private MatrixId As String
    Private oColumn As SAPbouiCOM.Column
    Private InvForConsumedItems, count As Integer
    Private blnFlag As Boolean = False
    Dim oDataSrc_Line As SAPbouiCOM.DBDataSource
    Dim oDataSrc_Line1 As SAPbouiCOM.DBDataSource
    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_ReplaceAuthorizer) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_ReplaceAuthorizer, frm_ReplaceAuthorizer)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        oForm.DataBrowser.BrowseBy = "14"
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.EnableMenu("1283", True)
        ' AddChooseFromList(oForm)
        databind(oForm)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RAUT1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oForm.Items.Item("6").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        reDrawForm(oForm)
        oForm.Freeze(False)
    End Sub
    Public Sub LoadForm1(ByVal salcode As String)
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_SalStru) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_SalStru, frm_hr_SalStru)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        ' oForm.DataBrowser.BrowseBy = "5"
        oForm.EnableMenu(mnu_ADD_ROW, True)
        oForm.EnableMenu(mnu_DELETE_ROW, True)
        oForm.EnableMenu("1283", True)
        AddChooseFromList(oForm)
        databind(oForm)
        oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RAUT1")
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oForm.PaneLevel = 3
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE
        oForm.Items.Item("5").Enabled = True
        oApplication.Utilities.setEdittextvalue(oForm, "5", salcode)
        oForm.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_OK_MODE Then
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        End If
        reDrawForm(oForm)
        oForm.Freeze(False)
    End Sub

#Region "Add Choose From List"
    Private Sub databind(ByVal aForm As SAPbouiCOM.Form)
        oMatrix = aForm.Items.Item("16").Specific
        Dim otest As SAPbobsCOM.Recordset
        otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oColumn = oMatrix.Columns.Item("V_0")
        For intRow As Integer = oColumn.ValidValues.Count - 1 To 0 Step -1
            Try
                oColumn.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
            Catch ex As Exception

            End Try

        Next
        oColumn.ValidValues.Add("", "")
        oColumn.ValidValues.Add("Train", "Training Request")
        oColumn.ValidValues.Add("EmpLife", "Employee Life Cycle")
        oColumn.ValidValues.Add("ExpCli", "Expense Claim")
        oColumn.ValidValues.Add("TraReq", "Travel Request")
        oColumn.ValidValues.Add("LveReq", "Leave Request")
        oColumn.ValidValues.Add("Rec", "Recruitment Request")
        oColumn.ValidValues.Add("LoanReq", "Loan Request")
        oColumn.ValidValues.Add("Loanee", "Loanee Expenses")
        oColumn.DisplayDesc = True
        oColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
        oEditText = aForm.Items.Item("6").Specific
        oEditText.ChooseFromListUID = "CFL_2"
        oEditText.ChooseFromListAlias = "USER_CODE"
        oEditText = aForm.Items.Item("9").Specific
        oEditText.ChooseFromListUID = "CFL_3"
        oEditText.ChooseFromListAlias = "USER_CODE"
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
            oCFLCreationParams.ObjectType = "Z_HR_OALLO"
            oCFLCreationParams.UniqueID = "CFL1"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "U_Z_Status"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.ObjectType = "Z_HR_OBEFI"
            oCFLCreationParams.UniqueID = "CFL2"
            oCFL = oCFLs.Add(oCFLCreationParams)
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "U_Z_Status"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()


            oCFLCreationParams.ObjectType = "Z_HR_OLVL"
            oCFLCreationParams.UniqueID = "CFL3"
            oCFL = oCFLs.Add(oCFLCreationParams)

            '' Adding Conditions to CFL2
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "U_Z_Status"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            oCFLCreationParams.ObjectType = "Z_HR_OGRD"
            oCFLCreationParams.UniqueID = "CFL4"
            oCFL = oCFLs.Add(oCFLCreationParams)

            '' Adding Conditions to CFL2
            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "U_Z_Status"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub ReplaceAuthorizer(aDocEnry As String)
        Dim oTemp, oTemp1, oTemp2 As SAPbobsCOM.Recordset
        Dim strReqType, strCurrentyApprover, strReplaceApprover, strQuery, strQuery1 As String
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim strTempID As String
        oTemp.DoQuery("Select * from ""@Z_HR_RAUT"" T0 Inner Join ""@Z_HR_RAUT1"" T1 on T1.""DocEntry""=T0.""DocEntry"" where T1.""U_Z_Active""='Y' and T0.""DocEntry""=" & aDocEnry)
        For intRow As Integer = 0 To oTemp.RecordCount - 1
            strCurrentyApprover = oTemp.Fields.Item("U_Z_CurAut").Value
            strReplaceApprover = oTemp.Fields.Item("U_Z_RepAut").Value
            strReqType = oTemp.Fields.Item("U_Z_TransType").Value
            strTempID = oTemp.Fields.Item("U_Z_TempID").Value
            If strTempID = "" Then
                strQuery = "select T0.DocEntry,LineID from [@Z_HR_OAPPT] T0 Inner Join [@Z_HR_APPT2] T1 on T1.DocEntry=T0.DocEntry where U_Z_AMan='Y' and  U_Z_DocType='" & strReqType & "' and T1.U_Z_aUser='" & strCurrentyApprover & "'"
            Else
                strQuery = "select T0.DocEntry,LineID from [@Z_HR_OAPPT] T0 Inner Join [@Z_HR_APPT2] T1 on T1.DocEntry=T0.DocEntry where U_Z_AMan='Y' and  T0.DocEntry=" & CInt(strTempID) & " and  U_Z_DocType='" & strReqType & "' and T1.U_Z_aUser='" & strCurrentyApprover & "'"
            End If
            oTemp1.DoQuery(strQuery)
            For intLoop As Integer = 0 To oTemp1.RecordCount - 1
                strQuery1 = "Update [@Z_HR_APPT2] set U_Z_AUser='" & strReplaceApprover & "',U_Z_AName='" & oTemp.Fields.Item("U_Z_RepAutName").Value & "' where DocEntry=" & oTemp1.Fields.Item("DocEntry").Value & " and LineID=" & oTemp1.Fields.Item("LineId").Value
                oTemp2.DoQuery(strQuery1)
                Select Case strReqType
                    Case "Train"
                        strQuery1 = "Update [@Z_HR_TRIN1] set U_Z_CurApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_CurApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)
                        strQuery1 = "Update [@Z_HR_TRIN1] set U_Z_NxtApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_NxtApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)


                        strQuery1 = "Update [@Z_HR_ONTREQ] set U_Z_CurApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_CurApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)
                        strQuery1 = "Update [@Z_HR_ONTREQ] set U_Z_NxtApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_NxtApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)

                    Case "EmpLife"

                        strQuery1 = "Update [@Z_HR_HEM2] set U_Z_CurApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_CurApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)
                        strQuery1 = "Update [@Z_HR_HEM2] set U_Z_NxtApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_NxtApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)

                        strQuery1 = "Update [@Z_HR_HEM4] set U_Z_CurApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_CurApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)
                        strQuery1 = "Update [@Z_HR_HEM4] set U_Z_NxtApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_NxtApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)

                        Try
                            strQuery1 = "Update [U_PEOPLEOBJ] set U_CurApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_CurApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                            oTemp2.DoQuery(strQuery1)
                            strQuery1 = "Update [@Z_HR_HEM4] set U_NxtApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_NxtApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                            oTemp2.DoQuery(strQuery1)
                        Catch ex As Exception

                        End Try
                       

                    Case "ExpCli"

                        strQuery1 = "Update [@Z_HR_EXPCL] set U_Z_CurApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_CurApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)

                        strQuery1 = "Update [@Z_HR_EXPCL] set U_Z_NxtApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_NxtApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)



                    Case "Loanee"

                        strQuery1 = "Update [@Z_HR_LEXPCL] set U_Z_CurApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_CurApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)

                        strQuery1 = "Update [@Z_HR_LEXPCL] set U_Z_NxtApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_NxtApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)
                       
                    Case "TraReq"

                        strQuery1 = "Update [@Z_HR_OTRAREQ] set U_Z_CurApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_CurApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)

                        strQuery1 = "Update [@Z_HR_OTRAREQ] set U_Z_NxtApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_NxtApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)
                    Case "LveReq"

                        strQuery1 = "Update [@Z_PAY_OLETRANS1] set U_Z_CurApprover='" & strReplaceApprover & "' where U_Z_Status='P' and U_Z_CurApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)

                        strQuery1 = "Update [@Z_PAY_OLETRANS1] set U_Z_NxtApprover='" & strReplaceApprover & "' where U_Z_Status='P' and U_Z_NxtApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)




                    Case "Rec"

                        strQuery1 = "Update [@Z_HR_ORMPREQ] set   U_Z_CurApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_CurApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)

                        strQuery1 = "Update [@Z_HR_ORMPREQ] set  U_Z_NxtApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_NxtApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)


                        strQuery1 = "Update [@Z_HR_ORMPREQ] set   U_Z_CurApprover1='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_CurApprover1='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)

                        strQuery1 = "Update [@Z_HR_ORMPREQ] set  U_Z_NxtApprover1='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_NxtApprover1='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)


                        strQuery1 = "Update [U_VACPOSITION] set   U_CurApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_CurApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)

                        strQuery1 = "Update [U_VACPOSITION] set  U_NxtApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_NxtApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)
                     

                        strQuery1 = "Update [@Z_HR_OHEM1] set   U_Z_CurApprover1='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_CurApprover1='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)

                        strQuery1 = "Update [@Z_HR_OHEM1] set  U_Z_NxtApprover1='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_Z_NxtApprover1='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)

                    Case "LoanReq"
                        strQuery1 = "Update [U_LOANREQ] set   U_CurApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_CurApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)

                        strQuery1 = "Update [U_LOANREQ] set  U_NxtApprover='" & strReplaceApprover & "' where U_Z_APPStatus='P' and U_NxtApprover='" & strCurrentyApprover & "' and U_Z_ApproveID=" & oTemp1.Fields.Item("DocEntry").Value
                        oTemp2.DoQuery(strQuery1)
                End Select
                oTemp1.MoveNext()
            Next

            oTemp.MoveNext()
        Next

    End Sub
#End Region

#Region "Methods"
    Private Sub AssignLineNo(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("16").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RAUT1")
            oMatrix.FlushToDataSource()
            For count = 1 To oDataSrc_Line.Size
                oDataSrc_Line.SetValue("LineId", count - 1, count)
            Next
            oMatrix.LoadFromDataSource()
            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
        End Try
    End Sub
#End Region

    Private Sub AddRow(ByVal aForm As SAPbouiCOM.Form)
        Try

            aForm.Freeze(True)
            oMatrix = aForm.Items.Item("16").Specific
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RAUT1")
            If oMatrix.RowCount <= 0 Then
                oMatrix.AddRow()
            End If
            oCombobox = oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific
            Try
                If oCombobox.Selected.Value <> "" Then
                    oMatrix.AddRow()
                    oMatrix.ClearRowData(oMatrix.RowCount)
                End If

            Catch ex As Exception
                aForm.Freeze(False)
            End Try
            oMatrix.FlushToDataSource()
            For count = 1 To oDataSrc_Line.Size
                oDataSrc_Line.SetValue("LineId", count - 1, count)
            Next
            oMatrix.LoadFromDataSource()
            '  oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(SAPbouiCOM.BoCellClickType.ct_Regular)
            AssignLineNo(aForm)
            aForm.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)

        End Try
    End Sub
    Private Sub deleterow(ByVal aForm As SAPbouiCOM.Form)
        oMatrix = aForm.Items.Item("16").Specific
        oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_RAUT1")
        oMatrix.FlushToDataSource()
        For introw As Integer = 1 To oMatrix.RowCount
            If oMatrix.IsRowSelected(introw) Then
                oMatrix.DeleteRow(introw)
                oDataSrc_Line.RemoveRecord(introw - 1)
                'oMatrix = frmSourceMatrix
                For count As Integer = 1 To oDataSrc_Line.Size
                    oDataSrc_Line.SetValue("LineId", count - 1, count)
                Next

                oMatrix = aForm.Items.Item("16").Specific
                oDataSrc_Line = aForm.DataSources.DBDataSources.Item("@Z_HR_RAUT1")
                AssignLineNo(aForm)
                oMatrix.LoadFromDataSource()
                If aForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                    aForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                End If
                Exit Sub
            End If
        Next

    End Sub

#Region "Delete Row"
    Private Sub RefereshDeleteRow(ByVal aForm As SAPbouiCOM.Form)
        If Me.MatrixId = "16" Then
            oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RAUT1")
        End If
        'oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_PRJ1")
        If intSelectedMatrixrow <= 0 Then
            Exit Sub
        End If
        Me.RowtoDelete = intSelectedMatrixrow
        oDataSrc_Line.RemoveRecord(Me.RowtoDelete - 1)
        oMatrix = frmSourceMatrix
        oMatrix.FlushToDataSource()
        For count = 1 To oDataSrc_Line.Size - 1
            oDataSrc_Line.SetValue("LineId", count - 1, count)
        Next
        oMatrix.LoadFromDataSource()
        If oMatrix.RowCount > 0 Then
            oMatrix.DeleteRow(oMatrix.RowCount)
        End If
    End Sub
#End Region

    Private Sub reDrawForm(ByVal oForm As SAPbouiCOM.Form)
        Try
            oForm.Freeze(True)
            'oForm.Items.Item("34").Width = oForm.Width - 30
            'oForm.Items.Item("34").Height = oForm.Height - 170
            oForm.Freeze(False)
        Catch ex As Exception
            oForm.Freeze(False)
        End Try
    End Sub

#Region "Validations"
    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim strCurAuth, strReplace As String
            If oApplication.Utilities.getEdittextvalue(aForm, "6") = "" Then
                oApplication.Utilities.Message("Current Authorizer is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            Else
                strCurAuth = oApplication.Utilities.getEdittextvalue(aForm, "6")
            End If

            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            If oApplication.Utilities.getEdittextvalue(aForm, "9") = "" Then
                oApplication.Utilities.Message("Replaced Authorizer is missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            Else
                strReplace = oApplication.Utilities.getEdittextvalue(aForm, "9")
            End If
            oMatrix = oForm.Items.Item("16").Specific
            Dim strcode, strcode1 As Double
            If oMatrix.RowCount - 1 < 0 Then
                oApplication.Utilities.Message("Line details missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            End If
            Dim strTempID, strQuery As String
            Dim oRec As SAPbobsCOM.Recordset
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            For intRow As Integer = 1 To oMatrix.RowCount
                oCombobox = oMatrix.Columns.Item("V_0").Cells.Item(intRow).Specific
                If oCombobox.Selected.Value <> "" Then
                    strTempID = oApplication.Utilities.getMatrixValues(oMatrix, "V_3", intRow)
                    If strTempID <> "" Then
                        strQuery = "select T0.DocEntry,LineID from [@Z_HR_OAPPT] T0 Inner Join [@Z_HR_APPT2] T1 on T1.DocEntry=T0.DocEntry where   T0.DocEntry=" & CInt(strTempID) & " and  U_Z_DocType='" & oCombobox.Selected.Value & "' and T1.U_Z_aUser='" & strCurAuth & "'"
                        oRec.DoQuery(strQuery)
                        If oRec.RecordCount <= 0 Then
                            oApplication.Utilities.Message("Current Authorizer : " & strCurAuth & " is not available in the selected template code : " & oApplication.Utilities.getMatrixValues(oMatrix, "V_2", intRow) & ".", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Return False
                        End If

                        strQuery = "select T0.DocEntry,LineID from [@Z_HR_OAPPT] T0 Inner Join [@Z_HR_APPT2] T1 on T1.DocEntry=T0.DocEntry where   T0.DocEntry=" & CInt(strTempID) & " and  U_Z_DocType='" & oCombobox.Selected.Value & "' and T1.U_Z_aUser='" & strReplace & "'"
                        oRec.DoQuery(strQuery)
                        If oRec.RecordCount > 0 Then
                            oApplication.Utilities.Message("Replace Authorizer : " & strReplace & " is already available in the selected template code : " & oApplication.Utilities.getMatrixValues(oMatrix, "V_2", intRow) & ".", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Return False
                        End If
                        strQuery = "select T0.DocEntry,LineID from [@Z_HR_OAPPT] T0 Inner Join [@Z_HR_APPT2] T1 on T1.DocEntry=T0.DocEntry where U_Z_AMan='Y' and  T0.DocEntry=" & CInt(strTempID) & " and  U_Z_DocType='" & oCombobox.Selected.Value & "' and T1.U_Z_aUser='" & strCurAuth & "'"

                        oRec.DoQuery(strQuery)
                        If oRec.RecordCount <= 0 Then
                            oApplication.Utilities.Message("Current Authorizer : " & strCurAuth & " is inactive in the selected template code : " & oApplication.Utilities.getMatrixValues(oMatrix, "V_2", intRow) & ".", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Return False
                        End If
                    Else
                        oApplication.Utilities.Message("Template code missing...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If

                  

                End If

            Next
           
            AssignLineNo(oForm)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
#End Region
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_ReplaceAuthorizer Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1" And (oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE) Then
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
                              
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim oCFLs As SAPbouiCOM.ChooseFromListCollection
                                Dim oCons As SAPbouiCOM.Conditions
                                Dim oCon As SAPbouiCOM.Condition
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim OItem As SAPbouiCOM.Item
                                Dim oEdittext As SAPbouiCOM.EditText
                                Dim oMatrix As SAPbouiCOM.Matrix
                                Dim strCFLID As String = ""
                                Try
                                    If oForm.TypeEx <> "0" And pVal.ItemUID <> "" Then
                                        OItem = oForm.Items.Item(pVal.ItemUID)
                                        If OItem.Type = SAPbouiCOM.BoFormItemTypes.it_MATRIX Then
                                            oMatrix = OItem.Specific
                                            strCFLID = oMatrix.Columns.Item(pVal.ColUID).ChooseFromListUID
                                        ElseIf OItem.Type = SAPbouiCOM.BoFormItemTypes.it_EDIT Then
                                            oEdittext = OItem.Specific
                                            strCFLID = oEdittext.ChooseFromListUID
                                        End If
                                        If OItem.UniqueID = "16" And pVal.ColUID = "V_3" Then
                                            oMatrix = OItem.Specific
                                            oCombobox = oMatrix.Columns.Item("V_0").Cells.Item(pVal.Row).Specific
                                            Dim strType As String = oCombobox.Selected.Value
                                            oApplication.Utilities.filterProjectChooseFromList(oForm, strCFLID, strType)

                                        End If


                                    End If
                                Catch ex As Exception
                                    oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                End Try

                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.GetForm(pVal.FormTypeEx, pVal.FormTypeCount)
                                If pVal.ItemUID = "16" And pVal.Row > 0 Then
                                    oMatrix = oForm.Items.Item("16").Specific
                                    Me.RowtoDelete = pVal.Row
                                    intSelectedMatrixrow = pVal.Row
                                    Me.MatrixId = "16"
                                    frmSourceMatrix = oMatrix
                                End If
                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                reDrawForm(oForm)
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "16" And pVal.ColUID = "V_0" Then
                                    oMatrix = oForm.Items.Item(pVal.ItemUID).Specific
                                    oCombobox = oMatrix.Columns.Item("V_0").Cells.Item(pVal.Row).Specific
                                    '  oApplication.Utilities.SetMatrixValues(oMatrix, "V_1", pVal.Row, oCombobox.Selected.Description)
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Select Case pVal.ItemUID
                                    Case "8"
                                        oForm.PaneLevel = 1
                                    Case "9"
                                        oForm.PaneLevel = 2
                                    Case "33"
                                        oForm.PaneLevel = 3
                                    Case "12"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_ADD_ROW)
                                    Case "13"
                                        oApplication.SBO_Application.ActivateMenuItem(mnu_DELETE_ROW)
                                End Select
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1 As String
                                Dim sCHFL_ID, val As String
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
                                        If pVal.ItemUID = "6" Then
                                            val = oDataTable.GetValue("USER_CODE", 0)
                                            val1 = oDataTable.GetValue("U_NAME", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "7", val1)
                                                oApplication.Utilities.setEdittextvalue(oForm, "6", val)
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If pVal.ItemUID = "9" Then
                                            val = oDataTable.GetValue("USER_CODE", 0)
                                            val1 = oDataTable.GetValue("U_NAME", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "10", val1)
                                                oApplication.Utilities.setEdittextvalue(oForm, "9", val)
                                            Catch ex As Exception
                                            End Try
                                        End If

                                        If pVal.ItemUID = "16" And pVal.ColUID = "V_3" Then
                                            val = oDataTable.GetValue("DocEntry", 0)
                                            val1 = oDataTable.GetValue("U_Z_Name", 0)
                                            oMatrix = oForm.Items.Item(pVal.ItemUID).Specific
                                            Try
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_2", pVal.Row, val1)
                                                oApplication.Utilities.SetMatrixValues(oMatrix, "V_3", pVal.Row, val)
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
                Case mnu_ReplaceAuthorizer
                    LoadForm()
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        oForm.Items.Item("5").Enabled = False
                        'oForm.Items.Item("7").Enabled = False
                    End If
                Case mnu_ADD_ROW

                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        AddRow(oForm)
                    End If
                Case mnu_DELETE_ROW
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = False Then
                        RefereshDeleteRow(oForm)
                    Else
                        'If ValidateDeletion(oForm) = False Then
                        '    BubbleEvent = False
                        '    Exit Sub
                        'End If
                    End If
                Case mnu_ADD
                    If pVal.BeforeAction = False Then
                        oForm = oApplication.SBO_Application.Forms.ActiveForm()
                        Dim strCode As String = oApplication.Utilities.getMaxCode("@Z_HR_RAUT", "DocNum")
                        oForm.Items.Item("6").Enabled = True
                        oForm.Items.Item("9").Enabled = True
                        oForm.Items.Item("7").Enabled = False
                        oForm.Items.Item("10").Enabled = False
                        oApplication.Utilities.setEdittextvalue(oForm, "14", strCode)
                        oForm.Items.Item("15").Enabled = True
                        oForm.Items.Item("15").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                        oApplication.Utilities.setEdittextvalue(oForm, "15", "t")
                        oApplication.SBO_Application.SendKeys("{TAB}")
                        oForm.Items.Item("6").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                        oForm.Items.Item("16").Enabled = True
                        oForm.Items.Item("14").Enabled = False
                        oForm.Items.Item("15").Enabled = False
                        oForm.Items.Item("12").Enabled = True
                        oForm.Items.Item("13").Enabled = True

                    End If
                Case mnu_FIND
                    If pVal.BeforeAction = False Then
                        oForm = oApplication.SBO_Application.Forms.ActiveForm()
                        oForm.Items.Item("6").Enabled = True
                        oForm.Items.Item("9").Enabled = True
                        oForm.Items.Item("7").Enabled = True
                        oForm.Items.Item("10").Enabled = True
                        oForm.Items.Item("14").Enabled = True
                        oForm.Items.Item("15").Enabled = True
                        oForm.Items.Item("16").Enabled = False
                        oForm.Items.Item("12").Enabled = True
                        oForm.Items.Item("13").Enabled = True
                    End If
                Case "1283"
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    If pVal.BeforeAction = True Then
                        Dim strValue As String
                        If oApplication.SBO_Application.MessageBox("Do you want to delete the details?", , "Yes", "No") = 2 Then
                            BubbleEvent = False
                            Exit Sub
                        End If
                        strValue = oApplication.Utilities.getEdittextvalue(oForm, "5")
                        If oApplication.Utilities.ValidateCode(strValue, "SALARY") = True Then
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
            If BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
                If oForm.TypeEx = frm_ReplaceAuthorizer Then
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oForm.Items.Item("17").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                    oForm.Items.Item("6").Enabled = False
                    oForm.Items.Item("9").Enabled = False
                    oForm.Items.Item("7").Enabled = False
                    oForm.Items.Item("10").Enabled = False
                    oForm.Items.Item("15").Enabled = False
                    oForm.Items.Item("16").Enabled = False
                    oForm.Items.Item("14").Enabled = False
                    oForm.Items.Item("15").Enabled = False
                    oForm.Items.Item("12").Enabled = False
                    oForm.Items.Item("13").Enabled = False
                End If
            End If
            If BusinessObjectInfo.BeforeAction = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD Or BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE) Then
                oForm = oApplication.SBO_Application.Forms.ActiveForm()
                ' strDocEntry = oApplication.Utilities.getEdittextvalue(oForm, "4")
            End If
            If BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD) Then
                oForm = oApplication.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID)
                If oForm.TypeEx = frm_ReplaceAuthorizer Then
                    Dim strdocnum As String
                    Dim stXML As String = BusinessObjectInfo.ObjectKey
                    stXML = stXML.Replace("<?xml version=""1.0"" encoding=""UTF-16"" ?><Replace_AuthorizerParams><DocEntry>", "")
                    stXML = stXML.Replace("</DocEntry></Replace_AuthorizerParams>", "")
                    Dim otest As SAPbobsCOM.Recordset
                    otest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    If stXML <> "" Then
                        otest.DoQuery("select * from [@Z_HR_RAUT]  where DocEntry=" & stXML)
                        If otest.RecordCount > 0 Then
                            ReplaceAuthorizer(otest.Fields.Item("DocEntry").Value)
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message)

        End Try
    End Sub
End Class
