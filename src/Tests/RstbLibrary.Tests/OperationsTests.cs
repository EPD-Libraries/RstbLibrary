using Revrs;

namespace RstbLibrary.Tests;

public class OperationsTests
{
    [Fact]
    public void ValidateReadRstb()
    {
        Rstb rstb = Rstb.FromBinary(Assets.RstbData);
        rstb.Version.Should().Be(RstbVersion.Fixed);
        rstb.Endianness.Should().Be(Endianness.Little);
        rstb.HashTable.Count.Should().Be(65816);
        rstb.OverflowTable.Count.Should().Be(2);
    }

    [Fact]
    public void ValidateReadRestbl()
    {
        Rstb restbl = Rstb.FromBinary(Assets.RestblData);
        restbl.Version.Should().Be(RstbVersion.Dynamic);
        restbl.Endianness.Should().Be(Endianness.Little);
        restbl.HashTable.Count.Should().Be(379715);
        restbl.OverflowTable.Count.Should().Be(32);
    }

    [Fact]
    public void ValidateWriteRstb()
    {
        byte[] rstbData = Assets.Rstb.ToBinary();

        Rstb rstb = Rstb.FromBinary(rstbData);
        rstb.Version.Should().Be(RstbVersion.Fixed);
        rstb.Endianness.Should().Be(Endianness.Little);
        rstb.HashTable.Count.Should().Be(65816);
        rstb.OverflowTable.Count.Should().Be(2);
    }

    [Fact]
    public void ValidateWriteRestbl()
    {
        byte[] restblData = Assets.Restbl.ToBinary();

        Rstb restbl = Rstb.FromBinary(restblData);
        restbl.Version.Should().Be(RstbVersion.Dynamic);
        restbl.Endianness.Should().Be(Endianness.Little);
        restbl.HashTable.Count.Should().Be(379715);
        restbl.OverflowTable.Count.Should().Be(32);
    }

    [Fact]
    public void ValidateWriteEmptyOverflowRestbl()
    {
        Rstb restbl = Rstb.FromBinary(Assets.RestblData);
        restbl.OverflowTable.Clear();
        byte[] restblData = restbl.ToBinary(Endianness.Little, RstbVersion.Dynamic);

        restbl = Rstb.FromBinary(restblData);
        restbl.Version.Should().Be(RstbVersion.Dynamic);
        restbl.Endianness.Should().Be(Endianness.Little);
        restbl.HashTable.Count.Should().Be(379715);
        restbl.OverflowTable.Count.Should().Be(0);
    }
}
