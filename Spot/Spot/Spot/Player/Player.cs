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
using System.Timers;

namespace Spot
{
    class Player : MoveableObject
    {
        ContentManager myContent;
        SpriteBatch mySpriteBatch;

        //variables and properties
        public enum PlayerState
        {
            Jumping,
            Boosting,
            Running,
            Idle,
            Attacking,
            Dead,
            Hitstun
        }
        public PlayerState myState, previousState;
        KeyboardState myKeyState, previousKeyState;
        GamePadState previousButtonState;
        XmlDocument myStats;

        //////////
        //X Move variables
        //////////
        bool leftMove = false;
        bool rightMove = false;
        float runningDecelRate;//5f
        //////////
        //Jump variables
        //////////
        bool canUpPress = true;
        int jumpSpeedLimit;//10
        int jumpSpeed;//-13
        int fallSpeed;//10
        int doubleJumpSpeed;//-9
        int jumpCount;//0
        //////////
        //Attack variables
        //////////
        public bool canAttack = true;
        bool canJpress = true;
        public bool canKpress = true;
        int damage = 1000;
        int attackSpeed;
        int attackReach = 30;
        //////////
        //Puzzle Check variables
        //////////
        int totalPieces;
        int totalCircles;
        int totalTriangle;
        int totalSquare;

        public bool canStartPuzzle = false;

        String lightRight;
        String lightLeft;
        LightAttack lAttack = null;

        GamePadState padState;
        HUD hud;

        public Player(Vector2 newPos)
        {
            myStats = new XmlDocument();
            myStats.Load("Content/XML/PlayerSample.xml");
            XmlNode playerNode = myStats.FirstChild;

            health = int.Parse(playerNode.Attributes.GetNamedItem("health").Value);
            maxHealth = health;
            height = int.Parse(playerNode.Attributes.GetNamedItem("height").Value);
            width = int.Parse(playerNode.Attributes.GetNamedItem("width").Value);
            maxSpeed = int.Parse(playerNode.Attributes.GetNamedItem("maxSpeed").Value);
            accelRate = float.Parse(playerNode.Attributes.GetNamedItem("accelRate").Value);
            decelRate = float.Parse(playerNode.Attributes.GetNamedItem("decelRate").Value);
            jumpSpeedLimit = int.Parse(playerNode.Attributes.GetNamedItem("jumpSpeedLimit").Value);
            jumpSpeed = int.Parse(playerNode.Attributes.GetNamedItem("jumpSpeed").Value);
            fallSpeed = int.Parse(playerNode.Attributes.GetNamedItem("fallSpeed").Value);
            gravity = float.Parse(playerNode.Attributes.GetNamedItem("gravity").Value);

            position = newPos;
            speed = new Vector2(0, 0);
            currentAccel = 0;

            hud = new HUD(position.X);
            hud.Font = Game1.Instance().Content.Load<SpriteFont>("Arial");
            hud.Health = health;

            idleAnim = "Player/HeroIdleRight";
            idleLeftAnim = "Player/HeroIdleLeft";
            runAnim = "Player/HeroWalkRight";
            runLeftAnim = "Player/HeroWalkLeft";
            lightRight = "Player/HeroAttackRight";
            lightLeft = "Player/HeroAttackLeft";
            hurtLeft = "Player/HeroLeftHurt";
            hurtRight = "Player/HeroRightHurt";
            jumpLeft = "Player/HeroJumpLeft";
            jumpRight = "Player/HeroJumpRight";
        }

        public void LoadContent(ContentManager content)
        {
            currentAnimation = idleAnim;
            texture = content.Load<Texture2D>(currentAnimation);
            animationRect = new Rectangle(0, 0, width, height);
            animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
            animTimer.Enabled = true;

            jumpCount = 0;
            myState = PlayerState.Idle;
            myContent = content;
        }

        public override void Update()
        {
            myKeyState = Keyboard.GetState();
            padState = GamePad.GetState(PlayerIndex.One);
            if (myState != PlayerState.Dead)
            {
                if (Game1.Instance().usingController)
                {
                    checkKeysDown(padState);
                    checkKeysUp(padState);
                }
                else
                {
                    checkKeysDown(myKeyState);
                    checkKeysUp(myKeyState);
                }
                UpdateMovement(myKeyState);
                UpdateTexture();
            }

            if (health < 1)
            {
                myState = PlayerState.Dead;
                controlsLocked = true;
                death();
            }
            if (health > maxHealth)
            {
                health = maxHealth;
            }
            hud.Health = health;
        }

