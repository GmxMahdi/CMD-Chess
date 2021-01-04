using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Chess
{
    class Chess
    {
        // PLAYERS
        private bool gameIsLAN; // Pour LAN
        private Color userColor; // Aussi pour LAN

        private Player _player_white = new Player(Color.WHITE);
        private Player _player_black = new Player(Color.BLACK);
        private Color _player_turn = Color.WHITE;

        private List<PieceType> deadPieceTableWhite;
        private List<PieceType> deadPieceTableBlack;

        private bool whiteWasOnCheck = false;
        private bool blackWasOnCheck = false;
        private bool isGameOver = false; // Voir si le jeu a fini
        private bool isStalemate; // Savoir si la fin du jeu est un draw

        private bool isHoldingPiece = false;
        private bool isSimulatingMove = false;

        // GRAPHICS
        private Coordinate _startCellPosition;
        private Coordinate _endCellPosition;

        // Cela sera expliqué plus tard...
        private Coordinate _lastCapturedPosition;
        private Coordinate _pivotPoint;
        private Coordinate _stringBoardPosition;

        private Coordinate _nextDrawConsole;
        private Coordinate _nextDrawString;
        private Coordinate _lastDrawn;
        //

        // J'ai décidé d'avoir une version du chess en string.
        // Lorsque je veux animer une piece qui bouge, je vois avoir en memoire quel dessin du board dois-je redessiner lorsque la pièce bouge.
        // C'est pour ça qu'on peut voir du stresse sur le dessin lors du muovement.
        private StringBuilder[] _mimicStringBoard;  // Copie de notre _stringBoard
        private string[] _stringBoard = new string[]
{
                                      "┏━━━━━━━━━┳━━━━━━━━━┳━━━━━━━━━┳━━━━━━━━━┳━━━━━━━━━┳━━━━━━━━━┳━━━━━━━━━┳━━━━━━━━━┓",
                                      "┃▒@▒@@@▒@▒┃   @@@@  ┃▒▒▒▒▒▒▒▒▒┃  @ @ @  ┃▒▒▒▒@▒▒▒▒┃         ┃▒▒▒@@@@▒▒┃ @ @@@ @ ┃",
                                      "┃▒▒@@@@@▒▒┃  @@H @@ ┃▒▒▒@@@▒▒▒┃   @Q@   ┃▒▒@@▒@@▒▒┃   @@@   ┃▒▒@@H▒@@▒┃  @@@@@  ┃",
                                      "┃▒▒@@@@@▒▒┃ @@  @@@ ┃▒▒@▒B▒@▒▒┃  @@@@@  ┃▒▒▒@▒@▒▒▒┃  @ B @  ┃▒@@▒▒@@@▒┃  @@@@@  ┃",
                                      "┃▒▒@T▒T@▒▒┃   @@@   ┃▒▒▒@@@▒▒▒┃   @@@   ┃▒▒▒@K@▒▒▒┃   @@@   ┃▒▒▒@@@▒▒▒┃  @T T@  ┃",
                                      "┃▒@@@@@@@▒┃ @@@@@@@ ┃▒@@@@@@@▒┃  @@@@@  ┃▒▒@@@@@▒▒┃ @@@@@@@ ┃▒@@@@@@@▒┃ @@@@@@@ ┃",
                                      "┝━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┤",
                                      "┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃",
                                      "┃  @@@@@  ┃▒▒@@@@@▒▒┃  @@@@@  ┃▒▒@@@@@▒▒┃  @@@@@  ┃▒▒@@@@@▒▒┃  @@@@@  ┃▒▒@@@@@▒▒┃",
                                      "┃  @@@@@  ┃▒▒@@@@@▒▒┃  @@@@@  ┃▒▒@@@@@▒▒┃  @@@@@  ┃▒▒@@@@@▒▒┃  @@@@@  ┃▒▒@@@@@▒▒┃",
                                      "┃   @@@   ┃▒▒▒@@@▒▒▒┃   @@@   ┃▒▒▒@@@▒▒▒┃   @@@   ┃▒▒▒@@@▒▒▒┃   @@@   ┃▒▒▒@@@▒▒▒┃",
                                      "┃ @@@@@@@ ┃▒@@@@@@@▒┃ @@@@@@@ ┃▒@@@@@@@▒┃ @@@@@@@ ┃▒@@@@@@@▒┃ @@@@@@@ ┃▒@@@@@@@▒┃",
                                      "┝━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┤",
                                      "┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃",
                                      "┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃",
                                      "┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃",
                                      "┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃",
                                      "┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃",
                                      "┝━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┤",
                                      "┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃",
                                      "┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃",
                                      "┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃",
                                      "┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃",
                                      "┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃",
                                      "┝━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┤",
                                      "┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃",
                                      "┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃",
                                      "┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃",
                                      "┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃",
                                      "┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃",
                                      "┝━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┤",
                                      "┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃",
                                      "┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃",
                                      "┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃",
                                      "┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃",
                                      "┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃",
                                      "┝━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┤",
                                      "┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃▒▒▒▒▒▒▒▒▒┃         ┃",
                                      "┃▒▒█████▒▒┃  █████  ┃▒▒█████▒▒┃  █████  ┃▒▒█████▒▒┃  █████  ┃▒▒█████▒▒┃  █████  ┃",
                                      "┃▒▒█████▒▒┃  █████  ┃▒▒█████▒▒┃  █████  ┃▒▒█████▒▒┃  █████  ┃▒▒█████▒▒┃  █████  ┃",
                                      "┃▒▒▒███▒▒▒┃   ███   ┃▒▒▒███▒▒▒┃   ███   ┃▒▒▒███▒▒▒┃   ███   ┃▒▒▒███▒▒▒┃   ███   ┃",
                                      "┃▒███████▒┃ ███████ ┃▒███████▒┃ ███████ ┃▒███████▒┃ ███████ ┃▒███████▒┃ ███████ ┃",
                                      "┝━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┿━━━━━━━━━┤",
                                      "┃ █ ███ █ ┃▒▒▒████▒▒┃         ┃▒▒█▒█▒█▒▒┃    █    ┃▒▒▒▒▒▒▒▒▒┃   ████  ┃▒█▒███▒█▒┃",
                                      "┃  █████  ┃▒▒██H▒██▒┃   ███   ┃▒▒▒█Q█▒▒▒┃  ██ ██  ┃▒▒▒███▒▒▒┃  ██H ██ ┃▒▒█████▒▒┃",
                                      "┃  █████  ┃▒██▒▒███▒┃  █ B █  ┃▒▒█████▒▒┃   █ █   ┃▒▒█▒B▒█▒▒┃ ██  ███ ┃▒▒█████▒▒┃",
                                      "┃  █T T█  ┃▒▒▒███▒▒▒┃   ███   ┃▒▒▒███▒▒▒┃   █K█   ┃▒▒▒███▒▒▒┃   ███   ┃▒▒█T▒T█▒▒┃",
                                      "┃ ███████ ┃▒███████▒┃ ███████ ┃▒▒█████▒▒┃  █████  ┃▒███████▒┃ ███████ ┃▒███████▒┃",
                                      "┗━━━━━━━━━┷━━━━━━━━━┷━━━━━━━━━┷━━━━━━━━━┷━━━━━━━━━┷━━━━━━━━━┷━━━━━━━━━┷━━━━━━━━━┛"
};

        // BOARD
        private Cell[,] _mimicChessBoard; // Copie de notre jeu d'échecs
        private Cell[,] _chessboard = new Cell[_chessboard_height, _chessboard_width]; // Notre jeu d'échecs.
        string[][] pieceMap = new string[][] { Graphics.kingGraphics, Graphics.queenGraphics,Graphics.bishopGraphics, Graphics.knightGraphics, Graphics.rookGraphics, Graphics.pawnGraphics,
                                                Graphics.kingBlackGraphics, Graphics.queenBlackGraphics, Graphics.bishopBlackGraphics, Graphics.knightBlackGraphics, Graphics.rookBlackGraphics, Graphics.pawnBlackGraphics};

        Thread THREAD_trackCursor = new Thread(RecordMouse.Run);

        // CONSTANTS
        const int kingIndex = 4; // Pour situer la piece dans le piece_array du player

        // Positions du dessin
        private const int _piece_width = 7; // Largeur d'une pièce (en charactères)
        private const int _piece_height = 5; // Longueur d'une pièce (en charactères)

        private int _chessboard_string_width_max = Graphics.topSplit.Length; // Largeur de la table du jeu (en charactères)
        private const int _chessboard_string_height_max = 49; // Longueur de la table du jeu (en charactères)

        private const int _chessboard_width = 8; // Largeur de la table du jeu
        private const int _chessboard_height = 8; // Longeur de la table du jeu

        private const int top_padding = 10; // Distance entre le jeu d'échec et le haut de la console
        private string left_padding = new string(' ', 38); // Distance entre le jeu d'échec et le coté gauche de la console


        // Couleur du joueur/de la pièce
        private enum Color
        {
            BLACK,
            WHITE,
        }

        // Type de pièce
        private enum PieceType
        {
            NONE = 0,
            KING,
            QUEEN,
            BISHOP,
            KNIGHT,
            ROOK,
            PAWN
        };

        private class Player
        {
            public Color player_color; // Sa couleur
            public Piece[] piece_array = new Piece[16]; // Ses pièces
            public bool kingIsInCheck = false; // Si son roi est en échec


            // Pour s'assurer que la copie ne référence rien de notre variable originale lorsqu'on va simuler les mouvements du future
            public Player Copy()
            {
                Player copy = new Player(player_color);
                copy.kingIsInCheck = kingIsInCheck;
                copy.piece_array = new Piece[piece_array.Length];
                for (int index = 0; index < piece_array.Length; ++index)
                {
                    copy.piece_array[index] = new Piece(piece_array[index].type, piece_array[index].color, piece_array[index].position);
                    copy.piece_array[index].isInGame = piece_array[index].isInGame;
                }
                return copy;
            }

            public Player(Color c_player_color)
            {
                player_color = c_player_color;
                int backLine, frontLine;
                if (player_color == Color.BLACK)
                {
                    backLine = 0;
                    frontLine = 1;
                }
                else
                {
                    backLine = 7;
                    frontLine = 6;
                }

                // Créer les pieces mineurs, majeurs et le roi et situe les dans la table
                int index = 0;

                piece_array[index] = new Piece(PieceType.ROOK, player_color, new Coordinate(backLine, index++));
                piece_array[index] = new Piece(PieceType.KNIGHT, player_color, new Coordinate(backLine, index++));
                piece_array[index] = new Piece(PieceType.BISHOP, player_color, new Coordinate(backLine, index++));
                piece_array[index] = new Piece(PieceType.QUEEN, player_color, new Coordinate(backLine, index++));
                piece_array[index] = new Piece(PieceType.KING, player_color, new Coordinate(backLine, index++));
                piece_array[index] = new Piece(PieceType.BISHOP, player_color, new Coordinate(backLine, index++));
                piece_array[index] = new Piece(PieceType.KNIGHT, player_color, new Coordinate(backLine, index++));
                piece_array[index] = new Piece(PieceType.ROOK, player_color, new Coordinate(backLine, index++));

                // Créer les pions
                while (index < 16)
                    piece_array[index] = new Piece(PieceType.PAWN, player_color, new Coordinate(frontLine, index++ % 8));
            }
        }

        private class Piece
        {

            public PieceType type; // Type de pièce
            public Color color; // Sa couleur
            public Hashtable hash_avaiableMoves; // Pour verifier plus vite si un mouvement fait partie de la table des mouvements valides
            public List<Coordinate> list_avaiableMoves; // Il sera utilisé lorsqu'on peut verifier que chaque mouvement est legale à 100%

            public Coordinate position; // Sa position dans la table
            public bool isInGame = true; // Si la piece est vivante ou morte.
            public bool pieceMoved = false; // Juste pour voir si la piece peut castle.
            public Piece(PieceType c_type, Color c_color, Coordinate c_position)
            {
                type = c_type;
                color = c_color;
                position = c_position;
            }
        }

        struct Coordinate
        {
            // Pas trop à dire...
            public int row;
            public int col;

            public Coordinate(int c_row, int c_col)
            {
                row = c_row;
                col = c_col;
            }
        }

        // Cellule du jeux
        private class Cell
        {
            private bool isEmpty; // Si il n'y a pas de pièce qui habite dedans.
            public Piece piece; // La pièce qu'il contient

            public Cell Copy()
            {
                return (Cell)this.MemberwiseClone();
            }

            public Cell(int row, int col)
            {
                isEmpty = true;
            }

            public bool IsEmpty() { return isEmpty; }

            public void ClearCell()
            {
                isEmpty = true;
            }

            // On référence la pièce car toute modification de cette pièce sera lié à la table de pièce du joueur
            public void SetCell(ref Piece c_piece)
            {
                piece = c_piece;
                isEmpty = false; // Comme il y a une pièce dans la cellule, la cellule n'est plus vide
            }
        }

        public Chess(bool isLAN = false, bool player_is_white = true)
        {
            gameIsLAN = isLAN;
            if (gameIsLAN)
                if (player_is_white)
                    userColor = Color.WHITE;
                else
                    userColor = Color.BLACK;

            InitializeCells(ref _chessboard);
            deadPieceTableWhite = new List<PieceType>();
            deadPieceTableBlack = new List<PieceType>();
        }


        public void Play()
        {

            PrintVersion(); // Info de la version du code et bla bla je ne sais pas coment le faire d'une manière professionel :)

            PlacesPiecesOnBoard(); // Mets les pièces dans notre jeu
            DrawGrid(); // Dessine la table
            DrawOriginalPieceState(); // Dessine les pieces dans leurs places originaux

            Console.CursorVisible = false; // Cache le cursor.

            Thread THREAD_runGame = new Thread(THREAD_RunGame); // Créer un thread qui va trouver la position de la souris
            THREAD_trackCursor.Start(); // Démarre le thread sur situer la position de la souris
            THREAD_runGame.Start(); // Démarre le jeu

            while (!isGameOver) ; // Tant que le jeu n'est pas fini, attend
            THREAD_trackCursor.Abort(); // Arrête de track la souris
            EndGameDisplay(); // La fin du jeu: échec et maths ou un draw
        }

        private void CheckmateAnimation()
        {
            Audio.Checkmate();
            Piece king = _player_turn == Color.WHITE ? _player_white.piece_array[kingIndex] : _player_black.piece_array[kingIndex];
            int col = CalculatePosColViaCell(king.position.col);
            int row = CalculatePosRowViaCell(king.position.row);
            Console.SetCursorPosition(col, row);
            Graphics.AnimateText("THE KING DIED.", 170);

        }

        private void StalemateAnimation()
        {
            Audio.Stalemate();
            Thread.Sleep(3250);

        }

        private void EndGameDisplay()
        {
            if (!isStalemate)
                CheckmateAnimation();
            else
                StalemateAnimation();

            Audio.Endgame(); // Fait un très beau bruit
            // Dessin de la boîte
            Console.ForegroundColor = ConsoleColor.DarkGray;
            string wipeLine = new string(' ', 70);
            string border = new string('░', 74);

            Console.SetCursorPosition(left_padding.Length + 5, top_padding + 9);
            Console.WriteLine(border);
            for (int line = 0; line < 10; ++line)
            {
                Console.CursorLeft = left_padding.Length + 5;
                Console.WriteLine("░░" + wipeLine + "░░");
            }
            Console.CursorLeft = left_padding.Length + 5;
            Console.WriteLine(border);
            //

            // Le texte
            string playerWhoHadTheLastMove = (_player_turn == Color.WHITE ? "Black" : "White");
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(left_padding.Length + 25, top_padding + 12);
            if (!isStalemate)
            {
                // Checkmate
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Graphics.AnimateText(" CHECKMATE! ", 0, 1000);

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Graphics.AnimateText(" " + playerWhoHadTheLastMove + " wins.", 200, 4000);
            }
            else
            {
                // Stalemate
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Graphics.AnimateText(" GAME OVER. ", 0, 1000);

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Graphics.AnimateText(" " + playerWhoHadTheLastMove + " landed a stalemate...", 100, 4000);
            }
            //
            Console.WriteLine('\n');
            Console.SetCursorPosition(55, Console.CursorTop);
            Thread.Sleep(4000);
            Environment.Exit(0);

        }

        private void SwitchPlayerTurns()
        {
            if (_player_turn == Color.WHITE)
                _player_turn = Color.BLACK;
            else
                _player_turn = Color.WHITE;
        }


        // Ecrit à gauche c'est le tour à qui.
        private void SayWhoIsNextToPlay()
        {
            Console.CursorVisible = false;
            const int pos_X = 11;
            const int pos_y = 3;
            Console.SetCursorPosition(pos_y, pos_X);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            if (gameIsLAN)
            {
                if (_player_turn == userColor)
                {
                    Console.Write("Player turn: You                   ");
                    Console.SetCursorPosition(3, 14);
                    Console.WriteLine("                       ");
                }
                else
                {
                    Console.Write("Player turn: " + PlayLan.player2Username);
                    Console.SetCursorPosition(3, 14);
                    Console.WriteLine("Waiting for oppoment...");
                }
            }
            else
            {
                if (_player_turn == Color.WHITE)
                    Console.Write("Player turn: White");
                else
                    Console.Write("Player turn: Black");
            }
        }

        // Je veux juste moins restranscrire, cette fonction fait l'écriture plus rapide.
        private ref Cell SelectCell(Coordinate position)
        {
            // Si on regarde la cellule du vrai du, ou du jeu simulé
            if (isSimulatingMove)
                return ref _mimicChessBoard[position.row, position.col];
            else
                return ref _chessboard[position.row, position.col];
        }

        // On va faire une copie du jeu, au cas qu'on veut retourner à l'originale
        private void CopyStringBoard()
        {
            // On utilise un StringBuilder car on va le modifier
            _mimicStringBoard = new StringBuilder[_stringBoard.Length];
            int lineIndex = 0;
            foreach (string line in _stringBoard)
            {
                _mimicStringBoard[lineIndex++] = new StringBuilder(line); // Copie un par un les lignes
            }

        }


        private static void LAN_SendMove(Coordinate start, Coordinate end, Coordinate stringBoardModify)
        {
            // Envoi le mouvement de la pièce, ainsi que le _stringBoardModify pour edit le _stringBoard
            string message = " " + start.row + "\n" + start.col + "\n" + end.row + "\n" + end.col + "\n" + stringBoardModify.row + "\n" + stringBoardModify.col;
            NetworkStream moveStream = PlayLan.client.GetStream();
            byte[] sendMessage = ASCIIEncoding.ASCII.GetBytes(message);

            try
            {
                moveStream.Write(sendMessage, 0, sendMessage.Length);
            }
            catch
            {
                Console.SetCursorPosition(3, 14);
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.Write("Oppoment Disconnected...");
                Thread.Sleep(4000);
                Environment.Exit(0);
            }
        }

        private void LAN_WaitForOppomentMove()
        {
            // Check le stream
            NetworkStream moveStream = PlayLan.client.GetStream();
            // Get le length du message
            byte[] oppomentMove = new byte[PlayLan.client.ReceiveBufferSize];
            try
            {
                // Get the message
                int bytesRead = moveStream.Read(oppomentMove, 0, PlayLan.client.ReceiveBufferSize);
            }
            catch
            {
                Console.SetCursorPosition(3, 14);
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.Write("Oppoment Disconnected...");
                Thread.Sleep(4000);
                Environment.Exit(0);
            }

            LAN_SetCoordinates(oppomentMove);
            LAN_MoveGame();
        }

        private void LAN_MoveGame()
        {
            // MIMIQUE LES ACTIONS POUR GARDER L'ETAT DU JEU EN SYNC
            Coordinate ClearCell = new Coordinate(_startCellPosition.row * 6 + 1, _startCellPosition.col * 10 + 1); // Position de la cellule dans le string
            CopyStringBoard();
            // Savoir si le dessin arrière est noir ou blanc
            string[] checkerType = ((_startCellPosition.row + _startCellPosition.col) % 2 == 0) ? Graphics.emptyBlackGraphics : Graphics.emptyWhiteGraphics;
            // Dessine l'arrière plan de la piece sur le mimicStringBoard
            for (int index = 0; index < 5; ++index)
            {
                for (int rowIndex = 0; rowIndex < 5; ++rowIndex)
                    for (int colIndex = 0; colIndex < 9; ++colIndex)
                        _mimicStringBoard[_stringBoardPosition.row + rowIndex][_stringBoardPosition.col + colIndex - 1] = checkerType[rowIndex][colIndex];
            }
            GetDrawingCoordinate();

            // Bouege la pièce
            Audio.MovePiece(); // Fait du bruit
            AbstractMovePiece(); // Bouge le dans notre table
            ConsoleMovePiece(); // Dessine la modification

            SwitchPlayerTurns(); // Change de joueur
            GenerateLegalMoveForPlayerTurn(); // Trouve tout les mouvement légales
            VerifyCheckState(); // Regarde si le roi est en échec
            if (IsCheckmate())// Si échec ou on draw
            {
                //Audio.Checkmate(); // Fait un très beau bruit
                isGameOver = true;  // Le jeu a fini
                return; // Sors du thread
            }

            DrawOnMimicString(); // Dessine le nouvel arrière plan sur notre string de plan arrière
            _stringBoard = ConvertBuilderArrayToStringArray(_mimicStringBoard); // Sauvegarde la modification dans notre _stringBoard

            SayWhoIsNextToPlay();
        }

        private void LAN_SetCoordinates(byte[] movement)
        {
            MemoryStream stream = new MemoryStream(movement);
            StreamReader reader = new StreamReader(stream);
            _startCellPosition = new Coordinate(Convert.ToInt32(reader.ReadLine()), Convert.ToInt32(reader.ReadLine()));
            _endCellPosition = new Coordinate(Convert.ToInt32(reader.ReadLine()), Convert.ToInt32(reader.ReadLine()));
            _stringBoardPosition = new Coordinate(Convert.ToInt32(reader.ReadLine()), Convert.ToInt32(reader.ReadLine()));
        }
        // RUN sur un thread
        public void THREAD_RunGame()
        {
            GenerateLegalMoveForPlayerTurn(); // Regarde tout les movements légales du joueur
            SayWhoIsNextToPlay(); // Dit c'est à qui de jouer
            while (true) // break par un check ou un stalemate
            {
                if (gameIsLAN)
                    if (_player_turn != userColor)
                        LAN_WaitForOppomentMove();
                // FIRST PRESS LMB
                while (RecordMouse.isMouseHeld == 0x01) // Rentre dans ce loop le moment qu'il va peser le left mouse button (LMB)
                {
                    _startCellPosition = LocateCell(); // Situe dans quel cellule de la table est-ce que la souris se situe

                    /*
                     * EXPLICATION: _pivotPoint
                     * Lorsqu'on va animer la piece qui bouge, selon la position de notre souris, je veux que le dessin de la pièce
                     * ne soit pas commencer par la position de la souris, mais selon le pixel où la souris a clique.
                     * Tentative de l'explication:
                     * P = Position du pivot = Position où la souris a cliqué la pièce
                     * | et - = différence entre le coin W et le pivot. Donc | = axe Y, - = axe X
                     * 
                     *  XXX█XXX    W  █ |          W| █  
                     *  X██X██X     ██ █|           |█ ██ 
                     *  XX█X█XX      █ █|           |█ █  
                     *  XX█K█XX    -----P          -P█K█ 
                     *  X█████X     █████           █████ 
                    */
                    _pivotPoint = new Coordinate // J'ai un beau pivot ici pour dessiner la piece qui bougera.
                        (
                            _lastCapturedPosition.row - (_startCellPosition.row) * 6 - 10,
                            _lastCapturedPosition.col - (_startCellPosition.col) * 10 - left_padding.Length - 1
                        );

                    int draw_row = _lastCapturedPosition.row - _pivotPoint.row + 1; // Dessine la pièce relativement au pivot point Y
                    int draw_col = _lastCapturedPosition.col - _pivotPoint.col + 1; // Dessine la pièce relativement au pivot point X

                    /*
                     * EXPLICATION: _lastDraw, _stringBoardPosition, _stringBoard,
                     * Malheureusement, cmd n'a qu'un ''layer'' de dessin. C'est à dire, si on veut
                     * faire semblant que la pièce flotte sur la table d'échecs, il faudra redessiner ce qu'il y a en arrière de la pièce.
                     * Ainsi, il faut avoir un moyen de savoir ce qu'il faut redessiner en arrièe de la pièce.
                     *
                     *  .Pièce qui bouge        .Arrière de la pièce (partie de la table d'échecs)
                     *  "@   ┃▒▒"               "@   ┃▒▒"
                     *  " █████▒"               "    ┃▒▒"
                     *  "@█████@"               "@@  ┃▒@"
                     *  "━━███━━"               "━━━━┿━━"
                     *  "███████"               "▒▒▒ ┃  "
                    */

                    _lastDrawn = new Coordinate(draw_row, draw_col);
                    _stringBoardPosition = new Coordinate(draw_row - top_padding, draw_col - left_padding.Length); // Get Coordinate of place to draw piece inside our string[]

                    try
                    { isHoldingPiece = !_chessboard[_startCellPosition.row, _startCellPosition.col].IsEmpty(); } // Esssaye de prendre la piece que la personne a cliqué sur.
                    catch
                    { isHoldingPiece = false; }; // Si ça rate, on fait qu'elle n'a cliquer sur rien

                    CopyStringBoard(); // Copier l'arriere plan

                    // Dessine l'arriere plan de la piece qui a été prise
                    if (isHoldingPiece)
                    {

                        Coordinate ClearCell = new Coordinate(_startCellPosition.row * 6 + 1, _startCellPosition.col * 10 + 1); // Position de la cellule dans le string

                        // Savoir si le dessin arrière est noir ou blanc
                        string[] checkerType = ((_startCellPosition.row + _startCellPosition.col) % 2 == 0) ? Graphics.emptyBlackGraphics : Graphics.emptyWhiteGraphics;
                        // Dessine l'arrière plan de la piece sur le mimicStringBoard
                        for (int index = 0; index < 5; ++index)
                        {
                            for (int rowIndex = 0; rowIndex < 5; ++rowIndex)
                                for (int colIndex = 0; colIndex < 9; ++colIndex)
                                    _mimicStringBoard[_stringBoardPosition.row + rowIndex][_stringBoardPosition.col + colIndex - 1] = checkerType[rowIndex][colIndex];
                        }
                        GetDrawingCoordinate();

                        // Tant qu'on appui sur l'LMB
                        while (RecordMouse.isMouseHeld == 0x1)
                        {
                            // Tant qu'elle tient une pièce ainsi que la bonne couleur de pièce
                            if (isHoldingPiece)
                            {
                                if (_chessboard[_startCellPosition.row, _startCellPosition.col].piece.color == _player_turn)
                                {
                                    Coordinate currentMousePosition = new Coordinate(RecordMouse.row, RecordMouse.col); // Prend les coordonées de la souris.

                                    // fait rien si la souris n'a pas été bougé
                                    if (currentMousePosition.row == _lastCapturedPosition.row && currentMousePosition.col == _lastCapturedPosition.col)
                                        continue;
                                    else
                                        _lastCapturedPosition = currentMousePosition;


                                    RedrawBoardPortion(); // Redessine la partie du jeu qui était 
                                    GetDrawingCoordinate(); // Prend les coordonés pour dessiner la pièce
                                    RedrawPiece(); // Redessine la pièce

                                    // Garde la valeur de la position de la piece desinné pour ensuite pouvoir redessiner l'arrière plan sur lui.
                                    _lastDrawn = new Coordinate(_nextDrawConsole.row, _nextDrawConsole.col);
                                }
                            }
                        }

                        _endCellPosition = LocateCell(true); // Situe dans quel cellule de la table est - ce que la souris se situe
                        RedrawBoardPortion(); // Fixer des petites erreurs si la souris a pris aucune pièce



                        // Action quand le mouvement est tout a fait legal.
                        if (IsNotSameCell() && IsMovementNotOutOfBounds() && isHoldingPiece)
                        {
                            Cell startingCell = SelectCell(_startCellPosition);
                            if (startingCell.piece.color == _player_turn)
                            {
                                // Si le mouvement est véritablement légal
                                if (PieceCanBeMovedThere(startingCell.piece, _endCellPosition))
                                {
                                    if (gameIsLAN)
                                        LAN_SendMove(_startCellPosition, _endCellPosition, _stringBoardPosition);

                                    // Bouege la pièce
                                    Audio.MovePiece(); // Fait du bruit
                                    AbstractMovePiece(); // Bouge le dans notre table
                                    ConsoleMovePiece(); // Dessine la modification

                                    SwitchPlayerTurns(); // Change de joueur
                                    GenerateLegalMoveForPlayerTurn(); // Trouve tout les mouvement légales
                                    VerifyCheckState(); // Regarde si le roi est en échec
                                    if (IsCheckmate())// Si échec ou on draw
                                    {
                                        isGameOver = true;  // Le jeu a fini
                                        return; // Sors du thread
                                    }

                                    DrawOnMimicString(); // Dessine le nouvel arrière plan sur notre string de plan arrière
                                    _stringBoard = ConvertBuilderArrayToStringArray(_mimicStringBoard); // Sauvegarde la modification dans notre _stringBoard

                                    SayWhoIsNextToPlay();
                                    continue; // Évite DrawOnCell() en bas.
                                }
                            }

                        }
                        DrawOnCell(_startCellPosition.row, _startCellPosition.col); // Redessine la pièce dans sa position original, si le mouvement était illégal.
                    }
                    else
                        while (RecordMouse.isMouseHeld == 0x1) ; // Si le premier click n'était pas sur une bonne pièce, attend que la personne relache l'LMB
                }
            }
        }

        // Spécéfique pour dessiner les red flags sur les roi
        private void DrawRedFlagOnMimic(Coordinate cell, string[] drawing)
        {
            Coordinate coordDraw = new Coordinate(cell.row * 6 + 1, cell.col * 10 + 3); // Position de la cellule dans le board

            char piece_background = (_endCellPosition.row + _endCellPosition.col) % 2 == 0 ? '▒' : ' ';
            for (int rowIndex = 0; rowIndex < 5; ++rowIndex)
                for (int colIndex = 0; colIndex < _piece_width; ++colIndex)
                    if (drawing[rowIndex][colIndex] != 'X')
                        _mimicStringBoard[coordDraw.row + rowIndex][coordDraw.col + colIndex - 1] = drawing[rowIndex][colIndex];
                    else
                        _mimicStringBoard[coordDraw.row + rowIndex][coordDraw.col + colIndex + 1] = piece_background;
        }
        private void VerifyCheckState()
        {
            // Regarde si les jouers sont en échecs
            _player_white.kingIsInCheck = IsKingOnCheck(_player_white, _player_black);
            _player_black.kingIsInCheck = IsKingOnCheck(_player_black, _player_white);

            // Player white en échec
            if (_player_white.kingIsInCheck)
            {
                whiteWasOnCheck = true;
                Audio.Check(); // Fait un beau bruit
                DrawRedFlagOnMimic(_player_white.piece_array[kingIndex].position, Graphics.kingWhiteCheckGraphics); // Dessiner le flag rouge sur l'arrire plan
                DrawOnCell(_player_white.piece_array[kingIndex].position.row, _player_white.piece_array[kingIndex].position.col); // Dessine le flag rouge sur console
            }
            else if (whiteWasOnCheck) // Pour éviter de stresser le redessinage du roi
            {
                WipeRedFlagOnKing(_player_white); // Enlève le red flag
                whiteWasOnCheck = false;
            }

            // Player black en échec
            if (_player_black.kingIsInCheck)
            {
                blackWasOnCheck = true;
                Audio.Check(); // Fait un beau bruit aussi
                DrawRedFlagOnMimic(_player_black.piece_array[kingIndex].position, Graphics.kingBlackCheckGraphics); // Dessiner le flag rouge sur l'arrire plan
                DrawOnCell(_player_black.piece_array[kingIndex].position.row, _player_black.piece_array[kingIndex].position.col); // Dessine le flag rouge sur console
            }
            else if (blackWasOnCheck) // Pour éviter de stresser le redessinage du roi
            {
                WipeRedFlagOnKing(_player_black); // Enlève le red flag
                blackWasOnCheck = false;
            }
        }
        private bool IsCheckmate()
        {
            bool atleastOnePieceCanMove = false;
            Player player = (_player_turn == Color.WHITE ? _player_white : _player_black); // Quel joueur joue
            for (int index = 0; index < player.piece_array.Length; ++index) // Va au travers des pièces du joeur
            {
                Piece piece = player.piece_array[index]; // Choisit la pièce
                if (piece.type == PieceType.KING || !piece.isInGame) continue; // Skip si c'est le roi, on va voir plus tard
                if (piece.hash_avaiableMoves.Count != 0) // Si la piece a au moins un mouvement, ca veut dire qu'il n'a pas encore de checkmate/stalemate
                {
                    atleastOnePieceCanMove = true; // Donc oui, au moins une piece peut bouger
                    break; // Casse le loop
                }
            }

            if (!atleastOnePieceCanMove) // Si aucune autre pièce peut bouger
            {
                // Et si le roi n'a aussi aucun mouvement, le jeu est fini.
                if (player.piece_array[kingIndex].hash_avaiableMoves.Count == 0)
                {
                    isStalemate = !player.kingIsInCheck;
                    return true;
                }
            }
            return false;
        }

        private void DrawChoosePieceDisplay()
        {
            // Dessine l'interface
        }
        private void THREAD_ChoosePiece()
        {
            // Interface qui permet de choisir si le pion devient une reine, une tour, un fou ou un cheval
        }

        private string[] ConvertBuilderArrayToStringArray(StringBuilder[] array)
        {
            string[] newArray = new string[array.Length];

            for (int index = 0; index < array.Length; ++index)
                newArray[index] = array[index].ToString();

            return newArray;

        }


        private void DrawOnMimicString(Coordinate cell)
        {
            Coordinate drawOnMimic = new Coordinate(cell.row * 6 + 1, cell.col * 10 + 1); // coordonate pour dessiner sur l'array du _stringBoard
            int locatePieceInArray = (SelectCell(cell).piece.color == Color.BLACK ? 5 : -1); // cherche c'est quel piece qu'il faut dessiner
            int LOC= (int)SelectCell(cell).piece.type; // prend l'index du dessin de la pièce
            string[] piece = pieceMap[locatePieceInArray + LOC]; // prend le dessin de la pièce

            if ((SelectCell(cell).IsEmpty()))
                piece = ((cell.row + cell.col) % 2 == 0 ? Graphics.emptyBlackGraphics : Graphics.emptyWhiteGraphics); // regarde si son contour est black ou white

            char piece_background = (cell.row + cell.col) % 2 == 0 ? '▒' : ' '; // Je ne sais pas pourquoi je l'ai fait deux fois
            for (int rowIndex = 0; rowIndex < 5; ++rowIndex) // row du dessin
                for (int colIndex = 0; colIndex < 7; ++colIndex) // col du dessin
                {
                    char pixel = piece[rowIndex][colIndex];
                    if (pixel != 'X') // Si ce n'est pas un X
                        _mimicStringBoard[drawOnMimic.row + rowIndex][drawOnMimic.col + colIndex + 1] = piece[rowIndex][colIndex]; // Dessine le pixel
                    else // sinon
                        _mimicStringBoard[drawOnMimic.row + rowIndex][drawOnMimic.col + colIndex + 1] = piece_background; // dessine l'arriere
                }
        }

        // Même chose, mais celui=là c'est automatiquement.
        private void DrawOnMimicString()
        {
            Coordinate drawOnMimic = new Coordinate(_endCellPosition.row * 6 + 1, _endCellPosition.col * 10 + 1);
            int locatePieceInArray = (SelectCell(_endCellPosition).piece.color == Color.BLACK ? 5 : -1);
            int LOC = (int)SelectCell(_endCellPosition).piece.type;
            string[] piece = pieceMap[locatePieceInArray + LOC];

            char piece_background = (_endCellPosition.row + _endCellPosition.col) % 2 == 0 ? '▒' : ' ';
            for (int rowIndex = 0; rowIndex < 5; ++rowIndex)
                for (int colIndex = 0; colIndex < 7; ++colIndex)
                {
                    char pixel = piece[rowIndex][colIndex];
                    if (pixel != 'X')
                        _mimicStringBoard[drawOnMimic.row + rowIndex][drawOnMimic.col + colIndex + 1] = piece[rowIndex][colIndex];
                    else
                        _mimicStringBoard[drawOnMimic.row + rowIndex][drawOnMimic.col + colIndex + 1] = piece_background;
                }
        }

        // Dépendant de la position du cursor, la place pour dessiner la pièce, selon le pivot, varie.
        private void GetDrawingCoordinate()
        {
            int draw_row = _lastCapturedPosition.row - _pivotPoint.row + 1; // Dessine la pièce relativement au pivot Y
            int draw_col = _lastCapturedPosition.col - _pivotPoint.col + 1; // Dessine la pièce relativement au pivot X

            int string_locate_row = draw_row - top_padding; // Trouve la position pour le _stringBoard, position Y
            int string_locate_col = draw_col - left_padding.Length; // Trouve la position pour le _stringBoard, position X

            // Piece bypasses left side
            if (string_locate_col < 0)
            {
                string_locate_col = 0; // Then set it to 0
                draw_col = left_padding.Length;  // That's the left side of the board, on console
            }

            // Piece bypasses right side
            else if (string_locate_col + _piece_width >= _chessboard_string_width_max)
            {
                // Same logic here getting to lazy to write in french
                string_locate_col = _chessboard_string_width_max - 1 - _piece_width - 1;
                draw_col = _chessboard_string_width_max + left_padding.Length - _piece_width - 2;
            }

            // Piece bypasses top
            if (string_locate_row < 0)
            {
                string_locate_row = 0;
                draw_row = top_padding;
            }
            // Piece bypasses bottom
            else if (string_locate_row + _piece_height >= _chessboard_string_height_max)
            {
                string_locate_row = _chessboard_string_height_max - _piece_height;
                draw_row = _chessboard_string_height_max + top_padding - _piece_height;
            }

            _nextDrawConsole = new Coordinate(draw_row, draw_col); // Next place to draw on console
            _nextDrawString = new Coordinate(string_locate_row, string_locate_col); // Next place to draw on string
        }

        private void RedrawPiece()
        {
            // Self explanatory...
            Console.SetCursorPosition(_nextDrawConsole.col, _nextDrawConsole.row); 
            SelectPiece(_chessboard[_startCellPosition.row, _startCellPosition.col]);
        }
        private void RedrawBoardPortion()
        {
            string[] drawBoard = new string[5];
            for (int index = 0; index < 5; ++index) // Assign the place to redraw with an array of string from the _stringBoard
                drawBoard[index] = _mimicStringBoard[_nextDrawString.row + index].ToString().Substring(_nextDrawString.col, _piece_width);
            Console.SetCursorPosition(_lastDrawn.col, _lastDrawn.row); // set the cursor to the cell inside the console
            DrawGraphicsOnConsole(drawBoard); ; // Then draw it
        }

        private bool IsNotSameCell()
        {
            // Do not move the piece if he left it on the same cell
            return !(_startCellPosition.row == _endCellPosition.row && _startCellPosition.col == _endCellPosition.col);
        }

        private bool IsMovementNotOutOfBounds()
        {
            // Do not move the piece if it's not even in bounds
            return (_startCellPosition.row >= 0 && _startCellPosition.row <= 7 && _startCellPosition.col >= 0 && _startCellPosition.col <= 7) &&
                    (_endCellPosition.row >= 0 && _endCellPosition.row <= 7 && _endCellPosition.col >= 0 && _endCellPosition.col <= 7);
        }

        private bool IsNotOutOfBounds(Coordinate position)
        {
            // Celui la c'est used lorsqu'on regarde la légalité des mouvement, s'assurer que ça ne sorts pas dehors au Congolo
            return (position.row >= 0 && position.row <= 7 && position.col >= 0 && position.col <= 7);
        }


        private bool PieceCanBeMovedThere(Piece piece, Coordinate move)
        {
            // Chaque pièce a une table de mouvement légales, puis on regarde si le mouvement désiré est dans notre table
            return (piece.hash_avaiableMoves.Contains(move));
        }

        /// <summary>
        /// ////
        /// ////
        /// //////////////////////////////////////////
        /// Je vais m'endormir si je commente tout. Si vous avez des question demandez moi, je vais answer.
        /// Desoler si les langues sont mélangés
        /// //////////////////////////////////////////
        /// ////
        /// ////
        /// </summary>
        /// <param name="player"></param>

        private void WipeRedFlagOnKing(Player player)
        {
            Coordinate kingPosition = player.piece_array[kingIndex].position;
            DrawOnMimicString(kingPosition);
            Console.SetCursorPosition(CalculatePosColViaCell(kingPosition.col), CalculatePosRowViaCell(kingPosition.row));

            --Console.CursorLeft;
            if ((kingPosition.row + kingPosition.col) % 2 == 0)
                DrawEmptyCellOnConsole(Graphics.emptyBlackGraphics);
            else
                DrawEmptyCellOnConsole(Graphics.emptyWhiteGraphics);

            Console.SetCursorPosition(CalculatePosColViaCell(kingPosition.col), CalculatePosRowViaCell(kingPosition.row));
            Cell cellToDraw = _chessboard[kingPosition.row, kingPosition.col];
            SelectPiece(cellToDraw);
            Console.ForegroundColor = ConsoleColor.White;
        }
        private bool IsKingOnCheck(Player playerOnCheck, Player playerThrowingCheck)
        {
            Coordinate kingPosition = playerOnCheck.piece_array[kingIndex].position;
            foreach (Piece piece in playerThrowingCheck.piece_array)
            {
                if (piece.isInGame)
                    if (piece.hash_avaiableMoves.Contains(kingPosition))
                        return true;
            }

            return false;
        }

        private void Copy2D_Array(Cell[,] board, ref Cell[,] copy)
        {
            for (int x = 0; x < 8; ++x)
                for (int y = 0; y < 8; ++y)
                    copy[x, y] = board[x, y].Copy();
        }

        private void GenerateLegalMoveForPlayerTurn()
        {
            // ADD ALL PRESENT MOVES
            for (int index = 0; index < _player_white.piece_array.Length; ++index)
                if (_player_white.piece_array[index].isInGame)
                    GenerateLegalMovesForPiece(ref _player_white.piece_array[index]);

            for (int index = 0; index < _player_white.piece_array.Length; ++index)
                if (_player_black.piece_array[index].isInGame)
                    GenerateLegalMovesForPiece(ref _player_black.piece_array[index]);

            VerifyCastling();

            Player player = (_player_turn == Color.WHITE ? _player_white : _player_black);
            Player attacker = (_player_turn == Color.WHITE ? _player_black : _player_white);

            // Enlève les mouvements du future qui vont mener/laisser le king en check
            isSimulatingMove = true;
            for (int index = 0; index < player.piece_array.Length; ++index)
            {
                Piece piece = player.piece_array[index];
                if (piece.isInGame)
                {
                    for (int moveNumber = 0; moveNumber < piece.list_avaiableMoves.Count; ++moveNumber)
                    {
                        // Copy players
                        Player mimic_player = player.Copy();
                        Player mimic_attacker = attacker.Copy();

                        // Copy board
                        _mimicChessBoard = new Cell[8, 8];
                        InitializeCells(ref _mimicChessBoard);
                        PlacesPiecesOnBoard(ref _mimicChessBoard, ref mimic_player, ref mimic_attacker);


                        //Move piece in copied board
                        SimulateAbstractMovePiece(ref _mimicChessBoard, piece.position, piece.list_avaiableMoves[moveNumber]);


                        //Generate legal moves for attacker
                        for (int pieceIndex = 0; pieceIndex < _player_white.piece_array.Length; ++pieceIndex)
                            if (mimic_attacker.piece_array[pieceIndex].isInGame)
                                GenerateLegalMovesForPiece(ref mimic_attacker.piece_array[pieceIndex]);

                        //Check if the move leads to check
                        if (IsKingOnCheck(mimic_player, mimic_attacker))
                            player.piece_array[index].hash_avaiableMoves.Remove(piece.list_avaiableMoves[moveNumber]);
                    }
                }
            }
            isSimulatingMove = false;

        }

        private void GenerateLegalMovesForPiece(ref Piece current_piece)
        {
            switch (current_piece.type)
            {
                case PieceType.PAWN:
                    LegalPawnMoves(ref current_piece);
                    break;
                case PieceType.KING:
                    LegalKingMoves(ref current_piece);
                    break;
                case PieceType.KNIGHT:
                    LegalKnightMoves(ref current_piece);
                    break;
                case PieceType.ROOK:
                    LegalRookMoves(ref current_piece);
                    break;
                case PieceType.BISHOP:
                    LegalBishopMoves(ref current_piece);
                    break;
                case PieceType.QUEEN:
                    LegalQueenMoves(ref current_piece);
                    break;
                default:
                    LegalPawnMoves(ref current_piece);
                    break;
            }
        }

        private void VerifyCastling()
        {
            Player player = _player_turn == Color.WHITE ? _player_white : _player_black;
            Player attacker = _player_turn == Color.WHITE ? _player_black : _player_white;

            Piece king = player.piece_array[kingIndex];
            if (king.pieceMoved) return;

            int row = king.position.row;

            // LEFT SIDE CASLTING
            bool areCellsEmpty = true;
            for (int col = 3; col > 0; --col)
                if (!_chessboard[row, col].IsEmpty())
                {
                    areCellsEmpty = false;
                    break;
                }
            if (areCellsEmpty)
                if (!_chessboard[row, 0].IsEmpty())
                    if (!_chessboard[row, 0].piece.pieceMoved)
                    {
                        Console.SetCursorPosition(0, 16);
                        Coordinate slide = new Coordinate(row, 3);
                        Coordinate land = new Coordinate(row, 2);
                        bool isSlideLegal = true;

                        foreach (Piece piece in attacker.piece_array)
                            if (piece.hash_avaiableMoves.Contains(slide) || piece.hash_avaiableMoves.Contains(land))
                            {
                                isSlideLegal = false;
                                break;
                            }
                        if (isSlideLegal)
                        {
                            Coordinate castleLeft = new Coordinate(row, 2);
                            king.hash_avaiableMoves.Add(castleLeft, castleLeft);
                        }

                    }

            // RIGHT SIDE CASLTING
            areCellsEmpty = true;
            for (int col = 5; col >= 7; ++col)
                if (!_chessboard[row, col].IsEmpty())
                {
                    areCellsEmpty = false;
                    break;
                }
            if (areCellsEmpty)
                if (!_chessboard[row, 7].IsEmpty())
                    if (!_chessboard[row, 7].piece.pieceMoved)
                    {
                        Coordinate slide = new Coordinate(row, 5);
                        Coordinate land = new Coordinate(row, 6);
                        bool isSlideLegal = true;
                        foreach (Piece piece in attacker.piece_array)
                            if (piece.hash_avaiableMoves.Contains(slide) || piece.hash_avaiableMoves.Contains(land))
                            {
                                isSlideLegal = false;
                                break;
                            }
                        if (isSlideLegal)
                        {
                            Coordinate castleLeft = new Coordinate(row, 6);
                            king.hash_avaiableMoves.Add(castleLeft, castleLeft);
                        }

                    }
        }
        private void LegalPawnMoves(ref Piece current_piece)
        {

            current_piece.hash_avaiableMoves = new Hashtable();
            current_piece.list_avaiableMoves = new List<Coordinate>();

            int moveDirection = (current_piece.color == Color.BLACK ? 1 : -1);

            Coordinate move_forward = new Coordinate(current_piece.position.row + 1 * moveDirection, current_piece.position.col);
            Coordinate move_doubleForward = new Coordinate(current_piece.position.row + 2 * moveDirection, current_piece.position.col);

            Coordinate[] move_captures = new Coordinate[]
            {
                new Coordinate(current_piece.position.row + 1 * moveDirection, current_piece.position.col - 1), // CAPTURE LEFT
                new Coordinate(current_piece.position.row + 1 * moveDirection, current_piece.position.col + 1) // CAPTURE RIGHT
            };

            if (IsNotOutOfBounds(move_forward))
                if (SelectCell(move_forward).IsEmpty())
                {
                    current_piece.hash_avaiableMoves.Add(move_forward, move_forward);
                    current_piece.list_avaiableMoves.Add(move_forward);

                    if (IsNotOutOfBounds(move_doubleForward))
                    {
                        int before_back_line = (current_piece.color == Color.BLACK ? 1 : 6);
                        if (SelectCell(move_doubleForward).IsEmpty() && current_piece.position.row == before_back_line)
                        {
                            current_piece.hash_avaiableMoves.Add(move_doubleForward, move_doubleForward);
                            current_piece.list_avaiableMoves.Add(move_doubleForward);
                        }
                    }
                }


            foreach (Coordinate move in move_captures)
            {
                if (IsNotOutOfBounds(move))
                {
                    Cell moveCell = SelectCell(move);
                    if (!moveCell.IsEmpty())
                        if (moveCell.piece.color != current_piece.color)
                        {
                            current_piece.hash_avaiableMoves.Add(move, move);
                            current_piece.list_avaiableMoves.Add(move);
                        }
                }
            }
        }

        private void LegalKingMoves(ref Piece current_piece)
        {
            current_piece.hash_avaiableMoves = new Hashtable();
            current_piece.list_avaiableMoves = new List<Coordinate>();

            for (int move_row = -1; move_row <= 1; ++move_row)
                for (int move_col = -1; move_col <= 1; ++move_col)
                {
                    Coordinate moveCoordinate = new Coordinate(current_piece.position.row + move_row, current_piece.position.col + move_col);
                    if (IsNotOutOfBounds(moveCoordinate))
                    {
                        Cell moveCell = SelectCell(moveCoordinate);
                        if (!moveCell.IsEmpty())
                            if (moveCell.piece.color == current_piece.color)
                                continue;

                        // Otherwise
                        current_piece.hash_avaiableMoves.Add(moveCoordinate, moveCoordinate);
                        current_piece.list_avaiableMoves.Add(moveCoordinate);
                    }
                }
        }

        private void LegalKnightMoves(ref Piece current_piece)
        {
            current_piece.hash_avaiableMoves = new Hashtable();
            current_piece.list_avaiableMoves = new List<Coordinate>();

            Coordinate[] all_moves = new Coordinate[]
            {
                new Coordinate(current_piece.position.row +2, current_piece.position.col +1),
                new Coordinate(current_piece.position.row +2, current_piece.position.col -1),
                new Coordinate(current_piece.position.row -2, current_piece.position.col +1),
                new Coordinate(current_piece.position.row -2, current_piece.position.col -1),
                new Coordinate(current_piece.position.row +1, current_piece.position.col +2),
                new Coordinate(current_piece.position.row +1, current_piece.position.col -2),
                new Coordinate(current_piece.position.row -1, current_piece.position.col +2),
                new Coordinate(current_piece.position.row -1, current_piece.position.col -2)
            };

            foreach (Coordinate moveCoordinate in all_moves)
            {
                if (IsNotOutOfBounds(moveCoordinate))
                {
                    Cell moveCell = SelectCell(moveCoordinate);
                    if (!SelectCell(moveCoordinate).IsEmpty())
                        if (SelectCell(moveCoordinate).piece.color == current_piece.color)
                            continue;

                    //Otherwise..
                    current_piece.hash_avaiableMoves.Add(moveCoordinate, moveCoordinate);
                    current_piece.list_avaiableMoves.Add(moveCoordinate);
                };
            }
        }

        private void LegalRookMoves(ref Piece current_piece)
        {
            current_piece.hash_avaiableMoves = new Hashtable();
            current_piece.list_avaiableMoves = new List<Coordinate>();

            LegalSingleAxeMove(ref current_piece, true, true);
            LegalSingleAxeMove(ref current_piece, true, false);
            LegalSingleAxeMove(ref current_piece, false, true);
            LegalSingleAxeMove(ref current_piece, false, false);
        }

        private void LegalSingleAxeMove(ref Piece current_piece, bool rowIsModified, bool isIncrementing)
        {
            int direction = (isIncrementing ? 1 : -1);
            int axis = (rowIsModified ? current_piece.position.row : current_piece.position.col);

            for (int move = axis + direction; move >= 0 && move <= 7; move = move + direction)
            {
                Coordinate moveCoordinate;
                if (rowIsModified)
                    moveCoordinate = new Coordinate(move, current_piece.position.col);
                else
                    moveCoordinate = new Coordinate(current_piece.position.row, move);

                if (SelectCell(moveCoordinate).IsEmpty())
                {
                    current_piece.hash_avaiableMoves.Add(moveCoordinate, moveCoordinate);
                    current_piece.list_avaiableMoves.Add(moveCoordinate);
                    continue; // Since it's empty, it can continue going towards it's direction
                }
                else if (SelectCell(moveCoordinate).piece.color != current_piece.color)
                {
                    current_piece.hash_avaiableMoves.Add(moveCoordinate, moveCoordinate);
                    current_piece.list_avaiableMoves.Add(moveCoordinate);
                }

                break;
            }
        }

        private void LegalBishopMoves(ref Piece current_piece)
        {
            current_piece.hash_avaiableMoves = new Hashtable();
            current_piece.list_avaiableMoves = new List<Coordinate>();

            LegalDiagonalMoves(ref current_piece, true, true);
            LegalDiagonalMoves(ref current_piece, true, false);
            LegalDiagonalMoves(ref current_piece, false, true);
            LegalDiagonalMoves(ref current_piece, false, false);
        }

        private void LegalDiagonalMoves(ref Piece current_piece, bool incrementRow, bool incrementCol)
        {
            int direction_row = (incrementRow ? 1 : -1);
            int direction_col = (incrementCol ? 1 : -1);

            for (int step = 1; ; ++step)
            {
                Coordinate moveCoordinate = new Coordinate(current_piece.position.row + step * direction_row, current_piece.position.col + step * direction_col);
                if (IsNotOutOfBounds(moveCoordinate))
                {
                    if (SelectCell(moveCoordinate).IsEmpty())
                    {
                        current_piece.hash_avaiableMoves.Add(moveCoordinate, moveCoordinate);
                        current_piece.list_avaiableMoves.Add(moveCoordinate);
                        continue; // Since it's empty, it can continue going towards it's direction
                    }
                    else if (SelectCell(moveCoordinate).piece.color != current_piece.color)
                    {
                        current_piece.hash_avaiableMoves.Add(moveCoordinate, moveCoordinate);
                        current_piece.list_avaiableMoves.Add(moveCoordinate);
                    }
                    break;
                }
                else
                    break;
            }
        }

        private void LegalQueenMoves(ref Piece current_piece)
        {
            current_piece.hash_avaiableMoves = new Hashtable();
            current_piece.list_avaiableMoves = new List<Coordinate>();

            LegalSingleAxeMove(ref current_piece, true, true);
            LegalSingleAxeMove(ref current_piece, true, false);
            LegalSingleAxeMove(ref current_piece, false, true);
            LegalSingleAxeMove(ref current_piece, false, false);

            LegalDiagonalMoves(ref current_piece, true, true);
            LegalDiagonalMoves(ref current_piece, true, false);
            LegalDiagonalMoves(ref current_piece, false, true);
            LegalDiagonalMoves(ref current_piece, false, false);
        }

        private Coordinate LocateCell(bool usePivotPoint = false)
        {
            int col = RecordMouse.col;
            int row = RecordMouse.row;
            if (usePivotPoint)
            {
                row -= (_pivotPoint.row + 1) / 2; // Draw Piece relative to pivot point Y
                col -= (_pivotPoint.col + 1) / 2; // Draw Piece relative to pivot point X
            }
            int col_select = (col - left_padding.Length) / 10;
            int row_select = (row - 10) / 6;

            _lastCapturedPosition = new Coordinate(row, col);
            return new Coordinate(row_select, col_select);
        }

        private void AbstractMovePiece()
        {
            ref Cell cell = ref SelectCell(_startCellPosition);
            ref Cell movement = ref SelectCell(_endCellPosition);


            // Piece gets eaten.
            if (!movement.IsEmpty())
            {
                movement.piece.isInGame = false;
                UpdateDeadPieceBoard(movement.piece.type);
                Audio.Capture();
            }

            // King is caslting
            if (cell.piece.type == PieceType.KING)
                if (_startCellPosition.col - _endCellPosition.col == 2) // King's horizontal movement is a length of 2
                {
                    ref Cell rookCell = ref _chessboard[_startCellPosition.row, 0];
                    ref Piece rook = ref rookCell.piece;
                    rook.pieceMoved = true;
                    _chessboard[_startCellPosition.row, 0].ClearCell();
                    _chessboard[_startCellPosition.row, 3].SetCell(ref rook);

                    // manually fix graphics and mimic
                    rook.position = new Coordinate(_startCellPosition.row, 3);
                    DrawOnMimicString(new Coordinate(_startCellPosition.row, 0));
                    DrawOnMimicString(new Coordinate(_startCellPosition.row, 3));
                    DrawOnCell(_startCellPosition.row, 0);
                    DrawOnCell(_startCellPosition.row, 3);

                }
                else if (_startCellPosition.col - _endCellPosition.col == -2)
                {
                    ref Cell rookCell = ref _chessboard[_startCellPosition.row, 7];
                    ref Piece rook = ref rookCell.piece;
                    rook.pieceMoved = true;
                    _chessboard[_startCellPosition.row, 7].ClearCell();
                    _chessboard[_startCellPosition.row, 5].SetCell(ref rook);

                    // manually fix graphics and mimic
                    rook.position = new Coordinate(_startCellPosition.row, 5);
                    DrawOnMimicString(new Coordinate(_startCellPosition.row, 7));
                    DrawOnMimicString(new Coordinate(_startCellPosition.row, 5));
                    DrawOnCell(_startCellPosition.row, 7);
                    DrawOnCell(_startCellPosition.row, 5);
                }

            // Pawn is in the other backline
            if (cell.piece.type == PieceType.PAWN)
                if (_endCellPosition.row == 0 || _endCellPosition.row == 7) // If movement ends at any backline
                {
                    cell.piece.type = PieceType.QUEEN;
                }

            ref Piece holdingPiece = ref cell.piece;
            cell.piece.pieceMoved = true;
            holdingPiece.position = _endCellPosition;


            cell.ClearCell(); // Clear Move Cell
            movement.SetCell(ref holdingPiece); // Move the piece
        }

        private void UpdateDeadPieceBoard(PieceType type)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            List<PieceType> table = _player_turn == Color.WHITE ? deadPieceTableWhite : deadPieceTableBlack;
            if (PieceIsAlreadyInTable(type))
            {
                int numTimesDead = CalculteTimesDead(type);
                Coordinate drawCounter = CalculaDrawDeadPieceCountPosition(type);
                Console.SetCursorPosition(drawCounter.col, drawCounter.row);
                Console.Write("x" + numTimesDead);
            }
            else
            {
                table.Add(type);
                Coordinate drawPiece = CalculateDrawDeadPiecePosition(table);
                Console.SetCursorPosition(drawPiece.col, drawPiece.row);
                DrawGraphicsOnConsole(pieceMap[(int)type + (_player_turn == Color.BLACK ? 0 : 6) - 1]);
            }
        }

        private Coordinate CalculateDrawDeadPiecePosition(List<PieceType> table)
        {
            int row = table.Count * (_piece_height + 2) + top_padding;
            int col = 8 + (_player_turn == Color.BLACK ? 1 : _piece_width + 3);
            return new Coordinate(row, col);
        }

        private Coordinate CalculaDrawDeadPieceCountPosition(PieceType piece)
        {
            int rowMultiplier = 0;
            List<PieceType> table = _player_turn == Color.WHITE ? deadPieceTableWhite : deadPieceTableBlack;
            while (table[rowMultiplier] != piece)
                ++rowMultiplier;

            int row = (rowMultiplier + 1) * (_piece_height + 2) + top_padding;
            int col = 5 + (_player_turn == Color.BLACK ? 3 + _piece_width + 1 : +_piece_width * 2 + 6);

            return new Coordinate(row, col);
        }

        private bool PieceIsAlreadyInTable(PieceType piece)
        {
            List<PieceType> table = _player_turn == Color.WHITE ? deadPieceTableWhite : deadPieceTableBlack;
            foreach (PieceType typeInTable in table)
                if (typeInTable == piece)
                    return true;
            return false;
        }

        private int CalculteTimesDead(PieceType piece)
        {
            int timesDead = 0;
            Player player = _player_turn == Color.BLACK ? _player_white : _player_black;
            foreach (Piece obj in player.piece_array)
                if (obj.type == piece)
                    if (!obj.isInGame)
                        ++timesDead;
            return timesDead;
        }


        private void SimulateAbstractMovePiece(ref Cell[,] board, Coordinate cellStart, Coordinate cellEnd)
        {
            ref Cell cell = ref SelectCell(cellStart);
            ref Cell movement = ref SelectCell(cellEnd);

            // Piece gets eaten.
            if (!movement.IsEmpty())
                movement.piece.isInGame = false;

            ref Piece holdingPiece = ref cell.piece;
            holdingPiece.position = cellEnd;


            cell.ClearCell(); // Clear Piece's original position
            movement.SetCell(ref holdingPiece); // Move the piece
        }

        private void ConsoleMovePiece()
        {
            DrawOnCell(_startCellPosition.row, _startCellPosition.col); // Draw (blank cell)
            DrawOnCell(_endCellPosition.row, _endCellPosition.col); // Draw (blank cell)
        }


        private void SelectPiece(Cell square)
        {
            // I'm hoping that using indexes (instead of a switch) will run this functions faster.
            int locatePieceInArray = -1;
            if (square.piece.color == Color.BLACK)
                locatePieceInArray += 6;

            int index = (int)square.piece.type;
            DrawGraphicsOnConsole(pieceMap[locatePieceInArray + index]);
            if (square.piece.type == PieceType.KING)
            {
                Player kingOwner = (square.piece.color == Color.WHITE ? _player_white : _player_black);
                if (kingOwner.kingIsInCheck)
                {
                    if (kingOwner.player_color == Color.WHITE)
                        DrawGraphicsOnConsole(Graphics.kingWhiteCheckGraphics);
                    else
                        DrawGraphicsOnConsole(Graphics.kingBlackCheckGraphics);
                }
            }
        }



        private int CalculatePosColViaCell(int col)
        {
            return left_padding.Length + col * 10 + 2;
        }

        private int CalculatePosRowViaCell(int row)
        {
            return row * 6 + top_padding + 1;
        }

        private void DrawOnCell(int row, int col)
        {

            Cell square = _chessboard[row, col];
            Console.SetCursorPosition(CalculatePosColViaCell(col) - 1, CalculatePosRowViaCell(row));
            if ((row + col) % 2 == 0)
                DrawEmptyCellOnConsole(Graphics.emptyBlackGraphics);
            else
                DrawEmptyCellOnConsole(Graphics.emptyWhiteGraphics);

            if (!square.IsEmpty())
            {
                Console.SetCursorPosition(CalculatePosColViaCell(col), CalculatePosRowViaCell(row));
                SelectPiece(square);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void DrawGrid()
        {
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            string topSkip = new string('\n', top_padding);
            string jumpToNextRow = new string('\n', 6);
            string frame = new string('━', 82);

            // Split rows
            Console.Write(topSkip + left_padding + Graphics.topSplit);
            for (int index = 1; index < 8; ++index)
                Console.Write(jumpToNextRow + left_padding + Graphics.rowSplit);
            Console.Write(jumpToNextRow + left_padding + Graphics.botSplit);

            // Split columns
            Console.SetCursorPosition(0, top_padding + 1);
            for (int cell = 0; cell < 8; ++cell)
            {
                string colSplitType;
                if (cell % 2 == 0)
                    colSplitType = Graphics.colSplitB;
                else
                    colSplitType = Graphics.colSplitW;
                for (int side = 0; side < 5; ++side)
                    Console.WriteLine(left_padding + colSplitType);
                Console.CursorTop += 1;
            }


        }

        private void DrawGraphicsOnConsole(string[] piece)
        {
            Console.CursorVisible = false;
            int piecePositionX = Console.CursorLeft;
            int piecePositionY = Console.CursorTop;
            foreach (string lineDrawing in piece)
            {
                for (int characterIndex = 0; characterIndex < lineDrawing.Length; ++characterIndex)
                {
                    char pixel = lineDrawing[characterIndex];
                    switch (pixel)
                    {
                        case 'X':
                            ++Console.CursorLeft;
                            break;
                        case '@':
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write('O');
                            break;
                        case '█':
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write('█');
                            break;
                        case 'R':
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write('▒');
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write(pixel);
                            break;
                    }
                }
                Console.WriteLine();
                Console.CursorLeft = piecePositionX;
            }
            Console.CursorTop = piecePositionY;
        }

        private void DrawEmptyCellOnConsole(string[] blankType)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;

            int piecePositionX = Console.CursorLeft;
            int piecePositionY = Console.CursorTop;
            foreach (string lineDrawing in blankType)
            {
                Console.WriteLine(lineDrawing);
                Console.CursorLeft = piecePositionX;
            }
        }

        private void DrawOriginalPieceState()
        {
            string padding = new string(' ', 20);
            Console.WriteLine(padding);
            for (int row = 0; row < _chessboard_width; ++row)
            {
                Console.Write(padding);
                for (int col = 0; col < _chessboard_height; ++col)
                    DrawOnCell(row, col);
                Console.WriteLine();
            }
            Console.WriteLine(padding);
        }
        private void InitializeCells(ref Cell[,] biard)
        {
            for (int row = 0; row < _chessboard_height; ++row)
                for (int col = 0; col < _chessboard_width; ++col)
                    biard[row, col] = new Cell(row, col);

        }

        private void PlacesPiecesOnBoard()
        {
            for (int index = 0; index < _player_white.piece_array.Length; ++index)
            {
                SelectCell(_player_white.piece_array[index].position).SetCell(ref _player_white.piece_array[index]);
                SelectCell(_player_black.piece_array[index].position).SetCell(ref _player_black.piece_array[index]);
            }
        }

        private void PlacesPiecesOnBoard(ref Cell[,] board, ref Player player1, ref Player player2)
        {
            for (int index = 0; index < player1.piece_array.Length; ++index)
            {
                ref Piece piece1 = ref player1.piece_array[index];
                ref Piece piece2 = ref player2.piece_array[index];
                if (piece1.isInGame)
                    board[piece1.position.row, piece1.position.col].SetCell(ref piece1);
                if (piece2.isInGame)
                    board[piece2.position.row, piece2.position.col].SetCell(ref piece2);
            }
        }

        private void PrintVersion()
        {
            Console.Title = version;
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n  RELEASE Version");
            foreach (string line in Graphics.inGameTitle)
            {
                Console.CursorLeft = left_padding.Length;
                Console.WriteLine(line);
            }
            string version_info = version;
            Console.SetCursorPosition(left_padding.Length + _stringBoard[0].Length - version_info.Length, top_padding - 1);
            Console.Write(version_info);
        }

        static string version = "ChessGUI 1.0.2";

    }
}
