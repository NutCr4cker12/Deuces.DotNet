using BenchmarkDotNet.Attributes;
using Deuces;

namespace BenchMarks;

[Config(typeof(AntiVirusFriendlyConfig))]
public class LookupTest
{
    [Benchmark]
    public void CreateLookupTable()
    {
        var table = new LookupTable();
    }
}