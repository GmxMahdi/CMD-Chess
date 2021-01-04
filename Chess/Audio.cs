using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chess
{
    public static class Audio
    {
        static private System.Media.SoundPlayer audio_movePiece = new System.Media.SoundPlayer(Properties.Resources.movePiece);
        static private System.Media.SoundPlayer audio_capture = new System.Media.SoundPlayer(Properties.Resources.capture);
        static private System.Media.SoundPlayer audio_check = new System.Media.SoundPlayer(Properties.Resources.check);
        static private System.Media.SoundPlayer audio_checkmate = new System.Media.SoundPlayer(Properties.Resources.checkmate);
        static private System.Media.SoundPlayer audio_stalemate = new System.Media.SoundPlayer(Properties.Resources.stalemate);
        static private System.Media.SoundPlayer audio_endgame = new System.Media.SoundPlayer(Properties.Resources.endgame);

        static public void MovePiece()
        {
            audio_movePiece.Play();
        }

        static public void Capture()
        {
            audio_capture.Play();
        }

        static public void Check()
        {
            audio_check.Play();
        }

        static public void Checkmate()
        {
            audio_checkmate.Play();
        }

        static public void Stalemate()
        {
            audio_stalemate.Play();
        }
        static public void Endgame()
        {
            audio_endgame.Play();
        }
    }
}