using Cinnamon.Models.Effects;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Display;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Cinnamon
{
    public class MessageConduit : DisplayConduit
    {
        const int X_GAP = 4; // pixels
        const int Y_GAP = 4; // pixels
        readonly string Message;

        public MessageConduit(string message)
        {
            Message = message;
        }

        private MessageConduit() { }

        protected override void DrawForeground(DrawEventArgs e)
        {
            int left, right, top, bottom, near, far;
            if (e.Viewport.GetScreenPort(out left, out right, out bottom, out top, out near, out far))
            {
                int width = right - left;
                int height = bottom - top;

                string str = Message;
                Rectangle rect = e.Display.Measure2dText(str, new Point2d(0, 0), false, 0.0, 36, "Arial");

                if (rect.Width + (2 * X_GAP) < width || rect.Height + (2 * Y_GAP) < height)
                {
                    // Cook up text location (lower right corner of viewport)
                    Point2d point = new Point2d(right - rect.Width - (X_GAP*2), bottom + rect.Height + Y_GAP);

                    //e.Display.Draw2dRectangle(rect, Color.White, 1, Color.White);
                    e.Display.Draw2dText(str, Color.Yellow, point, false, 36, Message);
                }
            }
        }
    }
}
