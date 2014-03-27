﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDMazeRunner.Maze_Generation
{
    /// <summary>
    /// A class to generate and hold a maze spanning multiple dimensions
    /// </summary>
    class MDMaze
    {
        //The max number of dimensions the maze can have
        protected static readonly int MAX_DIMENSIONS = 16;
        //The interior scale of a cell in the viewable grid
        public static readonly int CELL_SCALE = 1;
        //The scale of a bound in the viewable grid
        public static readonly int BOUND_SCALE = 1;
        //Viewable dimension constants
        protected static readonly int X = 0, Y = 1, Z = 2;

        protected static int dimensions;
        protected static int[] dimensionInfo;
        //protected static int cellScale;
        //protected static float safeZonePercent;
        protected int[] entranceCoor, exitCoor;
        protected static List<Cell> sets;
        //Storage arrays
        //protected static Array bounds;
        protected static Array worldBounds;

        /// <summary>
        /// Constructor for MDMaze
        /// </summary>
        /// <param name="_dInfo">Dimension information</param>
        /// <param name="_cellScale">Size of cell</param>
        /// <param name="_safeZonePercent">Inner percent of cell that is always open and safe to switch dimensions in.</param>
        public MDMaze(int[] _dInfo)
        {
            if (_dInfo.Length > MAX_DIMENSIONS)
                Array.Resize<int>(ref _dInfo, MAX_DIMENSIONS);

            dimensionInfo = _dInfo;
            dimensions = dimensionInfo.Length;

            //cellScale = _cellScale;
            //safeZonePercent = safeZonePercent - (int)safeZonePercent;

            sets = new List<Cell>();

            InitStorage();

            Build();
        }

        /*/// <summary>
        /// Constructor for AbstractMaze
        /// </summary>
        /// <param name="_dInfo">Dimension information</param>
        /// <param name="_cellScale">Size of cell</param>
        /// <param name="_safeZonePercent">Inner percent of cell that is always open and safe to switch dimensions in.</param>
        /// <param name="_values">Additional values to be assigned to variables</param>
        protected AbstractMaze(int[] _dInfo, int _cellScale, float _safeZonePercent, object[] _values)
        {
            dimensionInfo = _dInfo;
            dimensions = dimensionInfo.Length;

            cellScale = _cellScale;
            safeZonePercent = safeZonePercent - (int)safeZonePercent;

            sets = new List<Cell>();

            InitStorage();

            InitVariables(_values);

            Build();
        }*/

        /// <summary>
        /// Initiate storage arrays
        /// </summary>
        protected void InitStorage()
        {
            //Initiate storage arrays
            worldBounds = Array.CreateInstance(typeof(ushort), dimensionInfo);
        }

        /*/// <summary>
        /// Initiates any other variables that are passed in as values
        /// </summary>
        /// <param name="_values">Values passed in</param>
        protected abstract void InitVariables(object[] _values);*/

        /// <summary>
        /// Builds the maze
        /// </summary>
        protected void Build()
        {
            //Get coordinates of root cell in the maze
            int[] _coor = new int[dimensions];
            for (int _i = 0; _i < _coor.Length; _i++)
            {
                _coor[_i] = 0;
            }

            //Create root cell
            Cell _rootCell = GetFirstCell();

            //Expand in all dimensions last to first
            for (int _d = dimensions - 1; _d >= 0; _d--)
            {
                ExpandDimension(_d, _rootCell);
            }

            //Complete maze by joining all sets
            JoinSets();

            //Write all remaining cells to the maze
            sets.ElementAt(0).WriteSetToMaze();

            //Get entrance and exit
            GetOpenings();
        }

        /// <summary>
        /// Creates and returns the first cell in th maze
        /// </summary>
        /// <returns>The first cell in the maze</returns>
        protected Cell GetFirstCell()
        {
            int[] _rootCoor = new int[dimensions];
            for (int i = 0; i < dimensions; i++)
                _rootCoor[i] = 0;
            return new Cell(_rootCoor);
        }

        /// <summary>
        /// Expands cells in specified dimension
        /// </summary>
        /// <param name="_d">Specified dimension</param>
        /// <param name="_cell">Cell being expanded from</param>
        protected void ExpandDimension(int _d, Cell _cell)
        {
            Cell _current = _cell;
            for (int i = 0; i < (dimensionInfo[_d] - 1); i++)
            {
                _current = _current.GetNeighbor(_d);
                if (_current == null)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Merges sets randomly until only one remains
        /// </summary>
        void JoinSets()
        {
            while (sets.Count > 1)
            {
                sets.RemoveAll(x => x.parent != null);

                bool hasDuplicates;

                do
                {
                    foreach (Cell _c in sets)
                    {
                        if (sets.Count(delegate(Cell _inSet) { return (_inSet == _c); }) > 1)
                        {
                            sets.RemoveAll(x => x == _c);
                            sets.Add(_c);
                            break;
                        }
                    }
                    hasDuplicates = false;
                    foreach (Cell _c in sets)
                    {
                        if (sets.Count(delegate(Cell _inSet) { return (_inSet == _c); }) > 1)
                        {
                            hasDuplicates = true;
                            break;
                        }
                    }
                } while (hasDuplicates);

                if (sets.Count > 1)
                {
                    sets.ElementAt(Randomize.RandInt(sets.Count - 1)).MergeSet();
                }
            }
        }

        /// <summary>
        /// Gets the entrance and exit to the maze
        /// </summary>
        protected void GetOpenings()
        {
            //Randomly generate entrance and exit coordinates
            entranceCoor = Randomize.RandOpening(dimensionInfo);
            exitCoor = Randomize.RandOpening(dimensionInfo, entranceCoor);
        }

        /// <summary>
        /// Gets an 2D array of integers that define the maze in the current viewable dimensions
        /// Bit Values define parts of maze
        /// If bit 0 = 1, value represents a bound
        ///     bit 1 represents opened (0) or closed (1) status of bound
        /// If bit 0 = 0, value represents a cell interior
        ///     bit 1 represents ascending (1) or not (0)
        ///     bit 2 represents descending (1) or not (0)
        /// </summary>
        /// <param name="_dimensions">Viewable dimensions {X, Y, Z}</param>
        /// <param name="_currCell">Current cell coordinates in entire maze</param>
        /// <returns>2D array of integers (the current viewable grid of the maze)</returns>
        public int[,] Get2DViewable(int[] _dimensions, int[] _currCell)
        {
            //Initialize view array
            int _viewX = BOUND_SCALE + dimensionInfo[_dimensions[X]] * (BOUND_SCALE + CELL_SCALE),
                _viewY = BOUND_SCALE + dimensionInfo[_dimensions[Y]] * (BOUND_SCALE + CELL_SCALE);
            int[,] _view = new int[_viewX, _viewY];

            //Find coordinates of cell (0, 0) in current view
            int[] _toWrite = new int[dimensions];
            for (int _d = 0; _d < dimensions; _d++)
            {
                _toWrite[_d] = _d == _dimensions[X] || _d == _dimensions[Y] ? 0 : _currCell[_d];
            }


            //Fill view array
            //Intialize view array
            for (int _x = 0; _x < _viewX; _x++)
            {
                for (int _y = 0; _y < _viewY; _y++)
                {
                    //Find IDs
                    int _xID = (_x - BOUND_SCALE) - ((BOUND_SCALE + CELL_SCALE) * (int)Math.Floor((double)(_x - BOUND_SCALE) / (BOUND_SCALE + CELL_SCALE))),
                        _yID = (_y - BOUND_SCALE) - ((BOUND_SCALE + CELL_SCALE) * (int)Math.Floor((double)(_y - BOUND_SCALE) / (BOUND_SCALE + CELL_SCALE)));
                    //Initiate values using IDs
                    _view[_x, _y] = SetValType(0, _xID > CELL_SCALE || _yID > CELL_SCALE);
                    _view[_x, _y] = SetBound(0, (_xID > CELL_SCALE && _yID > CELL_SCALE) || (_x == 0 || _y == 0));
                }
            }

            //Write cells into view
            for (int _x = BOUND_SCALE; _x < _viewX; _x += BOUND_SCALE + CELL_SCALE)
            {
                for (int _y = BOUND_SCALE; _y < _viewY; _y += BOUND_SCALE + CELL_SCALE)
                {
                    //Write interior
                    _view[_x, _y] = SetAscending(_view[_x, _y], GetBit((int)worldBounds.GetValue(_toWrite), _dimensions[Z]) == 0);
                    if (_toWrite[_dimensions[Z]] != 0)
                    {
                        //Get if _toWrite has path form neighbor in negative Z dimension
                        _toWrite[_dimensions[Z]]--;
                        _view[_x, _y] = SetDescending(_view[_x, _y], GetBit((int)worldBounds.GetValue(_toWrite), _dimensions[Z]) == 0);
                        _toWrite[_dimensions[Z]]++;
                    }
                    else
                    {
                        _view[_x, _y] = SetDescending(_view[_x, _y], false);
                    }

                    //Write bound on X
                    _view[_x + CELL_SCALE, _y] = SetBound(_view[_x, _y], GetBit((int)worldBounds.GetValue(_toWrite), _dimensions[X]) == 1);

                    //Write bound on Y
                    _view[_x, _y + CELL_SCALE] = SetBound(_view[_x, _y], GetBit((int)worldBounds.GetValue(_toWrite), _dimensions[Y]) == 1);

                    //Increment Y value of _toWrite
                    _toWrite[_dimensions[Y]]++;
                }

                //Increment X value of _toWrite
                _toWrite[_dimensions[X]]++;
            }

            //Open all outside walls of entrance and exit cells if cells are viewable
            //Check entrance
            if (IsViewable(entranceCoor, _currCell, _dimensions[X], _dimensions[Y]))
            {
                //Check and Open X
                _view[0, BOUND_SCALE + entranceCoor[_dimensions[Y]] * (BOUND_SCALE + CELL_SCALE)] =
                    SetBound(_view[0, BOUND_SCALE + entranceCoor[_dimensions[Y]] * (BOUND_SCALE + CELL_SCALE)], !(entranceCoor[_dimensions[X]] == 0));
                _view[_viewX, BOUND_SCALE + entranceCoor[_dimensions[Y]] * (BOUND_SCALE + CELL_SCALE)] =
                    SetBound(_view[_viewX, BOUND_SCALE + entranceCoor[_dimensions[Y]] * (BOUND_SCALE + CELL_SCALE)], !(entranceCoor[_dimensions[X]] == dimensionInfo[_dimensions[X]] - 1));
                //Check and Open Y
                _view[BOUND_SCALE + entranceCoor[_dimensions[X]] * (BOUND_SCALE + CELL_SCALE), 0] =
                    SetBound(_view[BOUND_SCALE + entranceCoor[_dimensions[X]] * (BOUND_SCALE + CELL_SCALE), 0], !(entranceCoor[_dimensions[Y]] == 0));
                _view[BOUND_SCALE + entranceCoor[_dimensions[X]] * (BOUND_SCALE + CELL_SCALE), _viewY] =
                    SetBound(_view[BOUND_SCALE + entranceCoor[_dimensions[X]] * (BOUND_SCALE + CELL_SCALE), _viewY], !(entranceCoor[_dimensions[Y]] == dimensionInfo[_dimensions[Y]] - 1));
            //Check and Open Z
                _view[BOUND_SCALE + entranceCoor[_dimensions[X]] * (BOUND_SCALE + CELL_SCALE), BOUND_SCALE + entranceCoor[_dimensions[Y]] * (BOUND_SCALE + CELL_SCALE)] =
                     SetDescending(_view[BOUND_SCALE + entranceCoor[_dimensions[X]] * (BOUND_SCALE + CELL_SCALE), BOUND_SCALE + entranceCoor[_dimensions[Y]] * (BOUND_SCALE + CELL_SCALE)],
                     (entranceCoor[_dimensions[Z]] == 0));
                _view[BOUND_SCALE + entranceCoor[_dimensions[X]] * (BOUND_SCALE + CELL_SCALE), BOUND_SCALE + entranceCoor[_dimensions[Y]] * (BOUND_SCALE + CELL_SCALE)] =
                     SetAscending(_view[BOUND_SCALE + entranceCoor[_dimensions[X]] * (BOUND_SCALE + CELL_SCALE), BOUND_SCALE + entranceCoor[_dimensions[Y]] * (BOUND_SCALE + CELL_SCALE)],
                     (entranceCoor[_dimensions[Z]] == dimensionInfo[_dimensions[Z]] - 1));
            }
            //Check exit
            if (IsViewable(exitCoor, _currCell, _dimensions[X], _dimensions[Y]))
            {
                //Check and Open X
                _view[0, BOUND_SCALE + exitCoor[_dimensions[Y]] * (BOUND_SCALE + CELL_SCALE)] =
                    SetBound(_view[0, BOUND_SCALE + exitCoor[_dimensions[Y]] * (BOUND_SCALE + CELL_SCALE)], !(exitCoor[_dimensions[X]] == 0));
                _view[_viewX, BOUND_SCALE + exitCoor[_dimensions[Y]] * (BOUND_SCALE + CELL_SCALE)] =
                    SetBound(_view[_viewX, BOUND_SCALE + exitCoor[_dimensions[Y]] * (BOUND_SCALE + CELL_SCALE)], !(exitCoor[_dimensions[X]] == dimensionInfo[_dimensions[X]] - 1));
                //Check and Open Y
                _view[BOUND_SCALE + exitCoor[_dimensions[X]] * (BOUND_SCALE + CELL_SCALE), 0] =
                    SetBound(_view[BOUND_SCALE + exitCoor[_dimensions[X]] * (BOUND_SCALE + CELL_SCALE), 0], !(exitCoor[_dimensions[Y]] == 0));
                _view[BOUND_SCALE + exitCoor[_dimensions[X]] * (BOUND_SCALE + CELL_SCALE), _viewY] =
                    SetBound(_view[BOUND_SCALE + exitCoor[_dimensions[X]] * (BOUND_SCALE + CELL_SCALE), _viewY], !(exitCoor[_dimensions[Y]] == dimensionInfo[_dimensions[Y]] - 1));
                //Check and Open Z
                _view[BOUND_SCALE + exitCoor[_dimensions[X]] * (BOUND_SCALE + CELL_SCALE), BOUND_SCALE + exitCoor[_dimensions[Y]] * (BOUND_SCALE + CELL_SCALE)] =
                     SetDescending(_view[BOUND_SCALE + exitCoor[_dimensions[X]] * (BOUND_SCALE + CELL_SCALE), BOUND_SCALE + exitCoor[_dimensions[Y]] * (BOUND_SCALE + CELL_SCALE)],
                     (exitCoor[_dimensions[Z]] == 0));
                _view[BOUND_SCALE + exitCoor[_dimensions[X]] * (BOUND_SCALE + CELL_SCALE), BOUND_SCALE + exitCoor[_dimensions[Y]] * (BOUND_SCALE + CELL_SCALE)] =
                     SetAscending(_view[BOUND_SCALE + exitCoor[_dimensions[X]] * (BOUND_SCALE + CELL_SCALE), BOUND_SCALE + exitCoor[_dimensions[Y]] * (BOUND_SCALE + CELL_SCALE)],
                     (exitCoor[_dimensions[Z]] == dimensionInfo[_dimensions[Z]] - 1));
            }


            //All corners will be set as closed bounds due to opening positioning
            /*//Check corners and set bounds appropriatly
            for (int _x = 0; _x < _viewX; _x += BOUND_SCALE + CELL_SCALE)
            {
                for (int _y = 0; _y < _viewY; _y += BOUND_SCALE + CELL_SCALE)
                {
                    //Look at surrounding cells to decide if corner is filled
                    bool _isFilled = (_x == 0 ? true : ClosedBound(_view[_x - 1, _y]) &&
                        _x == dimensionInfo[_dimensions[X]] - 1 ? true : ClosedBound(_view[_x + BOUND_SCALE, _y])) ||
                        (_y == 0 ? true : ClosedBound(_view[_x, _y - 1]) &&
                        _y == dimensionInfo[_dimensions[Y]] - 1 ? true : ClosedBound(_view[_x, _y + BOUND_SCALE]));

                    //If is filled, set corner value to represent a closed bound
                    _view[_x, _y] = SetBound(_view[_x, _y], _isFilled);
                }
            }*/

            //Return view array
            return _view;
        }

        /// <summary>
        /// Changes a value to represent a bound if _bound is true,
        /// otherwise changes value to represent a cell interior
        /// </summary>
        /// <param name="_val">Value to set to represent a type</param>
        /// <param name="_isBound">If value is to be set to a bound</param>
        /// <returns>A value representing a bound (if true) or interior (if false)</returns>
        static protected int SetValType(int _val, bool _isBound)
        {
            return SetBitTo(_val, 0, _isBound ? 1 : 0);
        }

        /// <summary>
        /// Checks and returns ifa value represents a bound
        /// </summary>
        /// <param name="_val">Value to check</param>
        /// <returns>True if value represents a bound</returns>
        static public bool IsBound(int _val)
        {
            return GetBit(_val, 0) == 1;
        }

        /// <summary>
        /// Changes a given value to reflect that it represents an open or closed bound
        /// </summary>
        /// <param name="_val">Given value</param>
        /// <param name="_open">Represents if bound is to be opened (0) or closed (1)</param>
        /// <returns>Value reflecting bound</returns>
        static protected int SetBound(int _val, bool _open)
        {
            return IsBound(_val) ? SetBitTo(_val, 1, _open ? 0 : 1) : _val;
        }

        /// <summary>
        /// Checks if given value represents a closed bound
        /// </summary>
        /// <param name="_val">Given value</param>
        /// <returns>True if represents a closed bound</returns>
        static protected bool ClosedBound(int _val)
        {
            return IsBound(_val) && GetBit(_val, 1) == 1;
        }

        /// <summary>
        /// Checks if given value represents a open bound
        /// </summary>
        /// <param name="_val">Given value</param>
        /// <returns>True if represents a open bound</returns>
        static protected bool OpenBound(int _val)
        {
            return IsBound(_val) && GetBit(_val, 1) == 0;
        }

        /// <summary>
        /// Checks and returns ifa value represents a cell interior
        /// </summary>
        /// <param name="_val">Value to check</param>
        /// <returns>True if value represents a cell interior</returns>
        static public bool IsInterior(int _val)
        {
            return GetBit(_val, 0) == 0;
        }

        /// <summary>
        /// Changes a given value to reflect that the cell is ascending or not
        /// </summary>
        /// <param name="_val">Given value</param>
        /// <param name="_open">Represents if cell is ascending (1) or not (0)</param>
        /// <returns>Value reflecting cell interior</returns>
        static protected int SetAscending(int _val, bool _isAscending)
        {
            return IsInterior(_val) ? SetBitTo(_val, 1, _isAscending ? 1 : 0) : _val;
        }

        /// <summary>
        /// Changes a given value to reflect that the cell is descending or not
        /// </summary>
        /// <param name="_val">Given value</param>
        /// <param name="_open">Represents if cell is descending (1) or not (0)</param>
        /// <returns>Value reflecting cell interior</returns>
        static protected int SetDescending(int _val, bool _isDescending)
        {
            return IsInterior(_val) ? SetBitTo(_val, 2, _isDescending ? 1 : 0) : _val;
        }

        /// <summary>
        /// Checks if given value represents an ascending cell
        /// </summary>
        /// <param name="_val">Given value</param>
        /// <returns>True if represents an ascending cell</returns>
        static protected bool Ascending(int _val)
        {
            return IsInterior(_val) && GetBit(_val, 1) == 1;
        }

        /// <summary>
        /// Checks if given value represents a descending cell
        /// </summary>
        /// <param name="_val">Given value</param>
        /// <returns>True if represents a descending cell</returns>
        static protected bool Descending(int _val)
        {
            return IsInterior(_val) && GetBit(_val, 2) == 1;
        }
        
        /// <summary>
        /// Calculates and returns information about bounds from a value and other information
        /// Will return a 2D array of integers that contains n objects and four pieces of information for each object
        /// [cX, cY, w, d]
        /// cX - the x value on which the object is centered
        /// cY - the y value on which the object is centered
        /// w - the width (scale on x-axis) of the object
        /// d - the depth (scale on y-axis) of the object
        /// VALUES ENTERED AND RETURNED SHOULD BE IN TERMS OF PLAYER SCALE
        /// </summary>
        /// <param name="_viewCoor">The coordinates of the value in the viewable grid (as of right now should only be two)</param>
        /// <param name="_currCell">A current cell displayed in the veiwable grid</param>
        /// <param name="_currD">The current dimensions the viewable grid is showing</param>
        /// <param name="_val">The value</param>
        /// <param name="_cScale">The desired scale of the cell interior</param>
        /// <param name="_bScale">The desired scale of bounds</param>
        /// <param name="_oScale">The desired scale of opening</param>
        /// <returns>A 2D array with information about the bound represented by the given value, if value given does not represent a bound, an empty 2D array</returns>
        static public int[,] GetBoundInfo(int[] _viewCoor, int[] _currCell, int[] _currD, int _val, int _cScale, int _bScale, int _oScale)
        {
            //If not a bound value, return an empty array
            if (!IsBound(_val))
                return new int[0, 0];

            int[,] _bInfo;

            //Find and fill in information
            //Get upper left corner coordinate of cell in drawing
            int[] _cellULCoor = new int[2];
            _cellULCoor[X] = _bScale + (((_viewCoor[X] - BOUND_SCALE) / (BOUND_SCALE * CELL_SCALE)) * (_bScale + _cScale));
            _cellULCoor[Y] = _bScale + (((_viewCoor[Y] - BOUND_SCALE) / (BOUND_SCALE * CELL_SCALE)) * (_bScale + _cScale));
            //If closed bound, one set of information, else two sets of information
            if (ClosedBound(_val))
            {
                _bInfo = new int[1, 4];
                //Check if on X
                if (((_viewCoor[X] - BOUND_SCALE) % (BOUND_SCALE * CELL_SCALE)) >= CELL_SCALE)
                {
                    _bInfo[0, X] = _cScale + (_bScale / 2) + _cellULCoor[X];
                    _bInfo[0, 2] = _bScale;
                }
                else
                {
                    _bInfo[0, X] = (_cScale / 2) + _cellULCoor[X];
                    _bInfo[0, 2] = _cScale;
                }
                //Check if on Y
                if (((_viewCoor[Y] - BOUND_SCALE) % (BOUND_SCALE * CELL_SCALE)) >= CELL_SCALE)
                {
                    _bInfo[0, Y] = _cScale + (_bScale / 2) + _cellULCoor[Y];
                    _bInfo[0, 3] = _bScale;
                }
                else
                {
                    _bInfo[0, Y] = (_cScale / 2) + _cellULCoor[Y];
                    _bInfo[0, 3] = _cScale;
                }
            }
            else
            {
                _bInfo = new int[2, 4];
                int _centerCoor;
                //Must be on X or on Y, never a corner
                if (((_viewCoor[X] - BOUND_SCALE) % (BOUND_SCALE * CELL_SCALE)) >= CELL_SCALE)
                {
                    _bInfo[0, X] = _cScale + (_bScale / 2) + _cellULCoor[X];
                    _bInfo[0, 2] = _bScale;
                    _bInfo[1, X] = _cScale + (_bScale / 2) + _cellULCoor[X];
                    _bInfo[1, 2] = _bScale;

                    _centerCoor = FindOpeningCenter(X, true, _viewCoor, _currD, _currCell, _cScale, _bScale)[Y];
                    _bInfo[0, Y] = ((_centerCoor - (_bScale / 2)) / 2) + _cellULCoor[Y];
                    _bInfo[0, 3] = _centerCoor - (_bScale / 2);
                    _bInfo[1, Y] = ((_cScale + _centerCoor + (_bScale / 2)) / 2) + _cellULCoor[Y];
                    _bInfo[1, 3] = _cScale - _centerCoor - (_bScale / 2);
                }
                else
                {
                    _bInfo[0, Y] = _cScale + (_bScale / 2) + _cellULCoor[Y];
                    _bInfo[0, 3] = _bScale;
                    _bInfo[1, Y] = _cScale + (_bScale / 2) + _cellULCoor[Y];
                    _bInfo[1, 3] = _bScale;

                    _centerCoor = FindOpeningCenter(Y, true, _viewCoor, _currD, _currCell, _cScale, _bScale)[X];
                    _bInfo[0, X] = ((_centerCoor - (_bScale / 2)) / 2) + _cellULCoor[X];
                    _bInfo[0, 2] = _centerCoor - (_bScale / 2);
                    _bInfo[1, X] = ((_cScale + _centerCoor + (_bScale / 2)) / 2) + _cellULCoor[X];
                    _bInfo[1, 2] = _cScale - _centerCoor - (_bScale / 2);
                }
            }            

            //Return information
            return _bInfo;
        }

        /// <summary>
        /// Calculates and returns information about a cell interior from a value and other information
        /// Will return a 2D array of integers that contains n objects and four pieces of information for each object
        /// [cX, cY, w, d]
        /// cX - the x value on which the object is centered (-1 if object is not in interior)
        /// cY - the y value on which the object is centered (-1 if object is not in interior)
        /// w - the width (scale on x-axis) of the object (-1 if object is not in interior)
        /// d - the depth (scale on y-axis) of the object (-1 if object is not in interior)
        /// 0 - opening ascending
        /// 1 - opening descending
        /// VALUES ENTERED AND RETURNED SHOULD BE IN TERMS OF PLAYER SCALE
        /// </summary>
        /// <param name="_viewCoor">The coordinates of the value in the viewable grid (as of right now should only be two)</param>
        /// <param name="_currCell">A current cell displayed in the veiwable grid</param>
        /// <param name="_currD">The current dimensions the viewable grid is showing</param>
        /// <param name="_val">The value</param>
        /// <param name="_cScale">The desired scale of the cell interior</param>
        /// <param name="_bScale">The desired scale of bounds</param>
        /// <param name="_oScale">The desired scale of opening</param>
        /// <returns>A 2D array with information about the interior represented by the given value, if value given does not represent cell interior, an empty 2D array</returns>
        static public int[,] GetInteriorInfo(int[] _viewCoor, int[] _currCell, int[] _currD, int _val, int _cScale, int _bScale, int _oScale)
        {
            //If not a bound value, return an empty array
            if (!IsInterior(_val))
                return new int[0, 0];

            //Create iInfo
            int[,] _iInfo = new int[2, 4];

            //Find and fill in information
            //Get upper left corner coordinate of cell in drawing
            int[] _cellULCoor = new int[2];
            _cellULCoor[X] = _bScale + (((_viewCoor[X] - BOUND_SCALE) / (BOUND_SCALE * CELL_SCALE)) * (_bScale + _cScale));
            _cellULCoor[Y] = _bScale + (((_viewCoor[Y] - BOUND_SCALE) / (BOUND_SCALE * CELL_SCALE)) * (_bScale + _cScale));
            //If ascending
            if (Ascending(_val))
            {
                int[] _openingC = FindOpeningCenter(Z, true, _viewCoor, _currD, _currCell, _cScale, _bScale);
                _iInfo[0, X] = _openingC[X] + _cellULCoor[X];
                _iInfo[0, Y] = _openingC[Y] + _cellULCoor[Y];
                _iInfo[0, 2] = _bScale;
                _iInfo[0, 3] = _bScale;
            }
            //If descending
            if (Descending(_val))
            {
                int[] _openingC = FindOpeningCenter(Z, false, _viewCoor, _currD, _currCell, _cScale, _bScale);
                _iInfo[0, X] = _openingC[X] + _cellULCoor[X];
                _iInfo[0, Y] = _openingC[Y] + _cellULCoor[Y];
                _iInfo[0, 2] = _bScale;
                _iInfo[0, 3] = _bScale;
            }

            //Return information
            return _iInfo;
        }

        /// <summary>
        /// Finds and returns the X and Y center points of an opening in a specified dimension in a cell
        /// </summary>
        /// <param name="_openingD">The X, Y, or Z dimension of the cell</param>
        /// <param name="_forwards">True if getting opening in forwards direction</param>
        /// <param name="_viewCoor">Coordinates in the viewable grid</param>
        /// <param name="_currD">Current dimensions</param>
        /// <param name="_currCell">The full coordinates of a currently viewable cell</param>
        /// <param name="_cScale">The scale of the cell interior</param>
        /// <param name="_bScale">The scale of bounds</param>
        /// <returns>The center point of an opening within its cell</returns>
        static protected int[] FindOpeningCenter(int _openingD, bool _forwards, int[] _viewCoor, int[] _currD, int[] _currCell, int _cScale, int _bScale)
        {
            //Creater center
            int[] _center = new int[2];

            //Find center
            //Get cell coordinates
            int[] _cellCoor = new int[3];
            _cellCoor[X] = (_viewCoor[X] - BOUND_SCALE) / (BOUND_SCALE * CELL_SCALE);
            _cellCoor[Y] = (_viewCoor[Y] - BOUND_SCALE) / (BOUND_SCALE * CELL_SCALE);
            _cellCoor[Z] = _currCell[_currD[Z]];
            if (!_forwards)
                _cellCoor[_openingD]--;

            //Get constants
            int[] _constants = new int[2];
            for (int _i = 0; _i < dimensions; _i++)
            {
                if (_i < _currD[_openingD])
                    if (_i == _currD[X])
                        _constants[X] += _cellCoor[X];
                    else
                        if (_i == _currD[Y])
                            _constants[X] += _cellCoor[Y];
                        else
                            _constants[X] += _currCell[_i];
                if (_i > _currD[_openingD])
                    if (_i == _currD[X])
                        _constants[Y] += _cellCoor[X];
                    else
                        if (_i == _currD[Y])
                            _constants[Y] += _cellCoor[Y];
                        else
                            _constants[Y] += _currCell[_i];
            }

            //Get section values
            int[] _sections = new int[2];
            _sections[X] = (_cellCoor[_openingD] - 1) % 4 == 0 ? 0 : (_cellCoor[_openingD] - 1) % 8 > 4 ? (((((_cellCoor[_openingD] - 1) % 8) + 1) % 2) + 1) : ((((_cellCoor[_openingD] - 1) % 8) % 2) + 1);
            _sections[Y] = (_cellCoor[_openingD] - 3) % 4 == 0 ? 0 : (_cellCoor[_openingD] - 3) % 8 > 4 ? (((((_cellCoor[_openingD] - 3) % 8) + 1) % 2) + 1) : ((((_cellCoor[_openingD] - 3) % 8) % 2) + 1);

            //Get center
            _center[X] = (int)(Math.Floor((double)((_constants[X] * _cellCoor[_openingD]) + _cellCoor[_openingD]) % Math.Floor((double)(_cScale / (_sections[X] == 0 ? 1 : 2) + 1 - _bScale)) + (_bScale / 2))
                + Math.Ceiling((double)(_sections[X] == 0 ? 0 : _sections[X] - 1) * _cScale / 2));
            _center[Y] = (int)(Math.Floor((double)((_constants[Y] * _cellCoor[_openingD]) + _cellCoor[_openingD]) % Math.Floor((double)(_cScale / (_sections[Y] == 0 ? 1 : 2) + 1 - _bScale)) + (_bScale / 2))
                + Math.Ceiling((double)(_sections[Y] == 0 ? 0 : _sections[Y] - 1) * _cScale / 2));

            //Return center
            return _center;
        }

        /// <summary>
        /// Checks if the cell at the specified coordinates is viewable in the current 2D layer
        /// </summary>
        /// <param name="_toCheck">Cell to check</param>
        /// <param name="_currCell">Current cell in layer</param>
        /// <param name="_x">X dimension of layer</param>
        /// <param name="_y">Y dimension of layer</param>
        /// <returns>If cell is viewable in layer</returns>
        protected bool IsViewable(int[] _toCheck, int[] _currCell, int _x, int _y)
        {
            for (int _d = 0; _d < dimensions; _d++)
                if (_d != _x && _d != _y && _toCheck[_d] != _currCell[_d])
                    return false;
            return true;
        }

        /*/// <summary>
        /// Creates and returns a player to run the maze
        /// </summary>
        /// <param name="_texture">The player's texture</param>
        /// <param name="_maxSpeed">The player's maximum allowed speed</param>
        /// <param name="_tileSize">The tile size in the maze</param>
        /// <returns>A Player object to run the maze</returns>
        public Player GetPlayer(Texture2D _texture, int _frames, int _modes, int _maxSpeed, int _tileSize)
        {
            return new Player(_texture, _frames, _modes, _maxSpeed, cellScale, _tileSize, dimensionInfo, entranceD, entranceCoor);
        }

        /// <summary>
        /// Creates and returns a world to display the maze
        /// </summary>
        /// <param name="_fPosition">The prefered positoin for the world</param>
        /// <param name="_tileTextures">Tile textures for the world</param>
        /// <param name="_cCell">The cell the world is initally centered on.</param>
        /// <returns>A World object to display the maze</returns>
        public World GetWorld(Vector2 _fPosition, Texture2D[] _tileTextures, int[] _cCell, int[] _cTile, int[] _cDimensions)
        {
            return new World(_fPosition, _tileTextures, _cCell, _cTile, cellScale, GetViewable(_cDimensions, _cCell), _cDimensions);
        }*/

        /// <summary>
        /// Gets a specified bit from a value
        /// </summary>
        /// <param name="_val">Value to get bit from</param>
        /// <param name="_n">Position of specified bit</param>
        /// <returns>Value of bit</returns>
        static internal int GetBit(int _val, int _n)
        {
            return ((_val >> _n) & 1);
        }

        /// <summary>
        /// Flips a specified bit in a value
        /// </summary>
        /// <param name="_val">Value to flip bit in</param>
        /// <param name="_n">Position of bit to be flipped</param>
        /// <returns>New value with bit flipped</returns>
        static protected int FlipBit(int _val, int _n)
        {
            return _val + (GetBit(_val, _n) > 0 ? -(1 << _n) : (1 << _n));
        }

        /// <summary>
        /// Sets a specified bit in a value to specified value (1 or 0)
        /// </summary>
        /// <param name="_val">Value to set bit in</param>
        /// <param name="_n">Position of bit to be set</param>
        /// <param name="_bitVal">Value bit is to be set to</param>
        /// <returns></returns>
        static protected int SetBitTo(int _val, int _n, int _bitVal)
        {
            return (GetBit(_val, _n) == (_bitVal > 0 ? 1 : 0) ? _val : FlipBit(_val, _n));
        }

        /// <summary>
        /// An abstract class to define cells in the maze
        /// </summary>
        internal class Cell : IComparable<Cell>
        {
            internal Cell parent;
            internal List<Cell> childern;
            protected Cell[] neighbors;
            protected int[] coordinates;
            protected ushort bounds;

            /// <summary>
            /// Constructor
            /// Intializes variables, Gets a random type, and creates a new set for the cell
            /// </summary>
            /// <param name="_coor">Coordinates of cell in maze</param>
            internal Cell(int[] _coor)
            {
                childern = new List<Cell>();
                neighbors = new Cell[dimensions];
                neighbors.DefaultIfEmpty(null);
                coordinates = _coor;
                InitiateStorage();
                NewSet();
            }

            /// <summary>
            /// Initiates storage for cell data
            /// </summary>
            protected void InitiateStorage()
            {
                bounds = 0;
            }

            /// <summary>
            /// Creates and returns a new instance of a Cell
            /// </summary>
            /// <param name="_coor">Coordinates of new instance</param>
            /// <returns>New instance of a class extending Cell</returns>
            protected Cell CreateNewCell(int[] _coor)
            {
                return new Cell(_coor);
            }

            /// <summary>
            /// Releases resources of cell
            /// Also removes cell from its set while not destroying its actual set.
            /// </summary>
            protected virtual void Close()
            {
                //Remove cell from set
                RemoveFromSet();
            }

            /// <summary>
            /// Get's the neighboring cell in the specified dimension
            /// Creates the cell if necessary
            /// Will expand maze in dimensions higher than specified dimension as well
            /// If specified dimension is 0, it is assumed the last dimension has been reached
            ///     and will write all previous cells to world arrays and close the cells.
            /// </summary> 
            /// <param name="_d">Dimension</param>
            /// <returns>Neighboring cell</returns>
            internal Cell GetNeighbor(int _d)
            {
                if (neighbors[_d] == null)
                {
                    //Get neighbor coordinates
                    int[] _coor = new int[dimensions];
                    coordinates.CopyTo(_coor, 0);
                    _coor[_d]++;

                    //Create neighbor
                    neighbors[_d] = CreateNewCell(_coor);

                    /*//Define path for current cell in dimension
                    if ((_d == 0) && PathToLastDimension(coordinates[0]))
                    {
                        ForceOpenBound(_d);
                    }
                    else
                    {
                        DefineBound(_d);
                    }*/

                    //Define bound for current cell in dimension
                    DefineBound(_d);

                    //Get neighboring cells of neighbor
                    //for (int _n = (_d + 1); _n < neighbors.Length; _n++)
                    for (int _n = neighbors.Length - 1; _n > _d; _n--)
                    {
                        if (HasNeighbor(_n))
                        {
                            //Get neighbors[_n].neighbors[_d] and set as neighbors[_d].neighbors[_n]
                            neighbors[_d].neighbors[_n] = neighbors[_n].GetNeighbor(_d);

                            //Define path in dimension _n for neighbors[_d]
                            neighbors[_d].DefineBound(_n);
                        }
                    }
                    //If _d is 0, last dimension expansion. Write current neighbor cells to bitWorld array in BitmapMaze.
                    //  Close written cells.
                    if (_d == 0)
                    {
                        //Write path information to bitWorld and close cell
                        WriteToMaze();
                    }
                }
                return neighbors[_d];
            }

            /*/// <summary>
            /// Returns if a cell should have a path into the current layer of last dimension or not.
            /// </summary>
            /// <param name="_currLayer">Integer specifing the current layer</param>
            /// <returns>True if set is to have a path into the current layer of the next dimension</returns>
            protected bool PathToLastDimension(int _currLayer)
            {
                //Get the first (and greatest) cell in the set
                Cell _setParent = this;
                while (_setParent.parent != null)
                {
                    _setParent = _setParent.parent;
                }

                return (Randomize.RandBool() || ((CompareTo(_setParent) == 0) && !(_setParent.coordinates[0] == _currLayer)));
            }*/

            /// <summary>
            /// Writes the cell's path information to the actual maze arrays
            /// </summary>
            protected virtual void WriteToMaze()
            {
                worldBounds.SetValue(bounds, coordinates);

                //Close cell when done writing it
                Close();
            }

            /// <summary>
            /// For an entire set, recursivly writes cells' path information to the actual maze arrays
            /// Then closes the cells
            /// </summary>
            internal void WriteSetToMaze()
            {
                while (childern.Count > 0)
                {
                    childern.ElementAt(0).WriteSetToMaze();
                }
                WriteToMaze();
            }

            /// <summary>
            /// Sets the cell as the first cell of a new type
            /// </summary>
            protected void NewSet()
            {
                parent = null;
                if (!sets.Contains(this))
                {
                    sets.Add(this);
                }
            }

            /// <summary>
            /// Removes cell's parent
            /// </summary>
            internal void RemoveParent()
            {
                if (parent == null)
                {
                    return;
                }
                parent.childern.Remove(this);
                parent = null;
                if (!sets.Contains(this))
                {
                    sets.Add(this);
                }
            }

            /// <summary>
            /// Add a new child cell
            /// </summary>
            /// <param name="_c">New Child cell</param>
            protected void AddChild(Cell _c)
            {
                //Check to ensure cell does not parent itself
                if (this == _c)
                {
                    return;
                }
                sets.Remove(_c);
                childern.Add(_c);
                _c.parent = this;
            }

            /// <summary>
            /// Checks if cells are part of the same set
            /// </summary>
            /// <param name="_c">Cell to check</param>
            /// <returns>If cells are part of the same set</returns>
            protected bool SameSet(Cell _c)
            {
                return (GetFirstInSet() == _c.GetFirstInSet());
            }

            /// <summary>
            /// Checks if current cell and its neighbor in the specified dimension are part of the same set
            /// Returns false if cell has no neighbor in specifed dimension
            /// </summary>
            /// <param name="_d">Specified dimension</param>
            /// <returns>If current cell and its neighbor (if it exists) are part of the same set</returns>
            protected bool SameSet(int _d)
            {
                //Check if neighbor exsists
                if (neighbors[_d] == null)
                {
                    return false;
                }
                return (GetFirstInSet() == neighbors[_d].GetFirstInSet());
            }

            /// <summary>
            /// Adds the cell to a specified set
            /// </summary>
            /// <param name="_newSet">New set to be added to</param>
            protected void AddToSet(Cell _newSet)
            {
                //Check if same set
                if (SameSet(_newSet))
                {
                    return;
                }

                //Get first cell in current cell's set
                Cell _first = GetFirstInSet();

                //While _first is not part of new set, add cells from current set to new set.
                while (!_first.SameSet(_newSet))
                {
                    Cell _toAdd = _first.GetLastInSet();

                    _newSet.AddCellToSet(_toAdd);

                    _newSet.GetFirstInSet().SortSet();
                }
            }

            /// <summary>
            /// Adds a single cell to the current set if the cell does not already belong to the set
            /// </summary>
            /// <param name="_c"></param>
            protected void AddCellToSet(Cell _c)
            {
                //Check if same set
                if (SameSet(_c))
                {
                    return;
                }

                _c.RemoveFromSet();
                GetCellToAddChildTo().AddChild(_c);
            }

            /// <summary>
            /// Starting at the start of the set, gets the next cell in a set that can recieve a child cell
            /// </summary>
            /// <returns>The next cell that can recieve a child cell</returns>
            protected Cell GetCellToAddChildTo()
            {
                //Get the next child cell with room for childern
                Cell _current, _hold;
                _current = GetFirstInSet();
                _hold = GetFirstInSet();

                while (_current.childern.Count >= dimensions)
                {
                    List<Cell> _notFull = _current.childern.FindAll(x => (x.childern.Count < dimensions));
                    _notFull.Sort();
                    if (_notFull.Count == 0)
                    {
                        _current = _current.childern.Min();
                    }
                    else
                    {
                        _notFull.Sort();
                        _notFull.Sort((x, y) => -(x.childern.Count - y.childern.Count));
                        _current = _notFull.ElementAt(0);
                    }
                }
                return _current;
            }

            /// <summary>
            /// Starting at the start of the set, gets the cell with the least number of siblings and no childern
            /// </summary>
            /// <returns>The cell with the least number of sibling and no childern</returns>
            protected Cell GetLastInSet()
            {
                Cell _last;
                _last = GetFirstInSet();

                //Gets the cell with no childern and the least number of siblings
                while (_last.childern.Count > 0)
                {
                    //List<Cell> _notFull = _current.childern.FindAll(x => (x.childern.Count < dimensions && x.childern.Count > 0));
                    List<Cell> _notEmpty = _last.childern.FindAll(x => (x.childern.Count != 0));
                    List<Cell> _empty = _last.childern.FindAll(x => (x.childern.Count == 0));

                    //Console.WriteLine("Looping in GetLastinSet " + ToString() + " current: " + _current.ToString());
                    if (_notEmpty.Count != 0)
                    {
                        _notEmpty.Sort((x, y) => x.childern.Count - y.childern.Count);
                        _last = _notEmpty.ElementAt(0);
                    }
                    else
                    {
                        _last = _empty.ElementAt(0);
                    }

                }
                return _last;
            }

            /// <summary>
            /// Removes self from parent or sets if has no childern
            /// </summary>
            protected void NullSet()
            {
                foreach (Cell _child in childern.ToArray())
                {
                    _child.NullParent();
                }
                childern.Clear();
                NullParent();
            }

            /// <summary>
            /// Removes self from parent without creating itself a new set
            /// </summary>
            protected void NullParent()
            {
                sets.Remove(this);
                if (parent == null)
                {
                    return;
                }
                parent.childern.Remove(this);
                parent = null;
            }

            /// <summary>
            /// Removes cell from its set
            /// </summary>
            protected void RemoveFromSet()
            {
                //If no parent or childern, simply remove cell from sets
                if ((parent == null) && (childern.Count == 0))
                {
                    if (sets.Contains(this))
                    {
                        sets.Remove(this);
                    }
                    return;
                }

                //Replace cell with last cell in set, removing current cell's childern and parent
                //Then sort set
                ReplaceWithLast().SortSet();
            }

            /// <summary>
            /// Rebalances the max heap sorted set of cells recursively
            /// For after a set is edited.
            /// </summary>
            protected void RebalanceSet()
            {
                //Return if cell has no chilern
                if (childern.Count == 0)
                {
                    return;
                }

                //If cell has less childern than it should
                if (childern.Count < dimensions)
                {
                    Cell _current = childern.Max();

                    while ((childern.Count < dimensions) && (_current.childern.Count != 0))
                    {
                        Cell _holdChild = _current.childern.Max();
                        _holdChild.RemoveParent();
                        AddChild(_holdChild);
                    }
                    _current.RebalanceSet();
                }

                //If cell has more childern than it should
                if (childern.Count > dimensions)
                {
                    //List<Cell> _hold = new List<Cell>();

                    while (childern.Count > dimensions)
                    {
                        Cell _holdChild = childern.Min();
                        _holdChild.RemoveParent();
                        childern.Min().AddChild(_holdChild);
                    }
                }
            }

            /// <summary>
            /// Max heapsorts the set starting from the current node
            /// </summary>
            /// <returns>True if current cell is switched</returns>
            protected bool SortSet()
            {

                //If no childern, return false
                if (childern.Count == 0)
                {
                    return false;
                }

                bool _change = false, _childChange = false;

                _change = SwitchWithMaxChild();

                for (int _c = 0; _c < childern.Count; _c++)
                {
                    _childChange |= childern.ElementAt(_c).SortSet();
                }

                if (_childChange)
                {
                    _change = SwitchWithMaxChild();
                }

                return _change;
            }

            /// <summary>
            /// Switches a cell in a set with its max child if its max child is larger than it.
            /// </summary>
            /// <returns>True if switch is made.</returns>
            protected bool SwitchWithMaxChild()
            {
                //Return if no childern
                if (childern.Count == 0)
                {
                    return false;
                }

                //Return if larger than max child
                if (this.CompareTo(childern.Max()) > 0)
                {
                    return false;
                }

                //Create variables to hold childern and parent
                Cell[] _maxChildChildern, _childern;
                Cell _parent;

                //Hold current childern and parent
                _childern = childern.ToArray<Cell>();
                _parent = parent;

                //Hold max child
                Cell _maxChild = childern.Max();
                //  Hold max childern
                _maxChildChildern = _maxChild.childern.ToArray<Cell>();

                //Null current cell's set
                NullSet();

                //  Null max child's set
                _maxChild.NullSet();

                //Switch childern
                foreach (Cell _child in _maxChildChildern)
                {
                    AddChild(_child);
                }
                foreach (Cell _child in _childern)
                {
                    _maxChild.AddChild(_child);
                }

                //Add current as child of max child
                _maxChild.AddChild(this);

                //Assign max child to old parent, if any
                if (_parent == null)
                {
                    sets.Add(_maxChild);
                }
                else
                {
                    _parent.AddChild(_maxChild);
                }

                //Return true if switch is made
                return true;
            }

            /// <summary>
            /// Removes and replaces the current cell with the last cell in its set
            /// </summary>
            /// <returns>Start of old set</returns>
            protected Cell ReplaceWithLast()
            {
                Cell _last;

                _last = GetLastInSet();

                //Get start of set
                Cell _start = _last.GetFirstInSet();

                //Null set of last cell
                _last.NullSet();

                //Check if current is last
                if (this == _last)
                {
                    return _start;
                }

                //Store current cell's childern and parent
                Cell _parent = parent;
                Cell[] _childern = childern.ToArray<Cell>();

                //Nulls current cell's set
                NullSet();

                //If stored parent is not null, add last cell as a child, otherwise, add last cell to sets.
                if (_parent != null)
                {
                    _parent.AddChild(_last);
                }
                else
                {
                    sets.Add(_last);
                }

                //Add stored childern to last cell
                foreach (Cell _child in _childern)
                {
                    _last.AddChild(_child);
                }

                //Returns start of old set
                return _last.GetFirstInSet();
            }

            /// <summary>
            /// Gets the first cell in a set
            /// </summary>
            /// <returns>The first cell in a set</returns>
            protected Cell GetFirstInSet()
            {
                Cell _first = this;
                while (_first.parent != null)
                {
                    _first = _first.parent;
                }
                return _first;
            }

            /// <summary>
            /// Compares cell to specified cell and returns which is greater based on cell coodinates
            /// </summary>
            /// <param name="_c">Specified cell to compare to</param>
            /// <returns>-1 if cell is lesser than specified cell; 1 if cell is greater than specified cell; 0 if cells are equal</returns>
            public int CompareTo(Cell _c)
            {
                //Check to sort by coordinates (Decreasing to starting point)
                for (int _i = 0; _i < dimensions; _i++)
                {
                    if (coordinates[_i] < _c.coordinates[_i])
                    {
                        return -1;
                    }
                    if (coordinates[_i] > _c.coordinates[_i])
                    {
                        return 1;
                    }
                }

                //UNREACHABLE, NO CELLS HAVE IDENTICAL COORDINATES UNLESS THEY ARE THE SAME
                /*//Check to sort by number of childern (Most to least number of childern)
                if (childern.Count < _c.childern.Count)
                {
                    return -1;
                }
                if (childern.Count > _c.childern.Count)
                {
                    return 1;
                }*/

                //Cells are equal
                return 0;
            }

            /// <summary>
            /// BFS of set until the set successful merges with another set
            /// </summary>
            internal void MergeSet()
            {
                Cell _current;
                Queue<Cell> _queue = new Queue<Cell>();
                _queue.Enqueue(this);
                while (_queue.Count != 0)
                {
                    _current = _queue.Dequeue();

                    //Randomly decide to try to merge sets from current cell
                    if (Randomize.RandBool())
                    {
                        if (_current.RandMerge())
                        {
                            break;
                        }
                    }

                    //Add childern of current set to queue
                    foreach (Cell _child in _current.childern)
                    {
                        _queue.Enqueue(_child);
                    }
                }
            }

            /// <summary>
            /// If possible, randomly merges two neighboring sets
            /// </summary>
            /// <returns>If two sets were merged</returns>
            protected bool RandMerge()
            {
                //Get dimensions of neighbors not in same set
                int[] _notInSet = NeighborsNotInSet();
                int _d;

                //Check if any sets to merge, return if not
                if ((_notInSet.Length == 0))
                {
                    return false;
                }

                //Pick a random dimension to merge in
                _d = _notInSet[Randomize.RandInt(_notInSet.GetUpperBound(0))];

                //Force open bound
                ForceOpenBound(_d);

                return true;
            }

            /// <summary>
            /// Checks if cell has a neighbor in the specified dimension
            /// </summary>
            /// <param name="_d">Specified dimension</param>
            /// <returns>True or flase if cell has neighbor</returns>
            protected bool HasNeighbor(int _d)
            {
                return (neighbors[_d] != null);
            }

            /// <summary>
            /// Returns int array of dimensions that has neighbors not in its set
            /// </summary>
            /// <returns>Int array</returns>
            protected int[] NeighborsNotInSet()
            {
                int[] _notInSet = new int[0];
                for (int _d = 0; _d < neighbors.Length; _d++)
                {
                    if (!SameSet(_d))
                    {
                        Array.Resize(ref _notInSet, (_notInSet.Length + 1));
                        _notInSet[_notInSet.GetUpperBound(0)] = _d;
                    }
                }
                return _notInSet;
            }

            /// <summary>
            /// Adds neighboring cell in a specified dimension to the current cell's set should the neigher exist and is not already part of set
            /// </summary>
            /// <param name="_d">Specified dimension</param>
            protected void AddNeighborToSet(int _d)
            {
                //Check if neighbor exists
                if (!HasNeighbor(_d))
                {
                    return;
                }

                //Check if same set
                if (SameSet(neighbors[_d]))
                {
                    return;
                }

                //Add neighbor to set
                neighbors[_d].AddToSet(this);
            }

            /// <summary>
            /// Removes a neighboring cell in a specified dimension from the current cell's set should the neigher exist and is part of set
            /// </summary>
            /// <param name="_d">Specified dimension</param>
            protected void RemoveNeighborFromSet(int _d)
            {
                //Check if neighbor exists
                if (!HasNeighbor(_d))
                {
                    return;
                }

                //Check if same set
                if (!SameSet(neighbors[_d]))
                {
                    return;
                }

                //Add neighbor to set
                neighbors[_d].RemoveFromSet();
            }

            /// <summary>
            /// Defines the bounds in a specified dimension of a cell
            /// </summary>
            /// <param name="_d">Specified dimension</param>
            protected void DefineBound(int _d)
            {
                //Find bit value
                //Randomize
                int _b = Randomize.RandInt(1);
                //If neighbor in set or end of dimension, close
                if (SameSet(_d) || EndOfDimension(_d))
                    _b = 1;
                //If must extend set, open
                if (MustExtendSet(_d))
                    _b = 0;

                //Define bounds
                bounds = (ushort)SetBitTo(bounds, _d, _b);

                //If open, add neighbor to set
                if (Open(_d))
                {
                    AddNeighborToSet(_d);
                }
            }

            /// <summary>
            /// Opens the bounds in a specified dimension of a cell
            /// </summary>
            /// <param name="_d">Specified dimension</param>
            protected void ForceOpenBound(int _d)
            {
                bounds = (ushort)SetBitTo(bounds, _d, 0);
                //If open, add neighbor to set
                if (Open(_d))
                {
                    AddNeighborToSet(_d);
                }
            }

           /* /// <summary>
            /// Closes the bounds in a specified dimension of a cell
            /// </summary>
            /// <param name="_d">Specified dimension</param>
            protected void ForceCloseBound(int _d)
            {
                bounds = (ushort)SetBitTo(bounds, _d, 1);
                //If not open, remove neighbor from set
                if (!Open(_d))
                {
                    RemoveNeighborToSet(_d);
                }
            }*/

            /// <summary>
            /// Returns if a bound in a specified dimension is open
            /// </summary>
            /// <param name="_d">Specified dimension</param>
            /// <returns>True if bound is open</returns>
            protected bool Open(int _d)
            {
                return GetBit(bounds, _d) == 0;
            }

            /// <summary>
            /// Checks if a set must extend in a specified dimension from the current cell
            /// </summary>
            /// <param name="_d">Specified dimension</param>
            /// <returns>True if set must extend</returns>
            protected bool MustExtendSet(int _d)
            {
                //If last dimension, set parent, and not end of dimension, must extend set
                return  _d == 0 && parent == null && !EndOfDimension(_d);  
            }

            /// <summary>
            /// Checks if cell is at end of dimension
            /// </summary>
            /// <param name="_d">Dimension</param>
            /// <returns>If cell is at end of dimension</returns>
            protected bool EndOfDimension(int _d)
            {
                return coordinates[_d] == dimensionInfo[_d] - 1;
            }

            /// <summary>
            /// Gets a string representation of the cell
            /// </summary>
            /// <returns>String representation of the cell</returns>
            public override String ToString()
            {
                String _s = "{";
                int _i = 0;
                while (_i < coordinates.Length - 1)
                {
                    _s += coordinates[_i] + ", ";
                    _i++;
                }
                _s += coordinates[_i] + "}";
                return _s;
            }

            /// <summary>
            /// Gets a string representation of the cell, its neighbors, parent, and childern
            /// </summary>
            /// <returns>String representation of the cell</returns>
            protected String InfoToString()
            {
                String _s = this + "\n   Neighbors: ";
                int _i;
                _i = 0;
                while (_i < neighbors.Length - 1)
                {
                    _s += (neighbors[_i] != null ? neighbors[_i].ToString() : "null") + ", ";
                    _i++;
                }
                _s += (neighbors[_i] != null ? neighbors[_i].ToString() : "null") + "\n   Parent: " +
                      (parent != null ? parent.ToString() : "null") + "\n   Childern: ";
                _i = 0;
                while (_i < childern.Count - 1)
                {
                    _s += childern.ElementAt(_i) + ", ";
                    _i++;
                }
                _s += childern.LastOrDefault();
                return _s;
            }

            /// <summary>
            /// Gets a string representation of the cell, and its childern recursively for the entire set
            /// </summary>
            /// <returns>String representation of the cell and its set</returns>
            protected String SetToString()
            {
                Cell _first = GetFirstInSet();
                char?[,] _coorInfo = _first.SetToCharArray();
                String s = "";
                for (int _y = 0; _y < _coorInfo.GetLength(1); _y++)
                {
                    for (int _x = 0; _x < _coorInfo.GetLength(0); _x++)
                    {
                        if (_coorInfo[_x, _y].HasValue)
                        {
                            s += _coorInfo[_x, _y];
                        }
                        else
                        {
                            s += ' ';
                        }
                    }
                    s += '\n';
                }
                return s;
            }

            /// <summary>
            /// Starts at the first cell in the set and prints the set to the console
            /// </summary>
            internal void PrintSet()
            {
                Cell _first = GetFirstInSet();
                char?[,] _coorInfo = _first.SetToCharArray();
                for (int _y = 0; _y < _coorInfo.GetLength(1); _y++)
                {
                    for (int _x = 0; _x < _coorInfo.GetLength(0); _x++)
                    {
                        if (_coorInfo[_x, _y].HasValue)
                        {
                            Console.Write(_coorInfo[_x, _y]);
                        }
                        else
                        {
                            Console.Write(' ');
                        }
                    }
                    Console.WriteLine();
                }
            }

            /// <summary>
            /// Creates an array of characters the described a set
            /// </summary>
            /// <returns>An array of chars that describes a set</returns>
            private char?[,] SetToCharArray()
            {
                char?[,] _setArray;
                char[] _coorChar = ToString().ToCharArray();

                if (childern.Count == 0)
                {
                    _setArray = new char?[1, ((_coorChar.Length / 3) + 2)];
                    _setArray[0, 0] = '-';
                    _setArray[0, ((_coorChar.Length / 3) + 1)] = '-';
                    for (int _n = 0; _n < (_coorChar.Length / 3); _n++)
                    {
                        _setArray[0, (_n + 1)] = _coorChar[(1 + (_n * 3))];
                    }
                }
                else
                {
                    childern.Sort();
                    char?[][,] _childArrays = new char?[childern.Count][,];
                    int _childArrayX = 0, _childArrayY = 0;
                    for (int _c = 0; _c < childern.Count; _c++)
                    {
                        _childArrays[_c] = childern.ElementAt(_c).SetToCharArray();
                        _childArrayX++;
                        if (_childArrays[_c].GetLength(1) > _childArrayY)
                        {
                            _childArrayY = _childArrays[_c].GetLength(1);
                        }
                    }
                    _childArrayX = (int)Math.Pow(dimensions, ((_childArrayY + 1) / (dimensions + 3)));

                    int _setXbase, _setYbase, _setCenter, _childPlaceMod;
                    _setXbase = 0;
                    _setYbase = ((_coorChar.Length / 3) + 3);
                    _setCenter = (_childArrayX / 2);
                    _childPlaceMod = (_childArrayX - (childern.Count * (int)Math.Pow(dimensions, ((_childArrayY + 1) / (dimensions + 3)) - 1))) / 2;
                    _setArray = new char?[_childArrayX, _setYbase + _childArrayY];

                    for (int _n = 0; _n < (_coorChar.Length / 3); _n++)
                    {
                        _setArray[_setCenter, (_n + 1)] = _coorChar[(1 + (_n * 3))];
                    }
                    _setArray[_setCenter, 0] = '-';
                    _setArray[_setCenter, ((_coorChar.Length / 3) + 1)] = '-';
                    _setArray[_setCenter, ((_coorChar.Length / 3) + 2)] = 'v';

                    for (int _c = 0; _c < childern.Count; _c++)
                    {
                        _setXbase = (_c * (int)Math.Pow(dimensions, ((_childArrayY + 1) / (dimensions + 3)) - 1)) + _childPlaceMod;
                        for (int _x = 0; _x < _childArrays[_c].GetLength(0); _x++)
                        {
                            for (int _y = 0; _y < _childArrays[_c].GetLength(1); _y++)
                            {
                                _setArray[_setXbase + _x, _setYbase + _y] = _childArrays[_c][_x, _y];
                            }
                        }
                    }
                }

                return _setArray;
            }
        }
    }
}
