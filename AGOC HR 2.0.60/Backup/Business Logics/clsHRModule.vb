Public Class clsHRModule
    Inherits clsBase
    Private oCFLEvent As SAPbouiCOM.IChooseFromListEvent
    Private oDBSrc_Line As SAPbouiCOM.DBDataSource
    Private oMatrix As SAPbouiCOM.Matrix
    Private ostatic As SAPbouiCOM.StaticText
    Private oItem As SAPbouiCOM.Item
    Private ofolder As SAPbouiCOM.Folder
    Private oEditText As SAPbouiCOM.EditText
    Private oCombobox As SAPbouiCOM.ComboBox
    Private oComboColumn As SAPbouiCOM.ComboBoxColumn
    Private oEditTextColumn As SAPbouiCOM.EditTextColumn
    Private oCheckBox As SAPbouiCOM.CheckBox

    Private oGrid As SAPbouiCOM.Grid
    Private dtTemp As SAPbouiCOM.DataTable
    Private dtResult As SAPbouiCOM.DataTable
    Private oMode As SAPbouiCOM.BoFormMode
    Private oItem1 As SAPbobsCOM.Items
    Private oInvoice As SAPbobsCOM.Documents
    Private InvBase As DocumentType
    Private InvBaseDocNo As String
    Private InvForConsumedItems As Integer
    Private blnFlag As Boolean = False
    Private strEmpID As String
    Private strQuery As String
    Dim oFItem As SAPbouiCOM.Item
    Dim oRecSet As SAPbobsCOM.Recordset

    Public Sub New()
        MyBase.New()
        InvForConsumedItems = 0
    End Sub

