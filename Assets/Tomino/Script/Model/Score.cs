using System.Collections.Generic;

namespace Tomino
{
    public class Score
    {
        public int Value { get; private set; }

        private readonly Dictionary<int, int> scoreForClearedRows = new()
        {
            { 1, 100 },
            { 2, 300 },
            { 3, 500 },
            { 4, 800 }
        };

        public void RowsCleared(int count)
        {
            _ = scoreForClearedRows.TryGetValue(count, out int valueIncrease);
            Value += valueIncrease;
        }

        public void PieceFinishedFalling(int rowsCount)
        {
            Value += rowsCount * 2;
        }

        public void PieceMovedDown()
        {
            Value++;
        }
    }
}
