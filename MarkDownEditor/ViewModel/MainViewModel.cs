using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace MarkDownEditor.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            sourceCode.TextChanged += new EventHandler((object obj, EventArgs e) => UpdatePreview());
            UpdatePreview();
        }

        public string Title => "MarkDown Editor Alpha";

        private string markdownSourceTempPath = Path.GetTempFileName();
        private string previewSourceTempPath = Path.GetTempFileName() + ".html";

        public string PreviewSource
        {
            get
            {
                //if (!File.Exists(previewSourceTempPath))
                //{
                //    StreamWriter sw = new StreamWriter(previewSourceTempPath);
                //    sw.Write(Properties.Resources.EmptyHTML);
                //    sw.Close();
                //}
                //StreamReader sr = new StreamReader(previewSourceTempPath);
                //string all = sr.ReadToEnd();
                //sr.Close();
                //return all;
                return previewSourceTempPath;
            }
        }
        
        private TextDocument sourceCode = new TextDocument();
        public TextDocument SourceCode
        {
            get { return sourceCode; }
            set
            {
                if (sourceCode == value)
                    return;
                sourceCode = value;
                RaisePropertyChanged("SourceCode");
            }
        }

        private string editorFont = "Consolas";
        public string EditorFont
        {
            get { return editorFont; }
            set
            {
                if (editorFont == value)
                    return;
                editorFont = value;
                RaisePropertyChanged("EditorFont");
            }
        }

        private int editorFontSize = 12;
        public int EditorFontSize
        {
            get { return editorFontSize; }
            set
            {
                if (editorFontSize == value)
                    return;
                editorFontSize = value;
                RaisePropertyChanged("EditorFontSize");
            }
        }

        public double ScrollbarPos { get; set; }

        public Dictionary<string,string> MarkDownType => new Dictionary<string, string>()
        {
            { "Markdown", "markdown" },
            { "Markdown_Strict", "markdown_strict"},
            { "Markdown_Github", "markdown_github" },
            { "Markdown_mmd", "markdown_mmd" }
        };

        private string currentMarkdownTypeText = "Markdown";
        public string CurrentMarkdownTypeText
        {
            get { return currentMarkdownTypeText; }
            set
            {
                if (currentMarkdownTypeText == value)
                    return;
                currentMarkdownTypeText = value;
                RaisePropertyChanged("CurrentMarkdownTypeText");
                UpdatePreview();
            }
        }

        private void UpdatePreview()
        {
            StreamWriter sw = new StreamWriter(markdownSourceTempPath);
            sw.Write(SourceCode.Text);
            sw.Close();

            Process process = new Process();
            process.StartInfo.FileName = "pandoc";
            process.StartInfo.Arguments = $"\"{markdownSourceTempPath}\" -f {MarkDownType[CurrentMarkdownTypeText]} -t html --ascii -s -H theme.css -o \"{previewSourceTempPath}\"";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();

            RaisePropertyChanged("PreviewSource");
        }
    }
}