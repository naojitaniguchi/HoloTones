using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uOSC;

public class OSCServer : MonoBehaviour {
    public GameObject padObject;
    public GameObject keyLeftObject;
    public GameObject effectObject;
    public GameObject spikeObject;
    public GameObject boxObject;
    float[] values;
    int index;
    public float forcce;
    float padWidth;
    float padDepth;
    float spikeStepX;
    float spikeStepZ;
    public int spikeXNum = 16;
    public int spikeZNum = 16;
    Vector3 spikeOriginalScale;
    public float spikeDownSpeed = 1.0f;
    public float spikeHightMin = 0.0001f;

    float keyWidth;
    float keyDepth;
    float boxStepX;
    float boxStepZ;
    float boxOriginalHeight;
    public int boxXNum = 16;
    public int boxZNum = 16;
    public float boxDownSpeed = 1.0f;
    public float boxHightMin = 0.0001f;


    GameObject[] spikeArray;
    GameObject[] boxArray;

    void createSpikes(){
        spikeOriginalScale = spikeObject.transform.localScale;
        spikeArray = new GameObject[spikeXNum * spikeZNum];

        padWidth = padObject.transform.localScale.x ;
        padDepth = padObject.transform.localScale.z ;

        spikeStepX = padWidth / (float)(spikeXNum-1);
        spikeStepZ = padDepth / (float)(spikeZNum-1);

        int count = 0;
        for (int i = 0; i < spikeXNum; i++)
        {
            float xPos = padWidth * -0.5f + (float)i * spikeStepX;
            for (int j = 0; j < spikeZNum; j++)
            {
                float zPos = padDepth * -0.5f + (float)j * spikeStepZ;
                Vector3 pos = new Vector3(padObject.transform.position.x + xPos, padObject.transform.position.y, padObject.transform.position.z + zPos);
                GameObject obj = Instantiate(spikeObject, pos, Quaternion.identity);
                obj.transform.localScale = new Vector3(obj.transform.localScale.x, spikeHightMin, obj.transform.localScale.z);
                spikeArray[count] = obj;
                count++;
            }
        }
    }

    void createBoxes()
    {
        keyWidth = keyLeftObject.transform.localScale.x;
        keyDepth = keyLeftObject.transform.localScale.z;
        boxStepX = keyWidth / (float)(boxXNum - 1);
        boxStepZ = keyDepth / (float)(boxZNum - 1);
        boxOriginalHeight = boxObject.transform.localScale.y;

        boxArray = new GameObject[boxXNum * boxZNum];

        int count = 0;
        for (int i = 0; i < boxXNum; i++)
        {
            float xPos = keyWidth * -0.5f + (float)i * boxStepX - boxStepX * 0.5f ;
            for (int j = 0; j < boxZNum; j++)
            {
                float zPos = keyDepth * -0.5f + (float)j * boxStepZ + boxStepZ * 0.5f;
                Vector3 pos = new Vector3(keyLeftObject.transform.position.x + xPos, keyLeftObject.transform.position.y, keyLeftObject.transform.position.z + zPos);
                GameObject obj = Instantiate(boxObject, pos, Quaternion.identity);
                obj.transform.localScale = new Vector3(boxStepX, boxHightMin, boxStepZ);
                boxArray[count] = obj;
                count++;
            }
        }
    }

    // Use this for initialization
    void Start () {
        var server = GetComponent<uOSC.uOscServer>();
        server.onDataReceived.AddListener(OnDataReceived);

        values = new float[3];

        createSpikes();
        createBoxes();
    }

    void moveSpike( float x, float z)
    {
        int xIndex = (int)(x * spikeXNum);
        int zIndex = (int)(z * spikeZNum);

        int targetSpike = xIndex * spikeXNum + zIndex;
        spikeArray[targetSpike].transform.localScale = new Vector3(spikeOriginalScale.x, spikeOriginalScale.y, spikeOriginalScale.z);

        float yPos = spikeOriginalScale.y * 0.5f + padObject.transform.position.y;
        spikeArray[targetSpike].transform.position = new Vector3(spikeArray[targetSpike].transform.position.x, yPos, spikeArray[targetSpike].transform.position.z);
    }

