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

    [TestMethod()]
    public void EvaluateFiveTest()
    {
        Assert.Fail();
    }

    [TestMethod()]
    public void EvaluateSixTest()
    {
        Assert.Fail();
    }

    [DataTestMethod]
    [DataRow("AsKsQs5s3s", "JsTs", "JhTh", true)]
    public void EvaluateSevenTest(string board, string cards1, string cards2, bool cards1IsBetter)
    {
        var boardInts = StringToCards(board);
        var card1Ints = StringToCards(cards1);
        var card2Ints = StringToCards(cards2);

        var cards1Res = _evaluator!.Evaluate(card1Ints, boardInts);
        var cards2Res = _evaluator!.Evaluate(card2Ints, boardInts);
        Assert.AreEqual(cards1Res < cards2Res, cards1IsBetter);
    }

    [TestMethod()]
    public void GetRankClassTest()
    {
        Assert.Fail();
    }

    [TestMethod()]
    public void ClassToStringTest()
    {
        Assert.Fail();
    }

    [TestMethod()]
    public void GetFiveCardRankPercentageTest()
    {
        Assert.Fail();
    }
}