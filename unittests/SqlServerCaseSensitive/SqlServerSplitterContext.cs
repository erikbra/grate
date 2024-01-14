using grate.Infrastructure;
// ReSharper disable StringLiteralTypo

namespace SqlServerCaseSensitive.TestInfrastructure;

public static class SqlServerSplitterContext
{

    public static class FullSplitter
    {
        public static readonly string tsql_statement = @"
BOB1
GO

/* COMMENT */
BOB2
GO

-- GO

BOB3 GO

--`~!@#$%^&*()-_+=,.;:'""[]\/?<> GO

BOB5
   GO

BOB6
GO

/* GO */

BOB7

/* 

GO

*/

BOB8

--
GO

BOB9

-- `~!@#$%^&*()-_+=,.;:'""[]\/?<>
GO

BOB10GO

CREATE TABLE POGO
{}

INSERT INTO POGO (id,desc) VALUES (1,'GO')

BOB11

  -- TODO: To be good, there should be type column

-- dfgjhdfgdjkgk dfgdfg GO
BOB12

UPDATE Timmy SET id = 'something something go'
UPDATE Timmy SET id = 'something something: go'

ALTER TABLE Inv.something ADD
	gagagag decimal(20, 12) NULL,
    asdfasdf DECIMAL(20, 6) NULL,
	didbibi decimal(20, 6) NULL,
	yeppsasd decimal(20, 6) NULL,
	uhuhhh datetime NULL,
	slsald varchar(15) NULL,
	uhasdf varchar(15) NULL,
    daf_asdfasdf DECIMAL(20,6) NULL;
GO

EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Daily job', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=3, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'
dml statements
GO  
dml statements '

GO

INSERT [dbo].[Foo] ([Bar]) VALUES (N'hello--world.
Thanks!')
INSERT [dbo].[Foo] ([Bar]) VALUES (N'Go speed racer, go speed racer, go speed racer go!!!!! ')

GO";

        public static readonly string tsql_statement_scrubbed = @"
BOB1
" + StatementSplitter.BatchTerminatorReplacementString + @"

/* COMMENT */
BOB2
" + StatementSplitter.BatchTerminatorReplacementString + @"

-- GO

BOB3 " + StatementSplitter.BatchTerminatorReplacementString + @"

--`~!@#$%^&*()-_+=,.;:'""[]\/?<> GO

BOB5
   " + StatementSplitter.BatchTerminatorReplacementString + @"

BOB6
" + StatementSplitter.BatchTerminatorReplacementString + @"

/* GO */

BOB7

/* 

GO

*/

BOB8

--
" + StatementSplitter.BatchTerminatorReplacementString + @"

BOB9

-- `~!@#$%^&*()-_+=,.;:'""[]\/?<>
" + StatementSplitter.BatchTerminatorReplacementString + @"

BOB10GO

CREATE TABLE POGO
{}

INSERT INTO POGO (id,desc) VALUES (1,'GO')

BOB11

  -- TODO: To be good, there should be type column

-- dfgjhdfgdjkgk dfgdfg GO
BOB12

UPDATE Timmy SET id = 'something something go'
UPDATE Timmy SET id = 'something something: go'

ALTER TABLE Inv.something ADD
	gagagag decimal(20, 12) NULL,
    asdfasdf DECIMAL(20, 6) NULL,
	didbibi decimal(20, 6) NULL,
	yeppsasd decimal(20, 6) NULL,
	uhuhhh datetime NULL,
	slsald varchar(15) NULL,
	uhasdf varchar(15) NULL,
    daf_asdfasdf DECIMAL(20,6) NULL;
" + StatementSplitter.BatchTerminatorReplacementString + @"

EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Daily job', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=3, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'
dml statements
GO  
dml statements '

" + StatementSplitter.BatchTerminatorReplacementString + @"

INSERT [dbo].[Foo] ([Bar]) VALUES (N'hello--world.
Thanks!')
INSERT [dbo].[Foo] ([Bar]) VALUES (N'Go speed racer, go speed racer, go speed racer go!!!!! ')

" + StatementSplitter.BatchTerminatorReplacementString + @"";

        public static readonly string plsql_statement =
            @"
SQL1;
;
SQL2;
;
tmpSql := 'DROP SEQUENCE mutatieStockID';
EXECUTE IMMEDIATE tmpSql; 
;
BEGIN
INSERT into Table (columnname) values ("";"");
UPDATE Table set columnname="";"";
END;
";
        public static readonly string plsql_statement_scrubbed = @"
SQL1;
" + StatementSplitter.BatchTerminatorReplacementString + @"
SQL2;
" + StatementSplitter.BatchTerminatorReplacementString + @"
tmpSql := 'DROP SEQUENCE mutatieStockID';
EXECUTE IMMEDIATE tmpSql; 
" + StatementSplitter.BatchTerminatorReplacementString + @"
BEGIN
INSERT into Table (columnname) values ("";"");
UPDATE Table set columnname="";"";
END;
";
    }
}
