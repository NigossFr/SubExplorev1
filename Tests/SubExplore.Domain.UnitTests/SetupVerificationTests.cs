namespace SubExplore.Domain.UnitTests;

using FluentAssertions;
using Xunit;

/// <summary>
/// Tests de vérification de la configuration du projet de tests unitaires Domain.
/// Ces tests vérifient que xUnit, FluentAssertions et le setup de base fonctionnent correctement.
/// </summary>
public class SetupVerificationTests
{
    [Fact]
    public void XUnit_Should_Execute_Tests_Successfully()
    {
        // Arrange
        var expectedValue = true;

        // Act
        var actualValue = true;

        // Assert
        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void FluentAssertions_Should_Work_Correctly()
    {
        // Arrange
        var expectedString = "SubExplore Domain Tests";

        // Act
        var actualString = "SubExplore Domain Tests";

        // Assert
        actualString.Should().Be(expectedString);
        actualString.Should().NotBeNullOrEmpty();
        actualString.Should().Contain("Domain");
    }

    [Fact]
    public void FluentAssertions_Should_Provide_Readable_Assertions_For_Collections()
    {
        // Arrange
        var numbers = new[] { 1, 2, 3, 4, 5 };

        // Act & Assert
        numbers.Should().NotBeNull();
        numbers.Should().HaveCount(5);
        numbers.Should().Contain(3);
        numbers.Should().BeInAscendingOrder();
    }

    [Fact]
    public void FluentAssertions_Should_Provide_Readable_Assertions_For_Objects()
    {
        // Arrange
        var testObject = new { Name = "Test", Value = 42 };

        // Act & Assert
        testObject.Should().NotBeNull();
        testObject.Name.Should().Be("Test");
        testObject.Value.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(5, 5, 10)]
    [InlineData(-1, 1, 0)]
    [InlineData(0, 0, 0)]
    public void XUnit_Theory_Should_Work_With_Multiple_Datasets(int a, int b, int expected)
    {
        // Act
        var result = a + b;

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Test_Framework_Versions_Should_Be_Compatible()
    {
        // Arrange - Vérifier que nous pouvons instancier des objets de test
        var testData = new List<string> { "xUnit", "FluentAssertions", "Moq" };

        // Act & Assert
        testData.Should().HaveCount(3);
        testData.Should().Contain("FluentAssertions");
        testData.Should().AllSatisfy(item => item.Should().NotBeNullOrWhiteSpace());
    }
}
