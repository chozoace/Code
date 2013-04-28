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
using System.Xml.XPath;
using System.Xml.Schema;


namespace Spot
{
    class LevelConstructor
    {
        string levelOne = "Content/XML/Level1.xml";
        string levelTwo = "Content/XML/Level2.xml";
        string levelThree = "Content/XML/Level3.xml";
        string levelFour = "Content/XML/Level4.xml";
        string weapons1 = "Content/XML/weapons1.xml";
        string weapons2 = "Content/XML/weapons2.xml";
        string weapons3 = "Content/XML/weapons3.xml";
        string weapons4 = "Content/XML/weapons4.xml";
        List<Wall> walls = new List<Wall>();
        static LevelConstructor instance;
        Vector2 camera = new Vector2(0, 0);

        XmlDocument xDoc;

        string currentLevel;
        int playerXpos;
        int playerYpos;
        int mapWidth;
        int mapHeight;
        int tileWidth;
        int tileHeight;
        int numberOfTiles;
        List<int> gidList = new List<int>();
        List<int> gidList2 = new List<int>();

        public LevelConstructor()
        {
            instance = this;
        }

        public static LevelConstructor Instance()
        {
            return instance;
        }

        public List<Wall> getWallList()
        {
            return walls;
        }

        public void removefromWallList(Wall wall)
        {
            walls.Remove(wall);
        }

        public void loadLevel(String level)
        {
            currentLevel = level;
            XmlLoad(currentLevel, "level", 0);
        }

        //begin XML here
        public void XmlLoad(String level, String levelType, int currentPanel)
        {
            //map, tileset and layer, data, tile 
            xDoc = new XmlDocument();
            xDoc.Load(level);
            XmlNode mapData = xDoc.FirstChild;
            numberOfTiles = mapData.FirstChild.NextSibling.FirstChild.ChildNodes.Count;
            mapWidth = int.Parse(mapData.Attributes.GetNamedItem("width").Value);
            mapHeight = int.Parse(mapData.Attributes.GetNamedItem("height").Value);
            tileWidth = int.Parse(mapData.Attributes.GetNamedItem("tilewidth").Value);
            tileHeight = int.Parse(mapData.Attributes.GetNamedItem("tileheight").Value);

            if (levelType == "level")
            {
                foreach (XmlNode xNode in mapData.FirstChild.NextSibling.FirstChild.ChildNodes)
                {
                    gidList.Add(int.Parse(xNode.Attributes.GetNamedItem("gid").Value));
                }

                if (level == levelOne)
                {
                    tileLoadLevel1();
                }
                else if (level == levelTwo)
                    tileLoadLevel2();
                else if (level == levelThree)
                    tileLoadLevel3();
                else if (level == levelFour)
                {
                    Debug.WriteLine("loadlvl4");
                    tileLoadLevel1();
                }
            }
            else if (levelType == "puzzle")
            {
                gidList.Clear();
                foreach (XmlNode xNode in mapData.FirstChild.NextSibling.FirstChild.ChildNodes)
                {
                    gidList.Add(int.Parse(xNode.Attributes.GetNamedItem("gid").Value));
                }
                tileLoadPuzzlePanel(currentPanel);
            }
        }

