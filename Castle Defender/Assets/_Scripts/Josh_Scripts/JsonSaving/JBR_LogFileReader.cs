using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JBR_LogFileReader : MonoBehaviour {
    public Text logInfo;
    public TMP_Text tmp_LogInfo;
    public string output = "";
    public string stack = "";

    public List<string> lines = new List<string>();

    private void Start()
    {
     //   logInfo = GameObject.Find("LogInfo").GetComponent<Text>(); 
    }

    void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        output = logString;
        stack = stackTrace;

        lines.Insert(0,logString);

        if (logInfo != null)
        {
            logInfo.text = output;
            CancelInvoke();
            Invoke("ClearText", 10.0f);
        }
        else
        {
            if(tmp_LogInfo != null)
            {
                tmp_LogInfo.text = lines[0] +"\n" + lines[1] + "\n" + lines[2] + "\n" + lines[3];
                CancelInvoke();
                Invoke("ClearText", 10.0f);
            }
        }
    }


    public void ClearText()
    {
        logInfo.text = "";
        tmp_LogInfo.text = "";
    }
}
