using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Chess
{
    class Program
    {
        static void Main(string[] args)
        {
            SetupConsole();
            Menu();
        }

        static void Menu()
        {
            bool playLocal = MainMenu.PlayMainMenuGetOption(); // Regarde si c'est local ou LAN
            if (playLocal)
                StartChessGame(); // Si C'est local, juste start le jeu
            else
                PlayLan.RunLanSetup(); // Sinon, start le LAN setup
        }

        static void SetupConsole()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Permet des charactères unicode
            ScreenSettings.SetWindowSize(90, 20); // Change la grandeur du window
            ScreenSettings.DisableWindowButtons(); // Éviter l'usager de resize l'écran
            ConsoleFont.SetConsoleFont(); // Change le font de la console (pour qu'elle soit plus petit)
        }
        static void StartChessGame()
        {
            ScreenSettings.AdjustWindowForChess(); // Maintenant change la grandeur pour le jeu
            Chess game = new Chess(); // Créer une nouvelle instance de la classe
            game.Play(); // Commence
        }


        static void SetConsoleThings()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Accepte des characteres unicode.
            ScreenSettings.SetWindowPosition(10, 10); // Met le window au coin de votre ecran.
            ScreenSettings.DisableWindowButtons(); // Evite le user de changer la grosseur de la console.
        }

    }
}

