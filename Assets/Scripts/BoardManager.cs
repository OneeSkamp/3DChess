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
            else if (activeCell != null && hitInfo.collider.gameObject.GetComponent<Cell>().figure != null) {
                activeCell.GetComponent<Cell>().active = false;
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
        }     
    }

    private void CleaningCanMoveCells() {

        foreach (GameObject cell in gridArray) {
            cell.GetComponent<Cell>().canMove = false;    
        }
    }

    private void GetFigureType(string figureType) {
        
        CleaningCanMoveCells();
        
        switch (figureType) {
            case "BPawn":
                Debug.Log("GetBpawn");
                GetMovePawnMap();
                break;
        }
    }

    private void CleaningBoard() {
        var figures = GameObject.FindGameObjectsWithTag( "Figure" ); 
        for(int i = 0; i < figures.Length; i++){
            Destroy( figures[i] );  
        }      
    }

    private void Render() {
        
        CleaningBoard();
        
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
                if (cell.GetComponent<Cell>().canMove) {
                    cell.GetComponent<Renderer>().material = blue;
                }
            }

            if (cell.GetComponent<Cell>().figure != null) {               
                GameObject figure = Instantiate(cell.GetComponent<Cell>().figure);
                figure.transform.position = new Vector3(cell.transform.position.x, 0.5f, cell.transform.position.z);
            }
        } 
    }

    private void MoveFigure(GameObject cellForMove) {
        cellForMove.GetComponent<Cell>().figure = activeCell.GetComponent<Cell>().figure;
        activeCell.GetComponent<Cell>().figure = null;
    }

    private void GetMovePawnMap() {

        GameObject figure = activeCell.GetComponent<Cell>().figure;           

        if (figure.GetComponent<Figure>().step == 1 && activeCell != null) {

            int step = figure.GetComponent<Figure>().step;

            int posX = activeCell.GetComponent<Cell>().xPos;
            int posY = activeCell.GetComponent<Cell>().yPos;

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
