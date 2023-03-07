using BenchmarkDotNet.Attributes;
using Deuces;

namespace BenchMarks;

[Config(typeof(AntiVirusFriendlyConfig))]
public class EvaluatorPerformance
{
    private const int N = 10_000;
    private List<(int[] cards, int[] board)>? _eval5;
    private List<(int[] cards, int[] board)>? _eval6;
    private List<(int[] cards, int[] board)>? _eval7;
    private Evaluator _evaluator;

    [GlobalSetup]
    public void Setup()
    {
        _evaluator = new Evaluator();
        _eval5 = new List<(int[], int[])>(N);
        _eval6 = new List<(int[], int[])>(N);
        _eval7 = new List<(int[], int[])>(N);

        // Generate 10_000 random 5, 6 and 7 card list
        for (var i = 0; i < N; i++)
        {
            var deck = new Deck();
            var cards = deck.Draw(2);
            _eval5.Add((cards, deck.Draw(3)));
            _eval6.Add((cards, deck.Draw(4)));
            _eval7.Add((cards, deck.Draw(5)));
        }
    }

    [Benchmark]
    public void EvaluateFive()
    {
        for (var i = 0; i < N; i++)
        {
            var (cards, board) = _eval5![i];
            var _ = _evaluator.Evaluate(cards, board);
        }
    }

    [Benchmark]
    public void EvaluateSix()
    {
        for (var i = 0; i < N; i++)
        {
            var (cards, board) = _eval6![i];
            var rank = _evaluator.Evaluate(cards, board);
        }
    }

    [Benchmark]
    public void EvaluateSeven()
    {
        for (var i = 0; i < N; i++)
        {
            var (cards, board) = _eval7![i];
            var rank = _evaluator.Evaluate(cards, board);
        }
    }

}