// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using ColorCode;
using Common;
using Microsoft.PowerToys.PreviewHandler.CodeFile.Properties;
using Microsoft.PowerToys.PreviewHandler.CodeFile.Telemetry.Events;
using Microsoft.PowerToys.Telemetry;
using PreviewHandlerCommon;
using StyleDic = ColorCode.Styling.StyleDictionary;

namespace Microsoft.PowerToys.PreviewHandler.CodeFile
{
    /// <summary>
    /// Win Form Implementation for CodeFile Preview Handler.
    /// </summary>
    public class CodeFilePreviewHandlerControl : FormHandlerControl
    {
        private static readonly IFileSystem FileSystem = new FileSystem();
        private static readonly IPath Path = FileSystem.Path;
        private static readonly IFile File = FileSystem.File;

        /// <summary>
        /// Markdown HTML header.
        /// </summary>
        private readonly string htmlHeader = "<!doctype html><html><head><style>body{0}</style></head><body>";

        /// <summary>
        /// Markdown HTML footer.
        /// </summary>
        private readonly string htmlFooter = "</body></html>";

        /// <summary>
        /// RichTextBox control to display if external images are blocked.
        /// </summary>
        private RichTextBox _infoBar;

        /// <summary>
        /// Extended Browser Control to display markdown html.
        /// </summary>
        private WebBrowserExt _browser;

        /// <summary>
        /// True if external image is blocked, false otherwise.
        /// </summary>
        private bool _infoBarDisplayed;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFilePreviewHandlerControl"/> class.
        /// </summary>
        public CodeFilePreviewHandlerControl()
        {
        }

        /// <summary>
        /// Start the preview on the Control.
        /// </summary>
        /// <param name="dataSource">Path to the file.</param>
        public override void DoPreview<T>(T dataSource)
        {
            _infoBarDisplayed = false;

            try
            {
                if (!(dataSource is string filePath))
                {
                    throw new ArgumentException($"{nameof(dataSource)} for {nameof(CodeFilePreviewHandler)} must be a string but was a '{typeof(T)}'");
                }

                string ext = Path.GetExtension(filePath);
                string fileText = File.ReadAllText(filePath);

                string codeFileHTML = string.Empty;
                if (ext.Equals(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    codeFileHTML = fileText;
                }
                else
                {
                    StyleDic theme = StyleDic.DefaultLight;
                    string background = "{background:#FFFFFF;}";
                    string baseColor = ControlzEx.Theming.WindowsThemeHelper.GetWindowsBaseColor();
                    if (baseColor == "Dark")
                    {
                        theme = StyleDic.DefaultDark;
                        background = "{background:#202020;}";
                    }

                    string header = string.Format(System.Globalization.CultureInfo.CurrentCulture, htmlHeader, background);
                    HtmlFormatter formatter = new HtmlFormatter(theme);
                    codeFileHTML = formatter.GetHtmlString(fileText, GetLanguageByExtension(ext));
                    codeFileHTML = $"{header}{codeFileHTML}{htmlFooter}";
                }

                /* not work, why? the result of "codeFileHTML" is right. render fail.
                string lang = GetLangByExtension(ext);
                string js = File.ReadAllText(@"C:\res\shjs\sh_main.min.js");
                string jsLang = File.ReadAllText($"C:\\res\\shjs\\lang\\sh_{lang}.min.js");
                string css = File.ReadAllText(@"C:\res\shjs\css\default.css");
                string header = $"<!doctype html><html><head><style>{css}</style><script>{js}{jsLang}</script></head><body onload=\"sh_highlightDocument();\">";
                string footer = "</body></html>";
                string codeFileHTML = $"{header}<pre class=\"sh_{lang}\">{HttpUtility.HtmlEncode(fileText)}</pre>{footer}";
                */

                InvokeOnControlThread(() =>
                {
                    _browser = new WebBrowserExt
                    {
                        DocumentText = codeFileHTML,
                        Dock = DockStyle.Fill,
                        IsWebBrowserContextMenuEnabled = false,
                        ScriptErrorsSuppressed = true,
                        ScrollBarsEnabled = true,
                        AllowNavigation = false,
                    };
                    Controls.Add(_browser);

                    if (_infoBarDisplayed)
                    {
                        _infoBar = GetTextBoxControl(codeFileHTML);
                        Resize += FormResized;
                        Controls.Add(_infoBar);
                    }
                });

                PowerToysTelemetry.Log.WriteEvent(new CodeFilePreviewed());
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                PowerToysTelemetry.Log.WriteEvent(new CodeFilePreviewError { Message = ex.Message });

                InvokeOnControlThread(() =>
                {
                    Controls.Clear();
                    _infoBarDisplayed = true;
                    _infoBar = GetTextBoxControl(Resources.CodeFileNotPreviewedError);
                    Resize += FormResized;
                    Controls.Add(_infoBar);
                });
            }
            finally
            {
                base.DoPreview(dataSource);
            }
        }

        private RichTextBox GetTextBoxControl(string message)
        {
            RichTextBox richTextBox = new RichTextBox
            {
                Text = message,
                BackColor = Color.LightYellow,
                Multiline = true,
                Dock = DockStyle.Top,
                ReadOnly = true,
            };
            richTextBox.ContentsResized += RTBContentsResized;
            richTextBox.ScrollBars = RichTextBoxScrollBars.None;
            richTextBox.BorderStyle = BorderStyle.None;

            return richTextBox;
        }

        /// <summary>
        /// Callback when RichTextBox is resized.
        /// </summary>
        /// <param name="sender">Reference to resized control.</param>
        /// <param name="e">Provides data for the resize event.</param>
        private void RTBContentsResized(object sender, ContentsResizedEventArgs e)
        {
            RichTextBox richTextBox = (RichTextBox)sender;
            richTextBox.Height = e.NewRectangle.Height + 5;
        }

        /// <summary>
        /// Callback when form is resized.
        /// </summary>
        /// <param name="sender">Reference to resized control.</param>
        /// <param name="e">Provides data for the event.</param>
        private void FormResized(object sender, EventArgs e)
        {
            if (_infoBarDisplayed)
            {
                _infoBar.Width = Width;
            }
        }

        /*
        private static string GetLangByExtension(string ext)
        {
            switch (ext)
            {
                case ".cs":
                    return "csharp";
                case ".js":
                    return "javascript";
                case ".py":
                    return "python";
                default:
                    return ext.Substring(1);
            }
        }
        */

        private static ILanguage GetLanguageByExtension(string ext)
        {
            switch (ext)
            {
                case ".bat":
                case ".cmd":
                    return Languages.Batch;
                case ".cpp":
                    return Languages.Cpp;
                case ".cs":
                    return Languages.CSharp;
                case ".css":
                    return Languages.Css;
                case ".java":
                    return Languages.Java;
                case ".js":
                case ".json":
                    return Languages.JavaScript;
                case ".php":
                    return Languages.Php;
                case ".ps1":
                    return Languages.PowerShell;
                case ".py":
                    return Languages.Python;
                case ".sql":
                    return Languages.Sql;
                case ".ts":
                    return Languages.Typescript;
                case ".xml":
                    return Languages.Xml;
                default:
                    return null;
            }
        }
    }
}
