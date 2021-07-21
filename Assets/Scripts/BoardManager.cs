using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour {
    [SerializeField] private GameObject blackRook;
    [SerializeField] private GameObject blackKnight;
    [SerializeField] private GameObject blackBishop;
    [SerializeField] private GameObject blackQueen;

    [SerializeField] private GameObject whiteRook;
    [SerializeField] private GameObject whiteKnight;
    [SerializeField] private GameObject whiteBishop;
    [SerializeField] private GameObject whiteQueen;

    [SerializeField] private Canvas canvasForChangeFigure;
    [SerializeField] private GameObject board;
    [SerializeField] private Material yelow;
    [SerializeField] private Material black;
    [SerializeField] private Material white;
    [SerializeField] private Material blue;
    [SerializeField] private Material red;
    public Button changePawnToQueen;
    public Button changePawnToKnight;
    public Button changePawnToBishop;
    public Button changePawnToRook;

    private GameObject activeCell;
    private GameObject changeCell;
    private GameObject newActiveCell;
    private GameObject checkCell;
    private GameObject attackedCell;
    private GameObject king;

    private GameObject queenForChange;
    private GameObject knightForChange;
    private GameObject bishopForChange;
    private GameObject rookForChange;
    
    private GameObject currentFigure;
    private ChessControl chessAction;
    
    private bool pawnAttack = false;
    private GameObject[,] gridArray;
    private bool [,] kingIsAttackedMap = new bool [8,8];
    private bool [,] defenceKingMap = new bool [8,8];
    private Vector3 mousePosition = new Vector3();

    private int helpPosX;
    private int helpPosY;

    private bool whiteMove = true;

    private void Awake() {
        chessAction = new ChessControl();
        chessAction.Mouse.Click.performed += ctx => OnClick();
        chessAction.Mouse.Click.performed += ctx => Render();
    }

    private void Update() {
        //Debug.Log($"{helpPosX} {helpPosY}");
    }

    private void Start()
    {
        gridArray = board.GetComponent<BoardBuilder>().gridArray;

		changePawnToQueen.onClick.AddListener(() => ChangeFigure(changeCell, queenForChange));
        changePawnToKnight.onClick.AddListener(() => ChangeFigure(changeCell, knightForChange));
        changePawnToRook.onClick.AddListener(() => ChangeFigure(changeCell, rookForChange));
        changePawnToBishop.onClick.AddListener(() => ChangeFigure(changeCell, bishopForChange));
    }
    private void OnEnable() {
        chessAction.Enable();    
    }

    private void OnDisable() {    
        chessAction.Disable();    
    }

    private void OnClick() {
        
        mousePosition = chessAction.Mouse.MousePosition.ReadValue<Vector2>();

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hitInfo;


        if (Physics.Raycast(ray, out hitInfo)) {

            if (activeCell == null) {
                activeCell = hitInfo.collider.gameObject;
                activeCell.GetComponent<Cell>().active = true;
                if (activeCell.GetComponent<Cell>().figure != null) {
                    GetFigureType(activeCell.GetComponent<Cell>().figure.GetComponent<Figure>().type, whiteMove);
                }
            }
            else if (activeCell != null && hitInfo.collider.gameObject.GetComponent<Cell>().canMove) {
                CleaningCanMoveCells();
                 
                MoveFigure(hitInfo.collider.gameObject);
                activeCell.GetComponent<Cell>().active = false;
                GetKingIsAttackedMap(kingIsAttackedMap);
                //GetDefenceKingMap();
                activeCell = null;                        
                    
            }
            else if (activeCell != null && hitInfo.collider.gameObject.GetComponent<Cell>().figure != null) {
                activeCell.GetComponent<Cell>().active = false;
                activeCell = hitInfo.collider.gameObject;
                activeCell.GetComponent<Cell>().active = true;

                if (activeCell.GetComponent<Cell>().figure != null) {
                    GetFigureType(activeCell.GetComponent<Cell>().figure.GetComponent<Figure>().type, whiteMove);
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

    private void GetFigureType(string figureType, bool color) {
        
        CleaningCanMoveCells();
        if (color) {
            switch (figureType)
            {              
                case "WPawn":
                    GetMovePawnMap(-1);
                    break;
                case "WKnight":
                    GetMoveKnightMap();
                    break;
                case "WRook":
                    GetMoveRookMap();
                    break;
                case "WBishop":
                    GetMoveBishopMap();
                    break;
                case "WQueen":
                    GetMoveBishopMap();
                    GetMoveRookMap();
                    break;
                case "WKing":
                    GetMoveKingMap();
                    break;
            }
        }
        else {
            switch (figureType) {
                case "BPawn":
                    GetMovePawnMap(1);
                    break;
                case "BKnight":
                    GetMoveKnightMap();
                    break;
                case "BRook":
                    GetMoveRookMap();
                    break;
                case "BBishop":
                    GetMoveBishopMap();
                    break;
                case "BQueen":
                    GetMoveBishopMap();
                    GetMoveRookMap();
                    break;
                case "BKing":
                        GetMoveKingMap();
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
                    if (cell.GetComponent<Cell>().check) {
                        cell.GetComponent<Renderer>().material = red;
                    }
                }
            }
        } 
    }

    private void MoveFigure(GameObject cellForMove) {
        if(cellForMove.GetComponent<Cell>().figure != null){
            Destroy(cellForMove.GetComponent<Cell>().figure); 
        }
        CleaningCheckedCells();
        cellForMove.GetComponent<Cell>().figure = activeCell.GetComponent<Cell>().figure;
        activeCell.GetComponent<Cell>().figure = null;
        cellForMove.GetComponent<Cell>().figure.GetComponent<Figure>().step++;
        cellForMove.GetComponent<Cell>().figure.transform.position = new Vector3(cellForMove.transform.position.x, 0.5f, cellForMove.transform.position.z);
        //helpPosX = cellForMove.GetComponent<Cell>().xPos;
        //helpPosX = cellForMove.GetComponent<Cell>().yPos;
        whiteMove = !whiteMove;
        CleaningKingIsAttackedMap(kingIsAttackedMap);
    }

    private void GetMovePawnMap(int i) {
        
        int step = activeCell.GetComponent<Cell>().figure.GetComponent<Figure>().step;           
        int posX = activeCell.GetComponent<Cell>().xPos;
        int posY = activeCell.GetComponent<Cell>().yPos;

        //Debug.LogDebug.Log($"{posX + i} {posY}");
        if (!pawnAttack) {
            if (gridArray[posX + i, posY] != null && gridArray[posX + i, posY].GetComponent<Cell>().figure == null) {                   
                gridArray[posX + i, posY].GetComponent<Cell>().canMove = true;
            }

            if (step == 1) {
                if (gridArray[posX + 2 * i, posY] != null && gridArray[posX + 2 * i, posY].GetComponent<Cell>().figure == null) {
                        gridArray[posX + 2 * i, posY].GetComponent<Cell>().canMove = true;
                }
            } 
        } else {
            if (gridArray[posX + i, posY + 1] != null) {

                gridArray[posX + i, posY + 1].GetComponent<Cell>().canMove = true;
            }

            if (gridArray[posX + i, posY - 1] != null) {

                gridArray[posX + i, posY - 1].GetComponent<Cell>().canMove = true;
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

        if (i == -1 && posX == 2) {

            canvasForChangeFigure.GetComponent<Canvas>().enabled = true;

            Destroy(activeCell.GetComponent<Cell>().figure);
            activeCell.GetComponent<Cell>().figure = null;
            changeCell = gridArray[posX, posY];
            queenForChange = whiteQueen;
            rookForChange = whiteRook;
            bishopForChange = whiteBishop;
            knightForChange = whiteKnight; 
        }

        if (i == 1 && posX == 9) {

            canvasForChangeFigure.GetComponent<Canvas>().enabled = true;

            Destroy(activeCell.GetComponent<Cell>().figure);
            activeCell.GetComponent<Cell>().figure = null;
            changeCell = gridArray[posX, posY];
            queenForChange = blackQueen;
            rookForChange = blackRook;
            bishopForChange = blackBishop;
            knightForChange = blackKnight; 
        }
    }

    public void ChangeFigure(GameObject cell, GameObject newFigure) {
        GameObject figure = Instantiate(newFigure);
        cell.GetComponent<Cell>().figure = figure;
        figure.transform.position = new Vector3 (cell.transform.position.x, 0.5f, cell.transform.position.z);
        Debug.Log("Changefigure");
        canvasForChangeFigure.GetComponent<Canvas>().enabled = false;
    }

    private void GetMoveKnightMap() {
        int posX = activeCell.GetComponent<Cell>().xPos;
        int posY = activeCell.GetComponent<Cell>().yPos;

        if (gridArray[posX + 2, posY + 1] != null && gridArray[posX + 2, posY + 1].GetComponent<Cell>().figure == null) {                   
            gridArray[posX + 2, posY + 1].GetComponent<Cell>().canMove = true;
        }

        if (gridArray[posX + 2, posY + 1] != null && gridArray[posX + 2, posY + 1].GetComponent<Cell>().figure != null 
            && gridArray[posX + 2, posY + 1].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
            
            gridArray[posX +  2, posY + 1].GetComponent<Cell>().canMove = true;
        }

        if (gridArray[posX + 2, posY - 1] != null && gridArray[posX + 2, posY - 1].GetComponent<Cell>().figure == null) {                   
            gridArray[posX + 2, posY - 1].GetComponent<Cell>().canMove = true;
        }

        if (gridArray[posX + 2, posY - 1] != null && gridArray[posX + 2, posY - 1].GetComponent<Cell>().figure != null 
            && gridArray[posX + 2, posY - 1].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
            
            gridArray[posX + 2, posY - 1].GetComponent<Cell>().canMove = true;
        }

        if (gridArray[posX - 2, posY + 1] != null && gridArray[posX -2, posY + 1].GetComponent<Cell>().figure == null) {                   
            gridArray[posX - 2, posY + 1].GetComponent<Cell>().canMove = true;
        }

        if (gridArray[posX - 2, posY + 1] != null && gridArray[posX - 2, posY + 1].GetComponent<Cell>().figure != null 
            && gridArray[posX - 2, posY + 1].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
            
            gridArray[posX - 2, posY + 1].GetComponent<Cell>().canMove = true;
        }

        if (gridArray[posX - 2, posY - 1] != null && gridArray[posX - 2, posY - 1].GetComponent<Cell>().figure == null) {                   
            gridArray[posX - 2, posY - 1].GetComponent<Cell>().canMove = true;
        }

        if (gridArray[posX - 2, posY - 1] != null && gridArray[posX - 2, posY - 1].GetComponent<Cell>().figure != null 
            && gridArray[posX - 2, posY - 1].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
            
            gridArray[posX - 2, posY - 1].GetComponent<Cell>().canMove = true;
        }


        if (gridArray[posX + 1, posY + 2] != null && gridArray[posX + 1, posY + 2].GetComponent<Cell>().figure == null) {                   
            gridArray[posX + 1, posY + 2].GetComponent<Cell>().canMove = true;
        }

        if (gridArray[posX + 1, posY + 2] != null && gridArray[posX + 1, posY + 2].GetComponent<Cell>().figure != null 
            && gridArray[posX + 1, posY + 2].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
            
            gridArray[posX + 1, posY + 2].GetComponent<Cell>().canMove = true;
        }

        if (gridArray[posX - 1, posY + 2] != null && gridArray[posX - 1, posY + 2].GetComponent<Cell>().figure == null) {                   
            gridArray[posX - 1, posY + 2].GetComponent<Cell>().canMove = true;
        }

        if (gridArray[posX - 1, posY + 2] != null && gridArray[posX - 1, posY + 2].GetComponent<Cell>().figure != null 
            && gridArray[posX - 1, posY + 2].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
            
            gridArray[posX - 1, posY + 2].GetComponent<Cell>().canMove = true;
        }

        if (gridArray[posX + 1, posY - 2] != null && gridArray[posX + 1, posY - 2].GetComponent<Cell>().figure == null) {                   
            gridArray[posX + 1, posY - 2].GetComponent<Cell>().canMove = true;
        }

        if (gridArray[posX + 1, posY - 2] != null && gridArray[posX + 1, posY - 2].GetComponent<Cell>().figure != null 
            && gridArray[posX + 1, posY - 2].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
            
            gridArray[posX + 1, posY - 2].GetComponent<Cell>().canMove = true;
        }

        if (gridArray[posX - 1, posY - 2] != null && gridArray[posX - 1, posY - 2].GetComponent<Cell>().figure == null) {                   
            gridArray[posX - 1, posY - 2].GetComponent<Cell>().canMove = true;
        }

        if (gridArray[posX - 1, posY - 2] != null && gridArray[posX - 1, posY - 2].GetComponent<Cell>().figure != null 
            && gridArray[posX - 1, posY - 2].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
            
            gridArray[posX - 1, posY - 2].GetComponent<Cell>().canMove = true;
        }
    }

    private void GetMoveRookMap() {
        int step = activeCell.GetComponent<Cell>().figure.GetComponent<Figure>().step;           
        int posX = activeCell.GetComponent<Cell>().xPos;
        int posY = activeCell.GetComponent<Cell>().yPos;

        for (int i = 1; i < gridArray.GetLength(0) - 2 - posX; i++) {
            if (gridArray[posX + i, posY] != null && gridArray[posX + i, posY].GetComponent<Cell>().figure != null 
                && gridArray[posX + i, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color == gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {

                break;    
            } else if (gridArray[posX + i, posY] != null && gridArray[posX + i, posY].GetComponent<Cell>().figure == null && !defenceKingMap[posX + i - 2, posY -2]) {                   
                gridArray[posX + i, posY].GetComponent<Cell>().canMove = true;

            } else if (gridArray[posX + i, posY] != null && gridArray[posX + i, posY].GetComponent<Cell>().figure != null 
                && gridArray[posX + i, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color 
                && !defenceKingMap[posX + i - 2, posY -2]) {
                
                gridArray[posX +  i, posY].GetComponent<Cell>().canMove = true;
                break;        
            }                        
        }

        for (int i = 1; i < gridArray.GetLength(1) - 2 - posY; i++) {
            if (gridArray[posX, posY + i] != null && gridArray[posX, posY + i].GetComponent<Cell>().figure != null 
                && gridArray[posX, posY + i].GetComponent<Cell>().figure.GetComponent<Figure>().color == gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {

                break;    
            } else if (gridArray[posX, posY + i] != null && gridArray[posX, posY + i].GetComponent<Cell>().figure == null && !defenceKingMap[posX - 2, posY + i -2]) {                   
                gridArray[posX, posY + i].GetComponent<Cell>().canMove = true;

            } else if (gridArray[posX, posY + i] != null && gridArray[posX, posY + i].GetComponent<Cell>().figure != null 
                && gridArray[posX, posY + i].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color
                && !defenceKingMap[posX - 2, posY + i -2]) {
                
                gridArray[posX, posY + i].GetComponent<Cell>().canMove = true;
                break;        
            }                        
        }

        for (int i = 1; i < posY; i++) {
            if (gridArray[posX, posY - i] != null && gridArray[posX, posY - i].GetComponent<Cell>().figure != null 
                && gridArray[posX, posY - i].GetComponent<Cell>().figure.GetComponent<Figure>().color == gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {

                break;    
            } else if (gridArray[posX, posY - i] != null && gridArray[posX, posY - i].GetComponent<Cell>().figure == null && !defenceKingMap[posX - 2, posY - i -2]) {                   
                gridArray[posX, posY - i].GetComponent<Cell>().canMove = true;

            } else if (gridArray[posX, posY - i] != null && gridArray[posX, posY - i].GetComponent<Cell>().figure != null 
                && gridArray[posX, posY - i].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color
                && !defenceKingMap[posX - 2, posY - i -2]) {
                
                gridArray[posX, posY - i].GetComponent<Cell>().canMove = true;
                break;        
            }                        
        }

        for (int i = 1; i < posX; i++) {
            if (gridArray[posX - i, posY] != null && gridArray[posX - i, posY].GetComponent<Cell>().figure != null 
                && gridArray[posX - i, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color == gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
                
                break;    
            } else if (gridArray[posX - i, posY] != null && gridArray[posX - i, posY].GetComponent<Cell>().figure == null && !defenceKingMap[posX - 2 - i, posY - 2]) {                   
                gridArray[posX - i, posY].GetComponent<Cell>().canMove = true;

            } else if (gridArray[posX - i, posY] != null && gridArray[posX - i, posY].GetComponent<Cell>().figure != null 
                && gridArray[posX - i, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color
                && !defenceKingMap[posX - 2 - i, posY - 2]) {
                
                gridArray[posX - i, posY].GetComponent<Cell>().canMove = true;
                break;        
            }                        
        }
    }

    private void GetMoveBishopMap() {          
        int posX = activeCell.GetComponent<Cell>().xPos;
        int posY = activeCell.GetComponent<Cell>().yPos;

        for (int i = 1; i < 8; i++) {
            if (gridArray[posX + i, posY + i] == null) {
                 break;   
            }

            if (gridArray[posX + i, posY + i] != null && gridArray[posX + i, posY + i].GetComponent<Cell>().figure != null 
                && gridArray[posX + i, posY + i].GetComponent<Cell>().figure.GetComponent<Figure>().color == gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {

                break;    
            } else if (gridArray[posX + i, posY + i] != null && gridArray[posX + i, posY + i].GetComponent<Cell>().figure == null) {                   
                gridArray[posX + i, posY + i].GetComponent<Cell>().canMove = true;

            } else if (gridArray[posX + i, posY + i] != null && gridArray[posX + i, posY + i].GetComponent<Cell>().figure != null 
                && gridArray[posX + i, posY + i].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
                
                gridArray[posX +  i, posY + i].GetComponent<Cell>().canMove = true;
                break;        
            }                        
        }

        for (int i = 1; i < 8; i++) {
            if (gridArray[posX - i, posY - i] == null) {
                 break;   
            }
            if (gridArray[posX - i, posY - i] != null && gridArray[posX - i, posY - i].GetComponent<Cell>().figure != null 
                && gridArray[posX - i, posY - i].GetComponent<Cell>().figure.GetComponent<Figure>().color == gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {

                break;    
            } else if (gridArray[posX - i, posY - i] != null && gridArray[posX - i, posY - i].GetComponent<Cell>().figure == null) {                   
                gridArray[posX - i, posY - i].GetComponent<Cell>().canMove = true;

            } else if (gridArray[posX - i, posY - i] != null && gridArray[posX - i, posY - i].GetComponent<Cell>().figure != null 
                && gridArray[posX - i, posY - i].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
                
                gridArray[posX -  i, posY - i].GetComponent<Cell>().canMove = true;
                break;        
            }                        
        }

        for (int i = 1; i < 8; i++) {
            if (gridArray[posX + i, posY - i] == null) {
                 break;   
            }
            if (gridArray[posX + i, posY - i] != null && gridArray[posX + i, posY - i].GetComponent<Cell>().figure != null 
                && gridArray[posX + i, posY - i].GetComponent<Cell>().figure.GetComponent<Figure>().color == gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {

                break;    
            } else if (gridArray[posX + i, posY - i] != null && gridArray[posX + i, posY - i].GetComponent<Cell>().figure == null) {                   
                gridArray[posX + i, posY - i].GetComponent<Cell>().canMove = true;

            } else if (gridArray[posX + i, posY - i] != null && gridArray[posX + i, posY - i].GetComponent<Cell>().figure != null 
                && gridArray[posX + i, posY - i].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
                
                gridArray[posX +  i, posY - i].GetComponent<Cell>().canMove = true;
                break;        
            }                        
        }

        for (int i = 1; i < 8; i++) {
            if (gridArray[posX - i, posY + i] == null) {
                 break;   
            }
            if (gridArray[posX - i, posY + i] != null && gridArray[posX - i, posY + i].GetComponent<Cell>().figure != null 
                && gridArray[posX - i, posY + i].GetComponent<Cell>().figure.GetComponent<Figure>().color == gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {

                break;    
            } else if (gridArray[posX - i, posY + i] != null && gridArray[posX - i, posY + i].GetComponent<Cell>().figure == null) {                   
                gridArray[posX - i, posY + i].GetComponent<Cell>().canMove = true;

            } else if (gridArray[posX - i, posY + i] != null && gridArray[posX - i, posY + i].GetComponent<Cell>().figure != null 
                && gridArray[posX - i, posY + i].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
                
                gridArray[posX -  i, posY + i].GetComponent<Cell>().canMove = true;
                break;        
            }                        
        }
    }

    private void GetMoveKingMap() {
        int step = activeCell.GetComponent<Cell>().figure.GetComponent<Figure>().step;  
        int posX = activeCell.GetComponent<Cell>().xPos;
        int posY = activeCell.GetComponent<Cell>().yPos;

        if (gridArray[posX + 1, posY] != null && gridArray[posX + 1, posY ].GetComponent<Cell>().figure != null 
            && gridArray[posX + 1, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color == gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
    
        } else if (gridArray[posX + 1, posY] != null && gridArray[posX + 1, posY].GetComponent<Cell>().figure == null && !kingIsAttackedMap[posX + 1 - 2, posY -2]) {                   
            gridArray[posX + 1, posY].GetComponent<Cell>().canMove = true;

        } else if (gridArray[posX + 1, posY] != null && gridArray[posX + 1, posY].GetComponent<Cell>().figure != null 
            && gridArray[posX + 1, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color
            && !kingIsAttackedMap[posX + 1 - 2, posY -2]) {
                
            gridArray[posX +  1, posY].GetComponent<Cell>().canMove = true;        
        }

        if (gridArray[posX - 1, posY] != null && gridArray[posX - 1, posY].GetComponent<Cell>().figure != null 
            && gridArray[posX - 1, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color == gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
    
        } else if (gridArray[posX - 1, posY] != null && gridArray[posX - 1, posY].GetComponent<Cell>().figure == null && !kingIsAttackedMap[posX - 1 - 2, posY -2]) {                   
            gridArray[posX - 1, posY].GetComponent<Cell>().canMove = true;

        } else if (gridArray[posX - 1, posY] != null && gridArray[posX - 1, posY].GetComponent<Cell>().figure != null 
            && gridArray[posX - 1, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color
            && !kingIsAttackedMap[posX - 1 - 2, posY -2]) {
                
            gridArray[posX - 1, posY].GetComponent<Cell>().canMove = true;        
        }

        if (gridArray[posX, posY - 1] != null && gridArray[posX, posY - 1].GetComponent<Cell>().figure != null 
            && gridArray[posX, posY - 1].GetComponent<Cell>().figure.GetComponent<Figure>().color == gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
    
        } else if (gridArray[posX, posY - 1] != null && gridArray[posX, posY - 1].GetComponent<Cell>().figure == null && !kingIsAttackedMap[posX - 2, posY -2 - 1]) {                   
            gridArray[posX, posY - 1].GetComponent<Cell>().canMove = true;

        } else if (gridArray[posX, posY - 1] != null && gridArray[posX, posY - 1].GetComponent<Cell>().figure != null 
            && gridArray[posX, posY - 1].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color
            && !kingIsAttackedMap[posX - 2, posY -2 - 1]) {
                
            gridArray[posX, posY - 1].GetComponent<Cell>().canMove = true;        
        }

        if (gridArray[posX, posY + 1] != null && gridArray[posX, posY + 1].GetComponent<Cell>().figure != null 
            && gridArray[posX, posY + 1].GetComponent<Cell>().figure.GetComponent<Figure>().color == gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
    
        } else if (gridArray[posX, posY + 1] != null && gridArray[posX, posY + 1].GetComponent<Cell>().figure == null && !kingIsAttackedMap[posX - 2, posY -2 + 1]) {                   
            gridArray[posX, posY + 1].GetComponent<Cell>().canMove = true;

        } else if (gridArray[posX, posY + 1] != null && gridArray[posX, posY + 1].GetComponent<Cell>().figure != null 
            && gridArray[posX, posY + 1].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color
            && !kingIsAttackedMap[posX - 2, posY - 2 + 1]) {
                
            gridArray[posX, posY + 1].GetComponent<Cell>().canMove = true;        
        }       

////////////////////////////

        if (gridArray[posX + 1, posY + 1] != null && gridArray[posX + 1, posY + 1].GetComponent<Cell>().figure != null 
            && gridArray[posX + 1, posY + 1].GetComponent<Cell>().figure.GetComponent<Figure>().color == gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
    
        } else if (gridArray[posX + 1, posY + 1] != null && gridArray[posX + 1, posY + 1].GetComponent<Cell>().figure == null && !kingIsAttackedMap[posX - 2 + 1, posY -2 + 1]) {                   
            gridArray[posX + 1, posY + 1].GetComponent<Cell>().canMove = true;

        } else if (gridArray[posX + 1, posY + 1] != null && gridArray[posX + 1, posY + 1].GetComponent<Cell>().figure != null 
            && gridArray[posX + 1, posY + 1].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color
            && !kingIsAttackedMap[posX - 2 + 1, posY -2 + 1]) {
                
            gridArray[posX +  1, posY + 1].GetComponent<Cell>().canMove = true;        
        }

        if (gridArray[posX - 1, posY - 1] != null && gridArray[posX - 1, posY - 1].GetComponent<Cell>().figure != null 
            && gridArray[posX - 1, posY - 1].GetComponent<Cell>().figure.GetComponent<Figure>().color == gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
    
        } else if (gridArray[posX - 1, posY - 1] != null && gridArray[posX - 1, posY - 1].GetComponent<Cell>().figure == null && !kingIsAttackedMap[posX - 2 - 1, posY -2 - 1]) {                   
            gridArray[posX - 1, posY - 1].GetComponent<Cell>().canMove = true;

        } else if (gridArray[posX - 1, posY - 1] != null && gridArray[posX - 1, posY - 1].GetComponent<Cell>().figure != null 
            && gridArray[posX - 1, posY - 1].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color
            && !kingIsAttackedMap[posX - 2 - 1, posY -2 - 1]) {
                
            gridArray[posX - 1, posY - 1].GetComponent<Cell>().canMove = true;        
        }

        if (gridArray[posX + 1, posY - 1] != null && gridArray[posX + 1, posY - 1].GetComponent<Cell>().figure != null 
            && gridArray[posX + 1, posY - 1].GetComponent<Cell>().figure.GetComponent<Figure>().color == gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
    
        } else if (gridArray[posX + 1, posY - 1] != null && gridArray[posX + 1, posY - 1].GetComponent<Cell>().figure == null && !kingIsAttackedMap[posX - 2 + 1, posY -2 - 1]) {                   
            gridArray[posX + 1, posY - 1].GetComponent<Cell>().canMove = true;

        } else if (gridArray[posX + 1, posY - 1] != null && gridArray[posX + 1, posY - 1].GetComponent<Cell>().figure != null 
            && gridArray[posX + 1, posY - 1].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color
            && !kingIsAttackedMap[posX - 2 + 1, posY -2 - 1]) {
                
            gridArray[posX +  1, posY - 1].GetComponent<Cell>().canMove = true;        
        }

        if (gridArray[posX - 1, posY + 1] != null && gridArray[posX - 1, posY + 1].GetComponent<Cell>().figure != null 
            && gridArray[posX - 1, posY + 1].GetComponent<Cell>().figure.GetComponent<Figure>().color == gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color) {
    
        } else if (gridArray[posX - 1, posY + 1] != null && gridArray[posX - 1, posY + 1].GetComponent<Cell>().figure == null && !kingIsAttackedMap[posX - 2 - 1, posY -2 + 1]) {                   
            gridArray[posX - 1, posY + 1].GetComponent<Cell>().canMove = true;

        } else if (gridArray[posX - 1, posY + 1] != null && gridArray[posX - 1, posY + 1].GetComponent<Cell>().figure != null 
            && gridArray[posX - 1, posY + 1].GetComponent<Cell>().figure.GetComponent<Figure>().color != gridArray[posX, posY].GetComponent<Cell>().figure.GetComponent<Figure>().color
            && !kingIsAttackedMap[posX - 2 - 1, posY -2 + 1]) {
                
            gridArray[posX -  1, posY + 1].GetComponent<Cell>().canMove = true;        
        }
    }

    private void GetKingIsAttackedMap(bool[,] kingIsAttackedMap) {
        
        string color;

        if (whiteMove)
            color = "White";
        else
            color = "Black";

        foreach (GameObject cell in gridArray) {
            if (cell != null && cell.GetComponent<Cell>().figure != null && cell.GetComponent<Cell>().figure.GetComponent<Figure>().color != color) {
                //Debug.Log("+");
                activeCell = cell;
                pawnAttack = true;
                GetFigureType(cell.GetComponent<Cell>().figure.GetComponent<Figure>().type, !whiteMove);
                pawnAttack = false;
                
               foreach (GameObject cell2 in gridArray) {
                    if (cell2 != null && cell2.GetComponent<Cell>().figure != null && cell2.GetComponent<Cell>().figure.GetComponent<Figure>().type == "BKing" && cell2.GetComponent<Cell>().canMove 
                        || cell2 != null && cell2.GetComponent<Cell>().figure != null && cell2.GetComponent<Cell>().figure.GetComponent<Figure>().type == "WKing" && cell2.GetComponent<Cell>().canMove) {
                        attackedCell = cell;
                        Debug.Log(attackedCell.name);
                    }
                } 
                
                for(int i = 0; i < 8; i++){
                    for(int j = 0; j < 8; j++){
                        if (gridArray[i + 2, j + 2].GetComponent<Cell>().canMove == true) {
                            kingIsAttackedMap[i, j] = true;

                            gridArray[i + 2, j + 2].GetComponent<Cell>().canMove = false;
                        }
                    }
                }
                activeCell = null;
            }
        }
        // for(int i = 0; i < 8;i++){
        //     Debug.Log($"{kingIsAttackedMap[i,0]}   {kingIsAttackedMap[i, 1]}   {kingIsAttackedMap[i, 2]}   {kingIsAttackedMap[i, 3]}   {kingIsAttackedMap[i, 4]}   {kingIsAttackedMap[i, 5]}   {kingIsAttackedMap[i, 6]}  {kingIsAttackedMap[i, 7]}");
        // }
        CheckKing(kingIsAttackedMap);
    }

    private void CleaningKingIsAttackedMap(bool [,] kingIsAttackedMap) {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                kingIsAttackedMap[i, j] = false;
            }
        }
    }

    private void CleaningCheckedCells() {
        foreach (GameObject cell in gridArray) {
            if (cell != null && cell.GetComponent<Cell>().figure != null) {
                cell.GetComponent<Cell>().check = false;
            }
        }
    }

    private bool CheckKing( bool [,] kingIsAttackedMap) {
        if (whiteMove) {
            
            foreach (GameObject cell in gridArray) {
                if (cell != null && cell.GetComponent<Cell>().figure != null && cell.GetComponent<Cell>().figure.GetComponent<Figure>().type == "WKing") {
                    
                    int posX = cell.GetComponent<Cell>().xPos;
                    int posY = cell.GetComponent<Cell>().yPos;
                    Debug.Log("whiteMove");


                    if (kingIsAttackedMap[posX - 2, posY - 2] == true) {
                        
                        Debug.Log("Wcheck");
                        cell.GetComponent<Cell>().check = true;
                        checkCell = cell;            
                        cell.GetComponent<Cell>().figure.GetComponent<King>().checkForKing = true;
                        Debug.Log(cell.GetComponent<Cell>().figure.GetComponent<King>().checkForKing);
                        return true;
                    }  
                }
            }
            return false;
        } else { 
            
            foreach (GameObject cell in gridArray) {
                if (cell != null && cell.GetComponent<Cell>().figure != null && cell.GetComponent<Cell>().figure.GetComponent<Figure>().type == "BKing") {
                    
                    int posX = cell.GetComponent<Cell>().xPos;
                    int posY = cell.GetComponent<Cell>().yPos;
                    Debug.Log("blackMove");

                    if (kingIsAttackedMap[posX - 2, posY - 2] == true) {
                        
                        Debug.Log("Bcheck");
                        cell.GetComponent<Cell>().check = true;
                        checkCell = cell;
                        cell.GetComponent<Cell>().figure.GetComponent<King>().checkForKing = true;
                        Debug.Log(cell.GetComponent<Cell>().figure.GetComponent<King>().checkForKing);
                        
                        return true;
                    }
                }

            }
            return false;
        }   
    }
}
