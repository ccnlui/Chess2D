using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Com.WatchOutRitoGames.Chess2D {

    // King: Inherits abstract class Base Piece
    public class King : BasePiece {

        /////////////////////////////////////
        //
        #region MonoBehaviour Callbacks
        //
        /////////////////////////////////////

        // Start(): called once during initialization stage
        void Start() {

            Debug.Log("King::Start()");

            // initialize fields for moves
            moveNum = 3;
            moveTypes = new List<MoveType> { MoveType.Diagonal, MoveType.Horizontal, MoveType.Vertical };
            moveSteps = new List<MoveStep> { MoveStep.Single, MoveStep.Single, MoveStep.Single };

        }

        // OnEnable(): called when object is enabled
        void OnEnable() {
        
            Debug.Log("King::OnEnable()");

            // Assign methods to delegate
            HandleBeginDrag += PieceManager.Instance.DoBeginDrag;
            HandleDrag += PieceManager.Instance.DoDrag;
            HandleEndDrag += PieceManager.Instance.DoEndDrag;

        }

        // OnDisable(): called once when object is destroyed
        void OnDisable() {

            Debug.Log("King::OnDisable()");

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

        public override void Setup(Color teamColor) {
            Debug.Log("King::Setup()");

            // set up color here... but it's kinda messy :o
            this.teamColor = teamColor;
            if (teamColor == Color.white) {
                frontDirection = Board.Direction.N;
            }
            else if (teamColor == Color.black) {
                frontDirection = Board.Direction.S;
            }

            GetComponent<Image>().color = teamColor;
            GetComponent<Image>().sprite = Resources.Load<Sprite>("T_King");
        }

        #endregion

    }    

}