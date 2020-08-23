using Microsoft.AspNetCore.Mvc;
using Project.Controllers;
using Xunit;

namespace Project.test
{
    public class HomeControllerTests
    {
        [Fact]
        public void AboutViewResultNotNull()
        {
            // Arrange
            HomeController controller = new HomeController();
            // Act
            RedirectToActionResult result = controller.About() as RedirectToActionResult;
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void AddTechViewResultNotNull()
        {
            // Arrange
            HomeController controller = new HomeController();
            // Act
            RedirectToActionResult result = controller.AddTech() as RedirectToActionResult;
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void EditViewResultNotNull()
        {
            // Arrange
            HomeController controller = new HomeController();
            // Act
            RedirectToActionResult result = controller.Edit() as RedirectToActionResult;
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void LogoutViewResultNotNull()
        {
            // Arrange
            HomeController controller = new HomeController();
            // Act
            RedirectToActionResult result = controller.Logout() as RedirectToActionResult;
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void AboutViewNameEqualLogin()
        {
            // Arrange
            HomeController controller = new HomeController();
            // Act
            var result = controller.About();
            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectToActionResult.ActionName);
        }

       

    }
}
