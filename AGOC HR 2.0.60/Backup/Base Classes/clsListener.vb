Public Class clsListener
    Inherits Object
    Private ThreadClose As New Threading.Thread(AddressOf CloseApp)
    Private WithEvents _SBO_Application As SAPbouiCOM.Application
    Private _Company As SAPbobsCOM.Company
    Private _Utilities As clsUtilities
    Private _Collection As Hashtable
    Private _LookUpCollection As Hashtable
    Private _FormUID As String
    Private _Log As clsLog_Error
    Private oMenuObject As Object
    Private oItemObject As Object
    Private oSystemForms As Object
    Dim objFilters As SAPbouiCOM.EventFilters
    Dim objFilter As SAPbouiCOM.EventFilter

#Region "New"
    Public Sub New()
        MyBase.New()
        Try
            _Company = New SAPbobsCOM.Company
            _Utilities = New clsUtilities
            _Collection = New Hashtable(10, 0.5)
            _LookUpCollection = New Hashtable(10, 0.5)
            oSystemForms = New clsSystemForms
            _Log = New clsLog_Error

            SetApplication()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub
#End Region

#Region "Public Properties"

    Public ReadOnly Property SBO_Application() As SAPbouiCOM.Application
        Get
            Return _SBO_Application
        End Get
    End Property

    Public ReadOnly Property Company() As SAPbobsCOM.Company
        Get
            Return _Company
        End Get
    End Property

    Public ReadOnly Property Utilities() As clsUtilities
        Get
            Return _Utilities
        End Get
    End Property

    Public ReadOnly Property Collection() As Hashtable
        Get
            Return _Collection
        End Get
    End Property

    Public ReadOnly Property LookUpCollection() As Hashtable
        Get
            Return _LookUpCollection
        End Get
    End Property

    Public ReadOnly Property Log() As clsLog_Error
        Get
            Return _Log
        End Get
    End Property
#Region "Filter"

    Public Sub SetFilter(ByVal Filters As SAPbouiCOM.EventFilters)
        oApplication.SetFilter(Filters)
    End Sub
    Public Sub SetFilter()
        Try
            ''Form Load
            objFilters = New SAPbouiCOM.EventFilters

            objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED)
            objFilter.AddEx(frm_SalesOrder)

            objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_KEY_DOWN)

            objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_LOAD)
            objFilter.Add(frm_SalesOrder)
        Catch ex As Exception
            Throw ex
        End Try

    End Sub
#End Region

#End Region

