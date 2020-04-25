using System;
using System.Collections.Generic;
using System.Text;

namespace E.ExploreDeezer.WPF
{
    public class Program
    {
        [System.STAThreadAttribute()]
        public static void Main()
        {
            using (new E.ExploreDeezer.WPF.Host.App())
            {
                var app = new App();
                app.InitializeComponent();
                app.Run();
            }
        }
    }
}
