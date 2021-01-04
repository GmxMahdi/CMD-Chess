using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace Chess
{
    class MainMenu
    {
        static string[] title = new string[]
        {
            "  .oooooo.   ooo                                   ",
            " d8P'  `Y8b  888                                   ",
            "888          888 .oo.    .ooooo.   .oooo.o  .oooo.o",
            "888          888P\"Y88b  d88' `88b d88(  \"8 d88(  \"8",
            "888          888   888  888ooo888 `\"Y88b.  `\"Y88b. ",
            "`88b    ooo  888   888  888    .o o.  )88b o.  )88b",
            " `Y8bood8P'  88o   888  `Y8bod8P' 888888P' 888888P'"
        };

        static string mention = "Vous devez utilisez votre ";
        static string highlight = "souris";
        static string mention_2 = " pour jouer au jeu.";
        static string mention_3 = "Drag and drop les pièces pour les bouger.";
        static string mention_4 = "Le jeu a du son.";
        static string mention_5 = " Augmentez le volume!";
        static string mention_6 = "*Vous devez jouer à deux joueurs:";

        public static bool PlayMainMenuGetOption()
        {
            Intro();
            return Graphics.ChooseOption("PLAY BOTH HERE", "PLAY ON LAN"); // Choisit si c'est sur la même ordit ou sur LAN
        }


        private static void Intro()
        {
            Console.CursorVisible = false;
            Console.Write("\n\t\t  ");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (string line in title)
            {
                Console.Write(line + "\n\t\t  ");
                Thread.Sleep(34);
            }
            Thread.Sleep(700);

            Console.Write("\n\n\n\t\t   ");
            Console.Write(mention);

            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(highlight);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(mention_2);

            Console.Write("\n\t\t\t");
            Console.Write(mention_3);

            Console.Write("\n\n\t\t\t  ");
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(mention_4);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(mention_5);

            Console.Write("\n\t\t\t     ");
            Console.Write(mention_6);
        }

        private static void AnimText(string text)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(15);
            }
        }
    }
}
