using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MDMazeRunner.Sprites
{
    /// <summary>
    /// A class defining an ascending staircase
    /// </summary>
    class Ascending : Staircase
    {
        /// <summary>
        /// Constructor for Ascending
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_nonCollidable">Specifies noncollidable part of stair</param>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_openingScale">Width of sprite</param>
        /// <param name="_direction">Depth of sprite</param>
        public Ascending(Texture2D _texture, int _nonCollidable, Vector2 _position, int _openingScale, int _direction) :
            base(_texture, _nonCollidable, _position, _openingScale, 1)
        {
        }

        /// <summary>
        /// Constructor for Ascending
        /// </summary>
        /// <param name="_position">Position of sprite center</param>
        public Ascending(Vector2 _position, int _direction) :
            base(_position, 1)
        {
        }
    }
}
