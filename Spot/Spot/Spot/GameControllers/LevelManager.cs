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
    class LevelManager
    {
        ContentManager lmContent;
        SpriteBatch lmSpriteBatch;

        public enum LevelState
        {
            Gameplay,
            Restarting,
            Puzzle
        }
        public LevelState levelState = LevelState.Gameplay;

        public Player player = null;
        public LevelConstructor level;
        public PuzzlePanel panelOne;
        string levelOne = "Content/XML/Prototype1.xml";
        bool clearLists = false;
        List<Wall> listWalls = new List<Wall>();
        List<Sprite> spriteList = new List<Sprite>();
        List<Enemy> enemyList = new List<Enemy>();
        List<Sprite> spritesToAdd = new List<Sprite>();
        List<Sprite> spritesToRemove = new List<Sprite>();
        Vector2 camera = new Vector2(0, 0);
        float camMoveSpeed = 5f;
        bool paused = false;
        public static LevelManager instance;

        Sprite background = new Sprite("Backgrounds/newBack");
        Vector2 backgroundCamera = new Vector2(0, 0);

        public LevelManager(ContentManager content, SpriteBatch spriteBatch)
        {
            lmContent = content;
            lmSpriteBatch = spriteBatch;
            instance = this;
            Initialize();
            
        }

        public static LevelManager Instance()
        {
            return instance;
        }

        public void Initialize()
        {
            level = new LevelConstructor();
            panelOne = new PuzzlePanel();

            LoadContent();
            levelState = LevelState.Gameplay;
            clearLists = false;
        }

        public void LoadContent()
        {
            level.loadLevel(levelOne);
            panelOne.loadPanelXML();
            listWalls = level.getWallList();
            player.LoadContent(lmContent);
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
                panelOne.Update();
            }
            if (clearLists)//restarts level
            {
                listWalls.Clear();
                spriteList.Clear();
                enemyList.Clear();
                player = null;
                clearLists = false;
                levelState = LevelState.Gameplay;
                Initialize();
            }
        }

        public void UpdateCamera()
        {
            if (player != null)
            {
                if (player.BoundingBox.Y > camera.Y + 208)
                {
                    //camera.Y = player.BoundingBox.Y + 208;
                }
                if (player.BoundingBox.Y < camera.Y + 208)
                {
                    //camera.Y = player.BoundingBox.Y - 208;
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
                panelOne.drawPanel(spriteBatch, camera);
            }
        }

        public void restartLevel()
        {
            Debug.WriteLine("restart");
            levelState = LevelState.Restarting;
            clearLists = true;

            //MediaPlayer.Stop();
            //MediaPlayer.Play(Game1.Instance().song);
        }

        public void createPauseMenu()
        {

        }

        public void startPuzzle()
        {
            levelState = LevelState.Puzzle;
            panelOne.cursor.canKpress = false;
            //level.XmlLoad(puzzlePanelLevelOne, "puzzle");
        }
    }
}