        public void death()
        {
            LevelManager.Instance().restartLevel();
        }

        public void UpdateTexture()
        {
            if (!hitstun)
            {
                if (myState == PlayerState.Running)
                {
                    if (facing == 0 && currentAnimation != runAnim)
                    {
                        animationRect = new Rectangle(0, 0, width, height);
                        texture = myContent.Load<Texture2D>(runAnim);
                        currentAnimation = runAnim;

                    }
                    else if (facing == 1 && currentAnimation != runLeftAnim)
                    {
                        animationRect = new Rectangle(0, 0, width, height);
                        texture = myContent.Load<Texture2D>(runLeftAnim);
                        currentAnimation = runLeftAnim;
                    }
                }
                else if (myState == PlayerState.Idle)
                {
                    if (facing == 0 && currentAnimation != idleAnim)
                    {
                        animationRect = new Rectangle(0, 0, width, height);
                        texture = myContent.Load<Texture2D>(idleAnim);
                        currentAnimation = idleAnim;
                    }
                    else if (facing == 1 && currentAnimation != idleLeftAnim)
                    {
                        animationRect = new Rectangle(0, 0, width, height);
                        texture = myContent.Load<Texture2D>(idleLeftAnim);
                        currentAnimation = idleLeftAnim;
                    }
                }
                else if (myState == PlayerState.Jumping)
                {
                    if (facing == 0 && currentAnimation != jumpRight)
                    {
                        animationRect = new Rectangle(0, 0, width, height);
                        texture = myContent.Load<Texture2D>(jumpRight);
                        currentAnimation = jumpRight;
                    }
                    else if (facing == 1 && currentAnimation != jumpLeft)
                    {
                        animationRect = new Rectangle(0, 0, width, height);
                        texture = myContent.Load<Texture2D>(jumpLeft);
                        currentAnimation = jumpLeft;
                    }
                }
                else if (myState == PlayerState.Attacking)//light attack
                {
                    //attack animations here
                    if (facing == 0 && currentAnimation != lightRight)
                    {
                        animationRect = new Rectangle(0, 0, width, height);
                        texture = myContent.Load<Texture2D>(lightRight);
                        currentAnimation = lightRight;
                    }
                    else if (facing == 1 && currentAnimation != lightLeft)
                    {
                        animationRect = new Rectangle(0, 0, width, height);
                        texture = myContent.Load<Texture2D>(lightLeft);
                        currentAnimation = lightLeft;
                    }
                }
            }
            else if (hitstun)
            {
                if (facing == 1 && currentAnimation != hurtLeft)
                {
                    animationRect = new Rectangle(0, 0, width, height);
                    texture = myContent.Load<Texture2D>(hurtLeft);
                    currentAnimation = hurtLeft;
                }
                else if (facing == 0 && currentAnimation != hurtRight)
                {
                    animationRect = new Rectangle(0, 0, width, height);
                    texture = myContent.Load<Texture2D>(hurtRight);
                    currentAnimation = hurtRight;
                }
            }
        }

