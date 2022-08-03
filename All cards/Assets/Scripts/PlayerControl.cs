using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

using System;

public class PlayerControl : MonoBehaviour
{
    public ActionDisplay actionDisplay;

    public bool playerLost;

    public Camera playerCam;
    Vector3 cameraDirection;
    IEnumerator move;
    IEnumerator scroll;
    Tile prevTile;
    Tile currTile;
    CharacterStats currUnit;

    bool moving;
    public bool attacking;
    bool abilityMenuOpen;
    bool confirming;
    int tileDistance;

    // for use when ability is used
    bool casting;
    int abilityRange;
    int abilityRadius;
    int abilityIndex;
    Tile targetedTile;
    HashSet<Tile> targetedTiles;
    bool usingItems;

    public List<CharacterStats> units;
    public BUIManager UI;
    public EnemyControl enemy;
    bool playerTurn;

    // Start is called before the first frame update
    void Start()
    {
        playerLost = false;
        scroll = null;
        moving = false;
        casting = false;
        HashSet<Tile> targetedTiles = new HashSet<Tile>();
        abilityMenuOpen = false;
        cameraDirection = new Vector3(0, 0, 0);
        currTile = null;
    }

    // Update is called once per frame
    void Update()
    {
        // Cannot select tiles when confirming
        if (!confirming)
        {
            // Move
            if (Input.GetKeyDown(Inputs.move) && playerTurn && currTile != null && currTile.currUnit != null && !currTile.currUnit.moved && !abilityMenuOpen && !casting && !attacking)
            {
                Vector3 mousePos = playerCam.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D rayHit = Physics2D.Raycast(new Vector2(mousePos.x, mousePos.y), Vector2.zero);
                // Do nothing if UI is clicked
                if (rayHit.collider == null || rayHit.collider.gameObject.layer != 5)
                {
                    // attempt to move unit
                    if (rayHit && rayHit.collider.GetComponent<Tile>() != null && !confirming)
                    {
                        // checks that tile is with move range
                        HashSet<Tile> tiles = currTile.GetTiles(currTile, currTile.currUnit.GetMoveSpeed(), currTile.currUnit.GetFlying());
                        if (tiles.Contains(rayHit.collider.GetComponent<Tile>()) && !rayHit.collider.GetComponent<Tile>().occupied)
                        {
                            // prompts user for confirmation before finishing move
                            currUnit = currTile.currUnit;
                            rayHit.collider.GetComponent<Tile>().PlaceUnit(currUnit);
                            currTile.SelectTile();
                            UI.ToggleCharDisplay(false, true, true);
                            prevTile = currTile;
                            currTile = rayHit.collider.GetComponent<Tile>();
                            confirming = true;
                            moving = true;
                        }
                    }
                }
            }
            // do not allow selection if ability menu is open
            else if (Input.GetKeyDown(Inputs.select) && playerTurn && !abilityMenuOpen && !confirming)
            {
                Vector3 mousePos = playerCam.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D rayHit = Physics2D.Raycast(new Vector2(mousePos.x, mousePos.y), Vector2.zero);

                // Do nothing if UI is clicked
                if (rayHit.collider == null || rayHit.collider.gameObject.layer != 5)
                {
                    // attack unit if clicked
                    if (attacking && rayHit && rayHit.collider.GetComponent<Tile>() != null && rayHit.collider.GetComponent<Tile>().currUnit != null && !confirming)
                    {
                        tileDistance = Math.Abs(rayHit.collider.GetComponent<Tile>().xPos - currTile.xPos) + Math.Abs(rayHit.collider.GetComponent<Tile>().yPos - currTile.yPos);
                        // locks unit if attack is allowed
                        if (tileDistance <= currTile.currUnit.GetAttackRange() && currTile.currUnit.CanAttack(rayHit.collider.GetComponent<Tile>().currUnit))
                        {
                            currUnit = rayHit.collider.GetComponent<Tile>().currUnit;
                            UI.ToggleCharDisplay(false, true, true);
                            UI.DisplayForecast(rayHit.collider.GetComponent<Tile>().currUnit, currTile.currUnit.GetStats()[4], currTile.currUnit.GetStats()[5]);
                            confirming = true;
                        }
                    }

                    // change selected tile
                    else if (rayHit && rayHit.collider.GetComponent<Tile>() != null && !attacking)
                    {
                        if (currTile != null)
                        {
                            currTile.SelectTile();
                            currTile = null;
                        }
                        currTile = rayHit.collider.GetComponent<Tile>();
                        rayHit.collider.GetComponent<Tile>().SelectTile();
                    }

                    // deselect tile
                    else if (currTile != null && !attacking)
                    {
                        currTile.SelectTile();
                        currTile = null;
                    }
                }
            }
            else if (Input.GetKeyDown(Inputs.select) && playerTurn && casting)
            {
                // Display ability
                Vector3 mousePos = playerCam.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D rayHit = Physics2D.Raycast(new Vector2(mousePos.x, mousePos.y), Vector2.zero);

                if (rayHit && rayHit.collider.GetComponent<Tile>() != null)
                {
                    tileDistance = Math.Abs(rayHit.collider.GetComponent<Tile>().xPos - currTile.xPos) + Math.Abs(rayHit.collider.GetComponent<Tile>().yPos - currTile.yPos);
                    targetedTile = rayHit.collider.GetComponent<Tile>();
                    if (tileDistance <= abilityRange)
                    {
                        currTile.HighlightTiles(2, abilityRange);
                        targetedTile.HighlightTiles(3, abilityRadius);
                        targetedTiles = rayHit.collider.GetComponent<Tile>().CollectTiles(abilityRadius);
                        confirming = true;
                        UI.ToggleCharDisplay(false, true, true);
                        UI.DisplayForecast(targetedTile.currUnit, currTile.currUnit.GetAbility(abilityIndex, usingItems), currTile.currUnit);
                    }
                }
            }
        }

        // move camera
        if (Input.GetKeyDown(Inputs.cameraUp))
            cameraDirection = new Vector3(cameraDirection.x, cameraDirection.y + 0.2f, 0);
        if (Input.GetKeyDown(Inputs.cameraDown))
            cameraDirection = new Vector3(cameraDirection.x, cameraDirection.y - 0.2f, 0);
        if (Input.GetKeyDown(Inputs.cameraLeft))
            cameraDirection = new Vector3(cameraDirection.x - 0.2f, cameraDirection.y, 0);
        if (Input.GetKeyDown(Inputs.cameraRight))
            cameraDirection = new Vector3(cameraDirection.x + 0.2f, cameraDirection.y, 0);
        if (cameraDirection != new Vector3(0, 0, 0) && move == null)
        {
            move = MoveCamera();
            StartCoroutine(move);
        }

        if (Input.GetKeyUp(Inputs.cameraUp))
            cameraDirection = new Vector3(cameraDirection.x, cameraDirection.y - 0.2f, 0);
        if (Input.GetKeyUp(Inputs.cameraDown))
            cameraDirection = new Vector3(cameraDirection.x, cameraDirection.y + 0.2f, 0);
        if (Input.GetKeyUp(Inputs.cameraLeft))
            cameraDirection = new Vector3(cameraDirection.x + 0.2f, cameraDirection.y, 0);
        if (Input.GetKeyUp(Inputs.cameraRight))
            cameraDirection = new Vector3(cameraDirection.x - 0.2f, cameraDirection.y, 0);
        if (!Input.GetKey(Inputs.cameraUp) && !Input.GetKey(Inputs.cameraDown) && !Input.GetKey(Inputs.cameraLeft) && !Input.GetKey(Inputs.cameraRight) && move != null)
        {
            cameraDirection = new Vector3(0, 0, 0);
            StopCoroutine(move);
            move = null;
        }

        if (Input.mouseScrollDelta.y != 0 && scroll == null)
        {
            scroll = ZoomCamera();
            StartCoroutine(scroll);
        }
        if (Input.mouseScrollDelta.y == 0 && scroll != null)
        {
            StopCoroutine(scroll);
            scroll = null;
        }
    }

