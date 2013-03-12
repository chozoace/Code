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
    class PuzzlePanel : Sprite
    {
        string puzzlePanelLevelOne = "Content/XML/PuzzleTestScreen.xml";
        List<Sprite> spriteList = new List<Sprite>();
        public List<OpenPuzzleSlot> holeList = new List<OpenPuzzleSlot>();
        KeyboardState myKeyState, previousKeyState;
        public PuzzleCursor cursor;
        static PuzzlePanel instance;
        int[,] puzzleSlots = new int[3, 3];
        
        public PuzzlePanel()
        {
            cursor = new PuzzleCursor(new Vector2(100,100));
            instance = this;
        }

        public static PuzzlePanel Instance()
        {
            return instance;
        }

        public void loadPanelXML()
        {
            LevelManager.Instance().level.XmlLoad(puzzlePanelLevelOne, "puzzle");
            addToSpriteList(cursor);
        }

        public void addToHoleList(OpenPuzzleSlot sprite)
        {
            holeList.Add(sprite);
        }

        public void addToSpriteList(Sprite sprite)
        {
            spriteList.Add(sprite);
        }
        
        public override void  Update()
        {
            foreach (Sprite sprite in spriteList)
            {
                sprite.Update();
            }
        }

        public void drawPanel(SpriteBatch spriteBatch, Vector2 camera)
        {
            foreach (Sprite sprite in spriteList)
            {
                sprite.Draw(spriteBatch, new Vector2(0, 0));
            }
        }
    }
}