    void moveBox(float x, float z)
    {
        int xIndex = (int)(x * boxXNum);
        int zIndex = (int)(z * boxZNum);

        int targetBox = xIndex * boxXNum + zIndex;
        boxArray[targetBox].transform.localScale = new Vector3(boxStepX, boxOriginalHeight, boxStepX);

        float yPos = boxOriginalHeight * 0.5f + keyLeftObject.transform.position.y;
        boxArray[targetBox].transform.position = new Vector3(boxArray[targetBox].transform.position.x, yPos, boxArray[targetBox].transform.position.z);
    }

    void updateSpikes()
    {
        for ( int i = 0; i < spikeXNum; i ++)
        {
            for (int j = 0; j < spikeXNum; j++)
            {
                int targetSpike = i * spikeXNum + j;
                float spikeHeight = spikeArray[targetSpike].transform.localScale.y;
                spikeHeight -= spikeDownSpeed * Time.deltaTime;
                if (spikeHeight < spikeHightMin)
                {
                    spikeHeight = spikeHightMin;
                }
                spikeArray[targetSpike].transform.localScale = new Vector3(spikeOriginalScale.x, spikeHeight, spikeOriginalScale.z);
                float spikeY = spikeHeight * 0.5f + padObject.transform.position.y;
                spikeArray[targetSpike].transform.position = new Vector3(spikeArray[targetSpike].transform.position.x, spikeY, spikeArray[targetSpike].transform.position.z);
            }
        }
    }

    void updateBoxes()
    {
        for (int i = 0; i < boxXNum; i++)
        {
            for (int j = 0; j < boxXNum; j++)
            {
                int targetBox = i * boxXNum + j;
                float boxHeight = boxArray[targetBox].transform.localScale.y;
                boxHeight -= boxDownSpeed * Time.deltaTime;
                if (boxHeight < boxHightMin)
                {
                    boxHeight = boxHightMin;
                }
                boxArray[targetBox].transform.localScale = new Vector3(boxStepX, boxHeight, boxStepZ);
                float boxY = boxHeight * 0.5f + keyLeftObject.transform.position.y;
                boxArray[targetBox].transform.position = new Vector3(boxArray[targetBox].transform.position.x, boxY, boxArray[targetBox].transform.position.z);
            }
        }
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButton(0))
        {
            float x = Input.mousePosition.x / (float)Screen.width ;
            float y = Input.mousePosition.y / (float)Screen.height;

            // addCenterPad(x, y);
            moveSpike(x, y);
            moveBox(x, y);
            
        }
        updateSpikes();
        updateBoxes();

    }

    void addCenterPad(float x, float y)
    {
        Vector3 randVec = Random.onUnitSphere;
        randVec.y = 1.0f;
        Vector3 pos = new Vector3(gameObject.transform.position.x + (x - 0.5f) * 2.0f * 0.01f, gameObject.transform.position.y , gameObject.transform.position.z + (y - 0.5f) * 2.0f * 0.01f);
        GameObject obj = Instantiate(effectObject, pos, Quaternion.identity);
        obj.GetComponent<Rigidbody>().AddForce(randVec * forcce);
    }

    void OnDataReceived(uOSC.Message message)
    {
        if (values == null)
        {
            return;
        }
        // address
        var msg = message.address + ": ";

        // timestamp
        msg += "(" + message.timestamp.ToLocalTime() + ") ";

        if (message.address == "/blocks")
        {
            // values
            int count = 0;
            foreach (var value in message.values)
            {
                msg += value.GetString() + " ";
                if (count == 0)
                {
                    index = (int)value;
                }
                else {
                    values[count-1] = (float)value;
                }
                count++;
            }
            addCenterPad(values[0], values[1]);
            //Instantiate(effectObjct, gameObject.transform.position, Quaternion.identity);
        }
        Debug.Log(msg);
    }

}
