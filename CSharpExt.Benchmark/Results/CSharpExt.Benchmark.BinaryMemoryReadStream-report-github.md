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
|              Get | 11.589 ns | 0.1392 ns | 0.1234 ns | 11.563 ns |      - |     - |     - |         - |
|         GetLarge | 11.821 ns | 0.0948 ns | 0.0841 ns | 11.821 ns |      - |     - |     - |         - |
|  GetOffsetAmount | 11.178 ns | 0.0676 ns | 0.0632 ns | 11.167 ns |      - |     - |     - |         - |
|        GetOffset | 12.036 ns | 0.1050 ns | 0.0982 ns | 12.039 ns |      - |     - |     - |         - |
|             Read | 15.586 ns | 0.4062 ns | 0.4678 ns | 15.419 ns |      - |     - |     - |         - |
|        ReadLarge | 15.851 ns | 0.3298 ns | 0.3926 ns | 15.737 ns |      - |     - |     - |         - |
| ReadOffsetAmount | 14.959 ns | 0.1797 ns | 0.1681 ns | 14.876 ns |      - |     - |     - |         - |
|         GetBytes | 15.439 ns | 0.4664 ns | 1.3308 ns | 14.727 ns | 0.0095 |     - |     - |      40 B |
|        ReadBytes | 18.176 ns | 0.2788 ns | 0.2608 ns | 18.157 ns | 0.0095 |     - |     - |      40 B |
|          GetBool |  1.530 ns | 0.0466 ns | 0.0436 ns |  1.535 ns |      - |     - |     - |         - |
|    GetBoolOffset |  1.702 ns | 0.0487 ns | 0.0432 ns |  1.714 ns |      - |     - |     - |         - |
|         ReadBool |  5.331 ns | 0.0847 ns | 0.0792 ns |  5.326 ns |      - |     - |     - |         - |
|         GetUInt8 |  1.514 ns | 0.0646 ns | 0.0605 ns |  1.496 ns |      - |     - |     - |         - |
|   GetUInt8Offset |  1.460 ns | 0.0336 ns | 0.0298 ns |  1.466 ns |      - |     - |     - |         - |
|        ReadUInt8 |  5.071 ns | 0.0488 ns | 0.0407 ns |  5.084 ns |      - |     - |     - |         - |
|        GetUInt16 |  2.411 ns | 0.0559 ns | 0.0496 ns |  2.399 ns |      - |     - |     - |         - |
|  GetUInt16Offset |  2.362 ns | 0.0569 ns | 0.0505 ns |  2.356 ns |      - |     - |     - |         - |
|       ReadUInt16 |  6.328 ns | 0.1477 ns | 0.1517 ns |  6.337 ns |      - |     - |     - |         - |
|        GetUInt32 |  2.615 ns | 0.0715 ns | 0.0669 ns |  2.595 ns |      - |     - |     - |         - |
|  GetUInt32Offset |  2.578 ns | 0.0779 ns | 0.0765 ns |  2.551 ns |      - |     - |     - |         - |
|       ReadUInt32 |  6.261 ns | 0.1154 ns | 0.1080 ns |  6.232 ns |      - |     - |     - |         - |
|        GetUInt64 |  2.603 ns | 0.0764 ns | 0.0784 ns |  2.620 ns |      - |     - |     - |         - |
|  GetUInt64Offset |  2.353 ns | 0.0670 ns | 0.0560 ns |  2.343 ns |      - |     - |     - |         - |
|       ReadUInt64 |  5.929 ns | 0.0925 ns | 0.0820 ns |  5.919 ns |      - |     - |     - |         - |
|          GetInt8 |  1.483 ns | 0.0549 ns | 0.0459 ns |  1.477 ns |      - |     - |     - |         - |
|    GetInt8Offset |  1.375 ns | 0.0529 ns | 0.0495 ns |  1.368 ns |      - |     - |     - |         - |
|         ReadInt8 |  5.269 ns | 0.0944 ns | 0.0883 ns |  5.281 ns |      - |     - |     - |         - |
|         GetInt16 |  2.546 ns | 0.0607 ns | 0.0538 ns |  2.558 ns |      - |     - |     - |         - |
|   GetInt16Offset |  2.684 ns | 0.1059 ns | 0.3038 ns |  2.527 ns |      - |     - |     - |         - |
|        ReadInt16 |  6.167 ns | 0.1085 ns | 0.1015 ns |  6.147 ns |      - |     - |     - |         - |
|         GetInt32 |  2.412 ns | 0.0901 ns | 0.0842 ns |  2.382 ns |      - |     - |     - |         - |
|   GetInt32Offset |  2.389 ns | 0.0555 ns | 0.0520 ns |  2.378 ns |      - |     - |     - |         - |
|        ReadInt32 |  5.955 ns | 0.0633 ns | 0.0592 ns |  5.944 ns |      - |     - |     - |         - |
|         GetInt64 |  2.334 ns | 0.0540 ns | 0.0505 ns |  2.326 ns |      - |     - |     - |         - |
|   GetInt64Offset |  2.556 ns | 0.0639 ns | 0.0598 ns |  2.540 ns |      - |     - |     - |         - |
|        ReadInt64 |  6.104 ns | 0.0764 ns | 0.0677 ns |  6.086 ns |      - |     - |     - |         - |
|        GetString | 21.118 ns | 0.5413 ns | 0.5558 ns | 20.969 ns | 0.0210 |     - |     - |      88 B |
|  GetStringOffset | 20.307 ns | 0.5393 ns | 0.8708 ns | 19.970 ns | 0.0210 |     - |     - |      88 B |
|       ReadString | 24.671 ns | 0.5201 ns | 0.5341 ns | 24.654 ns | 0.0210 |     - |     - |      88 B |
|         GetFloat |  2.640 ns | 0.0796 ns | 0.1090 ns |  2.632 ns |      - |     - |     - |         - |
|   GetFloatOffset |  2.623 ns | 0.0772 ns | 0.0722 ns |  2.581 ns |      - |     - |     - |         - |
|        ReadFloat |  6.398 ns | 0.0640 ns | 0.0500 ns |  6.383 ns |      - |     - |     - |         - |
|        GetDouble |  2.923 ns | 0.0435 ns | 0.0363 ns |  2.903 ns |      - |     - |     - |         - |
|  GetDoubleOffset |  2.862 ns | 0.0579 ns | 0.0542 ns |  2.858 ns |      - |     - |     - |         - |
|       ReadDouble |  6.090 ns | 0.1038 ns | 0.0920 ns |  6.058 ns |      - |     - |     - |         - |
