using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.WatchOutRitoGames.Chess2D {

    // GameManager: Store all game states: who goes first, scores, time limit...
    // .. think of it as a model. Shoult be a singleton... :O
    public class GameManager : MonoBehaviour {

        //////////////////////////////////////
        //
        #region Singleton
        //
        //////////////////////////////////////

        public static GameManager Instance = null;

        #endregion

        //////////////////////////////////////
        //
        #region Private Fields
        //
        //////////////////////////////////////

        [Tooltip("Board game object in scene")]
        [SerializeField]
        Board mBoard;

        [Tooltip("Piece Manager game object in scene")]
        [SerializeField]
        PieceManager mPieceManager;

        [Tooltip("Game Over Menu")]
        [SerializeField]
        GameOverMenu mGameOverMenu;

        Color currentTurnTeamColor;

        bool gameOver;

        #endregion

        //////////////////////////////////////
        //
        #region MonoBehaviour Callbacks
        //
        //////////////////////////////////////

        // Awake(): Called once during pre-initialization stage
        void Awake() {
            Debug.Log("GameManager::Awake()");

            // Singleton: use the first instance!
            if (Instance == null) {
                Instance = this;
            }
            else if (Instance != this) {
                // destroy this to prevent more than 1 GameManager
                Destroy(gameObject);
            }

            // keep the same instance across different scenes
            DontDestroyOnLoad(gameObject);
        }

        // Start(): called once during initialization stage
        void Start() {
            Debug.Log("GameManager::Start()");

            // Create board
            mBoard.Setup();

            // Create pieces, pieceManager needs a reference to board
            mPieceManager.Setup(mBoard);

            // Initialize fields
            gameOver = false;
            currentTurnTeamColor = Color.black; //  white goes first!
            NextTurn();

            // Debug
            mBoard.PrintAllCurrentPieces();

        }

        // Update(): called once every frame
        void Update() {

            if (Input.GetKeyDown(KeyCode.Escape)) {

                if (!gameOver) {
                    mGameOverMenu.gameObject.SetActive(true);
                    gameOver = true;
                }
                else {
                    mGameOverMenu.gameObject.SetActive(false);
                    gameOver = false;
                }

            }

        }

        #endregion

        //////////////////////////////////////
        //
        #region Public Methods
        //
        //////////////////////////////////////

        // NextTurn(): 
        public void NextTurn() {

            Debug.Log("GameManager::NextTurn()");
            
            if (currentTurnTeamColor == Color.white) {

                mPieceManager.DisableTeamPieces(Color.white);
                currentTurnTeamColor = Color.black;
                mPieceManager.EnableTeamPieces(Color.black);
            }
            else if (currentTurnTeamColor == Color.black) {
                mPieceManager.DisableTeamPieces(Color.black);
                currentTurnTeamColor = Color.white;
                mPieceManager.EnableTeamPieces(Color.white);
            }

        }

        // GameOver():
        public void GameOver() {

            gameOver = true;
            mGameOverMenu.gameObject.SetActive(true);

        }

        // PlayAgain(): 
        public void PlayAgain() {

            Debug.Log("GameManager::PlayAgain()");

            // Does not work because didn't design scene objects for them to get destroyed and recreated multiple times!
            // SceneManager.LoadScene( SceneManager.GetActiveScene().name );
            mPieceManager.Reset();

            // hide game over panel
            mGameOverMenu.gameObject.SetActive(false);

            // Initialize fields
            gameOver = false;
            currentTurnTeamColor = Color.black; //  white goes first!
            NextTurn();

        }

        // Quit():
        public void Quit() {

            Debug.Log("GameManager::Quit()");
            Application.Quit();

        }

        #endregion

    }
}
