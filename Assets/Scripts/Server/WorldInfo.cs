using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static InitialProcess;
using static UnityEditor.Experimental.GraphView.GraphView;

public class WorldInfo {
    int worldSizeX;
    int worldSizeY;
    int chunkSizeX;
    int chunkSizeY;
    int heightRange;
    int seed;
    string worldName;
    public WorldInfo(int worldSizeX, int worldSizeY, int chunkSizeX, int chunkSizeY, int heightRange, int seed, string worldName) {
        this.worldSizeX = worldSizeX;
        this.worldSizeY = worldSizeY;
        this.chunkSizeX = chunkSizeX;
        this.chunkSizeY = chunkSizeY;
        this.heightRange = heightRange;
        this.seed = seed;
        this.worldName = worldName;
    }

    /// <summary>
    /// ワールドの横チャンク数を取得
    /// </summary>
    /// <returns>ワールドの横サイズ</returns>
    public int GetWorldSizeX() {
        return worldSizeX;
    }

    /// <summary>
    /// ワールドの縦チャンク数を取得
    /// </summary>
    /// <returns>ワールドの縦サイズ</returns>
    public int GetWorldSizeY() {
        return worldSizeY;
    }

    /// <summary>
    /// チャンクの横ブロック数を取得
    /// </summary>
    /// <returns>ワールドの横ブロック数</returns>
    public int GetChunkSizeX() {
        return chunkSizeX;
    }

    /// <summary>
    /// ワールドの縦ブロック数を取得
    /// </summary>
    /// <returns>ワールドの縦ブロック数</returns>
    public int GetChunkSizeY() {
        return chunkSizeY;
    }


    /// <summary>
    /// ワールドの高低差を取得する
    /// </summary>
    /// <returns>ワールドの高低差</returns>
    public int GetHeightRange() {
        return heightRange;
    }

    /// <summary>
    /// このワールドのシード値を取得
    /// </summary>
    /// <returns>ワールドのシード値</returns>
    public int GetSeed() {
        return seed;
    }

    /// <summary>
    /// ワールドの名称を取得
    /// </summary>
    /// <returns>ワールドの名称</returns>
    public string GetWorldName() {
        return worldName;
    }

    /// <summary>
    /// 現在の内容でworldinfo.datを上書きする
    /// </summary>
    public void SaveWorldData() {
        if (!Directory.Exists("./worlddata/")) {
            Directory.CreateDirectory("./worlddata");
        }
        string fileName = $"./worlddata/worldinfo.dat";
        using (var writer = new StreamWriter(fileName, false, Encoding.UTF8)) {
            writer.WriteLine($"{worldSizeX},{worldSizeY},{chunkSizeX},{chunkSizeY},{heightRange},{seed},{worldName}");
        }
    }

#nullable enable
    public static WorldInfo? LoadWorldData() {
        string fileName = $"./worlddata/worldinfo.dat";
        if (File.Exists(fileName)) {
            List<int[]> tempList = new List<int[]>();
            using (var reader = new StreamReader(fileName, Encoding.UTF8)) {
                var line = reader.ReadLine();
                var lineArr = line.Split(',');
                return new WorldInfo(Int32.Parse(lineArr[0]), Int32.Parse(lineArr[1]), Int32.Parse(lineArr[2]), Int32.Parse(lineArr[3]), Int32.Parse(lineArr[4]), Int32.Parse(lineArr[5]), lineArr[6]);
            }
        } else return null;
    }
}