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
|                          GetIntBitConverter | 1.888 ns | 0.0332 ns | 0.0295 ns |
|           GetIntBinaryPrimitivesMemorySlice | 6.841 ns | 0.0872 ns | 0.0815 ns |
|     GetIntBinaryPrimitivesMemorySliceToSize | 7.019 ns | 0.0759 ns | 0.0710 ns |
|       GetIntBinaryPrimitivesMemorySpanSlice | 5.629 ns | 0.1029 ns | 0.0912 ns |
| GetIntBinaryPrimitivesMemorySpanSliceToSize | 5.540 ns | 0.0419 ns | 0.0350 ns |
|                 GetIntBinaryPrimitivesSlice | 1.156 ns | 0.0441 ns | 0.0412 ns |
|           GetIntBinaryPrimitivesSliceToSize | 1.141 ns | 0.0533 ns | 0.0547 ns |
