namespace GreedyDiceGameSharp
{
    public class DiceValue
    {
        public int Face { get; }
        public bool Locked { get; }
        public bool Selected { get; }

        public DiceValue(int face, bool locked, bool selected)
        {
            Face = face;
            Locked = locked;
            Selected = selected;
        }
    }

    public class Dice
    {
        public bool Selected { get; private set; }
        public bool Locked { get; private set; }
        public int Face { get; private set; }

        public void Roll()
        {
            if (!Locked)
            {
                var face = RandomHelper.Generator.Next(1, 6);
                Face = face;
            }
        }

        public void Lock()
        {
            Locked = true;
        }

        public void Unlock()
        {
            Locked = false;
        }

        public void Select()
        {
            Selected = true;
        }

        public void Deselect()
        {
            Selected = false;
        }
    }
}
