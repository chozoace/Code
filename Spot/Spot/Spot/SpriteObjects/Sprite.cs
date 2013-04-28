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
    class Sprite
    {
        ContentManager myContent;
        SpriteBatch mySpriteBatch;

        public bool visible = true;
        public string name;
        public Sprite collidingWall;
        protected int width;
        protected int height;
        public Vector2 position;
        protected Vector2 previousPosition;
        public Texture2D texture;
        protected List<Wall> wallList;
        public bool isEnemy = false;
        public virtual Rectangle BoundingBox { get { return new Rectangle((int)position.X, (int)position.Y, width, height); } }
        protected Rectangle animationRect;
        public String currentAnimation;
        protected int currentFrame;
        protected int totalFrames;
        protected Timer animTimer = new Timer();

        public Sprite()
        {
            myContent = Game1.Instance().getContent();
            mySpriteBatch = Game1.Instance().getSpriteBatch();
        }

        public Sprite(String tex, float xpos = 0, float ypos = 0)
        {
            position = new Vector2(xpos, ypos);
            myContent = Game1.Instance().getContent();
            mySpriteBatch = Game1.Instance().getSpriteBatch();

            texture = myContent.Load<Texture2D>(tex);
            width = texture.Width;
            height = texture.Height;
        }

        public virtual void Update()
        {
            //get the keystate and update movement

        }

        public virtual void UpdateAnimation(object sender, ElapsedEventArgs e)
        {
            currentFrame = animationRect.X / 64;
            totalFrames = (texture.Width / 64) - 1;

            if (currentFrame >= totalFrames)
            {
                //startover
                //currentFrame = 0;
                animationRect = new Rectangle(0, 0, width, height);
            }
            else
            {
                //continue
                animationRect = new Rectangle((currentFrame + 1) * 64, 0, width, height);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            if(visible)
                spriteBatch.Draw(texture, position - camera, Color.White);
        }

        public virtual bool CheckCollision(Rectangle collisionBox)
        {
            wallList = LevelConstructor.Instance().getWallList();
            foreach (Wall wall in wallList)
            {
                if (wall.BoundingBox.Intersects(collisionBox) && wall.visible)
                {
                    collidingWall = wall;
                    return true;
                }
            }
            return false;
        }

        public Vector2 getPosition()
        {
            return position;
        }

        public virtual void setPosition(Vector2 newPos)
        {
            position = newPos;
        }
    }
}
