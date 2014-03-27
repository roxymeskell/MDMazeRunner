using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MDMazeRunner.Sprites
{
    /// <summary>
    /// An abstract definition of character sprites
    /// </summary>
    abstract class Character : CollidableSprite
    {
        protected static readonly int X = 0, Y = 1, Z = 2;
        protected readonly int maxSpeed;
        //cellScale - the tiles in a cell
        //cellSize - the pixels in a cell
        //tileSize - the pixels in a tile
        protected readonly int cellScale, cellSize, tileSize;
        protected int[] dInfo, currDimensions;
        //currCell - Cell character is currently occupying
        //currTile - Tile IN THE CELL character is currently occupying
        //currCoor - Pixel coordinates of the location of character
        protected int[] currCell, currCoor, currTile;
        protected Vector2 velocity, acceleration, velocityNoMax, nextAcceleration, positionToWorld;
        protected bool viewable;
        public int CellScale { get { return cellScale; } }
        public int BoundScale { get { return 1; } }
        public int InteriorScale { get { return 0; } }
        public int XDimension
        {
            get { return currDimensions[X]; }
            set
            {
                if (currDimensions.Contains(value))
                    currDimensions[currDimensions[X] == value ? X : (currDimensions[Y] == value ? Y : Z)] = currDimensions[X];
                currDimensions[X] = value < dInfo.Length && value >= 0 ? value : currDimensions[X];
            }
        }
        public int YDimension
        {
            get { return currDimensions[Y]; }
            set
            {
                if (currDimensions.Contains(value))
                    currDimensions[currDimensions[X] == value ? X : (currDimensions[Y] == value ? Y : Z)] = currDimensions[Y];
                currDimensions[Y] = value < dInfo.Length && value >= 0 ? value : currDimensions[Y];
            }
        }
        public int ZDimension
        {
            get { return currDimensions[Z]; }
            set
            {
                if (currDimensions.Contains(value))
                    currDimensions[currDimensions[X] == value ? X : (currDimensions[Y] == value ? Y : Z)] = currDimensions[Z];
                currDimensions[Z] = value < dInfo.Length && value >= 0 ? value : currDimensions[Z];
            }
        }
        public int[] CurrentCell { get { return currCell; } }
        public int[] CurrentCellCoor
        {
            get
            {
                int[] _cellCoor = new int[currCoor.Length];
                for (int _d = 0; _d < currCoor.Length; _d++)
                    _cellCoor[_d] = (currCoor[_d] - BoundScale) % CellScale;
                return _cellCoor;
            }
            set
            {
                int[] _currCoor = new int[currCoor.Length];
                for (int _d = 0; _d < currCoor.Length; _d++)
                    _currCoor[_d] = (currCell[_d] * CellScale) + value[_d] + BoundScale;
                currCoor = _currCoor;
            }
        }
        public int[] CurrentTile { get { return currTile; } }
        public int[] CurrentDimensions { get { return currDimensions; } }
        public int[] Current2DCell { get { return new int[] { currCell[currDimensions[X]], currCell[currDimensions[Y]] }; } }
        public int[] Current2DCellCoor
        {
            get { return new int[] { (currCoor[XDimension] - BoundScale) % CellScale, (currCoor[YDimension] - BoundScale) % CellScale }; }
            set
            {
                currCoor[XDimension] = (currCell[XDimension] * CellScale) + value[X] + BoundScale;
                currCoor[YDimension] = (currCell[YDimension] * CellScale) + value[Y] + BoundScale;
            }
        }
        public int[] Current2DTile { get { return new int[] { currTile[currDimensions[X]], currTile[currDimensions[Y]] }; } }

        /// <summary>
        /// Constructor for Character
        /// WILL NEED TO BE REVISED
        /// </summary>
        /// <param name="_texture"></param>
        /// <param name="_negX"></param>
        /// <param name="_posX"></param>
        /// <param name="_negY"></param>
        /// <param name="_posY"></param>
        /// <param name="_frames"></param>
        /// <param name="_modes"></param>
        /// <param name="_maxSpeed"></param>
        /// <param name="_cellScale"></param>
        /// <param name="_tileSize"></param>
        /// <param name="_dInfo"></param>
        /// <param name="_initialD"></param>
        /// <param name="_initialCell"></param>
        internal Character(Texture2D _texture, int _negX, int _posX, int _negY, int _posY, int _frames, int _modes, int _maxSpeed, int _cellScale, int _tileSize,
            int[] _dInfo, int _initialD, int[] _initialCell)
            : base(_texture, _negX, _posX, _negY, _posY, _frames, _modes)
        {
            //Store spped information
            maxSpeed = _maxSpeed;

            //Store cell information
            cellScale = _cellScale;
            tileSize = _tileSize;
            cellSize = cellScale * tileSize;

            //Randomize initial dimensions
            dInfo = _dInfo;
            currDimensions = new int[3];
            currDimensions[X] = _initialD;
            currDimensions[Y] = Randomize.RandInt((_dInfo.Length - 1), new int[] { currDimensions[X] });
            currDimensions[Z] = Randomize.RandInt((_dInfo.Length - 1), new int[] { currDimensions[X], currDimensions[Y] });

            //Store initial cell
            currCell = new int[dInfo.Length];
            _initialCell.CopyTo(currCell, 0);

            //Get and store initial tile
            currTile = new int[dInfo.Length];
            for (int _d = 0; _d < currTile.Length; _d++)
                currTile[_d] = cellScale / 2;
            //Adjust to be on entrance tile
            currTile[currDimensions[X]] = (currCell[currDimensions[X]] > 0 ? 1 : 0) * (cellScale - 1);

            //Get and store initial coordinates
            currCoor = new int[dInfo.Length];
            for (int _d = 0; _d < currCoor.Length; _d++)
                currCoor[_d] = (currCell[_d] * cellSize) + (currTile[_d] * tileSize) + (tileSize / 2);

            //Set intial position according to coordinates
            position = (new Vector2(currCoor[currDimensions[X]], currCoor[currDimensions[X]]));

            //Set other variables
            velocityNoMax = new Vector2(0, 0);
            nextAcceleration = new Vector2(0, 0);
            viewable = true;
        }

        /// <summary>
        /// Private method to shift a dimension
        /// PLACE IN CENTER OF CELL ON DIMENSION SHIFT
        /// </summary>
        /// <param name="_d">X, Y, or Z dimension to shift</param>
        /// <param name="_shift">Direction and magnitude of shift</param>
        protected void ShiftDimension(int _d, int _shift)
        {
            //Switch dimension
            int _holdD = currDimensions[_d];

            if (dInfo.Length > 3)
            {
                do
                {
                    currDimensions[_d] = (currDimensions[_d] + _shift) % dInfo.Length;
                    currDimensions[_d] = currDimensions[_d] < 0 ? (currDimensions[_d] + dInfo.Length) : currDimensions[_d];
                } while ((currDimensions[_d] == currDimensions[(_d + 1) % 3]) || (currDimensions[_d] == currDimensions[(_d + 2) % 3]));
            }
            else
            {
                int _nextD = currDimensions[Z];
                currDimensions[_d] = currDimensions[Z];
                currDimensions[Z] = _holdD;
            }

            //Place character in center of cell
            Current2DCellCoor = new int[] { InteriorScale / 2, InteriorScale / 2 };
        }

        /// <summary>
        /// Move positively in Z dimension
        /// </summary>
        public void Up()
        {
            currCell[currDimensions[Z]]++;
            currCoor[currDimensions[Z]] += cellSize;
        }

        /// <summary>
        /// Move negatively in Z dimension
        /// </summary>
        public void Down()
        {
            currCell[currDimensions[Z]]--;
            currCoor[currDimensions[Z]] -= cellSize;
        }

        /// <summary>
        /// Accelerate positively in Y dimension
        /// </summary>
        public void Forward()
        {
            nextAcceleration.Y += 2;
        }

        /// <summary>
        /// Accelerate negatively in Y dimension
        /// </summary>
        public void Backward()
        {
            nextAcceleration.Y -= 2;
        }

        /// <summary>
        /// Accelerate negatively in X dimension
        /// </summary>
        public void Left()
        {
            nextAcceleration.X -= 2;
        }

        /// <summary>
        /// Accelerate positively in X dimension
        /// </summary>
        public void Right()
        {
            nextAcceleration.X += 2;
        }

        /// <summary>
        /// Updates position based on velocity
        /// </summary>
        public virtual void Move()
        {
            Position += velocity;
            currCoor[currDimensions[X]] += (int)velocity.X;
            currCoor[currDimensions[Y]] += (int)velocity.Y;
            UpdateCurrCellAndTile();
        }

        /// <summary>
        /// Resets position to before last call to move
        /// </summary>
        public virtual void UndoMovement()
        {
            Position -= velocity;
            currCoor[currDimensions[X]] -= (int)velocity.X;
            currCoor[currDimensions[Y]] -= (int)velocity.Y;
            UpdateCurrCellAndTile();
        }

        /// <summary>
        /// Updates current cell and current tile within cell according to current coordinates
        /// </summary>
        private void UpdateCurrCellAndTile()
        {
            int[] _iCell = new int[] { currCell[currDimensions[X]], currCell[currDimensions[Y]] };
            int[] _iTile = new int[] { currTile[currDimensions[X]], currTile[currDimensions[Y]] };

            //Update current cell
            currCell[currDimensions[X]] = currCoor[currDimensions[X]] / cellSize;
            currCell[currDimensions[Y]] = currCoor[currDimensions[Y]] / cellSize;

            //Update current tile
            currTile[currDimensions[X]] = (currCoor[currDimensions[X]] - (currCell[currDimensions[X]] * cellSize)) / tileSize;
            currTile[currDimensions[Y]] = (currCoor[currDimensions[Y]] - (currCell[currDimensions[Y]] * cellSize)) / tileSize;

            if ((_iCell[X] != currCell[currDimensions[X]]) || (_iCell[Y] != currCell[currDimensions[Y]]) ||
                (_iTile[X] != currTile[currDimensions[X]]) || (_iTile[Y] != currTile[currDimensions[Y]]))
                Console.WriteLine("Character--\n   currCell[currDimensions[x]]:" + currCell[currDimensions[X]] + " currCell[currDimensions[y]]:" + currCell[currDimensions[Y]] +
                    "\n   currTile[x]:" + currTile[currDimensions[X]] + " currTile[currDimensions[y]]:" + currTile[currDimensions[Y]]);
        }

        /// <summary>
        /// Updates the sprite during the game
        /// </summary>
        /// <param name="_time">GameTime object</param>
        public override void Update(GameTime time)
        {
            acceleration = nextAcceleration;
            nextAcceleration = new Vector2(0, 0);

            velocityNoMax += acceleration;

            //Apply friction
            velocityNoMax.X += (velocityNoMax.X > 0 ? -1 : velocityNoMax.X < 0 ? 1 : 0);
            velocityNoMax.Y += (velocityNoMax.Y > 0 ? -1 : velocityNoMax.Y < 0 ? 1 : 0);

            //Keep to max speed
            float _speed = velocityNoMax.Length();
            float _vMod = _speed != 0 ? (Math.Abs(_speed) % maxSpeed) / Math.Abs(_speed) : 0;
            velocity.X = velocityNoMax.X * _vMod;
            velocity.Y = velocityNoMax.Y * _vMod;
            _speed = velocity.Length();

            //Animates character if in motion
            //MAY WANT TO CHANGE LATER TO CONTROL TEMPO OF ANIMATION USING SPEED
            if (_speed > 0)
                texture.NextFrame();

            //Move
            Move();
        }

        /// <summary>
        /// Responds to colliding with a bound
        /// </summary>
        internal void BoundCollision()
        {
            UndoMovement();
        }

        /// <summary>
        /// Responds to colliding with a functional sprite
        /// </summary>
        /// <param name="_sprite">Functional sprite collided with</param>
        internal abstract void FunctionalCollision(Functional _sprite);

        /// <summary>
        /// Responds to colliding with a staircase sprite
        /// </summary>
        /// <param name="_sprite">Functional sprite collided with</param>
        internal abstract void StairCollision(Functional _sprite);
    }
}