        public void weaponPickUp(Weapon theWeapon)
        {
            damage = theWeapon.damage;
            attackSpeed = theWeapon.speed;
            attackReach = theWeapon.range;

            switch (theWeapon.weaponName)
            {
                case "ChinSaw":
                    idleAnim = "Player/Chin/HeroIdleRight";
                    idleLeftAnim = "Player/Chin/HeroIdleLeft";
                    runAnim = "Player/Chin/HeroWalkRight";
                    runLeftAnim = "Player/Chin/HeroWalkLeft";
                    lightRight = "Player/Chin/HeroAttackRight";
                    lightLeft = "Player/Chin/HeroAttackLeft";
                    hurtLeft = "Player/Chin/HeroLeftHurt";
                    hurtRight = "Player/Chin/HeroRightHurt";
                    jumpLeft = "Player/Chin/HeroJumpLeft";
                    jumpRight = "Player/Chin/HeroJumpRight";
                    break;
                case "CalligraphyBrush":
                    idleAnim = "Player/CalligraphyBrush/HeroIdleRight";
                    idleLeftAnim = "Player/CalligraphyBrush/HeroIdleLeft";
                    runAnim = "Player/CalligraphyBrush/HeroWalkRight";
                    runLeftAnim = "Player/CalligraphyBrush/HeroWalkLeft";
                    lightRight = "Player/CalligraphyBrush/HeroAttackRight";
                    lightLeft = "Player/CalligraphyBrush/HeroAttackLeft";
                    hurtLeft = "Player/CalligraphyBrush/HeroLeftHurt";
                    hurtRight = "Player/CalligraphyBrush/HeroRightHurt";
                    jumpLeft = "Player/CalligraphyBrush/HeroJumpLeft";
                    jumpRight = "Player/CalligraphyBrush/HeroJumpRight";
                    break;
                case "FantasyBlade":
                    idleAnim = "Player/FantasyBlade/HeroIdleRight";
                    idleLeftAnim = "Player/FantasyBlade/HeroIdleLeft";
                    runAnim = "Player/FantasyBlade/HeroWalkRight";
                    runLeftAnim = "Player/FantasyBlade/HeroWalkLeft";
                    lightRight = "Player/FantasyBlade/HeroAttackRight";
                    lightLeft = "Player/FantasyBlade/HeroAttackLeft";
                    hurtLeft = "Player/FantasyBlade/HeroLeftHurt";
                    hurtRight = "Player/FantasyBlade/HeroRightHurt";
                    jumpLeft = "Player/FantasyBlade/HeroJumpLeft";
                    jumpRight = "Player/FantasyBlade/HeroJumpRight";
                    break;
                case "FluteSword":
                    idleAnim = "Player/FluteSword/HeroIdleRight";
                    idleLeftAnim = "Player/FluteSword/HeroIdleLeft";
                    runAnim = "Player/FluteSword/HeroWalkRight";
                    runLeftAnim = "Player/FluteSword/HeroWalkLeft";
                    lightRight = "Player/FluteSword/HeroAttackRight";
                    lightLeft = "Player/FluteSword/HeroAttackLeft";
                    hurtLeft = "Player/FluteSword/HeroLeftHurt";
                    hurtRight = "Player/FluteSword/HeroRightHurt";
                    jumpLeft = "Player/FluteSword/HeroJumpLeft";
                    jumpRight = "Player/FluteSword/HeroJumpRight";
                    break;
                case "PaintWand":
                    idleAnim = "Player/PaintWand/HeroIdleRight";
                    idleLeftAnim = "Player/PaintWand/HeroIdleLeft";
                    runAnim = "Player/PaintWand/HeroWalkRight";
                    runLeftAnim = "Player/PaintWand/HeroWalkLeft";
                    lightRight = "Player/PaintWand/HeroAttackRight";
                    lightLeft = "Player/PaintWand/HeroAttackLeft";
                    hurtLeft = "Player/PaintWand/HeroLeftHurt";
                    hurtRight = "Player/PaintWand/HeroRightHurt";
                    jumpLeft = "Player/PaintWand/HeroJumpLeft";
                    jumpRight = "Player/PaintWand/HeroJumpRight";
                    break;
                case "SpikeSabre":
                    idleAnim = "Player/SpikeSabre/HeroIdleRight";
                    idleLeftAnim = "Player/SpikeSabre/HeroIdleLeft";
                    runAnim = "Player/SpikeSabre/HeroWalkRight";
                    runLeftAnim = "Player/SpikeSabre/HeroWalkLeft";
                    lightRight = "Player/SpikeSabre/HeroAttackRight";
                    lightLeft = "Player/SpikeSabre/HeroAttackLeft";
                    hurtLeft = "Player/SpikeSabre/HeroLeftHurt";
                    hurtRight = "Player/SpikeSabre/HeroRightHurt";
                    jumpLeft = "Player/SpikeSabre/HeroJumpLeft";
                    jumpRight = "Player/SpikeSabre/HeroJumpRight";
                    break;
                case "ToyntonHammer":
                    idleAnim = "Player/ToyntonHammer/HeroIdleRight";
                    idleLeftAnim = "Player/ToyntonHammer/HeroIdleLeft";
                    runAnim = "Player/ToyntonHammer/HeroWalkRight";
                    runLeftAnim = "Player/ToyntonHammer/HeroWalkLeft";
                    lightRight = "Player/ToyntonHammer/HeroAttackRight";
                    lightLeft = "Player/ToyntonHammer/HeroAttackLeft";
                    hurtLeft = "Player/ToyntonHammer/HeroLeftHurt";
                    hurtRight = "Player/ToyntonHammer/HeroRightHurt";
                    jumpLeft = "Player/ToyntonHammer/HeroJumpLeft";
                    jumpRight = "Player/ToyntonHammer/HeroJumpRight";
                    break;
                case "Tripod":
                    idleAnim = "Player/Tripod/HeroIdleRight";
                    idleLeftAnim = "Player/Tripod/HeroIdleLeft";
                    runAnim = "Player/Tripod/HeroWalkRight";
                    runLeftAnim = "Player/Tripod/HeroWalkLeft";
                    lightRight = "Player/Tripod/HeroAttackRight";
                    lightLeft = "Player/Tripod/HeroAttackLeft";
                    hurtLeft = "Player/Tripod/HeroLeftHurt";
                    hurtRight = "Player/Tripod/HeroRightHurt";
                    jumpLeft = "Player/Tripod/HeroJumpLeft";
                    jumpRight = "Player/Tripod/HeroJumpRight";
                    break;
                case "MayaSword":
                    idleAnim = "Player/MayaSword/HeroIdleRight";
                    idleLeftAnim = "Player/MayaSword/HeroIdleLeft";
                    runAnim = "Player/MayaSword/HeroWalkRight";
                    runLeftAnim = "Player/MayaSword/HeroWalkLeft";
                    lightRight = "Player/MayaSword/HeroAttackRight";
                    lightLeft = "Player/MayaSword/HeroAttackLeft";
                    hurtLeft = "Player/MayaSword/HeroLeftHurt";
                    hurtRight = "Player/MayaSword/HeroRightHurt";
                    jumpLeft = "Player/MayaSword/HeroJumpLeft";
                    jumpRight = "Player/MayaSword/HeroJumpRight";
                    break;
                case "Parasol":
                    idleAnim = "Player/Parasol/HeroIdleRight";
                    idleLeftAnim = "Player/Parasol/HeroIdleLeft";
                    runAnim = "Player/Parasol/HeroWalkRight";
                    runLeftAnim = "Player/Parasol/HeroWalkLeft";
                    lightRight = "Player/Parasol/HeroAttackRight";
                    lightLeft = "Player/Parasol/HeroAttackLeft";
                    hurtLeft = "Player/Parasol/HeroLeftHurt";
                    hurtRight = "Player/Parasol/HeroRightHurt";
                    jumpLeft = "Player/Parasol/HeroJumpLeft";
                    jumpRight = "Player/Parasol/HeroJumpRight";
                    break;
                case "ScissorHands":
                    idleAnim = "Player/ScissorHands/HeroIdleRight";
                    idleLeftAnim = "Player/ScissorHands/HeroIdleLeft";
                    runAnim = "Player/ScissorHands/HeroWalkRight";
                    runLeftAnim = "Player/ScissorHands/HeroWalkLeft";
                    lightRight = "Player/ScissorHands/HeroAttackRight";
                    lightLeft = "Player/ScissorHands/HeroAttackLeft";
                    hurtLeft = "Player/ScissorHands/HeroLeftHurt";
                    hurtRight = "Player/ScissorHands/HeroRightHurt";
                    jumpLeft = "Player/ScissorHands/HeroJumpLeft";
                    jumpRight = "Player/ScissorHands/HeroJumpRight";
                    break;
                case "SlingoSlammer":
                    idleAnim = "Player/SlingoSlammer/HeroIdleRight";
                    idleLeftAnim = "Player/SlingoSlammer/HeroIdleLeft";
                    runAnim = "Player/SlingoSlammer/HeroWalkRight";
                    runLeftAnim = "Player/SlingoSlammer/HeroWalkLeft";
                    lightRight = "Player/SlingoSlammer/HeroAttackRight";
                    lightLeft = "Player/SlingoSlammer/HeroAttackLeft";
                    hurtLeft = "Player/SlingoSlammer/HeroLeftHurt";
                    hurtRight = "Player/SlingoSlammer/HeroRightHurt";
                    jumpLeft = "Player/SlingoSlammer/HeroJumpLeft";
                    jumpRight = "Player/SlingoSlammer/HeroJumpRight";
                    break;
            }
        }

