Public Module modVariables

    Public oApplication As clsListener
    Public strSQL As String
    Public cfl_Text As String
    Public cfl_Btn As String
    Public frmSourceMatrix As SAPbouiCOM.Matrix
    Public CompanyDecimalSeprator As String
    Public CompanyThousandSeprator As String
    Public strCardCode As String = ""
    Public EntryChoice As String = ""
    Public blnDraft As Boolean = False
    Public blnError As Boolean = False
    Public blnSourceForm As Boolean = False
    Public intNumofCount As Integer = 0
    Public strDocEntry As String
    Public intSelectedMatrixrow As Integer = 0
    Public strSourceformEmpID As String = ""
    Public strApprovalType As String = ""
    Public frmSourceForm As SAPbouiCOM.Form
    Public ManagerName As String = ""
    Public ManagerId As String = ""
    Public ManageDocType As String = ""
    Public LocalCurrency As String
    Public SystemCurrency As String
    Public enDocType As String = ""
    Public Enum ValidationResult As Integer
        CANCEL = 0
        OK = 1
    End Enum
    Public Enum DocumentType As Integer
        RENTAL_QUOTATION = 1
        RENTAL_ORDER
        RENTAL_RETURN
    End Enum

    Public Enum HeaderDoctype
        Train
        Rec
        EmpLife
        TraReq
        ExpCli
        LveReq
    End Enum
    Public Enum HistoryDoctype
        ExpCli
        RegTra
        NewTra
        Rec
        AppShort
        EmpPro
        EmpPos
        TraReq
        Final
        LveReq
    End Enum
    Public Const mnu_HRVariableEarning As String = "Z_Mnu_HRVEARNING"
    Public Const frm_HRVariableEarning As String = "frm_HR_VariableEarning"
    Public Const xml_HRVariableEarning As String = "frm_HR_VariableEarning.xml"


    Public Const mnu_HR_LveApproval As String = "mnu_HR_LveApproval"
    Public Const xml_hr_LeaveApproval As String = "frm_hr_LeaveApproval.xml"
    Public Const frm_hr_LeaveApproval As String = "frm_hr_LeaveApproval"

    Public Const mnu_HR_LveRequest As String = "mnu_HR_LveRequest"
    Public Const frm_hr_LveRequest As String = "frm_hr_LveRequest"
    Public Const xml_hr_LveRequest As String = "frm_hr_LveRequest.xml"

    Public Const mnu_hr_SheduleTrain As String = "mnu_HR_SheduleTrain"

    Public Const mnu_hr_IPHOD As String = "Z_mnu_HR_IPHOD"
    Public Const frm_hr_FinalApproval As String = "frm_hr_FinalApproval"
    Public Const xml_hr_FinalApproval As String = "frm_hr_FinalApproval.xml"

    Public Const mnu_hr_ASLMApproval As String = "Z_mnu_HR_ASLMApproval"
    Public Const xml_hr_ShortApproval As String = "frm_hr_ShortApproval.xml"
    Public Const frm_hr_ShortApproval As String = "frm_hr_ShortApproval"

    Public Const mnu_hr_RecHRApproval As String = "Z_mnu_HR_MPHrApproval"
    Public Const xml_hr_MPRApproval As String = "frm_hr_MPRApproval.xml"
    Public Const frm_hr_MPRApproval As String = "frm_hr_MPRApproval"

    Public Const frm_hr_TrainNewApproval As String = "frm_hr_TrNewApproval"
    Public Const xml_hr_TrainNewApproval As String = "frm_hr_TrainNewApproval.xml"

    Public Const frm_hr_TrainRegApproval As String = "frm_hr_TrRegApproval"
    Public Const xml_hr_TrainRegApproval As String = "frm_hr_TrainRegApproval.xml"

    Public Const frm_hr_ViewTraApp As String = "frm_hr_ViewTraApp"
    Public Const xml_hr_ViewTraApp As String = "frm_hr_ViewTraApp.xml"

    Public Const frm_hr_AppHisDetails As String = "frm_hr_AppHisDetails"
    Public Const xml_hr_AppHisDetails As String = "frm_hr_AppHisDetails.xml"

    Public Const xml_hr_TranApproval As String = "frm_hr_ClaimApproval.xml"
    Public Const frm_hr_ClaimApproval As String = "frm_hr_ClaimApproval"

    Public Const xml_hr_DocType As String = "frm_hr_DocType.xml"
    Public Const frm_hr_DocType As String = "frm_hr_DocType"
    Public Const mnu_hr_DocType As String = "mnu_hr_DocType"

    Public Const xml_hr_ApproveTemp As String = "frm_hr_ApproveTemp.xml"
    Public Const frm_hr_ApproveTemp As String = "frm_hr_ApproveTemp"
    Public Const mnu_hr_ApproveTemp As String = "mnu_hr_AppTemp"

    Public Const frm_DisRule As String = "frm_DisRule"
    Public Const xml_DisRule As String = "frm_DisRule.xml"

    Public Const xml_hr_PayMethod As String = "frm_hr_PayMethod.xml"
    Public Const frm_hr_PayMethod As String = "frm_hr_PayMethod"
    Public Const mnu_hr_PayMethod As String = "mnu_hr_PayMethod"

    Public Const xml_hr_LeaveMaster As String = "frm_hr_LeaveMaster.xml"
    Public Const frm_hr_LeaveMaster As String = "frm_hr_LeaveMaster"
    Public Const mnu_hr_LeaveMaster As String = "mnu_Hr_LeaveMa"

    Public Const mnu_hr_trainEvaluation As String = "Z_mnu_HR_TraEva"
    Public Const xml_hr_TrainEvaluation As String = "frm_hr_TrainEval.xml"
    Public Const frm_hr_TrainEva As String = "frm_hr_TrainEva"

    Public Const mnu_hr_MgrEvaluation As String = "Z_mnu_HR_MgrEva"
    Public Const xml_hr_MgrEvaluation As String = "frm_hr_MgrEva.xml"
    Public Const frm_hr_MgrEva As String = "frm_hr_MgrEva"

    Public Const xml_HR_TrainQCCate As String = "frm_hr_TrainQcCa.xml"
    Public Const frm_hr_TrainQcCa As String = "frm_Hr_TrainQCCA"
    Public Const mnu_hr_TrainQcCate As String = "Z_mnu_TraQcCa"

    Public Const xml_HR_TrainQCItem As String = "frm_hr_TrainQcIT.xml"
    Public Const frm_hr_TrainQcItem As String = "frm_Hr_TrainQCIT"
    Public Const mnu_hr_TrainQcItem As String = "Z_mnu_TraQcIt"

    Public Const xml_HR_TrainQCRA As String = "frm_hr_TrainQcRA.xml"
    Public Const frm_hr_TrainQcRA As String = "frm_Hr_TrainQCRA"
    Public Const mnu_hr_TrainQcRA As String = "Z_mnu_TraQcRA"



    Public Const xml_hr_CReqSelectionSe As String = "frm_hr_CReqSelectionSe.xml"
    Public Const frm_hr_CReqSelectionSe As String = "frm_hr_CReqSelectiSe"
    Public Const xml_hr_CReqSelIPLM As String = "frm_hr_CReqSelectionIPLM.xml"
    Public Const frm_hr_CReqSelIPLM As String = "frm_hr_CReqSelIPLM"
    Public Const xml_hr_CReqSelIPHOD As String = "frm_hr_CReqSelectionIPHOD.xml"
    Public Const frm_hr_CReqSelIPHOD As String = "frm_hr_CReqSelIPHOD"
    Public Const xml_hr_CReqSelIPSUM As String = "frm_hr_CReqSelectionIPSUM.xml"
    Public Const frm_hr_CReqSelIPSUM As String = "frm_hr_CReqSelIPSUM"
    Public Const xml_hr_CReqSelIPHR As String = "frm_hr_CReqSelectionIPHR.xml"
    Public Const frm_hr_CReqSelIPHR As String = "frm_hr_CReqSelIPHR"
    Public Const xml_hr_CReqSelIPGA As String = "frm_hr_IPGAcceptance.xml"
    Public Const frm_hr_CReqSelIPGA As String = "frm_hr_CReqSelIPGA"
    Public Const xml_hr_SlctnCreteriaGA As String = "frm_hr_SlctnCreteriaGA.xml"
    Public Const frm_hr_SlctnCrGA As String = "frm_hr_SlctnCrGA"


    Public Const frm_hr_FApproval As String = "frm_hr_FApproval"
    Public Const xml_hr_FApproval As String = "frm_hr_FApproval.xml"


    Public Const frm_hr_EmpPosChApp As String = "frm_hr_EmpPosChApp"
    Public Const xml_HR_EmpPosChApp As String = "frm_hr_EmpPosChApp.xml"
    Public Const mnu_HR_EmpLifeposch As String = "Z_mnu_HR_EmpLifePosCh"

    Public Const frm_hr_EmpLifeApp As String = "frm_hr_EmpLifeApp"
    Public Const xml_HR_EmpLifeApp As String = "frm_hr_EmpLifeApp.xml"
    Public Const mnu_HR_EmpLifeApp As String = "Z_mnu_HR_EmpLifeApp"

    Public Const frm_hr_EmpLifePost As String = "frm_hr_EmpLifePost"
    Public Const xml_HR_EmpLifePost As String = "frm_hr_EmpLifePost.xml"
    Public Const mnu_HR_EmpLifePost As String = "Z_mnu_HR_EmpLifePost"

    Public Const frm_HR_UpdatePayroll As String = "frm_HR_UpdatePayroll"
    Public Const xml_HR_UpdatePayroll As String = "xml_HR_UpdatePayroll.xml"
    Public Const mnu_HR_UpdatePayroll As String = "Z_mnu_HR_UpdatePayroll"

    Public Const frm_hr_RecReqReason As String = "frm_hr_RecReqReason"
    Public Const mnu_hr_RecReqReason As String = "z_mnu_hr_RecReqReason"
    Public Const xml_hr_RecReqReason As String = "frm_hr_RecReqReason.xml"

    Public Const frm_hr_AppDisMaster As String = "frm_hr_AppDisMaster"
    Public Const mnu_hr_AppDisMaster As String = "z_mnu_hr_ObjResult"
    Public Const xml_hr_AppDisMaster As String = "frm_hr_AppDisMaster.xml"

    Public Const frm_hr_ExitResponse As String = "frm_hr_ExitResponse"
    Public Const mnu_hr_ExitResponse As String = "z_mnu_hr_ExitRes"
    Public Const xml_hr_ExitResponse As String = "frm_hr_ExitResponse.xml"

    Public Const frm_hr_ExitInvForm1 As String = "frm_hr_ExitInvForm1"
    Public Const mnu_hr_ExitInvForm As String = "Z_mnu_HR_ExitInvForm"
    Public Const xml_hr_ExitInvForm As String = "frm_hr_ExitInvForm.xml"

    Public Const frm_hr_Qustionaire As String = "frm_hr_Qustionaire"
    Public Const mnu_hr_Qustionaire As String = "z_mnu_hr_QusMaster"
    Public Const xml_hr_Qustionaire As String = "frm_hr_Qustionaire.xml"

    Public Const frm_hr_ExitfrmInit As String = "frm_hr_ExitfrmInit"
    Public Const mnu_hr_ExitfrmInit As String = "Z_mnu_HR_ExitFormIni"
    Public Const xml_hr_ExitfrmInit As String = "frm_hr_ExitfrmInit.xml"

    Public Const frm_hr_ExitProcess As String = "frm_hr_ExitProcess"
    Public Const mnu_hr_ExitProcess As String = "Z_mnu_HR_ExitFormPro"
    Public Const xml_hr_ExitProcess As String = "frm_hr_ExitProcess.xml"


    Public Const frm_HR_Trainner As String = "frm_HR_Trainner"
    Public Const mnu_hr_Trainner As String = "Z_mnu_HR_Trainner"
    Public Const xml_hr_Trainner As String = "frm_HR_Trainner.xml"

    Public Const frm_hr_ObjLoan As String = "frm_hr_ObjLoan"
    Public Const mnu_hr_ObjLoan As String = "z_mnu_hr_OLoan"
    Public Const xml_hr_ObjLoan As String = "frm_hr_ObjLoan.xml"


    Public Const frm_hr_Languages1 As String = "frm_hr_Languages1"
    Public Const mnu_hr_Languages As String = "z_mnu_hr_Lang"
    Public Const xml_hr_Languages As String = "frm_hr_Languages.xml"


    Public Const mnu_hr_MgrRegTrainApproval As String = "Z_mnu_HR_MgrRegTrainApp"
    Public Const xml_hr_MgrRegTrainApproval As String = "frm_hr_MgrRegTrainApproval.xml"
    Public Const frm_hr_MgrRegTrainApproval As String = "frm_hr_MgrRegTrainApproval"

    Public Const mnu_hr_HRRegTrainApproval As String = "Z_mnu_HR_HRRegTrainApp"
    Public Const xml_hr_HRRegTrainApproval As String = "frm_hr_HRRegTrainApproval.xml"
    Public Const frm_hr_HRRegTrainApproval As String = "frm_hr_HRRegTrainApproval"

    Public Const xml_hr_EmpAbsSummary As String = "frm_hr_EmpAbsSummary.xml"
    Public Const frm_hr_EmpAbsSummary As String = "frm_hr_EmpAbsSummary"

    Public Const frm_hr_CompObj As String = "frm_hr_CompObj"
    Public Const xml_hr_CompObj As String = "frm_hr_CompObj.xml"


    Public Const frm_hr_CompObjmaster As String = "frm_hr_CompObjmaster"
    Public Const mnu_hr_CompObj As String = "z_mnu_hr_CompObj"
    Public Const xml_hr_CompObjmaster As String = "frm_hr_CompObjmaster.xml"

    Public Const frm_hr_MgrTrainApp As String = "frm_hr_MgrTrainApp"
    Public Const mnu_MgrTrainApp As String = "Z_mnu_HR_MgrTrainApp"
    Public Const xml_MgrTrainApp As String = "frm_hr_MgrTrainApp.xml"

    Public Const frm_hr_HRTrainApp As String = "frm_hr_HRTrainApp"
    Public Const mnu_HRTrainApp As String = "Z_mnu_HR_HRTrainApp"
    Public Const xml_HRTrainApp As String = "frm_hr_HRTrainApp.xml"

    Public Const frm_hr_NewTrainReq As String = "frm_hr_NewTrainReq"
    Public Const xml_hr_NewTrainReq As String = "frm_hr_NewTrainReq.xml"



    Public Const frm_ChoosefromList As String = "frm_CFL"
    Public Const frm_WAREHOUSES As Integer = 62
    Public Const frm_Department As String = "frm_Department"
    Public Const mnu_DeptMaster As String = "z_mnu_hr_Dept"
    Public Const xml_DeptMaster As String = "xml_hr_DeptMaster.xml"

    Public Const frm_Branches As String = "frm_Branches"
    Public Const mnu_BranchMaster As String = "z_mnu_hr_Branch"
    Public Const xml_BranchMaster As String = "frm_hr_BranchMaster.xml"

    Public Const frm_hr_CourseType As String = "frm_hr_CourseType"
    Public Const mnu_hr_CourseType As String = "z_mnu_hr_courseType"
    Public Const xml_hr_CourseType As String = "frm_hr_CourseType.xml"

    Public Const frm_hr_CourseCategory As String = "frm_hr_CourseCategory"
    Public Const mnu_hr_CourseCategory As String = "z_mnu_hr_courseCategory"
    Public Const xml_hr_CourseCategory As String = "frm_hr_CourseCategory.xml"

    Public Const frm_hr_TrainReg As String = "frm_hr_TrainReg"
    Public Const mnu_hr_TrainReg As String = "Z_mnu_HR_TrainingReg"
    Public Const xml_hr_TrainReg As String = "frm_hr_TrainReg.xml"

    Public Const frm_hr_AppAttendees As String = "frm_hr_AppAttendees"
    Public Const mnu_hr_AppAttendees As String = "Z_mnu_HR_AppAttendees"
    Public Const xml_hr_AppAttendees As String = "frm_hr_AppAttendees.xml"


    Public Const frm_hr_Comp As String = "frm_hr_Comp"
    Public Const mnu_hr_Comp As String = "z_mnu_hr_comp"
    Public Const xml_hr_Comp As String = "frm_hr_Company.xml"
    Public Const frm_hr_Func As String = "frm_hr_Func"
    Public Const mnu_hr_Func As String = "z_mnu_hr_Func"
    Public Const xml_hr_Func As String = "frm_hr_Function.xml"
    Public Const frm_hr_Unit As String = "frm_hr_Unit"
    Public Const mnu_hr_Unit As String = "z_mnu_hr_Unit"
    Public Const xml_hr_Unit As String = "frm_hr_Unit.xml"
    Public Const frm_hr_Loc As String = "frm_hr_Loc"
    Public Const mnu_hr_Loc As String = "z_mnu_hr_Loc"
    Public Const xml_hr_Loc As String = "frm_hr_Location.xml"
    Public Const frm_hr_OrgSt As String = "frm_hr_OrgSt"
    Public Const mnu_hr_OrgSt As String = "z_mnu_hr_OrgSt"
    Public Const xml_hr_OrgSt As String = "frm_hr_OrgStructure.xml"
    Public Const frm_hr_GrdLvl As String = "frm_hr_Grade"
    Public Const mnu_hr_GrdLvl As String = "z_mnu_hr_Grade"
    Public Const xml_hr_GrdLvl As String = "frm_hr_GradeLevel.xml"
    Public Const frm_hr_Level As String = "frm_hr_Level"
    Public Const mnu_hr_Level As String = "z_mnu_hr_Level"
    Public Const xml_hr_Level As String = "frm_hr_Level.xml"
    Public Const frm_hr_Allow As String = "frm_hr_Allow"
    Public Const mnu_hr_Allow As String = "z_mnu_hr_Allow"
    Public Const xml_hr_Allow As String = "frm_hr_Allowance.xml"
    Public Const frm_hr_Benefit As String = "frm_hr_Benefit"
    Public Const mnu_hr_Benefit As String = "z_mnu_hr_Benefit"
    Public Const xml_hr_Benefit As String = "frm_hr_Benefits.xml"
    Public Const frm_hr_SalStru As String = "frm_hr_SalStru"
    Public Const mnu_hr_SalStru As String = "z_mnu_hr_SalStru"
    Public Const xml_hr_SalStru As String = "frm_hr_SalStructure.xml"
    Public Const frm_hr_Ratings As String = "frm_hr_Ratings"
    Public Const mnu_hr_Ratings As String = "z_mnu_hr_Ratings"
    Public Const xml_hr_Ratings As String = "frm_hr_Ratings.xml"
    Public Const frm_hr_BussObj As String = "frm_hr_BussObj"
    Public Const mnu_hr_BussObj As String = "z_mnu_hr_BussObj"
    Public Const xml_hr_BussObj As String = "frm_hr_BussObjective.xml"
    Public Const frm_hr_PeoObj As String = "frm_hr_PeoObj"
    Public Const mnu_hr_PeoObj As String = "z_mnu_hr_PeoObj"
    Public Const xml_hr_PeoObj As String = "frm_hr_PeopleObj.xml"
    'Public Const frm_hr_CompObj As String = "frm_hr_CompObj"
    'Public Const mnu_hr_CompObj As String = "z_mnu_hr_CompObj"
    'Public Const xml_hr_CompObj As String = "frm_hr_CompObjective.xml"
    Public Const frm_hr_PosComp As String = "frm_hr_PosComp"
    Public Const mnu_hr_PosComp As String = "z_mnu_hr_PosComp"
    Public Const xml_hr_PosComp As String = "frm_hr_PosCompetenc.xml"

    Public Const frm_hr_TraPlan As String = "frm_hr_TraPlan"
    Public Const mnu_hr_TraPlan As String = "z_mnu_hr_TraPlan"
    Public Const xml_hr_TraPlan As String = "frm_hr_TravelPlan.xml"
    Public Const frm_hr_Course As String = "frm_hr_Course"
    Public Const mnu_hr_Course As String = "z_mnu_hr_course"
    Public Const xml_hr_Course As String = "frm_hr_Course.xml"
    Public Const frm_hr_TrainPlan1 As String = "frm_hr_TrainPlan1"
    Public Const mnu_hr_TrainPlan As String = "z_mnu_hr_TrnPlan"
    Public Const xml_hr_TrainPlan As String = "frm_hr_TrainPlan.xml"
    Public Const frm_hr_PeoCategory As String = "frm_hr_PeoCategory"
    Public Const mnu_hr_PeoCategory As String = "z_mnu_hr_PeoCategory"
    Public Const xml_hr_PeoCategory As String = "frm_hr_PeoCategory.xml"
    Public Const frm_hr_CompLevel As String = "frm_hr_CompLvl"
    Public Const mnu_hr_CompLevel As String = "z_mnu_hr_CompLvl"
    Public Const xml_hr_CompLevel As String = "frm_hr_CompLvl.xml"
    Public Const frm_hr_DeptMapp As String = "frm_hr_DeptMapp"
    Public Const mnu_hr_DeptMapp As String = "z_mnu_hr_DeptMapp"
    Public Const xml_hr_DeptMapp As String = "frm_hr_DeptMapp.xml"
    Public Const frm_hr_LogSetup As String = "frm_hr_LogSetup"
    Public Const mnu_hr_Logsetup As String = "z_mnu_hr_Login"
    Public Const xml_hr_Logsetup As String = "frm_hr_LoginSetup.xml"
    Public Const frm_hr_SelfAppraisal As String = "frm_hr_SelfAppraisal"
    Public Const mnu_hr_SelfAppr As String = "z_mnu_hr_SelfApp"
    Public Const xml_hr_SelfAppr As String = "frm_hr_SelfAppraisal.xml"
    Public Const mnu_hr_MgrAppr As String = "z_mnu_hr_MgrApp"
    Public Const mnu_hr_SMgrAppr As String = "z_mnu_hr_SMgr_App"
    Public Const mnu_hr_HRAppr As String = "z_mnu_hr_HRApp"
    Public Const frm_hr_Login As String = "frm_hr_Login"
    Public Const xml_hr_Login As String = "frm_hr_Login.xml"
    Public Const frm_hr_Approval As String = "frm_hr_Approval"
     Public Const frm_hr_SApproval As String = "frm_hr_SApproval"
    Public Const xml_hr_SApproval As String = "frm_hr_SApproval.xml"
    Public Const xml_hr_Approval As String = "frm_hr_Approval.xml"
    Public Const frm_hr_EmpTraining As String = "frm_hr_EmpTraining"
    Public Const xml_hr_EmpTraining As String = "frm_hr_EmpTraining.xml"
    Public Const frm_hr_CourseRev As String = "frm_hr_CourseRev"
    Public Const mnu_hr_CourseRev As String = "Z_mnu_HR_CourseRev"
    Public Const xml_hr_CourseRev As String = "frm_hr_CourseRev.xml"

    Public Const frm_hr_Position As String = "frm_hr_Position"
    Public Const mnu_hr_Position As String = "z_mnu_hr_Position1"
    Public Const xml_hr_Position As String = "frm_hr_Position.xml"

    Public Const frm_hr_empPosition As String = "frm_hr_EmpPosition"
    Public Const mnu_hr_empPosition As String = "z_mnu_hr_Position"
    Public Const xml_hr_empPosition As String = "frm_hr_empPosition.xml"

    Public Const frm_hr_GAcceptance As String = "frm_hr_GAcceptance"
    Public Const mnu_hr_GAcceptance As String = "z_mnu_hr_GAcceptance"
    Public Const xml_hr_GAcceptance As String = "frm_hr_GAcceptance.xml"

    Public Const frm_hr_SlctnCreteria As String = "frm_hr_SlctnCreteria"
    Public Const xml_hr_SlctnCreteria As String = "frm_hr_SlctnCreteria.xml"

    Public Const mnu_hr_EmpMaster As String = "3590"
    Public Const frm_hr_EmpMaster As String = "60100"

    Public Const frm_Hr_MPRequest As String = "frm_Hr_MPRequest"
    Public Const mnu_hr_MPRequest As String = "Z_mnu_HR_MPRequest"
    Public Const xml_hr_MPRequest As String = "frm_Hr_MPRequest.xml"

    Public Const frm_HR_CrtApplicants1 As String = "frm_HR_CrtApplicant1"
    Public Const mnu_hr_CrApplicants As String = "Z_mnu_HR_CrApplicants"
    Public Const xml_hr_CrApplicants As String = "frm_HR_CrApplicants.xml"

    Public Const mnu_hr_RecGMApproval As String = "Z_mnu_HR_MPGMApproval"
    Public Const frm_hr_RecApproval As String = "frm_hr_RecApproval"
    Public Const xml_hr_RecApproval As String = "frm_hr_RecApproval.xml"

    Public Const frm_hr_HRecApproval As String = "frm_hr_HRecApproval"
    Public Const xml_hr_hRecApproval As String = "frm_hr_hRecApproval.xml"

    Public Const frm_HR_Search1 As String = "frm_HR_Search1"
    Public Const mnu_hr_Search As String = "Z_mnu_HR_Search"
    Public Const xml_hr_Search As String = "frm_Hr_Search.xml"

    Public Const frm_hr_Candidate As String = "frm_hr_Candidate"
    Public Const xml_hr_Candidate As String = "frm_hr_Candidates.xml"

    ''2013-07-06

    Public Const frm_hr_HireToEmp As String = "frm_hr_HireToEmp"
    Public Const mnu_hr_Hiring As String = "Z_mnu_HR_Hiring"
    Public Const xml_hr_HireToEmp As String = "frm_hr_HireToEmp.xml"
    Public Const frm_HR_Hiring As String = "frm_HR_Hiring"
    Public Const xml_hr_Hiring As String = "frm_hr_Hiring.xml"

    Public Const frm_hr_Promotion As String = "frm_hr_Promotion"
    Public Const mnu_hr_Promotion1 As String = "Z_mnu_HR_Promotion"
    Public Const xml_hr_Promotion1 As String = "frm_hr_Promotion.xml"

    Public Const frm_hr_Transfer As String = "frm_hr_Transfer"
    Public Const mnu_hr_Transfer As String = "Z_mnu_HR_PlaTransfer"
    Public Const xml_hr_Transfer As String = "frm_hr_Transfer.xml"

    Public Const frm_hr_PosChanges As String = "frm_hr_PosChanges"
    Public Const mnu_hr_PosChanges As String = "Z_mnu_HR_PosChange"
    Public Const xml_hr_PosChanges As String = "frm_hr_PosChanges.xml"

    Public Const frm_hr_ViewEmpDetails As String = "frm_hr_ViewEmpDetails"
    Public Const xml_hr_ViewEmpDetails As String = "frm_hr_ViewEmpDetails.xml"

    '2013-07-12
    Public Const frm_hr_Expenses As String = "frm_hr_Expenses"
    Public Const mnu_hr_Expenses As String = "z_mnu_hr_Expenses"
    Public Const xml_hr_Expenses As String = "frm_hr_Expenses.xml"

    Public Const frm_hr_TraAgenda As String = "frm_hr_TraAgenda"
    Public Const mnu_hr_TraAgenda As String = "z_mnu_hr_TraAgenda"
    Public Const xml_hr_TraAgenda As String = "frm_hr_TraAgenda.xml"

    Public Const frm_hr_TraRequest As String = "frm_hr_TraRequest"
    Public Const mnu_hr_TraRequest As String = "Z_mnu_HR_TraRequest"
    Public Const xml_hr_TraRequest As String = "frm_hr_TraRequest.xml"

    Public Const frm_hr_TravelApproval As String = "frm_hr_TraApproval"
    Public Const mnu_hr_TraApproval As String = "Z_mnu_HR_TraApproval"
    Public Const xml_hr_TraApproval As String = "frm_hr_TravelApproval.xml"

    Public Const frm_hr_ExpenseClaim As String = "frm_hr_ExpClaim" '"frm_hr_ExpenseClaim"
    Public Const mnu_hr_ExpenseClaim As String = "Z_mnu_HR_ExpClaim"
    Public Const xml_hr_ExpenseClaim As String = "frm_hr_ExpClaimReq.xml" '"frm_hr_ExpenseClaim.xml"

    Public Const frm_hr_ExpApproval As String = "frm_hr_ExpApproval"
    Public Const mnu_hr_ExpApproval As String = "Z_mnu_HR_ExpApproval"
    Public Const xml_hr_ExpApproval As String = "frm_hr_ExpApproval.xml"

    Public Const frm_hr_AssignTraPlan As String = "frm_hr_AssignTraPlan"
    Public Const xml_hr_AssignTraPlan As String = "frm_hr_AssignTraPlan.xml"

    Public Const frm_hr_AssExpenses As String = "frm_hr_AssExpenses"
    Public Const xml_hr_AssExpenses As String = "frm_hr_AssExpenses.xml"

    Public Const frm_hr_TraExpOverView As String = "frm_hr_TraExpOverView"
    Public Const mnu_hr_TraOverview As String = "Z_mnu_HR_TraOverview"
    Public Const xml_hr_TraExpOverView As String = "frm_hr_TraExpOverView.xml"
    Public Const mnu_hr_ExpOverview As String = "Z_mnu_HR_ExpOverview"

    Public Const frm_hr_IniAppraisal As String = "frm_hr_IniAppraisal"
    Public Const mnu_hr_IniAppraisal As String = "z_mnu_hr_IniApp"
    Public Const xml_hr_IniAppraisal As String = "frm_hr_IniAppraisal.xml"

    Public Const frm_StockRequest As String = "frm_StRequest"
    Public Const frm_InvSO As String = "frm_InvSO"
    Public Const frm_Warehouse As String = "62"
    Public Const frm_SalesOrder As String = "139"
    Public Const frm_Invoice As String = "133"

    Public Const mnu_FIND As String = "1281"
    Public Const mnu_ADD As String = "1282"
    Public Const mnu_CLOSE As String = "1286"
    Public Const mnu_NEXT As String = "1288"
    Public Const mnu_PREVIOUS As String = "1289"
    Public Const mnu_FIRST As String = "1290"
    Public Const mnu_LAST As String = "1291"
    Public Const mnu_ADD_ROW As String = "1292"
    Public Const mnu_DELETE_ROW As String = "1293"
    Public Const mnu_TAX_GROUP_SETUP As String = "8458"
    Public Const mnu_DEFINE_ALTERNATIVE_ITEMS As String = "11531"
    Public Const mnu_CloseOrderLines As String = "DABT_910"
    Public Const mnu_InvSO As String = "DABT_911"

    Public Const xml_MENU As String = "Menu.xml"
    Public Const xml_MENU_REMOVE As String = "RemoveMenus.xml"
    Public Const xml_StRequest As String = "StRequest.xml"
    Public Const xml_InvSO As String = "frm_InvSO.xml"



    Public Const mnu_hr_ASSMApproval As String = "Z_mnu_HR_ASSMApproval"
    Public Const mnu_hr_OAcceptance As String = "Z_mnu_HR_IPOA"

    Public Const xml_hr_CReqSelection As String = "frm_hr_CReqSelection.xml"
    Public Const frm_hr_CReqSelection As String = "frm_hr_CReqSelection"

    Public Const xml_hr_AppShortListed As String = "frm_hr_AppShortListed.xml"
    Public Const frm_hr_AppShortListed As String = "frm_hr_AppShortListed"

    Public Const mnu_hr_IType As String = "Z_mnu_HR_IPMasSetUp"

    Public Const mnu_hr_IPLM As String = "Z_mnu_HR_IPLM"

    Public Const mnu_hr_IPHR As String = "Z_mnu_HR_IPHR"

    Public Const xml_hr_IPProcessForm As String = "frm_hr_IPProcessForm.xml"
    Public Const frm_hr_IPProcessForm As String = "frm_hr_IPProcessForm"

    Public Const mnu_hr_InterviewType As String = "z_mnu_hr_IntType"
    Public Const xml_hr_InterviewType As String = "frm_hr_InterviewType.xml"
    Public Const frm_hr_InterviewType As String = "frm_hr_InterviewType"
    Public Const mnu_hr_IPLMU As String = "Z_mnu_HR_IPLMU"

    Public Const mnu_hr_RejectionMaster As String = "z_mnu_hr_Rejection"
    Public Const xml_hr_RejectionMaster As String = "frm_hr_RejectionMaster.xml"
    Public Const frm_hr_RejectionMaster As String = "frm_hr_RejectionMaster"

    Public Const frm_hr_IntRatings As String = "frm_hr_IRatings"
    Public Const mnu_hr_IntRatings As String = "z_mnu_hr_IRating"
    Public Const xml_hr_IntRatings As String = "frm_hr_IRatings.xml"

    Public Const mnu_hr_ORejectionMaster As String = "z_mnu_hr_ORejection"
    Public Const xml_hr_ORejectionMaster As String = "frm_hr_ORejectionMaster.xml"
    Public Const frm_hr_ORejectionMaster As String = "frm_hr_ORejectionMaster"

    Public Const mnu_hr_RecClosing As String = "Z_mnu_HR_RecClosing"
    Public Const xml_hr_RecClosing As String = "frm_hr_RecClosing.xml"
    Public Const frm_hr_RecClosing As String = "frm_hr_RecClosing"

    Public Const mnu_hr_RecOverview As String = "Z_mnu_HR_RecOverview"
    Public Const xml_hr_RecOverview As String = "frm_hr_RecOverview.xml"
    Public Const frm_hr_RecOverview As String = "frm_hr_RecOverview"


    Public Const xml_hr_IPOfferAcceptance As String = "frm_hr_IPOfferAcceptance.xml"
    Public Const frm_hr_IPOfferAcceptance As String = "frm_hr_IPOfferAcceptance"

    Public Const mnu_hr_EmailSetUp As String = "z_mnu_hr_EmailSetUp"
    Public Const xml_hr_EmailSetUp As String = "frm_hr_EmailSetUp.xml"
    Public Const frm_hr_EmailSetUp As String = "frm_hr_EmailSetUp"

    Public Const frm_hr_AppEmail As String = "frm_hr_AppEmail"
    Public Const mnu_hr_AppraisalEmail As String = "z_mnu_hr_AppraisalEmail"
    Public Const xml_hr_AppraisalEmail As String = "frm_hr_AppraisalEmail.xml"

    Public Const frm_hr_Sec As String = "frm_hr_Sec"
    Public Const mnu_hr_Sec As String = "z_mnu_hr_Sec"
    Public Const xml_hr_Sec As String = "frm_hr_Section.xml"

    Public Const frm_hr_RSta As String = "frm_hr_RSta"
    Public Const mnu_hr_RSta As String = "z_mnu_hr_RSta"
    Public Const xml_hr_RSta As String = "frm_hr_ResidencyStatus.xml"

End Module
