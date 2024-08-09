// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Markdig.Syntax;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    public class ParsedMarkdownContent
    {
        public string FilePath { get; set; }
        public List<string> MarkdownLines { get; set; }
        public MarkdownDocument Ast { get; set; }
        public int CurrentIndex { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Warnings { get; set; }
        public ParsedMarkdownContent(string fileContent)
        {
            MarkdownLines = new List<string>(fileContent.Replace("\r", "").Split(Constants.LineSplitter));
            Ast = Markdig.Markdown.Parse(fileContent);
            CurrentIndex = 0;
            Errors = new List<string>();
            Warnings = new List<string>();
            FilePath = string.Empty;
        }

        public ParsedMarkdownContent(string[] lines)
        {
            MarkdownLines = new List<string>(lines);
            Ast = Markdig.Markdown.Parse(string.Join("\n", lines));
            CurrentIndex = 0;
            Errors = new List<string>();
            Warnings = new List<string>();
            FilePath = string.Empty;
        }

        public static ParsedMarkdownContent ParseFile(string path)
        {
            var parsedMarkdownContent = new ParsedMarkdownContent(File.ReadAllLines(path));
            parsedMarkdownContent.FilePath = path;
            return parsedMarkdownContent;
        }

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public void AddWarning(string warning)
        {
            Warnings.Add(warning);
        }

        public void UnGet()
        {
            if (CurrentIndex > 0)
            {
                CurrentIndex--;
            }
        }

        public void Reset()
        {
            CurrentIndex = 0;
        }

        public int FindHeader(int level, string title)
        {
            for(int i = CurrentIndex+1; i < Ast.Count; i++)
            {
                if (Ast[i] is HeadingBlock headerItem && headerItem.Level == level)
                {
                    if (title == string.Empty)
                    {
                        return i;
                    }
                    else if (string.Equals(headerItem?.Inline?.FirstChild?.ToString(), title, StringComparison.OrdinalIgnoreCase))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public bool IsEnd()
        {
            return CurrentIndex < 0 || CurrentIndex >= Ast.Count - 1;
        }

        public object? Peek()
        {
            if (IsEnd())
            {
                return null;
            }

            return Ast[CurrentIndex+1];
        }

        public object? GetCurrent()
        {
            if (CurrentIndex < 0 || CurrentIndex >= Ast.Count)
            {
                return null;
            }
            else
            {
                return Ast[CurrentIndex];
            }
        }

        public void Seek(int index)
        {
            CurrentIndex = index;
        }

        public object? Take()
        {
            if (CurrentIndex >= Ast.Count || CurrentIndex < 0)
            {
                return null;
            }

            try
            {
                return Ast[CurrentIndex];
            }
            finally
            {
                if (IsEnd())
                {
                    CurrentIndex = -1;
                }
                else
                {
                    CurrentIndex++;
                }
            }
        }

        public int GetTextLine(int offset)
        {
            if (offset < 0 || offset >= Ast.Count)
            {
                return -1;
            }
            else
            {
                return Ast[offset].Line + 1; // internally, we are 0 based
            }
        }

        public bool IsEmptyHeader(int Level)
        {
            var currentHeader = Ast[CurrentIndex] as HeadingBlock;
            var nextHeader = Ast[CurrentIndex + 1] as HeadingBlock;
            if (currentHeader is not null && nextHeader is not null)
            {
                if (currentHeader.Level == Level && nextHeader.Level == Level)
                {
                    return true;
                }
            }

            return false;
        }

        public string GetStringFromAst(int endIndex)
        {
            if (endIndex <= CurrentIndex)
            {
                return string.Empty;
            }

            StringBuilder? sb = null;
            try
            {
                sb = Constants.StringBuilderPool.Get();
                int startLine = Ast[CurrentIndex].Line;
                int endLine = Ast[endIndex].Line;
                if (Ast[endIndex] is HeadingBlock)
                {
                    endLine--;
                }

                for(int i = startLine; i < endLine; i++)
                {
                    sb.AppendLine(MarkdownLines[i].TrimEnd());
                }
                return sb.ToString().Replace("\r", "").Trim();
            }
            finally
            {
                if (sb is not null)
                {
                    Constants.StringBuilderPool.Return(sb);
                }
            }
        }

        public string GetStringFromFile(int lineCount)
        {
            StringBuilder? sb = null;
            try
            {
                sb = Constants.StringBuilderPool.Get();
                int startLine = Ast[CurrentIndex].Line;
                for(int i = startLine; i < startLine + lineCount; i++)
                {
                    sb.AppendLine(MarkdownLines[i].TrimEnd());
                }

                return sb.ToString().Replace("\r", "");
            }
            finally
            {
                if (sb is not null)
                {
                    Constants.StringBuilderPool.Return(sb);
                }
            }
        }

    }
}
