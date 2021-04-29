

namespace FastRacing
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    using SDD.Events;

    public class LevelManager : Manager<LevelManager>
    {
        [SerializeField] protected GameObject m_CarPrefab;
        [SerializeField] protected GameObject m_GroundPrefab;

        #region Manager implementation
        protected override IEnumerator InitCoroutine()
        {
            yield break;
        }
        #endregion

        public override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        public override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        protected override void GamePlay(GamePlayEvent e)
        {
            var trackPrefab = Resources.Load("Prefabs/Tracks/"+ e.track);
            Instantiate(trackPrefab, new Vector3(0,0,0) ,Quaternion.identity);
            //Mettre en place la voiture
     
        }

        protected override void GameMenu(GameMenuEvent e)
        {
            //penser à Destroy le circuit quand on retourne dans le menu
        }
    }
}
