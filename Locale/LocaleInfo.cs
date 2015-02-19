using System;
using System.Collections.Generic;
using System.Linq;

namespace HPG.Locale
{
    /// <summary>
    /// Language info 
    /// </summary>
    public class LocaleInfo
    {
        private struct UnicodeRange
        {
            private readonly int _start;
            private readonly int _end;

            private UnicodeRange(int start, int end)
            {
                _start = start;
                _end = end;
            }

            public bool IsCharInRange(char c)
            {
                return c >= _start && c <= _end;
            }

            /// <summary>
            /// Парсит определение юникодных интервалов во внутреннюю структуру
            /// </summary>
            /// <param name="inputRanges">Строка в формате "0020-007F,0600-06FF"</param>
            /// <returns></returns>
            public static UnicodeRange[] Parse(string inputRanges)
            {
                if (string.IsNullOrEmpty(inputRanges))
                    return null;

                var resultRanges = new List<UnicodeRange>();
                string[] rangeStrings = inputRanges.Split(',');
                foreach (var rangeString in rangeStrings)
                {
                    string[] rangeParts = rangeString.Trim().Split('-');
                    if (rangeParts.Length != 2)
                        throw new FormatException("Unicode ranges string is in invalid format: " + inputRanges);
                    resultRanges.Add(new UnicodeRange(Convert.ToInt32(rangeParts[0], 16), Convert.ToInt32(rangeParts[1], 16)));
                }
                return resultRanges.ToArray();
            }
        }

        /// <summary>
        /// Название языка на английском
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Сокращенное обозначение языка (напр. ru или en)
        /// </summary>
        public string Shortcut { get; private set; }

        /// <summary>
        /// Название языка в родном написании
        /// </summary>
        public string Native { get; private set; }

        /// <summary>
        /// Код языка (в формате ru-RU или en-US)
        /// </summary>
        public string Culture { get; private set; }

        private readonly UnicodeRange[] _inputRanges;

        /// <summary>
        /// Информация о языке локализации
        /// </summary>
        /// <param name="name">Название языка на английском</param>
        /// <param name="shortcut">Сокращенное обозначение языка (напр. ru или en)</param>
        /// <param name="culture">Код языка (в формате ru-RU или en-US)</param>
        /// <param name="inputRanges">Разрешенные для ввода юникод символы в формате "0020-007F,0600-06FF"</param>
        /// <param name="native">Название языка в родном написании</param>
        public LocaleInfo(string name, string shortcut, string culture, string native, string inputRanges)
        {
            Name = name;
            Shortcut = shortcut;
            Culture = culture;
            Native = native;
            _inputRanges = UnicodeRange.Parse(inputRanges);
        }

        /// <summary>
        /// Входит ли указанный символ в разрешенные диапазоны юникод сиволов данной локализации
        /// </summary>
        /// <param name="c">Символ</param>
        /// <returns>Разрешен ли для ввода</returns>
        public bool IsCharInputAllowed(char c)
        {
            if (_inputRanges == null)
                return true;
            return _inputRanges.Any(range => range.IsCharInRange(c));
        }
    }
}

