using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace ZXCL.WinFormUI
{
    class ControlBoxDrawImage
    {
        private static int width = 30;
        private static int height = 25;

        public enum ImgeType
        {
            Close,
            Minimize,
            Maximize,
            Restore
        }

        public static Bitmap DrawImage(Color foreColor, Color backColor, ImgeType type)
        {
            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            Rectangle rec = new Rectangle(0, 0, width, height);
            g.FillRectangle(new SolidBrush(backColor), rec);

            GraphicsPath path = new GraphicsPath();
            switch (type)
            {
                case ImgeType.Close:
                    path = createCloseFlagPath(rec);
                    break;
                case ImgeType.Minimize:
                    path = createMinimizeFlagPath(rec);
                    break;
                case ImgeType.Maximize:
                    path = createMaximizeFlagPath(rec);
                    break;
                case ImgeType.Restore:
                    path = createRestoreFlagPath(rec);
                    break;
                default:
                    break;
            }

            Pen p = new Pen(foreColor);
            g.DrawPath(p, path);
            p.Dispose();
            g.Dispose();
            return bmp;
        }

        private static GraphicsPath createCloseFlagPath(Rectangle rect)
        {

            GraphicsPath path = new GraphicsPath();

            int x = rect.X + (rect.Width - 9) / 2;
            int y = rect.Y + (rect.Height - 7) / 2;

            Point p1 = new Point(x + 1 - 1, y - 1);
            Point p2 = new Point(x + 7, y + 6);
            Point p3 = new Point(x + 8, y + 6);
            Point p4 = new Point(x + 2 - 1, y - 1);

            Point p5 = new Point(x + 6 + 1, y - 1);
            Point p6 = new Point(x, y + 6);
            Point p7 = new Point(x + 1, y + 6);
            Point p8 = new Point(x + 7 + 1, y - 1);

            path.AddLine(p1, p2);
            path.AddLine(p3, p4);
            path.CloseFigure();
            path.AddLine(p5, p6);
            path.AddLine(p7, p8);

            return path;
        }

        private static GraphicsPath createMinimizeFlagPath(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();
            int x = rect.X + (rect.Width - 9) / 2;
            int y = rect.Y + (rect.Height - 7) / 2;
            Point p1 = new Point(x + 1, y + 5);
            Point p2 = new Point(x + 7, y + 5);
            Point p3 = new Point(x + 1, y + 6);
            Point p4 = new Point(x + 7, y + 6);
            path.AddLines(new Point[] { p1, p2, p3, p4 });
            return path;
        }

        private static GraphicsPath createMaximizeFlagPath(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();
            int x = rect.X + (rect.Width - 9) / 2;
            int y = rect.Y + (rect.Height - 7) / 2;
            Point p1 = new Point(x + 1, y + 1);
            Point p2 = new Point(x + 7, y + 1);
            path.AddRectangle(new Rectangle(new Point(x, y), new Size(8, 6)));
            path.CloseFigure();
            path.AddLine(p1, p2);
            return path;
        }

        private static GraphicsPath createRestoreFlagPath(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();
            int x = rect.X + (rect.Width - 11) / 2;
            int y = rect.Y + (rect.Height - 9) / 2;

            Point p1 = new Point(x, y + 3);
            Point p2 = new Point(x + 6, y + 3);
            Point p3 = new Point(x + 6, y + 4);
            Point p4 = new Point(x + 6, y + 8);
            Point p5 = new Point(x, y + 8);
            Point p6 = new Point(x, y + 4);

            Point p7 = new Point(x + 7, y + 5);
            Point p8 = new Point(x + 9, y + 5);
            Point p9 = new Point(x + 9, y + 1);
            Point p10 = new Point(x + 3, y + 1);
            Point p11 = new Point(x + 3, y + 2);
            Point p12 = new Point(x + 3, y);
            Point p13 = new Point(x + 9, y);

            path.AddLines(new Point[] { p1, p2, p4, p5, p6, p3, p2, p1 });
            path.CloseFigure();

            path.AddLines(new Point[] { p7, p8, p9, p10, p11, p12, p13, p8, p7 });
            return path;
        }
    }
}