        public void tileLoadLevel1()
        {
            Wall theWall;
            Enemy enemy;
            Sprite theObject;
            int currentPanel = 1;
            int currentWeapon = 0;

            for (int spriteforX = 0; spriteforX < mapWidth; spriteforX++)
            {
                for (int spriteForY = 0; spriteForY < mapHeight; spriteForY++)
                {
                    int destY = spriteForY * tileHeight;
                    int destX = spriteforX * tileWidth;

                    switch (getTileAt(spriteforX, spriteForY))
                    {
                        case 1:
                            theWall = new Wall(new Vector2(destX, destY), tileWidth, tileHeight, 2, "Wall");
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                        case 2:
                            theWall = new Wall(new Vector2(destX, destY), tileWidth, tileHeight, 1, "Wall");
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                        case 3:
                            playerXpos = destX;
                            playerYpos = destY;
                            break;
                        case 4:
                            theObject = new PuzzlePanelObject(new Vector2(destX, destY), currentPanel);
                            LevelManager.Instance().addToSpriteList(theObject);
                            currentPanel++;
                            break;
                        case 5:
                            enemy = new MeleeEnemy(new Vector2(destX, destY));
                            LevelManager.Instance().addToEnemyList(enemy);
                            LevelManager.Instance().addToSpriteList(enemy);
                            break;
                        case 6:
                            theWall = new FallingBookCase(new Vector2(destX, destY));
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                        case 7:
                            theWall = new Wall(new Vector2(destX, destY), tileWidth, tileHeight, "LevelObjects/NumOne", "Door");
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                        case 8:
                            theWall = new CrackedFloor(new Vector2(destX, destY));
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                        case 9:
                            //health
                            theObject = new HealthPickUp(new Vector2(destX, destY));
                            LevelManager.Instance().addToSpriteList(theObject);
                            break;
                        case 10:
                            //weapon
                            Debug.WriteLine("weapon");
                            theObject = new Weapon(currentWeapon, weapons1, destX, destY);
                            currentWeapon++;
                            LevelManager.Instance().addToSpriteList(theObject);
                            break;
                        case 11:
                            //bookcase trigger
                            theWall = new BookCaseTrigger(new Vector2(destX, destY), tileWidth, tileHeight, 1);
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                        case 12:
                            //next Level
                            theWall = new Wall(new Vector2(destX, destY), tileWidth, tileHeight, "LevelObjects/TestPuzzleBlock", "NextLevel");
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                    }
                }
            }
            Debug.WriteLine("before player");
            LevelManager.Instance().player = new Player(new Vector2(playerXpos, playerYpos));
            LevelManager.Instance().addToSpriteList(LevelManager.Instance().player);
            Debug.WriteLine("after player");
            Debug.WriteLine(LevelManager.Instance().player.position);
        }

        public void tileLoadPuzzlePanel(int cPanel)
        {
            //Wall theWall;
            //Enemy enemy;
            int currentPanel = cPanel;
            int currentPuzzleBlock = 1;
            Sprite theObject;
            OpenPuzzleSlot slot;
            int slotRow = 0;
            int slotColumn = 0;

            for (int spriteforX = 0; spriteforX < mapWidth; spriteforX++)
            {
                for (int spriteForY = 0; spriteForY < mapHeight; spriteForY++)
                {
                    int destY = spriteForY * tileHeight;
                    int destX = spriteforX * tileWidth;

                    switch (getTileAt(spriteforX, spriteForY))
                    {
                        case 1:
                            theObject = new Wall(new Vector2(destX, destY), tileWidth, tileHeight, 5, "puzzle");
                            switch (currentPanel)
                            {
                                case 1:
                                    LevelManager.Instance().panelOne.addToSpriteList(theObject);
                                    break;
                                case 2:
                                    LevelManager.Instance().panelTwo.addToSpriteList(theObject);
                                    break;
                                case 3:
                                    LevelManager.Instance().panelThree.addToSpriteList(theObject);
                                    break;
                                case 4:
                                    LevelManager.Instance().panelFour.addToSpriteList(theObject);
                                    break;
                                case 5:
                                    LevelManager.Instance().panelFive.addToSpriteList(theObject);
                                    break;
                            }
                            break;
                        case 18:
                            slot = new OpenPuzzleSlot(new Vector2(destX, destY), 160, 128, false, slotRow, slotColumn, currentPanel);
                            slotRow++;
                            if (slotRow > 2)
                            {
                                slotRow = 0;
                                slotColumn++;
                            }
                            switch (currentPanel)
                            {
                                case 1:
                                    LevelManager.Instance().panelOne.addToSpriteList(slot);
                                    LevelManager.Instance().panelOne.addToHoleList(slot);
                                    break;
                                case 2:
                                    LevelManager.Instance().panelTwo.addToSpriteList(slot);
                                    LevelManager.Instance().panelTwo.addToHoleList(slot);
                                    break;
                                case 3:
                                    LevelManager.Instance().panelThree.addToSpriteList(slot);
                                    LevelManager.Instance().panelThree.addToHoleList(slot);
                                    break;
                                case 4:
                                    LevelManager.Instance().panelFour.addToSpriteList(slot);
                                    LevelManager.Instance().panelFour.addToHoleList(slot);
                                    break;
                                case 5:
                                    LevelManager.Instance().panelFive.addToSpriteList(slot);
                                    LevelManager.Instance().panelFive.addToHoleList(slot);
                                    break;
                            }
                            break;
                        case 2: case 3: case 4: case 5: case 6: case 7: case 8: case 9: case 10: case 11:
                            int blockValue = getTileAt(spriteforX, spriteForY) - 1;
                            theObject = new Wall(new Vector2(destX, destY), 32, 32, getTileAt(spriteforX, spriteForY), "puzzleHole");

                            switch (currentPanel)
                            {
                                case 1:
                                    LevelManager.Instance().panelOne.addToSpriteList(theObject);
                                    LevelManager.Instance().panelOne.assignWinConditions(currentPuzzleBlock, blockValue);
                                    break;
                                case 2:
                                    LevelManager.Instance().panelTwo.addToSpriteList(theObject);
                                    LevelManager.Instance().panelTwo.assignWinConditions(currentPuzzleBlock, blockValue);
                                    break;
                                case 3:
                                    LevelManager.Instance().panelThree.addToSpriteList(theObject);
                                    LevelManager.Instance().panelThree.assignWinConditions(currentPuzzleBlock, blockValue);
                                    break;
                                case 4:
                                    LevelManager.Instance().panelFour.addToSpriteList(theObject);
                                    LevelManager.Instance().panelFour.assignWinConditions(currentPuzzleBlock, blockValue);
                                    break;
                                case 5:
                                    LevelManager.Instance().panelFive.addToSpriteList(theObject);
                                    LevelManager.Instance().panelFive.assignWinConditions(currentPuzzleBlock, blockValue);
                                    break;
                            }
                            currentPuzzleBlock++;
                            break; 
                    }
                }
            }
        }

