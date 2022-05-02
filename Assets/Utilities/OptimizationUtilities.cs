

namespace Assets.Utilities
{
    public static class OptimizationUtilities
    {
        private const byte One = 1;
        private const byte Zero = 0;
        public static byte ToByte(this bool b)
        {
            return b ? One : Zero;
        }
    }
}
