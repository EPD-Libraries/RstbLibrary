namespace RstbLibrary.Tests;

public static class Assets
{
    public static readonly byte[] RstbData = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Assets", "Rstb.test"));
    public static readonly Rstb Rstb = Rstb.FromBinary(RstbData);

    public static readonly byte[] RestblData = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Assets", "Restbl.test"));
    public static readonly Rstb Restbl = Rstb.FromBinary(RestblData);
}
