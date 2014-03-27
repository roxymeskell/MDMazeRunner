using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MDMazeRunner.Sprites
{
    /// <summary>
    /// A class defining a descending staircase
    /// </summary>
    class Descending :Staircase
    {
        /// <summary>
        /// Constructor for Descending
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_openingScale">Width of sprite</param>
        /// <param name="_direction">Depth of sprite</param>
        public Descending(Texture2D _texture, Vector2 _position, int _openingScale, int _direction) :
            base(_texture, _position, _openingScale, 1)
        {
        }

        /// <summary>
        /// Constructor for Descending
        /// </summary>
        /// <param name="_position">Position of sprite center</param>
        public Descending(Vector2 _position, int _direction) :
            base(_position, -1)
        {
        }
    }
}
