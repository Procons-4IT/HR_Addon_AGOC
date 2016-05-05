Public Class clsApprovalTemplateSearch
    Inherits clsBase

#Region "Declarations"
    Public Shared ItemUID As String
    Public Shared SourceFormUID As String
    Public Shared SourceLabel As Integer
    Public Shared CFLChoice As String
    Public Shared ItemCode As String
    Public Shared sourceItemCode As String
    Public Shared choice As String
    Public Shared sourceColumID As String
    Public Shared sourcerowId As Integer
    Public Shared BinDescrUID As String
    Public Shared Documentchoice As String
    Dim oCombo As SAPbouiCOM.ComboBox

    Private oDbDatasource As SAPbouiCOM.DBDataSource
    Private Ouserdatasource As SAPbouiCOM.UserDataSource
    Private oConditions As SAPbouiCOM.Conditions
    Private ocondition As SAPbouiCOM.Condition
    Private intRowId As Integer
    Private strRowNum As Integer
    Private i As Integer
    Private oedit As SAPbouiCOM.EditText
    '   Private oForm As SAPbouiCOM.Form
    Private objSoureceForm As SAPbouiCOM.Form
    Private objform As SAPbouiCOM.Form
    Private oMatrix As SAPbouiCOM.Grid
    Private osourcegrid As SAPbouiCOM.Grid
    Private Const SEPRATOR As String = "~~~"
    Private SelectedRow As Integer
    Private sSearchColumn As String
    Private oItem As SAPbouiCOM.Item
    Public stritemid As SAPbouiCOM.Item
    Private intformmode As SAPbouiCOM.BoFormMode
    Private objGrid As SAPbouiCOM.Grid
    Private objSourcematrix As SAPbouiCOM.Matrix
    Private dtTemp As SAPbouiCOM.DataTable
    Private objStatic As SAPbouiCOM.StaticText
    Private inttable As Integer = 0
    Public strformid As String
    Public strStaticValue As String
    Public strSQL As String
    Private strSelectedItem1 As String
    Private strSelectedItem2 As String
    Private strSelectedItem3 As String
    Private strSelectedItem4 As String
    Private oGrid As SAPbouiCOM.Grid
    Private oRecSet As SAPbobsCOM.Recordset
    '   Private objSBOAPI As ClsSBO
    '   Dim objTransfer As clsTransfer
#End Region

#Region "New"
    '*****************************************************************
    'Type               : Constructor
    'Name               : New
    'Parameter          : 
    'Return Value       : 
    'Author             : DEV-2
    'Created Date       : 
    'Last Modified By   : 
    'Modified Date      : 
    'Purpose            : Create object for classes.
    '******************************************************************
    Public Sub New()
        '   objSBOAPI = New ClsSBO
        MyBase.New()
    End Sub
