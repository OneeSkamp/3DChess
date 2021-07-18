using UnityEngine;

public class BoardManager : MonoBehaviour {
    [SerializeField] private GameObject board;
    [SerializeField] private Material yelow;
    [SerializeField] private Material black;
    [SerializeField] private Material white;
    [SerializeField] private Material blue;

    private GameObject activeCell;
    private ChessControl chessAction;
    private GameObject[,] gridArray;
    private Vector3 mousePosition = new Vector3();

    private bool whiteMove = true;

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

    private void OnClick() {
        gridArray = board.GetComponent<BoardBuilder>().gridArray;
        mousePosition = chessAction.Mouse.MousePosition.ReadValue<Vector2>();

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hitInfo;


        if (Physics.Raycast(ray, out hitInfo)) {

            if (activeCell == null) {
                activeCell = hitInfo.collider.gameObject;
                activeCell.GetComponent<Cell>().active = true;
                if (activeCell.GetComponent<Cell>().figure != null) {
                    GetFigureType(activeCell.GetComponent<Cell>().figure.GetComponent<Figure>().type);
                }
            }
            else if (activeCell != null && hitInfo.collider.gameObject.GetComponent<Cell>().canMove) {
                CleaningCanMoveCells();
                MoveFigure(hitInfo.collider.gameObject);
                activeCell.GetComponent<Cell>().active = false;
                activeCell = null;
            }
            else if (activeCell != null && hitInfo.collider.gameObject.GetComponent<Cell>().figure != null) {
                activeCell.GetComponent<Cell>().active = false;
                activeCell = hitInfo.collider.gameObject;
                activeCell.GetComponent<Cell>().active = true;

                if (activeCell.GetComponent<Cell>().figure != null) {
                    GetFigureType(activeCell.GetComponent<Cell>().figure.GetComponent<Figure>().type);
                }
            }          
        }     
    }

    private void CleaningCanMoveCells() {

        foreach (GameObject cell in gridArray) {
            if (cell != null) {
                cell.GetComponent<Cell>().canMove = false;   
            }            
        }
    }

    private void GetFigureType(string figureType) {
        
        CleaningCanMoveCells();
        if (whiteMove) {
            switch (figureType)
            {
                case "WPawn":
                    Debug.Log("GetBpawn");
                    GetMovePawnMap(-1);
                    break;
            }
        }
        else {
            switch (figureType) {
                case "BPawn":
                    Debug.Log("GetBpawn");
                    GetMovePawnMap(1);
                    break;
            }
        }
    }

    private void Render() {     
        
        foreach(GameObject cell in gridArray) {
            if (cell != null) {
                if (cell.GetComponent<Cell>().active) {
                    cell.GetComponent<Renderer>().material = yelow;
                } else {
                    if (cell.GetComponent<Cell>().baceColor == "Black") {
                        cell.GetComponent<Renderer>().material = black;     
                    }
                    if (cell.GetComponent<Cell>().baceColor == "White") {
                        cell.GetComponent<Renderer>().material = white;     
                    }
                    if (cell.GetComponent<Cell>().canMove) {
                        cell.GetComponent<Renderer>().material = blue;
                    }
                }
            }
        } 
    }

    private void MoveFigure(GameObject cellForMove) {
        if(cellForMove.GetComponent<Cell>().figure != null){
            Destroy(cellForMove.GetComponent<Cell>().figure); 
        }
        cellForMove.GetComponent<Cell>().figure = activeCell.GetComponent<Cell>().figure;
        activeCell.GetComponent<Cell>().figure = null;
        cellForMove.GetComponent<Cell>().figure.GetComponent<Figure>().step++;
        cellForMove.GetComponent<Cell>().figure.transform.position = new Vector3(cellForMove.transform.position.x, 0.5f, cellForMove.transform.position.z);
        whiteMove = !whiteMove;
    }

    private void GetMovePawnMap(int i) {

        int step = activeCell.GetComponent<Cell>().figure.GetComponent<Figure>().step;           
        int posX = activeCell.GetComponent<Cell>().xPos;
        int posY = activeCell.GetComponent<Cell>().yPos;

        Debug.Log($"{posX + i} {posY}");

        if (gridArray[posX + i, posY] != null && gridArray[posX + i, posY].GetComponent<Cell>().figure == null) {                   
            gridArray[posX + i, posY].GetComponent<Cell>().canMove = true;
        }

        if (step == 1) {
            if (gridArray[posX + 2 * i, posY] != null && gridArray[posX + 2 * i, posY].GetComponent<Cell>().figure == null) {
                    gridArray[posX + 2 * i, posY].GetComponent<Cell>().canMove = true;
            }
        } 

        if (gridArray[posX + i, posY + 1] != null && gridArray[posX + i, posY+1].GetComponent<Cell>().figure != null 
            && gridArray[posX + i, posY + 1].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
            
            gridArray[posX +  i, posY+1].GetComponent<Cell>().canMove = true;
        }

        if (gridArray[posX + i, posY - 1] != null && gridArray[posX + i, posY - 1].GetComponent<Cell>().figure != null 
            && gridArray[posX + i, posY - 1].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
            
            gridArray[posX + i, posY - 1].GetComponent<Cell>().canMove = true;
        }
    }
}
