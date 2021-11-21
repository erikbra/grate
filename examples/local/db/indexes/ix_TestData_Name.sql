-- we could have created this in the `UP` script as well if we preferred...

create index ix_TestData_Name 
on TestTable(Name)
where IsDeleted = 0 -- More complex features like filtered indexes/compression on disk are trivial.
with (data_compression=page);