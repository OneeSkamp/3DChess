using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject blackCell;
    [SerializeField] private GameObject whiteCell;
    [SerializeField] private GameObject whitePawn;
    [SerializeField] private GameObject whiteBishop;
    [SerializeField] private GameObject whiteKing;
    [SerializeField] private GameObject whiteKnight;
    [SerializeField] private GameObject whiteQueen;
    [SerializeField] private GameObject whiteRook;
    [SerializeField] private GameObject blackPawn;
    [SerializeField] private GameObject blackBishop;
    [SerializeField] private GameObject blackKing;
    [SerializeField] private GameObject blackKnight;
    [SerializeField] private GameObject blackQueen;
    [SerializeField] private GameObject blackRook;

    private GameObject [,] cells = new GameObject[8,8];

    private void SetStartPositionFigures(){
        SetFigure(0, 0, blackRook);
        SetFigure(0, 7, blackRook);
        SetFigure(0, 1, blackKnight);
        SetFigure(0, 6, blackKnight);
        SetFigure(0, 2, blackBishop);
        SetFigure(0, 5, blackBishop);
        SetFigure(0, 3, blackRook);
        SetFigure(0, 4, blackRook);
    }

    private void SetFigure(int i, int j, GameObject figure){
        Cell cell = cells[i, j].GetComponent<Cell>();
        cell.figure = figure;
    }

    private void Rerender(){
        for (int i = 0; i < 8; i++){

            for (int j = 0; j < 8; j++){
                
                if (cells[i,j].GetComponent<Cell>().figure != null){
                    GameObject cell = cells[i,j];
                    GameObject figure = cells[i,j].GetComponent<Cell>().figure;           
                    Instantiate(figure);
                    figure.transform.position = new Vector3 (i, 0.5f, j);
                }
            }               
        }
    }

    private void CellCreator(){
        int count = 0;
        for (int i = 0; i < 8; i++){

            for (int j = 0; j < 8; j++){

                if (count %2 == 0){
                    cells[i,j] = blackCell;  
                } else {
                    cells[i,j] = whiteCell;
                }

                count++;                  
            }
            count++;
        }
    }    

    private void BoardCreator(){
        for (int i = 0; i < 8; i++){

            for (int j = 0; j < 8; j++){
                GameObject cell = cells[i,j];
                Instantiate(cell);
                cell.transform.position = new Vector3 (i, 0, j);
            }               
        }
    }
    private void Start() {
        CellCreator();
        BoardCreator();
        SetStartPositionFigures();
        Rerender();
    }
}
