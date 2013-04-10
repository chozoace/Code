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
using System.Xml;

namespace Spot
{
    class PuzzlePanel : Sprite
    {
        string puzzlePanelLevelOne = "Content/XML/PuzzlePanel1.xml";
        string panelString;
        List<Sprite> spriteList = new List<Sprite>();
        public List<OpenPuzzleSlot> holeList = new List<OpenPuzzleSlot>();
        KeyboardState myKeyState, previousKeyState;
        public PuzzleCursor cursor;
        public OpenPuzzleSlot[,] puzzleSlots = new OpenPuzzleSlot[3, 3];
        Sprite memoryFragment = null;

        Timer fragmentTimer = new Timer(1500);
        EventHandler currentEvent;
        public EventArgs eArgs = new EventArgs();
        Song tadaSound;
        public bool winEventOccured = false;

        public bool canStartPuzzle = false;

        int diagLeftRight;
        int diagRightLeft;
        int centColumn;
        int centRow;
        
        public PuzzlePanel(int panelNum, string panelXML)
        {
            cursor = new PuzzleCursor(new Vector2(100,100), panelNum);
            currentEvent = new EventHandler(nothingState);
            tadaSound = Game1.Instance().Content.Load<Song>("Music/Tada");

            panelString = panelXML;

            //diagLeftRight = 1;
            //diagRightLeft = 1;
            //centColumn = 1;
            //centRow = 1;
        }

        public void loadPanelXML(int currentPanel)
        {
            LevelManager.Instance().level.XmlLoad(puzzlePanelLevelOne, "puzzle", currentPanel);
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
            }
            else
            {
                if (!winEventOccured)
                {
                    MediaPlayer.Play(tadaSound);
                    currentEvent = new EventHandler(nothingState);
                    fragmentTimer = new Timer(1500);
                    fragmentTimer.Elapsed += new ElapsedEventHandler(endPuzzle);
                    fragmentTimer.Enabled = true;
                    winEventOccured = true;
                    List<Sprite> spriteList = new List<Sprite>();
                    spriteList = LevelManager.Instance().spriteList;

                    foreach (Sprite sprite in spriteList)
                    {
                        if (sprite.name == "Door")
                        {
                            if (sprite.position.X > LevelManager.Instance().player.position.X - 150 && sprite.position.X < LevelManager.Instance().player.position.X)
                                sprite.visible = false;
                            else if (sprite.position.X < LevelManager.Instance().player.position.X + 150 && sprite.position.X > LevelManager.Instance().player.position.X)
                                sprite.visible = false;
                        }
                    }
                }
            }
        }

        public void endPuzzle(object sender, ElapsedEventArgs e)
        {
            fragmentTimer.Dispose();
            LevelManager.Instance().levelState = LevelManager.LevelState.Gameplay;
        }

        public void assignWinConditions(int blockNum, int winNum)
        {
            if (winNum == 10)
            {
                winNum = -1;
            }
            switch (blockNum)
            {
                case 1: case 8:
                    diagLeftRight = winNum;
                    break;
                case 2: case 7:
                    centColumn = winNum;
                    break;
                case 3: case 6:
                    diagRightLeft = winNum;
                    break;
                case 4: case 5:
                    centRow = winNum;
                    break;
            }
            if (blockNum == 8)
            {
                Debug.WriteLine("diagLR " + diagLeftRight + " diagRL " + diagRightLeft + " row " + centRow + " column " + centColumn);
            }
        }
        
        public override void  Update()
        {
            currentEvent(this, eArgs);

            foreach (Sprite sprite in spriteList)
            {
                sprite.Update();
            }
            //checks win
            if ((puzzleSlots[0, 0].points + puzzleSlots[1, 1].points + puzzleSlots[2, 2].points == diagLeftRight) || diagLeftRight == -1)//diagonal left to right
                if ((puzzleSlots[0, 2].points + puzzleSlots[1, 1].points + puzzleSlots[2, 0].points == diagRightLeft) || diagRightLeft == -1)//diagonal right to left
                        if ((puzzleSlots[0, 1].points + puzzleSlots[1, 1].points + puzzleSlots[2, 1].points == centRow) || centRow == -1)//row
                            if ((puzzleSlots[1, 0].points + puzzleSlots[1, 1].points + puzzleSlots[1, 2].points == centColumn) || centColumn == -1)//column
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
