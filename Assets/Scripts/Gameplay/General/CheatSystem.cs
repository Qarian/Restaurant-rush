using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatSystem : MonoBehaviour
{
    // Start is called before the first frame update
    string cheat;
    public GameObject inputField;
    [SerializeField] private GameObject cheatsUI = default;
    private GameObject search;
    [SerializeField] private int cheatMoney = 100;
    [SerializeField] private float cheatAccelerationMultiplier = 2;
    [SerializeField] private float cheatJumpForceMultiplier = 2;
    [SerializeField] private float cheatMaxSpeedMultiplier = 2;

    void Start()
    {
        search = cheatsUI;
        search.SetActive(!search.activeSelf);
    }

    public void CheatField()
    {
        cheat = inputField.GetComponent<Text>().text;

        switch (cheat)
        {
            case "grandma":
                Debug.Log("Cheat activated: add money");
                GrandmaCheat();
                break;

            case "holidays":
                Debug.Log("Cheat activated: more time");
                HolidaysCheat();
                break;

            case "chincha":
                Debug.Log("Cheat activated: spawned chinchilla Julian");
                SzyszaCheat();
                break;

            case "flyhigh":
                Debug.Log("Cheat activated: platforms won't fall");
                FlyhighCheat();
                break;

            case "runboyrun":
                Debug.Log("Cheat activated: max speed");
                RunboyrunCheat();
                break;

            case "ilovebunny":
                Debug.Log("Cheat activated: higher jumps");
                IlovebunnyCheat();
                break;

            case "gohome":
                Debug.Log("Cheat activated: day ended");
                GohomeCheat();
                break;

            case "wuhan":
                Debug.Log("To nawet nie jest smieszne...");
                break;

            case "inertia":
                Debug.Log("Cheat activated: no gravity");
                InertiaCheat();
                break;
        }

    }
    void GrandmaCheat()
    {
                //skrypt do dodania pieniedzy
        PointsManager.singleton.ChangeScore(cheatMoney);
    }

    void HolidaysCheat()
    {
        //skrypt do dodania czasu

    }

    void SzyszaCheat()
    {
        //skrypt do szyszy Julianka

    }

    void FlyhighCheat()
    {
        //skrypt do platform
        //FallingPlatformSKS.jumpToFallPlatformStatic = 10000;
    }

    void RunboyrunCheat()
    {
        //skrypt do biegu
        PlayerMovementWallRun.singleton.accelaration *= cheatAccelerationMultiplier;
        PlayerMovementWallRun.singleton.maxSpeed *= cheatMaxSpeedMultiplier;
    }

    void IlovebunnyCheat()
    {
        //skrypt do wyzszego skoku
        PlayerMovementWallRun.singleton.jumpForce *= cheatJumpForceMultiplier;
    }

    void GohomeCheat()
    {
        //skrypt do konca dnia
        GameManager.singleton.EndWork();
    }

    void InertiaCheat()
    {
        //skrypt do wylaczenia grawitacji

    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.RightShift))
        {
            search.SetActive(!search.activeSelf);
            CheatField();
        }*/
    }
}