
using System;
using System.IO;

class Program
{
    static void Main()
    {
        Console.WriteLine("Starting cleanup...");
        CleanBashHistory();
        DeleteLogFiles();
    }

    static void CleanBashHistory()
    {
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bash_history");

        try
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                int index = Array.FindIndex(lines, line => line.Contains("###"));
                if (index >= 0)
                {
                    string[] keptLines = lines[..(index + 1)];
                    File.WriteAllLines(filePath, keptLines);
                    Console.WriteLine("~/.bash_history の \"###\" の行以降の行を削除しました。");
                }
                else
                {
                    Console.WriteLine("~/.bash_history に \"###\" の行が見つかりませんでした。変更なし。");
                }
            }
            else
            {
                // .bash_history が見つからない場合、以下の行でファイルを作成
                string[] defaultLines = {
                    "sudo dnf upgrade",
                    "sdk update",
                    "sdk upgrade",
                    "rustup update",
                    "rustup upgrade",
                    "sudo dnf autoremove",
                    "sudo dnf update",
                    "flatpak update",
                    "###"
                };
                File.WriteAllLines(filePath, defaultLines);
                Console.WriteLine("~/.bash_history が見つからないため、新しく作成しました。");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"~/.bash_history の処理中にエラーが発生しました: {ex.Message}");
        }
    }

    static void DeleteLogFiles()
    {
        string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string[] logFiles = Directory.GetFiles(homeDir, "server-*.log");
        foreach (string file in logFiles)
        {
            try
            {
                File.Delete(file);
                Console.WriteLine($"削除しました: {Path.GetFileName(file)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"削除に失敗しました: {Path.GetFileName(file)} - {ex.Message}");
            }
        }
    }
}

