﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class MultiplayerInputManager : MonoBehaviour
{
    //public data members that need explicit assignment in-editor
    public GameObject[] inputButtonObjects;
    public Material[] buttonMaterials;
    public List<Text> scoreObj = new List<Text>();

    //public data members that are assigned at runtime
    public bool[] notesOnButtons;


    //private data members
    private MeshRenderer[] inputButtonMaterials;
    private bool[] buttonStates;
    private MultiplayerButtonManager[] buttonScriptReferences;

    public Text announcementText;
    public Text powerupText;
    public GameObject backButton;
    public PlayerProps playerProps;

    private bool validInput = true;

    private bool hasPowerup = false;
    private bool powerupUsed = false;

    // Start is called before the first frame update
    void Start()
    {
        inputButtonMaterials = new MeshRenderer[inputButtonObjects.Length];
        buttonStates = new bool[inputButtonObjects.Length];
        buttonScriptReferences = new MultiplayerButtonManager[inputButtonObjects.Length];
        notesOnButtons = new bool[inputButtonObjects.Length];

        for (int i = 0; i < inputButtonObjects.Length; i++)
        {
            inputButtonMaterials[i] = inputButtonObjects[i].GetComponent<MeshRenderer>();
            buttonStates[i] = false;
            notesOnButtons[i] = false;
            buttonScriptReferences[i] = inputButtonObjects[i].GetComponent<MultiplayerButtonManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {

        List<int> scores = new List<int>();

        if (PhotonNetwork.CurrentRoom != null)
        {
            for (int i = 1; i <= PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                scores.Add(PhotonNetwork.CurrentRoom.Players[i].GetScore());
            }
        }

        int highestScore = scores[scores.Count - 1];

        if (validInput == true)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                inputButtonMaterials[0].material = buttonMaterials[5];
                buttonStates[0] = true;
            }
            if (Input.GetButtonDown("Fire2"))
            {
                inputButtonMaterials[1].material = buttonMaterials[6];
                buttonStates[1] = true;
            }
            if (Input.GetButtonDown("Fire3"))
            {
                inputButtonMaterials[2].material = buttonMaterials[7];
                buttonStates[2] = true;
            }
            if (Input.GetButtonDown("Fire4"))
            {
                inputButtonMaterials[3].material = buttonMaterials[8];
                buttonStates[3] = true;
            }
            if (Input.GetButtonDown("Fire5"))
            {
                inputButtonMaterials[4].material = buttonMaterials[9];
                buttonStates[4] = true;
            }

            if (Input.GetButtonUp("Fire1"))
            {
                inputButtonMaterials[0].material = buttonMaterials[0];
                buttonStates[0] = false;
            }
            if (Input.GetButtonUp("Fire2"))
            {
                inputButtonMaterials[1].material = buttonMaterials[1];
                buttonStates[1] = false;
            }
            if (Input.GetButtonUp("Fire3"))
            {
                inputButtonMaterials[2].material = buttonMaterials[2];
                buttonStates[2] = false;
            }
            if (Input.GetButtonUp("Fire4"))
            {
                inputButtonMaterials[3].material = buttonMaterials[3];
                buttonStates[3] = false;
            }
            if (Input.GetButtonUp("Fire5"))
            {
                inputButtonMaterials[4].material = buttonMaterials[4];
                buttonStates[4] = false;
            }


            if (Input.GetButtonDown("Fire6") || Input.GetButtonDown("Strum") || true)
            {
                if (checkStrum())
                {
                    executeStrum();
                    PhotonNetwork.LocalPlayer.AddScore(1000 + Random.Range(0, 500));
                    getScores();
                }
            }

            if (Input.GetButtonDown("Powerup"))
            {
                if (hasPowerup)
                {
                    if (PhotonNetwork.CurrentRoom != null)
                    {
                        for (int i = 1; i <= PhotonNetwork.CurrentRoom.PlayerCount; i++)
                        {
                            if (highestScore == PhotonNetwork.CurrentRoom.Players[i].GetScore())
                            {
                                if (powerupUsed == false)
                                {
                                    PhotonView PV = PhotonView.Get(this);
                                    PV.RPC("RPC_Powerup1", PhotonNetwork.CurrentRoom.Players[i]);
                                    powerupUsed = true;
                                }
                            }
                        }
                    }
                }
                hasPowerup = false;
                if (hasPowerup == false)
                {
                    powerupText.text = " ";
                }

            }
        }

        getScores();
        scores.Sort();

        for (int i = 1; i <= PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            if (highestScore > 5000)
            {
                if (scores[0] == PhotonNetwork.CurrentRoom.Players[i].GetScore())
                {
                    PhotonView PV = PhotonView.Get(this);
                    PV.RPC("RPC_Eliminate", PhotonNetwork.CurrentRoom.Players[i]);
                    PV.RPC("RPC_CancelPlayer", RpcTarget.All, PhotonNetwork.CurrentRoom.Players[i]);
                }
                if (scores[1] == PhotonNetwork.CurrentRoom.Players[i].GetScore())
                {
                    PhotonView PV = PhotonView.Get(this);
                    PV.RPC("RPC_GivePowerup", PhotonNetwork.CurrentRoom.Players[i]);
                }
            }
        }

        for (int i = 1; i <= PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            if (highestScore > 10000)
            {
                if (scores[1] == PhotonNetwork.CurrentRoom.Players[i].GetScore())
                {
                    PhotonView PV = PhotonView.Get(this);
                    PV.RPC("RPC_Eliminate", PhotonNetwork.CurrentRoom.Players[i]);
                    PV.RPC("RPC_CancelPlayer", RpcTarget.All, PhotonNetwork.CurrentRoom.Players[i]);
                }
                if (scores[2] == PhotonNetwork.CurrentRoom.Players[i].GetScore())
                {
                    PhotonView PV = PhotonView.Get(this);
                    PV.RPC("RPC_GivePowerup", PhotonNetwork.CurrentRoom.Players[i]);
                }
            }
        }

        for (int i = 1; i <= PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            if (highestScore > 15000)
            {
                if (scores[2] == PhotonNetwork.CurrentRoom.Players[i].GetScore())
                {
                    PhotonView PV = PhotonView.Get(this);
                    PV.RPC("RPC_Eliminate", PhotonNetwork.CurrentRoom.Players[i]);
                    PV.RPC("RPC_CancelPlayer", RpcTarget.All, PhotonNetwork.CurrentRoom.Players[i]);
                    PV.RPC("RPC_BackButton", RpcTarget.All);
                }
                if (highestScore == PhotonNetwork.CurrentRoom.Players[i].GetScore())
                {
                    PhotonView PV = PhotonView.Get(this);
                    PV.RPC("RPC_Winner", PhotonNetwork.CurrentRoom.Players[i]);
                }
            }
        }

        /*for (int i = 1; i <= PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            if (highestScore > 12000)
            {
                if (scores[3] == PhotonNetwork.CurrentRoom.Players[i].GetScore())
                {
                    PhotonView PV = PhotonView.Get(this);
                    PV.RPC("RPC_Eliminate", PhotonNetwork.CurrentRoom.Players[i]);
                    PV.RPC("RPC_CancelPlayer", RpcTarget.All, PhotonNetwork.CurrentRoom.Players[i]);
                }
                if (highestScore == PhotonNetwork.CurrentRoom.Players[i].GetScore())
                {
                    PhotonView PV = PhotonView.Get(this);
                    PV.RPC("RPC_Powerup1", PhotonNetwork.CurrentRoom.Players[i]);
                }
            }
        }

        for (int i = 1; i <= PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            if (highestScore > 15000)
            {
                if (scores[4] == PhotonNetwork.CurrentRoom.Players[i].GetScore())
                {
                    PhotonView PV = PhotonView.Get(this);
                    PV.RPC("RPC_Eliminate", PhotonNetwork.CurrentRoom.Players[i]);
                    PV.RPC("RPC_CancelPlayer", RpcTarget.All, PhotonNetwork.CurrentRoom.Players[i]);
                    PV.RPC("RPC_BackButton", RpcTarget.All);
                }

                if (highestScore == PhotonNetwork.CurrentRoom.Players[i].GetScore())
                {
                    PhotonView PV = PhotonView.Get(this);
                    PV.RPC("RPC_Winner", PhotonNetwork.CurrentRoom.Players[i]);
                }
            }
        }*/
    }

    private bool checkStrum()
    {
        bool atLeastOne = false;
        for (int i = 0; i < buttonStates.Length; i++)
        {
            if ((buttonStates[i]) != notesOnButtons[i])
            {
                return false;
            }
            if (notesOnButtons[i])
            {
                atLeastOne = true;
            }
        }
        return atLeastOne;
    }

    private void executeStrum()
    {
        for (int i = 0; i < notesOnButtons.Length; i++)
        {
            if (notesOnButtons[i])
            {
                buttonScriptReferences[i].destroyNote();
            }

            notesOnButtons[i] = false;
        }
    }

    private void getScores()
    {
        if (SceneManager.GetActiveScene().name == "MultiPlayerScene")
        {
            if (PhotonNetwork.CurrentRoom != null)
            {
                for (int i = 1; i <= PhotonNetwork.CurrentRoom.PlayerCount; i++)
                {
                    scoreObj[i].text = PhotonNetwork.CurrentRoom.Players[i].GetScore().ToString();
                }
            }
        }
    }


    [PunRPC]
    private void RPC_Eliminate()
    {
        validInput = false;
        announcementText.text = "You have been eliminated";
    }

    [PunRPC]
    private void RPC_BackButton()
    {
        backButton.SetActive(true);
    }

    [PunRPC]
    private void RPC_Winner()
    {
        announcementText.text = "You are the winner!";
        validInput = false;
    }

    [PunRPC]
    private void RPC_CancelPlayer(Player player)
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            for (int i = 1; i <= PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                if (playerProps.playerNames[i].text == player.NickName)
                {
                    playerProps.playerNames[i].GetComponent<Text>().color = Color.red;
                }
                if (scoreObj[i].text == player.GetScore().ToString())
                {
                    scoreObj[i].GetComponent<Text>().color = Color.red;
                }
            }
        }
    }

    IEnumerator Powerup1()
    {
        validInput = false;
        powerupText.text = "Incoming Powerup";
        yield return new WaitForSeconds(5);
        validInput = true;
        powerupText.text = " ";
    }

    [PunRPC]
    private void RPC_Powerup1()
    {
        StartCoroutine(Powerup1());
    }

    [PunRPC]
    private void RPC_GivePowerup()
    {
        hasPowerup = true;
        powerupText.text = "Press P to use powerup";

    }

}