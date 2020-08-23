using Microsoft.AspNetCore.Mvc;
using Project.Controllers;
using Project.Models;
using Xunit;

namespace Project.test
{
    public class GradeControllerTests
    {
        [Fact]
        public void GradeViewResultNotNull()
        {
            GradeName g = new GradeName();
            // Arrange
            GradeController controller = new GradeController();
            // Act
            ViewResult result = controller.Grade(g) as ViewResult;
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GradeEditViewResultNotNull()
        {
            GradeName g = new GradeName();
            // Arrange
            GradeController controller = new GradeController();
            // Act
            ViewResult result = controller.GradeEdit(g) as ViewResult;
            // Assert
            Assert.NotNull(result);
        }

      
       

    }
}
