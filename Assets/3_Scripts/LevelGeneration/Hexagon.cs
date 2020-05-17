﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /*
     *  Class purpose: Storing values of each individual tile and giving it behaviour, e. g. do something, when the ball stands on it
    **/
    public class Hexagon : MonoBehaviour
    {
    

        /*
        *  Negative numbers means false, but these are not booleans anymore
        *  Positive numbers get an additional meaning, for example:
        *  "isPath = 0" is the first tile of the path, "isPath = 1" the second and so on
        *  There can be more tiles which have the same number, which can make sense in the Multiplayer
        *  e.g.: Imagine a map for several players and each player has a different path,
        *  then "isPath = 0" will be the first tiles for the players, and
        *  "isStartingTile = 0" could be the starting tile for the first player,
        *  "isStartingTile = 1" for the second player, etc.
        **/
        [SerializeField] private int isPath;
        [SerializeField] private int isStartingTile;
        [SerializeField] private int isWinningTile;


        // Different booleans, not all them have setters or getters yet
        [SerializeField] private bool isCrackedTile;
        [SerializeField] private bool isMovingTile;
        [SerializeField] private bool isCheckPoint;
        [SerializeField] private bool isCurrentlyOccupied;


        // Start and End positions for moving tiles
        [SerializeField] private Vector3 movingTilePosA;
        [SerializeField] private Vector3 movingTilePosB;

        private int currentlyOccupiedCounter = 0; // Counts the number of players, who are currently on the tile
        
        // Map coordinates, not world coordinates!
        private float x;
        private float z;

        // Prepares all the standard values of the [SerializeField] for the editor mode
        public void Setup()
        {
            isPath = -1;
            isStartingTile = -1;
            isWinningTile = -1;
            isCrackedTile = false;
            isMovingTile = false;
            isCheckPoint = false;
            isCurrentlyOccupied = false;           
        }

        /*
         * Method is called at initiation of the game object.
         * Calls the method to set the positions of moving tiles and performs a coroutine that moves the tiles up and
         * down, as long as isMovingTile is set to true.
         */
        IEnumerator Start()
        {
            SetMovingTilePositions();
            while (isMovingTile) {
                yield return StartCoroutine(MoveObject(transform, movingTilePosA, movingTilePosB, 3));
                yield return StartCoroutine(MoveObject(transform, movingTilePosB, movingTilePosA, 3));
            }
        }

        /* ------------------------------ SETTER METHODS BEGINN ------------------------------  */
        
        public void SetIsPath(int status)
        {
            isPath = status; // negative numbers mean, it is not part of the path
        }
        
        public void SetIsStartingTile(int status)
        {
            isStartingTile = status; // negative numbers mean, it is not part of the path
        }

        public void SetIsWinningTile(int status)
        {
            isWinningTile = status; // negative numbers mean, it is not a winningTile
        }



        public void SetIsCrackedTile(bool status)
        {
            isCrackedTile = status;
        }
     
        public void SetIsMovingTile(bool status)
        {
            isMovingTile = status; 
        }
        

        // Setting map coordinates, not world coordinates
        public void SetMapPosition(float x, float z)
        {
            this.x = x;
            this.z = z;
        }

        public void SetWorldPosition(Vector3 pos)
        {
            transform.position = pos;
            // gameObject.transform.GetChild(0).transform.position = pos; // Probably child has to be moved as well, so this has to be tested if it is neccessary
        }

        public void SetColor(Color color)
        {
            gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = color;
        }

        public void SetIsCurrentlyOccupied(bool status)
        {
            isCurrentlyOccupied = status;
        }

        /*
         * Sets the initial positions of moving tiles
         */
        private void SetMovingTilePositions()
        {
            this.movingTilePosA = transform.position;
            Vector3 pointB = transform.position;
            pointB.y = -2;
            this.movingTilePosB = pointB;
        }


        /* ------------------------------ GETTER METHODS BEGINN ------------------------------  */

        public float GetPositionX()
        {
            return x;
        }

        public float GetPositionZ()
        {
            return z;
        }

        public bool IsCurrentlyOccupied()
        {
            return isCurrentlyOccupied;
        }

        public bool IsWinningTile()
        {
            if(isWinningTile < 0)
            {
                return false;
            }
            else
            {
                return true;
            }            
        }

        public bool IsPathTile()
        {
            if(isPath < 0)
            {
                return false;
            }
            else
            {
                return true;
            }            
        }


        /* ------------------------------ BEHAVIOUR METHODS BEGINN ------------------------------  */

        /* Method gets called in order to tell the tile that a player stands on it
        *  Depending on its values, the tile knows what to do
        **/ // All colour settings and other values like "delay" gotta go to another place later
        public void GotOccupied()
        {
            isCurrentlyOccupied = true;
            currentlyOccupiedCounter++; // Count the number of players on the tile
            
            if(IsWinningTile())
            {
                print("touched winning tile");
                // StateMachine.LevelUp();
            }
            else if(IsPathTile() & !isCrackedTile)
            {
                SetColor(Color.blue);
            }
            else if(isCrackedTile)
            {
                ActivateCrackedTile();
            }
            else if(!IsPathTile())
            {
                SetColor(Color.red);
            }
        }


        /* Method gets called in order to tell the tile that a player left the tile
        *  Depending on its values, the tile knows what to do
        **/
        public void GotUnoccupied()
        {

            // First check, if no more player is on the field
            currentlyOccupiedCounter--;
            if (currentlyOccupiedCounter == 0)
            {
                isCurrentlyOccupied = false;
            }
        }

        /*
        *  This method gets called in order to destroy a hexagon. Use always this method for this purpose, never Destroy()!
        *  Parameters: "inEditor" should be "false" if the hexagon should be deleted during Game mode - only inEditor mode "true"!
        **/
        public void DestroyHexagon(bool inEditor)
        {
            Platform platform = GetComponentInParent<Platform>(); // get the platform the tile is in
            int numberOfHexagonsInPlatform = platform.GetNumberOfHexagons(); // ask it now, platforms dies after last tile got deleted

            platform.RemoveHexagon(this, inEditor); // first tell the platform to remove the tile from the list!
            
            // if it wasn't the last tile in the platform, then destroy it
            if(numberOfHexagonsInPlatform > 1)
            {
                if(inEditor)
                {
                    DestroyImmediate(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }        


        /*
        *  Method gets called to change the color of the cracked tile and destroy it after a delay.
        **/
        void ActivateCrackedTile()
        {
            SetColor(Color.grey);
            float delay = 1f;
            Destroy (gameObject, delay);
        }


        /*
         *  Method gets called to move the tile up and down.
         */
        IEnumerator MoveObject (Transform thisTransform, Vector3 startPos, Vector3 endPos, float time)
        {
            float i = 0.0f;
            float rate = 1.0f / time;
            while (i < 1.0f) {
                i += Time.deltaTime * rate;
                thisTransform.position = Vector3.Lerp(startPos, endPos, i);
                yield return null;
            }
        }

    } // CLASS END
