create or alter view vw_CurrentTestData
as

select * from TestTable where IsDeleted = 0;