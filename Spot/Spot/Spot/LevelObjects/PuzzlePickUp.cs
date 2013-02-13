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
    class PuzzlePickUp : Sprite
    {
        public override void Update()
        {
            if (CheckCollision(BoundingBox))
            {
                LevelManager.Instance().removefromSpriteList(this);
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
