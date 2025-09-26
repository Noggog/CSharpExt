# Development Notes

This file contains helpful information for developers and AI assistants working on this codebase.

## Development Commands

### Building
- Build entire solution: `dotnet build -c Release`
- Build specific project: `dotnet build <ProjectName>/<ProjectName>.csproj -c Release`
- Clean and restore: `dotnet clean -c Release && dotnet nuget locals all --clear && dotnet restore`

### Testing
- Run all tests: `dotnet test CSharpExt.UnitTests/CSharpExt.UnitTests.csproj -c Release`
- Run specific test project: `dotnet test <TestProject>/<TestProject>.csproj -c Release`

### Packaging
- Packages are automatically generated in `/nupkg` directory when building with `GeneratePackageOnBuild=true`
- Package versions managed centrally via `Directory.Packages.props`

### Releases
- Create release tags using semantic versioning format: `<major>.<minor>.<patch>`
- Always include the patch number, even if it's zero (e.g., `3.1.0`, not `3.1`)
- **Do not prefix with `v`** (e.g., use `3.1.0`, not `v3.1.0`)
- This format is required for GitVersion compatibility

#### Creating GitHub Release Drafts
1. Find the last release tag: `git tag --sort=-version:refname`
2. Get commits since last release: `git log --oneline <last-tag>..HEAD`
3. Construct release notes by categorizing commits:
   - **Enhancements**: New features, performance improvements, major changes
   - **Bug Fixes**: Bug fixes and corrections
   - **Testing & Documentation**: Test additions, documentation updates
4. Create draft release: `gh release create <version> --draft --title "<version>" --notes "<release-notes>"`
5. Include full changelog link: `**Full Changelog**: https://github.com/Noggog/CSharpExt/compare/<last-tag>...<new-tag>`

### Benchmarking
- Run benchmarks: `dotnet run --project CSharpExt.Benchmark/CSharpExt.Benchmark.csproj -c Release`

## Architecture Overview

This is a multi-project .NET solution focused on providing generic reusable C# extensions and utilities. The repository follows a modular structure with distinct projects for different concerns:

### Core Projects
- **Noggog.CSharpExt**: Main library containing generic extension methods and utilities
  - Multi-targeted: .NET 8, .NET 9, .NET Standard 2.0
  - Organized by category: Containers, Extensions, IO, Reactive, Streams, Structs, etc.
  - Uses unsafe code blocks for performance-critical operations
- **Noggog.CSharpExt.Json**: JSON-specific extensions and utilities
- **Noggog.CSharpExt.Windows**: Windows-specific functionality (excluded from Unix builds)
- **Noggog.WPF**: WPF-specific extensions and controls (Windows-only)
- **Noggog.Autofac**: Autofac dependency injection extensions
- **Noggog.Testing**: Testing utilities and helpers
- **Noggog.SourceGenerators**: Source generators for code generation
- **Noggog.Nuget**: NuGet package management utilities

### Test Projects
- **CSharpExt.UnitTests**: Comprehensive unit tests using xUnit, AutoFixture, NSubstitute, Shouldly
- **Noggog.Nuget.Tests**: Tests for NuGet utilities
- **CSharpExt.Benchmark**: Performance benchmarks using BenchmarkDotNet

### Key Technologies and Patterns
- **Reactive Extensions (Rx.NET)**: Heavy use of observables and reactive patterns
- **DynamicData**: For reactive collections and data management
- **System.IO.Abstractions**: For testable file system operations
- **Source Generators**: Custom code generation for performance and developer experience
- **Central Package Management**: All package versions managed in `Directory.Packages.props`

### Build Configuration
- Uses Directory.Build.props for shared MSBuild properties
- Nullable reference types enabled with warnings as errors
- Documentation generation enabled for all projects
- Source Link integration for debugging
- EditorConfig with specific C# analyzer rules
- Cross-platform CI/CD via GitHub Actions (excludes WPF/Windows projects on Unix)

### Code Standards
- C# preview language features enabled
- Implicit usings enabled
- Comprehensive XML documentation expected (though not enforced as errors)
- Async/await patterns used extensively
- Task-based asynchronous programming preferred over blocking calls

### Testing Strategy
- Unit tests with high coverage using xUnit framework
- Property-based testing with AutoFixture
- Mocking with NSubstitute
- Snapshot testing with Verify
- Cross-platform testing (excluding Windows-specific projects)
- Source generator testing with dedicated testing framework

The codebase emphasizes performance, cross-platform compatibility (where applicable), and developer productivity through extensive use of modern .NET features and reactive programming patterns.