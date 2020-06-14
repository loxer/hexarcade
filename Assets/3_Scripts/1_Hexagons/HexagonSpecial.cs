﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonSpecial : MonoBehaviour
{    

    /* ------------------------------ FIELDS FOR TELEPORTERS ------------------------------  */
    [SerializeField] private bool teleporterEntrance;
    [SerializeField] private bool keepSpeedThroughTeleporter;
    [SerializeField] private bool reverseSpeed;
    [SerializeField] private int teleporterNumber;
    [SerializeField] private int teleporterConnectedWith;
    [SerializeField] private Vector3 teleporterOffset;


    /* ------------------------------ FIELDS FOR VELOCITY TILES ------------------------------  */
    [SerializeField] private float velocity;


    /* ------------------------------ FIELDS FOR JUMPAD ------------------------------  */
    [SerializeField] private Vector3 jumpDirection;


    /* ------------------------------ SPECIAL-TILE NUMBERS ------------------------------  */
    private int specialCase;
    private const int TELEPORTER = 0;
    private const int VELOCITY = 1;
    private const int JUMPAD = 2;


    /* ------------------------------ GENERAL INFORMATION FOR DIFFERENT OPERATIONS ------------------------------  */    
    private Dictionary<int, List<Hexagon>> specialTiles;
    private List<Ball> players = new List<Ball>();
    private int getIndexNumberInList;
    private Hexagon thisHexagon;
    


    /* ------------------------------ MAIN METHODS FOR SPECIAL TILES ------------------------------  */
    public void GetStarted(Dictionary<int, List<Hexagon>> specialTiles)
    {
        thisHexagon = this.transform.GetComponent<Hexagon>();
        specialCase = thisHexagon.GetSpecialNumber();
        this.specialTiles = specialTiles;
    }

    public void SpecialTileTouched(Ball player)
    {
        players.Add(player);
        getIndexNumberInList = GetIndexNumberInList();

        if(getIndexNumberInList >= 0) // catch potential errors with the special tile list
        {
            switch(specialCase)
            {
                case TELEPORTER:
                    if(teleporterEntrance)
                    {
                        if(!keepSpeedThroughTeleporter) player.StopMovement();
                        if(reverseSpeed) player.ReverseMovement();

                        Hexagon teleporterExit = FindTeleporterExit();
                        if(teleporterExit) player.GoToSpawnPosition(teleporterExit, teleporterOffset, false);                        
                    }
                break;

                case VELOCITY:
                    
                    Rigidbody rb = player.GetRigidbody();
                    Vector3 currentVelocity = rb.velocity;
                    currentVelocity *= velocity;
                    rb.velocity = currentVelocity;
                break;

                case JUMPAD:       
                    player.GetRigidbody().AddForce(jumpDirection);
                break;
            }
        }
    }

    /* ------------------------------ GENERAL METHODS FOR SPECIAL TILES ------------------------------  */
    public void SpecialTileLeft(Ball player)
    {
        players.Remove(player);

        if(specialCase == 0)
        {
            
        }
    }

    private int GetIndexNumberInList()
    {
        for(int i = 0; i < specialTiles[specialCase].Count; i++)
        {
            if(specialTiles[specialCase][i] == thisHexagon)
            {
                return i;
            }
        }
        return int.MinValue;
    }

    
    /* ------------------------------ SPECIFIC METHODS FOR TELEPORTERS ------------------------------  */

    private Hexagon FindTeleporterExit()
    {
        List<Hexagon> teleporterList = specialTiles[TELEPORTER];

        for(int i = 0; i < teleporterList.Count; i++)
        {
            HexagonSpecial teleporter = teleporterList[i].GetComponent<HexagonSpecial>();

            if(teleporter.GetTeleporterNumber() == teleporterConnectedWith)
            {
                return teleporter.GetComponent<Hexagon>();
            }
        }
        Debug.Log("This teleporter has not exit");
        return null;
    }

    public int GetTeleporterNumber()
    {
        return teleporterNumber;
    }

    /* ------------------------------ SETTERS FOR SPECIAL TILES ------------------------------  */

    public void SetVelocity(float velocity)
    {
        this.velocity = velocity;
    }



    /* ------------------------------ EDITOR METHODS ------------------------------  */

    public string GetNameOfFunction()
    {
        string prefix = "-> ";
        
        switch(specialCase)
        {
            case TELEPORTER:    
                prefix += nameof(TELEPORTER).ToLower(); 
                if(teleporterEntrance)
                {
                    prefix += ", " + teleporterNumber + " to " + teleporterConnectedWith;
                }
                else
                {
                    prefix += "_exit " + teleporterNumber;
                }
                return prefix;
    
            case VELOCITY:
                return prefix + nameof(VELOCITY).ToLower() + " " + velocity;
    
            case JUMPAD:
                return prefix + nameof(JUMPAD).ToLower();
        }
        return "";
    }
}