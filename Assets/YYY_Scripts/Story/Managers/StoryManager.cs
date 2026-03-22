using Sunset.Events;
using UnityEngine;

namespace Sunset.Story
{
    public class StoryManager : MonoBehaviour
    {
        #region Static Members
        private static StoryManager _instance;
        public static StoryManager Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = FindFirstObjectByType<StoryManager>();
                if (_instance != null)
                {
                    return _instance;
                }

                GameObject runtimeObject = new GameObject(nameof(StoryManager));
                _instance = runtimeObject.AddComponent<StoryManager>();
                return _instance;
            }
        }
        #endregion

        #region Serialized Fields
        [SerializeField] private StoryPhase initialPhase = StoryPhase.CrashAndMeet;
        [SerializeField] private bool startLanguageDecoded = false;
        #endregion

        #region Public Properties
        public StoryPhase CurrentPhase { get; private set; } = StoryPhase.None;
        public bool IsLanguageDecoded { get; private set; }
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }

            InitializeIfNeeded();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
        #endregion

        #region Public Methods
        public void SetLanguageDecoded(bool isDecoded)
        {
            InitializeIfNeeded();
            IsLanguageDecoded = isDecoded;
        }

        public bool SetPhase(StoryPhase nextPhase)
        {
            InitializeIfNeeded();

            if (nextPhase == StoryPhase.None || nextPhase == CurrentPhase)
            {
                return false;
            }

            StoryPhase previousPhase = CurrentPhase;
            CurrentPhase = nextPhase;

            EventBus.Publish(new StoryPhaseChangedEvent
            {
                PreviousPhase = previousPhase,
                CurrentPhase = CurrentPhase
            });

            return true;
        }

        public bool HasReachedPhase(StoryPhase phase)
        {
            InitializeIfNeeded();

            if (phase == StoryPhase.None)
            {
                return false;
            }

            return CurrentPhase >= phase;
        }

        public void ResetState(StoryPhase phase = StoryPhase.CrashAndMeet, bool isDecoded = false)
        {
            CurrentPhase = phase;
            IsLanguageDecoded = isDecoded;
        }
        #endregion

        #region Private Methods
        private void InitializeIfNeeded()
        {
            if (CurrentPhase != StoryPhase.None)
            {
                return;
            }

            CurrentPhase = initialPhase == StoryPhase.None ? StoryPhase.CrashAndMeet : initialPhase;
            IsLanguageDecoded = startLanguageDecoded;
        }
        #endregion
    }
}
