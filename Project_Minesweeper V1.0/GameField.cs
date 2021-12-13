using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    class GameField : IEnumerable<Tile>, IReadOnlyCollection<Tile>
    {
        private Tile[,] field;
        private Minesweeper game;

        public GameField(Minesweeper game, int width, int height)
        {
            this.game = game;
            field = new Tile[width, height];
        }

        public Tile this[int x, int y]
        {
            get => field[x, y];
            set
            {
                var oldValue = field[x, y];
                game.OnFieldChanged(new FieldChangedEventArgs(x, y, oldValue, value));
                field[x, y] = value;
            }
        }

        public int Count => field.Length;

        public IEnumerator<Tile> GetEnumerator()
        {
            foreach (var tile in field)
            {
                yield return tile;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
