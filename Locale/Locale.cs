using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HPG.Locale
{
    /// <summary>
    /// Хранилище локализованных строк
    /// Хранит пары ключ/значение
    /// Предоставляет к ним доступ по ключу
    /// </summary>
    public static class Locale
    {
        #region Languages

        private static LocaleInfo _current;

        /// <summary>
        /// Список поддерживаемых языков
        /// </summary>
        public static List<LocaleInfo> Supported { get; private set; }

        /// <summary>
        /// Текущий выбранный язык
        /// </summary>
        public static LocaleInfo Current { get { return _current; } }

        /// <summary>
        /// Инициализация списком поддерживаемых языков игры
        /// </summary>
        public static void SetSupported(List<LocaleInfo> supportedLocales)
        {
            Supported = supportedLocales;
        }

        /// <summary>
        /// Устанавливает текущий язык игры учитывая приоритеты
        /// Созраненное игрой последнее состояние в наивысшем приоритете -> Локаль девайса -> Дефолтная локаль
        /// </summary>
        /// <param name="cachedLocaleShort"></param>
        /// <param name="defaultLocaleShort"></param>
        public static void SetCurrent(string cachedLocaleShort, string defaultLocaleShort)
        {
            // подгружаем локаль с кеша
            if (!string.IsNullOrEmpty(cachedLocaleShort)
                && TryGetLocale(locale => locale.Shortcut == cachedLocaleShort, out _current))
            {
                Debug.Log(string.Format("Locale {0} loaded from cache", _current.Shortcut));
                return;
            }
            // если нет - пытаемся взять с девайса
            string deviceLanguage = Application.systemLanguage.ToString();
            if (!string.IsNullOrEmpty(deviceLanguage)
                && TryGetLocale(locale => locale.Name == deviceLanguage, out _current))
            {
                Debug.Log(string.Format("Locale {0} loaded from device settings", _current.Shortcut));
                return;
            }
            // подгружаем локаль с конфига
            if (!string.IsNullOrEmpty(defaultLocaleShort)
                && TryGetLocale(locale => locale.Shortcut == defaultLocaleShort, out _current))
            {
                Debug.Log(string.Format("Locale {0} loaded from config", _current.Shortcut));
                return;
            }
            throw new Exception("Unable to set current locale");
        }

        private static bool TryGetLocale(Func<LocaleInfo, bool> condition, out LocaleInfo locale)
        {
            locale = null;
            if (Supported == null)
                return false;
            locale = Supported.FirstOrDefault(condition);
            return locale != null;
        }

        #endregion

        private static readonly Dictionary<string, string> _strings = new Dictionary<string, string>();

        /// <summary>
        /// Инициализирует локаль парами ключ/значение
        /// Если предоставленный ключ уже содержится в локали, он перезаписывается новым значением
        /// </summary>
        public static void AddStrings(Dictionary<string, string> strings)
        {
            foreach (var kvp in strings)
                _strings[kvp.Key] = kvp.Value;
        }

        /// <summary>
        /// Получает форматированную строку по ключу из локали, подставляя args в плейсхолдеры вида {0}
        /// </summary>
        /// <param name="key">id ключа в хранилище строк</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Get(string key, params object[] args)
        {
            return string.Format(Get(key), args);
        }

        /// <summary>
        /// Получить строку по ключу из локали
        /// </summary>
        /// <param name="id">id ключа в хранилище строк</param>
        /// <returns></returns>
        public static string Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("Empty locale id requested");
                return string.Empty;
            }

            string localizedString;
            if (_strings.TryGetValue(id, out localizedString))
                return localizedString;

            Debug.LogWarning(string.Format("String with id \"{0}\" not found", id));
            return id;
        }

        /// <summary>
        /// Попытка получить строку по ключу из локали
        /// </summary>
        /// <param name="id"></param>
        /// <param name="localizedString"></param>
        /// <returns></returns>
        public static string TryGet(string id)
        {
            if (string.IsNullOrEmpty(id))
                return string.Empty;

            string localizedString;
            if (_strings.TryGetValue(id, out localizedString))
                return localizedString;

            return id;
        }

        /// <summary>
        /// Очищает хранилище строк
        /// </summary>
        public static void Uninit()
        {
            _strings.Clear();
        }
    }
}
