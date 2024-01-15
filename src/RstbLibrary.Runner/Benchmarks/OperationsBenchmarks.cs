using BenchmarkDotNet.Attributes;
using Revrs;

namespace RstbLibrary.Runner.Benchmarks;

[MemoryDiagnoser(true)]
public class OperationsBenchmarks
{
    [Benchmark]
    public void ReadRstb()
    {
        Rstb _ = Rstb.FromBinary(Assets.RstbData);
    }

    [Benchmark]
    public void ReadRstbImmutable()
    {
        RevrsReader reader = new(Assets.RstbData);
        ImmutableRstb _ = new(ref reader);
    }

    [Benchmark]
    public void WriteRstb()
    {
        byte[] _ = Assets.Rstb.ToBinary();
    }

    [Benchmark]
    public void ReadRestbl()
    {
        Rstb _ = Rstb.FromBinary(Assets.RestblData);
    }

    [Benchmark]
    public void ReadRestblImmutable()
    {
        RevrsReader reader = new(Assets.RestblData);
        ImmutableRstb _ = new(ref reader);
    }

    [Benchmark]
    public void WriteRestbl()
    {
        byte[] _ = Assets.Restbl.ToBinary();
    }
}
