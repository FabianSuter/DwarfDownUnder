using MonoGameLibrary;
using Xunit;

namespace DwarfDownUnderTest;

public class ScoreTest
{
	[Fact]
	public void NewScore_IsZero()
	{
		// Arrange
		var score = new Score();

		// Act
		var current = score.GetScore();

		// Assert
		Assert.Equal(0, current);
	}

	[Fact]
	public void IncrementScore_AddsAmount_WhenPositive()
	{
		// Arrange
		var score = new Score();

		// Act
		score.IncrementScore(10);
		var current = score.GetScore();

		// Assert
		Assert.Equal(10, current);
	}

	[Fact]
	public void IncrementScore_SequentialIncrements_ResultIsSum()
	{
		// Arrange
		var score = new Score();

		// Act
		score.IncrementScore(10);
		score.IncrementScore(5);
		var current = score.GetScore();

		// Assert
		Assert.Equal(15, current);
	}

	[Fact]
	public void IncrementScore_IgnoresZeroAmount()
	{
		// Arrange
		var score = new Score();

		// Act
		score.IncrementScore(0);
		var current = score.GetScore();

		// Assert
		Assert.Equal(0, current);
	}

	[Fact]
	public void IncrementScore_IgnoresNegativeAmount()
	{
		// Arrange
		var score = new Score();

		// Act
		score.IncrementScore(-5);
		var current = score.GetScore();

		// Assert
		Assert.Equal(0, current);
	}

	[Fact]
	public void DecrementScore_DecreasesByAmount()
	{
		// Arrange
		var score = new Score();
		score.IncrementScore(10);

		// Act
		score.DecrementScore(4);
		var current = score.GetScore();

		// Assert
		Assert.Equal(6, current);
	}

	[Fact]
	public void DecrementScore_ClampsToZero_WhenOverDecrement()
	{
		// Arrange
		var score = new Score();
		score.IncrementScore(10);

		// Act
		score.DecrementScore(100);
		var current = score.GetScore();

		// Assert
		Assert.Equal(0, current);
	}

	[Fact]
	public void DecrementScore_IgnoresZeroAmount()
	{
		// Arrange
		var score = new Score();
		score.IncrementScore(5);

		// Act
		score.DecrementScore(0);
		var current = score.GetScore();

		// Assert
		Assert.Equal(5, current);
	}

	[Fact]
	public void DecrementScore_IgnoresNegativeAmount()
	{
		// Arrange
		var score = new Score();
		score.IncrementScore(5);

		// Act
		score.DecrementScore(-3);
		var current = score.GetScore();

		// Assert
		Assert.Equal(5, current);
	}

	[Fact]
	public void ResetScore_SetsToZero()
	{
		// Arrange
		var score = new Score();
		score.IncrementScore(20);

		// Act
		score.ResetScore();
		var current = score.GetScore();

		// Assert
		Assert.Equal(0, current);
	}
}
