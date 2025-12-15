namespace DndInator.DiceService
{
    public class DiceService
    {
        public int RollDice(int sides)
        {
            var random = new Random();
            return random.Next(1, sides + 1);
        }
    }
}