
namespace GreenLightTracker.Src
{
    public static class PathId
    {
        static int m_id = 0;

        public static int Generate()
        {
            return m_id++;
        }

        public static void Reset()
        {
            m_id = 0;
        }
    }
}