        public void tileLoadLevel2()
        {
            Wall theWall;
            Enemy enemy;
            Sprite theObject;
            int currentPanel = 1;
            int currentWeapon = 0;

            for (int spriteforX = 0; spriteforX < mapWidth; spriteforX++)
            {
                for (int spriteForY = 0; spriteForY < mapHeight; spriteForY++)
                {
                    int destY = spriteForY * tileHeight;
                    int destX = spriteforX * tileWidth;

                    switch (getTileAt(spriteforX, spriteForY))
                    {
                        case 1://grass
                            theWall = new Wall(new Vector2(destX, destY), tileWidth, tileHeight, "LevelObjects/Level2Assets/greenTile", "Wall");
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                        case 2://dirt
                            theWall = new Wall(new Vector2(destX, destY), tileWidth, tileHeight, "LevelObjects/Level2Assets/dirtTile", "Wall");
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                        case 3:
                            theWall = new Wall(new Vector2(destX, destY), tileWidth, tileHeight, "LevelObjects/NumOne", "Door");
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                        case 4:
                            theWall = new Wall(new Vector2(destX, destY), tileWidth, tileHeight, "LevelObjects/NumOne", "NextLevel");
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                        case 7://tree
                            theObject = new Sprite("LevelObjects/Level2Assets/tree", destX, destY);
                            LevelManager.Instance().addToSpriteList(theObject);
                            break;
                        case 16:
                            playerXpos = destX;
                            playerYpos = destY;
                            break;
                        case 17://puzzlepanel
                            theObject = new PuzzlePanelObject(new Vector2(destX, destY), currentPanel);
                            LevelManager.Instance().addToSpriteList(theObject);
                            currentPanel++;
                            break;
                        case 31://17: dirt2
                            theWall = new Wall(new Vector2(destX, destY), tileWidth, tileHeight, "LevelObjects/Level2Assets/dirtTile2", "Wall");
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                        case 32://31: second grass
                            theWall = new Wall(new Vector2(destX, destY), tileWidth, tileHeight, "LevelObjects/Level2Assets/greenTile2", "Wall");
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                        case 46://46: weapon
                            theObject = new Weapon(currentWeapon, weapons1, destX, destY);
                            currentWeapon++;
                            LevelManager.Instance().addToSpriteList(theObject);
                            break;
                        case 47://47: enemy
                            enemy = new MeleeEnemy(new Vector2(destX, destY));
                            LevelManager.Instance().addToEnemyList(enemy);
                            LevelManager.Instance().addToSpriteList(enemy);
                            break;
                        case 62://62: health
                            theObject = new HealthPickUp(new Vector2(destX, destY));
                            LevelManager.Instance().addToSpriteList(theObject);
                            break;
                        case 63: // wood
                            theWall = new Wall(new Vector2(destX, destY), tileWidth, tileHeight, "LevelObjects/Level2Assets/woodTile", "Wall");
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                        case 106://106: Bench
                            theObject = new Sprite("LevelObjects/Level2Assets/bench", destX, destY - 32);
                            LevelManager.Instance().addToSpriteList(theObject);
                            break;
                        case 110://110: swingset
                            theObject = new Sprite("LevelObjects/Level2Assets/swing", destX, destY - 32);
                            LevelManager.Instance().addToSpriteList(theObject);
                            break;
                        case 116://116: tree 2
                            theObject = new Sprite("LevelObjects/Level2Assets/tree2", destX, destY - 32);
                            LevelManager.Instance().addToSpriteList(theObject);
                            break;
                        case 121: //146: trash
                            theObject = new Sprite("LevelObjects/Level2Assets/trashcan", destX, destY);
                            LevelManager.Instance().addToSpriteList(theObject);
                            break;
                        case 123://148: bush
                            theObject = new Sprite("LevelObjects/Level2Assets/bush", destX, destY);
                            LevelManager.Instance().addToSpriteList(theObject);
                            break;
                            
                    }
                }
            }
            LevelManager.Instance().player = new Player(new Vector2(playerXpos, playerYpos));
            LevelManager.Instance().addToSpriteList(LevelManager.Instance().player);
        }

