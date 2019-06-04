using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.WatchOutRitoGames.Chess2D {

    // GameOver: Manage UI interaction when game is over
    // .. think of this as a controller
    public class GameOverMenu : MonoBehaviour {

        ///////////////////////////////////////
        //
        #region Public Controller Methods
        //
        ///////////////////////////////////////

        public void PlayAgain() {

            GameManager.Instance.PlayAgain();

        }

        public void Quit() {

            GameManager.Instance.Quit();

        }

        #endregion

    }


}