#Region "AddControls"
    Private Function AddControls(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            aForm.Freeze(True)
            AddChooseFromList(aForm)

            oApplication.Utilities.AddControls(aForm, "stHRTitle", "22", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 1, 1, , "Emergency Contact Info", 200)
            aForm.Items.Item("stHRTitle").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
            oApplication.Utilities.AddControls(aForm, "stHRTitle1", "57", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 1, 1, , , )

            oApplication.Utilities.AddControls(aForm, "stHRRest", "stHRTitle", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 1, 1, , "Relation Name", 200)
            oApplication.Utilities.AddControls(aForm, "edHRReNa", "stHRTitle1", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 1, 1)
            oEditText = aForm.Items.Item("edHRReNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Rel_Name")
            oItem = aForm.Items.Item("stHRRest")
            oItem.LinkTo = "edHRReNa"

            oApplication.Utilities.AddControls(aForm, "stHRRTSt", "stHRRest", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 1, 1, , "Relationship Type")
            oApplication.Utilities.AddControls(aForm, "edHRRtNa", "edHRReNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 1, 1)
            oEditText = aForm.Items.Item("edHRRtNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Rel_Type")
            oItem = aForm.Items.Item("stHRRTSt")
            oItem.LinkTo = "edHRRtNa"

            oApplication.Utilities.AddControls(aForm, "stHRRPht", "stHRRTSt", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 1, 1, , "Contact Number")
            oApplication.Utilities.AddControls(aForm, "edHRRPNa", "edHRRtNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 1, 1)
            oEditText = aForm.Items.Item("edHRRPNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Rel_Phone")
            oItem = aForm.Items.Item("stHRRPht")
            oItem.LinkTo = "edHRRPNa"


            oApplication.Utilities.AddControls(aForm, "stPro", "125", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 1, 1, , "Probation Period", 200)
            aForm.Items.Item("stPro").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
            oApplication.Utilities.AddControls(aForm, "stPro1", "118", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 1, 1, , , )

            oApplication.Utilities.AddControls(aForm, "stPra", "stPro", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 1, 1, , "Probation Period", , )
            oApplication.Utilities.AddControls(aForm, "edPra", "stPro1", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 1, 1, , , , )
            oEditText = aForm.Items.Item("edPra").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Promonth")
            aForm.Items.Item("edPra").DisplayDesc = True

            oItem = aForm.Items.Item("stPra")
            oItem.LinkTo = "edPra"

            oApplication.Utilities.AddControls(aForm, "stPraDt", "stPra", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 1, 1, , "Probation Period Date", , )
            oApplication.Utilities.AddControls(aForm, "edPraDt", "edPra", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 1, 1, , , , )
            oEditText = aForm.Items.Item("edPraDt").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Prodate")
            aForm.Items.Item("edPraDt").DisplayDesc = True

            oItem = aForm.Items.Item("stPraDt")
            oItem.LinkTo = "edPraDt"

            Try
                oApplication.Utilities.AddControls(aForm, "fldHR", "fldPay", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 0, 0, "fldPay", "HR Details")
            Catch ex As Exception
                oApplication.Utilities.AddControls(aForm, "fldHR", "143", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 0, 0, "143", "HR Details")
            End Try

            Dim oldItem As SAPbouiCOM.Item
            oItem = aForm.Items.Item("fldHR")
            oldItem = aForm.Items.Item("26")
            oItem.AffectsFormMode = False
            ofolder = aForm.Items.Item("fldHR").Specific
            '  aForm.DataSources.UserDataSources.Add("HR1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            aForm.Items.Add("HRFldHr1", SAPbouiCOM.BoFormItemTypes.it_FOLDER)
            oldItem = aForm.Items.Item("26")
            oItem = aForm.Items.Item("HRFldHr1")
            oItem.Top = oldItem.Top + 25
            oItem.Left = oldItem.Left + 5
            oItem.Width = 250
            oItem.Height = oldItem.Height
            oItem.FromPane = 22 '18
            oItem.ToPane = 30 '22
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ' ofolder.GroupWith("143")
            ofolder.ValOn = "K"
            ofolder.ValOff = "M"
            ofolder.Caption = "Orgainization Details"
            '  aForm.DataSources.UserDataSources.Add("Acc1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            ofolder.DataBind.SetBound(True, "OHEM", "U_Fld")


            'oApplication.Utilities.AddControls(aForm, "HRFldHr1", "27", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 0, 20, "27", "Orgainization Details", 200)
            '' aForm.DataSources.DataTables.Add("dtCon")
            'oItem = aForm.Items.Item("HRFldHr1")
            'oItem.AffectsFormMode = False
            'ofolder = oItem.Specific
            'ofolder.GroupWith("27")
            'ofolder.ValOn = "K"
            'ofolder.ValOff = "M"


            oApplication.Utilities.AddControls(aForm, "HRFldHr2", "HRFldHr1", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 22, 30, "HRFldHr1", "HR 2nd Language details")
            ' aForm.DataSources.DataTables.Add("dtCon")
            oItem = aForm.Items.Item("HRFldHr2")
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ofolder.GroupWith("HRFldHr1")
            ofolder.ValOn = "O"
            ofolder.ValOff = "M"

            oApplication.Utilities.AddControls(aForm, "HRFldHr3", "HRFldHr2", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 18, 30, "HRFldHr2", "People Objectives")
            ' aForm.DataSources.DataTables.Add("dtCon")
            oItem = aForm.Items.Item("HRFldHr3")
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ofolder.GroupWith("HRFldHr2")
            ofolder.ValOn = "A"
            ofolder.ValOff = "B"

            oApplication.Utilities.AddControls(aForm, "HRFldHr4", "HRFldHr3", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 22, 30, "HRFldHr3", "Competencies")
            ' aForm.DataSources.DataTables.Add("dtCon")
            oItem = aForm.Items.Item("HRFldHr4")
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ofolder.GroupWith("HRFldHr3")
            ofolder.ValOn = "C"
            ofolder.ValOff = "Y"

            oApplication.Utilities.AddControls(aForm, "HRFldHr5", "HRFldHr4", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 22, 30, "HRFldHr4", "Fixed Assets")
            ' aForm.DataSources.DataTables.Add("dtCon")
            oItem = aForm.Items.Item("HRFldHr5")
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ofolder.GroupWith("HRFldHr4")
            ofolder.ValOn = "D"
            ofolder.ValOff = "E"



            'oApplication.Utilities.AddControls(aForm, "HRFldHr3", "HRFldHr1", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 0, 20, "HRFldHr1", "HR 2nd Languages")
            'oItem = aForm.Items.Item("HRFldHr3")
            'oItem.AffectsFormMode = False
            'ofolder = oItem.Specific
            'ofolder.GroupWith("HRFldHr1")



            ' oItem.Visible = False
            'oApplication.Utilities.AddControls(aForm, "HRFldHr2", "HRFldHr3", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 0, 20, "HRFldHr3", "People Objectives")
            ' aForm.DataSources.DataTables.Add("dtCon")
            'oItem = aForm.Items.Item("HRFldHr2")
            'oItem.AffectsFormMode = False
            'ofolder = oItem.Specific
            'ofolder.GroupWith("HRFldHr3")
            'ofolder.ValOn = "L"
            'ofolder.ValOff = "N"



            'oApplication.Utilities.AddControls(aForm, "stcomp", "93", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 31, 31, , "Company Code", 120)
            'oApplication.Utilities.AddControls(aForm, "edComp", "84", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 31, 31, , , 120)
            'oApplication.Utilities.AddControls(aForm, "stcoNa", "edComp", SAPbouiCOM.BoFormItemTypes.it_STATIC, "RIGHT", 31, 31, , "Company Name", 120)
            'oApplication.Utilities.AddControls(aForm, "edCoNa", "stcoNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "RIGHT", 31, 31, , , 120)
            'oEditText = aForm.Items.Item("edComp").Specific
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_CompCode")
            'oEditText.ChooseFromListUID = "CFL1"
            'oEditText.ChooseFromListAlias = "U_Z_CompCode"
            'oEditText = aForm.Items.Item("edCoNa").Specific
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_CompName")
            'aForm.Items.Item("edComp").Enabled = True
            'aForm.Items.Item("edCoNa").Enabled = False
            'oItem = aForm.Items.Item("stcomp")
            'oItem.LinkTo = "edComp"
            'oItem = aForm.Items.Item("stcoNa")
            'oItem.LinkTo = "edCoNa"

            'Try
            '    oApplication.Utilities.AddControls(aForm, "HRstthid", "480002077", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "Third Name", 80)
            '    oApplication.Utilities.AddControls(aForm, "HRedthId", "480002078", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , 90)
            'Catch ex As Exception
            '    oApplication.Utilities.AddControls(aForm, "HRstthid", "3", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "Third Name", 80)
            '    oApplication.Utilities.AddControls(aForm, "HRedthId", "33", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , 90)

            'End Try

            Try
                'HRstthid
                '  oApplication.Utilities.AddControls(aForm, "stTA", "HRstthid", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "T&A employee ID", , )
                ' oApplication.Utilities.AddControls(aForm, "edTA", "HRedthId", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , )
                oApplication.Utilities.AddControls(aForm, "HRstthid", "stTA", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "Third Name", 80)
                oApplication.Utilities.AddControls(aForm, "HRedthId", "edTA", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , 90)

            Catch ex As Exception

                Try
                    oApplication.Utilities.AddControls(aForm, "HRstthid", "480002077", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "Third Name", 130)
                    oApplication.Utilities.AddControls(aForm, "HRedthId", "480002078", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , 90)
                Catch ex1 As Exception
                    oApplication.Utilities.AddControls(aForm, "HRstthid", "3", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "Third Name", 120)
                    oApplication.Utilities.AddControls(aForm, "HRedthId", "33", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , 90)
                End Try

                '  oApplication.Utilities.AddControls(aForm, "stTA", "3", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "T&A employee ID", , )
                ' oApplication.Utilities.AddControls(aForm, "edTA", "33", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , )

            End Try

            oEditText = aForm.Items.Item("HRedthId").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_ThirdName")
            oItem = aForm.Items.Item("HRstthid")
            oItem.LinkTo = "HRedthId"



            'oApplication.Utilities.AddControls(aForm, "HRstthid", "3", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "Third Name", 80)
            'oApplication.Utilities.AddControls(aForm, "HRedthId", "33", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , 90)
            'oEditText = aForm.Items.Item("HRedthId").Specific
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_ThirdName")
            'oItem = aForm.Items.Item("HRstthid")
            'oItem.LinkTo = "HRedthId"
            '' oItem.Left = 310

            'oApplication.Utilities.AddControls(aForm, "HRstAppid", "HRstthid", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "Applicant Id", 80)
            'oApplication.Utilities.AddControls(aForm, "HRedAppId", "HRedthId", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , 90)
            'oApplication.Utilities.AddControls(aForm, "HRstAppLk", "HRedAppId", SAPbouiCOM.BoFormItemTypes.it_LINKED_BUTTON, "DOWN", 0, 0, , , 20)
            'oEditText = aForm.Items.Item("HRedAppId").Specific
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_ApplId")
            'oItem = aForm.Items.Item("HRstAppid")
            'oItem.LinkTo = "HRstAppLk"
            'oItem = aForm.Items.Item("HRstAppLk")
            'oItem.LinkTo = "HRedAppId"
            ' oItem.Left = 310


            oApplication.Utilities.AddControls(aForm, "HRstFna", "93", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 23, 23, , "First Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedFna", "84", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 23, 23, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstSeNa", "HRedFna", SAPbouiCOM.BoFormItemTypes.it_STATIC, "RIGHT", 23, 23, , "Second Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedSeNa", "HRstSeNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "RIGHT", 23, 23, , , 120)
            oEditText = aForm.Items.Item("HRedFna").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_AFirstName")
            oEditText = aForm.Items.Item("HRedSeNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_ASecondName")

            oItem = aForm.Items.Item("HRstFna")
            oItem.LinkTo = "HRedFna"
            oItem = aForm.Items.Item("HRstSeNa")
            oItem.LinkTo = "HRedSeNa"

            oApplication.Utilities.AddControls(aForm, "HRstTna", "HRstFna", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 23, 23, , "Third Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedTna", "HRedFna", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 23, 23, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstLaNa", "HRedTna", SAPbouiCOM.BoFormItemTypes.it_STATIC, "RIGHT", 23, 23, , "Last Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedLaNa", "HRstLaNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "RIGHT", 23, 23, , , 120)
            oEditText = aForm.Items.Item("HRedTna").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_AThirdName")
            oEditText = aForm.Items.Item("HRedLaNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_ALastName")

            oItem = aForm.Items.Item("HRstTna")
            oItem.LinkTo = "HRedTna"
            oItem = aForm.Items.Item("HRstLaNa")
            oItem.LinkTo = "HRedLaNa"

            oApplication.Utilities.AddControls(aForm, "HRstCNa", "HRstTna", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 23, 23, , "Company Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedCNa", "HRedTna", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 23, 23, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstDNa", "HRstLaNa", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 23, 23, , "Department Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedDNa", "HRedLaNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 23, 23, , , 120)
            oEditText = aForm.Items.Item("HRedCNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_ACmpName")
            oEditText = aForm.Items.Item("HRedDNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_ADeptName")

            oItem = aForm.Items.Item("HRstCNa")
            oItem.LinkTo = "HRedCNa"
            oItem = aForm.Items.Item("HRstDNa")
            oItem.LinkTo = "HRedDNa"



            oApplication.Utilities.AddControls(aForm, "HRstposi", "93", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 118, 118, , "Position Code", 120)
            oApplication.Utilities.AddControls(aForm, "HRedposi", "84", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 118, 118, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstpoNa", "HRedposi", SAPbouiCOM.BoFormItemTypes.it_STATIC, "RIGHT", 118, 118, , "Position Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedpoNa", "HRstpoNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "RIGHT", 118, 118, , , 120)
            oEditText = aForm.Items.Item("HRedposi").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_PosiCode")
            oEditText.ChooseFromListUID = "CFL_HR_4"
            oEditText.ChooseFromListAlias = "U_Z_PosCode"
            oEditText = aForm.Items.Item("HRedpoNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_PosiName")

            aForm.Items.Item("HRedposi").Enabled = True
            aForm.Items.Item("HRedpoNa").Enabled = False
            oItem = aForm.Items.Item("HRstposi")
            oItem.LinkTo = "HRedposi"
            oItem = aForm.Items.Item("HRstpoNa")
            oItem.LinkTo = "HRedpoNa"


            oApplication.Utilities.AddControls(aForm, "HRstcomp", "93", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 22, 22, , "Company Code (*)", 120)
            oApplication.Utilities.AddControls(aForm, "HRedComp", "84", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 22, 22, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstcoNa", "HRedComp", SAPbouiCOM.BoFormItemTypes.it_STATIC, "RIGHT", 22, 22, , "Company Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedCoNa", "HRstcoNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "RIGHT", 22, 22, , , 120)
            oEditText = aForm.Items.Item("HRedComp").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_CompCode")
            oEditText.ChooseFromListUID = "CFL_HR_1"
            oEditText.ChooseFromListAlias = "U_Z_CompCode"
            oEditText = aForm.Items.Item("HRedCoNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_CompName")
            aForm.Items.Item("HRedComp").Enabled = True
            aForm.Items.Item("HRedCoNa").Enabled = False
            oItem = aForm.Items.Item("HRstcomp")
            oItem.LinkTo = "HRedComp"
            oItem = aForm.Items.Item("HRstcoNa")
            oItem.LinkTo = "HRedCoNa"


            oApplication.Utilities.AddControls(aForm, "HRstDiv", "HRstcomp", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 22, 22, , "Division Code (*)", 120)
            oApplication.Utilities.AddControls(aForm, "HRedDiv", "HRedComp", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 22, 22, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstDiNa", "HRstcoNa", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 22, 22, , "Division Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedDivNa", "HRedCoNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 22, 22, , , 120)
            oEditText = aForm.Items.Item("HRedDiv").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_DivCode")
            'oEditText.ChooseFromListUID = "CFL_HR_1"
            'oEditText.ChooseFromListAlias = "U_Z_CompCode"
            oEditText = aForm.Items.Item("HRedDivNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_DivName")
            aForm.Items.Item("HRedDiv").Enabled = True
            aForm.Items.Item("HRedDivNa").Enabled = False
            oItem = aForm.Items.Item("HRstDiv")
            oItem.LinkTo = "HRedDiv"
            oItem = aForm.Items.Item("HRstDiNa")
            oItem.LinkTo = "HRedDivNa"




            oApplication.Utilities.AddControls(aForm, "HRstOrgSt", "HRstDiv", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 22, 22, , "Organization Code (*)", 120)
            oApplication.Utilities.AddControls(aForm, "HRedOrgSt", "HRedDiv", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 22, 22, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstOrgNa", "HRstDiNa", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 22, 22, , "Organization Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedOrgNa", "HRedDivNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 22, 22, , , 120)
            oEditText = aForm.Items.Item("HRedOrgSt").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_OrgstCode")
            oEditText.ChooseFromListUID = "CFL_HR_2"
            oEditText.ChooseFromListAlias = "U_Z_OrgCode"
            oEditText = aForm.Items.Item("HRedOrgNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_OrgstName")

            aForm.Items.Item("HRedOrgSt").Enabled = False
            aForm.Items.Item("HRedOrgNa").Enabled = False
            oItem = aForm.Items.Item("HRstOrgSt")
            oItem.LinkTo = "HRedOrgSt"
            oItem = aForm.Items.Item("HRstOrgNa")
            oItem.LinkTo = "HRedOrgNa"

            oApplication.Utilities.AddControls(aForm, "HRstJobSt", "HRstOrgSt", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 22, 22, , "Job Code (*)", 120)
            oApplication.Utilities.AddControls(aForm, "HRedJobSt", "HRedOrgSt", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 22, 22, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstJobNa", "HRstOrgNa", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 22, 22, , "Job Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedJobNa", "HRedOrgNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 22, 22, , , 120)
            oEditText = aForm.Items.Item("HRedJobSt").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_JobstCode")
            oEditText.ChooseFromListUID = "CFL_HR_POS"
            oEditText.ChooseFromListAlias = "U_Z_PosCode"
            oEditText = aForm.Items.Item("HRedJobNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_JobstName")
            aForm.Items.Item("HRedJobSt").Enabled = False
            aForm.Items.Item("HRedJobNa").Enabled = False
            oItem = aForm.Items.Item("HRstJobSt")
            oItem.LinkTo = "HRedJobSt"
            oItem = aForm.Items.Item("HRstJobNa")
            oItem.LinkTo = "HRedJobNa"


            oApplication.Utilities.AddControls(aForm, "HRstUnSt", "HRstJobSt", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 22, 22, , "Unit Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedUnSt", "HRedJobSt", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 22, 22, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstScNa", "HRstJobNa", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 22, 22, , "Section Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedScNa", "HRedJobNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 22, 22, , , 120)
            oEditText = aForm.Items.Item("HRedUnSt").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_UnitName")
            oEditText = aForm.Items.Item("HRedScNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_SecName")
            aForm.Items.Item("HRedUnSt").Enabled = False
            aForm.Items.Item("HRedScNa").Enabled = False
            oItem = aForm.Items.Item("HRstUnSt")
            oItem.LinkTo = "HRedUnSt"
            oItem = aForm.Items.Item("HRstScNa")
            oItem.LinkTo = "HRedScNa"







            oApplication.Utilities.AddControls(aForm, "HRstSal", "HRstUnSt", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 22, 22, , "Salary Code (*)", 120)
            oApplication.Utilities.AddControls(aForm, "HRedSal", "HRedUnSt", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 22, 22, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstJobNa2", "HRstScNa", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 22, 22, , "Branch Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedJobNa2", "HRedScNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 22, 22, , , 120)

            oEditText = aForm.Items.Item("HRedSal").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_SalaryCode")
            oEditText.ChooseFromListUID = "CFL_HR_3"
            oEditText.ChooseFromListAlias = "U_Z_SalCode"
            aForm.Items.Item("HRedSal").Enabled = True
            'aForm.Items.Item("HRstJobNa1").Visible = True
            'aForm.Items.Item("HRedJobNa1").Visible = True
            oItem = aForm.Items.Item("HRstSal")
            oItem.LinkTo = "HRedSal"

            oEditText = aForm.Items.Item("HRedJobNa2").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_BraName")
            oItem = aForm.Items.Item("HRstJobNa2")
            oItem.LinkTo = "HRedJobNa2"

            oApplication.Utilities.AddControls(aForm, "HRstLvl", "HRstSal", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 22, 22, , "Level Code (*)", 120)
            oApplication.Utilities.AddControls(aForm, "HRedLvl", "HRedSal", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 22, 22, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstLvNa", "HRstJobNa2", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 22, 22, , "Level Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedLvlNa", "HRedJobNa2", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 22, 22, , , 120)
            oEditText = aForm.Items.Item("HRedLvl").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_LvlCode")
            oEditText.ChooseFromListUID = "CFL_HR_Lv"
            oEditText.ChooseFromListAlias = "U_Z_LvelCode"
            oEditText = aForm.Items.Item("HRedLvlNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_LvlName")
            aForm.Items.Item("HRedJobSt").Enabled = True
            aForm.Items.Item("HRedLvlNa").Enabled = False
            oItem = aForm.Items.Item("HRstLvl")
            oItem.LinkTo = "HRedLvl"
            oItem = aForm.Items.Item("HRstLvNa")
            oItem.LinkTo = "HRedLvlNa"

            oApplication.Utilities.AddControls(aForm, "HRstgrd", "HRstLvl", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 22, 22, , "Grade Code (*)", 120)
            oApplication.Utilities.AddControls(aForm, "HRedgrd", "HRedLvl", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 22, 22, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstgrNa", "HRstLvNa", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 22, 22, , "Grade Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedgrNa", "HRedLvlNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 22, 22, , , 120)
            oEditText = aForm.Items.Item("HRedgrd").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_GrdCode")
            oEditText.ChooseFromListUID = "CFL_HR_Gr"
            oEditText.ChooseFromListAlias = "U_Z_GrdeCode"
            oEditText = aForm.Items.Item("HRedgrNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_GrdName")
            aForm.Items.Item("HRedJobSt").Enabled = True
            aForm.Items.Item("HRedgrNa").Enabled = False
            oItem = aForm.Items.Item("HRstgrd")
            oItem.LinkTo = "HRedgrd"
            oItem = aForm.Items.Item("HRstgrNa")
            oItem.LinkTo = "HRedgrNa"


            oApplication.Utilities.AddControls(aForm, "HRstLoc", "HRstgrd", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 22, 22, , "Location Code (*)", 120)
            oApplication.Utilities.AddControls(aForm, "HRedLoc", "HRedgrd", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 22, 22, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstLoNa", "HRstgrNa", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 22, 22, , "Location Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedLcNa", "HRedgrNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 22, 22, , , 120)
            oEditText = aForm.Items.Item("HRedLoc").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_LocCode")
            oEditText.ChooseFromListUID = "CFL_HR_LOC"
            oEditText.ChooseFromListAlias = "U_Z_LocCode"
            oEditText = aForm.Items.Item("HRedLcNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_LocName")
            aForm.Items.Item("HRedLoc").Enabled = True
            aForm.Items.Item("HRedLcNa").Enabled = False
            oItem = aForm.Items.Item("HRstLoc")
            oItem.LinkTo = "HRedLoc"
            oItem = aForm.Items.Item("HRstLoNa")
            oItem.LinkTo = "HRedLcNa"



            ' aForm.Items.Item("edCoNa").Enabled = False
            aForm.Items.Item("HRedpoNa").Enabled = False
            aForm.Items.Item("HRedOrgSt").Enabled = False
            aForm.Items.Item("HRedOrgNa").Enabled = False
            aForm.Items.Item("HRedJobSt").Enabled = False
            aForm.Items.Item("HRedJobNa").Enabled = False



            oApplication.Utilities.AddControls(aForm, "HRgrdpeo", "93", SAPbouiCOM.BoFormItemTypes.it_GRID, "DOWN", 24, 24, , , 350, , 200)
            aForm.DataSources.DataTables.Add("HRdtPeople")

            oApplication.Utilities.AddControls(aForm, "HRgrdOLoan", "93", SAPbouiCOM.BoFormItemTypes.it_GRID, "DOWN", 26, 26, , , 350, , 200)
            aForm.DataSources.DataTables.Add("HRdtObjLoan")

            oApplication.Utilities.AddControls(aForm, "HRbtnAdd", "1", SAPbouiCOM.BoFormItemTypes.it_BUTTON, "RIGHT", 24, 26, "1", "Add Row")
            oApplication.Utilities.AddControls(aForm, "HRbtnDel", "HRbtnAdd", SAPbouiCOM.BoFormItemTypes.it_BUTTON, "RIGHT", 24, 26, "HRbtnAdd", "Delete Row")

            oApplication.Utilities.AddControls(aForm, "HRbtnTrain", "61", SAPbouiCOM.BoFormItemTypes.it_BUTTON, "DOWN", 32, 32, "61", "Training")

            'oApplication.Utilities.AddControls(aForm, "lblRAut", "HRedSal", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 22, 22, , "Requisition Authorize", 120)
            oApplication.Utilities.AddControls(aForm, "chkRAut", "HRstLoc", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX, "DOWN", 22, 22, , "Is Recruitment Authorize", 150)
            oCheckBox = oForm.Items.Item("chkRAut").Specific
            oCheckBox.DataBind.SetBound(True, "OHEM", "U_Z_HR_ISReqAut")

            oApplication.Utilities.AddControls(aForm, "HRNotes", "chkRAut", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 22, 22, , "To view the details double click on (*) marked field value", 300)


            oApplication.Utilities.AddControls(aForm, "HRgrdcom", "93", SAPbouiCOM.BoFormItemTypes.it_GRID, "DOWN", 25, 25, , , 300, , 200)
            aForm.DataSources.DataTables.Add("HRdtCom")

            LoadGridValues(aForm, "LOAD")

            aForm.Freeze(False)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
            Return False

        End Try
    End Function
    Private Function AddControls_old(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Try
            aForm.Freeze(True)
            AddChooseFromList(aForm)

            oApplication.Utilities.AddControls(aForm, "stHRTitle", "22", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 1, 1, , "Emergency Contact Info", 200)
            aForm.Items.Item("stHRTitle").TextStyle = SAPbouiCOM.BoTextStyle.ts_UNDERLINE
            oApplication.Utilities.AddControls(aForm, "stHRTitle1", "57", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 1, 1, , , )

            oApplication.Utilities.AddControls(aForm, "stHRRest", "stHRTitle", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 1, 1, , "Relation Name", 200)
            oApplication.Utilities.AddControls(aForm, "edHRReNa", "stHRTitle1", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 1, 1)
            oEditText = aForm.Items.Item("edHRReNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Rel_Name")
            oItem = aForm.Items.Item("stHRRest")
            oItem.LinkTo = "edHRReNa"

            oApplication.Utilities.AddControls(aForm, "stHRRTSt", "stHRRest", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 1, 1, , "Relationship Type")
            oApplication.Utilities.AddControls(aForm, "edHRRtNa", "edHRReNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 1, 1)
            oEditText = aForm.Items.Item("edHRRtNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Rel_Type")
            oItem = aForm.Items.Item("stHRRTSt")
            oItem.LinkTo = "edHRRtNa"

            oApplication.Utilities.AddControls(aForm, "stHRRPht", "stHRRTSt", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 1, 1, , "Contact Number")
            oApplication.Utilities.AddControls(aForm, "edHRRPNa", "edHRRtNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 1, 1)
            oEditText = aForm.Items.Item("edHRRPNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_Rel_Phone")
            oItem = aForm.Items.Item("stHRRPht")
            oItem.LinkTo = "edHRRPNa"



            Try
                oApplication.Utilities.AddControls(aForm, "fldHR", "fldPay", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 0, 0, "fldPay", "HR Details")
            Catch ex As Exception
                oApplication.Utilities.AddControls(aForm, "fldHR", "143", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 0, 20, "143", "HR Details")
            End Try

            Dim oldItem As SAPbouiCOM.Item
            oItem = aForm.Items.Item("fldHR")
            oldItem = aForm.Items.Item("26")
            oItem.AffectsFormMode = False
            ofolder = aForm.Items.Item("fldHR").Specific
            '  aForm.DataSources.UserDataSources.Add("HR1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            aForm.Items.Add("HRFldHr1", SAPbouiCOM.BoFormItemTypes.it_FOLDER)
            oldItem = aForm.Items.Item("26")
            oItem = aForm.Items.Item("HRFldHr1")
            oItem.Top = oldItem.Top + 25
            oItem.Left = oldItem.Left + 5
            oItem.Width = 250
            oItem.Height = oldItem.Height
            oItem.FromPane = 18
            oItem.ToPane = 22
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ' ofolder.GroupWith("143")
            ofolder.ValOn = "K"
            ofolder.ValOff = "M"
            ofolder.Caption = "Orgainization Details"
            '  aForm.DataSources.UserDataSources.Add("Acc1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT)
            ofolder.DataBind.SetBound(True, "OHEM", "U_Fld")


            'oApplication.Utilities.AddControls(aForm, "HRFldHr1", "27", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 0, 20, "27", "Orgainization Details", 200)
            '' aForm.DataSources.DataTables.Add("dtCon")
            'oItem = aForm.Items.Item("HRFldHr1")
            'oItem.AffectsFormMode = False
            'ofolder = oItem.Specific
            'ofolder.GroupWith("27")
            'ofolder.ValOn = "K"
            'ofolder.ValOff = "M"


            oApplication.Utilities.AddControls(aForm, "HRFldHr2", "HRFldHr1", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 18, 22, "HRFldHr1", "HR 2nd Language details")
            ' aForm.DataSources.DataTables.Add("dtCon")
            oItem = aForm.Items.Item("HRFldHr2")
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ofolder.GroupWith("HRFldHr1")
            ofolder.ValOn = "O"
            ofolder.ValOff = "M"

            oApplication.Utilities.AddControls(aForm, "HRFldHr3", "HRFldHr2", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 18, 22, "HRFldHr2", "People Objectives")
            ' aForm.DataSources.DataTables.Add("dtCon")
            oItem = aForm.Items.Item("HRFldHr3")
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ofolder.GroupWith("HRFldHr2")
            ofolder.ValOn = "A"
            ofolder.ValOff = "B"

            oApplication.Utilities.AddControls(aForm, "HRFldHr4", "HRFldHr3", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 18, 22, "HRFldHr3", "Competencies")
            ' aForm.DataSources.DataTables.Add("dtCon")
            oItem = aForm.Items.Item("HRFldHr4")
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ofolder.GroupWith("HRFldHr3")
            ofolder.ValOn = "C"
            ofolder.ValOff = "Y"

            oApplication.Utilities.AddControls(aForm, "HRFldHr5", "HRFldHr4", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 18, 22, "HRFldHr4", "Objects on Loan")
            ' aForm.DataSources.DataTables.Add("dtCon")
            oItem = aForm.Items.Item("HRFldHr5")
            oItem.AffectsFormMode = False
            ofolder = oItem.Specific
            ofolder.GroupWith("HRFldHr4")
            ofolder.ValOn = "D"
            ofolder.ValOff = "E"



            'oApplication.Utilities.AddControls(aForm, "HRFldHr3", "HRFldHr1", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 0, 20, "HRFldHr1", "HR 2nd Languages")
            'oItem = aForm.Items.Item("HRFldHr3")
            'oItem.AffectsFormMode = False
            'ofolder = oItem.Specific
            'ofolder.GroupWith("HRFldHr1")



            ' oItem.Visible = False
            'oApplication.Utilities.AddControls(aForm, "HRFldHr2", "HRFldHr3", SAPbouiCOM.BoFormItemTypes.it_FOLDER, "RIGHT", 0, 20, "HRFldHr3", "People Objectives")
            ' aForm.DataSources.DataTables.Add("dtCon")
            'oItem = aForm.Items.Item("HRFldHr2")
            'oItem.AffectsFormMode = False
            'ofolder = oItem.Specific
            'ofolder.GroupWith("HRFldHr3")
            'ofolder.ValOn = "L"
            'ofolder.ValOff = "N"



            'oApplication.Utilities.AddControls(aForm, "stcomp", "93", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 31, 31, , "Company Code", 120)
            'oApplication.Utilities.AddControls(aForm, "edComp", "84", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 31, 31, , , 120)
            'oApplication.Utilities.AddControls(aForm, "stcoNa", "edComp", SAPbouiCOM.BoFormItemTypes.it_STATIC, "RIGHT", 31, 31, , "Company Name", 120)
            'oApplication.Utilities.AddControls(aForm, "edCoNa", "stcoNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "RIGHT", 31, 31, , , 120)
            'oEditText = aForm.Items.Item("edComp").Specific
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_CompCode")
            'oEditText.ChooseFromListUID = "CFL1"
            'oEditText.ChooseFromListAlias = "U_Z_CompCode"
            'oEditText = aForm.Items.Item("edCoNa").Specific
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_CompName")
            'aForm.Items.Item("edComp").Enabled = True
            'aForm.Items.Item("edCoNa").Enabled = False
            'oItem = aForm.Items.Item("stcomp")
            'oItem.LinkTo = "edComp"
            'oItem = aForm.Items.Item("stcoNa")
            'oItem.LinkTo = "edCoNa"

            Try
                oApplication.Utilities.AddControls(aForm, "HRstthid", "stFU", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "Third Name", 80)
                oApplication.Utilities.AddControls(aForm, "HRedthId", "edFU", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , 90)
            Catch ex As Exception
                oApplication.Utilities.AddControls(aForm, "HRstthid", "3", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "Third Name", 80)
                oApplication.Utilities.AddControls(aForm, "HRedthId", "33", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , 90)
            End Try
            oEditText = aForm.Items.Item("HRedthId").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_ThirdName")
            oItem = aForm.Items.Item("HRstthid")
            oItem.LinkTo = "HRedthId"


            'oApplication.Utilities.AddControls(aForm, "HRstthid", "3", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "Third Name", 80)
            'oApplication.Utilities.AddControls(aForm, "HRedthId", "33", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , 90)
            'oEditText = aForm.Items.Item("HRedthId").Specific
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_ThirdName")
            'oItem = aForm.Items.Item("HRstthid")
            'oItem.LinkTo = "HRedthId"
            '' oItem.Left = 310

            'oApplication.Utilities.AddControls(aForm, "HRstAppid", "HRstthid", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 0, 0, , "Applicant Id", 80)
            'oApplication.Utilities.AddControls(aForm, "HRedAppId", "HRedthId", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 0, 0, , , 90)
            'oApplication.Utilities.AddControls(aForm, "HRstAppLk", "HRedAppId", SAPbouiCOM.BoFormItemTypes.it_LINKED_BUTTON, "DOWN", 0, 0, , , 20)
            'oEditText = aForm.Items.Item("HRedAppId").Specific
            'oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_ApplId")
            'oItem = aForm.Items.Item("HRstAppid")
            'oItem.LinkTo = "HRstAppLk"
            'oItem = aForm.Items.Item("HRstAppLk")
            'oItem.LinkTo = "HRedAppId"
            ' oItem.Left = 310


            oApplication.Utilities.AddControls(aForm, "HRstFna", "93", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 19, 19, , "First Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedFna", "84", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 19, 19, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstSeNa", "HRedFna", SAPbouiCOM.BoFormItemTypes.it_STATIC, "RIGHT", 19, 19, , "Second Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedSeNa", "HRstSeNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "RIGHT", 19, 19, , , 120)
            oEditText = aForm.Items.Item("HRedFna").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_AFirstName")
            oEditText = aForm.Items.Item("HRedSeNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_ASecondName")

            oItem = aForm.Items.Item("HRstFna")
            oItem.LinkTo = "HRedFna"
            oItem = aForm.Items.Item("HRstSeNa")
            oItem.LinkTo = "HRedSeNa"

            oApplication.Utilities.AddControls(aForm, "HRstTna", "HRstFna", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 19, 19, , "Third Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedTna", "HRedFna", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 19, 19, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstLaNa", "HRedTna", SAPbouiCOM.BoFormItemTypes.it_STATIC, "RIGHT", 19, 19, , "Last Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedLaNa", "HRstLaNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "RIGHT", 19, 19, , , 120)
            oEditText = aForm.Items.Item("HRedTna").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_AThirdName")
            oEditText = aForm.Items.Item("HRedLaNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_ALastName")

            oItem = aForm.Items.Item("HRstTna")
            oItem.LinkTo = "HRedTna"
            oItem = aForm.Items.Item("HRstLaNa")
            oItem.LinkTo = "HRedLaNa"

            oApplication.Utilities.AddControls(aForm, "HRstCNa", "HRstTna", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 19, 19, , "Company Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedCNa", "HRedTna", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 19, 19, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstDNa", "HRstLaNa", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 19, 19, , "Department Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedDNa", "HRedLaNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 19, 19, , , 120)
            oEditText = aForm.Items.Item("HRedCNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_ACmpName")
            oEditText = aForm.Items.Item("HRedDNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_ADeptName")

            oItem = aForm.Items.Item("HRstCNa")
            oItem.LinkTo = "HRedCNa"
            oItem = aForm.Items.Item("HRstDNa")
            oItem.LinkTo = "HRedDNa"



            oApplication.Utilities.AddControls(aForm, "HRstposi", "93", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 118, 118, , "Position Code", 120)
            oApplication.Utilities.AddControls(aForm, "HRedposi", "84", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 118, 118, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstpoNa", "HRedposi", SAPbouiCOM.BoFormItemTypes.it_STATIC, "RIGHT", 118, 118, , "Position Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedpoNa", "HRstpoNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "RIGHT", 118, 118, , , 120)
            oEditText = aForm.Items.Item("HRedposi").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_PosiCode")
            oEditText.ChooseFromListUID = "CFL_HR_4"
            oEditText.ChooseFromListAlias = "U_Z_PosCode"
            oEditText = aForm.Items.Item("HRedpoNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_PosiName")

            aForm.Items.Item("HRedposi").Enabled = True
            aForm.Items.Item("HRedpoNa").Enabled = False
            oItem = aForm.Items.Item("HRstposi")
            oItem.LinkTo = "HRedposi"
            oItem = aForm.Items.Item("HRstpoNa")
            oItem.LinkTo = "HRedpoNa"


            oApplication.Utilities.AddControls(aForm, "HRstcomp", "93", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 18, 18, , "Company Code", 120)
            oApplication.Utilities.AddControls(aForm, "HRedComp", "84", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 18, 18, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstcoNa", "HRedComp", SAPbouiCOM.BoFormItemTypes.it_STATIC, "RIGHT", 18, 18, , "Company Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedCoNa", "HRstcoNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "RIGHT", 18, 18, , , 120)
            oEditText = aForm.Items.Item("HRedComp").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_CompCode")
            oEditText.ChooseFromListUID = "CFL_HR_1"
            oEditText.ChooseFromListAlias = "U_Z_CompCode"
            oEditText = aForm.Items.Item("HRedCoNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_CompName")
            aForm.Items.Item("HRedComp").Enabled = True
            aForm.Items.Item("HRedCoNa").Enabled = False
            oItem = aForm.Items.Item("HRstcomp")
            oItem.LinkTo = "HRedComp"
            oItem = aForm.Items.Item("HRstcoNa")
            oItem.LinkTo = "HRedCoNa"


            oApplication.Utilities.AddControls(aForm, "HRstDiv", "HRstcomp", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 18, 18, , "Division Code", 120)
            oApplication.Utilities.AddControls(aForm, "HRedDiv", "HRedComp", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 18, 18, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstDiNa", "HRstcoNa", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 18, 18, , "Division Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedDivNa", "HRedCoNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 18, 18, , , 120)
            oEditText = aForm.Items.Item("HRedDiv").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_DivCode")
            'oEditText.ChooseFromListUID = "CFL_HR_1"
            'oEditText.ChooseFromListAlias = "U_Z_CompCode"
            oEditText = aForm.Items.Item("HRedDivNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_DivName")
            aForm.Items.Item("HRedDiv").Enabled = True
            aForm.Items.Item("HRedDivNa").Enabled = False
            oItem = aForm.Items.Item("HRstDiv")
            oItem.LinkTo = "HRedDiv"
            oItem = aForm.Items.Item("HRstDiNa")
            oItem.LinkTo = "HRedDivNa"




            oApplication.Utilities.AddControls(aForm, "HRstOrgSt", "HRstDiv", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 18, 18, , "Organization Code", 120)
            oApplication.Utilities.AddControls(aForm, "HRedOrgSt", "HRedDiv", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 18, 18, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstOrgNa", "HRstDiNa", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 18, 18, , "Organization Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedOrgNa", "HRedDivNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 18, 18, , , 120)
            oEditText = aForm.Items.Item("HRedOrgSt").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_OrgstCode")
            oEditText.ChooseFromListUID = "CFL_HR_2"
            oEditText.ChooseFromListAlias = "U_Z_OrgCode"
            oEditText = aForm.Items.Item("HRedOrgNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_OrgstName")

            aForm.Items.Item("HRedOrgSt").Enabled = False
            aForm.Items.Item("HRedOrgNa").Enabled = False
            oItem = aForm.Items.Item("HRstOrgSt")
            oItem.LinkTo = "HRedOrgSt"
            oItem = aForm.Items.Item("HRstOrgNa")
            oItem.LinkTo = "HRedOrgNa"

            oApplication.Utilities.AddControls(aForm, "HRstJobSt", "HRstOrgSt", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 18, 18, , "Job Code", 120)
            oApplication.Utilities.AddControls(aForm, "HRedJobSt", "HRedOrgSt", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 18, 18, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstJobNa", "HRstOrgNa", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 18, 18, , "Job Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedJobNa", "HRedOrgNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 18, 18, , , 120)
            oEditText = aForm.Items.Item("HRedJobSt").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_JobstCode")
            oEditText.ChooseFromListUID = "CFL_HR_POS"
            oEditText.ChooseFromListAlias = "U_Z_PosCode"
            oEditText = aForm.Items.Item("HRedJobNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_JobstName")
            aForm.Items.Item("HRedJobSt").Enabled = False
            aForm.Items.Item("HRedJobNa").Enabled = False
            oItem = aForm.Items.Item("HRstJobSt")
            oItem.LinkTo = "HRedJobSt"
            oItem = aForm.Items.Item("HRstJobNa")
            oItem.LinkTo = "HRedJobNa"


            oApplication.Utilities.AddControls(aForm, "HRstUnSt", "HRstJobSt", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 18, 18, , "Unit Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedUnSt", "HRedJobSt", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 18, 18, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstScNa", "HRstJobNa", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 18, 18, , "Section Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedScNa", "HRedJobNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 18, 18, , , 120)
            oEditText = aForm.Items.Item("HRedUnSt").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_UnitName")
            oEditText = aForm.Items.Item("HRedScNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_SecName")
            aForm.Items.Item("HRedUnSt").Enabled = False
            aForm.Items.Item("HRedScNa").Enabled = False
            oItem = aForm.Items.Item("HRstUnSt")
            oItem.LinkTo = "HRedUnSt"
            oItem = aForm.Items.Item("HRstScNa")
            oItem.LinkTo = "HRedScNa"







            oApplication.Utilities.AddControls(aForm, "HRstSal", "HRstUnSt", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 18, 18, , "Salary Code", 120)
            oApplication.Utilities.AddControls(aForm, "HRedSal", "HRedUnSt", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 18, 18, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstJobNa2", "HRstScNa", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 18, 18, , "Branch Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedJobNa2", "HRedScNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 18, 18, , , 120)

            oEditText = aForm.Items.Item("HRedSal").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_SalaryCode")
            oEditText.ChooseFromListUID = "CFL_HR_3"
            oEditText.ChooseFromListAlias = "U_Z_SalCode"
            aForm.Items.Item("HRedSal").Enabled = True
            'aForm.Items.Item("HRstJobNa1").Visible = True
            'aForm.Items.Item("HRedJobNa1").Visible = True
            oItem = aForm.Items.Item("HRstSal")
            oItem.LinkTo = "HRedSal"

            oEditText = aForm.Items.Item("HRedJobNa2").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_HR_BraName")
            oItem = aForm.Items.Item("HRstJobNa2")
            oItem.LinkTo = "HRedJobNa2"

            oApplication.Utilities.AddControls(aForm, "HRstLvl", "HRstSal", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 18, 18, , "Level Code", 120)
            oApplication.Utilities.AddControls(aForm, "HRedLvl", "HRedSal", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 18, 18, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstLvNa", "HRstJobNa2", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 18, 18, , "Level Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedLvlNa", "HRedJobNa2", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 18, 18, , , 120)
            oEditText = aForm.Items.Item("HRedLvl").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_LvlCode")
            oEditText.ChooseFromListUID = "CFL_HR_Lv"
            oEditText.ChooseFromListAlias = "U_Z_LvelCode"
            oEditText = aForm.Items.Item("HRedLvlNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_LvlName")
            aForm.Items.Item("HRedJobSt").Enabled = True
            aForm.Items.Item("HRedLvlNa").Enabled = False
            oItem = aForm.Items.Item("HRstLvl")
            oItem.LinkTo = "HRedLvl"
            oItem = aForm.Items.Item("HRstLvNa")
            oItem.LinkTo = "HRedLvlNa"

            oApplication.Utilities.AddControls(aForm, "HRstgrd", "HRstLvl", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 18, 18, , "Grade Code", 120)
            oApplication.Utilities.AddControls(aForm, "HRedgrd", "HRedLvl", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 18, 18, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstgrNa", "HRstLvNa", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 18, 18, , "Grade Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedgrNa", "HRedLvlNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 18, 18, , , 120)
            oEditText = aForm.Items.Item("HRedgrd").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_GrdCode")
            oEditText.ChooseFromListUID = "CFL_HR_Gr"
            oEditText.ChooseFromListAlias = "U_Z_GrdeCode"
            oEditText = aForm.Items.Item("HRedgrNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_GrdName")
            aForm.Items.Item("HRedJobSt").Enabled = True
            aForm.Items.Item("HRedgrNa").Enabled = False
            oItem = aForm.Items.Item("HRstgrd")
            oItem.LinkTo = "HRedgrd"
            oItem = aForm.Items.Item("HRstgrNa")
            oItem.LinkTo = "HRedgrNa"


            oApplication.Utilities.AddControls(aForm, "HRstLoc", "HRstgrd", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 18, 18, , "Location Code", 120)
            oApplication.Utilities.AddControls(aForm, "HRedLoc", "HRedgrd", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 18, 18, , , 120)
            oApplication.Utilities.AddControls(aForm, "HRstLoNa", "HRstgrNa", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 18, 18, , "Location Name", 120)
            oApplication.Utilities.AddControls(aForm, "HRedLcNa", "HRedgrNa", SAPbouiCOM.BoFormItemTypes.it_EDIT, "DOWN", 18, 18, , , 120)
            oEditText = aForm.Items.Item("HRedLoc").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_LocCode")
            oEditText.ChooseFromListUID = "CFL_HR_LOC"
            oEditText.ChooseFromListAlias = "U_Z_LocCode"
            oEditText = aForm.Items.Item("HRedLcNa").Specific
            oEditText.DataBind.SetBound(True, "OHEM", "U_Z_LocName")
            aForm.Items.Item("HRedLoc").Enabled = True
            aForm.Items.Item("HRedLcNa").Enabled = False
            oItem = aForm.Items.Item("HRstLoc")
            oItem.LinkTo = "HRedLoc"
            oItem = aForm.Items.Item("HRstLoNa")
            oItem.LinkTo = "HRedLcNa"



            ' aForm.Items.Item("edCoNa").Enabled = False
            aForm.Items.Item("HRedpoNa").Enabled = False
            aForm.Items.Item("HRedOrgSt").Enabled = False
            aForm.Items.Item("HRedOrgNa").Enabled = False
            aForm.Items.Item("HRedJobSt").Enabled = False
            aForm.Items.Item("HRedJobNa").Enabled = False



            oApplication.Utilities.AddControls(aForm, "HRgrdpeo", "93", SAPbouiCOM.BoFormItemTypes.it_GRID, "DOWN", 20, 20, , , 200, , 100)
            aForm.DataSources.DataTables.Add("HRdtPeople")

            oApplication.Utilities.AddControls(aForm, "HRgrdOLoan", "93", SAPbouiCOM.BoFormItemTypes.it_GRID, "DOWN", 22, 22, , , 200, , 100)
            aForm.DataSources.DataTables.Add("HRdtObjLoan")

            oApplication.Utilities.AddControls(aForm, "HRbtnAdd", "1", SAPbouiCOM.BoFormItemTypes.it_BUTTON, "RIGHT", 20, 22, "1", "Add Row")
            oApplication.Utilities.AddControls(aForm, "HRbtnDel", "HRbtnAdd", SAPbouiCOM.BoFormItemTypes.it_BUTTON, "RIGHT", 20, 22, "HRbtnAdd", "Delete Row")

            oApplication.Utilities.AddControls(aForm, "HRbtnTrain", "61", SAPbouiCOM.BoFormItemTypes.it_BUTTON, "DOWN", 32, 32, "61", "Training")

            'oApplication.Utilities.AddControls(aForm, "lblRAut", "HRedSal", SAPbouiCOM.BoFormItemTypes.it_STATIC, "DOWN", 18, 18, , "Recruitment Authorize", 120)
            oApplication.Utilities.AddControls(aForm, "chkRAut", "HRstLoc", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX, "DOWN", 18, 18, , "Is Recruitment Authorize", 130)
            oCheckBox = oForm.Items.Item("chkRAut").Specific
            oCheckBox.DataBind.SetBound(True, "OHEM", "U_Z_HR_ISReqAut")

            oApplication.Utilities.AddControls(aForm, "HRgrdcom", "93", SAPbouiCOM.BoFormItemTypes.it_GRID, "DOWN", 21, 21, , , 200, , 100)
            aForm.DataSources.DataTables.Add("HRdtCom")

            LoadGridValues(aForm, "LOAD")

            aForm.Freeze(False)
            Return True
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aForm.Freeze(False)
            Return False

        End Try
    End Function
#End Region

#Region "AddChooseFromList"
    Private Sub AddChooseFromList(ByVal aform As SAPbouiCOM.Form)
        Try
            Dim oCFLs As SAPbouiCOM.ChooseFromListCollection
            Dim oCons As SAPbouiCOM.Conditions
            Dim oCon As SAPbouiCOM.Condition

            oCFLs = aform.ChooseFromLists

            Dim oCFL As SAPbouiCOM.ChooseFromList
            Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams


            ' Adding 1 CFL, one for the button and one for the edit text.
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_OADM"
            oCFLCreationParams.UniqueID = "CFL_HR_1"

            oCFL = oCFLs.Add(oCFLCreationParams)
            ' Adding Conditions to CFL1
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.Alias = "U_Z_Status"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"


            ' Adding 2 CFL, one for the button and one for the edit text.
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_ORGST"
            oCFLCreationParams.UniqueID = "CFL_HR_2"
            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding 3 CFL, one for the button and one for the edit text.
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_OSALST"
            oCFLCreationParams.UniqueID = "CFL_HR_3"
            oCFL = oCFLs.Add(oCFLCreationParams)


            ' Adding 4 CFL, one for the button and one for the edit text.
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_OPOSIN"
            oCFLCreationParams.UniqueID = "CFL_HR_4"
            oCFL = oCFLs.Add(oCFLCreationParams)


            ' Adding 4 CFL, one for the button and one for the edit text.
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_OPEOB"
            oCFLCreationParams.UniqueID = "CFL_HR_5"
            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding 4 CFL, one for the button and one for the edit text.
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "Z_HR_OCOMP"
            oCFLCreationParams.UniqueID = "CFL_HR_6"

            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "4"
            oCFLCreationParams.UniqueID = "CFL_HR_7"
            oCFL = oCFLs.Add(oCFLCreationParams)

            'oCons = oCFL.GetConditions()
            'oCon = oCons.Add()
            'oCon.Alias = "ItemType"
            'oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            'oCon.CondVal = "F"
            'oCFL.SetConditions(oCons)
            'oCon = oCons.Add()

            oCons = oCFL.GetConditions()
            oCon = oCons.Add
            oCon.BracketOpenNum = 2
            oCon.Alias = "ItemType"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "F"
            oCon.BracketCloseNum = 1

            oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND
            oCon = oCons.Add
            oCon.BracketOpenNum = 1
            oCon.Alias = "Employee"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_EQUAL
            oCon.CondVal = "x"
            oCon.BracketCloseNum = 2
            oCFL.SetConditions(oCons)
            oCon = oCons.Add()


            oCFLCreationParams.ObjectType = "Z_HR_OLVL"
            oCFLCreationParams.UniqueID = "CFL_HR_Lv"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams.ObjectType = "Z_HR_OGRD"
            oCFLCreationParams.UniqueID = "CFL_HR_Gr"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
            oCFLCreationParams.ObjectType = "Z_HR_OLOC"
            oCFLCreationParams.UniqueID = "CFL_HR_LOC"
            oCFL = oCFLs.Add(oCFLCreationParams)

            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)
            oCFLCreationParams.ObjectType = "Z_HR_EPOCOM"
            oCFLCreationParams.UniqueID = "CFL_HR_POS"
            oCFL = oCFLs.Add(oCFLCreationParams)
            'Z_HR_EPOCOM

            'Object on Loan Employee ID

            oCFLCreationParams.MultiSelection = False
            oCFLCreationParams.ObjectType = "171"
            oCFLCreationParams.UniqueID = "CFL_LEMP"

            oCFL = oCFLs.Add(oCFLCreationParams)
            ' Adding Conditions to CFL1
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            oCon.Alias = "Active"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "Y"

        Catch ex As Exception

        End Try
    End Sub


    Private Sub AddChooseFromList_Conditions_SalesOrder(ByVal objForm As SAPbouiCOM.Form)
        Try

            Dim oCFLs As SAPbouiCOM.ChooseFromListCollection
            Dim oCons As SAPbouiCOM.Conditions
            Dim oCon As SAPbouiCOM.Condition
            oCFLs = objForm.ChooseFromLists
            Dim oCFL As SAPbouiCOM.ChooseFromList
            Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams
            oCFLCreationParams = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)


            '   oCombobox = objForm.Items.Item("4").Specific

            strCardCode = oApplication.Utilities.getEdittextvalue(objForm, "33")

            '   oGrid = objForm.Items.Item("26").Specific
            oCFL = oCFLs.Item("CFL_HR_7")

            '  Exit Sub
            oCons = oCFL.GetConditions()
            If oCons.Count >= 2 Then
                oCon = oCons.Item(1)
                oCon.Alias = "Employee"
                If strCardCode = "" Then
                    oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_EQUAL
                    oCon.CondVal = "X"
                Else
                    oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
                    oCon.CondVal = strCardCode
                End If
                oCFL.SetConditions(oCons)
                ' oCon = oCons.Add()
            Else

                oCon = oCons.Add()
                oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND
                'oCon.Alias = "U_Z_Status"
                oCon.Alias = "Employee"
                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
                oCon.CondVal = strCardCode
                oCFL.SetConditions(oCons)
                oCon = oCons.Add()
            End If


            ' oCon = oCons.Add
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

#End Region

#Region "Load Grid Values"
    Private Sub LoadGridValues(ByVal aForm As SAPbouiCOM.Form, ByVal aChoice As String)
        Try
            aForm.Freeze(True)

            Select Case aChoice
                Case "LOAD"
                    oGrid = aForm.Items.Item("HRgrdpeo").Specific
                    oGrid.DataTable = aForm.DataSources.DataTables.Item("HRdtPeople")
                    oGrid.DataTable.ExecuteQuery("Select * from ""@Z_HR_PEOBJ1"" where 1=2")
                    oGrid.Columns.Item("Code").Visible = False
                    oGrid.Columns.Item("Name").Visible = False
                    oGrid.Columns.Item("U_Z_HREmpID").Visible = False
                    oGrid.Columns.Item("U_Z_HRPeoobjCode").TitleObject.Caption = "People Objective Code"
                    oGrid.Columns.Item("U_Z_HRPeoobjName").TitleObject.Caption = "Objective Description"
                    oGrid.Columns.Item("U_Z_HRPeoobjName").Editable = False
                    oGrid.Columns.Item("U_Z_HRPeoCategory").TitleObject.Caption = "Category"
                    oGrid.Columns.Item("U_Z_HRPeoCategory").Editable = False
                    oGrid.Columns.Item("U_Z_HRWeight").TitleObject.Caption = "Weight"
                    oGrid.Columns.Item("U_Z_MKPI").TitleObject.Caption = "Management Criteria(KPI)"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_HRPeoobjCode")
                    oEditTextColumn.ChooseFromListUID = "CFL_HR_5"
                    oEditTextColumn.ChooseFromListAlias = "U_Z_PeoobjCode"
                    oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    '  aForm.Items.Item("edCoNa").Enabled = False
                    aForm.Items.Item("HRedpoNa").Enabled = False

                    oGrid = aForm.Items.Item("HRgrdcom").Specific
                    oGrid.DataTable = aForm.DataSources.DataTables.Item("HRdtCom")
                    oGrid.DataTable.ExecuteQuery("Select * from ""@Z_HR_ECOLVL"" where 1 = 2")
                    oGrid.Columns.Item("Code").Visible = False
                    oGrid.Columns.Item("Name").Visible = False
                    oGrid.Columns.Item("U_Z_HREmpID").Visible = False
                    oGrid.Columns.Item("U_Z_CompCode").TitleObject.Caption = "Competency Code"
                    oGrid.Columns.Item("U_Z_CompName").TitleObject.Caption = "Competency Description"
                    oGrid.Columns.Item("U_Z_CompName").Editable = False
                    oGrid.Columns.Item("U_Z_Weight").TitleObject.Caption = "Weight"
                    oGrid.Columns.Item("U_Z_CompLevel").TitleObject.Caption = "Current Level"

                    oGrid.Columns.Item("U_Z_CompLevel").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_CompLevel")
                    oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oRecSet.DoQuery("Select ""U_Z_LvelCode"" As ""Code"",""U_Z_LvelName"" As ""Name"" From ""@Z_HR_COLVL""")

                    oComboColumn.ValidValues.Add("", "")
                    If Not oRecSet.EoF Then
                        For index As Integer = 0 To oRecSet.RecordCount - 1
                            If Not oRecSet.EoF Then
                                oComboColumn.ValidValues.Add(oRecSet.Fields.Item("Code").Value, oRecSet.Fields.Item("Name").Value)
                                oRecSet.MoveNext()
                            End If
                        Next
                    End If
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                    oEditTextColumn = oGrid.Columns.Item("U_Z_CompCode")
                    oEditTextColumn.ChooseFromListUID = "CFL_HR_6"
                    oEditTextColumn.ChooseFromListAlias = "U_Z_CompCode"
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single

                    oGrid = aForm.Items.Item("HRgrdOLoan").Specific
                    oGrid.DataTable = aForm.DataSources.DataTables.Item("HRdtObjLoan")
                    oGrid.DataTable.ExecuteQuery("Select * from ""@Z_HR_OBJLOAN"" where 1=2")
                    oGrid.Columns.Item("Code").Visible = False
                    oGrid.Columns.Item("Name").Visible = False
                    oGrid.Columns.Item("U_Z_HREmpID").Visible = False
                    oGrid.Columns.Item("U_Z_ObjCode").TitleObject.Caption = "Asset Code"
                    oGrid.Columns.Item("U_Z_ObjName").TitleObject.Caption = "Asset Description"
                    oGrid.Columns.Item("U_Z_ObjName").Editable = False
                    oEditTextColumn = oGrid.Columns.Item("U_Z_ObjCode")
                    oEditTextColumn.ChooseFromListUID = "CFL_HR_7"
                    oEditTextColumn.ChooseFromListAlias = "ItemCode"
                    oEditTextColumn.LinkedObjectType = "4"
                    oGrid.Columns.Item("U_Z_Dept").TitleObject.Caption = "Responsible Department"
                    oGrid.Columns.Item("U_Z_Dept").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_Dept")
                    For intRow As Integer = oComboColumn.ValidValues.Count - 1 To 0 Step -1
                        oComboColumn.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
                    Next
                    Try
                        oComboColumn.ValidValues.Add("", "")
                        Dim otestrs As SAPbobsCOM.Recordset
                        otestrs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        otestrs.DoQuery("SELECT T0.""Code"", T0.""Remarks"" FROM OUDP T0 order by ""Code""")
                        For intRow As Integer = 0 To otestrs.RecordCount - 1
                            oComboColumn.ValidValues.Add(otestrs.Fields.Item(0).Value, otestrs.Fields.Item(1).Value)
                            otestrs.MoveNext()
                        Next

                    Catch ex As Exception

                    End Try
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
                    oGrid.Columns.Item("U_Z_ResID").TitleObject.Caption = "Responsible Employee ID"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_ResID")
                    'oEditTextColumn.ChooseFromListUID = "CFL_LEMP"
                    'oEditTextColumn.ChooseFromListAlias = "empID"
                    oEditTextColumn.LinkedObjectType = "171"
                    oGrid.Columns.Item("U_Z_ResName").TitleObject.Caption = "Responsible Employee"
                    oGrid.Columns.Item("U_Z_ResName").Editable = False
                    oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remakrs"
                    oGrid.Columns.Item("U_Z_CompStatus").Visible = False
                    oGrid.Columns.Item("U_Z_ApprovedBy").Visible = False
                    oGrid.Columns.Item("U_Z_Appdt").Visible = False
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    '  aForm.Items.Item("edCoNa").Enabled = False
                    'aForm.Items.Item("HRedpoNa").Enabled = False
                Case "NAVIGATION"
                    Dim aCode As String
                    aCode = oApplication.Utilities.getEdittextvalue(aForm, "33")
                    oGrid = aForm.Items.Item("HRgrdpeo").Specific
                    oGrid.DataTable = aForm.DataSources.DataTables.Item("HRdtPeople")
                    oGrid.DataTable.ExecuteQuery("Select * from ""@Z_HR_PEOBJ1"" where ""U_Z_HREmpID""='" & aCode & "'")
                    oGrid.Columns.Item("Code").Visible = False
                    oGrid.Columns.Item("Name").Visible = False
                    oGrid.Columns.Item("U_Z_HREmpID").Visible = False
                    oGrid.Columns.Item("U_Z_HRPeoobjCode").TitleObject.Caption = "People Objective Code"
                    oGrid.Columns.Item("U_Z_HRPeoobjName").TitleObject.Caption = "Objective Description"
                    oGrid.Columns.Item("U_Z_HRPeoobjName").Editable = False
                    oGrid.Columns.Item("U_Z_HRPeoCategory").TitleObject.Caption = "Category"
                    oGrid.Columns.Item("U_Z_HRPeoCategory").Editable = False
                    oGrid.Columns.Item("U_Z_HRWeight").TitleObject.Caption = "Weight"
                    oGrid.Columns.Item("U_Z_MKPI").TitleObject.Caption = "Management Criteria(KPI)"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_HRPeoobjCode")
                    oEditTextColumn.ChooseFromListUID = "CFL_HR_5"
                    oEditTextColumn.ChooseFromListAlias = "U_Z_PeoobjCode"
                    oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    ' aForm.Items.Item("edCoNa").Enabled = False
                    aForm.Items.Item("HRedpoNa").Enabled = False


                    'Dim aCode As String
                    aCode = oApplication.Utilities.getEdittextvalue(aForm, "33")
                    oGrid = aForm.Items.Item("HRgrdcom").Specific
                    oGrid.DataTable = aForm.DataSources.DataTables.Item("HRdtCom")
                    oGrid.DataTable.ExecuteQuery("Select * from ""@Z_HR_ECOLVL"" where ""U_Z_HREmpID"" ='" & aCode & "'")
                    oGrid.Columns.Item("Code").Visible = False
                    oGrid.Columns.Item("Name").Visible = False
                    oGrid.Columns.Item("U_Z_HREmpID").Visible = False
                    oGrid.Columns.Item("U_Z_CompCode").TitleObject.Caption = "Competency Code"
                    oGrid.Columns.Item("U_Z_CompCode").Editable = True
                    oGrid.Columns.Item("U_Z_CompName").TitleObject.Caption = "Competency Description"
                    oGrid.Columns.Item("U_Z_CompName").Editable = False
                    oGrid.Columns.Item("U_Z_Weight").TitleObject.Caption = "Weight"
                    oGrid.Columns.Item("U_Z_Weight").Editable = True
                    oGrid.Columns.Item("U_Z_PosCode").TitleObject.Caption = "Position Name"
                    oGrid.Columns.Item("U_Z_PosCode").Editable = False
                    oGrid.Columns.Item("U_Z_CompLevel").TitleObject.Caption = "Current Level"
                    oGrid.Columns.Item("U_Z_CompLevel").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_CompLevel")
                    oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oRecSet.DoQuery("Select ""U_Z_LvelCode"" As ""Code"",""U_Z_LvelName"" As ""Name"" From ""@Z_HR_COLVL""")

                    oComboColumn.ValidValues.Add("", "")
                    If Not oRecSet.EoF Then
                        For index As Integer = 0 To oRecSet.RecordCount - 1
                            If Not oRecSet.EoF Then
                                oComboColumn.ValidValues.Add(oRecSet.Fields.Item("Code").Value, oRecSet.Fields.Item("Name").Value)
                                oRecSet.MoveNext()
                            End If
                        Next
                    End If
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description

                    oEditTextColumn = oGrid.Columns.Item("U_Z_CompCode")
                    oEditTextColumn.ChooseFromListUID = "CFL_HR_6"
                    oEditTextColumn.ChooseFromListAlias = "U_Z_CompCode"
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single


                    oGrid = aForm.Items.Item("HRgrdOLoan").Specific
                    aCode = oApplication.Utilities.getEdittextvalue(aForm, "33")
                    oGrid.DataTable = aForm.DataSources.DataTables.Item("HRdtObjLoan")
                    oGrid.DataTable.ExecuteQuery("Select * from ""@Z_HR_OBJLOAN"" where ""U_Z_HREmpID"" ='" & aCode & "'")
                    oGrid.Columns.Item("Code").Visible = False
                    oGrid.Columns.Item("Name").Visible = False
                    oGrid.Columns.Item("U_Z_HREmpID").Visible = False
                    oGrid.Columns.Item("U_Z_ObjCode").TitleObject.Caption = "Asset Code"
                    oGrid.Columns.Item("U_Z_ObjName").TitleObject.Caption = "Asset Description"
                    oGrid.Columns.Item("U_Z_ObjName").Editable = False
                    AddChooseFromList_Conditions_SalesOrder(aForm)
                    oEditTextColumn = oGrid.Columns.Item("U_Z_ObjCode")
                    oEditTextColumn.ChooseFromListUID = "CFL_HR_7"
                    oEditTextColumn.ChooseFromListAlias = "ItemCode"
                    oEditTextColumn.LinkedObjectType = "4"
                    oGrid.Columns.Item("U_Z_Dept").TitleObject.Caption = "Responsible Department"
                    oGrid.Columns.Item("U_Z_Dept").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oComboColumn = oGrid.Columns.Item("U_Z_Dept")
                    For intRow As Integer = oComboColumn.ValidValues.Count - 1 To 0 Step -1
                        oComboColumn.ValidValues.Remove(intRow, SAPbouiCOM.BoSearchKey.psk_Index)
                    Next
                    Try
                        oComboColumn.ValidValues.Add("", "")
                        Dim otestrs As SAPbobsCOM.Recordset
                        otestrs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        otestrs.DoQuery("SELECT T0.""Code"", T0.""Remarks"" FROM OUDP T0 order by ""Code""")
                        For intRow As Integer = 0 To otestrs.RecordCount - 1
                            oComboColumn.ValidValues.Add(otestrs.Fields.Item(0).Value, otestrs.Fields.Item(1).Value)
                            otestrs.MoveNext()
                        Next

                    Catch ex As Exception

                    End Try
                    oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oComboColumn.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly
                    oGrid.Columns.Item("U_Z_ResID").TitleObject.Caption = "Responsible Employee ID"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_ResID")
                    'oEditTextColumn.ChooseFromListUID = "CFL_LEMP"
                    'oEditTextColumn.ChooseFromListAlias = "empID"
                    oEditTextColumn.LinkedObjectType = "171"
                    oGrid.Columns.Item("U_Z_ResName").TitleObject.Caption = "Responsible Employee"
                    oGrid.Columns.Item("U_Z_ResName").Editable = False
                    oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remakrs"
                    oGrid.Columns.Item("U_Z_CompStatus").Visible = False
                    oGrid.Columns.Item("U_Z_ApprovedBy").Visible = False
                    oGrid.Columns.Item("U_Z_Appdt").Visible = False
                    oGrid.AutoResizeColumns()
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    '  aForm.Items.Item("edCoNa").Enabled = False
                    'aForm.Items.Item("HRedpoNa").Enabled = False
            End Select
            ' aForm.Items.Item("HRstJobNa1").Visible = True
            ' aForm.Items.Item("HRedJobNa1").Visible = True
            aForm.Freeze(False)
        Catch ex As Exception
            aForm.Freeze(False)
        End Try
    End Sub

#End Region

#Region "AddRow"
    Private Sub AddRow(ByVal aForm As SAPbouiCOM.Form)
        Select Case aForm.PaneLevel
            Case 24
                oGrid = aForm.Items.Item("HRgrdpeo").Specific
                oEditTextColumn = oGrid.Columns.Item("U_Z_HRPeoobjCode")
            Case 25
                oGrid = aForm.Items.Item("HRgrdcom").Specific
                oEditTextColumn = oGrid.Columns.Item("U_Z_CompCode")
            Case 26
                oGrid = aForm.Items.Item("HRgrdOLoan").Specific
                oEditTextColumn = oGrid.Columns.Item("U_Z_ObjCode")
        End Select
        Dim strCode As String
        If oGrid.DataTable.Rows.Count - 1 <= 0 Then
            oGrid.DataTable.Rows.Add()
        End If
        'oEditTextColumn = oGrid.Columns.Item("U_Z_HRPeoobjCode")
        strCode = oEditTextColumn.GetText(oGrid.DataTable.Rows.Count - 1).ToString
        ' strCode = oEditTextColumn.GetTex(oGrid.DataTable.Rows.Count - 1).Value
        If strCode <> "" Then
            oGrid.DataTable.Rows.Add()
            If aForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And aForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                aForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
            End If
        End If
    End Sub
#End Region

#Region "DeleteRow"
    Private Sub DeleteRow(ByVal aForm As SAPbouiCOM.Form)
        Dim strTable As String
        Select Case aForm.PaneLevel
            Case 24
                oGrid = aForm.Items.Item("HRgrdpeo").Specific
                strTable = "@Z_HR_PEOBJ1"
            Case 25
                oGrid = aForm.Items.Item("HRgrdcom").Specific
                strTable = "@Z_HR_ECOLVL"
            Case 26
                oGrid = aForm.Items.Item("HRgrdOLoan").Specific
                strTable = "@Z_HR_OBJLOAN"
        End Select
        Dim strCode As String
        Dim oTemp As SAPbobsCOM.Recordset
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.Rows.IsSelected(intRow) Then
                strCode = oGrid.DataTable.GetValue("Code", intRow)
                oTemp.DoQuery("Update """ & strTable & """ set ""Name""=""Name""+'_XD' where ""Code""='" & strCode & "'")
                oGrid.DataTable.Rows.Remove(intRow)
                If aForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And aForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                    aForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                End If
                Exit Sub
            End If
        Next
    End Sub
#End Region

#Region "AddToUDT"
    Private Function AddToUDT(ByVal aForm As SAPbouiCOM.Form) As Boolean
        Dim strTable, strEmpId, strCode, strType, strAccountCode As String
        Dim dblValue As Double
        Dim oUserTable, oCompTable, ObjLoanTable As SAPbobsCOM.UserTable
        Dim oValidateRS As SAPbobsCOM.Recordset
        strEmpId = oApplication.Utilities.getEdittextvalue(aForm, "33")
        oUserTable = oApplication.Company.UserTables.Item("Z_HR_PEOBJ1")
        oCompTable = oApplication.Company.UserTables.Item("Z_HR_ECOLVL")
        ObjLoanTable = oApplication.Company.UserTables.Item("Z_HR_OBJLOAN")
        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oGrid = aForm.Items.Item("HRgrdpeo").Specific
        Dim strCo, strCode1, strCate, strCate1 As String

        'Validation
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.DataTable.GetValue("U_Z_HRPeoobjCode", intRow) <> "" Then
                strCo = oGrid.DataTable.GetValue("U_Z_HRPeoobjCode", intRow)
                strCate = oGrid.DataTable.GetValue("U_Z_HRPeoCategory", intRow)
                For intLoop As Integer = intRow + 1 To oGrid.DataTable.Rows.Count - 1
                    strCode1 = oGrid.DataTable.GetValue("U_Z_HRPeoobjCode", intLoop)
                    strCate1 = oGrid.DataTable.GetValue("U_Z_HRPeoCategory", intLoop)
                    If strCode1 <> "" Then
                        If strCo.ToUpper = strCode1.ToUpper And strCate.ToUpper = strCate1.ToUpper Then
                            oApplication.Utilities.Message("Personal Objective : This entry already exists : " & strCode1, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            oGrid.Columns.Item("U_Z_HRPeoobjCode").Click(intLoop)
                            Return False
                        End If
                    End If
                Next
            End If
        Next

        'Validation
        Dim dbweight, TotWeight, dbweight1 As Double
        'For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
        '    dbweight = oGrid.DataTable.GetValue("U_Z_HRWeight", intRow)
        '    dbweight1 = dbweight1 + dbweight
        '    TotWeight = 100
        'Next
        If TotWeight <> dbweight1 Then
            ' oApplication.Utilities.Message("Total of People Objective weights should be 100% not less not more...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            ' Return False
        End If

        'Competency Objective
        oGrid = aForm.Items.Item("HRgrdcom").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.DataTable.GetValue("U_Z_CompCode", intRow) <> "" Then
                strCo = oGrid.DataTable.GetValue("U_Z_CompCode", intRow)
                'strCate = oGrid.DataTable.GetValue("U_Z_CompLevel", intRow)
                For intLoop As Integer = intRow + 1 To oGrid.DataTable.Rows.Count - 1
                    strCode1 = oGrid.DataTable.GetValue("U_Z_CompCode", intLoop)
                    ' strCate = oGrid.DataTable.GetValue("U_Z_CompLevel", intLoop)
                    If strCode1 <> "" Then
                        If strCo.ToUpper = strCode1.ToUpper Then
                            oApplication.Utilities.Message("Compentencies : This entry already exists :" & strCode1, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            oGrid.Columns.Item("U_Z_CompCode").Click(intLoop)
                            Return False
                        End If
                    End If
                Next
            End If
        Next

        'Objects On Loan Validation
        oGrid = aForm.Items.Item("HRgrdOLoan").Specific
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            If oGrid.DataTable.GetValue("U_Z_ObjCode", intRow) <> "" Then
                strCo = oGrid.DataTable.GetValue("U_Z_ObjCode", intRow)
                For intLoop As Integer = intRow + 1 To oGrid.DataTable.Rows.Count - 1
                    strCode1 = oGrid.DataTable.GetValue("U_Z_ObjCode", intLoop)
                    If strCode1 <> "" Then
                        If strCo.ToUpper = strCode1.ToUpper Then
                            oApplication.Utilities.Message("Object on Loan : This entry already exists : " & strCode1, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            oGrid.Columns.Item("U_Z_ObjCode").Click(intLoop)
                            Return False
                        End If
                    End If
                Next
            End If
        Next

        'Validation
        'dbweight = 0
        'TotWeight = 0
        'dbweight1 = 0
        'For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
        '    dbweight = oGrid.DataTable.GetValue("U_Z_Weight", intRow)
        '    dbweight1 = dbweight1 + dbweight
        '    TotWeight = 100
        'Next
        'If TotWeight <> dbweight1 Then
        '    ' oApplication.Utilities.Message("Total of Competency Objective weights should be 100% not less not more...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        '    ' Return False
        'End If


        'Addition of People Objective
        oGrid = aForm.Items.Item("HRgrdpeo").Specific
        strTable = "@Z_HR_PEOBJ1"
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strCode = oGrid.DataTable.GetValue("Code", intRow)
            oEditTextColumn = oGrid.Columns.Item("U_Z_HRPeoobjCode")
            Try
                strType = oEditTextColumn.GetText(oGrid.DataTable.Rows.Count - 1).ToString
            Catch ex As Exception
                strType = ""
            End Try

            If strType <> "" Then
                If oUserTable.GetByKey(strCode) Then
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode
                    oUserTable.UserFields.Fields.Item("U_Z_HREmpID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_HRPeoobjCode").Value = oGrid.DataTable.GetValue("U_Z_HRPeoobjCode", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_HRPeoobjName").Value = oGrid.DataTable.GetValue("U_Z_HRPeoobjName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_HRPeoCategory").Value = oGrid.DataTable.GetValue("U_Z_HRPeoCategory", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_HRWeight").Value = oGrid.DataTable.GetValue("U_Z_HRWeight", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_MKPI").Value = oGrid.DataTable.GetValue("U_Z_MKPI", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Remarks").Value = oGrid.DataTable.GetValue("U_Z_Remarks", intRow)
                    If oUserTable.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                Else
                    strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                    oUserTable.Code = strCode
                    oUserTable.Name = strCode + "_N"
                    oUserTable.UserFields.Fields.Item("U_Z_HREmpID").Value = strEmpId
                    oUserTable.UserFields.Fields.Item("U_Z_HRPeoobjCode").Value = oGrid.DataTable.GetValue("U_Z_HRPeoobjCode", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_HRPeoobjName").Value = oGrid.DataTable.GetValue("U_Z_HRPeoobjName", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_HRPeoCategory").Value = oGrid.DataTable.GetValue("U_Z_HRPeoCategory", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_HRWeight").Value = oGrid.DataTable.GetValue("U_Z_HRWeight", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_MKPI").Value = oGrid.DataTable.GetValue("U_Z_MKPI", intRow)
                    oUserTable.UserFields.Fields.Item("U_Z_Remarks").Value = oGrid.DataTable.GetValue("U_Z_Remarks", intRow)
                    If oUserTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
            End If
        Next

        'Addition of People Objective
        oGrid = aForm.Items.Item("HRgrdcom").Specific
        strTable = "@Z_HR_ECOLVL"
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strCode = oGrid.DataTable.GetValue("Code", intRow)
            oEditTextColumn = oGrid.Columns.Item("U_Z_CompCode")
            Try
                strType = oEditTextColumn.GetText(oGrid.DataTable.Rows.Count - 1).ToString
            Catch ex As Exception
                strType = ""
            End Try

            If strType <> "" Then
                If oCompTable.GetByKey(strCode) Then
                    oCompTable.Code = strCode
                    oCompTable.Name = strCode
                    oCompTable.UserFields.Fields.Item("U_Z_HREmpID").Value = strEmpId
                    oCompTable.UserFields.Fields.Item("U_Z_CompCode").Value = oGrid.DataTable.GetValue("U_Z_CompCode", intRow)
                    oCompTable.UserFields.Fields.Item("U_Z_CompName").Value = oGrid.DataTable.GetValue("U_Z_CompName", intRow)
                    oCompTable.UserFields.Fields.Item("U_Z_Weight").Value = oGrid.DataTable.GetValue("U_Z_Weight", intRow)
                    oCompTable.UserFields.Fields.Item("U_Z_CompLevel").Value = oGrid.DataTable.GetValue("U_Z_CompLevel", intRow)
                    If oCompTable.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                Else
                    strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                    oCompTable.Code = strCode
                    oCompTable.Name = strCode + "_N"
                    oCompTable.UserFields.Fields.Item("U_Z_HREmpID").Value = strEmpId
                    oCompTable.UserFields.Fields.Item("U_Z_CompCode").Value = oGrid.DataTable.GetValue("U_Z_CompCode", intRow)
                    oCompTable.UserFields.Fields.Item("U_Z_CompName").Value = oGrid.DataTable.GetValue("U_Z_CompName", intRow)
                    oCompTable.UserFields.Fields.Item("U_Z_Weight").Value = oGrid.DataTable.GetValue("U_Z_Weight", intRow)
                    oCompTable.UserFields.Fields.Item("U_Z_CompLevel").Value = oGrid.DataTable.GetValue("U_Z_CompLevel", intRow)
                    If oCompTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
            End If
        Next

        'Addition of Objects on Loan
        oGrid = aForm.Items.Item("HRgrdOLoan").Specific
        strTable = "@Z_HR_OBJLOAN"
        For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
            strCode = oGrid.DataTable.GetValue("Code", intRow)
            oEditTextColumn = oGrid.Columns.Item("U_Z_ObjCode")
            Try
                strType = oEditTextColumn.GetText(oGrid.DataTable.Rows.Count - 1).ToString
            Catch ex As Exception
                strType = ""
            End Try

            If strType <> "" Then
                If ObjLoanTable.GetByKey(strCode) Then
                    ObjLoanTable.Code = strCode
                    ObjLoanTable.Name = strCode
                    ObjLoanTable.UserFields.Fields.Item("U_Z_HREmpID").Value = strEmpId
                    ObjLoanTable.UserFields.Fields.Item("U_Z_ObjCode").Value = oGrid.DataTable.GetValue("U_Z_ObjCode", intRow)
                    ObjLoanTable.UserFields.Fields.Item("U_Z_ObjName").Value = oGrid.DataTable.GetValue("U_Z_ObjName", intRow)
                    ObjLoanTable.UserFields.Fields.Item("U_Z_Remarks").Value = oGrid.DataTable.GetValue("U_Z_Remarks", intRow)
                    ObjLoanTable.UserFields.Fields.Item("U_Z_ResID").Value = oGrid.DataTable.GetValue("U_Z_ResID", intRow)
                    ObjLoanTable.UserFields.Fields.Item("U_Z_ResName").Value = oGrid.DataTable.GetValue("U_Z_ResName", intRow)
                    oComboColumn = oGrid.Columns.Item("U_Z_Dept")
                    ObjLoanTable.UserFields.Fields.Item("U_Z_Dept").Value = oComboColumn.GetSelectedValue(intRow).Value
                    If ObjLoanTable.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                Else
                    strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                    ObjLoanTable.Code = strCode
                    ObjLoanTable.Name = strCode + "_N"
                    ObjLoanTable.UserFields.Fields.Item("U_Z_HREmpID").Value = strEmpId
                    ObjLoanTable.UserFields.Fields.Item("U_Z_ObjCode").Value = oGrid.DataTable.GetValue("U_Z_ObjCode", intRow)
                    ObjLoanTable.UserFields.Fields.Item("U_Z_ObjName").Value = oGrid.DataTable.GetValue("U_Z_ObjName", intRow)
                    ObjLoanTable.UserFields.Fields.Item("U_Z_Remarks").Value = oGrid.DataTable.GetValue("U_Z_Remarks", intRow)
                    ObjLoanTable.UserFields.Fields.Item("U_Z_ResID").Value = oGrid.DataTable.GetValue("U_Z_ResID", intRow)
                    ObjLoanTable.UserFields.Fields.Item("U_Z_ResName").Value = oGrid.DataTable.GetValue("U_Z_ResName", intRow)
                    oComboColumn = oGrid.Columns.Item("U_Z_Dept")
                    ObjLoanTable.UserFields.Fields.Item("U_Z_Dept").Value = oComboColumn.GetSelectedValue(intRow).Value
                    If ObjLoanTable.Add <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    End If
                End If
            End If
        Next



        Dim otes As SAPbobsCOM.Recordset
        otes = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otes.DoQuery("Delete from ""@Z_HR_PEOBJ1"" where ""Name"" like '%_XD'")
        otes.DoQuery("Delete from ""@Z_HR_ECOLVL"" where ""Name"" like '%_XD'")
        otes.DoQuery("Delete from ""@Z_HR_OBJLOAN"" where ""Name"" like '%_XD'")

        otes.DoQuery("Update  ""@Z_HR_PEOBJ1"" set Name=Code where ""Name"" like '%_N'")
        otes.DoQuery("Update ""@Z_HR_ECOLVL""   set Name=Code where ""Name"" like '%_N'")
        otes.DoQuery("Update ""@Z_HR_OBJLOAN""   set Name=Code where ""Name"" like '%_N'")
        oUserTable = Nothing
        Return True
    End Function
    Private Sub CommitTrans(ByVal aform As SAPbouiCOM.Form)
        Dim otes As SAPbobsCOM.Recordset
        otes = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otes.DoQuery("Delete from ""@Z_HR_PEOBJ1"" where ""Name"" like '%_N'")
        otes.DoQuery("Delete from ""@Z_HR_ECOLVL"" where ""Name"" like '%_N'")
        otes.DoQuery("Delete from ""@Z_HR_OBJLOAN"" where ""Name"" like '%_N'")

        otes.DoQuery("Update  ""@Z_HR_PEOBJ1"" set Name=Code where ""Name"" like '%_XD'")
        otes.DoQuery("Update ""@Z_HR_ECOLVL""   set Name=Code where ""Name"" like '%_XD'")
        otes.DoQuery("Update ""@Z_HR_OBJLOAN""   set Name=Code where ""Name"" like '%_XD'")
    End Sub

    Private Function AddToUDT_comp(ByVal aForm As SAPbouiCOM.Form, ByVal aEmpID As String, ByVal aCode As String, ByVal aName As String, ByVal aWeight As Double, ByVal aLevel As String, ByVal Postion As String) As Boolean
        Dim strTable, strEmpId, strCode, strType, strAccountCode As String
        Dim dblValue As Double
        Dim oUserTable, oCompTable, ObjLoanTable As SAPbobsCOM.UserTable
        Dim oValidateRS As SAPbobsCOM.Recordset
        strEmpId = aEmpID
        oUserTable = oApplication.Company.UserTables.Item("Z_HR_PEOBJ1")
        oCompTable = oApplication.Company.UserTables.Item("Z_HR_ECOLVL")
        ObjLoanTable = oApplication.Company.UserTables.Item("Z_HR_OBJLOAN")
        oValidateRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        'Addition of People Objective
        strTable = "@Z_HR_ECOLVL"
        strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
        oCompTable.Code = strCode
        oCompTable.Name = strCode + "_N"
        oCompTable.UserFields.Fields.Item("U_Z_HREmpID").Value = strEmpId
        oCompTable.UserFields.Fields.Item("U_Z_CompCode").Value = aCode
        oCompTable.UserFields.Fields.Item("U_Z_CompName").Value = aName
        oCompTable.UserFields.Fields.Item("U_Z_Weight").Value = aWeight
        oCompTable.UserFields.Fields.Item("U_Z_CompLevel").Value = aLevel
        oCompTable.UserFields.Fields.Item("U_Z_PosCode").Value = Postion
        If oCompTable.Add <> 0 Then
            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End If
        oUserTable = Nothing
        Return True
    End Function

#End Region

    Private Sub populateOrganizationDetails(ByVal aPosition As String, ByVal aform As SAPbouiCOM.Form)
        Dim oRec, oRec1 As SAPbobsCOM.Recordset
        Try
            aform.Freeze(True)
            Dim strSalarycode As String
            strSalarycode = ""

            oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRec.DoQuery("  select ""U_Z_PosCode"",""U_Z_PosName"",""U_Z_OrgCode"",""U_Z_OrgName"",""U_Z_CompCode"",""U_Z_CompName"",""U_Z_SalCode"",""U_Z_JobCode"",""U_Z_JobName"" ,""U_Z_DeptCode"" from ""@Z_HR_OPOSIN"" where ""U_Z_PosCode""='" & aPosition.Replace("'", "''") & "'")
             If oRec.RecordCount > 0 Then
                oApplication.Utilities.setEdittextvalue(aform, "HRedposi", oRec.Fields.Item("U_Z_PosCode").Value)
                oApplication.Utilities.setEdittextvalue(aform, "HRedpoNa", oRec.Fields.Item("U_Z_PosName").Value)

                oApplication.Utilities.setEdittextvalue(aform, "HRedComp", oRec.Fields.Item("U_Z_CompCode").Value)
                oApplication.Utilities.setEdittextvalue(aform, "HRedCoNa", oRec.Fields.Item("U_Z_CompName").Value)
                oRec1.DoQuery("Select * from [@Z_HR_OADM] where U_Z_CompCode='" & oRec.Fields.Item("U_Z_CompCode").Value & "'")
                oApplication.Utilities.setEdittextvalue(aform, "HRedCNa", oRec1.Fields.Item("U_Z_FrgnName").Value)

                oCombobox = aform.Items.Item("45").Specific
                Try
                    oCombobox.Select(oRec.Fields.Item("U_Z_DeptCode").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
                    oRec1.DoQuery("Select * from OUDP where Code='" & oRec.Fields.Item("U_Z_DeptCode").Value & "'")
                    oApplication.Utilities.setEdittextvalue(aform, "HRedDNa", oRec1.Fields.Item("U_Z_FrgnName").Value)
                Catch ex As Exception

                End Try

               
                oApplication.Utilities.setEdittextvalue(aform, "HRedJobSt", oRec.Fields.Item("U_Z_JobCode").Value)
                oApplication.Utilities.setEdittextvalue(aform, "HRedJobNa", oRec.Fields.Item("U_Z_JobName").Value)

                oApplication.Utilities.setEdittextvalue(aform, "HRedSal", oRec.Fields.Item("U_Z_SalCode").Value)
                strSalarycode = oRec.Fields.Item("U_Z_SalCode").Value

                Dim strQry As String
                strQry = "SELECT ""U_Z_PosCode"",""U_Z_PosName"",T0.[U_Z_OrgCode], T0.[U_Z_OrgDesc], T0.[U_Z_CompCode], T0.[U_Z_CompName], T0.[U_Z_DeptCode], T0.[U_Z_DeptName], T0.[U_Z_FuncCode], T0.[U_Z_FuncName], T0.[U_Z_LocCode], T0.[U_Z_LocName], T0.[U_Z_UnitName], T0.[U_Z_BranName], T0.[U_Z_SecName] FROM [dbo].[@Z_HR_ORGST]  T0 where ""U_Z_PosCode""='" & aPosition.Replace("'", "''") & "'"
                oRec.DoQuery(strQry)
                oApplication.Utilities.setEdittextvalue(aform, "HRedOrgSt", oRec.Fields.Item("U_Z_OrgCode").Value)
                oApplication.Utilities.setEdittextvalue(aform, "HRedOrgNa", oRec.Fields.Item("U_Z_OrgDesc").Value)
                oApplication.Utilities.setEdittextvalue(aform, "HRedLoc", oRec.Fields.Item("U_Z_LocCode").Value)
                oApplication.Utilities.setEdittextvalue(aform, "HRedLcNa", oRec.Fields.Item("U_Z_LocName").Value)
                oApplication.Utilities.setEdittextvalue(aform, "HRedDiv", oRec.Fields.Item("U_Z_FuncCode").Value)
                oApplication.Utilities.setEdittextvalue(aform, "HRedDivNa", oRec.Fields.Item("U_Z_FuncName").Value)


                oApplication.Utilities.setEdittextvalue(aform, "HRedUnSt", oRec.Fields.Item("U_Z_UnitName").Value)
                oApplication.Utilities.setEdittextvalue(aform, "HRedScNa", oRec.Fields.Item("U_Z_SecName").Value)
                oApplication.Utilities.setEdittextvalue(aform, "HRedJobNa2", oRec.Fields.Item("U_Z_BranName").Value)
               Else
                oApplication.Utilities.setEdittextvalue(aform, "HRedposi", "")
                oApplication.Utilities.setEdittextvalue(aform, "HRedpoNa", "")

                oApplication.Utilities.setEdittextvalue(aform, "HRedComp", "")
                oApplication.Utilities.setEdittextvalue(aform, "HRedCoNa", "")

                oApplication.Utilities.setEdittextvalue(aform, "HRedCNa", "")
                oApplication.Utilities.setEdittextvalue(aform, "HRedDNa", "")

                oApplication.Utilities.setEdittextvalue(aform, "HRedOrgSt", "")
                oApplication.Utilities.setEdittextvalue(aform, "HRedOrgNa", "")

                oApplication.Utilities.setEdittextvalue(aform, "HRedJobSt", "")
                oApplication.Utilities.setEdittextvalue(aform, "HRedJobNa", "")
                oApplication.Utilities.setEdittextvalue(aform, "HRedSal", "")
              

                oApplication.Utilities.setEdittextvalue(aform, "HRedLoc", "")
                oApplication.Utilities.setEdittextvalue(aform, "HRedLcNa", "")

                oApplication.Utilities.setEdittextvalue(aform, "HRedUnSt", "")
                oApplication.Utilities.setEdittextvalue(aform, "HRedScNa", "")
                oApplication.Utilities.setEdittextvalue(aform, "HRedJobNa2", "")

                oApplication.Utilities.setEdittextvalue(aform, "HRedDiv", "")
                oApplication.Utilities.setEdittextvalue(aform, "HRedDivNa", "")

                strSalarycode = ""
            End If
            If strSalarycode <> "" Then
                oRec.DoQuery("SELECT T0.""U_Z_SalCode"", T0.""U_Z_LevlCode"", T0.""U_Z_LevlName"", T0.""U_Z_GrdeCode"", T0.""U_Z_GrdeName"" FROM ""@Z_HR_OSALST""  T0 where T0.U_Z_SalCode='" & strSalarycode & "'")
                If oRec.RecordCount > 0 Then
                    oApplication.Utilities.setEdittextvalue(aform, "HRedSal", oRec.Fields.Item("U_Z_SalCode").Value)
                    oApplication.Utilities.setEdittextvalue(aform, "HRedLvl", oRec.Fields.Item("U_Z_LevlCode").Value)
                    oApplication.Utilities.setEdittextvalue(aform, "HRedLvlNa", oRec.Fields.Item("U_Z_LevlName").Value)
                    oApplication.Utilities.setEdittextvalue(aform, "HRedgrd", oRec.Fields.Item("U_Z_GrdeCode").Value)
                    oApplication.Utilities.setEdittextvalue(aform, "HRedgrNa", oRec.Fields.Item("U_Z_GrdeName").Value)
                Else

                    oApplication.Utilities.setEdittextvalue(aform, "HRedSal", "")
                    oApplication.Utilities.setEdittextvalue(aform, "HRedLvl", "")
                    oApplication.Utilities.setEdittextvalue(aform, "HRedLvlNa", "")
                    oApplication.Utilities.setEdittextvalue(aform, "HRedgrd", "")
                    oApplication.Utilities.setEdittextvalue(aform, "HRedgrNa", "")
                End If
            Else
                oApplication.Utilities.setEdittextvalue(aform, "HRedSal", "")
                oApplication.Utilities.setEdittextvalue(aform, "HRedLvl", "")
                oApplication.Utilities.setEdittextvalue(aform, "HRedLvlNa", "")
                oApplication.Utilities.setEdittextvalue(aform, "HRedgrd", "")
                oApplication.Utilities.setEdittextvalue(aform, "HRedgrNa", "")
            End If
          

            aform.Freeze(False)
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            aform.Freeze(False)

        End Try
    End Sub
    Private Sub PopulateCompetence(ByVal sform As SAPbouiCOM.Form, ByVal Poscode As String)
        Dim strSQL, strSQL1, aEmpID As String
        Dim oRec, oRectemp, otemp As SAPbobsCOM.Recordset
        Dim oMatrix As SAPbouiCOM.Matrix
        Dim oCombobox As SAPbouiCOM.ComboBox
        Dim intJobCode, strLevel As String
        aEmpID = oApplication.Utilities.getEdittextvalue(sform, "33")
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRectemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRec.DoQuery("select U_Z_JobCode  from [@Z_HR_OPOSIN] where U_Z_PosCode='" & Poscode & "'")
        If oRec.RecordCount > 0 Then
            intJobCode = oRec.Fields.Item("U_Z_JobCode").Value
            oGrid = oForm.Items.Item("HRgrdcom").Specific
            oGrid.DataTable = oForm.DataSources.DataTables.Item("HRdtCom")
            strSQL1 = "select ' ' 'Code',' ' 'Name', U_Z_CompCode,U_Z_CompDesc,U_Z_Weight,U_Z_CompLevel  from [@Z_HR_POSCO1] T0 inner join [@Z_HR_OPOSCO] T1 on T1.DocEntry=T0.DocEntry "
            strSQL1 = strSQL1 & " where T1.U_Z_PosCode='" & intJobCode & "'"
            otemp.DoQuery(strSQL1)
            oRectemp.DoQuery("Update [@Z_HR_ECOLVL] set Name=Name +'_XD' where isnull(U_Z_PosCode,'') <>'" & Poscode & "'")

            For intRow As Integer = 0 To otemp.RecordCount - 1
                oRectemp.DoQuery("Select * from [@Z_HR_ECOLVL] where Name not like '%_XD' and  U_Z_HREmpID='" & aEmpID & "' and U_Z_CompCode='" & otemp.Fields.Item("U_Z_CompCode").Value & "'")
                If oRectemp.RecordCount <= 0 Then
                    AddToUDT_comp(sform, aEmpID, otemp.Fields.Item("U_Z_CompCode").Value, otemp.Fields.Item("U_Z_CompDesc").Value, otemp.Fields.Item("U_Z_Weight").Value, otemp.Fields.Item("U_Z_CompLevel").Value, Poscode)
                End If
                otemp.MoveNext()
            Next
            oGrid = sform.Items.Item("HRgrdcom").Specific
            oGrid.DataTable = sform.DataSources.DataTables.Item("HRdtCom")
            oGrid.DataTable.ExecuteQuery("Select * from ""@Z_HR_ECOLVL"" where name not like '%_XD' and  ""U_Z_HREmpID"" ='" & aEmpID & "'")
            oGrid.Columns.Item("Code").Visible = False
            oGrid.Columns.Item("Name").Visible = False
            oGrid.Columns.Item("U_Z_HREmpID").Visible = False
            oGrid.Columns.Item("U_Z_CompCode").TitleObject.Caption = "Competency Code"
            oGrid.Columns.Item("U_Z_CompCode").Editable = False
            oGrid.Columns.Item("U_Z_CompName").TitleObject.Caption = "Competency Description"
            oGrid.Columns.Item("U_Z_CompName").Editable = False
            oGrid.Columns.Item("U_Z_Weight").TitleObject.Caption = "Weight"
            oGrid.Columns.Item("U_Z_Weight").Editable = False

            oGrid.Columns.Item("U_Z_PosCode").TitleObject.Caption = "Position Name"
            oGrid.Columns.Item("U_Z_PosCode").Editable = False
            oGrid.Columns.Item("U_Z_CompLevel").TitleObject.Caption = "Current Level"
            oGrid.Columns.Item("U_Z_CompLevel").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
            oComboColumn = oGrid.Columns.Item("U_Z_CompLevel")
            oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRecSet.DoQuery("Select ""U_Z_LvelCode"" As ""Code"",""U_Z_LvelName"" As ""Name"" From ""@Z_HR_COLVL""")
            oComboColumn.ValidValues.Add("", "")
            If Not oRecSet.EoF Then
                For index As Integer = 0 To oRecSet.RecordCount - 1
                    If Not oRecSet.EoF Then
                        oComboColumn.ValidValues.Add(oRecSet.Fields.Item("Code").Value, oRecSet.Fields.Item("Name").Value)
                        oRecSet.MoveNext()
                    End If
                Next
            End If
            oComboColumn.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
            oEditTextColumn = oGrid.Columns.Item("U_Z_CompCode")
            oEditTextColumn.ChooseFromListUID = "CFL_HR_6"
            oEditTextColumn.ChooseFromListAlias = "U_Z_CompCode"
            oGrid.AutoResizeColumns()
            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
        Else
            LoadGridValues(sform, "LOAD")
        End If
    End Sub

#Region "Item Event"
    Public Overrides Sub ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean)
        Try
            If pVal.FormTypeEx = frm_hr_EmpMaster Then
                Select Case pVal.BeforeAction
                    Case True
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                            Case SAPbouiCOM.BoEventTypes.et_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "2" And (oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Or oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE) Then
                                    strEmpID = oForm.Items.Item("33").Specific.value
                                    If AddToUDT(oForm) = False Then
                                        BubbleEvent = False
                                        Exit Sub
                                    Else
                                        blnFlag = False
                                    End If
                                End If
                                If pVal.ItemUID = "1" Then
                                    CommitTrans(oForm)
                                End If
                                If pVal.ItemUID = "2" And oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                    blnFlag = True
                                End If
                                If pVal.ItemUID = "HRstAppLk" Then
                                    Dim strcode As String = oApplication.Utilities.getEdittextvalue(oForm, "HRedAppId")
                                    Dim ooBj As New clshrCrApplicants
                                    ooBj.ViewCandidate(strcode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_DOUBLE_CLICK
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "HRedJobSt" Then
                                    Dim strCode As String = oApplication.Utilities.getEdittextvalue(oForm, "HRedJobSt")
                                    oApplication.Utilities.OpenMasterinLink(oForm, "JobScreen", strCode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "HRedSal" Then
                                    Dim strCode As String = oApplication.Utilities.getEdittextvalue(oForm, "HRedSal")
                                    oApplication.Utilities.OpenMasterinLink(oForm, "Salary", strCode)
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "HRedOrgSt" Then
                                    oApplication.Utilities.OpenMasterinLink(oForm, "OrgStructure")
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "HRedDiv" Then
                                    oApplication.Utilities.OpenMasterinLink(oForm, "Function")
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "HRedComp" Then
                                    oApplication.Utilities.OpenMasterinLink(oForm, "Company")
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "HRedLoc" Then
                                    oApplication.Utilities.OpenMasterinLink(oForm, "Location")
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "HRedgrd" Then
                                    oApplication.Utilities.OpenMasterinLink(oForm, "Grade")
                                    BubbleEvent = False
                                    Exit Sub
                                End If
                                If pVal.ItemUID = "HRedLvl" Then
                                    oApplication.Utilities.OpenMasterinLink(oForm, "Level")
                                    BubbleEvent = False
                                    Exit Sub
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                '                                If pVal.CharPressed <> 9 And (pVal.ItemUID = "HRedpoNa" Or pVal.ItemUID = "HRedOrgSt" Or pVal.ItemUID = "HRedOrgNa" Or pVal.ItemUID = "HRedJobSt" Or pVal.ItemUID = "HRedJobNa" Or pVal.ItemUID = "HRedAppId" Or pVal.ItemUID = "HRedSal") Then
                                If pVal.CharPressed <> 9 And (pVal.ItemUID = "HRedCNa" Or pVal.ItemUID = "HRedDNa" Or pVal.ItemUID = "HRedUnSt" Or pVal.ItemUID = "HRedScNa" Or pVal.ItemUID = "HRedJobNa1" Or pVal.ItemUID = "HRedLoc" Or pVal.ItemUID = "HRedLcNa" Or pVal.ItemUID = "HRedDiv" Or pVal.ItemUID = "HRedDivNa" Or pVal.ItemUID = "HRedComp" Or pVal.ItemUID = "HRedCoNa" Or pVal.ItemUID = "HRedLcNa" Or pVal.ItemUID = "HRedLvl" Or pVal.ItemUID = "HRedLvlNa" Or pVal.ItemUID = "HRedgrd" Or pVal.ItemUID = "HRedgrNa" Or pVal.ItemUID = "HRedJobSt" Or pVal.ItemUID = "HRedSal" Or pVal.ItemUID = "HRedposi" Or pVal.ItemUID = "HRedpoNa" Or pVal.ItemUID = "HRedOrgSt" Or pVal.ItemUID = "HRedpoNa" Or pVal.ItemUID = "HRedOrgNa" Or pVal.ItemUID = "HRedJobNa" Or pVal.ItemUID = "HRedAppId") Then
                                    BubbleEvent = False
                                    Exit Sub
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_PICKER_CLICKED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.CharPressed <> 9 And (pVal.ItemUID = "HRedCNa" Or pVal.ItemUID = "HRedDNa" Or pVal.ItemUID = "HRedComp" Or pVal.ItemUID = "HRedCoNa" Or pVal.ItemUID = "HRedLcNa" Or pVal.ItemUID = "HRedLvl" Or pVal.ItemUID = "HRedLvlNa" Or pVal.ItemUID = "HRedgrd" Or pVal.ItemUID = "HRedgrNa" Or pVal.ItemUID = "HRedJobSt" Or pVal.ItemUID = "HRedSal" Or pVal.ItemUID = "HRedposi" Or pVal.ItemUID = "HRedpoNa" Or pVal.ItemUID = "HRedOrgSt" Or pVal.ItemUID = "HRedpoNa" Or pVal.ItemUID = "HRedOrgNa" Or pVal.ItemUID = "HRedJobNa" Or pVal.ItemUID = "HRedAppId") Then
                                    BubbleEvent = False
                                    Exit Sub
                                End If

                        End Select

                    Case False
                        Select Case pVal.EventType
                            Case SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                ' oItem1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
                                oForm.Freeze(True)
                                If AddControls(oForm) = True Then
                                    LoadGridValues(oForm, "NAVIGATION")
                                End If
                                oForm.Freeze(False)
                            Case SAPbouiCOM.BoEventTypes.et_KEY_DOWN
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)

                                If pVal.ItemUID = "HRgrdOLoan" And pVal.ColUID = "U_Z_ResID" And pVal.CharPressed = 9 Then
                                    oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                    Dim strIns As String
                                    oGrid = oForm.Items.Item("HRgrdOLoan").Specific
                                    Dim objChooseForm As SAPbouiCOM.Form
                                    Dim objChoose As New clsChooseFromList
                                    clsChooseFromList.ItemUID = pVal.ItemUID
                                    clsChooseFromList.SourceFormUID = FormUID
                                    clsChooseFromList.SourceLabel = 0
                                    clsChooseFromList.CFLChoice = "ResponseEmployee" 'oCombo.Selected.Value
                                    clsChooseFromList.choice = "ResEmp"
                                    clsChooseFromList.Documentchoice = "ResponseEmployee"
                                    'clsChooseFromList.ItemCode = oApplication.Utilities.getEdittextvalue(oForm, "13")
                                    oComboColumn = oGrid.Columns.Item("U_Z_Dept")
                                    Try
                                        clsChooseFromList.BinDescrUID = oComboColumn.GetSelectedValue(pVal.Row).Value
                                    Catch ex As Exception
                                        clsChooseFromList.BinDescrUID = "x"
                                    End Try

                                    clsChooseFromList.sourceColumID = pVal.ColUID
                                    clsChooseFromList.SourceLabel = pVal.Row
                                    oApplication.Utilities.LoadForm("\CFL.xml", frm_ChoosefromList)
                                    objChooseForm = oApplication.SBO_Application.Forms.ActiveForm()
                                    objChoose.databound(objChooseForm)
                                End If

                            Case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                If pVal.ItemUID = "44" Then
                                    oForm.Items.Item("HRedposi").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                    oCombobox = oForm.Items.Item("44").Specific
                                    'oApplication.Utilities.setEdittextvalue(oForm, "HRedposi", oCombobox.Selected.Description)
                                    'oApplication.SBO_Application.SendKeys("{TAB}")
                                    Dim orec As SAPbobsCOM.Recordset
                                    orec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                                    orec.DoQuery("Select name from OHPS where posid=" & oCombobox.Selected.Value)

                                    populateOrganizationDetails(orec.Fields.Item(0).Value, oForm)
                                    PopulateCompetence(oForm, orec.Fields.Item(0).Value)
                                End If
                            Case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED
                                oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                                Dim empid, empname, poscode, posName As String
                                Select Case pVal.ItemUID
                                    Case "fldHR"
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = 22
                                        oForm.Items.Item("HRFldHr1").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                                        oForm.Freeze(False)
                                        'oForm.PaneLevel = 18
                                        'Case "HRFldHr0"
                                        '    oForm.PaneLevel = 18
                                    Case "HRFldHr1"
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = 22
                                        oForm.Freeze(False)
                                    Case "HRFldHr2"
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = 23
                                        oForm.Freeze(False)
                                    Case "HRFldHr3"
                                        oForm.Freeze(True)
                                        Dim itnMode As Integer = oForm.Mode
                                        oForm.PaneLevel = 24
                                        oForm.Mode = itnMode

                                        oForm.Freeze(False)
                                    Case "HRFldHr4"
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = 25
                                        oForm.Freeze(False)
                                    Case "HRFldHr5"
                                        oForm.Freeze(True)
                                        oForm.PaneLevel = 26
                                        oForm.Freeze(False)
                                    Case "HRbtnAdd"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                            AddRow(oForm)
                                        End If
                                    Case "HRbtnDel"
                                        If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                            DeleteRow(oForm)
                                        End If
                                    Case "HRbtnTrain"
                                        'empid = oApplication.Utilities.getEdittextvalue(oForm, "33")
                                        'empname = oApplication.Utilities.getEdittextvalue(oForm, "38")
                                        'poscode = oApplication.Utilities.getEdittextvalue(oForm, "HRedposi")
                                        'posName = oApplication.Utilities.getEdittextvalue(oForm, "HRedpoNa")
                                        'Dim objct As New clshrEmpTraining
                                        'objct.LoadForm(empid, empname, poscode, posName)
                                    Case "2"
                                        If pVal.Action_Success And (oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE Or oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE) Then
                                            updateSetUp(strEmpID)
                                        End If
                                End Select


                            Case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST
                                Dim oCFLEvento As SAPbouiCOM.IChooseFromListEvent
                                Dim oCFL As SAPbouiCOM.ChooseFromList
                                Dim val1, val2, val3 As String
                                Dim sCHFL_ID, val, val4, val5, val6 As String
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
                                        If pVal.ItemUID = "edComp" Then
                                            val1 = oDataTable.GetValue("U_Z_CompCode", 0)
                                            val = oDataTable.GetValue("U_Z_CompName", 0)
                                            If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                                If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                End If
                                            End If
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "edCoNa", val)
                                                oApplication.Utilities.setEdittextvalue(oForm, "edComp", val1)
                                            Catch ex As Exception
                                                '  oForm.Freeze(False)
                                            End Try
                                        End If

                                        If pVal.ItemUID = "HRedOrgSt" Then

                                            Try
                                                val = oDataTable.GetValue("U_Z_OrgCode", 0)
                                                val1 = oDataTable.GetValue("U_Z_OrgDesc", 0)
                                                oApplication.Utilities.setEdittextvalue(oForm, "HRedOrgNa", val1)
                                                oApplication.Utilities.setEdittextvalue(oForm, "HRedOrgSt", val)
                                            Catch ex As Exception

                                            End Try

                                        End If
                                        If pVal.ItemUID = "HRedSal" Then

                                            Try
                                                val = oDataTable.GetValue("U_Z_SalCode", 0)
                                                oApplication.Utilities.setEdittextvalue(oForm, "HRedSal", val)
                                            Catch ex As Exception
                                            End Try

                                        End If
                                        If pVal.ItemUID = "HRedLvl" Then

                                            Try
                                                val = oDataTable.GetValue("U_Z_LvelCode", 0)
                                                val1 = oDataTable.GetValue("U_Z_LvelName", 0)
                                                oApplication.Utilities.setEdittextvalue(oForm, "HRedLvlNa", val1)
                                                oApplication.Utilities.setEdittextvalue(oForm, "HRedLvl", val)
                                            Catch ex As Exception

                                            End Try
                                        End If

                                        If pVal.ItemUID = "HRedgrd" Then

                                            Try
                                                val = oDataTable.GetValue("U_Z_GrdeCode", 0)
                                                val1 = oDataTable.GetValue("U_Z_GrdeName", 0)
                                                oApplication.Utilities.setEdittextvalue(oForm, "HRedgrNa", val1)
                                                oApplication.Utilities.setEdittextvalue(oForm, "HRedgrd", val)
                                            Catch ex As Exception
                                            End Try
                                        End If

                                        If pVal.ItemUID = "HRedLoc" Then
                                            Dim strval As String
                                            val = oDataTable.GetValue("U_Z_LocCode", 0)
                                            val1 = oDataTable.GetValue("U_Z_LocName", 0)
                                            Try
                                                oApplication.Utilities.setEdittextvalue(oForm, "HRedLcNa", val1)
                                                oApplication.Utilities.setEdittextvalue(oForm, "HRedLoc", val)
                                            Catch ex As Exception
                                            End Try

                                        End If
                                        If pVal.ItemUID = "HRedJobSt" Then
                                            Try
                                                val2 = oDataTable.GetValue("U_Z_PosCode", 0)
                                                val3 = oDataTable.GetValue("U_Z_PosName", 0)

                                                oApplication.Utilities.setEdittextvalue(oForm, "HRedJobNa", val3)
                                                oApplication.Utilities.setEdittextvalue(oForm, "HRedJobSt", val2)
                                            Catch ex As Exception

                                            End Try
                                        End If

                                        If pVal.ItemUID = "HRedposi" Then

                                            Try
                                                val1 = oDataTable.GetValue("U_Z_PosCode", 0)
                                                val = oDataTable.GetValue("U_Z_PosName", 0)
                                                val2 = oDataTable.GetValue("U_Z_JobCode", 0)
                                                val3 = oDataTable.GetValue("U_Z_JobName", 0)
                                                val4 = oDataTable.GetValue("U_Z_OrgCode", 0)
                                                val5 = oDataTable.GetValue("U_Z_OrgName", 0)
                                                val6 = oDataTable.GetValue("U_Z_SalCode", 0)

                                                oApplication.Utilities.setEdittextvalue(oForm, "HRedSal", val6)
                                                oApplication.Utilities.setEdittextvalue(oForm, "HRedpoNa", val)
                                                oApplication.Utilities.setEdittextvalue(oForm, "HRedJobSt", val2)
                                                oApplication.Utilities.setEdittextvalue(oForm, "HRedJobNa", val3)
                                                oApplication.Utilities.setEdittextvalue(oForm, "HRedOrgSt", val4)
                                                oApplication.Utilities.setEdittextvalue(oForm, "HRedOrgNa", val5)
                                                oApplication.Utilities.setEdittextvalue(oForm, "HRedposi", val1)
                                            Catch ex As Exception

                                            End Try

                                            oApplication.Utilities.setEdittextvalue(oForm, "HRedSal", oDataTable.GetValue("U_Z_SalCode", 0))

                                        End If

                                        If pVal.ColUID = "U_Z_HRPeoobjCode" Then

                                            Try
                                                val = oDataTable.GetValue("U_Z_PeoobjCode", 0)
                                                val1 = oDataTable.GetValue("U_Z_PeoobjName", 0)
                                                val2 = oDataTable.GetValue("U_Z_PeoCategory", 0)
                                                val3 = oDataTable.GetValue("U_Z_Weight", 0)
                                                oGrid = oForm.Items.Item("HRgrdpeo").Specific
                                                If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                                    If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                                        oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                    End If
                                                End If

                                                oGrid.DataTable.SetValue("U_Z_HRPeoobjName", pVal.Row, val1)
                                                oGrid.DataTable.SetValue("U_Z_HRPeoCategory", pVal.Row, val2)
                                                oGrid.DataTable.SetValue("U_Z_HRWeight", pVal.Row, val3)
                                                oGrid.DataTable.SetValue("U_Z_HRPeoobjCode", pVal.Row, val)
                                            Catch ex As Exception

                                            End Try
                                        End If

                                        If pVal.ColUID = "U_Z_CompCode" Then
                                            Try
                                                val = oDataTable.GetValue("U_Z_CompCode", 0)
                                                val1 = oDataTable.GetValue("U_Z_CompName", 0)
                                                val3 = oDataTable.GetValue("U_Z_Weight", 0)
                                                oGrid = oForm.Items.Item("HRgrdcom").Specific
                                                If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                                    If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                                        oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                    End If
                                                End If
                                                oGrid.DataTable.SetValue("U_Z_CompCode", pVal.Row, val)
                                                oGrid.DataTable.SetValue("U_Z_CompName", pVal.Row, val1)
                                                oGrid.DataTable.SetValue("U_Z_Weight", pVal.Row, val3)
                                            Catch ex As Exception

                                            End Try
                                        End If

                                        If pVal.ColUID = "U_Z_ObjCode" Then
                                            Try
                                                val = oDataTable.GetValue("ItemCode", 0)
                                                val1 = oDataTable.GetValue("ItemName", 0)
                                                oGrid = oForm.Items.Item("HRgrdOLoan").Specific
                                                If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                                    If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                                        oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                    End If
                                                End If
                                                Try
                                                    oComboColumn = oGrid.Columns.Item("U_Z_Dept")
                                                    oComboColumn.SetSelectedValue(pVal.Row, oDataTable.GetValue("U_Z_Dept", 0))

                                                Catch ex As Exception

                                                End Try

                                                oGrid.DataTable.SetValue("U_Z_ObjName", pVal.Row, val1)
                                                oGrid.DataTable.SetValue("U_Z_ObjCode", pVal.Row, val)
                                            Catch ex As Exception

                                            End Try
                                        End If

                                        If pVal.ColUID = "U_Z_ResID" Then
                                            Try
                                                val = oDataTable.GetValue("empID", 0)
                                                val1 = oDataTable.GetValue("firstName", 0) & " " & oDataTable.GetValue("middleName", 0) & " " & oDataTable.GetValue("lastName", 0)
                                                oGrid = oForm.Items.Item("HRgrdOLoan").Specific
                                                If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_ADD_MODE And oForm.Mode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                                                    If oForm.Mode <> SAPbouiCOM.BoFormMode.fm_UPDATE_MODE Then
                                                        oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE
                                                    End If
                                                End If

                                                oGrid.DataTable.SetValue("U_Z_ResName", pVal.Row, val1)
                                                oGrid.DataTable.SetValue("U_Z_ResID", pVal.Row, val)
                                            Catch ex As Exception

                                            End Try
                                        End If
                                    End If
                                    oForm.Freeze(False)

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
                Case mnu_hr_EmpMaster
                    'Case "Training"
                    '    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    '    Dim empid, empname, poscode, posName As String
                    '    empid = oApplication.Utilities.getEdittextvalue(oForm, "33")
                    '    empname = oApplication.Utilities.getEdittextvalue(oForm, "38")
                    '    poscode = oApplication.Utilities.getEdittextvalue(oForm, "HRedposi")
                    '    posName = oApplication.Utilities.getEdittextvalue(oForm, "HRedpoNa")
                    '    Dim objct As New clshrEmpTraining
                    '    oCombobox = oForm.Items.Item("45").Specific
                    '    objct.LoadForm(empid, empname, poscode, posName, oCombobox.Selected.Value, oCombobox.Selected.Description)
                Case "HRTRANSFER"
                    Dim oObj As New clsViewEmpDetails
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oObj.LoadForm(oForm, oApplication.Utilities.getEdittextvalue(oForm, "33"), "Transfer")
                Case "HRPROMOTION"
                    Dim oObj As New clsViewEmpDetails
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oObj.LoadForm(oForm, oApplication.Utilities.getEdittextvalue(oForm, "33"), "Promotion")
                Case "HRPOSITION"
                    Dim oObj As New clsViewEmpDetails
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oObj.LoadForm(oForm, oApplication.Utilities.getEdittextvalue(oForm, "33"), "Position")

                Case "HRNEWPROMOTION"
                    Dim oObj As New clshrPromotion
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oObj.LoadForm1(oForm, oApplication.Utilities.getEdittextvalue(oForm, "33"))

                Case "HRNEWTRANSFER"
                    Dim oObj As New clshrTransfer
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oObj.LoadForm1(oForm, oApplication.Utilities.getEdittextvalue(oForm, "33"))

                Case "HRNEWPOSITION"
                    Dim oObj As New clshrPostionChanges
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oObj.LoadForm1(oForm, oApplication.Utilities.getEdittextvalue(oForm, "33"))

                Case "HRASSAIGNTP"
                    Dim oObj As New clshrAssignTraPlan
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oObj.LoadForm(oForm, oApplication.Utilities.getEdittextvalue(oForm, "33"))

                Case mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS
                    ' oForm = oApplication.SBO_Application.Forms.Item(FormUID)
                    If pVal.BeforeAction = False Then
                        oForm = oApplication.SBO_Application.Forms.ActiveForm()
                        ' oForm.Items.Item("HRedpoNa").Enabled = False
                    End If

                Case mnu_ADD
                    oForm = oApplication.SBO_Application.Forms.ActiveForm()
                    oForm.Items.Item("HRedpoNa").Enabled = False
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
                oForm = oApplication.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID)
                If oForm.TypeEx = frm_hr_EmpMaster And BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True Then
                    LoadGridValues(oForm, "NAVIGATION")

                End If
            ElseIf BusinessObjectInfo.BeforeAction = False And BusinessObjectInfo.ActionSuccess = True And (BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD Or BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE) Then
                Dim oobj As SAPbobsCOM.EmployeesInfo
                Dim strcode As String
                oApplication.Company.GetNewObjectCode(strcode)
                oobj = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oEmployeesInfo)
                If 1 = 1 Then ' oobj.Browser.GetByKeys(BusinessObjectInfo.ObjectKey) Then
                    'oApplication.Utilities.addnewlogin(oobj.EmployeeID)
                    LoadGridValues(oForm, "NAVIGATION")
                End If
                'CommitTransaction("Add")
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Public Sub RightClickEvent(ByRef eventInfo As SAPbouiCOM.ContextMenuInfo, ByRef BubbleEvent As Boolean)
        oForm = oApplication.SBO_Application.Forms.Item(eventInfo.FormUID)
        'If eventInfo.FormUID = "RightClk" Then
        If oForm.TypeEx = frm_hr_EmpMaster Then
            If (eventInfo.BeforeAction = True) Then
                Dim oMenuItem As SAPbouiCOM.MenuItem
                Dim oMenus As SAPbouiCOM.Menus
                Try
                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                        Dim oCreationPackage As SAPbouiCOM.MenuCreationParams
                        oCreationPackage = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)
                        oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING
                        oCreationPackage.UniqueID = "HRTRANSFER"
                        oCreationPackage.String = "View Transfer Details"
                        oCreationPackage.Enabled = True
                        oMenuItem = oApplication.SBO_Application.Menus.Item("1280") 'Data'
                        oMenus = oMenuItem.SubMenus
                        oMenus.AddEx(oCreationPackage)

                        oCreationPackage = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)
                        oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING
                        oCreationPackage.UniqueID = "HRPROMOTION"
                        oCreationPackage.String = "View Promotion Details"
                        oCreationPackage.Enabled = True
                        oMenuItem = oApplication.SBO_Application.Menus.Item("1280") 'Data'
                        oMenus = oMenuItem.SubMenus
                        oMenus.AddEx(oCreationPackage)

                        oCreationPackage = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)
                        oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING
                        oCreationPackage.UniqueID = "HRPOSITION"
                        oCreationPackage.String = "View Position Changes Details"
                        oCreationPackage.Enabled = True
                        oMenuItem = oApplication.SBO_Application.Menus.Item("1280") 'Data'
                        oMenus = oMenuItem.SubMenus
                        oMenus.AddEx(oCreationPackage)

                        oCreationPackage = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)
                        oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING
                        oCreationPackage.UniqueID = "HRNEWPROMOTION"
                        oCreationPackage.String = "Employee New Promotion"
                        oCreationPackage.Enabled = True
                        oMenuItem = oApplication.SBO_Application.Menus.Item("1280") 'Data'
                        oMenus = oMenuItem.SubMenus
                        oMenus.AddEx(oCreationPackage)

                        oCreationPackage = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)
                        oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING
                        oCreationPackage.UniqueID = "HRNEWTRANSFER"
                        oCreationPackage.String = "Employee New Transfer"
                        oCreationPackage.Enabled = True
                        oMenuItem = oApplication.SBO_Application.Menus.Item("1280") 'Data'
                        oMenus = oMenuItem.SubMenus
                        oMenus.AddEx(oCreationPackage)

                        oCreationPackage = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)
                        oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING
                        oCreationPackage.UniqueID = "HRNEWPOSITION"
                        oCreationPackage.String = "Employee New Position"
                        oCreationPackage.Enabled = True
                        oMenuItem = oApplication.SBO_Application.Menus.Item("1280") 'Data'
                        oMenus = oMenuItem.SubMenus
                        oMenus.AddEx(oCreationPackage)

                        oCreationPackage = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)
                        oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING
                        oCreationPackage.UniqueID = "HRASSAIGNTP"
                        oCreationPackage.String = "Assign Travel Plan"
                        oCreationPackage.Enabled = True
                        oMenuItem = oApplication.SBO_Application.Menus.Item("1280") 'Data'
                        oMenus = oMenuItem.SubMenus
                        oMenus.AddEx(oCreationPackage)

                        'oCreationPackage = oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)
                        'oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING
                        'oCreationPackage.UniqueID = "Training"
                        'oCreationPackage.String = "Training Details"
                        'oCreationPackage.Enabled = True
                        'oMenuItem = oApplication.SBO_Application.Menus.Item("1280") 'Data'
                        'oMenus = oMenuItem.SubMenus
                        'oMenus.AddEx(oCreationPackage)
                    End If
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try
            Else
                Dim oMenuItem As SAPbouiCOM.MenuItem
                Dim oMenus As SAPbouiCOM.Menus
                Try
                    If oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE Then
                        'oApplication.SBO_Application.Menus.RemoveEx("Training")
                        oApplication.SBO_Application.Menus.RemoveEx("HRTRANSFER")
                        oApplication.SBO_Application.Menus.RemoveEx("HRPROMOTION")
                        oApplication.SBO_Application.Menus.RemoveEx("HRPOSITION")
                        oApplication.SBO_Application.Menus.RemoveEx("HRNEWPROMOTION")
                        oApplication.SBO_Application.Menus.RemoveEx("HRNEWTRANSFER")
                        oApplication.SBO_Application.Menus.RemoveEx("HRNEWPOSITION")
                        oApplication.SBO_Application.Menus.RemoveEx("HRASSAIGNTP")
                    End If

                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try
            End If
        End If
    End Sub

#Region "Update Authorize SetUp"
    Private Sub updateSetUp(ByVal strEmpID As String)
        Dim oTemp As SAPbobsCOM.Recordset
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        strQuery = "Update T0 Set ""U_Z_MGRREQUEST"" = T1.""U_Z_HR_ISReqAut"" From ""@Z_HR_LOGIN"" T0 JOIN OHEM T1 On T0.""U_Z_EMPID"" = T1.""EmpID"" Where T1.""EmpID"" ='" & strEmpID & "'"
        oTemp.DoQuery(strQuery)
    End Sub
#End Region

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class