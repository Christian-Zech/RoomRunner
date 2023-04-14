using Microsoft.Xna.Framework;
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

        public Rectangle Rect;
        public int BossDamage;
        public readonly bool HasGravity;
        public Point Velocity;
        public bool InFrame;
        public bool DeltDamage;
        public bool Persists;
        public int Lifespan;
        public OnetimeAnimation anim;
        public bool ToRemove { get { return !InFrame || DeltDamage; } }

        private readonly static int FrameWidth, FrameHeight;

        public bool DamagesPlayer, DamagesBoss;

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
            Rect = rect;
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
            Rect = rect;
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

        public void Update()
        {
            if (!InFrame) return;
            if (Lifespan > 0) Lifespan--;
            IsInFrame();
            if (Lifespan == 0) { InFrame = false; DeltDamage = true; }
            Rect.X += Velocity.X;
            Rect.Y += Velocity.Y;
        }

        

        private void IsInFrame()
        {
            if (!InFrame || Persists) return;
            bool a, b, c, d;
            a = Rect.X + Rect.Width < 0;
            b = Rect.X > FrameWidth;
            c = Rect.Y + Rect.Height < 0;
            d = Rect.Y > FrameHeight;
            InFrame = !(a || b || c || d);
        }
        public Projectile Clone() { return new Projectile(new Rectangle(Rect.X, Rect.Y, Rect.Width, Rect.Height), BossDamage, Velocity, anim.Clone(), DamagesBoss, DamagesPlayer, HasGravity); }
        public void Draw(SpriteBatch sb)
        {
            Draw(sb, false, false);
        }
        public void Draw(SpriteBatch sb, bool flipX, bool flipY)
        {
            if (anim == default) { sb.Draw(Game1.pixel, Rect, Color.Red); return; }
            Texture2D txt = anim.CurrentTexture;
            if (flipX)
                sb.Draw(txt, Rect, null, Color.White, 0.0f, new Vector2(txt.Width / 2, txt.Height / 2), SpriteEffects.FlipHorizontally, 0.0f);
            else if (flipY)
                sb.Draw(txt, new Rectangle(Rect.X, Rect.Y + Rect.Height / 2, Rect.Width, Rect.Height), null, Color.White, 0.0f, new Vector2(txt.Width / 2, txt.Height / 2), SpriteEffects.FlipVertically, 0.0f);
            else
                sb.Draw(txt, Rect, Color.White);
        }
    }
    public class ProjectileClump
    {
        public Queue<Projectile> projs;
        public bool FlipX, FlipY, Delete;
        public Projectile Current { get { return projs.Peek(); } }

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
            Program.Game.UpdateProjectile(Current);
            if (!Current.InFrame || Current.DeltDamage) projs.Dequeue();
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
