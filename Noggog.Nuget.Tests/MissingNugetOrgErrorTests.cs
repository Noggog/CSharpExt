﻿using System.IO.Abstractions.TestingHelpers;
using System.Xml.Linq;
using AutoFixture.Xunit2;
using Noggog.Nuget.Errors;
using Noggog.Testing.AutoFixture;
using Shouldly;
using Xunit;

namespace Noggog.Nuget.Tests;

public class MissingNugetOrgErrorTests
{
    [Theory, DefaultAutoData]
    public void EmptyPackageSourcesFix(
        FilePath path,
        [Frozen]MockFileSystem fs,
        MissingNugetOrgError sut)
    {
        fs.File.WriteAllText(path, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                                   "<configuration>" +
                                   "<packageSources>" +
                                   "</packageSources>" +
                                   "</configuration>");
        sut.RunFix(path);
        var doc = XDocument.Load(fs.FileStream.New(path, FileMode.Open, FileAccess.Read));
        doc.ShouldBeEquivalentTo(NotExistsError.TypicalFile());
    }
        
    [Theory, DefaultAutoData]
    public void MissingPackageSourcesFix(
        FilePath path,
        [Frozen]MockFileSystem fs,
        MissingNugetOrgError sut)
    {
        fs.File.WriteAllText(path, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                                   "<configuration>" +
                                   "</configuration>");
        sut.RunFix(path);
        var doc = XDocument.Load(fs.FileStream.New(path, FileMode.Open, FileAccess.Read));
        doc.ShouldBeEquivalentTo(NotExistsError.TypicalFile());
    }
        
    [Theory, DefaultAutoData]
    public void OtherPackageSourcesFix(
        FilePath path,
        [Frozen]MockFileSystem fs,
        MissingNugetOrgError sut)
    {
        fs.File.WriteAllText(path, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                                   "<configuration>" +
                                   "<packageSources>" +
                                   "<add key=\"CSharp Dev\" value=\"C:\\Repos\\CSharpExt\\Noggog.CSharpExt\\bin\\Debug\" />" +
                                   "</packageSources>" +
                                   "</configuration>");
        sut.RunFix(path);
        var doc = XDocument.Load(fs.FileStream.New(path, FileMode.Open, FileAccess.Read));
        var elem = new XElement("configuration",
            new XElement("packageSources",
                new XElement("add",
                    new XAttribute("key", "CSharp Dev"),
                    new XAttribute("value", "C:\\Repos\\CSharpExt\\Noggog.CSharpExt\\bin\\Debug")),
                new XElement("add",
                    new XAttribute("key", "nuget.org"),
                    new XAttribute("value", "https://api.nuget.org/v3/index.json"),
                    new XAttribute("protocolVersion", "3"))));
        var expected = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            elem);
        doc.ShouldBeEquivalentTo(expected);
    }
        
    [Theory, DefaultAutoData]
    public void NugetExistsDoesNothing(
        FilePath path,
        [Frozen]MockFileSystem fs,
        MissingNugetOrgError sut)
    {
        var txt = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                  "<configuration>" +
                  "<packageSources>" +
                  "<add key=\"nuget.org\" value=\"https://api.nuget.org/v3/index.json\" />" +
                  "</packageSources>" +
                  "</configuration>";
        fs.File.WriteAllText(path, txt);
        sut.RunFix(path);
        fs.File.ReadAllText(path)
            .ShouldBe(txt);
    }
}