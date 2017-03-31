using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DeuTutor
{
	internal class Program
	{
		private const string Delimiter = "	";
		private const int WordRemovedAfter = 1;
		private static List<string> _lines;
		private static int _statGreen;
		private static int _statRed;
		private static int _statWhite;
		private static string _aLanguage;
		private static string _qLanguage;
		private static string _fileNameWithoutExtension;

		[STAThread]
		static void Main()
		{
			Console.InputEncoding = Encoding.Unicode;
			Console.OutputEncoding = Encoding.Unicode;
			Console.SetWindowSize(80, 40);
			Console.SetBufferSize(80, 400);
			
			while (true)
			{
				_statGreen = 0;
				_statRed = 0;
				_statWhite = 0;
				LoadData();
				RunExercise();
				Console.Title = "Finished!     " + _fileNameWithoutExtension;
				Console.WriteLine("Well done! Next level?");
				if (Console.ReadKey().KeyChar == 'n')
				{
					return;
				}
			}
		}

		private static void RunExercise()
		{
			var voice = new Voice();
			int maxLines;
			var rnd = new Random();
			do
			{
				_lines = _lines.OrderBy(x => rnd.Next()).ToList();
				maxLines = _lines.Count;
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("Words left: " + maxLines);
				Console.ResetColor();
				for (var i = 0; i < maxLines; i++)
				{
					try
					{
						Console.Title =
							$"Progress: {i + 1}/{maxLines}  G:{_statGreen} W:{_statWhite} R:{_statRed}     [{_fileNameWithoutExtension}]";
						var line = _lines[i].Split(new[] { Delimiter }, StringSplitOptions.None);

						if (!string.IsNullOrEmpty(_qLanguage)) voice.Say(line[1], _qLanguage);
						Console.WriteLine(line[1]);

						var cleanAnswer = line[0].Replace("!", "").Replace("?", "");
						Console.ForegroundColor = ProcessAnswer(Console.ReadLine(), line, ref i, ref maxLines);

						if (!string.IsNullOrEmpty(_aLanguage)) voice.Say(cleanAnswer, _aLanguage);
						Console.WriteLine(cleanAnswer);
						Console.WriteLine();
						UpdateStats(Console.ForegroundColor);
						Console.ResetColor();
					}
					catch (Exception e)
					{
						Console.Write("Error in line " + i);
						Console.WriteLine(" : " + _lines[i]);
						Console.WriteLine(e.Message);
					}
				}
			} while (maxLines > 0);
		}

		private static void LoadData()
		{
			var fd = new OpenFileDialog();
			fd.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
			if (fd.ShowDialog() == DialogResult.OK)
			{
				_lines = File.ReadAllLines(fd.FileName).ToList();
				_fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fd.FileName);
			}
			else
			{
				Exit();
			}
			var settingsFile = (Regex.Replace(fd.FileName, @"\d+", "")).Replace(Path.GetExtension(fd.FileName), "settings");
			if (File.Exists(settingsFile))
			{
				var settings = File.ReadAllLines(settingsFile);
				var voiceLanguageSetting = settings[0].Split(new[] { Delimiter }, StringSplitOptions.None);
				_aLanguage = voiceLanguageSetting[0];
				_qLanguage = voiceLanguageSetting[1];
			}
		}

		private static void UpdateStats(ConsoleColor foregroundColor)
		{
			switch (foregroundColor)
			{
				case ConsoleColor.Red:
					_statRed++;
					break;
				case ConsoleColor.Green:
					_statGreen++;
					break;
				default:
					_statWhite++;
					Console.CursorTop--;
					Console.ReadLine();
					break;
			}
		}

		private static ConsoleColor ProcessAnswer(string answer, string[] line, ref int i, ref int maxLines)
		{
			if (answer.Contains("SAVE"))
			{
				Save(answer.Split(new[] { " " }, StringSplitOptions.None)[1]);
				return ConsoleColor.White;
			}

			if (string.IsNullOrEmpty(answer))
			{
				Console.CursorTop--;
				return ConsoleColor.White;
			}
			var matchMethod = line[0][0]; //get first character
			bool match;

			switch (matchMethod.ToString())
			{
				case "!": //exact match
					match = line[0].Replace("!", "").Equals(answer);
					break;
				case "?": //should start with
					match = line[0].Replace("?", "").StartsWith(answer);
					break;
				default:
					match = !string.IsNullOrEmpty(answer) && Regex.IsMatch(line[0], answer.Replace(" ", " .*"));
					break;
			}
			if (match)
			{
				Console.CursorTop--;
				if (line.Count() < 3)
				{
					_lines[i] += Delimiter + "+";
				}
				else if (line[2].Length > WordRemovedAfter - 1)
				{
					_lines.RemoveAt(i);
					maxLines--;
					i--;
				}
				else
				{
					_lines[i] += "+";
				}
				return ConsoleColor.Green;
			}

			_lines.Add(line[0] + Delimiter + line[1] + Delimiter + "+"); //TODO: add (WORD_REMOVED_AFTER - 1) pluses
			return ConsoleColor.Red;
		}

		private static void Save(string toFile)
		{
			var path = Path.Combine(Environment.CurrentDirectory, "Resources", toFile);
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			using (var sw = File.CreateText(path))
			{
				foreach (var line in _lines)
				{
					sw.Write(line + Environment.NewLine);
				}
			}
		}

		private static void Exit()
		{
			Environment.Exit(0);
		}
	}
}
