# Resource Size Table Library

Simple Resource Size Table (RSTB/RESTBL) IO library written in modern C#

[![License](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/EPD-Libraries/RstbLibrary/blob/master/License.md) [![Downloads](https://img.shields.io/github/downloads/EPD-Libraries/RstbLibrary/total)](https://github.com/EPD-Libraries/RstbLibrary/releases)

## Usage

> [!NOTE]
> `Rstb` and `ImmutableRstb` support both RSTB (`Fixed`) and RESTBL (`Dynamic`) files.

### Read

```cs
byte[] data = File.ReadAllBytes("ResourceSizeTable.Product.121.rsizetable");
Rstb rstb = Rstb.FromBinary(data);
```

### Read Immutable (Readonly, but much faster)

```cs
byte[] data = File.ReadAllBytes("ResourceSizeTable.Product.121.rsizetable");
RevrsReader reader = new(data);
ImmutableRstb rstb = new(ref reader);
```

### Write

```cs
/* ... */

using FileStream fs = File.Create("ResourceSizeTable.Product.121.rsizetable");
rstb.WriteBinary(
    // The stream to write into
    fs,

    // Write in LE (uses source by default)
    endianness: Endianness.Little,

    // Write a dynamic Rstb (RESTBL) instead of Fixed (RSTB)
    version: RstbVersion.Dynamic,

    // Remove any extra entries in the OverflowTable to save space
    optimize: true)
```

### To Binary

```cs
/* ... */

byte[] outputData = rstb.ToBinary(
    /*
     *  Takes the same arguments as WriteBinary(stream, ...)
     */
);
```

# Benchmarks

> [!NOTE]
> `Rstb` methods read/write the BotW RSTB (515 KB) and `Restbl` methods read/write the TotK RESTBL (2,972 KB).

### RSTB

| Method        |     Mean |     Error |    StdDev |     Gen0 |     Gen1 |     Gen2 | Allocated |
| ------------- | -------: | --------: | --------: | -------: | -------: | -------: | --------: |
| Read          | 4.907 ms | 0.0245 ms | 0.0230 ms | 195.3125 | 164.0625 |        - |   3.01 MB |
| ReadImmutable | 8.743 ns | 0.0147 ns | 0.0115 ns |        - |        - |        - |         - |
| Write         | 1.814 ms | 0.0234 ms | 0.0219 ms | 130.8594 | 123.0469 | 123.0469 |   3.51 MB |

### RESTBL

| Method        |      Mean |     Error |    StdDev |      Gen0 |     Gen1 |     Gen2 | Allocated |
| ------------- | --------: | --------: | --------: | --------: | -------: | -------: | --------: |
| Read          | 36.337 ms | 0.6874 ms | 0.8183 ms | 1142.8571 | 928.5714 |        - |  17.39 MB |
| ReadImmutable | 15.353 ns | 0.0409 ns | 0.0342 ns |         - |        - |        - |         - |
| Write         | 11.625 ms | 0.2173 ms | 0.1814 ms |  578.1250 | 578.1250 | 578.1250 |   16.7 MB |

## Install

[![NuGet](https://img.shields.io/nuget/v/RstbLibrary.svg)](https://www.nuget.org/packages/RstbLibrary) [![NuGet](https://img.shields.io/nuget/dt/RstbLibrary.svg)](https://www.nuget.org/packages/RstbLibrary)

#### NuGet
```powershell
Install-Package RstbLibrary
```

#### Build From Source
```sh
git clone https://github.com/EPD-Libraries/RstbLibrary.git
dotnet build RstbLibrary
```