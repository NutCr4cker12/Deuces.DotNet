using Deuces;

namespace Tests;

[TestClass()]
public class EvaluatorTests
{
    private Evaluator? _evaluator;

    private int[] StringToCards(string cards)
    {
        return Enumerable
                .Range(0, cards.Length / 2)
                .Select(n => Card.New(cards.Substring(n * 2, 2)))
                .ToArray();
    }

    [TestInitialize]
    public void Setup()
    {
        _evaluator = new();
    }

    [DataTestMethod]
    [DataRow("QsJsTs", "AsKs", "9s8s")] // StraightFlush comparison
    [DataRow("Ts5s5h", "5c5d", "TdTc")] // Quads vs FullHouse
    [DataRow("TsTh5s", "Tc5d", "5h5c")] // FullHouse comparison
    [DataRow("Ts5s3s", "As2s", "KsQs")] // Flush comparison
    [DataRow("7s5s3s", "4s2s", "6d4d")] // Flush vs Straight
    [DataRow("5s3s2d", "6d4d", "Ad4h")] // Straight comparison
    [DataRow("5s3s2d", "Ad4h", "5d5c")] // Straight vs Trips
    [DataRow("TsTh5s", "AdTc", "KdTc")] // Trips comparison
    [DataRow("AsTh2d", "2s2h", "AdTs")] // Trips vs Two pair
    [DataRow("AsTh5s", "AdTs", "Ah5d")] // Two pair comparison
    [DataRow("As3s2d", "3d2s", "AdKd")] // Two pair vs Pair
    [DataRow("As3s2d", "Kd3d", "Kh2s")] // Pair comparison
    [DataRow("As3s2d", "Kh2s", "KdQs")] // Pair vs High Card
    [DataRow("As3s2d", "KdQs", "KhJs")] // High Card comparison
    public void EvaluateFiveTest(string board, string cards1, string cards2)
    {
        var boardInts = StringToCards(board);
        var card1Ints = StringToCards(cards1);
        var card2Ints = StringToCards(cards2);

        var cards1Res = _evaluator!.Evaluate(card1Ints, boardInts);
        var cards2Res = _evaluator!.Evaluate(card2Ints, boardInts);
        Assert.IsTrue(cards1Res < cards2Res);
    }

    [DataTestMethod]
    [DataRow("QsJsTs2d", "AsKs", "9s8s")] // StraightFlush comparison
    [DataRow("QsJsTsTh", "9s8s", "TcTd")] // StraightFlush vs Quads
    [DataRow("TsTh5s5h", "TcTd", "5c5d")] // Quads comparison
    [DataRow("TsTh5s5h", "5c5d", "AsTc")] // Quads vs FullHouse
    [DataRow("TsTh5s5h", "AsTc", "Ad5c")] // FullHouse comparison
    [DataRow("TsTh5s3s", "3d3h", "AsKs")] // FullHouse vs Flush
    [DataRow("TsTh5s3s", "As2s", "KsQs")] // Flush comparison
    [DataRow("Ts5s3s2d", "4s2s", "6d4d")] // Flush vs Straight
    [DataRow("Ts5s3s2d", "6d4d", "Ad4h")] // Straight comparison
    [DataRow("Ts5s3s2d", "Ad4h", "TdTc")] // Straight vs Trips
    [DataRow("TsTh5s3s", "AdTc", "KdTc")] // Trips comparison
    [DataRow("AsTh5s2d", "2s2h", "AdTs")] // Trips vs Two pair
    [DataRow("AsTh5s3s", "AdTs", "Ah5d")] // Two pair comparison
    [DataRow("AsTh3s2d", "3d2s", "AdKd")] // Two pair vs Pair
    [DataRow("AsTh3s2d", "Kd3d", "Kh2s")] // Pair comparison
    [DataRow("As5s3s2d", "Kh2s", "KdQs")] // Pair vs High Card
    [DataRow("As5s3s2d", "KdQs", "KhJs")] // High Card comparison
    public void EvaluateSixTest(string board, string cards1, string cards2)
    {
        var boardInts = StringToCards(board);
        var card1Ints = StringToCards(cards1);
        var card2Ints = StringToCards(cards2);

        var cards1Res = _evaluator!.Evaluate(card1Ints, boardInts);
        var cards2Res = _evaluator!.Evaluate(card2Ints, boardInts);
        Assert.IsTrue(cards1Res < cards2Res);
    }

