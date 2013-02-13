using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Spot
{
    class CirclePuzzlePiece : PuzzlePickUp
    {
        public CirclePuzzlePiece(Vector2 newPos)
        {
            width = 32;
            height = 32;
            texture = Game1.Instance().Content.Load<Texture2D>("Enemy/ToyBall_Afterdeath");
            position = newPos;
        }
    }
}
