using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Linq;

namespace BulkOperationsEntityFramework.Lib.Services
{

    /// <summary>
    /// Sources for the pluralization rules for Norwegian language:
    /// https://toppnorsk.com/2018/11/18/flertall-hovedregler/
    /// </summary>
    public class NorwegianPluralizationService : IPluralizationService
    {

        public static List<string> PluralizedWords = new List<string>();

        public string Pluralize(string word)
        {
            if (PluralizedWords.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                return word; // Return the already pluralized word
            }

            //#if DEBUG
            //            Debugger.Break();
            //            Debugger.Launch(); // Uncomment this line to break into the debugger when this method is called, for example when database migrations are made with EF Code First
            //#endif

            word = NormalizeWord(word);

            string pluralizedWord;

            if (_specialCases.ContainsKey(word))
            {
                pluralizedWord = _specialCases[word];
                PluralizedWords.Add(pluralizedWord);
                return pluralizedWord;
            }

            if (_wordsChangingVowelToÆ.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                if (word.Equals("Håndkle", StringComparison.OrdinalIgnoreCase))
                {
                    pluralizedWord = "Håndklær";
                    PluralizedWords.Add(pluralizedWord);
                    return pluralizedWord;
                }
                pluralizedWord = word.Replace("å", "æ").Replace("e", "æ") + "r";
                PluralizedWords.Add(pluralizedWord);
                return pluralizedWord;
            }

            if (_wordsForUnits.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                pluralizedWord = word;
                PluralizedWords.Add(pluralizedWord);
                return pluralizedWord;
            }

            if (_wordsForRelatives.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                switch (word.ToLower())
                {
                    case "far": pluralizedWord = "Fedre"; break;
                    case "mor": pluralizedWord = "Mødre"; break;
                    case "datter": pluralizedWord = "Døtre"; break;
                    case "søster": pluralizedWord = "Søstre"; break;
                    case "fetter": pluralizedWord = "Fettere"; break;
                    case "onkel": pluralizedWord = "Onkler"; break;
                    case "svigerbror": pluralizedWord = "Svigerbrødre"; break;
                    case "svigerfar": pluralizedWord = "Svigerfedre"; break;
                    case "svigersøster": pluralizedWord = "Svigersøstre"; break;
                    case "svigermor": pluralizedWord = "Svigermødre"; break;
                    case "bror": pluralizedWord = "Brødre"; break;
                    default: pluralizedWord = word; break;
                }
                PluralizedWords.Add(pluralizedWord);
                return pluralizedWord;
            }

            if (_wordsNeutralGenderEndingWithEumOrIum.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                if (word.EndsWith("eum"))
                    pluralizedWord = word.Substring(0, word.Length - 3) + "eer";
                else if (word.EndsWith("ium"))
                    pluralizedWord = word.Substring(0, word.Length - 3) + "ier";
                else
                    pluralizedWord = word;
                PluralizedWords.Add(pluralizedWord);
                return pluralizedWord;
            }

            if (_wordsNoPluralizationForNeutralGenderOneSyllable.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                pluralizedWord = word;
                PluralizedWords.Add(pluralizedWord);
                return pluralizedWord;
            }

            if (_wordChangingVowelsInPluralFemale.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                pluralizedWord = NormalizeWord(word.ToLower().Replace("å", "e").Replace("a", "e") + "er");
                PluralizedWords.Add(pluralizedWord);
                return pluralizedWord;
            }

            if (_wordsChangingVowelsInPluralMale.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                string rewrittenWord = NormalizeWord(word.Replace("o", "ø"));
                if (rewrittenWord.Equals("føt", StringComparison.OrdinalIgnoreCase))
                    pluralizedWord = rewrittenWord + "ter";
                else if (rewrittenWord.EndsWith("e"))
                    pluralizedWord = rewrittenWord + "r";
                else if (!rewrittenWord.EndsWith("er"))
                    pluralizedWord = rewrittenWord + "er";
                else
                    pluralizedWord = rewrittenWord;
                PluralizedWords.Add(pluralizedWord);
                return pluralizedWord;
            }

            if (_nonEndingWordsInPlural.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                pluralizedWord = word;
                PluralizedWords.Add(pluralizedWord);
                return pluralizedWord;
            }

            // General rules
            if (word.EndsWith("er"))
                pluralizedWord = word.Substring(0, word.Length - 2) + "ere";
            else if (word.EndsWith("el"))
                pluralizedWord = word.Substring(0, word.Length - 2) + "ler";
            else if (word.EndsWith("e"))
                pluralizedWord = word + "r";
            else if (word.EndsWith("en"))
                pluralizedWord = word + "er";
            else
                pluralizedWord = word + "er";

            PluralizedWords.Add(pluralizedWord);
            return pluralizedWord;
        }

