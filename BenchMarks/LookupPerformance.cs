using BenchmarkDotNet.Attributes;
using Deuces;

namespace BenchMarks;

[Config(typeof(AntiVirusFriendlyConfig))]
public class LookupPerformance
{
    [Benchmark]
    public void CreateLookupTable()
    {
        var table = new StdLookupTable();
    }
}