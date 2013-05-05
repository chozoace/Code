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
using System.Xml;

namespace Spot
{
    class Weapon : Sprite
    {
        public int damage;
        public int speed;
        public string weaponName;
        public string weaponType;
        public int range;
        string myTex;
        static int DebugCounter = 1;

        XmlDocument weapons;
        XmlNode myNode;

        public Weapon(int weaponLocation, string xmlFile, float xpos, float ypos)
        {
            weapons = new XmlDocument();
            weapons.Load(xmlFile);   

            int counter = 0;
            myNode = weapons.FirstChild;

            while (counter <= weaponLocation)
            {
                if (counter == 0)
                {
                    myNode = myNode.FirstChild;
                }
                else
                {
                    myNode = myNode.NextSibling;
                }
                counter++;
            }

            damage = int.Parse(myNode.Attributes.GetNamedItem("damage").Value);
            speed = int.Parse(myNode.Attributes.GetNamedItem("attackSpeed").Value);
            range = int.Parse(myNode.Attributes.GetNamedItem("range").Value);
            weaponName = myNode.Attributes.GetNamedItem("name").Value;
            weaponType = myNode.Attributes.GetNamedItem("weaponType").Value;

            Debug.WriteLine("Num weapons: " + DebugCounter + " name " + weaponName);
            DebugCounter++;

            myTex = "WeaponPickups/" + weaponName;
            texture = Game1.Instance().Content.Load<Texture2D>(myTex);
            width = 32;
            height = 32;
            position.X = xpos;
            position.Y = ypos;
        }

        public override void Update()
        {
            if (CheckCollision(BoundingBox) && visible)
            {
                visible = false;
                //LevelManager.Instance().removefromSpriteList(this);
            }
        }

        public override bool CheckCollision(Rectangle collisionBox)
        {
            Player player = LevelManager.Instance().player;

            if (BoundingBox.Intersects(player.BoundingBox))
            {
                player.weaponPickUp(this);
                return true;
            }

            return false;
        }
    }
}
