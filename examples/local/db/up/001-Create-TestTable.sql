create table TestTable
(
    Id int not null identity(1,1),
    Name varchar(20),

    constraint PK_Test PRIMARY KEY CLUSTERED 
	(
		Id ASC
	) with (data_compression = page) -- Note we can use all the features of Sql Server because we're just .sql files...

);