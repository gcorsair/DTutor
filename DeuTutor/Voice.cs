using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		public Voice()
		{
			apiKey = "c6aac2a97eef4a5d89c7b54b6f5a03d6";
			isSSL = false;
			text = "über eine Änderung der Verfassung";
			lang = Languages.German;

			voiceProvider = new VoiceProvider(apiKey, isSSL);

			voiceProvider.SpeechFailed += (Exception ex) =>
			{

				Console.WriteLine(ex.Message);
			};

			voiceProvider.SpeechReady += (object data) =>
			{
				var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "voice.mp3");
				File.WriteAllBytes(fileName, (byte[])data);
			};

			
		}

		public void Say(string text, string lang)
		{
			var voiceParams = new VoiceParameters(text, lang)
			{
				AudioCodec = AudioCodec.MP3,
				AudioFormat = AudioFormat.Format_44KHZ.AF_44khz_16bit_stereo,
				IsBase64 = false,
				IsSsml = false,
				SpeedRate = 0
			};
			voiceProvider.SpeechAsync<byte[]>(voiceParams);
		}
	}
}
