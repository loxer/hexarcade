﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  
 *  Class purpose: Giving each ball (respectively player) values and behaviour 
**/
public class Ball : MonoBehaviour
{    
    private Rigidbody rb;
    private HexagonBehaviour occupiedTile;
    private Hexagon lastSpawnPosition;
    private Timer timer;
    private List<Vector3> positions = new List<Vector3>();
    private int playerNumber;
    TileColors tileColors;
    private float loseHeight = -10;

    private int replayPositionCounter = 0;


    /* ------------------------------ STANDARD METHODS BEGINN ------------------------------  */

    public void GetStarted(int playerNumber)
    {
        rb = GetComponent<Rigidbody>();
        timer = this.GetComponentInChildren<Timer>();
        this.playerNumber = playerNumber;

        GameObject loseTile = GameObject.Find("Map/LoseHeight");
        loseHeight = loseTile.transform.position.y;

        GameObject tiles = GameObject.Find("Map/Tiles");
        tileColors = tiles.GetComponent<TileColors>();
        
        StartCoroutine(Introduction());
    }


    IEnumerator Introduction()
    {
        tileColors.DisplayTiles();
        
        // Just wait for the tiles to finish 
        while(!tileColors.IsFinished())
        {
            yield return new WaitForSeconds(0.1f);
        }        
        ActivatePlayerControls();
        timer.Show();

        GameStarts();
    }

    void GameStarts()
    {
        StartCoroutine(CheckLoseCondition());
    }

    IEnumerator CheckLoseCondition()
    {
        // Lose condition through falling
        for(;;)
        {
            if(loseHeight > transform.position.y)
            {
                PlayerLost();
            }
            yield return new WaitForSeconds(0.1f);
        }            
    }

    /*  
     *  Everythign which has to do with the players movement
    **/
    void FixedUpdate()
    {
        // Save current position, not used yet
        positions.Add(transform.position);

        // Start ghost/replay --- JUST A TEST SO FAR ---
        if(Input.GetKeyDown(KeyCode.R))
        {                     
            transform.position = positions[replayPositionCounter];
            replayPositionCounter++;
        }     
    }

    /*  
     *  The player checks, if it is standing on a tile; if true, then save the currentTile and tell the current and former tile
    **/
    void OnCollisionEnter(Collision collision)    
    {        
        GameObject tile = collision.gameObject;

        if(tile.tag == "Tile")
        {
            HexagonBehaviour currentTile = tile.GetComponent<HexagonBehaviour>();
        
            if(occupiedTile != currentTile)         // Check if the former occupiedTile has changed   
            {
                if(occupiedTile != null)            // Prevent a NullReferenceException
                {
                    occupiedTile.GotUnoccupied(this);   // Tell the former occupiedTile, that this ball left
                }                    
                currentTile.GotOccupied(this);          // Tell the currentTile, that this player stands on it
                occupiedTile = currentTile;         // Save the current tile

                AnalyseHexagon(occupiedTile.GetComponent<Hexagon>());
            }            
        }
    }



    /* ------------------------------ BEHAVIOUR METHODS BEGINN ------------------------------  */

    /*  
     *  Let the player spawn above the desired tile
    **/
    private void AnalyseHexagon(Hexagon hexagon)
    {

        if(hexagon.IsStartingTile())
        {
            timer.StartTiming();
            Debug.Log("Timer started/reseted");
            Debug.Log("Record to beat: " + timer.GetBestTime());
        }
        else if(hexagon.IsWinningTile())
        {
            timer.StopTiming();

            Debug.Log("Finish time: " + timer.GetLastFinishTime());

            if(timer.IsNewBestTime())
            {
                Debug.Log("New record");
            }
            else
            {
                Debug.Log("No new record");
            }
        }
    }

    private void PlayerLost()
    {
        GoToSpawnPosition(lastSpawnPosition);
    }

    
    public void GoToSpawnPosition(Hexagon spawnTile)
    {
        float distanceAboveTile = 1f; // Should go later to a central place for all settings
        transform.position = new Vector3(spawnTile.transform.position.x, spawnTile.transform.position.y + distanceAboveTile, spawnTile.transform.position.z);
        lastSpawnPosition = spawnTile;
    }


    /*
     *  Deactivates the player attached scripts "Ball" and "AccelerometerMovement". Hence all the effects and manipulations caused by them will be absent.
    **/
    void DeactivatePlayerControls()
    {
        GetComponent<BallControls>().enabled = false;
        GetComponent<AccelorometerMovement>().enabled = false;
    }


    /*
     * Activates the player attached scripts "Ball" and "AccelerometerMovement"
     **/
    void ActivatePlayerControls()
    {
        GetComponent<BallControls>().enabled = true;
        GetComponent<AccelorometerMovement>().enabled = true;
    }


} // CLASS END