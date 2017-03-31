using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DeuTutor
{
	//Thanks http://stackoverflow.com/users/13793/joe-doyle

	public static class Mp3Player
	{
		private static string _command;
		private static bool _isOpen;
		[DllImport("winmm.dll")]
		private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

		[DllImport("winmm.dll")]
		public static extern bool PlaySound(string pszSound, UIntPtr hmod, uint fdwSound);

		private static void Close()
		{
			_command = "close MediaFile";
			mciSendString(_command, null, 0, IntPtr.Zero);
			_isOpen = false;
		}

		private static void Open(string sFileName)
		{
			_command = "open \"" + sFileName + "\" type mpegvideo alias MediaFile";
			mciSendString(_command, null, 0, IntPtr.Zero);
			_isOpen = true;
		}

		private static void Play(bool loop)
		{
			if (_isOpen)
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
