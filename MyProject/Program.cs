using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProject
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new MyGame())
                game.Run();
        }
    }
}