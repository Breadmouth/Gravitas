using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravitas
{
    static public class MathX
    {
        static public float GetAngleBetween(Vector2 lhs, Vector2 rhs)
        {
            float xDiff = rhs.X - lhs.X;
            float yDiff = rhs.Y - lhs.Y;
            return (float)(Math.Atan2(yDiff, xDiff) * (180 / Math.PI));
        }

        static public float LineLength(Vector2 lhs, Vector2 rhs)
        {
            return (float)Math.Sqrt(((rhs.X - lhs.X) * (rhs.X - lhs.X)) + ((rhs.Y - lhs.Y) * (rhs.Y - lhs.Y)));
        }

        static public Vector2 Midpoint(Vector2 lhs, Vector2 rhs)
        {
            float x = (lhs.X + rhs.X) / 2;
            float y = (lhs.Y + rhs.Y) / 2;
            return new Vector2(x, y);
        }
    }
}
