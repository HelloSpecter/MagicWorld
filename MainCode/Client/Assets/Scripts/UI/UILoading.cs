using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoading : MonoBehaviour {

    public Text m_NumText;
    public Slider m_Slider;
    float loadProgress ;

    // Use this for initialization
     void OnEnable()
    {
        m_Slider.value = 0;
        loadProgress = 0;
        StartCoroutine(Start());
        
    }
    IEnumerator Start()
    {
        while (m_Slider.value < 100)
        {//加载资源的进度，这里先模拟，后期根据实际更改
            loadProgress = Mathf.Lerp(loadProgress, 101f, 1.5f * Time.deltaTime);
            m_Slider.value = loadProgress;
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
        yield return null;
    }
    // void Start()
    //{
    //    MessageBox.Show("111");
    //}

    // Update is called once per frame
    void Update () {
        m_NumText.text = Mathf.Round( m_Slider.value).ToString();
    }
}
