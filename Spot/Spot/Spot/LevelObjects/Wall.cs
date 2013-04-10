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
    class Wall : Sprite
    {
        protected Rectangle boundingBox;
        protected ContentManager content;
        public int imageId;
        private bool textureLoaded = false;
        public override Rectangle BoundingBox { get { return boundingBox; } }//this is needed for collision
        public int Width { get { return width; } }
        public int Height { get { return height; } }

        public Wall()
        {
        }

        public Wall(Vector2 Position, int theWidth, int theHeight, int id, string myName)
        {
            // isVisible = Visible;
            name = myName;
            position = Position;
            width = theWidth;
            height = theHeight;
            content = Game1.Instance().getContent();
            imageId = id; 
            boundingBox = new Rectangle((int)position.X, (int)position.Y, width, height);//this is needed to draw
            
            if (myName == "puzzleHole")
            {
                id -= 2;
                animationRect = new Rectangle(id * 32, 0, width, height);
                texture = content.Load<Texture2D>("PuzzlePanelSprites/numberTiles");
                textureLoaded = true;
            }
        }

        public Wall(Vector2 Position, int theWidth, int theHeight, string tex, string myName)
        {
            name = myName;
            position = Position;
            width = theWidth;
            height = theHeight;
            content = Game1.Instance().getContent();
            texture = content.Load<Texture2D>(tex);
            boundingBox = new Rectangle((int)position.X, (int)position.Y, width, height);//this is needed to draw
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            if (visible == true)
            {
                if(textureLoaded == false)
                    switch (imageId)
                    {
                        case 1:
                            texture = content.Load<Texture2D>("LevelObjects/bricktile");
                            break;

                        case 2:
                            texture = content.Load<Texture2D>("LevelObjects/woodtile");
                            break;

                        case 3:
                            texture = content.Load<Texture2D>("LevelObjects/Block3");
                            break;

                        case 4:
                            texture = content.Load<Texture2D>("LevelObjects/SquareHole");
                            break;

                        case 5:
                            texture = content.Load<Texture2D>("LevelObjects/TestPuzzleBlock");
                            break;

                        case 6:
                            texture = content.Load<Texture2D>("LevelObjects/NumOne");
                            break;
                    }
                if(name != "puzzleHole")
                    base.Draw(spriteBatch, camera);
                else
                    spriteBatch.Draw(texture, position - camera, animationRect, Color.White);
            }
        }

        public virtual void interact()
        {
            
        }

        public virtual void activateTrigger()
        {

        }
    }
}
