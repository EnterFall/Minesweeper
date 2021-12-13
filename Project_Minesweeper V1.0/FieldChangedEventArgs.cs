using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public delegate void FieldChangedEventHandler(Minesweeper sender, FieldChangedEventArgs e);

    public class FieldChangedEventArgs : EventArgs
    {
        public int X { get; }
        public int Y { get; }
        public Tile OldValue { get; }
        public Tile NewValue { get; }

        public FieldChangedEventArgs(int x, int y, Tile oldValue, Tile newValue)
        {
            X = x;
            Y = y;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
