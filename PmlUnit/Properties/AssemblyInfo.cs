// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Allgemeine Informationen über eine Assembly werden über die folgenden 
// Attribute gesteuert. Ändern Sie diese Attributwerte, um die Informationen zu ändern,
// die einer Assembly zugeordnet sind.
[assembly: AssemblyDescription("Test framework and test runner for PML code in AVEVA PDMS, Everything3D, and other products.")]

#if PDMS_121

[assembly: AssemblyTitle("PML Unit for PDMS 12.1")]
#if DEBUG
[assembly: AssemblyConfiguration("PDMS 12.1 Debug")]
#else
[assembly: AssemblyConfiguration("PDMS 12.1 Release")]
#endif

#elif E3D_11

[assembly: AssemblyTitle("PML Unit for E3D 1.1")]
#if DEBUG
[assembly: AssemblyConfiguration("E3D 1.1 Debug")]
#else
[assembly: AssemblyConfiguration("E3D 1.1 Release")]
#endif

#elif E3D_21
[assembly: AssemblyTitle("PML Unit for E3D 2.1")]
#if DEBUG
[assembly: AssemblyConfiguration("E3D 2.1 Debug")]
#else
[assembly: AssemblyConfiguration("E3D 2.1 Release")]
#endif

#endif

[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("PML Unit")]
[assembly: AssemblyCopyright("Copyright © 2019 Florian Zimmermann")]
[assembly: AssemblyCulture("")]

[assembly: CLSCompliant(true)]
[assembly: NeutralResourcesLanguage("en")]

[assembly: InternalsVisibleTo("PmlUnit.SmokeTest, PublicKey=00240000048000009400000006020000002400005253413100040000010001003188a8fc38ec4ff8e9a083c79e815a93976161b5939b55d509b87c7b5bee50c5deace818061429d071985c263bc197b2e66f71049d3babf84445819de2389f1501b030c265d93d162f1ccef7d3d6778146a96954bcbf3451e07667cdb9f656c77651a938f9b7a0b914d88d435d7499ade07b7679e22d054be3b0b7a49902b4f5")]
[assembly: InternalsVisibleTo("PmlUnit.TestForm, PublicKey=00240000048000009400000006020000002400005253413100040000010001003188a8fc38ec4ff8e9a083c79e815a93976161b5939b55d509b87c7b5bee50c5deace818061429d071985c263bc197b2e66f71049d3babf84445819de2389f1501b030c265d93d162f1ccef7d3d6778146a96954bcbf3451e07667cdb9f656c77651a938f9b7a0b914d88d435d7499ade07b7679e22d054be3b0b7a49902b4f5")]
[assembly: InternalsVisibleTo("PmlUnit.Tests, PublicKey=00240000048000009400000006020000002400005253413100040000010001003188a8fc38ec4ff8e9a083c79e815a93976161b5939b55d509b87c7b5bee50c5deace818061429d071985c263bc197b2e66f71049d3babf84445819de2389f1501b030c265d93d162f1ccef7d3d6778146a96954bcbf3451e07667cdb9f656c77651a938f9b7a0b914d88d435d7499ade07b7679e22d054be3b0b7a49902b4f5")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")]

// Durch Festlegen von ComVisible auf "false" werden die Typen in dieser Assembly unsichtbar 
// für COM-Komponenten.  Wenn Sie auf einen Typ in dieser Assembly von 
// COM aus zugreifen müssen, sollten Sie das ComVisible-Attribut für diesen Typ auf "True" festlegen.
[assembly: ComVisible(false)]

// Die folgende GUID bestimmt die ID der Typbibliothek, wenn dieses Projekt für COM verfügbar gemacht wird
[assembly: Guid("03530452-7c4a-4943-aa30-7efeadade1f2")]

// Versionsinformationen für eine Assembly bestehen aus den folgenden vier Werten:
//
//      Hauptversion
//      Nebenversion 
//      Buildnummer
//      Revision
//
// Sie können alle Werte angeben oder die standardmäßigen Build- und Revisionsnummern 
// übernehmen, indem Sie "*" eingeben:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.3.0.0")]
[assembly: AssemblyFileVersion("0.3.0.0")]
[assembly: AssemblyInformationalVersion("0.3.0")]
