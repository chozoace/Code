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
        public PuzzlePanelObject(Vector2 newPos)
        {
            position = newPos;
            width = 32;
            height = 32;
            texture = Game1.Instance().Content.Load<Texture2D>("LevelObjects/TestPuzzleBlock");
        }

        public override void Update()
        {
            Debug.WriteLine(LevelManager.Instance().panelOne.winEventOccured);
            if (CheckCollision(BoundingBox) && !LevelManager.Instance().player.canStartPuzzle && !LevelManager.Instance().panelOne.winEventOccured)
            {
                LevelManager.Instance().player.canStartPuzzle = true;
            }
            else if (!CheckCollision(BoundingBox) && LevelManager.Instance().player.canStartPuzzle)
            {
                LevelManager.Instance().player.canStartPuzzle = false;
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
