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
    class PuzzlePanelObject : Sprite
    {
        PuzzlePanel myPanel;
        bool canStartPuzzle = false;
        bool active = true;

        public PuzzlePanelObject(Vector2 newPos, int currentPanel)
        {
            position = newPos;
            width = 32;
            height = 32;
            texture = Game1.Instance().Content.Load<Texture2D>("LevelObjects/TestPuzzleBlock");

            switch (currentPanel)
            {
                case 1:
                    myPanel = LevelManager.Instance().panelOne;
                    break;
                case 2:
                    myPanel = LevelManager.Instance().panelTwo;
                    break;
                case 3:
                    myPanel = LevelManager.Instance().panelThree;
                    break;
                case 4:
                    myPanel = LevelManager.Instance().panelFour;
                    break;
                case 5:
                    myPanel = LevelManager.Instance().panelFive;
                    break;
            }
        }

        public override void Update()
        {
            if (active)
            {
                if (CheckCollision(BoundingBox) && !myPanel.canStartPuzzle && !myPanel.winEventOccured)
                {
                    myPanel.canStartPuzzle = true;
                }
                else if (!CheckCollision(BoundingBox) && myPanel.canStartPuzzle && myPanel.winEventOccured)
                {
                    myPanel.canStartPuzzle = false;
                }
            }
            if (myPanel.winEventOccured)
            {
                active = false;
            }
        }

        public override bool CheckCollision(Rectangle collisionBox)
        {
            Player player = LevelManager.Instance().player;

            if (BoundingBox.Intersects(player.BoundingBox))
            {
                return true;
            }

            return false;
        }
    }
}
