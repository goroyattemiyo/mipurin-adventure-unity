# みぷりんの冒険 Unity 2D版
# Phase 0：設計安定化計画（Codex実行用）

- 更新日：2026-07-13 JST
- 対象リポジトリ：`goroyattemiyo/mipurin-adventure-unity`
- 正本ブランチ：`main`
- 推奨作業方式：1タスク＝1ブランチ＝1PR
- Unityバージョン：`6000.5.0f1`
- 使用Input System：`com.unity.inputsystem 1.19.0`
- 使用URP：`com.unity.render-pipelines.universal 17.5.0`

---

## 1. 目的

現在のプロジェクトは、当初のCharacterTest向けMVPを越え、以下まで実装されている。

- みぷりんの移動
- Idle / Walk / Attack / Hurt / Down
- 羽の別レイヤー表示
- 敵への攻撃
- プレイヤーの被弾・HP・ダウン
- 複数種類の敵
- Wave進行
- Nectar回収・一時強化
- 村
- NPC会話
- ストーリー進行
- ショップ
- 所持品・通貨

次の機能を増やす前に、重複実装、責務過多、暗黙的な実行時設定、Scene依存を整理する。

このPhase 0の目的は、全面的な作り直しではない。

> 現在動いている戦闘・村・ショップを維持しながら、正式ゲームへ進めるための安定した土台へ整理する。

---

## 2. 次の完成目標

次の完成地点を、単独のCharacterTestではなく、以下のVertical Sliceとする。

```text
ゲーム起動
↓
村
↓
NPCから依頼を受ける
↓
南の森へ移動
↓
敵と戦う
↓
報酬を得る
↓
村へ帰還
↓
ショップで報酬を使う
```

Phase 0では、このループへ新機能を追加しない。
既存ループを壊れにくくするための整理だけを行う。

---

## 3. 現在確認されている設計課題

### 3.1 同じ責務のクラスが二重に存在する

候補：

```text
Assets/_Project/Scripts/Player/PlayerAttack.cs
Assets/_Project/Scripts/Player/MipurinAttack.cs

Assets/_Project/Scripts/Player/PlayerHealth.cs
Assets/_Project/Scripts/Player/MipurinHealth.cs
```

現在の`Player_Mipurin.prefab`では、主に次が使用されている。

```text
MipurinAttack
MipurinHealth
```

旧クラスを即削除してはいけない。
Scene、Prefab、Editorスクリプト、他コードからの参照を検索し、未使用であることを確認してから削除する。

---

### 3.2 PlayerControllerの責務が広すぎる

現在の`PlayerController`には、移動以外に次の処理が含まれている。

- `Application.targetFrameRate`設定
- `QualitySettings.vSyncCount`設定
- `Time.fixedDeltaTime`設定
- Camera Orthographic Size設定
- みぷりんのScale設定
- Wingsの位置・Scale設定
- Dialogue状態確認
- Shop状態確認

最終的に`PlayerController`へ残す責務：

```text
移動入力を受け取る
Rigidbody2Dを移動する
最後の移動方向を保持する
操作可能状態を確認する
```

ただし、Phase 0では一度に全面分割しない。
小さな差分で順番に移す。

---

### 3.3 入力処理が複数クラスへ分散している

現在は各クラスが`Keyboard.current`、`Mouse.current`、Legacy Inputを直接参照している。

対象例：

```text
PlayerController
MipurinAttack
ScenePortalInteractable
NPCInteractable
リスタート処理
```

将来的にはInput Actionsへ一本化する。

想定Action Map：

```text
Gameplay
├─ Move
├─ Attack
├─ Interact
├─ Pause
└─ Restart

UI
├─ Navigate
├─ Submit
└─ Cancel
```

ただし、入力一本化は影響範囲が大きいため、他の安定化作業と同じPRに混ぜない。

---

### 3.4 MipurinAttackの責務が多い

現在の主な責務：

- 入力検出
- クールダウン
- 攻撃方向
- 攻撃判定
- ダメージ通知
- 攻撃エフェクト生成
- ヒットエフェクト生成
- Power Level
- アニメーション再生

特に確認が必要な点：

