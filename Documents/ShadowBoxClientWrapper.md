# クラス ShadowBoxClientWrapper

継承: MonoBehaviour

## 列挙型



### 列挙: BlockLayer

| 名前 | 値 |
|:-------------------|------:|
| InsideWall | 1 |
| InsideBlock | 2 |
| OutsideWall | 3 |
| OutsideBlock | 4 |


----

## 構造体



### 構造体: PlayerData

| 名前 | 型 |
|:-------------------|:--------|
| name | string |
| skinType | int |
| playerID | Guid |
| playerX | float |
| playerY | float |
| playerLayer | BlockLayer |


### 構造体: Workspace

| 名前 | 型 |
|:-------------------|:--------|
| workspaceID | Guid |
| wsOwnerID | Guid |
| int | x1 |
| int | y1 |
| int | x2 |
| int | y2 |
| inEdit | bool |


----

## 変数



----

## メソッド



### メソッド: public int[][] GetChunk(BlockLayer layerID, int chunkID)

説明: 

現在接続しているサーバーへチャンクデータを要求する

戻り値: チャンク情報(int型2次元配列)



| 型 | 引数名 | 説明 |
|:--------|:----------------------|--------------------------------------|
| BlockLayer | layerID | 要求するチャンクが存在するレイヤーのID |
| int | chunkID | 要求するチャンク |


戻り値の型: int[][]

### メソッド: public bool SendChunk(BlockLayer layerID, int chunkID, int[][] chunkData)

説明: 

チャンクを現在接続しているサーバーに送信する

戻り値: 送信に成功したか



| 型 | 引数名 | 説明 |
|:--------|:----------------------|--------------------------------------|
| BlockLayer | layerID | 送信するチャンクが存在するレイヤーのID |
| int | chunkID | 送信するチャンクの場所 |
| int[][] | chunkData | 送信するチャンク情報 |


戻り値の型: bool

### メソッド: public void Connect(string ipAddress, int port)

説明: 

接続先のポート/IPアドレスを指定し、接続する。

ポートが範囲外の時は自動的に「11781」。

| 型 | 引数名 | 説明 |
|:--------|:----------------------|--------------------------------------|
| string | ipAddress | 接続先IPアドレス。 |
| int | port | 接続先ポート番号。デフォルトは11781。 |


戻り値の型: void

### メソッド: public PlayerData SetPlayerData(string name, int skinID)

説明: 

サーバーにプレイヤー情報を送信する。

戻り値: サーバーに登録されたPlayerData



| 型 | 引数名 | 説明 |
|:--------|:----------------------|--------------------------------------|
| string | name | 他人に表示される名前 |
| int | skinID | 他人から表示される見た目(いるか？これ) |


戻り値の型: PlayerData

### メソッド: public PlayerData[]? GetPlayers()

説明: 

接続中のプレイヤーをすべて取得する。SetPlayerData()の未実行等で登録がない場合はnull。

戻り値: 接続中のプレイヤーが含まれる PlayerData 配列



戻り値の型: PlayerData[]?

### メソッド: public PlayerData? GetPlayer(Guid PlayerID)

説明: 

プレイヤーの情報を取得する。指定したGuidのプレイヤーが存在しない場合はnull。

戻り値: なし



| 型 | 引数名 | 説明 |
|:--------|:----------------------|--------------------------------------|
| Guid | PlayerID | 情報を取得するPlayerID。 |


戻り値の型: PlayerData?

### メソッド: public void SendPlayerMove(BlockLayer layer, float x, float y)

説明: 

プレイヤーの移動情報を送信する。

| 型 | 引数名 | 説明 |
|:--------|:----------------------|--------------------------------------|
| BlockLayer | layer | プレイヤーが存在するレイヤー |
| float | x | プレイヤーのX座標 |
| float | y | プレイヤーのY座標 |


戻り値の型: void

### メソッド: public void SendBlockChange(BlockLayer layer, int x, int y, int blockID)

説明: 

ブロック単位の変更を送信する。

| 型 | 引数名 | 説明 |
|:--------|:----------------------|--------------------------------------|
| BlockLayer | layer | レイヤー番号 |
| int | x | ブロックのX座標 |
| int | y | ブロックのY座標 |
| int | blockID | 変更された後のブロックID |


戻り値の型: void

### メソッド: public void SendWSInfo(WorkSpace workspace)

説明: 

ワークスペースの情報を送信する。存在するワークスペースの場合は上書きされる。

| 型 | 引数名 | 説明 |
|:--------|:----------------------|--------------------------------------|
| WorkSpace | workspace | 送信するWorkspace 構造体配列 |


戻り値の型: void

### メソッド: public Workspace[]? GetWorkspaces()

説明: 

ワールドに存在するすべてのワークスペースを取得する。

戻り値: 存在するWorkspace 構造体配列(ワークスペースが存在しない場合はnull)



戻り値の型: Workspace[]?

### メソッド: public Workspace[]? GetWorkspacesOfPlayer(Guid wsOwnerGuid)

説明: 

指定したプレイヤーが所有するワークスペースを取得する。

戻り値: 検索結果としてのWorkspace 構造体配列(ワークスペースが存在しない、プレイヤーが存在しない場合はnull



| 型 | 引数名 | 説明 |
|:--------|:----------------------|--------------------------------------|
| Guid | wsOwnerGuid | 検索するプレイヤーのGuid |


戻り値の型: Workspace[]?

### メソッド: public void SendWSRemove(Guid removeWorkspaceGuid)

説明: 

ワークスペースを削除する

| 型 | 引数名 | 説明 |
|:--------|:----------------------|--------------------------------------|
| Guid | removeWorkspaceGuid | 削除するワークスペースのGuid |


戻り値の型: void

----

