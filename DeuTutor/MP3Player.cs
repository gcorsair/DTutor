using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DeuTutor
{
	public static class MP3Player
	{
		private static string _command;
		private static bool isOpen;
		[DllImport("winmm.dll")]
		private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

		[DllImport("winmm.dll")]
		public static extern bool PlaySound(string pszSound, UIntPtr hmod, uint fdwSound);

		private static void Close()
		{
			_command = "close MediaFile";
			mciSendString(_command, null, 0, IntPtr.Zero);
			isOpen = false;
		}

		private static void Open(string sFileName)
		{
			_command = "open \"" + sFileName + "\" type mpegvideo alias MediaFile";
			mciSendString(_command, null, 0, IntPtr.Zero);
			isOpen = true;
		}

		private static void Play(bool loop)
		{
			if (isOpen)
			{
				_command = "play MediaFile";
				if (loop)
					_command += " REPEAT";
				mciSendString(_command, null, 0, IntPtr.Zero);
			}
		}

		public static void PlayFile(string file)
		{
			Close();
			Open(file);
			Play(false);
		}
	}
}
