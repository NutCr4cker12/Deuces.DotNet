using Deuces;

namespace Tests;

[TestClass()]
public class CardTests
{
    [DataTestMethod()]
    [DataRow("As", "[ A ♠ ]")]
    [DataRow("Kh", "[ K ♥ ]")]
    [DataRow("Qd", "[ Q ♦ ]")]
    [DataRow("Jc", "[ J ♣ ]")]
    public void IntToPrettyStrTest(string card, string expected)
    {
        Card.UseColoredCards = false;
        var cardInt = Card.New(card);
        var actual = Card.IntToPrettyStr(cardInt);
        Assert.AreEqual(expected, actual);
    }
}