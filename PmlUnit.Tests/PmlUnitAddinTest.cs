using Aveva.ApplicationFramework;
using Moq;

using NUnit.Framework;

namespace PmlUnit.Tests
{
    [TestFixture]
    [TestOf(typeof(PmlUnitAddin))]
    class PmlUnitAddinTest
    {
        [Test]
        public void Start_RegistersTestCaseProvider()
        {
            // Arrange
            var mock = new Mock<ServiceManager>();
            mock.Setup(manager => manager.AddService(
                typeof(TestCaseProvider), It.IsNotNull<TestCaseProvider>()
            ));
            var addin = new PmlUnitAddin();
            // Act
            addin.Start(mock.Object);
            // Assert
            mock.Verify(manager => manager.AddService(
                typeof(TestCaseProvider), It.IsNotNull<TestCaseProvider>()
            ));
        }
    }
}
