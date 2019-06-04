using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Com.WatchOutRitoGames.Chess2D {

    // PieceManager: Manages pieces on the board :o
    // .. Listen for piece events, move them accordingly!
    // .. This should not be a singleton because parent (canvas) is destroyed on scene load
    public class PieceManager : MonoBehaviour {

        //////////////////////////////////////
        //
        #region Singleton
        //
        //////////////////////////////////////

        public static PieceManager Instance = null;

        #endregion

        ///////////////////////////////////////
        //
        #region Static Fields
        //
        ///////////////////////////////////////

        static Dictionary<string, Type> mPieceTypes = new Dictionary<string, Type>() {

            {"P", typeof(Pawn)},
            {"R", typeof(Rook)},
            {"KN", typeof(Knight)},
            {"B", typeof(Bishop)},
            {"K", typeof(King)},
            {"Q", typeof(Queen)}

        };

        static string[] mPieceOrder = new string[16] {

            "P", "P", "P", "P", "P", "P", "P", "P",
            "R", "KN", "B", "Q", "K", "B", "KN", "R"

        };

        static int mPieceTotalNumber = 16;

        #endregion

        ///////////////////////////////////////
        //
        #region Private Fields
        //
        ///////////////////////////////////////

        [Tooltip("Prefab to instantiate pieces")]
        [SerializeField]
        GameObject mPiecePrefab;

        List<BasePiece> mWhitePieces;
        List<BasePiece> mBlackPieces;

        Board mBoard; // a view

        // To implement dragging and moving pieces, maybe should be Vector2Int instead to decouple PieceManager from Board
        Cell targetCell;

        #endregion

        ///////////////////////////////////////
        //
        #region MonoBehaviour Callbacks
        //
        ///////////////////////////////////////

        // Awake(): called once during pre-initialization phase
        void Awake() {
            Debug.Log("PieceManager::Awake()");

            // Singleton pattern
            if (Instance == null) {
                Instance = this;
            }
            else if (Instance != this) {
                // destroy this to prevent more than 1 PieceManager
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);

            // initialize fields
            mWhitePieces = new List<BasePiece>();
            mBlackPieces = new List<BasePiece>();
            targetCell = null;
        }

        // Start(): called once during pre-initialization phase
        void Start() {
            Debug.Log("PieceManager::Start()");
        }

        // OnDisable():
        void OnDisable() {
            Debug.Log("PieceManager::OnDisable()");
        }

        #endregion

        ///////////////////////////////////////
        //
        #region Public Methods
        //
        ///////////////////////////////////////

        // Setup(): setup ALL pieces, called by GameManager
        public void Setup(Board mBoard) {

            this.mBoard = mBoard;

            Debug.Log("PieceManager::Setup()");

            // Create and Place white pieces
            CreatePieces(Color.white);
            PlacePieces(1, 0, mWhitePieces);

            // Create and Place black pieces
            CreatePieces(Color.black);
            PlacePieces(6, 7, mBlackPieces);

        }

        // Reset(): reset all pieces to represent the begining of a game
        public void Reset() {

            Debug.Log("PieceManager::Reset()");

            // unhighlight all cells
            mBoard.UnHighlightCells();

            // need to activate all pieces again
            foreach (BasePiece piece in mWhitePieces) {
                piece.gameObject.SetActive(true);
                if (piece.GetCurrentCell()) {
                    piece.GetCurrentCell().ClearCurrentPiece();
                }
                piece.ClearCurrentCell();

                if (piece is Pawn) {
                    Pawn pawnPiece = (Pawn)piece;
                    pawnPiece.Reset();
                }

            }
            foreach (BasePiece piece in mBlackPieces) {
                piece.gameObject.SetActive(true);
                if (piece.GetCurrentCell()) {
                    piece.GetCurrentCell().ClearCurrentPiece();
                }
                piece.ClearCurrentCell();

                if (piece is Pawn) {
                    Pawn pawnPiece = (Pawn)piece;
                    pawnPiece.Reset();
                }
            }

            // Place them at original positions
            PlacePieces(1, 0, mWhitePieces);
            PlacePieces(6, 7, mBlackPieces);

            // Debug
            mBoard.PrintAllCurrentPieces();
        }

        // DoBeginDrag(): Display feasible target cells  (controller methods)
        public void DoBeginDrag(BasePiece piece, PointerEventData eventData) {

            // Unhighlight king in check
            mBoard.UnHighlightCells();
            
            ShowMoves(piece); // highlight cells too!

        }

        // DoDrag(): Track cells on which mouse is hovering (controller methods)
        public void DoDrag(BasePiece piece, PointerEventData eventData) {

            targetCell = mBoard.FindContainingCell(eventData.position);

        }

        // DoEndDrag(): Move cells to target cells or return to original position!
        public void DoEndDrag(BasePiece piece, PointerEventData eventData) {

            Debug.Log("PieceManager::DoEndDrag()");

            // Unhighlight previous available moves
            mBoard.UnHighlightCells();

            if (targetCell) {
                Move(piece, targetCell);
            }
            else {
                // Return to original cell
                Place(piece, piece.GetCurrentCell());
            }

        }

        // EnableTeamPieces(): Enable scripts to receive events
        public void EnableTeamPieces(Color teamColor) {

            if (teamColor == Color.white) {

                foreach (BasePiece piece in mWhitePieces) {
                    // destroyed gameObject might still exists
                    if (piece) {
                        // Debug.Log(piece + " is NOT null!");
                        piece.enabled = true;
                    }
                    
                }

            }
            else if (teamColor == Color.black) {

                foreach (BasePiece piece in mBlackPieces) {
                    // destroyed gameObject might still exists
                    if (piece) {
                        // Debug.Log(piece + " is NOT null!");
                        piece.enabled = true;
                    }
                }
            }
        }

        // DisableTeamPieces(): Disable scripts to ignore events
        public void DisableTeamPieces(Color teamColor) {

            if (teamColor == Color.white) {

                foreach (BasePiece piece in mWhitePieces) {
                    // destroyed gameObject might still exists
                    if (piece) {
                        // Debug.Log(piece + " is NOT null!");
                        piece.enabled = false;
                    }
                }

            }
            else if (teamColor == Color.black) {

                foreach (BasePiece piece in mBlackPieces) {
                    // destroyed gameObject might still exists
                    if (piece) {
                        // Debug.Log(piece + " is NOT null!");
                        piece.enabled = false;
                    }
                }
            }
        }

        #endregion

        ///////////////////////////////////////
        //
        #region Private Methods
        //
        ///////////////////////////////////////

        // ShowMoves(): given a BasePiece and Position, display available moves
        void ShowMoves(BasePiece piece) {

            Debug.Log("PieceManager::ShowMoves()");

            // highlight original position
            mBoard.HighlightCell(piece.GetCurrentCell().GetBoardPosition(), piece.GetTeamColor(), false);

            // highlight available moves
            List<Vector2Int> allMovePositions = GetAllMovePositions(piece);
            mBoard.HighlightCells(allMovePositions, piece.GetTeamColor(), true);

        }

        // GetAllMovePositions(): given a BasePiece and a Cell, return list of all available move positions
        List<Vector2Int> GetAllMovePositions(BasePiece piece) {

            List<Vector2Int> result = new List<Vector2Int>();

            Cell currentCell = piece.GetCurrentCell();

            for (int i = 0; i < piece.GetMoveNum(); ++i) {

                List<Vector2Int> moveCellPositions = GetMovePositions(piece.GetMoveTypes()[i], piece.GetMoveSteps()[i], 
                                                                        currentCell, piece.GetFrontDirection());

                moveCellPositions.ForEach(position => result.Add(position));

            }

            return result;

        }

        // GetDirectionPositions(): given Direction, current cell, moveStep, return all available positions
        List<Vector2Int> GetDirectionPositions(Board.Direction direction, BasePiece.MoveStep moveStep, Cell currentCell, Board.Direction frontDirection) {

            List<Vector2Int> result = new List<Vector2Int>();
            Vector2Int currentPosition = currentCell.GetBoardPosition();
            BasePiece currentPiece = currentCell.GetCurrentPiece();

            // determine direction to step to
            Vector2Int offset = Vector2Int.zero;

            if (direction == Board.Direction.NW) {
                offset = new Vector2Int(-1, 1);
            }
            else if (direction == Board.Direction.N) {
                offset = Vector2Int.up;
            }
            else if (direction == Board.Direction.NE) {
                offset = new Vector2Int(1, 1);
            }
            else if (direction == Board.Direction.E) {
                offset = Vector2Int.right;
            }
            else if (direction == Board.Direction.SE) {
                offset = new Vector2Int(1, -1);
            }
            else if (direction == Board.Direction.S) {
                offset = Vector2Int.down;
            }
            else if (direction == Board.Direction.SW) {
                offset = new Vector2Int(-1, -1);
            }
            else if (direction == Board.Direction.W) {
                offset = Vector2Int.left;
            }

            // determine front direction adjustment
            Vector2Int frontOffset = Vector2Int.zero;
            if (frontDirection == Board.Direction.N) {
                frontOffset = Vector2Int.one;
            }
            else if (frontDirection == Board.Direction.S) {
                frontOffset = new Vector2Int(1, -1);
            }

            offset = Vector2Int.Scale(offset, frontOffset);

            // determine how many times we are stepping
            int stepNum = 0;

            if (moveStep == BasePiece.MoveStep.Single) {
                stepNum = 1;
            }
            else if (moveStep == BasePiece.MoveStep.Double) {
                stepNum = 2;
            }
            else if (moveStep == BasePiece.MoveStep.Multiple) {
                stepNum = -1;
            }

            while (stepNum != 0) {
                
                Vector2Int nextPosition = currentPosition + offset;
                    if (!Board.IsOutOfBound(nextPosition) && mBoard.IsTargetCellValid(currentPiece, nextPosition)) {
                        result.Add(nextPosition);

                        // cannot advance if path is blocked by another piece
                        if (!mBoard.IsCellEmpty(nextPosition)) {
                            break;
                        }

                    }
                    else {
                        break;
                    }

                    currentPosition = nextPosition;
                    stepNum -= 1;

            }

            return result;

        }

        // GetLPositions(): given current cell, return all available L positions
        List<Vector2Int> GetLPositions(Cell currentCell) {

            Debug.Log("PieceManager::GetLPositions()");

            List<Vector2Int> result = new List<Vector2Int>();
            Vector2Int currentPosition = currentCell.GetBoardPosition();

            Vector2Int offset1 = new Vector2Int(1, 2);
            Vector2Int offset2 = new Vector2Int(2, 1);

            List<Vector2Int> quadAdjustment = new List<Vector2Int>();

            quadAdjustment.Add(new Vector2Int(1, 1));
            quadAdjustment.Add(new Vector2Int(-1, 1));
            quadAdjustment.Add(new Vector2Int(-1, -1));
            quadAdjustment.Add(new Vector2Int(1, -1));

            foreach (Vector2Int vec in quadAdjustment) {

                Vector2Int lPosition1 = currentPosition + Vector2Int.Scale(vec, offset1);
                Vector2Int lPosition2 = currentPosition + Vector2Int.Scale(vec, offset2);

                if (!Board.IsOutOfBound(lPosition1)) {
                    result.Add(lPosition1);
                }

                if (!Board.IsOutOfBound(lPosition2)) {
                    result.Add(lPosition2);
                }

            }

            return result;

        }

        // GetPawnPositions():
        List<Vector2Int> GetPawnPositions(Cell currentCell, Board.Direction frontDirection) {

            Debug.Log("PieceManager::GetPawnPositions");

            List<Vector2Int> result = new List<Vector2Int>();
            Vector2Int originalPosition = currentCell.GetBoardPosition();

            Pawn piece = (Pawn)currentCell.GetCurrentPiece();

            // determine front direction adjustment
            Vector2Int frontOffset = Vector2Int.zero;
            if (frontDirection == Board.Direction.N) {
                frontOffset = Vector2Int.one;
            }
            else if (frontDirection == Board.Direction.S) {
                frontOffset = new Vector2Int(1, -1);
            }

            // get top positions
            Vector2Int step = Vector2Int.up;
            step = Vector2Int.Scale(step, frontOffset);
            Vector2Int currentPosition = originalPosition;

            for (int stepCount = 0; stepCount < 2; ++stepCount) {
                currentPosition = currentPosition + step;

                if (Board.IsOutOfBound(currentPosition) || !mBoard.IsCellEmpty(currentPosition)) {
                    break;
                }
                else {
                    result.Add(currentPosition);
                }

                if (!piece.IsFirstMove()) { // only 1 step forward if not first move
                    break;
                }
            }

            // get NW position
            step = new Vector2Int(-1, 1);
            step = Vector2Int.Scale(step, frontOffset);
            currentPosition = originalPosition;

            currentPosition = currentPosition + step;
            if (!Board.IsOutOfBound(currentPosition) && mBoard.IsEnemyInCell(piece, currentPosition)) {
                result.Add(currentPosition);
            }

            // get NE position
            step = new Vector2Int(1, 1);
            step = Vector2Int.Scale(step, frontOffset);
            currentPosition = originalPosition;

            currentPosition = currentPosition + step;
            if (!Board.IsOutOfBound(currentPosition) && mBoard.IsEnemyInCell(piece, currentPosition)) {
                result.Add(currentPosition);
            }

            return result;

        }

        // GetMovePositions(): given MoveType, MoveStep, currentCell, return list of Cells representing available moves
        // .. return Vector2Int to decouple PieceManager and Board
        // .. assume currentCell is valid 
        List<Vector2Int> GetMovePositions(BasePiece.MoveType moveType, BasePiece.MoveStep moveStep, 
                                            Cell currentCell, Board.Direction frontDirection) {

            Debug.Log("PieceManager::GetMovePositions()");

            List<Vector2Int> result = new List<Vector2Int>();
            Vector2Int currentPosition = currentCell.GetBoardPosition();

            if (moveType == BasePiece.MoveType.Vertical) {

                List<Vector2Int> topPositions = GetDirectionPositions(Board.Direction.N, moveStep, currentCell, frontDirection);
                topPositions.ForEach(position => result.Add(position));
                List<Vector2Int> bottomPositions = GetDirectionPositions(Board.Direction.S, moveStep, currentCell, frontDirection);
                bottomPositions.ForEach(position => result.Add(position));

            }
            else if (moveType == BasePiece.MoveType.Diagonal) {

                List<Vector2Int> topLeftPositions = GetDirectionPositions(Board.Direction.NW, moveStep, currentCell, frontDirection);
                topLeftPositions.ForEach(position => result.Add(position));
                List<Vector2Int> topRightPositions = GetDirectionPositions(Board.Direction.NE, moveStep, currentCell, frontDirection);
                topRightPositions.ForEach(position => result.Add(position));

                List<Vector2Int> bottomRightPositions = GetDirectionPositions(Board.Direction.SE, moveStep, currentCell, frontDirection);
                bottomRightPositions.ForEach(position => result.Add(position));
                List<Vector2Int> bottomLeftPositions = GetDirectionPositions(Board.Direction.SW, moveStep, currentCell, frontDirection);
                bottomLeftPositions.ForEach(position => result.Add(position));
            }
            else if (moveType == BasePiece.MoveType.Horizontal) {

                List<Vector2Int> leftPositions = GetDirectionPositions(Board.Direction.W, moveStep, currentCell, frontDirection);
                leftPositions.ForEach(position => result.Add(position));

                List<Vector2Int> rightPositions = GetDirectionPositions(Board.Direction.E, moveStep, currentCell, frontDirection);
                rightPositions.ForEach(position => result.Add(position));

            }
            else if (moveType == BasePiece.MoveType.L) {

                List<Vector2Int> lPositions = GetLPositions(currentCell);
                lPositions.ForEach(position => result.Add(position));

            }
            else if (moveType == BasePiece.MoveType.Pawn) {

                List<Vector2Int> pawnPositions = GetPawnPositions(currentCell, frontDirection);

                pawnPositions.ForEach(position => result.Add(position));
            }

            return result;

        }

        // GetGeneralDirection(): given 2 positions (Vector2Int), return the direction of targetPosition relative to originalPosition
        Board.Direction GetGeneralDirection(Vector2Int originalPosition, Vector2Int targetPosition) {

            int originalX = originalPosition.x;
            int originalY = originalPosition.y;
            int targetX = targetPosition.x;
            int targetY = targetPosition.y;

            Board.Direction generalDirection = Board.Direction.None;

            if (targetX < originalX && targetY > originalY) {
                generalDirection = Board.Direction.NW;
            }
            else if (targetX == originalX && targetY > originalY) {
                generalDirection = Board.Direction.N;
            }
            else if (targetX > originalX && targetY > originalY) {
                generalDirection = Board.Direction.NE;
            }
            else if (targetX > originalX && targetY == originalY) {
                generalDirection = Board.Direction.E;
            }
            else if (targetX > originalX && targetY < originalY) {
                generalDirection = Board.Direction.SE;
            }
            else if (targetX == originalX && targetY < originalY) {
                generalDirection = Board.Direction.S;
            }
            else if (targetX < originalX && targetY < originalY) {
                generalDirection = Board.Direction.SW;
            }
            else if (targetX < originalX && targetY == originalY) {
                generalDirection = Board.Direction.W;
            }

            return generalDirection;

        }

        // GetPathPositions(): given 2 positiosn (Vector2Int), return list of positions
        // .. maybe should be in PieceManager...
        List<Vector2Int> GetPathPositions(Vector2Int originalPosition, Vector2Int targetPosition) {

            Debug.Log("PieceManager::GetPathPositions()");

            List<Vector2Int> results = new List<Vector2Int>();

            int originalX = originalPosition.x;
            int originalY = originalPosition.y;
            int targetX = targetPosition.x;
            int targetY = targetPosition.y;

            Board.Direction generalDirection = GetGeneralDirection(originalPosition, targetPosition);
            Debug.Log("general direction is: " + generalDirection);

            // determine if it is a line
            Vector2Int offset = Vector2Int.zero;

            if (generalDirection == Board.Direction.N) {
                offset = Vector2Int.up;
            }
            else if (generalDirection == Board.Direction.E) {
                offset = Vector2Int.right;
            }
            else if (generalDirection == Board.Direction.S) {
                offset = Vector2Int.down;
            }
            else if (generalDirection == Board.Direction.W) {
                offset = Vector2Int.left;
            }
            else if (generalDirection == Board.Direction.NE) {
                if (targetX - originalX == targetY - originalY) {
                    Debug.Log("is a line!");
                    offset = new Vector2Int(1, 1);
                }
            }
            else if (generalDirection == Board.Direction.SE) {
                if (targetX - originalX == -(targetY - originalY)) {
                    Debug.Log("is a line!");
                    offset = new Vector2Int(1, -1);
                }
            }
            else if (generalDirection == Board.Direction.SW) {
                if (targetX - originalX == targetY - originalY) {
                    Debug.Log("is a line!");
                    offset = new Vector2Int(-1, -1);
                }
            }
            else if (generalDirection == Board.Direction.NW) {
                if (-(targetX - originalX) == targetY - originalY) {
                    Debug.Log("is a line");
                    offset = new Vector2Int(-1, 1);
                }
            }

            // construct list
            Vector2Int currentPosition = originalPosition;

            while (true) {

                results.Add(currentPosition);

                // reach target
                if (currentPosition == targetPosition) {
                    break;
                }

                // keep stepping
                Vector2Int nextPosition = currentPosition + offset;
                if (nextPosition == currentPosition) {
                    Debug.Log("not a line!");
                    currentPosition = targetPosition;
                }
                else {
                    currentPosition = nextPosition;
                }
            }

            return results;

        }

        // CreatePieces(): create all pieces for 1 team
        void CreatePieces(Color teamColor) {

            Debug.Log("PieceManager::CreatePieces()");

            // Create pieces
            for (int i = 0; i < mPieceOrder.Length; ++i) {

                Type pieceType = mPieceTypes[mPieceOrder[i]];

                // Create new piece
                GameObject newPieceObject = Instantiate(mPiecePrefab, transform);
                BasePiece newPiece = (BasePiece)newPieceObject.AddComponent(pieceType);
                
                newPiece.Setup(teamColor);

                // Add to list
                if (teamColor == Color.white) {
                    mWhitePieces.Add(newPiece);
                }
                else if (teamColor == Color.black) {
                    mBlackPieces.Add(newPiece);
                }

            }

        }

        // Place(): place a base on a cell
        void Place(BasePiece piece, Cell targetCell) {

            // no need to clear when placing piece on board for the first time
            if (piece.GetCurrentCell()) {
                piece.GetCurrentCell().ClearCurrentPiece();
            }
            piece.ClearCurrentCell();

            piece.transform.position = targetCell.transform.position;
            piece.SetCurrentCell(targetCell);
            targetCell.SetCurrentPiece(piece);

        }

        // PlacePieces():
        void PlacePieces(int pawnRow, int royaltyRow, List<BasePiece> pieces) {

            int dimension = mPieceTotalNumber / 2;

            // Place Pawns
            for (int i = 0; i < dimension; ++i) {
                Place(pieces[i], mBoard.GetAllCells()[i, pawnRow]);
            }

            // Place Royalties
            for (int i = 0; i < dimension; ++i) {
                Place(pieces[i + dimension], mBoard.GetAllCells()[i, royaltyRow]);
            }

        }

        // Move(): move is an attempt! only place if target cell state is empty or enemy
        void Move(BasePiece piece, Cell targetCell) {

            Cell.CellState targetCellState = Cell.InquireCellState(piece, targetCell);

            if (targetCellState == Cell.CellState.Empty) {

                Debug.Log("target cell state is: " + targetCellState);

                // Highlight path to target cell
                Vector2Int origPosition = piece.GetCurrentCell().GetBoardPosition();
                Vector2Int targetPosition = targetCell.GetBoardPosition();
                List<Vector2Int> pathPositions = GetPathPositions(origPosition, targetPosition);
                Color teamColor = piece.GetTeamColor();
                mBoard.HighlightCells(pathPositions, teamColor, false);

                // Move to empty cell
                Place(piece, targetCell);

                if (piece is Pawn) {
                    Pawn pawnPiece = (Pawn)piece;
                    pawnPiece.SetFirstMove(false);
                }

                // Verify if enemy king in check
                VerifyKingInCheck(piece);

                GameManager.Instance.NextTurn();
            }
            else if (targetCellState == Cell.CellState.Friend) {

                Debug.Log("target cell state is: " + targetCellState);
                
                // Return to original cell
                Place(piece, piece.GetCurrentCell());

            }
            else if (targetCellState == Cell.CellState.Enemy) {
                Debug.Log("target cell state is: " + targetCellState);

                // Highlight path to target cell
                Vector2Int origPosition = piece.GetCurrentCell().GetBoardPosition();
                Vector2Int targetPosition = targetCell.GetBoardPosition();
                List<Vector2Int> pathPositions = GetPathPositions(origPosition, targetPosition);
                Color teamColor = piece.GetTeamColor();
                mBoard.HighlightCells(pathPositions, teamColor, false);

                // Maybe not a good idea... looks kinda confusing...
                // // highlight piece to be captured
                // mBoard.HighlightCell(targetPosition, targetCell.GetCurrentPiece().GetTeamColor());

                // Capture
                Kill(targetCell.GetCurrentPiece());

                // Move to empty cell
                Place(piece, targetCell);

                if (piece is Pawn) {
                    Pawn pawnPiece = (Pawn)piece;
                    pawnPiece.SetFirstMove(false);
                }

                // Verify if enemy king in check
                VerifyKingInCheck(piece);

                GameManager.Instance.NextTurn();

            }
        }

        // Kill():
        void Kill(BasePiece piece) {

            Debug.Log("PieceManager::Kill() " + piece);

            // clean up cell reference
            if (piece.GetCurrentCell()) {
                piece.GetCurrentCell().ClearCurrentPiece();
            }
            // clean up base piece reference
            piece.ClearCurrentCell();

            // Don't destory them, because we can reuse them for the next game -> deactivate instead
            // Destroy(piece.gameObject);
            piece.gameObject.SetActive(false);

            // game is over when king is killed
            if (piece is King) {
                GameManager.Instance.GameOver();
            }

        }

        // VerifyKingInCheck(): highlight enemy king position if piece is checking enemy king
        void VerifyKingInCheck(BasePiece piece) {

            if (IsChecking(piece)) {

                // highlight enemy king
                Color teamColor = piece.GetTeamColor();
                if (teamColor == Color.white) {

                    foreach (BasePiece p in mBlackPieces) {

                        if (p is King) {

                            Debug.Log("highlight enemy king!");

                            Vector2Int position = p.GetCurrentCell().GetBoardPosition();

                            mBoard.HighlightCell(position, p.GetTeamColor(), true);

                            return;
                        }
                    }
                }
                else {

                    foreach (BasePiece p in mWhitePieces) {

                        if (p is King) {

                            Debug.Log("highlight enemy king!");

                            Vector2Int position = p.GetCurrentCell().GetBoardPosition();

                            mBoard.HighlightCell(position, p.GetTeamColor(), true);

                            return;
                        }

                    }
                }
            }
        }

        // IsChecking(): return true if a piece is checking enemy king after a valid move
        bool IsChecking(BasePiece piece) {

            Debug.Log("PieceManager::IsChecking()");

            List<Vector2Int> allMovePositions = GetAllMovePositions(piece);

            foreach (Vector2Int position in allMovePositions) {

                if (mBoard.IsEnemyKingInCell(piece, position)) {                    
                    return true;
                }

            }

            return false;

        }

        #endregion

    }

}
