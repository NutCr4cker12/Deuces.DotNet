using BenchmarkDotNet.Attributes;
using Deuces;

namespace BenchMarks;

[Config(typeof(AntiVirusFriendlyConfig))]
public class EvaluatorPerformance
{
    private Evaluator? _evaluator;
    private int[]? _board5Flush;
    private int[]? _cards5Flush;
    private int[]? _board5NonFlush;
    private int[]? _cards5NonFlush;

    private int[]? _board6Flush;
    private int[]? _cards6Flush;
    private int[]? _board6NonFlush;
    private int[]? _cards6NonFlush;

    private int[]? _board7Flush;
    private int[]? _cards7Flush;
    private int[]? _board7NonFlush;
    private int[]? _cards7NonFlush;

    [GlobalSetup]
    public void Setup()
    {
        _evaluator = new Evaluator();
        (_cards5Flush, _board5Flush, _cards5NonFlush, _board5NonFlush) = GetRandomCards(3);
        (_cards6Flush, _board6Flush, _cards6NonFlush, _board6NonFlush) = GetRandomCards(4);
        (_cards7Flush, _board7Flush, _cards7NonFlush, _board7NonFlush) = GetRandomCards(5);
    }

    [Benchmark]
    public int EvaluateFiveFlush()
    {
        return _evaluator!.Evaluate(_cards5Flush!, _board5Flush!);
    }

    [Benchmark]
    public int EvaluateFiveNonFlush()
    {
        return _evaluator!.Evaluate(_cards5NonFlush!, _board5NonFlush!);
    }

    [Benchmark]
    public int EvaluateSixFlush()
    {
        return _evaluator!.Evaluate(_cards6Flush!, _board6Flush!);
    }

    [Benchmark]
    public int EvaluateSixNonFlush()
    {
        return _evaluator!.Evaluate(_cards6NonFlush!, _board6NonFlush!);
    }

    [Benchmark]
    public int EvaluateSevenFlush()
    {
        return _evaluator!.Evaluate(_cards7Flush!, _board7Flush!);
    }

    [Benchmark]
    public int EvaluateSevenNonFlush()
    {
        return _evaluator!.Evaluate(_cards7NonFlush!, _board7NonFlush!);
    }

    private (int[] flushCards, int[] flushBoard, int[] nonFlushCards, int[] nonFlushBoard) GetRandomCards(int boardCount)
    {
        int[] flushCards = null!;
        int[] flushBoard = null!;
        int[] nonFlushCards = null!;
        int[] nonFlushBoard = null!;

        while (flushCards == null || nonFlushCards == null)
        {
            var deck = new Deck();
            var cards = deck.Draw(2);
            var board = deck.Draw(boardCount);

            var handRank = _evaluator!.Evaluate(cards, board);
            var rankClass = _evaluator.GetRankClass(handRank);
            var classString = _evaluator.ClassToString(rankClass);
            if (classString is "Flush" or "Straight Flush")
            {
                flushCards = cards;
                flushBoard = board;
            }
            else
            {
                nonFlushCards = cards;
                nonFlushBoard = board;
            }
        }

        return (flushCards, flushBoard!, nonFlushCards, nonFlushBoard!);
    }
}