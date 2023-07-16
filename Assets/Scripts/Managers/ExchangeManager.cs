using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ExchangeManager : MonoBehaviour
{
    public static ExchangeManager instance = null;
    
    [SerializeField] private GameObject pnl_exchange;
    [SerializeField] private GameObject pnl_objects;
    
    [SerializeField] private GameObject pnl_listObjects;
    [SerializeField] private GameObject pnl_listCurrency;
    
    [SerializeField] private GameObject pnl_descriptionObject;
    [SerializeField] private GameObject txt_nameItem;
    [SerializeField] private GameObject txt_description;
    
    [SerializeField] private List<ExchangeObject> listExchangeObjects;

    [SerializeField][Range(0,3)] private int cursorLocalPosition;
    [SerializeField] private int cursorGlobalPosition;
    [SerializeField] private int sumItems;
    [SerializeField] private int sumVisiableItems;

    // Списки ссылок на pnl_
    [SerializeField]private List<GameObject> listGameObjectsItems;
    [SerializeField]private List<GameObject> listGameObjectsCurrency;
    
    
    InputManager input;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
        input = InputManager.instance;

        input.playerInput.actions["ContinueDialogue"].performed += BuyPressed;
        
        
        for (int i = 0; i < pnl_listObjects.transform.childCount; i++)
        {
            listGameObjectsItems.Add(pnl_listObjects.transform.GetChild(i).gameObject);
        }
        
        for (int i = 0; i < pnl_listCurrency.transform.childCount; i++)
        {
            listGameObjectsCurrency.Add(pnl_listCurrency.transform.GetChild(i).gameObject);
        }



        LoadExchangeObjects();


    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Buy()
    {
        if(sumItems<1)return;
        //Взаимодействие с инвентарём пользователя, удаление валюты, записывание нового предмета в инвентарь
        listExchangeObjects.RemoveAt(cursorGlobalPosition);
        //Обновление количества объектов
        sumItems = listExchangeObjects.Count;
        sumVisiableItems = sumItems < 4 ? sumItems : 4;
        
        if ((cursorGlobalPosition>sumItems-1 || (cursorGlobalPosition-cursorLocalPosition+4>sumItems-1 && cursorGlobalPosition>1)&&cursorGlobalPosition-cursorLocalPosition>0))
        {
            cursorGlobalPosition--;
        }

        if (sumVisiableItems-1<cursorLocalPosition)
        {
            cursorLocalPosition--;
        }

        UpdateListDisplayablesExchangeObjects();
    }

    void UpdateListDisplayablesExchangeObjects()
    {
        sumItems = listExchangeObjects.Count;
        sumVisiableItems = sumItems < 4 ? sumItems : 4;

        

        // Сокрытие списка объектов
        for (int i = 0; i < listGameObjectsItems.Count; i++)
        {
            listGameObjectsItems[i].SetActive(false);
        }
        // Сокрытие элементов цены покупаемого объекта
        for (int i = 0; i < listGameObjectsCurrency.Count; i++)
        {
            listGameObjectsCurrency[i].SetActive(false);
        }
        //Сокрытие описания и наименования
        txt_nameItem.GetComponent<Text>().text = "";
        txt_description.GetComponent<Text>().text = "";
        
        if (sumItems == 0) return;
        // Обновление видимого списка объектов на покупку
        for (int i = 0; i < sumVisiableItems; i++)
        {
            listGameObjectsItems[i].SetActive(true);
            
            listGameObjectsItems[i].transform.GetChild(0).gameObject.SetActive(false);
            listGameObjectsItems[i].transform.GetChild(2).GetComponent<Text>().text =
                listExchangeObjects[cursorGlobalPosition-cursorLocalPosition+i].name;
            
            Sprite sprite = Resources.Load<Sprite>(listExchangeObjects[cursorGlobalPosition-cursorLocalPosition+i].pathImage);
            if (sprite) listGameObjectsItems[i].transform.GetChild(1).GetComponent<Image>().sprite = sprite;
            else
            {
                Debug.Log("Такой картинки нет");
            }
            
        }

        // Заполнение полей выбранного объекта
        listGameObjectsItems[cursorLocalPosition].transform.GetChild(0).gameObject.SetActive(true);
        txt_nameItem.GetComponent<Text>().text = listExchangeObjects[cursorGlobalPosition].name;
        txt_description.GetComponent<Text>().text = listExchangeObjects[cursorGlobalPosition].description;
        
        // Показ только элементов цены выбранного товара
        for (int i = 0; i < listExchangeObjects[cursorGlobalPosition].listCurrency.Count; i++)
        {
            listGameObjectsCurrency[i].SetActive(true);
            
            Sprite sprite = Resources.Load<Sprite>(listExchangeObjects[cursorGlobalPosition].listCurrency[i].pathImage);
            if (sprite) listGameObjectsCurrency[i].transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            else
            {
                Debug.Log("Такой картинки нет");
            }
            listGameObjectsCurrency[i].transform.GetChild(1).GetComponent<Text>().text = 
                listExchangeObjects[cursorGlobalPosition].listCurrency[i].amount;

        }

    }

    public void MoveGlobalCursorUp()
    {
        if (cursorGlobalPosition>0)
        {
            cursorGlobalPosition--;
        }
        
        if (cursorLocalPosition>1||(cursorGlobalPosition<1&&cursorLocalPosition>0))
        {
            cursorLocalPosition--;
        }
        else
        {
            // UpdateListDisplayablesExchangeObjects();
            // cursorLocalPosition
        }

        UpdateListDisplayablesExchangeObjects();
    }
    
    public void MoveGlobalCursorDown()
    {
        if (cursorGlobalPosition<sumItems-1)
        {
            cursorGlobalPosition++;
        }

        if ((cursorLocalPosition<2||cursorGlobalPosition>sumItems-2)&&cursorLocalPosition<sumVisiableItems-1)
        {
            cursorLocalPosition++;
        }
        else
        {
            // UpdateListDisplayablesExchangeObjects();
            // cursorLocalPosition
        }

        UpdateListDisplayablesExchangeObjects();
    }
    

    void LoadExchangeObjects()
    {
        sumItems = listExchangeObjects.Count;
        sumVisiableItems = sumItems < 4 ? sumItems : 4;
        UpdateListDisplayablesExchangeObjects();
    }

    void BuyPressed(InputAction.CallbackContext context)
    {
        Buy();
    }
    void EscPressed(InputAction.CallbackContext context)
    {
        // Esk();
    }
    void UpPressed(InputAction.CallbackContext context)
    {
        MoveGlobalCursorUp();
    }
    
    void DownPressed(InputAction.CallbackContext context)
    {
        MoveGlobalCursorDown();
    }

    private void OnDestroy()
    {
        InputManager.instance.playerInput.actions["ContinueDialogue"].performed -= BuyPressed;
    }
}
[Serializable] public class ExchangeObject
{
    public string name;
    public string description;
    public string pathImage;
    public List<ExchangedPrice> listCurrency;


}

[Serializable] public class ExchangedPrice
{
    public string name;
    public string amount;
    public string pathImage;

}


