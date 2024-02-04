using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Diagnostics;

namespace GetComment
{
    public static class MainFileWork
    {
        public static string? _path { get; private set; } = string.Empty;

        public static void SetFile()
        {
            if (!CheckPath()) return;
            var scriptFiles = Directory.GetFiles(_path, "*.cs", SearchOption.AllDirectories);
            using StreamWriter outputFile = new(Path.Combine(_path, "Comments.txt"));
            if(scriptFiles.Length < 1)
            {
                outputFile.WriteLine("Не найдены скрипты...");
                Process.Start("explorer.exe", $"select, {_path}\\Comments.txt");
                return;
            }
            foreach (var file in scriptFiles)
            {
                string code = File.ReadAllText(file);
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

                var root = syntaxTree.GetCompilationUnitRoot();
                var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

                foreach (var method in methods)
                {
                    var comments = method.DescendantTrivia()
                                         .Where(trivia => trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
                                         trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
                                         .Select(trivia => new { MethodName = method.Identifier.ValueText, Comment = trivia.ToString() });

                    foreach (var commentInfo in comments)
                    {
                        if (!commentInfo.Comment.Contains("<summary>")) continue;

                        outputFile.WriteLine("Method: " + commentInfo.MethodName);
                        outputFile.WriteLine("Comment: " + StandardtCommant(commentInfo.Comment));
                        outputFile.WriteLine();
                    }
                }
            }
            Process.Start("explorer.exe", $"select, {_path}\\Comments.txt");
        }

        public static void GetFolderPath()
        {
            var dialog = new OpenFolderDialog{Title = "Выберите папку"};
            dialog.ShowDialog();

            _path = dialog.FolderName;
        }

        public static bool CheckPath()
        {
            if(_path == null || _path == string.Empty)
            {
                MessageBox.Show("Сначала нужно указать путь!");
                return false;
            }
            return true;
        }

        public static string StandardtCommant(string comment)
        {
            var replaced = comment.Replace("<summary>", "");
            var replaced2 = replaced.Replace("</summary>", "");
            var replacedReturn = replaced2.Replace("<returns>", "Method return ");
            var replace = replacedReturn.Replace("///", "");
            var deleteSpace = replace.Replace("  ", "");
            return deleteSpace.Replace("</returns>", "");
        }
    }
}
