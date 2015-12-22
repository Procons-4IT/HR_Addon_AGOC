Imports System.IO
Public Class ClshrIPOfferAcceptance
    Inherits clsBase
    Private InvForConsumedItems As Integer
    Private Shared strFunction As String
    Private oGrid, oGridDetail, oOfferGrid As SAPbouiCOM.Grid
    Private strHeaderQry, strDetailQry, strFilepath As String
    Private oDT_DIAPI As DataTable
    Private oCombo As SAPbouiCOM.ComboBox
    Dim oGenService As SAPbobsCOM.GeneralService
    Dim oGenData As SAPbobsCOM.GeneralData
    Dim oGenDataCollection As SAPbobsCOM.GeneralDataCollection
    Dim oCompService As SAPbobsCOM.CompanyService
    Dim oChildData As SAPbobsCOM.GeneralData
    Dim oGeneralDataParams As SAPbobsCOM.GeneralDataParams
    Private ocomboCol As SAPbouiCOM.ComboBoxColumn
    Private strQuery As String
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub

#Region "LoadForm"

    Public Sub LoadForm(ByVal sFunction As String, ByVal strRQType As String)

        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_IPOfferAcceptance) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_IPOfferAcceptance, frm_hr_IPOfferAcceptance)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Title = "Employment Offer"

        oForm.Freeze(True)
        strFunction = sFunction
        oGrid = oForm.Items.Item("1").Specific
        oGridDetail = oForm.Items.Item("5").Specific
        oOfferGrid = oForm.Items.Item("6").Specific

        oForm.ActiveItem = 10
        oForm.DataSources.UserDataSources.Add("CFLRRR", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.DataTables.Add("DT_0")
        oForm.DataSources.DataTables.Add("DT_1")
        oForm.DataSources.DataTables.Add("DT_2")
        oGrid.DataTable = oForm.DataSources.DataTables.Item("DT_0")
        oGridDetail.DataTable = oForm.DataSources.DataTables.Item("DT_1")
        oOfferGrid.DataTable = oForm.DataSources.DataTables.Item("DT_2")
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single

        strHeaderQry = ""
        strHeaderQry = "Select T0.""DocEntry"",T0.""U_Z_HRAppID"" as ""App/ID"",T0.""U_Z_HRAppName"" as ""App/Name"",T0.""U_Z_DeptName"" as ""Department Name"",T0.""U_Z_JobPosi"" as ""Position Name"", "
        strHeaderQry += " T0.""U_Z_ReqNo"" as ""Req No"",T1.""U_Z_AppStatus"" As ""Request Status"",T0.""U_Z_Email"" as ""EMail"",T0.""U_Z_Mobile"" as ""Mobile"",T0.""U_Z_ApplStatus"" as ""App Status"", T0.""U_Z_Skills"" as ""Skills"",T0.""U_Z_YrExp"" as ""YOE"","
        strHeaderQry += " case T0.""U_Z_IPLUSta"" when 'S' then 'Selected' when 'R' then 'Rejected' when 'S' then 'Selected' else 'Pending' end as ""LM Status"",case T0.""U_Z_IPHODSta"" when 'S' "
        strHeaderQry += " then 'Selected' when 'R'then 'Rejected' when 'S' then 'Selected' else 'Pending' end as ""HOD Status"",case T0.""U_Z_IPHRSta"" when 'S' then 'Selected' when 'R'then "
        'strHeaderQry += " 'Rejected' when 'S' then 'Selected' else 'Pending' end as 'HR Status',U_Z_Finished As 'Work Flow Status' "
        strHeaderQry += " 'Rejected' when 'S' then 'Selected' else 'Pending' end as ""HR Status"""
        strHeaderQry += " from ""@Z_HR_OHEM1"" T0 Join ""@Z_HR_ORMPREQ"" T1 On T1.""DocEntry"" = T0.""U_Z_ReqNo""  "
        strHeaderQry += " Where T0.""U_Z_IntervStatus"" = 'A' And T0.""U_Z_ReqNo"" = " & strRQType & " And T0.U_Z_AppStatus = 'A'"

        oGrid.DataTable.ExecuteQuery(strHeaderQry)
        oEditTextColumn = oGrid.Columns.Item("Req No")
        '   oDataSrc_Line = oForm.DataSources.DBDataSources.Item("@Z_HR_RMPREQ1")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
        oForm.Items.Item("1").Enabled = False
        oEditTextColumn = oGrid.Columns.Item("App/ID")
        oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
        oGrid.Columns.Item("LM Status").TitleObject.Caption = "Interview Summary Status"
        oGrid.Columns.Item("HOD Status").TitleObject.Caption = "Approval Status"
        oGrid.Columns.Item("HR Status").Visible = False
        oGrid.Columns.Item("DocEntry").Visible = False
        oGrid.Columns.Item("Department Name").Visible = False
        oGrid.Columns.Item("Mobile").Visible = False
        oGrid.Columns.Item("Skills").Visible = False
        oGrid.Columns.Item("Position Name").Visible = False
        oGrid.Columns.Item("App Status").Visible = False
        oGrid.AutoResizeColumns()

        Dim DocNo As Integer = 0
        Dim HRStatus As String = String.Empty
        Dim WFStatus As String = String.Empty
        Dim ReqStatus As String = String.Empty
        If oGrid.DataTable.Rows.Count > 0 Then
            oGrid.Rows.SelectedRows.Add(0)
            DocNo = oGrid.DataTable.GetValue("DocEntry", 0)
            HRStatus = oGrid.DataTable.GetValue("HOD Status", 0)
            ' WFStatus = oGrid.DataTable.GetValue("Work Flow Status", 0)
            ReqStatus = oGrid.DataTable.GetValue("Request Status", 0)
        End If

        If DocNo = 0 Then
            oApplication.Utilities.Message("No Records Found...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
            Return
        End If

        Dim oCFLs1 As SAPbouiCOM.ChooseFromListCollection
        oCFLs1 = oForm.ChooseFromLists
        Dim oCFL1 As SAPbouiCOM.ChooseFromList
        Dim oCFLCreationParams5 As SAPbouiCOM.ChooseFromListCreationParams
        oCFLCreationParams5 = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
        oCFLCreationParams5.MultiSelection = False
        oCFLCreationParams5.UniqueID = "CFLORR"
        oCFLCreationParams5.ObjectType = "Z_HR_OOREJ"
        oCFL1 = oCFLs1.Add(oCFLCreationParams5)

        oGrid.Columns.Item("Request Status").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        ocomboCol = oGrid.Columns.Item("Request Status")
        ocomboCol.ValidValues.Add("P", "Pending")
        ocomboCol.ValidValues.Add("A", "Approved")
        ocomboCol.ValidValues.Add("R", "Rejected")
        ocomboCol.ValidValues.Add("C", "Closed")
        ocomboCol.ValidValues.Add("L", "Canceled")
        ocomboCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
        oGrid.Columns.Item("Request Status").Visible = False
        oApplication.Utilities.assignMatrixLineno(oGrid, oForm)
        ForHR(DocNo, HRStatus) 'Interview Details
        offerDetails(DocNo) 'Offer Details
        enableControl(HRStatus, ReqStatus)
        reDrawScreen(oForm)
        oForm.Freeze(False)

    End Sub

#End Region

#Region "For HR"
    Private Sub ForHR(ByVal DocNo As Integer, ByVal strHRStatus As String)
        oGridDetail = oForm.Items.Item("5").Specific
        strDetailQry = ""
        strDetailQry = "Select ISNULL(""U_Z_InType"",'-') as ""Interview Type"",""U_Z_ScheduleDate"" as ""Schedule Date"",""U_Z_SchEmpID"" as ""Scheduler EmpID"", T1.""firstName"" As ""Scheduler Name"",""U_Z_InterviewDate"" as ""Interview Date"",""U_Z_InterviwerID"" as ""Interviewer EmpID"",""U_Z_Status"" as ""Status"",""U_Z_InterviewStatus"" as ""Interview Status"",""U_Z_Rating"" as ""Rating"",""U_Z_RatPer"" as ""Rating Percentage"",""U_Z_FileName"" as ""Attachment"",""U_Z_Comments"" as ""Comments"" from ""@Z_HR_OHEM2"" T0 Left Outer Join OHEM T1 On T0.""U_Z_SchEmpID"" = T1.""empID"" Where ""DocEntry""=" & DocNo & ""
        oGridDetail.DataTable.ExecuteQuery(strDetailQry)

        Dim oGCol0, oGCol1, oGCol2 As SAPbouiCOM.GridColumn
        Dim oGCCol0, oGCCol1, oGCCol2 As SAPbouiCOM.ComboBoxColumn
        Dim oGECol, oGECol1, oGECol14 As SAPbouiCOM.EditTextColumn

        oGCol0 = oGridDetail.Columns.Item("Interview Type")
        oGCol1 = oGridDetail.Columns.Item("Status")
        oGCol2 = oGridDetail.Columns.Item("Interview Status")

        oGECol = oGridDetail.Columns.Item("Interviewer EmpID")
        oGECol.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
        oGECol.Editable = False
        oGECol = oGridDetail.Columns.Item("Interview Date")
        oGECol.Editable = False

        oGECol1 = oGridDetail.Columns.Item("Rating")
        oGCol0.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGCol1.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGCol2.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        oGCCol0 = oGridDetail.Columns.Item("Interview Type")
        oGCCol1 = oGridDetail.Columns.Item("Status")
        oGCCol2 = oGridDetail.Columns.Item("Interview Status")

        oGECol14 = oGridDetail.Columns.Item("Scheduler EmpID")
        oGECol14.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee


        Dim oRec As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRec.DoQuery("Select ""U_Z_TypeCode"" As ""Code"",""U_Z_TypeName"" As ""Name"" From ""@Z_HR_OITYP""")
        oGCCol0.ValidValues.Add("-", "-")
        For i As Integer = 0 To oRec.RecordCount - 1
            oGCCol0.ValidValues.Add(oRec.Fields.Item("Code").Value.ToString(), oRec.Fields.Item("Name").Value.ToString())
            oRec.MoveNext()
        Next
        oGCCol0.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

        oGCCol1.ValidValues.Add("-", "Pending")
        oGCCol1.ValidValues.Add("CO", "Conducted")
        oGCCol1.ValidValues.Add("CA", "Cancelled")
        oGCCol1.ValidValues.Add("RS", "Rescheduled")
        oGCCol1.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

        oGCCol2.ValidValues.Add("S", "Selected")
        oGCCol2.ValidValues.Add("R", "Rejected")
        oGCCol2.ValidValues.Add("-", "Pending")
        oGCCol2.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

        oGridDetail.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        oApplication.Utilities.assignMatrixLineno(oGridDetail, oForm)

        oForm.Items.Item("5").Enabled = False

    End Sub

    Private Sub offerDetails(ByVal strDocEntry As String)
        oOfferGrid = oForm.Items.Item("6").Specific
        Dim oGECol As SAPbouiCOM.EditTextColumn
        strDetailQry = ""
        strDetailQry = "Select ""U_Z_Basic"",""U_Z_Benifit"",""U_Z_Attachment"",""U_Z_Status"",""U_Z_JoinDate"",""U_Z_RejReason"",""U_Z_Remarks"" From ""@Z_HR_OHEM3"" Where ""DocEntry"" = '" & strDocEntry & "'"
        oOfferGrid.DataTable.ExecuteQuery(strDetailQry)

        oOfferGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single

        oOfferGrid.Columns.Item("U_Z_Basic").TitleObject.Caption = "Offered Basic"
        oOfferGrid.Columns.Item("U_Z_Benifit").TitleObject.Caption = "Benifits"
        oOfferGrid.Columns.Item("U_Z_Benifit").Visible = False
        oOfferGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Offer Status"
        oOfferGrid.Columns.Item("U_Z_JoinDate").TitleObject.Caption = "Joining Date"
        oOfferGrid.Columns.Item("U_Z_JoinDate").Visible = True
        oOfferGrid.Columns.Item("U_Z_Attachment").TitleObject.Caption = "Attachments(Double click to Select Attachment)"
        oGECol = oOfferGrid.Columns.Item("U_Z_Attachment")
        oGECol.LinkedObjectType = "Z_HR_OEXFOM"
        oOfferGrid.Columns.Item("U_Z_Attachment").Editable = False
        oOfferGrid.Columns.Item("U_Z_RejReason").TitleObject.Caption = "Rejection Reason"
        oOfferGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Additional Details"

        'Offer Acceptance Status
        oOfferGrid.Columns.Item("U_Z_Status").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
        Dim oGCCol As SAPbouiCOM.ComboBoxColumn
        oGCCol = oOfferGrid.Columns.Item("U_Z_Status")
        oGCCol.ValidValues.Add("-", "Pending")
        oGCCol.ValidValues.Add("O", "Offered")
        oGCCol.ValidValues.Add("A", "Offer Accepted")
        oGCCol.ValidValues.Add("J", "Offer Rejected")
        oGCCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

        Dim oGCol As SAPbouiCOM.EditTextColumn
        oGCol = oOfferGrid.Columns.Item("U_Z_RejReason")
        oGCol.ChooseFromListUID = "CFLORR"
        oGCol.ChooseFromListAlias = "U_Z_TypeCode"
        oApplication.Utilities.assignMatrixLineno(oOfferGrid, oForm)
    End Sub
#End Region

#Region "Offer Acceptance DIAPI"
    Private Function oFferAcceptance_DIAPI(ByVal DocNo As String) As Boolean
        Dim RetVal As Boolean
        RetVal = False
        Try

            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            End If

            oApplication.Company.StartTransaction()
            oCompService = oApplication.Company.GetCompanyService
            oGenService = oCompService.GetGeneralService("Z_HR_OHEM")
            oGenData = oGenService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData)
            oGeneralDataParams = oGenService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)
            oGeneralDataParams.SetProperty("DocEntry", Convert.ToInt32(DocNo))
            oGenData = oGenService.GetByParams(oGeneralDataParams)
            oGenDataCollection = oGenData.Child("Z_HR_OHEM3")
            For i As Integer = 0 To oOfferGrid.DataTable.Rows.Count - 1
                If i > oGenDataCollection.Count - 1 Then
                    oChildData = oGenDataCollection.Add()
                Else
                    oChildData = oGenDataCollection.Item(i)
                End If
                oChildData.SetProperty("U_Z_Basic", oOfferGrid.DataTable.GetValue("U_Z_Basic", i))
                'oChildData.SetProperty("U_Z_Benifit", oOfferGrid.DataTable.GetValue("U_Z_Benifit", i))
                oChildData.SetProperty("U_Z_Attachment", oOfferGrid.DataTable.GetValue("U_Z_Attachment", i))
                If oOfferGrid.DataTable.GetValue("U_Z_Status", i) <> "" Then
                    oChildData.SetProperty("U_Z_Status", oOfferGrid.DataTable.GetValue("U_Z_Status", i))
                End If
                If Not IsNothing(oOfferGrid.DataTable.GetValue("U_Z_JoinDate", i)) Then
                    'oGridDetail.DataTable.Columns.Item("Interview Date").Cells.Item(i).Value
                    If oOfferGrid.DataTable.GetValue("U_Z_JoinDate", i).ToString <> "" Then
                        oChildData.SetProperty("U_Z_JoinDate", oOfferGrid.DataTable.GetValue("U_Z_JoinDate", i))
                    End If
                End If
                oChildData.SetProperty("U_Z_RejReason", oOfferGrid.DataTable.GetValue("U_Z_RejReason", i))
                oChildData.SetProperty("U_Z_Remarks", oOfferGrid.DataTable.GetValue("U_Z_Remarks", i))
            Next
            
            oGenService.Update(oGenData)

            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                For i As Integer = 0 To oOfferGrid.DataTable.Rows.Count - 1
                    Dim oRec As SAPbobsCOM.Recordset
                    oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    Dim strQry = "Select ""AttachPath"" From OADP"
                    oRec.DoQuery(strQry)
                    Dim SPath As String = oOfferGrid.DataTable.GetValue("U_Z_Attachment", i).ToString()
                    If SPath = "" Then
                    Else
                        Dim DPath As String = ""
                        If Not oRec.EoF Then
                            DPath = oRec.Fields.Item("AttachPath").Value.ToString()
                        End If
                        If Not Directory.Exists(DPath) Then
                            Directory.CreateDirectory(DPath)
                        End If
                        Dim file = New FileInfo(SPath)
                        Dim Filename As String = Path.GetFileName(SPath)
                        Dim SavePath As String = Path.Combine(DPath, Filename)
                        If System.IO.File.Exists(SavePath) Then
                        Else
                            file.CopyTo(Path.Combine(DPath, file.Name), True)
                        End If
                    End If
                Next
            End If
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            If oApplication.Company.InTransaction() Then
                oApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            End If
            Return False
        End Try
    End Function
