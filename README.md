# FsPackageTemplate

Project templates for F# open-source libraries. Clone one of the layouts below and rename to get started.

## Layouts

### single-package

For projects that publish a single NuGet package.

```
single-package/
  src/MyLib/          — library source
  tests/MyLib.Tests/  — xUnit v3 tests
  examples/ExampleApp/— example console app
  docs/               — fsdocs content
  MyLib.slnx          — solution file
```

### multi-package

For projects that publish multiple NuGet packages from one repo.

```
multi-package/
  src/MyLib/           — core library
  src/MyLib.Extension/ — extension package (references MyLib)
  src/MyLib.Cli/       — CLI tool (references MyLib)
  tests/MyLib.Tests/   — tests for all packages
  examples/ExampleApp/ — example console app
  docs/                — fsdocs content
  semantic-tagger.json — multi-package release config
  MyLib.slnx           — solution file
```

## Getting Started

1. Copy the layout you want into a new directory
2. Rename `MyLib` to your project name in all files (fsproj, slnx, CI, mise.toml, README)
3. Update `YOUR_NAME`, `YOUR_USER`, `YOUR_DESCRIPTION` placeholders in fsproj files
4. Initialize version control: `jj git init` or `git init`
5. Push to GitHub and the CI workflow will use reusable workflows from [michaelglass/MichaelsWackyFsPackageTools](https://github.com/michaelglass/MichaelsWackyFsPackageTools)

## What's Included

- **Build**: `dotnet build` via solution file
- **Test**: xUnit v3 with Microsoft Testing Platform v2, Unquote assertions, code coverage
- **Format**: Fantomas 7.x
- **Lint**: FSharpLint
- **Docs**: fsdocs with sync placeholders for README content
- **CI**: GitHub Actions using reusable workflows from MichaelsWackyFsPackageTools
- **Release**: Tag-triggered NuGet publishing with GitHub Releases
- **Tasks**: mise.toml for local dev commands (build, test, format, lint, docs, pack)

## Tooling Reference

The CI workflows reference [michaelglass/MichaelsWackyFsPackageTools](https://github.com/michaelglass/MichaelsWackyFsPackageTools) which provides:

- `reusable-build.yml` — build, test, format check, lint
- `reusable-release.yml` — NuGet pack and publish, GitHub Release
- `reusable-docs.yml` — fsdocs build and GitHub Pages deploy
