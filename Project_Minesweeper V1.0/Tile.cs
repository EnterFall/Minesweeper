using System;
using System.Collections.Generic;
using System.Linq;
namespace Minesweeper
{
    public enum Tile
    {
        HiddenSafe,
        BombAround1,
        BombAround2,
        BombAround3,
        BombAround4,
        BombAround5,
        BombAround6,
        BombAround7,
        BombAround8,
        Empty,
        HiddenBomb,
        VisibleBomb,
        FlagOnSafe,
        FlagOnBomb
    }
}
