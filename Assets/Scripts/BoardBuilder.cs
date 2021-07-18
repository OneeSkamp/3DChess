using UnityEngine;

public class BoardBuilder : MonoBehaviour {
    [SerializeField] private Material black;
    [SerializeField] private Material white;
    [SerializeField] private GameObject blackRook;
    [SerializeField] private GameObject blackKnight;
    [SerializeField] private GameObject blackBishop;
    [SerializeField] private GameObject blackKing;
    [SerializeField] private GameObject blackQueen;
    [SerializeField] private GameObject blackPawn;

    [SerializeField] private GameObject whiteRook;
    [SerializeField] private GameObject whiteKnight;
    [SerializeField] private GameObject whiteBishop;
    [SerializeField] private GameObject whiteKing;
    [SerializeField] private GameObject whiteQueen;
    [SerializeField] private GameObject whitePawn;

    private GameObject[,] initialFigurePlacement;

    [SerializeField] GameObject cell;

    public GameObject[,] gridArray { get; private set; }


    private void Start() {
        GenerationInitialFigurePlacement();
        CreateGrid();
    }

    private void CreateGrid() {
        gridArray = new GameObject[12, 12];

        for (int x = 2; x < gridArray.GetLength(0) - 2; x++) {
            int counter = x;

            for (int y = 2; y < gridArray.GetLength(1) - 2; y++) {

                cell = Instantiate(cell);

                if (counter % 2 == 0) {                    
                    cell.GetComponent<Cell>().baceColor = "Black";
                    cell.GetComponent<Renderer>().material = black;

                } else {
                    cell.GetComponent<Cell>().baceColor = "White";
                    cell.GetComponent<Renderer>().material = white;
                }

                gridArray[x, y] = cell;

                cell.transform.position = new Vector3(x, 0.0f, y);
                cell.GetComponent<Cell>().xPos = x;
                cell.GetComponent<Cell>().yPos = y;
                cell.GetComponent<Cell>().figure = null;
                if (initialFigurePlacement[x-2, y-2] != null)
                {
                    GameObject figure = Instantiate(initialFigurePlacement[x-2, y-2]);

                    cell.GetComponent<Cell>().figure = figure;
                    figure.transform.position = new Vector3(cell.transform.position.x, 0.5f, cell.transform.position.z);
                    figure.GetComponent<Figure>().step = 1;

                    Debug.Log("1");
                }
                counter++;
            }
        }
    }

    private void GenerationInitialFigurePlacement() {
        initialFigurePlacement = new GameObject[8, 8];
        initialFigurePlacement[0, 0] = blackRook;
        initialFigurePlacement[0, 7] = blackRook;

        initialFigurePlacement[0, 1] = blackKnight;
        initialFigurePlacement[0, 6] = blackKnight;

        initialFigurePlacement[0, 2] = blackBishop;
        initialFigurePlacement[0, 5] = blackBishop;

        initialFigurePlacement[0, 3] = blackKing;
        initialFigurePlacement[0, 4] = blackQueen;

        for (int x = 0; x <= 7; x++) {
            initialFigurePlacement[1, x] = blackPawn;
        }

        initialFigurePlacement[7, 0] = whiteRook;
        initialFigurePlacement[7, 7] = whiteRook;

        initialFigurePlacement[7, 1] = whiteKnight;
        initialFigurePlacement[7, 6] = whiteKnight;

        initialFigurePlacement[7, 2] = whiteBishop;
        initialFigurePlacement[7, 5] = whiteBishop;

        initialFigurePlacement[7, 3] = whiteKing;
        initialFigurePlacement[7, 4] = whiteQueen;

        for (int x = 0; x <= 7; x++) {
            initialFigurePlacement[6, x] = whitePawn;
        }
    }
}
