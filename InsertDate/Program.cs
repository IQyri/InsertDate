using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoreLinq.Extensions;

namespace InsertDate
{
    /// <summary>
    /// Точка входа.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Точка входа.
        /// </summary>
        /// <param name="args">Аргументы.</param>
        public static void Main(string[] args)
        {
            Recieve(".//Data")
                .ForEach(
                    streamInOut =>
                    {
                        using (var reader = new StreamReader(streamInOut.Read))
                        using (var writer = new StreamWriter(streamInOut.Write))
                        {
                            ReadAllLines(reader)
                                .ForEach(
                                    line =>
                                    {
                                        if (line.StartsWith("$$18"))
                                        {
                                            writer.WriteLine(
                                                $"дата ввода изменений – {GetFirstWorkDay(DateTime.Now.AddDays(1)):dd.MM.yyyy}"
                                            );
                                        }
                                        writer.WriteLine(line);
                                    }
                                );
                        }
                    }
                );
        }

        /// <summary>
        /// Получение первого рабочего дня.
        /// </summary>
        /// <param name="date">Дата и время для получения первого рабочего дня.</param>
        /// <returns>Первый рабочий день.</returns>
        private static DateTime GetFirstWorkDay(DateTime date)
        {
            return IsWorkDay(date) ? date : GetFirstWorkDay(date.AddDays(1));
        }

        /// <summary>
        /// Проверка даты на рабочий день.
        /// П.С. Не учитывается "рабочий календарь".
        /// </summary>
        /// <param name="date">Дата и время для проверки.</param>
        /// <returns>true - является рабочим днем, false - иначе.</returns>
        private static bool IsWorkDay(DateTime date)
        {
            return !(date.DayOfWeek == DayOfWeek.Sunday |
                     date.DayOfWeek == DayOfWeek.Saturday);
        }

        /// <summary>
        /// Получение перечисления с потоками на чтение и на запись.
        /// </summary>
        /// <param name="path">Путь к файлу.</param>
        /// <returns>Перечисление с потоками на чтение и запись.</returns>
        private static IEnumerable<(FileStream Read, FileStream Write)> Recieve(string path)
        {
            var files = Directory.GetFiles(path, "*.txt");
            return files.Select(
                file =>
                (
                    File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read),
                    File.Create(Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ".new"))
                )
            );
        }

        /// <summary>
        /// Получение перечисления со строками из читателя потока.
        /// </summary>
        /// <param name="reader">Читатель потока..</param>
        /// <returns>Перечисление со строками.</returns>
        private static IEnumerable<string> ReadAllLines(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                yield return reader.ReadLine();
            }
        }
    }
}
