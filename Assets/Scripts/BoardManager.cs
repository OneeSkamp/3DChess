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
    private GameObject moveCell;
    private ChessControl chessAction;
    private GameObject[,] gridArray;
    private Vector3 mousePosition = new Vector3();

    private void Awake() {
        chessAction = new ChessControl();
        chessAction.Mouse.Click.performed += ctx => OnClick();
        chessAction.Mouse.Click.performed += ctx => GetMovePawnMap();
        
        chessAction.Mouse.Click.performed += ctx => MoveFigure();
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

            if (activeCell != null) {
                lastActiveCell = activeCell;
                lastActiveCell.GetComponent<Cell>().active = false;
                activeCell = hitInfo.collider.gameObject;
                activeCell.GetComponent<Cell>().active = true;
                
                Debug.Log(lastActiveCell.name);

                Debug.Log(activeCell.name);

            } else {
                activeCell = hitInfo.collider.gameObject;
                lastActiveCell = activeCell;
                activeCell.GetComponent<Cell>().active = true;
                Debug.Log(activeCell.name);
            }
        }

        Debug.Log(gridArray);       
    }

    private void CleaningBoard() {
        var figures = GameObject.FindGameObjectsWithTag( "Figure" ); 
        for( int i = 0; i < figures.Length; i++ ){
            Destroy( figures[ i ] );  
        }      
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

            if (cell.GetComponent<Cell>().figure != null) {
                
                GameObject figure = Instantiate(cell.GetComponent<Cell>().figure);
                figure.transform.position = new Vector3(cell.transform.position.x, 0.5f, cell.transform.position.z);
            }
        } 
    }

    private void MoveFigure() {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.collider.gameObject.GetComponent<Cell>().canMove)
            {
                Debug.Log("movefigure");
                moveCell = activeCell;
                moveCell.GetComponent<Cell>().figure = lastActiveCell.GetComponent<Cell>().figure;
                lastActiveCell.GetComponent<Cell>().figure = null;
                activeCell.GetComponent<Cell>().active = false;
                activeCell.GetComponent<Cell>().canMove = false;
                activeCell = null;
                lastActiveCell = null;
                moveCell = null;
                CleaningBoard();                
            }
        }
    }

    private void GetMovePawnMap() {
        
        if (lastActiveCell.GetComponent<Cell>().figure != null && lastActiveCell.GetComponent<Cell>().figure.GetComponent<Figure>().type == "BPawn") {
            //moveCell = activeCell;
            GameObject figure = lastActiveCell.GetComponent<Cell>().figure;

            if (figure.GetComponent<Figure>().step == 1 && lastActiveCell != null) {

                int step = figure.GetComponent<Figure>().step;

                int posX = lastActiveCell.GetComponent<Cell>().xPos;
                int posY = lastActiveCell.GetComponent<Cell>().yPos;
                Debug.Log($"{posX + 1} {posY}");
                if (gridArray[posX + 1, posY].GetComponent<Cell>().figure == null)
                {                   
                    gridArray[posX + 1, posY].GetComponent<Cell>().canMove = true;
                }

                if (step == 1)
                {
                    if (gridArray[posX + 2, posY].GetComponent<Cell>().figure == null)
                    {
                        gridArray[posX + 2, posY].GetComponent<Cell>().canMove = true;
                    }
                } 
            }
        }
    }
}
