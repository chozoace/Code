﻿using System;
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
    class LevelManager
    {
        ContentManager lmContent;
        SpriteBatch lmSpriteBatch;

        public enum LevelState
        {
            Gameplay,
            Restarting,
            Puzzle,
            NextLevel
        }
        public LevelState levelState = LevelState.Gameplay;

        public Player player = null;
        public LevelConstructor level;

        public PuzzlePanel panelOne;
        public PuzzlePanel panelTwo;
        public PuzzlePanel panelThree;
        public PuzzlePanel panelFour;
        public PuzzlePanel panelFive;
        int currentPanel;

        string levelOne = "Content/XML/Level1.xml";
        string levelTwo = "Content/XML/Level2.xml";
        string levelThree = "Content/XML/Level3.xml";
        string levelFour = "Content/XML/Level4.xml";
        public string currentLevel;

        bool clearLists = false;
        List<Wall> listWalls = new List<Wall>();
        public List<Sprite> spriteList = new List<Sprite>();
        List<Enemy> enemyList = new List<Enemy>();
        List<Sprite> spritesToAdd = new List<Sprite>();
        List<Sprite> spritesToRemove = new List<Sprite>();
        Vector2 camera = new Vector2(0, 0);
        float camMoveSpeed = 5f;
        bool paused = false;
        public static LevelManager instance;

        Sprite background = new Sprite();
        string level1tex = "Backgrounds/level1";
        string level2tex = "Backgrounds/level2";
        string level3tex = "Backgrounds/level3";
        string level4tex = "Backgrounds/level4";
        Vector2 backgroundCamera = new Vector2(0, 0);

        Texture2D blackScreen;
        Color fadeColor = Color.Black;
        Rectangle fadeRect = new Rectangle(0, 0, Game1.Instance().GraphicsDevice.Viewport.Width, Game1.Instance().GraphicsDevice.Viewport.Height);
        int fadeCounter = 5;

        public LevelManager(ContentManager content, SpriteBatch spriteBatch)
        {
            lmContent = content;
            lmSpriteBatch = spriteBatch;
            instance = this;
            levelState = LevelState.Gameplay;
            Initialize(levelOne);
            
        }

        public static LevelManager Instance()
        {
            return instance;
        }

        public void Initialize(string theLevel)
        {
            currentLevel = theLevel;
            level = new LevelConstructor();

            if (currentLevel == levelOne)
            {
                background.texture = lmContent.Load<Texture2D>(level1tex);

                panelOne = new PuzzlePanel(1, "Content/XML/Level1Puzzles/Panel1-1.xml");
                panelTwo = new PuzzlePanel(2, "Content/XML/Level1Puzzles/Panel1-2.xml");
                panelThree = new PuzzlePanel(3, "Content/XML/Level1Puzzles/Panel1-3.xml");
                panelFour = new PuzzlePanel(4, "Content/XML/Level1Puzzles/Panel1-4.xml");
                panelFive = new PuzzlePanel(5, "Content/XML/Level1Puzzles/Panel1-5.xml");
            }
            else if (currentLevel == levelTwo)
            {
                panelOne.panelString = "Content/XML/Level2Puzzles/Panel2-1.xml";
                panelTwo.panelString = "Content/XML/Level2Puzzles/Panel2-2.xml";
                panelThree.panelString = "Content/XML/Level2Puzzles/Panel2-3.xml";
                panelFour.panelString = "Content/XML/Level2Puzzles/Panel2-4.xml";
                panelFive.panelString = "Content/XML/Level2Puzzles/Panel2-5.xml";

                background.texture = lmContent.Load<Texture2D>(level2tex);
            }
            else if (currentLevel == levelThree)
            {
                panelOne.panelString = "Content/XML/Level3Puzzles/Panel3-1.xml";
                panelTwo.panelString = "Content/XML/Level3Puzzles/Panel3-2.xml";
                panelThree.panelString = "Content/XML/Level3Puzzles/Panel3-3.xml";
                panelFour.panelString = "Content/XML/Level3Puzzles/Panel3-4.xml";
                panelFive.panelString = "Content/XML/Level3Puzzles/Panel3-5.xml";

                background.texture = lmContent.Load<Texture2D>(level3tex);
            }
            else if (currentLevel == levelFour)
            {
                panelOne.panelString = "Content/XML/Level4Puzzles/Panel4-1.xml";
                panelTwo.panelString = "Content/XML/Level4Puzzles/Panel4-2.xml";
                panelThree.panelString = "Content/XML/Level4Puzzles/Panel4-3.xml";
                panelFour.panelString = "Content/XML/Level4Puzzles/Panel4-4.xml";
                panelFive.panelString = "Content/XML/Level4Puzzles/Panel4-5.xml";

                background.texture = lmContent.Load<Texture2D>(level4tex);
            }

            LoadContent();
            clearLists = false;
        }

        public void LoadContent()
        {
            level.loadLevel(currentLevel);

            if (currentLevel == levelOne)
            {
                panelOne.loadPanelXML(1);
                panelTwo.loadPanelXML(2);
                panelThree.loadPanelXML(3);
                panelFour.loadPanelXML(4);
                panelFive.loadPanelXML(5);
            }

            listWalls = level.getWallList();
            player.LoadContent(lmContent);
            blackScreen = Game1.Instance().Content.Load<Texture2D>("Menus/BlackScreen");
        }

        public void addToSpriteList(Sprite theSprite)
        {
            spritesToAdd.Add(theSprite);
        }

        public void removefromSpriteList(Sprite theSprite)
        {
            spritesToRemove.Add(theSprite);
        }

        public void addToEnemyList(Enemy enemy)
        {
            enemyList.Add(enemy);
        }

        public void removefromEnemyList(Enemy enemy)
        {
            enemyList.Remove(enemy);
        }

        public List<Enemy> getEnemyList()
        {
            return enemyList;
        }

        public void Update()
        {
            //Debug.WriteLine("num in list: " + spriteList.Count);
            if (levelState == LevelState.Gameplay)
            {
                foreach (Sprite sprite in spriteList.ToList<Sprite>())
                {
                    sprite.Update();
                }
                UpdateCamera();
            }
            else if (levelState == LevelState.Puzzle)
            {
                switch (currentPanel)
                {
                    case 1:
                        panelOne.Update();
                        break;
                    case 2:
                        panelTwo.Update();
                        break;
                    case 3:
                        panelThree.Update();
                        break;
                    case 4:
                        panelFour.Update();
                        break;
                    case 5:
                        panelFive.Update();
                        break;
                }
            }
            else if (levelState == LevelState.NextLevel)
            {
                fade();
            }
            else if (levelState == LevelState.Restarting)
            {
                fade();
            }
            if (clearLists)//restarts level
            {
                listWalls.Clear();
                spriteList.Clear();
                enemyList.Clear();
                player = null;
                clearLists = false;
                
                if (levelState == LevelState.Restarting)
                    Initialize(levelOne);
                else if (levelState == LevelState.NextLevel)
                {
                    if (currentLevel == levelOne)
                        Initialize(levelTwo);
                    else if (currentLevel == levelTwo)
                        Initialize(levelThree);
                    else if (currentLevel == levelThree)
                    {
                        Game1.Instance().gameOver();
                    }
                }
                if (Game1.Instance().gameState != Game1.GameState.GameOver)
                {
                    camera.X = player.BoundingBox.X - 288;
                    camera.Y = player.BoundingBox.Y - 208;
                }
            }
        }

        public void UpdateCamera()
        {
            if (player != null)
            {
                if (player.BoundingBox.Y > camera.Y + 208)
                {
                    camera.Y = player.BoundingBox.Y + 208;
                }
                if (player.BoundingBox.Y < camera.Y + 208)
                {
                    camera.Y = player.BoundingBox.Y - 208;
                }
                if (player.BoundingBox.X > camera.X + 288)
                {
                    camera.X = player.BoundingBox.X - 288;
                    backgroundCamera.X += 2;
                    if (backgroundCamera.X > 640)
                        backgroundCamera.X = 0;
                }
                if (player.BoundingBox.X < camera.X + 288)
                {
                    camera.X = player.BoundingBox.X - 288;
                    backgroundCamera.X -= 2;
                    if (backgroundCamera.X < -640)
                        backgroundCamera.X = 0;
                }
            }
        }

        public void drawGame(SpriteBatch spriteBatch)
        {
            if (levelState == LevelState.Gameplay)
            {
                background.Draw(spriteBatch, backgroundCamera);
                background.Draw(spriteBatch, backgroundCamera + new Vector2(640, 0));
                background.Draw(spriteBatch, backgroundCamera - new Vector2(640, 0));
                foreach (Sprite sprite in spriteList)
                {
                    sprite.Draw(spriteBatch, camera);
                }

                //player.drawCollisionBox(spriteBatch, lmContent, camera);

                foreach (Sprite addition in spritesToAdd)
                    spriteList.Add(addition);
                spritesToAdd.Clear();

                foreach (Sprite removed in spritesToRemove)
                    spriteList.Remove(removed);
                spritesToRemove.Clear();                
            }
            else if (levelState == LevelState.Puzzle)
            {
                switch(currentPanel)
                {
                    case 1:
                        panelOne.drawPanel(spriteBatch, camera);
                        break;
                    case 2:
                        panelTwo.drawPanel(spriteBatch, camera);
                        break;
                    case 3:
                        panelThree.drawPanel(spriteBatch, camera);
                        break;
                    case 4:
                        panelFour.drawPanel(spriteBatch, camera);
                        break;
                    case 5:
                        panelFive.drawPanel(spriteBatch, camera);
                        break;
                }
            }
            else if (levelState == LevelState.NextLevel || levelState == LevelState.Restarting)
            {
                background.Draw(spriteBatch, backgroundCamera);
                background.Draw(spriteBatch, backgroundCamera + new Vector2(640, 0));
                background.Draw(spriteBatch, backgroundCamera - new Vector2(640, 0));
                foreach (Sprite sprite in spriteList)
                {
                    sprite.Draw(spriteBatch, camera);
                }

                //player.drawCollisionBox(spriteBatch, lmContent, camera);

                foreach (Sprite addition in spritesToAdd)
                    spriteList.Add(addition);
                spritesToAdd.Clear();

                foreach (Sprite removed in spritesToRemove)
                    spriteList.Remove(removed);
                spritesToRemove.Clear();

                spriteBatch.Draw(blackScreen, fadeRect, fadeColor);
            }
        }

        public void restartLevel()
        {
            Debug.WriteLine("restart");
            levelState = LevelState.Restarting;
            fadeColor.A = 0;
            //clearLists = true;

            //MediaPlayer.Stop();
            //MediaPlayer.Play(Game1.Instance().song);
        }

        public void nextLevel()
        {
            levelState = LevelState.NextLevel;

            fadeColor.A = 0;

            //clearLists = true;
        }

        public void fade()
        {
            fadeColor.A = (byte)MathHelper.Clamp(fadeColor.A + fadeCounter, 0, 255);

            if (fadeColor.A == 0)
            {
                levelState = LevelState.Gameplay;
                fadeCounter *= -1;
            }
            else if (fadeColor.A == 255)
            {
                clearLists = true;
                fadeCounter *= -1;
                camera.X = player.BoundingBox.X - 288;
                
            }
        }

        public void createPauseMenu()
        {

        }

        public void startPuzzle(int puzzleNum)
        {
            levelState = LevelState.Puzzle;

            switch (puzzleNum)
            {
                case 1:
                    panelOne.cursor.canKpress = false;
                    currentPanel = puzzleNum;
                    break;
                case 2:
                    panelTwo.cursor.canKpress = false;
                    currentPanel = puzzleNum;
                    break;
                case 3: 
                    panelThree.cursor.canKpress = false;
                    currentPanel = puzzleNum;
                    break;
                case 4:
                    panelFour.cursor.canKpress = false;
                    currentPanel = puzzleNum;
                    break;
                case 5:
                    panelFive.cursor.canKpress = false;
                    currentPanel = puzzleNum;
                    break;
            }
            //level.XmlLoad(puzzlePanelLevelOne, "puzzle");
        }
    }
}
