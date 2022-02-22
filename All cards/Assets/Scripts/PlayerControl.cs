using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

using System;

public class PlayerControl : MonoBehaviour
{
    public bool playerLost;

    public Camera playerCam;
    public Inputs inputs;
    Vector3 CameraDirection;
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

    public CharacterStats[] units;
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
        CameraDirection = new Vector3(0, 0, 0);
        currTile = null;
    }

    // Update is called once per frame
    void Update()
    {
        // do not allow selection if ability menu is open
        if (Input.GetKeyDown(inputs.select) && playerTurn && !abilityMenuOpen)
        {
            Vector3 mousePos = playerCam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D rayHit = Physics2D.Raycast(new Vector2(mousePos.x, mousePos.y), Vector2.zero);

            // Do nothing if UI is clicked
            if (rayHit.collider == null || rayHit.collider.gameObject.layer != 5)
            {
                // attempt to move unit
                if (moving && rayHit && rayHit.collider.GetComponent<Tile>() != null && !confirming)
                {
                    // checks that tile is with move range
                    HashSet<Tile> tiles = currTile.FindTilesInRange(currTile, currTile.currUnit.GetMoveSpeed());
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
                    }
                }

                // attack unit if clicked
                else if (attacking && rayHit && rayHit.collider.GetComponent<Tile>() != null && rayHit.collider.GetComponent<Tile>().currUnit != null && !confirming)
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
                else if (rayHit && rayHit.collider.GetComponent<Tile>() != null && !moving && !attacking)
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
                else if (currTile != null && !moving && !attacking)
                {
                    currTile.SelectTile();
                    currTile = null;
                }
            }
        }
        else if (Input.GetKeyDown(inputs.select) && playerTurn && casting && !confirming)
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
                    UI.DisplayForecast(targetedTile.currUnit, currTile.currUnit.GetAbility(abilityIndex, usingItems));
                }
            }
        }

        // move camera
        if (Input.GetKeyDown(inputs.cameraUp))
            CameraDirection = new Vector3(CameraDirection.x, CameraDirection.y + 0.2f, 0);
        if (Input.GetKeyDown(inputs.cameraDown))
            CameraDirection = new Vector3(CameraDirection.x, CameraDirection.y - 0.2f, 0);
        if (Input.GetKeyDown(inputs.cameraLeft))
            CameraDirection = new Vector3(CameraDirection.x - 0.2f, CameraDirection.y, 0);
        if (Input.GetKeyDown(inputs.cameraRight))
            CameraDirection = new Vector3(CameraDirection.x + 0.2f, CameraDirection.y, 0);
        if (CameraDirection != new Vector3(0, 0, 0) && move == null)
        {
            move = MoveCamera();
            StartCoroutine(move);
        }

        if (Input.GetKeyUp(inputs.cameraUp))
            CameraDirection = new Vector3(CameraDirection.x, CameraDirection.y - 0.2f, 0);
        if (Input.GetKeyUp(inputs.cameraDown))
            CameraDirection = new Vector3(CameraDirection.x, CameraDirection.y + 0.2f, 0);
        if (Input.GetKeyUp(inputs.cameraLeft))
            CameraDirection = new Vector3(CameraDirection.x + 0.2f, CameraDirection.y, 0);
        if (Input.GetKeyUp(inputs.cameraRight))
            CameraDirection = new Vector3(CameraDirection.x - 0.2f, CameraDirection.y, 0);
        if (CameraDirection == new Vector3(0, 0, 0) && move != null)
        {
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
            playerCam.transform.position += CameraDirection;
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

    public void Moving()
    {
        UI.LockAction(moving, 3);
        moving = !moving;
    }
    public void Attacking()
    {
        if (attacking)
        {
            currTile.HighlightTiles(2);
            UI.LockAction(attacking, 0);
            if (UI.moveButton.interactable == true)
                currTile.HighlightTiles(1);
        }
        else
        {
            if (UI.moveButton.interactable == true)
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
            if (UI.moveButton.interactable && currTile.currUnit != null)
                currTile.HighlightTiles(1);
        }
        else
        {
            if (UI.moveButton.interactable)
                currTile.HighlightTiles(1);
            UI.LockAction(abilityMenuOpen, buttonIndex);
            UI.AbilityClick(currTile.currUnit, currTile, items);
        }
        abilityMenuOpen = !abilityMenuOpen;
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
            currTile.currUnit.Attack(currUnit);
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
            targetedTile.HighlightTiles(3, abilityRadius);
            confirming = false;
            OpenAbilityMenu(usingItems);
            currTile.SelectTile();
            currTile.currUnit.UseAbility(abilityIndex, targetedTiles, usingItems);
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
            UI.ToggleCharDisplay(true);
            currUnit = currTile.currUnit;
            currTile.ClearUnit(false);
            prevTile.PlaceUnit(currUnit);
            prevTile.SelectTile();
            UI.LockAction(false, 3);
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
