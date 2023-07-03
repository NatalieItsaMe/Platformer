namespace Platformer
{
    internal class Debug
    {
        public static void LogInfo(params object[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                System.Diagnostics.Debug.WriteLine("Object " + i + ": " + objects[i].ToString());
            }
        }
    }
}