        public void lockPlayerControls()
        {
            if (!controlsLocked)
            {
                currentAccel = 0;
                controlsLocked = true;
            }
        }

        public void unlockPlayerControls()
        {
            if (controlsLocked)
                controlsLocked = false;
        }

        public void checkKeysDown(KeyboardState keyState)
        {
            if (!controlsLocked && myState != PlayerState.Hitstun)
            {
                if (keyState.IsKeyDown(Keys.D) == true && previousKeyState.IsKeyDown(Keys.D) == true && myState != PlayerState.Attacking)
                {
                    rightMove = true;
                    facing = 0;
                }
                if (keyState.IsKeyDown(Keys.A) == true && previousKeyState.IsKeyDown(Keys.A) == true && myState != PlayerState.Attacking)
                {
                    leftMove = true;
                    facing = 1;
                }
                if (keyState.IsKeyDown(Keys.W) == true && previousKeyState.IsKeyDown(Keys.W) == true && myState != PlayerState.Jumping)
                {
                    if (!controlsLocked && myState != PlayerState.Attacking && canUpPress)
                    {
                        myState = PlayerState.Jumping;
                        position.Y -= 2;
                        speed.Y = jumpSpeed;
                        canUpPress = false;
                    }
                }
                if (keyState.IsKeyDown(Keys.J) == true && previousKeyState.IsKeyDown(Keys.J) == true)
                {
                    if (canAttack && CheckCollision(BottomBox) && canJpress)
                    {
                        //attack code
                        currentAccel = 0;
                        speed.X = 0;
                        myState = PlayerState.Attacking;
                        //controlsLocked = true;
                        canAttack = false;
                        canJpress = false;
                        attack();
                    }
                }
                if (keyState.IsKeyDown(Keys.K) == true && previousKeyState.IsKeyDown(Keys.K) == true)
                {
                    if (CheckCollision(BottomBox) && canAttack && canKpress)
                    {
                        //k button code
                        if (LevelManager.Instance().panelOne.canStartPuzzle && !LevelManager.Instance().panelOne.winEventOccured)
                        {
                            LevelManager.Instance().startPuzzle(1);
                        }
                        if (LevelManager.Instance().panelTwo.canStartPuzzle && !LevelManager.Instance().panelTwo.winEventOccured)
                        {
                            LevelManager.Instance().startPuzzle(2);
                        }
                        if (LevelManager.Instance().panelThree.canStartPuzzle && !LevelManager.Instance().panelThree.winEventOccured)
                        {
                            LevelManager.Instance().startPuzzle(3);
                        }
                        if (LevelManager.Instance().panelFour.canStartPuzzle && !LevelManager.Instance().panelFour.winEventOccured)
                        {
                            LevelManager.Instance().startPuzzle(4);
                        }
                        if (LevelManager.Instance().panelFive.canStartPuzzle && !LevelManager.Instance().panelFive.winEventOccured)
                        {
                            LevelManager.Instance().startPuzzle(5);
                        }
                    }
                }
                if (keyState.IsKeyDown(Keys.O) == true && previousKeyState.IsKeyDown(Keys.O) == false)
                {
                    if(LevelManager.Instance().currentLevel != "Content/XML/Level4.xml")
                        LevelManager.Instance().nextLevel();
                }

            }
            previousKeyState = keyState;
        }

