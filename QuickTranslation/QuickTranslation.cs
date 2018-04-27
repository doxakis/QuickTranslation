using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace QuickTranslationPack
{
	public enum LangMissingStrategy
	{
		EmptyString,
		ThrowException
	}

	public class QuickTranslation
	{
		/// <summary>
		/// All translations. (Code -> Lang -> text)
		/// </summary>
		private Dictionary<string, Dictionary<string, string>> AllTranslations;

		/// <summary>
		/// Code not found handler.
		/// </summary>
		private Func<string, string> NotFoundCallback;
		
		private QuickTranslation(string translations, Func<string, string> callbackIfNotFound)
		{
			var allTranslations = new Dictionary<string, Dictionary<string, string>>();

			string curCode = string.Empty;
			foreach (var item in translations.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
			{
				if (!string.IsNullOrWhiteSpace(item))
				{
					var line = item.TrimStart();

					// Remove comment.
					if (line.StartsWith("#"))
					{
						continue;
					}

					string lang = "en";
					int textStart;
					if (line.StartsWith("-"))
					{
						// Translation.

						// Get language.
						var langStart = 1;
						var langEnd = line.IndexOf(':');
						if (langEnd == -1 || langEnd > line.Length)
						{
							continue;
						}
						lang = line.Substring(langStart, langEnd - 1).Trim();
						if (string.IsNullOrWhiteSpace(lang))
						{
							// If lang is missing, fallback to english.
							lang = "en";
						}

						textStart = langEnd + 1;
					}
					else
					{
						// English text.

						// Get code.
						var codeStart = 0;
						var codeEnd = line.IndexOf(':', codeStart);
						if (codeEnd == -1 || codeEnd > line.Length)
						{
							continue;
						}

						textStart = codeEnd + 1;
						var code = line.Substring(codeStart, codeEnd - codeStart).Trim();

						curCode = code;
					}
					
					// Get text.
					if (textStart > line.Length)
					{
						continue;
					}
					var text = line.Substring(textStart).TrimStart();

					if (!allTranslations.ContainsKey(curCode))
					{
						allTranslations[curCode] = new Dictionary<string, string>();
					}

					if (!allTranslations[curCode].ContainsKey(lang))
					{
						allTranslations[curCode].Add(lang, text);
					}
				}
			}

			AllTranslations = allTranslations;
			NotFoundCallback = callbackIfNotFound;
		}

		/// <summary>
		/// Get all translations for the current language.
		/// </summary>
		/// <returns>Code and text</returns>
		public Dictionary<string, string> GetTranslations()
		{
			var lang = Thread.CurrentThread.CurrentUICulture.Name;
			var dict = new Dictionary<string, string>();
			foreach (var entry in AllTranslations)
			{
				var code = entry.Key;
				dict.Add(code, this[code]);
			}
			return dict;
		}

		/// <summary>
		/// Get text for the current language and code.
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public string this[string code]
		{
			get
			{
				if (!AllTranslations.ContainsKey(code))
				{
					// Not found.
					return NotFoundCallback(code);
				}

				var langDict = AllTranslations[code];
				var lang = Thread.CurrentThread.CurrentUICulture.Name;

				// Ex: lang = fr-CA or en-US.
				while (!langDict.ContainsKey(lang))
				{
					int pos = lang.LastIndexOf('-');
					if (pos != -1)
					{
						// Ex: fr-CA => fr
						lang = lang.Substring(0, pos);
					}
					else
					{
						// Ex: fr => en
						lang = "en";

						if (!langDict.ContainsKey(lang))
						{
							// Not found.
							return NotFoundCallback(lang);
						}
					}
				}

				return langDict[lang];
			}
		}

		/// <summary>
		/// Prepare the QuickTranslation object with default strategy.
		/// 
		/// If code is missing, it throws an exception.
		/// </summary>
		/// <param name="translations"></param>
		/// <returns></returns>
		public static QuickTranslation Prepare(string translations)
		{
			return Prepare(translations, LangMissingStrategy.ThrowException);
		}

		/// <summary>
		/// Prepare the QuickTranslation object with a predefined strategy.
		/// </summary>
		/// <param name="translations"></param>
		/// <param name="strategy"></param>
		/// <returns></returns>
		public static QuickTranslation Prepare(string translations, LangMissingStrategy strategy)
		{
			Func<string, string> callbackIfNotFound;
			switch (strategy)
			{
				case LangMissingStrategy.EmptyString:
					callbackIfNotFound = (code) => {
						return string.Empty;
					};
					break;
				case LangMissingStrategy.ThrowException:
					callbackIfNotFound = (code) => {
						throw new InvalidOperationException("No translation found for code: " + code);
					};
					break;
				default:
					throw new NotImplementedException();
			}
			return Prepare(translations, callbackIfNotFound);
		}

		/// <summary>
		/// Prepare the QuickTranslation object with a custom handler for missing translation.
		/// </summary>
		/// <param name="translations"></param>
		/// <param name="callbackIfNotFound"></param>
		/// <returns></returns>
		public static QuickTranslation Prepare(string translations, Func<string, string> callbackIfNotFound)
		{
			return new QuickTranslation(translations, callbackIfNotFound);
		}
	}
}
