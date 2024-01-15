namespace RstbLibrary.Runner;

public static class Assets
{
    public static readonly byte[] RstbData = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Assets", "Rstb.bnchmrk"));
    public static readonly Rstb Rstb = Rstb.FromBinary(RstbData);

    public static readonly byte[] RestblData = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Assets", "Restbl.bnchmrk"));
    public static readonly Rstb Restbl = Rstb.FromBinary(RestblData);
}
