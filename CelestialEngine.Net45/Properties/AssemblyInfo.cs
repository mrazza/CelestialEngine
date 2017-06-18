using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("CelestialEngine.Net45")]
[assembly: AssemblyDescription("A 2D deferred-rendered .NET game engine designed for programmers with advanced lighting, shadows, and physics.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("mrazza, wgraham17")]
[assembly: AssemblyProduct("CelestialEngine.Net45")]
[assembly: AssemblyCopyright("Copyright ©  2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("50831f80-9950-4abc-b1b9-c922504d9f22")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyInformationalVersion(VersionString.Value)]
[assembly: AssemblyVersion(VersionString.Value)]
[assembly: AssemblyFileVersion(VersionString.Value)]

internal class VersionString
{
    public const string Value = "0.1.1";
}