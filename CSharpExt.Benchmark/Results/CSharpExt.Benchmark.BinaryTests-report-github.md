``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.765 (1803/April2018Update/Redstone4)
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=3984652 Hz, Resolution=250.9629 ns, Timer=TSC
.NET Core SDK=2.1.700-preview-009618
  [Host]     : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT


```
|                                      Method |        Mean |     Error |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------------------------------------- |------------:|----------:|----------:|-------:|------:|------:|----------:|
|                          GetIntBitConverter |   1.2264 ns | 0.0145 ns | 0.0129 ns |      - |     - |     - |         - |
|           GetIntBinaryPrimitivesMemorySlice |   5.9264 ns | 0.0477 ns | 0.0446 ns |      - |     - |     - |         - |
|     GetIntBinaryPrimitivesMemorySliceToSize |   5.8270 ns | 0.0588 ns | 0.0521 ns |      - |     - |     - |         - |
|       GetIntBinaryPrimitivesMemorySpanSlice |   4.8524 ns | 0.0612 ns | 0.0572 ns |      - |     - |     - |         - |
| GetIntBinaryPrimitivesMemorySpanSliceToSize |   4.8338 ns | 0.0622 ns | 0.0519 ns |      - |     - |     - |         - |
|                 GetIntBinaryPrimitivesSlice |   0.6632 ns | 0.0129 ns | 0.0114 ns |      - |     - |     - |         - |
|           GetIntBinaryPrimitivesSliceToSize |   0.6975 ns | 0.0144 ns | 0.0127 ns |      - |     - |     - |         - |
|                                    GetBytes |  13.1507 ns | 0.2255 ns | 0.1883 ns | 0.0076 |     - |     - |      32 B |
|                             GetBytesViaSpan |   6.2235 ns | 0.0584 ns | 0.0517 ns | 0.0076 |     - |     - |      32 B |
|                                GetSomeBytes |  13.1524 ns | 0.2742 ns | 0.2431 ns | 0.0076 |     - |     - |      32 B |
|                         GetSomeBytesViaSpan |   6.3723 ns | 0.1831 ns | 0.1798 ns | 0.0076 |     - |     - |      32 B |
|                               GetLargeBytes | 299.6497 ns | 1.8380 ns | 1.7192 ns | 0.9813 |     - |     - |    4120 B |
|                        GetLargeBytesViaSpan | 318.4417 ns | 6.0907 ns | 5.3993 ns | 0.9813 |     - |     - |    4120 B |
|                           GetSomeLargeBytes | 322.4487 ns | 6.3703 ns | 8.2832 ns | 0.9794 |     - |     - |    4112 B |
|                    GetSomeLargeBytesViaSpan | 321.6054 ns | 6.0996 ns | 5.9906 ns | 0.9794 |     - |     - |    4112 B |
