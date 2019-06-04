using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Com.WatchOutRitoGames.Chess2D {

    // Queen: Inherits abstract class Base Piece
    public class Queen : BasePiece {

        /////////////////////////////////////
        //
        #region MonoBehaviour Callbacks
        //
        /////////////////////////////////////

        // Start(): called once during initialization stage
        void Start() {

            Debug.Log("Queen::Start()");

            // initialize fields for moves
            moveNum = 3;
            moveTypes = new List<MoveType> { MoveType.Diagonal, MoveType.Horizontal, MoveType.Vertical };
            moveSteps = new List<MoveStep> { MoveStep.Multiple, MoveStep.Multiple, MoveStep.Multiple };

        }

        // OnEnable(): called when object is enabled
        void OnEnable() {
        
            Debug.Log("Queen::OnEnable()");

            // Assign methods to delegate
            HandleBeginDrag += PieceManager.Instance.DoBeginDrag;
            HandleDrag += PieceManager.Instance.DoDrag;
            HandleEndDrag += PieceManager.Instance.DoEndDrag;

        }

        // OnDisable(): called once when object is destroyed
        void OnDisable() {

            Debug.Log("Queen::OnDisable()");

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
            Debug.Log("Queen::Setup()");

            // set up color here... but it's kinda messy :o
            this.teamColor = teamColor;
            if (teamColor == Color.white) {
                frontDirection = Board.Direction.N;
            }
            else if (teamColor == Color.black) {
                frontDirection = Board.Direction.S;
            }

            GetComponent<Image>().color = teamColor;
            GetComponent<Image>().sprite = Resources.Load<Sprite>("T_Queen");
        }

        #endregion

    }    

}