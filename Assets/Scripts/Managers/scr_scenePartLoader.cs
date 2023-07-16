using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

public enum CheckMethod
{   
    Trigger,
    Distance
}

public class scr_scenePartLoader : MonoBehaviour
{
    [SerializeField]private Transform player;
    [SerializeField]private CheckMethod checkMethod;
    [SerializeField]private float loadRange;

    [SerializeField]private bool isLoaded;
    [SerializeField]private bool shouldLoad;

    private bool gameManagerCalled = false;

    scr_GameManager GameManager;
    scr_SaveObjectManager SaveObjectManager;

    void Start()
    {
        GameManager = scr_GameManager.instance;
        SaveObjectManager = scr_SaveObjectManager.instance;

        player = GameManager.player.transform;

        if (SceneManager.sceneCount > 0)
        {
            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == gameObject.name)
                {
                    isLoaded = true;
                }
            }
        }
    }

    void Update()
    {
        if (checkMethod == CheckMethod.Distance)
        {
            DistanceCheck();
        }
        else if (checkMethod == CheckMethod.Trigger)
        {
            TriggerCheck();
        }
    }

    void DistanceCheck()
    {
        if (Vector3.Distance(player.position, transform.position) < loadRange)
        {
            LoadScene();
        }
        else
        {
            UnLoadScene();
        }
    }

    void LoadScene()
    {
        if (SceneManager.GetSceneByName(gameObject.name).isLoaded)
        {
            isLoaded = true;
        }

        if (!isLoaded)
        {
            isLoaded = true;
            gameManagerCalled = true;

            StartCoroutine(WaitForSceneToLoad());
        }

        if (!gameManagerCalled)
        {
            GameManager.NewSceneLoaded(gameObject.name);
            gameManagerCalled = true;
        }
    }

    private IEnumerator WaitForSceneToLoad()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        GameManager.NewSceneLoaded(gameObject.name);
    }

    void UnLoadScene()
    {
        if (isLoaded)
        {
            //SceneManager.UnloadSceneAsync(gameObject.name);
            isLoaded = false;
            // Debug.Log("��������"+gameObject.name);

            //GameManager.SceneUnloaded(gameObject.name);

            StartCoroutine(WaitForSceneToUnload());
        }
    }

    private IEnumerator WaitForSceneToUnload()
    {
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(gameObject.name);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        GameManager.SceneUnloaded(gameObject.name);
    }

    private void OnTriggerEnter2D(Collider2D colider)
    {
        if (colider.CompareTag("Player"))
        {
            shouldLoad = true;
        }
    }

    private void OnTriggerExit2D(Collider2D colider)
    {
        if (colider.CompareTag("Player"))
        {
            shouldLoad = false;
        }
    }

    void TriggerCheck()
    {
        if (shouldLoad)
        {
            LoadScene();
        }
        else
        {
            UnLoadScene();
        }
    }

    private void OnDestroy()
    {
        GameManager.SceneUnloaded(gameObject.name);
    }
}
