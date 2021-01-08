using System.Collections.Generic;
using ColorCode.Common;

namespace ColorCode.Compilation.Languages
{
    public class Batch : ILanguage
    {
        public string Id
        {
            get { return LanguageId.Batch; }
        }

        public string FirstLinePattern
        {
            get { return null; }
        }

        public string Name
        {
            get { return "Batch"; }
        }

        public IList<LanguageRule> Rules
        {
            get
            {
                return new List<LanguageRule>
                    {
                        new LanguageRule(
                            @"(::|REM).*",
                            new Dictionary<int, string>
                            {
                                { 0, ScopeName.Comment },
                            }),
                        new LanguageRule(
                            @"""[^\n]*?""",
                            new Dictionary<int, string>
                            {
                                { 0, ScopeName.String },
                            }),
                        new LanguageRule(
                            @"\b(assoc|attrib|break|bcdedit|cacls|call|cd|chcp|chdir|chkdsk|chkntfs|cls|cmd|color|comp|compact|convert|copy|con|date|del|dir|do|diskpart|doskey|driverquery|echo|else|endlocal|erase|errorlevel|exist|exit|fc|find|findstr|for|format|fsutil|ftype|goto|gpresult|graftabl|help|icacls|if|in|label|md|mkdir|mklink|mode|more|move|not|nul|openfiles|path|pause|popd|print|prompt|pushd|rd|recover|ren|rename|replace|rmdir|robocopy|set|setlocal|sc|schtasks|shift|shutdown|sort|start|subst|systeminfo|tasklist|taskkill|time|title|tree|type|ver|verify|vol|xcopy|wmic)\b",
                            new Dictionary<int, string>
                            {
                                { 0, ScopeName.Keyword },
                            }),
                        new LanguageRule(
                            @"%\d|%\*|%%\w|%\w*%",
                            new Dictionary<int, string>
                            {
                                { 0, ScopeName.PowerShellVariable },
                            }),
                        new LanguageRule(
                            @":\w*\b", // label
                            new Dictionary<int, string>
                            {
                                { 0, ScopeName.Attribute },
                            }),
                    };
            }
        }

        public string CssClassName
        {
            get { return "batch"; }
        }

        public bool HasAlias(string lang)
        {
            switch (lang.ToLower())
            {
                case "bat":
                case "cmd":
                    return true;
                default:
                    return false;
            };
        }
    }
}
