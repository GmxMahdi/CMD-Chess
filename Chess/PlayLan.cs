using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Chess
{
    class PlayLan
    {
        public static string playerUsername; // Name du premier joueur
        public static string player2Username; // Name du deuxième joueur

        static int port_number = 5000; // Le connection va être dans ce port.
        static string player_ip_address; // L'addresse pour que l'autre joueur se connecte
        static IPAddress localAddress; // Transformer le string en un IPAddress class
        static TcpListener listener; // L'affaire qui a écouter, en sorte
        public static TcpClient client; // Le beau client

        public static void RunLanSetup()
        {
            Console.Clear();
            Console.SetCursorPosition(10, 4);
            playerUsername = RequestName(); // Demande le nom du joueur

            bool isServer = Graphics.ChooseOption("CREATE GAME", "JOIN GAME"); // Demande si c'est lui qui créé le jeu
            if (isServer)
                CreateServer();
            else
                JoinServer();
        }

        private static void CreateServer()
        {
            WaitForPlayer(); // Attend la personne 
            player2Username = GetMessage(); // Prend son username
            SendMessage(playerUsername); // Donne ton username

            // un peu presseux pour faire les calculs.. je jsute sauvegarde la position du cursor
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            Console.SetCursorPosition(0, 14);
            Console.WriteLine("\t2: " + player2Username); 

            Console.SetCursorPosition(left, top);
            string info_game = "\t" + player2Username + " joined the game!";
            Graphics.AnimateText(info_game, 30, 1200);
            Console.Write("\n\n\tYOU ARE PLAYING AS WHITE.");
            Thread.Sleep(1500);
            Graphics.AnimateText("\n\n\n\n\n\n\n\tLe jeu commence dans 3...", 30, 3000);

            ScreenSettings.AdjustWindowForChess();
            Chess chess = new Chess(true, true);
            chess.Play();
        }

        private static void JoinServer()
        {
            GetConnection(); // Join la connection
            SendMessage(playerUsername); // Donne ton username
            player2Username = GetMessage(); // Prend son username

            // un peu presseux pour faire les calculs.. je jsute sauvegarde la position du cursor
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            // Dessine la table de lobby, puis l'info juste pour l'esthétique
            // Désoler pour vos yeux
            Console.Write("\n\n\n\n\n\tLOBBY\n\t_______________________________\n\t1: " + player2Username + "\n\t2: " +playerUsername);
            Console.SetCursorPosition(left, top);

            string info_game = "\n\tYou joined \"" + player2Username + "\"'s game...";
            Graphics.AnimateText(info_game, 30, 2100);
            Console.Write("\n\n\tYOU ARE PLAYING AS BLACK.");
            Thread.Sleep(1800);
            Graphics.AnimateText("\n\n\n\n\n\n\n\tLe jeu commence dans 3...", 30, 3000);

            ScreenSettings.AdjustWindowForChess(); // Adjuste la fenêtre pour le jeu
            Chess chess = new Chess(true, false);
            chess.Play();
        }

        private static void WaitForPlayer()
        {
            player_ip_address = GetLocalIP();
            Console.Clear();
            Console.SetCursorPosition(0, 3);
            Console.Write("\tPlayer 2 must write the following IP address: ");
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Write(player_ip_address);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("\n\n\tWaiting for Player 2...\n\n");

            // Paresse de resituer le cursor encore :)
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            // Dessine la table de lobby
            Console.Write("\n\n\n\n\tLOBBY\n\t_______________________________\n\t1: " + playerUsername);
            Console.SetCursorPosition(left, top);


            localAddress = IPAddress.Parse(player_ip_address); // Transforme le string en IPAddress class
            listener = new TcpListener(localAddress, port_number); // Créer une nouvelle écoute
            listener.Start(); // Commence l'écoute
            client = listener.AcceptTcpClient(); // Attend que quelqu'un arrive
        }

        public static string GetMessage()
        {
            NetworkStream messageStream = client.GetStream(); // Regarde le stream
            byte[] get_message = new byte[client.ReceiveBufferSize]; // Initialize la largeur du message
            int bytesRead = messageStream.Read(get_message, 0, client.ReceiveBufferSize); // Met le dans notre byte array
            return Encoding.ASCII.GetString(get_message, 0, bytesRead); // Retourne le byte array mais converti en string
        }

        public static void SendMessage(string message)
        {
            NetworkStream messageStream = client.GetStream(); // Regarde le stream
            byte[] send_message = ASCIIEncoding.ASCII.GetBytes(message); // Créer un message, transforme le en byte array
            messageStream.Write(send_message, 0, send_message.Length); // Envoie ton message au stream
        }

        private static void GetConnection()
        {
            while (true)
            {
                // Demande le joueur le IP de l'autre joueur
                Console.Clear();
                Console.SetCursorPosition(0, 3);
                Console.Write("\tEcrit le numero de serveur de l'autre joueur: ");
                player_ip_address = Console.ReadLine();

                // Connecte
                Console.Write("\n\tEntrain de connecter...\n");
                try
                {
                    client = new TcpClient(player_ip_address, port_number);
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    // Si ça rate, écrit que c'est mauvais puis recommence
                    Console.Write("\n\tErreur: ");
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.Write(e.Message + "\n\n");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Thread.Sleep(3000);
                    continue;
                }
                break;
            }
        }

        private static string RequestName()
        {
            // Très petite fonction...
            Console.Write("Écrivez votre nom: ");
            return Console.ReadLine();
        }

        private static string GetLocalIP()
        {

            IPAddress[] ipAddress = Dns.GetHostByName(Dns.GetHostName()).AddressList; // Get la liste les addresses qu'il trouve
            foreach (IPAddress address in ipAddress)
            {
                // 10.80 et 192.168, selon mes connaissances, sont les deux choses qui disent que l'ip est local.
                string str_address = address.ToString();
                if (str_address.StartsWith("10.80") || str_address.StartsWith("192.168"))
                    return str_address;
            }

            // Si ca ne le trouve pas, lance une exception disant qu'on a pas de bon IP trouvé.
            throw new NoValidIpFound();
        }

        private class NoValidIpFound: Exception
        {
            public NoValidIpFound()
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Je n'ai pas trouvé un bon IP pour une connection valide...");
                Environment.Exit(0x35);
            }

        }
    }
}
