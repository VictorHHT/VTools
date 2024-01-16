using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTSearchUtility
    {
        // https://gist.github.com/CDillinger/2aa02128f840bdca90340ce08ee71bc2

        /// <summary>
        /// Performs a pre-fuzzy match by comparing characters between the query and haystack in a case-insensitive manner.
        /// </summary>
        /// <param name="query">The query string to match.</param>
        /// <param name="haystack">The haystack string to match against.</param>
        /// <returns>True if the query matches the haystack; otherwise, false.</returns>
        public static bool PreFuzzyMatch(string query, string haystack)
        {
            var queryIndex = 0;
            var haystackIndex = 0;
            var queryLength = query.Length;
            var haystackLength = haystack.Length;

            while (queryIndex != queryLength && haystackIndex != haystackLength)
            {
                if (char.ToLower(query[queryIndex]) == char.ToLower(haystack[haystackIndex]))
                    ++queryIndex;
                ++haystackIndex;
            }

            return queryLength != 0 && haystackLength != 0 && queryIndex == queryLength;
        }

        /// <summary>
        /// Performs a fuzzy match between a query and a haystack, calculating a matching score and optionally providing indices of matched characters.
        /// </summary>
        /// <param name="query">The query string to match.</param>
        /// <param name="haystack">The haystack string to match against.</param>
        /// <param name="outScore">Output parameter to store the matching score.</param>
        /// <param name="matchedIndices">Optional array to store indices of matched characters.</param>
        /// <returns>True if the query matches the haystack; otherwise, false.</returns>
        public static bool FuzzyMatchWithScore(string query, string haystack, out int outScore, bool[] matchedIndices = null)
        {
            // Constants for scoring
            const int AdjacencyBonus = 5;            // Bonus for adjacent matches
            const int SeparatorBonus = 10;           // Bonus if match occurs after a separator
            const int CamelBonus = 10;               // Bonus if match is uppercase and prev is lowercase

            const int LeadingLetterPenalty = -3;     // Penalty for every letter in haystack before the first match
            const int MaxLeadingLetterPenalty = -9;  // Maximum penalty for leading letters
            const int UnmatchedLetterPenalty = -1;   // Penalty for every letter that doesn't match

            // Variables for scoring algorithm
            int score = 0;
            int queryIndex = 0;
            int queryLength = query.Length;
            int haystackIndex = 0;
            int haystackLength = haystack.Length;
            bool prevMatched = false;
            bool prevLower = false;
            bool prevSeparator = true;               // True if the first letter match gets a separator bonus

            // Use the "best" matched letter if multiple haystack letters match the query
            char? bestLetter = null;
            char? bestLower = null;
            int? bestLetterIndex = null;
            int bestLetterScore = 0;

            // Loop over haystack
            while (haystackIndex != haystackLength)
            {
                // Retrieve query character and its lowercase version
                var queryChar = queryIndex != queryLength ? query[queryIndex] as char? : null;
                var queryLower = queryChar != null ? char.ToLower((char)queryChar) as char? : null;

                // Retrieve haystack character and its lowercase and uppercase versions
                char haystackChar = haystack[haystackIndex];
                char haystackLower = char.ToLower(haystackChar);
                char haystackUpper = char.ToUpper(haystackChar);

                // Check for various match conditions
                bool nextMatch = queryChar.HasValue && queryLower == haystackLower;
                bool rematch = bestLetter.HasValue && bestLower == haystackLower;
                bool advanced = nextMatch && bestLetter.HasValue;
                bool queryRepeat = bestLetter.HasValue && queryChar.HasValue && bestLower == queryLower;

                // Handle advanced or repeated matches
                if (advanced || queryRepeat)
                {
                    score += bestLetterScore;

                    // Record matched index if needed
                    if (matchedIndices != null)
                    {
                        matchedIndices[bestLetterIndex.Value] = true;
                    }

                    bestLetter = null;
                    bestLower = null;
                    bestLetterIndex = null;
                    bestLetterScore = 0;
                }

                // Handle next or rematch
                if (nextMatch || rematch)
                {
                    int newScore = 0;

                    // Apply penalty for each letter before the first pattern match
                    if (queryIndex == 0)
                    {
                        int penalty = Mathf.Max(haystackIndex * LeadingLetterPenalty, MaxLeadingLetterPenalty);
                        score += penalty;
                    }

                    // Apply bonuses for consecutive matches
                    if (prevMatched)
                        newScore += AdjacencyBonus;

                    // Apply bonus for matches after a separator
                    if (prevSeparator)
                        newScore += SeparatorBonus;

                    // Apply bonus across camel case boundaries. Includes "clever" isLetter check.
                    if (prevLower && haystackChar == haystackUpper && haystackLower != haystackUpper)
                        newScore += CamelBonus;

                    // Update query index if the next query letter was matched
                    if (nextMatch)
                        ++queryIndex;

                    // Update the best letter in haystack, considering the next or rematch scenario
                    if (newScore >= bestLetterScore)
                    {
                        // Apply penalty for the now skipped letter
                        if (bestLetter.HasValue)
                            score += UnmatchedLetterPenalty;

                        bestLetter = haystackChar;
                        bestLower = char.ToLower(bestLetter.Value);
                        bestLetterIndex = haystackIndex;
                        bestLetterScore = newScore;
                    }

                    prevMatched = true;
                }
                else
                {
                    // Handle unmatched letter
                    score += UnmatchedLetterPenalty;
                    prevMatched = false;
                }

                // Update conditions for next iteration
                prevLower = haystackChar == haystackLower && haystackLower != haystackUpper;
                prevSeparator = haystackChar == '_' || haystackChar == ' ';

                // Move to the next character in haystack
                ++haystackIndex;
            }

            // Apply the score for the last match
            if (bestLetter.HasValue)
            {
                score += bestLetterScore;

                // Record matched index if needed
                if (matchedIndices != null)
                {
                    matchedIndices[bestLetterIndex.Value] = true;
                }
            }

            outScore = score;
            return queryIndex == queryLength;
        }

        // You can do something like this,
        // Opening Tag: <size=12><color=#24ff85><b>
        // Closing Tag: </b></color></size>
        /// <summary>
        /// Generates a string with rich text tags for highlighting matched characters in a query within a haystack.
        /// </summary>
        /// <param name="query">The query string to highlight.</param>
        /// <param name="haystack">The haystack string to search for matches.</param>
        /// <param name="openingTag">Opening tag for highlighting.</param>
        /// <param name="closingTag">Closing tag for highlighting.</param>
        /// <param name="isOverlay">If true, get the highlighted query without appending non-matching characters.</param>
        /// <returns>A string with rich text tags highlighting the matched characters.</returns>
        public static string GetHighlightedQuery(string query, string haystack, string openingTag = "<b>", string closingTag = "</b>", bool isOverlay = false)
        {
            if (string.IsNullOrEmpty(query))
            {
                return haystack;
            }

            bool[] matchingIndices = GetMatchedIndices(query, haystack);

            if (matchingIndices == null)
            {
                return haystack;
            }

            var sb = new StringBuilder();

            var isHighlighted = false;
            var wasHighlighted = false;

            for (var i = 0; i < haystack.Length; i++)
            {
                var character = haystack[i];
                isHighlighted = matchingIndices[i];

                // If the previous is not being highlighted and current one is highlighted
                if (!wasHighlighted && isHighlighted)
                {
                    sb.Append(openingTag);
                }
                else if (wasHighlighted && !isHighlighted)
                {
                    sb.Append(closingTag);
                }

                if (!isOverlay || isHighlighted)
                {
                    sb.Append(character);
                }
                else
                {
                    // If is in overlay mode and not being highlighted, append a white space so the char in the query could be seen
                    // The negation of a disjunction (OR) is equivalent to the conjunction (AND) of the negations of the individual conditions and vice versa
                    sb.Append(' ');
                }

                wasHighlighted = isHighlighted;
            }

            if (isHighlighted)
            {
                sb.Append(closingTag);
            }

            return sb.ToString();
        }

        private static bool[] GetMatchedIndices(string query, string haystack)
        {
            bool[] indices = new bool[haystack.Length];

            if (FuzzyMatchWithScore(query, haystack, out _, indices))
            {
                return indices;
            }
            else
            {
                return null;
            }
        }
    }
}
