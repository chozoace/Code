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
    class BookCaseTrigger : Wall
    {
        bool active = true;

        public BookCaseTrigger(Vector2 Position, int theWidth, int theHeight, int id)
            : base(Position, theWidth, theHeight, id , "bookCaseTrigger")
        {
            texture = content.Load<Texture2D>("LevelObjects/woodtile");

        }

        public override void interact()
        {
            if (active)
            {
                List<Wall> wallList = LevelConstructor.Instance().getWallList();
                foreach (Wall wall in wallList)
                {
                    if (wall.currentAnimation == "LevelObjects/fallingShelf")
                    {
                        Debug.WriteLine("trigger");
                        wall.activateTrigger();
                    }
                }
                active = false;
            }
        }
    }
}
