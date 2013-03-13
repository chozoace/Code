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
        public int points = 0;
        int row;
        int column;
        bool occupied = false;
        string emptyTex = "LevelObjects/SquareHole";
        string circleTex = "LevelObjects/SquareHoleRed";
        string triangleTex = "LevelObjects/SquareHoleBlue";
        string squareTex = "LevelObjects/SquareHoleGreen";


        public OpenPuzzleSlot(Vector2 newPos, int theWidth, int theHeight, bool interactable, int myRow, int myColumn)
        {
            position = newPos;
            width = theWidth;
            height = theHeight;
            texture = Game1.Instance().Content.Load<Texture2D>(emptyTex);
            canInteract = interactable;
            row = myRow;
            column = myColumn;
            PuzzlePanel.Instance().puzzleSlots[row, column] = this;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            base.Draw(spriteBatch, camera);
        }

        public void interact(int currentPiece)
        {
            if (currentPiece == 1)
            {
                if (!occupied)
                {
                    Debug.WriteLine("circle Interact " + row + " " + column);
                    points = 1;
                    occupied = true;
                    texture = Game1.Instance().Content.Load<Texture2D>(circleTex);
                }
                else
                {
                    points = 0;
                    occupied = false;
                    texture = Game1.Instance().Content.Load<Texture2D>(emptyTex);
                }
            }
            else if (currentPiece == 2)
            {
                if (!occupied)
                {
                    Debug.WriteLine("triangle Interact " + row + " " + column);
                    points = 2;
                    occupied = true;
                    texture = Game1.Instance().Content.Load<Texture2D>(triangleTex);
                }
                else
                {
                    points = 0;
                    occupied = false;
                    texture = Game1.Instance().Content.Load<Texture2D>(emptyTex);
                }
            }
            else if (currentPiece == 3)
            {
                if (!occupied)
                {
                    Debug.WriteLine("square Interact " + row + " " + column);
                    points = 3;
                    occupied = true;
                    texture = Game1.Instance().Content.Load<Texture2D>(squareTex);
                }
                else
                {
                    points = 0;
                    occupied = false;
                    texture = Game1.Instance().Content.Load<Texture2D>(emptyTex);
                }
            }
        }
    }
}
