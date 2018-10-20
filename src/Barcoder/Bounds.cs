namespace Barcoder
{
    public struct Bounds
    {
        public Bounds(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Bounds))
            {
                return false;
            }

            var bounds = (Bounds)obj;
            return X == bounds.X &&
                   Y == bounds.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
    }
}
