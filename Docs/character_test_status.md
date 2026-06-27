# CharacterTest 状態メモ

Unity版『ミプリンの冒険』の初期MVP確認用シーン `CharacterTest` の現在状態をまとめる。

このドキュメントは、あとから実装内容・仮素材・確認手順を見失わないための作業メモとして使う。

## 現在できること

### シーン

- `Assets/_Project/Scenes/CharacterTest.unity` で確認する
- 草原フィールドを表示できる
- カメラはテスト用に固定寄りの見やすい範囲へ調整済み
- 旧テストステージは非表示運用

### プレイヤー

- `Player_Mipurin` を表示できる
- WASD / 矢印キーで移動できる
- Space / 左クリックで攻撃できる
- Idle / Walk / Attack / Hurt / Down の仮アニメがある
- 羽レイヤーを別表示できる
- 被弾時に赤点滅、ノックバック、赤〜オレンジ系の仮バーストが出る
- HPが0になるとDown状態になり、移動・攻撃・羽アニメが止まる

### 敵

- `Enemy_HoneySlime.prefab` をWaveで生成できる
- `Enemy_PoisonMushroom.prefab` をWave 2以降で生成できる
- 敵出現時に黄色系の仮バーストが出る
- 敵はみぷりんへ近づく
- HoneySlimeは少し速く軽め
- PoisonMushroomは遅く重め
- 敵HP、被弾、撃破が動作する
- 敵撃破時にNectarを落とす

### アイテム

- `Pickup_HoneyNectar.prefab` を回収できる
- みぷりんに近づくと吸い寄せ回収される
- 回収時にNectarが加算される
- `Nectar +1` / `Nectar +2` の浮遊テキストが出る
- `honey_nectar_01.png` はUnityメニューから生成する方式

### Wave / クリア

- Wave 1〜5まで進行する
- 敵全滅後、少し待って次Waveへ進む
- Nectar 10到達、またはWave 5完了で `TEST COMPLETE`
- クリア時にNectar数、撃破数、到達Waveを表示する
- Rキーでテストをリスタートできる

## 確認手順

1. Unityで `Assets/_Project/Scenes/CharacterTest.unity` を開く
2. 必要に応じて以下のメニューを実行する

```text
Mipurin → Setup → Setup Character Test Wave Loop
Mipurin → Setup → Setup Character Test Camera Frame
Mipurin → Setup → Setup Character Test Player Bounds
Mipurin → Setup → Setup Honey Nectar Sprite
```

3. Playする
4. WASD / 矢印キーで移動する
5. Space / 左クリックで敵を攻撃する
6. 敵を倒してNectarを拾う
7. `TEST COMPLETE` が出るまで確認する
8. Rキーで再スタートできるか確認する

## 仮素材・仮演出

### 仮素材

- みぷりん本体画像はまだサイズ・見た目ともに仮
- みぷりんが現状やや大きく、画面内の状況確認が少ししづらい
- Nectar画像 `honey_nectar_01.png` はEditor生成の仮PNG
- Nectarはポット風だが、現状少し小さい
- 敵画像は現時点の仮素材
- README用プレビュー画像は未対応

### 仮演出

- 敵出現演出はParticleSystemの黄色バースト
- みぷりん被弾演出はParticleSystemの赤〜オレンジバースト
- `damage_star_01.png` 実素材はまだ未使用
- 専用エフェクトPrefab化はまだしていない

## 後で直すこと

### 優先度高

- みぷりんの表示サイズを少し小さくする
- みぷりんのPrefabスケール、Collider、攻撃範囲の整合を取る
- Nectarの表示サイズを少し大きくする
- Nectar画像の余白を減らす、またはPrefab scaleを調整する
- 被弾時のノックバック量が強すぎないか確認する

### 優先度中

- `damage_star_01.png` を実素材で追加する
- 敵出現演出を専用PNGまたはPrefabに置き換える
- 敵の種類ごとの接触ダメージ間隔や攻撃感を調整する
- Waveごとの敵数を調整する
- HUDの文字サイズ、背景、配置を正式UIに寄せる

### 優先度低

- クリア演出を専用UI化する
- リザルト表示をCanvas UIへ移行する
- サウンド追加
- 背景・草原フィールドの見た目強化

## 次の実装候補

1. みぷりんサイズ調整
2. Collider / 攻撃範囲の見直し
3. Nectarサイズ調整
4. damage_star実素材対応
5. 敵AIの種類追加
6. Waveバランス調整
7. CharacterTestをMVP確認用の正式テストシーンとして整理

## 現在の判定

```text
表示: OK
移動: OK
Idle / Walk: OK
羽: OK
攻撃: OK
敵1体への攻撃: OK
敵HP / 被弾 / 撃破: OK
みぷりんHP / 被弾 / ダウン: OK
Nectarドロップ / 回収: OK
Waveループ: OK
クリア表示: OK
```

MVPの土台は一通り動いている。次は見た目サイズ、当たり判定、素材差し替えの調整フェーズに入れる。
