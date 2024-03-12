// using Microsoft.Data.Sqlite;
// using Xunit.Sdk;
// namespace TestCommon.TestInfrastructure;
// public class SqliteExternalDatabase(GrateTestConfig grateTestConfig) : ITestDatabase
// {
//     public string AdminConnectionString { get; } = grateTestConfig.AdminConnectionString ?? throw new ArgumentNullException(nameof(grateTestConfig));
//
//     public string ConnectionString(string database)
//     {
//         var builder = new SqliteConnectionStringBuilder(AdminConnectionString)
//         {
//             DataSource = $"{database}.db",
//             DefaultTimeout = 2
//         };
//         return builder.ConnectionString;
//     }
//
//     public string UserConnectionString(string database)
//     {
//         var builder = new SqliteConnectionStringBuilder(AdminConnectionString)
//         {
//             DataSource = $"{database}.db",
//             DefaultTimeout = 2
//         };
//         return builder.ConnectionString;
//     }
//     
//     private readonly IMessageSink _messageSink;
//
//     public SqliteExternalDatabase(IMessageSink messageSink)
//     {
//         _messageSink = messageSink;
//     }
//     public Task DisposeAsync()
//     {
//         SqliteConnection.ClearAllPools();
//
//         var currentDirectory = Directory.GetCurrentDirectory();
//         var dbFiles = Directory.GetFiles(currentDirectory, "*.db");
//         var message = new DiagnosticMessage("After tests. Deleting DB files.");
//         _messageSink.OnMessage(message);
//         foreach (var dbFile in dbFiles)
//         {
//             var deleteMessage = new DiagnosticMessage("File: {0}", dbFile);
//             _messageSink.OnMessage(deleteMessage);
//             File.Delete(dbFile);
//         }
//         return Task.CompletedTask;
//     }
//
//     public Task InitializeAsync()
//     {
//         var currentDirectory = Directory.GetCurrentDirectory();
//         var dbFiles = Directory.GetFiles(currentDirectory, "*.db");
//         var message = new DiagnosticMessage("Before tests. Deleting old DB files.");
//         _messageSink.OnMessage(message);
//         foreach (var dbFile in dbFiles)
//         {
//             TryDeletingFile(dbFile);
//         }
//         return Task.CompletedTask;
//     }
//     private void TryDeletingFile(string dbFile)
//     {
//         var i = 0;
//         var sleepTime = 300;
//         const int maxTries = 5;
//         while (i++ < maxTries)
//         {
//             try
//             {
//                 var message = new DiagnosticMessage("File: {0}", dbFile);
//                 _messageSink.OnMessage(message);
//                 File.Delete(dbFile);
//                 return;
//             }
//             catch (IOException) when (i <= maxTries)
//             {
//                 Thread.Sleep(sleepTime);
//             }
//         }
//     }
//
// }
