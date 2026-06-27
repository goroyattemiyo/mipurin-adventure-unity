# Phase 4 正式素材チェックリスト

Unity版『ミプリンの冒険』の Phase 4 は、仮素材から正式素材へ差し替え、画面を「みぷりんのゲーム」として見える状態へ近づける段階。

このドキュメントは、必要なPNG、配置先、差し替え順、確認項目を整理するためのチェックリストとして使う。

## Phase 4 の目的

```text
仮キャラ・仮敵・仮エフェクトを正式PNGへ差し替える
CharacterTestのゲーム性は維持したまま見た目を強化する
素材差し替え後にサイズ・Collider・攻撃範囲を再調整する
```

## 進め方

一度に全部差し替えない。

```text
1. みぷりん本体
2. 羽
3. 攻撃エフェクト
4. Nectar
5. 敵
6. 被弾・撃破エフェクト
7. サイズ・当たり判定再調整
```

各ステップごとに、Unityで `CharacterTest` を再生して確認する。

## 素材配置先

```text
Assets/_Project/Sprites/Player/Mipurin/Body/
Assets/_Project/Sprites/Player/Mipurin/Wings/
Assets/_Project/Sprites/Effects/
Assets/_Project/Sprites/Items/Pickups/
Assets/_Project/Sprites/Enemies/
```

敵素材用フォルダは、まだなければ作成する。

```text
Assets/_Project/Sprites/Enemies/HoneySlime/
Assets/_Project/Sprites/Enemies/PoisonMushroom/
Assets/_Project/Sprites/Enemies/StingerBee/
Assets/_Project/Sprites/Enemies/FlowerTurret/
Assets/_Project/Sprites/Enemies/HeavyBeetle/
```

## 1. みぷりん本体素材

配置先：

```text
Assets/_Project/Sprites/Player/Mipurin/Body/
```

必要PNG：

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

確認項目：

```text
Idleが自然に見える
Walkが動いて見える
Attack時に破綻しない
Hurtが分かる
Downが分かる
サイズが大きすぎない
Colliderと見た目が大きくズレない
```

## 2. 羽素材

配置先：

```text
Assets/_Project/Sprites/Player/Mipurin/Wings/
```

必要PNG：

```text
wing_idle_01.png
wing_idle_02.png
wing_flap_01.png
wing_flap_02.png
wing_flap_03.png
```

確認項目：

```text
本体の背中付近に合う
羽だけ浮きすぎない
移動時・攻撃時に違和感が少ない
羽ばたき速度が速すぎない
```

## 3. 攻撃エフェクト素材

配置先：

```text
Assets/_Project/Sprites/Effects/
```

必要PNG：

```text
slash_yellow_01.png
honey_spark_01.png
```

確認項目：

```text
攻撃エフェクトが大きすぎない
みぷりんから離れすぎない
攻撃判定と見た目が大きくズレない
Power Up後も邪魔にならない
```

## 4. Nectar素材

配置先：

```text
Assets/_Project/Sprites/Items/Pickups/
```

必要PNG：

```text
honey_nectar_01.png
```

確認項目：

```text
小さすぎない
背景に埋もれない
回収対象だと分かる
Nectar +1 / +2表示と被りすぎない
```

## 5. 敵素材

### HoneySlime

配置先：

```text
Assets/_Project/Sprites/Enemies/HoneySlime/
```

候補PNG：

```text
honey_slime_idle_01.png
honey_slime_idle_02.png
honey_slime_hurt_01.png
honey_slime_down_01.png
```

役割：

```text
基本の接近敵
軽め
黄色・はちみつ感
```

### PoisonMushroom

配置先：

```text
Assets/_Project/Sprites/Enemies/PoisonMushroom/
```

候補PNG：

```text
poison_mushroom_idle_01.png
poison_mushroom_idle_02.png
poison_mushroom_hurt_01.png
poison_mushroom_down_01.png
```

役割：

```text
遅いが近づくと毒エリアで危険
紫・毒きのこ感
```

### StingerBee

配置先：

```text
Assets/_Project/Sprites/Enemies/StingerBee/
```

候補PNG：

```text
stinger_bee_idle_01.png
stinger_bee_idle_02.png
stinger_bee_dash_01.png
stinger_bee_hurt_01.png
stinger_bee_down_01.png
```

役割：

```text
警告後に突進する敵
小さく速い
蜂・針感
```

### FlowerTurret

配置先：

```text
Assets/_Project/Sprites/Enemies/FlowerTurret/
```

候補PNG：

```text
flower_turret_idle_01.png
flower_turret_idle_02.png
flower_turret_shoot_01.png
flower_turret_hurt_01.png
flower_turret_down_01.png
```

役割：

```text
動かない射撃敵
花型の砲台
かわいいが危険
```

### HeavyBeetle

配置先：

```text
Assets/_Project/Sprites/Enemies/HeavyBeetle/
```

候補PNG：

```text
heavy_beetle_idle_01.png
heavy_beetle_idle_02.png
heavy_beetle_walk_01.png
heavy_beetle_hurt_01.png
heavy_beetle_down_01.png
```

役割：

```text
遅いが硬い重敵
茶色・甲虫感
```

## 6. 被弾・撃破エフェクト

配置先：

```text
Assets/_Project/Sprites/Effects/
```

必要PNG：

```text
damage_star_01.png
wing_dust_01.png
```

確認項目：

```text
被弾が分かる
撃破が分かる
画面を邪魔しすぎない
ParticleSystem仮演出と置き換えられるか確認する
```

## 7. 差し替え後に再調整するもの

素材差し替え後は、以下を必ず見直す。

```text
みぷりんのScale
みぷりんのCollider
攻撃判定 attackRadius / forwardOffset
攻撃エフェクトScale
HitエフェクトScale
Nectar Scale
敵Collider
敵接触判定
毒エリア半径
StingerBee突進速度
FlowerTurret弾速
HeavyBeetle HP
```

## Phase 4 完了条件

```text
みぷりんが正式素材で動く
羽が別レイヤーで動く
攻撃エフェクトが正式素材になる
Nectarが正式素材になる
主要敵5種が見た目で判別できる
CharacterTestを最初からクリアまで遊べる
見た目が仮ゲームではなく、みぷりんのゲームに見える
```

## 最初に作るべき素材

Phase 4の最初は、以下だけでよい。

```text
front_idle_01.png
front_idle_02.png
front_walk_01.png
front_walk_02.png
front_walk_03.png
wing_idle_01.png
wing_idle_02.png
wing_flap_01.png
wing_flap_02.png
wing_flap_03.png
slash_yellow_01.png
honey_nectar_01.png
```

まずは「みぷりんが正式素材で動く」状態を最優先にする。
