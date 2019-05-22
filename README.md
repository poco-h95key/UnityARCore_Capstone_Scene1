* Main : https://github.com/Esensia/UnityARCore_CapstoneDesign_Main

## AssetBundle_Furniture_Load
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
##### 가져온 AssetBundle에 touchController클래스를 넣어 move,rotate,delete 활성화, 코루틴을 사용하여 객체를 불러옴 

## UITouch class
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
 ##### Select버튼, Move버튼, Rotate버튼, Delete버튼에 따라 상태를 바꿔 가구의 움직임을 조정가능 <br>
***
## touch_move
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
##### y축은 고정하고 x축과 z축 방향으로의 이동을 구현, 사용자와 거리를 두기위해 z이동의 가중치 추가
***
## touch_rotate
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
##### x축을 기준으로 왼쪽드래그, 오른쪽드래그에 따라 물체의 회전이 가능
*** 
## touch_delete
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
##### 삭제하고자 하는 물체 터치시, 객체 파괴

# Execution screen
![Screenshot_20190419-205407_final](https://user-images.githubusercontent.com/41403898/56423076-67cc8400-62e5-11e9-82f0-2a47cc456be5.jpg)
