using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MDMazeRunner.Sprites
{
    /// <summary>
    /// An abstract definition of a staircase
    /// </summary>
    abstract class Staircase : Functional
    {
        protected static readonly int dirAscend = 1, dirDescend = -1;
        static Texture2D _aTexture_, _dTexture_;
        static int? _aFrames_, _dFrames_;
        static int? _aModes_, _dModes_;
        static int? _openingScale_;
        static int? _nonCollidable_;
        public static Texture2D Ascending
        {
            get { return _aTexture_ == null ? new Texture2D(Batch.GraphicsDevice, OpeningScale * AscendingFrames, (OpeningScale + NonCollidable) * AscendingModes) : _aTexture_; }
            set { _aTexture_ = (value.Width / AscendingFrames >= OpeningScale && value.Height / AscendingModes >= OpeningScale + NonCollidable) ? value : Ascending; }
        }
        public static Texture2D Descending 
        {
            get { return _dTexture_ == null ? new Texture2D(Batch.GraphicsDevice, OpeningScale * DescendingFrames, OpeningScale * DescendingModes) : _dTexture_; }
            set { _dTexture_ = (value.Width / DescendingFrames >= OpeningScale && value.Height / DescendingModes >= OpeningScale) ? value : Descending; }
        }
        public static int AscendingFrames { get { return _aFrames_.HasValue ? _aFrames_.Value : 1; } set { _aFrames_ = value > 0 ? value : 1; } }
        public static int AscendingModes { get { return _aModes_.HasValue ? _aModes_.Value : 1; } set { _aModes_ = value > 0 ? value : 1; } }
        public static int DescendingFrames { get { return _dFrames_.HasValue ? _dFrames_.Value : 1; } set { _dFrames_ = value > 0 ? value : 1; } }
        public static int DescendingModes { get { return _dModes_.HasValue ? _dModes_.Value : 1; } set { _dModes_ = value > 0 ? value : 1; } }
        public static int OpeningScale { get { return _openingScale_.HasValue ? _openingScale_.Value : 1; } set { _openingScale_ = value > 0 ? value : 1; } }
        public static int NonCollidable { get { return _nonCollidable_.HasValue ? _nonCollidable_.Value : 0; } set { _nonCollidable_ = value >= 0 ? value : 0; } }

        readonly int direction;

        /// <summary>
        /// Constructor for Staircase
        /// Creates Ascending
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_nonCollidable">Specifies noncollidable part of stair</param>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_openingScale">Width of sprite</param>
        /// <param name="_direction">Direction of staircase</param>
        public Staircase(Texture2D _texture, int _nonCollidable, Vector2 _position, int _openingScale, int _direction) :
            base(_texture, 0, 0, _nonCollidable, 0, _position, _openingScale, _openingScale)
        {
            direction = _direction < 0 ? -1 : 1;
        }

        /// <summary>
        /// Constructor for Staircase
        /// Creates Descending
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_openingScale">Width of sprite</param>
        /// <param name="_direction">Direction of staircase</param>
        public Staircase(Texture2D _texture, Vector2 _position, int _openingScale, int _direction) :
            base(_texture, 0, 0, 0, 0, _position, _openingScale, _openingScale)
        {
            direction = _direction < 0 ? -1 : 1;
        }

        /// <summary>
        /// Constructor for Staircase
        /// </summary>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_direction">Direction of staircase</param>
        public Staircase(Vector2 _position, int _direction) :
            base(_direction < 0 ? Descending : Ascending, 0, 0, _direction < 0 ? 0 : NonCollidable, 0, _position, OpeningScale, OpeningScale)
        {
            direction = _direction < 0 ? -1 : 1;
        }

        /// <summary>
        /// Detects if a collision occurs with the specified CollidableSprite and takes appropriate action
        /// </summary>
        /// <param name="_sprite">Specified CollidableSprite</param>
        public override void Collision(CollidableSprite _sprite)
        {
            if (_sprite is Character)
                ((Character)_sprite).StairCollision(this);
        }

        /// <summary>
        /// Method to be called when trying to transverse a staircase
        /// Either returns direction of the staircase (1 or -1) or 0, in which case the staircase acts as a bound
        /// </summary>
        /// <param name="_sprite">Sprite trying to transverse staircase</param>
        /// <returns></returns>
        internal int Transverse(Character _sprite)
        {
            if (_sprite is Player)
                //If sprite is in proper place to transverse stairs
                if (direction * _sprite.NegX <= direction * PosX && direction * _sprite.PosX <= direction * NegX &&
                    _sprite.NegY > NegY && _sprite.PosY < PosY)
                    return direction;
            return 0;
        }
    }
}
