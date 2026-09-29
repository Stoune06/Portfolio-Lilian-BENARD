using Godot;
using System;
using System.Collections.Generic;
using Com.IsartDigital.ProjectName;

// Author : Louis Robin

namespace Com.IsartDigital.ProjectName
{

    public partial class PathFindingCell
    {
        public int posX;
        public int posY;
        public int cost;
        public int distance;
        public int costDistance => cost + distance;
        public PathFindingCell lastCell;

        public void SetDistance(int pTargetX, int pTargetY)
        {
            distance = (int)Mathf.Abs(Mathf.Abs(pTargetX - posX) + Mathf.Abs(pTargetY - posY));
        }

    }

    public partial class PathFinding : Node
    {
        public static List<Vector2> GetPath(List<List<Movable>> pGrid, int pStartPosX, int pStartPosY, int pTargetPosX, int pTargetPosY, Movable pClickedObject)
        {
            List<Vector2> lPath = new List<Vector2>();
            PathFindingCell lStartCell = new PathFindingCell { posX = pStartPosX, posY = pStartPosY };
            PathFindingCell lTargetCell = new PathFindingCell { posX = pTargetPosX, posY = pTargetPosY };

            lStartCell.SetDistance(lTargetCell.posX, lTargetCell.posY);

            List<PathFindingCell> lActiveCells = new List<PathFindingCell>();
            lActiveCells.Add(lStartCell);
            List<PathFindingCell> lVisitedCells = new List<PathFindingCell>();

            while (lActiveCells.Count > 0)
            {
                lActiveCells.Sort((x, y) => x.costDistance.CompareTo(y.costDistance));
                PathFindingCell lCheckCell = lActiveCells[0];

                if (lCheckCell.posX == lTargetCell.posX && lCheckCell.posY == lTargetCell.posY)
                {
                    PathFindingCell lCell = lCheckCell;
                    while (true)
                    {
                        lPath.Add(new Vector2(lCell.posX, lCell.posY));
                        lCell = lCell.lastCell;

                        if (lCell == null) return lPath;
                    }
                }

                lVisitedCells.Add(lCheckCell);
                lActiveCells.Remove(lCheckCell);

                List<PathFindingCell> lWalkableCells = GetWalkableCells(pGrid, lCheckCell, lTargetCell, pClickedObject);

                for (int i = 0; i < lWalkableCells.Count; i++)
                {
                    if (TestVisitedCells(lWalkableCells[i], lVisitedCells)) continue;


                    if (TestActiveCells(lWalkableCells[i], lActiveCells))
                    {
                        PathFindingCell lExistingCell = default;
                        for (int k = 0; k < lActiveCells.Count; k++)
                        {
                            if (lActiveCells[k].posX == lWalkableCells[i].posX && lActiveCells[k].posY == lWalkableCells[i].posY)
                            {
                                lExistingCell = lActiveCells[k];
                                break;
                            }
                        }

                        if (lExistingCell.costDistance > lCheckCell.costDistance)
                        {
                            lActiveCells.Remove(lExistingCell);
                            lActiveCells.Add(lWalkableCells[i]);
                        }

                    }
                    else lActiveCells.Add(lWalkableCells[i]);
                }
            }
            return lPath;
        }


        public static List<PathFindingCell> GetWalkableCells(List<List<Movable>> pGrid, PathFindingCell pCurrentCell, PathFindingCell pTargetCell, Movable pClickedObject)
        {
            List<PathFindingCell> lWalkableCells = new List<PathFindingCell>();

            int lMaxX = pGrid[0].Count - 1;
            int lMaxY = pGrid.Count - 1;

            lWalkableCells.Add(new PathFindingCell { posX = pCurrentCell.posX, posY = pCurrentCell.posY + 1, cost = pCurrentCell.cost + 1, lastCell = pCurrentCell });
            lWalkableCells.Add(new PathFindingCell { posX = pCurrentCell.posX, posY = pCurrentCell.posY - 1, cost = pCurrentCell.cost + 1, lastCell = pCurrentCell });
            lWalkableCells.Add(new PathFindingCell { posX = pCurrentCell.posX + 1, posY = pCurrentCell.posY, cost = pCurrentCell.cost + 1, lastCell = pCurrentCell });
            lWalkableCells.Add(new PathFindingCell { posX = pCurrentCell.posX - 1, posY = pCurrentCell.posY, cost = pCurrentCell.cost + 1, lastCell = pCurrentCell });

            for (int i = lWalkableCells.Count - 1; i >= 0; i--)
            {
                if (lWalkableCells[i].posX < 0 || lWalkableCells[i].posX > lMaxX || lWalkableCells[i].posY < 0 || lWalkableCells[i].posY > lMaxY)
                {
                    lWalkableCells.RemoveAt(i);
                }
                else if (pGrid[lWalkableCells[i].posY][lWalkableCells[i].posX] != null)
                {
                    if (!(pGrid[lWalkableCells[i].posY][lWalkableCells[i].posX] == pClickedObject)) lWalkableCells.RemoveAt(i);

                }
            }

            lWalkableCells.ForEach(lCell => lCell.SetDistance(pTargetCell.posX, pTargetCell.posY));

            return lWalkableCells;
        }

        public static bool TestVisitedCells(PathFindingCell pWalkableCell, List<PathFindingCell> pVisitedCells)
        {
            for (int i = 0; i < pVisitedCells.Count; i++)
            {
                if (pWalkableCell.posX == pVisitedCells[i].posX && pWalkableCell.posY == pVisitedCells[i].posY)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TestActiveCells(PathFindingCell pWalkableCell, List<PathFindingCell> pActiveCells)
        {
            for (int i = 0; i < pActiveCells.Count; i++)
            {
                if (pWalkableCell.posX == pActiveCells[i].posX && pWalkableCell.posY == pActiveCells[i].posY)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
