## Advent of Code Template

This repository is a batteries-included starter for solving Advent of Code in **.NET 10**, **TypeScript (Bun)**, or **Python 3**. All stacks share the same mental model: drop your puzzle inputs in the `Data` folders, inherit from a `SolutionBase`, and call a loader helper (`LoadLines`, `LoadGrid`, etc.) inside `Solve()`. Point the runner at a particular day/part via `appsettings.json` and iterate quickly with `dotnet run`, `bun run`, or `python main.py`.

### Repo Usage

The easiest way to utilise this repo would be to make a fork to host yourself, as then you will be able to use source control for your solutions (and share them with others if you choose!).

### Repo Layout

```
├── appsettings.json          # Day/part/run-type selector shared by both runners
├── dotnet/                   # .NET 10 console app
│   ├── Program.cs            # Bootstraps DI + runner
│   ├── Helpers/              # Debug/Extensions utilities
│   ├── Models/               # Grid + SolutionOptions
│   ├── Services/             # FileLoader + SolutionRunner
│   └── Solutions/            # One folder per day (data + solution class)
├── typescript/               # Bun + TypeScript implementation
│   ├── index.ts              # Entry point
│   ├── Helpers/, Models/, Services/ (TS twins of the .NET pieces)
│   └── Solutions/            # Mirrored structure for TS solvers
├── python/                   # Python 3 runner
│   ├── main.py               # Entry point
│   ├── helpers/, models/, services/ (Py twins)
│   ├── solutions/            # Solver classes
│   └── Solutions/            # Data directories consumed by the Python loader
├── EXAMPLE.md                # Day 1 (2024) puzzle write-up for Day100 example
└── Makefile                  # Convenience commands for build/run in each stack
```

Every day folder (e.g., `dotnet/Solutions/Day07/`) contains:

- `Data/Test1.txt`, `Data/Test2.txt`, `Data/Full1.txt`, `Data/Full2.txt` – drop AoC input here.
- `Day07.cs` or `Day07.ts` – your solver for that part/day.

Day 100 is wired up as an end-to-end example in both languages and described in `EXAMPLE.md`.

### Configuring Which Puzzle to Run

Edit the root `appsettings.json`:

```json
{
  "Solution": {
    "Day": "7",
    "SolutionType": "Second",   // First or Second part
    "RunType": "Full"           // Test uses Test*.txt, Full uses Full*.txt
  }
}
```

The runners read this file to decide which solution class to execute and whether to load the test or full input set.

### Running the .NET Solutions

Prerequisites: .NET 10 SDK.

```bash
# Quick build
make dev-dotnet

# Run the currently selected puzzle
make run-dotnet
# or manually
cd dotnet && dotnet run
```

`Program.cs` discovers every `ISolution` implementation via DI, so adding a new `DayXX` class is enough—no manual registration required. `SolutionRunner` prints which day/part/run-mode is executing and runs either one puzzle (`Run`) or all (`RunAll`).

### Running the TypeScript Solutions

