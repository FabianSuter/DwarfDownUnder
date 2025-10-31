using MonoGameLibrary;
using Microsoft.Xna.Framework;

namespace DwarfDownUnderTest;

public class CircleTest
{
    [Fact]
    public void Circle_SameCircle_IsEqual()
    {
        // Arrange
        Circle circle = new Circle(10, 10, 5);
        Circle otherCircle = new Circle(10, 10, 5);

        // Act
        bool areEqual = circle.Equals(otherCircle);

        // Assert
        Assert.True(areEqual);
    }

    [Fact]
    public void Circle_SimilarCircleWithPoint_IsEqual()
    {
        // Arrange
        Circle circle = new Circle(10, 10, 5);
        Point point = new Point(10, 10);
        Circle otherCircle = new Circle(point, 5);

        // Act
        bool areEqual = circle.Equals(otherCircle);

        // Assert
        Assert.True(areEqual);
    }

    [Fact]
    public void Circle_DifferentCircle_IsNotEqual()
    {
        // Arrange
        Circle circle = new Circle(10, 10, 5);
        Circle otherCircle = new Circle(15, 15, 5);

        // Act
        bool areEqual = circle.Equals(otherCircle);

        // Assert
        Assert.False(areEqual);
    }

    [Fact]
    public void Circle_DifferentRadius_IsNotEqual()
    {
        // Arrange
        Circle circle = new Circle(10, 10, 5);
        Circle otherCircle = new Circle(10, 10, 10);

        // Act
        bool areEqual = circle.Equals(otherCircle);

        // Assert
        Assert.False(areEqual);
    }

    [Fact]
    public void Circle_IntersectingCircles_ReturnsTrue()
    {
        // Arrange
        Circle circle1 = new Circle(0, 0, 5);
        Circle circle2 = new Circle(3, 4, 5); // Distance between centers is 5, sum of radii is 10

        // Act
        bool intersects = circle1.Intersects(circle2);

        // Assert
        Assert.True(intersects);
    }

    [Fact]
    public void Circle_NonIntersectingCircles_ReturnsFalse()
    {
        // Arrange
        Circle circle1 = new Circle(0, 0, 5);
        Circle circle2 = new Circle(20, 20, 5); // Distance between centers is greater than sum of radii

        // Act
        bool intersects = circle1.Intersects(circle2);

        // Assert
        Assert.False(intersects);
    }

    [Fact]
    public void Circle_EmptyCircle_IsEmpty()
    {
        // Arrange
        Circle emptyCircle = Circle.Empty;

        // Act
        bool isEmpty = emptyCircle.IsEmpty;

        // Assert
        Assert.True(isEmpty);
    }

    [Fact]
    public void Circle_NonEmptyCircle_IsNotEmpty()
    {
        // Arrange
        Circle circle = new Circle(10, 10, 5);

        // Act
        bool isEmpty = circle.IsEmpty;

        // Assert
        Assert.False(isEmpty);
    }

    [Fact]
    public void Circle_Location_ReturnCorrectPoint()
    {
        // Arrange
        Circle circle = new Circle(10, 15, 5);

        // Act
        Point location = circle.Location;

        // Assert
        Assert.Equal(new Point(10, 15), location);
    }
}