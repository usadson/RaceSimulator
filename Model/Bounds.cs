﻿namespace Model
{
    public class Bounds
    {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public int Left { get => X; }
        public int Right { get => X + Width; }

        public int Top { get => Y; }
        public int Bottom { get => Y + Height; }

        public int MiddleX { get => X + Width / 2; }
        public int MiddleY { get => Y + Height / 2; }

        public Bounds(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public static Bounds CreateWithLeftTopRightBottom(int left, int top, int right, int bottom)
        {
            return new Bounds(left, top, right - left, bottom - top);
        }
    }
}
