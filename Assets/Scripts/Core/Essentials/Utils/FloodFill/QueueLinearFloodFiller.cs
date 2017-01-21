using System;
using System.Collections.Generic;

namespace FloodFill2
{
    /// <summary>
    /// Implements the QueueLinear flood fill algorithm using array-based cell manipulation.
    /// </summary>
    public class QueueLinearFloodFiller : AbstractFloodFiller
    {
        //Queue of floodfill ranges. We use our own class to increase performance.
        FloodFillRangeQueue ranges = new FloodFillRangeQueue();
        GridCell[] _cellData;

        public QueueLinearFloodFiller(int gridSizeX, int gridSizeY, GridCell[] cellData) : base(gridSizeX, gridSizeY)
        {
            _cellData = cellData;
        }

        /// <summary>
        /// Fills the specified point on the bitmap with the fill value.
        /// </summary>
        /// <param name="pt">The starting point for the fill.</param>
        public override FillData FloodFill(Vector2Int pt)
        {
            ranges = new FloodFillRangeQueue(((gridWidth+gridHeight)/2)*5);

            int x = pt.x; int y = pt.y;

            //***Do first call to floodfill.
            LinearFill(ref x, ref y);

            //***Call floodfill routine while floodfill ranges still exist on the queue
            while (ranges.Count > 0)
            {
                //**Get Next Range Off the Queue
                FloodFillRange range = ranges.Dequeue();

	            //**Check Above and Below Each cell in the Floodfill Range
                int upY=range.Y - 1;//so we can pass the y coord by ref
                int downY = range.Y + 1;
                int i = range.StartX;
                if (range.StartX > 0) i--;  //check the diagonals

                int downPxIdx = (gridWidth * (range.Y + 1)) + i;
                int upPxIdx = (gridWidth * (range.Y - 1)) + i;
                while (i <= range.EndX + 1 && i < gridWidth)
                {
                    //*Start Fill Upwards
                    //if we're not above the top of the grid and the cell above this one is free
                    if (range.Y > 0 && (!gridCells[upPxIdx]) && CheckCell(ref upPxIdx))
                        LinearFill(ref i, ref upY);

                    //*Start Fill Downwards
                    //if we're not below the bottom of the grid and the pixel below this one is free
                    if (range.Y < (gridHeight - 1) && (!gridCells[downPxIdx]) && CheckCell(ref downPxIdx))
                        LinearFill(ref i, ref downY);
                    downPxIdx++;
                    upPxIdx++;
                    i++;
                }
            }

            return new FillData(gridWidth, gridCells);
        }

       /// <summary>
       /// Finds the furthermost left and right boundaries of the fill area
       /// on a given y coordinate, starting from a given x coordinate, filling as it goes.
       /// Adds the resulting horizontal range to the queue of floodfill ranges,
       /// to be processed in the main loop.
       /// </summary>
       /// <param name="x">The x coordinate to start from.</param>
       /// <param name="y">The y coordinate to check at.</param>
       void LinearFill(ref int x, ref int y)
        {
            //***Find Left Edge of fill area
            int lFillLoc = x; //the location to check/fill on the left
            int idx = CoordsToIndex(ref x, ref y); //the index of the current location
            while (true)
            {
                //**fill with the fillValue
                gridCells[idx] = true;
                //**de-increment
                lFillLoc--;     //de-increment counter
                idx -= 1;//de-increment index
                //**exit loop if we're at edge of fill area
                if (lFillLoc <= 0 || (gridCells[idx]) || !CheckCell(ref idx))
                    break;

            }
            lFillLoc++;

            //***Find Right Edge of free area
            int rFillLoc = x; //the location to check/fill on the right
            idx = CoordsToIndex(ref x, ref y);
            while (true)
            {
                //**fill with the fillValue
                gridCells[idx] = true;
                //**increment
                rFillLoc++;     //increment counter
                idx += 1;//increment index
                //**exit loop if we're at edge of fill area
                if (rFillLoc >= gridWidth || gridCells[idx] || !CheckCell(ref idx))
                    break;

            }
            rFillLoc--;

           //add range to queue
           FloodFillRange r = new FloodFillRange(lFillLoc, rFillLoc, y);
           ranges.Enqueue(ref r);
        }

        ///<summary>Sees if a cell is occupied by a wall.</summary>
        ///<param name="px">The index of the grid cell to check, passed by reference to increase performance.</param>
        protected bool CheckCell(ref int px)
        {
            if (_cellData[px].IsOccupied)
            {
                if (_cellData[px].Occupant.GridObjectType == GridObjectTypes.Building.WALL)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the linear index for a cell, given its x and y coordinates.
        /// </summary>
        /// <param name="x">The x coordinate of the cell.</param>
        /// <param name="y">The y coordinate of the cell.</param>
        /// <returns></returns>
        protected int CoordsToIndex(ref int x, ref int y)
        {
            return (gridWidth * y) + x;
        }

    }

    /// <summary>
    /// Represents a linear range to be filled and branched from.
    /// </summary>
    public struct FloodFillRange
    {
        public int StartX;
        public int EndX;
        public int Y;

        public FloodFillRange(int startX, int endX, int y)
        {
            StartX=startX;
            EndX = endX;
            Y = y;
        }
    }
}
