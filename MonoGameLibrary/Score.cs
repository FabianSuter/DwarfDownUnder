namespace MonoGameLibrary;

public class Score
{
    private int _score;

    /// <summary>
    /// Creates a new score tracker initialized to zero.
    /// </summary>
    public Score()
    {
        _score = 0;
    }

    /// <summary>
    /// Gets the current score.
    /// </summary>
    public int GetScore()
    {
        return _score;
    }

    /// <summary>
    /// Resets the score to zero.
    /// </summary>
    public void ResetScore()
    {
        _score = 0;
    }

    /// <summary>
    /// Increments the score by the specified amount.
    /// </summary>
    /// <param name="amount">Amount to increase. Should be positive</param>
    public void IncrementScore(int amount)
    {
        if (amount > 0)
        {
            _score += amount;
        }
    }
}