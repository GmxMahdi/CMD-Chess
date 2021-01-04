using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chess
{
    public static class Graphics
    {
        static public void AnimateText(string text, int speed, int wait = 0)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(speed); // Dors à chaque charactère écrit

            }
            if (wait != 0)
                Thread.Sleep(wait);
        }

        static public bool ChooseOption(string option1, string option2)
        {

            Thread recordMouse = new Thread(RecordMouse.Run);  // Commence à track la position de la souris
            recordMouse.Start();

            RecordMouse.row = 0;
            RecordMouse.col = 0;

            Console.CursorVisible = false;
            // Position de la première option
            const int option1_left = 20; 
            const int option1_top = 17;
            string option1_string = option1;

            // Position de la deuxième option
            const int option2_left = 60;
            const int option2_top = 17;
            string option2_string = option2;

            // Écrit-les
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(option1_left, option1_top);
            Console.WriteLine(option1_string);

            Console.SetCursorPosition(option2_left, option2_top);
            Console.WriteLine(option2_string);

            int option1_size = option1_string.Length;
            int option2_size = option2_string.Length;


            bool prevOption1 = false; // Enleve le stress du display en éviant de spamwrite
            bool selectOption1 = false;

            bool prevOption2 = false; // Enleve le stress du display en éviant de spamwrite
            bool selectOption2 = false;

            while (true)
            {
                int row = RecordMouse.row; // Get la position Y
                int col = RecordMouse.col; // Get la position X

                // OPTION 1 Display
                Console.SetCursorPosition(option1_left, option1_top);
                if (row == option1_top && (col >= option1_left && col < option1_left + option1_string.Length)) // Si le cursor est SUR le texte
                {
                    selectOption1 = true; 
                    if (prevOption1 == false)  // Si avant c'était pas sur le texte, desinne le highlight
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine(option1_string);
                    }
                    prevOption1 = true;

                }
                else
                {
                    selectOption1 = false;
                    if (prevOption1 == true) // Si avant c'était sur le texte, enlève le highlight
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(option1_string);
                    }
                    prevOption1 = false;
                }

                // Option 2 Display
                Console.SetCursorPosition(option2_left, option2_top);
                if (row == option2_top && (col >= option2_left && col < option2_left + option2_string.Length)) // Si le cursor est SUR le texte
                {
                    selectOption2 = true;
                    if (prevOption2 == false) // Si avant c'était pas sur le texte, desinne le highlight
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine(option2_string);
                    }
                    prevOption2 = true;
                }
                else
                {
                    selectOption2 = false;
                    if (prevOption2 == true) // Si avant c'était sur le texte, enlève le highlight
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(option2_string);
                    }
                    prevOption2 = false;
                }

                if (RecordMouse.isMouseHeld == 0x1)
                {
                    if (selectOption1) // Si la première option est selectioné, met une belle couleur
                    {
                        Console.SetCursorPosition(option1_left, option1_top);
                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(option1_string);
                    }
                    else if (selectOption2)  // Si la deuxième option est selectioné, met une belle couleur
                    {
                        Console.SetCursorPosition(option2_left, option2_top);
                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(option2_string);
                    }

                    while (RecordMouse.isMouseHeld == 0x1) ;
                    row = RecordMouse.row;
                    col = RecordMouse.col;
                    if ((row == option1_top && (col >= option1_left && col < option1_left + option1_string.Length)) && selectOption1)
                    {
                        recordMouse.Abort();
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        return true;
                    }
                    else if ((row == option2_top && (col >= option2_left && col < option2_left + option2_string.Length)) && selectOption2)
                    {
                        recordMouse.Abort();
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        return false;
                    }
                }
            }
        }

        public static string[] pawnGraphics = new string[]
{
            "XXXXXXX",
            "X█████X",
            "X█████X",
            "XX███XX",
            "███████"
};

        public static string[] queenGraphics = new string[]
        {
            "r█e█l█e",
            "aX█Q█XX",
            "s█████X",
            "eX███XX",
            "X█████X"
        };
        public static string[] rookGraphics = new string[]
        {
            "█X███X█",
            "X█████X",
            "X█████X",
            "X█TXT█X",
            "███████"
        };
        public static string[] knightGraphics = new string[]
        {
             "XX████X",
             "X██HX██",
             "██XX███",
             "XX███XX",
             "███████"
        };
        public static string[] bishopGraphics = new string[]
        {
            "XXXXXXX",
            "XX███XX",
            "X█XBX█X",
            "XX███XX",
            "███████"
        };
        public static string[] kingGraphics = new string[]
        {
            "XXX█XXX",
            "X██X██X",
            "XX█X█XX",
            "XX█K█XX",
            "X█████X"
        };

        public static string[] pawnBlackGraphics = new string[]
{
            "XXXXXXX",
            "X@@@@@X",
            "X@@@@@X",
            "XX@@@XX",
            "@@@@@@@"
};
        public static string[] queenBlackGraphics = new string[]
        {
            "X@X@X@X",
            "XX@Q@XX",
            "X@@@@@X",
            "XX@@@XX",
            "X@@@@@X"
        };
        public static string[] rookBlackGraphics = new string[]
        {
            "@X@@@X@",
            "X@@@@@X",
            "X@@@@@X",
            "X@TXT@X",
            "@@@@@@@"
        };
        public static string[] knightBlackGraphics = new string[]
        {
             "XX@@@XX",
             "X@@HX@@",
             "@@XX@@@",
             "XX@@@XX",
             "@@@@@@@"
        };
        public static string[] bishopBlackGraphics = new string[]
        {
            "XXXXXXX",
            "XX@@@XX",
            "X@XBX@X",
            "XX@@@XX",
            "@@@@@@@"
        };
        public static string[] kingBlackGraphics = new string[]
        {
            "XXX@XXX",
            "X@@X@@X",
            "XX@X@XX",
            "XX@K@XX",
            "X@@@@@X"
        };

        public static string[] kingWhiteCheckGraphics = new string[]
        {
            "RRR█RRR",
            "R██R██R",
            "RR█R█RR",
            "RR█R█RR",
            "R█████R"
        };

        public static string[] kingBlackCheckGraphics = new string[]
        {
            "RRR@RRR",
            "R@@R@@R",
            "RR@R@RR",
            "RR@R@RR",
            "R@@@@@R"
        };

        public static string[] emptyWhiteGraphics =
        { "         ", "         ", "         ", "         ", "         "};

        public static string[] emptyBlackGraphics =
        {
            "▒▒▒▒▒▒▒▒▒", "▒▒▒▒▒▒▒▒▒", "▒▒▒▒▒▒▒▒▒", "▒▒▒▒▒▒▒▒▒", "▒▒▒▒▒▒▒▒▒"
        };

        public const string colSplitB = "┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃";
        public const string colSplitW = "┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃";
        public const string topSplit = "┏━━━━━━━━━┳━━━━━━━━━┳━━━━━━━━━┳━━━━━━━━━┳━━━━━━━━━┳━━━━━━━━━┳━━━━━━━━━┳━━━━━━━━━┓";
        public const string rowSplit = "┝━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┤";
        public const string botSplit = "┗━━━━━━━━━┷━━━━━━━━━┷━━━━━━━━━┷━━━━━━━━━┷━━━━━━━━━┷━━━━━━━━━┷━━━━━━━━━┷━━━━━━━━━┛";

        public static string[] inGameTitle =
        {
            " .d8888b.  888                                 ",
            "d88P  Y88b 888                                 ",
            "888    888 888                                 ",
            "888        88888b.   .d88b.  .d8888b  .d8888b  ",
            "888        888 \"88b d8P  Y8b 88K      88K      ",
            "888    888 888  888 88888888 \"Y8888b. \"Y8888b. ",
            "Y88b  d88P 888  888 Y8b.          X88      X88",
            " \"Y8888P\"  888  888  \"Y8888   88888P'  88888P"
        };

        //public static string[] selectPieceInterface =
        //{
        //};

    }
}
