``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.765 (1803/April2018Update/Redstone4)
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=3984652 Hz, Resolution=250.9629 ns, Timer=TSC
.NET Core SDK=2.1.700-preview-009618
  [Host]     : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT


```
|           Method |      Mean |     Error |    StdDev |    Median |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------- |----------:|----------:|----------:|----------:|-------:|------:|------:|----------:|
|              Get | 11.675 ns | 0.2231 ns | 0.2087 ns | 11.644 ns |      - |     - |     - |         - |
|         GetLarge | 11.905 ns | 0.2501 ns | 0.2780 ns | 11.868 ns |      - |     - |     - |         - |
|  GetOffsetAmount | 12.455 ns | 0.2782 ns | 0.7378 ns | 12.164 ns |      - |     - |     - |         - |
|        GetOffset | 12.460 ns | 0.2611 ns | 0.3006 ns |         - |      - |     - |     - |         - |
|             Read | 15.303 ns | 0.0997 ns | 0.0833 ns | 15.309 ns |      - |     - |     - |         - |
|        ReadLarge | 15.564 ns | 0.1355 ns | 0.1131 ns | 15.527 ns |      - |     - |     - |         - |
| ReadOffsetAmount | 15.534 ns | 0.1126 ns | 0.0941 ns | 15.543 ns |      - |     - |     - |         - |
|         GetBytes | 14.980 ns | 0.1969 ns | 0.1842 ns | 14.965 ns | 0.0095 |     - |     - |      40 B |
|        ReadBytes | 18.106 ns | 0.1573 ns | 0.1228 ns | 18.112 ns | 0.0095 |     - |     - |      40 B |
|          GetBool |  1.891 ns | 0.0209 ns | 0.0195 ns |  1.888 ns |      - |     - |     - |         - |
|    GetBoolOffset |  1.703 ns | 0.0420 ns | 0.0372 ns |  1.717 ns |      - |     - |     - |         - |
|         ReadBool |  4.816 ns | 0.0641 ns | 0.0599 ns |  4.821 ns |      - |     - |     - |         - |
|         GetUInt8 |  1.441 ns | 0.0459 ns | 0.0429 ns |  1.439 ns |      - |     - |     - |         - |
|   GetUInt8Offset |  1.438 ns | 0.0361 ns | 0.0320 ns |  1.420 ns |      - |     - |     - |         - |
|        ReadUInt8 |  4.930 ns | 0.0649 ns | 0.0575 ns |  4.936 ns |      - |     - |     - |         - |
|        GetUInt16 |  2.788 ns | 0.0693 ns | 0.0614 ns |  2.789 ns |      - |     - |     - |         - |
|  GetUInt16Offset |  2.836 ns | 0.0813 ns | 0.0835 ns |  2.799 ns |      - |     - |     - |         - |
|       ReadUInt16 |  6.119 ns | 0.1310 ns | 0.1162 ns |  6.126 ns |      - |     - |     - |         - |
|        GetUInt32 |  2.668 ns | 0.0787 ns | 0.0808 ns |  2.647 ns |      - |     - |     - |         - |
|  GetUInt32Offset |  2.646 ns | 0.0676 ns | 0.0632 ns |  2.647 ns |      - |     - |     - |         - |
|       ReadUInt32 |  5.816 ns | 0.1118 ns | 0.0991 ns |  5.826 ns |      - |     - |     - |         - |
|        GetUInt64 |  2.536 ns | 0.0396 ns | 0.0331 ns |  2.530 ns |      - |     - |     - |         - |
|  GetUInt64Offset |  2.794 ns | 0.0817 ns | 0.0908 ns |  2.774 ns |      - |     - |     - |         - |
|       ReadUInt64 |  5.947 ns | 0.1311 ns | 0.1162 ns |  5.927 ns |      - |     - |     - |         - |
|          GetInt8 |  1.667 ns | 0.0472 ns | 0.0442 ns |  1.675 ns |      - |     - |     - |         - |
|    GetInt8Offset |  1.750 ns | 0.0619 ns | 0.1705 ns |  1.685 ns |      - |     - |     - |         - |
|         ReadInt8 |  4.658 ns | 0.1031 ns | 0.0861 ns |  4.627 ns |      - |     - |     - |         - |
|         GetInt16 |  2.277 ns | 0.0566 ns | 0.0502 ns |  2.251 ns |      - |     - |     - |         - |
|   GetInt16Offset |  2.314 ns | 0.0569 ns | 0.0505 ns |  2.306 ns |      - |     - |     - |         - |
|        ReadInt16 |  5.779 ns | 0.0790 ns | 0.0700 ns |  5.767 ns |      - |     - |     - |         - |
|         GetInt32 |  2.629 ns | 0.0774 ns | 0.0795 ns |  2.609 ns |      - |     - |     - |         - |
|   GetInt32Offset |  2.612 ns | 0.0609 ns | 0.0570 ns |  2.611 ns |      - |     - |     - |         - |
|        ReadInt32 |  5.988 ns | 0.1412 ns | 0.1387 ns |  5.950 ns |      - |     - |     - |         - |
|         GetInt64 |  2.565 ns | 0.0709 ns | 0.0663 ns |  2.601 ns |      - |     - |     - |         - |
|   GetInt64Offset |  2.400 ns | 0.0652 ns | 0.0610 ns |  2.388 ns |      - |     - |     - |         - |
|        ReadInt64 |  5.733 ns | 0.1372 ns | 0.1409 ns |  5.681 ns |      - |     - |     - |         - |
|        GetString | 20.216 ns | 0.4478 ns | 0.4189 ns | 20.257 ns | 0.0210 |     - |     - |      88 B |
|  GetStringOffset | 20.264 ns | 0.4319 ns | 0.4241 ns | 20.046 ns | 0.0210 |     - |     - |      88 B |
|       ReadString | 25.098 ns | 0.5646 ns | 0.5545 ns | 24.898 ns | 0.0210 |     - |     - |      88 B |
|         GetFloat |  3.091 ns | 0.0776 ns | 0.0726 ns |  3.072 ns |      - |     - |     - |         - |
|   GetFloatOffset |  3.102 ns | 0.0770 ns | 0.0720 ns |  3.074 ns |      - |     - |     - |         - |
|        ReadFloat |  6.486 ns | 0.1528 ns | 0.1569 ns |  6.450 ns |      - |     - |     - |         - |
|        GetDouble |  2.882 ns | 0.0835 ns | 0.0820 ns |  2.853 ns |      - |     - |     - |         - |
|  GetDoubleOffset |  2.864 ns | 0.0782 ns | 0.0694 ns |  2.844 ns |      - |     - |     - |         - |
|       ReadDouble |  6.051 ns | 0.1486 ns | 0.1526 ns |  6.046 ns |      - |     - |     - |         - |

Benchmarks with issues:
  BinaryMemoryReadStream.GetOffset: DefaultJob