#Region "Menu Event"

    Private Sub _SBO_Application_FormDataEvent(ByRef BusinessObjectInfo As SAPbouiCOM.BusinessObjectInfo, ByRef BubbleEvent As Boolean) Handles _SBO_Application.FormDataEvent
        Select Case BusinessObjectInfo.FormTypeEx
            'Case frm_Invoice, frm_InvSO
            '    Dim objInvoice As clsStockRequest
            '    objInvoice = New clsStockRequest
            '    objInvoice.FormDataEvent(BusinessObjectInfo, BubbleEvent)
        End Select
        '  End If

        If _Collection.ContainsKey(_FormUID) Then
            Dim objform As SAPbouiCOM.Form
            objform = oApplication.SBO_Application.Forms.ActiveForm()
            If 1 = 1 Then 'BusinessObjectInfo.FormTypeEx = frm_hr_EmpMaster Then
                oMenuObject = _Collection.Item(_FormUID)
                oMenuObject.FormDataEvent(BusinessObjectInfo, BubbleEvent)
            End If

        End If
    End Sub

    Private Sub SBO_Application_MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean) Handles _SBO_Application.MenuEvent
        Try
            If pVal.BeforeAction = False Then
                Select Case pVal.MenuUID

                    Case mnu_HRVariableEarning
                        oMenuObject = New clsHRVariableEarning
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_HR_LveApproval
                        oMenuObject = New clshrLeaveApproval
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_HR_LveRequest
                        oMenuObject = New clshrLeaveRequest
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_SheduleTrain
                        oMenuObject = New clshrEmpTraining
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_ASLMApproval
                        oMenuObject = New clshrShortApproval
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                        'Case mnu_hr_ASLMApproval
                        '    oMenuObject = New clshrASCanSelection
                        '    oMenuObject.MenuEvent(pVal, BubbleEvent)

                        'Case mnu_hr_IPHOD
                        '    oMenuObject = New clshrASCanSelectionIPSum
                        '    oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_IPHOD
                        oMenuObject = New clshrFinalApproval
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_RecHRApproval
                        oMenuObject = New clshrMPRApproval
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_MgrTrainApp
                        oMenuObject = New clshrTrainNewApproval
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_MgrRegTrainApproval
                        oMenuObject = New clshrTrainRegApproval
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_ExpApproval
                        oMenuObject = New clshrClaimApproval
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_TraApproval
                        oMenuObject = New clshrTravelApproval
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_DocType
                        oMenuObject = New clshrDocumentType
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_ApproveTemp
                        oMenuObject = New clshrApproveTemp
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_PayMethod
                        oMenuObject = New clshrPayMethod
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_LeaveMaster
                        oMenuObject = New clshrLeaveMaster
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_TrainQcCate
                        oMenuObject = New clshrTrainQcCategory
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_TrainQcItem
                        oMenuObject = New clshrTrainQcItem
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_TrainQcRA
                        oMenuObject = New clshrTrainQcRA
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_trainEvaluation
                        oMenuObject = New clshrTrainEvaluation
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_MgrEvaluation
                        oMenuObject = New clshrMgrTrainingEva
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_HR_EmpLifeposch
                        oMenuObject = New clshrEmpPosChangeApp
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_HR_EmpLifeApp
                        oMenuObject = New clshrEmpLifeApproval
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_HR_EmpLifePost
                        oMenuObject = New clshrEmpLifePosting
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_HR_UpdatePayroll
                        oMenuObject = New clsUpdatePayroll
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_RecReqReason
                        oMenuObject = New clshrRecReqReason
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_AppDisMaster
                        oMenuObject = New clshrAppraisalDistribution
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_ExitResponse
                        oMenuObject = New clshrExitResponse
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_Qustionaire
                        oMenuObject = New clshrExitQuestion
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_ExitfrmInit
                        oMenuObject = New clshrExitfrmInitialization
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_ExitProcess
                        oMenuObject = New clshrExitProcess
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_ExitInvForm
                        oMenuObject = New clshrExitInterview
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_ObjLoan
                        oMenuObject = New clshrObjLoan
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_Languages
                        oMenuObject = New clshrLanguages
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_Trainner
                        oMenuObject = New clshrTrainner
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_HRRegTrainApproval
                        oMenuObject = New clsHRRegTrainApproval
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_CompObj
                        oMenuObject = New clshrCompObjMaster
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_HRTrainApp
                        oMenuObject = New clshrHRTrainApp
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_Comp
                        oMenuObject = New clshrCompany
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_DeptMaster
                        oMenuObject = New clsDepartmentMaster
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_BranchMaster
                        oMenuObject = New clsBranchesMaster
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_CourseCategory
                        oMenuObject = New clshrCourseCategory
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_CourseType
                        oMenuObject = New clshrCourseType
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_TrainReg
                        oMenuObject = New clshrTrainingReg
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_AppAttendees
                        oMenuObject = New clsTrainApproved
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_Func
                        oMenuObject = New clshrFunction
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_Unit
                        oMenuObject = New clshrUnit
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_Loc
                        oMenuObject = New clshrLocation
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_OrgSt
                        oMenuObject = New clshrOrgStructure
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_GrdLvl
                        oMenuObject = New clshrGrade
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_Level
                        oMenuObject = New clshrLevel
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_Allow
                        oMenuObject = New clshrAllowance
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_Benefit
                        oMenuObject = New clshrBenefits
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_Ratings
                        oMenuObject = New clshrRating
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_BussObj
                        oMenuObject = New clshrBussObjective
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_PeoObj
                        oMenuObject = New clshrPeoObjective
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                        'Case mnu_hr_CompObj
                        '    oMenuObject = New clshrCompObjective
                        '    oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_PosComp
                        oMenuObject = New clshrPosCompetence
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_Logsetup
                        oMenuObject = New clshrLoginSetup
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_Course
                        oMenuObject = New clshrCourse
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_TrainPlan
                        oMenuObject = New clshrTrainPlan
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_SalStru
                        oMenuObject = New clshrSalStructure
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_PeoCategory
                        oMenuObject = New clshrPeoCategory
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_CompLevel
                        oMenuObject = New clshrCompLevel
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_DeptMapp
                        oMenuObject = New clshrDeptMapping
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_EmpMaster, "Training", "HRTRANSFER", "HRPROMOTION", "HRPOSITION", "HRNEWPROMOTION", "HRNEWTRANSFER", "HRNEWPOSITION", "HRASSAIGNTP"
                        oMenuObject = New clsHRModule
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_SelfAppr ', mnu_hr_MgrAppr, mnu_hr_HRAppr
                        oMenuObject = New clshrSelfAppraisal
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_CourseRev
                        oMenuObject = New clshrCourseReview
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_empPosition
                        oMenuObject = New clshrEmpPosition
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_MPRequest, mnu_hr_RecGMApproval, "CanList"
                        oMenuObject = New clshrMPRequest
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_CrApplicants, "IntList"
                        oMenuObject = New clshrCrApplicants
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_Search
                        oMenuObject = New clshrSearch
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                        ''2013-07-06
                    Case mnu_hr_Hiring
                        oMenuObject = New clshrHireToEmp
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_Promotion1
                        oMenuObject = New clshrPromotion
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_Transfer
                        oMenuObject = New clshrTransfer
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_PosChanges
                        oMenuObject = New clshrPostionChanges
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                        '2013-07-12
                    Case mnu_hr_Expenses
                        oMenuObject = New clshrExpenses
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_TraRequest, mnu_hr_ExpenseClaim
                        oMenuObject = New clshrTravelRequest
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_TraAgenda
                        oMenuObject = New clshrTravelAgenda
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                        'Case mnu_hr_TraApproval
                        '    oMenuObject = New clshrTravelApproval
                        '    oMenuObject.MenuEvent(pVal, BubbleEvent)
                        'Case mnu_hr_ExpenseClaim
                        '    oMenuObject = New clshrExpensesClaim
                        '    oMenuObject.MenuEvent(pVal, BubbleEvent)
                        'Case mnu_hr_ExpApproval
                        '    oMenuObject = New clshrExpensesAppr
                        '    oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_TraOverview, mnu_hr_ExpOverview
                        oMenuObject = New clshrTraExpOverView
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_IniAppraisal
                        oMenuObject = New clshrInitializeAppraisal
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                        'Case mnu_hr_MgrAppr
                        '    oMenuObject = New clshrApproval
                        '    oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_MgrAppr
                        oMenuObject = New clshrFApproval
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_SMgrAppr
                        oMenuObject = New clshrsApproval
                        oMenuObject.MenuEvent(pVal, BubbleEvent)


                    Case mnu_hr_HRAppr
                        oMenuObject = New ClshrSlctnCreteria
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_GAcceptance
                        oMenuObject = New ClshrSlctnCreteriaGA
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                        'Case mnu_hr_ASLMApproval, mnu_hr_ASSMApproval, mnu_hr_OAcceptance, mnu_hr_IPLM, mnu_hr_IPHOD, mnu_hr_IPHR, mnu_hr_IPLMU
                        '    oMenuObject = New clshrASCanSelection
                        '    oMenuObject.MenuEvent(pVal, BubbleEvent)



                    Case mnu_hr_ASSMApproval
                        oMenuObject = New clshrASCanSelectionSe
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_IPLM
                        oMenuObject = New clshrASCanSelectionIPLM
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_IPLMU
                        oMenuObject = New clshrASCanSelectionIPHOD
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_IPHR
                        oMenuObject = New clshrASCanSelectionIPHR
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_OAcceptance
                        oMenuObject = New clshrASCanSelectionGA
                        oMenuObject.MenuEvent(pVal, BubbleEvent)


                    Case mnu_hr_InterviewType
                        oMenuObject = New clshrInterviewType
                        oMenuObject.MenuEvent(pVal, BubbleEvent)


                    Case mnu_hr_RejectionMaster
                        oMenuObject = New clshrRejectionMaster
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_IntRatings
                        oMenuObject = New clshrIntRating
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_ORejectionMaster
                        oMenuObject = New clshrORejectionMaster
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_RecClosing
                        oMenuObject = New clshrRecClosing
                        oMenuObject.MenuEvent(pVal, BubbleEvent)
                    Case mnu_hr_RecOverview
                        oMenuObject = New clshrRecOverview
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_EmailSetUp
                        oMenuObject = New clshrEmailSetUp
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_AppraisalEmail
                        oMenuObject = New clshrAppraisalEmail
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_Sec
                        oMenuObject = New clshrSection
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case mnu_hr_RSta
                        oMenuObject = New clshrResidencyStatus
                        oMenuObject.MenuEvent(pVal, BubbleEvent)

                    Case "1283", mnu_ADD, mnu_FIND, mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS, mnu_ADD_ROW, mnu_DELETE_ROW
                        If _Collection.ContainsKey(_FormUID) Then
                            oMenuObject = _Collection.Item(_FormUID)
                            oMenuObject.MenuEvent(pVal, BubbleEvent)
                        End If

                End Select

            Else
                Select Case pVal.MenuUID
                    Case "5890"
                        'BubbleEvent = False
                        ' Exit Sub

                    Case mnu_CLOSE
                        If _Collection.ContainsKey(_FormUID) Then
                            oMenuObject = _Collection.Item(_FormUID)
                            oMenuObject.MenuEvent(pVal, BubbleEvent)
                        End If
                    Case "1283", mnu_ADD, mnu_FIND, mnu_FIRST, mnu_LAST, mnu_NEXT, mnu_PREVIOUS, mnu_ADD_ROW, mnu_DELETE_ROW
                        If _Collection.ContainsKey(_FormUID) Then
                            oMenuObject = _Collection.Item(_FormUID)
                            oMenuObject.MenuEvent(pVal, BubbleEvent)
                        End If
                End Select

            End If

        Catch ex As Exception
            Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
            oMenuObject = Nothing
        End Try
    End Sub
