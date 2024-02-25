// using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
// using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
// using Xunit.Abstractions;
// using Xunit.Sdk;
//
// namespace CommandLine_tests.TestInfrastructure;
//
// public class CommandLineTestFramework : XunitTestFramework
// {
//     public CommandLineTestFramework(IMessageSink messageSink)
//         : base(messageSink)
//     {
//     }
//     
//     protected override ITestFrameworkDiscoverer CreateDiscoverer(
//         IAssemblyInfo assemblyInfo)
//         => new CommandLineTestFrameworkDiscoverer(
//             assemblyInfo,
//             SourceInformationProvider,
//             DiagnosticMessageSink);
// }
// public class CommandLineTestFrameworkDiscoverer : XunitTestFrameworkDiscoverer
// {
//     public CommandLineTestFrameworkDiscoverer(
//         IAssemblyInfo assemblyInfo,
//         ISourceInformationProvider sourceProvider,
//         IMessageSink diagnosticMessageSink,
//         IXunitTestCollectionFactory? collectionFactory = null)
//         : base(
//             assemblyInfo,
//             sourceProvider,
//             diagnosticMessageSink,
//             collectionFactory)
//     {
//     }
//
//     protected override bool IsValidTestClass(ITypeInfo type)
//         => base.IsValidTestClass(type) &&
//            FilterType(type);
//
//     protected virtual bool FilterType(ITypeInfo type)
//     {
//         // Insert your custom filter conditions here.
//         return true;
//     }
// }
//
//