Prerequisites: [Bun](https://bun.sh). The Makefile installs Bun (globally) and project deps for you.

```bash
# Install deps (first run)
make dev-typescript

# Execute the selected puzzle
make run-typescript
# or manually
cd typescript && bun install && bun run index.ts
```

The TypeScript runner mirrors the .NET one: it builds a list of classes implementing `ISolution`, respects the same `appsettings.json`, and outputs answers to stdout.

### Running the Python Solutions

Prerequisites: Python 3.11+ (earlier versions should work too) and optionally a virtual environment.

```bash
# (Optional) create a venv and install dependencies
make dev-python

# Execute the selected puzzle
make run-python
# or manually
cd python && (. .venv/bin/activate 2>/dev/null || true) && python main.py
```

The Python runner behaves just like the others: it builds every `SolutionBase` descendant, honours `appsettings.json`, and prints the selected answer.

### Writing a New Solution (.NET)

1. Copy `dotnet/Solutions/DayNN/DayNN.cs` from a previous day or create a new folder.
2. Inherit from `SolutionBase`:

   ```csharp
   public sealed class Day07(IFileLoader loader, IOptions<SolutionOptions> options)
       : SolutionBase(loader, options)
   {
       public override int Day => 7;

       public override async Task<string> Solve()
       {
           var rows = await LoadLines<int>();
           // TODO: solve puzzle
           return result.ToString();
       }
   }
   ```

3. Use the built-in loaders based on the shape you need:
   - `LoadRaw<T>()` – entire file as a single value.
   - `LoadLines<T>()` – typed lines.
   - `LoadGrid<T>()` – 2D grid helper (`Grid<T>` has `Get`, `FindConnectedRegion`, etc.).
   - `LoadItems<T>()` – splits each line on whitespace into typed tuples.

`CurrentOptions` exposes the active `SolutionOptions`, so you can branch on `SolutionType` for part 1 vs 2 without re-reading config.

### Writing a New Solution (TypeScript)

1. Create `typescript/Solutions/DayNN/DayNN.ts`.
2. Extend the `SolutionBase` class:

   ```ts
   import { SolutionBase, type SolutionDependencies } from "../SolutionBase.ts";

   class Day07 extends SolutionBase {
     public readonly day = 7;

     public constructor(deps: SolutionDependencies) {
       super(deps);
     }

     public async solve(): Promise<string> {
       const rows = await this.loadLines<number>();
       // TODO: solve puzzle
       return result.toString();
     }
   }
   ```

3. Available helpers mirror the .NET versions:
   - `loadRaw()` returns the raw file contents.
   - `loadLines<T>()`, `loadGrid<T>()`, `loadItems<T>()` return typed structures.
   - `this.options` exposes `day`, `solutionType`, and `runType`.

Registering the class is automatic—just export it from its file.

### Writing a New Solution (Python)

1. Create `python/solutions/dayNN/dayNN.py`.
2. Extend the Python `SolutionBase`:

   ```py
   from solutions import SolutionBase


   class Day07(SolutionBase):
       @property
       def day(self) -> int:
           return 7

       def solve(self) -> str:
           rows = self.load_lines(int)
           # TODO: puzzle logic
           return str(result)
   ```

3. The helper surface matches the other stacks: `load_raw`, `load_lines`, `load_grid`, `load_items`, plus `current_options` for branching between parts.

Solutions register themselves by virtue of being imported in `python/main.py`.

### Logging & Utilities

- **Debug helpers:** `dotnet/Helpers/Debug.cs`, `typescript/Helpers/Debug.ts`, and `python/helpers/debug.py` each expose simple `writeLine`/`write_nested` helpers for dumping iterables while iterating on solutions.
- **Grid model:** every stack ships a `Grid<T>`/`Grid` type with convenience methods (`get`, `findItems`, `findConnectedRegion`, `calculateRegionPerimeter`, etc.) for 2D puzzles.
- **Extensions / helpers:** shared utility routines live under `Helpers/Extensions.*` (C#), `Helpers/Extensions.ts`, and Python’s helper modules if you need grouped iteration patterns.

### Example Puzzle (Day 100)

The `Day100` folder in each language shows a complete implementation for 2024 Day 1, including data files (`Test*.txt`, `Full*.txt`) and narrative (`EXAMPLE.md`). Use it as a reference when onboarding new folks.

### Suggested Workflow

1. Update `appsettings.json` to point at the day/part you’re tackling.
2. Drop AoC inputs into the corresponding `Data` files (use `Test` for sample inputs, `Full` for your full dataset).
3. Implement/modify the solver in `.cs`, `.ts`, or `.py`.
4. Run via `make run-dotnet`, `make run-typescript`, or `make run-python`.
5. Use the Debug helpers to inspect data structures or intermediate states.

### Language Requirements

The templates target .NET 10, Bun + TypeScript 5, and Python 3.11+ by default, but those are not hard requirements. You can change the target framework in the `.csproj`, switch to your preferred Node package manager, or pin a different Python version/virtual environment strategy if desired.

Happy coding!
