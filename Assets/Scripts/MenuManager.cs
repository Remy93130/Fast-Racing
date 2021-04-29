
namespace FastRacing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using SDD.Events;
    using UnityEngine.UI;

    public class MenuManager : Manager<MenuManager>
    {

        [Header("MenuManager")]

        #region Panels
        [Header("Panels")]
        [SerializeField] GameObject m_PanelMainMenu;
        [SerializeField] GameObject m_PanelInGameMenu;
        [SerializeField] GameObject m_PanelGameOver;
        [SerializeField] GameObject m_PanelTrackChoice;

        List<GameObject> m_AllPanels;
        #endregion

        #region Events' subscription
        public override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        public override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }
        #endregion

        #region Manager implementation
        protected override IEnumerator InitCoroutine()
        {
            yield break;
        }
        #endregion

        #region Monobehaviour lifecycle
        protected override void Awake()
        {
            base.Awake();
            RegisterPanels();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                EscapeButtonHasBeenClicked();
            }
        }
        #endregion

        #region Panel Methods
        void RegisterPanels()
        {
            m_AllPanels = new List<GameObject>
            {
                m_PanelMainMenu,
                m_PanelInGameMenu,
                m_PanelGameOver,
                m_PanelTrackChoice
            };
        }

        void OpenPanel(GameObject panel)
        {
            foreach (var item in m_AllPanels)
                if (item) item.SetActive(item == panel);
        }
        #endregion

        #region UI OnClick Events
        public void EscapeButtonHasBeenClicked()
        {
            EventManager.Instance.Raise(new EscapeButtonClickedEvent());
        }

        public void PlayButtonHasBeenClicked()
        {
            Dropdown dropdown = m_PanelTrackChoice.GetComponentInChildren<Dropdown>();
            string trackChoice = dropdown.options[dropdown.value].text;
            EventManager.Instance.Raise(new PlayButtonClickedEvent() {track = trackChoice});
        }

        public void PlayMainMenuButtonHasBeenClicked()
        {
            EventManager.Instance.Raise(new PlayMainMenuButtonClickedEvent());
        }

        public void ReturnButtonHasBeenClicked()
        {
            EventManager.Instance.Raise(new ReturnButtonClickedEvent());
        }

        public void ResumeButtonHasBeenClicked()
        {
            EventManager.Instance.Raise(new ResumeButtonClickedEvent());
        }

        public void MainMenuButtonHasBeenClicked()
        {
            EventManager.Instance.Raise(new MainMenuButtonClickedEvent());
        }

        public void QuitButtonHasBeenClicked()
        {
            EventManager.Instance.Raise(new QuitButtonClickedEvent());
        }

        #endregion

        #region Callbacks to GameManager events
        protected override void GameMenu(GameMenuEvent e)
        {
            OpenPanel(m_PanelMainMenu);
        }

        protected override void GamePlayMainMenu(GamePlayMainMenuEvent e)
        {
            OpenPanel(m_PanelTrackChoice);
        }

        protected override void GamePlay(GamePlayEvent e)
        {
            OpenPanel(null);
        }

        protected override void GameReturn(GameReturnEvent e)
        {
            OpenPanel(m_PanelMainMenu);
        }

        protected override void GamePause(GamePauseEvent e)
        {
            OpenPanel(m_PanelInGameMenu);
        }

        protected override void GameResume(GameResumeEvent e)
        {
            OpenPanel(null);
        }

        protected override void GameOver(GameOverEvent e)
        {
            OpenPanel(m_PanelGameOver);
        }
        #endregion
    }

}
