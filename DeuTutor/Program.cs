using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DeuTutor
{
    class Program
    {
        static string DELIMITER = "	";
        static int WORD_REMOVED_AFTER = 1;
        static List<string> lines;
        static int statGreen = 0;
        static int statRed = 0;
        static int statWhite = 0;

        [STAThread]
        static void Main(string[] args)
        {
            Console.InputEncoding = UTF8Encoding.Unicode;
            Console.OutputEncoding = UTF8Encoding.Unicode;

            int maxLines;
            Random rnd = new Random();
            Console.SetWindowSize(80, 40);
            Console.SetBufferSize(80, 400);
            OpenFileDialog fd = new OpenFileDialog();
            fd.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
            if (fd.ShowDialog() == DialogResult.OK)
            {
                lines = File.ReadAllLines(fd.FileName).ToList();
            }
            else
            {
                return;
            }

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
                        Console.ForegroundColor = ProcessAnswer(Console.ReadLine(), line, ref i, ref maxLines);
                        Console.WriteLine(line[0]);
                        Console.WriteLine();
                        UpdateStats(Console.ForegroundColor);
                        Console.ResetColor();
                    }
                    catch (Exception e)
                    {
                        Console.Write("Error in line " + i);
                        Console.WriteLine(" : " + lines[i]);
                        Console.WriteLine(e.Message);
                    }
                }
            } while (maxLines > 0);
            Console.Title = "Finished!";
            Console.WriteLine("Aufwiedersehen!");
            Console.ReadLine();
        }

        private static void UpdateStats(ConsoleColor foregroundColor)
        {
            switch (foregroundColor)
            {
                case ConsoleColor.Red:
                    statRed++;
                    break;
                case ConsoleColor.Green:
                    statGreen++;
                    break;
                default:
                    statWhite++;
                    Console.CursorTop--;
                    Console.ReadLine();
                    break;
            }
        }

        private static ConsoleColor ProcessAnswer(string answer, string[] line, ref int i, ref int maxLines)
        {
            if (answer.Contains("SAVE"))
            {
                SaveAndExit(answer.Split(new String[] { " " }, StringSplitOptions.None)[1]);
            }
            if (answer.Contains("EXIT"))
            {
                Exit();
            }

            if (String.IsNullOrEmpty(answer))
            {
                Console.CursorTop--;
                return ConsoleColor.White;
            }

            if (!String.IsNullOrEmpty(answer) && Regex.IsMatch(line[0], answer.Replace(" ", ".+")))
            {
                Console.CursorTop--;
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
                return ConsoleColor.Green;
            }

            lines.Add(line[0] + DELIMITER + line[1] + DELIMITER + "+"); //TODO: add (WORD_REMOVED_AFTER - 1) pluses
            return ConsoleColor.Red;
        }

        private static void SaveAndExit(string toFile)
        {
            string path = Path.Combine(Environment.CurrentDirectory, "Resources", toFile);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (StreamWriter sw = File.CreateText(path))
            {
                foreach (var line in lines)
                {
                    sw.Write(line + Environment.NewLine);
                }
            }
            Exit();
        }

        private static void Exit()
        {
            Environment.Exit(0);
        }
    }
}
