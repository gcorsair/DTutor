using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeuTutor
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.InputEncoding = UTF8Encoding.Unicode;
			Console.OutputEncoding = UTF8Encoding.Unicode;
			int WORD_REMOVED_AFTER = 1;
			int statGreen = 0;
			int statRed = 0;
			int statWhite = 0;
			string DELIMITER = "	";
			Random rnd = new Random();
			Console.SetBufferSize(80, 400);
			Console.SetWindowSize(80, 40);
			List<string> lines = File.ReadAllLines("data.txt").ToList();
			int maxLines;

			do
			{
				lines = lines.OrderBy(x => rnd.Next()).ToList();
				maxLines = lines.Count;
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("Words left: " + maxLines);
				Console.ResetColor();
				for (int i = 0; i < maxLines; i++)
				{
					try
					{
						Console.Title = String.Format("Progress: {0}/{1}  G:{2} W:{3} R:{4}", i + 1, maxLines, statGreen, statWhite, statRed);
						string[] line = lines[i].Split(new String[] { DELIMITER }, StringSplitOptions.None);
						Console.WriteLine(line[1]);
						string answer = Console.ReadLine();
						if (!String.IsNullOrEmpty(answer) && line[0].Contains(answer))
						{
							Console.CursorTop--;
							Console.ForegroundColor = ConsoleColor.Green;
							statGreen++;
							if (line.Count() < 3)
							{
								lines[i] += DELIMITER + "+";
							}
							else if (line[2].Length > WORD_REMOVED_AFTER - 1)
							{
								lines.RemoveAt(i);
								maxLines--;
								i--;
							}
							else
							{
								lines[i] += "+";
							}
						}
						else if (String.IsNullOrEmpty(answer))
						{
							Console.ForegroundColor = ConsoleColor.White;
							Console.CursorTop--;
							statWhite++;
						}
						else
						{
							Console.ForegroundColor = ConsoleColor.Red;
							lines.Add(line[0] + DELIMITER + line[1] + DELIMITER + "+");
							statRed++;
						}
						Console.WriteLine(line[0]);
						Console.WriteLine();
						if (Console.ForegroundColor == ConsoleColor.White)
						{
							Console.CursorTop--;
							Console.ReadLine();
						}
						Console.ResetColor();
					} catch (Exception e) {
						Console.Write("Error in line "+i);
						Console.WriteLine(" : "+lines[i]);
						Console.WriteLine(e.Message);
					}
				}
			} while (maxLines > 0);
			Console.Title = "Finished!";
			Console.WriteLine("Aufwiedersehen!");
			Console.ReadLine();
		}
	}
}
