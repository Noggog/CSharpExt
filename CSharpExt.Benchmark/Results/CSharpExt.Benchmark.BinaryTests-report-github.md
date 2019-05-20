``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.765 (1803/April2018Update/Redstone4)
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=3984652 Hz, Resolution=250.9629 ns, Timer=TSC
.NET Core SDK=2.1.700-preview-009618
  [Host]     : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT


```
|                                      Method |     Mean |     Error |    StdDev |
|-------------------------------------------- |---------:|----------:|----------:|
|                          GetIntBitConverter | 1.874 ns | 0.0129 ns | 0.0121 ns |
|           GetIntBinaryPrimitivesMemorySlice | 6.734 ns | 0.0778 ns | 0.0728 ns |
|     GetIntBinaryPrimitivesMemorySliceToSize | 6.966 ns | 0.0722 ns | 0.0676 ns |
|       GetIntBinaryPrimitivesMemorySpanSlice | 5.555 ns | 0.0937 ns | 0.0877 ns |
| GetIntBinaryPrimitivesMemorySpanSliceToSize | 5.485 ns | 0.0573 ns | 0.0508 ns |
|                 GetIntBinaryPrimitivesSlice | 1.132 ns | 0.0247 ns | 0.0219 ns |
|           GetIntBinaryPrimitivesSliceToSize | 1.128 ns | 0.0258 ns | 0.0241 ns |
