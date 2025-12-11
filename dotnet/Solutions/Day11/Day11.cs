using AdventOfCode.Models;
using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions;

public class Day11(IFileLoader fileLoader, IOptions<SolutionOptions> options) : SolutionBase(fileLoader, options)
{
    public override int Day => 11;

    public override async Task<string> Solve()
    {
        var lines = await LoadLines<string>();
        var devices = lines
            .Select(line => line.Split(':'))
            .ToDictionary(
                x => x[0].Trim(),
                x => x[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                         .Select(y => y.Trim())
                         .ToList()
            );

        var start = CurrentOptions.SolutionType is SolutionType.First ? "you" : "svr";
        var requireFftDac = CurrentOptions.SolutionType is SolutionType.Second;

        long paths = CountPaths(devices, start, requireFftDac);

        return paths.ToString();
    }

    private static long CountPaths(
        IReadOnlyDictionary<string, List<string>> devices,
        string start,
        bool requireFftDac)
    {
        // Build a stable index and attempt a DAG DP first (fastest).
        var names = new HashSet<string>(devices.Keys);
        foreach (var list in devices.Values)
            foreach (var name in list)
                names.Add(name);
        names.Add("out");
        names.Add(start);

        var ordered = names.OrderBy(x => x).ToList();
        var index = ordered.Select((name, i) => (name, i)).ToDictionary(x => x.name, x => x.i);
        var adj = new List<int>[ordered.Count];
        for (var i = 0; i < adj.Length; i++) adj[i] = [];

        foreach (var (name, neighbours) in devices)
        {
            var from = index[name];
            foreach (var n in neighbours)
            {
                if (!index.TryGetValue(n, out var to)) continue;
                adj[from].Add(to);
            }
        }

        var startIdx = index[start];
        var outIdx = index["out"];

        // Detect cycles; if cyclic, fall back to memoised DFS.
        var color = new byte[adj.Length]; // 0=unvisited,1=visiting,2=done
        bool hasCycle = false;
        bool DfsCycle(int node)
        {
            if (color[node] == 1) return true;
            if (color[node] == 2) return false;
            color[node] = 1;
            foreach (var n in adj[node])
            {
                if (DfsCycle(n))
                {
                    return hasCycle = true;
                }
            }
            color[node] = 2;
            return false;
        }
        DfsCycle(startIdx);

        if (!hasCycle)
        {
            // DAG DP with 2-bit flags (fft, dac).
            const int fftBit = 1;
            const int dacBit = 2;
            var nodeFlag = new int[ordered.Count];
            for (var i = 0; i < ordered.Count; i++)
            {
                if (ordered[i] == "fft") nodeFlag[i] |= fftBit;
                if (ordered[i] == "dac") nodeFlag[i] |= dacBit;
            }

            // Topological order (reverse finishing times).
            var topo = new List<int>(ordered.Count);
            Array.Fill(color, (byte)0);
            void DfsTopo(int node)
            {
                if (color[node] == 2) return;
                color[node] = 1;
                foreach (var n in adj[node]) DfsTopo(n);
                color[node] = 2;
                topo.Add(node);
            }
            DfsTopo(startIdx);

            // dp[node][mask] -> count
            var dp = new long[ordered.Count, 4];
            dp[outIdx, nodeFlag[outIdx]] = 1;

            foreach (var node in topo)
            {
                if (adj[node].Count == 0 && node != outIdx) continue;
                foreach (var child in adj[node])
                {
                    for (var childMask = 0; childMask < 4; childMask++)
                    {
                        var ways = dp[child, childMask];
                        if (ways == 0) continue;
                        var combinedMask = childMask | nodeFlag[node];
                        dp[node, combinedMask] += ways;
                    }
                }
            }

            if (!requireFftDac)
            {
                long total = 0;
                for (var mask = 0; mask < 4; mask++)
                {
                    total += dp[startIdx, mask];
                }
                return total;
            }
            else
            {
                return dp[startIdx, 3];
            }
        }

        // Fallback: memoised DFS with HashSet, preserving simple-path semantics.
        return DfsWithHashSet(
            start,
            devices,
            requireFftDac,
            new HashSet<string>(),
            seenFft: start == "fft",
            seenDac: start == "dac");
    }

    private static long DfsWithHashSet(
        string node,
        IReadOnlyDictionary<string, List<string>> devices,
        bool requireFftDac,
        HashSet<string> visited,
        bool seenFft,
        bool seenDac)
    {
        if (node == "out")
        {
            if (!requireFftDac) return 1;
            return (seenFft && seenDac) ? 1 : 0;
        }

        if (!devices.TryGetValue(node, out var neighbours) || neighbours.Count == 0)
            return 0;

        visited.Add(node);
        long total = 0;

        foreach (var next in neighbours)
        {
            if (visited.Contains(next)) continue;

            var nextSeenFft = seenFft || next == "fft";
            var nextSeenDac = seenDac || next == "dac";

            total += DfsWithHashSet(next, devices, requireFftDac, visited, nextSeenFft, nextSeenDac);
        }

        visited.Remove(node);
        return total;
    }
}