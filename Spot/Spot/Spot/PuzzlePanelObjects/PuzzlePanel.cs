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
    class PuzzlePanel : Sprite
    {
        string puzzlePanelLevelOne = "Content/XML/PuzzleTestScreen.xml";
        List<Sprite> spriteList = new List<Sprite>();
        public List<OpenPuzzleSlot> holeList = new List<OpenPuzzleSlot>();
        KeyboardState myKeyState, previousKeyState;
        public PuzzleCursor cursor;
        static PuzzlePanel instance;
        public OpenPuzzleSlot[,] puzzleSlots = new OpenPuzzleSlot[3, 3];
        Sprite memoryFragment = null;

        Timer fragmentTimer = new Timer(1500);
        EventHandler currentEvent;
        public EventArgs eArgs = new EventArgs();
        Song tadaSound;
        bool winEventOccured = false;
        
        public PuzzlePanel()
        {
            cursor = new PuzzleCursor(new Vector2(100,100));
            currentEvent = new EventHandler(nothingState);
            instance = this;
            tadaSound = Game1.Instance().Content.Load<Song>("Music/Tada");
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

        public void nothingState(object sender, EventArgs e)
        {

        }

        public void winState(object sender, EventArgs e)
        {
            cursor.controlsLocked = true;

            if (memoryFragment == null)
            {
                memoryFragment = new Sprite("LevelObjects/AwesomeFace");
                addToSpriteList(memoryFragment);
                memoryFragment.position.X = 320;
                memoryFragment.position.Y = -32;
            }

            if (memoryFragment.position.Y < 240)
            {
                memoryFragment.position.Y += 2;
                Debug.WriteLine(memoryFragment.position);
            }
            else
            {
                if (!winEventOccured)
                {
                    Debug.WriteLine("reached");
                    MediaPlayer.Play(tadaSound);
                    currentEvent = new EventHandler(nothingState);
                    fragmentTimer = new Timer(1500);
                    fragmentTimer.Elapsed += new ElapsedEventHandler(endPuzzle);
                    fragmentTimer.Enabled = true;
                    winEventOccured = true;
                }
            }
        }

        public void endPuzzle(object sender, ElapsedEventArgs e)
        {
            fragmentTimer.Dispose();
            LevelManager.Instance().levelState = LevelManager.LevelState.Gameplay;
        }
        
        public override void  Update()
        {
            currentEvent(this, eArgs);

            foreach (Sprite sprite in spriteList)
            {
                sprite.Update();
            }
            //checks win
            if (puzzleSlots[0, 0].points + puzzleSlots[1, 1].points + puzzleSlots[2, 2].points == 1)//diagonal left to right
                if (puzzleSlots[0, 2].points + puzzleSlots[1, 1].points + puzzleSlots[2, 0].points == 1)//diagonal right to left
                        if (puzzleSlots[0, 1].points + puzzleSlots[1, 1].points + puzzleSlots[2, 1].points == 1)//column
                            if (puzzleSlots[1, 0].points + puzzleSlots[1, 1].points + puzzleSlots[1, 2].points == 1)//row
                            {
                                currentEvent = new EventHandler(winState);
                            }
        }

        public void puzzleComplete()
        {
            cursor.controlsLocked = true;

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
