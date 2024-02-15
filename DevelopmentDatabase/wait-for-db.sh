#!/bin/bash

# Wait 60 seconds for SQL Server to start up by ensuring that 
# calling SQLCMD does not return an error code, which will ensure that sqlcmd is accessible
# and that system and user databases return "0" which means all databases are in an "online" state
# https://docs.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-databases-transact-sql?view=sql-server-2017 

DBSTATUS=1
ERRCODE=1

DBSTATUS=$(/opt/mssql-tools/bin/sqlcmd -h -1 -t 1 -U SA -P Change_this_password10 -Q "SET NOCOUNT ON; Select SUM(state) from sys.databases")
ERRCODE=$?

if [ $DBSTATUS -ne 0 ] OR [ $ERRCODE -ne 0 ]; then 
	echo "One or more databases are not in an ONLINE state"
	exit 1
fi

exit 0