using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatapultGame.Screens
{
    class Ground
    {
        Texture2D texture;
        int pass;

        public Ground(Texture2D texture, int pass)
        {
            Texture = texture;
            Pass = pass;
        }
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        public int Pass
        {
            get { return pass; }
            set { pass = value; }
        }
    }
}
