using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class LoadServerList : MonoBehaviour
{
    private int vIndex = 0;
    [SerializeField] private GameObject serverItemPrefab;
    // Start is called before the first frame update
    void Start()
    {
        if(File.Exists("./server.dat")) {
            ///起きてからの俺へ serverItemPrefabに良い感じの設定をしてなんとかしてください
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
