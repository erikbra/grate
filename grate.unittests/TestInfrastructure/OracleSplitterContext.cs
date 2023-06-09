using grate.Infrastructure;
// ReSharper disable StringLiteralTypo

namespace grate.unittests.TestInfrastructure;

public static class OracleSplitterContext
{

    public static class FullSplitter
    {
        public static readonly string PLSqlStatement = @"
BOB1
/

/* COMMENT */
BOB2
/

-- /

BOB3 /

--`~!@#$%^&*()-_+=,.;:'""[]\/?<> /

BOB5
   /

BOB6
/

/* / */

BOB7

/* 

/

*/

BOB8

--
/

BOB9

-- `~!@#$%^&*()-_+=,.;:'""[]\/?<>
/

BOB10/

CREATE TABLE PO/
{}

INSERT INTO PO/ (id,desc) VALUES (1,'/')

BOB11

  -- TODO: To be good, there should be type column

-- dfgjhdfgdjkgk dfgdfg /
BOB12

UPDATE Timmy SET id = 'something something /'
UPDATE Timmy SET id = 'something something: /'

ALTER TABLE Inv.something ADD
	gagagag decimal(20, 12) NULL,
    asdfasdf DECIMAL(20, 6) NULL,
	didbibi decimal(20, 6) NULL,
	yeppsasd decimal(20, 6) NULL,
	uhuhhh datetime NULL,
	slsald varchar(15) NULL,
	uhasdf varchar(15) NULL,
    daf_asdfasdf DECIMAL(20,6) NULL;
/

EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Daily job', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=3, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'PLSQL', 
		@command=N'
dml statements
/  
dml statements '

/

INSERT [dbo].[Foo] ([Bar]) VALUES (N'hello--world.
Thanks!')
INSERT [dbo].[Foo] ([Bar]) VALUES (N'/ speed racer, / speed racer, / speed racer /!!!!! ')

/";

        public static readonly string PLSqlStatementScrubbed = @"
BOB1
" + StatementSplitter.BatchTerminatorReplacementString + @"

/* COMMENT */
BOB2
" + StatementSplitter.BatchTerminatorReplacementString + @"

-- /

BOB3 " + StatementSplitter.BatchTerminatorReplacementString + @"

--`~!@#$%^&*()-_+=,.;:'""[]\/?<> /

BOB5
   " + StatementSplitter.BatchTerminatorReplacementString + @"

BOB6
" + StatementSplitter.BatchTerminatorReplacementString + @"

/* / */

BOB7

/* 

/

*/

BOB8

--
" + StatementSplitter.BatchTerminatorReplacementString + @"

BOB9

-- `~!@#$%^&*()-_+=,.;:'""[]\/?<>
" + StatementSplitter.BatchTerminatorReplacementString + @"

BOB10/

CREATE TABLE PO/
{}

INSERT INTO PO/ (id,desc) VALUES (1,'/')

BOB11

  -- TODO: To be good, there should be type column

-- dfgjhdfgdjkgk dfgdfg /
BOB12

UPDATE Timmy SET id = 'something something /'
UPDATE Timmy SET id = 'something something: /'

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
		@os_run_priority=0, @subsystem=N'PLSQL', 
		@command=N'
dml statements
/  
dml statements '

" + StatementSplitter.BatchTerminatorReplacementString + @"

INSERT [dbo].[Foo] ([Bar]) VALUES (N'hello--world.
Thanks!')
INSERT [dbo].[Foo] ([Bar]) VALUES (N'/ speed racer, / speed racer, / speed racer /!!!!! ')

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
