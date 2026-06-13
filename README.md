# hisclear

Linux 環境で .bash_history をクリーンアップし、server-*.log ファイルを削除するプログラム。

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

# プロジェクトをビルド
dotnet build

# 実行ファイルのコピーを確保
dotnet publish -c Release

# Self-contained バージョンを作成（.NET ランタイム不要）
dotnet publish -c Release --self-contained true -r linux-x64
```

## 使用方法

```bash
# プログラムを実行
dotnet run

# またはビルド済みの実行ファイルを使用
./bin/Release/net10.0/hisclear
```

### エイリアスの設定

頻繁に使用する場合、シェル設定ファイルにエイリアスを追加すると便利です。

#### bash の場合（~/.bashrc）

```bash
# ~/.bashrc に以下を追加
alias hc='<path to>/hisclear'

# 設定を反映
source ~/.bashrc

# 使用方法
$ hc
```

#### csh/tcsh の場合（~/.cshrc）

```csh
# ~/.cshrc に以下を追加
alias hc '<path to>/hisclear'

# 設定を反映
source ~/.cshrc

# 使用方法
% hc
```

#### Self-contained バージョンを使用する場合

```bash
# bash (~/.bashrc)
alias hc='<path to>/hisclear'

# csh/tcsh (~/.cshrc)
alias hc '<path to>/hisclear'
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

**重要**: `default_history.txt` `messages.ini`はビルドした `hisclear` 実行ファイルと**同じディレクトリ**に配置する必要があります。

```bash
# 開発環境の場合
hisclear/
├── Program.cs
├── hisclear.csproj
├── messages.ini
├── default_history.txt
└── bin/
    └── Release/
        └── net10.0/
            ├── hisclear          # 実行ファイル
            ├── messages.ini      # ここにもコピーされる
            └── default_history.txt  # ここにもコピーされる必要あり

# 配置方法
cp default_history.txt bin/Release/net10.0/
cp messages.ini bin/Release/net10.0/
```

Self-contained バージョンを使用する場合：

```bash
# Self-contained バージョンの配置
cp default_history.txt bin/Release/net10.0/linux-x64/publish/
cp messages.ini bin/Release/net10.0/linux-x64/publish/
```

プロジェクトファイル（hisclear.csproj）に以下の設定を追加すると、ビルド時に自動的にコピーされます：

```xml
<ItemGroup>
  <None Include="messages.ini" CopyToOutputDirectory="PreserveNewest" />
  <None Include="default_history.txt" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

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

### Devin Next について

Devin Next は VSCode ライクに使用できる AI コーディングアシスタントです。Windsurf の Devin Next を使用すると、ちょっとしたサイズのコーディングであれば課金なしで **Vibe Coding** を楽しむことができます。

- **VSCode ライクな操作**: 親しみやすいインターフェースでスムーズにコーディング
- **GitHub との連携**: バージョン管理と AI アシスタントの組み合わせで効率的な開発
- **Vibe Coding**: 小規模なプロジェクトでも楽しくコーディングできる体験

### おすすめ

コードの品質向上や開発効率化を考えている方に、Devin Next をおすすめします。

- **Windsurf**: https://windsurf.ai/
- **Devin Next**: Windsurf エディタに統合された AI コーディングアシスタント
