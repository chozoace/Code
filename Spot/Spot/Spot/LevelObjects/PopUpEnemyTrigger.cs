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
    class PopUpEnemyTrigger : Wall
    {
        bool active = true;

        public PopUpEnemyTrigger(Vector2 Position, int theWidth, int theHeight, int id)
            : base(Position, theWidth, theHeight, id, "PopupEnemyTrigger")
        {
            texture = content.Load<Texture2D>("LevelObjects/woodtile");
        }

        public override void interact()
        {
            if (active)
            {
                Enemy enemy = new MeleeEnemy(new Vector2(position.X + 200, position.Y - 66));
                LevelManager.Instance().addToSpriteList(enemy);
                LevelManager.Instance().addToEnemyList(enemy);
                enemy.speed.Y = -8;
                enemy.speed.X = 0;
                enemy.currentEvent = new EventHandler(enemy.popInState);
                Debug.WriteLine("touching");
                active = false;
            }
        }
    }
}
