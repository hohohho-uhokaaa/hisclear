# hisclear

Linux 環境で .bash_history をクリーンアップし、server-*.log ファイルを削除するプログラム。
2026/9/28 Program.cs, default_history.txt, hisclear.csproj ちょい修正 dotnet publish -c Release で PublishSingleFile

## 概要

hisclear は Linux 環境専用のクリーンアップツールです。以下の機能を提供します：

- `.bash_history` ファイルのクリーンアップ（`###` 行以降を削除）
- ホームディレクトリ内の `server-*.log` ファイルの削除
- 多言語対応（messages.ini によるメッセージ管理）
- デフォルト履歴の自動生成（default_history.txt から）

## 機能

### .bash_history のクリーンアップ

- `.bash_history` ファイル内の `###` 行以降のコマンドを削除します
- `###` 行が見つからない場合は変更なし
- `.bash_history` が存在しない場合は `default_history.txt` からデフォルト履歴を作成します

### ログファイルの削除

- ホームディレクトリ内の `server-*.log` パターンに一致するファイルを削除します
- 削除できないファイルはエラーメッセージを表示します

## 必要な環境

- .NET 10.0
- Linux オペレーティングシステム
- （Windows では実行されません）

### ビルド前の準備

このプロジェクトは .NET 標準ライブラリのみを使用しており、追加の NuGet パッケージは不要です。したがって、`dotnet add package` コマンドを実行する必要はありません。

もし将来追加のライブラリが必要になった場合は、以下のようにパッケージを追加できます：

```bash
# 例：追加のライブラリが必要な場合
dotnet add package <PackageName>
```

## インストール

```bash
# リポジトリをクローン
git clone <repository-url>
cd hisclear

# ビルド（実行ファイルの配置は行いません）
dotnet build

# publish して ~/.local/bin と ~/.config/hisclear に配置
dotnet publish -c Release --self-contained false

# .NET ランタイムを含む Self-contained 版を配置する場合
dotnet publish -c Release --self-contained true
```

publish 成果物は `linux-x64` 向けの単一ファイルです。publish が完了すると、実行ファイル `hisclear` は `~/.local/bin` に、`messages.ini` と `default_history.txt` は `~/.config/hisclear` に自動配置されます。配置先ディレクトリがない場合は自動作成されます。`--self-contained false` は実行環境に .NET ランタイムが必要で、`--self-contained true` はランタイムを含みます。

## 使用方法

```bash
# 開発中に実行
dotnet run

# publish 済みの実行ファイルを実行
~/.local/bin/hisclear
```

### エイリアスの設定

頻繁に使用する場合、シェル設定ファイルにエイリアスを追加すると便利です。

#### bash の場合（~/.bashrc）

```bash
# ~/.bashrc に以下を追加
alias hc="$HOME/.local/bin/hisclear"

# 設定を反映
source ~/.bashrc

# 使用方法
$ hc
```

#### csh/tcsh の場合（~/.cshrc）

```csh
# ~/.cshrc に以下を追加
alias hc "$HOME/.local/bin/hisclear"

# 設定を反映
source ~/.cshrc

# 使用方法
% hc
```

## 設定ファイル

### messages.ini

メッセージリソースファイル。プログラムのメッセージを管理します。

```ini
[Messages]
ProgramNotForLinux=このプログラムはLinux用です。
StartingCleanup=Starting cleanup...
BashHistoryCleaned=~/.bash_history の "###" の行以降の行を削除しました。
BashHistoryNotFound=~/.bash_history に "###" の行が見つかりませんでした。変更なし。
BashHistoryCreated=~/.bash_history が見つからないため、新しく作成しました。
BashHistoryError=~/.bash_history の処理中にエラーが発生しました: {0}
LogFileDeleted=削除しました: {0}
LogFileDeleteFailed=削除に失敗しました: {0} - {1}
IniNotFound=messages.ini が見つかりません。
DefaultHistoryLoadFailed=デフォルト履歴ファイルの読み込みに失敗しました。
```

### default_history.txt

デフォルトの bash_history コマンドが記述されたファイル。`.bash_history` が存在しない場合に使用されます。

各行にコマンドを記述し、最後の行に `###` を配置します。

#### カスタマイズ

このファイルは自分用に編集して使いやすくカスタマイズできます：

- よく使用するコマンドを追加
- コマンドの順序を変更（上にあるコマンドが履歴の上位に表示されます）
- 不要なコマンドを削除
- 複数のコマンドを1行にまとめる（`;` で区切る）

#### 配置場所の注意事項

`dotnet publish` により、以下の構成で自動配置されます。通常の `dotnet build` ではユーザーディレクトリへの配置は行われません。

```text
~/.local/bin/
└── hisclear

~/.config/hisclear/
├── messages.ini
└── default_history.txt
```

設定ファイルは `~/.config/hisclear` から読み込まれます。ファイルをカスタマイズした場合、次回以降の publish で変更内容がコピーされることがあります。既存設定を保持したい場合は、publish 後に必要に応じてバックアップから戻してください。

## ファイル構成

```
hisclear/
├── Program.cs              # メインプログラム
├── hisclear.csproj         # プロジェクトファイル
├── messages.ini            # メッセージリソースファイル
├── default_history.txt     # デフォルト履歴ファイル
└── README.md               # このファイル
```

## 作者

Grok code fast + windsurf devin

## ライセンス

MIT License

---

## 補足

このプロジェクトは **Windsurf の Devin Next** を使用して開発されました。GitHub と組み合わせることで、効率的にコードの品質を向上させることができました。

また、設定ファイルが見つからない場合に `~/.local/bin` へ作成する今回の仕様変更は、**VSCode + Copilot + gpt-6-luna の無料枠トライアル**を利用して行いました。

### Devin Next について

Devin Next は VSCode ライクに使用できる AI コーディングアシスタントです。Windsurf の Devin Next を使用すると、ちょっとしたサイズのコーディングであれば課金なしで **Vibe Coding** を楽しむことができます。

- **VSCode ライクな操作**: 親しみやすいインターフェースでスムーズにコーディング
- **GitHub との連携**: バージョン管理と AI アシスタントの組み合わせで効率的な開発
- **Vibe Coding**: 小規模なプロジェクトでも楽しくコーディングできる体験

### おすすめ

コードの品質向上や開発効率化を考えている方に、Devin Next をおすすめします。

- **Windsurf**: https://windsurf.ai/
- **Devin Next**: Windsurf エディタに統合された AI コーディングアシスタント