    IEnumerator MoveCamera()
    {
        while (true)
        {
            playerCam.transform.position += cameraDirection;
            // Camera bounds control
            if (playerCam.transform.position.x > 20)
                playerCam.transform.position = new Vector3(20, playerCam.transform.position.y, -10);
            else if (playerCam.transform.position.x < -10)
                playerCam.transform.position = new Vector3(-10, playerCam.transform.position.y, -10);
            if (playerCam.transform.position.y > 20)
                playerCam.transform.position = new Vector3(playerCam.transform.position.x, 20, -10);
            if (playerCam.transform.position.y < -10)
                playerCam.transform.position = new Vector3(playerCam.transform.position.x, -10, -10);
            yield return new WaitForSeconds(0.02f);
        }
    }

    IEnumerator ZoomCamera()
    {
        while (true)
        {
            if (!(playerCam.orthographicSize < 2 && Input.mouseScrollDelta.y > 0) && !(playerCam.orthographicSize > 10 && Input.mouseScrollDelta.y < 0))
                playerCam.orthographicSize -= Input.mouseScrollDelta.y * 0.8f;
            if (playerCam.orthographicSize < 2)
                playerCam.orthographicSize = 2;
            if (playerCam.orthographicSize > 10)
                playerCam.orthographicSize = 10;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void Attacking()
    {
        if (attacking)
        {
            currTile.HighlightTiles(2);
            UI.LockAction(attacking, 0);
            if (!currTile.currUnit.moved)
                currTile.HighlightTiles(1);
        }
        else
        {
            if (!currTile.currUnit.moved)
                currTile.HighlightTiles(1);
            UI.LockAction(attacking, 0);
            currTile.HighlightTiles(2);
        }
        attacking = !attacking;
    }
    // called by ability button
    public void Casting(int newIndex, int newRange, int newRadius, bool items)
    {
        abilityIndex = newIndex;
        abilityRange = newRange;
        abilityRadius = newRadius;
        usingItems = items;

        casting = true;
        currTile.HighlightTiles(2, abilityRange);

        UI.ToggleCharDisplay(false, true);
    }
    public void AbilityClick()
    {
        OpenAbilityMenu(false);
    }
    public void ItemClick()
    {
        OpenAbilityMenu(true);
    }
    public void OpenAbilityMenu(bool items)
    {
        int buttonIndex;
        if (items)
            buttonIndex = 2;
        else
            buttonIndex = 1;

        if (abilityMenuOpen)
        {
            UI.AbilityClick(currTile.currUnit, currTile, items);
            UI.LockAction(abilityMenuOpen, buttonIndex);
            if (currTile.currUnit != null && !currTile.currUnit.moved)
                currTile.HighlightTiles(1);
        }
        else
        {
            if (!currTile.currUnit.moved)
                currTile.HighlightTiles(1);
            UI.LockAction(abilityMenuOpen, buttonIndex);
            UI.AbilityClick(currTile.currUnit, currTile, items);
        }
        abilityMenuOpen = !abilityMenuOpen;
    }

    public bool IsPlayerTurn()
    {
        return playerTurn;
    }
    public void EndTurn()
    {
        if (attacking)
            currTile.HighlightTiles(2);
        if (currTile != null)
            currTile.SelectTile();
        currTile = null;
        moving = false;
        attacking = false;
        playerTurn = false;
        UI.DisplayMenu(true);
        UI.LockMenu(true);
        enemy.StartTurn();
    }
    public void StartTurn()
    {
        playerTurn = true;
        UI.LockMenu(false);
        foreach (CharacterStats unit in units)
        {
            unit.NewTurn();
        }
    }
    // Called when unit dies to check if player has lost
    public void ScanTeam()
    {
        bool allDead = true;
        foreach (CharacterStats unit in units)
        {
            if (!unit.dead)
                allDead = false;
        }
        if (allDead)
        {
            playerLost = true;
            UI.Lose();
        }
    }

    // used by confirm
    public void Confirm()
    {
        if (moving)
        {
            prevTile.ClearUnit(false);
            currTile.PlaceUnit(currTile.currUnit);
            currUnit.moved = true;
            moving = false;
            UI.ToggleCharDisplay(true);
            currTile.SelectTile();
            confirming = false;
            UI.UnlockMenu();
        }
        else if (attacking)
        {
            currTile.currUnit.Attack(currUnit, actionDisplay);
            // Activate display
            StartCoroutine(actionDisplay.StartAttackDisplay());
            attacking = false;
            currTile.HighlightTiles(2);
            currTile.SelectTile(false);
            currTile = null;
            UI.ToggleCharDisplay(false);
            confirming = false;
            UI.UnlockMenu();
            UI.HideForecast();
        }
        else
        {
            casting = false;
            targetedTile.HighlightTiles(3, abilityRadius);
            confirming = false;
            OpenAbilityMenu(usingItems);
            currTile.SelectTile();
            currTile.currUnit.UseAbility(abilityIndex, targetedTiles, targetedTile, usingItems, actionDisplay);
            // Activate display
            StartCoroutine(actionDisplay.StartAttackDisplay());
            currTile = null;
            UI.HideForecast();
        }
    }
    // used by cancel
    public void Cancel()
    {
        if (moving)
        {
            // revert to move selection
            confirming = false;
            moving = false;
            UI.ToggleCharDisplay(true);
            currUnit = currTile.currUnit;
            currTile.ClearUnit(false);
            prevTile.PlaceUnit(currUnit);
            prevTile.SelectTile();
            currTile = prevTile;
        }
        else if (attacking)
        {
            UI.ToggleCharDisplay(true);
            confirming = false;
            UI.HideForecast();
        }
        else if (casting && !confirming)
        {
            UI.ToggleCharDisplay(true);
            currTile.HighlightTiles(2, abilityRange);
            casting = false;
            UI.HideForecast();
        }
        else if (confirming)
        {
            confirming = false;
            targetedTile.HighlightTiles(3, abilityRadius);
            currTile.HighlightTiles(2, abilityRange);
            UI.ToggleCharDisplay(false, true, false);
            UI.HideForecast();
        }
    }
}
