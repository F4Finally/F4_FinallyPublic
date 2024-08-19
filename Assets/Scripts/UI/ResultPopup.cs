using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultData
{
    public int ItemKey;
    public int ItemResult;

    public ResultData(int key, int num)
    {
        ItemKey = key;
        ItemResult = num;
    }  
}

public class ResultPopup : MonoBehaviour
{
    [SerializeField] GameObject G_ResultItem;
    [SerializeField] Sprite[] itemICons;
    [SerializeField] Transform ResultParant;
    List<GameObject> G_Resilts = new List<GameObject>();

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void ResultSet(List<ResultData> data)
    {
        for(int i = 0; i < data.Count; i++)
        {
            GameObject resultItem = Instantiate(G_ResultItem, ResultParant);
            resultItem.GetComponent<ResultItemPrefab>().UiSet(itemICons[data[i].ItemKey - 1], data[i].ItemResult);
            G_Resilts.Add(resultItem);
        }
    }

    private void OnDisable()
    {
        for(int i = 0; i < G_Resilts.Count; i++)
        {
            Destroy(G_Resilts[i]);
        }

        G_Resilts.Clear();
    }
}
