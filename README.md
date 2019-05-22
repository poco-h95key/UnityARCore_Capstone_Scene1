## Previous step 
* Main : https://github.com/Esensia/UnityARCore_CapstoneDesign_Main
***
# Sunflower Rendering
![Screenshot_20190522-170004_scene_1](https://user-images.githubusercontent.com/41403898/58157627-b6d65200-7cb3-11e9-866d-611d2aaa9e91.jpg)

***
# UITouch
<img width="1198" alt="스크린샷 2019-05-23 오전 1 40 07" src="https://user-images.githubusercontent.com/41403898/58192478-d002f100-7cfb-11e9-9e0d-9699ada20ac7.png">

### UITouch class
```C#
public static int phase; // 상태를 나타내는 변수, 0일 때 변화없음, 1일 때 가구 이동 활성화, 2일 때 가구 회전 활성화, 3일 때 가구 삭제 활성화, 4일 때 나가기

public static int getPhase() // 상태를 받기위한 getter
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
    phase = -1; // UI, 객체 멀티터치 방지
    Invoke("switchMove", 1);
}
public void InvokeRotate()
{
    phase = -1;
    Invoke("switchRotate", 1);
}
public void InvokeDelete()
{
    phase = -1;
    Invoke("switchDelete", 1);
}
 ```
***

# Furniture Control
<img width="1200" alt="스크린샷 2019-05-22 오후 5 26 54" src="https://user-images.githubusercontent.com/41403898/58159121-d28f2780-7cb6-11e9-94b0-065c752af935.png">
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
    www = UnityWebRequestAssetBundle.GetAssetBundle("http://13.125.111.193/scene2and/"+Name); // get url of object 
    yield return www.SendWebRequest();
    if (www.isNetworkError || www.isHttpError)
    {
        Debug.Log(www.error);
    }
    else
    {
        bundle = DownloadHandlerAssetBundle.GetContent(www); // downloading the object
        GameObject obj = bundle.LoadAsset(Name) as GameObject; // loaded object
        obj.name = Name; 
        obj.AddComponent<touchController>(); // add touchController to object
        var newY = obj.transform.position.y; // fixed position y
        var mainCamera = Camera.main.transform; 
        obj.transform.position = new Vector3(1.5f * mainCamera.forward.x + mainCamera.position.x, newY, 1.5f *      mainCamera.forward.z + mainCamera.position.z); 
        // y값은 고정이나 카메라의 위치에 따라 사용자 앞에 오브젝트가 생성되어야 하기 때문에 x값과 z값의 변화만 줌
        Instantiate(obj);

        bundle.Unload(false); // bundle unload
        www.Dispose(); // url dispose
    }
}
```
***

### touch_move
```C#
if (UITouch.getPhase() == 1)
{
    var ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
    var hitInfo = new RaycastHit();
    if (Physics.Raycast(ray, out hitInfo))
    {
        if (hitInfo.transform.name != transform.name)
            return;
        var newPos = transform.position;
        newPos.x = hitInfo.point.x;
        if (hitInfo.point.z >= 0)
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
```
***
### touch_rotate
```C#
if (UITouch.getPhase() == 2)
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
    }
}
```
*** 
### touch_delete
```C#
if (UITouch.getPhase() == 3)
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
```
