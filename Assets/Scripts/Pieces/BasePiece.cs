using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Com.WatchOutRitoGames.Chess2D {

    // BasePiece: Base abstract class for all pieces, store board location
    // .. Publish events for PieceManager
    public abstract class BasePiece : EventTrigger {

        ////////////////////////////////////////////
        //
        #region Enums
        //
        ////////////////////////////////////////////

        public enum MoveType {
            L,
            Pawn,
            Diagonal,
            Horizontal,
            Vertical,
            Castle,
        }

        public enum MoveStep {
            Flexible, Single, Double, Multiple,
        }

        #endregion

        ////////////////////////////////////////////
        //
        #region Private Fields
        //
        ////////////////////////////////////////////

        protected Board.Direction frontDirection;
        protected Color teamColor;

        protected Cell mCurrentCell = null;

        // Write custom IEnumerable class to replace this...
        protected int moveNum;
        protected List<MoveType> moveTypes;
        protected List<MoveStep> moveSteps;

        protected delegate void BasePieceDelegate(BasePiece piece, PointerEventData eventData); // delegate (:
        protected BasePieceDelegate HandleBeginDrag; // private delegate instance
        protected BasePieceDelegate HandleDrag;
        protected BasePieceDelegate HandleEndDrag;

        #endregion

        ////////////////////////////////////////////
        //
        #region Abstract Methods (derived class MUST override)
        //
        ////////////////////////////////////////////

        public abstract void Setup(Color teamColor); // PieceManager calls this!

        // public abstract void ShowPath(); // PieceManager calls this!

        #endregion

        ////////////////////////////////////////////
        //
        #region Public Methods
        //
        ////////////////////////////////////////////

        public void SetCurrentCell(Cell cell) {
            this.mCurrentCell = cell;
        }

        public Cell GetCurrentCell() {
            return mCurrentCell;
        }

        public Board.Direction GetFrontDirection() {
            return frontDirection;
        }

        public int GetMoveNum() {
            return moveNum;
        }

        public List<MoveStep> GetMoveSteps() {
            return moveSteps;
        }

        public List<MoveType> GetMoveTypes() {
            return moveTypes;
        }

        public Color GetTeamColor() {
            return teamColor;
        }

        public void ClearCurrentCell() {
            this.mCurrentCell = null;
        }

        #endregion

        ////////////////////////////////////////////
        //
        #region EventTrigger Callbacks
        //
        ////////////////////////////////////////////

        // OnBeginDrag(): implement dragging a piece
        public override void OnBeginDrag(PointerEventData eventData) {

            base.OnBeginDrag(eventData);

            // draw this last
            transform.SetAsLastSibling();

            // invoke delegate
            HandleBeginDrag(this, eventData);

        }

        // OnDrag(): implement dragging a piece
        public override void OnDrag(PointerEventData eventData) {

            base.OnDrag(eventData);
            
            // move piece to follow pointer
            transform.position += (Vector3)eventData.delta;

            // invoke delegate
            HandleDrag(this, eventData);

        }

        // OnEndDrag(): implement dragging a piece
        public override void OnEndDrag(PointerEventData eventData) {

            base.OnEndDrag(eventData);

            // invoke delegate
            HandleEndDrag(this, eventData);

        }

        #endregion

    }

}