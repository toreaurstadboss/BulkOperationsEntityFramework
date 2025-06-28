using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkOperationsEntityFramework.Lib.Services
{

    /// <summary>
    /// Sources for the pluralization rules for Norwegian language:
    /// https://toppnorsk.com/2018/11/18/flertall-hovedregler/
    /// </summary>
    public class NorwegianPluralizationService : IPluralizationService
    {
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
            "museum", "jubileum", "kjemikalium"
        };

        public string Pluralize(string word)
        {
            word = NormalizeWord(word);

            if (_specialCases.ContainsKey(word))
            {
                return _specialCases[word]; // Handle special cases first
            }
            // Handle words that should use special pluralization rules next
            
            if (_wordsChangingVowelToÆ.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                if (word.Equals("Håndkle", StringComparison.OrdinalIgnoreCase))
                {
                    return "Håndklær"; // special case for 'håndkle' => 'håndklær'
                }
                return word.Replace("å", "æ").Replace("e", "æ") + "r"; // for words changing vowel to 'æ', e.g. 'håndkle' => 'håndkler', 'kne' => 'knær', 'tre' => 'trær', 'tå' => 'tær'
            }
            if (_wordsForUnits.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                return word; // for words that are units or monetary values, do not pluralize (exception 'krone' => 'krone' is covered by the 'e' ending rule at the top here
            }
            if (_wordsForRelatives.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                switch (word.ToLower())
                {
                    case "far":
                        return "Fedre"; // 'far' => 'fedre'
                    case "mor":
                        return "Mødre"; // 'mor' => 'mødre'
                    case "datter":
                        return "Døtre"; // 'datter' => 'døtre'
                    case "søster":
                        return "Søstre"; // 'søster' => 'søstre'
                    case "fetter":
                        return "Fettere"; // 'fetter' => 'fettere'
                    case "onkel":
                        return "Onkler"; // 'onkel' => 'onkler'
                    case "svigerbror":
                        return "Svigerbrødre"; // 'svigerbror' => 'svigerbrødre'    
                    case "svigerfar":
                        return "Svigerfedre"; // 'svigerfar' => 'svigerfedre'   
                    case "svigersøster":
                        return "Svigersøstre"; // 'svigersøster' => 'svigerøstre'    
                    case "svigermor":
                        return "Svigermødre"; // 'svigermor' => 'svigermødre'   
                    case "bror":
                        return "Brødre"; // 'bror' => 'brødre'
                    default:
                        break; //for other relatives, contain to logical checks below
                }
            }
            if (_wordsNeutralGenderEndingWithEumOrIum.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                if (word.EndsWith("eum"))
                {
                    return word.Substring(0, word.Length - 3) + "eer"; // eum => eer
                }
                if (word.EndsWith("ium"))
                {
                    return word.Substring(0, word.Length - 3) + "ier"; // ium => ier
                }
                //else - fallthrough to next checks below
            }
            if (_wordsNoPluralizationForNeutralGenderOneSyllable.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                return word; // Return the word as is for neutral genered one-syllable words
            }
            if (_wordChangingVowelsInPluralFemale.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                return NormalizeWord(word.ToLower().Replace("å", "e").Replace("a", "e") + "er"); // for words changing vowels in plural, e.g. 'natt' => 'netter', 'hand' => 'hender'
            }
            if (_wordsChangingVowelsInPluralMale.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                string rewrittenWord = NormalizeWord(word.Replace("o", "ø")); // for words changing vowels in plural, e.g. 'bok' => 'bøker'. Normalize once more to be sure we capitalize first letter.
                if (rewrittenWord.Equals("føt", StringComparison.OrdinalIgnoreCase))
                {
                    return rewrittenWord + "ter"; //  'fot' => 'føtter'
                }
                if (rewrittenWord.EndsWith("e")) // check if the rewritten word ends with 'e' after vowel change
                {
                    return rewrittenWord + "r"; // add 'r' for plural
                }
                if (!rewrittenWord.EndsWith("er"))
                {
                    return rewrittenWord + "er"; // for words changing vowels in plural, e.g. 'bok' => 'bøker'
                }
            }
            if (_nonEndingWordsInPlural.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                return word; // Return the word as is for non-ending words in plural
            }

            // Handle the general pluralization rules for cases where no specific rules apply
            if (word.EndsWith("er"))
            {
                return word.Substring(0, word.Length - 2) + "ere"; // for words ending with 'er' => plural : 'ere'
            }

            if (word.EndsWith("el"))
            {
                return word.Substring(0, word.Length - 2) + "ler"; // for words ending with 'el' => plural : 'ler'
            }
            if (word.EndsWith("e"))
            {
                return word + "r";
            }
            if (word.EndsWith("en"))
            {
                return word + "er";
            }
            return word + "er";
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
            if (string.IsNullOrEmpty(word) || word.Trim().Length <= 1) {
                return word?.ToUpper();
            }
            return word.Substring(0, 1).ToUpper() + word.Trim().ToLower().Substring(1);
        }
        
    }
}
