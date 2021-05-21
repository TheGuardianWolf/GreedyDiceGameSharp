namespace GreedyDiceGameSharp
{
    public class Player
    {
        public string Name { get; private set; }
        protected Score Score { get; set; } = new Score(0);

        public Player(string name)
        {
            ChangeName(name);
        }

        public void ChangeName(string name)
        {
            Name = name;
        }

        public int GetScore() => Score.Value;

        public void IncreaseScore(int score) 
        {
            Score = new Score(Score.Value + score);
        }

        public void ResetScore()
        {
            Score = new Score(0);
        }

        public void Reset()
        {
            Score = new Score(0);
        }

    }
}
