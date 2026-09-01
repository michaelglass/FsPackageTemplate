# AGENTS.md

Guidance for AI coding agents working in this repo. `CLAUDE.md` points here.

## Project

MyLib — an F# repo publishing two NuGet packages. Scaffolded from
[FsPackageTemplate](https://github.com/michaelglass/FsPackageTemplate)
(`multi-package`), and on the shared tooling in
[MichaelsWackyFsPackageTools](https://github.com/michaelglass/MichaelsWackyFsPackageTools).

```
src/MyLib/                    core package
src/MyLib.Extension/          extension package (references MyLib)
src/MyLib.Cli/                CLI, not published; versioned with the core tag
tests/MyLib.Tests/            tests for MyLib
tests/MyLib.Extension.Tests/  tests for MyLib.Extension
examples/ExampleApp/          example console app (built by CI)
docs/                         fsdocs content, generated from the READMEs
```

**One test project per package.** That is not stylistic: CI derives a coverage
directory from each test project's folder name and then requires a matching
`coverage-ratchet-<name>.json` for every such config at the root. Adding a
package means adding its test project, its ratchet config and its CHANGELOG.

## Before you claim done

Run `mise run ci`. It runs, in CI's order: format check, docs sync check, build
with `--warnaserror`, FSharpLint, fsdocs, tests with coverage, the coverage
ratchet, and fsprojlint. Keep it in step with `.github/workflows/ci.yml` — a
local `ci` that runs a different set from real CI can go green on work CI will
reject.

`mise run check` is the same ground with auto-fix (it formats and syncs docs
rather than failing on them).

Don't skip format. Fantomas is strict; a wrapped line in the wrong place fails
CI.

## Task runner

`mise.toml`. The SDK is pinned there (`dotnet = "10"`) so a local run and CI
compile with the same compiler.

```
mise run build            mise run test              mise run test-coverage
mise run format           mise run format-check      mise run lint
mise run lint-project     mise run sync-docs         mise run sync-docs-check
mise run coverage-check   mise run coverage-ratchet  mise run coverage-loosen
mise run docs             mise run pack              mise run check / ci
mise run release          mise run release-dry-run   mise run release-alpha
mise run changelog-check
```

## CI

`.github/workflows/ci.yml` calls two reusable workflows from the shared tooling
repo — `michaels-wacky-build.yml` and `michaels-wacky-lint-project.yml`.

Two things to know before editing it:

- **Inputs are validated.** GitHub rejects a reusable-workflow call that passes
  an input the called workflow does not declare, and the run fails before
  anything compiles. Read the called workflow's `on.workflow_call.inputs` before
  adding one.
- **fsprojlint is a JOB, not just a mise task.** A gate that lives only in the
  task runner is not on the path CI executes. That is exactly how nine repos in
  this family shipped four failing fsprojlint checks unnoticed for months.
  If you add a check, add it to both.

## Coverage

CoverageRatchet enforces per-file floors from `coverage-ratchet-MyLib.json` and `coverage-ratchet-MyLib.Extension.json`.
**A file with no entry defaults to 100% line and 100% branch**, not to a weaker
fallback — so an empty `"overrides"` object is a strict gate, not an inert one.

- `mise run coverage-check` — run tests and check the floors.
- `mise run coverage-ratchet` — tighten floors after coverage improves.
- `mise run coverage-loosen` — add an entry for a file that legitimately cannot
  reach 100% (a defensive branch, a CLI entry point). Always write a `reason`.

The coverage directory name is load-bearing: CI derives it from the test project
folder minus the `.Tests` suffix, then looks for `coverage-ratchet-<name>.json`
at the repo root. Keep one ratchet config per test project, named to match — a
config with no matching test project fails CI's ratchet step.

## Docs

`syncdocs` copies marked sections out of `README.md` into `docs/index.md`
(`README.md` → `docs/index.md`; `src/<Project>/README.md` → `docs/<Project>/index.md`).
The README is the authoritative copy. Run `mise run sync-docs` after editing a
`<!-- sync:... -->` block; CI fails on drift.

## Packing and releasing

- `mise run pack` produces a **ref-stamped** package: RefStamp (wired in the root
  `Directory.Build.props`) suffixes a local pack's version with the jj/git source
  ref, so a dev machine cannot produce a release-shaped version. Only the release
  pipeline (`-p:ReleaseBuild=true`) gets the clean version.
- `mise run release` runs `ci` and then `fssemantictagger release`, which derives
  each changed package's bump level from its public API diff, bumps `<Version>`,
  promotes that package's `## Unreleased` CHANGELOG section, commits, tags and
  pushes. A multi-package repo keeps **one CHANGELOG.md per package directory**
  (`src/MyLib/CHANGELOG.md`, `src/MyLib.Extension/CHANGELOG.md`), each needing a
  non-empty `## Unreleased` section — `mise run changelog-check` tells you.
- Tag prefixes are per package (`core-v`, `extension-v`), configured in
  `semantic-tagger.json`. `fsProjsSharingSameTag` there lists further `.fsproj`
  files whose `<Version>` moves with that package's tag — that is the key
  fssemantictagger reads; a differently-named key is silently ignored.
- The tag triggers `.github/workflows/release.yml`: the shared workflow packs
  and creates the GitHub Release, then a per-package `publish` job **in this repo** exchanges
  an OIDC token for a NuGet key (`nuget/login`) and pushes. That job must live
  here, not in the reusable workflow: NuGet Trusted Publishing checks the token's
  `job_workflow_ref` against the calling repo's own workflow file. Set the
  `NUGET_USER` repository variable and register this repo as a trusted publisher
  for each package on nuget.org.

## Version control

jj (Jujutsu). `jj describe -m "..."` — always with `-m`; without it jj opens
`$EDITOR` and waits. `jj new` starts a new change. A **non-colocated** checkout
(`jj git init`, no `.git` at the root) is supported: the root
`Directory.Build.props` disables SourceLink and the SCM queries when `.git` is
absent, which is what lets the repo pack itself locally.