        public void tileLoadLevel3()
        {
            Wall theWall;
            Enemy enemy;
            Sprite theObject;
            int currentPanel = 1;
            int currentWeapon = 0;

            for (int spriteforX = 0; spriteforX < mapWidth; spriteforX++)
            {
                for (int spriteForY = 0; spriteForY < mapHeight; spriteForY++)
                {
                    int destY = spriteForY * tileHeight;
                    int destX = spriteforX * tileWidth;

                    switch (getTileAt(spriteforX, spriteForY))
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                            theWall = new Wall(new Vector2(destX, destY), tileWidth, tileHeight, "LevelObjects/stoneTile", "Wall");
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                        case 10:
                            theObject = new Weapon(currentWeapon, weapons3, destX, destY);
                            currentWeapon++;
                            LevelManager.Instance().addToSpriteList(theObject);
                            break;
                        case 11:
                            playerXpos = destX;
                            playerYpos = destY;
                            break;
                        case 13:
                            theWall = new CrackedFloor(new Vector2(destX, destY));
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                        case 15://only orange
                        //case 16:
                            //enemy = new MeleeEnemy(new Vector2(destX, destY));
                            //LevelManager.Instance().addToEnemyList(enemy);
                            //LevelManager.Instance().addToSpriteList(enemy);
                            break;
                        case 19:
                            theObject = new PuzzlePanelObject(new Vector2(destX, destY), currentPanel);
                            LevelManager.Instance().addToSpriteList(theObject);
                            currentPanel++;
                            break;
                        case 20:
                            theWall = new Wall(new Vector2(destX, destY), tileWidth, tileHeight, "LevelObjects/NumOne", "Door");
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break; 
                        case 21:
                            theWall = new Wall(new Vector2(destX, destY), tileWidth, tileHeight, "LevelObjects/bricktile", "NextLevel");
                            walls.Add(theWall);
                            LevelManager.Instance().addToSpriteList(theWall);
                            break;
                    }
                }
            }
            LevelManager.Instance().player = new Player(new Vector2(playerXpos, playerYpos));
            LevelManager.Instance().addToSpriteList(LevelManager.Instance().player);
        }

        public int getTileAt(int x, int y)
        {
            //Debug.WriteLine("mapWidth: " + mapWidth + " mapHeight: " + mapHeight + " x: " + x + " y: " + y);
            return gidList[(x + (y * mapWidth))];
        }

    }
}
