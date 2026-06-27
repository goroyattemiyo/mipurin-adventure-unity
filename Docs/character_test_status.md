# CharacterTest 状態メモ

Unity版『ミプリンの冒険』の初期MVP確認用シーン `CharacterTest` の現在状態をまとめる。

このドキュメントは、あとから実装内容・仮素材・確認手順を見失わないための作業メモとして使う。

## Phase 2 判定

```text
Phase 1：CharacterTestを完成させる → 完了
Phase 2：ゲームとしての基本ループ作成 → 完了扱い
```

`CharacterTest` は、移動・攻撃・敵撃破・被弾・Nectar回収・Wave進行・強化・Wave報酬・クリアまでを確認できる状態になった。

以後は、`CharacterTest` を土台にして **Phase 3：敵と戦闘を増やす** へ進む。

## 現在できること

### シーン

- `Assets/_Project/Scenes/CharacterTest.unity` で確認する
- 草原フィールドを表示できる
- 16:9 / 4K確認時の速度感差を抑えるため、フレームレートとFixedDeltaTimeを固定している
- カメラOrthographic Sizeはテスト用に `3.2` へ固定している

### プレイヤー

- `Player_Mipurin` を表示できる
- WASD / 矢印キーで移動できる
- Space / 左クリックで攻撃できる
- Idle / Walk / Attack / Hurt / Down の仮アニメがある
- 羽レイヤーを別表示できる
- 表示サイズはPhase 1用に小さめへ調整済み
- 攻撃範囲・攻撃エフェクトサイズをPhase 1用に調整済み
- 攻撃判定は広すぎないよう再調整済み
- 被弾時に赤点滅、ノックバック、赤〜オレンジ系の仮バーストが出る
- HPが0になるとDown状態になり、移動・攻撃・羽アニメが止まる
- RキーでHP・Nectar・Power Levelを含めてテスト再スタートできる

### Nectar / Power Up

- `Pickup_HoneyNectar.prefab` を回収できる
- みぷりんに近づくと吸い寄せ回収される
- 回収時にNectarが加算される
- `Nectar +1` / `Nectar +2` の浮遊テキストが出る
- Nectar 5到達で `POWER UP Lv.1`
- Nectar 10到達で `POWER UP Lv.2`
- Power Levelに応じて攻撃クールダウンが短くなる
- Power Levelに応じて攻撃範囲が少しだけ広がる
- HUDに `Power: Lv.X` を表示する
- クリアリザルトにPower Levelを表示する

### 敵

- `Enemy_HoneySlime.prefab` をWaveで生成できる
- `Enemy_PoisonMushroom.prefab` をWave 2以降で生成できる
- 敵出現時に黄色系の仮バーストが出る
- 敵はみぷりんへ近づく
- HoneySlimeは少し速く軽めの接近敵
- PoisonMushroomは遅く重めで、近くにいると毒エリアで継続ダメージ
- 敵の接触ダメージ間隔・範囲をPhase 1用に調整済み
- 敵HP、被弾、撃破が動作する
- 敵撃破時にNectarを落とす

### Wave / 報酬 / クリア

- Wave 1〜5まで進行する
- 敵全滅後、少し待って次Waveへ進む
- Waveクリア時に `WAVE BONUS` 表示
- HPが減っていればWaveクリア時にHP +1
- Nectar 12到達、またはWave 5完了で `TEST COMPLETE`
- クリア時にNectar数、Power Level、撃破数、Wave Bonus回数、到達Waveを表示する
- Rキーでテストをリスタートできる

## 確認手順

1. Unityで `Assets/_Project/Scenes/CharacterTest.unity` を開く
2. Playする
3. WASD / 矢印キーで移動する
4. Space / 左クリックで敵を攻撃する
5. 敵を倒してNectarを拾う
6. Nectar 5で `POWER UP Lv.1` が出るか確認する
7. Nectar 10で `POWER UP Lv.2` が出るか確認する
8. Waveクリア時に `WAVE BONUS` が出るか確認する
9. ダメージを受けた状態でWaveクリアし、HP +1されるか確認する
10. Wave 2以降でPoisonMushroomの毒エリアを確認する
11. `TEST COMPLETE` が出るまで確認する
12. Rキーで再スタートできるか確認する

## Phase 1で調整済み

```text
みぷりんサイズ：調整済み
攻撃範囲：調整済み
攻撃エフェクトサイズ：調整済み
Nectar表示サイズ：調整済み
Nectar吸い寄せ：調整済み
HoneySlime接触ダメージ：調整済み
PoisonMushroom接触ダメージ：調整済み
PoisonMushroom毒エリア：調整済み
Wave敵数：調整済み
クリア条件：Nectar 12 or Wave 5
```

## Phase 2で追加済み

```text
Nectar 5 / 10 Power Up：追加済み
攻撃クールダウン短縮：追加済み
攻撃範囲の段階的強化：追加済み
POWER UP表示：追加済み
Waveクリア報酬：追加済み
Wave Bonus HP +1：追加済み
Rキーのテスト再スタート復帰：追加済み
リザルトPower Level表示：追加済み
```

## 仮素材・仮演出

### 仮素材

- みぷりん本体画像はまだ仮
- Nectar画像 `honey_nectar_01.png` はEditor生成の仮PNG
- 敵画像は現時点の仮素材
- README用プレビュー画像は未対応

### 仮演出

- 敵出現演出はParticleSystemの黄色バースト
- みぷりん被弾演出はParticleSystemの赤〜オレンジバースト
- PoisonMushroom毒エリアはParticleSystemの紫パルス
- Power Up / Wave Bonusは浮遊テキストの仮表示
- `damage_star_01.png` 実素材はまだ未使用
- 専用エフェクトPrefab化はまだしていない

## 後で直すこと

### 優先度高

- みぷりん正式素材差し替え後に、サイズ・Collider・攻撃範囲を再調整する
- Nectar正式素材差し替え後に、画像内余白とPrefab scaleを再調整する
- 敵正式素材差し替え後に、Colliderを再調整する

### 優先度中

- `damage_star_01.png` を実素材で追加する
- 敵出現演出を専用PNGまたはPrefabに置き換える
- HUDをDebug GUIから正式Canvas UIへ移行する
- Waveごとの敵数を本番用に再調整する
- Power Up演出を専用エフェクト化する
- Wave報酬を選択式にする

### 優先度低

- クリア演出を専用UI化する
- リザルト表示をCanvas UIへ移行する
- サウンド追加
- 背景・草原フィールドの見た目強化

## 次の実装候補

1. 追加敵 `StingerBee`
2. 追加敵 `FlowerTurret`
3. Stage_Grassland_01 作成
4. 正式素材差し替え
5. Debug HUDを正式UIへ移行
6. Wave報酬を選択式にする

## 現在の判定

```text
表示: OK
移動: OK
Idle / Walk: OK
羽: OK
攻撃: OK
攻撃範囲: OK
敵1体への攻撃: OK
敵HP / 被弾 / 撃破: OK
敵の役割差: OK
毒エリア: OK
みぷりんHP / 被弾 / ダウン: OK
Nectarドロップ / 回収: OK
Nectar Power Up: OK
Wave Bonus: OK
Waveループ: OK
クリア表示: OK
Rキー再スタート: OK
Phase 1: 完了扱い
Phase 2: 完了扱い
```

MVPの基本ループは一通り動いている。次は敵種類を増やして、戦闘の立ち回りを強化する。
