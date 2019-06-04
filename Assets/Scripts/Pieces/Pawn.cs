using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Com.WatchOutRitoGames.Chess2D {

    // Pawn: Inherits abstract class Base Piece
    public class Pawn : BasePiece {

        /////////////////////////////////////
        //
        #region Private Fields
        //
        /////////////////////////////////////

        bool firstMove = true;

        #endregion

        /////////////////////////////////////
        //
        #region MonoBehaviour Callbacks
        //
        /////////////////////////////////////

        // Start(): called once during initialization stage
        void Start() {

            Debug.Log("Pawn::Start()");

            // initialize fields for moves
            moveNum = 1;
            moveTypes = new List<MoveType> { MoveType.Pawn };
            moveSteps = new List<MoveStep> { MoveStep.Flexible };

            // initialize state
            firstMove = true;

        }

        // OnEnable(): called when object is enabled
        void OnEnable() {
        
            Debug.Log("Pawn::OnEnable()");

            // Assign methods to delegate
            HandleBeginDrag += PieceManager.Instance.DoBeginDrag;
            HandleDrag += PieceManager.Instance.DoDrag;
            HandleEndDrag += PieceManager.Instance.DoEndDrag;

        }

        // OnDisable(): called once when object is destroyed
        void OnDisable() {

            Debug.Log("Pawn::OnDisable()");

            if (PieceManager.Instance) {
                Debug.Log("PieceManager NOT null");
            }
            else {
                Debug.Log("PieceManager IS null!!!!");
            }

            HandleBeginDrag -= PieceManager.Instance.DoBeginDrag;
            HandleDrag -= PieceManager.Instance.DoDrag;
            HandleEndDrag -= PieceManager.Instance.DoEndDrag;

        }

        #endregion

        /////////////////////////////////////
        //
        #region Implementing Abstract Methods
        //
        /////////////////////////////////////

        // Setup(): Piece manager set up the color of this piece
        public override void Setup(Color teamColor) {
            Debug.Log("Pawn::Setup()");

            // set up color here... but it's kinda messy :o
            this.teamColor = teamColor;
            if (teamColor == Color.white) {
                frontDirection = Board.Direction.N;
            }
            else if (teamColor == Color.black) {
                frontDirection = Board.Direction.S;
            }
            
            GetComponent<Image>().color = teamColor;
            GetComponent<Image>().sprite = Resources.Load<Sprite>("T_Pawn");
        }

        #endregion

        /////////////////////////////////////
        //
        #region Public Methods
        //
        /////////////////////////////////////

        // Reset(): Reset the state of Pawn to beginning of Game
        public void Reset() {
            firstMove = true;
        }

        public bool IsFirstMove() {
            return firstMove;
        }

        public void SetFirstMove(bool firstMove) {
            this.firstMove = firstMove;
        }

        #endregion

    }    

}