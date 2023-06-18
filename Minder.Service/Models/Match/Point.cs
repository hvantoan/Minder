using System;

namespace Minder.Service.Models.Match {

    public class Point {
        public double X { get; set; }
        public double Y { get; set; }
        public string? StadiumId { get; set; }

        public Point(decimal x, decimal y, string stadiumId) {
            this.X = decimal.ToDouble(x);
            this.Y = decimal.ToDouble(y);
            this.StadiumId = stadiumId;
        }

        public Point(double x, double y) {
            this.X = x;
            this.Y = y;
        }

        public Point(decimal x, decimal y) {
            this.X = decimal.ToDouble(x);
            this.Y = decimal.ToDouble(y);
        }

        public double DistanceTo(Point other) {
            double dx = X - other.X;
            double dy = Y - other.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}