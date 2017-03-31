using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using VoiceRSS_SDK;

namespace DeuTutor
{
	class Voice
	{
		private string apiKey;
		private bool isSSL;
		private string text;
		private string lang;
		private VoiceProvider voiceProvider;
		private string filePath;
		public int speedRate;
	
		public Voice()
		{
			apiKey = Properties.Settings.Default.apiKey;
			speedRate = Properties.Settings.Default.speedRate;
			isSSL = false;
			voiceProvider = new VoiceProvider(apiKey, isSSL);

			voiceProvider.SpeechFailed += (Exception ex) =>
			{
				Console.WriteLine(ex.Message);
			};

			voiceProvider.SpeechReady += (object data) =>
			{
				File.WriteAllBytes(filePath, (byte[])data);
				MP3Player.PlayFile(filePath);
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
				SpeedRate = speedRate
			};
			filePath = GetFilePath(text);
			if (!File.Exists(filePath))
			{
				voiceProvider.SpeechAsync<byte[]>(voiceParams);
			}
			else
			{
				MP3Player.PlayFile(filePath);
			}
		}

		private string GetFilePath(string text)
		{
			Guid guid = GuidUtility.Create(GuidUtility.DnsNamespace, text);
			string folder = Path.Combine(Environment.CurrentDirectory, "Voiced");
			if (!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}
			return Path.Combine(folder, guid + ".mp3");
		}
	}
}
