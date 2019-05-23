## Previous step 
* Main : https://github.com/Esensia/UnityARCore_CapstoneDesign_Main
***
# Rendering
![Screenshot_20190522-170004_scene_1](https://user-images.githubusercontent.com/41403898/58165871-9ca47000-7cc3-11e9-9d90-e9e7626b03e7.jpg)

***
# UITouch
<img width="1196" alt="스크린샷 2019-05-23 오전 11 47 54" src="https://user-images.githubusercontent.com/41403898/58222339-ad4ef780-7d50-11e9-8d10-01c926400f25.png">

### UITouch class
```C#
public static int phase; 
// {0 : No change}, {1 : Activate movement}, {2 : Activate rotation}, {3 : Activate removal}, {4 : Quit}

public static int getPhase() // phase getter
{
    return phase;
}
public void switchSelect()
{
    phase = 0;
}
public void switchMove()
{
    phase = 1;
}

public void switchRotate()
{
    phase = 2;
}
public void switchDelete()
{
    phase = 3;
}
public void Quit()
{
    phase = 4;
    SceneManager.LoadSceneAsync("MainScreen", LoadSceneMode.Single);
    Screen.orientation = ScreenOrientation.Portrait;
}
public void InvokeMove()
{
    phase = -1; 
    Invoke("switchMove", 0.5); // phase = -1 => 0.5 delay time => phase = 1 (avoid multi touch)
}
public void InvokeRotate()
{
    phase = -1;
    Invoke("switchRotate", 0.5);
}
public void InvokeDelete()
{
    phase = -1;
    Invoke("switchDelete", 0.5);
}
```
***

# Furniture Control
<img width="1200" alt="스크린샷 2019-05-23 오전 11 50 42" src="https://user-images.githubusercontent.com/41403898/58222443-06b72680-7d51-11e9-9b08-da959307b12c.png">

### AssetBundle_Furniture_Load
```C#
AssetBundle bundle;
UnityWebRequest www;

public void SelectiveLoad()
{
    name = EventSystem.current.currentSelectedGameObject.name; // button click, then object same as the name of clicked button is loaded 
    StartCoroutine(GetAssetBundle(name));
}
    
IEnumerator GetAssetBundle(string Name)
{
    www = UnityWebRequestAssetBundle.GetAssetBundle("http://13.125.111.193/scene2and/"+Name); 
    yield return www.SendWebRequest();
    if (www.isNetworkError || www.isHttpError)
    {
        Debug.Log(www.error);
    }
    else
    {
        bundle = DownloadHandlerAssetBundle.GetContent(www); 
        GameObject obj = bundle.LoadAsset(Name) as GameObject; 
        obj.name = Name; 
        obj.AddComponent<touchController>(); // add touchController to object
        
        var newY = obj.transform.position.y; 
        var mainCamera = Camera.main.transform; 
        obj.transform.position = new Vector3(1.5f * mainCamera.forward.x + mainCamera.position.x, newY, 1.5f *      mainCamera.forward.z + mainCamera.position.z); 
        
        Instantiate(obj);

        bundle.Unload(false); 
        www.Dispose(); 
    }
}
```
*** 
### touchController

```C#
if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) // To avoid UI,Object multi touch
{
    return;
}
else
{
    if (UITouch.getPhase() == 1) // Operate movement
    {
        var ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
        var hitInfo = new RaycastHit();

        if (Physics.Raycast(ray, out hitInfo))
        {
           if (hitInfo.transform.name != transform.name)
               return;
           var newPos = transform.position;
           newPos.x = hitInfo.point.x;
           if(hitInfo.point.z >=0)
           {
               newPos.z = hitInfo.point.z + 0.2f;
           }
           else
           {
               newPos.z = hitInfo.point.z - 0.2f;
           }
           transform.position = newPos;

        }
    }
    if (UITouch.getPhase() == 2) // Operate rotation
    {
        var ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
        var hitInfo = new RaycastHit();
        float rotSpeed = 20;
        if (Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.transform.name != transform.name)
                return;
            float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
            transform.Rotate(Vector3.up, -rotX);

            //var yAxis = hitInfo.point.y*rotSpeed;
            //transform.Rotate(Vector3.up, yAxis);
        }

    }
    if (UITouch.getPhase() == 3) // // Operate removal
    {
        var ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
        var hitInfo = new RaycastHit();

        if (Physics.Raycast(ray, out hitInfo))
        {
           if (hitInfo.transform.name != transform.name)
               return;
           Destroy(hitInfo.transform.gameObject);

        }

    }
}
```
