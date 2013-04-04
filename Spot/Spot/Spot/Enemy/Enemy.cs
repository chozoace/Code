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
    class Enemy : MoveableObject
    {
        public enum EnemyState
        {
            Jumping,
            Running,
            Idle,
            Attacking,
            Dead,
            Waiting,
            Hitstun,
            Thrown
        }

        public int detectionRange = 300;//tracking distance
        public int scoreAward;
        public EnemyState enemyState;
        protected ContentManager enemyContent = Game1.Instance().getContent();
        protected Rectangle leftDetectionBox;
        protected Rectangle rightDetectionBox;
        protected bool canAnimChange = true;
        protected bool comboable = false;
        protected List<Hitbox> attacks = new List<Hitbox>();
        public Timer comboTime;

        public EventHandler currentEvent;
        public EventArgs eArgs = new EventArgs();
        float throwHeight;
        int damageToBeApplied;
        int stunTimeToBeApplied;
        bool assignGravity = true; //TEMPORARY
        protected bool canUpdate = true;

        protected PuzzlePickUp myPuzzle;

        public Enemy()
        {
           
        }

        public virtual void LoadContent()
        {
            isEnemy = true;
            currentEvent = new EventHandler(nothingState);
            currentAnimation = idleLeftAnim;
            texture = enemyContent.Load<Texture2D>(currentAnimation);
            animationRect = new Rectangle(0, 0, width, height);
            animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
            animTimer.Enabled = true;

            enemyState = EnemyState.Idle;
        }

        public override void Update()
        {
            currentEvent(this, eArgs);
        }

        public void UpdateLife()
        {
            if (health <= 0)
            {
                comboTime.Dispose();
                enemyState = EnemyState.Dead;
                canUpdate = false;
                currentEvent = new EventHandler(deathState);
                //death();
            }
        }

        public void nothingState(object sender, EventArgs e)
        {

        }

        public void startThrow(int damage, int stunTime)
        {
            damageToBeApplied = damage;
            stunTimeToBeApplied = stunTime;

            throwHeight = position.Y - 40;
            currentEvent = new EventHandler(throwEvent);
        }

        public void throwEvent(object sender, EventArgs e)
        {
            gravity = .8f;
            if(facing == 0)
            {
                speed.X = -6;
            }
            else
            {
                speed.X = 6;
            }
            if (assignGravity)
            {
                speed.Y = -2;
                assignGravity = false;
            }

            if (CheckCollision(BottomBox) && collidingWall != LevelManager.Instance().player)
            {
                currentEvent = new EventHandler(nothingState);
                gravity = 1.5f;
                hitBoxCollide(stunTimeToBeApplied);
                health -= damageToBeApplied;
                assignGravity = true;
            }
            else
            {

            }
        }

        public void hitBoxCollide(int stunTime)
        {
            if (this.enemyState != EnemyState.Hitstun)
            {
                this.enemyState = EnemyState.Hitstun;
                Debug.WriteLine("hitstun");
                startComboStun(stunTime);
            }
            else
            {
                startComboStun(stunTime);
            }
        }

        public void startComboStun(int stunTime)
        {
            comboTime = new Timer(stunTime);
            comboTime.Elapsed += new ElapsedEventHandler(endComboStun);
            comboTime.Enabled = true;
        }

        public void endComboStun(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine("endComboStun");
            if (comboTime != null)
            {
                comboTime.Dispose();
                comboTime = null;
                enemyState = EnemyState.Idle;
            }
        }

        public virtual void deathState(object sender, EventArgs e)
        {
            if (facing == 0 && currentAnimation != deathRight)
            {
                animationRect = new Rectangle(0, 0, width, height);
                texture = Game1.Instance().Content.Load<Texture2D>(deathRight);
                currentAnimation = deathRight;
            }
            if (facing == 1 && currentAnimation != deathLeft)
            {
                animationRect = new Rectangle(0, 0, width, height);
                texture = Game1.Instance().Content.Load<Texture2D>(deathLeft);
                currentAnimation = deathLeft;
            }

            if (currentFrame == totalFrames - 1)
            {
                currentEvent = new EventHandler(nothingState);
                LevelManager.Instance().removefromSpriteList(this);
                LevelManager.Instance().removefromEnemyList(this);

                if(facing == 0)
                    myPuzzle = new CirclePuzzlePiece(new Vector2(position.X/* - 50*/, position.Y));
                else
                    myPuzzle = new CirclePuzzlePiece(new Vector2(position.X/* + 50*/, position.Y));
                LevelManager.Instance().addToSpriteList(myPuzzle);
            }
        }

        public virtual void popInState(object sender, EventArgs e)
        {
            //speed.X = -6;
            //speed.Y = -2;

            if(CheckCollision(BoundingBox))
            {
                currentEvent = new EventHandler(nothingState);
            }
        }

        public void death()
        {
            //do some animation then timer in death
            LevelManager.Instance().removefromSpriteList(this);
            LevelManager.Instance().removefromEnemyList(this);
        }

        public virtual bool CheckDetection()
        {
            Player player = LevelManager.Instance().player;
            if (player.position.X < position.X)
            {
                facing = 1;
                if (position.X - (player.position.X) < detectionRange)
                {
                    return true;
                }
            }
            else if (player.position.X > position.X)
            {
                facing = 0;
                if (player.position.X - (position.X) < detectionRange)
                {
                    return true;
                }
            }
            
            return false;
        }

    }
}
