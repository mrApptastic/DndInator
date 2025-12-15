namespace DndInator.Services
{
    public interface IDiceService
    {
        int RollDice(int sides);
    }

    public class DiceService : IDiceService
    {
        public int RollDice(int sides)
        {
            var random = new Random();
            return random.Next(1, sides + 1);
        }
    }
}