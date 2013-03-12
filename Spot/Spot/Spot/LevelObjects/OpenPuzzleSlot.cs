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
    class OpenPuzzleSlot : Sprite
    {
        bool canInteract;
        int points = 1;
        int row;
        int column;


        public OpenPuzzleSlot(Vector2 newPos, int theWidth, int theHeight, bool interactable, int myRow, int myColumn)
        {
            position = newPos;
            width = theWidth;
            height = theHeight;
            texture = Game1.Instance().Content.Load<Texture2D>("LevelObjects/SquareHole");
            canInteract = interactable;
            row = myRow;
            column = myColumn;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            base.Draw(spriteBatch, camera);
        }

        public void interact(int currentPiece)
        {
            if (currentPiece == 1)
            {
                Debug.WriteLine("square Interact " + row + " " + column);
            }
            else if (currentPiece == 2)
            {
                Debug.WriteLine("Triangle Interact");
            }
        }
    }
}
