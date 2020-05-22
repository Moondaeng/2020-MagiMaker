using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CUiActive : MonoBehaviour
{
    public static void ActiveAllForm()
    {
        foreach (var obj in GameObject.FindGameObjectsWithTag("Input Form"))
        {
            obj.GetComponent<InputField>().interactable = true;
        }

        foreach (var obj in GameObject.FindGameObjectsWithTag("Button Form"))
        {
            obj.GetComponent<Button>().interactable = true;
        }
    }

    public static void DisableAllForm()
    {
        foreach (var obj in GameObject.FindGameObjectsWithTag("Input Form"))
        {
            obj.GetComponent<InputField>().interactable = false;
        }

        foreach (var obj in GameObject.FindGameObjectsWithTag("Button Form"))
        {
            obj.GetComponent<Button>().interactable = false;
        }
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
