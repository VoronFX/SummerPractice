using GoblinsGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatapultGame.Players
//namespace GoblinsGame

{
    class Hero 
    {

        public List<Squad> army = new List<Squad>();
        public Point currentPosition = new Point();
        //protected TheGame curGame;
        protected Texture2D texture;

        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                if (value != null)
                    texture = value;
                else
                    throw new NullReferenceException("Hero Texture!");
            }
        }

        public Hero(Texture2D text, int x, int y)
        {
            texture = text;
            currentPosition.X = x;
            currentPosition.Y = y;
        }

        public Hero(Texture2D text) : this(text, 0, 0) { }

        public void Go(int x, int y)
        {
            if (x >= 0 && y >= 0)
            {
                currentPosition.X = x;
                currentPosition.Y = y;
            }
            else
            {
                throw new ArgumentException("Hero Uncorrect Position");
            }
        }
        
        //public Hero(Game game,)
        //{

        //}


    }
}
