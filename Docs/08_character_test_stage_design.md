# 08 CharacterTest Stage Design

## 目的

`CharacterTest` シーンを、単なる動作確認ではなく、短く遊べるテストステージにする。

このステージは、Unity版『ミプリンの冒険』の1部屋バトルの基礎になる。

## 現在の状態

完了済み：

- Player_Mipurin Prefab
- 本体＋羽レイヤー
- 移動
- Idle / Walk
- Attack / Hurt / Down
- 敵HP / 撃破
- 接触ダメージ
- 攻撃エフェクト
- ヒットエフェクト
- 敵ノックバック
- 敵点滅
- DebugHUD
- カメラ追従

## 次フェーズの目的

CharacterTestを、以下の状態にする。

```text
みぷりんを動かす
敵を倒す
ダメージを受ける
ダウンする
攻撃エフェクトが見える
ヒット演出が見える
画面外に出ない
床とステージ枠がある
```

## ステージの基本仕様

### 画面

- 見下ろし型2D
- 1部屋制
- 画面内にプレイヤーと敵が収まる
- 背景色は暗すぎず、キャラとエフェクトが見やすい色

### 床

配置予定：

```text
Assets/_Project/Sprites/Stage/CharacterTest/
```

ファイル名案：

```text
floor_soft_grass_01.png
floor_soft_grass_02.png
```

デザイン方針：

- やさしい草地
- 花の国の入口感
- みぷりんと敵が見やすい淡い色
- 主張しすぎない
- タイルとして並べても違和感が少ない
- 正方形PNG

### ステージ枠

ファイル名案：

```text
stage_border_flower_01.png
stage_corner_flower_01.png
```

デザイン方針：

- 花、草、蜂の巣モチーフ
- CharacterTestの外枠として使う
- かわいいが邪魔にならない
- 床より少し濃い色
- ゲーム画面の境界が分かる

## 移動制限

みぷりんが画面外へ出ないようにする。

実装予定：

```text
PlayerBoundsLimiter.cs
```

制限対象：

- プレイヤー本体
- カメラ範囲
- 攻撃時の前進
- ノックバック後の位置

## ステージ自動セットアップ

Unityエディタメニューからステージを生成できるようにする。

予定メニュー：

```text
Mipurin → Setup → Setup Character Test Stage
```

実装予定：

```text
MipurinStageSetupTools.cs
```

生成・調整内容：

- 背景色
- 床
- ステージ枠
- PlayerBoundsLimiter
- 敵初期位置
- カメラ設定
- DebugHUD位置

## 敵配置

最初は1体。

```text
Enemy_Test
```

配置方針：

- プレイヤーから近すぎない
- 攻撃テストしやすい距離
- 接触ダメージ確認もできる距離
- カメラ内に入る

## 合格ライン

CharacterTestステージ化の合格ライン：

```text
1. みぷりんが画面外へ出ない
2. 敵が画面内にいる
3. 攻撃エフェクトが見える
4. ヒットエフェクトが見える
5. 敵を倒せる
6. みぷりんが被弾する
7. HP 0でDown表示になる
8. 床と枠が見やすい
9. カメラが不自然に動かない
```

## 今はやらないこと

- 部屋ランダム生成
- 報酬選択
- ボス
- 複数ステージ
- 本格背景
- 拠点
- 図鑑

これらは、CharacterTestが遊べる1部屋になった後に進める。
