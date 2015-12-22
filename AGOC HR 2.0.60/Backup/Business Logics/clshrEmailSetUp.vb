Public Class clshrEmailSetUp
    Inherits clsBase
    Private objForm As SAPbouiCOM.Form
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem As SAPbobsCOM.Items
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Private oDBDataSrc As SAPbouiCOM.DBDataSource
    Private oRecordSet As SAPbobsCOM.Recordset
    Private sQuery As String

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub

    Private Sub LoadForm()
        If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, frm_hr_EmailSetUp) = False Then
            oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Exit Sub
        End If
        oForm = oApplication.Utilities.LoadForm(xml_hr_EmailSetUp, frm_hr_EmailSetUp)
        oForm = oApplication.SBO_Application.Forms.ActiveForm()
        oForm.Freeze(True)
        BindData(oForm)
        oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE
        oForm.Freeze(False)
    End Sub

#Region "DataBind"

    Public Sub BindData(ByVal objform As SAPbouiCOM.Form)
        Try
            oDBDataSrc = objform.DataSources.DBDataSources.Item(0)
            Dim oColum As SAPbouiCOM.ComboBox
            oColum = oForm.Items.Item("12").Specific
            Try
                oColum.ValidValues.Add("True", "True")
                oColum.ValidValues.Add("False", "False")
            Catch ex As Exception

            End Try
            oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            sQuery = "Select Code From [@Z_HR_OMAIL]"
            oRecordSet.DoQuery(sQuery)

            If Not oRecordSet.EoF Then
                oDBDataSrc.Query()
                oForm.Update()
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
#Region "Save Email SetUp"
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
    Private Sub saveEmailSetUp(ByVal objApplication As SAPbouiCOM.Application, ByVal ocompany As SAPbobsCOM.Company, ByVal oForm As SAPbouiCOM.Form)
        Dim oMatrix As SAPbouiCOM.Matrix
        Dim oDBDSource As SAPbouiCOM.DBDataSource
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim lnErrCode As Long
        Dim strErrMsg As String
        Dim i As Integer
        Try
            oForm.Freeze(True)
            oUserTable = ocompany.UserTables.Item("Z_HR_OMAIL")
            oDBDSource = oForm.DataSources.DBDataSources.Item("@Z_HR_OMAIL")
            If oUserTable.GetByKey(oDBDSource.GetValue("Code", i).Trim) Then
                'oUserTable.Name = oDBDSource.GetValue("Name", i)
                'oUserTable.Code = oDBDSource.GetValue("Code", i)
                oUserTable.UserFields.Fields.Item("U_Z_SMTPSERV").Value = oDBDSource.GetValue("U_Z_SMTPSERV", i)
                oUserTable.UserFields.Fields.Item("U_Z_SMTPPORT").Value = oDBDSource.GetValue("U_Z_SMTPPORT", i)
                oUserTable.UserFields.Fields.Item("U_Z_SMTPUSER").Value = oDBDSource.GetValue("U_Z_SMTPUSER", i)
                oUserTable.UserFields.Fields.Item("U_Z_SMTPPWD").Value = oDBDSource.GetValue("U_Z_SMTPPWD", i)
                oUserTable.UserFields.Fields.Item("U_Z_SSL").Value = oDBDSource.GetValue("U_Z_SSL", i)
                If oUserTable.Update <> 0 Then
                    MsgBox(ocompany.GetLastErrorDescription)
                End If
            Else
                Dim strCode As Int32 = "1"
                If oDBDSource.GetValue("Code", 0).ToString = "" Then
                    oUserTable.Code = strCode.ToString("00000000")
                    oUserTable.Name = strCode.ToString("00000000")
                End If
                oUserTable.UserFields.Fields.Item("U_Z_SMTPSERV").Value = oDBDSource.GetValue("U_Z_SMTPSERV", i)
                oUserTable.UserFields.Fields.Item("U_Z_SMTPPORT").Value = oDBDSource.GetValue("U_Z_SMTPPORT", i)
                oUserTable.UserFields.Fields.Item("U_Z_SMTPUSER").Value = oDBDSource.GetValue("U_Z_SMTPUSER", i)
                oUserTable.UserFields.Fields.Item("U_Z_SMTPPWD").Value = oDBDSource.GetValue("U_Z_SMTPPWD", i)
                oUserTable.UserFields.Fields.Item("U_Z_SSL").Value = oDBDSource.GetValue("U_Z_SSL", i)
                If oUserTable.Add() <> 0 Then
                    MsgBox(ocompany.GetLastErrorDescription)
                End If
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



    Private Function Validation(ByVal aForm As SAPbouiCOM.Form) As Boolean
       
        Return True
    End Function


#End Region

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_EmailSetUp Then
                Select Case pVal.BeforeAction
                    Case True
                        If pVal.EventType = SAPbouiCOM.BoEventTypes.et_KEY_DOWN And pVal.CharPressed <> "9" And pVal.ItemUID = "3" And pVal.ColUID = "V_0" Then
                            Dim strVal As String
                            oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                        End If
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "1" And oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                    oForm.Freeze(True)
                                    If Validation(oForm) = False Then
                                        oForm.Freeze(False)
                                        BubbleEvent = False
                                        Exit Sub
                                    Else
                                        saveEmailSetUp(oApplication.SBO_Application, oApplication.Company, oForm)
                                    End If
                                    oForm.Freeze(False)
                                End If
                        End Select

                    Case False

                End Select
            End If


        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm = oApplication.SBO_Application.Forms.Item(FormUID)
            oForm.Freeze(False)
        End Try
    End Sub
#End Region

#Region "Menu Event"
    Public Overrides Sub MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean)
        Try
            Select Case pVal.MenuUID
                Case mnu_hr_EmailSetUp
                    LoadForm()
                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
            End Select
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub
#End Region

End Class
