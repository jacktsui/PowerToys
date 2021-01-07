using System.Collections.Generic;
using ColorCode.Common;

namespace ColorCode.Compilation.Languages
{
    public class Python : ILanguage
    {
        public string Id
        {
            get { return LanguageId.Python; }
        }

        public string FirstLinePattern
        {
            get { return null; }
        }

        public string Name
        {
            get { return "Python"; }
        }

        public IList<LanguageRule> Rules
        {
            get
            {
                return new List<LanguageRule>
                    {
                        new LanguageRule(
                            @"'''(.*)'''",
                            new Dictionary<int, string>
                            {
                                { 0, ScopeName.Comment },
                            }),
                        new LanguageRule(
                            @"""""""(.*)""""""",
                            new Dictionary<int, string>
                            {
                                { 0, ScopeName.Comment },
                            }),
                        new LanguageRule(
                            @"(#.*)",
                            new Dictionary<int, string>
                            {
                                { 0, ScopeName.Comment },
                            }),
                        new LanguageRule(
                            @"'[^\n]*?'",
                            new Dictionary<int, string>
                            {
                                { 0, ScopeName.String },
                            }),
                            new LanguageRule(
                            @"""[^\n]*?""",
                            new Dictionary<int, string>
                            {
                                { 0, ScopeName.String },
                            }),
                        new LanguageRule(
                            @"\b(and|as|assert|async|await|break|class|continue|def|del|elif|else|except|finally|for|from|global|if|import|in|is|lambda|not|or|pass|raise|return|try|while|with|yield)\b",
                            new Dictionary<int, string>
                            {
                                { 0, ScopeName.Keyword },
                            }),
                        new LanguageRule(
                            @"\b(__import__|abs|all|any|ascii|bin|bool|breakpoint|bytearray|bytes|callable|chr|classmethod|compile|complex|delattr|dict|dir|divmod|enumerate|eval|exec|filter|float|format|frozenset|getattr|globals|hasattr|hash|help|hex|id|input|int|isinstance|issubclass|iter|len|list|locals|map|max|memoryview|min|next|object|oct|open|ord|pow|print|property|range|repr|reversed|round|set|setattr|slice|sorted|staticmethod|str|sum|super|tuple|type|vars|zip)\b",
                            new Dictionary<int, string>
                            {
                                { 0, ScopeName.BuiltinFunction },
                            }),
                        new LanguageRule(
                            @"(0[xX])([0-9A-Fa-f]*)",
                            new Dictionary<int, string>
                            {
                                { 1, ScopeName.BuiltinValue },
                                { 2, ScopeName.Number },
                            }),
                        new LanguageRule(
                            @"(0[oO])([0-7]*)",
                            new Dictionary<int, string>
                            {
                                { 1, ScopeName.BuiltinValue },
                                { 2, ScopeName.Number },
                            }),
                        new LanguageRule(
                            @"(0[bB])([0-1]*)",
                            new Dictionary<int, string>
                            {
                                { 1, ScopeName.BuiltinValue },
                                { 2, ScopeName.Number },
                            }),
                        new LanguageRule(
                            @"\b[0-9]{1,}\b",
                            new Dictionary<int, string>
                                {
                                    { 0, ScopeName.Number }
                                }),
                    };
            }
        }

        public string CssClassName
        {
            get { return "python"; }
        }

        public override bool Equals(object obj)
        {
            return obj is Python python &&
                   Id == python.Id;
        }

        public bool HasAlias(string lang)
        {
            switch (lang.ToLower())
            {
                case "py":
                    return true;

                default:
                    return false;
            }
        }
    }
}
