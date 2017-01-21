using UnityEngine;
using System.Collections.Generic;

namespace FloodFill2
{
    /// <summary>
    /// The base class that the flood fill algorithms inherit from. Implements the
    /// basic flood filler functionality that is the same across all algorithms.
    /// </summary>
    public abstract class AbstractFloodFiller
    {
        //cached grid properties
        protected int gridWidth = 0;
        protected int gridHeight = 0;
        protected bool[] gridCells = null;

        public AbstractFloodFiller(int gridSizeX, int gridSizeY)
        {
            gridWidth = gridSizeX;
            gridHeight = gridSizeY;
            gridCells = new bool[gridWidth * gridHeight];
        }

        public abstract FillData FloodFill(Vector2Int pt);
    }

    public class FillData
    {
        private int _gridSize;
        private bool[] _cells;

        public FillData(int gridSize, bool[] cells)
        {
            _gridSize = gridSize;
            _cells = cells;
        }

        public bool IsCellReachable(Vector2Int pt)
        {
            int idx = CoordsToIndex(ref pt.x, ref pt.y);
            if (idx < _cells.Length)
            {
                return _cells[idx];
            }
            Debug.LogWarning("CellValue '" + pt.ToString() + "' out of range of grid!");
            return false;
        }

        /// <summary>
        /// Returns the linear index for a pixel, given its x and y coordinates.
        /// </summary>
        /// <param name="x">The x coordinate of the pixel.</param>
        /// <param name="y">The y coordinate of the pixel.</param>
        /// <returns></returns>
        protected int CoordsToIndex(ref int x, ref int y)
        {
            return (_gridSize * y) + x;
        }

        public void DebugDraw(int targetX, int targetY, GUISkin skin)
        {
            float startX = 15f;
            float startY = 15f;
            float w = 12f;
            float h = 12f;

            Rect rect = new Rect(startX, startY, w, h);

            GUI.skin = skin;
            GUI.color = Color.white;

            for (int y = 0; y < _gridSize; y++)
            {
                for (int x = 0; x < _gridSize; x++)
                {
                    if (!_cells[CoordsToIndex(ref x, ref y)])
                    {
                        rect.x = (w * x) + startX;
                        rect.y = (h * y) + startY;
                        GUI.Box(rect, "");
                    }
                }
            }

            GUI.color = Color.green;
            rect.x = (w * targetX) + startX;
            rect.y = (h * targetY) + startY;
            GUI.Box(rect, "X");
        }
    }
}
