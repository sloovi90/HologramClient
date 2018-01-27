using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InputController : MonoBehaviour {
    bool AutoPresentation;
    Vector3 axisRotation;
    int slidingSpeed;
    Color chosenButtonColor;
    public bool doneSetup = false;
    enum LightDir { NONE,BACK,FRONT,LEFT,RIGHT};
    public  enum Axis { X=1,Y,Z};
    public  MyClient client;
    public  enum RotDir {Left,Right,Up,Down,CW,CCW };
    LightDir lightDir = LightDir.NONE;
    public GameObject imageButtonPrefab;
    
    // Use this for initialization
    public void TogglePresentationMode()
    {
        AutoPresentation = GameObject.Find("PresentModeSwitch").GetComponent<Switch>().isOn;
        //send presentation mode change
        client.SendMsg("P");

    }
    public void ChangeSlidingSpeed(bool increase)
    {
        if (slidingSpeed == 0 && !increase)
            return;
        slidingSpeed += (increase ? 1 : -1);
        GameObject.Find("SlidingSpeed/SpeedInput").GetComponentInChildren<Text>().text = slidingSpeed.ToString();
        //send sliding speed
        client.SendMsg("SlidingSpeed|"+slidingSpeed);
    }
    public void IncreaseRotAxis(int axis)
    {
        ChangeRotAxis(axis, true);
    }
    public void DecreaseRotAxis(int axis)
    {
        ChangeRotAxis(axis, false);
    }
    void ChangeRotAxis(int axis,bool increase)
    {
        
        switch ((Axis)axis)    
        {
            case Axis.X:
                if (axisRotation.x == 0 && !increase)
                    return;
                axisRotation.x += (increase ? 1 : -1);
                GameObject.Find("xAxis/SpeedInput").GetComponentInChildren<Text>().text = axisRotation.x.ToString();
                //send x increase/decrease
                client.SendMsg("AxisRotation|X|" + axisRotation.x);
                break;
            case Axis.Y:
                if (axisRotation.y == 0 && !increase)
                    return;
                axisRotation.y += (increase ? 1 : -1);
                GameObject.Find("yAxis/SpeedInput").GetComponentInChildren<Text>().text = axisRotation.y.ToString();
                //send y increase/decrease
                client.SendMsg("AxisRotation|Y|" + axisRotation.y);
                break;
                
            case Axis.Z:
                if (axisRotation.z == 0 && !increase)
                    return;
                axisRotation.z += (increase ? 1 : -1);
                GameObject.Find("zAxis/SpeedInput").GetComponentInChildren<Text>().text = axisRotation.z.ToString();
                //send z increase/decrease
                client.SendMsg("AxisRotation|Z|" + axisRotation.z);
                break;
            
            default:
                break;
        }

    }
   
    public void EnlargeModel()
    {
        //send scale up
        client.SendMsg("Scale|Up");
    }
    public void ShrinkModel()
    {
        //send scale down
        client.SendMsg("Scale|Down");
    }
    void ResetLightArrows()
    {
        GameObject.Find("backLight").GetComponent<Image>().color = Color.white;
        GameObject.Find("frontLight").GetComponent<Image>().color = Color.white;
        GameObject.Find("leftLight").GetComponent<Image>().color = Color.white;
        GameObject.Find("rightLight").GetComponent<Image>().color = Color.white;
    }
    public void ChooseLight(int dir)
    {
        lightDir = (LightDir)dir;
        ResetLightArrows();
        switch (lightDir)
        {
            case LightDir.NONE:
                break;
            case LightDir.BACK:
                GameObject.Find("backLight").GetComponent<Image>().color = chosenButtonColor;
                break;
            case LightDir.FRONT:
                GameObject.Find("frontLight").GetComponent<Image>().color = chosenButtonColor;
                break;
            case LightDir.LEFT:
                GameObject.Find("leftLight").GetComponent<Image>().color = chosenButtonColor;
                break;
            case LightDir.RIGHT:
                GameObject.Find("rightLight").GetComponent<Image>().color = chosenButtonColor;
                break;
            default:
                break;
        }
        
    }
    public void ChangeLightIntesity(bool intensify)
    {
        string op="Darken";
        if (intensify)
        {
            op = "Lighten";
        }
        string dir="none";
        switch (lightDir)
        {
            case LightDir.NONE:
                break;
            case LightDir.BACK:
                dir = "Back";
                break;
            case LightDir.FRONT:
                dir = "Front";
                break;
            case LightDir.LEFT:
                dir = "Left";
                break;
            case LightDir.RIGHT:
                dir = "Right";
                break;
            default:
                break;
        }
        //send dir and op 
        client.SendMsg("Light|" + dir + "|" + op);
    }
    public void RequestModel(string name)
    {
        ///send request name
        client.SendMsg("ModelRequest|" + name);
        Debug.Log("requested " + name);
    }
    public void RequestNextModel(bool next)
    {
        //send nextModel
        if (next) {
            client.SendMsg("NextModel");
        }
        //send prevModel
        else {
            client.SendMsg("PrevModel");
        }
        

    }
    public void  Exit()
    {
        Application.Quit();
    }
    public void RotModel(int rotDir)
    {
        switch ((RotDir)rotDir) {
            case RotDir.Left:
                //send rot left
                client.SendMsg("Rotate|Left");
                break;
            case RotDir.Right:
                //send rot right
                client.SendMsg("Rotate|Right");
                break;
            case RotDir.Up:
                //send rot up
                client.SendMsg("Rotate|Up");
                break;
            case RotDir.Down:
                //send rot down
                client.SendMsg("Rotate|Down");
                break;
            case RotDir.CW:
                //send rot Cw
                client.SendMsg("Rotate|Cw");
                break;
            case RotDir.CCW:
                //send CCW
                client.SendMsg("Rotate|Ccw");
                break;

        } 
    }
    public void ClearModelRot()
    {
        //send clear rot
        client.SendMsg("Clear");
    }
    void Start () {
        Screen.orientation = ScreenOrientation.Landscape;
        GameObject.Find("xAxis/SpeedInput").GetComponentInChildren<Text>().text = "0";
        GameObject.Find("yAxis/SpeedInput").GetComponentInChildren<Text>().text = "0";
        GameObject.Find("zAxis/SpeedInput").GetComponentInChildren<Text>().text = "0";
        GameObject.Find("SlidingSpeed/SpeedInput").GetComponentInChildren<Text>().text = "0";
        slidingSpeed = 0;
        axisRotation = Vector3.zero;
        AutoPresentation = true;
        chosenButtonColor = Color.white;
        chosenButtonColor.a = 0.47f;
        client.gameMenu.SetActive(false);
     


    }
	public void SetPresentationMode(bool auto)
    {
        AutoPresentation = auto;
        GameObject.Find("PresentModeSwitch").GetComponent<Switch>().isOn = AutoPresentation;
    }
    public void SetRotationAxis(Axis axis,int delta)
    {
        switch (axis)
        {
            case Axis.X:
                axisRotation.x = delta;
                GameObject.Find("xAxis/SpeedInput").GetComponentInChildren<Text>().text =delta.ToString();
                break;
            case Axis.Y:
                axisRotation.y = delta;
                GameObject.Find("yAxis/SpeedInput").GetComponentInChildren<Text>().text = delta.ToString();
                break;
            case Axis.Z:
                axisRotation.z = delta;
                GameObject.Find("zAxis/SpeedInput").GetComponentInChildren<Text>().text = delta.ToString();
                break;
        }
    }
    public void SetSlidingSpeed(int speed)
    {
        slidingSpeed = speed;
        GameObject.Find("SlidingSpeed/SpeedInput").GetComponentInChildren<Text>().text = speed.ToString();
    }
    public void LoadImage(string name,byte[] data)
    {
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(data);
        GameObject button = Instantiate(imageButtonPrefab);
        button.transform.SetParent(GameObject.Find("ImagesList/anchor").transform);
        Vector3 pos = button.transform.localPosition;
        pos.z= 0;
        button.transform.localPosition = pos;
        button.transform.localScale = new Vector3(1, 1, 1);
        button.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0,0));
        button.GetComponentInChildren<Text>().text = name;
    }
    
	// Update is called once per frame
	void Update () {
        if (doneSetup)
        {
            doneSetup = false;
            client.gameMenu.transform.position-= new Vector3(0, 0, 1000);
        }
	}
}