#End Region

#Region "HR Status & Comments Update"
    Private Sub HR_SC_Update(ByVal DocNo As String, ByVal strAPPID As String, ByVal Status As String, ByVal Comments As String)
        Dim oRS As SAPbobsCOM.Recordset
        oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

        'Candidate Job Offered
        strQuery = "Update ""@Z_HR_OHEM1"" set ""U_Z_OfferStatus"" = '" & Status & "' where ""DocEntry"" = '" & DocNo & "'"
        oRS.DoQuery(strQuery)

        If Status = "O" Then
            strQuery = "Update ""@Z_HR_OCRAPP"" set ""U_Z_Status"" = 'O' where ""DocEntry"" = '" & strAPPID & "'"
            oRS.DoQuery(strQuery)
            'ElseIf Status = "A" Then
            '    strQuery = "Update [@Z_HR_OCRAPP] set U_Z_Status = 'A' where DocEntry = '" & strAPPID & "'"
            '    oRS.DoQuery(strQuery)
            'ElseIf Status = "J" Then
            '    strQuery = "Update [@Z_HR_OCRAPP] set U_Z_Status = 'J' where DocEntry = '" & strAPPID & "'"
            '    oRS.DoQuery(strQuery)
        End If

        oApplication.Utilities.Message("Document Updated Sucessfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
    End Sub

    Private Sub HR_SC_Update_Finished(ByVal DocNo As String, ByVal strAPPID As String, ByVal Status As String, ByVal Comments As String, ByVal aBasic As Double, ByVal stJDate As String, ByVal dtJDate As Date)
        Dim oRS As SAPbobsCOM.Recordset
        oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        'Candidate Job Offered
        strQuery = "Update ""@Z_HR_OHEM1"" set ""U_Z_OfferStatus"" = '" & Status & "' where ""DocEntry"" = '" & DocNo & "'"
        oRS.DoQuery(strQuery)

        If Status = "O" Then
            strQuery = "Update ""@Z_HR_OCRAPP"" set ""U_Z_Status"" = 'O' where ""DocEntry"" = '" & strAPPID & "'"
            oRS.DoQuery(strQuery)
        ElseIf Status = "A" Then
            strQuery = "Update ""@Z_HR_OHEM1"" set ""U_Z_Finished"" = 'Y' where ""DocEntry"" = '" & DocNo & "'"
            oRS.DoQuery(strQuery)
            If stJDate = "" Then
                strQuery = "Update ""@Z_HR_OCRAPP"" set ""U_Z_OffBasic"" = '" & aBasic & "', ""U_Z_Status"" = 'A' where ""DocEntry"" = '" & strAPPID & "'"
            Else
                Dim strmonth, strdate, stryear As String
                Dim Jdate As Date '= oApplication.Utilities.GetDateTimeValue(dtJDate)
                strQuery = "Update ""@Z_HR_OCRAPP"" set ""U_Z_OffBasic"" = '" & aBasic & "',""U_Z_JoinDate"" = '" & dtJDate.ToString("yyyy-MM-dd") & "', ""U_Z_Status"" = 'A' where ""DocEntry"" = '" & strAPPID & "'"
            End If
            oRS.DoQuery(strQuery)
        ElseIf Status = "J" Then
            strQuery = "Update ""@Z_HR_OHEM1"" set ""U_Z_Finished"" = 'Y' where ""DocEntry"" = '" & DocNo & "'"
            oRS.DoQuery(strQuery)

            strQuery = "Update ""@Z_HR_OCRAPP"" set ""U_Z_Status"" = 'R' where ""DocEntry"" = '" & strAPPID & "'"
            oRS.DoQuery(strQuery)
        End If

        oApplication.Utilities.Message("Document Updated Sucessfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
    End Sub
#End Region

#Region "FileOpen"
    Private Sub FileOpen()
        Dim mythr As New System.Threading.Thread(AddressOf ShowFileDialog)
        mythr.SetApartmentState(Threading.ApartmentState.STA)
        mythr.Start()
        mythr.Join()
    End Sub

    Private Sub ShowFileDialog()
        Dim oDialogBox As New OpenFileDialog
        Dim strMdbFilePath As String
        Dim oProcesses() As Process
        Try
            oProcesses = Process.GetProcessesByName("SAP Business One")
            If oProcesses.Length <> 0 Then
                For i As Integer = 0 To oProcesses.Length - 1
                    Dim MyWindow As New clsListener.WindowWrapper(oProcesses(i).MainWindowHandle)
                    If oDialogBox.ShowDialog(MyWindow) = DialogResult.OK Then
                        strMdbFilePath = oDialogBox.FileName
                        strFilepath = oDialogBox.FileName
                    Else
                    End If
                Next
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
        End Try
    End Sub
#End Region

    Private Sub LoadFiles(ByVal aform As SAPbouiCOM.Form)
        oGrid = aform.Items.Item("6").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.Rows.IsSelected(intRow) Then
                Dim strFilename, strFilePath As String
                strFilename = oGrid.DataTable.GetValue("U_Z_Attachment", intRow)
                Dim Filename As String = Path.GetFileName(strFilename)
                strFilePath = oGrid.DataTable.GetValue("U_Z_Attachment", intRow)

                If File.Exists(strFilePath) = False Then
                    Dim oRec As SAPbobsCOM.Recordset
                    oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    Dim strQry = "Select ""AttachPath"" From OADP"
                    oRec.DoQuery(strQry)
                    strFilePath = oRec.Fields.Item(0).Value

                    If Filename = "" Then
                        strFilePath = strFilePath
                    Else
                        strFilePath = strFilePath & Filename
                    End If
                    If File.Exists(strFilePath) = False Then
                        oApplication.Utilities.Message("File does not exists ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Exit Sub
                    End If
                    strFilename = strFilePath
                Else
                    strFilename = strFilePath
                End If

                Dim x As System.Diagnostics.ProcessStartInfo
                x = New System.Diagnostics.ProcessStartInfo
                x.UseShellExecute = True
                x.FileName = strFilename
                System.Diagnostics.Process.Start(x)
                x = Nothing
                Exit Sub
            End If
        Next
        oApplication.Utilities.Message("No file has been selected...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
    End Sub
#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_IPOfferAcceptance Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "3" Or pVal.ItemUID = "13" Then
                                    If Not validate() Then
                                        BubbleEvent = False
                                    End If
                                ElseIf (pVal.ItemUID = "14") Then
                                    oForm.Close()
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1" And pVal.ColUID = "App/ID" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcode As String = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    Dim ooBj As New clshrCrApplicants
                                    ooBj.ViewCandidate(strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If

                          
                                If pVal.ItemUID = "1" And pVal.ColUID = "Req No" Then
                                    oGrid = oForm.Items.Item(pVal.ItemUID).Specific
                                    Dim strcode As String = oGrid.DataTable.GetValue(pVal.ColUID, pVal.Row)
                                    If oForm.Title = "Employment Offer" Then
                                        Dim objct As New clshrMPRequest
                                        objct.LoadForm1(strcode, "Employment Offer", , , )
                                   
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If
                        End Select
                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_RESIZE
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                reDrawScreen(oForm)
                            Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "6" And pVal.ColUID = "U_Z_Attachment" Then
                                    oGrid = oForm.Items.Item("6").Specific
                                    oGrid.Columns.Item("RowsHeader").Click(pVal.Row)
                                    LoadFiles(oForm)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "20" Then
                                    LoadFiles(oForm)
                                End If
                             
                                If pVal.ItemUID = "3" Then
                                    oGrid = oForm.Items.Item("1").Specific
                                    oOfferGrid = oForm.Items.Item("6").Specific
                                    If oGrid.Rows.Count > 0 Then
                                        If oApplication.SBO_Application.MessageBox("Click Yes to Proceed", , "Yes", "No") = 2 Then
                                            Exit Sub
                                        Else
                                            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                                If oGrid.Rows.IsSelected(intRow) Then
                                                    Dim strDocEntry As String = oGrid.DataTable.GetValue("DocEntry", intRow)
                                                    Dim strAppID As String = oGrid.DataTable.GetValue("App/ID", intRow)
                                                    Dim strComment As String = ""
                                                    Dim strOStatus As String = oOfferGrid.DataTable.GetValue("U_Z_Status", oOfferGrid.DataTable.Rows.Count - 1)
                                                    Dim dblBasic As Double = oOfferGrid.DataTable.GetValue("U_Z_Basic", oOfferGrid.DataTable.Rows.Count - 1)

                                                    oFferAcceptance_DIAPI(strDocEntry)
                                                    HR_SC_Update(strDocEntry, strAppID, strOStatus, strComment)
                                                End If
                                            Next
                                            'oForm.Close()
                                        End If
                                    End If
                                ElseIf pVal.ItemUID = "13" Then
                                    oGrid = oForm.Items.Item("1").Specific
                                    If oGrid.Rows.Count > 0 Then
                                        If oApplication.SBO_Application.MessageBox("Click Yes to Proceed", , "Yes", "No") = 2 Then
                                            Exit Sub
                                        Else
                                            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                                                If oGrid.Rows.IsSelected(intRow) Then
                                                    Dim strDocEntry As String = oGrid.DataTable.GetValue("DocEntry", intRow)
                                                    Dim strAppID As String = oGrid.DataTable.GetValue("App/ID", intRow)
                                                    Dim strComment As String = ""
                                                    Dim strOStatus As String = oOfferGrid.DataTable.GetValue("U_Z_Status", oOfferGrid.DataTable.Rows.Count - 1)
                                                    Dim dblBasic As Double = oOfferGrid.DataTable.GetValue("U_Z_Basic", oOfferGrid.DataTable.Rows.Count - 1)
                                                    Dim dtJoinDt As String = oOfferGrid.DataTable.GetValue("U_Z_JoinDate", oOfferGrid.DataTable.Rows.Count - 1)

                                                    oFferAcceptance_DIAPI(strDocEntry)
                                                    HR_SC_Update(strDocEntry, strAppID, strOStatus, strComment)
                                                    If dtJoinDt = "" Then
                                                        HR_SC_Update_Finished(strDocEntry, strAppID, strOStatus, strComment, dblBasic, "", Now.Date)
                                                    Else
                                                        Dim dat As Date = oOfferGrid.DataTable.GetValue("U_Z_JoinDate", oOfferGrid.DataTable.Rows.Count - 1)
                                                        HR_SC_Update_Finished(strDocEntry, strAppID, strOStatus, strComment, dblBasic, dtJoinDt, dat)
                                                    End If
                                                End If
                                            Next
                                            ' oForm.Close()
                                        End If
                                    End If
                                ElseIf pVal.ItemUID = "1" And pVal.ColUID = "RowsHeader" And pVal.Row <> -1 Then
                                    oGrid = oForm.Items.Item("1").Specific
                                    If oGrid.Rows.Count > 0 Then
                                        Dim DocNo As Integer = Convert.ToInt32(oGrid.DataTable.GetValue("DocEntry", pVal.Row))
                                        Dim LMStatus As String = oGrid.DataTable.GetValue("HOD Status", pVal.Row)
                                        Dim HODStatus As String = oGrid.DataTable.GetValue("HOD Status", pVal.Row)
                                        Dim HRStatus As String = oGrid.DataTable.GetValue("HR Status", pVal.Row)
                                        Dim strRsta As String = oGrid.DataTable.GetValue("Request Status", pVal.Row)
                                        ' Dim WFStatus As String = oGrid.DataTable.GetValue("Work Flow Status", pVal.Row)
                                        Dim ReqStatus As String = oGrid.DataTable.GetValue("Request Status", 0)

                                        oForm.Freeze(True)
                                        ForHR(DocNo, HRStatus)
                                        offerDetails(DocNo)
                                        enableControl(HRStatus, ReqStatus)
                                        oForm.Freeze(False)
                                    End If
                                ElseIf pVal.ItemUID = "11" Then
                                    oOfferGrid = oForm.Items.Item("6").Specific
                                    oOfferGrid.DataTable.Rows.Add(1)
                                ElseIf pVal.ItemUID = "12" Then
                                    oOfferGrid = oForm.Items.Item("6").Specific
                                    If oOfferGrid.Rows.Count > 0 Then
                                        For intRow As Integer = 0 To oOfferGrid.DataTable.Rows.Count - 1
                                            If oOfferGrid.Rows.IsSelected(intRow) Then
                                                oOfferGrid.DataTable.Rows.Remove(intRow)
                                            End If
                                        Next
                                    End If
                                ElseIf pVal.ItemUID = "9" Then
                                    oForm.PaneLevel = 0
                                ElseIf pVal.ItemUID = "8" Then
                                    oForm.PaneLevel = 1
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_DOUBLE_CLICK
                                If pVal.ItemUID = "6" And pVal.ColUID = "U_Z_Attachment" Then
                                    oOfferGrid = oForm.Items.Item("6").Specific
                                    Dim strPath As String = oOfferGrid.DataTable.Columns.Item("U_Z_Attachment").Cells.Item(pVal.Row).Value.ToString()
                                    FileOpen()
                                    If strFilepath = "" Then
                                        'oApplication.Utilities.Message("Please Select a File", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                                        BubbleEvent = False
                                    Else
                                        oOfferGrid.DataTable.Columns.Item("U_Z_Attachment").Cells.Item(pVal.Row).Value = strFilepath
                                    End If
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                oOfferGrid = oForm.Items.Item("6").Specific
                                If pVal.ItemUID = "6" And pVal.ColUID = "U_Z_RejReason" Then
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
                                            val1 = oDataTable.GetValue("U_Z_TypeCode", 0)
                                            Try
                                                oOfferGrid.DataTable.Columns.Item("U_Z_RejReason").Cells.Item(pVal.Row).Value = val1
                                            Catch ex As Exception
                                            End Try
                                            oForm.Freeze(False)
                                        End If
                                    Catch ex As Exception
                                        oForm.Freeze(False)
                                    End Try
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

            End Select
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
#End Region

    Private Function validate() As Boolean
        Dim _retVal As Boolean = True
        oOfferGrid = oForm.Items.Item("6").Specific
        For index As Integer = 0 To oOfferGrid.DataTable.Rows.Count - 1
            If oOfferGrid.DataTable.GetValue("U_Z_Status", index) = "J" And oOfferGrid.DataTable.GetValue("U_Z_RejReason", index) = "" Then
                oApplication.Utilities.Message("Select Rejection Reason...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                _retVal = False
            End If
        Next
        Return _retVal
    End Function

    Private Sub enableControl(ByVal strHRStatus As String, ByVal strReqStatus As String)
        If strHRStatus = "Selected" Then
            oForm.Items.Item("3").Enabled = True
            oForm.Items.Item("13").Enabled = True
            oForm.Items.Item("6").Enabled = True
            oForm.Items.Item("11").Enabled = True
            oForm.Items.Item("12").Enabled = True
            'ElseIf (strWFStatus = "Y" Or (strReqStatus = "C" Or strReqStatus = "L")) Then
        ElseIf (strReqStatus = "C" Or strReqStatus = "L") Then
            oForm.Items.Item("3").Enabled = False
            oForm.Items.Item("13").Enabled = False
            oForm.Items.Item("6").Enabled = False
            oForm.Items.Item("11").Enabled = False
            oForm.Items.Item("12").Enabled = False
        Else
            oForm.Items.Item("3").Enabled = False
            oForm.Items.Item("13").Enabled = False
            oForm.Items.Item("6").Enabled = False
            oForm.Items.Item("11").Enabled = False
            oForm.Items.Item("12").Enabled = False
        End If
    End Sub

    Private Sub reDrawScreen(ByVal sboForm As SAPbouiCOM.Form)
        Try
            sboForm.Freeze(True)

            Dim intTop As Int16
            sboForm.Items.Item("1").Height = (sboForm.Height / 2) - 80
            sboForm.Items.Item("1").Width = (sboForm.Width) - 20

            intTop = sboForm.Items.Item("1").Top + sboForm.Items.Item("1").Height + 5
            sboForm.Items.Item("9").Top = intTop
            sboForm.Items.Item("8").Top = sboForm.Items.Item("8").Top

            intTop = sboForm.Items.Item("9").Top + sboForm.Items.Item("9").Height
            sboForm.Items.Item("2").Top = intTop - 1
            sboForm.Items.Item("2").Height = (sboForm.Height / 2) - 20
            sboForm.Items.Item("2").Width = (sboForm.Width) - 25


            sboForm.Items.Item("5").Top = intTop + 5
            sboForm.Items.Item("6").Top = intTop + 5
            sboForm.Items.Item("5").Height = sboForm.Items.Item("2").Height - 10
            sboForm.Items.Item("6").Height = sboForm.Items.Item("5").Height
            sboForm.Items.Item("5").Width = sboForm.Items.Item("2").Width - 20
            sboForm.Items.Item("6").Width = sboForm.Items.Item("5").Width

            oGrid = sboForm.Items.Item("1").Specific
            oGridDetail = oForm.Items.Item("5").Specific
            oOfferGrid = oForm.Items.Item("6").Specific
            oGrid.AutoResizeColumns()
            oGridDetail.AutoResizeColumns()
            oOfferGrid.AutoResizeColumns()

            sboForm.Freeze(False)
        Catch ex As Exception
            sboForm.Freeze(False)
        End Try
    End Sub

End Class


