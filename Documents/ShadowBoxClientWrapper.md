# �N���X ShadowBoxClientWrapper

�p��: MonoBehaviour

## �񋓌^



### ��: BlockLayer

| ���O | �l |
|:-------------------|------:|
| InsideWall | 1 |
| InsideBlock | 2 |
| OutsideWall | 3 |
| OutsideBlock | 4 |


----

## �\����



### �\����: PlayerData

| ���O | �^ |
|:-------------------|:--------|
| name | string |
| skinType | int |
| playerID | Guid |
| playerX | float |
| playerY | float |
| playerLayer | BlockLayer |


### �\����: Workspace

| ���O | �^ |
|:-------------------|:--------|
| workspaceID | Guid |
| wsOwnerID | Guid |
| int | x1 |
| int | y1 |
| int | x2 |
| int | y2 |
| inEdit | bool |


----

## �ϐ�



----

## ���\�b�h



### ���\�b�h: public int[][] GetChunk(BlockLayer layerID, int chunkID)

����: 

���ݐڑ����Ă���T�[�o�[�փ`�����N�f�[�^��v������

�߂�l: �`�����N���(int�^2�����z��)



| �^ | ������ | ���� |
|:--------|:----------------------|--------------------------------------|
| BlockLayer | layerID | �v������`�����N�����݂��郌�C���[��ID |
| int | chunkID | �v������`�����N |


�߂�l�̌^: int[][]

### ���\�b�h: public bool SendChunk(BlockLayer layerID, int chunkID, int[][] chunkData)

����: 

�`�����N�����ݐڑ����Ă���T�[�o�[�ɑ��M����

�߂�l: ���M�ɐ���������



| �^ | ������ | ���� |
|:--------|:----------------------|--------------------------------------|
| BlockLayer | layerID | ���M����`�����N�����݂��郌�C���[��ID |
| int | chunkID | ���M����`�����N�̏ꏊ |
| int[][] | chunkData | ���M����`�����N��� |


�߂�l�̌^: bool

### ���\�b�h: public void Connect(string ipAddress, int port)

����: 

�ڑ���̃|�[�g/IP�A�h���X���w�肵�A�ڑ�����B

�|�[�g���͈͊O�̎��͎����I�Ɂu11781�v�B

| �^ | ������ | ���� |
|:--------|:----------------------|--------------------------------------|
| string | ipAddress | �ڑ���IP�A�h���X�B |
| int | port | �ڑ���|�[�g�ԍ��B�f�t�H���g��11781�B |


�߂�l�̌^: void

### ���\�b�h: public PlayerData SetPlayerData(string name, int skinID)

����: 

�T�[�o�[�Ƀv���C���[���𑗐M����B

�߂�l: �T�[�o�[�ɓo�^���ꂽPlayerData



| �^ | ������ | ���� |
|:--------|:----------------------|--------------------------------------|
| string | name | ���l�ɕ\������閼�O |
| int | skinID | ���l����\������錩����(���邩�H����) |


�߂�l�̌^: PlayerData

### ���\�b�h: public PlayerData[]? GetPlayers()

����: 

�ڑ����̃v���C���[�����ׂĎ擾����BSetPlayerData()�̖����s���œo�^���Ȃ��ꍇ��null�B

�߂�l: �ڑ����̃v���C���[���܂܂�� PlayerData �z��



�߂�l�̌^: PlayerData[]?

### ���\�b�h: public PlayerData? GetPlayer(Guid PlayerID)

����: 

�v���C���[�̏����擾����B�w�肵��Guid�̃v���C���[�����݂��Ȃ��ꍇ��null�B

�߂�l: �Ȃ�



| �^ | ������ | ���� |
|:--------|:----------------------|--------------------------------------|
| Guid | PlayerID | �����擾����PlayerID�B |


�߂�l�̌^: PlayerData?

### ���\�b�h: public void SendPlayerMove(BlockLayer layer, float x, float y)

����: 

�v���C���[�̈ړ����𑗐M����B

| �^ | ������ | ���� |
|:--------|:----------------------|--------------------------------------|
| BlockLayer | layer | �v���C���[�����݂��郌�C���[ |
| float | x | �v���C���[��X���W |
| float | y | �v���C���[��Y���W |


�߂�l�̌^: void

### ���\�b�h: public void SendBlockChange(BlockLayer layer, int x, int y, int blockID)

����: 

�u���b�N�P�ʂ̕ύX�𑗐M����B

| �^ | ������ | ���� |
|:--------|:----------------------|--------------------------------------|
| BlockLayer | layer | ���C���[�ԍ� |
| int | x | �u���b�N��X���W |
| int | y | �u���b�N��Y���W |
| int | blockID | �ύX���ꂽ��̃u���b�NID |


�߂�l�̌^: void

### ���\�b�h: public void SendWSInfo(WorkSpace workspace)

����: 

���[�N�X�y�[�X�̏��𑗐M����B���݂��郏�[�N�X�y�[�X�̏ꍇ�͏㏑�������B

| �^ | ������ | ���� |
|:--------|:----------------------|--------------------------------------|
| WorkSpace | workspace | ���M����Workspace �\���̔z�� |


�߂�l�̌^: void

### ���\�b�h: public Workspace[]? GetWorkspaces()

����: 

���[���h�ɑ��݂��邷�ׂẴ��[�N�X�y�[�X���擾����B

�߂�l: ���݂���Workspace �\���̔z��(���[�N�X�y�[�X�����݂��Ȃ��ꍇ��null)



�߂�l�̌^: Workspace[]?

### ���\�b�h: public Workspace[]? GetWorkspacesOfPlayer(Guid wsOwnerGuid)

����: 

�w�肵���v���C���[�����L���郏�[�N�X�y�[�X���擾����B

�߂�l: �������ʂƂ��Ă�Workspace �\���̔z��(���[�N�X�y�[�X�����݂��Ȃ��A�v���C���[�����݂��Ȃ��ꍇ��null



| �^ | ������ | ���� |
|:--------|:----------------------|--------------------------------------|
| Guid | wsOwnerGuid | ��������v���C���[��Guid |


�߂�l�̌^: Workspace[]?

### ���\�b�h: public void SendWSRemove(Guid removeWorkspaceGuid)

����: 

���[�N�X�y�[�X���폜����

| �^ | ������ | ���� |
|:--------|:----------------------|--------------------------------------|
| Guid | removeWorkspaceGuid | �폜���郏�[�N�X�y�[�X��Guid |


�߂�l�̌^: void

----

