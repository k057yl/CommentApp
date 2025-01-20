using System.Text.RegularExpressions;

namespace CommentApp.Resources
{
    public class HtmlValidator
    {
        private static readonly HashSet<string> AllowedTags = new HashSet<string> { "a", "code", "i", "strong", "p", "b" };

        private static readonly HashSet<string> AllowedAttributes = new HashSet<string> { "href", "title" };

        public bool ValidateHtml(string input)
        {
            var tagPattern = @"<(/?)(\w+)([^>]*)>";
            var matches = Regex.Matches(input, tagPattern);

            foreach (Match match in matches)
            {
                string tagName = match.Groups[2].Value.ToLower();

                if (!AllowedTags.Contains(tagName))
                {
                    Console.WriteLine($"Недопустимый тег: <{tagName}>");
                    return false;
                }

                string attributes = match.Groups[3].Value;
                var attributePattern = @"(\w+)=""[^""]*""";
                var attributeMatches = Regex.Matches(attributes, attributePattern);

                foreach (Match attributeMatch in attributeMatches)
                {
                    string attributeName = attributeMatch.Groups[1].Value.ToLower();

                    if (!AllowedAttributes.Contains(attributeName))
                    {
                        Console.WriteLine($"Недопустимый атрибут: {attributeName}");
                        return false;
                    }

                    if (attributeName == "onclick" || attributeName == "onload")
                    {
                        Console.WriteLine($"Опасный атрибут: {attributeName}");
                        return false;
                    }
                }
            }

            var openTags = new Stack<string>();
            foreach (Match match in matches)
            {
                string tagName = match.Groups[2].Value.ToLower();
                if (match.Groups[1].Value == "/")
                {
                    if (openTags.Count == 0 || openTags.Pop() != tagName)
                    {
                        Console.WriteLine($"Ошибка: незакрытый тег <{tagName}>");
                        return false;
                    }
                }
                else
                {
                    openTags.Push(tagName);
                }
            }

            if (openTags.Count > 0)
            {
                Console.WriteLine($"Ошибка: незакрытый тег <{openTags.Peek()}>");
                return false;
            }

            return true;
        }
    }
}