    [DataTestMethod]
    [DataRow("QsJsTs2d2h", "AsKs", "9s8s")] // StraightFlush comparison
    [DataRow("QsJsTsTh5d", "9s8s", "TcTd")] // StraightFlush vs Quads
    [DataRow("TsTh5s5h2d", "TcTd", "5c5d")] // Quads comparison
    [DataRow("TsTh5s5h2d", "5c5d", "AsTc")] // Quads vs FullHouse
    [DataRow("TsTh5s5h2d", "AsTc", "Ad5c")] // FullHouse comparison
    [DataRow("TsTh5s3s2d", "2s2h", "AsKs")] // FullHouse vs Flush
    [DataRow("TsTh5s3s2d", "As2s", "KsQs")] // Flush comparison
    [DataRow("TsTh5s3s2d", "4s2s", "6d4d")] // Flush vs Straight
    [DataRow("TsTh5s3s2d", "6d4d", "Ad4h")] // Straight comparison
    [DataRow("TsTh5s3s2d", "Ad4h", "AdTc")] // Straight vs Trips
    [DataRow("TsTh5s3s2d", "AdTc", "KdTd")] // Trips comparison
    [DataRow("AsTh5s3s2d", "2s2h", "AdTs")] // Trips vs Two pair
    [DataRow("AsTh5s3s2d", "AdTs", "Ah5d")] // Two pair comparison
    [DataRow("AsTh5s3s2d", "3d2s", "AdKd")] // Two pair vs Pair
    [DataRow("AsTh5s3s2d", "Kd3d", "Kh2s")] // Pair comparison
    [DataRow("AsTh5s3s2d", "Kh2s", "KdQs")] // Pair vs High Card
    [DataRow("AsTh5s3s2d", "KdQs", "KhJs")] // High Card comparison
    public void EvaluateSevenTest(string board, string cards1, string cards2)
    {
        var boardInts = StringToCards(board);
        var card1Ints = StringToCards(cards1);
        var card2Ints = StringToCards(cards2);

        var cards1Res = _evaluator!.Evaluate(card1Ints, boardInts);
        var cards2Res = _evaluator!.Evaluate(card2Ints, boardInts);
        Assert.IsTrue(cards1Res < cards2Res);
    }

    [DataTestMethod]
    [DataRow("AsKsQsJsTs", "4s2s", "9s8s")] // StraightFlush 
    [DataRow("TsThTcTd2d", "AcKc", "AdQd")] // Quads
    [DataRow("TsTh5s5h2d", "AsTc", "Td5c")] // FullHouse
    [DataRow("AsQsJsTs9s", "7s2s", "6s5s")] // Flush
    [DataRow("QhJdTh", "AcKc", "AsKs")] // Straight
    [DataRow("TsTh5s3s", "AdTc", "AcTd")] // Trips
    [DataRow("AsTh5s3s", "AdTs", "AhTc")] // Two pair
    [DataRow("AsTh5s3s2d", "Kd3d", "Kh3c")] // Pair
    [DataRow("AsTh5s3s2d", "KdQs", "KhQd")] // High Card
    public void ShouldBeEqual(string board, string cards1, string cards2)
    {
        var boardInts = StringToCards(board);
        var card1Ints = StringToCards(cards1);
        var card2Ints = StringToCards(cards2);

        var cards1Res = _evaluator!.Evaluate(card1Ints, boardInts);
        var cards2Res = _evaluator!.Evaluate(card2Ints, boardInts);
        Assert.AreEqual(cards1Res, cards2Res);
    }

    [DataTestMethod]
    [DataRow("AsKsQsJsTs", "4s2s", 1)] // StraightFlush 
    [DataRow("TsThTcTd2d", "AcKc", 2)] // Quads
    [DataRow("TsTh5s5h2d", "AsTc", 3)] // FullHouse
    [DataRow("AsQsJsTs9s", "7s2s", 4)] // Flush
    [DataRow("QhJdTh", "AcKc", 5)] // Straight
    [DataRow("TsTh5s3s", "AdTc", 6)] // Trips
    [DataRow("AsTh5s3s", "AdTs", 7)] // Two pair
    [DataRow("AsTh5s3s2d", "Kd3d", 8)] // Pair
    [DataRow("AsTh5s3s2d", "KdQs", 9)] // High Card
    public void GetRankClassTest(string board, string cards, int expectedRankClass)
    {
        var boardInts = StringToCards(board);
        var cardInts = StringToCards(cards);

        var handRank = _evaluator!.Evaluate(cardInts, boardInts);
        var rankClass = _evaluator!.GetRankClass(handRank);
        Assert.AreEqual(expectedRankClass, rankClass);
    }

    [DataTestMethod]
    [DataRow("AsKsQsJsTs", "4s2s", "Straight Flush")] // StraightFlush 
    [DataRow("TsThTcTd2d", "AcKc", "Four of a Kind")] // Quads
    [DataRow("TsTh5s5h2d", "AsTc", "Full House")] // FullHouse
    [DataRow("AsQsJsTs9s", "7s2s", "Flush")] // Flush
    [DataRow("QhJdTh", "AcKc", "Straight")] // Straight
    [DataRow("TsTh5s3s", "AdTc", "Three of a Kind")] // Trips
    [DataRow("AsTh5s3s", "AdTs", "Two Pair")] // Two pair
    [DataRow("AsTh5s3s2d", "Kd3d", "Pair")] // Pair
    [DataRow("AsTh5s3s2d", "KdQs", "High Card")] // High Card
    public void ClassToStringTest(string board, string cards, string expectedRankClass)
    {
        var boardInts = StringToCards(board);
        var cardInts = StringToCards(cards);

        var handRank = _evaluator!.Evaluate(cardInts, boardInts);
        var rankClassInt = _evaluator!.GetRankClass(handRank);
        var rankClass = _evaluator!.ClassToString(rankClassInt);
        Assert.AreEqual(expectedRankClass, rankClass);
    }

    [DataTestMethod]
    [DataRow("AcKcQcJcTc", "4s2s", 0)] // StraightFlush 
    [DataRow("7s5s4h3h2h", "9s8s", 1)] // High Card
    public void GetFiveCardRankPercentageTest(string board, string cards, double expectedPct)
    {
        var boardInts = StringToCards(board);
        var cardInts = StringToCards(cards);

        var handRank = _evaluator!.Evaluate(cardInts, boardInts);
        var rankClass = _evaluator!.GetFiveCardRankPercentage(handRank);
        Assert.AreEqual(expectedPct, rankClass, 0.01);
    }
}