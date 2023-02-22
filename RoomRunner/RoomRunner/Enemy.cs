using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomRunner
{

    class Enemy : Animation
    {

        public Texture2D texture;
        public Rectangle rectangle;


        public Enemy(Texture2D texture, Rectangle rectangle, ContentManager content, GraphicsDevice graphics) : base(new string[] {"Idle"})
        {
            this.texture = texture;
            this.rectangle = rectangle;
            AddAnimation("Idle", texture, graphics, 5);


        }


        public new void Update() 
        {
            base.Update();
        }

    }
}
