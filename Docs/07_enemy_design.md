# 07 Enemy Design

## 目的

Unity版『ミプリンの冒険』に登場する敵の設計方針をまとめる。

敵は完全新規で作らず、Web版 `goroyattemiyo/mipurin-adventure` の敵定義を参考元・正本として、Unity版向けに美麗2D化・簡略実装する。

## 基本方針

- Web版の敵名・役割・テーマを引き継ぐ
- Unity版では、まず見た目・HP・接触ダメージ・ノックバック確認を優先する
- 最初から複雑なAIは入れない
- 怖すぎず、かわいいが敵として判別できるデザインにする
- プレイヤーのみぷりんより目立ちすぎない
- 画面上で当たり判定が分かりやすいシルエットにする

## Web版から引き継ぐ通常敵

```text
mushroom    : どくキノコ
slime       : はちみつスライム
spider      : あみぐもちゃん
bat         : やみコウモリ
beetle      : かぶとむしナイト
wasp        : わるいハチ
flower      : パクパクフラワー
worm        : もぐもぐイモムシ
ghost       : ひとだまホタル
golem       : いわいわゴーレム
vine        : つるつるツタ
queenbee    : クモの女王
splitslime  : わかれスライム
darkbee     : ダークビー
```

## Web版から引き継ぐボス

```text
queen_hornet   : スズメバチの女王
fungus_king    : キノコの王
crystal_golem  : クリスタルゴーレム
shadow_moth    : 闇の蛾
dark_root      : 闇の根
```

## 最初に作る敵

最初の正式差し替え対象は `slime`。

Unity版名称：

```text
はちみつスライム
```

用途：

- CharacterTest用の基本敵
- 接触ダメージ確認
- HP・撃破処理確認
- 攻撃ヒット確認
- ノックバック確認
- 点滅確認

## はちみつスライム素材

配置予定：

```text
Assets/_Project/Sprites/Enemies/
```

ファイル名：

```text
enemy_honey_slime_idle_01.png
enemy_honey_slime_idle_02.png
enemy_honey_slime_hurt_01.png
enemy_honey_slime_down_01.png
```

## はちみつスライム デザイン方針

- 丸くてかわいい
- 半透明の黄金色
- はちみつのぷるぷる感
- 小さな悪そうな目
- みぷりんより少し低い背丈
- 最初の敵なので怖すぎない
- Unity画面で見やすい輪郭
- 白背景または透過背景向け
- 1体のみ
- 文字なし

## 次に作る敵

2体目は `mushroom`。

Unity版名称：

```text
どくキノコ
```

ファイル名案：

```text
enemy_poison_mushroom_idle_01.png
enemy_poison_mushroom_idle_02.png
enemy_poison_mushroom_hurt_01.png
enemy_poison_mushroom_down_01.png
```

## 敵実装の段階

### Phase 1

- 敵1体
- 接触ダメージ
- HP
- 撃破
- ノックバック
- 点滅

### Phase 2

- 敵を複数体配置
- 部屋クリア判定
- 敵ごとのHP差
- 敵ごとの移動速度差

### Phase 3

- 敵ごとの簡易行動
- 遠距離攻撃
- 分裂
- 突進
- 設置攻撃

### Phase 4

- ボス実装
- 攻撃パターン
- フェーズ変化
- ボス撃破演出
