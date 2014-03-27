using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MDMazeRunner.Sprites
{
    /// <summary>
    /// A class representing bound objects
    /// </summary>
    class Bound : CollidableSprite
    {
        static Texture2D _texture_;
        static int? _frames_;
        static int? _modes_;
        static int? _cellScale_;
        static int? _boundScale_;
        static int? _nonCollidable_;
        public static Texture2D Texture
        {
            get { return _texture_ == null ? new Texture2D(Batch.GraphicsDevice, CellScale * Frames, (CellScale + NonCollidable) * Modes) : _texture_; }
            set { _texture_ = (value.Width / Frames >= CellScale && value.Height / Modes >= CellScale + NonCollidable) ? value : Texture; }
        }
        public static int Frames { get { return _frames_.HasValue ? _frames_.Value : 1; } set { _frames_ = value > 0 ? value : 1; } }
        public static int Modes { get { return _modes_.HasValue ? _modes_.Value : 1; } set { _modes_ = value > 0 ? value : 1; } }
        public static int CellScale { get { return _cellScale_.HasValue ? _cellScale_.Value : 4; } set { _cellScale_ = value > BoundScale + 3 ? value : BoundScale + 3; } }
        public static int BoundScale { get { return _boundScale_.HasValue ? _boundScale_.Value : 1; } set { _boundScale_ = value > 0 && value < CellScale - 2 ? value : 1; } }
        public static int NonCollidable { get { return _nonCollidable_.HasValue ? _nonCollidable_.Value : 0; } set { _nonCollidable_ = value >= 0 ? value : 0; } }

        /// <summary>
        /// Constructor for Bound
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_nonCollidable">Specifies noncollidable part of bound</param>
        /// <param name="_frames">Animation frames</param>
        /// <param name="_modes">Different modes</param>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_width">Width of sprite</param>
        /// <param name="_depth">Depth of sprite</param>
        public Bound(Texture2D _texture, int _nonCollidable, int _frames, int _modes, Vector2 _position, int _width, int _depth) :
            base(_texture, 0, 0, _nonCollidable, 0, _frames, _modes, _position, _width, _depth)
        {
        }

        /// <summary>
        /// Constructor for Bound
        /// Creates a corner
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_nonCollidable">Specifies noncollidable part of bound</param>
        /// <param name="_frames">Animation frames</param>
        /// <param name="_modes">Different modes</param>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_boundScale">Bound scale in pixels</param>
        public Bound(Texture2D _texture, int _nonCollidable, int _frames, int _modes, Vector2 _position, int _boundScale) :
            base(_texture, 0, 0, _nonCollidable, 0, _frames, _modes, _position, _boundScale, _boundScale)
        {
        }

        /// <summary>
        /// Constructor for Bound
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_nonCollidable">Specifies noncollidable part of bound</param>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_width">Width of sprite</param>
        /// <param name="_depth">Depth of sprite</param>
        public Bound(Texture2D _texture, int _nonCollidable, Vector2 _position, int _width, int _depth) :
            base(_texture, 0, 0, _nonCollidable, 0, _position, _width, _depth)
        {
        }

        /// <summary>
        /// Constructor for Bound
        /// Creates a corner
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_nonCollidable">Specifies noncollidable part of bound</param>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_boundScale">Bound scale in pixels</param>
        public Bound(Texture2D _texture, int _nonCollidable, Vector2 _position, int _boundScale) :
            base(_texture, 0, 0, _nonCollidable, 0,  _position, _boundScale, _boundScale)
        {
        }

        /// <summary>
        /// Constructor for Bound
        /// </summary>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_width">Width of sprite</param>
        /// <param name="_depth">Depth of sprite</param>
        public Bound(Vector2 _position, int _width, int _depth) :
            base(Texture, 0, 0, NonCollidable, 0, _position, _width, _depth)
        {
        }

        /// <summary>
        /// Constructor for Bound
        /// Creates a corner
        /// </summary>
        /// <param name="_position">Position of sprite center</param>
        public Bound(Vector2 _position) :
            base(Texture, 0, 0, NonCollidable, 0, _position, BoundScale, BoundScale)
        {
        }

        public override void Collision(CollidableSprite _sprite)
        {
            if (_sprite is Character)
                ((Character)_sprite).BoundCollision();
        }
    }
}
