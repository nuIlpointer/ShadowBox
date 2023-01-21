using System.Collections;
using System.Collections.Generic;
public class ChunkData {
    public List<int[]> chunkData;
    public ShadowBoxServer.BlockLayer blockLayer;
    public int chunkId;

    public ChunkData(List<int[]> chunkData, ShadowBoxServer.BlockLayer blockLayer, int chunkId) {
        this.chunkData = chunkData;
        this.blockLayer = blockLayer;
        this.chunkId = chunkId;
    }

    public void SetChunkData(List<int[]> chunkData) {
        this.chunkData = chunkData;
    }
    
    public List<int[]> GetChunkData() {
        return chunkData;
    }

    public ShadowBoxServer.BlockLayer GetBlockLayer() {
        return blockLayer;
    }

    public int GetChunkID() {
        return chunkId;
    }
}