- 攻撃ボタンを押したフレームで即座に判定していないか
- 敵が複数Colliderを持つ場合、同じ敵へ複数回ダメージが入らないか
- `targetLayers = ~0`により意図しないColliderを検出しないか

Phase 0では、まず同一攻撃内の重複ヒット防止を優先する。

---

### 3.5 MipurinHealthの責務が多い

現在の主な責務：

- HP
- 無敵時間
- Down
- Hurt / Down表示
- 点滅
- ノックバック
- ParticleSystem動的生成
- 移動・攻撃・羽の停止
- Power Levelリセット

Phase 0では全面分割しない。
まず、HPリセット処理が攻撃強化を直接初期化している依存を記録し、後続タスクで分離する。

---

### 3.6 Player Prefabの必須Component確認

`PlayerController`は`Rigidbody2D`がない場合、Transform移動へフォールバックする。

この挙動はPrefab設定ミスを隠すため、最終的には以下へ変更する。

```csharp
[RequireComponent(typeof(Rigidbody2D))]
```

ただし、先に全Sceneのプレイヤー実体とPrefab参照を確認する。

確認対象：

```text
Assets/_Project/Prefabs/Player_Mipurin.prefab
Assets/_Project/Scenes/CharacterTest.unity
Assets/_Project/Scenes/VillageTest.unity
```

---

### 3.7 永続オブジェクトが暗黙的に生成される

対象例：

```text
StoryProgress
HoneyWallet
VillageInventory
VillageShopManager
VillageHudManager
VillageRuntimeBootstrap
```

最終的にはBootstrap SceneまたはGameSessionへ集約する。

ただし、現在の村・戦闘ループを壊す可能性が高いため、Phase 0前半では変更しない。
現状調査と依存一覧作成だけに留める。

---

### 3.8 Build Settingsの起動Scene

現在のBuild Settingsでは、`SampleScene`が先頭に残っている可能性がある。

目標順：

```text
0 Bootstrap（作成後）
1 VillageTest
2 CharacterTest
```

Bootstrap作成前の暫定順：

```text
0 VillageTest
1 CharacterTest
```

`SampleScene`はBuild Settingsから外す。
Sceneファイル自体は、別タスクで不要と判断するまで削除しない。

---

### 3.9 READMEと実装状態の不一致

READMEは初期MVPの説明に留まっている。

更新内容：

- 現在のゲーム状態
- Unityバージョン
- 起動Scene
- 操作方法
- Scene一覧
- 主要Prefab
- 主要Script
- 実装済み機能
- 既知の課題
- 次の開発フェーズ
- WebGL確認手順

README更新はコード変更と分けてもよい。

---

## 4. Phase 0の実行順

一度に全作業を実行しない。
以下を独立したタスクとして順番に行う。

---

## Task 0A：参照監査と重複クラス整理

### 目的

旧実装と現行実装の参照関係を確定する。

### 最初に読むファイル

```text
Assets/_Project/Scripts/Player/PlayerAttack.cs
Assets/_Project/Scripts/Player/MipurinAttack.cs
Assets/_Project/Scripts/Player/PlayerHealth.cs
Assets/_Project/Scripts/Player/MipurinHealth.cs
Assets/_Project/Prefabs/Player_Mipurin.prefab
```

### 検索対象

```text
PlayerAttack
MipurinAttack
PlayerHealth
MipurinHealth
```

検索範囲：

```text
Assets/_Project/
ProjectSettings/
Docs/
README.md
```

### 実行内容

1. 参照元一覧を作る
2. 現在使用中のクラスを確定する
3. 未使用クラスだけ削除する
4. 対応する`.meta`も適切に扱う
5. Compile Errorが発生しない差分にする
6. 結果を短く報告する

### 完了条件

- 使用中クラスを誤って削除していない
- Scene / PrefabのMissing Scriptを増やしていない
- コード上の参照切れがない
- 変更対象が重複クラス整理に限定されている

### 今回やらないこと

- PlayerControllerの分割
- Input System移行
- MipurinAttackの再設計
- MipurinHealthの再設計
- Bootstrap作成
- 敵・村・ショップの変更

---

## Task 0B：Build Settingsと起動経路整理

### 目的

WebGLおよびEditor再生時の起点を明確にする。

### 最初に読むファイル

