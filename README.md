# FsPackageTemplate

Project templates for F# open-source libraries. Copy one of the layouts below,
rename, and you have a repo that builds green in CI and passes `fsprojlint`
17/17 (29/29 for multi-package) on day one.

## Layouts

### single-package

For projects that publish a single NuGet package.

```
single-package/
  src/MyLib/            — library source
  tests/MyLib.Tests/    — xUnit v3 tests
  examples/ExampleApp/  — example console app (built by CI)
  docs/                 — fsdocs content; index.md is synced from README.md
  MyLib.slnx            — solution file
  Directory.Build.props — RefStamp pack guard + non-colocated-jj SourceLink guard
  LICENSE, CHANGELOG.md, AGENTS.md, CLAUDE.md
  coverage-ratchet-MyLib.json — per-file coverage floors
```

### multi-package

For projects that publish multiple NuGet packages from one repo.

```
multi-package/
  src/MyLib/                   — core library
  src/MyLib.Extension/         — extension package (references MyLib)
  src/MyLib.Cli/               — CLI, not published; versioned with the core tag
  tests/MyLib.Tests/           — tests for MyLib
  tests/MyLib.Extension.Tests/ — tests for MyLib.Extension
  examples/ExampleApp/         — example console app (built by CI)
  docs/                        — fsdocs content, synced from the READMEs
  semantic-tagger.json         — per-package tag prefixes
  MyLib.slnx                   — solution file
  Directory.Build.props        — RefStamp pack guard + non-colocated-jj SourceLink guard
  LICENSE, AGENTS.md, CLAUDE.md, src/*/CHANGELOG.md
  coverage-ratchet-MyLib.json, coverage-ratchet-MyLib.Extension.json
```

**One test project per package** is a hard requirement, not a style: CI derives a
coverage directory from each test project's folder name minus `.Tests`, then runs
the coverage ratchet for every `coverage-ratchet-*.json` at the root against
`coverage/<name>/`. A ratchet config with no matching test project fails CI.

## Getting Started

1. Copy the layout you want into a new directory.
2. Rename `MyLib` to your project name — in file and directory names as well as
   file contents (fsproj, slnx, coverage-ratchet-*.json, CI workflows, mise.toml,
   semantic-tagger.json, README, AGENTS.md, docs/).
3. Replace the `YOUR_NAME`, `YOUR_USER` and `YOUR_DESCRIPTION` placeholders in the
   fsproj files, and the copyright holder in `LICENSE`.
4. Initialize version control: `jj git init` or `git init`. A non-colocated jj
   checkout (no `.git` at the root) is supported and can still `mise run pack`.
5. `mise run ci` — it should be green before you push anything.
6. Push to GitHub. Set the `NUGET_USER` repository variable and register the repo
   as a trusted publisher on nuget.org before the first release.

## What's Included

- **Build**: `dotnet build` via the solution file, `--warnaserror` in CI and in
  `mise run ci`. SDK pinned in `mise.toml`, `global.json` and `ci.yml`.
- **Test**: xUnit v3 with Microsoft Testing Platform v2, Unquote assertions.
- **Coverage**: per-file floors via CoverageRatchet, emitted and checked locally
  by `mise run coverage-check` and enforced in CI. A file with no entry defaults
  to 100% line and 100% branch.
- **Format**: Fantomas 8.x.
- **Lint**: FSharpLint (source) and FsProjLint (repo + fsproj structure) — the
  latter both as `mise run lint-project` and as its own CI job, because a gate
  that lives only in the task runner is not on the path CI executes.
- **Docs**: fsdocs, with `syncdocs` keeping `docs/` in lockstep with the READMEs.
- **CI**: GitHub Actions calling reusable workflows from
  [michaelglass/MichaelsWackyFsPackageTools](https://github.com/michaelglass/MichaelsWackyFsPackageTools).
- **Release**: tag-triggered pack + GitHub Release via the shared workflow, then a
  `publish` job in the generated repo that does the NuGet Trusted Publishing
  credential exchange (it cannot live in the reusable workflow — the OIDC token's
  `job_workflow_ref` must match the calling repo's own workflow file).
- **Pack guard**: RefStamp suffixes a local `dotnet pack` version with the jj/git
  source ref, so a dev machine cannot produce a release-shaped version.
- **Tasks**: `mise.toml` — build, test, coverage, format, lint, docs, pack,
  release, changelog.
- **Agent guide**: `AGENTS.md` (+ a `CLAUDE.md` pointer).

## Tooling Reference

The CI workflows reference
[michaelglass/MichaelsWackyFsPackageTools](https://github.com/michaelglass/MichaelsWackyFsPackageTools),
which provides:

- `michaels-wacky-build.yml` — format check, docs sync check, build, lint, docs,
  test with coverage, coverage ratchet, example build
- `michaels-wacky-lint-project.yml` — FsProjLint
- `michaels-wacky-release.yml` — pack, GitHub Release, NuGet artifact upload
- `michaels-wacky-docs.yml` — fsdocs build and GitHub Pages deploy

Reusable-workflow inputs are validated by GitHub: passing a name the called
workflow does not declare fails the run before anything compiles. Read that
workflow's `on.workflow_call.inputs` before adding one to a call here.
