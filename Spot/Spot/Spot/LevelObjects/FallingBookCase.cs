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
using System.Timers;

namespace Spot
{
    class FallingBookCase : Wall
    {
        public FallingBookCase(Vector2 newPos)
        {
            position = newPos;
            width = 64;
            height = 64;
            boundingBox = new Rectangle((int)position.X, (int)position.Y, width, height);
            currentAnimation = "LevelObjects/fallingShelf";

            texture = Game1.Instance().Content.Load<Texture2D>(currentAnimation);
            
            animationRect = new Rectangle(0, 0, width, height);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            spriteBatch.Draw(texture, position - camera, animationRect, Color.White);
        }

        public override void activateTrigger()
        {
            animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
            animTimer.Enabled = true;
        }

        public override void UpdateAnimation(object sender, ElapsedEventArgs e)
        {
            currentFrame = animationRect.X / 64;
            totalFrames = (texture.Width / 64) - 1;

            if (currentFrame >= totalFrames)
            {
                //startover
                //currentFrame = 0;
                Enemy enemy = new MeleeEnemy(position);
                LevelManager.Instance().addToEnemyList(enemy);
                LevelManager.Instance().addToSpriteList(enemy);

                LevelManager.Instance().removefromSpriteList(this);
                LevelConstructor.Instance().removefromWallList(this);
                animTimer.Dispose();
            }
            else
            {
                //continue
                animationRect = new Rectangle((currentFrame + 1) * 64, 0, width, height);
            }
        }
    }
}