```text
ProjectSettings/EditorBuildSettings.asset
Assets/_Project/Scenes/VillageTest.unity
Assets/_Project/Scenes/CharacterTest.unity
```

### 実行内容

1. Build SettingsのScene順を確認する
2. `SampleScene`をBuild Settingsから外す
3. 暫定的に`VillageTest`を先頭にする
4. `CharacterTest`を2番目にする
5. Scene名の直書き箇所を検索する
6. 存在しないScene名がないか確認する

### 完了条件

```text
0 VillageTest
1 CharacterTest
```

- Sceneファイル自体は削除しない
- ポータル遷移先がBuild Settingsに含まれている
- YAMLの不要な全面書き換えを行っていない

---

## Task 0C：PlayerControllerの隠れた設定を分離

### 目的

PlayerControllerがScene・Prefab・Application設定を実行時に上書きしないようにする。

### 最初に読むファイル

```text
Assets/_Project/Scripts/Player/PlayerController.cs
Assets/_Project/Prefabs/Player_Mipurin.prefab
Assets/_Project/Scenes/CharacterTest.unity
Assets/_Project/Scenes/VillageTest.unity
```

### 分離候補

```text
Application.targetFrameRate
QualitySettings.vSyncCount
Time.fixedDeltaTime
Camera.orthographicSize
Player scale
Wings localPosition
Wings localScale
```

### 実行内容

1. 現在のPrefabとSceneに値が保存されているか確認する
2. 実行時上書きがなくても表示が維持されることを確認する
3. `PlayerController`から表示・Camera・Application設定を除去する
4. 必要なら小さな`GameRuntimeSettings`を作る
5. CharacterTest専用値を汎用Playerコードへ残さない

### 完了条件

`PlayerController`の責務が、移動と操作可否へ近づいている。

### 注意

Unity Editorでしか安全に確認できないPrefab・Scene変更は、コードだけで推測して完了扱いにしない。
手動確認項目として明記する。

---

## Task 0D：攻撃の重複ヒット防止

### 目的

敵が複数Colliderを持っていても、1回の攻撃で同じ`IDamageable`へ1回だけダメージを与える。

### 最初に読むファイル

```text
Assets/_Project/Scripts/Player/MipurinAttack.cs
Assets/_Project/Scripts/Enemy/MipurinEnemy.cs
Assets/_Project/Scripts/Enemy/SimpleEnemy.cs
Assets/_Project/Scripts/Combat/IDamageable.cs
```

`IDamageable.cs`の実際の配置は検索で確認する。

### 実行内容

1. `OverlapCircleAll`の結果を確認する
2. 同じ`IDamageable`を重複排除する
3. 自分自身と子Colliderを除外する
4. 死亡済み対象を除外する
5. 1回の攻撃中だけ有効な`HashSet<IDamageable>`等を使う
6. 対象ごとにヒットエフェクトが1回だけ出るようにする

### 完了条件

- 1攻撃1対象1ダメージ
- 複数の別敵にはそれぞれ1回ずつ当たる
- 自分自身には当たらない
- 死亡済み敵には当たらない

### 今回やらないこと

- 攻撃フレーム同期
- コンボ
- 武器システム
- Object Pool
- 新エフェクト
- Power Level再設計

---

## Task 0E：Prefab必須Component監査

### 目的

Player PrefabがScene側の偶然の設定に依存せず、必要Componentを明示する。

### 確認項目

```text
Rigidbody2D
Collider2D
PlayerController
MipurinAttack
MipurinHealth
PlayerSpriteAnimator
PlayerWingAnimator
Body SpriteRenderer
Wings SpriteRenderer
```

### 実行内容

1. Player PrefabのComponent一覧を確認する
2. CharacterTest / VillageTestのPrefab Overrideを確認する
3. Rigidbody2DとCollider2Dの実体を確認する
4. 不足があれば、Unity Editorで追加すべき設定を報告する
5. 安全に追加できる場合だけPrefabへ反映する
6. 最終的に`PlayerController`へ`RequireComponent`を追加できるか判断する

### 完了条件

- Rigidbody2Dの有無が明確
- Collider2Dの有無が明確
- Transform移動フォールバックを残す理由または削除条件が明確
- Scene固有Overrideが記録されている

---

