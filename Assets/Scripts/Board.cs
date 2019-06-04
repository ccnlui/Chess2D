using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.WatchOutRitoGames.Chess2D {

    // Board: Hold and display a 2D array of cell. 
    // .. think of it as a view/controller class since it does not store game state
    public class Board : MonoBehaviour {

        ////////////////////////////////////////////
        //
        #region Enums
        //
        ////////////////////////////////////////////

        public enum Direction {

            None, NW, N, NE, E, SE, S, SW, W,
             
        };

        #endregion

        ////////////////////////////////
        //
        #region Static Varaibles
        //
        ////////////////////////////////
        
        static int dimension = 8;
        static Color32 tileColor1 = new Color32(186, 178, 151, 255);
        static Color32 tileColor2 = new Color32(230, 220, 187, 255);

        static Color32 highlightColor1 = new Color32(79, 123, 159, 191); // 25% transparent
        static Color32 highlightColor2 = new Color32(210, 94, 63, 191); // 25% transparent

        static Color32 highlightColor3 = new Color32(79, 123, 159, 64); // 75% transparent
        static Color32 highlightColor4 = new Color32(210, 94, 63, 64); //75% transparent

        #endregion

        ////////////////////////////////
        //
        #region Private Fields
        //
        ////////////////////////////////

        [Tooltip("Prefab to instantiate Cell")]
        [SerializeField]
        GameObject mCellPrefab;

        Cell[,] mAllCells = new Cell[Board.dimension, Board.dimension];

        List<Cell> highlightedCells = new List<Cell>();

        #endregion

        ////////////////////////////////
        //
        #region Public Static Methods
        //
        ////////////////////////////////

        public static bool IsOutOfBound(Vector2Int position) {

            if (position.x >= 0 && position.x < dimension && 
                  position.y >= 0 && position.y < dimension) {
                return false;
            }
            else {
                return true;
            }

        }

        #endregion

        ////////////////////////////////
        //
        #region Public Methods
        //
        ////////////////////////////////

        // Setup(): Create board, called by GameManager
        public void Setup() {

            Debug.Log("Board::Setup()");
            for (int y = 0; y < dimension; ++y) {

                for (int x = 0; x < dimension; ++x) {

                    // Instantiate cells
                    GameObject newCell = Instantiate(mCellPrefab, transform);

                    // Position
                    RectTransform recTransform = newCell.GetComponent<RectTransform>();
                    recTransform.anchoredPosition = new Vector2(100*x + 50, 100*y + 50);

                    // Color
                    if ((x + y) % 2 == 0) {
                        newCell.GetComponent<Image>().color = tileColor1;
                    }
                    else {
                        newCell.GetComponent<Image>().color = tileColor2;
                    }

                    // Setup
                    mAllCells[x, y] = newCell.GetComponent<Cell>();
                    mAllCells[x, y].Setup(new Vector2Int(x, y)); // store board coordinates

                }

            }
            
        }

        public Cell[,] GetAllCells() {
            return mAllCells;
        }

        public List<Cell> GetHighlightedCells() {
            return highlightedCells;
        }

        // HighlightCell(): given a Vector2Int, highlight the corresponding cell
        public void HighlightCell(Vector2Int position, Color teamColor, bool isSaturated) {

            Debug.Log("Board::HighlightCelll()");

            if (isSaturated) {
                if (teamColor == Color.white) {
                    mAllCells[position.x, position.y].Highlight(Board.highlightColor1);
                }
                else {
                    mAllCells[position.x, position.y].Highlight(Board.highlightColor2);
                }
            }
            else {
                if (teamColor == Color.white) {
                    mAllCells[position.x, position.y].Highlight(Board.highlightColor3);
                }
                else {
                    mAllCells[position.x, position.y].Highlight(Board.highlightColor4);
                }
            }

            highlightedCells.Add(mAllCells[position.x, position.y]);

        }

        // HighlightCells(): given list of Vector2Int, highlight corresponding cells
        public void HighlightCells(List<Vector2Int> positions, Color teamColor, bool isSaturated) {

            foreach (Vector2Int pos in positions) {

                HighlightCell(pos, teamColor, isSaturated);
                highlightedCells.Add(mAllCells[pos.x, pos.y]);

            }

        }

        // UnHighlightCells(): clear list of highlighted cells and unhighlight all of them
        public void UnHighlightCells() {

            foreach (Cell cell in highlightedCells) {
                cell.UnHighlight();
            }

            highlightedCells.Clear();

        }

        // FindContainingCell(): given a position (Vector2), return the Cell that contains it
        // .. TODO: this is gross, not very inefficient!
        public Cell FindContainingCell(Vector2 mousePosition) {

            for (int r = 0; r < Board.dimension; ++r) {

                for (int c = 0; c < Board.dimension; ++c) {

                    Cell currentCell = mAllCells[r, c];
                    if (RectTransformUtility.RectangleContainsScreenPoint(currentCell.GetComponent<RectTransform>(), mousePosition)) {
                        return currentCell;
                    }

                }

            }

            return null;

        }

        // IsCellEmpty(): given a position (Vector2Int), return true if target cell is empty
        public bool IsCellEmpty(Vector2Int position) {

            Cell targetCell = mAllCells[position.x, position.y];
            return targetCell.IsEmpty();

        }

        // IsTargetCellValid(): given a base piece and a position (Vector2Int), return true if target cell is a valid move for piece
        public bool IsTargetCellValid(BasePiece piece, Vector2Int position) {

            Cell targetCell = mAllCells[position.x, position.y];
            Cell.CellState targetCellState = Cell.InquireCellState(piece, targetCell);

            return ( targetCellState == Cell.CellState.Empty || targetCellState == Cell.CellState.Enemy);

        }

        // IsEnemyInCell(): given a position (Vector2Int) and a piece, return true if cell contains a enemy piece
        public bool IsEnemyInCell(BasePiece piece, Vector2Int position) {

            Cell targetCell = mAllCells[position.x, position.y];
            Cell.CellState targetCellState = Cell.InquireCellState(piece, targetCell);

            if (targetCellState == Cell.CellState.Enemy) {
                return true;
            }
            else {
                return false;
            }
        }

        // IsEnemyKingInCell(): given a position (Vector2Int) and a piece, return true if cell contains the piece opposing king
        public bool IsEnemyKingInCell(BasePiece piece, Vector2Int position) {

            Cell targetCell = mAllCells[position.x, position.y];
            Cell.CellState targetCellState = Cell.InquireCellState(piece, targetCell);

            if (targetCellState == Cell.CellState.Enemy) {

                BasePiece targetCellPiece = targetCell.GetCurrentPiece();
                if (targetCellPiece is King) {
                    return true;
                }

            }

            return false;

        }

        // PrintAllCurrentPieces():
        public void PrintAllCurrentPieces() {

            for (int y = 0; y < Board.dimension; ++y) {
                for (int x = 0; x < Board.dimension; ++x) {
                    Debug.Log("current piece at (" + x + "," + y + ") is: " + mAllCells[x, y].GetCurrentPiece());
                }
            }

        }

        #endregion

        ////////////////////////////////
        //
        #region MonoBehaviour Methods
        //
        ////////////////////////////////

        void Awake() {
            Debug.Log("Board::Awake()");
        }

        void Start() {
            Debug.Log("Board::Start()");
        }

        void OnDisable() {
            Debug.Log("Board::OnDisable()");
        }

        #endregion

    }

}

