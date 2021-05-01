

namespace FastRacing
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    using SDD.Events;

    public class LevelManager : Manager<LevelManager>
    {
        [SerializeField] GameObject m_Car;
        private Object currentTrack;
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
            currentTrack = Instantiate(trackPrefab, new Vector3(0,0,0) ,Quaternion.identity);
            GameObject go = (GameObject)trackPrefab;
            Track t = (Track)go.GetComponent<Track>();
            Vector3 pos = t.getFirstSegment();
            m_Car.transform.position = new Vector3(pos.x, pos.y, pos.z);
            m_Car.GetComponent<Rigidbody>().useGravity = true;
            
     
        }

        protected override void GameMenu(GameMenuEvent e)
        { 
            m_Car.transform.position = new Vector3(0, 4, 0);
            m_Car.transform.Rotate(0, 0, 0); //Ces deux lignes ne fonctionnent pas pour l'instant à voir pq
            m_Car.GetComponent<Rigidbody>().useGravity = false;
            Destroy(currentTrack);
        }
    }
}
