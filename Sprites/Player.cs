using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace MDMazeRunner.Sprites
{
    /// <summary>
    /// A class defining the player
    /// </summary>
    class Player : Character
    {
        /// <summary>
        /// An enum class that defines input types
        /// </summary>
        protected enum Input
        { B = 0, L = 1, F = 2, R = 3, tX = 4, tY = 5, tZ = 6, rX = 7, rY = 8, rZ = 9 }

        /// <summary>
        /// Constructor for Player
        /// </summary>
        /// <param name="_texture"></param>
        /// <param name="_nonCollidable"></param>
        /// <param name="_frames"></param>
        /// <param name="_modes"></param>
        /// <param name="_maxSpeed"></param>
        /// <param name="_cellScale"></param>
        /// <param name="_tileSize"></param>
        /// <param name="_dInfo"></param>
        /// <param name="_initialD"></param>
        /// <param name="_initialCell"></param>
        internal Player(Texture2D _texture, int _nonCollidable, int _frames, int _modes, int _maxSpeed, int _cellScale, int _tileSize,
            int[] _dInfo, int _initialD, int[] _initialCell)
            : base(_texture, 0, 0, _nonCollidable, 0, _frames, _modes, _maxSpeed, _cellScale, _tileSize, _dInfo, _initialD, _initialCell)
        {
        }

        public void UseInput(bool[] _input)
        {
            if (_input[(int)Input.F])
                Forward();
            if (_input[(int)Input.B])
                Backward();
            if (_input[(int)Input.R])
                Right();
            if (_input[(int)Input.L])
                Left();

            if (_input[(int)Input.tX])
                ShiftDimension(X, 1);
            if (_input[(int)Input.tY])
                ShiftDimension(Y, 1);
            if (_input[(int)Input.tZ])
                ShiftDimension(Z, 1);
            if (_input[(int)Input.rX])
                ShiftDimension(X, -1);
            if (_input[(int)Input.rY])
                ShiftDimension(Y, -1);
            if (_input[(int)Input.rZ])
                ShiftDimension(Z, -1);
        }
    }
}