        public void checkKeysDown(GamePadState keyState)
        {
            if (!controlsLocked && myState != PlayerState.Hitstun)
            {
                if (keyState.ThumbSticks.Left.X >= 0.3 /* && previousKeyState.IsKeyDown(Keys.D) == true */&& myState != PlayerState.Attacking)
                {
                    rightMove = true;
                    facing = 0;
                }
                if (keyState.ThumbSticks.Left.X <= -0.3 /* && previousKeyState.IsKeyDown(Keys.A) == true*/ && myState != PlayerState.Attacking)
                {
                    leftMove = true;
                    facing = 1;
                }
                if (keyState.IsButtonDown(Buttons.A) == true && previousButtonState.IsButtonDown(Buttons.A) == true && myState != PlayerState.Jumping)
                {
                    if (!controlsLocked && myState != PlayerState.Attacking && canUpPress)
                    {
                        myState = PlayerState.Jumping;
                        position.Y -= 2;
                        speed.Y = jumpSpeed;
                        canUpPress = false;
                    }
                }
                if (keyState.IsButtonDown(Buttons.X) == true && previousButtonState.IsButtonDown(Buttons.X) == true)
                {
                    if (canAttack && CheckCollision(BottomBox) && canJpress)
                    {
                        currentAccel = 0;
                        speed.X = 0;
                        myState = PlayerState.Attacking;
                        //controlsLocked = true;
                        canAttack = false;
                        canJpress = false;
                        attack();
                    }
                }
                if (keyState.IsButtonDown(Buttons.Y) == true && previousButtonState.IsButtonDown(Buttons.Y) == true)
                {
                    if (CheckCollision(BottomBox) && canAttack && canKpress)
                    {
                       
                    }
                }
            }
            previousButtonState = keyState;
        }

