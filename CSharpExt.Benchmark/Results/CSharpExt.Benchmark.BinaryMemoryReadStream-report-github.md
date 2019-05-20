``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.765 (1803/April2018Update/Redstone4)
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=3984652 Hz, Resolution=250.9629 ns, Timer=TSC
.NET Core SDK=2.1.700-preview-009618
  [Host]     : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT


```
|              Method |       Mean |     Error |    StdDev |     Median |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------------- |-----------:|----------:|----------:|-----------:|-------:|------:|------:|----------:|
|                 Get | 12.1983 ns | 0.1412 ns | 0.1252 ns | 12.1614 ns |      - |     - |     - |         - |
|            GetLarge | 10.7699 ns | 0.0575 ns | 0.0480 ns | 10.7908 ns |      - |     - |     - |         - |
|     GetOffsetAmount |  9.7991 ns | 0.1240 ns | 0.1160 ns |  9.8033 ns |      - |     - |     - |         - |
|           GetOffset | 10.7991 ns | 0.1505 ns | 0.1334 ns | 10.8065 ns |      - |     - |     - |         - |
|                Read | 12.1069 ns | 0.0919 ns | 0.0768 ns | 12.0882 ns |      - |     - |     - |         - |
|           ReadLarge | 12.3051 ns | 0.1352 ns | 0.1264 ns | 12.2934 ns |      - |     - |     - |         - |
|    ReadOffsetAmount | 11.4575 ns | 0.1819 ns | 0.1519 ns | 11.4522 ns |      - |     - |     - |         - |
|            GetBytes | 13.3088 ns | 0.1980 ns | 0.1852 ns | 13.3273 ns | 0.0095 |     - |     - |      40 B |
|           ReadBytes | 14.4765 ns | 0.2043 ns | 0.1911 ns | 14.4279 ns | 0.0095 |     - |     - |      40 B |
|             GetSpan |  1.6085 ns | 0.0261 ns | 0.0244 ns |  1.6006 ns |      - |     - |     - |         - |
|            ReadSpan |  4.0305 ns | 0.0545 ns | 0.0509 ns |  4.0219 ns |      - |     - |     - |         - |
|        GetSpanBytes |  9.0465 ns | 0.1619 ns | 0.1436 ns |  9.0198 ns | 0.0095 |     - |     - |      40 B |
|       ReadSpanBytes | 11.2601 ns | 0.3040 ns | 0.4733 ns | 11.0680 ns | 0.0095 |     - |     - |      40 B |
|             GetBool |  0.4864 ns | 0.0114 ns | 0.0101 ns |  0.4848 ns |      - |     - |     - |         - |
|       GetBoolOffset |  0.5917 ns | 0.0110 ns | 0.0098 ns |  0.5955 ns |      - |     - |     - |         - |
|            ReadBool |  2.1883 ns | 0.0176 ns | 0.0156 ns |  2.1868 ns |      - |     - |     - |         - |
|            GetUInt8 |  0.2418 ns | 0.0097 ns | 0.0081 ns |  0.2387 ns |      - |     - |     - |         - |
|      GetUInt8Offset |  0.2762 ns | 0.0464 ns | 0.0388 ns |  0.2634 ns |      - |     - |     - |         - |
|           ReadUInt8 |  1.8023 ns | 0.0628 ns | 0.1688 ns |  1.7136 ns |      - |     - |     - |         - |
|           GetUInt16 |  1.3297 ns | 0.0297 ns | 0.0278 ns |  1.3232 ns |      - |     - |     - |         - |
|     GetUInt16Offset |  1.3193 ns | 0.0170 ns | 0.0159 ns |  1.3198 ns |      - |     - |     - |         - |
|          ReadUInt16 |  2.7145 ns | 0.0484 ns | 0.0429 ns |  2.7057 ns |      - |     - |     - |         - |
|           GetUInt32 |  1.3447 ns | 0.0379 ns | 0.0336 ns |  1.3354 ns |      - |     - |     - |         - |
|     GetUInt32Offset |  1.3304 ns | 0.0290 ns | 0.0271 ns |  1.3213 ns |      - |     - |     - |         - |
|          ReadUInt32 |  2.7874 ns | 0.0440 ns | 0.0412 ns |  2.7916 ns |      - |     - |     - |         - |
|           GetUInt64 |  1.3247 ns | 0.0176 ns | 0.0164 ns |  1.3259 ns |      - |     - |     - |         - |
|     GetUInt64Offset |  1.2484 ns | 0.0175 ns | 0.0146 ns |  1.2497 ns |      - |     - |     - |         - |
|          ReadUInt64 |  2.6743 ns | 0.0506 ns | 0.0422 ns |  2.6769 ns |      - |     - |     - |         - |
|             GetInt8 |  0.2648 ns | 0.0112 ns | 0.0099 ns |  0.2621 ns |      - |     - |     - |         - |
|       GetInt8Offset |  0.3921 ns | 0.0088 ns | 0.0073 ns |  0.3912 ns |      - |     - |     - |         - |
|            ReadInt8 |  1.9014 ns | 0.0226 ns | 0.0211 ns |  1.8905 ns |      - |     - |     - |         - |
|            GetInt16 |  1.3457 ns | 0.0164 ns | 0.0137 ns |  1.3443 ns |      - |     - |     - |         - |
|      GetInt16Offset |  1.3426 ns | 0.0190 ns | 0.0169 ns |  1.3369 ns |      - |     - |     - |         - |
|           ReadInt16 |  2.7276 ns | 0.0425 ns | 0.0397 ns |  2.7347 ns |      - |     - |     - |         - |
|            GetInt32 |  1.3339 ns | 0.0182 ns | 0.0162 ns |  1.3317 ns |      - |     - |     - |         - |
|      GetInt32Offset |  1.3104 ns | 0.0184 ns | 0.0172 ns |  1.3025 ns |      - |     - |     - |         - |
|           ReadInt32 |  2.7555 ns | 0.0245 ns | 0.0217 ns |  2.7491 ns |      - |     - |     - |         - |
|            GetInt64 |  1.3472 ns | 0.0191 ns | 0.0179 ns |  1.3457 ns |      - |     - |     - |         - |
|      GetInt64Offset |  1.3138 ns | 0.0396 ns | 0.0351 ns |  1.3091 ns |      - |     - |     - |         - |
|           ReadInt64 |  2.6272 ns | 0.0083 ns | 0.0074 ns |  2.6273 ns |      - |     - |     - |         - |
|           GetString | 19.8379 ns | 0.5455 ns | 1.5738 ns | 18.9905 ns | 0.0210 |     - |     - |      88 B |
|     GetStringOffset | 18.6857 ns | 0.2556 ns | 0.2135 ns | 18.6758 ns | 0.0210 |     - |     - |      88 B |
|          ReadString | 21.1856 ns | 0.2146 ns | 0.2007 ns | 21.1570 ns | 0.0210 |     - |     - |      88 B |
|            GetFloat |  1.3525 ns | 0.0250 ns | 0.0234 ns |  1.3513 ns |      - |     - |     - |         - |
|      GetFloatOffset |  1.3884 ns | 0.0280 ns | 0.0248 ns |  1.3864 ns |      - |     - |     - |         - |
|           ReadFloat |  2.5825 ns | 0.0330 ns | 0.0309 ns |  2.5726 ns |      - |     - |     - |         - |
|           GetDouble |  1.3345 ns | 0.0317 ns | 0.0281 ns |  1.3321 ns |      - |     - |     - |         - |
|     GetDoubleOffset |  1.3730 ns | 0.0365 ns | 0.0304 ns |  1.3731 ns |      - |     - |     - |         - |
|          ReadDouble |  2.4733 ns | 0.0350 ns | 0.0292 ns |  2.4625 ns |      - |     - |     - |         - |
|       BytesToString | 20.2712 ns | 0.1450 ns | 0.1286 ns | 20.2694 ns | 0.0248 |     - |     - |     104 B |
| BytesToStringOffset | 19.8935 ns | 0.2777 ns | 0.2598 ns | 19.8310 ns | 0.0229 |     - |     - |      96 B |
