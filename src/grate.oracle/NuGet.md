# grate

grate is a SQL scripts migration runner, using plain, old SQL for migrations. No meta-language, no code, no config,
no EF migrations. It gives you full flexibility, and full control of your migrations, and lets you use
all the fancy features of you particular database system. You are not constrained to any lowest common
feature set of all supported databases.

## Minimal example
The only required argument to pass to grate is a **connection string** to tell it where to find your database. 
It will deploy to that database, looking for sql scripts in the current directory.

```csharp
[Fact]
public async Task Run_migration_agains_target_db()
{
    var serviceCollection = new ServiceCollection();
    serviceCollection.AddLogging();
    serviceCollection.AddGrate(builder =>
    {
        builder.WithSqlFilesDirectory("/db")
                .WithConnectionString("oracle connection string here")
                .UseOracle(); // Important!, you need to specify the database type to use.
    });
    var serviceProvider = serviceCollection.BuildServiceProvider();
    var grateMigrator = serviceProvider.GetService<IGrateMigrator>();
    await grateMigrator!.Migrate();
}
```

for more configuration options, see the [documentation](https://erikbra.github.io/grate/configuration-options/).



## grate supports the following DMBS's

* Microsoft SQL server (sqlserver)
* PostgreSQL (postgresql)
* MariaDB/MySQL (mariadb)
* Sqlite (sqlite)
* Oracle (oracle)

Full documentation can be found at [https://erikbra.github.io/grate/](https://erikbra.github.io/grate/).


