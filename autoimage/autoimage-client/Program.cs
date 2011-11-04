using System;
using System.Collections.Generic;
using System.Text;

namespace MGO.AutoImage.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Imager imager = new Imager();
            Console.WriteLine("autoimage v0.1");
            //Console.WriteLine("Bitte Taste drücken um diesen client für autoimage einzurichten(Timeout:20 Sekunden)");
            //Console.WriteLine("Ansonsten warten bis automatisches Image erstellt wird.");
            if (args.Length > 0)
                imager.CreateConfig();
            else    
                imager.DoImage();
        }
    }
}
