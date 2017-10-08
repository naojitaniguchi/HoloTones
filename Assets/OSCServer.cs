using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uOSC;

public class OSCServer : MonoBehaviour {
    public GameObject topObject;
    public GameObject padObject;
    public GameObject keyLeftObject;
    public GameObject keyRightObject;
    public GameObject effectObject;
    public GameObject spikeObject;
    public GameObject yellowBoxObject;
    public GameObject redBoxObject;
    public GameObject textObject;
    public GameObject textLeftObject;
    public GameObject textRightObject;
    public GameObject raibowCubeObject;
    public float keyMoveSpeed = 1.0f ;
    public float keyRotSpeed = 5.0f;
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

    float leftKeyWidth;
    float leftKeyDepth;
    float leftBoxStepX;
    float leftBoxStepZ;
    float leftBoxOriginalHeight;
    public int leftBoxXNum = 16;
    public int leftBoxZNum = 16;

    float rightKeyWidth;
    float rightKeyDepth;
    float rightBoxStepX;
    float rightBoxStepZ;
    float rightBoxOriginalHeight;
    public int rightBoxXNum = 16;
    public int rightBoxZNum = 16;

    public float boxDownSpeed = 1.0f;
    public float boxHightMin = 0.0001f;


    GameObject[] spikeArray;
    GameObject[] yellowBoxArray;
    GameObject[] redBoxArray;

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
                obj.transform.SetParent(topObject.transform);
                count++;
            }
        }
    }

    void createYellowBoxes()
    {
        leftKeyWidth = keyLeftObject.transform.localScale.x;
        leftKeyDepth = keyLeftObject.transform.localScale.z;
        leftBoxStepX = leftKeyWidth / (float)(leftBoxXNum - 1);
        leftBoxStepZ = leftKeyDepth / (float)(leftBoxZNum - 1);
        leftBoxOriginalHeight = yellowBoxObject.transform.localScale.y;

        yellowBoxArray = new GameObject[leftBoxXNum * leftBoxZNum];

        int count = 0;
        for (int i = 0; i < leftBoxXNum; i++)
        {
            float xPos = leftKeyWidth * -0.5f + (float)i * leftBoxStepX - leftBoxStepX * 0.5f ;
            for (int j = 0; j < leftBoxZNum; j++)
            {
                float zPos = leftKeyDepth * -0.5f + (float)j * leftBoxStepZ + leftBoxStepZ * 0.5f;
                Vector3 pos = new Vector3(keyLeftObject.transform.position.x + xPos, keyLeftObject.transform.position.y, keyLeftObject.transform.position.z + zPos);
                GameObject obj = Instantiate(yellowBoxObject, pos, Quaternion.identity);
                obj.transform.localScale = new Vector3(leftBoxStepX, boxHightMin, leftBoxStepZ);
                yellowBoxArray[count] = obj;
                obj.transform.SetParent(topObject.transform);
                obj.SetActive(false);
                count++;
            }
        }
    }

    void createRedBoxes()
    {
        rightKeyWidth = keyRightObject.transform.localScale.x;
        rightKeyDepth = keyRightObject.transform.localScale.z;
        rightBoxStepX = rightKeyWidth / (float)(rightBoxXNum - 1);
        rightBoxStepZ = rightKeyDepth / (float)(rightBoxZNum - 1);
        rightBoxOriginalHeight = redBoxObject.transform.localScale.y;

        redBoxArray = new GameObject[rightBoxXNum * rightBoxZNum];

        int count = 0;
        for (int i = 0; i < rightBoxXNum; i++)
        {
            float xPos = rightKeyWidth * -0.5f + (float)i * rightBoxStepX - rightBoxStepX * 0.5f;
            for (int j = 0; j < rightBoxZNum; j++)
            {
                float zPos = rightKeyDepth * -0.5f + (float)j * rightBoxStepZ + rightBoxStepZ * 0.5f;
                Vector3 pos = new Vector3(keyRightObject.transform.position.x + xPos, keyRightObject.transform.position.y, keyRightObject.transform.position.z + zPos);
                GameObject obj = Instantiate(redBoxObject, pos, Quaternion.identity);
                obj.transform.localScale = new Vector3(rightBoxStepX, boxHightMin, rightBoxStepZ);
                redBoxArray[count] = obj;
                obj.transform.SetParent(topObject.transform);
                obj.SetActive(false);
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
        createYellowBoxes();
        createRedBoxes();
    }

    void moveSpike( float x, float z)
    {
        float zReverse = 1.0f - z;
        int xIndex = (int)(x * spikeXNum);
        int zIndex = (int)(zReverse * spikeZNum);

        int targetSpike = xIndex * spikeXNum + zIndex;
        spikeArray[targetSpike].transform.localScale = new Vector3(spikeOriginalScale.x, spikeOriginalScale.y, spikeOriginalScale.z);

        float yPos = spikeOriginalScale.y * 0.5f + padObject.transform.position.y;
        spikeArray[targetSpike].transform.position = new Vector3(spikeArray[targetSpike].transform.position.x, yPos, spikeArray[targetSpike].transform.position.z);
    }

    void moveYellowBox(float x, float z)
    {
        int xIndex = (int)(x * leftBoxXNum);
        int zIndex = (int)(z * leftBoxZNum);

        int targetBox = xIndex * leftBoxXNum + zIndex;
        yellowBoxArray[targetBox].transform.localScale = new Vector3(leftBoxStepX, leftBoxOriginalHeight, leftBoxStepX);

        float yPos = leftBoxOriginalHeight * 0.5f + keyLeftObject.transform.position.y;
        yellowBoxArray[targetBox].transform.position = new Vector3(yellowBoxArray[targetBox].transform.position.x, yPos, yellowBoxArray[targetBox].transform.position.z);
    }

    void moveRedBox(float x, float z)
    {
        int xIndex = (int)(x * rightBoxXNum);
        int zIndex = (int)(z * rightBoxZNum);

        int targetBox = xIndex * rightBoxXNum + zIndex;
        redBoxArray[targetBox].transform.localScale = new Vector3(rightBoxStepX, rightBoxOriginalHeight, rightBoxStepX);

        float yPos = rightBoxOriginalHeight * 0.5f + keyRightObject.transform.position.y;
        redBoxArray[targetBox].transform.position = new Vector3(redBoxArray[targetBox].transform.position.x, yPos, redBoxArray[targetBox].transform.position.z);
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

    void updateYellowBoxes()
    {
        for (int i = 0; i < leftBoxXNum; i++)
        {
            for (int j = 0; j < leftBoxXNum; j++)
            {
                int targetBox = i * leftBoxXNum + j;
                float boxHeight = yellowBoxArray[targetBox].transform.localScale.y;
                boxHeight -= boxDownSpeed * Time.deltaTime;
                if (boxHeight < boxHightMin)
                {
                    boxHeight = boxHightMin;
                }
                yellowBoxArray[targetBox].transform.localScale = new Vector3(leftBoxStepX, boxHeight, leftBoxStepZ);
                float boxY = boxHeight * 0.5f + keyLeftObject.transform.position.y;
                yellowBoxArray[targetBox].transform.position = new Vector3(yellowBoxArray[targetBox].transform.position.x, boxY, yellowBoxArray[targetBox].transform.position.z);
            }
        }
    }

    void updateRedBoxes()
    {
        for (int i = 0; i < rightBoxXNum; i++)
        {
            for (int j = 0; j < rightBoxXNum; j++)
            {
                int targetBox = i * rightBoxXNum + j;
                float boxHeight = redBoxArray[targetBox].transform.localScale.y;
                boxHeight -= boxDownSpeed * Time.deltaTime;
                if (boxHeight < boxHightMin)
                {
                    boxHeight = boxHightMin;
                }
                redBoxArray[targetBox].transform.localScale = new Vector3(rightBoxStepX, boxHeight, rightBoxStepZ);
                float boxY = boxHeight * 0.5f + keyRightObject.transform.position.y;
                redBoxArray[targetBox].transform.position = new Vector3(redBoxArray[targetBox].transform.position.x, boxY, redBoxArray[targetBox].transform.position.z);
            }
        }
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Vector3 moveVec = topObject.transform.forward * keyMoveSpeed * Time.deltaTime;
            topObject.transform.position = new Vector3(topObject.transform.position.x + moveVec.x, topObject.transform.position.y, topObject.transform.position.z + moveVec.z);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 moveVec = topObject.transform.forward * keyMoveSpeed * Time.deltaTime * -1.0f ;
            topObject.transform.position = new Vector3(topObject.transform.position.x + moveVec.x, topObject.transform.position.y, topObject.transform.position.z + moveVec.z);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 moveVec = topObject.transform.right * keyMoveSpeed * Time.deltaTime * -1.0f;
            topObject.transform.position = new Vector3(topObject.transform.position.x + moveVec.x, topObject.transform.position.y, topObject.transform.position.z + moveVec.z);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 moveVec = topObject.transform.right * keyMoveSpeed * Time.deltaTime;
            topObject.transform.position = new Vector3(topObject.transform.position.x + moveVec.x, topObject.transform.position.y, topObject.transform.position.z + moveVec.z);
        }
        if (Input.GetKey(KeyCode.A))
        {
            topObject.transform.position = new Vector3(topObject.transform.position.x , topObject.transform.position.y + keyMoveSpeed * Time.deltaTime, topObject.transform.position.z);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            topObject.transform.position = new Vector3(topObject.transform.position.x, topObject.transform.position.y - keyMoveSpeed * Time.deltaTime, topObject.transform.position.z);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            topObject.transform.Rotate(0.0f, keyRotSpeed * Time.deltaTime, 0.0f);
        }
        if (Input.GetKey(KeyCode.W))
        {
            topObject.transform.Rotate(0.0f, -1.0f * keyRotSpeed * Time.deltaTime, 0.0f);
        }
        if (Input.GetKey(KeyCode.P))
        {
            Instantiate(raibowCubeObject, new Vector3(topObject.transform.position.x + Random.insideUnitCircle.x * 0.5f,
                topObject.transform.position.y + 1.0f, topObject.transform.position.z + Random.insideUnitCircle.y * 0.5f),Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (keyRightObject.activeSelf == false)
            {
                keyRightObject.SetActive(true);
                for (int i = 0; i < rightBoxXNum * rightBoxZNum; i++)
                {
                    redBoxArray[i].SetActive(true);
                }
            }
            else
            {
                keyRightObject.SetActive(false);
                for (int i = 0; i < rightBoxXNum * rightBoxZNum; i++)
                {
                    redBoxArray[i].SetActive(false);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (keyLeftObject.activeSelf == false)
            {
                keyLeftObject.SetActive(true);
                for (int i = 0; i < leftBoxXNum * leftBoxZNum; i++)
                {
                    yellowBoxArray[i].SetActive(true);
                }
            }else
            {
                keyLeftObject.SetActive(false);
                for (int i = 0; i < leftBoxXNum * leftBoxZNum; i++)
                {
                    yellowBoxArray[i].SetActive(false);
                }
            }
        }
        if (Input.GetMouseButton(0))
        {
            float x = Input.mousePosition.x / (float)Screen.width ;
            float y = Input.mousePosition.y / (float)Screen.height;

            // addCenterPad(x, y);
            moveSpike(x, y);
            moveYellowBox(x, y);
            moveRedBox(x, y);

        }
        updateSpikes();
        updateYellowBoxes();
        updateRedBoxes();

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
        // msg += "(" + message.timestamp.ToLocalTime() + ") ";

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
            moveSpike(values[0], values[1]);
            textObject.GetComponent<TextMesh>().text = msg;
        }

        if (message.address == "/key1")
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
                else
                {
                    values[count - 1] = (float)value;
                }
                count++;
            }
            if ( index >= 48)
            {
                moveYellowBox(values[0], values[1]);
                textLeftObject.GetComponent<TextMesh>().text = msg;
            }
            else
            {
                moveRedBox(values[0], values[1]);
                textRightObject.GetComponent<TextMesh>().text = msg;
            }
        }

        if (message.address == "/key2")
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
                else
                {
                    values[count - 1] = (float)value;
                }
                count++;
            }
            moveYellowBox(values[0], values[1]);
            textLeftObject.GetComponent<TextMesh>().text = msg;
        }
        // Debug.Log(msg);
        
    }

}