#End Region
    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_TemplateSearch) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_TemplateSearch, frm_TemplateSearch)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.DataSources.UserDataSources.Add("emp", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("Name", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oForm.DataSources.UserDataSources.Add("UName", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
        oedit = oForm.Items.Item("4").Specific
        oedit.DataBind.SetBound(True, "", "emp")
        oedit.ChooseFromListUID = "CFL_2"
        oedit.ChooseFromListAlias = "empID"
        oedit = oForm.Items.Item("10").Specific
        oedit.DataBind.SetBound(True, "", "Name")
        oedit = oForm.Items.Item("11").Specific
        oedit.DataBind.SetBound(True, "", "UName")
        AddChooseFromList(oForm)
        Databind(oForm)
    End Sub
#Region "Databind"
    Private Sub Databind(ByVal aform As SAPbouiCOM.Form)
        Try
            aform.Freeze(True)
            oGrid = aform.Items.Item("9").Specific
            dtTemp = oGrid.DataTable
            Dim str As String = "SELECT T0.[U_Z_Code], T0.[U_Z_Name], T0.[U_Z_DocType], T0.[U_Z_DocDesc], T0.[U_Z_LveType], T0.[U_Z_LveDesc], T2.[U_Z_AUser], T2.[U_Z_AName], T1.[U_Z_OUser], T1.[U_Z_OName], T1.[U_Z_EmpID] FROM [dbo].[@Z_HR_OAPPT]  T0 left outer Join  [dbo].[@Z_HR_APPT1]  T1 on T1.DocEntry=T0.DocEntry left outer Join  [dbo].[@Z_HR_APPT2]  T2 on T2.DocEntry=T0.DocEntry"
            str = str & "Where T0.DocEntry=0"
            str = "SELECT T0.[U_Z_Code] 'Code', T0.[U_Z_Name] 'Name', T0.[U_Z_DocDesc] 'Approval Type', T0.[U_Z_LveType] 'LeaveType', T0.[U_Z_LveDesc] 'Leave name', T2.[U_Z_AUser] 'Authorizer', T2.[U_Z_AName] 'Authorizer Name', T1.[U_Z_OUser] 'EmployeeID', T1.[U_Z_OName] 'Employee Name' ,T3.[U_Z_DeptCode], T3.[U_Z_DeptName]  from [@Z_HR_OAPPT]  T0 inner Join  [dbo].[@Z_HR_APPT1]  T1 on T1.DocEntry=T0.DocEntry left outer Join  [dbo].[@Z_HR_APPT2]  T2 on T2.DocEntry=T0.DocEntry left Outer Join [@Z_HR_APPT3] T3 on T3.DocEntry=T0.DocEntry where T0.U_Z_Code='xx'"
            dtTemp.ExecuteQuery(str)
            oGrid.DataTable = dtTemp
            oApplication.Utilities.assignMatrixLineno(oGrid, aform)
            oCombo = aform.Items.Item("8").Specific
            Dim oRec As SAPbobsCOM.Recordset
            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRec.DoQuery("SELECT Code,Remarks from OUDP")
            oCombo.ValidValues.Add("", "")
            For intRow As Integer = 0 To oRec.RecordCount - 1
                oCombo.ValidValues.Add(oRec.Fields.Item(0).Value, oRec.Fields.Item(1).Value)
                oRec.MoveNext()
            Next
            aform.Items.Item("9").DisplayDesc = True
            oCombo.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
            oCombo.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)
        End Try
    End Sub
#End Region

#Region "Add Choose From List"
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
            oCFL = oCFLs.Item("CFL_2")

            oCons = oCFL.GetConditions()
            oCon = oCons.Add()
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()

            

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

#End Region
#Region "Bind Data"
    '******************************************************************
    'Type               : Procedure
    'Name               : BindData
    'Parameter          : Form
    'Return Value       : 
    'Author             : DEV-2
    'Created Date       : 
    'Last Modified By   : 
    'Modified Date      : 
    'Purpose            : Binding the fields.
    '******************************************************************
    Public Sub databound(ByVal objform As SAPbouiCOM.Form)
        Try
            Dim strSQL As String = ""
            Dim ObjSegRecSet As SAPbobsCOM.Recordset
            ObjSegRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            objform.Freeze(True)
            ' objform.DataSources.DataTables.Add("dtLevel3")
            AddChooseFromList(objform)
            Ouserdatasource = objform.DataSources.UserDataSources.Add("dbFind", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            Ouserdatasource = objform.DataSources.UserDataSources.Add("dbFind1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            Ouserdatasource = objform.DataSources.UserDataSources.Add("dbFind2", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            Ouserdatasource = objform.DataSources.UserDataSources.Add("dbFind3", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            Ouserdatasource = objform.DataSources.UserDataSources.Add("dbFind4", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)

            Dim ststring As String()
            ststring = strStaticValue.Split(";")

            oedit = objform.Items.Item("ed1").Specific
            oedit.DataBind.SetBound(True, "", "dbFind")
            oedit.ChooseFromListUID = "CFL_2"
            oedit.ChooseFromListAlias = "OcrCode"
            oedit = objform.Items.Item("ed2").Specific
            oedit.DataBind.SetBound(True, "", "dbFind1")
            oedit.ChooseFromListUID = "CFL_3"
            oedit.ChooseFromListAlias = "OcrCode"
            oedit = objform.Items.Item("ed3").Specific
            oedit.DataBind.SetBound(True, "", "dbFind2")
            oedit.ChooseFromListUID = "CFL_4"
            oedit.ChooseFromListAlias = "OcrCode"
            oedit = objform.Items.Item("ed4").Specific
            oedit.DataBind.SetBound(True, "", "dbFind3")
            oedit.ChooseFromListUID = "CFL_5"
            oedit.ChooseFromListAlias = "OcrCode"
            oedit = objform.Items.Item("ed5").Specific
            oedit.DataBind.SetBound(True, "", "dbFind4")
            oedit.ChooseFromListUID = "CFL_6"
            oedit.ChooseFromListAlias = "OcrCode"
            Dim oTest As SAPbobsCOM.Recordset
            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTest.DoQuery("Select * from ODIM order by DimCode")
            Dim ost As SAPbouiCOM.StaticText
            For intRow As Integer = 0 To oTest.RecordCount - 1
                If oTest.Fields.Item("DimActive").Value = "Y" Then
                    objform.Items.Item("ed" & intRow + 1).Visible = True
                    objform.Items.Item("st" & intRow + 1).Visible = True

                    ost = objform.Items.Item("st" & intRow + 1).Specific
                    ost.Caption = oTest.Fields.Item("DimDesc").Value
                    objform.Items.Item("ed" & intRow + 1).Enabled = True
                    Try
                        oApplication.Utilities.setEdittextvalue(objform, "ed" & intRow + 1, ststring(intRow))
                    Catch ex As Exception
                    End Try
                Else
                    objform.Items.Item("ed" & intRow + 1).Visible = False
                    objform.Items.Item("st" & intRow + 1).Visible = False
                End If
                objform.Items.Item("ed6").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                If intRow < 3 Then
                    objform.Items.Item("ed" & intRow + 1).Visible = False
                    objform.Items.Item("st" & intRow + 1).Visible = False
                End If
                oTest.MoveNext()
            Next
            If strformid = "Approved" Then
                objform.Items.Item("3").Enabled = False
            Else
                objform.Items.Item("3").Enabled = True
            End If
            objform.Freeze(False)

        Catch ex As Exception
            oApplication.SBO_Application.MessageBox(ex.Message)
            oApplication.SBO_Application.MessageBox(ex.Message)
        Finally
        End Try
    End Sub
#End Region

#Region "Update On hand Qty"
    Private Sub FillOnhandqty(ByVal strItemcode As String, ByVal strwhs As String, ByVal aGrid As SAPbouiCOM.Grid)
        Dim oTemprec As SAPbobsCOM.Recordset
        oTemprec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim strBin, strSql As String
        For intRow As Integer = 0 To aGrid.DataTable.Rows.Count - 1
            strBin = aGrid.DataTable.GetValue(0, intRow)
            strSql = "Select isnull(Sum(U_InQty)-sum(U_OutQty),0) from [@DABT_BTRN] where U_Itemcode='" & strItemcode & "' and U_FrmWhs='" & strwhs & "' and U_BinCode='" & strBin & "'"
            oTemprec.DoQuery(strSql)
            Dim dblOnhand As Double
            dblOnhand = oTemprec.Fields.Item(0).Value

            aGrid.DataTable.SetValue(2, intRow, dblOnhand.ToString)
        Next
    End Sub
#End Region

#Region "Get Form"
    '******************************************************************
    'Type               : Function
    'Name               : GetForm
    'Parameter          : FormUID
    'Return Value       : 
    'Author             : DEV-2
    'Created Date       : 
    'Last Modified By   : 
    'Modified Date      : 
    'Purpose            : Get The Form
    '******************************************************************
    Public Function GetForm(ByVal FormUID As String) As SAPbouiCOM.Form
        Return oApplication.SBO_Application.Forms.Item(FormUID)
    End Function
#End Region

#Region "FormDataEvent"


#End Region

#Region "Class Menu Event"
    Public Overrides Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            Select Case pVal.MenuUID
                Case mnu_TemplateSearch
                    LoadForm()
                Case mnu_ADD_ROW

                Case mnu_DELETE_ROW
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
            End Select
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
#End Region

#Region "getBOQReference"
    Private Function getBOQReference(ByVal aItemCode As String, ByVal aProject As String, ByVal aProcess As String, ByVal aActivity As String) As String
        Dim oTest As SAPbobsCOM.Recordset
        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTest.DoQuery("Select isnull(U_Z_BOQREF,'') from [@Z_PRJ2] where U_Z_ItemCode='" & aItemCode & "' and  U_Z_PRJCODE='" & aProject.Replace("'", "''") & "' and U_Z_MODNAME='" & aProcess.Replace("'", "''") & "' and U_Z_ACTNAME='" & aActivity.Replace("'", "''") & "'")
        Return oTest.Fields.Item(0).Value
    End Function
#End Region

#Region "Search"
    Private Sub Search(aForm As SAPbouiCOM.Form)
        Dim strEmp, strUser, strDept As String
        strEmp = oApplication.Utilities.getEdittextvalue(aForm, "4")
        strUser = oApplication.Utilities.getEdittextvalue(aForm, "6")
        oCombo = aForm.Items.Item("8").Specific
        strDept = oCombo.Selected.Value

        If strEmp <> "" And strDept <> "" Then
            oApplication.Utilities.Message("You can search template by either Employee wise  or Department wise", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        Dim str As String
        str = "SELECT T0.[U_Z_Code] 'Code', T0.[U_Z_Name] 'Name', T0.[U_Z_DocDesc] 'Approval Type', T0.[U_Z_LveType] 'LeaveType', T0.[U_Z_LveDesc] 'Leave name', T2.[U_Z_AUser] 'Authorizer', T2.[U_Z_AName] 'Authorizer Name', T1.[U_Z_OUser] 'EmployeeID', T1.[U_Z_OName] 'Employee Name' ,T3.[U_Z_DeptCode] 'Department Code', T3.[U_Z_DeptName] 'Department Name'  from [@Z_HR_OAPPT]  T0 inner Join  [dbo].[@Z_HR_APPT1]  T1 on T1.DocEntry=T0.DocEntry left outer Join  [dbo].[@Z_HR_APPT2]  T2 on T2.DocEntry=T0.DocEntry left Outer Join [@Z_HR_APPT3] T3 on T3.DocEntry=T0.DocEntry where 1=1"

        If strEmp <> "" Then
            str = "SELECT T0.[U_Z_Code] 'Code', T0.[U_Z_Name] 'Name', T0.[U_Z_DocDesc] 'Approval Type', T0.[U_Z_LveType] 'LeaveType', T0.[U_Z_LveDesc] 'Leave name', T2.[U_Z_AUser] 'Authorizer', T2.[U_Z_AName] 'Authorizer Name', T1.[U_Z_OUser] 'EmployeeID', T1.[U_Z_OName] 'Employee Name'   from [@Z_HR_OAPPT]  T0 inner Join  [@Z_HR_APPT1]  T1 on T1.DocEntry=T0.DocEntry Inner Join  [dbo].[@Z_HR_APPT2]  T2 on T2.DocEntry=T0.DocEntry  where T1.U_Z_OUser='" & strEmp & "'"
        End If

        If strDept <> "" Then
            str = "SELECT T0.[U_Z_Code] 'Code', T0.[U_Z_Name] 'Name', T0.[U_Z_DocDesc] 'Approval Type', T0.[U_Z_LveType] 'LeaveType', T0.[U_Z_LveDesc] 'Leave name', T2.[U_Z_AUser] 'Authorizer', T2.[U_Z_AName] 'Authorizer Name' ,T3.[U_Z_DeptCode] 'Department Code', T3.[U_Z_DeptName] 'Department Name'  from [@Z_HR_OAPPT]  T0 inner Join  [@Z_HR_APPT2]  T2 on T2.DocEntry=T0.DocEntry Inner Join [@Z_HR_APPT3] T3 on T3.DocEntry=T0.DocEntry where T3.U_Z_DeptCode='" & strDept & "'"
        End If
        If strUser <> "" Then
            str = str & " and T2.U_Z_AUser='" & strUser & "'"
        End If
        oGrid = aForm.Items.Item("9").Specific
        oGrid.DataTable.ExecuteQuery(str)
        oGrid.Columns.Item("Code").TitleObject.Caption = "Template Code"
        Dim oEditColumn As SAPbouiCOM.EditTextColumn
        oEditColumn = oGrid.Columns.Item("Code")
        oEditColumn.LinkedObjectType = "2"
        If strDept = "" And strEmp = "" Then
            oEditColumn = oGrid.Columns.Item("EmployeeID")
            oEditColumn.LinkedObjectType = "171"
        ElseIf strDept = "" And strEmp <> "" Then
            oEditColumn = oGrid.Columns.Item("EmployeeID")
            oEditColumn.LinkedObjectType = "171"
        ElseIf strDept = "" And strEmp <> "" Then
        End If
        oEditColumn = oGrid.Columns.Item("Authorizer")
        oEditColumn.LinkedObjectType = "12"
       
        oGrid.AutoResizeColumns()
        oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None




    End Sub
#End Region

#Region "Item Event"

    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        BubbleEvent = True
        If pVal.FormTypeEx = frm_TemplateSearch Then


            Select Case pVal.BeforeAction
                Case True
                    Select Case pVal.EventType
                        Case SAPbouiCOM.BoEventTypes.et_MATRIX_LINK_PRESSED
                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            If pVal.ItemUID = "9" And pVal.ColUID = "Code" Then
                                Dim oObj As New clshrApproveTemp
                                oGrid = oForm.Items.Item("9").Specific
                                oObj.ViewForm(oGrid.DataTable.GetValue("Code", pVal.Row))
                                BubbleEvent = False
                                Exit Sub
                            End If
                    End Select
                Case False
                    Select Case pVal.EventType
                        Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                            If pVal.ItemUID = "3" Then
                                Search(oForm)
                            End If
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
                                    If pVal.ItemUID = "4" Then
                                        val1 = oDataTable.GetValue("firstName", 0) & " " & oDataTable.GetValue("middleName", 0) & " " & oDataTable.GetValue("lastName", 0)
                                        oApplication.Utilities.setEdittextvalue(oForm, "10", val1)
                                        oApplication.Utilities.setEdittextvalue(oForm, pVal.ItemUID, oDataTable.GetValue("empID", 0))
                                    End If
                                    If pVal.ItemUID = "6" Then
                                        oApplication.Utilities.setEdittextvalue(oForm, "11", oDataTable.GetValue("U_NAME", 0))
                                        oApplication.Utilities.setEdittextvalue(oForm, pVal.ItemUID, oDataTable.GetValue("USER_CODE", 0))
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
    End Sub
#End Region
End Class
