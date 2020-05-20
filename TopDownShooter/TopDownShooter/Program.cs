using System;

namespace TopDownShooter
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new Startup();
            game.Run();
        }
    }
}
