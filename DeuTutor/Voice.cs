using System;
using System.IO;
using VoiceRSS_SDK;

namespace DeuTutor
{
	class Voice
	{
		private readonly VoiceProvider _voiceProvider;
		private string _filePath;
		public int SpeedRate;
	
		public Voice()
		{
			var apiKey = Properties.Settings.Default.apiKey;
			SpeedRate = Properties.Settings.Default.speedRate;
			_voiceProvider = new VoiceProvider(apiKey, false);

			_voiceProvider.SpeechFailed += ex =>
			{
				Console.WriteLine(ex.Message);
			};

			_voiceProvider.SpeechReady += data =>
			{
				File.WriteAllBytes(_filePath, (byte[])data);
				Mp3Player.PlayFile(_filePath);
			};


		}

		public void Say(string text, string lang)
		{
			var voiceParams = new VoiceParameters(text, lang)
			{
				AudioCodec = AudioCodec.MP3,
				AudioFormat = AudioFormat.Format_44KHZ.AF_44khz_16bit_mono,
				IsBase64 = false,
				IsSsml = false,
				SpeedRate = SpeedRate
			};
			_filePath = GetFilePath(text);
			if (!File.Exists(_filePath))
			{
				_voiceProvider.SpeechAsync<byte[]>(voiceParams);
			}
			else
			{
				Mp3Player.PlayFile(_filePath);
			}
		}

		private string GetFilePath(string text)
		{
			var guid = GuidUtility.Create(GuidUtility.DnsNamespace, text);
			var folder = Path.Combine(Environment.CurrentDirectory, "Voiced");
			if (!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}
			return Path.Combine(folder, guid + ".mp3");
		}
	}
}