## Task 0F：READMEと状態資料更新

### 対象

```text
README.md
Docs/character_test_status.md
```

### 実行内容

1. READMEを現在の実装状態へ更新する
2. 初期MVP説明を履歴として残すか、現在状態と分ける
3. Vertical Slice v0.1を次の目標として記載する
4. Phase 0の完了済み・未完了を記録する
5. Unity Editorで必要な確認項目を明記する

### 完了条件

- READMEと実装状態が大きく矛盾していない
- 次に何をするか分かる
- 初見の開発者が起動Sceneと操作方法を把握できる

---

## 5. Phase 0で禁止する変更

Codexは、明示されていない限り以下を行わない。

- 全アーキテクチャの作り直し
- 全Namespaceの一括変更
- ファイル名の大量変更
- Scene YAMLの全面整形
- Prefab YAMLの全面整形
- Animatorへの全面移行
- Input Systemの同時全面移行
- ScriptableObjectの大量導入
- DIコンテナ導入
- DOTS / ECS導入
- Addressables導入
- 新しい敵・アイテム・ショップ機能追加
- 見た目だけのリファクタリング
- 関係のない警告修正
- 正式素材への差し替え
- 自動的なcommit / push / merge

スコープ外の問題を見つけた場合は、修正せず「後続候補」として報告する。

---

## 6. Codexの共通実行ルール

### 作業前

1. 現在ブランチと作業ツリーを確認する
2. 対象タスクのファイルだけ先に読む
3. `rg`等で参照箇所を検索する
4. 変更予定ファイルを列挙する
5. スコープ外変更が必要なら停止して理由を報告する

### 作業中

- 1タスクだけ実行する
- 必要な差分だけ変更する
- 無関係な整形をしない
- Unity YAMLを手作業で変更する場合は最小差分にする
- `.meta`を軽視しない
- Missing Script、GUID、Prefab参照切れを考慮する
- Unity Editorでしか確認できない事項を推測で成功扱いにしない

### 作業後

次の形式で短く報告する。

```text
結論
変更ファイル
実施内容
確認したこと
Unity Editorで人間が確認すること
残ったリスク
```

完全なファイル全文は出力しない。
差分概要だけを出力する。

---

## 7. Unity Editorでの共通確認

Codexによるコード確認だけでは完了しない。

人間がUnity Editorで次を確認する。

### VillageTest

- Sceneが開く
- ConsoleにCompile Errorがない
- みぷりんが表示される
- 移動できる
- NPCと会話できる
- Shopを開ける
- Shop中に移動しない
- 南の森へ移動できる

### CharacterTest

- Sceneが開く
- 移動できる
- Idle / Walkが切り替わる
- 攻撃できる
- 敵へ1回ずつダメージが入る
- 被弾する
- 無敵時間がある
- HP 0でDownする
- Rキーで再スタートできる
- Waveが進行する
- 村へ戻れる

### Build

- Build Settingsの先頭Sceneが正しい
- WebGL Buildが成功する
- 起動Sceneが正しい
- ブラウザ上で入力できる
- Consoleに重大エラーがない

---

## 8. Codexへ最初に渡す指示

最初はTask 0Aだけを実行する。

```text
このリポジトリの Docs/phase0_architecture_stabilization_plan.md を読み、
Task 0A「参照監査と重複クラス整理」だけを実行してください。

制約：
- 他のTaskへ進まない
- 最初に対象ファイルと参照元だけ確認する
- 未使用と確認できないクラスは削除しない
- 無関係なリファクタリングをしない
- 自動commit、push、PR作成はしない
- Unity Editorでしか確認できない点は、未確認として明記する
- 最後は、変更ファイル、確認結果、残ったリスクだけを簡潔に報告する
```

Task 0A完了後、人間が差分を確認してからTask 0Bへ進む。

---

## 9. Plusプラン内で消費を抑える運用

### 原則

```text
設計はChatGPTで固める
↓
Codexへは実装対象だけ渡す
↓
1回で1タスクだけ実行する
↓
人間がUnity Editorで確認する
↓
問題のある箇所だけCodexへ戻す
```

### Codexに毎回させないこと

