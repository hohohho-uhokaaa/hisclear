// <author>Grok code fast + windsurf devin</author>
// Linux 環境で .bash_history をクリーンアップし、server-*.log ファイルを削除するプログラム。
// Windows では実行されません。
// messages.ini からメッセージを読み込み、default_history.txt からデフォルト履歴を読み込みます。
// 自分用のクリーンアップツール
// .bash_history の###以降のコマンドを削除して upper cursor key でいつもの順番でいつもの作業を実行
// windsurf でちょい仕上げ

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

static class Program
{
    /// <summary>
    /// messages.ini から読み込んだメッセージを保持する辞書
    /// </summary>
    private static readonly Dictionary<string, string> messages = new Dictionary<string, string>();

    /// <summary>
    /// .bash_history 内で履歴の区切りとして使用するデリミタ文字列
    /// この行以降のコマンドはクリーンアップ時に削除されます
    /// </summary>
    private const string BashHistoryDelimiter = "###";

    /// <summary>
    /// デフォルトの bash_history コマンドが記述されたファイル名
    /// .bash_history が存在しない場合にこのファイルからコマンドを読み込みます
    /// </summary>
    private const string DefaultHistoryFile = "default_history.txt";

    /// <summary>
    /// プログラムのエントリーポイント
    /// OS をチェックし、Linux の場合にクリーンアップ処理を実行します
    /// </summary>
    static void Main()
    {
        // メッセージリソースファイルを読み込み
        LoadMessages();

        // Windows 環境の場合、処理を終了
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine(GetMessage("ProgramNotForLinux"));
            return;
        }

        // クリーンアップ処理を開始
        Console.WriteLine(GetMessage("StartingCleanup"));
        CleanBashHistory();
        DeleteLogFiles();
    }

    /// <summary>
    /// .bash_history ファイルをクリーンアップします
    /// "###" の行以降を削除し、ファイルが存在しない場合はデフォルトの履歴を作成します
    /// </summary>
    static void CleanBashHistory()
    {
        // ユーザーホームディレクトリの .bash_history パスを取得
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bash_history");

        try
        {
            if (File.Exists(filePath))
            {
                // 既存の履歴ファイルを読み込み
                string[] lines = File.ReadAllLines(filePath);
                
                // デリミタ行（"###"）のインデックスを検索
                int index = Array.FindIndex(lines, line => line.Contains(BashHistoryDelimiter));
                
                if (index >= 0)
                {
                    // デリミタ行までを保持し、それ以降を削除
                    string[] keptLines = lines[..(index + 1)];
                    File.WriteAllLines(filePath, keptLines);
                    Console.WriteLine(GetMessage("BashHistoryCleaned"));
                }
                else
                {
                    // デリミタが見つからない場合、変更なし
                    Console.WriteLine(GetMessage("BashHistoryNotFound"));
                }
            }
            else
            {
                // .bash_history が存在しない場合、デフォルト履歴から新規作成
                string[] defaultLines = LoadDefaultHistory();
                if (defaultLines.Length > 0)
                {
                    File.WriteAllLines(filePath, defaultLines);
                    Console.WriteLine(GetMessage("BashHistoryCreated"));
                }
                else
                {
                    // デフォルト履歴ファイルの読み込みに失敗
                    Console.WriteLine(GetMessage("DefaultHistoryLoadFailed"));
                }
            }
        }
        catch (IOException ex)
        {
            // ファイル入出力エラーを処理
            Console.WriteLine(string.Format(GetMessage("BashHistoryError"), ex.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
            // アクセス権限エラーを処理
            Console.WriteLine(string.Format(GetMessage("BashHistoryError"), ex.Message));
        }
    }

    /// <summary>
    /// ホームディレクトリ内の server-*.log ファイルを削除します
    /// </summary>
    static void DeleteLogFiles()
    {
        // ユーザーホームディレクトリのパスを取得
        string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        
        // server-*.log パターンに一致するファイルを検索
        string[] logFiles = Directory.GetFiles(homeDir, "server-*.log");
        
        foreach (string file in logFiles)
        {
            try
            {
                // ログファイルを削除
                File.Delete(file);
                Console.WriteLine(string.Format(GetMessage("LogFileDeleted"), Path.GetFileName(file)));
            }
            catch (IOException ex)
            {
                // ファイル入出力エラーを処理
                Console.WriteLine(string.Format(GetMessage("LogFileDeleteFailed"), Path.GetFileName(file), ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                // アクセス権限エラーを処理
                Console.WriteLine(string.Format(GetMessage("LogFileDeleteFailed"), Path.GetFileName(file), ex.Message));
            }
        }
    }

    /// <summary>
    /// default_history.txt からデフォルト履歴を読み込みます
    /// </summary>
    /// <returns>デフォルト履歴のコマンド配列。ファイルが存在しない場合は空の配列</returns>
    static string[] LoadDefaultHistory()
    {
        // アプリケーションベースディレクトリの default_history.txt パスを取得
        string historyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultHistoryFile);
        
        if (!File.Exists(historyPath))
        {
            // ファイルが存在しない場合、空の配列を返す
            return Array.Empty<string>();
        }
        
        // ファイルから全行を読み込み
        return File.ReadAllLines(historyPath);
    }

    /// <summary>
    /// messages.ini からメッセージを読み込みます
    /// INI ファイルの [Messages] セクションからキーと値のペアを解析して辞書に格納します
    /// </summary>
    static void LoadMessages()
    {
        // アプリケーションベースディレクトリの messages.ini パスを取得
        string iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "messages.ini");
        
        if (!File.Exists(iniPath))
        {
            // INI ファイルが存在しない場合、エラーメッセージを表示
            Console.WriteLine(GetMessage("IniNotFound"));
            return;
        }

        // INI ファイルから全行を読み込み
        string[] lines = File.ReadAllLines(iniPath);
        bool inMessagesSection = false;
        
        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            
            // 空行やコメント行（; または # で始まる）をスキップ
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith(";") || trimmed.StartsWith("#"))
            {
                continue;
            }
            
            // [Messages] セクションの開始を検出
            if (trimmed.StartsWith("[Messages]"))
            {
                inMessagesSection = true;
            }
            // 他のセクションの開始を検出（Messages セクションを終了）
            else if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
            {
                inMessagesSection = false;
            }
            // Messages セクション内のキー=値ペアを解析
            else if (inMessagesSection && trimmed.Contains("="))
            {
                string[] parts = trimmed.Split('=', 2);
                if (parts.Length == 2)
                {
                    // キーと値を辞書に格納（前後の空白をトリム）
                    messages[parts[0].Trim()] = parts[1].Trim();
                }
            }
        }
    }

    /// <summary>
    /// 指定されたキーに対応するメッセージを取得します
    /// </summary>
    /// <param name="key">メッセージのキー</param>
    /// <returns>メッセージ文字列。キーが存在しない場合はエラーメッセージ</returns>
    static string GetMessage(string key)
    {
        // 辞書にキーが存在する場合は対応するメッセージを返す
        // 存在しない場合はエラーメッセージを返す
        return messages.ContainsKey(key) ? messages[key] : $"Message not found: {key}";
    }
}

