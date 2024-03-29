
![License](https://img.shields.io/github/license/NutCr4cker12/Deuces.DotNet)
![Build](https://img.shields.io/github/actions/workflow/status/NutCr4cker12/Deuces.DotNet/ci.yml)
![Nuget](https://img.shields.io/nuget/v/Deuces.DotNet)

# Deuces.DotNet
A C# port of [deuces](https://github.com/worldveil/deuces) poker evaluation library

`[ 2 ❤ ] , [ 2 ♠ ]`

---
## Installation
This package is distributed via [Nuget](https://www.nuget.org/packages/Deuces.DotNet) and can be downloaded via Nuget Packet Manager or running
`dotnet add package Deuces.DotNet`

---
## Usage

Deces.DotNet is ported to C# so that it can be used the same as with python with a C# naming conventions:

```C#
using Deuces;

int card = Card.New("Qh");

int[] board = {
    Card.New("Ah"),
    Card.New("Kd"),
    Card.New("Jc"),
};
int[] hand =
{
    Card.New("Qs"),
    Card.New("Th")
};

Console.WriteLine(Card.IntsToPrettyStr(hand));
// [ Q ♠ ],[ T ♥ ]
```

```C#
var deck = new Deck();
var board = deck.Draw(5);
var hand = deck.Draw(2);

var evaluator = new Evaluator();
var rank = evaluator.Evaluate(hand, board);
Console.WriteLine(rank);
// 6330

Console.WriteLine(Card.IntsToPrettyStr(board));
// [ 4 ♣ ] , [ A ♠ ] , [ 5 ♦ ] , [ K ♣ ] , [ 2 ♠ ]
Console.WriteLine(Card.IntsToPrettyStr(hand));
// [ 6 ♣ ] , [ 7 ❤ ] 

var rankClass = evaluator.GetRankClass(rank);
var classString = evaluator.ClassToString(rankClass);

Console.WriteLine(rankClass);
// 9
Console.WriteLine(classString);
// High Card
```

By default, the pretty string methods returns a colored string. If the colors aren't showing up correctly in your console, you can disable card coloring by setting `Card.UseColoredCards = false;`

```C#
int[] cards = {
    Card.New("As"),
    Card.New("Kh"),
    Card.New("Qd"),
    Card.New("Jc"),
};

Console.WriteLine(Card.IntsToPrettyStr(cards));

Card.UseColoredCards = false;
Console.WriteLine(Card.IntsToPrettyStr(cards));

```

![Colored Cards](https://github.com/NutCr4cker12/Deuces.DotNet/blob/main/Images/ColoredCards.png)  
  


---
## Performance
Performance of Deuces.DotNet is measured with [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet).


``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1265/22H2/2022Update/SunValley2)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=7.0.200
  [Host] : .NET 7.0.3 (7.0.323.6910), X64 RyuJIT AVX2

Job=MediumRun  Toolchain=InProcessNoEmitToolchain  IterationCount=15  
LaunchCount=2  WarmupCount=10  

```
|                Method |      Mean |    Error |   StdDev |
|---------------------- |----------:|---------:|---------:|
|     EvaluateFiveFlush |  28.14 ns | 0.196 ns | 0.288 ns |
|  EvaluateFiveNonFlush |  15.13 ns | 0.099 ns | 0.136 ns |
|      EvaluateSixFlush | 122.20 ns | 0.563 ns | 0.842 ns |
|   EvaluateSixNonFlush | 108.66 ns | 0.576 ns | 0.862 ns |
|    EvaluateSevenFlush | 924.67 ns | 1.362 ns | 1.909 ns |
| EvaluateSevenNonFlush | 873.11 ns | 3.955 ns | 5.798 ns |

\
To compare against [python implementation](https://github.com/ihendley/treys), here's the performance with Deuces:  

![Trey performace](https://github.com/NutCr4cker12/Deuces.DotNet/blob/main/Images/deucesPerf.png "Performance")  

\
As a conclusion:
|                Method |        Avg  |    Evaluations per second |
|---------------------- |------------:|--------------------------:|
| Deuces 5              |    1 000 ns |                   909 137 |
| Deuces.DotNet 5       |       22 ns |                46 221 400 |
| Deuces 6              |    5 000 ns |                   185 185 |
| Deuces.DotNet 6       |      115 ns |                 8 663 259 |
| Deuces 7              |   18 000 ns |                    56 179 |
| Deuces.DotNet 7       |      899 ns |                 1 112 483 |

Creating a LookUp table taks about 0.0002883 Seconds

---
# License

MIT License

Copyright (c) 2023 NutCr4cker12

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.