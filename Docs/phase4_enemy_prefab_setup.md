# Phase 4 敵Prefab分離メモ

Phase 4では、Phase 3で追加した敵タイプを正式Prefabへ分離していく。

## 現在の対応

GitHub側で以下を追加済み。

```text
Assets/_Project/Scripts/Editor/MipurinPhase4EnemyPrefabSetupTools.cs
Assets/_Project/Scripts/Editor/MipurinPhase4EnemyPrefabSetupTools.cs.meta
```

Unityメニュー：

```text
Mipurin > Setup > Setup Phase 4 Enemy Prefabs
```

このメニューを実行すると、既存の敵Prefabをベースに以下を生成する。

```text
Assets/_Project/Prefabs/Enemies/Enemy_StingerBee.prefab
Assets/_Project/Prefabs/Enemies/Enemy_FlowerTurret.prefab
Assets/_Project/Prefabs/Enemies/Enemy_HeavyBeetle.prefab
```

既存Prefab：

```text
Assets/_Project/Prefabs/Enemies/Enemy_HoneySlime.prefab
Assets/_Project/Prefabs/Enemies/Enemy_PoisonMushroom.prefab
```

## Spawner側の対応

`CharacterTestEnemySpawner` は、正式5種Prefabに対応済み。

```text
HoneySlime
PoisonMushroom
StingerBee
FlowerTurret
HeavyBeetle
```

新Prefabが存在する場合はそれを使う。
まだ無い場合は、従来通り既存Prefabをベースに仮生成するfallbackを残している。

## Wave Loop Setup側の対応

`MipurinWaveLoopSetupTools` は、5種Prefabを読み込んでSpawnerへ渡す形へ更新済み。

```text
Mipurin > Setup > Setup Character Test Wave Loop
```

正式Prefab生成後にこのメニューを実行すると、`CharacterTest` のSpawner参照も5種Prefabへ寄せられる。

## 注意

`.prefab` はGitHub上で手書きするとUnityで壊れやすい。
そのため、Prefab本体の生成はUnity Editor上のセットアップツールで行う。

生成されたPrefabをGitHubへ入れる場合は、ローカルで以下のファイルが増える。

```text
Assets/_Project/Prefabs/Enemies/Enemy_StingerBee.prefab
Assets/_Project/Prefabs/Enemies/Enemy_StingerBee.prefab.meta
Assets/_Project/Prefabs/Enemies/Enemy_FlowerTurret.prefab
Assets/_Project/Prefabs/Enemies/Enemy_FlowerTurret.prefab.meta
Assets/_Project/Prefabs/Enemies/Enemy_HeavyBeetle.prefab
Assets/_Project/Prefabs/Enemies/Enemy_HeavyBeetle.prefab.meta
```

現時点では、Prefabが未生成でもCharacterTestはfallbackで動く。

## 確認手順

1. Unityで `Assets/_Project/Scenes/CharacterTest.unity` を開く
2. `Mipurin > Setup > Setup Phase 4 Enemy Prefabs` を実行する
3. `Mipurin > Setup > Setup Character Test Wave Loop` を実行する
4. Playする
5. Wave 3以降でStingerBeeが出るか確認する
6. Wave 4以降でFlowerTurretが出るか確認する
7. Wave 5でHeavyBeetleが出るか確認する
8. Rキーで再スタートできるか確認する

## 次の作業

敵Prefab分離が動いたら、次は敵正式素材の差し替えへ進む。

```text
HoneySlime正式素材
PoisonMushroom正式素材
StingerBee正式素材
FlowerTurret正式素材
HeavyBeetle正式素材
```
