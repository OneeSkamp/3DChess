using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoardManager : MonoBehaviour {
    [SerializeField] private GameObject board;
    [SerializeField] private Material yelow;
    [SerializeField] private Material black;
    [SerializeField] private Material white;
    private GameObject activeCell;
    private GameObject lastActiveCell;
    private ChessControl chessAction;
    private GameObject[,] gridArray;
    private Vector3 mousePosition = new Vector3();

    private void Awake() {
        chessAction = new ChessControl();
        chessAction.Mouse.Click.performed += ctx => OnClick();
        chessAction.Mouse.Click.performed += ctx => Render();
    }

    private void OnEnable() {
        chessAction.Enable();    
    }

    private void OnDisable() {    
        chessAction.Disable();    
    }

    private void Start(){
        
    }
    private void OnClick() {
        gridArray = board.GetComponent<BoardBuilder>().gridArray;
        mousePosition = chessAction.Mouse.MousePosition.ReadValue<Vector2>();

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo)){

            if (activeCell == null) {
                activeCell = hitInfo.collider.gameObject;
                activeCell.GetComponent<Cell>().active = true;
                Debug.Log(activeCell.name);
            }

            if (activeCell != null) {
                lastActiveCell = activeCell;
                lastActiveCell.GetComponent<Cell>().active = false;
                activeCell = hitInfo.collider.gameObject;
                activeCell.GetComponent<Cell>().active = true;
                
                Debug.Log(lastActiveCell.name);

                Debug.Log(activeCell.name);
            }
        }

        Debug.Log(gridArray);       
    }

    private void Render() {
        foreach(GameObject cell in gridArray) {
            if (cell.GetComponent<Cell>().active) {
                cell.GetComponent<Renderer>().material = yelow;
            } else {
                if (cell.GetComponent<Cell>().baceColor == "Black") {
                    cell.GetComponent<Renderer>().material = black;     
                }
                if (cell.GetComponent<Cell>().baceColor == "White") {
                    cell.GetComponent<Renderer>().material = white;     
                }

            }
        } 
    }
}
