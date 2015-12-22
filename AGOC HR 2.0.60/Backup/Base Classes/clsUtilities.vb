Imports System.IO
Imports System.Net.Mail
Public Class clsUtilities

    Private strThousSep As String = ","
    Private strDecSep As String = "."
    Private intQtyDec As Integer = 3
    Private FormNum As Integer
    Private oRecordSet, oTemp As SAPbobsCOM.Recordset
    Dim SmtpServer As New Net.Mail.SmtpClient()
    Dim mail As New Net.Mail.MailMessage
    Dim mailServer As String
    Dim mailPort As String
    Dim mailId As String
    Dim mailUser As String
    Dim mailPwd As String
    Dim mailSSL As String
    Dim toID As String
    Dim ccID As String
    Dim mType As String
    Dim path As String
    Dim sQuery As String
    Dim strEmpName As String

    Dim oGenService As SAPbobsCOM.GeneralService
    Dim oGenData As SAPbobsCOM.GeneralData
    Dim oCompService As SAPbobsCOM.CompanyService
    Dim oGeneralDataParams As SAPbobsCOM.GeneralDataParams

    Dim oCombo, oCombobox1, oCombobox2 As SAPbouiCOM.ComboBox
    Dim oEdit As SAPbouiCOM.EditText
    Dim oExEdit As SAPbouiCOM.EditText
    Dim oGrid As SAPbouiCOM.Grid



    Public Sub New()
        MyBase.New()
        FormNum = 1
    End Sub

    Public Function getloggedonuser() As String
        Return oApplication.Company.UserName
    End Function

    Public Function OpenMasterinLink(ByVal aForm As SAPbouiCOM.Form, ByVal FormType As String, Optional ByVal Code As String = "")
        Select Case FormType
            Case "Company"
                Dim ooBj As New clshrCompany
                ooBj.LoadForm()
            Case "Function"
                Dim ooBj As New clshrFunction
                ooBj.LoadForm()
            Case "Department"
                Dim ooBj As New clsDepartmentMaster
                ooBj.LoadForm()
            Case "CourseType"
                Dim ooBj As New clshrCourseType
                ooBj.LoadForm()
            Case "CourseCategory"
                Dim ooBj As New clshrCourseCategory
                ooBj.LoadForm()
            Case "Location"
                Dim ooBj As New clshrLocation
                ooBj.LoadForm()
            Case "Expenses"
                Dim ooBj As New clshrExpenses
                ooBj.LoadForm()
            Case "OrgStructure"
                Dim ooBj As New clshrOrgStructure
                ooBj.LoadForm()
            Case "Grade"
                Dim ooBj As New clshrGrade
                ooBj.LoadForm()
            Case "Level"
                Dim ooBj As New clshrLevel
                ooBj.LoadForm()
            Case "Salary"
                Dim ooBj As New clshrSalStructure
                ooBj.LoadForm1(Code)
            Case "Position"
                Dim ooBj As New clshrEmpPosition
                ooBj.LoadForm1(Code)
            Case "JobScreen"
                Dim ooBj As New clshrPosCompetence
                ooBj.LoadForm1(Code)
            Case "Course"
                Dim ooBj As New clshrCourse
                ooBj.LoadForm1(Code)
            Case "AgendaCode"
                Dim ooBj As New clshrTrainPlan
                ooBj.LoadForm1(Code)
            Case "TravelAgenda"
                Dim ooBj As New clshrTravelAgenda
                ooBj.LoadForm1(Code)
        End Select
    End Function

    Public Function createHRMainAuthorization() As Boolean
        Dim RetVal As Long
        Dim ErrCode As Long
        Dim ErrMsg As String
        Dim mUserPermission As SAPbobsCOM.UserPermissionTree
        mUserPermission = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserPermissionTree)
        '//Mandatory field, which is the key of the object.
        '//The partner namespace must be included as a prefix followed by _
        mUserPermission.PermissionID = "HRAddon"
        '//The Name value that will be displayed in the General Authorization Tree
        mUserPermission.Name = "Human Resource Addon"
        '//The permission that this object can get
        mUserPermission.Options = SAPbobsCOM.BoUPTOptions.bou_FullReadNone
        '//In case the level is one, there Is no need to set the FatherID parameter.
        '   mUserPermission.Levels = 1
        RetVal = mUserPermission.Add
        If RetVal = 0 Or -2035 Then
            Return True
        Else
            MsgBox(oApplication.Company.GetLastErrorDescription)
            Return False
        End If


    End Function

    Public Function addChildAuthorization(ByVal aChildID As String, ByVal aChildiDName As String, ByVal aorder As Integer, ByVal aFormType As String, ByVal aParentID As String, ByVal Permission As SAPbobsCOM.BoUPTOptions) As Boolean
        Dim RetVal As Long
        Dim ErrCode As Long
        Dim ErrMsg As String
        Dim mUserPermission As SAPbobsCOM.UserPermissionTree
        mUserPermission = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserPermissionTree)

        mUserPermission.PermissionID = aChildID
        mUserPermission.Name = aChildiDName
        mUserPermission.Options = Permission ' SAPbobsCOM.BoUPTOptions.bou_FullReadNone

        '//For level 2 and up you must set the object's father unique ID
        'mUserPermission.Level
        mUserPermission.ParentID = aParentID
        mUserPermission.UserPermissionForms.DisplayOrder = aorder
        '//this object manages forms
        ' If aFormType <> "" Then
        mUserPermission.UserPermissionForms.FormType = aFormType
        ' End If

        RetVal = mUserPermission.Add
        If RetVal = 0 Or RetVal = -2035 Then
            Return True
        Else
            MsgBox(oApplication.Company.GetLastErrorDescription)
            Return False
        End If


    End Function

    Public Sub AuthorizationCreation()
        addChildAuthorization("Setup", " Setup", 2, "", "HRAddon", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Trans", "Transactions", 2, "", "HRAddon", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        'Setup
        addChildAuthorization("UserSetup", "User Security Setup", 3, "", "Setup", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("Organization", "Organization Chart", 3, "", "Setup", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("SalaryStru", "Salary Structure", 3, "", "Setup", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("JobStru", "Job Structure", 3, "", "Setup", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Requisition", "Requisition Setup", 3, "", "Setup", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Apprisal", "Appraisal", 3, "", "Setup", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Training", "Training", 3, "", "Setup", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("TravelMgm", "Travle Management", 3, "", "Setup", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("HRApp", "Approval", 3, "", "Setup", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)

        addChildAuthorization("Login-Setup", "ESS Login setup", 4, "frm_hr_LogSetup", "UserSetup", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("Email-Setup", "EMail Setup", 4, frm_hr_EmailSetUp, "UserSetup", SAPbobsCOM.BoUPTOptions.bou_FullNone)

        addChildAuthorization("CompanyMaster", "Company Master", 4, frm_hr_Comp, "Organization", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Department", "Department Master", 4, frm_Department, "Organization", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Function", "Function Master", 4, frm_hr_Func, "Organization", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Unit", "Unit Master", 4, frm_hr_Unit, "Organization", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Location", "Location Master", 4, frm_hr_Loc, "Organization", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Branch", "Branch Master", 4, frm_Branches, "Organization", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Section", "Section Master", 4, frm_hr_Sec, "Organization", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("OrgStru", "Organizational Structure", 4, frm_hr_OrgSt, "Organization", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Objects", "Objects on Loan Master", 4, "frm_hr_ObjLoan", "Organization", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Lanugages", "Lanugages Master", 4, "frm_hr_Languages1", "Organization", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("ExitResp", "Exit Responsibilities", 4, "frm_hr_ExitResponse", "Organization", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Questionnaire", "Questionnaire Structure", 4, "frm_hr_Qustionaire", "Organization", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)

        addChildAuthorization("Grade", "Grade Master", 4, "frm_hr_Grade", "SalaryStru", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Level", "Level Master", 4, "frm_hr_Level", "SalaryStru", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Allowance", "Allowance Master", 4, "frm_hr_Allow", "SalaryStru", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Salary", "Salary Scale", 4, "frm_hr_SalStru", "SalaryStru", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Benifits", "Employer Contribution", 4, "frm_hr_Benefit", "SalaryStru", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("LeaveMaster", "Leave Type Master", 4, frm_hr_LeaveMaster, "SalaryStru", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)

        addChildAuthorization("JobDes", "Job Description Master", 4, "frm_hr_PosComp", "JobStru", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Position", "Position Master", 4, "frm_hr_EmpPosition", "JobStru", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("CompMaster", "Competency Master", 4, "frm_hr_CompObjmaster", "JobStru", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("CompLevel", "Competency Level Master", 4, "frm_hr_CompLvl", "JobStru", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)

        addChildAuthorization("InterView", "Interview Type", 4, "frm_hr_InterviewType", "Requisition", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("IntRating", "Interview Rating", 4, "frm_hr_Iratings", "Requisition", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("RecReqRea", "Requisition Request Reason", 4, "frm_hr_RecReqReason", "Requisition", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("RejReason", "Rejection Reason", 4, "frm_hr_RejMaster", "Requisition", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("OfferRej", "Offer Rejection", 4, "frm_hr_ORejMaster", "Requisition", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("ResStatus", "Residency Status Master", 4, "frm_hr_Rsta", "Requisition", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("InterView", "Interview Type", 4, "frm_hr_InterviewType", "Requisition", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)


        addChildAuthorization("Rating", "Apprisal Ratings", 4, "frm_hr_Ratings", "Apprisal", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("AppObjDis", "Appraisal Objective Distributuion", 4, "frm_hr_AppDisMaster", "Apprisal", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("POCate", "Personel Objective Category Master", 4, "frm_hr_PeoCategory", "Apprisal", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("PerObj", "Personel Objectives", 4, "frm_hr_PeoObj", "Apprisal", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("BusObj", "Business Objectives", 4, "frm_hr_BussObj", "Apprisal", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("De[Mapp", "Department Business Objectives", 4, "frm_hr_DeptMapp", "Apprisal", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)

        addChildAuthorization("CourceType", "Cource Type", 4, "frm_hr_CourseType", "Training", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("CourceCatType", "Cource Category Type", 4, "frm_hr_CourseCate", "Training", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Course", "Cource Setup", 4, "frm_hr_Course", "Training", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("TrainAgenda", "Trainning Agenda Setup", 4, "frm_hr_TrainPlan1", "Training", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("TrainerPro", "Trainner Profile", 4, "frm_HR_Trainner", "Training", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("Evaca", "Evaluation Category", 4, frm_hr_TrainQcCa, "Training", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("EvaItem", "Evaluation Items", 4, frm_hr_TrainQcItem, "Training", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("EvaRating", "Evaluation Rating", 4, frm_hr_TrainQcRA, "Training", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)

        addChildAuthorization("ExpMaster", "Expenses Master", 4, "frm_hr_Expenss", "TravelMgm", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("TravAg", "Travel Agenda-Setup", 4, frm_hr_TraAgenda, "TravelMgm", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("PayMth", "Payment Method-Setup", 4, frm_hr_PayMethod, "TravelMgm", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)


        addChildAuthorization("HRAppTem", "Approval Template -Setup", 4, frm_hr_ApproveTemp, "HRApp", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
     
        'Transactions

        'Self Request Approval

        addChildAuthorization("SESSREQ", "ESS Requests Approval", 3, "", "Trans", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("LevApp", "Leave Request ", 4, "frm_hr_LeaveApp", "SESSREQ", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("ResApp", "Resignation Request ", 4, "frm_hr_ResignAPP", "SESSREQ", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("RetApp", "Return From leave Request", 4, "frm_hr_RetLveApp", "SESSREQ", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("PerApp", "Permission Request", 4, "frm_hr_PerbyhouApp", "SESSREQ", SAPbobsCOM.BoUPTOptions.bou_FullNone)


        'Appraisals
        addChildAuthorization("Appraisal", "Appraisal", 3, "", "Trans", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("Initilize", "Appraisals Initializations", 4, "frm_hr_IniAppraisal", "Appraisal", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("AEmail", "Appraisal Email", 4, frm_hr_AppEmail, "Appraisal", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("SelfApp", "Sel Appraisals", 4, frm_hr_Approval, "Appraisal", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("FLApproval", "First Level Approval", 4, frm_hr_FApproval, "Appraisal", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("SLApproval", "Second Level Approval", 4, frm_hr_SApproval, "Appraisal", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("HRACC", "HR Acceptance", 4, frm_hr_SlctnCreteria, "Appraisal", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("HRGRI", "HR Grievance Acceptance", 4, frm_hr_SlctnCrGA, "Appraisal", SAPbobsCOM.BoUPTOptions.bou_FullNone)

        'Training
        addChildAuthorization("TrainingTrs", "Training", 3, "", "Trans", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("TrainReq", "Training Request", 4, "", "TrainingTrs", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        'addChildAuthorization("FLTRA", "First Level Training Req Approval", 5, frm_hr_MgrTrainApp, "TrainReq", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        'addChildAuthorization("HRTRA", "HR Training Req Approval", 5, frm_hr_HRTrainApp, "TrainReq", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("HRTrnApp", "Training Request Approval", 5, frm_hr_TrainRegApproval, "TrainReq", SAPbobsCOM.BoUPTOptions.bou_FullNone)

        addChildAuthorization("TRAP", "Training Request Approval", 4, frm_hr_TrainReg, "TrainingTrs", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("TROV", "Training OverView", 4, frm_hr_AppAttendees, "TrainingTrs", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("TREV", "Evaluation Review", 4, frm_hr_MgrEva, "TrainingTrs", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)

        addChildAuthorization("NTrainReq", "New Training Requests", 4, "", "TrainingTrs", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        'addChildAuthorization("FLNTRA", "Manager NewTraining Req Approval", 5, frm_hr_MgrTrainApp, "NTrainReq", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        'addChildAuthorization("HRNTRA", "HR New Training Req Approval", 5, frm_hr_HRTrainApp, "NTrainReq", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("HRNTrnApp", "New Training  Approval", 5, frm_hr_TrainNewApproval, "TrainReq", SAPbobsCOM.BoUPTOptions.bou_FullNone)

        'Requisition
        addChildAuthorization("RecrTR", "Requisition", 3, "", "Trans", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("RecReq", "Requisition Requisition", 4, frm_Hr_MPRequest, "RecrTR", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        'addChildAuthorization("Req1App", "First Level Approval", 4, frm_hr_RecApproval, "RecrTR", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        'addChildAuthorization("Req2App", "Second Level Approval", 4, frm_hr_HRecApproval, "RecrTR", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("HRRecApp", "Recruitment Request Approval", 5, frm_hr_MPRApproval, "RecrTR", SAPbobsCOM.BoUPTOptions.bou_FullNone)

        addChildAuthorization("AppProfile", "Applicant Profile", 4, frm_HR_CrtApplicants1, "RecrTR", SAPbobsCOM.BoUPTOptions.bou_FullReadNone)
        addChildAuthorization("ScrProces", "Screening Process", 4, frm_HR_Search1, "RecrTR", SAPbobsCOM.BoUPTOptions.bou_FullNone)

        addChildAuthorization("ShortList", "Shortlisting Process", 4, "", "RecrTR", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        'addChildAuthorization("Sht1App", "First Level Approval", 5, frm_hr_CReqSelection, "ShortList", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        'addChildAuthorization("Sht2App", "Second Level Approval", 5, frm_hr_CReqSelectionSe, "ShortList", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("HRShoApp", "Shortlistsed Candidate Approval", 5, frm_hr_ShortApproval, "ShortList", SAPbobsCOM.BoUPTOptions.bou_FullNone)

        addChildAuthorization("IntProcess", "Interview Process", 4, "", "RecrTR", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("IntSch", "Interview Scheduling", 4, frm_hr_CReqSelIPLM, "IntProcess", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("IntSum", "Interview Summary", 4, frm_hr_CReqSelIPHOD, "IntProcess", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        'addChildAuthorization("Int1App", "Candidate First Level Approval", 4, frm_hr_CReqSelIPHR, "IntProcess", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        'addChildAuthorization("Int2App", "Candidate HR Level Approval", 4, frm_hr_CReqSelIPSUM, "IntProcess", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("HRInvApp", "Final Interview Approval", 5, frm_hr_FinalApproval, "ShortList", SAPbobsCOM.BoUPTOptions.bou_FullNone)

        addChildAuthorization("IntOffer", "Employement Offer", 4, frm_hr_CReqSelIPGA, "IntProcess", SAPbobsCOM.BoUPTOptions.bou_FullNone)


        addChildAuthorization("Hiring", "Hiring Process", 4, frm_HR_Hiring, "RecrTR", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("ReqOv", "Requisition Overview", 4, frm_hr_RecOverview, "RecrTR", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("Req2Clo", "Requisition HR Approval", 4, frm_hr_RecClosing, "RecrTR", SAPbobsCOM.BoUPTOptions.bou_FullNone)



        'Employee Life Cycle
        addChildAuthorization("ELC", "Employee Life Cycle", 3, "", "Trans", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("Transfer", "Employee Transfer", 4, frm_hr_Transfer, "ELC", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("Promotion", "Employee Promotion", 4, frm_hr_Promotion, "ELC", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("POSCH", "Employee Position Change", 4, frm_hr_PosChanges, "ELC", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("ProApp", "Promotion Approval", 4, frm_hr_EmpLifeApp, "ELC", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("PosApp", "Position Change Approval", 4, frm_hr_EmpPosChApp, "ELC", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("Posting", "Life Cycle Posting", 4, frm_hr_EmpLifePost, "ELC", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("EmpExit", "Employee Exit", 4, "", "ELC", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("PayPost", "Update Payroll Details", 4, frm_HR_UpdatePayroll, "ELC", SAPbobsCOM.BoUPTOptions.bou_FullNone)

        addChildAuthorization("Intialization", "Exit Form Initialization", 5, frm_hr_ExitfrmInit, "EmpExit", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("Process", "Exit Form Process", 5, frm_hr_ExitProcess, "EmpExit", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("InterviewForm", "Exit interview Form", 5, frm_hr_ExitInvForm1, "EmpExit", SAPbobsCOM.BoUPTOptions.bou_FullNone)

        'Travel Mgmt
        addChildAuthorization("TrvMgmt", "Travel Managemnet", 3, "", "Trans", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("HRTraReApp", "Travel Request Approval", 5, frm_hr_TravelApproval, "TrvMgmt", SAPbobsCOM.BoUPTOptions.bou_FullNone)
        addChildAuthorization("HRClaApp", "Expense Claim Approval", 5, frm_hr_ClaimApproval, "TrvMgmt", SAPbobsCOM.BoUPTOptions.bou_FullNone)

    End Sub

    Public Function validateAuthorization(ByVal aUserId As String, ByVal aFormUID As String) As Boolean
        Dim oAuth As SAPbobsCOM.Recordset
        oAuth = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim struserid As String
        '    Return False
        struserid = oApplication.Company.UserName
        oAuth.DoQuery("select * from UPT1 where FormId='" & aFormUID & "'")
        If (oAuth.RecordCount <= 0) Then
            Return True
        Else
            Dim st As String
            st = oAuth.Fields.Item("PermId").Value
            st = "Select * from USR3 where PermId='" & st & "' and UserLink=" & aUserId
            oAuth.DoQuery(st)
            If oAuth.RecordCount > 0 Then
                If oAuth.Fields.Item("Permission").Value = "N" Then
                    Return False
                End If
                Return True
            Else
                Return True
            End If

        End If

        Return True

    End Function

    Public Function ValidateDeletionMaster(ByVal aCode As String, ByVal aChoice As String) As Boolean
        Dim oREC As SAPbobsCOM.Recordset
        Dim strString As String
        oREC = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

        Select Case aChoice
            Case "Allowance"
                strString = "Select * from ""@Z_PAY1"" where ""U_Z_EARN_TYPE""='" & aCode & "'"

            Case "Variable Earning"
                strString = "Select * from ""@Z_PAY_TRANS"" where ""U_Z_Type""='E' and  ""U_Z_TrnsCode""='" & aCode & "'"
            Case "Deduction"
                strString = "Select * from ""@Z_PAY2"" where ""U_Z_DEDUC_TYPE""='" & aCode & "'"
            Case "Social Benefit"
                strString = "Select * from ""@Z_PAY3"" where ""U_Z_CONTR_TYPE""='" & aCode & "'"
            Case "Leave"
                strString = "Select * from ""@Z_PAY_OLETRANS"" where ""U_Z_TrnsCode""='" & aCode & "'"
            Case "Loan"
                strString = "Select * from ""@Z_PAY5"" where ""U_Z_LoanCode""='" & aCode & "'"
            Case "Over Time"
                strString = "Select * from ""@Z_PAY_TRANS"" where ""U_Z_Type""='O' and  ""U_Z_TrnsCode""='" & aCode & "'"

        End Select
        oREC.DoQuery(strString)
        If oREC.RecordCount > 0 Then
            oApplication.Utilities.Message(aChoice & " Code : " & aCode & " already mapped to Transacton / Employee.", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End If
        Return True
    End Function
    Public Sub PopulatePositionDetails(ByVal aForm As SAPbouiCOM.Form, ByVal PosId As String, ByVal aChoice As String)
        Dim strqry As String
        Dim oCombobox As SAPbouiCOM.ComboBox
        Dim oRec As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If aChoice = "P" Then
            strqry = "SELECT T0.U_Z_PosName,T0.U_Z_JobCode,T0.U_Z_JobName,T0.U_Z_DeptCode,T0.U_Z_DeptName,T1.U_Z_OrgCode,T1.U_Z_OrgDesc  FROM [@Z_HR_OPOSIN]  T0 Left Join [dbo].[@Z_HR_ORGST]  T1 on T0.U_Z_PosCode=T1.U_Z_PosCode where T0.U_Z_PosCode='" & PosId & "'"
            oRec.DoQuery(strqry)
            If oRec.RecordCount > 0 Then
                oCombobox = aForm.Items.Item("1000011").Specific
                oCombobox.Select(oRec.Fields.Item("U_Z_DeptCode").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
                oApplication.Utilities.setEdittextvalue(aForm, "44", oRec.Fields.Item("U_Z_DeptName").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "48", oRec.Fields.Item("U_Z_PosName").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "50", oRec.Fields.Item("U_Z_JobCode").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "52", oRec.Fields.Item("U_Z_JobName").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "54", oRec.Fields.Item("U_Z_OrgCode").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "56", oRec.Fields.Item("U_Z_OrgDesc").Value)
            End If
        ElseIf aChoice = "C" Then
            strqry = "SELECT T0.U_Z_PosName,T0.U_Z_JobCode,T0.U_Z_JobName,T0.U_Z_DeptCode,T0.U_Z_DeptName,T0.U_Z_CompCode,T0.U_Z_CompName,T0.U_Z_DivCode,T0.U_Z_DivDesc,T1.U_Z_OrgCode,T1.U_Z_OrgDesc,"
            strqry = strqry & "T1.U_Z_UnitCode,T1.U_Z_UnitName,T1.U_Z_SecCode,T1.U_Z_SecName,T1.U_Z_LocCode,T1.U_Z_LocName,T1.U_Z_BranCode,T1.U_Z_BranName FROM [@Z_HR_OPOSIN]  T0 Left Join [dbo].[@Z_HR_ORGST]  T1 on T0.U_Z_PosCode=T1.U_Z_PosCode where T0.U_Z_PosCode='" & PosId & "'"
            oRec.DoQuery(strqry)
            If oRec.RecordCount > 0 Then
                oApplication.Utilities.setEdittextvalue(aForm, "48", oRec.Fields.Item("U_Z_PosName").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "50", oRec.Fields.Item("U_Z_JobCode").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "52", oRec.Fields.Item("U_Z_JobName").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "66", oRec.Fields.Item("U_Z_CompCode").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "68", oRec.Fields.Item("U_Z_CompName").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "74", oRec.Fields.Item("U_Z_DivCode").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "76", oRec.Fields.Item("U_Z_DivDesc").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "70", oRec.Fields.Item("U_Z_DeptCode").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "72", oRec.Fields.Item("U_Z_DeptName").Value)
                Try
                    oApplication.Utilities.setEdittextvalue(aForm, "54", oRec.Fields.Item("U_Z_OrgCode").Value)
                Catch ex As Exception
                    oApplication.Utilities.setEdittextvalue(aForm, "56", oRec.Fields.Item("U_Z_OrgDesc").Value)
                End Try
                oApplication.Utilities.setEdittextvalue(aForm, "82", oRec.Fields.Item("U_Z_UnitCode").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "84", oRec.Fields.Item("U_Z_UnitName").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "78", oRec.Fields.Item("U_Z_SecCode").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "80", oRec.Fields.Item("U_Z_SecName").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "86", oRec.Fields.Item("U_Z_LocCode").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "88", oRec.Fields.Item("U_Z_LocName").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "90", oRec.Fields.Item("U_Z_BranCode").Value)
                oApplication.Utilities.setEdittextvalue(aForm, "92", oRec.Fields.Item("U_Z_BranName").Value)
            End If
        End If
    End Sub
    Public Function UpdateEmployeeProfile(ByVal aForm As SAPbouiCOM.Form, ByVal Empid As String, ByVal PosCode As String, ByVal Joindt As Date, ByVal achoice As String, ByVal strCode As String) As Boolean
        Dim strqry As String
        Dim oRec, oTemp, oTemp1 As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim oEmployee As SAPbobsCOM.EmployeesInfo
        oEmployee = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oEmployeesInfo)
        Dim strDeptName, strBrancName, strPosiName, strDeptFrgnname As String
        strDeptName = ""
        strBrancName = ""
        strPosiName = ""
        If achoice = "P" Then
            If oEmployee.GetByKey(Empid) Then
                strqry = "SELECT T0.U_Z_PosName,T0.U_Z_JobCode,T0.U_Z_JobName,T0.U_Z_DeptCode,T0.U_Z_DeptName,T0.U_Z_CompCode,T0.U_Z_CompName,T0.U_Z_DivCode,T0.U_Z_DivDesc,T1.U_Z_OrgCode,T1.U_Z_OrgDesc,"
                strqry = strqry & "T0.U_Z_SalCode,T1.U_Z_UnitCode,T1.U_Z_UnitName,T1.U_Z_SecCode,T1.U_Z_SecName,T1.U_Z_LocCode,T1.U_Z_LocName,T1.U_Z_BranCode,T1.U_Z_BranName FROM [@Z_HR_OPOSIN]  T0 Left Join [dbo].[@Z_HR_ORGST]  T1 on T0.U_Z_PosCode=T1.U_Z_PosCode where T0.U_Z_PosCode='" & PosCode & "'"
                oRec.DoQuery(strqry)
                If oRec.RecordCount > 0 Then
                    oEmployee.UserFields.Fields.Item("U_Z_HR_PosiCode").Value = PosCode
                    oEmployee.UserFields.Fields.Item("U_Z_HR_PosiName").Value = oRec.Fields.Item("U_Z_PosName").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_JobstCode").Value = oRec.Fields.Item("U_Z_JobCode").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_JobstName").Value = oRec.Fields.Item("U_Z_JobName").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_OrgstCode").Value = oRec.Fields.Item("U_Z_OrgCode").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_OrgstName").Value = oRec.Fields.Item("U_Z_OrgDesc").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_SalaryCode").Value = oRec.Fields.Item("U_Z_SalCode").Value
                    oTemp1.DoQuery("Select * from OUDP where Code='" & oRec.Fields.Item("U_Z_DeptCode").Value & "'")
                    If oTemp1.RecordCount > 0 Then
                        oEmployee.Department = oTemp1.Fields.Item("Code").Value
                        strDeptName = oTemp1.Fields.Item("Code").Value
                        strDeptFrgnname = oTemp1.Fields.Item("U_Z_FrgnName").Value
                    End If
                    oTemp1.DoQuery("Select * from OHPS where name='" & PosCode & "'")
                    If oTemp1.RecordCount > 0 Then
                        oEmployee.Position = oTemp1.Fields.Item("posID").Value
                        strPosiName = oTemp1.Fields.Item("posID").Value
                    End If
                    Try
                        oEmployee.JobTitle = oRec.Fields.Item("U_Z_JobName").Value
                    Catch ex As Exception
                    End Try
                    Try
                        oEmployee.Branch = oRec.Fields.Item("U_Z_BranCode").Value
                        strBrancName = oRec.Fields.Item("U_Z_BranCode").Value
                    Catch ex As Exception
                    End Try
                    oEmployee.PaymentMethod = SAPbobsCOM.EmployeePaymentMethodEnum.epm_None
                    oEmployee.UserFields.Fields.Item("U_Z_HR_CompCode").Value = oRec.Fields.Item("U_Z_CompCode").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_CompName").Value = oRec.Fields.Item("U_Z_CompName").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_DivCode").Value = oRec.Fields.Item("U_Z_DivCode").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_DivName").Value = oRec.Fields.Item("U_Z_DivDesc").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_UnitName").Value = oRec.Fields.Item("U_Z_UnitName").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_SecName").Value = oRec.Fields.Item("U_Z_SecName").Value
                    If oRec.Fields.Item("U_Z_BranName").Value.ToString.Length > 20 Then
                        oEmployee.UserFields.Fields.Item("U_Z_HR_BraName").Value = oRec.Fields.Item("U_Z_BranName").Value.ToString.Substring(0, 19)
                    Else
                        oEmployee.UserFields.Fields.Item("U_Z_HR_BraName").Value = oRec.Fields.Item("U_Z_BranName").Value
                    End If
                    oEmployee.UserFields.Fields.Item("U_Z_LocName").Value = oRec.Fields.Item("U_Z_LocName").Value
                    oEmployee.UserFields.Fields.Item("U_Z_LocCode").Value = oRec.Fields.Item("U_Z_LocCode").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_JoinDate").Value = Joindt ' oApplication.Utilities.getEdittextvalue(oForm, "61")
                    oEmployee.UserFields.Fields.Item("U_Z_EmpLiCyStatus").Value = "P"
                    oEmployee.UserFields.Fields.Item("U_Z_EmpLifRef").Value = strCode
                    If oEmployee.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    Else
                        Dim strUserName As String
                        strUserName = oApplication.Company.UserName
                        strqry = "Update ""@Z_HR_HEM2"" set ""U_Z_PostedBy""='" & strUserName & "' ,""U_Z_PostDate""=getdate(), ""U_Z_Posting""='Y' where ""Code""='" & strCode & "'"
                        oRec.DoQuery(strqry)
                        Try
                            strqry = "Update OHEM set ""U_Z_Dept1""='" & strDeptName & "' ,""U_Z_Branch""='" & strBrancName & "', ""U_Z_Position""='" & strPosiName & "' where ""empID""='" & Empid & "'"
                            oRec.DoQuery(strqry)
                        Catch ex As Exception
                        End Try
                        Try
                            strqry = "Update OHEM set ""U_Z_HR_ADeptName""=N'" & strDeptFrgnname & "'  where ""empID""='" & Empid & "'"
                            oRec.DoQuery(strqry)
                        Catch ex As Exception

                        End Try

                        Return True
                    End If
                End If
            End If
        ElseIf achoice = "C" Then
            If oEmployee.GetByKey(Empid) Then
                strqry = "SELECT T0.U_Z_PosName,T0.U_Z_JobCode,T0.U_Z_JobName,T0.U_Z_DeptCode,T0.U_Z_DeptName,T0.U_Z_CompCode,T0.U_Z_CompName,T0.U_Z_DivCode,T0.U_Z_DivDesc,T1.U_Z_OrgCode,T1.U_Z_OrgDesc,"
                strqry = strqry & "T0.U_Z_SalCode,T1.U_Z_UnitCode,T1.U_Z_UnitName,T1.U_Z_SecCode,T1.U_Z_SecName,T1.U_Z_LocCode,T1.U_Z_LocName,T1.U_Z_BranCode,T1.U_Z_BranName FROM [@Z_HR_OPOSIN]  T0 Left Join [dbo].[@Z_HR_ORGST]  T1 on T0.U_Z_PosCode=T1.U_Z_PosCode where T0.U_Z_PosCode='" & PosCode & "'"
                oRec.DoQuery(strqry)
                If oRec.RecordCount > 0 Then
                    oEmployee.UserFields.Fields.Item("U_Z_HR_PosiCode").Value = PosCode
                    oEmployee.UserFields.Fields.Item("U_Z_HR_PosiName").Value = oRec.Fields.Item("U_Z_PosName").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_JobstCode").Value = oRec.Fields.Item("U_Z_JobCode").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_JobstName").Value = oRec.Fields.Item("U_Z_JobName").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_OrgstCode").Value = oRec.Fields.Item("U_Z_OrgCode").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_OrgstName").Value = oRec.Fields.Item("U_Z_OrgDesc").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_SalaryCode").Value = oRec.Fields.Item("U_Z_SalCode").Value
                    oTemp1.DoQuery("Select * from OUDP where Code='" & oRec.Fields.Item("U_Z_DeptCode").Value & "'")
                    If oTemp1.RecordCount > 0 Then
                        oEmployee.Department = oTemp1.Fields.Item("Code").Value
                        strDeptName = oTemp1.Fields.Item("Code").Value
                        strDeptFrgnname = oTemp1.Fields.Item("U_Z_FrgnName").Value
                    End If
                    oTemp1.DoQuery("Select * from OHPS where name='" & PosCode & "'")
                    If oTemp1.RecordCount > 0 Then
                        oEmployee.Position = oTemp1.Fields.Item("posID").Value
                        strPosiName = oTemp1.Fields.Item("posID").Value
                    End If
                    Try
                        oEmployee.JobTitle = oRec.Fields.Item("U_Z_JobName").Value
                    Catch ex As Exception
                    End Try
                    Try
                        oEmployee.Branch = oRec.Fields.Item("U_Z_BranCode").Value
                        strBrancName = oRec.Fields.Item("U_Z_BranCode").Value
                    Catch ex As Exception
                    End Try
                    oEmployee.PaymentMethod = SAPbobsCOM.EmployeePaymentMethodEnum.epm_None
                    oEmployee.UserFields.Fields.Item("U_Z_HR_CompCode").Value = oRec.Fields.Item("U_Z_CompCode").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_CompName").Value = oRec.Fields.Item("U_Z_CompName").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_DivCode").Value = oRec.Fields.Item("U_Z_DivCode").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_DivName").Value = oRec.Fields.Item("U_Z_DivDesc").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_UnitName").Value = oRec.Fields.Item("U_Z_UnitName").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_SecName").Value = oRec.Fields.Item("U_Z_SecName").Value
                    If oRec.Fields.Item("U_Z_BranName").Value.ToString.Length > 20 Then
                        oEmployee.UserFields.Fields.Item("U_Z_HR_BraName").Value = oRec.Fields.Item("U_Z_BranName").Value.ToString.Substring(0, 19)
                    Else
                        oEmployee.UserFields.Fields.Item("U_Z_HR_BraName").Value = oRec.Fields.Item("U_Z_BranName").Value
                    End If
                    oEmployee.UserFields.Fields.Item("U_Z_LocName").Value = oRec.Fields.Item("U_Z_LocName").Value
                    oEmployee.UserFields.Fields.Item("U_Z_LocCode").Value = oRec.Fields.Item("U_Z_LocCode").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_PosFrom").Value = Joindt
                    oEmployee.UserFields.Fields.Item("U_Z_EmpLiCyStatus").Value = "C"
                    oEmployee.UserFields.Fields.Item("U_Z_EmpLifRef").Value = strCode
                    If oEmployee.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    Else
                        Dim strUserName As String
                        strUserName = oApplication.Company.UserName
                        strqry = "Update ""@Z_HR_HEM4"" set ""U_Z_PostedBy""='" & strUserName & "' ,""U_Z_PostDate""=getdate(), ""U_Z_Posting""='Y' where ""Code""='" & strCode & "'"
                        oRec.DoQuery(strqry)
                        Try
                            strqry = "Update OHEM set ""U_Z_Dept1""='" & strDeptName & "' ,""U_Z_Branch""='" & strBrancName & "', ""U_Z_Position""='" & strPosiName & "' where ""empID""='" & Empid & "'"
                            oRec.DoQuery(strqry)
                        Catch ex As Exception
                        End Try
                        Try
                            strqry = "Update OHEM set ""U_Z_HR_ADeptName""=N'" & strDeptFrgnname & "'  where ""empID""='" & Empid & "'"
                            oRec.DoQuery(strqry)
                        Catch ex As Exception

                        End Try
                        Return True
                    End If
                End If
            End If
        End If
    End Function

    Public Function UpdateEmployeeHRDetails(ByVal aForm As SAPbouiCOM.Form, ByVal Empid As String) As Boolean
        Dim strqry As String
        Dim oRec, oTemp, oTemp1 As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim oEmployee As SAPbobsCOM.EmployeesInfo
        oEmployee = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oEmployeesInfo)
        Dim strDeptName, strBrancName, strPosiName, strDeptFrgnname, PosCode As String
        strDeptName = ""
        strBrancName = ""
        strPosiName = ""
        If 1 = 1 Then
            If oEmployee.GetByKey(Empid) Then
                PosCode = oEmployee.Position
                strqry = "SELECT T0.U_Z_PosName,T0.U_Z_JobCode,T0.U_Z_JobName,T0.U_Z_DeptCode,T0.U_Z_DeptName,T0.U_Z_CompCode,T0.U_Z_CompName,T0.U_Z_DivCode,T0.U_Z_DivDesc,T1.U_Z_OrgCode,T1.U_Z_OrgDesc,"
                strqry = strqry & "T0.U_Z_SalCode,T1.U_Z_UnitCode,T1.U_Z_UnitName,T1.U_Z_SecCode,T1.U_Z_SecName,T1.U_Z_LocCode,T1.U_Z_LocName,T1.U_Z_BranCode,T1.U_Z_BranName FROM [@Z_HR_OPOSIN]  T0 Left Join [dbo].[@Z_HR_ORGST]  T1 on T0.U_Z_PosCode=T1.U_Z_PosCode inner Join OHPS T3 on T3.name =T0.U_Z_PosCode where T3.posID=" & PosCode
                oRec.DoQuery(strqry)
                If oRec.RecordCount > 0 Then
                    oEmployee.UserFields.Fields.Item("U_Z_HR_PosiCode").Value = PosCode
                    oEmployee.UserFields.Fields.Item("U_Z_HR_PosiName").Value = oRec.Fields.Item("U_Z_PosName").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_JobstCode").Value = oRec.Fields.Item("U_Z_JobCode").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_JobstName").Value = oRec.Fields.Item("U_Z_JobName").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_OrgstCode").Value = oRec.Fields.Item("U_Z_OrgCode").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_OrgstName").Value = oRec.Fields.Item("U_Z_OrgDesc").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_SalaryCode").Value = oRec.Fields.Item("U_Z_SalCode").Value
                    oTemp1.DoQuery("Select * from OUDP where Code='" & oRec.Fields.Item("U_Z_DeptCode").Value & "'")
                    If oTemp1.RecordCount > 0 Then
                        oEmployee.Department = oTemp1.Fields.Item("Code").Value
                        strDeptName = oTemp1.Fields.Item("Code").Value
                        strDeptFrgnname = oTemp1.Fields.Item("U_Z_FrgnName").Value
                    End If
                    oTemp1.DoQuery("Select * from OHPS where name='" & PosCode & "'")
                    If oTemp1.RecordCount > 0 Then
                        oEmployee.Position = oTemp1.Fields.Item("posID").Value
                        strPosiName = oTemp1.Fields.Item("posID").Value
                    End If
                    Try
                        oEmployee.JobTitle = oRec.Fields.Item("U_Z_JobName").Value
                    Catch ex As Exception
                    End Try
                    Try
                        oEmployee.Branch = oRec.Fields.Item("U_Z_BranCode").Value
                        strBrancName = oRec.Fields.Item("U_Z_BranCode").Value
                    Catch ex As Exception
                    End Try
                    oEmployee.PaymentMethod = SAPbobsCOM.EmployeePaymentMethodEnum.epm_None
                    oEmployee.UserFields.Fields.Item("U_Z_HR_CompCode").Value = oRec.Fields.Item("U_Z_CompCode").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_CompName").Value = oRec.Fields.Item("U_Z_CompName").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_DivCode").Value = oRec.Fields.Item("U_Z_DivCode").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_DivName").Value = oRec.Fields.Item("U_Z_DivDesc").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_UnitName").Value = oRec.Fields.Item("U_Z_UnitName").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_SecName").Value = oRec.Fields.Item("U_Z_SecName").Value
                    If oRec.Fields.Item("U_Z_BranName").Value.ToString.Length > 20 Then
                        oEmployee.UserFields.Fields.Item("U_Z_HR_BraName").Value = oRec.Fields.Item("U_Z_BranName").Value.ToString.Substring(0, 19)
                    Else
                        oEmployee.UserFields.Fields.Item("U_Z_HR_BraName").Value = oRec.Fields.Item("U_Z_BranName").Value
                    End If
                    oEmployee.UserFields.Fields.Item("U_Z_LocName").Value = oRec.Fields.Item("U_Z_LocName").Value
                    oEmployee.UserFields.Fields.Item("U_Z_LocCode").Value = oRec.Fields.Item("U_Z_LocCode").Value
                    oEmployee.UserFields.Fields.Item("U_Z_HR_JoinDate").Value = oEmployee.StartDate  ' oApplication.Utilities.getEdittextvalue(oForm, "61")
                    If oEmployee.Update <> 0 Then
                        oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                        Return False
                    Else
                        Dim strUserName As String
                        strUserName = oApplication.Company.UserName
                        Try
                            strqry = "Update OHEM set ""U_Z_Dept1""='" & strDeptName & "' ,""U_Z_Branch""='" & strBrancName & "', ""U_Z_Position""='" & strPosiName & "' where ""empID""='" & Empid & "'"
                            oRec.DoQuery(strqry)
                        Catch ex As Exception
                        End Try
                        Try
                            strqry = "Update OHEM set ""U_Z_HR_ADeptName""=N'" & strDeptFrgnname & "'  where ""empID""='" & Empid & "'"
                            oRec.DoQuery(strqry)
                        Catch ex As Exception
                        End Try
                        Return True
                    End If
                End If
            End If
        End If
    End Function
    Public Sub UpdateObjectName(ByVal aTable As String, ByVal aobjID As String)
        Dim oObjRec As SAPbobsCOM.Recordset
        Dim sst As String
        sst = "Update """ & aTable & """ set ""Object""='" & aobjID & "'"
        oObjRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oObjRec.DoQuery(sst)

    End Sub
    Public Sub assignMatrixLineno(ByVal aGrid As SAPbouiCOM.Grid, ByVal aform As SAPbouiCOM.Form)
        aform.Freeze(True)
        Try
            For intNo As Integer = 0 To aGrid.DataTable.Rows.Count - 1
                aGrid.RowHeaders.SetText(intNo, intNo + 1)
            Next
        Catch ex As Exception
        End Try
        aGrid.Columns.Item("RowsHeader").TitleObject.Caption = "#"
        aform.Freeze(False)
    End Sub
#Region "Get SAP Accoutn Code"
    Public Function getSAPAccount(ByVal aCode As String) As String
        Dim oRS As SAPbobsCOM.Recordset
        oRS = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRS.DoQuery("Select isnull(AcctCode,'') from OACT where Formatcode='" & aCode & "'")
        Return oRS.Fields.Item(0).Value
    End Function
#End Region
    Public Function getEmpIDforMangers(ByVal aCode As String) As String
        Dim oTest As SAPbobsCOM.Recordset
        Dim intManagerid As Integer
        Dim strEmp As String = ""
        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTest.DoQuery("SELECT *  FROM OHEM T0  INNER JOIN OUSR T1 ON T0.userId = T1.USERID WHERE T1.[USER_CODE] ='" & aCode & "'")
        If oTest.RecordCount > 0 Then
            intManagerid = oTest.Fields.Item("empID").Value
            ' strEmp = "'" & intManagerid & "'"
            oTest.DoQuery("Select empId from OHEM where manager=" & intManagerid)
            For intRow As Integer = 0 To oTest.RecordCount - 1

                If strEmp = "" Then
                    strEmp = "'" & oTest.Fields.Item(0).Value & "'"
                Else
                    strEmp = strEmp & " ,'" & oTest.Fields.Item(0).Value & "'"
                End If
                oTest.MoveNext()
            Next
            Return strEmp
        Else
            Return "99999"
        End If
    End Function
    Public Function getEmpIDforMangersApp(ByVal aCode As String) As String
        Dim oTest As SAPbobsCOM.Recordset
        Dim intManagerid As Integer
        Dim strEmp As String = ""
        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTest.DoQuery("SELECT *  FROM OHEM T0  INNER JOIN OUSR T1 ON T0.userId = T1.USERID WHERE T1.[USERID] ='" & aCode & "'")
        If oTest.RecordCount > 0 Then
            intManagerid = oTest.Fields.Item("empID").Value
            strEmp = "'" & intManagerid & "'"
            oTest.DoQuery("Select empId from OHEM where manager=" & intManagerid)
            For intRow As Integer = 0 To oTest.RecordCount - 1

                If strEmp = "" Then
                    strEmp = "'" & oTest.Fields.Item(0).Value & "'"
                Else
                    strEmp = strEmp & " ,'" & oTest.Fields.Item(0).Value & "'"
                End If
                oTest.MoveNext()
            Next
            Return strEmp
        Else
            Return "99999"
        End If
    End Function

    Public Function getDeptID(ByVal DeptName As String) As String
        Dim oTest As SAPbobsCOM.Recordset
        Dim strDeptID As String = ""
        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim strQuery As String = "select Code from OUDP Where Name='" & DeptName.Trim() & "'"
        oTest.DoQuery(strQuery)
        If Not oTest.EoF Then
            strDeptID = oTest.Fields.Item("Code").Value.ToString()
        End If
        Return strDeptID
    End Function

    Public Function getManagerEmPID(ByVal aCode As String) As String
        Dim oTest As SAPbobsCOM.Recordset
        Dim intManagerid As Integer
        Dim strEmp As String = ""
        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTest.DoQuery("SELECT *  FROM OHEM T0  INNER JOIN OUSR T1 ON T0.userId = T1.USERID WHERE T1.[USER_CODE] ='" & aCode & "'")
        If oTest.RecordCount > 0 Then
            intManagerid = oTest.Fields.Item("empID").Value
            Return intManagerid.ToString
        Else
            Return "0"
        End If
    End Function
#Region "Recruitment"
    Public Sub CandidateUpdation(ByVal strchoice As String, ByVal aReqNo As String)
        Dim otemp1, otemprs As SAPbobsCOM.Recordset
        Dim strqry As String
        Dim strcount As Integer
        otemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strchoice = "Canddate" Then
            strqry = "Select count(*) as Code from [@Z_HR_OCRAPP] where U_Z_RequestCode='" & aReqNo & "' and U_Z_Status='R' group by U_Z_RequestCode"
            otemp1.DoQuery(strqry)
            If 1 = 1 Then ' otemp1.RecordCount > 0 Then
                strcount = otemp1.Fields.Item("Code").Value
                otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemprs.DoQuery("Update [@Z_HR_ORMPREQ] set U_Z_RecCandidate='" & strcount & "' where DocEntry='" & aReqNo & "' ")
            End If
        ElseIf strchoice = "Search" Then
            strqry = "Select count(*) as Code from [@Z_HR_OHEM1] where U_Z_ReqNo='" & aReqNo & "' and U_Z_ApplStatus='S' group by U_Z_ReqNo"
            otemp1.DoQuery(strqry)
            If 1 = 1 Then ' otemp1.RecordCount > 0 Then
                strcount = otemp1.Fields.Item("Code").Value
                otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemprs.DoQuery("Update [@Z_HR_ORMPREQ] set U_Z_ShortCandidate='" & strcount & "' where DocEntry='" & aReqNo & "' ")
            End If
        Else
            strqry = "Select count(*) as Code from [@Z_HR_OHEM1] where U_Z_ReqNo='" & aReqNo & "' and U_Z_IntervStatus='F' group by U_Z_ReqNo"
            otemp1.DoQuery(strqry)
            If 1 = 1 Then ' otemp1.RecordCount > 0 Then
                strcount = otemp1.Fields.Item("Code").Value
                otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemprs.DoQuery("Update [@Z_HR_ORMPREQ] set U_Z_OfferCandidate='" & strcount & "' where DocEntry='" & aReqNo & "' ")
            End If
            strqry = "Select count(*) as Code from [@Z_HR_OHEM1] where U_Z_ReqNo='" & aReqNo & "' and U_Z_IntervStatus='P' group by U_Z_ReqNo"
            otemp1.DoQuery(strqry)
            If 1 = 1 Then ' otemp1.RecordCount > 0 Then
                strcount = otemp1.Fields.Item("Code").Value
                otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemprs.DoQuery("Update [@Z_HR_ORMPREQ] set U_Z_PlacedCandidate='" & strcount & "' where DocEntry='" & aReqNo & "' ")
            End If
            strqry = "Select count(*) as Code from [@Z_HR_OHEM1] where U_Z_ReqNo='" & aReqNo & "' and U_Z_MgrStatus='A' group by U_Z_ReqNo"
            otemp1.DoQuery(strqry)
            If 1 = 1 Then ' otemp1.RecordCount > 0 Then
                strcount = otemp1.Fields.Item("Code").Value
                otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemprs.DoQuery("Update [@Z_HR_ORMPREQ] set U_Z_LMACandidate='" & strcount & "' where DocEntry='" & aReqNo & "' ")
            End If
            strqry = "Select count(*) as Code from [@Z_HR_OHEM1] where U_Z_ReqNo='" & aReqNo & "' and U_Z_MgrStatus='R' group by U_Z_ReqNo"
            otemp1.DoQuery(strqry)
            If 1 = 1 Then ' otemp1.RecordCount > 0 Then
                strcount = otemp1.Fields.Item("Code").Value
                otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemprs.DoQuery("Update [@Z_HR_ORMPREQ] set U_Z_LMRCandidate='" & strcount & "' where DocEntry='" & aReqNo & "' ")
            End If
            strqry = "Select count(*) as Code from [@Z_HR_OHEM1] where U_Z_ReqNo='" & aReqNo & "' and U_Z_SMgrStatus='A' group by U_Z_ReqNo"
            otemp1.DoQuery(strqry)
            If 1 = 1 Then ' otemp1.RecordCount > 0 Then
                strcount = otemp1.Fields.Item("Code").Value
                otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemprs.DoQuery("Update [@Z_HR_ORMPREQ] set U_Z_SMACandidate='" & strcount & "' where DocEntry='" & aReqNo & "' ")
            End If
            strqry = "Select count(*) as Code from [@Z_HR_OHEM1] where U_Z_ReqNo='" & aReqNo & "' and U_Z_SMgrStatus='R' group by U_Z_ReqNo"
            otemp1.DoQuery(strqry)
            If 1 = 1 Then ' otemp1.RecordCount > 0 Then
                strcount = otemp1.Fields.Item("Code").Value
                otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemprs.DoQuery("Update [@Z_HR_ORMPREQ] set U_Z_SMRCandidate='" & strcount & "' where DocEntry='" & aReqNo & "' ")
            End If
            strqry = "Select count(*) as Code from [@Z_HR_OHEM1] where U_Z_ReqNo='" & aReqNo & "' and U_Z_IPHODSta='A' group by U_Z_ReqNo"
            otemp1.DoQuery(strqry)
            If 1 = 1 Then ' otemp1.RecordCount > 0 Then
                strcount = otemp1.Fields.Item("Code").Value
                otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemprs.DoQuery("Update [@Z_HR_ORMPREQ] set U_Z_HODACandidate='" & strcount & "' where DocEntry='" & aReqNo & "' ")
            End If
            strqry = "Select count(*) as Code from [@Z_HR_OHEM1] where U_Z_ReqNo='" & aReqNo & "' and U_Z_IPHODSta='R' group by U_Z_ReqNo"
            otemp1.DoQuery(strqry)
            If 1 = 1 Then ' otemp1.RecordCount > 0 Then
                strcount = otemp1.Fields.Item("Code").Value
                otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemprs.DoQuery("Update [@Z_HR_ORMPREQ] set U_Z_HODRCandidate='" & strcount & "' where DocEntry='" & aReqNo & "' ")
            End If
            strqry = "Select count(*) as Code from [@Z_HR_OHEM1] where U_Z_ReqNo='" & aReqNo & "' and U_Z_IPHRSta='A' group by U_Z_ReqNo"
            otemp1.DoQuery(strqry)
            If 1 = 1 Then ' otemp1.RecordCount > 0 Then
                strcount = otemp1.Fields.Item("Code").Value
                otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemprs.DoQuery("Update [@Z_HR_ORMPREQ] set U_Z_HRACandidate='" & strcount & "' where DocEntry='" & aReqNo & "' ")
            End If
            strqry = "Select count(*) as Code from [@Z_HR_OHEM1] where U_Z_ReqNo='" & aReqNo & "' and U_Z_IPHRSta='R' group by U_Z_ReqNo"
            otemp1.DoQuery(strqry)
            If 1 = 1 Then ' otemp1.RecordCount > 0 Then
                strcount = otemp1.Fields.Item("Code").Value
                otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemprs.DoQuery("Update [@Z_HR_ORMPREQ] set U_Z_HRRCandidate='" & strcount & "' where DocEntry='" & aReqNo & "' ")
            End If
            strqry = "Select count(*) as Code from [@Z_HR_OHEM2] where DocEntry='" & aReqNo & "' and U_Z_InterviewStatus='A' group by DocEntry"
            otemp1.DoQuery(strqry)
            If 1 = 1 Then ' otemp1.RecordCount > 0 Then
                strcount = otemp1.Fields.Item("Code").Value
                otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemprs.DoQuery("Update [@Z_HR_ORMPREQ] set U_Z_InvSelectCan='" & strcount & "' where DocEntry='" & aReqNo & "' ")
            End If
            strqry = "Select count(*) as Code from [@Z_HR_OHEM2] where DocEntry='" & aReqNo & "' and U_Z_InterviewStatus='R' group by DocEntry"
            otemp1.DoQuery(strqry)
            If 1 = 1 Then ' otemp1.RecordCount > 0 Then
                strcount = otemp1.Fields.Item("Code").Value
                otemprs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                otemprs.DoQuery("Update [@Z_HR_ORMPREQ] set U_Z_InvRejectCan='" & strcount & "' where DocEntry='" & aReqNo & "' ")
            End If
        End If
    End Sub

#End Region

    Public Sub EnableDisable(ByVal sForm As SAPbouiCOM.Form, ByVal strtitle As String, ByVal strNeqReq As String, Optional ByVal strstatus As String = "")
        Dim dt As Date
        dt = Now.Date
        Dim oMatrix As SAPbouiCOM.Matrix
        Dim oColumn As SAPbouiCOM.Column
        If strtitle = "Employee Travel Request" Or strtitle = "Travel OverView" Then
            sForm.Items.Item("18").Visible = False
            sForm.Items.Item("44").Visible = False
            sForm.Items.Item("4").Enabled = False
            If strstatus <> "Open" Then
                sForm.Items.Item("21").Enabled = False
                sForm.Items.Item("23").Enabled = False
                sForm.Items.Item("25").Enabled = False
                sForm.Items.Item("27").Enabled = False
                sForm.Items.Item("29").Enabled = False
                sForm.Items.Item("31").Enabled = False
                sForm.Items.Item("35").Enabled = False
                sForm.Items.Item("51").Visible = False
                sForm.Items.Item("52").Visible = False
                sForm.Items.Item("53").Visible = False
                sForm.Items.Item("54").Visible = False
                sForm.Items.Item("55").Visible = False
                sForm.Items.Item("56").Visible = False
                sForm.Items.Item("57").Enabled = False
                'sForm.Items.Item("58").Visible = True
                'sForm.Items.Item("59").Visible = False
                'sForm.Items.Item("60").Visible = False
                'sForm.Items.Item("61").Visible = False
                'sForm.Items.Item("62").Visible = False
                'sForm.Items.Item("63").Visible = False
                'sForm.Items.Item("64").Visible = False
                'sForm.Items.Item("65").Visible = False
            Else
                sForm.Items.Item("21").Enabled = True
                sForm.Items.Item("23").Enabled = True
                sForm.Items.Item("25").Enabled = True
                sForm.Items.Item("27").Enabled = True
                sForm.Items.Item("29").Enabled = True
                sForm.Items.Item("31").Enabled = True
                sForm.Items.Item("35").Enabled = True
                sForm.Items.Item("51").Visible = False
                sForm.Items.Item("52").Visible = False
                sForm.Items.Item("53").Visible = False
                sForm.Items.Item("54").Visible = False
                sForm.Items.Item("55").Visible = False
                sForm.Items.Item("56").Visible = False
                sForm.Items.Item("57").Enabled = False
                'sForm.Items.Item("58").Visible = False
                'sForm.Items.Item("59").Visible = False
                'sForm.Items.Item("60").Visible = False
                'sForm.Items.Item("61").Visible = False
                'sForm.Items.Item("62").Visible = False
                'sForm.Items.Item("63").Visible = False
                'sForm.Items.Item("64").Visible = False
                'sForm.Items.Item("65").Visible = False

            End If
        ElseIf strtitle = "Travel Request Approval" Or strtitle = "Expenses OverView" Then
            sForm.Title = strtitle
            sForm.Items.Item("4").Enabled = False
            sForm.Items.Item("18").Visible = True
            If strNeqReq = "Y" Then
                sForm.Items.Item("21").Enabled = True
            Else
                sForm.Items.Item("21").Enabled = False
            End If
            sForm.Items.Item("23").Enabled = False
            sForm.Items.Item("25").Enabled = False
            sForm.Items.Item("27").Enabled = False
            sForm.Items.Item("29").Enabled = False
            sForm.Items.Item("31").Enabled = False
            sForm.Items.Item("35").Enabled = False
            sForm.Items.Item("33").Enabled = False
            sForm.Items.Item("37").Enabled = True
            ' sForm.Items.Item("38").Enabled = False
            oMatrix = sForm.Items.Item("38").Specific
            oMatrix.Columns.Item("V_0").Editable = False
            oMatrix.Columns.Item("V_1").Editable = False
            oMatrix.Columns.Item("V_2").Visible = False
            oMatrix.Columns.Item("V_3").Editable = False
            oMatrix.Columns.Item("V_4").Visible = False
            oMatrix.Columns.Item("V_5").Editable = False
            oColumn = oMatrix.Columns.Item("V_6")
            'oColumn.ValidValues.Add("A", "Applicable")
            'oColumn.ValidValues.Add("NA", "Not Applicable")
            'oColumn.ValidValues.Add("AP", "Approved")
            'oColumn.ValidValues.Add("P", "Paid")
            oColumn.DisplayDesc = True
            If strtitle = "Expenses OverView" Then
                oColumn.Editable = False
            Else
                oColumn.Editable = True
            End If

            'oMatrix.Columns.Item("V_6").Editable = True
            sForm.Items.Item("5").Visible = False
            sForm.Items.Item("6").Visible = False
            sForm.Items.Item("51").Visible = False
            sForm.Items.Item("52").Visible = False
            sForm.Items.Item("53").Visible = False
            sForm.Items.Item("54").Visible = False
            sForm.Items.Item("55").Visible = True
            sForm.Items.Item("56").Visible = True
            sForm.Items.Item("57").Enabled = False
            oApplication.Utilities.setEdittextvalue(sForm, "56", dt)
            'sForm.Items.Item("58").Visible = True
            'sForm.Items.Item("59").Visible = True
            'sForm.Items.Item("60").Visible = False
            'sForm.Items.Item("61").Visible = False
            'sForm.Items.Item("62").Visible = False
            'sForm.Items.Item("63").Visible = False
            'sForm.Items.Item("64").Visible = False
            'sForm.Items.Item("65").Visible = False
        ElseIf strtitle = "Employee Expenses Claim Request" Then
            sForm.Title = strtitle
            sForm.Items.Item("4").Enabled = False
            sForm.Items.Item("18").Visible = True
            sForm.Items.Item("44").Visible = True
            sForm.Items.Item("21").Enabled = False
            sForm.Items.Item("23").Enabled = False
            sForm.Items.Item("25").Enabled = False
            sForm.Items.Item("27").Enabled = False
            sForm.Items.Item("29").Enabled = False
            sForm.Items.Item("31").Enabled = False
            sForm.Items.Item("35").Enabled = False
            sForm.Items.Item("33").Enabled = False
            sForm.Items.Item("37").Enabled = False
            sForm.Items.Item("46").Enabled = True
            sForm.Items.Item("48").Enabled = True
            oMatrix = sForm.Items.Item("38").Specific
            oMatrix.Columns.Item("V_0").Editable = False
            oMatrix.Columns.Item("V_1").Visible = False
            oMatrix.Columns.Item("V_2").Visible = True
            oMatrix.Columns.Item("V_2").Editable = True
            oMatrix.Columns.Item("V_3").Visible = False
            oMatrix.Columns.Item("V_4").Visible = False
            oMatrix.Columns.Item("V_5").Visible = False
            oMatrix.Columns.Item("V_6").Visible = False
            sForm.Items.Item("57").Enabled = False
            sForm.Items.Item("5").Visible = False
            sForm.Items.Item("6").Visible = False
            sForm.Items.Item("51").Visible = True
            sForm.Items.Item("52").Visible = True
            sForm.Items.Item("53").Visible = False
            sForm.Items.Item("54").Visible = False
            sForm.Items.Item("55").Visible = False
            sForm.Items.Item("56").Visible = False
            oApplication.Utilities.setEdittextvalue(sForm, "52", dt)
            'sForm.Items.Item("58").Visible = True
            'sForm.Items.Item("59").Visible = True
            'sForm.Items.Item("60").Visible = True
            'sForm.Items.Item("61").Visible = True
            'sForm.Items.Item("62").Visible = False
            'sForm.Items.Item("63").Visible = False
            'sForm.Items.Item("64").Visible = False
            'sForm.Items.Item("65").Visible = False
        ElseIf strtitle = "Employee Expenses Approval" Then
            sForm.Title = strtitle
            sForm.Items.Item("4").Enabled = False
            sForm.Items.Item("18").Visible = True
            sForm.Items.Item("44").Visible = True
            sForm.Items.Item("21").Enabled = False
            sForm.Items.Item("23").Enabled = False
            sForm.Items.Item("25").Enabled = False
            sForm.Items.Item("27").Enabled = False
            sForm.Items.Item("29").Enabled = False
            sForm.Items.Item("31").Enabled = False
            sForm.Items.Item("35").Enabled = False
            sForm.Items.Item("33").Enabled = False
            sForm.Items.Item("37").Enabled = True
            'sForm.Items.Item("38").Enabled = False
            sForm.Items.Item("46").Enabled = False
            sForm.Items.Item("48").Enabled = False
            oMatrix = sForm.Items.Item("38").Specific
            oMatrix.Columns.Item("V_0").Editable = False
            oMatrix.Columns.Item("V_1").Editable = False
            oMatrix.Columns.Item("V_2").Editable = False
            oMatrix.Columns.Item("V_3").Editable = False
            oMatrix.Columns.Item("V_4").Editable = True
            oMatrix.Columns.Item("V_5").Editable = False
            oColumn = oMatrix.Columns.Item("V_6")
            'oColumn.ValidValues.Add("A", "Applicable")
            'oColumn.ValidValues.Add("NA", "Not Applicable")
            'oColumn.ValidValues.Add("AP", "Approved")
            'oColumn.ValidValues.Add("P", "Paid")
            oColumn.DisplayDesc = True
            oColumn.Editable = True
            'oMatrix.Columns.Item("V_6").Editable = True
            sForm.Items.Item("5").Visible = False
            sForm.Items.Item("6").Visible = False
            sForm.Items.Item("51").Visible = False
            sForm.Items.Item("52").Visible = False
            sForm.Items.Item("53").Visible = True
            sForm.Items.Item("54").Visible = True
            sForm.Items.Item("55").Visible = False
            sForm.Items.Item("56").Visible = False
            oApplication.Utilities.setEdittextvalue(sForm, "54", dt)
            sForm.Items.Item("57").Enabled = False
            'sForm.Items.Item("58").Visible = True
            'sForm.Items.Item("59").Visible = True
            'sForm.Items.Item("60").Visible = True
            'sForm.Items.Item("61").Visible = True
            'sForm.Items.Item("62").Visible = True
            'sForm.Items.Item("63").Visible = True
            'sForm.Items.Item("64").Visible = False
            'sForm.Items.Item("65").Visible = False
        End If
    End Sub

    Public Sub CandidateLists(ByVal aform As SAPbouiCOM.Form, ByVal canjobid As String, ByVal aChoice As String, Optional ByVal strTitle As String = "")
        Try
            Dim oGrid As SAPbouiCOM.Grid
            Dim ocombo As SAPbouiCOM.ComboBoxColumn
            Dim oTemp As SAPbobsCOM.Recordset
            Dim oEditTextColumn As SAPbouiCOM.EditTextColumn
            Dim oEdittext As SAPbouiCOM.EditText
            Dim strqry As String
            If aChoice = "Candidate" Then
                oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                strqry = "Select U_Z_HRAppID,U_Z_HRAppName,U_Z_Dob,U_Z_Email,U_Z_YrExp,U_Z_Mobile,U_Z_Skills,case U_Z_ApplStatus when 'O' then 'Open' when 'R' then 'Rejected' when 'S' then 'Selected' when 'P' then 'Placement'"
                strqry = strqry & " else 'Closed' end as U_Z_ApplStatus from [@Z_HR_OHEM1] where U_Z_HRAppID ='" & canjobid & "'"
                oTemp.DoQuery(strqry)
                If oTemp.RecordCount > 0 Then
                    oApplication.Utilities.setEdittextvalue(aform, "5", oTemp.Fields.Item(0).Value)
                    oApplication.Utilities.setEdittextvalue(aform, "7", oTemp.Fields.Item(1).Value)
                    oApplication.Utilities.setEdittextvalue(aform, "9", oTemp.Fields.Item(2).Value)
                    oApplication.Utilities.setEdittextvalue(aform, "11", oTemp.Fields.Item(3).Value)
                    oApplication.Utilities.setEdittextvalue(aform, "13", oTemp.Fields.Item(5).Value)
                    oApplication.Utilities.setEdittextvalue(aform, "15", oTemp.Fields.Item(4).Value)
                    oApplication.Utilities.setEdittextvalue(aform, "17", oTemp.Fields.Item(6).Value)
                    oApplication.Utilities.setEdittextvalue(aform, "28", oTemp.Fields.Item(7).Value)
                End If
                oGrid = aform.Items.Item("3").Specific
                oGrid.DataTable = aform.DataSources.DataTables.Item("DT_0")
                strqry = " select U_Z_ReqNo,U_Z_Dept,U_Z_DeptName,U_Z_JobPosi,U_Z_HRAppID,U_Z_HRAppName,U_Z_Dob,U_Z_Email,U_Z_YrExp,U_Z_Mobile,"
                strqry = strqry & " U_Z_Skills,case U_Z_ApplStatus when 'O' then 'Open' when 'R' then 'Rejected' when 'S' then 'Selected' when 'P' then 'Placement'"
                strqry = strqry & " end as U_Z_ApplStatus,DocEntry 'Code' from [@Z_HR_OHEM1] where U_Z_HRAppID ='" & canjobid & "' "
                oGrid.DataTable.ExecuteQuery(strqry)
                oGrid.Columns.Item("U_Z_ReqNo").TitleObject.Caption = "Job Request Id"
                oGrid.Columns.Item("U_Z_Dept").Visible = False
                oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
                oGrid.Columns.Item("U_Z_JobPosi").TitleObject.Caption = "Position"
                oGrid.Columns.Item("U_Z_HRAppID").Visible = False
                oGrid.Columns.Item("U_Z_HRAppName").Visible = False
                oGrid.Columns.Item("U_Z_Email").Visible = False
                oGrid.Columns.Item("U_Z_Mobile").Visible = False
                oGrid.Columns.Item("U_Z_Dob").Visible = False
                oGrid.Columns.Item("U_Z_YrExp").Visible = False
                oGrid.Columns.Item("U_Z_Skills").Visible = False
                oGrid.Columns.Item("U_Z_ApplStatus").Visible = False
                'oGrid.Columns.Item("U_Z_1stRounddt").TitleObject.Caption = "1st Round Scheduled Date"
                'oGrid.Columns.Item("U_Z_1stRoundRem").TitleObject.Caption = "1st Round Remarks"
                'oGrid.Columns.Item("U_Z_1stRoundStatus").TitleObject.Caption = "1st Round Status"
                'oGrid.Columns.Item("U_Z_2ndRounddt").TitleObject.Caption = "2nd Round Scheduled Date"
                'oGrid.Columns.Item("U_Z_2ndRoundRem").TitleObject.Caption = "2nd Round Remarks"
                'oGrid.Columns.Item("U_Z_2ndRoundStatus").TitleObject.Caption = "2nd Round Status"
                'oGrid.Columns.Item("U_Z_3rdRounddt").TitleObject.Caption = "3rd Round Scheduled Date"
                'oGrid.Columns.Item("U_Z_3rdRoundRem").TitleObject.Caption = "3rd Round Remarks"
                'oGrid.Columns.Item("U_Z_3rdRoundStatus").TitleObject.Caption = "3rd Round Status"
                oGrid.AutoResizeColumns()
                oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                oGrid.Columns.Item("Code").Visible = False
            Else
                oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                strqry = " Select DocEntry,U_Z_DeptCode,U_Z_DeptName,isnull(U_Z_PosName,'') +''+ISNULL(U_Z_NewPosi,'') from [@Z_HR_ORMPREQ] where DocEntry='" & canjobid & "'"
                oTemp.DoQuery(strqry)
                If oTemp.RecordCount > 0 Then
                    oApplication.Utilities.setEdittextvalue(aform, "19", oTemp.Fields.Item(0).Value)
                    oApplication.Utilities.setEdittextvalue(aform, "21", oTemp.Fields.Item(1).Value)
                    oApplication.Utilities.setEdittextvalue(aform, "23", oTemp.Fields.Item(2).Value)
                    oApplication.Utilities.setEdittextvalue(aform, "25", oTemp.Fields.Item(3).Value)
                End If
                oGrid = aform.Items.Item("26").Specific
                oGrid.DataTable = aform.DataSources.DataTables.Item("DT_1")
                strqry = " select U_Z_ReqNo,U_Z_Dept,U_Z_DeptName,U_Z_JobPosi ,U_Z_HRAppID,U_Z_HRAppName,U_Z_Dob,U_Z_Email,U_Z_YrExp,U_Z_Mobile,"
                strqry = strqry & " U_Z_Skills, U_Z_ApplStatus ,DocEntry 'Code' "
                strqry = strqry & " from [@Z_HR_OHEM1] where U_Z_ReqNo ='" & canjobid & "' "
                oGrid.DataTable.ExecuteQuery(strqry)
                oGrid.Columns.Item("U_Z_ReqNo").TitleObject.Caption = "Job Request Id"
                oGrid.Columns.Item("U_Z_ReqNo").Visible = False
                oGrid.Columns.Item("U_Z_Dept").TitleObject.Caption = "Department"
                oGrid.Columns.Item("U_Z_Dept").Visible = False
                oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
                oGrid.Columns.Item("U_Z_DeptName").Visible = False
                oGrid.Columns.Item("U_Z_JobPosi").TitleObject.Caption = "Position"
                oGrid.Columns.Item("U_Z_JobPosi").Visible = False
                oGrid.Columns.Item("U_Z_HRAppID").TitleObject.Caption = "Applicant Id"
                oGrid.Columns.Item("U_Z_HRAppID").Editable = False
                oEditTextColumn = oGrid.Columns.Item("U_Z_HRAppID")
                oEditTextColumn.LinkedObjectType = SAPbouiCOM.BoLinkedObject.lf_Employee
                oGrid.Columns.Item("U_Z_HRAppName").TitleObject.Caption = "Applicant Name "
                oGrid.Columns.Item("U_Z_HRAppName").Editable = False
                oGrid.Columns.Item("U_Z_Email").TitleObject.Caption = " Email Id"
                oGrid.Columns.Item("U_Z_Email").Editable = False
                oGrid.Columns.Item("U_Z_Mobile").TitleObject.Caption = "Mobile No"
                oGrid.Columns.Item("U_Z_Mobile").Editable = False
                oGrid.Columns.Item("U_Z_Dob").TitleObject.Caption = "Date of Birth"
                oGrid.Columns.Item("U_Z_Dob").Editable = False
                oGrid.Columns.Item("U_Z_YrExp").TitleObject.Caption = "Year of Experience"
                oGrid.Columns.Item("U_Z_YrExp").Editable = False
                oGrid.Columns.Item("U_Z_Skills").TitleObject.Caption = "Skill Sets"
                oGrid.Columns.Item("U_Z_Skills").Editable = False
                If strTitle = "HR Requisition Approval" Then
                    oGrid.Columns.Item("U_Z_ApplStatus").TitleObject.Caption = "Applicant Status"
                    oGrid.Columns.Item("U_Z_ApplStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    ocombo = oGrid.Columns.Item("U_Z_ApplStatus")
                    ocombo.ValidValues.Add("O", "Open")
                    ocombo.ValidValues.Add("S", "Selected")
                    ocombo.ValidValues.Add("R", "Rejected")
                    ocombo.ValidValues.Add("P", "Placement")
                    ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oGrid.Columns.Item("U_Z_ApplStatus").Editable = True
                Else
                    oGrid.Columns.Item("U_Z_ApplStatus").TitleObject.Caption = "Applicant Status"
                    oGrid.Columns.Item("U_Z_ApplStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    ocombo = oGrid.Columns.Item("U_Z_ApplStatus")
                    ocombo.ValidValues.Add("O", "Open")
                    ocombo.ValidValues.Add("S", "Selected")
                    ocombo.ValidValues.Add("R", "Rejected")
                    ocombo.ValidValues.Add("P", "Placement")
                    ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oGrid.Columns.Item("U_Z_ApplStatus").Editable = False
                End If


                'oGrid.Columns.Item("U_Z_1stRounddt").TitleObject.Caption = "1st Round Scheduled Date"
                'oGrid.Columns.Item("U_Z_1stRounddt").Editable = True
                'oGrid.Columns.Item("U_Z_1stRoundRem").TitleObject.Caption = "1st Round Remarks"
                'oGrid.Columns.Item("U_Z_1stRoundRem").Editable = True
                'oGrid.Columns.Item("U_Z_1stRoundStatus").TitleObject.Caption = "1st Round Status"
                'oGrid.Columns.Item("U_Z_1stRoundStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                'ocombo = oGrid.Columns.Item("U_Z_1stRoundStatus")
                'ocombo.ValidValues.Add("O", "Open")
                'ocombo.ValidValues.Add("S", "Selected")
                'ocombo.ValidValues.Add("R", "Rejected")
                'ocombo.ValidValues.Add("C", "Canceled")
                'ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                'oGrid.Columns.Item("U_Z_1stRoundStatus").Editable = True
                'oGrid.Columns.Item("U_Z_2ndRounddt").TitleObject.Caption = "2nd Round Scheduled Date"
                'oGrid.Columns.Item("U_Z_2ndRounddt").Editable = True
                'oGrid.Columns.Item("U_Z_2ndRoundRem").TitleObject.Caption = "2nd Round Remarks"
                'oGrid.Columns.Item("U_Z_2ndRoundRem").Editable = True
                'oGrid.Columns.Item("U_Z_2ndRoundStatus").TitleObject.Caption = "2nd Round Status"
                'oGrid.Columns.Item("U_Z_2ndRoundStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                'ocombo = oGrid.Columns.Item("U_Z_2ndRoundStatus")
                'ocombo.ValidValues.Add("O", "Open")
                'ocombo.ValidValues.Add("S", "Selected")
                'ocombo.ValidValues.Add("R", "Rejected")
                'ocombo.ValidValues.Add("C", "Canceled")
                'ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                'oGrid.Columns.Item("U_Z_2ndRoundStatus").Editable = True
                'oGrid.Columns.Item("U_Z_3rdRounddt").TitleObject.Caption = "3rd Round Scheduled Date"
                'oGrid.Columns.Item("U_Z_3rdRounddt").Editable = True
                'oGrid.Columns.Item("U_Z_3rdRoundRem").TitleObject.Caption = "3rd Round Remarks"
                'oGrid.Columns.Item("U_Z_3rdRoundRem").Editable = True
                'oGrid.Columns.Item("U_Z_3rdRoundStatus").TitleObject.Caption = "3rd Round Status"
                'oGrid.Columns.Item("U_Z_3rdRoundStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                'ocombo = oGrid.Columns.Item("U_Z_3rdRoundStatus")
                'ocombo.ValidValues.Add("O", "Open")
                'ocombo.ValidValues.Add("S", "Selected")
                'ocombo.ValidValues.Add("R", "Rejected")
                'ocombo.ValidValues.Add("C", "Canceled")
                'ocombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                'oGrid.Columns.Item("U_Z_3rdRoundStatus").Editable = True
                oGrid.AutoResizeColumns()
                oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                oGrid.Columns.Item("Code").Visible = False
            End If
        Catch ex As Exception
        End Try
    End Sub
#Region "Populate Business Objectives"
    Public Sub PopulateBusinessObjectives(ByVal aEmpId As String, ByVal aForm As SAPbouiCOM.Form)
        Dim strSQL, strSQL1 As String
        Dim oRec, oRectemp As SAPbobsCOM.Recordset
        Dim oMatrix As SAPbouiCOM.Matrix
        Dim intDeptCode As Integer
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRectemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        '  oRec.DoQuery("Select * from OHEM where empid=" & CInt(aEmpId))
        oRec.DoQuery("select  t1.U_Z_DeptCode  from [@Z_HR_OPOSCO] T1 inner join OHEM   T0 on T0.U_Z_HR_JobstCode=T1.U_Z_PosCode  where empid=" & CInt(aEmpId))
        If oRec.RecordCount > 0 Then
            intDeptCode = oRec.Fields.Item("U_Z_DeptCode").Value
            oMatrix = aForm.Items.Item("17").Specific
            oMatrix.Clear()
            strSQL1 = "SELECT T0.[U_Z_DeptCode], T0.[U_Z_DeptName], T1.[U_Z_BussCode], T1.[U_Z_BussName], T1.[U_Z_Weight] FROM [dbo].[@Z_HR_ODEMA]  T0  inner Join  [dbo].[@Z_HR_DEMA1]  T1 on T1.DocEntry=T0.DocEntry and T0.U_Z_DeptCode='" & intDeptCode & "'"
            oRectemp.DoQuery(strSQL1)
            For intRow As Integer = 0 To oRectemp.RecordCount - 1
                oMatrix.AddRow()
                SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_BussCode").Value)
                SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_BussName").Value)
                SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_Weight").Value)
                oRectemp.MoveNext()
            Next
        End If
        ' aForm.Items.Item("17").Enabled = False
    End Sub

    Public Sub PopulatePeopleObjectives(ByVal aEmpId As String, ByVal aForm As SAPbouiCOM.Form)
        Dim strSQL, strSQL1 As String
        Dim oRec, oRectemp As SAPbobsCOM.Recordset
        Dim oMatrix As SAPbouiCOM.Matrix
        Dim oCombobox As SAPbouiCOM.ComboBox
        Dim intDeptCode As Integer
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRectemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRec.DoQuery("Select * from OHEM where empid=" & CInt(aEmpId))
        If oRec.RecordCount > 0 Then
            intDeptCode = oRec.Fields.Item("Dept").Value
            oMatrix = aForm.Items.Item("24").Specific
            oMatrix.Clear()
            strSQL1 = "SELECT T0.[U_Z_HREmpID], T0.[U_Z_HRPeoobjCode], T0.[U_Z_HRPeoobjName], T0.[U_Z_HRPeoCategory], T0.[U_Z_HRWeight] FROM [dbo].[@Z_HR_PEOBJ1]  T0 where T0.U_Z_HREmpID='" & aEmpId & "'"

            oRectemp.DoQuery(strSQL1)
            For intRow As Integer = 0 To oRectemp.RecordCount - 1
                oMatrix.AddRow()
                SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_HRPeoobjCode").Value)
                SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_HRPeoobjName").Value)
                SetMatrixValues(oMatrix, "V_3", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_HRWeight").Value)
                oCombobox = oMatrix.Columns.Item("V_2").Cells.Item(oMatrix.RowCount).Specific
                oCombobox.Select(oRectemp.Fields.Item("U_Z_HRPeoCategory").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
                oRectemp.MoveNext()
            Next
            ' oMatrix.Columns.Item("V_0").Editable = False
        End If
        ' aForm.Items.Item("24").Enabled = False
    End Sub

    Public Sub PopulateCompetenceObjectives(ByVal aEmpId As String, ByVal aForm As SAPbouiCOM.Form)
        Dim strSQL, strSQL1 As String
        Dim oRec, oRectemp As SAPbobsCOM.Recordset
        Dim oMatrix As SAPbouiCOM.Matrix
        Dim oCombobox As SAPbouiCOM.ComboBox
        Dim intJobCode, strLevel As String
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRectemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        '  oRec.DoQuery("Select * from OHEM where empid=" & CInt(aEmpId))
        oRec.DoQuery("select U_Z_HR_JobstCode  from  OHEM  where empid=" & CInt(aEmpId))
        If oRec.RecordCount > 0 Then
            intJobCode = oRec.Fields.Item("U_Z_HR_JobstCode").Value
            oMatrix = aForm.Items.Item("31").Specific
            oMatrix.Clear()
            strSQL1 = "SELECT T1.[U_Z_CompCode], T1.[U_Z_CompDesc], T1.[U_Z_Weight],T1.[U_Z_CompLevel] FROM [dbo].[@Z_HR_OPOSCO]  T0  inner Join  [dbo].[@Z_HR_POSCO1]  T1 on T1.DocEntry=T0.DocEntry and T0.U_Z_PosCode='" & intJobCode & "'"
            oRectemp.DoQuery(strSQL1)
            For intRow As Integer = 0 To oRectemp.RecordCount - 1
                oMatrix.AddRow()
                SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_CompCode").Value)
                SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_CompDesc").Value)
                SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_Weight").Value)
                strLevel = oRectemp.Fields.Item("U_Z_CompLevel").Value
                oCombobox = oMatrix.Columns.Item("V_6").Cells.Item(oMatrix.RowCount).Specific
                oCombobox.Select(strLevel, SAPbouiCOM.BoSearchKey.psk_ByValue)
                'SetMatrixValues(oMatrix, "V_6", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_Weight").Value)
                oRectemp.MoveNext()
            Next
        End If
        ' aForm.Items.Item("17").Enabled = False
    End Sub

#End Region

#Region "Populate Business Manpower Request Objectives"
    Public Sub PopulateMPRBusinessObjectives(ByVal PosCode As String, ByVal aForm As SAPbouiCOM.Form)
        Dim strSQL, strSQL1 As String
        Dim oRec, oRectemp As SAPbobsCOM.Recordset
        Dim oMatrix As SAPbouiCOM.Matrix
        Dim intDeptCode As String
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRectemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        '  oRec.DoQuery("Select * from OHEM where empid=" & CInt(aEmpId))
        oRec.DoQuery("select T1.U_Z_DeptCode  from [@Z_HR_OPOSCO] T0 inner join [@Z_HR_OPOSIN] T1 on T0.U_Z_PosCode=T1.U_Z_JobCode where T1.U_Z_PosCode = '" & PosCode & "'")
        If oRec.RecordCount > 0 Then
            intDeptCode = oRec.Fields.Item("U_Z_DeptCode").Value
            oMatrix = aForm.Items.Item("31").Specific
            oMatrix.Clear()
            strSQL1 = "SELECT T0.[U_Z_DeptCode], T0.[U_Z_DeptName], T1.[U_Z_BussCode], T1.[U_Z_BussName], T1.[U_Z_Weight] FROM [dbo].[@Z_HR_ODEMA]  T0  inner Join  [dbo].[@Z_HR_DEMA1]  T1 on T1.DocEntry=T0.DocEntry and T0.U_Z_DeptCode='" & intDeptCode & "'"
            oRectemp.DoQuery(strSQL1)
            For intRow As Integer = 0 To oRectemp.RecordCount - 1
                oMatrix.AddRow()
                SetMatrixValues(oMatrix, "V_-1", oMatrix.RowCount, intRow + 1)
                SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_BussCode").Value)
                SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_BussName").Value)
                SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_Weight").Value)
                oRectemp.MoveNext()
            Next
        End If
        ' aForm.Items.Item("17").Enabled = False
    End Sub

    Public Sub PopulateMPRPeopleObjectives(ByVal aEmpId As String, ByVal aForm As SAPbouiCOM.Form)
        Dim strSQL, strSQL1 As String
        Dim oRec, oRectemp As SAPbobsCOM.Recordset
        Dim oMatrix As SAPbouiCOM.Matrix
        Dim oCombobox As SAPbouiCOM.ComboBox
        Dim intDeptCode As Integer
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRectemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRec.DoQuery("Select * from OHEM where empid=" & CInt(aEmpId))
        If oRec.RecordCount > 0 Then
            '  intDeptCode = oRec.Fields.Item("Dept").Value
            oMatrix = aForm.Items.Item("32").Specific
            oMatrix.Clear()
            strSQL1 = "SELECT T0.[U_Z_HREmpID], T0.[U_Z_HRPeoobjCode], T0.[U_Z_HRPeoobjName], T0.[U_Z_HRPeoCategory], T0.[U_Z_HRWeight] FROM [dbo].[@Z_HR_PEOBJ1]  T0 where T0.U_Z_HREmpID='" & aEmpId & "'"
            oRectemp.DoQuery(strSQL1)
            For intRow As Integer = 0 To oRectemp.RecordCount - 1
                oMatrix.AddRow()
                SetMatrixValues(oMatrix, "V_-1", oMatrix.RowCount, intRow + 1)
                SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_HRPeoobjCode").Value)
                SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_HRPeoobjName").Value)
                SetMatrixValues(oMatrix, "V_3", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_HRWeight").Value)
                oCombobox = oMatrix.Columns.Item("V_2").Cells.Item(oMatrix.RowCount).Specific
                oCombobox.Select(oRectemp.Fields.Item("U_Z_HRPeoCategory").Value, SAPbouiCOM.BoSearchKey.psk_ByValue)
                oRectemp.MoveNext()
            Next
            ' oMatrix.Columns.Item("V_0").Editable = False
        End If
        ' aForm.Items.Item("24").Enabled = False
    End Sub

    Public Sub PopulateMPRCompetenceObjectives(ByVal strPoscode As String, ByVal aForm As SAPbouiCOM.Form)
        Dim strSQL, strSQL1 As String
        Dim oRec, oRectemp As SAPbobsCOM.Recordset
        Dim oMatrix As SAPbouiCOM.Matrix
        Dim oCombobox As SAPbouiCOM.ComboBox
        Dim intJobCode, strLevel As String
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRectemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        '  oRec.DoQuery("Select * from OHEM where empid=" & CInt(aEmpId))
        oRec.DoQuery("select U_Z_JobCode  from [@Z_HR_OPOSIN] where U_Z_PosCode ='" & strPoscode & "'")
        If oRec.RecordCount > 0 Then
            intJobCode = oRec.Fields.Item("U_Z_JobCode").Value
            oMatrix = aForm.Items.Item("37").Specific
            oMatrix.Clear()
            strSQL1 = "SELECT T1.[U_Z_CompCode], T1.[U_Z_CompDesc], T1.[U_Z_Weight],T1.[U_Z_CompLevel] FROM [dbo].[@Z_HR_OPOSCO]  T0  inner Join  [dbo].[@Z_HR_POSCO1]  T1 on T1.DocEntry=T0.DocEntry and T0.U_Z_PosCode='" & intJobCode & "'"
            oRectemp.DoQuery(strSQL1)
            For intRow As Integer = 0 To oRectemp.RecordCount - 1
                oMatrix.AddRow()
                SetMatrixValues(oMatrix, "V_-1", oMatrix.RowCount, intRow + 1)
                SetMatrixValues(oMatrix, "V_0", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_CompCode").Value)
                SetMatrixValues(oMatrix, "V_1", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_CompDesc").Value)
                SetMatrixValues(oMatrix, "V_2", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_Weight").Value)
                strLevel = oRectemp.Fields.Item("U_Z_CompLevel").Value
                oCombobox = oMatrix.Columns.Item("V_4").Cells.Item(oMatrix.RowCount).Specific
                oCombobox.Select(strLevel, SAPbouiCOM.BoSearchKey.psk_ByValue)
                'SetMatrixValues(oMatrix, "V_6", oMatrix.RowCount, oRectemp.Fields.Item("U_Z_Weight").Value)
                oRectemp.MoveNext()
            Next
        End If
        ' aForm.Items.Item("17").Enabled = False
    End Sub

#End Region


#Region "Close Open Sales Order Lines"

    Public Sub WriteErrorlog(ByVal aMessage As String, ByVal aPath As String)
        Dim aSw As System.IO.StreamWriter
        Try
            If File.Exists(aPath) Then
            End If
            aSw = New StreamWriter(aPath, True)
            aSw.WriteLine(aMessage)
            aSw.Flush()
            aSw.Close()
            aSw.Dispose()
        Catch ex As Exception
            MsgBox("test")
        End Try
    End Sub


    Public Function GetDateTimeValue(ByVal DateString As String) As DateTime
        Dim objBridge As SAPbobsCOM.SBObob
        objBridge = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge)
        Return objBridge.Format_StringToDate(DateString).Fields.Item(0).Value
    End Function

    Public Sub createARINvoice()
        Dim strCardcode, stritemcode As String
        Dim intbaseEntry, intbaserow As Integer
        Dim oInv As SAPbobsCOM.Documents
        strCardcode = "C20000"
        intbaseEntry = 66
        intbaserow = 1
        oInv = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices)
        oInv.DocDate = Now.Date
        oInv.CardCode = strCardcode
        oInv.Lines.BaseType = 17
        oInv.Lines.BaseEntry = intbaseEntry
        oInv.Lines.BaseLine = intbaserow
        oInv.Lines.Quantity = 1
        If oInv.Add <> 0 Then
            oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Else
            oApplication.Utilities.Message("AR Invoice added", SAPbouiCOM.BoStatusBarMessageType.smt_Success)

        End If

    End Sub
    Public Sub CloseOpenSOLines()
        Try
            Dim oDoc As SAPbobsCOM.Documents
            Dim oTemp As SAPbobsCOM.Recordset
            Dim strSQL, strSQL1, spath As String
            oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            spath = System.Windows.Forms.Application.StartupPath & "\Sales Order Matching ErrorLog.txt"
            If File.Exists(spath) Then
                File.Delete(spath)
            End If
            blnError = False
            ' oTemp.DoQuery("Select DocEntry,LineNum from RDR1 where isnull(trgetentry,0)=0 and  LineStatus='O' and Quantity = isnull(U_RemQty,0) order by DocEntry,LineNum")
            '            oTemp.DoQuery("Select DocEntry,VisOrder,LineNum from RDR1 where isnull(trgetentry,0)=0 and  LineStatus='O' and Quantity = isnull(U_RemQty,0) order by DocEntry,LineNum")
            oTemp.DoQuery("Select DocEntry,VisOrder,LineNum from RDR1 where   LineStatus='O' and Quantity = isnull(U_RemQty,0) order by DocEntry,LineNum")
            oApplication.Utilities.Message("Processing closing Sales order Lines", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            Dim numb As Integer
            For introw As Integer = 0 To oTemp.RecordCount - 1
                oDoc = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders)
                numb = oTemp.Fields.Item(1).Value
                '  numb = oTemp.Fields.Item(2).Value
                If oDoc.GetByKey(oTemp.Fields.Item("DocEntry").Value) Then
                    oApplication.Utilities.Message("Processing Sales order :" & oDoc.DocNum, SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                    oDoc.Comments = oDoc.Comments & "XXX1"
                    If oDoc.Update() <> 0 Then
                        WriteErrorlog(" Error in Closing Sales order Line  SO No :" & oDoc.DocNum & " : Line No : " & oDoc.Lines.LineNum & " : Error : " & oApplication.Company.GetLastErrorDescription, spath)
                        blnError = True
                    Else
                        oDoc = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders)
                        If oDoc.GetByKey(oTemp.Fields.Item("DocEntry").Value) Then
                            Dim strcomments As String
                            strcomments = oDoc.Comments
                            strcomments = strcomments.Replace("XXX1", "")
                            oDoc.Comments = strcomments
                            oDoc.Lines.SetCurrentLine(numb)
                            '  MsgBox(oDoc.Lines.VisualOrder)
                            If oDoc.Lines.LineStatus <> SAPbobsCOM.BoStatus.bost_Close Then
                                oDoc.Lines.LineStatus = SAPbobsCOM.BoStatus.bost_Close
                            End If
                            If oDoc.Update <> 0 Then
                                WriteErrorlog(" Error in Closing Sales order Line  SO No :" & oDoc.DocNum & " : Line No : " & oDoc.Lines.LineNum & " : Error : " & oApplication.Company.GetLastErrorDescription, spath)
                                blnError = True
                                'oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            Else
                                WriteErrorlog(" Sales order Line  SO No :" & oDoc.DocNum & " : Line No : " & oDoc.Lines.LineNum & " : Closed successfully  ", spath)
                            End If
                        End If
                    End If

                End If
                oTemp.MoveNext()
            Next
            oApplication.Utilities.Message("Operation completed succesfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)
            blnError = True
            ' oApplication.SBO_Application.MessageBox("Error Occured...")\
            spath = System.Windows.Forms.Application.StartupPath & "\Sales Order Matching ErrorLog.txt"
            If File.Exists(spath) Then
                Dim x As System.Diagnostics.ProcessStartInfo
                x = New System.Diagnostics.ProcessStartInfo
                x.UseShellExecute = True

                x.FileName = spath
                System.Diagnostics.Process.Start(x)
                x = Nothing
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
#End Region

#Region "AddControls"
    Public Sub AddControls(ByVal objForm As SAPbouiCOM.Form, ByVal ItemUID As String, ByVal SourceUID As String, ByVal ItemType As SAPbouiCOM.BoFormItemTypes, ByVal position As String, Optional ByVal fromPane As Integer = 1, Optional ByVal toPane As Integer = 1, Optional ByVal linkedUID As String = "", Optional ByVal strCaption As String = "", Optional ByVal dblWidth As Double = 0, Optional ByVal dblTop As Double = 0, Optional ByVal Hight As Double = 0)
        Dim objNewItem, objOldItem As SAPbouiCOM.Item
        Dim ostatic As SAPbouiCOM.StaticText
        Dim oButton As SAPbouiCOM.Button
        Dim oCheckbox As SAPbouiCOM.CheckBox
        Dim ofolder As SAPbouiCOM.Folder
        objOldItem = objForm.Items.Item(SourceUID)
        objNewItem = objForm.Items.Add(ItemUID, ItemType)
        With objNewItem
            If ItemType = SAPbouiCOM.BoFormItemTypes.it_LINKED_BUTTON Then
                .Left = objOldItem.Left - 15
                .Top = objOldItem.Top + 1
                .LinkTo = linkedUID
            Else
                If position.ToUpper = "RIGHT" Then
                    .Left = objOldItem.Left + objOldItem.Width + 2
                    .Top = objOldItem.Top

                ElseIf position.ToUpper = "DOWN" Then
                    If ItemUID = "edWork" Then
                        .Left = objOldItem.Left + 40
                    Else
                        .Left = objOldItem.Left
                    End If
                    .Top = objOldItem.Top + objOldItem.Height + 3

                    .Width = objOldItem.Width
                    .Height = objOldItem.Height
                End If
            End If
            .FromPane = fromPane
            .ToPane = toPane
            If ItemType = SAPbouiCOM.BoFormItemTypes.it_STATIC Then
                .LinkTo = linkedUID
            End If
            .LinkTo = linkedUID
        End With
        If (ItemType = SAPbouiCOM.BoFormItemTypes.it_EDIT Or ItemType = SAPbouiCOM.BoFormItemTypes.it_STATIC) Then
            objNewItem.Width = objOldItem.Width
        End If
        If ItemType = SAPbouiCOM.BoFormItemTypes.it_BUTTON Then
            objNewItem.Width = objOldItem.Width '+ 50
            oButton = objNewItem.Specific
            oButton.Caption = strCaption
        ElseIf ItemType = SAPbouiCOM.BoFormItemTypes.it_FOLDER Then
            ofolder = objNewItem.Specific
            ofolder.Caption = strCaption
            ofolder.GroupWith(linkedUID)
        ElseIf ItemType = SAPbouiCOM.BoFormItemTypes.it_STATIC Then
            ostatic = objNewItem.Specific
            ostatic.Caption = strCaption
        ElseIf ItemType = SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX Then
            oCheckbox = objNewItem.Specific
            oCheckbox.Caption = strCaption

        End If
        If dblWidth <> 0 Then
            objNewItem.Width = dblWidth
        End If

        If dblTop <> 0 Then
            objNewItem.Top = objNewItem.Top + dblTop
        End If
        If Hight <> 0 Then
            objNewItem.Height = objNewItem.Height + Hight
        End If
    End Sub
#End Region


#Region "Connect to Company"
    Public Sub Connect()
        Dim strCookie As String
        Dim strConnectionContext As String

        Try
            strCookie = oApplication.Company.GetContextCookie
            strConnectionContext = oApplication.SBO_Application.Company.GetConnectionContext(strCookie)

            If oApplication.Company.SetSboLoginContext(strConnectionContext) <> 0 Then
                Throw New Exception("Wrong login credentials.")
            End If

            'Open a connection to company
            If oApplication.Company.Connect() <> 0 Then
                Throw New Exception("Cannot connect to company database. ")
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub
#End Region

#Region "Genral Functions"

#Region "Get MaxCode"
    Public Function getMaxCode(ByVal sTable As String, ByVal sColumn As String) As String
        Dim oRS As SAPbobsCOM.Recordset
        Dim MaxCode As Integer
        Dim sCode As String
        Dim strSQL As String
        Try
            strSQL = "SELECT MAX(CAST(" & sColumn & " AS Numeric)) FROM [" & sTable & "]"
            ExecuteSQL(oRS, strSQL)

            If Convert.ToString(oRS.Fields.Item(0).Value).Length > 0 Then
                MaxCode = oRS.Fields.Item(0).Value + 1
            Else
                MaxCode = 1
            End If

            sCode = Format(MaxCode, "00000000")
            Return sCode
        Catch ex As Exception
            Throw ex
        Finally
            oRS = Nothing
        End Try
    End Function

    Public Function getMaxCode_lineNo(ByVal sTable As String, ByVal sColumn As String, ByVal aEntry As Integer) As String
        Dim oRS As SAPbobsCOM.Recordset
        Dim MaxCode As Integer
        Dim sCode As String
        Dim strSQL As String
        Try
            strSQL = "SELECT MAX(CAST(" & sColumn & " AS Numeric)) FROM [" & sTable & "] where ""DocEntry""=" & aEntry
            ExecuteSQL(oRS, strSQL)

            If Convert.ToString(oRS.Fields.Item(0).Value).Length > 0 Then
                MaxCode = oRS.Fields.Item(0).Value + 1
            Else
                MaxCode = 1
            End If

            sCode = Format(MaxCode, "00000000")
            Return sCode
        Catch ex As Exception
            Throw ex
        Finally
            oRS = Nothing
        End Try
    End Function
#End Region

#Region "Status Message"
    Public Sub Message(ByVal sMessage As String, ByVal StatusType As SAPbouiCOM.BoStatusBarMessageType)
        oApplication.SBO_Application.StatusBar.SetText(sMessage, SAPbouiCOM.BoMessageTime.bmt_Short, StatusType)
    End Sub
#End Region

#Region "Add Choose from List"
    Public Sub AddChooseFromList(ByVal FormUID As String, ByVal CFL_Text As String, ByVal CFL_Button As String, _
                                        ByVal ObjectType As SAPbouiCOM.BoLinkedObject, _
                                            Optional ByVal AliasName As String = "", Optional ByVal CondVal As String = "", _
                                                    Optional ByVal Operation As SAPbouiCOM.BoConditionOperation = SAPbouiCOM.BoConditionOperation.co_EQUAL)

        Dim oCFLs As SAPbouiCOM.ChooseFromListCollection
        Dim oCons As SAPbouiCOM.Conditions
        Dim oCon As SAPbouiCOM.Condition
        Dim oCFL As SAPbouiCOM.ChooseFromList
        Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams
        Try
            oCFLs = oApplication.SBO_Application.Forms.Item(FormUID).ChooseFromLists
            oCFLCreationParams = oApplication.SBO_Application.CreateObject( _
                                    SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)

            ' Adding 2 CFL, one for the button and one for the edit text.
            If ObjectType = SAPbouiCOM.BoLinkedObject.lf_Items Then
                oCFLCreationParams.MultiSelection = True
            Else
                oCFLCreationParams.MultiSelection = False
            End If

            oCFLCreationParams.ObjectType = ObjectType
            oCFLCreationParams.UniqueID = CFL_Text

            oCFL = oCFLs.Add(oCFLCreationParams)

            ' Adding Conditions to CFL1

            oCons = oCFL.GetConditions()

            If Not AliasName = "" Then
                oCon = oCons.Add()
                oCon.Alias = AliasName
                oCon.Operation = Operation
                oCon.CondVal = CondVal
                oCFL.SetConditions(oCons)
            End If

            oCFLCreationParams.UniqueID = CFL_Button
            oCFL = oCFLs.Add(oCFLCreationParams)

        Catch ex As Exception
            Throw ex
        End Try
    End Sub
#End Region

#Region "Get Linked Object Type"
    Public Function getLinkedObjectType(ByVal Type As SAPbouiCOM.BoLinkedObject) As String
        Return CType(Type, String)
    End Function

#End Region

#Region "Execute Query"
    Public Sub ExecuteSQL(ByRef oRecordSet As SAPbobsCOM.Recordset, ByVal SQL As String)
        Try
            If oRecordSet Is Nothing Then
                oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            End If

            oRecordSet.DoQuery(SQL)

        Catch ex As Exception
            Throw ex
        End Try
    End Sub
#End Region

#Region "Get Application path"
    Public Function getApplicationPath() As String

        Return Application.StartupPath.Trim

        'Return IO.Directory.GetParent(Application.StartupPath).ToString
    End Function
#End Region

#Region "Date Manipulation"

#Region "Convert SBO Date to System Date"
    '********************************************************************
    'Type		            :   Public Procedure     
    'Name               	:	ConvertStrToDate
    'Parameter          	:   ByVal oDate As String, ByVal strFormat As String
    'Return Value       	:	
    'Author             	:	Manu
    'Created Date       	:	07/12/05
    'Last Modified By	    :	
    'Modified Date        	:	
    'Purpose             	:	To convert Date according to current culture info
    '********************************************************************
    Public Function ConvertStrToDate(ByVal strDate As String, ByVal strFormat As String) As DateTime
        Try
            Dim oDate As DateTime
            Dim ci As New System.Globalization.CultureInfo("en-GB", False)
            Dim newCi As System.Globalization.CultureInfo = CType(ci.Clone(), System.Globalization.CultureInfo)

            System.Threading.Thread.CurrentThread.CurrentCulture = newCi
            oDate = Date.ParseExact(strDate, strFormat, ci.DateTimeFormat)

            Return oDate
        Catch ex As Exception
            Throw ex
        End Try

    End Function
#End Region

#Region " Get SBO Date Format in String (ddmmyyyy)"
    '********************************************************************
    'Type		            :   Public Procedure     
    'Name               	:	StrSBODateFormat
    'Parameter          	:   none
    'Return Value       	:	
    'Author             	:	Manu
    'Created Date       	:	
    'Last Modified By	    :	
    'Modified Date        	:	
    'Purpose             	:	To get date Format(ddmmyy value) as applicable to SBO
    '********************************************************************
    Public Function StrSBODateFormat() As String
        Try
            Dim rsDate As SAPbobsCOM.Recordset
            Dim strsql As String, GetDateFormat As String
            Dim DateSep As Char

            strsql = "Select DateFormat,DateSep from OADM"
            rsDate = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            rsDate.DoQuery(strsql)
            DateSep = rsDate.Fields.Item(1).Value

            Select Case rsDate.Fields.Item(0).Value
                Case 0
                    GetDateFormat = "dd" & DateSep & "MM" & DateSep & "yy"
                Case 1
                    GetDateFormat = "dd" & DateSep & "MM" & DateSep & "yyyy"
                Case 2
                    GetDateFormat = "MM" & DateSep & "dd" & DateSep & "yy"
                Case 3
                    GetDateFormat = "MM" & DateSep & "dd" & DateSep & "yyyy"
                Case 4
                    GetDateFormat = "yyyy" & DateSep & "dd" & DateSep & "MM"
                Case 5
                    GetDateFormat = "dd" & DateSep & "MMM" & DateSep & "yyyy"
            End Select
            Return GetDateFormat

        Catch ex As Exception
            Throw ex
        End Try
    End Function
#End Region

#Region "Get SBO date Format in Number"
    '********************************************************************
    'Type		            :   Public Procedure     
    'Name               	:	IntSBODateFormat
    'Parameter          	:   none
    'Return Value       	:	
    'Author             	:	Manu
    'Created Date       	:	
    'Last Modified By	    :	
    'Modified Date        	:	
    'Purpose             	:	To get date Format(integer value) as applicable to SBO
    '********************************************************************
    Public Function NumSBODateFormat() As String
        Try
            Dim rsDate As SAPbobsCOM.Recordset
            Dim strsql As String
            Dim DateSep As Char

            strsql = "Select DateFormat,DateSep from OADM"
            rsDate = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            rsDate.DoQuery(strsql)
            DateSep = rsDate.Fields.Item(1).Value

            Select Case rsDate.Fields.Item(0).Value
                Case 0
                    NumSBODateFormat = 3
                Case 1
                    NumSBODateFormat = 103
                Case 2
                    NumSBODateFormat = 1
                Case 3
                    NumSBODateFormat = 120
                Case 4
                    NumSBODateFormat = 126
                Case 5
                    NumSBODateFormat = 130
            End Select
            Return NumSBODateFormat

        Catch ex As Exception
            Throw ex
        End Try
    End Function
#End Region

#End Region

#Region "Get Rental Period"
    Public Function getRentalDays(ByVal Date1 As String, ByVal Date2 As String, ByVal IsWeekDaysBilling As Boolean) As Integer
        Dim TotalDays, TotalDaysincSat, TotalBillableDays As Integer
        Dim TotalWeekEnds As Integer
        Dim StartDate As Date
        Dim EndDate As Date
        Dim oRecordset As SAPbobsCOM.Recordset

        StartDate = CType(Date1.Insert(4, "/").Insert(7, "/"), Date)
        EndDate = CType(Date2.Insert(4, "/").Insert(7, "/"), Date)

        TotalDays = DateDiff(DateInterval.Day, StartDate, EndDate)

        If IsWeekDaysBilling Then
            strSQL = " select dbo.WeekDays('" & Date1 & "','" & Date2 & "')"
            oApplication.Utilities.ExecuteSQL(oRecordset, strSQL)
            If oRecordset.RecordCount > 0 Then
                TotalBillableDays = oRecordset.Fields.Item(0).Value
            End If
            Return TotalBillableDays
        Else
            Return TotalDays + 1
        End If

    End Function

    Public Function WorkDays(ByVal dtBegin As Date, ByVal dtEnd As Date) As Long
        Try
            Dim dtFirstSunday As Date
            Dim dtLastSaturday As Date
            Dim lngWorkDays As Long

            ' get first sunday in range
            dtFirstSunday = dtBegin.AddDays((8 - Weekday(dtBegin)) Mod 7)

            ' get last saturday in range
            dtLastSaturday = dtEnd.AddDays(-(Weekday(dtEnd) Mod 7))

            ' get work days between first sunday and last saturday
            lngWorkDays = (((DateDiff(DateInterval.Day, dtFirstSunday, dtLastSaturday)) + 1) / 7) * 5

            ' if first sunday is not begin date
            If dtFirstSunday <> dtBegin Then

                ' assume first sunday is after begin date
                ' add workdays from begin date to first sunday
                lngWorkDays = lngWorkDays + (7 - Weekday(dtBegin))

            End If

            ' if last saturday is not end date
            If dtLastSaturday <> dtEnd Then

                ' assume last saturday is before end date
                ' add workdays from last saturday to end date
                lngWorkDays = lngWorkDays + (Weekday(dtEnd) - 1)

            End If

            WorkDays = lngWorkDays
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try


    End Function

#End Region

#Region "Get Item Price with Factor"
    Public Function getPrcWithFactor(ByVal CardCode As String, ByVal ItemCode As String, ByVal RntlDays As Integer, ByVal Qty As Double) As Double
        Dim oItem As SAPbobsCOM.Items
        Dim Price, Expressn As Double
        Dim oDataSet, oRecSet As SAPbobsCOM.Recordset

        oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
        oApplication.Utilities.ExecuteSQL(oDataSet, "Select U_RentFac, U_NumDys From [@REN_FACT] order by U_NumDys ")
        If oItem.GetByKey(ItemCode) And oDataSet.RecordCount > 0 Then

            oApplication.Utilities.ExecuteSQL(oRecSet, "Select ListNum from OCRD where CardCode = '" & CardCode & "'")
            oItem.PriceList.SetCurrentLine(oRecSet.Fields.Item(0).Value - 1)
            Price = oItem.PriceList.Price
            Expressn = 0
            oDataSet.MoveFirst()

            While RntlDays > 0

                If oDataSet.EoF Then
                    oDataSet.MoveLast()
                End If

                If RntlDays < oDataSet.Fields.Item(1).Value Then
                    Expressn += (oDataSet.Fields.Item(0).Value * RntlDays * Price * Qty)
                    RntlDays = 0
                    Exit While
                End If
                Expressn += (oDataSet.Fields.Item(0).Value * oDataSet.Fields.Item(1).Value * Price * Qty)
                RntlDays -= oDataSet.Fields.Item(1).Value
                oDataSet.MoveNext()

            End While

        End If
        If oItem.UserFields.Fields.Item("U_Rental").Value = "Y" Then
            Return CDbl(Expressn / Qty)
        Else
            Return Price
        End If


    End Function
#End Region

#Region "Get WareHouse List"
    Public Function getUsedWareHousesList(ByVal ItemCode As String, ByVal Quantity As Double) As DataTable
        Dim oDataTable As DataTable
        Dim oRow As DataRow
        Dim rswhs As SAPbobsCOM.Recordset
        Dim LeftQty As Double
        Try
            oDataTable = New DataTable
            oDataTable.Columns.Add(New System.Data.DataColumn("ItemCode"))
            oDataTable.Columns.Add(New System.Data.DataColumn("WhsCode"))
            oDataTable.Columns.Add(New System.Data.DataColumn("Quantity"))

            strSQL = "Select WhsCode, ItemCode, (OnHand + OnOrder - IsCommited) As Available From OITW Where ItemCode = '" & ItemCode & "' And " & _
                        "WhsCode Not In (Select Whscode From OWHS Where U_Reserved = 'Y' Or U_Rental = 'Y') Order By (OnHand + OnOrder - IsCommited) Desc "

            ExecuteSQL(rswhs, strSQL)
            LeftQty = Quantity

            While Not rswhs.EoF
                oRow = oDataTable.NewRow()

                oRow.Item("WhsCode") = rswhs.Fields.Item("WhsCode").Value
                oRow.Item("ItemCode") = rswhs.Fields.Item("ItemCode").Value

                LeftQty = LeftQty - CType(rswhs.Fields.Item("Available").Value, Double)

                If LeftQty <= 0 Then
                    oRow.Item("Quantity") = CType(rswhs.Fields.Item("Available").Value, Double) + LeftQty
                    oDataTable.Rows.Add(oRow)
                    Exit While
                Else
                    oRow.Item("Quantity") = CType(rswhs.Fields.Item("Available").Value, Double)
                End If

                oDataTable.Rows.Add(oRow)
                rswhs.MoveNext()
                oRow = Nothing
            End While

            'strSQL = ""
            'For count As Integer = 0 To oDataTable.Rows.Count - 1
            '    strSQL += oDataTable.Rows(count).Item("WhsCode") & " : " & oDataTable.Rows(count).Item("Quantity") & vbNewLine
            'Next
            'MessageBox.Show(strSQL)

            Return oDataTable

        Catch ex As Exception
            Throw ex
        Finally
            oRow = Nothing
        End Try
    End Function
#End Region

#Region "GetDocumentQuantity"
    Public Function getDocumentQuantity(ByVal strQuantity As String) As Double
        Dim dblQuant As Double
        Dim strTemp, strTemp1 As String
        Dim oRec As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRec.DoQuery("Select CurrCode  from OCRN")
        For intRow As Integer = 0 To oRec.RecordCount - 1
            strQuantity = strQuantity.Replace(oRec.Fields.Item(0).Value, "")
            oRec.MoveNext()
        Next
        strTemp1 = strQuantity
        strTemp = CompanyDecimalSeprator
        If CompanyDecimalSeprator <> "." Then
            If CompanyThousandSeprator <> strTemp Then
            End If
            strQuantity = strQuantity.Replace(".", ",")
        End If
        If strQuantity = "" Then
            Return 0
        End If
        Try
            dblQuant = Convert.ToDouble(strQuantity)
        Catch ex As Exception
            dblQuant = Convert.ToDouble(strTemp1)
        End Try

        Return dblQuant
    End Function
#End Region

    Public Sub AssignSerialNo(ByVal aMatrix As SAPbouiCOM.Matrix, ByVal aform As SAPbouiCOM.Form)
        aform.Freeze(True)
        For intRow As Integer = 1 To aMatrix.RowCount
            aMatrix.Columns.Item("SlNo").Cells.Item(intRow).Specific.value = intRow
        Next
        aform.Freeze(False)
    End Sub

    Public Sub AssignRowNo(ByVal aMatrix As SAPbouiCOM.Grid, ByVal aform As SAPbouiCOM.Form)
        aform.Freeze(True)
        For intRow As Integer = 0 To aMatrix.DataTable.Rows.Count - 1
            aMatrix.RowHeaders.SetText(intRow, intRow + 1)
        Next
        aform.Freeze(False)
    End Sub

#Region "ValidateCode"
    Public Function ValidateCode(ByVal aCode As String, ByVal aModule As String) As Boolean
        Dim oTemp As SAPbobsCOM.Recordset
        Dim strqry As String = ""
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If aModule = "Department" Then
            strqry = "Select * from ""@Z_HR_OPOSIN"" where ""U_Z_DeptCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Department Already mapped in Position Master...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If

            strqry = "Select * from OHEM where ""dept""=" & CInt(aCode)
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Allowance Code Already mapped in employee master...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "Branch" Then
            strqry = "Select * from ""@Z_HR_ORGST"" where ""U_Z_BranCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Branch Already mapped in Organisation Structer...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If

            strqry = "Select * from OHEM where ""branch""=" & CInt(aCode)
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Branch Code Already mapped in employee master...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "ALLOW" Then
            strqry = "Select * from ""@Z_HR_SALST1"" where ""U_Z_AllCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Allowance Code Already mapped in Salary Scale Master...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "RATING" Then
            strqry = "select * from ""@Z_HR_SEAPP1"" where ""U_Z_SelfRaCode""='" & aCode & "' or ""U_Z_MgrRaCode""='" & aCode & "' or ""U_Z_SMRaCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Rating Code Already mapped in Appraisals...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "EXPENCES" Then
            strqry = "select * from ""@Z_HR_TRAPL1"" where ""U_Z_ExpName""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Expences Already mapped in Travel Agenda...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "COURSE" Then
            strqry = "select * from ""@Z_HR_OTRIN"" where ""U_Z_CourseCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Course Code Already mapped in Training Agenda. Training Agenda Code : " & oTemp.Fields.Item("U_Z_TrainCode").Value, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "TRAINER" Then
            strqry = "select * from ""@Z_HR_OTRIN"" where ""U_Z_InsName""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Trainer Code Already mapped in Training Agenda. Training Agenda Code : " & oTemp.Fields.Item("U_Z_TrainCode").Value, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "TRAPLAN" Then
            strqry = "select * from ""@Z_HR_OASSTP"" where ""U_Z_TraCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Travel Agenda Code Already mapped in Employee Master. Employee Code : " & oTemp.Fields.Item("U_Z_EmpId").Value, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "TRAINAGENDA" Then
            strqry = "select * from ""@Z_HR_TRIN1"" where ""U_Z_TrainCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Training Code Already mapped in Employee Master. Employee Code : " & oTemp.Fields.Item("U_Z_HREmpID").Value, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "POSITION" Then
            strqry = "Select * from [@Z_HR_ORGST] where ""U_Z_PosCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Position Code already mapped in Organization Structure.Organization Code :" & oTemp.Fields.Item("U_Z_OrgCode").Value, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If

            strqry = "Select * from OHPS where ""Name""='" & aCode & "'"
            strqry = "SELECT *  FROM OHEM T0  INNER JOIN OHPS T1 ON T0.position = T1.posID WHERE T1.[name] ='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Position Code already mapped in Employee Master :", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If

        ElseIf aModule = "JOBSCREEN" Then
            strqry = "Select * from ""@Z_HR_OPOSIN"" where ""U_Z_JobCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Job Code already mapped in Position Master.Position Code :" & oTemp.Fields.Item("U_Z_PosCode").Value, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "SALARY" Then
            strqry = "Select * from ""@Z_HR_OPOSCO"" where ""U_Z_SalCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Salary Code Already mapped in Job Screen. Job Code  :" & oTemp.Fields.Item("U_Z_PosCode").Value, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "RECREQREASON" Then
            strqry = "select * from ""@Z_HR_ORMPREQ"" where ""U_Z_ReqReason""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Recruitment Request Reason Already mapped in Recruitment Requisition...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "INTRATING" Then
            strqry = "select * from ""@Z_HR_OHEM2"" where ""U_Z_Rating""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Interview Rating Code Already mapped in Interview Process form...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "INTERVIEWTYPE" Then
            strqry = "select * from ""@Z_HR_OHEM2"" where ""U_Z_InType""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Interview Type Already mapped in Interview Process form...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "RESPONSE" Then
            strqry = "Select * from ""@Z_HR_EXFORM1"" where ""U_Z_ResCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Responsibilities Code Already mapped in Employee exit initialization...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "QUS" Then
            strqry = "Select * from ""@Z_HR_EXFORM2"" where ""U_Z_QusCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Questionnaire Code Already mapped in Employee exit Interview form...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "LANG" Then
            strqry = "Select * from ""@Z_HR_RMPREQ5"" where ""U_Z_LanCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Language Code Already mapped in Recruitment Requisition...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "COUCAT" Then
            strqry = "Select * from ""@Z_HR_OCOUR"" where ""U_Z_CouCatCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Course Category Code Already mapped in Course Setup...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "COUTYP" Then
            strqry = "Select * from ""@Z_HR_OTRIN"" where ""U_Z_CourseTypeCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Course Type Code Already mapped in Training Agenda Setup...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "DEPT" Then
            strqry = "Select * from OUDP where ""Name""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                Return True
            End If
        ElseIf aModule = "BENEFIT" Then
            strqry = "Select * from ""@Z_HR_SALST2"" where ""U_Z_BeneCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Benefits Code Already mapped in Salary Scale Master...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "LEVEL" Then
            strqry = "Select * from ""@Z_HR_OSALST"" where ""U_Z_LevlCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Level Code Already mapped in Salary Scale Master...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "GRADE" Then
            strqry = "Select * from ""@Z_HR_OSALST"" where ""U_Z_GrdeCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Grade Code Already mapped in Salary Scale Master...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "OBJLOAN" Then
            strqry = "Select * from ""@Z_HR_OBJLOAN"" where ""U_Z_ObjCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Objects on Loan Code Already mapped in Employee Master...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "COMP" Then
            strqry = "Select * from ""@Z_HR_ORGST"" where ""U_Z_CompCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Company Code Already mapped in Organization Structure...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If

            strqry = "Select * from ""@Z_HR_OPOSIN"" where ""U_Z_CompCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Company Code Already mapped in Position...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If

        ElseIf aModule = "FUNC" Then
            strqry = "Select * from ""@Z_HR_ORGST"" where ""U_Z_FuncCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Function Code Already mapped in Organization Structure...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
            strqry = "Select * from ""@Z_HR_OPOSIN"" where ""U_Z_DivCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Function Code Already mapped in Position...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If

        ElseIf aModule = "UNIT" Then
            strqry = "Select * from ""@Z_HR_ORGST"" where ""U_Z_UnitCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Unit Code Already mapped in Organization Structure...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "LOC" Then
            strqry = "Select * from ""@Z_HR_ORGST"" where ""U_Z_LocCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Location Code Already mapped in Organization Structure...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If

        ElseIf aModule = "ORG" Then
            'strqry = "Select * from ""@Z_HR_OPOSCO"" where ""U_Z_OrgCode""='" & aCode & "'"
            'oTemp.DoQuery(strqry)
            'If oTemp.RecordCount > 0 Then
            '    oApplication.Utilities.Message("Organizational Code Already mapped in Job Screen...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '    Return True
            'End If
            strqry = "Select * from OHEM where ""U_Z_HR_OrgstCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Organizational Code Already mapped in Employee Master...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
            'strqry = "Select * from ""@Z_HR_OPOSIN"" where ""U_Z_OrgCode""='" & aCode & "'"
            'oTemp.DoQuery(strqry)
            'If oTemp.RecordCount > 0 Then
            '    Return True
            'End If


        ElseIf aModule = "BUSINESS" Then
            strqry = "Select * from ""@Z_HR_SEAPP1"" where ""U_Z_BussCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Business Objective Code already mapped in Appraisal Business Objective....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
            strqry = "Select * from ""@Z_HR_DEMA1"" where ""U_Z_BussCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Business Objective Code already mapped in Department Business Objective....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
            strqry = "Select * from ""@Z_HR_COUR1"" where ""U_Z_BussCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                Return True
            End If
        ElseIf aModule = "PEOBJCAT" Then
            strqry = "Select * from ""@Z_HR_OPEOB"" where ""U_Z_PeoCategory""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Personal Category Code Already mapped in Personel Objectives...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "COMPLEVEL" Then
            strqry = "Select * from ""@Z_HR_RMPREQ3"" where ""U_Z_CompLevel""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Competence Level Code Already mapped in Recruitment Requisition...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
            strqry = "Select * from ""@Z_HR_ECOLVL"" where ""U_Z_CompLevel""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Competence Level Code Already mapped in Employee Master...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If

            strqry = "Select * from ""@Z_HR_POSCO1"" where ""U_Z_CompLevel""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Competence Level Code Already mapped in Job Screen ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "PEOBJ" Then
            strqry = "Select * from ""@Z_HR_PEOBJ1"" where ""U_Z_HRPeoobjCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Personal Objective Code already mapped in Employee master Personal Objectives. ", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
            strqry = "Select * from ""@Z_HR_COUR2"" where ""U_Z_PeopleCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                Return True
            End If
            'ElseIf aModule = "INTERVIEWTYPE" Then
            '    strqry = "Select * from ""@Z_HR_OITYP"" where ""U_Z_TypeCode""='" & aCode & "'"
            '    oTemp.DoQuery(strqry)
            '    If oTemp.RecordCount > 0 Then
            '        Return True
            '    End If
        ElseIf aModule = "REJECTIONMASTER" Then
            strqry = "select * from ""@Z_HR_OCRAPP"" where ""U_Z_RejResn""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Rejection Code already mapped in Applicant profile....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "OREJECTIONMASTER" Then
            strqry = "select * from ""@Z_HR_OHEM3"" where ""U_Z_RejReason""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Offer Rejection Code already mapped in Employement offer details....", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "SEC" Then
            'strqry = "Select * from ""@Z_HR_OSEC"" where ""U_Z_SecCode""='" & aCode & "'"
            'oTemp.DoQuery(strqry)
            'If oTemp.RecordCount > 0 Then
            '    oApplication.Utilities.Message("Section Code Already Exits...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            '    Return True
            'End If
            strqry = "Select * from ""@Z_HR_ORGST"" where ""U_Z_SecCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Section Code Already mapped in Organizational Structure...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "RSTA" Then
            strqry = "Select * from ""@Z_HR_ORST"" where ""U_Z_StaCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                Return True
            End If
        ElseIf aModule = "COMPOBJ" Then
            strqry = "Select * from ""@Z_HR_COUR3"" where ""U_Z_CompCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Competence Code Already mapped in Course Master...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
            strqry = "Select * from ""@Z_HR_POSCO1"" where ""U_Z_CompCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Competence Code Already mapped in Job Screen...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        ElseIf aModule = "RATE" Then
            strqry = "Select * from ""@Z_HR_SEAPP1"" where ""U_Z_SelfRaCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Rating Code Already mapped in Self Appraisal Rating...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
            strqry = "Select * from ""@Z_HR_SEAPP2"" where ""U_Z_MgrRaCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Rating Code Already mapped in First Level Approval Rating...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
            strqry = "Select * from ""@Z_HR_SEAPP3"" where ""U_Z_SMRaCode""='" & aCode & "'"
            oTemp.DoQuery(strqry)
            If oTemp.RecordCount > 0 Then
                oApplication.Utilities.Message("Rating Code Already mapped in Second Level Approval Rating...", SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return True
            End If
        End If

        Return False
    End Function
#End Region
#Region "Set / Get Values from Matrix"
    Public Function getMatrixValues(ByVal aMatrix As SAPbouiCOM.Matrix, ByVal coluid As String, ByVal intRow As Integer) As String
        Return aMatrix.Columns.Item(coluid).Cells.Item(intRow).Specific.value
    End Function
    Public Sub SetMatrixValues(ByVal aMatrix As SAPbouiCOM.Matrix, ByVal coluid As String, ByVal intRow As Integer, ByVal strvalue As String)
        aMatrix.Columns.Item(coluid).Cells.Item(intRow).Specific.value = strvalue
    End Sub
#End Region

#Region "Get Edit Text"
    Public Function getEdittextvalue(ByVal aform As SAPbouiCOM.Form, ByVal UID As String) As String
        Dim objEdit As SAPbouiCOM.EditText
        objEdit = aform.Items.Item(UID).Specific
        Return objEdit.String
    End Function
    Public Sub setEdittextvalue(ByVal aform As SAPbouiCOM.Form, ByVal UID As String, ByVal newvalue As String)
        Dim objEdit As SAPbouiCOM.EditText
        objEdit = aform.Items.Item(UID).Specific
        objEdit.String = newvalue
    End Sub
#End Region

#End Region
    Public Sub addnewlogin(ByVal aEmpID As Integer)
        Try
            Dim oGeneralService As SAPbobsCOM.GeneralService
            Dim oGeneralData As SAPbobsCOM.GeneralData
            Dim oGeneralParams As SAPbobsCOM.GeneralDataParams
            Dim oCompanyService As SAPbobsCOM.CompanyService
            Dim oChildren As SAPbobsCOM.GeneralDataCollection
            oCompanyService = oApplication.Company.GetCompanyService()
            Dim oTestRs As SAPbobsCOM.Recordset
            Dim strCode As String
            oGeneralService = oCompanyService.GetGeneralService("Z_HR_LOGIN")
            oGeneralData = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData)
            oTestRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)
            strCode = getMaxCode("@Z_HR_LOGIN", "Code")
            oTestRs.DoQuery("SElect * from [@Z_HR_LOGIN] where U_Z_EmpID='" & aEmpID.ToString & "'")
            If oTestRs.RecordCount <= 0 Then
                oTestRs.DoQuery("Select empID,isnull(firstName,'') 'firstName' from OHEM where empid=" & aEmpID)
                oGeneralData.SetProperty("Code", strCode)
                oGeneralData.SetProperty("U_Z_EMPID", oTestRs.Fields.Item("empID").Value.ToString)
                oGeneralData.SetProperty("U_Z_UID", oTestRs.Fields.Item("firstName").Value.ToString)
                oGeneralData.SetProperty("U_Z_PWD", oTestRs.Fields.Item("firstName").Value.ToString)
                oGeneralData.SetProperty("U_Z_EMPNAME", oTestRs.Fields.Item("firstName").Value.ToString)
                oGeneralData.SetProperty("U_Z_APPROVER", "Y")
                oGeneralData.SetProperty("U_Z_SUPERUSER", "N")
                oGeneralData.SetProperty("U_Z_MGRAPPROVER", "N")
                oGeneralData.SetProperty("U_Z_HRAPPROVER", "N")
                oGeneralData.SetProperty("U_Z_MGRREQUEST", "N")
                oGeneralData.SetProperty("U_Z_HRRECAPPROVER", "N")
                oGeneralData.SetProperty("U_Z_GMRECAPPROVER", "N")
                oGeneralService.Add(oGeneralData)
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub
#Region "SetDatabind"
    Public Sub setUserDatabind(ByVal aForm As SAPbouiCOM.Form, ByVal UID As String, ByVal strDBID As String)
        Dim objEdit As SAPbouiCOM.EditText
        objEdit = aForm.Items.Item(UID).Specific
        objEdit.DataBind.SetBound(True, "", strDBID)
    End Sub
    Public Sub setUserDSCheckBox(ByVal aForm As SAPbouiCOM.Form, ByVal UID As String, ByVal strDBID As String)
        Dim objEdit As SAPbouiCOM.CheckBox
        objEdit = aForm.Items.Item(UID).Specific
        objEdit.DataBind.SetBound(True, "", strDBID)
    End Sub
    Public Sub setUserDSCombobox(ByVal aForm As SAPbouiCOM.Form, ByVal UID As String, ByVal strDBID As String)
        Dim objEdit As SAPbouiCOM.ComboBox
        objEdit = aForm.Items.Item(UID).Specific
        objEdit.DataBind.SetBound(True, "", strDBID)
    End Sub
#End Region

    Public Function GetCode(ByVal sTableName As String) As String
        Dim oRecSet As SAPbobsCOM.Recordset
        Dim sQuery As String
        oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        sQuery = "SELECT Top 1 DocEntry FROM " & sTableName + " ORDER BY Convert(Int,DocEntry) desc"
        oRecSet.DoQuery(sQuery)
        If Not oRecSet.EoF Then
            GetCode = Convert.ToInt32(oRecSet.Fields.Item(0).Value.ToString()) + 1
        Else
            GetCode = "1"
        End If
    End Function

#Region "Functions related to Load XML"

#Region "Add/Remove Menus "
    Public Sub AddRemoveMenus(ByVal sFileName As String)
        Dim oXMLDoc As New Xml.XmlDocument
        Dim sFilePath As String
        Try
            sFilePath = getApplicationPath() & "\XML Files\" & sFileName
            oXMLDoc.Load(sFilePath)
            oApplication.SBO_Application.LoadBatchActions(oXMLDoc.InnerXml)
        Catch ex As Exception
            Throw ex
        Finally
            oXMLDoc = Nothing
        End Try
    End Sub
#End Region

#Region "Load XML File "
    Private Function LoadXMLFiles(ByVal sFileName As String) As String
        Dim oXmlDoc As Xml.XmlDocument
        Dim oXNode As Xml.XmlNode
        Dim oAttr As Xml.XmlAttribute
        Dim sPath As String
        Dim FrmUID As String
        Try
            oXmlDoc = New Xml.XmlDocument

            sPath = getApplicationPath() & "\XML Files\" & sFileName

            oXmlDoc.Load(sPath)
            oXNode = oXmlDoc.GetElementsByTagName("form").Item(0)
            oAttr = oXNode.Attributes.GetNamedItem("uid")
            oAttr.Value = oAttr.Value & FormNum
            FormNum = FormNum + 1
            oApplication.SBO_Application.LoadBatchActions(oXmlDoc.InnerXml)
            FrmUID = oAttr.Value

            Return FrmUID

        Catch ex As Exception
            Throw ex
        Finally
            oXmlDoc = Nothing
        End Try
    End Function
#End Region

    Public Function LoadForm(ByVal XMLFile As String, ByVal FormType As String) As SAPbouiCOM.Form
        'Return LoadForm(XMLFile, FormType.ToString(), FormType & "_" & oApplication.SBO_Application.Forms.Count.ToString)
        LoadXMLFiles(XMLFile)
        Return Nothing
    End Function

    '*****************************************************************
    'Type               : Function   
    'Name               : LoadForm
    'Parameter          : XmlFile,FormType,FormUID
    'Return Value       : SBO Form
    'Author             : Senthil Kumar B Senthil Kumar B
    'Created Date       : 
    'Last Modified By   : 
    'Modified Date      : 
    'Purpose            : To Load XML file 
    '*****************************************************************

    Public Function LoadForm(ByVal XMLFile As String, ByVal FormType As String, ByVal FormUID As String) As SAPbouiCOM.Form

        Dim oXML As System.Xml.XmlDocument
        Dim objFormCreationParams As SAPbouiCOM.FormCreationParams
        Try
            oXML = New System.Xml.XmlDocument
            oXML.Load(XMLFile)
            objFormCreationParams = (oApplication.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams))
            objFormCreationParams.XmlData = oXML.InnerXml
            objFormCreationParams.FormType = FormType
            objFormCreationParams.UniqueID = FormUID
            Return oApplication.SBO_Application.Forms.AddEx(objFormCreationParams)
        Catch ex As Exception
            Throw ex

        End Try

    End Function



#Region "Load Forms"
    Public Sub LoadForm(ByRef oObject As Object, ByVal XmlFile As String)
        Try
            oObject.FrmUID = LoadXMLFiles(XmlFile)
            oObject.Form = oApplication.SBO_Application.Forms.Item(oObject.FrmUID)
            If Not oApplication.Collection.ContainsKey(oObject.FrmUID) Then
                oApplication.Collection.Add(oObject.FrmUID, oObject)
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
#End Region

#End Region

#Region "Functions related to System Initilization"

#Region "Create Tables"
    Public Sub CreateTables()
        Dim oCreateTable As clsTable
        Try
            oCreateTable = New clsTable
            oCreateTable.CreateTables()
        Catch ex As Exception
            Throw ex
        Finally
            oCreateTable = Nothing
        End Try
    End Sub
#End Region

#Region "Notify Alert"
    Public Sub NotifyAlert()
        'Dim oAlert As clsPromptAlert

        'Try
        '    oAlert = New clsPromptAlert
        '    oAlert.AlertforEndingOrdr()
        'Catch ex As Exception
        '    Throw ex
        'Finally
        '    oAlert = Nothing
        'End Try

    End Sub
#End Region

#End Region

#Region "Function related to Quantities"

#Region "Get Available Quantity"
    Public Function getAvailableQty(ByVal ItemCode As String) As Long
        Dim rsQuantity As SAPbobsCOM.Recordset

        strSQL = "Select SUM(T1.OnHand + T1.OnOrder - T1.IsCommited) From OITW T1 Left Outer Join OWHS T3 On T3.Whscode = T1.WhsCode " & _
                    "Where T1.ItemCode = '" & ItemCode & "'"
        Me.ExecuteSQL(rsQuantity, strSQL)

        If rsQuantity.Fields.Item(0) Is System.DBNull.Value Then
            Return 0
        Else
            Return CLng(rsQuantity.Fields.Item(0).Value)
        End If

    End Function
#End Region

#Region "Get Rented Quantity"
    Public Function getRentedQty(ByVal ItemCode As String, ByVal StartDate As String, ByVal EndDate As String) As Long
        Dim rsQuantity As SAPbobsCOM.Recordset
        Dim RentedQty As Long

        strSQL = " select Sum(U_ReqdQty) from [@REN_RDR1] Where U_ItemCode = '" & ItemCode & "' " & _
                    " And DocEntry IN " & _
                    " (Select DocEntry from [@REN_ORDR] Where U_Status = 'R') " & _
                    " and '" & StartDate & "' between [@REN_RDR1].U_ShipDt1 and [@REN_RDR1].U_ShipDt2 "
        '" and [@REN_RDR1].U_ShipDt1 between '" & StartDate & "' and '" & EndDate & "'"

        ExecuteSQL(rsQuantity, strSQL)
        If Not rsQuantity.Fields.Item(0).Value Is System.DBNull.Value Then
            RentedQty = rsQuantity.Fields.Item(0).Value
        End If

        Return RentedQty

    End Function
#End Region

#Region "Get Reserved Quantity"
    Public Function getReservedQty(ByVal ItemCode As String, ByVal StartDate As String, ByVal EndDate As String) As Long
        Dim rsQuantity As SAPbobsCOM.Recordset
        Dim ReservedQty As Long

        strSQL = " select Sum(U_ReqdQty) from [@REN_QUT1] Where U_ItemCode = '" & ItemCode & "' " & _
                    " And DocEntry IN " & _
                    " (Select DocEntry from [@REN_OQUT] Where U_Status = 'R' And Status = 'O') " & _
                    " and '" & StartDate & "' between [@REN_QUT1].U_ShipDt1 and [@REN_QUT1].U_ShipDt2"

        ExecuteSQL(rsQuantity, strSQL)
        If Not rsQuantity.Fields.Item(0).Value Is System.DBNull.Value Then
            ReservedQty = rsQuantity.Fields.Item(0).Value
        End If

        Return ReservedQty

    End Function
#End Region

#End Region

#Region "Functions related to Tax"

#Region "Get Tax Codes"
    Public Sub getTaxCodes(ByRef oCombo As SAPbouiCOM.ComboBox)
        Dim rsTaxCodes As SAPbobsCOM.Recordset

        strSQL = "Select Code, Name From OVTG Where Category = 'O' Order By Name"
        Me.ExecuteSQL(rsTaxCodes, strSQL)

        oCombo.ValidValues.Add("", "")
        If rsTaxCodes.RecordCount > 0 Then
            While Not rsTaxCodes.EoF
                oCombo.ValidValues.Add(rsTaxCodes.Fields.Item(0).Value, rsTaxCodes.Fields.Item(1).Value)
                rsTaxCodes.MoveNext()
            End While
        End If
        oCombo.ValidValues.Add("Define New", "Define New")
        'oCombo.Select("")
    End Sub
#End Region

#Region "Get Applicable Code"

    Public Function getApplicableTaxCode1(ByVal CardCode As String, ByVal ItemCode As String, ByVal Shipto As String) As String
        Dim oBP As SAPbobsCOM.BusinessPartners
        Dim oItem As SAPbobsCOM.Items
        Dim rsExempt As SAPbobsCOM.Recordset
        Dim TaxGroup As String
        oBP = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners)

        If oBP.GetByKey(CardCode.Trim) Then
            If oBP.VatLiable = SAPbobsCOM.BoVatStatus.vLiable Or oBP.VatLiable = SAPbobsCOM.BoVatStatus.vEC Then
                If oBP.VatGroup.Trim <> "" Then
                    TaxGroup = oBP.VatGroup.Trim
                Else
                    strSQL = "select LicTradNum from CRD1 where Address ='" & Shipto & "' and CardCode ='" & CardCode & "'"
                    Me.ExecuteSQL(rsExempt, strSQL)
                    If rsExempt.RecordCount > 0 Then
                        rsExempt.MoveFirst()
                        TaxGroup = rsExempt.Fields.Item(0).Value
                    Else
                        TaxGroup = ""
                    End If
                    'TaxGroup = oBP.FederalTaxID
                End If
            ElseIf oBP.VatLiable = SAPbobsCOM.BoVatStatus.vExempted Then
                strSQL = "Select Code From OVTG Where Rate = 0 And Category = 'O' Order By Code"
                Me.ExecuteSQL(rsExempt, strSQL)
                If rsExempt.RecordCount > 0 Then
                    rsExempt.MoveFirst()
                    TaxGroup = rsExempt.Fields.Item(0).Value
                Else
                    TaxGroup = ""
                End If
            End If
        End If




        Return TaxGroup

    End Function


    Public Function getApplicableTaxCode(ByVal CardCode As String, ByVal ItemCode As String) As String
        Dim oBP As SAPbobsCOM.BusinessPartners
        Dim oItem As SAPbobsCOM.Items
        Dim rsExempt As SAPbobsCOM.Recordset
        Dim TaxGroup As String
        oBP = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners)

        If oBP.GetByKey(CardCode.Trim) Then
            If oBP.VatLiable = SAPbobsCOM.BoVatStatus.vLiable Or oBP.VatLiable = SAPbobsCOM.BoVatStatus.vEC Then
                If oBP.VatGroup.Trim <> "" Then
                    TaxGroup = oBP.VatGroup.Trim
                Else
                    TaxGroup = oBP.FederalTaxID
                End If
            ElseIf oBP.VatLiable = SAPbobsCOM.BoVatStatus.vExempted Then
                strSQL = "Select Code From OVTG Where Rate = 0 And Category = 'O' Order By Code"
                Me.ExecuteSQL(rsExempt, strSQL)
                If rsExempt.RecordCount > 0 Then
                    rsExempt.MoveFirst()
                    TaxGroup = rsExempt.Fields.Item(0).Value
                Else
                    TaxGroup = ""
                End If
            End If
        End If

        'If oBP.GetByKey(CardCode.Trim) Then
        '    If oBP.VatLiable = SAPbobsCOM.BoVatStatus.vLiable Or oBP.VatLiable = SAPbobsCOM.BoVatStatus.vEC Then
        '        If oBP.VatGroup.Trim <> "" Then
        '            TaxGroup = oBP.VatGroup.Trim
        '        Else
        '            oItem = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)
        '            If oItem.GetByKey(ItemCode.Trim) Then
        '                TaxGroup = oItem.SalesVATGroup.Trim
        '            End If
        '        End If
        '    ElseIf oBP.VatLiable = SAPbobsCOM.BoVatStatus.vExempted Then
        '        strSQL = "Select Code From OVTG Where Rate = 0 And Category = 'O' Order By Code"
        '        Me.ExecuteSQL(rsExempt, strSQL)
        '        If rsExempt.RecordCount > 0 Then
        '            rsExempt.MoveFirst()
        '            TaxGroup = rsExempt.Fields.Item(0).Value
        '        Else
        '            TaxGroup = ""
        '        End If
        '    End If
        'End If
        Return TaxGroup

    End Function
#End Region

#End Region

#Region "Log Transaction"
    Public Sub LogTransaction(ByVal DocNum As Integer, ByVal ItemCode As String, _
                                    ByVal FromWhs As String, ByVal TransferedQty As Double, ByVal ProcessDate As Date)
        Dim sCode As String
        Dim sColumns As String
        Dim sValues As String
        Dim rsInsert As SAPbobsCOM.Recordset

        sCode = Me.getMaxCode("@REN_PORDR", "Code")

        sColumns = "Code, Name, U_DocNum, U_WhsCode, U_ItemCode, U_Quantity, U_RetQty, U_Date"
        sValues = "'" & sCode & "','" & sCode & "'," & DocNum & ",'" & FromWhs & "','" & ItemCode & "'," & TransferedQty & ", 0, Convert(DateTime,'" & ProcessDate.ToString("yyyyMMdd") & "')"

        strSQL = "Insert into [@REN_PORDR] (" & sColumns & ") Values (" & sValues & ")"
        oApplication.Utilities.ExecuteSQL(rsInsert, strSQL)

    End Sub

    Public Sub LogCreatedDocument(ByVal DocNum As Integer, ByVal CreatedDocType As SAPbouiCOM.BoLinkedObject, ByVal CreatedDocNum As String, ByVal sCreatedDate As String)
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim sCode As String
        Dim CreatedDate As DateTime
        Try
            oUserTable = oApplication.Company.UserTables.Item("REN_DORDR")

            sCode = Me.getMaxCode("@REN_DORDR", "Code")

            If Not oUserTable.GetByKey(sCode) Then
                oUserTable.Code = sCode
                oUserTable.Name = sCode

                With oUserTable.UserFields.Fields
                    .Item("U_DocNum").Value = DocNum
                    .Item("U_DocType").Value = CInt(CreatedDocType)
                    .Item("U_DocEntry").Value = CInt(CreatedDocNum)

                    If sCreatedDate <> "" Then
                        CreatedDate = CDate(sCreatedDate.Insert(4, "/").Insert(7, "/"))
                        .Item("U_Date").Value = CreatedDate
                    Else
                        .Item("U_Date").Value = CDate(Format(Now, "Long Date"))
                    End If

                End With

                If oUserTable.Add <> 0 Then
                    Throw New Exception(oApplication.Company.GetLastErrorDescription)
                End If
            End If

        Catch ex As Exception
            Throw ex
        Finally
            oUserTable = Nothing
        End Try
    End Sub
#End Region

    Public Function FormatDataSourceValue(ByVal Value As String) As Double
        Dim NewValue As Double

        If Value <> "" Then
            If Value.IndexOf(".") > -1 Then
                Value = Value.Replace(".", CompanyDecimalSeprator)
            End If

            If Value.IndexOf(CompanyThousandSeprator) > -1 Then
                Value = Value.Replace(CompanyThousandSeprator, "")
            End If
        Else
            Value = "0"

        End If

        ' NewValue = CDbl(Value)
        NewValue = Val(Value)

        Return NewValue


        'Dim dblValue As Double
        'Value = Value.Replace(CompanyThousandSeprator, "")
        'Value = Value.Replace(CompanyDecimalSeprator, System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
        'dblValue = Val(Value)
        'Return dblValue
    End Function

    Public Function FormatScreenValues(ByVal Value As String) As Double
        Dim NewValue As Double

        If Value <> "" Then
            If Value.IndexOf(".") > -1 Then
                Value = Value.Replace(".", CompanyDecimalSeprator)
            End If
        Else
            Value = "0"
        End If

        'NewValue = CDbl(Value)
        NewValue = Val(Value)

        Return NewValue

        'Dim dblValue As Double
        'Value = Value.Replace(CompanyThousandSeprator, "")
        'Value = Value.Replace(CompanyDecimalSeprator, System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
        'dblValue = Val(Value)
        'Return dblValue

    End Function

    Public Function SetScreenValues(ByVal Value As String) As String

        If Value.IndexOf(CompanyDecimalSeprator) > -1 Then
            Value = Value.Replace(CompanyDecimalSeprator, ".")
        End If

        Return Value

    End Function

    Public Function SetDBValues(ByVal Value As String) As String

        If Value.IndexOf(CompanyDecimalSeprator) > -1 Then
            Value = Value.Replace(CompanyDecimalSeprator, ".")
        End If

        Return Value

    End Function

    Public Function ReturnRefCode(ByVal strTracode As String, ByVal empid As Integer, Optional ByVal stcode As String = "") As String
        Dim oRecSet, otemp1 As SAPbobsCOM.Recordset
        Dim sQuery, strrefcode, strCode, strqry, strReCode As String

        Dim oUserTable, oUserTable1 As SAPbobsCOM.UserTable
        otemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oUserTable1 = oApplication.Company.UserTables.Item("Z_HR_ASSTP1")
        sQuery = "SELECT U_Z_RefCode FROM [@Z_HR_ASSTP1] where U_Z_RefCode='" & stcode & "' and U_Z_EmpId=" & empid & " and U_Z_TraCode='" & strTracode & "' "
        oRecSet.DoQuery(sQuery)
        If oRecSet.RecordCount > 0 Then
            strrefcode = oRecSet.Fields.Item(0).Value
            Return strrefcode
        Else
            strqry = "select T0.U_Z_ExpName,t0.U_Z_ActCode,t0.U_Z_Amount,T2.""CurrName""  from [@Z_HR_TRAPL1] T0 inner join [@Z_HR_OTRAPL] T1 "
            strqry = strqry & " on T0.DocEntry=t1.DocEntry left join OCRN T2 on T0.""U_Z_LocCurrency""=T2.""CurrCode"" where T1.U_Z_TraCode='" & strTracode & "'"
            otemp1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otemp1.DoQuery(strqry)
            strReCode = oApplication.Utilities.getMaxCode("@Z_HR_ASSTP1", "U_Z_RefCode")
            For intLoop As Integer = 0 To otemp1.RecordCount - 1
                strCode = oApplication.Utilities.getMaxCode("@Z_HR_ASSTP1", "Code")
                oUserTable1.Code = strCode
                oUserTable1.Name = strCode + "NX"
                oUserTable1.UserFields.Fields.Item("U_Z_EmpId").Value = empid
                oUserTable1.UserFields.Fields.Item("U_Z_TraCode").Value = strTracode
                oUserTable1.UserFields.Fields.Item("U_Z_ExpName").Value = otemp1.Fields.Item("U_Z_ExpName").Value
                oUserTable1.UserFields.Fields.Item("U_Z_ActCode").Value = otemp1.Fields.Item("U_Z_ActCode").Value
                oUserTable1.UserFields.Fields.Item("U_Z_Amount").Value = otemp1.Fields.Item("U_Z_Amount").Value
                oUserTable1.UserFields.Fields.Item("U_Z_LocCurrency").Value = otemp1.Fields.Item("CurrName").Value
                oUserTable1.UserFields.Fields.Item("U_Z_RefCode").Value = strReCode
                If oUserTable1.Add <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    Return False
                End If
                otemp1.MoveNext()
            Next
        End If
        'sQuery = "SELECT U_Z_RefCode FROM [@Z_HR_ASSTP1] where U_Z_EmpId=" & empid & " and U_Z_TraCode='" & strTracode & "' "
        'oRecSet.DoQuery(sQuery)
        'If oRecSet.RecordCount > 0 Then
        '    strrefcode = oRecSet.Fields.Item(0).Value
        '    Return strrefcode
        'End If
        Return stcode

    End Function

    Public Sub generateReport(ByVal oDtAppraisal As SAPbouiCOM.DataTable)
        'Dim dtHeader As DataTable
        'Dim dtBussiness As DataTable
        'Dim dtPeople As DataTable
        'Dim dtCompetency As DataTable
        'Dim dtFinalHR As DataTable
        Dim strEmpName As String
        Dim dr As DataRow
        For index As Integer = 0 To oDtAppraisal.Rows.Count - 1
            Dim oCrystalDocument As New CrystalDecisions.CrystalReports.Engine.ReportDocument
            Dim dsAp As New dsAppraisal
            oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            'Header
            sQuery = "Select T0.U_Z_EmpID,U_Z_EmpName,U_Z_Period,U_Z_LStrt,U_Z_Date,U_Z_BSelfRemark,U_Z_BMgrRemark,U_Z_BSMrRemark,U_Z_BHrRemark,T2.Descriptio,T3.Remarks,DocEntry  From [@Z_HR_OSEAPP] T0 JOIN OHEM T1 ON T0.U_Z_EmpId = T1.EmpID JOIN OHPS T2 On T2.PosID = T1.Position JOIN OUDP T3 On T3.Code = T1.Dept Where DocEntry = '" & oDtAppraisal.GetValue("DocEntry", index) & "'"
            oRecordSet.DoQuery(sQuery)
            If Not oRecordSet.EoF Then
                dr = dsAp.Tables("Header").NewRow()
                strEmpName = oRecordSet.Fields.Item("U_Z_EmpID").Value & "_" & oRecordSet.Fields.Item("U_Z_EmpName").Value
                dr("DocEntry") = oRecordSet.Fields.Item("DocEntry").Value
                dr("EmpID") = oRecordSet.Fields.Item("U_Z_EmpID").Value
                dr("EmpName") = oRecordSet.Fields.Item("U_Z_EmpName").Value
                dr("Period") = oRecordSet.Fields.Item("U_Z_Period").Value
                dr("AppraisalStarts") = oRecordSet.Fields.Item("U_Z_LStrt").Value
                dr("ReportType") = "Appraisal Document"
                dr("Date") = oRecordSet.Fields.Item("U_Z_Date").Value
                dr("SEComments") = oRecordSet.Fields.Item("U_Z_BSelfRemark").Value
                dr("LMComments") = oRecordSet.Fields.Item("U_Z_BMgrRemark").Value
                dr("SMComments") = oRecordSet.Fields.Item("U_Z_BSMrRemark").Value
                dr("HRComments") = oRecordSet.Fields.Item("U_Z_BHrRemark").Value
                dr("Position") = oRecordSet.Fields.Item("Descriptio").Value
                dr("Department") = oRecordSet.Fields.Item("Remarks").Value
                dsAp.Tables("Header").Rows.Add(dr)
            End If

            'Bussiness Objectives
            sQuery = "Select U_Z_BussCode,U_Z_BussDesc,U_Z_BussWeight,U_Z_BussSelfRate,U_Z_BussMgrRate,U_Z_BussSMRate,U_Z_BussSMRate As U_Z_BussHRRate,DocEntry From [@Z_HR_SEAPP1] Where DocEntry = '" & oDtAppraisal.GetValue("DocEntry", index) & "'"
            oRecordSet.DoQuery(sQuery)
            For index1 As Integer = 0 To oRecordSet.RecordCount - 1
                If Not oRecordSet.EoF Then
                    dr = dsAp.Tables("Bussiness").NewRow()
                    dr("DocEntry") = oRecordSet.Fields.Item("DocEntry").Value
                    dr("BussCode") = oRecordSet.Fields.Item("U_Z_BussCode").Value
                    dr("BussName") = oRecordSet.Fields.Item("U_Z_BussDesc").Value
                    dr("BussWeight") = oRecordSet.Fields.Item("U_Z_BussWeight").Value
                    dr("BussSR") = oRecordSet.Fields.Item("U_Z_BussSelfRate").Value
                    dr("BussLM") = oRecordSet.Fields.Item("U_Z_BussMgrRate").Value
                    dr("BussSM") = oRecordSet.Fields.Item("U_Z_BussSMRate").Value
                    dr("BussHR") = oRecordSet.Fields.Item("U_Z_BussHRRate").Value
                    dsAp.Tables("Bussiness").Rows.Add(dr)
                    oRecordSet.MoveNext()
                End If
            Next

            'People Objectives
            sQuery = "Select U_Z_PeopleCode,U_Z_PeopleDesc,U_Z_PeopleCat,U_Z_PeoWeight,U_Z_PeoSelfRate,U_Z_PeoMgrRate,U_Z_PeoSMRate As U_Z_PeoHrRate,U_Z_PeoSMRate,DocEntry From [@Z_HR_SEAPP2] Where DocEntry = '" & oDtAppraisal.GetValue("DocEntry", index) & "'"
            oRecordSet.DoQuery(sQuery)
            For index1 As Integer = 0 To oRecordSet.RecordCount - 1
                If Not oRecordSet.EoF Then
                    dr = dsAp.Tables("People").NewRow()
                    dr("DocEntry") = oRecordSet.Fields.Item("DocEntry").Value
                    dr("PeopleCode") = oRecordSet.Fields.Item("U_Z_PeopleCode").Value
                    dr("PeopleName") = oRecordSet.Fields.Item("U_Z_PeopleDesc").Value
                    dr("PeopleCat") = oRecordSet.Fields.Item("U_Z_PeopleCat").Value
                    dr("PeopleWeight") = oRecordSet.Fields.Item("U_Z_PeoWeight").Value
                    dr("PeopleSR") = oRecordSet.Fields.Item("U_Z_PeoSelfRate").Value
                    dr("PeopleLM") = oRecordSet.Fields.Item("U_Z_PeoMgrRate").Value
                    dr("PeopleSM") = oRecordSet.Fields.Item("U_Z_PeoSMRate").Value
                    dr("PeopleHR") = oRecordSet.Fields.Item("U_Z_PeoHrRate").Value
                    dsAp.Tables("People").Rows.Add(dr)
                    oRecordSet.MoveNext()
                End If
            Next

            'Competency Objectives
            sQuery = "Select U_Z_CompCode,U_Z_CompDesc,U_Z_CompWeight,U_Z_CompLevel,U_Z_CompSelfRate,U_Z_CompMgrRate,U_Z_CompSMRate As U_Z_CompHrRate,U_Z_CompSMRate,DocEntry From [@Z_HR_SEAPP3] Where DocEntry = '" & oDtAppraisal.GetValue("DocEntry", index) & "'"
            oRecordSet.DoQuery(sQuery)
            For index1 As Integer = 0 To oRecordSet.RecordCount - 1
                If Not oRecordSet.EoF Then
                    dr = dsAp.Tables("Competency").NewRow()
                    dr("DocEntry") = oRecordSet.Fields.Item("DocEntry").Value
                    dr("CompCode") = oRecordSet.Fields.Item("U_Z_CompCode").Value
                    dr("CompName") = oRecordSet.Fields.Item("U_Z_CompDesc").Value
                    dr("CompWeight") = oRecordSet.Fields.Item("U_Z_CompWeight").Value
                    dr("CompLevel") = oRecordSet.Fields.Item("U_Z_CompLevel").Value
                    dr("CompSR") = oRecordSet.Fields.Item("U_Z_CompSelfRate").Value
                    dr("CompLM") = oRecordSet.Fields.Item("U_Z_CompMgrRate").Value
                    dr("CompSM") = oRecordSet.Fields.Item("U_Z_CompSMRate").Value
                    dr("CompHR") = oRecordSet.Fields.Item("U_Z_CompHrRate").Value
                    dsAp.Tables("Competency").Rows.Add(dr)
                    oRecordSet.MoveNext()
                End If
            Next

            'HR Final Rating
            sQuery = "Select U_Z_CompType,U_Z_AvgComp,U_Z_HRComp,DocEntry From [@Z_HR_SEAPP4] Where DocEntry = '" & oDtAppraisal.GetValue("DocEntry", index) & "'"
            oRecordSet.DoQuery(sQuery)
            For index1 As Integer = 0 To oRecordSet.RecordCount - 1
                If Not oRecordSet.EoF Then
                    dr = dsAp.Tables("HRFinal").NewRow()
                    dr("DocEntry") = oRecordSet.Fields.Item("DocEntry").Value
                    dr("Type") = oRecordSet.Fields.Item("U_Z_CompType").Value
                    dr("AvgComp") = oRecordSet.Fields.Item("U_Z_AvgComp").Value
                    dr("HRComp") = oRecordSet.Fields.Item("U_Z_HRComp").Value
                    dsAp.Tables("HRFinal").Rows.Add(dr)
                    oRecordSet.MoveNext()
                End If
            Next

            Dim strFilename As String = System.Windows.Forms.Application.StartupPath & "\ApprisalPDFs\" & strEmpName & ".pdf"
            ' Dim strReportFileName As String = System.Windows.Forms.Application.StartupPath & "\Reports\" & "rptAppraisal1.rpt"
            Dim strReportFileName As String = System.Windows.Forms.Application.StartupPath & "\Reports\" & "rptApp.rpt"
            oCrystalDocument.Load(strReportFileName)
            oCrystalDocument.SetDataSource(dsAp)

            If File.Exists(strFilename) Then
                File.Delete(strFilename)
            End If

            Dim CrExportOptions As CrystalDecisions.Shared.ExportOptions
            Dim CrDiskFileDestinationOptions As New _
            CrystalDecisions.Shared.DiskFileDestinationOptions()
            Dim CrFormatTypeOptions As New CrystalDecisions.Shared.PdfRtfWordFormatOptions()
            CrDiskFileDestinationOptions.DiskFileName = strFilename
            CrExportOptions = oCrystalDocument.ExportOptions
            With CrExportOptions
                .ExportDestinationType = CrystalDecisions.Shared.ExportDestinationType.DiskFile
                .ExportFormatType = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat
                .DestinationOptions = CrDiskFileDestinationOptions
                .FormatOptions = CrFormatTypeOptions
            End With
            oCrystalDocument.Export()
            oCrystalDocument.Close()

            'Dim x As System.Diagnostics.ProcessStartInfo
            'x = New System.Diagnostics.ProcessStartInfo
            'x.UseShellExecute = True
            'x.FileName = strFilename
            'System.Diagnostics.Process.Start(x)
            'x = Nothing

            oDtAppraisal.SetValue("Path", index, strFilename)
        Next
    End Sub

    Public Sub PrintReport(ByVal strDocEntry As String)
        Dim strEmpName As String
        Dim dr As DataRow
        Dim oCrystalDocument As New CrystalDecisions.CrystalReports.Engine.ReportDocument
        Dim dsAp As New dsAppraisal
        oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

        'Header
        sQuery = "Select T0.U_Z_EmpID,U_Z_EmpName,T0.U_Z_Period,U_Z_LStrt,U_Z_Date,U_Z_BSelfRemark,U_Z_BMgrRemark,U_Z_BSMrRemark,U_Z_BHrRemark,T2.Descriptio,T3.Remarks,DocEntry  From [@Z_HR_OSEAPP] T0 JOIN OHEM T1 ON T0.U_Z_EmpId = T1.EmpID JOIN OHPS T2 On T2.PosID = T1.Position JOIN OUDP T3 On T3.Code = T1.Dept Where T0.DocEntry = '" & strDocEntry & "'"
        oRecordSet.DoQuery(sQuery)
        If Not oRecordSet.EoF Then
            dr = dsAp.Tables("Header").NewRow()
            strEmpName = oRecordSet.Fields.Item("U_Z_EmpID").Value & "_" & oRecordSet.Fields.Item("U_Z_EmpName").Value
            dr("DocEntry") = oRecordSet.Fields.Item("DocEntry").Value
            dr("EmpID") = oRecordSet.Fields.Item("U_Z_EmpID").Value
            dr("EmpName") = oRecordSet.Fields.Item("U_Z_EmpName").Value
            dr("Period") = oRecordSet.Fields.Item("U_Z_Period").Value
            dr("AppraisalStarts") = oRecordSet.Fields.Item("U_Z_LStrt").Value
            dr("ReportType") = "Appraisal Document"
            dr("Date") = oRecordSet.Fields.Item("U_Z_Date").Value
            dr("SEComments") = oRecordSet.Fields.Item("U_Z_BSelfRemark").Value
            dr("LMComments") = oRecordSet.Fields.Item("U_Z_BMgrRemark").Value
            dr("SMComments") = oRecordSet.Fields.Item("U_Z_BSMrRemark").Value
            dr("HRComments") = oRecordSet.Fields.Item("U_Z_BHrRemark").Value
            dr("Position") = oRecordSet.Fields.Item("Descriptio").Value
            dr("Department") = oRecordSet.Fields.Item("Remarks").Value
            dsAp.Tables("Header").Rows.Add(dr)
        End If

        'Bussiness Objectives
        sQuery = "Select U_Z_BussCode,U_Z_BussDesc,U_Z_BussWeight,U_Z_BussSelfRate,U_Z_BussMgrRate,U_Z_BussSMRate,U_Z_BussSMRate As U_Z_BussHRRate,DocEntry From [@Z_HR_SEAPP1] Where DocEntry = '" & strDocEntry & "'"
        oRecordSet.DoQuery(sQuery)
        For index1 As Integer = 0 To oRecordSet.RecordCount - 1
            If Not oRecordSet.EoF Then
                dr = dsAp.Tables("Bussiness").NewRow()
                dr("DocEntry") = oRecordSet.Fields.Item("DocEntry").Value
                dr("BussCode") = oRecordSet.Fields.Item("U_Z_BussCode").Value
                dr("BussName") = oRecordSet.Fields.Item("U_Z_BussDesc").Value
                dr("BussWeight") = oRecordSet.Fields.Item("U_Z_BussWeight").Value
                dr("BussSR") = oRecordSet.Fields.Item("U_Z_BussSelfRate").Value
                dr("BussLM") = oRecordSet.Fields.Item("U_Z_BussMgrRate").Value
                dr("BussSM") = oRecordSet.Fields.Item("U_Z_BussSMRate").Value
                dr("BussHR") = oRecordSet.Fields.Item("U_Z_BussHRRate").Value
                dsAp.Tables("Bussiness").Rows.Add(dr)
                oRecordSet.MoveNext()
            End If
        Next

        'People Objectives
        sQuery = "Select U_Z_PeopleCode,U_Z_PeopleDesc,U_Z_PeopleCat,U_Z_PeoWeight,U_Z_PeoSelfRate,U_Z_PeoMgrRate,U_Z_PeoSMRate As U_Z_PeoHrRate,U_Z_PeoSMRate,DocEntry From [@Z_HR_SEAPP2] Where DocEntry = '" & strDocEntry & "'"
        oRecordSet.DoQuery(sQuery)
        For index1 As Integer = 0 To oRecordSet.RecordCount - 1
            If Not oRecordSet.EoF Then
                dr = dsAp.Tables("People").NewRow()
                dr("DocEntry") = oRecordSet.Fields.Item("DocEntry").Value
                dr("PeopleCode") = oRecordSet.Fields.Item("U_Z_PeopleCode").Value
                dr("PeopleName") = oRecordSet.Fields.Item("U_Z_PeopleDesc").Value
                dr("PeopleCat") = oRecordSet.Fields.Item("U_Z_PeopleCat").Value
                dr("PeopleWeight") = oRecordSet.Fields.Item("U_Z_PeoWeight").Value
                dr("PeopleSR") = oRecordSet.Fields.Item("U_Z_PeoSelfRate").Value
                dr("PeopleLM") = oRecordSet.Fields.Item("U_Z_PeoMgrRate").Value
                dr("PeopleSM") = oRecordSet.Fields.Item("U_Z_PeoSMRate").Value
                dr("PeopleHR") = oRecordSet.Fields.Item("U_Z_PeoHrRate").Value
                dsAp.Tables("People").Rows.Add(dr)
                oRecordSet.MoveNext()
            End If
        Next

        'Competency Objectives
        sQuery = "Select U_Z_CompCode,U_Z_CompDesc,U_Z_CompWeight,U_Z_CompLevel,U_Z_CompSelfRate,U_Z_CompMgrRate,U_Z_CompSMRate As U_Z_CompHrRate,U_Z_CompSMRate,DocEntry From [@Z_HR_SEAPP3] Where DocEntry = '" & strDocEntry & "'"
        oRecordSet.DoQuery(sQuery)
        For index1 As Integer = 0 To oRecordSet.RecordCount - 1
            If Not oRecordSet.EoF Then
                dr = dsAp.Tables("Competency").NewRow()
                dr("DocEntry") = oRecordSet.Fields.Item("DocEntry").Value
                dr("CompCode") = oRecordSet.Fields.Item("U_Z_CompCode").Value
                dr("CompName") = oRecordSet.Fields.Item("U_Z_CompDesc").Value
                dr("CompWeight") = oRecordSet.Fields.Item("U_Z_CompWeight").Value
                dr("CompLevel") = oRecordSet.Fields.Item("U_Z_CompLevel").Value
                dr("CompSR") = oRecordSet.Fields.Item("U_Z_CompSelfRate").Value
                dr("CompLM") = oRecordSet.Fields.Item("U_Z_CompMgrRate").Value
                dr("CompSM") = oRecordSet.Fields.Item("U_Z_CompSMRate").Value
                dr("CompHR") = oRecordSet.Fields.Item("U_Z_CompHrRate").Value
                dsAp.Tables("Competency").Rows.Add(dr)
                oRecordSet.MoveNext()
            End If
        Next

        'HR Final Rating
        sQuery = "Select U_Z_CompType,U_Z_AvgComp,U_Z_HRComp,DocEntry From [@Z_HR_SEAPP4] Where DocEntry = '" & strDocEntry & "'"
        oRecordSet.DoQuery(sQuery)
        For index1 As Integer = 0 To oRecordSet.RecordCount - 1
            If Not oRecordSet.EoF Then
                dr = dsAp.Tables("HRFinal").NewRow()
                dr("DocEntry") = oRecordSet.Fields.Item("DocEntry").Value
                dr("Type") = oRecordSet.Fields.Item("U_Z_CompType").Value
                dr("AvgComp") = oRecordSet.Fields.Item("U_Z_AvgComp").Value
                dr("HRComp") = oRecordSet.Fields.Item("U_Z_HRComp").Value
                dsAp.Tables("HRFinal").Rows.Add(dr)
                oRecordSet.MoveNext()
            End If
        Next

        Dim strFilename As String = System.Windows.Forms.Application.StartupPath & "\ApprisalPDFs\" & strEmpName & ".pdf"
        ' Dim strReportFileName As String = System.Windows.Forms.Application.StartupPath & "\Reports\" & "rptAppraisal1.rpt"
        Dim strReportFileName As String = System.Windows.Forms.Application.StartupPath & "\Reports\" & "rptApp.rpt"
        oCrystalDocument.Load(strReportFileName)
        oCrystalDocument.SetDataSource(dsAp)

        If File.Exists(strFilename) Then
            File.Delete(strFilename)
        End If

        Dim CrExportOptions As CrystalDecisions.Shared.ExportOptions
        Dim CrDiskFileDestinationOptions As New _
        CrystalDecisions.Shared.DiskFileDestinationOptions()
        Dim CrFormatTypeOptions As New CrystalDecisions.Shared.PdfRtfWordFormatOptions()
        CrDiskFileDestinationOptions.DiskFileName = strFilename
        CrExportOptions = oCrystalDocument.ExportOptions
        With CrExportOptions
            .ExportDestinationType = CrystalDecisions.Shared.ExportDestinationType.DiskFile
            .ExportFormatType = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat
            .DestinationOptions = CrDiskFileDestinationOptions
            .FormatOptions = CrFormatTypeOptions
        End With
        oCrystalDocument.Export()
        oCrystalDocument.Close()

        Dim x As System.Diagnostics.ProcessStartInfo
        x = New System.Diagnostics.ProcessStartInfo
        x.UseShellExecute = True
        x.FileName = strFilename
        System.Diagnostics.Process.Start(x)
        x = Nothing
    End Sub

    Public Function checkmailconfiguration() As Boolean
        oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRecordSet.DoQuery("Select U_Z_SMTPSERV,U_Z_SMTPPORT,U_Z_SMTPUSER,U_Z_SMTPPWD,U_Z_SSL From [@Z_HR_OMAIL]")
        If oRecordSet.RecordCount <= 0 Then
            Return False
        Else
            Return True
        End If
    End Function

    Public Sub SendMail(ByVal dtAppraisal As SAPbouiCOM.DataTable, ByVal strType As String)
        oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRecordSet.DoQuery("Select U_Z_SMTPSERV,U_Z_SMTPPORT,U_Z_SMTPUSER,U_Z_SMTPPWD,U_Z_SSL From [@Z_HR_OMAIL]")
        If Not oRecordSet.EoF Then
            mailServer = oRecordSet.Fields.Item("U_Z_SMTPSERV").Value
            mailPort = oRecordSet.Fields.Item("U_Z_SMTPPORT").Value
            mailId = oRecordSet.Fields.Item("U_Z_SMTPUSER").Value
            mailPwd = oRecordSet.Fields.Item("U_Z_SMTPPWD").Value
            mailSSL = oRecordSet.Fields.Item("U_Z_SSL").Value
            If mailServer <> "" And mailId <> "" And mailPwd <> "" Then
                If strType = "Appraisal" Then
                    For index As Integer = 0 To dtAppraisal.Rows.Count - 1
                        toID = dtAppraisal.GetValue("toID", index)
                        ccID = dtAppraisal.GetValue("ccID", index)
                        mType = dtAppraisal.GetValue("Type", index)
                        path = dtAppraisal.GetValue("Path", index)

                        If toID.Length > 0 And ccID.Length = 0 Then
                            ccID = toID
                            SendMailforUsers(mailServer, mailPort, mailId, mailPwd, mailSSL, toID, ccID, mType, path, dtAppraisal.GetValue("DocEntry", index), dtAppraisal.GetValue("Name", index))
                        ElseIf toID.Length = 0 And ccID.Length > 0 Then
                            toID = ccID
                            SendMailforUsers(mailServer, mailPort, mailId, mailPwd, mailSSL, toID, ccID, mType, path, dtAppraisal.GetValue("DocEntry", index), dtAppraisal.GetValue("Name", index))
                        Else
                            SendMailforUsers(mailServer, mailPort, mailId, mailPwd, mailSSL, toID, ccID, mType, path, dtAppraisal.GetValue("DocEntry", index), dtAppraisal.GetValue("Name", index))
                        End If
                    Next
                ElseIf (strType = "Agenda") Then
                    For index As Integer = 0 To dtAppraisal.Rows.Count - 1
                        toID = dtAppraisal.GetValue("toID", index)
                        ccID = dtAppraisal.GetValue("ccID", index)
                        mType = dtAppraisal.GetValue("Type", index)
                        'path = dtAppraisal.GetValue("Path", index)
                        'SendMailforUsers(mailServer, mailPort, mailId, mailPwd, mailSSL, toID, ccID, mType, path, dtAppraisal.GetValue("DocEntry", index), dtAppraisal.GetValue("Name", index))
                        If toID.Length > 0 And ccID.Length = 0 Then
                            ccID = toID
                            SendMailforUsers(mailServer, mailPort, mailId, mailPwd, mailSSL, toID, ccID, mType, path, dtAppraisal.GetValue("DocEntry", index), dtAppraisal.GetValue("Name", index))
                        ElseIf toID.Length = 0 And ccID.Length > 0 Then
                            toID = ccID
                            SendMailforUsers(mailServer, mailPort, mailId, mailPwd, mailSSL, toID, ccID, mType, path, dtAppraisal.GetValue("DocEntry", index), dtAppraisal.GetValue("Name", index))
                        End If
                    Next
                End If
            Else
                oApplication.Utilities.Message("Mail Server Details Not Configured...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            End If

        End If
    End Sub

    Private Sub SendMailforUsers(ByVal mailServer As String, ByVal mailPort As String, ByVal mailId As String, ByVal mailpwd As String, ByVal mailSSL As String, ByVal toId As String, ByVal ccId As String, ByVal mType As String, ByVal path As String, ByVal DocEntry As String, ByVal Name As String)
        Try
            'Dim strRptPath As String = System.Windows.Forms.Application.StartupPath.Trim() & "\Report.pdf"
            SmtpServer.Credentials = New Net.NetworkCredential(mailId, mailpwd)
            SmtpServer.Port = mailPort
            SmtpServer.EnableSsl = mailSSL
            SmtpServer.Host = mailServer
            mail = New Net.Mail.MailMessage()
            mail.From = New Net.Mail.MailAddress(mailId, "HRMS")
            mail.To.Add(toId)
            mail.CC.Add(ccId)
            mail.IsBodyHtml = True
            mail.Priority = MailPriority.High
            If mType = "AI" Then
                mail.Subject = "Appraisal Initialized " & " - " & System.DateTime.Now.ToShortDateString()
                mail.Body = BuildHtmBody(DocEntry, Name, "Appraisal", mType)
                mail.Attachments.Add(New Net.Mail.Attachment(path))
            ElseIf mType = "LA" Then
                mail.Subject = "Line Manager Approved " & " - " & System.DateTime.Now.ToShortDateString()
                mail.Body = BuildHtmBody(DocEntry, Name, "Appraisal", mType)
                mail.Attachments.Add(New Net.Mail.Attachment(path))
            ElseIf mType = "HA" Then
                mail.Subject = "HR Approved " & " - " & System.DateTime.Now.ToShortDateString()
                mail.Body = BuildHtmBody(DocEntry, Name, "Appraisal", mType)
                mail.Attachments.Add(New Net.Mail.Attachment(path))
            ElseIf mType = "AG" Then
                mail.Subject = "Training Agenda " & " - " & System.DateTime.Now.ToShortDateString()
                mail.Body = BuildHtmBody(DocEntry, Name, "Agenda", mType)
            End If

            SmtpServer.Send(mail)

        Catch ex As Exception

        Finally
            mail.Dispose()
        End Try
    End Sub

    Private Function BuildHtmBody(ByVal DocEntry As String, ByVal Name As String, ByVal type As String, ByVal mtype As String)
        Dim oHTML As String
        Dim strCompany As String
        Dim strName As String
        Dim Address1, Address2, Mail As String
        Dim CourseCode, CourseName, StDate, EdDate, StTime, EndTime, TotalHours, Instrutor, AppED As String


        If type = "Appraisal" Then
            oHTML = GetFileContents("Appraisal.htm")
        ElseIf type = "Agenda" Then
            oHTML = GetFileContents("Agenda.htm")
        End If

        sQuery = " Select CompnyName,CompnyAddr,Country,PrintHeadr,Phone1,E_Mail From OADM"
        oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRecordSet.DoQuery(sQuery)

        If Not oRecordSet.EoF Then
            strCompany = oRecordSet.Fields.Item("CompnyName").Value
            strName = oRecordSet.Fields.Item("PrintHeadr").Value
            Address1 = oRecordSet.Fields.Item("CompnyAddr").Value
            Address2 = oRecordSet.Fields.Item("Country").Value
            Mail = oRecordSet.Fields.Item("E_Mail").Value
        End If


        If type = "Agenda" Then
            sQuery = "Select U_Z_CourseCode,U_Z_CourseName,U_Z_Startdt,U_Z_EndDt,U_Z_AppStDt,U_Z_AppEndDt,U_Z_InsName,Convert(VarChar(5),Convert(DateTime ,Convert(Date,GetDate()))+ Convert(VarChar(5),SUBSTRING(RIGHT('0000' + CAST(U_Z_StartTime AS NVARCHAR),4),1,2) + ':' + SUBSTRING(RIGHT('0000' + CAST(U_Z_StartTime AS NVARCHAR),4),3,2)) ,108) as U_Z_StartTime,Convert(VarChar(5),Convert(DateTime ,Convert(Date,GetDate()))+ Convert(VarChar(5),SUBSTRING(RIGHT('0000' + CAST(U_Z_EndTime AS NVARCHAR),4),1,2) + ':' + SUBSTRING(RIGHT('0000' + CAST(U_Z_EndTime AS NVARCHAR),4),3,2)) ,108)As U_Z_EndTime,U_Z_Sunday,U_Z_Monday,U_Z_Tuesday,U_Z_Wednesday,U_Z_Thursday,U_Z_Friday,U_Z_Saturday,U_Z_NoOfHours From [@Z_HR_OTRIN] Where U_Z_CourseCode ='" & DocEntry & "'"
            oRecordSet.DoQuery(sQuery)
            If Not oRecordSet.EoF Then
                CourseCode = oRecordSet.Fields.Item("U_Z_CourseCode").Value
                CourseName = oRecordSet.Fields.Item("U_Z_CourseName").Value
                StDate = oRecordSet.Fields.Item("U_Z_Startdt").Value
                EdDate = oRecordSet.Fields.Item("U_Z_EndDt").Value
                StTime = oRecordSet.Fields.Item("U_Z_StartTime").Value
                EndTime = oRecordSet.Fields.Item("U_Z_EndTime").Value
                TotalHours = oRecordSet.Fields.Item("U_Z_NoOfHours").Value
                Instrutor = oRecordSet.Fields.Item("U_Z_InsName").Value
                AppED = oRecordSet.Fields.Item("U_Z_AppEndDt").Value
            End If
        End If

        If Not IsDBNull(Name) Then
            oHTML = oHTML.Replace("$$EmpName$$", Name)
        Else
            oHTML = oHTML.Replace("$$EmpName$$", "")
        End If

        If Not IsDBNull(strName) Then
            oHTML = oHTML.Replace("$$Company$$", strName)
        Else
            oHTML = oHTML.Replace("$$Company$$", "")
        End If

        If Not IsDBNull(Address1) Then
            oHTML = oHTML.Replace("$$Address1$$", Address1)
        Else
            oHTML = oHTML.Replace("$$Address1$$", "")
        End If

        If Not IsDBNull(Address2) Then
            oHTML = oHTML.Replace("$$Address2$$", Address2)
        Else
            oHTML = oHTML.Replace("$$Address2$$", "")
        End If

        If Not IsDBNull(Mail) Then
            oHTML = oHTML.Replace("$$Mail$$", Mail)
        Else
            oHTML = oHTML.Replace("$$Mail$$", "")
        End If


        If mtype = "AI" Then
            oHTML = oHTML.Replace("$$Comments$$", "Appraisal Process Initiated...")
        ElseIf mtype = "LA" Then
            oHTML = oHTML.Replace("$$Comments$$", "Appraisal Document Approved by Line Manager...")
        ElseIf mtype = "HA" Then
            oHTML = oHTML.Replace("$$Comments$$", "Appraisal Document Approved by HR...")
        End If


        If Not IsDBNull(CourseCode) Then
            oHTML = oHTML.Replace("$$Course$$", CourseCode)
        Else
            oHTML = oHTML.Replace("$$Course$$", "")
        End If

        If Not IsDBNull(CourseName) Then
            oHTML = oHTML.Replace("$$Name$$", CourseName)
        Else
            oHTML = oHTML.Replace("$$Name$$", "")
        End If

        If Not IsDBNull(StDate) Then
            oHTML = oHTML.Replace("$$SD$$", StDate)
        Else
            oHTML = oHTML.Replace("$$SD$$", "")
        End If

        If Not IsDBNull(EdDate) Then
            oHTML = oHTML.Replace("$$ED$$", EdDate)
        Else
            oHTML = oHTML.Replace("$$ED$$", "")
        End If

        If Not IsDBNull(StTime) Then
            oHTML = oHTML.Replace("$$ST$$", StTime)
        Else
            oHTML = oHTML.Replace("$$ST$$", "")
        End If

        If Not IsDBNull(EndTime) Then
            oHTML = oHTML.Replace("$$ET$$", EndTime)
        Else
            oHTML = oHTML.Replace("$$ET$$", "")
        End If


        If Not IsDBNull(TotalHours) Then
            oHTML = oHTML.Replace("$$TH$$", TotalHours)
        Else
            oHTML = oHTML.Replace("$$TH$$", "")
        End If

        If Not IsDBNull(Instrutor) Then
            oHTML = oHTML.Replace("$$Instrutor$$", Instrutor)
        Else
            oHTML = oHTML.Replace("$$Instrutor$$", "")
        End If

        If Not IsDBNull(AppED) Then
            oHTML = oHTML.Replace("$$AED$$", AppED)
        Else
            oHTML = oHTML.Replace("$$AED$$", "")
        End If

        Return oHTML
    End Function

    Public Function GetFileContents(ByVal FullPath As String, _
       Optional ByRef ErrInfo As String = "") As String

        Dim strContents As String
        Dim objReader As StreamReader
        Try
            objReader = New StreamReader(FullPath)
            strContents = objReader.ReadToEnd()
            objReader.Close()
            Return strContents
        Catch Ex As Exception
            ErrInfo = Ex.Message
        End Try
    End Function

    Public Sub UpdateUsing_DIAPI(ByVal strObject As String, ByVal strDocEntry As String, ByVal oHT As Hashtable)
        oCompService = oApplication.Company.GetCompanyService
        oGenService = oCompService.GetGeneralService(strObject)
        oGenData = oGenService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData)
        oGeneralDataParams = oGenService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)
        oGeneralDataParams.SetProperty("DocEntry", Convert.ToInt32(strDocEntry))
        oGenData = oGenService.GetByParams(oGeneralDataParams)
        For Each item As DictionaryEntry In oHT
            oGenData.SetProperty(item.Key, item.Value)
        Next
        oGenService.Update(oGenData)
    End Sub

    Public Sub UpdateTimeStamp(ByVal DocNo As Integer, ByVal strType As String)
        Dim oRec As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strType = "IN" Then
            sQuery = " Update [@Z_HR_OSEAPP] Set U_Z_AIUserID = '" & oApplication.Company.UserName.ToString & "',U_Z_AIUDate = GetDate() Where DocEntry = '" & DocNo & "'"
            oRec.DoQuery(sQuery)
        ElseIf strType = "SF" Then
            sQuery = " Update [@Z_HR_OSEAPP] Set U_Z_SFUserID = '" & oApplication.Company.UserName.ToString & "',U_Z_SFUDate = GetDate() Where DocEntry = '" & DocNo & "'"
            oRec.DoQuery(sQuery)
        ElseIf strType = "SFA" Then
            sQuery = " Update [@Z_HR_OSEAPP] Set U_Z_SFAUserID = '" & oApplication.Company.UserName.ToString & "',U_Z_SFAUDate = GetDate() Where DocEntry = '" & DocNo & "'"
            oRec.DoQuery(sQuery)
        ElseIf strType = "FL" Then
            sQuery = " Update [@Z_HR_OSEAPP] Set U_Z_FUserID = '" & oApplication.Company.UserName.ToString & "',U_Z_FUDate = GetDate() Where DocEntry = '" & DocNo & "'"
            oRec.DoQuery(sQuery)
        ElseIf strType = "SL" Then
            sQuery = " Update [@Z_HR_OSEAPP] Set U_Z_SCUserID = '" & oApplication.Company.UserName.ToString & "',U_Z_SCUDate = GetDate() Where DocEntry = '" & DocNo & "'"
            oRec.DoQuery(sQuery)
        ElseIf strType = "HR" Then
            sQuery = " Update [@Z_HR_OSEAPP] Set U_Z_HRUserID = '" & oApplication.Company.UserName.ToString & "',U_Z_HRDate = GetDate() Where DocEntry = '" & DocNo & "'"
            oRec.DoQuery(sQuery)
        End If
    End Sub

    Public Sub UpdateRecruitmentTimeStamp(ByVal DocNo As Integer, ByVal strType As String)
        Dim oRec As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strType = "CR" Then
            sQuery = " Update ""@Z_HR_ORMPREQ"" Set ""U_Z_CREId""='" & ManagerId.Trim() & "', ""U_Z_CRUser"" = '" & ManagerName.Trim() & "',""U_Z_CRDate"" = GetDate() Where ""DocEntry"" = '" & DocNo & "'"
            oRec.DoQuery(sQuery)
        ElseIf strType = "FL" Then
            sQuery = " Update ""@Z_HR_ORMPREQ"" Set ""U_Z_FLEId""='" & ManagerId.Trim() & "', ""U_Z_FLUser"" = '" & ManagerName.Trim() & "',""U_Z_FLDate"" = GetDate() Where ""DocEntry"" = '" & DocNo & "' and ""U_Z_MgrStatus""<>'O'"
            oRec.DoQuery(sQuery)
        ElseIf strType = "HR" Then
            sQuery = " Update ""@Z_HR_ORMPREQ"" Set ""U_Z_SLEId""='" & ManagerId.Trim() & "', ""U_Z_HRUser"" = '" & ManagerName.Trim() & "',""U_Z_HRDate"" = GetDate() Where ""DocEntry"" = '" & DocNo & "' and ""U_Z_MgrStatus""<>'O'"
            oRec.DoQuery(sQuery)
        ElseIf strType = "CL" Then
            sQuery = " Update ""@Z_HR_ORMPREQ"" Set ""U_Z_CLEId""='" & oApplication.Company.UserSignature.ToString() & "', ""U_Z_CLUser"" = '" & oApplication.Company.UserName.ToString & "',""U_Z_CLDate"" = GetDate() Where ""DocEntry"" = '" & DocNo & "'"
            oRec.DoQuery(sQuery)
        End If
    End Sub

    Public Sub UpdateApplicantTimeStamp(ByVal DocNo As Integer, ByVal strType As String)
        Dim oRec As SAPbobsCOM.Recordset
        oRec = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        If strType = "CR" Then
            sQuery = " Update [@Z_HR_OCRAPP] Set U_Z_CRUser = '" & oApplication.Company.UserName.ToString & "',U_Z_CRDate = GetDate() Where DocEntry = '" & DocNo & "'"
            oRec.DoQuery(sQuery)
        ElseIf strType = "SFL" Then
            sQuery = " Update [@Z_HR_OCRAPP] Set U_Z_SFLUser = '" & oApplication.Company.UserName.ToString & "',U_Z_SFLDate = GetDate() Where DocEntry = '" & DocNo & "'"
            oRec.DoQuery(sQuery)
        ElseIf strType = "SSL" Then
            sQuery = " Update [@Z_HR_OCRAPP] Set U_Z_SSLUser = '" & oApplication.Company.UserName.ToString & "',U_Z_SSLDate = GetDate() Where DocEntry = '" & DocNo & "'"
            oRec.DoQuery(sQuery)
        ElseIf strType = "FL" Then
            sQuery = " Update [@Z_HR_OCRAPP] Set U_Z_FLUser = '" & oApplication.Company.UserName.ToString & "',U_Z_FLDate = GetDate() Where DocEntry = '" & DocNo & "'"
            oRec.DoQuery(sQuery)
        ElseIf strType = "HR" Then
            sQuery = " Update [@Z_HR_OCRAPP] Set U_Z_HRUser = '" & oApplication.Company.UserName.ToString & "',U_Z_HRDate = GetDate() Where DocEntry = '" & DocNo & "'"
            oRec.DoQuery(sQuery)
        ElseIf strType = "LU" Then
            sQuery = " Update [@Z_HR_OCRAPP] Set U_Z_LUUser = '" & oApplication.Company.UserName.ToString & "',U_Z_LUDate = GetDate() Where DocEntry = '" & DocNo & "'"
            oRec.DoQuery(sQuery)
        ElseIf strType = "HI" Then
            sQuery = " Update [@Z_HR_OCRAPP] Set U_Z_HIUser = '" & oApplication.Company.UserName.ToString & "',U_Z_HIDate = GetDate() Where DocEntry = '" & DocNo & "'"
            oRec.DoQuery(sQuery)
        End If
    End Sub
    Public Sub ApprovalSummary(ByVal aForm As SAPbouiCOM.Form, ByVal enDocType As modVariables.HeaderDoctype, ByVal aChoice As modVariables.HistoryDoctype)
        Try
            aForm.Freeze(True)
            Dim oTempDt As SAPbouiCOM.DataTable
            Dim oGrid As SAPbouiCOM.Grid
            oGrid = aForm.Items.Item("19").Specific
            Select Case enDocType

                Case HeaderDoctype.ExpCli
                    sQuery = " Select Code,T0.U_Z_EmpID,U_Z_EmpName,U_Z_SubDt,U_Z_Claimdt,U_Z_ExpType,U_Z_Currency,U_Z_CurAmt,U_Z_UsdAmt,U_Z_ReimAmt,U_Z_Attachment,U_Z_AppStatus,U_Z_Client,""U_Z_Month"",""U_Z_Year"",U_Z_Project "
                    sQuery += " From [@Z_HR_EXPCL] T0 JOIN [@Z_HR_APPT1] T1 ON T0.U_Z_EmpID = T1.U_Z_OUser "
                    sQuery += " JOIN [@Z_HR_APPT2] T2 ON T1.DocEntry = T2.DocEntry "
                    sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
                    sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and  T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = '" + HeaderDoctype.ExpCli.ToString() + "' Order by Convert(Numeric,Code) Desc"
                Case HeaderDoctype.TraReq
                    sQuery = " Select T0.DocEntry,T0.U_Z_EmpId,U_Z_EmpName,U_Z_DocDate,U_Z_TraName,U_Z_TraStLoc,U_Z_TraEdLoc,U_Z_TraStDate,U_Z_TraEndDate,U_Z_AppStatus "
                    sQuery += " From [@Z_HR_OTRAREQ] T0 JOIN [@Z_HR_APPT1] T1 ON T0.U_Z_EmpId = T1.U_Z_OUser "
                    sQuery += " JOIN [@Z_HR_APPT2] T2 ON T1.DocEntry = T2.DocEntry "
                    sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
                    sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and  T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = '" + HeaderDoctype.TraReq.ToString() + "' Order by T0.DocEntry desc "
                Case HeaderDoctype.Train
                    Select Case aChoice
                        Case HistoryDoctype.RegTra
                            sQuery = "  select T0.Code,T0.U_Z_HREmpID,U_Z_HREmpName,U_Z_TrainCode,U_Z_CourseCode,U_Z_CourseName,U_Z_CourseTypeDesc,U_Z_Startdt,U_Z_Enddt"
                            sQuery += " ,U_Z_AppStatus from [@Z_HR_TRIN1] T0 JOIN [@Z_HR_APPT1] T1 ON T0.U_Z_HREmpID = T1.U_Z_OUser "
                            sQuery += " JOIN [@Z_HR_APPT2] T2 ON T1.DocEntry = T2.DocEntry "
                            sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
                            sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and  T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = '" + HeaderDoctype.Train.ToString() + "' Order by Convert(Numeric,T0.Code) desc "
                        Case HistoryDoctype.NewTra
                            sQuery = "select T0.DocEntry,U_Z_ReqDate,T0.U_Z_HREmpID,U_Z_HREmpName,U_Z_CourseName,U_Z_CourseDetails,convert(varchar(10),U_Z_TrainFrdt,103) as U_Z_TrainFrdt,convert(varchar(10),U_Z_TrainTodt,103) as U_Z_TrainTodt,U_Z_TrainCost,U_Z_Notes"
                            sQuery += " ,U_Z_AppStatus from [@Z_HR_ONTREQ] T0 JOIN [@Z_HR_APPT1] T1 ON T0.U_Z_HREmpID = T1.U_Z_OUser "
                            sQuery += " JOIN [@Z_HR_APPT2] T2 ON T1.DocEntry = T2.DocEntry "
                            sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
                            sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and  T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = '" + HeaderDoctype.Train.ToString() + "' Order by T0.DocEntry desc "
                    End Select

                Case HeaderDoctype.Rec
                    Select Case aChoice
                        Case HistoryDoctype.Rec
                            sQuery = " Select T0.DocEntry,U_Z_ReqDate,T0.U_Z_EmpCode,U_Z_EmpName,T0.U_Z_DeptCode,T1.U_Z_DeptName,ISNULL(U_Z_PosName, '') as U_Z_PosName,U_Z_ExpMin,U_Z_ExpMax,U_Z_Vacancy,U_Z_AppStatus"
                            sQuery += " From [@Z_HR_ORMPREQ] T0 JOIN [@Z_HR_APPT3] T1 ON T0.U_Z_DeptCode = T1.U_Z_DeptCode"
                            sQuery += " JOIN [@Z_HR_APPT2] T2 ON T1.DocEntry = T2.DocEntry "
                            sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
                            sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and   T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = '" + HeaderDoctype.Rec.ToString() + "' Order by T0.DocEntry Desc"
                        Case HistoryDoctype.AppShort
                            sQuery = " Select T0.DocEntry,T0.U_Z_HRAppID,T0.U_Z_HRAppName,T0.U_Z_ReqNo,T0.U_Z_AppDate,T1.U_Z_DeptCode,T0.U_Z_DeptName,T0.U_Z_Email,T0.U_Z_YrExp,T0.U_Z_Skills,T0.U_Z_AppStatus"
                            sQuery += " From [@Z_HR_OHEM1] T0 JOIN [@Z_HR_ORMPREQ] T1 ON T1.DocEntry = T0.U_Z_ReqNo "
                            sQuery += " JOIN [@Z_HR_APPT3] T2 ON T1.U_Z_DeptCode = T2.U_Z_DeptCode"
                            sQuery += " JOIN [@Z_HR_APPT2] T3 ON T2.DocEntry = T3.DocEntry "
                            sQuery += " JOIN [@Z_HR_OAPPT] T4 ON T3.DocEntry = T4.DocEntry  "
                            sQuery += " And isnull(T3.U_Z_AMan,'N')='Y' AND isnull(T4.U_Z_Active,'N')='Y' and  T0.U_Z_AppStatus='P' And T3.U_Z_AUser = '" + oApplication.Company.UserName + "' And T4.U_Z_DocType = '" + HeaderDoctype.Rec.ToString() + "' Order by T0.DocEntry Desc"
                        Case HistoryDoctype.Final
                            sQuery = " Select T0.DocEntry,T0.U_Z_HRAppID,T0.U_Z_HRAppName,T0.U_Z_ReqNo,T0.U_Z_AppDate,T1.U_Z_DeptCode,T0.U_Z_DeptName,T0.U_Z_Email,T0.U_Z_YrExp,T0.U_Z_Skills,T0.U_Z_IPHODSta,T0.U_Z_AppStatus"
                            sQuery += " From [@Z_HR_OHEM1] T0 JOIN [@Z_HR_ORMPREQ] T1 ON T1.DocEntry = T0.U_Z_ReqNo "
                            sQuery += " JOIN [@Z_HR_APPT3] T2 ON T1.U_Z_DeptCode = T2.U_Z_DeptCode"
                            sQuery += " JOIN [@Z_HR_APPT2] T3 ON T2.DocEntry = T3.DocEntry "
                            sQuery += " JOIN [@Z_HR_OAPPT] T4 ON T3.DocEntry = T4.DocEntry  "
                            sQuery += " And isnull(T3.U_Z_AMan,'N')='Y' AND isnull(T4.U_Z_Active,'N')='Y' and  T0.U_Z_AppStatus='A' And (T0.U_Z_IntervStatus <>'P' and T0.U_Z_IntervStatus <>'F') And T3.U_Z_AUser = '" + oApplication.Company.UserName + "' And T4.U_Z_DocType = '" + HeaderDoctype.Rec.ToString() + "' Order by T0.DocEntry Desc"
                    End Select

                Case HeaderDoctype.EmpLife
                    Select Case aChoice
                        Case HistoryDoctype.EmpPro
                            sQuery = " Select ""Code"",T0.""U_Z_EmpId"",T0.""U_Z_FirstName"",T0.U_Z_Dept,T1.""U_Z_DeptName"",""U_Z_PosName"",""U_Z_JobName"",""U_Z_OrgName"",""U_Z_ProJoinDate"",""U_Z_IncAmount"",""U_Z_EffFromdt"",""U_Z_EffTodt"",""U_Z_AppStatus"""
                            sQuery += " From ""@Z_HR_HEM2"" T0 Join OHEM R3 on R3.""empID""=T0.""U_Z_EmpId"" JOIN [@Z_HR_APPT3] T1 ON R3.""dept"" = T1.U_Z_DeptCode "
                            sQuery += " JOIN [@Z_HR_APPT2] T2 ON T1.DocEntry = T2.DocEntry "
                            sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry AND T0.""U_Z_Posting""='N'"
                            sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and  T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = '" + HeaderDoctype.EmpLife.ToString() + "' Order by Convert(Numeric,T0.Code) Desc"
                        Case HistoryDoctype.EmpPos
                            sQuery = " select ""Code"",T0.""U_Z_EmpId"",T0.""U_Z_FirstName"",T0.U_Z_Dept,T1.""U_Z_DeptName"",""U_Z_PosCode"",""U_Z_PosName"",""U_Z_JobName"",""U_Z_OrgCode"",""U_Z_OrgName"","
                            sQuery += """U_Z_NewPosDate"",""U_Z_EffFromdt"",""U_Z_EffTodt"",""U_Z_AppStatus"" "
                            sQuery += " from ""@Z_HR_HEM4"" T0 Join OHEM R3 on R3.""empID""=T0.""U_Z_EmpId"" JOIN [@Z_HR_APPT3] T1 ON R3.""dept"" = T1.U_Z_DeptCode "
                            sQuery += " JOIN [@Z_HR_APPT2] T2 ON T1.DocEntry = T2.DocEntry "
                            sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry AND T0.""U_Z_Posting""='N'"
                            sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and  T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = '" + HeaderDoctype.EmpLife.ToString() + "' Order by Convert(Numeric,Code) Desc"
                    End Select
                Case HeaderDoctype.LveReq
                    sQuery = "Select T0.""Code"" as ""Code"",T0.""U_Z_EMPID"",T0.""U_Z_EMPNAME"",""U_Z_TrnsCode"",convert(varchar(10),""U_Z_StartDate"",103) AS ""U_Z_StartDate"","
                    sQuery += " convert(varchar(10),""U_Z_EndDate"",103) AS ""U_Z_EndDate"" ,T0.""U_Z_NoofDays"",T0.""U_Z_LevBal"" 'Leave Balance',""U_Z_Notes"",convert(varchar(10),"
                    sQuery += " ""U_Z_ReJoiNDate"",103) AS ""U_Z_ReJoiNDate"",""U_Z_Month"",""U_Z_Year"",case ""U_Z_Status"" when 'P' then 'Pending' when 'R' then 'Rejected' "
                    sQuery += " when 'A' then 'Approved' end as ""U_Z_Status"" "
                    sQuery += " from ""@Z_PAY_OLETRANS1"" T0 JOIN [@Z_HR_APPT1] T1 ON T0.U_Z_EMPID = T1.U_Z_OUser "
                    sQuery += " JOIN [@Z_HR_APPT2] T2 ON T1.DocEntry = T2.DocEntry "
                    sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
                    sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = '" + HeaderDoctype.LveReq.ToString() + "' Order by Convert(Numeric,T0.Code) Desc"
            End Select
            oTempDt = aForm.DataSources.DataTables.Item("dtDocumentList")
            oTempDt.ExecuteQuery(sQuery)
            ' oGrid.DataTable = oTempDt
            oGrid.DataTable.ExecuteQuery(sQuery)
            SummaryDocument(aForm, enDocType, aChoice)
            oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
            oGrid.Columns.Item("RowsHeader").Click(0, False, False)
            aForm.Freeze(False)
        Catch ex As Exception
            aForm.Freeze(False)
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
    Private Sub SummaryDocument(ByVal aForm As SAPbouiCOM.Form, ByVal enDocType As modVariables.HeaderDoctype, ByVal aChoice As modVariables.HistoryDoctype)
        Try
            aForm.Freeze(True)
            Dim strQuery As String
            Dim oGrid As SAPbouiCOM.Grid
            Dim oGridCombo As SAPbouiCOM.ComboBoxColumn
            Dim oEditTextColumn As SAPbouiCOM.EditTextColumn
            Dim oRecSet As SAPbobsCOM.Recordset
            Dim oGECol As SAPbouiCOM.EditTextColumn
            oGrid = aForm.Items.Item("19").Specific
            Select Case enDocType
                Case HeaderDoctype.LveReq
                    oGrid.Columns.Item("Code").TitleObject.Caption = "Request No."
                    oEditTextColumn = oGrid.Columns.Item("Code")
                    oEditTextColumn.LinkedObjectType = "Z_HR_EXPANCES"
                    oGrid.Columns.Item("U_Z_EMPID").TitleObject.Caption = "Employee"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_EMPID")
                    oEditTextColumn.LinkedObjectType = "171"
                    oGrid.Columns.Item("U_Z_EMPNAME").TitleObject.Caption = "Employee Name"
                    oGrid.Columns.Item("U_Z_TrnsCode").TitleObject.Caption = "Leave Type"
                    oGrid.Columns.Item("U_Z_TrnsCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGridCombo = oGrid.Columns.Item("U_Z_TrnsCode")
                    oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    strQuery = "Select ""Code"",""Name"" from ""@Z_PAY_LEAVE"" order by ""Code"""
                    oRecSet.DoQuery(strQuery)
                    oGridCombo.ValidValues.Add("", "")
                    If Not oRecSet.EoF Then
                        For index As Integer = 0 To oRecSet.RecordCount - 1
                            If Not oRecSet.EoF Then
                                oGridCombo.ValidValues.Add(oRecSet.Fields.Item("Code").Value, oRecSet.Fields.Item("Name").Value)
                                oRecSet.MoveNext()
                            End If
                        Next
                    End If
                    oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                    oGridCombo.ExpandType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                    oGrid.Columns.Item("U_Z_StartDate").TitleObject.Caption = "From Date"
                    oGrid.Columns.Item("U_Z_EndDate").TitleObject.Caption = "To Date"
                    oGrid.Columns.Item("U_Z_NoofDays").TitleObject.Caption = "No.of Days"
                    oGrid.Columns.Item("U_Z_Notes").TitleObject.Caption = "Remarks"
                    oGrid.Columns.Item("U_Z_ReJoiNDate").TitleObject.Caption = "ReJoin Date"
                    oGrid.Columns.Item("U_Z_Month").TitleObject.Caption = "Month"
                    oGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Year"
                    oGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Status"
                    oGrid.Columns.Item("U_Z_Status").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGridCombo = oGrid.Columns.Item("U_Z_Status")
                    oGridCombo.ValidValues.Add("P", "Pending")
                    oGridCombo.ValidValues.Add("A", "Approved")
                    oGridCombo.ValidValues.Add("R", "Rejected")
                    oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oGrid.AutoResizeColumns()
                Case HeaderDoctype.EmpLife
                    Select Case aChoice
                        Case HistoryDoctype.EmpPos
                            oGrid.Columns.Item("Code").Visible = False
                            oGrid.Columns.Item("U_Z_Dept").Visible = False
                            oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee ID"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
                            oEditTextColumn.LinkedObjectType = 171
                            oGrid.Columns.Item("U_Z_FirstName").TitleObject.Caption = "Employee Name"
                            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
                            oGrid.Columns.Item("U_Z_PosCode").Visible = False
                            oGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position Name"
                            oGrid.Columns.Item("U_Z_JobName").TitleObject.Caption = "Job Name"
                            oGrid.Columns.Item("U_Z_OrgCode").Visible = False
                            oGrid.Columns.Item("U_Z_OrgName").TitleObject.Caption = "Organization Name"
                            oGrid.Columns.Item("U_Z_NewPosDate").Visible = False
                            oGrid.Columns.Item("U_Z_EffFromdt").TitleObject.Caption = "Effective From Date"
                            oGrid.Columns.Item("U_Z_EffTodt").TitleObject.Caption = "Effective To Date"
                            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
                            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                            oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                            oGridCombo.ValidValues.Add("P", "Pending")
                            oGridCombo.ValidValues.Add("A", "Approved")
                            oGridCombo.ValidValues.Add("C", "Cancelled")
                            oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                            oGrid.AutoResizeColumns()
                        Case HistoryDoctype.EmpPro
                            oGrid.Columns.Item("Code").Visible = False
                            oGrid.Columns.Item("U_Z_Dept").Visible = False
                            oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee ID"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
                            oEditTextColumn.LinkedObjectType = 171
                            oGrid.Columns.Item("U_Z_FirstName").TitleObject.Caption = "Employee Name"
                            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
                            oGrid.Columns.Item("U_Z_OrgName").TitleObject.Caption = "Organization Name"
                            oGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position Name"
                            oGrid.Columns.Item("U_Z_JobName").TitleObject.Caption = "Job Name"
                            oGrid.Columns.Item("U_Z_ProJoinDate").TitleObject.Caption = "Promotion Date"
                            oGrid.Columns.Item("U_Z_IncAmount").TitleObject.Caption = "Increment Amount"
                            oGrid.Columns.Item("U_Z_EffFromdt").TitleObject.Caption = "Effective From Date"
                            oGrid.Columns.Item("U_Z_EffTodt").TitleObject.Caption = "Effective To Date"
                            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
                            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                            oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                            oGridCombo.ValidValues.Add("P", "Pending")
                            oGridCombo.ValidValues.Add("A", "Approved")
                            oGridCombo.ValidValues.Add("C", "Cancelled")
                            oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                            oGrid.AutoResizeColumns()
                    End Select

                Case HeaderDoctype.Rec
                    Select Case aChoice
                        Case HistoryDoctype.Rec
                            oGrid.Columns.Item("U_Z_DeptCode").Visible = False
                            oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Request No"
                            oEditTextColumn = oGrid.Columns.Item("DocEntry")
                            oEditTextColumn.LinkedObjectType = "Z_HR_ONTREQ"
                            oGrid.Columns.Item("U_Z_ReqDate").TitleObject.Caption = "Request Date"
                            oGrid.Columns.Item("U_Z_EmpCode").TitleObject.Caption = "Employee Id"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_EmpCode")
                            oEditTextColumn.LinkedObjectType = 171
                            oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
                            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department"
                            oGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position"
                            oGrid.Columns.Item("U_Z_ExpMin").TitleObject.Caption = "Minimum Experience"
                            oGrid.Columns.Item("U_Z_ExpMax").TitleObject.Caption = "Maximum Experience"
                            oGrid.Columns.Item("U_Z_Vacancy").TitleObject.Caption = "Vacancy"
                            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
                            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                            oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                            oGridCombo.ValidValues.Add("P", "Pending")
                            oGridCombo.ValidValues.Add("A", "Approved")
                            oGridCombo.ValidValues.Add("R", "Rejected")
                            oGridCombo.ValidValues.Add("C", "Closed")
                            oGridCombo.ValidValues.Add("L", "Canceled")
                            oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                            oGrid.AutoResizeColumns()
                        Case HistoryDoctype.AppShort
                            oGrid.Columns.Item("DocEntry").Visible = False
                            oGrid.Columns.Item("U_Z_DeptCode").Visible = False
                            oGrid.Columns.Item("U_Z_ReqNo").TitleObject.Caption = "Recruitment No"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_ReqNo")
                            oEditTextColumn.LinkedObjectType = "Z_HR_ONTREQ"
                            oGrid.Columns.Item("U_Z_AppDate").TitleObject.Caption = "Request Date"
                            oGrid.Columns.Item("U_Z_HRAppID").TitleObject.Caption = "Applicant Id"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_HRAppID")
                            oEditTextColumn.LinkedObjectType = "Z_HR_OCRAPPL"
                            oGrid.Columns.Item("U_Z_HRAppName").TitleObject.Caption = "Applicant Name"
                            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department"
                            oGrid.Columns.Item("U_Z_Email").TitleObject.Caption = "Email"
                            oGrid.Columns.Item("U_Z_YrExp").TitleObject.Caption = "Year Of Experience"
                            oGrid.Columns.Item("U_Z_Skills").TitleObject.Caption = "Skills"
                            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
                            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                            oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                            oGridCombo.ValidValues.Add("P", "Pending")
                            oGridCombo.ValidValues.Add("A", "Approved")
                            oGridCombo.ValidValues.Add("R", "Rejected")
                            oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                            oGrid.AutoResizeColumns()
                        Case HistoryDoctype.Final
                            oGrid.Columns.Item("DocEntry").Visible = False
                            oGrid.Columns.Item("U_Z_DeptCode").Visible = False
                            oGrid.Columns.Item("U_Z_ReqNo").TitleObject.Caption = "Recruitment No"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_ReqNo")
                            oEditTextColumn.LinkedObjectType = "Z_HR_ONTREQ"
                            oGrid.Columns.Item("U_Z_AppDate").TitleObject.Caption = "Request Date"
                            oGrid.Columns.Item("U_Z_HRAppID").TitleObject.Caption = "Applicant Id"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_HRAppID")
                            oEditTextColumn.LinkedObjectType = "Z_HR_OCRAPPL"
                            oGrid.Columns.Item("U_Z_HRAppName").TitleObject.Caption = "Applicant Name"
                            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department"
                            oGrid.Columns.Item("U_Z_Email").TitleObject.Caption = "Email"
                            oGrid.Columns.Item("U_Z_YrExp").TitleObject.Caption = "Year Of Experience"
                            oGrid.Columns.Item("U_Z_Skills").TitleObject.Caption = "Skills"
                            oGrid.Columns.Item("U_Z_IPHODSta").TitleObject.Caption = "Approval Status"
                            oGrid.Columns.Item("U_Z_IPHODSta").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                            oGridCombo = oGrid.Columns.Item("U_Z_IPHODSta")
                            oGridCombo.ValidValues.Add("-", "Pending")
                            oGridCombo.ValidValues.Add("S", "Selected")
                            oGridCombo.ValidValues.Add("R", "Rejected")
                            oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
                            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                            oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                            oGridCombo.ValidValues.Add("P", "Pending")
                            oGridCombo.ValidValues.Add("A", "Approved")
                            oGridCombo.ValidValues.Add("R", "Rejected")
                            oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                            oGrid.Columns.Item("U_Z_AppStatus").Visible = False
                            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                            oGrid.AutoResizeColumns()
                    End Select
                Case HeaderDoctype.Train
                    Select Case aChoice
                        Case HistoryDoctype.NewTra
                            oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Request No"
                            oEditTextColumn = oGrid.Columns.Item("DocEntry")
                            oEditTextColumn.LinkedObjectType = "Z_HR_ONTREQ"
                            oGrid.Columns.Item("U_Z_ReqDate").TitleObject.Caption = "Request Date"
                            oGrid.Columns.Item("U_Z_HREmpID").TitleObject.Caption = "Employee Id"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_HREmpID")
                            oEditTextColumn.LinkedObjectType = 171
                            oGrid.Columns.Item("U_Z_HREmpName").TitleObject.Caption = "Employee Name"
                            oGrid.Columns.Item("U_Z_CourseName").TitleObject.Caption = "Training Title"
                            oGrid.Columns.Item("U_Z_CourseDetails").TitleObject.Caption = "Justification"
                            oGrid.Columns.Item("U_Z_TrainFrdt").TitleObject.Caption = "Training From Date"
                            oGrid.Columns.Item("U_Z_TrainTodt").TitleObject.Caption = "Training To Date"
                            oGrid.Columns.Item("U_Z_TrainCost").TitleObject.Caption = "Training Course Cost"
                            oGrid.Columns.Item("U_Z_Notes").TitleObject.Caption = "Comments"
                            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
                            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                            oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                            oGridCombo.ValidValues.Add("P", "Pending")
                            oGridCombo.ValidValues.Add("A", "Approved")
                            oGridCombo.ValidValues.Add("R", "Rejected")
                            oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                            oGrid.AutoResizeColumns()
                        Case HistoryDoctype.RegTra
                            oGrid.Columns.Item("Code").Visible = False
                            oGrid.Columns.Item("U_Z_HREmpID").TitleObject.Caption = "Employee"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_HREmpID")
                            oEditTextColumn.LinkedObjectType = "171"
                            oGrid.Columns.Item("U_Z_HREmpName").TitleObject.Caption = "Employee Name"
                            oGrid.Columns.Item("U_Z_TrainCode").TitleObject.Caption = "Agenda Code"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_TrainCode")
                            oEditTextColumn.LinkedObjectType = "Z_HR_OTRIN"
                            oGrid.Columns.Item("U_Z_CourseCode").TitleObject.Caption = "Course Code"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_CourseCode")
                            oEditTextColumn.LinkedObjectType = "Z_HR_OCOURS"
                            oGrid.Columns.Item("U_Z_CourseName").TitleObject.Caption = "Course Name"
                            oGrid.Columns.Item("U_Z_CourseTypeDesc").TitleObject.Caption = "Course Type"
                            oGrid.Columns.Item("U_Z_Startdt").TitleObject.Caption = "Start Date"
                            oGrid.Columns.Item("U_Z_Enddt").TitleObject.Caption = "End Date"
                            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
                            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                            oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                            oGridCombo.ValidValues.Add("P", "Pending")
                            oGridCombo.ValidValues.Add("A", "Approved")
                            oGridCombo.ValidValues.Add("R", "Rejected")
                            oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                            oGrid.AutoResizeColumns()
                    End Select
                Case HeaderDoctype.ExpCli
                    oGrid.Columns.Item("Code").TitleObject.Caption = "Request No."
                    oGrid.Columns.Item("Code").Visible = False
                    oGrid.Columns.Item("U_Z_EmpID").TitleObject.Caption = "Employee"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_EmpID")
                    oEditTextColumn.LinkedObjectType = "171"
                    oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
                    oGrid.Columns.Item("U_Z_SubDt").TitleObject.Caption = "Submitted Date"
                    oGrid.Columns.Item("U_Z_Claimdt").TitleObject.Caption = "Transaction Date"
                    oGrid.Columns.Item("U_Z_ExpType").TitleObject.Caption = "Expense Type"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_ExpType")
                    oEditTextColumn.LinkedObjectType = "Z_HR_EXPANCES"
                    oGrid.Columns.Item("U_Z_Currency").TitleObject.Caption = "Transaction Currency"
                    oGrid.Columns.Item("U_Z_Client").TitleObject.Caption = "Client"
                    oGrid.Columns.Item("U_Z_Project").TitleObject.Caption = "Project"
                    oGrid.Columns.Item("U_Z_CurAmt").TitleObject.Caption = "Transaction Amount"
                    oGrid.Columns.Item("U_Z_UsdAmt").TitleObject.Caption = "Local Currency Amount"
                    oGrid.Columns.Item("U_Z_ReimAmt").TitleObject.Caption = "Redim Amount"
                    oGrid.Columns.Item("U_Z_Attachment").TitleObject.Caption = "Attachments"
                    oGECol = oGrid.Columns.Item("U_Z_Attachment")
                    oGECol.LinkedObjectType = "Z_HR_OEXFOM"
                    oGrid.Columns.Item("U_Z_Month").TitleObject.Caption = "Month"
                    oGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Year"
                    oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
                    oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                    oGridCombo.ValidValues.Add("P", "Pending")
                    oGridCombo.ValidValues.Add("A", "Approved")
                    oGridCombo.ValidValues.Add("R", "Rejected")
                    oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oGrid.AutoResizeColumns()
                Case HeaderDoctype.TraReq
                    oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Request No."
                    oEditTextColumn = oGrid.Columns.Item("DocEntry")
                    oEditTextColumn.LinkedObjectType = "Z_HR_OTRAREQ"
                    oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
                    oEditTextColumn.LinkedObjectType = "171"
                    oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
                    oGrid.Columns.Item("U_Z_DocDate").TitleObject.Caption = "Submitted Date"
                    oGrid.Columns.Item("U_Z_TraName").TitleObject.Caption = "Travel Description"
                    oGrid.Columns.Item("U_Z_TraStLoc").TitleObject.Caption = "From Location"
                    oGrid.Columns.Item("U_Z_TraEdLoc").TitleObject.Caption = "To Location"
                    oGrid.Columns.Item("U_Z_TraStDate").TitleObject.Caption = "From Date"
                    oGrid.Columns.Item("U_Z_TraEndDate").TitleObject.Caption = "To Date"
                    oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
                    oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                    oGridCombo.ValidValues.Add("P", "Pending")
                    oGridCombo.ValidValues.Add("A", "Approved")
                    oGridCombo.ValidValues.Add("R", "Rejected")
                    oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oGrid.AutoResizeColumns()
            End Select
            aForm.Freeze(False)
        Catch ex As Exception
            aForm.Freeze(False)
            Throw ex
        End Try
    End Sub
    Public Sub SummaryHistory(ByVal aForm As SAPbouiCOM.Form, ByVal enDocType As modVariables.HistoryDoctype, ByVal strDocEntry As String)
        Try
            aForm.Freeze(True)
            Dim oTempDt As SAPbouiCOM.DataTable
            oGrid = aForm.Items.Item("20").Specific
            Select Case enDocType
                Case HistoryDoctype.RegTra, HistoryDoctype.NewTra, HistoryDoctype.Rec, HistoryDoctype.AppShort, HistoryDoctype.EmpPro, HistoryDoctype.EmpPos, HistoryDoctype.TraReq, HistoryDoctype.Final
                    sQuery = " Select DocEntry,U_Z_DocEntry,U_Z_DocType,U_Z_EmpId,U_Z_EmpName,U_Z_ApproveBy,CreateDate ,CreateTime,UpdateDate,UpdateTime,U_Z_AppStatus,U_Z_Remarks From [@Z_HR_APHIS] "
                    sQuery += " Where U_Z_DocType = '" + enDocType.ToString() + "'"
                    sQuery += " And U_Z_DocEntry = '" + strDocEntry + "'"
                Case HistoryDoctype.ExpCli, HistoryDoctype.LveReq
                    sQuery = " Select DocEntry,U_Z_DocEntry,U_Z_DocType,U_Z_EmpId,U_Z_EmpName,U_Z_ApproveBy,CreateDate ,CreateTime,UpdateDate,UpdateTime,U_Z_AppStatus,U_Z_Remarks,U_Z_Year,U_Z_Month From [@Z_HR_APHIS] "
                    sQuery += " Where U_Z_DocType = '" + enDocType.ToString() + "'"
                    sQuery += " And U_Z_DocEntry = '" + strDocEntry + "'"
            End Select
            oTempDt = aForm.DataSources.DataTables.Item("dtHistoryList")
            oTempDt.ExecuteQuery(sQuery)
            oGrid.DataTable = oTempDt
            SummaryformatHistory(aForm, enDocType)
            oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
            aForm.Freeze(False)
        Catch ex As Exception
            aForm.Freeze(False)
            Throw ex
        End Try
    End Sub
    Private Sub SummaryformatHistory(ByVal aForm As SAPbouiCOM.Form, ByVal enDocType As modVariables.HistoryDoctype)
        Try
            aForm.Freeze(True)
            Dim oGrid As SAPbouiCOM.Grid
            Dim oComboBox As SAPbouiCOM.ComboBox
            Dim oGridCombo As SAPbouiCOM.ComboBoxColumn
            Dim oEditTextColumn As SAPbouiCOM.EditTextColumn
            oGrid = aForm.Items.Item("20").Specific
            Select Case enDocType
                Case HistoryDoctype.TraReq, HistoryDoctype.RegTra, HistoryDoctype.NewTra, HistoryDoctype.Rec, HistoryDoctype.AppShort, HistoryDoctype.EmpPro, HistoryDoctype.EmpPos, HistoryDoctype.Final
                    oGrid.Columns.Item("DocEntry").Visible = False
                    oGrid.Columns.Item("U_Z_DocEntry").TitleObject.Caption = "Reference No."
                    oGrid.Columns.Item("U_Z_DocEntry").Visible = False
                    oGrid.Columns.Item("U_Z_DocType").Visible = False
                    oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee ID"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
                    oEditTextColumn.LinkedObjectType = "171"
                    oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
                    oGrid.Columns.Item("U_Z_ApproveBy").TitleObject.Caption = "Approved By"
                    oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approved Status"
                    oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                    oGridCombo.ValidValues.Add("P", "Pending")
                    oGridCombo.ValidValues.Add("A", "Approved")
                    oGridCombo.ValidValues.Add("R", "Rejected")
                    oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                    oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oGrid.AutoResizeColumns()
                Case HistoryDoctype.ExpCli, HistoryDoctype.LveReq
                    oGrid.Columns.Item("DocEntry").Visible = False
                    oGrid.Columns.Item("U_Z_DocEntry").TitleObject.Caption = "Reference No."
                    oGrid.Columns.Item("U_Z_DocEntry").Visible = False
                    oGrid.Columns.Item("U_Z_DocType").Visible = False
                    oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee ID"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
                    oEditTextColumn.LinkedObjectType = "171"
                    oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
                    oGrid.Columns.Item("U_Z_ApproveBy").TitleObject.Caption = "Approved By"
                    oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approved Status"
                    oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                    oGridCombo.ValidValues.Add("P", "Pending")
                    oGridCombo.ValidValues.Add("A", "Approved")
                    oGridCombo.ValidValues.Add("R", "Rejected")
                    oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                    oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"
                    oGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Year"
                    oGrid.Columns.Item("U_Z_Month").TitleObject.Caption = "Month"
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oGrid.AutoResizeColumns()
            End Select
            aForm.Freeze(False)
        Catch ex As Exception
            aForm.Freeze(False)
            Throw ex
        End Try
    End Sub
    Public Function getLeaveType(ByVal aCode As String) As String
        Dim oTest As SAPbobsCOM.Recordset
        Dim intManagerid As String
        Dim strEmp As String = ""
        oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTest.DoQuery("select T0.U_Z_LveType from [@Z_HR_OAPPT] T0 JOIN [@Z_HR_APPT2] T1 on T0.DocEntry=T1.DocEntry where T1.U_Z_AUser ='" & aCode & "'")
        If oTest.RecordCount > 0 Then
            For intRow As Integer = 0 To oTest.RecordCount - 1

                If strEmp = "" Then
                    strEmp = "'" & oTest.Fields.Item(0).Value & "'"
                Else
                    strEmp = strEmp & " ,'" & oTest.Fields.Item(0).Value & "'"
                End If
                oTest.MoveNext()
            Next
            Return strEmp
        Else
            Return "99999"
        End If
    End Function
    Public Sub InitializationApproval(ByVal aForm As SAPbouiCOM.Form, ByVal enDocType As modVariables.HeaderDoctype, ByVal aChoice As modVariables.HistoryDoctype)
        Try
            aForm.Freeze(True)
            Dim oTempDt As SAPbouiCOM.DataTable
            Dim oGrid As SAPbouiCOM.Grid
            oGrid = aForm.Items.Item("1").Specific
            Select Case enDocType
              
                Case HeaderDoctype.ExpCli
                    sQuery = " Select Code,T0.U_Z_EmpID,U_Z_EmpName,U_Z_SubDt,U_Z_Claimdt,U_Z_ExpType,U_Z_Currency,U_Z_CurAmt,U_Z_UsdAmt,U_Z_ReimAmt,U_Z_Attachment,U_Z_AppStatus,U_Z_Client,U_Z_Project,""U_Z_Month"",""U_Z_Year"" "
                    sQuery += " From [@Z_HR_EXPCL] T0 JOIN [@Z_HR_APPT1] T1 ON T0.U_Z_EmpID = T1.U_Z_OUser  and (T0.""U_Z_AppStatus""='P' or T0.""U_Z_AppStatus""='-') "
                    sQuery += " JOIN [@Z_HR_APPT2] T2 ON T1.DocEntry = T2.DocEntry "
                    sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
                    sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and  T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = '" + HeaderDoctype.ExpCli.ToString() + "' Order by Convert(Numeric,Code) Desc"
                Case HeaderDoctype.TraReq
                    sQuery = " Select T0.DocEntry,T0.U_Z_EmpId,U_Z_EmpName,U_Z_DocDate,U_Z_TraName,U_Z_TraStLoc,U_Z_TraEdLoc,U_Z_TraStDate,U_Z_TraEndDate,U_Z_AppStatus "
                    sQuery += " From [@Z_HR_OTRAREQ] T0 JOIN [@Z_HR_APPT1] T1 ON T0.U_Z_EmpId = T1.U_Z_OUser and (T0.""U_Z_AppStatus""='P' or T0.""U_Z_AppStatus""='-') "
                    sQuery += " JOIN [@Z_HR_APPT2] T2 ON T1.DocEntry = T2.DocEntry "
                    sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
                    sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and  T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = '" + HeaderDoctype.TraReq.ToString() + "' Order by T0.DocEntry desc "
                Case HeaderDoctype.Train
                    Select Case aChoice
                        Case HistoryDoctype.RegTra
                            sQuery = "  select T0.Code,T0.U_Z_HREmpID,U_Z_HREmpName,U_Z_TrainCode,U_Z_CourseCode,U_Z_CourseName,U_Z_CourseTypeDesc,U_Z_Startdt,U_Z_Enddt"
                            sQuery += " ,U_Z_AppStatus from [@Z_HR_TRIN1] T0 JOIN [@Z_HR_APPT1] T1 ON T0.U_Z_HREmpID = T1.U_Z_OUser and (T0.""U_Z_AppStatus""='P' or T0.""U_Z_AppStatus""='-') "
                            sQuery += " JOIN [@Z_HR_APPT2] T2 ON T1.DocEntry = T2.DocEntry "
                            sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
                            sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and  T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = '" + HeaderDoctype.Train.ToString() + "' Order by Convert(Numeric,T0.Code) desc "
                        Case HistoryDoctype.NewTra
                            sQuery = "select T0.DocEntry,U_Z_ReqDate,T0.U_Z_HREmpID,U_Z_HREmpName,U_Z_CourseName,U_Z_CourseDetails,convert(varchar(10),U_Z_TrainFrdt,103) as U_Z_TrainFrdt,convert(varchar(10),U_Z_TrainTodt,103) as U_Z_TrainTodt,U_Z_TrainCost,U_Z_Notes"
                            sQuery += " ,U_Z_AppStatus from [@Z_HR_ONTREQ] T0 JOIN [@Z_HR_APPT1] T1 ON T0.U_Z_HREmpID = T1.U_Z_OUser and (T0.""U_Z_AppStatus""='P' or T0.""U_Z_AppStatus""='-') "
                            sQuery += " JOIN [@Z_HR_APPT2] T2 ON T1.DocEntry = T2.DocEntry "
                            sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
                            sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and  T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = '" + HeaderDoctype.Train.ToString() + "' Order by T0.DocEntry desc "
                    End Select

                Case HeaderDoctype.Rec
                    Select Case aChoice
                        Case HistoryDoctype.Rec
                            sQuery = " Select T0.DocEntry,U_Z_ReqDate,T0.U_Z_EmpCode,U_Z_EmpName,T0.U_Z_DeptCode,T1.U_Z_DeptName,ISNULL(U_Z_PosName, '') as U_Z_PosName,U_Z_ExpMin,U_Z_ExpMax,U_Z_Vacancy,U_Z_AppStatus"
                            sQuery += " From [@Z_HR_ORMPREQ] T0 JOIN [@Z_HR_APPT3] T1 ON T0.U_Z_DeptCode = T1.U_Z_DeptCode and (T0.""U_Z_AppStatus""='P' or T0.""U_Z_AppStatus""='-') "
                            sQuery += " JOIN [@Z_HR_APPT2] T2 ON T1.DocEntry = T2.DocEntry "
                            sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
                            sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and   T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = '" + HeaderDoctype.Rec.ToString() + "' Order by T0.DocEntry Desc"
                        Case HistoryDoctype.AppShort
                            sQuery = " Select T0.DocEntry,T0.U_Z_HRAppID,T0.U_Z_HRAppName,T0.U_Z_ReqNo,T0.U_Z_AppDate,T1.U_Z_DeptCode,T0.U_Z_DeptName,T0.U_Z_Email,T0.U_Z_YrExp,T0.U_Z_Skills,T0.U_Z_AppStatus"
                            sQuery += " From [@Z_HR_OHEM1] T0 JOIN [@Z_HR_ORMPREQ] T1 ON T1.DocEntry = T0.U_Z_ReqNo and (T0.""U_Z_AppStatus""='P' or T0.""U_Z_AppStatus""='-') "
                            sQuery += " JOIN [@Z_HR_APPT3] T2 ON T1.U_Z_DeptCode = T2.U_Z_DeptCode"
                            sQuery += " JOIN [@Z_HR_APPT2] T3 ON T2.DocEntry = T3.DocEntry "
                            sQuery += " JOIN [@Z_HR_OAPPT] T4 ON T3.DocEntry = T4.DocEntry  "
                            sQuery += " And isnull(T3.U_Z_AMan,'N')='Y' AND isnull(T4.U_Z_Active,'N')='Y' and  T0.U_Z_AppStatus='P' And T3.U_Z_AUser = '" + oApplication.Company.UserName + "' And T4.U_Z_DocType = '" + HeaderDoctype.Rec.ToString() + "' Order by T0.DocEntry Desc"
                        Case HistoryDoctype.Final
                            sQuery = " Select T0.DocEntry,T0.U_Z_HRAppID,T0.U_Z_HRAppName,T0.U_Z_ReqNo,T0.U_Z_AppDate,T1.U_Z_DeptCode,T0.U_Z_DeptName,T0.U_Z_Email,T0.U_Z_YrExp,T0.U_Z_Skills,T0.U_Z_IPHODSta,T0.U_Z_AppStatus"
                            sQuery += " From [@Z_HR_OHEM1] T0 JOIN [@Z_HR_ORMPREQ] T1 ON T1.DocEntry = T0.U_Z_ReqNo "
                            sQuery += " JOIN [@Z_HR_APPT3] T2 ON T1.U_Z_DeptCode = T2.U_Z_DeptCode"
                            sQuery += " JOIN [@Z_HR_APPT2] T3 ON T2.DocEntry = T3.DocEntry "
                            sQuery += " JOIN [@Z_HR_OAPPT] T4 ON T3.DocEntry = T4.DocEntry  "
                            'sQuery += " And isnull(T3.U_Z_AMan,'N')='Y' AND isnull(T4.U_Z_Active,'N')='Y' and  T0.U_Z_AppStatus='A' And (T0.U_Z_IntervStatus ='P' and T0.U_Z_IntervStatus <>'F') And T3.U_Z_AUser = '" + oApplication.Company.UserName + "' And T4.U_Z_DocType = '" + HeaderDoctype.Rec.ToString() + "' Order by T0.DocEntry Desc"
                            sQuery += " And isnull(T3.U_Z_AMan,'N')='Y' AND isnull(T4.U_Z_Active,'N')='Y' and  T0.U_Z_AppStatus='A' And T0.U_Z_IntervStatus ='O' And T3.U_Z_AUser = '" + oApplication.Company.UserName + "' And T4.U_Z_DocType = '" + HeaderDoctype.Rec.ToString() + "' Order by T0.DocEntry Desc"
                    End Select

                Case HeaderDoctype.EmpLife
                    Select Case aChoice
                        Case HistoryDoctype.EmpPro
                            sQuery = " Select ""Code"",T0.""U_Z_EmpId"",T0.""U_Z_FirstName"",T0.U_Z_Dept,T1.""U_Z_DeptName"",""U_Z_PosName"",""U_Z_JobName"",""U_Z_OrgName"",""U_Z_ProJoinDate"",""U_Z_IncAmount"",""U_Z_EffFromdt"",""U_Z_EffTodt"",""U_Z_AppStatus"""
                            sQuery += " From ""@Z_HR_HEM2"" T0 JOIN OHEM R3 on R3.""empID""=T0.""U_Z_EmpId"" Join  [@Z_HR_APPT3] T1 ON R3.""dept"" = T1.U_Z_DeptCode and (T0.""U_Z_AppStatus""='P' or T0.""U_Z_AppStatus""='-')"
                            sQuery += " JOIN [@Z_HR_APPT2] T2 ON T1.DocEntry = T2.DocEntry "
                            sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry AND T0.""U_Z_Posting""='N'"
                            sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and  T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = '" + HeaderDoctype.EmpLife.ToString() + "' Order by Convert(Numeric,T0.Code) Desc"
                        Case HistoryDoctype.EmpPos
                            sQuery = " select ""Code"",T0.""U_Z_EmpId"",T0.""U_Z_FirstName"",T0.U_Z_Dept,T1.""U_Z_DeptName"",""U_Z_PosCode"",""U_Z_PosName"",""U_Z_JobName"",""U_Z_OrgCode"",""U_Z_OrgName"","
                            sQuery += """U_Z_NewPosDate"",""U_Z_EffFromdt"",""U_Z_EffTodt"",""U_Z_AppStatus"" "
                            sQuery += " from ""@Z_HR_HEM4"" T0 JOIN OHEM R3 on R3.""empID""=T0.""U_Z_EmpId"" Join  [@Z_HR_APPT3] T1 ON R3.""dept"" = T1.U_Z_DeptCode and (T0.""U_Z_AppStatus""='P' or T0.""U_Z_AppStatus""='-')  "
                            sQuery += " JOIN [@Z_HR_APPT2] T2 ON T1.DocEntry = T2.DocEntry "
                            sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry AND T0.""U_Z_Posting""='N'"
                            sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and  T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = '" + HeaderDoctype.EmpLife.ToString() + "' Order by Convert(Numeric,Code) Desc"
                    End Select
                Case HeaderDoctype.LveReq
                    Dim strLvetype As String = getLeaveType(oApplication.Company.UserName)
                    sQuery = "Select T0.""Code"" as ""Code"",T0.""U_Z_EMPID"",T0.""U_Z_EMPNAME"",""U_Z_TrnsCode"",convert(varchar(10),""U_Z_StartDate"",103) AS ""U_Z_StartDate"","
                    sQuery += " convert(varchar(10),""U_Z_EndDate"",103) AS ""U_Z_EndDate"" ,T0.""U_Z_NoofDays"",T0.""U_Z_LevBal"" 'Leave Balance',""U_Z_Notes"",convert(varchar(10),"
                    sQuery += " ""U_Z_ReJoiNDate"",103) AS ""U_Z_ReJoiNDate"",""U_Z_Month"",""U_Z_Year"",case ""U_Z_Status"" when 'P' then 'Pending' when 'R' then 'Rejected' "
                    sQuery += " when 'A' then 'Approved' end as ""U_Z_Status"" "
                    sQuery += " from ""@Z_PAY_OLETRANS1"" T0 JOIN [@Z_HR_APPT1] T1 ON T0.U_Z_EMPID = T1.U_Z_OUser and (T0.""U_Z_Status""='P' or T0.""U_Z_Status""='-') "
                    sQuery += " JOIN [@Z_HR_APPT2] T2 ON T1.DocEntry = T2.DocEntry "
                    sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
                    sQuery += " And isnull(T2.U_Z_AMan,'N')='Y' AND isnull(T3.U_Z_Active,'N')='Y' and  T2.U_Z_AUser = '" + oApplication.Company.UserName + "'"
                    sQuery += "  And T3.U_Z_DocType = '" + HeaderDoctype.LveReq.ToString() + "' AND ""U_Z_TrnsCode"" in (" & strLvetype & ") Order by Convert(Numeric,T0.Code) Desc"
            End Select
            oTempDt = aForm.DataSources.DataTables.Item("dtDocumentList")
            oTempDt.ExecuteQuery(sQuery)
            ' oGrid.DataTable = oTempDt
            oGrid.DataTable.ExecuteQuery(sQuery)
            formatDocument(aForm, enDocType, aChoice)
            oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
            oGrid.Columns.Item("RowsHeader").Click(0, False, False)
            aForm.Freeze(False)
        Catch ex As Exception
            aForm.Freeze(False)
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub


    Private Sub formatDocument(ByVal aForm As SAPbouiCOM.Form, ByVal enDocType As modVariables.HeaderDoctype, ByVal aChoice As modVariables.HistoryDoctype)
        Try
            aForm.Freeze(True)
            Dim strQuery As String
            Dim oGrid As SAPbouiCOM.Grid
            Dim oGridCombo As SAPbouiCOM.ComboBoxColumn
            Dim oEditTextColumn As SAPbouiCOM.EditTextColumn
            Dim oRecSet As SAPbobsCOM.Recordset
            Dim oGECol As SAPbouiCOM.EditTextColumn
            oGrid = aForm.Items.Item("1").Specific
            Select Case enDocType
                Case HeaderDoctype.LveReq
                    oGrid.Columns.Item("Code").TitleObject.Caption = "Request No."
                    oEditTextColumn = oGrid.Columns.Item("Code")
                    oEditTextColumn.LinkedObjectType = "Z_HR_EXPANCES"
                    oGrid.Columns.Item("U_Z_EMPID").TitleObject.Caption = "Employee"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_EMPID")
                    oEditTextColumn.LinkedObjectType = "171"
                    oGrid.Columns.Item("U_Z_EMPNAME").TitleObject.Caption = "Employee Name"
                    oGrid.Columns.Item("U_Z_TrnsCode").TitleObject.Caption = "Leave Type"
                    oGrid.Columns.Item("U_Z_TrnsCode").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGridCombo = oGrid.Columns.Item("U_Z_TrnsCode")
                    oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    strQuery = "Select ""Code"",""Name"" from ""@Z_PAY_LEAVE"" order by ""Code"""
                    oRecSet.DoQuery(strQuery)
                    oGridCombo.ValidValues.Add("", "")
                    If Not oRecSet.EoF Then
                        For index As Integer = 0 To oRecSet.RecordCount - 1
                            If Not oRecSet.EoF Then
                                oGridCombo.ValidValues.Add(oRecSet.Fields.Item("Code").Value, oRecSet.Fields.Item("Name").Value)
                                oRecSet.MoveNext()
                            End If
                        Next
                    End If
                    oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                    oGridCombo.ExpandType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                    oGrid.Columns.Item("U_Z_StartDate").TitleObject.Caption = "From Date"
                    oGrid.Columns.Item("U_Z_EndDate").TitleObject.Caption = "To Date"
                    oGrid.Columns.Item("U_Z_NoofDays").TitleObject.Caption = "No.of Days"
                    oGrid.Columns.Item("U_Z_Notes").TitleObject.Caption = "Remarks"
                    oGrid.Columns.Item("U_Z_ReJoiNDate").TitleObject.Caption = "ReJoin Date"
                    oGrid.Columns.Item("U_Z_Month").TitleObject.Caption = "Month"
                    oGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Year"
                    oGrid.Columns.Item("U_Z_Status").TitleObject.Caption = "Status"
                    oGrid.Columns.Item("U_Z_Status").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGridCombo = oGrid.Columns.Item("U_Z_Status")
                    oGridCombo.ValidValues.Add("P", "Pending")
                    oGridCombo.ValidValues.Add("A", "Approved")
                    oGridCombo.ValidValues.Add("R", "Rejected")
                    oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oGrid.AutoResizeColumns()
                Case HeaderDoctype.EmpLife
                    Select Case aChoice
                        Case HistoryDoctype.EmpPos
                            oGrid.Columns.Item("Code").Visible = False
                            oGrid.Columns.Item("U_Z_Dept").Visible = False
                            oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee ID"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
                            oEditTextColumn.LinkedObjectType = 171
                            oGrid.Columns.Item("U_Z_FirstName").TitleObject.Caption = "Employee Name"
                            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
                            oGrid.Columns.Item("U_Z_PosCode").Visible = False
                            oGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position Name"
                            oGrid.Columns.Item("U_Z_JobName").TitleObject.Caption = "Job Name"
                            oGrid.Columns.Item("U_Z_OrgCode").Visible = False
                            oGrid.Columns.Item("U_Z_OrgName").TitleObject.Caption = "Organization Name"
                            oGrid.Columns.Item("U_Z_NewPosDate").Visible = False
                            oGrid.Columns.Item("U_Z_EffFromdt").TitleObject.Caption = "Effective From Date"
                            oGrid.Columns.Item("U_Z_EffTodt").TitleObject.Caption = "Effective To Date"
                            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
                            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                            oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                            oGridCombo.ValidValues.Add("P", "Pending")
                            oGridCombo.ValidValues.Add("A", "Approved")
                            oGridCombo.ValidValues.Add("C", "Cancelled")
                            oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                            oGrid.AutoResizeColumns()
                        Case HistoryDoctype.EmpPro
                            oGrid.Columns.Item("Code").Visible = False
                            oGrid.Columns.Item("U_Z_Dept").Visible = False
                            oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee ID"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
                            oEditTextColumn.LinkedObjectType = 171
                            oGrid.Columns.Item("U_Z_FirstName").TitleObject.Caption = "Employee Name"
                            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department Name"
                            oGrid.Columns.Item("U_Z_OrgName").TitleObject.Caption = "Organization Name"
                            oGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position Name"
                            oGrid.Columns.Item("U_Z_JobName").TitleObject.Caption = "Job Name"
                            oGrid.Columns.Item("U_Z_ProJoinDate").TitleObject.Caption = "Promotion Date"
                            oGrid.Columns.Item("U_Z_IncAmount").TitleObject.Caption = "Increment Amount"
                            oGrid.Columns.Item("U_Z_EffFromdt").TitleObject.Caption = "Effective From Date"
                            oGrid.Columns.Item("U_Z_EffTodt").TitleObject.Caption = "Effective To Date"
                            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
                            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                            oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                            oGridCombo.ValidValues.Add("P", "Pending")
                            oGridCombo.ValidValues.Add("A", "Approved")
                            oGridCombo.ValidValues.Add("C", "Cancelled")
                            oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_both
                            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                            oGrid.AutoResizeColumns()
                    End Select

                Case HeaderDoctype.Rec
                    Select Case aChoice
                        Case HistoryDoctype.Rec
                            oGrid.Columns.Item("U_Z_DeptCode").Visible = False
                            oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Request No"
                            oEditTextColumn = oGrid.Columns.Item("DocEntry")
                            oEditTextColumn.LinkedObjectType = "Z_HR_ONTREQ"
                            oGrid.Columns.Item("U_Z_ReqDate").TitleObject.Caption = "Request Date"
                            oGrid.Columns.Item("U_Z_EmpCode").TitleObject.Caption = "Employee Id"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_EmpCode")
                            oEditTextColumn.LinkedObjectType = 171
                            oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
                            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department"
                            oGrid.Columns.Item("U_Z_PosName").TitleObject.Caption = "Position"
                            oGrid.Columns.Item("U_Z_ExpMin").TitleObject.Caption = "Minimum Experience"
                            oGrid.Columns.Item("U_Z_ExpMax").TitleObject.Caption = "Maximum Experience"
                            oGrid.Columns.Item("U_Z_Vacancy").TitleObject.Caption = "Vacancy"
                            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
                            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                            oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                            oGridCombo.ValidValues.Add("P", "Pending")
                            oGridCombo.ValidValues.Add("A", "Approved")
                            oGridCombo.ValidValues.Add("R", "Rejected")
                            oGridCombo.ValidValues.Add("C", "Closed")
                            oGridCombo.ValidValues.Add("L", "Canceled")
                            oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                            oGrid.AutoResizeColumns()
                        Case HistoryDoctype.AppShort
                            oGrid.Columns.Item("DocEntry").Visible = False
                            oGrid.Columns.Item("U_Z_DeptCode").Visible = False
                            oGrid.Columns.Item("U_Z_ReqNo").TitleObject.Caption = "Recruitment No"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_ReqNo")
                            oEditTextColumn.LinkedObjectType = "Z_HR_ONTREQ"
                            oGrid.Columns.Item("U_Z_AppDate").TitleObject.Caption = "Request Date"
                            oGrid.Columns.Item("U_Z_HRAppID").TitleObject.Caption = "Applicant Id"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_HRAppID")
                            oEditTextColumn.LinkedObjectType = "Z_HR_OCRAPPL"
                            oGrid.Columns.Item("U_Z_HRAppName").TitleObject.Caption = "Applicant Name"
                            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department"
                            oGrid.Columns.Item("U_Z_Email").TitleObject.Caption = "Email"
                            oGrid.Columns.Item("U_Z_YrExp").TitleObject.Caption = "Year Of Experience"
                            oGrid.Columns.Item("U_Z_Skills").TitleObject.Caption = "Skills"
                            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
                            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                            oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                            oGridCombo.ValidValues.Add("P", "Pending")
                            oGridCombo.ValidValues.Add("A", "Approved")
                            oGridCombo.ValidValues.Add("R", "Rejected")
                            oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                            oGrid.AutoResizeColumns()
                        Case HistoryDoctype.Final
                            oGrid.Columns.Item("DocEntry").Visible = False
                            oGrid.Columns.Item("U_Z_DeptCode").Visible = False
                            oGrid.Columns.Item("U_Z_ReqNo").TitleObject.Caption = "Recruitment No"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_ReqNo")
                            oEditTextColumn.LinkedObjectType = "Z_HR_ONTREQ"
                            oGrid.Columns.Item("U_Z_AppDate").TitleObject.Caption = "Request Date"
                            oGrid.Columns.Item("U_Z_HRAppID").TitleObject.Caption = "Applicant Id"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_HRAppID")
                            oEditTextColumn.LinkedObjectType = "Z_HR_OCRAPPL"
                            oGrid.Columns.Item("U_Z_HRAppName").TitleObject.Caption = "Applicant Name"
                            oGrid.Columns.Item("U_Z_DeptName").TitleObject.Caption = "Department"
                            oGrid.Columns.Item("U_Z_Email").TitleObject.Caption = "Email"
                            oGrid.Columns.Item("U_Z_YrExp").TitleObject.Caption = "Year Of Experience"
                            oGrid.Columns.Item("U_Z_Skills").TitleObject.Caption = "Skills"
                            oGrid.Columns.Item("U_Z_IPHODSta").TitleObject.Caption = "Approval Status"
                            oGrid.Columns.Item("U_Z_IPHODSta").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                            oGridCombo = oGrid.Columns.Item("U_Z_IPHODSta")
                            oGridCombo.ValidValues.Add("-", "Pending")
                            oGridCombo.ValidValues.Add("S", "Selected")
                            oGridCombo.ValidValues.Add("R", "Rejected")
                            oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
                            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                            oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                            oGridCombo.ValidValues.Add("P", "Pending")
                            oGridCombo.ValidValues.Add("A", "Approved")
                            oGridCombo.ValidValues.Add("R", "Rejected")
                            oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                            oGrid.Columns.Item("U_Z_AppStatus").Visible = False
                            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                            oGrid.AutoResizeColumns()
                    End Select
                Case HeaderDoctype.Train
                    Select Case aChoice
                        Case HistoryDoctype.NewTra
                            oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Request No"
                            oEditTextColumn = oGrid.Columns.Item("DocEntry")
                            oEditTextColumn.LinkedObjectType = "Z_HR_ONTREQ"
                            oGrid.Columns.Item("U_Z_ReqDate").TitleObject.Caption = "Request Date"
                            oGrid.Columns.Item("U_Z_HREmpID").TitleObject.Caption = "Employee Id"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_HREmpID")
                            oEditTextColumn.LinkedObjectType = 171
                            oGrid.Columns.Item("U_Z_HREmpName").TitleObject.Caption = "Employee Name"
                            oGrid.Columns.Item("U_Z_CourseName").TitleObject.Caption = "Training Title"
                            oGrid.Columns.Item("U_Z_CourseDetails").TitleObject.Caption = "Justification"
                            oGrid.Columns.Item("U_Z_TrainFrdt").TitleObject.Caption = "Training From Date"
                            oGrid.Columns.Item("U_Z_TrainTodt").TitleObject.Caption = "Training To Date"
                            oGrid.Columns.Item("U_Z_TrainCost").TitleObject.Caption = "Training Course Cost"
                            oGrid.Columns.Item("U_Z_Notes").TitleObject.Caption = "Comments"
                            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
                            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                            oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                            oGridCombo.ValidValues.Add("P", "Pending")
                            oGridCombo.ValidValues.Add("A", "Approved")
                            oGridCombo.ValidValues.Add("R", "Rejected")
                            oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                            oGrid.AutoResizeColumns()
                        Case HistoryDoctype.RegTra
                            oGrid.Columns.Item("Code").Visible = False
                            oGrid.Columns.Item("U_Z_HREmpID").TitleObject.Caption = "Employee"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_HREmpID")
                            oEditTextColumn.LinkedObjectType = "171"
                            oGrid.Columns.Item("U_Z_HREmpName").TitleObject.Caption = "Employee Name"
                            oGrid.Columns.Item("U_Z_TrainCode").TitleObject.Caption = "Agenda Code"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_TrainCode")
                            oEditTextColumn.LinkedObjectType = "Z_HR_OTRIN"
                            oGrid.Columns.Item("U_Z_CourseCode").TitleObject.Caption = "Course Code"
                            oEditTextColumn = oGrid.Columns.Item("U_Z_CourseCode")
                            oEditTextColumn.LinkedObjectType = "Z_HR_OCOURS"
                            oGrid.Columns.Item("U_Z_CourseName").TitleObject.Caption = "Course Name"
                            oGrid.Columns.Item("U_Z_CourseTypeDesc").TitleObject.Caption = "Course Type"
                            oGrid.Columns.Item("U_Z_Startdt").TitleObject.Caption = "Start Date"
                            oGrid.Columns.Item("U_Z_Enddt").TitleObject.Caption = "End Date"
                            oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
                            oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                            oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                            oGridCombo.ValidValues.Add("P", "Pending")
                            oGridCombo.ValidValues.Add("A", "Approved")
                            oGridCombo.ValidValues.Add("R", "Rejected")
                            oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                            oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                            oGrid.AutoResizeColumns()
                    End Select
                Case HeaderDoctype.ExpCli
                    oGrid.Columns.Item("Code").TitleObject.Caption = "Request No."
                    oGrid.Columns.Item("Code").Visible = False
                    oGrid.Columns.Item("U_Z_EmpID").TitleObject.Caption = "Employee"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_EmpID")
                    oEditTextColumn.LinkedObjectType = "171"
                    oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
                    oGrid.Columns.Item("U_Z_SubDt").TitleObject.Caption = "Submitted Date"
                    oGrid.Columns.Item("U_Z_Claimdt").TitleObject.Caption = "Transaction Date"
                    oGrid.Columns.Item("U_Z_ExpType").TitleObject.Caption = "Expense Type"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_ExpType")
                    oEditTextColumn.LinkedObjectType = "Z_HR_EXPANCES"
                    oGrid.Columns.Item("U_Z_Currency").TitleObject.Caption = "Transaction Currency"
                    oGrid.Columns.Item("U_Z_Client").TitleObject.Caption = "Client"
                    oGrid.Columns.Item("U_Z_Project").TitleObject.Caption = "Project"
                    oGrid.Columns.Item("U_Z_CurAmt").TitleObject.Caption = "Transaction Amount"
                    oGrid.Columns.Item("U_Z_UsdAmt").TitleObject.Caption = "Local Currency Amount"
                    oGrid.Columns.Item("U_Z_ReimAmt").TitleObject.Caption = "Redim Amount"
                    oGrid.Columns.Item("U_Z_Attachment").TitleObject.Caption = "Attachments"
                    oGECol = oGrid.Columns.Item("U_Z_Attachment")
                    oGECol.LinkedObjectType = "Z_HR_OEXFOM"
                    oGrid.Columns.Item("U_Z_Month").TitleObject.Caption = "Month"
                    oGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Year"
                    oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
                    oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                    oGridCombo.ValidValues.Add("P", "Pending")
                    oGridCombo.ValidValues.Add("A", "Approved")
                    oGridCombo.ValidValues.Add("R", "Rejected")
                    oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oGrid.AutoResizeColumns()
                Case HeaderDoctype.TraReq
                    oGrid.Columns.Item("DocEntry").TitleObject.Caption = "Request No."
                    oEditTextColumn = oGrid.Columns.Item("DocEntry")
                    oEditTextColumn.LinkedObjectType = "Z_HR_OTRAREQ"
                    oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
                    oEditTextColumn.LinkedObjectType = "171"
                    oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
                    oGrid.Columns.Item("U_Z_DocDate").TitleObject.Caption = "Submitted Date"
                    oGrid.Columns.Item("U_Z_TraName").TitleObject.Caption = "Travel Description"
                    oGrid.Columns.Item("U_Z_TraStLoc").TitleObject.Caption = "From Location"
                    oGrid.Columns.Item("U_Z_TraEdLoc").TitleObject.Caption = "To Location"
                    oGrid.Columns.Item("U_Z_TraStDate").TitleObject.Caption = "From Date"
                    oGrid.Columns.Item("U_Z_TraEndDate").TitleObject.Caption = "To Date"
                    oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approval Status"
                    oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                    oGridCombo.ValidValues.Add("P", "Pending")
                    oGridCombo.ValidValues.Add("A", "Approved")
                    oGridCombo.ValidValues.Add("R", "Rejected")
                    oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oGrid.AutoResizeColumns()
            End Select
            aForm.Freeze(False)
        Catch ex As Exception
            aForm.Freeze(False)
            Throw ex
        End Try
    End Sub

    Public Sub LoadHistory(ByVal aForm As SAPbouiCOM.Form, ByVal enDocType As modVariables.HistoryDoctype, ByVal strDocEntry As String)
        Try
            aForm.Freeze(True)
            Dim oTempDt As SAPbouiCOM.DataTable
            oGrid = aForm.Items.Item("3").Specific
            Select Case enDocType
                Case HistoryDoctype.RegTra, HistoryDoctype.NewTra, HistoryDoctype.Rec, HistoryDoctype.AppShort, HistoryDoctype.EmpPro, HistoryDoctype.EmpPos, HistoryDoctype.TraReq, HistoryDoctype.Final
                    sQuery = " Select DocEntry,U_Z_DocEntry,U_Z_DocType,U_Z_EmpId,U_Z_EmpName,U_Z_ApproveBy,CreateDate ,CreateTime,UpdateDate,UpdateTime,U_Z_AppStatus,U_Z_Remarks From [@Z_HR_APHIS] "
                    sQuery += " Where U_Z_DocType = '" + enDocType.ToString() + "'"
                    sQuery += " And U_Z_DocEntry = '" + strDocEntry + "'"
                Case HistoryDoctype.LveReq, HistoryDoctype.ExpCli
                    sQuery = " Select DocEntry,U_Z_DocEntry,U_Z_DocType,U_Z_EmpId,U_Z_EmpName,U_Z_ApproveBy,CreateDate ,CreateTime,UpdateDate,UpdateTime,U_Z_AppStatus,U_Z_Remarks,U_Z_Year,U_Z_Month From [@Z_HR_APHIS] "
                    sQuery += " Where U_Z_DocType = '" + enDocType.ToString() + "'"
                    sQuery += " And U_Z_DocEntry = '" + strDocEntry + "'"
            End Select
            oTempDt = aForm.DataSources.DataTables.Item("dtHistoryList")
            oTempDt.ExecuteQuery(sQuery)
            oGrid.DataTable = oTempDt
            formatHistory(aForm, enDocType)
            oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
            aForm.Freeze(False)
        Catch ex As Exception
            aForm.Freeze(False)
            Throw ex
        End Try
    End Sub

    Private Sub clearStatusRemarks(ByVal aForm As SAPbouiCOM.Form)
        Try
            oEdit = aForm.Items.Item("6").Specific
            oCombo = aForm.Items.Item("8").Specific
            oExEdit = aForm.Items.Item("10").Specific
            oCombo.Select("-", SAPbouiCOM.BoSearchKey.psk_ByValue)
            oEdit.Value = String.Empty
            oExEdit.Value = String.Empty
            aForm.Items.Item("8").Enabled = True
            aForm.Items.Item("10").Enabled = True
        Catch ex As Exception
            aForm.Freeze(False)
            Throw ex
        End Try
    End Sub

    Private Sub formatHistory(ByVal aForm As SAPbouiCOM.Form, ByVal enDocType As modVariables.HistoryDoctype)
        Try
            aForm.Freeze(True)
            Dim oGrid As SAPbouiCOM.Grid
            Dim oComboBox, oComboBox1, oComboBox2 As SAPbouiCOM.ComboBox
            Dim oGridCombo As SAPbouiCOM.ComboBoxColumn
            Dim oEditTextColumn As SAPbouiCOM.EditTextColumn
            Select Case enDocType
                Case HistoryDoctype.TraReq, HistoryDoctype.RegTra, HistoryDoctype.NewTra, HistoryDoctype.Rec, HistoryDoctype.AppShort, HistoryDoctype.EmpPro, HistoryDoctype.EmpPos, HistoryDoctype.Final
                    oGrid = aForm.Items.Item("3").Specific
                    oGrid.Columns.Item("DocEntry").Visible = False
                    oGrid.Columns.Item("U_Z_DocEntry").TitleObject.Caption = "Reference No."
                    oGrid.Columns.Item("U_Z_DocEntry").Visible = False
                    oGrid.Columns.Item("U_Z_DocType").Visible = False
                    oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee ID"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
                    oEditTextColumn.LinkedObjectType = "171"
                    oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
                    oGrid.Columns.Item("U_Z_ApproveBy").TitleObject.Caption = "Approved By"
                    oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approved Status"
                    oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                    oGridCombo.ValidValues.Add("P", "Pending")
                    oGridCombo.ValidValues.Add("A", "Approved")
                    oGridCombo.ValidValues.Add("R", "Rejected")
                    oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                    oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oGrid.AutoResizeColumns()
                Case HistoryDoctype.LveReq, HistoryDoctype.ExpCli
                    oGrid = aForm.Items.Item("3").Specific
                    oGrid.Columns.Item("DocEntry").Visible = False
                    oGrid.Columns.Item("U_Z_DocEntry").TitleObject.Caption = "Reference No."
                    oGrid.Columns.Item("U_Z_DocEntry").Visible = False
                    oGrid.Columns.Item("U_Z_DocType").Visible = False
                    oGrid.Columns.Item("U_Z_EmpId").TitleObject.Caption = "Employee ID"
                    oEditTextColumn = oGrid.Columns.Item("U_Z_EmpId")
                    oEditTextColumn.LinkedObjectType = "171"
                    oGrid.Columns.Item("U_Z_EmpName").TitleObject.Caption = "Employee Name"
                    oGrid.Columns.Item("U_Z_ApproveBy").TitleObject.Caption = "Approved By"
                    oGrid.Columns.Item("U_Z_AppStatus").TitleObject.Caption = "Approved Status"
                    oGrid.Columns.Item("U_Z_AppStatus").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox
                    oGridCombo = oGrid.Columns.Item("U_Z_AppStatus")
                    oGridCombo.ValidValues.Add("P", "Pending")
                    oGridCombo.ValidValues.Add("A", "Approved")
                    oGridCombo.ValidValues.Add("R", "Rejected")
                    oGridCombo.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description
                    oGrid.Columns.Item("U_Z_Remarks").TitleObject.Caption = "Remarks"
                    oGrid.Columns.Item("U_Z_Year").TitleObject.Caption = "Year"
                    oGrid.Columns.Item("U_Z_Month").TitleObject.Caption = "Month"
                    oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single
                    oGrid.AutoResizeColumns()
            End Select
            aForm.Freeze(False)
            Dim blnRecordExist As Boolean = False
            For intRow As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                If oGrid.DataTable.GetValue("U_Z_ApproveBy", intRow) = oApplication.Company.UserName Then
                    oGrid.Columns.Item("RowsHeader").Click(intRow, False, False)
                    blnRecordExist = True
                    aForm.Freeze(False)
                    Exit Sub
                End If
            Next
            Select Case enDocType
                Case HistoryDoctype.TraReq, HistoryDoctype.RegTra, HistoryDoctype.NewTra, HistoryDoctype.Rec, HistoryDoctype.AppShort, HistoryDoctype.EmpPro, HistoryDoctype.EmpPos, HistoryDoctype.Final
                    Try
                        If blnRecordExist = False Then
                            oCombo = aForm.Items.Item("8").Specific
                            oCombo.Select("P", SAPbouiCOM.BoSearchKey.psk_ByValue)
                            setEdittextvalue(aForm, "10", "")
                        End If
                    Catch ex As Exception
                    End Try
                Case HistoryDoctype.ExpCli, HistoryDoctype.LveReq
                    Try
                        If blnRecordExist = False Then
                            oComboBox1 = aForm.Items.Item("13").Specific
                            oComboBox2 = aForm.Items.Item("15").Specific
                            oCombo = aForm.Items.Item("8").Specific
                            oCombo.Select("P", SAPbouiCOM.BoSearchKey.psk_ByValue)
                            setEdittextvalue(aForm, "10", "")
                            oComboBox1.Select(Now.Month, SAPbouiCOM.BoSearchKey.psk_Index)
                            oComboBox2.Select(0, SAPbouiCOM.BoSearchKey.psk_Index)
                        End If
                    Catch ex As Exception
                    End Try
            End Select

          
            aForm.Items.Item("8").Enabled = True
            aForm.Items.Item("10").Enabled = True
        Catch ex As Exception
            aForm.Freeze(False)
            Throw ex
        End Try
    End Sub
    Public Sub LoadLeaveRemarks(ByVal aForm As SAPbouiCOM.Form, ByVal intRow As Integer)
        Try
            aForm.Freeze(True)
            oGrid = aForm.Items.Item("3").Specific
            oEdit = aForm.Items.Item("6").Specific
            oCombo = aForm.Items.Item("8").Specific
            oCombobox2 = aForm.Items.Item("13").Specific
            oCombobox1 = aForm.Items.Item("15").Specific
            oExEdit = aForm.Items.Item("10").Specific
            oEdit.Value = oGrid.DataTable.GetValue("DocEntry", intRow)
            oCombo.Select(oGrid.DataTable.GetValue("U_Z_AppStatus", intRow), SAPbouiCOM.BoSearchKey.psk_ByValue)
            oExEdit.Value = oGrid.DataTable.GetValue("U_Z_Remarks", intRow)
            Dim stYear As Integer = oGrid.DataTable.GetValue("U_Z_Year", intRow)
            oCombobox1.Select(stYear.ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue)
            oCombobox2.Select(oGrid.DataTable.GetValue("U_Z_Month", intRow), SAPbouiCOM.BoSearchKey.psk_Index)


            If oGrid.DataTable.GetValue("U_Z_ApproveBy", intRow) <> oApplication.Company.UserName Then
                aForm.Items.Item("6").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                aForm.Items.Item("8").Enabled = False
                aForm.Items.Item("10").Enabled = False
            Else
                aForm.Items.Item("8").Enabled = True
                aForm.Items.Item("10").Enabled = True
            End If
            aForm.Freeze(False)
        Catch ex As Exception
            aForm.Freeze(False)
            Throw ex
        End Try
    End Sub
    Public Sub LoadStatusRemarks(ByVal aForm As SAPbouiCOM.Form, ByVal intRow As Integer)
        Try
            aForm.Freeze(True)
            oGrid = aForm.Items.Item("3").Specific
            oEdit = aForm.Items.Item("6").Specific
            oCombo = aForm.Items.Item("8").Specific
            oExEdit = aForm.Items.Item("10").Specific
            oEdit.Value = oGrid.DataTable.GetValue("DocEntry", intRow)
            oCombo.Select(oGrid.DataTable.GetValue("U_Z_AppStatus", intRow), SAPbouiCOM.BoSearchKey.psk_ByValue)

            oExEdit.Value = oGrid.DataTable.GetValue("U_Z_Remarks", intRow)

            If oGrid.DataTable.GetValue("U_Z_ApproveBy", intRow) <> oApplication.Company.UserName Then
                aForm.Items.Item("6").Click(SAPbouiCOM.BoCellClickType.ct_Regular)
                aForm.Items.Item("8").Enabled = False
                aForm.Items.Item("10").Enabled = False
            Else
                aForm.Items.Item("8").Enabled = True
                aForm.Items.Item("10").Enabled = True
            End If
            aForm.Freeze(False)
        Catch ex As Exception
            aForm.Freeze(False)
            Throw ex
        End Try
    End Sub
    Public Sub addUpdateDocument(ByVal aForm As SAPbouiCOM.Form, ByVal enDocType As modVariables.HistoryDoctype, ByVal HeadDoc As modVariables.HeaderDoctype)
        Dim oGeneralService As SAPbobsCOM.GeneralService
        Dim oGeneralData As SAPbobsCOM.GeneralData
        Dim oGeneralParams As SAPbobsCOM.GeneralDataParams
        Dim oCompanyService As SAPbobsCOM.CompanyService
        Dim oChildren As SAPbobsCOM.GeneralDataCollection
        oCompanyService = oApplication.Company.GetCompanyService()
        Dim otestRs As SAPbobsCOM.Recordset
        Dim oChild As SAPbobsCOM.GeneralData
        Dim strCode, strQuery As String
        Dim blnRecordExists As Boolean = False
        Dim HeadDocEntry, UserLineId As Integer
        Dim oRecordSet As SAPbobsCOM.Recordset
        Dim oComboBox1, oCombobox2 As SAPbouiCOM.ComboBox
        Try
            If oApplication.SBO_Application.MessageBox("Documents once approved can not be changed. Do you want Continue?", , "Contine", "Cancel") = 2 Then
                Exit Sub
            End If
            oGeneralService = oCompanyService.GetGeneralService("Z_HR_APHIS")
            oGeneralData = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData)
            oGeneralParams = oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)
            oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            otestRs = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oGrid = aForm.Items.Item("1").Specific
            oEdit = aForm.Items.Item("6").Specific
            oCombo = aForm.Items.Item("8").Specific
            oExEdit = aForm.Items.Item("10").Specific
            Dim strDocEntry As String = ""
            Dim strDocType1 As String
            Dim strHeader As String = enDocType
            Dim strEmpID As String = ""
            Select Case enDocType
                Case HistoryDoctype.LveReq
                    strDocType1 = "Leave Request"
                    For index As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                        If oGrid.Rows.IsSelected(index) Then
                            strDocEntry = oGrid.DataTable.GetValue("Code", index)
                            strEmpID = oGrid.DataTable.GetValue("U_Z_EMPID", index)
                            Exit For
                        End If
                    Next
                Case HistoryDoctype.ExpCli
                    strDocType1 = "Expense Claim"
                    For index As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                        If oGrid.Rows.IsSelected(index) Then
                            strDocEntry = oGrid.DataTable.GetValue("Code", index)
                            strEmpID = oGrid.DataTable.GetValue("U_Z_EmpID", index)
                            Exit For
                        End If
                    Next
                Case HistoryDoctype.TraReq
                    strDocType1 = "Travel Request"
                    For index As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                        If oGrid.Rows.IsSelected(index) Then
                            strDocEntry = oGrid.DataTable.GetValue("DocEntry", index)
                            strEmpID = oGrid.DataTable.GetValue("U_Z_EmpId", index)
                            Exit For
                        End If
                    Next
                Case HistoryDoctype.RegTra
                    strDocType1 = "Reg.Trainning Request"
                    For index As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                        If oGrid.Rows.IsSelected(index) Then
                            strDocEntry = oGrid.DataTable.GetValue("Code", index)
                            strEmpID = oGrid.DataTable.GetValue("U_Z_HREmpID", index)
                            Exit For
                        End If
                    Next
                Case HistoryDoctype.NewTra
                    strDocType1 = "New Training Request"
                    For index As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                        If oGrid.Rows.IsSelected(index) Then
                            strDocEntry = oGrid.DataTable.GetValue("DocEntry", index)
                            strEmpID = oGrid.DataTable.GetValue("U_Z_HREmpID", index)
                            Exit For
                        End If
                    Next
                Case HistoryDoctype.Rec
                    strDocType1 = "ManPowere  Request"
                    For index As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                        If oGrid.Rows.IsSelected(index) Then
                            strDocEntry = oGrid.DataTable.GetValue("DocEntry", index)
                            strEmpID = oGrid.DataTable.GetValue("U_Z_DeptCode", index)
                            Exit For
                        End If
                    Next
                Case HistoryDoctype.AppShort
                    strDocType1 = "Candiate Shortlisting"
                    For index As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                        If oGrid.Rows.IsSelected(index) Then
                            strDocEntry = oGrid.DataTable.GetValue("DocEntry", index)
                            strEmpID = oGrid.DataTable.GetValue("U_Z_DeptCode", index)
                            Exit For
                        End If
                    Next
                Case HistoryDoctype.Final
                    strDocType1 = "Final Candidate Approval"
                    For index As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                        If oGrid.Rows.IsSelected(index) Then
                            strDocEntry = oGrid.DataTable.GetValue("DocEntry", index)
                            strEmpID = oGrid.DataTable.GetValue("U_Z_DeptCode", index)
                            Exit For
                        End If
                    Next
                Case HistoryDoctype.EmpPro
                    strDocType1 = "Employee Promotion Request"
                    For index As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                        If oGrid.Rows.IsSelected(index) Then
                            strDocEntry = oGrid.DataTable.GetValue("Code", index)
                            strEmpID = oGrid.DataTable.GetValue("U_Z_EmpId", index)
                            Dim oTest As SAPbobsCOM.Recordset
                            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            oTest.DoQuery("Select ""dept"" from OHEM where ""empID""=" & strEmpID)
                            strEmpID = oTest.Fields.Item(0).Value
                            Exit For
                        End If
                    Next
                Case HistoryDoctype.EmpPos
                    strDocType1 = "Employee Position Change"
                    For index As Integer = 0 To oGrid.DataTable.Rows.Count - 1
                        If oGrid.Rows.IsSelected(index) Then
                            strDocEntry = oGrid.DataTable.GetValue("Code", index)
                            strEmpID = oGrid.DataTable.GetValue("U_Z_EmpId", index)
                            Dim oTest As SAPbobsCOM.Recordset
                            oTest = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            oTest.DoQuery("Select ""dept"" from OHEM where ""empID""=" & strEmpID)
                            strEmpID = oTest.Fields.Item(0).Value
                            Exit For
                        End If
                    Next
            End Select
            Select Case HeadDoc
                Case HeaderDoctype.EmpLife, HeaderDoctype.Rec
                    strQuery = "select T0.DocEntry,T1.LineId from [@Z_HR_OAPPT] T0 JOIN [@Z_HR_APPT2] T1 on T0.DocEntry=T1.DocEntry"
                    strQuery += " JOIN [@Z_HR_APPT3] T2 on T1.DocEntry=T2.DocEntry"
                    strQuery += " where T0.U_Z_DocType='" & HeadDoc.ToString() & "' AND T2.U_Z_DeptCode='" & strEmpID & "' AND T1.U_Z_AUser='" & oApplication.Company.UserName & "'"
                Case HeaderDoctype.ExpCli, HeaderDoctype.Train, HeaderDoctype.TraReq, HeaderDoctype.LveReq
                    strQuery = "select T0.DocEntry,T1.LineId from [@Z_HR_OAPPT] T0 JOIN [@Z_HR_APPT2] T1 on T0.DocEntry=T1.DocEntry"
                    strQuery += " JOIN [@Z_HR_APPT1] T2 on T1.DocEntry=T2.DocEntry"
                    strQuery += " where T0.U_Z_DocType='" & HeadDoc.ToString() & "' AND T2.U_Z_OUser='" & strEmpID & "' AND T1.U_Z_AUser='" & oApplication.Company.UserName & "'"
            End Select
            otestRs.DoQuery(strQuery)
            If otestRs.RecordCount > 0 Then
                HeadDocEntry = otestRs.Fields.Item(0).Value
                UserLineId = otestRs.Fields.Item(1).Value
            End If
            Dim strEmpName As String = ""
            strQuery = "Select * from [@Z_HR_APHIS] where U_Z_DocEntry='" & strDocEntry & "' and U_Z_DocType='" & enDocType.ToString() & "' and U_Z_ApproveBy='" & oApplication.Company.UserName & "'"
            oRecordSet.DoQuery(strQuery)
            If oRecordSet.RecordCount > 0 Then
                oGeneralParams.SetProperty("DocEntry", oRecordSet.Fields.Item("DocEntry").Value)
                oGeneralData = oGeneralService.GetByParams(oGeneralParams)
                oGeneralData.SetProperty("U_Z_AppStatus", oCombo.Selected.Value)
                oGeneralData.SetProperty("U_Z_Remarks", oExEdit.Value)
                oGeneralData.SetProperty("U_Z_ADocEntry", HeadDocEntry)
                oGeneralData.SetProperty("U_Z_ALineId", UserLineId)
                If (strHeader = 9 Or strHeader = 0) Then
                    oComboBox1 = aForm.Items.Item("13").Specific
                    oCombobox2 = aForm.Items.Item("15").Specific
                    oGeneralData.SetProperty("U_Z_Month", oComboBox1.Selected.Value)
                    oGeneralData.SetProperty("U_Z_Year", oCombobox2.Selected.Value)
                End If

                Dim oTemp As SAPbobsCOM.Recordset
                oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oTemp.DoQuery("Select * ,isnull(""firstName"",'') +  ' ' + isnull(""middleName"",'') +  ' ' + isnull(""lastName"",'') 'EmpName' from OHEM where ""userid""=" & oApplication.Company.UserSignature)
                If oTemp.RecordCount > 0 Then
                    oGeneralData.SetProperty("U_Z_EmpId", oTemp.Fields.Item("empID").Value.ToString())
                    oGeneralData.SetProperty("U_Z_EmpName", oTemp.Fields.Item("EmpName").Value)
                    strEmpName = oTemp.Fields.Item("EmpName").Value
                Else
                    oGeneralData.SetProperty("U_Z_EmpId", "")
                    oGeneralData.SetProperty("U_Z_EmpName", "")
                End If
                oGeneralService.Update(oGeneralData)
            Else
                Dim oTemp As SAPbobsCOM.Recordset
                oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                oTemp.DoQuery("Select * ,isnull(""firstName"",'') + ' ' + isnull(""middleName"",'') +  ' ' + isnull(""lastName"",'') 'EmpName' from OHEM where ""userid""=" & oApplication.Company.UserSignature)
                If oTemp.RecordCount > 0 Then
                    oGeneralData.SetProperty("U_Z_EmpId", oTemp.Fields.Item("empID").Value.ToString())
                    oGeneralData.SetProperty("U_Z_EmpName", oTemp.Fields.Item("EmpName").Value)
                    strEmpName=otemp.Fields.Item("EmpName").Value 
                Else
                    oGeneralData.SetProperty("U_Z_EmpId", "")
                    oGeneralData.SetProperty("U_Z_EmpName", "")
                End If
                oGeneralData.SetProperty("U_Z_DocEntry", strDocEntry.ToString())
                oGeneralData.SetProperty("U_Z_DocType", enDocType.ToString())
                oGeneralData.SetProperty("U_Z_AppStatus", oCombo.Selected.Value)
                oGeneralData.SetProperty("U_Z_Remarks", oExEdit.Value)
                oGeneralData.SetProperty("U_Z_ApproveBy", oApplication.Company.UserName)
                oGeneralData.SetProperty("U_Z_Approvedt", System.DateTime.Now)
                oGeneralData.SetProperty("U_Z_ADocEntry", HeadDocEntry)
                oGeneralData.SetProperty("U_Z_ALineId", UserLineId)
                If (strHeader = 9 Or strHeader = 0) Then
                    oComboBox1 = aForm.Items.Item("13").Specific
                    oCombobox2 = aForm.Items.Item("15").Specific
                    oGeneralData.SetProperty("U_Z_Month", oComboBox1.Selected.Value)
                    oGeneralData.SetProperty("U_Z_Year", oCombobox2.Selected.Value)
                End If
                oGeneralService.Add(oGeneralData)
            End If
            updateFinalStatus(aForm, enDocType, strDocEntry, strEmpID, HeadDoc)
            SendMessage(strDocType1, strDocEntry, "Test", HeadDocEntry, strEmpName, oApplication.Company.UserName)
            LoadHistory(aForm, enDocType, strDocEntry)
            InitializationApproval(aForm, HeadDoc, enDocType)
            ApprovalSummary(aForm, HeadDoc, enDocType)
            'Select Case enDocType
            '    Case Doctype.ExpCli
            '        Dim objApproveTemp As New clshrTranApproval
            '        objApproveTemp.LoadForm(aForm)
            '    Case Doctype.TraReq
            '        Dim objApproveTemp As New clshrTravelApproval
            '        objApproveTemp.LoadForm(aForm)
            'End Select

        Catch ex As Exception
            Throw ex
        End Try
    End Sub
    Public Sub AddUDTPayroll(ByVal aForm As SAPbouiCOM.Form, ByVal strHeadcode As String)
        Dim strTable, strCode, strQuery As String
        Dim oUserTable, oUserTable1 As SAPbobsCOM.UserTable
        Dim oRecSet, oRec2 As SAPbobsCOM.Recordset
        oRecSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oRec2 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oUserTable = oApplication.Company.UserTables.Item("Z_PAY_OLETRANS")
        strTable = "@Z_PAY_OLETRANS"
        Try
            strQuery = "Select * from ""@Z_PAY_OLETRANS1"" where ""U_Z_Status""='A' and  ""Code""='" & strHeadcode & "'"
            oRecSet.DoQuery(strQuery)
            If oRecSet.RecordCount > 0 Then
                strCode = oApplication.Utilities.getMaxCode(strTable, "Code")
                oUserTable.Code = strCode
                oUserTable.Name = strCode
                oRec2.DoQuery("Select * from OHEM where empID=" & oRecSet.Fields.Item("U_Z_EMPID").Value)
                oUserTable.UserFields.Fields.Item("U_Z_EmpId1").Value = oRec2.Fields.Item("U_Z_EmpID").Value
                oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = oRecSet.Fields.Item("U_Z_EMPID").Value
                oUserTable.UserFields.Fields.Item("U_Z_EMPNAME").Value = oRecSet.Fields.Item("U_Z_EMPNAME").Value
                oUserTable.UserFields.Fields.Item("U_Z_TrnsCode").Value = oRecSet.Fields.Item("U_Z_TrnsCode").Value
                oUserTable.UserFields.Fields.Item("U_Z_LeaveName").Value = oRecSet.Fields.Item("U_Z_LeaveName").Value
                oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oRecSet.Fields.Item("U_Z_StartDate").Value
                oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = oRecSet.Fields.Item("U_Z_EndDate").Value
                oUserTable.UserFields.Fields.Item("U_Z_NoofDays").Value = oRecSet.Fields.Item("U_Z_NoofDays").Value
                oUserTable.UserFields.Fields.Item("U_Z_Notes").Value = oRecSet.Fields.Item("U_Z_Notes").Value
                oUserTable.UserFields.Fields.Item("U_Z_Month").Value = oRecSet.Fields.Item("U_Z_Month").Value
                oUserTable.UserFields.Fields.Item("U_Z_Year").Value = oRecSet.Fields.Item("U_Z_Year").Value
                oUserTable.UserFields.Fields.Item("U_Z_ReJoiNDate").Value = oRecSet.Fields.Item("U_Z_ReJoiNDate").Value
                If oUserTable.Add <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                End If
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub
    Public Sub updateFinalStatus(ByVal aForm As SAPbouiCOM.Form, ByVal enDocType As modVariables.HistoryDoctype, ByVal strDocEntry As String, ByVal aEmpID As String, ByVal HeadDocType As modVariables.HeaderDoctype)
        Try
            oCombo = aForm.Items.Item("8").Specific
            oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim oComboBox1, oComboBox2 As SAPbouiCOM.ComboBox
            oExEdit = aForm.Items.Item("10").Specific
            If oCombo.Selected.Value = "A" Then
                Select Case HeadDocType
                    Case HeaderDoctype.Rec, HeaderDoctype.EmpLife
                        sQuery = " Select T2.DocEntry "
                        sQuery += " From [@Z_HR_APPT2] T2 "
                        sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
                        sQuery += " JOIN [@Z_HR_APPT3] T4 ON T4.DocEntry = T3.DocEntry  "
                        sQuery += " Where T4.U_Z_DeptCode='" & aEmpID & "' and  U_Z_AFinal = 'Y'"
                        sQuery += " And T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = '" + HeadDocType.ToString() + "'"
                    Case HeaderDoctype.ExpCli, HeaderDoctype.Train, HeaderDoctype.TraReq, HeaderDoctype.LveReq
                        sQuery = " Select T2.DocEntry "
                        sQuery += " From [@Z_HR_APPT2] T2 "
                        sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
                        sQuery += " JOIN [@Z_HR_APPT1] T4 ON T4.DocEntry = T3.DocEntry  "
                        sQuery += " Where T4.U_Z_Ouser='" & aEmpID & "' and  U_Z_AFinal = 'Y'"
                        sQuery += " And T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = '" + HeadDocType.ToString() + "'"
                End Select
                oRecordSet.DoQuery(sQuery)
                If Not oRecordSet.EoF Then
                    Select Case enDocType
                        Case HistoryDoctype.LveReq
                            oComboBox1 = aForm.Items.Item("13").Specific
                            oComboBox2 = aForm.Items.Item("15").Specific
                            sQuery = "Update ""@Z_PAY_OLETRANS1"" Set U_Z_Year=" & oComboBox2.Selected.Value & ",U_Z_Month=" & oComboBox1.Selected.Value & ", U_Z_Status = 'A',""U_Z_AppRemarks""='" & oApplication.Utilities.getEdittextvalue(aForm, "10") & "' Where Code = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)
                            AddUDTPayroll(aForm, strDocEntry)
                        Case HistoryDoctype.ExpCli
                            oComboBox1 = aForm.Items.Item("13").Specific
                            oComboBox2 = aForm.Items.Item("15").Specific
                            sQuery = "Update [@Z_HR_EXPCL] Set U_Z_Year=" & oComboBox2.Selected.Value & ",U_Z_Month=" & oComboBox1.Selected.Value & ", U_Z_AppStatus = 'A' Where Code = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)
                            AddtoUDT1_PayrollTrans(strDocEntry)
                        Case HistoryDoctype.TraReq
                            sQuery = "Update [@Z_HR_OTRAREQ] Set U_Z_AppStatus = 'A' Where DocEntry = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)
                        Case HistoryDoctype.RegTra
                            sQuery = "Update [@Z_HR_TRIN1] Set U_Z_AppStatus = 'A' Where Code = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)
                        Case HistoryDoctype.NewTra
                            sQuery = "Update [@Z_HR_ONTREQ] Set U_Z_AppStatus = 'A' Where DocEntry = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)
                        Case HistoryDoctype.Rec
                            sQuery = "Update [@Z_HR_ORMPREQ] Set U_Z_AppStatus = 'A' Where DocEntry = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)
                        Case HistoryDoctype.AppShort
                            sQuery = "Update [@Z_HR_OHEM1] Set U_Z_AppStatus = 'A' Where DocEntry = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)
                            sQuery = "Select U_Z_HRAppID from [@Z_HR_OHEM1] where DocEntry = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)
                            If oRecordSet.RecordCount > 0 Then
                                sQuery = "Update [@Z_HR_OCRAPP] Set U_Z_Status = 'N' Where DocEntry = '" + oRecordSet.Fields.Item(0).Value + "'"
                                oTemp.DoQuery(sQuery)
                            End If
                        Case HistoryDoctype.EmpPro
                            sQuery = "Update [@Z_HR_HEM2] Set U_Z_AppStatus = 'A' Where Code = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)
                        Case HistoryDoctype.EmpPos
                            sQuery = "Update [@Z_HR_HEM4] Set U_Z_AppStatus = 'A' Where Code = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)
                        Case HistoryDoctype.Final
                            sQuery = "Update [@Z_HR_OHEM1] set U_Z_IntervStatus = 'A',U_Z_IPHODSta = 'S' where DocEntry = '" & strDocEntry & "'"
                            oRecordSet.DoQuery(sQuery)

                            oRecordSet.DoQuery("Select U_Z_HRAppID from [@Z_HR_OHEM1] where DocEntry='" & strDocEntry & "'")
                            If oRecordSet.RecordCount > 0 Then
                                sQuery = "Update [@Z_HR_OCRAPP] set U_Z_Status = 'M' where DocEntry = '" & oRecordSet.Fields.Item(0).Value & "'"
                                oTemp.DoQuery(sQuery)
                            End If
                    End Select
                End If
            ElseIf oCombo.Selected.Value = "R" Then
                Select Case HeadDocType
                    Case HeaderDoctype.Rec, HeaderDoctype.EmpLife
                        sQuery = " Select T2.DocEntry "
                        sQuery += " From [@Z_HR_APPT2] T2 "
                        sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
                        sQuery += " JOIN [@Z_HR_APPT3] T4 ON T4.DocEntry = T3.DocEntry  "
                        sQuery += " Where T4.U_Z_DeptCode='" & aEmpID & "' and  U_Z_AFinal = 'Y'"
                        sQuery += " And T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = '" + HeadDocType.ToString() + "'"
                    Case HeaderDoctype.ExpCli, HeaderDoctype.Train, HeaderDoctype.TraReq, HeaderDoctype.LveReq
                        sQuery = " Select T2.DocEntry "
                        sQuery += " From [@Z_HR_APPT2] T2 "
                        sQuery += " JOIN [@Z_HR_OAPPT] T3 ON T2.DocEntry = T3.DocEntry  "
                        sQuery += " JOIN [@Z_HR_APPT1] T4 ON T4.DocEntry = T3.DocEntry  "
                        sQuery += " Where T4.U_Z_Ouser='" & aEmpID & "' and  U_Z_AFinal = 'Y'"
                        sQuery += " And T2.U_Z_AUser = '" + oApplication.Company.UserName + "' And T3.U_Z_DocType = '" + HeadDocType.ToString() + "'"
                End Select
                oRecordSet.DoQuery(sQuery)
                If Not oRecordSet.EoF Then
                    Select Case enDocType
                        Case HistoryDoctype.LveReq
                            oComboBox1 = aForm.Items.Item("13").Specific
                            oComboBox2 = aForm.Items.Item("15").Specific
                            sQuery = "Update ""@Z_PAY_OLETRANS1"" Set U_Z_Year=" & oComboBox2.Selected.Value & ",U_Z_Month=" & oComboBox1.Selected.Value & ", U_Z_Status = 'R',""U_Z_AppRemarks""='" & oApplication.Utilities.getEdittextvalue(aForm, "10") & "' Where Code = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)
                        Case HistoryDoctype.ExpCli
                            oComboBox1 = aForm.Items.Item("13").Specific
                            oComboBox2 = aForm.Items.Item("15").Specific
                            sQuery = "Update [@Z_HR_EXPCL] Set U_Z_Year=" & oComboBox2.Selected.Value & ",U_Z_Month=" & oComboBox1.Selected.Value & ", U_Z_AppStatus = 'R' Where Code = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)
                        Case HistoryDoctype.TraReq
                            sQuery = "Update [@Z_HR_OTRAREQ] Set U_Z_AppStatus = 'R' Where DocEntry = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)
                        Case HistoryDoctype.RegTra
                            sQuery = "Update [@Z_HR_TRIN1] Set U_Z_AppStatus = 'R' Where Code = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)
                        Case HistoryDoctype.NewTra
                            sQuery = "Update [@Z_HR_ONTREQ] Set U_Z_AppStatus = 'R' Where DocEntry = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)
                        Case HistoryDoctype.Rec
                            sQuery = "Update [@Z_HR_ORMPREQ] Set U_Z_AppStatus = 'R' Where DocEntry = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)
                        Case HistoryDoctype.AppShort
                            sQuery = "Update [@Z_HR_OHEM1] Set U_Z_AppStatus = 'R' Where DocEntry = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)

                            sQuery = "Select U_Z_HRAppID from [@Z_HR_OHEM1] where DocEntry = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)
                            If oRecordSet.RecordCount > 0 Then
                                sQuery = "Update [@Z_HR_OCRAPP] Set U_Z_Status = 'R' Where DocEntry = '" + oRecordSet.Fields.Item(0).Value + "'"
                                oTemp.DoQuery(sQuery)
                            End If
                        Case HistoryDoctype.EmpPro
                            sQuery = "Update [@Z_HR_HEM2] Set U_Z_AppStatus = 'R' Where Code = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)
                        Case HistoryDoctype.EmpPos
                            sQuery = "Update [@Z_HR_HEM4] Set U_Z_AppStatus = 'R' Where Code = '" + strDocEntry + "'"
                            oRecordSet.DoQuery(sQuery)

                        Case HistoryDoctype.Final
                            sQuery = "Update [@Z_HR_OHEM1] set U_Z_APPlStatus='R' , U_Z_IntervStatus = 'R',U_Z_Finished = 'Y',U_Z_IPHODSta = 'R' where DocEntry = '" & strDocEntry & "'"
                            oRecordSet.DoQuery(sQuery)

                            oRecordSet.DoQuery("Select U_Z_HRAppID from [@Z_HR_OHEM1] where DocEntry='" & strDocEntry & "'")
                            If oRecordSet.RecordCount > 0 Then
                                sQuery = "Update [@Z_HR_OCRAPP] set U_Z_Status = 'R',U_Z_RejResn='" & oExEdit.Value & "' where DocEntry = '" & oRecordSet.Fields.Item(0).Value & "'"
                                oTemp.DoQuery(sQuery)
                            End If
                    End Select
                End If
            End If
        Catch ex As Exception
            oApplication.Utilities.Message(ex.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
    End Sub

    Public Sub Resize(ByVal aForm As SAPbouiCOM.Form)
        Try
            aForm.Freeze(True)
            aForm.Items.Item("1").Height = (aForm.Height / 2) - 50
            aForm.Items.Item("1").Width = aForm.Width - 10
            aForm.Items.Item("4").Top = aForm.Items.Item("1").Top + aForm.Items.Item("1").Height + 1
            aForm.Items.Item("5").Top = aForm.Items.Item("4").Top
            aForm.Items.Item("3").Top = aForm.Items.Item("4").Top + aForm.Items.Item("4").Height + 5
            aForm.Items.Item("3").Width = (aForm.Width / 2)
            aForm.Items.Item("3").Height = (aForm.Height / 2) - 50
            aForm.Items.Item("5").Left = aForm.Items.Item("3").Left + aForm.Items.Item("3").Width + 50
            aForm.Items.Item("7").Left = aForm.Items.Item("5").Left
            aForm.Items.Item("9").Left = aForm.Items.Item("5").Left
            aForm.Items.Item("8").Left = aForm.Items.Item("7").Left + aForm.Items.Item("7").Width + 1
            aForm.Items.Item("10").Left = aForm.Items.Item("9").Left + aForm.Items.Item("9").Width + 1
            aForm.Items.Item("8").Top = aForm.Items.Item("3").Top
            aForm.Items.Item("7").Top = aForm.Items.Item("8").Top
            aForm.Items.Item("10").Top = aForm.Items.Item("8").Top + aForm.Items.Item("8").Height + 1
            aForm.Items.Item("9").Top = aForm.Items.Item("10").Top
            aForm.Freeze(False)
        Catch ex As Exception

        End Try
    End Sub

    Public Function DocApproval(ByVal aForm As SAPbouiCOM.Form, ByVal DocType As modVariables.HeaderDoctype, ByVal Empid As String, Optional ByVal LeaveType As String = "") As String
        Try
            Dim strQuery As String = ""
            Dim Status As String = ""
            oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Select Case DocType
                Case HeaderDoctype.EmpLife, HeaderDoctype.Rec
                    strQuery = "Select * from ""@Z_HR_OAPPT"" T0 left join ""@Z_HR_APPT3"" T1 on T0.""DocEntry""=T1.""DocEntry"" where T0.""U_Z_Active""='Y' and T0.""U_Z_DocType""='" & DocType.ToString() & "' and T1.""U_Z_DeptCode""='" & Empid & "' "
                Case HeaderDoctype.ExpCli, HeaderDoctype.Train, HeaderDoctype.TraReq, HeaderDoctype.LveReq
                    If DocType = HeaderDoctype.LveReq Then
                        If LeaveType <> "" Then
                            strQuery = "Select * from ""@Z_HR_OAPPT"" T0 left join ""@Z_HR_APPT1"" T1 on T0.""DocEntry""=T1.""DocEntry"" where T0.""U_Z_LveType""='" & LeaveType & "' and  T0.""U_Z_Active""='Y' and T0.""U_Z_DocType""='" & DocType.ToString() & "' and T1.""U_Z_OUser""='" & Empid & "' "
                        Else
                            strQuery = "Select * from ""@Z_HR_OAPPT"" T0 left join ""@Z_HR_APPT1"" T1 on T0.""DocEntry""=T1.""DocEntry"" where T0.""U_Z_Active""='Y' and T0.""U_Z_DocType""='" & DocType.ToString() & "' and T1.""U_Z_OUser""='" & Empid & "' "
                        End If
                    Else
                        strQuery = "Select * from ""@Z_HR_OAPPT"" T0 left join ""@Z_HR_APPT1"" T1 on T0.""DocEntry""=T1.""DocEntry"" where T0.""U_Z_Active""='Y' and T0.""U_Z_DocType""='" & DocType.ToString() & "' and T1.""U_Z_OUser""='" & Empid & "' "

                    End If
               End Select
            oRecordSet.DoQuery(strQuery)
            If oRecordSet.RecordCount > 0 Then
                Status = "P"
            Else
                Status = "A"
            End If
            Return Status
        Catch ex As Exception
            MsgBox(oApplication.Company.GetLastErrorDescription)
            Return False
        End Try
    End Function


    Public Function GetTemplateID(ByVal aForm As SAPbouiCOM.Form, ByVal DocType As modVariables.HeaderDoctype, ByVal Empid As String, Optional ByVal LeaveType As String = "") As String
        Try
            Dim strQuery As String = ""
            Dim Status As String = ""
            oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Select Case DocType
                Case HeaderDoctype.EmpLife, HeaderDoctype.Rec
                    strQuery = "Select * from ""@Z_HR_OAPPT"" T0 left join ""@Z_HR_APPT3"" T1 on T0.""DocEntry""=T1.""DocEntry"" where isnull(T0.""U_Z_Active"",'N')='Y' and T0.""U_Z_DocType""='" & DocType.ToString() & "' and T1.""U_Z_DeptCode""='" & Empid & "' "
                Case HeaderDoctype.ExpCli, HeaderDoctype.Train, HeaderDoctype.TraReq, HeaderDoctype.LveReq
                    If DocType = HeaderDoctype.LveReq Then
                        If LeaveType <> "" Then
                            strQuery = "Select * from ""@Z_HR_OAPPT"" T0 left join ""@Z_HR_APPT1"" T1 on T0.""DocEntry""=T1.""DocEntry"" where T0.""U_Z_LveType""='" & LeaveType & "' and  isnull(T0.""U_Z_Active"",'N')='Y' and T0.""U_Z_DocType""='" & DocType.ToString() & "' and T1.""U_Z_OUser""='" & Empid & "' "
                        Else
                            strQuery = "Select * from ""@Z_HR_OAPPT"" T0 left join ""@Z_HR_APPT1"" T1 on T0.""DocEntry""=T1.""DocEntry"" where isnull(T0.""U_Z_Active"",'N')='Y' and T0.""U_Z_DocType""='" & DocType.ToString() & "' and T1.""U_Z_OUser""='" & Empid & "' "
                        End If
                    Else
                        strQuery = "Select * from ""@Z_HR_OAPPT"" T0 left join ""@Z_HR_APPT1"" T1 on T0.""DocEntry""=T1.""DocEntry"" where isnull(T0.""U_Z_Active"",'N')='Y' and T0.""U_Z_DocType""='" & DocType.ToString() & "' and T1.""U_Z_OUser""='" & Empid & "' "

                    End If
            End Select
            oRecordSet.DoQuery(strQuery)
            If oRecordSet.RecordCount > 0 Then
                Status = oRecordSet.Fields.Item("DocEntry").Value
            Else
                Status = "0"
            End If
            Return Status
        Catch ex As Exception
            MsgBox(oApplication.Company.GetLastErrorDescription)
            Return False
        End Try
    End Function

    Public Function AddtoUDT1_PayrollTrans(ByVal aCode As String) As Boolean
        Dim oUserTable As SAPbobsCOM.UserTable
        Dim strCode, strECode, strESocial, strEname, strETax, strGLAcc, strType, strEmp, strMonth, strYear As String
        Dim OCHECKBOXCOLUMN As SAPbouiCOM.CheckBoxColumn
        Dim oTemp, orec1 As SAPbobsCOM.Recordset
        oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        orec1 = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        oTemp.DoQuery("select Code,Name,* from [@Z_HR_EXPCL] where isnull(U_Z_PayPosted,'N')='N' and U_Z_Reimburse='Y' and  U_Z_APPStatus='A'  and Code='" & aCode & "'")
        If oTemp.RecordCount > 0 Then
            oUserTable = oApplication.Company.UserTables.Item("Z_PAY_TRANS")
            strCode = oApplication.Utilities.getMaxCode("@Z_PAY_TRANS", "Code")
            oUserTable.Code = strCode
            oUserTable.Name = strCode
            orec1.DoQuery("Select empID,U_Z_EmpId 'U_Z_EMPID',isnull(firstName,'')+' ' + isnull(middleName,'') +' ' + isnull(lastName,'') 'Name' from OHEM where empId=" & oTemp.Fields.Item("U_Z_EmpID").Value)
            oUserTable.UserFields.Fields.Item("U_Z_EmpId1").Value = orec1.Fields.Item("U_Z_EmpID").Value
            oUserTable.UserFields.Fields.Item("U_Z_TYPE").Value = "E"
            oUserTable.UserFields.Fields.Item("U_Z_Month").Value = oTemp.Fields.Item("U_Z_Month").Value
            oUserTable.UserFields.Fields.Item("U_Z_EMPNAME").Value = orec1.Fields.Item("Name").Value
            oUserTable.UserFields.Fields.Item("U_Z_Year").Value = oTemp.Fields.Item("U_Z_Year").Value
            oUserTable.UserFields.Fields.Item("U_Z_EMPID").Value = oTemp.Fields.Item("U_Z_EmpID").Value
            oUserTable.UserFields.Fields.Item("U_Z_TrnsCode").Value = oTemp.Fields.Item("U_Z_AlloCode").Value
            oUserTable.UserFields.Fields.Item("U_Z_StartDate").Value = oTemp.Fields.Item("U_Z_Claimdt").Value
            oUserTable.UserFields.Fields.Item("U_Z_EndDate").Value = "" ' oGrid.DataTable.GetValue("U_Z_EndDate", intRow)
            oUserTable.UserFields.Fields.Item("U_Z_Amount").Value = getDocumentQuantity(oTemp.Fields.Item("U_Z_ReimAmt").Value)
            oUserTable.UserFields.Fields.Item("U_Z_NoofHours").Value = 0 ' oTemp.Fields.Item("U_Z_EMPID").Value
            oUserTable.UserFields.Fields.Item("U_Z_Notes").Value = oTemp.Fields.Item("U_Z_Notes").Value
            oUserTable.UserFields.Fields.Item("U_Z_offTool").Value = "N"
            If oUserTable.Add() <> 0 Then
                oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                Return False
            Else
                orec1.DoQuery("Update [@Z_HR_EXPCL] set U_Z_PayPosted='Y' where Code='" & aCode & "'")
            End If
        End If
        oApplication.Utilities.Message("Operation completed successfully", SAPbouiCOM.BoStatusBarMessageType.smt_Success)

    End Function

    Public Sub ViewHistory(ByVal aForm As SAPbouiCOM.Form, ByVal DocType As modVariables.HistoryDoctype, ByVal DocNo As String)
        Try
            aForm.Freeze(True)
            oGrid = aForm.Items.Item("1").Specific
            Select Case DocType
                Case HistoryDoctype.ExpCli
                    sQuery = " Select Code,U_Z_EmpID,U_Z_EmpName,U_Z_SubDt,U_Z_Claimdt,U_Z_ExpType,U_Z_Currency,U_Z_CurAmt,U_Z_UsdAmt,U_Z_ReimAmt,U_Z_Attachment,U_Z_AppStatus,U_Z_Client,U_Z_Project,""U_Z_Year"",""U_Z_Month"" "
                    sQuery += " From [@Z_HR_EXPCL] T0 where "
                    sQuery += " Code = '" + DocNo.Trim() + "'  Order by Code desc"
                    oGrid.DataTable.ExecuteQuery(sQuery)
                    formatDocument(aForm, HeaderDoctype.ExpCli, DocType)
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
                Case HistoryDoctype.TraReq
                    sQuery = " Select T0.DocEntry,U_Z_EmpId,U_Z_EmpName,U_Z_DocDate,U_Z_TraName,U_Z_TraStLoc,U_Z_TraEdLoc,U_Z_TraStDate,U_Z_TraEndDate,U_Z_AppStatus "
                    sQuery += " From [@Z_HR_OTRAREQ] T0 where "
                    sQuery += "  T0.DocEntry = '" + DocNo.Trim() + "' Order by T0.DocEntry desc "
                    oGrid.DataTable.ExecuteQuery(sQuery)
                    formatDocument(aForm, HeaderDoctype.TraReq, DocType)
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
                Case HistoryDoctype.RegTra
                    sQuery = "  select T0.Code,U_Z_HREmpID,U_Z_HREmpName,U_Z_TrainCode,U_Z_CourseCode,U_Z_CourseName,U_Z_CourseTypeDesc,U_Z_Startdt,U_Z_Enddt"
                    sQuery += " ,U_Z_AppStatus from [@Z_HR_TRIN1] T0 where T0.Code='" + DocNo.Trim() + "' Order by T0.Code desc"
                    oGrid.DataTable.ExecuteQuery(sQuery)
                    formatDocument(aForm, HeaderDoctype.Train, DocType)
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
                    oGrid.Columns.Item("RowsHeader").Click(0, False, False)
                Case HistoryDoctype.NewTra
                    sQuery = "select T0.DocEntry,U_Z_ReqDate,U_Z_HREmpID,U_Z_HREmpName,U_Z_CourseName,U_Z_CourseDetails,convert(varchar(10),U_Z_TrainFrdt,103) as U_Z_TrainFrdt,convert(varchar(10),U_Z_TrainTodt,103) as U_Z_TrainTodt,U_Z_TrainCost,U_Z_Notes"
                    sQuery += " ,U_Z_AppStatus from [@Z_HR_ONTREQ] T0 where "
                    sQuery += "  T0.DocEntry = '" + DocNo.Trim() + "' Order by T0.DocEntry desc "
                    oGrid.DataTable.ExecuteQuery(sQuery)
                    formatDocument(aForm, HeaderDoctype.Train, DocType)
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
                    oGrid.Columns.Item("RowsHeader").Click(0, False, False)
                Case HistoryDoctype.Rec
                    sQuery = " Select T0.DocEntry,U_Z_ReqDate,U_Z_EmpCode,U_Z_EmpName,U_Z_DeptCode,U_Z_DeptName,ISNULL(U_Z_PosName, '') as U_Z_PosName,U_Z_ExpMin,U_Z_ExpMax,U_Z_Vacancy,U_Z_AppStatus"
                    sQuery += " From [@Z_HR_ORMPREQ] T0 where "
                    sQuery += "  T0.DocEntry = '" + DocNo.Trim() + "' Order by T0.DocEntry desc "
                    oGrid.DataTable.ExecuteQuery(sQuery)
                    formatDocument(aForm, HeaderDoctype.Rec, DocType)
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
                    oGrid.Columns.Item("RowsHeader").Click(0, False, False)
                Case HistoryDoctype.EmpPro
                    sQuery = " Select ""Code"",""U_Z_EmpId"",""U_Z_FirstName"",U_Z_Dept,""U_Z_DeptName"",""U_Z_PosName"",""U_Z_JobName"",""U_Z_OrgName"",""U_Z_ProJoinDate"",""U_Z_IncAmount"",""U_Z_EffFromdt"",""U_Z_EffTodt"",""U_Z_AppStatus"""
                    sQuery += " From ""@Z_HR_HEM2""  T0 where "
                    sQuery += "  T0.U_Z_EmpId = '" + DocNo.Trim() + "' Order by T0.Code desc "
                    oGrid.DataTable.ExecuteQuery(sQuery)
                    formatDocument(aForm, HeaderDoctype.EmpLife, DocType)
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
                    oGrid.Columns.Item("RowsHeader").Click(0, False, False)
                Case HistoryDoctype.EmpPos
                    sQuery = " select ""Code"",""U_Z_EmpId"",""U_Z_FirstName"",U_Z_Dept,""U_Z_DeptName"",""U_Z_PosCode"",""U_Z_PosName"",""U_Z_JobName"",""U_Z_OrgCode"",""U_Z_OrgName"","
                    sQuery += """U_Z_NewPosDate"",""U_Z_EffFromdt"",""U_Z_EffTodt"",""U_Z_AppStatus"" from ""@Z_HR_HEM4""  T0 where "
                    sQuery += "  T0.U_Z_EmpId = '" + DocNo.Trim() + "' Order by T0.Code desc "
                    oGrid.DataTable.ExecuteQuery(sQuery)
                    formatDocument(aForm, HeaderDoctype.EmpLife, DocType)
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
                    oGrid.Columns.Item("RowsHeader").Click(0, False, False)
                Case HistoryDoctype.LveReq
                    sQuery = "Select T0.""Code"" as ""Code"",""U_Z_EMPID"",""U_Z_EMPNAME"",""U_Z_TrnsCode"",convert(varchar(10),"
                    sQuery += " ""U_Z_StartDate"",103) AS ""U_Z_StartDate"",convert(varchar(10),""U_Z_EndDate"",103) AS ""U_Z_EndDate"" ,"
                    sQuery += " T0.""U_Z_NoofDays"",""U_Z_Notes"",convert(varchar(10),""U_Z_ReJoiNDate"",103) AS ""U_Z_ReJoiNDate"",""U_Z_Status"",""U_Z_Year"",""U_Z_Month"""
                    sQuery += " from ""@Z_PAY_OLETRANS1"" T0 where  T0.""Code""='" & DocNo.Trim() & "'"
                    oGrid.DataTable.ExecuteQuery(sQuery)
                    formatDocument(aForm, HeaderDoctype.LveReq, DocType)
                    oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
                    oGrid.Columns.Item("RowsHeader").Click(0, False, False)
            End Select
            aForm.Freeze(False)
        Catch ex As Exception
            Throw ex
            aForm.Freeze(False)
        End Try
    End Sub
    Public Sub LoadViewHistory(ByVal aForm As SAPbouiCOM.Form, ByVal enDocType As modVariables.HistoryDoctype, ByVal strDocEntry As String)
        Try
            aForm.Freeze(True)
            oGrid = aForm.Items.Item("3").Specific
            Select Case enDocType
                Case HistoryDoctype.TraReq, HistoryDoctype.RegTra, HistoryDoctype.NewTra, HistoryDoctype.Rec, HistoryDoctype.EmpPro, HistoryDoctype.EmpPos, HistoryDoctype.Final
                    sQuery = " Select DocEntry,U_Z_DocEntry,U_Z_DocType,U_Z_EmpId,U_Z_EmpName,U_Z_ApproveBy,CreateDate ,CreateTime,UpdateDate,UpdateTime,U_Z_AppStatus,U_Z_Remarks From [@Z_HR_APHIS] "
                    sQuery += " Where U_Z_DocType = '" + enDocType.ToString() + "'"
                    sQuery += " And U_Z_DocEntry = '" + strDocEntry + "'"
                Case HistoryDoctype.ExpCli, HistoryDoctype.LveReq
                    sQuery = " Select DocEntry,U_Z_DocEntry,U_Z_DocType,U_Z_EmpId,U_Z_EmpName,U_Z_ApproveBy,CreateDate ,CreateTime,UpdateDate,UpdateTime,U_Z_AppStatus,U_Z_Remarks,""U_Z_Year"",""U_Z_Month"" From [@Z_HR_APHIS] "
                    sQuery += " Where U_Z_DocType = '" + enDocType.ToString() + "'"
                    sQuery += " And U_Z_DocEntry = '" + strDocEntry + "'"
            End Select
            oGrid.DataTable.ExecuteQuery(sQuery)
            formatHistory(aForm, enDocType)
            oApplication.Utilities.assignMatrixLineno(oGrid, aForm)
            aForm.Freeze(False)
        Catch ex As Exception
            aForm.Freeze(False)
            Throw ex
        End Try
    End Sub
    Public Sub InitialMessage(ByVal strReqType As String, ByVal strReqNo As String, ByVal strAppStatus As String _
            , ByVal strTemplateNo As String, ByVal strOrginator As String, ByVal enDocType As modVariables.HistoryDoctype)
        Try
            Dim strQuery As String
            Dim strMessageUser As String
            Dim oRecordSet, oTemp As SAPbobsCOM.Recordset
            Dim oCmpSrv As SAPbobsCOM.CompanyService
            Dim oMessageService As SAPbobsCOM.MessagesService
            Dim oMessage As SAPbobsCOM.Message
            Dim pMessageDataColumns As SAPbobsCOM.MessageDataColumns
            Dim pMessageDataColumn As SAPbobsCOM.MessageDataColumn
            Dim oLines As SAPbobsCOM.MessageDataLines
            Dim oLine As SAPbobsCOM.MessageDataLine
            Dim oRecipientCollection As SAPbobsCOM.RecipientCollection
            oCmpSrv = oApplication.Company.GetCompanyService()
            oMessageService = oCmpSrv.GetBusinessService(SAPbobsCOM.ServiceTypes.MessagesService)
            oMessage = oMessageService.GetDataInterface(SAPbobsCOM.MessagesServiceDataInterfaces.msdiMessage)
            oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oTemp = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            strQuery = "Select Top 1 U_Z_AUser From [@Z_HR_APPT2] Where DocEntry = '" + strTemplateNo + "'  and isnull(U_Z_AMan,'')='Y' Order By LineId Asc "
            oRecordSet.DoQuery(strQuery)
            If Not oRecordSet.EoF Then
                strMessageUser = oRecordSet.Fields.Item(0).Value
                oMessage.Subject = strReqType + ":" + "Need Your Approval "
                oMessage.Text = strReqType + ":" + strReqNo + " Originated by " + strOrginator + " Need Your Approval "

                oRecipientCollection = oMessage.RecipientCollection
                oRecipientCollection.Add()
                oRecipientCollection.Item(0).SendInternal = SAPbobsCOM.BoYesNoEnum.tYES
                oRecipientCollection.Item(0).UserCode = strMessageUser
                pMessageDataColumns = oMessage.MessageDataColumns

                pMessageDataColumn = pMessageDataColumns.Add()
                pMessageDataColumn.ColumnName = "Request No"
                oLines = pMessageDataColumn.MessageDataLines()
                oLine = oLines.Add()
                oLine.Value = strReqNo

                oMessageService.SendMessage(oMessage)

                Select Case enDocType
                    Case HistoryDoctype.LveReq
                        strQuery = "Update [@Z_PAY_OLETRANS1] set U_Z_CurApprover='" & strMessageUser & "',U_Z_NxtApprover='" & strMessageUser & "' where Code='" & strReqNo & "'"
                End Select
                oTemp.DoQuery(strQuery)
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub SendMessage(ByVal strReqType As String, ByVal strReqNo As String, ByVal strAppStatus As String _
        , ByVal strTemplateNo As String, ByVal strOrginator As String, ByVal strAuthorizer As String)
        Try
            Dim strQuery As String
            Dim strMessageUser As String
            Dim intLineID As Integer
            Dim oRecordSet As SAPbobsCOM.Recordset
            Dim oCmpSrv As SAPbobsCOM.CompanyService
            Dim oMessageService As SAPbobsCOM.MessagesService
            Dim oMessage As SAPbobsCOM.Message
            Dim pMessageDataColumns As SAPbobsCOM.MessageDataColumns
            Dim pMessageDataColumn As SAPbobsCOM.MessageDataColumn
            Dim oLines As SAPbobsCOM.MessageDataLines
            Dim oLine As SAPbobsCOM.MessageDataLine
            Dim oRecipientCollection As SAPbobsCOM.RecipientCollection
            oCmpSrv = oApplication.Company.GetCompanyService()
            oMessageService = oCmpSrv.GetBusinessService(SAPbobsCOM.ServiceTypes.MessagesService)
            oMessage = oMessageService.GetDataInterface(SAPbobsCOM.MessagesServiceDataInterfaces.msdiMessage)
            oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            strQuery = "Select LineId From [@Z_HR_APPT2] Where DocEntry = '" & strTemplateNo & "' And U_Z_AUser = '" & strAuthorizer & "'"
            oRecordSet.DoQuery(strQuery)

            If Not oRecordSet.EoF Then
                intLineID = CInt(oRecordSet.Fields.Item(0).Value)
                strQuery = "Select Top 1 U_Z_AUser From [@Z_HR_APPT2] Where  DocEntry = '" & strTemplateNo & "' And LineId > '" & intLineID.ToString() & "' and isnull(U_Z_AMan,'')='Y'  Order By LineId Asc "
                oRecordSet.DoQuery(strQuery)
                If Not oRecordSet.EoF Then
                    strMessageUser = oRecordSet.Fields.Item(0).Value
                    oMessage.Subject = strReqType & ":" & " Need Your Approval "
                    oMessage.Text = strReqType & ":" & strReqNo & " Originated by " & strOrginator & " Need Your Approval "
                    oRecipientCollection = oMessage.RecipientCollection
                    oRecipientCollection.Add()
                    oRecipientCollection.Item(0).SendInternal = SAPbobsCOM.BoYesNoEnum.tYES
                    oRecipientCollection.Item(0).UserCode = strMessageUser
                    pMessageDataColumns = oMessage.MessageDataColumns

                    pMessageDataColumn = pMessageDataColumns.Add()
                    pMessageDataColumn.ColumnName = "Request No"
                    oLines = pMessageDataColumn.MessageDataLines()
                    oLine = oLines.Add()
                    oLine.Value = strReqNo
                    oMessageService.SendMessage(oMessage)

                End If
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub


End Class
