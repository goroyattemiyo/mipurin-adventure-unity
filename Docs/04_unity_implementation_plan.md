# Unity実装計画

## Phase 0：プロジェクト作成

Unity Hubで2D Coreテンプレートを選択。
プロジェクト名：mipurin-adventure-unity

## Phase 1：Player Prefab

作成するPrefab：

Assets/_Project/Prefabs/Player/Player_Mipurin.prefab

構成：

Player_Mipurin
- SpriteRenderer
- Animator
- Rigidbody2D
- CapsuleCollider2D
- PlayerController
- PlayerHealth
- PlayerAttack
- PlayerAnimationController

## Phase 2：CharacterTest

CharacterTest.unity で以下を確認。

- 移動
- 向き変更
- Idle
- Walk
- Attack
- Hurt
- Down

## Phase 3：1部屋バトル

Game.unity で以下を確認。

- 床と壁
- 敵1体
- 攻撃判定
- HP
- 敵撃破
- 部屋クリア

## Phase 4：ローグライト化

- 部屋生成
- 武器ドロップ
- 祝福
- フロア進行
- ボス
- 花壇/図鑑
