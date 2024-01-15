using System.Runtime.InteropServices.Marshalling;

namespace RstbLibrary.Extensions;

public static class Utf8StringExtensions
{
    public unsafe static string ToManaged(this Span<byte> data)
    {
        fixed (byte* ptr = data) {
            return Utf8StringMarshaller.ConvertToManaged(ptr)
                ?? string.Empty;
        }
    }
}
