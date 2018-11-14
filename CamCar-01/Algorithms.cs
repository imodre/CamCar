using System;
using System.Collections;

public abstract class LineResearching   
{
    protected byte[,] GetPixelMatrix()
    {
        return new byte[0, 0];
    }
    public abstract float GetAngle();
    #region Utilities
    /*private static byte[] FromImageToArray(Image imageIn)
    {
        MemoryStream ms = new MemoryStream();
        imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
        return ms.ToArray();
    }*/
    private static byte[,] From1DArrayTo2DArray(byte[] input)
    {
        byte[,] result = new byte[320, 240];
        for (int i = 0; i < 240; i++)
        {
            for (int ii = 0; ii < 320; ii++)
            {
                result[ii, i] = input[ii + i * 240];
            }
        }
        return result;
    }
    private static byte[] From2DArrayTo1DArray(byte[,] input)
    {
        byte[] result = new byte[320 * 240];
        for (int i = 0; i < 240; i++)
        {
            for (int ii = 0; ii < 320; ii++)
            {
                result[ii * 320 + i] = input[i, ii];
            }
        }
        return result;
    }
    /*private static Image FromArrayToImage(byte[] input)
    {
        return (Bitmap)((new ImageConverter()).ConvertFrom(input));
    }*/
    #endregion
}
public static class AllSideAlgorithm
{
    public static int threshold;
    public static int width;
    public static int height;

  /*  public static float GetAngle(int[,] grid)
    {

    }
    public static float GetAngle(int[,] grid, int side)
    {

    }*/
    private static int ScanSide(int[,] grid, int side)
    {
        int start = -1;
        int end = -1;
        switch (side)
        {
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
}
