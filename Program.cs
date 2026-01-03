
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

/// <author>Grok Code Fast 1</author>
static class Program
{
    private static readonly Dictionary<string, string> messages = new Dictionary<string, string>();
    private const string IniNotFoundMessage = "messages.ini が見つかりません。";

    /// <summary>
    /// プログラムのエントリーポイント。OS をチェックし、Linux の場合にクリーンアップ処理を実行。
    /// </summary>
    static void Main()
    {
        // messages.ini を読み込み
        LoadMessages();

        // Windows の場合、プログラムを終了
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine(GetMessage("ProgramNotForLinux"));
            return;
        }
        Console.WriteLine(GetMessage("StartingCleanup"));
        CleanBashHistory();
        DeleteLogFiles();
    }

    /// <summary>
    /// .bash_history ファイルをクリーンアップ。 "###" の行以降を削除し、ファイルが存在しない場合はデフォルトの履歴を作成。
    /// </summary>
    static void CleanBashHistory()
    {
        // .bash_history のパスを取得
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bash_history");

        try
        {
            if (File.Exists(filePath))
            {
                // ファイルを読み込み
                string[] lines = File.ReadAllLines(filePath);
                // "###" の行を探す
                int index = Array.FindIndex(lines, line => line.Contains("###"));
                if (index >= 0)
                {
                    // "###" の行までを保持
                    string[] keptLines = lines[..(index + 1)];
                    File.WriteAllLines(filePath, keptLines);
                    Console.WriteLine(GetMessage("BashHistoryCleaned"));
                }
                else
                {
                    Console.WriteLine(GetMessage("BashHistoryNotFound"));
                }
            }
            else
            {
                // .bash_history が見つからない場合、デフォルトの履歴を作成
                string[] defaultLines = {
                "sdk update",
                "sdk upgrade",
                "rustup update",
                "rustup upgrade",
                "hisclear",
                "flatpak update",
                "sudo dnf upgrade",
                "sudo dnf autoremove",
                "sudo dnf update",
                "###"
            };
                File.WriteAllLines(filePath, defaultLines);
                Console.WriteLine(GetMessage("BashHistoryCreated"));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{GetMessage("BashHistoryError").Replace("{0}", ex.Message)}");
        }
    }

    /// <summary>
    /// ホームディレクトリ内の server-*.log ファイルを削除。
    /// </summary>
    static void DeleteLogFiles()
    {
        // ホームディレクトリのパスを取得
        string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        // server-*.log のファイルを検索
        string[] logFiles = Directory.GetFiles(homeDir, "server-*.log");
        foreach (string file in logFiles)
        {
            try
            {
                // ファイルを削除
                File.Delete(file);
                Console.WriteLine($"{GetMessage("LogFileDeleted").Replace("{0}", Path.GetFileName(file))}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{GetMessage("LogFileDeleteFailed").Replace("{0}", Path.GetFileName(file)).Replace("{1}", ex.Message)}");
            }
        }
    }

    /// <summary>
    /// messages.ini からメッセージを読み込み。
    /// </summary>
    static void LoadMessages()
    {
        string iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "messages.ini");
        if (!File.Exists(iniPath))
        {
            Console.WriteLine(IniNotFoundMessage);
            return;
        }

        string[] lines = File.ReadAllLines(iniPath);
        bool inMessagesSection = false;
        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith(";") || trimmed.StartsWith("#"))
            {
                continue; // 空行やコメントをスキップ
            }
            if (trimmed.StartsWith("[Messages]"))
            {
                inMessagesSection = true;
            }
            else if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
            {
                inMessagesSection = false;
            }
            else if (inMessagesSection && trimmed.Contains("="))
            {
                string[] parts = trimmed.Split('=', 2);
                if (parts.Length == 2)
                {
                    messages[parts[0].Trim()] = parts[1].Trim();
                }
            }
        }
    }

    /// <summary>
    /// 指定されたキーに対応するメッセージを取得。
    /// </summary>
    /// <param name="key">メッセージのキー</param>
    /// <returns>メッセージ文字列</returns>
    static string GetMessage(string key)
    {
        return messages.ContainsKey(key) ? messages[key] : $"Message not found: {key}";
    }
}