        public string Singularize(string word)
        {
            word = NormalizeWord(word);

            // Reverse special cases
            var specialSingular = _specialCases.FirstOrDefault(kvp => kvp.Value.Equals(word, StringComparison.OrdinalIgnoreCase));
            if (!specialSingular.Equals(default(KeyValuePair<string, string>)))
                return specialSingular.Key;

            // Words that are the same in singular and plural
            if (_nonEndingWordsInPlural.Contains(word, StringComparer.OrdinalIgnoreCase) ||
                _wordsNoPluralizationForNeutralGenderOneSyllable.Contains(word, StringComparer.OrdinalIgnoreCase) ||
                _wordsForUnits.Contains(word, StringComparer.OrdinalIgnoreCase))
                return word;

            // Irregulars and vowel changes (expand as needed)
            if (word.Equals("Bøker", StringComparison.OrdinalIgnoreCase)) return "Bok";
            if (word.Equals("Føtter", StringComparison.OrdinalIgnoreCase)) return "Fot";
            if (word.Equals("Brødre", StringComparison.OrdinalIgnoreCase)) return "Bror";
            if (word.Equals("Menn", StringComparison.OrdinalIgnoreCase)) return "Mann";
            if (word.Equals("Kvinner", StringComparison.OrdinalIgnoreCase)) return "Kvinne";
            if (word.Equals("Gutter", StringComparison.OrdinalIgnoreCase)) return "Gutt";
            if (word.Equals("Netter", StringComparison.OrdinalIgnoreCase)) return "Natt";
            if (word.Equals("Tær", StringComparison.OrdinalIgnoreCase)) return "Tå";
            if (word.Equals("Tenner", StringComparison.OrdinalIgnoreCase)) return "Tann";
            if (word.Equals("Trær", StringComparison.OrdinalIgnoreCase)) return "Tre";
            if (word.Equals("Knær", StringComparison.OrdinalIgnoreCase)) return "Kne";
            if (word.Equals("Bønder", StringComparison.OrdinalIgnoreCase)) return "Bonde";
            if (word.Equals("Hender", StringComparison.OrdinalIgnoreCase)) return "Hand";
            if (word.Equals("Døtre", StringComparison.OrdinalIgnoreCase)) return "Datter";
            if (word.Equals("Fedre", StringComparison.OrdinalIgnoreCase)) return "Far";
            if (word.Equals("Mødre", StringComparison.OrdinalIgnoreCase)) return "Mor";
            if (word.Equals("Søstre", StringComparison.OrdinalIgnoreCase)) return "Søster";
            if (word.Equals("Øyne", StringComparison.OrdinalIgnoreCase)) return "Øye";

            // "ler" ending (from "el")
            if (word.EndsWith("ler"))
            {
                return word.Substring(0, word.Length - 2);
            }
            if (word.EndsWith("ter"))
            {
                return word.Substring(0, word.Length - 1);
            }

            // "ere" ending (from "er" ending in singular, e.g. "Lærere" -> "Lærer")
            if (word.EndsWith("ere"))
                return word.Substring(0, word.Length - 1);

            // "er" ending (general case, e.g. "Biler" -> "Bil", "Stoler" -> "Stol", "Jenter" -> "Jente")
            if (word.EndsWith("er"))
                return word.Substring(0, word.Length - 2);

            // "r" ending (from "e" ending in singular, e.g. "Jenter" -> "Jente" already handled above)
            if (word.EndsWith("r"))
            {
                var possibleSingular = word.Substring(0, word.Length - 1);
                return possibleSingular;
            }

            // Default: return as is
            return word;
        }

        /// <summary>
        /// Make the world normalized, i.e. first letter upper case and rest lower case letters, the word is trimmed.
        /// Not considering using invariant culture here, as this is a Norwegian pluralization service.
        /// </summary>
        /// <remarks>In case an empty word (null or empty) is passed in, just return the word.
        /// Edge case: In case just One non-empty letter was passed in, make the word also uppercase.</remarks>
        private string NormalizeWord(string word)
        {
            word = word?.Trim();
            if (string.IsNullOrEmpty(word) || word.Trim().Length <= 1)
            {
                return word?.ToUpper();
            }
            return word.Substring(0, 1).ToUpper() + word.Trim().ToLower().Substring(1);
        }

        private string[] _nonEndingWordsInPlural = new string[] {
            "mus", "sko", "ski", "feil", "ting" }; // Add more non-ending words in plural as needed

        private string[] _wordsChangingVowelsInPluralMale = new string[]
        {
            "bonde", "fot", "bok", "bot", "rot"
        };

        private Dictionary<string, string> _specialCases = new Dictionary<string, string>
        {
            { "Mann", "Menn" } , // 'mann' => 'menn'
            { "Barn", "Barn" }, // 'barn' => 'barn' (no pluralization)
            { "Øye", "Øyne" }, // 'øye' => 'øyne' (plural form of 'eye') //consider adding more special cases here in case all the other pluralization rules do not cover the given word
        };

        private string[] _wordsChangingVowelToÆ = new string[]
        {
            "Håndkle", "Kne", "Tre", "Tå"
        };

        private string[] _wordsForUnits = new string[]
        {
            "meter", "centimeter", "millimeter", "kilometer", "gram", "kilogram", "tonn", "liter", "desiliter", "centiliter", "dollar", "lire",
            "pesetas", "euro", "yen", "franc", "pund", "rupee", "ringgit", "peso", "real", "won", "yuan"
        };

        private string[] _wordChangingVowelsInPluralFemale = new string[]
        {
            "and", "hand", "hånd", "natt", "stang", "strand", "tang", "tann"
        };

        private string[] _wordsForRelatives = new string[]
        {
            "far", "mor", "datter", "fetter", "onkel", "bror", "svigerbror", "svigerfar", "svigermor", "svigersøster", "søster"
        };

        private string[] _wordsNoPluralizationForNeutralGenderOneSyllable = new string[]
        {
            "hus", "fjell", "blad"
        };

        private string[] _wordsNeutralGenderEndingWithEumOrIum = new string[]
        {
            "museum", "Jubileum", "kjemikalium"
        };

    }
}