#End Region

#Region "Item Event"
    Private Sub SBO_Application_ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean) Handles _SBO_Application.ItemEvent
        Try
            _FormUID = FormUID

            If pVal.EventType <> SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD Then
                Select Case pVal.FormType
                End Select
            End If

            If pVal.EventType <> SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD Then
                Select Case pVal.FormTypeEx

                    Case frm_HRVariableEarning
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsHRVariableEarning
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_LeaveApproval
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrLeaveApproval
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_LveRequest
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrLeaveRequest
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_FinalApproval
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrFinalApproval
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_ShortApproval
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrShortApproval
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_MPRApproval
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrMPRApproval
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_TrainNewApproval
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrTrainNewApproval
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_TrainRegApproval
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrTrainRegApproval
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_AppHisDetails
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrAppHisDetails
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_ViewTraApp
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrViewTraRequest
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_ClaimApproval
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrClaimApproval
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_DocType
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrDocumentType
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_ApproveTemp
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrApproveTemp
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_DisRule
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsDisRule
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_PayMethod
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrPayMethod
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_LeaveMaster
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrLeaveMaster
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_TrainQcCa
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrTrainQcCategory
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_TrainQcItem
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrTrainQcItem
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_TrainQcRA
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrTrainQcRA
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_MgrEva
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrMgrTrainingEva
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_TrainEva
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrTrainEvaluation
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_CReqSelIPGA
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrASCanSelectionGA
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_CReqSelIPHR
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrASCanSelectionIPHR
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_CReqSelIPSUM
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrASCanSelectionIPSum
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_CReqSelIPHOD
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrASCanSelectionIPHOD
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_CReqSelIPLM
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrASCanSelectionIPLM
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_CReqSelectionSe
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrASCanSelectionSe
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_EmpPosChApp
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrEmpPosChangeApp
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_EmpLifeApp
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrEmpLifeApproval
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_EmpLifePost
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrEmpLifePosting
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_HR_UpdatePayroll
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsUpdatePayroll
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_RecReqReason
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrRecReqReason
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_AppDisMaster
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrAppraisalDistribution
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_ExitInvForm1
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrExitInterview
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_ExitProcess
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrExitProcess
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_ExitResponse
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrExitResponse
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_Qustionaire
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrExitQuestion
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_ExitfrmInit
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrExitfrmInitialization
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_ObjLoan
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrObjLoan
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_Languages1
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrLanguages
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_MgrRegTrainApproval
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrMgrRegTrainApproval
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_HRRegTrainApproval
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsHRRegTrainApproval
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_HR_Trainner
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrTrainner
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_EmpAbsSummary
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrEmpAbsSummary
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If


                    Case frm_hr_CompObjmaster
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrCompObjMaster
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_HRTrainApp
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrHRTrainApp
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_MgrTrainApp
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrMgrTrainApp
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_NewTrainReq
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrNewTrainRequest
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_Comp
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrCompany
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_ChoosefromList
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsChooseFromList
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_Department
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsDepartmentMaster
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_TrainReg
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrTrainingReg
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_Branches
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsBranchesMaster
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_CourseType
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrCourseType
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_CourseCategory
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrCourseCategory
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_AppAttendees
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsTrainApproved
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If


                    Case frm_hr_Func
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrFunction
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_Unit
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrUnit
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If


                    Case frm_hr_Loc
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrLocation
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_OrgSt
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrOrgStructure
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_GrdLvl
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrGrade
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_Level
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrLevel
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_Allow
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrAllowance
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_Benefit
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrBenefits
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_SalStru
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrSalStructure
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_Ratings
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrRating
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_BussObj
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrBussObjective
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_PeoObj
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrPeoObjective
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_CompObj
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrCompObjective
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_PosComp
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrPosCompetence
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_LogSetup
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrLoginSetup
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_Course
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrCourse
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_TrainPlan1
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrTrainPlan
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_PeoCategory
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrPeoCategory
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_CompLevel
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrCompLevel
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_DeptMapp
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrDeptMapping
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_EmpMaster
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsHRModule
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_SelfAppraisal
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrSelfAppraisal
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_Login
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrLogin
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_Approval
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrApproval
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_FApproval
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrFApproval
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_SApproval
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrsApproval
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_EmpTraining
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrEmpTraining
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_CourseRev
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrCourseReview
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_empPosition
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrEmpPosition
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_Hr_MPRequest
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrMPRequest
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_HR_CrtApplicants1
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrCrApplicants
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_RecApproval
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrRecApproval
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_HRecApproval
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrHRecApproval
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_HR_Search1
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrSearch
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_Candidate
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrCandidates
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                        ''2013-07-06
                    Case frm_hr_HireToEmp
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrHireToEmp
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_HR_Hiring
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrHiring
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_Promotion
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrPromotion
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_Transfer
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrTransfer
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_PosChanges
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrPostionChanges
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                        '2013-07-12

                    Case frm_hr_Expenses
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrExpenses
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_TraAgenda
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrTravelAgenda
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_TraRequest
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrTravelRequest
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_TravelApproval
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrTravelApproval
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_ExpenseClaim
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrExpClaimRequest
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                        'Case frm_hr_ExpenseClaim
                        '    If Not _Collection.ContainsKey(FormUID) Then
                        '        oItemObject = New clshrExpensesClaim
                        '        oItemObject.FrmUID = FormUID
                        '        _Collection.Add(FormUID, oItemObject)
                        '    End If
                        'Case frm_hr_ExpApproval
                        '    If Not _Collection.ContainsKey(FormUID) Then
                        '        oItemObject = New clshrExpensesAppr
                        '        oItemObject.FrmUID = FormUID
                        '        _Collection.Add(FormUID, oItemObject)
                        '    End If
                    Case frm_hr_AssignTraPlan
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrAssignTraPlan
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_AssExpenses
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrAssExpenses
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_TraExpOverView
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrTraExpOverView
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_IniAppraisal
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrInitializeAppraisal
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_SlctnCreteria
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New ClshrSlctnCreteria
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_SlctnCrGA
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New ClshrSlctnCreteriaGA
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_GAcceptance
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New ClshrGAcceptance
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_CReqSelection
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrASCanSelection
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_AppShortListed
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clsAppShortListed
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_IPProcessForm
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New ClshrIPProcessForm
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_InterviewType
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrInterviewType
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_RejectionMaster
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrRejectionMaster
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_IntRatings
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrIntRating
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_ORejectionMaster
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrORejectionMaster
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_RecClosing
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrRecClosing
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If
                    Case frm_hr_RecOverview
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrRecOverview
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_IPOfferAcceptance
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New ClshrIPOfferAcceptance
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_EmailSetUp
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrEmailSetUp
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_AppEmail
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrAppraisalEmail
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_Sec
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrSection
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                    Case frm_hr_RSta
                        If Not _Collection.ContainsKey(FormUID) Then
                            oItemObject = New clshrResidencyStatus
                            oItemObject.FrmUID = FormUID
                            _Collection.Add(FormUID, oItemObject)
                        End If

                End Select

            End If

            If _Collection.ContainsKey(FormUID) Then
                oItemObject = _Collection.Item(FormUID)
                If oItemObject.IsLookUpOpen And pVal.BeforeAction = True Then
                    _SBO_Application.Forms.Item(oItemObject.LookUpFormUID).Select()
                    BubbleEvent = False
                    Exit Sub
                End If
                'Dim oform As SAPbouiCOM.Form
                'oform = oApplication.SBO_Application.Forms.Item(FormUID)
                'If oApplication.Utilities.validateAuthorization(oApplication.Company.UserSignature, oform.TypeEx) = False Then
                '    oApplication.Utilities.Message("You are not authorized to do this action", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                '    BubbleEvent = False
                '    Exit Sub
                'End If
                _Collection.Item(FormUID).ItemEvent(FormUID, pVal, BubbleEvent)
            End If
            If pVal.EventType = SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD And pVal.BeforeAction = False Then
                If _LookUpCollection.ContainsKey(FormUID) Then
                    oItemObject = _Collection.Item(_LookUpCollection.Item(FormUID))
                    If Not oItemObject Is Nothing Then
                        oItemObject.IsLookUpOpen = False
                    End If
                    _LookUpCollection.Remove(FormUID)
                End If

                If _Collection.ContainsKey(FormUID) Then
                    _Collection.Item(FormUID) = Nothing
                    _Collection.Remove(FormUID)
                End If

            End If

        Catch ex As Exception
            Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
            GC.WaitForPendingFinalizers()
            GC.Collect()
        End Try
    End Sub
