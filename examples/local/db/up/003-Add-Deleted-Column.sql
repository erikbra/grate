
-- we can run alter statements here because we know grate will create the table for us first in the 001 script
alter table TestTable add IsDeleted bit not null default(0);
