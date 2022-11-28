using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 地形を生成する
    /// </summary>
    /// <param name="width">生成するチャンク数</param>
    /// <param name="chunkWidth">1チャンクあたりの横幅</param>
    /// <param name="chunkHeight">1チャンクあたりの高さ</param>
    /// <param name="heightRange">高度差</param>
    /// <returns></returns>
    public int[][][] Generate(int width, int chunkWidth, int chunkHeight, int heightRange) {
        int heightBase = new System.Random().Next(heightRange, chunkHeight - heightRange);
        List<int[][]> chunks = new List<int[][]>();
        for (int i = 0; i < width; i++) {
            List<int[]> row = new List<int[]>();
            
            for (int j = 0; j < chunkWidth; j++) {
                int x = i * chunkWidth + j;
                List<int> column = new List<int>();
                int noise = (int)Mathf.Floor(Mathf.PerlinNoise(x * 0.1f, 0f) * heightRange) + heightBase;
                for (int k = 0; k < chunkHeight; k++)
                    column.Add(k<noise?0:k==noise?10:12);
                row.Add(column.ToArray());
            }
            chunks.Add(rotate(row.ToArray()));
        }
        return chunks.ToArray();
    }

    private int[][] rotate(int[][] array) {
        List<int[]> resultArray = new List<int[]>();
        int i = 0;
        for (int j = 0; j < array[0].Length; j++)
            resultArray.Add(new int[array.Length]);
        foreach (int[] row in array) {
            for (int k = 0, l = 0; k < row.Length && l < array.Length; k++, l++) {
                resultArray[l][i] = row[k];
            }
            i++;
        }
        return resultArray.ToArray();
    }
}
