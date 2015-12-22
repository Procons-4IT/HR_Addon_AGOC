Public NotInheritable Class clsTable

#Region "Private Functions"
    '*************************************************************************************************************
    'Type               : Private Function
    'Name               : AddTables
    'Parameter          : 
    'Return Value       : none
    'Author             : Manu
    'Created Dt         : 
    'Last Modified By   : 
    'Modified Dt        : 
    'Purpose            : Generic Function for adding all Tables in DB. This function shall be called by 
    '                     public functions to create a table
    '**************************************************************************************************************
    Private Sub AddTables(ByVal strTab As String, ByVal strDesc As String, ByVal nType As SAPbobsCOM.BoUTBTableType)
        Dim oUserTablesMD As SAPbobsCOM.UserTablesMD
        Try
            oUserTablesMD = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables)
            'Adding Table
            If Not oUserTablesMD.GetByKey(strTab) Then
                oUserTablesMD.TableName = strTab
                oUserTablesMD.TableDescription = strDesc
                oUserTablesMD.TableType = nType
                If oUserTablesMD.Add <> 0 Then
                    Throw New Exception(oApplication.Company.GetLastErrorDescription)
                End If
            End If
        Catch ex As Exception
            Throw ex
        Finally
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserTablesMD)
            oUserTablesMD = Nothing
            GC.WaitForPendingFinalizers()
            GC.Collect()
        End Try
    End Sub

    '*************************************************************************************************************
    'Type               : Private Function
    'Name               : AddFields
    'Parameter          : SstrTab As String,strCol As String,
    '                     strDesc As String,nType As Integer,i,nEditSize,nSubType As Integer
    'Return Value       : none
    'Author             : Manu
    'Created Dt         : 
    'Last Modified By   : 
    'Modified Dt        : 
    'Purpose            : Generic Function for adding all Fields in DB Tables. This function shall be called by 
    '                     public functions to create a Field
    '**************************************************************************************************************
    Private Sub AddFields(ByVal strTab As String, _
                            ByVal strCol As String, _
                                ByVal strDesc As String, _
                                    ByVal nType As SAPbobsCOM.BoFieldTypes, _
                                        Optional ByVal i As Integer = 0, _
                                            Optional ByVal nEditSize As Integer = 10, _
                                                Optional ByVal nSubType As SAPbobsCOM.BoFldSubTypes = 0, _
                                                    Optional ByVal Mandatory As SAPbobsCOM.BoYesNoEnum = SAPbobsCOM.BoYesNoEnum.tNO)
        Dim oUserFieldMD As SAPbobsCOM.UserFieldsMD
        Try

            If Not (strTab = "OCRY" Or strTab = "OUBR" Or strTab = "OADM" Or strTab = "OUDP" Or strTab = "OUBR" Or strTab = "OITT" Or strTab = "OHPS" Or strTab = "OITM" Or strTab = "INV1" Or strTab = "RDR1" Or strTab = "OINV" Or strTab = "OHEM" Or strTab = "OCLG") Then
                strTab = "@" + strTab
            End If
           
            If Not IsColumnExists(strTab, strCol) Then
                oUserFieldMD = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields)

                oUserFieldMD.Description = strDesc
                oUserFieldMD.Name = strCol
                oUserFieldMD.Type = nType
                oUserFieldMD.SubType = nSubType
                oUserFieldMD.TableName = strTab
                oUserFieldMD.EditSize = nEditSize
                oUserFieldMD.Mandatory = Mandatory
                If oUserFieldMD.Add <> 0 Then
                    Throw New Exception(oApplication.Company.GetLastErrorDescription)
                End If

                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserFieldMD)
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserFieldMD = Nothing
            GC.WaitForPendingFinalizers()
            GC.Collect()
        End Try
    End Sub

    Public Sub addField(ByVal TableName As String, ByVal ColumnName As String, ByVal ColDescription As String, ByVal FieldType As SAPbobsCOM.BoFieldTypes, ByVal Size As Integer, ByVal SubType As SAPbobsCOM.BoFldSubTypes, ByVal ValidValues As String, ByVal ValidDescriptions As String, ByVal SetValidValue As String)
        Dim intLoop As Integer
        Dim strValue, strDesc As Array
        Dim objUserFieldMD As SAPbobsCOM.UserFieldsMD
        Try

            strValue = ValidValues.Split(Convert.ToChar(","))
            strDesc = ValidDescriptions.Split(Convert.ToChar(","))
            If (strValue.GetLength(0) <> strDesc.GetLength(0)) Then
                Throw New Exception("Invalid Valid Values")
            End If


            If (Not IsColumnExists(TableName, ColumnName)) Then
                objUserFieldMD = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields)
                objUserFieldMD.TableName = TableName
                objUserFieldMD.Name = ColumnName
                objUserFieldMD.Description = ColDescription
                objUserFieldMD.Type = FieldType
                If (FieldType <> SAPbobsCOM.BoFieldTypes.db_Numeric) Then
                    objUserFieldMD.Size = Size
                Else
                    objUserFieldMD.EditSize = Size
                End If
                objUserFieldMD.SubType = SubType
                objUserFieldMD.DefaultValue = SetValidValue
                For intLoop = 0 To strValue.GetLength(0) - 1
                    objUserFieldMD.ValidValues.Value = strValue(intLoop)
                    objUserFieldMD.ValidValues.Description = strDesc(intLoop)
                    objUserFieldMD.ValidValues.Add()
                Next
                If (objUserFieldMD.Add() <> 0) Then
                    MsgBox(oApplication.Company.GetLastErrorDescription)
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserFieldMD)
            Else
            End If

        Catch ex As Exception
            MsgBox(ex.Message)

        Finally
            objUserFieldMD = Nothing
            GC.WaitForPendingFinalizers()
            GC.Collect()

        End Try


    End Sub

    '*************************************************************************************************************
    'Type               : Private Function
    'Name               : IsColumnExists
    'Parameter          : ByVal Table As String, ByVal Column As String
    'Return Value       : Boolean
    'Author             : Manu
    'Created Dt         : 
    'Last Modified By   : 
    'Modified Dt        : 
    'Purpose            : Function to check if the Column already exists in Table
    '**************************************************************************************************************
    Private Function IsColumnExists(ByVal Table As String, ByVal Column As String) As Boolean
        Dim oRecordSet As SAPbobsCOM.Recordset

        Try
            strSQL = "SELECT COUNT(*) FROM CUFD WHERE TableID = '" & Table & "' AND AliasID = '" & Column & "'"
            oRecordSet = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRecordSet.DoQuery(strSQL)

            If oRecordSet.Fields.Item(0).Value = 0 Then
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oRecordSet = Nothing
            GC.Collect()
        End Try
    End Function

    Private Sub AddKey(ByVal strTab As String, ByVal strColumn As String, ByVal strKey As String, ByVal i As Integer)
        Dim oUserKeysMD As SAPbobsCOM.UserKeysMD

        Try
            '// The meta-data object must be initialized with a
            '// regular UserKeys object
            oUserKeysMD = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserKeys)

            If Not oUserKeysMD.GetByKey("@" & strTab, i) Then

                '// Set the table name and the key name
                oUserKeysMD.TableName = strTab
                oUserKeysMD.KeyName = strKey

                '// Set the column's alias
                oUserKeysMD.Elements.ColumnAlias = strColumn
                oUserKeysMD.Elements.Add()
                oUserKeysMD.Elements.ColumnAlias = "RentFac"

                '// Determine whether the key is unique or not
                oUserKeysMD.Unique = SAPbobsCOM.BoYesNoEnum.tYES

                '// Add the key
                If oUserKeysMD.Add <> 0 Then
                    Throw New Exception(oApplication.Company.GetLastErrorDescription)
                End If

            End If

        Catch ex As Exception
            Throw ex

        Finally
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserKeysMD)
            oUserKeysMD = Nothing
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Try

    End Sub

    '********************************************************************
    'Type		            :   Function    
    'Name               	:	AddUDO
    'Parameter          	:   
    'Return Value       	:	Boolean
    'Author             	:	
    'Created Date       	:	
    'Last Modified By	    :	
    'Modified Date        	:	
    'Purpose             	:	To Add a UDO for Transaction Tables
    '********************************************************************
    Private Sub AddUDO(ByVal strUDO As String, ByVal strDesc As String, ByVal strTable As String, _
                               Optional ByVal sFind1 As String = "", Optional ByVal sFind2 As String = "", _
                                       Optional ByVal strChildTbl As String = "", Optional ByVal strChildTb2 As String = "", _
                                       Optional ByVal strChildTb3 As String = "", Optional ByVal strChildTb4 As String = "", Optional ByVal strChildTb5 As String = "", Optional ByVal strChildTb6 As String = "", Optional ByVal nObjectType As SAPbobsCOM.BoUDOObjType = SAPbobsCOM.BoUDOObjType.boud_Document, Optional ByVal blnCanArchive As Boolean = False, Optional ByVal strLogName As String = "")

        Dim oUserObjectMD As SAPbobsCOM.UserObjectsMD
        Try
            oUserObjectMD = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjectMD.GetByKey(strUDO) = 0 Then
                oUserObjectMD.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjectMD.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjectMD.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjectMD.CanFind = SAPbobsCOM.BoYesNoEnum.tYES

                If sFind1 <> "" And sFind2 <> "" Then
                    oUserObjectMD.FindColumns.ColumnAlias = sFind1
                    oUserObjectMD.FindColumns.Add()
                    oUserObjectMD.FindColumns.SetCurrentLine(1)
                    oUserObjectMD.FindColumns.ColumnAlias = sFind2
                    oUserObjectMD.FindColumns.Add()
                End If

                oUserObjectMD.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjectMD.LogTableName = ""
                oUserObjectMD.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjectMD.ExtensionName = ""

                Dim intTables As Integer = 0
                If strChildTbl <> "" Then
                    oUserObjectMD.ChildTables.TableName = strChildTbl
                End If
                If strChildTb2 <> "" Then
                    If strChildTbl <> "" Then
                        oUserObjectMD.ChildTables.Add()
                        intTables = intTables + 1
                    End If
                    oUserObjectMD.ChildTables.SetCurrentLine(intTables)
                    oUserObjectMD.ChildTables.TableName = strChildTb2
                End If
                If strChildTb3 <> "" Then
                    If strChildTb2 <> "" Then
                        oUserObjectMD.ChildTables.Add()
                        intTables = intTables + 1
                    End If
                    oUserObjectMD.ChildTables.SetCurrentLine(intTables)

                    oUserObjectMD.ChildTables.TableName = strChildTb3
                End If
                If strChildTb4 <> "" Then
                    If strChildTb3 <> "" Then
                        oUserObjectMD.ChildTables.Add()
                        intTables = intTables + 1
                    End If
                    oUserObjectMD.ChildTables.SetCurrentLine(intTables)
                    oUserObjectMD.ChildTables.TableName = strChildTb4
                End If
                If strChildTb5 <> "" Then
                    If strChildTb4 <> "" Then
                        oUserObjectMD.ChildTables.Add()
                        intTables = intTables + 1
                    End If
                    oUserObjectMD.ChildTables.SetCurrentLine(intTables)
                    oUserObjectMD.ChildTables.TableName = strChildTb5
                End If
                If strChildTb6 <> "" Then
                    If strChildTb5 <> "" Then
                        oUserObjectMD.ChildTables.Add()
                        intTables = intTables + 1
                    End If
                    oUserObjectMD.ChildTables.SetCurrentLine(intTables)
                    oUserObjectMD.ChildTables.TableName = strChildTb6
                End If
                oUserObjectMD.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjectMD.Code = strUDO
                oUserObjectMD.Name = strDesc
                oUserObjectMD.ObjectType = nObjectType
                oUserObjectMD.TableName = strTable


                If blnCanArchive Then
                    oUserObjectMD.CanLog = SAPbobsCOM.BoYesNoEnum.tYES
                    oUserObjectMD.LogTableName = strLogName
                End If

                If oUserObjectMD.Add() <> 0 Then
                    Throw New Exception(oApplication.Company.GetLastErrorDescription)
                End If
            End If

        Catch ex As Exception
            Throw ex
        Finally
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjectMD)
            oUserObjectMD = Nothing
            GC.WaitForPendingFinalizers()
            GC.Collect()
        End Try

    End Sub
    Public Function UDOIntRatings(ByVal strUDO As String, _
                        ByVal strDesc As String, _
                            ByVal strTable As String, _
                                ByVal intFind As Integer, _
                                    Optional ByVal strCode As String = "", _
                                        Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_RateCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_RateCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_RateName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_RateName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function
    Public Function UDOOfferRejectionMaster(ByVal strUDO As String, _
                     ByVal strDesc As String, _
                         ByVal strTable As String, _
                             ByVal intFind As Integer, _
                                 Optional ByVal strCode As String = "", _
                                     Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_TypeCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_TypeCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_TypeName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_TypeName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function
    Public Function UDORecruitmentRequestReason(ByVal strUDO As String, _
                       ByVal strDesc As String, _
                           ByVal strTable As String, _
                               ByVal intFind As Integer, _
                                   Optional ByVal strCode As String = "", _
                                       Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_ReasonCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_ReasonCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_ReasonName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_ReasonName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function

    Public Function UDOCompany(ByVal strUDO As String, _
                           ByVal strDesc As String, _
                               ByVal strTable As String, _
                                   ByVal intFind As Integer, _
                                       Optional ByVal strCode As String = "", _
                                           Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_CompCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_CompCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_CompName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_CompName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function
    Public Function UDOCourseType(ByVal strUDO As String, _
                        ByVal strDesc As String, _
                            ByVal strTable As String, _
                                ByVal intFind As Integer, _
                                    Optional ByVal strCode As String = "", _
                                        Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_CouTypeCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_CouTypeCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_CouTypeDesc"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_CouTypeDesc"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function
    Public Function UDOCompetenceObj(ByVal strUDO As String, _
       ByVal strDesc As String, _
           ByVal strTable As String, _
               ByVal intFind As Integer, _
                   Optional ByVal strCode As String = "", _
                       Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_CompCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_CompCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_CompName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_CompName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_CompLevel"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_CompLevel"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Weight"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Weight"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function
    Public Function UDOLanguages(ByVal strUDO As String, _
                         ByVal strDesc As String, _
                             ByVal strTable As String, _
                                 ByVal intFind As Integer, _
                                     Optional ByVal strCode As String = "", _
                                         Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_LanCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_LanCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_LanName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_LanName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function
    Public Function UDOResponsibilities(ByVal strUDO As String, _
      ByVal strDesc As String, _
          ByVal strTable As String, _
              ByVal intFind As Integer, _
                  Optional ByVal strCode As String = "", _
                      Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_DeptCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_DeptCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_DeptName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_DeptName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_PosCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_PosCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_PosName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_PosName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_ResCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_ResCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_ResDesc"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_ResDesc"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function
    Public Function UDOQustinnaries(ByVal strUDO As String, _
                       ByVal strDesc As String, _
                           ByVal strTable As String, _
                               ByVal intFind As Integer, _
                                   Optional ByVal strCode As String = "", _
                                       Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_QusCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_QusCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_QusName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_QusName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function
    Public Function UDOCourseCategory(ByVal strUDO As String, _
                        ByVal strDesc As String, _
                            ByVal strTable As String, _
                                ByVal intFind As Integer, _
                                    Optional ByVal strCode As String = "", _
                                        Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_CouCatCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_CouCatCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_CouCatDesc"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_CouCatDesc"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function
    Public Function UDOFunction(ByVal strUDO As String, _
                        ByVal strDesc As String, _
                            ByVal strTable As String, _
                                ByVal intFind As Integer, _
                                    Optional ByVal strCode As String = "", _
                                        Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_FuncCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_FuncCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_FuncName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_FuncName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function
    Public Function UDOSection(ByVal strUDO As String, _
                        ByVal strDesc As String, _
                            ByVal strTable As String, _
                                ByVal intFind As Integer, _
                                    Optional ByVal strCode As String = "", _
                                        Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_SecCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_SecCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_SecName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_SecName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function

    Public Function UDOResidency(ByVal strUDO As String, _
                        ByVal strDesc As String, _
                            ByVal strTable As String, _
                                ByVal intFind As Integer, _
                                    Optional ByVal strCode As String = "", _
                                        Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_StaCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_StaCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_StaName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_StaName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function
    Public Function UDOUnit(ByVal strUDO As String, _
                      ByVal strDesc As String, _
                          ByVal strTable As String, _
                              ByVal intFind As Integer, _
                                  Optional ByVal strCode As String = "", _
                                      Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_UnitCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_UnitCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_UnitName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_UnitName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function
    Public Function UDORejectionMaster(ByVal strUDO As String, _
                     ByVal strDesc As String, _
                         ByVal strTable As String, _
                             ByVal intFind As Integer, _
                                 Optional ByVal strCode As String = "", _
                                     Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_TypeCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_TypeCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_TypeName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_TypeName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function
    Public Function UDOLocation(ByVal strUDO As String, _
                       ByVal strDesc As String, _
                           ByVal strTable As String, _
                               ByVal intFind As Integer, _
                                   Optional ByVal strCode As String = "", _
                                       Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_LocCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_LocCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_CouName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_CouName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_LocName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_LocName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function

    Public Function UDOGrade(ByVal strUDO As String, _
                     ByVal strDesc As String, _
                         ByVal strTable As String, _
                             ByVal intFind As Integer, _
                                 Optional ByVal strCode As String = "", _
                                     Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_GrdeCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_GrdeCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_GrdeName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_GrdeName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function

    Public Function UDOLevel(ByVal strUDO As String, _
                     ByVal strDesc As String, _
                         ByVal strTable As String, _
                             ByVal intFind As Integer, _
                                 Optional ByVal strCode As String = "", _
                                     Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_LvelCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_LvelCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_LvelName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_LvelName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function

    Public Function UDOExpances(ByVal strUDO As String, _
                        ByVal strDesc As String, _
                            ByVal strTable As String, _
                                ByVal intFind As Integer, _
                                    Optional ByVal strCode As String = "", _
                                        Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_ExpName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_ExpName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()

                oUserObjects.FormColumns.FormColumnAlias = "U_Z_ActCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_ActCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function

    Public Function UDOAllowance(ByVal strUDO As String, _
                    ByVal strDesc As String, _
                        ByVal strTable As String, _
                            ByVal intFind As Integer, _
                                Optional ByVal strCode As String = "", _
                                    Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_AlloCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_AlloCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_AlloName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_AlloName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function

    Public Function UDOBenefits(ByVal strUDO As String, _
                    ByVal strDesc As String, _
                        ByVal strTable As String, _
                            ByVal intFind As Integer, _
                                Optional ByVal strCode As String = "", _
                                    Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_BenefCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_BenefCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_BenefName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_BenefName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function
    Public Function UDORatings(ByVal strUDO As String, _
                        ByVal strDesc As String, _
                            ByVal strTable As String, _
                                ByVal intFind As Integer, _
                                    Optional ByVal strCode As String = "", _
                                        Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_RateCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_RateCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_RateName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_RateName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Total"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Total"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function

    Public Function UDOBussObjective(ByVal strUDO As String, _
                        ByVal strDesc As String, _
                            ByVal strTable As String, _
                                ByVal intFind As Integer, _
                                    Optional ByVal strCode As String = "", _
                                        Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_BussCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_BussCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_BussName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_BussName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Weight"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Weight"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function

    Public Function UDOPeopleCatry(ByVal strUDO As String, _
                   ByVal strDesc As String, _
                       ByVal strTable As String, _
                           ByVal intFind As Integer, _
                               Optional ByVal strCode As String = "", _
                                   Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_CatCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_CatCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_CatName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_CatName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function

    Public Function UDOPeopleObj(ByVal strUDO As String, _
                  ByVal strDesc As String, _
                      ByVal strTable As String, _
                          ByVal intFind As Integer, _
                              Optional ByVal strCode As String = "", _
                                  Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_PeoobjCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_PeoobjCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_PeoobjName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_PeoobjName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_PeoCategory"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_PeoCategory"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Weight"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Weight"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function

    Public Function UDOCompetenceLevel(ByVal strUDO As String, _
                 ByVal strDesc As String, _
                     ByVal strTable As String, _
                         ByVal intFind As Integer, _
                             Optional ByVal strCode As String = "", _
                                 Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_LvelCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_LvelCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_LvelName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_LvelName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function


    Public Function UDOTrainingQuestionCategory(ByVal strUDO As String, _
                 ByVal strDesc As String, _
                     ByVal strTable As String, _
                         ByVal intFind As Integer, _
                             Optional ByVal strCode As String = "", _
                                 Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Code"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Name"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function
    Public Function UDOTrainingQuestionItem(ByVal strUDO As String, _
                 ByVal strDesc As String, _
                     ByVal strTable As String, _
                         ByVal intFind As Integer, _
                             Optional ByVal strCode As String = "", _
                                 Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Code"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Name"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function

    Public Function UDOOrgStructure(ByVal strUDO As String, _
                ByVal strDesc As String, _
                    ByVal strTable As String, _
                        ByVal intFind As Integer, _
                            Optional ByVal strCode As String = "", _
                                Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_OrgCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_OrgCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_CompCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_CompCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_CompName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_CompName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_FuncCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_FuncCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_FuncName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_FuncName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_UnitCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_UnitCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_UnitName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_UnitName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_CouName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_CouName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_LocCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_LocCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_LocName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_LocName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_DeptCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_DeptCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_DeptName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_DeptName"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function

    Public Function UDOTrainPlan(ByVal strUDO As String, _
            ByVal strDesc As String, _
                ByVal strTable As String, _
                    ByVal intFind As Integer, _
                        Optional ByVal strCode As String = "", _
                            Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_TrainCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_TrainCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_CourseCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_CourseCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_CourseName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_CourseName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Venue"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Venue"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Fromdt"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Fromdt"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Todt"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Todt"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_AppStdt"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_AppStdt"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_AppEnddt"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_AppEnddt"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_MinAttendees"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_MinAttendees"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_MaxAttendees"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_MaxAttendees"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Instruct"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Instruct"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function

    Public Function UDOLogin(ByVal strUDO As String, _
                            ByVal strDesc As String, _
                                ByVal strTable As String, _
                                    ByVal intFind As Integer, _
                                        Optional ByVal strCode As String = "", _
                                            Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_UID"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_UID"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_PWD"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_PWD"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_EMPID"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_EMPID"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_EMPNAME"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_EMPNAME"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_SUPERUSER"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_SUPERUSER"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_APPROVER"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_APPROVER"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_MGRAPPROVER"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_MGRAPPROVER"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_HRAPPROVER"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_HRAPPROVER"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function

    Public Function UDOInterviewType(ByVal strUDO As String, _
                      ByVal strDesc As String, _
                          ByVal strTable As String, _
                              ByVal intFind As Integer, _
                                  Optional ByVal strCode As String = "", _
                                      Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_TypeCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_TypeCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_TypeName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_TypeName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function

#End Region

#Region "Public Functions"
    '*************************************************************************************************************
    'Type               : Public Function
    'Name               : CreateTables
    'Parameter          : 
    'Return Value       : none
    'Author             : Manu
    'Created Dt         : 
    'Last Modified By   : 
    'Modified Dt        : 
    'Purpose            : Creating Tables by calling the AddTables & AddFields Functions
    '**************************************************************************************************************   

    Public Sub CreateTables()
        Dim oProgressBar As SAPbouiCOM.ProgressBar
        Try

            oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            '    AddFields("OUBR", "Test", "test Field", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20, SAPbobsCOM.BoFldSubTypes.st_Address)
            AddTables("Z_HR_ORES", "Responsibilities - Setup", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddFields("Z_HR_ORES", "Z_DeptCode", "Department Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_ORES", "Z_DeptName", "Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ORES", "Z_PosCode", "Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_ORES", "Z_PosName", "Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ORES", "Z_ResCode", "Responsibilities Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_ORES", "Z_ResDesc", "Responsibilities Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddTables("Z_HR_OQUS", "Questionnaire - Setup", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddFields("Z_HR_OQUS", "Z_QusCode", "Questionnaire Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OQUS", "Z_QusName", "Questionnaire Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_OQUS", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")


            AddTables("Z_HR_OEXFOM", "Employee Exit Form", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddTables("Z_HR_EXFORM1", "Exit Form Process", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_EXFORM2", "Exit interview Form ", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_EXFORM3", "Attachments", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)

            AddFields("Z_HR_EXFORM2", "Z_QusCode", "Questionnaire Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_EXFORM2", "Z_QusName", "Questionnaire Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_EXFORM2", "Z_Answer", "Questionnaire Answer", SAPbobsCOM.BoFieldTypes.db_Memo)

            AddFields("Z_HR_EXFORM1", "Z_DeptCode", "Department Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_EXFORM1", "Z_DeptName", "Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_EXFORM1", "Z_PosCode", "Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_EXFORM1", "Z_PosName", "Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_EXFORM1", "Z_ResCode", "Responsibilities Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_EXFORM1", "Z_ResDesc", "Responsibilities Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_EXFORM1", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N,A", "Yes,No,Not Applicable", "Y")
            addField("@Z_HR_EXFORM1", "Z_CompStatus", "Completion Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "C,P", "Complete,Pending", "P")
            AddFields("Z_HR_EXFORM1", "Z_Remarks", "Remarks", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_HR_OEXFOM", "Z_empID", "Employee Id", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OEXFOM", "Z_FirstName", "First Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OEXFOM", "Z_MiddleName", "Middle Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OEXFOM", "Z_LastName", "Last Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OEXFOM", "Z_JobTitle", "Job Title", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OEXFOM", "Z_DeptCode", "Department Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OEXFOM", "Z_DeptName", "Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OEXFOM", "Z_PosCode", "Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OEXFOM", "Z_PosName", "Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OEXFOM", "Z_JoinDate", "Joining Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OEXFOM", "Z_ResignDate", "Resignation Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OEXFOM", "Z_LstWrDate", "Last Working Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OEXFOM", "Z_ResReason", "Resignation Reason", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OEXFOM", "Z_InvCode", "Interviewer Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OEXFOM", "Z_InvName", "Interviewer Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OEXFOM", "Z_InvPosCode", "Int.Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OEXFOM", "Z_InvPosName", "Int. Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OEXFOM", "Z_InvComm", "Interviewer Comments", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_HR_OEXFOM", "Z_Status", "Employee Exit Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "O,E", "Open,Close", "O")


            AddFields("Z_HR_EXFORM3", "Z_FileName", "File Name", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_EXFORM3", "Z_AttDate", "Attachment Date", SAPbobsCOM.BoFieldTypes.db_Date)



            AddTables("Z_HR_OLNG", "Language - Setup", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddFields("Z_HR_OLNG", "Z_LanCode", "Language Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OLNG", "Z_LanName", "Language Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_OLNG", "Z_Status", "Language Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            addField("@Z_HR_ORMPREQ", "Z_Gender", "Gender", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "M,F,N", "Male,Female,No Specification", "M")
            AddFields("Z_HR_ORMPREQ", "Z_AgeGroup", "Preferred Age Group", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            addField("@Z_HR_ORMPREQ", "Z_DriLicStatus", "Driving License Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            AddFields("Z_HR_ORMPREQ", "Z_OtherReq", "Other Requirement", SAPbobsCOM.BoFieldTypes.db_Memo)

            AddTables("Z_HR_RMPREQ5", "Requisition Languages", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddFields("Z_HR_RMPREQ5", "Z_LanCode", "Language Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_RMPREQ5", "Z_LanName", "Language Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("OUDP", "Z_HOD", "Head of Department HOD", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            AddFields("OUDP", "Z_FrgnName", "Second Language Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddTables("Z_HR_OCOMP", "Competence Objectives", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddFields("Z_HR_OCOMP", "Z_CompCode", "Competence Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OCOMP", "Z_CompName", "Competence Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OCOMP", "Z_CompLevel", "Competence Level", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OCOMP", "Z_Weight", "Weight", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            addField("@Z_HR_OCOMP", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            ' AddFields("RDR1", "RemQty", "Invoiced Quantity", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Quantity)
            AddTables("Z_HR_OADM", "Company Master-Setup", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddTables("Z_HR_OFCA", "Division - Master", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddTables("Z_HR_OUNT", "Unit - Master", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddTables("Z_HR_OLOC", "Location - Master", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddTables("Z_HR_OGRD", "Grade - Master", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddTables("Z_HR_OLVL", "Level - Master", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddTables("Z_HR_OALLO", "Allowance - Master", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddTables("Z_HR_OBEFI", "Benefits - Master", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddTables("Z_HR_ORATE", "Ratings - Master", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddTables("Z_HR_OBUOB", "Business Objectives Master", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddTables("Z_HR_PECAT", "People Category Master", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddTables("Z_HR_OPEOB", "People Objectives", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddTables("Z_HR_COLVL", "Competence Level - Master", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddTables("Z_HR_ORGST", "Organizational Structure", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddTables("Z_HR_OCOTY", "Course Type - Setup", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddTables("Z_HR_OCOCA", "Course Category - Setup", SAPbobsCOM.BoUTBTableType.bott_MasterData)

            AddFields("Z_HR_OCOTY", "Z_CouTypeCode", "Course Type Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OCOTY", "Z_CouTypeDesc", "Course Type Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_OCOTY", "Z_Status", "Course Type Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddFields("Z_HR_OCOCA", "Z_CouCatCode", "Course Category Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OCOCA", "Z_CouCatDesc", "Course Category Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_OCOCA", "Z_Status", "Course Category Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")


            AddTables("Z_HR_OTRIN", "Training Agenda - Setup", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddTables("Z_HR_OTRIN1", "Training Attachments", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)

            AddFields("Z_HR_OTRIN", "Z_TrainCode", "Training Agenda Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OTRIN", "Z_DocDate", "Training Agenda Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OTRIN", "Z_CourseCode", "Course Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OTRIN", "Z_CourseName", "Course Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OTRIN", "Z_CourseTypeCode", "Course Type Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OTRIN", "Z_CourseTypeDesc", "Course Type Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OTRIN", "Z_Startdt", "Course Start Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OTRIN", "Z_Enddt", "Course End Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OTRIN", "Z_MinAttendees", "No of Minimum Attendees", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_OTRIN", "Z_MaxAttendees", "No of Maximum Attendees", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_OTRIN", "Z_AppStdt", "Application Issue Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OTRIN", "Z_AppEnddt", "Application Deadline Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OTRIN", "Z_InsName", "Instructor Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_OTRIN", "Z_Active", "Training Active", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            AddFields("Z_HR_OTRIN", "Z_NoOfHours", "Total No of Hours", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_HR_OTRIN", "Z_StartTime", "Course Start Time", SAPbobsCOM.BoFieldTypes.db_Date, , , SAPbobsCOM.BoFldSubTypes.st_Time)
            AddFields("Z_HR_OTRIN", "Z_EndTime", "Course End Time", SAPbobsCOM.BoFieldTypes.db_Date, , , SAPbobsCOM.BoFldSubTypes.st_Time)
            addField("@Z_HR_OTRIN", "Z_Sunday", "Sunday", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_OTRIN", "Z_Monday", "Monday", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_OTRIN", "Z_Tuesday", "Tuesday", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_OTRIN", "Z_Wednesday", "Wednesday", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_OTRIN", "Z_Thursday", "Thursday", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_OTRIN", "Z_Friday", "Friday", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_OTRIN", "Z_Saturday", "Saturday", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            AddFields("Z_HR_OTRIN", "Z_AttCost", "Cost of Attendees", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_HR_OTRIN", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "O,C,L", "Open, Closed,Canceled", "O")

            AddFields("Z_HR_OTRIN1", "Z_FileName", "File Name", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OTRIN1", "Z_AttDate", "Attachment Date", SAPbobsCOM.BoFieldTypes.db_Date)



            AddFields("Z_HR_ORGST", "Z_OrgCode", "Organization Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_ORGST", "Z_OrgDesc", "Organization Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ORGST", "Z_CompCode", "Company Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_ORGST", "Z_CompName", "Company Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ORGST", "Z_FuncCode", "Function Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_ORGST", "Z_FuncName", "Function Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ORGST", "Z_UnitCode", "Unit Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_ORGST", "Z_UnitName", "Unit Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ORGST", "Z_CouName", "Country Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ORGST", "Z_LocCode", "Location Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_ORGST", "Z_LocName", "Location Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ORGST", "Z_DeptCode", "Department Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_ORGST", "Z_DeptName", "Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ORGST", "Z_PosCode", "Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_ORGST", "Z_PosName", "Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ORGST", "Z_SecCode", "Section Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_ORGST", "Z_SecName", "Section Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ORGST", "Z_BranCode", "Branch Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_ORGST", "Z_BranName", "Branch Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_HR_OADM", "Z_CompCode", "Company Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OADM", "Z_CompName", "Company Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_HR_OADM", "Z_FrgnName", "Foreign Lanugage Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_OADM", "Z_Status", "Company Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddFields("Z_HR_OFCA", "Z_FuncCode", "Function Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OFCA", "Z_FuncName", "Function Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OFCA", "Z_FrgnName", "Foreign Lanugage Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_OFCA", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddFields("Z_HR_OUNT", "Z_UnitCode", "Unit Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OUNT", "Z_UnitName", "Unit Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OUNT", "Z_FrgnName", "Foreign Lanugage Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_OUNT", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddFields("Z_HR_OLOC", "Z_LocCode", "Location Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OLOC", "Z_CouName", "Country Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OLOC", "Z_LocName", "Location Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OLOC", "Z_FrgnName", "Foreign Lanugage Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_OLOC", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")


            AddFields("OUBR", "Z_FrgnName", "Foreign Lanugage Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_HR_OGRD", "Z_GrdeCode", "Grade Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OGRD", "Z_GrdeName", "Grade Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            addField("@Z_HR_OGRD", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddFields("Z_HR_OLVL", "Z_LvelCode", "Level Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OLVL", "Z_LvelName", "Level Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_OLVL", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            AddFields("Z_HR_OALLO", "Z_AlloCode", "Allowance Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OALLO", "Z_AlloName", "Allowance Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_OALLO", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddFields("Z_HR_OBEFI", "Z_BenefCode", "Benefits Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OBEFI", "Z_BenefName", "Benefits Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_OBEFI", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddFields("Z_HR_ORATE", "Z_RateCode", "Ratings Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_ORATE", "Z_RateName", "Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_ORATE", "Z_Total", "Total", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_ORATE", "Z_RatePerc", "Rating Percentage", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            addField("@Z_HR_ORATE", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddFields("Z_HR_OBUOB", "Z_BussCode", "Business Objective Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OBUOB", "Z_BussName", "Business Objectives", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OBUOB", "Z_Weight", "Weight", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            addField("@Z_HR_OBUOB", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            '  oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            AddFields("Z_HR_PECAT", "Z_CatCode", "Category Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_PECAT", "Z_CatName", "Category Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_PECAT", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddFields("Z_HR_OPEOB", "Z_PeoobjCode", "People Objective Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OPEOB", "Z_PeoobjName", "People Objectives", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OPEOB", "Z_PeoCategory", "People Category", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OPEOB", "Z_Weight", "Weight", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            addField("@Z_HR_OPEOB", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddFields("Z_HR_COLVL", "Z_LvelCode", "Competence Level Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_COLVL", "Z_LvelName", "Competence Level Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_COLVL", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            ' oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)


            AddTables("Z_HR_OCOB", "Competence Objectives Master", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddTables("Z_HR_COB1", "Competence Levels Detail1", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_COB2", "Competence Description Detail2", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)

            AddFields("Z_HR_OCOB", "Z_CompCode", "Competence Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OCOB", "Z_CompName", "Competence Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_HR_COB1", "Z_CompLevel", "Competence Level", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_COB1", "Z_CompWeight", "Competence Weight", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)

            AddFields("Z_HR_COB2", "Z_CompLevel", "Competence Level", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_COB2", "Z_Code", "Competence Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_COB2", "Z_CompDesc", "Competence Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddTables("Z_HR_ODEMA", "Department Mapping", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddTables("Z_HR_DEMA1", "Business Objectives", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)

            AddFields("Z_HR_ODEMA", "Z_DeptCode", "Department Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_ODEMA", "Z_DeptName", "Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_HR_DEMA1", "Z_BussCode", "Business Objective Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_DEMA1", "Z_BussName", "Business Objectives", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_DEMA1", "Z_Weight", "Weight", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)

            ' oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)


            AddTables("Z_HR_OSALST", "Salary Scale - Setup", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddTables("Z_HR_SALST1", "Salary Scale -Allowance ", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_SALST2", "Salary Scale -Benefits", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)

            AddFields("Z_HR_OSALST", "Z_SalCode", "Salary Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OSALST", "Z_LevlCode", "Level Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OSALST", "Z_LevlName", "Level Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OSALST", "Z_GrdeCode", "Grade Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OSALST", "Z_GrdeName", "Grade Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OSALST", "Z_MinBSal", "Min Basic Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_OSALST", "Z_MidBSal", "Mid Basic Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_OSALST", "Z_MaxBSal", "Max Basic Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_OSALST", "Z_MinTotSal", "Min Total Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_OSALST", "Z_MidTotSal", "Mid Total Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_OSALST", "Z_MaxTotSal", "Max Total Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_OSALST", "Z_OtherAll", "Other Allowance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("Z_HR_SALST2", "Z_BeneCode", "Benefits Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_SALST2", "Z_BeneDesc", "Benefits Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_HR_SALST1", "Z_Amount", "Allowance Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_SALST1", "Z_AllCode", "Allowance Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_SALST1", "Z_AllDesc", "Allowance Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            AddTables("Z_HR_OPOSCO", "Employee Position Competence", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddTables("Z_HR_POSCO1", "Position Competence", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_POSCO2", "Course", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_POSCO3", "Major Duties", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_POSCO4", "Min Requirements", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)


            AddFields("Z_HR_OPOSCO", "Z_PosCode", "Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OPOSCO", "Z_PosName", "Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OPOSCO", "Z_LevlCode", "Level Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OPOSCO", "Z_LevlName", "Level Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OPOSCO", "Z_GrdeCode", "Grade Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OPOSCO", "Z_GrdeName", "Grade Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OPOSCO", "Z_DeptCode", "Department Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OPOSCO", "Z_DeptName", "Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OPOSCO", "Z_Gradefull", "Gradefull Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_OPOSCO", "Z_HeadOffice", "Head Office", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            AddFields("Z_HR_OPOSCO", "Z_OrgCode", "Organization Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OPOSCO", "Z_OrgDesc", "Organization Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OPOSCO", "Z_DivCode", "Division Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OPOSCO", "Z_DivDesc", "Division Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OPOSCO", "Z_ReportTo", "Reporting To", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OPOSCO", "Z_RptName", "Reporting to Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OPOSCO", "Z_JobBrief", "Job Brief", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OPOSCO", "Z_WorkCont", "Work Contract", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_HR_OPOSCO", "Z_JobActive", "Job Active", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            AddFields("Z_HR_OPOSCO", "Z_SalCode", "Salary Structure Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20) '''2013-08-05
            AddFields("Z_HR_OPOSCO", "Z_RptJobCode", "Reporting Job Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20) '''2013-08-14

            AddFields("Z_HR_OPOSCO", "Z_CmpCode", "Company Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OPOSCO", "Z_CmpName", "Company Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)

            AddFields("Z_HR_POSCO1", "Z_CompCode", "Competence Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_POSCO1", "Z_CompDesc", "Competence Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_POSCO1", "Z_CompLevel", "Competence Level", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40) '''2013-08-05
            AddFields("Z_HR_POSCO1", "Z_Weight", "Competence Weight", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)

            AddFields("Z_HR_POSCO2", "Z_CourseCode", "Course Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_POSCO2", "Z_CourseName", "Course Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_POSCO3", "Z_MajrDuties", "Major Duties", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_POSCO4", "Z_MinReq", "Min.Requirements", SAPbobsCOM.BoFieldTypes.db_Memo)

            AddFields("OCRY", "Z_FrgnName", "Second Language Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)


            AddFields("OHEM", "Z_HR_CompCode", "Company Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_HR_CompName", "Company Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("OHEM", "Z_HR_OrgstCode", "Organization Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_HR_OrgstName", "Organization Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("OHEM", "Z_HR_JobstCode", "Job Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_HR_JobstName", "Job Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("OHEM", "Z_HR_SalaryCode", "Salary Structure Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_HR_PosiCode", "Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_HR_PosiName", "Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("OHEM", "Z_HR_ThirdName", "Third Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_HR_AFirstName", "Arabic First Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_HR_ASecondName", "Arabic Second Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_HR_AThirdName", "Arabic Third Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_HR_ALastName", "Arabic Last Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_HR_ACmpName", "Arabic Company Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("OHEM", "Z_HR_ADeptName", "Arabic Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)


            AddFields("OHEM", "Z_HR_JoinDate", "Joining Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("OHEM", "Z_HR_ApplId", "Applicant Id", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("OHEM", "Z_HR_PromoCode", "Promotion Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 8)
            AddFields("OHEM", "Z_HR_TransferCode", "Transfer  Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 8)
            AddFields("OHEM", "Z_HR_PosChangeCode", "Position Changes Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 8)
            AddFields("OHEM", "Z_HR_TrasFrom", "Transfer Effective From", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("OHEM", "Z_HR_PosFrom", "Position Change Effective From", SAPbobsCOM.BoFieldTypes.db_Date)
            addField("OHEM", "Z_HR_ISReqAut", "Is Requisition Authorized", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            AddFields("OHEM", "Z_LvlCode", "Level Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_LvlName", "Level Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("OHEM", "Z_GrdCode", "Grade Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_GrdName", "Grade Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("OHEM", "Z_LocCode", "Location Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_LocName", "Location Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("OHEM", "Z_HR_UnitName", "Unit Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_HR_SecName", "Section Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("OHEM", "Z_HR_BraName", "Branch Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OHEM", "Z_HR_DivCode", "Division Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("OHEM", "Z_HR_DivName", "Division Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)


            AddTables("Z_HR_PEOBJ1", "Employee People Objective", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_HR_PEOBJ1", "Z_HREmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_HR_PEOBJ1", "Z_HRPeoobjCode", "People Objective Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_PEOBJ1", "Z_HRPeoobjName", "People Objectives", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_PEOBJ1", "Z_HRPeoCategory", "People Category", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_PEOBJ1", "Z_HRWeight", "Weight", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_HR_PEOBJ1", "Z_MKPI", "Management Criteria(KPI)", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_PEOBJ1", "Z_Remarks", "Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            AddTables("Z_HR_ECOLVL", "Employee Competence Level", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_HR_ECOLVL", "Z_HREmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_HR_ECOLVL", "Z_CompCode", "Competence Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_ECOLVL", "Z_CompName", "Competence Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ECOLVL", "Z_Weight", "Weight", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_HR_ECOLVL", "Z_CompLevel", "Competence Level", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_ECOLVL", "Z_PosCode", "Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddTables("Z_HR_TRIN2", "Training Absense Details", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_HR_TRIN2", "Z_RefCode", "Training Employee RefCode", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_HR_TRIN2", "Z_HREmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_HR_TRIN2", "Z_HREmpName", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_TRIN2", "Z_TrainCode", "Training Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_HR_TRIN2", "Z_DeptCode", "Department Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_TRIN2", "Z_DeptName", "Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            addField("@Z_HR_TRIN2", "Z_Status", "Applicant Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,A,R", "Pending,Approved,Rejected", "P")
            AddFields("Z_HR_TRIN2", "Z_Date", "Training Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_TRIN2", "Z_Hours", "Number of Hours", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            addField("@Z_HR_TRIN2", "Z_AttendeesStatus", "Attendees Training Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "D,C,F", "Dropped,Completed,Failed", "C")
            AddFields("Z_HR_TRIN2", "Z_Remarks", "Remarks", SAPbobsCOM.BoFieldTypes.db_Alpha, , 250)



            AddTables("Z_HR_TRIN1", "Employee Training Schedule", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_HR_TRIN1", "Z_HREmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_HR_TRIN1", "Z_HREmpName", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_TRIN1", "Z_TrainCode", "Training Agenda Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_TRIN1", "Z_DocDate", "Training Agenda Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_TRIN1", "Z_CourseCode", "Course Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_TRIN1", "Z_CourseName", "Course Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_TRIN1", "Z_CourseTypeCode", "Course Type Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_TRIN1", "Z_CourseTypeDesc", "Course Type Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_TRIN1", "Z_Startdt", "Course Start Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_TRIN1", "Z_Enddt", "Course End Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_TRIN1", "Z_MinAttendees", "No of Minimum Attendees", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_TRIN1", "Z_MaxAttendees", "No of Maximum Attendees", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_TRIN1", "Z_AppStdt", "Application Issue Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_TRIN1", "Z_AppEnddt", "Application Deadline Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_TRIN1", "Z_InsName", "Instructor Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_TRIN1", "Z_Active", "Training Active", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            AddFields("Z_HR_TRIN1", "Z_NoOfHours", "Total No of Hours", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_HR_TRIN1", "Z_StartTime", "Course Start Time", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Time)
            AddFields("Z_HR_TRIN1", "Z_EndTime", "Course End Time", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Time)
            addField("@Z_HR_TRIN1", "Z_Sunday", "Sunday", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_TRIN1", "Z_Monday", "Monday", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_TRIN1", "Z_Tuesday", "Tuesday", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_TRIN1", "Z_Wednesday", "Wednesday", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_TRIN1", "Z_Thursday", "Thursday", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_TRIN1", "Z_Friday", "Friday", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_TRIN1", "Z_Saturday", "Saturday", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            AddFields("Z_HR_TRIN1", "Z_AttCost", "Cost of Attendees", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_HR_TRIN1", "Z_Status", "Applicant Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,A,R", "Pending,Approved,Rejected", "P")
            AddFields("Z_HR_TRIN1", "Z_Remarks", "Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_TRIN1", "Z_PosiCode", "Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_TRIN1", "Z_PosiName", "Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_TRIN1", "Z_DeptCode", "Department Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_TRIN1", "Z_DeptName", "Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_TRIN1", "Z_ApplyDate", "Requested Date", SAPbobsCOM.BoFieldTypes.db_Date)
            addField("@Z_HR_TRIN1", "Z_AttendeesStatus", "Attendees Training Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "D,C,F,L,W", "Dropped,completed,Failed,Canceled,WithDraw", "C")
            AddFields("Z_HR_TRIN1", "Z_AddionalCost", "Additional Cost of Attendees", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_TRIN1", "Z_TrainHours", "Training Attened Hours", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_TRIN1", "Z_AbsenceDate", "Absence Date", SAPbobsCOM.BoFieldTypes.db_Date)
            addField("@Z_HR_TRIN1", "Z_UpEmpTrain", "Update Emp. Training Profile", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_TRIN1", "Z_UpEmpComp", "Update Employee Competence", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            AddFields("Z_HR_TRIN1", "Z_TotalCost", "Total Cost", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_TRIN1", "Z_ApyproveRemarks", "Approved Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_TRIN1", "Z_AbsenceRemarks", "Absence Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_TRIN1", "Z_CloseRemarks", "Closing Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_TRIN1", "Z_ClosingDate", "Closing Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_TRIN1", "Z_Closeby", "Closed by", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_TRIN1", "Z_ApproveRemarks", "Approval Remarks", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)


            AddTables("Z_HR_OCOUR", "Course Master", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddTables("Z_HR_COUR1", "Course Buss.Objective", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_COUR2", "Course People Objective", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_COUR3", "Course Comp.Objective", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_COUR4", "Course Emp.Position", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_COUR5", "Course Attachements", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)

            AddFields("Z_HR_OCOUR", "Z_CourseCode", "Course Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OCOUR", "Z_CourseName", "Course Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_OCOUR", "Z_AllPos", "All Position", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            AddFields("Z_HR_OCOUR", "Z_CouCatCode", "Course Category Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OCOUR", "Z_CouCatDesc", "Course Category Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OCOUR", "Z_CourseDetails", "Course Details", SAPbobsCOM.BoFieldTypes.db_Memo)

            AddFields("Z_HR_COUR1", "Z_BussCode", "Business Objective Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_COUR1", "Z_BussDesc", "Business Obj.Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_HR_COUR2", "Z_PeopleCode", "People Objective Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_COUR2", "Z_PeopleDesc", "People Obj.Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_COUR2", "Z_PeopleCat", "People Obj.Category", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_HR_COUR3", "Z_CompCode", "Competence Objective Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_COUR3", "Z_CompDesc", "Competence Obj.Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_COUR3", "Z_CompLevel", "Competence Level", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            AddFields("Z_HR_COUR4", "Z_PosCode", "Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_COUR4", "Z_PosName", "Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_HR_COUR5", "Z_FileName", "File Name", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_COUR5", "Z_AttDate", "Attachment Date", SAPbobsCOM.BoFieldTypes.db_Date)

            AddTables("Z_HR_LOGIN", "Login Details", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddFields("Z_HR_LOGIN", "Z_UID", "User ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_LOGIN", "Z_PWD", "Password", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_LOGIN", "Z_EMPID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_HR_LOGIN", "Z_EMPNAME", "Employee NAME", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_LOGIN", "Z_SUPERUSER", "Superuser", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_LOGIN", "Z_APPROVER", "Self Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_LOGIN", "Z_MGRAPPROVER", "Manager Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_LOGIN", "Z_HRAPPROVER", "HR Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_LOGIN", "Z_MGRREQUEST", "Manager Requisition", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_LOGIN", "Z_HRRECAPPROVER", "HR Rec. Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_LOGIN", "Z_GMRECAPPROVER", "GM Rec. Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddTables("Z_HR_OSEAPP", "Self Appraisal", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddTables("Z_HR_SEAPP1", "Self App.Buss.Objective", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_SEAPP2", "Self App.People Objective", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_SEAPP3", "Self App.Comp.Objective", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_SEAPP4", "Self App.HR.Objective", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)

            AddFields("Z_HR_OSEAPP", "Z_EmpId", "Employee Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OSEAPP", "Z_EmpName", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OSEAPP", "Z_Date", "Document Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OSEAPP", "Z_Period", "Appraisal Period", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_HR_OSEAPP", "Z_LStrt", "Level start From", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)


            addField("@Z_HR_OSEAPP", "Z_Status", "Approval Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "D,F,S,L,C", "Draft,Approved,2nd Level Approval,Closed,Canceled", "D")

            AddFields("Z_HR_OSEAPP", "Z_BSelfRemark", "Business Self Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OSEAPP", "Z_BMgrRemark", "Business Manager Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OSEAPP", "Z_BSMrRemark", "Business Self Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OSEAPP", "Z_BHrRemark", "Business HR Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OSEAPP", "Z_PSelfRemark", "People Self Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OSEAPP", "Z_PMgrRemark", "People Manager Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OSEAPP", "Z_PSMrRemark", "Business Self Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OSEAPP", "Z_PHrRemark", "People HR Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OSEAPP", "Z_CSelfRemark", "Competence Self Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OSEAPP", "Z_CMgrRemark", "Competence Manager Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OSEAPP", "Z_CSMrRemark", "Business Self Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OSEAPP", "Z_CHrRemark", "Competence HR Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)

            addField("@Z_HR_OSEAPP", "Z_Initialize", "Initialize Appraisal", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddFields("Z_HR_SEAPP1", "Z_BussCode", "Business Objective Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_SEAPP1", "Z_BussDesc", "Business Obj.Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_SEAPP1", "Z_BussWeight", "Business Weight", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_HR_SEAPP1", "Z_BussSelfRate", "Business Self Rating", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_HR_SEAPP1", "Z_BussMgrRate", "Business Manager Rating", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_HR_SEAPP1", "Z_BussSMRate", "Business Sr.Manager Rating", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)

            AddFields("Z_HR_SEAPP1", "Z_SelfRaCode", "Self Rating Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_SEAPP1", "Z_MgrRaCode", " Manager Rating Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_SEAPP1", "Z_SMRaCode", " Sr.Manager Rating Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)


            AddFields("Z_HR_SEAPP2", "Z_PeopleCode", "People Objective Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_SEAPP2", "Z_PeopleDesc", "People Obj.Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_SEAPP2", "Z_PeopleCat", "People Obj.Category", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_SEAPP2", "Z_PeoWeight", "People Weight", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_HR_SEAPP2", "Z_PeoSelfRate", "People Self Rating", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_HR_SEAPP2", "Z_PeoMgrRate", "People Manager Rating", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_HR_SEAPP2", "Z_PeoSMRate", "People Sr,Manager Rating", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_HR_SEAPP2", "Z_SelfRaCode", "Self Rating Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_SEAPP2", "Z_MgrRaCode", " Manager Rating Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_SEAPP2", "Z_SMRaCode", " Sr.Manager Rating Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)



            AddFields("Z_HR_SEAPP3", "Z_CompCode", "Self Comp.Objective Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_SEAPP3", "Z_CompDesc", "Self Comp.Obj.Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_SEAPP3", "Z_CompWeight", "Competence Weight", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_HR_SEAPP3", "Z_CompLevel", "Competence Level", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40) '''2013-08-05
            AddFields("Z_HR_SEAPP3", "Z_CompSelfRate", "Competence Self Rating", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_HR_SEAPP3", "Z_CompMgrRate", "Competence Manager Rating", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_HR_SEAPP3", "Z_CompSMRate", "Competence Sr.Manager Rating", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_HR_SEAPP3", "Z_CompSelf", "Competence Self Rating", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_HR_SEAPP3", "Z_CompMgr", "Competence Manager Rating", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_HR_SEAPP3", "Z_CompSM", "Competence Sr.Manager Rating", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_HR_SEAPP3", "Z_SelfRaCode", "Self Rating Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_SEAPP3", "Z_MgrRaCode", " Manager Rating Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_SEAPP3", "Z_SMRaCode", " Sr.Manager Rating Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            AddFields("Z_HR_SEAPP4", "Z_CompType", "Self Comp.Objective Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_SEAPP4", "Z_AvgComp", "Avg Competence Rating", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)
            AddFields("Z_HR_SEAPP4", "Z_HRComp", "Competence HR Rating", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)

            AddTables("Z_HR_OPOSIN", "Position Mapping", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddFields("Z_HR_OPOSIN", "Z_PosCode", "Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OPOSIN", "Z_PosName", "Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OPOSIN", "Z_FrgnName", "Second Language Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OPOSIN", "Z_RptCode", "Reporting To Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OPOSIN", "Z_RptName", "Reporting To Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OPOSIN", "Z_JobCode", "Job Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OPOSIN", "Z_JobName", "Job Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OPOSIN", "Z_OrgCode", "Organization Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OPOSIN", "Z_OrgName", "Organization Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OPOSIN", "Z_DocDate", "Document Date", SAPbobsCOM.BoFieldTypes.db_Date)
            addField("@Z_HR_OPOSIN", "Z_PosActive", "Position Active", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            AddFields("Z_HR_OPOSIN", "Z_SalCode", "Salary Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_HR_OPOSIN", "Z_CurrEmp", "No.of Current Employees", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_OPOSIN", "Z_ExpEmp", "No.of Expected Employees", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_OPOSIN", "Z_VacPosition", "Vacant Positions", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_OPOSIN", "Z_DivCode", "Division Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20) '''2013-08-14
            AddFields("Z_HR_OPOSIN", "Z_DivDesc", "Division Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200) '''2013-08-14
            AddFields("Z_HR_OPOSIN", "Z_CompCode", "Company Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20) '''2013-08-14
            AddFields("Z_HR_OPOSIN", "Z_CompName", "Company Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200) '''2013-08-14
            AddFields("Z_HR_OPOSIN", "Z_DeptCode", "Department Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20) '''2013-08-14
            AddFields("Z_HR_OPOSIN", "Z_DeptName", "Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200) '''2013-08-14
            AddFields("Z_HR_OPOSIN", "Z_RptPCode", "Reporting Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OPOSIN", "Z_RptPName", "Reporting Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)


            AddTables("Z_HR_HEM2", "Employee Promotion", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_HR_HEM2", "Z_EmpId", "Employee Id", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_HEM2", "Z_FirstName", "Employee First Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_HEM2", "Z_LastName", "Employee Last Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_HEM2", "Z_Dept", "Department", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_HEM2", "Z_DeptName", "Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_HEM2", "Z_PosCode", "Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_HEM2", "Z_PosName", "Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_HEM2", "Z_JobCode", "Job Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_HEM2", "Z_JobName", "Job Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_HEM2", "Z_OrgCode", "Organization Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_HEM2", "Z_OrgName", "Organization Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_HEM2", "Z_JoinDate", "Employee Joining Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_HEM2", "Z_SalCode", "Salary Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_HEM2", "Z_ProJoinDate", "Promotion Joining Date", SAPbobsCOM.BoFieldTypes.db_Date)

            AddTables("Z_HR_HEM3", "Employee Transfer", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_HR_HEM3", "Z_EmpId", "Employee Id", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_HEM3", "Z_FirstName", "Employee First Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_HEM3", "Z_LastName", "Employee Last Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_HEM3", "Z_Dept", "Department", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_HEM3", "Z_DeptName", "Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_HEM3", "Z_PosCode", "Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_HEM3", "Z_PosName", "Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_HEM3", "Z_JobCode", "Job Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_HEM3", "Z_JobName", "Job Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_HEM3", "Z_OrgCode", "Organization Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_HEM3", "Z_OrgName", "Organization Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_HEM3", "Z_JoinDate", "Employee Joining Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_HEM3", "Z_SalCode", "Salary Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_HEM3", "Z_TraJoinDate", "Transfer Joining Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_HEM3", "Z_Branch", "Branch Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            '  oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            AddTables("Z_HR_HEM4", "Employee Position Changes", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_HR_HEM4", "Z_EmpId", "Employee Id", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_HEM4", "Z_FirstName", "Employee First Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_HEM4", "Z_LastName", "Employee Last Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_HEM4", "Z_Dept", "Department", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_HEM4", "Z_DeptName", "Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_HEM4", "Z_PosCode", "Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_HEM4", "Z_PosName", "Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_HEM4", "Z_JobCode", "Job Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_HEM4", "Z_JobName", "Job Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_HEM4", "Z_OrgCode", "Organization Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_HEM4", "Z_OrgName", "Organization Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_HEM4", "Z_JoinDate", "old Position Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_HEM4", "Z_SalCode", "Salary Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_HEM4", "Z_NewPosDate", "New Position Date", SAPbobsCOM.BoFieldTypes.db_Date)

            ''2013-07-12
            AddTables("Z_HR_EXPANCES", "Expences Master - Setup", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddFields("Z_HR_EXPANCES", "Z_ExpName", "Expences Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_EXPANCES", "Z_ActCode", "Account Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            addField("@Z_HR_EXPANCES", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddTables("Z_HR_OTRAPL", "Travel Agenda Setup", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddTables("Z_HR_TRAPL1", "Travel Expenses", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)

            AddFields("Z_HR_OTRAPL", "Z_TraCode", "Travel Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OTRAPL", "Z_TraName", "Travel Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_HR_TRAPL1", "Z_ExpName", "Expenses Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_TRAPL1", "Z_ActCode", "Account Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_TRAPL1", "Z_Amount", "Expenses Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            ' oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            AddTables("Z_HR_OASSTP", "Assigned Travel Plan", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddTables("Z_HR_ASSTP1", "Assigned Expenses Details", SAPbobsCOM.BoUTBTableType.bott_NoObject)

            AddFields("Z_HR_OASSTP", "Z_TraCode", "Travel Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OASSTP", "Z_TraName", "Travel Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OASSTP", "Z_EffeFromDt", "Effective From Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OASSTP", "Z_EffeToDt", "Effective To Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OASSTP", "Z_EmpId", "Employee Id", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_OASSTP", "Z_EmpName", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OASSTP", "Z_Dept", "Department", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OASSTP", "Z_DeptName", "Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OASSTP", "Z_PosCode", "Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OASSTP", "Z_PosName", "Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_HR_ASSTP1", "Z_EmpId", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_ASSTP1", "Z_TraCode", "Travel Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_ASSTP1", "Z_ExpName", "Expenses Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_ASSTP1", "Z_ActCode", "Account Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_ASSTP1", "Z_Amount", "Budget Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_ASSTP1", "Z_UtilAmt", "Utilize Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_ASSTP1", "Z_BalAmount", "Balance Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_ASSTP1", "Z_RefCode", "Reference Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)

            '  oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            AddTables("Z_HR_OTRAREQ", "Employee Travel Request", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddTables("Z_HR_TRAREQ1", "Travel Request Expenses", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_TRAREQ2", "Travel Expenses Attachments", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)

            AddFields("Z_HR_OTRAREQ", "Z_DocDate", "Document Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OTRAREQ", "Z_ReqAppDate", "Request Approval Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OTRAREQ", "Z_ReqClaimDate", "Request Claim Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OTRAREQ", "Z_AppClaimDate", "Approved Claim Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OTRAREQ", "Z_TraDocCode", "Travel Document Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OTRAREQ", "Z_TraCode", "Travel Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OTRAREQ", "Z_TraName", "Travel Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OTRAREQ", "Z_EmpId", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_OTRAREQ", "Z_EmpName", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OTRAREQ", "Z_DeptId", "Department ID", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_OTRAREQ", "Z_DeptName", "Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OTRAREQ", "Z_PosCode", "Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OTRAREQ", "Z_PosName", "Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OTRAREQ", "Z_TraStLoc", "Travel Start Location", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OTRAREQ", "Z_TraEdLoc", "Travel End Location", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OTRAREQ", "Z_TraStDate", "Travel Start Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OTRAREQ", "Z_TraEndDate", "Travel End Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OTRAREQ", "Z_EmpComme", "Employee Comments", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_HR_OTRAREQ", "Z_NewReq", "New Trip Request", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_HR_OTRAREQ", "Z_HRComme", "HR Comments", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_HR_OTRAREQ", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 2, SAPbobsCOM.BoFldSubTypes.st_Address, "O,RA,RR,CR,CA,CJ,C", "Open,Request Approved,Request Rejected,Claim Received,Claim Approved,Claim Rejected,Closed", "O")


            AddFields("Z_HR_TRAREQ1", "Z_ExpName", "Expenses Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_TRAREQ1", "Z_Amount", "Expenses Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_TRAREQ1", "Z_UtilAmt", "Utilize Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_TRAREQ1", "Z_BalAmount", "Balance Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_TRAREQ1", "Z_ReqClaimAmt", "Request Claim Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_TRAREQ1", "Z_ApprClaimAmt", "Approved Claim Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_HR_TRAREQ1", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 2, SAPbobsCOM.BoFldSubTypes.st_Address, "A,NA,AP,P", "Applicable,Not Applicable,Approved,Paid", "A")

            AddFields("Z_HR_TRAREQ2", "Z_FileName", "File Name", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_TRAREQ2", "Z_AttDate", "Attachment Date", SAPbobsCOM.BoFieldTypes.db_Date)

            AddFields("OHPS", "Z_POSRef", " HR Position Reference", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            AddFields("OHPS", "Z_FrgnName", "Second Language Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

           

            AddFields("Z_HR_OTRIN", "Z_CGLACC", "Training Credit G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OTRIN", "Z_DGLACC", "Training Debit G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            AddFields("Z_HR_OSEAPP", "Z_FDate", "From Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OSEAPP", "Z_TDate", "To Date", SAPbobsCOM.BoFieldTypes.db_Date)
            addField("@Z_HR_OSEAPP", "Z_WStatus", "Workflow Ststus", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "SE,LM,SM,HR,DR", "SelfApproved,LineManager Approved,Sr.Manager Approved,HR Approved,Draft", "DR")
            addField("@Z_HR_OSEAPP", "Z_Status", "Approval Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "D,F,S,L,C", "Draft,Approved,2nd Level Approval,Closed,Canceled", "D")
            addField("@Z_HR_OSEAPP", "Z_GStatus", "Grevence Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "-,A,G", "-,Accepted,Grevence", "-")
            AddFields("Z_HR_OSEAPP", "Z_GDate", "Grevence Date", SAPbobsCOM.BoFieldTypes.db_Date)

            AddFields("Z_HR_OSEAPP", "Z_GNo", "Grevence No", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_OSEAPP", "Z_GRef", "Grevence Ref", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_OSEAPP", "Z_GRemarks", "Grevence Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OSEAPP", "Z_GHRSts", "Grevence HR Status", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)

            AddFields("Z_HR_OSEAPP", "Z_SCkApp", "Self Approved Status", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OSEAPP", "Z_LCkApp", "LineManager Approved Status", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OSEAPP", "Z_SrCkApp", "Sr.Manager Approved Status", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OSEAPP", "Z_HrCkApp", "Hr Approved Status", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)


            ''Personal Requistaion and Applicants

            AddTables("Z_HR_ORMPREQ", "Requisition Requisition", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddTables("Z_HR_RMPREQ1", "Recr. Business Objectives", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_RMPREQ2", "Requisition People Objective", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_RMPREQ3", "Requisition Comp. Objective", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_RMPREQ4", "Requisition Skill Sets", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)

            AddFields("Z_HR_ORMPREQ", "Z_EmpCode", "Employee Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_ORMPREQ", "Z_EmpName", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ORMPREQ", "Z_DeptCode", "Department Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_ORMPREQ", "Z_DeptName", "Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ORMPREQ", "Z_ReqDate", "Requested Date", SAPbobsCOM.BoFieldTypes.db_Date)
            addField("@Z_HR_ORMPREQ", "Z_ReqClss", "Requested Classification", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "N,E", "New Position,Existing/Vacancy Position", "N")
            AddFields("Z_HR_ORMPREQ", "Z_EmpPosi", "Employee Position", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_ORMPREQ", "Z_NewPosi", "New Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_ORMPREQ", "Z_HODStatus", "Head Of Department Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 2, SAPbobsCOM.BoFldSubTypes.st_Address, "O,SA,SR", "Open,First Level Approved,First Level Rejected", "O")
            AddFields("Z_HR_ORMPREQ", "Z_ExpMin", "Experience Minimum", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_ORMPREQ", "Z_ExpMax", "Experience Maximum", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_HR_ORMPREQ", "Z_HRStatus", "HR Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 2, SAPbobsCOM.BoFldSubTypes.st_Address, "O,HA,HF,L,HR", "Open,HR Approved,HR Follow-UP,HR Canceled,HR Rejected", "O")
            AddFields("Z_HR_ORMPREQ", "Z_Vacancy", "Vacancy", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_ORMPREQ", "Z_MgrRemarks", "Manager Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_ORMPREQ", "Z_HODCode", "HOD Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_ORMPREQ", "Z_HODName", "HOD Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ORMPREQ", "Z_HODDate", "HOD Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_ORMPREQ", "Z_HODRemarks", "HOD Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_ORMPREQ", "Z_HRCode", "HR Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_ORMPREQ", "Z_HRName", "HR Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ORMPREQ", "Z_HRDate", "HR Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_ORMPREQ", "Z_HRRemarks", "HR Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_HR_ORMPREQ", "Z_MgrStatus", "Manager Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 2, SAPbobsCOM.BoFldSubTypes.st_Address, "O,SA,SR,C,L,HF,HA,HR", "Open,First Level Approved,First Level Rejected,Closed,Canceled,HR Follow-UP,HR Approved,HR Rejected", "O")
            AddFields("Z_HR_ORMPREQ", "Z_PosName", "Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ORMPREQ", "Z_EmpstDate", "Employment Start Date", SAPbobsCOM.BoFieldTypes.db_Date) ''2013-08-06
            AddFields("Z_HR_ORMPREQ", "Z_IntAppDead", "Internal Application Deadline", SAPbobsCOM.BoFieldTypes.db_Date) ''2013-08-06
            AddFields("Z_HR_ORMPREQ", "Z_ExtAppDead", "External Application Deadline", SAPbobsCOM.BoFieldTypes.db_Date) ''2013-08-06

            ' oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            AddFields("Z_HR_RMPREQ2", "Z_PeoobjCode", "People Objective Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_RMPREQ2", "Z_PeoobjName", "People Objectives", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_RMPREQ2", "Z_PeoCategory", "People Category", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_RMPREQ2", "Z_Weight", "Weight", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)

            AddFields("Z_HR_RMPREQ1", "Z_BussCode", "Business Objective Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_RMPREQ1", "Z_BussDesc", "Business Obj.Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_RMPREQ1", "Z_BussWeight", "Business Weight", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)

            AddFields("Z_HR_RMPREQ3", "Z_CompCode", "Comp.Objective Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_RMPREQ3", "Z_CompDesc", "Comp.Obj.Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_RMPREQ3", "Z_CompWeight", "Competence Weight", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_HR_RMPREQ3", "Z_CompLevel", "Competence Level", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40) '''2013-08-05
            AddFields("Z_HR_RMPREQ4", "Z_Skillsets", "Skill Sets", SAPbobsCOM.BoFieldTypes.db_Memo)

            AddTables("Z_HR_OCRAPP", "Applicants Profiles", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddTables("Z_HR_CRAPP1", "Applicants Personal Details", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_CRAPP2", "Applicants Skill Sets", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_CRAPP3", "Applicants Education Details", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_CRAPP4", "Applicants Prev Employement", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_CRAPP5", "Applicants Attachments", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_CRAPP6", "Applicants Position", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)

            AddFields("Z_HR_OCRAPP", "Z_FirstName", "Applicant First Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OCRAPP", "Z_LastName", "Applicant Last Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OCRAPP", "Z_EmailId", "Email Id", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OCRAPP", "Z_Mobile", "Mobile No", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OCRAPP", "Z_AppDate", "Applicants Date", SAPbobsCOM.BoFieldTypes.db_Date)
            addField("@Z_HR_OCRAPP", "Z_Sex", "Sex", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "M,F", "Male,Female", "M")
            AddFields("Z_HR_OCRAPP", "Z_YrExp", "Year of Experience", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_OCRAPP", "Z_Skills", "Skill Sets Keywords", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OCRAPP", "Z_Dob", "Date of Birth", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OCRAPP", "Z_Nationality", "Nationality", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OCRAPP", "Z_PStreet", "Permanent Street", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OCRAPP", "Z_PCity", "Permanent City", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OCRAPP", "Z_PZipCode", "Permanent ZipCode", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OCRAPP", "Z_PCountry", "Permanent Country", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OCRAPP", "Z_TStreet", "Temporary Street", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OCRAPP", "Z_TCity", "Temporary City", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OCRAPP", "Z_TZipCode", "Temporary ZipCode", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OCRAPP", "Z_TCountry", "Temporary Country", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OCRAPP", "Z_Remarks", "Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OCRAPP", "Z_RequestCode", "Requisition Requisition Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20) ''2013-08-06
            AddFields("Z_HR_OCRAPP", "Z_InvScore", "Interview score", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum) ''2013-08-06
            addField("@Z_HR_OCRAPP", "Z_Status", "Applicant Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "R,S,F,N,I,D,M,O,J,C,A,H", "Received,Shortlisted,Shortlisted 1st Level,ShortListed Approved,Interview,Interview 1st Approval,Interview HR Approval,Job Offering,Offer Rejected,Canceled,Offer Accepted,Hired", "R")
            AddFields("Z_HR_OCRAPP", "Z_EmpId", "Employee Id", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_OCRAPP", "Z_Dept", "Department", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OCRAPP", "Z_DeptName", "Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OCRAPP", "Z_PosCode", "Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OCRAPP", "Z_PosName", "Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OCRAPP", "Z_JobCode", "Job Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OCRAPP", "Z_JobName", "Job Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OCRAPP", "Z_OrgCode", "Organization Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OCRAPP", "Z_OrgName", "Organization Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OCRAPP", "Z_JoinDate", "Joining Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OCRAPP", "Z_SalCode", "Salary Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            addField("@Z_HR_OCRAPP", "Z_AllPos", "All Position", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            AddFields("Z_HR_OCRAPP", "Z_OffBasic", "Accepted Basic Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)



            ''Edited 07-08-2013 By VetriSelvan.S (BUSON)
            AddFields("Z_HR_OCRAPP", "Z_ScndName", "Applicant Second Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OCRAPP", "Z_ThrdName", "Applicant Third Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OCRAPP", "Z_RejResn", "Applicant Rejection Reason", SAPbobsCOM.BoFieldTypes.db_Alpha, , 250)
            AddFields("Z_HR_OCRAPP", "Z_Active", "Active Status", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)
            '''''

            AddFields("Z_HR_OCRAPP", "Z_PStreetNo", "Permanent Street No", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OCRAPP", "Z_PBlock", "Permanent Block", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OCRAPP", "Z_PBuilding", "Permanent Building", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OCRAPP", "Z_PState", "Permanent State", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OCRAPP", "Z_TStreetNo", "Temporary Street No", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OCRAPP", "Z_TBlock", "Temporary Block", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OCRAPP", "Z_TBuilding", "Temporary Building", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OCRAPP", "Z_TState", "Temporary State", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            addField("@Z_HR_OCRAPP", "Z_Marital", "Marital Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "S,M,D,W", "Single,Married,Divorced,Widowed", "S")
            AddFields("Z_HR_OCRAPP", "Z_Children", "No of Children", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_OCRAPP", "Z_Citizen", "Citizenship", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OCRAPP", "Z_Passport", "Passport Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OCRAPP", "Z_Passexpdate", "Passport Expiry Date", SAPbobsCOM.BoFieldTypes.db_Date)

            AddFields("Z_HR_OCRAPP", "Z_CRUser", "Created User", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OCRAPP", "Z_CRDate", "Created Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OCRAPP", "Z_SFLUser", "Shortlisting FL User", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OCRAPP", "Z_SFLDate", "Shortlisting FL Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OCRAPP", "Z_SSLUser", "Shortlisting SL User", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OCRAPP", "Z_SSLDate", "Shortlisting SL Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OCRAPP", "Z_FLUser", "First Level User", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OCRAPP", "Z_FLDate", "First Level Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OCRAPP", "Z_HRUser", "HR Update User", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OCRAPP", "Z_HRDate", "HR Update Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OCRAPP", "Z_LUUser", "Last Update User", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OCRAPP", "Z_LUDate", "Last Update Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OCRAPP", "Z_HIUser", "Hired Update User", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OCRAPP", "Z_HIDate", "Hired Update Date", SAPbobsCOM.BoFieldTypes.db_Date)

            AddFields("Z_HR_OCRAPP", "Z_RStatus", "Residency Status", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OCRAPP", "Z_PCountry", "Placement Country", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OCRAPP", "Z_PSalary", "Present Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            AddFields("Z_HR_OCRAPP", "Z_ESalary", "Expected Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)

            addField("@Z_HR_OCRAPP", "Z_Source", "Applicant Source", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "I,E", "Internal,External", "E")
            '   AddFields("@Z_HR_OCRAPP", "Z_IntId", "Internal Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)


            AddFields("Z_HR_CRAPP1", "Z_LangName", "Languages Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_CRAPP1", "Z_Rate", "Languages Rating", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)

            AddFields("Z_HR_CRAPP2", "Z_SkillName", "Skill Sets Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_CRAPP2", "Z_Version", "Version", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_HR_CRAPP2", "Z_Rate", "Languages Rating", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Rate)


            AddFields("Z_HR_CRAPP3", "Z_GrFromDate", "Graduation From Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_CRAPP3", "Z_GrT0Date", "Graduation T0 Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_CRAPP3", "Z_Level", "Education Level", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_CRAPP3", "Z_School", "Institution", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_CRAPP3", "Z_Major", "Major", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_CRAPP3", "Z_Diploma", "Diploma", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)



            AddFields("Z_HR_CRAPP6", "Z_PosCode", "Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_CRAPP6", "Z_PosName", "Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)


            AddFields("Z_HR_CRAPP4", "Z_FromDate", "From Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_CRAPP4", "Z_ToDate", "To Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_CRAPP4", "Z_PrEmployer", "Previous Company Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_CRAPP4", "Z_PrPosition", "Previous Position", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_CRAPP4", "Z_Remarks", "Remarks", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_HR_CRAPP5", "Z_FileName", "File Name", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_CRAPP5", "Z_AttDate", "Attachment Date", SAPbobsCOM.BoFieldTypes.db_Date)
            ' oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)


            ''2013-07-04
            AddTables("Z_HR_OHEM1", "Shortlisted Candidates", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddTables("Z_HR_OHEM2", "Applicants Interview Process", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)

            AddFields("Z_HR_OHEM1", "Z_HRAppID", "Applicant ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_HR_OHEM1", "Z_HRAppName", "Applicant Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OHEM1", "Z_Dob", "Date of Birth", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OHEM1", "Z_Email", "Email Id", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OHEM1", "Z_YrExp", "Year of Experience", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_OHEM1", "Z_AppDate", "Allocated Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OHEM1", "Z_Skills", "Applicant Skills", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OHEM1", "Z_Dept", "Department", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_OHEM1", "Z_DeptName", "Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OHEM1", "Z_ReqNo", "Request No", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OHEM1", "Z_JobPosi", "Request Position", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OHEM1", "Z_JobPosiCode", "Request Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OHEM1", "Z_Mobile", "Mobile No", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            addField("@Z_HR_OHEM1", "Z_ApplStatus", "Applicant Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "O,S,A,R", "Open,Shortlisted,Approved,Rejected", "O")
            addField("@Z_HR_OHEM1", "Z_IntervStatus", "Interview Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "O,A,R,F,P", "Open,Accepted,Rejected,Job Offering,Placement", "O")
            addField("@Z_HR_OHEM1", "Z_OfferStatus", "Offer Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "O,A,R,J", "Open,Accepted,Rejected,Rejected", "O")
            AddFields("Z_HR_OHEM1", "Z_EmpId", "Employee Id", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_OHEM1", "Z_FileName", "Offer Attachment", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OHEM1", "Z_ApplyDate", "Apply Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OHEM1", "Z_RejRsn", "Rejection Reason", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_OHEM1", "Z_Finished", "Work Flow Finished", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            AddFields("Z_HR_OHEM2", "Z_HRAppID", "Applicant ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_HR_OHEM2", "Z_ScheduleDate", "Interview Scheduled Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OHEM2", "Z_InterviwerName", "Interviewer Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OHEM2", "Z_InterviewDate", "Interview Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OHEM2", "Z_Comments", "Comments", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OHEM2", "Z_Rating", "Interview Rating", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OHEM2", "Z_RatPer", "Interview Rating", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            addField("@Z_HR_OHEM2", "Z_InterviewStatus", "Interview Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,R,S", "Pending,Rejected,Selected", "P")
            AddFields("Z_HR_OHEM2", "Z_FileName", "File Name", SAPbobsCOM.BoFieldTypes.db_Memo)

            AddFields("Z_HR_OHEM2", "Z_SchEmpID", "Schedule EmpID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_HR_OHEM2", "Z_ScTime", "Schedule Time", SAPbobsCOM.BoFieldTypes.db_Date, , , SAPbobsCOM.BoFldSubTypes.st_Time)


            '  oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)


            '    oApplication.Utilities.Message("Initializing Database...", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            'Fields for approving Screened candidates (Shortlisted)
            AddFields("Z_HR_OHEM1", "Z_MgrRemarks", "First Level Approval Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_HR_OHEM1", "Z_MgrStatus", "First Level Approval Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 2, SAPbobsCOM.BoFldSubTypes.st_Address, "-,A,R", "Pending,Approved,Rejected", "-")
            AddFields("Z_HR_OHEM1", "Z_SMgrRemarks", "Second Level Approval Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_HR_OHEM1", "Z_SMgrStatus", "Second Level Approval Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 2, SAPbobsCOM.BoFldSubTypes.st_Address, "-,A,R", "Pending,Approved,Rejected", "-")

            AddFields("Z_HR_OHEM1", "Z_OARemarks", "Offer  Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_HR_OHEM1", "Z_OAStatus", "Offer Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 2, SAPbobsCOM.BoFldSubTypes.st_Address, "-,A,R", "Pending,Accepted,Rejected", "-")
            AddFields("Z_HR_OHEM1", "Z_IPHODRmks", "First Level Approval Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_HR_OHEM1", "Z_IPHODSta", "First Level Approval Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 2, SAPbobsCOM.BoFldSubTypes.st_Address, "-,S,R", "Pending,Selected,Rejected", "-")
            AddFields("Z_HR_OHEM1", "Z_IPHRRmks", "Second Level Approval Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_HR_OHEM1", "Z_IPHRSta", "Second Level Approval Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 2, SAPbobsCOM.BoFldSubTypes.st_Address, "-,S,R", "Pending,Selected,Rejected", "-")
            AddFields("Z_HR_OHEM1", "Z_IPHRRgR", "Second Level Rejection Reason", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OHEM1", "Z_IPHODRgR", "First Level Rejection Reason", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OHEM2", "Z_InterviwerID", "Interviewer ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddTables("Z_HR_OITYP", "Interview Type - Master", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddFields("Z_HR_OITYP", "Z_TypeCode", "Type Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OITYP", "Z_TypeName", "Type Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            addField("@Z_HR_OITYP", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddFields("Z_HR_OHEM2", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_HR_OHEM2", "Z_InType", "Interview Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)

            AddFields("Z_HR_OHEM1", "Z_IPLURmks", "IP LMU Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_HR_OHEM1", "Z_IPLUSta", "LMU Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 2, SAPbobsCOM.BoFldSubTypes.st_Address, "-,S,R", "-,Selected,Rejected", "-")



            AddTables("Z_HR_OREJC", "Rejection - Master", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddFields("Z_HR_OREJC", "Z_TypeCode", "Type Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OREJC", "Z_TypeName", "Type Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            addField("@Z_HR_OREJC", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            '---- User Defined Object's

            AddTables("Z_HR_OOREJ", "Offer Rejection - Master", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddFields("Z_HR_OOREJ", "Z_TypeCode", "Type Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OOREJ", "Z_TypeName", "Type Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            addField("@Z_HR_OOREJ", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            '---- User Defined Object's
            AddTables("Z_HR_IRATE", "Interview Rating Master", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddFields("Z_HR_IRATE", "Z_RateCode", "Ratings Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_IRATE", "Z_RateName", "Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_IRATE", "Z_RatePerc", "Rating Percentage", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            addField("@Z_HR_IRATE", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddFields("Z_HR_ORMPREQ", "Z_RecCandidate", "Received Candidates", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_ORMPREQ", "Z_ShortCandidate", "Shortlisted Candidates", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_ORMPREQ", "Z_OfferCandidate", "Offering Candidates", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_ORMPREQ", "Z_PlacedCandidate", "Placement Candidates", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_ORMPREQ", "Z_LMACandidate", "LM Approved Candidates", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_ORMPREQ", "Z_LMRCandidate", "LM Rejected Candidates", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_ORMPREQ", "Z_SMACandidate", "SM Approved Candidates", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_ORMPREQ", "Z_SMRCandidate", "SM Rejected Candidates", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_ORMPREQ", "Z_InvSelectCan", "Inv.Selected Candidates", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_ORMPREQ", "Z_InvRejectCan", "Inv.Rejected Candidates", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_ORMPREQ", "Z_HODACandidate", "HOD Approved Interview", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_ORMPREQ", "Z_HODRCandidate", "HOD Rejected Interview", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_ORMPREQ", "Z_HRACandidate", "HR Approved Interview", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_ORMPREQ", "Z_HRRCandidate", "HR Rejected Interview", SAPbobsCOM.BoFieldTypes.db_Numeric)

            AddFields("Z_HR_ORMPREQ", "Z_CRUser", "Created User", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_ORMPREQ", "Z_CRDate", "Created Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_ORMPREQ", "Z_FLUser", "First Level User", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_ORMPREQ", "Z_FLDate", "First Level Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_ORMPREQ", "Z_HRUser", "HR User", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_ORMPREQ", "Z_HRDate", "HR Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_ORMPREQ", "Z_CLUser", "Closed User", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_ORMPREQ", "Z_CLDate", "Closed Date", SAPbobsCOM.BoFieldTypes.db_Date)


            '2013-09-12  'Training Cost field updates

            AddFields("Z_HR_TRIN1", "Z_JENO", "Journal Entry TransID", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_TRIN1", "Z_JENUMBER", "Journal Entry Number", SAPbobsCOM.BoFieldTypes.db_Alpha)

            addField("@Z_HR_TRIN1", "Z_MgrRegStatus", "Manager Request Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,A,R", "Pending,Approved,Rejected", "P")
            AddFields("Z_HR_TRIN1", "Z_MgrRegRemarks", "Manager Request Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_HR_TRIN1", "Z_HRRegStatus", "HR Request Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,A,R", "Pending,Approved,Rejected", "P")
            AddFields("Z_HR_TRIN1", "Z_HrRegRemarks", "HR Request Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)

            AddTables("Z_HR_OHEM3", "Offer Details", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddFields("Z_HR_OHEM3", "Z_Basic", "Basic Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_OHEM3", "Z_Benifit", "Benifit Details", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OHEM3", "Z_Attachment", "Attachment", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_HR_OHEM3", "Z_Status", "Offer Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "O,A,J", "Offered,Offer Accepted,Offer rejected", "O")
            AddFields("Z_HR_OHEM3", "Z_RejReason", "Rejection Reason", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OHEM3", "Z_Remarks", "Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OHEM3", "Z_JoinDate", "Joining Date", SAPbobsCOM.BoFieldTypes.db_Date)

            AddFields("Z_HR_OCRAPP", "Z_InvBaseNo", "Interview Base Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OCRAPP", "Z_Basic", "Basic Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddTables("Z_HR_OMAIL", "Email SetUp Details", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_HR_OMAIL", "Z_SMTPSERV", "SMTP SERVER", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OMAIL", "Z_SMTPPORT", "SMTP PORT", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_HR_OMAIL", "Z_SMTPUSER", "SMTP USER", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OMAIL", "Z_SMTPPWD", "SMTP PASSWORD", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OMAIL", "Z_SSL", "SMTP SSL", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)

            addField("@Z_HR_OSEAPP", "Z_LMNotify", "Line Manager Notification", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            addField("@Z_HR_OSEAPP", "Z_HRNotify", "HR Notification", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddFields("Z_HR_OSEAPP", "Z_AIUserID", "Initialize UserID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OSEAPP", "Z_AIUDate", "Initialize Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OSEAPP", "Z_SFUserID", "Self UserID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OSEAPP", "Z_SFUDate", "Self Update Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OSEAPP", "Z_SFAUserID", "Self Acceptance UserID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OSEAPP", "Z_SFAUDate", "Self Acceptance Date", SAPbobsCOM.BoFieldTypes.db_Date)

            AddFields("Z_HR_OSEAPP", "Z_FUserID", "First Level UserID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OSEAPP", "Z_FUDate", "First Level Update Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OSEAPP", "Z_SCUserID", "Second Level UserID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OSEAPP", "Z_SCUDate", "Second Level Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OSEAPP", "Z_HRUserID", "HR UserID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OSEAPP", "Z_HRDate", "HR Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("OHEM", "Fld", "Folder", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)


            'Trainner Profile

            AddTables("Z_HR_TRRAPP", "Trainner Profiles", SAPbobsCOM.BoUTBTableType.bott_Document)

            AddFields("Z_HR_TRRAPP", "Z_FirstName", "Trainner First Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_TRRAPP", "Z_LastName", "Trainner Last Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_TRRAPP", "Z_ScndName", "Trainner Second Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_TRRAPP", "Z_ThrdName", "Trainner Third Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_TRRAPP", "Z_Active", "Active Status", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)
            addField("@Z_HR_TRRAPP", "Z_Type", "Trainner Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "E,I", "External,Internal", "E")
            AddFields("Z_HR_TRRAPP", "Z_EmailId", "Email Id", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_TRRAPP", "Z_Mobile", "Mobile No", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_TRRAPP", "Z_Dob", "Date of Birth", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_TRRAPP", "Z_Nationality", "Nationality", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            addField("@Z_HR_TRRAPP", "Z_Gender", "Gender", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "M,F", "Male,Female", "M")

            AddFields("Z_HR_TRRAPP", "Z_PStreet", "Permanent Street", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_TRRAPP", "Z_PCity", "Permanent City", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_TRRAPP", "Z_PZipCode", "Permanent ZipCode", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_TRRAPP", "Z_PCountry", "Permanent Country", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_TRRAPP", "Z_TStreet", "Temporary Street", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_TRRAPP", "Z_TCity", "Temporary City", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_TRRAPP", "Z_TZipCode", "Temporary ZipCode", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_TRRAPP", "Z_TCountry", "Temporary Country", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_TRRAPP", "Z_Remarks", "Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)

            AddFields("Z_HR_TRRAPP", "Z_CreditPoint", "Total Credit Points", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_TRRAPP", "Z_TotalPay", "Total Payment", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("Z_HR_OTRIN", "Z_CreditPoint", "Credit Points to Trainner", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_OTRIN", "Z_TotalPay", "Total Payment", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            addField("@Z_HR_OCRAPP", "Z_Reason", "Hiring Reason", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "R,P,D,S,T,C,H", "Replacement,New Position,New Department,New Subsidiary,Segregation of role and tasks,Crash,Projects based Hiring", "R")
            addField("@Z_HR_ORMPREQ", "Z_RecReason", "Requisition Reason", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "R,P,D,S,T,C,H", "Replacement,New Position,New Department,New Subsidiary,Segregation of role and tasks,Crash,Projects based Hiring", "R")
            AddFields("Z_HR_ORMPREQ", "Z_Location", "Requisition Location", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_HR_OCRAPP", "Z_RejCom", "Rejection Comments", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddTables("Z_HR_OSEC", "Section - Master", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddFields("Z_HR_OSEC", "Z_SecCode", "Section Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OSEC", "Z_SecName", "Section Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OSEC", "Z_FrgnName", "Second Lanugage Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_OSEC", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddTables("Z_HR_ORST", "Residency Status - Master", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddFields("Z_HR_ORST", "Z_StaCode", "Residency Status Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_ORST", "Z_StaName", "Residency Status Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            addField("@Z_HR_ORST", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddFields("OHEM", "Z_Rel_Name", "Relation Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_Rel_Type", "Relationship Details", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("OHEM", "Z_Rel_Phone", "Emergency Contact Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)


            AddTables("Z_HR_OBJLOAN", "Objects on Loan", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_HR_OBJLOAN", "Z_HREmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_HR_OBJLOAN", "Z_ObjCode", "Objects Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OBJLOAN", "Z_ObjName", "Objects Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OBJLOAN", "Z_Remarks", "Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_OBJLOAN", "Z_Dept", "Responsible Department", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OBJLOAN", "Z_ResID", "Responsible Employee", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_HR_OBJLOAN", "Z_ResName", "Responsible Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)


            AddTables("Z_HR_OLOAN", "Objects On Loan - Setup", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddFields("Z_HR_OLOAN", "Z_ObjCode", "Objects Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_OLOAN", "Z_ObjName", "Objects Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_OLOAN", "Z_Status", "Objects Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")
            AddFields("Z_HR_OLOAN", "Z_Dept", "Responsible Department", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)


            'Appraisal Result
            AddTables("Z_HR_OARE", "Objective Distribution", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_HR_OARE", "Z_Obj", "Objective", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OARE", "Z_Weight", "Weight", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)

            '2014-02-04
            AddFields("Z_HR_SALST1", "Z_BasicPer", "% of basic salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_HR_OCRAPP", "Z_SkypeId", "Skype Id", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)

            AddTables("Z_HR_ORRRE", "Rec.Request Reason - Master", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddFields("Z_HR_ORRRE", "Z_ReasonCode", "Reason Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_ORRRE", "Z_ReasonName", "Request Reason Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            addField("@Z_HR_ORRRE", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddFields("Z_HR_ORMPREQ", "Z_SalRangeFrom", "Salary Range From", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_ORMPREQ", "Z_SalRangeTo", "Salary Range To", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_ORMPREQ", "Z_ReqReason", "Request Reason", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)

            AddFields("Z_HR_OCRAPP", "Z_Prodate", "Probation Period Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_OCRAPP", "Z_Promonth", "Probation Period in Months", SAPbobsCOM.BoFieldTypes.db_Numeric)

            AddFields("OHEM", "Z_Prodate", "Probation Period Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("OHEM", "Z_Promonth", "Probation Period in Months", SAPbobsCOM.BoFieldTypes.db_Numeric)

            AddTables("Z_PAY_OEAR", "Allowance Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_OEAR", "Z_CODE", "Allowance Code", SAPbobsCOM.BoFieldTypes.db_Alpha, )
            AddFields("Z_PAY_OEAR", "Z_NAME", "Allowance Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_OEAR", "Z_FrgnName", "Foreign Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_OEAR", "Z_EAR_GLACC", "Deduction G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            addField("@Z_PAY_OEAR", "Z_SOCI_BENE", "Under Social Security", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OEAR", "Z_INCOM_TAX", "Taxable", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OEAR", "Z_Percentage", "Default Percentage", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            addField("@Z_PAY_OEAR", "Z_OffCycle", "OffCyle", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OEAR", "Z_EOS", "Affects EOS", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OEAR", "Z_DefAmt", "Default Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY_OEAR", "Z_PaidWkd", "Paid per working day", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OEAR", "Z_ProRate", "Prorated", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OEAR", "Z_Max", "Max.Exemption Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            addField("@Z_PAY_OEAR", "Z_PostType", "Posting Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "B,A", "Business Partner,GL Account", "A")
            addField("@Z_PAY_OEAR", "Z_PaidLeave", "Inlcude for Paid Leave", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OEAR", "Z_AnnulaLeave", "Include for Annual Leave", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OEAR", "Z_Type", "Allowance Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "F,V", "Fixed,Variable", "F")
            addField("@Z_PAY_OEAR", "Z_OVERTIME", "Affects Overtime", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")


            AddTables("Z_PAY_OCON", "Contribution Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_OCON", "Z_FrgnName", "Foreign Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_OCON", "Z_CON_GLACC", "Contribution G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            addField("@Z_PAY_OCON", "Z_PostType", "Posting Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "B,A", "Business Partner,GL Account", "A")
            AddFields("Z_PAY1", "Z_SalCode", "Salary Scale Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY3", "Z_SalCode", "Salary Scale Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)

            AddFields("Z_PAY3", "Z_StartDate", "Start Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY3", "Z_EndDate", "End Date", SAPbobsCOM.BoFieldTypes.db_Date)


            AddFields("Z_HR_HEM2", "Z_IncAmount", "Increment Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_HEM2", "Z_EffFromdt", "Effective From Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_HEM2", "Z_EffTodt", "Effective To Date", SAPbobsCOM.BoFieldTypes.db_Date)
            addField("@Z_HR_HEM2", "Z_Status", "Promotion Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,A,C", "Pending,Approved,Cancelled", "P")
            addField("@Z_HR_HEM2", "Z_Posting", "Promotion Posting", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            AddFields("Z_HR_HEM4", "Z_EffFromdt", "Effective From Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_HEM4", "Z_EffTodt", "Effective To Date", SAPbobsCOM.BoFieldTypes.db_Date)
            addField("@Z_HR_HEM4", "Z_Status", "Promotion Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,A,C", "Pending,Approved,Cancelled", "P")
            addField("@Z_HR_HEM4", "Z_Posting", "Promotion Posting", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            addField("OHEM", "Z_EmpLiCyStatus", "Employee Life Cycle Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,T,C,R", "Promotion,Transfer,Position Change,Regular", "R")
            'Second Phase

            addField("@Z_HR_LOGIN", "Z_ESSAPPROVER", "ESS Approval", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "E,M", "Employee,Manager", "E")

            AddTables("Z_HR_TRQCCA", "Training Qust. categories", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddFields("Z_HR_TRQCCA", "Z_Code", "Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_TRQCCA", "Z_Name", "Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_TRQCCA", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddTables("Z_HR_TRQCIT", "Training Qust.Items", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddFields("Z_HR_TRQCIT", "Z_Code", "Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_TRQCIT", "Z_Name", "Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_TRQCIT", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddTables("Z_HR_TRQCRA", "Training Qust.Rating", SAPbobsCOM.BoUTBTableType.bott_MasterData)
            AddFields("Z_HR_TRQCRA", "Z_Code", "Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_TRQCRA", "Z_Name", "Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_TRQCRA", "Z_Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "Y")

            AddTables("Z_HR_TRAEVA", "Training Evaluation", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_HR_TRAEVA", "Z_EmpCode", "Employee Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_TRAEVA", "Z_EmpName", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_TRAEVA", "Z_AgendaCode", "Agenda Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_TRAEVA", "Z_QusCatCode", "Category Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_TRAEVA", "Z_QusCatName", "Category Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_TRAEVA", "Z_QusItemCode", "Items Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_TRAEVA", "Z_QusItemName", "Items Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_TRAEVA", "Z_QusRatCode", "Rating Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_TRAEVA", "Z_QusRatName", "Rating Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_TRAEVA", "Z_Comments", "Comments", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_TRAEVA", "Z_MgrStatus", "Manager Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,R,A", "Pending,Approved,Rejected", "P")



            'Leave Master

            AddTables("Z_PAY_LEAVE", "Leave Type Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_LEAVE", "Z_FrgnName", "Second Language Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_LEAVE", "Z_DedRate", "Deduction Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY_LEAVE", "Z_PaidLeave", "Leave Category", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,U,A", "Paid Leave,UnPaid,Annual Leave ", "P")
            AddFields("Z_PAY_LEAVE", "Z_DaysYear", "Yearly Upper Limit", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Quantity)
            AddFields("Z_PAY_LEAVE", "Z_NoofDays", "Accured Days per Month ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Quantity)
            addField("@Z_PAY_LEAVE", "Z_Accured", "Accured", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,NO", "Y")
            addField("@Z_PAY_LEAVE", "Z_Cutoff", "Cuttoff Days", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "H,W,B,N", "Holiday,Weekends,Both,None", "N")
            addField("@Z_PAY_LEAVE", "Z_EOS", "Affect EOS", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,NO", "N")
            AddFields("Z_PAY_LEAVE", "Z_EntAft", "Antitled After", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_LEAVE", "Z_TimesTaken", "Times Taken per Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_LEAVE", "Z_MaxDays", "Max days taken/Transaction", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_LEAVE", "Z_DailyRate", "Daily Rate Days", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_LEAVE", "Z_LifeTime", "Taken per LifeTime", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_LEAVE", "Z_GLACC", "Debit GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_LEAVE", "Z_GLACC1", "Credit GL Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            addField("@Z_PAY_LEAVE", "Z_OffCycle", "Affect Off Cycle Payroll", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_LEAVE", "Z_OB", "Default Opening Balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Quantity)
            AddFields("Z_PAY_LEAVE", "Z_SickLeave", "Sick Leave Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)
            addField("@Z_PAY_LEAVE", "Z_StopProces", "Stop Process", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")


            'Leave Transaction

            AddTables("Z_PAY_OLETRANS1", " ESS Leave  Transaction", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_OLETRANS1", "Z_EMPID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY_OLETRANS1", "Z_EMPNAME", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_PAY_OLETRANS1", "Z_TransType", "Transaction Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "L,R,T,P", "Leave Request,Return From Leave,Resignation,Permission", "L")
            AddFields("Z_PAY_OLETRANS1", "Z_TrnsCode", "Leave Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY_OLETRANS1", "Z_StartDate", "Date From", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_OLETRANS1", "Z_EndDate", "Date T0", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_OLETRANS1", "Z_NoofDays", "Number of Days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OLETRANS1", "Z_Notes", "Notes", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_PAY_OLETRANS1", "Z_Month", "Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_OLETRANS1", "Z_Year", "Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            '  addField("@Z_PAY_OLETRANS1", "Z_IsTerm", "Termination Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OLETRANS1", "Z_ReJoiNDate", "Re Joining Date", SAPbobsCOM.BoFieldTypes.db_Date)
            addField("@Z_PAY_OLETRANS1", "Z_OffCycle", "OffCycle", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OLETRANS1", "Z_ApprovedBy", "Approved By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_OLETRANS1", "Z_AppRemarks", "Approver Remarks", SAPbobsCOM.BoFieldTypes.db_Alpha, , 250)
            AddFields("Z_PAY_OLETRANS1", "Z_ApprDate", "Approved Date", SAPbobsCOM.BoFieldTypes.db_Date)

            AddFields("Z_PAY_OLETRANS1", "Z_RetJoiNDate", "Return ReJoining Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_OLETRANS1", "Z_RApprovedBy", "Return Approved By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_PAY_OLETRANS1", "Z_RAppRemarks", "Return Approver Remarks", SAPbobsCOM.BoFieldTypes.db_Alpha, , 250)
            AddFields("Z_PAY_OLETRANS1", "Z_RApprDate", "Return Approved Date", SAPbobsCOM.BoFieldTypes.db_Date)

            AddFields("Z_PAY_OLETRANS1", "Z_FromTime", "Leave by Fromhour", SAPbobsCOM.BoFieldTypes.db_Date, , , SAPbobsCOM.BoFldSubTypes.st_Time)
            AddFields("Z_PAY_OLETRANS1", "Z_ToTime", "Leave by Tohour", SAPbobsCOM.BoFieldTypes.db_Date, , , SAPbobsCOM.BoFldSubTypes.st_Time)
            addField("@Z_PAY_OLETRANS1", "Z_Status", "Approval Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,R,A", "Pending,Rejected,Approved", "P")
            addField("@Z_PAY_OLETRANS1", "Z_RStatus", "Return Approval Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,R,A", "Pending,Rejected,Approved", "P")
            AddFields("Z_PAY_OLETRANS1", "Z_LevBal", "Leave balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Quantity)



            AddTables("Z_PAY_OLETRANS", "Leave Transaction", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_OLETRANS", "Z_EMPID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY_OLETRANS", "Z_EMPNAME", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_OLETRANS", "Z_TrnsCode", "Leave Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY_OLETRANS", "Z_StartDate", "Date From", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_OLETRANS", "Z_EndDate", "Date T0", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_OLETRANS", "Z_NoofDays", "Number of Days", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OLETRANS", "Z_NoofHours", "Number of Hours", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Quantity)
            AddFields("Z_PAY_OLETRANS", "Z_Notes", "Notes", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_PAY_OLETRANS", "Z_Month", "Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_OLETRANS", "Z_Year", "Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_OLETRANS", "Z_Attachment", "Attachment", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_PAY_OLETRANS", "Z_IsTerm", "Termination Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OLETRANS", "Z_ReJoiNDate", "Re Joining Date", SAPbobsCOM.BoFieldTypes.db_Date)
            addField("@Z_PAY_OLETRANS", "Z_OffCycle", "OffCycle", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OLETRANS", "Z_DailyRate", "Daily Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OLETRANS", "Z_Amount", "Daily Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY_OLETRANS", "Z_StopProces", "Stop Process", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OLETRANS", "Z_Cutoff", "Cuttoff Days", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "H,W,B,N", "Holiday,Weekends,Both,None", "N")
            addField("@Z_PAY_OLETRANS", "Z_Posted", "Posted", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OLETRANS", "Z_TermRea", "Termination Reason", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            addField("@Z_PAY_OLETRANS", "Z_EOS", "Include EOS", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OLETRANS", "Z_Leave", "Include Leave", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OLETRANS", "Z_Ticket", "Include Ticket", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OLETRANS", "Z_Saving", "Include Saving", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OLETRANS", "Z_LevBal", "Leave balance", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Quantity)
          
            'New changes 2014-05-30

            AddFields("Z_HR_HEM4", "Z_CreatedBy", "Position Change Created By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_HEM4", "Z_Credt", "Position Change Created Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_HEM4", "Z_ApprovedBy", "Position Change Approved By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_HEM4", "Z_Appdt", "Position Change Approved Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_HEM4", "Z_PostedBy", "Position Change Posted By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_HEM4", "Z_PostDate", "Position Change Created Date", SAPbobsCOM.BoFieldTypes.db_Date)

            AddFields("Z_HR_HEM2", "Z_CreatedBy", "Position Change Created By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_HEM2", "Z_Credt", "Position Change Created Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_HEM2", "Z_ApprovedBy", "Position Change Approved By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_HEM2", "Z_Appdt", "Position Change Approved Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_HEM2", "Z_PostedBy", "Position Change Posted By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_HEM2", "Z_PostDate", "Position Change Created Date", SAPbobsCOM.BoFieldTypes.db_Date)

            AddFields("Z_HR_TRRAPP", "Z_EmpId", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            'AddFields("Z_HR_TRRAPP", "Z_ExEmpId", "System Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)

            AddFields("OHEM", "Z_EmpLifRef", "Emp.Life Cycle Posting Ref", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)


           
            AddTables("Z_HR_EXFORM4", "Fixed Assets", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)

            AddFields("Z_HR_EXFORM4", "Z_ResID", "Responsible Employee", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_HR_EXFORM4", "Z_Dept", "Responsible Department", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_HR_EXFORM4", "Z_ResName", "Responsible Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_EXFORM4", "Z_ObjCode", "Objects Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_EXFORM4", "Z_ObjName", "Objects Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_EXFORM4", "Z_Remarks", "Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_EXFORM4", "Z_ApprovedBy", "Fixed Assets Approved By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_EXFORM4", "Z_Appdt", "Fixed Assets Approved Date", SAPbobsCOM.BoFieldTypes.db_Date)
            addField("@Z_HR_EXFORM4", "Z_CompStatus", "Completion Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "C,P", "Complete,Pending", "P")

            AddFields("Z_HR_EXFORM1", "Z_ResID", "Responsible Employee", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_HR_EXFORM1", "Z_ResName", "Responsible Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_EXFORM1", "Z_ApprovedBy", "Responsibility Approved By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_EXFORM1", "Z_Appdt", "Responsibility Approved Date", SAPbobsCOM.BoFieldTypes.db_Date)

            AddFields("Z_HR_OEXFOM", "Z_ResExit", "Reason of Exit", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_HR_OEXFOM", "Z_EmpLevel", "Employee level", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OEXFOM", "Z_Subsidiary ", "Subsidiary  Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_OEXFOM", "Z_Remarks", "Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_HR_OEXFOM", "Z_ExitType", "Exit Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "R,T,E", "Resignation,Termination,End of contract", "R")
            AddFields("Z_HR_OEXFOM", "Z_ExtEmployee", "T&A Employee", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)

            AddFields("Z_HR_ORES", "Z_ResID", "Responsible Employee", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_HR_ORES", "Z_ResName", "Responsible Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_HR_OBJLOAN", "Z_ApprovedBy", "Fixed Assets Approved By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OBJLOAN", "Z_Appdt", "Fixed Assets Approved Date", SAPbobsCOM.BoFieldTypes.db_Date)
            addField("@Z_HR_OBJLOAN", "Z_CompStatus", "Completion Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "C,P", "Complete,Pending", "P")


            AddFields("Z_HR_EXFORM1", "Z_Attachment", "Attachment", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_EXFORM2", "Z_Attachment", "Attachment", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_EXFORM4", "Z_Attachment", "Attachment", SAPbobsCOM.BoFieldTypes.db_Memo)

            AddFields("Z_HR_EXFORM2", "Z_ApprovedBy", "Fixed Assets Approved By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_EXFORM2", "Z_Appdt", "Fixed Assets Approved Date", SAPbobsCOM.BoFieldTypes.db_Date)

            AddFields("Z_HR_ORMPREQ", "Z_SalMid", "Mid Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_ORMPREQ", "Z_ExpSal", "Expexted Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)

            AddFields("OUDP", "Z_ReqHR", "Request to HR", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OCLG", "Z_HREmpID", "Request Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OCLG", "Z_HREmpName", "Request Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("OCLG", "Z_HRSystemID", "Employee T&A Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("OCLG", "Z_AssEmpID", "Assaigned Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            ''Empenses Claim Request

            AddTables("Z_HR_EXPCL", "Expenses Claim Request", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_HR_EXPCL", "Z_EmpID", "Request Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            AddFields("Z_HR_EXPCL", "Z_EmpName", "Request Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_EXPCL", "Z_Subdt", "Submitted Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_EXPCL", "Z_Client", "Client Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_EXPCL", "Z_Project", "Project Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_EXPCL", "Z_Claimdt", "Claim Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_EXPCL", "Z_City", "City", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_EXPCL", "Z_Currency", "Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_EXPCL", "Z_CurAmt", "Currency Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_EXPCL", "Z_ExcRate", "Exchange Rate", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_EXPCL", "Z_UsdAmt", "US Dollar Amount", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            addField("@Z_HR_EXPCL", "Z_Reimburse", "To be Reimbursed", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_HR_EXPCL", "Z_ReimAmt", "Reimbursed Amount", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)
            AddFields("Z_HR_EXPCL", "Z_ExpCode", "Expenses Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_EXPCL", "Z_ExpType", "Expenses type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_EXPCL", "Z_PayMethod", "Payment Method", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_EXPCL", "Z_Notes", "Note", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_EXPCL", "Z_ApproveBy", "Approved By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_EXPCL", "Z_Approvedt", "Approver Date", SAPbobsCOM.BoFieldTypes.db_Date)
            addField("@Z_HR_EXPCL", "Z_AppStatus", "Approved Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,R,A", "Pending,Rejected,Approved", "P")
            AddFields("Z_HR_EXPCL", "Z_TraCode", "Travel Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_EXPCL", "Z_TraDesc", "Travel Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_EXPCL", "Z_Attachment", "Attachments", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_EXPCL", "Z_TripType", "Travel Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "E,N", "Existing,New", "E")

            AddTables("Z_HR_PAYMD", "Payment Method Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_HR_PAYMD", "Z_PayMethod", "Payment Method", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)


            'New Training Request

            AddTables("Z_HR_ONTREQ", "New Training Request", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddFields("Z_HR_ONTREQ", "Z_ReqDate", "Requested Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_ONTREQ", "Z_HREmpID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_HR_ONTREQ", "Z_HREmpName", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ONTREQ", "Z_CourseName", "Course Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ONTREQ", "Z_CourseDetails", "Course Details", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_ONTREQ", "Z_PosiCode", "Position Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_ONTREQ", "Z_PosiName", "Position Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ONTREQ", "Z_DeptCode", "Department Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_ONTREQ", "Z_DeptName", "Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_HR_ONTREQ", "Z_ReqStatus", "New Training Request Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 2, SAPbobsCOM.BoFldSubTypes.st_Address, "P,MA,MR,HA,HR", "Pending,Manager Approved,Manager Rejected,HR Approved,HR Rejected", "P")
            AddFields("Z_HR_ONTREQ", "Z_MgrRemarks", "Manager Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_HR_ONTREQ", "Z_MgrStatus", "Manager Approved Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 2, SAPbobsCOM.BoFldSubTypes.st_Address, "P,MA,MR", "Pending,Manager Approved,Manager Rejected", "P")
            AddFields("Z_HR_ONTREQ", "Z_HRRemarks", "HR Remarks", SAPbobsCOM.BoFieldTypes.db_Memo)
            addField("@Z_HR_ONTREQ", "Z_HRStatus", "HR Approved Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 2, SAPbobsCOM.BoFldSubTypes.st_Address, "P,HA,HR", "Pending,HR Approved,HR Rejected", "P")
            AddFields("Z_HR_ONTREQ", "Z_TrainCost", "Training Course Cost", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            AddFields("Z_HR_ONTREQ", "Z_EstExpe", "Est.Travel&Living Expenses", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Price)
            AddFields("Z_HR_ONTREQ", "Z_TrainLoc", "Training Location", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_HR_ONTREQ", "Z_TrainFrdt", "Training From Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_ONTREQ", "Z_TrainTodt", "Training To Date", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_ONTREQ", "Z_BussDays", "Train.Duration(Bus.Days)", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_ONTREQ", "Z_CalDays", "Train.Duration(Cal.Days)", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_ONTREQ", "Z_AwayOff", "Away From Office(Bus.Days)", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_HR_ONTREQ", "Z_CerTestAvail", "Certification Test Available", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)
            AddFields("Z_HR_ONTREQ", "Z_CerTestIncl", "Certification Test Included", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)
            AddFields("Z_HR_ONTREQ", "Z_LveDuty", "Trainee Leaves Duty On", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_ONTREQ", "Z_TravelOn", "Trainee Travels On", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_ONTREQ", "Z_ReturnOn", "Trainee Returns On", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_ONTREQ", "Z_ResumeOn", "Trainee Resumes Duty On", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_HR_ONTREQ", "Z_Notes", "Comments", SAPbobsCOM.BoFieldTypes.db_Memo)

            AddFields("Z_HR_TRAPL1", "Z_LocCurrency", "Local Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, , 60)
            AddFields("Z_HR_ASSTP1", "Z_LocCurrency", "Local Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, , 60)

            AddFields("Z_HR_OTRAPL", "Z_CostCode", "Center Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            AddTables("Z_HR_DOCTY", "Document Type Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_HR_DOCTY", "Z_DocType", "Document Type Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_DOCTY", "Z_DocDesc", "Document Type Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_HR_ORMPREQ", "Z_CREId", "Created EmpId", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_ORMPREQ", "Z_FLEId", "First Level EmpId", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_ORMPREQ", "Z_SLEId", "Second Level EmpId", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_ORMPREQ", "Z_CLEId", "Closing EmpId", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_ORMPREQ", "Z_SalCode", "Salary Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 50)

            AddTables("Z_HR_OAPPT", "Approval Template", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddTables("Z_HR_APPT1", "Approval Orginator", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_APPT2", "Approval Authorizer", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)
            AddTables("Z_HR_APPT3", "Department Authorizer", SAPbobsCOM.BoUTBTableType.bott_DocumentLines)

            AddFields("Z_HR_OAPPT", "Z_Code", "Approval Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OAPPT", "Z_Name", "Approval Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_OAPPT", "Z_DocType", "Document Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OAPPT", "Z_DocDesc", "Document Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)

            AddFields("Z_HR_APPT1", "Z_OUser", "Orginator Id", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_APPT1", "Z_OName", "Orginator Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_HR_APPT3", "Z_DeptCode", "Department Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_APPT3", "Z_DeptName", "Department Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            AddFields("Z_HR_APPT2", "Z_AUser", "Authorizer Id", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_APPT2", "Z_AName", "Authorizer Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("Z_HR_APPT2", "Z_AMan", "Mandatory", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)
            AddFields("Z_HR_APPT2", "Z_AFinal", "Final Stage", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)

            AddTables("Z_HR_APHIS", "Approval History", SAPbobsCOM.BoUTBTableType.bott_Document)
            AddFields("Z_HR_APHIS", "Z_DocEntry", "Document Entry", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_APHIS", "Z_DocType", "Document Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_APHIS", "Z_EmpId", "Employee Id", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_APHIS", "Z_EmpName", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)

            addField("@Z_HR_APHIS", "Z_AppStatus", "Approved Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,A,R", "Pending,Approved,Rejected", "P")
            AddFields("Z_HR_APHIS", "Z_Remarks", "Comments", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_HR_APHIS", "Z_ApproveBy", "Approved By", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_APHIS", "Z_Approvedt", "Approver Date", SAPbobsCOM.BoFieldTypes.db_Date)

            AddFields("Z_HR_APHIS", "Z_ADocEntry", "Template DocEntry", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_APHIS", "Z_ALineId", "Template LineId", SAPbobsCOM.BoFieldTypes.db_Numeric)

            AddFields("Z_HR_APHIS", "Z_Month", "Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_APHIS", "Z_Year", "Year", SAPbobsCOM.BoFieldTypes.db_Numeric)

            addField("@Z_HR_OTRAREQ", "Z_AppStatus", "Approval Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,A,R", "Pending,Approved,Rejected", "P")
            addField("@Z_HR_TRIN1", "Z_AppStatus", "Approval Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,A,R", "Pending,Approved,Rejected", "P")
            addField("@Z_HR_ONTREQ", "Z_AppStatus", "Approval Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,A,R", "Pending,Approved,Rejected", "P")
            addField("@Z_HR_ORMPREQ", "Z_AppStatus", "Approval Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,A,R,C,L", "Pending,Approved,Rejected,Closed,Canceled", "P")
            addField("@Z_HR_OCRAPP", "Z_AppStatus", "Approval Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,A,R", "Pending,Approved,Rejected", "P")
            addField("@Z_HR_OHEM1", "Z_AppStatus", "Approval Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,A,R", "Pending,Approved,Rejected", "P")
            addField("@Z_HR_HEM2", "Z_AppStatus", "Approval Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,A,R", "Pending,Approved,Rejected", "P")
            addField("@Z_HR_HEM4", "Z_AppStatus", "Approval Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "P,A,R", "Pending,Approved,Rejected", "P")
            AddFields("Z_HR_ORMPREQ", "Z_ExEmpID", "Ext.EmpNo", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)


            AddFields("Z_HR_EXPANCES", "Z_AlloCode", "Allowance Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)

            AddFields("Z_HR_APPT1", "Z_EmpID", "T&A Employee Id", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OAPPT", "Z_Active", "Active Template", SAPbobsCOM.BoFieldTypes.db_Alpha, , 1)
            AddFields("Z_HR_OAPPT", "Z_LveType", "Leave Type", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OAPPT", "Z_LveDesc", "Leave Description", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)
            AddFields("OHEM", "Z_EmpID", "T&A Employee Id", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)

            AddFields("Z_HR_EXPCL", "Z_Month", "Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_EXPCL", "Z_Year", "Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_HR_EXPCL", "Z_AlloCode", "Allowance Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)



            AddTables("Z_PAY_OEAR1", "Variable Earning Master", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_OEAR1", "Z_FrgnName", "Foreign Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_OEAR1", "Z_DefAmt", "Default Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY_OEAR1", "Z_SOCI_BENE", "Under Social Security", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_OEAR1", "Z_INCOM_TAX", "Taxable", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OEAR1", "Z_Max", "Max.Exemption Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            addField("@Z_PAY_OEAR1", "Z_OffCycle", "OffCyle", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OEAR1", "Z_EAR_GLACC", "Earing G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            addField("@Z_PAY_OEAR1", "Z_PostType", "Posting Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "B,A", "Business Partner,GL Account", "A")
            addField("@Z_PAY_OEAR1", "Z_EOS", "Effects EOS", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_OEAR1", "Z_BaiscPer", "Percentage in Basic Salary", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Percentage)
            AddFields("Z_PAY_OEAR1", "Z_AvgYear", "Average Years for EOS ", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_OEAR1", "Z_DED_GLACC", "Deduction G/L Account", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)
            addField("@Z_PAY_OEAR1", "Z_AffDedu", "Part of Deduction ", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")


            AddFields("Z_PAY_OLETRANS", "Z_LeaveName", "Leave Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            AddFields("Z_PAY_OLETRANS1", "Z_LeaveName", "Leave Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)





            AddTables("Z_PAY_TRANS", "Payroll Transaction", SAPbobsCOM.BoUTBTableType.bott_NoObject)
            AddFields("Z_PAY_TRANS", "Z_EMPID", "Employee ID", SAPbobsCOM.BoFieldTypes.db_Alpha, , 10)
            AddFields("Z_PAY_TRANS", "Z_EMPNAME", "Employee Name", SAPbobsCOM.BoFieldTypes.db_Alpha, , 200)
            addField("@Z_PAY_TRANS", "Z_Type", "Transaction Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "O,E,D,H", "Over Time,Earning,Deductions,Hourly Transactions", "E")
            AddFields("Z_PAY_TRANS", "Z_TrnsCode", "Transaction Code", SAPbobsCOM.BoFieldTypes.db_Alpha, , 30)
            AddFields("Z_PAY_TRANS", "Z_StartDate", "Date From", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_TRANS", "Z_EndDate", "Date T0", SAPbobsCOM.BoFieldTypes.db_Date)
            AddFields("Z_PAY_TRANS", "Z_Amount", "Amount", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Sum)
            AddFields("Z_PAY_TRANS", "Z_NoofHours", "Number of Hours", SAPbobsCOM.BoFieldTypes.db_Float, , , SAPbobsCOM.BoFldSubTypes.st_Quantity)
            AddFields("Z_PAY_TRANS", "Z_Notes", "Notes", SAPbobsCOM.BoFieldTypes.db_Memo)
            AddFields("Z_PAY_TRANS", "Z_Month", "Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_TRANS", "Z_Year", "Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            addField("@Z_PAY_TRANS", "Z_Posted", "Posted", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_TRANS", "Z_DedMonth", "Deduction Month", SAPbobsCOM.BoFieldTypes.db_Numeric)
            AddFields("Z_PAY_TRANS", "Z_DedYear", "Deduction Year", SAPbobsCOM.BoFieldTypes.db_Numeric)
            addField("@Z_PAY_TRANS", "Z_AffDedu", "Part of Deduction ", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            addField("@Z_PAY_TRANS", "Z_offTool", "OffCycle Tool", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")
            AddFields("Z_PAY_TRANS", "Z_JVNo", "Journal Reference", SAPbobsCOM.BoFieldTypes.db_Alpha, , 20)

            AddFields("Z_PAY_TRANS", "Z_EmpId1", "Employee Number", SAPbobsCOM.BoFieldTypes.db_Alpha, , 100)


            addField("@Z_HR_EXPCL", "Z_PayPosted", "Posted to Payroll", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Address, "Y,N", "Yes,No", "N")

            AddFields("Z_HR_EXPCL", "Z_CurApprover", "Current Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_EXPCL", "Z_NxtApprover", "Next Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OTRAREQ", "Z_CurApprover", "Current Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OTRAREQ", "Z_NxtApprover", "Next Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_TRIN1", "Z_CurApprover", "Current Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_TRIN1", "Z_NxtApprover", "Next Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_ONTREQ", "Z_CurApprover", "Current Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_ONTREQ", "Z_NxtApprover", "Next Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_ORMPREQ", "Z_CurApprover", "Current Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_ORMPREQ", "Z_NxtApprover", "Next Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OHEM1", "Z_CurApprover", "Current Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_OHEM1", "Z_NxtApprover", "Next Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_HEM2", "Z_CurApprover", "Current Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_HEM2", "Z_NxtApprover", "Next Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_HEM4", "Z_CurApprover", "Current Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_HR_HEM4", "Z_NxtApprover", "Next Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_PAY_OLETRANS1", "Z_CurApprover", "Current Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)
            AddFields("Z_PAY_OLETRANS1", "Z_NxtApprover", "Next Approver", SAPbobsCOM.BoFieldTypes.db_Alpha, , 40)

            CreateUDO()
        Catch ex As Exception
            Throw ex
        Finally
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Try
    End Sub

    Public Sub CreateUDO()
        Try
            oApplication.Utilities.Message("Creating User Defined Objects.....", SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            AddUDO("Z_HR_APHIS", "Approval History", "Z_HR_APHIS", "DocEntry", "U_Z_DocEntry", , , , , , , SAPbobsCOM.BoUDOObjType.boud_Document, True, "AZ_HR_APHIS")
            AddUDO("Z_HR_OAPPT", "Approval Template", "Z_HR_OAPPT", "DocEntry", "U_Z_Code", "Z_HR_APPT1", "Z_HR_APPT2", "Z_HR_APPT3", , , , SAPbobsCOM.BoUDOObjType.boud_Document)
            AddUDO("Z_HR_OEXFOM", "Exit Form", "Z_HR_OEXFOM", "U_Z_empID", "DocEntry", "Z_HR_EXFORM1", "Z_HR_EXFORM2", "Z_HR_EXFORM3", "Z_HR_EXFORM4", , , SAPbobsCOM.BoUDOObjType.boud_Document)
            UDOResponsibilities("Z_HR_ORES", "Exit from Responsibilities", "Z_HR_ORES", 1, "U_Z_ResCode", )
            UDOQustinnaries("Z_HR_OQUS", "Questionnaire", "Z_HR_OQUS", 1, "U_Z_QusCode", )

            AddUDO("Z_HR_TRRAPP", "Trainer Profile", "Z_HR_TRRAPP", "DocEntry", "U_Z_FirstName", , , , , , , SAPbobsCOM.BoUDOObjType.boud_Document)
            UDOLanguages("Z_HR_OLNG", "Languages-master", "Z_HR_OLNG", 1, "U_Z_LanName", )
            UDOCompetenceObj("Z_HR_OCOMP", "Competence Objective", "Z_HR_OCOMP", 1, "U_Z_CompCode", )
            AddUDO("Z_HR_OTRIN", "Training Agenda Setup", "Z_HR_OTRIN", "U_Z_TrainCode", "DocEntry", "Z_HR_OTRIN1", , , , , , SAPbobsCOM.BoUDOObjType.boud_Document)
            AddUDO("Z_HR_OTRAREQ", "Employee Travel Request", "Z_HR_OTRAREQ", "DocEntry", "U_Z_EmpId", "Z_HR_TRAREQ1", "Z_HR_TRAREQ2", , , , , SAPbobsCOM.BoUDOObjType.boud_Document)
            AddUDO("Z_HR_OTRAPLA", "Travel Plan-Setup", "Z_HR_OTRAPL", "U_Z_TraCode", "U_Z_TraName", "Z_HR_TRAPL1", , , , , , SAPbobsCOM.BoUDOObjType.boud_Document)
            AddUDO("Z_HR_OCRAPPL", "Create Applicants", "Z_HR_OCRAPP", "U_Z_FirstName", "DocEntry", "Z_HR_CRAPP1", "Z_HR_CRAPP2", "Z_HR_CRAPP3", "Z_HR_CRAPP4", "Z_HR_CRAPP5", "Z_HR_CRAPP6", SAPbobsCOM.BoUDOObjType.boud_Document)
            AddUDO("Z_HR_ORREQS", "Requisition List", "Z_HR_ORMPREQ", "U_Z_EmpCode", "DocEntry", "Z_HR_RMPREQ1", "Z_HR_RMPREQ2", "Z_HR_RMPREQ3", "Z_HR_RMPREQ4", , , SAPbobsCOM.BoUDOObjType.boud_Document)
            AddUDO("Z_HR_OPOSIN", "Position Mapping", "Z_HR_OPOSIN", "U_Z_PosCode", "DocEntry", , , , , , , SAPbobsCOM.BoUDOObjType.boud_Document)
            AddUDO("Z_HR_OSELAPP", "Self Appraisal", "Z_HR_OSEAPP", "U_Z_EmpId", "DocEntry", "Z_HR_SEAPP1", "Z_HR_SEAPP2", "Z_HR_SEAPP3", "Z_HR_SEAPP4", , , SAPbobsCOM.BoUDOObjType.boud_Document, True, "AZ_HR_OSEAPP")
            AddUDO("Z_HR_OCOURS", "Course Master Setup", "Z_HR_OCOUR", "U_Z_CourseCode", "DocEntry", "Z_HR_COUR1", "Z_HR_COUR2", "Z_HR_COUR3", "Z_HR_COUR4", "Z_HR_COUR5", , SAPbobsCOM.BoUDOObjType.boud_Document)
            AddUDO("Z_HR_EPOCOM", "Employee Position Competence", "Z_HR_OPOSCO", "U_Z_PosCode", "U_Z_PosName", "Z_HR_POSCO1", "Z_HR_POSCO2", "Z_HR_POSCO3", "Z_HR_POSCO4", , , SAPbobsCOM.BoUDOObjType.boud_Document)
            AddUDO("Z_HR_OSALST", "Salary Structure", "Z_HR_OSALST", "U_Z_SalCode", "DocEntry", "Z_HR_SALST1", "Z_HR_SALST2", , , , , SAPbobsCOM.BoUDOObjType.boud_Document)
            AddUDO("Z_HR_DEMAP", "Department Mapping", "Z_HR_ODEMA", "U_Z_DeptCode", "U_Z_DeptName", "Z_HR_DEMA1", , , , , , SAPbobsCOM.BoUDOObjType.boud_Document)
            AddUDO("Z_HR_OCOBJ", "Competence Objective", "Z_HR_OCOB", "U_Z_CompCode", "U_Z_CompName", "Z_HR_COB1", "Z_HR_COB2", , , , , SAPbobsCOM.BoUDOObjType.boud_Document)
            UDOExpances("Z_HR_EXPANCES", "Expences - Master", "Z_HR_EXPANCES", 1, "U_Z_ExpName")
            UDOCompany("Z_HR_OADM", "Company-master", "Z_HR_OADM", 1, "U_Z_CompName", )
            UDOFunction("Z_HR_OFCA", "Division-master", "Z_HR_OFCA", 1, "U_Z_FuncName", )
            UDOUnit("Z_HR_OUNT", "Unit-master", "Z_HR_OUNT", 1, "U_Z_UnitName", )
            UDOLocation("Z_HR_OLOC", "Location-Master", "Z_HR_OLOC", 1, "U_Z_LocName", )
            UDOGrade("Z_HR_OGRD", "Grade-Master", "Z_HR_OGRD", 1, "U_Z_GrdeName", )
            UDOLevel("Z_HR_OLVL", "Level-Master", "Z_HR_OLVL", 1, "U_Z_LvelName", )
            UDOAllowance("Z_HR_OALLO", "Allowance-Master", "Z_HR_OALLO", 1, "U_Z_AlloName", )
            UDOBenefits("Z_HR_OBEFI", "Benefits-Master", "Z_HR_OBEFI", 1, "U_Z_BenefName", )
            UDORatings("Z_HR_ORATE", "Ratings-Master", "Z_HR_ORATE", 1, "U_Z_RateName", )
            UDOBussObjective("Z_HR_OBUOB", "Business Objectives", "Z_HR_OBUOB", 1, "U_Z_BussName", )
            UDOPeopleCatry("Z_HR_PECAT", "People Category", "Z_HR_PECAT", 1, "U_Z_CatName", )
            UDOPeopleObj("Z_HR_OPEOB", "People Objective", "Z_HR_OPEOB", 1, "U_Z_PeoobjCode", )
            UDOCompetenceLevel("Z_HR_COLVL", "Competence Level-Master", "Z_HR_COLVL", 1, "U_Z_LvelName", )
            UDOOrgStructure("Z_HR_ORGST", "Organization Structure", "Z_HR_ORGST", 1, "U_Z_CompName", )
            UDOLogin("Z_HR_LOGIN", "Login Setup", "Z_HR_LOGIN", 1, "U_Z_EMPID", )
            UDOCourseType("Z_HR_OCOTY", "Course Type-Setup", "Z_HR_OCOTY", 1, "U_Z_CouTypeCode", )
            UDOCourseCategory("Z_HR_OCOCA", "Course Category-Setup", "Z_HR_OCOCA", 1, "U_Z_CouCatCode", )

            AddUDO("Z_HR_ONTREQ", "New Training Request", "Z_HR_ONTREQ", "DocEntry", , , , , , , , SAPbobsCOM.BoUDOObjType.boud_Document)
            AddUDO("Z_HR_OHEM", "Applicants Interview Process", "Z_HR_OHEM1", "DocEntry", "U_Z_HRAppID", "Z_HR_OHEM2", "Z_HR_OHEM3", , , , , SAPbobsCOM.BoUDOObjType.boud_Document)
            UDOInterviewType("Z_HR_OITYP", "Interview Type -Master", "Z_HR_OITYP", 1, "U_Z_TypeName", )
            UDORejectionMaster("Z_HR_OREJC", "Rejection -Master", "Z_HR_OREJC", 1, "U_Z_TypeName", )
            UDOIntRatings("Z_HR_IRATE", "Interview Ratings-Master", "Z_HR_IRATE", 1, "U_Z_RateName", )
            UDOOfferRejectionMaster("Z_HR_OOREJ", "Offer Rejection - Master", "Z_HR_OOREJ", 1, "U_Z_TypeName", )
            UDOSection("Z_HR_OSEC", "Section-master", "Z_HR_OSEC", 1, "U_Z_SecName", )
            UDOResidency("Z_HR_ORST", "Residency Status -master", "Z_HR_ORST", 1, "U_Z_StaName", )

            UDOObjectsonLoan("Z_HR_OLOAN", "Objects on Laon -master", "Z_HR_OLOAN", 1, "U_Z_ObjName", )
            UDORecruitmentRequestReason("Z_HR_ORRRE", "Rec.Request Reason - Master", "Z_HR_ORRRE", 1, "U_Z_ReasonCode", )



            'Second Phae
            UDOTrainingQuestionCategory("Z_HR_TRQCCA", "Train-quest categories", "Z_HR_TRQCCA", 1, "U_Z_Name", )
            UDOTrainingQuestionItem("Z_HR_TRQCIT", "Train-questItem", "Z_HR_TRQCIT", 1, "U_Z_Name", )
            UDOTrainingQuestionCategory("Z_HR_TRQCRA", "Train-quest Rate", "Z_HR_TRQCRA", 1, "U_Z_Name", )


        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Function UDOObjectsonLoan(ByVal strUDO As String, _
                           ByVal strDesc As String, _
                               ByVal strTable As String, _
                                   ByVal intFind As Integer, _
                                       Optional ByVal strCode As String = "", _
                                           Optional ByVal strName As String = "") As Boolean
        Dim oUserObjects As SAPbobsCOM.UserObjectsMD
        Dim lngRet As Long
        Try
            oUserObjects = oApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)
            If oUserObjects.GetByKey(strUDO) = 0 Then
                oUserObjects.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanClose = SAPbobsCOM.BoYesNoEnum.tYES
                oUserObjects.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.CanFind = SAPbobsCOM.BoYesNoEnum.tYES


                oUserObjects.CanLog = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.LogTableName = ""
                oUserObjects.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.ExtensionName = ""

                oUserObjects.FormColumns.FormColumnAlias = "Code"
                oUserObjects.FormColumns.FormColumnDescription = "Code"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "Name"
                oUserObjects.FormColumns.FormColumnDescription = "Name"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "DocEntry"
                oUserObjects.FormColumns.FormColumnDescription = "DocEntry"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_ObjCode"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_ObjCode"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_ObjName"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_ObjName"
                oUserObjects.FormColumns.Add()
                oUserObjects.FormColumns.FormColumnAlias = "U_Z_Status"
                oUserObjects.FormColumns.FormColumnDescription = "U_Z_Status"
                oUserObjects.FormColumns.Add()
                oUserObjects.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO
                oUserObjects.Code = strUDO
                oUserObjects.Name = strDesc
                oUserObjects.ObjectType = SAPbobsCOM.BoUDOObjType.boud_MasterData
                oUserObjects.TableName = strTable

                If oUserObjects.CanFind = 1 Then
                    oUserObjects.FindColumns.ColumnAlias = strCode
                    ' oUserObjects.FindColumns.Add()
                    'oUserObjects.FindColumns.SetCurrentLine(1)
                    'oUserObjects.FindColumns.ColumnAlias = strName
                    'oUserObjects.FindColumns.Add()
                End If

                If oUserObjects.Add() <> 0 Then
                    oApplication.Utilities.Message(oApplication.Company.GetLastErrorDescription, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                    oUserObjects = Nothing
                    Return False
                End If
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjects)
                oUserObjects = Nothing
                Return True
            End If
        Catch ex As Exception
            Throw ex
        Finally
            oUserObjects = Nothing
            GC.Collect()
        End Try
    End Function

#End Region

End Class
