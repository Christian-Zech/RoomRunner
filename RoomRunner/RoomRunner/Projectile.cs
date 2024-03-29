﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomRunner
{
    public class Projectile
    {
        public readonly static Dictionary<Projectiles, Projectile> Defaults;

        public Rectangle Rectangle { get { if (getRect != default) return getRect.Invoke(); return rect; } }
        private Rectangle rect;
        private Func<Rectangle> getRect;
        public int BossDamage;
        public readonly bool HasGravity;
        public Point Velocity;
        public bool InFrame;
        public bool DeltDamage;
        public bool Persists;
        public int Lifespan;
        public OnetimeAnimation anim;
        public bool ToRemove { get { return !InFrame || (!Persists && DeltDamage); } } // placeholder: public bool ToRemove { get { return !InFrame || (!Persists && DeltDamage); } }

        private readonly static int FrameWidth, FrameHeight;

        public bool DamagesPlayer, DamagesBoss;
        public Point Position
        {
            set
            {
                rect.X = value.X;
                rect.Y = value.Y;
            }
        }

        static Projectile()
        {
            FrameWidth = Game1.window.Width;
            FrameHeight = Game1.window.Height;

            Defaults = new Dictionary<Projectiles, Projectile>();
            List<Projectile> projs = new List<Projectile>
            {
                new Projectile(new Rectangle(0, 0, 50, 50), 5, new Point(8, 0), OnetimeAnimation.Anims[OnetimeAnims.Fireball].Clone())
            };

            for (int i = 0; i < projs.Count; i++)
                Defaults.Add((Projectiles)i, projs[i]);
        }
        public Projectile(Rectangle rect, int bossDmg, Point velo, OnetimeAnimation anim = default, bool dmgBoss = true, bool dmgPlayer = false, bool hasGravity = false)
        {
            this.anim = anim;
            this.rect = rect;
            BossDamage = bossDmg;
            Velocity = velo;
            Lifespan = -1;
            HasGravity = hasGravity;
            DamagesBoss = dmgBoss;
            DamagesPlayer = dmgPlayer;
            InFrame = true;
        }
        public Projectile(bool persists, Rectangle rect, int bossDmg, Point velo, OnetimeAnimation anim = default, bool dmgBoss = true, bool dmgPlayer = false, bool hasGravity = false) : this(rect, bossDmg, velo, anim, dmgBoss, dmgPlayer, hasGravity)
        {
            Persists = persists;
        }
        public Projectile(Rectangle rect, int bossDmg, int lifespan, OnetimeAnimation anim = default, bool dmgBoss = true, bool dmgPlayer = false, bool hasGravity = false)
        {
            this.anim = anim;
            this.rect = rect;
            BossDamage = bossDmg;
            Velocity = Point.Zero;
            Lifespan = lifespan;
            HasGravity = hasGravity;
            DamagesBoss = dmgBoss;
            DamagesPlayer = dmgPlayer;
            Persists = false;
            InFrame = true;
        }
        public Projectile(bool persists, Rectangle rect, int bossDmg, int lifespan, OnetimeAnimation anim = default, bool dmgBoss = true, bool dmgPlayer = false, bool hasGravity = false) : this(rect, bossDmg, lifespan, anim, dmgBoss, dmgPlayer, hasGravity)
        {
            Persists = persists;
        }
        public Projectile(Func<Rectangle> rect, int bossDmg, int lifespan, OnetimeAnimation anim = default, bool dmgBoss = true, bool dmgPlayer = false, bool hasGravity = false)
        {
            this.anim = anim;
            getRect = rect;
            BossDamage = bossDmg;
            Velocity = Point.Zero;
            Lifespan = lifespan;
            HasGravity = hasGravity;
            DamagesBoss = dmgBoss;
            DamagesPlayer = dmgPlayer;
            Persists = false;
            InFrame = true;
        }
        public Projectile(bool persists, Func<Rectangle> rect, int bossDmg, int lifespan, OnetimeAnimation anim = default, bool dmgBoss = true, bool dmgPlayer = false, bool hasGravity = false) : this(rect, bossDmg, lifespan, anim, dmgBoss, dmgPlayer, hasGravity)
        {
            Persists = persists;
        }

        public void Update()
        {
            if (!InFrame) return;
            if (Program.Game.activePowerupIndex == 0)
            {
                //Program.Game.slowTimeTemp++;
                if (Program.Game.slowTimeTemp % 2 == 0)
                    return;

            }
            if (Lifespan > 0) Lifespan--;
            IsInFrame();
            if (Lifespan == 0) { InFrame = false; DeltDamage = true; }
            rect.X += Velocity.X;
            rect.Y += Velocity.Y;
        }

        

        private void IsInFrame()
        {
            if (!InFrame) return;
            bool a, c, d;
            a = rect.X + rect.Width < 0;
            c = rect.Y + rect.Height < 0;
            d = rect.Y > FrameHeight;
            InFrame = !(a || c || d);
        }
        public Projectile Clone() { return new Projectile(new Rectangle(rect.X, rect.Y, rect.Width, rect.Height), BossDamage, Velocity, anim.Clone(), DamagesBoss, DamagesPlayer, HasGravity); }
        public void Draw(SpriteBatch sb)
        {
            Draw(sb, false, false);
        }
        public void Draw(SpriteBatch sb, bool flipX, bool flipY)
        {
            if (anim == default) { sb.Draw(Game1.pixel, rect, Color.Red); return; }
            Texture2D txt = anim.CurrentTexture;
            if (flipX)
                sb.Draw(txt, rect, null, Color.White, 0.0f, new Vector2(txt.Width / 2, txt.Height / 2), SpriteEffects.FlipHorizontally, 0.0f);
            else if (flipY)
                sb.Draw(txt, new Rectangle(rect.X, rect.Y + rect.Height / 2, rect.Width, rect.Height), null, Color.White, 0.0f, new Vector2(txt.Width / 2, txt.Height / 2), SpriteEffects.FlipVertically, 0.0f);
            else
                sb.Draw(txt, rect, Color.White);
        }
        public void Save()
        {
            SaveAndLoad temp = new SaveAndLoad();
        }
    }
    public class ProjectileClump
    {
        public Queue<Projectile> projs;
        public bool FlipX, FlipY, Delete;
        public Projectile Current 
        { 
            get {
                return projs.Peek(); 
            } 
        }

        public ProjectileClump(bool flipX, bool flipY, params Projectile[] projs)
        {
            this.projs = new Queue<Projectile>(projs);
            FlipX = flipX;
            FlipY = flipY;
            Delete = false;
        }
        public void DrawAndUpdate(SpriteBatch sb)
        {
            if (Delete) return;
            if (Program.Game.activePowerupIndex == 1 && Program.Game.slowTimeTemp % 2 == 0) return;
            Program.Game.UpdateProjectile(Current);
            if (!Current.InFrame || (!Current.Persists && Current.DeltDamage)) projs.Dequeue();
            if (projs.Count == 0)
            {
                Delete = true;
                return;
            }
            Current.Draw(sb, FlipX, FlipY);
        }
    }
    public enum Projectiles
    {
        PlayerShot
    }
}
