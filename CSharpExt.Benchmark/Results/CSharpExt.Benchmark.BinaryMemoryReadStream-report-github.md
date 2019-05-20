``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.765 (1803/April2018Update/Redstone4)
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=3984652 Hz, Resolution=250.9629 ns, Timer=TSC
.NET Core SDK=2.1.700-preview-009618
  [Host]     : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT


```
|              Method |       Mean |     Error |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------------- |-----------:|----------:|----------:|-------:|------:|------:|----------:|
|                 Get | 10.2799 ns | 0.0777 ns | 0.0649 ns |      - |     - |     - |         - |
|            GetLarge | 10.4966 ns | 0.1248 ns | 0.1167 ns |      - |     - |     - |         - |
|     GetOffsetAmount |  9.6880 ns | 0.0827 ns | 0.0774 ns |      - |     - |     - |         - |
|           GetOffset | 10.6215 ns | 0.0605 ns | 0.0536 ns |      - |     - |     - |         - |
|                Read | 13.1330 ns | 0.1270 ns | 0.1188 ns |      - |     - |     - |         - |
|           ReadLarge | 12.2405 ns | 0.1564 ns | 0.1386 ns |      - |     - |     - |         - |
|    ReadOffsetAmount | 11.2544 ns | 0.2440 ns | 0.2905 ns |      - |     - |     - |         - |
|            GetBytes | 12.9031 ns | 0.1912 ns | 0.1788 ns | 0.0095 |     - |     - |      40 B |
|           ReadBytes | 14.6538 ns | 0.1946 ns | 0.1820 ns | 0.0095 |     - |     - |      40 B |
|             GetBool |  0.5500 ns | 0.0234 ns | 0.0208 ns |      - |     - |     - |         - |
|       GetBoolOffset |  0.6275 ns | 0.0488 ns | 0.0522 ns |      - |     - |     - |         - |
|            ReadBool |  2.2941 ns | 0.0547 ns | 0.0511 ns |      - |     - |     - |         - |
|            GetUInt8 |  0.2182 ns | 0.0136 ns | 0.0127 ns |      - |     - |     - |         - |
|      GetUInt8Offset |  0.3630 ns | 0.0203 ns | 0.0190 ns |      - |     - |     - |         - |
|           ReadUInt8 |  1.7288 ns | 0.0242 ns | 0.0226 ns |      - |     - |     - |         - |
|           GetUInt16 |  1.1204 ns | 0.0229 ns | 0.0214 ns |      - |     - |     - |         - |
|     GetUInt16Offset |  1.1270 ns | 0.0214 ns | 0.0200 ns |      - |     - |     - |         - |
|          ReadUInt16 |  2.7320 ns | 0.0372 ns | 0.0330 ns |      - |     - |     - |         - |
|           GetUInt32 |  1.1333 ns | 0.0248 ns | 0.0232 ns |      - |     - |     - |         - |
|     GetUInt32Offset |  1.1208 ns | 0.0232 ns | 0.0181 ns |      - |     - |     - |         - |
|          ReadUInt32 |  2.6927 ns | 0.0611 ns | 0.0572 ns |      - |     - |     - |         - |
|           GetUInt64 |  1.1776 ns | 0.0197 ns | 0.0175 ns |      - |     - |     - |         - |
|     GetUInt64Offset |  1.1437 ns | 0.0147 ns | 0.0137 ns |      - |     - |     - |         - |
|          ReadUInt64 |  2.7986 ns | 0.0305 ns | 0.0271 ns |      - |     - |     - |         - |
|             GetInt8 |  0.3401 ns | 0.0115 ns | 0.0108 ns |      - |     - |     - |         - |
|       GetInt8Offset |  0.3610 ns | 0.0078 ns | 0.0065 ns |      - |     - |     - |         - |
|            ReadInt8 |  1.9656 ns | 0.0204 ns | 0.0191 ns |      - |     - |     - |         - |
|            GetInt16 |  1.1436 ns | 0.0210 ns | 0.0197 ns |      - |     - |     - |         - |
|      GetInt16Offset |  1.2147 ns | 0.0205 ns | 0.0182 ns |      - |     - |     - |         - |
|           ReadInt16 |  2.9884 ns | 0.0391 ns | 0.0365 ns |      - |     - |     - |         - |
|            GetInt32 |  1.1376 ns | 0.0311 ns | 0.0276 ns |      - |     - |     - |         - |
|      GetInt32Offset |  1.2944 ns | 0.0325 ns | 0.0288 ns |      - |     - |     - |         - |
|           ReadInt32 |  2.6670 ns | 0.0385 ns | 0.0341 ns |      - |     - |     - |         - |
|            GetInt64 |  1.1160 ns | 0.0202 ns | 0.0189 ns |      - |     - |     - |         - |
|      GetInt64Offset |  1.2280 ns | 0.0216 ns | 0.0202 ns |      - |     - |     - |         - |
|           ReadInt64 |  2.8171 ns | 0.0328 ns | 0.0307 ns |      - |     - |     - |         - |
|           GetString | 18.5279 ns | 0.1752 ns | 0.1639 ns | 0.0210 |     - |     - |      88 B |
|     GetStringOffset | 18.9589 ns | 0.2250 ns | 0.2104 ns | 0.0210 |     - |     - |      88 B |
|          ReadString | 20.2691 ns | 0.1661 ns | 0.1473 ns | 0.0210 |     - |     - |      88 B |
|            GetFloat |  1.3306 ns | 0.0145 ns | 0.0129 ns |      - |     - |     - |         - |
|      GetFloatOffset |  1.3033 ns | 0.0137 ns | 0.0114 ns |      - |     - |     - |         - |
|           ReadFloat |  2.8518 ns | 0.0163 ns | 0.0144 ns |      - |     - |     - |         - |
|           GetDouble |  1.3581 ns | 0.0268 ns | 0.0251 ns |      - |     - |     - |         - |
|     GetDoubleOffset |  1.3486 ns | 0.0161 ns | 0.0151 ns |      - |     - |     - |         - |
|          ReadDouble |  2.7205 ns | 0.0330 ns | 0.0308 ns |      - |     - |     - |         - |
|       BytesToString | 20.8909 ns | 0.1759 ns | 0.1559 ns | 0.0248 |     - |     - |     104 B |
| BytesToStringOffset | 18.8388 ns | 0.1033 ns | 0.0966 ns | 0.0229 |     - |     - |      96 B |