- リポジトリ全体の説明
- 全コードレビュー
- 全Docsの読み込み
- Web版リポジトリとの全面比較
- 設計案の大量生成
- 実装・レビュー・README更新の同時実行
- Unity Editorでしか分からない見た目の推測
- 全警告の修正
- 将来機能の先回り実装

### プロンプトの節約

悪い例：

```text
リポジトリ全体を調べて、設計をよくして、必要なものを全部直してください。
```

良い例：

```text
計画書のTask 0Dだけを実行。
対象はMipurinAttack.csとIDamageable実装。
1攻撃中の同一対象への重複ダメージだけを防止。
他の変更は禁止。
```

### モデルの使い分け

```text
Luna
- 参照検索
- README修正
- コメント整理
- 小さな設定変更
- 単純な重複コード削除

Terra
- 通常のUnity C#修正
- 2～4ファイル程度のリファクタリング
- Build Settings整理
- Prefab参照の通常調査
- テスト追加

Sol
- 複数Scene・Prefab・Scriptをまたぐ原因不明の不具合
- Input System全面移行
- Bootstrap / GameSession設計
- 大きな状態管理変更
- Phase完了時の設計レビュー
```

通常はTerraを使う。
Solは、Terraで原因を絞れない場合か、影響範囲の大きい設計判断だけに使う。

### 会話の扱い

- 同じタスクの修正中は同じセッションを使う
- タスクが終わったら、次タスクは短い新規セッションでもよい
- 長い雑談や設計議論をCodexセッションへ混ぜない
- 完了済みタスクのログ全文を次のプロンプトへ貼らない
- 必要な決定事項だけ計画書へ反映する

### AGENTS.md

作成する場合は短くする。

ルートの`AGENTS.md`には、全作業で必要な共通規則だけを書く。

例：

```text
- Unity版正本はこのリポジトリ
- Unity 6000.5.0f1
- Assets/_Project配下を主に使用
- Library/ Temp/ Obj/ Build/を変更しない
- .metaを維持する
- 1タスク1差分
- Unity Editor未確認事項を成功扱いにしない
```

Player固有ルールは、必要なら`Assets/_Project/Scripts/Player/AGENTS.md`へ分ける。
長いプロジェクト説明をルートAGENTS.mdへ入れない。

### 出力を短く指定する

毎回、次を付ける。

```text
説明は簡潔にする。
ファイル全文を貼らない。
差分概要、テスト結果、未確認事項だけ報告する。
```

### ツールを絞る

- 必要のないWeb検索を使わせない
- 必要のないMCPサーバーを無効にする
- GitHub・Unity・ローカルファイル以外の接続を増やさない
- 画像生成をCodex作業と混ぜない

---

## 10. ブランチ例

```text
chore/phase0-remove-duplicate-player-components
chore/phase0-build-scene-order
refactor/phase0-player-controller-scope
fix/phase0-attack-deduplicate-targets
chore/phase0-player-prefab-audit
docs/phase0-readme-refresh
```

---

## 11. Phase 0完了条件

```text
[ ] 旧PlayerAttack / PlayerHealthの参照状態が確定
[ ] 未使用の重複クラスが整理済み
[ ] Build Settingsの起点が明確
[ ] PlayerControllerが表示・Camera設定を上書きしない
[ ] 同じ敵へ1攻撃で複数回ダメージが入らない
[ ] Player Prefabの必須Componentが明確
[ ] Unity EditorでVillageTestを確認済み
[ ] Unity EditorでCharacterTestを確認済み
[ ] WebGLの起動Sceneを確認済み
[ ] READMEと現在状態が一致
[ ] Consoleに重大エラーがない
```

---

## 12. 次のPhase

Phase 0完了後、以下へ進む。

```text
Phase 1：戦闘コア安定化
- 攻撃アニメと判定タイミング同期
- Healthイベント化
- 被弾演出分離
- Down状態統一

Phase 2：ゲーム進行一本化
- Bootstrap
- GameSession
- SceneFlow
- 村 → 森 → 村
- 報酬持ち帰り

Phase 3：正式UI
- Debug HUDのuGUI化
- Wave UI
- Result UI
- Pickup表示

Phase 4：正式素材
- みぷりん
- 敵
- Collider再調整
- エフェクトPrefab
- 効果音

Phase 5：WebGL Vertical Slice v0.1
```
