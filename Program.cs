// <author>Grok Code Fast 1</author>
// Linux 環境で .bash_history をクリーンアップし、server-*.log ファイルを削除するプログラム。Windows では実行されない。
// messages.ini からメッセージを読み込み、エラーハンドリングも実装。
// messages.ini の例:
// [Messages]
// ProgramNotForLinux=このプログラムは Linux 環境でのみ実行できます。
// StartingCleanup=クリーンアップを開始します。
// BashHistoryCleaned=.bash_history がクリーンアップされました。
// BashHistoryNotFound=.bash_history ファイルが見つかりませんでした。
// BashHistoryCreated=.bash_history ファイルが作成されました。
// BashHistoryError=.bash_history のクリーンアップ中にエラーが発生しました: {0}
// LogFileDeleted={0} が削除されました。
// LogFileDeleteFailed={0} の削除に失敗しました: {1}


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
                "yes | curl -fsSL https://ollama.com/install.sh | sh; sudo systemctl restart ollama",
                "sudo systemctl restart ollama",
                "ollama stop gemma4:12b",
                "ollama serve gemma4:12b",
                "ollama stop qwen2.5 - coder:7b",
                "ollama serve qwen2.5 - coder:7b",
                "sudo e4defrag / dev / sdb1",
                "sdk update",
                "sdk upgrade",
                "sdk upgrade java",
                "sdk current list",
                "sdk flash candidatess",
                "rustup update",
                "rustup upgrade",
                "rustup toolchain",
                "flatpak update - y",
                "flatpak uninstall --unused",
                "sudo dnf upgrade",
                "sudo dnf autoremove",
                "sudo dnf update",
                "hermes update",
                "hc",
                "hc; sudo dnf update - y; sudo dnf autoremove - y; sudo dnf upgrade - y; flatpak update -y; flatoak uninstall --unused; rustup upgrade; rustup update; rustup self update; yes | sdk upgrade; sdk current; sdk flash candidatess; hc",
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