        public void checkKeysUp(KeyboardState keyState)
        {
            if (myState != PlayerState.Hitstun)
            {
                if (keyState.IsKeyUp(Keys.J) == true)
                {
                    if (canJpress == false)
                        canJpress = true;
                }
                if (keyState.IsKeyUp(Keys.K) == true)
                {
                    if (canKpress == false)
                        canKpress = true;
                }
            }

            if (!controlsLocked && myState != PlayerState.Hitstun && myState != PlayerState.Attacking)
            {
                if (keyState.IsKeyUp(Keys.D) == true)
                {
                    rightMove = false;
                    currentAccel = 0;
                }
                if (keyState.IsKeyUp(Keys.A) == true)
                {
                    leftMove = false;
                    currentAccel = 0;
                }
                if (keyState.IsKeyUp(Keys.W) == true)
                {
                    canUpPress = true;
                }
                //if (keyState.IsKeyUp(Keys.J) == true)
                //{
                //    if (canJpress == false)
                //        canJpress = true;
                //}

                if (!leftMove && !rightMove && CheckCollision(BottomBox))
                {
                    myState = PlayerState.Idle;
                }
            }
        }

        public void checkKeysUp(GamePadState keyState)
        {
            if (myState != PlayerState.Hitstun)
            {
                if (keyState.IsButtonUp(Buttons.X) == true)
                {
                    if (canJpress == false)
                        canJpress = true;
                }
                if (keyState.IsButtonUp(Buttons.Y) == true)
                {
                    if (canKpress == false)
                        canKpress = true;
                }
            }

            if (!controlsLocked && myState != PlayerState.Hitstun && myState != PlayerState.Attacking)
            {
                if (keyState.ThumbSticks.Left.X <= 0.3 && keyState.ThumbSticks.Left.X >= 0)
                {
                    rightMove = false;
                    currentAccel = 0;
                }
                if (keyState.ThumbSticks.Left.X >= -0.3 && keyState.ThumbSticks.Left.X <= 0)
                {
                    leftMove = false;
                    currentAccel = 0;
                }
                if (keyState.IsButtonUp(Buttons.A))
                {
                    canUpPress = true;
                }

                if (!leftMove && !rightMove && CheckCollision(BottomBox))
                {
                    myState = PlayerState.Idle;
                }
            }
        }

