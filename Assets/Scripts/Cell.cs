using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.WatchOutRitoGames.Chess2D {

    // Cell: This is a Cell :D
    public class Cell : MonoBehaviour {

        ///////////////////////////////
        //
        #region Enum
        //
        ///////////////////////////////

        public enum CellState {

            Empty, Friend, Enemy,

        };

        #endregion

        ///////////////////////////////
        //
        #region Private Fields
        //
        ///////////////////////////////

        Vector2Int mBoardPosition = Vector2Int.zero; // board coordinate
        BasePiece mCurrentPiece = null;

        [Tooltip("Outline Image")]
        [SerializeField]
        Image mOutlineImage;

        #endregion

        ///////////////////////////////
        //
        #region Public Methods
        //
        ///////////////////////////////

        // Setup(): store variables
        public void Setup(Vector2Int newBoardPosition) {

            mBoardPosition = newBoardPosition;

        }

        // GetBoardPosition(): return board coordinate
        public Vector2Int GetBoardPosition() {
            return mBoardPosition;
        }

        // GetCurrentPiece()
        public BasePiece GetCurrentPiece() {
            return mCurrentPiece;
        }

        // SetCurrentPiece()
        public void SetCurrentPiece(BasePiece piece) {
            this.mCurrentPiece = piece;
        }

        // ClearCurrentPiece()
        public void ClearCurrentPiece() {
            this.mCurrentPiece = null;
        }

        // IsEmpty()
        public bool IsEmpty() {
            return !mCurrentPiece;
        }

        // InquireCellState()
        public static CellState InquireCellState(BasePiece inquirerPiece, Cell targetCell) {

            Debug.Log("Cell::InquireCellState: " + targetCell.GetBoardPosition());

            if (!targetCell.GetCurrentPiece()) {
                return Cell.CellState.Empty;
            }
            else {

                Color inquirerTeamColor = inquirerPiece.GetTeamColor();
                Color targetTeamColor = targetCell.GetCurrentPiece().GetTeamColor();

                if (inquirerTeamColor == targetTeamColor) {
                    return Cell.CellState.Friend;
                }
                else {
                    return Cell.CellState.Enemy;
                }

            }

        }

        public void Highlight(Color highlightColor) {

            mOutlineImage.color = highlightColor;
            mOutlineImage.enabled = true;

        }

        public void UnHighlight() {

            mOutlineImage.enabled = false;

        }

        #endregion

    }
}
