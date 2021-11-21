create or alter proc sp_GetTestById
  @id int
as

Select Id, Name
from vw_CurrentTestData
where Id = @id;

GO

-- we could do permissions here if we needed
--  grant execute on sp_GetTestById to some_user;
