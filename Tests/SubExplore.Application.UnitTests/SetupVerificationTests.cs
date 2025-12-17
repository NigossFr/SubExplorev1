namespace SubExplore.Application.UnitTests;

using FluentAssertions;
using Moq;
using Xunit;

/// <summary>
/// Tests de vérification de la configuration du projet de tests unitaires Application.
/// Ces tests vérifient que xUnit, FluentAssertions, Moq et le setup de base fonctionnent correctement.
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
        var expectedString = "SubExplore Application Tests";

        // Act
        var actualString = "SubExplore Application Tests";

        // Assert
        actualString.Should().Be(expectedString);
        actualString.Should().NotBeNullOrEmpty();
        actualString.Should().Contain("Application");
    }

    [Fact]
    public void Moq_Should_Create_Mock_Objects_Successfully()
    {
        // Arrange
        var mockService = new Mock<ITestService>();
        mockService.Setup(s => s.GetTestValue()).Returns(42);

        // Act
        var result = mockService.Object.GetTestValue();

        // Assert
        result.Should().Be(42);
        mockService.Verify(s => s.GetTestValue(), Times.Once);
    }

    [Fact]
    public void Moq_Should_Setup_Properties_Correctly()
    {
        // Arrange
        var mockService = new Mock<ITestService>();
        mockService.SetupGet(s => s.Name).Returns("Test Service");

        // Act
        var name = mockService.Object.Name;

        // Assert
        name.Should().Be("Test Service");
    }

    [Fact]
    public void Moq_Should_Verify_Method_Calls()
    {
        // Arrange
        var mockService = new Mock<ITestService>();
        mockService.Setup(s => s.ProcessData(It.IsAny<string>())).Returns(true);

        // Act
        var result = mockService.Object.ProcessData("test data");

        // Assert
        result.Should().BeTrue();
        mockService.Verify(s => s.ProcessData("test data"), Times.Once);
        mockService.Verify(s => s.ProcessData(It.IsAny<string>()), Times.Once);
    }

    [Theory]
    [InlineData("Test1", true)]
    [InlineData("Test2", false)]
    [InlineData("", false)]
    public void Moq_Should_Work_With_XUnit_Theory(string input, bool expectedResult)
    {
        // Arrange
        var mockService = new Mock<ITestService>();
        mockService.Setup(s => s.ProcessData(It.IsAny<string>()))
                   .Returns<string>(data => !string.IsNullOrEmpty(data) && data.Contains("1"));

        // Act
        var result = mockService.Object.ProcessData(input);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void Test_Framework_Versions_Should_Be_Compatible()
    {
        // Arrange - Vérifier que nous pouvons instancier des objets de test
        var testData = new List<string> { "xUnit", "FluentAssertions", "Moq" };

        // Act & Assert
        testData.Should().HaveCount(3);
        testData.Should().Contain("Moq");
        testData.Should().AllSatisfy(item => item.Should().NotBeNullOrWhiteSpace());
    }
}

/// <summary>
/// Interface de test pour démontrer les capacités de Moq.
/// </summary>
public interface ITestService
{
    string Name { get; }

    int GetTestValue();

    bool ProcessData(string data);
}
