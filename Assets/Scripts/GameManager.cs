using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : SingletonComponent<GameManager>
{
    public GameObject player;
    public GameObject camera;
    public GameObject controller1;
    public GameObject controller2;
    public float maxHook = 100;
    public Controller controller;
}