        public void UpdateMovement(KeyboardState keyState)
        {
            #region Xmovement
            //if (!hitstun)
            //{
            if (rightMove && !controlsLocked && myState != PlayerState.Attacking)
            {

                if (CheckCollision(BottomBox))
                {
                    myState = PlayerState.Running;
                }

                if (!CheckCollision(RightBox))
                {
                    currentAccel = accelRate;
                }
            }
            if (leftMove && !controlsLocked && myState != PlayerState.Attacking)
            {
                if (CheckCollision(BottomBox))
                {
                    myState = PlayerState.Running;
                }

                if (!CheckCollision(LeftBox))
                {
                    currentAccel = -accelRate;
                }
            }
            //}
            #endregion

            #region gravity
            //makes play fall off ledges when he walks off
            if (!CheckCollision(BottomBox))
            {
                myState = PlayerState.Jumping;
            }
            else
            {
                if (collidingWall.isEnemy)
                {
                    if (position.X < collidingWall.position.X && speed.Y > 0)
                    {
                        //collidingWall.position.X = RightBox.X + RightBox.Width;
                        collidingWall.position.X += 15;
                    }
                    else if (position.X > collidingWall.position.X && speed.Y > 0)
                    {
                        //collidingWall.position.X = position.X - 60;
                        collidingWall.position.X -= 15;
                    }
                }
                else
                {
                    position.Y = collidingWall.BoundingBox.Y - height;
                    speed.Y = 0;
                }
            }

            if (myState == PlayerState.Jumping)
            {
                speed.Y += gravity;

                if (speed.Y > fallSpeed)
                    speed.Y = fallSpeed;

                if (CheckCollision(UpperBox))
                {
                    position.Y = collidingWall.BoundingBox.Y + collidingWall.BoundingBox.Height;
                    speed.Y *= -1;
                }
            }
            #endregion

            #region Acceleration
            if (speed.X - 1 < maxSpeed && speed.X + 1 > -maxSpeed)//accelerate player
            {
                if (rightMove && !leftMove)
                {
                    if (speed.X < maxSpeed || speed.X == 0)
                        speed.X += currentAccel;
                    else if (speed.X > -maxSpeed)
                        speed.X -= runningDecelRate;
                }
                if (leftMove && !rightMove)
                {
                    if (speed.X > -maxSpeed || speed.X == 0)
                        speed.X += currentAccel;
                    else if (speed.X < maxSpeed)
                        speed.X += runningDecelRate;
                }
            }

            else if (speed.X > maxSpeed && myState != PlayerState.Boosting)
            {
                speed.X = maxSpeed;
            }
            else if (speed.X < -maxSpeed && myState != PlayerState.Boosting)
            {
                speed.X = -maxSpeed;
            }

            if (currentAccel == 0 && speed.X != 0 && myState != PlayerState.Boosting)
            {
                if (Math.Abs(speed.X) < 1)
                {
                    speed.X = 0;
                }

                else
                {
                    if (myState != PlayerState.Jumping)
                    {
                        if (speed.X > 0)
                            speed.X -= decelRate;
                        else
                            speed.X += decelRate;
                    }
                    else
                    {
                        if (speed.X > 0)
                            speed.X -= 0.4f;
                        else
                            speed.X += 0.4f;
                    }
                }
            }

            #endregion

            #region Collisions
            if (speed.X > 0)
            {
                if (CheckCollision(RightBox))
                {
                    position.X = collidingWall.BoundingBox.X - 60;//60 is the bounding box position x + its width
                    currentAccel = 0;
                }
            }

            else if (speed.X < 0)
            {
                if (CheckCollision(LeftBox))
                {

                    position.X = collidingWall.BoundingBox.X + collidingWall.BoundingBox.Width;
                    currentAccel = 0;
                }
            }
            #endregion
            //Debug.WriteLine(myState);
            previousState = myState;
            previousPosition = position;
            position += speed;
        }

        public void attack()
        {
            lAttack = new LightAttack((int)(position.X), (int)(position.Y), Game1.Instance().getContent(), facing, damage, attackSpeed, attackReach);
        }

        public void attacksToNull()
        {
            //sets all attacks to null
        }

        public override bool CheckCollision(Rectangle collisionBox)
        {
            wallList = LevelConstructor.Instance().getWallList();
            List<Enemy> enemyList = LevelManager.Instance().getEnemyList();
            foreach (Wall wall in wallList)
            {
                if (wall.BoundingBox.Intersects(collisionBox) && wall.visible)
                {
                    wall.interact();
                    collidingWall = wall;
                    if (wall.name == "NextLevel")
                    {
                        LevelManager.Instance().nextLevel();
                    }
                    return true;
                }
            }
            foreach (Enemy enemy in enemyList)
            {
                if (collisionBox.Intersects(enemy.BoundingBox) && enemy.visible)
                {
                    collidingWall = enemy;
                    currentAccel = 0;
                    return true;
                }
            }
            return false;
        }

        public override void startHitstun(int stunTime)
        {
            myState = PlayerState.Hitstun;
            base.startHitstun(stunTime);
        }

        public override void endHitstun(object sender, ElapsedEventArgs e)
        {
            myState = PlayerState.Idle;
            base.endHitstun(sender, e);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            //drawCollisionBox(spriteBatch, myContent, camera);
            hud.Draw(spriteBatch, camera, position);

            base.Draw(spriteBatch, camera);
        }
    }
}
