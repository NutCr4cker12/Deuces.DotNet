//BenchmarkRunner.Run<LookupTest>(); --> 288.3 us Mean

using BenchmarkDotNet.Running;
using BenchMarks;

BenchmarkRunner.Run<EvaluatorPerformance>();
Console.ReadLine();

