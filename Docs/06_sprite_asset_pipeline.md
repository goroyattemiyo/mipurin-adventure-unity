# 06 Sprite Asset Pipeline

## 目的

Unity版『ミプリンの冒険』で使用するキャラクター・羽・エフェクト素材の制作方針を固定する。

このドキュメントは、素材制作チャット・Unity実装チャット・GitHub管理チャットで判断がブレないようにするための正本とする。

## 現在の採用方式

最初から全身パーツ分割にはしない。

採用方式は以下。

```text
本体フレームアニメ + 羽だけ別レイヤー + エフェクト + Unity Transform演出
```

## レイヤー構成

```text
Layer 3: 攻撃・ヒット・ダメージ・移動エフェクト
Layer 2: 羽
Layer 1: みぷりん本体
Layer 0: 影・床
```

## 本体素材

みぷりん本体は、1枚絵のフレームアニメとして扱う。

配置予定：

```text
Assets/_Project/Sprites/Player/Mipurin/Body/
```

最初の正面素材：

```text
front_idle_01.png
front_idle_02.png
front_walk_01.png
front_walk_02.png
front_walk_03.png
front_attack_01.png
front_attack_02.png
front_attack_03.png
front_hurt_01.png
front_down_01.png
```

## 羽素材

羽は本体とは別レイヤーにする。

理由：

- 蜂キャラクターとして羽ばたきが重要
- 本体フレームを増やしすぎずに生き物感を出せる
- Unity側でIdle / Flapを切り替えやすい

配置予定：

```text
Assets/_Project/Sprites/Player/Mipurin/Wings/
```

素材：

```text
wing_idle_01.png
wing_idle_02.png
wing_flap_01.png
wing_flap_02.png
wing_flap_03.png
```

## エフェクト素材

攻撃・ヒット・移動・被弾は、別PNGとして扱う。

配置予定：

```text
Assets/_Project/Sprites/Effects/
```

素材：

```text
slash_yellow_01.png
honey_spark_01.png
wing_dust_01.png
damage_star_01.png
```

## Unity側で足す演出

画像だけで全てを表現しない。

Unity側で以下を加える。

- 上下ふわふわ
- 移動方向への軽い傾き
- 攻撃時の前進
- 被弾時のノックバック
- 被弾時の点滅
- 敵ヒット時のノックバック
- 敵ヒット時の点滅
- カメラ追従
- 必要に応じて画面揺れ

## 素材形式

現在はPNGのみ。

- Unity投入用：PNG
- WebP：使わない
- JPG：使わない
- README用プレビュー：後回し

白背景PNGで生成した素材は、必要に応じて後から透過PNG化する。

## 注意

`Docs/03_unity_parts_split.md` は将来案として扱う。

現時点の正本方針は、この `06_sprite_asset_pipeline.md` の方式とする。
