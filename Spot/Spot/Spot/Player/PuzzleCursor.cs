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
    class PuzzleCursor : Sprite
    {
        KeyboardState myKeyState, previousKeyState;
        bool upMove, rightMove, leftMove, downMove;
        String currentTexture;
        String block1 = "LevelObjects/Block1";
        String block2 = "LevelObjects/Block2";
        String block3 = "LevelObjects/Block3";
        int currentPiece = 1;
        bool canTexChange = true;
        public bool canKpress = false;
        bool canJpress = true;
        Vector2 speed;
        List<OpenPuzzleSlot> holeList;
        OpenPuzzleSlot currentSlot;
        public bool controlsLocked = false;

        public PuzzleCursor(Vector2 newPos)
        {
            position = newPos;
            texture = Game1.Instance().Content.Load<Texture2D>(block1);
            currentTexture = block1;
            width = 32;
            height = 32;
            speed.X = 6;
            speed.Y = 6;
        }

        public override void Update()
        {
            myKeyState = Keyboard.GetState();

            CheckKeysDown(myKeyState);
            CheckKeysUp(myKeyState);
            UpdateMovement();
            //Debug.WriteLine(currentTexture + " " + canTexChange);

            previousKeyState = myKeyState;
        }

        public void CheckKeysDown(KeyboardState keyState)
        {
            if (!controlsLocked)
            {
                if (keyState.IsKeyDown(Keys.A) == true && previousKeyState.IsKeyDown(Keys.A) == true)
                {
                    leftMove = true;
                }
                if (keyState.IsKeyDown(Keys.D) == true && previousKeyState.IsKeyDown(Keys.D) == true)
                {
                    rightMove = true;
                }
                if (keyState.IsKeyDown(Keys.W) == true && previousKeyState.IsKeyDown(Keys.W) == true)
                {
                    upMove = true;
                }
                if (keyState.IsKeyDown(Keys.S) == true && previousKeyState.IsKeyDown(Keys.S) == true)
                {
                    downMove = true;
                }
                if (keyState.IsKeyDown(Keys.K) == true && previousKeyState.IsKeyDown(Keys.K) == true)
                {
                    if (canKpress)
                    {
                        LevelManager.Instance().player.canKpress = false;
                        LevelManager.Instance().levelState = LevelManager.LevelState.Gameplay;
                    }
                }
                if (keyState.IsKeyDown(Keys.U) == true && previousKeyState.IsKeyDown(Keys.U) == true && canTexChange)
                {
                    if (currentTexture == block1)
                    {
                        texture = Game1.Instance().Content.Load<Texture2D>(block2);
                        currentTexture = block2;
                        currentPiece = 2;
                    }
                    else if (currentTexture == block2)
                    {
                        texture = Game1.Instance().Content.Load<Texture2D>(block3);
                        currentPiece = 3;
                        currentTexture = block3;
                    }
                    else if (currentTexture == block3)
                    {
                        texture = Game1.Instance().Content.Load<Texture2D>(block1);
                        currentPiece = 1;
                        currentTexture = block1;
                    }
                    canTexChange = false;
                }
                if (keyState.IsKeyDown(Keys.J) == true && previousKeyState.IsKeyDown(Keys.J) == true && canTexChange)
                {
                    if (canJpress)
                    {
                        if (CheckCollision(BoundingBox))
                        {
                            currentSlot.interact(currentPiece);
                            canJpress = false;
                        }
                    }
                }
            }
        }

        public override bool CheckCollision(Rectangle collisionBox)
        {
            holeList = PuzzlePanel.Instance().holeList;
            foreach (OpenPuzzleSlot hole in holeList)
            {
                if(collisionBox.Intersects(hole.BoundingBox))
                {
                    currentSlot = hole;
                    return true;
                }
            }
            return false;
        }

        public void CheckKeysUp(KeyboardState keyState)
        {
            if (keyState.IsKeyUp(Keys.A) == true && previousKeyState.IsKeyUp(Keys.A) == true)
            {
                leftMove = false;
            }
            if (keyState.IsKeyUp(Keys.D) == true && previousKeyState.IsKeyUp(Keys.D) == true)
            {
                rightMove = false;
            }
            if (keyState.IsKeyUp(Keys.W) == true && previousKeyState.IsKeyUp(Keys.W) == true)
            {
                upMove = false;
            }
            if (keyState.IsKeyUp(Keys.S) == true && previousKeyState.IsKeyUp(Keys.S) == true)
            {
                downMove = false;
            }

            if (keyState.IsKeyUp(Keys.K) == true && previousKeyState.IsKeyUp(Keys.K) == true)
            {
                canKpress = true;
            }

            if (keyState.IsKeyUp(Keys.U) == true && previousKeyState.IsKeyUp(Keys.U) == true)
            {
                canTexChange = true;
            }
            if (keyState.IsKeyUp(Keys.J) == true && previousKeyState.IsKeyUp(Keys.J) == true)
            {
                canJpress = true;
            }
        }

        public void UpdateMovement()
        {
            if (rightMove)
                position.X += speed.X;
            if (leftMove)
                position.X -= speed.X;
            if (upMove)
                position.Y -= speed.Y;
            if (downMove)
                position.Y += speed.Y;
        }
    }
}
