using System;
using System.Collections;

public static class AllSideAlgorithm
{
    public static int threshold;
    public static int width;
    public static int height;
    public static int lastSide;
    public static short GetAngle(byte[,] grid)
    {
        try
        {
            int coord = -1;
            // Up
            coord = ScanSide(grid, 2);
            if (coord != -1)
            {
                lastSide = 2;
                return Convert.ToInt16(Math.Round(Math.Atan(Math.Tan(grid.GetLength(1) / (coord - (grid.GetLength(0)) / 2))) * (180 / Math.PI) * 2.833));
            }
            // Right
            coord = ScanSide(grid, 3);
            if (coord != -1)
            {
                lastSide = 3;
                return Convert.ToInt16(Math.Round(Math.Atan(Math.Tan(coord / grid.GetLength(0) / 2)) * (180 / Math.PI) * 2.833));
            }
            // Left
            coord = ScanSide(grid, 1);
            if (coord != -1)
            {
                lastSide = 1;
                Math.Atan(Math.Tan(coord / grid.GetLength(0) / 2));
                return Convert.ToInt16(Math.Round(Math.Atan(Math.Tan(coord / grid.GetLength(0) / 2)) * (180 / Math.PI) * 2.833));
            }
            // Down
            /*coord = ScanSide(grid, 0);
            if (coord != -1)
            {
                lastSide = 0;
                return Math.Atan(Math.Tan(grid.GetLength(1) / (coord - (grid.GetLength(0)) / 2)));
            }*/
            return 0;
        }
        catch
        {
            System.Diagnostics.Debug.WriteLine("GetAngle ex");
            return 0;
        }
     }

    private static int ScanSide(byte[,] grid, int side)
    {
        try
        {
            int start = -1; // coord of the line starting
            int end = -1; // coord of the line ending
            switch (side)
            {
                // Left
                case 1:
                    for (int i = 0; i < grid.GetLength(1); i++)
                    {
                        if (grid[0, i] > threshold && start == -1)
                            start = i;
                        else if (grid[0, i] <= threshold && start != -1)
                        {
                            end = i - 1;
                            break;
                        }
                    }
                    end = grid.GetLength(1) - 1;
                    break;
                // Top
                case 2:
                    for (int i = 0; i < grid.GetLength(0); i++)
                    {
                        if (grid[i, grid.GetLength(1) - 1] > threshold && start == -1)
                            start = i;
                        else
                        {
                            end = i - 1;
                            break;
                        }
                    }
                    end = grid.GetLength(0) - 1;
                    break;
                // Right
                case 3:
                    for (int i = 0; i < grid.GetLength(1); i++)
                    {
                        if (grid[grid.GetLength(0) - 1, i] > threshold && start == -1)
                            start = i;
                        else if (grid[grid.GetLength(0) - 1, i] <= threshold && start != -1)
                        {
                            end = i - 1;
                            break;
                        }
                    }
                    end = grid.GetLength(1) - 1;
                    break;
                // Down
                case 0:
                    for (int i = 0; i < grid.GetLength(0); i++)
                    {
                        if (grid[i, 0] > threshold && start == -1)
                            start = i;
                        else if (grid[i, 0] <= threshold && start != -1)
                        {
                            end = i - 1;
                            break;
                        }
                    }
                    end = grid.GetLength(0) - 1;
                    break;
            }
            if (start != -1)
                return end - start;
            else
                return -1;
        }
        catch
        {
            System.Diagnostics.Debug.WriteLine("scanside ex");
            return -1;
        }
       }
}