#End Region

#Region "Application Event"
    Private Sub SBO_Application_AppEvent(ByVal EventType As SAPbouiCOM.BoAppEventTypes) Handles _SBO_Application.AppEvent
        Try
            Select Case EventType
                Case SAPbouiCOM.BoAppEventTypes.aet_ShutDown, SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition, SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged
                    _Utilities.AddRemoveMenus("RemoveMenus.xml")
                    CloseApp()
            End Select
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Termination Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
        End Try
    End Sub
#End Region

#Region "Close Application"
    Private Sub CloseApp()
        Try
            If Not _SBO_Application Is Nothing Then
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_SBO_Application)
            End If

            If Not _Company Is Nothing Then
                If _Company.Connected Then
                    _Company.Disconnect()
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_Company)
            End If

            _Utilities = Nothing
            _Collection = Nothing
            _LookUpCollection = Nothing

            ThreadClose.Sleep(10)
            System.Windows.Forms.Application.Exit()
        Catch ex As Exception
            Throw ex
        Finally
            oApplication = Nothing
            GC.WaitForPendingFinalizers()
            GC.Collect()
        End Try
    End Sub
#End Region

#Region "Set Application"
    Private Sub SetApplication()
        Dim SboGuiApi As SAPbouiCOM.SboGuiApi
        Dim sConnectionString As String

        Try
            If Environment.GetCommandLineArgs.Length > 1 Then
                sConnectionString = Environment.GetCommandLineArgs.GetValue(1)
                SboGuiApi = New SAPbouiCOM.SboGuiApi
                SboGuiApi.Connect(sConnectionString)
                _SBO_Application = SboGuiApi.GetApplication()
            Else
                Throw New Exception("Connection string missing.")
            End If

        Catch ex As Exception
            Throw ex
        Finally
            SboGuiApi = Nothing
        End Try
    End Sub
