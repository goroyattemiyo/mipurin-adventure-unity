# Unity向け パーツ分割設計

## 目的

美麗2DみぷりんをUnity上で動かすため、1枚絵ではなくパーツ分割前提で設計する。

## 推奨パーツ

- Head
- Hair_Back
- Hair_Front
- Face_Base
- Eye_L
- Eye_R
- Mouth
- Antenna_L
- Antenna_R
- Ear_L
- Ear_R
- Body
- Collar
- Sleeve_L
- Sleeve_R
- Arm_L
- Arm_R
- Hand_L
- Hand_R
- Skirt
- Leg_L
- Leg_R
- Shoe_L
- Shoe_R
- Wing_Upper_L
- Wing_Lower_L
- Wing_Upper_R
- Wing_Lower_R
- HoneyPot
- Bag
- Weapon_NeedleSpear

## Sorting Order 案

- Back hair: 0
- Wings: 1
- Body: 2
- Arms: 3
- Skirt/Legs: 4
- Head: 5
- Face parts: 6
- Antenna/Ears: 7
- Weapon/Items: 8

## 基本アニメーション

### Idle

- 体が上下にゆっくり動く
- 羽が小さく震える
- 触角がわずかに揺れる
- まばたき

### Walk / Fly

- 足を小さく動かす
- 羽ばたき強め
- 体がふわふわ上下
- 髪とスカートが揺れる

### Attack

- 体を少し前に出す
- 針スピアを突き出す
- 羽が一瞬強く開く
- 花粉/はちみつ色の軌跡

### Hurt

- 目を閉じる
- 体を後ろに倒す
- 羽が縮む
- 小さな星エフェクト

### Down

- 横倒れ
- 目を閉じる
- 羽がしおれる

## 最初の実装方針

最初から完全な2Dボーンにせず、まずは高解像度4方向スプライトで動かす。
その後、正式版でパーツ分割＋骨アニメへ移行する。