#End Region

#Region "Finalize"
    Protected Overrides Sub Finalize()
        Try
            MyBase.Finalize()
            '            CloseApp()

            oMenuObject = Nothing
            oItemObject = Nothing
            oSystemForms = Nothing

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Addon Termination Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
        Finally
            GC.WaitForPendingFinalizers()
            GC.Collect()
        End Try
    End Sub
#End Region

    Private Sub _SBO_Application_RightClickEvent(ByRef eventInfo As SAPbouiCOM.ContextMenuInfo, ByRef BubbleEvent As Boolean) Handles _SBO_Application.RightClickEvent
        Try
            Dim oForm As SAPbouiCOM.Form
            oForm = oApplication.SBO_Application.Forms.Item(eventInfo.FormUID)
            'If eventInfo.FormUID = "RightClk" Then
            If oForm.TypeEx = frm_Hr_MPRequest Then
                oMenuObject = New clshrMPRequest
                oMenuObject.RightClickEvent(eventInfo, BubbleEvent)
            End If

            If oForm.TypeEx = frm_HR_Trainner Then
                oMenuObject = New clshrTrainner
                oMenuObject.RightClickEvent(eventInfo, BubbleEvent)
            End If

            If oForm.TypeEx = frm_HR_CrtApplicants1 Then
                oMenuObject = New clshrCrApplicants
                oMenuObject.RightClickEvent(eventInfo, BubbleEvent)
            End If

            If oForm.TypeEx = frm_hr_EmpMaster Then
                oMenuObject = New clsHRModule
                oMenuObject.RightClickEvent(eventInfo, BubbleEvent)
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        End Try

    End Sub

    Public Class WindowWrapper
        Implements System.Windows.Forms.IWin32Window
        Private _hwnd As IntPtr

        Public Sub New(ByVal handle As IntPtr)
            _hwnd = handle
        End Sub

        Public ReadOnly Property Handle() As System.IntPtr Implements System.Windows.Forms.IWin32Window.Handle
            Get
                Return _hwnd
            End Get
        End Property

    End Class

End Class
